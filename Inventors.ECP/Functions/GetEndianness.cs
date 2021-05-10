using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.ECP.Functions
{
    public class GetEndianness :
        DeviceFunction
    {
        public static readonly byte CODE = 0x03;

        public GetEndianness() : 
            base(code: CODE, requestLength: 0, responseLength: 2) 
        {
            Response.InsertUInt16(0, 1);
        }

        public override FunctionDispatcher CreateDispatcher() => new FunctionDispatcher(CODE, () => new GetEndianness());

        public override int Dispatch(dynamic listener) => listener.Accept(this);

        [Category("Endianness")]
        [Description("Are the slave and master of the same endianness")]
        public bool EqualEndianness => Response == null || Response.GetUInt16(0) == 1;

        public override string ToString() => "[0x03] GetEndianness";
    }
}
