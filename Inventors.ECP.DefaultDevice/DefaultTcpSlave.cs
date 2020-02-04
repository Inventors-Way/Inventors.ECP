using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Inventors.ECP;
using Inventors.ECP.Communication;
using Inventors.ECP.Discovery;
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

        public BeaconID Beacon { get; private set; } = new BeaconID(1, 1, "Default Device");

        public DefaultTcpSlave SetPort(IPAddress localAddress, ushort port) 
        {
            Port = CommunicationLayer.FormatTcpPort(localAddress, port);
            return this;
        }

        public void Start()
        {
            if (!IsOpen)
            {
                commLayer = new TcpServerLayer(Beacon, Port);
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
            func.DeviceID = 1;
            func.ManufactureID = 1;
            func.Manufacture = "Inventors' Way";
            func.Device = "Default Device";
            func.MajorVersion = 1;
            func.MinorVersion = 2;
            func.PatchVersion = 3;
            func.EngineeringVersion = 4;
            func.Checksum = 5;
            func.SerialNumber = 1000;

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
