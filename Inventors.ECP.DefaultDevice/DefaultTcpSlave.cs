using System;
using System.Collections.Generic;
using System.Text;
using Inventors.ECP;
using Inventors.ECP.Functions;
using Inventors.ECP.Messages;
using Inventors.Logging;

namespace Inventors.ECP.DefaultDevice
{
    public class DefaultTcpSlave
    {
        private TcpServerLayer commLayer = null;
        private DeviceSlave slave;

        public uint Pings { get; set; }

        public string Address { get; set; }

        public int Port { get; set; }

        public bool IsOpen { get; private set; }

        public string Device { get; set; } = "Default Device";

        public ushort DeviceID { get; set; } = 1;

        public uint ManufactureID { get; set; } = 1;

        public string Manufacturer { get; set; } = "Inventors' Way";

        public UInt32 SerialNumber { get; set; } = 1001;

        public void Start()
        {
            if (!IsOpen)
            {
                commLayer = new TcpServerLayer()
                {
                    Address = Address,
                    Port = Port
                };
                slave = new DeviceSlave(commLayer)
                {
                    FunctionListener = this,
                    MessageListener = this
                };

                slave.Add(new PrintfMessage());
                slave.Add(new DeviceIdentification());
                slave.Add(new Ping());
                slave.Add(new GetEndianness());
                slave.Open();

                IsOpen = true;
                Pings = 0;
            }
        }

        public void Stop()
        {
            if (IsOpen)
            {
                slave.Close();
                IsOpen = false;
            }
        }

        public bool Accept(DeviceIdentification func)
        {
            func.DeviceID = DeviceID;
            func.ManufactureID = ManufactureID;
            func.Manufacture = Manufacturer;
            func.Device = Device;
            func.MajorVersion = 1;
            func.MinorVersion = 2;
            func.PatchVersion = 3;
            func.EngineeringVersion = 4;
            func.Checksum = 5;
            func.SerialNumber = SerialNumber;
            Log.Status("Device Identification:");
            Log.Status("   Manufacturer: {0} [{1}]", func.Manufacture, func.ManufactureID);
            Log.Status("   Device      : {0} [{1}] (Checksum: {2})", func.Device, func.DeviceID, func.Checksum);
            Log.Status("   Firmware    : {0}", func.Version);

            return true;
        }

        public bool Accept(Ping func)
        {
            func.Count = Pings;
            ++Pings;

            return true;
        }

        public bool Accept(GetEndianness func)
        {
            Log.Status("Endianness: {0}", func.EqualEndianness);
            return true;
        }

        public void Accept(PrintfMessage msg)
        {
            Log.Status("PRINTF: {0}", msg.DebugMessage);
        }
    }
}
