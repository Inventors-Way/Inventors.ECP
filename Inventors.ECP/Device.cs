using Inventors.ECP.Communication;
using Inventors.ECP.Functions;
using Inventors.ECP.Messages;
using Inventors.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Inventors.ECP
{
    public abstract class Device
    {
        private bool connected = false;
        private readonly object lockObject = new object();

        [Browsable(false)]
        public abstract DeviceState State { get; }

        public event EventHandler<MessageEventArgs<PrintfMessage>> OnPrintf;
        public event EventHandler<DeviceState> OnStateChanged;

        public Device(CommunicationLayer commLayer, DeviceType device)
        {
            CommLayer = commLayer;
            this.Master = new DeviceMaster(commLayer, device);
            Master.MessageListener = this;
            Master.Add(new PrintfMessage());
        }

        public List<DeviceType> GetAvailableDevices()
        {
            if (CommLayer.IsOpen)
            {
                throw new InvalidOperationException();
            }

            var retValue = new List<DeviceType>();
            var currentPort = CommLayer.Port;

            foreach (var port in CommLayer.GetAvailablePorts())
            {
                try
                {
                    CommLayer.Port = port;
                    var devId = Connect();
                    var devType = new DeviceType(port, devId);

                    if (IsCompatible(devId))
                    {
                        retValue.Add(devType);
                        Log.Debug("[ {0} ]: {1}", port, devType.ToString());
                    }
                    else
                    {
                        Log.Debug("[ {0} ]: Incompatible device found [ {1} ]", port, devType.ToString());
                    }

                    Close();
                }
                catch (Exception ex)
                {
                    Close();
                    Log.Debug("[ {0} ]: No compatible device found ({1})", port, ex.Message);
                }
            }

            CommLayer.Port = currentPort;

            return retValue;
        }

        public async Task<List<DeviceType>> GetAvailableDevicesAsync() =>
            await Task.Run(() => GetAvailableDevices()).ConfigureAwait(false);

        public virtual DeviceIdentification Connect()
        {
            DeviceIdentification retValue = null;

            if (!Master.IsOpen)
            {
                retValue = new DeviceIdentification();
                Master.Open();
                WaitOnConnected(200);
                Execute(retValue);

                if (IsCompatible(retValue))
                {
                    Connected = true;
                }
                else
                {
                    Master.Close();    
                    throw new IncompatibleDeviceException(retValue.ToString());
                }
            }

            return retValue;
        }

        private void WaitOnConnected(int timeout)
        {
            var watch = new Stopwatch();
            watch.Restart();

            while (!CommLayer.IsConnected)
            {
                if (watch.ElapsedMilliseconds > timeout)
                {
                    throw new SlaveNotRespondingException(string.Format(CultureInfo.CurrentCulture, "Could not connect to: {0}", CommLayer.Port));
                }
            }
            watch.Stop();
            Log.Debug("Time to connect: {0}", watch.ElapsedMilliseconds);
        }


        public async Task ConnectAsync() => await Task.Run(() => Connect()).ConfigureAwait(false);

        public virtual void Disconnect()
        {
            if (Master.IsOpen)
            {
                Master.Close();
                Connected = false;
            }
        }

        public async Task DisconnectAsync() => await Task.Run(() => Disconnect()).ConfigureAwait(false);

        [Browsable(false)]
        [XmlIgnore]
        public bool Connected
        {
            get
            {
                lock (lockObject)
                {
                    return connected;
                }
            }
            set
            {
                lock (lockObject)
                {
                    if (connected != value)
                    {
                        connected = value;
                    }
                }
            }
        }

        [Category("Communication Layer")]
        [XmlIgnore]
        public bool ResetOnConnection
        {
            get
            {
                return Master.ResetOnConnection;
            }
            set
            {
                Master.ResetOnConnection = value;
            }
        } 

        public void Open()
        {
            if (!Master.IsOpen)
            {
                Master.Open();                
            }
        }

        public void Close()
        {
            if (Master.IsOpen)
            {
                Master.Close();
            }
        }

        [Browsable(false)]
        public bool IsOpen
        {
            get
            {
                return Master.IsOpen;
            }
        }

        public void Execute(DeviceFunction function)
        {
            if (function is object)
            {
                for (int n = 0; n < Retries; ++n)
                {
                    try
                    {
                        watch.Restart();
                        Master.Execute(function);
                        watch.Stop();
                        function.TransmissionTime = watch.ElapsedMilliseconds;
                        break;
                    }
                    catch { if (n == Retries - 1) { throw; } }
                }
            }
        }

        public async Task ExecuteAsync(DeviceFunction function) =>
            await Task.Run(() => { Execute(function); }).ConfigureAwait(false);

        public void Send(DeviceMessage message)
        {
            Master.Send(message);
        }

        public void Accept(PrintfMessage message)
        {
            OnPrintf?.Invoke(this, new MessageEventArgs<PrintfMessage>(message));
        }

        protected void NotifyPropertyListeners()
        {
            OnStateChanged?.Invoke(this, State);
        }

        public override string ToString()
        {
            return "ECP DEFAULT DEVICE";
        }

        [Browsable(false)]
        public List<DeviceFunction> Functions
        {
            get
            {
                return FunctionList;
            }
        }

        public abstract bool IsCompatible(DeviceIdentification identification);

        [Browsable(false)]
        public DeviceMaster Master { get; }

        [Category("Communication Layer")]
        [XmlIgnore]
        public int Timeout
        {
            get
            {
                return Master.Timeout;
            }
            set
            {
                Master.Timeout = value;
            }
        }

        [Category("Retries")]
        public int Retries { get; set; } = 1;

        [XmlIgnore]
        [Browsable(false)]
        public CommunicationLayer CommLayer { get; private set; }

        [XmlIgnore]
        [Browsable(false)]
        protected List<DeviceFunction> FunctionList { get; } = new List<DeviceFunction>();

        [XmlIgnore]
        [Browsable(false)]
        protected List<MessageDispatcher> Dispatchers { get; } = new List<MessageDispatcher>();

        private readonly Stopwatch watch = new Stopwatch();
    }
}
