using System;
using System.Collections.Generic;
using System.Text;

namespace Inventors.ECP
{
    public class FunctionDispatcher
    {
        public FunctionDispatcher(byte code, Func<DeviceFunction> creator)
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

        public DeviceFunction Create(Packet packet)
        {
            var retValue = creator();
            retValue.SetRequest(packet);
            retValue.OnSlaveReceived();
            return retValue;
        }

        private readonly byte code;
        private readonly Func<DeviceFunction> creator;
    }
}
