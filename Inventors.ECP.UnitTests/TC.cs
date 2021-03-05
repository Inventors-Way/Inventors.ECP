using Inventors.ECP.DefaultDevice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.ECP.UnitTests
{
    public class TC : IDisposable
    {
        private bool disposedValue;

        public static string CentralPort => "COM6";

        public static string PeripheralPort => "COM4";

        private static TC _instance;

        private static TC Instance
        {
            get
            {
                if (_instance is null)
                    _instance = new TC();

                return _instance;
            }
        }

        public static DefaultSerialDevice CentralDevice => Instance.Device;

        public static DefaultPeripheral PeripheralDevice => Instance.Peripheral;

        public DefaultSerialDevice Device { get; }

        public DefaultPeripheral Peripheral { get; }

        private TC()
        {
            Device = new DefaultSerialDevice()
            {
                Location = CentralPort,
                BaudRate = 115200,
                Timeout = 300,
                Retries = 3
            };
            Peripheral = new DefaultPeripheral()
            {
                Location = PeripheralPort,
                BaudRate = 115200
            };

            Device.Open();
            Peripheral.Open();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Device.Dispose();
                    Peripheral.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
