namespace UserManager
{
    partial class DeleteUserForm
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
            this.hScrollBar1 = new System.Windows.Forms.HScrollBar();
            this.lvCouldBeDeleteUserDisp = new System.Windows.Forms.ListView();
            this.lblCouldBeDeleteUserTitle = new System.Windows.Forms.Label();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // hScrollBar1
            // 
            this.hScrollBar1.Location = new System.Drawing.Point(315, 276);
            this.hScrollBar1.Name = "hScrollBar1";
            this.hScrollBar1.Size = new System.Drawing.Size(9, 8);
            this.hScrollBar1.TabIndex = 0;
            // 
            // lvCouldBeDeleteUserDisp
            // 
            this.lvCouldBeDeleteUserDisp.Font = new System.Drawing.Font("Times New Roman", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lvCouldBeDeleteUserDisp.Location = new System.Drawing.Point(13, 40);
            this.lvCouldBeDeleteUserDisp.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.lvCouldBeDeleteUserDisp.Name = "lvCouldBeDeleteUserDisp";
            this.lvCouldBeDeleteUserDisp.Size = new System.Drawing.Size(472, 221);
            this.lvCouldBeDeleteUserDisp.TabIndex = 2;
            this.lvCouldBeDeleteUserDisp.UseCompatibleStateImageBehavior = false;
            // 
            // lblCouldBeDeleteUserTitle
            // 
            this.lblCouldBeDeleteUserTitle.Font = new System.Drawing.Font("SimSun", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCouldBeDeleteUserTitle.Location = new System.Drawing.Point(12, 10);
            this.lblCouldBeDeleteUserTitle.Name = "lblCouldBeDeleteUserTitle";
            this.lblCouldBeDeleteUserTitle.Size = new System.Drawing.Size(170, 24);
            this.lblCouldBeDeleteUserTitle.TabIndex = 5;
            this.lblCouldBeDeleteUserTitle.Text = "可被删除用户:";
            this.lblCouldBeDeleteUserTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnDelete
            // 
            this.btnDelete.Font = new System.Drawing.Font("SimSun", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnDelete.Location = new System.Drawing.Point(49, 270);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(103, 36);
            this.btnDelete.TabIndex = 6;
            this.btnDelete.Text = "删除";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Font = new System.Drawing.Font("SimSun", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnCancel.Location = new System.Drawing.Point(315, 270);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(103, 36);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // DeleteUserForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(497, 318);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.lblCouldBeDeleteUserTitle);
            this.Controls.Add(this.lvCouldBeDeleteUserDisp);
            this.Controls.Add(this.hScrollBar1);
            this.Font = new System.Drawing.Font("SimSun", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DeleteUserForm";
            this.Text = "删除用户";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.HScrollBar hScrollBar1;
        private System.Windows.Forms.ListView lvCouldBeDeleteUserDisp;
        private System.Windows.Forms.Label lblCouldBeDeleteUserTitle;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnCancel;
    }
}