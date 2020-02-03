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

        public DeviceType Identification { get; } = new DeviceType()
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
                commLayer = new TcpServerLayer(Identification.BeaconName)
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
            return true;
        }

        public bool Accept(Ping func)
        {
            func.Count = Pings++;
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
