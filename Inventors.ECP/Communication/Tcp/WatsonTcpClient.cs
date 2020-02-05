﻿using System;
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
        /// Function called when authentication is requested from the server.  Expects the 16-byte preshared key.
        /// </summary>
        public Func<string> AuthenticationRequested = null;

        /// <summary>
        /// Function called when authentication has succeeded.
        /// </summary>
        public Func<Task> AuthenticationSucceeded = null;

        /// <summary>
        /// Function called when authentication has failed.
        /// </summary>
        public Func<Task> AuthenticationFailure = null;

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
                if (_StreamReceived != null) throw new InvalidOperationException("Only one of 'MessageReceived' and 'StreamReceived' can be set.");
                if (_StreamReceivedWithMetadata != null) throw new InvalidOperationException("You may not use 'MessageReceived' when 'StreamReceivedWithMetadata' has been set.");
                _MessageReceived = value;
            }
        }

        /// <summary>
        /// Use of 'MessageReceivedWithMetadata' is exclusive and cannot be used with 'StreamReceivedWithMetadata'.
        /// This callback is called when a message is received from the server with attached metadata.
        /// The metadata is contained in the supplied Dictionary, and the entire message payload is passed into the supplied byte array.
        /// </summary>
        public Func<Dictionary<object, object>, byte[], Task> MessageReceivedWithMetadata
        {
            get
            {
                return _MessageReceivedWithMetadata;
            }
            set
            {
                if (_StreamReceived != null) throw new InvalidOperationException("'MessageReceivedWithMetadata' cannot be used when 'StreamReceived' has been set.");
                if (_StreamReceivedWithMetadata != null) throw new InvalidOperationException("You may not use 'MessageReceivedWithMetadata' when 'StreamReceivedWithMetadata' has been set.");
                _MessageReceivedWithMetadata = value;
            }
        }

        /// <summary>
        /// Use of 'StreamReceived' is exclusive and cannot be used with 'StreamReceived'. 
        /// If receiving messages with metadata, 'StreamReceivedWithMetadata' must be set and 'MessageReceivedWithMetadata' cannot be used.
        /// This callback is called when a stream is received from the server.
        /// The stream and its length are passed to your application. 
        /// </summary>
        public Func<long, Stream, Task> StreamReceived
        {
            get
            {
                return _StreamReceived;
            }
            set
            {
                if (_MessageReceived != null) throw new InvalidOperationException("Only one of 'MessageReceived' and 'StreamReceived' can be set.");
                if (_MessageReceivedWithMetadata != null) throw new InvalidOperationException("You may not use 'StreamReceived' when 'MessageReceivedWithMetadata' has been set.");
                _StreamReceived = value;
            }
        }

        /// <summary>
        /// Use of 'StreamReceivedWithMetadata' is exclusive and cannot be used with 'MessageReceivedWithMetadata'.
        /// This callback is called when a stream is received from the server with attached metadata.
        /// The metadata is contained in the supplied Dictionary, and the stream and its length are passed into your application.
        /// </summary>
        public Func<Dictionary<object, object>, long, Stream, Task> StreamReceivedWithMetadata
        {
            get
            {
                return _StreamReceivedWithMetadata;
            }
            set
            {
                if (_MessageReceived != null) throw new InvalidOperationException("'StreamReceivedWithMetadata' cannot be used when 'MessageReceived' has been set.");
                if (_MessageReceivedWithMetadata != null) throw new InvalidOperationException("You may not use 'StreamReceivedWithMetadata' when 'MessageReceivedWithMetadata' has been set.");
                _StreamReceivedWithMetadata = value;
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
        /// Enable acceptance of SSL certificates from the server that cannot be validated.
        /// </summary>
        public bool AcceptInvalidCertificates = true;

        /// <summary>
        /// Require mutual authentication between the server and this client.
        /// </summary>
        public bool MutuallyAuthenticate
        {
            get
            {
                return _MutuallyAuthenticate;
            }
            set
            {
                if (value)
                {
                    if (_Mode == Mode.Tcp) throw new ArgumentException("Mutual authentication only supported with SSL.");
                    if (_SslCertificate == null) throw new ArgumentException("Mutual authentication requires a certificate.");
                }

                _MutuallyAuthenticate = value;
            }
        }

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
        private Mode _Mode;
        private string _SourceIp;
        private int _SourcePort;
        private string _ServerIp;
        private int _ServerPort;
        private bool _MutuallyAuthenticate = false;
        private TcpClient _Client = null;
        private NetworkStream _TcpStream = null;
        private SslStream _SslStream = null;

        private X509Certificate2 _SslCertificate = null;
        private X509Certificate2Collection _SslCertificateCollection = null;

        private SemaphoreSlim _WriteLock = new SemaphoreSlim(1);
        private SemaphoreSlim _ReadLock = new SemaphoreSlim(1);

        private CancellationTokenSource _TokenSource = new CancellationTokenSource();
        private CancellationToken _Token;

        private Func<byte[], Task> _MessageReceived = null;
        private Func<Dictionary<object, object>, byte[], Task> _MessageReceivedWithMetadata = null;
        private Func<long, Stream, Task> _StreamReceived = null;
        private Func<Dictionary<object, object>, long, Stream, Task> _StreamReceivedWithMetadata = null;

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
            _Mode = Mode.Tcp;
            _ServerIp = serverIp;
            _ServerPort = serverPort;  
        }

        /// <summary>
        /// Initialize the Watson TCP client with SSL.  Call Start() afterward to connect to the server.
        /// </summary>
        /// <param name="serverIp">The IP address or hostname of the server.</param>
        /// <param name="serverPort">The TCP port on which the server is listening.</param>
        /// <param name="pfxCertFile">The file containing the SSL certificate.</param>
        /// <param name="pfxCertPass">The password for the SSL certificate.</param>
        public WatsonTcpClient(
            string serverIp,
            int serverPort,
            string pfxCertFile,
            string pfxCertPass)
        {
            if (String.IsNullOrEmpty(serverIp)) throw new ArgumentNullException(nameof(serverIp));
            if (serverPort < 1) throw new ArgumentOutOfRangeException(nameof(serverPort));
             
            _Token = _TokenSource.Token;
            _Mode = Mode.Ssl;
            _ServerIp = serverIp;
            _ServerPort = serverPort;

            if (!String.IsNullOrEmpty(pfxCertFile))
            {
                if (String.IsNullOrEmpty(pfxCertPass))
                {
                    _SslCertificate = new X509Certificate2(pfxCertFile);
                }
                else
                {
                    _SslCertificate = new X509Certificate2(pfxCertFile, pfxCertPass);
                }

                _SslCertificateCollection = new X509Certificate2Collection
                {
                    _SslCertificate
                };
            }
            else
            {
                _SslCertificateCollection = new X509Certificate2Collection();
            }
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
             
            if (_SslStream != null)
            { 
                _SslStream.Close();
                _SslStream.Dispose();
                _SslStream = null;
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

            if (_Mode == Mode.Tcp)
            {
                #region TCP

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
                    _SslStream = null;

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

                #endregion TCP
            }
            else if (_Mode == Mode.Ssl)
            {
                #region SSL

                Log("Watson TCP client connecting with SSL to " + _ServerIp + ":" + _ServerPort);

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

                    if (AcceptInvalidCertificates)
                    {
                        // accept invalid certs
                        _SslStream = new SslStream(_Client.GetStream(), false, new RemoteCertificateValidationCallback(AcceptCertificate));
                    }
                    else
                    {
                        // do not accept invalid SSL certificates
                        _SslStream = new SslStream(_Client.GetStream(), false);
                    }

                    _SslStream.AuthenticateAsClient(_ServerIp, _SslCertificateCollection, SslProtocols.Tls12, !AcceptInvalidCertificates);

                    if (!_SslStream.IsEncrypted)
                    {
                        throw new AuthenticationException("Stream is not encrypted");
                    }

                    if (!_SslStream.IsAuthenticated)
                    {
                        throw new AuthenticationException("Stream is not authenticated");
                    }

                    if (MutuallyAuthenticate && !_SslStream.IsMutuallyAuthenticated)
                    {
                        throw new AuthenticationException("Mutual authentication failed");
                    }

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

                #endregion SSL
            }
            else
            {
                throw new ArgumentException("Unknown mode: " + _Mode.ToString());
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

            if (_Mode == Mode.Tcp)
            {
                #region TCP

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
                    _SslStream = null;

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

                #endregion TCP
            }
            else if (_Mode == Mode.Ssl)
            {
                #region SSL

                Log("Watson TCP client connecting with SSL to " + _ServerIp + ":" + _ServerPort);

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

                    if (AcceptInvalidCertificates)
                    {
                        // accept invalid certs
                        _SslStream = new SslStream(_Client.GetStream(), false, new RemoteCertificateValidationCallback(AcceptCertificate));
                    }
                    else
                    {
                        // do not accept invalid SSL certificates
                        _SslStream = new SslStream(_Client.GetStream(), false);
                    }

                    _SslStream.AuthenticateAsClient(_ServerIp, _SslCertificateCollection, SslProtocols.Tls12, !AcceptInvalidCertificates);

                    if (!_SslStream.IsEncrypted)
                    {
                        throw new AuthenticationException("Stream is not encrypted");
                    }

                    if (!_SslStream.IsAuthenticated)
                    {
                        throw new AuthenticationException("Stream is not authenticated");
                    }

                    if (MutuallyAuthenticate && !_SslStream.IsMutuallyAuthenticated)
                    {
                        throw new AuthenticationException("Mutual authentication failed");
                    }

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

                #endregion SSL
            }
            else
            {
                throw new ArgumentException("Unknown mode: " + _Mode.ToString());
            }

            if (ServerConnected != null)
            {
                Task serverConnected = Task.Run(() => ServerConnected());
            }

            return DataReceiver();
        }

        /// <summary>
        /// Send a pre-shared key to the server to authenticate.
        /// </summary>
        /// <param name="presharedKey">Up to 16-character string.</param>
        public void Authenticate(string presharedKey)
        {
            if (String.IsNullOrEmpty(presharedKey)) throw new ArgumentNullException(nameof(presharedKey));
            if (presharedKey.Length != 16) throw new ArgumentException("Preshared key length must be 16 bytes.");

            presharedKey = presharedKey.PadRight(16, ' ');
            WatsonMessage msg = new WatsonMessage();
            msg.Status = MessageStatus.AuthRequested;
            msg.PresharedKey = Encoding.UTF8.GetBytes(presharedKey);
            msg.Data = null;
            msg.ContentLength = 0;
            MessageWrite(msg);
        }

        /// <summary>
        /// Send data to the server.
        /// </summary>
        /// <param name="data">String containing data.</param>
        /// <returns>Boolean indicating if the message was sent successfully.</returns>
        public bool Send(string data)
        {
            if (String.IsNullOrEmpty(data)) return Send(new byte[0]);
            else return MessageWrite(null, Encoding.UTF8.GetBytes(data));
        }

        /// <summary>
        /// Send data and metadata to the server.
        /// </summary>
        /// <param name="metadata">Dictionary containing metadata.</param>
        /// <param name="data">String containing data.</param>
        /// <returns>Boolean indicating if the message was sent successfully.</returns>
        public bool Send(Dictionary<object, object> metadata, string data)
        {
            if (String.IsNullOrEmpty(data)) return Send(new byte[0]);
            else return MessageWrite(metadata, Encoding.UTF8.GetBytes(data));
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
        /// Send data and metadata to the server.
        /// </summary>
        /// <param name="metadata">Dictionary containing metadata.</param>
        /// <param name="data">Byte array containing data.</param>
        /// <returns>Boolean indicating if the message was sent successfully.</returns>
        public bool Send(Dictionary<object, object> metadata, byte[] data)
        {
            return MessageWrite(metadata, data);
        }

        /// <summary>
        /// Send data to the server using a stream.
        /// </summary>
        /// <param name="contentLength">The number of bytes in the stream.</param>
        /// <param name="stream">The stream containing the data.</param>
        /// <returns>Boolean indicating if the message was sent successfully.</returns>
        public bool Send(long contentLength, Stream stream)
        {
            return MessageWrite(null, contentLength, stream);
        }

        /// <summary>
        /// Send data and metadata to the server using a stream.
        /// </summary>
        /// <param name="metadata">Dictionary containing metadata.</param>
        /// <param name="contentLength">The number of bytes in the stream.</param>
        /// <param name="stream">The stream containing the data.</param>
        /// <returns>Boolean indicating if the message was sent successfully.</returns>
        public bool Send(Dictionary<object, object> metadata, long contentLength, Stream stream)
        {
            return MessageWrite(metadata, contentLength, stream);
        }

        /// <summary>
        /// Send data to the server asynchronously.
        /// </summary>
        /// <param name="data">String containing data.</param>
        /// <returns>Boolean indicating if the message was sent successfully.</returns>
        public async Task<bool> SendAsync(string data)
        {
            if (String.IsNullOrEmpty(data)) return await SendAsync(new byte[0]);
            else return await MessageWriteAsync(null, Encoding.UTF8.GetBytes(data));
        }

        /// <summary>
        /// Send data and metadata to the server asynchronously.
        /// </summary>
        /// <param name="metadata">Dictionary containing metadata.</param>
        /// <param name="data">String containing data.</param>
        /// <returns>Boolean indicating if the message was sent successfully.</returns>
        public async Task<bool> SendAsync(Dictionary<object, object> metadata, string data)
        {
            if (String.IsNullOrEmpty(data)) return await SendAsync(new byte[0]);
            else return await MessageWriteAsync(metadata, Encoding.UTF8.GetBytes(data));
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

        /// <summary>
        /// Send data and metadata to the server asynchronously.
        /// </summary>
        /// <param name="metadata">Dictionary containing metadata.</param>
        /// <param name="data">Byte array containing data.</param>
        /// <returns>Task with Boolean indicating if the message was sent successfully.</returns>
        public async Task<bool> SendAsync(Dictionary<object, object> metadata, byte[] data)
        {
            return await MessageWriteAsync(metadata, data);
        }

        /// <summary>
        /// Send data to the server from a stream asynchronously.
        /// </summary>
        /// <param name="contentLength">The number of bytes to send.</param>
        /// <param name="stream">The stream containing the data.</param>
        /// <returns>Task with Boolean indicating if the message was sent successfully.</returns>
        public async Task<bool> SendAsync(long contentLength, Stream stream)
        {
            return await MessageWriteAsync(null, contentLength, stream);
        }

        /// <summary>
        /// Send data and metadata to the server from a stream asynchronously.
        /// </summary>
        /// <param name="metadata">Dictionary containing metadata.</param>
        /// <param name="contentLength">The number of bytes to send.</param>
        /// <param name="stream">The stream containing the data.</param>
        /// <returns>Task with Boolean indicating if the message was sent successfully.</returns>
        public async Task<bool> SendAsync(Dictionary<object, object> metadata, long contentLength, Stream stream)
        {
            return await MessageWriteAsync(metadata, contentLength, stream);
        }

        #endregion Public-Methods

        #region Private-Methods

        private bool AcceptCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            // return true; // Allow untrusted certificates.
            return AcceptInvalidCertificates;
        }

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

                    if (_SslStream != null)
                    {
                        msg = new WatsonMessage(_SslStream, Debug); 
                    }
                    else
                    {
                        msg = new WatsonMessage(_TcpStream, Debug); 
                    }

                    if (_MessageReceived != null)
                    {
                        buildSuccess = await msg.Build();
                    }
                    else if (_StreamReceived != null)
                    {
                        buildSuccess = await msg.BuildStream();
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
                    else if (msg.Status == MessageStatus.AuthSuccess)
                    {
                        Log("Authentication successful");
                        AuthenticationSucceeded?.Invoke();
                        continue;
                    }
                    else if (msg.Status == MessageStatus.AuthFailure)
                    {
                        Log("Authentication failed");
                        AuthenticationFailure?.Invoke();
                        continue;
                    }

                    if (msg.Status == MessageStatus.AuthRequired)
                    {
                        Log("Authentication required by server, please authenticate using pre-shared key");
                        if (AuthenticationRequested != null)
                        {
                            string psk = AuthenticationRequested();
                            if (!String.IsNullOrEmpty(psk))
                            {
                                Authenticate(psk);
                            }
                        }
                        continue;
                    }

                    if (msg.Metadata.Count > 0)
                    {
                        if (_MessageReceivedWithMetadata != null)
                        {
                            // does not need to be awaited, because the stream has been fully read
                            Task unawaited = Task.Run(() => _MessageReceivedWithMetadata(msg.Metadata, msg.Data));
                        }
                        else if (_StreamReceivedWithMetadata != null)
                        {
                            // must be awaited, because the content has not yet been fully read
                            await _StreamReceivedWithMetadata(msg.Metadata, msg.ContentLength, msg.DataStream);
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        if (_MessageReceived != null)
                        {
                            // does not need to be awaited, because the stream has been fully read
                            Task unawaited = Task.Run(() => _MessageReceived(msg.Data));
                        }
                        else if (_StreamReceived != null)
                        {
                            // must be awaited, because the content has not yet been fully read
                            await _StreamReceived(msg.ContentLength, msg.DataStream);
                        }
                        else
                        {
                            break;
                        }
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
                    if (_Mode == Mode.Tcp)
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
                    else if (_Mode == Mode.Ssl)
                    {
                        // write headers
                        _SslStream.Write(headerBytes, 0, headerBytes.Length);

                        // write metadata
                        if (msg.MetadataBytes != null && msg.MetadataBytes.Length > 0)
                        {
                            _SslStream.Write(msg.MetadataBytes, 0, msg.MetadataBytes.Length);
                        }

                        // write data
                        if (contentLength > 0)
                        {
                            while (bytesRemaining > 0)
                            {
                                bytesRead = stream.Read(buffer, 0, buffer.Length);
                                if (bytesRead > 0)
                                {
                                    _SslStream.Write(buffer, 0, bytesRead);
                                    bytesRemaining -= bytesRead;
                                }
                            }
                        }

                        _SslStream.Flush();
                    } 
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
                    if (_Mode == Mode.Tcp)
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
                    else if (_Mode == Mode.Ssl)
                    {
                        // write headers
                        _SslStream.Write(headerBytes, 0, headerBytes.Length);

                        // write metadata
                        if (msg.MetadataBytes != null && msg.MetadataBytes.Length > 0)
                        {
                            _SslStream.Write(msg.MetadataBytes, 0, msg.MetadataBytes.Length);
                        }

                        // write data
                        if (msg.Data != null && msg.Data.Length > 0)
                        {
                            _SslStream.Write(msg.Data, 0, msg.Data.Length);
                        }

                        _SslStream.Flush();
                    }
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
                    if (_Mode == Mode.Tcp)
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
                    else if (_Mode == Mode.Ssl)
                    {
                        // write headers
                        await _SslStream.WriteAsync(headerBytes, 0, headerBytes.Length);

                        // write metadata
                        if (msg.MetadataBytes != null && msg.MetadataBytes.Length > 0)
                        {
                            await _SslStream.WriteAsync(msg.MetadataBytes, 0, msg.MetadataBytes.Length);
                        }

                        // write data
                        if (contentLength > 0)
                        {
                            while (bytesRemaining > 0)
                            {
                                bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                                if (bytesRead > 0)
                                {
                                    await _SslStream.WriteAsync(buffer, 0, bytesRead);
                                    bytesRemaining -= bytesRead;
                                }
                            }
                        }

                        await _SslStream.FlushAsync();
                    }
                    else
                    {
                        throw new ArgumentException("Unknown mode: " + _Mode.ToString());
                    }
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