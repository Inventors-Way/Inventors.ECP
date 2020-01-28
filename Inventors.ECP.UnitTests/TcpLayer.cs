using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using Inventors.ECP;
using Inventors.ECP.DefaultDevice;
using Inventors.ECP.Functions;
using Inventors.ECP.Messages;
using System.Net;
using System.Threading;

namespace Inventors.ECP.UnitTests
{
    [TestClass]
    public class TcpLayer
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
            Assert.AreEqual(TcpTestContext.Slave.Device, info.Device);
            Assert.AreEqual(TcpTestContext.Slave.DeviceID, info.DeviceID);
        }

    }
}
