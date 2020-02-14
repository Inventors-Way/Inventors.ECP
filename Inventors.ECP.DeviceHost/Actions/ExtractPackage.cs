using Inventors.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.ECP.DeviceHost.Actions
{
    public class ExtractPackage :
        PackageAction
    {
        public ExtractPackage(DevicePackage package) : base(package) { }

        public override bool Execute()
        {
            bool retValue = false;

            try
            {
                ZipFile.ExtractToDirectory(Package.SourceFile, Package.DestinationDirectory);
                Log.Status("Extracted device package [ {0} ]", Path.GetFileName(Package.SourceFile));
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
            try
            {
                foreach (var file in Directory.EnumerateFiles(Package.DestinationDirectory))
                {
                    File.Delete(file);
                    Log.Debug("File: {0} deleted", file);
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }
    }
}
