using System.Threading;

namespace ThreeDimensionAVI
{
    partial class MaintenanceCylinderControlForm
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
            if (_statusRefreshThread != null)
            {
                //if (!this.Visible)
                //    _statusRefreshThread.Resume();
                _statusRefreshThread.Abort();
                _statusRefreshThread = null;
            }

            Thread.Sleep(100);//等待线程关闭

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
            this.rectangleShape1 = new Microsoft.VisualBasic.PowerPacks.RectangleShape();
            this.shapeContainer1 = new Microsoft.VisualBasic.PowerPacks.ShapeContainer();
            this.btnCameraTransferCylinder = new System.Windows.Forms.Button();
            this.btnCameraTransferCylinderBackStatus = new System.Windows.Forms.Button();
            this.btnCameraTransferCylinderOutStatus = new System.Windows.Forms.Button();
            this.btnConveyor1StopCylinder = new System.Windows.Forms.Button();
            this.btnConveyor1StopCylinderDownStatus = new System.Windows.Forms.Button();
            this.btnConveyor1StopCylinderUpStatus = new System.Windows.Forms.Button();
            this.btnConveyor1ClampCylinderReleaseStatus = new System.Windows.Forms.Button();
            this.btnConveyor1ClampCylinder = new System.Windows.Forms.Button();
            this.btnConveyor1ClampCylinderClampStatus = new System.Windows.Forms.Button();
            this.btnConveyor2StopCylinderDownStatus = new System.Windows.Forms.Button();
            this.btnConveyor2StopCylinder = new System.Windows.Forms.Button();
            this.btnConveyor2StopCylinderUpStatus = new System.Windows.Forms.Button();
            this.btnConveyor2ClampCylinderReleaseStatus = new System.Windows.Forms.Button();
            this.btnConveyor2ClampCylinder = new System.Windows.Forms.Button();
            this.btnConveyor2ClampCylinderClampStatus = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // rectangleShape1
            // 
            this.rectangleShape1.Location = new System.Drawing.Point(0, 0);
            this.rectangleShape1.Name = "rectangleShape1";
            this.rectangleShape1.Size = new System.Drawing.Size(1316, 685);
            // 
            // shapeContainer1
            // 
            this.shapeContainer1.Location = new System.Drawing.Point(0, 0);
            this.shapeContainer1.Margin = new System.Windows.Forms.Padding(0);
            this.shapeContainer1.Name = "shapeContainer1";
            this.shapeContainer1.Shapes.AddRange(new Microsoft.VisualBasic.PowerPacks.Shape[] {
            this.rectangleShape1});
            this.shapeContainer1.Size = new System.Drawing.Size(1317, 686);
            this.shapeContainer1.TabIndex = 0;
            this.shapeContainer1.TabStop = false;
            // 
            // btnCameraTransferCylinder
            // 
            this.btnCameraTransferCylinder.BackColor = System.Drawing.Color.LimeGreen;
            this.btnCameraTransferCylinder.Font = new System.Drawing.Font("SimSun", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnCameraTransferCylinder.Location = new System.Drawing.Point(56, 25);
            this.btnCameraTransferCylinder.Name = "btnCameraTransferCylinder";
            this.btnCameraTransferCylinder.Size = new System.Drawing.Size(135, 63);
            this.btnCameraTransferCylinder.TabIndex = 6;
            this.btnCameraTransferCylinder.Text = "相机移动气缸回";
            this.btnCameraTransferCylinder.UseVisualStyleBackColor = false;
            this.btnCameraTransferCylinder.Click += new System.EventHandler(this.CylinderControlBtnEvent);
            // 
            // btnCameraTransferCylinderBackStatus
            // 
            this.btnCameraTransferCylinderBackStatus.BackColor = System.Drawing.SystemColors.ControlDark;
            this.btnCameraTransferCylinderBackStatus.Font = new System.Drawing.Font("SimSun", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnCameraTransferCylinderBackStatus.Location = new System.Drawing.Point(27, 25);
            this.btnCameraTransferCylinderBackStatus.Name = "btnCameraTransferCylinderBackStatus";
            this.btnCameraTransferCylinderBackStatus.Size = new System.Drawing.Size(27, 63);
            this.btnCameraTransferCylinderBackStatus.TabIndex = 6;
            this.btnCameraTransferCylinderBackStatus.UseVisualStyleBackColor = false;
            // 
            // btnCameraTransferCylinderOutStatus
            // 
            this.btnCameraTransferCylinderOutStatus.BackColor = System.Drawing.SystemColors.ControlDark;
            this.btnCameraTransferCylinderOutStatus.Font = new System.Drawing.Font("SimSun", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnCameraTransferCylinderOutStatus.Location = new System.Drawing.Point(193, 25);
            this.btnCameraTransferCylinderOutStatus.Name = "btnCameraTransferCylinderOutStatus";
            this.btnCameraTransferCylinderOutStatus.Size = new System.Drawing.Size(27, 63);
            this.btnCameraTransferCylinderOutStatus.TabIndex = 6;
            this.btnCameraTransferCylinderOutStatus.UseVisualStyleBackColor = false;
            // 
            // btnConveyor1StopCylinder
            // 
            this.btnConveyor1StopCylinder.BackColor = System.Drawing.Color.LimeGreen;
            this.btnConveyor1StopCylinder.Font = new System.Drawing.Font("SimSun", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnConveyor1StopCylinder.Location = new System.Drawing.Point(56, 219);
            this.btnConveyor1StopCylinder.Name = "btnConveyor1StopCylinder";
            this.btnConveyor1StopCylinder.Size = new System.Drawing.Size(135, 63);
            this.btnConveyor1StopCylinder.TabIndex = 6;
            this.btnConveyor1StopCylinder.Text = "传送线1阻挡气缸下";
            this.btnConveyor1StopCylinder.UseVisualStyleBackColor = false;
            this.btnConveyor1StopCylinder.Click += new System.EventHandler(this.CylinderControlBtnEvent);
            // 
            // btnConveyor1StopCylinderDownStatus
            // 
            this.btnConveyor1StopCylinderDownStatus.BackColor = System.Drawing.SystemColors.ControlDark;
            this.btnConveyor1StopCylinderDownStatus.Font = new System.Drawing.Font("SimSun", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnConveyor1StopCylinderDownStatus.Location = new System.Drawing.Point(27, 219);
            this.btnConveyor1StopCylinderDownStatus.Name = "btnConveyor1StopCylinderDownStatus";
            this.btnConveyor1StopCylinderDownStatus.Size = new System.Drawing.Size(27, 63);
            this.btnConveyor1StopCylinderDownStatus.TabIndex = 6;
            this.btnConveyor1StopCylinderDownStatus.UseVisualStyleBackColor = false;
            // 
            // btnConveyor1StopCylinderUpStatus
            // 
            this.btnConveyor1StopCylinderUpStatus.BackColor = System.Drawing.SystemColors.ControlDark;
            this.btnConveyor1StopCylinderUpStatus.Font = new System.Drawing.Font("SimSun", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnConveyor1StopCylinderUpStatus.Location = new System.Drawing.Point(193, 219);
            this.btnConveyor1StopCylinderUpStatus.Name = "btnConveyor1StopCylinderUpStatus";
            this.btnConveyor1StopCylinderUpStatus.Size = new System.Drawing.Size(27, 63);
            this.btnConveyor1StopCylinderUpStatus.TabIndex = 6;
            this.btnConveyor1StopCylinderUpStatus.UseVisualStyleBackColor = false;
            // 
            // btnConveyor1ClampCylinderReleaseStatus
            // 
            this.btnConveyor1ClampCylinderReleaseStatus.BackColor = System.Drawing.SystemColors.ControlDark;
            this.btnConveyor1ClampCylinderReleaseStatus.Font = new System.Drawing.Font("SimSun", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnConveyor1ClampCylinderReleaseStatus.Location = new System.Drawing.Point(257, 220);
            this.btnConveyor1ClampCylinderReleaseStatus.Name = "btnConveyor1ClampCylinderReleaseStatus";
            this.btnConveyor1ClampCylinderReleaseStatus.Size = new System.Drawing.Size(27, 63);
            this.btnConveyor1ClampCylinderReleaseStatus.TabIndex = 6;
            this.btnConveyor1ClampCylinderReleaseStatus.UseVisualStyleBackColor = false;
            // 
            // btnConveyor1ClampCylinder
            // 
            this.btnConveyor1ClampCylinder.BackColor = System.Drawing.Color.LimeGreen;
            this.btnConveyor1ClampCylinder.Font = new System.Drawing.Font("SimSun", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnConveyor1ClampCylinder.Location = new System.Drawing.Point(286, 220);
            this.btnConveyor1ClampCylinder.Name = "btnConveyor1ClampCylinder";
            this.btnConveyor1ClampCylinder.Size = new System.Drawing.Size(135, 63);
            this.btnConveyor1ClampCylinder.TabIndex = 6;
            this.btnConveyor1ClampCylinder.Text = "传送线1夹紧气缸松开";
            this.btnConveyor1ClampCylinder.UseVisualStyleBackColor = false;
            this.btnConveyor1ClampCylinder.Click += new System.EventHandler(this.CylinderControlBtnEvent);
            // 
            // btnConveyor1ClampCylinderClampStatus
            // 
            this.btnConveyor1ClampCylinderClampStatus.BackColor = System.Drawing.SystemColors.ControlDark;
            this.btnConveyor1ClampCylinderClampStatus.Font = new System.Drawing.Font("SimSun", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnConveyor1ClampCylinderClampStatus.Location = new System.Drawing.Point(423, 220);
            this.btnConveyor1ClampCylinderClampStatus.Name = "btnConveyor1ClampCylinderClampStatus";
            this.btnConveyor1ClampCylinderClampStatus.Size = new System.Drawing.Size(27, 63);
            this.btnConveyor1ClampCylinderClampStatus.TabIndex = 6;
            this.btnConveyor1ClampCylinderClampStatus.UseVisualStyleBackColor = false;
            // 
            // btnConveyor2StopCylinderDownStatus
            // 
            this.btnConveyor2StopCylinderDownStatus.BackColor = System.Drawing.SystemColors.ControlDark;
            this.btnConveyor2StopCylinderDownStatus.Font = new System.Drawing.Font("SimSun", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnConveyor2StopCylinderDownStatus.Location = new System.Drawing.Point(27, 122);
            this.btnConveyor2StopCylinderDownStatus.Name = "btnConveyor2StopCylinderDownStatus";
            this.btnConveyor2StopCylinderDownStatus.Size = new System.Drawing.Size(27, 63);
            this.btnConveyor2StopCylinderDownStatus.TabIndex = 6;
            this.btnConveyor2StopCylinderDownStatus.UseVisualStyleBackColor = false;
            // 
            // btnConveyor2StopCylinder
            // 
            this.btnConveyor2StopCylinder.BackColor = System.Drawing.Color.LimeGreen;
            this.btnConveyor2StopCylinder.Font = new System.Drawing.Font("SimSun", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnConveyor2StopCylinder.Location = new System.Drawing.Point(56, 122);
            this.btnConveyor2StopCylinder.Name = "btnConveyor2StopCylinder";
            this.btnConveyor2StopCylinder.Size = new System.Drawing.Size(135, 63);
            this.btnConveyor2StopCylinder.TabIndex = 6;
            this.btnConveyor2StopCylinder.Text = "传送线2阻挡气缸下";
            this.btnConveyor2StopCylinder.UseVisualStyleBackColor = false;
            this.btnConveyor2StopCylinder.Click += new System.EventHandler(this.CylinderControlBtnEvent);
            // 
            // btnConveyor2StopCylinderUpStatus
            // 
            this.btnConveyor2StopCylinderUpStatus.BackColor = System.Drawing.SystemColors.ControlDark;
            this.btnConveyor2StopCylinderUpStatus.Font = new System.Drawing.Font("SimSun", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnConveyor2StopCylinderUpStatus.Location = new System.Drawing.Point(193, 122);
            this.btnConveyor2StopCylinderUpStatus.Name = "btnConveyor2StopCylinderUpStatus";
            this.btnConveyor2StopCylinderUpStatus.Size = new System.Drawing.Size(27, 63);
            this.btnConveyor2StopCylinderUpStatus.TabIndex = 6;
            this.btnConveyor2StopCylinderUpStatus.UseVisualStyleBackColor = false;
            // 
            // btnConveyor2ClampCylinderReleaseStatus
            // 
            this.btnConveyor2ClampCylinderReleaseStatus.BackColor = System.Drawing.SystemColors.ControlDark;
            this.btnConveyor2ClampCylinderReleaseStatus.Font = new System.Drawing.Font("SimSun", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnConveyor2ClampCylinderReleaseStatus.Location = new System.Drawing.Point(257, 122);
            this.btnConveyor2ClampCylinderReleaseStatus.Name = "btnConveyor2ClampCylinderReleaseStatus";
            this.btnConveyor2ClampCylinderReleaseStatus.Size = new System.Drawing.Size(27, 63);
            this.btnConveyor2ClampCylinderReleaseStatus.TabIndex = 6;
            this.btnConveyor2ClampCylinderReleaseStatus.UseVisualStyleBackColor = false;
            // 
            // btnConveyor2ClampCylinder
            // 
            this.btnConveyor2ClampCylinder.BackColor = System.Drawing.Color.LimeGreen;
            this.btnConveyor2ClampCylinder.Font = new System.Drawing.Font("SimSun", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnConveyor2ClampCylinder.Location = new System.Drawing.Point(286, 122);
            this.btnConveyor2ClampCylinder.Name = "btnConveyor2ClampCylinder";
            this.btnConveyor2ClampCylinder.Size = new System.Drawing.Size(135, 63);
            this.btnConveyor2ClampCylinder.TabIndex = 6;
            this.btnConveyor2ClampCylinder.Text = "传送线2夹紧气缸松开";
            this.btnConveyor2ClampCylinder.UseVisualStyleBackColor = false;
            this.btnConveyor2ClampCylinder.Click += new System.EventHandler(this.CylinderControlBtnEvent);
            // 
            // btnConveyor2ClampCylinderClampStatus
            // 
            this.btnConveyor2ClampCylinderClampStatus.BackColor = System.Drawing.SystemColors.ControlDark;
            this.btnConveyor2ClampCylinderClampStatus.Font = new System.Drawing.Font("SimSun", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnConveyor2ClampCylinderClampStatus.Location = new System.Drawing.Point(423, 122);
            this.btnConveyor2ClampCylinderClampStatus.Name = "btnConveyor2ClampCylinderClampStatus";
            this.btnConveyor2ClampCylinderClampStatus.Size = new System.Drawing.Size(27, 63);
            this.btnConveyor2ClampCylinderClampStatus.TabIndex = 6;
            this.btnConveyor2ClampCylinderClampStatus.UseVisualStyleBackColor = false;
            // 
            // MaintenanceCylinderControlForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1317, 686);
            this.ControlBox = false;
            this.Controls.Add(this.btnConveyor2ClampCylinderClampStatus);
            this.Controls.Add(this.btnConveyor2ClampCylinder);
            this.Controls.Add(this.btnConveyor2StopCylinderDownStatus);
            this.Controls.Add(this.btnConveyor2ClampCylinderReleaseStatus);
            this.Controls.Add(this.btnConveyor2StopCylinder);
            this.Controls.Add(this.btnConveyor2StopCylinderUpStatus);
            this.Controls.Add(this.btnConveyor1ClampCylinderClampStatus);
            this.Controls.Add(this.btnConveyor1ClampCylinder);
            this.Controls.Add(this.btnConveyor1ClampCylinderReleaseStatus);
            this.Controls.Add(this.btnConveyor1StopCylinderUpStatus);
            this.Controls.Add(this.btnConveyor1StopCylinder);
            this.Controls.Add(this.btnConveyor1StopCylinderDownStatus);
            this.Controls.Add(this.btnCameraTransferCylinderOutStatus);
            this.Controls.Add(this.btnCameraTransferCylinderBackStatus);
            this.Controls.Add(this.btnCameraTransferCylinder);
            this.Controls.Add(this.shapeContainer1);
            this.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "MaintenanceCylinderControlForm";
            this.Text = "MaintenanceCylinderControlForm";
            this.Load += new System.EventHandler(this.MaintenanceCylinderControlForm_Load);
            this.VisibleChanged += new System.EventHandler(this.MaintenanceCylinderControlForm_VisibleChanged);
            this.ResumeLayout(false);

        }

        #endregion

        private Microsoft.VisualBasic.PowerPacks.RectangleShape rectangleShape1;
        private Microsoft.VisualBasic.PowerPacks.ShapeContainer shapeContainer1;
        private System.Windows.Forms.Button btnConveyor1StopCylinder;
        private System.Windows.Forms.Button btnConveyor1StopCylinderDownStatus;
        private System.Windows.Forms.Button btnConveyor1StopCylinderUpStatus;
        private System.Windows.Forms.Button btnConveyor1ClampCylinderReleaseStatus;
        private System.Windows.Forms.Button btnConveyor1ClampCylinder;
        private System.Windows.Forms.Button btnConveyor1ClampCylinderClampStatus;
        private System.Windows.Forms.Button btnConveyor2StopCylinderDownStatus;
        private System.Windows.Forms.Button btnConveyor2StopCylinder;
        private System.Windows.Forms.Button btnConveyor2StopCylinderUpStatus;
        private System.Windows.Forms.Button btnConveyor2ClampCylinderReleaseStatus;
        private System.Windows.Forms.Button btnConveyor2ClampCylinder;
        private System.Windows.Forms.Button btnConveyor2ClampCylinderClampStatus;
        public System.Windows.Forms.Button btnCameraTransferCylinder;
        public System.Windows.Forms.Button btnCameraTransferCylinderBackStatus;
        public System.Windows.Forms.Button btnCameraTransferCylinderOutStatus;
    }
}