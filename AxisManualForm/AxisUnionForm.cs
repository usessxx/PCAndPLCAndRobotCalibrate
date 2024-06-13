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
using SoftKeyBoard;
using CSVFile;
using System.Diagnostics;//用于stopwatch
using System.IO;

namespace AxisAndIOForm
{
    public delegate void AxisUnionManualDataChangeDelegate();//声明委托—轴运行InchFlag切换
    public delegate void AxisUnionFormCloseRequestDelegate();//声明委托—窗口关闭
    public delegate void AxisUnionFormAxisPositionDataModifiedDelegate();//声明委托-点位参数发生变化
    public delegate void AxisUnionFormAxisUnionMoveDelegate(bool[] actionAxisFlag, int targetPointNumber);//声明委托-轴联合运动
    public delegate void AxisUnionFormJogInchMoveDelegate(int axisIndex, bool moveDirection, bool stopFlag);//声明委托-轴JOG/INCH运动
    public partial class AxisUnionForm : Form
    {
        /*--------------------------------------------------------------------------------------
       //Copyright (C) 2022 深圳市利器精工科技有限公司
       //版权所有
       //
       //文件名：轴联合动作窗口
       //文件功能描述：联合控制轴的动作
       //
       //
       //创建标识：MaLi 20220310
       //
       //修改标识：MaLi 20220310 Change
       //修改描述：增加轴联合动作窗口
       //
       //
       //------------------------------------------------------------------------------------*/
        public event AxisUnionManualDataChangeDelegate ManualDataChangedEvent;//声明事件—轴运行InchFlag切换
        public event AxisUnionFormCloseRequestDelegate FormCloseRequestEvent;//声明事件—窗口关闭
        public event AxisUnionFormAxisPositionDataModifiedDelegate AxisPositionDataModifiedEvent;//声明事件-点位参数发生变化
        public event AxisUnionFormAxisUnionMoveDelegate AxisUnionMoveEvent;//声明事件-轴联合移动
        public event AxisUnionFormJogInchMoveDelegate JogInchMoveEvent;//声明事件-轴Jog/Inch移动事件

        #region 变量

        const int AXIS_QUANTITY = 4;//此画面能显示的轴个数
        public int _maxPointQuantity;//轴点位个数上限 
        
        //*************************外部可设定参数*******************************//
        public double[] _axisCurrentPosition_mm = new double[AXIS_QUANTITY];//当前位置_mm
        public string[] _axisUnitArray = new string[AXIS_QUANTITY] { "mm", "mm", "°", "mm" };//轴单位数组，为mm或°


        //软负极限
        public float[] _axisSoftLimitMinus = new float[AXIS_QUANTITY] { 0f, 0f, 0f, 0f };
        //软正极限
        public float[] _axisSoftLimitPlus = new float[AXIS_QUANTITY] { 500f, 500f, 100f, 100f };

        //JogMinSpeed
        public float[] _axisJogMinSpeed_mmps = new float[AXIS_QUANTITY];
        //JogMaxSpeed
        public float[] _axisJogMaxSpeed_mmps = new float[AXIS_QUANTITY];
        //InchMinDistance
        public float[] _axisInchMinDistance_mm = new float[AXIS_QUANTITY];
        //InchMaxDistance
        public float[] _axisInchMaxDistance_mm = new float[AXIS_QUANTITY];

        //MoveMinSpeed
        public float[] _axisMoveMinSpeed_mmps = new float[AXIS_QUANTITY];
        //MoveMaxSpeed
        public float[] _axisMoveMaxSpeed_mmps = new float[AXIS_QUANTITY];
        //MoveminACCTime
        public float[] _axisMoveMinAccTime_s = new float[AXIS_QUANTITY];
        //MoveMaxACCTime
        public float[] _axisMoveMaxAccTime_s = new float[AXIS_QUANTITY];
        //MoveminDecTime
        public float[] _axisMoveMinDecTime_s = new float[AXIS_QUANTITY];
        //MoveMaxDecTime
        public float[] _axisMoveMaxDecTime_s = new float[AXIS_QUANTITY];
        //AxisMoveStartVelocity
        public float[] _axisMoveStartVelocity_mmps = new float[AXIS_QUANTITY];
        //AxisMoveStopVelocity
        public float[] _axisMoveStopVelocity_mmps = new float[AXIS_QUANTITY];

        public int[] _axisPointQuantity = new int[AXIS_QUANTITY];//轴点位个数

        public string _currentProductName = "Default";//当前产品名称
        public bool _motionCardInitialFlag = false;//运动控制卡初始化Flag，false-initial失败，true

        public float[] _axisJogInchVelocity_mmps = new float[AXIS_QUANTITY] { 10, 10, 10, 10 };//轴JOG/INCH的速度
        public float[] _axisInchDistance_mm = new float[AXIS_QUANTITY] { 1, 1, 1, 1 };//轴INCH的距离

        public bool[] _axisInchFlag = new bool[AXIS_QUANTITY];//轴寸动FLAG，false-点动，true-寸动

        public bool _deviceAutoModeFlag = false;//轴自动模式标志，false-非自动模式，true-自动模式
        public bool[] _axisHomedFlag = new bool[AXIS_QUANTITY];//轴原点复归完成标志，false-未完成，true-完成

        public float[] _absoluteCoor = new float[AXIS_QUANTITY]{0,0,0,0};//轴绝对坐标
        public bool _useRelativeFlag = false;//使用相对坐标标志，false-不使用，true-使用
        //*************************内部私有变量*******************************//
        private bool[] _axisUnionFlag = new bool[AXIS_QUANTITY];//轴联合FLAG，false-不联合，true-联合

        DataTable[] _axisPointDataDT = new DataTable[AXIS_QUANTITY];//用于读取存取轴点位数据的DataTable

        // 创建the ToolTip 
        ToolTip _toolTip = new ToolTip();

        Stopwatch _motionCardErrorLampBlinkSW = null;//用于控制运动控制卡错误时的闪烁

        //状态更新线程
        private Thread _statusUpdateThread = null;

        int _selectPointNumber = 1;//选中点位号
        //
        #endregion

        #region AxisUnionForm
        public AxisUnionForm(bool[] inchFlag, float[] jogInchVelocity_mmps, float[] jogInchDistance_mm, int[] pointQuantity, string currentProductName)
        {
            InitializeComponent();

            #region
            _maxPointQuantity = AxisControlMainForm._maxPointQuantity;//轴点位个数上限
            #endregion

            _currentProductName = currentProductName;
            for (int i = 0; i < AXIS_QUANTITY; i++)
            {
                _axisInchFlag[i] = inchFlag[i];
                _axisJogInchVelocity_mmps[i] = jogInchVelocity_mmps[i];
                _axisInchDistance_mm[i] = jogInchDistance_mm[i];
                _axisPointQuantity[i] = pointQuantity[i];
            }
        }
        #endregion

        #region MaintenanceMotorUnionForm_FormClosing
        private void MaintenanceMotorUnionForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            if (_statusUpdateThread != null)
            {
                _statusUpdateThread.Abort();
                _statusUpdateThread = null;
                Thread.Sleep(100);
            }
            if (FormCloseRequestEvent != null)
                FormCloseRequestEvent();
        }
        #endregion

        #region MaintenanceMotorUnionForm_Load
        private void MaintenanceMotorUnionForm_Load(object sender, EventArgs e)
        {
            AxisPosFileInitial(_currentProductName);

            //状态更新线程启动
            _statusUpdateThread = new Thread(StatusUpdateTD);
            _statusUpdateThread.IsBackground = true;
            _statusUpdateThread.Start();

            txtPointNumberSet.Text = _selectPointNumber.ToString("000");//设置选中点位号

            RefreshManualCtrlData();
        }
        #endregion

        #region 状态更新线程
        private void StatusUpdateTD()
        {
            while (true)
            {
                if (_motionCardInitialFlag)//如果运动控制卡启动成功
                {
                    lblAxisMotionCardStatus.BeginInvoke(new Action(() =>
                    {
                        lblAxisMotionCardStatus.ForeColor = Color.LimeGreen;
                        lblAxisMotionCardStatus.BackColor = SystemColors.Control;
                        lblAxisMotionCardStatus.Text = "运动控制卡正常";
                    })); ;
                }
                else
                {
                    if (_motionCardErrorLampBlinkSW == null)
                    {
                        _motionCardErrorLampBlinkSW = new Stopwatch();
                        _motionCardErrorLampBlinkSW.Start();
                    }
                    lblAxisMotionCardStatus.BeginInvoke(new Action(() =>
                        {
                            lblAxisMotionCardStatus.Text = "运动控制卡异常";
                            if (Convert.ToInt32(_motionCardErrorLampBlinkSW.ElapsedMilliseconds) >= 0 && Convert.ToInt32(_motionCardErrorLampBlinkSW.ElapsedMilliseconds) < 1000)//如果计时未超过1s
                            {
                                lblAxisMotionCardStatus.ForeColor = Color.Red;
                                lblAxisMotionCardStatus.BackColor = SystemColors.Control;
                            }
                            else if (Convert.ToInt32(_motionCardErrorLampBlinkSW.ElapsedMilliseconds) >= 1000)//如果计时未超过1s
                            {
                                lblAxisMotionCardStatus.ForeColor = Color.White;
                                lblAxisMotionCardStatus.BackColor = Color.Red;
                                if (Convert.ToInt32(_motionCardErrorLampBlinkSW.ElapsedMilliseconds) > 2000)
                                    _motionCardErrorLampBlinkSW.Restart();
                            }
                        })); ;
                }

                this.Invoke(new Action(() =>
                    {
                        if (true)
                        {
                            #region 更新Label控件显示


                            //更新当前位置Title
                            lblAxis01CurrentPosition.Text = "当前位置(" + _axisUnitArray[0] + ")";
                            lblAxis02CurrentPosition.Text = "当前位置(" + _axisUnitArray[1] + ")";
                            lblAxis03CurrentPosition.Text = "当前位置(" + _axisUnitArray[2] + ")";
                            lblAxis04CurrentPosition.Text = "当前位置(" + _axisUnitArray[3] + ")";

                            //更新JOG INCH速度Title
                            lblAxis01JogInchVelocity.Text = "速度(" + _axisUnitArray[0] + "/s)";
                            lblAxis02JogInchVelocity.Text = "速度(" + _axisUnitArray[1] + "/s)";
                            lblAxis03JogInchVelocity.Text = "速度(" + _axisUnitArray[2] + "/s)";
                            lblAxis04JogInchVelocity.Text = "速度(" + _axisUnitArray[3] + "/s)";

                            //更新寸动距离Title
                            lblAxis01InchDistance.Text = "寸动长度(" + _axisUnitArray[0] + ")";
                            lblAxis02InchDistance.Text = "寸动长度(" + _axisUnitArray[1] + ")";
                            lblAxis03InchDistance.Text = "寸动长度(" + _axisUnitArray[2] + ")";
                            lblAxis04InchDistance.Text = "寸动长度(" + _axisUnitArray[3] + ")";

                            //更新保存的位置Titile
                            lblAxis01SavedPosition.Text = "保存的位置(" + _axisUnitArray[0] + ")";
                            lblAxis02SavedPosition.Text = "保存的位置(" + _axisUnitArray[1] + ")";
                            lblAxis03SavedPosition.Text = "保存的位置(" + _axisUnitArray[2] + ")";
                            lblAxis04SavedPosition.Text = "保存的位置(" + _axisUnitArray[3] + ")";

                            //更新保存的速度Title
                            lblAxis01SavedVelocity.Text = "保存的速度(" + _axisUnitArray[0] + "/s)";
                            lblAxis02SavedVelocity.Text = "保存的速度(" + _axisUnitArray[1] + "/s)";
                            lblAxis03SavedVelocity.Text = "保存的速度(" + _axisUnitArray[2] + "/s)";
                            lblAxis04SavedVelocity.Text = "保存的速度(" + _axisUnitArray[3] + "/s)";

                            //更新修改的位置Titile
                            lblAxis01ModifyPosition.Text = "修改的位置(" + _axisUnitArray[0] + ")";
                            lblAxis02ModifyPosition.Text = "修改的位置(" + _axisUnitArray[1] + ")";
                            lblAxis03ModifyPosition.Text = "修改的位置(" + _axisUnitArray[2] + ")";
                            lblAxis04ModifyPosition.Text = "修改的位置(" + _axisUnitArray[3] + ")";

                            //更新修改的速度Title
                            lblAxis01ModifyVelocity.Text = "修改的速度(" + _axisUnitArray[0] + "/s)";
                            lblAxis02ModifyVelocity.Text = "修改的速度(" + _axisUnitArray[1] + "/s)";
                            lblAxis03ModifyVelocity.Text = "修改的速度(" + _axisUnitArray[2] + "/s)";
                            lblAxis04ModifyVelocity.Text = "修改的速度(" + _axisUnitArray[3] + "/s)";

                            #endregion
                        }
                        #region 更新Textbox控件显示数据
                        //更新当前位置
                        txtAxis01CurrentPosition.Text = _axisCurrentPosition_mm[0].ToString("f2");
                        txtAxis02CurrentPosition.Text = _axisCurrentPosition_mm[1].ToString("f2");
                        txtAxis03CurrentPosition.Text = _axisCurrentPosition_mm[2].ToString("f2");
                        txtAxis04CurrentPosition.Text = _axisCurrentPosition_mm[3].ToString("f2");

                        #endregion

                        #region 更新Button控件Text
                        //更新点动寸动按钮
                        if (_axisInchFlag[0] && btnAxis01JogOrInchSelector.Text != "寸动")
                        {
                            btnAxis01JogOrInchSelector.BackColor = Color.Yellow;
                            btnAxis01JogOrInchSelector.ForeColor = Color.Black;
                            btnAxis01JogOrInchSelector.Text = "寸动";
                        }
                        else if (!_axisInchFlag[0] && btnAxis01JogOrInchSelector.Text != "点动")
                        {
                            btnAxis01JogOrInchSelector.BackColor = Color.Blue;
                            btnAxis01JogOrInchSelector.ForeColor = Color.White;
                            btnAxis01JogOrInchSelector.Text = "点动";
                        }

                        if (_axisInchFlag[1] && btnAxis02JogOrInchSelector.Text != "寸动")
                        {
                            btnAxis02JogOrInchSelector.BackColor = Color.Yellow;
                            btnAxis02JogOrInchSelector.ForeColor = Color.Black;
                            btnAxis02JogOrInchSelector.Text = "寸动";
                        }
                        else if (!_axisInchFlag[1] && btnAxis02JogOrInchSelector.Text != "点动")
                        {
                            btnAxis02JogOrInchSelector.BackColor = Color.Blue;
                            btnAxis02JogOrInchSelector.ForeColor = Color.White;
                            btnAxis02JogOrInchSelector.Text = "点动";
                        }

                        if (_axisInchFlag[2] && btnAxis03JogOrInchSelector.Text != "寸动")
                        {
                            btnAxis03JogOrInchSelector.BackColor = Color.Yellow;
                            btnAxis03JogOrInchSelector.ForeColor = Color.Black;
                            btnAxis03JogOrInchSelector.Text = "寸动";
                        }
                        else if (!_axisInchFlag[2] && btnAxis03JogOrInchSelector.Text != "点动")
                        {
                            btnAxis03JogOrInchSelector.BackColor = Color.Blue;
                            btnAxis03JogOrInchSelector.ForeColor = Color.White;
                            btnAxis03JogOrInchSelector.Text = "点动";
                        }

                        if (_axisInchFlag[3] && btnAxis04JogOrInchSelector.Text != "寸动")
                        {
                            btnAxis04JogOrInchSelector.BackColor = Color.Yellow;
                            btnAxis04JogOrInchSelector.ForeColor = Color.Black;
                            btnAxis04JogOrInchSelector.Text = "寸动";
                        }
                        else if (!_axisInchFlag[3] && btnAxis04JogOrInchSelector.Text != "点动")
                        {
                            btnAxis04JogOrInchSelector.BackColor = Color.Blue;
                            btnAxis04JogOrInchSelector.ForeColor = Color.White;
                            btnAxis04JogOrInchSelector.Text = "点动";
                        }

                        //更新union flag状态
                        if (_axisUnionFlag[0] && btnAxis01UnionFlag.Text != "联合")
                        {
                            btnAxis01UnionFlag.Text = "联合";
                            btnAxis01UnionFlag.ForeColor = Color.Black;
                            btnAxis01UnionFlag.BackColor = Color.LimeGreen;
                        }
                        else if (!_axisUnionFlag[0] && btnAxis01UnionFlag.Text != "不联合")
                        {
                            btnAxis01UnionFlag.Text = "不联合";
                            btnAxis01UnionFlag.ForeColor = Color.White;
                            btnAxis01UnionFlag.BackColor = SystemColors.ControlDark;
                        }

                        if (_axisUnionFlag[1] && btnAxis02UnionFlag.Text != "联合")
                        {
                            btnAxis02UnionFlag.Text = "联合";
                            btnAxis02UnionFlag.ForeColor = Color.Black;
                            btnAxis02UnionFlag.BackColor = Color.LimeGreen;
                        }
                        else if (!_axisUnionFlag[1] && btnAxis02UnionFlag.Text != "不联合")
                        {
                            btnAxis02UnionFlag.Text = "不联合";
                            btnAxis02UnionFlag.ForeColor = Color.White;
                            btnAxis02UnionFlag.BackColor = SystemColors.ControlDark;
                        }

                        if (_axisUnionFlag[2] && btnAxis03UnionFlag.Text != "联合")
                        {
                            btnAxis03UnionFlag.Text = "联合";
                            btnAxis03UnionFlag.ForeColor = Color.Black;
                            btnAxis03UnionFlag.BackColor = Color.LimeGreen;
                        }
                        else if (!_axisUnionFlag[2] && btnAxis03UnionFlag.Text != "不联合")
                        {
                            btnAxis03UnionFlag.Text = "不联合";
                            btnAxis03UnionFlag.ForeColor = Color.White;
                            btnAxis03UnionFlag.BackColor = SystemColors.ControlDark;
                        }

                        if (_axisUnionFlag[3] && btnAxis04UnionFlag.Text != "联合")
                        {
                            btnAxis04UnionFlag.Text = "联合";
                            btnAxis04UnionFlag.ForeColor = Color.Black;
                            btnAxis04UnionFlag.BackColor = Color.LimeGreen;
                        }
                        else if (!_axisUnionFlag[3] && btnAxis04UnionFlag.Text != "不联合")
                        {
                            btnAxis04UnionFlag.Text = "不联合";
                            btnAxis04UnionFlag.ForeColor = Color.White;
                            btnAxis04UnionFlag.BackColor = SystemColors.ControlDark;
                        }
                        #endregion
                    })); ;


                Thread.Sleep(100);
            }
        }
        #endregion

        #region ControlHideFunc
        public void ControlHideFunc(bool[] axisUseFlag, string[] axisName)
        {
            #region 根据轴使用与否设置控件的显示隐藏
            for (int i = 0; i < AXIS_QUANTITY; i++)
            {
                string ctrlName = "";
                object tempCtrl = null;
                if (axisUseFlag[i])
                {
                    ctrlName = "gbxAxis" + (i + 1).ToString("00");
                    tempCtrl = this.GetType().GetField(ctrlName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);
                    if (!((GroupBox)tempCtrl).Visible)
                        ((GroupBox)tempCtrl).Visible = true;

                    ctrlName = "btnAxis" + (i + 1).ToString("00") + "MinusMove";
                    tempCtrl = this.GetType().GetField(ctrlName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);
                    if (!((UCArrow.UCArrow)tempCtrl).Visible)
                        ((UCArrow.UCArrow)tempCtrl).Visible = true;

                    ctrlName = "btnAxis" + (i + 1).ToString("00") + "PlusMove";
                    tempCtrl = this.GetType().GetField(ctrlName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);
                    if (!((UCArrow.UCArrow)tempCtrl).Visible)
                        ((UCArrow.UCArrow)tempCtrl).Visible = true;


                    ctrlName = "lblAxis" + (i + 1).ToString("00") + "PointNameTitle";
                    tempCtrl = this.GetType().GetField(ctrlName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);
                    if (!((Label)tempCtrl).Visible)
                        ((Label)tempCtrl).Visible = true;

                    ctrlName = "txtAxis" + (i + 1).ToString("00") + "SelectedPointNameChange";
                    tempCtrl = this.GetType().GetField(ctrlName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);
                    if (!((TextBox)tempCtrl).Visible)
                        ((TextBox)tempCtrl).Visible = true;

                }
                else
                {
                    ctrlName = "gbxAxis" + (i + 1).ToString("00");
                    tempCtrl = this.GetType().GetField(ctrlName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);
                    if (((GroupBox)tempCtrl).Visible)
                        ((GroupBox)tempCtrl).Visible = false;

                    ctrlName = "btnAxis" + (i + 1).ToString("00") + "MinusMove";
                    tempCtrl = this.GetType().GetField(ctrlName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);
                    if (((UCArrow.UCArrow)tempCtrl).Visible)
                        ((UCArrow.UCArrow)tempCtrl).Visible = false;

                    ctrlName = "btnAxis" + (i + 1).ToString("00") + "PlusMove";
                    tempCtrl = this.GetType().GetField(ctrlName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);
                    if (((UCArrow.UCArrow)tempCtrl).Visible)
                        ((UCArrow.UCArrow)tempCtrl).Visible = false;


                    ctrlName = "lblAxis" + (i + 1).ToString("00") + "PointNameTitle";
                    tempCtrl = this.GetType().GetField(ctrlName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);
                    if (((Label)tempCtrl).Visible)
                        ((Label)tempCtrl).Visible = false;

                    ctrlName = "txtAxis" + (i + 1).ToString("00") + "SelectedPointNameChange";
                    tempCtrl = this.GetType().GetField(ctrlName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);
                    if (((TextBox)tempCtrl).Visible)
                        ((TextBox)tempCtrl).Visible = false;
                }
            }
            #endregion

            //更新groupbox的名称
            gbxAxis01.Text = "轴01-" + axisName[0];
            gbxAxis02.Text = "轴02-" + axisName[1];
            gbxAxis03.Text = "轴03-" + axisName[2];
            gbxAxis04.Text = "轴04-" + axisName[3];
        }
        #endregion

        #region AxisPosFileInitial:轴点位数据初始化
        public void AxisPosFileInitial(string currentProductName)
        {
            string axisPositionFilePath; //轴参数文件保存路径
            string axisPositionFileSaveFolderPath;//轴参数保存文件夹路径
            DataTable axisPositionDataDT = null;
            _currentProductName = currentProductName;
            axisPositionFileSaveFolderPath = Directory.GetCurrentDirectory() + @"\" +
                MyTool.FolderAndFileManageClass.FilterPathIllegalChar("AxisParameterAndPositionData_" + _currentProductName) + @"\";//过滤掉作为Directory不合法的字符
            if (!Directory.Exists(axisPositionFileSaveFolderPath))
            {
                Directory.CreateDirectory(axisPositionFileSaveFolderPath);//创建文件夹
            }

            for (int i = 0; i < AXIS_QUANTITY; i++)
            {
                axisPositionFilePath = Path.Combine(axisPositionFileSaveFolderPath,
                    MyTool.FolderAndFileManageClass.FilterPathIllegalChar("axis" + i.ToString("00") + "Pos" + "_" + _currentProductName)
                    + ".csv");
                if (!File.Exists(axisPositionFilePath))
                {
                    File.Create(axisPositionFilePath).Close();//创建文件
                    //初始化文件内容
                    CSVFileOperation.SaveCSV_Append(axisPositionFilePath, "PointNumber,Position(mm),Speed(mm/s),AccTime(s),Deviation(mm),联合(0/1),名字_中文,Name_English");
                    for (int j = 1; j <= _axisPointQuantity[i]; j++)
                    {
                        CSVFileOperation.SaveCSV_Append(axisPositionFilePath, j.ToString("000") + ",0.00,100,1,0," + "0," + "预留,Spare");
                    }
                    MessageBox.Show("轴" + i.ToString("00") + "点位文件不存在,已创建默认轴" + i.ToString("00") + "点位文件!请根据实际情况修改此文件!");
                }

                //读取数据至DT中
                axisPositionDataDT = CSVFileOperation.ReadCSV(axisPositionFilePath);//不包含第一行
                if (axisPositionDataDT.Rows.Count < _axisPointQuantity[i])//如果点位个数小于设定的点位个数
                {
                    MessageBox.Show("轴" + i.ToString("00") + "点位文件中读取的点位数据个数小于设定的轴点位个数,利用默认数据填充，请根据实际情况修改此文件!");
                    int sourceDTRowCount = axisPositionDataDT.Rows.Count;
                    for (int j = 0; j < _axisPointQuantity[i] - sourceDTRowCount; j++)
                    {
                        DataRow dr = axisPositionDataDT.NewRow();
                        dr[0] = (sourceDTRowCount + j + 1).ToString("000");
                        dr[1] = "0.00";
                        dr[2] = "100";
                        dr[3] = "1";
                        dr[4] = "0";
                        dr[5] = "0";
                        dr[6] = "预留";
                        dr[7] = "Spare";
                        axisPositionDataDT.Rows.Add(dr.ItemArray);
                    }
                }
                DataTable finalAxisPositionDataDT = new DataTable();
                for (int j = 0; j < axisPositionDataDT.Columns.Count; j++)
                {
                    finalAxisPositionDataDT.Columns.Add(axisPositionDataDT.Columns[j].ColumnName);
                }
                for (int j = 0; j < _axisPointQuantity[i]; j++)
                {
                    DataRow dr = axisPositionDataDT.Rows[j];
                    finalAxisPositionDataDT.Rows.Add(dr.ItemArray);
                }
                _axisPointDataDT[i] = finalAxisPositionDataDT;
            }

            RefreshPositionDataAndPointNameToCtr();
        }
        #endregion

        #region RefreshPositionDataAndPointNameToCtrl:更新点位数据及点位名称至控件
        private void RefreshPositionDataAndPointNameToCtr()
        {
            string ctrlName = "";
            object tempCtrl = null;
            for (int i = 0; i < AXIS_QUANTITY; i++)
            {
                if (_axisPointQuantity[i] < _selectPointNumber || _axisPointDataDT[i].Rows.Count < _selectPointNumber || _selectPointNumber < 1 || _selectPointNumber > _maxPointQuantity)
                {
                    //更新轴保存的位置数据
                    ctrlName = "txtAxis" + (i + 1).ToString("00") + "SavedPosition";
                    tempCtrl = this.GetType().GetField(ctrlName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);
                    ((TextBox)tempCtrl).Text = "0.00";
                    //更新轴保存的速度数据
                    ctrlName = "txtAxis" + (i + 1).ToString("00") + "SavedVelocity";
                    tempCtrl = this.GetType().GetField(ctrlName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);
                    ((TextBox)tempCtrl).Text = "0";
                    //更新轴保存的加速度数据
                    ctrlName = "txtAxis" + (i + 1).ToString("00") + "SavedAcc";
                    tempCtrl = this.GetType().GetField(ctrlName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);
                    ((TextBox)tempCtrl).Text = "0.00";

                    //更新轴修改的位置数据
                    ctrlName = "txtAxis" + (i + 1).ToString("00") + "ModifyPosition";
                    tempCtrl = this.GetType().GetField(ctrlName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);
                    ((TextBox)tempCtrl).Text = "0.00";
                    //更新轴修改的速度数据
                    ctrlName = "txtAxis" + (i + 1).ToString("00") + "ModifyVelocity";
                    tempCtrl = this.GetType().GetField(ctrlName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);
                    ((TextBox)tempCtrl).Text = "0";
                    //更新轴修改的加速度数据
                    ctrlName = "txtAxis" + (i + 1).ToString("00") + "ModifyAcc";
                    tempCtrl = this.GetType().GetField(ctrlName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);
                    ((TextBox)tempCtrl).Text = "0.00";

                    //更新位置名
                    ctrlName = "txtAxis" + (i + 1).ToString("00") + "SelectedPointNameChange";
                    tempCtrl = this.GetType().GetField(ctrlName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);
                    ((TextBox)tempCtrl).Text = "无";

                    //更新联动Flag
                    _axisUnionFlag[i] = false;
                }
                else
                {
                    //更新轴保存的位置数据
                    ctrlName = "txtAxis" + (i + 1).ToString("00") + "SavedPosition";
                    tempCtrl = this.GetType().GetField(ctrlName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);
                    ((TextBox)tempCtrl).Text = _axisPointDataDT[i].Rows[_selectPointNumber - 1][1].ToString();
                    //更新轴保存的速度数据
                    ctrlName = "txtAxis" + (i + 1).ToString("00") + "SavedVelocity";
                    tempCtrl = this.GetType().GetField(ctrlName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);
                    ((TextBox)tempCtrl).Text = _axisPointDataDT[i].Rows[_selectPointNumber - 1][2].ToString();
                    //更新轴保存的加速度数据
                    ctrlName = "txtAxis" + (i + 1).ToString("00") + "SavedAcc";
                    tempCtrl = this.GetType().GetField(ctrlName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);
                    ((TextBox)tempCtrl).Text = _axisPointDataDT[i].Rows[_selectPointNumber - 1][3].ToString();

                    //更新轴修改的位置数据
                    ctrlName = "txtAxis" + (i + 1).ToString("00") + "ModifyPosition";
                    tempCtrl = this.GetType().GetField(ctrlName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);
                    ((TextBox)tempCtrl).Text = _axisPointDataDT[i].Rows[_selectPointNumber - 1][1].ToString();
                    //更新轴修改的速度数据
                    ctrlName = "txtAxis" + (i + 1).ToString("00") + "ModifyVelocity";
                    tempCtrl = this.GetType().GetField(ctrlName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);
                    ((TextBox)tempCtrl).Text = _axisPointDataDT[i].Rows[_selectPointNumber - 1][2].ToString();
                    //更新轴修改的加速度数据
                    ctrlName = "txtAxis" + (i + 1).ToString("00") + "ModifyAcc";
                    tempCtrl = this.GetType().GetField(ctrlName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);
                    ((TextBox)tempCtrl).Text = _axisPointDataDT[i].Rows[_selectPointNumber - 1][3].ToString();

                    //更新位置名
                    ctrlName = "txtAxis" + (i + 1).ToString("00") + "SelectedPointNameChange";
                    tempCtrl = this.GetType().GetField(ctrlName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);
                    ((TextBox)tempCtrl).Text = _axisPointDataDT[i].Rows[_selectPointNumber - 1][6].ToString();

                    //更新联动Flag
                    ctrlName = "btnAxis" + (i + 1).ToString("00") + "UnionFlag";
                    tempCtrl = this.GetType().GetField(ctrlName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);
                    if (_axisPointDataDT[i].Rows[_selectPointNumber - 1][5].ToString() == "1")
                        _axisUnionFlag[i] = true;
                    else
                        _axisUnionFlag[i] = false;
                }
            }
        }
        #endregion

        #region 关于手动控制控件中数据变化后，数据检查
        //Textbox数据普通检查函数，正数，浮点数检查
        private void ManualCtrlDataCheckEvent(object sender, EventArgs e)
        {
            int textBoxIndex = Convert.ToInt32(((TextBox)sender).Name.Substring(7, 2)) - 1;
            string textBoxNameFlag = ((TextBox)sender).Name.Substring(9, ((TextBox)sender).Name.Length - 9);

            if (textBoxNameFlag == "JogInchVelocity")
            {
                ((TextBox)sender).Text = MyTool.DataProcessing.TextBoxDataCheck(((TextBox)sender).Text, _axisJogMinSpeed_mmps[textBoxIndex], _axisJogMaxSpeed_mmps[textBoxIndex],
                    true, true, true, false);
            }
            else if (textBoxNameFlag == "InchDistance")
            {
                ((TextBox)sender).Text = MyTool.DataProcessing.TextBoxDataCheck(((TextBox)sender).Text, _axisInchMinDistance_mm[textBoxIndex], _axisInchMaxDistance_mm[textBoxIndex],
                    true, true, true, false);
            }
            else if (textBoxNameFlag == "ModifyPosition")
            {
                ((TextBox)sender).Text = MyTool.DataProcessing.TextBoxDataCheck(((TextBox)sender).Text, _axisSoftLimitMinus[textBoxIndex], _axisSoftLimitPlus[textBoxIndex],
                    true, true, true, false);
            }
            else if (textBoxNameFlag == "ModifyVelocity")
            {
                ((TextBox)sender).Text = MyTool.DataProcessing.TextBoxDataCheck(((TextBox)sender).Text, _axisMoveMinSpeed_mmps[textBoxIndex], _axisMoveMaxSpeed_mmps[textBoxIndex],
                    true, true, true, false);
            }
            else if (textBoxNameFlag == "ModifyAcc")
            {
                ((TextBox)sender).Text = MyTool.DataProcessing.TextBoxDataCheck(((TextBox)sender).Text, _axisMoveMinAccTime_s[textBoxIndex], _axisMoveMaxAccTime_s[textBoxIndex],
                    true, true, true, false);
            }

            ((TextBox)sender).SelectionStart = ((TextBox)sender).TextLength;
            ((TextBox)sender).SelectionLength = 0;

        }

        //Textbox数据最后一次检查函数，正数，浮点数，整数检查
        private void ManualCtrlDataCheckLeaveEvent(object sender, EventArgs e)
        {
            int textBoxIndex = Convert.ToInt32(((TextBox)sender).Name.Substring(7, 2)) - 1;
            string textBoxNameFlag = ((TextBox)sender).Name.Substring(9, ((TextBox)sender).Name.Length - 9);

            if (textBoxNameFlag == "JogInchVelocity")
            {
                ((TextBox)sender).Text = MyTool.DataProcessing.TextBoxDataCheck(((TextBox)sender).Text, _axisJogMinSpeed_mmps[textBoxIndex], _axisJogMaxSpeed_mmps[textBoxIndex],
                    true, true, true, true);
                _axisJogInchVelocity_mmps[textBoxIndex] = Convert.ToSingle(((TextBox)sender).Text);
            }
            else if (textBoxNameFlag == "InchDistance")
            {
                ((TextBox)sender).Text = MyTool.DataProcessing.TextBoxDataCheck(((TextBox)sender).Text, _axisInchMinDistance_mm[textBoxIndex], _axisInchMaxDistance_mm[textBoxIndex],
                    true, true, true, true);
                _axisInchDistance_mm[textBoxIndex] = Convert.ToSingle(((TextBox)sender).Text);
            }
            else if (textBoxNameFlag == "ModifyPosition")
            {
                ((TextBox)sender).Text = MyTool.DataProcessing.TextBoxDataCheck(((TextBox)sender).Text, _axisSoftLimitMinus[textBoxIndex], _axisSoftLimitPlus[textBoxIndex],
                    true, true, true, true);
            }
            else if (textBoxNameFlag == "ModifyVelocity")
            {
                ((TextBox)sender).Text = MyTool.DataProcessing.TextBoxDataCheck(((TextBox)sender).Text, _axisMoveMinSpeed_mmps[textBoxIndex], _axisMoveMaxSpeed_mmps[textBoxIndex],
                    true, true, true, true);
            }
            else if (textBoxNameFlag == "ModifyAcc")
            {
                ((TextBox)sender).Text = MyTool.DataProcessing.TextBoxDataCheck(((TextBox)sender).Text, _axisMoveMinAccTime_s[textBoxIndex], _axisMoveMaxAccTime_s[textBoxIndex],
                    true, true, true, true);
            }


            ((TextBox)sender).SelectionStart = ((TextBox)sender).TextLength;
            ((TextBox)sender).SelectionLength = 0;

            if (textBoxNameFlag == "JogInchVelocity" || textBoxNameFlag == "InchDistance")
            {
                //如果切换那么触发事件
                if (ManualDataChangedEvent != null)
                {
                    ManualDataChangedEvent();
                }
            }

        }
        #endregion

        #region RefreshManualCtrlData:更新手动控件数据
        public void RefreshManualCtrlData()
        {
            //更新JOG INCH速度
            txtAxis01JogInchVelocity.Text = _axisJogInchVelocity_mmps[0].ToString("f2");
            txtAxis02JogInchVelocity.Text = _axisJogInchVelocity_mmps[1].ToString("f2");
            txtAxis03JogInchVelocity.Text = _axisJogInchVelocity_mmps[2].ToString("f2");
            txtAxis04JogInchVelocity.Text = _axisJogInchVelocity_mmps[3].ToString("f2");

            //更新INCH距离
            txtAxis01InchDistance.Text = _axisInchDistance_mm[0].ToString("f2");
            txtAxis02InchDistance.Text = _axisInchDistance_mm[1].ToString("f2");
            txtAxis03InchDistance.Text = _axisInchDistance_mm[2].ToString("f2");
            txtAxis04InchDistance.Text = _axisInchDistance_mm[3].ToString("f2");
        }
        #endregion

        #region Union flag切换事件
        private void UnionFlagChangeEvent(object sender, EventArgs e)
        {
            int buttonIndex = Convert.ToInt32(((Button)sender).Name.Substring(7, 2)) - 1;
            if (_axisUnionFlag[buttonIndex])
                _axisUnionFlag[buttonIndex] = false;
            else
                _axisUnionFlag[buttonIndex] = true;
        }
        #endregion

        #region jog inch flag切换事件
        private void InchFlagChangeEvent(object sender, EventArgs e)
        {
            int buttonIndex = Convert.ToInt32(((Button)sender).Name.Substring(7, 2)) - 1;
            if (_axisInchFlag[buttonIndex])
                _axisInchFlag[buttonIndex] = false;
            else
                _axisInchFlag[buttonIndex] = true;

            //如果切换那么触发事件
            if (ManualDataChangedEvent != null)
            {
                ManualDataChangedEvent();
            }

        }
        #endregion

        #region 点位坐标及速度参数等的修改
        //点击保存当前坐标或编辑坐标数据按钮事件
        private void AxisSaveCurrentPositionOrModifiedPositionEvent(object sender, EventArgs e)
        {
            string ctrlNameStr = "";
            object tempCtrl = null;
            bool[] saveDataFlag = new bool[AXIS_QUANTITY] { false, false, false, false };
            bool haveAxisPotionNeedSaveFlag = false;

            if (_deviceAutoModeFlag)//如果轴处于自动模式
            {
                MessageBox.Show("当前设备处于自动模式，无法保存轴点位参数！");
                return;
            }

            for (int i = 0; i < AXIS_QUANTITY; i++)
            {
                if (_axisHomedFlag[i])//如果完成原点复归
                {
                    ctrlNameStr = "txtAxis" + (i + 1).ToString("00") + "ModifyPosition";
                    tempCtrl = this.GetType().GetField(ctrlNameStr, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);
                    if (((Button)sender).Name == "btnSaveCurrentPosition")
                    {
                        if (_useRelativeFlag)//如果使用相对位置
                            ((TextBox)tempCtrl).Text = _axisCurrentPosition_mm[i].ToString("f2");
                        else
                            ((TextBox)tempCtrl).Text = (_axisCurrentPosition_mm[i] - _absoluteCoor[i]).ToString("f2");
                        
                    }

                    float minData = 0f;
                    float maxData = 0f;

                    minData = _axisSoftLimitMinus[i];
                    maxData = _axisSoftLimitPlus[i];

                    if (_selectPointNumber < 1)
                    {
                        MessageBox.Show("当前选定的点位号小于1，不合法！\n无法保存点位数据！");
                        saveDataFlag[i] = false;
                        continue;
                    }
                    else if (_selectPointNumber > _axisPointQuantity[i])
                    {
                        MessageBox.Show("当前选定的点位号大于轴" + (i + 1).ToString("00") + "的最大点位个数，无法保存数据至当前轴点位数据表！");
                        saveDataFlag[i] = false;
                        continue;
                    }

                    if (MyTool.DataProcessing.TextBoxDataCheck2(((TextBox)tempCtrl).Text, minData, maxData, true, true, true, true))
                    {
                        saveDataFlag[i] = true;
                        haveAxisPotionNeedSaveFlag = true;
                    }
                    else
                    {
                        saveDataFlag[i] = false;
                        MessageBox.Show("轴:" + (i + 1).ToString("00") + "点位数据错误,此轴点位数据不能保存!");
                    }
                }
                else
                {
                    saveDataFlag[i] = false;
                    MessageBox.Show("轴:" + (i + 1).ToString("00") + "未完成原点复归，无法保存轴" + (i + 1).ToString("00") + "的点位数据，请先回原点！");
                }
            }

            if (haveAxisPotionNeedSaveFlag)
            {
                string infoStr = "";
                for (int i = 0; i < AXIS_QUANTITY; i++)
                {
                    if (saveDataFlag[i])
                    {
                        infoStr = infoStr + "轴:" + (i + 1).ToString("00") + "\n" + "点位号:" + _selectPointNumber.ToString("000") + "\n" +
                                "名称:" + _axisPointDataDT[i].Rows[_selectPointNumber - 1][6].ToString() + "\n";
                    }
                }
                if (((Button)sender).Name == "btnSaveCurrentPosition")
                {
                    if (MessageBox.Show("确认要保存轴当前位置?" + "\n" + infoStr, "保存当前位置", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.OK)
                    {
                        return;
                    }

                }
                else
                {
                    if (MessageBox.Show("确认要保存轴编辑位置?" + "\n" + infoStr, "保存编辑位置", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.OK)
                    {
                        return;
                    }
                }

                for (int i = 0; i < AXIS_QUANTITY; i++)
                {
                    if (saveDataFlag[i])
                    {
                        SavePointPosition(i);
                    }
                }

                if (AxisPositionDataModifiedEvent != null)
                {
                    AxisPositionDataModifiedEvent();
                }
                MessageBox.Show("点位位置数据保存完成！");
            }
            else
            {
                MessageBox.Show("经数据检查，无有效位置数据可以进行保存！");
            }
        }

        //保存点位数据
        private void SavePointPosition(int axisIndex)
        {
            DataTable dt = new DataTable();
            string axisPositionFileSaveFolderPath = Directory.GetCurrentDirectory() + @"\" +
                MyTool.FolderAndFileManageClass.FilterPathIllegalChar("AxisParameterAndPositionData_" + _currentProductName) + @"\";//过滤掉作为Directory不合法的字符
            string axisPositionFilePath = Path.Combine(axisPositionFileSaveFolderPath,
                     MyTool.FolderAndFileManageClass.FilterPathIllegalChar("axis" + axisIndex.ToString("00") + "Pos" + "_" + _currentProductName)
                     + ".csv");
            string ctrlNameStr = "txtAxis" + (axisIndex + 1).ToString("00") + "ModifyPosition";
            object tempCtrl = this.GetType().GetField(ctrlNameStr, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);
            _axisPointDataDT[axisIndex].Rows[_selectPointNumber - 1][1] = ((TextBox)tempCtrl).Text;
            try
            {
                CSVFileOperation.SaveCSV(_axisPointDataDT[axisIndex], axisPositionFilePath);
            }
            catch
            {
                MessageBox.Show("保存轴" + (axisIndex + 1).ToString("00") + "点位数据失败");
            }
        }

        //保存速度及其余参数按钮事件
        private void SaveSpeedAndOthersEvent(object sender, EventArgs e)
        {
            string ctrlNameStr = "";
            object tempCtrl = null;
            bool[] saveSpeedDataFlag = new bool[AXIS_QUANTITY] { false, false, false, false };
            bool[] saveAccDataFlag = new bool[AXIS_QUANTITY] { false, false, false, false };
            bool haveAxisPotionNeedSaveFlag = false;

            if (_deviceAutoModeFlag)//如果轴处于自动模式
            {
                MessageBox.Show("当前设备处于自动模式，无法保存轴点位参数！");
                return;
            }

            for (int i = 0; i < AXIS_QUANTITY; i++)
            {
                if (_selectPointNumber < 1)
                {
                    MessageBox.Show("当前选定的点位号小于1，不合法！\n无法保存点位数据！");
                    saveSpeedDataFlag[i] = false;
                    saveAccDataFlag[i] = false;
                    continue;
                }
                else if (_selectPointNumber > _axisPointQuantity[i])
                {
                    MessageBox.Show("当前选定的点位号大于轴" + (i + 1).ToString("00") + "的最大点位个数，无法保存数据至当前轴点位数据表！");
                    saveSpeedDataFlag[i] = false;
                    saveAccDataFlag[i] = false;
                    continue;
                }

                float minData = 0f;
                float maxData = 0f;

                ctrlNameStr = "txtAxis" + (i + 1).ToString("00") + "ModifyVelocity";
                tempCtrl = this.GetType().GetField(ctrlNameStr, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);

                minData = _axisMoveMinSpeed_mmps[i];
                maxData = _axisMoveMaxSpeed_mmps[i];

                if (MyTool.DataProcessing.TextBoxDataCheck2(((TextBox)tempCtrl).Text, minData, maxData, true, true, true, true))
                {
                    saveSpeedDataFlag[i] = true;
                }
                else
                {
                    saveSpeedDataFlag[i] = false;
                    MessageBox.Show("轴:" + (i + 1).ToString("00") + "点位速度错误,此轴速度及加减速数据不能保存!");
                }

                ctrlNameStr = "txtAxis" + (i + 1).ToString("00") + "ModifyAcc";
                tempCtrl = this.GetType().GetField(ctrlNameStr, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);

                minData = _axisMoveMinAccTime_s[i];
                maxData = _axisMoveMaxAccTime_s[i];

                if (MyTool.DataProcessing.TextBoxDataCheck2(((TextBox)tempCtrl).Text, minData, maxData, true, true, true, true))
                {
                    saveAccDataFlag[i] = true;
                }
                else
                {
                    saveAccDataFlag[i] = false;
                    MessageBox.Show("轴:" + (i + 1).ToString("00") + "点位速度错误,此轴速度及加减速数据不能保存!");
                }

                if (saveSpeedDataFlag[i] && saveAccDataFlag[i])
                    haveAxisPotionNeedSaveFlag = true;
            }

            if (haveAxisPotionNeedSaveFlag)
            {
                string infoStr = "";
                for (int i = 0; i < AXIS_QUANTITY; i++)
                {
                    if (saveSpeedDataFlag[i] && saveAccDataFlag[i])
                    {
                        infoStr = infoStr + "轴:" + (i + 1).ToString("00") + "\n" + "点位号:" + _selectPointNumber.ToString("000") + "\n" +
                                "名称:" + _axisPointDataDT[i].Rows[_selectPointNumber - 1][6].ToString() + "\n";
                    }
                }

                if (MessageBox.Show("确认要更改轴速度及其他?" + "\n" + infoStr, "更改速度及其他", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.OK)
                {
                    return;
                }

                for (int i = 0; i < AXIS_QUANTITY; i++)
                {
                    if (saveSpeedDataFlag[i] && saveAccDataFlag[i])
                    {
                        SavePointSpeedAndOthers(i);
                    }
                }

                if (AxisPositionDataModifiedEvent != null)
                {
                    AxisPositionDataModifiedEvent();
                }
                MessageBox.Show("点位速度及其他数据保存完成！");
            }
            else
            {
                MessageBox.Show("经数据检查，无有效数据可以进行保存！");
            }
        }

        //保存速度及其余点位参数数据
        private void SavePointSpeedAndOthers(int axisIndex)
        {
            DataTable dt = new DataTable();
            string axisPositionFileSaveFolderPath = Directory.GetCurrentDirectory() + @"\" +
                MyTool.FolderAndFileManageClass.FilterPathIllegalChar("AxisParameterAndPositionData_" + _currentProductName) + @"\";//过滤掉作为Directory不合法的字符
            string axisPositionFilePath = Path.Combine(axisPositionFileSaveFolderPath,
                     MyTool.FolderAndFileManageClass.FilterPathIllegalChar("axis" + axisIndex.ToString("00") + "Pos" + "_" + _currentProductName)
                     + ".csv");
            string ctrlNameStr = "";
            object tempCtrl = null;

            ctrlNameStr = "txtAxis" + (axisIndex + 1).ToString("00") + "ModifyVelocity";
            tempCtrl = this.GetType().GetField(ctrlNameStr, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);

            _axisPointDataDT[axisIndex].Rows[_selectPointNumber - 1][2] = ((TextBox)tempCtrl).Text;

            ctrlNameStr = "txtAxis" + (axisIndex + 1).ToString("00") + "ModifyAcc";
            tempCtrl = this.GetType().GetField(ctrlNameStr, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);
            _axisPointDataDT[axisIndex].Rows[_selectPointNumber - 1][3] = ((TextBox)tempCtrl).Text;

            if (_axisUnionFlag[axisIndex])
                _axisPointDataDT[axisIndex].Rows[_selectPointNumber - 1][5] = "1";
            else
                _axisPointDataDT[axisIndex].Rows[_selectPointNumber - 1][5] = "0";
            try
            {
                CSVFileOperation.SaveCSV(_axisPointDataDT[axisIndex], axisPositionFilePath);
            }
            catch
            {
                MessageBox.Show("保存轴" + (axisIndex + 1).ToString("00") + "速度及其它数据失败");
            }
        }

        #endregion

        #region 点位名称修改
        //点击更改点位号名称事件
        private void SavePointNameEvent(object sender, EventArgs e)
        {
            string ctrlNameStr = "";
            object tempCtrl = null;
            bool[] saveDataFlag = new bool[AXIS_QUANTITY] { false, false, false, false };
            bool haveAxisPotionNeedSaveFlag = false;

            for (int i = 0; i < AXIS_QUANTITY; i++)
            {

                if (_selectPointNumber < 1)
                {
                    MessageBox.Show("当前选定的点位号小于1，不合法！\n无法保存点位数据！");
                    saveDataFlag[i] = false;
                    continue;
                }
                else if (_selectPointNumber > _axisPointQuantity[i])
                {
                    MessageBox.Show("当前选定的点位号大于轴" + (i + 1).ToString("00") + "的最大点位个数，无法保存数据至当前轴点位数据表！");
                    saveDataFlag[i] = false;
                    continue;
                }

                saveDataFlag[i] = true;
                haveAxisPotionNeedSaveFlag = true;
            }

            if (haveAxisPotionNeedSaveFlag)
            {
                string infoStr = "";
                for (int i = 0; i < AXIS_QUANTITY; i++)
                {
                    if (saveDataFlag[i])
                    {
                        ctrlNameStr = "txtAxis" + (i + 1).ToString("00") + "SelectedPointNameChange";
                        tempCtrl = this.GetType().GetField(ctrlNameStr, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);
                        infoStr = infoStr + "轴:" + (i + 1).ToString("00") + "\n" + "点位号:" + _selectPointNumber.ToString("000") + "\n" +
                                "名称:" + _axisPointDataDT[i].Rows[_selectPointNumber - 1][6].ToString() + "->" + ((TextBox)tempCtrl).Text + "\n";
                    }
                }

                if (MessageBox.Show("确认要更改轴点位名称吗?" + "\n" + infoStr, "更改轴点位名称", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.OK)
                {
                    return;
                }

                for (int i = 0; i < AXIS_QUANTITY; i++)
                {
                    if (saveDataFlag[i])
                    {
                        ChangePointName(i);
                    }
                }

                if (AxisPositionDataModifiedEvent != null)
                {
                    AxisPositionDataModifiedEvent();
                }

                MessageBox.Show("修改点位名称完成！");
            }
            else
            {
                MessageBox.Show("经数据检查，无有效位置数据可以进行保存！");
            }
        }

        //保存点位数据
        private void ChangePointName(int axisIndex)
        {
            DataTable dt = new DataTable();
            string axisPositionFileSaveFolderPath = Directory.GetCurrentDirectory() + @"\" +
                MyTool.FolderAndFileManageClass.FilterPathIllegalChar("AxisParameterAndPositionData_" + _currentProductName) + @"\";//过滤掉作为Directory不合法的字符
            string axisPositionFilePath = Path.Combine(axisPositionFileSaveFolderPath,
                     MyTool.FolderAndFileManageClass.FilterPathIllegalChar("axis" + axisIndex.ToString("00") + "Pos" + "_" + _currentProductName)
                     + ".csv");
            string ctrlNameStr = "txtAxis" + (axisIndex + 1).ToString("00") + "SelectedPointNameChange";
            object tempCtrl = this.GetType().GetField(ctrlNameStr, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);
            _axisPointDataDT[axisIndex].Rows[_selectPointNumber - 1][6] = ((TextBox)tempCtrl).Text;

            try
            {
                CSVFileOperation.SaveCSV(_axisPointDataDT[axisIndex], axisPositionFilePath);
            }
            catch
            {
                MessageBox.Show("更改轴" + (axisIndex + 1).ToString("00") + "名称失败");
            }
        }

        #endregion

        #region 修改了产品点位号相关事件
        //选择产品点位号,textbox change事件
        private void txtPointNumberSet_TextChanged(object sender, EventArgs e)
        {
            ((TextBox)sender).Text = MyTool.DataProcessing.TextBoxDataCheck(((TextBox)sender).Text, 1, _maxPointQuantity, true, true, false, false);
            _selectPointNumber = Convert.ToInt32(((TextBox)sender).Text);
        }

        //选择产品点位号,textbox leave事件
        private void txtPointNumberSet_Leave(object sender, EventArgs e)
        {
            ((TextBox)sender).Text = MyTool.DataProcessing.TextBoxDataCheck(((TextBox)sender).Text, 1, _maxPointQuantity, true, true, false, true);
            _selectPointNumber = Convert.ToInt32(((TextBox)sender).Text);
            RefreshPositionDataAndPointNameToCtr();

        }
        #endregion

        #region AxisUnionMove:轴联动事件
        private void AxisUnionMove(object sender, EventArgs e)
        {
            bool[] actionAxisFlag = new bool[AXIS_QUANTITY];//动作轴flag，false-不动作，true-动作

            for (int i = 0; i < AXIS_QUANTITY; i++)//flag初始化
            {
                actionAxisFlag[i] = false;
                if (_selectPointNumber >= 1 && _selectPointNumber < _axisPointQuantity[i])//首先检查选中的点位号是否合理
                {
                    if (_axisUnionFlag[i])//如果联合
                    {
                        actionAxisFlag[i] = true;
                    }
                }
            }

            if (AxisUnionMoveEvent != null)
                AxisUnionMoveEvent(actionAxisFlag, _selectPointNumber);
        }
        #endregion

        #region AxisJogInchMove:轴jog/inch移动事件
        //轴JOG INCH鼠标按下事件
        private void AxisJogInchMove(object sender, MouseEventArgs e)
        {
            bool jogDirection = false;
            int axisIndex = Convert.ToInt32(((UCArrow.UCArrow)sender).Name.Substring(7, 2)) - 1;

            if (((UCArrow.UCArrow)sender).Name.Substring(9, 4) == "Plus")
                jogDirection = true;
            else
                jogDirection = false;

            if (JogInchMoveEvent != null)
                JogInchMoveEvent(axisIndex, jogDirection, false);

        }

        //轴JOG INCH鼠标离开事件
        private void AxisJogInchMouseLeaveEvent(object sender, EventArgs e)
        {
            int axisIndex = Convert.ToInt32(((UCArrow.UCArrow)sender).Name.Substring(7, 2)) - 1;

            if (JogInchMoveEvent != null)
                JogInchMoveEvent(axisIndex, false, true);
        }

        //轴JOG INCH鼠标松开事件
        private void AxisJogInchMouseUpEvent(object sender, MouseEventArgs e)
        {
            int axisIndex = Convert.ToInt32(((UCArrow.UCArrow)sender).Name.Substring(7, 2)) - 1;

            if (JogInchMoveEvent != null)
                JogInchMoveEvent(axisIndex, false, true);
        }
        #endregion

        #region MouseHover事件

        private void PositionModifyMouseHover(object sender, EventArgs e)
        {
            int axisIndex = Convert.ToInt32(((TextBox)sender).Name.Substring(7, 2)) - 1;
            //  设置伴随的对象.
            string s = "[" + _axisSoftLimitMinus[axisIndex].ToString() + "," + _axisSoftLimitPlus[axisIndex].ToString() + "]";
            _toolTip.SetToolTip((TextBox)sender, s);//设置提示按钮和提示内容
        }

        private void SpeedModifyMouseHover(object sender, EventArgs e)
        {
            int axisIndex = Convert.ToInt32(((TextBox)sender).Name.Substring(7, 2)) - 1;
            //  设置伴随的对象.
            string s = "[" + _axisMoveMinSpeed_mmps[axisIndex].ToString() + "," + _axisMoveMaxSpeed_mmps[axisIndex].ToString() + "]";
            _toolTip.SetToolTip((TextBox)sender, s);//设置提示按钮和提示内容
        }

        private void ACCTimeModifyMouseHover(object sender, EventArgs e)
        {
            int axisIndex = Convert.ToInt32(((TextBox)sender).Name.Substring(7, 2)) - 1;
            //  设置伴随的对象.
            string s = "[" + _axisMoveMinAccTime_s[axisIndex].ToString() + "," + _axisMoveMaxAccTime_s[axisIndex].ToString() + "]";
            _toolTip.SetToolTip((TextBox)sender, s);//设置提示按钮和提示内容
        }

        private void JogInchSpeedMouseHover(object sender, EventArgs e)
        {
            int axisIndex = Convert.ToInt32(((TextBox)sender).Name.Substring(7, 2)) - 1;
            //  设置伴随的对象.
            string s = "[" + _axisJogMinSpeed_mmps[axisIndex].ToString() + "," + _axisJogMaxSpeed_mmps[axisIndex].ToString() + "]";
            _toolTip.SetToolTip((TextBox)sender, s);//设置提示按钮和提示内容
        }

        private void InchLengthMouseHover(object sender, EventArgs e)
        {
            int axisIndex = Convert.ToInt32(((TextBox)sender).Name.Substring(7, 2)) - 1;
            //  设置伴随的对象.
            string s = "[" + _axisInchMinDistance_mm[axisIndex].ToString() + "," + _axisInchMaxDistance_mm[axisIndex].ToString() + "]";
            _toolTip.SetToolTip((TextBox)sender, s);//设置提示按钮和提示内容
        }

        #endregion

        #region 虚拟键盘相关函数
        //textbox 鼠标双击事件
        private void TextBoxDoubleClickEvent(Object sender, EventArgs e)
        {
            string txtKindStr = ((TextBox)sender).Name.Substring(9, ((TextBox)sender).Name.Length - 9);
            if (txtKindStr != "SelectedPointNameChange")
            {
                SoftNumberKeyboard.LanguageFlag = 1;//设置语言为中文
                SoftNumberKeyboard numberKeyboard = new SoftNumberKeyboard();
                numberKeyboard.StartPosition = FormStartPosition.CenterScreen;


                if (txtKindStr == "ModifyPosition")//如果为序号为1的列（Position）
                    numberKeyboard.PLUSMINUSBTHIDEFLAG = false;
                else
                    numberKeyboard.PLUSMINUSBTHIDEFLAG = true;

                if (((TextBox)sender).Name != "txtPointNumberSet")
                    numberKeyboard.FULLSTOPBTHIDEFLAG = false;
                else
                    numberKeyboard.FULLSTOPBTHIDEFLAG = true;

                numberKeyboard.SOURCESTR = ((TextBox)sender).Text;
                numberKeyboard.SENDER = sender;
                numberKeyboard.SendInputDataIncludeObj += new SENDINPUTDATAINCLUDEOBJ(GetKeyboardInputData);//添加事件
                numberKeyboard.Show();
            }
            else
            {
                SoftMainKeyboard.LanguageFlag = 1;
                SoftMainKeyboard mainKeyboard = new SoftMainKeyboard();
                mainKeyboard.StartPosition = FormStartPosition.CenterScreen;
                mainKeyboard.SOURCESTR = ((TextBox)sender).Text;
                mainKeyboard.SENDER = sender;
                mainKeyboard.SendInputDataIncludeObj += new SENDINPUTDATAINCLUDEOBJ(GetKeyboardInputData);//添加事件
                mainKeyboard.ShowDialog();
            }

        }

        //键盘按下OK之后获取数据
        private void GetKeyboardInputData(object sender, string GetDataStr)
        {
            ((TextBox)sender).Text = GetDataStr;
        }
        #endregion
    }
}
