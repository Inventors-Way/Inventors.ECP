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
        public DefaultTcpDevice(string beaconName) :
            base(new TcpClientLayer(beaconName))
        {
            FunctionList.Add(new DeviceIdentification());
            FunctionList.Add(new Ping());
            FunctionList.Add(new GetEndianness());
        }

        public override bool IsCompatible(DeviceIdentification identification)
        {
            return true;
        }
    }
}
