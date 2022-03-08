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
using ScottPlot;

namespace Inventors.ECP.TestFramework.Profiling
{
    public partial class ProfilerWindow : Form
    {
        public event EventHandler<bool> OnProfilerClosed;

        private readonly Dictionary<int, ListBox> signalLists = new Dictionary<int, ListBox>();
        private Device device;
        private Profiler profiler;
        private Plot plot;
        private ColorSet colors = new ColorSet();

        public ProfilerWindow()
        {
            InitializeComponent();
            analysisMenu.Enabled = false;
            fileMenu.Enabled = false;
            clearProfilerToolStripMenuItem.Checked = false;
        }

        private bool MaximumEnabled
        {
            get => maximumToolStripMenuItem.Checked;
            set => maximumToolStripMenuItem.Checked = value;
        }

        private bool MinimumEnabled
        {
            get => minimumToolStripMenuItem.Checked;
            set => minimumToolStripMenuItem.Checked = value;
        }

        private bool EventsEnabled
        {
            get => eventsToolStripMenuItem.Checked;
            set => eventsToolStripMenuItem.Checked = value;
        }

        private bool ViolationsEnabled
        {
            get => timeViolationsToolStripMenuItem.Checked;
            set => timeViolationsToolStripMenuItem.Checked = value;
        }


        public void SetDevice(Device device)
        {
            if (device is null)
                throw new ArgumentNullException(nameof(device));

            if (this.device is null)
            {
                this.device = device;
                this.profiler = device.Profiler;

                analysisMenu.Enabled = true;
                fileMenu.Enabled = true;
                InitializeDebugSignals();
                SetDebugSignals();
            }

            plot = new Plot(width: pictureBox.Width, height: pictureBox.Height);
            Redraw();
        }

        private void InitializeDebugSignals()
        {
            var debugSpecification = device.GetActiveDebugSpecification();
            debugSpecification.Initialize();

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

                if (debugSpecification is not null && debugSpecification.Signals is not null)
                {
                    listBox.Items.Add(DebugSignal.None);

                    foreach (var signal in debugSpecification.Signals)
                    {
                        listBox.Items.Add(signal);
                    }
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
                else
                {
                    retValue[n] = DebugSignal.None;
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
                profiler.Reset();
                colors.Reset();
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
            if (profiler is object)
            {
                if (profiler.Updated)
                {
                    Redraw();
                }
            }
        }

        private void Redraw()
        {
            if (plot is object)
            {
                if (profiler is object)
                {
                    var report = profiler.GetReport();
                    plot.Clear();

                    foreach (var signal in report.Timing)
                    {
                        plot.PlotScatter(xs: signal.Time, 
                                         ys: signal.Average, 
                                         markerShape: MarkerShape.none,
                                         label: $"{signal.Code}", 
                                         color: colors.GetColor(signal.Code));

                        if (MaximumEnabled)
                        {
                            plot.PlotScatter(xs: signal.Time,
                                             ys: signal.Maximum,
                                             lineStyle: LineStyle.Dot,
                                             markerShape: MarkerShape.none,
                                             color: colors.GetColor(signal.Code));
                        }

                        if (MinimumEnabled)
                        {
                            plot.PlotScatter(xs: signal.Time,
                                             ys: signal.Minimum,
                                             lineStyle: LineStyle.Dot,
                                             markerShape: MarkerShape.none,
                                             color: colors.GetColor(signal.Code));
                        }
                    }

                    if (EventsEnabled)
                    {
                        foreach (var e in report.Events)
                        {
                            var line = plot.AddVerticalLine(e.Time);
                            line.Color = Color.Black;

                            var txt = plot.AddText(label: e.Description, x: e.Time, y: 0);
                            txt.Alignment = Alignment.LowerLeft;
                            txt.Rotation = -90;
                            txt.Color = Color.Black;
                            
                        }
                    }

                    if (ViolationsEnabled)
                    {
                        foreach (var t in report.Violation)
                        {
                            var line = plot.AddVerticalLine(t.Time);
                            line.Color = Color.Black;

                            var txt = plot.AddText(label: $"{t.Time}us", x: t.Time, y: 0);
                            txt.Alignment = Alignment.LowerLeft;
                            txt.Color = Color.Black;
                            txt.Rotation = -90;
                        }
                    }

                    //plot.Legend(location: legendLocation.lowerLeft);

                    plot.XLabel("Time [s]");
                    plot.YLabel("Elapsed Time [us]");
                }

                try
                {
                    var image = plot.GetBitmap();
                    pictureBox.Image = image;
                }
                catch { }
            }
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

        private void s60timeChanged_Click(object sender, EventArgs e)
        {
            if (profiler is object)
            {
                profiler.TimeSpan = 60;

                s60timeToolStripMenuItem.Checked = true;
                s300timeToolStripMenuItem.Checked = false;
                s600timeToolStripMenuItem.Checked = false;
            }
        }

        private void s300timeChanged_Click(object sender, EventArgs e)
        {
            if (profiler is object)
            {
                profiler.TimeSpan = 300;

                s60timeToolStripMenuItem.Checked = false;
                s300timeToolStripMenuItem.Checked = true;
                s600timeToolStripMenuItem.Checked = false;
            }
        }

        private void s600timeChanged_Click(object sender, EventArgs e)
        {
            if (profiler is object)
            {
                profiler.TimeSpan = 600;

                s60timeToolStripMenuItem.Checked = false;
                s300timeToolStripMenuItem.Checked = false;
                s600timeToolStripMenuItem.Checked = true;
            }
        }

        private void MaximumToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MaximumEnabled = !MaximumEnabled;
        }

        private void MinimumToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MinimumEnabled = !MinimumEnabled;
        }

        private void EventsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EventsEnabled = !EventsEnabled;
        }

        private void TimeViolationsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ViolationsEnabled = !ViolationsEnabled;
        }

        private void PausedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (profiler is object)
            {
                profiler.Paused = !profiler.Paused;
                pausedToolStripMenuItem.Checked = profiler.Paused;
                hScrollBar.Enabled = profiler.Paused && profiler.End > profiler.TimeSpan;

                if (hScrollBar.Enabled)
                {
                    hScrollBar.Value = 0;
                    hScrollBar.Maximum = (int) (Math.Round(profiler.End - profiler.TimeSpan + 0.5));
                    hScrollBar.Value = hScrollBar.Maximum;
                }
            }
        }

        private void HScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            if (profiler is object)
            {
                if (profiler.Paused)
                {
                    profiler.End = e.NewValue + profiler.TimeSpan;
                }
            }
        }


        private void SplitContainer_Panel2_Resize(object sender, EventArgs e) => ResizePlot();

        private void ResizePlot()
        {
            pictureBox.Width = hScrollBar.Width;
            pictureBox.Height = splitContainer.Panel2.Height - hScrollBar.Height;

            if (plot is object)
            {
                plot.Resize(width: pictureBox.Width, height: pictureBox.Height);
                pictureBox.Image = plot.GetBitmap();
            }
        }

        private void ProfilerWindow_SizeChanged(object sender, EventArgs e) => ResizePlot();
    }
}
