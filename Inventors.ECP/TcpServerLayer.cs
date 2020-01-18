using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Inventors.ECP
{
    public class TcpServerLayer :
        CommunicationLayer
    {
        public override int BaudRate { get; set; } = 1;

        public override bool IsOpen => throw new NotImplementedException();

        public override void Transmit(byte[] frame)
        {
            throw new NotImplementedException();
        }

        protected override void DoClose()
        {
            throw new NotImplementedException();
        }

        protected override void DoOpen()
        {
            throw new NotImplementedException();
        }

        public static string GetIpAddress()
        {
            IPHostEntry localhost;
            string localAddress = "";
            // Get the hostname of the local machine
            localhost = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress address in localhost.AddressList)
            {
                // Look for the IPv4 address of the local machine
                if (address.AddressFamily.ToString() == "InterNetwork")
                {
                    // Convert the IP address to a string and return it
                    localAddress = address.ToString();
                }
            }

            return localAddress;
        }
    }
}
