﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.ECP.Profiling
{
    public class TargetEvent :
        Record
    {
        public TargetEvent(string description)
        {
            Description = description;
        }

        public string Description { get; }
    }
}
