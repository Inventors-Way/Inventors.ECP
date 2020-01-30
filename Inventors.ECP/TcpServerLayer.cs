using Inventors.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using WatsonTcp;

namespace Inventors.ECP
{
    public class TcpServerLayer :
        CommunicationLayer
    {
        private WatsonTcpServer server;
        private bool _isOpen = false;
        private bool _isConnected = false;
        private string _IpPort = null;
        private int _port = 9000;
        private string _address = IPAddress.Loopback.ToString();

        public override int BaudRate { get; set; } = int.MaxValue;

        public int Port 
        {
            get => _port;
            set
            {
                if (!IsOpen)
                {
                    _port = value;
                }
                else
                {
                    throw new InvalidOperationException("Cannot change the port while the server is open");
                }
            }
        }

        public string Address 
        {
            get => _address;
            set
            {
                if (!IsOpen)
                {
                    _address = value;
                }
                else
                {
                    throw new InvalidOperationException("Cannot change the address while the server is open");
                }
            }
        }

        public TcpServerLayer() { }

        public override bool IsOpen
        {
            get { lock (this) { return _isOpen; } }
        }

        private bool IsConnected
        {
            get { lock(this) { return _isConnected; }}
            set { lock (this) { _isConnected = value; }}
        }

        private string ClientPort
        {
            get { lock (this) { return _IpPort; } }
            set { lock (this) { _IpPort = value; } }
        }

        private void SetOpen(bool open) { lock(this) { _isOpen = open; } }

        public override void Transmit(byte[] frame)
        {
            var port = ClientPort;

            if (IsConnected)
            {
                server.Send(port, frame);
            }
        }

        protected override void DoClose()
        {
            if (IsOpen)
            {
                lock (this)
                {
                    foreach (var client in server.ListClients())
                    {
                        server.DisconnectClient(client);
                    }

                    server.Dispose();
                    server = null;
                    IsConnected = false;
                    SetOpen(false);
                }
            }            
        }

        protected override void DoOpen(DeviceData device)
        {
            if (!IsOpen)
            {
                IsConnected = false;
                ClientPort = null;
                server = new WatsonTcpServer(Address, Port);
                server.ClientConnected += OnConnected;
                server.ClientDisconnected += OnDisconnected;
                server.MessageReceived += MessageReceived;
                server.Start();
                SetOpen(true);
            }
        }

        private async Task OnConnected(string ipPort)
        {
            await Task.Run(() =>
            {
                if (!IsConnected)
                {
                    ClientPort = ipPort;
                    IsConnected = true;
                }
                else
                {
                    server.DisconnectClient(ipPort);
                }
            });
        }

        private async Task OnDisconnected(string ipPort, DisconnectReason reason)
        {
            await Task.Run(() =>
            {
                if (IsConnected)
                {
                    if (ClientPort == ipPort)
                    {
                        IsConnected = false;
                    }
                }
            });
        }

        private async Task MessageReceived(string ipPort, byte[] data)
        {
            await Task.Run(() =>
            {
                if (IsConnected)
                {
                    if (ipPort == ClientPort)
                    {
                        foreach (var d in data)
                        {
                            Destuffer.Add(d);
                        }
                    }
                }
            });
        }

        public static string GetLocalAddress()
        {
            IPHostEntry localhost = Dns.GetHostEntry(Dns.GetHostName());
 
            foreach (IPAddress address in localhost.AddressList)
            {
                // Look for the IPv4 address of the local machine
                if (address.AddressFamily.ToString() == "InterNetwork")
                {
                    // Convert the IP address to a string and return it
                    return address.ToString();
                }
            }

            return null;
        }
    }
}
