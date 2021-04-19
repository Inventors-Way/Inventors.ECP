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
        public TimingRecord(UInt32 signal, double average, double min, double max)
        {
            Signal = signal;
            Average = average;
            Min = min;
            Max = max;
        }

        public UInt32 Signal { get; }

        public double Max { get; }

        public double Min { get; }

        public double Average { get; }

        public override string ToString() =>
            $"TIMING [ { Signal } ] Time: { Time }s, Average { Average }us, Max: { Max }us, Min: { Min }us";
    }
}
