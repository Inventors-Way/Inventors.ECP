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
            var slave = new DefaultTcpSlave()
            {
                Address = IPAddress.Loopback.ToString(),
                Port = 10000
            };
            var layer = new TcpClientLayer()
            {
                Address = slave.Address,
                Port = slave.Port
            };
            var device = new DefaultDevice.DefaultDevice(layer);
            var timer = new Timer((info) => device.Dispatch(), device, 100, 100);
            var ping = new Ping();
            slave.Start();
            device.Open();

            //Thread.Sleep(100);

            device.Execute(ping);
            Assert.IsTrue(ping.Count == 0);            
            device.Execute(ping);
            Assert.IsTrue(ping.Count == 1);
            device.Execute(ping);
            Assert.IsTrue(ping.Count == 2);

            device.Close();
            slave.Stop();
        }

        [TestMethod]
        public void DeviceIdentification()
        {
            var slave = new DefaultTcpSlave()
            {
                Address = IPAddress.Loopback.ToString(),
                Port = 10000
            };
            var layer = new TcpClientLayer()
            {
                Address = slave.Address,
                Port = slave.Port
            };
            var device = new DefaultDevice.DefaultDevice(layer);
            var timer = new Timer((info) => device.Dispatch(), device, 100, 100);
            var info = new DeviceIdentification();
            slave.Start();
            device.Open();

            //Thread.Sleep(100);

            device.Execute(info);
            Assert.AreEqual(slave.Device, info.Device);
            Assert.AreEqual(slave.DeviceID, info.DeviceID);

            device.Close();
            slave.Stop();
        }

    }
}
