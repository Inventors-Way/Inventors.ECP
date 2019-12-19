using System;
using System.Collections.Generic;
using System.Text;

namespace Inventors.ECP
{
    public class FunctionDispatcher
    {
        public FunctionDispatcher(byte code, Func<Packet, Function> creator)
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
            return creator(packet);
        }

        private readonly byte code;
        private Func<Packet, Function> creator;
    }
}
