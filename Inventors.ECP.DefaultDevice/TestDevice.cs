using Inventors.ECP.DefaultDevice.Functions;
using Inventors.ECP.DefaultDevice.Messages;
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
    public class TestDevice :
        Device
    {
        public TestDevice() :
            base(new SerialPortLayer(), new Profiler())
        {
            Add(new DeviceIdentification());
            Add(new Ping());
            Add(new GetEndianness());
            Add(new SetDebugSignal());

            Add(new SimpleFunction());
            Add(new DataFunction());

            Add(new SimpleMessage());
            Add(new DataMessage());

            Add(new TimingViolationMessage());
            Add(new TimingMessage());
        }

        protected override string GetPeripheralErrorString(int errorCode)
        {
            switch ((TestErrorCode) errorCode)
            {
                case TestErrorCode.INVALID_OPERATION: return "Invalid operation";
                case TestErrorCode.INVALID_STATE: return "Invalid state";
                default:
                    return "Unknown error";
            }
        }

        public override int NumberOfSupportedDebugSignals => 2;

        public void Accept(TimingViolationMessage msg) => Profiler.Add(new TimingViolation(msg.DebugSignal, msg.Time, msg.TimeLimit, 0));

        public void Accept(TimingMessage msg) =>  Profiler.Add(new TimingRecord(msg.DebugSignal, msg.AverageTime, msg.Min, msg.Max));

        public void Accept(SimpleMessage msg)
        {
            X = msg.X;
        }

        public void Accept(DataMessage msg)
        {
            Data = msg.Data;
        }

        public override bool IsCompatible(DeviceFunction identification) => true;

        public int X { get; set; }

        public List<int> Data { get; set; }
    }
}
