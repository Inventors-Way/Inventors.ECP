using Inventors.ECP.Communication;
using Inventors.ECP.Functions;
using Inventors.ECP.Messages;
using Inventors.ECP.Profiling;
using Inventors.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Inventors.ECP
{
    public abstract class Device :
        NotifyPropertyChanged
    {
        #region Properties
        #region PrintLevel
        private LogLevel _printLevel = LogLevel.DEBUG;
        [Category("Debug")]
        [XmlIgnore]
        public LogLevel PrintLevel
        {
            get => _printLevel;
            set => SetProperty(ref _printLevel, value);
        }
        #endregion
        #region Connected
        private bool _connected = false;

        [Browsable(false)]
        [XmlIgnore]
        public bool Connected
        {
            get
            {
                lock (LockObject)
                {
                    return _connected;
                }
            }
            set => SetPropertyLocked(ref _connected, value);
        }
        #endregion
        #region DeviceMaster
        [Browsable(false)]
        [XmlIgnore]
        public DeviceMaster Master { get; }
        #endregion
        #region ResetOnConnected
        [Category("Communication Layer")]
        [XmlIgnore]
        public bool ResetOnConnection
        {
            get => Master.ResetOnConnection;
            set => Master.ResetOnConnection = NotifyIfChanged(Master.ResetOnConnection, value);
        }
        #endregion
        #region Timeout
        [Category("Communication Layer")]
        [XmlIgnore]
        public int Timeout
        {
            get => Master.Timeout;
            set => Master.Timeout = NotifyIfChanged(Master.Timeout, value);
        }
        #endregion
        #region IsOpen
        [Browsable(false)]
        [XmlIgnore]
        public bool IsOpen => Master.IsOpen;
        #endregion
        #region IsConnected
        [Browsable(false)]
        public bool IsConnected => Master.IsConnected;

        #endregion
        #region Port
        [Browsable(false)]
        [XmlIgnore]
        public string Port
        {
            get => Master.Port;
            set => Master.Port = NotifyIfChanged(Master.Port, value);
        }
        #endregion
        #region BaudRate
        [Browsable(false)]
        [XmlIgnore]
        public int BaudRate
        {
            get => Master.BaudRate;
            set => Master.BaudRate = NotifyIfChanged(Master.BaudRate, value);
        }
        #endregion
        #region Profiler
        [Browsable(false)]
        [XmlIgnore]
        public Profiler Profiler => Master.Profiler;
        #endregion
        #endregion

        protected Device(CommunicationLayer commLayer)
        {
            this.Master = new DeviceMaster(commLayer);
            Master.MessageListener = this;
            Master.Add(new PrintfMessage());
        }

        public List<DeviceType> GetAvailableDevices()
        {
            if (Master.IsOpen)
            {
                throw new InvalidOperationException();
            }

            var retValue = new List<DeviceType>();
            var currentPort = Master.Port;

            foreach (var port in Master.GetAvailablePorts())
            {
                try
                {
                    Master.Port = port;
                    var devId = Connect();
                    var devType = new DeviceType(port, devId);

                    if (IsCompatible(devId))
                    {
                        retValue.Add(devType);
                    }

                    Close();
                }
                catch 
                {
                    Close();
                }
            }

            Master.Port = currentPort;

            return retValue;
        }

        public CommunicationLayerStatistics GetStatistics() => Master.GetStatistics();

        public async Task<List<DeviceType>> GetAvailableDevicesAsync() =>
            await Task.Run(() => GetAvailableDevices()).ConfigureAwait(false);

        public virtual DeviceIdentification Connect()
        {
            DeviceIdentification retValue = null;

            if (!Master.IsOpen)
            {
                var retries = Retries;
                Retries = 3;
                retValue = new DeviceIdentification();
                Master.Open();
                WaitOnConnected(200);
                Execute(retValue);
                Retries = retries;

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

            while (!Master.IsConnected)
            {
                if (watch.ElapsedMilliseconds > timeout)
                {
                    throw new SlaveNotRespondingException(string.Format(CultureInfo.CurrentCulture, "Could not connect to: {0}", Port));
                }
            }
            watch.Stop();
        }


        public async Task<DeviceIdentification> ConnectAsync() => 
            await Task.Run(() => Connect()).ConfigureAwait(false);

        public virtual void Disconnect()
        {
            if (Master.IsOpen)
            {
                Master.Close();
                Connected = false;
            }
        }

        public async Task DisconnectAsync() => await Task.Run(() => Disconnect()).ConfigureAwait(false);

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
            if (message is object)
            {
                switch (PrintLevel)
                {
                    case LogLevel.DEBUG: Log.Debug(message.DebugMessage); break;
                    case LogLevel.STATUS: Log.Status(message.DebugMessage); break;
                    case LogLevel.ERROR: Log.Error(message.DebugMessage); break;
                    default:
                        break;
                }
            }
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

        private int _retries = 1;

        [Category("Retries")]
        public int Retries 
        {
            get => _retries;
            set => SetProperty(ref _retries, value);
        }

        [XmlIgnore]
        [Browsable(false)]
        protected List<DeviceFunction> FunctionList { get; } = new List<DeviceFunction>();

        [XmlIgnore]
        [Browsable(false)]
        protected List<MessageDispatcher> Dispatchers { get; } = new List<MessageDispatcher>();

        private readonly Stopwatch watch = new Stopwatch();
    }
}
