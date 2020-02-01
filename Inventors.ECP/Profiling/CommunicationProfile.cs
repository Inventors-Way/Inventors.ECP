using Inventors.ECP.Communication;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inventors.ECP.Profiling
{
    public class CommunicationProfile :
        Statistics
    {
        public byte Code { get; set; } = 0;

        public double Rate { get; set; } = 0;

        public long Count { get; set; } = 0;

        public double Bytes { get; set; } = 0;
    }
}
