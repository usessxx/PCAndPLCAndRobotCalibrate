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
using SoftKeyBoard;
using System.Threading;

namespace ThreeDimensionAVI
{
    public delegate void MaintenanceAxesControlFormRequestXYMoveToAssignedPointDelegate(int assignedPoint,int trackIndex);//声明委托，轴控制手动界面请求XY移动至指定点位
    public delegate void MaintenanceAxesControlFormRequestChangeConveyorWidthDelegate(int conveyorControlFlag);//声明委托，轴控制手动画面请求调节传送线宽度
    /// <summary>
    /// MaintenanceAxesControlFormRequestConveyorActionDelegate:声明委托，轴控制手动画面请求控制传送线
    /// </summary>
    /// <param name="conveyorActionSpeed">float:传送线动作速度，小于等于0-停止，其余-其余动作速度</param>
    /// <param name="conveyorIndex">int:控制的传送线索引，0-传送线1，1-传送线2</param>
    public delegate void MaintenanceAxesControlFormRequestConveyorActionDelegate(float conveyorActionSpeed,int conveyorIndex);//声明委托，轴控制手动画面请求控制传送线
    public partial class MaintenanceAxesControlForm : Form
    {
        /*--------------------------------------------------------------------------------------
        //Copyright (C) 2022 深圳市利器精工科技有限公司
        //版权所有
        //
        //文件名：轴手动控制界面
        //文件功能描述：用于调试时手动控制轴的运动
        //
        //
        //创建标识：MaLi 20220319
        //
        //修改标识：MaLi 20220319 Change
        //修改描述：增加轴手动控制界面
        //
        //------------------------------------------------------------------------------------*/
        //
        //******************************事件************************************//
        public event MaintenanceAxesControlFormRequestXYMoveToAssignedPointDelegate MaintenanceAxesControlFormRequestXYMoveToAssignedPointEvent;//声明事件，轴控制手动界面请求XY移动至指定点位
        public event MaintenanceAxesControlFormRequestChangeConveyorWidthDelegate MaintenanceAxesControlFormRequestChangeConveyorWidthEvent;//声明事件，轴控制手动画面请求调节传送线宽度
        public event MaintenanceAxesControlFormRequestConveyorActionDelegate MaintenanceAxesControlFormRequestConveyorActionEvent;//声明事件，轴控制手动画面请求控制传送线
        //*************************外部可读写变量*******************************//

        //*************************公共静态变量*******************************//

        //*************************内部私有变量*******************************//
        Thread _statusRefreshThread = null;
        //
        //

        public MaintenanceAxesControlForm()
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
                    if (BaseForm._xyMoveToAssignedPointPositionModuleOffDlyT.STATUS)//如果主界面xy移动模块ON
                    {
                        btnMoveToTrack1AssignedDestinationPointPosition.BackColor = Color.LimeGreen;
                        btnMoveToTrack1AssignedDestinationPointPosition.ForeColor = Color.Black;
                        btnMoveToTrack2AssignedDestinationPointPosition.BackColor = Color.LimeGreen;
                        btnMoveToTrack2AssignedDestinationPointPosition.ForeColor = Color.Black;
                    }
                    else
                    {
                        btnMoveToTrack1AssignedDestinationPointPosition.BackColor = Color.Orange;
                        btnMoveToTrack1AssignedDestinationPointPosition.ForeColor = Color.White;
                        btnMoveToTrack2AssignedDestinationPointPosition.BackColor = Color.Orange;
                        btnMoveToTrack2AssignedDestinationPointPosition.ForeColor = Color.White;
                    }

                    if (BaseForm._conveyorChangeToAssignedWidthModuleOffDlyT.STATUS)//如果主界面传送线宽度调节模块ON
                    {
                        btnChangeConveyor1Width.BackColor = Color.LimeGreen;
                        btnChangeConveyor1Width.ForeColor = Color.Black;
                        btnChangeConveyor2Width.BackColor = Color.LimeGreen;
                        btnChangeConveyor2Width.ForeColor = Color.Black;
                        btnChangeAllConveyorWidth.BackColor = Color.LimeGreen;
                        btnChangeAllConveyorWidth.ForeColor = Color.Black;
                    }
                    else
                    {
                        btnChangeConveyor1Width.BackColor = Color.Orange;
                        btnChangeConveyor1Width.ForeColor = Color.White;
                        btnChangeConveyor2Width.BackColor = Color.Orange;
                        btnChangeConveyor2Width.ForeColor = Color.White;
                        btnChangeAllConveyorWidth.BackColor = Color.Orange;
                        btnChangeAllConveyorWidth.ForeColor = Color.White;
                    }

                    if (BaseForm._conveyor1ActionModuleActionFlag != 1)//如果传送线1动作模块ON
                    {
                        btnConveyor1Action.BackColor = Color.LimeGreen;
                        btnConveyor1Action.ForeColor = Color.Black;
                        btnConveyor1Action.Text = "传送线1启动";
                    }
                    else
                    {
                        btnConveyor1Action.BackColor = Color.Orange;
                        btnConveyor1Action.ForeColor = Color.White;
                        btnConveyor1Action.Text = "传送线1停止";
                    }

                    if (BaseForm._conveyor2ActionModuleActionFlag != 1)//如果传送线1动作模块ON
                    {
                        btnConveyor2Action.BackColor = Color.LimeGreen;
                        btnConveyor2Action.ForeColor = Color.Black;
                        btnConveyor2Action.Text = "传送线2启动";
                    }
                    else
                    {
                        btnConveyor2Action.BackColor = Color.Orange;
                        btnConveyor2Action.ForeColor = Color.White;
                        btnConveyor2Action.Text = "传送线2停止";
                    }
                })); ;

                Thread.Sleep(100);
            }
        }
        #endregion

        #region 虚拟键盘相关函数

        #region TextBox双击事件

        //双击按钮，打开数字软键盘，数据无正负限制，浮点数
        private void TextBoxDoubleClickEventToOpenNumberKeyboard(object sender, EventArgs e)
        {
            MyTool.TxtFileProcess.CreateLog("手动轴控制页面-打开字母软键盘！" + "----用户：" + BaseForm._operatorName,
            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

            SoftNumberKeyboard.LanguageFlag = 1;
            SoftNumberKeyboard numberKeyboard = new SoftNumberKeyboard();
            numberKeyboard.StartPosition = FormStartPosition.CenterScreen;
            numberKeyboard.PLUSMINUSBTHIDEFLAG = false;
            numberKeyboard.FULLSTOPBTHIDEFLAG = false;
            numberKeyboard.SOURCESTR = ((TextBox)sender).Text;
            numberKeyboard.SENDER = sender;
            numberKeyboard.SendInputDataIncludeObj += new SENDINPUTDATAINCLUDEOBJ(GetKeyboardInputData);//添加事件
            numberKeyboard.ShowDialog();
        }

        //双击按钮，打开数字软键盘，数据有正限制，浮点数，和整数
        private void TextBoxDoubleClickEventToOpenNumberKeyboardForPlusData(object sender, EventArgs e)
        {
            MyTool.TxtFileProcess.CreateLog("手动轴控制页面-打开数字软键盘！" + "----用户：" + BaseForm._operatorName,
            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

            SoftNumberKeyboard.LanguageFlag = 1;
            SoftNumberKeyboard numberKeyboard = new SoftNumberKeyboard();
            numberKeyboard.StartPosition = FormStartPosition.CenterScreen;
            if (((TextBox)sender).Name == "txtDestinationNumber")
            {
                numberKeyboard.PLUSMINUSBTHIDEFLAG = true;
                numberKeyboard.FULLSTOPBTHIDEFLAG = true;
            }
            else
            {
                numberKeyboard.PLUSMINUSBTHIDEFLAG = true;
                numberKeyboard.FULLSTOPBTHIDEFLAG = false;
            }
            numberKeyboard.SOURCESTR = ((TextBox)sender).Text;
            numberKeyboard.SENDER = sender;
            numberKeyboard.SendInputDataIncludeObj += new SENDINPUTDATAINCLUDEOBJ(GetKeyboardInputData);//添加事件
            numberKeyboard.ShowDialog();
        }

        #endregion

        //键盘按下OK之后获取数据
        private void GetKeyboardInputData(object Sender, string GetDataStr)
        {
            ((TextBox)Sender).Text = GetDataStr;//将键盘按钮输入的数据传递给密码输入框
            ((TextBox)Sender).Focus();
            ((TextBox)Sender).SelectionLength = 0;
        }

        #endregion

        #region 数据检查事件
        //Textbox数据普通检查函数，正数，浮点数，整数检查
        private void TextBoxDataPlusFloatOrIntCheckEvent(object sender, EventArgs e)
        {
            if (((TextBox)sender).Name == "txtDestinationNumber")
            {
                if (SettingsProductForm._editProductParameter._pointPositionDT != null)
                    ((TextBox)sender).Text = MyTool.DataProcessing.TextBoxDataCheck(((TextBox)sender).Text, 1, SettingsProductForm._editProductParameter._pointPositionDT.Rows.Count, true, true, false, false);
                else
                    ((TextBox)sender).Text = MyTool.DataProcessing.TextBoxDataCheck(((TextBox)sender).Text, 1, 1, true, true, false, false);
            }
            else
            {
                ((TextBox)sender).Text = MyTool.DataProcessing.TextBoxDataCheck(((TextBox)sender).Text, 0, 600, true, true, true, false);
            }
            ((TextBox)sender).SelectionStart = ((TextBox)sender).TextLength;
            ((TextBox)sender).SelectionLength = 0;
        }

        //Textbox数据最后一次检查函数，正数，浮点数，整数检查
        private void TextBoxDataPlusFloatOrIntCheckLeaveEvent(object sender, EventArgs e)
        {
            if (((TextBox)sender).Name == "txtDestinationNumber")
            {
                if (SettingsProductForm._editProductParameter._pointPositionDT != null)
                    ((TextBox)sender).Text = MyTool.DataProcessing.TextBoxDataCheck(((TextBox)sender).Text, 1, SettingsProductForm._editProductParameter._pointPositionDT.Rows.Count, true, true, false, true);
                else
                    ((TextBox)sender).Text = MyTool.DataProcessing.TextBoxDataCheck(((TextBox)sender).Text, 1, 1, true, true, false, true);

                MyTool.TxtFileProcess.CreateLog("手动轴控制页面-修改目标点位号为" + ((TextBox)sender).Text + "----用户：" + BaseForm._operatorName,
                BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);
            }
            else
            {
                ((TextBox)sender).Text = MyTool.DataProcessing.TextBoxDataCheck(((TextBox)sender).Text, 0, 600, true, true, true, true);

                MyTool.TxtFileProcess.CreateLog("手动轴控制页面-修改" + ((TextBox)sender).Name + "的数据为" + ((TextBox)sender).Text + "----用户：" + BaseForm._operatorName,
                BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);
            }
            ((TextBox)sender).SelectionStart = ((TextBox)sender).TextLength;
            ((TextBox)sender).SelectionLength = 0;
        }
        #endregion

        //点击移动至指定点位按钮
        private void btnMoveToAssignedDestinationPointPosition_Click(object sender, EventArgs e)
        {
            int destinationPointNumber = Convert.ToInt32(txtDestinationNumber.Text);
            int trackIndex = 0;

            if (((Button)sender).Name == "btnMoveToTrack2AssignedDestinationPointPosition")
            {
                trackIndex = 1;
            }

            MyTool.TxtFileProcess.CreateLog("手动轴控制页面-移动XYR至传送线" + (trackIndex + 1).ToString() + "的第" + destinationPointNumber.ToString() + "点" + "----用户：" + BaseForm._operatorName,
            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);


            if (!SettingsProductForm._editProductParameter._parameterLoadSuccessfulFlag)
            {
                MyTool.TxtFileProcess.CreateLog("手动轴控制页面-移动XYR至传送线" + (trackIndex + 1).ToString() + "的第" + destinationPointNumber.ToString() + "点失败，未能成功读取产品参数！" + "----用户：" + BaseForm._operatorName,
            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                MessageBox.Show("未能成功读取产品参数，无法控制XY轴移动至指定点位号！");
                return;
            }

            if (SettingsProductForm._editProductParameter._pointPositionDT == null)
            {
                MyTool.TxtFileProcess.CreateLog("手动轴控制页面-移动XYR至传送线" + (trackIndex + 1).ToString() + "的第" + destinationPointNumber.ToString() + "点失败，无产品点位坐标！" + "----用户：" + BaseForm._operatorName,
            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                MessageBox.Show("无产品点位坐标数据，无法控制XY轴移动至指定点位号！");
                return;
            }

            if (destinationPointNumber < 1)
            {
                MyTool.TxtFileProcess.CreateLog("手动轴控制页面-移动XYR至传送线" + (trackIndex + 1).ToString() + "的第" + destinationPointNumber.ToString() + "点失败，指定的点位号非法（小于1）！" + "----用户：" + BaseForm._operatorName,
                BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                MessageBox.Show("指定点位号码非法，无法控制XY轴移动至指定点位号！（指定点位号小于1）");
                return;
            }

            if (destinationPointNumber > SettingsProductForm._editProductParameter._pointPositionDT.Rows.Count)
            {
                MyTool.TxtFileProcess.CreateLog("手动轴控制页面-移动XYR至传送线" + (trackIndex + 1).ToString() + "的第" + destinationPointNumber.ToString() + "点失败，指定的点位号非法（大于最大点位个数"+
                   SettingsProductForm._editProductParameter._pointPositionDT.Rows.Count.ToString()+"）！" + "----用户：" + BaseForm._operatorName,
                BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                MessageBox.Show("指定点位号码非法，无法控制XY轴移动至指定点位号！（指定点位号大于点位数据表中的点位个数）");
                return;
            }
            if (MaintenanceAxesControlFormRequestXYMoveToAssignedPointEvent != null)
                MaintenanceAxesControlFormRequestXYMoveToAssignedPointEvent(destinationPointNumber,trackIndex);
        }

        //点击手动调节传送线宽度事件按钮
        private void ManualChangeConveyorWidthEvent(object sender, EventArgs e)
        {
            int conveyorControlFlag = 0;

            switch (((Button)sender).Name)
            {
                case "btnChangeConveyor1Width":
                    MyTool.TxtFileProcess.CreateLog("手动轴控制页面-调节传送线1的宽度" + "----用户：" + BaseForm._operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);
                    conveyorControlFlag = 0;
                    break;
                case "btnChangeConveyor2Width":
                    MyTool.TxtFileProcess.CreateLog("手动轴控制页面-调节传送线2的宽度" + "----用户：" + BaseForm._operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);
                    conveyorControlFlag = 1;
                    break;
                case "btnChangeAllConveyorWidth":
                    MyTool.TxtFileProcess.CreateLog("手动轴控制页面-同时调节传送线1和传送线2的宽度" + "----用户：" + BaseForm._operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);
                    conveyorControlFlag = 2;
                    break;
                default:
                    return;

            }

            if (!SettingsProductForm._editProductParameter._parameterLoadSuccessfulFlag)
            {
                MyTool.TxtFileProcess.CreateLog("手动轴控制页面-调节传送线的宽度失败，未能成功读取产品参数！" + "----用户：" + BaseForm._operatorName,
                BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                MessageBox.Show("未能成功读取产品参数，无法调节传送线宽度！");
                return;
            }

            if (MaintenanceAxesControlFormRequestChangeConveyorWidthEvent != null)
                MaintenanceAxesControlFormRequestChangeConveyorWidthEvent(conveyorControlFlag);
        }

        //手动控制传送线事件
        private void ManualControlConveyorEvent(object sender, EventArgs e)
        {
            MyTool.TxtFileProcess.CreateLog("手动轴控制页面-控制传送线动作！" + "----用户：" + BaseForm._operatorName,
            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

            float actionSpeed = -1;
            int conveyorIndex = 0;//控制的传送线索引，0-传送线1，1-传送线2
            switch (((Button)sender).Name)
            { 
                case "btnConveyor1Action":
                    conveyorIndex = 0;
                    actionSpeed = Convert.ToSingle(txtConveyor1ActionSpeed.Text);
                    break;
                case "btnConveyor2Action":
                    conveyorIndex = 1;
                    actionSpeed = Convert.ToSingle(txtConveyor2ActionSpeed.Text);
                    break;
                default:
                    return;
            }
            if (conveyorIndex == 0 && BaseForm._conveyor1ActionModuleActionFlag == 1)//如果为传送线1
            {
                actionSpeed = -1;
            }
            else if (conveyorIndex == 1 && BaseForm._conveyor2ActionModuleActionFlag == 1)//如果为传送线2
            {
                actionSpeed = -1;
            }

            if (MaintenanceAxesControlFormRequestConveyorActionEvent != null)
            {
                MaintenanceAxesControlFormRequestConveyorActionEvent(actionSpeed,conveyorIndex);
            }
        }

        private void MaintenanceAxesControlForm_Load(object sender, EventArgs e)
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

        private void MaintenanceAxesControlForm_VisibleChanged(object sender, EventArgs e)
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
