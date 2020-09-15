using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.ECP.Profiling
{
    public class TargetEvent
    {
        public TargetEvent(string description)
        {
            Description = description;
            Time = ProfileTiming.Mark();
        }

        public string Description { get; }

        public double Time { get; }
    }
}
