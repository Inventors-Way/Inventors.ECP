using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.ECP.Functions
{
    public class GetEndianness :
        Function
    {
        private static byte ResponseLength = 2;

        public GetEndianness() : base(0x03) { }

        protected override bool IsResponseValid()
        {
            return response.Length == ResponseLength;
        }

        [Category("Endianness")]
        [Description("Are the slave and master of the same endianness")]
        public bool EqualEndianness
        {
            get
            {
                return response != null ? response.GetUInt16(0) == 1 : true;
            }
        }

        public override string ToString()
        {
            return "[0x03] GetEndianness";
        }
    }
}
