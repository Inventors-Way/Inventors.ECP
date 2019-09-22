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
        }

        public override string ToString()
        {
            return DebugMessage;
        }
    }
}
