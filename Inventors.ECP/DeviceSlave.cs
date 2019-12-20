using Inventors.ECP.Messages;
using Inventors.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inventors.ECP
{
    public class DeviceSlave
    {

        private List<MessageDispatcher> MessageDispatchers { get; } = new List<MessageDispatcher>();
        private List<FunctionDispatcher> FunctionDispatchers { get; } = new List<FunctionDispatcher>();

        public dynamic MessageListener { get; set; } = null;

        public dynamic FunctionListener { get; set; } = null;

        public DeviceSlave(CommunicationLayer layer)
        {
            connection = layer;
            connection.Destuffer.OnReceive += HandleIncommingFrame;
        }

        public void Open()
        {
            connection.Open();
        }

        public void Close()
        {
            connection.Close();
        }

        public void Printf(string format, params object[] args)
        {
            Send(new PrintfMessage
            {
                DebugMessage = String.Format(format, args)
            });
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

        public void Send(Message message)
        {
            if (connection.IsOpen)
            {
                message.OnSend();
                connection.Transmit(Frame.Encode(message.GetPacket()));
            }
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
                        if (DispatchFunction(response, out Function function))
                        {
                            function.OnSlaveSend();
                            connection.Transmit(Frame.Encode(function.GetResponse()));
                        }
                        else
                        {
                            Packet nack = new Packet(0, 1);
                            nack.InsertByte(0, 0);

                            connection.Transmit(Frame.Encode(nack.ToArray()));
                        }
                    }
                    else
                    {
                        if (!DispatchMessage(response))
                        {
                            Log.Debug("MESSAGE [0x{0:X}] ERROR NO DISPATCHER FOUND", response.Code);
                        }
                    }
                }
                else
                {
                    Log.Debug("Received a NACK, should be impossible for a slave");
                }
            }
            catch (Exception e)
            {
                Log.Debug("Error in creating Packet: {0}", e.Message);
            }
        }

        public void Add(Function function)
        {
            FunctionDispatchers.Add(function.CreateDispatcher());
        }

        public void Add(Message message)
        {
            MessageDispatchers.Add(message.CreateDispatcher());
        }

        public bool DispatchMessage(Packet packet)
        {
            bool retValue = false;

            foreach (var displatcher in MessageDispatchers)
            {
                if ((displatcher.Code == packet.Code) &&
                    (MessageListener != null))
                {
                    displatcher.Create(packet).Dispatch(MessageListener);
                    retValue = true;
                }
            }

            return retValue;
        }

        public bool DispatchFunction(Packet packet, out Function function)
        {
            bool retValue = false;
            function = null;

            foreach (var displatcher in FunctionDispatchers)
            {
                if ((displatcher.Code == packet.Code) &&
                    (FunctionListener != null))
                {
                    function = displatcher.Create(packet);
                    retValue = function.Dispatch(FunctionListener);
                }
            }

            return retValue;
        }

        private readonly CommunicationLayer connection;
    }
}
