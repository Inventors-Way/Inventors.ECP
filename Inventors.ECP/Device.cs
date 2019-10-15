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

        [Browsable(false)]
        public abstract DeviceState State {get; }

        public event EventHandler<MessageEventArgs<PrintfMessage>> OnPrintf;
        public event EventHandler<DeviceState> OnStateChanged;

        public Device(CommunicationLayer commLayer)
        {
            CommLayer = commLayer;
            this.Master = new DeviceMaster(commLayer);
            Master.MessageListener = this;
            Master.Dispatchers.Add(new MessageDispatcher(PrintfMessage.CODE, (p) => { return new PrintfMessage(p); }));
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

        public bool IsOpen()
        {
            return Master.IsOpen;
        }

        public void Dispatch()
        {
            if (Master.IsOpen)
            {
                Master.DispatchMessages();
            }
        }

        public void Execute(Function function)
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
                catch (Exception e)
                {
                    if (n == Retries - 1)
                    {
                        throw e;
                    }
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
                return functions;
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

        [Browsable(false)]
        public CommunicationLayer CommLayer { get; private set; }

        protected readonly List<Function> functions = new List<Function>();
        protected readonly List<MessageDispatcher> dispatchers = new List<MessageDispatcher>();
        private Stopwatch watch = new Stopwatch();
    }
}
