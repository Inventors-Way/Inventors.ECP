using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using Inventors.ECP.Communication.Discovery;
using System.Threading;

namespace Inventors.ECP.UnitTests.Utility
{
    [TestClass]
    public class DiscoveryTest
    {
        [TestMethod]
        public void Beacon()
        {
            var beaconId = new BeaconID(Manufacturer.InnoCon, 1, 10);
            var probeId = new BeaconID(Manufacturer.InnoCon, 1);

            using (var beacon = new Beacon(beaconId, 9001))
            {
                using (var probe = new Probe(probeId))
                {
                    List<BeaconLocation> b = new List<BeaconLocation>();
                    probe.BeaconsUpdated += (beacons) =>
                    {
                        b.Clear();
                        b.AddRange(beacons);
                    };
                    beacon.Start();
                    probe.Start();
                    Thread.Sleep(500);
                    Assert.AreEqual(expected: 1, actual: b.Count);
                    Assert.AreEqual(expected: "10", actual: b[0].Data);
                }
            }

        }
    }
}
