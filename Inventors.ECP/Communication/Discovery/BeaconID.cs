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
            Serial = serial.ToString(CultureInfo.InvariantCulture);
        }

        [XmlAttribute("manufacturer-id")]
        public Manufacturer ManufactureID { get; set; } = Manufacturer.Generic;

        [XmlAttribute("device-id")]
        public UInt16 DeviceID { get; set; } = 1;

        [XmlAttribute("device")]
        public string Serial { get; set; } = "0".ToString(CultureInfo.InvariantCulture);

        [XmlIgnore]
        public string ID => String.Format(CultureInfo.InvariantCulture, "ECP:{0}:{1}", ManufactureID, DeviceID);

        [XmlIgnore]
        public string Data => Serial;

        public override string ToString() => ID;
    }
}
