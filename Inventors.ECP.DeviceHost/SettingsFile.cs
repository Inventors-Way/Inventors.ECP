using Inventors.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Inventors.ECP.DeviceHost
{
    [XmlRoot("settings")]
    public class SettingsFile
    {
        [XmlAttribute("level")]
        public LogLevel Level { get; set; } = LogLevel.STATUS;

        [XmlArray("devices")]
        [XmlArrayItem("device")]
        public List<Loader> Devices { get; } = new List<Loader>();
    }
}
