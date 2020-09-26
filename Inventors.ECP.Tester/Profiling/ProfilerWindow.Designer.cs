namespace Inventors.ECP.Tester.Profiling
{
    partial class ProfilerWindow
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProfilerWindow));
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.analysisMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.timeMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.TimeSpan60sMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.TimeSpan300sMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.TimeSpan600sMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.TimeSpanOffMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel = new System.Windows.Forms.Panel();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearProfilerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileMenu,
            this.analysisMenu,
            this.timeMenu});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(800, 24);
            this.menuStrip.TabIndex = 0;
            this.menuStrip.Text = "menuStrip1";
            // 
            // fileMenu
            // 
            this.fileMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.clearProfilerToolStripMenuItem,
            this.toolStripSeparator2,
            this.closeToolStripMenuItem});
            this.fileMenu.Name = "fileMenu";
            this.fileMenu.Size = new System.Drawing.Size(37, 20);
            this.fileMenu.Text = "File";
            // 
            // analysisMenu
            // 
            this.analysisMenu.Name = "analysisMenu";
            this.analysisMenu.Size = new System.Drawing.Size(62, 20);
            this.analysisMenu.Text = "Analysis";
            // 
            // timeMenu
            // 
            this.timeMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TimeSpan60sMenuItem,
            this.TimeSpan300sMenuItem,
            this.TimeSpan600sMenuItem,
            this.toolStripSeparator1,
            this.TimeSpanOffMenuItem});
            this.timeMenu.Name = "timeMenu";
            this.timeMenu.Size = new System.Drawing.Size(45, 20);
            this.timeMenu.Text = "Time";
            // 
            // TimeSpan60sMenuItem
            // 
            this.TimeSpan60sMenuItem.Name = "TimeSpan60sMenuItem";
            this.TimeSpan60sMenuItem.Size = new System.Drawing.Size(97, 22);
            this.TimeSpan60sMenuItem.Text = "60s";
            this.TimeSpan60sMenuItem.Click += new System.EventHandler(this.TimeSpan60s_Click);
            // 
            // TimeSpan300sMenuItem
            // 
            this.TimeSpan300sMenuItem.Name = "TimeSpan300sMenuItem";
            this.TimeSpan300sMenuItem.Size = new System.Drawing.Size(97, 22);
            this.TimeSpan300sMenuItem.Text = "300s";
            this.TimeSpan300sMenuItem.Click += new System.EventHandler(this.TimeSpan300s_Click);
            // 
            // TimeSpan600sMenuItem
            // 
            this.TimeSpan600sMenuItem.Name = "TimeSpan600sMenuItem";
            this.TimeSpan600sMenuItem.Size = new System.Drawing.Size(97, 22);
            this.TimeSpan600sMenuItem.Text = "600s";
            this.TimeSpan600sMenuItem.Click += new System.EventHandler(this.TimeSpan600s_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(94, 6);
            // 
            // TimeSpanOffMenuItem
            // 
            this.TimeSpanOffMenuItem.Name = "TimeSpanOffMenuItem";
            this.TimeSpanOffMenuItem.Size = new System.Drawing.Size(97, 22);
            this.TimeSpanOffMenuItem.Text = "Off";
            this.TimeSpanOffMenuItem.Click += new System.EventHandler(this.TimeSpanOffToolStripMenuItem_Click);
            // 
            // panel
            // 
            this.panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel.Location = new System.Drawing.Point(0, 24);
            this.panel.Name = "panel";
            this.panel.Size = new System.Drawing.Size(800, 536);
            this.panel.TabIndex = 1;
            // 
            // timer
            // 
            this.timer.Enabled = true;
            this.timer.Interval = 500;
            this.timer.Tick += new System.EventHandler(this.Timer_Tick);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(177, 6);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.closeToolStripMenuItem.Text = "Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.CloseToolStripMenuItem_Click);
            // 
            // clearProfilerToolStripMenuItem
            // 
            this.clearProfilerToolStripMenuItem.Name = "clearProfilerToolStripMenuItem";
            this.clearProfilerToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.clearProfilerToolStripMenuItem.Text = "Clear Profiler";
            this.clearProfilerToolStripMenuItem.Click += new System.EventHandler(this.ClearProfilerToolStripMenuItem_Click);
            // 
            // ProfilerWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 560);
            this.Controls.Add(this.panel);
            this.Controls.Add(this.menuStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip;
            this.Name = "ProfilerWindow";
            this.Text = "Profiler";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ProfilerWindow_FormClosing);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileMenu;
        private System.Windows.Forms.ToolStripMenuItem analysisMenu;
        private System.Windows.Forms.Panel panel;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.ToolStripMenuItem timeMenu;
        private System.Windows.Forms.ToolStripMenuItem TimeSpan60sMenuItem;
        private System.Windows.Forms.ToolStripMenuItem TimeSpan300sMenuItem;
        private System.Windows.Forms.ToolStripMenuItem TimeSpan600sMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem TimeSpanOffMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearProfilerToolStripMenuItem;
    }
}