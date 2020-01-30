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

        public DefaultDevice(CommunicationLayer layer, DeviceData device) :
            base(layer, device)
        {
            functions.Add(new DeviceIdentification());
            functions.Add(new Ping());
            functions.Add(new GetEndianness());
        }

        public DefaultDevice(CommunicationLayer layer) : this(layer, DefaultIdentification) { }

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
