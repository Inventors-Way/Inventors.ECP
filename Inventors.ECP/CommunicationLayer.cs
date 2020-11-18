using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.ECP
{
    public abstract class CommunicationLayer
    {
        public abstract CommunicationProtocol Protocol { get; }

        public abstract int BaudRate { get; set; }

        public abstract string Location { get; set; }

        public bool ResetOnConnection { get; set; }

        public CommunicationLayer()
        {
            BytesTransmitted = 0;
            BytesReceived = 0;
        }

        public void RestartStatistics()
        {
            BytesReceived = 0;
            BytesTransmitted = 0;
            testWatch.Restart();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>A list of device locations</returns>
        public abstract List<string> GetLocations();

        public CommunicationLayer SetLocation(string location)
        {
            Location = location;
            return this;
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

        public abstract bool IsConnected { get; }

        public abstract void Transmit(byte[] frame);

        internal Destuffer Destuffer { get; } = new Destuffer();

        public long BytesReceived { get; protected set; }

        public long BytesTransmitted { get; protected set; }

        public double RxRate => 1000 * ((double)BytesReceived) / ((double)testWatch.ElapsedMilliseconds);

        public double TxRate => 1000 * ((double)BytesTransmitted) / ((double)testWatch.ElapsedMilliseconds);

        private readonly Stopwatch testWatch = new Stopwatch();
    }
}
