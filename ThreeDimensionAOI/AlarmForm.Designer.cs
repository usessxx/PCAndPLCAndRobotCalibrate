using System.Threading;

namespace ThreeDimensionAVI
{
    partial class AlarmForm
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
            if (_alarmInfoRefreshTimer != null)
            {
                _alarmInfoRefreshTimer.Dispose();
                _alarmInfoRefreshTimer = null;
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgrdvAlarmInfo = new System.Windows.Forms.DataGridView();
            this.Number = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Contents = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Time = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnAlarmReset = new System.Windows.Forms.Button();
            this.btnAlarmAndAlarmRecordExchange = new System.Windows.Forms.Button();
            this.btnLoadAlarmRecord = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgrdvAlarmInfo)).BeginInit();
            this.SuspendLayout();
            // 
            // dgrdvAlarmInfo
            // 
            this.dgrdvAlarmInfo.AllowUserToAddRows = false;
            this.dgrdvAlarmInfo.AllowUserToDeleteRows = false;
            this.dgrdvAlarmInfo.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.Sunken;
            this.dgrdvAlarmInfo.ColumnHeadersHeight = 30;
            this.dgrdvAlarmInfo.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Number,
            this.Contents,
            this.Time});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Menu;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgrdvAlarmInfo.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgrdvAlarmInfo.Location = new System.Drawing.Point(13, 14);
            this.dgrdvAlarmInfo.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.dgrdvAlarmInfo.Name = "dgrdvAlarmInfo";
            this.dgrdvAlarmInfo.ReadOnly = true;
            this.dgrdvAlarmInfo.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.dgrdvAlarmInfo.RowHeadersVisible = false;
            this.dgrdvAlarmInfo.RowTemplate.Height = 23;
            this.dgrdvAlarmInfo.ShowCellErrors = false;
            this.dgrdvAlarmInfo.Size = new System.Drawing.Size(801, 365);
            this.dgrdvAlarmInfo.TabIndex = 0;
            this.dgrdvAlarmInfo.Tag = "0";
            this.dgrdvAlarmInfo.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.Alarm_Disp_DataGridView_CellFormatting);
            // 
            // Number
            // 
            this.Number.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Number.FillWeight = 20F;
            this.Number.HeaderText = "序号";
            this.Number.Name = "Number";
            this.Number.ReadOnly = true;
            this.Number.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // Contents
            // 
            this.Contents.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Contents.HeaderText = "报警内容";
            this.Contents.Name = "Contents";
            this.Contents.ReadOnly = true;
            // 
            // Time
            // 
            this.Time.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Time.FillWeight = 40F;
            this.Time.HeaderText = "触发时间";
            this.Time.Name = "Time";
            this.Time.ReadOnly = true;
            // 
            // btnAlarmReset
            // 
            this.btnAlarmReset.Font = new System.Drawing.Font("Times New Roman", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAlarmReset.Location = new System.Drawing.Point(13, 389);
            this.btnAlarmReset.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnAlarmReset.Name = "btnAlarmReset";
            this.btnAlarmReset.Size = new System.Drawing.Size(107, 45);
            this.btnAlarmReset.TabIndex = 1;
            this.btnAlarmReset.Tag = "1";
            this.btnAlarmReset.Text = "复位";
            this.btnAlarmReset.UseVisualStyleBackColor = true;
            this.btnAlarmReset.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PressVirtualBTEvent);
            this.btnAlarmReset.MouseUp += new System.Windows.Forms.MouseEventHandler(this.LeaveVirtualBTEvet);
            // 
            // btnAlarmAndAlarmRecordExchange
            // 
            this.btnAlarmAndAlarmRecordExchange.Font = new System.Drawing.Font("Times New Roman", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAlarmAndAlarmRecordExchange.Location = new System.Drawing.Point(703, 389);
            this.btnAlarmAndAlarmRecordExchange.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnAlarmAndAlarmRecordExchange.Name = "btnAlarmAndAlarmRecordExchange";
            this.btnAlarmAndAlarmRecordExchange.Size = new System.Drawing.Size(112, 45);
            this.btnAlarmAndAlarmRecordExchange.TabIndex = 3;
            this.btnAlarmAndAlarmRecordExchange.Tag = "3";
            this.btnAlarmAndAlarmRecordExchange.Text = "报警记录";
            this.btnAlarmAndAlarmRecordExchange.UseVisualStyleBackColor = true;
            this.btnAlarmAndAlarmRecordExchange.Click += new System.EventHandler(this.btnAlarmAndAlarmRecordExchange_Click);
            // 
            // btnLoadAlarmRecord
            // 
            this.btnLoadAlarmRecord.Font = new System.Drawing.Font("Times New Roman", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLoadAlarmRecord.Location = new System.Drawing.Point(540, 389);
            this.btnLoadAlarmRecord.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnLoadAlarmRecord.Name = "btnLoadAlarmRecord";
            this.btnLoadAlarmRecord.Size = new System.Drawing.Size(155, 45);
            this.btnLoadAlarmRecord.TabIndex = 2;
            this.btnLoadAlarmRecord.Tag = "2";
            this.btnLoadAlarmRecord.Text = "读取报警记录";
            this.btnLoadAlarmRecord.UseVisualStyleBackColor = true;
            this.btnLoadAlarmRecord.Click += new System.EventHandler(this.btnLoadAlarmRecord_Click);
            // 
            // AlarmForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(827, 445);
            this.Controls.Add(this.btnAlarmAndAlarmRecordExchange);
            this.Controls.Add(this.btnLoadAlarmRecord);
            this.Controls.Add(this.btnAlarmReset);
            this.Controls.Add(this.dgrdvAlarmInfo);
            this.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AlarmForm";
            this.Text = "报警";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AlarmForm_FormClosing);
            this.Load += new System.EventHandler(this.AlarmForm_Load);
            this.VisibleChanged += new System.EventHandler(this.AlarmForm_VisibleChanged);
            ((System.ComponentModel.ISupportInitialize)(this.dgrdvAlarmInfo)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgrdvAlarmInfo;
        private System.Windows.Forms.Button btnAlarmReset;
        private System.Windows.Forms.Button btnAlarmAndAlarmRecordExchange;
        private System.Windows.Forms.Button btnLoadAlarmRecord;
        private System.Windows.Forms.DataGridViewTextBoxColumn Number;
        private System.Windows.Forms.DataGridViewTextBoxColumn Contents;
        private System.Windows.Forms.DataGridViewTextBoxColumn Time;

    }
}