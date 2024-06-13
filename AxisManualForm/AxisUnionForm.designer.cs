using System.Threading; 

namespace AxisAndIOForm
{
    partial class AxisUnionForm
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
            #region 程序退出时停止状态更新线程
            if (_statusUpdateThread != null)
            {
                _statusUpdateThread.Abort();
                _statusUpdateThread = null;
            }
            Thread.Sleep(100);

            #endregion

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
            this.lblAxisMotionCardStatus = new System.Windows.Forms.Label();
            this.txtPointNumberSet = new System.Windows.Forms.TextBox();
            this.lblSelectedPointNumberTitle = new System.Windows.Forms.Label();
            this.txtAxis01CurrentPosition = new System.Windows.Forms.TextBox();
            this.lblAxis01CurrentPosition = new System.Windows.Forms.Label();
            this.txtAxis01SelectedPointNameChange = new System.Windows.Forms.TextBox();
            this.lblAxis01PointNameTitle = new System.Windows.Forms.Label();
            this.txtAxis01InchDistance = new System.Windows.Forms.TextBox();
            this.txtAxis01JogInchVelocity = new System.Windows.Forms.TextBox();
            this.lblAxis01InchDistance = new System.Windows.Forms.Label();
            this.lblAxis01JogInchVelocity = new System.Windows.Forms.Label();
            this.btnAxis01JogOrInchSelector = new System.Windows.Forms.Button();
            this.txtAxis01ModifyAcc = new System.Windows.Forms.TextBox();
            this.txtAxis01ModifyVelocity = new System.Windows.Forms.TextBox();
            this.txtAxis01ModifyPosition = new System.Windows.Forms.TextBox();
            this.txtAxis01SavedAcc = new System.Windows.Forms.TextBox();
            this.txtAxis01SavedVelocity = new System.Windows.Forms.TextBox();
            this.txtAxis01SavedPosition = new System.Windows.Forms.TextBox();
            this.lblAxis01ModifydAcc = new System.Windows.Forms.Label();
            this.lblAxis01ModifyVelocity = new System.Windows.Forms.Label();
            this.lblAxis01ModifyPosition = new System.Windows.Forms.Label();
            this.lblAxis01SavedAcc = new System.Windows.Forms.Label();
            this.lblAxis01SavedVelocity = new System.Windows.Forms.Label();
            this.lblAxis01SavedPosition = new System.Windows.Forms.Label();
            this.btnSaveSpeedOrOthers = new System.Windows.Forms.Button();
            this.btnSaveModifyPosition = new System.Windows.Forms.Button();
            this.btnSaveCurrentPosition = new System.Windows.Forms.Button();
            this.txtAxis02ModifyAcc = new System.Windows.Forms.TextBox();
            this.txtAxis02ModifyVelocity = new System.Windows.Forms.TextBox();
            this.txtAxis02ModifyPosition = new System.Windows.Forms.TextBox();
            this.txtAxis02SavedAcc = new System.Windows.Forms.TextBox();
            this.txtAxis02SavedVelocity = new System.Windows.Forms.TextBox();
            this.txtAxis02SavedPosition = new System.Windows.Forms.TextBox();
            this.lblAxis02ModifydAcc = new System.Windows.Forms.Label();
            this.lblAxis02ModifyVelocity = new System.Windows.Forms.Label();
            this.lblAxis02ModifyPosition = new System.Windows.Forms.Label();
            this.lblAxis02SavedAcc = new System.Windows.Forms.Label();
            this.lblAxis02SavedVelocity = new System.Windows.Forms.Label();
            this.lblAxis02SavedPosition = new System.Windows.Forms.Label();
            this.btnAxis02JogOrInchSelector = new System.Windows.Forms.Button();
            this.txtAxis02InchDistance = new System.Windows.Forms.TextBox();
            this.txtAxis02JogInchVelocity = new System.Windows.Forms.TextBox();
            this.lblAxis02InchDistance = new System.Windows.Forms.Label();
            this.lblAxis02JogInchVelocity = new System.Windows.Forms.Label();
            this.txtAxis02CurrentPosition = new System.Windows.Forms.TextBox();
            this.txtAxis03ModifyAcc = new System.Windows.Forms.TextBox();
            this.txtAxis03ModifyVelocity = new System.Windows.Forms.TextBox();
            this.txtAxis03ModifyPosition = new System.Windows.Forms.TextBox();
            this.txtAxis03SavedAcc = new System.Windows.Forms.TextBox();
            this.txtAxis03SavedVelocity = new System.Windows.Forms.TextBox();
            this.txtAxis03SavedPosition = new System.Windows.Forms.TextBox();
            this.lblAxis03ModifydAcc = new System.Windows.Forms.Label();
            this.lblAxis03ModifyVelocity = new System.Windows.Forms.Label();
            this.lblAxis03ModifyPosition = new System.Windows.Forms.Label();
            this.lblAxis03SavedAcc = new System.Windows.Forms.Label();
            this.lblAxis03SavedVelocity = new System.Windows.Forms.Label();
            this.lblAxis03SavedPosition = new System.Windows.Forms.Label();
            this.btnAxis03JogOrInchSelector = new System.Windows.Forms.Button();
            this.txtAxis03InchDistance = new System.Windows.Forms.TextBox();
            this.txtAxis03JogInchVelocity = new System.Windows.Forms.TextBox();
            this.lblAxis03InchDistance = new System.Windows.Forms.Label();
            this.lblAxis03JogInchVelocity = new System.Windows.Forms.Label();
            this.txtAxis03CurrentPosition = new System.Windows.Forms.TextBox();
            this.txtAxis04ModifyAcc = new System.Windows.Forms.TextBox();
            this.txtAxis04ModifyVelocity = new System.Windows.Forms.TextBox();
            this.txtAxis04ModifyPosition = new System.Windows.Forms.TextBox();
            this.txtAxis04SavedAcc = new System.Windows.Forms.TextBox();
            this.txtAxis04SavedVelocity = new System.Windows.Forms.TextBox();
            this.txtAxis04SavedPosition = new System.Windows.Forms.TextBox();
            this.lblAxis04ModifydAcc = new System.Windows.Forms.Label();
            this.lblAxis04ModifyVelocity = new System.Windows.Forms.Label();
            this.lblAxis04ModifyPosition = new System.Windows.Forms.Label();
            this.lblAxis04SavedAcc = new System.Windows.Forms.Label();
            this.lblAxis04SavedVelocity = new System.Windows.Forms.Label();
            this.lblAxis04SavedPosition = new System.Windows.Forms.Label();
            this.btnAxis04JogOrInchSelector = new System.Windows.Forms.Button();
            this.txtAxis04InchDistance = new System.Windows.Forms.TextBox();
            this.txtAxis04JogInchVelocity = new System.Windows.Forms.TextBox();
            this.lblAxis04InchDistance = new System.Windows.Forms.Label();
            this.lblAxis04JogInchVelocity = new System.Windows.Forms.Label();
            this.txtAxis04CurrentPosition = new System.Windows.Forms.TextBox();
            this.gbxAxis01 = new System.Windows.Forms.GroupBox();
            this.btnAxis01UnionFlag = new System.Windows.Forms.Button();
            this.lblAxis02CurrentPosition = new System.Windows.Forms.Label();
            this.lblAxis03CurrentPosition = new System.Windows.Forms.Label();
            this.lblAxis04CurrentPosition = new System.Windows.Forms.Label();
            this.gbxAxis02 = new System.Windows.Forms.GroupBox();
            this.btnAxis02UnionFlag = new System.Windows.Forms.Button();
            this.gbxAxis03 = new System.Windows.Forms.GroupBox();
            this.btnAxis03UnionFlag = new System.Windows.Forms.Button();
            this.gbxAxis04 = new System.Windows.Forms.GroupBox();
            this.btnAxis04UnionFlag = new System.Windows.Forms.Button();
            this.btnUnionPointNameChange = new System.Windows.Forms.Button();
            this.btnAxis04PlusMove = new UCArrow.UCArrow();
            this.btnAxis04MinusMove = new UCArrow.UCArrow();
            this.btnAxis03PlusMove = new UCArrow.UCArrow();
            this.btnAxis02PlusMove = new UCArrow.UCArrow();
            this.btnAxis02MinusMove = new UCArrow.UCArrow();
            this.btnAxis01MinusMove = new UCArrow.UCArrow();
            this.btnAxis01PlusMove = new UCArrow.UCArrow();
            this.btnAxisUnionRun = new System.Windows.Forms.Button();
            this.txtAxis02SelectedPointNameChange = new System.Windows.Forms.TextBox();
            this.lblAxis02PointNameTitle = new System.Windows.Forms.Label();
            this.txtAxis03SelectedPointNameChange = new System.Windows.Forms.TextBox();
            this.lblAxis03PointNameTitle = new System.Windows.Forms.Label();
            this.txtAxis04SelectedPointNameChange = new System.Windows.Forms.TextBox();
            this.lblAxis04PointNameTitle = new System.Windows.Forms.Label();
            this.shapeContainer1 = new Microsoft.VisualBasic.PowerPacks.ShapeContainer();
            this.rectangleShape3 = new Microsoft.VisualBasic.PowerPacks.RectangleShape();
            this.rectangleShape2 = new Microsoft.VisualBasic.PowerPacks.RectangleShape();
            this.rectangleShape1 = new Microsoft.VisualBasic.PowerPacks.RectangleShape();
            this.btnAxis03MinusMove = new UCArrow.UCArrow();
            this.gbxAxis01.SuspendLayout();
            this.gbxAxis02.SuspendLayout();
            this.gbxAxis03.SuspendLayout();
            this.gbxAxis04.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblAxisMotionCardStatus
            // 
            this.lblAxisMotionCardStatus.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblAxisMotionCardStatus.Font = new System.Drawing.Font("Times New Roman", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAxisMotionCardStatus.ForeColor = System.Drawing.Color.LimeGreen;
            this.lblAxisMotionCardStatus.Location = new System.Drawing.Point(33, 296);
            this.lblAxisMotionCardStatus.Name = "lblAxisMotionCardStatus";
            this.lblAxisMotionCardStatus.Size = new System.Drawing.Size(208, 82);
            this.lblAxisMotionCardStatus.TabIndex = 182;
            this.lblAxisMotionCardStatus.Tag = "";
            this.lblAxisMotionCardStatus.Text = "运动控制卡正常";
            this.lblAxisMotionCardStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txtPointNumberSet
            // 
            this.txtPointNumberSet.BackColor = System.Drawing.Color.White;
            this.txtPointNumberSet.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtPointNumberSet.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPointNumberSet.ForeColor = System.Drawing.Color.Orange;
            this.txtPointNumberSet.Location = new System.Drawing.Point(218, 44);
            this.txtPointNumberSet.Name = "txtPointNumberSet";
            this.txtPointNumberSet.Size = new System.Drawing.Size(79, 35);
            this.txtPointNumberSet.TabIndex = 186;
            this.txtPointNumberSet.Tag = "";
            this.txtPointNumberSet.Text = "000";
            this.txtPointNumberSet.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtPointNumberSet.TextChanged += new System.EventHandler(this.txtPointNumberSet_TextChanged);
            this.txtPointNumberSet.DoubleClick += new System.EventHandler(this.TextBoxDoubleClickEvent);
            this.txtPointNumberSet.Leave += new System.EventHandler(this.txtPointNumberSet_Leave);
            // 
            // lblSelectedPointNumberTitle
            // 
            this.lblSelectedPointNumberTitle.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSelectedPointNumberTitle.ForeColor = System.Drawing.Color.Orange;
            this.lblSelectedPointNumberTitle.Location = new System.Drawing.Point(58, 42);
            this.lblSelectedPointNumberTitle.Name = "lblSelectedPointNumberTitle";
            this.lblSelectedPointNumberTitle.Size = new System.Drawing.Size(148, 36);
            this.lblSelectedPointNumberTitle.TabIndex = 185;
            this.lblSelectedPointNumberTitle.Tag = "";
            this.lblSelectedPointNumberTitle.Text = "选择点位号:";
            this.lblSelectedPointNumberTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtAxis01CurrentPosition
            // 
            this.txtAxis01CurrentPosition.BackColor = System.Drawing.Color.Yellow;
            this.txtAxis01CurrentPosition.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtAxis01CurrentPosition.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAxis01CurrentPosition.ForeColor = System.Drawing.Color.Black;
            this.txtAxis01CurrentPosition.Location = new System.Drawing.Point(184, 32);
            this.txtAxis01CurrentPosition.Name = "txtAxis01CurrentPosition";
            this.txtAxis01CurrentPosition.ReadOnly = true;
            this.txtAxis01CurrentPosition.Size = new System.Drawing.Size(98, 35);
            this.txtAxis01CurrentPosition.TabIndex = 188;
            this.txtAxis01CurrentPosition.Tag = "";
            this.txtAxis01CurrentPosition.Text = "0000.000";
            this.txtAxis01CurrentPosition.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lblAxis01CurrentPosition
            // 
            this.lblAxis01CurrentPosition.AutoSize = true;
            this.lblAxis01CurrentPosition.BackColor = System.Drawing.Color.Yellow;
            this.lblAxis01CurrentPosition.Font = new System.Drawing.Font("Times New Roman", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAxis01CurrentPosition.Location = new System.Drawing.Point(34, 40);
            this.lblAxis01CurrentPosition.Name = "lblAxis01CurrentPosition";
            this.lblAxis01CurrentPosition.Size = new System.Drawing.Size(132, 22);
            this.lblAxis01CurrentPosition.TabIndex = 187;
            this.lblAxis01CurrentPosition.Tag = "";
            this.lblAxis01CurrentPosition.Text = "当前位置(mm)";
            this.lblAxis01CurrentPosition.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // txtAxis01SelectedPointNameChange
            // 
            this.txtAxis01SelectedPointNameChange.BackColor = System.Drawing.Color.White;
            this.txtAxis01SelectedPointNameChange.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtAxis01SelectedPointNameChange.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAxis01SelectedPointNameChange.ForeColor = System.Drawing.Color.Orange;
            this.txtAxis01SelectedPointNameChange.Location = new System.Drawing.Point(218, 106);
            this.txtAxis01SelectedPointNameChange.Name = "txtAxis01SelectedPointNameChange";
            this.txtAxis01SelectedPointNameChange.Size = new System.Drawing.Size(340, 35);
            this.txtAxis01SelectedPointNameChange.TabIndex = 196;
            this.txtAxis01SelectedPointNameChange.Tag = "";
            this.txtAxis01SelectedPointNameChange.Text = "等待位置";
            this.txtAxis01SelectedPointNameChange.DoubleClick += new System.EventHandler(this.TextBoxDoubleClickEvent);
            // 
            // lblAxis01PointNameTitle
            // 
            this.lblAxis01PointNameTitle.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAxis01PointNameTitle.ForeColor = System.Drawing.Color.Orange;
            this.lblAxis01PointNameTitle.Location = new System.Drawing.Point(20, 106);
            this.lblAxis01PointNameTitle.Name = "lblAxis01PointNameTitle";
            this.lblAxis01PointNameTitle.Size = new System.Drawing.Size(188, 39);
            this.lblAxis01PointNameTitle.TabIndex = 197;
            this.lblAxis01PointNameTitle.Tag = "";
            this.lblAxis01PointNameTitle.Text = "轴01点位名称:";
            this.lblAxis01PointNameTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtAxis01InchDistance
            // 
            this.txtAxis01InchDistance.BackColor = System.Drawing.Color.White;
            this.txtAxis01InchDistance.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtAxis01InchDistance.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAxis01InchDistance.Location = new System.Drawing.Point(690, 32);
            this.txtAxis01InchDistance.Name = "txtAxis01InchDistance";
            this.txtAxis01InchDistance.Size = new System.Drawing.Size(96, 35);
            this.txtAxis01InchDistance.TabIndex = 201;
            this.txtAxis01InchDistance.Tag = "";
            this.txtAxis01InchDistance.Text = "00.00";
            this.txtAxis01InchDistance.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtAxis01InchDistance.TextChanged += new System.EventHandler(this.ManualCtrlDataCheckEvent);
            this.txtAxis01InchDistance.DoubleClick += new System.EventHandler(this.TextBoxDoubleClickEvent);
            this.txtAxis01InchDistance.Leave += new System.EventHandler(this.ManualCtrlDataCheckLeaveEvent);
            this.txtAxis01InchDistance.MouseHover += new System.EventHandler(this.InchLengthMouseHover);
            // 
            // txtAxis01JogInchVelocity
            // 
            this.txtAxis01JogInchVelocity.BackColor = System.Drawing.Color.White;
            this.txtAxis01JogInchVelocity.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtAxis01JogInchVelocity.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAxis01JogInchVelocity.Location = new System.Drawing.Point(423, 33);
            this.txtAxis01JogInchVelocity.Name = "txtAxis01JogInchVelocity";
            this.txtAxis01JogInchVelocity.Size = new System.Drawing.Size(96, 35);
            this.txtAxis01JogInchVelocity.TabIndex = 200;
            this.txtAxis01JogInchVelocity.Tag = "";
            this.txtAxis01JogInchVelocity.Text = "000";
            this.txtAxis01JogInchVelocity.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtAxis01JogInchVelocity.TextChanged += new System.EventHandler(this.ManualCtrlDataCheckEvent);
            this.txtAxis01JogInchVelocity.DoubleClick += new System.EventHandler(this.TextBoxDoubleClickEvent);
            this.txtAxis01JogInchVelocity.Leave += new System.EventHandler(this.ManualCtrlDataCheckLeaveEvent);
            this.txtAxis01JogInchVelocity.MouseHover += new System.EventHandler(this.JogInchSpeedMouseHover);
            // 
            // lblAxis01InchDistance
            // 
            this.lblAxis01InchDistance.AutoSize = true;
            this.lblAxis01InchDistance.Font = new System.Drawing.Font("Times New Roman", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAxis01InchDistance.Location = new System.Drawing.Point(546, 39);
            this.lblAxis01InchDistance.Name = "lblAxis01InchDistance";
            this.lblAxis01InchDistance.Size = new System.Drawing.Size(132, 22);
            this.lblAxis01InchDistance.TabIndex = 199;
            this.lblAxis01InchDistance.Tag = "";
            this.lblAxis01InchDistance.Text = "寸动长度(mm)";
            this.lblAxis01InchDistance.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblAxis01JogInchVelocity
            // 
            this.lblAxis01JogInchVelocity.AutoSize = true;
            this.lblAxis01JogInchVelocity.Font = new System.Drawing.Font("Times New Roman", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAxis01JogInchVelocity.Location = new System.Drawing.Point(302, 39);
            this.lblAxis01JogInchVelocity.Name = "lblAxis01JogInchVelocity";
            this.lblAxis01JogInchVelocity.Size = new System.Drawing.Size(106, 22);
            this.lblAxis01JogInchVelocity.TabIndex = 198;
            this.lblAxis01JogInchVelocity.Tag = "";
            this.lblAxis01JogInchVelocity.Text = "速度(mm/s)";
            this.lblAxis01JogInchVelocity.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnAxis01JogOrInchSelector
            // 
            this.btnAxis01JogOrInchSelector.BackColor = System.Drawing.Color.Blue;
            this.btnAxis01JogOrInchSelector.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAxis01JogOrInchSelector.ForeColor = System.Drawing.Color.White;
            this.btnAxis01JogOrInchSelector.Location = new System.Drawing.Point(824, 114);
            this.btnAxis01JogOrInchSelector.Name = "btnAxis01JogOrInchSelector";
            this.btnAxis01JogOrInchSelector.Size = new System.Drawing.Size(130, 87);
            this.btnAxis01JogOrInchSelector.TabIndex = 202;
            this.btnAxis01JogOrInchSelector.Tag = "";
            this.btnAxis01JogOrInchSelector.Text = "点动";
            this.btnAxis01JogOrInchSelector.UseVisualStyleBackColor = false;
            this.btnAxis01JogOrInchSelector.Click += new System.EventHandler(this.InchFlagChangeEvent);
            // 
            // txtAxis01ModifyAcc
            // 
            this.txtAxis01ModifyAcc.BackColor = System.Drawing.Color.White;
            this.txtAxis01ModifyAcc.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtAxis01ModifyAcc.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAxis01ModifyAcc.ForeColor = System.Drawing.Color.Orange;
            this.txtAxis01ModifyAcc.Location = new System.Drawing.Point(651, 201);
            this.txtAxis01ModifyAcc.Name = "txtAxis01ModifyAcc";
            this.txtAxis01ModifyAcc.Size = new System.Drawing.Size(96, 35);
            this.txtAxis01ModifyAcc.TabIndex = 235;
            this.txtAxis01ModifyAcc.Tag = "";
            this.txtAxis01ModifyAcc.Text = "0.0";
            this.txtAxis01ModifyAcc.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtAxis01ModifyAcc.TextChanged += new System.EventHandler(this.ManualCtrlDataCheckEvent);
            this.txtAxis01ModifyAcc.DoubleClick += new System.EventHandler(this.TextBoxDoubleClickEvent);
            this.txtAxis01ModifyAcc.Leave += new System.EventHandler(this.ManualCtrlDataCheckLeaveEvent);
            this.txtAxis01ModifyAcc.MouseHover += new System.EventHandler(this.ACCTimeModifyMouseHover);
            // 
            // txtAxis01ModifyVelocity
            // 
            this.txtAxis01ModifyVelocity.BackColor = System.Drawing.Color.White;
            this.txtAxis01ModifyVelocity.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtAxis01ModifyVelocity.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAxis01ModifyVelocity.ForeColor = System.Drawing.Color.Orange;
            this.txtAxis01ModifyVelocity.Location = new System.Drawing.Point(339, 201);
            this.txtAxis01ModifyVelocity.Name = "txtAxis01ModifyVelocity";
            this.txtAxis01ModifyVelocity.Size = new System.Drawing.Size(96, 35);
            this.txtAxis01ModifyVelocity.TabIndex = 234;
            this.txtAxis01ModifyVelocity.Tag = "";
            this.txtAxis01ModifyVelocity.Text = "0000";
            this.txtAxis01ModifyVelocity.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtAxis01ModifyVelocity.TextChanged += new System.EventHandler(this.ManualCtrlDataCheckEvent);
            this.txtAxis01ModifyVelocity.DoubleClick += new System.EventHandler(this.TextBoxDoubleClickEvent);
            this.txtAxis01ModifyVelocity.Leave += new System.EventHandler(this.ManualCtrlDataCheckLeaveEvent);
            this.txtAxis01ModifyVelocity.MouseHover += new System.EventHandler(this.SpeedModifyMouseHover);
            // 
            // txtAxis01ModifyPosition
            // 
            this.txtAxis01ModifyPosition.BackColor = System.Drawing.Color.White;
            this.txtAxis01ModifyPosition.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtAxis01ModifyPosition.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAxis01ModifyPosition.ForeColor = System.Drawing.Color.Orange;
            this.txtAxis01ModifyPosition.Location = new System.Drawing.Point(60, 201);
            this.txtAxis01ModifyPosition.Name = "txtAxis01ModifyPosition";
            this.txtAxis01ModifyPosition.Size = new System.Drawing.Size(96, 35);
            this.txtAxis01ModifyPosition.TabIndex = 233;
            this.txtAxis01ModifyPosition.Tag = "";
            this.txtAxis01ModifyPosition.Text = "0000.00";
            this.txtAxis01ModifyPosition.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtAxis01ModifyPosition.TextChanged += new System.EventHandler(this.ManualCtrlDataCheckEvent);
            this.txtAxis01ModifyPosition.DoubleClick += new System.EventHandler(this.TextBoxDoubleClickEvent);
            this.txtAxis01ModifyPosition.Leave += new System.EventHandler(this.ManualCtrlDataCheckLeaveEvent);
            this.txtAxis01ModifyPosition.MouseHover += new System.EventHandler(this.PositionModifyMouseHover);
            // 
            // txtAxis01SavedAcc
            // 
            this.txtAxis01SavedAcc.BackColor = System.Drawing.Color.White;
            this.txtAxis01SavedAcc.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtAxis01SavedAcc.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAxis01SavedAcc.ForeColor = System.Drawing.Color.Green;
            this.txtAxis01SavedAcc.Location = new System.Drawing.Point(651, 120);
            this.txtAxis01SavedAcc.Name = "txtAxis01SavedAcc";
            this.txtAxis01SavedAcc.ReadOnly = true;
            this.txtAxis01SavedAcc.Size = new System.Drawing.Size(96, 35);
            this.txtAxis01SavedAcc.TabIndex = 232;
            this.txtAxis01SavedAcc.Tag = "";
            this.txtAxis01SavedAcc.Text = "0.0";
            this.txtAxis01SavedAcc.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtAxis01SavedVelocity
            // 
            this.txtAxis01SavedVelocity.BackColor = System.Drawing.Color.White;
            this.txtAxis01SavedVelocity.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtAxis01SavedVelocity.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAxis01SavedVelocity.ForeColor = System.Drawing.Color.Green;
            this.txtAxis01SavedVelocity.Location = new System.Drawing.Point(339, 120);
            this.txtAxis01SavedVelocity.Name = "txtAxis01SavedVelocity";
            this.txtAxis01SavedVelocity.ReadOnly = true;
            this.txtAxis01SavedVelocity.Size = new System.Drawing.Size(96, 35);
            this.txtAxis01SavedVelocity.TabIndex = 231;
            this.txtAxis01SavedVelocity.Tag = "";
            this.txtAxis01SavedVelocity.Text = "0000";
            this.txtAxis01SavedVelocity.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtAxis01SavedPosition
            // 
            this.txtAxis01SavedPosition.BackColor = System.Drawing.Color.White;
            this.txtAxis01SavedPosition.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtAxis01SavedPosition.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAxis01SavedPosition.ForeColor = System.Drawing.Color.Green;
            this.txtAxis01SavedPosition.Location = new System.Drawing.Point(60, 120);
            this.txtAxis01SavedPosition.Name = "txtAxis01SavedPosition";
            this.txtAxis01SavedPosition.ReadOnly = true;
            this.txtAxis01SavedPosition.Size = new System.Drawing.Size(96, 35);
            this.txtAxis01SavedPosition.TabIndex = 230;
            this.txtAxis01SavedPosition.Tag = "";
            this.txtAxis01SavedPosition.Text = "0000.00";
            this.txtAxis01SavedPosition.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lblAxis01ModifydAcc
            // 
            this.lblAxis01ModifydAcc.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAxis01ModifydAcc.ForeColor = System.Drawing.Color.Orange;
            this.lblAxis01ModifydAcc.Location = new System.Drawing.Point(561, 159);
            this.lblAxis01ModifydAcc.Name = "lblAxis01ModifydAcc";
            this.lblAxis01ModifydAcc.Size = new System.Drawing.Size(262, 36);
            this.lblAxis01ModifydAcc.TabIndex = 229;
            this.lblAxis01ModifydAcc.Tag = "";
            this.lblAxis01ModifydAcc.Text = "修改的加速度时间(秒)";
            this.lblAxis01ModifydAcc.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblAxis01ModifyVelocity
            // 
            this.lblAxis01ModifyVelocity.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAxis01ModifyVelocity.ForeColor = System.Drawing.Color.Orange;
            this.lblAxis01ModifyVelocity.Location = new System.Drawing.Point(298, 159);
            this.lblAxis01ModifyVelocity.Name = "lblAxis01ModifyVelocity";
            this.lblAxis01ModifyVelocity.Size = new System.Drawing.Size(216, 36);
            this.lblAxis01ModifyVelocity.TabIndex = 228;
            this.lblAxis01ModifyVelocity.Tag = "";
            this.lblAxis01ModifyVelocity.Text = "修改的速度(mm/s)";
            this.lblAxis01ModifyVelocity.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblAxis01ModifyPosition
            // 
            this.lblAxis01ModifyPosition.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAxis01ModifyPosition.ForeColor = System.Drawing.Color.Orange;
            this.lblAxis01ModifyPosition.Location = new System.Drawing.Point(24, 159);
            this.lblAxis01ModifyPosition.Name = "lblAxis01ModifyPosition";
            this.lblAxis01ModifyPosition.Size = new System.Drawing.Size(212, 36);
            this.lblAxis01ModifyPosition.TabIndex = 227;
            this.lblAxis01ModifyPosition.Tag = "";
            this.lblAxis01ModifyPosition.Text = "修改的位置(mm)";
            this.lblAxis01ModifyPosition.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblAxis01SavedAcc
            // 
            this.lblAxis01SavedAcc.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAxis01SavedAcc.ForeColor = System.Drawing.Color.Green;
            this.lblAxis01SavedAcc.Location = new System.Drawing.Point(561, 80);
            this.lblAxis01SavedAcc.Name = "lblAxis01SavedAcc";
            this.lblAxis01SavedAcc.Size = new System.Drawing.Size(248, 36);
            this.lblAxis01SavedAcc.TabIndex = 226;
            this.lblAxis01SavedAcc.Tag = "";
            this.lblAxis01SavedAcc.Text = "保存的加速度时间(秒)";
            this.lblAxis01SavedAcc.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblAxis01SavedVelocity
            // 
            this.lblAxis01SavedVelocity.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAxis01SavedVelocity.ForeColor = System.Drawing.Color.Green;
            this.lblAxis01SavedVelocity.Location = new System.Drawing.Point(298, 80);
            this.lblAxis01SavedVelocity.Name = "lblAxis01SavedVelocity";
            this.lblAxis01SavedVelocity.Size = new System.Drawing.Size(207, 36);
            this.lblAxis01SavedVelocity.TabIndex = 225;
            this.lblAxis01SavedVelocity.Tag = "";
            this.lblAxis01SavedVelocity.Text = "保存的速度(mm/s)";
            this.lblAxis01SavedVelocity.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblAxis01SavedPosition
            // 
            this.lblAxis01SavedPosition.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAxis01SavedPosition.ForeColor = System.Drawing.Color.Green;
            this.lblAxis01SavedPosition.Location = new System.Drawing.Point(24, 80);
            this.lblAxis01SavedPosition.Name = "lblAxis01SavedPosition";
            this.lblAxis01SavedPosition.Size = new System.Drawing.Size(212, 36);
            this.lblAxis01SavedPosition.TabIndex = 224;
            this.lblAxis01SavedPosition.Tag = "";
            this.lblAxis01SavedPosition.Text = "保存的位置(mm)";
            this.lblAxis01SavedPosition.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnSaveSpeedOrOthers
            // 
            this.btnSaveSpeedOrOthers.BackColor = System.Drawing.Color.Blue;
            this.btnSaveSpeedOrOthers.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSaveSpeedOrOthers.ForeColor = System.Drawing.Color.White;
            this.btnSaveSpeedOrOthers.Location = new System.Drawing.Point(406, 903);
            this.btnSaveSpeedOrOthers.Name = "btnSaveSpeedOrOthers";
            this.btnSaveSpeedOrOthers.Size = new System.Drawing.Size(154, 93);
            this.btnSaveSpeedOrOthers.TabIndex = 238;
            this.btnSaveSpeedOrOthers.Tag = "";
            this.btnSaveSpeedOrOthers.Text = "更改速度及其它";
            this.btnSaveSpeedOrOthers.UseVisualStyleBackColor = false;
            this.btnSaveSpeedOrOthers.Click += new System.EventHandler(this.SaveSpeedAndOthersEvent);
            // 
            // btnSaveModifyPosition
            // 
            this.btnSaveModifyPosition.BackColor = System.Drawing.Color.Blue;
            this.btnSaveModifyPosition.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSaveModifyPosition.ForeColor = System.Drawing.Color.White;
            this.btnSaveModifyPosition.Location = new System.Drawing.Point(212, 903);
            this.btnSaveModifyPosition.Name = "btnSaveModifyPosition";
            this.btnSaveModifyPosition.Size = new System.Drawing.Size(154, 93);
            this.btnSaveModifyPosition.TabIndex = 237;
            this.btnSaveModifyPosition.Tag = "";
            this.btnSaveModifyPosition.Text = "保存修改位置";
            this.btnSaveModifyPosition.UseVisualStyleBackColor = false;
            this.btnSaveModifyPosition.Click += new System.EventHandler(this.AxisSaveCurrentPositionOrModifiedPositionEvent);
            // 
            // btnSaveCurrentPosition
            // 
            this.btnSaveCurrentPosition.BackColor = System.Drawing.Color.Blue;
            this.btnSaveCurrentPosition.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSaveCurrentPosition.ForeColor = System.Drawing.Color.White;
            this.btnSaveCurrentPosition.Location = new System.Drawing.Point(16, 903);
            this.btnSaveCurrentPosition.Name = "btnSaveCurrentPosition";
            this.btnSaveCurrentPosition.Size = new System.Drawing.Size(154, 93);
            this.btnSaveCurrentPosition.TabIndex = 236;
            this.btnSaveCurrentPosition.Tag = "";
            this.btnSaveCurrentPosition.Text = "保存当前位置";
            this.btnSaveCurrentPosition.UseVisualStyleBackColor = false;
            this.btnSaveCurrentPosition.Click += new System.EventHandler(this.AxisSaveCurrentPositionOrModifiedPositionEvent);
            // 
            // txtAxis02ModifyAcc
            // 
            this.txtAxis02ModifyAcc.BackColor = System.Drawing.Color.White;
            this.txtAxis02ModifyAcc.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtAxis02ModifyAcc.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAxis02ModifyAcc.ForeColor = System.Drawing.Color.Orange;
            this.txtAxis02ModifyAcc.Location = new System.Drawing.Point(651, 201);
            this.txtAxis02ModifyAcc.Name = "txtAxis02ModifyAcc";
            this.txtAxis02ModifyAcc.Size = new System.Drawing.Size(96, 35);
            this.txtAxis02ModifyAcc.TabIndex = 257;
            this.txtAxis02ModifyAcc.Tag = "";
            this.txtAxis02ModifyAcc.Text = "0.0";
            this.txtAxis02ModifyAcc.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtAxis02ModifyAcc.TextChanged += new System.EventHandler(this.ManualCtrlDataCheckEvent);
            this.txtAxis02ModifyAcc.DoubleClick += new System.EventHandler(this.TextBoxDoubleClickEvent);
            this.txtAxis02ModifyAcc.Leave += new System.EventHandler(this.ManualCtrlDataCheckLeaveEvent);
            this.txtAxis02ModifyAcc.MouseHover += new System.EventHandler(this.ACCTimeModifyMouseHover);
            // 
            // txtAxis02ModifyVelocity
            // 
            this.txtAxis02ModifyVelocity.BackColor = System.Drawing.Color.White;
            this.txtAxis02ModifyVelocity.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtAxis02ModifyVelocity.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAxis02ModifyVelocity.ForeColor = System.Drawing.Color.Orange;
            this.txtAxis02ModifyVelocity.Location = new System.Drawing.Point(339, 201);
            this.txtAxis02ModifyVelocity.Name = "txtAxis02ModifyVelocity";
            this.txtAxis02ModifyVelocity.Size = new System.Drawing.Size(96, 35);
            this.txtAxis02ModifyVelocity.TabIndex = 256;
            this.txtAxis02ModifyVelocity.Tag = "";
            this.txtAxis02ModifyVelocity.Text = "0000";
            this.txtAxis02ModifyVelocity.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtAxis02ModifyVelocity.TextChanged += new System.EventHandler(this.ManualCtrlDataCheckEvent);
            this.txtAxis02ModifyVelocity.DoubleClick += new System.EventHandler(this.TextBoxDoubleClickEvent);
            this.txtAxis02ModifyVelocity.Leave += new System.EventHandler(this.ManualCtrlDataCheckLeaveEvent);
            this.txtAxis02ModifyVelocity.MouseHover += new System.EventHandler(this.SpeedModifyMouseHover);
            // 
            // txtAxis02ModifyPosition
            // 
            this.txtAxis02ModifyPosition.BackColor = System.Drawing.Color.White;
            this.txtAxis02ModifyPosition.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtAxis02ModifyPosition.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAxis02ModifyPosition.ForeColor = System.Drawing.Color.Orange;
            this.txtAxis02ModifyPosition.Location = new System.Drawing.Point(60, 201);
            this.txtAxis02ModifyPosition.Name = "txtAxis02ModifyPosition";
            this.txtAxis02ModifyPosition.Size = new System.Drawing.Size(96, 35);
            this.txtAxis02ModifyPosition.TabIndex = 255;
            this.txtAxis02ModifyPosition.Tag = "";
            this.txtAxis02ModifyPosition.Text = "0000.00";
            this.txtAxis02ModifyPosition.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtAxis02ModifyPosition.TextChanged += new System.EventHandler(this.ManualCtrlDataCheckEvent);
            this.txtAxis02ModifyPosition.DoubleClick += new System.EventHandler(this.TextBoxDoubleClickEvent);
            this.txtAxis02ModifyPosition.Leave += new System.EventHandler(this.ManualCtrlDataCheckLeaveEvent);
            this.txtAxis02ModifyPosition.MouseHover += new System.EventHandler(this.PositionModifyMouseHover);
            // 
            // txtAxis02SavedAcc
            // 
            this.txtAxis02SavedAcc.BackColor = System.Drawing.Color.White;
            this.txtAxis02SavedAcc.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtAxis02SavedAcc.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAxis02SavedAcc.ForeColor = System.Drawing.Color.Green;
            this.txtAxis02SavedAcc.Location = new System.Drawing.Point(651, 120);
            this.txtAxis02SavedAcc.Name = "txtAxis02SavedAcc";
            this.txtAxis02SavedAcc.ReadOnly = true;
            this.txtAxis02SavedAcc.Size = new System.Drawing.Size(96, 35);
            this.txtAxis02SavedAcc.TabIndex = 254;
            this.txtAxis02SavedAcc.Tag = "";
            this.txtAxis02SavedAcc.Text = "0.0";
            this.txtAxis02SavedAcc.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtAxis02SavedVelocity
            // 
            this.txtAxis02SavedVelocity.BackColor = System.Drawing.Color.White;
            this.txtAxis02SavedVelocity.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtAxis02SavedVelocity.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAxis02SavedVelocity.ForeColor = System.Drawing.Color.Green;
            this.txtAxis02SavedVelocity.Location = new System.Drawing.Point(339, 120);
            this.txtAxis02SavedVelocity.Name = "txtAxis02SavedVelocity";
            this.txtAxis02SavedVelocity.ReadOnly = true;
            this.txtAxis02SavedVelocity.Size = new System.Drawing.Size(96, 35);
            this.txtAxis02SavedVelocity.TabIndex = 253;
            this.txtAxis02SavedVelocity.Tag = "";
            this.txtAxis02SavedVelocity.Text = "0000";
            this.txtAxis02SavedVelocity.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtAxis02SavedPosition
            // 
            this.txtAxis02SavedPosition.BackColor = System.Drawing.Color.White;
            this.txtAxis02SavedPosition.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtAxis02SavedPosition.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAxis02SavedPosition.ForeColor = System.Drawing.Color.Green;
            this.txtAxis02SavedPosition.Location = new System.Drawing.Point(60, 120);
            this.txtAxis02SavedPosition.Name = "txtAxis02SavedPosition";
            this.txtAxis02SavedPosition.ReadOnly = true;
            this.txtAxis02SavedPosition.Size = new System.Drawing.Size(96, 35);
            this.txtAxis02SavedPosition.TabIndex = 252;
            this.txtAxis02SavedPosition.Tag = "";
            this.txtAxis02SavedPosition.Text = "0000.00";
            this.txtAxis02SavedPosition.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lblAxis02ModifydAcc
            // 
            this.lblAxis02ModifydAcc.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAxis02ModifydAcc.ForeColor = System.Drawing.Color.Orange;
            this.lblAxis02ModifydAcc.Location = new System.Drawing.Point(561, 159);
            this.lblAxis02ModifydAcc.Name = "lblAxis02ModifydAcc";
            this.lblAxis02ModifydAcc.Size = new System.Drawing.Size(262, 36);
            this.lblAxis02ModifydAcc.TabIndex = 251;
            this.lblAxis02ModifydAcc.Tag = "";
            this.lblAxis02ModifydAcc.Text = "修改的加速度时间(秒)";
            this.lblAxis02ModifydAcc.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblAxis02ModifyVelocity
            // 
            this.lblAxis02ModifyVelocity.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAxis02ModifyVelocity.ForeColor = System.Drawing.Color.Orange;
            this.lblAxis02ModifyVelocity.Location = new System.Drawing.Point(298, 159);
            this.lblAxis02ModifyVelocity.Name = "lblAxis02ModifyVelocity";
            this.lblAxis02ModifyVelocity.Size = new System.Drawing.Size(216, 36);
            this.lblAxis02ModifyVelocity.TabIndex = 250;
            this.lblAxis02ModifyVelocity.Tag = "";
            this.lblAxis02ModifyVelocity.Text = "修改的速度(mm/s)";
            this.lblAxis02ModifyVelocity.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblAxis02ModifyPosition
            // 
            this.lblAxis02ModifyPosition.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAxis02ModifyPosition.ForeColor = System.Drawing.Color.Orange;
            this.lblAxis02ModifyPosition.Location = new System.Drawing.Point(24, 159);
            this.lblAxis02ModifyPosition.Name = "lblAxis02ModifyPosition";
            this.lblAxis02ModifyPosition.Size = new System.Drawing.Size(212, 36);
            this.lblAxis02ModifyPosition.TabIndex = 249;
            this.lblAxis02ModifyPosition.Tag = "";
            this.lblAxis02ModifyPosition.Text = "修改的位置(mm)";
            this.lblAxis02ModifyPosition.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblAxis02SavedAcc
            // 
            this.lblAxis02SavedAcc.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAxis02SavedAcc.ForeColor = System.Drawing.Color.Green;
            this.lblAxis02SavedAcc.Location = new System.Drawing.Point(561, 80);
            this.lblAxis02SavedAcc.Name = "lblAxis02SavedAcc";
            this.lblAxis02SavedAcc.Size = new System.Drawing.Size(248, 36);
            this.lblAxis02SavedAcc.TabIndex = 248;
            this.lblAxis02SavedAcc.Tag = "";
            this.lblAxis02SavedAcc.Text = "保存的加速度时间(秒)";
            this.lblAxis02SavedAcc.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblAxis02SavedVelocity
            // 
            this.lblAxis02SavedVelocity.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAxis02SavedVelocity.ForeColor = System.Drawing.Color.Green;
            this.lblAxis02SavedVelocity.Location = new System.Drawing.Point(298, 80);
            this.lblAxis02SavedVelocity.Name = "lblAxis02SavedVelocity";
            this.lblAxis02SavedVelocity.Size = new System.Drawing.Size(207, 36);
            this.lblAxis02SavedVelocity.TabIndex = 247;
            this.lblAxis02SavedVelocity.Tag = "";
            this.lblAxis02SavedVelocity.Text = "保存的速度(mm/s)";
            this.lblAxis02SavedVelocity.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblAxis02SavedPosition
            // 
            this.lblAxis02SavedPosition.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAxis02SavedPosition.ForeColor = System.Drawing.Color.Green;
            this.lblAxis02SavedPosition.Location = new System.Drawing.Point(24, 80);
            this.lblAxis02SavedPosition.Name = "lblAxis02SavedPosition";
            this.lblAxis02SavedPosition.Size = new System.Drawing.Size(212, 36);
            this.lblAxis02SavedPosition.TabIndex = 246;
            this.lblAxis02SavedPosition.Tag = "";
            this.lblAxis02SavedPosition.Text = "保存的位置(mm)";
            this.lblAxis02SavedPosition.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnAxis02JogOrInchSelector
            // 
            this.btnAxis02JogOrInchSelector.BackColor = System.Drawing.Color.Blue;
            this.btnAxis02JogOrInchSelector.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAxis02JogOrInchSelector.ForeColor = System.Drawing.Color.White;
            this.btnAxis02JogOrInchSelector.Location = new System.Drawing.Point(824, 105);
            this.btnAxis02JogOrInchSelector.Name = "btnAxis02JogOrInchSelector";
            this.btnAxis02JogOrInchSelector.Size = new System.Drawing.Size(130, 87);
            this.btnAxis02JogOrInchSelector.TabIndex = 245;
            this.btnAxis02JogOrInchSelector.Tag = "";
            this.btnAxis02JogOrInchSelector.Text = "点动";
            this.btnAxis02JogOrInchSelector.UseVisualStyleBackColor = false;
            this.btnAxis02JogOrInchSelector.Click += new System.EventHandler(this.InchFlagChangeEvent);
            // 
            // txtAxis02InchDistance
            // 
            this.txtAxis02InchDistance.BackColor = System.Drawing.Color.White;
            this.txtAxis02InchDistance.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtAxis02InchDistance.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAxis02InchDistance.Location = new System.Drawing.Point(690, 32);
            this.txtAxis02InchDistance.Name = "txtAxis02InchDistance";
            this.txtAxis02InchDistance.Size = new System.Drawing.Size(96, 35);
            this.txtAxis02InchDistance.TabIndex = 244;
            this.txtAxis02InchDistance.Tag = "";
            this.txtAxis02InchDistance.Text = "00.00";
            this.txtAxis02InchDistance.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtAxis02InchDistance.TextChanged += new System.EventHandler(this.ManualCtrlDataCheckEvent);
            this.txtAxis02InchDistance.DoubleClick += new System.EventHandler(this.TextBoxDoubleClickEvent);
            this.txtAxis02InchDistance.Leave += new System.EventHandler(this.ManualCtrlDataCheckLeaveEvent);
            this.txtAxis02InchDistance.MouseHover += new System.EventHandler(this.InchLengthMouseHover);
            // 
            // txtAxis02JogInchVelocity
            // 
            this.txtAxis02JogInchVelocity.BackColor = System.Drawing.Color.White;
            this.txtAxis02JogInchVelocity.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtAxis02JogInchVelocity.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAxis02JogInchVelocity.Location = new System.Drawing.Point(423, 33);
            this.txtAxis02JogInchVelocity.Name = "txtAxis02JogInchVelocity";
            this.txtAxis02JogInchVelocity.Size = new System.Drawing.Size(96, 35);
            this.txtAxis02JogInchVelocity.TabIndex = 243;
            this.txtAxis02JogInchVelocity.Tag = "";
            this.txtAxis02JogInchVelocity.Text = "000";
            this.txtAxis02JogInchVelocity.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtAxis02JogInchVelocity.TextChanged += new System.EventHandler(this.ManualCtrlDataCheckEvent);
            this.txtAxis02JogInchVelocity.DoubleClick += new System.EventHandler(this.TextBoxDoubleClickEvent);
            this.txtAxis02JogInchVelocity.Leave += new System.EventHandler(this.ManualCtrlDataCheckLeaveEvent);
            this.txtAxis02JogInchVelocity.MouseHover += new System.EventHandler(this.JogInchSpeedMouseHover);
            // 
            // lblAxis02InchDistance
            // 
            this.lblAxis02InchDistance.AutoSize = true;
            this.lblAxis02InchDistance.Font = new System.Drawing.Font("Times New Roman", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAxis02InchDistance.Location = new System.Drawing.Point(546, 39);
            this.lblAxis02InchDistance.Name = "lblAxis02InchDistance";
            this.lblAxis02InchDistance.Size = new System.Drawing.Size(132, 22);
            this.lblAxis02InchDistance.TabIndex = 242;
            this.lblAxis02InchDistance.Tag = "";
            this.lblAxis02InchDistance.Text = "寸动长度(mm)";
            this.lblAxis02InchDistance.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblAxis02JogInchVelocity
            // 
            this.lblAxis02JogInchVelocity.AutoSize = true;
            this.lblAxis02JogInchVelocity.Font = new System.Drawing.Font("Times New Roman", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAxis02JogInchVelocity.Location = new System.Drawing.Point(302, 39);
            this.lblAxis02JogInchVelocity.Name = "lblAxis02JogInchVelocity";
            this.lblAxis02JogInchVelocity.Size = new System.Drawing.Size(106, 22);
            this.lblAxis02JogInchVelocity.TabIndex = 241;
            this.lblAxis02JogInchVelocity.Tag = "";
            this.lblAxis02JogInchVelocity.Text = "速度(mm/s)";
            this.lblAxis02JogInchVelocity.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txtAxis02CurrentPosition
            // 
            this.txtAxis02CurrentPosition.BackColor = System.Drawing.Color.Yellow;
            this.txtAxis02CurrentPosition.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtAxis02CurrentPosition.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAxis02CurrentPosition.ForeColor = System.Drawing.Color.Black;
            this.txtAxis02CurrentPosition.Location = new System.Drawing.Point(184, 33);
            this.txtAxis02CurrentPosition.Name = "txtAxis02CurrentPosition";
            this.txtAxis02CurrentPosition.ReadOnly = true;
            this.txtAxis02CurrentPosition.Size = new System.Drawing.Size(98, 35);
            this.txtAxis02CurrentPosition.TabIndex = 240;
            this.txtAxis02CurrentPosition.Tag = "";
            this.txtAxis02CurrentPosition.Text = "0000.000";
            this.txtAxis02CurrentPosition.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtAxis03ModifyAcc
            // 
            this.txtAxis03ModifyAcc.BackColor = System.Drawing.Color.White;
            this.txtAxis03ModifyAcc.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtAxis03ModifyAcc.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAxis03ModifyAcc.ForeColor = System.Drawing.Color.Orange;
            this.txtAxis03ModifyAcc.Location = new System.Drawing.Point(651, 202);
            this.txtAxis03ModifyAcc.Name = "txtAxis03ModifyAcc";
            this.txtAxis03ModifyAcc.Size = new System.Drawing.Size(96, 35);
            this.txtAxis03ModifyAcc.TabIndex = 276;
            this.txtAxis03ModifyAcc.Tag = "";
            this.txtAxis03ModifyAcc.Text = "0.0";
            this.txtAxis03ModifyAcc.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtAxis03ModifyAcc.TextChanged += new System.EventHandler(this.ManualCtrlDataCheckEvent);
            this.txtAxis03ModifyAcc.DoubleClick += new System.EventHandler(this.TextBoxDoubleClickEvent);
            this.txtAxis03ModifyAcc.Leave += new System.EventHandler(this.ManualCtrlDataCheckLeaveEvent);
            this.txtAxis03ModifyAcc.MouseHover += new System.EventHandler(this.ACCTimeModifyMouseHover);
            // 
            // txtAxis03ModifyVelocity
            // 
            this.txtAxis03ModifyVelocity.BackColor = System.Drawing.Color.White;
            this.txtAxis03ModifyVelocity.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtAxis03ModifyVelocity.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAxis03ModifyVelocity.ForeColor = System.Drawing.Color.Orange;
            this.txtAxis03ModifyVelocity.Location = new System.Drawing.Point(339, 202);
            this.txtAxis03ModifyVelocity.Name = "txtAxis03ModifyVelocity";
            this.txtAxis03ModifyVelocity.Size = new System.Drawing.Size(96, 35);
            this.txtAxis03ModifyVelocity.TabIndex = 275;
            this.txtAxis03ModifyVelocity.Tag = "";
            this.txtAxis03ModifyVelocity.Text = "0000";
            this.txtAxis03ModifyVelocity.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtAxis03ModifyVelocity.TextChanged += new System.EventHandler(this.ManualCtrlDataCheckEvent);
            this.txtAxis03ModifyVelocity.DoubleClick += new System.EventHandler(this.TextBoxDoubleClickEvent);
            this.txtAxis03ModifyVelocity.Leave += new System.EventHandler(this.ManualCtrlDataCheckLeaveEvent);
            this.txtAxis03ModifyVelocity.MouseHover += new System.EventHandler(this.SpeedModifyMouseHover);
            // 
            // txtAxis03ModifyPosition
            // 
            this.txtAxis03ModifyPosition.BackColor = System.Drawing.Color.White;
            this.txtAxis03ModifyPosition.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtAxis03ModifyPosition.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAxis03ModifyPosition.ForeColor = System.Drawing.Color.Orange;
            this.txtAxis03ModifyPosition.Location = new System.Drawing.Point(60, 202);
            this.txtAxis03ModifyPosition.Name = "txtAxis03ModifyPosition";
            this.txtAxis03ModifyPosition.Size = new System.Drawing.Size(96, 35);
            this.txtAxis03ModifyPosition.TabIndex = 274;
            this.txtAxis03ModifyPosition.Tag = "";
            this.txtAxis03ModifyPosition.Text = "0000.00";
            this.txtAxis03ModifyPosition.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtAxis03ModifyPosition.TextChanged += new System.EventHandler(this.ManualCtrlDataCheckEvent);
            this.txtAxis03ModifyPosition.DoubleClick += new System.EventHandler(this.TextBoxDoubleClickEvent);
            this.txtAxis03ModifyPosition.Leave += new System.EventHandler(this.ManualCtrlDataCheckLeaveEvent);
            this.txtAxis03ModifyPosition.MouseHover += new System.EventHandler(this.PositionModifyMouseHover);
            // 
            // txtAxis03SavedAcc
            // 
            this.txtAxis03SavedAcc.BackColor = System.Drawing.Color.White;
            this.txtAxis03SavedAcc.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtAxis03SavedAcc.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAxis03SavedAcc.ForeColor = System.Drawing.Color.Green;
            this.txtAxis03SavedAcc.Location = new System.Drawing.Point(651, 120);
            this.txtAxis03SavedAcc.Name = "txtAxis03SavedAcc";
            this.txtAxis03SavedAcc.ReadOnly = true;
            this.txtAxis03SavedAcc.Size = new System.Drawing.Size(96, 35);
            this.txtAxis03SavedAcc.TabIndex = 273;
            this.txtAxis03SavedAcc.Tag = "";
            this.txtAxis03SavedAcc.Text = "0.0";
            this.txtAxis03SavedAcc.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtAxis03SavedVelocity
            // 
            this.txtAxis03SavedVelocity.BackColor = System.Drawing.Color.White;
            this.txtAxis03SavedVelocity.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtAxis03SavedVelocity.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAxis03SavedVelocity.ForeColor = System.Drawing.Color.Green;
            this.txtAxis03SavedVelocity.Location = new System.Drawing.Point(339, 120);
            this.txtAxis03SavedVelocity.Name = "txtAxis03SavedVelocity";
            this.txtAxis03SavedVelocity.ReadOnly = true;
            this.txtAxis03SavedVelocity.Size = new System.Drawing.Size(96, 35);
            this.txtAxis03SavedVelocity.TabIndex = 272;
            this.txtAxis03SavedVelocity.Tag = "";
            this.txtAxis03SavedVelocity.Text = "0000";
            this.txtAxis03SavedVelocity.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtAxis03SavedPosition
            // 
            this.txtAxis03SavedPosition.BackColor = System.Drawing.Color.White;
            this.txtAxis03SavedPosition.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtAxis03SavedPosition.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAxis03SavedPosition.ForeColor = System.Drawing.Color.Green;
            this.txtAxis03SavedPosition.Location = new System.Drawing.Point(60, 120);
            this.txtAxis03SavedPosition.Name = "txtAxis03SavedPosition";
            this.txtAxis03SavedPosition.ReadOnly = true;
            this.txtAxis03SavedPosition.Size = new System.Drawing.Size(96, 35);
            this.txtAxis03SavedPosition.TabIndex = 271;
            this.txtAxis03SavedPosition.Tag = "";
            this.txtAxis03SavedPosition.Text = "0000.00";
            this.txtAxis03SavedPosition.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lblAxis03ModifydAcc
            // 
            this.lblAxis03ModifydAcc.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAxis03ModifydAcc.ForeColor = System.Drawing.Color.Orange;
            this.lblAxis03ModifydAcc.Location = new System.Drawing.Point(561, 160);
            this.lblAxis03ModifydAcc.Name = "lblAxis03ModifydAcc";
            this.lblAxis03ModifydAcc.Size = new System.Drawing.Size(262, 36);
            this.lblAxis03ModifydAcc.TabIndex = 270;
            this.lblAxis03ModifydAcc.Tag = "";
            this.lblAxis03ModifydAcc.Text = "修改的加速度时间(秒)";
            this.lblAxis03ModifydAcc.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblAxis03ModifyVelocity
            // 
            this.lblAxis03ModifyVelocity.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAxis03ModifyVelocity.ForeColor = System.Drawing.Color.Orange;
            this.lblAxis03ModifyVelocity.Location = new System.Drawing.Point(298, 160);
            this.lblAxis03ModifyVelocity.Name = "lblAxis03ModifyVelocity";
            this.lblAxis03ModifyVelocity.Size = new System.Drawing.Size(216, 36);
            this.lblAxis03ModifyVelocity.TabIndex = 269;
            this.lblAxis03ModifyVelocity.Tag = "";
            this.lblAxis03ModifyVelocity.Text = "修改的速度(mm/s)";
            this.lblAxis03ModifyVelocity.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblAxis03ModifyPosition
            // 
            this.lblAxis03ModifyPosition.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAxis03ModifyPosition.ForeColor = System.Drawing.Color.Orange;
            this.lblAxis03ModifyPosition.Location = new System.Drawing.Point(24, 160);
            this.lblAxis03ModifyPosition.Name = "lblAxis03ModifyPosition";
            this.lblAxis03ModifyPosition.Size = new System.Drawing.Size(212, 36);
            this.lblAxis03ModifyPosition.TabIndex = 268;
            this.lblAxis03ModifyPosition.Tag = "";
            this.lblAxis03ModifyPosition.Text = "修改的位置(mm)";
            this.lblAxis03ModifyPosition.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblAxis03SavedAcc
            // 
            this.lblAxis03SavedAcc.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAxis03SavedAcc.ForeColor = System.Drawing.Color.Green;
            this.lblAxis03SavedAcc.Location = new System.Drawing.Point(561, 81);
            this.lblAxis03SavedAcc.Name = "lblAxis03SavedAcc";
            this.lblAxis03SavedAcc.Size = new System.Drawing.Size(248, 36);
            this.lblAxis03SavedAcc.TabIndex = 267;
            this.lblAxis03SavedAcc.Tag = "";
            this.lblAxis03SavedAcc.Text = "保存的加速度时间(秒)";
            this.lblAxis03SavedAcc.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblAxis03SavedVelocity
            // 
            this.lblAxis03SavedVelocity.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAxis03SavedVelocity.ForeColor = System.Drawing.Color.Green;
            this.lblAxis03SavedVelocity.Location = new System.Drawing.Point(298, 81);
            this.lblAxis03SavedVelocity.Name = "lblAxis03SavedVelocity";
            this.lblAxis03SavedVelocity.Size = new System.Drawing.Size(207, 36);
            this.lblAxis03SavedVelocity.TabIndex = 266;
            this.lblAxis03SavedVelocity.Tag = "";
            this.lblAxis03SavedVelocity.Text = "保存的速度(mm/s)";
            this.lblAxis03SavedVelocity.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblAxis03SavedPosition
            // 
            this.lblAxis03SavedPosition.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAxis03SavedPosition.ForeColor = System.Drawing.Color.Green;
            this.lblAxis03SavedPosition.Location = new System.Drawing.Point(24, 81);
            this.lblAxis03SavedPosition.Name = "lblAxis03SavedPosition";
            this.lblAxis03SavedPosition.Size = new System.Drawing.Size(212, 36);
            this.lblAxis03SavedPosition.TabIndex = 265;
            this.lblAxis03SavedPosition.Tag = "";
            this.lblAxis03SavedPosition.Text = "保存的位置(mm)";
            this.lblAxis03SavedPosition.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnAxis03JogOrInchSelector
            // 
            this.btnAxis03JogOrInchSelector.BackColor = System.Drawing.Color.Blue;
            this.btnAxis03JogOrInchSelector.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAxis03JogOrInchSelector.ForeColor = System.Drawing.Color.White;
            this.btnAxis03JogOrInchSelector.Location = new System.Drawing.Point(824, 104);
            this.btnAxis03JogOrInchSelector.Name = "btnAxis03JogOrInchSelector";
            this.btnAxis03JogOrInchSelector.Size = new System.Drawing.Size(130, 87);
            this.btnAxis03JogOrInchSelector.TabIndex = 264;
            this.btnAxis03JogOrInchSelector.Tag = "";
            this.btnAxis03JogOrInchSelector.Text = "点动";
            this.btnAxis03JogOrInchSelector.UseVisualStyleBackColor = false;
            this.btnAxis03JogOrInchSelector.Click += new System.EventHandler(this.InchFlagChangeEvent);
            // 
            // txtAxis03InchDistance
            // 
            this.txtAxis03InchDistance.BackColor = System.Drawing.Color.White;
            this.txtAxis03InchDistance.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtAxis03InchDistance.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAxis03InchDistance.Location = new System.Drawing.Point(690, 33);
            this.txtAxis03InchDistance.Name = "txtAxis03InchDistance";
            this.txtAxis03InchDistance.Size = new System.Drawing.Size(96, 35);
            this.txtAxis03InchDistance.TabIndex = 263;
            this.txtAxis03InchDistance.Tag = "";
            this.txtAxis03InchDistance.Text = "00.00";
            this.txtAxis03InchDistance.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtAxis03InchDistance.TextChanged += new System.EventHandler(this.ManualCtrlDataCheckEvent);
            this.txtAxis03InchDistance.DoubleClick += new System.EventHandler(this.TextBoxDoubleClickEvent);
            this.txtAxis03InchDistance.Leave += new System.EventHandler(this.ManualCtrlDataCheckLeaveEvent);
            this.txtAxis03InchDistance.MouseHover += new System.EventHandler(this.InchLengthMouseHover);
            // 
            // txtAxis03JogInchVelocity
            // 
            this.txtAxis03JogInchVelocity.BackColor = System.Drawing.Color.White;
            this.txtAxis03JogInchVelocity.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtAxis03JogInchVelocity.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAxis03JogInchVelocity.Location = new System.Drawing.Point(423, 33);
            this.txtAxis03JogInchVelocity.Name = "txtAxis03JogInchVelocity";
            this.txtAxis03JogInchVelocity.Size = new System.Drawing.Size(96, 35);
            this.txtAxis03JogInchVelocity.TabIndex = 262;
            this.txtAxis03JogInchVelocity.Tag = "";
            this.txtAxis03JogInchVelocity.Text = "000";
            this.txtAxis03JogInchVelocity.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtAxis03JogInchVelocity.TextChanged += new System.EventHandler(this.ManualCtrlDataCheckEvent);
            this.txtAxis03JogInchVelocity.DoubleClick += new System.EventHandler(this.TextBoxDoubleClickEvent);
            this.txtAxis03JogInchVelocity.Leave += new System.EventHandler(this.ManualCtrlDataCheckLeaveEvent);
            this.txtAxis03JogInchVelocity.MouseHover += new System.EventHandler(this.JogInchSpeedMouseHover);
            // 
            // lblAxis03InchDistance
            // 
            this.lblAxis03InchDistance.AutoSize = true;
            this.lblAxis03InchDistance.Font = new System.Drawing.Font("Times New Roman", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAxis03InchDistance.Location = new System.Drawing.Point(546, 39);
            this.lblAxis03InchDistance.Name = "lblAxis03InchDistance";
            this.lblAxis03InchDistance.Size = new System.Drawing.Size(132, 22);
            this.lblAxis03InchDistance.TabIndex = 261;
            this.lblAxis03InchDistance.Tag = "";
            this.lblAxis03InchDistance.Text = "寸动长度(mm)";
            this.lblAxis03InchDistance.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblAxis03JogInchVelocity
            // 
            this.lblAxis03JogInchVelocity.AutoSize = true;
            this.lblAxis03JogInchVelocity.Font = new System.Drawing.Font("Times New Roman", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAxis03JogInchVelocity.Location = new System.Drawing.Point(302, 39);
            this.lblAxis03JogInchVelocity.Name = "lblAxis03JogInchVelocity";
            this.lblAxis03JogInchVelocity.Size = new System.Drawing.Size(106, 22);
            this.lblAxis03JogInchVelocity.TabIndex = 260;
            this.lblAxis03JogInchVelocity.Tag = "";
            this.lblAxis03JogInchVelocity.Text = "速度(mm/s)";
            this.lblAxis03JogInchVelocity.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txtAxis03CurrentPosition
            // 
            this.txtAxis03CurrentPosition.BackColor = System.Drawing.Color.Yellow;
            this.txtAxis03CurrentPosition.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtAxis03CurrentPosition.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAxis03CurrentPosition.ForeColor = System.Drawing.Color.Black;
            this.txtAxis03CurrentPosition.Location = new System.Drawing.Point(184, 34);
            this.txtAxis03CurrentPosition.Name = "txtAxis03CurrentPosition";
            this.txtAxis03CurrentPosition.ReadOnly = true;
            this.txtAxis03CurrentPosition.Size = new System.Drawing.Size(98, 35);
            this.txtAxis03CurrentPosition.TabIndex = 259;
            this.txtAxis03CurrentPosition.Tag = "";
            this.txtAxis03CurrentPosition.Text = "0000.000";
            this.txtAxis03CurrentPosition.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtAxis04ModifyAcc
            // 
            this.txtAxis04ModifyAcc.BackColor = System.Drawing.Color.White;
            this.txtAxis04ModifyAcc.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtAxis04ModifyAcc.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAxis04ModifyAcc.ForeColor = System.Drawing.Color.Orange;
            this.txtAxis04ModifyAcc.Location = new System.Drawing.Point(651, 202);
            this.txtAxis04ModifyAcc.Name = "txtAxis04ModifyAcc";
            this.txtAxis04ModifyAcc.Size = new System.Drawing.Size(96, 35);
            this.txtAxis04ModifyAcc.TabIndex = 295;
            this.txtAxis04ModifyAcc.Tag = "";
            this.txtAxis04ModifyAcc.Text = "0.0";
            this.txtAxis04ModifyAcc.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtAxis04ModifyAcc.TextChanged += new System.EventHandler(this.ManualCtrlDataCheckEvent);
            this.txtAxis04ModifyAcc.DoubleClick += new System.EventHandler(this.TextBoxDoubleClickEvent);
            this.txtAxis04ModifyAcc.Leave += new System.EventHandler(this.ManualCtrlDataCheckLeaveEvent);
            this.txtAxis04ModifyAcc.MouseHover += new System.EventHandler(this.ACCTimeModifyMouseHover);
            // 
            // txtAxis04ModifyVelocity
            // 
            this.txtAxis04ModifyVelocity.BackColor = System.Drawing.Color.White;
            this.txtAxis04ModifyVelocity.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtAxis04ModifyVelocity.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAxis04ModifyVelocity.ForeColor = System.Drawing.Color.Orange;
            this.txtAxis04ModifyVelocity.Location = new System.Drawing.Point(339, 202);
            this.txtAxis04ModifyVelocity.Name = "txtAxis04ModifyVelocity";
            this.txtAxis04ModifyVelocity.Size = new System.Drawing.Size(96, 35);
            this.txtAxis04ModifyVelocity.TabIndex = 294;
            this.txtAxis04ModifyVelocity.Tag = "";
            this.txtAxis04ModifyVelocity.Text = "0000";
            this.txtAxis04ModifyVelocity.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtAxis04ModifyVelocity.TextChanged += new System.EventHandler(this.ManualCtrlDataCheckEvent);
            this.txtAxis04ModifyVelocity.DoubleClick += new System.EventHandler(this.TextBoxDoubleClickEvent);
            this.txtAxis04ModifyVelocity.Leave += new System.EventHandler(this.ManualCtrlDataCheckLeaveEvent);
            this.txtAxis04ModifyVelocity.MouseHover += new System.EventHandler(this.SpeedModifyMouseHover);
            // 
            // txtAxis04ModifyPosition
            // 
            this.txtAxis04ModifyPosition.BackColor = System.Drawing.Color.White;
            this.txtAxis04ModifyPosition.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtAxis04ModifyPosition.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAxis04ModifyPosition.ForeColor = System.Drawing.Color.Orange;
            this.txtAxis04ModifyPosition.Location = new System.Drawing.Point(60, 202);
            this.txtAxis04ModifyPosition.Name = "txtAxis04ModifyPosition";
            this.txtAxis04ModifyPosition.Size = new System.Drawing.Size(96, 35);
            this.txtAxis04ModifyPosition.TabIndex = 293;
            this.txtAxis04ModifyPosition.Tag = "";
            this.txtAxis04ModifyPosition.Text = "0000.00";
            this.txtAxis04ModifyPosition.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtAxis04ModifyPosition.TextChanged += new System.EventHandler(this.ManualCtrlDataCheckEvent);
            this.txtAxis04ModifyPosition.DoubleClick += new System.EventHandler(this.TextBoxDoubleClickEvent);
            this.txtAxis04ModifyPosition.Leave += new System.EventHandler(this.ManualCtrlDataCheckLeaveEvent);
            this.txtAxis04ModifyPosition.MouseHover += new System.EventHandler(this.PositionModifyMouseHover);
            // 
            // txtAxis04SavedAcc
            // 
            this.txtAxis04SavedAcc.BackColor = System.Drawing.Color.White;
            this.txtAxis04SavedAcc.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtAxis04SavedAcc.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAxis04SavedAcc.ForeColor = System.Drawing.Color.Green;
            this.txtAxis04SavedAcc.Location = new System.Drawing.Point(651, 122);
            this.txtAxis04SavedAcc.Name = "txtAxis04SavedAcc";
            this.txtAxis04SavedAcc.ReadOnly = true;
            this.txtAxis04SavedAcc.Size = new System.Drawing.Size(96, 35);
            this.txtAxis04SavedAcc.TabIndex = 292;
            this.txtAxis04SavedAcc.Tag = "";
            this.txtAxis04SavedAcc.Text = "0.0";
            this.txtAxis04SavedAcc.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtAxis04SavedVelocity
            // 
            this.txtAxis04SavedVelocity.BackColor = System.Drawing.Color.White;
            this.txtAxis04SavedVelocity.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtAxis04SavedVelocity.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAxis04SavedVelocity.ForeColor = System.Drawing.Color.Green;
            this.txtAxis04SavedVelocity.Location = new System.Drawing.Point(339, 122);
            this.txtAxis04SavedVelocity.Name = "txtAxis04SavedVelocity";
            this.txtAxis04SavedVelocity.ReadOnly = true;
            this.txtAxis04SavedVelocity.Size = new System.Drawing.Size(96, 35);
            this.txtAxis04SavedVelocity.TabIndex = 291;
            this.txtAxis04SavedVelocity.Tag = "";
            this.txtAxis04SavedVelocity.Text = "0000";
            this.txtAxis04SavedVelocity.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtAxis04SavedPosition
            // 
            this.txtAxis04SavedPosition.BackColor = System.Drawing.Color.White;
            this.txtAxis04SavedPosition.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtAxis04SavedPosition.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAxis04SavedPosition.ForeColor = System.Drawing.Color.Green;
            this.txtAxis04SavedPosition.Location = new System.Drawing.Point(60, 122);
            this.txtAxis04SavedPosition.Name = "txtAxis04SavedPosition";
            this.txtAxis04SavedPosition.ReadOnly = true;
            this.txtAxis04SavedPosition.Size = new System.Drawing.Size(96, 35);
            this.txtAxis04SavedPosition.TabIndex = 290;
            this.txtAxis04SavedPosition.Tag = "";
            this.txtAxis04SavedPosition.Text = "0000.00";
            this.txtAxis04SavedPosition.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lblAxis04ModifydAcc
            // 
            this.lblAxis04ModifydAcc.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAxis04ModifydAcc.ForeColor = System.Drawing.Color.Orange;
            this.lblAxis04ModifydAcc.Location = new System.Drawing.Point(561, 160);
            this.lblAxis04ModifydAcc.Name = "lblAxis04ModifydAcc";
            this.lblAxis04ModifydAcc.Size = new System.Drawing.Size(262, 36);
            this.lblAxis04ModifydAcc.TabIndex = 289;
            this.lblAxis04ModifydAcc.Tag = "";
            this.lblAxis04ModifydAcc.Text = "修改的加速度时间(秒)";
            this.lblAxis04ModifydAcc.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblAxis04ModifyVelocity
            // 
            this.lblAxis04ModifyVelocity.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAxis04ModifyVelocity.ForeColor = System.Drawing.Color.Orange;
            this.lblAxis04ModifyVelocity.Location = new System.Drawing.Point(298, 160);
            this.lblAxis04ModifyVelocity.Name = "lblAxis04ModifyVelocity";
            this.lblAxis04ModifyVelocity.Size = new System.Drawing.Size(216, 36);
            this.lblAxis04ModifyVelocity.TabIndex = 288;
            this.lblAxis04ModifyVelocity.Tag = "";
            this.lblAxis04ModifyVelocity.Text = "修改的速度(mm/s)";
            this.lblAxis04ModifyVelocity.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblAxis04ModifyPosition
            // 
            this.lblAxis04ModifyPosition.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAxis04ModifyPosition.ForeColor = System.Drawing.Color.Orange;
            this.lblAxis04ModifyPosition.Location = new System.Drawing.Point(24, 160);
            this.lblAxis04ModifyPosition.Name = "lblAxis04ModifyPosition";
            this.lblAxis04ModifyPosition.Size = new System.Drawing.Size(212, 36);
            this.lblAxis04ModifyPosition.TabIndex = 287;
            this.lblAxis04ModifyPosition.Tag = "";
            this.lblAxis04ModifyPosition.Text = "修改的位置(mm)";
            this.lblAxis04ModifyPosition.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblAxis04SavedAcc
            // 
            this.lblAxis04SavedAcc.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAxis04SavedAcc.ForeColor = System.Drawing.Color.Green;
            this.lblAxis04SavedAcc.Location = new System.Drawing.Point(561, 81);
            this.lblAxis04SavedAcc.Name = "lblAxis04SavedAcc";
            this.lblAxis04SavedAcc.Size = new System.Drawing.Size(248, 36);
            this.lblAxis04SavedAcc.TabIndex = 286;
            this.lblAxis04SavedAcc.Tag = "";
            this.lblAxis04SavedAcc.Text = "保存的加速度时间(秒)";
            this.lblAxis04SavedAcc.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblAxis04SavedVelocity
            // 
            this.lblAxis04SavedVelocity.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAxis04SavedVelocity.ForeColor = System.Drawing.Color.Green;
            this.lblAxis04SavedVelocity.Location = new System.Drawing.Point(298, 81);
            this.lblAxis04SavedVelocity.Name = "lblAxis04SavedVelocity";
            this.lblAxis04SavedVelocity.Size = new System.Drawing.Size(207, 36);
            this.lblAxis04SavedVelocity.TabIndex = 285;
            this.lblAxis04SavedVelocity.Tag = "";
            this.lblAxis04SavedVelocity.Text = "保存的速度(mm/s)";
            this.lblAxis04SavedVelocity.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblAxis04SavedPosition
            // 
            this.lblAxis04SavedPosition.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAxis04SavedPosition.ForeColor = System.Drawing.Color.Green;
            this.lblAxis04SavedPosition.Location = new System.Drawing.Point(24, 81);
            this.lblAxis04SavedPosition.Name = "lblAxis04SavedPosition";
            this.lblAxis04SavedPosition.Size = new System.Drawing.Size(212, 36);
            this.lblAxis04SavedPosition.TabIndex = 284;
            this.lblAxis04SavedPosition.Tag = "";
            this.lblAxis04SavedPosition.Text = "保存的位置(mm)";
            this.lblAxis04SavedPosition.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnAxis04JogOrInchSelector
            // 
            this.btnAxis04JogOrInchSelector.BackColor = System.Drawing.Color.Blue;
            this.btnAxis04JogOrInchSelector.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAxis04JogOrInchSelector.ForeColor = System.Drawing.Color.White;
            this.btnAxis04JogOrInchSelector.Location = new System.Drawing.Point(824, 105);
            this.btnAxis04JogOrInchSelector.Name = "btnAxis04JogOrInchSelector";
            this.btnAxis04JogOrInchSelector.Size = new System.Drawing.Size(130, 87);
            this.btnAxis04JogOrInchSelector.TabIndex = 283;
            this.btnAxis04JogOrInchSelector.Tag = "";
            this.btnAxis04JogOrInchSelector.Text = "点动";
            this.btnAxis04JogOrInchSelector.UseVisualStyleBackColor = false;
            this.btnAxis04JogOrInchSelector.Click += new System.EventHandler(this.InchFlagChangeEvent);
            // 
            // txtAxis04InchDistance
            // 
            this.txtAxis04InchDistance.BackColor = System.Drawing.Color.White;
            this.txtAxis04InchDistance.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtAxis04InchDistance.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAxis04InchDistance.Location = new System.Drawing.Point(690, 33);
            this.txtAxis04InchDistance.Name = "txtAxis04InchDistance";
            this.txtAxis04InchDistance.Size = new System.Drawing.Size(96, 35);
            this.txtAxis04InchDistance.TabIndex = 282;
            this.txtAxis04InchDistance.Tag = "";
            this.txtAxis04InchDistance.Text = "00.00";
            this.txtAxis04InchDistance.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtAxis04InchDistance.TextChanged += new System.EventHandler(this.ManualCtrlDataCheckEvent);
            this.txtAxis04InchDistance.DoubleClick += new System.EventHandler(this.TextBoxDoubleClickEvent);
            this.txtAxis04InchDistance.Leave += new System.EventHandler(this.ManualCtrlDataCheckLeaveEvent);
            this.txtAxis04InchDistance.MouseHover += new System.EventHandler(this.InchLengthMouseHover);
            // 
            // txtAxis04JogInchVelocity
            // 
            this.txtAxis04JogInchVelocity.BackColor = System.Drawing.Color.White;
            this.txtAxis04JogInchVelocity.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtAxis04JogInchVelocity.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAxis04JogInchVelocity.Location = new System.Drawing.Point(423, 34);
            this.txtAxis04JogInchVelocity.Name = "txtAxis04JogInchVelocity";
            this.txtAxis04JogInchVelocity.Size = new System.Drawing.Size(96, 35);
            this.txtAxis04JogInchVelocity.TabIndex = 281;
            this.txtAxis04JogInchVelocity.Tag = "";
            this.txtAxis04JogInchVelocity.Text = "000";
            this.txtAxis04JogInchVelocity.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtAxis04JogInchVelocity.TextChanged += new System.EventHandler(this.ManualCtrlDataCheckEvent);
            this.txtAxis04JogInchVelocity.DoubleClick += new System.EventHandler(this.TextBoxDoubleClickEvent);
            this.txtAxis04JogInchVelocity.Leave += new System.EventHandler(this.ManualCtrlDataCheckLeaveEvent);
            this.txtAxis04JogInchVelocity.MouseHover += new System.EventHandler(this.JogInchSpeedMouseHover);
            // 
            // lblAxis04InchDistance
            // 
            this.lblAxis04InchDistance.AutoSize = true;
            this.lblAxis04InchDistance.Font = new System.Drawing.Font("Times New Roman", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAxis04InchDistance.Location = new System.Drawing.Point(546, 40);
            this.lblAxis04InchDistance.Name = "lblAxis04InchDistance";
            this.lblAxis04InchDistance.Size = new System.Drawing.Size(132, 22);
            this.lblAxis04InchDistance.TabIndex = 280;
            this.lblAxis04InchDistance.Tag = "";
            this.lblAxis04InchDistance.Text = "寸动长度(mm)";
            this.lblAxis04InchDistance.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblAxis04JogInchVelocity
            // 
            this.lblAxis04JogInchVelocity.AutoSize = true;
            this.lblAxis04JogInchVelocity.Font = new System.Drawing.Font("Times New Roman", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAxis04JogInchVelocity.Location = new System.Drawing.Point(302, 40);
            this.lblAxis04JogInchVelocity.Name = "lblAxis04JogInchVelocity";
            this.lblAxis04JogInchVelocity.Size = new System.Drawing.Size(106, 22);
            this.lblAxis04JogInchVelocity.TabIndex = 279;
            this.lblAxis04JogInchVelocity.Tag = "";
            this.lblAxis04JogInchVelocity.Text = "速度(mm/s)";
            this.lblAxis04JogInchVelocity.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txtAxis04CurrentPosition
            // 
            this.txtAxis04CurrentPosition.BackColor = System.Drawing.Color.Yellow;
            this.txtAxis04CurrentPosition.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtAxis04CurrentPosition.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAxis04CurrentPosition.ForeColor = System.Drawing.Color.Black;
            this.txtAxis04CurrentPosition.Location = new System.Drawing.Point(184, 34);
            this.txtAxis04CurrentPosition.Name = "txtAxis04CurrentPosition";
            this.txtAxis04CurrentPosition.ReadOnly = true;
            this.txtAxis04CurrentPosition.Size = new System.Drawing.Size(98, 35);
            this.txtAxis04CurrentPosition.TabIndex = 278;
            this.txtAxis04CurrentPosition.Tag = "";
            this.txtAxis04CurrentPosition.Text = "0000.000";
            this.txtAxis04CurrentPosition.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // gbxAxis01
            // 
            this.gbxAxis01.Controls.Add(this.btnAxis01UnionFlag);
            this.gbxAxis01.Controls.Add(this.lblAxis01CurrentPosition);
            this.gbxAxis01.Controls.Add(this.btnAxis01JogOrInchSelector);
            this.gbxAxis01.Controls.Add(this.txtAxis01CurrentPosition);
            this.gbxAxis01.Controls.Add(this.lblAxis01InchDistance);
            this.gbxAxis01.Controls.Add(this.lblAxis01JogInchVelocity);
            this.gbxAxis01.Controls.Add(this.txtAxis01JogInchVelocity);
            this.gbxAxis01.Controls.Add(this.txtAxis01InchDistance);
            this.gbxAxis01.Controls.Add(this.lblAxis01SavedPosition);
            this.gbxAxis01.Controls.Add(this.lblAxis01SavedVelocity);
            this.gbxAxis01.Controls.Add(this.lblAxis01SavedAcc);
            this.gbxAxis01.Controls.Add(this.lblAxis01ModifyPosition);
            this.gbxAxis01.Controls.Add(this.lblAxis01ModifyVelocity);
            this.gbxAxis01.Controls.Add(this.lblAxis01ModifydAcc);
            this.gbxAxis01.Controls.Add(this.txtAxis01SavedPosition);
            this.gbxAxis01.Controls.Add(this.txtAxis01SavedVelocity);
            this.gbxAxis01.Controls.Add(this.txtAxis01SavedAcc);
            this.gbxAxis01.Controls.Add(this.txtAxis01ModifyPosition);
            this.gbxAxis01.Controls.Add(this.txtAxis01ModifyVelocity);
            this.gbxAxis01.Controls.Add(this.txtAxis01ModifyAcc);
            this.gbxAxis01.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbxAxis01.Location = new System.Drawing.Point(584, 4);
            this.gbxAxis01.Name = "gbxAxis01";
            this.gbxAxis01.Size = new System.Drawing.Size(957, 248);
            this.gbxAxis01.TabIndex = 296;
            this.gbxAxis01.TabStop = false;
            this.gbxAxis01.Text = "轴01";
            // 
            // btnAxis01UnionFlag
            // 
            this.btnAxis01UnionFlag.BackColor = System.Drawing.SystemColors.ControlDark;
            this.btnAxis01UnionFlag.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAxis01UnionFlag.ForeColor = System.Drawing.Color.White;
            this.btnAxis01UnionFlag.Location = new System.Drawing.Point(824, 32);
            this.btnAxis01UnionFlag.Name = "btnAxis01UnionFlag";
            this.btnAxis01UnionFlag.Size = new System.Drawing.Size(130, 50);
            this.btnAxis01UnionFlag.TabIndex = 300;
            this.btnAxis01UnionFlag.Tag = "";
            this.btnAxis01UnionFlag.Text = "不联合";
            this.btnAxis01UnionFlag.UseVisualStyleBackColor = false;
            this.btnAxis01UnionFlag.Click += new System.EventHandler(this.UnionFlagChangeEvent);
            // 
            // lblAxis02CurrentPosition
            // 
            this.lblAxis02CurrentPosition.AutoSize = true;
            this.lblAxis02CurrentPosition.BackColor = System.Drawing.Color.Yellow;
            this.lblAxis02CurrentPosition.Font = new System.Drawing.Font("Times New Roman", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAxis02CurrentPosition.Location = new System.Drawing.Point(34, 39);
            this.lblAxis02CurrentPosition.Name = "lblAxis02CurrentPosition";
            this.lblAxis02CurrentPosition.Size = new System.Drawing.Size(132, 22);
            this.lblAxis02CurrentPosition.TabIndex = 188;
            this.lblAxis02CurrentPosition.Tag = "";
            this.lblAxis02CurrentPosition.Text = "当前位置(mm)";
            this.lblAxis02CurrentPosition.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblAxis03CurrentPosition
            // 
            this.lblAxis03CurrentPosition.AutoSize = true;
            this.lblAxis03CurrentPosition.BackColor = System.Drawing.Color.Yellow;
            this.lblAxis03CurrentPosition.Font = new System.Drawing.Font("Times New Roman", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAxis03CurrentPosition.Location = new System.Drawing.Point(34, 40);
            this.lblAxis03CurrentPosition.Name = "lblAxis03CurrentPosition";
            this.lblAxis03CurrentPosition.Size = new System.Drawing.Size(132, 22);
            this.lblAxis03CurrentPosition.TabIndex = 297;
            this.lblAxis03CurrentPosition.Tag = "";
            this.lblAxis03CurrentPosition.Text = "当前位置(mm)";
            this.lblAxis03CurrentPosition.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblAxis04CurrentPosition
            // 
            this.lblAxis04CurrentPosition.AutoSize = true;
            this.lblAxis04CurrentPosition.BackColor = System.Drawing.Color.Yellow;
            this.lblAxis04CurrentPosition.Font = new System.Drawing.Font("Times New Roman", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAxis04CurrentPosition.Location = new System.Drawing.Point(34, 40);
            this.lblAxis04CurrentPosition.Name = "lblAxis04CurrentPosition";
            this.lblAxis04CurrentPosition.Size = new System.Drawing.Size(132, 22);
            this.lblAxis04CurrentPosition.TabIndex = 298;
            this.lblAxis04CurrentPosition.Tag = "";
            this.lblAxis04CurrentPosition.Text = "当前位置(mm)";
            this.lblAxis04CurrentPosition.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // gbxAxis02
            // 
            this.gbxAxis02.Controls.Add(this.btnAxis02UnionFlag);
            this.gbxAxis02.Controls.Add(this.lblAxis02CurrentPosition);
            this.gbxAxis02.Controls.Add(this.btnAxis02JogOrInchSelector);
            this.gbxAxis02.Controls.Add(this.txtAxis02InchDistance);
            this.gbxAxis02.Controls.Add(this.txtAxis02CurrentPosition);
            this.gbxAxis02.Controls.Add(this.lblAxis02JogInchVelocity);
            this.gbxAxis02.Controls.Add(this.lblAxis02InchDistance);
            this.gbxAxis02.Controls.Add(this.txtAxis02JogInchVelocity);
            this.gbxAxis02.Controls.Add(this.lblAxis02SavedPosition);
            this.gbxAxis02.Controls.Add(this.lblAxis02SavedVelocity);
            this.gbxAxis02.Controls.Add(this.lblAxis02SavedAcc);
            this.gbxAxis02.Controls.Add(this.lblAxis02ModifyPosition);
            this.gbxAxis02.Controls.Add(this.lblAxis02ModifyVelocity);
            this.gbxAxis02.Controls.Add(this.lblAxis02ModifydAcc);
            this.gbxAxis02.Controls.Add(this.txtAxis02SavedPosition);
            this.gbxAxis02.Controls.Add(this.txtAxis02SavedVelocity);
            this.gbxAxis02.Controls.Add(this.txtAxis02SavedAcc);
            this.gbxAxis02.Controls.Add(this.txtAxis02ModifyPosition);
            this.gbxAxis02.Controls.Add(this.txtAxis02ModifyVelocity);
            this.gbxAxis02.Controls.Add(this.txtAxis02ModifyAcc);
            this.gbxAxis02.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbxAxis02.Location = new System.Drawing.Point(584, 256);
            this.gbxAxis02.Name = "gbxAxis02";
            this.gbxAxis02.Size = new System.Drawing.Size(957, 248);
            this.gbxAxis02.TabIndex = 297;
            this.gbxAxis02.TabStop = false;
            this.gbxAxis02.Text = "轴02";
            // 
            // btnAxis02UnionFlag
            // 
            this.btnAxis02UnionFlag.BackColor = System.Drawing.SystemColors.ControlDark;
            this.btnAxis02UnionFlag.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAxis02UnionFlag.ForeColor = System.Drawing.Color.White;
            this.btnAxis02UnionFlag.Location = new System.Drawing.Point(824, 30);
            this.btnAxis02UnionFlag.Name = "btnAxis02UnionFlag";
            this.btnAxis02UnionFlag.Size = new System.Drawing.Size(130, 50);
            this.btnAxis02UnionFlag.TabIndex = 301;
            this.btnAxis02UnionFlag.Tag = "";
            this.btnAxis02UnionFlag.Text = "不联合";
            this.btnAxis02UnionFlag.UseVisualStyleBackColor = false;
            this.btnAxis02UnionFlag.Click += new System.EventHandler(this.UnionFlagChangeEvent);
            // 
            // gbxAxis03
            // 
            this.gbxAxis03.Controls.Add(this.btnAxis03UnionFlag);
            this.gbxAxis03.Controls.Add(this.lblAxis03CurrentPosition);
            this.gbxAxis03.Controls.Add(this.btnAxis03JogOrInchSelector);
            this.gbxAxis03.Controls.Add(this.txtAxis03InchDistance);
            this.gbxAxis03.Controls.Add(this.txtAxis03CurrentPosition);
            this.gbxAxis03.Controls.Add(this.lblAxis03JogInchVelocity);
            this.gbxAxis03.Controls.Add(this.lblAxis03InchDistance);
            this.gbxAxis03.Controls.Add(this.txtAxis03JogInchVelocity);
            this.gbxAxis03.Controls.Add(this.lblAxis03SavedPosition);
            this.gbxAxis03.Controls.Add(this.lblAxis03SavedVelocity);
            this.gbxAxis03.Controls.Add(this.lblAxis03SavedAcc);
            this.gbxAxis03.Controls.Add(this.lblAxis03ModifyPosition);
            this.gbxAxis03.Controls.Add(this.lblAxis03ModifyVelocity);
            this.gbxAxis03.Controls.Add(this.lblAxis03ModifydAcc);
            this.gbxAxis03.Controls.Add(this.txtAxis03SavedPosition);
            this.gbxAxis03.Controls.Add(this.txtAxis03SavedVelocity);
            this.gbxAxis03.Controls.Add(this.txtAxis03SavedAcc);
            this.gbxAxis03.Controls.Add(this.txtAxis03ModifyPosition);
            this.gbxAxis03.Controls.Add(this.txtAxis03ModifyVelocity);
            this.gbxAxis03.Controls.Add(this.txtAxis03ModifyAcc);
            this.gbxAxis03.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbxAxis03.Location = new System.Drawing.Point(584, 510);
            this.gbxAxis03.Name = "gbxAxis03";
            this.gbxAxis03.Size = new System.Drawing.Size(957, 248);
            this.gbxAxis03.TabIndex = 298;
            this.gbxAxis03.TabStop = false;
            this.gbxAxis03.Text = "轴03";
            // 
            // btnAxis03UnionFlag
            // 
            this.btnAxis03UnionFlag.BackColor = System.Drawing.SystemColors.ControlDark;
            this.btnAxis03UnionFlag.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAxis03UnionFlag.ForeColor = System.Drawing.Color.White;
            this.btnAxis03UnionFlag.Location = new System.Drawing.Point(824, 32);
            this.btnAxis03UnionFlag.Name = "btnAxis03UnionFlag";
            this.btnAxis03UnionFlag.Size = new System.Drawing.Size(130, 50);
            this.btnAxis03UnionFlag.TabIndex = 302;
            this.btnAxis03UnionFlag.Tag = "";
            this.btnAxis03UnionFlag.Text = "不联合";
            this.btnAxis03UnionFlag.UseVisualStyleBackColor = false;
            this.btnAxis03UnionFlag.Click += new System.EventHandler(this.UnionFlagChangeEvent);
            // 
            // gbxAxis04
            // 
            this.gbxAxis04.Controls.Add(this.btnAxis04UnionFlag);
            this.gbxAxis04.Controls.Add(this.lblAxis04CurrentPosition);
            this.gbxAxis04.Controls.Add(this.btnAxis04JogOrInchSelector);
            this.gbxAxis04.Controls.Add(this.txtAxis04InchDistance);
            this.gbxAxis04.Controls.Add(this.lblAxis04JogInchVelocity);
            this.gbxAxis04.Controls.Add(this.lblAxis04InchDistance);
            this.gbxAxis04.Controls.Add(this.txtAxis04JogInchVelocity);
            this.gbxAxis04.Controls.Add(this.lblAxis04SavedPosition);
            this.gbxAxis04.Controls.Add(this.lblAxis04SavedVelocity);
            this.gbxAxis04.Controls.Add(this.lblAxis04SavedAcc);
            this.gbxAxis04.Controls.Add(this.txtAxis04CurrentPosition);
            this.gbxAxis04.Controls.Add(this.txtAxis04ModifyAcc);
            this.gbxAxis04.Controls.Add(this.lblAxis04ModifyPosition);
            this.gbxAxis04.Controls.Add(this.txtAxis04ModifyVelocity);
            this.gbxAxis04.Controls.Add(this.lblAxis04ModifyVelocity);
            this.gbxAxis04.Controls.Add(this.txtAxis04ModifyPosition);
            this.gbxAxis04.Controls.Add(this.lblAxis04ModifydAcc);
            this.gbxAxis04.Controls.Add(this.txtAxis04SavedAcc);
            this.gbxAxis04.Controls.Add(this.txtAxis04SavedPosition);
            this.gbxAxis04.Controls.Add(this.txtAxis04SavedVelocity);
            this.gbxAxis04.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbxAxis04.Location = new System.Drawing.Point(584, 764);
            this.gbxAxis04.Name = "gbxAxis04";
            this.gbxAxis04.Size = new System.Drawing.Size(957, 248);
            this.gbxAxis04.TabIndex = 299;
            this.gbxAxis04.TabStop = false;
            this.gbxAxis04.Text = "轴04";
            // 
            // btnAxis04UnionFlag
            // 
            this.btnAxis04UnionFlag.BackColor = System.Drawing.SystemColors.ControlDark;
            this.btnAxis04UnionFlag.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAxis04UnionFlag.ForeColor = System.Drawing.Color.White;
            this.btnAxis04UnionFlag.Location = new System.Drawing.Point(824, 32);
            this.btnAxis04UnionFlag.Name = "btnAxis04UnionFlag";
            this.btnAxis04UnionFlag.Size = new System.Drawing.Size(130, 50);
            this.btnAxis04UnionFlag.TabIndex = 302;
            this.btnAxis04UnionFlag.Tag = "";
            this.btnAxis04UnionFlag.Text = "不联合";
            this.btnAxis04UnionFlag.UseVisualStyleBackColor = false;
            this.btnAxis04UnionFlag.Click += new System.EventHandler(this.UnionFlagChangeEvent);
            // 
            // btnUnionPointNameChange
            // 
            this.btnUnionPointNameChange.BackColor = System.Drawing.Color.Blue;
            this.btnUnionPointNameChange.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnUnionPointNameChange.ForeColor = System.Drawing.Color.White;
            this.btnUnionPointNameChange.Location = new System.Drawing.Point(351, 296);
            this.btnUnionPointNameChange.Name = "btnUnionPointNameChange";
            this.btnUnionPointNameChange.Size = new System.Drawing.Size(208, 82);
            this.btnUnionPointNameChange.TabIndex = 300;
            this.btnUnionPointNameChange.Tag = "";
            this.btnUnionPointNameChange.Text = "更改点位号名称";
            this.btnUnionPointNameChange.UseVisualStyleBackColor = false;
            this.btnUnionPointNameChange.Click += new System.EventHandler(this.SavePointNameEvent);
            // 
            // btnAxis04PlusMove
            // 
            this.btnAxis04PlusMove.ArrowColor = System.Drawing.Color.DeepPink;
            this.btnAxis04PlusMove.BackColor = System.Drawing.SystemColors.Control;
            this.btnAxis04PlusMove.BorderColor = null;
            this.btnAxis04PlusMove.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnAxis04PlusMove.Direction = UCArrow.ArrowDirection.Right;
            this.btnAxis04PlusMove.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAxis04PlusMove.ForeColor = System.Drawing.Color.White;
            this.btnAxis04PlusMove.Location = new System.Drawing.Point(417, 474);
            this.btnAxis04PlusMove.Name = "btnAxis04PlusMove";
            this.btnAxis04PlusMove.Size = new System.Drawing.Size(127, 68);
            this.btnAxis04PlusMove.TabIndex = 308;
            this.btnAxis04PlusMove.Text = "轴04+";
            this.btnAxis04PlusMove.Visible = false;
            this.btnAxis04PlusMove.MouseDown += new System.Windows.Forms.MouseEventHandler(this.AxisJogInchMove);
            this.btnAxis04PlusMove.MouseLeave += new System.EventHandler(this.AxisJogInchMouseLeaveEvent);
            this.btnAxis04PlusMove.MouseUp += new System.Windows.Forms.MouseEventHandler(this.AxisJogInchMouseUpEvent);
            // 
            // btnAxis04MinusMove
            // 
            this.btnAxis04MinusMove.ArrowColor = System.Drawing.Color.DeepPink;
            this.btnAxis04MinusMove.BorderColor = null;
            this.btnAxis04MinusMove.Direction = UCArrow.ArrowDirection.Left;
            this.btnAxis04MinusMove.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAxis04MinusMove.ForeColor = System.Drawing.Color.White;
            this.btnAxis04MinusMove.Location = new System.Drawing.Point(417, 400);
            this.btnAxis04MinusMove.Name = "btnAxis04MinusMove";
            this.btnAxis04MinusMove.Size = new System.Drawing.Size(127, 68);
            this.btnAxis04MinusMove.TabIndex = 307;
            this.btnAxis04MinusMove.Text = "轴04-";
            this.btnAxis04MinusMove.Visible = false;
            this.btnAxis04MinusMove.MouseDown += new System.Windows.Forms.MouseEventHandler(this.AxisJogInchMove);
            this.btnAxis04MinusMove.MouseLeave += new System.EventHandler(this.AxisJogInchMouseLeaveEvent);
            this.btnAxis04MinusMove.MouseUp += new System.Windows.Forms.MouseEventHandler(this.AxisJogInchMouseUpEvent);
            // 
            // btnAxis03PlusMove
            // 
            this.btnAxis03PlusMove.ArrowColor = System.Drawing.Color.DarkOrange;
            this.btnAxis03PlusMove.BorderColor = null;
            this.btnAxis03PlusMove.Direction = UCArrow.ArrowDirection.Right;
            this.btnAxis03PlusMove.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAxis03PlusMove.ForeColor = System.Drawing.Color.White;
            this.btnAxis03PlusMove.Location = new System.Drawing.Point(240, 743);
            this.btnAxis03PlusMove.Name = "btnAxis03PlusMove";
            this.btnAxis03PlusMove.Size = new System.Drawing.Size(138, 129);
            this.btnAxis03PlusMove.TabIndex = 306;
            this.btnAxis03PlusMove.Text = "R1轴+";
            this.btnAxis03PlusMove.MouseDown += new System.Windows.Forms.MouseEventHandler(this.AxisJogInchMove);
            this.btnAxis03PlusMove.MouseLeave += new System.EventHandler(this.AxisJogInchMouseLeaveEvent);
            this.btnAxis03PlusMove.MouseUp += new System.Windows.Forms.MouseEventHandler(this.AxisJogInchMouseUpEvent);
            // 
            // btnAxis02PlusMove
            // 
            this.btnAxis02PlusMove.ArrowColor = System.Drawing.Color.Blue;
            this.btnAxis02PlusMove.BorderColor = null;
            this.btnAxis02PlusMove.Direction = UCArrow.ArrowDirection.Bottom;
            this.btnAxis02PlusMove.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAxis02PlusMove.ForeColor = System.Drawing.Color.White;
            this.btnAxis02PlusMove.Location = new System.Drawing.Point(141, 609);
            this.btnAxis02PlusMove.Name = "btnAxis02PlusMove";
            this.btnAxis02PlusMove.Size = new System.Drawing.Size(129, 138);
            this.btnAxis02PlusMove.TabIndex = 304;
            this.btnAxis02PlusMove.Text = "Y1轴+";
            this.btnAxis02PlusMove.MouseDown += new System.Windows.Forms.MouseEventHandler(this.AxisJogInchMove);
            this.btnAxis02PlusMove.MouseLeave += new System.EventHandler(this.AxisJogInchMouseLeaveEvent);
            this.btnAxis02PlusMove.MouseUp += new System.Windows.Forms.MouseEventHandler(this.AxisJogInchMouseUpEvent);
            // 
            // btnAxis02MinusMove
            // 
            this.btnAxis02MinusMove.ArrowColor = System.Drawing.Color.Blue;
            this.btnAxis02MinusMove.BorderColor = null;
            this.btnAxis02MinusMove.Direction = UCArrow.ArrowDirection.Top;
            this.btnAxis02MinusMove.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAxis02MinusMove.ForeColor = System.Drawing.Color.White;
            this.btnAxis02MinusMove.Location = new System.Drawing.Point(141, 400);
            this.btnAxis02MinusMove.Name = "btnAxis02MinusMove";
            this.btnAxis02MinusMove.Size = new System.Drawing.Size(129, 138);
            this.btnAxis02MinusMove.TabIndex = 303;
            this.btnAxis02MinusMove.Text = "Y1轴-";
            this.btnAxis02MinusMove.MouseDown += new System.Windows.Forms.MouseEventHandler(this.AxisJogInchMove);
            this.btnAxis02MinusMove.MouseLeave += new System.EventHandler(this.AxisJogInchMouseLeaveEvent);
            this.btnAxis02MinusMove.MouseUp += new System.Windows.Forms.MouseEventHandler(this.AxisJogInchMouseUpEvent);
            // 
            // btnAxis01MinusMove
            // 
            this.btnAxis01MinusMove.ArrowColor = System.Drawing.Color.Green;
            this.btnAxis01MinusMove.BorderColor = null;
            this.btnAxis01MinusMove.Direction = UCArrow.ArrowDirection.Right;
            this.btnAxis01MinusMove.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAxis01MinusMove.ForeColor = System.Drawing.Color.White;
            this.btnAxis01MinusMove.Location = new System.Drawing.Point(240, 510);
            this.btnAxis01MinusMove.Name = "btnAxis01MinusMove";
            this.btnAxis01MinusMove.Size = new System.Drawing.Size(138, 129);
            this.btnAxis01MinusMove.TabIndex = 302;
            this.btnAxis01MinusMove.Text = "X1轴-";
            this.btnAxis01MinusMove.MouseDown += new System.Windows.Forms.MouseEventHandler(this.AxisJogInchMove);
            this.btnAxis01MinusMove.MouseLeave += new System.EventHandler(this.AxisJogInchMouseLeaveEvent);
            this.btnAxis01MinusMove.MouseUp += new System.Windows.Forms.MouseEventHandler(this.AxisJogInchMouseUpEvent);
            // 
            // btnAxis01PlusMove
            // 
            this.btnAxis01PlusMove.ArrowColor = System.Drawing.Color.Green;
            this.btnAxis01PlusMove.BorderColor = null;
            this.btnAxis01PlusMove.Direction = UCArrow.ArrowDirection.Left;
            this.btnAxis01PlusMove.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAxis01PlusMove.ForeColor = System.Drawing.Color.White;
            this.btnAxis01PlusMove.Location = new System.Drawing.Point(33, 510);
            this.btnAxis01PlusMove.Name = "btnAxis01PlusMove";
            this.btnAxis01PlusMove.Size = new System.Drawing.Size(138, 129);
            this.btnAxis01PlusMove.TabIndex = 301;
            this.btnAxis01PlusMove.Text = "X1轴+";
            this.btnAxis01PlusMove.MouseDown += new System.Windows.Forms.MouseEventHandler(this.AxisJogInchMove);
            this.btnAxis01PlusMove.MouseLeave += new System.EventHandler(this.AxisJogInchMouseLeaveEvent);
            this.btnAxis01PlusMove.MouseUp += new System.Windows.Forms.MouseEventHandler(this.AxisJogInchMouseUpEvent);
            // 
            // btnAxisUnionRun
            // 
            this.btnAxisUnionRun.BackColor = System.Drawing.Color.DarkOrange;
            this.btnAxisUnionRun.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAxisUnionRun.ForeColor = System.Drawing.Color.Black;
            this.btnAxisUnionRun.Location = new System.Drawing.Point(338, 24);
            this.btnAxisUnionRun.Name = "btnAxisUnionRun";
            this.btnAxisUnionRun.Size = new System.Drawing.Size(222, 76);
            this.btnAxisUnionRun.TabIndex = 311;
            this.btnAxisUnionRun.Tag = "";
            this.btnAxisUnionRun.Text = "点位号联动运行";
            this.btnAxisUnionRun.UseVisualStyleBackColor = false;
            this.btnAxisUnionRun.Click += new System.EventHandler(this.AxisUnionMove);
            // 
            // txtAxis02SelectedPointNameChange
            // 
            this.txtAxis02SelectedPointNameChange.BackColor = System.Drawing.Color.White;
            this.txtAxis02SelectedPointNameChange.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtAxis02SelectedPointNameChange.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAxis02SelectedPointNameChange.ForeColor = System.Drawing.Color.Orange;
            this.txtAxis02SelectedPointNameChange.Location = new System.Drawing.Point(218, 154);
            this.txtAxis02SelectedPointNameChange.Name = "txtAxis02SelectedPointNameChange";
            this.txtAxis02SelectedPointNameChange.Size = new System.Drawing.Size(340, 35);
            this.txtAxis02SelectedPointNameChange.TabIndex = 196;
            this.txtAxis02SelectedPointNameChange.Tag = "";
            this.txtAxis02SelectedPointNameChange.Text = "等待位置";
            this.txtAxis02SelectedPointNameChange.DoubleClick += new System.EventHandler(this.TextBoxDoubleClickEvent);
            // 
            // lblAxis02PointNameTitle
            // 
            this.lblAxis02PointNameTitle.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAxis02PointNameTitle.ForeColor = System.Drawing.Color.Orange;
            this.lblAxis02PointNameTitle.Location = new System.Drawing.Point(20, 154);
            this.lblAxis02PointNameTitle.Name = "lblAxis02PointNameTitle";
            this.lblAxis02PointNameTitle.Size = new System.Drawing.Size(188, 39);
            this.lblAxis02PointNameTitle.TabIndex = 197;
            this.lblAxis02PointNameTitle.Tag = "";
            this.lblAxis02PointNameTitle.Text = "轴02点位名称:";
            this.lblAxis02PointNameTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtAxis03SelectedPointNameChange
            // 
            this.txtAxis03SelectedPointNameChange.BackColor = System.Drawing.Color.White;
            this.txtAxis03SelectedPointNameChange.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtAxis03SelectedPointNameChange.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAxis03SelectedPointNameChange.ForeColor = System.Drawing.Color.Orange;
            this.txtAxis03SelectedPointNameChange.Location = new System.Drawing.Point(218, 202);
            this.txtAxis03SelectedPointNameChange.Name = "txtAxis03SelectedPointNameChange";
            this.txtAxis03SelectedPointNameChange.Size = new System.Drawing.Size(340, 35);
            this.txtAxis03SelectedPointNameChange.TabIndex = 196;
            this.txtAxis03SelectedPointNameChange.Tag = "";
            this.txtAxis03SelectedPointNameChange.Text = "等待位置";
            this.txtAxis03SelectedPointNameChange.DoubleClick += new System.EventHandler(this.TextBoxDoubleClickEvent);
            // 
            // lblAxis03PointNameTitle
            // 
            this.lblAxis03PointNameTitle.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAxis03PointNameTitle.ForeColor = System.Drawing.Color.Orange;
            this.lblAxis03PointNameTitle.Location = new System.Drawing.Point(20, 202);
            this.lblAxis03PointNameTitle.Name = "lblAxis03PointNameTitle";
            this.lblAxis03PointNameTitle.Size = new System.Drawing.Size(188, 39);
            this.lblAxis03PointNameTitle.TabIndex = 197;
            this.lblAxis03PointNameTitle.Tag = "";
            this.lblAxis03PointNameTitle.Text = "轴03点位名称:";
            this.lblAxis03PointNameTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtAxis04SelectedPointNameChange
            // 
            this.txtAxis04SelectedPointNameChange.BackColor = System.Drawing.Color.White;
            this.txtAxis04SelectedPointNameChange.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtAxis04SelectedPointNameChange.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAxis04SelectedPointNameChange.ForeColor = System.Drawing.Color.Orange;
            this.txtAxis04SelectedPointNameChange.Location = new System.Drawing.Point(218, 250);
            this.txtAxis04SelectedPointNameChange.Name = "txtAxis04SelectedPointNameChange";
            this.txtAxis04SelectedPointNameChange.Size = new System.Drawing.Size(340, 35);
            this.txtAxis04SelectedPointNameChange.TabIndex = 196;
            this.txtAxis04SelectedPointNameChange.Tag = "";
            this.txtAxis04SelectedPointNameChange.Text = "等待位置";
            this.txtAxis04SelectedPointNameChange.DoubleClick += new System.EventHandler(this.TextBoxDoubleClickEvent);
            // 
            // lblAxis04PointNameTitle
            // 
            this.lblAxis04PointNameTitle.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAxis04PointNameTitle.ForeColor = System.Drawing.Color.Orange;
            this.lblAxis04PointNameTitle.Location = new System.Drawing.Point(20, 250);
            this.lblAxis04PointNameTitle.Name = "lblAxis04PointNameTitle";
            this.lblAxis04PointNameTitle.Size = new System.Drawing.Size(188, 39);
            this.lblAxis04PointNameTitle.TabIndex = 197;
            this.lblAxis04PointNameTitle.Tag = "";
            this.lblAxis04PointNameTitle.Text = "轴04点位名称:";
            this.lblAxis04PointNameTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
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
            this.shapeContainer1.Size = new System.Drawing.Size(1546, 1017);
            this.shapeContainer1.TabIndex = 312;
            this.shapeContainer1.TabStop = false;
            // 
            // rectangleShape3
            // 
            this.rectangleShape3.BorderColor = System.Drawing.Color.Silver;
            this.rectangleShape3.Location = new System.Drawing.Point(5, 12);
            this.rectangleShape3.Name = "rectangleShape3";
            this.rectangleShape3.Size = new System.Drawing.Size(376, 246);
            // 
            // rectangleShape2
            // 
            this.rectangleShape2.BorderColor = System.Drawing.Color.Silver;
            this.rectangleShape2.Location = new System.Drawing.Point(5, 597);
            this.rectangleShape2.Name = "rectangleShape2";
            this.rectangleShape2.Size = new System.Drawing.Size(376, 71);
            // 
            // rectangleShape1
            // 
            this.rectangleShape1.BorderColor = System.Drawing.Color.Silver;
            this.rectangleShape1.Location = new System.Drawing.Point(5, 262);
            this.rectangleShape1.Name = "rectangleShape1";
            this.rectangleShape1.Size = new System.Drawing.Size(376, 332);
            // 
            // btnAxis03MinusMove
            // 
            this.btnAxis03MinusMove.ArrowColor = System.Drawing.Color.DarkOrange;
            this.btnAxis03MinusMove.BorderColor = null;
            this.btnAxis03MinusMove.Direction = UCArrow.ArrowDirection.Left;
            this.btnAxis03MinusMove.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAxis03MinusMove.ForeColor = System.Drawing.Color.White;
            this.btnAxis03MinusMove.Location = new System.Drawing.Point(33, 743);
            this.btnAxis03MinusMove.Name = "btnAxis03MinusMove";
            this.btnAxis03MinusMove.Size = new System.Drawing.Size(138, 129);
            this.btnAxis03MinusMove.TabIndex = 305;
            this.btnAxis03MinusMove.Text = "R1轴-";
            this.btnAxis03MinusMove.MouseDown += new System.Windows.Forms.MouseEventHandler(this.AxisJogInchMove);
            this.btnAxis03MinusMove.MouseLeave += new System.EventHandler(this.AxisJogInchMouseLeaveEvent);
            this.btnAxis03MinusMove.MouseUp += new System.Windows.Forms.MouseEventHandler(this.AxisJogInchMouseUpEvent);
            // 
            // AxisUnionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1546, 1017);
            this.Controls.Add(this.btnAxis04PlusMove);
            this.Controls.Add(this.btnAxis04MinusMove);
            this.Controls.Add(this.btnAxisUnionRun);
            this.Controls.Add(this.btnAxis03PlusMove);
            this.Controls.Add(this.btnAxis03MinusMove);
            this.Controls.Add(this.btnAxis02PlusMove);
            this.Controls.Add(this.btnAxis02MinusMove);
            this.Controls.Add(this.btnAxis01MinusMove);
            this.Controls.Add(this.btnAxis01PlusMove);
            this.Controls.Add(this.btnUnionPointNameChange);
            this.Controls.Add(this.btnSaveSpeedOrOthers);
            this.Controls.Add(this.btnSaveModifyPosition);
            this.Controls.Add(this.btnSaveCurrentPosition);
            this.Controls.Add(this.lblAxis04PointNameTitle);
            this.Controls.Add(this.lblAxis03PointNameTitle);
            this.Controls.Add(this.lblAxis02PointNameTitle);
            this.Controls.Add(this.lblAxis01PointNameTitle);
            this.Controls.Add(this.txtAxis04SelectedPointNameChange);
            this.Controls.Add(this.txtAxis03SelectedPointNameChange);
            this.Controls.Add(this.txtAxis02SelectedPointNameChange);
            this.Controls.Add(this.txtAxis01SelectedPointNameChange);
            this.Controls.Add(this.txtPointNumberSet);
            this.Controls.Add(this.lblSelectedPointNumberTitle);
            this.Controls.Add(this.lblAxisMotionCardStatus);
            this.Controls.Add(this.gbxAxis02);
            this.Controls.Add(this.gbxAxis03);
            this.Controls.Add(this.gbxAxis04);
            this.Controls.Add(this.gbxAxis01);
            this.Controls.Add(this.shapeContainer1);
            this.Name = "AxisUnionForm";
            this.Text = "MaintenanceMotorUnionForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MaintenanceMotorUnionForm_FormClosing);
            this.Load += new System.EventHandler(this.MaintenanceMotorUnionForm_Load);
            this.gbxAxis01.ResumeLayout(false);
            this.gbxAxis01.PerformLayout();
            this.gbxAxis02.ResumeLayout(false);
            this.gbxAxis02.PerformLayout();
            this.gbxAxis03.ResumeLayout(false);
            this.gbxAxis03.PerformLayout();
            this.gbxAxis04.ResumeLayout(false);
            this.gbxAxis04.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblAxisMotionCardStatus;
        private System.Windows.Forms.TextBox txtPointNumberSet;
        private System.Windows.Forms.Label lblSelectedPointNumberTitle;
        private System.Windows.Forms.TextBox txtAxis01CurrentPosition;
        private System.Windows.Forms.Label lblAxis01CurrentPosition;
        private System.Windows.Forms.TextBox txtAxis01SelectedPointNameChange;
        private System.Windows.Forms.Label lblAxis01PointNameTitle;
        private System.Windows.Forms.TextBox txtAxis01InchDistance;
        private System.Windows.Forms.TextBox txtAxis01JogInchVelocity;
        private System.Windows.Forms.Label lblAxis01InchDistance;
        private System.Windows.Forms.Label lblAxis01JogInchVelocity;
        private System.Windows.Forms.Button btnAxis01JogOrInchSelector;
        private System.Windows.Forms.TextBox txtAxis01ModifyAcc;
        private System.Windows.Forms.TextBox txtAxis01ModifyVelocity;
        private System.Windows.Forms.TextBox txtAxis01ModifyPosition;
        private System.Windows.Forms.TextBox txtAxis01SavedAcc;
        private System.Windows.Forms.TextBox txtAxis01SavedVelocity;
        private System.Windows.Forms.TextBox txtAxis01SavedPosition;
        private System.Windows.Forms.Label lblAxis01ModifydAcc;
        private System.Windows.Forms.Label lblAxis01ModifyVelocity;
        private System.Windows.Forms.Label lblAxis01ModifyPosition;
        private System.Windows.Forms.Label lblAxis01SavedAcc;
        private System.Windows.Forms.Label lblAxis01SavedVelocity;
        private System.Windows.Forms.Label lblAxis01SavedPosition;
        private System.Windows.Forms.Button btnSaveSpeedOrOthers;
        private System.Windows.Forms.Button btnSaveModifyPosition;
        private System.Windows.Forms.Button btnSaveCurrentPosition;
        private System.Windows.Forms.TextBox txtAxis02ModifyAcc;
        private System.Windows.Forms.TextBox txtAxis02ModifyVelocity;
        private System.Windows.Forms.TextBox txtAxis02ModifyPosition;
        private System.Windows.Forms.TextBox txtAxis02SavedAcc;
        private System.Windows.Forms.TextBox txtAxis02SavedVelocity;
        private System.Windows.Forms.TextBox txtAxis02SavedPosition;
        private System.Windows.Forms.Label lblAxis02ModifydAcc;
        private System.Windows.Forms.Label lblAxis02ModifyVelocity;
        private System.Windows.Forms.Label lblAxis02ModifyPosition;
        private System.Windows.Forms.Label lblAxis02SavedAcc;
        private System.Windows.Forms.Label lblAxis02SavedVelocity;
        private System.Windows.Forms.Label lblAxis02SavedPosition;
        private System.Windows.Forms.Button btnAxis02JogOrInchSelector;
        private System.Windows.Forms.TextBox txtAxis02InchDistance;
        private System.Windows.Forms.TextBox txtAxis02JogInchVelocity;
        private System.Windows.Forms.Label lblAxis02InchDistance;
        private System.Windows.Forms.Label lblAxis02JogInchVelocity;
        private System.Windows.Forms.TextBox txtAxis02CurrentPosition;
        private System.Windows.Forms.TextBox txtAxis03ModifyAcc;
        private System.Windows.Forms.TextBox txtAxis03ModifyVelocity;
        private System.Windows.Forms.TextBox txtAxis03ModifyPosition;
        private System.Windows.Forms.TextBox txtAxis03SavedAcc;
        private System.Windows.Forms.TextBox txtAxis03SavedVelocity;
        private System.Windows.Forms.TextBox txtAxis03SavedPosition;
        private System.Windows.Forms.Label lblAxis03ModifydAcc;
        private System.Windows.Forms.Label lblAxis03ModifyVelocity;
        private System.Windows.Forms.Label lblAxis03ModifyPosition;
        private System.Windows.Forms.Label lblAxis03SavedAcc;
        private System.Windows.Forms.Label lblAxis03SavedVelocity;
        private System.Windows.Forms.Label lblAxis03SavedPosition;
        private System.Windows.Forms.Button btnAxis03JogOrInchSelector;
        private System.Windows.Forms.TextBox txtAxis03InchDistance;
        private System.Windows.Forms.TextBox txtAxis03JogInchVelocity;
        private System.Windows.Forms.Label lblAxis03InchDistance;
        private System.Windows.Forms.Label lblAxis03JogInchVelocity;
        private System.Windows.Forms.TextBox txtAxis03CurrentPosition;
        private System.Windows.Forms.TextBox txtAxis04ModifyAcc;
        private System.Windows.Forms.TextBox txtAxis04ModifyVelocity;
        private System.Windows.Forms.TextBox txtAxis04ModifyPosition;
        private System.Windows.Forms.TextBox txtAxis04SavedAcc;
        private System.Windows.Forms.TextBox txtAxis04SavedVelocity;
        private System.Windows.Forms.TextBox txtAxis04SavedPosition;
        private System.Windows.Forms.Label lblAxis04ModifydAcc;
        private System.Windows.Forms.Label lblAxis04ModifyVelocity;
        private System.Windows.Forms.Label lblAxis04ModifyPosition;
        private System.Windows.Forms.Label lblAxis04SavedAcc;
        private System.Windows.Forms.Label lblAxis04SavedVelocity;
        private System.Windows.Forms.Label lblAxis04SavedPosition;
        private System.Windows.Forms.Button btnAxis04JogOrInchSelector;
        private System.Windows.Forms.TextBox txtAxis04InchDistance;
        private System.Windows.Forms.TextBox txtAxis04JogInchVelocity;
        private System.Windows.Forms.Label lblAxis04InchDistance;
        private System.Windows.Forms.Label lblAxis04JogInchVelocity;
        private System.Windows.Forms.TextBox txtAxis04CurrentPosition;
        private System.Windows.Forms.GroupBox gbxAxis01;
        private System.Windows.Forms.Label lblAxis02CurrentPosition;
        private System.Windows.Forms.Label lblAxis03CurrentPosition;
        private System.Windows.Forms.Label lblAxis04CurrentPosition;
        private System.Windows.Forms.GroupBox gbxAxis02;
        private System.Windows.Forms.GroupBox gbxAxis03;
        private System.Windows.Forms.GroupBox gbxAxis04;
        private System.Windows.Forms.Button btnAxis01UnionFlag;
        private System.Windows.Forms.Button btnAxis02UnionFlag;
        private System.Windows.Forms.Button btnAxis03UnionFlag;
        private System.Windows.Forms.Button btnAxis04UnionFlag;
        private System.Windows.Forms.Button btnUnionPointNameChange;
        private UCArrow.UCArrow btnAxis01PlusMove;
        private UCArrow.UCArrow btnAxis01MinusMove;
        private UCArrow.UCArrow btnAxis02MinusMove;
        private UCArrow.UCArrow btnAxis02PlusMove;
        private UCArrow.UCArrow btnAxis03PlusMove;
        private UCArrow.UCArrow btnAxis04PlusMove;
        private UCArrow.UCArrow btnAxis04MinusMove;
        private System.Windows.Forms.Button btnAxisUnionRun;
        private System.Windows.Forms.TextBox txtAxis02SelectedPointNameChange;
        private System.Windows.Forms.Label lblAxis02PointNameTitle;
        private System.Windows.Forms.TextBox txtAxis03SelectedPointNameChange;
        private System.Windows.Forms.Label lblAxis03PointNameTitle;
        private System.Windows.Forms.TextBox txtAxis04SelectedPointNameChange;
        private System.Windows.Forms.Label lblAxis04PointNameTitle;
        private Microsoft.VisualBasic.PowerPacks.ShapeContainer shapeContainer1;
        private Microsoft.VisualBasic.PowerPacks.RectangleShape rectangleShape3;
        private Microsoft.VisualBasic.PowerPacks.RectangleShape rectangleShape2;
        private Microsoft.VisualBasic.PowerPacks.RectangleShape rectangleShape1;
        private UCArrow.UCArrow btnAxis03MinusMove;
    }
}