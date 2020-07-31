using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.ECP.Communication
{
    public class MessageDispatcher
    {
        public MessageDispatcher(byte code, Func<Packet, DeviceMessage> creator)
        {
            this.Code = code;
            this.creator = creator;
        }

        public byte Code { get; }

        public DeviceMessage Create(Packet packet)
        {
            var retValue = creator(packet);
            retValue.OnReceived();
            return retValue;
        }

        private readonly Func<Packet, DeviceMessage> creator;
    }
}
