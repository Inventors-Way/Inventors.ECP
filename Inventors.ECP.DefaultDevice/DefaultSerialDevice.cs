using Inventors.ECP.Communication;
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
        public DefaultSerialDevice() :
            base(new SerialPortLayer())
        {
            FunctionList.Add(new DeviceIdentification());
            FunctionList.Add(new Ping());
            FunctionList.Add(new GetEndianness());

            Master.Add(new TimingViolationMessage());
            Master.Add(new TimingMessage());
        }

        public override bool IsCompatible(DeviceFunction identification)
        {
            return true;
        }
    }
}
