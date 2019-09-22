using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventors.ECP.Tester
{
    public partial class TrialsDialog : Form
    {
        public TrialsDialog()
        {
            InitializeComponent();
            trials.Text = number.ToString();
        }

        public int Trials
        {
            get
            {
                int x;

                if (Int32.TryParse(trials.Text, out x))
                {
                    number = x;
                }

                return number;
            }
            set
            {
                trials.Text = value.ToString();
                number = value;
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

        private int number = 2;

        private void TrialsDialog_Load(object sender, EventArgs e)
        {

        }
    }
}
