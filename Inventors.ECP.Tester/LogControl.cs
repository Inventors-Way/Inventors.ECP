using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;

namespace Inventors.ECP.Tester
{
    public partial class LogControl : 
        UserControl,
        ILogger
    {
        public delegate void InvokeDelegate();

        public LogLevel Level { get; set; } = LogLevel.STATUS;

        public Color DebugColor { get; set; } = Color.Blue;

        public Color StatusColor { get; set; } = Color.Black;

        public Color ErrorColor { get; set; } = Color.Red;
        
        public string Content => logBox.Text;

        public LogControl()
        {
            InitializeComponent();
            logBox.VisibleChanged += (o, e) => ScrollToEnd();
            ResizeLogBox();
        }

        private void ScrollToEnd()
        {
            if (logBox.Visible)
            {
                logBox.SelectionStart = logBox.Text.Length;
                logBox.ScrollToCaret();
            }
        }

        public void Initialize() => logBox.Text = "";

        public void Add(DateTime time, LogLevel level, string message)
        {
            if (logBox.InvokeRequired)
            {
                logBox.BeginInvoke(new InvokeDelegate(() => LogText(FormatMessage(time, message), GetColor(level))));
            }
            else
            {
                LogText(FormatMessage(time, message), GetColor(level));
            }
        }

        private static string FormatMessage(DateTime time, string message) =>
            String.Format(CultureInfo.CurrentCulture, "{0}| {1}", time, message);

        public void LogText(string text, Color color)
        {
            logBox.AppendText(text, color);
            ScrollToEnd();
        }

        private void LogEntry_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                if (logEntry.Text.Length > 0)
                {
                    switch (Level)
                    {
                        case LogLevel.DEBUG:
                            Log.Debug(logEntry.Text);
                            break;
                        case LogLevel.STATUS:
                            Log.Status(logEntry.Text);
                            break;
                        case LogLevel.ERROR:
                            Log.Error(logEntry.Text);
                            break;
                        default:
                            break;
                    }
                }

                logEntry.Text = "";
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        public Color GetColor(LogLevel level)
        {
            Color retValue = StatusColor;

            switch (level)
            {
                case LogLevel.DEBUG:
                    retValue = DebugColor;
                    break;

                case LogLevel.STATUS:
                    retValue = StatusColor;
                    break;

                case LogLevel.ERROR:
                    retValue = ErrorColor;
                    break;
            }

            return retValue;
        }

        internal void Clear() => logBox.Text = "";

        private void LogControl_SizeChanged(object sender, EventArgs e) =>
            ResizeLogBox();

        private void ResizeLogBox()
        {
            logBox.Size = new Size(width: Width, height: Height - logEntry.Height);
        }
    }

    public static class RichTextBoxExtensions
    {
        public static void AppendText(this RichTextBox box, string text, Color color)
        {
            if (box is null)
                throw new ArgumentNullException(nameof(box));

            box.SelectionStart = box.TextLength;
            box.SelectionLength = 0;

            box.SelectionColor = color;
            box.AppendText(text);
            box.AppendText(Environment.NewLine);
            box.SelectionColor = box.ForeColor;
        }
    }
}
