using System.Threading; 

namespace AxisAndIOForm
{
    partial class AxisControlMainForm
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
            if (_exchangeDataWithMotionCardThread != null)
            {
                _exchangeDataWithMotionCardThread.Abort();
                _exchangeDataWithMotionCardThread = null;
            }

            if (_axisActionThread != null)
            {
                _axisActionThread.Abort();
                _axisActionThread = null;
            }

            if (_statusUpdateThread != null)
            {
                _statusUpdateThread.Abort();
                _statusUpdateThread = null;
            }

            if (_axisMotionControlBoardVariate != null)
            {
                _axisMotionControlBoardVariate.Close();
                Thread.Sleep(300);//等待停止
                _axisMotionControlBoardVariate = null;
            }

            if (_ctrlUpdateThread != null)
            {
                _ctrlUpdateThread.Abort();
                _ctrlUpdateThread = null;
            }

            if (_axisPositionDataViewFormVariate != null)
            {
                _axisPositionDataViewFormVariate.Dispose();
                _axisPositionDataViewFormVariate = null;
            }

            if (_axisUnionFormVariate != null)
            {
                _axisUnionFormVariate.Dispose();
                _axisUnionFormVariate = null;
            }

            if (_motionCardIOFormVaraite != null)
            {
                _motionCardIOFormVaraite.Dispose();
                _motionCardIOFormVaraite = null;
            }

            Thread.Sleep(100);

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
            this.lblAxisAutoVelocityPCT = new System.Windows.Forms.Label();
            this.lblAxisManualVelocityPCT = new System.Windows.Forms.Label();
            this.lblAxisCurrentCommad = new System.Windows.Forms.Label();
            this.txtAxisCurrentCommad = new System.Windows.Forms.TextBox();
            this.lblAxisCurrentMission = new System.Windows.Forms.Label();
            this.txtAxisCurrentMission = new System.Windows.Forms.TextBox();
            this.lblAxisMotionCardStatus = new System.Windows.Forms.Label();
            this.lblAxisCurrentVelocity = new System.Windows.Forms.Label();
            this.txtAxisCurrentVelocity = new System.Windows.Forms.TextBox();
            this.lblAxisMoveStatus = new System.Windows.Forms.Label();
            this.txtAxisMoveStatus = new System.Windows.Forms.TextBox();
            this.btnAxisPointDataView = new System.Windows.Forms.Button();
            this.btnNextPage = new System.Windows.Forms.Button();
            this.btnPrevPage = new System.Windows.Forms.Button();
            this.btnAxisHomeSensor = new System.Windows.Forms.Button();
            this.btnAxisPlusLimitSensor = new System.Windows.Forms.Button();
            this.btnAxisMinusLimitSensor = new System.Windows.Forms.Button();
            this.btnMoveToPosition010 = new System.Windows.Forms.Button();
            this.btnPositionNumberSelector010 = new System.Windows.Forms.Button();
            this.btnMoveToPosition009 = new System.Windows.Forms.Button();
            this.btnPositionNumberSelector009 = new System.Windows.Forms.Button();
            this.btnMoveToPosition008 = new System.Windows.Forms.Button();
            this.btnPositionNumberSelector008 = new System.Windows.Forms.Button();
            this.btnMoveToPosition007 = new System.Windows.Forms.Button();
            this.btnPositionNumberSelector007 = new System.Windows.Forms.Button();
            this.btnMoveToPosition006 = new System.Windows.Forms.Button();
            this.btnPositionNumberSelector006 = new System.Windows.Forms.Button();
            this.btnMoveToPosition005 = new System.Windows.Forms.Button();
            this.btnPositionNumberSelector005 = new System.Windows.Forms.Button();
            this.btnMoveToPosition004 = new System.Windows.Forms.Button();
            this.btnPositionNumberSelector004 = new System.Windows.Forms.Button();
            this.btnMoveToPosition003 = new System.Windows.Forms.Button();
            this.btnPositionNumberSelector003 = new System.Windows.Forms.Button();
            this.btnMoveToPosition002 = new System.Windows.Forms.Button();
            this.btnPositionNumberSelector002 = new System.Windows.Forms.Button();
            this.btnMoveToPosition001 = new System.Windows.Forms.Button();
            this.btnPositionNumberSelector001 = new System.Windows.Forms.Button();
            this.txtAxisModifyAcc = new System.Windows.Forms.TextBox();
            this.txtAxisModifyVelocity = new System.Windows.Forms.TextBox();
            this.txtAxisModifyPosition = new System.Windows.Forms.TextBox();
            this.txtAxisSavedAcc = new System.Windows.Forms.TextBox();
            this.txtAxisSavedVelocity = new System.Windows.Forms.TextBox();
            this.txtAxisSavedPosition = new System.Windows.Forms.TextBox();
            this.btnAxisSaveSpeedAndOthers = new System.Windows.Forms.Button();
            this.btnAxisSaveModifyPosition = new System.Windows.Forms.Button();
            this.btnAxisSaveCurrentPosition = new System.Windows.Forms.Button();
            this.lblAxisModifydAcc = new System.Windows.Forms.Label();
            this.lblAxisModifyVelocity = new System.Windows.Forms.Label();
            this.lblAxisModifyPosition = new System.Windows.Forms.Label();
            this.lblAxisSavedAcc = new System.Windows.Forms.Label();
            this.lblAxisSavedVelocity = new System.Windows.Forms.Label();
            this.txtAxisSelectedPointName = new System.Windows.Forms.TextBox();
            this.lblAxisSelectedPointName = new System.Windows.Forms.Label();
            this.lblAxisSavedPosition = new System.Windows.Forms.Label();
            this.txtAxisSelectedPointNumber = new System.Windows.Forms.TextBox();
            this.lblAxisSelectedPointNumber = new System.Windows.Forms.Label();
            this.btnAxisJogOrInchSelector = new System.Windows.Forms.Button();
            this.btnAxisMinusMove = new System.Windows.Forms.Button();
            this.txtAxisInchDistance = new System.Windows.Forms.TextBox();
            this.txtAxisJogInchVelocity = new System.Windows.Forms.TextBox();
            this.txtAxisCurrentPosition = new System.Windows.Forms.TextBox();
            this.btnAxisHomeBack = new System.Windows.Forms.Button();
            this.btnAxisServoOnOff = new System.Windows.Forms.Button();
            this.btnAxisAlarmReset = new System.Windows.Forms.Button();
            this.txtAxisAlarmCode = new System.Windows.Forms.TextBox();
            this.lblAxisInchDistance = new System.Windows.Forms.Label();
            this.lblAxisJogInchVelocity = new System.Windows.Forms.Label();
            this.lblAxisCurrentPosition = new System.Windows.Forms.Label();
            this.lblAxisAlarmCode = new System.Windows.Forms.Label();
            this.lblAxisName = new System.Windows.Forms.Label();
            this.shapeContainer1 = new Microsoft.VisualBasic.PowerPacks.ShapeContainer();
            this.rectangleShape3 = new Microsoft.VisualBasic.PowerPacks.RectangleShape();
            this.rectangleShape2 = new Microsoft.VisualBasic.PowerPacks.RectangleShape();
            this.rectangleShape1 = new Microsoft.VisualBasic.PowerPacks.RectangleShape();
            this.lblAxisIndexName = new System.Windows.Forms.Label();
            this.lblPointPage = new System.Windows.Forms.Label();
            this.btnAxisUnion = new System.Windows.Forms.Button();
            this.btnIODisplay = new System.Windows.Forms.Button();
            this.btnAxis01Selector = new System.Windows.Forms.Button();
            this.btnAxis02Selector = new System.Windows.Forms.Button();
            this.btnAxis03Selector = new System.Windows.Forms.Button();
            this.btnAxis04Selector = new System.Windows.Forms.Button();
            this.btnAxis05Selector = new System.Windows.Forms.Button();
            this.btnAxis06Selector = new System.Windows.Forms.Button();
            this.btnAxis07Selector = new System.Windows.Forms.Button();
            this.btnAxis08Selector = new System.Windows.Forms.Button();
            this.btnAxisPlusMove = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblAxisAutoVelocityPCT
            // 
            this.lblAxisAutoVelocityPCT.Font = new System.Drawing.Font("Times New Roman", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAxisAutoVelocityPCT.ForeColor = System.Drawing.Color.Orange;
            this.lblAxisAutoVelocityPCT.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblAxisAutoVelocityPCT.Location = new System.Drawing.Point(803, 6);
            this.lblAxisAutoVelocityPCT.Name = "lblAxisAutoVelocityPCT";
            this.lblAxisAutoVelocityPCT.Size = new System.Drawing.Size(127, 19);
            this.lblAxisAutoVelocityPCT.TabIndex = 271;
            this.lblAxisAutoVelocityPCT.Tag = "";
            this.lblAxisAutoVelocityPCT.Text = "自动速度:100%";
            this.lblAxisAutoVelocityPCT.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblAxisManualVelocityPCT
            // 
            this.lblAxisManualVelocityPCT.Font = new System.Drawing.Font("Times New Roman", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAxisManualVelocityPCT.ForeColor = System.Drawing.Color.Blue;
            this.lblAxisManualVelocityPCT.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblAxisManualVelocityPCT.Location = new System.Drawing.Point(691, 6);
            this.lblAxisManualVelocityPCT.Name = "lblAxisManualVelocityPCT";
            this.lblAxisManualVelocityPCT.Size = new System.Drawing.Size(115, 19);
            this.lblAxisManualVelocityPCT.TabIndex = 270;
            this.lblAxisManualVelocityPCT.Tag = "";
            this.lblAxisManualVelocityPCT.Text = "手动速度:30%";
            this.lblAxisManualVelocityPCT.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblAxisCurrentCommad
            // 
            this.lblAxisCurrentCommad.AutoSize = true;
            this.lblAxisCurrentCommad.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAxisCurrentCommad.Location = new System.Drawing.Point(341, 141);
            this.lblAxisCurrentCommad.Name = "lblAxisCurrentCommad";
            this.lblAxisCurrentCommad.Size = new System.Drawing.Size(41, 19);
            this.lblAxisCurrentCommad.TabIndex = 267;
            this.lblAxisCurrentCommad.Tag = "";
            this.lblAxisCurrentCommad.Text = "命令";
            this.lblAxisCurrentCommad.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtAxisCurrentCommad
            // 
            this.txtAxisCurrentCommad.Location = new System.Drawing.Point(420, 141);
            this.txtAxisCurrentCommad.Name = "txtAxisCurrentCommad";
            this.txtAxisCurrentCommad.ReadOnly = true;
            this.txtAxisCurrentCommad.Size = new System.Drawing.Size(56, 21);
            this.txtAxisCurrentCommad.TabIndex = 266;
            this.txtAxisCurrentCommad.MouseHover += new System.EventHandler(this.CommandDisplayMouseHover);
            // 
            // lblAxisCurrentMission
            // 
            this.lblAxisCurrentMission.AutoSize = true;
            this.lblAxisCurrentMission.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAxisCurrentMission.Location = new System.Drawing.Point(341, 107);
            this.lblAxisCurrentMission.Name = "lblAxisCurrentMission";
            this.lblAxisCurrentMission.Size = new System.Drawing.Size(41, 19);
            this.lblAxisCurrentMission.TabIndex = 265;
            this.lblAxisCurrentMission.Tag = "";
            this.lblAxisCurrentMission.Text = "任务";
            this.lblAxisCurrentMission.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtAxisCurrentMission
            // 
            this.txtAxisCurrentMission.Location = new System.Drawing.Point(420, 107);
            this.txtAxisCurrentMission.Name = "txtAxisCurrentMission";
            this.txtAxisCurrentMission.ReadOnly = true;
            this.txtAxisCurrentMission.Size = new System.Drawing.Size(56, 21);
            this.txtAxisCurrentMission.TabIndex = 264;
            this.txtAxisCurrentMission.MouseHover += new System.EventHandler(this.TaskDisplayMouseHover);
            // 
            // lblAxisMotionCardStatus
            // 
            this.lblAxisMotionCardStatus.Font = new System.Drawing.Font("Times New Roman", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAxisMotionCardStatus.ForeColor = System.Drawing.Color.LimeGreen;
            this.lblAxisMotionCardStatus.Location = new System.Drawing.Point(343, 6);
            this.lblAxisMotionCardStatus.Name = "lblAxisMotionCardStatus";
            this.lblAxisMotionCardStatus.Size = new System.Drawing.Size(123, 19);
            this.lblAxisMotionCardStatus.TabIndex = 263;
            this.lblAxisMotionCardStatus.Tag = "";
            this.lblAxisMotionCardStatus.Text = "运动控制卡正常";
            this.lblAxisMotionCardStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblAxisCurrentVelocity
            // 
            this.lblAxisCurrentVelocity.AutoSize = true;
            this.lblAxisCurrentVelocity.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAxisCurrentVelocity.Location = new System.Drawing.Point(341, 73);
            this.lblAxisCurrentVelocity.Name = "lblAxisCurrentVelocity";
            this.lblAxisCurrentVelocity.Size = new System.Drawing.Size(73, 19);
            this.lblAxisCurrentVelocity.TabIndex = 262;
            this.lblAxisCurrentVelocity.Tag = "";
            this.lblAxisCurrentVelocity.Text = "当前速度";
            this.lblAxisCurrentVelocity.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtAxisCurrentVelocity
            // 
            this.txtAxisCurrentVelocity.Location = new System.Drawing.Point(420, 73);
            this.txtAxisCurrentVelocity.Name = "txtAxisCurrentVelocity";
            this.txtAxisCurrentVelocity.ReadOnly = true;
            this.txtAxisCurrentVelocity.Size = new System.Drawing.Size(56, 21);
            this.txtAxisCurrentVelocity.TabIndex = 261;
            this.txtAxisCurrentVelocity.MouseHover += new System.EventHandler(this.SpeedGetDisplay);
            // 
            // lblAxisMoveStatus
            // 
            this.lblAxisMoveStatus.AutoSize = true;
            this.lblAxisMoveStatus.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAxisMoveStatus.Location = new System.Drawing.Point(341, 39);
            this.lblAxisMoveStatus.Name = "lblAxisMoveStatus";
            this.lblAxisMoveStatus.Size = new System.Drawing.Size(73, 19);
            this.lblAxisMoveStatus.TabIndex = 260;
            this.lblAxisMoveStatus.Tag = "";
            this.lblAxisMoveStatus.Text = "运行状态";
            this.lblAxisMoveStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtAxisMoveStatus
            // 
            this.txtAxisMoveStatus.Location = new System.Drawing.Point(420, 39);
            this.txtAxisMoveStatus.Name = "txtAxisMoveStatus";
            this.txtAxisMoveStatus.ReadOnly = true;
            this.txtAxisMoveStatus.Size = new System.Drawing.Size(56, 21);
            this.txtAxisMoveStatus.TabIndex = 259;
            this.txtAxisMoveStatus.MouseHover += new System.EventHandler(this.CheckDoneCodeMouseHover);
            // 
            // btnAxisPointDataView
            // 
            this.btnAxisPointDataView.BackColor = System.Drawing.Color.Cyan;
            this.btnAxisPointDataView.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAxisPointDataView.ForeColor = System.Drawing.Color.Black;
            this.btnAxisPointDataView.Location = new System.Drawing.Point(852, 456);
            this.btnAxisPointDataView.Name = "btnAxisPointDataView";
            this.btnAxisPointDataView.Size = new System.Drawing.Size(98, 67);
            this.btnAxisPointDataView.TabIndex = 258;
            this.btnAxisPointDataView.Tag = "";
            this.btnAxisPointDataView.Text = "点位预览";
            this.btnAxisPointDataView.UseVisualStyleBackColor = false;
            this.btnAxisPointDataView.Click += new System.EventHandler(this.PositionDataViewEvent);
            // 
            // btnNextPage
            // 
            this.btnNextPage.BackColor = System.Drawing.Color.LimeGreen;
            this.btnNextPage.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNextPage.ForeColor = System.Drawing.Color.Black;
            this.btnNextPage.Location = new System.Drawing.Point(853, 385);
            this.btnNextPage.Name = "btnNextPage";
            this.btnNextPage.Size = new System.Drawing.Size(97, 67);
            this.btnNextPage.TabIndex = 257;
            this.btnNextPage.Tag = "";
            this.btnNextPage.Text = "下一页";
            this.btnNextPage.UseVisualStyleBackColor = false;
            this.btnNextPage.Click += new System.EventHandler(this.PageChangeEvent);
            // 
            // btnPrevPage
            // 
            this.btnPrevPage.BackColor = System.Drawing.Color.LimeGreen;
            this.btnPrevPage.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPrevPage.ForeColor = System.Drawing.Color.Black;
            this.btnPrevPage.Location = new System.Drawing.Point(853, 314);
            this.btnPrevPage.Name = "btnPrevPage";
            this.btnPrevPage.Size = new System.Drawing.Size(97, 67);
            this.btnPrevPage.TabIndex = 256;
            this.btnPrevPage.Tag = "";
            this.btnPrevPage.Text = "上一页";
            this.btnPrevPage.UseVisualStyleBackColor = false;
            this.btnPrevPage.Click += new System.EventHandler(this.PageChangeEvent);
            // 
            // btnAxisHomeSensor
            // 
            this.btnAxisHomeSensor.BackColor = System.Drawing.SystemColors.ControlDark;
            this.btnAxisHomeSensor.Enabled = false;
            this.btnAxisHomeSensor.Font = new System.Drawing.Font("Times New Roman", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAxisHomeSensor.ForeColor = System.Drawing.Color.White;
            this.btnAxisHomeSensor.Location = new System.Drawing.Point(423, 180);
            this.btnAxisHomeSensor.Name = "btnAxisHomeSensor";
            this.btnAxisHomeSensor.Size = new System.Drawing.Size(77, 57);
            this.btnAxisHomeSensor.TabIndex = 251;
            this.btnAxisHomeSensor.Tag = "";
            this.btnAxisHomeSensor.Text = "原点传感器";
            this.btnAxisHomeSensor.UseVisualStyleBackColor = false;
            // 
            // btnAxisPlusLimitSensor
            // 
            this.btnAxisPlusLimitSensor.BackColor = System.Drawing.SystemColors.ControlDark;
            this.btnAxisPlusLimitSensor.Enabled = false;
            this.btnAxisPlusLimitSensor.Font = new System.Drawing.Font("Times New Roman", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAxisPlusLimitSensor.ForeColor = System.Drawing.Color.White;
            this.btnAxisPlusLimitSensor.Location = new System.Drawing.Point(507, 180);
            this.btnAxisPlusLimitSensor.Name = "btnAxisPlusLimitSensor";
            this.btnAxisPlusLimitSensor.Size = new System.Drawing.Size(77, 57);
            this.btnAxisPlusLimitSensor.TabIndex = 250;
            this.btnAxisPlusLimitSensor.Tag = "";
            this.btnAxisPlusLimitSensor.Text = "正极限传感器";
            this.btnAxisPlusLimitSensor.UseVisualStyleBackColor = false;
            // 
            // btnAxisMinusLimitSensor
            // 
            this.btnAxisMinusLimitSensor.BackColor = System.Drawing.SystemColors.ControlDark;
            this.btnAxisMinusLimitSensor.Enabled = false;
            this.btnAxisMinusLimitSensor.Font = new System.Drawing.Font("Times New Roman", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAxisMinusLimitSensor.ForeColor = System.Drawing.Color.White;
            this.btnAxisMinusLimitSensor.Location = new System.Drawing.Point(339, 180);
            this.btnAxisMinusLimitSensor.Name = "btnAxisMinusLimitSensor";
            this.btnAxisMinusLimitSensor.Size = new System.Drawing.Size(77, 57);
            this.btnAxisMinusLimitSensor.TabIndex = 249;
            this.btnAxisMinusLimitSensor.Tag = "";
            this.btnAxisMinusLimitSensor.Text = "负极限传感器";
            this.btnAxisMinusLimitSensor.UseVisualStyleBackColor = false;
            // 
            // btnMoveToPosition010
            // 
            this.btnMoveToPosition010.BackColor = System.Drawing.SystemColors.ControlDark;
            this.btnMoveToPosition010.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMoveToPosition010.ForeColor = System.Drawing.Color.White;
            this.btnMoveToPosition010.Location = new System.Drawing.Point(249, 475);
            this.btnMoveToPosition010.Name = "btnMoveToPosition010";
            this.btnMoveToPosition010.Size = new System.Drawing.Size(73, 45);
            this.btnMoveToPosition010.TabIndex = 244;
            this.btnMoveToPosition010.Tag = "";
            this.btnMoveToPosition010.Text = "运行";
            this.btnMoveToPosition010.UseVisualStyleBackColor = false;
            this.btnMoveToPosition010.Click += new System.EventHandler(this.PointMoveEvent);
            // 
            // btnPositionNumberSelector010
            // 
            this.btnPositionNumberSelector010.BackColor = System.Drawing.SystemColors.ControlDark;
            this.btnPositionNumberSelector010.Font = new System.Drawing.Font("Times New Roman", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPositionNumberSelector010.ForeColor = System.Drawing.Color.White;
            this.btnPositionNumberSelector010.Location = new System.Drawing.Point(4, 475);
            this.btnPositionNumberSelector010.Name = "btnPositionNumberSelector010";
            this.btnPositionNumberSelector010.Size = new System.Drawing.Size(242, 45);
            this.btnPositionNumberSelector010.TabIndex = 243;
            this.btnPositionNumberSelector010.Tag = "";
            this.btnPositionNumberSelector010.Text = "等待位置";
            this.btnPositionNumberSelector010.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPositionNumberSelector010.UseVisualStyleBackColor = false;
            this.btnPositionNumberSelector010.Click += new System.EventHandler(this.PointSelectEvent);
            // 
            // btnMoveToPosition009
            // 
            this.btnMoveToPosition009.BackColor = System.Drawing.SystemColors.ControlDark;
            this.btnMoveToPosition009.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMoveToPosition009.ForeColor = System.Drawing.Color.White;
            this.btnMoveToPosition009.Location = new System.Drawing.Point(249, 426);
            this.btnMoveToPosition009.Name = "btnMoveToPosition009";
            this.btnMoveToPosition009.Size = new System.Drawing.Size(73, 45);
            this.btnMoveToPosition009.TabIndex = 242;
            this.btnMoveToPosition009.Tag = "";
            this.btnMoveToPosition009.Text = "运行";
            this.btnMoveToPosition009.UseVisualStyleBackColor = false;
            this.btnMoveToPosition009.Click += new System.EventHandler(this.PointMoveEvent);
            // 
            // btnPositionNumberSelector009
            // 
            this.btnPositionNumberSelector009.BackColor = System.Drawing.SystemColors.ControlDark;
            this.btnPositionNumberSelector009.Font = new System.Drawing.Font("Times New Roman", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPositionNumberSelector009.ForeColor = System.Drawing.Color.White;
            this.btnPositionNumberSelector009.Location = new System.Drawing.Point(4, 426);
            this.btnPositionNumberSelector009.Name = "btnPositionNumberSelector009";
            this.btnPositionNumberSelector009.Size = new System.Drawing.Size(242, 45);
            this.btnPositionNumberSelector009.TabIndex = 241;
            this.btnPositionNumberSelector009.Tag = "";
            this.btnPositionNumberSelector009.Text = "等待位置";
            this.btnPositionNumberSelector009.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPositionNumberSelector009.UseVisualStyleBackColor = false;
            this.btnPositionNumberSelector009.Click += new System.EventHandler(this.PointSelectEvent);
            // 
            // btnMoveToPosition008
            // 
            this.btnMoveToPosition008.BackColor = System.Drawing.SystemColors.ControlDark;
            this.btnMoveToPosition008.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMoveToPosition008.ForeColor = System.Drawing.Color.White;
            this.btnMoveToPosition008.Location = new System.Drawing.Point(249, 377);
            this.btnMoveToPosition008.Name = "btnMoveToPosition008";
            this.btnMoveToPosition008.Size = new System.Drawing.Size(73, 45);
            this.btnMoveToPosition008.TabIndex = 240;
            this.btnMoveToPosition008.Tag = "";
            this.btnMoveToPosition008.Text = "运行";
            this.btnMoveToPosition008.UseVisualStyleBackColor = false;
            this.btnMoveToPosition008.Click += new System.EventHandler(this.PointMoveEvent);
            // 
            // btnPositionNumberSelector008
            // 
            this.btnPositionNumberSelector008.BackColor = System.Drawing.SystemColors.ControlDark;
            this.btnPositionNumberSelector008.Font = new System.Drawing.Font("Times New Roman", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPositionNumberSelector008.ForeColor = System.Drawing.Color.White;
            this.btnPositionNumberSelector008.Location = new System.Drawing.Point(4, 377);
            this.btnPositionNumberSelector008.Name = "btnPositionNumberSelector008";
            this.btnPositionNumberSelector008.Size = new System.Drawing.Size(242, 45);
            this.btnPositionNumberSelector008.TabIndex = 239;
            this.btnPositionNumberSelector008.Tag = "";
            this.btnPositionNumberSelector008.Text = "等待位置";
            this.btnPositionNumberSelector008.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPositionNumberSelector008.UseVisualStyleBackColor = false;
            this.btnPositionNumberSelector008.Click += new System.EventHandler(this.PointSelectEvent);
            // 
            // btnMoveToPosition007
            // 
            this.btnMoveToPosition007.BackColor = System.Drawing.SystemColors.ControlDark;
            this.btnMoveToPosition007.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMoveToPosition007.ForeColor = System.Drawing.Color.White;
            this.btnMoveToPosition007.Location = new System.Drawing.Point(249, 328);
            this.btnMoveToPosition007.Name = "btnMoveToPosition007";
            this.btnMoveToPosition007.Size = new System.Drawing.Size(73, 45);
            this.btnMoveToPosition007.TabIndex = 238;
            this.btnMoveToPosition007.Tag = "";
            this.btnMoveToPosition007.Text = "运行";
            this.btnMoveToPosition007.UseVisualStyleBackColor = false;
            this.btnMoveToPosition007.Click += new System.EventHandler(this.PointMoveEvent);
            // 
            // btnPositionNumberSelector007
            // 
            this.btnPositionNumberSelector007.BackColor = System.Drawing.SystemColors.ControlDark;
            this.btnPositionNumberSelector007.Font = new System.Drawing.Font("Times New Roman", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPositionNumberSelector007.ForeColor = System.Drawing.Color.White;
            this.btnPositionNumberSelector007.Location = new System.Drawing.Point(4, 328);
            this.btnPositionNumberSelector007.Name = "btnPositionNumberSelector007";
            this.btnPositionNumberSelector007.Size = new System.Drawing.Size(242, 45);
            this.btnPositionNumberSelector007.TabIndex = 237;
            this.btnPositionNumberSelector007.Tag = "";
            this.btnPositionNumberSelector007.Text = "等待位置";
            this.btnPositionNumberSelector007.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPositionNumberSelector007.UseVisualStyleBackColor = false;
            this.btnPositionNumberSelector007.Click += new System.EventHandler(this.PointSelectEvent);
            // 
            // btnMoveToPosition006
            // 
            this.btnMoveToPosition006.BackColor = System.Drawing.SystemColors.ControlDark;
            this.btnMoveToPosition006.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMoveToPosition006.ForeColor = System.Drawing.Color.White;
            this.btnMoveToPosition006.Location = new System.Drawing.Point(249, 279);
            this.btnMoveToPosition006.Name = "btnMoveToPosition006";
            this.btnMoveToPosition006.Size = new System.Drawing.Size(73, 45);
            this.btnMoveToPosition006.TabIndex = 236;
            this.btnMoveToPosition006.Tag = "";
            this.btnMoveToPosition006.Text = "运行";
            this.btnMoveToPosition006.UseVisualStyleBackColor = false;
            this.btnMoveToPosition006.Click += new System.EventHandler(this.PointMoveEvent);
            // 
            // btnPositionNumberSelector006
            // 
            this.btnPositionNumberSelector006.BackColor = System.Drawing.SystemColors.ControlDark;
            this.btnPositionNumberSelector006.Font = new System.Drawing.Font("Times New Roman", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPositionNumberSelector006.ForeColor = System.Drawing.Color.White;
            this.btnPositionNumberSelector006.Location = new System.Drawing.Point(4, 279);
            this.btnPositionNumberSelector006.Name = "btnPositionNumberSelector006";
            this.btnPositionNumberSelector006.Size = new System.Drawing.Size(242, 45);
            this.btnPositionNumberSelector006.TabIndex = 235;
            this.btnPositionNumberSelector006.Tag = "";
            this.btnPositionNumberSelector006.Text = "等待位置";
            this.btnPositionNumberSelector006.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPositionNumberSelector006.UseVisualStyleBackColor = false;
            this.btnPositionNumberSelector006.Click += new System.EventHandler(this.PointSelectEvent);
            // 
            // btnMoveToPosition005
            // 
            this.btnMoveToPosition005.BackColor = System.Drawing.SystemColors.ControlDark;
            this.btnMoveToPosition005.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMoveToPosition005.ForeColor = System.Drawing.Color.White;
            this.btnMoveToPosition005.Location = new System.Drawing.Point(249, 230);
            this.btnMoveToPosition005.Name = "btnMoveToPosition005";
            this.btnMoveToPosition005.Size = new System.Drawing.Size(73, 45);
            this.btnMoveToPosition005.TabIndex = 234;
            this.btnMoveToPosition005.Tag = "";
            this.btnMoveToPosition005.Text = "运行";
            this.btnMoveToPosition005.UseVisualStyleBackColor = false;
            this.btnMoveToPosition005.Click += new System.EventHandler(this.PointMoveEvent);
            // 
            // btnPositionNumberSelector005
            // 
            this.btnPositionNumberSelector005.BackColor = System.Drawing.SystemColors.ControlDark;
            this.btnPositionNumberSelector005.Font = new System.Drawing.Font("Times New Roman", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPositionNumberSelector005.ForeColor = System.Drawing.Color.White;
            this.btnPositionNumberSelector005.Location = new System.Drawing.Point(4, 230);
            this.btnPositionNumberSelector005.Name = "btnPositionNumberSelector005";
            this.btnPositionNumberSelector005.Size = new System.Drawing.Size(242, 45);
            this.btnPositionNumberSelector005.TabIndex = 233;
            this.btnPositionNumberSelector005.Tag = "";
            this.btnPositionNumberSelector005.Text = "等待位置";
            this.btnPositionNumberSelector005.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPositionNumberSelector005.UseVisualStyleBackColor = false;
            this.btnPositionNumberSelector005.Click += new System.EventHandler(this.PointSelectEvent);
            // 
            // btnMoveToPosition004
            // 
            this.btnMoveToPosition004.BackColor = System.Drawing.SystemColors.ControlDark;
            this.btnMoveToPosition004.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMoveToPosition004.ForeColor = System.Drawing.Color.White;
            this.btnMoveToPosition004.Location = new System.Drawing.Point(249, 181);
            this.btnMoveToPosition004.Name = "btnMoveToPosition004";
            this.btnMoveToPosition004.Size = new System.Drawing.Size(73, 45);
            this.btnMoveToPosition004.TabIndex = 232;
            this.btnMoveToPosition004.Tag = "";
            this.btnMoveToPosition004.Text = "运行";
            this.btnMoveToPosition004.UseVisualStyleBackColor = false;
            this.btnMoveToPosition004.Click += new System.EventHandler(this.PointMoveEvent);
            // 
            // btnPositionNumberSelector004
            // 
            this.btnPositionNumberSelector004.BackColor = System.Drawing.SystemColors.ControlDark;
            this.btnPositionNumberSelector004.Font = new System.Drawing.Font("Times New Roman", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPositionNumberSelector004.ForeColor = System.Drawing.Color.White;
            this.btnPositionNumberSelector004.Location = new System.Drawing.Point(4, 181);
            this.btnPositionNumberSelector004.Name = "btnPositionNumberSelector004";
            this.btnPositionNumberSelector004.Size = new System.Drawing.Size(242, 45);
            this.btnPositionNumberSelector004.TabIndex = 231;
            this.btnPositionNumberSelector004.Tag = "";
            this.btnPositionNumberSelector004.Text = "等待位置";
            this.btnPositionNumberSelector004.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPositionNumberSelector004.UseVisualStyleBackColor = false;
            this.btnPositionNumberSelector004.Click += new System.EventHandler(this.PointSelectEvent);
            // 
            // btnMoveToPosition003
            // 
            this.btnMoveToPosition003.BackColor = System.Drawing.SystemColors.ControlDark;
            this.btnMoveToPosition003.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMoveToPosition003.ForeColor = System.Drawing.Color.White;
            this.btnMoveToPosition003.Location = new System.Drawing.Point(249, 132);
            this.btnMoveToPosition003.Name = "btnMoveToPosition003";
            this.btnMoveToPosition003.Size = new System.Drawing.Size(73, 45);
            this.btnMoveToPosition003.TabIndex = 230;
            this.btnMoveToPosition003.Tag = "";
            this.btnMoveToPosition003.Text = "运行";
            this.btnMoveToPosition003.UseVisualStyleBackColor = false;
            this.btnMoveToPosition003.Click += new System.EventHandler(this.PointMoveEvent);
            // 
            // btnPositionNumberSelector003
            // 
            this.btnPositionNumberSelector003.BackColor = System.Drawing.SystemColors.ControlDark;
            this.btnPositionNumberSelector003.Font = new System.Drawing.Font("Times New Roman", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPositionNumberSelector003.ForeColor = System.Drawing.Color.White;
            this.btnPositionNumberSelector003.Location = new System.Drawing.Point(4, 132);
            this.btnPositionNumberSelector003.Name = "btnPositionNumberSelector003";
            this.btnPositionNumberSelector003.Size = new System.Drawing.Size(242, 45);
            this.btnPositionNumberSelector003.TabIndex = 229;
            this.btnPositionNumberSelector003.Tag = "";
            this.btnPositionNumberSelector003.Text = "等待位置";
            this.btnPositionNumberSelector003.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPositionNumberSelector003.UseVisualStyleBackColor = false;
            this.btnPositionNumberSelector003.Click += new System.EventHandler(this.PointSelectEvent);
            // 
            // btnMoveToPosition002
            // 
            this.btnMoveToPosition002.BackColor = System.Drawing.SystemColors.ControlDark;
            this.btnMoveToPosition002.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMoveToPosition002.ForeColor = System.Drawing.Color.White;
            this.btnMoveToPosition002.Location = new System.Drawing.Point(249, 83);
            this.btnMoveToPosition002.Name = "btnMoveToPosition002";
            this.btnMoveToPosition002.Size = new System.Drawing.Size(73, 45);
            this.btnMoveToPosition002.TabIndex = 228;
            this.btnMoveToPosition002.Tag = "";
            this.btnMoveToPosition002.Text = "运行";
            this.btnMoveToPosition002.UseVisualStyleBackColor = false;
            this.btnMoveToPosition002.Click += new System.EventHandler(this.PointMoveEvent);
            // 
            // btnPositionNumberSelector002
            // 
            this.btnPositionNumberSelector002.BackColor = System.Drawing.SystemColors.ControlDark;
            this.btnPositionNumberSelector002.Font = new System.Drawing.Font("Times New Roman", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPositionNumberSelector002.ForeColor = System.Drawing.Color.White;
            this.btnPositionNumberSelector002.Location = new System.Drawing.Point(4, 83);
            this.btnPositionNumberSelector002.Name = "btnPositionNumberSelector002";
            this.btnPositionNumberSelector002.Size = new System.Drawing.Size(242, 45);
            this.btnPositionNumberSelector002.TabIndex = 227;
            this.btnPositionNumberSelector002.Tag = "";
            this.btnPositionNumberSelector002.Text = "等待位置";
            this.btnPositionNumberSelector002.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPositionNumberSelector002.UseVisualStyleBackColor = false;
            this.btnPositionNumberSelector002.Click += new System.EventHandler(this.PointSelectEvent);
            // 
            // btnMoveToPosition001
            // 
            this.btnMoveToPosition001.BackColor = System.Drawing.SystemColors.ControlDark;
            this.btnMoveToPosition001.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMoveToPosition001.ForeColor = System.Drawing.Color.White;
            this.btnMoveToPosition001.Location = new System.Drawing.Point(249, 34);
            this.btnMoveToPosition001.Name = "btnMoveToPosition001";
            this.btnMoveToPosition001.Size = new System.Drawing.Size(73, 45);
            this.btnMoveToPosition001.TabIndex = 226;
            this.btnMoveToPosition001.Tag = "";
            this.btnMoveToPosition001.Text = "运行";
            this.btnMoveToPosition001.UseVisualStyleBackColor = false;
            this.btnMoveToPosition001.Click += new System.EventHandler(this.PointMoveEvent);
            // 
            // btnPositionNumberSelector001
            // 
            this.btnPositionNumberSelector001.BackColor = System.Drawing.SystemColors.ControlDark;
            this.btnPositionNumberSelector001.Font = new System.Drawing.Font("Times New Roman", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPositionNumberSelector001.ForeColor = System.Drawing.Color.White;
            this.btnPositionNumberSelector001.Location = new System.Drawing.Point(4, 34);
            this.btnPositionNumberSelector001.Name = "btnPositionNumberSelector001";
            this.btnPositionNumberSelector001.Size = new System.Drawing.Size(242, 45);
            this.btnPositionNumberSelector001.TabIndex = 225;
            this.btnPositionNumberSelector001.Tag = "";
            this.btnPositionNumberSelector001.Text = "等待位置";
            this.btnPositionNumberSelector001.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPositionNumberSelector001.UseVisualStyleBackColor = false;
            this.btnPositionNumberSelector001.Click += new System.EventHandler(this.PointSelectEvent);
            // 
            // txtAxisModifyAcc
            // 
            this.txtAxisModifyAcc.BackColor = System.Drawing.Color.White;
            this.txtAxisModifyAcc.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtAxisModifyAcc.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAxisModifyAcc.ForeColor = System.Drawing.Color.Orange;
            this.txtAxisModifyAcc.Location = new System.Drawing.Point(693, 415);
            this.txtAxisModifyAcc.Name = "txtAxisModifyAcc";
            this.txtAxisModifyAcc.Size = new System.Drawing.Size(95, 26);
            this.txtAxisModifyAcc.TabIndex = 224;
            this.txtAxisModifyAcc.Tag = "";
            this.txtAxisModifyAcc.Text = "0.0";
            this.txtAxisModifyAcc.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtAxisModifyAcc.TextChanged += new System.EventHandler(this.TextBoxDataPlusFloatCheckEvent);
            this.txtAxisModifyAcc.DoubleClick += new System.EventHandler(this.TextBoxDoubleClickEventToOpenNumberKeyboard);
            this.txtAxisModifyAcc.Leave += new System.EventHandler(this.TextBoxDataPlusFloatCheckLeaveEvent);
            this.txtAxisModifyAcc.MouseHover += new System.EventHandler(this.ACCTimeModifyMouseHover);
            // 
            // txtAxisModifyVelocity
            // 
            this.txtAxisModifyVelocity.BackColor = System.Drawing.Color.White;
            this.txtAxisModifyVelocity.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtAxisModifyVelocity.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAxisModifyVelocity.ForeColor = System.Drawing.Color.Orange;
            this.txtAxisModifyVelocity.Location = new System.Drawing.Point(521, 415);
            this.txtAxisModifyVelocity.Name = "txtAxisModifyVelocity";
            this.txtAxisModifyVelocity.Size = new System.Drawing.Size(95, 26);
            this.txtAxisModifyVelocity.TabIndex = 223;
            this.txtAxisModifyVelocity.Tag = "";
            this.txtAxisModifyVelocity.Text = "0000";
            this.txtAxisModifyVelocity.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtAxisModifyVelocity.TextChanged += new System.EventHandler(this.TextBoxDataPlusFloatCheckEvent);
            this.txtAxisModifyVelocity.DoubleClick += new System.EventHandler(this.TextBoxDoubleClickEventToOpenNumberKeyboard);
            this.txtAxisModifyVelocity.Leave += new System.EventHandler(this.TextBoxDataPlusFloatCheckLeaveEvent);
            this.txtAxisModifyVelocity.MouseHover += new System.EventHandler(this.SpeedModifyMouseHover);
            // 
            // txtAxisModifyPosition
            // 
            this.txtAxisModifyPosition.BackColor = System.Drawing.Color.White;
            this.txtAxisModifyPosition.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtAxisModifyPosition.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAxisModifyPosition.ForeColor = System.Drawing.Color.Orange;
            this.txtAxisModifyPosition.Location = new System.Drawing.Point(349, 415);
            this.txtAxisModifyPosition.Name = "txtAxisModifyPosition";
            this.txtAxisModifyPosition.Size = new System.Drawing.Size(95, 26);
            this.txtAxisModifyPosition.TabIndex = 222;
            this.txtAxisModifyPosition.Tag = "";
            this.txtAxisModifyPosition.Text = "0000.00";
            this.txtAxisModifyPosition.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtAxisModifyPosition.TextChanged += new System.EventHandler(this.TextBoxDataPlusFloatCheckEvent);
            this.txtAxisModifyPosition.DoubleClick += new System.EventHandler(this.TextBoxDoubleClickEventToOpenNumberKeyboard);
            this.txtAxisModifyPosition.Leave += new System.EventHandler(this.TextBoxDataPlusFloatCheckLeaveEvent);
            this.txtAxisModifyPosition.MouseHover += new System.EventHandler(this.PositionModifyMouseHover);
            // 
            // txtAxisSavedAcc
            // 
            this.txtAxisSavedAcc.BackColor = System.Drawing.Color.White;
            this.txtAxisSavedAcc.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtAxisSavedAcc.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAxisSavedAcc.ForeColor = System.Drawing.Color.Green;
            this.txtAxisSavedAcc.Location = new System.Drawing.Point(693, 344);
            this.txtAxisSavedAcc.Name = "txtAxisSavedAcc";
            this.txtAxisSavedAcc.ReadOnly = true;
            this.txtAxisSavedAcc.Size = new System.Drawing.Size(95, 26);
            this.txtAxisSavedAcc.TabIndex = 221;
            this.txtAxisSavedAcc.Tag = "";
            this.txtAxisSavedAcc.Text = "0.0";
            this.txtAxisSavedAcc.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtAxisSavedVelocity
            // 
            this.txtAxisSavedVelocity.BackColor = System.Drawing.Color.White;
            this.txtAxisSavedVelocity.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtAxisSavedVelocity.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAxisSavedVelocity.ForeColor = System.Drawing.Color.Green;
            this.txtAxisSavedVelocity.Location = new System.Drawing.Point(521, 343);
            this.txtAxisSavedVelocity.Name = "txtAxisSavedVelocity";
            this.txtAxisSavedVelocity.ReadOnly = true;
            this.txtAxisSavedVelocity.Size = new System.Drawing.Size(95, 26);
            this.txtAxisSavedVelocity.TabIndex = 220;
            this.txtAxisSavedVelocity.Tag = "";
            this.txtAxisSavedVelocity.Text = "0000";
            this.txtAxisSavedVelocity.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtAxisSavedPosition
            // 
            this.txtAxisSavedPosition.BackColor = System.Drawing.Color.White;
            this.txtAxisSavedPosition.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtAxisSavedPosition.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAxisSavedPosition.ForeColor = System.Drawing.Color.Green;
            this.txtAxisSavedPosition.Location = new System.Drawing.Point(349, 344);
            this.txtAxisSavedPosition.Name = "txtAxisSavedPosition";
            this.txtAxisSavedPosition.ReadOnly = true;
            this.txtAxisSavedPosition.Size = new System.Drawing.Size(95, 26);
            this.txtAxisSavedPosition.TabIndex = 219;
            this.txtAxisSavedPosition.Tag = "";
            this.txtAxisSavedPosition.Text = "0000.00";
            this.txtAxisSavedPosition.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // btnAxisSaveSpeedAndOthers
            // 
            this.btnAxisSaveSpeedAndOthers.BackColor = System.Drawing.Color.Blue;
            this.btnAxisSaveSpeedAndOthers.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAxisSaveSpeedAndOthers.ForeColor = System.Drawing.Color.White;
            this.btnAxisSaveSpeedAndOthers.Location = new System.Drawing.Point(679, 457);
            this.btnAxisSaveSpeedAndOthers.Name = "btnAxisSaveSpeedAndOthers";
            this.btnAxisSaveSpeedAndOthers.Size = new System.Drawing.Size(137, 67);
            this.btnAxisSaveSpeedAndOthers.TabIndex = 218;
            this.btnAxisSaveSpeedAndOthers.Tag = "";
            this.btnAxisSaveSpeedAndOthers.Text = "更改速度及其它";
            this.btnAxisSaveSpeedAndOthers.UseVisualStyleBackColor = false;
            this.btnAxisSaveSpeedAndOthers.Click += new System.EventHandler(this.SaveSpeedAndOthersEvent);
            // 
            // btnAxisSaveModifyPosition
            // 
            this.btnAxisSaveModifyPosition.BackColor = System.Drawing.Color.Blue;
            this.btnAxisSaveModifyPosition.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAxisSaveModifyPosition.ForeColor = System.Drawing.Color.White;
            this.btnAxisSaveModifyPosition.Location = new System.Drawing.Point(507, 457);
            this.btnAxisSaveModifyPosition.Name = "btnAxisSaveModifyPosition";
            this.btnAxisSaveModifyPosition.Size = new System.Drawing.Size(124, 67);
            this.btnAxisSaveModifyPosition.TabIndex = 217;
            this.btnAxisSaveModifyPosition.Tag = "";
            this.btnAxisSaveModifyPosition.Text = "保存修改位置";
            this.btnAxisSaveModifyPosition.UseVisualStyleBackColor = false;
            this.btnAxisSaveModifyPosition.Click += new System.EventHandler(this.AxisSaveCurrentPositionOrModifiedPositionEvent);
            // 
            // btnAxisSaveCurrentPosition
            // 
            this.btnAxisSaveCurrentPosition.BackColor = System.Drawing.Color.Blue;
            this.btnAxisSaveCurrentPosition.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAxisSaveCurrentPosition.ForeColor = System.Drawing.Color.White;
            this.btnAxisSaveCurrentPosition.Location = new System.Drawing.Point(336, 457);
            this.btnAxisSaveCurrentPosition.Name = "btnAxisSaveCurrentPosition";
            this.btnAxisSaveCurrentPosition.Size = new System.Drawing.Size(124, 67);
            this.btnAxisSaveCurrentPosition.TabIndex = 216;
            this.btnAxisSaveCurrentPosition.Tag = "";
            this.btnAxisSaveCurrentPosition.Text = "保存当前位置";
            this.btnAxisSaveCurrentPosition.UseVisualStyleBackColor = false;
            this.btnAxisSaveCurrentPosition.Click += new System.EventHandler(this.AxisSaveCurrentPositionOrModifiedPositionEvent);
            // 
            // lblAxisModifydAcc
            // 
            this.lblAxisModifydAcc.AutoSize = true;
            this.lblAxisModifydAcc.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAxisModifydAcc.ForeColor = System.Drawing.Color.Orange;
            this.lblAxisModifydAcc.Location = new System.Drawing.Point(667, 391);
            this.lblAxisModifydAcc.Name = "lblAxisModifydAcc";
            this.lblAxisModifydAcc.Size = new System.Drawing.Size(172, 19);
            this.lblAxisModifydAcc.TabIndex = 215;
            this.lblAxisModifydAcc.Tag = "";
            this.lblAxisModifydAcc.Text = "修改的加速度时间(秒)";
            this.lblAxisModifydAcc.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblAxisModifyVelocity
            // 
            this.lblAxisModifyVelocity.AutoSize = true;
            this.lblAxisModifyVelocity.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAxisModifyVelocity.ForeColor = System.Drawing.Color.Orange;
            this.lblAxisModifyVelocity.Location = new System.Drawing.Point(510, 391);
            this.lblAxisModifyVelocity.Name = "lblAxisModifyVelocity";
            this.lblAxisModifyVelocity.Size = new System.Drawing.Size(139, 19);
            this.lblAxisModifyVelocity.TabIndex = 214;
            this.lblAxisModifyVelocity.Tag = "";
            this.lblAxisModifyVelocity.Text = "修改的速度(mm/s)";
            this.lblAxisModifyVelocity.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblAxisModifyPosition
            // 
            this.lblAxisModifyPosition.AutoSize = true;
            this.lblAxisModifyPosition.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAxisModifyPosition.ForeColor = System.Drawing.Color.Orange;
            this.lblAxisModifyPosition.Location = new System.Drawing.Point(332, 391);
            this.lblAxisModifyPosition.Name = "lblAxisModifyPosition";
            this.lblAxisModifyPosition.Size = new System.Drawing.Size(128, 19);
            this.lblAxisModifyPosition.TabIndex = 213;
            this.lblAxisModifyPosition.Tag = "";
            this.lblAxisModifyPosition.Text = "修改的位置(mm)";
            this.lblAxisModifyPosition.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblAxisSavedAcc
            // 
            this.lblAxisSavedAcc.AutoSize = true;
            this.lblAxisSavedAcc.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAxisSavedAcc.ForeColor = System.Drawing.Color.Green;
            this.lblAxisSavedAcc.Location = new System.Drawing.Point(671, 321);
            this.lblAxisSavedAcc.Name = "lblAxisSavedAcc";
            this.lblAxisSavedAcc.Size = new System.Drawing.Size(163, 19);
            this.lblAxisSavedAcc.TabIndex = 212;
            this.lblAxisSavedAcc.Tag = "";
            this.lblAxisSavedAcc.Text = "保存的加速度时间(秒)";
            this.lblAxisSavedAcc.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblAxisSavedVelocity
            // 
            this.lblAxisSavedVelocity.AutoSize = true;
            this.lblAxisSavedVelocity.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAxisSavedVelocity.ForeColor = System.Drawing.Color.Green;
            this.lblAxisSavedVelocity.Location = new System.Drawing.Point(510, 321);
            this.lblAxisSavedVelocity.Name = "lblAxisSavedVelocity";
            this.lblAxisSavedVelocity.Size = new System.Drawing.Size(131, 19);
            this.lblAxisSavedVelocity.TabIndex = 211;
            this.lblAxisSavedVelocity.Tag = "";
            this.lblAxisSavedVelocity.Text = "保存的速度(mm/s)";
            this.lblAxisSavedVelocity.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtAxisSelectedPointName
            // 
            this.txtAxisSelectedPointName.BackColor = System.Drawing.Color.White;
            this.txtAxisSelectedPointName.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtAxisSelectedPointName.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAxisSelectedPointName.ForeColor = System.Drawing.Color.Blue;
            this.txtAxisSelectedPointName.Location = new System.Drawing.Point(449, 288);
            this.txtAxisSelectedPointName.Name = "txtAxisSelectedPointName";
            this.txtAxisSelectedPointName.ReadOnly = true;
            this.txtAxisSelectedPointName.Size = new System.Drawing.Size(501, 26);
            this.txtAxisSelectedPointName.TabIndex = 210;
            this.txtAxisSelectedPointName.Tag = "";
            this.txtAxisSelectedPointName.Text = "等待位置";
            // 
            // lblAxisSelectedPointName
            // 
            this.lblAxisSelectedPointName.AutoSize = true;
            this.lblAxisSelectedPointName.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAxisSelectedPointName.ForeColor = System.Drawing.Color.Blue;
            this.lblAxisSelectedPointName.Location = new System.Drawing.Point(445, 262);
            this.lblAxisSelectedPointName.Name = "lblAxisSelectedPointName";
            this.lblAxisSelectedPointName.Size = new System.Drawing.Size(121, 19);
            this.lblAxisSelectedPointName.TabIndex = 209;
            this.lblAxisSelectedPointName.Tag = "";
            this.lblAxisSelectedPointName.Text = "选择点位号名称";
            this.lblAxisSelectedPointName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblAxisSavedPosition
            // 
            this.lblAxisSavedPosition.AutoSize = true;
            this.lblAxisSavedPosition.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAxisSavedPosition.ForeColor = System.Drawing.Color.Green;
            this.lblAxisSavedPosition.Location = new System.Drawing.Point(339, 321);
            this.lblAxisSavedPosition.Name = "lblAxisSavedPosition";
            this.lblAxisSavedPosition.Size = new System.Drawing.Size(121, 19);
            this.lblAxisSavedPosition.TabIndex = 208;
            this.lblAxisSavedPosition.Tag = "";
            this.lblAxisSavedPosition.Text = "保存的位置(mm)";
            this.lblAxisSavedPosition.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtAxisSelectedPointNumber
            // 
            this.txtAxisSelectedPointNumber.BackColor = System.Drawing.Color.White;
            this.txtAxisSelectedPointNumber.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtAxisSelectedPointNumber.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAxisSelectedPointNumber.ForeColor = System.Drawing.Color.Blue;
            this.txtAxisSelectedPointNumber.Location = new System.Drawing.Point(352, 288);
            this.txtAxisSelectedPointNumber.Name = "txtAxisSelectedPointNumber";
            this.txtAxisSelectedPointNumber.ReadOnly = true;
            this.txtAxisSelectedPointNumber.Size = new System.Drawing.Size(68, 26);
            this.txtAxisSelectedPointNumber.TabIndex = 207;
            this.txtAxisSelectedPointNumber.Tag = "";
            this.txtAxisSelectedPointNumber.Text = "0000";
            this.txtAxisSelectedPointNumber.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lblAxisSelectedPointNumber
            // 
            this.lblAxisSelectedPointNumber.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAxisSelectedPointNumber.ForeColor = System.Drawing.Color.Blue;
            this.lblAxisSelectedPointNumber.Location = new System.Drawing.Point(341, 262);
            this.lblAxisSelectedPointNumber.Name = "lblAxisSelectedPointNumber";
            this.lblAxisSelectedPointNumber.Size = new System.Drawing.Size(92, 19);
            this.lblAxisSelectedPointNumber.TabIndex = 206;
            this.lblAxisSelectedPointNumber.Tag = "";
            this.lblAxisSelectedPointNumber.Text = "选择点位号";
            this.lblAxisSelectedPointNumber.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnAxisJogOrInchSelector
            // 
            this.btnAxisJogOrInchSelector.BackColor = System.Drawing.Color.Blue;
            this.btnAxisJogOrInchSelector.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAxisJogOrInchSelector.ForeColor = System.Drawing.Color.White;
            this.btnAxisJogOrInchSelector.Location = new System.Drawing.Point(729, 170);
            this.btnAxisJogOrInchSelector.Name = "btnAxisJogOrInchSelector";
            this.btnAxisJogOrInchSelector.Size = new System.Drawing.Size(97, 77);
            this.btnAxisJogOrInchSelector.TabIndex = 205;
            this.btnAxisJogOrInchSelector.Tag = "";
            this.btnAxisJogOrInchSelector.Text = "点动";
            this.btnAxisJogOrInchSelector.UseVisualStyleBackColor = false;
            this.btnAxisJogOrInchSelector.Click += new System.EventHandler(this.btnAxisJogOrInchSelector_Click);
            // 
            // btnAxisMinusMove
            // 
            this.btnAxisMinusMove.BackColor = System.Drawing.Color.LightSeaGreen;
            this.btnAxisMinusMove.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAxisMinusMove.Location = new System.Drawing.Point(603, 170);
            this.btnAxisMinusMove.Name = "btnAxisMinusMove";
            this.btnAxisMinusMove.Size = new System.Drawing.Size(97, 77);
            this.btnAxisMinusMove.TabIndex = 203;
            this.btnAxisMinusMove.Tag = "";
            this.btnAxisMinusMove.Text = "负";
            this.btnAxisMinusMove.UseVisualStyleBackColor = false;
            this.btnAxisMinusMove.MouseDown += new System.Windows.Forms.MouseEventHandler(this.JogInchButtonMoveDownEvent);
            this.btnAxisMinusMove.MouseLeave += new System.EventHandler(this.JogInchButtonMoveLeaveEvent);
            this.btnAxisMinusMove.MouseUp += new System.Windows.Forms.MouseEventHandler(this.JogInchButtonMoveUpEvent);
            // 
            // txtAxisInchDistance
            // 
            this.txtAxisInchDistance.BackColor = System.Drawing.Color.White;
            this.txtAxisInchDistance.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtAxisInchDistance.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAxisInchDistance.Location = new System.Drawing.Point(878, 127);
            this.txtAxisInchDistance.Name = "txtAxisInchDistance";
            this.txtAxisInchDistance.Size = new System.Drawing.Size(72, 26);
            this.txtAxisInchDistance.TabIndex = 202;
            this.txtAxisInchDistance.Tag = "";
            this.txtAxisInchDistance.Text = "00.00";
            this.txtAxisInchDistance.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtAxisInchDistance.TextChanged += new System.EventHandler(this.TextBoxDataPlusFloatCheckEvent);
            this.txtAxisInchDistance.DoubleClick += new System.EventHandler(this.TextBoxDoubleClickEventToOpenNumberKeyboard);
            this.txtAxisInchDistance.Leave += new System.EventHandler(this.TextBoxDataPlusFloatCheckLeaveEvent);
            this.txtAxisInchDistance.MouseHover += new System.EventHandler(this.InchLengthMouseHover);
            // 
            // txtAxisJogInchVelocity
            // 
            this.txtAxisJogInchVelocity.BackColor = System.Drawing.Color.White;
            this.txtAxisJogInchVelocity.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtAxisJogInchVelocity.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAxisJogInchVelocity.Location = new System.Drawing.Point(693, 127);
            this.txtAxisJogInchVelocity.Name = "txtAxisJogInchVelocity";
            this.txtAxisJogInchVelocity.Size = new System.Drawing.Size(72, 26);
            this.txtAxisJogInchVelocity.TabIndex = 201;
            this.txtAxisJogInchVelocity.Tag = "";
            this.txtAxisJogInchVelocity.Text = "000";
            this.txtAxisJogInchVelocity.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtAxisJogInchVelocity.TextChanged += new System.EventHandler(this.TextBoxDataPlusFloatCheckEvent);
            this.txtAxisJogInchVelocity.DoubleClick += new System.EventHandler(this.TextBoxDoubleClickEventToOpenNumberKeyboard);
            this.txtAxisJogInchVelocity.Leave += new System.EventHandler(this.TextBoxDataPlusFloatCheckLeaveEvent);
            this.txtAxisJogInchVelocity.MouseHover += new System.EventHandler(this.JogInchSpeedMouseHover);
            // 
            // txtAxisCurrentPosition
            // 
            this.txtAxisCurrentPosition.BackColor = System.Drawing.Color.Yellow;
            this.txtAxisCurrentPosition.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtAxisCurrentPosition.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAxisCurrentPosition.ForeColor = System.Drawing.Color.Black;
            this.txtAxisCurrentPosition.Location = new System.Drawing.Point(497, 137);
            this.txtAxisCurrentPosition.Name = "txtAxisCurrentPosition";
            this.txtAxisCurrentPosition.ReadOnly = true;
            this.txtAxisCurrentPosition.Size = new System.Drawing.Size(76, 26);
            this.txtAxisCurrentPosition.TabIndex = 200;
            this.txtAxisCurrentPosition.Tag = "";
            this.txtAxisCurrentPosition.Text = "0000.00";
            this.txtAxisCurrentPosition.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // btnAxisHomeBack
            // 
            this.btnAxisHomeBack.BackColor = System.Drawing.SystemColors.ControlDark;
            this.btnAxisHomeBack.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAxisHomeBack.ForeColor = System.Drawing.Color.White;
            this.btnAxisHomeBack.Location = new System.Drawing.Point(853, 34);
            this.btnAxisHomeBack.Name = "btnAxisHomeBack";
            this.btnAxisHomeBack.Size = new System.Drawing.Size(97, 77);
            this.btnAxisHomeBack.TabIndex = 199;
            this.btnAxisHomeBack.Tag = "";
            this.btnAxisHomeBack.Text = "回原点";
            this.btnAxisHomeBack.UseVisualStyleBackColor = false;
            this.btnAxisHomeBack.Click += new System.EventHandler(this.btnAxisHomeBack_Click);
            // 
            // btnAxisServoOnOff
            // 
            this.btnAxisServoOnOff.BackColor = System.Drawing.SystemColors.ControlDark;
            this.btnAxisServoOnOff.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAxisServoOnOff.ForeColor = System.Drawing.Color.White;
            this.btnAxisServoOnOff.Location = new System.Drawing.Point(729, 34);
            this.btnAxisServoOnOff.Name = "btnAxisServoOnOff";
            this.btnAxisServoOnOff.Size = new System.Drawing.Size(97, 77);
            this.btnAxisServoOnOff.TabIndex = 198;
            this.btnAxisServoOnOff.Tag = "";
            this.btnAxisServoOnOff.Text = "伺服关";
            this.btnAxisServoOnOff.UseVisualStyleBackColor = false;
            this.btnAxisServoOnOff.Click += new System.EventHandler(this.btnAxisServoOnOff_Click);
            // 
            // btnAxisAlarmReset
            // 
            this.btnAxisAlarmReset.BackColor = System.Drawing.Color.Red;
            this.btnAxisAlarmReset.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAxisAlarmReset.Location = new System.Drawing.Point(603, 34);
            this.btnAxisAlarmReset.Name = "btnAxisAlarmReset";
            this.btnAxisAlarmReset.Size = new System.Drawing.Size(97, 77);
            this.btnAxisAlarmReset.TabIndex = 197;
            this.btnAxisAlarmReset.Tag = "";
            this.btnAxisAlarmReset.Text = "报警复位";
            this.btnAxisAlarmReset.UseVisualStyleBackColor = false;
            this.btnAxisAlarmReset.Click += new System.EventHandler(this.btnAxisAlarmReset_Click);
            // 
            // txtAxisAlarmCode
            // 
            this.txtAxisAlarmCode.BackColor = System.Drawing.Color.White;
            this.txtAxisAlarmCode.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtAxisAlarmCode.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAxisAlarmCode.Location = new System.Drawing.Point(500, 68);
            this.txtAxisAlarmCode.Name = "txtAxisAlarmCode";
            this.txtAxisAlarmCode.ReadOnly = true;
            this.txtAxisAlarmCode.Size = new System.Drawing.Size(76, 26);
            this.txtAxisAlarmCode.TabIndex = 196;
            this.txtAxisAlarmCode.Tag = "";
            this.txtAxisAlarmCode.Text = "0000";
            this.txtAxisAlarmCode.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lblAxisInchDistance
            // 
            this.lblAxisInchDistance.AutoSize = true;
            this.lblAxisInchDistance.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAxisInchDistance.Location = new System.Drawing.Point(772, 130);
            this.lblAxisInchDistance.Name = "lblAxisInchDistance";
            this.lblAxisInchDistance.Size = new System.Drawing.Size(105, 19);
            this.lblAxisInchDistance.TabIndex = 194;
            this.lblAxisInchDistance.Tag = "";
            this.lblAxisInchDistance.Text = "寸动距离(mm)";
            this.lblAxisInchDistance.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblAxisJogInchVelocity
            // 
            this.lblAxisJogInchVelocity.AutoSize = true;
            this.lblAxisJogInchVelocity.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAxisJogInchVelocity.Location = new System.Drawing.Point(603, 130);
            this.lblAxisJogInchVelocity.Name = "lblAxisJogInchVelocity";
            this.lblAxisJogInchVelocity.Size = new System.Drawing.Size(83, 19);
            this.lblAxisJogInchVelocity.TabIndex = 193;
            this.lblAxisJogInchVelocity.Tag = "";
            this.lblAxisJogInchVelocity.Text = "速度(mm/s)";
            this.lblAxisJogInchVelocity.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblAxisCurrentPosition
            // 
            this.lblAxisCurrentPosition.BackColor = System.Drawing.Color.Yellow;
            this.lblAxisCurrentPosition.Font = new System.Drawing.Font("Times New Roman", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAxisCurrentPosition.Location = new System.Drawing.Point(486, 105);
            this.lblAxisCurrentPosition.Name = "lblAxisCurrentPosition";
            this.lblAxisCurrentPosition.Size = new System.Drawing.Size(98, 24);
            this.lblAxisCurrentPosition.TabIndex = 192;
            this.lblAxisCurrentPosition.Tag = "";
            this.lblAxisCurrentPosition.Text = "当前位置(mm)";
            this.lblAxisCurrentPosition.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblAxisAlarmCode
            // 
            this.lblAxisAlarmCode.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAxisAlarmCode.Location = new System.Drawing.Point(500, 39);
            this.lblAxisAlarmCode.Name = "lblAxisAlarmCode";
            this.lblAxisAlarmCode.Size = new System.Drawing.Size(76, 25);
            this.lblAxisAlarmCode.TabIndex = 195;
            this.lblAxisAlarmCode.Tag = "";
            this.lblAxisAlarmCode.Text = "报警代码";
            this.lblAxisAlarmCode.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblAxisName
            // 
            this.lblAxisName.AutoSize = true;
            this.lblAxisName.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAxisName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.lblAxisName.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblAxisName.Location = new System.Drawing.Point(471, 4);
            this.lblAxisName.Name = "lblAxisName";
            this.lblAxisName.Size = new System.Drawing.Size(215, 22);
            this.lblAxisName.TabIndex = 191;
            this.lblAxisName.Tag = "";
            this.lblAxisName.Text = "X1(检测头部移载X1轴)";
            this.lblAxisName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // shapeContainer1
            // 
            this.shapeContainer1.Location = new System.Drawing.Point(0, 0);
            this.shapeContainer1.Margin = new System.Windows.Forms.Padding(0);
            this.shapeContainer1.Name = "shapeContainer1";
            this.shapeContainer1.Shapes.AddRange(new Microsoft.VisualBasic.PowerPacks.Shape[] {
            this.rectangleShape3,
            this.rectangleShape2,
            this.rectangleShape1});
            this.shapeContainer1.Size = new System.Drawing.Size(1036, 537);
            this.shapeContainer1.TabIndex = 272;
            this.shapeContainer1.TabStop = false;
            // 
            // rectangleShape3
            // 
            this.rectangleShape3.BorderColor = System.Drawing.Color.Yellow;
            this.rectangleShape3.BorderWidth = 3;
            this.rectangleShape3.Enabled = false;
            this.rectangleShape3.Location = new System.Drawing.Point(329, 257);
            this.rectangleShape3.Name = "rectangleShape3";
            this.rectangleShape3.Size = new System.Drawing.Size(625, 271);
            // 
            // rectangleShape2
            // 
            this.rectangleShape2.BorderColor = System.Drawing.Color.Yellow;
            this.rectangleShape2.BorderWidth = 3;
            this.rectangleShape2.Enabled = false;
            this.rectangleShape2.Location = new System.Drawing.Point(329, 31);
            this.rectangleShape2.Name = "rectangleShape2";
            this.rectangleShape2.Size = new System.Drawing.Size(625, 219);
            // 
            // rectangleShape1
            // 
            this.rectangleShape1.BorderColor = System.Drawing.Color.Yellow;
            this.rectangleShape1.BorderWidth = 3;
            this.rectangleShape1.Enabled = false;
            this.rectangleShape1.Location = new System.Drawing.Point(0, 31);
            this.rectangleShape1.Name = "rectangleShape1";
            this.rectangleShape1.Size = new System.Drawing.Size(324, 497);
            // 
            // lblAxisIndexName
            // 
            this.lblAxisIndexName.AutoSize = true;
            this.lblAxisIndexName.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAxisIndexName.Location = new System.Drawing.Point(4, 4);
            this.lblAxisIndexName.Name = "lblAxisIndexName";
            this.lblAxisIndexName.Size = new System.Drawing.Size(242, 22);
            this.lblAxisIndexName.TabIndex = 273;
            this.lblAxisIndexName.Text = "Servo Axis-01 Servo Control";
            // 
            // lblPointPage
            // 
            this.lblPointPage.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPointPage.ForeColor = System.Drawing.Color.Blue;
            this.lblPointPage.Location = new System.Drawing.Point(731, 262);
            this.lblPointPage.Name = "lblPointPage";
            this.lblPointPage.Size = new System.Drawing.Size(219, 19);
            this.lblPointPage.TabIndex = 209;
            this.lblPointPage.Tag = "";
            this.lblPointPage.Text = "当前点位页面：1/90";
            this.lblPointPage.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnAxisUnion
            // 
            this.btnAxisUnion.BackColor = System.Drawing.Color.DeepSkyBlue;
            this.btnAxisUnion.Font = new System.Drawing.Font("Times New Roman", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAxisUnion.ForeColor = System.Drawing.Color.Black;
            this.btnAxisUnion.Location = new System.Drawing.Point(959, 420);
            this.btnAxisUnion.Name = "btnAxisUnion";
            this.btnAxisUnion.Size = new System.Drawing.Size(76, 55);
            this.btnAxisUnion.TabIndex = 275;
            this.btnAxisUnion.Tag = "";
            this.btnAxisUnion.Text = "轴联合调试";
            this.btnAxisUnion.UseVisualStyleBackColor = false;
            this.btnAxisUnion.Visible = false;
            this.btnAxisUnion.Click += new System.EventHandler(this.btnAxisUnion_Click);
            // 
            // btnIODisplay
            // 
            this.btnIODisplay.BackColor = System.Drawing.Color.HotPink;
            this.btnIODisplay.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnIODisplay.ForeColor = System.Drawing.Color.Black;
            this.btnIODisplay.Location = new System.Drawing.Point(959, 476);
            this.btnIODisplay.Name = "btnIODisplay";
            this.btnIODisplay.Size = new System.Drawing.Size(76, 55);
            this.btnIODisplay.TabIndex = 274;
            this.btnIODisplay.Tag = "";
            this.btnIODisplay.Text = "I/O";
            this.btnIODisplay.UseVisualStyleBackColor = false;
            this.btnIODisplay.Click += new System.EventHandler(this.btnIODisplay_Click);
            // 
            // btnAxis01Selector
            // 
            this.btnAxis01Selector.BackColor = System.Drawing.SystemColors.ControlDark;
            this.btnAxis01Selector.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAxis01Selector.ForeColor = System.Drawing.Color.White;
            this.btnAxis01Selector.Location = new System.Drawing.Point(956, 30);
            this.btnAxis01Selector.Name = "btnAxis01Selector";
            this.btnAxis01Selector.Size = new System.Drawing.Size(76, 34);
            this.btnAxis01Selector.TabIndex = 252;
            this.btnAxis01Selector.Tag = "";
            this.btnAxis01Selector.Text = "轴1";
            this.btnAxis01Selector.UseVisualStyleBackColor = false;
            this.btnAxis01Selector.Click += new System.EventHandler(this.AxisChangeEvent);
            // 
            // btnAxis02Selector
            // 
            this.btnAxis02Selector.BackColor = System.Drawing.SystemColors.ControlDark;
            this.btnAxis02Selector.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAxis02Selector.ForeColor = System.Drawing.Color.White;
            this.btnAxis02Selector.Location = new System.Drawing.Point(956, 69);
            this.btnAxis02Selector.Name = "btnAxis02Selector";
            this.btnAxis02Selector.Size = new System.Drawing.Size(76, 34);
            this.btnAxis02Selector.TabIndex = 276;
            this.btnAxis02Selector.Tag = "";
            this.btnAxis02Selector.Text = "轴2";
            this.btnAxis02Selector.UseVisualStyleBackColor = false;
            this.btnAxis02Selector.Click += new System.EventHandler(this.AxisChangeEvent);
            // 
            // btnAxis03Selector
            // 
            this.btnAxis03Selector.BackColor = System.Drawing.SystemColors.ControlDark;
            this.btnAxis03Selector.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAxis03Selector.ForeColor = System.Drawing.Color.White;
            this.btnAxis03Selector.Location = new System.Drawing.Point(956, 107);
            this.btnAxis03Selector.Name = "btnAxis03Selector";
            this.btnAxis03Selector.Size = new System.Drawing.Size(76, 34);
            this.btnAxis03Selector.TabIndex = 277;
            this.btnAxis03Selector.Tag = "";
            this.btnAxis03Selector.Text = "轴3";
            this.btnAxis03Selector.UseVisualStyleBackColor = false;
            this.btnAxis03Selector.Click += new System.EventHandler(this.AxisChangeEvent);
            // 
            // btnAxis04Selector
            // 
            this.btnAxis04Selector.BackColor = System.Drawing.SystemColors.ControlDark;
            this.btnAxis04Selector.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAxis04Selector.ForeColor = System.Drawing.Color.White;
            this.btnAxis04Selector.Location = new System.Drawing.Point(956, 146);
            this.btnAxis04Selector.Name = "btnAxis04Selector";
            this.btnAxis04Selector.Size = new System.Drawing.Size(76, 34);
            this.btnAxis04Selector.TabIndex = 278;
            this.btnAxis04Selector.Tag = "";
            this.btnAxis04Selector.Text = "轴4";
            this.btnAxis04Selector.UseVisualStyleBackColor = false;
            this.btnAxis04Selector.Click += new System.EventHandler(this.AxisChangeEvent);
            // 
            // btnAxis05Selector
            // 
            this.btnAxis05Selector.BackColor = System.Drawing.SystemColors.ControlDark;
            this.btnAxis05Selector.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAxis05Selector.ForeColor = System.Drawing.Color.White;
            this.btnAxis05Selector.Location = new System.Drawing.Point(956, 185);
            this.btnAxis05Selector.Name = "btnAxis05Selector";
            this.btnAxis05Selector.Size = new System.Drawing.Size(76, 34);
            this.btnAxis05Selector.TabIndex = 279;
            this.btnAxis05Selector.Tag = "";
            this.btnAxis05Selector.Text = "轴5";
            this.btnAxis05Selector.UseVisualStyleBackColor = false;
            this.btnAxis05Selector.Click += new System.EventHandler(this.AxisChangeEvent);
            // 
            // btnAxis06Selector
            // 
            this.btnAxis06Selector.BackColor = System.Drawing.SystemColors.ControlDark;
            this.btnAxis06Selector.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAxis06Selector.ForeColor = System.Drawing.Color.White;
            this.btnAxis06Selector.Location = new System.Drawing.Point(956, 223);
            this.btnAxis06Selector.Name = "btnAxis06Selector";
            this.btnAxis06Selector.Size = new System.Drawing.Size(76, 34);
            this.btnAxis06Selector.TabIndex = 280;
            this.btnAxis06Selector.Tag = "";
            this.btnAxis06Selector.Text = "轴6";
            this.btnAxis06Selector.UseVisualStyleBackColor = false;
            this.btnAxis06Selector.Click += new System.EventHandler(this.AxisChangeEvent);
            // 
            // btnAxis07Selector
            // 
            this.btnAxis07Selector.BackColor = System.Drawing.SystemColors.ControlDark;
            this.btnAxis07Selector.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAxis07Selector.ForeColor = System.Drawing.Color.White;
            this.btnAxis07Selector.Location = new System.Drawing.Point(956, 262);
            this.btnAxis07Selector.Name = "btnAxis07Selector";
            this.btnAxis07Selector.Size = new System.Drawing.Size(76, 34);
            this.btnAxis07Selector.TabIndex = 281;
            this.btnAxis07Selector.Tag = "";
            this.btnAxis07Selector.Text = "轴7";
            this.btnAxis07Selector.UseVisualStyleBackColor = false;
            this.btnAxis07Selector.Click += new System.EventHandler(this.AxisChangeEvent);
            // 
            // btnAxis08Selector
            // 
            this.btnAxis08Selector.BackColor = System.Drawing.SystemColors.ControlDark;
            this.btnAxis08Selector.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAxis08Selector.ForeColor = System.Drawing.Color.White;
            this.btnAxis08Selector.Location = new System.Drawing.Point(956, 301);
            this.btnAxis08Selector.Name = "btnAxis08Selector";
            this.btnAxis08Selector.Size = new System.Drawing.Size(76, 34);
            this.btnAxis08Selector.TabIndex = 282;
            this.btnAxis08Selector.Tag = "";
            this.btnAxis08Selector.Text = "轴8";
            this.btnAxis08Selector.UseVisualStyleBackColor = false;
            this.btnAxis08Selector.Click += new System.EventHandler(this.AxisChangeEvent);
            // 
            // btnAxisPlusMove
            // 
            this.btnAxisPlusMove.BackColor = System.Drawing.Color.LightSeaGreen;
            this.btnAxisPlusMove.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAxisPlusMove.Location = new System.Drawing.Point(853, 170);
            this.btnAxisPlusMove.Name = "btnAxisPlusMove";
            this.btnAxisPlusMove.Size = new System.Drawing.Size(97, 77);
            this.btnAxisPlusMove.TabIndex = 204;
            this.btnAxisPlusMove.Tag = "";
            this.btnAxisPlusMove.Text = "正";
            this.btnAxisPlusMove.UseVisualStyleBackColor = false;
            this.btnAxisPlusMove.MouseDown += new System.Windows.Forms.MouseEventHandler(this.JogInchButtonMoveDownEvent);
            this.btnAxisPlusMove.MouseLeave += new System.EventHandler(this.JogInchButtonMoveLeaveEvent);
            this.btnAxisPlusMove.MouseUp += new System.Windows.Forms.MouseEventHandler(this.JogInchButtonMoveUpEvent);
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.SystemColors.ControlDark;
            this.button1.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.ForeColor = System.Drawing.Color.White;
            this.button1.Location = new System.Drawing.Point(956, 406);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(76, 32);
            this.button1.TabIndex = 284;
            this.button1.Tag = "";
            this.button1.Text = "飞拍";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.SystemColors.ControlDark;
            this.button2.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button2.ForeColor = System.Drawing.Color.White;
            this.button2.Location = new System.Drawing.Point(956, 338);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(76, 32);
            this.button2.TabIndex = 285;
            this.button2.Tag = "";
            this.button2.Text = "Light";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.BackColor = System.Drawing.SystemColors.ControlDark;
            this.button3.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button3.ForeColor = System.Drawing.Color.White;
            this.button3.Location = new System.Drawing.Point(956, 372);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(76, 32);
            this.button3.TabIndex = 286;
            this.button3.Tag = "";
            this.button3.Text = "Camera";
            this.button3.UseVisualStyleBackColor = false;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.BackColor = System.Drawing.SystemColors.ControlDark;
            this.button4.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button4.ForeColor = System.Drawing.Color.White;
            this.button4.Location = new System.Drawing.Point(956, 440);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(76, 32);
            this.button4.TabIndex = 287;
            this.button4.Tag = "";
            this.button4.Text = "直差";
            this.button4.UseVisualStyleBackColor = false;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // AxisControlMainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1036, 537);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnAxis08Selector);
            this.Controls.Add(this.btnAxis07Selector);
            this.Controls.Add(this.btnAxis06Selector);
            this.Controls.Add(this.btnAxis05Selector);
            this.Controls.Add(this.btnAxis04Selector);
            this.Controls.Add(this.btnAxis03Selector);
            this.Controls.Add(this.btnAxis02Selector);
            this.Controls.Add(this.btnAxisUnion);
            this.Controls.Add(this.btnIODisplay);
            this.Controls.Add(this.lblPointPage);
            this.Controls.Add(this.lblAxisIndexName);
            this.Controls.Add(this.lblAxisAutoVelocityPCT);
            this.Controls.Add(this.lblAxisManualVelocityPCT);
            this.Controls.Add(this.lblAxisCurrentCommad);
            this.Controls.Add(this.txtAxisCurrentCommad);
            this.Controls.Add(this.lblAxisCurrentMission);
            this.Controls.Add(this.txtAxisCurrentMission);
            this.Controls.Add(this.lblAxisMotionCardStatus);
            this.Controls.Add(this.lblAxisCurrentVelocity);
            this.Controls.Add(this.txtAxisCurrentVelocity);
            this.Controls.Add(this.lblAxisMoveStatus);
            this.Controls.Add(this.txtAxisMoveStatus);
            this.Controls.Add(this.btnAxisPointDataView);
            this.Controls.Add(this.btnNextPage);
            this.Controls.Add(this.btnPrevPage);
            this.Controls.Add(this.btnAxis01Selector);
            this.Controls.Add(this.btnAxisHomeSensor);
            this.Controls.Add(this.btnAxisPlusLimitSensor);
            this.Controls.Add(this.btnAxisMinusLimitSensor);
            this.Controls.Add(this.btnMoveToPosition010);
            this.Controls.Add(this.btnPositionNumberSelector010);
            this.Controls.Add(this.btnMoveToPosition009);
            this.Controls.Add(this.btnPositionNumberSelector009);
            this.Controls.Add(this.btnMoveToPosition008);
            this.Controls.Add(this.btnPositionNumberSelector008);
            this.Controls.Add(this.btnMoveToPosition007);
            this.Controls.Add(this.btnPositionNumberSelector007);
            this.Controls.Add(this.btnMoveToPosition006);
            this.Controls.Add(this.btnPositionNumberSelector006);
            this.Controls.Add(this.btnMoveToPosition005);
            this.Controls.Add(this.btnPositionNumberSelector005);
            this.Controls.Add(this.btnMoveToPosition004);
            this.Controls.Add(this.btnPositionNumberSelector004);
            this.Controls.Add(this.btnMoveToPosition003);
            this.Controls.Add(this.btnPositionNumberSelector003);
            this.Controls.Add(this.btnMoveToPosition002);
            this.Controls.Add(this.btnPositionNumberSelector002);
            this.Controls.Add(this.btnMoveToPosition001);
            this.Controls.Add(this.btnPositionNumberSelector001);
            this.Controls.Add(this.txtAxisModifyAcc);
            this.Controls.Add(this.txtAxisModifyVelocity);
            this.Controls.Add(this.txtAxisModifyPosition);
            this.Controls.Add(this.txtAxisSavedAcc);
            this.Controls.Add(this.txtAxisSavedVelocity);
            this.Controls.Add(this.txtAxisSavedPosition);
            this.Controls.Add(this.btnAxisSaveSpeedAndOthers);
            this.Controls.Add(this.btnAxisSaveModifyPosition);
            this.Controls.Add(this.btnAxisSaveCurrentPosition);
            this.Controls.Add(this.lblAxisModifydAcc);
            this.Controls.Add(this.lblAxisModifyVelocity);
            this.Controls.Add(this.lblAxisModifyPosition);
            this.Controls.Add(this.lblAxisSavedAcc);
            this.Controls.Add(this.lblAxisSavedVelocity);
            this.Controls.Add(this.txtAxisSelectedPointName);
            this.Controls.Add(this.lblAxisSelectedPointName);
            this.Controls.Add(this.lblAxisSavedPosition);
            this.Controls.Add(this.txtAxisSelectedPointNumber);
            this.Controls.Add(this.lblAxisSelectedPointNumber);
            this.Controls.Add(this.btnAxisJogOrInchSelector);
            this.Controls.Add(this.btnAxisPlusMove);
            this.Controls.Add(this.btnAxisMinusMove);
            this.Controls.Add(this.txtAxisInchDistance);
            this.Controls.Add(this.txtAxisJogInchVelocity);
            this.Controls.Add(this.txtAxisCurrentPosition);
            this.Controls.Add(this.btnAxisHomeBack);
            this.Controls.Add(this.btnAxisServoOnOff);
            this.Controls.Add(this.btnAxisAlarmReset);
            this.Controls.Add(this.txtAxisAlarmCode);
            this.Controls.Add(this.lblAxisInchDistance);
            this.Controls.Add(this.lblAxisJogInchVelocity);
            this.Controls.Add(this.lblAxisCurrentPosition);
            this.Controls.Add(this.lblAxisAlarmCode);
            this.Controls.Add(this.lblAxisName);
            this.Controls.Add(this.shapeContainer1);
            this.Name = "AxisControlMainForm";
            this.Text = "Axis control form";
            this.Deactivate += new System.EventHandler(this.AxisControlMainForm_Deactivate);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AxisControlMainForm_FormClosing);
            this.Load += new System.EventHandler(this.AxisControlMainForm_Load);
            this.VisibleChanged += new System.EventHandler(this.AxisControlMainForm_VisibleChanged);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblAxisAutoVelocityPCT;
        private System.Windows.Forms.Label lblAxisManualVelocityPCT;
        private System.Windows.Forms.Label lblAxisCurrentCommad;
        private System.Windows.Forms.TextBox txtAxisCurrentCommad;
        private System.Windows.Forms.Label lblAxisCurrentMission;
        private System.Windows.Forms.TextBox txtAxisCurrentMission;
        private System.Windows.Forms.Label lblAxisMotionCardStatus;
        private System.Windows.Forms.Label lblAxisCurrentVelocity;
        private System.Windows.Forms.TextBox txtAxisCurrentVelocity;
        private System.Windows.Forms.Label lblAxisMoveStatus;
        private System.Windows.Forms.TextBox txtAxisMoveStatus;
        private System.Windows.Forms.Button btnAxisPointDataView;
        private System.Windows.Forms.Button btnNextPage;
        private System.Windows.Forms.Button btnPrevPage;
        private System.Windows.Forms.Button btnAxisHomeSensor;
        private System.Windows.Forms.Button btnAxisPlusLimitSensor;
        private System.Windows.Forms.Button btnAxisMinusLimitSensor;
        private System.Windows.Forms.Button btnMoveToPosition010;
        private System.Windows.Forms.Button btnPositionNumberSelector010;
        private System.Windows.Forms.Button btnMoveToPosition009;
        private System.Windows.Forms.Button btnPositionNumberSelector009;
        private System.Windows.Forms.Button btnMoveToPosition008;
        private System.Windows.Forms.Button btnPositionNumberSelector008;
        private System.Windows.Forms.Button btnMoveToPosition007;
        private System.Windows.Forms.Button btnPositionNumberSelector007;
        private System.Windows.Forms.Button btnMoveToPosition006;
        private System.Windows.Forms.Button btnPositionNumberSelector006;
        private System.Windows.Forms.Button btnMoveToPosition005;
        private System.Windows.Forms.Button btnPositionNumberSelector005;
        private System.Windows.Forms.Button btnMoveToPosition004;
        private System.Windows.Forms.Button btnPositionNumberSelector004;
        private System.Windows.Forms.Button btnMoveToPosition003;
        private System.Windows.Forms.Button btnPositionNumberSelector003;
        private System.Windows.Forms.Button btnMoveToPosition002;
        private System.Windows.Forms.Button btnPositionNumberSelector002;
        private System.Windows.Forms.Button btnMoveToPosition001;
        private System.Windows.Forms.Button btnPositionNumberSelector001;
        private System.Windows.Forms.TextBox txtAxisModifyAcc;
        private System.Windows.Forms.TextBox txtAxisModifyVelocity;
        private System.Windows.Forms.TextBox txtAxisModifyPosition;
        private System.Windows.Forms.TextBox txtAxisSavedAcc;
        private System.Windows.Forms.TextBox txtAxisSavedVelocity;
        private System.Windows.Forms.TextBox txtAxisSavedPosition;
        private System.Windows.Forms.Button btnAxisSaveSpeedAndOthers;
        private System.Windows.Forms.Button btnAxisSaveModifyPosition;
        private System.Windows.Forms.Button btnAxisSaveCurrentPosition;
        private System.Windows.Forms.Label lblAxisModifydAcc;
        private System.Windows.Forms.Label lblAxisModifyVelocity;
        private System.Windows.Forms.Label lblAxisModifyPosition;
        private System.Windows.Forms.Label lblAxisSavedAcc;
        private System.Windows.Forms.Label lblAxisSavedVelocity;
        private System.Windows.Forms.TextBox txtAxisSelectedPointName;
        private System.Windows.Forms.Label lblAxisSelectedPointName;
        private System.Windows.Forms.Label lblAxisSavedPosition;
        private System.Windows.Forms.TextBox txtAxisSelectedPointNumber;
        private System.Windows.Forms.Label lblAxisSelectedPointNumber;
        private System.Windows.Forms.Button btnAxisJogOrInchSelector;
        private System.Windows.Forms.Button btnAxisMinusMove;
        private System.Windows.Forms.TextBox txtAxisInchDistance;
        private System.Windows.Forms.TextBox txtAxisJogInchVelocity;
        private System.Windows.Forms.TextBox txtAxisCurrentPosition;
        private System.Windows.Forms.Button btnAxisHomeBack;
        private System.Windows.Forms.Button btnAxisServoOnOff;
        private System.Windows.Forms.Button btnAxisAlarmReset;
        private System.Windows.Forms.TextBox txtAxisAlarmCode;
        private System.Windows.Forms.Label lblAxisInchDistance;
        private System.Windows.Forms.Label lblAxisJogInchVelocity;
        private System.Windows.Forms.Label lblAxisCurrentPosition;
        private System.Windows.Forms.Label lblAxisAlarmCode;
        private System.Windows.Forms.Label lblAxisName;
        private Microsoft.VisualBasic.PowerPacks.ShapeContainer shapeContainer1;
        private Microsoft.VisualBasic.PowerPacks.RectangleShape rectangleShape3;
        private Microsoft.VisualBasic.PowerPacks.RectangleShape rectangleShape2;
        private Microsoft.VisualBasic.PowerPacks.RectangleShape rectangleShape1;
        private System.Windows.Forms.Label lblAxisIndexName;
        private System.Windows.Forms.Label lblPointPage;
        private System.Windows.Forms.Button btnAxisUnion;
        private System.Windows.Forms.Button btnIODisplay;
        private System.Windows.Forms.Button btnAxis01Selector;
        private System.Windows.Forms.Button btnAxis02Selector;
        private System.Windows.Forms.Button btnAxis03Selector;
        private System.Windows.Forms.Button btnAxis04Selector;
        private System.Windows.Forms.Button btnAxis05Selector;
        private System.Windows.Forms.Button btnAxis06Selector;
        private System.Windows.Forms.Button btnAxis07Selector;
        private System.Windows.Forms.Button btnAxis08Selector;
        private System.Windows.Forms.Button btnAxisPlusMove;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
    }
}

