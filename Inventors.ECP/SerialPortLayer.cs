using Serilog;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.ECP
{
    public class SerialPortLayer :
        CommunicationLayer,
        IDisposable
    {
        #region IDisposable Support
        private bool disposedValue; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (port is object)
                    {
                        port.Dispose();
                    }
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
        private readonly object lockObject = new object();

        public SerialPortLayer()
        {
            port = new SerialPort();
        }

        protected override void DoOpen()
        {
            lock (lockObject)
            {
                if (port.IsOpen)
                {
                    Close();
                }

                port.PortName = Location;
                port.BaudRate = BaudRate;
                port.Parity = Parity.None;
                port.StopBits = StopBits.One;
                port.DataBits = 8;
                port.Handshake = Handshake.None;
                port.DtrEnable = ResetOnConnection;
                port.ReadTimeout = 10;

                Destuffer.Reset();
                port.Open();
                InitializeRead();
            }
        }

        private void InitializeRead()
        {
            byte[] buffer = new byte[BlockLimit];

            void reader()
            {
                if (port.IsOpen)
                {
                    try
                    {
                        port.BaseStream.BeginRead(buffer, 0, buffer.Length, delegate (IAsyncResult ar)
                        {
                            if (port.IsOpen)
                            {
                                try
                                {
                                    int bytesRead = port.BaseStream.EndRead(ar);
                                    byte[] received = new byte[bytesRead];
                                    Buffer.BlockCopy(buffer, 0, received, 0, bytesRead);
                                    Destuffer.Add(bytesRead, received);
                                    BytesReceived += bytesRead;
                                }
                                catch (Exception e)
                                {
                                    EcpLog.Error(e.Message);
                                }

                                reader();
                            }
                        }, null);
                    }
                    catch (Exception e)
                    {
                        EcpLog.Error(e.Message);
                    }
                }
            }

            reader();
        }

        protected override void DoClose()
        {
            lock (lockObject)
            {
                if (port.IsOpen)
                {
                    port.Close();
                }
            }
        }

        protected override void DoTransmit(byte[] frame)
        {
            lock (lockObject)
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
        }

        public override List<string> GetLocations() => SerialPort.GetPortNames().ToList();

        public override bool IsOpen => port is object && port.IsOpen;

        public override string Location { get; set; }

        public override int BaudRate { get; set; }

        private readonly SerialPort port;
        private readonly int BlockLimit = 1024;
    }
}
