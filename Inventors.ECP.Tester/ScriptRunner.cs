using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.ECP.Tester
{
    public class ScriptRunner
    {
        public event EventHandler<bool> Completed;

        private readonly Device device;

        public ScriptRunner(Device device)
        {
            if (device is null)
                throw new ArgumentNullException(nameof(device));

            this.device = device;
        }

        public void Run(IScript script)
        {
            Task.Run(() =>
            {
                bool status = true;

                foreach (var function in script.Functions)
                {
                    try
                    {
                        device.Execute(function);
                        Report(function);
                    }
                    catch (Exception e)
                    {
                        Log.Error(e.Message);
                        status = false;
                        break;
                    }
                }

                Completed?.Invoke(this, status);
            });
        }

        private void Report(DeviceFunction function)
        {
            var builder = new StringBuilder();
            builder.AppendLine(function.ToString());
            builder.Append(function.ReportResponse());
            Log.Status(builder.ToString());
        }
    }
}
