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
        public static readonly byte CODE = 0x01;

        public DeviceIdentification() : 
            base(code: CODE, requestLength: 0, responseLength: 64) 
        { 
        }

        public static FunctionDispatcher CreateDispatcher()
        {
            return new FunctionDispatcher(CODE, () => new DeviceIdentification());
        }

        public override bool Dispatch(dynamic listener)
        {
            return listener.Accept(this);
        }

        [Category("Manufacturer")]
        [Description("The identifier of the manufacturer")]
        public UInt32 ManufactureID
        {
            get
            {
                return response.GetUInt32(0);
            }
            set
            {
                response.InsertUInt32(0, value);
            }
        }

        [Category("Manufacturer")]
        [Description("The name of the manufacturer")]
        public string Manufacture
        {
            get
            {
                return response.GetString(16, 24);
            }
            set
            {
                response.InsertString(16, 24, value);
            }
        }

        [Category("Device")]
        [Description("The identifier of the device")]
        public UInt16 DeviceID
        {
            get
            {
                return response.GetUInt16(4);
            }
            set
            {
                response.InsertUInt16(4, value);
            }
        }

        [Category("Device")]
        [Description("The name of the device")]
        public string Device
        {
            get
            {
                return response.GetString(40, 24);
            }
            set
            {
                response.InsertString(40, 24, value);
            }
        }


        [Category("Firmware")]
        [Description("Major Version")]
        public byte MajorVersion
        {
            get
            {
                return response.GetByte(10);
            }
            set
            {
                response.InsertByte(10, value);
            }
        }

        [Category("Firmware")]
        [Description("Major Version")]
        public byte MinorVersion
        {
            get
            {
                return response.GetByte(11);
            }
            set
            {
                response.InsertByte(11, value);
            }
        }

        [Category("Firmware")]
        [Description("Patch Version")]
        public byte PatchVersion
        {
            get
            {
                return response.GetByte(12);
            }
            set
            {
                response.InsertByte(12, value);
            }
        }

        [Category("Firmware")]
        [Description("Engineering Version")]
        public byte EngineeringVersion
        {
            get
            {
                return response.GetByte(13);
            }
            set
            {
                response.InsertByte(13, value);
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
                return response.GetUInt32(6);
            }
            set
            {
                response.InsertUInt32(6, value);
            }
        }

        [Category("Firmware")]
        [Description("The checksum number of device that is connected")]
        public UInt16 Checksum
        {
            get
            {
                return response.GetUInt16(14);
            }
            set
            {
                response.InsertUInt16(14, value);
            }
        }

        public override string ToString()
        {
            return "[0x01] Device Identification";
        }
    }
}
