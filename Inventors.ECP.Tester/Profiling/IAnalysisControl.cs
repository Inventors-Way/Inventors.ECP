using Inventors.ECP.Profiling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventors.ECP.Tester.Profiling
{
    public interface IAnalysisControl 
    {
        bool Active { get; set; }

        bool Dirty { get; }

        void RefreshDisplay();

        Control Control { get; }
    }
}
