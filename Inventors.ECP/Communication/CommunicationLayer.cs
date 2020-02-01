using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.ECP.Communication
{
    public abstract class CommunicationLayer
    {
        public abstract int BaudRate { get; set; }

        public abstract string Port { get; set; }

        public bool ResetOnConnection { get; set; } = false;

        public CommunicationLayer(DeviceData device)
        {
            if (!(device is object))
                throw new ArgumentNullException(nameof(device));

            Identification = device;
            BytesTransmitted = 0;
            BytesReceived = 0;
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
                RxRate = ((double)BytesReceived) / time,
                TxRate = ((double)BytesTransmitted) / time
            };
        }

        public void Open()
        {
            DoOpen();
            RestartStatistics();
        }

        protected abstract void DoOpen();

        public void Close()
        {
            testWatch.Stop();
            DoClose();
        }

        protected abstract void DoClose();

        public abstract bool IsOpen { get; }

        public abstract void Transmit(byte[] frame);

        internal Destuffer Destuffer { get; } = new Destuffer();

        protected long BytesReceived { get; set; }

        protected long BytesTransmitted { get; set; }

        public DeviceData Identification { get; }

        private readonly Stopwatch testWatch = new Stopwatch();
    }
}
