using IronPython.Hosting;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using ScottPlot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.ECP.TestFramework.Analysis
{
    public class AnalysisEngine
    {
        public event EventHandler<Plot> PlotChanged;

        public AnalysisEngine(MessageAnalyser analyser)
        {
            if (File.Exists(analyser.Script))
                throw new InvalidOperationException($"Script file {analyser.Script} does not exists");

            var script = File.ReadAllText(analyser.Script);
            var source = _engine.CreateScriptSourceFromString(script, SourceCodeKind.Statements);
            _code = source.Compile();

            Initialize();

            analyser.OnMessage += OnMessage;
        }

        private void Initialize()
        {
            _dataSet.Reset();
            var scope = _engine.CreateScope(new Dictionary<string, object>
            {
                { "data", _dataSet }
            });
            _code.Execute(scope);
            dynamic function = scope.GetVariable("initialize");

            function();
        }

        private static ScriptEngine CreateEngine()
        {
            var engine = Python.CreateEngine();
            var runtime = engine.Runtime;
            runtime.LoadAssembly(typeof(DateTime).Assembly);
            runtime.LoadAssembly(typeof(Plot).Assembly);

            return engine;
        }


        private void OnMessage(object sender, DeviceMessage message)
        {
            if (sender is not MessageAnalyser)
                return;

            var scope = _engine.CreateScope(new Dictionary<string, object>
            {
                { "plt", _plot },
                { "data", _dataSet }
            });
            _code.Execute(scope);
            dynamic function = scope.GetVariable("analyse");

            _plot.Clear();
            function(message);

            PlotChanged?.Invoke(this, _plot);
        }

        public Plot Plot => _plot;

        private static readonly ScriptEngine _engine = CreateEngine();
        private readonly CompiledCode _code;
        private readonly Plot _plot = new();
        private readonly DataSet _dataSet = new();

    }
}
