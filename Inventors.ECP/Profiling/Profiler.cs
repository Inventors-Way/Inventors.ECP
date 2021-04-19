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
        private class DataNode<T>
            where T : Record
        {
            public DataNode(T value)
            {
                Value = value;
            }

            public DataNode<T> Next { get; set; }

            public DataNode<T> Previous { get; set; }

            public T Value { get; }
        }

        private interface IFilter
        {
            bool IsIncluded(Record value);
        }

        private class TimeFilter :
            IFilter
        {
            public double End { get; set; }

            public double Duration { get; set; }

            public bool IsIncluded(Record value)
            {
                return (value.Time >= (End - Duration)) && (value.Time <= End);
            }
        }

        private class DataSet<T>
            where T : Record
        {
            private DataNode<T> last;
            private DataNode<T> start;
            private DataNode<T> end;
            private readonly IFilter filter;

            public bool Updated { get; private set; }

            public DataSet(IFilter filter)
            {
                this.filter = filter;
            }

            public void Clear()
            {
                last = start = end = null;
                Updated = true;
            }

            public void Add(T value)
            {
                var current = new DataNode<T>(value);

                if (last is object)
                {
                    last.Next = current;
                    current.Previous = last;
                    last = current;

                    if (filter.IsIncluded(last.Value))
                    {
                        end = last;
                        Updated = true;
                    }

                    if (!filter.IsIncluded(start.Value))
                    {
                        if (start.Next is object)
                        {
                            start = start.Next;
                            Updated = true;
                        }
                    }
                }
                else
                {
                    last = start = end = current;
                }
            }

            public void Refilter()
            {
                if (last is null)
                    return;

                // First we find the end of the window
                end = last;

                while (!filter.IsIncluded(end.Value) && (end.Previous is object))
                {
                    end = end.Previous;
                }

                start = end;

                while (filter.IsIncluded(start.Value) && (start.Previous is object))
                {
                    start = start.Previous;
                }

                Updated = true;
            }

            public List<T> GetValues()
            {
                List<T> retValue = new List<T>();

                if ((start is object) && (end is object))
                {
                    var current = start;

                    do
                    {
                        retValue.Add(current.Value);
                        current = current.Next;
                    } while ((current != end) && (current is object));

                    retValue.Add(end.Value);
                }

                Updated = false;

                return retValue;
            }
        }

        private readonly TimeFilter filter;
        private readonly DataSet<TargetEvent> events;
        private readonly Dictionary<UInt32, DataSet<TimingRecord>> timings = new Dictionary<uint, DataSet<TimingRecord>>();
        private readonly Dictionary<UInt32, DataSet<TimingViolation>> violations= new Dictionary<uint, DataSet<TimingViolation>>();

        #region Properties
        #region Enabled Property
        private bool _enabled;

        public bool Enabled 
        {
            get => GetPropertyLocked(ref _enabled);
            set => SetPropertyLocked(ref _enabled, value);
        }

        #endregion
        #region TimeSpan Property
        private double _timeSpan = 60;

        public double TimeSpan
        {
            get => GetPropertyLocked(ref _timeSpan);
            set
            {
                lock (LockObject)
                {
                    filter.Duration = value;
                    events.Refilter();
                    Updated = events.Updated;

                    foreach (var pair in timings)
                    {
                        pair.Value.Refilter();
                        Updated = pair.Value.Updated;
                    }

                    foreach (var pair in violations)
                    {
                        pair.Value.Refilter();
                        Updated = pair.Value.Updated;
                    }
                    SetProperty(ref _timeSpan, value);
                }
            }
        }

        #endregion
        #region Updated Property
        private bool _updated;

        public bool Updated 
        {
            get => GetPropertyLocked(ref _updated);
            private set => SetPropertyLocked(ref _updated, value);
        }
        #endregion
        #region Paused Property
        private bool _paused;

        public bool Paused
        {
            get => GetPropertyLocked(ref _paused);
            set => SetPropertyLocked(ref _paused, value);
        }
        #endregion
        #endregion

        public Profiler()
        {
            filter = new TimeFilter() 
            { 
                End = 0, 
                Duration = TimeSpan 
            };
            events = new DataSet<TargetEvent>(filter);
        }

        public void Reset()
        {
            lock (LockObject)
            {
                ProfileTiming.Reset();
                events.Clear();
                timings.Clear();
                violations.Clear();
            }
        }

        #region Adding Profiling

        private void UpdateTime(double time)
        {
            if (!Paused)
            {
                filter.End = time;
            }
            else
            {
                if (filter.End < filter.Duration)
                {
                    filter.End = time;
                }
            }
        }


        public void Add(TargetEvent e)
        {
            if (e is null)
                throw new ArgumentNullException(nameof(e));

            if (Enabled)
            {
                UpdateTime(e.Time);

                lock (LockObject)
                {
                    events.Add(e);
                }
            }

            Updated = events.Updated;
        }

        public void Add(TimingRecord record)
        {
            if (record is null)
                throw new ArgumentNullException(nameof(record));

            if (Enabled)
            {
                DataSet<TimingRecord> set;

                lock (LockObject)
                {
                    UpdateTime(record.Time);

                    if (timings.ContainsKey(record.Signal))
                    {
                        timings[record.Signal].Add(record);
                        set = timings[record.Signal];
                    }
                    else
                    {
                        set = new DataSet<TimingRecord>(filter);
                        set.Add(record);
                        timings.Add(record.Signal, set);
                    }
                }

                Updated = set.Updated;
            }
        }

        public void Add(TimingViolation violation)
        {
            if (violation is null)
                throw new ArgumentNullException(nameof(violation));

            if (Enabled)
            {
                DataSet<TimingViolation> set;
                lock (LockObject)
                {
                    UpdateTime(violation.Time);

                    if (violations.ContainsKey(violation.Signal))
                    {
                        violations[violation.Signal].Add(violation);
                        set = violations[violation.Signal];
                    }
                    else
                    {
                        set = new DataSet<TimingViolation>(filter);
                        set.Add(violation);
                        violations.Add(violation.Signal, set);
                    }
                }

                Updated = set.Updated;
            }
        }

        #endregion

        public ProfileReport GetReport()
        {
            var report = new ProfileReport();

            lock (LockObject)
            {
                report.Events = events.GetValues();

                foreach (var pair in timings)
                {
                    var set = pair.Value;
                    var values = set.GetValues();
                    var signalTiming = new SignalTiming(signal: "Foo",
                                                        code: (int) pair.Key,
                                                        time: (from e in values select e.Time).ToArray(),
                                                        average: (from e in values select e.Average).ToArray(),
                                                        maximum: (from e in values select e.Max).ToArray(),
                                                        minimum: (from e in values select e.Min).ToArray()
                                                        );

                    report.Timing.Add(signalTiming);
                }

                report.Violation = new List<TimingViolation>();
                foreach (var pair in violations)
                {
                    var set = pair.Value;
                    report.Violation.AddRange(set.GetValues());
                }
            }

            Updated = false;

            return report;
        }
    }
}
