using Inventors.ECP.Communication;
using Inventors.ECP.Functions;
using Inventors.ECP.Messages;
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
        private UInt32 pings = 0;

        public MainForm()
        {
            InitializeComponent();
            SetupLogging();
            SetupSerialSlave();
            SetupPorts();
            SetTitle();
            UpdateStatus();
        }

        private void SetupLogging()
        {
            logger = new Logger() { Box = logBox };
            Log.SetLogger(logger);
            Log.Level = LogLevel.DEBUG;
        }

        private void SetupSerialSlave()
        {
            if (slave is object)
            {
                slave.Close();
                slave = null;
            }

            serial = new SerialPortLayer()
            {
                BaudRate = 38400
            };
            slave = new DeviceSlave(serial)
            {
                FunctionListener = this,
                MessageListener = this
            };
            slave.Add(new PrintfMessage());
            slave.Add(new DeviceIdentification());
            slave.Add(new Ping());
            slave.Add(new GetEndianness());

            serialToolStripMenuItem.Checked = true;
            networkToolStripMenuItem.Checked = false;
        }

        private void SetupPorts()
        {
            var names = SerialPort.GetPortNames();
            portMenuItems = new MenuItemSet((s) =>
            {
                Log.Debug("Port changed to: {0}", s.Text);
                serial.Port = s.Text;
                UpdateStatus();
            });

            for (int n = 0; n < names.Length; ++n)
            {
                var item = new ToolStripMenuItem(names[n]);
                portMenuItems.Add(item);
                portMenuItem.DropDownItems.Add(item);

                if (n == 0)
                {
                    serial.Port = names[n];
                    Log.Status("Serial port: {0}", serial.Port);
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

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void ConnectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!slave.IsOpen)
            {
                pings = 0;
                slave.Open();
                slave.Printf("Default Slave Device");
                UpdateStatus();
            }
        }

        private void DisconnectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (slave.IsOpen)
            {
                slave.Close();
                UpdateStatus();
            }
        }

        private void SerialToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!slave.IsOpen)
            {

                serialToolStripMenuItem.Checked = true;
                networkToolStripMenuItem.Checked = false;
            }
        }

        private void NetworkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!slave.IsOpen)
            {

                serialToolStripMenuItem.Checked = false;
                networkToolStripMenuItem.Checked = true;
            }
        }


        private void DocumentationToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void UpdateStatus()
        {
            if (slave.IsOpen)
            {
                statusText.Text = String.Format("Connected [ {0} (Ping: {1})]", serial.Port, pings);
            }
            else
            {
                statusText.Text = String.Format("Not connected [ {0} ]", serial.Port);
            }
        }

        public bool Accept(DeviceIdentification func)
        {
            func.DeviceID = 1;
            func.ManufactureID = 1;
            func.Manufacture = "Inventors' Way";
            func.Device = "Default Device";
            func.MajorVersion = 1;
            func.MinorVersion = 2;
            func.PatchVersion = 3;
            func.EngineeringVersion = 4;
            func.Checksum = 5;
            func.SerialNumber = 1000;

            Log.Status("Device Identification:");
            Log.Status("   Manufacturer: {0} [{1}]", func.Manufacture, func.ManufactureID);
            Log.Status("   Device      : {0} [{1}] (Checksum: {2})", func.Device, func.DeviceID, func.Checksum);
            Log.Status("   Firmware    : {0}", func.Version);

            return true;
        }

        public bool Accept(Ping func)
        {
            func.Count = pings;
            UpdateStatus();
            ++pings;
            return true;
        }

        public bool Accept(GetEndianness func)
        {
            Log.Status("Endianness: {0}", func.EqualEndianness);
            return true;
        }

        public void Accept(PrintfMessage msg)
        {
            Log.Status("PRINTF: {0}", msg.DebugMessage);
        }
    }
}
