using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Inventors.ECP.CommunicationLayer;

namespace Inventors.ECP.Profiling
{
    public class Profiler
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1034:Nested types should not be visible", Justification = "<Pending>")]
        public class Report
        {
            public Report(List<double> time, 
                          int trials,
                          CommunicationLayer.CommunicationLayerStatistics stats,
                          double RunTime)
            {
                if (!(time is object))
                    throw new ArgumentException(Resources.INVALID_TIME_NULL);

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

                Statistics = stats;
            }

            public override string ToString()
            {
                var builder = new StringBuilder();
                builder.AppendLine("Test Report");
                builder.AppendLine(String.Format(CultureInfo.CurrentCulture, "Success    : {0:0.00}%", Success));
                builder.AppendLine(String.Format(CultureInfo.CurrentCulture, "Time       : {0:0.00} +/- {1:0.00}ms, ({2}ms - {3}ms)", Tavg, Tstd, Tmin, Tmax));
                builder.AppendLine(String.Format(CultureInfo.CurrentCulture, "Data rate  : Rx: {0}, Tx: {1}",
                    CommunicationLayer.Statistics.FormatRate(Statistics.RxRate),
                    CommunicationLayer.Statistics.FormatRate(Statistics.TxRate)));
                builder.Append(String.Format(CultureInfo.CurrentCulture, "Run Time   : {0:0.00}s", RunTime/1000));

                return builder.ToString();
            }

            public int Trials { get; private set; }
            public double Tavg { get; private set; }
            public double Tstd { get; private set; }
            public double Tmin { get; private set; }
            public double Tmax { get; private set; }

            public double Success { get; private set; }

            public double RunTime { get; private set; }

            public CommunicationLayer.CommunicationLayerStatistics Statistics { get; private set; }
        }

        public class CommunicationProfile :
            CommunicationLayer.Statistics
        {
            public byte Code { get; set; } = 0;

            public double Rate { get; set; } = 0;

            public long Count { get; set; } = 0;

            public double Bytes { get; set; } = 0;
        }

        private class FrameStat
        {
            public long Count { get; set; } = 0;
            public long Bytes { get; set; } = 0;
        }

        private bool profiling = false;
        private readonly Dictionary<byte, FrameStat> framesReceived = new Dictionary<byte, FrameStat>();
        private Device device = null;

        public Device Device
        {
            get
            {
                return device;
            }
            set
            {
                if (device == null)
                {
                    device = value;

                    if (profiling && (device is object))
                    {
                        device.CommLayer.Destuffer.OnReceive += HandleIncommingFrame;
                        profileWatch.Restart();
                    }
                }
            }
        }

        public Function Function { get; set; } = null;

        private CommunicationLayer.CommunicationLayerStatistics statistics = new CommunicationLayer.CommunicationLayerStatistics();

        public int Trials { get; set; } = 100;

        public int TestDelay { get; set; } = 50;

        public List<double> Time { get; private set; } = new List<double>();

        public long RunTime { get; private set; } = 0;

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
                            if (device != null)
                            {
                                device.CommLayer.Destuffer.OnReceive += HandleIncommingFrame;
                            }
                            ClearProfile();
                            
                        }
                        else
                        {
                            if (device != null)
                            {
                                device.CommLayer.Destuffer.OnReceive -= HandleIncommingFrame;
                            }
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
                        CommunicationLayer.Statistics.FormatRate(p.Bytes),
                        CommunicationLayer.Statistics.FormatRate(p.Rate, "MSG/s")));
                }

            }

            return builder.ToString();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public void Test()
        {
            if (Device != null && Function != null)
            {
                ClearProfile();
                Time.Clear();
                watch.Restart();
                Device.CommLayer.RestartStatistics();
                for (int n = 0; n < Trials; ++n)
                {
                    try
                    {
                        Device.Execute(Function);
                        Time.Add(Function.TransmissionTime);
                    }
                    catch { }

                    if (TestDelay > 0)
                    {
                        Thread.Sleep(TestDelay);
                    }
                }
                watch.Stop();
                RunTime = watch.ElapsedMilliseconds;
                statistics = Device.CommLayer.GetStatistics();
                Device.CommLayer.RestartStatistics();

                Profiling = false;
            }
        }

        public Report Compile()
        {
            return new Report(Time, Trials, statistics, RunTime);
        }


        private Stopwatch watch = new Stopwatch();
        private Stopwatch profileWatch = new Stopwatch();
    }
}
