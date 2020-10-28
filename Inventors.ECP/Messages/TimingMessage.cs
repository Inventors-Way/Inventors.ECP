using Inventors.ECP.Communication;
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
            if (Packet.Length < 16)
            {
                throw new InvalidMessageException("TimingMessage is shorter than 16 bytes");
            }

        }

        public uint Time => Packet.GetUInt32(0);

        public uint Count => Packet.GetUInt32(4);

        public uint Max => Packet.GetUInt32(8);

        public uint Min => Packet.GetUInt32(12);

        public string Name => Packet.Length > 16 ? 
                              Packet.GetString(16, Packet.Length - 16) :  
                              "Default";

        public double AverageTime => 
            Count > 0 ?
            ((double)Time) / ((double)Count) :
            Double.NaN;

        public override void Dispatch(dynamic listener) => listener.Accept(this);

        public override MessageDispatcher CreateDispatcher() => new MessageDispatcher(CODE, (p) => { return new TimingMessage(p); });
    }
}
