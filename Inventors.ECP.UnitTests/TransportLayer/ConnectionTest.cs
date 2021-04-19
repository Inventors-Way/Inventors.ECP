using Inventors.ECP.DefaultDevice;
using Inventors.ECP.Functions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.ECP.UnitTests.TransportLayer
{
    [TestClass]
    public class ConnectionTest
    {
        [TestMethod]
        public void DeviceIdentification()
        {
            var device = TC.CentralDevice;
            var id = new DeviceIdentification();

            device.Execute(id);
            Console.WriteLine($"Device: {id.Device}");
        }

        [TestMethod]
        public void Ping()
        {
            var device = TC.CentralDevice;
            var id = new Ping();

            device.Execute(id);
            Console.WriteLine($"Ping: {id.Count}");
        }

        [TestMethod]
        public void Endianness()
        {
            var device = TC.CentralDevice;
            var id = new GetEndianness();

            device.Execute(id);
            Console.WriteLine($"Equal endinness: {id.EqualEndianness}");
        }

        [TestMethod]
        public void SetDebugSignal()
        {
            var device = TC.CentralDevice;
            var id = new SetDebugSignal();
            id.Signals.Add(new DebugSignal() { Code = 0 });
            id.Signals.Add(new DebugSignal() { Code = 1 });

            device.Execute(id);
        }
    }
}
