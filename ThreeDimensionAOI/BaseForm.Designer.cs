using System.Windows.Forms;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System;
namespace ThreeDimensionAVI
{
    partial class BaseForm
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
            bool disposeFlag = false;

            if (!_closeSoftWithOutInfoFlag)
            {
                if (MessageBox.Show("是否确认退出软件！", "退出软件", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
                {
                    disposeFlag = true;
                }
                else
                {
                    disposeFlag = false;
                }
            }
            else
                disposeFlag = true;
           if(disposeFlag)
            {
                MyTool.TxtFileProcess.CreateLog("退出软件！" + "----用户：" + _operatorName,
                BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                #region 停止线程
                if (_deviceActionThread != null)
                {
                    _deviceActionThread.Abort();
                    _deviceActionThread = null;
                }

                if (_stateAndAlarmDispTimer != null)
                {
                    _stateAndAlarmDispTimer.Dispose();
                    _stateAndAlarmDispTimer = null;
                }
                Thread.Sleep(100);

                if (_formUpdateThread != null)
                {
                    _formUpdateThread.Abort();
                    _formUpdateThread = null;
                }

                if (_timerUseInSoftCountThread != null)
                {
                    _timerUseInSoftCountThread.Abort();
                    _timerUseInSoftCountThread = null;
                }
                Thread.Sleep(100);

               if(_cameraImageGrabAndDispThread !=null)
               {
                _cameraImageGrabAndDispThread.Abort();
                   _cameraImageGrabAndDispThread = null;
               }
               if (_leftCameraCs.hv_AcqHandle != null)
                   _leftCameraCs.CloseCamera();

               if (_rightCameraCs.hv_AcqHandle != null)
                   _rightCameraCs.CloseCamera();

               if (_backCameraCs.hv_AcqHandle != null)
                   _backCameraCs.CloseCamera();

                #endregion

                #region 启动参数保存

                if (_baseSettingFormVariate != null
                    && _baseSettingFormVariate._settingsGeneralFormVariate != null
                    && _baseSettingFormVariate._settingsAdminFormVariate != null
                    && _baseSettingFormVariate._settingsProductFormVariate != null)
                {
                    _baseSettingFormVariate._settingsGeneralFormVariate.SavingDataToXmlFile();
                    _baseSettingFormVariate._settingsAdminFormVariate.SavingDataToXmlFile();
                    _baseSettingFormVariate._settingsProductFormVariate.MainFormDataSavingSettingData();
                }

                #endregion

                if (_mainFormVariate != null)
                {
                    _mainFormVariate.Dispose();
                    _mainFormVariate = null;
                }

                if (_baseSettingFormVariate != null)
                {
                    _baseSettingFormVariate.Dispose();
                    _baseSettingFormVariate = null;
                }

                if (_axisControlFormVariate != null)
                {
                    _axisControlFormVariate.Dispose();
                    _axisControlFormVariate = null;
                }

                if (_alarmFormVariate != null)
                {
                    _alarmFormVariate.Dispose();
                    _alarmFormVariate = null;
                }

               //关闭串口
                if (_newDAM02AIAOOperation.comm.IsOpen)
                {
                    _newDAM02AIAOOperation.comm.Close();
                }

                if (disposing && (components != null))
                {
                    components.Dispose();
                }
                try
                {
                    base.Dispose(disposing);
                }
                catch
                { }

                if (_SPCFormVariate != null)
                {
                    _SPCFormVariate.Dispose();
                    _SPCFormVariate = null;
                }
            }
          
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BaseForm));
            this.pnlChildFormDisplay = new System.Windows.Forms.Panel();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.lblTime = new System.Windows.Forms.Label();
            this.lblDeviceStatus = new System.Windows.Forms.Label();
            this.lblMotionCardStatus = new System.Windows.Forms.Label();
            this.lblAxisCoordination = new System.Windows.Forms.Label();
            this.lblAccessLevel = new System.Windows.Forms.Label();
            this.tmrRefreshBottomLabel = new System.Windows.Forms.Timer(this.components);
            this.tctlInferResult = new System.Windows.Forms.TabControl();
            this.tpageConveyor1InferResult = new System.Windows.Forms.TabPage();
            this.dgrdvConveyor1InferResultDisp = new System.Windows.Forms.DataGridView();
            this.Conveyor1ElementNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Conveyor1ElementName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Conveyor1LeftResult = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Conveyor1RightResult = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Conveyor1ElementResult = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tpageConveyor2InferResult = new System.Windows.Forms.TabPage();
            this.dgrdvConveyor2InferResultDisp = new System.Windows.Forms.DataGridView();
            this.Conveyor2ElementNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Conveyor2ElementName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Conveyor2LeftResult = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Conveyor2RightResult = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Conveyor2ElementResult = new System.Windows.Forms.DataGridViewTextBoxColumn();
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
            this.tpageBackCameraReconginitionImage = new System.Windows.Forms.TabPage();
            this.lblConveyor2Mark2SearchResultTitle = new System.Windows.Forms.Label();
            this.lblConveyor2Mark1SearchResultTitle = new System.Windows.Forms.Label();
            this.lblConveyor1Mark2SearchResultTitle = new System.Windows.Forms.Label();
            this.lblConveyor1Mark1SearchResultTitle = new System.Windows.Forms.Label();
            this.lblConveyor2BarcodeScanResultImageTitle = new System.Windows.Forms.Label();
            this.lblConveyor1BarcodeScanResultImageTitle = new System.Windows.Forms.Label();
            this.picConveyor2Mark2SearchResultImageDisp = new HalconDotNet.HWindowControl();
            this.picConveyor2Mark1SearchResultImageDisp = new HalconDotNet.HWindowControl();
            this.picConveyor1Mark2SearchResultImageDisp = new HalconDotNet.HWindowControl();
            this.picConveyor2BarcodeScanResultImageDisp = new HalconDotNet.HWindowControl();
            this.picConveyor1Mark1SearchResultImageDisp = new HalconDotNet.HWindowControl();
            this.picConveyor1BarcodeScanResultImageDisp = new HalconDotNet.HWindowControl();
            this.dgrdvYieldRecord = new System.Windows.Forms.DataGridView();
            this.yieldProjectName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.totalYield = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.conveyor1Yield = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.conveyor2Yield = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pnlYieldRecord = new System.Windows.Forms.Panel();
            this.lblMarkSearchTime = new System.Windows.Forms.Label();
            this.lblConveyor2MeasurementUsingTime = new System.Windows.Forms.Label();
            this.lblConveyor1MeasurementUsingTime = new System.Windows.Forms.Label();
            this.btnClearYield = new System.Windows.Forms.Button();
            this.rbtnElementYield = new System.Windows.Forms.RadioButton();
            this.rbtnPanelYield = new System.Windows.Forms.RadioButton();
            this.rbtnPcsYield = new System.Windows.Forms.RadioButton();
            this.pnlOther = new System.Windows.Forms.Panel();
            this.btnSPCOpen = new System.Windows.Forms.Button();
            this.lblAccessLevelTitle = new System.Windows.Forms.Label();
            this.lblUserName = new System.Windows.Forms.Label();
            this.lblMESStatus = new System.Windows.Forms.Label();
            this.pnlChildFormDisplay.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tctlInferResult.SuspendLayout();
            this.tpageConveyor1InferResult.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgrdvConveyor1InferResultDisp)).BeginInit();
            this.tpageConveyor2InferResult.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgrdvConveyor2InferResultDisp)).BeginInit();
            this.tctlDeviceStatusRecord.SuspendLayout();
            this.tpageDeviceControlLOG.SuspendLayout();
            this.tpageMESLOG.SuspendLayout();
            this.tpageAlarmRecord.SuspendLayout();
            this.tpageDeviceStatus.SuspendLayout();
            this.tpageBackCameraReconginitionImage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgrdvYieldRecord)).BeginInit();
            this.pnlYieldRecord.SuspendLayout();
            this.pnlOther.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlChildFormDisplay
            // 
            this.pnlChildFormDisplay.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlChildFormDisplay.Controls.Add(this.tabControl1);
            this.pnlChildFormDisplay.Font = new System.Drawing.Font("Times New Roman", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pnlChildFormDisplay.Location = new System.Drawing.Point(1, 0);
            this.pnlChildFormDisplay.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.pnlChildFormDisplay.Name = "pnlChildFormDisplay";
            this.pnlChildFormDisplay.Size = new System.Drawing.Size(1317, 777);
            this.pnlChildFormDisplay.TabIndex = 4;
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Font = new System.Drawing.Font("Times New Roman", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControl1.Location = new System.Drawing.Point(1314, 494);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(290, 489);
            this.tabControl1.TabIndex = 7;
            // 
            // tabPage1
            // 
            this.tabPage1.Location = new System.Drawing.Point(4, 25);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(282, 460);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "传送线1判定结果";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(4, 25);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(282, 460);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "传送线2判定结果";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // lblTime
            // 
            this.lblTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTime.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblTime.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTime.Location = new System.Drawing.Point(864, 925);
            this.lblTime.Name = "lblTime";
            this.lblTime.Size = new System.Drawing.Size(114, 30);
            this.lblTime.TabIndex = 3;
            this.lblTime.Text = "2021/10/26 24:00:00";
            this.lblTime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblDeviceStatus
            // 
            this.lblDeviceStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDeviceStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblDeviceStatus.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDeviceStatus.Location = new System.Drawing.Point(252, 925);
            this.lblDeviceStatus.Name = "lblDeviceStatus";
            this.lblDeviceStatus.Size = new System.Drawing.Size(612, 30);
            this.lblDeviceStatus.TabIndex = 5;
            this.lblDeviceStatus.Text = "Wait press start buttom.";
            this.lblDeviceStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblDeviceStatus.DoubleClick += new System.EventHandler(this.Step_Status_Label_DoubleClick);
            // 
            // lblMotionCardStatus
            // 
            this.lblMotionCardStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblMotionCardStatus.BackColor = System.Drawing.Color.Red;
            this.lblMotionCardStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblMotionCardStatus.Font = new System.Drawing.Font("Times New Roman", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMotionCardStatus.ForeColor = System.Drawing.Color.White;
            this.lblMotionCardStatus.Location = new System.Drawing.Point(1, 925);
            this.lblMotionCardStatus.Name = "lblMotionCardStatus";
            this.lblMotionCardStatus.Size = new System.Drawing.Size(125, 30);
            this.lblMotionCardStatus.TabIndex = 5;
            this.lblMotionCardStatus.Text = "运动控制卡异常";
            this.lblMotionCardStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblMotionCardStatus.Click += new System.EventHandler(this.lblMotionCardStatus_Click);
            // 
            // lblAxisCoordination
            // 
            this.lblAxisCoordination.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblAxisCoordination.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblAxisCoordination.Font = new System.Drawing.Font("Times New Roman", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAxisCoordination.Location = new System.Drawing.Point(978, 925);
            this.lblAxisCoordination.Name = "lblAxisCoordination";
            this.lblAxisCoordination.Size = new System.Drawing.Size(659, 30);
            this.lblAxisCoordination.TabIndex = 5;
            this.lblAxisCoordination.Text = "X=999.99;Y=999.99;Z=999.99;O=999.99,    ABS:X=999.99;Y=999.99;Z=999.99;O=999.99;T" +
    "(O)=100.00Nm;T(R1)=100.00Nm";
            this.lblAxisCoordination.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblAccessLevel
            // 
            this.lblAccessLevel.BackColor = System.Drawing.SystemColors.Control;
            this.lblAccessLevel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblAccessLevel.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAccessLevel.ForeColor = System.Drawing.Color.Black;
            this.lblAccessLevel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblAccessLevel.Location = new System.Drawing.Point(54, 2);
            this.lblAccessLevel.Name = "lblAccessLevel";
            this.lblAccessLevel.Size = new System.Drawing.Size(133, 42);
            this.lblAccessLevel.TabIndex = 3;
            this.lblAccessLevel.Text = "操作员";
            this.lblAccessLevel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.lblAccessLevel.Click += new System.EventHandler(this.lblAccessLevel_Click);
            // 
            // tmrRefreshBottomLabel
            // 
            this.tmrRefreshBottomLabel.Interval = 1000;
            this.tmrRefreshBottomLabel.Tick += new System.EventHandler(this.tmrRefreshBottomLabel_Tick);
            // 
            // tctlInferResult
            // 
            this.tctlInferResult.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tctlInferResult.Controls.Add(this.tpageConveyor1InferResult);
            this.tctlInferResult.Controls.Add(this.tpageConveyor2InferResult);
            this.tctlInferResult.Font = new System.Drawing.Font("Times New Roman", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tctlInferResult.Location = new System.Drawing.Point(4, 47);
            this.tctlInferResult.Name = "tctlInferResult";
            this.tctlInferResult.SelectedIndex = 0;
            this.tctlInferResult.Size = new System.Drawing.Size(309, 458);
            this.tctlInferResult.TabIndex = 7;
            // 
            // tpageConveyor1InferResult
            // 
            this.tpageConveyor1InferResult.Controls.Add(this.dgrdvConveyor1InferResultDisp);
            this.tpageConveyor1InferResult.Location = new System.Drawing.Point(4, 25);
            this.tpageConveyor1InferResult.Name = "tpageConveyor1InferResult";
            this.tpageConveyor1InferResult.Padding = new System.Windows.Forms.Padding(3);
            this.tpageConveyor1InferResult.Size = new System.Drawing.Size(301, 429);
            this.tpageConveyor1InferResult.TabIndex = 0;
            this.tpageConveyor1InferResult.Text = "传送线1判定结果";
            this.tpageConveyor1InferResult.UseVisualStyleBackColor = true;
            // 
            // dgrdvConveyor1InferResultDisp
            // 
            this.dgrdvConveyor1InferResultDisp.AllowUserToAddRows = false;
            this.dgrdvConveyor1InferResultDisp.AllowUserToDeleteRows = false;
            this.dgrdvConveyor1InferResultDisp.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Times New Roman", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgrdvConveyor1InferResultDisp.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgrdvConveyor1InferResultDisp.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgrdvConveyor1InferResultDisp.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Conveyor1ElementNumber,
            this.Conveyor1ElementName,
            this.Conveyor1LeftResult,
            this.Conveyor1RightResult,
            this.Conveyor1ElementResult});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Times New Roman", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgrdvConveyor1InferResultDisp.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgrdvConveyor1InferResultDisp.Location = new System.Drawing.Point(6, 7);
            this.dgrdvConveyor1InferResultDisp.Name = "dgrdvConveyor1InferResultDisp";
            this.dgrdvConveyor1InferResultDisp.ReadOnly = true;
            this.dgrdvConveyor1InferResultDisp.RowHeadersVisible = false;
            this.dgrdvConveyor1InferResultDisp.RowTemplate.Height = 23;
            this.dgrdvConveyor1InferResultDisp.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgrdvConveyor1InferResultDisp.Size = new System.Drawing.Size(288, 416);
            this.dgrdvConveyor1InferResultDisp.TabIndex = 0;
            // 
            // Conveyor1ElementNumber
            // 
            this.Conveyor1ElementNumber.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Conveyor1ElementNumber.FillWeight = 15F;
            this.Conveyor1ElementNumber.HeaderText = "序号";
            this.Conveyor1ElementNumber.Name = "Conveyor1ElementNumber";
            this.Conveyor1ElementNumber.ReadOnly = true;
            // 
            // Conveyor1ElementName
            // 
            this.Conveyor1ElementName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Conveyor1ElementName.FillWeight = 15F;
            this.Conveyor1ElementName.HeaderText = "名称";
            this.Conveyor1ElementName.Name = "Conveyor1ElementName";
            this.Conveyor1ElementName.ReadOnly = true;
            // 
            // Conveyor1LeftResult
            // 
            this.Conveyor1LeftResult.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Conveyor1LeftResult.FillWeight = 23F;
            this.Conveyor1LeftResult.HeaderText = "左侧结果";
            this.Conveyor1LeftResult.Name = "Conveyor1LeftResult";
            this.Conveyor1LeftResult.ReadOnly = true;
            // 
            // Conveyor1RightResult
            // 
            this.Conveyor1RightResult.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Conveyor1RightResult.FillWeight = 23F;
            this.Conveyor1RightResult.HeaderText = "右侧结果";
            this.Conveyor1RightResult.Name = "Conveyor1RightResult";
            this.Conveyor1RightResult.ReadOnly = true;
            // 
            // Conveyor1ElementResult
            // 
            this.Conveyor1ElementResult.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Conveyor1ElementResult.FillWeight = 23F;
            this.Conveyor1ElementResult.HeaderText = "元件结果";
            this.Conveyor1ElementResult.Name = "Conveyor1ElementResult";
            this.Conveyor1ElementResult.ReadOnly = true;
            // 
            // tpageConveyor2InferResult
            // 
            this.tpageConveyor2InferResult.Controls.Add(this.dgrdvConveyor2InferResultDisp);
            this.tpageConveyor2InferResult.Location = new System.Drawing.Point(4, 25);
            this.tpageConveyor2InferResult.Name = "tpageConveyor2InferResult";
            this.tpageConveyor2InferResult.Padding = new System.Windows.Forms.Padding(3);
            this.tpageConveyor2InferResult.Size = new System.Drawing.Size(301, 429);
            this.tpageConveyor2InferResult.TabIndex = 1;
            this.tpageConveyor2InferResult.Text = "传送线2判定结果";
            this.tpageConveyor2InferResult.UseVisualStyleBackColor = true;
            // 
            // dgrdvConveyor2InferResultDisp
            // 
            this.dgrdvConveyor2InferResultDisp.AllowUserToAddRows = false;
            this.dgrdvConveyor2InferResultDisp.AllowUserToDeleteRows = false;
            this.dgrdvConveyor2InferResultDisp.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Times New Roman", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgrdvConveyor2InferResultDisp.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dgrdvConveyor2InferResultDisp.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgrdvConveyor2InferResultDisp.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Conveyor2ElementNumber,
            this.Conveyor2ElementName,
            this.Conveyor2LeftResult,
            this.Conveyor2RightResult,
            this.Conveyor2ElementResult});
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Times New Roman", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgrdvConveyor2InferResultDisp.DefaultCellStyle = dataGridViewCellStyle4;
            this.dgrdvConveyor2InferResultDisp.Location = new System.Drawing.Point(6, 7);
            this.dgrdvConveyor2InferResultDisp.Name = "dgrdvConveyor2InferResultDisp";
            this.dgrdvConveyor2InferResultDisp.ReadOnly = true;
            this.dgrdvConveyor2InferResultDisp.RowHeadersVisible = false;
            this.dgrdvConveyor2InferResultDisp.RowTemplate.Height = 23;
            this.dgrdvConveyor2InferResultDisp.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgrdvConveyor2InferResultDisp.Size = new System.Drawing.Size(300, 447);
            this.dgrdvConveyor2InferResultDisp.TabIndex = 1;
            // 
            // Conveyor2ElementNumber
            // 
            this.Conveyor2ElementNumber.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Conveyor2ElementNumber.FillWeight = 15F;
            this.Conveyor2ElementNumber.HeaderText = "序号";
            this.Conveyor2ElementNumber.Name = "Conveyor2ElementNumber";
            this.Conveyor2ElementNumber.ReadOnly = true;
            // 
            // Conveyor2ElementName
            // 
            this.Conveyor2ElementName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Conveyor2ElementName.FillWeight = 15F;
            this.Conveyor2ElementName.HeaderText = "名称";
            this.Conveyor2ElementName.Name = "Conveyor2ElementName";
            this.Conveyor2ElementName.ReadOnly = true;
            // 
            // Conveyor2LeftResult
            // 
            this.Conveyor2LeftResult.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Conveyor2LeftResult.FillWeight = 23F;
            this.Conveyor2LeftResult.HeaderText = "左侧结果";
            this.Conveyor2LeftResult.Name = "Conveyor2LeftResult";
            this.Conveyor2LeftResult.ReadOnly = true;
            // 
            // Conveyor2RightResult
            // 
            this.Conveyor2RightResult.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Conveyor2RightResult.FillWeight = 23F;
            this.Conveyor2RightResult.HeaderText = "右侧结果";
            this.Conveyor2RightResult.Name = "Conveyor2RightResult";
            this.Conveyor2RightResult.ReadOnly = true;
            // 
            // Conveyor2ElementResult
            // 
            this.Conveyor2ElementResult.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Conveyor2ElementResult.FillWeight = 23F;
            this.Conveyor2ElementResult.HeaderText = "元件结果";
            this.Conveyor2ElementResult.Name = "Conveyor2ElementResult";
            this.Conveyor2ElementResult.ReadOnly = true;
            // 
            // tctlDeviceStatusRecord
            // 
            this.tctlDeviceStatusRecord.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.tctlDeviceStatusRecord.Controls.Add(this.tpageDeviceControlLOG);
            this.tctlDeviceStatusRecord.Controls.Add(this.tpageMESLOG);
            this.tctlDeviceStatusRecord.Controls.Add(this.tpageAlarmRecord);
            this.tctlDeviceStatusRecord.Controls.Add(this.tpageDeviceStatus);
            this.tctlDeviceStatusRecord.Controls.Add(this.tpageBackCameraReconginitionImage);
            this.tctlDeviceStatusRecord.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tctlDeviceStatusRecord.Location = new System.Drawing.Point(3, 778);
            this.tctlDeviceStatusRecord.Name = "tctlDeviceStatusRecord";
            this.tctlDeviceStatusRecord.SelectedIndex = 0;
            this.tctlDeviceStatusRecord.Size = new System.Drawing.Size(1315, 144);
            this.tctlDeviceStatusRecord.TabIndex = 7;
            // 
            // tpageDeviceControlLOG
            // 
            this.tpageDeviceControlLOG.BackColor = System.Drawing.SystemColors.Control;
            this.tpageDeviceControlLOG.Controls.Add(this.lvDeviceControlLogDisp);
            this.tpageDeviceControlLOG.Location = new System.Drawing.Point(4, 24);
            this.tpageDeviceControlLOG.Name = "tpageDeviceControlLOG";
            this.tpageDeviceControlLOG.Padding = new System.Windows.Forms.Padding(3);
            this.tpageDeviceControlLOG.Size = new System.Drawing.Size(1307, 116);
            this.tpageDeviceControlLOG.TabIndex = 0;
            this.tpageDeviceControlLOG.Text = "设备控制日志";
            // 
            // lvDeviceControlLogDisp
            // 
            this.lvDeviceControlLogDisp.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lvDeviceControlLogDisp.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Info});
            this.lvDeviceControlLogDisp.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lvDeviceControlLogDisp.FullRowSelect = true;
            this.lvDeviceControlLogDisp.GridLines = true;
            this.lvDeviceControlLogDisp.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lvDeviceControlLogDisp.HideSelection = false;
            this.lvDeviceControlLogDisp.Location = new System.Drawing.Point(0, 1);
            this.lvDeviceControlLogDisp.MultiSelect = false;
            this.lvDeviceControlLogDisp.Name = "lvDeviceControlLogDisp";
            this.lvDeviceControlLogDisp.Size = new System.Drawing.Size(1307, 115);
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
            this.tpageMESLOG.Size = new System.Drawing.Size(1307, 116);
            this.tpageMESLOG.TabIndex = 3;
            this.tpageMESLOG.Text = "MES通讯日志";
            // 
            // lvMESLogDisp
            // 
            this.lvMESLogDisp.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lvMESLogDisp.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader2});
            this.lvMESLogDisp.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lvMESLogDisp.FullRowSelect = true;
            this.lvMESLogDisp.GridLines = true;
            this.lvMESLogDisp.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lvMESLogDisp.HideSelection = false;
            this.lvMESLogDisp.Location = new System.Drawing.Point(0, 1);
            this.lvMESLogDisp.MultiSelect = false;
            this.lvMESLogDisp.Name = "lvMESLogDisp";
            this.lvMESLogDisp.Size = new System.Drawing.Size(1307, 115);
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
            this.tpageAlarmRecord.Size = new System.Drawing.Size(1307, 116);
            this.tpageAlarmRecord.TabIndex = 1;
            this.tpageAlarmRecord.Text = "报警记录";
            // 
            // lvAlarmDisp
            // 
            this.lvAlarmDisp.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lvAlarmDisp.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.lvAlarmDisp.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lvAlarmDisp.FullRowSelect = true;
            this.lvAlarmDisp.GridLines = true;
            this.lvAlarmDisp.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lvAlarmDisp.HideSelection = false;
            this.lvAlarmDisp.Location = new System.Drawing.Point(0, 1);
            this.lvAlarmDisp.MultiSelect = false;
            this.lvAlarmDisp.Name = "lvAlarmDisp";
            this.lvAlarmDisp.Size = new System.Drawing.Size(1307, 115);
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
            this.tpageDeviceStatus.Size = new System.Drawing.Size(1307, 116);
            this.tpageDeviceStatus.TabIndex = 2;
            this.tpageDeviceStatus.Text = "设备状态";
            // 
            // lvDeviceStatus
            // 
            this.lvDeviceStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lvDeviceStatus.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.VariateName,
            this.VariateData});
            this.lvDeviceStatus.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lvDeviceStatus.FullRowSelect = true;
            this.lvDeviceStatus.GridLines = true;
            this.lvDeviceStatus.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvDeviceStatus.HideSelection = false;
            this.lvDeviceStatus.Location = new System.Drawing.Point(0, 1);
            this.lvDeviceStatus.MultiSelect = false;
            this.lvDeviceStatus.Name = "lvDeviceStatus";
            this.lvDeviceStatus.Size = new System.Drawing.Size(1307, 115);
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
            // tpageBackCameraReconginitionImage
            // 
            this.tpageBackCameraReconginitionImage.Controls.Add(this.lblConveyor2Mark2SearchResultTitle);
            this.tpageBackCameraReconginitionImage.Controls.Add(this.lblConveyor2Mark1SearchResultTitle);
            this.tpageBackCameraReconginitionImage.Controls.Add(this.lblConveyor1Mark2SearchResultTitle);
            this.tpageBackCameraReconginitionImage.Controls.Add(this.lblConveyor1Mark1SearchResultTitle);
            this.tpageBackCameraReconginitionImage.Controls.Add(this.lblConveyor2BarcodeScanResultImageTitle);
            this.tpageBackCameraReconginitionImage.Controls.Add(this.lblConveyor1BarcodeScanResultImageTitle);
            this.tpageBackCameraReconginitionImage.Controls.Add(this.picConveyor2Mark2SearchResultImageDisp);
            this.tpageBackCameraReconginitionImage.Controls.Add(this.picConveyor2Mark1SearchResultImageDisp);
            this.tpageBackCameraReconginitionImage.Controls.Add(this.picConveyor1Mark2SearchResultImageDisp);
            this.tpageBackCameraReconginitionImage.Controls.Add(this.picConveyor2BarcodeScanResultImageDisp);
            this.tpageBackCameraReconginitionImage.Controls.Add(this.picConveyor1Mark1SearchResultImageDisp);
            this.tpageBackCameraReconginitionImage.Controls.Add(this.picConveyor1BarcodeScanResultImageDisp);
            this.tpageBackCameraReconginitionImage.Location = new System.Drawing.Point(4, 24);
            this.tpageBackCameraReconginitionImage.Name = "tpageBackCameraReconginitionImage";
            this.tpageBackCameraReconginitionImage.Padding = new System.Windows.Forms.Padding(3);
            this.tpageBackCameraReconginitionImage.Size = new System.Drawing.Size(1307, 116);
            this.tpageBackCameraReconginitionImage.TabIndex = 4;
            this.tpageBackCameraReconginitionImage.Text = "后相机识别图像";
            this.tpageBackCameraReconginitionImage.UseVisualStyleBackColor = true;
            // 
            // lblConveyor2Mark2SearchResultTitle
            // 
            this.lblConveyor2Mark2SearchResultTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lblConveyor2Mark2SearchResultTitle.Location = new System.Drawing.Point(1089, 2);
            this.lblConveyor2Mark2SearchResultTitle.Name = "lblConveyor2Mark2SearchResultTitle";
            this.lblConveyor2Mark2SearchResultTitle.Size = new System.Drawing.Size(63, 112);
            this.lblConveyor2Mark2SearchResultTitle.TabIndex = 63;
            this.lblConveyor2Mark2SearchResultTitle.Text = "传送线2 Mark点2识别图像";
            this.lblConveyor2Mark2SearchResultTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblConveyor2Mark1SearchResultTitle
            // 
            this.lblConveyor2Mark1SearchResultTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lblConveyor2Mark1SearchResultTitle.Location = new System.Drawing.Point(871, 2);
            this.lblConveyor2Mark1SearchResultTitle.Name = "lblConveyor2Mark1SearchResultTitle";
            this.lblConveyor2Mark1SearchResultTitle.Size = new System.Drawing.Size(63, 112);
            this.lblConveyor2Mark1SearchResultTitle.TabIndex = 63;
            this.lblConveyor2Mark1SearchResultTitle.Text = "传送线2 Mark点1识别图像";
            this.lblConveyor2Mark1SearchResultTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblConveyor1Mark2SearchResultTitle
            // 
            this.lblConveyor1Mark2SearchResultTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lblConveyor1Mark2SearchResultTitle.Location = new System.Drawing.Point(435, 1);
            this.lblConveyor1Mark2SearchResultTitle.Name = "lblConveyor1Mark2SearchResultTitle";
            this.lblConveyor1Mark2SearchResultTitle.Size = new System.Drawing.Size(63, 112);
            this.lblConveyor1Mark2SearchResultTitle.TabIndex = 63;
            this.lblConveyor1Mark2SearchResultTitle.Text = "传送线1 Mark点2识别图像";
            this.lblConveyor1Mark2SearchResultTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblConveyor1Mark1SearchResultTitle
            // 
            this.lblConveyor1Mark1SearchResultTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lblConveyor1Mark1SearchResultTitle.Location = new System.Drawing.Point(217, 1);
            this.lblConveyor1Mark1SearchResultTitle.Name = "lblConveyor1Mark1SearchResultTitle";
            this.lblConveyor1Mark1SearchResultTitle.Size = new System.Drawing.Size(63, 112);
            this.lblConveyor1Mark1SearchResultTitle.TabIndex = 63;
            this.lblConveyor1Mark1SearchResultTitle.Text = "传送线1 Mark点1识别图像";
            this.lblConveyor1Mark1SearchResultTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblConveyor2BarcodeScanResultImageTitle
            // 
            this.lblConveyor2BarcodeScanResultImageTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lblConveyor2BarcodeScanResultImageTitle.Location = new System.Drawing.Point(657, 1);
            this.lblConveyor2BarcodeScanResultImageTitle.Name = "lblConveyor2BarcodeScanResultImageTitle";
            this.lblConveyor2BarcodeScanResultImageTitle.Size = new System.Drawing.Size(58, 112);
            this.lblConveyor2BarcodeScanResultImageTitle.TabIndex = 63;
            this.lblConveyor2BarcodeScanResultImageTitle.Text = "传送线2二维码识别图像";
            this.lblConveyor2BarcodeScanResultImageTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblConveyor1BarcodeScanResultImageTitle
            // 
            this.lblConveyor1BarcodeScanResultImageTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lblConveyor1BarcodeScanResultImageTitle.Location = new System.Drawing.Point(3, 1);
            this.lblConveyor1BarcodeScanResultImageTitle.Name = "lblConveyor1BarcodeScanResultImageTitle";
            this.lblConveyor1BarcodeScanResultImageTitle.Size = new System.Drawing.Size(58, 112);
            this.lblConveyor1BarcodeScanResultImageTitle.TabIndex = 63;
            this.lblConveyor1BarcodeScanResultImageTitle.Text = "传送线1二维码识别图像";
            this.lblConveyor1BarcodeScanResultImageTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // picConveyor2Mark2SearchResultImageDisp
            // 
            this.picConveyor2Mark2SearchResultImageDisp.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.picConveyor2Mark2SearchResultImageDisp.BackColor = System.Drawing.SystemColors.ControlDark;
            this.picConveyor2Mark2SearchResultImageDisp.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.picConveyor2Mark2SearchResultImageDisp.ImagePart = new System.Drawing.Rectangle(0, 0, 640, 480);
            this.picConveyor2Mark2SearchResultImageDisp.Location = new System.Drawing.Point(1154, 1);
            this.picConveyor2Mark2SearchResultImageDisp.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.picConveyor2Mark2SearchResultImageDisp.Name = "picConveyor2Mark2SearchResultImageDisp";
            this.picConveyor2Mark2SearchResultImageDisp.Size = new System.Drawing.Size(151, 113);
            this.picConveyor2Mark2SearchResultImageDisp.TabIndex = 62;
            this.picConveyor2Mark2SearchResultImageDisp.WindowSize = new System.Drawing.Size(151, 113);
            // 
            // picConveyor2Mark1SearchResultImageDisp
            // 
            this.picConveyor2Mark1SearchResultImageDisp.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.picConveyor2Mark1SearchResultImageDisp.BackColor = System.Drawing.SystemColors.ControlDark;
            this.picConveyor2Mark1SearchResultImageDisp.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.picConveyor2Mark1SearchResultImageDisp.ImagePart = new System.Drawing.Rectangle(0, 0, 640, 480);
            this.picConveyor2Mark1SearchResultImageDisp.Location = new System.Drawing.Point(936, 1);
            this.picConveyor2Mark1SearchResultImageDisp.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.picConveyor2Mark1SearchResultImageDisp.Name = "picConveyor2Mark1SearchResultImageDisp";
            this.picConveyor2Mark1SearchResultImageDisp.Size = new System.Drawing.Size(151, 113);
            this.picConveyor2Mark1SearchResultImageDisp.TabIndex = 62;
            this.picConveyor2Mark1SearchResultImageDisp.WindowSize = new System.Drawing.Size(151, 113);
            // 
            // picConveyor1Mark2SearchResultImageDisp
            // 
            this.picConveyor1Mark2SearchResultImageDisp.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.picConveyor1Mark2SearchResultImageDisp.BackColor = System.Drawing.SystemColors.ControlDark;
            this.picConveyor1Mark2SearchResultImageDisp.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.picConveyor1Mark2SearchResultImageDisp.ImagePart = new System.Drawing.Rectangle(0, 0, 640, 480);
            this.picConveyor1Mark2SearchResultImageDisp.Location = new System.Drawing.Point(500, 1);
            this.picConveyor1Mark2SearchResultImageDisp.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.picConveyor1Mark2SearchResultImageDisp.Name = "picConveyor1Mark2SearchResultImageDisp";
            this.picConveyor1Mark2SearchResultImageDisp.Size = new System.Drawing.Size(151, 113);
            this.picConveyor1Mark2SearchResultImageDisp.TabIndex = 62;
            this.picConveyor1Mark2SearchResultImageDisp.WindowSize = new System.Drawing.Size(151, 113);
            // 
            // picConveyor2BarcodeScanResultImageDisp
            // 
            this.picConveyor2BarcodeScanResultImageDisp.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.picConveyor2BarcodeScanResultImageDisp.BackColor = System.Drawing.SystemColors.ControlDark;
            this.picConveyor2BarcodeScanResultImageDisp.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.picConveyor2BarcodeScanResultImageDisp.ImagePart = new System.Drawing.Rectangle(0, 0, 640, 480);
            this.picConveyor2BarcodeScanResultImageDisp.Location = new System.Drawing.Point(718, 1);
            this.picConveyor2BarcodeScanResultImageDisp.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.picConveyor2BarcodeScanResultImageDisp.Name = "picConveyor2BarcodeScanResultImageDisp";
            this.picConveyor2BarcodeScanResultImageDisp.Size = new System.Drawing.Size(151, 113);
            this.picConveyor2BarcodeScanResultImageDisp.TabIndex = 62;
            this.picConveyor2BarcodeScanResultImageDisp.WindowSize = new System.Drawing.Size(151, 113);
            // 
            // picConveyor1Mark1SearchResultImageDisp
            // 
            this.picConveyor1Mark1SearchResultImageDisp.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.picConveyor1Mark1SearchResultImageDisp.BackColor = System.Drawing.SystemColors.ControlDark;
            this.picConveyor1Mark1SearchResultImageDisp.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.picConveyor1Mark1SearchResultImageDisp.ImagePart = new System.Drawing.Rectangle(0, 0, 640, 480);
            this.picConveyor1Mark1SearchResultImageDisp.Location = new System.Drawing.Point(282, 1);
            this.picConveyor1Mark1SearchResultImageDisp.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.picConveyor1Mark1SearchResultImageDisp.Name = "picConveyor1Mark1SearchResultImageDisp";
            this.picConveyor1Mark1SearchResultImageDisp.Size = new System.Drawing.Size(151, 113);
            this.picConveyor1Mark1SearchResultImageDisp.TabIndex = 62;
            this.picConveyor1Mark1SearchResultImageDisp.WindowSize = new System.Drawing.Size(151, 113);
            // 
            // picConveyor1BarcodeScanResultImageDisp
            // 
            this.picConveyor1BarcodeScanResultImageDisp.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.picConveyor1BarcodeScanResultImageDisp.BackColor = System.Drawing.SystemColors.ControlDark;
            this.picConveyor1BarcodeScanResultImageDisp.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.picConveyor1BarcodeScanResultImageDisp.ImagePart = new System.Drawing.Rectangle(0, 0, 640, 480);
            this.picConveyor1BarcodeScanResultImageDisp.Location = new System.Drawing.Point(64, 1);
            this.picConveyor1BarcodeScanResultImageDisp.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.picConveyor1BarcodeScanResultImageDisp.Name = "picConveyor1BarcodeScanResultImageDisp";
            this.picConveyor1BarcodeScanResultImageDisp.Size = new System.Drawing.Size(151, 113);
            this.picConveyor1BarcodeScanResultImageDisp.TabIndex = 62;
            this.picConveyor1BarcodeScanResultImageDisp.WindowSize = new System.Drawing.Size(151, 113);
            // 
            // dgrdvYieldRecord
            // 
            this.dgrdvYieldRecord.AllowUserToAddRows = false;
            this.dgrdvYieldRecord.AllowUserToDeleteRows = false;
            this.dgrdvYieldRecord.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgrdvYieldRecord.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle5;
            this.dgrdvYieldRecord.ColumnHeadersHeight = 50;
            this.dgrdvYieldRecord.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgrdvYieldRecord.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.yieldProjectName,
            this.totalYield,
            this.conveyor1Yield,
            this.conveyor2Yield});
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgrdvYieldRecord.DefaultCellStyle = dataGridViewCellStyle6;
            this.dgrdvYieldRecord.Location = new System.Drawing.Point(4, 37);
            this.dgrdvYieldRecord.Name = "dgrdvYieldRecord";
            this.dgrdvYieldRecord.ReadOnly = true;
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle7.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle7.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle7.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle7.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgrdvYieldRecord.RowHeadersDefaultCellStyle = dataGridViewCellStyle7;
            this.dgrdvYieldRecord.RowHeadersVisible = false;
            this.dgrdvYieldRecord.RowTemplate.DefaultCellStyle.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.dgrdvYieldRecord.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgrdvYieldRecord.RowTemplate.DefaultCellStyle.ForeColor = System.Drawing.Color.Black;
            this.dgrdvYieldRecord.RowTemplate.Height = 40;
            this.dgrdvYieldRecord.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgrdvYieldRecord.Size = new System.Drawing.Size(309, 271);
            this.dgrdvYieldRecord.TabIndex = 9;
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
            this.totalYield.HeaderText = "设备产量";
            this.totalYield.Name = "totalYield";
            this.totalYield.ReadOnly = true;
            this.totalYield.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // conveyor1Yield
            // 
            this.conveyor1Yield.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.conveyor1Yield.FillWeight = 25F;
            this.conveyor1Yield.HeaderText = "传送线1产量";
            this.conveyor1Yield.Name = "conveyor1Yield";
            this.conveyor1Yield.ReadOnly = true;
            this.conveyor1Yield.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // conveyor2Yield
            // 
            this.conveyor2Yield.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.conveyor2Yield.FillWeight = 25F;
            this.conveyor2Yield.HeaderText = "传送线2产量";
            this.conveyor2Yield.Name = "conveyor2Yield";
            this.conveyor2Yield.ReadOnly = true;
            this.conveyor2Yield.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // pnlYieldRecord
            // 
            this.pnlYieldRecord.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlYieldRecord.Controls.Add(this.lblMarkSearchTime);
            this.pnlYieldRecord.Controls.Add(this.lblConveyor2MeasurementUsingTime);
            this.pnlYieldRecord.Controls.Add(this.lblConveyor1MeasurementUsingTime);
            this.pnlYieldRecord.Controls.Add(this.btnClearYield);
            this.pnlYieldRecord.Controls.Add(this.rbtnElementYield);
            this.pnlYieldRecord.Controls.Add(this.rbtnPanelYield);
            this.pnlYieldRecord.Controls.Add(this.rbtnPcsYield);
            this.pnlYieldRecord.Controls.Add(this.dgrdvYieldRecord);
            this.pnlYieldRecord.Location = new System.Drawing.Point(1320, 506);
            this.pnlYieldRecord.Name = "pnlYieldRecord";
            this.pnlYieldRecord.Size = new System.Drawing.Size(316, 416);
            this.pnlYieldRecord.TabIndex = 9;
            // 
            // lblMarkSearchTime
            // 
            this.lblMarkSearchTime.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblMarkSearchTime.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMarkSearchTime.Location = new System.Drawing.Point(12, 384);
            this.lblMarkSearchTime.Name = "lblMarkSearchTime";
            this.lblMarkSearchTime.Size = new System.Drawing.Size(77, 19);
            this.lblMarkSearchTime.TabIndex = 47;
            this.lblMarkSearchTime.Text = "----s";
            this.lblMarkSearchTime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblMarkSearchTime.Visible = false;
            // 
            // lblConveyor2MeasurementUsingTime
            // 
            this.lblConveyor2MeasurementUsingTime.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblConveyor2MeasurementUsingTime.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblConveyor2MeasurementUsingTime.Location = new System.Drawing.Point(14, 349);
            this.lblConveyor2MeasurementUsingTime.Name = "lblConveyor2MeasurementUsingTime";
            this.lblConveyor2MeasurementUsingTime.Size = new System.Drawing.Size(295, 19);
            this.lblConveyor2MeasurementUsingTime.TabIndex = 46;
            this.lblConveyor2MeasurementUsingTime.Text = "传送线2检测用时：----s";
            this.lblConveyor2MeasurementUsingTime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblConveyor1MeasurementUsingTime
            // 
            this.lblConveyor1MeasurementUsingTime.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblConveyor1MeasurementUsingTime.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblConveyor1MeasurementUsingTime.Location = new System.Drawing.Point(14, 320);
            this.lblConveyor1MeasurementUsingTime.Name = "lblConveyor1MeasurementUsingTime";
            this.lblConveyor1MeasurementUsingTime.Size = new System.Drawing.Size(295, 19);
            this.lblConveyor1MeasurementUsingTime.TabIndex = 46;
            this.lblConveyor1MeasurementUsingTime.Text = "传送线1检测用时：----s";
            this.lblConveyor1MeasurementUsingTime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnClearYield
            // 
            this.btnClearYield.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClearYield.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClearYield.Location = new System.Drawing.Point(95, 379);
            this.btnClearYield.Name = "btnClearYield";
            this.btnClearYield.Size = new System.Drawing.Size(128, 28);
            this.btnClearYield.TabIndex = 11;
            this.btnClearYield.Text = "清除产量";
            this.btnClearYield.UseVisualStyleBackColor = true;
            this.btnClearYield.Click += new System.EventHandler(this.btnClearYield_Click);
            // 
            // rbtnElementYield
            // 
            this.rbtnElementYield.Font = new System.Drawing.Font("Times New Roman", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbtnElementYield.Location = new System.Drawing.Point(218, 5);
            this.rbtnElementYield.Name = "rbtnElementYield";
            this.rbtnElementYield.Size = new System.Drawing.Size(80, 26);
            this.rbtnElementYield.TabIndex = 10;
            this.rbtnElementYield.TabStop = true;
            this.rbtnElementYield.Text = "元件";
            this.rbtnElementYield.UseVisualStyleBackColor = true;
            this.rbtnElementYield.CheckedChanged += new System.EventHandler(this.rbtnElementYield_CheckedChanged);
            // 
            // rbtnPanelYield
            // 
            this.rbtnPanelYield.Font = new System.Drawing.Font("Times New Roman", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbtnPanelYield.Location = new System.Drawing.Point(115, 5);
            this.rbtnPanelYield.Name = "rbtnPanelYield";
            this.rbtnPanelYield.Size = new System.Drawing.Size(80, 26);
            this.rbtnPanelYield.TabIndex = 10;
            this.rbtnPanelYield.TabStop = true;
            this.rbtnPanelYield.Text = "整版";
            this.rbtnPanelYield.UseVisualStyleBackColor = true;
            this.rbtnPanelYield.CheckedChanged += new System.EventHandler(this.rbtnPanelYield_CheckedChanged);
            // 
            // rbtnPcsYield
            // 
            this.rbtnPcsYield.Font = new System.Drawing.Font("Times New Roman", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbtnPcsYield.Location = new System.Drawing.Point(12, 5);
            this.rbtnPcsYield.Name = "rbtnPcsYield";
            this.rbtnPcsYield.Size = new System.Drawing.Size(80, 26);
            this.rbtnPcsYield.TabIndex = 10;
            this.rbtnPcsYield.TabStop = true;
            this.rbtnPcsYield.Text = "PCS";
            this.rbtnPcsYield.UseVisualStyleBackColor = true;
            this.rbtnPcsYield.CheckedChanged += new System.EventHandler(this.rbtnPcsYield_CheckedChanged);
            // 
            // pnlOther
            // 
            this.pnlOther.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlOther.Controls.Add(this.btnSPCOpen);
            this.pnlOther.Controls.Add(this.tctlInferResult);
            this.pnlOther.Controls.Add(this.lblAccessLevelTitle);
            this.pnlOther.Controls.Add(this.lblUserName);
            this.pnlOther.Controls.Add(this.lblAccessLevel);
            this.pnlOther.Location = new System.Drawing.Point(1320, 0);
            this.pnlOther.Name = "pnlOther";
            this.pnlOther.Size = new System.Drawing.Size(316, 505);
            this.pnlOther.TabIndex = 11;
            // 
            // btnSPCOpen
            // 
            this.btnSPCOpen.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSPCOpen.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSPCOpen.Location = new System.Drawing.Point(194, 2);
            this.btnSPCOpen.Name = "btnSPCOpen";
            this.btnSPCOpen.Size = new System.Drawing.Size(115, 42);
            this.btnSPCOpen.TabIndex = 8;
            this.btnSPCOpen.Text = "SPC";
            this.btnSPCOpen.UseVisualStyleBackColor = true;
            this.btnSPCOpen.Click += new System.EventHandler(this.btnSPCOpen_Click);
            // 
            // lblAccessLevelTitle
            // 
            this.lblAccessLevelTitle.BackColor = System.Drawing.SystemColors.Control;
            this.lblAccessLevelTitle.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblAccessLevelTitle.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAccessLevelTitle.ForeColor = System.Drawing.Color.Black;
            this.lblAccessLevelTitle.Image = ((System.Drawing.Image)(resources.GetObject("lblAccessLevelTitle.Image")));
            this.lblAccessLevelTitle.Location = new System.Drawing.Point(4, 2);
            this.lblAccessLevelTitle.Name = "lblAccessLevelTitle";
            this.lblAccessLevelTitle.Size = new System.Drawing.Size(51, 42);
            this.lblAccessLevelTitle.TabIndex = 3;
            this.lblAccessLevelTitle.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblAccessLevelTitle.Click += new System.EventHandler(this.lblAccessLevel_Click);
            // 
            // lblUserName
            // 
            this.lblUserName.BackColor = System.Drawing.SystemColors.Control;
            this.lblUserName.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUserName.ForeColor = System.Drawing.Color.Black;
            this.lblUserName.Location = new System.Drawing.Point(53, 22);
            this.lblUserName.Name = "lblUserName";
            this.lblUserName.Size = new System.Drawing.Size(133, 21);
            this.lblUserName.TabIndex = 3;
            this.lblUserName.Text = "3DAOI";
            this.lblUserName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblUserName.Click += new System.EventHandler(this.lblAccessLevel_Click);
            // 
            // lblMESStatus
            // 
            this.lblMESStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblMESStatus.BackColor = System.Drawing.Color.Red;
            this.lblMESStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblMESStatus.Font = new System.Drawing.Font("Times New Roman", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMESStatus.ForeColor = System.Drawing.Color.White;
            this.lblMESStatus.Location = new System.Drawing.Point(126, 925);
            this.lblMESStatus.Name = "lblMESStatus";
            this.lblMESStatus.Size = new System.Drawing.Size(126, 30);
            this.lblMESStatus.TabIndex = 5;
            this.lblMESStatus.Text = "MES通讯屏蔽";
            this.lblMESStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblMESStatus.Click += new System.EventHandler(this.lblMotionCardStatus_Click);
            // 
            // BaseForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(1639, 957);
            this.Controls.Add(this.pnlOther);
            this.Controls.Add(this.pnlYieldRecord);
            this.Controls.Add(this.tctlDeviceStatusRecord);
            this.Controls.Add(this.lblTime);
            this.Controls.Add(this.lblAxisCoordination);
            this.Controls.Add(this.lblDeviceStatus);
            this.Controls.Add(this.lblMESStatus);
            this.Controls.Add(this.lblMotionCardStatus);
            this.Controls.Add(this.pnlChildFormDisplay);
            this.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.IsMdiContainer = true;
            this.Location = new System.Drawing.Point(1, 1);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "BaseForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "3D AVI";
            this.Load += new System.EventHandler(this.BaseForm_Load);
            this.pnlChildFormDisplay.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tctlInferResult.ResumeLayout(false);
            this.tpageConveyor1InferResult.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgrdvConveyor1InferResultDisp)).EndInit();
            this.tpageConveyor2InferResult.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgrdvConveyor2InferResultDisp)).EndInit();
            this.tctlDeviceStatusRecord.ResumeLayout(false);
            this.tpageDeviceControlLOG.ResumeLayout(false);
            this.tpageMESLOG.ResumeLayout(false);
            this.tpageAlarmRecord.ResumeLayout(false);
            this.tpageDeviceStatus.ResumeLayout(false);
            this.tpageBackCameraReconginitionImage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgrdvYieldRecord)).EndInit();
            this.pnlYieldRecord.ResumeLayout(false);
            this.pnlOther.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlChildFormDisplay;
        private Label lblTime;
        private Label lblDeviceStatus;
        private Label lblMotionCardStatus;
        private Label lblAxisCoordination;
        private Label lblAccessLevel;
        private System.Windows.Forms.Timer tmrRefreshBottomLabel;
        private TabControl tctlInferResult;
        private TabPage tpageConveyor1InferResult;
        private TabPage tpageConveyor2InferResult;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private TabControl tctlDeviceStatusRecord;
        private TabPage tpageDeviceControlLOG;
        private TabPage tpageAlarmRecord;
        private TabPage tpageDeviceStatus;
        private DataGridView dgrdvConveyor1InferResultDisp;
        private DataGridView dgrdvConveyor2InferResultDisp;
        private DataGridViewTextBoxColumn Conveyor1ElementNumber;
        private DataGridViewTextBoxColumn Conveyor1ElementName;
        private DataGridViewTextBoxColumn Conveyor1LeftResult;
        private DataGridViewTextBoxColumn Conveyor1RightResult;
        private DataGridViewTextBoxColumn Conveyor1ElementResult;
        private DataGridViewTextBoxColumn Conveyor2ElementNumber;
        private DataGridViewTextBoxColumn Conveyor2ElementName;
        private DataGridViewTextBoxColumn Conveyor2LeftResult;
        private DataGridViewTextBoxColumn Conveyor2RightResult;
        private DataGridViewTextBoxColumn Conveyor2ElementResult;
        private DataGridView dgrdvYieldRecord;
        private Panel pnlYieldRecord;
        private DataGridViewTextBoxColumn yieldProjectName;
        private DataGridViewTextBoxColumn totalYield;
        private DataGridViewTextBoxColumn conveyor1Yield;
        private DataGridViewTextBoxColumn conveyor2Yield;
        private RadioButton rbtnPanelYield;
        private RadioButton rbtnPcsYield;
        private RadioButton rbtnElementYield;
        private Button btnClearYield;
        private Panel pnlOther;
        private Label lblAccessLevelTitle;
        private Label lblUserName;
        private TabPage tpageMESLOG;
        private ListView lvDeviceControlLogDisp;
        private ColumnHeader Info;
        private ListView lvMESLogDisp;
        private ColumnHeader columnHeader2;
        private ListView lvAlarmDisp;
        private ColumnHeader columnHeader1;
        private ListView lvDeviceStatus;
        private ColumnHeader VariateName;
        private ColumnHeader VariateData;
        public Label lblConveyor2MeasurementUsingTime;
        public Label lblConveyor1MeasurementUsingTime;
        private TabPage tpageBackCameraReconginitionImage;
        private Label lblConveyor2Mark2SearchResultTitle;
        private Label lblConveyor2Mark1SearchResultTitle;
        private Label lblConveyor1Mark2SearchResultTitle;
        private Label lblConveyor1Mark1SearchResultTitle;
        private Label lblConveyor1BarcodeScanResultImageTitle;
        public HalconDotNet.HWindowControl picConveyor2Mark2SearchResultImageDisp;
        public HalconDotNet.HWindowControl picConveyor2Mark1SearchResultImageDisp;
        public HalconDotNet.HWindowControl picConveyor1Mark2SearchResultImageDisp;
        public HalconDotNet.HWindowControl picConveyor1Mark1SearchResultImageDisp;
        public HalconDotNet.HWindowControl picConveyor1BarcodeScanResultImageDisp;
        private Label lblConveyor2BarcodeScanResultImageTitle;
        public HalconDotNet.HWindowControl picConveyor2BarcodeScanResultImageDisp;
        public Label lblMarkSearchTime;
        private Button btnSPCOpen;
        private Label lblMESStatus;
    }
}

