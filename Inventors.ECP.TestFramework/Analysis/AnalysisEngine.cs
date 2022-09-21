using IronPython.Hosting;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using ScottPlot;
using Serilog;
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

        public AnalysisEngine(MessageAnalyser analyser, string path)
        {
            if (File.Exists(analyser.Script))
                throw new InvalidOperationException($"Script file {analyser.Script} does not exists");

            var script = File.ReadAllText(Path.Combine(path, analyser.Script));
            var source = _engine.CreateScriptSourceFromString(script, SourceCodeKind.Statements);
            _code = source.Compile();
            _msgCode = analyser.Code;
            _name = analyser.Name;
            _dataSet.SetNumberOfSignals(analyser.Signals);
            _dataSet.Reset();


            analyser.OnMessage += OnMessage;
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
                { "data", _dataSet },
                { "msg", message }
            });
            _plot.Clear();
            _dataSet.AddRow();

            try
            {
                _code.Execute(scope);
                Updated = true;

                PlotChanged?.Invoke(this, _plot);
            }
            catch (Exception e)
            {
                Log.Error("Error [ {0} ]: {1} ", e.Message, e);
            }
        }

        public Plot Plot => _plot;

        public bool Updated { get; set; } = false;

        public override string ToString() => $"[{_msgCode}] {_name}";

        private static readonly ScriptEngine _engine = CreateEngine();
        private readonly CompiledCode _code;
        private readonly Plot _plot = new();
        private readonly DataSet _dataSet = new();
        private readonly byte _msgCode;
        private readonly string _name;

    }
}
