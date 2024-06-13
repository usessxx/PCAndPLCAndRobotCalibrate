using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Runtime.InteropServices;//for [DllImport("Kernel32.dll")]
using HalconDotNet;//C:\Program Files\MVTec\HALCON-18.11-Progress\bin\dotnet35

namespace HalconMVTec
{
    public class HalconMVTec
    {
        [DllImport("Kernel32.dll")]
        //internal static extern void CopyMemory(int dest, int source, int size);
        internal static extern void CopyMemory(Int64 dest, Int64 source, Int64 size);

        public static int LanguageFlag;//用于设定当前语言，0-英文，1-中文

        #region ReadImage:读取图片
        public HObject ho_Image_ReadImage;
        public HTuple hv_ImageWidth_ReadImage;
        public HTuple hv_ImageHeight_ReadImage;
        public HTuple hv_WindowHandle_ReadImage;
        /// <summary>
        /// ReadImage:读取图片
        /// </summary>
        /// <param name="imagePath">string:图片路径</param>
        /// <param name="imageWidth">int out:图片宽度</param>
        /// <param name="imageHeight">int out:图片高度</param>
        /// <returns></returns>
        public void ReadImage(string imagePath, out int imageWidth, out int imageHeight)
        {
            HOperatorSet.ReadImage(out ho_Image_ReadImage, imagePath);
            HOperatorSet.GetImageSize(ho_Image_ReadImage, out hv_ImageWidth_ReadImage, out hv_ImageHeight_ReadImage);
            imageWidth = hv_ImageWidth_ReadImage.I;
            imageHeight = hv_ImageHeight_ReadImage.I;
        }
        #endregion

        #region ReadImageAndDisplay:读取图片并显示
        /// <summary>
        /// ReadImageAndDisplay:读取图片并显示
        /// </summary>
        /// <param name="imagePath">string:图片路径</param>
        /// <returns></returns>
        public void ReadImageAndDisplay(string imagePath, int width, int height, IntPtr fatherWindowhandle)
        {
            HOperatorSet.ReadImage(out ho_Image_ReadImage, imagePath);
            HOperatorSet.GetImageSize(ho_Image_ReadImage, out hv_ImageWidth_ReadImage, out hv_ImageHeight_ReadImage);

            //HOperatorSet.CloseWindow(hv_WindowHandle_Cam);
            //HOperatorSet.CloseWindow(hv_WindowHandle_ReadImage);
            if (hv_WindowHandle_Cam != null)
                hv_WindowHandle_Cam.Dispose();
            if (hv_WindowHandle_ReadImage != null)
                hv_WindowHandle_ReadImage.Dispose();


            HOperatorSet.OpenWindow(0, 0, width, height, fatherWindowhandle, "visible", "", out hv_WindowHandle_ReadImage);//这里不能重复OpenWindows,不然picturebox的图片不更新
            HOperatorSet.DispObj(ho_Image_ReadImage, hv_WindowHandle_ReadImage);
        }
        #endregion

        #region OpenCamera:打开相机(大恒图像USB3相机)
        public HObject ho_Image_Cam = null;
        public HTuple hv_AcqHandle_Cam = null;
        public HTuple hv_WindowHandle_Cam = null;
        public bool cameraOpenedFlag_Cam = false;
        /// <summary>
        /// OpenCamera:打开相机(大恒图像USB3相机)
        /// </summary>
        /// <param name="width">int:图像窗口宽度</param>
        /// <param name="height">int:图像窗口高度</param>
        /// <param name="fatherWindowhandle">IntPtr:图像父窗口控件句柄</param>
        /// <returns></returns>
        public void OpenCamera(int width, int height, IntPtr fatherWindowhandle)
        {
            //HOperatorSet.CloseWindow(hv_WindowHandle_Cam);
            //HOperatorSet.CloseWindow(hv_WindowHandle_ReadImage);
            if (hv_WindowHandle_Cam != null)
                hv_WindowHandle_Cam.Dispose();
            if (hv_WindowHandle_ReadImage != null)
                hv_WindowHandle_ReadImage.Dispose();

            if (hv_AcqHandle_Cam == null)
            {
                try
                {
                    //HOperatorSet.OpenFramegrabber("USB3Vision", 0, 0, 0, 0, 0, 0, "progressive",
                    //    -1, "default", -1, "false", "default", "2BA200004365_DahengImaging_MER50014U3M",
                    //    0, -1, out hv_AcqHandle_Cam);
                    HOperatorSet.OpenFramegrabber("USB3Vision", 0, 0, 0, 0, 0, 0, "progressive",
                    -1, "default", -1, "false", "default", "2BA200003897_DahengImaging_ME2L16161U3M",
                    0, -1, out hv_AcqHandle_Cam);
                    cameraOpenedFlag_Cam = true;
                }
                catch
                {
                    cameraOpenedFlag_Cam = false;
                    if (LanguageFlag == 0)
                        MessageBox.Show("Open camera failed!");
                    else
                        MessageBox.Show("打开相机失败!");
                    return;
                }
            }
            HOperatorSet.GrabImage(out ho_Image_Cam, hv_AcqHandle_Cam);
            HOperatorSet.OpenWindow(0, 0, width, height, fatherWindowhandle, "visible", "", out hv_WindowHandle_Cam);//这里不能重复OpenWindows,不然picturebox的图片不更新
            HOperatorSet.DispObj(ho_Image_Cam, hv_WindowHandle_Cam);
        }
        #endregion

        #region OpenCamera2:打开相机(大恒图像USB3相机)
        /// <summary>
        /// OpenCamera2:打开相机(大恒图像USB3相机)
        /// </summary>
        /// <param name="width">int:图像窗口宽度</param>
        /// <param name="height">int:图像窗口高度</param>
        /// <param name="fatherWindowhandle">IntPtr:图像父窗口控件句柄</param>
        /// <param name="WindowHandle">ref HTuple:图像显示窗口的句柄</param>
        /// <returns></returns>
        public void OpenCamera2(int width, int height, IntPtr fatherWindowhandle, ref HTuple WindowHandle)
        {
            HOperatorSet.CloseFramegrabber(hv_AcqHandle_Cam);
            cameraOpenedFlag_Cam = false;
            HOperatorSet.CloseWindow(WindowHandle);
            HObject ho_Image = null;
            //HOperatorSet.OpenFramegrabber("USB3Vision", 0, 0, 0, 0, 0, 0, "progressive",
            //    -1, "default", -1, "false", "default", "2BA200004365_DahengImaging_MER50014U3M",
            //    0, -1, out hv_AcqHandle_Cam);
            HOperatorSet.OpenFramegrabber("USB3Vision", 0, 0, 0, 0, 0, 0, "progressive",
            -1, "default", -1, "false", "default", "2BA200003897_DahengImaging_ME2L16161U3M",
            0, -1, out hv_AcqHandle_Cam);
            cameraOpenedFlag_Cam = true;
            HOperatorSet.GrabImage(out ho_Image, hv_AcqHandle_Cam);
            HOperatorSet.OpenWindow(0, 0, width, height, fatherWindowhandle, "visible", "", out WindowHandle);
            HOperatorSet.DispObj(ho_Image, WindowHandle);
            ho_Image.Dispose();
        }
        #endregion

        #region OpenCameraAndOutputImage:打开相机(大恒图像USB3相机),输出Halcon格式的图像
        /// <summary>
        /// OpenCameraAndOutputImage:打开相机(大恒图像USB3相机),输出Halcon格式的图像
        /// </summary>
        /// <returns>ho_Image</returns>
        public HObject OpenCameraAndOutputImage()
        {
            HOperatorSet.CloseFramegrabber(hv_AcqHandle_Cam);
            HObject ho_Image = null;
            cameraOpenedFlag_Cam = false;
            //HOperatorSet.OpenFramegrabber("USB3Vision", 0, 0, 0, 0, 0, 0, "progressive",
            //    -1, "default", -1, "false", "default", "2BA200004365_DahengImaging_MER50014U3M",
            //    0, -1, out hv_AcqHandle_Cam);
            HOperatorSet.OpenFramegrabber("USB3Vision", 0, 0, 0, 0, 0, 0, "progressive",
            -1, "default", -1, "false", "default", "2BA200003897_DahengImaging_ME2L16161U3M",
            0, -1, out hv_AcqHandle_Cam);
            cameraOpenedFlag_Cam = true;
            HOperatorSet.GrabImage(out ho_Image, hv_AcqHandle_Cam);
            return ho_Image;
        }
        #endregion

        #region OpenCameraOnly:只是打开相机(大恒图像USB3相机)
        /// <summary>
        /// OpenCameraOnly:只是打开相机(大恒图像USB3相机)
        /// </summary>
        /// <returns></returns>
        public void OpenCameraOnly()
        {
            HOperatorSet.CloseFramegrabber(hv_AcqHandle_Cam);
            cameraOpenedFlag_Cam = false;
            try
            {
                //HOperatorSet.OpenFramegrabber("USB3Vision", 0, 0, 0, 0, 0, 0, "progressive",
                //    -1, "default", -1, "false", "default", "2BA200004365_DahengImaging_MER50014U3M",
                //    0, -1, out hv_AcqHandle_Cam);
                HOperatorSet.OpenFramegrabber("USB3Vision", 0, 0, 0, 0, 0, 0, "progressive",
                -1, "default", -1, "false", "default", "2BA200003897_DahengImaging_ME2L16161U3M",
                0, -1, out hv_AcqHandle_Cam);
                cameraOpenedFlag_Cam = true;
            }
            catch
            {
                hv_AcqHandle_Cam = null;
                if (LanguageFlag == 0)
                    MessageBox.Show("Open camera failed!");
                else
                    MessageBox.Show("打开相机失败!");
            }
        }
        #endregion

        #region GrabImage:相机拍照(大恒图像USB3相机)
        /// <summary>
        /// GrabImage:相机拍照(大恒图像USB3相机)
        /// </summary>
        /// <param name="width">int:图像窗口宽度</param>
        /// <param name="height">int:图像窗口高度</param>
        /// <param name="fatherWindowhandle">IntPtr:图像父窗口控件句柄</param>
        /// <returns>HObject:ho_Image</returns>
        //public HObject GrabImage(int width, int height, IntPtr fatherWindowhandle)
        //{
        //    HObject ho_Image = null;
        //    if (hv_AcqHandle_Cam != null)
        //    {
        //        HOperatorSet.CloseWindow(hv_WindowHandle_Cam);

        //        HOperatorSet.GrabImage(out ho_Image, hv_AcqHandle_Cam);
        //        if (cameraOpenedFlag_Cam)
        //        {
        //            HOperatorSet.CloseWindow(hv_WindowHandle_Cam);
        //            cameraOpenedFlag_Cam = false;
        //        }
        //        if (!cameraOpenedFlag_Cam)
        //        {
        //            HOperatorSet.OpenWindow(0, 0, width, height, fatherWindowhandle, "visible", "", out hv_WindowHandle_Cam);//这里不能重复OpenWindows,不然picturebox的图片不更新
        //            cameraOpenedFlag_Cam = true;
        //        }
        //        HOperatorSet.DispObj(ho_Image, hv_WindowHandle_Cam);
        //    }
        //    else
        //    {
        //        MessageBox.Show("请先打开相机");
        //    }
        //    return ho_Image;
        //}

        //public void GrabImageContinue(int width, int height, IntPtr fatherWindowhandle)
        //{
        //    if (hv_AcqHandle_Cam != null)
        //    {
        //        HOperatorSet.GrabImage(out ho_Image, hv_AcqHandle_Cam);
        //    }
        //    if (hv_WindowHandle_Cam == null)
        //    {
        //        HOperatorSet.OpenWindow(0, 0, width, height, fatherWindowhandle, "visible", "", out hv_WindowHandle_Cam);//这里不能重复OpenWindows,不然picturebox的图片不更新
        //    }
        //    if (hv_WindowHandle_Cam != null)
        //    {
        //        HOperatorSet.DispObj(ho_Image, hv_WindowHandle_Cam);
        //    }
        //}

        public HObject GrabImage()
        {
            HObject ho_Image = null;
            if (hv_AcqHandle_Cam != null && hv_WindowHandle_Cam != null)
            {
                //HOperatorSet.GrabImage(out ho_Image, hv_AcqHandle_Cam);
                //HOperatorSet.DispObj(ho_Image, hv_WindowHandle_Cam);

                HOperatorSet.GrabImage(out ho_Image_Cam, hv_AcqHandle_Cam);
                HOperatorSet.DispObj(ho_Image_Cam, hv_WindowHandle_Cam);

            }
            else
            {
                if (LanguageFlag == 0)
                    MessageBox.Show("Please open camera!");
                else
                    MessageBox.Show("请先打开相机");
            }
            return ho_Image;
        }
        #endregion

        #region GrabImage2:相机拍照(大恒图像USB3相机)
        /// <summary>
        /// GrabImage2_static:相机拍照(大恒图像USB3相机)
        /// </summary>
        /// <param name="width">int:图像窗口宽度</param>
        /// <param name="height">int:图像窗口高度</param>
        /// <param name="fatherWindowhandle">IntPtr:图像父窗口控件句柄</param>
        /// <param name="WindowHandle">ref HTuple:图像显示窗口的句柄</param>
        /// <returns>HObject:ho_Image</returns>
        public HObject GrabImage2(int width, int height, IntPtr fatherWindowhandle, ref HTuple WindowHandle)
        {
            HObject ho_Image = null;
            if (hv_AcqHandle_Cam != null)
            {
                HOperatorSet.CloseWindow(WindowHandle);

                HOperatorSet.GrabImage(out ho_Image, hv_AcqHandle_Cam);
                if (cameraOpenedFlag_Cam)
                {
                    HOperatorSet.CloseWindow(WindowHandle);
                    cameraOpenedFlag_Cam = false;
                }
                if (!cameraOpenedFlag_Cam)
                {
                    HOperatorSet.OpenWindow(0, 0, width, height, fatherWindowhandle, "visible", "", out WindowHandle);//这里不能重复OpenWindows,不然picturebox的图片不更新
                    cameraOpenedFlag_Cam = true;
                }
                HOperatorSet.DispObj(ho_Image, WindowHandle);
            }
            else
            {
                if (LanguageFlag == 0)
                    MessageBox.Show("Please open camera!");
                else
                    MessageBox.Show("请先打开相机");
            }
            return ho_Image;
        }
        #endregion

        #region GrabImage3:相机拍照(大恒图像USB3相机),输出Halcon格式的图像,不显示图像
        public HObject GrabImage3()
        {
            //HObject ho_Image = null;
            if (hv_AcqHandle_Cam != null)
            {
                //HOperatorSet.GrabImage(out ho_Image, hv_AcqHandle_Cam);
                HOperatorSet.GrabImage(out ho_Image_Cam, hv_AcqHandle_Cam);
                //HOperatorSet.DispObj(ho_Image, hv_WindowHandle_Cam);
            }
            else
            {
                if (LanguageFlag == 0)
                    MessageBox.Show("Please open camera!");
                else
                    MessageBox.Show("请先打开相机");
            }
            return ho_Image_Cam;
        }
        #endregion

        #region ConvertHalconGrayByteImageToBitmap:将Halcon 8位灰度图转换为Bitmap图像
        /// <summary>
        /// ConvertHalconGrayByteImageToBitmap:将Halcon 8位灰度图转换为Bitmap图像
        /// </summary>
        /// <param name="ho_Image_Gray">HObject:Halcon 8位灰度图</param>
        /// <param name="bmp">out Bitmap:Bitmap图像</param>
        /// <returns></returns>
        public void ConvertHalconGrayByteImageToBitmap(HObject ho_Image_Gray, out Bitmap bmp)
        {
            HTuple hpoint, type, width, height;
            const int Alpha = 255;
            //int[] ptr = new int[2];
            Int64[] ptr = new Int64[2];
            HOperatorSet.GetImagePointer1(ho_Image_Gray, out hpoint, out type, out width, out height);
            bmp = new Bitmap(width, height, PixelFormat.Format8bppIndexed);
            ColorPalette pal = bmp.Palette;
            for (int i = 0; i <= 255; i++)
            {
                pal.Entries[i] = Color.FromArgb(Alpha, i, i, i);
            }
            bmp.Palette = pal;
            Rectangle rect = new Rectangle(0, 0, width, height);
            BitmapData bitmapData = bmp.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);
            int PixelSize = Bitmap.GetPixelFormatSize(bitmapData.PixelFormat) / 8;
            //ptr[0] = bitmapData.Scan0.ToInt32();
            ptr[0] = bitmapData.Scan0.ToInt64();
            //ptr[1] = hpoint.I;
            ptr[1] = hpoint.L;
            if (width % 4 == 0)
                CopyMemory(ptr[0], ptr[1], width * height * PixelSize);
            else
            {
                for (int i = 0; i < height - 1; i++)
                {
                    ptr[1] += width;
                    CopyMemory(ptr[0], ptr[1], width * PixelSize);
                    ptr[0] += bitmapData.Stride;
                }
            }
            bmp.UnlockBits(bitmapData);
        }
        #endregion

        #region ConvertHalconRGBImageToBitmap:将Halcon RGB图像转换为Bitmap图像----此函数没有Debug,可能有问题
        /// <summary>
        /// ConvertHalconRGBImageToBitmap:将Halcon RGB图像转换为Bitmap图像----此函数没有Debug,可能有问题
        /// </summary>
        /// <param name="ho_Image_RGB">HObject:Halcon RGB图像</param>
        /// <param name="bmp">out Bitmap:Bitmap图像</param>
        public void ConvertHalconRGBImageToBitmap(HObject ho_Image_RGB, out Bitmap bmp)
        {
            HTuple hred, hgreen, hblue, type, width, height;
            HOperatorSet.GetImagePointer3(ho_Image_RGB, out hred, out hgreen, out hblue, out type, out width, out height);
            bmp = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
            Rectangle rect = new Rectangle(0, 0, width, height);
            BitmapData bitmapData = bmp.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format32bppRgb);
            unsafe
            {
                byte* bptr = (byte*)bitmapData.Scan0;
                byte* r = ((byte*)hred.I);
                byte* g = ((byte*)hgreen.I);
                byte* b = ((byte*)hblue.I);
                for (int i = 0; i < width * height; i++)
                {
                    bptr[i * 4] = (b)[i];
                    bptr[i * 4 + 1] = (g)[i];
                    bptr[i * 4 + 2] = (r)[i];
                    bptr[i * 4 + 3] = 255;
                }
            }
            bmp.UnlockBits(bitmapData);
        }
        #endregion

        #region ResizeBitmapImage:修改Bitmap图片尺寸
        /// <summary>
        /// ResizeBitmapImage:修改Bitmap图片尺寸
        /// </summary>
        /// <param name="oldbmp">Bitmap:旧图像</param>
        /// <param name="newWidth">int:修改后的宽度</param>
        /// <param name="newHeight">int:修改后的高度</param>
        /// <returns>Bitmap:修改尺寸后的图像</returns>
        public Bitmap ResizeBitmapImage(Bitmap oldbmp, int newWidth, int newHeight)
        {
            try
            {
                Bitmap b = new Bitmap(newWidth, newHeight);
                Graphics g = Graphics.FromImage(b);
                // 插值算法的质量
                //g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.DrawImage(oldbmp, new Rectangle(0, 0, newWidth, newHeight), new Rectangle(0, 0, oldbmp.Width, oldbmp.Height), GraphicsUnit.Pixel);
                g.Dispose();
                return b;
            }
            catch
            {
                return null;
            }
        }
        #endregion

        #region CreateCircleInspectionSample:创建圆检测模型(创建测量模型区域以及设定在此区域里查找圆的范围)
        public HObject ho_Image_CircleInspectionSample;
        public HTuple hv_WindowHandle_CircleInspectionSample;

        public int picBoxForDisplayWidth_CircleInspectionSample;//显示图片的文件名用
        public int picBoxForDisplayHeight_CircleInspectionSample;//显示图片的文件名用

        public string imageFileName_CircleInspectionSample = "实时图片";//图片文件名
        public string shapeModelImageFileDirectionPath_CircleInspectionSample = "";//圆检测模型参数文件夹路径
        public string shapeModelImageFileName_CircleInspectionSample = "";//圆检测模型参数文件名

        private HTuple hv_Information_CircleInspectionSample = new HTuple();

        private HTuple hv_ModelID_CircleInspectionSample;

        private int circlesCounterFlag = 0;//0,1:只有圆1;2:有圆1圆2
        private int stepFlag_CircleInspectionSample = 0;

        private HTuple hv_ShapeModeDataPath_CircleInspectionSample;
        private HTuple hv_ShapeModeDataFilePath_CircleInspectionSample;
        private HTuple hv_FileHandle_CircleInspectionSample;

        public FitCircleAdjust FitCirAdjWindow = null;
        private Thread fitcircleadjust = null;

        private bool CreatingCircleInspectionSampleFlag_CircleInspectionSample = false;//创建模型时Flag

        //模型与圆的区域的形状选择参数
        private int shapeModelRectangleOrCircleFlag = 0;//0:Rectangle;1:Circle
        private int circle1FindRegionRectangleOrCircleFlag = 0;//0:Rectangle;1:Circle;2:Annulus
        private int circle2FindRegionRectangleOrCircleFlag = 0;//0:Rectangle;1:Circle;2:Annulus
        //用于绘制模型匹配图形并创建模型_矩形
        private HTuple hv_Row_CircleInspectionSample;
        private HTuple hv_Column_CircleInspectionSample;
        private HTuple hv_Phi_CircleInspectionSample;
        private HTuple hv_Length1_CircleInspectionSample;
        private HTuple hv_Length2_CircleInspectionSample;
        private HObject ho_Rectangle_CircleInspectionSample;//模型的矩形区域
        ////用于绘制模型匹配图形并创建模型_圆形
        //private HTuple hv_Row_CircleInspectionSample_Circle;
        //private HTuple hv_Column_CircleInspectionSample_Circle;
        //private HTuple hv_Phi_CircleInspectionSample_Circle;
        //private HTuple hv_Radius1_CircleInspectionSample_Circle;
        //private HTuple hv_Radius2_CircleInspectionSample_Circle;
        //private HObject ho_Circlr_CircleInspectionSample;//模型的圆形区域
        //模型的矩形或者圆形区域与Image的ReduceDomain
        private HObject ho_ImageReduced_CircleInspectionSample;

        //用于绘制查找圆1的矩形区域
        private HTuple hv_FindRegionRow_CircleInspectionSample;
        private HTuple hv_FindRegionColumn_CircleInspectionSample;
        private HTuple hv_FindRegionPhi_CircleInspectionSample;
        private HTuple hv_FindRegionPhi2_CircleInspectionSample;
        private HTuple hv_FindRegionLength1_CircleInspectionSample;
        private HTuple hv_FindRegionLength2_CircleInspectionSample;
        private HObject ho_FindRegionRectangle_CircleInspectionSample;//寻找圆1的矩形区域
        private HObject ho_FindRegionImageReduced_CreateCircleInspectionSample_Rectangle;//寻找圆1的矩形区域与Image的ReduceDomain
        //用于绘制查找圆1的圆形区域
        //private HTuple hv_FindRegionRow_CircleInspectionSample_Circle;
        //private HTuple hv_FindRegionColumn_CircleInspectionSample_Circle;
        //private HTuple hv_FindRegionPhi_CircleInspectionSample_Circle;
        //private HTuple hv_FindRegionRadius1_CircleInspectionSample_Circle;
        //private HTuple hv_FindRegionRadius2_CircleInspectionSample_Circle;
        //private HObject ho_FindRegionCircle_CircleInspectionSample;//寻找圆1的圆形区域
        //private HObject ho_FindRegionImageReduced_CreateCircleInspectionSample_Circle;//寻找圆1的圆形区域与Image的ReduceDomain

        //用于绘制查找圆2的矩形区域
        private HTuple hv_FindRegionRow_CircleInspectionSample_2th;
        private HTuple hv_FindRegionColumn_CircleInspectionSample_2th;
        private HTuple hv_FindRegionPhi_CircleInspectionSample_2th;
        private HTuple hv_FindRegionPhi2_CircleInspectionSample_2th;
        private HTuple hv_FindRegionLength1_CircleInspectionSample_2th;
        private HTuple hv_FindRegionLength2_CircleInspectionSample_2th;
        private HObject ho_FindRegionRectangle_CircleInspectionSample_2th;//寻找圆2的矩形区域
        private HObject ho_FindRegionImageReduced_CreateCircleInspectionSample_Rectangle_2th;//寻找圆2的矩形区域与Image的ReduceDomain
        //用于绘制查找圆2的圆形区域
        //private HTuple hv_FindRegionRow_CircleInspectionSample_Circle_2th;
        //private HTuple hv_FindRegionColumn_CircleInspectionSample_Circle_2th;
        //private HTuple hv_FindRegionPhi_CircleInspectionSample_Circle_2th;
        //private HTuple hv_FindRegionRadius1_CircleInspectionSample_Circle_2th;
        //private HTuple hv_FindRegionRadius2_CircleInspectionSample_Circle_2th;
        //private HObject ho_FindRegionCircle_CircleInspectionSample_2th;//寻找圆2的圆形区域
        //private HObject ho_FindRegionImageReduced_CreateCircleInspectionSample_Circle_2th;//寻找圆2的圆形区域与Image的ReduceDomain

        /// <summary>
        /// CreateCircleInspectionSample:创建圆检测模型(创建测量模型区域以及设定在此区域里查找圆的范围)
        /// </summary>
        public void CreateCircleInspectionSample(int circles_Counter_Flag, int shape_Model_Rectangle_Or_Circle_Flag, int circle1_Find_Region_Rectangle_Or_Circle_Flag, int circle2_Find_Region_Rectangle_Or_Circle_Flag)
        {
            circlesCounterFlag = circles_Counter_Flag;
            shapeModelRectangleOrCircleFlag = shape_Model_Rectangle_Or_Circle_Flag;
            circle1FindRegionRectangleOrCircleFlag = circle1_Find_Region_Rectangle_Or_Circle_Flag;
            circle2FindRegionRectangleOrCircleFlag = circle2_Find_Region_Rectangle_Or_Circle_Flag;

            CreatingCircleInspectionSampleFlag_CircleInspectionSample = true;//重要:创建模型时显示区域用

            if (ho_Image_CircleInspectionSample != null && hv_WindowHandle_CircleInspectionSample != null)
            {
                hv_Information_CircleInspectionSample = new HTuple();//显示消息初始化,清除以前复制的消息
                HOperatorSet.DispObj(ho_Image_CircleInspectionSample, hv_WindowHandle_CircleInspectionSample);//为了清除上次显示的message
                if (shapeModelRectangleOrCircleFlag == 0)
                {
                    if (LanguageFlag == 0)
                        hv_Information_CircleInspectionSample[0] = "Please draw a rectangle to select need set as measurement model region!";
                    else
                        hv_Information_CircleInspectionSample[0] = "请绘制一个矩形框，选中需要设定为测量模型的区域。";
                }
                if (shapeModelRectangleOrCircleFlag == 1)
                {
                    if (LanguageFlag == 0)
                        hv_Information_CircleInspectionSample[0] = "Please draw a circle to select need set as measurement model region!";
                    else
                        hv_Information_CircleInspectionSample[0] = "请绘制一个圆，选中需要设定为测量模型的区域。";
                }
                if (LanguageFlag == 0)
                    hv_Information_CircleInspectionSample[1] = "Click right button to confirm";
                else
                    hv_Information_CircleInspectionSample[1] = "点击右键确认";
                disp_message_Halcon(hv_WindowHandle_CircleInspectionSample, hv_Information_CircleInspectionSample, "image", 0, 0, "yellow", "false");
                //设定显示参数
                HOperatorSet.SetDraw(hv_WindowHandle_CircleInspectionSample, "margin");
                HOperatorSet.SetColor(hv_WindowHandle_CircleInspectionSample, "yellow");
                HOperatorSet.SetLineWidth(hv_WindowHandle_CircleInspectionSample, 1);

                //try
                //{
                //绘制模板匹配图形并创建模板_矩形
                if (shapeModelRectangleOrCircleFlag == 0)
                {
                    HOperatorSet.DrawRectangle2(hv_WindowHandle_CircleInspectionSample, out hv_Row_CircleInspectionSample, out hv_Column_CircleInspectionSample,
                        out hv_Phi_CircleInspectionSample, out hv_Length1_CircleInspectionSample, out hv_Length2_CircleInspectionSample);
                    HOperatorSet.GenRectangle2(out ho_Rectangle_CircleInspectionSample, hv_Row_CircleInspectionSample,
                        hv_Column_CircleInspectionSample, hv_Phi_CircleInspectionSample, hv_Length1_CircleInspectionSample, hv_Length2_CircleInspectionSample);
                }
                //绘制模板匹配图形并创建模板_圆形
                if (shapeModelRectangleOrCircleFlag == 1)
                {
                    HOperatorSet.DrawEllipse(hv_WindowHandle_CircleInspectionSample, out hv_Row_CircleInspectionSample, out hv_Column_CircleInspectionSample,
                       out hv_Phi_CircleInspectionSample, out hv_Length1_CircleInspectionSample, out hv_Length2_CircleInspectionSample);
                    HOperatorSet.GenEllipse(out ho_Rectangle_CircleInspectionSample, hv_Row_CircleInspectionSample,
                        hv_Column_CircleInspectionSample, hv_Phi_CircleInspectionSample, hv_Length1_CircleInspectionSample, hv_Length2_CircleInspectionSample);
                }
                HOperatorSet.ReduceDomain(ho_Image_CircleInspectionSample, ho_Rectangle_CircleInspectionSample, out ho_ImageReduced_CircleInspectionSample);
                HOperatorSet.CreateShapeModel(ho_ImageReduced_CircleInspectionSample, "auto", (new HTuple(-360)).TupleRad(),
                    (new HTuple(360)).TupleRad(), "auto", "auto", "use_polarity", "auto", "auto",
                    out hv_ModelID_CircleInspectionSample);

                //提示信息设置并显示
                hv_Information_CircleInspectionSample = new HTuple();//显示消息初始化,清除以前复制的消息
                HOperatorSet.DispObj(ho_Image_CircleInspectionSample, hv_WindowHandle_CircleInspectionSample);//为了清除上次显示的message

                HOperatorSet.SetColor(hv_WindowHandle_CircleInspectionSample, "yellow");
                HOperatorSet.DispObj(ho_Rectangle_CircleInspectionSample, hv_WindowHandle_CircleInspectionSample);//显示绘制的模型的矩形或者圆形区域

                HOperatorSet.SetColor(hv_WindowHandle_CircleInspectionSample, "violet");
                if (circle1FindRegionRectangleOrCircleFlag == 0)
                {
                    if (LanguageFlag == 0)
                        hv_Information_CircleInspectionSample[0] = "Please draw a rectangle, select the region to search the threaded hole which be set as standard hole.";
                    else
                        hv_Information_CircleInspectionSample[0] = "请绘制一个矩形框，选中需要设定为基准孔位的螺纹孔的查找范围。";
                }
                if (circle1FindRegionRectangleOrCircleFlag == 1)
                {
                    if (LanguageFlag == 0)
                        hv_Information_CircleInspectionSample[0] = "Please draw a circle, select the region to search the threaded hole which be set as standard hole.";
                    else
                        hv_Information_CircleInspectionSample[0] = "请绘制一个圆，选中需要设定为基准孔位的螺纹孔的查找范围。";
                }
                if (circle1FindRegionRectangleOrCircleFlag == 2)
                {
                    if (LanguageFlag == 0)
                        hv_Information_CircleInspectionSample[0] = "Please draw an annulus, select the region to search the threaded hole which be set as standard hole.";
                    else
                        hv_Information_CircleInspectionSample[0] = "请绘制一个圆环，选中需要设定为基准孔位的螺纹孔的查找范围。";
                }

                if (LanguageFlag == 0)
                {
                    hv_Information_CircleInspectionSample[1] = "Attention: Do not select multi threaded hole in search region.";
                    hv_Information_CircleInspectionSample[2] = "Click right button to confirm";
                }
                else
                {
                    hv_Information_CircleInspectionSample[1] = "注意:不要在查找区域中选中多个螺纹孔。";
                    hv_Information_CircleInspectionSample[2] = "点击右键确认！";
                }

                disp_message_Halcon(hv_WindowHandle_CircleInspectionSample, hv_Information_CircleInspectionSample, "image", 0, 0, "violet", "false");

                //绘制查找圆1的矩形区域
                if (circle1FindRegionRectangleOrCircleFlag == 0)
                {
                    HOperatorSet.DrawRectangle2(hv_WindowHandle_CircleInspectionSample, out hv_FindRegionRow_CircleInspectionSample, out hv_FindRegionColumn_CircleInspectionSample,
                        out hv_FindRegionPhi_CircleInspectionSample, out hv_FindRegionLength1_CircleInspectionSample, out hv_FindRegionLength2_CircleInspectionSample);
                    HOperatorSet.GenRectangle2(out ho_FindRegionRectangle_CircleInspectionSample, hv_FindRegionRow_CircleInspectionSample, hv_FindRegionColumn_CircleInspectionSample,
                        hv_FindRegionPhi_CircleInspectionSample, hv_FindRegionLength1_CircleInspectionSample, hv_FindRegionLength2_CircleInspectionSample);
                }
                //绘制查找圆1的圆形区域
                if (circle1FindRegionRectangleOrCircleFlag == 1)
                {
                    HOperatorSet.DrawEllipse(hv_WindowHandle_CircleInspectionSample, out hv_FindRegionRow_CircleInspectionSample, out hv_FindRegionColumn_CircleInspectionSample,
                       out hv_FindRegionPhi_CircleInspectionSample, out hv_FindRegionLength1_CircleInspectionSample, out hv_FindRegionLength2_CircleInspectionSample);
                    HOperatorSet.GenEllipse(out ho_FindRegionRectangle_CircleInspectionSample, hv_FindRegionRow_CircleInspectionSample, hv_FindRegionColumn_CircleInspectionSample,
                        hv_FindRegionPhi_CircleInspectionSample, hv_FindRegionLength1_CircleInspectionSample, hv_FindRegionLength2_CircleInspectionSample);
                }
                //绘制查找圆1的圆环区域
                if (circle1FindRegionRectangleOrCircleFlag == 2)
                {
                    //初始化时内外圆参数
                    HTuple hv_xc = new HTuple();
                    HTuple hv_yc = new HTuple();
                    HTuple hv_r_o = new HTuple();
                    HTuple hv_r_i = new HTuple();
                    HTuple hv_CenterRow = new HTuple();
                    HTuple hv_CenterCol = new HTuple();
                    HTuple hv_RadiusInner = new HTuple();
                    HTuple hv_RadiusOuter = new HTuple();
                    HTuple hv_StartPhi = new HTuple();
                    HTuple hv_EndPhi = new HTuple();
                    HObject ho_WindowImagePre = new HObject();

                    //hv_xc = 100;
                    //hv_yc = 100;
                    //hv_r_o = 100;
                    //hv_r_i = 50;
                    HOperatorSet.GetImageSize(ho_Image_CircleInspectionSample, out hv_xc, out hv_yc);
                    hv_xc = hv_xc / 2;
                    hv_yc = hv_yc / 2;
                    hv_r_o = hv_xc / 2;
                    hv_r_i = hv_yc / 2;

                    HOperatorSet.DumpWindowImage(out ho_WindowImagePre, hv_WindowHandle_CircleInspectionSample);
                    draw_annular(hv_WindowHandle_CircleInspectionSample, hv_Information_CircleInspectionSample, ho_Image_CircleInspectionSample, ho_WindowImagePre, "violet", hv_yc, hv_xc, hv_r_i, hv_r_o, 0, 90, out hv_CenterRow, out hv_CenterCol, out hv_RadiusInner, out hv_RadiusOuter, out hv_StartPhi, out hv_EndPhi);

                    HObject circleSector1 = new HObject();
                    HObject circleSector2 = new HObject();

                    double angle = hv_EndPhi.TupleReal() - hv_StartPhi.TupleReal();
                    if (angle < 0)
                        angle = angle * -1;
                    if (angle != 360 && angle != 0)
                    {
                        HOperatorSet.GenCircleSector(out circleSector1, hv_CenterRow, hv_CenterCol, hv_RadiusInner, hv_EndPhi.TupleRad(), hv_StartPhi.TupleRad());
                        HOperatorSet.GenCircleSector(out circleSector2, hv_CenterRow, hv_CenterCol, hv_RadiusOuter, hv_EndPhi.TupleRad(), hv_StartPhi.TupleRad());
                    }
                    if (angle == 360 || angle == 0)
                    {
                        HOperatorSet.GenEllipse(out circleSector1, hv_CenterRow, hv_CenterCol, 0, hv_RadiusInner, hv_RadiusInner);
                        HOperatorSet.GenEllipse(out circleSector2, hv_CenterRow, hv_CenterCol, 0, hv_RadiusOuter, hv_RadiusOuter);
                    }

                    hv_FindRegionRow_CircleInspectionSample = hv_CenterRow;
                    hv_FindRegionColumn_CircleInspectionSample = hv_CenterCol;
                    hv_FindRegionPhi_CircleInspectionSample = hv_StartPhi;
                    hv_FindRegionPhi2_CircleInspectionSample = hv_EndPhi;
                    hv_FindRegionLength1_CircleInspectionSample = hv_RadiusInner;
                    hv_FindRegionLength2_CircleInspectionSample = hv_RadiusOuter;

                    HOperatorSet.Difference(circleSector2, circleSector1, out ho_FindRegionRectangle_CircleInspectionSample);
                    HOperatorSet.SetDraw(hv_WindowHandle_CircleInspectionSample, "margin");
                }
                HOperatorSet.ReduceDomain(ho_Image_CircleInspectionSample, ho_FindRegionRectangle_CircleInspectionSample, out ho_FindRegionImageReduced_CreateCircleInspectionSample_Rectangle);

                displayRegion1();

                if (circlesCounterFlag == 2)
                {
                    HOperatorSet.SetColor(hv_WindowHandle_CircleInspectionSample, "cyan");
                    if (circle2FindRegionRectangleOrCircleFlag == 0)
                    {
                        if (LanguageFlag == 0)
                            hv_Information_CircleInspectionSample[0] = "Please draw a rectangle, select the region to search the threaded hole which be set as standard hole.";
                        else
                            hv_Information_CircleInspectionSample[0] = "请绘制一个矩形框，选中需要设定为基准孔位的螺纹孔的查找范围。";
                    }
                    if (circle2FindRegionRectangleOrCircleFlag == 1)
                    {
                        if (LanguageFlag == 0)
                            hv_Information_CircleInspectionSample[0] = "Please draw a circle, select the region to search the threaded hole which be set as standard hole.";
                        else
                            hv_Information_CircleInspectionSample[0] = "请绘制一个圆，选中需要设定为基准孔位的螺纹孔的查找范围。";
                    }
                    if (circle2FindRegionRectangleOrCircleFlag == 2)
                    {
                        if (LanguageFlag == 0)
                            hv_Information_CircleInspectionSample[0] = "Please draw an annulus, select the region to search the threaded hole which be set as standard hole.";
                        else
                            hv_Information_CircleInspectionSample[0] = "请绘制一个圆环，选中需要设定为基准孔位的螺纹孔的查找范围。";
                    }

                    if (LanguageFlag == 0)
                    {
                        hv_Information_CircleInspectionSample[1] = "Attention: Do not select multi threaded hole in search region.";
                        hv_Information_CircleInspectionSample[2] = "Click right button to confirm";
                    }
                    else
                    {
                        hv_Information_CircleInspectionSample[1] = "注意:不要在查找区域中选中多个螺纹孔。";
                        hv_Information_CircleInspectionSample[2] = "点击右键确认！";
                    }

                    disp_message_Halcon(hv_WindowHandle_CircleInspectionSample, hv_Information_CircleInspectionSample, "image", 0, 0, "cyan", "false");

                    //绘制查找圆2的矩形区域
                    if (circle2FindRegionRectangleOrCircleFlag == 0)
                    {
                        HOperatorSet.DrawRectangle2(hv_WindowHandle_CircleInspectionSample, out hv_FindRegionRow_CircleInspectionSample_2th, out hv_FindRegionColumn_CircleInspectionSample_2th,
                            out hv_FindRegionPhi_CircleInspectionSample_2th, out hv_FindRegionLength1_CircleInspectionSample_2th, out hv_FindRegionLength2_CircleInspectionSample_2th);
                        HOperatorSet.GenRectangle2(out ho_FindRegionRectangle_CircleInspectionSample_2th, hv_FindRegionRow_CircleInspectionSample_2th, hv_FindRegionColumn_CircleInspectionSample_2th,
                            hv_FindRegionPhi_CircleInspectionSample_2th, hv_FindRegionLength1_CircleInspectionSample_2th, hv_FindRegionLength2_CircleInspectionSample_2th);
                    }
                    //绘制查找圆2的圆形区域
                    if (circle2FindRegionRectangleOrCircleFlag == 1)
                    {
                        HOperatorSet.DrawEllipse(hv_WindowHandle_CircleInspectionSample, out hv_FindRegionRow_CircleInspectionSample_2th, out hv_FindRegionColumn_CircleInspectionSample_2th,
                            out hv_FindRegionPhi_CircleInspectionSample_2th, out hv_FindRegionLength1_CircleInspectionSample_2th, out hv_FindRegionLength2_CircleInspectionSample_2th);
                        HOperatorSet.GenEllipse(out ho_FindRegionRectangle_CircleInspectionSample_2th, hv_FindRegionRow_CircleInspectionSample_2th, hv_FindRegionColumn_CircleInspectionSample_2th,
                            hv_FindRegionPhi_CircleInspectionSample_2th, hv_FindRegionLength1_CircleInspectionSample_2th, hv_FindRegionLength2_CircleInspectionSample_2th);
                    }
                    //绘制查找圆2的圆环区域
                    if (circle2FindRegionRectangleOrCircleFlag == 2)
                    {
                        //初始化时内外圆参数
                        HTuple hv_xc = new HTuple();
                        HTuple hv_yc = new HTuple();
                        HTuple hv_r_o = new HTuple();
                        HTuple hv_r_i = new HTuple();
                        HTuple hv_CenterRow = new HTuple();
                        HTuple hv_CenterCol = new HTuple();
                        HTuple hv_RadiusInner = new HTuple();
                        HTuple hv_RadiusOuter = new HTuple();
                        HTuple hv_StartPhi = new HTuple();
                        HTuple hv_EndPhi = new HTuple();
                        HObject ho_WindowImagePre = new HObject();

                        //hv_xc = 100;
                        //hv_yc = 100;
                        //hv_r_o = 100;
                        //hv_r_i = 50;
                        HOperatorSet.GetImageSize(ho_Image_CircleInspectionSample, out hv_xc, out hv_yc);
                        hv_xc = hv_xc / 2;
                        hv_yc = hv_yc / 2;
                        hv_r_o = hv_xc / 2;
                        hv_r_i = hv_yc / 2;

                        HOperatorSet.DumpWindowImage(out ho_WindowImagePre, hv_WindowHandle_CircleInspectionSample);
                        draw_annular(hv_WindowHandle_CircleInspectionSample, hv_Information_CircleInspectionSample, ho_Image_CircleInspectionSample, ho_WindowImagePre, "cyan", hv_yc, hv_xc, hv_r_i, hv_r_o, 0, 90, out hv_CenterRow, out hv_CenterCol, out hv_RadiusInner, out hv_RadiusOuter, out hv_StartPhi, out hv_EndPhi);

                        HObject circleSector1 = new HObject();
                        HObject circleSector2 = new HObject();

                        double angle = hv_EndPhi.TupleReal() - hv_StartPhi.TupleReal();
                        if (angle < 0)
                            angle = angle * -1;
                        if (angle != 360 && angle != 0)
                        {
                            HOperatorSet.GenCircleSector(out circleSector1, hv_CenterRow, hv_CenterCol, hv_RadiusInner, hv_EndPhi.TupleRad(), hv_StartPhi.TupleRad());
                            HOperatorSet.GenCircleSector(out circleSector2, hv_CenterRow, hv_CenterCol, hv_RadiusOuter, hv_EndPhi.TupleRad(), hv_StartPhi.TupleRad());
                        }
                        if (angle == 360 || angle == 0)
                        {
                            HOperatorSet.GenEllipse(out circleSector1, hv_CenterRow, hv_CenterCol, 0, hv_RadiusInner, hv_RadiusInner);
                            HOperatorSet.GenEllipse(out circleSector2, hv_CenterRow, hv_CenterCol, 0, hv_RadiusOuter, hv_RadiusOuter);
                        }

                        hv_FindRegionRow_CircleInspectionSample_2th = hv_CenterRow;
                        hv_FindRegionColumn_CircleInspectionSample_2th = hv_CenterCol;
                        hv_FindRegionPhi_CircleInspectionSample_2th = hv_StartPhi;
                        hv_FindRegionPhi2_CircleInspectionSample_2th = hv_EndPhi;
                        hv_FindRegionLength1_CircleInspectionSample_2th = hv_RadiusInner;
                        hv_FindRegionLength2_CircleInspectionSample_2th = hv_RadiusOuter;

                        HOperatorSet.Difference(circleSector2, circleSector1, out ho_FindRegionRectangle_CircleInspectionSample_2th);
                        HOperatorSet.SetDraw(hv_WindowHandle_CircleInspectionSample, "margin");
                    }
                    HOperatorSet.ReduceDomain(ho_Image_CircleInspectionSample, ho_FindRegionRectangle_CircleInspectionSample_2th, out ho_FindRegionImageReduced_CreateCircleInspectionSample_Rectangle_2th);
                }

                //显示调试窗口
                if (FitCirAdjWindow == null)
                    FitCirAdjWindow = new FitCircleAdjust();
                FitCirAdjWindow.CircleInspectionAdjustDisplay = this;
                FitCirAdjWindow.circlesCounterFlag = this.circlesCounterFlag;
                FitCirAdjWindow.stepFlag_CircleInspectionSample = this.stepFlag_CircleInspectionSample;
                FitCirAdjWindow.HalconOperatorValueChangedFlag = 1;
                FitCirAdjWindow.Show();

                if (fitcircleadjust == null)//重要:不然会重复开启多个线程  下一步:取消参数!
                {
                    fitcircleadjust = new Thread(FitCirAdj);
                    fitcircleadjust.IsBackground = true;
                    fitcircleadjust.Start(ho_FindRegionImageReduced_CreateCircleInspectionSample_Rectangle);
                }

                displayRegion2();

                //}
                //catch
                //{
                //}
            }
            else
            {
                if (LanguageFlag == 0)
                    MessageBox.Show("Please load measurement model image", "Hint");
                else
                    MessageBox.Show("请先读取测量模型图片", "提示");
            }
        }
        #endregion

        #region FitCirAdj
        private void FitCirAdj(object o)
        {
            hv_Information_CircleInspectionSample = new HTuple();//显示消息初始化,清除以前复制的消息
            HOperatorSet.DispObj(ho_Image_CircleInspectionSample, hv_WindowHandle_CircleInspectionSample);//为了清除上次显示的message

            displayRegion2();

            if (LanguageFlag == 0)
                hv_Information_CircleInspectionSample[0] = "Fitting circle...";
            else
                hv_Information_CircleInspectionSample[0] = "拟合圆...";
            disp_message_Halcon(hv_WindowHandle_CircleInspectionSample, hv_Information_CircleInspectionSample, "image", 0, 0, "yellow", "false");

            while (true)
            {
                if (CreatingCircleInspectionSampleFlag_CircleInspectionSample)
                {
                    if (FitCirAdjWindow.HalconOperatorValueChangedFlag == 1)
                    {
                        ho_Image_FitCircle = ho_Image_CircleInspectionSample;
                        hv_WindowHandle_FitCircle = hv_WindowHandle_CircleInspectionSample;

                        stepFlag_CircleInspectionSample = 0;
                        HFitCircle((HObject)o, FitCirAdjWindow);
                        stepFlag_CircleInspectionSample++;
                        if (circlesCounterFlag == 2)
                        {
                            HFitCircle((HObject)o, FitCirAdjWindow);
                            stepFlag_CircleInspectionSample++;
                        }
                        FitCirAdjWindow.HalconOperatorValueChangedFlag = 2;
                    }
                }
                Thread.Sleep(10);
            }
        }
        #endregion

        #region HFitCircle:拟合圆
        public HObject ho_Image_FitCircle;
        public HTuple hv_WindowHandle_FitCircle;

        public HObject ho_Circle_FitCircle;
        public HObject ho_Circle_FitCircle_2th;

        public HObject ho_ImageScaled_FitCircle;
        public HObject ho_Regions_FitCircle;
        public HObject ho_RegionBorder_FitCircle;
        public HObject ho_RegionDilation_FitCircle;
        public HObject ho_ImageReduced1_FitCircle;
        public HObject ho_SelectedXLD_FitCircle;
        public HObject ho_UnionContours_FitCircle;
        public int FitCircleStepNo_FitCircle = 0;

        public HObject ho_ImageScaled_FitCircle_2th;
        public HObject ho_Regions_FitCircle_2th;
        public HObject ho_RegionBorder_FitCircle_2th;
        public HObject ho_RegionDilation_FitCircle_2th;
        public HObject ho_ImageReduced1_FitCircle_2th;
        public HObject ho_SelectedXLD_FitCircle_2th;
        public HObject ho_UnionContours_FitCircle_2th;
        public int FitCircleStepNo_FitCircle_2th = 0;

        private HObject ho_ConnectedRegions_FitCircle;
        private HObject ho_SelectedRegions_FitCircle;
        private HObject ho_RegionFillUp_FitCircle;

        private HObject ho_ConnectedRegions_FitCircle_2th;
        private HObject ho_SelectedRegions_FitCircle_2th;
        private HObject ho_RegionFillUp_FitCircle_2th;

        public HObject ho_Edges_FitCircle;
        private HTuple hv_TempArea_FitCircle;
        private HTuple hv_TempRow_FitCircle;
        private HTuple hv_TempColumn_FitCircle;
        private HTuple hv_PointOrder_FitCircle;

        public HObject ho_Edges_FitCircle_2th;
        private HTuple hv_TempArea_FitCircle_2th;
        private HTuple hv_TempRow_FitCircle_2th;
        private HTuple hv_TempColumn_FitCircle_2th;
        private HTuple hv_PointOrder_FitCircle_2th;

        private HTuple hv_circleData_FitCircle;
        private HTuple hv_CircleRow_FitCircle;
        private HTuple hv_CircleColumn_FitCircle;
        private HTuple hv_CircleRadius_FitCircle;
        private HTuple hv_CircleStartPhi_FitCircle;
        private HTuple hv_CircleEndPhi_FitCircle;
        private HTuple hv_PointOrder1_FitCircle;

        private HTuple hv_circleData_FitCircle_2th;
        private HTuple hv_CircleRow_FitCircle_2th;
        private HTuple hv_CircleColumn_FitCircle_2th;
        private HTuple hv_CircleRadius_FitCircle_2th;
        private HTuple hv_CircleStartPhi_FitCircle_2th;
        private HTuple hv_CircleEndPhi_FitCircle_2th;
        private HTuple hv_PointOrder1_FitCircle_2th;

        public HTuple HFitCircle(HObject ho_FindRegionImageReduced, FitCircleAdjust fitciradjwindow)
        {
            HTuple HT = new HTuple();

            if (CreatingCircleInspectionSampleFlag_CircleInspectionSample)
            {
                if (stepFlag_CircleInspectionSample == 0)//拟合圆1
                {
                    ho_FindRegionImageReduced = ho_FindRegionImageReduced_CreateCircleInspectionSample_Rectangle;

                }

                if (circlesCounterFlag == 2 && stepFlag_CircleInspectionSample == 1)//拟合圆2
                {
                    ho_FindRegionImageReduced = ho_FindRegionImageReduced_CreateCircleInspectionSample_Rectangle_2th;
                }
            }

            if (stepFlag_CircleInspectionSample == 0)
                ho_Circle_FitCircle = null;
            ho_Circle_FitCircle_2th = null;

            if (stepFlag_CircleInspectionSample == 0)
            {
                ho_ImageScaled_FitCircle = null;
                ho_Regions_FitCircle = null;
                ho_RegionBorder_FitCircle = null;
                ho_RegionDilation_FitCircle = null;
                ho_ImageReduced1_FitCircle = null;
                ho_SelectedXLD_FitCircle = null;
                ho_UnionContours_FitCircle = null;
                FitCircleStepNo_FitCircle = 0;
            }

            ho_ImageScaled_FitCircle_2th = null;
            ho_Regions_FitCircle_2th = null;
            ho_RegionBorder_FitCircle_2th = null;
            ho_RegionDilation_FitCircle_2th = null;
            ho_ImageReduced1_FitCircle_2th = null;
            ho_SelectedXLD_FitCircle_2th = null;
            ho_UnionContours_FitCircle_2th = null;
            FitCircleStepNo_FitCircle_2th = 0;

            if (stepFlag_CircleInspectionSample == 0)
            {
                ho_ConnectedRegions_FitCircle = null;
                ho_SelectedRegions_FitCircle = null;
                ho_RegionFillUp_FitCircle = null;
            }

            ho_ConnectedRegions_FitCircle_2th = null;
            ho_SelectedRegions_FitCircle_2th = null;
            ho_RegionFillUp_FitCircle_2th = null;

            if (stepFlag_CircleInspectionSample == 0)
            {
                ho_Edges_FitCircle = null;
                hv_TempArea_FitCircle = null;
                hv_TempRow_FitCircle = null;
                hv_TempColumn_FitCircle = null;
                hv_PointOrder_FitCircle = null;
            }

            ho_Edges_FitCircle_2th = null;
            hv_TempArea_FitCircle_2th = null;
            hv_TempRow_FitCircle_2th = null;
            hv_TempColumn_FitCircle_2th = null;
            hv_PointOrder_FitCircle_2th = null;

            if (stepFlag_CircleInspectionSample == 0)
            {
                hv_CircleRow_FitCircle = null;
                hv_CircleColumn_FitCircle = null;
                hv_CircleRadius_FitCircle = null;
                hv_CircleStartPhi_FitCircle = null;
                hv_CircleEndPhi_FitCircle = null;
                hv_PointOrder1_FitCircle = null;
            }

            hv_CircleRow_FitCircle_2th = null;
            hv_CircleColumn_FitCircle_2th = null;
            hv_CircleRadius_FitCircle_2th = null;
            hv_CircleStartPhi_FitCircle_2th = null;
            hv_CircleEndPhi_FitCircle_2th = null;
            hv_PointOrder1_FitCircle_2th = null;

            if (stepFlag_CircleInspectionSample == 0)
                hv_circleData_FitCircle = null;
            hv_circleData_FitCircle_2th = null;

            try
            {
                if (stepFlag_CircleInspectionSample == 0)
                {
                    FitCircleStepNo_FitCircle = 0;
                    HOperatorSet.ScaleImage(ho_FindRegionImageReduced, out ho_ImageScaled_FitCircle, fitciradjwindow.scale_image_numeric1, fitciradjwindow.scale_image_numeric2);
                    FitCircleStepNo_FitCircle = 1;
                    HOperatorSet.Threshold(ho_ImageScaled_FitCircle, out ho_Regions_FitCircle, fitciradjwindow.threshold_numeric1, fitciradjwindow.threshold_numeric2);
                    FitCircleStepNo_FitCircle = 2;
                    HOperatorSet.Connection(ho_Regions_FitCircle, out ho_ConnectedRegions_FitCircle);
                    HOperatorSet.SelectShape(ho_ConnectedRegions_FitCircle, out ho_SelectedRegions_FitCircle, "area", "and", fitciradjwindow.select_shape_numeric1, fitciradjwindow.select_shape_numeric2);
                    HOperatorSet.FillUp(ho_SelectedRegions_FitCircle, out ho_RegionFillUp_FitCircle);
                    HOperatorSet.Boundary(ho_RegionFillUp_FitCircle, out ho_RegionBorder_FitCircle, "inner");
                    FitCircleStepNo_FitCircle = 3;
                    if (ho_RegionBorder_FitCircle.CountObj() > 0)
                        HOperatorSet.DilationCircle(ho_RegionBorder_FitCircle, out ho_RegionDilation_FitCircle, fitciradjwindow.dilation_circle_numeri1);
                    else
                    {
                        if (stepFlag_CircleInspectionSample == 0 && FitCircleStepNo_FitCircle == 3)
                        {
                            HOperatorSet.DispObj(ho_Image_FitCircle, hv_WindowHandle_FitCircle);//为了清除上次显示的message
                            if (ho_ImageScaled_FitCircle.CountObj() > 0)
                                HOperatorSet.DispObj(ho_ImageScaled_FitCircle, hv_WindowHandle_FitCircle);
                            if (ho_RegionBorder_FitCircle.CountObj() > 0)
                                HOperatorSet.DispObj(ho_RegionBorder_FitCircle, hv_WindowHandle_FitCircle);
                            if (circlesCounterFlag < 2)
                            {
                                hv_Information_CircleInspectionSample = new HTuple();//显示消息初始化,清除以前复制的消息
                                if (LanguageFlag == 0)
                                {
                                    hv_Information_CircleInspectionSample[0] = "Fitting circle 1 failed! Failure at step 3!";
                                    hv_Information_CircleInspectionSample[1] = "Offset angle:" + realTimeShapeModelOffsideAngle;
                                }
                                else
                                {
                                    hv_Information_CircleInspectionSample[0] = "拟合圆1失败!失败在第3步!";
                                    hv_Information_CircleInspectionSample[1] = "偏移角度:" + realTimeShapeModelOffsideAngle;
                                }
                                disp_message_Halcon(hv_WindowHandle_FitCircle, hv_Information_CircleInspectionSample, "image", 0, 0, "red", "false");
                            }
                        }
                        if (circlesCounterFlag < 2)
                        {
                            //显示图片的文件名
                            disp_message_Halcon(hv_WindowHandle_CircleInspectionSample, imageFileName_CircleInspectionSample, "window", 0, picBoxForDisplayWidth_CircleInspectionSample - imageFileName_CircleInspectionSample.Length * 8, "yellow", "false");
                        }
                        return HT;
                    }
                    HOperatorSet.ReduceDomain(ho_ImageScaled_FitCircle, ho_RegionDilation_FitCircle, out ho_ImageReduced1_FitCircle);
                    int k1 = ho_ImageReduced1_FitCircle.CountObj();
                    FitCircleStepNo_FitCircle = 4;
                    HOperatorSet.EdgesSubPix(ho_ImageReduced1_FitCircle, out ho_Edges_FitCircle, "canny", 1, fitciradjwindow.edges_sub_pix_numeric1, fitciradjwindow.edges_sub_pix_numeric2);
                    int k2 = ho_Edges_FitCircle.CountObj();
                    HOperatorSet.AreaCenterXld(ho_Edges_FitCircle, out hv_TempArea_FitCircle, out hv_TempRow_FitCircle, out hv_TempColumn_FitCircle, out hv_PointOrder_FitCircle);
                    int k3 = hv_TempArea_FitCircle.Length;
                    HOperatorSet.SelectShapeXld(ho_Edges_FitCircle, out ho_SelectedXLD_FitCircle, "area", "and", hv_TempArea_FitCircle.TupleMax(), 999999999);
                    int k4 = ho_SelectedXLD_FitCircle.CountObj();
                    FitCircleStepNo_FitCircle = 5;
                    HOperatorSet.UnionCocircularContoursXld(ho_SelectedXLD_FitCircle, out ho_UnionContours_FitCircle, (new HTuple(fitciradjwindow.union_cocircular_contours_xld_numeric1)).TupleRad(), (new HTuple(fitciradjwindow.union_cocircular_contours_xld_numeric2)).TupleRad(), (new HTuple(fitciradjwindow.union_cocircular_contours_xld_numeric3)).TupleRad(), fitciradjwindow.union_cocircular_contours_xld_numeric4, fitciradjwindow.union_cocircular_contours_xld_numeric5, fitciradjwindow.union_cocircular_contours_xld_numeric6, "true", 1);
                    FitCircleStepNo_FitCircle = 6;
                    HOperatorSet.DispObj(ho_Image_FitCircle, hv_WindowHandle_FitCircle);//为了清除上次显示的message
                    HOperatorSet.FitCircleContourXld(ho_UnionContours_FitCircle, "algebraic", -1, 0, 0, 3, 2, out hv_CircleRow_FitCircle, out hv_CircleColumn_FitCircle, out hv_CircleRadius_FitCircle, out hv_CircleStartPhi_FitCircle, out hv_CircleEndPhi_FitCircle, out hv_PointOrder1_FitCircle);
                }

                if (circlesCounterFlag == 2 && stepFlag_CircleInspectionSample == 1)
                {
                    FitCircleStepNo_FitCircle_2th = 0;
                    HOperatorSet.ScaleImage(ho_FindRegionImageReduced, out ho_ImageScaled_FitCircle_2th, fitciradjwindow.scale_image_numeric1_2th, fitciradjwindow.scale_image_numeric2_2th);
                    FitCircleStepNo_FitCircle_2th = 1;
                    HOperatorSet.Threshold(ho_ImageScaled_FitCircle_2th, out ho_Regions_FitCircle_2th, fitciradjwindow.threshold_numeric1_2th, fitciradjwindow.threshold_numeric2_2th);
                    FitCircleStepNo_FitCircle_2th = 2;
                    HOperatorSet.Connection(ho_Regions_FitCircle_2th, out ho_ConnectedRegions_FitCircle_2th);
                    HOperatorSet.SelectShape(ho_ConnectedRegions_FitCircle_2th, out ho_SelectedRegions_FitCircle_2th, "area", "and", fitciradjwindow.select_shape_numeric1_2th, fitciradjwindow.select_shape_numeric2_2th);
                    HOperatorSet.FillUp(ho_SelectedRegions_FitCircle_2th, out ho_RegionFillUp_FitCircle_2th);
                    HOperatorSet.Boundary(ho_RegionFillUp_FitCircle_2th, out ho_RegionBorder_FitCircle_2th, "inner");
                    FitCircleStepNo_FitCircle_2th = 3;
                    if (ho_RegionBorder_FitCircle_2th.CountObj() > 0)
                        HOperatorSet.DilationCircle(ho_RegionBorder_FitCircle_2th, out ho_RegionDilation_FitCircle_2th, fitciradjwindow.dilation_circle_numeri1_2th);
                    else
                    {
                        if (hv_circleData_FitCircle == null)
                        {
                            HOperatorSet.DispObj(ho_Image_FitCircle, hv_WindowHandle_FitCircle);//为了清除上次显示的message
                            hv_Information_CircleInspectionSample = new HTuple();//显示消息初始化,清除以前复制的消息
                            if (LanguageFlag == 0)
                            {
                                hv_Information_CircleInspectionSample[0] = "Fitting circle 1 failed! Failure at step " + FitCircleStepNo_FitCircle + "!";
                                hv_Information_CircleInspectionSample[1] = "Offset angle:" + realTimeShapeModelOffsideAngle;
                            }
                            else
                            {
                                hv_Information_CircleInspectionSample[0] = "拟合圆1失败!失败在第" + FitCircleStepNo_FitCircle + "步!";
                                hv_Information_CircleInspectionSample[1] = "偏移角度:" + realTimeShapeModelOffsideAngle;
                            }
                            disp_message_Halcon(hv_WindowHandle_FitCircle, hv_Information_CircleInspectionSample, "image", 0, 0, "red", "false");
                        }
                        if (hv_circleData_FitCircle != null)
                        {
                            hv_Information_CircleInspectionSample = new HTuple();//显示消息初始化,清除以前复制的消息
                            if (LanguageFlag == 0)
                            {
                                hv_Information_CircleInspectionSample[0] = "Fitting circle 1 successful!";
                                hv_Information_CircleInspectionSample[1] = "Circle 1 center coordination: " + "(" + hv_circleData_FitCircle[0] + "," + hv_circleData_FitCircle[1] + ")" + ",Radius:" + hv_circleData_FitCircle[2];
                                hv_Information_CircleInspectionSample[2] = "Offset angle:" + realTimeShapeModelOffsideAngle;
                            }
                            else
                            {
                                hv_Information_CircleInspectionSample[0] = "拟合圆1成功";
                                hv_Information_CircleInspectionSample[1] = "圆心1坐标:" + "(" + hv_circleData_FitCircle[0] + "," + hv_circleData_FitCircle[1] + ")" + ",半径:" + hv_circleData_FitCircle[2];
                                hv_Information_CircleInspectionSample[2] = "偏移角度:" + realTimeShapeModelOffsideAngle;
                            }
                            disp_message_Halcon(hv_WindowHandle_CircleInspectionSample, hv_Information_CircleInspectionSample, "image", 0, 0, "green", "false");
                            disp_message_Halcon(hv_WindowHandle_CircleInspectionSample, 1, "image", hv_circleData_FitCircle[0], hv_circleData_FitCircle[1], "green", "false");
                        }
                        if (circlesCounterFlag == 2 && stepFlag_CircleInspectionSample == 1 && FitCircleStepNo_FitCircle_2th == 3)
                        {
                            if (ho_ImageScaled_FitCircle_2th.CountObj() > 0)
                                HOperatorSet.DispObj(ho_ImageScaled_FitCircle_2th, hv_WindowHandle_FitCircle);
                            if (ho_RegionBorder_FitCircle_2th.CountObj() > 0)
                                HOperatorSet.DispObj(ho_RegionBorder_FitCircle_2th, hv_WindowHandle_FitCircle);
                            hv_Information_CircleInspectionSample = new HTuple();//显示消息初始化,清除以前复制的消息

                            if (LanguageFlag == 0)
                            {
                                hv_Information_CircleInspectionSample[0] = "Fitting circle 2 failed! Failure at step 3!";
                                hv_Information_CircleInspectionSample[1] = "Offset angle:" + realTimeShapeModelOffsideAngle;
                            }
                            else
                            {
                                hv_Information_CircleInspectionSample[0] = "拟合圆2失败!失败在第3步!";
                                hv_Information_CircleInspectionSample[1] = "偏移角度:" + realTimeShapeModelOffsideAngle;
                            }

                            disp_message_Halcon(hv_WindowHandle_FitCircle, hv_Information_CircleInspectionSample, "image", 0, 200, "red", "false");
                        }
                        //显示图片的文件名
                        disp_message_Halcon(hv_WindowHandle_CircleInspectionSample, imageFileName_CircleInspectionSample, "window", 0, picBoxForDisplayWidth_CircleInspectionSample - imageFileName_CircleInspectionSample.Length * 8, "yellow", "false");
                        return HT;
                    }
                    HOperatorSet.ReduceDomain(ho_ImageScaled_FitCircle_2th, ho_RegionDilation_FitCircle_2th, out ho_ImageReduced1_FitCircle_2th);
                    FitCircleStepNo_FitCircle_2th = 4;
                    HOperatorSet.EdgesSubPix(ho_ImageReduced1_FitCircle_2th, out ho_Edges_FitCircle_2th, "canny", 1, fitciradjwindow.edges_sub_pix_numeric1_2th, fitciradjwindow.edges_sub_pix_numeric2_2th);
                    HOperatorSet.AreaCenterXld(ho_Edges_FitCircle_2th, out hv_TempArea_FitCircle_2th, out hv_TempRow_FitCircle_2th, out hv_TempColumn_FitCircle_2th, out hv_PointOrder_FitCircle_2th);
                    HOperatorSet.SelectShapeXld(ho_Edges_FitCircle_2th, out ho_SelectedXLD_FitCircle_2th, "area", "and", hv_TempArea_FitCircle_2th.TupleMax(), 999999999);
                    FitCircleStepNo_FitCircle_2th = 5;
                    HOperatorSet.UnionCocircularContoursXld(ho_SelectedXLD_FitCircle_2th, out ho_UnionContours_FitCircle_2th, (new HTuple(fitciradjwindow.union_cocircular_contours_xld_numeric1_2th)).TupleRad(), (new HTuple(fitciradjwindow.union_cocircular_contours_xld_numeric2_2th)).TupleRad(), (new HTuple(fitciradjwindow.union_cocircular_contours_xld_numeric3_2th)).TupleRad(), fitciradjwindow.union_cocircular_contours_xld_numeric4_2th, fitciradjwindow.union_cocircular_contours_xld_numeric5_2th, fitciradjwindow.union_cocircular_contours_xld_numeric6_2th, "true", 1);
                    FitCircleStepNo_FitCircle_2th = 6;
                    HOperatorSet.FitCircleContourXld(ho_UnionContours_FitCircle_2th, "algebraic", -1, 0, 0, 3, 2, out hv_CircleRow_FitCircle_2th, out hv_CircleColumn_FitCircle_2th, out hv_CircleRadius_FitCircle_2th, out hv_CircleStartPhi_FitCircle_2th, out hv_CircleEndPhi_FitCircle_2th, out hv_PointOrder1_FitCircle_2th);
                }

                if (hv_circleData_FitCircle == null && stepFlag_CircleInspectionSample == 0)
                {
                    hv_circleData_FitCircle = new HTuple();
                    //hv_circleData_FitCircle[0] = hv_CircleRow_FitCircle;
                    //hv_circleData_FitCircle[1] = hv_CircleColumn_FitCircle;
                    hv_circleData_FitCircle[0] = hv_CircleColumn_FitCircle;
                    hv_circleData_FitCircle[1] = hv_CircleRow_FitCircle;
                    hv_circleData_FitCircle[2] = hv_CircleRadius_FitCircle;
                    hv_circleData_FitCircle[3] = hv_CircleStartPhi_FitCircle;
                    hv_circleData_FitCircle[4] = hv_CircleEndPhi_FitCircle;
                    hv_circleData_FitCircle[5] = hv_PointOrder1_FitCircle;

                    if (circlesCounterFlag < 2)
                    {
                        hv_Information_CircleInspectionSample = new HTuple();//显示消息初始化,清除以前复制的消息
                        if (LanguageFlag == 0)
                        {
                            hv_Information_CircleInspectionSample[0] = "Fitting circle 1 successful!";
                            hv_Information_CircleInspectionSample[1] = "Circle 1 center coordination: " + "(" + hv_circleData_FitCircle[0] + "," + hv_circleData_FitCircle[1] + ")" + ",Radius:" + hv_circleData_FitCircle[2];
                            hv_Information_CircleInspectionSample[2] = "Offset angle:" + realTimeShapeModelOffsideAngle;
                        }
                        else
                        {
                            hv_Information_CircleInspectionSample[0] = "拟合圆1成功";
                            hv_Information_CircleInspectionSample[1] = "圆心1坐标:" + "(" + hv_circleData_FitCircle[0] + "," + hv_circleData_FitCircle[1] + ")" + ",半径:" + hv_circleData_FitCircle[2];
                            hv_Information_CircleInspectionSample[2] = "偏移角度:" + realTimeShapeModelOffsideAngle;
                        }
                        disp_message_Halcon(hv_WindowHandle_CircleInspectionSample, hv_Information_CircleInspectionSample, "image", 0, 0, "green", "false");
                        //disp_message_Halcon(hv_WindowHandle_CircleInspectionSample, 1, "image", hv_circleData_FitCircle[0], hv_circleData_FitCircle[1], "green", "false");
                        disp_message_Halcon(hv_WindowHandle_CircleInspectionSample, 1, "image", hv_circleData_FitCircle[1], hv_circleData_FitCircle[0], "green", "false");
                        //显示图片的文件名
                        disp_message_Halcon(hv_WindowHandle_CircleInspectionSample, imageFileName_CircleInspectionSample, "window", 0, picBoxForDisplayWidth_CircleInspectionSample - imageFileName_CircleInspectionSample.Length * 8, "yellow", "false");
                    }

                    HOperatorSet.SetDraw(hv_WindowHandle_FitCircle, "margin");
                    HOperatorSet.SetColor(hv_WindowHandle_FitCircle, "green");//显示拟合的圆
                    HOperatorSet.GenCircle(out ho_Circle_FitCircle, hv_CircleRow_FitCircle, hv_CircleColumn_FitCircle, hv_CircleRadius_FitCircle);
                    HOperatorSet.DispObj(ho_Circle_FitCircle, hv_WindowHandle_FitCircle);

                    if (CreatingCircleInspectionSampleFlag_CircleInspectionSample)
                    {
                        HOperatorSet.SetColor(hv_WindowHandle_CircleInspectionSample, "yellow");
                        HOperatorSet.DispObj(ho_Rectangle_CircleInspectionSample, hv_WindowHandle_FitCircle);
                        HOperatorSet.SetColor(hv_WindowHandle_CircleInspectionSample, "violet");
                        HOperatorSet.DispObj(ho_FindRegionRectangle_CircleInspectionSample, hv_WindowHandle_FitCircle);
                    }
                }

                if (hv_circleData_FitCircle_2th == null && stepFlag_CircleInspectionSample == 1)
                {
                    hv_circleData_FitCircle_2th = new HTuple();
                    //hv_circleData_FitCircle_2th[0] = hv_CircleRow_FitCircle_2th;
                    //hv_circleData_FitCircle_2th[1] = hv_CircleColumn_FitCircle_2th;
                    hv_circleData_FitCircle_2th[0] = hv_CircleColumn_FitCircle_2th;
                    hv_circleData_FitCircle_2th[1] = hv_CircleRow_FitCircle_2th;
                    hv_circleData_FitCircle_2th[2] = hv_CircleRadius_FitCircle_2th;
                    hv_circleData_FitCircle_2th[3] = hv_CircleStartPhi_FitCircle_2th;
                    hv_circleData_FitCircle_2th[4] = hv_CircleEndPhi_FitCircle_2th;
                    hv_circleData_FitCircle_2th[5] = hv_PointOrder1_FitCircle_2th;

                    if (hv_circleData_FitCircle != null)
                    {
                        hv_Information_CircleInspectionSample = new HTuple();//显示消息初始化,清除以前复制的消息
                        if (LanguageFlag == 0)
                        {
                            hv_Information_CircleInspectionSample[0] = "Fitting circle 1 & 2 successful.";
                            hv_Information_CircleInspectionSample[1] = "Circle 1 center coordination: " + "(" + hv_circleData_FitCircle[0] + "," + hv_circleData_FitCircle[1] + ")" + ",Radius:" + hv_circleData_FitCircle[2];
                            hv_Information_CircleInspectionSample[2] = "Circle 2 center coordination: " + "(" + hv_circleData_FitCircle_2th[0] + "," + hv_circleData_FitCircle_2th[1] + ")" + ",Radius:" + hv_circleData_FitCircle_2th[2];
                            hv_Information_CircleInspectionSample[3] = "Offset angle:" + realTimeShapeModelOffsideAngle;
                        }
                        else
                        {
                            hv_Information_CircleInspectionSample[0] = "拟合圆1,2成功";
                            hv_Information_CircleInspectionSample[1] = "圆心1坐标:" + "(" + hv_circleData_FitCircle[0] + "," + hv_circleData_FitCircle[1] + ")" + ",半径:" + hv_circleData_FitCircle[2];
                            hv_Information_CircleInspectionSample[2] = "圆心2坐标:" + "(" + hv_circleData_FitCircle_2th[0] + "," + hv_circleData_FitCircle_2th[1] + ")" + ",半径:" + hv_circleData_FitCircle_2th[2];
                            hv_Information_CircleInspectionSample[3] = "偏移角度:" + realTimeShapeModelOffsideAngle;
                        }
                        disp_message_Halcon(hv_WindowHandle_CircleInspectionSample, hv_Information_CircleInspectionSample, "image", 0, 0, "green", "false");
                        //disp_message_Halcon(hv_WindowHandle_CircleInspectionSample, 1, "image", hv_circleData_FitCircle[0], hv_circleData_FitCircle[1], "green", "false");
                        //disp_message_Halcon(hv_WindowHandle_CircleInspectionSample, 2, "image", hv_circleData_FitCircle_2th[0], hv_circleData_FitCircle_2th[1], "green", "false");
                        disp_message_Halcon(hv_WindowHandle_CircleInspectionSample, 1, "image", hv_circleData_FitCircle[1], hv_circleData_FitCircle[0], "green", "false");
                        disp_message_Halcon(hv_WindowHandle_CircleInspectionSample, 2, "image", hv_circleData_FitCircle_2th[1], hv_circleData_FitCircle_2th[0], "green", "false");
                    }
                    else
                    {
                        hv_Information_CircleInspectionSample = new HTuple();//显示消息初始化,清除以前复制的消息
                        if (LanguageFlag == 0)
                        {
                            hv_Information_CircleInspectionSample[0] = "Fitting circle 1 failed! Failure at step " + FitCircleStepNo_FitCircle + "!";
                            hv_Information_CircleInspectionSample[1] = "Offset angle:" + realTimeShapeModelOffsideAngle;
                        }
                        else
                        {
                            hv_Information_CircleInspectionSample[0] = "拟合圆1失败!失败在第" + FitCircleStepNo_FitCircle + "步!";
                            hv_Information_CircleInspectionSample[1] = "偏移角度:" + realTimeShapeModelOffsideAngle;
                        }
                        disp_message_Halcon(hv_WindowHandle_CircleInspectionSample, hv_Information_CircleInspectionSample, "image", 0, 0, "red", "false");
                        hv_Information_CircleInspectionSample = new HTuple();//显示消息初始化,清除以前复制的消息
                        if (LanguageFlag == 0)
                        {
                            hv_Information_CircleInspectionSample[0] = "Fitting circle 2 successful.";
                            hv_Information_CircleInspectionSample[1] = "Circle 2 center coordination: " + "(" + hv_circleData_FitCircle_2th[0] + "," + hv_circleData_FitCircle_2th[1] + ")" + ",Radius: " + hv_circleData_FitCircle_2th[2];
                            hv_Information_CircleInspectionSample[2] = "Offset angle:" + realTimeShapeModelOffsideAngle;
                        }
                        else
                        {
                            hv_Information_CircleInspectionSample[0] = "拟合圆2成功";
                            hv_Information_CircleInspectionSample[1] = "圆心2坐标:" + "(" + hv_circleData_FitCircle_2th[0] + "," + hv_circleData_FitCircle_2th[1] + ")" + ",半径:" + hv_circleData_FitCircle_2th[2];
                            hv_Information_CircleInspectionSample[2] = "偏移角度:" + realTimeShapeModelOffsideAngle;
                        }
                        disp_message_Halcon(hv_WindowHandle_CircleInspectionSample, hv_Information_CircleInspectionSample, "image", 0, 200, "green", "false");
                        //disp_message_Halcon(hv_WindowHandle_CircleInspectionSample, 2, "image", hv_circleData_FitCircle_2th[0], hv_circleData_FitCircle_2th[1], "green", "false");
                        disp_message_Halcon(hv_WindowHandle_CircleInspectionSample, 2, "image", hv_circleData_FitCircle_2th[1], hv_circleData_FitCircle_2th[0], "green", "false");
                    }

                    //显示图片的文件名
                    disp_message_Halcon(hv_WindowHandle_CircleInspectionSample, imageFileName_CircleInspectionSample, "window", 0, picBoxForDisplayWidth_CircleInspectionSample - imageFileName_CircleInspectionSample.Length * 8, "yellow", "false");

                    HOperatorSet.SetDraw(hv_WindowHandle_FitCircle, "margin");
                    HOperatorSet.SetColor(hv_WindowHandle_FitCircle, "green");//显示拟合的圆
                    HOperatorSet.GenCircle(out ho_Circle_FitCircle_2th, hv_CircleRow_FitCircle_2th, hv_CircleColumn_FitCircle_2th, hv_CircleRadius_FitCircle_2th);
                    if (hv_circleData_FitCircle != null)
                        HOperatorSet.DispObj(ho_Circle_FitCircle, hv_WindowHandle_FitCircle);
                    HOperatorSet.DispObj(ho_Circle_FitCircle_2th, hv_WindowHandle_FitCircle);

                    if (CreatingCircleInspectionSampleFlag_CircleInspectionSample)
                    {
                        HOperatorSet.SetColor(hv_WindowHandle_CircleInspectionSample, "yellow");
                        HOperatorSet.DispObj(ho_Rectangle_CircleInspectionSample, hv_WindowHandle_FitCircle);
                        HOperatorSet.SetColor(hv_WindowHandle_CircleInspectionSample, "violet");
                        HOperatorSet.DispObj(ho_FindRegionRectangle_CircleInspectionSample, hv_WindowHandle_FitCircle);
                        if (circlesCounterFlag == 2)
                        {
                            HOperatorSet.SetColor(hv_WindowHandle_CircleInspectionSample, "cyan");
                            HOperatorSet.DispObj(ho_FindRegionRectangle_CircleInspectionSample_2th, hv_WindowHandle_FitCircle);
                        }
                    }
                }
            }
            catch
            {
                if (stepFlag_CircleInspectionSample == 0)
                    hv_circleData_FitCircle = null;
                if (circlesCounterFlag == 2 && stepFlag_CircleInspectionSample == 1)
                    hv_circleData_FitCircle_2th = null;

                if (stepFlag_CircleInspectionSample == 0)
                {
                    HOperatorSet.DispObj(ho_Image_FitCircle, hv_WindowHandle_FitCircle);//为了清除上次显示的message
                    hv_Information_CircleInspectionSample = new HTuple();//显示消息初始化,清除以前复制的消息
                    if (LanguageFlag == 0)
                    {
                        hv_Information_CircleInspectionSample[0] = "Fitting circle 1 failed! Failure at step " + FitCircleStepNo_FitCircle + "!";
                        hv_Information_CircleInspectionSample[1] = "Offset angle:" + realTimeShapeModelOffsideAngle;
                    }
                    else
                    {
                        hv_Information_CircleInspectionSample[0] = "拟合圆1失败!失败在第" + FitCircleStepNo_FitCircle + "步!";
                        hv_Information_CircleInspectionSample[1] = "偏移角度:" + realTimeShapeModelOffsideAngle;
                    }
                    disp_message_Halcon(hv_WindowHandle_FitCircle, hv_Information_CircleInspectionSample, "image", 0, 0, "red", "false");

                    //显示图片的文件名
                    disp_message_Halcon(hv_WindowHandle_CircleInspectionSample, imageFileName_CircleInspectionSample, "window", 0, picBoxForDisplayWidth_CircleInspectionSample - imageFileName_CircleInspectionSample.Length * 8, "yellow", "false");
                }
                if (circlesCounterFlag == 2 && stepFlag_CircleInspectionSample == 1)
                {
                    hv_Information_CircleInspectionSample = new HTuple();//显示消息初始化,清除以前复制的消息
                    if (LanguageFlag == 0)
                    {
                        hv_Information_CircleInspectionSample[0] = "Fitting circle 2 failed! Failure at step " + FitCircleStepNo_FitCircle_2th + "!";
                        hv_Information_CircleInspectionSample[1] = "Offset angle:" + realTimeShapeModelOffsideAngle;
                    }
                    else
                    {
                        hv_Information_CircleInspectionSample[0] = "拟合圆2失败!失败在第" + FitCircleStepNo_FitCircle_2th + "步!";
                        hv_Information_CircleInspectionSample[1] = "偏移角度:" + realTimeShapeModelOffsideAngle;
                    }
                    disp_message_Halcon(hv_WindowHandle_FitCircle, hv_Information_CircleInspectionSample, "image", 0, 200, "red", "false");
                }

            }

            if (stepFlag_CircleInspectionSample == 0)
                HT = hv_circleData_FitCircle;
            if (circlesCounterFlag == 2 && stepFlag_CircleInspectionSample == 1)
                HT = hv_circleData_FitCircle_2th;
            return HT;
        }
        #endregion

        #region displayRegion1
        private void displayRegion1()
        {
            hv_Information_CircleInspectionSample = new HTuple();//显示消息初始化,清除以前复制的消息
            HOperatorSet.DispObj(ho_Image_CircleInspectionSample, hv_WindowHandle_CircleInspectionSample);//为了清除上次显示的message
            HOperatorSet.SetColor(hv_WindowHandle_CircleInspectionSample, "yellow");
            HOperatorSet.DispObj(ho_Rectangle_CircleInspectionSample, hv_WindowHandle_CircleInspectionSample);//显示绘制的模型的矩形或者圆形区域
            HOperatorSet.SetColor(hv_WindowHandle_CircleInspectionSample, "violet");
            HOperatorSet.DispObj(ho_FindRegionRectangle_CircleInspectionSample, hv_WindowHandle_CircleInspectionSample);
        }
        #endregion

        #region displayRegion2
        private void displayRegion2()
        {
            HOperatorSet.SetColor(hv_WindowHandle_CircleInspectionSample, "yellow");
            HOperatorSet.DispObj(ho_Rectangle_CircleInspectionSample, hv_WindowHandle_CircleInspectionSample);//显示绘制的模型的矩形或者圆形区域
            HOperatorSet.SetColor(hv_WindowHandle_CircleInspectionSample, "violet");
            HOperatorSet.DispObj(ho_FindRegionRectangle_CircleInspectionSample, hv_WindowHandle_CircleInspectionSample);
            if (circlesCounterFlag == 2)
            {
                HOperatorSet.SetColor(hv_WindowHandle_CircleInspectionSample, "cyan");
                HOperatorSet.DispObj(ho_FindRegionRectangle_CircleInspectionSample_2th, hv_WindowHandle_CircleInspectionSample);
            }
        }
        #endregion

        #region SaveCircleInspectionSample:保存圆检测模型参数
        public void SaveCircleInspectionSample()
        {
            //smp: shapemodel文件
            //smd: shapemodel参数文件
            //smdd: 检测矩形区域参数文件
            //smddd: 参考圆孔的参数(圆心坐标,半径) 及 算子参数文件
            //if (ho_ImageReduced_CircleInspectionSample != null && hv_circleData_FitCircle != null)
            if ((circlesCounterFlag < 2 && ho_ImageReduced_CircleInspectionSample != null && hv_circleData_FitCircle != null) || (circlesCounterFlag == 2 && ho_ImageReduced_CircleInspectionSample != null && hv_circleData_FitCircle != null && hv_circleData_FitCircle_2th != null))
            {
                //保存shapemodel模型
                string s = shapeModelImageFileDirectionPath_CircleInspectionSample + shapeModelImageFileName_CircleInspectionSample;
                hv_ShapeModeDataPath_CircleInspectionSample = s + ".smp";
                HOperatorSet.WriteShapeModel(hv_ModelID_CircleInspectionSample, hv_ShapeModeDataPath_CircleInspectionSample);

                //保存shapemodel模型相关的坐标角度等
                hv_ShapeModeDataFilePath_CircleInspectionSample = s + ".smd";
                HOperatorSet.OpenFile(hv_ShapeModeDataFilePath_CircleInspectionSample, "output", out hv_FileHandle_CircleInspectionSample);
                HOperatorSet.FwriteString(hv_FileHandle_CircleInspectionSample, (((((((((((circlesCounterFlag + " ") + shapeModelRectangleOrCircleFlag) + " ") + hv_Row_CircleInspectionSample) + " ") + hv_Column_CircleInspectionSample) + " ") + hv_Phi_CircleInspectionSample) + " ") + hv_Length1_CircleInspectionSample) + " ") + hv_Length2_CircleInspectionSample);
                HOperatorSet.CloseFile(hv_FileHandle_CircleInspectionSample);

                //保存查找圆的矩形或者圆形区域的参数
                HOperatorSet.OpenFile(s + ".smdd", "output", out hv_FileHandle_CircleInspectionSample);
                if (circlesCounterFlag < 2)//只有圆1
                {
                    if (circle1FindRegionRectangleOrCircleFlag == 0 || circle1FindRegionRectangleOrCircleFlag == 1)//圆1矩形或者圆形
                        HOperatorSet.FwriteString(hv_FileHandle_CircleInspectionSample, (((((((((circle1FindRegionRectangleOrCircleFlag + " ") + hv_FindRegionRow_CircleInspectionSample) + " ") + hv_FindRegionColumn_CircleInspectionSample) + " ") + hv_FindRegionPhi_CircleInspectionSample) + " ") + hv_FindRegionLength1_CircleInspectionSample) + " ") + hv_FindRegionLength2_CircleInspectionSample);
                    if (circle1FindRegionRectangleOrCircleFlag == 2)//圆1圆环
                        HOperatorSet.FwriteString(hv_FileHandle_CircleInspectionSample, (((((((((((circle1FindRegionRectangleOrCircleFlag + " ") + hv_FindRegionRow_CircleInspectionSample) + " ") + hv_FindRegionColumn_CircleInspectionSample) + " ") + hv_FindRegionPhi_CircleInspectionSample) + " ") + hv_FindRegionPhi2_CircleInspectionSample) + " ") + hv_FindRegionLength1_CircleInspectionSample) + " ") + hv_FindRegionLength2_CircleInspectionSample);
                }
                if (circlesCounterFlag == 2)//有圆1及圆2
                {
                    //查找区域:圆1矩形或者圆形 && 圆2矩形或者圆形
                    if ((circle1FindRegionRectangleOrCircleFlag == 0 || circle1FindRegionRectangleOrCircleFlag == 1) && (circle2FindRegionRectangleOrCircleFlag == 0 || circle2FindRegionRectangleOrCircleFlag == 1))
                        HOperatorSet.FwriteString(hv_FileHandle_CircleInspectionSample, (((((((((circle1FindRegionRectangleOrCircleFlag + " ") + hv_FindRegionRow_CircleInspectionSample) + " ") + hv_FindRegionColumn_CircleInspectionSample) + " ") + hv_FindRegionPhi_CircleInspectionSample) + " ") + hv_FindRegionLength1_CircleInspectionSample) + " ") + hv_FindRegionLength2_CircleInspectionSample
    + "\r" + (((((((((circle2FindRegionRectangleOrCircleFlag + " ") + hv_FindRegionRow_CircleInspectionSample_2th) + " ") + hv_FindRegionColumn_CircleInspectionSample_2th) + " ") + hv_FindRegionPhi_CircleInspectionSample_2th) + " ") + hv_FindRegionLength1_CircleInspectionSample_2th) + " ") + hv_FindRegionLength2_CircleInspectionSample_2th);
                    //查找区域:圆1矩形或者圆形 && 圆2环形
                    if ((circle1FindRegionRectangleOrCircleFlag == 0 || circle1FindRegionRectangleOrCircleFlag == 1) && (circle2FindRegionRectangleOrCircleFlag == 2))
                        HOperatorSet.FwriteString(hv_FileHandle_CircleInspectionSample, (((((((((circle1FindRegionRectangleOrCircleFlag + " ") + hv_FindRegionRow_CircleInspectionSample) + " ") + hv_FindRegionColumn_CircleInspectionSample) + " ") + hv_FindRegionPhi_CircleInspectionSample) + " ") + hv_FindRegionLength1_CircleInspectionSample) + " ") + hv_FindRegionLength2_CircleInspectionSample
    + "\r" + (((((((((((circle2FindRegionRectangleOrCircleFlag + " ") + hv_FindRegionRow_CircleInspectionSample_2th) + " ") + hv_FindRegionColumn_CircleInspectionSample_2th) + " ") + hv_FindRegionPhi_CircleInspectionSample_2th) + " ") + hv_FindRegionPhi2_CircleInspectionSample_2th) + " ") + hv_FindRegionLength1_CircleInspectionSample_2th) + " ") + hv_FindRegionLength2_CircleInspectionSample_2th);
                    //查找区域:圆1环形 && 圆2矩形或者圆形
                    if ((circle1FindRegionRectangleOrCircleFlag == 2) && (circle2FindRegionRectangleOrCircleFlag == 0 || circle2FindRegionRectangleOrCircleFlag == 1))
                        HOperatorSet.FwriteString(hv_FileHandle_CircleInspectionSample, (((((((((((circle1FindRegionRectangleOrCircleFlag + " ") + hv_FindRegionRow_CircleInspectionSample) + " ") + hv_FindRegionColumn_CircleInspectionSample) + " ") + hv_FindRegionPhi_CircleInspectionSample) + " ") + hv_FindRegionPhi2_CircleInspectionSample) + " ") + hv_FindRegionLength1_CircleInspectionSample) + " ") + hv_FindRegionLength2_CircleInspectionSample
    + "\r" + (((((((((circle2FindRegionRectangleOrCircleFlag + " ") + hv_FindRegionRow_CircleInspectionSample_2th) + " ") + hv_FindRegionColumn_CircleInspectionSample_2th) + " ") + hv_FindRegionPhi_CircleInspectionSample_2th) + " ") + hv_FindRegionLength1_CircleInspectionSample_2th) + " ") + hv_FindRegionLength2_CircleInspectionSample_2th);
                    //查找区域:圆1环形 && 圆2环形
                    if (circle1FindRegionRectangleOrCircleFlag == 2 && circle2FindRegionRectangleOrCircleFlag == 2)
                        HOperatorSet.FwriteString(hv_FileHandle_CircleInspectionSample, (((((((((((circle1FindRegionRectangleOrCircleFlag + " ") + hv_FindRegionRow_CircleInspectionSample) + " ") + hv_FindRegionColumn_CircleInspectionSample) + " ") + hv_FindRegionPhi_CircleInspectionSample) + " ") + hv_FindRegionPhi2_CircleInspectionSample) + " ") + hv_FindRegionLength1_CircleInspectionSample) + " ") + hv_FindRegionLength2_CircleInspectionSample
    + "\r" + (((((((((((circle2FindRegionRectangleOrCircleFlag + " ") + hv_FindRegionRow_CircleInspectionSample_2th) + " ") + hv_FindRegionColumn_CircleInspectionSample_2th) + " ") + hv_FindRegionPhi_CircleInspectionSample_2th) + " ") + hv_FindRegionPhi2_CircleInspectionSample_2th) + " ") + hv_FindRegionLength1_CircleInspectionSample_2th) + " ") + hv_FindRegionLength2_CircleInspectionSample_2th);
                }
                HOperatorSet.CloseFile(hv_FileHandle_CircleInspectionSample);

                //保存拟合获取的圆的参数 及 算子参数
                HOperatorSet.OpenFile(s + ".smddd", "output", out hv_FileHandle_CircleInspectionSample);
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    HOperatorSet.FwriteString(hv_FileHandle_CircleInspectionSample, (((hv_circleData_FitCircle[0] + " ") + hv_circleData_FitCircle[1]) + " ") + hv_circleData_FitCircle[2]);
                    HOperatorSet.FwriteString(hv_FileHandle_CircleInspectionSample, "\r");
                    HOperatorSet.FwriteString(hv_FileHandle_CircleInspectionSample, (((FitCirAdjWindow.scale_image_numeric1 + " ") + FitCirAdjWindow.scale_image_numeric2 + " ")
                        + FitCirAdjWindow.threshold_numeric1 + " ") + FitCirAdjWindow.threshold_numeric2
                        + " " + FitCirAdjWindow.select_shape_numeric1 + " " + FitCirAdjWindow.select_shape_numeric2 + " "
                        + FitCirAdjWindow.dilation_circle_numeri1 + " " + FitCirAdjWindow.edges_sub_pix_numeric1 + " "
                        + FitCirAdjWindow.edges_sub_pix_numeric2 + " " + FitCirAdjWindow.union_cocircular_contours_xld_numeric1 + " "
                        + FitCirAdjWindow.union_cocircular_contours_xld_numeric2 + " " + FitCirAdjWindow.union_cocircular_contours_xld_numeric3 + " "
                        + FitCirAdjWindow.union_cocircular_contours_xld_numeric4 + " " + FitCirAdjWindow.union_cocircular_contours_xld_numeric5 + " "
                        + FitCirAdjWindow.union_cocircular_contours_xld_numeric6 + " ");

                    if (circlesCounterFlag == 2)
                    {
                        HOperatorSet.FwriteString(hv_FileHandle_CircleInspectionSample, "\r");
                        HOperatorSet.FwriteString(hv_FileHandle_CircleInspectionSample, (((hv_circleData_FitCircle_2th[0] + " ") + hv_circleData_FitCircle_2th[1]) + " ") + hv_circleData_FitCircle_2th[2]);
                        HOperatorSet.FwriteString(hv_FileHandle_CircleInspectionSample, "\r");
                        HOperatorSet.FwriteString(hv_FileHandle_CircleInspectionSample, (((FitCirAdjWindow.scale_image_numeric1_2th + " ") + FitCirAdjWindow.scale_image_numeric2_2th + " ")
                            + FitCirAdjWindow.threshold_numeric1_2th + " ") + FitCirAdjWindow.threshold_numeric2_2th
                            + " " + FitCirAdjWindow.select_shape_numeric1_2th + " " + FitCirAdjWindow.select_shape_numeric2_2th + " "
                            + FitCirAdjWindow.dilation_circle_numeri1_2th + " " + FitCirAdjWindow.edges_sub_pix_numeric1_2th + " "
                            + FitCirAdjWindow.edges_sub_pix_numeric2_2th + " " + FitCirAdjWindow.union_cocircular_contours_xld_numeric1_2th + " "
                            + FitCirAdjWindow.union_cocircular_contours_xld_numeric2_2th + " " + FitCirAdjWindow.union_cocircular_contours_xld_numeric3_2th + " "
                            + FitCirAdjWindow.union_cocircular_contours_xld_numeric4_2th + " " + FitCirAdjWindow.union_cocircular_contours_xld_numeric5_2th + " "
                            + FitCirAdjWindow.union_cocircular_contours_xld_numeric6_2th + " ");
                    }

                }
                HOperatorSet.CloseFile(hv_FileHandle_CircleInspectionSample);
                HOperatorSet.WriteImage(ho_Image_CircleInspectionSample, "jpg", 0, shapeModelImageFileDirectionPath_CircleInspectionSample + shapeModelImageFileName_CircleInspectionSample);//保存图片

            }
            else
            {
                if (LanguageFlag == 0)
                    MessageBox.Show("Please edit circle measurement model", "Hint");
                else
                    MessageBox.Show("请先编辑圆测量模型", "提示");
            }
        }
        #endregion

        #region CircleInspectionAction
        private double realTimeShapeModelOffsideAngle = 0.0;
        public int CircleInspectionAction(string shapeModelFileDirectionPath, string shapeModelFileName, HTuple windowhandle, HObject ho_Image, HTuple hv_WindowWidth, HTuple hv_WindowHeight, out double[] circle1Data, out double[] circle2Data, bool shapeModelDisplayFlag, bool realTimeShapeModelDisplayFlag, bool findRegionDisplayFlag, bool realTimeFindRegionDisplayFlag)
        {
            circle1Data = null;
            circle2Data = null;

            int circleInspectionActionFlag = 0;//1-4:读取参数文件失败;20:未能查找到模型,请采集或读取新图片重试

            CreatingCircleInspectionSampleFlag_CircleInspectionSample = false;

            //显示调试画面
            if (FitCirAdjWindow == null || !FitCirAdjWindow.IsHandleCreated)
            {
                FitCirAdjWindow = new FitCircleAdjust();
                FitCirAdjWindow.CircleInspectionAdjustDisplay = this;
                FitCirAdjWindow.circlesCounterFlag = this.circlesCounterFlag;
                FitCirAdjWindow.stepFlag_CircleInspectionSample = this.stepFlag_CircleInspectionSample;
                FitCirAdjWindow.Show();
                FitCirAdjWindow.Visible = false;
            }
            FitCirAdjWindow.windowRefreshFlag = true;

            ////readCameraParametersFile
            //readCameraParametersFile(CameraParametersFilePatch);

            //Shape Model Data Read
            circleInspectionActionFlag = ReadShapeModelData_CircleInspection(shapeModelFileDirectionPath, shapeModelFileName);
            if (circleInspectionActionFlag > 0)
            {
                HOperatorSet.DispObj(ho_Image, windowhandle);

                //显示图片的文件名
                disp_message_Halcon(windowhandle, imageFileName_CircleInspectionSample, "window", 0, hv_WindowWidth - (imageFileName_CircleInspectionSample.Length) * 8, "yellow", "false");

                switch (circleInspectionActionFlag)
                {
                    case 1:
                        if (LanguageFlag == 0)
                            disp_message_Halcon(windowhandle, "Load " + shapeModelFileName + ".smp" + " file failed!", "image", 0, 0, "red", "false");
                        else
                            disp_message_Halcon(windowhandle, "读取 " + shapeModelFileName + ".smp" + " 文件失败!", "image", 0, 0, "red", "false");
                        break;
                    case 2:
                        if (LanguageFlag == 0)
                            disp_message_Halcon(windowhandle, "Load " + shapeModelFileName + ".smd" + " file failed!", "image", 0, 0, "red", "false");
                        else
                            disp_message_Halcon(windowhandle, "读取 " + shapeModelFileName + ".smd" + " 文件失败!", "image", 0, 0, "red", "false");
                        break;
                    case 3:
                        if (LanguageFlag == 0)
                            disp_message_Halcon(windowhandle, "Load " + shapeModelFileName + ".smdd" + " file failed!", "image", 0, 0, "red", "false");
                        else
                            disp_message_Halcon(windowhandle, "读取 " + shapeModelFileName + ".smdd" + " 文件失败!", "image", 0, 0, "red", "false");
                        break;
                    case 4:
                        if (LanguageFlag == 0)
                            disp_message_Halcon(windowhandle, "Load " + shapeModelFileName + ".smddd" + " file failed!", "image", 0, 0, "red", "false");
                        else
                            disp_message_Halcon(windowhandle, "读取 " + shapeModelFileName + ".smp" + " 文件失败!", "image", 0, 0, "red", "false");
                        break;

                    default:
                        break;
                }
                return circleInspectionActionFlag;
            }

            //绘制原模型的矩形或者圆形区域
            HObject ho_ROI_3 = new HObject();
            shapeModelRectangleOrCircleFlag = hv_shapeModelData_ReadShapeModelData_CircleInspection.TupleSelect(1).I;
            HTuple hv_Row_SM = hv_shapeModelData_ReadShapeModelData_CircleInspection.TupleSelect(2);
            HTuple hv_Column_SM = hv_shapeModelData_ReadShapeModelData_CircleInspection.TupleSelect(3);
            HTuple hv_Phi_SM = hv_shapeModelData_ReadShapeModelData_CircleInspection.TupleSelect(4);
            HTuple hv_Length1_SM = hv_shapeModelData_ReadShapeModelData_CircleInspection.TupleSelect(5);
            HTuple hv_Length2_SM = hv_shapeModelData_ReadShapeModelData_CircleInspection.TupleSelect(6);
            if (shapeModelRectangleOrCircleFlag == 0)
                HOperatorSet.GenRectangle2(out ho_ROI_3, hv_Row_SM, hv_Column_SM, hv_Phi_SM, hv_Length1_SM, hv_Length2_SM);
            if (shapeModelRectangleOrCircleFlag == 1)
                HOperatorSet.GenEllipse(out ho_ROI_3, hv_Row_SM, hv_Column_SM, hv_Phi_SM, hv_Length1_SM, hv_Length2_SM);

            //绘制原查找区域的矩形或者圆形区域或者环形区域
            HObject ho_ROI_4 = new HObject();
            HObject ho_ROI_4_2th = new HObject();
            if (circle1FindRegionRectangleOrCircleFlag == 0)//查找区域:圆1矩形
                HOperatorSet.GenRectangle2(out ho_ROI_4, hv_RRRow_ReadShapeModelData_CircleInspection, hv_RRColumn_ReadShapeModelData_CircleInspection, hv_RRPhi_ReadShapeModelData_CircleInspection, hv_RRLength1_ReadShapeModelData_CircleInspection, hv_RRLength2_ReadShapeModelData_CircleInspection);
            if (circle1FindRegionRectangleOrCircleFlag == 1)//查找区域:圆1圆形
                HOperatorSet.GenEllipse(out ho_ROI_4, hv_RRRow_ReadShapeModelData_CircleInspection, hv_RRColumn_ReadShapeModelData_CircleInspection, hv_RRPhi_ReadShapeModelData_CircleInspection, hv_RRLength1_ReadShapeModelData_CircleInspection, hv_RRLength2_ReadShapeModelData_CircleInspection);
            if (circle1FindRegionRectangleOrCircleFlag == 2)//查找区域:圆1圆环
            {
                HObject circleSector1 = new HObject();
                HObject circleSector2 = new HObject();

                double angle = hv_RRPhi2_ReadShapeModelData_CircleInspection.TupleReal() - hv_RRPhi_ReadShapeModelData_CircleInspection.TupleReal();
                if (angle < 0)
                    angle = angle * -1;
                if (angle != 360 && angle != 0)
                {
                    HOperatorSet.GenCircleSector(out circleSector1, hv_RRRow_ReadShapeModelData_CircleInspection, hv_RRColumn_ReadShapeModelData_CircleInspection, hv_RRLength1_ReadShapeModelData_CircleInspection, hv_RRPhi2_ReadShapeModelData_CircleInspection.TupleRad(), hv_RRPhi_ReadShapeModelData_CircleInspection.TupleRad());
                    HOperatorSet.GenCircleSector(out circleSector2, hv_RRRow_ReadShapeModelData_CircleInspection, hv_RRColumn_ReadShapeModelData_CircleInspection, hv_RRLength2_ReadShapeModelData_CircleInspection, hv_RRPhi2_ReadShapeModelData_CircleInspection.TupleRad(), hv_RRPhi_ReadShapeModelData_CircleInspection.TupleRad());
                }
                if (angle == 360 || angle == 0)
                {
                    HOperatorSet.GenEllipse(out circleSector1, hv_RRRow_ReadShapeModelData_CircleInspection, hv_RRColumn_ReadShapeModelData_CircleInspection, 0, hv_RRLength1_ReadShapeModelData_CircleInspection, hv_RRLength1_ReadShapeModelData_CircleInspection);
                    HOperatorSet.GenEllipse(out circleSector2, hv_RRRow_ReadShapeModelData_CircleInspection, hv_RRColumn_ReadShapeModelData_CircleInspection, 0, hv_RRLength2_ReadShapeModelData_CircleInspection, hv_RRLength2_ReadShapeModelData_CircleInspection);
                }
                HOperatorSet.Difference(circleSector2, circleSector1, out ho_ROI_4);
                //HOperatorSet.SetDraw(hv_WindowHandle_CircleInspectionSample, "margin");
            }
            if (circlesCounterFlag == 2)
            {
                if (circle2FindRegionRectangleOrCircleFlag == 0)//查找区域:圆2矩形
                    HOperatorSet.GenRectangle2(out ho_ROI_4_2th, hv_RRRow_ReadShapeModelData_CircleInspection_2th, hv_RRColumn_ReadShapeModelData_CircleInspection_2th, hv_RRPhi_ReadShapeModelData_CircleInspection_2th, hv_RRLength1_ReadShapeModelData_CircleInspection_2th, hv_RRLength2_ReadShapeModelData_CircleInspection_2th);
                if (circle2FindRegionRectangleOrCircleFlag == 1)//查找区域:圆2圆形
                    HOperatorSet.GenEllipse(out ho_ROI_4_2th, hv_RRRow_ReadShapeModelData_CircleInspection_2th, hv_RRColumn_ReadShapeModelData_CircleInspection_2th, hv_RRPhi_ReadShapeModelData_CircleInspection_2th, hv_RRLength1_ReadShapeModelData_CircleInspection_2th, hv_RRLength2_ReadShapeModelData_CircleInspection_2th);
                if (circle2FindRegionRectangleOrCircleFlag == 2)//查找区域:圆2圆环
                {
                    HObject circleSector1 = new HObject();
                    HObject circleSector2 = new HObject();

                    double angle = hv_RRPhi2_ReadShapeModelData_CircleInspection_2th.TupleReal() - hv_RRPhi_ReadShapeModelData_CircleInspection_2th.TupleReal();
                    if (angle < 0)
                        angle = angle * -1;
                    if (angle != 360 && angle != 0)
                    {
                        HOperatorSet.GenCircleSector(out circleSector1, hv_RRRow_ReadShapeModelData_CircleInspection_2th, hv_RRColumn_ReadShapeModelData_CircleInspection_2th, hv_RRLength1_ReadShapeModelData_CircleInspection_2th, hv_RRPhi2_ReadShapeModelData_CircleInspection_2th.TupleRad(), hv_RRPhi_ReadShapeModelData_CircleInspection_2th.TupleRad());
                        HOperatorSet.GenCircleSector(out circleSector2, hv_RRRow_ReadShapeModelData_CircleInspection_2th, hv_RRColumn_ReadShapeModelData_CircleInspection_2th, hv_RRLength2_ReadShapeModelData_CircleInspection_2th, hv_RRPhi2_ReadShapeModelData_CircleInspection_2th.TupleRad(), hv_RRPhi_ReadShapeModelData_CircleInspection_2th.TupleRad());
                    }
                    if (angle == 360 || angle == 0)
                    {
                        HOperatorSet.GenEllipse(out circleSector1, hv_RRRow_ReadShapeModelData_CircleInspection_2th, hv_RRColumn_ReadShapeModelData_CircleInspection_2th, 0, hv_RRLength1_ReadShapeModelData_CircleInspection_2th, hv_RRLength1_ReadShapeModelData_CircleInspection_2th);
                        HOperatorSet.GenEllipse(out circleSector2, hv_RRRow_ReadShapeModelData_CircleInspection_2th, hv_RRColumn_ReadShapeModelData_CircleInspection_2th, 0, hv_RRLength2_ReadShapeModelData_CircleInspection_2th, hv_RRLength2_ReadShapeModelData_CircleInspection_2th);
                    }
                    HOperatorSet.Difference(circleSector2, circleSector1, out ho_ROI_4_2th);
                    //HOperatorSet.SetDraw(hv_WindowHandle_CircleInspectionSample, "margin");
                }
            }

            //模板匹配
            HTuple hv_Row1_SM;
            HTuple hv_Column1_SM;
            HTuple hv_Angle;
            HTuple hv_Score;

            HTuple hv_Qx = new HTuple();
            HTuple hv_Qy = new HTuple();

            HTuple hv_Qx_2th = new HTuple();
            HTuple hv_Qy_2th = new HTuple();

            HTuple hv_HomMat2D;

            HOperatorSet.FindShapeModel(ho_Image, hv_ModelID_ReadShapeModelData_CircleInspection, (new HTuple(-360)).TupleRad()
                , (new HTuple(360)).TupleRad(), 0.2, 1, 0.5, "least_squares", 0, 0.9,
                out hv_Row1_SM, out hv_Column1_SM, out hv_Angle, out hv_Score);//hv_Angle是相对原来hv_Phi_SM的角度(弧度值)!!!  

            if ((int)(new HTuple((new HTuple(hv_Row1_SM.TupleLength())).TupleGreater(0))) != 0)
            {
                realTimeShapeModelOffsideAngle = (hv_Angle / (new HTuple(180)).TupleRad() * new HTuple(180)).TupleReal();

                //计算仿射变换矩阵
                HOperatorSet.VectorAngleToRigid(hv_Row_SM, hv_Column_SM, hv_Phi_SM, hv_Row1_SM, hv_Column1_SM, hv_Phi_SM + hv_Angle, out hv_HomMat2D);//不是横平竖直的点位坐标变换!与角度有关系!

                //将原测量矩形的矩形中心进行对应的平移变换
                HOperatorSet.AffineTransPoint2d(hv_HomMat2D, hv_RRRow_ReadShapeModelData_CircleInspection, hv_RRColumn_ReadShapeModelData_CircleInspection, out hv_Qx, out hv_Qy);
                if (circlesCounterFlag == 2)
                {
                    HOperatorSet.AffineTransPoint2d(hv_HomMat2D, hv_RRRow_ReadShapeModelData_CircleInspection_2th, hv_RRColumn_ReadShapeModelData_CircleInspection_2th, out hv_Qx_2th, out hv_Qy_2th);
                }

                //绘制现在测量矩形
                HObject ho_ROI_5 = new HObject();
                HObject ho_ROI_5_2th = new HObject();
                if (circle1FindRegionRectangleOrCircleFlag == 0)//查找区域:圆1矩形
                    HOperatorSet.GenRectangle2(out ho_ROI_5, hv_Qx, hv_Qy, hv_RRPhi_ReadShapeModelData_CircleInspection + hv_Angle, hv_RRLength1_ReadShapeModelData_CircleInspection, hv_RRLength2_ReadShapeModelData_CircleInspection);
                if (circle1FindRegionRectangleOrCircleFlag == 1)//查找区域:圆1圆形
                    HOperatorSet.GenEllipse(out ho_ROI_5, hv_Qx, hv_Qy, hv_RRPhi_ReadShapeModelData_CircleInspection + hv_Angle, hv_RRLength1_ReadShapeModelData_CircleInspection, hv_RRLength2_ReadShapeModelData_CircleInspection);
                if (circle1FindRegionRectangleOrCircleFlag == 2)//查找区域:圆1圆环
                {
                    HObject circleSector1 = new HObject();
                    HObject circleSector2 = new HObject();

                    HTuple fullAngle = (new HTuple(360)).TupleRad();
                    double angle = hv_RRPhi2_ReadShapeModelData_CircleInspection.TupleReal() - hv_RRPhi_ReadShapeModelData_CircleInspection.TupleReal();
                    if (angle < 0)
                        angle = angle * -1;
                    if (angle != 360 && angle != 0)
                    {
                        HTuple hv_Phi1 = new HTuple(hv_RRPhi2_ReadShapeModelData_CircleInspection.TupleRad() + hv_Angle);
                        HTuple hv_Phi2 = new HTuple(hv_RRPhi_ReadShapeModelData_CircleInspection.TupleRad() + hv_Angle);
                        while (hv_Phi1 < 0)
                        {
                            hv_Phi1 = hv_Phi1 + fullAngle;
                        }
                        while (hv_Phi1 > fullAngle)
                        {
                            hv_Phi1 = hv_Phi1 - fullAngle;
                        }
                        while (hv_Phi2 < 0)
                        {
                            hv_Phi2 = hv_Phi2 + fullAngle;
                        }
                        while (hv_Phi2 > fullAngle)
                        {
                            hv_Phi2 = hv_Phi2 - fullAngle;
                        }
                        HOperatorSet.GenCircleSector(out circleSector1, hv_Qx, hv_Qy, hv_RRLength1_ReadShapeModelData_CircleInspection, hv_Phi1, hv_Phi2);
                        HOperatorSet.GenCircleSector(out circleSector2, hv_Qx, hv_Qy, hv_RRLength2_ReadShapeModelData_CircleInspection, hv_Phi1, hv_Phi2);
                    }
                    if (angle == 360 || angle == 0)
                    {
                        HOperatorSet.GenEllipse(out circleSector1, hv_Qx, hv_Qy, 0 + hv_Angle, hv_RRLength1_ReadShapeModelData_CircleInspection, hv_RRLength1_ReadShapeModelData_CircleInspection);
                        HOperatorSet.GenEllipse(out circleSector2, hv_Qx, hv_Qy, 0 + hv_Angle, hv_RRLength2_ReadShapeModelData_CircleInspection, hv_RRLength2_ReadShapeModelData_CircleInspection);
                    }
                    HOperatorSet.Difference(circleSector2, circleSector1, out ho_ROI_5);
                }
                if (circlesCounterFlag == 2)
                {
                    if (circle2FindRegionRectangleOrCircleFlag == 0)//查找区域:圆2矩形
                        HOperatorSet.GenRectangle2(out ho_ROI_5_2th, hv_Qx_2th, hv_Qy_2th, hv_RRPhi_ReadShapeModelData_CircleInspection_2th + hv_Angle, hv_RRLength1_ReadShapeModelData_CircleInspection_2th, hv_RRLength2_ReadShapeModelData_CircleInspection_2th);
                    if (circle2FindRegionRectangleOrCircleFlag == 1)//查找区域:圆2圆形
                        HOperatorSet.GenEllipse(out ho_ROI_5_2th, hv_Qx_2th, hv_Qy_2th, hv_RRPhi_ReadShapeModelData_CircleInspection_2th + hv_Angle, hv_RRLength1_ReadShapeModelData_CircleInspection_2th, hv_RRLength2_ReadShapeModelData_CircleInspection_2th);
                    if (circle2FindRegionRectangleOrCircleFlag == 2)//查找区域:圆2圆环
                    {
                        HObject circleSector1 = new HObject();
                        HObject circleSector2 = new HObject();

                        HTuple fullAngle = (new HTuple(360)).TupleRad();
                        double angle = hv_RRPhi2_ReadShapeModelData_CircleInspection_2th.TupleReal() - hv_RRPhi_ReadShapeModelData_CircleInspection_2th.TupleReal();
                        if (angle < 0)
                            angle = angle * -1;
                        if (angle != 360 && angle != 0)
                        {
                            HTuple hv_Phi1 = new HTuple(hv_RRPhi2_ReadShapeModelData_CircleInspection_2th.TupleRad() + hv_Angle);
                            HTuple hv_Phi2 = new HTuple(hv_RRPhi_ReadShapeModelData_CircleInspection_2th.TupleRad() + hv_Angle);
                            while (hv_Phi1 < 0)
                            {
                                hv_Phi1 = hv_Phi1 + fullAngle;
                            }
                            while (hv_Phi1 > fullAngle)
                            {
                                hv_Phi1 = hv_Phi1 - fullAngle;
                            }
                            while (hv_Phi2 < 0)
                            {
                                hv_Phi2 = hv_Phi2 + fullAngle;
                            }
                            while (hv_Phi2 > fullAngle)
                            {
                                hv_Phi2 = hv_Phi2 - fullAngle;
                            }
                            HOperatorSet.GenCircleSector(out circleSector1, hv_Qx_2th, hv_Qy_2th, hv_RRLength1_ReadShapeModelData_CircleInspection_2th, hv_Phi1, hv_Phi2);
                            HOperatorSet.GenCircleSector(out circleSector2, hv_Qx_2th, hv_Qy_2th, hv_RRLength2_ReadShapeModelData_CircleInspection_2th, hv_Phi1, hv_Phi2);
                        }
                        if (angle == 360 || angle == 0)
                        {
                            HOperatorSet.GenEllipse(out circleSector1, hv_Qx_2th, hv_Qy_2th, 0 + hv_Angle, hv_RRLength1_ReadShapeModelData_CircleInspection_2th, hv_RRLength1_ReadShapeModelData_CircleInspection_2th);
                            HOperatorSet.GenEllipse(out circleSector2, hv_Qx_2th, hv_Qy_2th, 0 + hv_Angle, hv_RRLength2_ReadShapeModelData_CircleInspection_2th, hv_RRLength2_ReadShapeModelData_CircleInspection_2th);
                        }
                        HOperatorSet.Difference(circleSector2, circleSector1, out ho_ROI_5_2th);
                    }
                }

                //绘制现在模型矩形
                HObject ho_ROI_6 = new HObject();
                if (shapeModelRectangleOrCircleFlag == 0)
                    HOperatorSet.GenRectangle2(out ho_ROI_6, hv_Row1_SM, hv_Column1_SM, hv_Phi_SM + hv_Angle, hv_Length1_SM, hv_Length2_SM);
                if (shapeModelRectangleOrCircleFlag == 1)
                    HOperatorSet.GenEllipse(out ho_ROI_6, hv_Row1_SM, hv_Column1_SM, hv_Phi_SM + hv_Angle, hv_Length1_SM, hv_Length2_SM);

                //得到圆拟合区域
                HObject ho_ROI_7 = new HObject();
                HObject ho_ROI_7_2th = new HObject();
                HOperatorSet.ReduceDomain(ho_Image, ho_ROI_5, out ho_ROI_7);
                if (circlesCounterFlag == 2)
                {
                    HOperatorSet.ReduceDomain(ho_Image, ho_ROI_5_2th, out ho_ROI_7_2th);
                }

                //进行圆拟合
                stepFlag_CircleInspectionSample = 0;
                HTuple temp1 = HFitCircle(ho_ROI_7, FitCirAdjWindow);
                if (temp1 != null && temp1.Length > 0)
                {
                    circle1Data = new double[temp1.Length - 1];
                    for (int i = 0; i < temp1.Length - 1; i++)
                    {
                        circle1Data[i] = temp1.TupleSelect(i).TupleReal();
                    }
                }
                stepFlag_CircleInspectionSample++;
                if (circlesCounterFlag == 2)
                {
                    HTuple temp2 = HFitCircle(ho_ROI_7_2th, FitCirAdjWindow);
                    if (temp2 != null && temp2.Length > 0)
                    {
                        circle2Data = new double[temp2.Length - 1];
                        for (int i = 0; i < temp2.Length - 1; i++)
                        {
                            circle2Data[i] = temp2.TupleSelect(i).TupleReal();
                        }
                    }
                    stepFlag_CircleInspectionSample++;
                }
                FitCirAdjWindow.HalconOperatorValueChangedFlag = 2;

                //显示原模型矩形
                if (shapeModelDisplayFlag)
                {
                    HOperatorSet.SetDraw(windowhandle, "margin");
                    HOperatorSet.SetColor(windowhandle, "yellow");
                    HOperatorSet.DispObj(ho_ROI_3, windowhandle);
                }

                //显示原测量矩形区域
                if (findRegionDisplayFlag)
                {
                    HOperatorSet.SetDraw(windowhandle, "margin");
                    HOperatorSet.SetColor(windowhandle, "violet");
                    HOperatorSet.DispObj(ho_ROI_4, windowhandle);
                    if (circlesCounterFlag == 2)
                    {
                        HOperatorSet.SetDraw(windowhandle, "margin");
                        HOperatorSet.SetColor(windowhandle, "cyan");
                        HOperatorSet.DispObj(ho_ROI_4_2th, windowhandle);
                    }
                }

                //显示现在模型矩形
                if (realTimeShapeModelDisplayFlag)
                {
                    HOperatorSet.SetDraw(windowhandle, "margin");
                    HOperatorSet.SetColor(windowhandle, "yellow");
                    HOperatorSet.DispObj(ho_ROI_6, windowhandle);
                }

                //显示现在测量矩形
                if (realTimeFindRegionDisplayFlag)
                {
                    HOperatorSet.SetDraw(windowhandle, "margin");
                    HOperatorSet.SetColor(windowhandle, "violet");
                    HOperatorSet.DispObj(ho_ROI_5, windowhandle);
                    if (circlesCounterFlag == 2)
                    {
                        HOperatorSet.SetDraw(windowhandle, "margin");
                        HOperatorSet.SetColor(windowhandle, "cyan");
                        HOperatorSet.DispObj(ho_ROI_5_2th, windowhandle);
                    }
                }

                return circleInspectionActionFlag;
            }
            else
            {
                HOperatorSet.DispObj(ho_Image, windowhandle);
                //set_display_font_Halcon(windowhandle, 16, "mono", "true", "false");
                if (LanguageFlag == 0)
                    disp_message_Halcon(windowhandle, "Can not find model from current image, please grab new image or load new picture and retry", "image", 0, 0, "red", "false");
                else
                    disp_message_Halcon(windowhandle, "未能查找到模型,请采集或读取新图片重试", "image", 0, 0, "red", "false");
                //显示图片的文件名
                disp_message_Halcon(windowhandle, imageFileName_CircleInspectionSample, "window", 0, hv_WindowWidth - (imageFileName_CircleInspectionSample.Length) * 8, "yellow", "false");

                circleInspectionActionFlag = 20;
                return circleInspectionActionFlag;
            }
        }
        #endregion

        #region ReadShapeModelData_CircleInspection
        private HTuple hv_ModelID_ReadShapeModelData_CircleInspection;
        private HTuple hv_shapeModelData_ReadShapeModelData_CircleInspection = new HTuple();

        private HTuple hv_RRRow_ReadShapeModelData_CircleInspection;////RR:Inspection Rectangle region
        private HTuple hv_RRColumn_ReadShapeModelData_CircleInspection;
        private HTuple hv_RRPhi_ReadShapeModelData_CircleInspection;
        private HTuple hv_RRPhi2_ReadShapeModelData_CircleInspection;//ADD For 环形
        private HTuple hv_RRLength1_ReadShapeModelData_CircleInspection;
        private HTuple hv_RRLength2_ReadShapeModelData_CircleInspection;

        private HTuple hv_RRRow_ReadShapeModelData_CircleInspection_2th;////RR:Inspection Rectangle region
        private HTuple hv_RRColumn_ReadShapeModelData_CircleInspection_2th;
        private HTuple hv_RRPhi_ReadShapeModelData_CircleInspection_2th;
        private HTuple hv_RRPhi2_ReadShapeModelData_CircleInspection_2th;//ADD For 环形
        private HTuple hv_RRLength1_ReadShapeModelData_CircleInspection_2th;
        private HTuple hv_RRLength2_ReadShapeModelData_CircleInspection_2th;

        public int ReadShapeModelData_CircleInspection(string shapeModelFileDirectionPath, string shapeModelFileName)
        {
            //1:"读取 " + shapeModelFileName + ".smp" + " 文件失败!"
            //2:"读取 " + shapeModelFileName + ".smd" + " 文件失败!"
            //3:"读取 " + shapeModelFileName + ".smdd" + " 文件失败!"
            //4:"读取 " + shapeModelFileName + ".smddd" + " 文件失败!"
            int readFlag = 0;

            HTuple hv_FileHandle;
            HTuple hv_OutString;
            HTuple hv_IsEOF;
            HTuple hv_Number;
            HTuple hv_i;

            HTuple hv_Row_SM;
            HTuple hv_Column_SM;
            HTuple hv_Phi_SM;
            HTuple hv_Length1_SM;
            HTuple hv_Length2_SM;

            HTuple hv_Row_SC = new HTuple();//SC:Standard Circle
            HTuple hv_Column_SC = new HTuple();
            HTuple hv_Phi_SC = new HTuple();
            HTuple scale_image_numeric1 = new HTuple();
            HTuple scale_image_numeric2 = new HTuple();
            HTuple threshold_numeric1 = new HTuple();
            HTuple threshold_numeric2 = new HTuple();
            HTuple select_shape_numeric1 = new HTuple();
            HTuple select_shape_numeric2 = new HTuple();
            HTuple edges_sub_pix_numeric1 = new HTuple();
            HTuple edges_sub_pix_numeric2 = new HTuple();
            HTuple dilation_circle_numeri1 = new HTuple();
            HTuple union_cocircular_contours_xld_numeric1 = new HTuple();
            HTuple union_cocircular_contours_xld_numeric2 = new HTuple();
            HTuple union_cocircular_contours_xld_numeric3 = new HTuple();
            HTuple union_cocircular_contours_xld_numeric4 = new HTuple();
            HTuple union_cocircular_contours_xld_numeric5 = new HTuple();
            HTuple union_cocircular_contours_xld_numeric6 = new HTuple();

            HTuple hv_Row_SC_2th = new HTuple();//SC:Standard Circle
            HTuple hv_Column_SC_2th = new HTuple();
            HTuple hv_Phi_SC_2th = new HTuple();
            HTuple scale_image_numeric1_2th = new HTuple();
            HTuple scale_image_numeric2_2th = new HTuple();
            HTuple threshold_numeric1_2th = new HTuple();
            HTuple threshold_numeric2_2th = new HTuple();
            HTuple select_shape_numeric1_2th = new HTuple();
            HTuple select_shape_numeric2_2th = new HTuple();
            HTuple edges_sub_pix_numeric1_2th = new HTuple();
            HTuple edges_sub_pix_numeric2_2th = new HTuple();
            HTuple dilation_circle_numeri1_2th = new HTuple();
            HTuple union_cocircular_contours_xld_numeric1_2th = new HTuple();
            HTuple union_cocircular_contours_xld_numeric2_2th = new HTuple();
            HTuple union_cocircular_contours_xld_numeric3_2th = new HTuple();
            HTuple union_cocircular_contours_xld_numeric4_2th = new HTuple();
            HTuple union_cocircular_contours_xld_numeric5_2th = new HTuple();
            HTuple union_cocircular_contours_xld_numeric6_2th = new HTuple();

            //读取保存的shapemodel
            try
            {
                HOperatorSet.ReadShapeModel(shapeModelFileDirectionPath + shapeModelFileName + ".smp", out hv_ModelID_ReadShapeModelData_CircleInspection);
            }
            catch
            {
                MessageBox.Show("读取 " + shapeModelFileName + ".smp" + " 文件失败!");
                readFlag = 1;
                return readFlag;
            }

            //读取保存的shapemodel相关的参数
            try
            {
                HOperatorSet.OpenFile(shapeModelFileDirectionPath + shapeModelFileName + ".smd", "input", out hv_FileHandle);
                HOperatorSet.FreadString(hv_FileHandle, out hv_OutString, out hv_IsEOF);
                HOperatorSet.TupleNumber(hv_OutString, out hv_Number);
                hv_shapeModelData_ReadShapeModelData_CircleInspection[0] = hv_Number;
                hv_i = 1;
                while ((int)(new HTuple(hv_IsEOF.TupleEqual(0))) != 0)
                {
                    HOperatorSet.FreadString(hv_FileHandle, out hv_OutString, out hv_IsEOF);
                    if ((int)(new HTuple(hv_IsEOF.TupleEqual(0))) != 0)
                    {
                        HOperatorSet.TupleNumber(hv_OutString, out hv_Number);
                        hv_shapeModelData_ReadShapeModelData_CircleInspection[hv_i] = hv_Number;
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
                MessageBox.Show("读取 " + shapeModelFileName + ".smd" + " 文件失败!");
                readFlag = 2;
                return readFlag;
            }

            //将读取出来的shapemodel数据赋值给对应的变量
            //hv_Row_SM = hv_shapeModelData_ReadShapeModelData_CircleInspection.TupleSelect(2);
            //hv_Column_SM = hv_shapeModelData_ReadShapeModelData_CircleInspection.TupleSelect(3);
            //hv_Phi_SM = hv_shapeModelData_ReadShapeModelData_CircleInspection.TupleSelect(4);
            //hv_Length1_SM = hv_shapeModelData_ReadShapeModelData_CircleInspection.TupleSelect(5);
            //hv_Length2_SM = hv_shapeModelData_ReadShapeModelData_CircleInspection.TupleSelect(6);

            //读取保存的Rectangle Region相关的参数
            HTuple hv_RRData = new HTuple();
            try
            {
                HTuple hv_RRDataFilePath = shapeModelFileDirectionPath + shapeModelFileName + ".smdd";

                HOperatorSet.OpenFile(hv_RRDataFilePath, "input", out hv_FileHandle);
                HOperatorSet.FreadString(hv_FileHandle, out hv_OutString, out hv_IsEOF);
                HOperatorSet.TupleNumber(hv_OutString, out hv_Number);
                hv_RRData[0] = hv_Number;
                hv_i = 1;
                while ((int)(new HTuple(hv_IsEOF.TupleEqual(0))) != 0)
                {
                    HOperatorSet.FreadString(hv_FileHandle, out hv_OutString, out hv_IsEOF);
                    if ((int)(new HTuple(hv_IsEOF.TupleEqual(0))) != 0)
                    {
                        HOperatorSet.TupleNumber(hv_OutString, out hv_Number);
                        hv_RRData[hv_i] = hv_Number;
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
                MessageBox.Show("读取 " + shapeModelFileName + ".smdd" + " 文件失败!");
                readFlag = 3;
                return readFlag;
            }

            //将读取出来的Rectangle Region数据赋值给对应的变量
            circlesCounterFlag = hv_shapeModelData_ReadShapeModelData_CircleInspection.TupleSelect(0).I;
            circle1FindRegionRectangleOrCircleFlag = hv_RRData.TupleSelect(0).I;
            if (circle1FindRegionRectangleOrCircleFlag == 0 || circle1FindRegionRectangleOrCircleFlag == 1)//查找区域:圆1矩形 或者 圆形
            {
                hv_RRRow_ReadShapeModelData_CircleInspection = hv_RRData.TupleSelect(1);
                hv_RRColumn_ReadShapeModelData_CircleInspection = hv_RRData.TupleSelect(2);
                hv_RRPhi_ReadShapeModelData_CircleInspection = hv_RRData.TupleSelect(3);
                hv_RRLength1_ReadShapeModelData_CircleInspection = hv_RRData.TupleSelect(4);
                hv_RRLength2_ReadShapeModelData_CircleInspection = hv_RRData.TupleSelect(5);
            }
            if (circle1FindRegionRectangleOrCircleFlag == 2)//查找区域:圆1环形
            {
                hv_RRRow_ReadShapeModelData_CircleInspection = hv_RRData.TupleSelect(1);
                hv_RRColumn_ReadShapeModelData_CircleInspection = hv_RRData.TupleSelect(2);
                hv_RRPhi_ReadShapeModelData_CircleInspection = hv_RRData.TupleSelect(3);
                hv_RRPhi2_ReadShapeModelData_CircleInspection = hv_RRData.TupleSelect(4);
                hv_RRLength1_ReadShapeModelData_CircleInspection = hv_RRData.TupleSelect(5);
                hv_RRLength2_ReadShapeModelData_CircleInspection = hv_RRData.TupleSelect(6);
            }
            if (circlesCounterFlag == 2)//查找区域:圆2矩形 或者 圆形
            {
                if (circle1FindRegionRectangleOrCircleFlag == 0 || circle1FindRegionRectangleOrCircleFlag == 1)
                {
                    circle2FindRegionRectangleOrCircleFlag = hv_RRData.TupleSelect(6).I;
                    if ((circle2FindRegionRectangleOrCircleFlag == 0 || circle2FindRegionRectangleOrCircleFlag == 1))
                    {
                        hv_RRRow_ReadShapeModelData_CircleInspection_2th = hv_RRData.TupleSelect(7);
                        hv_RRColumn_ReadShapeModelData_CircleInspection_2th = hv_RRData.TupleSelect(8);
                        hv_RRPhi_ReadShapeModelData_CircleInspection_2th = hv_RRData.TupleSelect(9);
                        hv_RRLength1_ReadShapeModelData_CircleInspection_2th = hv_RRData.TupleSelect(10);
                        hv_RRLength2_ReadShapeModelData_CircleInspection_2th = hv_RRData.TupleSelect(11);
                    }
                    if (circle2FindRegionRectangleOrCircleFlag == 2)
                    {
                        hv_RRRow_ReadShapeModelData_CircleInspection_2th = hv_RRData.TupleSelect(7);
                        hv_RRColumn_ReadShapeModelData_CircleInspection_2th = hv_RRData.TupleSelect(8);
                        hv_RRPhi_ReadShapeModelData_CircleInspection_2th = hv_RRData.TupleSelect(9);
                        hv_RRPhi2_ReadShapeModelData_CircleInspection_2th = hv_RRData.TupleSelect(10);
                        hv_RRLength1_ReadShapeModelData_CircleInspection_2th = hv_RRData.TupleSelect(11);
                        hv_RRLength2_ReadShapeModelData_CircleInspection_2th = hv_RRData.TupleSelect(12);
                    }
                }
                if (circle1FindRegionRectangleOrCircleFlag == 2)
                {
                    circle2FindRegionRectangleOrCircleFlag = hv_RRData.TupleSelect(7).I;
                    if ((circle2FindRegionRectangleOrCircleFlag == 0 || circle2FindRegionRectangleOrCircleFlag == 1))
                    {
                        hv_RRRow_ReadShapeModelData_CircleInspection_2th = hv_RRData.TupleSelect(8);
                        hv_RRColumn_ReadShapeModelData_CircleInspection_2th = hv_RRData.TupleSelect(9);
                        hv_RRPhi_ReadShapeModelData_CircleInspection_2th = hv_RRData.TupleSelect(10);
                        hv_RRLength1_ReadShapeModelData_CircleInspection_2th = hv_RRData.TupleSelect(11);
                        hv_RRLength2_ReadShapeModelData_CircleInspection_2th = hv_RRData.TupleSelect(12);
                    }
                    if (circle2FindRegionRectangleOrCircleFlag == 2)
                    {
                        hv_RRRow_ReadShapeModelData_CircleInspection_2th = hv_RRData.TupleSelect(8);
                        hv_RRColumn_ReadShapeModelData_CircleInspection_2th = hv_RRData.TupleSelect(9);
                        hv_RRPhi_ReadShapeModelData_CircleInspection_2th = hv_RRData.TupleSelect(10);
                        hv_RRPhi2_ReadShapeModelData_CircleInspection_2th = hv_RRData.TupleSelect(11);
                        hv_RRLength1_ReadShapeModelData_CircleInspection_2th = hv_RRData.TupleSelect(12);
                        hv_RRLength2_ReadShapeModelData_CircleInspection_2th = hv_RRData.TupleSelect(13);
                    }
                }
            }

            //参考圆孔的参数(圆心坐标,半径) 及 算子参数
            HTuple hv_ParData = new HTuple();
            try
            {
                HTuple hv_ParDataFilePath = shapeModelFileDirectionPath + shapeModelFileName + ".smddd";

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
                MessageBox.Show("读取 " + shapeModelFileName + ".smddd" + " 文件失败!");
                readFlag = 4;
                return readFlag;
            }

            //将读取出来的数据赋值给对应的变量
            hv_Row_SC = hv_ParData.TupleSelect(0);
            hv_Column_SC = hv_ParData.TupleSelect(1);
            hv_Phi_SC = hv_ParData.TupleSelect(2);
            scale_image_numeric1 = hv_ParData.TupleSelect(3);
            scale_image_numeric2 = hv_ParData.TupleSelect(4);
            threshold_numeric1 = hv_ParData.TupleSelect(5);
            threshold_numeric2 = hv_ParData.TupleSelect(6);
            select_shape_numeric1 = hv_ParData.TupleSelect(7);
            select_shape_numeric2 = hv_ParData.TupleSelect(8);
            edges_sub_pix_numeric1 = hv_ParData.TupleSelect(9);
            edges_sub_pix_numeric2 = hv_ParData.TupleSelect(10);
            dilation_circle_numeri1 = hv_ParData.TupleSelect(11);
            union_cocircular_contours_xld_numeric1 = hv_ParData.TupleSelect(12);
            union_cocircular_contours_xld_numeric2 = hv_ParData.TupleSelect(13);
            union_cocircular_contours_xld_numeric3 = hv_ParData.TupleSelect(14);
            union_cocircular_contours_xld_numeric4 = hv_ParData.TupleSelect(15);
            union_cocircular_contours_xld_numeric5 = hv_ParData.TupleSelect(16);
            union_cocircular_contours_xld_numeric6 = hv_ParData.TupleSelect(17);

            if (hv_ParData.Length == 36)
            {
                hv_Row_SC_2th = hv_ParData.TupleSelect(18);
                hv_Column_SC_2th = hv_ParData.TupleSelect(19);
                hv_Phi_SC_2th = hv_ParData.TupleSelect(20);
                scale_image_numeric1_2th = hv_ParData.TupleSelect(21);
                scale_image_numeric2_2th = hv_ParData.TupleSelect(22);
                threshold_numeric1_2th = hv_ParData.TupleSelect(23);
                threshold_numeric2_2th = hv_ParData.TupleSelect(24);
                select_shape_numeric1_2th = hv_ParData.TupleSelect(25);
                select_shape_numeric2_2th = hv_ParData.TupleSelect(26);
                edges_sub_pix_numeric1_2th = hv_ParData.TupleSelect(27);
                edges_sub_pix_numeric2_2th = hv_ParData.TupleSelect(28);
                dilation_circle_numeri1_2th = hv_ParData.TupleSelect(29);
                union_cocircular_contours_xld_numeric1_2th = hv_ParData.TupleSelect(30);
                union_cocircular_contours_xld_numeric2_2th = hv_ParData.TupleSelect(31);
                union_cocircular_contours_xld_numeric3_2th = hv_ParData.TupleSelect(32);
                union_cocircular_contours_xld_numeric4_2th = hv_ParData.TupleSelect(33);
                union_cocircular_contours_xld_numeric5_2th = hv_ParData.TupleSelect(34);
                union_cocircular_contours_xld_numeric6_2th = hv_ParData.TupleSelect(35);
            }

            return readFlag;
        }
        #endregion

        #region disp_message_Halcon:Halcon消息显示
        /// <summary>
        /// disp_message_Halcon:Halcon消息显示
        /// </summary>
        /// <param name="hv_WindowHandle">HTuple:要显示消息的图像窗口的句柄</param>
        /// <param name="hv_String">HTuple:要显示的消息</param>
        /// <param name="hv_CoordSystem">HTuple:要显示消息的坐标系,image,window</param>
        /// <param name="hv_Row">HTuple:</param>
        /// <param name="hv_Column">HTuple:</param>
        /// <param name="hv_Color">HTuple:</param>
        /// <param name="hv_Box">HTuple:</param>
        /// <returns></returns>
        //例如:
        //hv_Information[0] = "拟合圆失败!失败在第1步!";                  
        //hv_Information[1] = "";            
        //hv_Information[2] = "";          
        //disp_message_Halcon(WindowHandle_CircleInspection, hv_Information, "image", 0, 0, "red", "false");
        //disp_message_Halcon(posInspection.WH, "未能查找到边缘，请采集或读取新图片重试", "image", 0, 0, "red", "true");
        //'black', 'blue', 'yellow', 'red', 'green', 'cyan', 'magenta', 'forest green', 'lime green', 'coral', 'slate blue'
        public void disp_message_Halcon(HTuple hv_WindowHandle, HTuple hv_String,
            HTuple hv_CoordSystem, HTuple hv_Row, HTuple hv_Column, HTuple hv_Color, HTuple hv_Box)
        {
            // Local iconic variables 
            // Local control variables 
            HTuple hv_GenParamName = new HTuple(), hv_GenParamValue = new HTuple();
            HTuple hv_Color_COPY_INP_TMP = new HTuple(hv_Color);
            HTuple hv_Column_COPY_INP_TMP = new HTuple(hv_Column);
            HTuple hv_CoordSystem_COPY_INP_TMP = new HTuple(hv_CoordSystem);
            HTuple hv_Row_COPY_INP_TMP = new HTuple(hv_Row);
            // Initialize local and output iconic variables 
            try
            {
                if ((int)((new HTuple(hv_Row_COPY_INP_TMP.TupleEqual(new HTuple()))).TupleOr(
                    new HTuple(hv_Column_COPY_INP_TMP.TupleEqual(new HTuple())))) != 0)
                {
                    hv_Color_COPY_INP_TMP.Dispose();
                    hv_Column_COPY_INP_TMP.Dispose();
                    hv_CoordSystem_COPY_INP_TMP.Dispose();
                    hv_Row_COPY_INP_TMP.Dispose();
                    hv_GenParamName.Dispose();
                    hv_GenParamValue.Dispose();
                    return;
                }
                if ((int)(new HTuple(hv_Row_COPY_INP_TMP.TupleEqual(-1))) != 0)
                {
                    hv_Row_COPY_INP_TMP.Dispose();
                    hv_Row_COPY_INP_TMP = 12;
                }
                if ((int)(new HTuple(hv_Column_COPY_INP_TMP.TupleEqual(-1))) != 0)
                {
                    hv_Column_COPY_INP_TMP.Dispose();
                    hv_Column_COPY_INP_TMP = 12;
                }
                //Convert the parameter Box to generic parameters.
                hv_GenParamName.Dispose();
                hv_GenParamName = new HTuple();
                hv_GenParamValue.Dispose();
                hv_GenParamValue = new HTuple();
                if ((int)(new HTuple((new HTuple(hv_Box.TupleLength())).TupleGreater(0))) != 0)
                {
                    if ((int)(new HTuple(((hv_Box.TupleSelect(0))).TupleEqual("false"))) != 0)
                    {
                        //Display no box
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_GenParamName = hv_GenParamName.TupleConcat(
                                    "box");
                                hv_GenParamName.Dispose();
                                hv_GenParamName = ExpTmpLocalVar_GenParamName;
                            }
                        }
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_GenParamValue = hv_GenParamValue.TupleConcat(
                                    "false");
                                hv_GenParamValue.Dispose();
                                hv_GenParamValue = ExpTmpLocalVar_GenParamValue;
                            }
                        }
                    }
                    else if ((int)(new HTuple(((hv_Box.TupleSelect(0))).TupleNotEqual(
                        "true"))) != 0)
                    {
                        //Set a color other than the default.
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_GenParamName = hv_GenParamName.TupleConcat(
                                    "box_color");
                                hv_GenParamName.Dispose();
                                hv_GenParamName = ExpTmpLocalVar_GenParamName;
                            }
                        }
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_GenParamValue = hv_GenParamValue.TupleConcat(
                                    hv_Box.TupleSelect(0));
                                hv_GenParamValue.Dispose();
                                hv_GenParamValue = ExpTmpLocalVar_GenParamValue;
                            }
                        }
                    }
                }
                if ((int)(new HTuple((new HTuple(hv_Box.TupleLength())).TupleGreater(1))) != 0)
                {
                    if ((int)(new HTuple(((hv_Box.TupleSelect(1))).TupleEqual("false"))) != 0)
                    {
                        //Display no shadow.
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_GenParamName = hv_GenParamName.TupleConcat(
                                    "shadow");
                                hv_GenParamName.Dispose();
                                hv_GenParamName = ExpTmpLocalVar_GenParamName;
                            }
                        }
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_GenParamValue = hv_GenParamValue.TupleConcat(
                                    "false");
                                hv_GenParamValue.Dispose();
                                hv_GenParamValue = ExpTmpLocalVar_GenParamValue;
                            }
                        }
                    }
                    else if ((int)(new HTuple(((hv_Box.TupleSelect(1))).TupleNotEqual(
                        "true"))) != 0)
                    {
                        //Set a shadow color other than the default.
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_GenParamName = hv_GenParamName.TupleConcat(
                                    "shadow_color");
                                hv_GenParamName.Dispose();
                                hv_GenParamName = ExpTmpLocalVar_GenParamName;
                            }
                        }
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_GenParamValue = hv_GenParamValue.TupleConcat(
                                    hv_Box.TupleSelect(1));
                                hv_GenParamValue.Dispose();
                                hv_GenParamValue = ExpTmpLocalVar_GenParamValue;
                            }
                        }
                    }
                }
                //Restore default CoordSystem behavior.
                if ((int)(new HTuple(hv_CoordSystem_COPY_INP_TMP.TupleNotEqual("window"))) != 0)
                {
                    hv_CoordSystem_COPY_INP_TMP.Dispose();
                    hv_CoordSystem_COPY_INP_TMP = "image";
                }
                //
                if ((int)(new HTuple(hv_Color_COPY_INP_TMP.TupleEqual(""))) != 0)
                {
                    //disp_text does not accept an empty string for Color.
                    hv_Color_COPY_INP_TMP.Dispose();
                    hv_Color_COPY_INP_TMP = new HTuple();
                }
                //
                HOperatorSet.DispText(hv_WindowHandle, hv_String, hv_CoordSystem_COPY_INP_TMP,
                    hv_Row_COPY_INP_TMP, hv_Column_COPY_INP_TMP, hv_Color_COPY_INP_TMP, hv_GenParamName,
                    hv_GenParamValue);

                hv_Color_COPY_INP_TMP.Dispose();
                hv_Column_COPY_INP_TMP.Dispose();
                hv_CoordSystem_COPY_INP_TMP.Dispose();
                hv_Row_COPY_INP_TMP.Dispose();
                hv_GenParamName.Dispose();
                hv_GenParamValue.Dispose();
                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                hv_Color_COPY_INP_TMP.Dispose();
                hv_Column_COPY_INP_TMP.Dispose();
                hv_CoordSystem_COPY_INP_TMP.Dispose();
                hv_Row_COPY_INP_TMP.Dispose();
                hv_GenParamName.Dispose();
                hv_GenParamValue.Dispose();
                throw HDevExpDefaultException;
            }
        }
        #endregion

        #region set_display_font_Halcon:设置显示文字的字体
        /// <summary>
        /// set_display_font_Halcon:设置显示文字的字体
        /// </summary>
        /// <param name="hv_WindowHandle">HTuple:要显示消息的图像窗口的句柄</param>
        /// <returns></returns>
        public void set_display_font_Halcon(HTuple hv_WindowHandle, HTuple hv_Size, HTuple hv_Font,
            HTuple hv_Bold, HTuple hv_Slant)
        {
            // Local iconic variables 

            // Local control variables 

            HTuple hv_OS = new HTuple(), hv_Fonts = new HTuple();
            HTuple hv_Style = new HTuple(), hv_Exception = new HTuple();
            HTuple hv_AvailableFonts = new HTuple(), hv_Fdx = new HTuple();
            HTuple hv_Indices = new HTuple();
            HTuple hv_Font_COPY_INP_TMP = new HTuple(hv_Font);
            HTuple hv_Size_COPY_INP_TMP = new HTuple(hv_Size);

            // Initialize local and output iconic variables 
            try
            {
                //This procedure sets the text font of the current window with
                //the specified attributes.
                //
                //Input parameters:
                //WindowHandle: The graphics window for which the font will be set
                //Size: The font size. If Size=-1, the default of 16 is used.
                //Bold: If set to 'true', a bold font is used
                //Slant: If set to 'true', a slanted font is used
                //
                hv_OS.Dispose();
                HOperatorSet.GetSystem("operating_system", out hv_OS);
                if ((int)((new HTuple(hv_Size_COPY_INP_TMP.TupleEqual(new HTuple()))).TupleOr(
                    new HTuple(hv_Size_COPY_INP_TMP.TupleEqual(-1)))) != 0)
                {
                    hv_Size_COPY_INP_TMP.Dispose();
                    hv_Size_COPY_INP_TMP = 16;
                }
                if ((int)(new HTuple(((hv_OS.TupleSubstr(0, 2))).TupleEqual("Win"))) != 0)
                {
                    //Restore previous behaviour
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_Size = ((1.13677 * hv_Size_COPY_INP_TMP)).TupleInt()
                                ;
                            hv_Size_COPY_INP_TMP.Dispose();
                            hv_Size_COPY_INP_TMP = ExpTmpLocalVar_Size;
                        }
                    }
                }
                else
                {
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_Size = hv_Size_COPY_INP_TMP.TupleInt()
                                ;
                            hv_Size_COPY_INP_TMP.Dispose();
                            hv_Size_COPY_INP_TMP = ExpTmpLocalVar_Size;
                        }
                    }
                }
                if ((int)(new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("Courier"))) != 0)
                {
                    hv_Fonts.Dispose();
                    hv_Fonts = new HTuple();
                    hv_Fonts[0] = "Courier";
                    hv_Fonts[1] = "Courier 10 Pitch";
                    hv_Fonts[2] = "Courier New";
                    hv_Fonts[3] = "CourierNew";
                    hv_Fonts[4] = "Liberation Mono";
                }
                else if ((int)(new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("mono"))) != 0)
                {
                    hv_Fonts.Dispose();
                    hv_Fonts = new HTuple();
                    hv_Fonts[0] = "Consolas";
                    hv_Fonts[1] = "Menlo";
                    hv_Fonts[2] = "Courier";
                    hv_Fonts[3] = "Courier 10 Pitch";
                    hv_Fonts[4] = "FreeMono";
                    hv_Fonts[5] = "Liberation Mono";
                }
                else if ((int)(new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("sans"))) != 0)
                {
                    hv_Fonts.Dispose();
                    hv_Fonts = new HTuple();
                    hv_Fonts[0] = "Luxi Sans";
                    hv_Fonts[1] = "DejaVu Sans";
                    hv_Fonts[2] = "FreeSans";
                    hv_Fonts[3] = "Arial";
                    hv_Fonts[4] = "Liberation Sans";
                }
                else if ((int)(new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("serif"))) != 0)
                {
                    hv_Fonts.Dispose();
                    hv_Fonts = new HTuple();
                    hv_Fonts[0] = "Times New Roman";
                    hv_Fonts[1] = "Luxi Serif";
                    hv_Fonts[2] = "DejaVu Serif";
                    hv_Fonts[3] = "FreeSerif";
                    hv_Fonts[4] = "Utopia";
                    hv_Fonts[5] = "Liberation Serif";
                }
                else
                {
                    hv_Fonts.Dispose();
                    hv_Fonts = new HTuple(hv_Font_COPY_INP_TMP);
                }
                hv_Style.Dispose();
                hv_Style = "";
                if ((int)(new HTuple(hv_Bold.TupleEqual("true"))) != 0)
                {
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_Style = hv_Style + "Bold";
                            hv_Style.Dispose();
                            hv_Style = ExpTmpLocalVar_Style;
                        }
                    }
                }
                else if ((int)(new HTuple(hv_Bold.TupleNotEqual("false"))) != 0)
                {
                    hv_Exception.Dispose();
                    hv_Exception = "Wrong value of control parameter Bold";
                    throw new HalconException(hv_Exception);
                }
                if ((int)(new HTuple(hv_Slant.TupleEqual("true"))) != 0)
                {
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_Style = hv_Style + "Italic";
                            hv_Style.Dispose();
                            hv_Style = ExpTmpLocalVar_Style;
                        }
                    }
                }
                else if ((int)(new HTuple(hv_Slant.TupleNotEqual("false"))) != 0)
                {
                    hv_Exception.Dispose();
                    hv_Exception = "Wrong value of control parameter Slant";
                    throw new HalconException(hv_Exception);
                }
                if ((int)(new HTuple(hv_Style.TupleEqual(""))) != 0)
                {
                    hv_Style.Dispose();
                    hv_Style = "Normal";
                }
                hv_AvailableFonts.Dispose();
                HOperatorSet.QueryFont(hv_WindowHandle, out hv_AvailableFonts);
                hv_Font_COPY_INP_TMP.Dispose();
                hv_Font_COPY_INP_TMP = "";
                for (hv_Fdx = 0; (int)hv_Fdx <= (int)((new HTuple(hv_Fonts.TupleLength())) - 1); hv_Fdx = (int)hv_Fdx + 1)
                {
                    hv_Indices.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Indices = hv_AvailableFonts.TupleFind(
                            hv_Fonts.TupleSelect(hv_Fdx));
                    }
                    if ((int)(new HTuple((new HTuple(hv_Indices.TupleLength())).TupleGreater(
                        0))) != 0)
                    {
                        if ((int)(new HTuple(((hv_Indices.TupleSelect(0))).TupleGreaterEqual(0))) != 0)
                        {
                            hv_Font_COPY_INP_TMP.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_Font_COPY_INP_TMP = hv_Fonts.TupleSelect(
                                    hv_Fdx);
                            }
                            break;
                        }
                    }
                }
                if ((int)(new HTuple(hv_Font_COPY_INP_TMP.TupleEqual(""))) != 0)
                {
                    throw new HalconException("Wrong value of control parameter Font");
                }
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    {
                        HTuple
                          ExpTmpLocalVar_Font = (((hv_Font_COPY_INP_TMP + "-") + hv_Style) + "-") + hv_Size_COPY_INP_TMP;
                        hv_Font_COPY_INP_TMP.Dispose();
                        hv_Font_COPY_INP_TMP = ExpTmpLocalVar_Font;
                    }
                }
                HOperatorSet.SetFont(hv_WindowHandle, hv_Font_COPY_INP_TMP);

                hv_Font_COPY_INP_TMP.Dispose();
                hv_Size_COPY_INP_TMP.Dispose();
                hv_OS.Dispose();
                hv_Fonts.Dispose();
                hv_Style.Dispose();
                hv_Exception.Dispose();
                hv_AvailableFonts.Dispose();
                hv_Fdx.Dispose();
                hv_Indices.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {

                hv_Font_COPY_INP_TMP.Dispose();
                hv_Size_COPY_INP_TMP.Dispose();
                hv_OS.Dispose();
                hv_Fonts.Dispose();
                hv_Style.Dispose();
                hv_Exception.Dispose();
                hv_AvailableFonts.Dispose();
                hv_Fdx.Dispose();
                hv_Indices.Dispose();

                throw HDevExpDefaultException;
            }
        }
        #endregion

        #region draw_annular
        public void draw_annular(HTuple hv_WindowHandle, HTuple hv_Information, HObject ho_ImagePre, HObject ho_WindowImagePre, string color, HTuple hv_InCenterRow, HTuple hv_InCenterCol, HTuple hv_InRadiusInner, HTuple hv_InRadiusOuter, HTuple hv_InStartPhi, HTuple hv_InEndPhi, out HTuple hv_CenterRow, out HTuple hv_CenterCol, out HTuple hv_RadiusInner, out HTuple hv_RadiusOuter, out HTuple hv_StartPhi, out HTuple hv_EndPhi)
        {
            // Local iconic variables 

            HObject ho_xld_o, ho_xld_i, ho_rect_1, ho_rect_2;

            // Local control variables 

            HTuple hv_StartRow_O = new HTuple(), hv_StartCol_O = new HTuple();
            HTuple hv_StartRow_I = new HTuple(), hv_StartCol_I = new HTuple();
            HTuple hv_EndRow_O = new HTuple(), hv_EndCol_O = new HTuple();
            HTuple hv_EndRow_I = new HTuple(), hv_EndCol_I = new HTuple();
            HTuple hv_order = new HTuple(), hv_AngleStartRow = new HTuple();
            HTuple hv_AngleStartCol = new HTuple(), hv_AngleEndRow = new HTuple();
            HTuple hv_AngleEndCol = new HTuple(), hv_flag2 = new HTuple();
            HTuple hv_Row = new HTuple(), hv_Col = new HTuple(), hv_Button = new HTuple();
            HTuple hv_min_ = new HTuple(), hv_max_ = new HTuple();
            HTuple hv_DistanceMin = new HTuple(), hv_DistanceMax = new HTuple();
            HTuple hv_indice = new HTuple(), hv_flag = new HTuple();
            HTuple hv_Distance = new HTuple(), hv_Phi = new HTuple();
            HTuple hv_EndAngle = new HTuple();
            HTuple hv_InCenterCol_COPY_INP_TMP = new HTuple(hv_InCenterCol);
            HTuple hv_InCenterRow_COPY_INP_TMP = new HTuple(hv_InCenterRow);
            HTuple hv_InRadiusInner_COPY_INP_TMP = new HTuple(hv_InRadiusInner);
            HTuple hv_InRadiusOuter_COPY_INP_TMP = new HTuple(hv_InRadiusOuter);
            HTuple hv_InStartPhi_COPY_INP_TMP = new HTuple(hv_InStartPhi);

            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_xld_o);
            HOperatorSet.GenEmptyObj(out ho_xld_i);
            HOperatorSet.GenEmptyObj(out ho_rect_1);
            HOperatorSet.GenEmptyObj(out ho_rect_2);
            hv_CenterRow = new HTuple();
            hv_CenterCol = new HTuple();
            hv_RadiusInner = new HTuple();
            hv_RadiusOuter = new HTuple();
            hv_StartPhi = new HTuple();
            hv_EndPhi = new HTuple();

            if ((int)(new HTuple(hv_WindowHandle.TupleEqual(new HTuple()))) != 0)
            {
                ho_xld_o.Dispose();
                ho_xld_i.Dispose();
                ho_rect_1.Dispose();
                ho_rect_2.Dispose();

                hv_InCenterCol_COPY_INP_TMP.Dispose();
                hv_InCenterRow_COPY_INP_TMP.Dispose();
                hv_InRadiusInner_COPY_INP_TMP.Dispose();
                hv_InRadiusOuter_COPY_INP_TMP.Dispose();
                hv_InStartPhi_COPY_INP_TMP.Dispose();
                hv_StartRow_O.Dispose();
                hv_StartCol_O.Dispose();
                hv_StartRow_I.Dispose();
                hv_StartCol_I.Dispose();
                hv_EndRow_O.Dispose();
                hv_EndCol_O.Dispose();
                hv_EndRow_I.Dispose();
                hv_EndCol_I.Dispose();
                hv_order.Dispose();
                hv_AngleStartRow.Dispose();
                hv_AngleStartCol.Dispose();
                hv_AngleEndRow.Dispose();
                hv_AngleEndCol.Dispose();
                hv_flag2.Dispose();
                hv_Row.Dispose();
                hv_Col.Dispose();
                hv_Button.Dispose();
                hv_min_.Dispose();
                hv_max_.Dispose();
                hv_DistanceMin.Dispose();
                hv_DistanceMax.Dispose();
                hv_indice.Dispose();
                hv_flag.Dispose();
                hv_Distance.Dispose();
                hv_Phi.Dispose();
                hv_EndAngle.Dispose();

                return;
            }
            if ((int)((new HTuple(hv_InCenterRow_COPY_INP_TMP.TupleEqual(new HTuple()))).TupleOr(
                new HTuple(hv_InCenterCol_COPY_INP_TMP.TupleEqual(new HTuple())))) != 0)
            {
                ho_xld_o.Dispose();
                ho_xld_i.Dispose();
                ho_rect_1.Dispose();
                ho_rect_2.Dispose();

                hv_InCenterCol_COPY_INP_TMP.Dispose();
                hv_InCenterRow_COPY_INP_TMP.Dispose();
                hv_InRadiusInner_COPY_INP_TMP.Dispose();
                hv_InRadiusOuter_COPY_INP_TMP.Dispose();
                hv_InStartPhi_COPY_INP_TMP.Dispose();
                hv_StartRow_O.Dispose();
                hv_StartCol_O.Dispose();
                hv_StartRow_I.Dispose();
                hv_StartCol_I.Dispose();
                hv_EndRow_O.Dispose();
                hv_EndCol_O.Dispose();
                hv_EndRow_I.Dispose();
                hv_EndCol_I.Dispose();
                hv_order.Dispose();
                hv_AngleStartRow.Dispose();
                hv_AngleStartCol.Dispose();
                hv_AngleEndRow.Dispose();
                hv_AngleEndCol.Dispose();
                hv_flag2.Dispose();
                hv_Row.Dispose();
                hv_Col.Dispose();
                hv_Button.Dispose();
                hv_min_.Dispose();
                hv_max_.Dispose();
                hv_DistanceMin.Dispose();
                hv_DistanceMax.Dispose();
                hv_indice.Dispose();
                hv_flag.Dispose();
                hv_Distance.Dispose();
                hv_Phi.Dispose();
                hv_EndAngle.Dispose();

                return;
            }
            if ((int)((new HTuple(hv_InRadiusInner_COPY_INP_TMP.TupleEqual(new HTuple()))).TupleOr(
                new HTuple(hv_InRadiusOuter_COPY_INP_TMP.TupleEqual(new HTuple())))) != 0)
            {
                ho_xld_o.Dispose();
                ho_xld_i.Dispose();
                ho_rect_1.Dispose();
                ho_rect_2.Dispose();

                hv_InCenterCol_COPY_INP_TMP.Dispose();
                hv_InCenterRow_COPY_INP_TMP.Dispose();
                hv_InRadiusInner_COPY_INP_TMP.Dispose();
                hv_InRadiusOuter_COPY_INP_TMP.Dispose();
                hv_InStartPhi_COPY_INP_TMP.Dispose();
                hv_StartRow_O.Dispose();
                hv_StartCol_O.Dispose();
                hv_StartRow_I.Dispose();
                hv_StartCol_I.Dispose();
                hv_EndRow_O.Dispose();
                hv_EndCol_O.Dispose();
                hv_EndRow_I.Dispose();
                hv_EndCol_I.Dispose();
                hv_order.Dispose();
                hv_AngleStartRow.Dispose();
                hv_AngleStartCol.Dispose();
                hv_AngleEndRow.Dispose();
                hv_AngleEndCol.Dispose();
                hv_flag2.Dispose();
                hv_Row.Dispose();
                hv_Col.Dispose();
                hv_Button.Dispose();
                hv_min_.Dispose();
                hv_max_.Dispose();
                hv_DistanceMin.Dispose();
                hv_DistanceMax.Dispose();
                hv_indice.Dispose();
                hv_flag.Dispose();
                hv_Distance.Dispose();
                hv_Phi.Dispose();
                hv_EndAngle.Dispose();

                return;
            }
            if ((int)((new HTuple(hv_InStartPhi_COPY_INP_TMP.TupleLess(0))).TupleAnd(new HTuple(hv_InStartPhi_COPY_INP_TMP.TupleGreater(
                360)))) != 0)
            {
                ho_xld_o.Dispose();
                ho_xld_i.Dispose();
                ho_rect_1.Dispose();
                ho_rect_2.Dispose();

                hv_InCenterCol_COPY_INP_TMP.Dispose();
                hv_InCenterRow_COPY_INP_TMP.Dispose();
                hv_InRadiusInner_COPY_INP_TMP.Dispose();
                hv_InRadiusOuter_COPY_INP_TMP.Dispose();
                hv_InStartPhi_COPY_INP_TMP.Dispose();
                hv_StartRow_O.Dispose();
                hv_StartCol_O.Dispose();
                hv_StartRow_I.Dispose();
                hv_StartCol_I.Dispose();
                hv_EndRow_O.Dispose();
                hv_EndCol_O.Dispose();
                hv_EndRow_I.Dispose();
                hv_EndCol_I.Dispose();
                hv_order.Dispose();
                hv_AngleStartRow.Dispose();
                hv_AngleStartCol.Dispose();
                hv_AngleEndRow.Dispose();
                hv_AngleEndCol.Dispose();
                hv_flag2.Dispose();
                hv_Row.Dispose();
                hv_Col.Dispose();
                hv_Button.Dispose();
                hv_min_.Dispose();
                hv_max_.Dispose();
                hv_DistanceMin.Dispose();
                hv_DistanceMax.Dispose();
                hv_indice.Dispose();
                hv_flag.Dispose();
                hv_Distance.Dispose();
                hv_Phi.Dispose();
                hv_EndAngle.Dispose();

                return;
            }
            if ((int)((new HTuple(hv_InEndPhi.TupleLess(0))).TupleOr(new HTuple(hv_InEndPhi.TupleGreaterEqual(
                360)))) != 0)
            {
                ho_xld_o.Dispose();
                ho_xld_i.Dispose();
                ho_rect_1.Dispose();
                ho_rect_2.Dispose();

                hv_InCenterCol_COPY_INP_TMP.Dispose();
                hv_InCenterRow_COPY_INP_TMP.Dispose();
                hv_InRadiusInner_COPY_INP_TMP.Dispose();
                hv_InRadiusOuter_COPY_INP_TMP.Dispose();
                hv_InStartPhi_COPY_INP_TMP.Dispose();
                hv_StartRow_O.Dispose();
                hv_StartCol_O.Dispose();
                hv_StartRow_I.Dispose();
                hv_StartCol_I.Dispose();
                hv_EndRow_O.Dispose();
                hv_EndCol_O.Dispose();
                hv_EndRow_I.Dispose();
                hv_EndCol_I.Dispose();
                hv_order.Dispose();
                hv_AngleStartRow.Dispose();
                hv_AngleStartCol.Dispose();
                hv_AngleEndRow.Dispose();
                hv_AngleEndCol.Dispose();
                hv_flag2.Dispose();
                hv_Row.Dispose();
                hv_Col.Dispose();
                hv_Button.Dispose();
                hv_min_.Dispose();
                hv_max_.Dispose();
                hv_DistanceMin.Dispose();
                hv_DistanceMax.Dispose();
                hv_indice.Dispose();
                hv_flag.Dispose();
                hv_Distance.Dispose();
                hv_Phi.Dispose();
                hv_EndAngle.Dispose();

                return;
            }

            HOperatorSet.SetSystem("flush_graphic", "false");

            hv_StartRow_O.Dispose();
            hv_StartRow_O = new HTuple();
            hv_StartCol_O.Dispose();
            hv_StartCol_O = new HTuple();
            hv_StartRow_I.Dispose();
            hv_StartRow_I = new HTuple();
            hv_StartCol_I.Dispose();
            hv_StartCol_I = new HTuple();
            //内外圆起点
            if ((int)((new HTuple(hv_InStartPhi_COPY_INP_TMP.TupleEqual(0))).TupleOr(new HTuple(hv_InStartPhi_COPY_INP_TMP.TupleEqual(
                360)))) != 0)
            {
                //X轴正向
                hv_StartRow_O.Dispose();
                hv_StartRow_O = new HTuple(hv_InCenterRow_COPY_INP_TMP);
                hv_StartCol_O.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_StartCol_O = hv_InCenterCol_COPY_INP_TMP + hv_InRadiusOuter_COPY_INP_TMP;
                }
                hv_StartRow_I.Dispose();
                hv_StartRow_I = new HTuple(hv_InCenterRow_COPY_INP_TMP);
                hv_StartCol_I.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_StartCol_I = hv_InCenterCol_COPY_INP_TMP + hv_InRadiusInner_COPY_INP_TMP;
                }
            }
            else if ((int)((new HTuple(hv_InStartPhi_COPY_INP_TMP.TupleGreater(0))).TupleAnd(
                new HTuple(hv_InStartPhi_COPY_INP_TMP.TupleLess(90)))) != 0)
            {
                //第一象限
                hv_StartRow_O.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_StartRow_O = hv_InCenterRow_COPY_INP_TMP - (hv_InRadiusOuter_COPY_INP_TMP * (hv_InStartPhi_COPY_INP_TMP.TupleSin()
                        ));
                }
                hv_StartCol_O.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_StartCol_O = hv_InCenterCol_COPY_INP_TMP + (hv_InRadiusOuter_COPY_INP_TMP * (hv_InStartPhi_COPY_INP_TMP.TupleCos()
                        ));
                }
                hv_StartRow_I.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_StartRow_I = hv_InCenterRow_COPY_INP_TMP - (hv_InRadiusInner_COPY_INP_TMP * (hv_InStartPhi_COPY_INP_TMP.TupleSin()
                        ));
                }
                hv_StartCol_I.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_StartCol_I = hv_InCenterCol_COPY_INP_TMP + (hv_InRadiusInner_COPY_INP_TMP * (hv_InStartPhi_COPY_INP_TMP.TupleCos()
                        ));
                }
            }
            else if ((int)(new HTuple(hv_InStartPhi_COPY_INP_TMP.TupleEqual(90))) != 0)
            {
                //Y轴正向
                hv_StartRow_O.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_StartRow_O = hv_InCenterRow_COPY_INP_TMP - hv_InRadiusOuter_COPY_INP_TMP;
                }
                hv_StartCol_O.Dispose();
                hv_StartCol_O = new HTuple(hv_InCenterCol_COPY_INP_TMP);
                hv_StartRow_I.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_StartRow_I = hv_InCenterRow_COPY_INP_TMP - hv_InRadiusInner_COPY_INP_TMP;
                }
                hv_StartCol_I.Dispose();
                hv_StartCol_I = new HTuple(hv_InCenterCol_COPY_INP_TMP);
            }
            else if ((int)((new HTuple(hv_InStartPhi_COPY_INP_TMP.TupleGreater(90))).TupleAnd(
                new HTuple(hv_InStartPhi_COPY_INP_TMP.TupleLess(180)))) != 0)
            {
                //第二象限
                hv_StartRow_O.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_StartRow_O = hv_InCenterRow_COPY_INP_TMP - (hv_InRadiusOuter_COPY_INP_TMP * (((180 - hv_InStartPhi_COPY_INP_TMP)).TupleSin()
                        ));
                }
                hv_StartCol_O.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_StartCol_O = hv_InCenterCol_COPY_INP_TMP - (hv_InRadiusOuter_COPY_INP_TMP * (((180 - hv_InStartPhi_COPY_INP_TMP)).TupleCos()
                        ));
                }
                hv_StartRow_I.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_StartRow_I = hv_InCenterRow_COPY_INP_TMP - (hv_InRadiusInner_COPY_INP_TMP * (((180 - hv_InStartPhi_COPY_INP_TMP)).TupleSin()
                        ));
                }
                hv_StartCol_I.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_StartCol_I = hv_InCenterCol_COPY_INP_TMP - (hv_InRadiusInner_COPY_INP_TMP * (((180 - hv_InStartPhi_COPY_INP_TMP)).TupleCos()
                        ));
                }
            }
            else if ((int)(new HTuple(hv_InStartPhi_COPY_INP_TMP.TupleEqual(180))) != 0)
            {
                //X轴负向
                hv_StartRow_O.Dispose();
                hv_StartRow_O = new HTuple(hv_InCenterRow_COPY_INP_TMP);
                hv_StartCol_O.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_StartCol_O = hv_InCenterCol_COPY_INP_TMP - hv_InRadiusOuter_COPY_INP_TMP;
                }
                hv_StartRow_I.Dispose();
                hv_StartRow_I = new HTuple(hv_InCenterRow_COPY_INP_TMP);
                hv_StartCol_I.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_StartCol_I = hv_InCenterCol_COPY_INP_TMP - hv_InRadiusInner_COPY_INP_TMP;
                }
            }
            else if ((int)((new HTuple(hv_InStartPhi_COPY_INP_TMP.TupleGreater(180))).TupleAnd(
                new HTuple(hv_InStartPhi_COPY_INP_TMP.TupleLess(270)))) != 0)
            {
                //第三象限
                hv_StartRow_O.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_StartRow_O = hv_InCenterRow_COPY_INP_TMP + (hv_InRadiusOuter_COPY_INP_TMP * (((hv_InStartPhi_COPY_INP_TMP - 180)).TupleSin()
                        ));
                }
                hv_StartCol_O.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_StartCol_O = hv_InCenterCol_COPY_INP_TMP - (hv_InRadiusOuter_COPY_INP_TMP * (((hv_InStartPhi_COPY_INP_TMP - 180)).TupleCos()
                        ));
                }
                hv_StartRow_I.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_StartRow_I = hv_InCenterRow_COPY_INP_TMP + (hv_InRadiusInner_COPY_INP_TMP * (((hv_InStartPhi_COPY_INP_TMP - 180)).TupleSin()
                        ));
                }
                hv_StartCol_I.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_StartCol_I = hv_InCenterCol_COPY_INP_TMP - (hv_InRadiusInner_COPY_INP_TMP * (((hv_InStartPhi_COPY_INP_TMP - 180)).TupleCos()
                        ));
                }
            }
            else if ((int)(new HTuple(hv_InStartPhi_COPY_INP_TMP.TupleEqual(270))) != 0)
            {
                //Y轴负向
                hv_StartRow_O.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_StartRow_O = hv_InCenterRow_COPY_INP_TMP + hv_InRadiusOuter_COPY_INP_TMP;
                }
                hv_StartCol_O.Dispose();
                hv_StartCol_O = new HTuple(hv_InCenterCol_COPY_INP_TMP);
                hv_StartRow_I.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_StartRow_I = hv_InCenterRow_COPY_INP_TMP + hv_InRadiusInner_COPY_INP_TMP;
                }
                hv_StartCol_I.Dispose();
                hv_StartCol_I = new HTuple(hv_InCenterCol_COPY_INP_TMP);
            }
            else if ((int)((new HTuple(hv_InStartPhi_COPY_INP_TMP.TupleGreater(270))).TupleAnd(
                new HTuple(hv_InStartPhi_COPY_INP_TMP.TupleLess(360)))) != 0)
            {
                //第四象限
                hv_StartRow_O.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_StartRow_O = hv_InCenterRow_COPY_INP_TMP + (hv_InRadiusOuter_COPY_INP_TMP * (((360 - hv_InStartPhi_COPY_INP_TMP)).TupleSin()
                        ));
                }
                hv_StartCol_O.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_StartCol_O = hv_InCenterCol_COPY_INP_TMP + (hv_InRadiusOuter_COPY_INP_TMP * (((360 - hv_InStartPhi_COPY_INP_TMP)).TupleCos()
                        ));
                }
                hv_StartRow_I.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_StartRow_I = hv_InCenterRow_COPY_INP_TMP + (hv_InRadiusInner_COPY_INP_TMP * (((360 - hv_InStartPhi_COPY_INP_TMP)).TupleSin()
                        ));
                }
                hv_StartCol_I.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_StartCol_I = hv_InCenterCol_COPY_INP_TMP + (hv_InRadiusInner_COPY_INP_TMP * (((360 - hv_InStartPhi_COPY_INP_TMP)).TupleCos()
                        ));
                }
            }

            //内外圆终点
            hv_EndRow_O.Dispose();
            hv_EndRow_O = new HTuple();
            hv_EndCol_O.Dispose();
            hv_EndCol_O = new HTuple();
            hv_EndRow_I.Dispose();
            hv_EndRow_I = new HTuple();
            hv_EndCol_I.Dispose();
            hv_EndCol_I = new HTuple();
            //内外圆起点
            if ((int)((new HTuple(hv_InEndPhi.TupleEqual(0))).TupleOr(new HTuple(hv_InEndPhi.TupleEqual(
                360)))) != 0)
            {
                //X轴正向
                hv_EndRow_O.Dispose();
                hv_EndRow_O = new HTuple(hv_InCenterRow_COPY_INP_TMP);
                hv_EndCol_O.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_EndCol_O = hv_InCenterCol_COPY_INP_TMP + hv_InRadiusOuter_COPY_INP_TMP;
                }
                hv_EndRow_I.Dispose();
                hv_EndRow_I = new HTuple(hv_InCenterRow_COPY_INP_TMP);
                hv_EndCol_I.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_EndCol_I = hv_InCenterCol_COPY_INP_TMP + hv_InRadiusInner_COPY_INP_TMP;
                }
            }
            else if ((int)((new HTuple(hv_InEndPhi.TupleGreater(0))).TupleAnd(new HTuple(hv_InEndPhi.TupleLess(
                90)))) != 0)
            {
                //第一象限
                hv_EndRow_O.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_EndRow_O = hv_InCenterRow_COPY_INP_TMP - (hv_InRadiusOuter_COPY_INP_TMP * (hv_InStartPhi_COPY_INP_TMP.TupleSin()
                        ));
                }
                hv_EndCol_O.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_EndCol_O = hv_InCenterCol_COPY_INP_TMP + (hv_InRadiusOuter_COPY_INP_TMP * (hv_InStartPhi_COPY_INP_TMP.TupleCos()
                        ));
                }
                hv_EndRow_I.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_EndRow_I = hv_InCenterRow_COPY_INP_TMP - (hv_InRadiusInner_COPY_INP_TMP * (hv_InStartPhi_COPY_INP_TMP.TupleSin()
                        ));
                }
                hv_EndCol_I.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_EndCol_I = hv_InCenterCol_COPY_INP_TMP + (hv_InRadiusInner_COPY_INP_TMP * (hv_InStartPhi_COPY_INP_TMP.TupleCos()
                        ));
                }
            }
            else if ((int)(new HTuple(hv_InEndPhi.TupleEqual(90))) != 0)
            {
                //Y轴正向
                hv_EndRow_O.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_EndRow_O = hv_InCenterRow_COPY_INP_TMP - hv_InRadiusOuter_COPY_INP_TMP;
                }
                hv_EndCol_O.Dispose();
                hv_EndCol_O = new HTuple(hv_InCenterCol_COPY_INP_TMP);
                hv_EndRow_I.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_EndRow_I = hv_InCenterRow_COPY_INP_TMP - hv_InRadiusInner_COPY_INP_TMP;
                }
                hv_EndCol_I.Dispose();
                hv_EndCol_I = new HTuple(hv_InCenterCol_COPY_INP_TMP);
            }
            else if ((int)((new HTuple(hv_InEndPhi.TupleGreater(90))).TupleAnd(new HTuple(hv_InEndPhi.TupleLess(
                180)))) != 0)
            {
                //第二象限
                hv_EndRow_O.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_EndRow_O = hv_InCenterRow_COPY_INP_TMP - (hv_InRadiusOuter_COPY_INP_TMP * (((180 - hv_InStartPhi_COPY_INP_TMP)).TupleSin()
                        ));
                }
                hv_EndCol_O.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_EndCol_O = hv_InCenterCol_COPY_INP_TMP - (hv_InRadiusOuter_COPY_INP_TMP * (((180 - hv_InStartPhi_COPY_INP_TMP)).TupleCos()
                        ));
                }
                hv_EndRow_I.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_EndRow_I = hv_InCenterRow_COPY_INP_TMP - (hv_InRadiusInner_COPY_INP_TMP * (((180 - hv_InStartPhi_COPY_INP_TMP)).TupleSin()
                        ));
                }
                hv_EndCol_I.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_EndCol_I = hv_InCenterCol_COPY_INP_TMP - (hv_InRadiusInner_COPY_INP_TMP * (((180 - hv_InStartPhi_COPY_INP_TMP)).TupleCos()
                        ));
                }
            }
            else if ((int)(new HTuple(hv_InEndPhi.TupleEqual(180))) != 0)
            {
                //X轴负向
                hv_EndRow_O.Dispose();
                hv_EndRow_O = new HTuple(hv_InCenterRow_COPY_INP_TMP);
                hv_EndCol_O.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_EndCol_O = hv_InCenterCol_COPY_INP_TMP - hv_InRadiusOuter_COPY_INP_TMP;
                }
                hv_EndRow_I.Dispose();
                hv_EndRow_I = new HTuple(hv_InCenterRow_COPY_INP_TMP);
                hv_EndCol_I.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_EndCol_I = hv_InCenterCol_COPY_INP_TMP - hv_InRadiusInner_COPY_INP_TMP;
                }
            }
            else if ((int)((new HTuple(hv_InEndPhi.TupleGreater(180))).TupleAnd(new HTuple(hv_InEndPhi.TupleLess(
                270)))) != 0)
            {
                //第三象限
                hv_EndRow_O.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_EndRow_O = hv_InCenterRow_COPY_INP_TMP + (hv_InRadiusOuter_COPY_INP_TMP * (((hv_InStartPhi_COPY_INP_TMP - 180)).TupleSin()
                        ));
                }
                hv_EndCol_O.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_EndCol_O = hv_InCenterCol_COPY_INP_TMP - (hv_InRadiusOuter_COPY_INP_TMP * (((hv_InStartPhi_COPY_INP_TMP - 180)).TupleCos()
                        ));
                }
                hv_EndRow_I.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_EndRow_I = hv_InCenterRow_COPY_INP_TMP + (hv_InRadiusInner_COPY_INP_TMP * (((hv_InStartPhi_COPY_INP_TMP - 180)).TupleSin()
                        ));
                }
                hv_EndCol_I.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_EndCol_I = hv_InCenterCol_COPY_INP_TMP - (hv_InRadiusInner_COPY_INP_TMP * (((hv_InStartPhi_COPY_INP_TMP - 180)).TupleCos()
                        ));
                }
            }
            else if ((int)(new HTuple(hv_InEndPhi.TupleEqual(270))) != 0)
            {
                //Y轴负向
                hv_EndRow_O.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_EndRow_O = hv_InCenterRow_COPY_INP_TMP + hv_InRadiusOuter_COPY_INP_TMP;
                }
                hv_EndCol_O.Dispose();
                hv_EndCol_O = new HTuple(hv_InCenterCol_COPY_INP_TMP);
                hv_EndRow_I.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_EndRow_I = hv_InCenterRow_COPY_INP_TMP + hv_InRadiusInner_COPY_INP_TMP;
                }
                hv_EndCol_I.Dispose();
                hv_EndCol_I = new HTuple(hv_InCenterCol_COPY_INP_TMP);
            }
            else if ((int)((new HTuple(hv_InEndPhi.TupleGreater(270))).TupleAnd(new HTuple(hv_InEndPhi.TupleLess(
                360)))) != 0)
            {
                //第四象限
                hv_EndRow_O.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_EndRow_O = hv_InCenterRow_COPY_INP_TMP + (hv_InRadiusOuter_COPY_INP_TMP * (((360 - hv_InStartPhi_COPY_INP_TMP)).TupleSin()
                        ));
                }
                hv_EndCol_O.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_EndCol_O = hv_InCenterCol_COPY_INP_TMP + (hv_InRadiusOuter_COPY_INP_TMP * (((360 - hv_InStartPhi_COPY_INP_TMP)).TupleCos()
                        ));
                }
                hv_EndRow_I.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_EndRow_I = hv_InCenterRow_COPY_INP_TMP + (hv_InRadiusInner_COPY_INP_TMP * (((360 - hv_InStartPhi_COPY_INP_TMP)).TupleSin()
                        ));
                }
                hv_EndCol_I.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_EndCol_I = hv_InCenterCol_COPY_INP_TMP + (hv_InRadiusInner_COPY_INP_TMP * (((360 - hv_InStartPhi_COPY_INP_TMP)).TupleCos()
                        ));
                }
            }

            HOperatorSet.SetSystem("flush_graphic", "true");
            if ((int)(new HTuple(hv_InStartPhi_COPY_INP_TMP.TupleGreaterEqual(hv_InEndPhi))) != 0)
            {
                hv_order.Dispose();
                hv_order = "negative";
            }
            else if ((int)(new HTuple(hv_InStartPhi_COPY_INP_TMP.TupleLess(hv_InEndPhi))) != 0)
            {
                hv_order.Dispose();
                hv_order = "positive";
            }
            ho_xld_o.Dispose(); ho_xld_i.Dispose(); ho_rect_1.Dispose(); ho_rect_2.Dispose(); hv_AngleStartRow.Dispose(); hv_AngleStartCol.Dispose(); hv_AngleEndRow.Dispose(); hv_AngleEndCol.Dispose();
            gen_annular_2(out ho_xld_o, out ho_xld_i, out ho_rect_1, out ho_rect_2, hv_WindowHandle, hv_Information, ho_ImagePre, ho_WindowImagePre, color,
                hv_InCenterRow_COPY_INP_TMP, hv_InCenterCol_COPY_INP_TMP, hv_InRadiusInner_COPY_INP_TMP,
                hv_InRadiusOuter_COPY_INP_TMP, hv_order, hv_StartRow_O, hv_StartCol_O, hv_EndRow_O,
                hv_EndCol_O, out hv_AngleStartRow, out hv_AngleStartCol, out hv_AngleEndRow,
                out hv_AngleEndCol);

            hv_flag2.Dispose();
            hv_flag2 = 1;
            while ((int)(hv_flag2) != 0)
            {

                hv_Row.Dispose(); hv_Col.Dispose(); hv_Button.Dispose();
                HOperatorSet.GetMbuttonSubPix(hv_WindowHandle, out hv_Row, out hv_Col, out hv_Button);
                hv_min_.Dispose();
                hv_min_ = new HTuple();
                hv_max_.Dispose();
                hv_max_ = new HTuple();
                switch (hv_Button.I)
                {
                    case 1:
                        hv_DistanceMin.Dispose(); hv_DistanceMax.Dispose();
                        HOperatorSet.DistancePc(ho_xld_o, hv_Row, hv_Col, out hv_DistanceMin, out hv_DistanceMax);
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_min_ = hv_min_.TupleConcat(
                                    hv_DistanceMin);
                                hv_min_.Dispose();
                                hv_min_ = ExpTmpLocalVar_min_;
                            }
                        }
                        hv_DistanceMin.Dispose(); hv_DistanceMax.Dispose();
                        HOperatorSet.DistancePc(ho_xld_i, hv_Row, hv_Col, out hv_DistanceMin, out hv_DistanceMax);
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_min_ = hv_min_.TupleConcat(
                                    hv_DistanceMin);
                                hv_min_.Dispose();
                                hv_min_ = ExpTmpLocalVar_min_;
                            }
                        }
                        hv_DistanceMin.Dispose();
                        HOperatorSet.DistancePp(hv_Row, hv_Col, hv_AngleStartRow, hv_AngleStartCol,
                            out hv_DistanceMin);
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_min_ = hv_min_.TupleConcat(
                                    hv_DistanceMin);
                                hv_min_.Dispose();
                                hv_min_ = ExpTmpLocalVar_min_;
                            }
                        }
                        hv_DistanceMin.Dispose();
                        HOperatorSet.DistancePp(hv_Row, hv_Col, hv_AngleEndRow, hv_AngleEndCol, out hv_DistanceMin);
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_min_ = hv_min_.TupleConcat(
                                    hv_DistanceMin);
                                hv_min_.Dispose();
                                hv_min_ = ExpTmpLocalVar_min_;
                            }
                        }
                        hv_DistanceMin.Dispose();
                        HOperatorSet.DistancePp(hv_Row, hv_Col, hv_InCenterRow_COPY_INP_TMP, hv_InCenterCol_COPY_INP_TMP,
                            out hv_DistanceMin);
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_min_ = hv_min_.TupleConcat(
                                    hv_DistanceMin);
                                hv_min_.Dispose();
                                hv_min_ = ExpTmpLocalVar_min_;
                            }
                        }
                        hv_indice.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_indice = hv_min_.TupleFind(
                                hv_min_.TupleMin());
                        }
                        hv_flag.Dispose();
                        hv_flag = 1;
                        if ((int)(new HTuple((new HTuple(hv_indice.TupleLength())).TupleNotEqual(
                            1))) != 0)
                        {
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                {
                                    HTuple
                                      ExpTmpLocalVar_indice = hv_indice.TupleSelect(
                                        0);
                                    hv_indice.Dispose();
                                    hv_indice = ExpTmpLocalVar_indice;
                                }
                            }
                        }
                        switch (hv_indice.I)
                        {
                            case 0:
                                while ((int)(hv_flag) != 0)
                                {
                                    //外径
                                    hv_Row.Dispose(); hv_Col.Dispose(); hv_Button.Dispose();
                                    HOperatorSet.GetMbuttonSubPix(hv_WindowHandle, out hv_Row, out hv_Col,
                                        out hv_Button);
                                    if ((int)(new HTuple(hv_Button.TupleNotEqual(4))) != 0)
                                    {
                                        hv_Distance.Dispose();
                                        HOperatorSet.DistancePp(hv_InCenterRow_COPY_INP_TMP, hv_InCenterCol_COPY_INP_TMP,
                                            hv_Row, hv_Col, out hv_Distance);
                                        if ((int)(new HTuple(hv_Distance.TupleLess(hv_InRadiusInner_COPY_INP_TMP))) != 0)
                                        {
                                            break;
                                        }
                                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                        {
                                            {
                                                HTuple
                                                  ExpTmpLocalVar_StartCol_O = ((hv_Distance / hv_InRadiusOuter_COPY_INP_TMP) * (hv_StartCol_O - hv_InCenterCol_COPY_INP_TMP)) + hv_InCenterCol_COPY_INP_TMP;
                                                hv_StartCol_O.Dispose();
                                                hv_StartCol_O = ExpTmpLocalVar_StartCol_O;
                                            }
                                        }
                                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                        {
                                            {
                                                HTuple
                                                  ExpTmpLocalVar_StartRow_O = ((hv_Distance / hv_InRadiusOuter_COPY_INP_TMP) * (hv_StartRow_O - hv_InCenterRow_COPY_INP_TMP)) + hv_InCenterRow_COPY_INP_TMP;
                                                hv_StartRow_O.Dispose();
                                                hv_StartRow_O = ExpTmpLocalVar_StartRow_O;
                                            }
                                        }
                                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                        {
                                            {
                                                HTuple
                                                  ExpTmpLocalVar_EndCol_O = ((hv_Distance / hv_InRadiusOuter_COPY_INP_TMP) * (hv_EndCol_O - hv_InCenterCol_COPY_INP_TMP)) + hv_InCenterCol_COPY_INP_TMP;
                                                hv_EndCol_O.Dispose();
                                                hv_EndCol_O = ExpTmpLocalVar_EndCol_O;
                                            }
                                        }
                                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                        {
                                            {
                                                HTuple
                                                  ExpTmpLocalVar_EndRow_O = ((hv_Distance / hv_InRadiusOuter_COPY_INP_TMP) * (hv_EndRow_O - hv_InCenterRow_COPY_INP_TMP)) + hv_InCenterRow_COPY_INP_TMP;
                                                hv_EndRow_O.Dispose();
                                                hv_EndRow_O = ExpTmpLocalVar_EndRow_O;
                                            }
                                        }
                                        hv_InRadiusOuter_COPY_INP_TMP.Dispose();
                                        hv_InRadiusOuter_COPY_INP_TMP = new HTuple(hv_Distance);
                                        ho_xld_o.Dispose(); ho_xld_i.Dispose(); ho_rect_1.Dispose(); ho_rect_2.Dispose(); hv_AngleStartRow.Dispose(); hv_AngleStartCol.Dispose(); hv_AngleEndRow.Dispose(); hv_AngleEndCol.Dispose();
                                        gen_annular_2(out ho_xld_o, out ho_xld_i, out ho_rect_1, out ho_rect_2,
                                            hv_WindowHandle, hv_Information, ho_ImagePre, ho_WindowImagePre, color, hv_InCenterRow_COPY_INP_TMP, hv_InCenterCol_COPY_INP_TMP,
                                            hv_InRadiusInner_COPY_INP_TMP, hv_InRadiusOuter_COPY_INP_TMP, hv_order,
                                            hv_StartRow_O, hv_StartCol_O, hv_EndRow_O, hv_EndCol_O, out hv_AngleStartRow,
                                            out hv_AngleStartCol, out hv_AngleEndRow, out hv_AngleEndCol);
                                    }
                                    else
                                    {
                                        hv_flag.Dispose();
                                        hv_flag = 0;
                                    }
                                }
                                break;
                            case 1:
                                while ((int)(hv_flag) != 0)
                                {
                                    //内径
                                    hv_Row.Dispose(); hv_Col.Dispose(); hv_Button.Dispose();
                                    HOperatorSet.GetMbuttonSubPix(hv_WindowHandle, out hv_Row, out hv_Col,
                                        out hv_Button);
                                    if ((int)(new HTuple(hv_Button.TupleNotEqual(4))) != 0)
                                    {
                                        hv_Distance.Dispose();
                                        HOperatorSet.DistancePp(hv_InCenterRow_COPY_INP_TMP, hv_InCenterCol_COPY_INP_TMP,
                                            hv_Row, hv_Col, out hv_Distance);
                                        if ((int)((new HTuple(hv_Distance.TupleGreaterEqual(hv_InRadiusOuter_COPY_INP_TMP))).TupleOr(
                                            new HTuple(hv_Distance.TupleLessEqual(2)))) != 0)
                                        {
                                            break;
                                        }
                                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                        {
                                            {
                                                HTuple
                                                  ExpTmpLocalVar_StartCol_I = ((hv_Distance / hv_InRadiusInner_COPY_INP_TMP) * (hv_StartCol_I - hv_InCenterCol_COPY_INP_TMP)) + hv_InCenterCol_COPY_INP_TMP;
                                                hv_StartCol_I.Dispose();
                                                hv_StartCol_I = ExpTmpLocalVar_StartCol_I;
                                            }
                                        }
                                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                        {
                                            {
                                                HTuple
                                                  ExpTmpLocalVar_StartRow_I = ((hv_Distance / hv_InRadiusInner_COPY_INP_TMP) * (hv_StartRow_I - hv_InCenterRow_COPY_INP_TMP)) + hv_InCenterRow_COPY_INP_TMP;
                                                hv_StartRow_I.Dispose();
                                                hv_StartRow_I = ExpTmpLocalVar_StartRow_I;
                                            }
                                        }
                                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                        {
                                            {
                                                HTuple
                                                  ExpTmpLocalVar_EndCol_I = ((hv_Distance / hv_InRadiusInner_COPY_INP_TMP) * (hv_EndCol_I - hv_InCenterCol_COPY_INP_TMP)) + hv_InCenterCol_COPY_INP_TMP;
                                                hv_EndCol_I.Dispose();
                                                hv_EndCol_I = ExpTmpLocalVar_EndCol_I;
                                            }
                                        }
                                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                        {
                                            {
                                                HTuple
                                                  ExpTmpLocalVar_EndRow_I = ((hv_Distance / hv_InRadiusInner_COPY_INP_TMP) * (hv_EndRow_I - hv_InCenterRow_COPY_INP_TMP)) + hv_InCenterRow_COPY_INP_TMP;
                                                hv_EndRow_I.Dispose();
                                                hv_EndRow_I = ExpTmpLocalVar_EndRow_I;
                                            }
                                        }
                                        hv_InRadiusInner_COPY_INP_TMP.Dispose();
                                        hv_InRadiusInner_COPY_INP_TMP = new HTuple(hv_Distance);
                                        ho_xld_o.Dispose(); ho_xld_i.Dispose(); ho_rect_1.Dispose(); ho_rect_2.Dispose(); hv_AngleStartRow.Dispose(); hv_AngleStartCol.Dispose(); hv_AngleEndRow.Dispose(); hv_AngleEndCol.Dispose();
                                        gen_annular_2(out ho_xld_o, out ho_xld_i, out ho_rect_1, out ho_rect_2,
                                            hv_WindowHandle, hv_Information, ho_ImagePre, ho_WindowImagePre, color, hv_InCenterRow_COPY_INP_TMP, hv_InCenterCol_COPY_INP_TMP,
                                            hv_InRadiusInner_COPY_INP_TMP, hv_InRadiusOuter_COPY_INP_TMP, hv_order,
                                            hv_StartRow_O, hv_StartCol_O, hv_EndRow_O, hv_EndCol_O, out hv_AngleStartRow,
                                            out hv_AngleStartCol, out hv_AngleEndRow, out hv_AngleEndCol);
                                    }
                                    else
                                    {
                                        hv_flag.Dispose();
                                        hv_flag = 0;
                                    }
                                }
                                break;
                            case 2:
                                //起点
                                while ((int)(hv_flag) != 0)
                                {
                                    hv_Row.Dispose(); hv_Col.Dispose(); hv_Button.Dispose();
                                    HOperatorSet.GetMbuttonSubPix(hv_WindowHandle, out hv_Row, out hv_Col,
                                        out hv_Button);
                                    hv_Distance.Dispose();
                                    HOperatorSet.DistancePp(hv_InCenterRow_COPY_INP_TMP, hv_InCenterCol_COPY_INP_TMP,
                                        hv_Row, hv_Col, out hv_Distance);
                                    if ((int)(new HTuple(hv_Button.TupleNotEqual(4))) != 0)
                                    {
                                        hv_StartCol_O.Dispose();
                                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                        {
                                            hv_StartCol_O = ((hv_InRadiusOuter_COPY_INP_TMP / hv_Distance) * (hv_Col - hv_InCenterCol_COPY_INP_TMP)) + hv_InCenterCol_COPY_INP_TMP;
                                        }
                                        hv_StartRow_O.Dispose();
                                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                        {
                                            hv_StartRow_O = ((hv_InRadiusOuter_COPY_INP_TMP / hv_Distance) * (hv_Row - hv_InCenterRow_COPY_INP_TMP)) + hv_InCenterRow_COPY_INP_TMP;
                                        }
                                        hv_StartCol_I.Dispose();
                                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                        {
                                            hv_StartCol_I = ((hv_InRadiusInner_COPY_INP_TMP / hv_Distance) * (hv_Col - hv_InCenterCol_COPY_INP_TMP)) + hv_InCenterCol_COPY_INP_TMP;
                                        }
                                        hv_StartRow_I.Dispose();
                                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                        {
                                            hv_StartRow_I = ((hv_InRadiusInner_COPY_INP_TMP / hv_Distance) * (hv_Row - hv_InCenterRow_COPY_INP_TMP)) + hv_InCenterRow_COPY_INP_TMP;
                                        }
                                        if ((int)((new HTuple(((hv_StartRow_O - hv_InCenterRow_COPY_INP_TMP)).TupleLessEqual(
                                            0))).TupleAnd(new HTuple(((hv_StartCol_O - hv_InCenterCol_COPY_INP_TMP)).TupleGreaterEqual(
                                            0)))) != 0)
                                        {
                                            //第一象限
                                            hv_Phi.Dispose();
                                            HOperatorSet.LineOrientation(hv_InCenterRow_COPY_INP_TMP, hv_InCenterCol_COPY_INP_TMP,
                                                hv_StartRow_O, hv_StartCol_O, out hv_Phi);
                                            hv_InStartPhi_COPY_INP_TMP.Dispose();
                                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                            {
                                                hv_InStartPhi_COPY_INP_TMP = hv_Phi.TupleDeg()
                                                    ;
                                            }
                                        }
                                        else if ((int)((new HTuple(((hv_StartRow_O - hv_InCenterRow_COPY_INP_TMP)).TupleLess(
                                            0))).TupleAnd(new HTuple(((hv_StartCol_O - hv_InCenterCol_COPY_INP_TMP)).TupleLessEqual(
                                            0)))) != 0)
                                        {
                                            //第二象限
                                            hv_Phi.Dispose();
                                            HOperatorSet.LineOrientation(hv_InCenterRow_COPY_INP_TMP, hv_InCenterCol_COPY_INP_TMP,
                                                hv_StartRow_O, hv_StartCol_O, out hv_Phi);
                                            hv_InStartPhi_COPY_INP_TMP.Dispose();
                                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                            {
                                                hv_InStartPhi_COPY_INP_TMP = 180 + (hv_Phi.TupleDeg()
                                                    );
                                            }
                                        }
                                        else if ((int)((new HTuple(((hv_StartRow_O - hv_InCenterRow_COPY_INP_TMP)).TupleGreaterEqual(
                                            0))).TupleAnd(new HTuple(((hv_StartCol_O - hv_InCenterCol_COPY_INP_TMP)).TupleLess(
                                            0)))) != 0)
                                        {
                                            //第三象限
                                            hv_Phi.Dispose();
                                            HOperatorSet.LineOrientation(hv_InCenterRow_COPY_INP_TMP, hv_InCenterCol_COPY_INP_TMP,
                                                hv_StartRow_O, hv_StartCol_O, out hv_Phi);
                                            hv_InStartPhi_COPY_INP_TMP.Dispose();
                                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                            {
                                                hv_InStartPhi_COPY_INP_TMP = 180 + (hv_Phi.TupleDeg()
                                                    );
                                            }
                                        }
                                        else if ((int)((new HTuple(((hv_StartRow_O - hv_InCenterRow_COPY_INP_TMP)).TupleGreaterEqual(
                                            0))).TupleAnd(new HTuple(((hv_StartCol_O - hv_InCenterCol_COPY_INP_TMP)).TupleGreater(
                                            0)))) != 0)
                                        {
                                            //第四象限
                                            hv_Phi.Dispose();
                                            HOperatorSet.LineOrientation(hv_InCenterRow_COPY_INP_TMP, hv_InCenterCol_COPY_INP_TMP,
                                                hv_StartRow_O, hv_StartCol_O, out hv_Phi);
                                            hv_InStartPhi_COPY_INP_TMP.Dispose();
                                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                            {
                                                hv_InStartPhi_COPY_INP_TMP = 360 + (hv_Phi.TupleDeg()
                                                    );
                                            }
                                        }
                                        ho_xld_o.Dispose(); ho_xld_i.Dispose(); ho_rect_1.Dispose(); ho_rect_2.Dispose(); hv_AngleStartRow.Dispose(); hv_AngleStartCol.Dispose(); hv_AngleEndRow.Dispose(); hv_AngleEndCol.Dispose();
                                        gen_annular_2(out ho_xld_o, out ho_xld_i, out ho_rect_1, out ho_rect_2,
                                            hv_WindowHandle, hv_Information, ho_ImagePre, ho_WindowImagePre, color, hv_InCenterRow_COPY_INP_TMP, hv_InCenterCol_COPY_INP_TMP,
                                            hv_InRadiusInner_COPY_INP_TMP, hv_InRadiusOuter_COPY_INP_TMP, hv_order,
                                            hv_StartRow_O, hv_StartCol_O, hv_EndRow_O, hv_EndCol_O, out hv_AngleStartRow,
                                            out hv_AngleStartCol, out hv_AngleEndRow, out hv_AngleEndCol);
                                    }
                                    else
                                    {
                                        hv_flag.Dispose();
                                        hv_flag = 0;
                                    }
                                }
                                break;
                            case 3:
                                //终点
                                while ((int)(hv_flag) != 0)
                                {
                                    hv_Row.Dispose(); hv_Col.Dispose(); hv_Button.Dispose();
                                    HOperatorSet.GetMbuttonSubPix(hv_WindowHandle, out hv_Row, out hv_Col,
                                        out hv_Button);
                                    hv_Distance.Dispose();
                                    HOperatorSet.DistancePp(hv_InCenterRow_COPY_INP_TMP, hv_InCenterCol_COPY_INP_TMP,
                                        hv_Row, hv_Col, out hv_Distance);
                                    if ((int)(new HTuple(hv_Button.TupleNotEqual(4))) != 0)
                                    {
                                        hv_EndCol_O.Dispose();
                                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                        {
                                            hv_EndCol_O = ((hv_InRadiusOuter_COPY_INP_TMP / hv_Distance) * (hv_Col - hv_InCenterCol_COPY_INP_TMP)) + hv_InCenterCol_COPY_INP_TMP;
                                        }
                                        hv_EndRow_O.Dispose();
                                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                        {
                                            hv_EndRow_O = ((hv_InRadiusOuter_COPY_INP_TMP / hv_Distance) * (hv_Row - hv_InCenterRow_COPY_INP_TMP)) + hv_InCenterRow_COPY_INP_TMP;
                                        }
                                        hv_EndCol_I.Dispose();
                                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                        {
                                            hv_EndCol_I = ((hv_InRadiusInner_COPY_INP_TMP / hv_Distance) * (hv_Col - hv_InCenterCol_COPY_INP_TMP)) + hv_InCenterCol_COPY_INP_TMP;
                                        }
                                        hv_EndRow_I.Dispose();
                                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                        {
                                            hv_EndRow_I = ((hv_InRadiusInner_COPY_INP_TMP / hv_Distance) * (hv_Row - hv_InCenterRow_COPY_INP_TMP)) + hv_InCenterRow_COPY_INP_TMP;
                                        }
                                        if ((int)((new HTuple(((hv_EndRow_O - hv_InCenterRow_COPY_INP_TMP)).TupleLess(
                                            0))).TupleAnd(new HTuple(((hv_EndCol_O - hv_InCenterCol_COPY_INP_TMP)).TupleGreaterEqual(
                                            0)))) != 0)
                                        {
                                            //第一象限
                                            hv_Phi.Dispose();
                                            HOperatorSet.LineOrientation(hv_InCenterRow_COPY_INP_TMP, hv_InCenterCol_COPY_INP_TMP,
                                                hv_EndRow_O, hv_EndCol_O, out hv_Phi);
                                            hv_EndAngle.Dispose();
                                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                            {
                                                hv_EndAngle = hv_Phi.TupleDeg()
                                                    ;
                                            }
                                        }
                                        else if ((int)((new HTuple(((hv_EndRow_O - hv_InCenterRow_COPY_INP_TMP)).TupleLess(
                                            0))).TupleAnd(new HTuple(((hv_EndCol_O - hv_InCenterCol_COPY_INP_TMP)).TupleLessEqual(
                                            0)))) != 0)
                                        {
                                            //第二象限
                                            hv_Phi.Dispose();
                                            HOperatorSet.LineOrientation(hv_InCenterRow_COPY_INP_TMP, hv_InCenterCol_COPY_INP_TMP,
                                                hv_EndRow_O, hv_EndCol_O, out hv_Phi);
                                            hv_EndAngle.Dispose();
                                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                            {
                                                hv_EndAngle = 180 + (hv_Phi.TupleDeg()
                                                    );
                                            }
                                        }
                                        else if ((int)((new HTuple(((hv_EndRow_O - hv_InCenterRow_COPY_INP_TMP)).TupleGreaterEqual(
                                            0))).TupleAnd(new HTuple(((hv_EndCol_O - hv_InCenterCol_COPY_INP_TMP)).TupleLess(
                                            0)))) != 0)
                                        {
                                            //第三象限
                                            hv_Phi.Dispose();
                                            HOperatorSet.LineOrientation(hv_InCenterRow_COPY_INP_TMP, hv_InCenterCol_COPY_INP_TMP,
                                                hv_EndRow_O, hv_EndCol_O, out hv_Phi);
                                            hv_EndAngle.Dispose();
                                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                            {
                                                hv_EndAngle = 180 + (hv_Phi.TupleDeg()
                                                    );
                                            }
                                        }
                                        else if ((int)((new HTuple(((hv_EndRow_O - hv_InCenterRow_COPY_INP_TMP)).TupleGreaterEqual(
                                            0))).TupleAnd(new HTuple(((hv_EndCol_O - hv_InCenterCol_COPY_INP_TMP)).TupleGreater(
                                            0)))) != 0)
                                        {
                                            //第四象限
                                            hv_Phi.Dispose();
                                            HOperatorSet.LineOrientation(hv_InCenterRow_COPY_INP_TMP, hv_InCenterCol_COPY_INP_TMP,
                                                hv_EndRow_O, hv_EndCol_O, out hv_Phi);
                                            hv_EndAngle.Dispose();
                                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                            {
                                                hv_EndAngle = 360 + (hv_Phi.TupleDeg()
                                                    );
                                            }
                                        }
                                        ho_xld_o.Dispose(); ho_xld_i.Dispose(); ho_rect_1.Dispose(); ho_rect_2.Dispose(); hv_AngleStartRow.Dispose(); hv_AngleStartCol.Dispose(); hv_AngleEndRow.Dispose(); hv_AngleEndCol.Dispose();
                                        gen_annular_2(out ho_xld_o, out ho_xld_i, out ho_rect_1, out ho_rect_2,
                                            hv_WindowHandle, hv_Information, ho_ImagePre, ho_WindowImagePre, color, hv_InCenterRow_COPY_INP_TMP, hv_InCenterCol_COPY_INP_TMP,
                                            hv_InRadiusInner_COPY_INP_TMP, hv_InRadiusOuter_COPY_INP_TMP, hv_order,
                                            hv_StartRow_O, hv_StartCol_O, hv_EndRow_O, hv_EndCol_O, out hv_AngleStartRow,
                                            out hv_AngleStartCol, out hv_AngleEndRow, out hv_AngleEndCol);
                                    }
                                    else
                                    {
                                        hv_flag.Dispose();
                                        hv_flag = 0;
                                    }
                                }
                                break;
                            case 4:
                                //while ((int)(new HTuple(hv_Button.TupleNotEqual(4))) != 0)//修改:不会把点击鼠标右键认为新的圆心
                                while ((int)(hv_flag) != 0)
                                {
                                    //中心点
                                    if ((int)(new HTuple(hv_Button.TupleNotEqual(4))) != 0)
                                    {
                                        hv_Row.Dispose(); hv_Col.Dispose(); hv_Button.Dispose();
                                        HOperatorSet.GetMbuttonSubPix(hv_WindowHandle, out hv_Row, out hv_Col,
                                            out hv_Button);
                                        if ((int)(new HTuple(hv_Button.TupleNotEqual(4))) != 0)//修改:不会把点击鼠标右键认为新的圆心
                                        {
                                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                            {
                                                {
                                                    HTuple
                                                      ExpTmpLocalVar_StartCol_O = (hv_StartCol_O + hv_Col) - hv_InCenterCol_COPY_INP_TMP;
                                                    hv_StartCol_O.Dispose();
                                                    hv_StartCol_O = ExpTmpLocalVar_StartCol_O;
                                                }
                                            }
                                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                            {
                                                {
                                                    HTuple
                                                      ExpTmpLocalVar_StartRow_O = (hv_StartRow_O + hv_Row) - hv_InCenterRow_COPY_INP_TMP;
                                                    hv_StartRow_O.Dispose();
                                                    hv_StartRow_O = ExpTmpLocalVar_StartRow_O;
                                                }
                                            }
                                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                            {
                                                {
                                                    HTuple
                                                      ExpTmpLocalVar_EndCol_O = (hv_EndCol_O + hv_Col) - hv_InCenterCol_COPY_INP_TMP;
                                                    hv_EndCol_O.Dispose();
                                                    hv_EndCol_O = ExpTmpLocalVar_EndCol_O;
                                                }
                                            }
                                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                            {
                                                {
                                                    HTuple
                                                      ExpTmpLocalVar_EndRow_O = (hv_EndRow_O + hv_Row) - hv_InCenterRow_COPY_INP_TMP;
                                                    hv_EndRow_O.Dispose();
                                                    hv_EndRow_O = ExpTmpLocalVar_EndRow_O;
                                                }
                                            }
                                            hv_InCenterRow_COPY_INP_TMP.Dispose();
                                            hv_InCenterRow_COPY_INP_TMP = new HTuple(hv_Row);
                                            hv_InCenterCol_COPY_INP_TMP.Dispose();
                                            hv_InCenterCol_COPY_INP_TMP = new HTuple(hv_Col);
                                            ho_xld_o.Dispose(); ho_xld_i.Dispose(); ho_rect_1.Dispose(); ho_rect_2.Dispose(); hv_AngleStartRow.Dispose(); hv_AngleStartCol.Dispose(); hv_AngleEndRow.Dispose(); hv_AngleEndCol.Dispose();
                                            gen_annular_2(out ho_xld_o, out ho_xld_i, out ho_rect_1, out ho_rect_2,
                                                hv_WindowHandle, hv_Information, ho_ImagePre, ho_WindowImagePre, color, hv_InCenterRow_COPY_INP_TMP, hv_InCenterCol_COPY_INP_TMP,
                                                hv_InRadiusInner_COPY_INP_TMP, hv_InRadiusOuter_COPY_INP_TMP, hv_order,
                                                hv_StartRow_O, hv_StartCol_O, hv_EndRow_O, hv_EndCol_O, out hv_AngleStartRow,
                                                out hv_AngleStartCol, out hv_AngleEndRow, out hv_AngleEndCol);
                                        }
                                    }
                                    else
                                    {
                                        hv_flag.Dispose();
                                        hv_flag = 0;
                                    }
                                }
                                break;
                        }
                        break;
                    case 4:
                        hv_flag2.Dispose();
                        hv_flag2 = 0;
                        break;
                    default:
                        break;
                }

            }

            hv_CenterRow.Dispose();
            hv_CenterRow = new HTuple(hv_InCenterRow_COPY_INP_TMP);
            hv_CenterCol.Dispose();
            hv_CenterCol = new HTuple(hv_InCenterCol_COPY_INP_TMP);
            hv_RadiusInner.Dispose();
            hv_RadiusInner = new HTuple(hv_InRadiusInner_COPY_INP_TMP);
            hv_RadiusOuter.Dispose();
            hv_RadiusOuter = new HTuple(hv_InRadiusOuter_COPY_INP_TMP);
            hv_StartPhi.Dispose();
            hv_StartPhi = new HTuple(hv_InStartPhi_COPY_INP_TMP);
            hv_EndPhi.Dispose();

            if (hv_EndAngle.Length == 0)//修改
                hv_EndPhi = new HTuple(hv_InEndPhi);
            else
                hv_EndPhi = new HTuple(hv_EndAngle);

            ho_xld_o.Dispose();
            ho_xld_i.Dispose();
            ho_rect_1.Dispose();
            ho_rect_2.Dispose();

            hv_InCenterCol_COPY_INP_TMP.Dispose();
            hv_InCenterRow_COPY_INP_TMP.Dispose();
            hv_InRadiusInner_COPY_INP_TMP.Dispose();
            hv_InRadiusOuter_COPY_INP_TMP.Dispose();
            hv_InStartPhi_COPY_INP_TMP.Dispose();
            hv_StartRow_O.Dispose();
            hv_StartCol_O.Dispose();
            hv_StartRow_I.Dispose();
            hv_StartCol_I.Dispose();
            hv_EndRow_O.Dispose();
            hv_EndCol_O.Dispose();
            hv_EndRow_I.Dispose();
            hv_EndCol_I.Dispose();
            hv_order.Dispose();
            hv_AngleStartRow.Dispose();
            hv_AngleStartCol.Dispose();
            hv_AngleEndRow.Dispose();
            hv_AngleEndCol.Dispose();
            hv_flag2.Dispose();
            hv_Row.Dispose();
            hv_Col.Dispose();
            hv_Button.Dispose();
            hv_min_.Dispose();
            hv_max_.Dispose();
            hv_DistanceMin.Dispose();
            hv_DistanceMax.Dispose();
            hv_indice.Dispose();
            hv_flag.Dispose();
            hv_Distance.Dispose();
            hv_Phi.Dispose();
            hv_EndAngle.Dispose();

            return;
        }
        #endregion

        //在 draw_annular 里调用
        #region gen_annular_2
        public void gen_annular_2(out HObject ho_Contour_o, out HObject ho_Contour_i, out HObject ho_AnchorStart, out HObject ho_AnchorEnd, HTuple hv_WindowHandle, HTuple hv_Information, HObject ho_ImagePre, HObject ho_WindowImagePre, string color, HTuple hv_Row, HTuple hv_Column, HTuple hv_RadiusInner, HTuple hv_RadiusOuter, HTuple hv_Order, HTuple hv_StartRow, HTuple hv_StartCol, HTuple hv_EndRow, HTuple hv_EndCol, out HTuple hv_AngleStartRow, out HTuple hv_AngleStartCol, out HTuple hv_AngleEndRow, out HTuple hv_AngleEndCol)
        {
            // Local iconic variables 
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Contour_o);
            HOperatorSet.GenEmptyObj(out ho_Contour_i);
            HOperatorSet.GenEmptyObj(out ho_AnchorStart);
            HOperatorSet.GenEmptyObj(out ho_AnchorEnd);

            hv_AngleStartRow = new HTuple();
            hv_AngleStartCol = new HTuple();
            hv_AngleEndRow = new HTuple();
            hv_AngleEndCol = new HTuple();
            if ((int)((new HTuple((new HTuple((new HTuple(hv_Row.TupleEqual(new HTuple()))).TupleOr(
                new HTuple(hv_Column.TupleEqual(new HTuple()))))).TupleOr(new HTuple(hv_RadiusInner.TupleEqual(
                new HTuple()))))).TupleOr(new HTuple(hv_RadiusOuter.TupleEqual(new HTuple())))) != 0)
            {


                return;
            }
            if ((int)((new HTuple(hv_RadiusInner.TupleLess(0))).TupleOr(new HTuple(hv_RadiusOuter.TupleLess(
                0)))) != 0)
            {


                return;
            }
            if ((int)((new HTuple((new HTuple((new HTuple(hv_StartRow.TupleEqual(new HTuple()))).TupleOr(
                new HTuple(hv_StartCol.TupleEqual(new HTuple()))))).TupleOr(new HTuple(hv_EndRow.TupleEqual(
                new HTuple()))))).TupleOr(new HTuple(hv_EndCol.TupleEqual(new HTuple())))) != 0)
            {


                return;
            }
            if ((int)(new HTuple(hv_Order.TupleEqual(""))) != 0)
            {


                return;
            }
            ho_Contour_o.Dispose();
            HOperatorSet.GenEmptyObj(out ho_Contour_o);
            ho_Contour_i.Dispose();
            HOperatorSet.GenEmptyObj(out ho_Contour_i);

            HOperatorSet.SetSystem("flush_graphic", "false");

            ho_Contour_o.Dispose();
            gen_circular(out ho_Contour_o, hv_Row, hv_Column, hv_RadiusOuter, hv_Order, hv_StartRow,
                hv_StartCol, hv_EndRow, hv_EndCol);
            ho_Contour_i.Dispose();
            gen_circular(out ho_Contour_i, hv_Row, hv_Column, hv_RadiusInner, hv_Order, hv_StartRow,
                hv_StartCol, hv_EndRow, hv_EndCol);

            //起始点锚点
            hv_AngleStartRow.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_AngleStartRow = (((hv_RadiusOuter + 20) / hv_RadiusOuter) * (hv_StartRow - hv_Row)) + hv_Row;
            }
            hv_AngleStartCol.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_AngleStartCol = (((hv_RadiusOuter + 20) / hv_RadiusOuter) * (hv_StartCol - hv_Column)) + hv_Column;
            }
            hv_AngleEndRow.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_AngleEndRow = (((hv_RadiusOuter + 20) / hv_RadiusOuter) * (hv_EndRow - hv_Row)) + hv_Row;
            }
            hv_AngleEndCol.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_AngleEndCol = (((hv_RadiusOuter + 20) / hv_RadiusOuter) * (hv_EndCol - hv_Column)) + hv_Column;
            }
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                ho_AnchorStart.Dispose();
                HOperatorSet.GenRectangle1(out ho_AnchorStart, hv_AngleStartRow - 2, hv_AngleStartCol - 2,
                    hv_AngleStartRow + 2, hv_AngleStartCol + 2);
            }
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                ho_AnchorEnd.Dispose();
                HOperatorSet.GenRectangle1(out ho_AnchorEnd, hv_AngleEndRow - 2, hv_AngleEndCol - 2,
                    hv_AngleEndRow + 2, hv_AngleEndCol + 2);
            }


            HOperatorSet.ClearWindow(hv_WindowHandle);//清除显示
            HTuple Width, Height;
            HOperatorSet.GetImageSize(ho_ImagePre, out Width, out Height);
            HOperatorSet.SetPart(hv_WindowHandle, 0, 0, Height - 1, Width - 1);
            HOperatorSet.DispObj(ho_ImagePre, hv_WindowHandle);//画圆环前的窗口图片

            disp_message_Halcon(hv_WindowHandle, hv_Information, "image", 0, 0, color, "false");

            HOperatorSet.SetLineWidth(hv_WindowHandle_CircleInspectionSample, 1);

            HOperatorSet.SetSystem("flush_graphic", "true");

            HOperatorSet.SetDraw(hv_WindowHandle, "margin");
            HOperatorSet.SetColor(hv_WindowHandle, color);
            HOperatorSet.DispObj(ho_Contour_o, hv_WindowHandle);
            HOperatorSet.DispObj(ho_Contour_i, hv_WindowHandle);

            HOperatorSet.SetDraw(hv_WindowHandle, "fill");
            HOperatorSet.SetColor(hv_WindowHandle, color);
            HOperatorSet.DispObj(ho_AnchorStart, hv_WindowHandle);
            HOperatorSet.DispObj(ho_AnchorEnd, hv_WindowHandle);

            return;
        }
        #endregion

        //在 gen_annular 里调用
        #region gen_circular
        public void gen_circular(out HObject ho_Circular, HTuple hv_CenterRow, HTuple hv_CenterCol, HTuple hv_Radius, HTuple hv_Order, HTuple hv_StartRow, HTuple hv_StartCol, HTuple hv_EndRow, HTuple hv_EndCol)
        {
            // Local iconic variables 

            HObject ho_ContCircle = null, ho_RegionLines;
            HObject ho_line;

            // Local control variables 

            HTuple hv_Phi = new HTuple(), hv_startAngle = new HTuple();
            HTuple hv_EndAngle = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Circular);
            HOperatorSet.GenEmptyObj(out ho_ContCircle);
            HOperatorSet.GenEmptyObj(out ho_RegionLines);
            HOperatorSet.GenEmptyObj(out ho_line);
            if ((int)((new HTuple((new HTuple((new HTuple(hv_StartRow.TupleEqual(0))).TupleOr(
                new HTuple(hv_StartCol.TupleEqual(0))))).TupleOr(new HTuple(hv_EndRow.TupleEqual(
                0))))).TupleOr(new HTuple(hv_EndCol.TupleEqual(0)))) != 0)
            {
                ho_ContCircle.Dispose();
                ho_RegionLines.Dispose();
                ho_line.Dispose();

                hv_Phi.Dispose();
                hv_startAngle.Dispose();
                hv_EndAngle.Dispose();

                return;
            }
            if ((int)(new HTuple(hv_Order.TupleEqual(""))) != 0)
            {
                ho_ContCircle.Dispose();
                ho_RegionLines.Dispose();
                ho_line.Dispose();

                hv_Phi.Dispose();
                hv_startAngle.Dispose();
                hv_EndAngle.Dispose();

                return;
            }

            ho_Circular.Dispose();
            HOperatorSet.GenEmptyObj(out ho_Circular);
            HOperatorSet.SetSystem("flush_graphic", "false");

            if ((int)((new HTuple(((hv_StartRow - hv_CenterRow)).TupleLess(0))).TupleAnd(new HTuple(((hv_StartCol - hv_CenterCol)).TupleGreaterEqual(
                0)))) != 0)
            {
                //第一象限
                hv_Phi.Dispose();
                HOperatorSet.LineOrientation(hv_CenterRow, hv_CenterCol, hv_StartRow, hv_StartCol,
                    out hv_Phi);
                hv_startAngle.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_startAngle = hv_Phi.TupleDeg()
                        ;
                }
            }
            else if ((int)((new HTuple(((hv_StartRow - hv_CenterRow)).TupleLess(0))).TupleAnd(
                new HTuple(((hv_StartCol - hv_CenterCol)).TupleLess(0)))) != 0)
            {
                //第二象限
                hv_Phi.Dispose();
                HOperatorSet.LineOrientation(hv_CenterRow, hv_CenterCol, hv_StartRow, hv_StartCol,
                    out hv_Phi);
                hv_startAngle.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_startAngle = 180 + (hv_Phi.TupleDeg()
                        );
                }
            }
            else if ((int)((new HTuple(((hv_StartRow - hv_CenterRow)).TupleGreaterEqual(
                0))).TupleAnd(new HTuple(((hv_StartCol - hv_CenterCol)).TupleLess(0)))) != 0)
            {
                //第三象限
                hv_Phi.Dispose();
                HOperatorSet.LineOrientation(hv_CenterRow, hv_CenterCol, hv_StartRow, hv_StartCol,
                    out hv_Phi);
                hv_startAngle.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_startAngle = 180 + (hv_Phi.TupleDeg()
                        );
                }
            }
            else if ((int)((new HTuple(((hv_StartRow - hv_CenterRow)).TupleGreaterEqual(
                0))).TupleAnd(new HTuple(((hv_StartCol - hv_CenterCol)).TupleGreaterEqual(0)))) != 0)
            {
                //第四象限
                hv_Phi.Dispose();
                HOperatorSet.LineOrientation(hv_CenterRow, hv_CenterCol, hv_StartRow, hv_StartCol,
                    out hv_Phi);
                hv_startAngle.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_startAngle = 360 + (hv_Phi.TupleDeg()
                        );
                }
            }

            if ((int)((new HTuple(((hv_EndRow - hv_CenterRow)).TupleLess(0))).TupleAnd(new HTuple(((hv_EndCol - hv_CenterCol)).TupleGreaterEqual(
                0)))) != 0)
            {
                //第一象限
                hv_Phi.Dispose();
                HOperatorSet.LineOrientation(hv_CenterRow, hv_CenterCol, hv_EndRow, hv_EndCol,
                    out hv_Phi);
                hv_EndAngle.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_EndAngle = hv_Phi.TupleDeg()
                        ;
                }
            }
            else if ((int)((new HTuple(((hv_EndRow - hv_CenterRow)).TupleLess(0))).TupleAnd(
                new HTuple(((hv_EndCol - hv_CenterCol)).TupleLess(0)))) != 0)
            {
                //第二象限
                hv_Phi.Dispose();
                HOperatorSet.LineOrientation(hv_CenterRow, hv_CenterCol, hv_EndRow, hv_EndCol,
                    out hv_Phi);
                hv_EndAngle.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_EndAngle = 180 + (hv_Phi.TupleDeg()
                        );
                }
            }
            else if ((int)((new HTuple(((hv_EndRow - hv_CenterRow)).TupleGreaterEqual(
                0))).TupleAnd(new HTuple(((hv_EndCol - hv_CenterCol)).TupleLess(0)))) != 0)
            {
                //第三象限
                hv_Phi.Dispose();
                HOperatorSet.LineOrientation(hv_CenterRow, hv_CenterCol, hv_EndRow, hv_EndCol,
                    out hv_Phi);
                hv_EndAngle.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_EndAngle = 180 + (hv_Phi.TupleDeg()
                        );
                }
            }
            else if ((int)((new HTuple(((hv_EndRow - hv_CenterRow)).TupleGreaterEqual(
                0))).TupleAnd(new HTuple(((hv_EndCol - hv_CenterCol)).TupleGreaterEqual(0)))) != 0)
            {
                //第四象限
                hv_Phi.Dispose();
                HOperatorSet.LineOrientation(hv_CenterRow, hv_CenterCol, hv_EndRow, hv_EndCol,
                    out hv_Phi);
                hv_EndAngle.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_EndAngle = 360 + (hv_Phi.TupleDeg()
                        );
                }
            }

            //画圆
            double angle = hv_EndAngle.TupleReal() - hv_startAngle.TupleReal();
            if (angle < 0)
                angle = angle * -1;
            if (angle != 360 && angle != 0)
            {
                if ((int)(new HTuple(hv_Order.TupleEqual("positive"))) != 0)
                {
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        ho_ContCircle.Dispose();
                        HOperatorSet.GenCircleContourXld(out ho_ContCircle, hv_CenterRow, hv_CenterCol,
                            hv_Radius, hv_startAngle.TupleRad(), hv_EndAngle.TupleRad(), "negative",
                            1);
                    }
                }
                else if ((int)(new HTuple(hv_Order.TupleEqual("negative"))) != 0)
                {
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        ho_ContCircle.Dispose();
                        HOperatorSet.GenCircleContourXld(out ho_ContCircle, hv_CenterRow, hv_CenterCol,
                            hv_Radius, hv_startAngle.TupleRad(), hv_EndAngle.TupleRad(), "positive",
                            1);
                    }
                }
            }
            if (angle == 360 || angle == 0)
            {
                //HTuple fullAngle = (new HTuple(360)).TupleRad();

                //if ((int)(new HTuple(hv_Order.TupleEqual("positive"))) != 0)
                //    HOperatorSet.GenEllipseContourXld(out ho_ContCircle, hv_CenterRow, hv_CenterCol, 0, hv_Radius, hv_Radius, 0, fullAngle, "negative", 1);
                //else if ((int)(new HTuple(hv_Order.TupleEqual("negative"))) != 0)
                //    HOperatorSet.GenEllipseContourXld(out ho_ContCircle, hv_CenterRow, hv_CenterCol, 0, hv_Radius, hv_Radius, 0, fullAngle, "positive", 1);

                //if ((int)(new HTuple(hv_Order.TupleEqual("positive"))) != 0)
                //    HOperatorSet.GenCircleContourXld(out ho_ContCircle, hv_CenterRow, hv_CenterCol, hv_Radius, 0, fullAngle, "negative", 1);
                //else if ((int)(new HTuple(hv_Order.TupleEqual("negative"))) != 0)
                //    HOperatorSet.GenCircleContourXld(out ho_ContCircle, hv_CenterRow, hv_CenterCol, hv_Radius, 0, fullAngle, "positive", 1);

                HObject ho_ContCircle_Temp;
                HOperatorSet.GenEllipse(out ho_ContCircle_Temp, hv_CenterRow, hv_CenterCol, 0, hv_Radius, hv_Radius);
                HOperatorSet.GenContourRegionXld(ho_ContCircle_Temp, out ho_ContCircle, "border");
            }

            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                ho_RegionLines.Dispose();
                HOperatorSet.GenRegionLine(out ho_RegionLines, hv_CenterRow.TupleConcat(hv_CenterRow),
                    hv_CenterCol.TupleConcat(hv_CenterCol), hv_StartRow.TupleConcat(hv_EndRow),
                    hv_StartCol.TupleConcat(hv_EndCol));
            }
            ho_line.Dispose();
            //HOperatorSet.GenContourRegionXld(ho_RegionLines, out ho_line, "border");//修改
            HOperatorSet.GenContourRegionXld(ho_RegionLines, out ho_line, "center");
            ho_Circular.Dispose();
            HOperatorSet.ConcatObj(ho_ContCircle, ho_line, out ho_Circular);

            HOperatorSet.SetSystem("flush_graphic", "true");

            ho_ContCircle.Dispose();
            ho_RegionLines.Dispose();
            ho_line.Dispose();

            hv_Phi.Dispose();
            hv_startAngle.Dispose();
            hv_EndAngle.Dispose();

            return;
        }

        #endregion

    }
}
