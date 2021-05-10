using Inventors.ECP.Functions;
using Inventors.ECP.Messages;
using Inventors.ECP.Profiling;
using Inventors.ECP.Utility;
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
        NotifyPropertyChanged,
        IDisposable
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
        private bool _connected;

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
        public BusCentral Master { get; }
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
        public string Location
        {
            get => Master.Location;
            set => Master.Location = NotifyIfChanged(Master.Location, value);
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
        #region PingEnabled
        private bool _pingEnabled;

        [Category("Debug")]
        [XmlIgnore]
        public bool PingEnabled
        {
            get => _pingEnabled;
            set => SetProperty(ref _pingEnabled, value);
        }

        #endregion
        #region Profiler
        [Browsable(false)]
        [XmlIgnore]
        public Profiler Profiler => Master.Profiler;
        #endregion
        #region AvailableAdresses

        public virtual List<DeviceAddress> AvailableAddress => null;

        #endregion
        #region CurrentAddress
        private DeviceAddress _address;

        [Browsable(false)]
        [XmlIgnore]
        public DeviceAddress CurrentAddress
        {
            get => _address;
            set => SetProperty(ref _address, value);
        }
        #endregion
        #region Retries property

        private int _retries = 1;

        [Category("Retries")]
        public int Retries
        {
            get => _retries;
            set => SetProperty(ref _retries, value);
        }

        #endregion
        #endregion

        protected Device(CommunicationLayer commLayer, 
                         Profiler profiler)
        {
            Master = new BusCentral(commLayer, profiler)
            {
                MessageListener = this
            };
            Master.Add(new PrintfMessage());
        }

        #region Implementation of ping

        /// <summary>
        /// Ping the connected device.
        /// </summary>
        /// <returns>the ping count of the connected the device</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public virtual int Ping()
        {
            int retValue = -1;

            try
            {
                dynamic ping = CreatePing();
                Execute(ping);
                retValue = (int) ping.Count;
            }
            catch { }

            return retValue;
        }

        /// <summary>
        /// Create the Ping for testing the connection 
        /// </summary>
        /// <returns>Ping function</returns>
        public virtual DeviceFunction CreatePing() => new Ping();

        #endregion
        #region Debugging
        private readonly List<DebugSpecification> _debugSpecifications = new List<DebugSpecification>();

        internal void AddDebugSpecification(DebugSpecification specification)
        {
            if (specification is null)
                throw new ArgumentNullException(nameof(specification));

            if (_debugSpecifications.Any((s) => s.Address == specification.Address))
                throw new ArgumentException($"Debug specification for address {specification.Address} allready exists");

            _debugSpecifications.Add(specification);
        }

        public DebugSpecification GetActiveDebugSpecification()
        {
            if (_debugSpecifications.Count == 0)
                return null;

            if (CurrentAddress is null)
                return _debugSpecifications[0];

            if (!_debugSpecifications.Any((s) => s.Address == CurrentAddress.Value))
                return null;

            return _debugSpecifications.Find((s) => s.Address == CurrentAddress.Value);
        }

        /// <summary>
        /// Set the active debug signals
        /// </summary>
        /// <param name="signals">the debug signals that should be active</param>
        public virtual void SetActiveDebugSignals(DebugSignal[] signals)
        {
            if (signals is null)
                throw new ArgumentNullException(nameof(signals));

            if (signals.Length != NumberOfSupportedDebugSignals)
                throw new ArgumentException($"Invalid number of debug signals [ {signals.Length} ] must be {NumberOfSupportedDebugSignals}");

            var function = new SetDebugSignal();
            function.Signals.AddRange(signals);
            Execute(function);
        }

        public abstract int NumberOfSupportedDebugSignals { get; }

        #endregion
        #region Implementation of connect and disconnect

        public List<string> GetLocationsDevices() => Master.GetLocations();

        public bool Connect()
        {
            bool retValue = false;

            if (!Master.IsOpen)
            {
                DeviceFunction identification = CreateIdentificationFunction();
                var retries = Retries;
                Retries = Retries > 3 ? Retries : 3;

                try
                {
                    Master.Open();
                    WaitOnConnected(200);
                    Execute(identification);
                }
                catch
                {
                    Master.Close();
                    Retries = retries;
                    throw;
                }

                Retries = retries;

                if (IsCompatible(identification))
                {
                    Connected = true;
                    retValue = true;
                }
                else
                {
                    Master.Close();    
                    throw new IncompatibleDeviceException(identification.ToString());
                }
            }

            return retValue;
        }

        /// <summary>
        /// Create the DeviceFunction for identifying the device. After it has been successfully executed the 
        /// IsCompatible() function can be used to check if the connected device is compatible.
        /// </summary>
        /// <returns>The DeviceFunction that performs device identification for the device</returns>
        public virtual DeviceFunction CreateIdentificationFunction() => new DeviceIdentification();

        public abstract bool IsCompatible(DeviceFunction function);

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

        #endregion
        #region Open and closing of the device

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

        #endregion

        public void Execute(DeviceFunction function)
        {
            if (function is object)
            {
                for (int n = 0; n < Retries; ++n)
                {
                    try
                    {
                        watch.Restart();
                        Master.Execute(function, CurrentAddress);
                        watch.Stop();
                        function.TransmissionTime = watch.ElapsedMilliseconds;
                        break;
                    }
                    catch 
                    { 
                        if (n == Retries - 1) 
                        { 
                            throw; 
                        } 
                    }
                }
            }
        }

        public void Send(DeviceMessage message)
        {
            Master.Send(message, CurrentAddress);
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
        public List<DeviceFunction> Functions => FunctionList;

        [XmlIgnore]
        [Browsable(false)]
        protected List<DeviceFunction> FunctionList { get; } = new List<DeviceFunction>();

        [XmlIgnore]
        [Browsable(false)]
        protected List<MessageDispatcher> Dispatchers { get; } = new List<MessageDispatcher>();

        private readonly Stopwatch watch = new Stopwatch();

        #region Implementation of dispose pattern 

        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (Master is object)
                        Master.Dispose();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
