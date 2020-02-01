using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Inventors.ECP.Communication
{
    public abstract class Statistics
    {
        public static string FormatRate(double rate, string unit = "B/s")
        {
            if (rate > 1048576)
            {
                return String.Format(CultureInfo.CurrentCulture, "{0:0.00}M{1}", rate / 1048576, unit);
            }
            else if (rate > 1024)
            {
                return String.Format(CultureInfo.CurrentCulture, "{0:0.00}k{1}", rate / 1024, unit);
            }
            else
            {
                return String.Format(CultureInfo.CurrentCulture, "{0:0.00}{1}", rate, unit);
            }
        }
    }
}
