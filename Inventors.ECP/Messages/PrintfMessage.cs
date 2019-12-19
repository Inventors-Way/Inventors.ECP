using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.ECP.Messages
{
    public class PrintfMessage :
        Message
    {
        public static readonly byte CODE = 0xFF;

        public override byte Code { get { return CODE; } }

        public PrintfMessage(Packet response) :
            base(response)
        {
        }

        public override void Dispatch(dynamic listener)
        {
            listener.Accept(this);
        }

        public String DebugMessage
        {
            get
            {
                if (mPacket.Length > 0)
                {
                    return mPacket.GetString(0, mPacket.Length);
                }
                else
                {
                    return "";
                }
            }
            set
            {
                if ((value.Length < 255) && (value.Length > 0))
                {
                    mPacket = new Packet(CODE, (byte) value.Length);
                    mPacket.InsertString(0, value.Length, value);
                }
                else
                {
                    mPacket = new Packet(CODE, 0);
                }
            }
        }

        public override string ToString()
        {
            return DebugMessage;
        }
    }
}
