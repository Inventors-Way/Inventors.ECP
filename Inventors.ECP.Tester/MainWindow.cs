using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Inventors.ECP;
using System.IO.Ports;
using Inventors.Logging;
using Inventors.ECP.Messages;
using Inventors.ECP.Functions;
using System.Diagnostics;
using System.Threading;
using System.IO;
using System.Reflection;
using Inventors.ECP.Profiling;
using Inventors.ECP.Communication;

namespace Inventors.ECP.Tester
{
    public partial class MainWindow : 
        Form
    {
        private enum AppState
        {
            APP_STATE_UNINITIALIZED,
            APP_STATE_INITIALIZED,
            APP_STATE_CONNECTED,
            APP_STATE_ACTIVE
        }

        private Logger logger;
        private Device device = null;
        private DeviceType selectedDevice = null;
        private AppState state = AppState.APP_STATE_UNINITIALIZED;

        public MainWindow()
        {
            InitializeComponent();
            SetupLogging();
            UpdateAppStates(AppState.APP_STATE_UNINITIALIZED);
            UpdateStatus();
            SetTitle();
            UpdateProfiling();
            SetLoggingLevel(Settings.Level);
        }

        public MainWindow(string deviceFileName) :
            this()
        {
            LoadDevice(deviceFileName);
            SetTitle();
        }

        private void SetTitle()
        {
            Text = String.Format("ECP Tester, Rev {0}", VersionInformation);
        }

        public static string VersionInformation
        {
            get
            {
                return Assembly.GetAssembly(typeof(MainWindow)).GetName().Version.ToString();
            }
        }

        private void SetupLogging()
        {
            logger = new Logger() { Box = logBox };
            Log.SetLogger(logger);
            Log.Level = LogLevel.DEBUG;
        }

        private void UpdateStatus()
        {
            if (device is object)
            {
                var report = device.GetStatistics();
                statusText.Text = String.Format("DATA [Rx: {0}, Tx: {1}]",
                    Statistics.FormatRate(report.RxRate),
                    Statistics.FormatRate(report.TxRate));
            }
        }

        private void MsgTimer_Tick(object sender, EventArgs e)
        {
            if (device != null)
            {
                UpdateStatus();
            }
        }

        #region GUI HANDLING
        private void OpenDeviceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog()
            {
                Filter = "Device Definition Files (*.ddfx)|*.ddfx",
                Title = "Open Device Definition File"
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    LoadDevice(dialog.FileName);
                }
                catch (Exception ex)
                {
                    Log.Error(ex.Message);
                }

            }
        }

        private async void LoadDevice(string fileName)
        {
            try
            {
                var loader = DeviceLoader.Load(fileName);
                Log.Status("Device: {0}", loader.AssemblyName);
                device = loader.Create();

                device.Profiler.Profiling = loader.Profiling;
                device.Profiler.Trials = loader.Trials;
                device.Profiler.TestDelay = loader.TestDelay;
                Log.Status("Profiler: {0} (Test Trials: {1}, Test Delay: {2})",
                    loader.Profiling ? "ENABLED" : "DISABLED",
                    device.Profiler.Trials,
                    device.Profiler.TestDelay);
                UpdateProfiling();
                InitializeFunctions();
                await UpdatePorts();
                UpdateAppStates(AppState.APP_STATE_INITIALIZED);
            }
            catch (Exception e)
            {
                UpdateAppStates(AppState.APP_STATE_INITIALIZED);
                Log.Error(e.Message);
                MessageBox.Show(e.Message, "Error loading device");
            }
        }

        private bool CheckDevicesChanged(List<DeviceType> devices)
        {
            if (devices.Count != portMenuItem.DropDownItems.Count)
                return true;

            if (devices.Count == portMenuItem.DropDownItems.Count)
            {
                for (int n = 0; n < devices.Count; ++n)
                {
                    if (portMenuItem.DropDownItems[n].Tag is DeviceType menuDevice)
                    {
                        if (menuDevice.ToString() != devices[n].ToString())
                        {
                            return true;
                        }
                    }
                    else
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private async Task UpdatePorts()
        {
            if (!device.IsOpen)
            {
                var timeout = device.Timeout;
                device.Timeout = 100;
                var devices = await device.GetAvailableDevicesAsync().ConfigureAwait(true);
                device.Timeout = timeout;

                if (CheckDevicesChanged(devices))
                {
                    Log.Debug("Available devices changed:");
                    portMenuItem.DropDownItems.Clear();

                    for (int n = 0; n < devices.Count; ++n)
                    {
                        var item = new ToolStripMenuItem(devices[n].ToString()) { Tag = devices[n] };
                        portMenuItem.DropDownItems.Add(item);
                        Log.Debug("  [ {0} ] Device: {1}", n, devices[n].ToString());
                        if (selectedDevice is object)
                        {
                            if (selectedDevice.ToString() == devices[n].ToString())
                            {
                                selectedDevice = devices[n];
                                item.Checked = true;
                            }
                        }
                        else
                        {
                            if (n == 0)
                            {
                                selectedDevice = devices[n];
                                item.Checked = true;
                            }
                        }
                    }
                }
            }
        }

        private async void DeviceTimer_Tick(object sender, EventArgs e)
        {
            if (device is object)
            {
                if (!device.IsOpen)
                {
                    Log.Debug("Updating ports");
                    await UpdatePorts();                    
                }
            }
        }

        private void PortMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (device is object)
            {
                if (!device.IsOpen)
                {
                    if (e.ClickedItem.Tag is DeviceType current)
                    {
                        Log.Debug("Port changed to: {0}", e.ClickedItem.Text);
                        device.Port = current.Port;
                        selectedDevice = current;

                        foreach (var item in portMenuItem.DropDownItems)
                        {
                            ((ToolStripMenuItem)item).Checked = item != e.ClickedItem ? false : true;
                        }
                    }
                    else
                    {
                        Log.Error("Clicked item did not have a DeviceType as its tag");
                    }
                }
                else
                {
                    Log.Error("Attempted to change device while the device is open");
                }
            }
        }

        private void InitializeFunctions()
        {
            functionList.Items.Clear();
            functionList.Items.Add(device);
            device.Functions.ForEach((f) => functionList.Items.Add(f));
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void FunctionList_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateFunction();
        }

        private void UpdateFunction()
        {
            if (functionList.SelectedIndex >= 0)
            {
                propertyGrid.SelectedObject = functionList.SelectedItem;
            }
        }

        private async void FunctionList_DoubleClick(object sender, EventArgs e)
        {
            if (state == AppState.APP_STATE_CONNECTED)
            {
                if (functionList.SelectedItem is DeviceFunction)
                {
                    try
                    {
                        functionList.Enabled = false;
                        testToolStripMenuItem.Enabled = false;
                        UpdateAppStates(AppState.APP_STATE_ACTIVE);
                        await Execute(functionList.SelectedItem as DeviceFunction, true);
                    }
                    catch { }

                    functionList.Enabled = true;
                    testToolStripMenuItem.Enabled = true;
                    UpdateAppStates(AppState.APP_STATE_CONNECTED);
                }
                else
                {
                    Log.Debug("Selected item is not a function");
                }
            }
            else
            {
                Log.Status("Please connect first to a device");
            }
        }

        private async void ConnectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selectedDevice is object)
            {
                try
                {
                    deviceTimer.Enabled = false;
                    device.Port = selectedDevice.Port;
                    await device.ConnectAsync().ConfigureAwait(true);
                    Log.Status("Device Connected: {0} [{1}]", device.ToString(), device.Port);
                    UpdateAppStates(AppState.APP_STATE_CONNECTED);
                }
                catch (Exception ex)
                {
                    Log.Error("Problem connecting to device: " + ex.Message);
                    UpdateAppStates(AppState.APP_STATE_INITIALIZED);
                    deviceTimer.Enabled = true;
                }
            }
            else
            {
                Log.Status("No ports available");
            }
        }

        private async void DisconnectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                await device.DisconnectAsync().ConfigureAwait(true);
                UpdateAppStates(AppState.APP_STATE_INITIALIZED);
                Log.Status("Device disconnected: {0} [{1}]", device.ToString(), device.Port);
                deviceTimer.Enabled = true;
            }
            catch (Exception ex)
            {
                Log.Error("Problem disconnecting from device: " + ex.Message);
                UpdateAppStates(AppState.APP_STATE_INITIALIZED);
            }
        }

        private void UpdateAppStates(AppState newState)
        {
            state = newState;

            switch (state)
            {
                case AppState.APP_STATE_UNINITIALIZED:
                    functionList.Enabled = false;
                    connectToolStripMenuItem.Enabled = false;
                    disconnectToolStripMenuItem.Enabled = false;
                    openDeviceToolStripMenuItem.Enabled = true;
                    portMenuItem.Enabled = true;
                    testToolStripMenuItem.Enabled = false;
                    trialsToolStripMenuItem.Enabled = true;
                    break;
                case AppState.APP_STATE_INITIALIZED:
                    functionList.Enabled = true;
                    connectToolStripMenuItem.Enabled = true;
                    disconnectToolStripMenuItem.Enabled = false;
                    openDeviceToolStripMenuItem.Enabled = false;
                    portMenuItem.Enabled = true;
                    testToolStripMenuItem.Enabled = false;
                    trialsToolStripMenuItem.Enabled = true;
                    break;
                case AppState.APP_STATE_CONNECTED:
                    functionList.Enabled = true;
                    connectToolStripMenuItem.Enabled = false;
                    disconnectToolStripMenuItem.Enabled = true;
                    openDeviceToolStripMenuItem.Enabled = false;
                    portMenuItem.Enabled = false;
                    testToolStripMenuItem.Enabled = true;
                    trialsToolStripMenuItem.Enabled = true;
                    break;
                case AppState.APP_STATE_ACTIVE:
                    functionList.Enabled = false;
                    connectToolStripMenuItem.Enabled = false;
                    disconnectToolStripMenuItem.Enabled = false;
                    openDeviceToolStripMenuItem.Enabled = false;
                    portMenuItem.Enabled = false;
                    testToolStripMenuItem.Enabled = false;
                    trialsToolStripMenuItem.Enabled = false;
                    break;
            }
        }

        private void PropertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            propertyGrid.Refresh();
        }

        #endregion
        #region Functions and message handling
        private async Task Execute(DeviceFunction function, bool doLogging = false)
        {
            if (device != null)
            {
                try
                {
                    await device.ExecuteAsync(function).ConfigureAwait(true);

                    if (doLogging)
                    {
                        Log.Status(String.Format("Complete [{0}] ({1}ms)", function.ToString(), function.TransmissionTime));

                        if (function is DeviceIdentification)
                        {
                            DeviceIdentification identification = function as DeviceIdentification;
                            if (!device.IsCompatible(identification))
                            {
                                Log.Error("Incompatible device: {0} [{1}:{2}]",
                                    identification.Device,
                                    identification.ManufactureID,
                                    identification.DeviceID);
                            }
                            else
                            {
                                Log.Status("Compatible device: {0} [{1}:{2}]",
                                    identification.Device,
                                    identification.ManufactureID,
                                    identification.DeviceID);
                            }
                        }
                    }

                    propertyGrid.Refresh();
                }
                catch (Exception e)
                {
                    if (doLogging)
                    {
                        Log.Error("EXCEPTION:" + function.ToString() + " [" + e.Message + " ] ");
                    }

                    throw;
                }
            }
        }      

        #endregion

        private async void TestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (state == AppState.APP_STATE_CONNECTED)
            {
                device.Profiler.Function = functionList.SelectedItem as DeviceFunction;

                if (device.Profiler.Function != null)
                {
                    UpdateAppStates(AppState.APP_STATE_ACTIVE);
                    var report = await device.Profiler.TestAsync().ConfigureAwait(true);
                    Log.Status(report.ToString());
                    UpdateAppStates(AppState.APP_STATE_CONNECTED);
                    UpdateProfiling();
                }
                else
                {
                    Log.Status("Please select a function");
                }
            }
            else
            {
                Log.Status("Please connect first to a device");
            }
        }

        private void TrialsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var dialog = new TrialsDialog())
            {
                dialog.Trials = device.Profiler.Trials;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    device.Profiler.Trials = dialog.Trials;
                    Log.Debug("Number of test trials set to {0}", device.Profiler.Trials);
                }
            }
        }

        private void ClearLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            logBox.Text = "";
        }

        private void SaveLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var dialog = new SaveFileDialog()
            {
                Filter = "Text Files|*.txt",
                Title = "Save Log File"
            })
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllText(dialog.FileName, logBox.Text);
                }
            }
        }

        private void DocumentationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/Inventors-Way/Inventors.ECP");
        }

        private void AboutECPTesterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var dialog = new AboutDialog()
            {
                Text = "About ECP Tester",
                Product = "ECP Tester",
                Version = "1.3.1",
                Description = "Copyright (C) 2019-2020 Inventors' Way ApS. All rights reserved." + Environment.NewLine + "ECP Tester is made possible by the open source ECP project made by the scientists and engineers at Inventors' Way.",
                Image = Image.FromFile("about.png")
            };
            dialog.ShowDialog();
        }

        private void EnabledToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (device is object)
            {
                device.Profiler.Profiling = true;
                UpdateProfiling();
            }
        }

        private void DisabledToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (device is object)
            {
                device.Profiler.Profiling = false;
                UpdateProfiling();
            }
        }

        private void ReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (device is object)
            {
                var profile = device.Profiler.GetProfile();
                Log.Status(Profiler.CreateProfileReport(profile));
            }
        }

        private void UpdateProfiling()
        {
            if (device is object)
            {
                enabledToolStripMenuItem.Checked = device.Profiler.Profiling;
                disabledToolStripMenuItem.Checked = !device.Profiler.Profiling;
            }
            else
            {
                enabledToolStripMenuItem.Enabled = disabledToolStripMenuItem.Enabled = false;
            }
        }

        private void DebugToolStripMenuItem_Click(object sender, EventArgs e) => SetLoggingLevel(LogLevel.DEBUG);

        private void StatusToolStripMenuItem_Click(object sender, EventArgs e) => SetLoggingLevel(LogLevel.STATUS);

        private void ErrorToolStripMenuItem_Click(object sender, EventArgs e) => SetLoggingLevel(LogLevel.ERROR);

        private void SetLoggingLevel(LogLevel level)
        {
            debugToolStripMenuItem.Checked = level == LogLevel.DEBUG;
            statusToolStripMenuItem.Checked = level == LogLevel.STATUS;
            errorToolStripMenuItem.Checked = level == LogLevel.ERROR;
            Log.Level = level;
            Settings.Level = level;
        }
    }
}
