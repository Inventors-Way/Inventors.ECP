using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using Inventors.ECP;
using System.Text.RegularExpressions;

namespace Inventors.ECP.UnitTests.Utility
{
    [TestClass]
    public class LocationTest
    {
        [TestMethod]
        public void RegexSerialPortTest()
        {
            string pattern = "^COM[1-9][0-9]*$";
            Assert.IsTrue(Regex.Match("COM1", pattern).Success);
            Assert.IsTrue(Regex.Match("COM2", pattern).Success);
            Assert.IsTrue(Regex.Match("COM12", pattern).Success);
            Assert.IsFalse(Regex.Match("COM0", pattern).Success);
            Assert.IsFalse(Regex.Match("COM01", pattern).Success);
            Assert.IsFalse(Regex.Match(" COM1", pattern).Success);
            Assert.IsFalse(Regex.Match("192.172.0.1", pattern).Success);
        }

        [TestMethod]
        public void RegexDeviceTest()
        {
            string pattern = "^[1-9][0-9]*\\.[1-9][0-9]*$";
            Assert.IsTrue(Regex.IsMatch("1.1", pattern));
            Assert.IsTrue(Regex.IsMatch("1.10", pattern));
            Assert.IsTrue(Regex.IsMatch("20.1", pattern));
            Assert.IsFalse(Regex.IsMatch("01.1", pattern));
            Assert.IsFalse(Regex.IsMatch("1.01", pattern));
            Assert.IsFalse(Regex.IsMatch(" 1.1", pattern));
            Assert.IsFalse(Regex.IsMatch("1.1 ", pattern));
            Assert.IsFalse(Regex.IsMatch("foo", pattern));
        }

        [TestMethod]
        public void RegexNetworkAddressTest()
        {
            string pattern = "^\\d{1,3}\\.\\d{1,3}\\.\\d{1,3}\\.\\d{1,3}:\\d{1,5}$";
            Assert.IsTrue(Regex.IsMatch("192.172.0.1:1000", pattern));
            Assert.IsFalse(Regex.IsMatch("192.172.0.1", pattern));
            Assert.IsFalse(Regex.IsMatch("192.172.0.1:1000 ", pattern));
            Assert.IsFalse(Regex.IsMatch(" 192.172.0.1:1000", pattern));
        }

        [TestMethod]
        public void DefaultSerialLocation()
        {
            var location = Location.Parse("COM1");

            Assert.AreEqual(expected: CommunicationProtocol.SERIAL, actual: location.Protocol);
            Assert.AreEqual(expected: "COM1", actual: location.Address);
            Assert.AreEqual<ushort>(expected: 0, actual: location.Port);
            Assert.AreEqual(expected: Manufacturer.Invalid, actual: location.ManufacturerID);
            Assert.AreEqual<ushort>(expected: 0, actual: location.DeviceID);
            Assert.AreEqual<UInt32>(expected: 0, actual: location.SerialNumber);
            Assert.AreEqual(expected: "serial://COM1", actual: location.ToString());
        }

        [TestMethod]
        public void SerialLocationNoDevice()
        {
            var location = Location.Parse("serial://COM1");

            Assert.AreEqual(expected: CommunicationProtocol.SERIAL, actual: location.Protocol);
            Assert.AreEqual(expected: "COM1", actual: location.Address);
            Assert.AreEqual<ushort>(expected: 0, actual: location.Port);
            Assert.AreEqual(expected: Manufacturer.Invalid, actual: location.ManufacturerID);
            Assert.AreEqual<ushort>(expected: 0, actual: location.DeviceID);
            Assert.AreEqual<UInt32>(expected: 0, actual: location.SerialNumber);
            Assert.AreEqual(expected: "serial://COM1", actual: location.ToString());
        }

        [TestMethod]
        public void SerialLocationNoSerialNumber()
        {
            var location = Location.Parse("serial://COM1/1.1");

            Assert.AreEqual(expected: CommunicationProtocol.SERIAL, actual: location.Protocol);
            Assert.AreEqual(expected: "COM1", actual: location.Address);
            Assert.AreEqual<ushort>(expected: 0, actual: location.Port);
            Assert.AreEqual(expected: Manufacturer.InventorsWay, actual: location.ManufacturerID);
            Assert.AreEqual<ushort>(expected: 1, actual: location.DeviceID);
            Assert.AreEqual<UInt32>(expected: 0, actual: location.SerialNumber);
            Assert.AreEqual(expected: "serial://COM1/1.1", actual: location.ToString());
        }

        [TestMethod]
        public void SerialLocation()
        {
            var location = Location.Parse("serial://COM1/1.1/10");

            Assert.AreEqual(expected: CommunicationProtocol.SERIAL, actual: location.Protocol);
            Assert.AreEqual(expected: "COM1", actual: location.Address);
            Assert.AreEqual<ushort>(expected: 0, actual: location.Port);
            Assert.AreEqual(expected: Manufacturer.InventorsWay, actual: location.ManufacturerID);
            Assert.AreEqual<ushort>(expected: 1, actual: location.DeviceID);
            Assert.AreEqual<UInt32>(expected: 10, actual: location.SerialNumber);
            Assert.AreEqual(expected: "serial://COM1/1.1/10", actual: location.ToString());
        }

        [TestMethod]
        public void NetworkLocationNoDevice()
        {
            var location = Location.Parse("network://192.172.0.1:1001");

            Assert.AreEqual(expected: CommunicationProtocol.NETWORK, actual: location.Protocol);
            Assert.AreEqual(expected: "192.172.0.1", actual: location.Address);
            Assert.AreEqual<ushort>(expected: 1001, actual: location.Port);
            Assert.AreEqual(expected: Manufacturer.Invalid, actual: location.ManufacturerID);
            Assert.AreEqual<ushort>(expected: 0, actual: location.DeviceID);
            Assert.AreEqual<UInt32>(expected: 0, actual: location.SerialNumber);
            Assert.AreEqual(expected: "network://192.172.0.1:1001", actual: location.ToString());
        }

        [TestMethod]
        public void NetworkLocationNoSerialNumber()
        {
            var location = Location.Parse("network://192.172.0.1:1001/1.1");

            Assert.AreEqual(expected: CommunicationProtocol.NETWORK, actual: location.Protocol);
            Assert.AreEqual(expected: "192.172.0.1", actual: location.Address);
            Assert.AreEqual<ushort>(expected: 1001, actual: location.Port);
            Assert.AreEqual(expected: Manufacturer.InventorsWay, actual: location.ManufacturerID);
            Assert.AreEqual<ushort>(expected: 1, actual: location.DeviceID);
            Assert.AreEqual<UInt32>(expected: 0, actual: location.SerialNumber);
            Assert.AreEqual(expected: "network://192.172.0.1:1001/1.1", actual: location.ToString());
        }

        [TestMethod]
        public void NetworkLocation()
        {
            var location = Location.Parse("network://192.172.0.1:1001/1.1/10");

            Assert.AreEqual(expected: CommunicationProtocol.NETWORK, actual: location.Protocol);
            Assert.AreEqual(expected: "192.172.0.1", actual: location.Address);
            Assert.AreEqual<ushort>(expected: 1001, actual: location.Port);
            Assert.AreEqual(expected: Manufacturer.InventorsWay, actual: location.ManufacturerID);
            Assert.AreEqual<ushort>(expected: 1, actual: location.DeviceID);
            Assert.AreEqual<UInt32>(expected: 10, actual: location.SerialNumber);
            Assert.AreEqual(expected: "network://192.172.0.1:1001/1.1/10", actual: location.ToString());
        }

        [TestMethod]
        public void InvalidManufacturer()
        {
            var location = Location.Parse("serial://COM1/99.1/10");
        }
    }
}
