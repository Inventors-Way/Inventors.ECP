using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.ECP.Utility
{
    public static class AdditiveChecksum
    {
        public static byte Calculate(byte[] data)
        {
            byte retValue = 0;

            if (data is object)
            {
                foreach (byte d in data)
                {
                    retValue += d;
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
                    retValue += data[n];
                }
            }

            return retValue;
        }
    }
}
