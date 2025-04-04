﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Inventors.ECP.Profiling;
using Serilog;

namespace Inventors.ECP
{
    public class BusCentral :
        DynamicObject,
        IDisposable
    {
        private enum CommState
        {
            IDLE,
            WAITING,
            COMPLETED,
            ERROR
        };

        #region Properties

        public int Timeout { get; set; }

        public string Location
        {
            get => connection.Location;
            set => connection.Location = value;
        }

        public int BaudRate
        {
            get => connection.BaudRate;
            set => connection.BaudRate = value;
        }

        public bool ResetOnConnection
        {
            get => connection.ResetOnConnection;
            set => connection.ResetOnConnection = value;
        }

        public bool IsOpen => connection.IsOpen;

        public Profiler Profiler { get; }

        public double RxRate => connection.RxRate;

        public double TxRate => connection.TxRate;

        #endregion

        public BusCentral(Device device, CommunicationLayer connection, Profiler profiler)
        {
            if (connection is null)
                throw new ArgumentException(Resources.CONNECTION_NULL);

            this.connection = connection;
            connection.Destuffer.OnReceive += HandleIncommingFrame;
            Timeout = 500;
            Profiler = profiler;
            this.device = device;
        }

        /// <summary>
        /// Open the communication with a device.
        /// </summary>
        public void Open() => connection.Open();

        /// <summary>
        /// Close the communication with a device.
        /// </summary>
        public void Close() => connection.Close();

        #region Execution of device functions

        /// <summary>
        /// Execute a function.
        /// </summary>
        /// <param name="function">The function to execute.</param>
        public void Execute(DeviceFunction function, DeviceAddress address)
        {
            if (function is null)
                return;

            Task.Run(async () =>
            {
                await commSemaphore.WaitAsync();

                try
                {
                    function.OnSend();
                    Initiate(function, address);

                    while (!IsCompleted())
                        await Task.Yield();

                    state = CommState.IDLE;

                    if (currentException is not null)
                        throw currentException;
                }
                finally { commSemaphore.Release(); }
            }).Wait();
        }

        private bool IsCompleted()
        {
            if (stopwatch.ElapsedMilliseconds >= Timeout)
            {
                currentException = new PeripheralNotRespondingException(Resources.NO_RESPONSE);
                return true;
            }

            lock (lockObject)
            {
                if (state != CommState.WAITING)
                    return true;
            }

            return false;
        }

        private void Initiate(DeviceFunction function, DeviceAddress address)
        {
            var bytes = function.GetRequest((byte) (address is not null ? address.Value : 0));

            lock (lockObject)
            {
                stopwatch.Restart();
                current = function;
                state = CommState.WAITING;
                currentException = null;
            }

            connection.Transmit(Frame.Encode(bytes));
        }

        #endregion
        #region Sending messages

        /// <summary>
        /// Send an unacknowledged message to the device.
        /// </summary>
        /// <param name="message">The message to send</param>
        public void Send(DeviceMessage message, DeviceAddress address)
        {
            if (connection.IsOpen && (message is not null))
            {
                Task.Run(async () =>
                {
                    await commSemaphore.WaitAsync();

                    try
                    {
                        connection.Transmit(Frame.Encode(message.GetPacket(address)));
                    }
                    finally { commSemaphore.Release(); }
                }).Wait();
            }
        }

        #endregion

        public void RestartStatistics() => connection.RestartStatistics();


        private void HandleIncommingFrame(Destuffer caller, byte[] frame)
        {
            try
            {
                if (!Packet.IsValid(frame))
                {
                    return;
                }

                var packet = new Packet(frame);

                if (packet.Code != 0x00)
                {
                    if (packet.IsFunction)
                    {
                        lock (lockObject)
                        {
                            if (current != null)
                            {
                                current.SetResponse(packet);
                                current.OnReceived();
                            }

                            state = CommState.COMPLETED;
                        }

                        if (ECPLog.Enabled)
                            Log.Debug("Response received [ Code: {code} ]", packet.Code);
                        else
                            Log.Verbose("Response received [ Code: {code} ]", packet.Code);
                    }
                    else
                    {
                        try
                        {
                            Dispatch(packet);
                        }
                        catch (Exception e)
                        {
                            if (ECPLog.Enabled)
                                Log.Error("Exception when dispacthing message: {exception}", e);
                            else
                                Log.Verbose("Exception when dispacthing message: {exception}", e);

                            Profiler.Add(new TargetEvent(e.Message));
                        }
                    }
                }
                else
                {                    
                    lock (lockObject)
                    {
                        var errorCode = packet.GetByte(0);
                        currentException = new FunctionNotAcknowledgedException(device.GetErrorString(errorCode));
                        state = CommState.ERROR;
                    }
                }
            }
            catch (Exception e)
            {
                if (ECPLog.Enabled)
                    Log.Error("Error in HandleIncommingFrame: {exception}", e);
                else
                    Log.Verbose("Error in HandleIncommingFrame: {exception}", e);
            }
        }

        private void Dispatch(Packet packet)
        {
            if (Dispatchers.ContainsKey(packet.Code) && (MessageListener is object))
            {
                var msg = Dispatchers[packet.Code].Create(packet);
                msg.Dispatch(MessageListener);

                if (Analysers.ContainsKey(packet.Code))
                {
                    foreach (var analyser in Analysers[packet.Code])
                        analyser.Accept(msg);
                }
            }
        }

        public void Add(DeviceMessage message)
        {
            if (message is null)
                throw new ArgumentNullException(nameof(message));

            if (Dispatchers.ContainsKey(message.Code))
                throw new ArgumentException($"Message [ { message } ] is allready present in Dispatchers");

            Dispatchers.Add(message.Code, message.CreateDispatcher());
        }

        public void Add(MessageAnalyser analyser)
        {
            if (analyser is null)
                throw new ArgumentNullException(nameof(analyser));

            if (!Dispatchers.ContainsKey(analyser.Code))
                throw new ArgumentException($"No dispatcher is defined for Analyser [ {analyser.Code} ]");

            if (!Analysers.ContainsKey(analyser.Code))
                Analysers.Add(analyser.Code, new List<MessageAnalyser>());

            Analysers[analyser.Code].Add(analyser);
        }

        private Dictionary<byte, MessageDispatcher> Dispatchers { get; } = new Dictionary<byte, MessageDispatcher>();

        private Dictionary<byte, List<MessageAnalyser>> Analysers { get; } = new Dictionary<byte, List<MessageAnalyser>>();

        public List<string> GetLocations() => connection.GetLocations();

        #region Dispose Pattern
        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (IsOpen)
                    {
                        Close();
                    }
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion

        /// <summary>
        /// The message listener.
        /// </summary>
        public dynamic MessageListener { get; set; }

        private readonly CommunicationLayer connection;
        private DeviceFunction current;
        private readonly object lockObject = new();
        private readonly SemaphoreSlim commSemaphore = new(1, 1);
        private Exception currentException;
        private readonly Stopwatch stopwatch = new Stopwatch();
        private CommState state = CommState.WAITING;
        private readonly Device device;
    }
}
