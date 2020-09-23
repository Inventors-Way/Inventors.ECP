using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.ECP.Profiling
{
    public class CommRecord : 
        Record
    {
        public CommRecord(double rxRate, double txRate)
        {
            RxRate = rxRate;
            TxRate = txRate;
        }

        public double RxRate { get; }

        public double TxRate { get; }
    }
}
