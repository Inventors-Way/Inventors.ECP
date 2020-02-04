using Inventors.ECP.Communication;
using Inventors.ECP.Functions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inventors.ECP.DefaultDevice
{
    public class DefaultTcpDevice :
        Device
    {
        public DefaultTcpDevice(DeviceType device) :
            base(new TcpClientLayer(device.BeaconName), device)
        {
            FunctionList.Add(new DeviceIdentification());
            FunctionList.Add(new Ping());
            FunctionList.Add(new GetEndianness());
        }

        public DefaultTcpDevice() : this(DefaultIdentification) { }

        private static DeviceType DefaultIdentification => new DeviceType()
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
