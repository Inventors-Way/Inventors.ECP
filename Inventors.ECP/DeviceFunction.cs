﻿using Inventors.ECP.Communication;
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

        public DeviceFunction(byte code, int requestLength)
        {
            functionCode = code;
            RequestLength = requestLength;
            ResponseLength = 0;
            Request = new Packet(code, requestLength);
            Response = new Packet(code, 0);
        }

        public DeviceFunction(byte code, int requestLength, int responseLength)
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

        /// <summary>
        /// Default response verification. Override this function if more than length verification is required.
        /// </summary>
        /// <returns>True if valid, otherwise false.</returns>
        protected virtual bool IsResponseValid()
        {
            return Response.Length == ResponseLength;
        }

        /// <summary>
        /// Default request verification. Override this function in a slave if more than length verification is required.
        /// </summary>
        /// <returns>True if valid, otherwise false.</returns>
        protected virtual bool IsRequestValid()
        {
            return Request.Length == RequestLength;
        }

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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract string ReportResponse();

        [XmlIgnore]
        [Category("Statistics")]
        [Description("The time it took to transmit the function and get a response from the slave")]
        public long TransmissionTime { get; internal set; } = 0;

        protected Packet Request { get; set; } = null;

        protected Packet Response { get; set; } = null;

        private readonly byte functionCode = 0x00;

        protected int ResponseLength { get; } = 0;

        protected int RequestLength { get; } = 0;

    }
}
