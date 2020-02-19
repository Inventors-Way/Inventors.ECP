using Inventors.ECP.Communication;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Inventors.ECP
{
    public abstract class DeviceFunction
    {
        public DeviceFunction()
        {
            Request = new Packet(functionCode, 0);
            ResponseLength = 0;
            RequestLength = 0;
        }

        public DeviceFunction(byte code)
        {
            functionCode = code;
            ResponseLength = 0;
            RequestLength = 0;
            Request = new Packet(code, 0);
        }

        public DeviceFunction(byte code, byte length)
        {
            functionCode = code;
            RequestLength = length;
            ResponseLength = 0;
            Request = new Packet(code, length);
            Response = new Packet(code, 0);
        }

        public DeviceFunction(byte code, byte requestLength, byte responseLength)
        {
            functionCode = code;
            RequestLength = requestLength;
            ResponseLength = responseLength;
            Request = new Packet(code, requestLength);
            Response = new Packet(code, responseLength);
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
            return Request;
        }

        internal virtual Packet GetResponsePacket()
        {
            return Response;
        }

        internal void SetResponse(Packet packet)
        {
            Response = packet;

            if (Response.Code != GetRequestPacket().Code)
                throw new InvalidSlaveResponseException(Resources.INVALID_FUNCTION_CODE);

            if (!IsResponseValid())
                throw new InvalidSlaveResponseException(Resources.INVALID_RESPONSE_CONTENT);
        }

        internal DeviceFunction SetRequest(Packet packet)
        {
            Request = packet;

            if (Request.Code != functionCode)
                throw new InvalidMasterRequestException(Resources.INVALID_FUNCTION_CODE);

            if (!IsRequestValid())
                throw new InvalidMasterRequestException(Resources.INVALID_RESPONSE_CONTENT);

            return this;
        }

        protected virtual bool IsResponseValid()
        {
            return Response.Length == ResponseLength;
        }

        protected virtual bool IsRequestValid()
        {
            return Request.Length == RequestLength;
        }

        public virtual void OnSend()
        {

        }

        public virtual void OnReceived()
        {

        }

        public virtual void OnSlaveReceived()
        {

        }

        public virtual void OnSlaveSend()
        {

        }

        public abstract FunctionDispatcher CreateDispatcher();

        public abstract bool Dispatch(dynamic listener);

        [XmlIgnore]
        [Category("Statistics")]
        [Description("The time it took to transmit the function and get a response from the slave")]
        public long TransmissionTime { get; internal set; } = 0;

        protected Packet Request { get; set; } = null;

        protected Packet Response { get; set; } = null;

        private readonly byte functionCode = 0x00;

        protected byte ResponseLength { get; } = 0;

        protected byte RequestLength { get; } = 0;

    }
}
