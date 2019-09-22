using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventors.ECP;
using Inventors.ECP.Messages;

namespace Inventors.ECP
{
    public abstract class Message
    {
        public abstract byte Code { get; }

        public Message(Packet response)
        {
            mPacket = response;
        }

        public Message(byte code)
        {
            mPacket = new Packet(code, 0);
        }

        public Message(byte code, byte length)
        {
            mPacket = new Packet(code, length);
        }

        internal byte[] GetPacket()
        {
            return mPacket.ToArray();
        }

        public abstract void Dispatch(dynamic listener);

        protected Packet mPacket;
    }
}
