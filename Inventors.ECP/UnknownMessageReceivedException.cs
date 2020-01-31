using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.ECP
{
    [Serializable]
    public class UnknownMessageReceivedException :
        Exception
    {
        public UnknownMessageReceivedException(String message) : base(message) { }
        public UnknownMessageReceivedException(String message, Exception inner) : base(message, inner) { }
        protected UnknownMessageReceivedException(SerializationInfo info, StreamingContext context)
         : base(info, context) 
      { }

        public UnknownMessageReceivedException()
        {
        }
    }
}
