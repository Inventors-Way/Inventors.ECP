using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventors.ECP.Tester
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (args.Length > 0)
            {
                if (File.Exists(args[0]))
                {
                    Application.Run(new MainWindow(args[0]));
                }
                else
                {
                    Console.WriteLine("File [ {0} ] does not exists", args[0]);
                }
            }
            else
            {
                Application.Run(new MainWindow());
            }
        }
    }
}
