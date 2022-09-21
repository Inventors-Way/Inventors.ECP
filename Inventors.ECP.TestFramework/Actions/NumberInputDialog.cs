using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventors.ECP.TestFramework.Actions
{
    public partial class NumberInputDialog : Form
    {
        public NumberInputDialog(string title, double value)
        {
            InitializeComponent();
            Value = value;
            textBox1.Text = value.ToString();
            Text = title;
        }


        public double Value { get; set; }

        private void TextBox_TextChanged(object sender, EventArgs e) => UpdateValue(textBox1.Text);

        private void UpdateValue(string text)
        {
            if (double.TryParse(textBox1.Text, out double value))
            {
                Value = value;
                okBtn.Enabled = true;
            }
            else
            {
                okBtn.Enabled = false;
            }
        }
    }
}
