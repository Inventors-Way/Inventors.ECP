namespace Inventors.ECP.TestFramework.Profiling
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
            this.clearProfilerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.debugSignalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.analysisMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.timeSpanToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.s60timeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.s300timeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.s600timeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.maximumToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.minimumToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.eventsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.timeViolationsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.pausedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.signalSplitContainer = new System.Windows.Forms.SplitContainer();
            this.debugSignalTabControl = new System.Windows.Forms.TabControl();
            this.descriptionTextBox = new System.Windows.Forms.TextBox();
            this.hScrollBar = new System.Windows.Forms.HScrollBar();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.menuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.signalSplitContainer)).BeginInit();
            this.signalSplitContainer.Panel1.SuspendLayout();
            this.signalSplitContainer.Panel2.SuspendLayout();
            this.signalSplitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileMenu,
            this.debugSignalToolStripMenuItem,
            this.analysisMenu});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(1067, 28);
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
            this.fileMenu.Size = new System.Drawing.Size(46, 24);
            this.fileMenu.Text = "File";
            // 
            // clearProfilerToolStripMenuItem
            // 
            this.clearProfilerToolStripMenuItem.Name = "clearProfilerToolStripMenuItem";
            this.clearProfilerToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Delete)));
            this.clearProfilerToolStripMenuItem.Size = new System.Drawing.Size(193, 26);
            this.clearProfilerToolStripMenuItem.Text = "Reset";
            this.clearProfilerToolStripMenuItem.Click += new System.EventHandler(this.ClearProfilerToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(190, 6);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(193, 26);
            this.closeToolStripMenuItem.Text = "Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.CloseToolStripMenuItem_Click);
            // 
            // debugSignalToolStripMenuItem
            // 
            this.debugSignalToolStripMenuItem.Name = "debugSignalToolStripMenuItem";
            this.debugSignalToolStripMenuItem.Size = new System.Drawing.Size(113, 24);
            this.debugSignalToolStripMenuItem.Text = "Debug Signal";
            // 
            // analysisMenu
            // 
            this.analysisMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.timeSpanToolStripMenuItem,
            this.toolStripSeparator4,
            this.maximumToolStripMenuItem,
            this.minimumToolStripMenuItem,
            this.toolStripSeparator1,
            this.eventsToolStripMenuItem,
            this.timeViolationsToolStripMenuItem,
            this.toolStripSeparator3,
            this.pausedToolStripMenuItem});
            this.analysisMenu.Name = "analysisMenu";
            this.analysisMenu.Size = new System.Drawing.Size(76, 24);
            this.analysisMenu.Text = "Analysis";
            // 
            // timeSpanToolStripMenuItem
            // 
            this.timeSpanToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.s60timeToolStripMenuItem,
            this.s300timeToolStripMenuItem,
            this.s600timeToolStripMenuItem});
            this.timeSpanToolStripMenuItem.Name = "timeSpanToolStripMenuItem";
            this.timeSpanToolStripMenuItem.Size = new System.Drawing.Size(193, 26);
            this.timeSpanToolStripMenuItem.Text = "Time span";
            // 
            // s60timeToolStripMenuItem
            // 
            this.s60timeToolStripMenuItem.Checked = true;
            this.s60timeToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.s60timeToolStripMenuItem.Name = "s60timeToolStripMenuItem";
            this.s60timeToolStripMenuItem.Size = new System.Drawing.Size(122, 26);
            this.s60timeToolStripMenuItem.Text = "60s";
            this.s60timeToolStripMenuItem.Click += new System.EventHandler(this.s60timeChanged_Click);
            // 
            // s300timeToolStripMenuItem
            // 
            this.s300timeToolStripMenuItem.Name = "s300timeToolStripMenuItem";
            this.s300timeToolStripMenuItem.Size = new System.Drawing.Size(122, 26);
            this.s300timeToolStripMenuItem.Text = "300s";
            this.s300timeToolStripMenuItem.Click += new System.EventHandler(this.s300timeChanged_Click);
            // 
            // s600timeToolStripMenuItem
            // 
            this.s600timeToolStripMenuItem.Name = "s600timeToolStripMenuItem";
            this.s600timeToolStripMenuItem.Size = new System.Drawing.Size(122, 26);
            this.s600timeToolStripMenuItem.Text = "600s";
            this.s600timeToolStripMenuItem.Click += new System.EventHandler(this.s600timeChanged_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(190, 6);
            // 
            // maximumToolStripMenuItem
            // 
            this.maximumToolStripMenuItem.Checked = true;
            this.maximumToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.maximumToolStripMenuItem.Name = "maximumToolStripMenuItem";
            this.maximumToolStripMenuItem.Size = new System.Drawing.Size(193, 26);
            this.maximumToolStripMenuItem.Text = "Maximum";
            this.maximumToolStripMenuItem.Click += new System.EventHandler(this.MaximumToolStripMenuItem_Click);
            // 
            // minimumToolStripMenuItem
            // 
            this.minimumToolStripMenuItem.Checked = true;
            this.minimumToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.minimumToolStripMenuItem.Name = "minimumToolStripMenuItem";
            this.minimumToolStripMenuItem.Size = new System.Drawing.Size(193, 26);
            this.minimumToolStripMenuItem.Text = "Minimum";
            this.minimumToolStripMenuItem.Click += new System.EventHandler(this.MinimumToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(190, 6);
            // 
            // eventsToolStripMenuItem
            // 
            this.eventsToolStripMenuItem.Checked = true;
            this.eventsToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.eventsToolStripMenuItem.Name = "eventsToolStripMenuItem";
            this.eventsToolStripMenuItem.Size = new System.Drawing.Size(193, 26);
            this.eventsToolStripMenuItem.Text = "Events";
            this.eventsToolStripMenuItem.Click += new System.EventHandler(this.EventsToolStripMenuItem_Click);
            // 
            // timeViolationsToolStripMenuItem
            // 
            this.timeViolationsToolStripMenuItem.Checked = true;
            this.timeViolationsToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.timeViolationsToolStripMenuItem.Name = "timeViolationsToolStripMenuItem";
            this.timeViolationsToolStripMenuItem.Size = new System.Drawing.Size(193, 26);
            this.timeViolationsToolStripMenuItem.Text = "Time violations";
            this.timeViolationsToolStripMenuItem.Click += new System.EventHandler(this.TimeViolationsToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(190, 6);
            // 
            // pausedToolStripMenuItem
            // 
            this.pausedToolStripMenuItem.Name = "pausedToolStripMenuItem";
            this.pausedToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
            this.pausedToolStripMenuItem.Size = new System.Drawing.Size(193, 26);
            this.pausedToolStripMenuItem.Text = "Paused";
            this.pausedToolStripMenuItem.Click += new System.EventHandler(this.PausedToolStripMenuItem_Click);
            // 
            // timer
            // 
            this.timer.Enabled = true;
            this.timer.Tick += new System.EventHandler(this.Timer_Tick);
            // 
            // splitContainer
            // 
            this.splitContainer.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point(0, 28);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.signalSplitContainer);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.hScrollBar);
            this.splitContainer.Panel2.Controls.Add(this.pictureBox);
            this.splitContainer.Panel2.Resize += new System.EventHandler(this.SplitContainer_Panel2_Resize);
            this.splitContainer.Size = new System.Drawing.Size(1067, 661);
            this.splitContainer.SplitterDistance = 285;
            this.splitContainer.TabIndex = 2;
            // 
            // signalSplitContainer
            // 
            this.signalSplitContainer.BackColor = System.Drawing.Color.White;
            this.signalSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.signalSplitContainer.Location = new System.Drawing.Point(0, 0);
            this.signalSplitContainer.Name = "signalSplitContainer";
            this.signalSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // signalSplitContainer.Panel1
            // 
            this.signalSplitContainer.Panel1.Controls.Add(this.debugSignalTabControl);
            // 
            // signalSplitContainer.Panel2
            // 
            this.signalSplitContainer.Panel2.Controls.Add(this.descriptionTextBox);
            this.signalSplitContainer.Size = new System.Drawing.Size(281, 657);
            this.signalSplitContainer.SplitterDistance = 440;
            this.signalSplitContainer.TabIndex = 0;
            // 
            // debugSignalTabControl
            // 
            this.debugSignalTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.debugSignalTabControl.Location = new System.Drawing.Point(0, 0);
            this.debugSignalTabControl.Name = "debugSignalTabControl";
            this.debugSignalTabControl.SelectedIndex = 0;
            this.debugSignalTabControl.Size = new System.Drawing.Size(281, 440);
            this.debugSignalTabControl.TabIndex = 0;
            this.debugSignalTabControl.SelectedIndexChanged += new System.EventHandler(this.DebugSignalTabControl_SelectedIndexChanged);
            // 
            // descriptionTextBox
            // 
            this.descriptionTextBox.BackColor = System.Drawing.Color.White;
            this.descriptionTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.descriptionTextBox.Location = new System.Drawing.Point(0, 0);
            this.descriptionTextBox.Multiline = true;
            this.descriptionTextBox.Name = "descriptionTextBox";
            this.descriptionTextBox.ReadOnly = true;
            this.descriptionTextBox.Size = new System.Drawing.Size(281, 213);
            this.descriptionTextBox.TabIndex = 0;
            // 
            // hScrollBar
            // 
            this.hScrollBar.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.hScrollBar.Enabled = false;
            this.hScrollBar.Location = new System.Drawing.Point(0, 636);
            this.hScrollBar.Name = "hScrollBar";
            this.hScrollBar.Size = new System.Drawing.Size(774, 21);
            this.hScrollBar.TabIndex = 1;
            this.hScrollBar.Value = 20;
            this.hScrollBar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.HScrollBar_Scroll);
            // 
            // pictureBox
            // 
            this.pictureBox.BackColor = System.Drawing.Color.White;
            this.pictureBox.Location = new System.Drawing.Point(0, 0);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(749, 589);
            this.pictureBox.TabIndex = 0;
            this.pictureBox.TabStop = false;
            // 
            // ProfilerWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1067, 689);
            this.Controls.Add(this.splitContainer);
            this.Controls.Add(this.menuStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "ProfilerWindow";
            this.Text = "Profiler";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ProfilerWindow_FormClosing);
            this.SizeChanged += new System.EventHandler(this.ProfilerWindow_SizeChanged);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.signalSplitContainer.Panel1.ResumeLayout(false);
            this.signalSplitContainer.Panel2.ResumeLayout(false);
            this.signalSplitContainer.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.signalSplitContainer)).EndInit();
            this.signalSplitContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileMenu;
        private System.Windows.Forms.ToolStripMenuItem analysisMenu;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearProfilerToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.SplitContainer signalSplitContainer;
        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.TabControl debugSignalTabControl;
        private System.Windows.Forms.ToolStripMenuItem debugSignalToolStripMenuItem;
        private System.Windows.Forms.TextBox descriptionTextBox;
        private System.Windows.Forms.HScrollBar hScrollBar;
        private System.Windows.Forms.ToolStripMenuItem timeSpanToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem s60timeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem s300timeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem s600timeToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem maximumToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem minimumToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem eventsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem timeViolationsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem pausedToolStripMenuItem;
    }
}