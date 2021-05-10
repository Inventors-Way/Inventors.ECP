using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Inventors.ECP;

namespace Inventors.ECP.Functions
{
    public class DeviceIdentification :
       DeviceFunction
    {
        public override byte Code => 0x01;

        public DeviceIdentification() : 
            base(requestLength: 0, responseLength: 64) 
        { 
        }

        public override FunctionDispatcher CreateDispatcher() =>
            new FunctionDispatcher(Code, () => new DeviceIdentification());

        public override int Dispatch(dynamic listener) => listener.Accept(this);

        [XmlIgnore]
        [Category("Manufacturer")]
        [Description("The identifier of the manufacturer")]
        public Manufacturer ManufactureID
        {
            get => (Manufacturer) Response.GetUInt32(0);
            set => Response.InsertUInt32(0, (UInt32) value);
        }

        [XmlIgnore]
        [Category("Manufacturer")]
        [Description("The name of the manufacturer")]
        public string Manufacture
        {
            get => Response.GetString(16, 24);
            set => Response.InsertString(16, 24, value);
        }

        [XmlIgnore]
        [Category("Device")]
        [Description("The identifier of the device")]
        public UInt16 DeviceID
        {
            get => Response.GetUInt16(4);
            set => Response.InsertUInt16(4, value);
        }

        [XmlIgnore]
        [Category("Device")]
        [Description("The name of the device")]
        public string Device
        {
            get => Response.GetString(40, 24);
            set => Response.InsertString(40, 24, value);
        }


        [XmlIgnore]
        [Category("Firmware")]
        [Description("Major Version")]
        public byte MajorVersion
        {
            get => Response.GetByte(10);
            set => Response.InsertByte(10, value);
        }

        [XmlIgnore]
        [Category("Firmware")]
        [Description("Major Version")]
        public byte MinorVersion
        {
            get => Response.GetByte(11);
            set => Response.InsertByte(11, value);
        }

        [XmlIgnore]
        [Category("Firmware")]
        [Description("Patch Version")]
        public byte PatchVersion
        {
            get => Response.GetByte(12);
            set => Response.InsertByte(12, value);
        }

        [XmlIgnore]
        [Category("Firmware")]
        [Description("Engineering Version")]
        public byte EngineeringVersion
        {
            get => Response.GetByte(13);
            set => Response.InsertByte(13, value);
        }

        [XmlIgnore]
        [Category("Firmware")]
        [Description("Version")]
        public string Version => EngineeringVersion == 0 ?
                                 $"{MajorVersion}.{MinorVersion}.{PatchVersion}" :
                                 $"{MajorVersion}.{MinorVersion}.{PatchVersion}.r{EngineeringVersion}";

        [XmlIgnore]
        [Category("Device")]
        [Description("The serial number of device that is connected")]
        public UInt32 SerialNumber
        {
            get => Response.GetUInt32(6);
            set => Response.InsertUInt32(6, value);
        }

        [XmlIgnore]
        [Category("Firmware")]
        [Description("The checksum number of device that is connected")]
        public UInt16 Checksum
        {
            get => Response.GetUInt16(14);
            set => Response.InsertUInt16(14, value);
        }

        public override string ToString() => "[0x01] Device Identification";
    }
}
