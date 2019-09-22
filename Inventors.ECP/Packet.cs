using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.ECP
{
    public class Packet
    {
        public Packet(byte code, byte length)
        {
            this.code = code;
            this.length = length;
            data = new byte[length];
        }

        public Packet(byte[] frame)
        {
            if (frame.Length < 2)
            {
                throw new PacketFormatException("The frame is less than 2 bytes long");
            }
            if (frame.Length != frame[1] + 2)
            {
                throw new PacketFormatException(String.Format("Unexpected length, expected [ {0} ] but it was [ {1} ]", frame[1] + 2, frame.Length));
            }

            code = frame[0];
            length = frame[1];
            data = new byte[length];

            for (int i = 0; i < length; ++i)
                data[i] = frame[i + 2];
        }

        public Packet CreateResponse(byte length)
        {
            return new Packet(code, length);
        }

        public byte Code
        {
            get
            {
                return code;
            }
        }

        public bool IsFunction
        {
            get
            {
                return code < 128;
            }
        }

        public byte Length
        {
            get
            {
                return length;
            }
        }

        public bool Empty
        {
            get
            {
                return length == 0;
            }
        }

        public bool ReverseEndianity { get; set; }

        public byte[] ToArray()
        {
            byte[] retValue = new byte[length + 2];
            retValue[0] = code;
            retValue[1] = length;

            for (int i = 0; i < length; ++i)
                retValue[i + 2] = data[i];

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
            VerifyPosition(position, 1);
            data[position] = value;
        }

        public void InsertSByte(int position, sbyte value)
        {
            VerifyPosition(position, 1);
            data[position] = (byte) value;
        }


        public void InsertUInt16(int position, UInt16 value)
        {
            Serialize(position, BitConverter.GetBytes(value));
        }

        public void InsertInt16(int position, Int16 value)
        {
            Serialize(position, BitConverter.GetBytes(value));
        }

        public void InsertUInt32(int position, UInt32 value)
        {
            Serialize(position, BitConverter.GetBytes(value));
        }

        public void InsertInt32(int position, Int32 value)
        {
            Serialize(position, BitConverter.GetBytes(value));
        }

        public void InsertString(int position, int maxSize, String value)
        {
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            var bytes = encoding.GetBytes(value);
            VerifyPosition(position, maxSize);

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
            VerifyPosition(position, 1);
            return data[position];
        }

        public sbyte GetSByte(int position)
        {
            return (sbyte) (data[position]);
        }

        public UInt16 GetUInt16(int position)
        {
            return BitConverter.ToUInt16(Deserialize(position, 2), 0);
        }

        public Int16 GetInt16(int position)
        {
            return BitConverter.ToInt16(Deserialize(position, 2), 0);
        }

        public UInt32 GetUInt32(int position)
        {
            return BitConverter.ToUInt32(Deserialize(position, 4), 0);
        }

        public Int32 GetInt32(int position)
        {
            return BitConverter.ToInt32(Deserialize(position, 4), 0);
        }

        public String GetString(int position, int size)
        {
            VerifyPosition(position, size);
            var bytes = new List<byte>();

            for (int i = 0; i < size; ++i)
            {
                if (data[position + i] != 0)
                    bytes.Add(data[position + i]);
            }

            return System.Text.Encoding.ASCII.GetString(bytes.ToArray());
        }

        private void Serialize(int position, byte[] bytes)
        {
            VerifyPosition(position, bytes.Length);

            if (ReverseEndianity)
                Array.Reverse(bytes);

            for (int i = 0; i < bytes.Length; ++i)
                data[position + i] = bytes[i];
        }

        private byte[] Deserialize(int position, int size)
        {
            VerifyPosition(position, size);
            var retValue = new byte[size];

            for (int i = 0; i < size; ++i)
                retValue[i] = data[position + i];

            if (ReverseEndianity)
                Array.Reverse(retValue);

            return retValue;
        }

        private void VerifyPosition(int position, int size)
        {
            if (!ValidPosition(position, size))
                throw new ArgumentOutOfRangeException();
        }

        private bool ValidPosition(int position, int size)
        {
            return (position < length) &&
                   (position + size <= length);
        }

        private readonly byte code;
        private readonly byte length;
        private readonly byte[] data;
    }
}
