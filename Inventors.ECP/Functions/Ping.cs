using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.ECP.Functions
{
    public class Ping : 
        Function
    {
        public static readonly byte CODE = 0x02;

        public Ping() : 
            base(code: CODE, requestLength: 0, responseLength: 4) 
        { 
        }

        public override FunctionDispatcher CreateDispatcher()
        {
            return new FunctionDispatcher(CODE, () => new Ping());
        }

        public override bool Dispatch(dynamic listener)
        {
            return listener.Accept(this);
        }

        [Category("Ping")]
        [Description("Number of pings answered by the slave")]
        public UInt32 Count
        {
            get
            {
                return Response.GetUInt32(0);
            }
            set
            {
                Response.InsertUInt32(0, value);
            }
        }

        public override string ToString()
        {
            return "[0x02] Ping";
        }
    }
}
