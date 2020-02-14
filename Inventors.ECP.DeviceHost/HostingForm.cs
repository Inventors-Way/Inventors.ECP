using Inventors.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventors.ECP.DeviceHost
{
    public partial class HostingForm : Form
    {
        private Logger logger;

        public Icon NotifyIcon
        {
            get => notifyIcon.Icon;
            set => notifyIcon.Icon = value;
        }

        public bool CloseToTray { get; set; } = true;

        public bool MinimizeToTray { get; set; } = true;

        public string Product { get; set; }

        public string Version { get; set; }

        public string Description { get; set; }

        public Image AboutImage { get; set; }

        public HostingForm()
        {
            InitializeComponent();
            SetupLogging();
            SetLoggingLevel(Settings.Level);
            SetupDevices();
            UpdateDeviceState(null);
        }

        private void SetupLogging()
        {
            logger = new Logger() { Box = logBox };
            Log.SetLogger(logger);
            Log.Level = Settings.Level;
        }

        private void SetupDevices()
        {
            foreach (var dev in Settings.Devices)
            {
                if (!dev.RemoveAtStart)
                {
                    try
                    {
                        if (dev.Create() is IHostedDevice device)
                        {
                            if (dev.State == DeviceState.RUNNING)
                            {
                                device.Run();
                            }

                            device.OnPropertyChanged += (p) => BeginInvoke((Action)(() => propertyGrid.Refresh()));
                            deviceList.Items.Add(device);
                            Log.Status("Loaded device: {0} [{1}]", device.ToString(), device.State);
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Error(e.Message);
                        Log.Status("Please remove the directory [ {0} ] manually", dev.BasePath);
                        Settings.Devices.Remove(dev);
                        Settings.Save();
                        Process.Start(dev.BasePath);
                    }
                }
                else
                {
                    try
                    {
                        DevicePackage.Remove(dev);
                        Log.Status("Removed device: {0}", dev.DeviceType);
                    }
                    catch (Exception e)
                    {
                        Log.Error(e.Message);
                    }
                }
            }
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

                if (package.Install())
                {
                    try
                    {
                        deviceList.Items.Add(package.Device);

                        if (package.Loader.State == DeviceState.RUNNING)
                        {
                            package.Device.Run();
                            Log.Status("Device [ {0} ] is started", package.Device.ToString());
                        }
                        package.Device.OnPropertyChanged += (p) => BeginInvoke((Action)(() => propertyGrid.Refresh()));

                        Settings.Devices.Add(package.Loader);
                        Settings.Save();
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex.Message);
                    }
                }
            }
        }

        private void RemoveDeviceMenuItem_Click(object sender, EventArgs e)
        {
            if (deviceList.SelectedItem is IHostedDevice device)
            {
                if (device.State == DeviceState.RUNNING)
                {
                    device.Stop();
                }
                deviceList.Items.Remove(device);
                var loader = Settings.Devices.Find((l) => l.ID == device.ID);
                loader.RemoveAtStart = true;
                Settings.Save();

                if (MessageBox.Show("Please restart the application to complete the removal of the device.", "Restart application", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    CloseToTray = false;
                    Application.Exit();
                }
            }
            else
            {
                Log.Debug("No device selected");
            }
        }

        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Show();
            this.WindowState = FormWindowState.Normal;
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CloseToTray = false;
            Application.Exit();
        }

        private void NotifyIcon_DoubleClick(object sender, EventArgs e)
        {
            Show();
            this.WindowState = FormWindowState.Normal;
        }

        private void HostingForm_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                if (MinimizeToTray)
                {
                    Hide();
                }
            }
        }

        private void HostingForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (CloseToTray)
            {
                if (e.CloseReason == CloseReason.UserClosing)
                {
                    e.Cancel = true;
                    Hide();
                }
            }
        }

        private void Run_Click(object sender, EventArgs e)
        {
            if (deviceList.SelectedItem is IHostedDevice device)
            {
                if (device.State == DeviceState.STOPPED)
                {
                    device.Run();
                    UpdateDeviceState(device);
                    Log.Status("Device {0} started", device.ToString());
                }
            }
        }

        private void Stop_Click(object sender, EventArgs e)
        {
            if (deviceList.SelectedItem is IHostedDevice device)
            {
                if (device.State == DeviceState.RUNNING)
                {
                    device.Stop();
                    UpdateDeviceState(device);
                    Log.Status("Device {0} stopped", device.ToString());
                }
            }

        }


        private void DeviceList_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateDevice();
        }

        private void UpdateDevice()
        {
            if (deviceList.SelectedItem is IHostedDevice device)
            {
                propertyGrid.SelectedObject = device;
                UpdateDeviceState(device);
            }
            else
            {
                UpdateDeviceState(null);
            }
        }

        private void UpdateDeviceState(IHostedDevice device)
        {
            if (device is object)
            {
                var loader = Settings.Devices.Find((l) => l.ID == device.ID);

                if (loader.State != device.State)
                {
                    loader.State = device.State;
                    Settings.Save();
                }

                runMenuItem.Enabled = runButton.Enabled = device.State == DeviceState.STOPPED;
                stopMenuItem.Enabled = stopButton.Enabled = device.State == DeviceState.RUNNING;
                removeDeviceMenuItem.Enabled = true;
                deviceStatus.Text = String.Format("{0} ({1}) [{2}]", device.ToString(), device.Port, device.State);
            }
            else
            {
                runMenuItem.Enabled = runButton.Enabled = false;
                stopMenuItem.Enabled = stopButton.Enabled = false;
                removeDeviceMenuItem.Enabled = false;
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

        private void AboutMenuItem_Click(object sender, EventArgs e)
        {
            AboutDialog dialog = new AboutDialog()
            {
                Text = String.Format("About {0}", Product),
                Product = Product,
                Version = Version,
                Description = Description,
                Image = AboutImage,
            };

            dialog.ShowDialog();

        }
    }
}
