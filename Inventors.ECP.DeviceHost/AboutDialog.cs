using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventors.ECP.DeviceHost
{
    public partial class AboutDialog : Form
    {
        public AboutDialog()
        {
            InitializeComponent();
            ReparentToPicture(product);
            ReparentToPicture(version);
        }

        private void ReparentToPicture(Label label)
        {
            var pos = this.PointToScreen(label.Location);
            pos = picture.PointToClient(pos);
            label.Parent = picture;
            label.Location = pos;
            label.BackColor = Color.Transparent;
        }   

        public Image Image
        {
            get => picture.Image;
            set => picture.Image = value;
        }

        public string Product
        {
            get => product.Text;
            set => product.Text = value;
        }

        public string Version
        {
            get => version.Text;
            set => version.Text = value;
        }

        public string Line1
        {
            get => line1.Text;
            set => line1.Text = value;
        }

        public string Line2
        {
            get => line2.Text;
            set => line2.Text = value;
        }

        public string Line3
        {
            get => line3.Text;
            set => line3.Text = value;
        }
    }
}
