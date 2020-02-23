using Inventors.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Inventors.ECP.Communication.Discovery
{
    /// <summary>
    /// Instances of this class can be autodiscovered on the local network through UDP broadcasts
    /// </summary>
    /// <remarks>
    /// The advertisement consists of the beacon's application type and a short beacon-specific string.
    /// </remarks>
    public sealed class Beacon : IDisposable
    {
        internal const int DiscoveryPort = 35891;
        private readonly UdpClient udp;
 
        /// <summary>
        /// Create a beacon.
        /// </summary>
        /// <param name="id">The BeaconID of the beacon</param>
        /// <param name="advertisedPort">The port of the service to advertice</param>
        public Beacon(BeaconID id, ushort advertisedPort)
        {
            if (id is null)
                throw new ArgumentNullException(nameof(id));

            BeaconType     = id.ID;
            AdvertisedPort = advertisedPort;
            BeaconData     = id.Data;

            udp = new UdpClient();
            udp.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            udp.Client.Bind(new IPEndPoint(IPAddress.Any, DiscoveryPort));

            try 
            {
                udp.AllowNatTraversal(true);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error switching on NAT traversal: " + ex.Message);
            }

            Log.Debug("Beacon [ {0}/{1} ] created on Port: {2}", BeaconType, BeaconData, AdvertisedPort);
        }

        public void Start()
        {
            Stopped = false;
            udp.BeginReceive(ProbeReceived, null);
            Log.Debug("Beacon [ {0}/{1} ] started on Port: {2}", BeaconType, BeaconData, AdvertisedPort);
        }

        public void Stop()
        {
            Stopped = true;
            Log.Debug("Beacon [ {0}/{1} ] stopped on Port: {2}", BeaconType, BeaconData, AdvertisedPort);
        }

        private void ProbeReceived(IAsyncResult ar)
        {
            var remote = new IPEndPoint(IPAddress.Any, 0);

            if (!Stopped)
            {
                var bytes = udp.EndReceive(ar, ref remote);

                // Compare beacon type to probe type
                var typeBytes = Encode(BeaconType);

                if (HasPrefix(bytes, typeBytes))
                {
                    // If true, respond again with our type, port and payload
                    var responseData = Encode(BeaconType)
                        .Concat(BitConverter.GetBytes((ushort)IPAddress.HostToNetworkOrder((short)AdvertisedPort)))
                        .Concat(Encode(BeaconData)).ToArray();
                    udp.Send(responseData, responseData.Length, remote);
                }

                udp.BeginReceive(ProbeReceived, null);
            }
        }

        internal static bool HasPrefix<T>(IEnumerable<T> haystack, IEnumerable<T> prefix)
        {
            return haystack.Count() >= prefix.Count() &&
                haystack.Zip(prefix, (a, b) => a.Equals(b)).All(_ => _);
        }

        /// <summary>
        /// Convert a string to network bytes
        /// </summary>
        internal static IEnumerable<byte> Encode(string data) 
        {
            var bytes = Encoding.UTF8.GetBytes(data);
            var len = IPAddress.HostToNetworkOrder((short)bytes.Length);

            return BitConverter.GetBytes(len).Concat(bytes);
        }

        /// <summary>
        /// Convert network bytes to a string
        /// </summary>
        internal static string Decode(IEnumerable<byte> data)
        {
            var listData = data as IList<byte> ?? data.ToList();
            var len = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(listData.Take(2).ToArray(), 0));

            if (listData.Count < 2 + len) throw new ArgumentException(Resources.BEACON_PACKET_DATA_ERROR);

            return Encoding.UTF8.GetString(listData.Skip(2).Take(len).ToArray());
        }

        public override string ToString()
        {
            return BeaconType;
        }

        /// <summary>
        /// Return the machine's hostname (usually nice to mention in the beacon text)
        /// </summary>
        public static string HostName
        {
            get { return Dns.GetHostName(); }
        }

        public string BeaconType { get; private set; }
        public ushort AdvertisedPort { get; private set; }
        public bool Stopped { get; private set; }
        public string BeaconData { get; private set; }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Stop();
                    udp.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
