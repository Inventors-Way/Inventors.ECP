using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.ECP
{
   [Serializable]
   public class UnknownFunctionCallException :
      Exception
   {
      public UnknownFunctionCallException(String message) : base(message) { }
      public UnknownFunctionCallException(String message, Exception inner) : base(message, inner) { }
      protected UnknownFunctionCallException(SerializationInfo info, StreamingContext context)
         : base(info, context) 
      { }

        public UnknownFunctionCallException()
        {
        }
    }
}
