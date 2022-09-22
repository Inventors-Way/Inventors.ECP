using ScottPlot;
using Serilog;
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
            analysis.Resize(width: chart.Width, height: chart.Height);

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
            var w = chart.Width = splitContainer1.Panel2.Width;
            var h = chart.Height = splitContainer1.Panel2.Height;

            foreach (var item in analysisList.Items)
            {
                if (item is AnalysisEngine analyser)
                {
                    analyser.Resize(width: w, height: h);
                }
            }

            if (current is not null)
            {
                try
                {
                    chart.Image = current.GetBitmap();
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
                    chart.Image = current.GetBitmap();
                }
                catch 
                { 
                }

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

        private void StartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (current is null)
                return;

            current.Start();
            UpdateState();
        }

        private void PauseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (current is null)
                return;

            current.Pause();
            UpdateState();
        }

        private void StopToolStripMenuItem_Click(object sender, EventArgs e)
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

            saveToolStripMenuItem.Enabled = current is not null;

            Text = $"{current} ({current.State})";
        }

        private void resultsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (current is null)
                return;

            var dialog = new SaveFileDialog()
            {
                Title = "Save results",
                Filter = "Comma Seperated Values (.csv)|*.csv",
                OverwritePrompt = true
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    current.SaveResults(dialog.FileName);
                }
                catch (Exception ex)
                {
                    Log.Error(ex.Message);
                }
            }

        }

        private void figureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (current is null)
                return;

            var dialog = new SaveFileDialog()
            {
               Title ="Save figure",
               Filter = "Bitmap Image (.bmp)|*.bmp|JPEG Image (.jpeg)|*.jpeg|PNG Image (.png)|*.png|Tiff Image (.tiff)|*.tiff",
               FilterIndex = 3,
               OverwritePrompt = true
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    current.SaveFig(dialog.FileName);
                }
                catch (Exception ex)
                {
                    Log.Error(ex.Message);
                }
            }
        }
    }
}
