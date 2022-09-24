using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Inventors.ECP.Parameters
{
    public abstract class Parameter
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        public abstract object GetValue();
    }
}
