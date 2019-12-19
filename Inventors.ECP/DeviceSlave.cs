using Inventors.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inventors.ECP
{
    public class DeviceSlave
    {

        public List<MessageDispatcher> MessageDispatchers { get; } = new List<MessageDispatcher>();
        public List<FunctionDispatcher> FunctionDispatchers { get; } = new List<FunctionDispatcher>();

        public dynamic Listener { get; set; } = null;

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
                    if (response.IsFunction ? DispatchFunction(response) : DispatchMessage(response))
                    {
                        Log.Debug("PACKET [0x{0:X}] Dispatched", response.Code);
                    }
                    else
                    {
                        Log.Debug("PACKET [0x{0:X}] ERROR NO DISPATCHER FOUND", response.Code);
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

        public bool DispatchMessage(Packet packet)
        {
            bool retValue = false;

            foreach (var displatcher in MessageDispatchers)
            {
                if ((displatcher.Code == packet.Code) &&
                    (Listener != null))
                {
                    displatcher.Create(packet).Dispatch(Listener);
                    retValue = true;
                }
            }

            return retValue;
        }

        public bool DispatchFunction(Packet packet)
        {
            bool retValue = false;

            foreach (var displatcher in FunctionDispatchers)
            {
                if ((displatcher.Code == packet.Code) &&
                    (Listener != null))
                {
                    displatcher.Create(packet).Dispatch(Listener);
                    retValue = true;
                }
            }

            return retValue;
        }

        private readonly CommunicationLayer connection;
    }
}
