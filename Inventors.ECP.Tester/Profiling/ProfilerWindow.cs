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
        public readonly Dictionary<int, ListBox> signalLists = new Dictionary<int, ListBox>();
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
            InitializeDebugSignals();
        }

        private void InitializeDebugSignals()
        {
            var debugSpecification = device.GetActiveDebugSpecification();

            debugSignalTabControl.TabPages.Clear();

            for (int n = 0; n < device.NumberOfSupportedDebugSignals; ++n)
            {
                var page = new TabPage($"{n+1}")
                {
                    BackColor = Color.White
                };
                var listBox = new ListBox()
                {
                    Dock = DockStyle.Fill,
                    BorderStyle = BorderStyle.None,
                    SelectionMode = SelectionMode.One,
                    BackColor = Color.White,
                    Tag = n
                };
                var menuItem = new ToolStripMenuItem($"Signal {n+1}")
                {
                    Tag = n,
                    ShortcutKeys = Keys.Control | (Keys.D1 + n)
                };
                menuItem.Click += MenuItem_Click;

                listBox.SelectedIndexChanged += ListBox_SelectedIndexChanged;

                foreach (var signal in debugSpecification.Signals)
                {
                    listBox.Items.Add(signal);
                }

                page.Controls.Add(listBox);
                debugSignalTabControl.TabPages.Add(page);
                signalLists.Add(n, listBox);
                debugSignalToolStripMenuItem.DropDownItems.Add(menuItem);
            }

            UpdateDebugSignalMenu();
        }

        private void MenuItem_Click(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem item)
            {
                if (item.Tag is int index)
                {
                    debugSignalTabControl.SelectedIndex = index;
                }

                UpdateDebugSignalMenu();
            }
        }

        private void ListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (sender is ListBox listBox)
            {
                if (listBox.SelectedIndex >= 0)
                {
                    if (listBox.Items[listBox.SelectedIndex] is DebugSignal signal)
                    {
                        descriptionTextBox.Text = signal.Description;
                    }
                }
                else
                {
                    descriptionTextBox.Text = "";
                }

                SetDebugSignals();
            }
        }

        private void UpdateDebugSignalMenu()
        {
            foreach (var item in debugSignalToolStripMenuItem.DropDownItems)
            {
                if (item is ToolStripMenuItem menuItem)
                {
                    menuItem.Checked = menuItem.Text == $"Signal {debugSignalTabControl.SelectedIndex+1}";
                }
            }
        }

        private void DebugSignalTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (sender is TabControl tabControl)
            {
                if (tabControl.SelectedIndex >= 0)
                {
                    var list = signalLists[tabControl.SelectedIndex];
                    
                    if (list.SelectedIndex >= 0)
                    {
                        if (list.Items[list.SelectedIndex] is DebugSignal signal)
                        {
                            descriptionTextBox.Text = signal.Description;
                        }
                    }
                    else
                    {
                        descriptionTextBox.Text = "";
                    }
                }

                UpdateDebugSignalMenu();
            }
        }

        #region Updating of debug signals

        private DebugSignal[] GetActiveSignals()
        {
            DebugSignal[] retValue = new DebugSignal[device.NumberOfSupportedDebugSignals];

            for (int n = 0; n < device.NumberOfSupportedDebugSignals; ++n)
            {
                var list = signalLists[n];

                if (list.SelectedIndex >= 0)
                {
                    if (list.Items[list.SelectedIndex] is DebugSignal signal)
                    {
                        retValue[n] = signal;
                    }
                    else
                    {
                        retValue[n] = DebugSignal.None;
                    }
                }
            }

            return retValue;
        }

        private void SetDebugSignals()
        {
            if (device is null)
                return;

            try
            {
                device.SetActiveDebugSignals(GetActiveSignals());
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }

        #endregion

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

        private void SplitContainer_Panel2_Resize(object sender, EventArgs e)
        {
            pictureBox.Width = hScrollBar.Width;
            pictureBox.Height = splitContainer.Panel2.Height - hScrollBar.Height;
        }
    }
}
