using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.ECP.Logging
{
    public abstract class LogConfig
    {
        public abstract void Visit(ILogConfigVisitor visitor);
    }
}
