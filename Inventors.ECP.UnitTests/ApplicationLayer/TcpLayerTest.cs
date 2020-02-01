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
using BeaconLib;

namespace Inventors.ECP.UnitTests.ApplicationLayer
{
    [TestClass]
    public class TcpLayerTest
    {
        [TestMethod]
        public void Ping()
        {
            var ping = new Ping();

            TcpTestContext.Slave.Pings = 0;
            TcpTestContext.Device.Execute(ping);
            Assert.IsTrue(ping.Count == 0);
            TcpTestContext.Device.Execute(ping);
            Assert.IsTrue(ping.Count == 1);
            TcpTestContext.Device.Execute(ping);
            Assert.IsTrue(ping.Count == 2);
        }

        [TestMethod]
        public void DeviceIdentification()
        {
            var info = new DeviceIdentification();
            TcpTestContext.Device.Execute(info);
            Assert.AreEqual(expected: TcpTestContext.Slave.Identification.Device, actual: info.Device);
            Assert.AreEqual(expected: TcpTestContext.Slave.Identification.DeviceID, actual: info.DeviceID);
        }

        [TestMethod]
        public void GetEndianity()
        {
            var endianity = new GetEndianness();
            TcpTestContext.Device.Execute(endianity);
            Assert.AreEqual(expected: true, actual: endianity.EqualEndianness);
        }

        [TestMethod]
        public void BeaconTest()
        {
            List<BeaconLocation> locations = new List<BeaconLocation>();
            var probe = new Probe(TcpTestContext.Slave.Identification.BeaconName);
            probe.BeaconsUpdated += (beacons) => locations = beacons.ToList();
            probe.Start();
            Thread.Sleep(500);
            probe.Stop();
            Assert.AreEqual(expected: 1, actual: locations.Count);
            var beacon = locations[0];
            var data = DeviceData.Create(locations[0].Data);
            Assert.AreEqual(expected: TcpTestContext.Slave.Identification.DeviceID, actual: data.DeviceID);
        }
    }
}
