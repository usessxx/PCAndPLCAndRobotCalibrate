using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using HalconDotNet;

namespace HalconMVTec
{
    public partial class FitCircleAdjust : Form
    {
        public static int LanguageFlag;//用于设定当前语言，0-英文，1-中文

        public HalconMVTec CircleInspectionAdjustDisplay;
        public HObject ho_Image_CircleInspectionAdjustDisplay;

        public int circlesCounterFlag = 0;//0,1:只有圆1;2:有圆1圆2

        public int stepFlag_CircleInspectionSample = 0;
        public bool windowRefreshFlag = false;

        public double scale_image_numeric1;
        public double scale_image_numeric2;
        public double threshold_numeric1;
        public double threshold_numeric2;
        public double select_shape_numeric1;
        public double select_shape_numeric2;
        public double edges_sub_pix_numeric1;
        public double edges_sub_pix_numeric2;
        public double dilation_circle_numeri1;
        public double union_cocircular_contours_xld_numeric1;
        public double union_cocircular_contours_xld_numeric2;
        public double union_cocircular_contours_xld_numeric3;
        public double union_cocircular_contours_xld_numeric4;
        public double union_cocircular_contours_xld_numeric5;
        public double union_cocircular_contours_xld_numeric6;

        public double scale_image_numeric1_2th;
        public double scale_image_numeric2_2th;
        public double threshold_numeric1_2th;
        public double threshold_numeric2_2th;
        public double select_shape_numeric1_2th;
        public double select_shape_numeric2_2th;
        public double edges_sub_pix_numeric1_2th;
        public double edges_sub_pix_numeric2_2th;
        public double dilation_circle_numeri1_2th;
        public double union_cocircular_contours_xld_numeric1_2th;
        public double union_cocircular_contours_xld_numeric2_2th;
        public double union_cocircular_contours_xld_numeric3_2th;
        public double union_cocircular_contours_xld_numeric4_2th;
        public double union_cocircular_contours_xld_numeric5_2th;
        public double union_cocircular_contours_xld_numeric6_2th;

        public int HalconOperatorValueChangedFlag = 0;//0:NA,1:算子参数有修改;2:图像处理完成 

        public Thread td;//设定为PUBLIC:在CameraAdjust.cs里dispose此窗口时,先td.Abort();

        private bool displayFlag1 = false;
        private bool displayFlag2 = false;
        private bool displayFlag3 = false;
        private bool displayFlag4 = false;
        private bool displayFlag5 = false;
        private bool displayFlag6 = false;

        private HTuple WindowHandle1, WindowHandle2, WindowHandle3, WindowHandle4, WindowHandle5, WindowHandle6;
        private HTuple WindowHandle1_2th, WindowHandle2_2th, WindowHandle3_2th, WindowHandle4_2th, WindowHandle5_2th, WindowHandle6_2th;

        private HTuple hv_Row1, hv_Column1, hv_Row2, hv_Column2;
        private HTuple hv_Row1_2th, hv_Column1_2th, hv_Row2_2th, hv_Column2_2th;

        private HTuple hv_Information = new HTuple();

        public FitCircleAdjust()
        {
            InitializeComponent();
            this.Width = 1440;
            this.Height = 662;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            numericUpDown1.ValueChanged += new EventHandler(HOValueChanged);
            numericUpDown2.ValueChanged += new EventHandler(HOValueChanged);
            numericUpDown3.ValueChanged += new EventHandler(HOValueChanged);
            numericUpDown4.ValueChanged += new EventHandler(HOValueChanged);
            numericUpDown5.ValueChanged += new EventHandler(HOValueChanged);
            numericUpDown6.ValueChanged += new EventHandler(HOValueChanged);
            numericUpDown7.ValueChanged += new EventHandler(HOValueChanged);
            numericUpDown8.ValueChanged += new EventHandler(HOValueChanged);
            numericUpDown9.ValueChanged += new EventHandler(HOValueChanged);
            numericUpDown10.ValueChanged += new EventHandler(HOValueChanged);
            numericUpDown11.ValueChanged += new EventHandler(HOValueChanged);
            numericUpDown12.ValueChanged += new EventHandler(HOValueChanged);
            numericUpDown13.ValueChanged += new EventHandler(HOValueChanged);
            numericUpDown14.ValueChanged += new EventHandler(HOValueChanged);
            numericUpDown15.ValueChanged += new EventHandler(HOValueChanged);

            numericUpDown16.ValueChanged += new EventHandler(HOValueChanged);
            numericUpDown17.ValueChanged += new EventHandler(HOValueChanged);
            numericUpDown18.ValueChanged += new EventHandler(HOValueChanged);
            numericUpDown19.ValueChanged += new EventHandler(HOValueChanged);
            numericUpDown20.ValueChanged += new EventHandler(HOValueChanged);
            numericUpDown21.ValueChanged += new EventHandler(HOValueChanged);
            numericUpDown22.ValueChanged += new EventHandler(HOValueChanged);
            numericUpDown23.ValueChanged += new EventHandler(HOValueChanged);
            numericUpDown24.ValueChanged += new EventHandler(HOValueChanged);
            numericUpDown25.ValueChanged += new EventHandler(HOValueChanged);
            numericUpDown26.ValueChanged += new EventHandler(HOValueChanged);
            numericUpDown27.ValueChanged += new EventHandler(HOValueChanged);
            numericUpDown28.ValueChanged += new EventHandler(HOValueChanged);
            numericUpDown29.ValueChanged += new EventHandler(HOValueChanged);
            numericUpDown30.ValueChanged += new EventHandler(HOValueChanged);

            System.Windows.Forms.Timer LanguageChangeTimer = new System.Windows.Forms.Timer();
            LanguageChangeTimer.Interval = 100;
            LanguageChangeTimer.Tick += new EventHandler(LanguageChange);
            LanguageChangeTimer.Start();
        }

        private void LanguageChange(object sender, EventArgs e)
        { 
            if(LanguageFlag==0)
            {
                button1.Text = "Load function parameter";
                button2.Text = "Save function parameter";
                button1.Font = new Font("Times New Roman", button1.Font.Size, button1.Font.Style);
                button2.Font = new Font("Times New Roman", button2.Font.Size, button2.Font.Style);
            }
            else
            {
                button1.Text = "读取算子参数";
                button2.Text = "保存算子参数";
                button1.Font = new Font("宋体", button1.Font.Size, button1.Font.Style);
                button2.Font = new Font("宋体", button2.Font.Size, button2.Font.Style);
            }
        }

        private void HOValueChanged(object sender, System.EventArgs e)
        {
            HalconOperatorValueChangedFlag = 1;
        }

        private void FitCircleAdjust_Load(object sender, EventArgs e)
        {
            ho_Image_CircleInspectionAdjustDisplay = CircleInspectionAdjustDisplay.ho_Image_CircleInspectionSample;

            HTuple HOPar = HRead(CircleInspectionAdjustDisplay.shapeModelImageFileDirectionPath_CircleInspectionSample + CircleInspectionAdjustDisplay.shapeModelImageFileName_CircleInspectionSample + ".smddd");
            //if (HOPar.Length == 18)
            if (HOPar.Length == 18 || HOPar.Length == 36)
            {
                numericUpDown1.Value = Convert.ToDecimal(HOPar.TupleSelect(3).ToString());
                numericUpDown2.Value = Convert.ToDecimal(HOPar.TupleSelect(4).ToString());
                numericUpDown3.Value = Convert.ToDecimal(HOPar.TupleSelect(5).ToString());
                numericUpDown4.Value = Convert.ToDecimal(HOPar.TupleSelect(6).ToString());
                numericUpDown5.Value = Convert.ToDecimal(HOPar.TupleSelect(7).ToString());
                numericUpDown6.Value = Convert.ToDecimal(HOPar.TupleSelect(8).ToString());
                numericUpDown7.Value = Convert.ToDecimal(HOPar.TupleSelect(9).ToString());
                numericUpDown8.Value = Convert.ToDecimal(HOPar.TupleSelect(10).ToString());
                numericUpDown9.Value = Convert.ToDecimal(HOPar.TupleSelect(11).ToString());
                numericUpDown10.Value = Convert.ToDecimal(HOPar.TupleSelect(12).ToString());
                numericUpDown11.Value = Convert.ToDecimal(HOPar.TupleSelect(13).ToString());
                numericUpDown12.Value = Convert.ToDecimal(HOPar.TupleSelect(14).ToString());
                numericUpDown13.Value = Convert.ToDecimal(HOPar.TupleSelect(15).ToString());
                numericUpDown14.Value = Convert.ToDecimal(HOPar.TupleSelect(16).ToString());
                numericUpDown15.Value = Convert.ToDecimal(HOPar.TupleSelect(17).ToString());
            }

            if (HOPar.Length == 36)
            {
                numericUpDown16.Value = Convert.ToDecimal(HOPar.TupleSelect(21).ToString());
                numericUpDown17.Value = Convert.ToDecimal(HOPar.TupleSelect(22).ToString());
                numericUpDown18.Value = Convert.ToDecimal(HOPar.TupleSelect(23).ToString());
                numericUpDown19.Value = Convert.ToDecimal(HOPar.TupleSelect(24).ToString());
                numericUpDown20.Value = Convert.ToDecimal(HOPar.TupleSelect(25).ToString());
                numericUpDown21.Value = Convert.ToDecimal(HOPar.TupleSelect(26).ToString());
                numericUpDown22.Value = Convert.ToDecimal(HOPar.TupleSelect(27).ToString());
                numericUpDown23.Value = Convert.ToDecimal(HOPar.TupleSelect(28).ToString());
                numericUpDown24.Value = Convert.ToDecimal(HOPar.TupleSelect(29).ToString());
                numericUpDown25.Value = Convert.ToDecimal(HOPar.TupleSelect(30).ToString());
                numericUpDown26.Value = Convert.ToDecimal(HOPar.TupleSelect(31).ToString());
                numericUpDown27.Value = Convert.ToDecimal(HOPar.TupleSelect(32).ToString());
                numericUpDown28.Value = Convert.ToDecimal(HOPar.TupleSelect(33).ToString());
                numericUpDown29.Value = Convert.ToDecimal(HOPar.TupleSelect(34).ToString());
                numericUpDown30.Value = Convert.ToDecimal(HOPar.TupleSelect(35).ToString());
            }

            //在第一次运行时,让HFitCircle能拿到算子的参数
            scale_image_numeric1 = Convert.ToDouble(numericUpDown1.Value);
            scale_image_numeric2 = Convert.ToDouble(numericUpDown2.Value);
            threshold_numeric1 = Convert.ToDouble(numericUpDown3.Value);
            threshold_numeric2 = Convert.ToDouble(numericUpDown4.Value);
            select_shape_numeric1 = Convert.ToDouble(numericUpDown5.Value);
            select_shape_numeric2 = Convert.ToDouble(numericUpDown6.Value);
            dilation_circle_numeri1 = Convert.ToDouble(numericUpDown7.Value);
            edges_sub_pix_numeric1 = Convert.ToDouble(numericUpDown8.Value);
            edges_sub_pix_numeric2 = Convert.ToDouble(numericUpDown9.Value);
            union_cocircular_contours_xld_numeric1 = Convert.ToDouble(numericUpDown10.Value);
            union_cocircular_contours_xld_numeric2 = Convert.ToDouble(numericUpDown11.Value);
            union_cocircular_contours_xld_numeric3 = Convert.ToDouble(numericUpDown12.Value);
            union_cocircular_contours_xld_numeric4 = Convert.ToDouble(numericUpDown13.Value);
            union_cocircular_contours_xld_numeric5 = Convert.ToDouble(numericUpDown14.Value);
            union_cocircular_contours_xld_numeric6 = Convert.ToDouble(numericUpDown15.Value);

            scale_image_numeric1_2th = Convert.ToDouble(numericUpDown16.Value);
            scale_image_numeric2_2th = Convert.ToDouble(numericUpDown17.Value);
            threshold_numeric1_2th = Convert.ToDouble(numericUpDown18.Value);
            threshold_numeric2_2th = Convert.ToDouble(numericUpDown19.Value);
            select_shape_numeric1_2th = Convert.ToDouble(numericUpDown20.Value);
            select_shape_numeric2_2th = Convert.ToDouble(numericUpDown21.Value);
            dilation_circle_numeri1_2th = Convert.ToDouble(numericUpDown22.Value);
            edges_sub_pix_numeric1_2th = Convert.ToDouble(numericUpDown23.Value);
            edges_sub_pix_numeric2_2th = Convert.ToDouble(numericUpDown24.Value);
            union_cocircular_contours_xld_numeric1_2th = Convert.ToDouble(numericUpDown25.Value);
            union_cocircular_contours_xld_numeric2_2th = Convert.ToDouble(numericUpDown26.Value);
            union_cocircular_contours_xld_numeric3_2th = Convert.ToDouble(numericUpDown27.Value);
            union_cocircular_contours_xld_numeric4_2th = Convert.ToDouble(numericUpDown28.Value);
            union_cocircular_contours_xld_numeric5_2th = Convert.ToDouble(numericUpDown29.Value);
            union_cocircular_contours_xld_numeric6_2th = Convert.ToDouble(numericUpDown30.Value);

            HalconOperatorValueChangedFlag = 1;

            //UI Updata Thread
            td = new Thread(Updata);
            td.IsBackground = true;
            td.Start();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            HTuple HOPar = HRead(CircleInspectionAdjustDisplay.shapeModelImageFileDirectionPath_CircleInspectionSample + CircleInspectionAdjustDisplay.shapeModelImageFileName_CircleInspectionSample + ".smddd");
            numericUpDown1.Value = Convert.ToDecimal(HOPar.TupleSelect(3).ToString());
            numericUpDown2.Value = Convert.ToDecimal(HOPar.TupleSelect(4).ToString());
            numericUpDown3.Value = Convert.ToDecimal(HOPar.TupleSelect(5).ToString());
            numericUpDown4.Value = Convert.ToDecimal(HOPar.TupleSelect(6).ToString());
            numericUpDown5.Value = Convert.ToDecimal(HOPar.TupleSelect(7).ToString());
            numericUpDown6.Value = Convert.ToDecimal(HOPar.TupleSelect(8).ToString());
            numericUpDown7.Value = Convert.ToDecimal(HOPar.TupleSelect(9).ToString());
            numericUpDown8.Value = Convert.ToDecimal(HOPar.TupleSelect(10).ToString());
            numericUpDown9.Value = Convert.ToDecimal(HOPar.TupleSelect(11).ToString());
            numericUpDown10.Value = Convert.ToDecimal(HOPar.TupleSelect(12).ToString());
            numericUpDown11.Value = Convert.ToDecimal(HOPar.TupleSelect(13).ToString());
            numericUpDown12.Value = Convert.ToDecimal(HOPar.TupleSelect(14).ToString());
            numericUpDown13.Value = Convert.ToDecimal(HOPar.TupleSelect(15).ToString());
            numericUpDown14.Value = Convert.ToDecimal(HOPar.TupleSelect(16).ToString());
            numericUpDown15.Value = Convert.ToDecimal(HOPar.TupleSelect(17).ToString());

            numericUpDown16.Value = Convert.ToDecimal(HOPar.TupleSelect(21).ToString());
            numericUpDown17.Value = Convert.ToDecimal(HOPar.TupleSelect(22).ToString());
            numericUpDown18.Value = Convert.ToDecimal(HOPar.TupleSelect(23).ToString());
            numericUpDown19.Value = Convert.ToDecimal(HOPar.TupleSelect(24).ToString());
            numericUpDown20.Value = Convert.ToDecimal(HOPar.TupleSelect(25).ToString());
            numericUpDown21.Value = Convert.ToDecimal(HOPar.TupleSelect(26).ToString());
            numericUpDown22.Value = Convert.ToDecimal(HOPar.TupleSelect(27).ToString());
            numericUpDown23.Value = Convert.ToDecimal(HOPar.TupleSelect(28).ToString());
            numericUpDown24.Value = Convert.ToDecimal(HOPar.TupleSelect(29).ToString());
            numericUpDown25.Value = Convert.ToDecimal(HOPar.TupleSelect(30).ToString());
            numericUpDown26.Value = Convert.ToDecimal(HOPar.TupleSelect(31).ToString());
            numericUpDown27.Value = Convert.ToDecimal(HOPar.TupleSelect(32).ToString());
            numericUpDown28.Value = Convert.ToDecimal(HOPar.TupleSelect(33).ToString());
            numericUpDown29.Value = Convert.ToDecimal(HOPar.TupleSelect(34).ToString());
            numericUpDown30.Value = Convert.ToDecimal(HOPar.TupleSelect(35).ToString());
        }

        private void button2_Click(object sender, EventArgs e)
        {
            HTuple HOPar = HRead(CircleInspectionAdjustDisplay.shapeModelImageFileDirectionPath_CircleInspectionSample + CircleInspectionAdjustDisplay.shapeModelImageFileName_CircleInspectionSample + ".smddd");

            if (HOPar.Length == 15 || HOPar.Length == 30)
            {
                HOPar[0] = 0;
                HOPar[1] = 0;
                HOPar[2] = 0;

                HOPar[18] = 0;
                HOPar[19] = 0;
                HOPar[20] = 0;
            }

            HOPar[3] = numericUpDown1.Value.ToString();
            HOPar[4] = numericUpDown2.Value.ToString();
            HOPar[5] = numericUpDown3.Value.ToString();
            HOPar[6] = numericUpDown4.Value.ToString();
            HOPar[7] = numericUpDown5.Value.ToString();
            HOPar[8] = numericUpDown6.Value.ToString();
            HOPar[9] = numericUpDown7.Value.ToString();
            HOPar[10] = numericUpDown8.Value.ToString();
            HOPar[11] = numericUpDown9.Value.ToString();
            HOPar[12] = numericUpDown10.Value.ToString();
            HOPar[13] = numericUpDown11.Value.ToString();
            HOPar[14] = numericUpDown12.Value.ToString();
            HOPar[15] = numericUpDown13.Value.ToString();
            HOPar[16] = numericUpDown14.Value.ToString();
            HOPar[17] = numericUpDown15.Value.ToString();

            HOPar[21] = numericUpDown16.Value.ToString();
            HOPar[22] = numericUpDown17.Value.ToString();
            HOPar[23] = numericUpDown18.Value.ToString();
            HOPar[24] = numericUpDown19.Value.ToString();
            HOPar[25] = numericUpDown20.Value.ToString();
            HOPar[26] = numericUpDown21.Value.ToString();
            HOPar[27] = numericUpDown22.Value.ToString();
            HOPar[28] = numericUpDown23.Value.ToString();
            HOPar[29] = numericUpDown24.Value.ToString();
            HOPar[30] = numericUpDown25.Value.ToString();
            HOPar[31] = numericUpDown26.Value.ToString();
            HOPar[32] = numericUpDown27.Value.ToString();
            HOPar[33] = numericUpDown28.Value.ToString();
            HOPar[34] = numericUpDown29.Value.ToString();
            HOPar[35] = numericUpDown30.Value.ToString();

            string s = "";
            //if (circlesCounterFlag < 2)
            //    s = HOPar.TupleSelect(0) + " " + HOPar.TupleSelect(1) + " " + HOPar.TupleSelect(2) +
            //        "\r"
            //        + HOPar.TupleSelect(3) + " " + HOPar.TupleSelect(4) + " " + HOPar.TupleSelect(5) + " " + HOPar.TupleSelect(6) + " " + HOPar.TupleSelect(7) + " "
            //        + HOPar.TupleSelect(8) + " " + HOPar.TupleSelect(9) + " " + HOPar.TupleSelect(10) + " " + HOPar.TupleSelect(11) + " "
            //        + HOPar.TupleSelect(12) + " " + HOPar.TupleSelect(13) + " " + HOPar.TupleSelect(14) + " " + HOPar.TupleSelect(15) + " "
            //        + HOPar.TupleSelect(16) + " " + HOPar.TupleSelect(17);

            //if (circlesCounterFlag == 2)
                s = HOPar.TupleSelect(0) + " " + HOPar.TupleSelect(1) + " " + HOPar.TupleSelect(2) +
                    "\r"
                    + HOPar.TupleSelect(3) + " " + HOPar.TupleSelect(4) + " " + HOPar.TupleSelect(5) + " " + HOPar.TupleSelect(6) + " " + HOPar.TupleSelect(7) + " "
                    + HOPar.TupleSelect(8) + " " + HOPar.TupleSelect(9) + " " + HOPar.TupleSelect(10) + " " + HOPar.TupleSelect(11) + " "
                    + HOPar.TupleSelect(12) + " " + HOPar.TupleSelect(13) + " " + HOPar.TupleSelect(14) + " " + HOPar.TupleSelect(15) + " "
                    + HOPar.TupleSelect(16) + " " + HOPar.TupleSelect(17) +
                    "\r"
                    + HOPar.TupleSelect(18) + " " + HOPar.TupleSelect(19) + " " + HOPar.TupleSelect(20) +
                    "\r"
                + HOPar.TupleSelect(21) + " " + HOPar.TupleSelect(22) + " " + HOPar.TupleSelect(23) + " " + HOPar.TupleSelect(24) + " " + HOPar.TupleSelect(25) + " "
                + HOPar.TupleSelect(26) + " " + HOPar.TupleSelect(27) + " " + HOPar.TupleSelect(28) + " " + HOPar.TupleSelect(29) + " "
                + HOPar.TupleSelect(30) + " " + HOPar.TupleSelect(31) + " " + HOPar.TupleSelect(32) + " " + HOPar.TupleSelect(33) + " "
                + HOPar.TupleSelect(34) + " " + HOPar.TupleSelect(35);

            HWrite(CircleInspectionAdjustDisplay.shapeModelImageFileDirectionPath_CircleInspectionSample + CircleInspectionAdjustDisplay.shapeModelImageFileName_CircleInspectionSample + ".smddd", s);
        }

        //读取参数
        private HTuple HRead(string ReadFilePath)
        {
            HTuple hv_ParData = new HTuple();
            HTuple hv_FileHandle;
            HTuple hv_ParDataFilePath;
            HTuple hv_OutString;
            HTuple hv_IsEOF;
            HTuple hv_Number;
            HTuple hv_i;

            try
            {
                hv_ParDataFilePath = ReadFilePath;
                HOperatorSet.OpenFile(hv_ParDataFilePath, "input", out hv_FileHandle);
                HOperatorSet.FreadString(hv_FileHandle, out hv_OutString, out hv_IsEOF);
                HOperatorSet.TupleNumber(hv_OutString, out hv_Number);
                hv_ParData[0] = hv_Number;
                hv_i = 1;
                while ((int)(new HTuple(hv_IsEOF.TupleEqual(0))) != 0)
                {
                    HOperatorSet.FreadString(hv_FileHandle, out hv_OutString, out hv_IsEOF);
                    if ((int)(new HTuple(hv_IsEOF.TupleEqual(0))) != 0)
                    {
                        HOperatorSet.TupleNumber(hv_OutString, out hv_Number);
                        hv_ParData[hv_i] = hv_Number;
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple ExpTmpLocalVar_i = hv_i + 1;
                                hv_i = ExpTmpLocalVar_i;
                            }
                        }
                    }
                }
                HOperatorSet.CloseFile(hv_FileHandle);
            }
            catch
            {
                if(LanguageFlag == 0)
                    MessageBox.Show("Load picture process function parameter failed!");
                else
                    MessageBox.Show("读取算子参数文件失败!");
            }
            return hv_ParData;
        }

        //写入参数
        private void HWrite(string WriteFilePath, string sData)
        {
            HTuple hv_FileHandle;
            HOperatorSet.OpenFile(WriteFilePath, "output", out hv_FileHandle);
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                HOperatorSet.FwriteString(hv_FileHandle, sData);
            }
            HOperatorSet.CloseFile(hv_FileHandle);
        }

        //UI Updata
        private void Updata()
        {
            while (true)
            {
                if (this != null)
                {
                    if (!this.IsDisposed)
                    {
                        this.Invoke(new Action(() =>
                        {
                            scale_image_numeric1 = Convert.ToDouble(numericUpDown1.Value);
                            scale_image_numeric2 = Convert.ToDouble(numericUpDown2.Value);
                            threshold_numeric1 = Convert.ToDouble(numericUpDown3.Value);
                            threshold_numeric2 = Convert.ToDouble(numericUpDown4.Value);
                            select_shape_numeric1 = Convert.ToDouble(numericUpDown5.Value);
                            select_shape_numeric2 = Convert.ToDouble(numericUpDown6.Value);
                            dilation_circle_numeri1 = Convert.ToDouble(numericUpDown7.Value);
                            edges_sub_pix_numeric1 = Convert.ToDouble(numericUpDown8.Value);
                            edges_sub_pix_numeric2 = Convert.ToDouble(numericUpDown9.Value);
                            union_cocircular_contours_xld_numeric1 = Convert.ToDouble(numericUpDown10.Value);
                            union_cocircular_contours_xld_numeric2 = Convert.ToDouble(numericUpDown11.Value);
                            union_cocircular_contours_xld_numeric3 = Convert.ToDouble(numericUpDown12.Value);
                            union_cocircular_contours_xld_numeric4 = Convert.ToDouble(numericUpDown13.Value);
                            union_cocircular_contours_xld_numeric5 = Convert.ToDouble(numericUpDown14.Value);
                            union_cocircular_contours_xld_numeric6 = Convert.ToDouble(numericUpDown15.Value);

                            scale_image_numeric1_2th = Convert.ToDouble(numericUpDown16.Value);
                            scale_image_numeric2_2th = Convert.ToDouble(numericUpDown17.Value);
                            threshold_numeric1_2th = Convert.ToDouble(numericUpDown18.Value);
                            threshold_numeric2_2th = Convert.ToDouble(numericUpDown19.Value);
                            select_shape_numeric1_2th = Convert.ToDouble(numericUpDown20.Value);
                            select_shape_numeric2_2th = Convert.ToDouble(numericUpDown21.Value);
                            dilation_circle_numeri1_2th = Convert.ToDouble(numericUpDown22.Value);
                            edges_sub_pix_numeric1_2th = Convert.ToDouble(numericUpDown23.Value);
                            edges_sub_pix_numeric2_2th = Convert.ToDouble(numericUpDown24.Value);
                            union_cocircular_contours_xld_numeric1_2th = Convert.ToDouble(numericUpDown25.Value);
                            union_cocircular_contours_xld_numeric2_2th = Convert.ToDouble(numericUpDown26.Value);
                            union_cocircular_contours_xld_numeric3_2th = Convert.ToDouble(numericUpDown27.Value);
                            union_cocircular_contours_xld_numeric4_2th = Convert.ToDouble(numericUpDown28.Value);
                            union_cocircular_contours_xld_numeric5_2th = Convert.ToDouble(numericUpDown29.Value);
                            union_cocircular_contours_xld_numeric6_2th = Convert.ToDouble(numericUpDown30.Value);

                            label7.Text = CircleInspectionAdjustDisplay.FitCircleStepNo_FitCircle.ToString();
                            label10.Text = CircleInspectionAdjustDisplay.FitCircleStepNo_FitCircle_2th.ToString();

                            //while (circleInspectionAdjustDisplay == null || MainWindow.circleInspection_Copy == null || circleInspectionAdjustDisplay.ho_Image == null)
                            //{
                            //    circleInspectionAdjustDisplay = MainWindow.circleInspection_Copy;
                            //}

                            if (!displayFlag1)
                            {
                                HOperatorSet.OpenWindow(0, 0, pictureBox1.Width, pictureBox1.Height, pictureBox1.Handle, "visible", "", out WindowHandle1);

                                HOperatorSet.OpenWindow(0, 0, pictureBox7.Width, pictureBox7.Height, pictureBox7.Handle, "visible", "", out WindowHandle1_2th);

                                //HDevWindowStack.Push(WindowHandle1);
                                displayFlag1 = true;
                            }
                            if (!displayFlag2)
                            {
                                HOperatorSet.OpenWindow(0, 0, pictureBox2.Width, pictureBox2.Height, pictureBox2.Handle, "visible", "", out WindowHandle2);

                                HOperatorSet.OpenWindow(0, 0, pictureBox8.Width, pictureBox8.Height, pictureBox8.Handle, "visible", "", out WindowHandle2_2th);

                                //HDevWindowStack.Push(WindowHandle2);
                                displayFlag2 = true;
                            }
                            if (!displayFlag3)
                            {
                                HOperatorSet.OpenWindow(0, 0, pictureBox3.Width, pictureBox3.Height, pictureBox3.Handle, "visible", "", out WindowHandle3);

                                HOperatorSet.OpenWindow(0, 0, pictureBox9.Width, pictureBox9.Height, pictureBox9.Handle, "visible", "", out WindowHandle3_2th);

                                //HDevWindowStack.Push(WindowHandle3);
                                displayFlag3 = true;
                            }
                            if (!displayFlag4)
                            {
                                HOperatorSet.OpenWindow(0, 0, pictureBox4.Width, pictureBox4.Height, pictureBox4.Handle, "visible", "", out WindowHandle4);

                                HOperatorSet.OpenWindow(0, 0, pictureBox10.Width, pictureBox10.Height, pictureBox10.Handle, "visible", "", out WindowHandle4_2th);

                                //HDevWindowStack.Push(WindowHandle4);
                                displayFlag4 = true;
                            }
                            if (!displayFlag5)
                            {
                                HOperatorSet.OpenWindow(0, 0, pictureBox5.Width, pictureBox5.Height, pictureBox5.Handle, "visible", "", out WindowHandle5);

                                HOperatorSet.OpenWindow(0, 0, pictureBox11.Width, pictureBox11.Height, pictureBox11.Handle, "visible", "", out WindowHandle5_2th);

                                //HDevWindowStack.Push(WindowHandle5);
                                displayFlag5 = true;
                            }
                            if (!displayFlag6)
                            {
                                HOperatorSet.OpenWindow(0, 0, pictureBox6.Width, pictureBox6.Height, pictureBox6.Handle, "visible", "", out WindowHandle6);

                                HOperatorSet.OpenWindow(0, 0, pictureBox12.Width, pictureBox12.Height, pictureBox12.Handle, "visible", "", out WindowHandle6_2th);

                                //HDevWindowStack.Push(WindowHandle6);
                                displayFlag6 = true;
                            }

                            if (windowRefreshFlag)
                            {
                                HOperatorSet.ClearWindow(WindowHandle1);
                                HOperatorSet.ClearWindow(WindowHandle2);
                                HOperatorSet.ClearWindow(WindowHandle3);
                                HOperatorSet.ClearWindow(WindowHandle4);
                                HOperatorSet.ClearWindow(WindowHandle5);
                                HOperatorSet.ClearWindow(WindowHandle6);

                                HOperatorSet.ClearWindow(WindowHandle1_2th);
                                HOperatorSet.ClearWindow(WindowHandle2_2th);
                                HOperatorSet.ClearWindow(WindowHandle3_2th);
                                HOperatorSet.ClearWindow(WindowHandle4_2th);
                                HOperatorSet.ClearWindow(WindowHandle5_2th);
                                HOperatorSet.ClearWindow(WindowHandle6_2th);
                                windowRefreshFlag = false;
                            }
                            if (HalconOperatorValueChangedFlag == 2)
                            {
                                HOperatorSet.ClearWindow(WindowHandle1);
                                HOperatorSet.ClearWindow(WindowHandle2);
                                HOperatorSet.ClearWindow(WindowHandle3);
                                HOperatorSet.ClearWindow(WindowHandle4);
                                HOperatorSet.ClearWindow(WindowHandle5);
                                HOperatorSet.ClearWindow(WindowHandle6);

                                HOperatorSet.ClearWindow(WindowHandle1_2th);
                                HOperatorSet.ClearWindow(WindowHandle2_2th);
                                HOperatorSet.ClearWindow(WindowHandle3_2th);
                                HOperatorSet.ClearWindow(WindowHandle4_2th);
                                HOperatorSet.ClearWindow(WindowHandle5_2th);
                                HOperatorSet.ClearWindow(WindowHandle6_2th);

                                //Step1
                                if (CircleInspectionAdjustDisplay.ho_ImageScaled_FitCircle != null)
                                {
                                    try
                                    {
                                        HOperatorSet.SmallestRectangle1(CircleInspectionAdjustDisplay.ho_ImageScaled_FitCircle, out hv_Row1, out hv_Column1, out hv_Row2, out hv_Column2);
                                        if (hv_Row1 != null && hv_Row1.Type.ToString() != "EMPTY")
                                        {
                                            HOperatorSet.DispObj(ho_Image_CircleInspectionAdjustDisplay, WindowHandle1);//为了清除上次显示的message
                                            HOperatorSet.SetPart(WindowHandle1, hv_Row1, hv_Column1, hv_Row2, hv_Column2);
                                            HOperatorSet.SetPartStyle(WindowHandle1, 2);
                                            HOperatorSet.DispObj(CircleInspectionAdjustDisplay.ho_ImageScaled_FitCircle, WindowHandle1);
                                        }
                                    }
                                    catch
                                    {
                                        HOperatorSet.DispObj(ho_Image_CircleInspectionAdjustDisplay, WindowHandle1);//清除上次显示的画面
                                        if (LanguageFlag == 0)
                                            hv_Information[0] = "Error";
                                        else
                                            hv_Information[0] = "错误";
                                        CircleInspectionAdjustDisplay.disp_message_Halcon(WindowHandle1, hv_Information, "window", 0, 0, "red", "false");
                                    }
                                }
                                if (CircleInspectionAdjustDisplay.ho_ImageScaled_FitCircle_2th != null)//_2th
                                {
                                    try
                                    {
                                        HOperatorSet.SmallestRectangle1(CircleInspectionAdjustDisplay.ho_ImageScaled_FitCircle_2th, out hv_Row1_2th, out hv_Column1_2th, out hv_Row2_2th, out hv_Column2_2th);
                                        if (hv_Row1_2th != null && hv_Row1_2th.Type.ToString() != "EMPTY")
                                        {
                                            HOperatorSet.DispObj(ho_Image_CircleInspectionAdjustDisplay, WindowHandle1_2th);//为了清除上次显示的message
                                            HOperatorSet.SetPart(WindowHandle1_2th, hv_Row1_2th, hv_Column1_2th, hv_Row2_2th, hv_Column2_2th);
                                            HOperatorSet.SetPartStyle(WindowHandle1_2th, 2);
                                            HOperatorSet.DispObj(CircleInspectionAdjustDisplay.ho_ImageScaled_FitCircle_2th, WindowHandle1_2th);
                                        }
                                    }
                                    catch
                                    {
                                        HOperatorSet.DispObj(ho_Image_CircleInspectionAdjustDisplay, WindowHandle1_2th);//清除上次显示的画面
                                        if (LanguageFlag == 0)
                                            hv_Information[0] = "Error";
                                        else
                                            hv_Information[0] = "错误";
                                        CircleInspectionAdjustDisplay.disp_message_Halcon(WindowHandle1_2th, hv_Information, "window", 0, 0, "red", "false");
                                    }
                                }

                                //Step2
                                if (CircleInspectionAdjustDisplay.ho_ImageScaled_FitCircle != null && CircleInspectionAdjustDisplay.ho_Regions_FitCircle != null)
                                {
                                    try
                                    {
                                        HOperatorSet.SmallestRectangle1(CircleInspectionAdjustDisplay.ho_ImageScaled_FitCircle, out hv_Row1, out hv_Column1, out hv_Row2, out hv_Column2);
                                        if (hv_Row1 != null && hv_Row1.Type.ToString() != "EMPTY" && CircleInspectionAdjustDisplay.ho_Regions_FitCircle.CountObj() > 0)
                                        {
                                            HOperatorSet.DispObj(ho_Image_CircleInspectionAdjustDisplay, WindowHandle2);//为了清除上次显示的message
                                            HOperatorSet.SetPart(WindowHandle2, hv_Row1, hv_Column1, hv_Row2, hv_Column2);
                                            HOperatorSet.SetPartStyle(WindowHandle2, 2);
                                            HOperatorSet.DispObj(CircleInspectionAdjustDisplay.ho_ImageScaled_FitCircle, WindowHandle2);
                                            HOperatorSet.SetColor(WindowHandle2, "green");
                                            HOperatorSet.DispObj(CircleInspectionAdjustDisplay.ho_Regions_FitCircle, WindowHandle2);
                                        }
                                        else
                                        {
                                            HOperatorSet.DispObj(ho_Image_CircleInspectionAdjustDisplay, WindowHandle2);//清除上次显示的画面
                                            if (LanguageFlag == 0)
                                                hv_Information[0] = "Error";
                                            else
                                                hv_Information[0] = "错误";
                                            CircleInspectionAdjustDisplay.disp_message_Halcon(WindowHandle2, hv_Information, "window", 0, 0, "red", "false");
                                        }
                                    }
                                    catch
                                    {
                                        HOperatorSet.DispObj(ho_Image_CircleInspectionAdjustDisplay, WindowHandle2);//清除上次显示的画面
                                        if (LanguageFlag == 0)
                                            hv_Information[0] = "Error";
                                        else
                                            hv_Information[0] = "错误";
                                        CircleInspectionAdjustDisplay.disp_message_Halcon(WindowHandle2, hv_Information, "window", 0, 0, "red", "false");
                                    }
                                }
                                if (CircleInspectionAdjustDisplay.ho_ImageScaled_FitCircle_2th != null && CircleInspectionAdjustDisplay.ho_Regions_FitCircle_2th != null)//_2th
                                {
                                    try
                                    {
                                        HOperatorSet.SmallestRectangle1(CircleInspectionAdjustDisplay.ho_ImageScaled_FitCircle_2th, out hv_Row1_2th, out hv_Column1_2th, out hv_Row2_2th, out hv_Column2_2th);
                                        if (hv_Row1_2th != null && hv_Row1_2th.Type.ToString() != "EMPTY" && CircleInspectionAdjustDisplay.ho_Regions_FitCircle_2th.CountObj() > 0)
                                        {
                                            HOperatorSet.DispObj(ho_Image_CircleInspectionAdjustDisplay, WindowHandle2_2th);//为了清除上次显示的message
                                            HOperatorSet.SetPart(WindowHandle2_2th, hv_Row1_2th, hv_Column1_2th, hv_Row2_2th, hv_Column2_2th);
                                            HOperatorSet.SetPartStyle(WindowHandle2_2th, 2);
                                            HOperatorSet.DispObj(CircleInspectionAdjustDisplay.ho_ImageScaled_FitCircle_2th, WindowHandle2_2th);
                                            HOperatorSet.SetColor(WindowHandle2_2th, "green");
                                            HOperatorSet.DispObj(CircleInspectionAdjustDisplay.ho_Regions_FitCircle_2th, WindowHandle2_2th);
                                        }
                                        else
                                        {
                                            HOperatorSet.DispObj(ho_Image_CircleInspectionAdjustDisplay, WindowHandle2_2th);//清除上次显示的画面
                                            if (LanguageFlag == 0)
                                                hv_Information[0] = "Error";
                                            else
                                                hv_Information[0] = "错误";
                                            CircleInspectionAdjustDisplay.disp_message_Halcon(WindowHandle2_2th, hv_Information, "window", 0, 0, "red", "false");
                                        }
                                    }
                                    catch
                                    {
                                        HOperatorSet.DispObj(ho_Image_CircleInspectionAdjustDisplay, WindowHandle2_2th);//清除上次显示的画面
                                        if (LanguageFlag == 0)
                                            hv_Information[0] = "Error";
                                        else
                                            hv_Information[0] = "错误";
                                        CircleInspectionAdjustDisplay.disp_message_Halcon(WindowHandle2_2th, hv_Information, "window", 0, 0, "red", "false");
                                    }
                                }

                                //Step3
                                if (CircleInspectionAdjustDisplay.ho_ImageScaled_FitCircle != null && CircleInspectionAdjustDisplay.ho_RegionBorder_FitCircle != null)
                                {
                                    try
                                    {
                                        HOperatorSet.SmallestRectangle1(CircleInspectionAdjustDisplay.ho_ImageScaled_FitCircle, out hv_Row1, out hv_Column1, out hv_Row2, out hv_Column2);
                                        if (hv_Row1 != null && hv_Row1.Type.ToString() != "EMPTY" && CircleInspectionAdjustDisplay.ho_RegionBorder_FitCircle.CountObj() > 0)
                                        {
                                            HOperatorSet.DispObj(ho_Image_CircleInspectionAdjustDisplay, WindowHandle3);//为了清除上次显示的message
                                            HOperatorSet.SetPart(WindowHandle3, hv_Row1, hv_Column1, hv_Row2, hv_Column2);
                                            HOperatorSet.SetPartStyle(WindowHandle3, 2);
                                            HOperatorSet.DispObj(CircleInspectionAdjustDisplay.ho_ImageScaled_FitCircle, WindowHandle3);
                                            HOperatorSet.SetColor(WindowHandle3, "green");
                                            HOperatorSet.DispObj(CircleInspectionAdjustDisplay.ho_RegionBorder_FitCircle, WindowHandle3);
                                        }
                                        else
                                        {
                                            HOperatorSet.DispObj(ho_Image_CircleInspectionAdjustDisplay, WindowHandle3);//清除上次显示的画面
                                            if (LanguageFlag == 0)
                                                hv_Information[0] = "Error";
                                            else
                                                hv_Information[0] = "错误";
                                            CircleInspectionAdjustDisplay.disp_message_Halcon(WindowHandle3, hv_Information, "window", 0, 0, "red", "false");
                                        }
                                    }
                                    catch
                                    {
                                        HOperatorSet.DispObj(ho_Image_CircleInspectionAdjustDisplay, WindowHandle3);//清除上次显示的画面
                                        if (LanguageFlag == 0)
                                            hv_Information[0] = "Error";
                                        else
                                            hv_Information[0] = "错误";
                                        CircleInspectionAdjustDisplay.disp_message_Halcon(WindowHandle3, hv_Information, "window", 0, 0, "red", "false");
                                    }
                                }
                                if (CircleInspectionAdjustDisplay.ho_ImageScaled_FitCircle_2th != null && CircleInspectionAdjustDisplay.ho_RegionBorder_FitCircle_2th != null)//_2th
                                {
                                    try
                                    {
                                        HOperatorSet.SmallestRectangle1(CircleInspectionAdjustDisplay.ho_ImageScaled_FitCircle_2th, out hv_Row1_2th, out hv_Column1_2th, out hv_Row2_2th, out hv_Column2_2th);
                                        if (hv_Row1_2th != null && hv_Row1_2th.Type.ToString() != "EMPTY" && CircleInspectionAdjustDisplay.ho_RegionBorder_FitCircle_2th.CountObj() > 0)
                                        {
                                            HOperatorSet.DispObj(ho_Image_CircleInspectionAdjustDisplay, WindowHandle3_2th);//为了清除上次显示的message
                                            HOperatorSet.SetPart(WindowHandle3_2th, hv_Row1_2th, hv_Column1_2th, hv_Row2_2th, hv_Column2_2th);
                                            HOperatorSet.SetPartStyle(WindowHandle3_2th, 2);
                                            HOperatorSet.DispObj(CircleInspectionAdjustDisplay.ho_ImageScaled_FitCircle_2th, WindowHandle3_2th);
                                            HOperatorSet.SetColor(WindowHandle3_2th, "green");
                                            HOperatorSet.DispObj(CircleInspectionAdjustDisplay.ho_RegionBorder_FitCircle_2th, WindowHandle3_2th);
                                        }
                                        else
                                        {
                                            HOperatorSet.DispObj(ho_Image_CircleInspectionAdjustDisplay, WindowHandle3_2th);//清除上次显示的画面
                                            if (LanguageFlag == 0)
                                                hv_Information[0] = "Error";
                                            else
                                                hv_Information[0] = "错误";
                                            CircleInspectionAdjustDisplay.disp_message_Halcon(WindowHandle3_2th, hv_Information, "window", 0, 0, "red", "false");
                                        }
                                    }
                                    catch
                                    {
                                        HOperatorSet.DispObj(ho_Image_CircleInspectionAdjustDisplay, WindowHandle3_2th);//清除上次显示的画面
                                        if (LanguageFlag == 0)
                                            hv_Information[0] = "Error";
                                        else
                                            hv_Information[0] = "错误";
                                        CircleInspectionAdjustDisplay.disp_message_Halcon(WindowHandle3_2th, hv_Information, "window", 0, 0, "red", "false");
                                    }
                                }

                                //Step4
                                if (CircleInspectionAdjustDisplay.ho_ImageScaled_FitCircle != null && CircleInspectionAdjustDisplay.ho_RegionDilation_FitCircle != null)
                                {
                                    try
                                    {
                                        HOperatorSet.SmallestRectangle1(CircleInspectionAdjustDisplay.ho_ImageScaled_FitCircle, out hv_Row1, out hv_Column1, out hv_Row2, out hv_Column2);
                                        if (hv_Row1 != null && hv_Row1.Type.ToString() != "EMPTY" && CircleInspectionAdjustDisplay.ho_RegionDilation_FitCircle.CountObj() > 0 && CircleInspectionAdjustDisplay.ho_Edges_FitCircle.CountObj()>0)
                                        //if (hv_Row1 != null && hv_Row1.Type.ToString() != "EMPTY" && CircleInspectionAdjustDisplay.ho_RegionDilation_FitCircle.CountObj() > 0)
                                        {
                                            HOperatorSet.DispObj(ho_Image_CircleInspectionAdjustDisplay, WindowHandle4);//为了清除上次显示的message
                                            HOperatorSet.SetPart(WindowHandle4, hv_Row1, hv_Column1, hv_Row2, hv_Column2);
                                            HOperatorSet.SetPartStyle(WindowHandle4, 2);
                                            HOperatorSet.DispObj(CircleInspectionAdjustDisplay.ho_ImageScaled_FitCircle, WindowHandle4);
                                            HOperatorSet.SetColor(WindowHandle4, "green");
                                            HOperatorSet.DispObj(CircleInspectionAdjustDisplay.ho_RegionDilation_FitCircle, WindowHandle4);
                                            //HOperatorSet.DispObj(CircleInspectionAdjustDisplay.ho_ImageReduced1_FitCircle, WindowHandle4);
                                        }
                                        else
                                        {
                                            HOperatorSet.DispObj(ho_Image_CircleInspectionAdjustDisplay, WindowHandle4);//清除上次显示的画面
                                            if (LanguageFlag == 0)
                                                hv_Information[0] = "Error";
                                            else
                                                hv_Information[0] = "错误";
                                            CircleInspectionAdjustDisplay.disp_message_Halcon(WindowHandle4, hv_Information, "window", 0, 0, "red", "false");
                                        }
                                    }
                                    catch
                                    {
                                        HOperatorSet.DispObj(ho_Image_CircleInspectionAdjustDisplay, WindowHandle4);//清除上次显示的画面
                                        if (LanguageFlag == 0)
                                            hv_Information[0] = "Error";
                                        else
                                            hv_Information[0] = "错误";
                                        CircleInspectionAdjustDisplay.disp_message_Halcon(WindowHandle4, hv_Information, "window", 0, 0, "red", "false");
                                    }
                                }
                                if (CircleInspectionAdjustDisplay.ho_ImageScaled_FitCircle_2th != null && CircleInspectionAdjustDisplay.ho_RegionDilation_FitCircle_2th != null)//_2th
                                {
                                    try
                                    {
                                        HOperatorSet.SmallestRectangle1(CircleInspectionAdjustDisplay.ho_ImageScaled_FitCircle_2th, out hv_Row1_2th, out hv_Column1_2th, out hv_Row2_2th, out hv_Column2_2th);
                                        if (hv_Row1_2th != null && hv_Row1_2th.Type.ToString() != "EMPTY" && CircleInspectionAdjustDisplay.ho_RegionDilation_FitCircle_2th.CountObj() > 0 && CircleInspectionAdjustDisplay.ho_Edges_FitCircle_2th.CountObj() > 0)
                                        {
                                            HOperatorSet.DispObj(ho_Image_CircleInspectionAdjustDisplay, WindowHandle4_2th);//为了清除上次显示的message
                                            HOperatorSet.SetPart(WindowHandle4_2th, hv_Row1_2th, hv_Column1_2th, hv_Row2_2th, hv_Column2_2th);
                                            HOperatorSet.SetPartStyle(WindowHandle4_2th, 2);
                                            HOperatorSet.DispObj(CircleInspectionAdjustDisplay.ho_ImageScaled_FitCircle_2th, WindowHandle4_2th);
                                            HOperatorSet.SetColor(WindowHandle4_2th, "green");
                                            HOperatorSet.DispObj(CircleInspectionAdjustDisplay.ho_RegionDilation_FitCircle_2th, WindowHandle4_2th);
                                            //HOperatorSet.DispObj(CircleInspectionAdjustDisplay.ho_ImageReduced1_FitCircle_2th, WindowHandle4_2th);
                                        }
                                        else
                                        {
                                            HOperatorSet.DispObj(ho_Image_CircleInspectionAdjustDisplay, WindowHandle4_2th);//清除上次显示的画面
                                            if (LanguageFlag == 0)
                                                hv_Information[0] = "Error";
                                            else
                                                hv_Information[0] = "错误";
                                            CircleInspectionAdjustDisplay.disp_message_Halcon(WindowHandle4_2th, hv_Information, "window", 0, 0, "red", "false");
                                        }
                                    }
                                    catch
                                    {
                                        HOperatorSet.DispObj(ho_Image_CircleInspectionAdjustDisplay, WindowHandle4_2th);//清除上次显示的画面
                                        if (LanguageFlag == 0)
                                            hv_Information[0] = "Error";
                                        else
                                            hv_Information[0] = "错误";
                                        CircleInspectionAdjustDisplay.disp_message_Halcon(WindowHandle4_2th, hv_Information, "window", 0, 0, "red", "false");
                                    }
                                }

                                //Step5
                                if (CircleInspectionAdjustDisplay.ho_ImageScaled_FitCircle != null && CircleInspectionAdjustDisplay.ho_SelectedXLD_FitCircle != null)
                                {
                                    try
                                    {
                                        HOperatorSet.SmallestRectangle1(CircleInspectionAdjustDisplay.ho_ImageScaled_FitCircle, out hv_Row1, out hv_Column1, out hv_Row2, out hv_Column2);
                                        if (hv_Row1 != null && hv_Row1.Type.ToString() != "EMPTY" && CircleInspectionAdjustDisplay.ho_SelectedXLD_FitCircle.CountObj() > 0)
                                        {
                                            HOperatorSet.DispObj(ho_Image_CircleInspectionAdjustDisplay, WindowHandle5);//为了清除上次显示的message
                                            HOperatorSet.SetPart(WindowHandle5, hv_Row1, hv_Column1, hv_Row2, hv_Column2);
                                            HOperatorSet.SetPartStyle(WindowHandle5, 2);
                                            HOperatorSet.DispObj(CircleInspectionAdjustDisplay.ho_ImageScaled_FitCircle, WindowHandle5);
                                            HOperatorSet.SetColor(WindowHandle5, "green");
                                            HOperatorSet.DispObj(CircleInspectionAdjustDisplay.ho_SelectedXLD_FitCircle, WindowHandle5);
                                        }
                                        else
                                        {
                                            HOperatorSet.DispObj(ho_Image_CircleInspectionAdjustDisplay, WindowHandle5);//清除上次显示的画面
                                            if (LanguageFlag == 0)
                                                hv_Information[0] = "Error";
                                            else
                                                hv_Information[0] = "错误";
                                            CircleInspectionAdjustDisplay.disp_message_Halcon(WindowHandle5, hv_Information, "window", 0, 0, "red", "false");
                                        }
                                    }
                                    catch
                                    {
                                        HOperatorSet.DispObj(ho_Image_CircleInspectionAdjustDisplay, WindowHandle5);//清除上次显示的画面
                                        if (LanguageFlag == 0)
                                            hv_Information[0] = "Error";
                                        else
                                            hv_Information[0] = "错误";
                                        CircleInspectionAdjustDisplay.disp_message_Halcon(WindowHandle5, hv_Information, "window", 0, 0, "red", "false");
                                    }
                                }
                                if (CircleInspectionAdjustDisplay.ho_ImageScaled_FitCircle_2th != null && CircleInspectionAdjustDisplay.ho_SelectedXLD_FitCircle_2th != null)//_2th
                                {
                                    try
                                    {
                                        HOperatorSet.SmallestRectangle1(CircleInspectionAdjustDisplay.ho_ImageScaled_FitCircle_2th, out hv_Row1_2th, out hv_Column1_2th, out hv_Row2_2th, out hv_Column2_2th);
                                        if (hv_Row1_2th != null && hv_Row1_2th.Type.ToString() != "EMPTY" && CircleInspectionAdjustDisplay.ho_SelectedXLD_FitCircle_2th.CountObj() > 0)
                                        {
                                            HOperatorSet.DispObj(ho_Image_CircleInspectionAdjustDisplay, WindowHandle5_2th);//为了清除上次显示的message
                                            HOperatorSet.SetPart(WindowHandle5_2th, hv_Row1_2th, hv_Column1_2th, hv_Row2_2th, hv_Column2_2th);
                                            HOperatorSet.SetPartStyle(WindowHandle5_2th, 2);
                                            HOperatorSet.DispObj(CircleInspectionAdjustDisplay.ho_ImageScaled_FitCircle_2th, WindowHandle5_2th);
                                            HOperatorSet.SetColor(WindowHandle5_2th, "green");
                                            HOperatorSet.DispObj(CircleInspectionAdjustDisplay.ho_SelectedXLD_FitCircle_2th, WindowHandle5_2th);
                                        }
                                        else
                                        {
                                            HOperatorSet.DispObj(ho_Image_CircleInspectionAdjustDisplay, WindowHandle5_2th);//清除上次显示的画面
                                            if (LanguageFlag == 0)
                                                hv_Information[0] = "Error";
                                            else
                                                hv_Information[0] = "错误";
                                            CircleInspectionAdjustDisplay.disp_message_Halcon(WindowHandle5_2th, hv_Information, "window", 0, 0, "red", "false");
                                        }
                                    }
                                    catch
                                    {
                                        HOperatorSet.DispObj(ho_Image_CircleInspectionAdjustDisplay, WindowHandle5_2th);//清除上次显示的画面
                                        if (LanguageFlag == 0)
                                            hv_Information[0] = "Error";
                                        else
                                            hv_Information[0] = "错误";
                                        CircleInspectionAdjustDisplay.disp_message_Halcon(WindowHandle5_2th, hv_Information, "window", 0, 0, "red", "false");
                                    }
                                }

                                //Step6
                                if (CircleInspectionAdjustDisplay.ho_ImageScaled_FitCircle != null && CircleInspectionAdjustDisplay.ho_UnionContours_FitCircle != null)
                                {
                                    try
                                    {
                                        HOperatorSet.SmallestRectangle1(CircleInspectionAdjustDisplay.ho_ImageScaled_FitCircle, out hv_Row1, out hv_Column1, out hv_Row2, out hv_Column2);
                                        if (hv_Row1 != null && hv_Row1.Type.ToString() != "EMPTY" && CircleInspectionAdjustDisplay.ho_Circle_FitCircle.CountObj() > 0)
                                        {
                                            HOperatorSet.DispObj(ho_Image_CircleInspectionAdjustDisplay, WindowHandle6);//为了清除上次显示的message
                                            HOperatorSet.SetPart(WindowHandle6, hv_Row1, hv_Column1, hv_Row2, hv_Column2);
                                            HOperatorSet.SetPartStyle(WindowHandle6, 2);
                                            HOperatorSet.DispObj(CircleInspectionAdjustDisplay.ho_ImageScaled_FitCircle, WindowHandle6);
                                            //HOperatorSet.SetColor(WindowHandle6, "yellow");
                                            //HOperatorSet.DispObj(CircleInspectionAdjustDisplay.ho_UnionContours_FitCircle, WindowHandle6);
                                            HOperatorSet.SetColor(WindowHandle6, "green");
                                            HOperatorSet.SetDraw(WindowHandle6, "margin");
                                            HOperatorSet.DispObj(CircleInspectionAdjustDisplay.ho_Circle_FitCircle, WindowHandle6);
                                        }
                                        else
                                        {
                                            HOperatorSet.DispObj(ho_Image_CircleInspectionAdjustDisplay, WindowHandle6);//清除上次显示的画面
                                            if (LanguageFlag == 0)
                                                hv_Information[0] = "Error";
                                            else
                                                hv_Information[0] = "错误";
                                            CircleInspectionAdjustDisplay.disp_message_Halcon(WindowHandle6, hv_Information, "window", 0, 0, "red", "false");
                                        }
                                    }
                                    catch
                                    {
                                        HOperatorSet.DispObj(ho_Image_CircleInspectionAdjustDisplay, WindowHandle6);//清除上次显示的画面
                                        if (LanguageFlag == 0)
                                            hv_Information[0] = "Error";
                                        else
                                            hv_Information[0] = "错误";
                                        CircleInspectionAdjustDisplay.disp_message_Halcon(WindowHandle6, hv_Information, "window", 0, 0, "red", "false");
                                    }
                                }
                                if (CircleInspectionAdjustDisplay.ho_ImageScaled_FitCircle_2th != null && CircleInspectionAdjustDisplay.ho_UnionContours_FitCircle_2th != null)//_2th
                                {
                                    try
                                    {
                                        HOperatorSet.SmallestRectangle1(CircleInspectionAdjustDisplay.ho_ImageScaled_FitCircle_2th, out hv_Row1_2th, out hv_Column1_2th, out hv_Row2_2th, out hv_Column2_2th);
                                        if (hv_Row1_2th != null && hv_Row1_2th.Type.ToString() != "EMPTY" && CircleInspectionAdjustDisplay.ho_Circle_FitCircle_2th.CountObj() > 0)
                                        {
                                            HOperatorSet.DispObj(ho_Image_CircleInspectionAdjustDisplay, WindowHandle6_2th);//为了清除上次显示的message
                                            HOperatorSet.SetPart(WindowHandle6_2th, hv_Row1_2th, hv_Column1_2th, hv_Row2_2th, hv_Column2_2th);
                                            HOperatorSet.SetPartStyle(WindowHandle6_2th, 2);
                                            HOperatorSet.DispObj(CircleInspectionAdjustDisplay.ho_ImageScaled_FitCircle_2th, WindowHandle6_2th);
                                            //HOperatorSet.SetColor(WindowHandle6_2th, "yellow");
                                            //HOperatorSet.DispObj(CircleInspectionAdjustDisplay.ho_UnionContours_FitCircle_2th, WindowHandle6_2th);
                                            HOperatorSet.SetColor(WindowHandle6_2th, "green");
                                            HOperatorSet.SetDraw(WindowHandle6_2th, "margin");
                                            HOperatorSet.DispObj(CircleInspectionAdjustDisplay.ho_Circle_FitCircle_2th, WindowHandle6_2th);
                                        }
                                        else
                                        {
                                            HOperatorSet.DispObj(ho_Image_CircleInspectionAdjustDisplay, WindowHandle6_2th);//清除上次显示的画面
                                            if (LanguageFlag == 0)
                                                hv_Information[0] = "Error";
                                            else
                                                hv_Information[0] = "错误";
                                            CircleInspectionAdjustDisplay.disp_message_Halcon(WindowHandle6_2th, hv_Information, "window", 0, 0, "red", "false");
                                        }
                                    }
                                    catch
                                    {
                                        HOperatorSet.DispObj(ho_Image_CircleInspectionAdjustDisplay, WindowHandle6_2th);//清除上次显示的画面
                                        if (LanguageFlag == 0)
                                            hv_Information[0] = "Error";
                                        else
                                            hv_Information[0] = "错误";
                                        CircleInspectionAdjustDisplay.disp_message_Halcon(WindowHandle6_2th, hv_Information, "window", 0, 0, "red", "false");
                                    }
                                }

                                HalconOperatorValueChangedFlag = 0;
                            }
                        })); ;
                    }
                    Thread.Sleep(10);
                }
            }
        }

        private void FitCircleAdjust_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Visible = false;

            //td.Abort();//如果FormClosing时不Abort,会有错误:无法访问已释放的对象
        }

    }
}
