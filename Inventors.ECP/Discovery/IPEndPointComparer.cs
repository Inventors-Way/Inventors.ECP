﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Inventors.ECP.Discovery
{
    class IPEndPointComparer : IComparer<IPEndPoint>
    {
        public static readonly IPEndPointComparer Instance = new IPEndPointComparer();

        public int Compare(IPEndPoint x, IPEndPoint y)
        {
            var c = String.Compare(x.Address.ToString(), y.Address.ToString(), StringComparison.Ordinal);
            if (c != 0) return c;
            return y.Port - x.Port;
        }
    }
}
