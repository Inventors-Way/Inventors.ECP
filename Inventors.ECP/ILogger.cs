using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.ECP
{
    public enum LogLevel
    {
        DEBUG = 0,
        STATUS,
        ERROR,
        DISABLED
    }

    public interface ILogger
    {
        void Initialize();

        void Add(DateTime time, LogLevel level, string str);

        void AddMonitor(ILogger log);
    }
}
