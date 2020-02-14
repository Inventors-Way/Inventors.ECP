using Inventors.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.ECP.DeviceHost.Actions
{
    public class CreateDirectory :
        PackageAction
    {
        public CreateDirectory(DevicePackage package) : base(package) { }

        public override bool Execute()
        {
            var retValue = false;

            if (!Directory.Exists(Package.DestinationDirectory))
            {
                Directory.CreateDirectory(Package.DestinationDirectory);
                Log.Debug("Created: {0}", Package.DestinationDirectory);
                retValue = true;
            }
            else
            {
                Log.Error("Directory [ {0} ] allready exists, please uinstall device first", Package.DestinationDirectory);
            }

            return retValue;
        }

        public override void Revert()
        {
            try
            {
                if (Directory.Exists(Package.DestinationDirectory))
                {
                    Directory.Delete(Package.DestinationDirectory);
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }
    }
}
