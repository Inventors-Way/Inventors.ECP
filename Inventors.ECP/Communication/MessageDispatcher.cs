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
            if (creator is null)
                throw new ArgumentNullException(nameof(creator));

            Code = code;
            _creator = creator;
        }


        public DeviceMessage Create(Packet packet)
        {
            if (packet is null)
                throw new ArgumentNullException(nameof(packet));

            var retValue = _creator(packet);
            retValue.OnReceived();
            return retValue;
        }

        public byte Code { get; }
        private readonly Func<Packet, DeviceMessage> _creator;
    }
}
