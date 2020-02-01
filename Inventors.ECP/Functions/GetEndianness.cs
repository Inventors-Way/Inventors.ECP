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
        public static readonly byte CODE = 0x03;

        public GetEndianness() : 
            base(code: CODE, requestLength: 0, responseLength: 2) 
        {
            Response.InsertUInt16(0, 1);
        }

        public override FunctionDispatcher CreateDispatcher()
        {
            return new FunctionDispatcher(CODE, () => new GetEndianness());
        }

        public override bool Dispatch(dynamic listener)
        {
            return listener.Accept(this);
        }

        [Category("Endianness")]
        [Description("Are the slave and master of the same endianness")]
        public bool EqualEndianness
        {
            get
            {
                return Response != null ? Response.GetUInt16(0) == 1 : true;
            }
        }

        public override string ToString()
        {
            return "[0x03] GetEndianness";
        }
    }
}
