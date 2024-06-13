using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace ThreeDimensionAVI
{ 
    /// <summary>
    /// CylinderControlDelegate:声明委托-手动气缸控制委托
    /// </summary>
    /// <param name="cylinderControlIndex">cylinderControlIndex:气缸控制索引，0-相机移动气缸回，1-相机移动气缸出，
    /// 2-传送线1阻挡气缸下，3-传送线1阻挡气缸上，4-传送线1夹紧气缸松开，5-传送线1夹紧气缸夹紧，
    /// 6-传送线2阻挡气缸下，7-传送线2阻挡气缸上，8-传送线2夹紧气缸松开，9-传送线2夹紧气缸夹紧</param>
    public delegate void CylinderControlDelegate(int cylinderControlIndex);
    public partial class MaintenanceCylinderControlForm : Form
    {
        /*--------------------------------------------------------------------------------------
        //Copyright (C) 2022 深圳市利器精工科技有限公司
        //版权所有
        //
        //文件名：气缸手动控制界面
        //文件功能描述：用于调试时手动控制气缸的动作
        //
        //
        //创建标识：MaLi 20220402
        //
        //修改标识：MaLi 20220402 Change
        //修改描述：增加气缸手动控制程序事件等
        //
        //------------------------------------------------------------------------------------*/
        //
        //******************************事件************************************//
        public event CylinderControlDelegate CylinderControlEvent;//声明事件-手动气缸控制
        //*************************外部可读写变量*******************************//

        //*************************公共静态变量*******************************//

        //*************************内部私有变量*******************************//
        Thread _statusRefreshThread = null;
        //
        //
        //
        public MaintenanceCylinderControlForm()
        {
            InitializeComponent();

        }

        #region StatusRefreshThreadFunc：状态更新线程
        private void StatusRefreshThreadFunc()
        {
            while (true)
            {
                this.Invoke(new Action(() =>
                    {
                        if (BaseForm._inputIOT[13].STATUS)//如果相机移动气缸回传感器ON
                            btnCameraTransferCylinderBackStatus.BackColor = Color.LimeGreen;
                        else
                            btnCameraTransferCylinderBackStatus.BackColor = SystemColors.ControlDark;

                        if (BaseForm._inputIOT[14].STATUS)//如果相机移动气缸出传感器ON
                            btnCameraTransferCylinderOutStatus.BackColor = Color.LimeGreen;
                        else
                            btnCameraTransferCylinderOutStatus.BackColor = SystemColors.ControlDark;

                        if (BaseForm._inputIOT[84].STATUS)//如果传送线1阻挡气缸下传感器ON
                            btnConveyor1StopCylinderDownStatus.BackColor = Color.LimeGreen;
                        else
                            btnConveyor1StopCylinderDownStatus.BackColor = SystemColors.ControlDark;

                        if (BaseForm._inputIOT[85].STATUS)//如果传送线1阻挡气缸上传感器ON
                            btnConveyor1StopCylinderUpStatus.BackColor = Color.LimeGreen;
                        else
                            btnConveyor1StopCylinderUpStatus.BackColor = SystemColors.ControlDark;

                        if (BaseForm._inputIOT[86].STATUS)//如果传送线1夹紧气缸松开传感器ON
                            btnConveyor1ClampCylinderReleaseStatus.BackColor = Color.LimeGreen;
                        else
                            btnConveyor1ClampCylinderReleaseStatus.BackColor = SystemColors.ControlDark;

                        if (BaseForm._inputIOT[87].STATUS)//如果传送线1夹紧气缸夹紧传感器ON
                            btnConveyor1ClampCylinderClampStatus.BackColor = Color.LimeGreen;
                        else
                            btnConveyor1ClampCylinderClampStatus.BackColor = SystemColors.ControlDark;

                        if (BaseForm._inputIOT[80].STATUS)//如果传送线2阻挡气缸下传感器ON
                            btnConveyor2StopCylinderDownStatus.BackColor = Color.LimeGreen;
                        else
                            btnConveyor2StopCylinderDownStatus.BackColor = SystemColors.ControlDark;

                        if (BaseForm._inputIOT[81].STATUS)//如果传送线2阻挡气缸上传感器ON
                            btnConveyor2StopCylinderUpStatus.BackColor = Color.LimeGreen;
                        else
                            btnConveyor2StopCylinderUpStatus.BackColor = SystemColors.ControlDark;

                        if (BaseForm._inputIOT[82].STATUS)//如果传送线2夹紧气缸松开传感器ON
                            btnConveyor2ClampCylinderReleaseStatus.BackColor = Color.LimeGreen;
                        else
                            btnConveyor2ClampCylinderReleaseStatus.BackColor = SystemColors.ControlDark;

                        if (BaseForm._inputIOT[83].STATUS)//如果传送线2夹紧气缸夹紧传感器ON
                            btnConveyor2ClampCylinderClampStatus.BackColor = Color.LimeGreen;
                        else
                            btnConveyor2ClampCylinderClampStatus.BackColor = SystemColors.ControlDark;


                        if (BaseForm._outputIOT[4].STATUS)//如果触发相机移动气缸回
                        {
                            btnCameraTransferCylinder.Text = "相机移动气缸回";
                            btnCameraTransferCylinder.ForeColor = Color.Black;
                            btnCameraTransferCylinder.BackColor = Color.LimeGreen;
                        }
                        else
                        {
                            btnCameraTransferCylinder.Text = "相机移动气缸出";
                            btnCameraTransferCylinder.ForeColor = Color.White;
                            btnCameraTransferCylinder.BackColor = Color.Orange;
                        }

                        if (!BaseForm._outputIOT[81].STATUS)//如果触发传送线1阻挡气缸
                        {
                            btnConveyor1StopCylinder.Text = "传送线1阻挡气缸下";
                            btnConveyor1StopCylinder.ForeColor = Color.Black;
                            btnConveyor1StopCylinder.BackColor = Color.LimeGreen;
                        }
                        else
                        {
                            btnConveyor1StopCylinder.Text = "传送线1阻挡气缸上";
                            btnConveyor1StopCylinder.ForeColor = Color.White;
                            btnConveyor1StopCylinder.BackColor = Color.Orange;
                        }

                        if (!BaseForm._outputIOT[83].STATUS)//如果触发传送线1夹紧气缸
                        {
                            btnConveyor1ClampCylinder.Text = "传送线1夹紧气缸松开";
                            btnConveyor1ClampCylinder.ForeColor = Color.Black;
                            btnConveyor1ClampCylinder.BackColor = Color.LimeGreen;
                        }
                        else
                        {
                            btnConveyor1ClampCylinder.Text = "传送线1夹紧气缸夹紧";
                            btnConveyor1ClampCylinder.ForeColor = Color.White;
                            btnConveyor1ClampCylinder.BackColor = Color.Orange;
                        }

                        if (!BaseForm._outputIOT[77].STATUS)//如果触发传送线2阻挡气缸
                        {
                            btnConveyor2StopCylinder.Text = "传送2阻挡气缸下";
                            btnConveyor2StopCylinder.ForeColor = Color.Black;
                            btnConveyor2StopCylinder.BackColor = Color.LimeGreen;
                        }
                        else
                        {
                            btnConveyor2StopCylinder.Text = "传送线2阻挡气缸上";
                            btnConveyor2StopCylinder.ForeColor = Color.White;
                            btnConveyor2StopCylinder.BackColor = Color.Orange;
                        }

                        if (!BaseForm._outputIOT[79].STATUS)//如果触发传送线2夹紧气缸
                        {
                            btnConveyor2ClampCylinder.Text = "传送线2夹紧气缸松开";
                            btnConveyor2ClampCylinder.ForeColor = Color.Black;
                            btnConveyor2ClampCylinder.BackColor = Color.LimeGreen;
                        }
                        else
                        {
                            btnConveyor2ClampCylinder.Text = "传送线2夹紧气缸夹紧";
                            btnConveyor2ClampCylinder.ForeColor = Color.White;
                            btnConveyor2ClampCylinder.BackColor = Color.Orange;
                        }
                    })); ;
                Thread.Sleep(100);
            }
        }
        #endregion

        #region CylinderControlBtnEvent:气缸控制事件
        private void CylinderControlBtnEvent(object sender, EventArgs e)
        {
            MyTool.TxtFileProcess.CreateLog("手动气缸控制页面-请求气缸动作" + "----用户：" + BaseForm._operatorName,
            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

            int cylinderControlIndex = 0;
            string triggerBtnName = ((Button)sender).Name;

            switch (triggerBtnName)
            {
                case "btnCameraTransferCylinder":
                    if (!BaseForm._outputIOT[4].STATUS)//如果相机移动气缸回OFF
                        cylinderControlIndex = 0;
                    else
                        cylinderControlIndex = 1;
                    break;
                case "btnConveyor1StopCylinder":
                    if (BaseForm._outputIOT[81].STATUS)//如果传送线1阻挡气缸下OFF
                        cylinderControlIndex = 2;
                    else
                        cylinderControlIndex = 3;
                    break;
                case "btnConveyor1ClampCylinder":
                    if (BaseForm._outputIOT[83].STATUS)//如果传送线1夹紧气缸松开OFF
                        cylinderControlIndex = 4;
                    else
                        cylinderControlIndex = 5;
                    break;
                case "btnConveyor2StopCylinder":
                    if (BaseForm._outputIOT[77].STATUS)//如果传送线2阻挡气缸下OFF
                        cylinderControlIndex = 6;
                    else
                        cylinderControlIndex = 7;
                    break;
                case "btnConveyor2ClampCylinder":
                    if (BaseForm._outputIOT[79].STATUS)//如果传送线2夹紧气缸松开OFF
                        cylinderControlIndex = 8;
                    else
                        cylinderControlIndex = 9;
                    break;
                default:
                    return;
            }

            if (BaseForm._autoModeFlag)
            {
                MyTool.TxtFileProcess.CreateLog("手动气缸控制页面-请求气缸动作失败，当前设备处于自动模式" + "----用户：" + BaseForm._operatorName,
                BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                MessageBox.Show("当前设备处于自动模式，无法手动控制气缸的动作！");
                return;
            }

            if (CylinderControlEvent != null)
                CylinderControlEvent(cylinderControlIndex);
        }
        #endregion

        private void MaintenanceCylinderControlForm_Load(object sender, EventArgs e)
        {
            if (_statusRefreshThread != null)
            {
                _statusRefreshThread.Abort();
                _statusRefreshThread = null;
            }
            _statusRefreshThread = new Thread(StatusRefreshThreadFunc);
            _statusRefreshThread.IsBackground = true;
            _statusRefreshThread.Start();
            //_statusRefreshThread.Suspend();
        }

        private void MaintenanceCylinderControlForm_VisibleChanged(object sender, EventArgs e)
        {
            //if (this.Visible)
            //{
            //    if (_statusRefreshThread != null)
            //    {
            //        if (!_statusRefreshThread.IsAlive)//如果线程处于不活动状态
            //            _statusRefreshThread.Resume();//线程继续
            //    }
            //}
            //else
            //{
            //    if (_statusRefreshThread != null)
            //    {
            //        if (_statusRefreshThread.IsAlive)//如果线程处于不活动状态
            //            _statusRefreshThread.Suspend();//线程继续
            //    }
            //}
        }
    }
}
