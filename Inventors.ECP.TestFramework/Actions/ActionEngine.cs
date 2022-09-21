using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using ScottPlot;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.ECP.TestFramework.Actions
{
    public class ActionEngine
    {
        public static ScriptEngine GetInstance()
        {
            if (_instance is null)
                throw new InvalidOperationException("Action runtine has not been initialized");

            return _instance._engine;
        }

        public static void Initialize(Assembly commLib)
        {
            if (_instance is not null)
                throw new InvalidOperationException("Action runtime has allready been initialized");

            _instance = new ActionEngine(commLib);

        }

        private ActionEngine(Assembly commLib)
        {
            _engine = Python.CreateEngine();
            var runtime = _engine.Runtime;
            runtime.LoadAssembly(typeof(DateTime).Assembly);
            runtime.LoadAssembly(typeof(Plot).Assembly);
            runtime.LoadAssembly(typeof(Log).Assembly);
            runtime.LoadAssembly(typeof(Device).Assembly);
            runtime.LoadAssembly(commLib);
        }

        private readonly ScriptEngine _engine;
        private static ActionEngine _instance;
    }
}
