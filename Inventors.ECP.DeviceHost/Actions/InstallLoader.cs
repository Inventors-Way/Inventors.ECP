using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.ECP.DeviceHost.Actions
{
    public class InstallLoader :
        PackageAction
    {
        public InstallLoader(DevicePackage package) : base(package) { }

        public override bool Execute()
        {
            bool retValue = false;

            try
            {
                Settings.Devices.Add(Package.Loader);
                Settings.Save();
                retValue = true;
            }
            catch { }

            return retValue;
        }

        public override void Revert() => DeInstall(Package.Loader);

        public static void DeInstall(Loader loader)
        {
            if (loader is object)
            {
                if (Settings.Devices.Any((l) => l.ID == loader.ID))
                {
                    loader.RemoveAtStart = true;
                    Settings.Save();
                }
            }
        }
    }
}
