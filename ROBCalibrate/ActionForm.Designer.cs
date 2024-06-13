namespace ROBCalibrate
{
    partial class ActionForm
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

            #region 停止线程
            if (ROBDataExchangeThread != null)
            {
                ROBDataExchangeThread.Abort();
                ROBDataExchangeThread = null;
            }
            if (_cameraImageGrabAndDispThread != null)
            {
                _cameraImageGrabAndDispThread.Abort();
                _cameraImageGrabAndDispThread = null;
            }
            #endregion

            newSocketServerForROB.socketServerClose();

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ActionForm));
            this.panel1 = new System.Windows.Forms.Panel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.btnDebug_S3 = new System.Windows.Forms.Button();
            this.btnDebug_S2 = new System.Windows.Forms.Button();
            this.btnSocketServerClose = new System.Windows.Forms.Button();
            this.lblActualProductSelect = new System.Windows.Forms.Label();
            this.lblAccessLevelTitle = new System.Windows.Forms.Label();
            this.lblActualProduct = new System.Windows.Forms.Label();
            this.btnSocketServerStart = new System.Windows.Forms.Button();
            this.lblAccessLevel = new System.Windows.Forms.Label();
            this.lblUserName = new System.Windows.Forms.Label();
            this.lblConnectStatus = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.btnDebug_S1 = new System.Windows.Forms.Button();
            this.picGotoSettingForm = new System.Windows.Forms.PictureBox();
            this.tctlDeviceStatusRecord = new System.Windows.Forms.TabControl();
            this.tpageDeviceControlLOG = new System.Windows.Forms.TabPage();
            this.lvDeviceControlLogDisp = new System.Windows.Forms.ListView();
            this.Info = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tpageMESLOG = new System.Windows.Forms.TabPage();
            this.lvMESLogDisp = new System.Windows.Forms.ListView();
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tpageAlarmRecord = new System.Windows.Forms.TabPage();
            this.lvAlarmDisp = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tpageDeviceStatus = new System.Windows.Forms.TabPage();
            this.lvDeviceStatus = new System.Windows.Forms.ListView();
            this.VariateName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.VariateData = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picGotoSettingForm)).BeginInit();
            this.tctlDeviceStatusRecord.SuspendLayout();
            this.tpageDeviceControlLOG.SuspendLayout();
            this.tpageMESLOG.SuspendLayout();
            this.tpageAlarmRecord.SuspendLayout();
            this.tpageDeviceStatus.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.tableLayoutPanel2.SetColumnSpan(this.panel1, 12);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 103);
            this.panel1.Name = "panel1";
            this.tableLayoutPanel2.SetRowSpan(this.panel1, 20);
            this.panel1.Size = new System.Drawing.Size(474, 500);
            this.panel1.TabIndex = 453;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tableLayoutPanel2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tctlDeviceStatusRecord);
            this.splitContainer1.Panel2.Controls.Add(this.tableLayoutPanel1);
            this.splitContainer1.Size = new System.Drawing.Size(1472, 823);
            this.splitContainer1.SplitterDistance = 606;
            this.splitContainer1.TabIndex = 454;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 36;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 2.777777F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 2.777777F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 2.777777F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 2.777777F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 2.777777F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 2.777777F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 2.777777F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 2.777777F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 2.777777F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 2.777777F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 2.777777F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 2.777777F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 2.777777F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 2.777777F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 2.777777F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 2.777777F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 2.777777F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 2.777777F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 2.777777F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 2.777777F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 2.777777F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 2.777777F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 2.777777F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 2.777777F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 2.777777F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 2.777777F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 2.777777F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 2.777777F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 2.777777F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 2.777777F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 2.777777F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 2.777777F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 2.777777F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 2.777777F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 2.777777F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 2.777777F));
            this.tableLayoutPanel2.Controls.Add(this.btnDebug_S3, 18, 2);
            this.tableLayoutPanel2.Controls.Add(this.btnDebug_S2, 16, 2);
            this.tableLayoutPanel2.Controls.Add(this.btnSocketServerClose, 7, 2);
            this.tableLayoutPanel2.Controls.Add(this.panel1, 0, 4);
            this.tableLayoutPanel2.Controls.Add(this.lblActualProductSelect, 4, 0);
            this.tableLayoutPanel2.Controls.Add(this.lblAccessLevelTitle, 27, 0);
            this.tableLayoutPanel2.Controls.Add(this.lblActualProduct, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.btnSocketServerStart, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.lblAccessLevel, 30, 0);
            this.tableLayoutPanel2.Controls.Add(this.lblUserName, 30, 1);
            this.tableLayoutPanel2.Controls.Add(this.lblConnectStatus, 21, 2);
            this.tableLayoutPanel2.Controls.Add(this.panel2, 12, 4);
            this.tableLayoutPanel2.Controls.Add(this.panel3, 24, 4);
            this.tableLayoutPanel2.Controls.Add(this.btnDebug_S1, 14, 2);
            this.tableLayoutPanel2.Controls.Add(this.picGotoSettingForm, 33, 2);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 24;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 4.166667F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 4.166667F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 4.166667F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 4.166667F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 4.166667F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 4.166667F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 4.166667F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 4.166667F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 4.166667F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 4.166667F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 4.166667F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 4.166667F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 4.166667F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 4.166667F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 4.166667F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 4.166667F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 4.166667F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 4.166667F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 4.166667F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 4.166667F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 4.166667F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 4.166667F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 4.166667F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 4.166667F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(1472, 606);
            this.tableLayoutPanel2.TabIndex = 454;
            // 
            // btnDebug_S3
            // 
            this.btnDebug_S3.BackColor = System.Drawing.SystemColors.ControlDark;
            this.tableLayoutPanel2.SetColumnSpan(this.btnDebug_S3, 2);
            this.btnDebug_S3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnDebug_S3.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDebug_S3.ForeColor = System.Drawing.Color.White;
            this.btnDebug_S3.Location = new System.Drawing.Point(723, 53);
            this.btnDebug_S3.Name = "btnDebug_S3";
            this.tableLayoutPanel2.SetRowSpan(this.btnDebug_S3, 2);
            this.btnDebug_S3.Size = new System.Drawing.Size(74, 44);
            this.btnDebug_S3.TabIndex = 457;
            this.btnDebug_S3.Tag = "";
            this.btnDebug_S3.Text = "Debug_S3";
            this.btnDebug_S3.UseVisualStyleBackColor = false;
            this.btnDebug_S3.Visible = false;
            this.btnDebug_S3.Click += new System.EventHandler(this.btnDebug_S3_Click);
            // 
            // btnDebug_S2
            // 
            this.btnDebug_S2.BackColor = System.Drawing.SystemColors.ControlDark;
            this.tableLayoutPanel2.SetColumnSpan(this.btnDebug_S2, 2);
            this.btnDebug_S2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnDebug_S2.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDebug_S2.ForeColor = System.Drawing.Color.White;
            this.btnDebug_S2.Location = new System.Drawing.Point(643, 53);
            this.btnDebug_S2.Name = "btnDebug_S2";
            this.tableLayoutPanel2.SetRowSpan(this.btnDebug_S2, 2);
            this.btnDebug_S2.Size = new System.Drawing.Size(74, 44);
            this.btnDebug_S2.TabIndex = 456;
            this.btnDebug_S2.Tag = "";
            this.btnDebug_S2.Text = "Debug_S2";
            this.btnDebug_S2.UseVisualStyleBackColor = false;
            this.btnDebug_S2.Visible = false;
            this.btnDebug_S2.Click += new System.EventHandler(this.btnDebug_S2_Click);
            // 
            // btnSocketServerClose
            // 
            this.btnSocketServerClose.BackColor = System.Drawing.SystemColors.ControlDark;
            this.tableLayoutPanel2.SetColumnSpan(this.btnSocketServerClose, 6);
            this.btnSocketServerClose.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnSocketServerClose.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSocketServerClose.ForeColor = System.Drawing.Color.White;
            this.btnSocketServerClose.Location = new System.Drawing.Point(283, 53);
            this.btnSocketServerClose.Name = "btnSocketServerClose";
            this.tableLayoutPanel2.SetRowSpan(this.btnSocketServerClose, 2);
            this.btnSocketServerClose.Size = new System.Drawing.Size(234, 44);
            this.btnSocketServerClose.TabIndex = 451;
            this.btnSocketServerClose.Tag = "";
            this.btnSocketServerClose.Text = "关闭服务器";
            this.btnSocketServerClose.UseVisualStyleBackColor = false;
            this.btnSocketServerClose.Click += new System.EventHandler(this.btnSocketServerClose_Click);
            // 
            // lblActualProductSelect
            // 
            this.tableLayoutPanel2.SetColumnSpan(this.lblActualProductSelect, 20);
            this.lblActualProductSelect.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblActualProductSelect.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Bold);
            this.lblActualProductSelect.Location = new System.Drawing.Point(163, 0);
            this.lblActualProductSelect.Name = "lblActualProductSelect";
            this.tableLayoutPanel2.SetRowSpan(this.lblActualProductSelect, 2);
            this.lblActualProductSelect.Size = new System.Drawing.Size(794, 50);
            this.lblActualProductSelect.TabIndex = 357;
            this.lblActualProductSelect.Text = "TanMo";
            this.lblActualProductSelect.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblActualProductSelect.UseMnemonic = false;
            this.lblActualProductSelect.DoubleClick += new System.EventHandler(this.ProductChangeEvent);
            // 
            // lblAccessLevelTitle
            // 
            this.lblAccessLevelTitle.BackColor = System.Drawing.SystemColors.Control;
            this.lblAccessLevelTitle.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tableLayoutPanel2.SetColumnSpan(this.lblAccessLevelTitle, 4);
            this.lblAccessLevelTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblAccessLevelTitle.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAccessLevelTitle.ForeColor = System.Drawing.Color.Black;
            this.lblAccessLevelTitle.Image = ((System.Drawing.Image)(resources.GetObject("lblAccessLevelTitle.Image")));
            this.lblAccessLevelTitle.Location = new System.Drawing.Point(1083, 0);
            this.lblAccessLevelTitle.Name = "lblAccessLevelTitle";
            this.tableLayoutPanel2.SetRowSpan(this.lblAccessLevelTitle, 2);
            this.lblAccessLevelTitle.Size = new System.Drawing.Size(154, 50);
            this.lblAccessLevelTitle.TabIndex = 359;
            this.lblAccessLevelTitle.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblAccessLevelTitle.Click += new System.EventHandler(this.lblAccessLevel_Click);
            // 
            // lblActualProduct
            // 
            this.tableLayoutPanel2.SetColumnSpan(this.lblActualProduct, 4);
            this.lblActualProduct.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblActualProduct.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Bold);
            this.lblActualProduct.Location = new System.Drawing.Point(3, 0);
            this.lblActualProduct.Name = "lblActualProduct";
            this.tableLayoutPanel2.SetRowSpan(this.lblActualProduct, 2);
            this.lblActualProduct.Size = new System.Drawing.Size(154, 50);
            this.lblActualProduct.TabIndex = 358;
            this.lblActualProduct.Text = "当前产品：";
            this.lblActualProduct.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblActualProduct.UseMnemonic = false;
            this.lblActualProduct.DoubleClick += new System.EventHandler(this.ProductChangeEvent);
            // 
            // btnSocketServerStart
            // 
            this.btnSocketServerStart.BackColor = System.Drawing.SystemColors.ControlDark;
            this.tableLayoutPanel2.SetColumnSpan(this.btnSocketServerStart, 6);
            this.btnSocketServerStart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnSocketServerStart.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSocketServerStart.ForeColor = System.Drawing.Color.White;
            this.btnSocketServerStart.Location = new System.Drawing.Point(3, 53);
            this.btnSocketServerStart.Name = "btnSocketServerStart";
            this.tableLayoutPanel2.SetRowSpan(this.btnSocketServerStart, 2);
            this.btnSocketServerStart.Size = new System.Drawing.Size(234, 44);
            this.btnSocketServerStart.TabIndex = 450;
            this.btnSocketServerStart.Tag = "";
            this.btnSocketServerStart.Text = "开启服务器";
            this.btnSocketServerStart.UseVisualStyleBackColor = false;
            this.btnSocketServerStart.Click += new System.EventHandler(this.btnSocketServerStart_Click);
            // 
            // lblAccessLevel
            // 
            this.lblAccessLevel.BackColor = System.Drawing.SystemColors.Control;
            this.lblAccessLevel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tableLayoutPanel2.SetColumnSpan(this.lblAccessLevel, 5);
            this.lblAccessLevel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblAccessLevel.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAccessLevel.ForeColor = System.Drawing.Color.Black;
            this.lblAccessLevel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblAccessLevel.Location = new System.Drawing.Point(1243, 0);
            this.lblAccessLevel.Name = "lblAccessLevel";
            this.lblAccessLevel.Size = new System.Drawing.Size(226, 25);
            this.lblAccessLevel.TabIndex = 361;
            this.lblAccessLevel.Text = "操作员";
            this.lblAccessLevel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.lblAccessLevel.Click += new System.EventHandler(this.lblAccessLevel_Click);
            // 
            // lblUserName
            // 
            this.lblUserName.BackColor = System.Drawing.SystemColors.Control;
            this.tableLayoutPanel2.SetColumnSpan(this.lblUserName, 5);
            this.lblUserName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblUserName.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUserName.ForeColor = System.Drawing.Color.Black;
            this.lblUserName.Location = new System.Drawing.Point(1243, 25);
            this.lblUserName.Name = "lblUserName";
            this.lblUserName.Size = new System.Drawing.Size(226, 25);
            this.lblUserName.TabIndex = 360;
            this.lblUserName.Text = "TanMo";
            this.lblUserName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblUserName.Click += new System.EventHandler(this.lblAccessLevel_Click);
            // 
            // lblConnectStatus
            // 
            this.lblConnectStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tableLayoutPanel2.SetColumnSpan(this.lblConnectStatus, 6);
            this.lblConnectStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblConnectStatus.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblConnectStatus.Location = new System.Drawing.Point(843, 50);
            this.lblConnectStatus.Name = "lblConnectStatus";
            this.tableLayoutPanel2.SetRowSpan(this.lblConnectStatus, 2);
            this.lblConnectStatus.Size = new System.Drawing.Size(234, 50);
            this.lblConnectStatus.TabIndex = 356;
            this.lblConnectStatus.Text = "未连接ROB";
            this.lblConnectStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel2
            // 
            this.tableLayoutPanel2.SetColumnSpan(this.panel2, 12);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(483, 103);
            this.panel2.Name = "panel2";
            this.tableLayoutPanel2.SetRowSpan(this.panel2, 20);
            this.panel2.Size = new System.Drawing.Size(474, 500);
            this.panel2.TabIndex = 454;
            // 
            // panel3
            // 
            this.tableLayoutPanel2.SetColumnSpan(this.panel3, 12);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(963, 103);
            this.panel3.Name = "panel3";
            this.tableLayoutPanel2.SetRowSpan(this.panel3, 20);
            this.panel3.Size = new System.Drawing.Size(506, 500);
            this.panel3.TabIndex = 455;
            // 
            // btnDebug_S1
            // 
            this.btnDebug_S1.BackColor = System.Drawing.SystemColors.ControlDark;
            this.tableLayoutPanel2.SetColumnSpan(this.btnDebug_S1, 2);
            this.btnDebug_S1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnDebug_S1.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDebug_S1.ForeColor = System.Drawing.Color.White;
            this.btnDebug_S1.Location = new System.Drawing.Point(563, 53);
            this.btnDebug_S1.Name = "btnDebug_S1";
            this.tableLayoutPanel2.SetRowSpan(this.btnDebug_S1, 2);
            this.btnDebug_S1.Size = new System.Drawing.Size(74, 44);
            this.btnDebug_S1.TabIndex = 401;
            this.btnDebug_S1.Tag = "";
            this.btnDebug_S1.Text = "Debug_S1";
            this.btnDebug_S1.UseVisualStyleBackColor = false;
            this.btnDebug_S1.Visible = false;
            this.btnDebug_S1.Click += new System.EventHandler(this.btnDebug_S1_Click);
            // 
            // picGotoSettingForm
            // 
            this.tableLayoutPanel2.SetColumnSpan(this.picGotoSettingForm, 3);
            this.picGotoSettingForm.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picGotoSettingForm.Image = ((System.Drawing.Image)(resources.GetObject("picGotoSettingForm.Image")));
            this.picGotoSettingForm.InitialImage = null;
            this.picGotoSettingForm.Location = new System.Drawing.Point(1323, 53);
            this.picGotoSettingForm.Name = "picGotoSettingForm";
            this.tableLayoutPanel2.SetRowSpan(this.picGotoSettingForm, 2);
            this.picGotoSettingForm.Size = new System.Drawing.Size(146, 44);
            this.picGotoSettingForm.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picGotoSettingForm.TabIndex = 355;
            this.picGotoSettingForm.TabStop = false;
            this.picGotoSettingForm.Click += new System.EventHandler(this.picGotoSettingForm_Click);
            // 
            // tctlDeviceStatusRecord
            // 
            this.tctlDeviceStatusRecord.Controls.Add(this.tpageDeviceControlLOG);
            this.tctlDeviceStatusRecord.Controls.Add(this.tpageMESLOG);
            this.tctlDeviceStatusRecord.Controls.Add(this.tpageAlarmRecord);
            this.tctlDeviceStatusRecord.Controls.Add(this.tpageDeviceStatus);
            this.tctlDeviceStatusRecord.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tctlDeviceStatusRecord.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tctlDeviceStatusRecord.Location = new System.Drawing.Point(0, 0);
            this.tctlDeviceStatusRecord.Name = "tctlDeviceStatusRecord";
            this.tctlDeviceStatusRecord.SelectedIndex = 0;
            this.tctlDeviceStatusRecord.Size = new System.Drawing.Size(1472, 213);
            this.tctlDeviceStatusRecord.TabIndex = 454;
            // 
            // tpageDeviceControlLOG
            // 
            this.tpageDeviceControlLOG.BackColor = System.Drawing.SystemColors.Control;
            this.tpageDeviceControlLOG.Controls.Add(this.lvDeviceControlLogDisp);
            this.tpageDeviceControlLOG.Location = new System.Drawing.Point(4, 24);
            this.tpageDeviceControlLOG.Name = "tpageDeviceControlLOG";
            this.tpageDeviceControlLOG.Padding = new System.Windows.Forms.Padding(3);
            this.tpageDeviceControlLOG.Size = new System.Drawing.Size(1464, 185);
            this.tpageDeviceControlLOG.TabIndex = 0;
            this.tpageDeviceControlLOG.Text = "设备控制日志";
            // 
            // lvDeviceControlLogDisp
            // 
            this.lvDeviceControlLogDisp.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Info});
            this.lvDeviceControlLogDisp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvDeviceControlLogDisp.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lvDeviceControlLogDisp.FullRowSelect = true;
            this.lvDeviceControlLogDisp.GridLines = true;
            this.lvDeviceControlLogDisp.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lvDeviceControlLogDisp.HideSelection = false;
            this.lvDeviceControlLogDisp.Location = new System.Drawing.Point(3, 3);
            this.lvDeviceControlLogDisp.MultiSelect = false;
            this.lvDeviceControlLogDisp.Name = "lvDeviceControlLogDisp";
            this.lvDeviceControlLogDisp.Size = new System.Drawing.Size(1458, 179);
            this.lvDeviceControlLogDisp.TabIndex = 0;
            this.lvDeviceControlLogDisp.UseCompatibleStateImageBehavior = false;
            this.lvDeviceControlLogDisp.View = System.Windows.Forms.View.Details;
            // 
            // Info
            // 
            this.Info.Width = 1200;
            // 
            // tpageMESLOG
            // 
            this.tpageMESLOG.BackColor = System.Drawing.SystemColors.Control;
            this.tpageMESLOG.Controls.Add(this.lvMESLogDisp);
            this.tpageMESLOG.Location = new System.Drawing.Point(4, 24);
            this.tpageMESLOG.Name = "tpageMESLOG";
            this.tpageMESLOG.Padding = new System.Windows.Forms.Padding(3);
            this.tpageMESLOG.Size = new System.Drawing.Size(1464, 185);
            this.tpageMESLOG.TabIndex = 3;
            this.tpageMESLOG.Text = "MES通讯日志";
            // 
            // lvMESLogDisp
            // 
            this.lvMESLogDisp.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader2});
            this.lvMESLogDisp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvMESLogDisp.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lvMESLogDisp.FullRowSelect = true;
            this.lvMESLogDisp.GridLines = true;
            this.lvMESLogDisp.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lvMESLogDisp.HideSelection = false;
            this.lvMESLogDisp.Location = new System.Drawing.Point(3, 3);
            this.lvMESLogDisp.MultiSelect = false;
            this.lvMESLogDisp.Name = "lvMESLogDisp";
            this.lvMESLogDisp.Size = new System.Drawing.Size(1458, 179);
            this.lvMESLogDisp.TabIndex = 1;
            this.lvMESLogDisp.UseCompatibleStateImageBehavior = false;
            this.lvMESLogDisp.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Width = 1200;
            // 
            // tpageAlarmRecord
            // 
            this.tpageAlarmRecord.BackColor = System.Drawing.SystemColors.Control;
            this.tpageAlarmRecord.Controls.Add(this.lvAlarmDisp);
            this.tpageAlarmRecord.Location = new System.Drawing.Point(4, 24);
            this.tpageAlarmRecord.Name = "tpageAlarmRecord";
            this.tpageAlarmRecord.Padding = new System.Windows.Forms.Padding(3);
            this.tpageAlarmRecord.Size = new System.Drawing.Size(1464, 185);
            this.tpageAlarmRecord.TabIndex = 1;
            this.tpageAlarmRecord.Text = "报警记录";
            // 
            // lvAlarmDisp
            // 
            this.lvAlarmDisp.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.lvAlarmDisp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvAlarmDisp.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lvAlarmDisp.FullRowSelect = true;
            this.lvAlarmDisp.GridLines = true;
            this.lvAlarmDisp.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lvAlarmDisp.HideSelection = false;
            this.lvAlarmDisp.Location = new System.Drawing.Point(3, 3);
            this.lvAlarmDisp.MultiSelect = false;
            this.lvAlarmDisp.Name = "lvAlarmDisp";
            this.lvAlarmDisp.Size = new System.Drawing.Size(1458, 179);
            this.lvAlarmDisp.TabIndex = 1;
            this.lvAlarmDisp.UseCompatibleStateImageBehavior = false;
            this.lvAlarmDisp.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Width = 1200;
            // 
            // tpageDeviceStatus
            // 
            this.tpageDeviceStatus.BackColor = System.Drawing.SystemColors.Control;
            this.tpageDeviceStatus.Controls.Add(this.lvDeviceStatus);
            this.tpageDeviceStatus.Location = new System.Drawing.Point(4, 24);
            this.tpageDeviceStatus.Name = "tpageDeviceStatus";
            this.tpageDeviceStatus.Padding = new System.Windows.Forms.Padding(3);
            this.tpageDeviceStatus.Size = new System.Drawing.Size(1464, 185);
            this.tpageDeviceStatus.TabIndex = 2;
            this.tpageDeviceStatus.Text = "设备状态";
            // 
            // lvDeviceStatus
            // 
            this.lvDeviceStatus.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.VariateName,
            this.VariateData});
            this.lvDeviceStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvDeviceStatus.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lvDeviceStatus.FullRowSelect = true;
            this.lvDeviceStatus.GridLines = true;
            this.lvDeviceStatus.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvDeviceStatus.HideSelection = false;
            this.lvDeviceStatus.Location = new System.Drawing.Point(3, 3);
            this.lvDeviceStatus.MultiSelect = false;
            this.lvDeviceStatus.Name = "lvDeviceStatus";
            this.lvDeviceStatus.Size = new System.Drawing.Size(1458, 179);
            this.lvDeviceStatus.TabIndex = 2;
            this.lvDeviceStatus.UseCompatibleStateImageBehavior = false;
            this.lvDeviceStatus.View = System.Windows.Forms.View.Details;
            // 
            // VariateName
            // 
            this.VariateName.Text = "变量名";
            this.VariateName.Width = 300;
            // 
            // VariateData
            // 
            this.VariateData.Text = "数据";
            this.VariateData.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.VariateData.Width = 300;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1472, 213);
            this.tableLayoutPanel1.TabIndex = 455;
            // 
            // ActionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1472, 823);
            this.Controls.Add(this.splitContainer1);
            this.Name = "ActionForm";
            this.Text = "运行画面";
            this.Load += new System.EventHandler(this.ActionForm_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picGotoSettingForm)).EndInit();
            this.tctlDeviceStatusRecord.ResumeLayout(false);
            this.tpageDeviceControlLOG.ResumeLayout(false);
            this.tpageMESLOG.ResumeLayout(false);
            this.tpageAlarmRecord.ResumeLayout(false);
            this.tpageDeviceStatus.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.PictureBox picGotoSettingForm;
        private System.Windows.Forms.Button btnSocketServerClose;
        private System.Windows.Forms.Label lblAccessLevelTitle;
        private System.Windows.Forms.Button btnSocketServerStart;
        private System.Windows.Forms.Label lblUserName;
        private System.Windows.Forms.Label lblAccessLevel;
        private System.Windows.Forms.Button btnDebug_S1;
        public System.Windows.Forms.Label lblActualProduct;
        public System.Windows.Forms.Label lblActualProductSelect;
        private System.Windows.Forms.Label lblConnectStatus;
        private System.Windows.Forms.TabControl tctlDeviceStatusRecord;
        private System.Windows.Forms.TabPage tpageDeviceControlLOG;
        private System.Windows.Forms.ListView lvDeviceControlLogDisp;
        private System.Windows.Forms.ColumnHeader Info;
        private System.Windows.Forms.TabPage tpageMESLOG;
        private System.Windows.Forms.ListView lvMESLogDisp;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.TabPage tpageAlarmRecord;
        private System.Windows.Forms.ListView lvAlarmDisp;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.TabPage tpageDeviceStatus;
        private System.Windows.Forms.ListView lvDeviceStatus;
        private System.Windows.Forms.ColumnHeader VariateName;
        private System.Windows.Forms.ColumnHeader VariateData;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button btnDebug_S2;
        private System.Windows.Forms.Button btnDebug_S3;
    }
}