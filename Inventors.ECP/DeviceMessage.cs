﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventors.ECP;
using Inventors.ECP.Messages;

namespace Inventors.ECP
{
    public abstract class DeviceMessage
    {
        public abstract byte Code { get; }

        public DeviceMessage(Packet response)
        {
            Packet = response;
        }

        public DeviceMessage(byte code)
        {
            Packet = new Packet(code, 0);
        }

        public DeviceMessage(byte code, byte length)
        {
            Packet = new Packet(code, length);
        }

        internal byte[] GetPacket()
        {
            return Packet.ToArray();
        }

        public virtual void OnSend()
        {

        }

        public virtual void OnReceived()
        {

        }

        protected Packet Packet { get; set; }

        public abstract MessageDispatcher CreateDispatcher();

        public abstract void Dispatch(dynamic listener);
    }
}