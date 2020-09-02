﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Inventors.ECP
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1028:Enum Storage should be Int32", Justification = "Required as it is for embeded software")]
    public enum Manufacturer : UInt32
    {
        /// <summary>
        /// Invalid MID, used to signal an error.
        /// </summary>
        Invalid = 0,
        /// <summary>
        /// Inventors' Way ApS
        /// </summary>
        InventorsWay = 1,
        /// <summary>
        /// Detectronic A/S
        /// </summary>
        Detectronic = 2,
        /// <summary>
        /// Nocitech ApS
        /// </summary>
        Nocitech = 3,
        /// <summary>
        /// InnoCon Medical ApS
        /// </summary>
        InnoCon = 4,
        /// <summary>
        /// Nordic-NeuroSTIM ApS
        /// </summary>
        NordicNeuroSTIM = 5,
        /// <summary>
        /// Use only for testing purposes. Please contact Inventors' Way at info@inventors.dk to get an MID allocated to your company, which is free for non-profit or open source projects.
        /// </summary>
        Generic = uint.MaxValue
    }
}
