using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using XMLFile;
using System.Threading;
using CSVFile;
using System.Collections;
using System.Drawing.Printing;//用于启用printdocument类，来打印文件，pdf等
using System.Drawing.Drawing2D;
using SoftKeyBoard;
using HalconDotNet;
using System.Xml;

namespace ThreeDimensionAVI
{
    public delegate void MainFormRequestCloseSoftwareDelegate();//声明委托-请求关闭软件
    public delegate void MainFormRequestChangeFormToMaintenanceDelegate();//声明委托-请求切换界面至手动界面
    public delegate void MainFormClickVirtualButtonDelegate(int virtualButtonIndex);//声明委托-按下虚拟案件
    public delegate void MainFormChangeProductDelegate(string productName,int changeIndex);//声明委托-切换产品
    public delegate void MainFormConveyorForcedMoveOutDelegate(int conveyorIndex);//声明委托-传送线强制流出产品 
    public partial class MainForm : Form
    {

        /*--------------------------------------------------------------------------------------
        //Copyright (C) 2022 深圳市利器精工科技有限公司
        //版权所有
        //
        //文件名：主界面
        //文件功能描述：用于主要的显示等
        //
        //
        //创建标识：MaLi 20220317
        //
        //修改标识：MaLi 20220317 Change
        //修改描述：增加主界面
        //
        //------------------------------------------------------------------------------------*/
        //
        //******************************事件************************************//
        public event MainFormRequestCloseSoftwareDelegate MainFormRequestCloseSoftwareEvent;//声明事件-请求关闭软件
        public event MainFormRequestChangeFormToMaintenanceDelegate MainFormRequestChangeFormToMaintenanceEvent;//声明事件-请求切换界面至手动界面
        public event MainFormClickVirtualButtonDelegate MainFormClickVirtualButtonEvent;//声明事件-按下虚拟按钮
        public event MainFormChangeProductDelegate MainFormChangeProductEvent;//声明事件-切换产品
        public event MainFormConveyorForcedMoveOutDelegate MainFormConveyorForcedMoveOutEvent;//声明事件-传送线强制流出产品 
        //*************************外部可读写变量*******************************//
        //*************************公共静态变量*******************************//
        public static bool _stepModeFlag = false;//步进按钮状态，false-关闭，true-触发
        //*************************内部私有变量*******************************//
        const int MAX_USER_QUANITTY = 10;//最多操作人员数
        ProductSelectForm _productSelectFormVariate = null;//产品选择窗口界面变量
        ////////////////////////////////////////////////////

        //构造函数
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }


        //form显示变换事件
        private void MainForm_VisibleChanged(object sender, EventArgs e)
        {
            //设置当前产品名
            lblTrack1ActualProduct1.Text = SettingsProductForm._track1ActualsProductName;
            lblTrack2ActualProduct1.Text = SettingsProductForm._track2ActualsProductName;

        }

        //当数据发生变化时
        private void Batch_Number_TextBox_TextChanged(object sender, EventArgs e)
        {
            //SavingSettingData();
        }

        //点击程序退出按钮
        private void picExit_Click(object sender, EventArgs e)
        {
            MyTool.TxtFileProcess.CreateLog("主界面-请求退出软件！" + "----用户：" + BaseForm._operatorName,
            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

            if (MainFormRequestCloseSoftwareEvent != null)
            {
                MainFormRequestCloseSoftwareEvent();
            }
        }

        //点击切换至手动界面按钮
        private void picGotoSettingForm_Click(object sender, EventArgs e)
        {
            MyTool.TxtFileProcess.CreateLog("主界面-请求切换手动模式！" + "----用户：" + BaseForm._operatorName,
            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

            if (MainFormRequestChangeFormToMaintenanceEvent != null)
            {
                MainFormRequestChangeFormToMaintenanceEvent();
            }
        }

        #region  产品切换相关函数

        //点击产品切换按钮
        private void ProductChangeEvent(object sender, EventArgs e)
        {
            MyTool.TxtFileProcess.CreateLog("主界面-请求更换产品！" + "----用户：" + BaseForm._operatorName,
            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

            if (BaseForm._autoModeFlag)
            {
                MyTool.TxtFileProcess.CreateLog("主界面-请求更换产品失败，设备当前处于自动模式！" + "----用户：" + BaseForm._operatorName,
                BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                MessageBox.Show("当前设备处于自动模式，无法切换产品！");
                return;
            }

            #region 创建产品选择窗口

            this.Invoke(new Action(() =>
            {
                
                if (_productSelectFormVariate != null)
                {
                    _productSelectFormVariate.Dispose();
                    _productSelectFormVariate = null;
                }

                if (((Label)sender).Name == "lblTrack1ActualProduct" || ((Label)sender).Name == "lblTrack1ActualProduct1")
                    _productSelectFormVariate = new ProductSelectForm(0);
                else
                    _productSelectFormVariate = new ProductSelectForm(1);

                _productSelectFormVariate.SendProductSelectResult += new SENDPRODUCTSELECTRESULT(GetProductSelectResult);
                _productSelectFormVariate.StartPosition = FormStartPosition.CenterScreen;
                _productSelectFormVariate.TopLevel = true;
                _productSelectFormVariate.Show();
            })); ;

            #endregion
        }

        //获取弹出的产品选择界面选择的产品名称以及选择结果事件
        private void GetProductSelectResult(string selectProductName, bool changeProductFlag,int changeIndex)
        {
            if (changeProductFlag)
            {
                if (MainFormChangeProductEvent != null)
                {
                    MainFormChangeProductEvent(selectProductName, changeIndex);
                }
            }
            else
            {
                MyTool.TxtFileProcess.CreateLog("主界面-放弃产品更换！" + "----用户：" + BaseForm._operatorName,
                BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);
                    MessageBox.Show("放弃产品切换！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            if (_productSelectFormVariate != null)
            {
                _productSelectFormVariate.Dispose();
                _productSelectFormVariate = null;
            }
        }

        #endregion

        //点击步进按钮
        private void btnStep_Click(object sender, EventArgs e)
        {
            _stepModeFlag = !_stepModeFlag;
            if (!_stepModeFlag)
            {
                MyTool.TxtFileProcess.CreateLog("主界面-设置设备至步进模式！" + "----用户：" + BaseForm._operatorName,
                BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                btnStep.BackColor = SystemColors.Control;
                btnStep.ForeColor = Color.Black;
            }
            else
            {
                MyTool.TxtFileProcess.CreateLog("主界面-取消设备的步进模式！" + "----用户：" + BaseForm._operatorName,
                BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                btnStep.BackColor = Color.LimeGreen;
                btnStep.ForeColor = Color.Black;
            }
        }

        #region 虚拟按钮按下松开事件
        //点击虚拟按钮事件
        private void PressVirtualButtonEvent(object sender, MouseEventArgs e)
        {
            int virtualButtonIndex = -1;
            switch (((Button)sender).Name)
            { 
                case "btnAuto"://点击自动按钮
                    MyTool.TxtFileProcess.CreateLog("主界面-点击自动按钮！" + "----用户：" + BaseForm._operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                    virtualButtonIndex = 0;
                    break;
                case "btnManual"://点击手动按钮
                    MyTool.TxtFileProcess.CreateLog("主界面-点击手动按钮！" + "----用户：" + BaseForm._operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                    virtualButtonIndex = 1;
                    break;
                case "btnMeasureStart"://点击检测启动按钮
                    MyTool.TxtFileProcess.CreateLog("主界面-点击检测启动按钮！" + "----用户：" + BaseForm._operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                    virtualButtonIndex = 2;
                    break;
                case "btnStart"://点击启动按钮
                    MyTool.TxtFileProcess.CreateLog("主界面-点击启动按钮！" + "----用户：" + BaseForm._operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                    virtualButtonIndex = 10;
                    break;
                case "btnStop"://点击停止按钮
                    MyTool.TxtFileProcess.CreateLog("主界面-点击停止按钮！" + "----用户：" + BaseForm._operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                    virtualButtonIndex = 11;
                    break;
                case "btnReset"://点击复位按钮
                    MyTool.TxtFileProcess.CreateLog("主界面-点击复位启动按钮！" + "----用户：" + BaseForm._operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                    virtualButtonIndex = 12;
                    break;
                default:
                    MyTool.TxtFileProcess.CreateLog("主界面-点击无效按钮！" + "----用户：" + BaseForm._operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                    virtualButtonIndex = -1;
                    break;
            }
            if (MainFormClickVirtualButtonEvent != null)
                MainFormClickVirtualButtonEvent(virtualButtonIndex);
        }

        //鼠标离开虚拟按钮事件
        private void MouseLeaveVirtualButtonEvent(object sender, EventArgs e)
        {
            if (MainFormClickVirtualButtonEvent != null)
                MainFormClickVirtualButtonEvent(-1);
        }

        //鼠标松开虚拟按钮事件
        private void MouseUpVirtualButtonEvent(object sender, MouseEventArgs e)
        {
            if (MainFormClickVirtualButtonEvent != null)
                MainFormClickVirtualButtonEvent(-1);
        }

        #endregion

        //传送线强制流出产品点击事件
        private void ConveyorForcedMoveOutClickEvent(object sender, EventArgs e)
        {
            if (MainFormConveyorForcedMoveOutEvent != null)
            {
                if (((UCArrow.UCArrow)sender).Name == "btnConveyor1ProductForcedMoveOut")//如果点击的时传送线1强制流出产品
                {
                    MyTool.TxtFileProcess.CreateLog("主界面-请求传送线1强制流出产品！" + "----用户：" + BaseForm._operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                    MainFormConveyorForcedMoveOutEvent(0);
                }
                else if (((UCArrow.UCArrow)sender).Name == "btnConveyor2ProductForcedMoveOut")//如果点击的时传送线2强制流出产品
                {
                    MyTool.TxtFileProcess.CreateLog("主界面-请求传送线2强制流出产品！" + "----用户：" + BaseForm._operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                    MainFormConveyorForcedMoveOutEvent(1);
                }
            }
        }

    }
}
 