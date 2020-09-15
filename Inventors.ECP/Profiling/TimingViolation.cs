using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.ECP.Profiling
{
    public class TimingViolation
    {
        public TimingViolation(string id, double elapsedTime, double timeLimit, UInt32 context)
        {
            ID = id;
            Time = ProfileTiming.Mark();
            ElapsedTime = elapsedTime;
            TimeLimit = timeLimit;
            Context = context;
        }

        public string ID { get; }

        public double Time { get; }

        public UInt32 Context { get; }

        public double ElapsedTime { get; }

        public double TimeLimit { get; }
    }
}
