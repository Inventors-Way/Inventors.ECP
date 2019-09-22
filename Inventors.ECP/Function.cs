using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Inventors.ECP
{
    public abstract class Function
    {
        public Function()
        {
            request = new Packet(0x00, 0);
        }

        public Function(byte code)
        {
            request = new Packet(code, 0);
        }

        public Function(byte code, byte length)
        {
            request = new Packet(code, length);
        }

        internal byte[] GetRequest()
        {
            return GetRequestPacket().ToArray();
        }

        internal virtual Packet GetRequestPacket()
        {
            return request;
        }

        internal void SetResponse(Packet packet)
        {
            response = packet;

            if (response.Code != GetRequestPacket().Code)
                throw new InvalidSlaveResponseException("Invalid function code");

            if (!IsResponseValid())
                throw new InvalidSlaveResponseException("Response content invalid");
        }

        protected virtual bool IsResponseValid()
        {
            return response.Length == 0;
        }

        public virtual void OnSend()
        {

        }

        [XmlIgnore]
        [Category("Statistics")]
        [Description("The time it took to transmit the function and get a response from the slave")]
        public long TransmissionTime { get; internal set; } = 0;

        protected Packet request;
        protected Packet response = null;
    }
}
