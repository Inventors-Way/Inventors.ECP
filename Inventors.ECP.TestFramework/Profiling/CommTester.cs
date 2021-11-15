using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Inventors.ECP.TestFramework.Profiling
{
    public class CommTester
    {
        private readonly Stopwatch watch = new Stopwatch();

        public List<double> Time { get; private set; } = new List<double>();

        public long RunTime { get; private set; }

        public int Trials { get; set; } = 100;

        public int TestDelay { get; set; } = 50;

        public DeviceAddress Address { get; set; } = null;

        public BusCentral Master { get; set; }

        public CommunicationReport Test(DeviceFunction function)
        {
            if (Master is null)
                throw new ArgumentNullException(nameof(Master));

            if (function is null)
                throw new ArgumentNullException(nameof(function));
            
            Time.Clear();
            watch.Restart();

            for (int n = 0; n < Trials; ++n)
            {
                try
                {
                    Master.Execute(function, Address);
                    Time.Add(function.TransmissionTime);
                }
                catch
                {
                }

                if (TestDelay > 0)
                {
                    Thread.Sleep(TestDelay);
                }
            }
            watch.Stop();
            RunTime = watch.ElapsedMilliseconds;

            return new CommunicationReport(Time, Trials, RunTime);
        }

        public async Task<CommunicationReport> TestAsync(DeviceFunction function) => 
            await Task.Run(() => Test(function)).ConfigureAwait(false);
    }
}
