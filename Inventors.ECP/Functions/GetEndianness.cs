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
        public GetEndianness() : 
            base(code: 0x03, requestLength: 0, responseLength: 2) 
        {
            response.InsertUInt16(0, 1);
        }

        public override void Dispatch(dynamic listener)
        {
            listener.Accept(this);
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
