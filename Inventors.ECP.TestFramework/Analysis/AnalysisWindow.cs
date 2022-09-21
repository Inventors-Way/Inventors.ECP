using ScottPlot;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Inventors.ECP.TestFramework.Analysis
{
    public partial class AnalysisWindow : Form
    {
        public event EventHandler<bool> OnAnalysisClosed;
        private readonly List<AnalysisEngine> analyses = new();
        private AnalysisEngine current;

        public AnalysisWindow()
        {
            InitializeComponent();
            Text = "Analysis";
            UpdateState();
        }

        public void Add(AnalysisEngine analysis)
        {
            analyses.Add(analysis);

            analysisList.Items.Add(analysis);
            analysis.Plot.Resize(width: chart.Width, height: chart.Height);

            if (current is null)
            {
                current = analysis;
                analysisList.SelectedIndex = 0;
                Text = current.ToString();
            }

            UpdateState();
        }

        private void AnalysisWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                OnAnalysisClosed?.Invoke(this, false);
            }
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OnAnalysisClosed?.Invoke(this, false);
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (current is null)
                return;

            if (!current.Updated)
                return;

            Redraw();
        }

        private void splitContainer1_Panel2_Resize(object sender, EventArgs e) => ResizePlot();

        private void ResizePlot()
        {
            chart.Width = splitContainer1.Panel2.Width;
            chart.Height = splitContainer1.Panel2.Height;

            if (current is not null)
            {
                current.Plot.Resize(width: chart.Width, height: chart.Height);

                try
                {
                    chart.Image = current.Plot.GetBitmap();
                }
                catch 
                { 
                }

                current.Updated = false;
            }
        }

        private void Redraw()
        {
            if (current is not null)
            {
                try
                {
                    chart.Image = current.Plot.GetBitmap();
                }
                catch 
                { 
                }

                current.Updated = false;
            }
        }

        private void analysisList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (analysisList.SelectedIndex < 0)
                return;
                current = analysisList.Items[analysisList.SelectedIndex] as AnalysisEngine;

            if (current is null)
                return;

            Redraw();
            Text = current.ToString();
            UpdateState();
        }

        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (current is null)
                return;

            current.Start();
            UpdateState();
        }

        private void pauseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (current is null)
                return;

            current.Pause();
            UpdateState();
        }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (current is null)
                return;

            current.Stop();
            UpdateState();
        }

        private void UpdateState()
        {
            if (current is null)
            {
                startToolStripMenuItem.Enabled = startBtn.Enabled = false;
                pauseToolStripMenuItem.Enabled = pauseBtn.Enabled = false;
                stopToolStripMenuItem.Enabled = stopBtn.Enabled = false;
                return;
            }

            startToolStripMenuItem.Enabled = startBtn.Enabled = current.StartPossible;
            startToolStripMenuItem.Checked = current.State == AnalysisState.RUNNING;
            pauseToolStripMenuItem.Enabled = pauseBtn.Enabled = current.PausePossible;
            pauseToolStripMenuItem.Checked = current.State == AnalysisState.PAUSED;
            stopToolStripMenuItem.Enabled = stopBtn.Enabled = current.StopPossible;
            stopToolStripMenuItem.Checked = current.State == AnalysisState.STOPPED;

            Text = $"{current.ToString()} ({current.State})";
        }
    }
}
