using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using WatsonTcp;

namespace Inventors.ECP
{
    public class TcpClientLayer :
        CommunicationLayer
    {
        private WatsonTcpClient client;
        private bool _open = false;
        private bool _connected = false;
        private int _port = 9000;
        private string _address = IPAddress.Loopback.ToString();

        public override int BaudRate { get; set; } = 1;

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
                    throw new InvalidOperationException("Cannot change the port while the client is open");
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
                    throw new InvalidOperationException("Cannot change the address while the client is open");
                }
            }
        }

        public override bool IsOpen => _open;

        public bool IsConnected
        {
            get { lock (this) { return _connected; } }
            set { lock(this) { _connected = value; } }
        }

        private void SetOpen(bool open) { lock (this) { _open = open; } }

        public override void Transmit(byte[] frame)
        {
            if (IsConnected)
            {
                client.Send(frame);
            }            
        }


        protected override void DoClose()
        {
            if (IsOpen)
            {
                IsConnected = false;
                client.Dispose();
                client = null;
                SetOpen(false);
            }
        }

        protected override void DoOpen(DeviceData device)
        {
            if (!IsOpen)
            {
                IsConnected = false;
                client = new WatsonTcpClient(Address, Port);
                client.ServerConnected += OnConnected;
                client.ServerDisconnected += OnDisconnected;
                client.MessageReceived += MessageReceived;
                client.Start();
                SetOpen(true);
            }
        }

        private async Task MessageReceived(byte[] data)
        {
            await Task.Run(() =>
            {
                foreach (var d in data)
                {
                    Destuffer.Add(d);
                }
            });
        }

        private async Task OnConnected() => await Task.Run(() => IsConnected = true);

        private async Task OnDisconnected() => await Task.Run(() => IsConnected = false);
    }
}
