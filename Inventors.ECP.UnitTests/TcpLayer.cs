using Microsoft.VisualStudio.TestTools.UnitTesting;
using Inventors.ECP;

namespace Inventors.ECP.UnitTests
{
    [TestClass]
    public class TcpLayer
    {
        [TestMethod]
        public void GetAddress()
        {
            var address = TcpServerLayer.GetIpAddress();
            Assert.IsFalse(string.IsNullOrEmpty(address));
        }
    }
}
