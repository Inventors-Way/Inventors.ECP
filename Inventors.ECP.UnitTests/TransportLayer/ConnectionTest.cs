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
        public void T07_LargeDataFunction()
        {
            var device = TC.CentralDevice;
            var data = new List<int>();

            for (int n = 0; n < 800; ++n)
                data.Add(n);

            var func = new DataFunction(data);

            device.Timeout = 500;
            device.Execute(func);

            CollectionAssert.AreEqual(data, TC.PeripheralDevice.FuncData);
        }

        [TestMethod]
        public void T08_SimpleMessage2Peripheral()
        {
            var device = TC.CentralDevice;
            var msg = new SimpleMessage() { X = 12 };
            device.Send(msg);
            Thread.Sleep(200);
            Assert.AreEqual(12, TC.PeripheralDevice.X);
        }

        [TestMethod]
        public void T09_SimpleMessage2Central()
        {
            var peripheral = TC.PeripheralDevice;
            var msg = new SimpleMessage() { X = 12 };
            peripheral.Send(msg);
            Thread.Sleep(200);
            Assert.AreEqual(12, TC.CentralDevice.X);
        }

        [TestMethod]
        public void T10_NACK()
        {
            var device = TC.CentralDevice;
            TC.PeripheralDevice.ErrorCode = (int)TestErrorCode.INVALID_OPERATION;
            var data = new List<int>() { 1, 2, 3, 4 };
            var func = new DataFunction(data);
            Assert.ThrowsException<FunctionNotAcknowledgedException>(() => device.Execute(func));

            try
            {
                device.Execute(func);
            }
            catch (FunctionNotAcknowledgedException e)
            {
                Console.WriteLine($"NACK Received: {device.GetErrorString(e.ErrorCode)}");
            }

            TC.PeripheralDevice.ErrorCode = 0;
            device.Execute(func);
        }
    }
}
