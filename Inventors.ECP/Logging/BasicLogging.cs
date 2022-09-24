using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.ECP.Logging
{
    public class BasicLogging :
        LogConfig
    {
        public override void Visit(ILogConfigVisitor visitor) => visitor.Accept(this);
    }
}
