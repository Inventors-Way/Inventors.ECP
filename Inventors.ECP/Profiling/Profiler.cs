using Inventors.ECP.Communication;
using Inventors.ECP.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Inventors.ECP.Profiling
{
    public class Profiler :
        IProfiler
    {
        private readonly object dataLock = new object();
        private readonly Dictionary<string, List<TimingRecord>> timing = new Dictionary<string, List<TimingRecord>>();
        private readonly Dictionary<string, List<TimingViolation>> violations = new Dictionary<string, List<TimingViolation>>();
        private readonly Dictionary<byte, long> packets = new Dictionary<byte, long>();

        #region Enabled Property
        private readonly LockedVariable<bool> _enabled = new LockedVariable<bool>(false);

        public bool Enabled 
        {
            get => _enabled.Get(dataLock);
            set => _enabled.Set(dataLock, value);
        }

        #endregion

        public void Add(TargetEvent e)
        {
            if (Enabled)
            {

            }
        }

        public void Add(TimingRecord record)
        {
            if (record is null)
                throw new ArgumentNullException(nameof(record));

            if (Enabled)
            {
                lock (dataLock)
                {
                    if (timing.ContainsKey(record.ID))
                    {
                        timing[record.ID].Add(record);
                    }
                    else
                    {
                        timing.Add(record.ID, new List<TimingRecord>());
                        timing[record.ID].Add(record);
                    }
                }
            }
        }

        public void Add(TimingViolation violation)
        {
            if (violation is null)
                throw new ArgumentNullException(nameof(violation));

            if (Enabled)
            {
                lock (dataLock)
                {
                    if (violations.ContainsKey(violation.ID))
                    {
                        violations[violation.ID].Add(violation);
                    }
                    else
                    {
                        violations.Add(violation.ID, new List<TimingViolation>());
                        violations[violation.ID].Add(violation);
                    }
                }
            }
        }

        public void Add(Packet packet)
        {
            if (Enabled)
            {

            }
        }

        public void Add(double elapsedTime, CommRecord record)
        {
            if (Enabled)
            {

            }
        }
    }
}
