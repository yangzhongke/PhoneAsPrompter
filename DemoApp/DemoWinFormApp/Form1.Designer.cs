
namespace DemoWinFormApp
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.labelUrl = new System.Windows.Forms.LinkLabel();
            this.labelMsg = new System.Windows.Forms.Label();
            this.imgQRCode = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.imgQRCode)).BeginInit();
            this.SuspendLayout();
            // 
            // labelUrl
            // 
            this.labelUrl.AutoSize = true;
            this.labelUrl.Location = new System.Drawing.Point(12, 9);
            this.labelUrl.Name = "labelUrl";
            this.labelUrl.Size = new System.Drawing.Size(31, 17);
            this.labelUrl.TabIndex = 0;
            this.labelUrl.TabStop = true;
            this.labelUrl.Text = "URL";
            // 
            // labelMsg
            // 
            this.labelMsg.AutoSize = true;
            this.labelMsg.Location = new System.Drawing.Point(12, 64);
            this.labelMsg.Name = "labelMsg";
            this.labelMsg.Size = new System.Drawing.Size(38, 17);
            this.labelMsg.TabIndex = 1;
            this.labelMsg.Text = "value";
            // 
            // imgQRCode
            // 
            this.imgQRCode.Location = new System.Drawing.Point(167, 13);
            this.imgQRCode.Name = "imgQRCode";
            this.imgQRCode.Size = new System.Drawing.Size(222, 192);
            this.imgQRCode.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.imgQRCode.TabIndex = 2;
            this.imgQRCode.TabStop = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.imgQRCode);
            this.Controls.Add(this.labelMsg);
            this.Controls.Add(this.labelUrl);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.imgQRCode)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.LinkLabel labelUrl;
        private System.Windows.Forms.Label labelMsg;
        private System.Windows.Forms.PictureBox imgQRCode;
    }
}

