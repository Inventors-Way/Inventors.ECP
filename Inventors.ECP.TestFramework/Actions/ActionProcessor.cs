using Inventors.ECP.TestFramework.Analysis;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using Microsoft.Scripting;
using ScottPlot;
using Serilog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventors.ECP;
using System.Reflection;

namespace Inventors.ECP.TestFramework.Actions
{
    public class ActionProcessor
    {
        public ActionProcessor(CustomAction action, string path)
        {
            var filename = path + action.Script;

            if (!string.IsNullOrEmpty(action.Path))
                filename = path + action.Path + action.Script;

            if (!File.Exists(filename))
                throw new InvalidOperationException($"Script file {filename} does not exists");


            foreach (var p in action.Parameters)
            {
                _variables.Add(p.Name, p.GetValue());
            }

            var engine = ActionEngine.GetInstance();

            var script = File.ReadAllText(filename);
            var source = engine.CreateScriptSourceFromString(script, SourceCodeKind.Statements);
            _code = source.Compile();
            _name = action.Name;
        }

        public void Run(Device device)
        {
            var engine = ActionEngine.GetInstance();
            var scope = engine.CreateScope(_variables);
            scope.SetVariable("dev", device);
            scope.SetVariable("dialog", dialogs);

            try
            {
                _code.Execute(scope);
            }
            catch (Exception e)
            {
                Log.Error("Error: {message}", e.Message);
            }
        }

        public override string ToString() => _name;

        private readonly CompiledCode _code;
        private readonly string _name;
        private readonly DialogEngine dialogs = new();
        private readonly Dictionary<string, object> _variables = new();
    }
}
