using Inventors.ECP.Functions;
using Inventors.ECP.Messages;
using Serilog;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Text;

namespace Inventors.ECP
{
    public class BusPeripheral :
        DynamicObject,
        IDisposable
    {
        private List<MessageDispatcher> MessageDispatchers { get; } = new List<MessageDispatcher>();
        private List<FunctionDispatcher> FunctionDispatchers { get; } = new List<FunctionDispatcher>();

        public dynamic MessageListener { get; set; } 

        public dynamic FunctionListener { get; set; }

        public string Location
        {
            get => _connection.Location;
            set => _connection.Location = value;
        }

        public int BaudRate
        {
            get => _connection.BaudRate;
            set => _connection.BaudRate = value;
        }

        public bool ResetOnConnection
        {
            get => _connection.ResetOnConnection;
            set => _connection.ResetOnConnection = value;
        }

        public bool IsOpen => _connection.IsOpen;

        public virtual DeviceAddress Address => null;

        public BusPeripheral(CommunicationLayer layer)
        {
            if (layer is null)
                throw new ArgumentException(Resources.LAYER_OR_DEVICE_DATA_IS_NULL);

            _connection = layer;
            _connection.Destuffer.OnReceive += HandleIncommingFrame;
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

        public void Send(DeviceMessage message)
        {
            if (_connection.IsOpen && (message is object))
            {
                message.OnSend();
                _connection.Transmit(Frame.Encode(message.GetPacket(Address)));
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
                        try
                        {
                            int errorCode = DispatchFunction(response, out DeviceFunction function);

                            if (errorCode == 0)
                            {
                                function.OnSlaveSend();
                                _connection.Transmit(Frame.Encode(function.GetResponse()));
                            }
                            else
                            {
                                Packet nack = new Packet(0, 1);
                                nack.InsertByte(0, (byte)errorCode);

                                _connection.Transmit(Frame.Encode(nack.ToArray()));
                            }
                        }
                        catch (Exception e)
                        {
                            Packet nack = new Packet(0, 1);
                            nack.InsertByte(0, (byte) ErrorCode.DISPATCH_ERR);

                            _connection.Transmit(Frame.Encode(nack.ToArray()));
                            Log.Debug($"{e.GetType()} => {e.Message}");
                        }
                    }
                    else
                    {
                        if (!DispatchMessage(response))
                        {
                            if (ECPLog.Enabled)
                                Log.Debug("MESSAGE [0x{0:X}] ERROR NO DISPATCHER FOUND", response.Code);
                        }
                    }
                }
                else
                {
                    if (ECPLog.Enabled)
                        Log.Debug("Received a NACK, should be impossible for a slave");
                }
            }
            catch (Exception e)
            {
                if (ECPLog.Enabled)
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

        private bool DispatchMessage(Packet packet)
        {
            bool retValue = false;

            foreach (var displatcher in MessageDispatchers)
            {
                if ((displatcher.Code == packet.Code) &&
                    (MessageListener is object))
                {
                    var message = displatcher.Create(packet);
                    message.Dispatch(MessageListener);
                    retValue = true;
                }
            }

            return retValue;
        }

        private int DispatchFunction(Packet packet, out DeviceFunction function)
        {
            int retValue = 0;
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

        private readonly CommunicationLayer _connection;
        private bool disposedValue;
    }
}
