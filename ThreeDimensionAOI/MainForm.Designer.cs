namespace ThreeDimensionAVI
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.lblInfo = new System.Windows.Forms.Label();
            this.lblProgramVersion = new System.Windows.Forms.Label();
            this.lblUsingTime = new System.Windows.Forms.Label();
            this.picExit = new System.Windows.Forms.PictureBox();
            this.picGotoSettingForm = new System.Windows.Forms.PictureBox();
            this.btnReset = new System.Windows.Forms.Button();
            this.btnStep = new System.Windows.Forms.Button();
            this.btnMeasureStart = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnManual = new System.Windows.Forms.Button();
            this.btnAuto = new System.Windows.Forms.Button();
            this.lblConveyor2BarcodeTitle = new System.Windows.Forms.Label();
            this.lblConveyor2Status = new System.Windows.Forms.Label();
            this.lblConveyor1Status = new System.Windows.Forms.Label();
            this.lblConveyor2Barcode = new System.Windows.Forms.Label();
            this.lblConveyor1BarcodeTitle = new System.Windows.Forms.Label();
            this.lblConveyor1Barcode = new System.Windows.Forms.Label();
            this.lblResultImageTitle = new System.Windows.Forms.Label();
            this.lblSourceImageTitle = new System.Windows.Forms.Label();
            this.lblTrack2ActualProduct = new System.Windows.Forms.Label();
            this.lblTrack1ActualProduct = new System.Windows.Forms.Label();
            this.picRightCameraSourceImageDisp = new HalconDotNet.HWindowControl();
            this.picLeftCameraSourceImageDisp = new HalconDotNet.HWindowControl();
            this.picLeftCameraResultImageDisp = new HalconDotNet.HWindowControl();
            this.picRightCameraResultImageDisp = new HalconDotNet.HWindowControl();
            this.pnlMainFormDisplay = new System.Windows.Forms.Panel();
            this.lblConveyor1ProductForcedMoveOutTitle = new System.Windows.Forms.Label();
            this.lblConveyor2ProductForcedMoveOutTitle = new System.Windows.Forms.Label();
            this.btnConveyor1ProductForcedMoveOut = new UCArrow.UCArrow();
            this.btnConveyor2ProductForcedMoveOut = new UCArrow.UCArrow();
            this.lblTrack1ActualProduct1 = new System.Windows.Forms.Label();
            this.lblTrack2ActualProduct1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.picExit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picGotoSettingForm)).BeginInit();
            this.pnlMainFormDisplay.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblInfo
            // 
            this.lblInfo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblInfo.Font = new System.Drawing.Font("Times New Roman", 219.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblInfo.ForeColor = System.Drawing.Color.Black;
            this.lblInfo.Location = new System.Drawing.Point(268, 3);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(975, 767);
            this.lblInfo.TabIndex = 60;
            this.lblInfo.Text = "By Pass";
            this.lblInfo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblProgramVersion
            // 
            this.lblProgramVersion.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProgramVersion.Location = new System.Drawing.Point(13, 747);
            this.lblProgramVersion.Name = "lblProgramVersion";
            this.lblProgramVersion.Size = new System.Drawing.Size(244, 19);
            this.lblProgramVersion.TabIndex = 45;
            this.lblProgramVersion.Text = "程序版本：V1.00-20220505";
            this.lblProgramVersion.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblUsingTime
            // 
            this.lblUsingTime.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUsingTime.Location = new System.Drawing.Point(13, 717);
            this.lblUsingTime.Name = "lblUsingTime";
            this.lblUsingTime.Size = new System.Drawing.Size(244, 19);
            this.lblUsingTime.TabIndex = 45;
            this.lblUsingTime.Text = "用时：----s";
            this.lblUsingTime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // picExit
            // 
            this.picExit.Image = ((System.Drawing.Image)(resources.GetObject("picExit.Image")));
            this.picExit.InitialImage = null;
            this.picExit.Location = new System.Drawing.Point(1247, 717);
            this.picExit.Name = "picExit";
            this.picExit.Size = new System.Drawing.Size(65, 53);
            this.picExit.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picExit.TabIndex = 51;
            this.picExit.TabStop = false;
            this.picExit.Click += new System.EventHandler(this.picExit_Click);
            // 
            // picGotoSettingForm
            // 
            this.picGotoSettingForm.Image = ((System.Drawing.Image)(resources.GetObject("picGotoSettingForm.Image")));
            this.picGotoSettingForm.InitialImage = null;
            this.picGotoSettingForm.Location = new System.Drawing.Point(1247, 658);
            this.picGotoSettingForm.Name = "picGotoSettingForm";
            this.picGotoSettingForm.Size = new System.Drawing.Size(65, 53);
            this.picGotoSettingForm.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picGotoSettingForm.TabIndex = 52;
            this.picGotoSettingForm.TabStop = false;
            this.picGotoSettingForm.Click += new System.EventHandler(this.picGotoSettingForm_Click);
            // 
            // btnReset
            // 
            this.btnReset.Font = new System.Drawing.Font("Times New Roman", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnReset.Location = new System.Drawing.Point(84, 563);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(101, 62);
            this.btnReset.TabIndex = 58;
            this.btnReset.Text = "复位";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PressVirtualButtonEvent);
            this.btnReset.MouseLeave += new System.EventHandler(this.MouseLeaveVirtualButtonEvent);
            this.btnReset.MouseUp += new System.Windows.Forms.MouseEventHandler(this.MouseUpVirtualButtonEvent);
            // 
            // btnStep
            // 
            this.btnStep.Font = new System.Drawing.Font("Times New Roman", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStep.Location = new System.Drawing.Point(84, 636);
            this.btnStep.Name = "btnStep";
            this.btnStep.Size = new System.Drawing.Size(101, 62);
            this.btnStep.TabIndex = 58;
            this.btnStep.Text = "步进";
            this.btnStep.UseVisualStyleBackColor = true;
            this.btnStep.Click += new System.EventHandler(this.btnStep_Click);
            // 
            // btnMeasureStart
            // 
            this.btnMeasureStart.Font = new System.Drawing.Font("Times New Roman", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMeasureStart.Location = new System.Drawing.Point(678, -13);
            this.btnMeasureStart.Name = "btnMeasureStart";
            this.btnMeasureStart.Size = new System.Drawing.Size(107, 45);
            this.btnMeasureStart.TabIndex = 58;
            this.btnMeasureStart.Text = "检测启动";
            this.btnMeasureStart.UseVisualStyleBackColor = true;
            this.btnMeasureStart.Visible = false;
            this.btnMeasureStart.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PressVirtualButtonEvent);
            this.btnMeasureStart.MouseLeave += new System.EventHandler(this.MouseLeaveVirtualButtonEvent);
            this.btnMeasureStart.MouseUp += new System.Windows.Forms.MouseEventHandler(this.MouseUpVirtualButtonEvent);
            // 
            // btnStop
            // 
            this.btnStop.Font = new System.Drawing.Font("Times New Roman", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStop.Location = new System.Drawing.Point(84, 490);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(101, 62);
            this.btnStop.TabIndex = 58;
            this.btnStop.Text = "停止";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PressVirtualButtonEvent);
            this.btnStop.MouseLeave += new System.EventHandler(this.MouseLeaveVirtualButtonEvent);
            this.btnStop.MouseUp += new System.Windows.Forms.MouseEventHandler(this.MouseUpVirtualButtonEvent);
            // 
            // btnStart
            // 
            this.btnStart.Font = new System.Drawing.Font("Times New Roman", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStart.Location = new System.Drawing.Point(84, 417);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(101, 62);
            this.btnStart.TabIndex = 58;
            this.btnStart.Text = "启动";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PressVirtualButtonEvent);
            this.btnStart.MouseLeave += new System.EventHandler(this.MouseLeaveVirtualButtonEvent);
            this.btnStart.MouseUp += new System.Windows.Forms.MouseEventHandler(this.MouseUpVirtualButtonEvent);
            // 
            // btnManual
            // 
            this.btnManual.Font = new System.Drawing.Font("Times New Roman", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnManual.Location = new System.Drawing.Point(84, 344);
            this.btnManual.Name = "btnManual";
            this.btnManual.Size = new System.Drawing.Size(101, 62);
            this.btnManual.TabIndex = 58;
            this.btnManual.Text = "手动";
            this.btnManual.UseVisualStyleBackColor = true;
            this.btnManual.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PressVirtualButtonEvent);
            this.btnManual.MouseLeave += new System.EventHandler(this.MouseLeaveVirtualButtonEvent);
            this.btnManual.MouseUp += new System.Windows.Forms.MouseEventHandler(this.MouseUpVirtualButtonEvent);
            // 
            // btnAuto
            // 
            this.btnAuto.Font = new System.Drawing.Font("Times New Roman", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAuto.Location = new System.Drawing.Point(84, 271);
            this.btnAuto.Name = "btnAuto";
            this.btnAuto.Size = new System.Drawing.Size(101, 62);
            this.btnAuto.TabIndex = 58;
            this.btnAuto.Text = "自动";
            this.btnAuto.UseVisualStyleBackColor = true;
            this.btnAuto.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PressVirtualButtonEvent);
            this.btnAuto.MouseLeave += new System.EventHandler(this.MouseLeaveVirtualButtonEvent);
            this.btnAuto.MouseUp += new System.Windows.Forms.MouseEventHandler(this.MouseUpVirtualButtonEvent);
            // 
            // lblConveyor2BarcodeTitle
            // 
            this.lblConveyor2BarcodeTitle.Font = new System.Drawing.Font("Times New Roman", 14.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblConveyor2BarcodeTitle.Location = new System.Drawing.Point(6, 162);
            this.lblConveyor2BarcodeTitle.Name = "lblConveyor2BarcodeTitle";
            this.lblConveyor2BarcodeTitle.Size = new System.Drawing.Size(238, 22);
            this.lblConveyor2BarcodeTitle.TabIndex = 56;
            this.lblConveyor2BarcodeTitle.Text = "传送线2条形码：";
            // 
            // lblConveyor2Status
            // 
            this.lblConveyor2Status.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblConveyor2Status.ForeColor = System.Drawing.Color.LimeGreen;
            this.lblConveyor2Status.Location = new System.Drawing.Point(6, 56);
            this.lblConveyor2Status.Name = "lblConveyor2Status";
            this.lblConveyor2Status.Size = new System.Drawing.Size(238, 22);
            this.lblConveyor2Status.TabIndex = 56;
            this.lblConveyor2Status.Text = "传送线2状态: 正常运行";
            this.lblConveyor2Status.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblConveyor1Status
            // 
            this.lblConveyor1Status.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblConveyor1Status.ForeColor = System.Drawing.Color.Red;
            this.lblConveyor1Status.Location = new System.Drawing.Point(6, 132);
            this.lblConveyor1Status.Name = "lblConveyor1Status";
            this.lblConveyor1Status.Size = new System.Drawing.Size(238, 22);
            this.lblConveyor1Status.TabIndex = 56;
            this.lblConveyor1Status.Text = "传送线1状态：By Pass";
            this.lblConveyor1Status.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblConveyor2Barcode
            // 
            this.lblConveyor2Barcode.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblConveyor2Barcode.Location = new System.Drawing.Point(6, 190);
            this.lblConveyor2Barcode.Name = "lblConveyor2Barcode";
            this.lblConveyor2Barcode.Size = new System.Drawing.Size(238, 22);
            this.lblConveyor2Barcode.TabIndex = 56;
            this.lblConveyor2Barcode.Text = "---------";
            this.lblConveyor2Barcode.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblConveyor1BarcodeTitle
            // 
            this.lblConveyor1BarcodeTitle.Font = new System.Drawing.Font("Times New Roman", 14.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblConveyor1BarcodeTitle.Location = new System.Drawing.Point(6, 216);
            this.lblConveyor1BarcodeTitle.Name = "lblConveyor1BarcodeTitle";
            this.lblConveyor1BarcodeTitle.Size = new System.Drawing.Size(238, 22);
            this.lblConveyor1BarcodeTitle.TabIndex = 56;
            this.lblConveyor1BarcodeTitle.Text = "传送线1条形码：";
            // 
            // lblConveyor1Barcode
            // 
            this.lblConveyor1Barcode.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblConveyor1Barcode.Location = new System.Drawing.Point(6, 246);
            this.lblConveyor1Barcode.Name = "lblConveyor1Barcode";
            this.lblConveyor1Barcode.Size = new System.Drawing.Size(238, 22);
            this.lblConveyor1Barcode.TabIndex = 56;
            this.lblConveyor1Barcode.Text = "---------";
            this.lblConveyor1Barcode.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblResultImageTitle
            // 
            this.lblResultImageTitle.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblResultImageTitle.ForeColor = System.Drawing.Color.Black;
            this.lblResultImageTitle.Location = new System.Drawing.Point(1264, 512);
            this.lblResultImageTitle.Name = "lblResultImageTitle";
            this.lblResultImageTitle.Size = new System.Drawing.Size(29, 132);
            this.lblResultImageTitle.TabIndex = 56;
            this.lblResultImageTitle.Text = "检测结果图像";
            this.lblResultImageTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblSourceImageTitle
            // 
            this.lblSourceImageTitle.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSourceImageTitle.ForeColor = System.Drawing.Color.Black;
            this.lblSourceImageTitle.Location = new System.Drawing.Point(1264, 150);
            this.lblSourceImageTitle.Name = "lblSourceImageTitle";
            this.lblSourceImageTitle.Size = new System.Drawing.Size(29, 93);
            this.lblSourceImageTitle.TabIndex = 56;
            this.lblSourceImageTitle.Text = "原始图像";
            this.lblSourceImageTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblTrack2ActualProduct
            // 
            this.lblTrack2ActualProduct.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTrack2ActualProduct.Location = new System.Drawing.Point(6, 6);
            this.lblTrack2ActualProduct.Name = "lblTrack2ActualProduct";
            this.lblTrack2ActualProduct.Size = new System.Drawing.Size(238, 22);
            this.lblTrack2ActualProduct.TabIndex = 12;
            this.lblTrack2ActualProduct.Text = "传送线2当前产品：";
            this.lblTrack2ActualProduct.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblTrack2ActualProduct.UseMnemonic = false;
            this.lblTrack2ActualProduct.DoubleClick += new System.EventHandler(this.ProductChangeEvent);
            // 
            // lblTrack1ActualProduct
            // 
            this.lblTrack1ActualProduct.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTrack1ActualProduct.Location = new System.Drawing.Point(6, 84);
            this.lblTrack1ActualProduct.Name = "lblTrack1ActualProduct";
            this.lblTrack1ActualProduct.Size = new System.Drawing.Size(238, 22);
            this.lblTrack1ActualProduct.TabIndex = 12;
            this.lblTrack1ActualProduct.Text = "传送线1当前产品：";
            this.lblTrack1ActualProduct.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblTrack1ActualProduct.UseMnemonic = false;
            this.lblTrack1ActualProduct.DoubleClick += new System.EventHandler(this.ProductChangeEvent);
            // 
            // picRightCameraSourceImageDisp
            // 
            this.picRightCameraSourceImageDisp.BackColor = System.Drawing.SystemColors.ControlDark;
            this.picRightCameraSourceImageDisp.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.picRightCameraSourceImageDisp.ImagePart = new System.Drawing.Rectangle(0, 0, 640, 480);
            this.picRightCameraSourceImageDisp.Location = new System.Drawing.Point(758, 3);
            this.picRightCameraSourceImageDisp.Name = "picRightCameraSourceImageDisp";
            this.picRightCameraSourceImageDisp.Size = new System.Drawing.Size(485, 380);
            this.picRightCameraSourceImageDisp.TabIndex = 59;
            this.picRightCameraSourceImageDisp.WindowSize = new System.Drawing.Size(485, 380);
            // 
            // picLeftCameraSourceImageDisp
            // 
            this.picLeftCameraSourceImageDisp.BackColor = System.Drawing.SystemColors.ControlDark;
            this.picLeftCameraSourceImageDisp.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.picLeftCameraSourceImageDisp.ImagePart = new System.Drawing.Rectangle(0, 0, 640, 480);
            this.picLeftCameraSourceImageDisp.Location = new System.Drawing.Point(266, 3);
            this.picLeftCameraSourceImageDisp.Name = "picLeftCameraSourceImageDisp";
            this.picLeftCameraSourceImageDisp.Size = new System.Drawing.Size(485, 380);
            this.picLeftCameraSourceImageDisp.TabIndex = 59;
            this.picLeftCameraSourceImageDisp.WindowSize = new System.Drawing.Size(485, 380);
            // 
            // picLeftCameraResultImageDisp
            // 
            this.picLeftCameraResultImageDisp.BackColor = System.Drawing.SystemColors.ControlDark;
            this.picLeftCameraResultImageDisp.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.picLeftCameraResultImageDisp.ImagePart = new System.Drawing.Rectangle(0, 0, 640, 480);
            this.picLeftCameraResultImageDisp.Location = new System.Drawing.Point(266, 390);
            this.picLeftCameraResultImageDisp.Name = "picLeftCameraResultImageDisp";
            this.picLeftCameraResultImageDisp.Size = new System.Drawing.Size(485, 380);
            this.picLeftCameraResultImageDisp.TabIndex = 59;
            this.picLeftCameraResultImageDisp.WindowSize = new System.Drawing.Size(485, 380);
            // 
            // picRightCameraResultImageDisp
            // 
            this.picRightCameraResultImageDisp.BackColor = System.Drawing.SystemColors.ControlDark;
            this.picRightCameraResultImageDisp.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.picRightCameraResultImageDisp.ImagePart = new System.Drawing.Rectangle(0, 0, 640, 480);
            this.picRightCameraResultImageDisp.Location = new System.Drawing.Point(758, 390);
            this.picRightCameraResultImageDisp.Name = "picRightCameraResultImageDisp";
            this.picRightCameraResultImageDisp.Size = new System.Drawing.Size(485, 380);
            this.picRightCameraResultImageDisp.TabIndex = 59;
            this.picRightCameraResultImageDisp.WindowSize = new System.Drawing.Size(485, 380);
            // 
            // pnlMainFormDisplay
            // 
            this.pnlMainFormDisplay.Controls.Add(this.lblConveyor1ProductForcedMoveOutTitle);
            this.pnlMainFormDisplay.Controls.Add(this.lblConveyor2ProductForcedMoveOutTitle);
            this.pnlMainFormDisplay.Controls.Add(this.btnConveyor1ProductForcedMoveOut);
            this.pnlMainFormDisplay.Controls.Add(this.btnConveyor2ProductForcedMoveOut);
            this.pnlMainFormDisplay.Controls.Add(this.picRightCameraResultImageDisp);
            this.pnlMainFormDisplay.Controls.Add(this.picLeftCameraResultImageDisp);
            this.pnlMainFormDisplay.Controls.Add(this.picLeftCameraSourceImageDisp);
            this.pnlMainFormDisplay.Controls.Add(this.picRightCameraSourceImageDisp);
            this.pnlMainFormDisplay.Controls.Add(this.lblTrack1ActualProduct1);
            this.pnlMainFormDisplay.Controls.Add(this.lblTrack1ActualProduct);
            this.pnlMainFormDisplay.Controls.Add(this.lblTrack2ActualProduct1);
            this.pnlMainFormDisplay.Controls.Add(this.lblTrack2ActualProduct);
            this.pnlMainFormDisplay.Controls.Add(this.lblSourceImageTitle);
            this.pnlMainFormDisplay.Controls.Add(this.lblResultImageTitle);
            this.pnlMainFormDisplay.Controls.Add(this.lblConveyor1Barcode);
            this.pnlMainFormDisplay.Controls.Add(this.lblConveyor1BarcodeTitle);
            this.pnlMainFormDisplay.Controls.Add(this.lblConveyor2Barcode);
            this.pnlMainFormDisplay.Controls.Add(this.lblConveyor1Status);
            this.pnlMainFormDisplay.Controls.Add(this.lblConveyor2Status);
            this.pnlMainFormDisplay.Controls.Add(this.lblConveyor2BarcodeTitle);
            this.pnlMainFormDisplay.Controls.Add(this.btnAuto);
            this.pnlMainFormDisplay.Controls.Add(this.btnManual);
            this.pnlMainFormDisplay.Controls.Add(this.btnStart);
            this.pnlMainFormDisplay.Controls.Add(this.btnStop);
            this.pnlMainFormDisplay.Controls.Add(this.btnMeasureStart);
            this.pnlMainFormDisplay.Controls.Add(this.btnStep);
            this.pnlMainFormDisplay.Controls.Add(this.btnReset);
            this.pnlMainFormDisplay.Controls.Add(this.picGotoSettingForm);
            this.pnlMainFormDisplay.Controls.Add(this.picExit);
            this.pnlMainFormDisplay.Controls.Add(this.lblUsingTime);
            this.pnlMainFormDisplay.Controls.Add(this.lblProgramVersion);
            this.pnlMainFormDisplay.Controls.Add(this.lblInfo);
            this.pnlMainFormDisplay.Location = new System.Drawing.Point(-1, 2);
            this.pnlMainFormDisplay.Name = "pnlMainFormDisplay";
            this.pnlMainFormDisplay.Size = new System.Drawing.Size(1315, 775);
            this.pnlMainFormDisplay.TabIndex = 12;
            // 
            // lblConveyor1ProductForcedMoveOutTitle
            // 
            this.lblConveyor1ProductForcedMoveOutTitle.Location = new System.Drawing.Point(1247, 110);
            this.lblConveyor1ProductForcedMoveOutTitle.Name = "lblConveyor1ProductForcedMoveOutTitle";
            this.lblConveyor1ProductForcedMoveOutTitle.Size = new System.Drawing.Size(65, 20);
            this.lblConveyor1ProductForcedMoveOutTitle.TabIndex = 62;
            this.lblConveyor1ProductForcedMoveOutTitle.Text = "轨1流出";
            this.lblConveyor1ProductForcedMoveOutTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblConveyor2ProductForcedMoveOutTitle
            // 
            this.lblConveyor2ProductForcedMoveOutTitle.Location = new System.Drawing.Point(1247, 41);
            this.lblConveyor2ProductForcedMoveOutTitle.Name = "lblConveyor2ProductForcedMoveOutTitle";
            this.lblConveyor2ProductForcedMoveOutTitle.Size = new System.Drawing.Size(65, 20);
            this.lblConveyor2ProductForcedMoveOutTitle.TabIndex = 62;
            this.lblConveyor2ProductForcedMoveOutTitle.Text = "轨2流出";
            this.lblConveyor2ProductForcedMoveOutTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnConveyor1ProductForcedMoveOut
            // 
            this.btnConveyor1ProductForcedMoveOut.ArrowColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.btnConveyor1ProductForcedMoveOut.BorderColor = null;
            this.btnConveyor1ProductForcedMoveOut.Direction = UCArrow.ArrowDirection.Right;
            this.btnConveyor1ProductForcedMoveOut.Font = new System.Drawing.Font("Times New Roman", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnConveyor1ProductForcedMoveOut.ForeColor = System.Drawing.Color.Black;
            this.btnConveyor1ProductForcedMoveOut.Location = new System.Drawing.Point(1247, 74);
            this.btnConveyor1ProductForcedMoveOut.Name = "btnConveyor1ProductForcedMoveOut";
            this.btnConveyor1ProductForcedMoveOut.Size = new System.Drawing.Size(65, 33);
            this.btnConveyor1ProductForcedMoveOut.TabIndex = 61;
            this.btnConveyor1ProductForcedMoveOut.Text = null;
            this.btnConveyor1ProductForcedMoveOut.Click += new System.EventHandler(this.ConveyorForcedMoveOutClickEvent);
            // 
            // btnConveyor2ProductForcedMoveOut
            // 
            this.btnConveyor2ProductForcedMoveOut.ArrowColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.btnConveyor2ProductForcedMoveOut.BorderColor = null;
            this.btnConveyor2ProductForcedMoveOut.Direction = UCArrow.ArrowDirection.Right;
            this.btnConveyor2ProductForcedMoveOut.Font = new System.Drawing.Font("Times New Roman", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnConveyor2ProductForcedMoveOut.ForeColor = System.Drawing.Color.Black;
            this.btnConveyor2ProductForcedMoveOut.Location = new System.Drawing.Point(1247, 6);
            this.btnConveyor2ProductForcedMoveOut.Name = "btnConveyor2ProductForcedMoveOut";
            this.btnConveyor2ProductForcedMoveOut.Size = new System.Drawing.Size(65, 33);
            this.btnConveyor2ProductForcedMoveOut.TabIndex = 61;
            this.btnConveyor2ProductForcedMoveOut.Text = null;
            this.btnConveyor2ProductForcedMoveOut.Click += new System.EventHandler(this.ConveyorForcedMoveOutClickEvent);
            // 
            // lblTrack1ActualProduct1
            // 
            this.lblTrack1ActualProduct1.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTrack1ActualProduct1.Location = new System.Drawing.Point(6, 108);
            this.lblTrack1ActualProduct1.Name = "lblTrack1ActualProduct1";
            this.lblTrack1ActualProduct1.Size = new System.Drawing.Size(238, 22);
            this.lblTrack1ActualProduct1.TabIndex = 12;
            this.lblTrack1ActualProduct1.Text = "Rhino";
            this.lblTrack1ActualProduct1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblTrack1ActualProduct1.UseMnemonic = false;
            this.lblTrack1ActualProduct1.DoubleClick += new System.EventHandler(this.ProductChangeEvent);
            // 
            // lblTrack2ActualProduct1
            // 
            this.lblTrack2ActualProduct1.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTrack2ActualProduct1.Location = new System.Drawing.Point(6, 31);
            this.lblTrack2ActualProduct1.Name = "lblTrack2ActualProduct1";
            this.lblTrack2ActualProduct1.Size = new System.Drawing.Size(238, 22);
            this.lblTrack2ActualProduct1.TabIndex = 12;
            this.lblTrack2ActualProduct1.Text = "Rhino";
            this.lblTrack2ActualProduct1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblTrack2ActualProduct1.UseMnemonic = false;
            this.lblTrack2ActualProduct1.DoubleClick += new System.EventHandler(this.ProductChangeEvent);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1317, 777);
            this.ControlBox = false;
            this.Controls.Add(this.pnlMainFormDisplay);
            this.Font = new System.Drawing.Font("Times New Roman", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "MainForm";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.VisibleChanged += new System.EventHandler(this.MainForm_VisibleChanged);
            ((System.ComponentModel.ISupportInitialize)(this.picExit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picGotoSettingForm)).EndInit();
            this.pnlMainFormDisplay.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.Label lblInfo;
        public System.Windows.Forms.Label lblProgramVersion;
        public System.Windows.Forms.Label lblUsingTime;
        private System.Windows.Forms.PictureBox picExit;
        private System.Windows.Forms.PictureBox picGotoSettingForm;
        public System.Windows.Forms.Button btnReset;
        public System.Windows.Forms.Button btnStep;
        public System.Windows.Forms.Button btnMeasureStart;
        public System.Windows.Forms.Button btnStop;
        public System.Windows.Forms.Button btnStart;
        public System.Windows.Forms.Button btnManual;
        public System.Windows.Forms.Button btnAuto;
        private System.Windows.Forms.Label lblConveyor2BarcodeTitle;
        public System.Windows.Forms.Label lblConveyor2Status;
        public System.Windows.Forms.Label lblConveyor1Status;
        public System.Windows.Forms.Label lblConveyor2Barcode;
        private System.Windows.Forms.Label lblConveyor1BarcodeTitle;
        public System.Windows.Forms.Label lblConveyor1Barcode;
        public System.Windows.Forms.Label lblResultImageTitle;
        public System.Windows.Forms.Label lblSourceImageTitle;
        public System.Windows.Forms.Label lblTrack2ActualProduct;
        public System.Windows.Forms.Label lblTrack1ActualProduct;
        public HalconDotNet.HWindowControl picRightCameraSourceImageDisp;
        public HalconDotNet.HWindowControl picLeftCameraSourceImageDisp;
        public HalconDotNet.HWindowControl picLeftCameraResultImageDisp;
        public HalconDotNet.HWindowControl picRightCameraResultImageDisp;
        private System.Windows.Forms.Panel pnlMainFormDisplay;
        public UCArrow.UCArrow btnConveyor2ProductForcedMoveOut;
        public UCArrow.UCArrow btnConveyor1ProductForcedMoveOut;
        public System.Windows.Forms.Label lblConveyor1ProductForcedMoveOutTitle;
        public System.Windows.Forms.Label lblConveyor2ProductForcedMoveOutTitle;
        public System.Windows.Forms.Label lblTrack1ActualProduct1;
        public System.Windows.Forms.Label lblTrack2ActualProduct1;

    }
}