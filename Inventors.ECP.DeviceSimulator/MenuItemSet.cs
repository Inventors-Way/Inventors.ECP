using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventors.ECP.DeviceSimulator
{
    /**
     * \brief A set of menu items
     */
    public class MenuItemSet
    {
        private List<ToolStripMenuItem> items = new List<ToolStripMenuItem>();
        private readonly Action<ToolStripMenuItem> action;

        public bool Checked { get; set; } = true;

        public MenuItemSet(Action<ToolStripMenuItem> action)
        {
            this.action = action;
        }

        public void Add(ToolStripMenuItem item)
        {
            if (item != null)
            {
                item.Click += OnClicked;
                items.Add(item);
            }
        }

        public ToolStripMenuItem[] ToArray()
        {
            return items.ToArray();
        }

        private void OnClicked(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem)
            {
                var current = sender as ToolStripMenuItem;

                foreach (var item in items)
                {
                    if ((current == item))
                    {
                        action?.Invoke(current);
                    }

                    if (Checked)
                    {
                        item.Checked = current == item ? true : false;
                    }
                }
            }
        }
    }
}
