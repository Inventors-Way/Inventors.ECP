﻿using Inventors.ECP.Logging;
using Serilog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
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

        [XmlIgnore]
        public string Version { get; private set; } = "";

        [XmlAttribute("baudrate")]
        public string BaudRateString
        {
            get
            {
                return BaudRate.ToString(CultureInfo.CurrentCulture);
            }
            set
            {
                if (int.TryParse(value, out int result))
                {
                    BaudRate = result;
                }
            }
        }

        [XmlIgnore]
        public int BaudRate { get; set; } = -1;

        [XmlAttribute("profiling")]
        public bool Profiling { get; set; }

        [XmlAttribute("number-of-trials")]
        public int Trials { get; set; }

        [XmlAttribute("test-delay")]
        public int TestDelay { get; set; }

        [XmlAttribute("confirm-log-deletion")]
        public bool ConfirmLogDeletion { get; set; }

        [XmlAttribute("auto-save-log")]
        public bool AutoSaveLog { get; set; }

        [XmlArray("debug-specifications")]
        [XmlArrayItem("debug-specification")]
        public List<DebugSpecification> DebugSpecifications { get; } = new List<DebugSpecification>();

        [XmlArray("analysis-specification")]
        [XmlArrayItem("analysis")]
        public List<MessageAnalyser> Analysers { get; } = new List<MessageAnalyser>();

        [XmlArray("actions")]
        [XmlArrayItem("action")]
        public List<CustomAction> Actions { get; } = new List<CustomAction>();

        [XmlElement("basic-logging", typeof(BasicLogging))]
        [XmlElement("seq-logging", typeof(SeqLogging))]
        public LogConfig LogConfiguration { get; set; } = new BasicLogging();


        [XmlIgnore]
        public string CreationTime { get; set; } = "Unknown";

        [XmlIgnore]
        public string FileName { get; private set; } = null;

        public static DeviceLoader Load(string filename)
        {
            DeviceLoader retValue = null;
            XmlSerializer serializer = new XmlSerializer(typeof(DeviceLoader));
            XmlReaderSettings settings = new XmlReaderSettings() { };
            
            using (var file = File.Open(filename, FileMode.Open, FileAccess.Read))
            {
                using (var reader = XmlReader.Create(file, settings))
                {
                    retValue = (DeviceLoader)serializer.Deserialize(reader);
                }
            }
            retValue.FileName = filename;
            retValue.CreationTime = File.GetLastWriteTime(filename).ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

            return retValue;
        }

        public Device Create()
        {
            Device retValue = null;
            string basePath = Path.GetDirectoryName(FileName);
            string fullAssemblyName = Path.Combine(basePath, AssemblyName) + ".dll";
            assembly = Assembly.LoadFrom(fullAssemblyName);
            var type = assembly.GetType(Factory);
            Version = assembly.GetName().Version.ToString();  

            if ((type is object))
            {
                retValue = (Device) Activator.CreateInstance(type);

                if (BaudRate > 0)
                {
                    retValue.BaudRate = BaudRate;
                    Log.Debug("Baudrate set to: {0}", BaudRate);
                }


                if (DebugSpecifications.Count > 0)
                {
                    DebugSpecifications.ForEach((s) => retValue.AddDebugSpecification(s));
                }
            }

            return retValue;
        }

        [XmlIgnore]
        public Assembly Library => assembly;

        private Assembly assembly;
    }
}
