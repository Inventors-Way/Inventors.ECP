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
        public override byte Code => 0x11;

        public DataFunction() :
            base(requestLength: 0, responseLength: 0)
        {
        }

        public DataFunction(List<int> data) :
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

        public override FunctionDispatcher CreateDispatcher() => new FunctionDispatcher(Code, () => new DataFunction());

        public override int Dispatch(dynamic listener) => listener.Accept(this);

        [Category("Data")]
        [XmlElement("data")]
        public List<int> Data { get; } = new List<int>();

        public override void OnSend()
        {
            if (Data is null)
                throw new InvalidOperationException("No data to send");

            Request = new Packet(Code, Data.Count*sizeof(int));

            for (int n = 0; n < Data.Count; ++n)
            {
                Request.InsertInt32(n * sizeof(int), Data[n]);
            }
        }

        public override void OnSlaveReceived()
        {
            if (Request.Length > 0)
            {
                Data.Clear();

                for (int n = 0; n < Request.Length / 4; ++n)
                {
                    Data.Add(Request.GetInt32(n*sizeof(int)));
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
