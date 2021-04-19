using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.ECP.Profiling
{
    public class ProfileReport
    {
        public List<TargetEvent> Events { get; internal set; }

        public List<SignalTiming> Timing { get; } = new List<SignalTiming>();

        public List<TimingViolation> Violation { get; internal set; }
    }
}
