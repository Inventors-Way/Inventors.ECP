﻿using Inventors.Logging;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.ECP.Communication
{
    public class SerialPortLayer :
        CommunicationLayer,
        IDisposable
    {
        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    port.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        public SerialPortLayer(DeviceType device) : 
            base(device) 
        { 
        }

        protected override void DoOpen()
        {
            if (port != null)
            {
                Close();
            }

            if (port == null)
            {
                port = new SerialPort(Port)
                {
                    BaudRate = BaudRate,
                    Parity = Parity.None,
                    StopBits = StopBits.One,
                    DataBits = 8,
                    Handshake = Handshake.None,
                    DtrEnable = ResetOnConnection,
                    ReadTimeout = 10
                };

                Destuffer.Reset();
                port.Open();
                InitializeRead();
            }

            Log.Debug("Serial Port ({0}) opened [Baud = {1} ]", Port, BaudRate);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        private void InitializeRead()
        {
            byte[] buffer = new byte[BlockLimit];
            void reader()
            {
                if (port != null)
                {
                    if (port.IsOpen)
                    {
                        port.BaseStream.BeginRead(buffer, 0, buffer.Length, delegate (IAsyncResult ar)
                        {
                            try
                            {
                                int bytesRead = port.BaseStream.EndRead(ar);
                                byte[] received = new byte[bytesRead];
                                Buffer.BlockCopy(buffer, 0, received, 0, bytesRead);

                                foreach (var b in received)
                                {
                                    Destuffer.Add(b);
                                    ++BytesReceived;
                                }
                            }
                            catch { }

                            reader();
                        }, null);
                    }
                }
            }

            reader();
        }

        protected override void DoClose()
        {
            if (port != null)
            {
                if (port.IsOpen)
                {
                    port.Close();
                    port.Dispose();
                    port = null;
                }
                else
                {
                    port = null;
                }
            }
        }

        public override void Transmit(byte[] frame)
        {
            if ((port is object) && (frame is object))
            {
                if (port.IsOpen)
                {
                    port.Write(frame, 0, frame.Length);
                    BytesTransmitted += frame.Length;
                }
            }
        }

        public override List<string> GetAvailablePorts()
        {
            return SerialPort.GetPortNames().ToList();
        }

        public override bool IsOpen
        {
            get
            {
                bool retValue = false;

                if (port != null)
                    retValue = port.IsOpen;

                return retValue;
            }
        }

        public override string Port { get; set; }

        public override int BaudRate { get; set; }

        private SerialPort port = null;
        private readonly int BlockLimit = 1024;
    }
}
