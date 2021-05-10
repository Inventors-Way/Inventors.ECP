using System;
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
        public const byte FUNCTION_CODE = 0x11;

        public DataFunction() :
            base(FUNCTION_CODE, requestLength: 0, responseLength: 0)
        {
        }

        public DataFunction(byte[] data) :
            base(FUNCTION_CODE, requestLength: data.Length, responseLength: 0)
        {
            ImageData = data;
        }

        protected override bool IsRequestValid() => Request.Length > 0;

        public override FunctionDispatcher CreateDispatcher() => new FunctionDispatcher(FUNCTION_CODE, () => new DataFunction());

        public override int Dispatch(dynamic listener) => listener.Accept(this);

        [Category("Data")]
        [XmlElement("data")]
        public byte[] ImageData { get; private set; } = new byte[0];

        public override void OnSend()
        {
            if (ImageData is null)
                throw new InvalidOperationException("No data to send");

            Request = new Packet(FUNCTION_CODE, ImageData.Length);

            for (int n = 0; n < ImageData.Length; ++n)
            {
                Request.InsertByte(n, ImageData[n]);
            }
        }

        public override void OnSlaveReceived()
        {
            if (Request.Length > 0)
            {
                ImageData = new byte[Request.Length];

                for (int n = 0; n < Request.Length; ++n)
                {
                    ImageData[n] = Request.GetByte(n);
                }
            }
            else
            {
                ImageData = new byte[0];
            }
        }

        public override string ToString() => "[0x11] Data Function";
    }
}
