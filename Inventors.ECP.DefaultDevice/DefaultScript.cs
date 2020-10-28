using Inventors.ECP.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Inventors.ECP.DefaultDevice
{
    [XmlRoot("script")]
    public class DefaultScript :
        IScript
    {
        [XmlElement("identification", type: typeof(DeviceIdentification))]
        [XmlElement("endianness", type: typeof(GetEndianness))]
        [XmlElement("ping", type: typeof(Ping))]
        public List<DeviceFunction> Elements { get; set; } = new List<DeviceFunction>();

        [XmlIgnore]
        public IList<DeviceFunction> Functions => Elements.AsReadOnly();
    }
}
