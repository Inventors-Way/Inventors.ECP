﻿using Inventors.ECP.Functions;
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
        public DeviceType() { }

        public DeviceType(string port, DeviceIdentification id)
        {
            if (!(id is object))
                throw new ArgumentNullException(nameof(id));

            Port = port;
            ManufactureID = id.ManufactureID;
            Manufacture = id.Manufacture;
            DeviceID = id.DeviceID;
            Device = id.Device;
            SerialNumber = id.SerialNumber;
        }

        [XmlAttribute("manufacturer-id")]
        public UInt32 ManufactureID { get; set; } = 1;

        [XmlAttribute("manufacturer")]
        public string Manufacture { get; set; } = "";

        [XmlAttribute("device-id")]
        public UInt16 DeviceID { get; set; } = 1;

        [XmlAttribute("device")]
        public string Device { get; set; } = "";

        [XmlAttribute("serial-number")]
        public UInt32 SerialNumber { get; set; } = 1;

        [XmlIgnore]
        public string Port { get; set; }

        public override string ToString() =>
            string.Format(CultureInfo.CurrentCulture, "{0}.{1} {2} [{3}]", ManufactureID, DeviceID, Device, SerialNumber);
    }
}
