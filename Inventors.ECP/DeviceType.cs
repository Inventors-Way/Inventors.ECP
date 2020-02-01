using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Inventors.ECP
{
    [XmlRoot("device-id")]
    public class DeviceType
    {
        [XmlAttribute("manufacturer-id")]
        public UInt32 ManufactureID { get; set; } = 1;

        [XmlAttribute("manufacturer")]
        public string Manufacture { get; set; } = "";

        [XmlAttribute("device-id")]
        public UInt16 DeviceID { get; set; } = 1;

        [XmlAttribute("device")]
        public string Device { get; set; } = "";

        [XmlAttribute("major-attribute")]
        public byte MajorVersion { get; set; }

        [XmlAttribute("minor-attribute")]
        public byte MinorVersion { get; set; }

        [XmlAttribute("path-version")]
        public byte PatchVersion { get; set; }

        [XmlAttribute("engineering-version")]
        public byte EngineeringVersion { get; set; }

        [XmlIgnore]
        public string Version => EngineeringVersion == 0 ?
                                 String.Format(CultureInfo.CurrentCulture, "{0}.{1}.{2}", MajorVersion, MinorVersion, PatchVersion) :
                                 String.Format(CultureInfo.CurrentCulture, "{0}.{1}.{2}.r{3}", MajorVersion, MinorVersion, PatchVersion, EngineeringVersion);

        [XmlAttribute("serial-number")]
        public UInt32 SerialNumber { get; set; } = 1;

        [XmlAttribute("checksum")]            
        public UInt16 Checksum { get; set; } = 0;

        [XmlIgnore]
        public string BeaconName => String.Format(CultureInfo.CurrentCulture, "ECP-{0}-{1}", ManufactureID, DeviceID);

        [XmlIgnore]
        public string Port { get; set; }

        public static DeviceType Create(string xml)
        {
            DeviceType retValue = null;
            XmlSerializer serializer = new XmlSerializer(typeof(DeviceType));
            XmlReaderSettings settings = new XmlReaderSettings() { };

            using (var stream = new StringReader(xml))
            {
                using (var reader = XmlReader.Create(stream, settings))
                {
                    retValue = (DeviceType)serializer.Deserialize(reader);
                }
            }

            return retValue;
        }

        public string ToXML()
        {
            using (var writer = new StringWriter())
            {
                XmlSerializer serializer = new XmlSerializer(typeof(DeviceType));
                serializer.Serialize(writer, this);
                return writer.ToString();
            }
        }
    }
}
