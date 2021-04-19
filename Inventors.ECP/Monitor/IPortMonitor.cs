using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.ECP.Monitor
{
    public interface IPortMonitor
    {
        void Receive(DataChunk chunk);

        bool Enabled { get; }
    }
}
