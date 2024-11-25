namespace Inventors.ECP.TestFramework
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
            components = new System.ComponentModel.Container();
            logEntry = new System.Windows.Forms.TextBox();
            logBox = new System.Windows.Forms.TextBox();
            timer = new System.Windows.Forms.Timer(components);
            SuspendLayout();
            // 
            // logEntry
            // 
            logEntry.Dock = System.Windows.Forms.DockStyle.Bottom;
            logEntry.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            logEntry.Location = new System.Drawing.Point(0, 446);
            logEntry.Margin = new System.Windows.Forms.Padding(4);
            logEntry.Name = "logEntry";
            logEntry.Size = new System.Drawing.Size(917, 25);
            logEntry.TabIndex = 1;
            logEntry.KeyDown += LogEntry_KeyDown;
            // 
            // logBox
            // 
            logBox.BackColor = System.Drawing.Color.White;
            logBox.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            logBox.Location = new System.Drawing.Point(0, 0);
            logBox.Multiline = true;
            logBox.Name = "logBox";
            logBox.ReadOnly = true;
            logBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            logBox.Size = new System.Drawing.Size(915, 444);
            logBox.TabIndex = 2;
            // 
            // timer
            // 
            timer.Tick += Timer_Tick;
            // 
            // LogControl
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(logBox);
            Controls.Add(logEntry);
            Margin = new System.Windows.Forms.Padding(4);
            Name = "LogControl";
            Size = new System.Drawing.Size(917, 471);
            SizeChanged += LogControl_SizeChanged;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private System.Windows.Forms.TextBox logEntry;
        private System.Windows.Forms.TextBox logBox;
        private System.Windows.Forms.Timer timer;
    }
}
