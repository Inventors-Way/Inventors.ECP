using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventors.ECP.Utility;

namespace Inventors.ECP
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1028:Enum Storage should be Int32", Justification = "<Pending>")]
    public enum LengthEncodingType
    {
        UInt8Encoding  = 0x00,
        UInt16Encoding = 0x01,
        UInt32Encoding = 0x02
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1028:Enum Storage should be Int32", Justification = "<Pending>")]
    public enum ChecksumAlgorithmType
    { 
        None     = 0x00,
        Additive = 0x04,
        CRC8CCIT = 0x08
    }

    public class Packet
    {
        #region Properties
        public byte Code => _code;

        public bool IsFunction => _code < 128;

        public int Length => _length;

        public LengthEncodingType LengthEncoding => _lengthEncoding;

        public bool Empty => _length == 0;

        public byte Address => _address;

        public bool AddressEnabled => _addressEnabled;

        public byte Checksum => _checksum;

        public ChecksumAlgorithmType ChecksumAlgorithm => _checksumType;

        public bool ReverseEndianity { get; set; }

        public bool Extended { get; }

        #endregion

        public Packet(byte code, int length)
        {
            _code = code;
            _length = length;
            _lengthEncoding = GetLengthEncoding(length);
            _addressEnabled = false;
            _checksumType = ChecksumAlgorithmType.None;

            switch (_lengthEncoding)
            {
                case LengthEncodingType.UInt16Encoding:
                case LengthEncodingType.UInt32Encoding:
                    Extended = true;
                    break;
                case LengthEncodingType.UInt8Encoding:
                    Extended = length >= 128;
                    break;
            }

            data = new byte[length];
        }

        public Packet(byte code, int length, ChecksumAlgorithmType checksum)
        {
            _code = code;
            _length = length;
            _lengthEncoding = GetLengthEncoding(length);
            _address = 0;
            _addressEnabled = false;
            _checksumType = checksum;
            Extended = true;
        }

        public Packet (byte code, int length, byte address, ChecksumAlgorithmType checksum)
        {
            _code = code;
            _length = length;
            _lengthEncoding = GetLengthEncoding(length);
            _address = address;
            _addressEnabled = true;
            _checksumType = checksum;
            Extended = true;
        }

        public Packet(byte[] frame)
        {
            if (!(frame is object))
                throw new ArgumentException(Resources.FRAME_IS_NULL);

            if (frame.Length < 2)
                throw new PacketFormatException(Resources.INVALID_FRAME_TOO_SHORT);

            // Parsing the code byte
            _code = frame[0];

            // Parsing the format byte
            if (frame[1] < 128)
            {
                _length = frame[1];
                _lengthEncoding = LengthEncodingType.UInt8Encoding;
                _addressEnabled = false;
                _checksumType = ChecksumAlgorithmType.None;
            }
            else
            {
                // Parse the length encoding
                switch ((LengthEncodingType) (0x03 & frame[1]))
                {
                    case LengthEncodingType.UInt8Encoding:
                        _lengthEncoding = LengthEncodingType.UInt8Encoding;
                        break;
                    case LengthEncodingType.UInt16Encoding:
                        _lengthEncoding = LengthEncodingType.UInt16Encoding;
                        break;
                    case LengthEncodingType.UInt32Encoding:
                        _lengthEncoding = LengthEncodingType.UInt32Encoding;
                        break;
                    default:
                        throw new InvalidOperationException($"Invalid length encoding [ {0x03 & frame[1]} ]");
                }

                // Parse the checksum field
                switch ((ChecksumAlgorithmType) (0xC0 & frame[1]))
                {
                    case ChecksumAlgorithmType.None:
                        _checksumType = ChecksumAlgorithmType.None;
                        break;
                    case ChecksumAlgorithmType.Additive:
                        _checksumType = ChecksumAlgorithmType.Additive;
                        break;
                    case ChecksumAlgorithmType.CRC8CCIT:
                        _checksumType = ChecksumAlgorithmType.CRC8CCIT;
                        break;
                    default:
                        throw new InvalidOperationException($"Invalid checksum type [ {0xC0 & frame[1]} ]");
                }

                // Parse the address bit.
                _addressEnabled = (0x10 & frame[1]) != 0;
            }

            if (Extended)
            {
                int offset = 0;
                _length = DecodeLength(frame);
                data = new byte[_length];

                for (int i = 0; i < _length; ++i)
                {
                    data[i] = frame[i + offset];
                }

                // Validate checksum
                switch (ChecksumAlgorithm)
                {
                    case ChecksumAlgorithmType.Additive:
                        {
                            _checksum = frame[GetChecksumOffset()];
                            byte actualChecksum = AdditiveChecksum.Calculate(frame, frame.Length - 1);

                            if (_checksum != actualChecksum)
                            {
                                throw new InvalidOperationException($"Checksum incorrect (expected: {_checksum}, actual: {actualChecksum})");
                            }
                        }
                        break;

                    case ChecksumAlgorithmType.CRC8CCIT:
                        {
                            _checksum = frame[GetChecksumOffset()];
                            byte actualChecksum = CRC8CCITT.Calculate(frame, frame.Length - 1);

                            if (_checksum != actualChecksum)
                            {
                                throw new InvalidOperationException($"Checksum incorrect (expected: {_checksum}, actual: {actualChecksum})");
                            }
                        }
                        break;

                    default:
                        break;
                }
            }
            else // Standard frame
            {
                data = new byte[_length];
                int offset = GetDataOffset();

                for (int i = 0; i < _length; ++i)
                {
                    data[i] = frame[i + offset];
                }
            }
        }

        private static LengthEncodingType GetLengthEncoding(int length)
        {
            var retValue = LengthEncodingType.UInt8Encoding;

            if (length > UInt16.MaxValue)
            {
                retValue = LengthEncodingType.UInt32Encoding;
            }
            else if (length > Byte.MaxValue)
            {
                retValue = LengthEncodingType.UInt16Encoding;
            }

            return retValue;
        }

        private static int DecodeLength(byte[] frame)
        {
            int retValue;

            if (frame[1] == (byte)LengthEncodingType.UInt32Encoding)
            {
                retValue = (int)BitConverter.ToUInt32(frame, 2);
            }
            else if (frame[1] == (byte)LengthEncodingType.UInt16Encoding)
            {
                retValue = BitConverter.ToUInt16(frame, 2);
            }
            else
            {
                retValue = frame[2];
            }

            return retValue;
        }

        private int GetLengthSize()
        {
            switch (LengthEncoding)
            {
                case LengthEncodingType.UInt8Encoding: return 1;
                case LengthEncodingType.UInt16Encoding: return 2;
                case LengthEncodingType.UInt32Encoding: return 4;
                default:
                    throw new InvalidOperationException($"Unknown length encoding [ {(byte)LengthEncoding} ]");
            }
        }

        private int GetDataOffset()
        {
            int retValue = GetLengthSize() + 2;

            if (AddressEnabled)
            {
                retValue += 1;
            }

            return retValue;
        }

        private int GetChecksumOffset() => GetDataOffset() + Length;

        private int GetPacketSize() => ChecksumAlgorithm == ChecksumAlgorithmType.None ?  
                                       GetChecksumOffset() + 1 : 
                                       GetChecksumOffset();

        private void EncodeLength(byte[] frame)
        {
            switch (LengthEncoding)
            {
                case LengthEncodingType.UInt8Encoding:
                    frame[2] = (byte)_length;
                    break;
                case LengthEncodingType.UInt16Encoding:
                    Serialize(frame, 2, BitConverter.GetBytes((UInt16)_length));
                    break;
                case LengthEncodingType.UInt32Encoding:
                    Serialize(frame, 2, BitConverter.GetBytes((UInt32)_length));
                    break;
                default:
                    throw new InvalidOperationException(Resources.INVALID_PACKET_TYPE);
            }
        }

        private void EncodeFormat(byte[] frame)
        {
            int format = ((int)LengthEncoding) + ((int)ChecksumAlgorithm) + (AddressEnabled ? 0x10 : 0x00);
            frame[1] = (byte)format;
        }

        public byte[] ToArray()
        {
            byte[] retValue;

            if (Extended)
            {
                int offset = GetDataOffset();
                retValue = new byte[GetPacketSize()];
                retValue[0] = Code;
                EncodeFormat(retValue);
                EncodeLength(retValue);

                if (AddressEnabled)
                {

                }

                for (int i = 0; i < _length; ++i)
                {
                    retValue[i + offset] = data[i];
                }

                switch (ChecksumAlgorithm)
                {
                    case ChecksumAlgorithmType.Additive:
                        _checksum = AdditiveChecksum.Calculate(retValue, retValue.Length - 1);
                        retValue[retValue.Length - 1] = _checksum;
                        break;
                    case ChecksumAlgorithmType.CRC8CCIT:
                        _checksum = CRC8CCITT.Calculate(retValue, retValue.Length - 1);
                        retValue[retValue.Length - 1] = _checksum;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                int offset = 2;
                retValue = new byte[_length + offset];
                retValue[0] = Code;
                retValue[1] = (byte)_length;

                for (int i = 0; i < _length; ++i)
                {
                    retValue[i + offset] = data[i];
                }

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
            ASCIIEncoding encoding = new ASCIIEncoding();
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

        public sbyte GetSByte(int position) => (sbyte) (data[position]);
        public UInt16 GetUInt16(int position) => BitConverter.ToUInt16(Deserialize(position, 2, data), 0);
        public Int16 GetInt16(int position) => BitConverter.ToInt16(Deserialize(position, 2, data), 0);
        public UInt32 GetUInt32(int position) => BitConverter.ToUInt32(Deserialize(position, 4, data), 0);
        public Int32 GetInt32(int position) => BitConverter.ToInt32(Deserialize(position, 4, data), 0);

        public String GetString(int position, int size)
        {
            VerifyPosition(data, position, size);
            var bytes = new List<byte>();

            for (int i = 0; i < size; ++i)
            {
                if (data[position + i] != 0)
                {
                    bytes.Add(data[position + i]);
                }
            }

            return Encoding.ASCII.GetString(bytes.ToArray());
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
            {
                throw new ArgumentOutOfRangeException(nameof(position));
            }
        }

        private static bool ValidPosition(byte[] data, int position, int size)
        {
            return (position < data.Length) &&
                   (position + size <= data.Length);
        }

        private readonly byte _code;
        private byte _checksum;
        private readonly ChecksumAlgorithmType _checksumType;
        private readonly byte _address;
        private readonly bool _addressEnabled;
        private readonly int _length;
        private readonly LengthEncodingType _lengthEncoding;
        private readonly byte[] data;
    }
}
