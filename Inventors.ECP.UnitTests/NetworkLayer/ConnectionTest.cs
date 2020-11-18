using Inventors.ECP.DefaultDevice;
using Inventors.ECP.Functions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.ECP.UnitTests.NetworkLayer
{
    [TestClass]
    public class ConnectionTest
    {
        [TestMethod]
        public void Baudrate38400()
        {
            using (var device = new DefaultSerialDevice() { Location = "COM14", BaudRate = 38400 })
            {
                for (int n = 0; n < 10; ++n)
                {
                    var id = new DeviceIdentification();

                    device.Open();
                    device.Execute(id);
                    Console.WriteLine($"Device: {id.Device}");
                    device.Close();
                    Console.WriteLine($"Iteration: {n+1}");
                }
            }
        }

    }
}
