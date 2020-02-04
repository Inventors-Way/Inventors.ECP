using Inventors.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Inventors.ECP.Discovery
{
    /// <summary>
    /// Counterpart of the beacon, searches for beacons
    /// </summary>
    /// <remarks>
    /// The beacon list event will not be raised on your main thread!
    /// </remarks>
    public sealed class Probe : IDisposable
    {
        /// <summary>
        /// Remove beacons older than this
        /// </summary>
        private static readonly TimeSpan BeaconTimeout = new TimeSpan(0, 0, 0, 5); // seconds

        public event Action<IEnumerable<BeaconLocation>> BeaconsUpdated;

        private readonly Thread thread;
        private readonly EventWaitHandle waitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
        private readonly UdpClient udp = new UdpClient();
        private IEnumerable<BeaconLocation> currentBeacons = Enumerable.Empty<BeaconLocation>();

        private bool running = true;

        public Probe(BeaconID beaconType)
        {
            if (beaconType is null)
                throw new ArgumentNullException(nameof(beaconType));

            udp.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

            BeaconType = beaconType.ID;
            thread = new Thread(BackgroundLoop) { IsBackground = true };

            udp.Client.Bind(new IPEndPoint(IPAddress.Any, 0));
            try 
            {
                udp.AllowNatTraversal(true);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error switching on NAT traversal: " + ex.Message);
            }

            udp.BeginReceive(ResponseReceived, null);
        }

        public void Start()
        {
            thread.Start();
        }

        private void ResponseReceived(IAsyncResult ar)
        {
            var remote = new IPEndPoint(IPAddress.Any, 0);
            var bytes = udp.EndReceive(ar, ref remote);

            var typeBytes = Beacon.Encode(BeaconType).ToList();
            Debug.WriteLine(string.Join(", ", typeBytes.Select(_ => (char)_)));
            if (Beacon.HasPrefix(bytes, typeBytes))
            {
                try
                {
                    var portBytes = bytes.Skip(typeBytes.Count).Take(2).ToArray();
                    var port      = (ushort)IPAddress.NetworkToHostOrder((short)BitConverter.ToUInt16(portBytes, 0));
                    var payload   = Beacon.Decode(bytes.Skip(typeBytes.Count + 2));
                    NewBeacon(new BeaconLocation(new IPEndPoint(remote.Address, port), payload, DateTime.Now));
                }
                catch (Exception ex)
                {
                    Log.Debug(ex.Message);
                }
            }

            udp.BeginReceive(ResponseReceived, null);
        }

        public string BeaconType { get; private set; }

        private void BackgroundLoop()
        {
            while (running)
            {
                try
                {
                    BroadcastProbe();
                }
                catch (Exception ex)
                {
                    Log.Debug(ex.Message);
                }

                waitHandle.WaitOne(2000);
                PruneBeacons();
            }
        }

        private void BroadcastProbe()
        {
            var probe = Beacon.Encode(BeaconType).ToArray();
            udp.Send(probe, probe.Length, new IPEndPoint(IPAddress.Broadcast, Beacon.DiscoveryPort));
        }

        private void PruneBeacons()
        {
            var cutOff = DateTime.Now - BeaconTimeout;
            var oldBeacons = currentBeacons.ToList();
            var newBeacons = oldBeacons.Where(_ => _.LastAdvertised >= cutOff).ToList();
            if (EnumsEqual(oldBeacons, newBeacons)) return;

            BeaconsUpdated?.Invoke(newBeacons);
            currentBeacons = newBeacons;
        }

        private void NewBeacon(BeaconLocation newBeacon)
        {
            var newBeacons = currentBeacons
                .Where(_ => !_.Equals(newBeacon))
                .Concat(new [] { newBeacon })
                .OrderBy(_ => _.Data)
                .ThenBy(_ => _.Address, IPEndPointComparer.Instance)
                .ToList();
            BeaconsUpdated?.Invoke(newBeacons);
            currentBeacons = newBeacons;
        }

        private static bool EnumsEqual<T>(IEnumerable<T> xs, IEnumerable<T> ys)
        {
            return xs.Zip(ys, (x, y) => x.Equals(y)).Count() == xs.Count();
        }

        public void Stop()
        {
            running = false;
            waitHandle.Set();
            thread.Join();
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    try
                    {
                        Stop();
                        waitHandle.Dispose();
                        udp.Dispose();
                    }
                    catch (Exception ex)
                    {
                        Log.Debug(ex.Message);
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
