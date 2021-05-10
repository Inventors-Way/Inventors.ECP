using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.ECP.DefaultDevice.Messages
{
    public class SimpleMessage :
        DeviceMessage
    {
        public static readonly byte CODE = 0x80;

        public override byte Code => CODE;

        public SimpleMessage() : base(CODE, sizeof(int))
        {
            X = 0;
        }

        public SimpleMessage(Packet response) :
            base(response)
        {
            if (Packet.Length != 2 * sizeof(UInt16))
            {
                throw new InvalidMessageException($"A received ClickMessage does not have a length of {2 * sizeof(UInt16)}");
            }
        }

        public int X
        {
            get => Packet.GetInt32(0);
            set => Packet.InsertInt32(0, value);
        }

        public override MessageDispatcher CreateDispatcher() => new MessageDispatcher(CODE, (p) => new SimpleMessage(p));

        public override void Dispatch(dynamic listener) => listener.Accept(this);
    }
}
