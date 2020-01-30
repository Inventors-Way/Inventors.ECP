using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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

        public DeviceMaster(CommunicationLayer connection, DeviceData device)
        {
            this.connection = connection;
            this.device = device;
            connection.Destuffer.OnReceive += HandleIncommingFrame;
            Timeout = 500;            
        }


        public void Open()
        {
            connection.Open(device);
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

        public void Execute(Function function)
        {
            function.OnSend();
            Initiate(function);

            while (!IsCompleted());

            state = CommState.WAITING;

            if (currentException != null)
                throw currentException;
        }

        public void Send(Message message)
        {
            if (connection.IsOpen)
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
                currentException = new SlaveNotRespondingException("No response from the slave");
                retValue = true;
            }

            return retValue;
        }

        private void Initiate(Function function)
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
                        currentException = new UnknownFunctionCallException(String.Format("{0}", response.GetByte(0)));
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

        public void Add(Message message)
        {
            Dispatchers.Add(message.CreateDispatcher());
        }

        private List<MessageDispatcher> Dispatchers { get; } = new List<MessageDispatcher>();

        public dynamic MessageListener { get; set; } = null;

        private readonly CommunicationLayer connection;
        private Function current = null;
        private readonly object lockObject = new object();
        private Exception currentException = null;
        private readonly Stopwatch stopwatch = new Stopwatch();
        private CommState state = CommState.WAITING;
        private Queue<Packet> messages = new Queue<Packet>();
        private DeviceData device;
    }
}
