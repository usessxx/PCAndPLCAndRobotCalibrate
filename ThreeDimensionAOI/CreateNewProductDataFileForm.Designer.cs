namespace ThreeDimensionAVI
{
    partial class CreateNewProductDataFileForm
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
            this.lblInfoMsg = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.txtNewProductName = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // lblInfoMsg
            // 
            this.lblInfoMsg.AutoSize = true;
            this.lblInfoMsg.Location = new System.Drawing.Point(53, 30);
            this.lblInfoMsg.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblInfoMsg.Name = "lblInfoMsg";
            this.lblInfoMsg.Size = new System.Drawing.Size(73, 19);
            this.lblInfoMsg.TabIndex = 0;
            this.lblInfoMsg.Text = "产品名？";
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(399, 23);
            this.btnOK.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(112, 37);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "确认";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.OK_BT_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(399, 68);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(112, 37);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.Cancel_BT_Click);
            // 
            // txtNewProductName
            // 
            this.txtNewProductName.Location = new System.Drawing.Point(24, 129);
            this.txtNewProductName.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtNewProductName.Name = "txtNewProductName";
            this.txtNewProductName.Size = new System.Drawing.Size(507, 26);
            this.txtNewProductName.TabIndex = 2;
            this.txtNewProductName.DoubleClick += new System.EventHandler(this.TextBoxDoubleClickEventToOpenNumberKeyboard);
            // 
            // CreateNewProductDataFileForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(546, 184);
            this.Controls.Add(this.txtNewProductName);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.lblInfoMsg);
            this.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CreateNewProductDataFileForm";
            this.Text = "创建新产品";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Label lblInfoMsg;
        public System.Windows.Forms.Button btnOK;
        public System.Windows.Forms.Button btnCancel;
        public System.Windows.Forms.TextBox txtNewProductName;
    }
}