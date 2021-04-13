namespace Inventors.ECP.Tester
{
    partial class LogControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.logEntry = new System.Windows.Forms.TextBox();
            this.logBox = new System.Windows.Forms.TextBox();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // logEntry
            // 
            this.logEntry.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.logEntry.Location = new System.Drawing.Point(0, 480);
            this.logEntry.Margin = new System.Windows.Forms.Padding(4);
            this.logEntry.Name = "logEntry";
            this.logEntry.Size = new System.Drawing.Size(1048, 22);
            this.logEntry.TabIndex = 1;
            this.logEntry.KeyDown += new System.Windows.Forms.KeyEventHandler(this.LogEntry_KeyDown);
            // 
            // logBox
            // 
            this.logBox.BackColor = System.Drawing.Color.White;
            this.logBox.Font = new System.Drawing.Font("Courier New", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.logBox.Location = new System.Drawing.Point(0, 0);
            this.logBox.Multiline = true;
            this.logBox.Name = "logBox";
            this.logBox.ReadOnly = true;
            this.logBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.logBox.Size = new System.Drawing.Size(1045, 473);
            this.logBox.TabIndex = 2;
            // 
            // timer
            // 
            this.timer.Tick += new System.EventHandler(this.Timer_Tick);
            // 
            // LogControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.logBox);
            this.Controls.Add(this.logEntry);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "LogControl";
            this.Size = new System.Drawing.Size(1048, 502);
            this.SizeChanged += new System.EventHandler(this.LogControl_SizeChanged);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox logEntry;
        private System.Windows.Forms.TextBox logBox;
        private System.Windows.Forms.Timer timer;
    }
}
