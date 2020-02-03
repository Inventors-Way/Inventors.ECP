using System;
using System.Collections.Generic;
using System.Text;
using Inventors.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Inventors.ECP.UnitTests
{
    [TestClass]
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Required by MSTests")]
        public static void OnAssemblyInitialize(TestContext tc)
        {
            Log.SetLogger(log);
        }
    }
}
