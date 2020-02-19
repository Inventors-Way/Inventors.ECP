using Inventors.ECP.Communication.Discovery;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inventors.ECP
{
    /// <summary>
    /// Responsible for finding network devices (Singleton)
    /// </summary>
    public class DeviceLocator
    {
        private DeviceLocator _instance = null;

        private DeviceLocator Instance
        {
            get
            {
                if (_instance is null)
                {
                    _instance = new DeviceLocator();
                }

                return _instance;
            }
        }

        private DeviceLocator()
        {

        }

        public static List<Location> GetAvailableLocations() => throw new NotImplementedException();

        internal static void Register(BeaconID id) => throw new NotImplementedException();
    }
}
