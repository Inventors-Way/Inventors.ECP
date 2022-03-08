using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.ECP.DefaultDevice.Functions
{
    public class DIOSetPin :
        DeviceFunction
    {
        public override byte Code => 0x22;

        public DIOSetPin() : base(requestLength: 2, responseLength: 0)
        {
        }

        public override FunctionDispatcher CreateDispatcher() => new FunctionDispatcher(Code, () => new DIOSetPin());

        public override int Dispatch(dynamic listener) => listener.Accept(this);

        public Pin Pin
        {
            get => (Pin) Request.GetByte(0);
            set => Request.InsertByte(0, (byte)value);
        }

        public bool Value
        {
            get => Request.GetByte(1) != 0;
            set => Request.InsertByte(1, (byte) (value ? 1 : 0));
        }

        public override string ToString() => "[0x22] DIO SetPin";
    }
}
