using CommandLine;
using System;
using System.Collections.Generic;
using System.Text;
using Inventors.ECP;
using Inventors.ECP.DefaultDevice;

namespace Inventors.ECP.Tools
{
    [Verb("scan", HelpText = "Scan for ECP devices")]
    public class ScanAction :
        CommandAction
    {
        [Value(0, HelpText = "Type of comm layer (serial or network)", MetaName = "comm layer")]
        public string CommType { get; set; }

        [Option('b', "baudrate", Required = false, HelpText = "The baudrate of the device")]
        public int BaudRate { get; set; } = 38400;


        public override int Run()
        {
            int retValue = 0;

            if (CreateDevice() is Device device)
            {
                var fmtStrHeader = " | {0, -15} | {1}";
                var fmtStr = " | {0, -15} | {1} (Serial number: {2})";
                device.Timeout = 100;

                Console.Write("Scanning for devices ... ");
                var available = device.GetAvailableDevices();
                Console.WriteLine("completed");
                Console.WriteLine();
                Console.WriteLine(fmtStrHeader, "PORT", "DEVICE");

                foreach (var current in available)
                {
                    Console.WriteLine(fmtStr,
                        current.Port,
                        "[" + current.ManufactureID.ToString() + "." + current.DeviceID.ToString() + "] " + current.Device,
                        current.SerialNumber);
                }
            }

            return retValue;
        }

        private Device CreateDevice()
        {
            Device retValue = null;

            if (CommType == "serial")
            {
                retValue = new DefaultSerialDevice
                {
                    BaudRate = BaudRate
                };
            }
            else if (CommType == "network")
            {
                retValue = new DefaultTcpDevice();
            }
            else
            {
                Console.WriteLine("Error: unkown comm layer type [ {0} ]", CommType);
            }

            return retValue;
        }
    }
}
