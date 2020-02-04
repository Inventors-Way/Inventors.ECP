using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventors.ECP.Utility;
using Inventors.Logging;

namespace Inventors.ECP
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "<Pending>")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1028:Enum Storage should be Int32", Justification = "<Pending>")]
    public enum PacketType : byte
    {
        LENGTH_UINT8_ENCODED = 0,
        LENGTH_UINT16_ENCODED = 0xFE,
        LENGTH_UINT32_ENCODED = 0xFF
    }

    public class Packet
    {
        #region Properties
        public byte Code => _code;

        public bool IsFunction => _code < 128;

        public int Length => _length;

        public bool Empty => _length == 0;

        public bool ReverseEndianity { get; set; } = false;

        public PacketType PacketType => _type;

        #endregion

        public Packet(byte code, int length)
        {
            this._code = code;
            this._length = length;
            this._type = GetLengthEncoding(length);
            data = new byte[length];
        }

        private static PacketType GetLengthEncoding(int length)
        {
            var retValue = PacketType.LENGTH_UINT8_ENCODED;

            if (length > UInt16.MaxValue)
            {
                retValue = PacketType.LENGTH_UINT32_ENCODED;
            }
            else if (length >= 0xFE)
            {
                retValue = PacketType.LENGTH_UINT16_ENCODED;
            }

            return retValue;
        }

        public Packet(byte[] frame)
        {
            if (!(frame is object))
            {
                Log.Debug(Resources.FRAME_IS_NULL);
                throw new ArgumentException(Resources.FRAME_IS_NULL);
            }

            if (frame.Length < 2)
            {
                Log.Debug(Resources.INVALID_FRAME_TOO_SHORT);
                throw new PacketFormatException(Resources.INVALID_FRAME_TOO_SHORT);
            }

            _code = frame[0];
            _length = DecodeLength(frame);
            _type = GetLengthEncoding(_length);
            int offset = GetOverhead(_type);
            data = new byte[_length];

            for (int i = 0; i < _length; ++i)
            {
                data[i] = frame[i + offset];
            }
        }

        private static int GetOverhead(PacketType type)
        {
            switch (type)
            {
                case PacketType.LENGTH_UINT8_ENCODED: return 2;
                case PacketType.LENGTH_UINT16_ENCODED: return 4;
                case PacketType.LENGTH_UINT32_ENCODED: return 6;
                default:
                    throw new InvalidOperationException(Resources.UNKNOWN_PACKET_ENCODING + (byte) type);
            }
        }

        private int DecodeLength(byte[] frame)
        {
            int retValue;

            if (frame[1] == (byte) PacketType.LENGTH_UINT32_ENCODED)
            {
                retValue = (int) BitConverter.ToUInt32(frame, 2);
            }
            else if (frame[1] == (byte) PacketType.LENGTH_UINT16_ENCODED)
            {
                retValue = (int)BitConverter.ToUInt16(frame, 2);
            }
            else
            {
                retValue = (int)frame[1];
            }

            if (retValue + GetOverhead(GetLengthEncoding(retValue)) != frame.Length)
            {
                var msg = String.Format(CultureInfo.CurrentCulture, 
                                        Resources.INVALID_PACKET_LENGTH, 
                                        retValue, 
                                        frame.Length);

                Log.Debug(msg);
                throw new PacketFormatException(msg);
            }

            return retValue;
        }

        private void EncodeLength(byte[] frame)
        {
            if (_type == PacketType.LENGTH_UINT8_ENCODED)
            {
                frame[1] = (byte)_length;
            }
            else if (_type == PacketType.LENGTH_UINT16_ENCODED)
            {
                frame[1] = (byte)PacketType.LENGTH_UINT16_ENCODED;
                Serialize(frame, 2, BitConverter.GetBytes((UInt16)_length));
            }
            else if (_type == PacketType.LENGTH_UINT32_ENCODED)
            {
                frame[1] = (byte) PacketType.LENGTH_UINT32_ENCODED;
                Serialize(frame, 2, BitConverter.GetBytes((UInt32)_length));
            }
            else
            {
                throw new InvalidOperationException(Resources.INVALID_PACKET_TYPE);
            }
        }

        public Packet CreateResponse(byte length)
        {
            return new Packet(_code, length);
        }

        public byte[] ToArray()
        {
            int offset = GetOverhead(_type);
            byte[] retValue = new byte[_length + offset];
            retValue[0] = _code;
            EncodeLength(retValue);

            for (int i = 0; i < _length; ++i)
            {
                retValue[i + offset] = data[i];
            }

            return retValue;
        }

        public byte CalculateChecksum(int offset, int length)
        {
            byte retValue = 0;

            if (offset + length <= Length)
            {
                byte[] input = new byte[length];

                for (int n = 0; n < length; ++n)
                {
                    input[n] = data[n + offset];
                }

                retValue = CRC8CCITT.Calculate(input);
            }

            return retValue;
        }

        public void InsertByte(int position, byte value)
        {
            VerifyPosition(data, position, 1);
            data[position] = value;
        }

        public void InsertSByte(int position, sbyte value)
        {
            VerifyPosition(data, position, 1);
            data[position] = (byte) value;
        }


        public void InsertUInt16(int position, UInt16 value)
        {
            Serialize(data, position, BitConverter.GetBytes(value));
        }

        public void InsertInt16(int position, Int16 value)
        {
            Serialize(data, position, BitConverter.GetBytes(value));
        }

        public void InsertUInt32(int position, UInt32 value)
        {
            Serialize(data, position, BitConverter.GetBytes(value));
        }

        public void InsertInt32(int position, Int32 value)
        {
            Serialize(data, position, BitConverter.GetBytes(value));
        }

        public void InsertString(int position, int maxSize, String value)
        {
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            var bytes = encoding.GetBytes(value);
            VerifyPosition(data, position, maxSize);

            for (int i = 0; i < bytes.Length; ++i)
            {
                data[position + i] = bytes[i];
            }

            for (int i = bytes.Length; i < maxSize; ++i)
            {
                data[position + i] = 0x00;
            }

        }

        public byte GetByte(int position)
        {
            VerifyPosition(data, position, 1);
            return data[position];
        }

        public sbyte GetSByte(int position)
        {
            return (sbyte) (data[position]);
        }

        public UInt16 GetUInt16(int position)
        {
            return BitConverter.ToUInt16(Deserialize(position, 2, data), 0);
        }

        public Int16 GetInt16(int position)
        {
            return BitConverter.ToInt16(Deserialize(position, 2, data), 0);
        }

        public UInt32 GetUInt32(int position)
        {
            return BitConverter.ToUInt32(Deserialize(position, 4, data), 0);
        }

        public Int32 GetInt32(int position)
        {
            return BitConverter.ToInt32(Deserialize(position, 4, data), 0);
        }

        public String GetString(int position, int size)
        {
            VerifyPosition(data, position, size);
            var bytes = new List<byte>();

            for (int i = 0; i < size; ++i)
            {
                if (data[position + i] != 0)
                    bytes.Add(data[position + i]);
            }

            return System.Text.Encoding.ASCII.GetString(bytes.ToArray());
        }

        private void Serialize(byte[] dest, int position, byte[] bytes)
        {
            VerifyPosition(dest, position, bytes.Length);

            if (ReverseEndianity)
            {
                Array.Reverse(bytes);
            }

            for (int i = 0; i < bytes.Length; ++i)
            {
                dest[position + i] = bytes[i];
            }
        }

        private byte[] Deserialize(int position, int size, byte[] src)
        {
            VerifyPosition(src, position, size);
            var retValue = new byte[size];

            for (int i = 0; i < size; ++i)
            {
                retValue[i] = src[position + i];
            }

            if (ReverseEndianity)
            {
                Array.Reverse(retValue);
            }

            return retValue;
        }

        private static void VerifyPosition(byte[] data, int position, int size)
        {
            if (!ValidPosition(data, position, size))
                throw new ArgumentOutOfRangeException(nameof(position));
        }

        private static bool ValidPosition(byte[] data, int position, int size)
        {
            return (position < data.Length) &&
                   (position + size <= data.Length);
        }

        private readonly byte _code;
        private readonly int _length;
        private readonly PacketType _type;
        private readonly byte[] data;
    }
}
