﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Inventors.ECP;

namespace Inventors.ECP.Tester
{
    public class Logger : ILogger
    {
        public delegate void InvokeDelegate();

        public void Add(DateTime time, LogLevel level, string str)
        {
            if (mBox != null)
            {
                if (mBox.InvokeRequired)
                {
                    mBox.BeginInvoke(new InvokeDelegate(() => mBox.AppendText(level.ToString() + ": " + str + System.Environment.NewLine)));
                }
                else
                    mBox.AppendText("[" + DateTime.Now.ToLongTimeString() + "]" + level.ToString() + ": " + str + System.Environment.NewLine);

            }
        }

        public void Initialize()
        {
        }

        public void AddMonitor(ILogger log)
        {
        }

        public TextBox Box
        {
            get
            {
                return mBox;
            }
            set
            {
                mBox = value;
            }
        }

        TextBox mBox = null;
    }
}
