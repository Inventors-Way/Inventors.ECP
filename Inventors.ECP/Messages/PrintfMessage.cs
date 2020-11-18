using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.ECP.Messages
{
    public class PrintfMessage :
        DeviceMessage
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

        public override MessageDispatcher CreateDispatcher()
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
                if (Packet.Length > 0)
                {
                    return Packet.GetString(0, Packet.Length);
                }
                else
                {
                    return "";
                }
            }
            set
            {
                if (value is object)
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

                    Packet = new Packet(CODE, (byte)str.Length);
                    Packet.InsertString(0, str.Length, str);
                }
            }
        }

        public override string ToString() => DebugMessage;
    }
}
