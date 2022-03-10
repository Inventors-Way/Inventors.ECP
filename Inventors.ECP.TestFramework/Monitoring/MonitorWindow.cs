using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Inventors.ECP.Monitor;

namespace Inventors.ECP.TestFramework.Monitoring
{
    public partial class MonitorWindow : 
        Form,
        IPortMonitor
    {
        public event EventHandler<bool> OnMonitorClosed;

        private bool paused;
        private bool updated;
        private StringBuilder buffer = new StringBuilder();
        private readonly object lockObject = new object();
        private bool msgEnabled = true;

        public MonitorWindow()
        {
            InitializeComponent();
            messagesToolStripMenuItem.Checked = msgEnabled;
        }

        private bool _enabled;

        public bool ProfilingEnabled
        {
            get => _enabled;
            set => _enabled = value;
        }

        public void Receive(DataChunk chunk)
        {
            lock (lockObject)
            {

                if (chunk.Data[2] >= 0x80)
                {
                    var dir = "MSG";

                    if (msgEnabled)
                    {
                        buffer.AppendLine($"{chunk.Time.ToLongTimeString()} | {dir} | DATA: {chunk}");
                        updated = true;
                    }
                }
                else
                {
                    var dir = chunk.Rx ? "RX " : "TX ";
                    buffer.AppendLine($"{chunk.Time.ToLongTimeString()} | {dir} | DATA: {chunk}");
                    updated = true;
                }
            }
        }

        private void MonitorWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                OnMonitorClosed?.Invoke(this, false);
            }
        }

        private void PausedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            paused = !paused;
            pausedToolStripMenuItem.Checked = paused;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (paused)
                return;

            lock (lockObject)
            {
                if (updated)
                {
                    textBox.AppendText(buffer.ToString());
                    updated = false;
                    buffer.Clear();
                    ScrollToEnd();
                }
            }
        }

        private void ScrollToEnd()
        {
            if (textBox.Visible)
            {
                textBox.SelectionStart = textBox.Text.Length;
                textBox.ScrollToCaret();
            }
        }

        private void messagesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lock (lockObject)
            {
                msgEnabled = !msgEnabled;
                messagesToolStripMenuItem.Checked = msgEnabled;
            }
        }
    }
}
