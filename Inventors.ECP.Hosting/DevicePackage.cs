using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.ECP.Hosting
{
    public class DevicePackage
    {
        public string SourceFile { get; }

        public DevicePackage(string source)
        {
            SourceFile = source;
        }

        public Loader Install()
        {
            Loader retValue = null;

            return retValue;
        }
    }
}
