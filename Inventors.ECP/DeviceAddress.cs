using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.ECP
{
    public class DeviceAddress
    {
        public DeviceAddress(byte value, string name)
        {
            Value = value;
            Name = name;
        }

        public byte Value { get; }

        public string Name { get; }
    }
}
