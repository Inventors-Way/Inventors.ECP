﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Xml.Serialization;
using Inventors.ECP;
using Inventors.ECP.Communication;
using Inventors.ECP.Communication.Discovery;
using Inventors.ECP.Functions;
using Inventors.ECP.Hosting;
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

        [XmlIgnore]
        public uint Pings { get; set; }

        [XmlIgnore]
        public string Port { get; set; }

        [XmlAttribute("port")]
        public ushort IPPort { get; set; }

        [XmlAttribute("loopback")]
        public bool Loopback { get; set; }

        [XmlIgnore]
        public bool IsOpen { get; private set; }

        [XmlIgnore]
        public BeaconID Beacon { get; private set; } = new BeaconID(1, 1, "Default Device");

        [XmlIgnore]
        public string ID { get; set; }

        [XmlIgnore]
        public DeviceState State { get; private set; } = DeviceState.STOPPED;

        public string DeviceFile { get; set; }

        public DefaultTcpSlave SetPort(IPAddress localAddress, ushort port) 
        {
            Port = CommunicationLayer.FormatTcpPort(localAddress, port);
            return this;
        }

        public void Run() => Start();

        public void Start()
        {
            if (!IsOpen)
            {
                if (string.IsNullOrEmpty(Port))
                {
                    SetPort(Loopback ? IPAddress.Loopback : TcpServerLayer.LocalAddress, IPPort);
                }

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
            return String.Format("DEFAULT TCP SLAVE [ {0} ]", State.ToString());
        }

    }
}
