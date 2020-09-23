using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Inventors.ECP.Communication;
using Inventors.ECP.Profiling;

namespace Inventors.ECP
{
    public class DeviceMaster :
        IDisposable
    {
        private enum CommState
        {
            IDLE,
            WAITING,
            COMPLETED,
            ERROR
        };

        public int Timeout { get; set; }

        public Location Location
        {
            get => connection.Location;
            set => connection.Location = value;
        }

        public int BaudRate
        {
            get => connection.BaudRate;
            set => connection.BaudRate = value;
        }

        public bool IsConnected => connection.IsConnected;

        public bool IsOpen => connection.IsOpen;

        public bool ResetOnConnection
        {
            get => connection.ResetOnConnection;
            set => connection.ResetOnConnection = value;
        }

        public IProfiler Profiler { get; }

        public double RxRate => connection.RxRate;

        public double TxRate => connection.TxRate;

        public DeviceMaster(CommunicationLayer connection, IProfiler profiler)
        {
            if (connection is null)
                throw new ArgumentException(Resources.CONNECTION_NULL);

            this.connection = connection;
            connection.Destuffer.OnReceive += HandleIncommingFrame;
            Timeout = 500;
            Profiler = profiler;
        }

        /// <summary>
        /// Open the communication with a device.
        /// </summary>
        public void Open() => connection.Open();

        /// <summary>
        /// Close the communication with a device.
        /// </summary>
        public void Close() => connection.Close();

        #region Execution of device functions

        /// <summary>
        /// Execute a function.
        /// </summary>
        /// <param name="function">The function to execute.</param>
        public void Execute(DeviceFunction function)
        {
            if (function is object)
            {
                lock (commLock)
                {
                    function.OnSend();
                    Initiate(function);

                    while (!IsCompleted()) ;

                    state = CommState.WAITING;

                    if (currentException != null)
                        throw currentException;
                }
            }
        }

        private bool IsCompleted()
        {
            bool retValue = false;

            if (stopwatch.ElapsedMilliseconds < Timeout)
            {
                lock (lockObject)
                {
                    if (state != CommState.WAITING)
                    {
                        retValue = true;
                    }
                }
            }
            else
            {
                currentException = new SlaveNotRespondingException(Resources.NO_RESPONSE);
                retValue = true;
            }

            return retValue;
        }

        private void Initiate(DeviceFunction function)
        {
            var bytes = function.GetRequest();

            lock (lockObject)
            {
                stopwatch.Restart();
                current = function;
                state = CommState.WAITING;
                currentException = null;
            }

            connection.Transmit(Frame.Encode(bytes));
        }

        #endregion
        #region Sending messages

        /// <summary>
        /// Send an unacknowledged message to the device.
        /// </summary>
        /// <param name="message">The message to send</param>
        public void Send(DeviceMessage message)
        {
            if (connection.IsOpen && (message is object))
            {
                lock (commLock)
                {
                    connection.Transmit(Frame.Encode(message.GetPacket()));
                }
            }
        }

        #endregion

        public void RestartStatistics() => connection.RestartStatistics();


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        private void HandleIncommingFrame(Destuffer caller, byte[] frame)
        {
            try
            {
                var response = new Packet(frame);
                Profiler.Add(response);

                if (response.Code != 0x00)
                {
                    if (response.IsFunction)
                    {
                        lock (lockObject)
                        {
                            if (current != null)
                            {
                                current.SetResponse(response);
                                current.OnReceived();
                            }

                            state = CommState.COMPLETED;
                        }
                    }
                    else
                    {
                        try
                        {
                            Dispatch(response);
                        }
                        catch (Exception e)
                        {
                            Log.Error(e.Message);
                            Profiler.Add(new TargetEvent(e.Message));
                        }
                    }
                }
                else
                {
                    
                    lock (lockObject)
                    {
                        currentException = new UnknownFunctionCallException(String.Format(CultureInfo.CurrentCulture, "{0}", response.GetByte(0)));
                        state = CommState.ERROR;
                    }
                }
            }
            catch (Exception e)
            {
                Log.Debug("Error in creating Packet: {0}", e.Message);
            }
        }

        private void Dispatch(Packet packet)
        {
            if (Dispatchers.ContainsKey(packet.Code) && (MessageListener is object))
            {
                Dispatchers[packet.Code].Create(packet).Dispatch(MessageListener);
            }
        }

        public void Add(DeviceMessage message)
        {
            if (message is null)
                throw new ArgumentNullException(nameof(message));

            if (Dispatchers.ContainsKey(message.Code))
                throw new ArgumentException($"Message [ { message } ] is allready present in Dispatchers");

            Dispatchers.Add(message.Code, message.CreateDispatcher());
        }

        private Dictionary<byte, MessageDispatcher> Dispatchers { get; } = new Dictionary<byte, MessageDispatcher>();

        public List<Location> GetLocations() => connection.GetLocations();

        #region Dispose Pattern
        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (IsOpen)
                    {
                        Close();
                    }
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion

        /// <summary>
        /// The message listener.
        /// </summary>
        public dynamic MessageListener { get; set; }

        private readonly CommunicationLayer connection;
        private DeviceFunction current;
        private readonly object lockObject = new object();
        private readonly object commLock = new object();
        private Exception currentException;
        private readonly Stopwatch stopwatch = new Stopwatch();
        private CommState state = CommState.WAITING;
    }
}
