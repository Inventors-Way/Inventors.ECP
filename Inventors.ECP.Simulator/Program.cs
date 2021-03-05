using CommandLine;
using Inventors.ECP.DefaultDevice;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.ECP.Simulator
{
    class Program
    {
        static BusPeripheral CreatePeripheral(ProgramOptions o)
        {
            if (o.Composite)
            {
                return new DefaultCompositePeripheral();
            }
            else
            {
                return new DefaultPeripheral();
            }
        }

        static void Run(ProgramOptions o)
        {
            if (!string.IsNullOrEmpty(o.PortName))
            {
                using (var peripheral = CreatePeripheral(o))
                {

                    Console.WriteLine($"Opening a [ {peripheral.GetType()}] on port [ {o.PortName}:{o.BaudRate} ]");
                    peripheral.BaudRate = o.BaudRate;
                    peripheral.Location = o.PortName;
                    peripheral.Open();

                    Console.Write("Press any key to exit ...");
                    Console.ReadKey();
                    Console.WriteLine();

                    peripheral.Close();
                }
            }
            else
            {
                var provider = new PortInformationProvider();

                foreach (var port in SerialPort.GetPortNames())
                {
                    Console.WriteLine($"[ {port} ] {provider.GetDescription(port)}");
                }
            }
        }

        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<ProgramOptions>(args).WithParsed<ProgramOptions>(o => Run(o));
        }
    }
}
