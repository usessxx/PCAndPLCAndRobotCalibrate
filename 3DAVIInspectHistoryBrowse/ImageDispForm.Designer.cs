namespace ThreeDAVIInspectHistoryBrowse
{
    partial class ImageDispForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImageDispForm));
            this.picDispInspectImage = new System.Windows.Forms.PictureBox();
            this.pnlImageDispRegion = new System.Windows.Forms.Panel();
            this.btnNextImage = new UCArrow.UCArrow();
            this.btnPreviousImage = new UCArrow.UCArrow();
            this.lblImageName = new System.Windows.Forms.Label();
            this.cmsImageExport = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.导出ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.导出当前图片对应的原图ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnImageExport = new System.Windows.Forms.Button();
            this.显示当前图片的原图ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.picDispInspectImage)).BeginInit();
            this.pnlImageDispRegion.SuspendLayout();
            this.cmsImageExport.SuspendLayout();
            this.SuspendLayout();
            // 
            // picDispInspectImage
            // 
            this.picDispInspectImage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.picDispInspectImage.Location = new System.Drawing.Point(0, 0);
            this.picDispInspectImage.Margin = new System.Windows.Forms.Padding(4);
            this.picDispInspectImage.Name = "picDispInspectImage";
            this.picDispInspectImage.Size = new System.Drawing.Size(809, 603);
            this.picDispInspectImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picDispInspectImage.TabIndex = 0;
            this.picDispInspectImage.TabStop = false;
            this.picDispInspectImage.MouseClick += new System.Windows.Forms.MouseEventHandler(this.picDispInspectImage_MouseClick);
            this.picDispInspectImage.MouseDown += new System.Windows.Forms.MouseEventHandler(this.picDispInspectImage_MouseDown);
            this.picDispInspectImage.MouseLeave += new System.EventHandler(this.picDispInspectImage_MouseLeave);
            this.picDispInspectImage.MouseMove += new System.Windows.Forms.MouseEventHandler(this.picDispInspectImage_MouseMove);
            this.picDispInspectImage.MouseUp += new System.Windows.Forms.MouseEventHandler(this.picDispInspectImage_MouseUp);
            // 
            // pnlImageDispRegion
            // 
            this.pnlImageDispRegion.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlImageDispRegion.Controls.Add(this.picDispInspectImage);
            this.pnlImageDispRegion.Location = new System.Drawing.Point(72, 37);
            this.pnlImageDispRegion.Margin = new System.Windows.Forms.Padding(4);
            this.pnlImageDispRegion.Name = "pnlImageDispRegion";
            this.pnlImageDispRegion.Size = new System.Drawing.Size(809, 603);
            this.pnlImageDispRegion.TabIndex = 1;
            // 
            // btnNextImage
            // 
            this.btnNextImage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNextImage.ArrowColor = System.Drawing.SystemColors.ControlDarkDark;
            this.btnNextImage.BorderColor = null;
            this.btnNextImage.Direction = UCArrow.ArrowDirection.Right;
            this.btnNextImage.Font = new System.Drawing.Font("KaiTi", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNextImage.ForeColor = System.Drawing.Color.White;
            this.btnNextImage.Location = new System.Drawing.Point(889, 235);
            this.btnNextImage.Margin = new System.Windows.Forms.Padding(4);
            this.btnNextImage.Name = "btnNextImage";
            this.btnNextImage.Size = new System.Drawing.Size(56, 174);
            this.btnNextImage.TabIndex = 2;
            this.btnNextImage.Text = "下一张";
            this.btnNextImage.Click += new System.EventHandler(this.btnNextImageClick);
            // 
            // btnPreviousImage
            // 
            this.btnPreviousImage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.btnPreviousImage.ArrowColor = System.Drawing.SystemColors.ControlDarkDark;
            this.btnPreviousImage.BorderColor = null;
            this.btnPreviousImage.Direction = UCArrow.ArrowDirection.Left;
            this.btnPreviousImage.Font = new System.Drawing.Font("KaiTi", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPreviousImage.ForeColor = System.Drawing.Color.White;
            this.btnPreviousImage.Location = new System.Drawing.Point(5, 235);
            this.btnPreviousImage.Margin = new System.Windows.Forms.Padding(4);
            this.btnPreviousImage.Name = "btnPreviousImage";
            this.btnPreviousImage.Size = new System.Drawing.Size(56, 174);
            this.btnPreviousImage.TabIndex = 2;
            this.btnPreviousImage.Text = "上一张";
            this.btnPreviousImage.Click += new System.EventHandler(this.btnPreviousImageClick);
            // 
            // lblImageName
            // 
            this.lblImageName.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblImageName.Location = new System.Drawing.Point(72, 5);
            this.lblImageName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblImageName.Name = "lblImageName";
            this.lblImageName.Size = new System.Drawing.Size(731, 28);
            this.lblImageName.TabIndex = 3;
            this.lblImageName.Text = "L_3-S202_20220519093738122.jpg";
            this.lblImageName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cmsImageExport
            // 
            this.cmsImageExport.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.导出ToolStripMenuItem,
            this.导出当前图片对应的原图ToolStripMenuItem,
            this.显示当前图片的原图ToolStripMenuItem});
            this.cmsImageExport.Name = "cmsImageExport";
            this.cmsImageExport.Size = new System.Drawing.Size(209, 70);
            // 
            // 导出ToolStripMenuItem
            // 
            this.导出ToolStripMenuItem.Name = "导出ToolStripMenuItem";
            this.导出ToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.导出ToolStripMenuItem.Text = "导出当前图片";
            this.导出ToolStripMenuItem.Click += new System.EventHandler(this.导出ToolStripMenuItem_Click);
            // 
            // 导出当前图片对应的原图ToolStripMenuItem
            // 
            this.导出当前图片对应的原图ToolStripMenuItem.Name = "导出当前图片对应的原图ToolStripMenuItem";
            this.导出当前图片对应的原图ToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.导出当前图片对应的原图ToolStripMenuItem.Text = "导出当前图片对应的原图";
            this.导出当前图片对应的原图ToolStripMenuItem.Click += new System.EventHandler(this.导出当前图片对应的原图ToolStripMenuItem_Click);
            // 
            // btnImageExport
            // 
            this.btnImageExport.BackColor = System.Drawing.SystemColors.Control;
            this.btnImageExport.Font = new System.Drawing.Font("KaiTi", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnImageExport.Location = new System.Drawing.Point(810, 5);
            this.btnImageExport.Name = "btnImageExport";
            this.btnImageExport.Size = new System.Drawing.Size(71, 28);
            this.btnImageExport.TabIndex = 4;
            this.btnImageExport.Text = "导出";
            this.btnImageExport.UseVisualStyleBackColor = false;
            this.btnImageExport.Click += new System.EventHandler(this.导出ToolStripMenuItem_Click);
            // 
            // 显示当前图片的原图ToolStripMenuItem
            // 
            this.显示当前图片的原图ToolStripMenuItem.Name = "显示当前图片的原图ToolStripMenuItem";
            this.显示当前图片的原图ToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.显示当前图片的原图ToolStripMenuItem.Text = "显示当前图片的原图";
            this.显示当前图片的原图ToolStripMenuItem.Click += new System.EventHandler(this.显示当前图片的原图ToolStripMenuItem_Click);
            // 
            // ImageDispForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(954, 646);
            this.Controls.Add(this.btnImageExport);
            this.Controls.Add(this.lblImageName);
            this.Controls.Add(this.btnPreviousImage);
            this.Controls.Add(this.btnNextImage);
            this.Controls.Add(this.pnlImageDispRegion);
            this.Font = new System.Drawing.Font("KaiTi", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ImageDispForm";
            this.Text = "图像浏览";
            this.Load += new System.EventHandler(this.ImageDispForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.picDispInspectImage)).EndInit();
            this.pnlImageDispRegion.ResumeLayout(false);
            this.cmsImageExport.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox picDispInspectImage;
        private System.Windows.Forms.Panel pnlImageDispRegion;
        private UCArrow.UCArrow btnNextImage;
        private UCArrow.UCArrow btnPreviousImage;
        private System.Windows.Forms.Label lblImageName;
        private System.Windows.Forms.ContextMenuStrip cmsImageExport;
        private System.Windows.Forms.ToolStripMenuItem 导出ToolStripMenuItem;
        private System.Windows.Forms.Button btnImageExport;
        private System.Windows.Forms.ToolStripMenuItem 导出当前图片对应的原图ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 显示当前图片的原图ToolStripMenuItem;
    }
}