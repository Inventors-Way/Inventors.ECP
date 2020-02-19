using System;
using System.Collections.Generic;
using System.Text;

namespace Inventors.ECP
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1028:Enum Storage should be Int32", Justification = "Required as it is for embeded software")]
    public enum Manufacturer : UInt32
    {
        Invalid = 0,
        /// <summary>
        /// Inventors' Way ApS
        /// Morten Nielsens Vej 6
        /// 9200 Aalborg SV
        /// Denmark
        /// </summary>
        InventorsWay = 1,
        Detectronic = 2,
        Nocitech = 3,
        InnoCon = 4,
        /// <summary>
        /// Use only for testing purposes. 
        /// Please contact Inventors' Way at info@inventors.dk to get an MID allocated to your company. 
        /// This is free for non-profit or open source projects.
        /// </summary>
        Generic = uint.MaxValue
    }
}
