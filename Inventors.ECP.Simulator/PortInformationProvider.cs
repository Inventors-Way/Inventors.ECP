using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.ECP.Simulator
{
    public class PortInformationProvider
    {
        private readonly ManagementObjectSearcher searcher;

        public PortInformationProvider()
        {
            searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_PnPEntity");
        }

        public string GetDescription(string port)
        {
            string retValue = null;

            foreach (var device in searcher.Get())
            {
                if (device is object)
                {
                    var name = device["Caption"];

                    if (name is object)
                    {
                        if (name.ToString().Contains(port))
                        {

                            retValue = name.ToString();
                            retValue = retValue.Substring(0, retValue.Length - port.Length - 3);
                            break;
                        }
                    }
                }
            }

            return retValue;
        }
    }
}
