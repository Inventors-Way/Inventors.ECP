using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Inventors.ECP
{
    public class MessageAnalyser
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("code")]
        public byte Code { get; set; }

        [XmlAttribute("script")]
        public string Script { get; set; }

        [XmlAttribute("signals")]
        public int Signals { get; set; }

        public event EventHandler<DeviceMessage> OnMessage;

        public void Accept(DeviceMessage message)
        {
            OnMessage?.Invoke(this, message);
        }
    }
}
