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
        public abstract class DeviceState
        {
            public override sealed string ToString()
            {
                return "- DEVICE STATE";
            }
        }

        private bool connected = false;
        private readonly object lockObject = new object();

        [Browsable(false)]
        public abstract DeviceState State { get; }

        public event EventHandler<MessageEventArgs<PrintfMessage>> OnPrintf;
        public event EventHandler<DeviceState> OnStateChanged;
        public event EventHandler<bool> OnConnected;
        public event EventHandler<Exception> OnConnectFailed;
        public event EventHandler<bool> OnDisconnected;
        public event EventHandler<Exception> OnDisconnectFailed;


        public Device(CommunicationLayer commLayer, DeviceData device)
        {
            CommLayer = commLayer;
            this.Master = new DeviceMaster(commLayer, device);
            Master.MessageListener = this;
            Master.Add(new PrintfMessage());
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Client is notified by event")]
        public virtual void Connect()
        {
            if (!Master.IsOpen)
            {
                try
                {
                    Master.Open();
                    BeginExecute(new DeviceIdentification(),
                        (f) =>
                        {
                            var devId = f as DeviceIdentification;

                            if (IsCompatible(devId))
                            {
                                Connected = true;
                                NotifyConnection(true);
                            }
                            else
                            {
                                NotifyConnectionFailed(new IncompatibleDeviceException(devId.ToString()));
                                Master.Close();
                            }
                        },
                        (f, e) =>
                        {
                            NotifyConnectionFailed(e);
                            Master.Close();
                        });
                }
                catch (Exception e)
                {
                    NotifyConnectionFailed(e);
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Client is notified by NotifyDisconnectFailed")]
        public virtual void Disconnect()
        {
            if (Master.IsOpen)
            {
                try
                {
                    Master.Close();
                    Connected = false;
                    NotifyDisconnected(true);
                }
                catch (Exception e)
                {
                    NotifyDisconnectFailed(e);
                }
            }
        }

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

        protected void NotifyConnection(bool success)
        {
            OnConnected?.Invoke(this, success);
        }

        protected void NotifyConnectionFailed(Exception e)
        {
            OnConnectFailed?.Invoke(this, e);
        }

        protected void NotifyDisconnected(bool success)
        {
            OnDisconnected?.Invoke(this, success);
        }

        protected void NotifyDisconnectFailed(Exception e)
        {
            OnDisconnectFailed?.Invoke(this, e);
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

        public void Execute(Function function)
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

        public Task BeginExecute(Function function, 
                                 Action<Function> onSuccess, 
                                 Action<Function, Exception> onFailure)
        {
            return Task.Run(() =>
            {
                try
                {
                    Execute(function);
                    onSuccess(function);
                }
                catch (Exception e)
                {
                    onFailure(function, e);
                }
            });
        }

        public void Send(Message message)
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
        public List<Function> Functions
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
        protected List<Function> FunctionList { get; } = new List<Function>();

        [XmlIgnore]
        [Browsable(false)]
        protected List<MessageDispatcher> Dispatchers { get; } = new List<MessageDispatcher>();

        private readonly Stopwatch watch = new Stopwatch();
    }
}
