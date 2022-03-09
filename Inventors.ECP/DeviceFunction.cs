using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Inventors.ECP
{
    public abstract class DeviceFunction :
        DynamicObject
    {
        public abstract byte Code { get; }

        public DeviceFunction()
        {
            Request = new Packet(Code, 0);
            Response = new Packet(Code, 0);
            ResponseLength = 0;
            RequestLength = 0;
        }

        public DeviceFunction(int requestLength)
        {
            RequestLength = requestLength;
            ResponseLength = 0;
            Request = new Packet(Code, requestLength);
            Response = new Packet(Code, 0);
        }

        public DeviceFunction(int requestLength, int responseLength)
        {
            RequestLength = requestLength;
            ResponseLength = responseLength;
            Request = new Packet(Code, requestLength);
            Response = new Packet(Code, responseLength);
        }

        internal byte[] GetRequest(byte address)
        {
            Packet packet = GetRequestPacket();
            packet.Address = address;
            return packet.ToArray();
        }

        internal byte[] GetResponse() => GetResponsePacket().ToArray();

        internal virtual Packet GetRequestPacket() => Request;

        internal virtual Packet GetResponsePacket() => Response;

        internal void SetResponse(Packet packet)
        {
            Response = packet;

            if (Response.Code != GetRequestPacket().Code)
            {
                throw new InvalidSlaveResponseException($"Invalid function code ({Response.Code} != {GetRequestPacket().Code}");
            }

            if (!IsResponseValid())
            {
                throw new InvalidSlaveResponseException("Response if invalid (IsResponseValid() returned false)");
            }
        }

        internal DeviceFunction SetRequest(Packet packet)
        {
            Request = packet;

            if (Request.Code != Code)
                throw new InvalidMasterRequestException(Resources.INVALID_FUNCTION_CODE);

            if (!IsRequestValid())
                throw new InvalidMasterRequestException(Resources.INVALID_RESPONSE_CONTENT);

            return this;
        }

        /// <summary>
        /// Default response verification. Override this function if more than length verification is required.
        /// </summary>
        /// <returns>True if valid, otherwise false.</returns>
        protected virtual bool IsResponseValid() => Response.Length == ResponseLength;

        /// <summary>
        /// Default request verification. Override this function in a slave if more than length verification is required.
        /// </summary>
        /// <returns>True if valid, otherwise false.</returns>
        protected virtual bool IsRequestValid() => Request.Length == RequestLength;

        /// <summary>
        /// Override this function to build the request packet (Request), when the function is executed.
        /// </summary>
        public virtual void OnSend() { }

        /// <summary>
        /// Override this function to parse the response packet (Response), when the response is received from the slave.
        /// </summary>
        public virtual void OnReceived() { }

        /// <summary>
        /// Override this function in a slave to parse the request packet (Request), when the function is received from the master.
        /// </summary>
        public virtual void OnSlaveReceived() { }

        /// <summary>
        /// Override this function to build the response packet (Response) in a slave, when the response to master has to be send.
        /// </summary>
        public virtual void OnSlaveSend() { }

        public abstract FunctionDispatcher CreateDispatcher();

        public abstract int Dispatch(dynamic listener);

        [XmlIgnore]
        [Category("Statistics")]
        [Description("The time it took to transmit the function and get a response from the slave")]
        public long TransmissionTime { get; internal set; } = 0;

        [Browsable(false)]
        public int Address => Request.AddressEnabled ? Request.Address : -1;

        protected Packet Request { get; set; }

        protected Packet Response { get; set; }

        protected int ResponseLength { get; }

        protected int RequestLength { get; }
    }
}
