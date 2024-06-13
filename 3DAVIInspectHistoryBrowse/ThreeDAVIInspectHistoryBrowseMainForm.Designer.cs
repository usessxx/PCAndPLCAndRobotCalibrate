namespace ThreeDAVIInspectHistoryBrowse
{
    partial class ThreeDAVIInspectHistoryBrowseMainForm
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ThreeDAVIInspectHistoryBrowseMainForm));
            this.gbxSearchRegion = new System.Windows.Forms.GroupBox();
            this.dtpSearchStopHourTime = new System.Windows.Forms.DateTimePicker();
            this.dtpSearchStopDateTime = new System.Windows.Forms.DateTimePicker();
            this.dtpSearchStartHourTime = new System.Windows.Forms.DateTimePicker();
            this.lblSearchStopTimeTitle = new System.Windows.Forms.Label();
            this.dtpSearchStartDateTime = new System.Windows.Forms.DateTimePicker();
            this.lblSearchStartTimeTitle = new System.Windows.Forms.Label();
            this.rbtnSearchRegionSettime = new System.Windows.Forms.RadioButton();
            this.rbtnSearchRegionOneMonth = new System.Windows.Forms.RadioButton();
            this.rbtnSearchRegionOneWeek = new System.Windows.Forms.RadioButton();
            this.rbtnSearchRegionOneDay = new System.Windows.Forms.RadioButton();
            this.btnBrowseNormalProductCheckResultSavePath = new System.Windows.Forms.Button();
            this.txtNormalProductCheckResultSavePath = new System.Windows.Forms.TextBox();
            this.gbxPathAndFilter = new System.Windows.Forms.GroupBox();
            this.chkCheckSampleProduct = new System.Windows.Forms.CheckBox();
            this.chkCheckNormalProduct = new System.Windows.Forms.CheckBox();
            this.txtBarcodeFilterBarcode = new System.Windows.Forms.TextBox();
            this.cboProductName = new System.Windows.Forms.ComboBox();
            this.chkBarcodeFilter = new System.Windows.Forms.CheckBox();
            this.chkProductNameFilter = new System.Windows.Forms.CheckBox();
            this.txtSamplePanelCheckResultSavePath = new System.Windows.Forms.TextBox();
            this.btnBrowseSamplePanelCheckResultSavePath = new System.Windows.Forms.Button();
            this.btnSearch = new System.Windows.Forms.Button();
            this.dgrdvSearchResultDisp = new System.Windows.Forms.DataGridView();
            this.Number = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ProductName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BarcodeData = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.InspectStartTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.InspectStopTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.InspectTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PassRate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Path = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnSearchKindOKProduct = new System.Windows.Forms.Button();
            this.btnSearchKindNGProduct = new System.Windows.Forms.Button();
            this.lblInpsectResult = new System.Windows.Forms.Label();
            this.lblSearchResultQuantity = new System.Windows.Forms.Label();
            this.tmrRefreshTime = new System.Windows.Forms.Timer(this.components);
            this.lblCurrentTime = new System.Windows.Forms.Label();
            this.rbtnElementYield = new System.Windows.Forms.RadioButton();
            this.rbtnPanelYield = new System.Windows.Forms.RadioButton();
            this.rbtnPcsYield = new System.Windows.Forms.RadioButton();
            this.dgrdvPassRateRecord = new System.Windows.Forms.DataGridView();
            this.yieldProjectName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.totalYield = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.gbxPassRate = new System.Windows.Forms.GroupBox();
            this.gbxInspectResultDisp = new System.Windows.Forms.GroupBox();
            this.lblSelectedProductName = new System.Windows.Forms.Label();
            this.dgrdvCheckResultDisp = new System.Windows.Forms.DataGridView();
            this.rbtnElementResult = new System.Windows.Forms.RadioButton();
            this.rbtnPCSResult = new System.Windows.Forms.RadioButton();
            this.cmsSearchResultDispRightClick = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.删除ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lblAccessLevelTitle = new System.Windows.Forms.Label();
            this.lblUserName = new System.Windows.Forms.Label();
            this.lblAccessLevel = new System.Windows.Forms.Label();
            this.pnlSearching = new System.Windows.Forms.Panel();
            this.lblSearching = new System.Windows.Forms.Label();
            this.gbxSearchRegion.SuspendLayout();
            this.gbxPathAndFilter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgrdvSearchResultDisp)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgrdvPassRateRecord)).BeginInit();
            this.gbxPassRate.SuspendLayout();
            this.gbxInspectResultDisp.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgrdvCheckResultDisp)).BeginInit();
            this.cmsSearchResultDispRightClick.SuspendLayout();
            this.pnlSearching.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbxSearchRegion
            // 
            this.gbxSearchRegion.Controls.Add(this.dtpSearchStopHourTime);
            this.gbxSearchRegion.Controls.Add(this.dtpSearchStopDateTime);
            this.gbxSearchRegion.Controls.Add(this.dtpSearchStartHourTime);
            this.gbxSearchRegion.Controls.Add(this.lblSearchStopTimeTitle);
            this.gbxSearchRegion.Controls.Add(this.dtpSearchStartDateTime);
            this.gbxSearchRegion.Controls.Add(this.lblSearchStartTimeTitle);
            this.gbxSearchRegion.Controls.Add(this.rbtnSearchRegionSettime);
            this.gbxSearchRegion.Controls.Add(this.rbtnSearchRegionOneMonth);
            this.gbxSearchRegion.Controls.Add(this.rbtnSearchRegionOneWeek);
            this.gbxSearchRegion.Controls.Add(this.rbtnSearchRegionOneDay);
            this.gbxSearchRegion.Location = new System.Drawing.Point(10, 12);
            this.gbxSearchRegion.Margin = new System.Windows.Forms.Padding(4);
            this.gbxSearchRegion.Name = "gbxSearchRegion";
            this.gbxSearchRegion.Padding = new System.Windows.Forms.Padding(4);
            this.gbxSearchRegion.Size = new System.Drawing.Size(514, 177);
            this.gbxSearchRegion.TabIndex = 2;
            this.gbxSearchRegion.TabStop = false;
            this.gbxSearchRegion.Text = "查询范围";
            // 
            // dtpSearchStopHourTime
            // 
            this.dtpSearchStopHourTime.CalendarFont = new System.Drawing.Font("KaiTi", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.dtpSearchStopHourTime.Font = new System.Drawing.Font("KaiTi", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.dtpSearchStopHourTime.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dtpSearchStopHourTime.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.dtpSearchStopHourTime.Location = new System.Drawing.Point(335, 112);
            this.dtpSearchStopHourTime.Name = "dtpSearchStopHourTime";
            this.dtpSearchStopHourTime.ShowUpDown = true;
            this.dtpSearchStopHourTime.Size = new System.Drawing.Size(149, 29);
            this.dtpSearchStopHourTime.TabIndex = 2;
            this.dtpSearchStopHourTime.Value = new System.DateTime(2022, 5, 14, 0, 0, 0, 0);
            // 
            // dtpSearchStopDateTime
            // 
            this.dtpSearchStopDateTime.CalendarFont = new System.Drawing.Font("KaiTi", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.dtpSearchStopDateTime.Font = new System.Drawing.Font("KaiTi", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.dtpSearchStopDateTime.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpSearchStopDateTime.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.dtpSearchStopDateTime.Location = new System.Drawing.Point(167, 112);
            this.dtpSearchStopDateTime.Name = "dtpSearchStopDateTime";
            this.dtpSearchStopDateTime.Size = new System.Drawing.Size(149, 29);
            this.dtpSearchStopDateTime.TabIndex = 2;
            this.dtpSearchStopDateTime.Value = new System.DateTime(2022, 5, 14, 0, 0, 0, 0);
            this.dtpSearchStopDateTime.ValueChanged += new System.EventHandler(this.DateTimePickerControlValueChangedDataCheckEvent);
            // 
            // dtpSearchStartHourTime
            // 
            this.dtpSearchStartHourTime.CalendarFont = new System.Drawing.Font("KaiTi", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.dtpSearchStartHourTime.Font = new System.Drawing.Font("KaiTi", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.dtpSearchStartHourTime.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dtpSearchStartHourTime.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.dtpSearchStartHourTime.Location = new System.Drawing.Point(335, 69);
            this.dtpSearchStartHourTime.Name = "dtpSearchStartHourTime";
            this.dtpSearchStartHourTime.ShowUpDown = true;
            this.dtpSearchStartHourTime.Size = new System.Drawing.Size(149, 29);
            this.dtpSearchStartHourTime.TabIndex = 2;
            this.dtpSearchStartHourTime.Value = new System.DateTime(2022, 5, 14, 0, 0, 0, 0);
            // 
            // lblSearchStopTimeTitle
            // 
            this.lblSearchStopTimeTitle.Location = new System.Drawing.Point(4, 115);
            this.lblSearchStopTimeTitle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblSearchStopTimeTitle.Name = "lblSearchStopTimeTitle";
            this.lblSearchStopTimeTitle.Size = new System.Drawing.Size(160, 23);
            this.lblSearchStopTimeTitle.TabIndex = 1;
            this.lblSearchStopTimeTitle.Text = "查询截止时间:";
            this.lblSearchStopTimeTitle.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // dtpSearchStartDateTime
            // 
            this.dtpSearchStartDateTime.CalendarFont = new System.Drawing.Font("KaiTi", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.dtpSearchStartDateTime.Font = new System.Drawing.Font("KaiTi", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.dtpSearchStartDateTime.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpSearchStartDateTime.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.dtpSearchStartDateTime.Location = new System.Drawing.Point(167, 69);
            this.dtpSearchStartDateTime.Name = "dtpSearchStartDateTime";
            this.dtpSearchStartDateTime.Size = new System.Drawing.Size(149, 29);
            this.dtpSearchStartDateTime.TabIndex = 2;
            this.dtpSearchStartDateTime.Value = new System.DateTime(2022, 5, 14, 0, 0, 0, 0);
            this.dtpSearchStartDateTime.ValueChanged += new System.EventHandler(this.DateTimePickerControlValueChangedDataCheckEvent);
            // 
            // lblSearchStartTimeTitle
            // 
            this.lblSearchStartTimeTitle.Location = new System.Drawing.Point(4, 72);
            this.lblSearchStartTimeTitle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblSearchStartTimeTitle.Name = "lblSearchStartTimeTitle";
            this.lblSearchStartTimeTitle.Size = new System.Drawing.Size(160, 23);
            this.lblSearchStartTimeTitle.TabIndex = 1;
            this.lblSearchStartTimeTitle.Text = "查询开始时间:";
            this.lblSearchStartTimeTitle.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // rbtnSearchRegionSettime
            // 
            this.rbtnSearchRegionSettime.AutoSize = true;
            this.rbtnSearchRegionSettime.Font = new System.Drawing.Font("KaiTi", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.rbtnSearchRegionSettime.Location = new System.Drawing.Point(338, 36);
            this.rbtnSearchRegionSettime.Margin = new System.Windows.Forms.Padding(4);
            this.rbtnSearchRegionSettime.Name = "rbtnSearchRegionSettime";
            this.rbtnSearchRegionSettime.Size = new System.Drawing.Size(107, 24);
            this.rbtnSearchRegionSettime.TabIndex = 0;
            this.rbtnSearchRegionSettime.TabStop = true;
            this.rbtnSearchRegionSettime.Text = "设置时间";
            this.rbtnSearchRegionSettime.UseVisualStyleBackColor = true;
            this.rbtnSearchRegionSettime.CheckedChanged += new System.EventHandler(this.SelectSearchRegionChangeEvent);
            // 
            // rbtnSearchRegionOneMonth
            // 
            this.rbtnSearchRegionOneMonth.AutoSize = true;
            this.rbtnSearchRegionOneMonth.Font = new System.Drawing.Font("KaiTi", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.rbtnSearchRegionOneMonth.Location = new System.Drawing.Point(237, 36);
            this.rbtnSearchRegionOneMonth.Margin = new System.Windows.Forms.Padding(4);
            this.rbtnSearchRegionOneMonth.Name = "rbtnSearchRegionOneMonth";
            this.rbtnSearchRegionOneMonth.Size = new System.Drawing.Size(67, 24);
            this.rbtnSearchRegionOneMonth.TabIndex = 0;
            this.rbtnSearchRegionOneMonth.TabStop = true;
            this.rbtnSearchRegionOneMonth.Text = "一月";
            this.rbtnSearchRegionOneMonth.UseVisualStyleBackColor = true;
            this.rbtnSearchRegionOneMonth.CheckedChanged += new System.EventHandler(this.SelectSearchRegionChangeEvent);
            // 
            // rbtnSearchRegionOneWeek
            // 
            this.rbtnSearchRegionOneWeek.AutoSize = true;
            this.rbtnSearchRegionOneWeek.Font = new System.Drawing.Font("KaiTi", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.rbtnSearchRegionOneWeek.Location = new System.Drawing.Point(136, 36);
            this.rbtnSearchRegionOneWeek.Margin = new System.Windows.Forms.Padding(4);
            this.rbtnSearchRegionOneWeek.Name = "rbtnSearchRegionOneWeek";
            this.rbtnSearchRegionOneWeek.Size = new System.Drawing.Size(67, 24);
            this.rbtnSearchRegionOneWeek.TabIndex = 0;
            this.rbtnSearchRegionOneWeek.TabStop = true;
            this.rbtnSearchRegionOneWeek.Text = "一周";
            this.rbtnSearchRegionOneWeek.UseVisualStyleBackColor = true;
            this.rbtnSearchRegionOneWeek.CheckedChanged += new System.EventHandler(this.SelectSearchRegionChangeEvent);
            // 
            // rbtnSearchRegionOneDay
            // 
            this.rbtnSearchRegionOneDay.AutoSize = true;
            this.rbtnSearchRegionOneDay.Font = new System.Drawing.Font("KaiTi", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.rbtnSearchRegionOneDay.Location = new System.Drawing.Point(34, 36);
            this.rbtnSearchRegionOneDay.Margin = new System.Windows.Forms.Padding(4);
            this.rbtnSearchRegionOneDay.Name = "rbtnSearchRegionOneDay";
            this.rbtnSearchRegionOneDay.Size = new System.Drawing.Size(67, 24);
            this.rbtnSearchRegionOneDay.TabIndex = 0;
            this.rbtnSearchRegionOneDay.TabStop = true;
            this.rbtnSearchRegionOneDay.Text = "一天";
            this.rbtnSearchRegionOneDay.UseVisualStyleBackColor = true;
            this.rbtnSearchRegionOneDay.CheckedChanged += new System.EventHandler(this.SelectSearchRegionChangeEvent);
            // 
            // btnBrowseNormalProductCheckResultSavePath
            // 
            this.btnBrowseNormalProductCheckResultSavePath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowseNormalProductCheckResultSavePath.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBrowseNormalProductCheckResultSavePath.Location = new System.Drawing.Point(858, 29);
            this.btnBrowseNormalProductCheckResultSavePath.Name = "btnBrowseNormalProductCheckResultSavePath";
            this.btnBrowseNormalProductCheckResultSavePath.Size = new System.Drawing.Size(49, 30);
            this.btnBrowseNormalProductCheckResultSavePath.TabIndex = 31;
            this.btnBrowseNormalProductCheckResultSavePath.Text = "...";
            this.btnBrowseNormalProductCheckResultSavePath.UseVisualStyleBackColor = true;
            this.btnBrowseNormalProductCheckResultSavePath.Click += new System.EventHandler(this.DirectoryBrowseEvent);
            // 
            // txtNormalProductCheckResultSavePath
            // 
            this.txtNormalProductCheckResultSavePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtNormalProductCheckResultSavePath.BackColor = System.Drawing.Color.White;
            this.txtNormalProductCheckResultSavePath.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtNormalProductCheckResultSavePath.Location = new System.Drawing.Point(277, 29);
            this.txtNormalProductCheckResultSavePath.Name = "txtNormalProductCheckResultSavePath";
            this.txtNormalProductCheckResultSavePath.ReadOnly = true;
            this.txtNormalProductCheckResultSavePath.Size = new System.Drawing.Size(575, 29);
            this.txtNormalProductCheckResultSavePath.TabIndex = 30;
            // 
            // gbxPathAndFilter
            // 
            this.gbxPathAndFilter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbxPathAndFilter.Controls.Add(this.chkCheckSampleProduct);
            this.gbxPathAndFilter.Controls.Add(this.chkCheckNormalProduct);
            this.gbxPathAndFilter.Controls.Add(this.txtBarcodeFilterBarcode);
            this.gbxPathAndFilter.Controls.Add(this.cboProductName);
            this.gbxPathAndFilter.Controls.Add(this.chkBarcodeFilter);
            this.gbxPathAndFilter.Controls.Add(this.chkProductNameFilter);
            this.gbxPathAndFilter.Controls.Add(this.txtSamplePanelCheckResultSavePath);
            this.gbxPathAndFilter.Controls.Add(this.txtNormalProductCheckResultSavePath);
            this.gbxPathAndFilter.Controls.Add(this.btnBrowseSamplePanelCheckResultSavePath);
            this.gbxPathAndFilter.Controls.Add(this.btnBrowseNormalProductCheckResultSavePath);
            this.gbxPathAndFilter.Location = new System.Drawing.Point(537, 12);
            this.gbxPathAndFilter.Name = "gbxPathAndFilter";
            this.gbxPathAndFilter.Size = new System.Drawing.Size(917, 177);
            this.gbxPathAndFilter.TabIndex = 32;
            this.gbxPathAndFilter.TabStop = false;
            this.gbxPathAndFilter.Text = "路径及数据筛选";
            // 
            // chkCheckSampleProduct
            // 
            this.chkCheckSampleProduct.AutoSize = true;
            this.chkCheckSampleProduct.Location = new System.Drawing.Point(24, 67);
            this.chkCheckSampleProduct.Name = "chkCheckSampleProduct";
            this.chkCheckSampleProduct.Size = new System.Drawing.Size(228, 24);
            this.chkCheckSampleProduct.TabIndex = 36;
            this.chkCheckSampleProduct.Text = "样品板检测结果路径: ";
            this.chkCheckSampleProduct.UseVisualStyleBackColor = true;
            // 
            // chkCheckNormalProduct
            // 
            this.chkCheckNormalProduct.AutoSize = true;
            this.chkCheckNormalProduct.Location = new System.Drawing.Point(24, 31);
            this.chkCheckNormalProduct.Name = "chkCheckNormalProduct";
            this.chkCheckNormalProduct.Size = new System.Drawing.Size(248, 24);
            this.chkCheckNormalProduct.TabIndex = 36;
            this.chkCheckNormalProduct.Text = "常规产品检测结果路径: ";
            this.chkCheckNormalProduct.UseVisualStyleBackColor = true;
            // 
            // txtBarcodeFilterBarcode
            // 
            this.txtBarcodeFilterBarcode.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtBarcodeFilterBarcode.Enabled = false;
            this.txtBarcodeFilterBarcode.Location = new System.Drawing.Point(167, 136);
            this.txtBarcodeFilterBarcode.Name = "txtBarcodeFilterBarcode";
            this.txtBarcodeFilterBarcode.Size = new System.Drawing.Size(740, 30);
            this.txtBarcodeFilterBarcode.TabIndex = 35;
            // 
            // cboProductName
            // 
            this.cboProductName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboProductName.Enabled = false;
            this.cboProductName.FormattingEnabled = true;
            this.cboProductName.Location = new System.Drawing.Point(167, 100);
            this.cboProductName.Name = "cboProductName";
            this.cboProductName.Size = new System.Drawing.Size(121, 28);
            this.cboProductName.TabIndex = 34;
            this.cboProductName.Click += new System.EventHandler(this.cboProductName_Click);
            // 
            // chkBarcodeFilter
            // 
            this.chkBarcodeFilter.AutoSize = true;
            this.chkBarcodeFilter.Location = new System.Drawing.Point(24, 139);
            this.chkBarcodeFilter.Name = "chkBarcodeFilter";
            this.chkBarcodeFilter.Size = new System.Drawing.Size(128, 24);
            this.chkBarcodeFilter.TabIndex = 33;
            this.chkBarcodeFilter.Text = "二维码筛选";
            this.chkBarcodeFilter.UseVisualStyleBackColor = true;
            this.chkBarcodeFilter.CheckedChanged += new System.EventHandler(this.chkBarcodeFilter_CheckedChanged);
            // 
            // chkProductNameFilter
            // 
            this.chkProductNameFilter.AutoSize = true;
            this.chkProductNameFilter.Location = new System.Drawing.Point(24, 103);
            this.chkProductNameFilter.Name = "chkProductNameFilter";
            this.chkProductNameFilter.Size = new System.Drawing.Size(128, 24);
            this.chkProductNameFilter.TabIndex = 33;
            this.chkProductNameFilter.Text = "产品名筛选";
            this.chkProductNameFilter.UseVisualStyleBackColor = true;
            this.chkProductNameFilter.CheckedChanged += new System.EventHandler(this.chkProductNameFilter_CheckedChanged);
            // 
            // txtSamplePanelCheckResultSavePath
            // 
            this.txtSamplePanelCheckResultSavePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSamplePanelCheckResultSavePath.BackColor = System.Drawing.Color.White;
            this.txtSamplePanelCheckResultSavePath.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSamplePanelCheckResultSavePath.Location = new System.Drawing.Point(277, 65);
            this.txtSamplePanelCheckResultSavePath.Name = "txtSamplePanelCheckResultSavePath";
            this.txtSamplePanelCheckResultSavePath.ReadOnly = true;
            this.txtSamplePanelCheckResultSavePath.Size = new System.Drawing.Size(575, 29);
            this.txtSamplePanelCheckResultSavePath.TabIndex = 30;
            // 
            // btnBrowseSamplePanelCheckResultSavePath
            // 
            this.btnBrowseSamplePanelCheckResultSavePath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowseSamplePanelCheckResultSavePath.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBrowseSamplePanelCheckResultSavePath.Location = new System.Drawing.Point(858, 65);
            this.btnBrowseSamplePanelCheckResultSavePath.Name = "btnBrowseSamplePanelCheckResultSavePath";
            this.btnBrowseSamplePanelCheckResultSavePath.Size = new System.Drawing.Size(49, 30);
            this.btnBrowseSamplePanelCheckResultSavePath.TabIndex = 31;
            this.btnBrowseSamplePanelCheckResultSavePath.Text = "...";
            this.btnBrowseSamplePanelCheckResultSavePath.UseVisualStyleBackColor = true;
            this.btnBrowseSamplePanelCheckResultSavePath.Click += new System.EventHandler(this.DirectoryBrowseEvent);
            // 
            // btnSearch
            // 
            this.btnSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSearch.Font = new System.Drawing.Font("KaiTi", 48F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnSearch.Location = new System.Drawing.Point(1461, 70);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(171, 119);
            this.btnSearch.TabIndex = 33;
            this.btnSearch.Text = "搜索";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // dgrdvSearchResultDisp
            // 
            this.dgrdvSearchResultDisp.AllowUserToAddRows = false;
            this.dgrdvSearchResultDisp.AllowUserToDeleteRows = false;
            this.dgrdvSearchResultDisp.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgrdvSearchResultDisp.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Sunken;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("KaiTi", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgrdvSearchResultDisp.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgrdvSearchResultDisp.ColumnHeadersHeight = 30;
            this.dgrdvSearchResultDisp.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgrdvSearchResultDisp.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Number,
            this.ProductName,
            this.BarcodeData,
            this.InspectStartTime,
            this.InspectStopTime,
            this.InspectTime,
            this.PassRate,
            this.Path});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("KaiTi", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgrdvSearchResultDisp.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgrdvSearchResultDisp.Location = new System.Drawing.Point(10, 236);
            this.dgrdvSearchResultDisp.Name = "dgrdvSearchResultDisp";
            this.dgrdvSearchResultDisp.ReadOnly = true;
            this.dgrdvSearchResultDisp.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dgrdvSearchResultDisp.RowHeadersVisible = false;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("KaiTi", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.Black;
            this.dgrdvSearchResultDisp.RowsDefaultCellStyle = dataGridViewCellStyle3;
            this.dgrdvSearchResultDisp.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("Times New Roman", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgrdvSearchResultDisp.RowTemplate.DefaultCellStyle.ForeColor = System.Drawing.Color.Black;
            this.dgrdvSearchResultDisp.RowTemplate.Height = 23;
            this.dgrdvSearchResultDisp.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dgrdvSearchResultDisp.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgrdvSearchResultDisp.Size = new System.Drawing.Size(1207, 723);
            this.dgrdvSearchResultDisp.TabIndex = 35;
            this.dgrdvSearchResultDisp.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgrdvSearchResultDisp_CellDoubleClick);
            this.dgrdvSearchResultDisp.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgrdvSearchResultDisp_CellMouseClick);
            this.dgrdvSearchResultDisp.MouseDown += new System.Windows.Forms.MouseEventHandler(this.dgrdvSearchResultDisp_MouseDown);
            // 
            // Number
            // 
            this.Number.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Number.FillWeight = 7F;
            this.Number.HeaderText = "序号";
            this.Number.Name = "Number";
            this.Number.ReadOnly = true;
            // 
            // ProductName
            // 
            this.ProductName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ProductName.FillWeight = 15F;
            this.ProductName.HeaderText = "产品名";
            this.ProductName.Name = "ProductName";
            this.ProductName.ReadOnly = true;
            // 
            // BarcodeData
            // 
            this.BarcodeData.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.BarcodeData.FillWeight = 20F;
            this.BarcodeData.HeaderText = "二维码";
            this.BarcodeData.Name = "BarcodeData";
            this.BarcodeData.ReadOnly = true;
            // 
            // InspectStartTime
            // 
            this.InspectStartTime.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.InspectStartTime.FillWeight = 20F;
            this.InspectStartTime.HeaderText = "检测开始时间";
            this.InspectStartTime.Name = "InspectStartTime";
            this.InspectStartTime.ReadOnly = true;
            // 
            // InspectStopTime
            // 
            this.InspectStopTime.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.InspectStopTime.FillWeight = 20F;
            this.InspectStopTime.HeaderText = "检测结束时间";
            this.InspectStopTime.Name = "InspectStopTime";
            this.InspectStopTime.ReadOnly = true;
            // 
            // InspectTime
            // 
            this.InspectTime.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.InspectTime.FillWeight = 12F;
            this.InspectTime.HeaderText = "检测结果";
            this.InspectTime.Name = "InspectTime";
            this.InspectTime.ReadOnly = true;
            // 
            // PassRate
            // 
            this.PassRate.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.PassRate.FillWeight = 7F;
            this.PassRate.HeaderText = "良率";
            this.PassRate.Name = "PassRate";
            this.PassRate.ReadOnly = true;
            // 
            // Path
            // 
            this.Path.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Path.FillWeight = 30F;
            this.Path.HeaderText = "文件夹路径";
            this.Path.Name = "Path";
            this.Path.ReadOnly = true;
            // 
            // btnSearchKindOKProduct
            // 
            this.btnSearchKindOKProduct.BackColor = System.Drawing.SystemColors.ButtonShadow;
            this.btnSearchKindOKProduct.Font = new System.Drawing.Font("KaiTi", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnSearchKindOKProduct.ForeColor = System.Drawing.Color.Green;
            this.btnSearchKindOKProduct.Location = new System.Drawing.Point(609, 196);
            this.btnSearchKindOKProduct.Name = "btnSearchKindOKProduct";
            this.btnSearchKindOKProduct.Size = new System.Drawing.Size(86, 34);
            this.btnSearchKindOKProduct.TabIndex = 36;
            this.btnSearchKindOKProduct.Text = "良品";
            this.btnSearchKindOKProduct.UseVisualStyleBackColor = false;
            this.btnSearchKindOKProduct.Click += new System.EventHandler(this.btnSearchKindOKProduct_Click);
            // 
            // btnSearchKindNGProduct
            // 
            this.btnSearchKindNGProduct.BackColor = System.Drawing.SystemColors.ButtonShadow;
            this.btnSearchKindNGProduct.Font = new System.Drawing.Font("KaiTi", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnSearchKindNGProduct.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnSearchKindNGProduct.Location = new System.Drawing.Point(701, 196);
            this.btnSearchKindNGProduct.Name = "btnSearchKindNGProduct";
            this.btnSearchKindNGProduct.Size = new System.Drawing.Size(86, 34);
            this.btnSearchKindNGProduct.TabIndex = 36;
            this.btnSearchKindNGProduct.Text = "不良品";
            this.btnSearchKindNGProduct.UseVisualStyleBackColor = false;
            this.btnSearchKindNGProduct.Click += new System.EventHandler(this.btnSearchKindNGProduct_Click);
            // 
            // lblInpsectResult
            // 
            this.lblInpsectResult.Location = new System.Drawing.Point(494, 196);
            this.lblInpsectResult.Name = "lblInpsectResult";
            this.lblInpsectResult.Size = new System.Drawing.Size(109, 34);
            this.lblInpsectResult.TabIndex = 37;
            this.lblInpsectResult.Text = "检测结果: ";
            this.lblInpsectResult.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblSearchResultQuantity
            // 
            this.lblSearchResultQuantity.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSearchResultQuantity.Location = new System.Drawing.Point(1131, 196);
            this.lblSearchResultQuantity.Name = "lblSearchResultQuantity";
            this.lblSearchResultQuantity.Size = new System.Drawing.Size(304, 34);
            this.lblSearchResultQuantity.TabIndex = 37;
            this.lblSearchResultQuantity.Text = "搜索结果个数: 0";
            this.lblSearchResultQuantity.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tmrRefreshTime
            // 
            this.tmrRefreshTime.Enabled = true;
            this.tmrRefreshTime.Interval = 1000;
            this.tmrRefreshTime.Tick += new System.EventHandler(this.tmrRefreshTime_Tick);
            // 
            // lblCurrentTime
            // 
            this.lblCurrentTime.Location = new System.Drawing.Point(13, 196);
            this.lblCurrentTime.Name = "lblCurrentTime";
            this.lblCurrentTime.Size = new System.Drawing.Size(199, 34);
            this.lblCurrentTime.TabIndex = 38;
            this.lblCurrentTime.Text = "2022/05/18 09:00:00";
            this.lblCurrentTime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // rbtnElementYield
            // 
            this.rbtnElementYield.Font = new System.Drawing.Font("Times New Roman", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbtnElementYield.Location = new System.Drawing.Point(217, 28);
            this.rbtnElementYield.Name = "rbtnElementYield";
            this.rbtnElementYield.Size = new System.Drawing.Size(80, 26);
            this.rbtnElementYield.TabIndex = 40;
            this.rbtnElementYield.TabStop = true;
            this.rbtnElementYield.Text = "元件";
            this.rbtnElementYield.UseVisualStyleBackColor = true;
            this.rbtnElementYield.Click += new System.EventHandler(this.YieldDispRadioButtonClickEvent);
            // 
            // rbtnPanelYield
            // 
            this.rbtnPanelYield.Font = new System.Drawing.Font("Times New Roman", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbtnPanelYield.Location = new System.Drawing.Point(114, 28);
            this.rbtnPanelYield.Name = "rbtnPanelYield";
            this.rbtnPanelYield.Size = new System.Drawing.Size(80, 26);
            this.rbtnPanelYield.TabIndex = 41;
            this.rbtnPanelYield.TabStop = true;
            this.rbtnPanelYield.Text = "整版";
            this.rbtnPanelYield.UseVisualStyleBackColor = true;
            this.rbtnPanelYield.Click += new System.EventHandler(this.YieldDispRadioButtonClickEvent);
            // 
            // rbtnPcsYield
            // 
            this.rbtnPcsYield.Font = new System.Drawing.Font("Times New Roman", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbtnPcsYield.Location = new System.Drawing.Point(11, 28);
            this.rbtnPcsYield.Name = "rbtnPcsYield";
            this.rbtnPcsYield.Size = new System.Drawing.Size(80, 26);
            this.rbtnPcsYield.TabIndex = 42;
            this.rbtnPcsYield.TabStop = true;
            this.rbtnPcsYield.Text = "PCS";
            this.rbtnPcsYield.UseVisualStyleBackColor = true;
            this.rbtnPcsYield.Click += new System.EventHandler(this.YieldDispRadioButtonClickEvent);
            // 
            // dgrdvPassRateRecord
            // 
            this.dgrdvPassRateRecord.AllowUserToAddRows = false;
            this.dgrdvPassRateRecord.AllowUserToDeleteRows = false;
            this.dgrdvPassRateRecord.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("KaiTi", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgrdvPassRateRecord.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.dgrdvPassRateRecord.ColumnHeadersHeight = 30;
            this.dgrdvPassRateRecord.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgrdvPassRateRecord.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.yieldProjectName,
            this.totalYield});
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("KaiTi", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgrdvPassRateRecord.DefaultCellStyle = dataGridViewCellStyle5;
            this.dgrdvPassRateRecord.Location = new System.Drawing.Point(10, 60);
            this.dgrdvPassRateRecord.Name = "dgrdvPassRateRecord";
            this.dgrdvPassRateRecord.ReadOnly = true;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("KaiTi", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgrdvPassRateRecord.RowHeadersDefaultCellStyle = dataGridViewCellStyle6;
            this.dgrdvPassRateRecord.RowHeadersVisible = false;
            this.dgrdvPassRateRecord.RowTemplate.DefaultCellStyle.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.dgrdvPassRateRecord.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgrdvPassRateRecord.RowTemplate.DefaultCellStyle.ForeColor = System.Drawing.Color.Black;
            this.dgrdvPassRateRecord.RowTemplate.Height = 40;
            this.dgrdvPassRateRecord.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgrdvPassRateRecord.Size = new System.Drawing.Size(392, 172);
            this.dgrdvPassRateRecord.TabIndex = 39;
            // 
            // yieldProjectName
            // 
            this.yieldProjectName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.yieldProjectName.FillWeight = 25F;
            this.yieldProjectName.HeaderText = "项目";
            this.yieldProjectName.Name = "yieldProjectName";
            this.yieldProjectName.ReadOnly = true;
            this.yieldProjectName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // totalYield
            // 
            this.totalYield.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.totalYield.FillWeight = 25F;
            this.totalYield.HeaderText = "产量";
            this.totalYield.Name = "totalYield";
            this.totalYield.ReadOnly = true;
            this.totalYield.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // gbxPassRate
            // 
            this.gbxPassRate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.gbxPassRate.Controls.Add(this.dgrdvPassRateRecord);
            this.gbxPassRate.Controls.Add(this.rbtnElementYield);
            this.gbxPassRate.Controls.Add(this.rbtnPcsYield);
            this.gbxPassRate.Controls.Add(this.rbtnPanelYield);
            this.gbxPassRate.Location = new System.Drawing.Point(1223, 236);
            this.gbxPassRate.Name = "gbxPassRate";
            this.gbxPassRate.Size = new System.Drawing.Size(409, 243);
            this.gbxPassRate.TabIndex = 43;
            this.gbxPassRate.TabStop = false;
            this.gbxPassRate.Text = "良率";
            // 
            // gbxInspectResultDisp
            // 
            this.gbxInspectResultDisp.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbxInspectResultDisp.Controls.Add(this.lblSelectedProductName);
            this.gbxInspectResultDisp.Controls.Add(this.dgrdvCheckResultDisp);
            this.gbxInspectResultDisp.Controls.Add(this.rbtnElementResult);
            this.gbxInspectResultDisp.Controls.Add(this.rbtnPCSResult);
            this.gbxInspectResultDisp.Location = new System.Drawing.Point(1223, 485);
            this.gbxInspectResultDisp.Name = "gbxInspectResultDisp";
            this.gbxInspectResultDisp.Size = new System.Drawing.Size(409, 474);
            this.gbxInspectResultDisp.TabIndex = 43;
            this.gbxInspectResultDisp.TabStop = false;
            this.gbxInspectResultDisp.Text = "检测结果";
            // 
            // lblSelectedProductName
            // 
            this.lblSelectedProductName.Font = new System.Drawing.Font("KaiTi", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblSelectedProductName.Location = new System.Drawing.Point(10, 52);
            this.lblSelectedProductName.Name = "lblSelectedProductName";
            this.lblSelectedProductName.Size = new System.Drawing.Size(392, 26);
            this.lblSelectedProductName.TabIndex = 43;
            this.lblSelectedProductName.Text = "名称: JA12606_05_B 2022040819031701519Y";
            this.lblSelectedProductName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // dgrdvCheckResultDisp
            // 
            this.dgrdvCheckResultDisp.AllowUserToAddRows = false;
            this.dgrdvCheckResultDisp.AllowUserToDeleteRows = false;
            this.dgrdvCheckResultDisp.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle7.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            dataGridViewCellStyle7.Font = new System.Drawing.Font("KaiTi", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle7.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle7.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgrdvCheckResultDisp.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle7;
            this.dgrdvCheckResultDisp.ColumnHeadersHeight = 40;
            this.dgrdvCheckResultDisp.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle8.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            dataGridViewCellStyle8.Font = new System.Drawing.Font("KaiTi", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle8.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle8.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle8.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgrdvCheckResultDisp.DefaultCellStyle = dataGridViewCellStyle8;
            this.dgrdvCheckResultDisp.Location = new System.Drawing.Point(10, 81);
            this.dgrdvCheckResultDisp.Name = "dgrdvCheckResultDisp";
            this.dgrdvCheckResultDisp.ReadOnly = true;
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle9.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle9.Font = new System.Drawing.Font("KaiTi", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle9.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle9.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle9.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle9.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgrdvCheckResultDisp.RowHeadersDefaultCellStyle = dataGridViewCellStyle9;
            this.dgrdvCheckResultDisp.RowHeadersVisible = false;
            this.dgrdvCheckResultDisp.RowTemplate.DefaultCellStyle.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.dgrdvCheckResultDisp.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgrdvCheckResultDisp.RowTemplate.DefaultCellStyle.ForeColor = System.Drawing.Color.Black;
            this.dgrdvCheckResultDisp.RowTemplate.Height = 20;
            this.dgrdvCheckResultDisp.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgrdvCheckResultDisp.Size = new System.Drawing.Size(392, 387);
            this.dgrdvCheckResultDisp.TabIndex = 39;
            this.dgrdvCheckResultDisp.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgrdvCheckResultDisp_CellDoubleClick);
            // 
            // rbtnElementResult
            // 
            this.rbtnElementResult.Font = new System.Drawing.Font("Times New Roman", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbtnElementResult.Location = new System.Drawing.Point(114, 24);
            this.rbtnElementResult.Name = "rbtnElementResult";
            this.rbtnElementResult.Size = new System.Drawing.Size(62, 26);
            this.rbtnElementResult.TabIndex = 40;
            this.rbtnElementResult.TabStop = true;
            this.rbtnElementResult.Text = "元件";
            this.rbtnElementResult.UseVisualStyleBackColor = true;
            this.rbtnElementResult.Click += new System.EventHandler(this.InspectResultDispKindRadioButtonClickEvent);
            // 
            // rbtnPCSResult
            // 
            this.rbtnPCSResult.Font = new System.Drawing.Font("Times New Roman", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbtnPCSResult.Location = new System.Drawing.Point(11, 24);
            this.rbtnPCSResult.Name = "rbtnPCSResult";
            this.rbtnPCSResult.Size = new System.Drawing.Size(62, 26);
            this.rbtnPCSResult.TabIndex = 42;
            this.rbtnPCSResult.TabStop = true;
            this.rbtnPCSResult.Text = "PCS";
            this.rbtnPCSResult.UseVisualStyleBackColor = true;
            this.rbtnPCSResult.Click += new System.EventHandler(this.InspectResultDispKindRadioButtonClickEvent);
            // 
            // cmsSearchResultDispRightClick
            // 
            this.cmsSearchResultDispRightClick.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.删除ToolStripMenuItem});
            this.cmsSearchResultDispRightClick.Name = "cmsSearchResultDispRightClick";
            this.cmsSearchResultDispRightClick.Size = new System.Drawing.Size(153, 48);
            // 
            // 删除ToolStripMenuItem
            // 
            this.删除ToolStripMenuItem.Name = "删除ToolStripMenuItem";
            this.删除ToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.删除ToolStripMenuItem.Text = "删除";
            this.删除ToolStripMenuItem.Click += new System.EventHandler(this.删除ToolStripMenuItem_Click);
            // 
            // lblAccessLevelTitle
            // 
            this.lblAccessLevelTitle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblAccessLevelTitle.BackColor = System.Drawing.SystemColors.Control;
            this.lblAccessLevelTitle.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblAccessLevelTitle.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAccessLevelTitle.ForeColor = System.Drawing.Color.Black;
            this.lblAccessLevelTitle.Image = ((System.Drawing.Image)(resources.GetObject("lblAccessLevelTitle.Image")));
            this.lblAccessLevelTitle.Location = new System.Drawing.Point(1461, 23);
            this.lblAccessLevelTitle.Name = "lblAccessLevelTitle";
            this.lblAccessLevelTitle.Size = new System.Drawing.Size(51, 42);
            this.lblAccessLevelTitle.TabIndex = 44;
            this.lblAccessLevelTitle.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblAccessLevelTitle.Click += new System.EventHandler(this.lblAccessLevel_Click);
            // 
            // lblUserName
            // 
            this.lblUserName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblUserName.BackColor = System.Drawing.SystemColors.Control;
            this.lblUserName.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUserName.ForeColor = System.Drawing.Color.Black;
            this.lblUserName.Location = new System.Drawing.Point(1511, 43);
            this.lblUserName.Name = "lblUserName";
            this.lblUserName.Size = new System.Drawing.Size(118, 21);
            this.lblUserName.TabIndex = 45;
            this.lblUserName.Text = "3DAOI";
            this.lblUserName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblUserName.Click += new System.EventHandler(this.lblAccessLevel_Click);
            // 
            // lblAccessLevel
            // 
            this.lblAccessLevel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblAccessLevel.BackColor = System.Drawing.SystemColors.Control;
            this.lblAccessLevel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblAccessLevel.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAccessLevel.ForeColor = System.Drawing.Color.Black;
            this.lblAccessLevel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblAccessLevel.Location = new System.Drawing.Point(1511, 23);
            this.lblAccessLevel.Name = "lblAccessLevel";
            this.lblAccessLevel.Size = new System.Drawing.Size(120, 42);
            this.lblAccessLevel.TabIndex = 46;
            this.lblAccessLevel.Text = "操作员";
            this.lblAccessLevel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.lblAccessLevel.Click += new System.EventHandler(this.lblAccessLevel_Click);
            // 
            // pnlSearching
            // 
            this.pnlSearching.Controls.Add(this.lblSearching);
            this.pnlSearching.Location = new System.Drawing.Point(279, 444);
            this.pnlSearching.Name = "pnlSearching";
            this.pnlSearching.Size = new System.Drawing.Size(658, 231);
            this.pnlSearching.TabIndex = 47;
            // 
            // lblSearching
            // 
            this.lblSearching.Font = new System.Drawing.Font("KaiTi", 72F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblSearching.Location = new System.Drawing.Point(77, 32);
            this.lblSearching.Name = "lblSearching";
            this.lblSearching.Size = new System.Drawing.Size(530, 169);
            this.lblSearching.TabIndex = 0;
            this.lblSearching.Text = "搜索中...";
            this.lblSearching.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ThreeDAVIInspectHistoryBrowseMainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1642, 971);
            this.Controls.Add(this.pnlSearching);
            this.Controls.Add(this.lblAccessLevelTitle);
            this.Controls.Add(this.lblUserName);
            this.Controls.Add(this.lblAccessLevel);
            this.Controls.Add(this.gbxInspectResultDisp);
            this.Controls.Add(this.gbxPassRate);
            this.Controls.Add(this.lblCurrentTime);
            this.Controls.Add(this.lblSearchResultQuantity);
            this.Controls.Add(this.lblInpsectResult);
            this.Controls.Add(this.btnSearchKindNGProduct);
            this.Controls.Add(this.btnSearchKindOKProduct);
            this.Controls.Add(this.dgrdvSearchResultDisp);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.gbxPathAndFilter);
            this.Controls.Add(this.gbxSearchRegion);
            this.Font = new System.Drawing.Font("KaiTi", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(5);
            this.Name = "ThreeDAVIInspectHistoryBrowseMainForm";
            this.Text = "SPC";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ThreeDAVIInspectHistoryBrowseMainForm_FormClosing);
            this.Load += new System.EventHandler(this.ThreeDAVIInspectHistoryBrowseMainForm_Load);
            this.gbxSearchRegion.ResumeLayout(false);
            this.gbxSearchRegion.PerformLayout();
            this.gbxPathAndFilter.ResumeLayout(false);
            this.gbxPathAndFilter.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgrdvSearchResultDisp)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgrdvPassRateRecord)).EndInit();
            this.gbxPassRate.ResumeLayout(false);
            this.gbxInspectResultDisp.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgrdvCheckResultDisp)).EndInit();
            this.cmsSearchResultDispRightClick.ResumeLayout(false);
            this.pnlSearching.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbxSearchRegion;
        private System.Windows.Forms.RadioButton rbtnSearchRegionOneDay;
        private System.Windows.Forms.Label lblSearchStartTimeTitle;
        private System.Windows.Forms.RadioButton rbtnSearchRegionSettime;
        private System.Windows.Forms.RadioButton rbtnSearchRegionOneMonth;
        private System.Windows.Forms.RadioButton rbtnSearchRegionOneWeek;
        private System.Windows.Forms.DateTimePicker dtpSearchStartHourTime;
        private System.Windows.Forms.DateTimePicker dtpSearchStartDateTime;
        private System.Windows.Forms.DateTimePicker dtpSearchStopHourTime;
        private System.Windows.Forms.DateTimePicker dtpSearchStopDateTime;
        private System.Windows.Forms.Label lblSearchStopTimeTitle;
        private System.Windows.Forms.Button btnBrowseNormalProductCheckResultSavePath;
        public System.Windows.Forms.TextBox txtNormalProductCheckResultSavePath;
        private System.Windows.Forms.GroupBox gbxPathAndFilter;
        public System.Windows.Forms.TextBox txtSamplePanelCheckResultSavePath;
        private System.Windows.Forms.Button btnBrowseSamplePanelCheckResultSavePath;
        private System.Windows.Forms.ComboBox cboProductName;
        private System.Windows.Forms.CheckBox chkProductNameFilter;
        private System.Windows.Forms.CheckBox chkBarcodeFilter;
        private System.Windows.Forms.TextBox txtBarcodeFilterBarcode;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.DataGridView dgrdvSearchResultDisp;
        private System.Windows.Forms.Button btnSearchKindOKProduct;
        private System.Windows.Forms.Button btnSearchKindNGProduct;
        private System.Windows.Forms.Label lblInpsectResult;
        private System.Windows.Forms.Label lblSearchResultQuantity;
        private System.Windows.Forms.Timer tmrRefreshTime;
        private System.Windows.Forms.Label lblCurrentTime;
        private System.Windows.Forms.RadioButton rbtnElementYield;
        private System.Windows.Forms.RadioButton rbtnPanelYield;
        private System.Windows.Forms.RadioButton rbtnPcsYield;
        private System.Windows.Forms.DataGridView dgrdvPassRateRecord;
        private System.Windows.Forms.GroupBox gbxPassRate;
        private System.Windows.Forms.DataGridViewTextBoxColumn yieldProjectName;
        private System.Windows.Forms.DataGridViewTextBoxColumn totalYield;
        private System.Windows.Forms.GroupBox gbxInspectResultDisp;
        private System.Windows.Forms.DataGridView dgrdvCheckResultDisp;
        private System.Windows.Forms.RadioButton rbtnElementResult;
        private System.Windows.Forms.RadioButton rbtnPCSResult;
        private System.Windows.Forms.Label lblSelectedProductName;
        private System.Windows.Forms.DataGridViewTextBoxColumn Number;
        private System.Windows.Forms.DataGridViewTextBoxColumn ProductName;
        private System.Windows.Forms.DataGridViewTextBoxColumn BarcodeData;
        private System.Windows.Forms.DataGridViewTextBoxColumn InspectStartTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn InspectStopTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn InspectTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn PassRate;
        private System.Windows.Forms.DataGridViewTextBoxColumn Path;
        private System.Windows.Forms.ContextMenuStrip cmsSearchResultDispRightClick;
        private System.Windows.Forms.ToolStripMenuItem 删除ToolStripMenuItem;
        private System.Windows.Forms.CheckBox chkCheckSampleProduct;
        private System.Windows.Forms.CheckBox chkCheckNormalProduct;
        private System.Windows.Forms.Label lblAccessLevelTitle;
        private System.Windows.Forms.Label lblUserName;
        private System.Windows.Forms.Label lblAccessLevel;
        private System.Windows.Forms.Panel pnlSearching;
        private System.Windows.Forms.Label lblSearching;
    }
}

