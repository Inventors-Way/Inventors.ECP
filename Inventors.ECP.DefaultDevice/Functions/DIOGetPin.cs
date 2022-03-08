using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.ECP.DefaultDevice.Functions
{
    public class DIOGetPin :
        DeviceFunction
    {
        public override byte Code => 0x23;

        public DIOGetPin() : base(requestLength: 1, responseLength: 1)
        {
        }

        public override FunctionDispatcher CreateDispatcher() => new FunctionDispatcher(Code, () => new DIOGetPin());

        public override int Dispatch(dynamic listener) => listener.Accept(this);

        public Pin Pin
        {
            get => (Pin)Request.GetByte(0);
            set => Request.InsertByte(0, (byte)value);
        }

        public bool Value
        {
            get => Response.GetByte(0) != 0;
            set => Response.InsertByte(0, (byte)(value ? 1 : 0));
        }

        public override string ToString() => "[0x23] DIO GetPin";
    }
}
