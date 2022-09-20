using Inventors.ECP.DefaultDevice.Functions;
using Inventors.ECP.DefaultDevice.Messages;
using Inventors.ECP.Functions;
using Inventors.ECP.Messages;
using Inventors.ECP.Profiling;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.ECP.DefaultDevice
{
    public class DefaultSerialDevice :
        Device
    {
        public DefaultSerialDevice() :
            base(new SerialPortLayer(), new Profiler())
        {
            Add(new DeviceIdentification());
            Add(new Ping());
            Add(new GetEndianness());
            Add(new SetDebugSignal());

            Add(new SimpleFunction());
            Add(new DataFunction());

            Add(new SPIConfigureFunction());
            Add(new SPITestFunction());

            Add(new DIOSetPin());
            Add(new DIOGetPin());

            Add(new ADCGetValue());

            Add(new MCP48X2Update());

            Add(new SignalMessage());   
            Add(new TimingViolationMessage());
            Add(new TimingMessage());
        }

        protected override string GetPeripheralErrorString(int errorCode) => $"Unknown error {errorCode:X}";

        public override int NumberOfSupportedDebugSignals => 2;

        public void Accept(TimingViolationMessage msg)
        {
            Profiler.Add(new TimingViolation(msg.DebugSignal, msg.Time, msg.TimeLimit, 0));
        }

        public void Accept(TimingMessage msg)
        {
            Profiler.Add(new TimingRecord(msg.DebugSignal, msg.AverageTime, msg.Min, msg.Max));
        }

        public void Accept(SignalMessage msg)
        {
            Log.Information("Signal message; {0}", msg.X);
        }

        public override bool IsCompatible(DeviceFunction identification) => true;
    }
}
