using Serilog.Events;

namespace Inventors.ECP.TestFramework
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            statusStrip = new System.Windows.Forms.StatusStrip();
            statusText = new System.Windows.Forms.ToolStripStatusLabel();
            pingStatus = new System.Windows.Forms.ToolStripStatusLabel();
            menuStrip = new System.Windows.Forms.MenuStrip();
            fileMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            clearLogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            saveLogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            pauseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            autoSaveLogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            logLevelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            verboseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            debugToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            informationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            warningToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            errorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            fatalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            connectMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            openDeviceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            connectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            disconnectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            portMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            addressMenu = new System.Windows.Forms.ToolStripMenuItem();
            actionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            profilerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            testToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            trialsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            profilerMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            portMonitorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            analysisToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            aboutECPTesterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            hSplitContainer = new System.Windows.Forms.SplitContainer();
            vSplitContainer = new System.Windows.Forms.SplitContainer();
            functionList = new System.Windows.Forms.ListBox();
            propertyGrid = new System.Windows.Forms.PropertyGrid();
            logControl = new LogControl();
            imageList = new System.Windows.Forms.ImageList(components);
            msgTimer = new System.Windows.Forms.Timer(components);
            deviceTimer = new System.Windows.Forms.Timer(components);
            statusStrip.SuspendLayout();
            menuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)hSplitContainer).BeginInit();
            hSplitContainer.Panel1.SuspendLayout();
            hSplitContainer.Panel2.SuspendLayout();
            hSplitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)vSplitContainer).BeginInit();
            vSplitContainer.Panel1.SuspendLayout();
            vSplitContainer.Panel2.SuspendLayout();
            vSplitContainer.SuspendLayout();
            SuspendLayout();
            // 
            // statusStrip
            // 
            statusStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { statusText, pingStatus });
            statusStrip.Location = new System.Drawing.Point(0, 567);
            statusStrip.Name = "statusStrip";
            statusStrip.Padding = new System.Windows.Forms.Padding(1, 0, 15, 0);
            statusStrip.Size = new System.Drawing.Size(927, 22);
            statusStrip.TabIndex = 0;
            statusStrip.Text = "statusStrip1";
            // 
            // statusText
            // 
            statusText.Name = "statusText";
            statusText.Size = new System.Drawing.Size(42, 17);
            statusText.Text = "Status:";
            // 
            // pingStatus
            // 
            pingStatus.Name = "pingStatus";
            pingStatus.Size = new System.Drawing.Size(74, 17);
            pingStatus.Text = "| Ping count:";
            // 
            // menuStrip
            // 
            menuStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { fileMenuItem, connectMenuItem, addressMenu, actionsToolStripMenuItem, profilerToolStripMenuItem, helpToolStripMenuItem });
            menuStrip.Location = new System.Drawing.Point(0, 0);
            menuStrip.Name = "menuStrip";
            menuStrip.Padding = new System.Windows.Forms.Padding(5, 2, 0, 2);
            menuStrip.Size = new System.Drawing.Size(927, 24);
            menuStrip.TabIndex = 1;
            menuStrip.Text = "menuStrip1";
            // 
            // fileMenuItem
            // 
            fileMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { clearLogToolStripMenuItem, saveLogToolStripMenuItem, pauseToolStripMenuItem, autoSaveLogToolStripMenuItem, toolStripSeparator7, logLevelToolStripMenuItem, toolStripSeparator1, exitToolStripMenuItem });
            fileMenuItem.Name = "fileMenuItem";
            fileMenuItem.Size = new System.Drawing.Size(37, 20);
            fileMenuItem.Text = "File";
            // 
            // clearLogToolStripMenuItem
            // 
            clearLogToolStripMenuItem.Name = "clearLogToolStripMenuItem";
            clearLogToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            clearLogToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            clearLogToolStripMenuItem.Text = "Clear Log";
            clearLogToolStripMenuItem.Click += ClearLogToolStripMenuItem_Click;
            // 
            // saveLogToolStripMenuItem
            // 
            saveLogToolStripMenuItem.Name = "saveLogToolStripMenuItem";
            saveLogToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S;
            saveLogToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            saveLogToolStripMenuItem.Text = "Save Log";
            saveLogToolStripMenuItem.Click += SaveLogToolStripMenuItem_Click;
            // 
            // pauseToolStripMenuItem
            // 
            pauseToolStripMenuItem.Name = "pauseToolStripMenuItem";
            pauseToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            pauseToolStripMenuItem.Text = "Pause";
            pauseToolStripMenuItem.Click += PauseToolStripMenuItem_Click;
            // 
            // autoSaveLogToolStripMenuItem
            // 
            autoSaveLogToolStripMenuItem.Name = "autoSaveLogToolStripMenuItem";
            autoSaveLogToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            autoSaveLogToolStripMenuItem.Text = "Auto Save Log";
            autoSaveLogToolStripMenuItem.Click += AutoSaveLogToolStripMenuItem_Click;
            // 
            // toolStripSeparator7
            // 
            toolStripSeparator7.Name = "toolStripSeparator7";
            toolStripSeparator7.Size = new System.Drawing.Size(158, 6);
            // 
            // logLevelToolStripMenuItem
            // 
            logLevelToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { verboseToolStripMenuItem, debugToolStripMenuItem, informationToolStripMenuItem, warningToolStripMenuItem, errorToolStripMenuItem, fatalToolStripMenuItem });
            logLevelToolStripMenuItem.Name = "logLevelToolStripMenuItem";
            logLevelToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            logLevelToolStripMenuItem.Text = "Log Level";
            // 
            // verboseToolStripMenuItem
            // 
            verboseToolStripMenuItem.Name = "verboseToolStripMenuItem";
            verboseToolStripMenuItem.Size = new System.Drawing.Size(137, 22);
            verboseToolStripMenuItem.Text = "Verbose";
            verboseToolStripMenuItem.Click += VerboseToolStripMenuItem_Click;
            // 
            // debugToolStripMenuItem
            // 
            debugToolStripMenuItem.Name = "debugToolStripMenuItem";
            debugToolStripMenuItem.Size = new System.Drawing.Size(137, 22);
            debugToolStripMenuItem.Text = "Debug";
            debugToolStripMenuItem.Click += DebugToolStripMenuItem_Click;
            // 
            // informationToolStripMenuItem
            // 
            informationToolStripMenuItem.Name = "informationToolStripMenuItem";
            informationToolStripMenuItem.Size = new System.Drawing.Size(137, 22);
            informationToolStripMenuItem.Text = "Information";
            informationToolStripMenuItem.Click += InformationToolStripMenuItem_Click;
            // 
            // warningToolStripMenuItem
            // 
            warningToolStripMenuItem.Name = "warningToolStripMenuItem";
            warningToolStripMenuItem.Size = new System.Drawing.Size(137, 22);
            warningToolStripMenuItem.Text = "Warning";
            warningToolStripMenuItem.Click += WarningToolStripMenuItem_Click;
            // 
            // errorToolStripMenuItem
            // 
            errorToolStripMenuItem.Name = "errorToolStripMenuItem";
            errorToolStripMenuItem.Size = new System.Drawing.Size(137, 22);
            errorToolStripMenuItem.Text = "Error";
            errorToolStripMenuItem.Click += ErrorToolStripMenuItem_Click;
            // 
            // fatalToolStripMenuItem
            // 
            fatalToolStripMenuItem.Name = "fatalToolStripMenuItem";
            fatalToolStripMenuItem.Size = new System.Drawing.Size(137, 22);
            fatalToolStripMenuItem.Text = "Fatal";
            fatalToolStripMenuItem.Click += FatalToolStripMenuItem_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new System.Drawing.Size(158, 6);
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            exitToolStripMenuItem.Text = "Exit";
            exitToolStripMenuItem.Click += ExitToolStripMenuItem_Click;
            // 
            // connectMenuItem
            // 
            connectMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { openDeviceToolStripMenuItem, toolStripSeparator3, connectToolStripMenuItem, disconnectToolStripMenuItem, toolStripSeparator2, portMenuItem });
            connectMenuItem.Name = "connectMenuItem";
            connectMenuItem.Size = new System.Drawing.Size(81, 20);
            connectMenuItem.Text = "Connection";
            // 
            // openDeviceToolStripMenuItem
            // 
            openDeviceToolStripMenuItem.Name = "openDeviceToolStripMenuItem";
            openDeviceToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.L;
            openDeviceToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            openDeviceToolStripMenuItem.Text = "Load Device";
            openDeviceToolStripMenuItem.Click += OpenDeviceToolStripMenuItem_Click;
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new System.Drawing.Size(175, 6);
            // 
            // connectToolStripMenuItem
            // 
            connectToolStripMenuItem.Name = "connectToolStripMenuItem";
            connectToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O;
            connectToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            connectToolStripMenuItem.Text = "Open";
            connectToolStripMenuItem.Click += ConnectToolStripMenuItem_Click;
            // 
            // disconnectToolStripMenuItem
            // 
            disconnectToolStripMenuItem.Name = "disconnectToolStripMenuItem";
            disconnectToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X;
            disconnectToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            disconnectToolStripMenuItem.Text = "Close";
            disconnectToolStripMenuItem.Click += DisconnectToolStripMenuItem_Click;
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new System.Drawing.Size(175, 6);
            // 
            // portMenuItem
            // 
            portMenuItem.Name = "portMenuItem";
            portMenuItem.Size = new System.Drawing.Size(178, 22);
            portMenuItem.Text = "Device";
            portMenuItem.DropDownItemClicked += PortMenuItem_DropDownItemClicked;
            // 
            // addressMenu
            // 
            addressMenu.Name = "addressMenu";
            addressMenu.Size = new System.Drawing.Size(61, 20);
            addressMenu.Text = "Address";
            // 
            // actionsToolStripMenuItem
            // 
            actionsToolStripMenuItem.Name = "actionsToolStripMenuItem";
            actionsToolStripMenuItem.Size = new System.Drawing.Size(59, 20);
            actionsToolStripMenuItem.Text = "Actions";
            // 
            // profilerToolStripMenuItem
            // 
            profilerToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { testToolStripMenuItem, trialsToolStripMenuItem, toolStripSeparator4, profilerMenuItem, portMonitorToolStripMenuItem, analysisToolStripMenuItem });
            profilerToolStripMenuItem.Name = "profilerToolStripMenuItem";
            profilerToolStripMenuItem.Size = new System.Drawing.Size(62, 20);
            profilerToolStripMenuItem.Text = "Analysis";
            // 
            // testToolStripMenuItem
            // 
            testToolStripMenuItem.Name = "testToolStripMenuItem";
            testToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.T;
            testToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            testToolStripMenuItem.Text = "Test";
            testToolStripMenuItem.Click += TestToolStripMenuItem_Click;
            // 
            // trialsToolStripMenuItem
            // 
            trialsToolStripMenuItem.Name = "trialsToolStripMenuItem";
            trialsToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            trialsToolStripMenuItem.Text = "Trials";
            trialsToolStripMenuItem.Click += TrialsToolStripMenuItem_Click;
            // 
            // toolStripSeparator4
            // 
            toolStripSeparator4.Name = "toolStripSeparator4";
            toolStripSeparator4.Size = new System.Drawing.Size(184, 6);
            // 
            // profilerMenuItem
            // 
            profilerMenuItem.Name = "profilerMenuItem";
            profilerMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P;
            profilerMenuItem.Size = new System.Drawing.Size(187, 22);
            profilerMenuItem.Text = "Profiler";
            profilerMenuItem.Click += ProfilerMenuItem_Click;
            // 
            // portMonitorToolStripMenuItem
            // 
            portMonitorToolStripMenuItem.Name = "portMonitorToolStripMenuItem";
            portMonitorToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.M;
            portMonitorToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            portMonitorToolStripMenuItem.Text = "Port Monitor";
            portMonitorToolStripMenuItem.Click += PortMonitorToolStripMenuItem_Click;
            // 
            // analysisToolStripMenuItem
            // 
            analysisToolStripMenuItem.Name = "analysisToolStripMenuItem";
            analysisToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A;
            analysisToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            analysisToolStripMenuItem.Text = "Analysis";
            analysisToolStripMenuItem.Click += analysisToolStripMenuItem_Click;
            // 
            // helpToolStripMenuItem
            // 
            helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { aboutECPTesterToolStripMenuItem });
            helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            helpToolStripMenuItem.Text = "Help";
            // 
            // aboutECPTesterToolStripMenuItem
            // 
            aboutECPTesterToolStripMenuItem.Name = "aboutECPTesterToolStripMenuItem";
            aboutECPTesterToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            aboutECPTesterToolStripMenuItem.Text = "About";
            aboutECPTesterToolStripMenuItem.Click += AboutECPTesterToolStripMenuItem_Click;
            // 
            // hSplitContainer
            // 
            hSplitContainer.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            hSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            hSplitContainer.Location = new System.Drawing.Point(0, 24);
            hSplitContainer.Name = "hSplitContainer";
            hSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // hSplitContainer.Panel1
            // 
            hSplitContainer.Panel1.Controls.Add(vSplitContainer);
            // 
            // hSplitContainer.Panel2
            // 
            hSplitContainer.Panel2.Controls.Add(logControl);
            hSplitContainer.Size = new System.Drawing.Size(927, 543);
            hSplitContainer.SplitterDistance = 386;
            hSplitContainer.TabIndex = 2;
            // 
            // vSplitContainer
            // 
            vSplitContainer.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            vSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            vSplitContainer.Location = new System.Drawing.Point(0, 0);
            vSplitContainer.Name = "vSplitContainer";
            // 
            // vSplitContainer.Panel1
            // 
            vSplitContainer.Panel1.Controls.Add(functionList);
            // 
            // vSplitContainer.Panel2
            // 
            vSplitContainer.Panel2.Controls.Add(propertyGrid);
            vSplitContainer.Size = new System.Drawing.Size(927, 386);
            vSplitContainer.SplitterDistance = 307;
            vSplitContainer.TabIndex = 0;
            // 
            // functionList
            // 
            functionList.Dock = System.Windows.Forms.DockStyle.Fill;
            functionList.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            functionList.FormattingEnabled = true;
            functionList.ItemHeight = 17;
            functionList.Location = new System.Drawing.Point(0, 0);
            functionList.Name = "functionList";
            functionList.Size = new System.Drawing.Size(303, 382);
            functionList.TabIndex = 0;
            functionList.SelectedIndexChanged += FunctionList_SelectedIndexChanged;
            functionList.DoubleClick += FunctionList_DoubleClick;
            // 
            // propertyGrid
            // 
            propertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            propertyGrid.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            propertyGrid.Location = new System.Drawing.Point(0, 0);
            propertyGrid.Name = "propertyGrid";
            propertyGrid.Size = new System.Drawing.Size(612, 382);
            propertyGrid.TabIndex = 0;
            // 
            // logControl
            // 
            logControl.AutoSave = true;
            logControl.Dock = System.Windows.Forms.DockStyle.Fill;
            logControl.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            logControl.Level = LogEventLevel.Information;
            logControl.Location = new System.Drawing.Point(0, 0);
            logControl.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            logControl.Name = "logControl";
            logControl.Paused = true;
            logControl.Size = new System.Drawing.Size(923, 149);
            logControl.TabIndex = 0;
            // 
            // imageList
            // 
            imageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            imageList.ImageStream = (System.Windows.Forms.ImageListStreamer)resources.GetObject("imageList.ImageStream");
            imageList.TransparentColor = System.Drawing.Color.Transparent;
            imageList.Images.SetKeyName(0, "B_0.png");
            imageList.Images.SetKeyName(1, "B_1.png");
            imageList.Images.SetKeyName(2, "B_2.png");
            imageList.Images.SetKeyName(3, "B_3.png");
            imageList.Images.SetKeyName(4, "B_4.png");
            imageList.Images.SetKeyName(5, "B_5.png");
            imageList.Images.SetKeyName(6, "B_6.png");
            imageList.Images.SetKeyName(7, "B_7.png");
            imageList.Images.SetKeyName(8, "B_8.png");
            imageList.Images.SetKeyName(9, "B_9.png");
            imageList.Images.SetKeyName(10, "B_C.png");
            imageList.Images.SetKeyName(11, "B_LEFT.png");
            imageList.Images.SetKeyName(12, "B_MA.png");
            imageList.Images.SetKeyName(13, "B_P.png");
            imageList.Images.SetKeyName(14, "B_RIGHT.png");
            imageList.Images.SetKeyName(15, "B_STAR.png");
            // 
            // msgTimer
            // 
            msgTimer.Enabled = true;
            msgTimer.Tick += MsgTimer_Tick;
            // 
            // deviceTimer
            // 
            deviceTimer.Enabled = true;
            deviceTimer.Interval = 500;
            deviceTimer.Tick += DeviceTimer_Tick;
            // 
            // MainWindow
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            ClientSize = new System.Drawing.Size(927, 589);
            Controls.Add(hSplitContainer);
            Controls.Add(statusStrip);
            Controls.Add(menuStrip);
            DoubleBuffered = true;
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = menuStrip;
            Name = "MainWindow";
            Text = "ECP Tester";
            statusStrip.ResumeLayout(false);
            statusStrip.PerformLayout();
            menuStrip.ResumeLayout(false);
            menuStrip.PerformLayout();
            hSplitContainer.Panel1.ResumeLayout(false);
            hSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)hSplitContainer).EndInit();
            hSplitContainer.ResumeLayout(false);
            vSplitContainer.Panel1.ResumeLayout(false);
            vSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)vSplitContainer).EndInit();
            vSplitContainer.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
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
        private System.Windows.Forms.ToolStripMenuItem aboutECPTesterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem profilerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem testToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem trialsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.Timer deviceTimer;
        private System.Windows.Forms.ToolStripMenuItem logLevelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem debugToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem informationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem errorToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel pingStatus;
        private LogControl logControl;
        private System.Windows.Forms.ToolStripMenuItem profilerMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.ToolStripMenuItem addressMenu;
        private System.Windows.Forms.ToolStripMenuItem pauseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem autoSaveLogToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem portMonitorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem analysisToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem actionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem verboseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem warningToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fatalToolStripMenuItem;
    }
}

