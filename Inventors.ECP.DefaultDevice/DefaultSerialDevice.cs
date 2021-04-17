using Inventors.ECP.Functions;
using Inventors.ECP.Messages;
using Inventors.ECP.Profiling;
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
            FunctionList.Add(new DeviceIdentification());
            FunctionList.Add(new Ping());
            FunctionList.Add(new GetEndianness());
            FunctionList.Add(new SetDebugSignal());

            Master.Add(new TimingViolationMessage());
            Master.Add(new TimingMessage());
        }

        public override List<DebugSignal> GetSupportedDebugSignals() =>
            new List<DebugSignal>()
            {
                new DebugSignal() { Code = 1, Name = "Polling Loop", Description = "Timing measurement for the main polling loop" },
                new DebugSignal() { Code = 2, Name = "Hal Layer", Description = "Timing measurement for the Run() function in the HAL layer" },
                new DebugSignal() { Code = 3, Name = "System Layer", Description = "Timing measurement for the Run() function in the System layer" },
                new DebugSignal() { Code = 4, Name = "Debug Signal Timer", Description = "Timing measurement for the Debug Signal timer that sends the timing profiles" },
                new DebugSignal() { Code = 5, Name = "Heartbeat Timer", Description = "Timing measurement for the Heartbeat timer that sends the 'Beat' printf every second" }
            };

        public override int NumberOfSupportedDebugSignals => 2;

        public override IScript CreateScript(string content) => content.ToObject<DefaultScript>();

        public void Accept(TimingViolationMessage msg)
        {
            Profiler.Add(new TimingViolation(msg.DebugSignal.ToString(), msg.Time, msg.TimeLimit, 0));
        }

        public void Accept(TimingMessage msg)
        {
            Profiler.Add(new TimingRecord(msg.DebugSignal, msg.AverageTime, msg.Min, msg.Max));
        }

        public override bool IsCompatible(DeviceFunction identification) => true;
    }
}
