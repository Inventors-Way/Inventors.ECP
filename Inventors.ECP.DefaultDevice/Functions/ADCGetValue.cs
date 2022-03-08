using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.ECP.DefaultDevice.Functions
{
    public class ADCGetValue :
        DeviceFunction
    {
        public enum AnalogChannel
        {
            CHAN01 = 0,
            CHAN02,
            CHAN03,
            CHAN04,
        }

        public override byte Code => 0x24;

        public ADCGetValue() : base(requestLength: 1, responseLength: 2)
        {
        }

        public override FunctionDispatcher CreateDispatcher() => new FunctionDispatcher(Code, () => new ADCGetValue());

        public override int Dispatch(dynamic listener) => listener.Accept(this);

        public AnalogChannel Channel
        {
            get => (AnalogChannel)Request.GetByte(0);
            set => Request.InsertByte(0, (byte)value);
        }

        public UInt16 Value
        {
            get => Response.GetUInt16(0);
            set => Response.InsertUInt16(0, value);
        }

        public override string ToString() => "[0x24] ADC GetValue";
    }
}
