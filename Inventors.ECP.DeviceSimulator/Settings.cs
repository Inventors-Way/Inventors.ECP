using Inventors.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Inventors.ECP.DeviceSimulator
{
    public enum SimulatorType
    {
        SERIAL = 0,
        TCP
    }

    public enum BaudRate
    {
        _9600,
        _14400,
        _19200,
        _38400,
        _57600,
        _115200
    }

    public class Settings
    {
        private SettingsFile _file = null;
        private static Settings _instance = null;

        private static Settings Instance
        {
            get
            {
                if (_instance is null)
                {
                    _instance = new Settings();
                }

                return _instance;
            }
        }

        private Settings()
        {
            if (!Directory.Exists(SystemDirectory))
            {
                Directory.CreateDirectory(SystemDirectory);
            }

            Load();
        }

        [XmlRoot("settings")]
        public class SettingsFile
        {
            [XmlAttribute("level")]
            public LogLevel Level { get; set; } = LogLevel.STATUS;

            [XmlAttribute("simulator-type")]
            public SimulatorType Simulator { get; set; } = SimulatorType.SERIAL;

            [XmlAttribute("port")]
            public string Port { get; set; }

            [XmlAttribute("baudrate")]
            public BaudRate BaudRate { get; set; }
        }

        private static string SystemDirectory
        {
            get
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ecp_simulator");
            }
        }

        private static string StateFile
        {
            get
            {
                return Path.Combine(SystemDirectory, "settings.xml");
            }
        }

        public static LogLevel Level
        {
            get => Instance.Load().Level;
            set
            {
                if (value != Instance.Load().Level)
                {
                    Instance.Load().Level = value;
                    Instance.Save();
                }
            }
        }

        public static SimulatorType Simulator
        {
            get => Instance.Load().Simulator;
            set
            {
                if (value != Instance.Load().Simulator)
                {
                    Instance.Load().Simulator = value;
                    Instance.Save();
                }
            }
        }

        public static string Port
        {
            get => Instance.Load().Port;
            set
            {
                if (value != Instance.Load().Port)
                {
                    Instance.Load().Port = value;
                    Instance.Save();
                }
            }
        }

        public static BaudRate BaudRate
        {
            get => Instance.Load().BaudRate;
            set
            {
                if (value != Instance.Load().BaudRate)
                {
                    Instance.Load().BaudRate = value;
                    Instance.Save();
                }
            }
        }

        private SettingsFile Load()
        {
            if (_file is null)
            {
                _file = LoadXML<SettingsFile>(StateFile);
            }

            return _file;
        }

        private void Save()
        {
            if (_file is null)
            {
                Load();
            }

            SaveXML<SettingsFile>(StateFile, _file);
        }

        #region Handling of saving and loading xml data

        public static T LoadXML<T>(string filename, bool create = true)
            where T : class, new()
        {
            T retValue = null;

            if (File.Exists(filename))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));

                using (var reader = new StreamReader(filename))
                {
                    retValue = (T)serializer.Deserialize(reader);

                    if (retValue == null)
                    {
                        if (create)
                        {
                            retValue = new T();
                        }
                    }
                }
            }
            else
            {
                if (create)
                {
                    retValue = new T();
                    SaveXML<T>(filename, retValue);
                }
            }

            return retValue;
        }

        public static void SaveXML<T>(string filename, T instance)
            where T : class
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));

            using (var writer = new StreamWriter(filename))
            {
                serializer.Serialize(writer, instance);
            }
        }

        #endregion
    }
}
