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
        private static byte ResponseLength = 4;

        public Ping() : base(0x02) { }

        protected override bool IsResponseValid()
        {
            return response.Length == ResponseLength;
        }

        [Category("Ping")]
        [Description("Number of pings answered by the slave")]
        public UInt32 Count
        {
            get
            {
                return response != null ? response.GetUInt32(0) : 0;
            }
        }

        public override string ToString()
        {
            return "[0x02] Ping";
        }
    }
}
