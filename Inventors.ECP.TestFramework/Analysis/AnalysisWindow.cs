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
        }

        public void Add(AnalysisEngine analysis)
        {
            analyses.Add(analysis);

            analysisList.Items.Add(analysis);

            if (current is null)
            {
                current = analysis;
                analysisList.SelectedIndex = 0;
                Text = current.ToString();
            }
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

        }
    }
}
