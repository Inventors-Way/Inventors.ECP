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
            this.clearProfilerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.debugSignalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.analysisMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.signalSplitContainer = new System.Windows.Forms.SplitContainer();
            this.debugSignalTabControl = new System.Windows.Forms.TabControl();
            this.descriptionTextBox = new System.Windows.Forms.TextBox();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.hScrollBar = new System.Windows.Forms.HScrollBar();
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
            this.clearProfilerToolStripMenuItem.Size = new System.Drawing.Size(178, 26);
            this.clearProfilerToolStripMenuItem.Text = "Clear Profiler";
            this.clearProfilerToolStripMenuItem.Click += new System.EventHandler(this.ClearProfilerToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(175, 6);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(178, 26);
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
            this.analysisMenu.Name = "analysisMenu";
            this.analysisMenu.Size = new System.Drawing.Size(76, 24);
            this.analysisMenu.Text = "Analysis";
            // 
            // timer
            // 
            this.timer.Enabled = true;
            this.timer.Interval = 500;
            this.timer.Tick += new System.EventHandler(this.Timer_Tick);
            // 
            // toolStrip
            // 
            this.toolStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip.Location = new System.Drawing.Point(0, 28);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(1067, 25);
            this.toolStrip.TabIndex = 1;
            this.toolStrip.Text = "toolStrip1";
            // 
            // splitContainer
            // 
            this.splitContainer.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point(0, 53);
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
            this.splitContainer.Size = new System.Drawing.Size(1067, 636);
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
            this.signalSplitContainer.Size = new System.Drawing.Size(281, 632);
            this.signalSplitContainer.SplitterDistance = 425;
            this.signalSplitContainer.TabIndex = 0;
            // 
            // debugSignalTabControl
            // 
            this.debugSignalTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.debugSignalTabControl.Location = new System.Drawing.Point(0, 0);
            this.debugSignalTabControl.Name = "debugSignalTabControl";
            this.debugSignalTabControl.SelectedIndex = 0;
            this.debugSignalTabControl.Size = new System.Drawing.Size(281, 425);
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
            this.descriptionTextBox.Size = new System.Drawing.Size(281, 203);
            this.descriptionTextBox.TabIndex = 0;
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
            // hScrollBar
            // 
            this.hScrollBar.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.hScrollBar.Location = new System.Drawing.Point(0, 611);
            this.hScrollBar.Name = "hScrollBar";
            this.hScrollBar.Size = new System.Drawing.Size(774, 21);
            this.hScrollBar.TabIndex = 1;
            this.hScrollBar.Value = 20;
            // 
            // ProfilerWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1067, 689);
            this.Controls.Add(this.splitContainer);
            this.Controls.Add(this.toolStrip);
            this.Controls.Add(this.menuStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "ProfilerWindow";
            this.Text = "Profiler";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ProfilerWindow_FormClosing);
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
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.SplitContainer signalSplitContainer;
        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.TabControl debugSignalTabControl;
        private System.Windows.Forms.ToolStripMenuItem debugSignalToolStripMenuItem;
        private System.Windows.Forms.TextBox descriptionTextBox;
        private System.Windows.Forms.HScrollBar hScrollBar;
    }
}