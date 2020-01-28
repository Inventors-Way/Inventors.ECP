﻿using Inventors.ECP.DefaultDevice;
using Inventors.ECP;
using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace Inventors.ECP.UnitTests
{
    public class TcpTestContext : 
        IDisposable
    {
        private static TcpTestContext Instance
        {
            get
            {
                if (!(_instance is object))
                {
                    _instance = new TcpTestContext();
                }

                return _instance;
            }
        }

        public static DefaultTcpSlave Slave => Instance._slave;

        public static DefaultDevice.DefaultDevice Device => Instance._device;

        private TcpTestContext()
        {
            _slave = new DefaultTcpSlave()
            {
                Address = IPAddress.Loopback.ToString(),
                Port = 10000
            };
            var layer = new TcpClientLayer()
            {
                Address = _slave.Address,
                Port = _slave.Port
            };
            _device = new DefaultDevice.DefaultDevice(layer);
            _slave.Start();
            _device.Open();
        }

        private static TcpTestContext _instance;
        private DefaultTcpSlave _slave = null;
        private DefaultDevice.DefaultDevice _device = null;

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