using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Inventors.ECP
{
    public class InvalidMasterRequestException : Exception
    {
        public InvalidMasterRequestException(String message) : base(message) { }
        public InvalidMasterRequestException(String message, Exception inner) : base(message, inner) { }
        protected InvalidMasterRequestException(SerializationInfo info, StreamingContext context)
           : base(info, context)
        { }

        public InvalidMasterRequestException() { }
    }
}
