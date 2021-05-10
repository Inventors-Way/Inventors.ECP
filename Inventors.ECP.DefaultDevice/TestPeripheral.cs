using Inventors.ECP.DefaultDevice.Functions;
using Inventors.ECP.DefaultDevice.Messages;
using Inventors.ECP.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.ECP.DefaultDevice
{
    public class TestPeripheral :
        BusPeripheral
    {
        private UInt32 count;

        public TestPeripheral() :
            base(new SerialPortLayer())
        {
            Add(new DeviceIdentification());
            Add(new Ping());
            Add(new GetEndianness());
            Add(new SetDebugSignal());

            Add(new SimpleFunction());
            Add(new DataFunction());

            Add(new SimpleMessage());
            Add(new DataMessage());

            FunctionListener = this;
            MessageListener = this;
        }

        public int Accept(DeviceIdentification func)
        {
            Log.Debug("Test Petripheral: DeviceIdentification");
            func.ManufactureID = Manufacturer.InventorsWay;
            func.Manufacture = "Inventors' Way ApS";
            func.DeviceID = UInt16.MaxValue;
            func.Device = "Test Peripheral";

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
            Log.Debug("Peripheral: GetEndinanness");

            return 0;
        }

        public int Accept(SetDebugSignal func)
        {
            Log.Debug("Peripheral: SetDebugSignal");

            return 0;
        }

        public int Accept(SimpleFunction func)
        {
            func.Answer = func.Operand + 1;

            return 0;
        }

        public int Accept(DataFunction func)
        {
            FuncData.Clear();
            FuncData.AddRange(func.Data);

            return 0;
        }

        public void Accept(SimpleMessage msg)
        {
            X = msg.X;
        }

        public void Accept(DataMessage msg)
        {
            MsgData.Clear();
            MsgData.AddRange(msg.Data);
        }

        public List<int> FuncData { get; } = new List<int>();
        public List<int> MsgData { get; } = new List<int>();
        public int X { get; set; }
    }
}
