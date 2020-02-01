using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.ECP
{
    public class Frame
    {
        public const byte DLE = 0xFF;
        public const byte STX = 0xF1;
        public const byte ETX = 0xF2;

        public static byte[] Encode(byte[] packet)
        {
            if (!(packet is object))
                throw new ArgumentNullException(nameof(packet));

            var frame = new Frame();
            
            frame.AddStartOfText();

            foreach (var x in packet)
            {
                frame.AddData(x);
            }

            frame.AddEndOfText();

            return frame.GetData();
        }

        #region Implementation of Framing

        private Frame()
        {
            buffer = new List<byte>();
        }

        private void AddData(byte x)
        {
            if (x == DLE)
            {
                buffer.Add(DLE);
                buffer.Add(DLE);
            }
            else
                buffer.Add(x);
        }

        private void AddEndOfText()
        {
            buffer.Add(DLE);
            buffer.Add(ETX);
        }

        private void AddStartOfText()
        {
            buffer.Add(DLE);
            buffer.Add(STX);
        }

        private byte[] GetData()
        {
            return buffer.ToArray();
        }

        private readonly List<byte> buffer;

        #endregion
    }
}
