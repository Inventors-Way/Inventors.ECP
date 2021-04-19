using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.ECP.Monitor
{
    public class DataChunk
    {
        public DateTime Time { get; }

        public bool Rx { get; }

        public byte[] Data { get; }

        public DataChunk(bool rx, byte[] data)
        {
            Time = DateTime.Now;
            Rx = rx;
            Data = data;
        }
    }
}
