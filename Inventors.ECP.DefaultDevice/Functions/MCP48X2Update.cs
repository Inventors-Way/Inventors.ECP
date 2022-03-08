using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.ECP.DefaultDevice.Functions
{
    public class MCP48X2Update : 
        DeviceFunction
    {
        public override byte Code => 0x30;

        public MCP48X2Update() : base(requestLength: 4, responseLength: 0)
        {
        }

        public override FunctionDispatcher CreateDispatcher() => new FunctionDispatcher(Code, () => new MCP48X2Update());

        public override int Dispatch(dynamic listener) => listener.Accept(this);

        public UInt16 Channel01
        {
            get => Request.GetUInt16(0);
            set => Request.InsertUInt16(0, value);
        }

        public UInt16 Channel02
        {
            get => Request.GetUInt16(2);
            set => Request.InsertUInt16(2, value);
        }

        public override string ToString() => "[0x30] MCP48X2 Update";
    }
}
