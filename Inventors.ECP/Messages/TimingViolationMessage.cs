using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.ECP.Messages
{
    public class TimingViolationMessage :
        DeviceMessage
    {
        public override byte Code => 0xFE;

        public TimingViolationMessage() : base(12) { }

        public TimingViolationMessage(Packet response) :
            base(response)
        {
            if (Packet.Length != 12)
            {
                throw new InvalidMessageException("TimingViolation has an invalid length");
            }
        }

        public uint DebugSignal
        {
            get => Packet.GetUInt32(0);
            set => Packet.InsertUInt32(0, value);
        }

        public uint TimeLimit 
        {
            get => Packet.GetUInt32(4); 
            set => Packet.InsertUInt32(4, value);
        }

        public uint Time 
        {
            get => Packet.GetUInt32(8); 
            set => Packet.InsertUInt32(8, value);
        }

        public override void Dispatch(dynamic listener) => listener.Accept(this);

        public override MessageDispatcher CreateDispatcher() => new MessageDispatcher(Code, (p) => { return new TimingViolationMessage(p); });
    }
}
