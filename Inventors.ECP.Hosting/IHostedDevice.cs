﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.ECP.Hosting
{
    public interface IHostedDevice
    {
        string ID { get; set; }

        DeviceState State { get; }

        string DeviceFile { get; set; }

        void Run();

        void Stop();
    }
}
