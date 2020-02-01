using Inventors.ECP.Communication;
using Inventors.ECP.Functions;
using Inventors.ECP.Messages;
using Inventors.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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

        public Device(CommunicationLayer commLayer, DeviceData device)
        {
            CommLayer = commLayer;
            this.Master = new DeviceMaster(commLayer, device);
            Master.MessageListener = this;
            Master.Add(new PrintfMessage());
        }

        public virtual void Connect()
        {
            if (!Master.IsOpen)
            {
                var devId = new DeviceIdentification();
                Master.Open();
                Execute(devId);

                if (IsCompatible(devId))
                {
                    Connected = true;
                }
                else
                {
                    Master.Close();    
                    throw new IncompatibleDeviceException(devId.ToString());
                }
            }
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
