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
        public Location Location
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
            Master = new DeviceMaster(commLayer)
            {
                MessageListener = this
            };
            Master.Add(new PrintfMessage());
        }

        public List<Location> GetLocationsDevices() => Master.GetLocations();

        public CommunicationLayerStatistics GetStatistics() => Master.GetStatistics();

        public DeviceIdentification Connect()
        {
            DeviceIdentification retValue = null;

            if (!Master.IsOpen)
            {
                var retries = Retries;
                Retries = Retries > 3 ? Retries : 3;
                retValue = new DeviceIdentification();

                try
                {
                    Master.Open();
                    WaitOnConnected(200);
                    Execute(retValue);
                }
                catch
                {
                    Master.Close();
                    Retries = retries;
                    throw;
                }

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
                    throw new SlaveNotRespondingException(string.Format(CultureInfo.CurrentCulture, "Could not connect to: {0}", Location));
                }
            }
            watch.Stop();
        }

        /// <summary>
        /// Disconnects a device.
        /// </summary>
        public virtual void Disconnect()
        {
            if (Master.IsOpen)
            {
                Master.Close();
                Connected = false;
            }
        }

        /// <summary>
        /// Open the location of device, but does not check if a device is present by connecting to it.
        /// </summary>
        public void Open()
        {
            if (!Master.IsOpen)
            {
                Master.Open();                
            }
        }

        /// <summary>
        /// Closes the location of the device.
        /// </summary>
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
