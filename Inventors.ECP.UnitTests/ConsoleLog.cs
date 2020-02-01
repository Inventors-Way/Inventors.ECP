using System;
using System.Collections.Generic;
using System.Text;
using Inventors.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Inventors.ECP.UnitTests
{
    public class ConsoleLog :
        ILogger
    {
        public static ConsoleLog log = new ConsoleLog();

        public void Add(DateTime time, LogCategory category, LogLevel level, string str)
        {
            Console.WriteLine("[{0}: {1}", level.ToString(), str);
        }

        public void AddMonitor(LogCategory category, ILogger log)  {}

        public void Initialize() { }

        [AssemblyInitialize]
        public void OnAssemblyInitialize()
        {
            Log.SetLogger(log);
        }
    }
}
