using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;
using Inventors.Logging;
using System.IO.Compression;
using System.Reflection;
using Inventors.ECP.DeviceHost.Actions;

namespace Inventors.ECP.DeviceHost
{
    public class DevicePackage
    {
        public Queue<PackageAction> actions = new Queue<PackageAction>();
        public Queue<PackageAction> completed = new Queue<PackageAction>();

        public string SourceFile { get; }

        public string DestinationDirectory =>
            Path.Combine(Settings.SystemDirectory, Path.GetFileNameWithoutExtension(SourceFile));

        public Loader Loader { get; set; }

        public IHostedDevice Device { get; set; }

        public DevicePackage(string source)
        {
            SourceFile = source;
        }

        public bool Install()
        {
            bool retValue = true;

            Build();

            while (actions.Count > 0)
            {
                var action = actions.Dequeue();

                if (action.Execute())
                {
                    completed.Enqueue(action);
                }
                else
                {
                    retValue = false;
                    break;
                }
            }

            if (!retValue)
            {
                Revert();
            }

            return retValue;
        }

        private void Build()
        {
            actions.Enqueue(new VerifyPackage(this));
            actions.Enqueue(new CreateDirectory(this));
            actions.Enqueue(new ExtractPackage(this));
            actions.Enqueue(new CreateLoader(this));
            actions.Enqueue(new CreateDevice(this));
            actions.Enqueue(new InstallLoader(this));
        }

        private void Revert()
        {
            while (completed.Count > 0)
            {
                completed.Dequeue().Revert();
            }
        }

        public static void Remove(Loader loader)
        {            
            foreach (var file in Directory.EnumerateFiles(loader.BasePath))
            {
                File.Delete(file);
                Log.Debug("File: {0} deleted", file);
            }
            Directory.Delete(loader.BasePath);
            Settings.Devices.Remove(loader);
            Settings.Save();
        }
    }
}
