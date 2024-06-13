namespace ThreeDimensionAVI
{
    partial class SettingBaseForm
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
            if (_settingsAdminFormVariate != null)
            {
                _settingsAdminFormVariate.Dispose();
                _settingsAdminFormVariate = null;
            }
            if (_settingsGeneralFormVariate != null)
            {
                _settingsGeneralFormVariate.Dispose();
                _settingsGeneralFormVariate = null;
            }
            if (_settingsProductFormVariate != null)
            {
                _settingsProductFormVariate.Dispose();
                _settingsProductFormVariate = null;
            }
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
            this.pnlMaintenanceAndSettingsFormDisplay = new System.Windows.Forms.Panel();
            this.btnSettingsAdministrator = new System.Windows.Forms.Button();
            this.btnSettingsProduct = new System.Windows.Forms.Button();
            this.btnSettingsGeneral = new System.Windows.Forms.Button();
            this.btnMaintenanceAxesCtrl = new System.Windows.Forms.Button();
            this.btnMaintenanceCylinderCtrl = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.btnSetting = new System.Windows.Forms.Button();
            this.btnMaintenance = new System.Windows.Forms.Button();
            this.btnMain = new System.Windows.Forms.Button();
            this.pnlButton = new System.Windows.Forms.Panel();
            this.btnEachAxisCtrl = new System.Windows.Forms.Button();
            this.btnMaintenanceCamera = new System.Windows.Forms.Button();
            this.btnSettingsCameraCalibration = new System.Windows.Forms.Button();
            this.pnlButton.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlMaintenanceAndSettingsFormDisplay
            // 
            this.pnlMaintenanceAndSettingsFormDisplay.Location = new System.Drawing.Point(0, 90);
            this.pnlMaintenanceAndSettingsFormDisplay.Name = "pnlMaintenanceAndSettingsFormDisplay";
            this.pnlMaintenanceAndSettingsFormDisplay.Size = new System.Drawing.Size(1317, 686);
            this.pnlMaintenanceAndSettingsFormDisplay.TabIndex = 2;
            // 
            // btnSettingsAdministrator
            // 
            this.btnSettingsAdministrator.BackColor = System.Drawing.Color.Gainsboro;
            this.btnSettingsAdministrator.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSettingsAdministrator.Location = new System.Drawing.Point(378, 44);
            this.btnSettingsAdministrator.Name = "btnSettingsAdministrator";
            this.btnSettingsAdministrator.Size = new System.Drawing.Size(159, 37);
            this.btnSettingsAdministrator.TabIndex = 3;
            this.btnSettingsAdministrator.Text = "设置-管理员参数";
            this.btnSettingsAdministrator.UseVisualStyleBackColor = false;
            this.btnSettingsAdministrator.Visible = false;
            this.btnSettingsAdministrator.Click += new System.EventHandler(this.ClickFormChangeBtnEvent);
            // 
            // btnSettingsProduct
            // 
            this.btnSettingsProduct.BackColor = System.Drawing.Color.Gainsboro;
            this.btnSettingsProduct.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSettingsProduct.Location = new System.Drawing.Point(196, 44);
            this.btnSettingsProduct.Name = "btnSettingsProduct";
            this.btnSettingsProduct.Size = new System.Drawing.Size(159, 37);
            this.btnSettingsProduct.TabIndex = 4;
            this.btnSettingsProduct.Text = "设置-产品参数";
            this.btnSettingsProduct.UseVisualStyleBackColor = false;
            this.btnSettingsProduct.Visible = false;
            this.btnSettingsProduct.Click += new System.EventHandler(this.ClickFormChangeBtnEvent);
            // 
            // btnSettingsGeneral
            // 
            this.btnSettingsGeneral.BackColor = System.Drawing.Color.Gainsboro;
            this.btnSettingsGeneral.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSettingsGeneral.Location = new System.Drawing.Point(14, 44);
            this.btnSettingsGeneral.Name = "btnSettingsGeneral";
            this.btnSettingsGeneral.Size = new System.Drawing.Size(159, 37);
            this.btnSettingsGeneral.TabIndex = 1;
            this.btnSettingsGeneral.Text = "设置-通用参数";
            this.btnSettingsGeneral.UseVisualStyleBackColor = false;
            this.btnSettingsGeneral.Visible = false;
            this.btnSettingsGeneral.Click += new System.EventHandler(this.ClickFormChangeBtnEvent);
            // 
            // btnMaintenanceAxesCtrl
            // 
            this.btnMaintenanceAxesCtrl.BackColor = System.Drawing.Color.Orange;
            this.btnMaintenanceAxesCtrl.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMaintenanceAxesCtrl.Location = new System.Drawing.Point(196, 44);
            this.btnMaintenanceAxesCtrl.Name = "btnMaintenanceAxesCtrl";
            this.btnMaintenanceAxesCtrl.Size = new System.Drawing.Size(159, 37);
            this.btnMaintenanceAxesCtrl.TabIndex = 8;
            this.btnMaintenanceAxesCtrl.Text = "手动-轴运动控制";
            this.btnMaintenanceAxesCtrl.UseVisualStyleBackColor = false;
            this.btnMaintenanceAxesCtrl.Click += new System.EventHandler(this.ClickFormChangeBtnEvent);
            // 
            // btnMaintenanceCylinderCtrl
            // 
            this.btnMaintenanceCylinderCtrl.BackColor = System.Drawing.Color.Gainsboro;
            this.btnMaintenanceCylinderCtrl.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMaintenanceCylinderCtrl.Location = new System.Drawing.Point(14, 44);
            this.btnMaintenanceCylinderCtrl.Name = "btnMaintenanceCylinderCtrl";
            this.btnMaintenanceCylinderCtrl.Size = new System.Drawing.Size(159, 37);
            this.btnMaintenanceCylinderCtrl.TabIndex = 10;
            this.btnMaintenanceCylinderCtrl.Text = "手动-气缸控制";
            this.btnMaintenanceCylinderCtrl.UseVisualStyleBackColor = false;
            this.btnMaintenanceCylinderCtrl.Click += new System.EventHandler(this.ClickFormChangeBtnEvent);
            // 
            // btnExit
            // 
            this.btnExit.BackColor = System.Drawing.Color.OrangeRed;
            this.btnExit.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExit.Location = new System.Drawing.Point(742, 1);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(159, 37);
            this.btnExit.TabIndex = 5;
            this.btnExit.Text = "退出";
            this.btnExit.UseVisualStyleBackColor = false;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnSetting
            // 
            this.btnSetting.BackColor = System.Drawing.Color.Gainsboro;
            this.btnSetting.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSetting.Location = new System.Drawing.Point(378, 1);
            this.btnSetting.Name = "btnSetting";
            this.btnSetting.Size = new System.Drawing.Size(159, 37);
            this.btnSetting.TabIndex = 7;
            this.btnSetting.Text = "设置页面";
            this.btnSetting.UseVisualStyleBackColor = false;
            this.btnSetting.Click += new System.EventHandler(this.ClickFormChangeBtnEvent);
            // 
            // btnMaintenance
            // 
            this.btnMaintenance.BackColor = System.Drawing.Color.Orange;
            this.btnMaintenance.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMaintenance.Location = new System.Drawing.Point(196, 1);
            this.btnMaintenance.Name = "btnMaintenance";
            this.btnMaintenance.Size = new System.Drawing.Size(159, 37);
            this.btnMaintenance.TabIndex = 9;
            this.btnMaintenance.Text = "手动页面";
            this.btnMaintenance.UseVisualStyleBackColor = false;
            this.btnMaintenance.Click += new System.EventHandler(this.ClickFormChangeBtnEvent);
            // 
            // btnMain
            // 
            this.btnMain.BackColor = System.Drawing.Color.Lime;
            this.btnMain.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMain.Location = new System.Drawing.Point(14, 1);
            this.btnMain.Name = "btnMain";
            this.btnMain.Size = new System.Drawing.Size(159, 37);
            this.btnMain.TabIndex = 11;
            this.btnMain.Text = "主界面";
            this.btnMain.UseVisualStyleBackColor = false;
            this.btnMain.Click += new System.EventHandler(this.btnMain_Click);
            // 
            // pnlButton
            // 
            this.pnlButton.Controls.Add(this.btnMaintenanceAxesCtrl);
            this.pnlButton.Controls.Add(this.btnMain);
            this.pnlButton.Controls.Add(this.btnMaintenanceCylinderCtrl);
            this.pnlButton.Controls.Add(this.btnMaintenance);
            this.pnlButton.Controls.Add(this.btnSetting);
            this.pnlButton.Controls.Add(this.btnEachAxisCtrl);
            this.pnlButton.Controls.Add(this.btnExit);
            this.pnlButton.Controls.Add(this.btnSettingsGeneral);
            this.pnlButton.Controls.Add(this.btnSettingsProduct);
            this.pnlButton.Controls.Add(this.btnMaintenanceCamera);
            this.pnlButton.Controls.Add(this.btnSettingsCameraCalibration);
            this.pnlButton.Controls.Add(this.btnSettingsAdministrator);
            this.pnlButton.Location = new System.Drawing.Point(3, 4);
            this.pnlButton.Name = "pnlButton";
            this.pnlButton.Size = new System.Drawing.Size(907, 83);
            this.pnlButton.TabIndex = 0;
            // 
            // btnEachAxisCtrl
            // 
            this.btnEachAxisCtrl.BackColor = System.Drawing.Color.LightYellow;
            this.btnEachAxisCtrl.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEachAxisCtrl.Location = new System.Drawing.Point(560, 1);
            this.btnEachAxisCtrl.Name = "btnEachAxisCtrl";
            this.btnEachAxisCtrl.Size = new System.Drawing.Size(159, 37);
            this.btnEachAxisCtrl.TabIndex = 5;
            this.btnEachAxisCtrl.Text = "轴控制界面";
            this.btnEachAxisCtrl.UseVisualStyleBackColor = false;
            this.btnEachAxisCtrl.Click += new System.EventHandler(this.btnEachAxisCtrl_Click);
            // 
            // btnMaintenanceCamera
            // 
            this.btnMaintenanceCamera.BackColor = System.Drawing.Color.Gainsboro;
            this.btnMaintenanceCamera.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMaintenanceCamera.Location = new System.Drawing.Point(378, 44);
            this.btnMaintenanceCamera.Name = "btnMaintenanceCamera";
            this.btnMaintenanceCamera.Size = new System.Drawing.Size(159, 37);
            this.btnMaintenanceCamera.TabIndex = 3;
            this.btnMaintenanceCamera.Text = "手动-相机控制";
            this.btnMaintenanceCamera.UseVisualStyleBackColor = false;
            this.btnMaintenanceCamera.Visible = false;
            this.btnMaintenanceCamera.Click += new System.EventHandler(this.ClickFormChangeBtnEvent);
            // 
            // btnSettingsCameraCalibration
            // 
            this.btnSettingsCameraCalibration.BackColor = System.Drawing.Color.Gainsboro;
            this.btnSettingsCameraCalibration.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSettingsCameraCalibration.Location = new System.Drawing.Point(560, 44);
            this.btnSettingsCameraCalibration.Name = "btnSettingsCameraCalibration";
            this.btnSettingsCameraCalibration.Size = new System.Drawing.Size(159, 37);
            this.btnSettingsCameraCalibration.TabIndex = 3;
            this.btnSettingsCameraCalibration.Text = "设置-相机标定";
            this.btnSettingsCameraCalibration.UseVisualStyleBackColor = false;
            this.btnSettingsCameraCalibration.Visible = false;
            this.btnSettingsCameraCalibration.Click += new System.EventHandler(this.ClickFormChangeBtnEvent);
            // 
            // SettingBaseForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1317, 777);
            this.ControlBox = false;
            this.Controls.Add(this.pnlButton);
            this.Controls.Add(this.pnlMaintenanceAndSettingsFormDisplay);
            this.Font = new System.Drawing.Font("Times New Roman", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "SettingBaseForm";
            this.Text = "SettingBaseForm";
            this.VisibleChanged += new System.EventHandler(this.SettingBaseForm_VisibleChanged);
            this.pnlButton.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlMaintenanceAndSettingsFormDisplay;
        private System.Windows.Forms.Button btnSettingsAdministrator;
        private System.Windows.Forms.Button btnSettingsProduct;
        private System.Windows.Forms.Button btnSettingsGeneral;
        private System.Windows.Forms.Button btnMaintenanceAxesCtrl;
        private System.Windows.Forms.Button btnMaintenanceCylinderCtrl;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Button btnSetting;
        private System.Windows.Forms.Button btnMaintenance;
        private System.Windows.Forms.Button btnMain;
        private System.Windows.Forms.Panel pnlButton;
        private System.Windows.Forms.Button btnEachAxisCtrl;
        private System.Windows.Forms.Button btnMaintenanceCamera;
        private System.Windows.Forms.Button btnSettingsCameraCalibration;
    }
}