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
    public partial class ListDialog : Form
    {
        public ListDialog(string title, string values)
        {
            InitializeComponent();

            var items = values.Split(';');

            foreach (var item in items)
            {
                if (!string.IsNullOrEmpty(item))
                    listBox.Items.Add(item);
            }

            okBtn.Enabled = listBox.SelectedIndex >= 0;
            Text = title;
        }

        public int Index { get; private set; }

        private void ListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Index = listBox.SelectedIndex;
            okBtn.Enabled = listBox.SelectedIndex >= 0;
        }
    }
}
