using Inventors.ECP.Monitor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.ECP
{
    public class Destuffer
    {
        private enum State
        {
            WAITING_FOR_DLE,
            WAITING_FOR_STX,
            RECEIVING_DATA,
            WAITING_FOR_ETX
        }

        public event Action<Destuffer, byte[]> OnReceive;

        public void Reset()
        {
            state = State.WAITING_FOR_DLE;
            Discard();
        }

        public void Add(int length, byte[] buffer)
        {
            if (buffer is object)
            {
                for (int n = 0; n < length; ++n)
                {
                    var data = buffer[n];

                    if (PortMonitor.Enabled)
                    {
                        raw.Add(data);
                    }

                    switch (state)
                    {
                        case State.WAITING_FOR_DLE:
                            HandleWaitingForDLE(data);
                            break;

                        case State.WAITING_FOR_STX:
                            HandleWaitingForSTX(data);
                            break;

                        case State.RECEIVING_DATA:
                            HandleReceivingData(data);
                            break;
                        case State.WAITING_FOR_ETX:
                            HandleWaitingForETX(data);
                            break;
                    }
                }
            }
        }

        private void HandleWaitingForDLE(byte data)
        {
            if (data == Frame.DLE)
            {
                buffer.Clear();
                state = State.WAITING_FOR_STX;
            }
        }

        private void HandleWaitingForSTX(byte data)
        {
            if (data == Frame.STX)
            {
                state = State.RECEIVING_DATA;
                buffer.Clear();
            }
            else if (data != Frame.DLE)
            {
                state = State.WAITING_FOR_DLE;
                Discard();
            }
        }

        private void HandleReceivingData(byte data)
        {
            if (data != Frame.DLE)
            {
                buffer.Add(data);
            }
            else
            {
                state = State.WAITING_FOR_ETX;
            }
        }

        private void HandleWaitingForETX(byte data)
        {
            if (data == Frame.DLE)
            {
                buffer.Add(Frame.DLE);
                state = State.RECEIVING_DATA;
            }
            else if (data == Frame.ETX)
            {
                state = State.WAITING_FOR_DLE;                
                NotifyListeners();
                Discard();
            }
            else if (data == Frame.STX)
            {
                state = State.RECEIVING_DATA;
                Discard();
            }
            else
            {
                state = State.WAITING_FOR_DLE;
                Discard();
            }
        }

        private void Discard()
        {
            if (buffer.Count > 0)
            {
                buffer.Clear();
            }

            if (raw.Count > 0)
            {
                if (PortMonitor.Enabled)
                {
                    PortMonitor.Add(rx: true, raw.ToArray());
                }

                raw.Clear();
            }
        }

        private void NotifyListeners() => OnReceive?.Invoke(this, buffer.ToArray());

        private State state = State.WAITING_FOR_DLE;
        private readonly List<byte> buffer = new List<byte>();
        private readonly List<byte> raw = new List<byte>();
    }
}
