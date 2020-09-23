using Inventors.ECP.Communication;
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
    public class Profiler
    {
        private class FrameStat
        {
            public long Count { get; set; }
            public long Bytes { get; set; }
        }

        private bool profiling;
        private readonly Dictionary<byte, FrameStat> framesReceived = new Dictionary<byte, FrameStat>();


        public DeviceFunction Function { get; set; }

        private CommunicationLayerStatistics statistics = new CommunicationLayerStatistics();

        public int Trials { get; set; } = 100;

        public int TestDelay { get; set; } = 50;

        public List<double> Time { get; private set; } = new List<double>();

        public long RunTime { get; private set; }

        public Profiler(CommunicationLayer layer, DeviceMaster master)
        {
            this.commLayer = layer;
            this.master = master;
        }

        private void ClearProfile()
        {
            framesReceived.Clear();
            profileWatch.Restart();
        }

        public bool Profiling
        {
            get
            {
                lock (framesReceived)
                {
                    return profiling;
                }
            }
            set
            {
                if (value != profiling)
                {
                    lock (framesReceived)
                    {
                        if (value)
                        {
                            commLayer.Destuffer.OnReceive += HandleIncommingFrame;
                            ClearProfile();
                        }
                        else
                        {
                            commLayer.Destuffer.OnReceive -= HandleIncommingFrame;
                            profileWatch.Stop();
                        }

                        profiling = value;
                    }
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        private void HandleIncommingFrame(Destuffer caller, byte[] frame)
        {
            lock (framesReceived)
            {
                if (profiling)
                {
                    try
                    {
                        var response = new Packet(frame);

                        if (!framesReceived.ContainsKey(response.Code))
                        {
                            framesReceived.Add(response.Code, new FrameStat()
                            {
                                Bytes = frame.Length,
                                Count = 1
                            });
                        }
                        else
                        {
                            framesReceived[response.Code].Bytes += frame.Length;
                            framesReceived[response.Code].Count += 1;
                        }
                    }
                    catch {}
                }
            }
        }

        public List<CommunicationProfile> GetProfile()
        {
            List<CommunicationProfile> retValue = new List<CommunicationProfile>();

            lock (framesReceived)
            {
                double time = ((double)profileWatch.ElapsedMilliseconds) / 1000.0;

                foreach (var item in framesReceived)
                {
                    var stat = item.Value;

                    retValue.Add(new CommunicationProfile()
                    {
                        Code = item.Key,
                        Bytes = ((double) stat.Bytes) / time,
                        Rate = ((double)stat.Count) / time,
                        Count = stat.Count
                    });
                }
            }

            return retValue;
        }

        public static string CreateProfileReport(List<CommunicationProfile> profile)
        {
            var builder = new StringBuilder();

            if (profile is object)
            {
                string fmtString = " {0,-5} | {1,-12} | {2,-12} | {3,-12}";
                string fmtString2 = " {0,-5} | {1,12} | {2,12} | {3,12}";
                builder.AppendLine("PROFILE REPORT");
                builder.AppendLine(String.Format(CultureInfo.CurrentCulture, fmtString, "CODE", "COUNT", "DATA RATE", "FRAME RATE"));

                foreach (var p in profile)
                {
                    builder.AppendLine(String.Format(CultureInfo.CurrentCulture, fmtString2,
                        String.Format(CultureInfo.CurrentCulture, "0x{0:X2}", p.Code),
                        p.Count.ToString(CultureInfo.CurrentCulture),
                        Statistics.FormatRate(p.Bytes),
                        Statistics.FormatRate(p.Rate, "MSG/s")));
                }
            }

            return builder.ToString();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public ProfileReport Test()
        {
            if (Function is object)
            {
                ClearProfile();
                Time.Clear();
                watch.Restart();
                commLayer.RestartStatistics();

                for (int n = 0; n < Trials; ++n)
                {
                    try
                    {
                        master.Execute(Function);
                        Time.Add(Function.TransmissionTime);
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
                statistics = commLayer.GetStatistics();
                commLayer.RestartStatistics();

                Profiling = false;
            }

            return Compile();
        }

        public async Task<ProfileReport> TestAsync() => await Task.Run((Func<ProfileReport>)(() => Test())).ConfigureAwait(false);

        private ProfileReport Compile()
        {
            return new ProfileReport(Time, Trials, statistics, RunTime);
        }


        private readonly Stopwatch watch = new Stopwatch();
        private readonly Stopwatch profileWatch = new Stopwatch();
        private readonly CommunicationLayer commLayer;
        private readonly DeviceMaster master;
    }
}
