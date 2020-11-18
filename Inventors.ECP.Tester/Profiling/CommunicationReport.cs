using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.ECP.Tester.Profiling
{
    public class CommunicationReport
    {
        public CommunicationReport(List<double> time,
                      int trials,
                      double RunTime)
        {
            if (time is null)
                throw new ArgumentException(nameof(time));

            Trials = trials;
            this.RunTime = RunTime;

            if (time.Count > 0)
            {
                Tavg = time.Average();
                Tstd = Math.Sqrt(time.Sum((v) => (v - Tavg) * (v - Tavg)) / time.Count);
                Tmin = time.Min();
                Tmax = time.Max();
                Success = 100 * (((double)time.Count) / ((double)trials));
            }
            else
            {
                Tavg = 0;
                Tstd = 0;
                Tmin = 0;
                Tmax = 0;
                Success = 0;
            }
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.AppendLine("Test Report");
            builder.AppendLine(String.Format(CultureInfo.CurrentCulture, "Success    : {0:0.00}%", Success));
            builder.AppendLine(String.Format(CultureInfo.CurrentCulture, "Time       : {0:0.00} +/- {1:0.00}ms, ({2}ms - {3}ms)", Tavg, Tstd, Tmin, Tmax));
            builder.Append(String.Format(CultureInfo.CurrentCulture, "Run Time   : {0:0.00}s", RunTime / 1000));

            return builder.ToString();
        }

        public int Trials { get; private set; }
        public double Tavg { get; private set; }
        public double Tstd { get; private set; }
        public double Tmin { get; private set; }
        public double Tmax { get; private set; }
        public double Success { get; private set; }

        public double RunTime { get; private set; }
    }
}
