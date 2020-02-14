using Inventors.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.ECP.DeviceHost.Actions
{
    public class CreateLoader :
        PackageAction
    {
        public CreateLoader(DevicePackage package) : base(package) { }

        public override bool Execute()
        {
            bool retValue = false;

            try
            {
                Package.Loader = Loader.Load(Path.Combine(Package.DestinationDirectory, "loader.xml"));
                Log.Status("Loader created: {0}", Package.Loader.AssemblyName);
                retValue = true;
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }

            return retValue;
        }

        public override void Revert() 
        { 
            if (Package.Loader is object)
            {

            }
        } 
    }
}
