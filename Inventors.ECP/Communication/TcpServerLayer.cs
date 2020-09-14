using Inventors.ECP.Communication.Discovery;
using Inventors.ECP.Communication.Tcp;
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
        private readonly object lockObject = new object();
        private WatsonTcpServer server;
        private Beacon beacon;
        private bool _isOpen;
        private bool _isConnected;
        private string _IpPort;
        private Location _port;

        public override int BaudRate { get; set; } = int.MaxValue;

        public override Location Location
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
            Location = location;
        }

        public override CommunicationProtocol Protocol => CommunicationProtocol.NETWORK;

        public override List<Location> GetLocations()
        {
            return new List<Location>();
        }

        public override bool IsOpen
        {
            get { lock (lockObject) { return _isOpen; } }
        }

        public override bool IsConnected
        {
            get { lock(lockObject) { return _isConnected; }}
        }

        private void SetConnected(bool value)
        {
            lock (lockObject) { _isConnected = value; } 
        }

        private string ClientPort
        {
            get { lock (lockObject) { return _IpPort; } }
            set { lock (lockObject) { _IpPort = value; } }
        }

        private void SetOpen(bool open) { lock(lockObject) { _isOpen = open; } }

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
                lock (lockObject)
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
                lock (lockObject)
                {
                    SetConnected(false);
                    ClientPort = null;
                    server = new WatsonTcpServer(Location.Address, Location.Port);
                    server.ClientConnected += OnConnected;
                    server.ClientDisconnected += OnDisconnected;
                    server.MessageReceived += MessageReceived;
                    server.Start();
                    SetOpen(true);
                    beacon = new Beacon(Location.BeaconID, (ushort)Location.Port);
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
        private bool disposedValue; 

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
