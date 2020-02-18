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
using Inventors.ECP.Communication.Discovery;
using Inventors.Logging;
using System.Globalization;

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
            Assert.AreEqual(expected: TC.Slave.Beacon.Serial, actual: info.SerialNumber.ToString(CultureInfo.InvariantCulture));
            Assert.AreEqual(expected: TC.Slave.Beacon.DeviceID, actual: info.DeviceID);
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
            var probe = new Probe(TC.Slave.Beacon);
            probe.BeaconsUpdated += (beacons) => locations = beacons.ToList();
            probe.Start();
            Thread.Sleep(500);
            probe.Stop();
            Assert.AreEqual(expected: 1, actual: locations.Count);
            var beacon = locations[0];
            Assert.AreEqual(expected: TC.Slave.Beacon.Data, actual: beacon.Data);
        }

        [TestMethod]
        public void GetTcpPorts()
        {
            var device = TC.Device;
            Log.Debug("TESTING GET TCP PORTS");
            device.Close();
            Thread.Sleep(2500);
            var devices = device.GetAvailableDevices();
            device.Open();
            Assert.AreEqual(expected: 1, actual: devices.Count);
        }
    }
}
