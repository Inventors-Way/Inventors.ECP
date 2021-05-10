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
        public override byte Code => 0xFD;

        public TimingMessage() : base(20) { }

        public TimingMessage(Packet response) :
            base(response)
        {
            if (Packet.Length != 20)
            {
                throw new InvalidMessageException("TimingMessage has an invalid length");
            }
        }

        public UInt32 DebugSignal
        {
            get => Packet.GetUInt32(0);
            set => Packet.InsertUInt32(0, value);
        }

        public UInt32 Time
        {
            get => Packet.GetUInt32(4);
            set => Packet.InsertUInt32(4, value);
        }

        public UInt32 Count 
        {
            get => Packet.GetUInt32(8); 
            set => Packet.InsertUInt32(8, value);
        } 

        public UInt32 Max 
        {
            get => Packet.GetUInt32(12); 
            set => Packet.InsertUInt32(12, value);
        }

        public UInt32 Min 
        {
            get => Packet.GetUInt32(16); 
            set => Packet.InsertUInt32(16, value);
        }


        public double AverageTime => 
            Count > 0 ?
            ((double)Time) / ((double)Count) :
            Double.NaN;

        public override void Dispatch(dynamic listener) => listener.Accept(this);

        public override MessageDispatcher CreateDispatcher() => new MessageDispatcher(Code, (p) => { return new TimingMessage(p); });
    }
}
