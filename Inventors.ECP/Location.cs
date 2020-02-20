using Inventors.ECP.Communication;
using Inventors.ECP.Communication.Discovery;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace Inventors.ECP
{
    /*
     * Detailed description, the encoding string is of the form:
     * 
     *    serial://COM1{/MID.DID/SERIAL_NUMBER}
     *    tcp://192.172.0.1:10001{/MID.DID/SERIAL_NUMBER}
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
        private static readonly string networkStr = "tcp://";

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

        [XmlIgnore]
        public BeaconID BeaconID => new BeaconID(manufactuer: ManufacturerID, device: DeviceID, serial: SerialNumber);

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

            Protocol = CommunicationProtocol.SERIAL;

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
            
            if (UInt32.TryParse(tokens[0], out UInt32 mid))
            {
                if (ValidateManufacturer(mid))
                {
                    ManufacturerID = (Manufacturer) mid;
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

            if (ushort.TryParse(tokens[1], out ushort did))
            {
                DeviceID = did;
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

            Protocol = CommunicationProtocol.NETWORK;

            if (tokens.Length == 1)
            {
                ParseNetworkAddress(tokens[0]);
            }
            else if (tokens.Length == 2)
            {
                ParseNetworkAddress(tokens[0]);
                ParseDevice(tokens[1]);
            }
            else if (tokens.Length == 3)
            {
                ParseNetworkAddress(tokens[0]);
                ParseDevice(tokens[1]);
                ParseSerialNumber(tokens[2]);
            }
            else
            {
                throw new ArgumentException(str + " is not a valid string encoding a network device location");
            }
        }

        private void ParseNetworkAddress(string str)
        {
            if (Regex.IsMatch(str, "^loopback:\\d{1,5}$"))
            {
                var tokens = str.Split(':');
                Address = IPAddress.Loopback.ToString();
                Port = ushort.Parse(tokens[1]);
            }
            else if (Regex.IsMatch(str, "^local:\\d{1,5}$"))
            {
                var tokens = str.Split(':');
                Address = TcpServerLayer.LocalAddress.ToString();
                Port = ushort.Parse(tokens[1]);
            }
            else
            {
                if (!ValidateNetworkAddress(str))
                    throw new ArgumentException(str + " is not a valid network address");

                var tokens = str.Split(':');

                if (tokens.Length == 2)
                {
                    Address = tokens[0];
                    Port = ushort.Parse(tokens[1]);
                }
            }
        }

        private static bool ValidateSerialPort(string token) => 
            Regex.IsMatch(token, "^COM[1-9][0-9]*$");

        private static bool ValidateNetworkAddress(string token) => 
            Regex.IsMatch(token, "^\\d{1,3}\\.\\d{1,3}\\.\\d{1,3}\\.\\d{1,3}:\\d{1,5}$");

        private static bool ValidateDevice(string token) => 
            Regex.IsMatch(token, "^[1-9][0-9]*\\.[1-9][0-9]*$");

        private static bool ValidateManufacturer(UInt32 value) =>
            Enum.IsDefined(typeof(Manufacturer), value);

        public Location() { }

        public Location(IPEndPoint endpoint, BeaconID id)
        {
            if (endpoint is null)
                throw new ArgumentNullException(nameof(endpoint));

            Protocol = CommunicationProtocol.NETWORK;
            Address = endpoint.Address.ToString();
            Port = (ushort) endpoint.Port;
            DeviceID = id.DeviceID;
            ManufacturerID = id.ManufactureID;
            SerialNumber = id.Serial;
        }

        public Location(CommunicationProtocol protocol, 
                        string address, 
                        UInt16 deviceId,
                        Manufacturer manufacturerId,
                        UInt32 serialNumber) 
        {
            if (address is null)
                throw new ArgumentNullException(nameof(address));

            if (protocol == CommunicationProtocol.NETWORK)
            {
                if (!ValidateNetworkAddress(address))
                    throw new ArgumentException(address + "is not a valid network address");

                ParseNetworkAddress(address);
            }
            else if (protocol == CommunicationProtocol.SERIAL)
            {
                if (!ValidateSerialPort(address))
                    throw new ArgumentException(address + " is not a valid serial port");

                Address = address;
            }
            else
            {
                throw new ArgumentException(protocol.ToString() + " is not a valid protocol");
            }
            Protocol = protocol;
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
                    {
                        if (Address == IPAddress.Loopback.ToString())
                        {
                            return String.Format(CultureInfo.InvariantCulture,
                                                 "{0}{1}:{2}{3}{4}",
                                                 networkStr,
                                                 "loopback",
                                                 Port,
                                                 DeviceString(),
                                                 SerialString());
                        }
                        else if (Address == TcpServerLayer.LocalAddress.ToString())
                        {
                            return String.Format(CultureInfo.InvariantCulture,
                                                 "{0}{1}:{2}{3}{4}",
                                                 networkStr,
                                                 "local",
                                                 Port,
                                                 DeviceString(),
                                                 SerialString());
                        }
                        else
                        {
                            return String.Format(CultureInfo.InvariantCulture,
                                                 "{0}{1}:{2}{3}{4}",
                                                 networkStr,
                                                 Address,
                                                 Port,
                                                 DeviceString(),
                                                 SerialString());
                        }
                    }
                default:
                    return "";
            }
        }
    }
}
