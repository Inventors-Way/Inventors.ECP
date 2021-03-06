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

        public override bool IsCompatible(DeviceFunction identification) => true;
    }
}
