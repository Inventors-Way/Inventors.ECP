using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Inventors.ECP.DeviceHost
{
    [XmlRoot("loader")]
    public class Loader
    {
        [XmlAttribute("id")]
        public string ID { get; set; }

        [XmlAttribute("assembly")]
        public string AssemblyName { get; set; }

        [XmlAttribute("device-type")]
        public string DeviceType { get; set; }

        [XmlIgnore]
        public string Device => "device.xml";

        [XmlAttribute("state")]
        public DeviceState State { get; set; }

        [XmlAttribute("basepath")]
        public string BasePath { get; set; }

        [XmlAttribute("marked-for-removal")]
        public bool RemoveAtStart { get; set; }

        [XmlIgnore]
        public string AssemblyPath => Path.Combine(BasePath, AssemblyName) + ".dll";

        [XmlIgnore]
        public string DevicePath => Path.Combine(BasePath, Device);

        public static Loader Load(string filename)
        {
            Loader retValue = null;
            XmlSerializer serializer = new XmlSerializer(typeof(Loader));
            XmlReaderSettings settings = new XmlReaderSettings() { };

            using (var file = File.Open(filename, FileMode.Open, FileAccess.Read))
            {
                using (var reader = XmlReader.Create(file, settings))
                {
                    retValue = (Loader)serializer.Deserialize(reader);
                    retValue.BasePath = Path.GetDirectoryName(filename);
                    retValue.ID = Guid.NewGuid().ToString();
                }
            }

            return retValue;
        }

        public IHostedDevice Create()
        {
            IHostedDevice retValue = null;
            assembly = Assembly.LoadFrom(AssemblyPath);
            type = assembly.GetType(DeviceType);

            if (type is object)
            {
                XmlSerializer serializer = new XmlSerializer(type);
                XmlReaderSettings settings = new XmlReaderSettings() { };

                using (var file = File.Open(DevicePath, FileMode.Open, FileAccess.Read))
                {
                    using (var reader = XmlReader.Create(file, settings))
                    {
                        retValue = (IHostedDevice) serializer.Deserialize(reader);
                        retValue.DeviceFile = DevicePath;
                        retValue.ID = ID;
                    }
                }
            }

            return retValue;
        }

        public void Uncreate()
        {

        }

        private AppDomain domain;
        private Assembly assembly = null;
        private Type type = null;
    }
}
