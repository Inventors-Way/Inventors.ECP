using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Inventors.ECP.Parameters
{
    public class DoubleParameter :
        Parameter
    {
        [XmlAttribute("value")]
        public double Value { get; set; }

        public override object GetValue() => Value;
    }
}
