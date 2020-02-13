using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.ECP.Hosting
{
    public interface IHostedDevice
    {
        event Action<string> OnPropertyChanged;

        string ID { get; set; }

        string Port { get; }

        DeviceState State { get; }

        string DeviceFile { get; set; }

        void Run();

        void Stop();
    }
}
