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
        }

        public void SetDevice(Device device)
        {
            if (device is null)
                throw new ArgumentNullException(nameof(device));

            this.device = device;
            CreateAnalyses();
        }

        private void CreateAnalyses()
        {
            AddAnalysis("Overview", new OverviewControl(device.Profiler));

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
    }
}
