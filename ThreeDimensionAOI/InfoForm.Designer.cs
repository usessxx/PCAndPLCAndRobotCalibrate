namespace ThreeDimensionAVI
{
    partial class InfoForm
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
            this.txtInfoMessageDisp = new System.Windows.Forms.TextBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnOut = new System.Windows.Forms.Button();
            this.btnReleaseAndTakeOut = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtInfoMessageDisp
            // 
            this.txtInfoMessageDisp.Location = new System.Drawing.Point(35, 13);
            this.txtInfoMessageDisp.Multiline = true;
            this.txtInfoMessageDisp.Name = "txtInfoMessageDisp";
            this.txtInfoMessageDisp.ReadOnly = true;
            this.txtInfoMessageDisp.Size = new System.Drawing.Size(435, 176);
            this.txtInfoMessageDisp.TabIndex = 0;
            // 
            // btnOK
            // 
            this.btnOK.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOK.Location = new System.Drawing.Point(41, 206);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(106, 43);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "确定";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnOut
            // 
            this.btnOut.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOut.Location = new System.Drawing.Point(355, 206);
            this.btnOut.Name = "btnOut";
            this.btnOut.Size = new System.Drawing.Size(106, 43);
            this.btnOut.TabIndex = 1;
            this.btnOut.Text = "流出";
            this.btnOut.UseVisualStyleBackColor = true;
            this.btnOut.Click += new System.EventHandler(this.btnOut_Click);
            // 
            // btnReleaseAndTakeOut
            // 
            this.btnReleaseAndTakeOut.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnReleaseAndTakeOut.Location = new System.Drawing.Point(198, 206);
            this.btnReleaseAndTakeOut.Name = "btnReleaseAndTakeOut";
            this.btnReleaseAndTakeOut.Size = new System.Drawing.Size(106, 43);
            this.btnReleaseAndTakeOut.TabIndex = 1;
            this.btnReleaseAndTakeOut.Text = "人工取出";
            this.btnReleaseAndTakeOut.UseVisualStyleBackColor = true;
            this.btnReleaseAndTakeOut.Click += new System.EventHandler(this.btnReleaseAndTakeOut_Click);
            // 
            // InfoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(509, 261);
            this.Controls.Add(this.btnReleaseAndTakeOut);
            this.Controls.Add(this.btnOut);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.txtInfoMessageDisp);
            this.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "InfoForm";
            this.Text = "InfoForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.InfoForm_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.TextBox txtInfoMessageDisp;
        public System.Windows.Forms.Button btnOK;
        public System.Windows.Forms.Button btnOut;
        public System.Windows.Forms.Button btnReleaseAndTakeOut;
    }
}