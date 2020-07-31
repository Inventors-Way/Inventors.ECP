using System;
using System.Collections.Generic;
using System.Text;

namespace Inventors.ECP.Communication
{
    public class FunctionDispatcher
    {
        public FunctionDispatcher(byte code, Func<DeviceFunction> creator)
        {
            this.Code = code;
            this.creator = creator;
        }

        public byte Code { get; }

        public DeviceFunction Create(Packet packet)
        {
            var retValue = creator();
            retValue.SetRequest(packet);
            retValue.OnSlaveReceived();
            return retValue;
        }

        private readonly Func<DeviceFunction> creator;
    }
}
