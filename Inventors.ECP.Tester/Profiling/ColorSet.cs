using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.ECP.Tester.Profiling
{
    public class ColorSet
    {
        private static readonly Color[] colorSet = new Color[]
        {
            Color.Blue,
            Color.Red,
            Color.Black,
            Color.DarkMagenta
        };

        private Dictionary<int, Color> colors = new Dictionary<int, Color>();

        public void Reset()
        {
            colors.Clear();
        }

        public Color GetColor(int code)
        {
            if (colors.ContainsKey(code))
            {
                return colors[code];
            }
            else
            {
                colors.Add(code, colorSet[colors.Count % colorSet.Length]);
                return colors[code];
            }
        }
    }
}
