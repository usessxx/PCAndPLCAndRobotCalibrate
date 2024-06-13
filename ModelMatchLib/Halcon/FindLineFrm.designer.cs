namespace MatchModel.Halcon
{
    partial class FindLineFrm
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.label3 = new System.Windows.Forms.Label();
            this.BnAddFindEdgeRegion = new System.Windows.Forms.Button();
            this.checkBox_ShowAllFindEdgeRegion = new System.Windows.Forms.CheckBox();
            this.Cmb_FindLineTransition = new System.Windows.Forms.ComboBox();
            this.NdFindLineEdgeThreshold = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.listBox_FindEdge = new System.Windows.Forms.ListBox();
            this.contextMenuStrip_FindEdge = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.删除ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.删除所有ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label2 = new System.Windows.Forms.Label();
            this.BnSaveFindEdge = new System.Windows.Forms.Button();
            this.BnTestFindEdge = new System.Windows.Forms.Button();
            this.Cmb_FindLineSelect = new System.Windows.Forms.ComboBox();
            this.contextMenuStrip_FaiEdit = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.删除此行ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip_DetectSilver = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NdFindLineEdgeThreshold)).BeginInit();
            this.contextMenuStrip_FindEdge.SuspendLayout();
            this.contextMenuStrip_FaiEdit.SuspendLayout();
            this.contextMenuStrip_DetectSilver.SuspendLayout();
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
            this.splitContainer1.Panel2.Controls.Add(this.label3);
            this.splitContainer1.Panel2.Controls.Add(this.BnAddFindEdgeRegion);
            this.splitContainer1.Panel2.Controls.Add(this.checkBox_ShowAllFindEdgeRegion);
            this.splitContainer1.Panel2.Controls.Add(this.Cmb_FindLineTransition);
            this.splitContainer1.Panel2.Controls.Add(this.NdFindLineEdgeThreshold);
            this.splitContainer1.Panel2.Controls.Add(this.label1);
            this.splitContainer1.Panel2.Controls.Add(this.listBox_FindEdge);
            this.splitContainer1.Panel2.Controls.Add(this.label2);
            this.splitContainer1.Panel2.Controls.Add(this.BnSaveFindEdge);
            this.splitContainer1.Panel2.Controls.Add(this.BnTestFindEdge);
            this.splitContainer1.Panel2.Controls.Add(this.Cmb_FindLineSelect);
            this.splitContainer1.Size = new System.Drawing.Size(909, 646);
            this.splitContainer1.SplitterDistance = 579;
            this.splitContainer1.SplitterWidth = 5;
            this.splitContainer1.TabIndex = 0;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(41, 391);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 17);
            this.label3.TabIndex = 44;
            this.label3.Text = "边缘阈值";
            // 
            // BnAddFindEdgeRegion
            // 
            this.BnAddFindEdgeRegion.Location = new System.Drawing.Point(206, 68);
            this.BnAddFindEdgeRegion.Name = "BnAddFindEdgeRegion";
            this.BnAddFindEdgeRegion.Size = new System.Drawing.Size(82, 34);
            this.BnAddFindEdgeRegion.TabIndex = 36;
            this.BnAddFindEdgeRegion.Text = "添加区域";
            this.BnAddFindEdgeRegion.UseVisualStyleBackColor = true;
            this.BnAddFindEdgeRegion.Click += new System.EventHandler(this.BnAddFindEdgeRegion_Click);
            // 
            // checkBox_ShowAllFindEdgeRegion
            // 
            this.checkBox_ShowAllFindEdgeRegion.AutoSize = true;
            this.checkBox_ShowAllFindEdgeRegion.Location = new System.Drawing.Point(45, 70);
            this.checkBox_ShowAllFindEdgeRegion.Name = "checkBox_ShowAllFindEdgeRegion";
            this.checkBox_ShowAllFindEdgeRegion.Size = new System.Drawing.Size(75, 21);
            this.checkBox_ShowAllFindEdgeRegion.TabIndex = 46;
            this.checkBox_ShowAllFindEdgeRegion.Text = "显示所有";
            this.checkBox_ShowAllFindEdgeRegion.UseVisualStyleBackColor = true;
            this.checkBox_ShowAllFindEdgeRegion.CheckedChanged += new System.EventHandler(this.checkBox_ShowAllFindEdgeRegion_CheckedChanged);
            // 
            // Cmb_FindLineTransition
            // 
            this.Cmb_FindLineTransition.FormattingEnabled = true;
            this.Cmb_FindLineTransition.Items.AddRange(new object[] {
            "所有",
            "黑到白",
            "白到黑"});
            this.Cmb_FindLineTransition.Location = new System.Drawing.Point(116, 302);
            this.Cmb_FindLineTransition.Name = "Cmb_FindLineTransition";
            this.Cmb_FindLineTransition.Size = new System.Drawing.Size(121, 25);
            this.Cmb_FindLineTransition.TabIndex = 41;
            this.Cmb_FindLineTransition.Text = "所有";
            this.Cmb_FindLineTransition.SelectedIndexChanged += new System.EventHandler(this.Cmb_FindLineTransition_SelectedIndexChanged);
            // 
            // NdFindLineEdgeThreshold
            // 
            this.NdFindLineEdgeThreshold.Location = new System.Drawing.Point(116, 389);
            this.NdFindLineEdgeThreshold.Maximum = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.NdFindLineEdgeThreshold.Name = "NdFindLineEdgeThreshold";
            this.NdFindLineEdgeThreshold.Size = new System.Drawing.Size(121, 23);
            this.NdFindLineEdgeThreshold.TabIndex = 45;
            this.NdFindLineEdgeThreshold.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.NdFindLineEdgeThreshold.ValueChanged += new System.EventHandler(this.NdFindLineEdgeThreshold_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(45, 307);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 17);
            this.label1.TabIndex = 40;
            this.label1.Text = "边缘极性";
            // 
            // listBox_FindEdge
            // 
            this.listBox_FindEdge.ContextMenuStrip = this.contextMenuStrip_FindEdge;
            this.listBox_FindEdge.FormattingEnabled = true;
            this.listBox_FindEdge.ItemHeight = 17;
            this.listBox_FindEdge.Location = new System.Drawing.Point(45, 103);
            this.listBox_FindEdge.Name = "listBox_FindEdge";
            this.listBox_FindEdge.Size = new System.Drawing.Size(120, 157);
            this.listBox_FindEdge.TabIndex = 34;
            this.listBox_FindEdge.SelectedIndexChanged += new System.EventHandler(this.listBox_FindEdge_SelectedIndexChanged);
            // 
            // contextMenuStrip_FindEdge
            // 
            this.contextMenuStrip_FindEdge.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip_FindEdge.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.删除ToolStripMenuItem,
            this.删除所有ToolStripMenuItem});
            this.contextMenuStrip_FindEdge.Name = "contextMenuStrip_FindEdge";
            this.contextMenuStrip_FindEdge.Size = new System.Drawing.Size(125, 48);
            // 
            // 删除ToolStripMenuItem
            // 
            this.删除ToolStripMenuItem.Name = "删除ToolStripMenuItem";
            this.删除ToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.删除ToolStripMenuItem.Text = "删除";
            this.删除ToolStripMenuItem.Click += new System.EventHandler(this.删除ToolStripMenuItem_Click);
            // 
            // 删除所有ToolStripMenuItem
            // 
            this.删除所有ToolStripMenuItem.Name = "删除所有ToolStripMenuItem";
            this.删除所有ToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.删除所有ToolStripMenuItem.Text = "删除所有";
            this.删除所有ToolStripMenuItem.Click += new System.EventHandler(this.删除所有ToolStripMenuItem_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(45, 352);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 17);
            this.label2.TabIndex = 42;
            this.label2.Text = "选点方式";
            // 
            // BnSaveFindEdge
            // 
            this.BnSaveFindEdge.Location = new System.Drawing.Point(206, 203);
            this.BnSaveFindEdge.Name = "BnSaveFindEdge";
            this.BnSaveFindEdge.Size = new System.Drawing.Size(82, 34);
            this.BnSaveFindEdge.TabIndex = 38;
            this.BnSaveFindEdge.Text = "保存";
            this.BnSaveFindEdge.UseVisualStyleBackColor = true;
            this.BnSaveFindEdge.Click += new System.EventHandler(this.BnSaveFindEdge_Click);
            // 
            // BnTestFindEdge
            // 
            this.BnTestFindEdge.Location = new System.Drawing.Point(206, 132);
            this.BnTestFindEdge.Name = "BnTestFindEdge";
            this.BnTestFindEdge.Size = new System.Drawing.Size(82, 34);
            this.BnTestFindEdge.TabIndex = 39;
            this.BnTestFindEdge.Text = "测试";
            this.BnTestFindEdge.UseVisualStyleBackColor = true;
            this.BnTestFindEdge.Click += new System.EventHandler(this.BnTestFindEdge_Click);
            // 
            // Cmb_FindLineSelect
            // 
            this.Cmb_FindLineSelect.FormattingEnabled = true;
            this.Cmb_FindLineSelect.Items.AddRange(new object[] {
            "所有",
            "第一点",
            "最后点"});
            this.Cmb_FindLineSelect.Location = new System.Drawing.Point(116, 348);
            this.Cmb_FindLineSelect.Name = "Cmb_FindLineSelect";
            this.Cmb_FindLineSelect.Size = new System.Drawing.Size(121, 25);
            this.Cmb_FindLineSelect.TabIndex = 43;
            this.Cmb_FindLineSelect.Text = "所有";
            this.Cmb_FindLineSelect.SelectedIndexChanged += new System.EventHandler(this.Cmb_FindLineSelect_SelectedIndexChanged);
            // 
            // contextMenuStrip_FaiEdit
            // 
            this.contextMenuStrip_FaiEdit.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip_FaiEdit.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.删除此行ToolStripMenuItem});
            this.contextMenuStrip_FaiEdit.Name = "contextMenuStrip_FaiEdit";
            this.contextMenuStrip_FaiEdit.Size = new System.Drawing.Size(125, 26);
            // 
            // 删除此行ToolStripMenuItem
            // 
            this.删除此行ToolStripMenuItem.Name = "删除此行ToolStripMenuItem";
            this.删除此行ToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.删除此行ToolStripMenuItem.Text = "删除此行";
            // 
            // contextMenuStrip_DetectSilver
            // 
            this.contextMenuStrip_DetectSilver.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip_DetectSilver.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.toolStripMenuItem2});
            this.contextMenuStrip_DetectSilver.Name = "contextMenuStrip_FindEdge";
            this.contextMenuStrip_DetectSilver.Size = new System.Drawing.Size(125, 48);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(124, 22);
            this.toolStripMenuItem1.Text = "删除";
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(124, 22);
            this.toolStripMenuItem2.Text = "删除所有";
            // 
            // DetectSilverFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(909, 646);
            this.Controls.Add(this.splitContainer1);
            this.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "DetectSilverFrm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "RectEdgeFrm";
            this.Load += new System.EventHandler(this.RectEdgeFrm_Load);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.NdFindLineEdgeThreshold)).EndInit();
            this.contextMenuStrip_FindEdge.ResumeLayout(false);
            this.contextMenuStrip_FaiEdit.ResumeLayout(false);
            this.contextMenuStrip_DetectSilver.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ListBox listBox_FindEdge;
        private System.Windows.Forms.Button BnAddFindEdgeRegion;
        private System.Windows.Forms.Button BnTestFindEdge;
        private System.Windows.Forms.Button BnSaveFindEdge;
        private System.Windows.Forms.NumericUpDown NdFindLineEdgeThreshold;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox Cmb_FindLineSelect;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox Cmb_FindLineTransition;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox checkBox_ShowAllFindEdgeRegion;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip_FindEdge;
        private System.Windows.Forms.ToolStripMenuItem 删除ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 删除所有ToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip_DetectSilver;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip_FaiEdit;
        private System.Windows.Forms.ToolStripMenuItem 删除此行ToolStripMenuItem;
    }
}