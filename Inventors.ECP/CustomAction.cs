using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Inventors.ECP
{
    public class CustomAction
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("script")]
        public string Script { get; set; }
    }
}
