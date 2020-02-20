using Inventors.ECP.Communication.Discovery;
using Inventors.ECP.Communication.Tcp;
using Inventors.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.ECP.Communication
{
    public class TcpServerLayer :
        CommunicationLayer,
        IDisposable
    {
        private WatsonTcpServer server;
        private Beacon beacon;
        private bool _isOpen = false;
        private bool _isConnected = false;
        private string _IpPort = null;
        private Location _port = new Location(new IPEndPoint(IPAddress.Loopback, 9000));
        private readonly BeaconID _beaconId;

        public override int BaudRate { get; set; } = int.MaxValue;

        public override Location Port
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
                    throw new InvalidOperationException(Resources.INVALID_OPERATION_WHILE_OPEN);
                }
            }
        }

        public TcpServerLayer(Location location)
        {
            if (location is null)
                throw new ArgumentNullException(nameof(location));

            Log.Debug("TCP SERVER [ {0} ]", location.ToString());
            Port = location;
        }

        public override CommunicationProtocol Protocol => CommunicationProtocol.NETWORK;

        public override List<Location> GetLocations()
        {
            return new List<Location>();
        }

        public override bool IsOpen
        {
            get { lock (this) { return _isOpen; } }
        }

        public override bool IsConnected
        {
            get { lock(this) { return _isConnected; }}
        }

        private void SetConnected(bool value)
        {
            lock (this) { _isConnected = value; } 
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
                    SetConnected(false);
                    SetOpen(false);
                    beacon.Stop();
                    beacon.Dispose();
                    beacon = null;
                }
            }            
        }

        protected override void DoOpen()
        {
            if (!IsOpen)
            {
                lock (this)
                {
                    SetConnected(false);
                    ClientPort = null;
                    server = new WatsonTcpServer(Port.Address, Port.Port);
                    server.ClientConnected += OnConnected;
                    server.ClientDisconnected += OnDisconnected;
                    server.MessageReceived += MessageReceived;
                    server.Start();
                    SetOpen(true);
                    beacon = new Beacon(_beaconId, (ushort)Port.Port);
                    beacon.Start();
                }
            }
        }

        private async Task OnConnected(string ipPort)
        {
            await Task.Run(() =>
            {
                if (!IsConnected)
                {
                    ClientPort = ipPort;
                    SetConnected(true);
                    Log.Debug("TcpServerLayer: Client [{0}] connected", ipPort);
                }
                else
                {
                    server.DisconnectClient(ipPort);
                    Log.Debug("TcpServerLayer: Client [{0}] disconnected as a client is allready connected", ipPort);
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
                        SetConnected(false);
                        Log.Debug("TcpServerLayer: Client disconnected: {0}", ipPort);
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
                        Destuffer.Add(data.Length, data);
                    }
                }
            }).ConfigureAwait(false);
        }

        public static IPAddress LocalAddress
        {
            get
            {
                IPHostEntry localhost = Dns.GetHostEntry(Dns.GetHostName());

                foreach (IPAddress address in localhost.AddressList)
                {
                    // Look for the IPv4 address of the local machine
                    if (address.AddressFamily.ToString() == "InterNetwork")
                    {
                        // Convert the IP address to a string and return it
                        return address;
                    }
                }

                return null;
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; 

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Close();
                    server.Dispose();

                    if (beacon is object)
                    {
                        beacon.Dispose();
                    }
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
