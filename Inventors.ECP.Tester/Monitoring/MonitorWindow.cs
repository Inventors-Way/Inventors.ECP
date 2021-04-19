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

namespace Inventors.ECP.Tester.Monitoring
{
    public partial class MonitorWindow : 
        Form,
        IPortMonitor
    {
        public event EventHandler<bool> OnMonitorClosed;

        public MonitorWindow()
        {
            InitializeComponent();
        }

        private bool _enabled;

        public bool ProfilingEnabled
        {
            get => _enabled;
            set => _enabled = value;
        }

        public void Receive(DataChunk chunk)
        {
            listView.Items.Add(CreateItem(chunk));
        }

        private ListViewItem CreateItem(DataChunk chunk)
        {
            var dir = chunk.Rx ? "Rx" : "Tx";
            var retValue = new ListViewItem(chunk.Time.ToShortTimeString());
            retValue.SubItems.Add(dir);
            retValue.SubItems.Add(chunk.ToString());
            return retValue;
        }

        private void MonitorWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                OnMonitorClosed?.Invoke(this, false);
            }
        }
    }
}
