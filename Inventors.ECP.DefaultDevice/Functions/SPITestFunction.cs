using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.ECP.DefaultDevice.Functions
{
    public class SPITestFunction :
        DeviceFunction
    {
        public override byte Code => 0x21;

        public SPITestFunction() :
            base(requestLength: 0, responseLength: 0)
        {
        }

        public SPITestFunction(List<byte> data) :
            base(requestLength: data.Count, responseLength: data.Count)
        {
            TxData.Clear();
            TxData.AddRange(data);
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

        public override FunctionDispatcher CreateDispatcher() => new FunctionDispatcher(Code, () => new SPITestFunction());

        public override int Dispatch(dynamic listener) => listener.Accept(this);

        [Category("Data")]
        public List<byte> TxData { get; } = new List<byte>();

        [Category("Data")]
        public List<byte> RxData { get; } = new List<byte>();

        public override void OnSend()
        {
            if (TxData is null)
                throw new InvalidOperationException("No data to send");

            Request = new Packet(Code, TxData.Count);

            for (int n = 0; n < TxData.Count; ++n)
            {
                Request.InsertByte(n, TxData[n]);
            }
        }

        public override void OnSlaveReceived()
        {
            if (Request.Length > 0)
            {
                TxData.Clear();

                for (int n = 0; n < Request.Length; ++n)
                {
                    TxData.Add(Request.GetByte(n));
                }
            }
            else
            {
                TxData.Clear();
            }
        }

        public override void OnReceived()
        {
            if (Response.Length > 0)
            {
                RxData.Clear();

                for (int n = 0; n < Response.Length; ++n)
                {
                    RxData.Add(Response.GetByte(n));
                }
            }
            else
            {
                RxData.Clear();
            }
        }

        public override string ToString() => "[0x21] SPI Test Function";
    }
}
