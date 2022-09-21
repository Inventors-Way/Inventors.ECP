using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.ECP.TestFramework.Actions
{
    public class DialogEngine
    {
        public string GetString(string title, string value)
        {
            var dialog = new TextInputDialog(title, value);

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                return dialog.Value;

            throw new InvalidOperationException($"User didn't provide an answer for {title}");
        }

        public double GetNumber(string title, double value)
        {
            var dialog = new NumberInputDialog(title, value);

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                return dialog.Value;

            throw new InvalidOperationException($"User didn't provide an answer for {title}");
        }

        public int GetListIndex(string title, string values)
        {
            var dialog = new ListDialog(title, values);

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                return dialog.Index;

            throw new InvalidOperationException($"User didn't provide an answer for {title}");
        }
    }
}
