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
            SetupSlave();
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

        private void SetupSlave()
        {
            serial = new SerialPortLayer()
            {
                BaudRate = 115200
            };
            slave = new DeviceSlave(serial);
            slave.FunctionListener = this;
            slave.MessageListener = this;
            slave.MessageDispatchers.Add(PrintfMessage.CreateDispatcher());
            slave.FunctionDispatchers.Add(DeviceIdentification.CreateDispatcher());
            slave.FunctionDispatchers.Add(Ping.CreateDispatcher());
            slave.FunctionDispatchers.Add(GetEndianness.CreateDispatcher());
        }

        private void SetupPorts()
        {
            var names = SerialPort.GetPortNames();
            portMenuItems = new MenuItemSet((s) =>
            {
                Log.Debug("Port changed to: {0}", s.Text);
                serial.PortName = s.Text;
                UpdateStatus();
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
            if (!slave.IsOpen)
            {
                pings = 0;
                slave.Open();
                slave.Printf("Default Slave Device");
                UpdateStatus();
            }
        }

        private void disconnectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (slave.IsOpen)
            {
                slave.Close();
                UpdateStatus();
            }
        }

        private void documentationToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void UpdateStatus()
        {
            if (slave.IsOpen)
            {
                statusText.Text = String.Format("Connected [ {0} (Ping: {1})]", serial.PortName, pings);
            }
            else
            {
                statusText.Text = String.Format("Not connected [ {0} ]", serial.PortName);
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
            func.SerialNumber = 1001;
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
