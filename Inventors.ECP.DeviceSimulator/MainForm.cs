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

            for (int n = 0; n < names.Length; ++n)
            {
                var item = new ToolStripMenuItem(names[n]);
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
    }
}
