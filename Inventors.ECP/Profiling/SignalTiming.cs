using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.ECP.Profiling
{
    public class SignalTiming
    {
        public string Signal { get; }

        public int Code { get; }

        public double[] Time { get; }

        public double[] Average { get; }

        public double[] Minimum { get; }

        public double[] Maximum { get; }

        internal SignalTiming(string signal,
                            int code,
                            double[] time,
                            double[] average,
                            double[] minimum,
                            double[] maximum)
        {
            Signal = signal;
            Code = code;
            Time = time;
            Average = average;
            Maximum = maximum;
            Minimum = minimum;
        }
    }
}
