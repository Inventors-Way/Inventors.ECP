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
        private readonly static ConsoleLog log = new ConsoleLog();

        public void Add(DateTime time, LogLevel level, string str) => Console.WriteLine($"[{level}: {str}");

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Required by interface")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1801:Review unused parameters", Justification = "Required by interface")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Required by interface")]
        public void AddMonitor(ILogger log)  {}

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Required by interface")]
        public void Initialize() { }

        [AssemblyInitialize]
        public static void OnAssemblyInitialize(TestContext _) => Log.SetLogger(log);
    }
}
