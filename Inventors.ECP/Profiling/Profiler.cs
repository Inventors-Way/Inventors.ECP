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

            public double Start { get; set; }

            public bool IsIncluded(Record value)
            {
                return (value.Time >= Start) && (value.Time <= End);
            }
        }

        private class DataSet<T>
            where T : Record
        {
            private DataNode<T> last;
            private DataNode<T> start;
            private DataNode<T> end;
            private readonly IFilter filter;

            public DataSet(IFilter filter)
            {
                this.filter = filter;
            }

            public void Clear()
            {
                last = start = end = null;
            }

            public void Add(T value)
            {
                var current = new DataNode<T>(value);

                if (last is object)
                {
                    last.Next = current;
                    current.Previous = last;
                    last = current;
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
                    } while (current != end);

                    retValue.Add(end.Value);
                }

                return retValue;
            }
        }

        private readonly TimeFilter filter = new TimeFilter();
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
        private double _timeSpan = Double.NaN;

        public double TimeSpan
        {
            get => GetPropertyLocked(ref _timeSpan);
            set => SetPropertyLocked(ref _timeSpan, value);
        }

        #endregion
        #endregion

        public Profiler()
        {
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
                    if (timings.ContainsKey(record.Signal))
                    {
                        timings[record.Signal].Add(record);
                    }
                    else
                    {
                        var set = new DataSet<TimingRecord>(filter);
                        set.Add(record);
                        timings.Add(record.Signal, set);                        
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
                lock (LockObject)
                {
                    if (violations.ContainsKey(violation.Signal))
                    {
                        violations[violation.Signal].Add(violation);
                    }
                    else
                    {
                        var set = new DataSet<TimingViolation>(filter);
                        set.Add(violation);
                        violations.Add(violation.Signal, set);
                    }
                }
            }
        }

        #endregion
    }
}
