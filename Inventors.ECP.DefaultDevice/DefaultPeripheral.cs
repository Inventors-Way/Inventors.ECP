using Inventors.ECP.Functions;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.ECP.DefaultDevice
{
    public class DefaultPeripheral :
        BusPeripheral
    {
        private UInt32 count;

        public DefaultPeripheral() :
            base(new SerialPortLayer())
        {
            Add(new DeviceIdentification());
            Add(new Ping());
            Add(new GetEndianness());
            Add(new SetDebugSignal());

            FunctionListener = this;
            MessageListener = this;
        }

        public int Accept(DeviceIdentification func)
        {
            EcpLog.Debug("Default Petripheral: DeviceIdentification");
            func.ManufactureID = Manufacturer.InventorsWay;
            func.Manufacture = "Inventors' Way ApS";
            func.DeviceID = UInt16.MaxValue;
            func.Device = "Default Peripheral";

            func.MajorVersion = 1;
            func.MinorVersion = 0;
            func.PatchVersion = 0;
            func.EngineeringVersion = 0;

            func.Checksum = 0;
            func.SerialNumber = 1;

            return 0;
        }

        public int Accept(Ping func)
        {
            func.Count = count;
            ++count;

            return 0;
        }

        public int Accept(GetEndianness func)
        {
            EcpLog.Debug("Peripheral: GetEndinanness");

            return 0;
        }

        public int Accept(SetDebugSignal func)
        {
            EcpLog.Debug("Peripheral: SetDebugSignal");

            return 0;
        }
    }
}
