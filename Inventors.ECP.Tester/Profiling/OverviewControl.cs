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

        private void UpdateAnalysis()
        {
            if (Active)
            {
                if (profiler.Overview is OverviewAnalysis analysis)
                {
                    plot.Clear();

                    plot.PlotScatter(xs: analysis.Maximum.ToArray(), ys: analysis.X.ToArray(), lineWidth: 0, markerSize: 5, label: "Maximum");
                    plot.PlotScatter(xs: analysis.ScaleMaximum.ToArray(), ys: analysis.X.ToArray(), lineWidth: 0, markerSize: 10, label: "Maximum");
                    plot.PlotBar(xs: analysis.Average.ToArray(), ys: analysis.X.ToArray(), showValues: false, horizontal: true, label: "Average");
                    plot.PlotScatter(xs: analysis.Minimum.ToArray(), ys: analysis.X.ToArray(), lineWidth: 0, markerSize: 5, label: "Minimum");

                    plot.YTicks(analysis.X.ToArray(), analysis.Labels.ToArray());
                    plot.Grid(enableVertical: false, lineStyle: LineStyle.Dot);

                    plot.Frame(top: false, right: false);
                    plot.YLabel("Timer ID");
                    plot.XLabel("Time [us]");
                    plot.Title("Timer Statistics");

                    Dirty = true;
                }
            }
        }

        public void RefreshDisplay()
        {
            plot.Resize(width: pictureBox.Width, height: pictureBox.Height);
            pictureBox.Image = plot.GetBitmap();
        }
    }
}
