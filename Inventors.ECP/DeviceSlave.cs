using Inventors.ECP.Communication;
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

        public DeviceType Identification => _deviceData;

        public DeviceSlave(CommunicationLayer layer, DeviceType deviceData)
        {
            if ((layer is object) && (deviceData is object))
            {
                _connection = layer;
                _deviceData = deviceData;
                _connection.Destuffer.OnReceive += HandleIncommingFrame;
            }
            else
                throw new ArgumentException(Resources.LAYER_OR_DEVICE_DATA_IS_NULL);
        }

        public void Open()
        {
            _connection.Open();
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

        public void Send(DeviceMessage message)
        {
            if (_connection.IsOpen && (message is object))
            {
                message.OnSend();
                _connection.Transmit(Frame.Encode(message.GetPacket()));
            }
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
                        if (DispatchFunction(response, out DeviceFunction function))
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

        public void Add(DeviceFunction function)
        {
            if (function is object)
            {
                FunctionDispatchers.Add(function.CreateDispatcher());
            }
        }

        public void Add(DeviceMessage message)
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

        public bool DispatchFunction(Packet packet, out DeviceFunction function)
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
        private readonly DeviceType _deviceData;
    }
}
