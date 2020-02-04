using Inventors.ECP.DefaultDevice;
using Inventors.ECP;
using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Threading;
using Inventors.ECP.Communication;

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

        public static DefaultTcpSlave Slave => Instance._slave;

        public static DefaultTcpDevice Device => Instance._device;

        private TC()
        {
            _slave = new DefaultTcpSlave()
            {
                Port = new IPEndPoint(TcpServerLayer.LocalAddress, 9000).ToString()
            };
            _device = new DefaultTcpDevice();
            _device.Port = _slave.Port;
            _slave.Start();
            Thread.Sleep(100);
            _device.Connect();
        }

        private static TC _instance;
        private readonly DefaultTcpSlave _slave = null;
        private readonly DefaultTcpDevice _device = null;

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _slave.Stop();
                    _device.Close();
                }

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
