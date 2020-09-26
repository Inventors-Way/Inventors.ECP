using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.ECP.Profiling
{
    public class TimingRecord :
        Record
    {
        public TimingRecord(string id, double average, double min, double max)
        {
            ID = id;
            Average = average;
            Min = min;
            Max = max;
        }

        public string ID { get; }

        public double Max { get; }

        public double Min { get; }

        public double Average { get; }

        public override string ToString() =>
            $"TIMING [ { ID } ] Time: { Time }s, Average { Average }us, Max: { Max }us, Min: { Min }us";
    }
}
