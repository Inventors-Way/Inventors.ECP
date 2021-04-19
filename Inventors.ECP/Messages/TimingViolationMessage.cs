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
        public static readonly byte CODE = 0xFE;

        public override byte Code => CODE;

        public TimingViolationMessage() : base(CODE, 0) { }

        public TimingViolationMessage(Packet response) :
            base(response)
        {
            if (Packet.Length != 12)
            {
                throw new InvalidMessageException("TimingViolation has an invalid length");
            }
        }

        public uint DebugSignal => Packet.GetUInt32(0);

        public uint TimeLimit => Packet.GetUInt32(4);

        public uint Time => Packet.GetUInt32(8);

        public override void Dispatch(dynamic listener) => listener.Accept(this);

        public override MessageDispatcher CreateDispatcher() => new MessageDispatcher(CODE, (p) => { return new TimingViolationMessage(p); });
    }
}
