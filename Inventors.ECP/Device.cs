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
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Inventors.ECP
{
    public abstract class Device :
        INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
        private readonly object lockObject = new object();
        public event PropertyChangedEventHandler PropertyChanged;
        private readonly Dictionary<string, PropertyChangedEventArgs> eventArgs = new Dictionary<string, PropertyChangedEventArgs>();

        private PropertyChangedEventArgs GetEventArgs(string propertyName)
        {
            if (!eventArgs.ContainsKey(propertyName))
            {
                eventArgs.Add(propertyName, new PropertyChangedEventArgs(propertyName));
            }

            return eventArgs[propertyName];
        }

        protected bool SetProperty<T>(ref T field, T newValue, [CallerMemberName]string propertyName = null)
        {
            Debug.Assert(string.IsNullOrEmpty(propertyName) ||
                         (this.GetType().GetRuntimeProperty(propertyName) != null),
                         "Check that the property name exists for this instance.");

            if (!EqualityComparer<T>.Default.Equals(field, newValue))
            {
                field = newValue;
                PropertyChanged?.Invoke(this, GetEventArgs(propertyName));

                return true;
            }

            return false;
        }

        protected bool SetPropertyLocked<T>(ref T field, T newValue, [CallerMemberName]string propertyName = null)
        {
            Debug.Assert(string.IsNullOrEmpty(propertyName) ||
                         (this.GetType().GetRuntimeProperty(propertyName) != null),
                         "Check that the property name exists for this instance.");

            lock (lockObject)
            {
                if (!EqualityComparer<T>.Default.Equals(field, newValue))
                {
                    field = newValue;
                    PropertyChanged?.Invoke(this, GetEventArgs(propertyName));

                    return true;
                }
            }

            return false;
        }


        protected T NotifyIfChanged<T>(T current, T newValue, [CallerMemberName]string propertyName = null)
        {
            Debug.Assert(string.IsNullOrEmpty(propertyName) ||
                         (this.GetType().GetRuntimeProperty(propertyName) != null),
                         "Check that the property name exists for this instance.");

            if (!EqualityComparer<T>.Default.Equals(current, newValue))
            {
                PropertyChanged?.Invoke(this, GetEventArgs(propertyName));
            }

            return newValue;
        }

        protected void Notify(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, GetEventArgs(propertyName));
        }

        #endregion
        #region Properties
        #region Connected
        private bool _connected = false;

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
            set => SetPropertyLocked(ref _connected, value);
        }
        #endregion
        #region DeviceMaster
        [Browsable(false)]
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
        public bool IsOpen => Master.IsOpen;
        #endregion
        #endregion

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
                    }

                    Close();
                }
                catch 
                {
                    Close();
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
            Log.Debug(message.DebugMessage);
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
