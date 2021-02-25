﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.ECP.Functions
{
    public class SetDebugSignal :
        DeviceFunction
    {
        public const byte FUNCTION_CODE = 0x04;

        public SetDebugSignal() : 
            base(FUNCTION_CODE, requestLength: 0, responseLength: 0) 
        { 
        }

        protected override bool IsRequestValid() => (Request.Length > 0) && ((Request.Length % sizeof(UInt32)) == 0);

        public override FunctionDispatcher CreateDispatcher() => new FunctionDispatcher(FUNCTION_CODE, () => new SetDebugSignal());

        public override int Dispatch(dynamic listener) => listener.Accept(this);

        public List<UInt32> Signals { get; } = new List<UInt32>();

        public override void OnSend()
        {
            Request = new Packet(FUNCTION_CODE, Signals.Count * sizeof(UInt32));

            for (int n = 0; n < Signals.Count; ++n)
            {
                Request.InsertUInt32(n * sizeof(UInt32), Signals[n]);
            }
        }

        public override void OnSlaveReceived()
        {
            Signals.Clear();

            for (int n = 0; n < Request.Length / sizeof(UInt32); ++n)
            {
                Signals.Add(Request.GetUInt32(n * sizeof(UInt32)));
            }
        }

        public override string ToString() => "[0x04] SetDebugSignal";

        public override string ReportResponse() => "Debug signals updated";
    }
}