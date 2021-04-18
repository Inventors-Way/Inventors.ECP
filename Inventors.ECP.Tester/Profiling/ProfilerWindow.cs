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
        private Device device;
        private Profiler profiler;

        public ProfilerWindow()
        {
            InitializeComponent();
            analysisMenu.Enabled = false;
            fileMenu.Enabled = false;
            clearProfilerToolStripMenuItem.Checked = false;
        }

        public void SetDevice(Device device)
        {
            if (device is null)
                throw new ArgumentNullException(nameof(device));

            this.device = device;
            this.profiler = device.Profiler;

            analysisMenu.Enabled = true;
            fileMenu.Enabled = true;
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
        }

        private void ResetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (device is object)
                device.Profiler.Reset();
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
