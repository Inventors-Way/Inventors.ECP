using Inventors.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventors.ECP.DeviceSimulator
{
    public partial class MainForm : Form
    {
        private Logger logger;
        private SerialPortLayer serial = null;
        private MenuItemSet portMenuItems;
        private DeviceSlave slave;

        public MainForm()
        {
            InitializeComponent();
            SetupLogging();
            SetupPorts();
            SetTitle();
        }

        private void SetupLogging()
        {
            logger = new Logger() { Box = logBox };
            Log.SetLogger(logger);
            Log.Level = LogLevel.DEBUG;
        }

        private void SetupPorts()
        {
            var names = SerialPort.GetPortNames();
            serial = new SerialPortLayer()
            {
                BaudRate = 115200
            };
            slave = new DeviceSlave(serial);
            portMenuItems = new MenuItemSet((s) =>
            {
                Log.Debug("Port changed to: {0}", s.Text);
                serial.PortName = s.Text;
            });

            for (int n = 0; n < names.Length; ++n)
            {
                var item = new ToolStripMenuItem(names[n]);
                portMenuItems.Add(item);
                portMenuItem.DropDownItems.Add(item);

                if (n == 0)
                {
                    serial.PortName = names[n];
                    Log.Status("Serial port: {0}", serial.PortName);
                    item.Checked = true;
                }
            }
        }

        private void SetTitle()
        {
            Text = String.Format("ECP Default Device Simulator, Rev {0}", VersionInformation);
        }

        public static string VersionInformation
        {
            get
            {
                return Assembly.GetAssembly(typeof(MainForm)).GetName().Version.ToString();
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void connectToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void disconnectToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void documentationToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}
