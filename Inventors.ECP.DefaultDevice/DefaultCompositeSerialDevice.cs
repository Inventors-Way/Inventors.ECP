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
    public class DefaultCompositeSerialDevice :
        Device
    {
        public DefaultCompositeSerialDevice() :
            base(new SerialPortLayer(), new Profiler())
        {
            Add(new DeviceIdentification());
            Add(new Ping());
            Add(new GetEndianness());

            Add(new TimingViolationMessage());
            Add(new TimingMessage());
        }

        public override int NumberOfSupportedDebugSignals => 2;

        protected override string GetPeripheralErrorString(int errorCode) => $"Unknown error {errorCode:X}";

        public override List<DeviceAddress> AvailableAddress => new List<DeviceAddress>()
        {
            new DeviceAddress(1, "Sub-device 1"),
            new DeviceAddress(2, "Sub-device 2")
        };

        public void Accept(TimingViolationMessage msg) =>
            Profiler.Add(new TimingViolation(msg.DebugSignal, msg.Time, msg.TimeLimit, 0));

        public void Accept(TimingMessage msg) =>
            Profiler.Add(new TimingRecord(msg.DebugSignal, msg.AverageTime, msg.Min, msg.Max));

        public override bool IsCompatible(DeviceFunction identification) => true;
    }
}
