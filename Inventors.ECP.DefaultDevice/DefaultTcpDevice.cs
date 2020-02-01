using Inventors.ECP.Functions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inventors.ECP.DefaultDevice
{
    public class DefaultTcpDevice :
        Device
    {
        public class DefaultState :
            DeviceState
        {
        }

        public override DeviceState State { get { return state; } }

        private readonly DefaultState state = new DefaultState();

        public DefaultTcpDevice(DeviceData device) :
            base(new TcpClientLayer(), device)
        {
            FunctionList.Add(new DeviceIdentification());
            FunctionList.Add(new Ping());
            FunctionList.Add(new GetEndianness());
        }

        public DefaultTcpDevice() : this(DefaultIdentification) { }

        private static DeviceData DefaultIdentification => new DeviceData()
        {
            DeviceID = 1,
            Device = "Default Device",
            ManufactureID = 1,
            Manufacture = "Inventors' Way",
            SerialNumber = 1001
        };

        public override bool IsCompatible(DeviceIdentification identification)
        {
            return true;
        }
    }
}
