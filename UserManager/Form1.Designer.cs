namespace UserManager
{
    partial class UserCreateAndLogTotalForm
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
            UserPasswordListSaving();

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
            this.components = new System.ComponentModel.Container();
            this.lvUserDisplay = new System.Windows.Forms.ListView();
            this.btnCreateUser = new System.Windows.Forms.Button();
            this.btnEditUser = new System.Windows.Forms.Button();
            this.btnDeleteUser = new System.Windows.Forms.Button();
            this.txtPasswordInput = new System.Windows.Forms.TextBox();
            this.btnLogIn = new System.Windows.Forms.Button();
            this.lblCurrentUserNameTitle = new System.Windows.Forms.Label();
            this.lblCurrentUserName = new System.Windows.Forms.Label();
            this.lblAccessLevelTimeoutTitle = new System.Windows.Forms.Label();
            this.txtTimeoutTime = new System.Windows.Forms.TextBox();
            this.lblAccessLevelTimeoutUnit = new System.Windows.Forms.Label();
            this.lblChangeUserNameAfterTimeoutTitle = new System.Windows.Forms.Label();
            this.cboChangeUserNameAfterTimeout = new System.Windows.Forms.ComboBox();
            this.tmrJudgeAccessLevelChange = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // lvUserDisplay
            // 
            this.lvUserDisplay.Font = new System.Drawing.Font("Times New Roman", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lvUserDisplay.Location = new System.Drawing.Point(13, 34);
            this.lvUserDisplay.Margin = new System.Windows.Forms.Padding(4);
            this.lvUserDisplay.Name = "lvUserDisplay";
            this.lvUserDisplay.Size = new System.Drawing.Size(550, 264);
            this.lvUserDisplay.TabIndex = 1;
            this.lvUserDisplay.UseCompatibleStateImageBehavior = false;
            // 
            // btnCreateUser
            // 
            this.btnCreateUser.Enabled = false;
            this.btnCreateUser.Location = new System.Drawing.Point(13, 306);
            this.btnCreateUser.Name = "btnCreateUser";
            this.btnCreateUser.Size = new System.Drawing.Size(86, 26);
            this.btnCreateUser.TabIndex = 2;
            this.btnCreateUser.Text = "新建";
            this.btnCreateUser.UseVisualStyleBackColor = true;
            this.btnCreateUser.Click += new System.EventHandler(this.btnCreateUser_Click);
            // 
            // btnEditUser
            // 
            this.btnEditUser.Enabled = false;
            this.btnEditUser.Location = new System.Drawing.Point(105, 305);
            this.btnEditUser.Name = "btnEditUser";
            this.btnEditUser.Size = new System.Drawing.Size(86, 26);
            this.btnEditUser.TabIndex = 2;
            this.btnEditUser.Text = "编辑";
            this.btnEditUser.UseVisualStyleBackColor = true;
            this.btnEditUser.Click += new System.EventHandler(this.btnEditUser_Click);
            // 
            // btnDeleteUser
            // 
            this.btnDeleteUser.Enabled = false;
            this.btnDeleteUser.Location = new System.Drawing.Point(197, 305);
            this.btnDeleteUser.Name = "btnDeleteUser";
            this.btnDeleteUser.Size = new System.Drawing.Size(86, 26);
            this.btnDeleteUser.TabIndex = 2;
            this.btnDeleteUser.Text = "删除";
            this.btnDeleteUser.UseVisualStyleBackColor = true;
            this.btnDeleteUser.Click += new System.EventHandler(this.btnDeleteUser_Click);
            // 
            // txtPasswordInput
            // 
            this.txtPasswordInput.Location = new System.Drawing.Point(298, 306);
            this.txtPasswordInput.Name = "txtPasswordInput";
            this.txtPasswordInput.PasswordChar = '*';
            this.txtPasswordInput.Size = new System.Drawing.Size(173, 24);
            this.txtPasswordInput.TabIndex = 3;
            this.txtPasswordInput.DoubleClick += new System.EventHandler(this.TextBoxDoubleClickEvent);
            // 
            // btnLogIn
            // 
            this.btnLogIn.Location = new System.Drawing.Point(477, 306);
            this.btnLogIn.Name = "btnLogIn";
            this.btnLogIn.Size = new System.Drawing.Size(86, 26);
            this.btnLogIn.TabIndex = 2;
            this.btnLogIn.Text = "登录";
            this.btnLogIn.UseVisualStyleBackColor = true;
            this.btnLogIn.Click += new System.EventHandler(this.btnLogIn_Click);
            // 
            // lblCurrentUserNameTitle
            // 
            this.lblCurrentUserNameTitle.Font = new System.Drawing.Font("SimSun", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCurrentUserNameTitle.Location = new System.Drawing.Point(12, 9);
            this.lblCurrentUserNameTitle.Name = "lblCurrentUserNameTitle";
            this.lblCurrentUserNameTitle.Size = new System.Drawing.Size(87, 21);
            this.lblCurrentUserNameTitle.TabIndex = 4;
            this.lblCurrentUserNameTitle.Text = "当前用户:";
            this.lblCurrentUserNameTitle.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblCurrentUserName
            // 
            this.lblCurrentUserName.Font = new System.Drawing.Font("SimSun", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblCurrentUserName.Location = new System.Drawing.Point(102, 9);
            this.lblCurrentUserName.Name = "lblCurrentUserName";
            this.lblCurrentUserName.Size = new System.Drawing.Size(181, 21);
            this.lblCurrentUserName.TabIndex = 4;
            this.lblCurrentUserName.Text = "3DAOI";
            this.lblCurrentUserName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblAccessLevelTimeoutTitle
            // 
            this.lblAccessLevelTimeoutTitle.Location = new System.Drawing.Point(15, 339);
            this.lblAccessLevelTimeoutTitle.Name = "lblAccessLevelTimeoutTitle";
            this.lblAccessLevelTimeoutTitle.Size = new System.Drawing.Size(103, 23);
            this.lblAccessLevelTimeoutTitle.TabIndex = 5;
            this.lblAccessLevelTimeoutTitle.Text = "权限超时时间:";
            this.lblAccessLevelTimeoutTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txtTimeoutTime
            // 
            this.txtTimeoutTime.Enabled = false;
            this.txtTimeoutTime.Location = new System.Drawing.Point(116, 339);
            this.txtTimeoutTime.Name = "txtTimeoutTime";
            this.txtTimeoutTime.Size = new System.Drawing.Size(100, 24);
            this.txtTimeoutTime.TabIndex = 6;
            this.txtTimeoutTime.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtTimeoutTime.TextChanged += new System.EventHandler(this.TextBoxDataCheckEvent);
            this.txtTimeoutTime.DoubleClick += new System.EventHandler(this.TextBoxDoubleClickEvent);
            this.txtTimeoutTime.Leave += new System.EventHandler(this.TextBoxLeaveEvent);
            // 
            // lblAccessLevelTimeoutUnit
            // 
            this.lblAccessLevelTimeoutUnit.Location = new System.Drawing.Point(219, 339);
            this.lblAccessLevelTimeoutUnit.Name = "lblAccessLevelTimeoutUnit";
            this.lblAccessLevelTimeoutUnit.Size = new System.Drawing.Size(41, 23);
            this.lblAccessLevelTimeoutUnit.TabIndex = 5;
            this.lblAccessLevelTimeoutUnit.Text = "秒";
            this.lblAccessLevelTimeoutUnit.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblChangeUserNameAfterTimeoutTitle
            // 
            this.lblChangeUserNameAfterTimeoutTitle.Location = new System.Drawing.Point(264, 339);
            this.lblChangeUserNameAfterTimeoutTitle.Name = "lblChangeUserNameAfterTimeoutTitle";
            this.lblChangeUserNameAfterTimeoutTitle.Size = new System.Drawing.Size(161, 23);
            this.lblChangeUserNameAfterTimeoutTitle.TabIndex = 5;
            this.lblChangeUserNameAfterTimeoutTitle.Text = "权限超时后切换用户名:";
            this.lblChangeUserNameAfterTimeoutTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // cboChangeUserNameAfterTimeout
            // 
            this.cboChangeUserNameAfterTimeout.Enabled = false;
            this.cboChangeUserNameAfterTimeout.FormattingEnabled = true;
            this.cboChangeUserNameAfterTimeout.Location = new System.Drawing.Point(428, 339);
            this.cboChangeUserNameAfterTimeout.Name = "cboChangeUserNameAfterTimeout";
            this.cboChangeUserNameAfterTimeout.Size = new System.Drawing.Size(135, 24);
            this.cboChangeUserNameAfterTimeout.TabIndex = 8;
            this.cboChangeUserNameAfterTimeout.SelectedIndexChanged += new System.EventHandler(this.cboChangeUserNameAfterTimeout_SelectedIndexChanged);
            // 
            // tmrJudgeAccessLevelChange
            // 
            this.tmrJudgeAccessLevelChange.Tick += new System.EventHandler(this.tmrJudgeAccessLevelChange_Tick);
            // 
            // UserCreateAndLogTotalForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(575, 367);
            this.Controls.Add(this.cboChangeUserNameAfterTimeout);
            this.Controls.Add(this.txtTimeoutTime);
            this.Controls.Add(this.lblAccessLevelTimeoutUnit);
            this.Controls.Add(this.lblChangeUserNameAfterTimeoutTitle);
            this.Controls.Add(this.lblAccessLevelTimeoutTitle);
            this.Controls.Add(this.lblCurrentUserName);
            this.Controls.Add(this.lblCurrentUserNameTitle);
            this.Controls.Add(this.txtPasswordInput);
            this.Controls.Add(this.btnLogIn);
            this.Controls.Add(this.btnDeleteUser);
            this.Controls.Add(this.btnEditUser);
            this.Controls.Add(this.btnCreateUser);
            this.Controls.Add(this.lvUserDisplay);
            this.Font = new System.Drawing.Font("Times New Roman", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "UserCreateAndLogTotalForm";
            this.Text = "用户登录创建界面";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.UserCreateAndLogTotalForm_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView lvUserDisplay;
        private System.Windows.Forms.Button btnCreateUser;
        private System.Windows.Forms.Button btnEditUser;
        private System.Windows.Forms.Button btnDeleteUser;
        private System.Windows.Forms.TextBox txtPasswordInput;
        private System.Windows.Forms.Button btnLogIn;
        private System.Windows.Forms.Label lblCurrentUserNameTitle;
        private System.Windows.Forms.Label lblCurrentUserName;
        private System.Windows.Forms.Label lblAccessLevelTimeoutTitle;
        private System.Windows.Forms.TextBox txtTimeoutTime;
        private System.Windows.Forms.Label lblAccessLevelTimeoutUnit;
        private System.Windows.Forms.Label lblChangeUserNameAfterTimeoutTitle;
        private System.Windows.Forms.ComboBox cboChangeUserNameAfterTimeout;
        private System.Windows.Forms.Timer tmrJudgeAccessLevelChange;
    }
}

