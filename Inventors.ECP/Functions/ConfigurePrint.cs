using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.ECP.Functions
{
    public class ConfigurePrint :
        Function
    {
        private static byte ResponseLength = 0;

        public ConfigurePrint() : base(0x04) { }

        protected override bool IsResponseValid()
        {
            return response.Length == ResponseLength;
        }

        [Category("Enable")]
        [Description("Enable/Disable the Print system")]
        public bool Enable { get; set; } = true;

        [Category("Subsystems")]
        [Description("Subsystems to enable/disable")]
        public List<byte> Subsystems { get; set; } = new List<byte>();

        public override void OnSend()
        {
            int n = 1;
            request = new Packet(0x04, (byte) (1 + (Subsystems.Count < 255 ? Subsystems.Count : 255)));
            request.InsertByte(0, (byte)(Enable ? 1 : 0));

            foreach (var s in Subsystems)
            {
                if (n < 255)
                {
                    request.InsertByte(n, s);
                    ++n;
                }
            }
        }

        public override string ToString()
        {
            return "[0x04] Configure Print";
        }
    }
}
