using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Inventors.ECP.Hosting
{
    [XmlRoot("device")]
    public class HostedDeviceLoader
    {
        [XmlAttribute("assembly")]
        public string AssemblyName { get; set; }

        [XmlAttribute("device-type")]
        public string DeviceType { get; set; }

        [XmlAttribute("device")]
        public string Device { get; set; }

        [XmlIgnore]
        public string FileName { get; private set; }

        public static HostedDeviceLoader Load(string filename)
        {
            HostedDeviceLoader retValue = null;
            XmlSerializer serializer = new XmlSerializer(typeof(HostedDeviceLoader));
            XmlReaderSettings settings = new XmlReaderSettings() { };

            using (var file = File.Open(filename, FileMode.Open, FileAccess.Read))
            {
                using (var reader = XmlReader.Create(file, settings))
                {
                    retValue = (HostedDeviceLoader)serializer.Deserialize(reader);
                    retValue.FileName = filename;
                }
            }

            return retValue;
        }

        public IHostedDevice Create()
        {
            IHostedDevice retValue = null;
            string basePath = Path.GetDirectoryName(FileName);
            string fullAssemblyName = Path.Combine(basePath, AssemblyName) + ".dll";
            var assembly = Assembly.LoadFrom(fullAssemblyName);
            var type = assembly.GetType(DeviceType);

            if (type is object)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(HostedDeviceLoader));
                XmlReaderSettings settings = new XmlReaderSettings() { };
                var deviceFile = Path.Combine(basePath, Device);

                using (var file = File.Open(deviceFile, FileMode.Open, FileAccess.Read))
                {
                    using (var reader = XmlReader.Create(file, settings))
                    {
                        retValue = (IHostedDevice) serializer.Deserialize(reader);
                        retValue.DeviceFile = deviceFile;
                    }
                }
            }

            return retValue;
        }

    }
}
