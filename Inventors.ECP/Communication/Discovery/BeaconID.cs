using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Xml.Serialization;

namespace Inventors.ECP.Communication.Discovery
{
    public class BeaconID
    {
        public BeaconID(Manufacturer manufactuer, UInt16 device, UInt32 serial = 0)
        {
            ManufactureID = manufactuer;
            DeviceID = device;
            Serial = serial;
        }

        [XmlAttribute("manufacturer-id")]
        public Manufacturer ManufactureID { get; set; } = Manufacturer.Generic;

        [XmlAttribute("device-id")]
        public UInt16 DeviceID { get; set; } = 1;

        [XmlAttribute("device")]
        public UInt32 Serial { get; set; } = 0;

        [XmlIgnore]
        public string ID => String.Format(CultureInfo.InvariantCulture, "ecp://{0}.{1}", (UInt32) ManufactureID, DeviceID);

        [XmlIgnore]
        public string Data => Serial.ToString(CultureInfo.InvariantCulture);

        public override string ToString() =>
            String.Format(CultureInfo.InvariantCulture, "ecp://{0}.{1}/{2}", (UInt32)ManufactureID, DeviceID, Serial);
    }
}
