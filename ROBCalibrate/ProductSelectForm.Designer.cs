namespace ROBCalibrate
{
    partial class ProductSelectForm
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
            this.cboAvaliableProduct = new System.Windows.Forms.ComboBox();
            this.lblUsefulProductTitle = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnConfirm = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // cboAvaliableProduct
            // 
            this.cboAvaliableProduct.FormattingEnabled = true;
            this.cboAvaliableProduct.Location = new System.Drawing.Point(182, 24);
            this.cboAvaliableProduct.Margin = new System.Windows.Forms.Padding(4);
            this.cboAvaliableProduct.Name = "cboAvaliableProduct";
            this.cboAvaliableProduct.Size = new System.Drawing.Size(245, 29);
            this.cboAvaliableProduct.TabIndex = 0;
            this.cboAvaliableProduct.SelectedIndexChanged += new System.EventHandler(this.Avaliable_Product_ComboBox_SelectedIndexChanged);
            this.cboAvaliableProduct.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Avaliable_Product_ComboBox_MouseClick);
            // 
            // lblUsefulProductTitle
            // 
            this.lblUsefulProductTitle.AutoSize = true;
            this.lblUsefulProductTitle.Location = new System.Drawing.Point(13, 27);
            this.lblUsefulProductTitle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblUsefulProductTitle.Name = "lblUsefulProductTitle";
            this.lblUsefulProductTitle.Size = new System.Drawing.Size(110, 21);
            this.lblUsefulProductTitle.TabIndex = 1;
            this.lblUsefulProductTitle.Text = "可用产品名";
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.Yellow;
            this.btnCancel.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancel.Location = new System.Drawing.Point(310, 79);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(94, 39);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.Cancel_Button_Click);
            // 
            // btnConfirm
            // 
            this.btnConfirm.BackColor = System.Drawing.Color.LimeGreen;
            this.btnConfirm.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnConfirm.Location = new System.Drawing.Point(78, 79);
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.Size = new System.Drawing.Size(94, 39);
            this.btnConfirm.TabIndex = 2;
            this.btnConfirm.Text = "确认";
            this.btnConfirm.UseVisualStyleBackColor = false;
            this.btnConfirm.Click += new System.EventHandler(this.Comfirm_Button_Click);
            // 
            // ProductSelectForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(496, 134);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnConfirm);
            this.Controls.Add(this.lblUsefulProductTitle);
            this.Controls.Add(this.cboAvaliableProduct);
            this.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ProductSelectForm";
            this.Text = "产品选择";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ProductSelectForm_FormClosing);
            this.Load += new System.EventHandler(this.ProductSelectForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cboAvaliableProduct;
        private System.Windows.Forms.Label lblUsefulProductTitle;
        private System.Windows.Forms.Button btnConfirm;
        private System.Windows.Forms.Button btnCancel;
    }
}