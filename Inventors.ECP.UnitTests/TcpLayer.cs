using Microsoft.VisualStudio.TestTools.UnitTesting;
using Inventors.ECP;
using Inventors.ECP.Functions;
using Inventors.ECP.DefaultDevice;
using System.Net;

namespace Inventors.ECP.UnitTests
{
    [TestClass]
    public class TcpLayer
    {
        [TestMethod]
        public void GetAddress()
        {
            var address = TcpServerLayer.GetLocalAddress();
            Assert.IsFalse(string.IsNullOrEmpty(address));
        }

        [TestMethod]
        public void PingTest()
        {
            int port = 30000;
            string address = IPAddress.Loopback.ToString();

            var layer = new TcpClientLayer()
            {
                Port = port,
                Address = address
            };
            var device = new Inventors.ECP.DefaultDevice.DefaultDevice(layer);
            var slave = new DefaultTcpSlave()
            {
                Address = layer.Address,
                Port = layer.Port
            };
            slave.Start();
            device.Open();

            var ping = new Ping();
            device.Execute(ping);
            device.Close();
            slave.Stop();
        }
    }
}
