using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.ECP.DefaultDevice.Functions
{
    public class SimpleFunction :
        DeviceFunction
    {
        public static readonly int REQUEST_SIZE = sizeof(int);
        public static readonly int RESPONSE_SIZE = sizeof(int);

        public const byte FUNCTION_CODE = 0x10;

        public SimpleFunction() : base(FUNCTION_CODE, requestLength: REQUEST_SIZE, responseLength: RESPONSE_SIZE)
        {
            Operand = 0;
            Answer = 0;
        }

        public override FunctionDispatcher CreateDispatcher() => new FunctionDispatcher(FUNCTION_CODE, () => new SimpleFunction());

        public override int Dispatch(dynamic listener) => listener.Accept(this);

        public int Operand
        {
            get => Request.GetInt32(0);
            set => Request.InsertInt32(0, value);
        }

        public int Answer
        {
            get => Response.GetInt32(0);
            set => Response.InsertInt32(0, value);
        }

        public override string ToString() => "[0x10] Simple Function";
    }
}
