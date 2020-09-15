using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.ECP.Profiling
{
    public class ProfileTiming
    {
        private static ProfileTiming _instance;
        private DateTime _refTime;


        private static ProfileTiming Instance
        {
            get
            {
                if (_instance is null)
                    _instance = new ProfileTiming();

                return _instance;
            }
        }

        private ProfileTiming()
        {
            _refTime = DateTime.Now;
        }

        public static void Reset() =>
            Instance._refTime = DateTime.Now;

        public static double Mark() =>
            (DateTime.Now - Instance._refTime).TotalSeconds;
    }
}
