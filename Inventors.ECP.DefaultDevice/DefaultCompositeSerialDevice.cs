﻿using Inventors.ECP.Functions;
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
            FunctionList.Add(new DeviceIdentification());
            FunctionList.Add(new Ping());
            FunctionList.Add(new GetEndianness());

            Master.Add(new TimingViolationMessage());
            Master.Add(new TimingMessage());
        }

        public override int NumberOfSupportedDebugSignals => 2;

        public override IScript CreateScript(string content) => content.ToObject<DefaultScript>();

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
