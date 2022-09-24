using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Inventors.ECP.Parameters
{
    public class IntegerParameter :
        Parameter
    {
        [XmlAttribute("value")]
        public int Value { get; set; }

        public override object GetValue() => Value;
    }
}
