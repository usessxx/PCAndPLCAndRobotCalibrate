using System.Threading;

namespace AxisAndIOForm
{
    partial class MotionCardIOForm
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
            if (_ioStatusUpdateThread != null)
            {
                _ioStatusUpdateThread.Abort();
                _ioStatusUpdateThread = null;
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
            this.lblIOName001 = new System.Windows.Forms.Label();
            this.picIOStatus001 = new System.Windows.Forms.PictureBox();
            this.picIOStatus002 = new System.Windows.Forms.PictureBox();
            this.lblIOName002 = new System.Windows.Forms.Label();
            this.picIOStatus003 = new System.Windows.Forms.PictureBox();
            this.lblIOName003 = new System.Windows.Forms.Label();
            this.picIOStatus004 = new System.Windows.Forms.PictureBox();
            this.lblIOName004 = new System.Windows.Forms.Label();
            this.picIOStatus005 = new System.Windows.Forms.PictureBox();
            this.lblIOName005 = new System.Windows.Forms.Label();
            this.picIOStatus006 = new System.Windows.Forms.PictureBox();
            this.lblIOName006 = new System.Windows.Forms.Label();
            this.picIOStatus007 = new System.Windows.Forms.PictureBox();
            this.lblIOName007 = new System.Windows.Forms.Label();
            this.picIOStatus008 = new System.Windows.Forms.PictureBox();
            this.lblIOName008 = new System.Windows.Forms.Label();
            this.picIOStatus009 = new System.Windows.Forms.PictureBox();
            this.lblIOName009 = new System.Windows.Forms.Label();
            this.picIOStatus010 = new System.Windows.Forms.PictureBox();
            this.lblIOName010 = new System.Windows.Forms.Label();
            this.picIOStatus011 = new System.Windows.Forms.PictureBox();
            this.lblIOName011 = new System.Windows.Forms.Label();
            this.picIOStatus012 = new System.Windows.Forms.PictureBox();
            this.lblIOName012 = new System.Windows.Forms.Label();
            this.picIOStatus013 = new System.Windows.Forms.PictureBox();
            this.lblIOName013 = new System.Windows.Forms.Label();
            this.picIOStatus014 = new System.Windows.Forms.PictureBox();
            this.lblIOName014 = new System.Windows.Forms.Label();
            this.picIOStatus015 = new System.Windows.Forms.PictureBox();
            this.lblIOName015 = new System.Windows.Forms.Label();
            this.picIOStatus016 = new System.Windows.Forms.PictureBox();
            this.lblIOName016 = new System.Windows.Forms.Label();
            this.picIOStatus017 = new System.Windows.Forms.PictureBox();
            this.lblIOName017 = new System.Windows.Forms.Label();
            this.picIOStatus018 = new System.Windows.Forms.PictureBox();
            this.lblIOName018 = new System.Windows.Forms.Label();
            this.picIOStatus019 = new System.Windows.Forms.PictureBox();
            this.lblIOName019 = new System.Windows.Forms.Label();
            this.picIOStatus020 = new System.Windows.Forms.PictureBox();
            this.lblIOName020 = new System.Windows.Forms.Label();
            this.picIOStatus021 = new System.Windows.Forms.PictureBox();
            this.lblIOName021 = new System.Windows.Forms.Label();
            this.picIOStatus022 = new System.Windows.Forms.PictureBox();
            this.lblIOName022 = new System.Windows.Forms.Label();
            this.picIOStatus023 = new System.Windows.Forms.PictureBox();
            this.lblIOName023 = new System.Windows.Forms.Label();
            this.picIOStatus024 = new System.Windows.Forms.PictureBox();
            this.picIOStatus025 = new System.Windows.Forms.PictureBox();
            this.lblIOName024 = new System.Windows.Forms.Label();
            this.picIOStatus026 = new System.Windows.Forms.PictureBox();
            this.lblIOName025 = new System.Windows.Forms.Label();
            this.picIOStatus027 = new System.Windows.Forms.PictureBox();
            this.lblIOName026 = new System.Windows.Forms.Label();
            this.picIOStatus028 = new System.Windows.Forms.PictureBox();
            this.lblIOName027 = new System.Windows.Forms.Label();
            this.picIOStatus029 = new System.Windows.Forms.PictureBox();
            this.picIOStatus030 = new System.Windows.Forms.PictureBox();
            this.lblIOName028 = new System.Windows.Forms.Label();
            this.picIOStatus031 = new System.Windows.Forms.PictureBox();
            this.lblIOName029 = new System.Windows.Forms.Label();
            this.picIOStatus032 = new System.Windows.Forms.PictureBox();
            this.lblIOName030 = new System.Windows.Forms.Label();
            this.lblIOName031 = new System.Windows.Forms.Label();
            this.lblIOName032 = new System.Windows.Forms.Label();
            this.btnInput = new System.Windows.Forms.Button();
            this.btnOutput = new System.Windows.Forms.Button();
            this.btnNextPage = new System.Windows.Forms.Button();
            this.btnPrevPage = new System.Windows.Forms.Button();
            this.lblCurrentPage = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.picIOStatus001)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picIOStatus002)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picIOStatus003)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picIOStatus004)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picIOStatus005)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picIOStatus006)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picIOStatus007)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picIOStatus008)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picIOStatus009)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picIOStatus010)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picIOStatus011)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picIOStatus012)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picIOStatus013)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picIOStatus014)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picIOStatus015)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picIOStatus016)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picIOStatus017)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picIOStatus018)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picIOStatus019)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picIOStatus020)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picIOStatus021)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picIOStatus022)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picIOStatus023)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picIOStatus024)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picIOStatus025)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picIOStatus026)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picIOStatus027)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picIOStatus028)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picIOStatus029)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picIOStatus030)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picIOStatus031)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picIOStatus032)).BeginInit();
            this.SuspendLayout();
            // 
            // lblIOName001
            // 
            this.lblIOName001.AutoSize = true;
            this.lblIOName001.Location = new System.Drawing.Point(73, 20);
            this.lblIOName001.Name = "lblIOName001";
            this.lblIOName001.Size = new System.Drawing.Size(326, 16);
            this.lblIOName001.TabIndex = 56;
            this.lblIOName001.Text = "Speedpin fastening station transfer X1 axis home sensor";
            // 
            // picIOStatus001
            // 
            this.picIOStatus001.Location = new System.Drawing.Point(14, 17);
            this.picIOStatus001.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.picIOStatus001.Name = "picIOStatus001";
            this.picIOStatus001.Size = new System.Drawing.Size(53, 27);
            this.picIOStatus001.TabIndex = 57;
            this.picIOStatus001.TabStop = false;
            // 
            // picIOStatus002
            // 
            this.picIOStatus002.Location = new System.Drawing.Point(14, 46);
            this.picIOStatus002.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.picIOStatus002.Name = "picIOStatus002";
            this.picIOStatus002.Size = new System.Drawing.Size(53, 27);
            this.picIOStatus002.TabIndex = 57;
            this.picIOStatus002.TabStop = false;
            // 
            // lblIOName002
            // 
            this.lblIOName002.AutoSize = true;
            this.lblIOName002.Location = new System.Drawing.Point(73, 49);
            this.lblIOName002.Name = "lblIOName002";
            this.lblIOName002.Size = new System.Drawing.Size(326, 16);
            this.lblIOName002.TabIndex = 56;
            this.lblIOName002.Text = "Speedpin fastening station transfer X1 axis home sensor";
            // 
            // picIOStatus003
            // 
            this.picIOStatus003.Location = new System.Drawing.Point(14, 75);
            this.picIOStatus003.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.picIOStatus003.Name = "picIOStatus003";
            this.picIOStatus003.Size = new System.Drawing.Size(53, 27);
            this.picIOStatus003.TabIndex = 57;
            this.picIOStatus003.TabStop = false;
            // 
            // lblIOName003
            // 
            this.lblIOName003.AutoSize = true;
            this.lblIOName003.Location = new System.Drawing.Point(73, 78);
            this.lblIOName003.Name = "lblIOName003";
            this.lblIOName003.Size = new System.Drawing.Size(326, 16);
            this.lblIOName003.TabIndex = 56;
            this.lblIOName003.Text = "Speedpin fastening station transfer X1 axis home sensor";
            // 
            // picIOStatus004
            // 
            this.picIOStatus004.Location = new System.Drawing.Point(14, 103);
            this.picIOStatus004.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.picIOStatus004.Name = "picIOStatus004";
            this.picIOStatus004.Size = new System.Drawing.Size(53, 27);
            this.picIOStatus004.TabIndex = 57;
            this.picIOStatus004.TabStop = false;
            // 
            // lblIOName004
            // 
            this.lblIOName004.AutoSize = true;
            this.lblIOName004.Location = new System.Drawing.Point(73, 107);
            this.lblIOName004.Name = "lblIOName004";
            this.lblIOName004.Size = new System.Drawing.Size(326, 16);
            this.lblIOName004.TabIndex = 56;
            this.lblIOName004.Text = "Speedpin fastening station transfer X1 axis home sensor";
            // 
            // picIOStatus005
            // 
            this.picIOStatus005.Location = new System.Drawing.Point(14, 132);
            this.picIOStatus005.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.picIOStatus005.Name = "picIOStatus005";
            this.picIOStatus005.Size = new System.Drawing.Size(53, 27);
            this.picIOStatus005.TabIndex = 57;
            this.picIOStatus005.TabStop = false;
            // 
            // lblIOName005
            // 
            this.lblIOName005.AutoSize = true;
            this.lblIOName005.Location = new System.Drawing.Point(73, 135);
            this.lblIOName005.Name = "lblIOName005";
            this.lblIOName005.Size = new System.Drawing.Size(326, 16);
            this.lblIOName005.TabIndex = 56;
            this.lblIOName005.Text = "Speedpin fastening station transfer X1 axis home sensor";
            // 
            // picIOStatus006
            // 
            this.picIOStatus006.Location = new System.Drawing.Point(14, 161);
            this.picIOStatus006.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.picIOStatus006.Name = "picIOStatus006";
            this.picIOStatus006.Size = new System.Drawing.Size(53, 27);
            this.picIOStatus006.TabIndex = 57;
            this.picIOStatus006.TabStop = false;
            // 
            // lblIOName006
            // 
            this.lblIOName006.AutoSize = true;
            this.lblIOName006.Location = new System.Drawing.Point(73, 164);
            this.lblIOName006.Name = "lblIOName006";
            this.lblIOName006.Size = new System.Drawing.Size(326, 16);
            this.lblIOName006.TabIndex = 56;
            this.lblIOName006.Text = "Speedpin fastening station transfer X1 axis home sensor";
            // 
            // picIOStatus007
            // 
            this.picIOStatus007.Location = new System.Drawing.Point(14, 190);
            this.picIOStatus007.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.picIOStatus007.Name = "picIOStatus007";
            this.picIOStatus007.Size = new System.Drawing.Size(53, 27);
            this.picIOStatus007.TabIndex = 57;
            this.picIOStatus007.TabStop = false;
            // 
            // lblIOName007
            // 
            this.lblIOName007.AutoSize = true;
            this.lblIOName007.Location = new System.Drawing.Point(73, 193);
            this.lblIOName007.Name = "lblIOName007";
            this.lblIOName007.Size = new System.Drawing.Size(326, 16);
            this.lblIOName007.TabIndex = 56;
            this.lblIOName007.Text = "Speedpin fastening station transfer X1 axis home sensor";
            // 
            // picIOStatus008
            // 
            this.picIOStatus008.Location = new System.Drawing.Point(14, 219);
            this.picIOStatus008.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.picIOStatus008.Name = "picIOStatus008";
            this.picIOStatus008.Size = new System.Drawing.Size(53, 27);
            this.picIOStatus008.TabIndex = 57;
            this.picIOStatus008.TabStop = false;
            // 
            // lblIOName008
            // 
            this.lblIOName008.AutoSize = true;
            this.lblIOName008.Location = new System.Drawing.Point(73, 222);
            this.lblIOName008.Name = "lblIOName008";
            this.lblIOName008.Size = new System.Drawing.Size(326, 16);
            this.lblIOName008.TabIndex = 56;
            this.lblIOName008.Text = "Speedpin fastening station transfer X1 axis home sensor";
            // 
            // picIOStatus009
            // 
            this.picIOStatus009.Location = new System.Drawing.Point(14, 247);
            this.picIOStatus009.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.picIOStatus009.Name = "picIOStatus009";
            this.picIOStatus009.Size = new System.Drawing.Size(53, 27);
            this.picIOStatus009.TabIndex = 57;
            this.picIOStatus009.TabStop = false;
            // 
            // lblIOName009
            // 
            this.lblIOName009.AutoSize = true;
            this.lblIOName009.Location = new System.Drawing.Point(73, 251);
            this.lblIOName009.Name = "lblIOName009";
            this.lblIOName009.Size = new System.Drawing.Size(326, 16);
            this.lblIOName009.TabIndex = 56;
            this.lblIOName009.Text = "Speedpin fastening station transfer X1 axis home sensor";
            // 
            // picIOStatus010
            // 
            this.picIOStatus010.Location = new System.Drawing.Point(14, 276);
            this.picIOStatus010.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.picIOStatus010.Name = "picIOStatus010";
            this.picIOStatus010.Size = new System.Drawing.Size(53, 27);
            this.picIOStatus010.TabIndex = 57;
            this.picIOStatus010.TabStop = false;
            // 
            // lblIOName010
            // 
            this.lblIOName010.AutoSize = true;
            this.lblIOName010.Location = new System.Drawing.Point(73, 279);
            this.lblIOName010.Name = "lblIOName010";
            this.lblIOName010.Size = new System.Drawing.Size(326, 16);
            this.lblIOName010.TabIndex = 56;
            this.lblIOName010.Text = "Speedpin fastening station transfer X1 axis home sensor";
            // 
            // picIOStatus011
            // 
            this.picIOStatus011.Location = new System.Drawing.Point(14, 305);
            this.picIOStatus011.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.picIOStatus011.Name = "picIOStatus011";
            this.picIOStatus011.Size = new System.Drawing.Size(53, 27);
            this.picIOStatus011.TabIndex = 57;
            this.picIOStatus011.TabStop = false;
            // 
            // lblIOName011
            // 
            this.lblIOName011.AutoSize = true;
            this.lblIOName011.Location = new System.Drawing.Point(73, 308);
            this.lblIOName011.Name = "lblIOName011";
            this.lblIOName011.Size = new System.Drawing.Size(326, 16);
            this.lblIOName011.TabIndex = 56;
            this.lblIOName011.Text = "Speedpin fastening station transfer X1 axis home sensor";
            // 
            // picIOStatus012
            // 
            this.picIOStatus012.Location = new System.Drawing.Point(14, 334);
            this.picIOStatus012.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.picIOStatus012.Name = "picIOStatus012";
            this.picIOStatus012.Size = new System.Drawing.Size(53, 27);
            this.picIOStatus012.TabIndex = 57;
            this.picIOStatus012.TabStop = false;
            // 
            // lblIOName012
            // 
            this.lblIOName012.AutoSize = true;
            this.lblIOName012.Location = new System.Drawing.Point(73, 337);
            this.lblIOName012.Name = "lblIOName012";
            this.lblIOName012.Size = new System.Drawing.Size(326, 16);
            this.lblIOName012.TabIndex = 56;
            this.lblIOName012.Text = "Speedpin fastening station transfer X1 axis home sensor";
            // 
            // picIOStatus013
            // 
            this.picIOStatus013.Location = new System.Drawing.Point(14, 363);
            this.picIOStatus013.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.picIOStatus013.Name = "picIOStatus013";
            this.picIOStatus013.Size = new System.Drawing.Size(53, 27);
            this.picIOStatus013.TabIndex = 57;
            this.picIOStatus013.TabStop = false;
            // 
            // lblIOName013
            // 
            this.lblIOName013.AutoSize = true;
            this.lblIOName013.Location = new System.Drawing.Point(73, 366);
            this.lblIOName013.Name = "lblIOName013";
            this.lblIOName013.Size = new System.Drawing.Size(326, 16);
            this.lblIOName013.TabIndex = 56;
            this.lblIOName013.Text = "Speedpin fastening station transfer X1 axis home sensor";
            // 
            // picIOStatus014
            // 
            this.picIOStatus014.Location = new System.Drawing.Point(14, 391);
            this.picIOStatus014.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.picIOStatus014.Name = "picIOStatus014";
            this.picIOStatus014.Size = new System.Drawing.Size(53, 27);
            this.picIOStatus014.TabIndex = 57;
            this.picIOStatus014.TabStop = false;
            // 
            // lblIOName014
            // 
            this.lblIOName014.AutoSize = true;
            this.lblIOName014.Location = new System.Drawing.Point(73, 395);
            this.lblIOName014.Name = "lblIOName014";
            this.lblIOName014.Size = new System.Drawing.Size(326, 16);
            this.lblIOName014.TabIndex = 56;
            this.lblIOName014.Text = "Speedpin fastening station transfer X1 axis home sensor";
            // 
            // picIOStatus015
            // 
            this.picIOStatus015.Location = new System.Drawing.Point(14, 420);
            this.picIOStatus015.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.picIOStatus015.Name = "picIOStatus015";
            this.picIOStatus015.Size = new System.Drawing.Size(53, 27);
            this.picIOStatus015.TabIndex = 57;
            this.picIOStatus015.TabStop = false;
            // 
            // lblIOName015
            // 
            this.lblIOName015.AutoSize = true;
            this.lblIOName015.Location = new System.Drawing.Point(73, 423);
            this.lblIOName015.Name = "lblIOName015";
            this.lblIOName015.Size = new System.Drawing.Size(326, 16);
            this.lblIOName015.TabIndex = 56;
            this.lblIOName015.Text = "Speedpin fastening station transfer X1 axis home sensor";
            // 
            // picIOStatus016
            // 
            this.picIOStatus016.Location = new System.Drawing.Point(14, 449);
            this.picIOStatus016.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.picIOStatus016.Name = "picIOStatus016";
            this.picIOStatus016.Size = new System.Drawing.Size(53, 27);
            this.picIOStatus016.TabIndex = 57;
            this.picIOStatus016.TabStop = false;
            // 
            // lblIOName016
            // 
            this.lblIOName016.AutoSize = true;
            this.lblIOName016.Location = new System.Drawing.Point(73, 452);
            this.lblIOName016.Name = "lblIOName016";
            this.lblIOName016.Size = new System.Drawing.Size(326, 16);
            this.lblIOName016.TabIndex = 56;
            this.lblIOName016.Text = "Speedpin fastening station transfer X1 axis home sensor";
            // 
            // picIOStatus017
            // 
            this.picIOStatus017.Location = new System.Drawing.Point(529, 17);
            this.picIOStatus017.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.picIOStatus017.Name = "picIOStatus017";
            this.picIOStatus017.Size = new System.Drawing.Size(53, 27);
            this.picIOStatus017.TabIndex = 57;
            this.picIOStatus017.TabStop = false;
            // 
            // lblIOName017
            // 
            this.lblIOName017.AutoSize = true;
            this.lblIOName017.Location = new System.Drawing.Point(589, 20);
            this.lblIOName017.Name = "lblIOName017";
            this.lblIOName017.Size = new System.Drawing.Size(326, 16);
            this.lblIOName017.TabIndex = 56;
            this.lblIOName017.Text = "Speedpin fastening station transfer X1 axis home sensor";
            // 
            // picIOStatus018
            // 
            this.picIOStatus018.Location = new System.Drawing.Point(529, 46);
            this.picIOStatus018.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.picIOStatus018.Name = "picIOStatus018";
            this.picIOStatus018.Size = new System.Drawing.Size(53, 27);
            this.picIOStatus018.TabIndex = 57;
            this.picIOStatus018.TabStop = false;
            // 
            // lblIOName018
            // 
            this.lblIOName018.AutoSize = true;
            this.lblIOName018.Location = new System.Drawing.Point(589, 49);
            this.lblIOName018.Name = "lblIOName018";
            this.lblIOName018.Size = new System.Drawing.Size(326, 16);
            this.lblIOName018.TabIndex = 56;
            this.lblIOName018.Text = "Speedpin fastening station transfer X1 axis home sensor";
            // 
            // picIOStatus019
            // 
            this.picIOStatus019.Location = new System.Drawing.Point(529, 75);
            this.picIOStatus019.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.picIOStatus019.Name = "picIOStatus019";
            this.picIOStatus019.Size = new System.Drawing.Size(53, 27);
            this.picIOStatus019.TabIndex = 57;
            this.picIOStatus019.TabStop = false;
            // 
            // lblIOName019
            // 
            this.lblIOName019.AutoSize = true;
            this.lblIOName019.Location = new System.Drawing.Point(589, 78);
            this.lblIOName019.Name = "lblIOName019";
            this.lblIOName019.Size = new System.Drawing.Size(326, 16);
            this.lblIOName019.TabIndex = 56;
            this.lblIOName019.Text = "Speedpin fastening station transfer X1 axis home sensor";
            // 
            // picIOStatus020
            // 
            this.picIOStatus020.Location = new System.Drawing.Point(529, 103);
            this.picIOStatus020.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.picIOStatus020.Name = "picIOStatus020";
            this.picIOStatus020.Size = new System.Drawing.Size(53, 27);
            this.picIOStatus020.TabIndex = 57;
            this.picIOStatus020.TabStop = false;
            // 
            // lblIOName020
            // 
            this.lblIOName020.AutoSize = true;
            this.lblIOName020.Location = new System.Drawing.Point(589, 107);
            this.lblIOName020.Name = "lblIOName020";
            this.lblIOName020.Size = new System.Drawing.Size(326, 16);
            this.lblIOName020.TabIndex = 56;
            this.lblIOName020.Text = "Speedpin fastening station transfer X1 axis home sensor";
            // 
            // picIOStatus021
            // 
            this.picIOStatus021.Location = new System.Drawing.Point(529, 132);
            this.picIOStatus021.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.picIOStatus021.Name = "picIOStatus021";
            this.picIOStatus021.Size = new System.Drawing.Size(53, 27);
            this.picIOStatus021.TabIndex = 57;
            this.picIOStatus021.TabStop = false;
            // 
            // lblIOName021
            // 
            this.lblIOName021.AutoSize = true;
            this.lblIOName021.Location = new System.Drawing.Point(589, 135);
            this.lblIOName021.Name = "lblIOName021";
            this.lblIOName021.Size = new System.Drawing.Size(326, 16);
            this.lblIOName021.TabIndex = 56;
            this.lblIOName021.Text = "Speedpin fastening station transfer X1 axis home sensor";
            // 
            // picIOStatus022
            // 
            this.picIOStatus022.Location = new System.Drawing.Point(529, 161);
            this.picIOStatus022.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.picIOStatus022.Name = "picIOStatus022";
            this.picIOStatus022.Size = new System.Drawing.Size(53, 27);
            this.picIOStatus022.TabIndex = 57;
            this.picIOStatus022.TabStop = false;
            // 
            // lblIOName022
            // 
            this.lblIOName022.AutoSize = true;
            this.lblIOName022.Location = new System.Drawing.Point(589, 164);
            this.lblIOName022.Name = "lblIOName022";
            this.lblIOName022.Size = new System.Drawing.Size(326, 16);
            this.lblIOName022.TabIndex = 56;
            this.lblIOName022.Text = "Speedpin fastening station transfer X1 axis home sensor";
            // 
            // picIOStatus023
            // 
            this.picIOStatus023.Location = new System.Drawing.Point(529, 190);
            this.picIOStatus023.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.picIOStatus023.Name = "picIOStatus023";
            this.picIOStatus023.Size = new System.Drawing.Size(53, 27);
            this.picIOStatus023.TabIndex = 57;
            this.picIOStatus023.TabStop = false;
            // 
            // lblIOName023
            // 
            this.lblIOName023.AutoSize = true;
            this.lblIOName023.Location = new System.Drawing.Point(589, 193);
            this.lblIOName023.Name = "lblIOName023";
            this.lblIOName023.Size = new System.Drawing.Size(326, 16);
            this.lblIOName023.TabIndex = 56;
            this.lblIOName023.Text = "Speedpin fastening station transfer X1 axis home sensor";
            // 
            // picIOStatus024
            // 
            this.picIOStatus024.Location = new System.Drawing.Point(529, 219);
            this.picIOStatus024.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.picIOStatus024.Name = "picIOStatus024";
            this.picIOStatus024.Size = new System.Drawing.Size(53, 27);
            this.picIOStatus024.TabIndex = 57;
            this.picIOStatus024.TabStop = false;
            // 
            // picIOStatus025
            // 
            this.picIOStatus025.Location = new System.Drawing.Point(529, 247);
            this.picIOStatus025.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.picIOStatus025.Name = "picIOStatus025";
            this.picIOStatus025.Size = new System.Drawing.Size(53, 27);
            this.picIOStatus025.TabIndex = 57;
            this.picIOStatus025.TabStop = false;
            // 
            // lblIOName024
            // 
            this.lblIOName024.AutoSize = true;
            this.lblIOName024.Location = new System.Drawing.Point(589, 222);
            this.lblIOName024.Name = "lblIOName024";
            this.lblIOName024.Size = new System.Drawing.Size(326, 16);
            this.lblIOName024.TabIndex = 56;
            this.lblIOName024.Text = "Speedpin fastening station transfer X1 axis home sensor";
            // 
            // picIOStatus026
            // 
            this.picIOStatus026.Location = new System.Drawing.Point(529, 276);
            this.picIOStatus026.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.picIOStatus026.Name = "picIOStatus026";
            this.picIOStatus026.Size = new System.Drawing.Size(53, 27);
            this.picIOStatus026.TabIndex = 57;
            this.picIOStatus026.TabStop = false;
            // 
            // lblIOName025
            // 
            this.lblIOName025.AutoSize = true;
            this.lblIOName025.Location = new System.Drawing.Point(589, 251);
            this.lblIOName025.Name = "lblIOName025";
            this.lblIOName025.Size = new System.Drawing.Size(326, 16);
            this.lblIOName025.TabIndex = 56;
            this.lblIOName025.Text = "Speedpin fastening station transfer X1 axis home sensor";
            // 
            // picIOStatus027
            // 
            this.picIOStatus027.Location = new System.Drawing.Point(529, 305);
            this.picIOStatus027.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.picIOStatus027.Name = "picIOStatus027";
            this.picIOStatus027.Size = new System.Drawing.Size(53, 27);
            this.picIOStatus027.TabIndex = 57;
            this.picIOStatus027.TabStop = false;
            // 
            // lblIOName026
            // 
            this.lblIOName026.AutoSize = true;
            this.lblIOName026.Location = new System.Drawing.Point(589, 279);
            this.lblIOName026.Name = "lblIOName026";
            this.lblIOName026.Size = new System.Drawing.Size(326, 16);
            this.lblIOName026.TabIndex = 56;
            this.lblIOName026.Text = "Speedpin fastening station transfer X1 axis home sensor";
            // 
            // picIOStatus028
            // 
            this.picIOStatus028.Location = new System.Drawing.Point(529, 334);
            this.picIOStatus028.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.picIOStatus028.Name = "picIOStatus028";
            this.picIOStatus028.Size = new System.Drawing.Size(53, 27);
            this.picIOStatus028.TabIndex = 57;
            this.picIOStatus028.TabStop = false;
            // 
            // lblIOName027
            // 
            this.lblIOName027.AutoSize = true;
            this.lblIOName027.Location = new System.Drawing.Point(589, 308);
            this.lblIOName027.Name = "lblIOName027";
            this.lblIOName027.Size = new System.Drawing.Size(326, 16);
            this.lblIOName027.TabIndex = 56;
            this.lblIOName027.Text = "Speedpin fastening station transfer X1 axis home sensor";
            // 
            // picIOStatus029
            // 
            this.picIOStatus029.Location = new System.Drawing.Point(529, 363);
            this.picIOStatus029.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.picIOStatus029.Name = "picIOStatus029";
            this.picIOStatus029.Size = new System.Drawing.Size(53, 27);
            this.picIOStatus029.TabIndex = 57;
            this.picIOStatus029.TabStop = false;
            // 
            // picIOStatus030
            // 
            this.picIOStatus030.Location = new System.Drawing.Point(529, 391);
            this.picIOStatus030.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.picIOStatus030.Name = "picIOStatus030";
            this.picIOStatus030.Size = new System.Drawing.Size(53, 27);
            this.picIOStatus030.TabIndex = 57;
            this.picIOStatus030.TabStop = false;
            // 
            // lblIOName028
            // 
            this.lblIOName028.AutoSize = true;
            this.lblIOName028.Location = new System.Drawing.Point(589, 337);
            this.lblIOName028.Name = "lblIOName028";
            this.lblIOName028.Size = new System.Drawing.Size(326, 16);
            this.lblIOName028.TabIndex = 56;
            this.lblIOName028.Text = "Speedpin fastening station transfer X1 axis home sensor";
            // 
            // picIOStatus031
            // 
            this.picIOStatus031.Location = new System.Drawing.Point(529, 420);
            this.picIOStatus031.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.picIOStatus031.Name = "picIOStatus031";
            this.picIOStatus031.Size = new System.Drawing.Size(53, 27);
            this.picIOStatus031.TabIndex = 57;
            this.picIOStatus031.TabStop = false;
            // 
            // lblIOName029
            // 
            this.lblIOName029.AutoSize = true;
            this.lblIOName029.Location = new System.Drawing.Point(589, 366);
            this.lblIOName029.Name = "lblIOName029";
            this.lblIOName029.Size = new System.Drawing.Size(326, 16);
            this.lblIOName029.TabIndex = 56;
            this.lblIOName029.Text = "Speedpin fastening station transfer X1 axis home sensor";
            // 
            // picIOStatus032
            // 
            this.picIOStatus032.Location = new System.Drawing.Point(529, 449);
            this.picIOStatus032.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.picIOStatus032.Name = "picIOStatus032";
            this.picIOStatus032.Size = new System.Drawing.Size(53, 27);
            this.picIOStatus032.TabIndex = 57;
            this.picIOStatus032.TabStop = false;
            // 
            // lblIOName030
            // 
            this.lblIOName030.AutoSize = true;
            this.lblIOName030.Location = new System.Drawing.Point(589, 395);
            this.lblIOName030.Name = "lblIOName030";
            this.lblIOName030.Size = new System.Drawing.Size(326, 16);
            this.lblIOName030.TabIndex = 56;
            this.lblIOName030.Text = "Speedpin fastening station transfer X1 axis home sensor";
            // 
            // lblIOName031
            // 
            this.lblIOName031.AutoSize = true;
            this.lblIOName031.Location = new System.Drawing.Point(589, 423);
            this.lblIOName031.Name = "lblIOName031";
            this.lblIOName031.Size = new System.Drawing.Size(326, 16);
            this.lblIOName031.TabIndex = 56;
            this.lblIOName031.Text = "Speedpin fastening station transfer X1 axis home sensor";
            // 
            // lblIOName032
            // 
            this.lblIOName032.AutoSize = true;
            this.lblIOName032.Location = new System.Drawing.Point(589, 452);
            this.lblIOName032.Name = "lblIOName032";
            this.lblIOName032.Size = new System.Drawing.Size(326, 16);
            this.lblIOName032.TabIndex = 56;
            this.lblIOName032.Text = "Speedpin fastening station transfer X1 axis home sensor";
            // 
            // btnInput
            // 
            this.btnInput.BackColor = System.Drawing.SystemColors.ControlDark;
            this.btnInput.Font = new System.Drawing.Font("Times New Roman", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnInput.Location = new System.Drawing.Point(13, 484);
            this.btnInput.Name = "btnInput";
            this.btnInput.Size = new System.Drawing.Size(94, 54);
            this.btnInput.TabIndex = 109;
            this.btnInput.Text = "输入";
            this.btnInput.UseVisualStyleBackColor = false;
            this.btnInput.Click += new System.EventHandler(this.SelectInputFormEvent);
            // 
            // btnOutput
            // 
            this.btnOutput.BackColor = System.Drawing.SystemColors.ControlDark;
            this.btnOutput.Font = new System.Drawing.Font("Times New Roman", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOutput.Location = new System.Drawing.Point(113, 484);
            this.btnOutput.Name = "btnOutput";
            this.btnOutput.Size = new System.Drawing.Size(94, 54);
            this.btnOutput.TabIndex = 110;
            this.btnOutput.Text = "输出";
            this.btnOutput.UseVisualStyleBackColor = false;
            this.btnOutput.Click += new System.EventHandler(this.SelectOutputFormEvent);
            // 
            // btnNextPage
            // 
            this.btnNextPage.BackColor = System.Drawing.Color.LimeGreen;
            this.btnNextPage.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNextPage.ForeColor = System.Drawing.Color.Black;
            this.btnNextPage.Location = new System.Drawing.Point(929, 484);
            this.btnNextPage.Name = "btnNextPage";
            this.btnNextPage.Size = new System.Drawing.Size(94, 54);
            this.btnNextPage.TabIndex = 259;
            this.btnNextPage.Tag = "";
            this.btnNextPage.Text = "下一页";
            this.btnNextPage.UseVisualStyleBackColor = false;
            this.btnNextPage.Click += new System.EventHandler(this.btnNextPage_Click);
            // 
            // btnPrevPage
            // 
            this.btnPrevPage.BackColor = System.Drawing.Color.LimeGreen;
            this.btnPrevPage.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPrevPage.ForeColor = System.Drawing.Color.Black;
            this.btnPrevPage.Location = new System.Drawing.Point(829, 484);
            this.btnPrevPage.Name = "btnPrevPage";
            this.btnPrevPage.Size = new System.Drawing.Size(94, 54);
            this.btnPrevPage.TabIndex = 258;
            this.btnPrevPage.Tag = "";
            this.btnPrevPage.Text = "上一页";
            this.btnPrevPage.UseVisualStyleBackColor = false;
            this.btnPrevPage.Click += new System.EventHandler(this.btnPrevPage_Click);
            // 
            // lblCurrentPage
            // 
            this.lblCurrentPage.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCurrentPage.Location = new System.Drawing.Point(592, 495);
            this.lblCurrentPage.Name = "lblCurrentPage";
            this.lblCurrentPage.Size = new System.Drawing.Size(230, 36);
            this.lblCurrentPage.TabIndex = 260;
            this.lblCurrentPage.Text = "当前IO页面：1/1";
            this.lblCurrentPage.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // MotionCardIOForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1034, 546);
            this.Controls.Add(this.lblCurrentPage);
            this.Controls.Add(this.btnNextPage);
            this.Controls.Add(this.btnPrevPage);
            this.Controls.Add(this.btnInput);
            this.Controls.Add(this.btnOutput);
            this.Controls.Add(this.lblIOName032);
            this.Controls.Add(this.lblIOName016);
            this.Controls.Add(this.lblIOName031);
            this.Controls.Add(this.lblIOName015);
            this.Controls.Add(this.lblIOName030);
            this.Controls.Add(this.lblIOName014);
            this.Controls.Add(this.picIOStatus032);
            this.Controls.Add(this.picIOStatus016);
            this.Controls.Add(this.lblIOName029);
            this.Controls.Add(this.lblIOName013);
            this.Controls.Add(this.picIOStatus031);
            this.Controls.Add(this.picIOStatus015);
            this.Controls.Add(this.lblIOName028);
            this.Controls.Add(this.lblIOName012);
            this.Controls.Add(this.picIOStatus030);
            this.Controls.Add(this.picIOStatus014);
            this.Controls.Add(this.picIOStatus029);
            this.Controls.Add(this.picIOStatus013);
            this.Controls.Add(this.lblIOName027);
            this.Controls.Add(this.lblIOName011);
            this.Controls.Add(this.picIOStatus028);
            this.Controls.Add(this.picIOStatus012);
            this.Controls.Add(this.lblIOName026);
            this.Controls.Add(this.lblIOName010);
            this.Controls.Add(this.picIOStatus027);
            this.Controls.Add(this.picIOStatus011);
            this.Controls.Add(this.lblIOName025);
            this.Controls.Add(this.lblIOName009);
            this.Controls.Add(this.picIOStatus026);
            this.Controls.Add(this.picIOStatus010);
            this.Controls.Add(this.lblIOName024);
            this.Controls.Add(this.lblIOName008);
            this.Controls.Add(this.picIOStatus025);
            this.Controls.Add(this.picIOStatus009);
            this.Controls.Add(this.picIOStatus024);
            this.Controls.Add(this.picIOStatus008);
            this.Controls.Add(this.lblIOName023);
            this.Controls.Add(this.lblIOName007);
            this.Controls.Add(this.picIOStatus023);
            this.Controls.Add(this.picIOStatus007);
            this.Controls.Add(this.lblIOName022);
            this.Controls.Add(this.lblIOName006);
            this.Controls.Add(this.picIOStatus022);
            this.Controls.Add(this.picIOStatus006);
            this.Controls.Add(this.lblIOName021);
            this.Controls.Add(this.lblIOName005);
            this.Controls.Add(this.picIOStatus021);
            this.Controls.Add(this.picIOStatus005);
            this.Controls.Add(this.lblIOName020);
            this.Controls.Add(this.lblIOName004);
            this.Controls.Add(this.picIOStatus020);
            this.Controls.Add(this.picIOStatus004);
            this.Controls.Add(this.lblIOName019);
            this.Controls.Add(this.lblIOName003);
            this.Controls.Add(this.picIOStatus019);
            this.Controls.Add(this.picIOStatus003);
            this.Controls.Add(this.lblIOName018);
            this.Controls.Add(this.lblIOName002);
            this.Controls.Add(this.picIOStatus018);
            this.Controls.Add(this.picIOStatus002);
            this.Controls.Add(this.lblIOName017);
            this.Controls.Add(this.lblIOName001);
            this.Controls.Add(this.picIOStatus017);
            this.Controls.Add(this.picIOStatus001);
            this.Font = new System.Drawing.Font("Times New Roman", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "MotionCardIOForm";
            this.Text = "MotionCardIOForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MotionCardIOForm_FormClosing);
            this.Load += new System.EventHandler(this.MotionCardIOForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.picIOStatus001)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picIOStatus002)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picIOStatus003)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picIOStatus004)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picIOStatus005)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picIOStatus006)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picIOStatus007)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picIOStatus008)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picIOStatus009)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picIOStatus010)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picIOStatus011)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picIOStatus012)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picIOStatus013)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picIOStatus014)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picIOStatus015)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picIOStatus016)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picIOStatus017)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picIOStatus018)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picIOStatus019)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picIOStatus020)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picIOStatus021)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picIOStatus022)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picIOStatus023)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picIOStatus024)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picIOStatus025)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picIOStatus026)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picIOStatus027)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picIOStatus028)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picIOStatus029)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picIOStatus030)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picIOStatus031)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picIOStatus032)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblIOName001;
        private System.Windows.Forms.PictureBox picIOStatus001;
        private System.Windows.Forms.PictureBox picIOStatus002;
        private System.Windows.Forms.Label lblIOName002;
        private System.Windows.Forms.PictureBox picIOStatus003;
        private System.Windows.Forms.Label lblIOName003;
        private System.Windows.Forms.PictureBox picIOStatus004;
        private System.Windows.Forms.Label lblIOName004;
        private System.Windows.Forms.PictureBox picIOStatus005;
        private System.Windows.Forms.Label lblIOName005;
        private System.Windows.Forms.PictureBox picIOStatus006;
        private System.Windows.Forms.Label lblIOName006;
        private System.Windows.Forms.PictureBox picIOStatus007;
        private System.Windows.Forms.Label lblIOName007;
        private System.Windows.Forms.PictureBox picIOStatus008;
        private System.Windows.Forms.Label lblIOName008;
        private System.Windows.Forms.PictureBox picIOStatus009;
        private System.Windows.Forms.Label lblIOName009;
        private System.Windows.Forms.PictureBox picIOStatus010;
        private System.Windows.Forms.Label lblIOName010;
        private System.Windows.Forms.PictureBox picIOStatus011;
        private System.Windows.Forms.Label lblIOName011;
        private System.Windows.Forms.PictureBox picIOStatus012;
        private System.Windows.Forms.Label lblIOName012;
        private System.Windows.Forms.PictureBox picIOStatus013;
        private System.Windows.Forms.Label lblIOName013;
        private System.Windows.Forms.PictureBox picIOStatus014;
        private System.Windows.Forms.Label lblIOName014;
        private System.Windows.Forms.PictureBox picIOStatus015;
        private System.Windows.Forms.Label lblIOName015;
        private System.Windows.Forms.PictureBox picIOStatus016;
        private System.Windows.Forms.Label lblIOName016;
        private System.Windows.Forms.PictureBox picIOStatus017;
        private System.Windows.Forms.Label lblIOName017;
        private System.Windows.Forms.PictureBox picIOStatus018;
        private System.Windows.Forms.Label lblIOName018;
        private System.Windows.Forms.PictureBox picIOStatus019;
        private System.Windows.Forms.Label lblIOName019;
        private System.Windows.Forms.PictureBox picIOStatus020;
        private System.Windows.Forms.Label lblIOName020;
        private System.Windows.Forms.PictureBox picIOStatus021;
        private System.Windows.Forms.Label lblIOName021;
        private System.Windows.Forms.PictureBox picIOStatus022;
        private System.Windows.Forms.Label lblIOName022;
        private System.Windows.Forms.PictureBox picIOStatus023;
        private System.Windows.Forms.Label lblIOName023;
        private System.Windows.Forms.PictureBox picIOStatus024;
        private System.Windows.Forms.PictureBox picIOStatus025;
        private System.Windows.Forms.Label lblIOName024;
        private System.Windows.Forms.PictureBox picIOStatus026;
        private System.Windows.Forms.Label lblIOName025;
        private System.Windows.Forms.PictureBox picIOStatus027;
        private System.Windows.Forms.Label lblIOName026;
        private System.Windows.Forms.PictureBox picIOStatus028;
        private System.Windows.Forms.Label lblIOName027;
        private System.Windows.Forms.PictureBox picIOStatus029;
        private System.Windows.Forms.PictureBox picIOStatus030;
        private System.Windows.Forms.Label lblIOName028;
        private System.Windows.Forms.PictureBox picIOStatus031;
        private System.Windows.Forms.Label lblIOName029;
        private System.Windows.Forms.PictureBox picIOStatus032;
        private System.Windows.Forms.Label lblIOName030;
        private System.Windows.Forms.Label lblIOName031;
        private System.Windows.Forms.Label lblIOName032;
        private System.Windows.Forms.Button btnInput;
        private System.Windows.Forms.Button btnOutput;
        private System.Windows.Forms.Button btnNextPage;
        private System.Windows.Forms.Button btnPrevPage;
        private System.Windows.Forms.Label lblCurrentPage;
    }
}