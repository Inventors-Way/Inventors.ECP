using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Inventors.ECP.UnitTests
{
    [TestClass]
    public class ConsoleLog :
        ILogger
    {
        public static ConsoleLog log = new ConsoleLog();

        public void Add(DateTime time, LogLevel level, string str) => Console.WriteLine($"[{level}: {str}");

        public void AddMonitor(ILogger log)  {}

        public void Initialize() { }

        [AssemblyInitialize]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Required by MSTests")]
        public static void OnAssemblyInitialize(TestContext _) => Log.SetLogger(log);
    }
}
