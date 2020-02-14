using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventors.ECP.DeviceHost.Actions
{
    public class VerifyPackage :
        PackageAction
    {
        public VerifyPackage(DevicePackage package) : base(package) { }

        public override bool Execute()
        {
            bool retValue = false;
            var msg = String.Format($"Please check that the SHA-256 checksum [ {CalculateChecksum()} ] is correct and matches the one provided by the provider of the device package");

            if (MessageBox.Show(msg, "Verify Package", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                retValue = true;
            }

            return retValue;
        }

        public override void Revert() { } // Nothing to do

        private string CalculateChecksum()
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                using (var file = File.Open(Package.SourceFile, FileMode.Open))
                {
                    return Bytes2String(sha256.ComputeHash(file));
                }
            }
        }

        public static string Bytes2String(byte[] array)
        {
            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < array.Length; i++)
            {
                builder.AppendFormat($"{array[i]:X2}");
                if ((i % 2) == 1)
                {
                    builder.AppendFormat(" ");
                }
            }

            return builder.ToString();
        }
    }
}
