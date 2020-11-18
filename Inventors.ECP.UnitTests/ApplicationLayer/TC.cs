using Inventors.ECP.DefaultDevice;
using Inventors.ECP;
using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Threading;

namespace Inventors.ECP.UnitTests.ApplicationLayer
{
    public class TC : 
        IDisposable
    {
        private static TC Instance
        {
            get
            {
                if (!(_instance is object))
                {
                    _instance = new TC();
                }

                return _instance;
            }
        }

        public static DefaultSerialDevice Device => Instance._device;

        private TC()
        {
            _device = new DefaultSerialDevice() { Location = "COM14" };
        }

        private static TC _instance;
        private bool disposedValue;
        private readonly DefaultSerialDevice _device;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (_device is object)
                    {
                        _device.Dispose();
                    }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~TC()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
