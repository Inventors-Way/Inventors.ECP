using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using Inventors.ECP;
using Inventors.ECP.DefaultDevice;
using Inventors.ECP.Functions;
using Inventors.ECP.Messages;
using System.Net;
using System.Threading;
using Inventors.ECP.Discovery;

namespace Inventors.ECP.UnitTests.ApplicationLayer
{
    [TestClass]
    public class TcpLayerTest
    {
        [TestMethod]
        public void Ping()
        {
            var ping = new Ping();

            TC.Slave.Pings = 0;
            TC.Device.Execute(ping);
            Assert.IsTrue(ping.Count == 0);
            TC.Device.Execute(ping);
            Assert.IsTrue(ping.Count == 1);
            TC.Device.Execute(ping);
            Assert.IsTrue(ping.Count == 2);
        }

        [TestMethod]
        public void DeviceIdentification()
        {
            var info = new DeviceIdentification();
            TC.Device.Execute(info);
            Assert.AreEqual(expected: TC.Slave.Identification.Device, actual: info.Device);
            Assert.AreEqual(expected: TC.Slave.Identification.DeviceID, actual: info.DeviceID);
        }

        [TestMethod]
        public void GetEndianity()
        {
            var endianity = new GetEndianness();
            TC.Device.Execute(endianity);
            Assert.AreEqual(expected: true, actual: endianity.EqualEndianness);
        }

        [TestMethod]
        public void BeaconTest()
        {
            List<BeaconLocation> locations = new List<BeaconLocation>();
            var probe = new Probe(TC.Slave.Identification.BeaconName);
            probe.BeaconsUpdated += (beacons) => locations = beacons.ToList();
            probe.Start();
            Thread.Sleep(500);
            probe.Stop();
            Assert.AreEqual(expected: 1, actual: locations.Count);
            var beacon = locations[0];
            var data = DeviceType.Create(locations[0].Data);
            Assert.AreEqual(expected: TC.Slave.Identification.DeviceID, actual: data.DeviceID);
        }

        [TestMethod]
        public void GetTcpPorts()
        {
            var device = TC.Device;

            Thread.Sleep(2500);
            device.Close();
            var devices = device.GetAvailableDevices();
            device.Open();
            Assert.AreEqual(expected: 1, actual: devices.Count);
        }
    }
}
