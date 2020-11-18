namespace TS2TOOLS
{
    partial class Method
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Method));
            this.DcBtn = new System.Windows.Forms.Button();
            this.EcBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // DcBtn
            // 
            this.DcBtn.Location = new System.Drawing.Point(12, 12);
            this.DcBtn.Name = "DcBtn";
            this.DcBtn.Size = new System.Drawing.Size(155, 27);
            this.DcBtn.TabIndex = 0;
            this.DcBtn.Text = "DECRYPT";
            this.DcBtn.UseVisualStyleBackColor = true;
            // 
            // EcBtn
            // 
            this.EcBtn.Location = new System.Drawing.Point(12, 45);
            this.EcBtn.Name = "EcBtn";
            this.EcBtn.Size = new System.Drawing.Size(155, 27);
            this.EcBtn.TabIndex = 1;
            this.EcBtn.Text = "ENCRYPT";
            this.EcBtn.UseVisualStyleBackColor = true;
            // 
            // Method
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(180, 90);
            this.Controls.Add(this.EcBtn);
            this.Controls.Add(this.DcBtn);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Method";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Method";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button DcBtn;
        private System.Windows.Forms.Button EcBtn;
    }
}