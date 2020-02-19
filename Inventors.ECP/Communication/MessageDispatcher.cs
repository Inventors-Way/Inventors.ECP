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
            this.code = code;
            this.creator = creator;
        }

        public byte Code
        {
            get
            {
                return code;
            }
        }

        public DeviceMessage Create(Packet packet)
        {
            var retValue = creator(packet);
            retValue.OnReceived();
            return retValue;
        }

        private readonly byte code;
        private readonly Func<Packet, DeviceMessage> creator;
    }
}
