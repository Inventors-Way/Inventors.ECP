using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ScottPlot;
using Inventors.ECP.Profiling;
using Inventors.ECP.Profiling.Analysis;

namespace Inventors.ECP.Tester.Profiling
{
    public partial class OverviewControl : 
        UserControl,
        IAnalysisControl
    {
        private PropertyBinder<Profiler> binder;
        private Profiler profiler;
        private Plot plot = new Plot();

        public bool Active { get; set; }

        public bool Dirty { get; private set; }

        public Control Control => this;

        public OverviewControl(Profiler profiler) 
        {
            InitializeComponent();
            Dock = DockStyle.Fill;
            this.profiler = profiler;
            binder = new PropertyBinder<Profiler>(profiler, this);
            binder.Bind(nameof(profiler.Overview), UpdateAnalysis);
        }

        private double Convert(double x)
        {
            if (Double.IsNaN(x))
                return 0;
            else if (x == Int32.MaxValue)
                return 0;
            else return x;
        }

        private double[] Convert(IList<double> x)
        {
            return (from v in x select Convert(v)).ToArray();
        }

        private void UpdateAnalysis()
        {
            if (Active)
            {
                if (profiler.Overview is OverviewAnalysis analysis)
                {
                    var x = analysis.X.ToArray();
                    var avg = Convert(analysis.Average);
                    var max = Convert(analysis.Maximum);
                    var globalMax = Convert(analysis.ScaleMaximum);

                    plot.Clear();

                    plot.PlotBar(xs: x, ys: avg, showValues: false, horizontal: true, label: "Average");
                    plot.PlotScatter(xs: max, ys: x, lineWidth: 0, markerSize: 5, label: "Maximum");
                    plot.PlotScatter(xs: globalMax, ys: x, lineWidth: 0, markerSize: 5, label: "Global Max");

                    plot.YTicks(x, analysis.Labels.ToArray());
                    plot.Grid(enableVertical: false, lineStyle: LineStyle.Dot);

                    plot.Frame(top: false, right: false);
                    plot.YLabel("Task ID");
                    plot.XLabel("Time [us]");
                    plot.Title("Task Overview");
                    plot.Axis(x1: 0, x2: globalMax.Max() * 1.05);

                    Dirty = true;
                }
            }
        }

        public void RefreshDisplay()
        {
            plot.Resize(width: pictureBox.Width, height: pictureBox.Height);
            pictureBox.Image = plot.GetBitmap();
            Dirty = false;
        }
    }
}
