namespace AxisAndIOForm
{
    partial class AxisParameterDataViewForm
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
            this.btnParameterSave = new System.Windows.Forms.Button();
            this.lblTitle = new System.Windows.Forms.Label();
            this.dgrdvAxisParameterShow = new System.Windows.Forms.DataGridView();
            this.btnImport = new System.Windows.Forms.Button();
            this.btnOutport = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgrdvAxisParameterShow)).BeginInit();
            this.SuspendLayout();
            // 
            // btnParameterSave
            // 
            this.btnParameterSave.BackColor = System.Drawing.Color.LightSeaGreen;
            this.btnParameterSave.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnParameterSave.Location = new System.Drawing.Point(924, 281);
            this.btnParameterSave.Name = "btnParameterSave";
            this.btnParameterSave.Size = new System.Drawing.Size(108, 68);
            this.btnParameterSave.TabIndex = 125;
            this.btnParameterSave.Tag = "";
            this.btnParameterSave.Text = "保存";
            this.btnParameterSave.UseVisualStyleBackColor = false;
            this.btnParameterSave.Click += new System.EventHandler(this.AxisParameterDataSaveEvent);
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.ForeColor = System.Drawing.Color.Black;
            this.lblTitle.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblTitle.Location = new System.Drawing.Point(8, 10);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(94, 19);
            this.lblTitle.TabIndex = 124;
            this.lblTitle.Tag = "";
            this.lblTitle.Text = "轴参数数据";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // dgrdvAxisParameterShow
            // 
            this.dgrdvAxisParameterShow.AllowUserToAddRows = false;
            this.dgrdvAxisParameterShow.AllowUserToDeleteRows = false;
            this.dgrdvAxisParameterShow.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgrdvAxisParameterShow.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dgrdvAxisParameterShow.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgrdvAxisParameterShow.Location = new System.Drawing.Point(13, 42);
            this.dgrdvAxisParameterShow.Name = "dgrdvAxisParameterShow";
            this.dgrdvAxisParameterShow.RowTemplate.Height = 30;
            this.dgrdvAxisParameterShow.Size = new System.Drawing.Size(1019, 233);
            this.dgrdvAxisParameterShow.TabIndex = 123;
            // 
            // btnImport
            // 
            this.btnImport.BackColor = System.Drawing.Color.CornflowerBlue;
            this.btnImport.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnImport.Location = new System.Drawing.Point(12, 281);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(108, 68);
            this.btnImport.TabIndex = 125;
            this.btnImport.Tag = "";
            this.btnImport.Text = "导入";
            this.btnImport.UseVisualStyleBackColor = false;
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // btnOutport
            // 
            this.btnOutport.BackColor = System.Drawing.Color.CornflowerBlue;
            this.btnOutport.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOutport.Location = new System.Drawing.Point(141, 281);
            this.btnOutport.Name = "btnOutport";
            this.btnOutport.Size = new System.Drawing.Size(108, 68);
            this.btnOutport.TabIndex = 125;
            this.btnOutport.Tag = "";
            this.btnOutport.Text = "导出";
            this.btnOutport.UseVisualStyleBackColor = false;
            this.btnOutport.Click += new System.EventHandler(this.btnOutport_Click);
            // 
            // AxisParameterDataViewForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1044, 356);
            this.Controls.Add(this.btnOutport);
            this.Controls.Add(this.btnImport);
            this.Controls.Add(this.btnParameterSave);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.dgrdvAxisParameterShow);
            this.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "AxisParameterDataViewForm";
            this.Text = "轴参数数据";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AxisParameterDataViewForm_FormClosing);
            this.Load += new System.EventHandler(this.AxisParameterDataViewForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgrdvAxisParameterShow)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnParameterSave;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.DataGridView dgrdvAxisParameterShow;
        private System.Windows.Forms.Button btnImport;
        private System.Windows.Forms.Button btnOutport;


    }
}