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
        public Ping() : 
            base(code: 0x02, requestLength: 0, responseLength: 4) 
        { 
        }

        public override void Dispatch(dynamic listener)
        {
            listener.Accept(this);
        }

        [Category("Ping")]
        [Description("Number of pings answered by the slave")]
        public UInt32 Count
        {
            get
            {
                return response.GetUInt32(0);
            }
            set
            {
                response.InsertUInt32(0, value);
            }
        }

        public override string ToString()
        {
            return "[0x02] Ping";
        }
    }
}
