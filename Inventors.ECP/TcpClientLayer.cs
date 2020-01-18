using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Inventors.ECP
{
    public class TcpClientLayer :
        CommunicationLayer
    {
        private readonly Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private bool _isopen = false;
        private readonly byte[] rxBuffer = new byte[UInt16.MaxValue];

        public override int BaudRate { get; set; } = 1;

        public string Address { get; set; }

        public int Port { get; set; }

        public override bool IsOpen => _isopen;

        public override void Transmit(byte[] frame)
        {
            if (IsOpen)
            {
                socket.BeginSend(frame, 0, frame.Length, SocketFlags.None, delegate (IAsyncResult ar)
                {
                    socket.EndSend(ar);
                }, null);
            }            
        }

        private void InitializeRead()
        {
            Action reader = null;
            reader = delegate {
                if (socket!= null)
                {
                    if (socket.Connected && IsOpen)
                    {
                        socket.BeginReceive(rxBuffer, 0, rxBuffer.Length, SocketFlags.None, delegate (IAsyncResult ar)
                        {
                            try
                            {
                                int bytesRead = socket.EndReceive(ar);
                                byte[] received = new byte[bytesRead];
                                Buffer.BlockCopy(rxBuffer, 0, received, 0, bytesRead);

                                foreach (var b in received)
                                {
                                    Destuffer.Add(b);
                                    ++bytesReceived;
                                }
                            }
                            catch { }

                            reader();
                        }, null);
                    }
                }
            };

            reader();
        }

        protected override void DoClose()
        {
            if (IsOpen)
            {
                socket.Disconnect(true);
                _isopen = false;
            }
        }

        protected override void DoOpen()
        {
            if (!IsOpen)
            {
                destuffer.Reset();
                socket.Connect(IPAddress.Parse(Address), Port);
                _isopen = true;
                InitializeRead();
            }
        }
    }
}
