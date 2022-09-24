using Inventors.ECP.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

        [XmlArray("parameters")]
        [XmlArrayItem("int", typeof(IntegerParameter))]
        [XmlArrayItem("bool", typeof(BooleanParameter))]
        [XmlArrayItem("string", typeof(StringParameter))]
        [XmlArrayItem("double", typeof(DoubleParameter))]
        public List<Parameter> Parameters { get; } = new List<Parameter>();
    }
}
