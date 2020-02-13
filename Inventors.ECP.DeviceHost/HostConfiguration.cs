using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Inventors.ECP.DeviceHost
{
    [XmlRoot("configuration")]
    public class HostConfiguration
    {
        /// <summary>
        /// Name of the program in the title bar.
        /// </summary>
        [XmlAttribute("title")]
        public string Title { get; set; } = "ECP Device Host";

        /// <summary>
        /// The filename of the icon that will be used for the Notification Icon in the system tray
        /// </summary>
        [XmlAttribute("notify-icon")]
        public string NotifyIconFileName { get; set; } = "";

        /// <summary>
        /// The filename of the icon that will be used for the icon of the main window of the application
        /// </summary>
        [XmlAttribute("icon")]
        public string IconFileName { get; set; } = "";

        /// <summary>
        /// If true then the application will be not be closed but will instead be minimized to tray, when the close on the form is pressed.
        /// </summary>
        [XmlAttribute("close-to-tray")]
        public bool CloseToTray { get; set; } = true;

        /// <summary>
        /// If true then the application will minimize to tray instead of task bar.
        /// </summary>
        [XmlAttribute("minimize-to-tray")]
        public bool MinimizeToTray { get; set; } = false;

        /// <summary>
        /// If NotifyIconFile is set then this will load and return the icon for the Notification Icon in the system tray
        /// </summary>
        [XmlIgnore]
        public Icon NotifyIcon
        {
            get
            {
                return string.IsNullOrEmpty(NotifyIconFileName) ? null : Icon.ExtractAssociatedIcon(NotifyIconFileName);
            }
        }

        [XmlIgnore]
        public Icon Icon
        {
            get
            {
                return string.IsNullOrEmpty(IconFileName) ? null : Icon.ExtractAssociatedIcon(IconFileName);
            }
        }

        [XmlAttribute("product")]
        public string Product { get; set; }

        [XmlAttribute("version")]
        public string Version { get; set; }

        [XmlAttribute("line1")]
        public string Line1 { get; set; }

        [XmlAttribute("line2")]
        public string Line2 { get; set; }

        [XmlAttribute("line3")]
        public string Line3 { get; set; }

        [XmlAttribute("about-image")]
        public string AboutImageFileName { get; set; }

        [XmlIgnore]
        public Image AboutImage => string.IsNullOrEmpty(AboutImageFileName) ? null : Image.FromFile(AboutImageFileName);

        public static HostConfiguration Load()
        {
            return Settings.LoadXML<HostConfiguration>("configuration.xml", true);
        }
    }
}
