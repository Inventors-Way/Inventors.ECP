﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;
using System.IO;

namespace Inventors.ECP.Tester
{
    public partial class LogControl : 
        UserControl,
        ILogger
    {
        private readonly StringBuilder logBuffer = new StringBuilder();
        private readonly object lockObject = new object();
        private bool _paused = false;
        private bool _autosave = true;
        private string logFile = null;

        public delegate void InvokeDelegate();

        public LogLevel Level { get; set; } = LogLevel.STATUS;      
      
        public string Content => logBox.Text;

        public LogControl()
        {
            InitializeComponent();
            logBox.VisibleChanged += (o, e) => ScrollToEnd();
            ResizeLogBox();
            timer.Enabled = true;
        }

        public void InitializeLogFile(string directory)
        {
            var time = DateTime.Now;
            logFile = Path.Combine(directory, $"ECPLOG-{time.Year}.{time.Month}.{time.Day}-{time.Hour}h{time.Minute}m{time.Second}s.txt");
        }

        public bool Paused
        {
            get => _paused;
            set => _paused = value;
        }

        public bool AutoSave
        {
            get => _autosave;
            set => _autosave = value;
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
            lock (lockObject)
            {
                logBuffer.AppendLine(String.Format(CultureInfo.CurrentCulture, 
                                                   "{0} {1, -6} {2}", 
                                                   time, 
                                                   level, 
                                                   message));
            }
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

        internal void Clear()
        {
            logBox.Text = "";

            if (!string.IsNullOrEmpty(logFile))
            {
                try
                {
                    if (File.Exists(logFile))
                    {
                        File.Delete(logFile);
                    }
                }
                catch { } // eat everything
            }
        }

        private void LogControl_SizeChanged(object sender, EventArgs e) =>
            ResizeLogBox();

        private void ResizeLogBox()
        {
            logBox.Size = new Size(width: Width, height: Height - logEntry.Height);
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (_paused)
                return;

            lock (lockObject)
            {
                if (logBuffer is null)
                    return;

                var content = logBuffer.ToString();
                logBuffer.Clear();

                if (string.IsNullOrEmpty(content))
                    return;

                logBox.AppendText(content);

                if (_autosave)
                    File.AppendAllText(logFile, content);

                ScrollToEnd();
            }
        }
    }
}
