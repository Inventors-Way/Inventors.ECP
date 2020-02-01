using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.ECP
{
    [Serializable]
    public class InvalidMessageException :
        Exception
    {
        public InvalidMessageException(String message) : base(message) { }
        public InvalidMessageException(String message, Exception inner) : base(message, inner) { }
        protected InvalidMessageException(SerializationInfo info, StreamingContext context)
         : base(info, context)
        { }

        public InvalidMessageException() { }
    }
}
