namespace ThreeDimensionAVI
{
    partial class RecheckOrChangeBarcodeForm
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
            this.lblBarcodeDataTitle = new System.Windows.Forms.Label();
            this.txtBarcodeData = new System.Windows.Forms.TextBox();
            this.btnEnsure = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblBarcodeDataTitle
            // 
            this.lblBarcodeDataTitle.Location = new System.Drawing.Point(13, 24);
            this.lblBarcodeDataTitle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblBarcodeDataTitle.Name = "lblBarcodeDataTitle";
            this.lblBarcodeDataTitle.Size = new System.Drawing.Size(99, 20);
            this.lblBarcodeDataTitle.TabIndex = 0;
            this.lblBarcodeDataTitle.Text = "条形码数据:";
            this.lblBarcodeDataTitle.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtBarcodeData
            // 
            this.txtBarcodeData.Location = new System.Drawing.Point(119, 22);
            this.txtBarcodeData.Name = "txtBarcodeData";
            this.txtBarcodeData.Size = new System.Drawing.Size(271, 26);
            this.txtBarcodeData.TabIndex = 1;
            this.txtBarcodeData.DoubleClick += new System.EventHandler(this.TextBoxDoubleClickEvent);
            // 
            // btnEnsure
            // 
            this.btnEnsure.Location = new System.Drawing.Point(216, 62);
            this.btnEnsure.Name = "btnEnsure";
            this.btnEnsure.Size = new System.Drawing.Size(75, 32);
            this.btnEnsure.TabIndex = 2;
            this.btnEnsure.Text = "确定";
            this.btnEnsure.UseVisualStyleBackColor = true;
            this.btnEnsure.Click += new System.EventHandler(this.btnEnsure_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(299, 62);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 32);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // RecheckOrChangeBarcodeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(406, 100);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnEnsure);
            this.Controls.Add(this.txtBarcodeData);
            this.Controls.Add(this.lblBarcodeDataTitle);
            this.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RecheckOrChangeBarcodeForm";
            this.Text = "条形码数据修改或二次检测";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.RecheckOrChangeBarcodeForm_FormClosing);
            this.Load += new System.EventHandler(this.RecheckOrChangeBarcodeForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblBarcodeDataTitle;
        private System.Windows.Forms.TextBox txtBarcodeData;
        private System.Windows.Forms.Button btnEnsure;
        private System.Windows.Forms.Button btnCancel;
    }
}