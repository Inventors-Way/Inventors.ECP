using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventors.Logging;

namespace Inventors.ECP
{
    public enum PacketType
    {
        NORMAL_LENGTH = 0,
        EXTENDED_LENGTH
    }

    public class Packet
    {

        public Packet(byte code, int length)
        {
            this._code = code;
            this._length = length;
            this._type = length >= 255 ? PacketType.EXTENDED_LENGTH : PacketType.NORMAL_LENGTH;            
            data = new byte[length];
        }

        public Packet(byte[] frame)
        {
            if (frame.Length < 2)
            {
                Log.Debug("The frame is less than 2 bytes long");
                throw new PacketFormatException("The frame is less than 2 bytes long");
            }

            _code = frame[0];
            _length = DecodeLength(frame);
            this._type = _length >= 255 ? PacketType.EXTENDED_LENGTH : PacketType.NORMAL_LENGTH;
            int offset = _type == PacketType.NORMAL_LENGTH ? 2 : 6;
            data = new byte[_length];

            for (int i = 0; i < _length; ++i)
            {
                data[i] = frame[i + offset];
            }
        }

        private int DecodeLength(byte[] frame)
        {
            int retValue = 0;
            int overhead = 2;

            if (frame[1] == 0xFF)
            {
                retValue = (int) BitConverter.ToUInt32(frame, 2);
                overhead = 6;
            }
            else
            {
                retValue = (int)frame[1];
            }

            if (retValue + overhead != frame.Length)
            {
                Log.Debug("Unexpected length, expected [ {0} ] but it was [ {1} ]", retValue, frame.Length);
                throw new PacketFormatException(String.Format("Unexpected length, expected [ {0} ] but it was [ {1} ]", retValue, frame.Length));
            }

            return retValue;
        }

        private void EncodeLength(byte[] frame)
        {
            if (_type == PacketType.NORMAL_LENGTH)
            {
                frame[1] = (byte)_length;
            }
            else if (_type == PacketType.EXTENDED_LENGTH)
            {
                frame[1] = 0xFF;
                Serialize(frame, 2, BitConverter.GetBytes((UInt32)_length));
            }
            else
            {
                throw new InvalidOperationException("Invalid packet type");
            }
        }

        public Packet CreateResponse(byte length)
        {
            return new Packet(_code, length);
        }

        public byte Code
        {
            get
            {
                return _code;
            }
        }

        public bool IsFunction
        {
            get
            {
                return _code < 128;
            }
        }

        public int Length
        {
            get
            {
                return _length;
            }
        }

        public bool Empty
        {
            get
            {
                return _length == 0;
            }
        }

        public bool ReverseEndianity { get; set; }

        public byte[] ToArray()
        {
            int offset = _type == PacketType.NORMAL_LENGTH ? 2 : 6;
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

        private void VerifyPosition(byte[] data, int position, int size)
        {
            if (!ValidPosition(data, position, size))
                throw new ArgumentOutOfRangeException();
        }

        private bool ValidPosition(byte[] data, int position, int size)
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
