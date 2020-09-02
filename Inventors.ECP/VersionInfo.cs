using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.ECP
{
    public static class VersionInfo
    {
        private readonly static Version version = Assembly.GetAssembly(typeof(VersionInfo)).GetName().Version;

        public static string VersionDescription => 
            string.Format(CultureInfo.InvariantCulture, "{0}.{1}.{2}", version.Major, version.Minor, version.Build);        
    }
}
