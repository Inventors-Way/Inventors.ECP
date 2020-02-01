﻿using Inventors.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
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
        private string _port = String.Format(CultureInfo.CurrentCulture, "{0}:{1}", IPAddress.Loopback.ToString(), 9000);

        public override int BaudRate { get; set; } = int.MaxValue;

        public override string Port
        {
            get => _port;
            set
            {
                if (!IsOpen)
                {
                    _port = value;

                    if (!IPAddress.TryParse(Address, out IPAddress _))
                        throw new ArgumentException(Resources.INVALID_IP_ADDRESS);
                }
                else
                {
                    throw new InvalidOperationException(Resources.INVALID_OPERATION_WHILE_OPEN);
                }
            }
        }

        public string Address => Port.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries)[0];

        public int IPPort => int.Parse(Port.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries)[1], CultureInfo.CurrentCulture);

        public override List<string> GetAvailablePorts()
        {
            throw new NotImplementedException();
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
                server = new WatsonTcpServer(Address, IPPort);
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
            }).ConfigureAwait(false);
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
            }).ConfigureAwait(false);
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
            }).ConfigureAwait(false);
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
