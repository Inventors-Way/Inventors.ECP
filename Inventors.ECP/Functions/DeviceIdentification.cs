using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventors.ECP;

namespace Inventors.ECP.Functions
{
    public class DeviceIdentification :
       Function
    {
        private readonly static byte ResponseLength = 64;

        public DeviceIdentification() : base(0x01) { }

        protected override bool IsResponseValid()
        {
            return response.Length == ResponseLength;
        }

        [Category("Manufacturer")]
        [Description("The identifier of the manufacturer")]
        public UInt32 ManufactureID
        {
            get
            {
                return response != null ? response.GetUInt32(0) : 0;
            }
        }

        [Category("Manufacturer")]
        [Description("The name of the manufacturer")]
        public string Manufacture
        {
            get
            {
                return response != null ? response.GetString(16, 24) : "";
            }
        }

        [Category("Device")]
        [Description("The identifier of the device")]
        public UInt16 DeviceID
        {
            get
            {
                return response != null ? response.GetUInt16(4) : (UInt16) 0;
            }
        }

        [Category("Device")]
        [Description("The name of the device")]
        public string Device
        {
            get
            {
                return response != null ? response.GetString(40, 24) : "";
            }
        }


        [Category("Firmware")]
        [Description("Major Version")]
        public byte MajorVersion
        {
            get
            {
                if (response != null)
                {
                    return response.GetByte(10);
                }
                else
                    return 0;
            }
        }

        [Category("Firmware")]
        [Description("Major Version")]
        public byte MinorVersion
        {
            get
            {
                if (response != null)
                {
                    return response.GetByte(11);
                }
                else
                    return 0;
            }
        }

        [Category("Firmware")]
        [Description("Patch Version")]
        public byte PatchVersion
        {
            get
            {
                if (response != null)
                {
                    return response.GetByte(12);
                }
                else
                    return 0;
            }
        }

        [Category("Firmware")]
        [Description("Engineering Version")]
        public byte EngineeringVersion
        {
            get
            {
                if (response != null)
                {
                    return response.GetByte(13);
                }
                else
                    return 0;
            }
        }

        [Category("Firmware")]
        [Description("Version")]
        public string Version
        {
            get
            {
                if (response != null)
                {
                    return EngineeringVersion == 0 ?
                           String.Format("{0}.{1}.{2}", MajorVersion, MinorVersion, PatchVersion) :
                           String.Format("{0}.{1}.{2}.r{3}", MajorVersion, MinorVersion, PatchVersion, EngineeringVersion);
                }
                else
                    return "";
            }
        }

        [Category("Device")]
        [Description("The serial number of device that is connected")]
        public UInt32 SerialNumber
        {
            get
            {
                if (response != null)
                {
                    return response.GetUInt32(6);
                }
                else
                    return 0;
            }
        }

        [Category("Firmware")]
        [Description("The checksum number of device that is connected")]
        public UInt16 Checksum
        {
            get
            {
                if (response != null)
                {
                    return response.GetUInt16(14);
                }
                else
                    return 0;
            }
        }

        public override string ToString()
        {
            return "[0x01] Device Identification";
        }
    }
}
