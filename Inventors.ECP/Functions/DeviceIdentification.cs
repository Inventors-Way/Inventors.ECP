using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventors.ECP;
using Inventors.ECP.Communication;

namespace Inventors.ECP.Functions
{
    public class DeviceIdentification :
       DeviceFunction
    {
        public static readonly byte CODE = 0x01;

        public DeviceIdentification() : 
            base(code: CODE, requestLength: 0, responseLength: 64) 
        { 
        }

        public override FunctionDispatcher CreateDispatcher()
        {
            return new FunctionDispatcher(CODE, () => new DeviceIdentification());
        }

        public override bool Dispatch(dynamic listener)
        {
            return listener.Accept(this);
        }

        [Category("Manufacturer")]
        [Description("The identifier of the manufacturer")]
        public Manufacturer ManufactureID
        {
            get
            {
                return (Manufacturer) Response.GetUInt32(0);
            }
            set
            {
                Response.InsertUInt32(0, (UInt32) value);
            }
        }

        [Category("Manufacturer")]
        [Description("The name of the manufacturer")]
        public string Manufacture
        {
            get
            {
                return Response.GetString(16, 24);
            }
            set
            {
                Response.InsertString(16, 24, value);
            }
        }

        [Category("Device")]
        [Description("The identifier of the device")]
        public UInt16 DeviceID
        {
            get
            {
                return Response.GetUInt16(4);
            }
            set
            {
                Response.InsertUInt16(4, value);
            }
        }

        [Category("Device")]
        [Description("The name of the device")]
        public string Device
        {
            get
            {
                return Response.GetString(40, 24);
            }
            set
            {
                Response.InsertString(40, 24, value);
            }
        }


        [Category("Firmware")]
        [Description("Major Version")]
        public byte MajorVersion
        {
            get
            {
                return Response.GetByte(10);
            }
            set
            {
                Response.InsertByte(10, value);
            }
        }

        [Category("Firmware")]
        [Description("Major Version")]
        public byte MinorVersion
        {
            get
            {
                return Response.GetByte(11);
            }
            set
            {
                Response.InsertByte(11, value);
            }
        }

        [Category("Firmware")]
        [Description("Patch Version")]
        public byte PatchVersion
        {
            get
            {
                return Response.GetByte(12);
            }
            set
            {
                Response.InsertByte(12, value);
            }
        }

        [Category("Firmware")]
        [Description("Engineering Version")]
        public byte EngineeringVersion
        {
            get
            {
                return Response.GetByte(13);
            }
            set
            {
                Response.InsertByte(13, value);
            }
        }

        [Category("Firmware")]
        [Description("Version")]
        public string Version
        {
            get
            {
                if (Response != null)
                {
                    return EngineeringVersion == 0 ?
                           String.Format(CultureInfo.CurrentCulture, "{0}.{1}.{2}", MajorVersion, MinorVersion, PatchVersion) :
                           String.Format(CultureInfo.CurrentCulture, "{0}.{1}.{2}.r{3}", MajorVersion, MinorVersion, PatchVersion, EngineeringVersion);
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
                return Response.GetUInt32(6);
            }
            set
            {
                Response.InsertUInt32(6, value);
            }
        }

        [Category("Firmware")]
        [Description("The checksum number of device that is connected")]
        public UInt16 Checksum
        {
            get
            {
                return Response.GetUInt16(14);
            }
            set
            {
                Response.InsertUInt16(14, value);
            }
        }

        public override string ToString()
        {
            return "[0x01] Device Identification";
        }
    }
}
