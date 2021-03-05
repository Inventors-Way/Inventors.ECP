using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace Inventors.ECP.Simulator
{
    public class ProgramOptions
    {
        [Option('p', "port", Required = false, Default = "", HelpText = "Port name")]
        public string PortName { get; set; }

        [Option('r', "rate", Required = false, Default = 38400, HelpText = "Baudrate")]
        public int BaudRate { get; set; }

        [Option('c', "composite", Required = false, Default = false, HelpText = "Composite device")]
        public bool Composite { get; set; }
    }
}
