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
            this.logBox = new System.Windows.Forms.RichTextBox();
            this.logEntry = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // logBox
            // 
            this.logBox.BackColor = System.Drawing.Color.White;
            this.logBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logBox.Location = new System.Drawing.Point(0, 0);
            this.logBox.Name = "logBox";
            this.logBox.ReadOnly = true;
            this.logBox.Size = new System.Drawing.Size(786, 408);
            this.logBox.TabIndex = 0;
            this.logBox.Text = "";
            // 
            // logEntry
            // 
            this.logEntry.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.logEntry.Location = new System.Drawing.Point(0, 388);
            this.logEntry.Name = "logEntry";
            this.logEntry.Size = new System.Drawing.Size(786, 20);
            this.logEntry.TabIndex = 1;
            this.logEntry.KeyDown += new System.Windows.Forms.KeyEventHandler(this.LogEntry_KeyDown);
            // 
            // LogControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.logEntry);
            this.Controls.Add(this.logBox);
            this.Name = "LogControl";
            this.Size = new System.Drawing.Size(786, 408);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox logBox;
        private System.Windows.Forms.TextBox logEntry;
    }
}
