using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.ECP.DefaultDevice.Functions
{
    public class SPIConfigureFunction :
        DeviceFunction
    {
		public enum SPI_DORD
		{
			DORD_LSB = 0,
			DORD_MSB
		}

		public enum SPI_CPOL
		{
			CPOL0 = 0,
			CPOL1
		}

		public enum SPI_CPHA
		{
			CPHA0 = 0,
			CPHA1
		}

		public enum SPI_CLK
		{
			CLKDIV_02 = 0,
			CLKDIV_04,
			CLKDIV_08,
			CLKDIV_16,
			CLKDIV_32,
			CLKDIV_64,
			CLKDIV_128
		}

		public static readonly int REQUEST_SIZE = 4;

        public override byte Code => 0x20;

        public SPIConfigureFunction() : base(requestLength: REQUEST_SIZE, responseLength: 0)
        {
        }

		[Category("Configuration")]
		public SPI_DORD DORD
        {
			get => (SPI_DORD) Request.GetByte(0);
			set => Request.InsertByte(0, (byte)value);
        }

		[Category("Configuration")]
		public SPI_CPOL CPOL
		{
			get => (SPI_CPOL)Request.GetByte(1);
			set => Request.InsertByte(1, (byte)value);
		}

		[Category("Configuration")]
		public SPI_CPHA CPHA
		{
			get => (SPI_CPHA)Request.GetByte(2);
			set => Request.InsertByte(2, (byte)value);
		}

		[Category("Configuration")]
		public SPI_CLK CLK
		{
			get => (SPI_CLK)Request.GetByte(3);
			set => Request.InsertByte(3, (byte)value);
		}

		public override FunctionDispatcher CreateDispatcher() => new FunctionDispatcher(Code, () => new SPIConfigureFunction());

        public override int Dispatch(dynamic listener) => listener.Accept(this);

		public override string ToString() => "[0x20] SPI Configure";
	}
}
