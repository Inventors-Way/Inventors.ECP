using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Xml.Serialization;

namespace Inventors.ECP.Communication.Discovery
{
    public class BeaconID
    {
        public BeaconID(UInt32 manufactuer, UInt16 device, string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            ManufactureID = manufactuer;
            DeviceID = device;
            Device = name;
        }

        [XmlAttribute("manufacturer-id")]
        public UInt32 ManufactureID { get; set; } = 1;

        [XmlAttribute("device-id")]
        public UInt16 DeviceID { get; set; } = 1;

        [XmlAttribute("device")]
        public string Device { get; set; } = "Default Device";

        [XmlIgnore]
        public string ID => String.Format(CultureInfo.CurrentCulture, "ECP-{0}-{1}", ManufactureID, DeviceID);

        [XmlIgnore]
        public string Data => String.Format(CultureInfo.CurrentCulture, "[{0}.{1}] {2}", ManufactureID, DeviceID, Device);

        public override string ToString() => ID;
    }
}
