using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HalconDotNet;

namespace ThreeDimensionAVI
{
    /// <summary>
    /// MaintenanceCameraFormRequestOpenOrCloseCameraDelegate:界面请求打开或关闭相机
    /// </summary>
    /// <param name="cameraIndex">int:相机索引，0-左侧相机，1-右侧相机，2-后侧相机</param>
    public delegate void MaintenanceCameraFormRequestOpenOrCloseCameraDelegate(int cameraIndex);

    /// <summary>
    /// MaintenanceCameraFormRequestGrabImageAndDispDelegate:界面请求抓取图像并显示委托
    /// </summary>
    /// <param name="cameraIndex">int:相机索引，0-左侧相机，1-右侧相机，2-后侧相机</param>
    /// <param name="dispCrossLineFlag">bool:显示十字线标志，false-不显示，true-显示</param>
    public delegate void MaintenanceCameraFormRequestGrabImageAndDispDelegate(int cameraIndex, bool dispCrossLineFlag);

    /// <summary>
    /// MaintenanceCameraFormRequestGrabImageAndDispVideoDelegate:界面请求抓取图像并显示视频委托
    /// </summary>
    /// <param name="cameraIndex">int:相机索引，0-左侧相机，1-右侧相机，2-后侧相机,3-由于十字线显示与否切换引发的索引</param>
    /// <param name="dispCrossLineFlag">bool:显示十字线标志，false-不显示，true-显示</param>
    public delegate void MaintenanceCameraFormRequestGrabImageAndDispVideoDelegate(int cameraIndex, bool dispCrossLineFlag);

    /// <summary>
    /// MaintenanceCameraFormRequestSaveImageDelegate：界面请求保存图片委托
    /// </summary>
    /// <param name="cameraIndex">int:相机索引，0-左侧相机，1-右侧相机，2-后侧相机</param>  
    public delegate void MaintenanceCameraFormRequestSaveImageDelegate(int cameraIndex);

    /// <summary>
    /// MaintenanceCameraFormRequestBackCameraScanBarcode:请求后相机识别条码
    /// </summary>
    public delegate void MaintenanceCameraFormRequestBackCameraScanBarcodeDelegate();

    public partial class MaintenanceCameraForm : Form
    {
        /*--------------------------------------------------------------------------------------
        //Copyright (C) 2022 深圳市利器精工科技有限公司
        //版权所有
        //
        //文件名：相机显示画面
        //文件功能描述：用于手动控制显示相机画面
        //
        //
        //创建标识：MaLi 20220410
        //
        //修改标识：MaLi 20220410 Change
        //修改描述：增加相机显示画面
        //
        //------------------------------------------------------------------------------------*/
        //
        //******************************事件************************************//
        public event MaintenanceCameraFormRequestOpenOrCloseCameraDelegate MaintenanceCameraFormRequestOpenOrCloseCameraEvent;
        public event MaintenanceCameraFormRequestGrabImageAndDispDelegate MaintenanceCameraFormRequestGrabImageAndDispEvent;
        public event MaintenanceCameraFormRequestGrabImageAndDispVideoDelegate MaintenanceCameraFormRequestGrabImageAndDispVideoEvent;
        public event MaintenanceCameraFormRequestSaveImageDelegate MaintenanceCameraFormRequestSaveImageEvent;
        public event MaintenanceCameraFormRequestBackCameraScanBarcodeDelegate MaintenanceCameraFormRequestBackCameraScanBarcodeEvent;
        //*************************外部可读写变量*******************************//

        //*************************公共静态变量*******************************//

        //*************************内部私有变量*******************************//

        //
        //
        //

        public MaintenanceCameraForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// RequetCameraOpenOrCloseEventFunc：请求相机打开或关闭事件函数
        /// </summary>
        private void RequetCameraOpenOrCloseEventFunc(object sender, EventArgs e)
        {
            MyTool.TxtFileProcess.CreateLog("手动相机控制页面-请求开闭相机" + "----用户：" + BaseForm._operatorName,
            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

            if (BaseForm._autoModeFlag)
            {
                MyTool.TxtFileProcess.CreateLog("手动相机控制页面-请求开闭相机失败，当前设备处于自动模式" + "----用户：" + BaseForm._operatorName,
            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                MessageBox.Show("当前设备处于自动模式，无法进行手动控制！");
                return;
            }

            int cameraIndex = -1;
            switch (((Button)sender).Name)
            {
                case "btnLeftCameraOpen":
                    cameraIndex = 0;
                    break;
                case "btnRightCameraOpen":
                    cameraIndex = 1;
                    break;
                case "btnBackCameraOpen":
                    cameraIndex = 2;
                    break;
                default:
                    return;
            }

            if (MaintenanceCameraFormRequestOpenOrCloseCameraEvent != null)
            {
                MaintenanceCameraFormRequestOpenOrCloseCameraEvent(cameraIndex);
            }
        }

        /// <summary>
        /// RequetCameraGrabImageAndDispEventFunc：请求相机拍照并显示事件函数
        /// </summary>
        private void RequetCameraGrabImageAndDispEventFunc(object sender, EventArgs e)
        {
            MyTool.TxtFileProcess.CreateLog("手动相机控制页面-请求相机抓取并显示图像" + "----用户：" + BaseForm._operatorName,
            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

            if (BaseForm._autoModeFlag)
            {
                MyTool.TxtFileProcess.CreateLog("手动相机控制页面-请求相机抓取并显示图像失败，当前设备处于自动模式" + "----用户：" + BaseForm._operatorName,
                BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                MessageBox.Show("当前设备处于自动模式，无法进行手动控制！");
                return;
            }

            if (MaintenanceCameraFormRequestGrabImageAndDispEvent != null)
            {
                switch (((Button)sender).Name)
                {
                    case "btnLeftCameraGrabImage":
                        MaintenanceCameraFormRequestGrabImageAndDispEvent(0, false);
                        break;
                    case "btnRightCameraGrabImage":
                        MaintenanceCameraFormRequestGrabImageAndDispEvent(1, false);
                        break;
                    case "btnBackCameraGrabImage":
                        MaintenanceCameraFormRequestGrabImageAndDispEvent(2, chkDisCrossLine.Checked);
                        break;
                    default:
                        break;
                }

            }
        }

        /// <summary>
        /// RequetCameraGrabImageAndAndDispVideoFunc：请求相机拍照并显示视频事件函数
        /// </summary>
        private void RequetCameraGrabImageAndAndDispVideoFunc(object sender, EventArgs e)
        {
            MyTool.TxtFileProcess.CreateLog("手动相机控制页面-请求相机显示实时图像" + "----用户：" + BaseForm._operatorName,
            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

            if (BaseForm._autoModeFlag)
            {
                MyTool.TxtFileProcess.CreateLog("手动相机控制页面-请求相机显示实时图像失败，当前设备处于自动模式" + "----用户：" + BaseForm._operatorName,
                BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                MessageBox.Show("当前设备处于自动模式，无法进行手动控制！");
                return;
            }

            if (MaintenanceCameraFormRequestGrabImageAndDispVideoEvent != null)
            {
                switch (((Button)sender).Name)
                {
                    case "btnLeftCameraVideo":
                        MaintenanceCameraFormRequestGrabImageAndDispVideoEvent(0, false);
                        break;
                    case "btnRightCameraVideo":
                        MaintenanceCameraFormRequestGrabImageAndDispVideoEvent(1, false);
                        break;
                    case "btnBackCameraVideo":
                        MaintenanceCameraFormRequestGrabImageAndDispVideoEvent(2, chkDisCrossLine.Checked);
                        break;
                    default:
                        break;
                }

            }
        }

        /// <summary>
        /// RequestSaveImageEventFunc：请求保存图片事件函数
        /// </summary>
        private void RequestSaveImageEventFunc(object sender, EventArgs e)
        {
            MyTool.TxtFileProcess.CreateLog("手动相机控制页面-请求保存图像" + "----用户：" + BaseForm._operatorName,
            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

            if (BaseForm._autoModeFlag)
            {
                MyTool.TxtFileProcess.CreateLog("手动相机控制页面-请求保存图像失败，当前设备处于自动模式" + "----用户：" + BaseForm._operatorName,
                BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                MessageBox.Show("当前设备处于自动模式，无法进行手动控制！");
                return;
            }

            int cameraIndex = -1;
            switch (((Button)sender).Name)
            {
                case "btnLeftCameraImageSave":
                    cameraIndex = 0;
                    break;
                case "btnRightCameraImageSave":
                    cameraIndex = 1;
                    break;
                case "btnBackCameraImageSave":
                    cameraIndex = 2;
                    break;
                default:
                    return;
            }

            if (MaintenanceCameraFormRequestSaveImageEvent != null)
            {
                MaintenanceCameraFormRequestSaveImageEvent(cameraIndex);
            }
        }

        private void chkDisCrossLine_CheckedChanged(object sender, EventArgs e)
        {
            MyTool.TxtFileProcess.CreateLog("手动相机控制页面-切换显示十字线" + "----用户：" + BaseForm._operatorName,
            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

            if (btnBackCameraVideo.BackColor == Color.Green)//如果处于显示视频阶段
            {
                if (MaintenanceCameraFormRequestGrabImageAndDispVideoEvent != null && !BaseForm._autoModeFlag)
                {
                    MaintenanceCameraFormRequestGrabImageAndDispVideoEvent(3, chkDisCrossLine.Checked);
                }
            }
        }

        private void btnBackCameraScanBarcode_Click(object sender, EventArgs e)
        {
           MyTool.TxtFileProcess.CreateLog("手动相机控制页面-条形码识别" + "----用户：" + BaseForm._operatorName,
           BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

            if (BaseForm._autoModeFlag)
            {
                MyTool.TxtFileProcess.CreateLog("手动相机控制页面-条形码识别失败，当前设备处于自动模式" + "----用户：" + BaseForm._operatorName,
           BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                MessageBox.Show("当前设备处于自动模式，无法条形码识别！");
                return;
            }

            if (MaintenanceCameraFormRequestBackCameraScanBarcodeEvent != null)
            {
                MaintenanceCameraFormRequestBackCameraScanBarcodeEvent();
            }
        }
    }
}
