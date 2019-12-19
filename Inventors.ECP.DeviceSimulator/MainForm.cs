﻿using Inventors.ECP.Functions;
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
                Log.Status("Connection opened");
            }
        }

        private void disconnectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (slave.IsOpen)
            {
                slave.Close();
                Log.Status("Connection closed");
            }
        }

        private void documentationToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {

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

            return true;
        }

        public bool Accept(Ping func)
        {
            func.Count = pings;
            ++pings;
            return true;
        }

        public bool Accept(GetEndianness func)
        {
            return true;
        }

        public void Accept(PrintfMessage msg)
        {
            Log.Status("PRINTF: {0}", msg.DebugMessage);
        }
    }
}
