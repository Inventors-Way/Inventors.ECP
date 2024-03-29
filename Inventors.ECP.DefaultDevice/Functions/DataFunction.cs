﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Inventors.ECP.DefaultDevice.Functions
{
    public class DataFunction :
        DeviceFunction
    {
        public override byte Code => 0x11;

        public DataFunction() :
            base(requestLength: 0, responseLength: 0)
        {
        }

        public DataFunction(List<byte> data) :
            base(requestLength: data.Count * sizeof(int), responseLength: 0)
        {
            Data.Clear();
            Data.AddRange(data);
        }

        protected override bool IsRequestValid()
        {
            if (Request.Length > 0)
                return true;

            if ((Request.Length % sizeof(int)) == 0)
                return true;

            return false;
        }

        protected override bool IsResponseValid()
        {
            if (Request.Length == Response.Length)
                return true;

            return false;
        }

        public override FunctionDispatcher CreateDispatcher() => new FunctionDispatcher(Code, () => new DataFunction());

        public override int Dispatch(dynamic listener) => listener.Accept(this);

        [Category("Data")]
        [XmlElement("data")]
        public List<byte> Data { get; } = new List<byte>();

        public override void OnSend()
        {
            if (Data is null)
                throw new InvalidOperationException("No data to send");

            Request = new Packet(Code, Data.Count);

            for (int n = 0; n < Data.Count; ++n)
            {
                Request.InsertByte(n, Data[n]);
            }
        }

        public override void OnSlaveReceived()
        {
            if (Request.Length > 0)
            {
                Data.Clear();

                for (int n = 0; n < Request.Length; ++n)
                {
                    Data.Add(Request.GetByte(n));
                }
            }
            else
            {
                Data.Clear();
            }
        }

        public override void OnReceived()
        {
            if (Response.Length > 0)
            {
                Data.Clear();

                for (int n = 0; n < Response.Length; ++n)
                {
                    Data.Add(Response.GetByte(n));
                }
            }
            else
            {
                Data.Clear();
            }
        }

        public override string ToString() => "[0x11] Data Function";
    }
}
