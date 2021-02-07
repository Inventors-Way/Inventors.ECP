﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using Inventors.ECP;

namespace Inventors.ECP.UnitTests.TransportLayer
{
    [TestClass]
    public class PacketTest
    {
        [TestMethod]
        public void TC01_StandardPacket()
        {
            var destuffer = new Destuffer();
            destuffer.OnReceive += Destruffer_OnReceive;
            Packet packet = new Packet(0x10, 64);
            Assert.AreEqual(expected: 64, actual: packet.Length);

            packet.InsertByte(0, 20);
            packet.InsertInt16(1, -100);
            packet.InsertInt32(3, -100);
            packet.InsertUInt32(7, 200);
            Send(destuffer, Frame.Encode(packet.ToArray()));

            Assert.IsTrue(_received is object);
            Assert.AreEqual<Byte>(expected: 0x10, actual: _received.Code);
            Assert.AreEqual<int>(expected: 64, actual: _received.Length);
            Assert.AreEqual<Byte>(expected: 20, actual: _received.GetByte(0));
            Assert.AreEqual<Int16>(expected: -100, actual: _received.GetInt16(1));
            Assert.AreEqual<Int32>(expected: -100, actual: _received.GetInt32(3));
            Assert.AreEqual<UInt32>(expected: 200, actual: _received.GetUInt32(7));
        }

        [TestMethod]
        public void TC02_Adressing()
        {
            var destuffer = new Destuffer();
            destuffer.OnReceive += Destruffer_OnReceive;
            Packet packet = new Packet(0x10, 4, 0x01);
            Assert.AreEqual(expected: 4, actual: packet.Length);
            packet.InsertUInt32(0, 1);

            Send(destuffer, Frame.Encode(packet.ToArray()));

            Assert.IsTrue(_received is object);
            Assert.AreEqual<Byte>(expected: 0x10, actual: _received.Code);
            Assert.AreEqual<int>(expected: 4, actual: _received.Length);
            Assert.AreEqual<Byte>(expected: 0x01, actual: _received.Address);
            Assert.AreEqual<UInt32>(expected: 1, actual: _received.GetUInt32(0));
        }

        [TestMethod]
        public void TC03_CRC8CCITChecksum()
        {
            var destuffer = new Destuffer();
            destuffer.OnReceive += Destruffer_OnReceive;
            Packet packet = new Packet(0x10, 4, ChecksumAlgorithmType.CRC8CCIT);
            Assert.AreEqual(expected: 4, actual: packet.Length);
            packet.InsertUInt32(0, 1);

            Send(destuffer, Frame.Encode(packet.ToArray()));

            Assert.IsTrue(_received is object);
            Assert.AreEqual<Byte>(expected: 0x10, actual: _received.Code);
            Assert.AreEqual<int>(expected: 4, actual: _received.Length);
            Assert.AreEqual<Byte>(expected: packet.Checksum, actual: _received.Checksum);
            Assert.AreEqual<UInt32>(expected: 1, actual: _received.GetUInt32(0));
        }

        /*
        [TestMethod]
        public void Length16bit()
        {
            var destuffer = new Destuffer();
            destuffer.OnReceive += Destruffer_OnReceive;
            int numberOfEntries = 1000;
            Packet packet = new Packet(0x10, numberOfEntries * 4);
            Assert.AreEqual(expected: LengthEncodingType.UInt16Encoding, actual: packet.LengthEncoding);

            for (int n = 0; n < numberOfEntries; ++n)
            {
                packet.InsertInt32(n * 4, n);
            }

            Send(destuffer, Frame.Encode(packet.ToArray()));
            Assert.AreEqual<Byte>(expected: 0x10, actual: _received.Code);
            Assert.AreEqual<int>(expected: numberOfEntries * 4, actual: _received.Length);

            for (int n = 0; n < numberOfEntries; ++n)
            {
                Assert.AreEqual<Int32>(expected: n, actual: _received.GetInt32(n * 4));
            }
        }

        [TestMethod]
        public void Length32bit()
        {
            var destuffer = new Destuffer();
            destuffer.OnReceive += Destruffer_OnReceive;
            int length = 2 * UInt16.MaxValue;
            Packet packet = new Packet(0x10, length);
            Assert.AreEqual(expected: LengthEncodingType.UInt32Encoding, actual: packet.LengthEncoding);

            for (int n = 0; n < length; ++n)
            {
                packet.InsertByte(n, (byte) n);
            }

            Send(destuffer, Frame.Encode(packet.ToArray()));
            Assert.AreEqual<Byte>(expected: 0x10, actual: _received.Code);
            Assert.AreEqual<int>(expected: length, actual: _received.Length);

            for (int n = 0; n < length; ++n)
            {
                Assert.AreEqual<byte>(expected: (byte) n, actual: _received.GetByte(n));
            }
        }

        */
        private void Send(Destuffer destuffer, byte[] frame)
        {
            _received = null;
            destuffer.Add(frame.Length, frame);
        }

        private void Destruffer_OnReceive(Destuffer sender, byte[] frame)
        {
            _received = new Packet(frame);
        }

        private Packet _received;
    }
}
