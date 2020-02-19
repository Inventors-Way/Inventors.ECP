using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Inventors.ECP
{
    /*
     * Detailed description, the encoding string is of the form:
     * 
     *    serial://COM1{/MID.DID/SERIAL_NUMBER}
     *    network://192.172.0.1:10001{/MID.DID/SERIAL_NUMBER}
     *    COM1{/MID.DID}{/SERIAL_NUMBER}
     * 
     * where {} denotes optional parts
     * 
     * - Protocol: serial / network
     * - Address:
     * -- Serial: the COM port
     * -- Network: the IP Address
     * - Port
     * -- Serial: Not allowed
     * -- Network: The port number of server
     * -
    */
    /// <summary>
    /// Encodes the location of a device. 
    /// </summary>
    [XmlRoot("device-location")]
    public class Location
    {
        private static readonly string serialStr = "serial";
        private static readonly string networkStr = "network";

        [XmlIgnore]
        public CommunicationProtocol Protocol { get; private set; }

        [XmlIgnore]
        public string Address { get; private set; }

        [XmlIgnore]
        public ushort Port { get; private set; }

        [XmlIgnore]
        public UInt16 DeviceID { get; private set; }

        [XmlIgnore]
        public Manufacturer ManufacturerID { get; private set; }

        [XmlIgnore]
        public UInt32 SerialNumber { get; private set; }

        public static Location Parse(string str)
        {
            Location retValue = null;

            if (str is null)
                throw new ArgumentNullException(nameof(str));

            if (str.StartsWith(serialStr, StringComparison.InvariantCulture))
            {

            }
            else if (str.StartsWith(networkStr, StringComparison.InvariantCulture))
            {

            }
            else // if no protocol is specified then default to serial
            {

            }

            return retValue;
        }

        public Location(CommunicationProtocol protocol, 
                         string address, 
                         ushort port, 
                         UInt16 deviceId,
                         Manufacturer manufacturerId,
                         UInt32 serialNumber) 
        {
            Protocol = protocol;
            Address = address;
            Port = port;
            DeviceID = deviceId;
            ManufacturerID = manufacturerId;
            SerialNumber = serialNumber;
        } 

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
