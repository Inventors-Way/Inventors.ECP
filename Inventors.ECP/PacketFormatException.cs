using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.ECP
{
    [Serializable]
   public class PacketFormatException :
      Exception
   {
      public PacketFormatException(String message) : base(message) { }
      public PacketFormatException(String message, Exception inner) : base(message, inner) { }
      protected PacketFormatException(SerializationInfo info, StreamingContext context)
         : base(info, context) 
      { }
   }
}
