using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.ECP.DefaultDevice.Messages
{
    public class SignalMessage :
        DeviceMessage
    {
        public override byte Code => 0x80;

        public SignalMessage() : base(sizeof(byte))
        {
            X = 0;
        }

        public SignalMessage(Packet response) :
            base(response)
        {
            if (Packet.Length != sizeof(byte))
            {
                throw new InvalidMessageException($"A received SignalMessage does not have a length of {sizeof(byte)}");
            }
        }

        public byte X
        {
            get => Packet.GetByte(0);
            set => Packet.InsertByte(0, value);
        }

        public override MessageDispatcher CreateDispatcher() => new MessageDispatcher(Code, (p) => new SignalMessage(p));

        public override void Dispatch(dynamic listener) => listener.Accept(this);
    }
}
