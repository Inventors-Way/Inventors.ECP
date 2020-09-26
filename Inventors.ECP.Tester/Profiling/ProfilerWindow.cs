using Inventors.ECP.Functions;
using Inventors.ECP.Profiling;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventors.ECP.Tester.Profiling
{
    public partial class ProfilerWindow : Form
    {
        public event EventHandler<bool> OnProfilerClosed;
        private IAnalysisControl activeControl;
        private readonly Dictionary<IAnalysisControl, ToolStripMenuItem> analysis = new Dictionary<IAnalysisControl, ToolStripMenuItem>();
        private Device device;

        public ProfilerWindow()
        {
            InitializeComponent();
            analysisMenu.Enabled = false;
            timeMenu.Enabled = false;
            fileMenu.Enabled = false;
            clearProfilerToolStripMenuItem.Checked = false;
        }

        public void SetDevice(Device device)
        {
            if (device is null)
                throw new ArgumentNullException(nameof(device));

            this.device = device;
            CreateAnalyses();
            UpdateTimeSpan();

            analysisMenu.Enabled = true;
            timeMenu.Enabled = true;
            fileMenu.Enabled = true;
        }

        private void CreateAnalyses()
        {
            activeControl = AddAnalysis("Overview", new OverviewControl(device.Profiler));
            AddAnalysis("Task Profile", new TaskProfileControl(device.Profiler));
            analysis[activeControl].Checked = true;
            SetActive(activeControl);
        }

        private IAnalysisControl AddAnalysis(string name, IAnalysisControl control)
        {
            var item = new ToolStripMenuItem(name)
            {
                Tag = control
            };
            item.Click += Analysis_Click;
            analysisMenu.DropDownItems.Add(item);
            analysis.Add(control, item);

            return control;
        }

        private void Analysis_Click(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem current)
            {
                SetActive(current.Tag as IAnalysisControl);
                UpdateAnalysis();
            }
        }

        private void SetActive(IAnalysisControl control)
        { 
            if (control is object)
            {
                activeControl = control;
                activeControl.Active = true;
                panel.Controls.Clear();
                panel.Controls.Add(activeControl.Control);
            }
        }

        private void ProfilerWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                OnProfilerClosed?.Invoke(this, false);
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (activeControl is object)
            {
                activeControl.RefreshDisplay();
            }
        }

        private void ResetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (device is object)
                device.Profiler.Reset();
        }

        private void TimeSpan60s_Click(object sender, EventArgs e)
        {
            if (device is object)
                device.Profiler.TimeSpan = 60;

            UpdateTimeSpan();
        }

        private void TimeSpan300s_Click(object sender, EventArgs e)
        {
            if (device is object)
                device.Profiler.TimeSpan = 300;

            UpdateTimeSpan();
        }

        private void TimeSpan600s_Click(object sender, EventArgs e)
        {
            if (device is object)
                device.Profiler.TimeSpan = 600;

            UpdateTimeSpan();
        }

        private void TimeSpanOffToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (device is object)
                device.Profiler.TimeSpan = Double.NaN;

            UpdateTimeSpan();
        }

        private void UpdateTimeSpan()
        {
            if (device is object)
            {
                TimeSpan60sMenuItem.Checked = device.Profiler.TimeSpan == 60;
                TimeSpan300sMenuItem.Checked = device.Profiler.TimeSpan == 300;
                TimeSpan600sMenuItem.Checked = device.Profiler.TimeSpan == 600;
                TimeSpanOffMenuItem.Checked = device.Profiler.TimeSpan == Double.NaN;
            }
        }

        private void UpdateAnalysis()
        {
            foreach (var item in analysis)
            {
                analysis[item.Key].Checked = item.Key == activeControl;
                analysis[item.Key].Enabled= item.Key != activeControl;
            }
        }

        private void CloseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OnProfilerClosed?.Invoke(this, false);
        }

        private void ClearProfilerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (device is object)
            {
                device.Profiler.Reset();
            }
        }
    }
}
