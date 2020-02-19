using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
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
        private static readonly string serialStr = "serial://";
        private static readonly string networkStr = "network://";

        [XmlIgnore]
        public CommunicationProtocol Protocol { get; private set; } = CommunicationProtocol.SERIAL;

        [XmlIgnore]
        public string Address { get; private set; } = "";

        [XmlIgnore]
        public ushort Port { get; private set; } = 0;

        [XmlIgnore]
        public UInt16 DeviceID { get; private set; } = 0;

        [XmlIgnore]
        public Manufacturer ManufacturerID { get; private set; } = Manufacturer.Invalid;

        [XmlIgnore]
        public UInt32 SerialNumber { get; private set; } = 0;

        public static Location Parse(string str)
        {
            Location retValue = new Location();

            if (str is null)
                throw new ArgumentNullException(nameof(str));

            if (str.StartsWith(serialStr, StringComparison.InvariantCulture))
            {
                retValue.ParseSerial(str.Substring(serialStr.Length));
            }
            else if (str.StartsWith(networkStr, StringComparison.InvariantCulture))
            {
                retValue.ParseNetwork(str.Substring(networkStr.Length));
            }
            else // if no protocol is specified then default to serial
            {
                retValue.ParseSerialPortAddress(str);
            }

            return retValue;
        }

        private void ParseSerial(string str)
        {
            var tokens = str.Split('/');

            if (tokens.Length == 1)
            {
                ParseSerialPortAddress(tokens[0]);
            }
            else if (tokens.Length == 2)
            {
                ParseSerialPortAddress(tokens[0]);
                ParseDevice(tokens[1]);
            }
            else if (tokens.Length == 3)
            {
                ParseSerialPortAddress(tokens[0]);
                ParseDevice(tokens[1]);
                ParseSerialNumber(tokens[2]);
            }
            else
            {
                throw new ArgumentException("{0} is not a valid string encoding a serial device location");
            }
        }

        private void ParseSerialPortAddress(string address)
        {
            if (!ValidateSerialPort(address))
                throw new ArgumentException(String.Format(CultureInfo.InvariantCulture, "{0} is not a valid serial port", address));

            Address = address;
        }

        private void ParseDevice(string device)
        {
            if (!ValidateDevice(device))
                throw new ArgumentException(String.Format(CultureInfo.InvariantCulture, "{0} is not a valid device encoding", device));

            var tokens = device.Split('.');
            
            if (UInt32.TryParse(tokens[0], out UInt32 result))
            {
                var manufacturer = (Manufacturer)result;

                if (Enum.TryParse<Manufacturer>(manufacturer.ToString(), out Manufacturer mid))
                {
                    ManufacturerID = mid;
                }
                else
                {
                    throw new ArgumentException(String.Format(CultureInfo.InvariantCulture, "{0} is not a valid manufacturer", tokens[0]));
                }
            }
            else
            {
                throw new ArgumentException("Not a valid integer: " + tokens[0]);
            }

            if (ushort.TryParse(tokens[1], out ushort id))
            {
                DeviceID = id;
            }
            else
            {
                throw new ArgumentException(String.Format(CultureInfo.InvariantCulture, "{0} is not a valid device ID", tokens[1]));
            }
        }

        private void ParseSerialNumber(string str)
        {
            if (UInt32.TryParse(str, out UInt32 result))
            {
                SerialNumber = result;
            }
            else
            {
                throw new ArgumentException(str + " is not a valid serial number");
            }
        }

        private void ParseNetwork(string str)
        {
            var tokens = str.Split('/');

            if (tokens.Length == 1)
            {

            }
            else if (tokens.Length == 2)
            {

            }
            else if (tokens.Length == 3)
            {

            }
            else
            {
                throw new ArgumentException("{0} is not a valid string encoding a network device location");
            }
        }

        private static bool ValidateSerialPort(string token) => 
            Regex.IsMatch(token, "^COM[1-9][0-9]*$");

        private static bool ValidateNetworkAddress(string token) => 
            Regex.IsMatch(token, "^\\d{1,3}\\.\\d{1,3}\\.\\d{1,3}\\.\\d{1,3}:\\d{1,5}$");

        private static bool ValidateDevice(string token) => 
            Regex.IsMatch(token, "^[1-9][0-9]*\\.[1-9][0-9]*$");

        public Location() { }

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

        private string DeviceString() =>
            DeviceID > 0 ? String.Format(CultureInfo.InvariantCulture, "/{0}.{1}", DeviceID, (UInt32) ManufacturerID) : "";

        private string SerialString() =>
            DeviceID > 0 && SerialNumber > 0 ? String.Format(CultureInfo.InvariantCulture, "/{0}", SerialNumber) : "";

        public override string ToString()
        {
            switch (Protocol)
            {
                case CommunicationProtocol.SERIAL:
                    return String.Format(CultureInfo.InvariantCulture, 
                                         "{0}{1}{2}{3}", 
                                         serialStr, 
                                         Address, 
                                         DeviceString(), 
                                         SerialString());
                case CommunicationProtocol.NETWORK:
                    return String.Format(CultureInfo.InvariantCulture,
                                         "{0}{1}:{2}{3}{4}",
                                         serialStr,
                                         Address,
                                         Port,
                                         DeviceString(),
                                         SerialString());
                default:
                    return "";
            }
        }
    }
}
