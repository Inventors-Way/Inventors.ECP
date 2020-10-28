using Inventors.ECP.Communication;
using Inventors.ECP.Communication.Discovery;
using Inventors.ECP.Functions;
using Inventors.ECP.Profiling;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inventors.ECP.DefaultDevice
{
    public class DefaultTcpDevice :
        Device
    {
        public DefaultTcpDevice() : this(new BeaconID(Manufacturer.Generic, 1, 0)) { }

        public DefaultTcpDevice(BeaconID id) :
            base(new TcpClientLayer(id), new Profiler())
        {
            FunctionList.Add(new DeviceIdentification());
            FunctionList.Add(new Ping());
            FunctionList.Add(new GetEndianness());
        }

        public override IScript CreateScript(string content) => content.ToObject<DefaultScript>();

        public override bool IsCompatible(DeviceFunction identification)
        {
            return true;
        }
    }
}
