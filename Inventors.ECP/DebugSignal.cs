using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Inventors.ECP
{
    [XmlRoot("debug-signal")]
    public class DebugSignal
    {
        [XmlAttribute("code")]
        public UInt32 Code { get; set; } = 1;

        [XmlAttribute("name")]
        public string Name { get; set; } = "";

        [XmlAttribute("description")]
        public string Description { get; set; } = "";

        public override string ToString() => $"[ {Code} ] {Name}";
    }
}
