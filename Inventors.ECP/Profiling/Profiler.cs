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
        NotifyPropertyChanged,
        IProfiler
    {
        private readonly List<TargetEvent> events = new List<TargetEvent>();
        private readonly Dictionary<string, List<TimingRecord>> timing = new Dictionary<string, List<TimingRecord>>();
        private readonly Dictionary<string, List<TimingViolation>> violations = new Dictionary<string, List<TimingViolation>>();
        private readonly Dictionary<byte, long> packets = new Dictionary<byte, long>();
        private readonly List<CommRecord> commRecords = new List<CommRecord>();

        #region Properties
        #region Enabled Property
        private bool _enabled;

        public bool Enabled 
        {
            get => GetPropertyLocked(ref _enabled);
            set => SetPropertyLocked(ref _enabled, value);
        }

        #endregion
        #region ActiveProfile Property
        private string _activeProfile;

        public string ActiveProfile
        {
            get => GetPropertyLocked(ref _activeProfile);
            set => SetPropertyLocked(ref _activeProfile, value);
        }

        #endregion
        #region TimeSpan Property
        private double _timeSpan = Double.NaN;

        public double TimeSpan
        {
            get => GetPropertyLocked(ref _timeSpan);
            set => SetPropertyLocked(ref _timeSpan, value);
        }

        #endregion
        #endregion

        #region Adding Profiling

        public void Add(TargetEvent e)
        {
            if (e is null)
                throw new ArgumentNullException(nameof(e));

            if (Enabled)
            {
                lock (LockObject)
                {
                    events.Add(e);
                }
            }
        }

        public void Add(TimingRecord record)
        {
            if (record is null)
                throw new ArgumentNullException(nameof(record));

            if (Enabled)
            {
                lock (LockObject)
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

                if (string.IsNullOrEmpty(ActiveProfile))
                {
                    ActiveProfile = record.ID;
                }
            }
        }

        public void Add(TimingViolation violation)
        {
            if (violation is null)
                throw new ArgumentNullException(nameof(violation));

            if (Enabled)
            {
                lock (LockObject)
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

                if (string.IsNullOrEmpty(ActiveProfile))
                {
                    ActiveProfile = violation.ID;
                }
            }
        }

        public void Add(Packet packet)
        {
            if (packet is null)
                throw new ArgumentNullException(nameof(packet));

            if (Enabled)
            {
                lock (LockObject)
                {
                    if (packets.ContainsKey(packet.Code))
                    {
                        ++packets[packet.Code];
                    }
                    else
                    {
                        packets.Add(packet.Code, 1);
                    }
                }
            }
        }

        public void Add(double elapsedTime, CommRecord record)
        {
            if (record is null)
                throw new ArgumentNullException(nameof(record));

            if (Enabled)
            {
                lock (LockObject)
                {
                    commRecords.Add(record);
                }
            }
        }

        #endregion
    }
}
