using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Inventors.ECP.Logging
{
    public class SeqLogging :
        LogConfig
    {
        [XmlAttribute("url")]
        public string Url { get; set; }

        [XmlAttribute("api-key")]
        public string ApiKey { get; set; }

        public override void Visit(ILogConfigVisitor visitor) => visitor.Accept(this);
    }
}
