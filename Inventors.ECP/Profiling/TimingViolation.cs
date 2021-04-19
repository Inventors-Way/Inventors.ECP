using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.ECP.Profiling
{
    public class TimingViolation : 
        Record
    {
        public TimingViolation(UInt32 signal, double elapsedTime, double timeLimit, UInt32 context)
        {
            Signal = signal;
            ElapsedTime = elapsedTime;
            TimeLimit = timeLimit;
            Context = context;
        }

        public UInt32 Signal { get; }

        public UInt32 Context { get; }

        public double ElapsedTime { get; }

        public double TimeLimit { get; }
    }
}
