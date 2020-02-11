using Inventors.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventors.ECP.Hosting
{
    public partial class HostingForm : Form
    {
        private Logger logger;

        public Icon NotifyIcon
        {
            get => notifyIcon.Icon;
            set => notifyIcon.Icon = value;
        }

        public HostingForm()
        {
            InitializeComponent();
            SetupLogging();
            SetLoggingLevel(Settings.Level);
        }

        private void SetupLogging()
        {
            logger = new Logger() { Box = logBox };
            Log.SetLogger(logger);
            Log.Level = Settings.Level;
        }

        private void InstallDeviceMenuItem_Click(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog()
            {
                Title = "Select device package",
                Filter = "Device Package Files|*.devx"
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                var package = new DevicePackage(dialog.FileName);

                try
                {
                    var loader = package.Install();
                }
                catch (Exception ex)
                {
                    Log.Error(ex.Message);
                }
            }
        }

        private void RemoveDeviceMenuItem_Click(object sender, EventArgs e)
        {
            if (deviceList.SelectedItem is IHostedDevice device)
            {

            }
        }

        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Show();
            this.WindowState = FormWindowState.Normal;
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _exitAllowed = true;
            Application.Exit();
        }

        private void NotifyIcon_DoubleClick(object sender, EventArgs e)
        {
            Show();
            this.WindowState = FormWindowState.Normal;
        }

        private void HostingForm_Resize(object sender, EventArgs e)
        {
            //if the form is minimized  
            //hide it from the task bar  
            //and show the system tray icon (represented by the NotifyIcon control)  
            if (this.WindowState == FormWindowState.Minimized)
            {
                Hide();
            }
        }

        private void HostingForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!_exitAllowed)
            {
                if (e.CloseReason == CloseReason.UserClosing)
                {
                    e.Cancel = true;
                    Hide();
                }
            }
        }

        private bool _exitAllowed = true; // Change for when released

        private void Run_Click(object sender, EventArgs e)
        {

        }

        private void Stop_Click(object sender, EventArgs e)
        {

        }

        private void DeviceList_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateDevice();
        }

        private void UpdateDevice()
        {
            if (deviceList.SelectedIndex >= 0)
            {
                propertyGrid.SelectedObject = deviceList.SelectedItem;
            }
        }


        private void SetLoggingLevel(LogLevel level)
        {
            debugToolStripMenuItem.Checked = level == LogLevel.DEBUG;
            statusToolStripMenuItem.Checked = level == LogLevel.STATUS;
            errorToolStripMenuItem.Checked = level == LogLevel.ERROR;
            Log.Level = level;
            Settings.Level = level;
        }

        private void DebugToolStripMenuItem_Click(object sender, EventArgs e) => SetLoggingLevel(LogLevel.DEBUG);
        private void StatusToolStripMenuItem_Click(object sender, EventArgs e) => SetLoggingLevel(LogLevel.STATUS);
        private void ErrorToolStripMenuItem_Click(object sender, EventArgs e) => SetLoggingLevel(LogLevel.ERROR);
    }
}
