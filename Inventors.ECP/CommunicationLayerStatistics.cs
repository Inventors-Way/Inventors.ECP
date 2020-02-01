using System;
using System.Collections.Generic;
using System.Text;

namespace Inventors.ECP
{
    public class CommunicationLayerStatistics :
        CommunicationLayer.Statistics
    {
        public long BytesTransmitted { get; set; } = 0;
        public long BytesReceived { get; set; } = 0;
        public double RxRate { get; set; } = 0;
        public double TxRate { get; set; } = 0;
    }
}
