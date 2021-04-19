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
        private static DebugSignal noneInstance;

        public static DebugSignal None 
        { 
            get
            {
                if (noneInstance is null)
                    noneInstance = new DebugSignal()
                    {
                        Code = 0,
                        Name = "None",
                        Description = "Represent a debug signal that is not enabled"
                    };

                return noneInstance;
            }            
        }

        [XmlAttribute("code")]
        public UInt32 Code { get; set; } = 1;

        [XmlAttribute("name")]
        public string Name { get; set; } = "";

        [XmlAttribute("description")]
        public string Description { get; set; } = "";

        public override string ToString() => $"[ {Code} ] {Name}";
    }
}
