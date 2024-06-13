namespace MatchModel.Halcon
{
    partial class ShapeModelFrm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ShapeModelFrm));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.BnTestModelImage = new System.Windows.Forms.Button();
            this.BnInputTestImage = new System.Windows.Forms.Button();
            this.listBox_FindModelROI = new System.Windows.Forms.ListBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.删除ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.BnFindModel = new System.Windows.Forms.Button();
            this.BnAddSearchROI = new System.Windows.Forms.Button();
            this.BnCreateModel = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.toolStrip3 = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.ROI_Circle = new System.Windows.Forms.ToolStripButton();
            this.ROI_Rect2 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton_ShowCreateModelRoi = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton_ModelMask = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton_ClearModel = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton_InputImage = new System.Windows.Forms.ToolStripButton();
            this.BnSaveModel = new System.Windows.Forms.Button();
            this.checkBox_EnableNcc = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.toolStrip3.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.checkBox_EnableNcc);
            this.splitContainer1.Panel2.Controls.Add(this.groupBox2);
            this.splitContainer1.Panel2.Controls.Add(this.BnCreateModel);
            this.splitContainer1.Panel2.Controls.Add(this.groupBox1);
            this.splitContainer1.Panel2.Controls.Add(this.toolStrip3);
            this.splitContainer1.Panel2.Controls.Add(this.BnSaveModel);
            this.splitContainer1.Size = new System.Drawing.Size(1094, 621);
            this.splitContainer1.SplitterDistance = 542;
            this.splitContainer1.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.BnTestModelImage);
            this.groupBox2.Controls.Add(this.BnInputTestImage);
            this.groupBox2.Controls.Add(this.listBox_FindModelROI);
            this.groupBox2.Controls.Add(this.BnFindModel);
            this.groupBox2.Controls.Add(this.BnAddSearchROI);
            this.groupBox2.Location = new System.Drawing.Point(14, 367);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(281, 225);
            this.groupBox2.TabIndex = 26;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "搜索区域";
            // 
            // BnTestModelImage
            // 
            this.BnTestModelImage.Font = new System.Drawing.Font("微软雅黑", 7F);
            this.BnTestModelImage.Location = new System.Drawing.Point(172, 182);
            this.BnTestModelImage.Name = "BnTestModelImage";
            this.BnTestModelImage.Size = new System.Drawing.Size(91, 33);
            this.BnTestModelImage.TabIndex = 28;
            this.BnTestModelImage.Text = "测试模板图像";
            this.BnTestModelImage.UseVisualStyleBackColor = true;
            this.BnTestModelImage.Click += new System.EventHandler(this.BnTestModelImage_Click);
            // 
            // BnInputTestImage
            // 
            this.BnInputTestImage.Font = new System.Drawing.Font("微软雅黑", 7F);
            this.BnInputTestImage.Location = new System.Drawing.Point(172, 134);
            this.BnInputTestImage.Name = "BnInputTestImage";
            this.BnInputTestImage.Size = new System.Drawing.Size(91, 33);
            this.BnInputTestImage.TabIndex = 27;
            this.BnInputTestImage.Text = "导入测试图像";
            this.BnInputTestImage.UseVisualStyleBackColor = true;
            this.BnInputTestImage.Click += new System.EventHandler(this.BnInputTestImage_Click);
            // 
            // listBox_FindModelROI
            // 
            this.listBox_FindModelROI.ContextMenuStrip = this.contextMenuStrip1;
            this.listBox_FindModelROI.FormattingEnabled = true;
            this.listBox_FindModelROI.ItemHeight = 20;
            this.listBox_FindModelROI.Location = new System.Drawing.Point(23, 30);
            this.listBox_FindModelROI.Name = "listBox_FindModelROI";
            this.listBox_FindModelROI.Size = new System.Drawing.Size(120, 164);
            this.listBox_FindModelROI.TabIndex = 0;
            this.listBox_FindModelROI.SelectedIndexChanged += new System.EventHandler(this.listBox_FindModelROI_SelectedIndexChanged);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.删除ToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(109, 28);
            // 
            // 删除ToolStripMenuItem
            // 
            this.删除ToolStripMenuItem.Name = "删除ToolStripMenuItem";
            this.删除ToolStripMenuItem.Size = new System.Drawing.Size(108, 24);
            this.删除ToolStripMenuItem.Text = "删除";
            this.删除ToolStripMenuItem.Click += new System.EventHandler(this.删除ToolStripMenuItem_Click);
            // 
            // BnFindModel
            // 
            this.BnFindModel.Location = new System.Drawing.Point(172, 82);
            this.BnFindModel.Name = "BnFindModel";
            this.BnFindModel.Size = new System.Drawing.Size(91, 33);
            this.BnFindModel.TabIndex = 22;
            this.BnFindModel.Text = "查找模板";
            this.BnFindModel.UseVisualStyleBackColor = true;
            this.BnFindModel.Click += new System.EventHandler(this.BnFindModel_Click);
            // 
            // BnAddSearchROI
            // 
            this.BnAddSearchROI.Location = new System.Drawing.Point(172, 30);
            this.BnAddSearchROI.Name = "BnAddSearchROI";
            this.BnAddSearchROI.Size = new System.Drawing.Size(91, 33);
            this.BnAddSearchROI.TabIndex = 24;
            this.BnAddSearchROI.Text = "添加";
            this.BnAddSearchROI.UseVisualStyleBackColor = true;
            this.BnAddSearchROI.Click += new System.EventHandler(this.BnAddSearchROI_Click);
            // 
            // BnCreateModel
            // 
            this.BnCreateModel.Location = new System.Drawing.Point(390, 449);
            this.BnCreateModel.Name = "BnCreateModel";
            this.BnCreateModel.Size = new System.Drawing.Size(91, 42);
            this.BnCreateModel.TabIndex = 21;
            this.BnCreateModel.Text = "创建模板";
            this.BnCreateModel.UseVisualStyleBackColor = true;
            this.BnCreateModel.Click += new System.EventHandler(this.BnCreateModel_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.propertyGrid1);
            this.groupBox1.Location = new System.Drawing.Point(14, 61);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(522, 290);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "模板参数";
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.Location = new System.Drawing.Point(12, 26);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.Size = new System.Drawing.Size(494, 249);
            this.propertyGrid1.TabIndex = 26;
            this.propertyGrid1.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGrid1_PropertyValueChanged);
            // 
            // toolStrip3
            // 
            this.toolStrip3.AutoSize = false;
            this.toolStrip3.BackColor = System.Drawing.SystemColors.ControlDark;
            this.toolStrip3.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip3.ImageScalingSize = new System.Drawing.Size(26, 26);
            this.toolStrip3.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.toolStripSeparator1,
            this.ROI_Circle,
            this.ROI_Rect2,
            this.toolStripButton_ShowCreateModelRoi,
            this.toolStripButton_ModelMask,
            this.toolStripSeparator2,
            this.toolStripButton_ClearModel,
            this.toolStripButton_InputImage});
            this.toolStrip3.Location = new System.Drawing.Point(0, 0);
            this.toolStrip3.Name = "toolStrip3";
            this.toolStrip3.Size = new System.Drawing.Size(548, 45);
            this.toolStrip3.TabIndex = 1;
            this.toolStrip3.Text = "日志";
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(95, 42);
            this.toolStripLabel1.Text = "创建模板ROI";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 45);
            // 
            // ROI_Circle
            // 
            this.ROI_Circle.AutoSize = false;
            this.ROI_Circle.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ROI_Circle.Image = ((System.Drawing.Image)(resources.GetObject("ROI_Circle.Image")));
            this.ROI_Circle.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ROI_Circle.Name = "ROI_Circle";
            this.ROI_Circle.Size = new System.Drawing.Size(50, 45);
            this.ROI_Circle.Text = "圆";
            this.ROI_Circle.Click += new System.EventHandler(this.BnROI_Click);
            // 
            // ROI_Rect2
            // 
            this.ROI_Rect2.AutoSize = false;
            this.ROI_Rect2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ROI_Rect2.Image = ((System.Drawing.Image)(resources.GetObject("ROI_Rect2.Image")));
            this.ROI_Rect2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ROI_Rect2.Name = "ROI_Rect2";
            this.ROI_Rect2.Size = new System.Drawing.Size(50, 45);
            this.ROI_Rect2.Text = "Rect2";
            this.ROI_Rect2.Click += new System.EventHandler(this.BnROI_Click);
            // 
            // toolStripButton_ShowCreateModelRoi
            // 
            this.toolStripButton_ShowCreateModelRoi.AutoSize = false;
            this.toolStripButton_ShowCreateModelRoi.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton_ShowCreateModelRoi.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_ShowCreateModelRoi.Image")));
            this.toolStripButton_ShowCreateModelRoi.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_ShowCreateModelRoi.Name = "toolStripButton_ShowCreateModelRoi";
            this.toolStripButton_ShowCreateModelRoi.Size = new System.Drawing.Size(50, 45);
            this.toolStripButton_ShowCreateModelRoi.Text = "显示";
            this.toolStripButton_ShowCreateModelRoi.Click += new System.EventHandler(this.toolStripButton_ShowCreateModelRoi_Click);
            // 
            // toolStripButton_ModelMask
            // 
            this.toolStripButton_ModelMask.AutoSize = false;
            this.toolStripButton_ModelMask.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton_ModelMask.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_ModelMask.Image")));
            this.toolStripButton_ModelMask.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_ModelMask.Name = "toolStripButton_ModelMask";
            this.toolStripButton_ModelMask.Size = new System.Drawing.Size(80, 45);
            this.toolStripButton_ModelMask.Text = "模板掩膜";
            this.toolStripButton_ModelMask.Visible = false;
            this.toolStripButton_ModelMask.Click += new System.EventHandler(this.toolStripButton_ModelMask_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 45);
            // 
            // toolStripButton_ClearModel
            // 
            this.toolStripButton_ClearModel.AutoSize = false;
            this.toolStripButton_ClearModel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton_ClearModel.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_ClearModel.Image")));
            this.toolStripButton_ClearModel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_ClearModel.Name = "toolStripButton_ClearModel";
            this.toolStripButton_ClearModel.Size = new System.Drawing.Size(80, 45);
            this.toolStripButton_ClearModel.Text = "清除模板";
            this.toolStripButton_ClearModel.Click += new System.EventHandler(this.toolStripButton_ClearModel_Click);
            // 
            // toolStripButton_InputImage
            // 
            this.toolStripButton_InputImage.AutoSize = false;
            this.toolStripButton_InputImage.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton_InputImage.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_InputImage.Image")));
            this.toolStripButton_InputImage.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_InputImage.Name = "toolStripButton_InputImage";
            this.toolStripButton_InputImage.Size = new System.Drawing.Size(80, 45);
            this.toolStripButton_InputImage.Text = "导入图像";
            this.toolStripButton_InputImage.Click += new System.EventHandler(this.toolStripButton_InputImage_Click);
            // 
            // BnSaveModel
            // 
            this.BnSaveModel.Location = new System.Drawing.Point(390, 519);
            this.BnSaveModel.Name = "BnSaveModel";
            this.BnSaveModel.Size = new System.Drawing.Size(91, 42);
            this.BnSaveModel.TabIndex = 23;
            this.BnSaveModel.Text = "保存";
            this.BnSaveModel.UseVisualStyleBackColor = true;
            this.BnSaveModel.Click += new System.EventHandler(this.BnSaveModel_Click);
            // 
            // checkBox_EnableNcc
            // 
            this.checkBox_EnableNcc.AutoSize = true;
            this.checkBox_EnableNcc.Location = new System.Drawing.Point(388, 397);
            this.checkBox_EnableNcc.Name = "checkBox_EnableNcc";
            this.checkBox_EnableNcc.Size = new System.Drawing.Size(93, 24);
            this.checkBox_EnableNcc.TabIndex = 27;
            this.checkBox_EnableNcc.Text = "启用NCC";
            this.checkBox_EnableNcc.UseVisualStyleBackColor = true;
            this.checkBox_EnableNcc.CheckedChanged += new System.EventHandler(this.checkBox_EnableNcc_CheckedChanged);
            // 
            // ShapeModelFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1094, 621);
            this.Controls.Add(this.splitContainer1);
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "ShapeModelFrm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ShapeModelFrm";
            this.Load += new System.EventHandler(this.ShapeModelFrm_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ShapeModelFrm_KeyDown);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.contextMenuStrip1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.toolStrip3.ResumeLayout(false);
            this.toolStrip3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ToolStrip toolStrip3;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton ROI_Circle;
        private System.Windows.Forms.ToolStripButton ROI_Rect2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton toolStripButton_InputImage;
        private System.Windows.Forms.Button BnSaveModel;
        private System.Windows.Forms.Button BnFindModel;
        private System.Windows.Forms.Button BnCreateModel;
        private System.Windows.Forms.Button BnAddSearchROI;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripButton toolStripButton_ShowCreateModelRoi;
        private System.Windows.Forms.ToolStripButton toolStripButton_ModelMask;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.PropertyGrid propertyGrid1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ListBox listBox_FindModelROI;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 删除ToolStripMenuItem;
        private System.Windows.Forms.Button BnInputTestImage;
        private System.Windows.Forms.Button BnTestModelImage;
        private System.Windows.Forms.ToolStripButton toolStripButton_ClearModel;
        private System.Windows.Forms.CheckBox checkBox_EnableNcc;
    }
}