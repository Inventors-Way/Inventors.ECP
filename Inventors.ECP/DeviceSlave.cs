using Inventors.ECP.Functions;
using Inventors.ECP.Messages;
using Inventors.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Inventors.ECP
{
    public class DeviceSlave
    {

        private List<MessageDispatcher> MessageDispatchers { get; } = new List<MessageDispatcher>();
        private List<FunctionDispatcher> FunctionDispatchers { get; } = new List<FunctionDispatcher>();

        public dynamic MessageListener { get; set; } = null;

        public dynamic FunctionListener { get; set; } = null;

        public DeviceSlave(CommunicationLayer layer, DeviceData deviceData)
        {
            _connection = layer;
            _deviceData = deviceData;
            _connection.Destuffer.OnReceive += HandleIncommingFrame;
        }

        public void Open()
        {
            _connection.Open(_deviceData);
        }

        public void Close()
        {
            _connection.Close();
        }

        public void Printf(string format, params object[] args)
        {
            Send(new PrintfMessage
            {
                DebugMessage = String.Format(CultureInfo.CurrentCulture, format, args)
            });
        }

        public bool IsOpen
        {
            get
            {
                return _connection.IsOpen;
            }
        }

        public bool ResetOnConnection
        {
            get
            {
                return _connection.ResetOnConnection;
            }
            set
            {
                _connection.ResetOnConnection = value;
            }
        }

        public void Send(Message message)
        {
            if (_connection.IsOpen && (message is object))
            {
                message.OnSend();
                _connection.Transmit(Frame.Encode(message.GetPacket()));
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
                            _connection.Transmit(Frame.Encode(function.GetResponse()));
                        }
                        else
                        {
                            Packet nack = new Packet(0, 1);
                            nack.InsertByte(0, 0);

                            _connection.Transmit(Frame.Encode(nack.ToArray()));
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
            if (function is object)
            {
                FunctionDispatchers.Add(function.CreateDispatcher());
            }
        }

        public void Add(Message message)
        {
            if (message is object)
            {
                MessageDispatchers.Add(message.CreateDispatcher());
            }
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

        public void Accept(DeviceIdentification func)
        {
            if (func is object)
            {
                func.DeviceID = _deviceData.DeviceID;
                func.ManufactureID = _deviceData.ManufactureID;
                func.Manufacture = _deviceData.Manufacture;
                func.Device = _deviceData.Device;
                func.MajorVersion = _deviceData.MajorVersion;
                func.MinorVersion = _deviceData.MinorVersion;
                func.PatchVersion = _deviceData.PatchVersion;
                func.EngineeringVersion = _deviceData.EngineeringVersion;
                func.Checksum = _deviceData.Checksum;
                func.SerialNumber = _deviceData.SerialNumber;
            }
        }

        private readonly CommunicationLayer _connection;
        private readonly DeviceData _deviceData;
    }
}
