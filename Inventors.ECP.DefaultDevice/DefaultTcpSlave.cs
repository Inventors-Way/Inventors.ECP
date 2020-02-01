using System;
using System.Collections.Generic;
using System.Text;
using Inventors.ECP;
using Inventors.ECP.Communication;
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

        public string Port { get; set; }

        public bool IsOpen { get; private set; }

        public DeviceData Identification { get; } = new DeviceData()
        {
            DeviceID = 1,
            Device = "Default Device",
            ManufactureID = 1,
            Manufacture = "Inventors' Way",
            SerialNumber = 1001
        };

        public void Start()
        {
            if (!IsOpen)
            {
                commLayer = new TcpServerLayer()
                {
                    Port = Port
                };
                slave = new DeviceSlave(commLayer, Identification)
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
            slave.Accept(func);
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
