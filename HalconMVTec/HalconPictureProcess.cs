using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HalconDotNet;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;//for [DllImport("Kernel32.dll")]

namespace HalconMVTec
{
    public class HalconPictureProcess
    {
        /// <summary>
        /// BarcodeAcquire:获取二维码
        /// </summary>
        /// <param name="ho_Image_input">HObject:</param>
        /// <param name="hv_result"></param>
        /// <param name="ho_resultRectangle"></param>
        /// <returns></returns>
        public static bool BarcodeAcquire(HObject ho_Image_input,out string barcodeData,out HObject ho_resultRectangle)
        {
            try
            {
                HTuple hv_DataCodeHandle = new HTuple();
                HTuple hv_ResultHandles = new HTuple();
                HTuple hv_DecodedDataStrings = new HTuple();
                
                HOperatorSet.CreateDataCode2dModel("Data Matrix ECC 200", new HTuple(), new HTuple(),
                out hv_DataCodeHandle);

                HOperatorSet.FindDataCode2d(ho_Image_input, out ho_resultRectangle, hv_DataCodeHandle,
                new HTuple(), new HTuple(), out hv_ResultHandles, out hv_DecodedDataStrings);

                if ((int)(new HTuple((new HTuple(hv_DecodedDataStrings.TupleLength())).TupleGreater(
                0))) != 0)
                {
                    barcodeData = hv_DecodedDataStrings.S;
                    hv_DataCodeHandle.Dispose();
                    hv_ResultHandles.Dispose();
                    hv_DecodedDataStrings.Dispose();
                    
                    return true;
                }
                else
                {
                    hv_DataCodeHandle.Dispose();
                    hv_ResultHandles.Dispose();
                    hv_DecodedDataStrings.Dispose();

                    barcodeData = "";
                    return false;
                }


            }
            catch (Exception ex)
            {
                throw ex;
                barcodeData = "";
                return false;
            }
        }

        /// <summary>
        ///  BlobGetCircleCenterPosition：Blob分析获取圆的中心位置
        /// </summary>
        /// <param name="ho_Image">HObject:输入的原始图像</param>
        /// <param name="threSholdMin">int:二值化的最小值</param>
        /// <param name="threSholdMax">int:二值化的最大值</param>
        /// <param name="areaSelectRegionMin">float:区域利用面积筛选最小值</param>
        /// <param name="areaSelectRegionMax">float:区域利用面积筛选最大值</param>
        /// <param name="minCircularity">float:最小圆度值，低于此圆度也判定为识别失败</param>
        /// <param name="feedBackStepNo">int:需要返回的图像的步骤号，0-二值化之后停止，1-区域筛选后停止，2-全部运行完</param>
        /// <param name="rowCoor">float:获取的圆的圆心Row坐标，单位为像素</param>
        /// <param name="columnCoor">float:获取的圆的圆心Column坐标，单位为像素</param>
        /// <param name="feedBackRegion">Hobject:返回的区域，用于显示</param>
        /// <returns>int:-1-失败，0-成功</returns>
        public static int BlobGetCircleCenterPosition(ref HObject ho_Image, int threSholdMin, int threSholdMax, float areaSelectRegionMin, float areaSelectRegionMax, float minCircularity, int feedBackStepNo,
            out float rowCoor, out float columnCoor, out HObject feedBackRegion)
        {
            HObject ho_Regions;
            HObject ho_ConnectedRegions;
            HObject ho_SelectedRegions;
            HTuple hv_Circularity = new HTuple();
            HOperatorSet.GenEmptyObj(out ho_Regions);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions);
            HTuple hv_Area = new HTuple(), hv_Row = new HTuple(), hv_Column = new HTuple();
            HTuple hv_ObjectQuantity = new HTuple();

            rowCoor = 0f;
            columnCoor = 0f;
            feedBackRegion = null;

            //参数检查
            if (threSholdMin < 0)
                threSholdMin = 0;
            else if (threSholdMin > 255)
                threSholdMin = 255;

            if (threSholdMax < 0)
                threSholdMax = 0;
            else if (threSholdMax > 255)
                threSholdMax = 255;

            if (areaSelectRegionMin < 0)
                areaSelectRegionMin = 0;

            if (areaSelectRegionMax < 0)
                areaSelectRegionMin = 0;

            if (feedBackStepNo >= 0)
            {
                ho_Regions.Dispose();
                try
                {
                    HOperatorSet.Threshold(ho_Image, out ho_Regions, threSholdMin, threSholdMax);
                }
                catch
                {
                    return -1;
                }

                hv_ObjectQuantity.Dispose();
                HOperatorSet.CountObj(ho_Regions, out hv_ObjectQuantity);
                HOperatorSet.CopyObj(ho_Regions, out feedBackRegion, 1, hv_ObjectQuantity);
            }

            if (feedBackStepNo >= 1)
            {
                ho_ConnectedRegions.Dispose();
                try
                {
                    HOperatorSet.Connection(ho_Regions, out ho_ConnectedRegions);
                    ho_SelectedRegions.Dispose();
                    HOperatorSet.SelectShape(ho_ConnectedRegions, out ho_SelectedRegions, "area",
                        "and", areaSelectRegionMin, areaSelectRegionMax);
                }
                catch
                {
                    return -1;
                }

                hv_ObjectQuantity.Dispose();
                HOperatorSet.CountObj(ho_SelectedRegions, out hv_ObjectQuantity);
                HOperatorSet.CopyObj(ho_SelectedRegions, out feedBackRegion, 1, hv_ObjectQuantity);
            }

            if (feedBackStepNo >= 2)
            {
                hv_Circularity.Dispose();
                try
                {
                    HOperatorSet.Circularity(ho_SelectedRegions, out hv_Circularity);
                }
                catch
                {
                    return -1;
                }

                int regionCount = 0;
                regionCount = hv_Circularity.TupleLength();

                if (regionCount <= 0)
                {
                    feedBackRegion = null;
                    return -1;
                }
                else
                {
                    float tempCircularity = 0f;
                    int maxCircularityIndex = 0;
                    tempCircularity = hv_Circularity[maxCircularityIndex].F;
                    for (int i = 1; i < regionCount; i++)
                    {
                        if (hv_Circularity[i].F > tempCircularity)
                        {
                            maxCircularityIndex = i;
                            tempCircularity = hv_Circularity[maxCircularityIndex].F;
                        }
                    }
                    if (hv_Circularity[maxCircularityIndex].F >= minCircularity)
                    {
                        HOperatorSet.CopyObj(ho_SelectedRegions, out feedBackRegion, maxCircularityIndex + 1, 1);

                        HOperatorSet.AreaCenter(feedBackRegion, out hv_Area, out hv_Row, out hv_Column);

                        rowCoor = hv_Row[0].F;
                        columnCoor = hv_Column[0].F;
                    }
                    else
                    {
                        feedBackRegion = null;
                        return -1;
                    }
                }
            }


            return 0;
        }
    }

    public class ImageDispose
    {
        [DllImport("Kernel32.dll")]
        internal static extern void CopyMemory(Int64 dest, Int64 source, Int64 size);

        #region ConvertHalconGrayByteImageToBitmap:将Halcon 8位灰度图转换为Bitmap图像
        /// <summary>
        /// ConvertHalconGrayByteImageToBitmap:将Halcon 8位灰度图转换为Bitmap图像
        /// </summary>
        /// <param name="ho_Image_Gray">HObject:Halcon 8位灰度图</param>
        /// <param name="bmp">out Bitmap:Bitmap图像</param>
        /// <returns></returns>
        public static void ConvertHalconGrayByteImageToBitmap(HObject ho_Image_Gray, out Bitmap bmp)
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
        public static void ConvertHalconRGBImageToBitmap(HObject ho_Image_RGB, out Bitmap bmp)
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
        public static Bitmap ResizeBitmapImage(Bitmap oldbmp, int newWidth, int newHeight)
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
    }
}
