using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Inventors.ECP.Communication;
using Inventors.Logging;

namespace Inventors.ECP
{
    public class DeviceMaster
    {
        private enum CommState
        {
            IDLE,
            WAITING,
            COMPLETED,
            ERROR
        };

        public DeviceMaster(CommunicationLayer connection, DeviceType device)
        {
            if (!(connection is object))
                throw new ArgumentException(Resources.CONNECTION_NULL);

            if (!(connection is object))
                throw new ArgumentException(Resources.DEVICE_NULL);

            this.connection = connection;
            this.device = device;
            connection.Destuffer.OnReceive += HandleIncommingFrame;
            Timeout = 500;            
        }


        public void Open()
        {
            connection.Open();
        }

        public void Close()
        {
            connection.Close();
        }

        public bool IsOpen
        {
            get
            {
                return connection.IsOpen;
            }
        }

        public bool ResetOnConnection
        {
            get
            {
                return connection.ResetOnConnection;
            }
            set
            {
                connection.ResetOnConnection = value;
            }
        }

        public void Execute(DeviceFunction function)
        {
            if (function is object)
            {
                function.OnSend();
                Initiate(function);

                while (!IsCompleted()) ;

                state = CommState.WAITING;

                if (currentException != null)
                    throw currentException;
            }
        }

        public void Send(DeviceMessage message)
        {
            if (connection.IsOpen && (message is object))
            {
                connection.Transmit(Frame.Encode(message.GetPacket()));
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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        private void HandleIncommingFrame(Destuffer caller, byte[] frame)
        {
            try
            {
                var response = new Packet(frame);

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

        public int Timeout { get; set; }

        private void Dispatch(Packet packet)
        {
            foreach (var displatcher in Dispatchers)
            {
                if ((displatcher.Code == packet.Code) &&
                    (MessageListener != null))
                {
                    displatcher.Create(packet).Dispatch(MessageListener);
                }
            }
        }

        public void Add(DeviceMessage message)
        {
            if (message is object)
            {
                Dispatchers.Add(message.CreateDispatcher());
            }
        }

        private List<MessageDispatcher> Dispatchers { get; } = new List<MessageDispatcher>();

        public dynamic MessageListener { get; set; } = null;

        private readonly CommunicationLayer connection;
        private DeviceFunction current = null;
        private readonly object lockObject = new object();
        private Exception currentException = null;
        private readonly Stopwatch stopwatch = new Stopwatch();
        private CommState state = CommState.WAITING;
        private readonly DeviceType device;
    }
}
