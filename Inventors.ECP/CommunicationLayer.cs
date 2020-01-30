using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.ECP
{
    public abstract class CommunicationLayer
    {
        public abstract class Statistics
        {
            public string FormatRate(double rate, string unit = "B/s")
            {
                if (rate > 1048576)
                {
                    return String.Format("{0:0.00}M{1}", rate / 1048576, unit);
                }
                else if (rate > 1024)
                {
                    return String.Format("{0:0.00}k{1}", rate / 1024, unit);
                }
                else
                {
                    return String.Format("{0:0.00}{1}", rate, unit);
                }
            }
        }

        public class CommunicationLayerStatistics :
            Statistics
        {
            public long BytesTransmitted { get; set; } = 0;
            public long BytesReceived { get; set; } = 0;
            public double RxRate { get; set; } = 0;
            public double TxRate { get; set; } = 0;
        }

        public abstract int BaudRate { get; set; }

        public bool ResetOnConnection { get; set; } = false;

        public CommunicationLayer()
        {
            bytesTransmitted = 0;
            bytesReceived = 0;
        }

        public void Open(DeviceData device)
        {
            DoOpen(device);
            RestartStatistics();
        }

        public void RestartStatistics()
        {
            bytesReceived = 0;
            bytesTransmitted = 0;
            testWatch.Restart();
        }

        public CommunicationLayerStatistics GetStatistics()
        {
            double time = ((double)testWatch.ElapsedMilliseconds) / 1000;
            return new CommunicationLayerStatistics()
            {
                BytesTransmitted = bytesTransmitted,
                BytesReceived = bytesReceived,
                RxRate = ((double) bytesReceived) / time,
                TxRate = ((double) bytesTransmitted) / time
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


        public Destuffer Destuffer
        {
            get
            {
                return destuffer;
            }
        }

        protected Destuffer destuffer = new Destuffer();
        protected long bytesReceived;
        protected long bytesTransmitted;
        private Stopwatch testWatch = new Stopwatch();
    }
}
