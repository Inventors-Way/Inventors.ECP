using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventors.ECP.TestFramework
{
    public partial class TrialsDialog : Form
    {
        public TrialsDialog()
        {
            InitializeComponent();
            trials.Text = _trials.ToString();
        }

        private int _trials = 2;

        public int Trials
        {
            get
            {

                if (Int32.TryParse(trials.Text, out int x))
                {
                    _trials = x;
                }

                return _trials;
            }
            set
            {
                trials.Text = value.ToString();
                _trials = value;
            }
        }

        private void Trials_Validating(object sender, CancelEventArgs e)
        {
            int x;

            if (Int32.TryParse(trials.Text, out x))
            {
                if (x > 1)
                {
                    errorProvider.SetError(trials, "");
                    okBtn.Enabled = true;
                }
                else
                {
                    errorProvider.SetError(trials, "Please enter a number larger than one");
                    okBtn.Enabled = false;
                }
            }
            else
            {
                errorProvider.SetError(trials, "Please enter a number");
                okBtn.Enabled = false;
            }
        }


        private void TrialsDialog_Load(object sender, EventArgs e)
        {

        }
    }
}
