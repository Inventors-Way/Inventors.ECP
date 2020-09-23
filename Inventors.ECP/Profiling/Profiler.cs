using Inventors.ECP.Communication;
using Inventors.ECP.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Inventors.ECP.Profiling
{
    public class Profiler :
        IProfiler
    {
        private readonly object dataLock = new object();

        #region Enabled Property
        private readonly LockedVariable<bool> _enabled = new LockedVariable<bool>(false);

        public bool Enabled 
        {
            get => _enabled.Get(dataLock);
            set => _enabled.Set(dataLock, value);
        }

        #endregion

        public void Add(TargetEvent e)
        {
            if (Enabled)
            {

            }
        }

        public void Add(TimingRecord record)
        {
            if (Enabled)
            {

            }
        }

        public void Add(TimingViolation violation)
        {
            if (Enabled)
            {

            }
        }

        public void Add(Packet packet)
        {
            if (Enabled)
            {

            }
        }

        public void Add(CommRecord record)
        {
            if (Enabled)
            {

            }
        }
    }
}
