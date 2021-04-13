namespace Inventors.ECP.Tester
{
    partial class MainWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.statusText = new System.Windows.Forms.ToolStripStatusLabel();
            this.pingStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearLogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveLogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pauseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.autoSaveLogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.runScriptToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.logLevelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.debugToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.errorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.connectMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openDeviceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.connectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.disconnectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.portMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addressMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.profilerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.testToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.trialsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.profilerMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.documentationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.aboutECPTesterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hSplitContainer = new System.Windows.Forms.SplitContainer();
            this.vSplitContainer = new System.Windows.Forms.SplitContainer();
            this.functionList = new System.Windows.Forms.ListBox();
            this.propertyGrid = new System.Windows.Forms.PropertyGrid();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.msgTimer = new System.Windows.Forms.Timer(this.components);
            this.deviceTimer = new System.Windows.Forms.Timer(this.components);
            this.logControl = new Inventors.ECP.Tester.LogControl();
            this.openLogsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip.SuspendLayout();
            this.menuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.hSplitContainer)).BeginInit();
            this.hSplitContainer.Panel1.SuspendLayout();
            this.hSplitContainer.Panel2.SuspendLayout();
            this.hSplitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.vSplitContainer)).BeginInit();
            this.vSplitContainer.Panel1.SuspendLayout();
            this.vSplitContainer.Panel2.SuspendLayout();
            this.vSplitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip
            // 
            this.statusStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusText,
            this.pingStatus});
            this.statusStrip.Location = new System.Drawing.Point(0, 710);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Padding = new System.Windows.Forms.Padding(1, 0, 19, 0);
            this.statusStrip.Size = new System.Drawing.Size(1159, 26);
            this.statusStrip.TabIndex = 0;
            this.statusStrip.Text = "statusStrip1";
            // 
            // statusText
            // 
            this.statusText.Name = "statusText";
            this.statusText.Size = new System.Drawing.Size(52, 20);
            this.statusText.Text = "Status:";
            // 
            // pingStatus
            // 
            this.pingStatus.Name = "pingStatus";
            this.pingStatus.Size = new System.Drawing.Size(90, 20);
            this.pingStatus.Text = "| Ping count:";
            // 
            // menuStrip
            // 
            this.menuStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileMenuItem,
            this.connectMenuItem,
            this.addressMenu,
            this.profilerToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(1159, 28);
            this.menuStrip.TabIndex = 1;
            this.menuStrip.Text = "menuStrip1";
            // 
            // fileMenuItem
            // 
            this.fileMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.clearLogToolStripMenuItem,
            this.saveLogToolStripMenuItem,
            this.pauseToolStripMenuItem,
            this.autoSaveLogToolStripMenuItem,
            this.openLogsToolStripMenuItem,
            this.toolStripSeparator7,
            this.runScriptToolStripMenuItem,
            this.toolStripSeparator6,
            this.logLevelToolStripMenuItem,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
            this.fileMenuItem.Name = "fileMenuItem";
            this.fileMenuItem.Size = new System.Drawing.Size(46, 24);
            this.fileMenuItem.Text = "File";
            // 
            // clearLogToolStripMenuItem
            // 
            this.clearLogToolStripMenuItem.Name = "clearLogToolStripMenuItem";
            this.clearLogToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.clearLogToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            this.clearLogToolStripMenuItem.Text = "Clear Log";
            this.clearLogToolStripMenuItem.Click += new System.EventHandler(this.ClearLogToolStripMenuItem_Click);
            // 
            // saveLogToolStripMenuItem
            // 
            this.saveLogToolStripMenuItem.Image = global::Inventors.ECP.Tester.Properties.Resources._216190___export;
            this.saveLogToolStripMenuItem.Name = "saveLogToolStripMenuItem";
            this.saveLogToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveLogToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            this.saveLogToolStripMenuItem.Text = "Save Log";
            this.saveLogToolStripMenuItem.Click += new System.EventHandler(this.SaveLogToolStripMenuItem_Click);
            // 
            // pauseToolStripMenuItem
            // 
            this.pauseToolStripMenuItem.Name = "pauseToolStripMenuItem";
            this.pauseToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
            this.pauseToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            this.pauseToolStripMenuItem.Text = "Pause";
            this.pauseToolStripMenuItem.Click += new System.EventHandler(this.PauseToolStripMenuItem_Click);
            // 
            // autoSaveLogToolStripMenuItem
            // 
            this.autoSaveLogToolStripMenuItem.Name = "autoSaveLogToolStripMenuItem";
            this.autoSaveLogToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            this.autoSaveLogToolStripMenuItem.Text = "Auto Save Log";
            this.autoSaveLogToolStripMenuItem.Click += new System.EventHandler(this.AutoSaveLogToolStripMenuItem_Click);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(221, 6);
            // 
            // runScriptToolStripMenuItem
            // 
            this.runScriptToolStripMenuItem.Name = "runScriptToolStripMenuItem";
            this.runScriptToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            this.runScriptToolStripMenuItem.Text = "Run Script";
            this.runScriptToolStripMenuItem.Click += new System.EventHandler(this.RunScriptToolStripMenuItem_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(221, 6);
            // 
            // logLevelToolStripMenuItem
            // 
            this.logLevelToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.debugToolStripMenuItem,
            this.statusToolStripMenuItem,
            this.errorToolStripMenuItem});
            this.logLevelToolStripMenuItem.Name = "logLevelToolStripMenuItem";
            this.logLevelToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            this.logLevelToolStripMenuItem.Text = "Log Level";
            // 
            // debugToolStripMenuItem
            // 
            this.debugToolStripMenuItem.Name = "debugToolStripMenuItem";
            this.debugToolStripMenuItem.Size = new System.Drawing.Size(137, 26);
            this.debugToolStripMenuItem.Text = "Debug";
            this.debugToolStripMenuItem.Click += new System.EventHandler(this.DebugToolStripMenuItem_Click);
            // 
            // statusToolStripMenuItem
            // 
            this.statusToolStripMenuItem.Name = "statusToolStripMenuItem";
            this.statusToolStripMenuItem.Size = new System.Drawing.Size(137, 26);
            this.statusToolStripMenuItem.Text = "Status";
            this.statusToolStripMenuItem.Click += new System.EventHandler(this.StatusToolStripMenuItem_Click);
            // 
            // errorToolStripMenuItem
            // 
            this.errorToolStripMenuItem.Name = "errorToolStripMenuItem";
            this.errorToolStripMenuItem.Size = new System.Drawing.Size(137, 26);
            this.errorToolStripMenuItem.Text = "Error";
            this.errorToolStripMenuItem.Click += new System.EventHandler(this.ErrorToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(221, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.ExitToolStripMenuItem_Click);
            // 
            // connectMenuItem
            // 
            this.connectMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openDeviceToolStripMenuItem,
            this.toolStripSeparator3,
            this.connectToolStripMenuItem,
            this.disconnectToolStripMenuItem,
            this.toolStripSeparator2,
            this.portMenuItem});
            this.connectMenuItem.Name = "connectMenuItem";
            this.connectMenuItem.Size = new System.Drawing.Size(98, 24);
            this.connectMenuItem.Text = "Connection";
            // 
            // openDeviceToolStripMenuItem
            // 
            this.openDeviceToolStripMenuItem.Image = global::Inventors.ECP.Tester.Properties.Resources._216173___device_phone;
            this.openDeviceToolStripMenuItem.Name = "openDeviceToolStripMenuItem";
            this.openDeviceToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.L)));
            this.openDeviceToolStripMenuItem.Size = new System.Drawing.Size(223, 26);
            this.openDeviceToolStripMenuItem.Text = "Load Device";
            this.openDeviceToolStripMenuItem.Click += new System.EventHandler(this.OpenDeviceToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(220, 6);
            // 
            // connectToolStripMenuItem
            // 
            this.connectToolStripMenuItem.Name = "connectToolStripMenuItem";
            this.connectToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.connectToolStripMenuItem.Size = new System.Drawing.Size(223, 26);
            this.connectToolStripMenuItem.Text = "Open";
            this.connectToolStripMenuItem.Click += new System.EventHandler(this.ConnectToolStripMenuItem_Click);
            // 
            // disconnectToolStripMenuItem
            // 
            this.disconnectToolStripMenuItem.Name = "disconnectToolStripMenuItem";
            this.disconnectToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this.disconnectToolStripMenuItem.Size = new System.Drawing.Size(223, 26);
            this.disconnectToolStripMenuItem.Text = "Close";
            this.disconnectToolStripMenuItem.Click += new System.EventHandler(this.DisconnectToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(220, 6);
            // 
            // portMenuItem
            // 
            this.portMenuItem.Name = "portMenuItem";
            this.portMenuItem.Size = new System.Drawing.Size(223, 26);
            this.portMenuItem.Text = "Device";
            this.portMenuItem.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.PortMenuItem_DropDownItemClicked);
            // 
            // addressMenu
            // 
            this.addressMenu.Name = "addressMenu";
            this.addressMenu.Size = new System.Drawing.Size(76, 24);
            this.addressMenu.Text = "Address";
            // 
            // profilerToolStripMenuItem
            // 
            this.profilerToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.testToolStripMenuItem,
            this.trialsToolStripMenuItem,
            this.toolStripSeparator4,
            this.profilerMenuItem});
            this.profilerToolStripMenuItem.Name = "profilerToolStripMenuItem";
            this.profilerToolStripMenuItem.Size = new System.Drawing.Size(71, 24);
            this.profilerToolStripMenuItem.Text = "Profiler";
            // 
            // testToolStripMenuItem
            // 
            this.testToolStripMenuItem.Name = "testToolStripMenuItem";
            this.testToolStripMenuItem.Size = new System.Drawing.Size(140, 26);
            this.testToolStripMenuItem.Text = "Test";
            this.testToolStripMenuItem.Click += new System.EventHandler(this.TestToolStripMenuItem_Click);
            // 
            // trialsToolStripMenuItem
            // 
            this.trialsToolStripMenuItem.Name = "trialsToolStripMenuItem";
            this.trialsToolStripMenuItem.Size = new System.Drawing.Size(140, 26);
            this.trialsToolStripMenuItem.Text = "Trials";
            this.trialsToolStripMenuItem.Click += new System.EventHandler(this.TrialsToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(137, 6);
            // 
            // profilerMenuItem
            // 
            this.profilerMenuItem.Name = "profilerMenuItem";
            this.profilerMenuItem.Size = new System.Drawing.Size(140, 26);
            this.profilerMenuItem.Text = "Profiler";
            this.profilerMenuItem.Click += new System.EventHandler(this.ProfilerMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.documentationToolStripMenuItem,
            this.toolStripSeparator5,
            this.aboutECPTesterToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(55, 24);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // documentationToolStripMenuItem
            // 
            this.documentationToolStripMenuItem.Name = "documentationToolStripMenuItem";
            this.documentationToolStripMenuItem.Size = new System.Drawing.Size(195, 26);
            this.documentationToolStripMenuItem.Text = "Documentation";
            this.documentationToolStripMenuItem.Click += new System.EventHandler(this.DocumentationToolStripMenuItem_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(192, 6);
            // 
            // aboutECPTesterToolStripMenuItem
            // 
            this.aboutECPTesterToolStripMenuItem.Name = "aboutECPTesterToolStripMenuItem";
            this.aboutECPTesterToolStripMenuItem.Size = new System.Drawing.Size(195, 26);
            this.aboutECPTesterToolStripMenuItem.Text = "About";
            this.aboutECPTesterToolStripMenuItem.Click += new System.EventHandler(this.AboutECPTesterToolStripMenuItem_Click);
            // 
            // hSplitContainer
            // 
            this.hSplitContainer.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.hSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.hSplitContainer.Location = new System.Drawing.Point(0, 28);
            this.hSplitContainer.Margin = new System.Windows.Forms.Padding(4);
            this.hSplitContainer.Name = "hSplitContainer";
            this.hSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // hSplitContainer.Panel1
            // 
            this.hSplitContainer.Panel1.Controls.Add(this.vSplitContainer);
            // 
            // hSplitContainer.Panel2
            // 
            this.hSplitContainer.Panel2.Controls.Add(this.logControl);
            this.hSplitContainer.Size = new System.Drawing.Size(1159, 682);
            this.hSplitContainer.SplitterDistance = 473;
            this.hSplitContainer.SplitterWidth = 5;
            this.hSplitContainer.TabIndex = 2;
            // 
            // vSplitContainer
            // 
            this.vSplitContainer.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.vSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.vSplitContainer.Location = new System.Drawing.Point(0, 0);
            this.vSplitContainer.Margin = new System.Windows.Forms.Padding(4);
            this.vSplitContainer.Name = "vSplitContainer";
            // 
            // vSplitContainer.Panel1
            // 
            this.vSplitContainer.Panel1.Controls.Add(this.functionList);
            // 
            // vSplitContainer.Panel2
            // 
            this.vSplitContainer.Panel2.Controls.Add(this.propertyGrid);
            this.vSplitContainer.Size = new System.Drawing.Size(1159, 473);
            this.vSplitContainer.SplitterDistance = 221;
            this.vSplitContainer.SplitterWidth = 5;
            this.vSplitContainer.TabIndex = 0;
            // 
            // functionList
            // 
            this.functionList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.functionList.FormattingEnabled = true;
            this.functionList.ItemHeight = 16;
            this.functionList.Location = new System.Drawing.Point(0, 0);
            this.functionList.Margin = new System.Windows.Forms.Padding(4);
            this.functionList.Name = "functionList";
            this.functionList.Size = new System.Drawing.Size(217, 469);
            this.functionList.TabIndex = 0;
            this.functionList.SelectedIndexChanged += new System.EventHandler(this.FunctionList_SelectedIndexChanged);
            this.functionList.DoubleClick += new System.EventHandler(this.FunctionList_DoubleClick);
            // 
            // propertyGrid
            // 
            this.propertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid.Location = new System.Drawing.Point(0, 0);
            this.propertyGrid.Margin = new System.Windows.Forms.Padding(4);
            this.propertyGrid.Name = "propertyGrid";
            this.propertyGrid.Size = new System.Drawing.Size(929, 469);
            this.propertyGrid.TabIndex = 0;
            // 
            // imageList
            // 
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList.Images.SetKeyName(0, "B_0.png");
            this.imageList.Images.SetKeyName(1, "B_1.png");
            this.imageList.Images.SetKeyName(2, "B_2.png");
            this.imageList.Images.SetKeyName(3, "B_3.png");
            this.imageList.Images.SetKeyName(4, "B_4.png");
            this.imageList.Images.SetKeyName(5, "B_5.png");
            this.imageList.Images.SetKeyName(6, "B_6.png");
            this.imageList.Images.SetKeyName(7, "B_7.png");
            this.imageList.Images.SetKeyName(8, "B_8.png");
            this.imageList.Images.SetKeyName(9, "B_9.png");
            this.imageList.Images.SetKeyName(10, "B_C.png");
            this.imageList.Images.SetKeyName(11, "B_LEFT.png");
            this.imageList.Images.SetKeyName(12, "B_MA.png");
            this.imageList.Images.SetKeyName(13, "B_P.png");
            this.imageList.Images.SetKeyName(14, "B_RIGHT.png");
            this.imageList.Images.SetKeyName(15, "B_STAR.png");
            // 
            // msgTimer
            // 
            this.msgTimer.Enabled = true;
            this.msgTimer.Tick += new System.EventHandler(this.MsgTimer_Tick);
            // 
            // deviceTimer
            // 
            this.deviceTimer.Enabled = true;
            this.deviceTimer.Interval = 1000;
            this.deviceTimer.Tick += new System.EventHandler(this.DeviceTimer_Tick);
            // 
            // logControl
            // 
            this.logControl.AutoSave = true;
            this.logControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logControl.Level = Inventors.ECP.LogLevel.STATUS;
            this.logControl.Location = new System.Drawing.Point(0, 0);
            this.logControl.Margin = new System.Windows.Forms.Padding(5);
            this.logControl.Name = "logControl";
            this.logControl.Paused = true;
            this.logControl.Size = new System.Drawing.Size(1155, 200);
            this.logControl.TabIndex = 0;
            // 
            // openLogsToolStripMenuItem
            // 
            this.openLogsToolStripMenuItem.Name = "openLogsToolStripMenuItem";
            this.openLogsToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            this.openLogsToolStripMenuItem.Text = "Open Logs";
            this.openLogsToolStripMenuItem.Click += new System.EventHandler(this.OpenLogsToolStripMenuItem_Click);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(1159, 736);
            this.Controls.Add(this.hSplitContainer);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.menuStrip);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "MainWindow";
            this.Text = "ECP Tester";
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.hSplitContainer.Panel1.ResumeLayout(false);
            this.hSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.hSplitContainer)).EndInit();
            this.hSplitContainer.ResumeLayout(false);
            this.vSplitContainer.Panel1.ResumeLayout(false);
            this.vSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.vSplitContainer)).EndInit();
            this.vSplitContainer.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveLogToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem connectMenuItem;
        private System.Windows.Forms.ToolStripMenuItem connectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem disconnectToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.SplitContainer hSplitContainer;
        private System.Windows.Forms.SplitContainer vSplitContainer;
        private System.Windows.Forms.ListBox functionList;
        private System.Windows.Forms.Timer msgTimer;
        private System.Windows.Forms.ToolStripMenuItem portMenuItem;
        private System.Windows.Forms.ImageList imageList;
        private System.Windows.Forms.PropertyGrid propertyGrid;
        private System.Windows.Forms.ToolStripStatusLabel statusText;
        private System.Windows.Forms.ToolStripMenuItem clearLogToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openDeviceToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem documentationToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem aboutECPTesterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem profilerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem testToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem trialsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.Timer deviceTimer;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripMenuItem logLevelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem debugToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem statusToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem errorToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel pingStatus;
        private LogControl logControl;
        private System.Windows.Forms.ToolStripMenuItem profilerMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.ToolStripMenuItem runScriptToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addressMenu;
        private System.Windows.Forms.ToolStripMenuItem pauseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem autoSaveLogToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openLogsToolStripMenuItem;
    }
}

