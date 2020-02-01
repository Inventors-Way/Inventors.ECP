using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.ECP
{
    public abstract class CommunicationLayer
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1034:Nested types should not be visible", Justification = "<Pending>")]
        public abstract class Statistics
        {
            public static string FormatRate(double rate, string unit = "B/s")
            {
                if (rate > 1048576)
                {
                    return String.Format(CultureInfo.CurrentCulture, "{0:0.00}M{1}", rate / 1048576, unit);
                }
                else if (rate > 1024)
                {
                    return String.Format(CultureInfo.CurrentCulture, "{0:0.00}k{1}", rate / 1024, unit);
                }
                else
                {
                    return String.Format(CultureInfo.CurrentCulture, "{0:0.00}{1}", rate, unit);
                }
            }
        }

        public abstract int BaudRate { get; set; }

        public abstract string Port { get; set; }

        public bool ResetOnConnection { get; set; } = false;

        public CommunicationLayer()
        {
            BytesTransmitted = 0;
            BytesReceived = 0;
        }

        public void Open(DeviceData device)
        {
            DoOpen(device);
            RestartStatistics();
        }

        public void RestartStatistics()
        {
            BytesReceived = 0;
            BytesTransmitted = 0;
            testWatch.Restart();
        }

        public abstract List<string> GetAvailablePorts();

        public CommunicationLayerStatistics GetStatistics()
        {
            double time = ((double)testWatch.ElapsedMilliseconds) / 1000;
            return new CommunicationLayerStatistics()
            {
                BytesTransmitted = BytesTransmitted,
                BytesReceived = BytesReceived,
                RxRate = ((double) BytesReceived) / time,
                TxRate = ((double) BytesTransmitted) / time
            };
        }

        protected abstract void DoOpen(DeviceData device);

        public void Close()
        {
            testWatch.Stop();
            DoClose();
        }

        protected abstract void DoClose();

        public abstract bool IsOpen { get; }

        public abstract void Transmit(byte[] frame);

        public Destuffer Destuffer { get; } = new Destuffer();

        protected long BytesReceived { get; set; }

        protected long BytesTransmitted { get; set; }

        private readonly Stopwatch testWatch = new Stopwatch();
    }
}
