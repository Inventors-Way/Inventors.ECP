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
        private CommunicationLayer commLayer = null;
        private readonly Profiler profiler = new Profiler();
        private AppState state = AppState.APP_STATE_UNINITIALIZED;

        public MainWindow()
        {
            InitializeComponent();
            SetupLogging();
            SetupPorts();
            UpdateAppStates(AppState.APP_STATE_UNINITIALIZED);
            UpdateStatus();
            SetTitle();
            UpdateProfiling();
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

        private void SetupPorts()
        {
            var names = SerialPort.GetPortNames();            
            commLayer = new SerialPortLayer()
            {
                BaudRate = 115200
            };

            for (int n = 0; n < names.Length; ++n)
            {
                var item = new ToolStripMenuItem(names[n]);
                portMenuItem.DropDownItems.Add(item);

                if (n == 0)
                {
                    commLayer.Port = names[n];
                    Log.Status("Serial port: {0}", commLayer.Port);
                    item.Checked = true;
                }
            }
        }

        private void PortMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            Log.Debug("Port changed to: {0}", e.ClickedItem.Text);
            commLayer.Port = e.ClickedItem.Text;

            foreach (var item in portMenuItem.DropDownItems)
            {
                ((ToolStripMenuItem)item).Checked = item != e.ClickedItem ? false : true;
            }
        }

        private void SetPort(DeviceLoader loader)
        {
            if (loader.PortName != null)
            {
                ToolStripMenuItem selected = null;

                foreach (var item in portMenuItem.DropDownItems)
                {
                    if (item is ToolStripMenuItem)
                    {
                        var tsItem = item as ToolStripMenuItem;

                        if (tsItem.Text == loader.PortName)
                        {
                            selected = tsItem;
                            device.CommLayer.Port = selected.Text;
                        }
                    }
                }

                if (selected != null)
                {
                    Log.Status("SELECTED PORT [ {0} ]", device.CommLayer.Port);

                    foreach (var item in portMenuItem.DropDownItems)
                    {
                        ((ToolStripMenuItem)item).Checked = item != selected ? false : true;
                    }
                }
                else
                {
                    Log.Status("DEFAULT PORT [ {0} ] not found, keeping port [ {1} ]", loader.PortName, commLayer.Port);
                }
            }
        }

        private void UpdateStatus()
        {
            if (device is object)
            {
                var report = device.CommLayer.GetStatistics();
                statusText.Text = String.Format("DATA [Rx: {0}, Tx: {1}]",
                    CommunicationLayer.Statistics.FormatRate(report.RxRate),
                    CommunicationLayer.Statistics.FormatRate(report.TxRate));
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

        private void LoadDevice(string fileName)
        {
            var loader = DeviceLoader.Load(fileName);
            Log.Status("Device: {0}", loader.AssemblyName);
            device = loader.Create();
            device.OnStateChanged += (sender, message) =>
            {
                if (functionList.SelectedItem is DeviceState)
                {
                    BeginUpdate(() => { propertyGrid.Refresh(); });
                }
            };

            profiler.Device = device;
            profiler.Profiling = loader.Profiling;
            profiler.Trials = loader.Trials;
            profiler.TestDelay = loader.TestDelay;
            Log.Status("Profiler: {0} (Test Trials: {1}, Test Delay: {2})", 
                loader.Profiling ? "ENABLED" : "DISABLED", 
                profiler.Trials,
                profiler.TestDelay);
            UpdateProfiling();
            InitializeFunctions();
            SetPort(loader);
            UpdateAppStates(AppState.APP_STATE_INITIALIZED);
        }

        private void InitializeFunctions()
        {
            functionList.Items.Clear();
            functionList.Items.Add(device);
            functionList.Items.Add(device.State);
            device.Functions.ForEach((f) => functionList.Items.Add(f));
            device.OnPrintf += OnPrintf;
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

        private void FunctionList_DoubleClick(object sender, EventArgs e)
        {
            if (state == AppState.APP_STATE_CONNECTED)
            {
                if (functionList.SelectedItem is Function)
                {
                    try
                    {
                        functionList.Enabled = false;
                        testToolStripMenuItem.Enabled = false;
                        UpdateAppStates(AppState.APP_STATE_ACTIVE);
                        Execute(functionList.SelectedItem as Function, true);
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
                Log.Status("Please connect first to a device");
        }

        private async void ConnectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (portMenuItem.DropDownItems.Count > 0)
            {
                try
                {
                    await device.ConnectAsync();
                    Log.Status("Device Connected: {0} [{1}]", device.ToString(), commLayer.Port);
                    UpdateAppStates(AppState.APP_STATE_CONNECTED);
                }
                catch (Exception ex)
                {
                    Log.Error("Problem connecting to device: " + ex.Message);
                    UpdateAppStates(AppState.APP_STATE_INITIALIZED);
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
                await device.DisconnectAsync();
                UpdateAppStates(AppState.APP_STATE_INITIALIZED);
                Log.Status("Device disconnected: {0} [{1}]", device.ToString(), commLayer.Port);
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

        private delegate void Updater();

        private void BeginUpdate(Updater updater)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(updater);
            }
            else
            {
                updater();
            }
        }

        #endregion
        #region Functions and message handling
        private async void Execute(Function function, bool doLogging = false)
        {
            if (device != null)
            {
                try
                {
                    await device.ExecuteAsync(function);

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
      
        public void OnPrintf(object sender, MessageEventArgs<PrintfMessage> e) => Log.Status("Printf: {0}", e.Message.ToString());

        #endregion

        private async void TestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (state == AppState.APP_STATE_CONNECTED)
            {
                profiler.Function = functionList.SelectedItem as Function;

                if (profiler.Function != null)
                {
                    functionList.Enabled = false;
                    testToolStripMenuItem.Enabled = false;
                    UpdateAppStates(AppState.APP_STATE_ACTIVE);

                    await profiler.TestAsync();

                    Log.Status("TEST COMPLETED [ {0}ms ]", profiler.RunTime);
                    Log.Status(profiler.Compile().ToString());
                    UpdateAppStates(AppState.APP_STATE_CONNECTED);
                    UpdateProfiling();
                }
                else
                    Log.Status("Please select a function");
            }
            else
                Log.Status("Please connect first to a device");
        }

        private void TrialsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var dialog = new TrialsDialog())
            {
                dialog.Trials = profiler.Trials;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    profiler.Trials = dialog.Trials;
                    Log.Debug("Number of test trials set to {0}", profiler.Trials);
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
            Process.Start(String.Format("http://inventors.dk/ecp_tester_about_{0}.html", VersionInformation));
        }

        private void EnabledToolStripMenuItem_Click(object sender, EventArgs e)
        {
            profiler.Profiling = true;
            UpdateProfiling();
        }

        private void DisabledToolStripMenuItem_Click(object sender, EventArgs e)
        {
            profiler.Profiling = false;
            UpdateProfiling();
        }

        private void ReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var profile = profiler.GetProfile();
            Log.Status(Profiler.CreateProfileReport(profile));
        }

        private void UpdateProfiling()
        {
            enabledToolStripMenuItem.Checked = profiler.Profiling;
            disabledToolStripMenuItem.Checked = !profiler.Profiling;
        }
    }
}
