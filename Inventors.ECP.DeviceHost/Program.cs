using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

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
            var config = HostConfiguration.Load();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new HostingForm()
            {
                NotifyIcon = config.NotifyIcon,
                Icon = config.Icon,
                Text = config.Title,
                CloseToTray = config.CloseToTray,
                MinimizeToTray = config.MinimizeToTray
            });
        }
    }
}
