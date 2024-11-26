using Serilog;
using Serilog.Parsing;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
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
        private CancellationTokenSource cancellationTokenSource;
        private Task task;

        public SerialPortLayer()
        {
            port = new SerialPort();
        }

        protected override void DoOpen()
        {
            if (port.IsOpen)
                return;

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

            cancellationTokenSource = new();
            task = Task.Run(async() =>
            {
                byte[] buffer = new byte[BlockLimit];

                try
                {
                    while (port.IsOpen)
                    {
                        try
                        {
                            int bytesRead = await port.BaseStream.ReadAsync(buffer, cancellationTokenSource.Token);
                            Destuffer.Add(bytesRead, buffer);
                            BytesReceived += bytesRead;
                        }
                        catch (OperationCanceledException) { throw; }
                        catch (Exception ex) 
                        {
                            if (ECPLog.Enabled)
                                Log.Error("Exception in ECP read task: {exception}", ex);
                            else
                                Log.Verbose("Exception in ECP read task: {exception}", ex);
                        }
                    }
                }
                catch (OperationCanceledException) { }
            });
        }

        protected override void DoClose()
        {
            if (!port.IsOpen)
                return;

            cancellationTokenSource.Cancel();
            task?.Wait();
            port.Close();
        }

        protected override void DoTransmit(byte[] frame)
        {
            if (frame is null)
                return;

            if (port is null)
                return; 

            if (port.IsOpen)
            {
                port.Write(frame, 0, frame.Length);
                BytesTransmitted += frame.Length;
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
