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

        public PrintfMessage() :
            base(CODE)
        {
        }

        public static MessageDispatcher CreateDispatcher()
        {
            return new MessageDispatcher(CODE, (p) => new PrintfMessage(p));
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
                var str = "";

                if (value.Length <= 255)
                {
                    str = value;    
                }
                else
                {
                    if (value.Length > 0)
                    {
                        str = value.Substring(0, 255);
                    }
                }

                mPacket = new Packet(CODE, (byte)str.Length);
                mPacket.InsertString(0, str.Length, str);
            }
        }

        public override string ToString()
        {
            return DebugMessage;
        }
    }
}
