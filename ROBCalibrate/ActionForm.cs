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
using System.Runtime.InteropServices;//用于判断文件是否处于打开被占用状态
using System.IO;
using FX5UPLC;
using System.Diagnostics;//用于stopwatch
//using CSVFile;
using HalconDotNet;
using HalconMVTec;
//using MyTool;
using System.Xml;
//using XMLFile;
using UserManager;
using SocketCommunicationClass;
//using MatchModel.Halcon;
using ChoiceTech.Halcon.Control;

namespace ROBCalibrate
{
    public partial class ActionForm : Form
    {
        //窗体声明
        public static HWindow_Final hw_S1;
        public static HWindow_Final hw_S2;
        public static HWindow_Final hw_S3;

        CalibrateForm newCalibrateForm;//参数设定窗口
        ProductSelectForm _productSelectFormVariate = null;//产品选择窗口
        UserCreateAndLogTotalForm _userManagerVariate = null;

        public static int _accessLevel;
        public static string _operatorName;//操作员名字
        public static int _logFileMaxSize = 10240;//单个Log文件大小
        public static string _deviceControlLogFolderPath = Directory.GetCurrentDirectory() + "//LOG//ControlLog";//Log文件保存地址
        public static bool _autoModeFlag;

        public static HalconMVTec.HalconCameraControl _CameraCs_S1 = null;//相机类
        public static HalconMVTec.HalconCameraControl _CameraCs_S2 = null;//相机类
        public static HalconMVTec.HalconCameraControl _CameraCs_S3 = null;//相机类

        Thread _cameraImageGrabAndDispThread = null;//相机获取并显示图像线程

        public static bool _CameraVideo_S1 = false;//相机实时显示标志
        public static bool _CameraVideo_S2 = false;//相机实时显示标志
        public static bool _CameraVideo_S3 = false;//相机实时显示标志
        bool _CameraGrab_S1 = false;//相机抓取图像标志
        bool _CameraGrab_S2 = false;//相机抓取图像标志
        bool _CameraGrab_S3 = false;//相机抓取图像标志

        //ADD 20221017 拍照出错标志
        bool _CameraGrabFail_S1 = false;
        bool _CameraGrabFail_S2 = false;
        bool _CameraGrabFail_S3 = false;
        //ADD 20221017 取料位置计算处理出错标志
        bool _PickupPositionProcessFail_S1 = false;
        bool _PickupPositionProcessFail_S2 = false;
        bool _PickupPositionProcessFail_S3 = false;

        bool ROBConnectOKNeedLogFlag;
        bool ROBConnectNGNeedLogFlag;

        public static bool _socketServerStartOKAndConnectROBOKFlag = false;//服务器开启OK并且连接ROB OK标志

        //MinimumEnclosingRectangle newMinimumEnclosingRectangle = new MinimumEnclosingRectangle();

        #region  构造函数
        public ActionForm()
        {
            InitializeComponent();

            //用户登录编辑页面
            MyTool.TxtFileProcess.CreateLog("实例化用户管理页面！", _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
            if (_userManagerVariate != null)
            {
                _userManagerVariate.Dispose();
                _userManagerVariate = null;
            }
            _userManagerVariate = new UserCreateAndLogTotalForm();
            _userManagerVariate.StartPosition = FormStartPosition.CenterParent;
            _accessLevel = _userManagerVariate._accessLevel;
            _operatorName = _userManagerVariate._currentUserName;
            if (_accessLevel == 0)
            {
                lblAccessLevel.Text = "未登录";
                lblAccessLevel.ForeColor = Color.Black;
            }
            else if (_accessLevel == 100)
            {
                lblAccessLevel.Text = "操作员";
                lblAccessLevel.ForeColor = Color.LimeGreen;
            }
            else if (_accessLevel == 101)
            {
                lblAccessLevel.Text = "工程师";
                lblAccessLevel.ForeColor = Color.Orange;
            }
            else if (_accessLevel == 102)
            {
                lblAccessLevel.Text = "厂家";
                lblAccessLevel.ForeColor = Color.Red;
            }
            lblUserName.Text = _operatorName;
            _userManagerVariate.LogInFinishEvent += UserFormUserLogInFinishEventFunc;
            _userManagerVariate.requestCloseEvent += UserFormRequestCloseEventFunc;

            //绑定委托事件,当添加了LOG信息至LOG文件后,触发日志显示事件
            MyTool.TxtFileProcess.AddMessageToLOGFileFinishEvent += AddMessageToLOGFileFinishEventFunc;

            //初始化窗体

            HOperatorSet.GenImageConst(out HObject EmptyImage, "byte", 5000, 5000);
            hw_S1 = new HWindow_Final();
            panel1.Controls.Add(hw_S1);
            hw_S1.Dock = DockStyle.Fill;
            hw_S1.HobjectToHimage(EmptyImage);

            hw_S2 = new HWindow_Final();
            panel2.Controls.Add(hw_S2);
            hw_S2.Dock = DockStyle.Fill;
            hw_S2.HobjectToHimage(EmptyImage);

            hw_S3 = new HWindow_Final();
            panel3.Controls.Add(hw_S3);
            hw_S3.Dock = DockStyle.Fill;
            hw_S3.HobjectToHimage(EmptyImage);

            this.Width = (int)(Screen.PrimaryScreen.WorkingArea.Width * 1.0);
            this.Height = (int)(Screen.PrimaryScreen.WorkingArea.Height * 1.0);
            this.StartPosition = FormStartPosition.CenterScreen;

        }
        #endregion

        #region ActionForm_Load
        private void ActionForm_Load(object sender, EventArgs e)
        {
            _CameraCs_S1 = new HalconCameraControl();
            _CameraCs_S2 = new HalconCameraControl();
            _CameraCs_S3 = new HalconCameraControl();
            if (_cameraImageGrabAndDispThread != null)
            {
                _cameraImageGrabAndDispThread.Abort();
                _cameraImageGrabAndDispThread = null;
            }
            _cameraImageGrabAndDispThread = new Thread(CameraImageGrabAndDispThreadFunc);
            _cameraImageGrabAndDispThread.IsBackground = true;
            _cameraImageGrabAndDispThread.Start();

            newCalibrateForm = new CalibrateForm();
            newCalibrateForm.Visible = false;
            newCalibrateForm.LoadingDataFromXmlFile();//最基本参数的Setting.xml文件读取
            newCalibrateForm.LoadingVariateParameterFromXmlFile();//最基本参数的Current.rcpActuals文件读取
            newCalibrateForm.MaintenanceCameraFormRequestOpenOrCloseCameraEvent += MaintenanceCameraFormRequestOpenOrCloseCameraEventFunc;
            newCalibrateForm.MaintenanceCameraFormRequestGrabImageAndDispEvent += MaintenanceCameraFormRequestGrabImageAndDispEventFunc;
            newCalibrateForm.MaintenanceCameraFormRequestGrabImageAndDispVideoEvent += MaintenanceCameraFormRequestGrabImageAndDispVideoEventFunc;
            newCalibrateForm.MaintenanceCameraFormRequestSaveImageEvent += MaintenanceCameraFormRequestSaveImageEventFunc;

            lblActualProductSelect.Text = CalibrateForm._currentProductName;//更新显示主画面的当前产品

            //实例化ROB交互程序
            newSocketServerForROB = new SocketServer("192.168.0.2", 1000);

            //ADD 202210131822
            if (newSocketServerForROB._socketServerStartOKFlag)
                MyTool.TxtFileProcess.CreateLog("服务器端成功开启" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
            else
                MyTool.TxtFileProcess.CreateLog("服务器端开启失败!" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);

            ROBDataExchangeThread = new Thread(ROBDataExchange);
            ROBDataExchangeThread.Start();
            ROBDataExchangeThread.IsBackground = true;
            ROBDataExchangeThread.Priority = ThreadPriority.Highest;

        }
        #endregion

        #region  产品切换相关函数

        //点击产品切换按钮
        private void ProductChangeEvent(object sender, EventArgs e)
        {
            MyTool.TxtFileProcess.CreateLog("主界面-请求更换产品！" + "----用户：" + _operatorName,
            _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

            if (_autoModeFlag)
            {
                MyTool.TxtFileProcess.CreateLog("主界面-请求更换产品失败，设备当前处于自动模式！" + "----用户：" + _operatorName,
                _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
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
            }));

            #endregion
        }

        //获取弹出的产品选择界面选择的产品名称以及选择结果事件
        private void GetProductSelectResult(string selectProductName, bool changeProductFlag, int changeIndex)
        {
            if (changeProductFlag)
            {
                CalibrateForm._currentProductName = selectProductName;
                lblActualProductSelect.Text = selectProductName;
                newCalibrateForm.lblCurrentProduct.Text = selectProductName;
                newCalibrateForm.SavingVariateParameterToXmlFile();//更新保存当前产品名
            }
            else
            {
                MyTool.TxtFileProcess.CreateLog("主界面-放弃产品更换！" + "----用户：" + _operatorName,
                _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                MessageBox.Show("放弃产品切换！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            if (_productSelectFormVariate != null)
            {
                _productSelectFormVariate.Dispose();
                _productSelectFormVariate = null;
            }
        }

        #endregion

        #region 请求进入参数设定画面按钮事件
        private void picGotoSettingForm_Click(object sender, EventArgs e)
        {
            MyTool.TxtFileProcess.CreateLog("主界面-请求进入参数设定画面！" + "----用户：" + _operatorName,
            _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

            if (newCalibrateForm != null)
            {
                newCalibrateForm.Visible = true;
                newCalibrateForm.Focus();
            }
        }
        #endregion

        #region 用户登录等
        /// <summary>
        /// lblAccessLevel_Click:点击权限按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lblAccessLevel_Click(object sender, EventArgs e)
        {
            MyTool.TxtFileProcess.CreateLog("打开用户登录及编辑页面！" + "----用户：" + _operatorName,
            _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

            _userManagerVariate.StartPosition = FormStartPosition.CenterScreen;
            _userManagerVariate.Show();
            _userManagerVariate.Focus();
        }

        /// <summary>
        /// UserFormUserLogInFinishEventFunc:用户界面登录完成事件
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="accessLevel"></param>
        private void UserFormUserLogInFinishEventFunc(int accessLevel, string userName, bool hideFormFlag)
        {
            MyTool.TxtFileProcess.CreateLog("切换用户至" + userName + ",权限等级为" + accessLevel.ToString() + "----用户：" + _operatorName,
            _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

            _accessLevel = accessLevel;
            _operatorName = userName;
            if (_accessLevel == 0)
            {
                lblAccessLevel.Text = "未登录";
                lblAccessLevel.ForeColor = Color.Black;
            }
            else if (_accessLevel == 100)
            {
                lblAccessLevel.Text = "操作员";
                lblAccessLevel.ForeColor = Color.LimeGreen;
            }
            else if (_accessLevel == 101)
            {
                lblAccessLevel.Text = "工程师";
                lblAccessLevel.ForeColor = Color.Orange;
            }
            else if (_accessLevel == 102)
            {
                lblAccessLevel.Text = "厂家";
                lblAccessLevel.ForeColor = Color.Red;
            }
            lblUserName.Text = _operatorName;

            if (hideFormFlag)
                _userManagerVariate.Hide();
        }

        /// <summary>
        /// UserFormRequestCloseEventFunc:用户界面请求关闭事件
        /// </summary>
        private void UserFormRequestCloseEventFunc()
        {
            MyTool.TxtFileProcess.CreateLog("关闭用户登录编辑页面！" + "----用户：" + _operatorName,
            _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
            _userManagerVariate.Hide();
        }

        #endregion

        #region 日志显示事件
        /// <summary>
        /// AddMessageToLOGFileFinishEventFunc:添加信息至LOG文件完成事件函数
        /// </summary>
        /// <param name="addMsg">string:需要添加的内容</param>
        /// <param name="logFilePath">string:用于辨别属于什么类型的LOG</param>
        private void AddMessageToLOGFileFinishEventFunc(string addMsg, string logFilePath)
        {
            if (logFilePath == _deviceControlLogFolderPath)
            {
                if (!this.lvDeviceControlLogDisp.InvokeRequired)//如果没有跨线程访问
                {
                    ListViewItem lstItem = new ListViewItem(string.Format("{0:yyyy/MM/dd HH:mm:ss}", DateTime.Now) + "---->" + addMsg);
                    this.lvDeviceControlLogDisp.Items.Insert(0, lstItem);//保证最新的显示在第一条
                    //只保留最后300条记录
                    if (lvDeviceControlLogDisp.Items.Count > 300)
                    {
                        lvDeviceControlLogDisp.Items.RemoveAt(300);
                    }
                }
                else
                {
                    this.lvDeviceControlLogDisp.Invoke(new Action(() =>
                    {
                        ListViewItem lstItem = new ListViewItem();
                        lstItem.Text = string.Format("{0:yyyy/MM/dd HH:mm:ss}", DateTime.Now) + "---->" + addMsg;
                        this.lvDeviceControlLogDisp.Items.Insert(0, lstItem);//保证最新的显示在第一条
                                                                             //只保留最后300条记录
                        if (lvDeviceControlLogDisp.Items.Count > 300)
                        {
                            lvDeviceControlLogDisp.Items.RemoveAt(300);
                        }
                    })); ;
                }
            }
            else if (false)//logFilePath == BaseForm._mesCommunicationLogFolderPath)
            {
                if (!this.lvMESLogDisp.InvokeRequired)//如果没有跨线程访问
                {
                    ListViewItem lstItem = new ListViewItem(string.Format("{0:yyyy/MM/dd HH:mm:ss}", DateTime.Now) + "---->" + addMsg);
                    this.lvMESLogDisp.Items.Insert(0, lstItem);//保证最新的显示在第一条
                    //只保留最后300条记录
                    if (lvMESLogDisp.Items.Count > 300)
                    {
                        lvMESLogDisp.Items.RemoveAt(300);
                    }
                }
                else
                {
                    this.lvMESLogDisp.Invoke(new Action(() =>
                    {
                        ListViewItem lstItem = new ListViewItem();
                        lstItem.Text = string.Format("{0:yyyy/MM/dd HH:mm:ss}", DateTime.Now) + "---->" + addMsg;
                        this.lvMESLogDisp.Items.Insert(0, lstItem);//保证最新的显示在第一条
                                                                   //只保留最后300条记录
                        if (lvMESLogDisp.Items.Count > 300)
                        {
                            lvMESLogDisp.Items.RemoveAt(300);
                        }
                    })); ;
                }
            }
            else if (false)//logFilePath == Directory.GetCurrentDirectory() + "\\Alarm Record")//如果为报警记录路径
            {
                if (!this.lvAlarmDisp.InvokeRequired)//如果没有跨线程访问
                {
                    ListViewItem lstItem = new ListViewItem(addMsg);
                    this.lvAlarmDisp.Items.Insert(0, lstItem);//保证最新的显示在第一条
                    //只保留最后300条记录
                    if (lvAlarmDisp.Items.Count > 300)
                    {
                        lvAlarmDisp.Items.RemoveAt(300);
                    }
                }
                else
                {
                    this.lvAlarmDisp.Invoke(new Action(() =>
                    {
                        ListViewItem lstItem = new ListViewItem();
                        lstItem.Text = addMsg;
                        this.lvAlarmDisp.Items.Insert(0, lstItem);//保证最新的显示在第一条
                        //只保留最后300条记录
                        if (lvAlarmDisp.Items.Count > 300)
                        {
                            lvAlarmDisp.Items.RemoveAt(300);
                        }
                    })); ;
                }
            }
        }
        #endregion

        #region ROB通讯相关
        SocketServer newSocketServerForROB = null;
        //bool ROBConnectSatus = false;//ROB通讯连接状态标志，false-通讯异常，true-正常通讯
        public static bool ROBPCCommunicationStatus = false;//PLC与PC通讯状态
        Thread ROBDataExchangeThread = null;//与ROB数据交互线程
        private volatile bool ROBDataExchangeThreadStopFlag = false;//数据交互线程停止FLAG

        private bool ROBRequestFlag_S1 = false;//ROB请求拍照
        private bool ROBRequestFlag_S2 = false;//ROB请求拍照
        private bool ROBRequestFlag_S3 = false;//ROB请求拍照

        private bool newPickupPositionOKFlag_S1 = false;//PC已拍照并且计算好ROB的取料位置
        private bool newPickupPositionOKFlag_S2 = false;//PC已拍照并且计算好ROB的取料位置
        private bool newPickupPositionOKFlag_S3 = false;//PC已拍照并且计算好ROB的取料位置

        private bool classificationFinishFlag_S3 = false;//分类完成

        public static bool _autoModeCameraGrabOKFlag_S1;//自动时拍照完成
        public static bool _autoModeCameraGrabOKFlag_S2;//自动时拍照完成
        public static bool _autoModeCameraGrabOKFlag_S3;//自动时拍照完成

        public static float ROB_XCurrentPosition = 0.0f;//ROB_X当前位置
        public static float ROB_YCurrentPosition = 0.0f;//ROB_Y当前位置
        public static float ROB_ZCurrentPosition = 0.0f;//ROB_Z当前位置
        public static float ROB_RXCurrentPosition = 0.0f;//ROB_RX当前位置
        public static float ROB_RWCurrentPosition = 0.0f;//ROB_RW当前位置
        public static float ROB_RZCurrentPosition = 0.0f;//ROB_RZ当前位置
        #endregion

        #region ROB数据交互主程序
        private void ROBDataExchange()
        {
            while (!ROBDataExchangeThreadStopFlag)
            {
                _socketServerStartOKAndConnectROBOKFlag = newSocketServerForROB._socketServerStartOKFlag && ROBPCCommunicationStatus;

                #region send
                if (newSocketServerForROB._socketServerStartOKFlag)
                {
                    for (int i = newSocketServerForROB._clientAdress.Count - 1; i >= 0; i--)//ROB多次断线再连,PC端确保只对最新连接的客户端发送信息
                    {
                        string[] ip = newSocketServerForROB._clientAdress[i].Split(':');
                        if (ip[0] == "192.168.0.90")
                        {
                            //if (!newSocketServerForROB.sendMessage1(newSocketServerForROB._clientAdress[i], "POS"))
                            //{
                            //    MyTool.TxtFileProcess.CreateLog("IPC1发送\"POS\"到ROB1失败!" + "----用户：" + _operatorName,
                            //    _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                            //    MessageBox.Show("IPC1发送\"POS\"到ROB1失败!");
                            //}
                            //else
                            ROBPCCommunicationStatus = true;

                            //Station1
                            if (ROBPCCommunicationStatus && (ROBRequestFlag_S1 || _btnDebugSendFlag_S1) && newPickupPositionOKFlag_S1)
                            {
                                //发送取料位置给ROB
                                string _pos = CalibrateForm.ROB_XPickupCurrentPosition_S1.ToString() + "," +
                                              CalibrateForm.ROB_YPickupCurrentPosition_S1.ToString() + "," +
                                              CalibrateForm.ROB_ZPickupCurrentPosition_S1.ToString() + "," +
                                              CalibrateForm.ROB_RXPickupCurrentPosition_S1.ToString() + "," +
                                              CalibrateForm.ROB_RWPickupCurrentPosition_S1.ToString() + "," +
                                              CalibrateForm.ROB_RZPickupCurrentPosition_S1.ToString();
                                if (!newSocketServerForROB.sendMessage1(newSocketServerForROB._clientAdress[i], "TRI1OQ," + _pos))
                                {
                                    ROBRequestFlag_S1 = false;
                                    _btnDebugSendFlag_S1 = false;
                                    MyTool.TxtFileProcess.CreateLog("IPC1发送TRI1OQ," + _pos + "到ROB1:" + newSocketServerForROB._clientAdress[i].ToString() + "失败!" + "----用户：" + _operatorName,
                                    _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                                    //MessageBox.Show("IPC1发送TRI1OQ, " + _pos + "到ROB1失败!");
                                }
                                else
                                {
                                    newPickupPositionOKFlag_S1 = false;
                                    ROBRequestFlag_S1 = false;
                                    _btnDebugSendFlag_S1 = false;
                                    MyTool.TxtFileProcess.CreateLog("IPC1发送TRI1OQ," + _pos + "到ROB1:" + newSocketServerForROB._clientAdress[i].ToString() + "成功!" + "----用户：" + _operatorName,
                                    _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                                }
                            }

                            //Station2
                            if (ROBPCCommunicationStatus && (ROBRequestFlag_S2 || _btnDebugSendFlag_S2) && newPickupPositionOKFlag_S2)
                            {
                                //发送取料位置给ROB
                                string _pos = CalibrateForm.ROB_XPickupCurrentPosition_S2.ToString() + "," +
                                              CalibrateForm.ROB_YPickupCurrentPosition_S2.ToString() + "," +
                                              CalibrateForm.ROB_ZPickupCurrentPosition_S2.ToString() + "," +
                                              CalibrateForm.ROB_RXPickupCurrentPosition_S2.ToString() + "," +
                                              CalibrateForm.ROB_RWPickupCurrentPosition_S2.ToString() + "," +
                                              CalibrateForm.ROB_RZPickupCurrentPosition_S2.ToString();
                                if (!newSocketServerForROB.sendMessage1(newSocketServerForROB._clientAdress[i], "TRI2OQ," + _pos))
                                {
                                    ROBRequestFlag_S2 = false;
                                    _btnDebugSendFlag_S2 = false;
                                    MyTool.TxtFileProcess.CreateLog("IPC1发送TRI2OQ," + _pos + "到ROB1:" + newSocketServerForROB._clientAdress[i].ToString() + "失败!" + "----用户：" + _operatorName,
                                    _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                                    //MessageBox.Show("IPC1发送TRI2OQ, " + _pos + "到ROB1失败!");
                                }
                                else
                                {
                                    newPickupPositionOKFlag_S2 = false;
                                    ROBRequestFlag_S2 = false;
                                    _btnDebugSendFlag_S2 = false;
                                    MyTool.TxtFileProcess.CreateLog("IPC1发送TRI2OQ," + _pos + "到ROB1:" + newSocketServerForROB._clientAdress[i].ToString() + "成功!" + "----用户：" + _operatorName,
                                    _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                                }
                            }

                            //Station3
                            if (ROBPCCommunicationStatus && (ROBRequestFlag_S3 || _btnDebugSendFlag_S3) && newPickupPositionOKFlag_S3 && classificationFinishFlag_S3 && newCalibrateForm._classificationResult_S3 == 1)
                            {
                                //发送取料位置给ROB
                                string _pos = CalibrateForm.ROB_XPickupCurrentPosition_S3.ToString() + "," +
                                              CalibrateForm.ROB_YPickupCurrentPosition_S3.ToString() + "," +
                                              CalibrateForm.ROB_ZPickupCurrentPosition_S3.ToString() + "," +
                                              CalibrateForm.ROB_RXPickupCurrentPosition_S3.ToString() + "," +
                                              CalibrateForm.ROB_RWPickupCurrentPosition_S3.ToString() + "," +
                                              CalibrateForm.ROB_RZPickupCurrentPosition_S3.ToString();
                                if (!newSocketServerForROB.sendMessage1(newSocketServerForROB._clientAdress[i], "TRI3OQ," + _pos))
                                {
                                    ROBRequestFlag_S3 = false;
                                    _btnDebugSendFlag_S3 = false;
                                    MyTool.TxtFileProcess.CreateLog("IPC1发送TRI3OQ," + _pos + "到ROB1:" + newSocketServerForROB._clientAdress[i].ToString() + "失败!" + "----用户：" + _operatorName,
                                    _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                                    //MessageBox.Show("IPC1发送TRI3OQ, " + _pos + "到ROB1失败!");
                                }
                                else
                                {
                                    classificationFinishFlag_S3 = false;
                                    newPickupPositionOKFlag_S3 = false;
                                    ROBRequestFlag_S3 = false;
                                    _btnDebugSendFlag_S3 = false;
                                    MyTool.TxtFileProcess.CreateLog("IPC1发送TRI3OQ," + _pos + "到ROB1:" + newSocketServerForROB._clientAdress[i].ToString() + "成功!" + "----用户：" + _operatorName,
                                    _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                                }
                            }

                            if (ROBPCCommunicationStatus && (ROBRequestFlag_S3 || _btnDebugSendFlag_S3) && classificationFinishFlag_S3 && newCalibrateForm._classificationResult_S3 == 2)
                            {
                                //发送TRI3CQ给ROB
                                if (!newSocketServerForROB.sendMessage1(newSocketServerForROB._clientAdress[i], "TRI3CQ,"))
                                {
                                    ROBRequestFlag_S3 = false;
                                    _btnDebugSendFlag_S3 = false;
                                    MyTool.TxtFileProcess.CreateLog("IPC1发送TRI3CQ," + "到ROB1:" + newSocketServerForROB._clientAdress[i].ToString() + "失败!" + "----用户：" + _operatorName,
                                    _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                                    //MessageBox.Show("IPC1发送TRI3CQ, " + "到ROB1失败!");
                                }
                                else
                                {
                                    classificationFinishFlag_S3 = false;
                                    newPickupPositionOKFlag_S3 = false;
                                    ROBRequestFlag_S3 = false;
                                    _btnDebugSendFlag_S3 = false;
                                    MyTool.TxtFileProcess.CreateLog("IPC1发送TRI3CQ," + "到ROB1:" + newSocketServerForROB._clientAdress[i].ToString() + "成功!" + "----用户：" + _operatorName,
                                    _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                                }
                            }

                            if (ROBPCCommunicationStatus && (ROBRequestFlag_S3 || _btnDebugSendFlag_S3) && classificationFinishFlag_S3 && newCalibrateForm._classificationResult_S3 == 3)
                            {
                                //发送TRI3NQ给ROB
                                if (!newSocketServerForROB.sendMessage1(newSocketServerForROB._clientAdress[i], "TRI3NQ,"))
                                {
                                    ROBRequestFlag_S3 = false;
                                    _btnDebugSendFlag_S3 = false;
                                    MyTool.TxtFileProcess.CreateLog("IPC1发送TRI3NQ," + "到ROB1:" + newSocketServerForROB._clientAdress[i].ToString() + "失败!" + "----用户：" + _operatorName,
                                    _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                                    //MessageBox.Show("IPC1发送TRI3NQ, " + "到ROB1失败!");
                                }
                                else
                                {
                                    classificationFinishFlag_S3 = false;
                                    newPickupPositionOKFlag_S3 = false;
                                    ROBRequestFlag_S3 = false;
                                    _btnDebugSendFlag_S3 = false;
                                    MyTool.TxtFileProcess.CreateLog("IPC1发送TRI3NQ," + "到ROB1:" + newSocketServerForROB._clientAdress[i].ToString() + "成功!" + "----用户：" + _operatorName,
                                    _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                                }
                            }

                        }
                    }

                    //Station1
                    if (newCalibrateForm.manualSendPositionToRobRequest_S1)
                    {
                        bool _findRobConnectFlag = false;
                        for (int i = newSocketServerForROB._clientAdress.Count - 1; i >= 0; i--)
                        {
                            string[] ip = newSocketServerForROB._clientAdress[i].Split(':');
                            if (ip[0] == "192.168.0.90")
                            {
                                _findRobConnectFlag = true;
                                if (ROBPCCommunicationStatus)
                                {
                                    //发送取料位置给ROB
                                    string _pos = CalibrateForm.ROB_XPickupCurrentPosition_S1.ToString() + "," +
                                                  CalibrateForm.ROB_YPickupCurrentPosition_S1.ToString() + "," +
                                                  CalibrateForm.ROB_ZPickupCurrentPosition_S1.ToString() + "," +
                                                  CalibrateForm.ROB_RXPickupCurrentPosition_S1.ToString() + "," +
                                                  CalibrateForm.ROB_RWPickupCurrentPosition_S1.ToString() + "," +
                                                  CalibrateForm.ROB_RZPickupCurrentPosition_S1.ToString();
                                    if (!newSocketServerForROB.sendMessage1(newSocketServerForROB._clientAdress[i], "TRI1OQ," + _pos))
                                    {
                                        newCalibrateForm.manualSendPositionToRobRequest_S1 = false;
                                        MyTool.TxtFileProcess.CreateLog("IPC1发送TRI1OQ," + _pos + "到ROB1:" + newSocketServerForROB._clientAdress[i].ToString() + "失败!" + "----用户：" + _operatorName,
                                        _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                                        //MessageBox.Show("IPC1发送TRI1OQ, " + _pos + "到ROB1失败!");
                                    }
                                    else
                                    {
                                        MyTool.TxtFileProcess.CreateLog("IPC1发送TRI1OQ," + _pos + "到ROB1:" + newSocketServerForROB._clientAdress[i].ToString() + "成功!" + "----用户：" + _operatorName,
                                        _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                                    }
                                }
                            }
                        }
                        if (!_findRobConnectFlag)
                        {
                            MyTool.TxtFileProcess.CreateLog("没有与ROB1连接!" + "----用户：" + _operatorName,
                            _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                            //MessageBox.Show("没有与ROB1连接!");
                        }
                        newCalibrateForm.manualSendPositionToRobRequest_S1 = false;
                    }

                    //Station2
                    if (newCalibrateForm.manualSendPositionToRobRequest_S2)
                    {
                        bool _findRobConnectFlag = false;
                        for (int i = newSocketServerForROB._clientAdress.Count - 1; i >= 0; i--)
                        {
                            string[] ip = newSocketServerForROB._clientAdress[i].Split(':');
                            if (ip[0] == "192.168.0.90")
                            {
                                _findRobConnectFlag = true;
                                if (ROBPCCommunicationStatus)
                                {
                                    //发送取料位置给ROB
                                    string _pos = CalibrateForm.ROB_XPickupCurrentPosition_S2.ToString() + "," +
                                                  CalibrateForm.ROB_YPickupCurrentPosition_S2.ToString() + "," +
                                                  CalibrateForm.ROB_ZPickupCurrentPosition_S2.ToString() + "," +
                                                  CalibrateForm.ROB_RXPickupCurrentPosition_S2.ToString() + "," +
                                                  CalibrateForm.ROB_RWPickupCurrentPosition_S2.ToString() + "," +
                                                  CalibrateForm.ROB_RZPickupCurrentPosition_S2.ToString();
                                    if (!newSocketServerForROB.sendMessage1(newSocketServerForROB._clientAdress[i], "TRI2OQ," + _pos))
                                    {
                                        newCalibrateForm.manualSendPositionToRobRequest_S2 = false;
                                        MyTool.TxtFileProcess.CreateLog("IPC1发送TRI2OQ," + _pos + "到ROB1:" + newSocketServerForROB._clientAdress[i].ToString() + "失败!" + "----用户：" + _operatorName,
                                        _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                                        //MessageBox.Show("IPC1发送TRI2OQ, " + _pos + "到ROB1失败!");
                                    }
                                    else
                                    {
                                        MyTool.TxtFileProcess.CreateLog("IPC1发送TRI2OQ," + _pos + "到ROB1:" + newSocketServerForROB._clientAdress[i].ToString() + "成功!" + "----用户：" + _operatorName,
                                        _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                                    }
                                }
                            }
                        }
                        if (!_findRobConnectFlag)
                        {
                            MyTool.TxtFileProcess.CreateLog("没有与ROB1连接!" + "----用户：" + _operatorName,
                            _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                            //MessageBox.Show("没有与ROB1连接!");
                        }
                        newCalibrateForm.manualSendPositionToRobRequest_S2 = false;
                    }

                    //Station3
                    if (newCalibrateForm.manualSendPositionToRobRequest_S3)
                    {
                        bool _findRobConnectFlag = false;
                        for (int i = newSocketServerForROB._clientAdress.Count - 1; i >= 0; i--)
                        {
                            string[] ip = newSocketServerForROB._clientAdress[i].Split(':');
                            if (ip[0] == "192.168.0.90")
                            {
                                _findRobConnectFlag = true;
                                if (ROBPCCommunicationStatus)
                                {
                                    //发送取料位置给ROB
                                    string _pos = CalibrateForm.ROB_XPickupCurrentPosition_S3.ToString() + "," +
                                                  CalibrateForm.ROB_YPickupCurrentPosition_S3.ToString() + "," +
                                                  CalibrateForm.ROB_ZPickupCurrentPosition_S3.ToString() + "," +
                                                  CalibrateForm.ROB_RXPickupCurrentPosition_S3.ToString() + "," +
                                                  CalibrateForm.ROB_RWPickupCurrentPosition_S3.ToString() + "," +
                                                  CalibrateForm.ROB_RZPickupCurrentPosition_S3.ToString();
                                    if (!newSocketServerForROB.sendMessage1(newSocketServerForROB._clientAdress[i], "TRI3OQ," + _pos))
                                    {
                                        newCalibrateForm.manualSendPositionToRobRequest_S3 = false;
                                        MyTool.TxtFileProcess.CreateLog("IPC1发送TRI3OQ," + _pos + "到ROB1:" + newSocketServerForROB._clientAdress[i].ToString() + "失败!" + "----用户：" + _operatorName,
                                        _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                                        //MessageBox.Show("IPC1发送TRI3OQ, " + _pos + "到ROB1失败!");
                                    }
                                    else
                                    {
                                        MyTool.TxtFileProcess.CreateLog("IPC1发送TRI3OQ," + _pos + "到ROB1:" + newSocketServerForROB._clientAdress[i].ToString() + "成功!" + "----用户：" + _operatorName,
                                        _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                                    }
                                }
                            }
                        }
                        if (!_findRobConnectFlag)
                        {
                            MyTool.TxtFileProcess.CreateLog("没有与ROB1连接!" + "----用户：" + _operatorName,
                            _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                            //MessageBox.Show("没有与ROB1连接!");
                        }
                        newCalibrateForm.manualSendPositionToRobRequest_S3 = false;
                    }

                    //读取ROB的当前位置请求----暂时没有用到
                    if (newCalibrateForm.manualGetPositionToRobRequest)
                    {
                        bool _findRobConnectFlag = false;
                        for (int i = newSocketServerForROB._clientAdress.Count - 1; i >= 0; i--)
                        {
                            string[] ip = newSocketServerForROB._clientAdress[i].Split(':');
                            if (ip[0] == "192.168.0.90")
                            {
                                _findRobConnectFlag = true;
                                if (ROBPCCommunicationStatus)
                                {
                                    if (!newSocketServerForROB.sendMessage1(newSocketServerForROB._clientAdress[i], "POS"))
                                    {
                                        MyTool.TxtFileProcess.CreateLog("IPC1发送\"POS\"到ROB1失败!" + "----用户：" + _operatorName,
                                        _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                                        newCalibrateForm.manualGetPositionToRobRequest = false;
                                        //MessageBox.Show("IPC1发送\"POS\"到ROB1失败!");
                                    }
                                    else
                                    {
                                        MyTool.TxtFileProcess.CreateLog("IPC1发送\"POS\"到ROB1成功!" + "----用户：" + _operatorName,
                                        _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                                        newCalibrateForm.manualGetPositionToRobRequest = false;
                                    }
                                }
                            }
                        }
                        if (!_findRobConnectFlag)
                        {
                            MyTool.TxtFileProcess.CreateLog("没有与ROB1连接!" + "----用户：" + _operatorName,
                            _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                            //MessageBox.Show("没有与ROB1连接!");
                        }
                        newCalibrateForm.manualGetPositionToRobRequest = false;
                    }

                    //ADD 20221017 发送拍照失败信息到ROB
                    if (_CameraGrabFail_S1)
                    {
                        _CameraGrabFail_S1 = false;
                        ROBRequestFlag_S1 = false;
                        bool _findRobConnectFlag = false;
                        for (int i = newSocketServerForROB._clientAdress.Count - 1; i >= 0; i--)
                        {
                            string[] ip = newSocketServerForROB._clientAdress[i].Split(':');
                            if (ip[0] == "192.168.0.90")
                            {
                                _findRobConnectFlag = true;
                                if (ROBPCCommunicationStatus)
                                {
                                    //发送拍照失败信号给ROB
                                    if (!newSocketServerForROB.sendMessage1(newSocketServerForROB._clientAdress[i], "TRI1GQ,"))
                                    {
                                        MyTool.TxtFileProcess.CreateLog("IPC1发送TRI1GQ," + "到ROB1:" + newSocketServerForROB._clientAdress[i].ToString() + "失败!" + "----用户：" + _operatorName,
                                        _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                                        //MessageBox.Show("IPC1发送TRI1GQ, " + "到ROB1失败!");
                                    }
                                    else
                                    {
                                        MyTool.TxtFileProcess.CreateLog("IPC1发送TRI1GQ," + "到ROB1:" + newSocketServerForROB._clientAdress[i].ToString() + "成功!" + "----用户：" + _operatorName,
                                        _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                                        break;
                                    }
                                }
                            }
                        }
                        if (!_findRobConnectFlag)
                        {
                            MyTool.TxtFileProcess.CreateLog("没有与ROB1连接!" + "----用户：" + _operatorName,
                            _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                            //MessageBox.Show("没有与ROB1连接!");
                        }
                    }
                    if (_CameraGrabFail_S2)
                    {
                        _CameraGrabFail_S2 = false;
                        ROBRequestFlag_S2 = false;
                        bool _findRobConnectFlag = false;
                        for (int i = newSocketServerForROB._clientAdress.Count - 1; i >= 0; i--)
                        {
                            string[] ip = newSocketServerForROB._clientAdress[i].Split(':');
                            if (ip[0] == "192.168.0.90")
                            {
                                _findRobConnectFlag = true;
                                if (ROBPCCommunicationStatus)
                                {
                                    //发送拍照失败信号给ROB
                                    if (!newSocketServerForROB.sendMessage1(newSocketServerForROB._clientAdress[i], "TRI2GQ,"))
                                    {
                                        MyTool.TxtFileProcess.CreateLog("IPC1发送TRI2GQ," + "到ROB1:" + newSocketServerForROB._clientAdress[i].ToString() + "失败!" + "----用户：" + _operatorName,
                                        _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                                        //MessageBox.Show("IPC1发送TRI2GQ, " + "到ROB1失败!");
                                    }
                                    else
                                    {
                                        MyTool.TxtFileProcess.CreateLog("IPC1发送TRI2GQ," + "到ROB1:" + newSocketServerForROB._clientAdress[i].ToString() + "成功!" + "----用户：" + _operatorName,
                                        _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                                        break;
                                    }
                                }
                            }
                        }
                        if (!_findRobConnectFlag)
                        {
                            MyTool.TxtFileProcess.CreateLog("没有与ROB1连接!" + "----用户：" + _operatorName,
                            _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                            //MessageBox.Show("没有与ROB1连接!");
                        }
                    }
                    if (_CameraGrabFail_S3)
                    {
                        _CameraGrabFail_S3 = false;
                        ROBRequestFlag_S3 = false;
                        bool _findRobConnectFlag = false;
                        for (int i = newSocketServerForROB._clientAdress.Count - 1; i >= 0; i--)
                        {
                            string[] ip = newSocketServerForROB._clientAdress[i].Split(':');
                            if (ip[0] == "192.168.0.90")
                            {
                                _findRobConnectFlag = true;
                                if (ROBPCCommunicationStatus)
                                {
                                    //发送拍照失败信号给ROB
                                    if (!newSocketServerForROB.sendMessage1(newSocketServerForROB._clientAdress[i], "TRI3GQ,"))
                                    {
                                        MyTool.TxtFileProcess.CreateLog("IPC1发送TRI3GQ," + "到ROB1:" + newSocketServerForROB._clientAdress[i].ToString() + "失败!" + "----用户：" + _operatorName,
                                        _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                                        //MessageBox.Show("IPC1发送TRI3GQ, " + "到ROB1失败!");
                                    }
                                    else
                                    {
                                        MyTool.TxtFileProcess.CreateLog("IPC1发送TRI3GQ," + "到ROB1:" + newSocketServerForROB._clientAdress[i].ToString() + "成功!" + "----用户：" + _operatorName,
                                        _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                                        break;
                                    }
                                }
                            }
                        }
                        if (!_findRobConnectFlag)
                        {
                            MyTool.TxtFileProcess.CreateLog("没有与ROB1连接!" + "----用户：" + _operatorName,
                            _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                            //MessageBox.Show("没有与ROB1连接!");
                        }
                    }

                    //ADD 20221017 发送取料位置计算处理失败信息到ROB
                    if (_PickupPositionProcessFail_S1)
                    {
                        _PickupPositionProcessFail_S1 = false;
                        ROBRequestFlag_S1 = false;
                        _autoModeCameraGrabOKFlag_S1 = false;
                        bool _findRobConnectFlag = false;
                        for (int i = newSocketServerForROB._clientAdress.Count - 1; i >= 0; i--)
                        {
                            string[] ip = newSocketServerForROB._clientAdress[i].Split(':');
                            if (ip[0] == "192.168.0.90")
                            {
                                _findRobConnectFlag = true;
                                if (ROBPCCommunicationStatus)
                                {
                                    //发送取料位置计算处理失败信息到ROB
                                    if (!newSocketServerForROB.sendMessage1(newSocketServerForROB._clientAdress[i], "TRI1JQ,"))
                                    {
                                        MyTool.TxtFileProcess.CreateLog("IPC1发送TRI1JQ," + "到ROB1:" + newSocketServerForROB._clientAdress[i].ToString() + "失败!" + "----用户：" + _operatorName,
                                        _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                                        //MessageBox.Show("IPC1发送TRI1JQ, " + "到ROB1失败!");
                                    }
                                    else
                                    {
                                        MyTool.TxtFileProcess.CreateLog("IPC1发送TRI1JQ," + "到ROB1:" + newSocketServerForROB._clientAdress[i].ToString() + "成功!" + "----用户：" + _operatorName,
                                        _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                                        break;
                                    }
                                }
                            }
                        }
                        if (!_findRobConnectFlag)
                        {
                            MyTool.TxtFileProcess.CreateLog("没有与ROB1连接!" + "----用户：" + _operatorName,
                            _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                            //MessageBox.Show("没有与ROB1连接!");
                        }
                    }
                    if (_PickupPositionProcessFail_S2)
                    {
                        _PickupPositionProcessFail_S2 = false;
                        ROBRequestFlag_S2 = false;
                        _autoModeCameraGrabOKFlag_S2 = false;
                        bool _findRobConnectFlag = false;
                        for (int i = newSocketServerForROB._clientAdress.Count - 1; i >= 0; i--)
                        {
                            string[] ip = newSocketServerForROB._clientAdress[i].Split(':');
                            if (ip[0] == "192.168.0.90")
                            {
                                _findRobConnectFlag = true;
                                if (ROBPCCommunicationStatus)
                                {
                                    //发送取料位置计算处理失败信息到ROB
                                    if (!newSocketServerForROB.sendMessage1(newSocketServerForROB._clientAdress[i], "TRI2JQ,"))
                                    {
                                        MyTool.TxtFileProcess.CreateLog("IPC1发送TRI2JQ," + "到ROB1:" + newSocketServerForROB._clientAdress[i].ToString() + "失败!" + "----用户：" + _operatorName,
                                        _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                                        //MessageBox.Show("IPC1发送TRI2JQ, " + "到ROB1失败!");
                                    }
                                    else
                                    {
                                        MyTool.TxtFileProcess.CreateLog("IPC1发送TRI2JQ," + "到ROB1:" + newSocketServerForROB._clientAdress[i].ToString() + "成功!" + "----用户：" + _operatorName,
                                        _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                                        break;
                                    }
                                }
                            }
                        }
                        if (!_findRobConnectFlag)
                        {
                            MyTool.TxtFileProcess.CreateLog("没有与ROB1连接!" + "----用户：" + _operatorName,
                            _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                            //MessageBox.Show("没有与ROB1连接!");
                        }
                    }
                    if (_PickupPositionProcessFail_S3)
                    {
                        _PickupPositionProcessFail_S3 = false;
                        ROBRequestFlag_S3 = false;
                        _autoModeCameraGrabOKFlag_S3 = false;
                        bool _findRobConnectFlag = false;
                        for (int i = newSocketServerForROB._clientAdress.Count - 1; i >= 0; i--)
                        {
                            string[] ip = newSocketServerForROB._clientAdress[i].Split(':');
                            if (ip[0] == "192.168.0.90")
                            {
                                _findRobConnectFlag = true;
                                if (ROBPCCommunicationStatus)
                                {
                                    //发送取料位置计算处理失败信息到ROB
                                    if (!newSocketServerForROB.sendMessage1(newSocketServerForROB._clientAdress[i], "TRI3JQ,"))
                                    {
                                        MyTool.TxtFileProcess.CreateLog("IPC1发送TRI3JQ," + "到ROB1:" + newSocketServerForROB._clientAdress[i].ToString() + "失败!" + "----用户：" + _operatorName,
                                        _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                                        //MessageBox.Show("IPC1发送TRI3JQ, " + "到ROB1失败!");
                                    }
                                    else
                                    {
                                        MyTool.TxtFileProcess.CreateLog("IPC1发送TRI3JQ," + "到ROB1:" + newSocketServerForROB._clientAdress[i].ToString() + "成功!" + "----用户：" + _operatorName,
                                        _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                                        break;
                                    }
                                }
                            }
                        }
                        if (!_findRobConnectFlag)
                        {
                            MyTool.TxtFileProcess.CreateLog("没有与ROB1连接!" + "----用户：" + _operatorName,
                            _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                            //MessageBox.Show("没有与ROB1连接!");
                        }
                    }
                }
                #endregion

                #region receive
                if (newSocketServerForROB._socketServerStartOKFlag && newSocketServerForROB.receiveStatus != "")//receiveStatus:包含客户端的IP地址及端口号
                {
                    MyTool.TxtFileProcess.CreateLog("接收到的ROB发送的信息" + " 信息:" + newSocketServerForROB.receiveStatus + "----用户：" + _operatorName,
                    _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                    string _ip = "";
                    string _message = "";
                    if (newSocketServerForROB.receiveStatus != "")
                    {
                        if (newSocketServerForROB.receiveStatus.Substring(0, 3) != "成功与")
                        {
                            try
                            {
                                _ip = newSocketServerForROB.receiveStatus.Split(':')[0];
                            }
                            catch (Exception ex)
                            {
                                MyTool.TxtFileProcess.CreateLog("接收到的ROB发送的信息中的IP地址错误!" + " 信息:" + newSocketServerForROB.receiveStatus + "----用户：" + _operatorName,
                                _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                                //MessageBox.Show("接收到的ROB发送的信息中的IP地址错误!" + " 信息:" + newSocketServerForROB.receiveStatus);
                            }
                            try
                            {
                                _message = newSocketServerForROB.receiveStatus.Split('[')[1].Substring(0, newSocketServerForROB.receiveStatus.Split('[')[1].Length - 1);
                            }
                            catch (Exception ex)
                            {
                                MyTool.TxtFileProcess.CreateLog("接收到的ROB发送的信息中的数据错误!" + " 信息:" + newSocketServerForROB.receiveStatus + "----用户：" + _operatorName,
                                _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                                //MessageBox.Show("接收到的ROB发送的信息中的数据错误!" + " 信息:" + newSocketServerForROB.receiveStatus);
                            }
                        }

                        if (_message == "")
                        {
                            MyTool.TxtFileProcess.CreateLog("接收到的ROB发送的信息为空!" + " 信息:" + newSocketServerForROB.receiveStatus + "----用户：" + _operatorName,
                            _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                        }
                        if (_message != "")
                        {
                            if (_ip == "192.168.0.90" && _message.Substring(0, 3) == "POS")//接收到ROB发送的位置数据
                            {
                                try
                                {
                                    ROB_XCurrentPosition = Convert.ToSingle(_message.Split(',')[1]);
                                }
                                catch (Exception ex)
                                {
                                    MyTool.TxtFileProcess.CreateLog("接收到的ROB发送的信息中的X坐标数据错误!" + " 信息:" + newSocketServerForROB.receiveStatus + "----用户：" + _operatorName,
                                    _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                                    //MessageBox.Show("接收到的ROB发送的信息中的X坐标数据错误!" + " 信息:" + newSocketServerForROB.receiveStatus);
                                }
                                try
                                {
                                    ROB_YCurrentPosition = Convert.ToSingle(_message.Split(',')[2]);
                                }
                                catch (Exception ex)
                                {
                                    MyTool.TxtFileProcess.CreateLog("接收到的ROB发送的信息中的Y坐标数据错误!" + " 信息:" + newSocketServerForROB.receiveStatus + "----用户：" + _operatorName,
                                    _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                                    //MessageBox.Show("接收到的ROB发送的信息中的Y坐标数据错误!" + " 信息:" + newSocketServerForROB.receiveStatus);
                                }
                                try
                                {
                                    ROB_ZCurrentPosition = Convert.ToSingle(_message.Split(',')[3]);
                                }
                                catch (Exception ex)
                                {
                                    MyTool.TxtFileProcess.CreateLog("接收到的ROB发送的信息中的Z坐标数据错误!" + " 信息:" + newSocketServerForROB.receiveStatus + "----用户：" + _operatorName,
                                    _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                                    //MessageBox.Show("接收到的ROB发送的信息中的Z坐标数据错误!" + " 信息:" + newSocketServerForROB.receiveStatus);
                                }
                                try
                                {
                                    ROB_RXCurrentPosition = Convert.ToSingle(_message.Split(',')[4]);
                                }
                                catch (Exception ex)
                                {
                                    MyTool.TxtFileProcess.CreateLog("接收到的ROB发送的信息中的RX坐标数据错误!" + " 信息:" + newSocketServerForROB.receiveStatus + "----用户：" + _operatorName,
                                    _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                                    //MessageBox.Show("接收到的ROB发送的信息中的RX坐标数据错误!" + " 信息:" + newSocketServerForROB.receiveStatus);
                                }
                                try
                                {
                                    ROB_RWCurrentPosition = Convert.ToSingle(_message.Split(',')[5]);
                                }
                                catch (Exception ex)
                                {
                                    MyTool.TxtFileProcess.CreateLog("接收到的ROB发送的信息中的RW坐标数据错误!" + " 信息:" + newSocketServerForROB.receiveStatus + "----用户：" + _operatorName,
                                    _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                                    //MessageBox.Show("接收到的ROB发送的信息中的RW坐标数据错误!" + " 信息:" + newSocketServerForROB.receiveStatus);
                                }
                                try
                                {
                                    ROB_RZCurrentPosition = Convert.ToSingle(_message.Split(',')[6]);
                                }
                                catch (Exception ex)
                                {
                                    MyTool.TxtFileProcess.CreateLog("接收到的ROB发送的信息中的RZ坐标数据错误!" + " 信息:" + newSocketServerForROB.receiveStatus + "----用户：" + _operatorName,
                                    _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                                    //MessageBox.Show("接收到的ROB发送的信息中的RZ坐标数据错误!" + " 信息:" + newSocketServerForROB.receiveStatus);
                                }
                                newCalibrateForm.manualGetPositionToRobRequest_Respond = true;
                            }

                            if (_ip == "192.168.0.90" && _message.Length == 4)
                            {
                                //Station1
                                if (_message.Substring(0, 4) == "TRI1")//接收到ROB发送的拍照请求
                                {
                                    MyTool.TxtFileProcess.CreateLog("接收到ROB" + newSocketServerForROB.receiveStatus + "发送的拍照请求(TRI1)!" + "----用户：" + _operatorName,
                                    _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                                    _autoModeCameraGrabOKFlag_S1 = false;
                                    ROBRequestFlag_S1 = true;
                                    _CameraGrab_S1 = true;
                                }
                                //Station2
                                if (_message.Substring(0, 4) == "TRI2")//接收到ROB发送的拍照请求
                                {
                                    MyTool.TxtFileProcess.CreateLog("接收到ROB" + newSocketServerForROB.receiveStatus + "发送的拍照请求(TRI2)!" + "----用户：" + _operatorName,
                                    _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                                    _autoModeCameraGrabOKFlag_S2 = false;
                                    ROBRequestFlag_S2 = true;
                                    _CameraGrab_S2 = true;
                                }
                                //Station3
                                if (_message.Substring(0, 4) == "TRI3")//接收到ROB发送的拍照请求
                                {
                                    MyTool.TxtFileProcess.CreateLog("接收到ROB" + newSocketServerForROB.receiveStatus + "发送的拍照请求(TRI3)!" + "----用户：" + _operatorName,
                                    _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                                    _autoModeCameraGrabOKFlag_S3 = false;
                                    ROBRequestFlag_S3 = true;
                                    _CameraGrab_S3 = true;
                                }
                            }
                        }
                    }

                    newSocketServerForROB.receiveStatus = "";
                }
                #endregion

                if (!_socketServerStartOKAndConnectROBOKFlag)
                {
                    lblConnectStatus.BeginInvoke(new Action(() =>
                    {
                        lblConnectStatus.Text = "未连接ROB";
                    }));
                }
                else
                {
                    this.BeginInvoke(new Action(() =>
                    {
                        lblConnectStatus.Text = "已连接ROB";
                    }));
                }

                //MODIFY 20221017 增加对取料位置计算处理失败标志的置位
                if (ROBRequestFlag_S1 && _autoModeCameraGrabOKFlag_S1)
                    //计算ROB的取料位置
                    if (!ROBNewPickupPositionCalculate_S(1))
                        _PickupPositionProcessFail_S1 = true;
                if (ROBRequestFlag_S2 && _autoModeCameraGrabOKFlag_S2)
                    //计算ROB的取料位置
                    if (!ROBNewPickupPositionCalculate_S(2))
                        _PickupPositionProcessFail_S2 = true;
                if (ROBRequestFlag_S3 && _autoModeCameraGrabOKFlag_S3)
                    //计算ROB的取料位置
                    if (!ROBNewPickupPositionCalculate_S(3))
                        _PickupPositionProcessFail_S3 = true;

                if (_socketServerStartOKAndConnectROBOKFlag && !ROBConnectOKNeedLogFlag)
                {
                    ROBConnectOKNeedLogFlag = true;
                    ROBConnectNGNeedLogFlag = false;
                    MyTool.TxtFileProcess.CreateLog("已连接ROB" + "----用户：" + _operatorName,
                    _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                }
                if (!_socketServerStartOKAndConnectROBOKFlag && !ROBConnectNGNeedLogFlag)
                {
                    ROBConnectNGNeedLogFlag = true;
                    ROBConnectOKNeedLogFlag = false;
                    MyTool.TxtFileProcess.CreateLog("未连接ROB" + "----用户：" + _operatorName,
                    _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                }

                Thread.Sleep(1);
            }
        }
        #endregion

        #region ROBNewPickupPositionCalculate_S:计算ROB的取料位置
        private bool ROBNewPickupPositionCalculate_S(int _stationNumber)
        {
            //Station1
            if (_stationNumber == 1)
            {
                newPickupPositionOKFlag_S1 = false;
                if (newCalibrateForm.mainProcess_S1() != 0)
                {
                    //MessageBox.Show("计算S1的ROB1的取料位置出错!");
                    MyTool.TxtFileProcess.CreateLog("计算S1的ROB1的取料位置出错!" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    _autoModeCameraGrabOKFlag_S1 = false;
                    _CameraGrab_S1 = false;
                    return false;
                }
                newPickupPositionOKFlag_S1 = true;
                _autoModeCameraGrabOKFlag_S1 = false;
                return true;
            }
            //Station2
            if (_stationNumber == 2)
            {
                newPickupPositionOKFlag_S2 = false;
                if (newCalibrateForm.mainProcess_S2() != 0)
                {
                    //MessageBox.Show("计算S2的ROB2的取料位置出错!");
                    MyTool.TxtFileProcess.CreateLog("计算S2的ROB1的取料位置出错!" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    _autoModeCameraGrabOKFlag_S2 = false;
                    _CameraGrab_S2 = false;
                    return false;
                }
                newPickupPositionOKFlag_S2 = true;
                _autoModeCameraGrabOKFlag_S2 = false;
                return true;
            }
            //Station3
            if (_stationNumber == 3)
            {
                classificationFinishFlag_S3 = false;
                newPickupPositionOKFlag_S3 = false;

                int kk = newCalibrateForm.mainProcess_S3();
                if (kk == -1)
                {
                    //MessageBox.Show("S3分类出错!");
                    MyTool.TxtFileProcess.CreateLog("S3分类出错!" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    _autoModeCameraGrabOKFlag_S3 = false;
                    _CameraGrab_S3 = false;
                    return false;
                }

                if (kk != 0)
                {
                    //MessageBox.Show("计算S3的ROB1的取料位置出错!");
                    MyTool.TxtFileProcess.CreateLog("计算S3的ROB1的取料位置出错!" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    _autoModeCameraGrabOKFlag_S3 = false;
                    _CameraGrab_S3 = false;
                    return false;
                }
                classificationFinishFlag_S3 = true;
                newPickupPositionOKFlag_S3 = true;
                _autoModeCameraGrabOKFlag_S3 = false;
                return true;
            }
            return false;
        }
        #endregion

        #region 相机动作
        #region 打开相机
        /// <summary>
        /// OpenCamera:打开相机
        /// </summary>
        /// <param name="cameraControlCs">HalconCameraControl:指定相机控制类名</param>
        /// <param name="deviceName">HTuple:相机名称</param>
        /// <param name="externalTriggerFlag">bool:是否使用外部触发，如果使用外部触发，那么调用外部触发打开相机函数</param>
        /// <returns>bool:返回相机打开结果</returns>
        private bool OpenCamera(ref HalconCameraControl cameraControlCs, HTuple deviceName, bool externalTriggerFlag, bool dispErrorMsgFlag)
        {
            cameraControlCs.hv_AcqHandle = null;
            if (externalTriggerFlag)
            {
                if (cameraControlCs.OpenCameraInExternalTrigger("MVision", 0, 0, 0, 0, 0, 0,
                    "progressive", -1, "default", -1, "false", "default", deviceName, 0, -1, dispErrorMsgFlag))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (cameraControlCs.OpenCameraInSoftTrigger("MVision", 0, 0, 0, 0, 0, 0,
                     "progressive", -1, "default", -1, "false", "default", deviceName, 0, -1, dispErrorMsgFlag))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        #endregion

        #region CloseCamera:关闭相机
        /// <summary>
        /// CloseCamera:关闭相机
        /// </summary>
        /// <param name="cameraControlCs">HalconCameraControl:指定相机控制类名</param>
        /// <returns>bool:返回相机关闭结果</returns>
        private bool CloseCamera(ref HalconCameraControl cameraControlCs)
        {
            return cameraControlCs.CloseCamera();
        }
        #endregion

        #region CameraImageGrabAndDispThreadFunc:相机图像抓取显示线程函数
        /// <summary>
        /// CameraImageGrabAndDispThreadFunc:相机图像抓取显示线程函数
        /// </summary>
        private void CameraImageGrabAndDispThreadFunc()
        {
            bool tempbool = false;//用于跳出某些步骤
            while (true)
            {
                //Station1
                if (_CameraVideo_S1 || _CameraGrab_S1)//如果触发相机实时显示标志或抓取图像标志
                {
                    if (_CameraCs_S1.hv_AcqHandle == null)
                    {
                        //MODIFY 20221017
                        for (int i = 0; i < 4; i++)
                        {
                            if (i >= 3)
                            {
                                //MessageBox.Show("S1 打开相机失败");
                                if (ROBRequestFlag_S1)
                                    _CameraGrabFail_S1 = true;
                                break;
                            }
                            if (!OpenCamera(ref _CameraCs_S1, CalibrateForm._cameraName_S1, false, false))
                            {
                                MyTool.TxtFileProcess.CreateLog("S1 第" + (i + 1).ToString() + "次打开相机失败!" + "----用户：" + _operatorName,
                               _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                                _CameraVideo_S1 = false;
                                _CameraGrab_S1 = false;

                                if (!CloseCamera(ref _CameraCs_S1))
                                    MyTool.TxtFileProcess.CreateLog("S1 第" + (i + 1).ToString() + "次打开相机失败,第" + (i + 1).ToString() + "次关闭相机失败!" + "----用户：" + _operatorName,
                                    _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                                else
                                    MyTool.TxtFileProcess.CreateLog("S1 第" + (i + 1).ToString() + "次打开相机失败,第" + (i + 1).ToString() + "次关闭相机成功." + "----用户：" + _operatorName,
                                    _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                                Thread.Sleep(100);//打开相机之后等待100ms
                            }
                            else
                            {
                                MyTool.TxtFileProcess.CreateLog("S1 第" + (i + 1).ToString() + "次打开相机成功!" + "----用户：" + _operatorName,
                                _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                                break;
                            }
                        }
                        Thread.Sleep(100);//打开相机之后等待100ms
                    }
                    if (_CameraCs_S1.hv_AcqHandle != null)
                    {
                        tempbool = true;
                        Thread.Sleep(10);
                        if (_CameraCs_S1.ho_Image != null)
                        {
                            _CameraCs_S1.ho_Image.Dispose();
                            _CameraCs_S1.ho_Image = null;
                        }
                        if (newCalibrateForm.ho_Image_S1 != null)
                        {
                            newCalibrateForm.ho_Image_S1.Dispose();
                            newCalibrateForm.ho_Image_S1 = null;
                        }

                        //MODIFY 20221017
                        for (int i = 0; i < 5; i++)
                        {
                            if (i >= 4)
                            {
                                if (ROBRequestFlag_S1)
                                    _CameraGrabFail_S1 = true;
                                break;
                            }
                            if (!_CameraCs_S1.GrabImage_SoftwareTriggerAsync(false))
                            {
                                MyTool.TxtFileProcess.CreateLog("S1 相机第" + (i + 1).ToString() + "次拍照失败!" + "----用户：" + _operatorName,
                                _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                                _CameraVideo_S1 = false;
                                _CameraGrab_S1 = false;
                                tempbool = false;
                                for (int k = 0; k < 3; k++)
                                {
                                    if (!CloseCamera(ref _CameraCs_S1))
                                        MyTool.TxtFileProcess.CreateLog("S1 相机第" + (i + 1).ToString() + "次拍照失败,第" + (k + 1).ToString() + "次关闭相机失败!" + "----用户：" + _operatorName,
                                        _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                                    else
                                        MyTool.TxtFileProcess.CreateLog("S1 相机第" + (i + 1).ToString() + "次拍照失败,第" + (k + 1).ToString() + "次关闭相机成功." + "----用户：" + _operatorName,
                                        _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                                    Thread.Sleep(100);
                                    if (!OpenCamera(ref _CameraCs_S1, CalibrateForm._cameraName_S1, false, false))
                                        MyTool.TxtFileProcess.CreateLog("S1 相机第" + (i + 1).ToString() + "次拍照失败,第" + (k + 1).ToString() + "次打开相机失败!" + "----用户：" + _operatorName,
                                       _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                                    else
                                    {
                                        MyTool.TxtFileProcess.CreateLog("S1 相机第" + (i + 1).ToString() + "次拍照失败,第" + (k + 1).ToString() + "次打开相机成功." + "----用户：" + _operatorName,
                                        _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                                        break;
                                    }
                                    Thread.Sleep(100);
                                }
                            }
                            else
                            {
                                MyTool.TxtFileProcess.CreateLog("S1 相机第" + (i + 1).ToString() + "次拍照成功!" + "----用户：" + _operatorName,
                                _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                                //add 20230324
                                if (!tempbool)
                                {
                                    tempbool = true;
                                    MyTool.TxtFileProcess.CreateLog("S1 相机第" + (i + 1).ToString() + "次拍照成功!" + "并且恢复 tempbool 为true" + " ----用户：" + _operatorName,
                                    _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                                }
                                break;
                            }
                        }

                        //Add Rotation
                        if (_CameraCs_S1.ho_Image != null)
                        {
                            //HOperatorSet.RotateImage(_CameraCs_S1.ho_Image, out _CameraCs_S1.ho_Image, -90, "constant");
                        }

                        if (tempbool)
                        {
                            hw_S1.ClearWindow();
                            hw_S1.HobjectToHimage(_CameraCs_S1.ho_Image);

                            newCalibrateForm.hw_S1.ClearWindow();
                            HOperatorSet.CopyImage(_CameraCs_S1.ho_Image, out newCalibrateForm.ho_Image_S1);
                            newCalibrateForm.hw_S1.HobjectToHimage(newCalibrateForm.ho_Image_S1);
                            _autoModeCameraGrabOKFlag_S1 = true;

                            if (_CameraGrab_S1)
                                _CameraGrab_S1 = false;
                            Thread.Sleep(CalibrateForm._CameraGrabImageInterval);//等待相机抓取图像延时
                        }
                    }
                }

                //Station2
                if (_CameraVideo_S2 || _CameraGrab_S2)//如果触发相机实时显示标志或抓取图像标志
                {
                    if (_CameraCs_S2.hv_AcqHandle == null)
                    {
                        //MODIFY 20221017
                        for (int i = 0; i < 4; i++)
                        {
                            if (i >= 3)
                            {
                                //MessageBox.Show("S2 打开相机失败");
                                if (ROBRequestFlag_S2)
                                    _CameraGrabFail_S2 = true;
                                break;
                            }
                            if (!OpenCamera(ref _CameraCs_S2, CalibrateForm._cameraName_S2, false, false))
                            {
                                MyTool.TxtFileProcess.CreateLog("S2 第" + (i + 1).ToString() + "次打开相机失败!" + "----用户：" + _operatorName,
                               _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                                _CameraVideo_S2 = false;
                                _CameraGrab_S2 = false;

                                if (!CloseCamera(ref _CameraCs_S2))
                                    MyTool.TxtFileProcess.CreateLog("S2 第" + (i + 1).ToString() + "次打开相机失败,第" + (i + 1).ToString() + "次关闭相机失败!" + "----用户：" + _operatorName,
                                    _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                                else
                                    MyTool.TxtFileProcess.CreateLog("S2 第" + (i + 1).ToString() + "次打开相机失败,第" + (i + 1).ToString() + "次关闭相机成功." + "----用户：" + _operatorName,
                                    _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                                Thread.Sleep(100);//打开相机之后等待100ms
                            }
                            else
                            {
                                MyTool.TxtFileProcess.CreateLog("S2 第" + (i + 1).ToString() + "次打开相机成功!" + "----用户：" + _operatorName,
                                _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                                break;
                            }
                        }
                        Thread.Sleep(100);//打开相机之后等待100ms
                    }
                    if (_CameraCs_S2.hv_AcqHandle != null)
                    {
                        tempbool = true;
                        Thread.Sleep(10);
                        if (_CameraCs_S2.ho_Image != null)
                        {
                            _CameraCs_S2.ho_Image.Dispose();
                            _CameraCs_S2.ho_Image = null;
                        }
                        if (newCalibrateForm.ho_Image_S2 != null)
                        {
                            newCalibrateForm.ho_Image_S2.Dispose();
                            newCalibrateForm.ho_Image_S2 = null;
                        }

                        //MODIFY 20221017
                        for (int i = 0; i < 5; i++)
                        {
                            if (i >= 4)
                            {
                                if (ROBRequestFlag_S2)
                                    _CameraGrabFail_S2 = true;
                                break;
                            }
                            if (!_CameraCs_S2.GrabImage_SoftwareTriggerAsync(false))
                            {
                                MyTool.TxtFileProcess.CreateLog("S2 相机第" + (i + 1).ToString() + "次拍照失败!" + "----用户：" + _operatorName,
                                _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                                _CameraVideo_S2 = false;
                                _CameraGrab_S2 = false;
                                tempbool = false;
                                for (int k = 0; k < 3; k++)
                                {
                                    if (!CloseCamera(ref _CameraCs_S2))
                                        MyTool.TxtFileProcess.CreateLog("S2 相机第" + (i + 1).ToString() + "次拍照失败,第" + (k + 1).ToString() + "次关闭相机失败!" + "----用户：" + _operatorName,
                                        _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                                    else
                                        MyTool.TxtFileProcess.CreateLog("S2 相机第" + (i + 1).ToString() + "次拍照失败,第" + (k + 1).ToString() + "次关闭相机成功." + "----用户：" + _operatorName,
                                        _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                                    Thread.Sleep(100);
                                    if (!OpenCamera(ref _CameraCs_S2, CalibrateForm._cameraName_S2, false, false))
                                        MyTool.TxtFileProcess.CreateLog("S2 相机第" + (i + 1).ToString() + "次拍照失败,第" + (k + 1).ToString() + "次打开相机失败!" + "----用户：" + _operatorName,
                                       _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                                    else
                                    {
                                        MyTool.TxtFileProcess.CreateLog("S2 相机第" + (i + 1).ToString() + "次拍照失败,第" + (k + 1).ToString() + "次打开相机成功." + "----用户：" + _operatorName,
                                        _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                                        break;
                                    }
                                    Thread.Sleep(100);
                                }
                            }
                            else
                            {
                                MyTool.TxtFileProcess.CreateLog("S2 相机第" + (i + 1).ToString() + "次拍照成功!" + "----用户：" + _operatorName,
                                _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                                //add 20230324
                                if (!tempbool)
                                {
                                    tempbool = true;
                                    MyTool.TxtFileProcess.CreateLog("S2 相机第" + (i + 1).ToString() + "次拍照成功!" + "并且恢复 tempbool 为true" + " ----用户：" + _operatorName,
                                    _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                                }
                                break;
                            }
                        }

                        //Add Rotation
                        if (_CameraCs_S2.ho_Image != null)
                        {
                            //HOperatorSet.RotateImage(_CameraCs_S2.ho_Image, out _CameraCs_S2.ho_Image, -90, "constant");
                        }

                        if (tempbool)
                        {
                            hw_S2.ClearWindow();
                            hw_S2.HobjectToHimage(_CameraCs_S2.ho_Image);

                            newCalibrateForm.hw_S2.ClearWindow();
                            HOperatorSet.CopyImage(_CameraCs_S2.ho_Image, out newCalibrateForm.ho_Image_S2);
                            newCalibrateForm.hw_S2.HobjectToHimage(newCalibrateForm.ho_Image_S2);
                            _autoModeCameraGrabOKFlag_S2 = true;

                            if (_CameraGrab_S2)
                                _CameraGrab_S2 = false;
                            Thread.Sleep(CalibrateForm._CameraGrabImageInterval);//等待相机抓取图像延时
                        }
                    }
                }

                //Station3
                if (_CameraVideo_S3 || _CameraGrab_S3)//如果触发相机实时显示标志或抓取图像标志
                {
                    if (_CameraCs_S3.hv_AcqHandle == null)
                    {
                        //MODIFY 20221017
                        for (int i = 0; i < 4; i++)
                        {
                            if (i >= 3)
                            {
                                //MessageBox.Show("S3 打开相机失败");
                                if (ROBRequestFlag_S3)
                                    _CameraGrabFail_S3 = true;
                                break;
                            }
                            if (!OpenCamera(ref _CameraCs_S3, CalibrateForm._cameraName_S3, false, false))
                            {
                                MyTool.TxtFileProcess.CreateLog("S3 第" + (i + 1).ToString() + "次打开相机失败!" + "----用户：" + _operatorName,
                               _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                                _CameraVideo_S3 = false;
                                _CameraGrab_S3 = false;

                                if (!CloseCamera(ref _CameraCs_S3))
                                    MyTool.TxtFileProcess.CreateLog("S3 第" + (i + 1).ToString() + "次打开相机失败,第" + (i + 1).ToString() + "次关闭相机失败!" + "----用户：" + _operatorName,
                                    _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                                else
                                    MyTool.TxtFileProcess.CreateLog("S3 第" + (i + 1).ToString() + "次打开相机失败,第" + (i + 1).ToString() + "次关闭相机成功." + "----用户：" + _operatorName,
                                    _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                                Thread.Sleep(100);//打开相机之后等待100ms
                            }
                            else
                            {
                                MyTool.TxtFileProcess.CreateLog("S3 第" + (i + 1).ToString() + "次打开相机成功!" + "----用户：" + _operatorName,
                                _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                                break;
                            }
                        }
                        Thread.Sleep(100);//打开相机之后等待100ms
                    }
                    if (_CameraCs_S3.hv_AcqHandle != null)
                    {
                        tempbool = true;
                        Thread.Sleep(10);
                        if (_CameraCs_S3.ho_Image != null)
                        {
                            _CameraCs_S3.ho_Image.Dispose();
                            _CameraCs_S3.ho_Image = null;
                        }
                        if (newCalibrateForm.ho_Image_S3 != null)
                        {
                            newCalibrateForm.ho_Image_S3.Dispose();
                            newCalibrateForm.ho_Image_S3 = null;
                        }

                        //MODIFY 20221017
                        for (int i = 0; i < 5; i++)
                        {
                            if (i >= 4)
                            {
                                if (ROBRequestFlag_S3)
                                    _CameraGrabFail_S3 = true;
                                break;
                            }
                            if (!_CameraCs_S3.GrabImage_SoftwareTriggerAsync(false))
                            {
                                MyTool.TxtFileProcess.CreateLog("S3 相机第" + (i + 1).ToString() + "次拍照失败!" + "----用户：" + _operatorName,
                                _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                                _CameraVideo_S3 = false;
                                _CameraGrab_S3 = false;
                                tempbool = false;
                                for (int k = 0; k < 3; k++)
                                {
                                    if (!CloseCamera(ref _CameraCs_S3))
                                        MyTool.TxtFileProcess.CreateLog("S3 相机第" + (i + 1).ToString() + "次拍照失败,第" + (k + 1).ToString() + "次关闭相机失败!" + "----用户：" + _operatorName,
                                        _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                                    else
                                        MyTool.TxtFileProcess.CreateLog("S3 相机第" + (i + 1).ToString() + "次拍照失败,第" + (k + 1).ToString() + "次关闭相机成功." + "----用户：" + _operatorName,
                                        _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                                    Thread.Sleep(100);
                                    if (!OpenCamera(ref _CameraCs_S3, CalibrateForm._cameraName_S3, false, false))
                                        MyTool.TxtFileProcess.CreateLog("S3 相机第" + (i + 1).ToString() + "次拍照失败,第" + (k + 1).ToString() + "次打开相机失败!" + "----用户：" + _operatorName,
                                       _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                                    else
                                    {
                                        MyTool.TxtFileProcess.CreateLog("S3 相机第" + (i + 1).ToString() + "次拍照失败,第" + (k + 1).ToString() + "次打开相机成功." + "----用户：" + _operatorName,
                                        _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                                        break;
                                    }
                                    Thread.Sleep(100);
                                }
                            }
                            else
                            {
                                MyTool.TxtFileProcess.CreateLog("S3 相机第" + (i + 1).ToString() + "次拍照成功!" + "----用户：" + _operatorName,
                                _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                                //add 20230324
                                if (!tempbool)
                                {
                                    tempbool = true;
                                    MyTool.TxtFileProcess.CreateLog("S3 相机第" + (i + 1).ToString() + "次拍照成功!" + "并且恢复 tempbool 为true" + " ----用户：" + _operatorName,
                                    _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                                }
                                break;
                            }
                        }

                        //Add Rotation
                        if (_CameraCs_S3.ho_Image != null)
                        {
                            //HOperatorSet.RotateImage(_CameraCs_S3.ho_Image, out _CameraCs_S3.ho_Image, -90, "constant");
                        }

                        if (tempbool)
                        {
                            hw_S3.ClearWindow();
                            hw_S3.HobjectToHimage(_CameraCs_S3.ho_Image);

                            newCalibrateForm.hw_S3.ClearWindow();
                            HOperatorSet.CopyImage(_CameraCs_S3.ho_Image, out newCalibrateForm.ho_Image_S3);
                            newCalibrateForm.hw_S3.HobjectToHimage(newCalibrateForm.ho_Image_S3);
                            _autoModeCameraGrabOKFlag_S3 = true;

                            if (_CameraGrab_S3)
                                _CameraGrab_S3 = false;
                            Thread.Sleep(CalibrateForm._CameraGrabImageInterval);//等待相机抓取图像延时
                        }
                    }
                }

                Thread.Sleep(10);
            }
        }
        #endregion
        #endregion

        #region 相机事件函数
        //maintenance camera form请求打开或关闭相机事件函数
        private void MaintenanceCameraFormRequestOpenOrCloseCameraEventFunc(int cameraIndex)
        {
            switch (cameraIndex)
            {
                case 0://相机序号为0
                    if (_CameraCs_S1.hv_AcqHandle == null)
                    {
                        MyTool.TxtFileProcess.CreateLog("S1从手动设置界面请求打开相机！" + "----用户：" + _operatorName,
                        _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

                        Thread.Sleep(100);//等待
                        if (!OpenCamera(ref _CameraCs_S1, CalibrateForm._cameraName_S1, false, true))
                        {
                            MyTool.TxtFileProcess.CreateLog("S1打开相机失败！" + "----用户：" + _operatorName,
                            _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                            MessageBox.Show("S1打开相机失败");
                        }
                    }
                    else
                    {
                        MyTool.TxtFileProcess.CreateLog("S1从手动设置界面请求关闭相机！" + "----用户：" + _operatorName,
                        _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                        Thread.Sleep(100);//等待
                        CloseCamera(ref _CameraCs_S1);
                    }
                    break;
                case 1://相机序号为1
                    if (_CameraCs_S2.hv_AcqHandle == null)
                    {
                        MyTool.TxtFileProcess.CreateLog("S2从手动设置界面请求打开相机！" + "----用户：" + _operatorName,
                        _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

                        Thread.Sleep(100);//等待
                        if (!OpenCamera(ref _CameraCs_S2, CalibrateForm._cameraName_S2, false, true))
                        {
                            MyTool.TxtFileProcess.CreateLog("S2打开相机失败！" + "----用户：" + _operatorName,
                            _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                            MessageBox.Show("S2打开相机失败");
                        }
                    }
                    else
                    {
                        MyTool.TxtFileProcess.CreateLog("S2从手动设置界面请求关闭相机！" + "----用户：" + _operatorName,
                        _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                        Thread.Sleep(100);//等待
                        CloseCamera(ref _CameraCs_S2);
                    }
                    break;
                case 2://相机序号为2
                    if (_CameraCs_S3.hv_AcqHandle == null)
                    {
                        MyTool.TxtFileProcess.CreateLog("S3从手动设置界面请求打开相机！" + "----用户：" + _operatorName,
                        _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

                        Thread.Sleep(100);//等待
                        if (!OpenCamera(ref _CameraCs_S3, CalibrateForm._cameraName_S3, false, true))
                        {
                            MyTool.TxtFileProcess.CreateLog("S3打开相机失败！" + "----用户：" + _operatorName,
                            _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                            MessageBox.Show("S3打开相机失败");
                        }
                    }
                    else
                    {
                        MyTool.TxtFileProcess.CreateLog("S3从手动设置界面请求关闭相机！" + "----用户：" + _operatorName,
                        _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                        Thread.Sleep(100);//等待
                        CloseCamera(ref _CameraCs_S3);
                    }
                    break;

            }
        }
        //maintenance camera form请求抓取图像并显示事件函数
        private void MaintenanceCameraFormRequestGrabImageAndDispEventFunc(int cameraIndex, bool dispCrossLineFlag)
        {
            string tempLogStr = "手动设置界面";
            //if (_baseSettingFormVariate._settingsCameraCalibrationVariate.Visible)
            //    tempLogStr = "相机标定界面";
            switch (cameraIndex)
            {
                case 0://相机序号为0
                    MyTool.TxtFileProcess.CreateLog(tempLogStr + "S1请求相机抓取图像！" + "----用户：" + _operatorName,
                    _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

                    _CameraVideo_S1 = false;
                    Thread.Sleep(100);//等待相机抓取图像及显示线程运行完一个周期以上
                    _CameraGrab_S1 = true;
                    break;
                case 1://相机序号为1
                    MyTool.TxtFileProcess.CreateLog(tempLogStr + "S2请求相机抓取图像！" + "----用户：" + _operatorName,
                    _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

                    _CameraVideo_S2 = false;
                    Thread.Sleep(100);//等待相机抓取图像及显示线程运行完一个周期以上
                    _CameraGrab_S2 = true;
                    break;
                case 2://相机序号为2
                    MyTool.TxtFileProcess.CreateLog(tempLogStr + "S3请求相机抓取图像！" + "----用户：" + _operatorName,
                    _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

                    _CameraVideo_S3 = false;
                    Thread.Sleep(100);//等待相机抓取图像及显示线程运行完一个周期以上
                    _CameraGrab_S3 = true;
                    break;
            }
        }

        //maintenance camera form请求抓取图像并实时显示事件函数
        private void MaintenanceCameraFormRequestGrabImageAndDispVideoEventFunc(int cameraIndex, bool dispCrossLineFlag)
        {
            string tempLogStr = "手动设置界面";
            //if (_baseSettingFormVariate._settingsCameraCalibrationVariate.Visible)
            //    tempLogStr = "相机标定界面";

            switch (cameraIndex)
            {
                case 0://相机序号为0
                    if (_CameraVideo_S1)
                        MyTool.TxtFileProcess.CreateLog(tempLogStr + "S1请求相机停止实时显示！" + "----用户：" + _operatorName,
                        _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                    else
                        MyTool.TxtFileProcess.CreateLog(tempLogStr + "S1请求相机启动实时显示！" + "----用户：" + _operatorName,
                        _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

                    _CameraGrab_S1 = false;
                    Thread.Sleep(100);//等待相机抓取图像及显示线程运行完一个周期以上
                    _CameraVideo_S1 = !_CameraVideo_S1;
                    break;
                case 1://相机序号为1
                    if (_CameraVideo_S2)
                        MyTool.TxtFileProcess.CreateLog(tempLogStr + "S2请求相机停止实时显示！" + "----用户：" + _operatorName,
                        _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                    else
                        MyTool.TxtFileProcess.CreateLog(tempLogStr + "S2请求相机启动实时显示！" + "----用户：" + _operatorName,
                        _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

                    _CameraGrab_S2 = false;
                    Thread.Sleep(100);//等待相机抓取图像及显示线程运行完一个周期以上
                    _CameraVideo_S2 = !_CameraVideo_S2;
                    break;
                case 2://相机序号为2
                    if (_CameraVideo_S3)
                        MyTool.TxtFileProcess.CreateLog(tempLogStr + "S3请求相机停止实时显示！" + "----用户：" + _operatorName,
                        _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                    else
                        MyTool.TxtFileProcess.CreateLog(tempLogStr + "S3请求相机启动实时显示！" + "----用户：" + _operatorName,
                        _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

                    _CameraGrab_S3 = false;
                    Thread.Sleep(100);//等待相机抓取图像及显示线程运行完一个周期以上
                    _CameraVideo_S3 = !_CameraVideo_S3;
                    break;
            }
        }

        //maintenance camera form请求保存图片事件函数
        private void MaintenanceCameraFormRequestSaveImageEventFunc(int cameraIndex)
        {
            switch (cameraIndex)
            {
                case 0:
                    MyTool.TxtFileProcess.CreateLog("从手动设置界面请求S1相机保存抓取到的图片！" + "----用户：" + _operatorName,
                    _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                    break;
                case 1:
                    MyTool.TxtFileProcess.CreateLog("从手动设置界面请求S2相机保存抓取到的图片！" + "----用户：" + _operatorName,
                    _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                    break;
                case 2:
                    MyTool.TxtFileProcess.CreateLog("从手动设置界面请求S3相机保存抓取到的图片！" + "----用户：" + _operatorName,
                    _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                    break;
            }

            if (cameraIndex == 0 && _CameraCs_S1.hv_AcqHandle == null ||
                cameraIndex == 1 && _CameraCs_S2.hv_AcqHandle == null ||
                cameraIndex == 2 && _CameraCs_S3.hv_AcqHandle == null)
            {
                if (cameraIndex == 0)
                {
                    MyTool.TxtFileProcess.CreateLog("从手动设置界面请求S1相机保存抓取到的图片失败，S1相机未打开！" + "----用户：" + _operatorName,
                    _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                    MessageBox.Show("请打开S1相机！");
                    return;
                }
                if (cameraIndex == 1)
                {
                    MyTool.TxtFileProcess.CreateLog("从手动设置界面请求S2相机保存抓取到的图片失败，S2相机未打开！" + "----用户：" + _operatorName,
                    _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                    MessageBox.Show("请打开S2相机！");
                    return;
                }
                if (cameraIndex == 2)
                {
                    MyTool.TxtFileProcess.CreateLog("从手动设置界面请求S3相机保存抓取到的图片失败，S3相机未打开！" + "----用户：" + _operatorName,
                    _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                    MessageBox.Show("请打开S3相机！");
                    return;
                }
            }

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = "保存相机采集图像";
            sfd.Filter = "jpg(*.jpg)|*.jpg";
            sfd.RestoreDirectory = true;
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                switch (cameraIndex)
                {
                    case 0://相机序号为0
                        _CameraGrab_S1 = false;
                        _CameraVideo_S1 = false;
                        Thread.Sleep(100);//等待相机抓取图像及显示线程运行完一个周期以上
                        if (HalconCameraControl.SaveImage(_CameraCs_S1.ho_Image, "jpg", sfd.FileName.Substring(0, sfd.FileName.LastIndexOf("."))))//默认的filename路径为带后缀的
                        {
                            MyTool.TxtFileProcess.CreateLog("从手动设置界面请求S1相机保存抓取到的图片成功，保存路径为" + sfd.FileName + "----用户：" + _operatorName,
                            _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                            MessageBox.Show("保存S1相机图像成功！");
                        }
                        else
                        {
                            MyTool.TxtFileProcess.CreateLog("从手动设置界面请求S1相机保存抓取到的图片失败！" + "----用户：" + _operatorName,
                            _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                            MessageBox.Show("保存S1相机图像失败！");
                        }
                        break;
                    case 1://相机序号为1
                        _CameraGrab_S2 = false;
                        _CameraVideo_S2 = false;
                        Thread.Sleep(100);//等待相机抓取图像及显示线程运行完一个周期以上
                        if (HalconCameraControl.SaveImage(_CameraCs_S2.ho_Image, "jpg", sfd.FileName.Substring(0, sfd.FileName.LastIndexOf("."))))//默认的filename路径为带后缀的
                        {
                            MyTool.TxtFileProcess.CreateLog("从手动设置界面请求S2相机保存抓取到的图片成功，保存路径为" + sfd.FileName + "----用户：" + _operatorName,
                            _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                            MessageBox.Show("保存S2相机图像成功！");
                        }
                        else
                        {
                            MyTool.TxtFileProcess.CreateLog("从手动设置界面请求S2相机保存抓取到的图片失败！" + "----用户：" + _operatorName,
                            _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                            MessageBox.Show("保存S2相机图像失败！");
                        }
                        break;
                    case 2://相机序号为2
                        _CameraGrab_S3 = false;
                        _CameraVideo_S3 = false;
                        Thread.Sleep(100);//等待相机抓取图像及显示线程运行完一个周期以上
                        if (HalconCameraControl.SaveImage(_CameraCs_S3.ho_Image, "jpg", sfd.FileName.Substring(0, sfd.FileName.LastIndexOf("."))))//默认的filename路径为带后缀的
                        {
                            MyTool.TxtFileProcess.CreateLog("从手动设置界面请求S3相机保存抓取到的图片成功，保存路径为" + sfd.FileName + "----用户：" + _operatorName,
                            _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                            MessageBox.Show("保存S3相机图像成功！");
                        }
                        else
                        {
                            MyTool.TxtFileProcess.CreateLog("从手动设置界面请求S3相机保存抓取到的图片失败！" + "----用户：" + _operatorName,
                            _deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                            MessageBox.Show("保存S3相机图像失败！");
                        }
                        break;
                }
            }
        }
        #endregion

        #region btnSocketServerStart
        private void btnSocketServerStart_Click(object sender, EventArgs e)
        {
            if (!newSocketServerForROB._socketServerStartOKFlag)
            {
                newSocketServerForROB.socketServerStart("192.168.0.2", 1000);
                //ADD 202210131822
                if (newSocketServerForROB._socketServerStartOKFlag)
                    MyTool.TxtFileProcess.CreateLog("服务器端成功开启" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                else
                    MyTool.TxtFileProcess.CreateLog("服务器端开启失败!" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
            }
        }
        #endregion

        #region btnSocketServerClose
        private void btnSocketServerClose_Click(object sender, EventArgs e)
        {
            newSocketServerForROB.socketServerClose();
            //ADD 202210131822
            if (!newSocketServerForROB._socketServerStartOKFlag)
                MyTool.TxtFileProcess.CreateLog("服务器端关闭成功" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
            else
                MyTool.TxtFileProcess.CreateLog("服务器端关闭失败!" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
        }
        #endregion

        #region btnDebug
        bool _btnDebugSendFlag_S1 = false;
        bool _btnDebugSendFlag_S2 = false;
        bool _btnDebugSendFlag_S3 = false;
        private void btnDebug_S1_Click(object sender, EventArgs e)
        {
            if (ROBPCCommunicationStatus)
            {
                if (ROBNewPickupPositionCalculate_S(1))
                    _btnDebugSendFlag_S1 = true;
            }
            else
            {
                MessageBox.Show("没有与ROB连接!");
            }
        }
        private void btnDebug_S2_Click(object sender, EventArgs e)
        {
            if (ROBPCCommunicationStatus)
            {
                if (ROBNewPickupPositionCalculate_S(2))
                    _btnDebugSendFlag_S2 = true;
            }
            else
            {
                MessageBox.Show("没有与ROB连接!");
            }
        }

        private void btnDebug_S3_Click(object sender, EventArgs e)
        {
            if (ROBPCCommunicationStatus)
            {
                if (ROBNewPickupPositionCalculate_S(3))
                    _btnDebugSendFlag_S3 = true;
            }
            else
            {
                MessageBox.Show("没有与ROB连接!");
            }
        }
        #endregion

    }
}
