﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.ECP.Utility
{
    public static class CRC8CCITT
    {
        public static byte Calculate(byte[] data)
        {
            byte retValue = 0;

            if (data is object)
            {
                foreach (byte d in data)
                {
                    retValue = Update(retValue, d);
                }
            }

            return retValue;
        }

        public static byte Calculate(byte[] data, int length)
        {
            byte retValue = 0;

            if (data is object)
            {
                for (int n = 0; n < length; ++n)
                {
                    retValue = Update(retValue, data[n]);
                }
            }

            return retValue;
        }

        public static byte Update(byte inCrc, byte inData)
        {
            byte data = (byte) (inCrc ^ inData);

            for (byte i = 0; i < 8; i++)
            {
                if ((data & 0x80) != 0)
                {
                    data <<= 1;
                    data ^= 0x07;
                }
                else
                {
                    data <<= 1;
                }
            }

            return data;
        }
    }
}
