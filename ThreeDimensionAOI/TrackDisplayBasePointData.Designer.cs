namespace ThreeDimensionAVI
{
    partial class TrackDisplayBasePointData
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
            this.btnImageExport = new System.Windows.Forms.Button();
            this.lblProductName = new System.Windows.Forms.Label();
            this.pnlTrackDispRegion = new System.Windows.Forms.Panel();
            this.picDispTrack = new System.Windows.Forms.PictureBox();
            this.cmsTrackImageControl = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.恢复原图大小ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.导出轨迹图ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pnlTrackDispRegion.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picDispTrack)).BeginInit();
            this.cmsTrackImageControl.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnImageExport
            // 
            this.btnImageExport.BackColor = System.Drawing.SystemColors.Control;
            this.btnImageExport.Font = new System.Drawing.Font("KaiTi", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnImageExport.Location = new System.Drawing.Point(985, 6);
            this.btnImageExport.Name = "btnImageExport";
            this.btnImageExport.Size = new System.Drawing.Size(71, 28);
            this.btnImageExport.TabIndex = 9;
            this.btnImageExport.Text = "导出";
            this.btnImageExport.UseVisualStyleBackColor = false;
            this.btnImageExport.Click += new System.EventHandler(this.导出轨迹图ToolStripMenuItem_Click);
            // 
            // lblProductName
            // 
            this.lblProductName.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProductName.Location = new System.Drawing.Point(31, 6);
            this.lblProductName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblProductName.Name = "lblProductName";
            this.lblProductName.Size = new System.Drawing.Size(947, 28);
            this.lblProductName.TabIndex = 8;
            this.lblProductName.Text = "JA2606-05-B";
            this.lblProductName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pnlTrackDispRegion
            // 
            this.pnlTrackDispRegion.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlTrackDispRegion.Controls.Add(this.picDispTrack);
            this.pnlTrackDispRegion.Location = new System.Drawing.Point(31, 38);
            this.pnlTrackDispRegion.Margin = new System.Windows.Forms.Padding(4);
            this.pnlTrackDispRegion.Name = "pnlTrackDispRegion";
            this.pnlTrackDispRegion.Size = new System.Drawing.Size(1024, 725);
            this.pnlTrackDispRegion.TabIndex = 5;
            // 
            // picDispTrack
            // 
            this.picDispTrack.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.picDispTrack.Location = new System.Drawing.Point(0, 0);
            this.picDispTrack.Margin = new System.Windows.Forms.Padding(4);
            this.picDispTrack.Name = "picDispTrack";
            this.picDispTrack.Size = new System.Drawing.Size(1024, 725);
            this.picDispTrack.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picDispTrack.TabIndex = 0;
            this.picDispTrack.TabStop = false;
            this.picDispTrack.MouseClick += new System.Windows.Forms.MouseEventHandler(this.picDispTrack_MouseClick);
            this.picDispTrack.MouseDown += new System.Windows.Forms.MouseEventHandler(this.picDispTrack_MouseDown);
            this.picDispTrack.MouseLeave += new System.EventHandler(this.picDispTrack_MouseLeave);
            this.picDispTrack.MouseMove += new System.Windows.Forms.MouseEventHandler(this.picDispTrack_MouseMove);
            this.picDispTrack.MouseUp += new System.Windows.Forms.MouseEventHandler(this.picDispTrack_MouseUp);
            // 
            // cmsTrackImageControl
            // 
            this.cmsTrackImageControl.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.恢复原图大小ToolStripMenuItem,
            this.导出轨迹图ToolStripMenuItem});
            this.cmsTrackImageControl.Name = "cmsImageExport";
            this.cmsTrackImageControl.Size = new System.Drawing.Size(149, 48);
            // 
            // 恢复原图大小ToolStripMenuItem
            // 
            this.恢复原图大小ToolStripMenuItem.Name = "恢复原图大小ToolStripMenuItem";
            this.恢复原图大小ToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.恢复原图大小ToolStripMenuItem.Text = "恢复原图大小";
            this.恢复原图大小ToolStripMenuItem.Click += new System.EventHandler(this.恢复原图大小ToolStripMenuItem_Click);
            // 
            // 导出轨迹图ToolStripMenuItem
            // 
            this.导出轨迹图ToolStripMenuItem.Name = "导出轨迹图ToolStripMenuItem";
            this.导出轨迹图ToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.导出轨迹图ToolStripMenuItem.Text = "导出轨迹图";
            this.导出轨迹图ToolStripMenuItem.Click += new System.EventHandler(this.导出轨迹图ToolStripMenuItem_Click);
            // 
            // TrackDisplayBasePointData
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1083, 768);
            this.Controls.Add(this.btnImageExport);
            this.Controls.Add(this.lblProductName);
            this.Controls.Add(this.pnlTrackDispRegion);
            this.Font = new System.Drawing.Font("KaiTi", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TrackDisplayBasePointData";
            this.Text = "Track";
            this.Load += new System.EventHandler(this.TrackDisplayBasePointData_Load);
            this.pnlTrackDispRegion.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picDispTrack)).EndInit();
            this.cmsTrackImageControl.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnImageExport;
        private System.Windows.Forms.Label lblProductName;
        private System.Windows.Forms.Panel pnlTrackDispRegion;
        private System.Windows.Forms.PictureBox picDispTrack;
        private System.Windows.Forms.ContextMenuStrip cmsTrackImageControl;
        private System.Windows.Forms.ToolStripMenuItem 恢复原图大小ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 导出轨迹图ToolStripMenuItem;

    }
}