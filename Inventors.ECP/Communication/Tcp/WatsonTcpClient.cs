using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Inventors.ECP.Communication.Tcp.Message;

namespace Inventors.ECP.Communication.Tcp
{
    /// <summary>
    /// Watson TCP client, with or without SSL.
    /// </summary>
    public class WatsonTcpClient : IDisposable
    {
        #region Public-Members
         
        /// <summary>
        /// Buffer size to use when reading input and output streams.  Default is 65536.
        /// </summary>
        public int StreamBufferSize
        {
            get
            {
                return _ReadStreamBufferSize;
            }
            set
            {
                if (value < 1) throw new ArgumentException("Read stream buffer size must be greater than zero.");
                _ReadStreamBufferSize = value;
            }
        }

        /// <summary>
        /// Enable or disable console debugging.
        /// </summary>
        public bool Debug = false;

        /// <summary>
        /// Use of 'MessageReceived' is exclusive and cannot be used with 'StreamReceived'. 
        /// If receiving messages with metadata, 'MessageReceivedWithMetadata' must be set and 'StreamReceivedWithMetadata' cannot be used.
        /// This callback is called when a message is received from the server.
        /// The entire message payload is passed to your application in a byte array.
        /// </summary>
        public Func<byte[], Task> MessageReceived
        {
            get
            {
                return _MessageReceived;
            }
            set
            {
                _MessageReceived = value;
            }
        }

        /// <summary>
        /// Function called when the client successfully connects to the server.
        /// </summary>
        public Func<Task> ServerConnected = null;

        /// <summary>
        /// Function called when the client disconnects from the server.
        /// </summary>
        public Func<Task> ServerDisconnected = null;

        /// <summary>
        /// Indicates whether or not the client is connected to the server.
        /// </summary>
        public bool Connected { get; private set; }

        /// <summary>
        /// The number of seconds to wait before timing out a connection attempt.  Default is 5 seconds.
        /// </summary>
        public int ConnectTimeoutSeconds
        {
            get
            {
                return _ConnectTimeoutSeconds;
            }
            set
            {
                if (value < 1) throw new ArgumentException("ConnectTimeoutSeconds must be greater than zero.");
                _ConnectTimeoutSeconds = value;
            }
        }

        #endregion Public-Members

        #region Private-Members
         
        private int _ReadStreamBufferSize = 65536;
        private int _ConnectTimeoutSeconds = 5;
        private string _SourceIp;
        private int _SourcePort;
        private string _ServerIp;
        private int _ServerPort;
        private TcpClient _Client = null;
        private NetworkStream _TcpStream = null;

        private SemaphoreSlim _WriteLock = new SemaphoreSlim(1);
        private SemaphoreSlim _ReadLock = new SemaphoreSlim(1);

        private CancellationTokenSource _TokenSource = new CancellationTokenSource();
        private CancellationToken _Token;

        private Func<byte[], Task> _MessageReceived = null;

        #endregion Private-Members

        #region Constructors-and-Factories

        /// <summary>
        /// Initialize the Watson TCP client without SSL.  Call Start() afterward to connect to the server.
        /// </summary>
        /// <param name="serverIp">The IP address or hostname of the server.</param>
        /// <param name="serverPort">The TCP port on which the server is listening.</param>
        public WatsonTcpClient(
            string serverIp,
            int serverPort)
        {
            if (String.IsNullOrEmpty(serverIp)) throw new ArgumentNullException(nameof(serverIp));
            if (serverPort < 1) throw new ArgumentOutOfRangeException(nameof(serverPort));
             
            _Token = _TokenSource.Token;
            _ServerIp = serverIp;
            _ServerPort = serverPort;  
        }


        #endregion Constructors-and-Factories

        #region Public-Methods

        /// <summary>
        /// Tear down the client and dispose of background workers.
        /// </summary>
        public void Dispose()
        {
            Log("Disposing WatsonTcpClient");
              
            if (Connected)
            {
                WatsonMessage msg = new WatsonMessage();
                msg.Status = MessageStatus.Disconnecting;
                msg.Data = null;
                msg.ContentLength = 0;
                MessageWrite(msg);
            }

            if (_TokenSource != null)
            {
                if (!_TokenSource.IsCancellationRequested) _TokenSource.Cancel();
                _TokenSource.Dispose();
                _TokenSource = null;
            }

            if (_WriteLock != null)
            {
                _WriteLock.Dispose();
                _WriteLock = null;
            }

            if (_ReadLock != null)
            {
                _ReadLock.Dispose();
                _ReadLock = null;
            }
                         
            if (_TcpStream != null)
            { 
                _TcpStream.Close();
                _TcpStream.Dispose();
                _TcpStream = null;
            }
             
            if (_Client != null)
            {
                _Client.Close();
                _Client.Dispose();
                _Client = null; 
            } 

            Connected = false; 
            Log("Dispose routine complete");
        }

        /// <summary>
        /// Start the client and establish a connection to the server.
        /// </summary>
        public void Start()
        {
            _Client = new TcpClient();
            IAsyncResult asyncResult = null;
            WaitHandle waitHandle = null;
            bool connectSuccess = false;

            Log("Watson TCP client connecting to " + _ServerIp + ":" + _ServerPort);

            _Client.LingerState = new LingerOption(true, 0);
            asyncResult = _Client.BeginConnect(_ServerIp, _ServerPort, null, null);
            waitHandle = asyncResult.AsyncWaitHandle;

            try
            {
                connectSuccess = waitHandle.WaitOne(TimeSpan.FromSeconds(_ConnectTimeoutSeconds), false);
                if (!connectSuccess)
                {
                    _Client.Close();
                    throw new TimeoutException("Timeout connecting to " + _ServerIp + ":" + _ServerPort);
                }

                _Client.EndConnect(asyncResult);

                _SourceIp = ((IPEndPoint)_Client.Client.LocalEndPoint).Address.ToString();
                _SourcePort = ((IPEndPoint)_Client.Client.LocalEndPoint).Port;
                _TcpStream = _Client.GetStream();

                Connected = true;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                waitHandle.Close();
            }

            if (ServerConnected != null)
            {
                Task serverConnected = Task.Run(() => ServerConnected());
            }

            Task dataReceiver = Task.Run(() => DataReceiver(), _Token);
        }

        /// <summary>
        /// Start the client and establish a connection to the server.
        /// </summary>
        /// <returns></returns>
        public Task StartAsync()
        {
            _Client = new TcpClient();
            IAsyncResult asyncResult = null;
            WaitHandle waitHandle = null;
            bool connectSuccess = false;

                Log("Watson TCP client connecting to " + _ServerIp + ":" + _ServerPort);

                _Client.LingerState = new LingerOption(true, 0);
                asyncResult = _Client.BeginConnect(_ServerIp, _ServerPort, null, null);
                waitHandle = asyncResult.AsyncWaitHandle;

                try
                {
                    connectSuccess = waitHandle.WaitOne(TimeSpan.FromSeconds(_ConnectTimeoutSeconds), false);
                    if (!connectSuccess)
                    {
                        _Client.Close();
                        throw new TimeoutException("Timeout connecting to " + _ServerIp + ":" + _ServerPort);
                    }

                    _Client.EndConnect(asyncResult);

                    _SourceIp = ((IPEndPoint)_Client.Client.LocalEndPoint).Address.ToString();
                    _SourcePort = ((IPEndPoint)_Client.Client.LocalEndPoint).Port;
                    _TcpStream = _Client.GetStream();

                    Connected = true;
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    waitHandle.Close();
                }

            if (ServerConnected != null)
            {
                Task serverConnected = Task.Run(() => ServerConnected());
            }

            return DataReceiver();
        }

        /// <summary>
        /// Send data to the server.
        /// </summary>
        /// <param name="data">Byte array containing data.</param>
        /// <returns>Boolean indicating if the message was sent successfully.</returns>
        public bool Send(byte[] data)
        {
            return MessageWrite(null, data);
        }

        /// <summary>
        /// Send data to the server asynchronously.
        /// </summary>
        /// <param name="data">Byte array containing data.</param>
        /// <returns>Task with Boolean indicating if the message was sent successfully.</returns>
        public async Task<bool> SendAsync(byte[] data)
        {
            return await MessageWriteAsync(null, data);
        }

        #endregion Public-Methods

        #region Private-Methods

        private void Log(string msg)
        {
            if (Debug)
            {
                Console.WriteLine(msg);
            }
        }
         
        private async Task DataReceiver()
        {  
            while (true)
            {
                bool readLocked = false;
                 
                try
                {
                    _Token.ThrowIfCancellationRequested();
                     
                    if (_Client == null 
                        || !_Client.Connected
                        || _Token.IsCancellationRequested)
                    {
                        Log("Disconnect detected");
                        break;
                    }

                    WatsonMessage msg = null;
                    readLocked = await _ReadLock.WaitAsync(1);
                    bool buildSuccess = false;

                    msg = new WatsonMessage(_TcpStream, Debug); 

                    if (_MessageReceived != null)
                    {
                        buildSuccess = await msg.Build();
                    }
                    else
                    {
                        break;
                    }

                    if (!buildSuccess)
                    {
                        Log("Message build failed due to disconnect");
                        break;
                    }

                    if (msg == null)
                    { 
                        await Task.Delay(30);
                        continue;
                    }

                    if (msg.Status == MessageStatus.Removed)
                    {
                        Log("Disconnect due to server-side removal");
                        break;
                    }
                    else if (msg.Status == MessageStatus.Disconnecting)
                    {
                        Log("Disconnect due to server shutting down");
                        break;
                    }

                        if (_MessageReceived != null)
                        {
                            // does not need to be awaited, because the stream has been fully read
                            Task unawaited = Task.Run(() => _MessageReceived(msg.Data));
                        }
                        else
                        {
                            break;
                        }
                } 
                catch (Exception e)
                {
                    Log(Environment.NewLine +
                        "Data receiver exception:" +
                        Environment.NewLine +
                        e.ToString() +
                        Environment.NewLine); 
                    break;
                } 
                finally
                {
                    if (readLocked) _ReadLock.Release();
                }
            } 

            Log("Data receiver terminated");
            Connected = false;
            ServerDisconnected?.Invoke();
            Dispose();
        }

        private bool MessageWrite(Dictionary<object, object> metadata, byte[] data)
        {
            long dataLen = 0;
            MemoryStream ms = new MemoryStream();
            if (data != null && data.Length > 0)
            {
                dataLen = data.Length;
                ms.Write(data, 0, data.Length);
                ms.Seek(0, SeekOrigin.Begin);
            }
            else
            {
                ms = new MemoryStream(new byte[0]);
            }

            return MessageWrite(metadata, dataLen, ms);
        }

        private bool MessageWrite(Dictionary<object, object> metadata, long contentLength, Stream stream)
        {
            if (contentLength < 0) throw new ArgumentException("Content length must be zero or greater bytes.");
            if (contentLength > 0)
            {
                if (stream == null || !stream.CanRead)
                {
                    throw new ArgumentException("Cannot read from supplied stream.");
                }
            }

            bool disconnectDetected = false;

            try
            {
                if (_Client == null
                    || !_Client.Connected)
                {
                    disconnectDetected = true;
                    return false;
                }

                WatsonMessage msg = new WatsonMessage(metadata, contentLength, stream, Debug);
                byte[] headerBytes = msg.ToHeaderBytes(contentLength);

                int bytesRead = 0;
                long bytesRemaining = contentLength;
                byte[] buffer = new byte[_ReadStreamBufferSize];

                _WriteLock.Wait(1);

                try
                {
                        // write headers
                        _TcpStream.Write(headerBytes, 0, headerBytes.Length);

                        // write metadata
                        if (msg.MetadataBytes != null && msg.MetadataBytes.Length > 0)
                        {
                            _TcpStream.Write(msg.MetadataBytes, 0, msg.MetadataBytes.Length);
                        }

                        // write data
                        if (contentLength > 0)
                        {
                            while (bytesRemaining > 0)
                            {
                                bytesRead = stream.Read(buffer, 0, buffer.Length);
                                if (bytesRead > 0)
                                {
                                    _TcpStream.Write(buffer, 0, bytesRead);
                                    bytesRemaining -= bytesRead;
                                }
                            }
                        }

                        _TcpStream.Flush();
                }
                finally
                {
                    _WriteLock.Release();
                }
                 
                return true;
            }
            catch (Exception e)
            {
                Log(Environment.NewLine +
                    "MessageWrite exception encountered:" +
                    Environment.NewLine +
                    e.ToString() +
                    Environment.NewLine);

                disconnectDetected = true;
                return false;
            }
            finally
            {
                if (disconnectDetected)
                {
                    Connected = false;
                    Dispose();
                }
            }
        }

        private bool MessageWrite(WatsonMessage msg)
        {
            bool disconnectDetected = false;
            long dataLen = 0;
            if (msg.Data != null) dataLen = msg.Data.Length;

            try
            {
                if (_Client == null
                    || !_Client.Connected)
                {
                    disconnectDetected = true;
                    return false;
                }

                byte[] headerBytes = msg.ToHeaderBytes(dataLen);

                _WriteLock.Wait(1);

                try
                {
                        // write headers
                        _TcpStream.Write(headerBytes, 0, headerBytes.Length);

                        // write metadata
                        if (msg.MetadataBytes != null && msg.MetadataBytes.Length > 0)
                        {
                            _TcpStream.Write(msg.MetadataBytes, 0, msg.MetadataBytes.Length);
                        }

                        // write data
                        if (msg.Data != null && msg.Data.Length > 0)
                        {
                            _TcpStream.Write(msg.Data, 0, msg.Data.Length);
                        }

                        _TcpStream.Flush();
                }
                finally
                {
                    _WriteLock.Release();
                }

                return true;
            }
            catch (Exception e)
            {
                Log(Environment.NewLine +
                    "MessageWrite exception encountered:" +
                    Environment.NewLine +
                    e.ToString() +
                    Environment.NewLine);

                disconnectDetected = true;
                return false;
            }
            finally
            {
                if (disconnectDetected)
                {
                    Connected = false;
                    Dispose();
                }
            }
        }

        private async Task<bool> MessageWriteAsync(Dictionary<object, object> metadata, byte[] data)
        {
            long dataLen = 0;
            MemoryStream ms = new MemoryStream();
            if (data != null)
            {
                dataLen = data.Length;
                ms.Write(data, 0, data.Length);
                ms.Seek(0, SeekOrigin.Begin);
            }
            else
            {
                ms = new MemoryStream(new byte[0]);
            }

            return await MessageWriteAsync(metadata, dataLen, ms);
        }

        private async Task<bool> MessageWriteAsync(Dictionary<object, object> metadata, long contentLength, Stream stream)
        {
            if (!Connected) return false;
            if (contentLength < 0) throw new ArgumentException("Content length must be zero or greater bytes.");
            if (contentLength > 0)
            {
                if (stream == null || !stream.CanRead)
                {
                    throw new ArgumentException("Cannot read from supplied stream.");
                }
            }

            bool disconnectDetected = false;

            try
            {
                if (_Client == null || !_Client.Connected)
                {
                    disconnectDetected = true;
                    return false;
                }

                WatsonMessage msg = new WatsonMessage(metadata, contentLength, stream, Debug);
                byte[] headerBytes = msg.ToHeaderBytes(contentLength);

                int bytesRead = 0;
                long bytesRemaining = contentLength;
                byte[] buffer = new byte[_ReadStreamBufferSize];

                await _WriteLock.WaitAsync();

                try
                {
                        // write headers
                        await _TcpStream.WriteAsync(headerBytes, 0, headerBytes.Length);

                        // write metadata
                        if (msg.MetadataBytes != null && msg.MetadataBytes.Length > 0)
                        {
                            await _TcpStream.WriteAsync(msg.MetadataBytes, 0, msg.MetadataBytes.Length);
                        }

                        // write data
                        if (contentLength > 0)
                        {
                            while (bytesRemaining > 0)
                            {
                                bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                                if (bytesRead > 0)
                                {
                                    await _TcpStream.WriteAsync(buffer, 0, bytesRead);
                                    bytesRemaining -= bytesRead;
                                }
                            }
                        }

                        await _TcpStream.FlushAsync();
                }
                finally
                {
                    _WriteLock.Release();
                }
                 
                return true;
            }
            catch (Exception e)
            {
                Log(Environment.NewLine +
                    "MessageWrite exception encountered:" +
                    Environment.NewLine +
                    e.ToString() +
                    Environment.NewLine);

                disconnectDetected = true;
                return false;
            }
            finally
            {
                if (disconnectDetected)
                {
                    Connected = false;
                    Dispose();
                }
            }
        }

        #endregion Private-Methods
    }
}