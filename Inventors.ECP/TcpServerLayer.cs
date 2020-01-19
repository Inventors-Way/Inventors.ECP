using Inventors.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Inventors.ECP
{
    public class TcpServerLayer :
        CommunicationLayer
    {
        private Socket _listener = null;
        private Socket _socket = null;

        private bool _isopen = false;
        private readonly byte[] buffer = new byte[UInt16.MaxValue];

        public override int BaudRate { get; set; } = 1;

        public int Port { get; set; } = 30001;

        public string Address { get; set; } = GetLocalAddress();

        public TcpServerLayer()
        {

        }

        public override bool IsOpen
        {
            get
            {
                lock (this)
                {
                    return _isopen;
                }
            }
        }

        public override void Transmit(byte[] frame)
        {
            if (IsOpen && (_socket != null))
            {
                if (_socket.Connected)
                {
                    _socket.BeginSend(frame, 0, frame.Length, SocketFlags.None, delegate (IAsyncResult ar)
                    {
                        try
                        {
                            _listener.EndSend(ar);
                        }
                        catch (Exception e)
                        {
                            Log.Error(e.Message);
                        }
                    }, null);
                }
            }
        }

        protected override void DoClose()
        {
            if (IsOpen)
            {
                lock (this)
                {
                    _listener.Close();
                    _listener = null;

                    if (_socket is object)
                    {
                        _socket.Shutdown(SocketShutdown.Both);
                        _socket.Close();
                        _socket = null;
                    }

                    _isopen = false;
                }
            }            
        }

        protected override void DoOpen()
        {
            if (!IsOpen)
            {                
                lock (this)
                {
                    _listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    _listener.Bind(new IPEndPoint(IPAddress.Parse(Address), Port));
                    _listener.Listen(10);
                    _listener.BeginAccept(OnConnectionReady, null);

                    _isopen = true;
                    _socket = null;
                }
            }
        }

        private void OnConnectionReady(IAsyncResult ar)
        {
            lock (this)
            {
                if (_listener == null) return;
                Socket conn = _listener.EndAccept(ar);

                lock (this)
                {
                    if (_socket != null)
                    {
                        conn.Shutdown(SocketShutdown.Both);
                        conn.Close();

                        Log.Debug("Connection attempted but we are allready connected");
                    }
                    else
                    {
                        _socket = conn;
                        _socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, OnReceivedData, null);
                    }

                    _listener.BeginAccept(OnConnectionReady, null);
                }
            }
        }

        private void OnReceivedData(IAsyncResult ar)
        {
            if (_socket is object)
            {
                try
                {
                    var bytesReceived = _socket.EndReceive(ar);
                    byte[] received = new byte[bytesReceived];
                    Buffer.BlockCopy(buffer, 0, received, 0, bytesReceived);

                    foreach (var b in received)
                    {
                        Destuffer.Add(b);
                        ++bytesReceived;
                    }

                    _socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, OnReceivedData, null);
                }
                catch (SocketException se)
                {
                    Log.Debug(se.Message);
                }
            }
        }

        public static string GetLocalAddress()
        {
            IPHostEntry localhost = Dns.GetHostEntry(Dns.GetHostName());
 
            foreach (IPAddress address in localhost.AddressList)
            {
                // Look for the IPv4 address of the local machine
                if (address.AddressFamily.ToString() == "InterNetwork")
                {
                    // Convert the IP address to a string and return it
                    return address.ToString();
                }
            }

            return null;
        }
    }
}
