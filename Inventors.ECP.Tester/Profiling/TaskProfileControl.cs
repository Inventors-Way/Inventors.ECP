using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Inventors.ECP.Profiling;
using ScottPlot;
using Inventors.ECP.Profiling.Analysis;

namespace Inventors.ECP.Tester.Profiling
{
    public partial class TaskProfileControl : 
        UserControl,
        IAnalysisControl
    {
        private PropertyBinder<Profiler> binder;
        private Profiler profiler;
        private Plot plot = new Plot();

        public bool Active { get; set; }

        public bool Dirty { get; private set; }

        public Control Control => this;

        public TaskProfileControl(Profiler profiler)
        {
            InitializeComponent();
            Dock = DockStyle.Fill;
            this.profiler = profiler;
            binder = new PropertyBinder<Profiler>(profiler, this);
            binder.Bind(nameof(profiler.TaskProfile), UpdateAnalysis);
            binder.Bind(nameof(profiler.ActiveProfile), UpdateAnalysis);
            binder.Bind(nameof(profiler.AvailableProfiles), UpdateAvailableProfiles);

            eventsOffMenuItem.Enabled = eventsOnMenuItem.Checked = true;
            eventsOnMenuItem.Enabled = eventsOffMenuItem.Checked = false;
            violationsOffMenuItem.Enabled = violationsOnMenuItem.Checked = true;
            violationsOnMenuItem.Enabled = violationsOffMenuItem.Checked = false;
        }

        private void UpdateAvailableProfiles()
        {
            var profiles = profiler.AvailableProfiles;

            if (profiles is object)
            {
                if (activeTask.Items.Count > 0)
                {
                    var selected = activeTask.SelectedItem;

                    if (activeTask.Items.Count != profiles.Count)
                    {
                        foreach (var p in profiles)
                        {
                            if (!activeTask.Items.Contains(p))
                            {
                                activeTask.Items.Add(p);
                            }
                        }
                    }

                    activeTask.SelectedItem = selected;
                }
                else
                {
                    if (profiles.Count > 0)
                    {
                        activeTask.Items.AddRange(profiles.ToArray());
                        activeTask.SelectedIndex = 0;
                        profiler.ActiveProfile = activeTask.SelectedItem.ToString();
                    }
                }
            }
        }

        private void PlotViolations(IList<TimingViolation> violations)
        {
            if (violations is object)
            {
                var x = (from v in violations select v.Time).ToArray();
                var y = (from v in violations select v.ElapsedTime).ToArray();

                foreach (var v in violations)
                {
                    plot.PlotVLine(x: v.Time, color: Color.Red);

                    plot.PlotText($"Context: { v.Context }",
                                    x: v.Time,
                                    y: 0,
                                    alignment: TextAlignment.middleCenter,
                                    rotation: 270,
                                    color: Color.Black);
                }

                plot.PlotScatter(xs: x, ys: y, color: Color.Red, lineWidth: 0, markerSize: 5);
            }
        }

        private void PlotEvents(IList<TargetEvent> events)
        {
            if (events is object)
            {
                foreach (var x in events)
                {
                    plot.PlotVLine(x: x.Time, color: Color.Blue);

                    plot.PlotText($"Context: { x.Description }",
                                    x: x.Time,
                                    y: 0,
                                    alignment: TextAlignment.middleCenter,
                                    rotation: 270,
                                    color: Color.Black);
                }
            }
        }


        private void UpdateAnalysis()
        {
            if (Active)
            {
                if (profiler.TaskProfile is TaskAnalysis analysis)
                {
                    plot.Clear();
                    var min = analysis.Minimum.ToArray();
                    var max = analysis.Maximum.ToArray();
                    var average = analysis.Average.ToArray();
                    var t = analysis.Time.ToArray();

                    plot.PlotScatter(xs: t, ys: max);
                    plot.PlotScatter(xs: t, ys: average);
                    plot.PlotScatter(xs: t, ys: min);

                    if (violationsOnMenuItem.Checked)
                        PlotViolations(analysis.Violations);

                    if (eventsOnMenuItem.Checked)
                        PlotEvents(analysis.Events);

                    plot.Title(analysis.ID);
                    plot.XLabel("Time [s]");
                    plot.YLabel("Execution Time [us]");
                    plot.Frame(top: false, right: false);
                    plot.Legend(enableLegend: true, location: legendLocation.lowerLeft);

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

        private void EventsOnMenuItem_Click(object sender, EventArgs e)
        {
            eventsOffMenuItem.Enabled = eventsOnMenuItem.Checked = true;
            eventsOnMenuItem.Enabled = eventsOffMenuItem.Checked = false;
            UpdateAnalysis();
        }

        private void EventsOffMenuItem_Click(object sender, EventArgs e)
        {
            eventsOffMenuItem.Enabled = eventsOnMenuItem.Checked = false;
            eventsOnMenuItem.Enabled = eventsOffMenuItem.Checked = true;
            UpdateAnalysis();
        }

        private void ViolationsOnMenuItem_Click(object sender, EventArgs e)
        {
            violationsOffMenuItem.Enabled = violationsOnMenuItem.Checked = true;
            violationsOnMenuItem.Enabled = violationsOffMenuItem.Checked = false;
            UpdateAnalysis();
        }

        private void ViolationsOffMenuItem_Click(object sender, EventArgs e)
        {
            violationsOffMenuItem.Enabled = violationsOnMenuItem.Checked = false;
            violationsOnMenuItem.Enabled = violationsOffMenuItem.Checked = true;
            UpdateAnalysis();
        }

        private void ActiveTask_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (activeTask.SelectedItem is object)
            {
                profiler.ActiveProfile = activeTask.SelectedItem.ToString();
            }
        }
    }
}
