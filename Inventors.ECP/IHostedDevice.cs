using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Inventors.ECP
{
    public enum DeviceState
    {
        [XmlEnum("stopped")]
        STOPPED = 0,
        [XmlEnum("running")]
        RUNNING
    }

    public interface IHostedDevice
    {

        event Action<object, string> OnPropertyChanged;

        string ID { get; set; }

        string Port { get; }

        DeviceState State { get; }

        string DeviceFile { get; set; }

        void Run();

        void Stop();
    }
}
