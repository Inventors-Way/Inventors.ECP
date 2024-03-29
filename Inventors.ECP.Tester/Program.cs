using Inventors.ECP.TestFramework;
using System;
using System.IO;
using System.Windows.Forms;

namespace Inventors.ECP.Tester
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            #if NETFRAMEWORK
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            #else
            ApplicationConfiguration.Initialize();
            #endif

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