using Inventors.ECP.Functions;
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
        private Device device;

        public ProfilerWindow()
        {
            InitializeComponent();
            analysisMenu.Enabled = false;
            timeMenu.Enabled = false;
            fileMenu.Enabled = false;
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
            AddAnalysis("Overview", new OverviewControl(device.Profiler));
            AddAnalysis("Task Profile", new TaskProfileControl(device.Profiler));

            if (fileMenu.DropDownItems.Count > 0)
            {
                if (fileMenu.DropDownItems[0] is ToolStripMenuItem item)
                {
                    item.Checked = true;
                }
            }
        }

        private void AddAnalysis(string name, IAnalysisControl control)
        {
            var item = new ToolStripMenuItem(name)
            {
                Tag = control
            };
            item.Click += Analysis_Click;
            analysisMenu.DropDownItems.Add(item);
        }

        private void Analysis_Click(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem current)
            {
                foreach (var temp in analysisMenu.DropDownItems)
                {
                    if (temp is ToolStripMenuItem item)
                    {
                        item.Checked = item == current;
                    }
                }

                SetActive(current.Tag as IAnalysisControl);
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
    }
}
