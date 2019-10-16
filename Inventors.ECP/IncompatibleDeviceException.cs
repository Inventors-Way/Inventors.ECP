using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.ECP
{
    public class IncompatibleDeviceException :
        Exception
    {
        public IncompatibleDeviceException(String message) : base(message) { }
        public IncompatibleDeviceException(String message, Exception inner) : base(message, inner) { }
        protected IncompatibleDeviceException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
