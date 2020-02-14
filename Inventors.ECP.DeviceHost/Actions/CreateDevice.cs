using Inventors.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.ECP.DeviceHost.Actions
{
    class CreateDevice :
        PackageAction
    {
        public CreateDevice(DevicePackage package) : base(package) { }

        public override bool Execute()
        {
            bool retValue = false;

            if (Package.Loader is object)
            {
                try
                {
                    Package.Device = Package.Loader.Create();
                    Log.Status("Device created: {0}", Package.Device.ToString());
                    retValue = true;
                }
                catch (Exception e)
                {
                    Log.Error(e.Message);
                }
            }

            return retValue;
        }

        public override void Revert() 
        {
            try
            {
                if (Package.Device is object)
                {
                    if (Package.Device.State == DeviceState.RUNNING)
                    {
                        Package.Device.Stop();
                    }

                    Package.Device = null;
                    Package.Loader.Uncreate();
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }
    }
}
