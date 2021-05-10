using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Inventors.ECP.Functions
{
    public class Ping : 
        DeviceFunction
    {
        public override byte Code => 0x02;

        public Ping() : base(requestLength: 0, responseLength: 4) { }

        public override FunctionDispatcher CreateDispatcher() =>
            new FunctionDispatcher(Code, () => new Ping());

        public override int Dispatch(dynamic listener) => listener.Accept(this);

        [XmlIgnore]
        [Category("Ping")]
        [Description("Number of pings answered by the slave")]
        public UInt32 Count
        {
            get => Response.GetUInt32(0);
            set => Response.InsertUInt32(0, value);
        }

        public override string ToString() => "[0x02] Ping";
    }
}
