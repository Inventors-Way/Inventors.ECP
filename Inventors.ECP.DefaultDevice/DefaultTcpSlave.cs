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
        public Location Location { get; set; } = null;

        [Browsable(false)]
        [XmlAttribute("address")]
        public string IPPort 
        {
            get => Location.ToString();
            set => Location = Location.Parse(value);
        }

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
            Location = location;
            return this;
        }

        public void Start()
        {
            if (!IsOpen)
            {
                Log.Debug("Location: {0}", Location);
                commLayer = new TcpServerLayer(Location);
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

        public int Accept(DeviceIdentification func)
        {
            func.DeviceID = Location.DeviceID;
            func.ManufactureID = Location.ManufacturerID;
            func.Manufacture = "Inventors' Way";
            func.Device = "Default Device";
            func.MajorVersion = 1;
            func.MinorVersion = 2;
            func.PatchVersion = 3;
            func.EngineeringVersion = 4;
            func.Checksum = 5;
            func.SerialNumber = 1000;

            return 0;
        }

        public int Accept(Ping func)
        {
            func.Count = Pings++;
            OnPropertyChanged?.Invoke(this, nameof(Pings));
            return 0;
        }

        public int Accept(GetEndianness func)
        {
            Log.Status("Endianness: {0}", func.EqualEndianness);
            return 0;
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
