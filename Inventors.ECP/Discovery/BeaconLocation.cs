using System;
using System.Net;

namespace Inventors.ECP.Discovery
{
    /// <summary>
    /// Class that represents a discovered beacon
    /// </summary>
    public class BeaconLocation 
    {
        public BeaconLocation(IPEndPoint address, string data, DateTime lastAdvertised)
        {
            Address = address;
            Data    = data;
            LastAdvertised = lastAdvertised;
        }

        public IPEndPoint Address { get; private set; }
        public string Data { get; private set; }
        public DateTime LastAdvertised { get; private set; }

        public override string ToString()
        {
            return Data;
        }

        protected bool Equals(BeaconLocation other)
        {
            if (other is null)
                throw new ArgumentNullException(nameof(other));

            return Equals(Address, other.Address);
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((BeaconLocation) obj);
        }

        public override int GetHashCode()
        {
            return (Address != null ? Address.GetHashCode() : 0);
        }
    }
}
