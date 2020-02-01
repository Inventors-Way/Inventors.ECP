using Inventors.ECP.Functions;
using Inventors.ECP.Messages;
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
        public class DefaultState :
            DeviceState
        {
        }

        public override DeviceState State { get { return state; } }

        private readonly DefaultState state = new DefaultState();

        public DefaultSerialDevice(DeviceData device) :
            base(new SerialPortLayer(), device)
        {
            FunctionList.Add(new DeviceIdentification());
            FunctionList.Add(new Ping());
            FunctionList.Add(new GetEndianness());
        }

        public DefaultSerialDevice() : this(DefaultIdentification) { }

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
