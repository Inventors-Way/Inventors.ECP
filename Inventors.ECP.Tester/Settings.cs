﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Inventors.ECP.Tester
{
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

        public class StartupDirectory
        {
            [XmlAttribute("id")]
            public string ID { get; set; }

            [XmlAttribute("startup-path")]
            public string StartupPath { get; set; }
        }

        [XmlRoot("settings")]
        public class SettingsFile
        {
            [XmlAttribute("level")]
            public LogLevel Level { get; set; } = LogLevel.STATUS;

            [XmlArray("logging-directories")]
            [XmlArrayItem("directory")]
            public List<StartupDirectory> LoggingDirectories { get; } = new List<StartupDirectory>();
        }

        private static string SystemDirectory => 
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ecp_tester");

        private static string LoggingDirectory =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ECP Logs");

        public static string GetDeviceDefaultLoggingDirectory(string id)
        {
            var directory = Path.Combine(LoggingDirectory, id);
            CheckLoggingDirectory(directory);
            return directory;
        }

        private static void CheckLoggingDirectory(string directory)
        {
            if (!Directory.Exists(LoggingDirectory))
                Directory.CreateDirectory(LoggingDirectory);

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
        }

        private static string StateFile => Path.Combine(SystemDirectory, "settings.xml");

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

        #region Handle logging directories

        public static string GetLoggingDirectory(string id)
        {
            var file = Instance.Load();
            var directory = GetLoggingDirectory(id);

            if (file.LoggingDirectories.Any((d) => d.ID == id))
            {

            }
            else
            {

            }

            return directory;
        }

        #endregion

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
