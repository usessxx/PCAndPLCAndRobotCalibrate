namespace UserManager
{
    partial class CreateNewOrEditUserForm
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
            this.lblUserNameTitle = new System.Windows.Forms.Label();
            this.lblPasswordTitle = new System.Windows.Forms.Label();
            this.lblPassword2Title = new System.Windows.Forms.Label();
            this.txtUserName = new System.Windows.Forms.TextBox();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.txtPassword2 = new System.Windows.Forms.TextBox();
            this.rbtnEngineerLevel = new System.Windows.Forms.RadioButton();
            this.lblLevelSelection = new System.Windows.Forms.Label();
            this.rbtnOperatorLevel = new System.Windows.Forms.RadioButton();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblUserNameTitle
            // 
            this.lblUserNameTitle.Location = new System.Drawing.Point(2, 24);
            this.lblUserNameTitle.Name = "lblUserNameTitle";
            this.lblUserNameTitle.Size = new System.Drawing.Size(89, 21);
            this.lblUserNameTitle.TabIndex = 0;
            this.lblUserNameTitle.Text = "用户名:";
            this.lblUserNameTitle.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblPasswordTitle
            // 
            this.lblPasswordTitle.Location = new System.Drawing.Point(2, 56);
            this.lblPasswordTitle.Name = "lblPasswordTitle";
            this.lblPasswordTitle.Size = new System.Drawing.Size(89, 21);
            this.lblPasswordTitle.TabIndex = 0;
            this.lblPasswordTitle.Text = "密码:";
            this.lblPasswordTitle.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblPassword2Title
            // 
            this.lblPassword2Title.Location = new System.Drawing.Point(2, 88);
            this.lblPassword2Title.Name = "lblPassword2Title";
            this.lblPassword2Title.Size = new System.Drawing.Size(89, 21);
            this.lblPassword2Title.TabIndex = 0;
            this.lblPassword2Title.Text = "确认密码:";
            this.lblPassword2Title.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtUserName
            // 
            this.txtUserName.Location = new System.Drawing.Point(98, 24);
            this.txtUserName.Name = "txtUserName";
            this.txtUserName.Size = new System.Drawing.Size(160, 23);
            this.txtUserName.TabIndex = 1;
            this.txtUserName.DoubleClick += new System.EventHandler(this.TextBoxDoubleClickEvent);
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(98, 56);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(160, 23);
            this.txtPassword.TabIndex = 1;
            this.txtPassword.DoubleClick += new System.EventHandler(this.TextBoxDoubleClickEvent);
            // 
            // txtPassword2
            // 
            this.txtPassword2.Location = new System.Drawing.Point(98, 88);
            this.txtPassword2.Name = "txtPassword2";
            this.txtPassword2.PasswordChar = '*';
            this.txtPassword2.Size = new System.Drawing.Size(160, 23);
            this.txtPassword2.TabIndex = 1;
            this.txtPassword2.DoubleClick += new System.EventHandler(this.TextBoxDoubleClickEvent);
            // 
            // rbtnEngineerLevel
            // 
            this.rbtnEngineerLevel.AutoSize = true;
            this.rbtnEngineerLevel.Location = new System.Drawing.Point(35, 141);
            this.rbtnEngineerLevel.Name = "rbtnEngineerLevel";
            this.rbtnEngineerLevel.Size = new System.Drawing.Size(95, 18);
            this.rbtnEngineerLevel.TabIndex = 2;
            this.rbtnEngineerLevel.TabStop = true;
            this.rbtnEngineerLevel.Text = "工程师权限";
            this.rbtnEngineerLevel.UseVisualStyleBackColor = true;
            this.rbtnEngineerLevel.CheckedChanged += new System.EventHandler(this.rbtnEngineerLevel_CheckedChanged);
            // 
            // lblLevelSelection
            // 
            this.lblLevelSelection.Font = new System.Drawing.Font("SimSun", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblLevelSelection.Location = new System.Drawing.Point(9, 119);
            this.lblLevelSelection.Name = "lblLevelSelection";
            this.lblLevelSelection.Size = new System.Drawing.Size(140, 17);
            this.lblLevelSelection.TabIndex = 3;
            this.lblLevelSelection.Text = "权限等级";
            this.lblLevelSelection.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // rbtnOperatorLevel
            // 
            this.rbtnOperatorLevel.AutoSize = true;
            this.rbtnOperatorLevel.Location = new System.Drawing.Point(159, 141);
            this.rbtnOperatorLevel.Name = "rbtnOperatorLevel";
            this.rbtnOperatorLevel.Size = new System.Drawing.Size(95, 18);
            this.rbtnOperatorLevel.TabIndex = 2;
            this.rbtnOperatorLevel.TabStop = true;
            this.rbtnOperatorLevel.Text = "操作员权限";
            this.rbtnOperatorLevel.UseVisualStyleBackColor = true;
            this.rbtnOperatorLevel.CheckedChanged += new System.EventHandler(this.rbtnOperatorLevel_CheckedChanged);
            // 
            // btnOK
            // 
            this.btnOK.Font = new System.Drawing.Font("SimSun", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnOK.Location = new System.Drawing.Point(35, 166);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(80, 31);
            this.btnOK.TabIndex = 4;
            this.btnOK.Text = "确认";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Font = new System.Drawing.Font("SimSun", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnCancel.Location = new System.Drawing.Point(149, 166);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(80, 31);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // CreateNewOrEditUserForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 209);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.lblLevelSelection);
            this.Controls.Add(this.rbtnOperatorLevel);
            this.Controls.Add(this.rbtnEngineerLevel);
            this.Controls.Add(this.txtPassword2);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.txtUserName);
            this.Controls.Add(this.lblPassword2Title);
            this.Controls.Add(this.lblPasswordTitle);
            this.Controls.Add(this.lblUserNameTitle);
            this.Font = new System.Drawing.Font("SimSun", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Name = "CreateNewOrEditUserForm";
            this.Text = "新建用户";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblUserNameTitle;
        private System.Windows.Forms.Label lblPasswordTitle;
        private System.Windows.Forms.Label lblPassword2Title;
        private System.Windows.Forms.TextBox txtUserName;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.TextBox txtPassword2;
        private System.Windows.Forms.RadioButton rbtnEngineerLevel;
        private System.Windows.Forms.Label lblLevelSelection;
        private System.Windows.Forms.RadioButton rbtnOperatorLevel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
    }
}