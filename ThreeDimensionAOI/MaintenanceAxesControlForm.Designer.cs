using System.Threading;

namespace ThreeDimensionAVI
{
    partial class MaintenanceAxesControlForm
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
            this.lblDestinationNumberTitle = new System.Windows.Forms.Label();
            this.txtDestinationNumber = new System.Windows.Forms.TextBox();
            this.btnMoveToTrack1AssignedDestinationPointPosition = new System.Windows.Forms.Button();
            this.gbxMoveAxesToDestinationPoint = new System.Windows.Forms.GroupBox();
            this.btnMoveToTrack2AssignedDestinationPointPosition = new System.Windows.Forms.Button();
            this.shapeContainer1 = new Microsoft.VisualBasic.PowerPacks.ShapeContainer();
            this.rectangleShape1 = new Microsoft.VisualBasic.PowerPacks.RectangleShape();
            this.gbxConveyorWidthChange = new System.Windows.Forms.GroupBox();
            this.btnChangeConveyor2Width = new System.Windows.Forms.Button();
            this.btnChangeAllConveyorWidth = new System.Windows.Forms.Button();
            this.btnChangeConveyor1Width = new System.Windows.Forms.Button();
            this.gbxConveyorAction = new System.Windows.Forms.GroupBox();
            this.btnConveyor2Action = new System.Windows.Forms.Button();
            this.txtConveyor2ActionSpeed = new System.Windows.Forms.TextBox();
            this.btnConveyor1Action = new System.Windows.Forms.Button();
            this.lblConveyor2ActionSpeedTitle = new System.Windows.Forms.Label();
            this.lblConveyor1ActionSpeedTitle = new System.Windows.Forms.Label();
            this.txtConveyor1ActionSpeed = new System.Windows.Forms.TextBox();
            this.gbxMoveAxesToDestinationPoint.SuspendLayout();
            this.gbxConveyorWidthChange.SuspendLayout();
            this.gbxConveyorAction.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblDestinationNumberTitle
            // 
            this.lblDestinationNumberTitle.AutoSize = true;
            this.lblDestinationNumberTitle.Location = new System.Drawing.Point(81, 43);
            this.lblDestinationNumberTitle.Name = "lblDestinationNumberTitle";
            this.lblDestinationNumberTitle.Size = new System.Drawing.Size(96, 19);
            this.lblDestinationNumberTitle.TabIndex = 0;
            this.lblDestinationNumberTitle.Text = "目标点位号: ";
            // 
            // txtDestinationNumber
            // 
            this.txtDestinationNumber.Location = new System.Drawing.Point(178, 40);
            this.txtDestinationNumber.Name = "txtDestinationNumber";
            this.txtDestinationNumber.Size = new System.Drawing.Size(100, 26);
            this.txtDestinationNumber.TabIndex = 1;
            this.txtDestinationNumber.Text = "1";
            this.txtDestinationNumber.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtDestinationNumber.TextChanged += new System.EventHandler(this.TextBoxDataPlusFloatOrIntCheckEvent);
            this.txtDestinationNumber.DoubleClick += new System.EventHandler(this.TextBoxDoubleClickEventToOpenNumberKeyboardForPlusData);
            this.txtDestinationNumber.Leave += new System.EventHandler(this.TextBoxDataPlusFloatOrIntCheckLeaveEvent);
            // 
            // btnMoveToTrack1AssignedDestinationPointPosition
            // 
            this.btnMoveToTrack1AssignedDestinationPointPosition.BackColor = System.Drawing.Color.LimeGreen;
            this.btnMoveToTrack1AssignedDestinationPointPosition.Font = new System.Drawing.Font("SimSun", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnMoveToTrack1AssignedDestinationPointPosition.Location = new System.Drawing.Point(32, 84);
            this.btnMoveToTrack1AssignedDestinationPointPosition.Name = "btnMoveToTrack1AssignedDestinationPointPosition";
            this.btnMoveToTrack1AssignedDestinationPointPosition.Size = new System.Drawing.Size(154, 63);
            this.btnMoveToTrack1AssignedDestinationPointPosition.TabIndex = 5;
            this.btnMoveToTrack1AssignedDestinationPointPosition.Text = "移动至传送线1目标点位";
            this.btnMoveToTrack1AssignedDestinationPointPosition.UseVisualStyleBackColor = false;
            this.btnMoveToTrack1AssignedDestinationPointPosition.Click += new System.EventHandler(this.btnMoveToAssignedDestinationPointPosition_Click);
            // 
            // gbxMoveAxesToDestinationPoint
            // 
            this.gbxMoveAxesToDestinationPoint.Controls.Add(this.btnMoveToTrack2AssignedDestinationPointPosition);
            this.gbxMoveAxesToDestinationPoint.Controls.Add(this.btnMoveToTrack1AssignedDestinationPointPosition);
            this.gbxMoveAxesToDestinationPoint.Controls.Add(this.lblDestinationNumberTitle);
            this.gbxMoveAxesToDestinationPoint.Controls.Add(this.txtDestinationNumber);
            this.gbxMoveAxesToDestinationPoint.Location = new System.Drawing.Point(26, 21);
            this.gbxMoveAxesToDestinationPoint.Name = "gbxMoveAxesToDestinationPoint";
            this.gbxMoveAxesToDestinationPoint.Size = new System.Drawing.Size(404, 168);
            this.gbxMoveAxesToDestinationPoint.TabIndex = 6;
            this.gbxMoveAxesToDestinationPoint.TabStop = false;
            this.gbxMoveAxesToDestinationPoint.Text = "移动轴至目标点位号点位";
            // 
            // btnMoveToTrack2AssignedDestinationPointPosition
            // 
            this.btnMoveToTrack2AssignedDestinationPointPosition.BackColor = System.Drawing.Color.LimeGreen;
            this.btnMoveToTrack2AssignedDestinationPointPosition.Font = new System.Drawing.Font("SimSun", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnMoveToTrack2AssignedDestinationPointPosition.Location = new System.Drawing.Point(221, 84);
            this.btnMoveToTrack2AssignedDestinationPointPosition.Name = "btnMoveToTrack2AssignedDestinationPointPosition";
            this.btnMoveToTrack2AssignedDestinationPointPosition.Size = new System.Drawing.Size(154, 63);
            this.btnMoveToTrack2AssignedDestinationPointPosition.TabIndex = 5;
            this.btnMoveToTrack2AssignedDestinationPointPosition.Text = "移动至传送线2目标点位";
            this.btnMoveToTrack2AssignedDestinationPointPosition.UseVisualStyleBackColor = false;
            this.btnMoveToTrack2AssignedDestinationPointPosition.Click += new System.EventHandler(this.btnMoveToAssignedDestinationPointPosition_Click);
            // 
            // shapeContainer1
            // 
            this.shapeContainer1.Location = new System.Drawing.Point(0, 0);
            this.shapeContainer1.Margin = new System.Windows.Forms.Padding(0);
            this.shapeContainer1.Name = "shapeContainer1";
            this.shapeContainer1.Shapes.AddRange(new Microsoft.VisualBasic.PowerPacks.Shape[] {
            this.rectangleShape1});
            this.shapeContainer1.Size = new System.Drawing.Size(1317, 686);
            this.shapeContainer1.TabIndex = 7;
            this.shapeContainer1.TabStop = false;
            // 
            // rectangleShape1
            // 
            this.rectangleShape1.Location = new System.Drawing.Point(0, 0);
            this.rectangleShape1.Name = "rectangleShape1";
            this.rectangleShape1.Size = new System.Drawing.Size(1316, 685);
            // 
            // gbxConveyorWidthChange
            // 
            this.gbxConveyorWidthChange.Controls.Add(this.btnChangeConveyor2Width);
            this.gbxConveyorWidthChange.Controls.Add(this.btnChangeAllConveyorWidth);
            this.gbxConveyorWidthChange.Controls.Add(this.btnChangeConveyor1Width);
            this.gbxConveyorWidthChange.Location = new System.Drawing.Point(26, 211);
            this.gbxConveyorWidthChange.Name = "gbxConveyorWidthChange";
            this.gbxConveyorWidthChange.Size = new System.Drawing.Size(404, 182);
            this.gbxConveyorWidthChange.TabIndex = 6;
            this.gbxConveyorWidthChange.TabStop = false;
            this.gbxConveyorWidthChange.Text = "传送线宽度调节";
            // 
            // btnChangeConveyor2Width
            // 
            this.btnChangeConveyor2Width.BackColor = System.Drawing.Color.LimeGreen;
            this.btnChangeConveyor2Width.Font = new System.Drawing.Font("SimSun", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnChangeConveyor2Width.Location = new System.Drawing.Point(221, 35);
            this.btnChangeConveyor2Width.Name = "btnChangeConveyor2Width";
            this.btnChangeConveyor2Width.Size = new System.Drawing.Size(154, 63);
            this.btnChangeConveyor2Width.TabIndex = 5;
            this.btnChangeConveyor2Width.Text = "调节传送线2宽度";
            this.btnChangeConveyor2Width.UseVisualStyleBackColor = false;
            this.btnChangeConveyor2Width.Click += new System.EventHandler(this.ManualChangeConveyorWidthEvent);
            // 
            // btnChangeAllConveyorWidth
            // 
            this.btnChangeAllConveyorWidth.BackColor = System.Drawing.Color.LimeGreen;
            this.btnChangeAllConveyorWidth.Font = new System.Drawing.Font("SimSun", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnChangeAllConveyorWidth.Location = new System.Drawing.Point(118, 113);
            this.btnChangeAllConveyorWidth.Name = "btnChangeAllConveyorWidth";
            this.btnChangeAllConveyorWidth.Size = new System.Drawing.Size(171, 63);
            this.btnChangeAllConveyorWidth.TabIndex = 5;
            this.btnChangeAllConveyorWidth.Text = "同时调节两条传送线宽度";
            this.btnChangeAllConveyorWidth.UseVisualStyleBackColor = false;
            this.btnChangeAllConveyorWidth.Click += new System.EventHandler(this.ManualChangeConveyorWidthEvent);
            // 
            // btnChangeConveyor1Width
            // 
            this.btnChangeConveyor1Width.BackColor = System.Drawing.Color.LimeGreen;
            this.btnChangeConveyor1Width.Font = new System.Drawing.Font("SimSun", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnChangeConveyor1Width.Location = new System.Drawing.Point(32, 35);
            this.btnChangeConveyor1Width.Name = "btnChangeConveyor1Width";
            this.btnChangeConveyor1Width.Size = new System.Drawing.Size(154, 63);
            this.btnChangeConveyor1Width.TabIndex = 5;
            this.btnChangeConveyor1Width.Text = "调节传送线1宽度";
            this.btnChangeConveyor1Width.UseVisualStyleBackColor = false;
            this.btnChangeConveyor1Width.Click += new System.EventHandler(this.ManualChangeConveyorWidthEvent);
            // 
            // gbxConveyorAction
            // 
            this.gbxConveyorAction.Controls.Add(this.btnConveyor2Action);
            this.gbxConveyorAction.Controls.Add(this.txtConveyor2ActionSpeed);
            this.gbxConveyorAction.Controls.Add(this.btnConveyor1Action);
            this.gbxConveyorAction.Controls.Add(this.lblConveyor2ActionSpeedTitle);
            this.gbxConveyorAction.Controls.Add(this.lblConveyor1ActionSpeedTitle);
            this.gbxConveyorAction.Controls.Add(this.txtConveyor1ActionSpeed);
            this.gbxConveyorAction.Location = new System.Drawing.Point(455, 21);
            this.gbxConveyorAction.Name = "gbxConveyorAction";
            this.gbxConveyorAction.Size = new System.Drawing.Size(351, 263);
            this.gbxConveyorAction.TabIndex = 6;
            this.gbxConveyorAction.TabStop = false;
            this.gbxConveyorAction.Text = "传送线动作";
            // 
            // btnConveyor2Action
            // 
            this.btnConveyor2Action.BackColor = System.Drawing.Color.LimeGreen;
            this.btnConveyor2Action.Font = new System.Drawing.Font("SimSun", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnConveyor2Action.Location = new System.Drawing.Point(100, 66);
            this.btnConveyor2Action.Name = "btnConveyor2Action";
            this.btnConveyor2Action.Size = new System.Drawing.Size(171, 63);
            this.btnConveyor2Action.TabIndex = 5;
            this.btnConveyor2Action.Text = "传送线2启动";
            this.btnConveyor2Action.UseVisualStyleBackColor = false;
            this.btnConveyor2Action.Click += new System.EventHandler(this.ManualControlConveyorEvent);
            // 
            // txtConveyor2ActionSpeed
            // 
            this.txtConveyor2ActionSpeed.Location = new System.Drawing.Point(211, 34);
            this.txtConveyor2ActionSpeed.Name = "txtConveyor2ActionSpeed";
            this.txtConveyor2ActionSpeed.Size = new System.Drawing.Size(100, 26);
            this.txtConveyor2ActionSpeed.TabIndex = 1;
            this.txtConveyor2ActionSpeed.Text = "1";
            this.txtConveyor2ActionSpeed.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtConveyor2ActionSpeed.TextChanged += new System.EventHandler(this.TextBoxDataPlusFloatOrIntCheckEvent);
            this.txtConveyor2ActionSpeed.DoubleClick += new System.EventHandler(this.TextBoxDoubleClickEventToOpenNumberKeyboardForPlusData);
            this.txtConveyor2ActionSpeed.Leave += new System.EventHandler(this.TextBoxDataPlusFloatOrIntCheckLeaveEvent);
            // 
            // btnConveyor1Action
            // 
            this.btnConveyor1Action.BackColor = System.Drawing.Color.LimeGreen;
            this.btnConveyor1Action.Font = new System.Drawing.Font("SimSun", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnConveyor1Action.Location = new System.Drawing.Point(100, 190);
            this.btnConveyor1Action.Name = "btnConveyor1Action";
            this.btnConveyor1Action.Size = new System.Drawing.Size(171, 63);
            this.btnConveyor1Action.TabIndex = 5;
            this.btnConveyor1Action.Text = "传送线1启动";
            this.btnConveyor1Action.UseVisualStyleBackColor = false;
            this.btnConveyor1Action.Click += new System.EventHandler(this.ManualControlConveyorEvent);
            // 
            // lblConveyor2ActionSpeedTitle
            // 
            this.lblConveyor2ActionSpeedTitle.AutoSize = true;
            this.lblConveyor2ActionSpeedTitle.Location = new System.Drawing.Point(35, 37);
            this.lblConveyor2ActionSpeedTitle.Name = "lblConveyor2ActionSpeedTitle";
            this.lblConveyor2ActionSpeedTitle.Size = new System.Drawing.Size(174, 19);
            this.lblConveyor2ActionSpeedTitle.TabIndex = 0;
            this.lblConveyor2ActionSpeedTitle.Text = "传送线2动作速度(mm/s):";
            // 
            // lblConveyor1ActionSpeedTitle
            // 
            this.lblConveyor1ActionSpeedTitle.AutoSize = true;
            this.lblConveyor1ActionSpeedTitle.Location = new System.Drawing.Point(35, 161);
            this.lblConveyor1ActionSpeedTitle.Name = "lblConveyor1ActionSpeedTitle";
            this.lblConveyor1ActionSpeedTitle.Size = new System.Drawing.Size(174, 19);
            this.lblConveyor1ActionSpeedTitle.TabIndex = 0;
            this.lblConveyor1ActionSpeedTitle.Text = "传送线1动作速度(mm/s):";
            // 
            // txtConveyor1ActionSpeed
            // 
            this.txtConveyor1ActionSpeed.Location = new System.Drawing.Point(211, 158);
            this.txtConveyor1ActionSpeed.Name = "txtConveyor1ActionSpeed";
            this.txtConveyor1ActionSpeed.Size = new System.Drawing.Size(100, 26);
            this.txtConveyor1ActionSpeed.TabIndex = 1;
            this.txtConveyor1ActionSpeed.Text = "1";
            this.txtConveyor1ActionSpeed.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtConveyor1ActionSpeed.TextChanged += new System.EventHandler(this.TextBoxDataPlusFloatOrIntCheckEvent);
            this.txtConveyor1ActionSpeed.DoubleClick += new System.EventHandler(this.TextBoxDoubleClickEventToOpenNumberKeyboardForPlusData);
            this.txtConveyor1ActionSpeed.Leave += new System.EventHandler(this.TextBoxDataPlusFloatOrIntCheckLeaveEvent);
            // 
            // MaintenanceAxesControlForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1317, 686);
            this.ControlBox = false;
            this.Controls.Add(this.gbxConveyorAction);
            this.Controls.Add(this.gbxConveyorWidthChange);
            this.Controls.Add(this.gbxMoveAxesToDestinationPoint);
            this.Controls.Add(this.shapeContainer1);
            this.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "MaintenanceAxesControlForm";
            this.Text = "MaintenanceAxesControlForm";
            this.Load += new System.EventHandler(this.MaintenanceAxesControlForm_Load);
            this.VisibleChanged += new System.EventHandler(this.MaintenanceAxesControlForm_VisibleChanged);
            this.gbxMoveAxesToDestinationPoint.ResumeLayout(false);
            this.gbxMoveAxesToDestinationPoint.PerformLayout();
            this.gbxConveyorWidthChange.ResumeLayout(false);
            this.gbxConveyorAction.ResumeLayout(false);
            this.gbxConveyorAction.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblDestinationNumberTitle;
        private System.Windows.Forms.TextBox txtDestinationNumber;
        private System.Windows.Forms.Button btnMoveToTrack1AssignedDestinationPointPosition;
        private System.Windows.Forms.GroupBox gbxMoveAxesToDestinationPoint;
        private Microsoft.VisualBasic.PowerPacks.ShapeContainer shapeContainer1;
        private Microsoft.VisualBasic.PowerPacks.RectangleShape rectangleShape1;
        private System.Windows.Forms.Button btnMoveToTrack2AssignedDestinationPointPosition;
        private System.Windows.Forms.GroupBox gbxConveyorWidthChange;
        private System.Windows.Forms.Button btnChangeConveyor2Width;
        private System.Windows.Forms.Button btnChangeAllConveyorWidth;
        private System.Windows.Forms.Button btnChangeConveyor1Width;
        private System.Windows.Forms.GroupBox gbxConveyorAction;
        private System.Windows.Forms.Button btnConveyor2Action;
        private System.Windows.Forms.Button btnConveyor1Action;
        private System.Windows.Forms.Label lblConveyor2ActionSpeedTitle;
        private System.Windows.Forms.Label lblConveyor1ActionSpeedTitle;
        private System.Windows.Forms.TextBox txtConveyor2ActionSpeed;
        private System.Windows.Forms.TextBox txtConveyor1ActionSpeed;
    }
}