using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.ECP.Profiling
{
    public abstract class Record
    {
        public Record()
        {
            Time = ProfileTiming.Mark();
        }

        public double Time { get; }
    }
}
