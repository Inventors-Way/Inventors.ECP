using System;
using System.Collections.Generic;
using System.Text;

namespace Inventors.ECP
{
    public class FunctionDispatcher
    {
        public FunctionDispatcher(byte code, Func<Function> creator)
        {
            this.code = code;
            this.creator = creator;
        }

        public byte Code
        {
            get
            {
                return code;
            }
        }

        public Function Create(Packet packet)
        {
            var retValue = creator();
            retValue.SetRequest(packet);
            retValue.OnSlaveReceived();
            return retValue;
        }

        private readonly byte code;
        private readonly Func<Function> creator;
    }
}
