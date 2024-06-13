using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HalconDotNet;//C:\Program Files\MVTec\HALCON-18.11-Progress\bin\dotnet35
using System.Windows.Forms;
using System.Threading;
using System.IO;

namespace HalconMVTec
{  
    public class HalconCameraControl
    {     
        /*--------------------------------------------------------------------------------------
        //Copyright (C) 2022 深圳市利器精工科技有限公司
        //版权所有
        //
        //文件名：利用halcon对相机控制类
        //文件功能描述：利用halcon对相机进行控制，包含相机打开关闭，采集图像，实时显示图像等
        //
        //
        //创建标识：MaLi 20220410
        //
        //修改标识：MaLi 20220410 Change
        //修改描述：增加利用halcon对相机控制类
        //
        //------------------------------------------------------------------------------------*/
        //
        //******************************事件************************************//

        //*************************外部可读写变量*******************************//
        public HTuple hv_AcqHandle = null;//设备句柄
        public HObject ho_Image = null;//获取到的图像
        bool _getImageContinuely = false;//连续获取图像标志，false-不连续获取，true-连续获取
        //*************************公共静态变量*******************************//

        //*************************内部私有变量*******************************//
        private HTuple hv_GrabTimeOut = 1000;//获取图像超时时间

        //
        //
        public HalconCameraControl()
        {

        }

        #region OpenCameraInExternalTrigger:打开相机，指定相机触发方式为外部触发
        /// <summary>
        /// OpenCameraInExternalTrigger:打开相机，指定相机触发方式为外部触发
        /// </summary>
        /// <param name="name">HALCON image acquisition interface name</param>
        /// <param name="horizontalResolution">Desired horizontal resolution of image acquisition interface (absolute value or 1 for full resolution, 2 for half resolution, or 4 for quarter resolution).</param>
        /// <param name="verticalResolution">Desired vertical resolution of image acquisition interface (absolute value or 1 for full resolution, 2 for half resolution, or 4 for quarter resolution).</param>
        /// <param name="imageWidth">Width of desired image part (absolute value or 0 for HorizontalResolution - 2*StartColumn).</param>
        /// <param name="imageHeight">Width of desired image part (absolute value or 0 for HorizontalResolution - 2*StartColumn).</param>
        /// <param name="startRow">Width of desired image part (absolute value or 0 for HorizontalResolution - 2*StartColumn).</param>
        /// <param name="startColumn">Width of desired image part (absolute value or 0 for HorizontalResolution - 2*StartColumn).</param>
        /// <param name="field">Width of desired image part (absolute value or 0 for HorizontalResolution - 2*StartColumn).</param>
        /// <param name="bitsPerChannel">Number of transferred bits per pixel and image channel (-1: device-specific default value).</param>
        /// <param name="colorSpace">Output color format of the grabbed images, typically 'gray' or 'raw' for single-channel or 'rgb' or 'yuv' for three-channel images ('default': device-specific default value).</param>
        /// <param name="generic">Generic parameter with device-specific meaning.</param>
        /// <param name="externalTrigger">External triggering.</param>
        /// <param name="cameraType">Type of used camera ('default': device-specific default value).</param>
        /// <param name="device">Device the image acquisition device is connected to ('default': device-specific default value).</param>
        /// <param name="port">Port the image acquisition device is connected to (-1: device-specific default value).</param>
        /// <param name="lineIn">Camera input line of multiplexer (-1: device-specific default value).</param>
        /// <param name="dispErrorMsgFlag">bool:是否显示相机错误信息</param>
        /// <returns></returns>
        public bool OpenCameraInExternalTrigger(HTuple name, HTuple horizontalResolution, HTuple verticalResolution,
            HTuple imageWidth, HTuple imageHeight, HTuple startRow, HTuple startColumn, HTuple field, HTuple bitsPerChannel,
            HTuple colorSpace, HTuple generic, HTuple externalTrigger, HTuple cameraType, HTuple device, HTuple port, HTuple lineIn,bool dispErrorMsgFlag)
        {
            if (hv_AcqHandle != null)
            {
                return true;
            }
            try
            {
                //获取相机句柄
                HOperatorSet.OpenFramegrabber(name, horizontalResolution, verticalResolution, imageWidth, imageHeight, startRow, startColumn,
                    field, bitsPerChannel, colorSpace, generic, externalTrigger, cameraType, device, port, lineIn, out hv_AcqHandle);
                //帧触发模式(获取开始采集的时间)
                //HOperatorSet.SetFramegrabberParam(hv_AcqHandle_L, "FrameStartTriggerMode", "On");
                HOperatorSet.SetFramegrabberParam(hv_AcqHandle, "grab_timeout", hv_GrabTimeOut);
                HOperatorSet.SetFramegrabberParam(hv_AcqHandle, "LineSelector", "Line1");
                //HOperatorSet.SetFramegrabberParam(hv_AcqHandle_L, "LineFormat", "TTL");
                HOperatorSet.GrabImageStart(hv_AcqHandle, -1);
                return true;
            }
            catch (Exception ex)
            {
                if (dispErrorMsgFlag)
                    MessageBox.Show(ex.ToString());
                hv_AcqHandle = null;
                return false;
            }
        }


        #endregion

        #region OpenCameraInSoftTrigger:打开相机，指定相机触发方式为软触发
        /// <summary>
        /// OpenCameraInExternalTrigger:打开相机，指定相机触发方式为外部触发
        /// </summary>
        /// <param name="name">HALCON image acquisition interface name</param>
        /// <param name="horizontalResolution">Desired horizontal resolution of image acquisition interface (absolute value or 1 for full resolution, 2 for half resolution, or 4 for quarter resolution).</param>
        /// <param name="verticalResolution">Desired vertical resolution of image acquisition interface (absolute value or 1 for full resolution, 2 for half resolution, or 4 for quarter resolution).</param>
        /// <param name="imageWidth">Width of desired image part (absolute value or 0 for HorizontalResolution - 2*StartColumn).</param>
        /// <param name="imageHeight">Width of desired image part (absolute value or 0 for HorizontalResolution - 2*StartColumn).</param>
        /// <param name="startRow">Width of desired image part (absolute value or 0 for HorizontalResolution - 2*StartColumn).</param>
        /// <param name="startColumn">Width of desired image part (absolute value or 0 for HorizontalResolution - 2*StartColumn).</param>
        /// <param name="field">Width of desired image part (absolute value or 0 for HorizontalResolution - 2*StartColumn).</param>
        /// <param name="bitsPerChannel">Number of transferred bits per pixel and image channel (-1: device-specific default value).</param>
        /// <param name="colorSpace">Output color format of the grabbed images, typically 'gray' or 'raw' for single-channel or 'rgb' or 'yuv' for three-channel images ('default': device-specific default value).</param>
        /// <param name="generic">Generic parameter with device-specific meaning.</param>
        /// <param name="externalTrigger">External triggering.</param>
        /// <param name="cameraType">Type of used camera ('default': device-specific default value).</param>
        /// <param name="device">Device the image acquisition device is connected to ('default': device-specific default value).</param>
        /// <param name="port">Port the image acquisition device is connected to (-1: device-specific default value).</param>
        /// <param name="lineIn">Camera input line of multiplexer (-1: device-specific default value).</param>
        /// <returns></returns>
        public bool OpenCameraInSoftTrigger(HTuple name, HTuple horizontalResolution, HTuple verticalResolution,
            HTuple imageWidth, HTuple imageHeight, HTuple startRow, HTuple startColumn, HTuple field, HTuple bitsPerChannel,
            HTuple colorSpace, HTuple generic, HTuple externalTrigger, HTuple cameraType, HTuple device, HTuple port, HTuple lineIn, bool dispErrorMsgFlag)
        {
            if (hv_AcqHandle != null)
            {
                return true;
            }
            try
            {
                for (int i = 0; i < 3; i++)
                {
                    try
                    {
                        //获取相机句柄
                        HOperatorSet.OpenFramegrabber(name, horizontalResolution, verticalResolution, imageWidth, imageHeight, startRow, startColumn,
                    field, bitsPerChannel, colorSpace, generic, externalTrigger, cameraType, device, port, lineIn, out hv_AcqHandle);

                        //modify 20240403
                        //这句代码不能运行
                        //HOperatorSet.SetFramegrabberParam(hv_AcqHandle, "FrameStartTriggerMode", "On");
                        //这句代码不能运行
                        //HOperatorSet.SetFramegrabberParam(hv_AcqHandle, "TriggerSelector", "FrameStart");
                        //这句代码不能运行
                        //HOperatorSet.SetFramegrabberParam(hv_AcqHandle, "external_trigger", "true");
                        HOperatorSet.SetFramegrabberParam(hv_AcqHandle, "grab_timeout", 1000);
                        HOperatorSet.SetFramegrabberParam(hv_AcqHandle, "TriggerMode", "On");
                        HOperatorSet.SetFramegrabberParam(hv_AcqHandle, "TriggerSource", "Software");
                        HOperatorSet.GrabImageStart(hv_AcqHandle, -1);

                        break;
                    }
                    catch (Exception ex)
                    {
                        if (i < 3)
                            continue;
                        if (dispErrorMsgFlag)
                            MessageBox.Show(ex.ToString());
                        hv_AcqHandle = null;
                        return false;
                    }
                }

                //try
                //{
                //    HOperatorSet.SetFramegrabberParam(hv_AcqHandle, "[Stream]StreamBufferHandlingMode", "NewestOnly");
                //}
                //catch { }
                return true;
            }
            catch (Exception ex)
            {
                if (dispErrorMsgFlag)
                    MessageBox.Show(ex.ToString());
                hv_AcqHandle = null;
                return false;
            }
        }
        #endregion

        #region CloseCamera:关闭相机
        /// <summary>
        /// CloseCamera:关闭相机
        /// </summary>
        /// <returns></returns>
        public bool CloseCamera()
        {
            if (hv_AcqHandle == null)
            {
                return true;
            }
            try
            {
                HOperatorSet.CloseFramegrabber(hv_AcqHandle);
                hv_AcqHandle = null;
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                hv_AcqHandle = null;
                return false;
            }
        }

        /// <summary>
        /// CloseCamera2:关闭相机
        /// </summary>
        /// <returns></returns>
        public bool CloseCamera2()
        {
            if (hv_AcqHandle == null)
            {
                return true;
            }
            try
            {
                HOperatorSet.CloseFramegrabber(hv_AcqHandle);
                hv_AcqHandle = null;
                return true;
            }
            catch
            {
                //MessageBox.Show(ex.ToString());
                hv_AcqHandle = null;
                return false;
            }
        }

        /// <summary>
        /// CloseCamera3:关闭相机
        /// </summary>
        /// <returns></returns>
        public bool CloseCamera3()
        {
            if (hv_AcqHandle == null)
            {
                return true;
            }
            try
            {
                HOperatorSet.CloseFramegrabber(hv_AcqHandle);
                hv_AcqHandle = null;
                return true;
            }
            catch
            {
                //MessageBox.Show(ex.ToString());
                //hv_AcqHandle = null;
                return false;
            }
        }
        #endregion

        #region GrabImage:获取图像

        /// <summary>
        /// GrabImage:异步获取图像
        /// </summary>
        /// <returns></returns>
        public bool GrabImage(bool dispInfoFlag)
        {
            try
            {
                HOperatorSet.GrabImageAsync(out ho_Image, hv_AcqHandle, -1);
                return true;
            }
            catch (Exception ex)
            {
                if (dispInfoFlag)
                    MessageBox.Show(ex.ToString());
                //HOperatorSet.CloseFramegrabber(hv_AcqHandle);
                //hv_AcqHandle = null;
                return false;
            }
        }

        //modify 20240403
        /// <summary>
        /// GrabImage_SoftwareTriggerAsync:软触发异步获取图像
        /// </summary>
        /// <returns></returns>
        public bool GrabImage_SoftwareTriggerAsync(bool dispInfoFlag)
        {
            try
            {
                HOperatorSet.SetFramegrabberParam(hv_AcqHandle, "TriggerSoftware", "");
                HOperatorSet.GrabImageAsync(out ho_Image, hv_AcqHandle, -1);
                return true;
            }
            catch (Exception ex)
            {
                if (dispInfoFlag)
                    MessageBox.Show(ex.ToString());
                //HOperatorSet.CloseFramegrabber(hv_AcqHandle);
                //hv_AcqHandle = null;
                return false;
            }
        }

        /// <summary>
        /// GrabImage2:同步获取图像
        /// </summary>
        /// <param name="dispInfoFlag"></param>
        /// <returns></returns>
        public bool GrabImage2(bool dispInfoFlag)//报超时,不能用
        {
            try
            {
                //modify 20240403
                HOperatorSet.SetFramegrabberParam(hv_AcqHandle, "TriggerSoftware", "");

                HOperatorSet.GrabImage(out ho_Image, hv_AcqHandle);

                return true;
            }
            catch (Exception ex)
            {
                if (dispInfoFlag)
                    MessageBox.Show(ex.ToString());
                //HOperatorSet.CloseFramegrabber(hv_AcqHandle);
                //hv_AcqHandle = null;
                return false;
            }

        }

        /// <summary>
        /// GrabImage3:异步获取图像，获取所有内存中的图像，并取出最后一个
        /// </summary>
        /// <returns></returns>
        public bool GrabImage3(bool dispInfoFlag)
        {
            HObject tempImage = null;
            HTuple imageCount = null;
            ho_Image = null;
            try
            {
                try
                {
                    HOperatorSet.GrabImageAsync(out tempImage, hv_AcqHandle, 1000);
                    if (tempImage != null)
                    {
                        HOperatorSet.CountObj(tempImage, out imageCount);
                        HOperatorSet.CopyObj(tempImage, out ho_Image, imageCount, 1);
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    if (dispInfoFlag)
                        MessageBox.Show(ex.ToString());
                    //HOperatorSet.CloseFramegrabber(hv_AcqHandle);
                    //hv_AcqHandle = null;
                    return false;
                }

                //while (tempImage != null)//此处使用try catch有点太浪费时间
                //{
                //    tempImage = null;
                //    HOperatorSet.GrabImageAsync(out tempImage, hv_AcqHandle, 0);
                //    if (tempImage != null)
                //    {
                //        HOperatorSet.CountObj(tempImage, out imageCount);
                //        HOperatorSet.CopyObj(tempImage, out ho_Image, imageCount, 1);
                //    }
                //}

                //HOperatorSet.GrabImageAsync(out ho_Image, hv_AcqHandle, -1);

                //return true;
            }
            catch
            {

            }

            tempImage = null;
            imageCount = null;
            if (ho_Image != null)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        #endregion

        #region ClearCameraImage:清除相机缓存图像
        /// <summary>
        /// ClearCameraImage:清除相机缓存图像
        /// </summary>
        public void ClearCameraImage()
        {
            HObject tempImage = null;
            try
            {
                try
                {
                    HOperatorSet.GrabImageAsync(out tempImage, hv_AcqHandle, 0);
                }
                catch
                { 
                    
                }

                while (tempImage != null)//此处使用try catch有点太浪费时间
                {
                    tempImage = null;
                    HOperatorSet.GrabImageAsync(out tempImage, hv_AcqHandle, 0);
                }
            }
            catch
            {

            }
            tempImage = null;
        }
        #endregion

        #region ImageGrabImageContinuelyThreadFunc:相机连续获取图像线程函数
        /// <summary>
        /// ImageGrabImageContinuelyThreadFunc:相机连续获取图像线程函数
        /// </summary>
        private void ImageGrabImageContinuelyThreadFunc()
        {
            while (true)
            {
                if (hv_AcqHandle != null && _getImageContinuely)
                {
                    GrabImage(false);
                }
                Thread.Sleep(10);
            }
        }
        #endregion


        /*************************静态函数***************************/
        #region DispImageAdaptively(自适应显示图像,不会改变图像长宽比)
        /// <summary>
        /// 自适应显示图像,不会改变图像长宽比
        /// </summary>
        /// <param name="hWindowControl">HWindowControl:显示窗口句柄</param>
        /// <param name="ho_DispImage">Hobject:需要显示的图片</param> 
        /// <param name="dispCrossLineFlag">bool:显示十字线标志，false-不显示，true-显示</param>
        public static void DispImageAdaptively(HWindowControl hWindowControl, HObject ho_DispImage, bool dispCrossLineFlag)
        {
            int mW = hWindowControl.Width;
            int mH = hWindowControl.Height;
            HTuple hv_width = new HTuple(); HTuple hv_height = new HTuple();

            HOperatorSet.ClearWindow(hWindowControl.HalconWindow);
            HOperatorSet.GetImageSize(ho_DispImage, out hv_width, out hv_height);

            int imageW = Convert.ToInt32(hv_width.ToString());
            int imageH = Convert.ToInt32(hv_height.ToString());

            if (mW > 0 && mH > 0)
            {
                double mScale_Window = Convert.ToDouble(mW) / Convert.ToDouble(mH);
                double mScale_Image = Convert.ToDouble(imageW) / Convert.ToDouble(imageH);
                double row1, column1, row2, column2;
                int mH_1 = Convert.ToInt32(mW / mScale_Image);
                System.Drawing.Rectangle rect = hWindowControl.ImagePart;

                if (mH_1 > mH)
                {
                    row1 = 0;
                    row2 = imageH;
                    double mImage_w = imageH * mScale_Window - imageW;
                    double mD_Image_W = Math.Abs(mImage_w / 2.0);
                    column1 = mD_Image_W;
                    column2 = imageW + mD_Image_W;
                    rect.X = -(int)Math.Round(mD_Image_W);
                    rect.Y = 0;
                    rect.Height = imageH;
                    rect.Width = (int)Math.Round(imageH * mScale_Window);
                }
                else
                {
                    column1 = 0;
                    column2 = imageW;
                    double mImage_h = Convert.ToDouble(imageW) / mScale_Window - imageH;
                    double mD_Image_H = Math.Abs(mImage_h / 2.0);
                    row1 = mD_Image_H;
                    row2 = imageH + mD_Image_H;

                    rect.X = 0;
                    rect.Y = -(int)Math.Round(mD_Image_H);
                    rect.Height = (int)Math.Round(Convert.ToDouble(imageW) / mScale_Window);
                    rect.Width = imageW;
                }
                hWindowControl.ImagePart = rect;

            }

            HOperatorSet.DispObj(ho_DispImage, hWindowControl.HalconWindow);
            if (dispCrossLineFlag)
            {
                HOperatorSet.SetColor(hWindowControl.HalconWindow, "green");
                HOperatorSet.DispLine(hWindowControl.HalconWindow, hv_height / 2, 0, hv_height / 2, hv_width);
                HOperatorSet.DispLine(hWindowControl.HalconWindow, 0, hv_width / 2, hv_height, hv_width / 2);
            }
        }
        #endregion

        #region SaveImage:保存halcon Image至指定路径
        /// <summary>
        /// SaveImage:保存halcon Image至指定路径
        /// </summary>
        /// <param name="ho_SaveImage">HObject：想要保存的图像</param>
        /// <param name="hv_ImageFormat">string:图像格式</param>
        /// <param name="hv_SavePath">string：想要保存的图像路径，不包含后缀</param>
        /// <returns>bool:保存结果，true-成功，false-失败</returns>
        public static bool SaveImage(HObject ho_SaveImage,string imageFormat,string savePath)
        {
            HTuple hv_imageFormat = imageFormat;
            HTuple hv_savePath = savePath;
            string folderPath = "";//包含最后的反斜杠
            try
            {
                folderPath = savePath.Substring(0,savePath.LastIndexOf("\\")+1);
            }
            catch
            {
                return false;
            }

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            try
            {
                HOperatorSet.WriteImage(ho_SaveImage, hv_imageFormat, 0, hv_savePath);
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region SaveWindowImage:保存窗口中的图像至指定路径
        /// <summary>
        /// SaveImage:保存halcon Image至指定路径
        /// </summary>
        /// <param name="hWindowControl">HWindowControl:显示窗口句柄</param>
        /// <param name="hv_ImageFormat">string:图像格式</param>
        /// <param name="hv_SavePath">string：想要保存的图像路径，不包含后缀</param>
        /// <returns>bool:保存结果，true-成功，false-失败</returns>
        public static bool SaveWindowImage(ref HWindowControl hWindowControl,string imageFormat, string savePath)
        {
            HTuple hv_imageFormat = imageFormat;
            HTuple hv_savePath = savePath;
            HObject ho_saveImage = null;
            try
            {
                HOperatorSet.DumpWindowImage(out ho_saveImage, hWindowControl.HalconWindow);
                if (SaveImage(ho_saveImage, imageFormat, savePath))
                    return true;
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region SetDisplayFont：设置在窗口上显示的字体及颜色等
        // Chapter: Graphics / Text
        // Short Description: Set font independent of OS 
        public static void SetDisplayFont(HTuple hv_WindowHandle, HTuple hv_Size, HTuple hv_Font,
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
    }
}
