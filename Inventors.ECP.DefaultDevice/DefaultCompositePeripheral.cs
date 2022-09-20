using Inventors.ECP.Functions;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.ECP.DefaultDevice
{
    public class DefaultCompositePeripheral :
        BusPeripheral
    {
        private UInt32 count01;
        private UInt32 count02;

        public DefaultCompositePeripheral() :
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
            if (func.Address == 1)
            {
                EcpLog.Debug("Default Petripheral: DeviceIdentification");
                func.ManufactureID = Manufacturer.InventorsWay;
                func.Manufacture = "Inventors' Way ApS";
                func.DeviceID = UInt16.MaxValue;
                func.Device = "Default Peripheral 1";

                func.MajorVersion = 1;
                func.MinorVersion = 0;
                func.PatchVersion = 0;
                func.EngineeringVersion = 0;

                func.Checksum = 0;
                func.SerialNumber = 1;

                return 0;
            }
            else if (func.Address == 2)
            {
                EcpLog.Debug("Default Petripheral: DeviceIdentification");
                func.ManufactureID = Manufacturer.InventorsWay;
                func.Manufacture = "Inventors' Way ApS";
                func.DeviceID = UInt16.MaxValue;
                func.Device = "Default Peripheral 2";

                func.MajorVersion = 1;
                func.MinorVersion = 0;
                func.PatchVersion = 0;
                func.EngineeringVersion = 0;

                func.Checksum = 0;
                func.SerialNumber = 2;

                return 0;
            }

            return 1;
        }

        public int Accept(Ping func)
        {
            if (func.Address == 1)
            {
                func.Count = count01;
                ++count01;

                return 0;
            }
            else if (func.Address == 2)
            {
                func.Count = count02;
                ++count02;

                return 0;
            }

            return 1;
        }

        public int Accept(GetEndianness func)
        {
            EcpLog.Debug($"Peripheral: GetEndinanness [Address: {func.Address}]");

            return 0;
        }

        public int Accept(SetDebugSignal func)
        {
            EcpLog.Debug($"Peripheral: SetDebugSignal [Address: {func.Address}]");

            return 0;
        }

    }
}
