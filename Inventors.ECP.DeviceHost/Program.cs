﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Inventors.ECP.Hosting;

namespace Inventors.ECP.DeviceHost
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new HostingForm()
            {
                NotifyIcon = Resource.hammer,
                Icon = Resource.hammer,
                Text = "Device Manager"
            });
        }
    }
}