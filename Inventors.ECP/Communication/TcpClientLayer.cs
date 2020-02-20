using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Inventors.ECP.Communication.Tcp;
using System.Linq;
using Inventors.ECP.Communication.Discovery;
using Inventors.Logging;

namespace Inventors.ECP.Communication
{
    public class TcpClientLayer :
        CommunicationLayer,
        IDisposable
    {
        private readonly BeaconID _id;
        private readonly Probe _probe;
        private readonly List<BeaconLocation> _beacons = new List<BeaconLocation>();
        private WatsonTcpClient _client;
        private bool _open = false;
        private bool _connected = false;
        private Location _port = new Location(new IPEndPoint(IPAddress.Loopback, 9000));

        public override int BaudRate { get; set; } = 1;

        public TcpClientLayer(BeaconID id) 
        {
            if (id is null)
                throw new ArgumentNullException(nameof(id));
            
            Log.Debug("TCP CLIENT [ {0} ]", id);
            _id = id;
            _probe = new Probe(id);
            _probe.BeaconsUpdated += (beacons) =>
            {
                lock (_beacons)
                {
                    _beacons.Clear();
                    _beacons.AddRange(beacons);
                }
            };
            _probe.Start();
        }

        public override CommunicationProtocol Protocol => CommunicationProtocol.NETWORK;

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
                    throw new InvalidOperationException(Resources.PORT_CHANGE_WHILE_OPEN);
                }
            }
        }

        public TcpClientLayer() { }

        public TcpClientLayer(Location location) => SetLocation(location);

        public override bool IsOpen => _open;

        public override bool IsConnected
        {
            get { lock (this) { return _connected; } }
        }

        private void SetConnected(bool value)
        {
            lock (this) { _connected = value; }
        }

        private void SetOpen(bool open) { lock (this) { _open = open; } }

        public override void Transmit(byte[] frame)
        {
            if (IsConnected)
            {
                _client.Send(frame);
            }            
        }


        protected override void DoClose()
        {
            if (IsOpen)
            {
                SetConnected(false);
                _client.Dispose();
                _client = null;
                SetOpen(false);
            }
        }

        protected override void DoOpen()
        {
            if (!IsOpen)
            {
                SetConnected(false);
                _client = new WatsonTcpClient(Port.Address, Port.Port);
                _client.ServerConnected += OnConnected;
                _client.ServerDisconnected += OnDisconnected;
                _client.MessageReceived += MessageReceived;
                _client.Start();
                SetOpen(true);
            }
        }

        private async Task MessageReceived(byte[] data)
        {
            await Task.Run(() =>
            {
                Destuffer.Add(data.Length, data);
            }).ConfigureAwait(false);
        }

        private async Task OnConnected() => await Task.Run(() => SetConnected(true)).ConfigureAwait(false);

        private async Task OnDisconnected() => await Task.Run(() => SetConnected(false)).ConfigureAwait(false);

        public override List<Location> GetLocations() 
        {            
            lock (_beacons)
            {
                return (from b in _beacons
                        select new Location(protocol: CommunicationProtocol.NETWORK,
                                            address: b.Address.ToString(),
                                            manufacturerId: _id.ManufactureID,
                                            deviceId: _id.DeviceID,
                                            serialNumber: UInt32.Parse(b.Data)
                                            )).ToList();
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _client.Dispose();
                    _probe.Dispose();
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
