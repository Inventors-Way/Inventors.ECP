using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.ECP.DefaultDevice.Messages
{
    public class DataMessage :
        DeviceMessage
    {
        public static readonly byte CODE = 0x81;

        public override byte Code => CODE;

        public DataMessage() : base(CODE, length: 0)
        {
        }
        
        public DataMessage(Packet response) :
            base(response)
        {
            if (Packet.Length == 0)
                throw new InvalidMessageException($"A received DataMessage does not contain any data");

            if (Packet.Length % sizeof(int) != 0)
                throw new InvalidMessageException("A received DataMessages does not contain a whole number of ints");
        }

        public List<int> Data { get; } = new List<int>();

        public override void OnSend()
        {
            Packet = new Packet(CODE, sizeof(int) * Data.Count);

            for (int n = 0; n < Data.Count; ++n)
            {
                Packet.InsertInt32(n * sizeof(int), Data[n]);
            }
        }

        public override void OnReceived()
        {
            int noOfPoints = Packet.Length / sizeof(int);
            Data.Clear();

            for (int n = 0; n < noOfPoints; ++n)
            {
                Data.Add(Packet.GetInt32(n * sizeof(int)));
            }
        }

        public override MessageDispatcher CreateDispatcher() => new MessageDispatcher(CODE, (p) => new DataMessage(p));

        public override void Dispatch(dynamic listener) => listener.Accept(this);
    }
}
