using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.ECP.Messages
{
    public class TimingMessage :
        DeviceMessage
    {
        public static readonly byte CODE = 0xFD;

        public override byte Code => CODE;

        public TimingMessage() : base(CODE, 0) { }

        public TimingMessage(Packet response) :
            base(response)
        {
            if (Packet.Length != 20)
            {
                throw new InvalidMessageException("TimingMessage has an invalid length");
            }
        }

        public UInt32 DebugSignal => Packet.GetUInt32(0);

        public UInt32 Time => Packet.GetUInt32(4);

        public UInt32 Count => Packet.GetUInt32(8);

        public UInt32 Max => Packet.GetUInt32(12);

        public UInt32 Min => Packet.GetUInt32(16);


        public double AverageTime => 
            Count > 0 ?
            ((double)Time) / ((double)Count) :
            Double.NaN;

        public override void Dispatch(dynamic listener) => listener.Accept(this);

        public override MessageDispatcher CreateDispatcher() => new MessageDispatcher(CODE, (p) => { return new TimingMessage(p); });
    }
}
