using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.ECP.Profiling
{
    public interface IProfiler
    {
        bool Enabled { get; set; }

        void Add(TargetEvent e);

        void Add(TimingRecord record);

        void Add(TimingViolation violation);

        void Add(Packet packet);

        void Add(CommRecord record);
    }
}
