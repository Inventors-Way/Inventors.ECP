using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.ECP.Functions
{
    public class SetDebugSignal :
        DeviceFunction
    {
        public override byte Code => 0x04;

        public SetDebugSignal() : 
            base(requestLength: 0, responseLength: 0) 
        { 
        }

        protected override bool IsRequestValid() => (Request.Length > 0) && ((Request.Length % sizeof(UInt32)) == 0);

        public override FunctionDispatcher CreateDispatcher() => new FunctionDispatcher(Code, () => new SetDebugSignal());

        public override int Dispatch(dynamic listener) => listener.Accept(this);

        public List<DebugSignal> Signals { get; } = new List<DebugSignal>();

        public override void OnSend()
        {
            Request = new Packet(Code, Signals.Count * sizeof(UInt32));

            for (int n = 0; n < Signals.Count; ++n)
            {
                Request.InsertUInt32(n * sizeof(UInt32), Signals[n].Code);
            }
        }

        public override void OnSlaveReceived()
        {
            Signals.Clear();

            for (int n = 0; n < Request.Length / sizeof(UInt32); ++n)
            {
                Signals.Add(new DebugSignal()
                {
                    Code = Request.GetUInt32(n * sizeof(UInt32))
                });
            }
        }

        public override string ToString() => "[0x04] SetDebugSignal";
    }
}
