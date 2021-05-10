using Inventors.ECP.DefaultDevice;
using Inventors.ECP.DefaultDevice.Functions;
using Inventors.ECP.DefaultDevice.Messages;
using Inventors.ECP.Functions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Inventors.ECP.UnitTests.TransportLayer
{
    [TestClass]
    public class ConnectionTest
    {
        [TestMethod]
        public void T01_DeviceIdentification()
        {
            var device = TC.CentralDevice;
            var id = new DeviceIdentification();

            device.Execute(id);
            Console.WriteLine($"Device: {id.Device}");
        }

        [TestMethod]
        public void T02_Ping()
        {
            var device = TC.CentralDevice;
            var id = new Ping();

            device.Execute(id);
            Console.WriteLine($"Ping: {id.Count}");
        }

        [TestMethod]
        public void T03_Endianness()
        {
            var device = TC.CentralDevice;
            var id = new GetEndianness();

            device.Execute(id);
            Console.WriteLine($"Equal endinness: {id.EqualEndianness}");
        }

        [TestMethod]
        public void T04_SetDebugSignal()
        {
            var device = TC.CentralDevice;
            var id = new SetDebugSignal();
            id.Signals.Add(new DebugSignal() { Code = 0 });
            id.Signals.Add(new DebugSignal() { Code = 1 });

            device.Execute(id);
        }

        [TestMethod]
        public void T05_SimpleFunction()
        {
            var device = TC.CentralDevice;
            var func = new SimpleFunction
            {
                Operand = 1
            };
            device.Execute(func);

            Assert.AreEqual(2, func.Answer);

        }

        [TestMethod]
        public void T06_DataFunction()
        {
            var device = TC.CentralDevice;
            var data = new List<int>() { 1, 2, 3, 4 };
            var func = new DataFunction(data);

            device.Execute(func);

            CollectionAssert.AreEqual(data, TC.PeripheralDevice.FuncData);
        }

        [TestMethod]
        public void T07_SimpleMessage2Peripheral()
        {
            var device = TC.CentralDevice;
            var msg = new SimpleMessage() { X = 12 };
            device.Send(msg);
            Thread.Sleep(200);
            Assert.AreEqual(12, TC.PeripheralDevice.X);
        }
    }
}
