using Inventors.ECP.Functions;
using Inventors.ECP.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.ECP.DefaultDevice
{
    public class DefaultDevice :
        Device
    {
        public class DefaultState :
            DeviceState
        {
        }

        public override DeviceState State { get { return state; } }

        private readonly DefaultState state = new DefaultState();

        public DefaultDevice(CommunicationLayer layer) :
            base(layer)
        {
            functions.Add(new DeviceIdentification());
            functions.Add(new Ping());
            functions.Add(new GetEndianness());
            functions.Add(new ConfigurePrint());
        }

        public override bool IsCompatible(DeviceIdentification identification)
        {
            return true;
        }
    }
}
