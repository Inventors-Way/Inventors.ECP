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

        public ProfilerWindow()
        {
            InitializeComponent();
        }

        public void SetDevice(Device device)
        {
            if (device is null)
                throw new ArgumentNullException(nameof(device));


        }

        private void ProfilerWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                OnProfilerClosed?.Invoke(this, false);
            }
        }
    }
}
