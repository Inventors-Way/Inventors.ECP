using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.ECP.Monitor
{
    public static class PortMonitor
    {
        private static readonly object lockObject = new object();
        private static IPortMonitor _monitor;

        public static void SetMonitor(IPortMonitor monitor)
        {
            lock (lockObject)
            {
                _monitor = monitor;
            }
        }

        public static void Add(bool rx, byte[] data)
        {
            lock (lockObject)
            {
                if (_monitor is object)
                {
                    if (_monitor.ProfilingEnabled)
                    {
                        _monitor.Receive(new DataChunk(rx, data));
                    }
                }
            }
        }

        public static bool Enabled
        {
            get
            {
                if (_monitor is null)
                    return false;

                return _monitor.ProfilingEnabled;
            }
            set
            {
                if (_monitor is object)
                {
                    _monitor.ProfilingEnabled = value;
                }
            }
        }
    }
}
