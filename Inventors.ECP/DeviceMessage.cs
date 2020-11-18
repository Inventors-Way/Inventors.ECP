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

        public DeviceMessage(byte code)
        {
            Packet = new Packet(code, 0);
        }

        public DeviceMessage(byte code, int length)
        {
            Packet = new Packet(code, length);
        }

        internal byte[] GetPacket()
        {
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

        protected Packet Packet { get; set; }

        public abstract MessageDispatcher CreateDispatcher();

        public abstract void Dispatch(dynamic listener);
    }
}
