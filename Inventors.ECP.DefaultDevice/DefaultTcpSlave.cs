using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Text;
using System.Xml.Serialization;
using Inventors.ECP;
using Inventors.ECP.Communication;
using Inventors.ECP.Communication.Discovery;
using Inventors.ECP.Functions;
using Inventors.ECP.Messages;
using Inventors.Logging;

namespace Inventors.ECP.DefaultDevice
{
    [XmlRoot("default-tcp-slave")]
    public class DefaultTcpSlave :
        IHostedDevice
    {
        private TcpServerLayer commLayer = null;
        private DeviceSlave slave;

        public event Action<object, string> OnPropertyChanged;

        [Category("Communication")]
        [XmlIgnore]
        public uint Pings { get; set; }

        [Browsable(false)]
        [XmlIgnore]
        public Location Port { get; set; }

        [Browsable(false)]
        [XmlAttribute("port")]
        public ushort IPPort { get; set; }

        [Browsable(false)]
        [XmlAttribute("loopback")]
        public bool Loopback { get; set; }

        [Browsable(false)]
        [XmlIgnore]
        public bool IsOpen { get; private set; }

        [Browsable(false)]
        [XmlIgnore]
        public BeaconID Beacon { get; private set; } = new BeaconID(Manufacturer.Generic, 1, 1000);

        [Browsable(false)]
        [XmlIgnore]
        public string ID { get; set; }

        [Browsable(false)]
        [XmlIgnore]
        public DeviceState State { get; private set; } = DeviceState.STOPPED;

        [Browsable(false)]
        public string DeviceFile { get; set; }

        public DefaultTcpSlave SetLocation(Location location) 
        {
            Port = location;
            return this;
        }

        public void Run() => Start();

        public void Start()
        {
            if (!IsOpen)
            {
                if (Port is null)
                {
                    Port = new Location(Loopback ? IPAddress.Loopback : TcpServerLayer.LocalAddress, IPPort);

                }
                Log.Debug("Port: {0} (Beacon: {1})", Port, Beacon.ToString());
                commLayer = new TcpServerLayer(Port);
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
                State = DeviceState.RUNNING;
            }
        }

        public void Stop()
        {
            if (IsOpen)
            {
                slave.Close();
                IsOpen = false;
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
            OnPropertyChanged?.Invoke(this, nameof(Pings));
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

        public override string ToString()
        {
            return String.Format("DEFAULT TCP SLAVE", State.ToString());
        }

    }
}
