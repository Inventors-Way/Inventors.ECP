using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Inventors.ECP
{
   [Serializable]
   class SlaveNotRespondingException :
      Exception
   {
      public SlaveNotRespondingException(String message) : base(message) { }
      public SlaveNotRespondingException(String message, Exception inner) : base(message, inner) { }
      protected SlaveNotRespondingException(SerializationInfo info, StreamingContext context)
         : base(info, context) 
      { }
   }
}
