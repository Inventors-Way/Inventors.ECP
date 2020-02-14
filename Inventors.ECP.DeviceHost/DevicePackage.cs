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
            Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Path.GetFileNameWithoutExtension(SourceFile));

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
            /*
            if (VerifyChecksum())
            {
                if (CreateDirectory())
                {
                    if (ExtractPackage())
                    {
                        if (CreateLoader())
                        {
                            if (CreateDevice())
                            {
                                retValue = true;
                            }
                        }
                    }
                }
            }*/

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

        private bool VerifyChecksum()
        {
            Log.Status("Device Package Checksum: {0} (SHA-256)", CalculateChecksum());
            return true;
        }

        private bool CreateDirectory()
        {
            var retValue = false;

            if (!Directory.Exists(DestinationDirectory))
            {
                Directory.CreateDirectory(DestinationDirectory);
                Log.Debug("Created: {0}", DestinationDirectory);
                retValue = true;
            }
            else
            {
                Log.Error("Directory [ {0} ] allready exists, please uinstall device first", DestinationDirectory);
            }

            return retValue;
        }

        private bool ExtractPackage()
        {
            bool retValue = false;

            try
            {
                ZipFile.ExtractToDirectory(SourceFile, DestinationDirectory);
                Log.Status("Extracted device package [ {0} ] to: {1}", Path.GetFileName(SourceFile), DestinationDirectory);
                retValue = true;
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }

            return retValue;
        }

        private bool CreateLoader()
        {
            bool retValue = false;

            try
            {
                Loader = Loader.Load(Path.Combine(DestinationDirectory, "loader.xml"));
                Log.Status("Loader created: {0}", Loader.AssemblyName);
                retValue = true;
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }

            return retValue;
        }

        private bool CreateDevice()
        {
            bool retValue = false;

            if (Loader is object)
            {
                try
                {
                    Device = Loader.Create();
                    Log.Status("Device created: {0}", Device.ToString());
                    retValue = true;
                }
                catch (Exception e)
                {
                    Log.Error(e.Message);
                }
            }

            return retValue;
        }

        private string CalculateChecksum()
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                using (var file = File.Open(SourceFile, FileMode.Open))
                {
                    return Bytes2String(sha256.ComputeHash(file));
                }
            }
        }

        public static string Bytes2String(byte[] array)
        {
            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < array.Length; i++)
            {
                builder.AppendFormat($"{array[i]:X2}");
                if ((i % 2) == 1)
                {
                    builder.AppendFormat(" ");
                }
            }

            return builder.ToString();
        }
    }
}
