﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            TC.Slave.Pings = 0;
            Assert.AreEqual(expected: 0, actual: TC.Device.Ping());
            Assert.AreEqual(expected: 1, actual: TC.Device.Ping());
            Assert.AreEqual(expected: 2, actual: TC.Device.Ping());
            Assert.AreEqual(expected: 3, actual: TC.Device.Ping());
        }

        [TestMethod]
        public void DeviceIdentification()
        {
            var info = new DeviceIdentification();
            TC.Device.Execute(info);
            Assert.AreEqual<UInt32>(expected: TC.Slave.Beacon.Serial, actual: info.SerialNumber);
            Assert.AreEqual<ushort>(expected: TC.Slave.Beacon.DeviceID, actual: info.DeviceID);
        }

        [TestMethod]
        public void GetEndianity()
        {
            var endianity = new GetEndianness();
            TC.Device.Execute(endianity);
            Assert.AreEqual(expected: true, actual: endianity.EqualEndianness);
        }

        [TestMethod]
        public void GetTcpPorts()
        {
            var device = TC.Device;
            Thread.Sleep(2500);
            var devices = device.GetLocationsDevices();
            Assert.AreEqual(expected: 1, actual: devices.Count);
        }
    }
}
