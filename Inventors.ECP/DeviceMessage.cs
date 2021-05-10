using System;
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

        public DeviceMessage()
        {
            Packet = new Packet(Code, 0);
        }

        public DeviceMessage(int length)
        {
            Packet = new Packet(Code, length);
        }

        internal byte[] GetPacket(DeviceAddress address)
        {
            if (address is object)
            {
                Packet.Address = address.Value;
            }

            return Packet.ToArray();
        }

        /// <summary>
        /// Override this function to create the message Packet from properties when the 
        /// message is send. This enables messages where their size cannot be known when
        /// they are constructed.
        /// </summary>
        public virtual void OnSend() { }

        /// <summary>
        /// Override this function provide initialization of properties when the message
        /// is received by the master or slave. This can be used if it is to expensive to
        /// create the properties of the message everytime they are accessed, instead they
        /// can be created once when the message is received.
        /// </summary>
        public virtual void OnReceived() { }

        public int Address => Packet.AddressEnabled ? Packet.Address : -1;

        protected Packet Packet { get; set; }

        public abstract MessageDispatcher CreateDispatcher();

        public abstract void Dispatch(dynamic listener);
    }
}
