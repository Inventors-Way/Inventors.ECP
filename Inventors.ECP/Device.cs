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
        IDisposable
    {
        private readonly object lockObject = new object();

        #region Properties
        #region PrintLevel
        [Category("Debug")]
        [XmlIgnore]
        public LogLevel PrintLevel { get; set; } = LogLevel.DEBUG;
        #endregion
        #region Connected
        private bool _connected;

        [Browsable(false)]
        [XmlIgnore]
        public bool Connected
        {
            get
            {
                lock (lockObject)
                {
                    return _connected;
                }
            }
            set
            {
                lock (lockObject)
                {
                    _connected = value;
                }
            }
        }
        #endregion
        #region BusCentral
        [Browsable(false)]
        [XmlIgnore]
        public BusCentral Central { get; }

        /// <summary>
        /// Add a message to be handled by the BusCentral.
        /// </summary>
        /// <param name="message">An instance of the message to be handled.</param>
        protected void Add(DeviceMessage message) => Central.Add(message);

        #endregion
        #region ResetOnConnected
        [Category("Communication Layer")]
        [XmlIgnore]
        public bool ResetOnConnection
        {
            get => Central.ResetOnConnection;
            set => Central.ResetOnConnection = value;
        }
        #endregion
        #region Timeout
        [Category("Communication Layer")]
        [XmlIgnore]
        public int Timeout
        {
            get => Central.Timeout;
            set => Central.Timeout = value;
        }
        #endregion
        #region IsOpen
        [Browsable(false)]
        [XmlIgnore]
        public bool IsOpen => Central.IsOpen;
        #endregion
        #region Port
        [Browsable(false)]
        [XmlIgnore]
        public string Location
        {
            get => Central.Location;
            set => Central.Location = value;
        }
        #endregion
        #region BaudRate
        [Browsable(false)]
        [XmlIgnore]
        public int BaudRate
        {
            get => Central.BaudRate;
            set => Central.BaudRate = value;
        }
        #endregion
        #region PingEnabled

        [Category("Debug")]
        [XmlIgnore]
        public bool PingEnabled { get; set; }

        #endregion
        #region Profiler
        [Browsable(false)]
        [XmlIgnore]
        public Profiler Profiler => Central.Profiler;
        #endregion
        #region AvailableAdresses

        public virtual List<DeviceAddress> AvailableAddress => null;

        #endregion
        #region CurrentAddress
        [Browsable(false)]
        [XmlIgnore]
        public DeviceAddress CurrentAddress { get; set; }
        #endregion
        #region Retries property

        [Category("Retries")]
        public int Retries { get; set; } = 1;

        #endregion
        #region Functions property
        private readonly List<DeviceFunction> functions = new List<DeviceFunction>();

        [Browsable(false)]
        public IList<DeviceFunction> Functions => functions;

        protected void Add(DeviceFunction function) => functions.Add(function);

        #endregion
        #endregion

        protected Device(CommunicationLayer commLayer, 
                         Profiler profiler)
        {
            Central = new BusCentral(commLayer, profiler)
            {
                MessageListener = this
            };
            Central.Add(new PrintfMessage());
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

        public List<string> GetLocationsDevices() => Central.GetLocations();

        public bool Connect()
        {
            bool retValue = false;

            if (!Central.IsOpen)
            {
                DeviceFunction identification = CreateIdentificationFunction();
                var retries = Retries;
                Retries = Retries > 3 ? Retries : 3;

                try
                {
                    Central.Open();
                    Execute(identification);
                }
                catch
                {
                    Central.Close();
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
                    Central.Close();    
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

        /// <summary>
        /// Must be implemented by concrete Devices to test if the Peripheral is compatible.
        /// </summary>
        /// <param name="function">Function identifying of the Peripheral</param>
        /// <returns>true if the peripheral is compatible, otherwise false.</returns>
        public abstract bool IsCompatible(DeviceFunction function);

        /// <summary>
        /// Disconnects a device.
        /// </summary>
        public virtual void Disconnect()
        {
            if (Central.IsOpen)
            {
                Central.Close();
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
            if (!Central.IsOpen)
            {
                Central.Open();                
            }
        }

        /// <summary>
        /// Closes the location of the device.
        /// </summary>
        public void Close()
        {
            if (Central.IsOpen)
            {
                Central.Close();
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
                        Central.Execute(function, CurrentAddress);
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
            Central.Send(message, CurrentAddress);
        }

        public void Accept(PrintfMessage message)
        {
            if (message is object)
            {
                switch (PrintLevel)
                {
                    case LogLevel.DEBUG: EcpLog.Debug(message.DebugMessage); break;
                    case LogLevel.STATUS: EcpLog.Status(message.DebugMessage); break;
                    case LogLevel.ERROR: EcpLog.Error(message.DebugMessage); break;
                    default:
                        break;
                }
            }
        }

        public string GetErrorString(int errorCode)
        {
            if (ErrorCode.NO_ERROR == ((ErrorCode)errorCode))
                return "No error (ErrorCode = 0x00)";

            if (ErrorCode.UNKNOWN_FUNCTION_ERR == ((ErrorCode)errorCode))
                return "Unknown function (ErrorCode = 0x01)";

            if (ErrorCode.INVALID_CONTENT_ERR == ((ErrorCode)errorCode))
                return "Invalid content (ErrorCode = 0x02)";

            return GetPeripheralErrorString(errorCode);
        }

        protected abstract string GetPeripheralErrorString(int errorCode);

        public override string ToString() => "ECP DEFAULT DEVICE";

        private readonly Stopwatch watch = new Stopwatch();

        #region Implementation of dispose pattern 

        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (Central is object)
                        Central.Dispose();
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
