using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Inventors.ECP.Hosting
{
    public enum DeviceState
    {
        [XmlEnum("stopped")]
        STOPPED = 0,
        [XmlEnum("running")]
        RUNNING
    }
}
