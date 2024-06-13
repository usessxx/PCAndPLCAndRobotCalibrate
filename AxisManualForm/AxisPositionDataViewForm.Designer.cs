namespace AxisAndIOForm
{
    partial class AxisPositionDataViewForm
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
            //如果轴参数显示画面变量不为空，并且处于打开状态的话，那么dispose
            if (_axisParameterDataViewFormVariate != null && _axisParameterDataViewFormVariate.Visible)
            {
                _axisParameterDataViewFormVariate.Dispose();
                _axisParameterDataViewFormVariate = null;
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
            this.dgrdvAxisPositionDataShow = new System.Windows.Forms.DataGridView();
            this.lblAxisPositionDataNotification = new System.Windows.Forms.Label();
            this.btnPositionDataSave = new System.Windows.Forms.Button();
            this.btnAxisParameterDisplay = new System.Windows.Forms.Button();
            this.btnOutport = new System.Windows.Forms.Button();
            this.btnImport = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgrdvAxisPositionDataShow)).BeginInit();
            this.SuspendLayout();
            // 
            // dgrdvAxisPositionDataShow
            // 
            this.dgrdvAxisPositionDataShow.AllowUserToAddRows = false;
            this.dgrdvAxisPositionDataShow.AllowUserToDeleteRows = false;
            this.dgrdvAxisPositionDataShow.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgrdvAxisPositionDataShow.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dgrdvAxisPositionDataShow.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgrdvAxisPositionDataShow.Location = new System.Drawing.Point(13, 42);
            this.dgrdvAxisPositionDataShow.Name = "dgrdvAxisPositionDataShow";
            this.dgrdvAxisPositionDataShow.RowTemplate.Height = 30;
            this.dgrdvAxisPositionDataShow.Size = new System.Drawing.Size(889, 416);
            this.dgrdvAxisPositionDataShow.TabIndex = 0;
            // 
            // lblAxisPositionDataNotification
            // 
            this.lblAxisPositionDataNotification.AutoSize = true;
            this.lblAxisPositionDataNotification.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAxisPositionDataNotification.ForeColor = System.Drawing.Color.Black;
            this.lblAxisPositionDataNotification.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblAxisPositionDataNotification.Location = new System.Drawing.Point(8, 10);
            this.lblAxisPositionDataNotification.Name = "lblAxisPositionDataNotification";
            this.lblAxisPositionDataNotification.Size = new System.Drawing.Size(102, 19);
            this.lblAxisPositionDataNotification.TabIndex = 95;
            this.lblAxisPositionDataNotification.Tag = "";
            this.lblAxisPositionDataNotification.Text = "轴1点位数据";
            this.lblAxisPositionDataNotification.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnPositionDataSave
            // 
            this.btnPositionDataSave.BackColor = System.Drawing.Color.LightSeaGreen;
            this.btnPositionDataSave.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPositionDataSave.Location = new System.Drawing.Point(794, 464);
            this.btnPositionDataSave.Name = "btnPositionDataSave";
            this.btnPositionDataSave.Size = new System.Drawing.Size(108, 68);
            this.btnPositionDataSave.TabIndex = 122;
            this.btnPositionDataSave.Tag = "";
            this.btnPositionDataSave.Text = "保存";
            this.btnPositionDataSave.UseVisualStyleBackColor = false;
            this.btnPositionDataSave.Click += new System.EventHandler(this.PositionDataSaveEvent);
            // 
            // btnAxisParameterDisplay
            // 
            this.btnAxisParameterDisplay.BackColor = System.Drawing.Color.Gold;
            this.btnAxisParameterDisplay.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAxisParameterDisplay.Location = new System.Drawing.Point(13, 464);
            this.btnAxisParameterDisplay.Name = "btnAxisParameterDisplay";
            this.btnAxisParameterDisplay.Size = new System.Drawing.Size(108, 68);
            this.btnAxisParameterDisplay.TabIndex = 123;
            this.btnAxisParameterDisplay.Tag = "";
            this.btnAxisParameterDisplay.Text = "轴参数预览";
            this.btnAxisParameterDisplay.UseVisualStyleBackColor = false;
            this.btnAxisParameterDisplay.Click += new System.EventHandler(this.AxisParameterDisplay);
            // 
            // btnOutport
            // 
            this.btnOutport.BackColor = System.Drawing.Color.CornflowerBlue;
            this.btnOutport.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOutport.Location = new System.Drawing.Point(674, 464);
            this.btnOutport.Name = "btnOutport";
            this.btnOutport.Size = new System.Drawing.Size(108, 68);
            this.btnOutport.TabIndex = 126;
            this.btnOutport.Tag = "";
            this.btnOutport.Text = "导出";
            this.btnOutport.UseVisualStyleBackColor = false;
            this.btnOutport.Click += new System.EventHandler(this.btnOutport_Click);
            // 
            // btnImport
            // 
            this.btnImport.BackColor = System.Drawing.Color.CornflowerBlue;
            this.btnImport.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnImport.Location = new System.Drawing.Point(545, 464);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(108, 68);
            this.btnImport.TabIndex = 127;
            this.btnImport.Tag = "";
            this.btnImport.Text = "导入";
            this.btnImport.UseVisualStyleBackColor = false;
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // AxisPositionDataViewForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
            this.ClientSize = new System.Drawing.Size(914, 537);
            this.Controls.Add(this.btnOutport);
            this.Controls.Add(this.btnImport);
            this.Controls.Add(this.btnAxisParameterDisplay);
            this.Controls.Add(this.btnPositionDataSave);
            this.Controls.Add(this.lblAxisPositionDataNotification);
            this.Controls.Add(this.dgrdvAxisPositionDataShow);
            this.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "AxisPositionDataViewForm";
            this.Text = "轴点位数据";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AxisPositionDataViewForm_FormClosing);
            this.Load += new System.EventHandler(this.AxisPositionDataViewForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgrdvAxisPositionDataShow)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgrdvAxisPositionDataShow;
        private System.Windows.Forms.Label lblAxisPositionDataNotification;
        private System.Windows.Forms.Button btnPositionDataSave;
        private System.Windows.Forms.Button btnAxisParameterDisplay;
        private System.Windows.Forms.Button btnOutport;
        private System.Windows.Forms.Button btnImport;
    }
}