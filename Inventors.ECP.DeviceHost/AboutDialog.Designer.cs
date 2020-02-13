namespace Inventors.ECP.DeviceHost
{
    partial class AboutDialog
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
            this.picture = new System.Windows.Forms.PictureBox();
            this.line1 = new System.Windows.Forms.Label();
            this.line2 = new System.Windows.Forms.Label();
            this.line3 = new System.Windows.Forms.Label();
            this.product = new System.Windows.Forms.Label();
            this.version = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.picture)).BeginInit();
            this.SuspendLayout();
            // 
            // picture
            // 
            this.picture.Location = new System.Drawing.Point(1, 0);
            this.picture.Name = "picture";
            this.picture.Size = new System.Drawing.Size(400, 180);
            this.picture.TabIndex = 0;
            this.picture.TabStop = false;
            // 
            // line1
            // 
            this.line1.AutoSize = true;
            this.line1.Location = new System.Drawing.Point(12, 185);
            this.line1.Name = "line1";
            this.line1.Size = new System.Drawing.Size(35, 13);
            this.line1.TabIndex = 1;
            this.line1.Text = "label1";
            // 
            // line2
            // 
            this.line2.AutoSize = true;
            this.line2.Location = new System.Drawing.Point(12, 198);
            this.line2.Name = "line2";
            this.line2.Size = new System.Drawing.Size(35, 13);
            this.line2.TabIndex = 2;
            this.line2.Text = "label1";
            // 
            // line3
            // 
            this.line3.AutoSize = true;
            this.line3.Location = new System.Drawing.Point(12, 211);
            this.line3.Name = "line3";
            this.line3.Size = new System.Drawing.Size(35, 13);
            this.line3.TabIndex = 3;
            this.line3.Text = "label1";
            // 
            // product
            // 
            this.product.AutoSize = true;
            this.product.BackColor = System.Drawing.Color.Transparent;
            this.product.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.product.Location = new System.Drawing.Point(15, 22);
            this.product.Name = "product";
            this.product.Size = new System.Drawing.Size(82, 24);
            this.product.TabIndex = 4;
            this.product.Text = "Product";
            // 
            // version
            // 
            this.version.AutoSize = true;
            this.version.BackColor = System.Drawing.Color.Transparent;
            this.version.Location = new System.Drawing.Point(16, 46);
            this.version.Name = "version";
            this.version.Size = new System.Drawing.Size(42, 13);
            this.version.TabIndex = 5;
            this.version.Text = "Version";
            // 
            // AboutDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(401, 241);
            this.Controls.Add(this.version);
            this.Controls.Add(this.product);
            this.Controls.Add(this.line3);
            this.Controls.Add(this.line2);
            this.Controls.Add(this.line1);
            this.Controls.Add(this.picture);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AboutDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "AboutDialog";
            ((System.ComponentModel.ISupportInitialize)(this.picture)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox picture;
        private System.Windows.Forms.Label line1;
        private System.Windows.Forms.Label line2;
        private System.Windows.Forms.Label line3;
        private System.Windows.Forms.Label product;
        private System.Windows.Forms.Label version;
    }
}