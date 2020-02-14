using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.ECP.DeviceHost
{
    public abstract class PackageAction
    {
        public PackageAction(DevicePackage package) => Package = package;

        public abstract bool Execute();

        public abstract void Revert();

        protected DevicePackage Package { get; }
    }
}
