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
            request = new Packet(functionCode, 0);
            ResponseLength = 0;
            RequestLength = 0;
        }

        public Function(byte code)
        {
            functionCode = code;
            ResponseLength = 0;
            RequestLength = 0;
            request = new Packet(code, 0);
        }

        public Function(byte code, byte length)
        {
            functionCode = code;
            RequestLength = length;
            ResponseLength = 0;
            request = new Packet(code, length);
            response = new Packet(code, 0);
        }

        public Function(byte code, byte requestLength, byte responseLength)
        {
            functionCode = code;
            RequestLength = requestLength;
            ResponseLength = responseLength;
            request = new Packet(code, requestLength);
            response = new Packet(code, responseLength);
        }

        internal byte[] GetRequest()
        {
            return GetRequestPacket().ToArray();
        }

        internal byte[] GetResponse()
        {
            return GetResponsePacket().ToArray();
        }

        internal virtual Packet GetRequestPacket()
        {
            return request;
        }

        internal virtual Packet GetResponsePacket()
        {
            return response;
        }

        internal void SetResponse(Packet packet)
        {
            response = packet;

            if (response.Code != GetRequestPacket().Code)
                throw new InvalidSlaveResponseException("Invalid function code");

            if (!IsResponseValid())
                throw new InvalidSlaveResponseException("Response content invalid");
        }

        internal Function SetRequest(Packet packet)
        {
            request = packet;

            if (request.Code != functionCode)
                throw new InvalidMasterRequestException("Invalid function code");

            if (!IsRequestValid())
                throw new InvalidMasterRequestException("Request content invalid");

            return this;
        }

        protected virtual bool IsResponseValid()
        {
            return response.Length == ResponseLength;
        }

        protected virtual bool IsRequestValid()
        {
            return request.Length == RequestLength;
        }

        public virtual void OnSend()
        {

        }

        public abstract bool Dispatch(dynamic listener);

        [XmlIgnore]
        [Category("Statistics")]
        [Description("The time it took to transmit the function and get a response from the slave")]
        public long TransmissionTime { get; internal set; } = 0;

        protected Packet request = null;
        protected Packet response = null;
        private readonly byte functionCode = 0x00;
        protected readonly byte ResponseLength = 0;
        protected readonly byte RequestLength = 0;

    }
}
