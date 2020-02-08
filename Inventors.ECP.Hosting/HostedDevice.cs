using Inventors.ECP.Communication;
using Inventors.ECP.Communication.Discovery;
using Inventors.ECP.Functions;
using Inventors.ECP.Messages;
using Inventors.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Inventors.ECP.Hosting
{
    [XmlRoot("hosted-device")]
    public class HostedDevice :
        IHostedDevice
    {
        private TcpServerLayer commLayer = null;
        private DeviceSlave slave;

        [XmlIgnore]
        public DeviceState State { get; private set; }

        [XmlIgnore]
        public string DeviceFile { get; set; }

        [XmlIgnore]
        public uint Pings { get; private set; }

        public string Port { get; set; }

        [XmlIgnore]
        public BeaconID Beacon { get; private set; } = new BeaconID(1, 1, "Default Device");

        private string GetPort()
        {
            if (string.IsNullOrEmpty(Port))
            {

            }

            return Port;
        }

        public void Run()
        {
            if (State == DeviceState.STOPPED)
            {
                commLayer = new TcpServerLayer(Beacon, GetPort());
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

                State = DeviceState.RUNNING;
                Pings = 0;
            }

        }

        public void Stop()
        {
            if (State == DeviceState.RUNNING)
            {
                slave.Close();
                State = DeviceState.STOPPED;
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
            return true;
        }

        public void Accept(PrintfMessage msg)
        {
            Log.Status("PRINTF: {0}", msg.DebugMessage);
        }
    }
}
