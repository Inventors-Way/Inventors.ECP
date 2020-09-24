using Inventors.ECP.Communication;
using Inventors.ECP.Profiling.Analysis;
using Inventors.ECP.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Inventors.ECP.Profiling
{
    public class Profiler :
        NotifyPropertyChanged
    {
        private readonly List<TargetEvent> events = new List<TargetEvent>();
        private readonly Dictionary<string, List<TimingRecord>> timing = new Dictionary<string, List<TimingRecord>>();
        private readonly Dictionary<string, List<TimingViolation>> violations = new Dictionary<string, List<TimingViolation>>();
        private readonly Dictionary<byte, long> packets = new Dictionary<byte, long>();
        private readonly List<CommRecord> commRecords = new List<CommRecord>();
        private readonly Dictionary<string, double> timingMax = new Dictionary<string, double>();
        private double time = 0;

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
            set
            {
                if (ActiveProfile != value)
                {
                    SetPropertyLocked(ref _activeProfile, value);
                    UpdateTaskProfile();
                }
            }
        }

        #endregion
        #region AvailableProfiles
        private readonly List<string> profiles = new List<string>();

        public IList<string> AvailableProfiles
        {
            get
            {
                lock (LockObject)
                {
                    return profiles.AsReadOnly();
                }
            }
        }

        private void ClearProfiles()
        {
            lock (LockObject)
            {
                profiles.Clear();
            }

            Notify(nameof(AvailableProfiles));
        }

        private void AddProfile(string profile)
        {
            lock (LockObject)
            {
                profiles.Add(profile);
            }

            Notify(nameof(AvailableProfiles));
        }

        private bool ProfileExists(string profile)
        {
            lock (LockObject)
            {
                return profiles.Any((p) => p == profile);
            }
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
        #region Overview Property
        private OverviewAnalysis _overview;

        public OverviewAnalysis Overview
        {
            get => GetPropertyLocked(ref _overview);
            private set => SetPropertyLocked(ref _overview, value);
        }
        #endregion
        #region TaskProfile Property
        private TaskAnalysis _taskProfile;

        public TaskAnalysis TaskProfile
        {
            get => GetPropertyLocked(ref _taskProfile);
            private set => SetProperty(ref _taskProfile, value);
        }

        #endregion
        #endregion
        #region Overview Analysis

        private TimingRecord GetCurrentTiming(string id) =>
            timing[id][timing[id].Count - 1];

        private List<TimingRecord> Current
        {
            get
            {
                List<TimingRecord> retValue = new List<TimingRecord>();

                foreach (var pair in timing)
                {
                    retValue.Add(GetCurrentTiming(pair.Key));
                }

                return retValue;
            }
        }

        private void UpdateOverview()
        {
            OverviewAnalysis analysis = null;

            lock (LockObject)
            {
                var current = Current;
                var x = (from v in Enumerable.Range(0, timing.Count) select (double)v).ToList();
                var average = (from record in current select record.Average).ToList();
                var max = (from record in current select record.Max).ToList();
                var min = (from record in current select record.Min).ToList();
                var labels = (from record in current select record.ID).ToList();

                foreach (var r in current)
                {
                    if (timingMax.ContainsKey(r.ID))
                    {
                        if (timingMax[r.ID] < r.Max)
                        {
                            timingMax[r.ID] = r.Max;
                        }
                    }
                    else
                    {
                        timingMax.Add(r.ID, r.Max);
                    }
                }

                var globalMax = (from r in current select timingMax[r.ID]).ToList();

                analysis = new OverviewAnalysis(x: x, 
                                                average: average, 
                                                maximum: max, 
                                                minimum: min, 
                                                labels: labels, 
                                                scaleMaximum: globalMax);
            }

            Overview = analysis;
        }

        #endregion
        #region Task Analysis

        private void UpdateTaskProfile()
        {
            TaskAnalysis analysis = null;

            if (!string.IsNullOrEmpty(ActiveProfile) && timing.ContainsKey(ActiveProfile))
            {
                lock (LockObject)
                {
                    var records = from r in timing[ActiveProfile]
                                  where IsIncuded(r)
                                  select r;
                    var violationRecords = violations.ContainsKey(ActiveProfile) ?
                                           (from r in violations[ActiveProfile]
                                           where IsIncuded(r)
                                           select r).ToList() :
                                           null;
                    var selectedEvents = (from e in events
                                          where IsIncuded(e)
                                          select e).ToList();

                    var time = records.Select((r) => r.Time).ToList();
                    var average = records.Select((r) => r.Average).ToList();
                    var max = records.Select((r) => r.Max).ToList();
                    var min = records.Select((r) => r.Min).ToList();

                    analysis = new TaskAnalysis(ActiveProfile, average, max, min, time, violationRecords, selectedEvents);
                }
            }

            TaskProfile = analysis;
        }

        #endregion

        private bool IsIncuded(Record record)
        {
            bool retValue = true;

            if (!Double.IsNaN(TimeSpan))
            {
                retValue = record.Time > time - TimeSpan;
            }

            return retValue;
        }


        public void Reset()
        {
            lock (LockObject)
            {
                ProfileTiming.Reset();
                timingMax.Clear();
                events.Clear();
                timing.Clear();
                violations.Clear();
                packets.Clear();
                commRecords.Clear();
                
                // Reset analysis
                Overview = null;
                ActiveProfile = "";
                ClearProfiles();

                time = 0;
            }
        }

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
                    time = e.Time;
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

                    time = record.Time;
                }

                if (!ProfileExists(record.ID))
                {
                    AddProfile(record.ID);
                }

                if (string.IsNullOrEmpty(ActiveProfile))
                {
                    ActiveProfile = record.ID;
                }

                if (record.ID == ActiveProfile)
                {
                    UpdateTaskProfile();
                }

                UpdateOverview();
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

                    time = violation.Time;
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
                    time = record.Time;
                }
            }
        }

        #endregion
    }
}
