﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Inventors.ECP
{
    [XmlRoot("device")]
    public class DeviceLoader
    {
        [XmlAttribute("assembly")]
        public string AssemblyName { get; set; }

        [XmlAttribute("factory")]
        public string Factory { get; set; }

        [XmlAttribute("baudrate")]
        public int BaudRate { get; set; }

        [XmlAttribute("port")]
        public string PortName { get; set; }

        [XmlAttribute("profiling")]
        public bool Profiling { get; set; }

        [XmlAttribute("number-of-trials")]
        public int Trials { get; set; }

        [XmlAttribute("test-delay")]
        public int TestDelay { get; set; }

        [XmlIgnore]
        public string FileName { get; private set; } = null;

        public static DeviceLoader Load(string filename)
        {
            DeviceLoader retValue = null;
            XmlSerializer serializer = new XmlSerializer(typeof(DeviceLoader));

            using (var reader = File.Open(filename, FileMode.Open, FileAccess.Read))
            {
                retValue = (DeviceLoader)serializer.Deserialize(reader);
            }
            retValue.FileName = filename;

            return retValue;
        }

        public Device Create(CommunicationLayer layer)
        {
            Device retValue = null;
            string basePath = Path.GetDirectoryName(FileName);
            string fullAssemblyName = Path.Combine(basePath, AssemblyName) + ".dll";
            var assembly = Assembly.LoadFrom(fullAssemblyName);
            var type = assembly.GetType(Factory);

            if (type != null)
            {
                retValue = (Device) Activator.CreateInstance(type, new object[] { layer });
                layer.BaudRate = BaudRate;
            }

            return retValue;
        }
    }
}
