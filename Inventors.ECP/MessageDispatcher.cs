using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.ECP
{
    public class MessageDispatcher
    {
        public MessageDispatcher(byte code, Func<Packet, Message> creator)
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

        public Message Create(Packet packet)
        {
            return creator(packet);
        }

        private readonly byte code;
        private Func<Packet, Message> creator;
    }
}
