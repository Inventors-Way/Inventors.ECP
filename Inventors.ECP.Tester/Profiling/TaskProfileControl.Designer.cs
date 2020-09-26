namespace Inventors.ECP.Tester.Profiling
{
    partial class TaskProfileControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TaskProfileControl));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.activeTask = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.eventsOnMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.eventsOffMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripDropDownButton2 = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.violationsOnMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.violationsOffMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.activeTask,
            this.toolStripSeparator1,
            this.toolStripDropDownButton1,
            this.toolStripSeparator2,
            this.toolStripDropDownButton2});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(649, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(32, 22);
            this.toolStripLabel1.Text = "Task:";
            // 
            // activeTask
            // 
            this.activeTask.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.activeTask.Name = "activeTask";
            this.activeTask.Size = new System.Drawing.Size(121, 25);
            this.activeTask.SelectedIndexChanged += new System.EventHandler(this.ActiveTask_SelectedIndexChanged);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // pictureBox
            // 
            this.pictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox.Location = new System.Drawing.Point(0, 25);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(649, 486);
            this.pictureBox.TabIndex = 1;
            this.pictureBox.TabStop = false;
            // 
            // toolStripDropDownButton1
            // 
            this.toolStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.eventsOnMenuItem,
            this.eventsOffMenuItem});
            this.toolStripDropDownButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton1.Image")));
            this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.Size = new System.Drawing.Size(54, 22);
            this.toolStripDropDownButton1.Text = "Events";
            // 
            // eventsOnMenuItem
            // 
            this.eventsOnMenuItem.Checked = true;
            this.eventsOnMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.eventsOnMenuItem.Name = "eventsOnMenuItem";
            this.eventsOnMenuItem.Size = new System.Drawing.Size(180, 22);
            this.eventsOnMenuItem.Text = "On";
            this.eventsOnMenuItem.Click += new System.EventHandler(this.EventsOnMenuItem_Click);
            // 
            // eventsOffMenuItem
            // 
            this.eventsOffMenuItem.Name = "eventsOffMenuItem";
            this.eventsOffMenuItem.Size = new System.Drawing.Size(180, 22);
            this.eventsOffMenuItem.Text = "Off";
            this.eventsOffMenuItem.Click += new System.EventHandler(this.EventsOffMenuItem_Click);
            // 
            // toolStripDropDownButton2
            // 
            this.toolStripDropDownButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownButton2.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.violationsOnMenuItem,
            this.violationsOffMenuItem});
            this.toolStripDropDownButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton2.Image")));
            this.toolStripDropDownButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton2.Name = "toolStripDropDownButton2";
            this.toolStripDropDownButton2.Size = new System.Drawing.Size(112, 22);
            this.toolStripDropDownButton2.Text = "Timing Violations";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // violationsOnMenuItem
            // 
            this.violationsOnMenuItem.Checked = true;
            this.violationsOnMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.violationsOnMenuItem.Name = "violationsOnMenuItem";
            this.violationsOnMenuItem.Size = new System.Drawing.Size(180, 22);
            this.violationsOnMenuItem.Text = "On";
            this.violationsOnMenuItem.Click += new System.EventHandler(this.ViolationsOnMenuItem_Click);
            // 
            // violationsOffMenuItem
            // 
            this.violationsOffMenuItem.Name = "violationsOffMenuItem";
            this.violationsOffMenuItem.Size = new System.Drawing.Size(180, 22);
            this.violationsOffMenuItem.Text = "Off";
            this.violationsOffMenuItem.Click += new System.EventHandler(this.ViolationsOffMenuItem_Click);
            // 
            // TaskProfileControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pictureBox);
            this.Controls.Add(this.toolStrip1);
            this.Name = "TaskProfileControl";
            this.Size = new System.Drawing.Size(649, 511);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripComboBox activeTask;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
        private System.Windows.Forms.ToolStripMenuItem eventsOnMenuItem;
        private System.Windows.Forms.ToolStripMenuItem eventsOffMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton2;
        private System.Windows.Forms.ToolStripMenuItem violationsOnMenuItem;
        private System.Windows.Forms.ToolStripMenuItem violationsOffMenuItem;
    }
}
