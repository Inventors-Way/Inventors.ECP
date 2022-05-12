﻿using System;
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
using Inventors.ECP.Messages;
using Inventors.ECP.Functions;
using System.Diagnostics;
using System.Threading;
using System.IO;
using System.Reflection;
using Inventors.ECP.Profiling;
using Inventors.ECP.TestFramework.Profiling;
using Inventors.ECP.TestFramework.Monitoring;
using Inventors.ECP.Monitor;

namespace Inventors.ECP.TestFramework
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

        private delegate void InvokeDelegate();

        private Device device = null;
        private string deviceId = null;
        private string logDirectory = null;
        private string selectedDevice = null;
        private bool confirmLogDeletion = true;
        private AppState state = AppState.APP_STATE_UNINITIALIZED;
        private readonly ProfilerWindow profilerWindow;
        private readonly MonitorWindow monitorWindow;
        private readonly CommTester commTester;

        public Image AboutImage { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            SetupLogging();
            UpdateAppStates(AppState.APP_STATE_UNINITIALIZED);
            UpdateStatus();
            SetTitle();
            UpdateProfiling();
            SetLoggingLevel(Settings.Level);

            profilerWindow = new ProfilerWindow();
            profilerWindow.OnProfilerClosed += ProfilerWindow_OnClosed;
            monitorWindow = new MonitorWindow();
            monitorWindow.OnMonitorClosed += PortMonitorWindow_OnClosed;
            PortMonitor.SetMonitor(monitorWindow);

            commTester = new CommTester();
        }
        
        public MainWindow(string deviceFileName) :
            this()
        {
            LoadDevice(deviceFileName);
            SetTitle();
        }

        private void SetTitle()
        {
            Text = String.Format("ECP Tester, Rev {0}", VersionInfo.VersionDescription);
        }

        private void SetupLogging()
        {
            EcpLog.SetLogger(logControl);
            EcpLog.Level = LogLevel.DEBUG;
        }

        private void UpdateStatus()
        {
            if (device is object)
            {
                statusText.Text = String.Format("DATA [Rx: {0}, Tx: {1}]",
                    Statistics.FormatRate(device.Central.RxRate),
                    Statistics.FormatRate(device.Central.TxRate));
            }
        }

        private void MsgTimer_Tick(object sender, EventArgs e)
        {
            if (device != null)
            {
                UpdateStatus();
                device.Central.RestartStatistics();
            }
        }

        #region GUI HANDLING
        private void OpenDeviceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog()
            {
                Filter = "Device Definition Files (*.ddfx)|*.ddfx",
                Title = "Open Device Definition File",
                InitialDirectory = Settings.DeviceDirectory
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    LoadDevice(dialog.FileName);
                    var directory = Path.GetDirectoryName(dialog.FileName);
                    Settings.DeviceDirectory = directory;
                }
                catch (Exception ex)
                {
                    EcpLog.Error(ex.Message);
                }
            }
        }

        private void LoadDevice(string fileName)
        {
            try
            {
                var loader = DeviceLoader.Load(fileName);
                deviceId = loader.Factory;
                logDirectory = Settings.GetDeviceDefaultLoggingDirectory(deviceId);
                logControl.Paused = false;
                EcpLog.Status("Loaded assembly: {0}", loader.FileName);
                EcpLog.Status("Device: {0} [Creation time: {1}]", loader.Factory, loader.CreationTime);
                EcpLog.Status("Logging directory: {0}", logDirectory);
                EcpLog.Status("Log settings [Auto save: {0}, Confirm deletion: {1}]", loader.AutoSaveLog, loader.ConfirmLogDeletion);

                autoSaveLogToolStripMenuItem.Checked = logControl.AutoSave = loader.AutoSaveLog;
                logControl.InitializeLogFile(logDirectory);
                confirmLogDeletion = loader.ConfirmLogDeletion;

                device = loader.Create();

                device.Profiler.Enabled = loader.Profiling;
                profilerWindow.SetDevice(device);
                commTester.Trials = loader.Trials;
                commTester.TestDelay = loader.TestDelay;
                commTester.Master = device.Central;

                EcpLog.Status("Profiler: {0} (Test Trials: {1}, Test Delay: {2})",
                    loader.Profiling ? "ENABLED" : "DISABLED",
                    commTester.Trials,
                    commTester.TestDelay);

                if (device.AvailableAddress is object)
                {
                    foreach (var address in device.AvailableAddress)
                    {
                        var menuItem = new ToolStripMenuItem(address.Name)
                        {
                            Tag = address
                        };
                        menuItem.Click += (sender, e) =>
                        {
                            if (sender is ToolStripMenuItem item)
                            {
                                if (item.Tag is DeviceAddress current)
                                {
                                    device.CurrentAddress = current;
                                    EcpLog.Status($"CURRENT ADDRESS: {current.Name} [ {current.Value} ]");
                                    UpdateAddressMenu();
                                }
                            }
                        };

                        addressMenu.DropDownItems.Add(menuItem);
                    };

                    device.CurrentAddress = device.AvailableAddress[0];
                    UpdateAddressMenu();
                }

                UpdateProfiling();
                InitializeFunctions();
                UpdatePorts();
                UpdateAppStates(AppState.APP_STATE_INITIALIZED);
            }
            catch (Exception e)
            {
                UpdateAppStates(AppState.APP_STATE_UNINITIALIZED);
                EcpLog.Error(e.Message);
                MessageBox.Show(e.Message, "Error loading device");
            }
        }

        private void UpdateAddressMenu()
        {
            foreach (var iterator in addressMenu.DropDownItems)
            {
                if (iterator is ToolStripMenuItem item)
                {
                    if (device.CurrentAddress is DeviceAddress current)
                    {
                        if (item.Tag is DeviceAddress address)
                        {
                            var state = current.Value == address.Value;
                            item.Checked = state;

                        }
                        else
                            item.Checked = false;
                    }
                    else
                        item.Checked = false;
                }
            }
        }

        private bool CheckDevicesChanged(List<string> locations)
        {
            if (locations.Count != portMenuItem.DropDownItems.Count)
                return true;

            if (locations.Count == portMenuItem.DropDownItems.Count)
            {
                for (int n = 0; n < locations.Count; ++n)
                {
                    if (portMenuItem.DropDownItems[n].Tag is string menuDevice)
                    {
                        if (menuDevice.ToString() != locations[n].ToString())
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

        private void UpdatePorts()
        {
            if (!device.IsOpen)
            {
                var devices = device.GetLocationsDevices(); 

                if (CheckDevicesChanged(devices))
                {
                    EcpLog.Debug("Available devices changed:");
                    portMenuItem.DropDownItems.Clear();

                    for (int n = 0; n < devices.Count; ++n)
                    {
                        var location = devices[n];
                        var item = new ToolStripMenuItem(location) 
                        { 
                            Tag = devices[n] 
                        };
                        portMenuItem.DropDownItems.Add(item);

                        EcpLog.Debug("  [ {0} ] Device: {1}", n, location);
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

        private void DeviceTimer_Tick(object sender, EventArgs e)
        {
            if (device is object)
            {
                if (!device.IsOpen)
                {
                    UpdatePorts();
                }
                else if (device.IsOpen)
                {
                    if (device.PingEnabled)
                    {
                        try
                        {
                            var ping = device.Ping();

                            if (ping < 0)
                            {
                                EcpLog.Error("Ping failed!");
                                device.Profiler.Add(new TargetEvent("Ping failed"));
                            }
                            else
                            {
                                pingStatus.Text = String.Format($"| Ping count: { ping }");
                            }
                        }
                        catch
                        {
                            EcpLog.Error("Ping failed!");
                            device.Profiler.Add(new TargetEvent("Ping failed"));
                        }
                    }

                    if (!(functionList.SelectedItem is DeviceFunction))
                    {
                        propertyGrid.Refresh();
                    }
                }
            }
        }

        private void PortMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (device is object)
            {
                if (!device.IsOpen)
                {
                    if (e.ClickedItem.Tag is string current)
                    {
                        EcpLog.Debug("Port changed to: {0}", e.ClickedItem.Text);
                        device.Location = current;
                        selectedDevice = current;

                        foreach (var item in portMenuItem.DropDownItems)
                        {
                            ((ToolStripMenuItem)item).Checked = item == e.ClickedItem;
                        }
                    }
                    else
                    {
                        EcpLog.Error("Clicked item did not have a DeviceType as its tag");
                    }
                }
                else
                {
                    EcpLog.Error("Attempted to change device while the device is open");
                }
            }
        }

        private void InitializeFunctions()
        {
            functionList.Items.Clear();
            functionList.Items.Add(device);

            foreach (var function in device.Functions)
            {
                functionList.Items.Add(function);
            }
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e) => Application.Exit();

        private void FunctionList_SelectedIndexChanged(object sender, EventArgs e) => UpdateFunction();

        private void UpdateFunction()
        {
            if (functionList.SelectedIndex >= 0)
            {
                propertyGrid.SelectedObject = functionList.SelectedItem;
            }
        }

        private void FunctionList_DoubleClick(object sender, EventArgs e)
        {
            if (state == AppState.APP_STATE_CONNECTED)
            {
                if (functionList.SelectedItem is DeviceFunction function)
                {
                    try
                    {
                        functionList.Enabled = false;
                        testToolStripMenuItem.Enabled = false;
                        UpdateAppStates(AppState.APP_STATE_ACTIVE);
                        Execute(function, true);
                    }
                    catch (FunctionNotAcknowledgedException fnae)
                    {
                        EcpLog.Status($"NACK: {device.GetErrorString(fnae.ErrorCode)}");
                    }
                    catch (PeripheralNotRespondingException snre)
                    {
                        EcpLog.Status($"Slave not responding: {snre.Message}");
                    }
                    catch (PacketFormatException pfe)
                    {
                        EcpLog.Status($"Invalid packet format: {pfe.Message}");
                    }
                    catch (Exception ex)
                    {
                        EcpLog.Status($"Exception [ {ex.GetType()} ]: {ex.Message}");
                    }

                    functionList.Enabled = true;
                    testToolStripMenuItem.Enabled = true;
                    UpdateAppStates(AppState.APP_STATE_CONNECTED);
                }
                else
                {
                    EcpLog.Debug("Selected item is not a function");
                }
            }
            else
            {
                EcpLog.Status("Please connect first to a device");
            }
        }

        private void ConnectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selectedDevice is object)
            {
                try
                {
                    device.Location = selectedDevice;
                    device.Open();
                    EcpLog.Status($"Location opened: {device.Location}");
                    device.Profiler.Add(new TargetEvent($"Location opened: {device.Location}"));
                    UpdateAppStates(AppState.APP_STATE_CONNECTED);
                }
                catch (Exception ex)
                {
                    EcpLog.Error("Problem connecting to device: " + ex.Message);
                    UpdateAppStates(AppState.APP_STATE_INITIALIZED);
                }
            }
            else
            {
                EcpLog.Status("No ports available");
            }
        }

        private void DisconnectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                device.Close();
                UpdateAppStates(AppState.APP_STATE_INITIALIZED);
                EcpLog.Status("Location closed: {0}", device.Location);
                device.Profiler.Add(new TargetEvent("Connection closed"));
            }
            catch (Exception ex)
            {
                EcpLog.Error("Problem closing location: " + ex.Message);
                UpdateAppStates(AppState.APP_STATE_INITIALIZED);
            }
        }

        private void UpdateAppStates(AppState newState)
        {
            state = newState;

            switch (state)
            {
                case AppState.APP_STATE_UNINITIALIZED:
                    clearLogToolStripMenuItem.Enabled = false;
                    saveLogToolStripMenuItem.Enabled = false;
                    pauseToolStripMenuItem.Enabled = false;
                    autoSaveLogToolStripMenuItem.Enabled = false;
                    openLogsToolStripMenuItem.Enabled = false;

                    functionList.Enabled = false;
                    connectToolStripMenuItem.Enabled = false;
                    disconnectToolStripMenuItem.Enabled = false;
                    openDeviceToolStripMenuItem.Enabled = true;
                    portMenuItem.Enabled = true;
                    testToolStripMenuItem.Enabled = false;
                    trialsToolStripMenuItem.Enabled = true;
                    break;
                case AppState.APP_STATE_INITIALIZED:
                    clearLogToolStripMenuItem.Enabled = true;
                    saveLogToolStripMenuItem.Enabled = true;
                    pauseToolStripMenuItem.Enabled = true;
                    autoSaveLogToolStripMenuItem.Enabled = true;
                    openLogsToolStripMenuItem.Enabled = true;

                    functionList.Enabled = true;
                    connectToolStripMenuItem.Enabled = true;
                    disconnectToolStripMenuItem.Enabled = false;
                    openDeviceToolStripMenuItem.Enabled = false;
                    portMenuItem.Enabled = true;
                    testToolStripMenuItem.Enabled = false;
                    trialsToolStripMenuItem.Enabled = true;
                    break;
                case AppState.APP_STATE_CONNECTED:
                    clearLogToolStripMenuItem.Enabled = true;
                    saveLogToolStripMenuItem.Enabled = true;
                    pauseToolStripMenuItem.Enabled = true;
                    autoSaveLogToolStripMenuItem.Enabled = true;
                    openLogsToolStripMenuItem.Enabled = true;

                    functionList.Enabled = true;
                    connectToolStripMenuItem.Enabled = false;
                    disconnectToolStripMenuItem.Enabled = true;
                    openDeviceToolStripMenuItem.Enabled = false;
                    portMenuItem.Enabled = false;
                    testToolStripMenuItem.Enabled = true;
                    trialsToolStripMenuItem.Enabled = true;
                    break;
                case AppState.APP_STATE_ACTIVE:
                    clearLogToolStripMenuItem.Enabled = true;
                    saveLogToolStripMenuItem.Enabled = true;
                    pauseToolStripMenuItem.Enabled = true;
                    autoSaveLogToolStripMenuItem.Enabled = true;
                    openLogsToolStripMenuItem.Enabled = true;

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
        private void Execute(DeviceFunction function, bool doLogging = false)
        {
            if (device != null)
            {
                try
                {
                    device.Execute(function);

                    if (doLogging)
                    {
                        EcpLog.Status(String.Format("Complete [{0}] ({1}ms)", function.ToString(), function.TransmissionTime));

                        if (function is DeviceIdentification identification)
                        {
                            if (!device.IsCompatible(identification))
                            {
                                EcpLog.Error("Incompatible device: {0} [{1}:{2}]",
                                    identification.Device,
                                    identification.ManufactureID,
                                    identification.DeviceID);
                            }
                            else
                            {
                                EcpLog.Status("Compatible device: {0} [{1}:{2}]",
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
                        EcpLog.Error("EXCEPTION:" + function.ToString() + " [" + e.Message + " ] ");
                    }
                }
            }
        }

#endregion

        private async void TestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (state == AppState.APP_STATE_CONNECTED)
            {
                if (functionList.SelectedItem is DeviceFunction function)
                {
                    UpdateAppStates(AppState.APP_STATE_ACTIVE);
                    var report = await commTester.TestAsync(function).ConfigureAwait(true);
                    EcpLog.Status(report.ToString());
                    UpdateAppStates(AppState.APP_STATE_CONNECTED);
                    UpdateProfiling();
                }
                else
                {
                    EcpLog.Status("Please select a function");
                }
            }
            else
            {
                EcpLog.Status("Please connect first to a device");
            }
        }

        private void TrialsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var dialog = new TrialsDialog())
            {
                dialog.Trials = commTester.Trials;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    commTester.Trials = dialog.Trials;
                    EcpLog.Debug("Number of test trials set to {0}", commTester.Trials);
                }
            }
        }

        private void ClearLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(logControl.Content))
            {
                bool deleteLog = false;

                if (confirmLogDeletion)
                {
                    if (MessageBox.Show(this,
                                        "This will delete all content in the log, do you intend to do this",
                                        "Deleting Log",
                                        MessageBoxButtons.YesNo,
                                        MessageBoxIcon.Question,
                                        MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                    {
                        deleteLog = true;
                    }
                }
                else
                {
                    deleteLog = true;
                }

                if (deleteLog)
                {
                    logControl.Clear();
                }
            }
        }

        private void SaveLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(logControl.Content))
            {
                using (var dialog = new SaveFileDialog()
                {
                    Filter = "Text Files|*.txt",
                    Title = "Save Log File",
                    InitialDirectory = Settings.GetLoggingDirectory(deviceId)                    
                })
                {
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        File.WriteAllText(dialog.FileName, logControl.Content);
                        Settings.UpdateLoggingDirectory(deviceId, Path.GetDirectoryName(dialog.FileName));
                    }
                }
            }
        }

        private void AboutECPTesterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var dialog = new AboutDialog()
            {
                Text = "About ECP Tester",
                Product = "ECP Tester",
                Version = VersionInfo.VersionDescription,
                Description = "Copyright (C) 2009-2022 Inventors' Way ApS. All rights reserved." + Environment.NewLine + "ECP Tester is made possible by the open source ECP project made by the scientists and engineers at Inventors' Way.",
            };

            if (AboutImage is object)
            {
                dialog.Image = AboutImage;
            }
            else
            {
                dialog.Image = Resource.AboutImage;
            }

            dialog.ShowDialog();
        }

        private void EnabledToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (device is object)
            {
                device.Profiler.Enabled = true;
                UpdateProfiling();
            }
        }

        private void DisabledToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (device is object)
            {
                device.Profiler.Enabled = false;
                UpdateProfiling();
            }
        }

        private void UpdateProfiling()
        {
            if (device is object)
            {
                profilerMenuItem.Checked = device.Profiler.Enabled;

                if (device.Profiler.Enabled)
                {
                    profilerWindow.Show();
                }
                else
                {
                    profilerWindow.Visible = false;
                }
            }
        }

        private void ProfilerMenuItem_Click(object sender, EventArgs e)
        {
            if (device is object)
            {
                device.Profiler.Enabled = !device.Profiler.Enabled;
                UpdateProfiling();
            }
        }


        private void ProfilerWindow_OnClosed(object sender, bool close)
        {
            if (device is object)
            {
                device.Profiler.Enabled = false;
                UpdateProfiling();
                profilerWindow.Visible = false;
            }
        }

        private void UpdatePortMonitor()
        {
            if (device is object)
            {
                portMonitorToolStripMenuItem.Checked = PortMonitor.Enabled;

                if (PortMonitor.Enabled)
                {
                    monitorWindow.Show();
                }
                else
                {
                    monitorWindow.Visible = false;
                }
            }
        }

        private void PortMonitorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (device is object)
            {
                PortMonitor.Enabled = true;
                UpdatePortMonitor();
            }
        }

        private void PortMonitorWindow_OnClosed(object sender, bool close)
        {
            if (device is object)
            {
                PortMonitor.Enabled = false;
                UpdatePortMonitor();
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
            EcpLog.Level = level;
            Settings.Level = level;
        }

        private void PauseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            logControl.Paused = !logControl.Paused;
            pauseToolStripMenuItem.Checked = logControl.Paused;
        }

        private void AutoSaveLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            logControl.AutoSave = !logControl.AutoSave;
            autoSaveLogToolStripMenuItem.Checked = logControl.AutoSave;
        }

        private void OpenLogsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (logDirectory is string)
            {
                Process.Start(logDirectory);
            }
        }
    }
}
