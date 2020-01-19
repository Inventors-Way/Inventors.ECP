using Microsoft.VisualStudio.TestTools.UnitTesting;
using Inventors.ECP;
using Inventors.ECP.Functions;
using Inventors.ECP.DefaultDevice;
using System.Net;
using Inventors.Logging;
using WatsonTcp;
using System;
using System.Threading.Tasks;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;

namespace Inventors.ECP.UnitTests
{
    [TestClass]
    public class WatsonTest
    {
        private bool _clientConnected = false;
        private bool _serverConnected = false;
        private string _serverMessage = "";
        private string _clientMessage = "";
        private string _ipPort = "";

        public bool IsClientConnected
        {
            get { lock(this) { return _clientConnected; } }
            set { lock(this) { _clientConnected = value; } }
        }

        public string IpPort
        {
            get { lock (this) { return _ipPort; } }
            set { lock (this) { _ipPort = value; } }
        }

        public string ClientMessage
        {
            get { lock (this) { return _clientMessage; } }
            set { lock (this) { _clientMessage = value; } }
        }

        private bool IsServerConnected
        {
            get { lock (this) { return _serverConnected; } }
            set { lock (this) { _serverConnected = value; } }
        }

        public string ServerMessage
        {
            get { lock (this) { return _serverMessage; } }
            set { lock (this) { _serverMessage = value; } }
        }

        private void WaitUntilTrue(Func<bool> test, int timeout)
        {
            Stopwatch watch = new Stopwatch();

            watch.Restart();
            while (watch.ElapsedMilliseconds < 100)
            {
                if (test())
                {
                    break;
                }
            };
        }

        private Stopwatch msgWatch = new Stopwatch();
        private List<int> times = new List<int>();

        [TestMethod]
        public void TestWatson()
        {
            times = new List<int>();
            WatsonTcpServer server = new WatsonTcpServer("127.0.0.1", 9000);
            server.ClientConnected += ClientConnected;
            server.ClientDisconnected += ClientDisconnected;
            server.MessageReceived += ClientMessageReceived;
            server.Start();

            WatsonTcpClient client = new WatsonTcpClient("127.0.0.1", 9000);
            client.ServerConnected = ServerConnected;
            client.ServerDisconnected = ServerDisconnected;
            client.MessageReceived = ServerMessageReceived;
            client.Start();

            WaitUntilTrue(() => IsClientConnected && IsServerConnected, 100);

            Assert.IsTrue(IsServerConnected);
            Assert.IsTrue(IsClientConnected);

            client.Send(Encoding.UTF8.GetBytes("Hello Server"));

            WaitUntilTrue(() => !string.IsNullOrEmpty(ClientMessage), 100);

            Assert.AreEqual("Hello Server", ClientMessage);

            for (int n = 0; n < 10; ++n)
            {
                msgWatch.Restart();
                ServerMessage = "";
                server.Send(IpPort, Encoding.UTF8.GetBytes("Hello Client"));
                WaitUntilTrue(() => !string.IsNullOrEmpty(ServerMessage), 100);
            }

            WaitUntilTrue(() => !string.IsNullOrEmpty(ServerMessage), 100);

            Assert.AreEqual("Hello Client", ServerMessage);

            Assert.IsTrue(times.TrueForAll((e) => e < 50));
        }

        private async Task ClientConnected(string ipPort)
        {
            IpPort = ipPort;
            IsClientConnected = true;
        }

        private async Task ClientDisconnected(string ipPort, DisconnectReason reason)
        {
            IsClientConnected = false;
        }

        private async Task ClientMessageReceived(string ipPort, byte[] data)
        {
            string msg = "";
            if (data != null && data.Length > 0) msg = Encoding.UTF8.GetString(data);
            ClientMessage = msg;
            Console.WriteLine("Message received from " + ipPort + ": " + msg);

        }

        private async Task ServerMessageReceived(byte[] data)
        {
            msgWatch.Stop();
            times.Add((int)msgWatch.ElapsedMilliseconds);

            string msg = "";
            if (data != null && data.Length > 0) msg = Encoding.UTF8.GetString(data);
            ServerMessage = msg;

            Console.WriteLine("Message from server: " + Encoding.UTF8.GetString(data));
        }

        private async Task ServerConnected()
        {
            IsServerConnected = true;
            Console.WriteLine("Server connected");
        }

        private async Task ServerDisconnected()
        {
            IsServerConnected = false;
            Console.WriteLine("Server disconnected");
        }
    }
}
