namespace ThreeDimensionAVI
{
    partial class LaunchForm
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
            this.picLaunchMsg = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.picLaunchMsg)).BeginInit();
            this.SuspendLayout();
            // 
            // picLaunchMsg
            // 
            this.picLaunchMsg.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picLaunchMsg.Location = new System.Drawing.Point(0, 0);
            this.picLaunchMsg.Name = "picLaunchMsg";
            this.picLaunchMsg.Size = new System.Drawing.Size(503, 252);
            this.picLaunchMsg.TabIndex = 0;
            this.picLaunchMsg.TabStop = false;
            // 
            // LaunchForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(505, 253);
            this.ControlBox = false;
            this.Controls.Add(this.picLaunchMsg);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LaunchForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "LaunchForm";
            this.Load += new System.EventHandler(this.LaunchForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.picLaunchMsg)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox picLaunchMsg;
    }
}