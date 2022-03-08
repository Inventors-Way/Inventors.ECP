using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Inventors.ECP
{
    [XmlRoot("debug-signals")]
    public class DebugSpecification
    {
        [XmlAttribute("address")]
        public byte Address { get; set; } = 0;

        [XmlAttribute("name")]
        public string Name { get; set; } = "Default";

        [XmlElement("debug-signal")] 
        public List<DebugSignal> Signals { get; } = new List<DebugSignal>();

        public void Initialize()
        {
            if (Signals is null)
                return;

            for (int n = 0; n < Signals.Count; n++)
            {
                Signals[n].Code = (uint) (n + 1);
            }
        }
    }
}
