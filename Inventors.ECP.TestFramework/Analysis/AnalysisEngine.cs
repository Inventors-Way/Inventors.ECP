using IronPython.Hosting;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using ScottPlot;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Microsoft.Scripting.Hosting.Shell.ConsoleHostOptions;
using static System.Windows.Forms.AxHost;

namespace Inventors.ECP.TestFramework.Analysis
{
    public class AnalysisEngine
    {
        public event EventHandler<Plot> PlotChanged;

        public AnalysisEngine(MessageAnalyser analyser, string path)
        {
            var filename = path + analyser.Script;

            if (!string.IsNullOrEmpty(analyser.Path))
                filename = path + analyser.Path + analyser.Script;

            if (!File.Exists(filename))
                throw new InvalidOperationException($"Script file {filename} does not exists");

            foreach (var p in analyser.Parameters)
            {
                _variables.Add(p.Name, p.GetValue());
            }

            var script = File.ReadAllText(filename);
            var source = _engine.CreateScriptSourceFromString(script, SourceCodeKind.Statements);
            _code = source.Compile();
            _msgCode = analyser.Code;
            _name = analyser.Name;
            _dataSet.SetNumberOfSignals(analyser.Signals);
            _dataSet.Reset();
            _state = AnalysisState.STOPPED;

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
            if (_state != AnalysisState.RUNNING)
                return;

            if (sender is not MessageAnalyser)
                return;

            var scope = _engine.CreateScope(_variables);
            scope.SetVariable("plt", _plot);
            scope.SetVariable("data", _dataSet);
            scope.SetVariable("msg", message);

            lock (lockObject)
            {
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
                    Log.Error("Error: {message}", e.Message);
                }
            }
        }

        public bool StartPossible => (_state == AnalysisState.PAUSED) || (_state == AnalysisState.STOPPED);

        public void Start()
        {
            if (!StartPossible)
                return;

            _state = AnalysisState.RUNNING;
        }

        public bool PausePossible => _state == AnalysisState.RUNNING;

        public void Pause()
        {
            if (!PausePossible)
                return;

            _state = AnalysisState.PAUSED;
        }

        public bool StopPossible => (_state == AnalysisState.RUNNING) || (_state == AnalysisState.PAUSED);

        public void Stop()
        {
            if (!StopPossible)
                return;

            lock (lockObject)
            {
                _state = AnalysisState.STOPPED;
                _dataSet.Reset();
                _plot.Clear();
                Updated = true;
            }
        }

        public void Resize(int width, int height)
        {
            lock (lockObject)
            {
                _plot.Resize(width, height);
            }
        }

        public Bitmap GetBitmap()
        {
            lock (lockObject)
            {
                Updated = false;
                return _plot.GetBitmap();
            }
        }

        public void SaveFig(string filename)
        {
            bool restart = false;

            if (_state == AnalysisState.RUNNING)
            {
                Pause();
                restart = true;
            }

            _plot.SaveFig(filename);

            if (restart)
                Start();
        }

        public bool Updated { get; set; } = false;

        public AnalysisState State => _state;

        public override string ToString() => $"[{_msgCode}] {_name}";

        public void SaveResults(string fileName)
        {
            bool restart = false;

            if (_state == AnalysisState.RUNNING)
            {
                Pause();
                restart = true;
            }

            StringBuilder builder = new();

            for (int n = 0; n < _dataSet.Length; ++n)
            {
                var row = _dataSet.GetRow(n);
                var str = String.Join(";", (from v in row
                                            select v.ToString(CultureInfo.InvariantCulture)));
                builder.AppendLine(str);
            }

            if (restart)
                Start();

            File.WriteAllText(fileName, builder.ToString());
        }

        private static readonly ScriptEngine _engine = CreateEngine();
        private readonly CompiledCode _code;
        private readonly Plot _plot = new();
        private readonly DataSet _dataSet = new();
        private readonly byte _msgCode;
        private readonly string _name;
        private AnalysisState _state;
        private readonly object lockObject = new();
        private readonly Dictionary<string, object> _variables = new();
    }
}
