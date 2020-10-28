using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.ECP
{
    public interface IScript
    {
        IList<DeviceFunction> Functions { get; }
    }
}
