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

namespace Inventors.ECP.Communication
{
    public class TcpClientLayer :
        CommunicationLayer,
        IDisposable
    {
        private readonly Probe _probe;
        private readonly List<BeaconLocation> _beacons = new List<BeaconLocation>();
        private WatsonTcpClient _client;
        private bool _open = false;
        private bool _connected = false;
        private string _port = new IPEndPoint(IPAddress.Loopback, 9000).ToString();

        public override int BaudRate { get; set; } = 1;

        public TcpClientLayer(BeaconID id) 
        {
            if (id is null)
                throw new ArgumentNullException(nameof(id));

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

        public override string Port
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

        public TcpClientLayer(long address, ushort port) => SetTcpPort(address, port);

        public TcpClientLayer(IPAddress address, ushort port) => SetTcpPort(address, port);

        public string Address => Port.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries)[0];

        public int IPPort => int.Parse(Port.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries)[1], CultureInfo.CurrentCulture);

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
                _client = new WatsonTcpClient(Address, IPPort);
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
                foreach (var d in data)
                {
                    Destuffer.Add(d);
                }
            }).ConfigureAwait(false);
        }

        private async Task OnConnected() => await Task.Run(() => SetConnected(true)).ConfigureAwait(false);

        private async Task OnDisconnected() => await Task.Run(() => SetConnected(false)).ConfigureAwait(false);

        public override List<string> GetAvailablePorts()
        {            
            lock (_beacons)
            {
                return (from b in _beacons
                        select b.Address.ToString()).ToList();
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
