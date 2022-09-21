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
    public partial class TextInputDialog : Form
    {
        public TextInputDialog(string title, string initialValue = "")
        {
            InitializeComponent();
            Value = initialValue;
            textBox.Text = initialValue;
            Text = title;
            
        }

        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            Value = textBox.Text;
        }

        public string Value { get; set; }
    }
}
