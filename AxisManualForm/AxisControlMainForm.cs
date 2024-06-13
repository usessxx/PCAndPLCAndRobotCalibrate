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
using System.IO;
using System.Collections;
using MotionCard;
using CSVFile;
using MyTool;
using SoftKeyBoard;
using System.Diagnostics;//用于stopwatch

namespace AxisAndIOForm
{
    public delegate void AxisControlMainFormRequestCloseDelegate();//声明委托-请求关闭窗口
    public partial class AxisControlMainForm : Form
    {
        /*--------------------------------------------------------------------------------------
        //Copyright (C) 2022 深圳市利器精工科技有限公司
        //版权所有
        //
        //文件名：轴控制主窗口类
        //文件功能描述：将轴控制窗口可视化，便于操纵轴
        //
        //
        //创建标识：MaLi 20220303
        //
        //修改标识：MaLi 20220303 Change
        //修改描述：增加轴控制主窗口
        //
        //修改标识：MaLi 20220310 Change
        //修改描述：增加轴联合控制窗口相关的程序
        //
        //------------------------------------------------------------------------------------*/
        //*****************************事件*************************************//
        public event AxisControlMainFormRequestCloseDelegate AxisControlMainFormRequestCloseEvent;//声明事件，请求关闭窗口

        #region 变量
        //*********************************************************************//
        const int ONE_PAGE_DISPLAY_POINT_QUANTITY = 10;//一页显示的点位个数
        public static int _maxPointQuantity = 900;//点位个数上限----不能大于900
        public static int _axisQuantity;//轴的个数
        public static int _maxIOQuantity;//输入或者输出的IO个数(一块卡为输入及输出都为64点,两块卡为128)

        //*************************外部可设定参数*******************************//
        public bool _deviceAutoModeFlag = false;

        //0:NA;1-900:点位号定位;901:Jogging;999:Homing;1000:实时位置数据定位;1001:减速停止
        public int[] _axisActionCommand;//轴运动指令号，当轴收到指令号后，自动移动，只针对 1-999
        public int[] _axisCurrentPositionNumber;//轴当前位置编号，用于反馈运动结果
        public int[] _axisCommandSaved;//轴Command保存，用于记录运动过程中的指令

        //原点标志
        public bool[] _axisHomedFlag;//轴原点复归状态

        public float[] _assignedPositionActionMaxVelocity_mmps; //float:当为指定目标运动时的最大速度
        public float[] _assignedPositionActionTargetPosition_mm; //float:当为指定目标运动时的目标坐标
        public float[] _assignedPositionActionAccTime_s; //float:当为指定目标运动时的加减速时间
        public float[] _assignedPositionActionDeviation; //float:当为指定目标运动时的deviation
        public bool[] _assignedPositionActionSMoveFlag; //bool:当为指定目标运动时的S移动启用与否，false-不启用，true-启用
        public double[] _assignedPositionActionSMoveTime; //double:当为指定目标运动启用S移动时S移动时间

        public bool[] _axisActionSafeFlag;//bool:轴可以动作安全标志，用于安全门等运动限制状态
        //*************************外部可读取参数*******************************//

        //当前位置_mm
        public double[] _axisCurrentPosition_mm;
        //当前轴速度_mmps
        public double[] _axisCurrentSpeed_mmps;

        public bool[,] _ioStatus;//io状态，0-为输入，1-为输出

        public DataTable[] _axisPointDataDT;//轴点位数据DATA TABLE，用于读取轴点位数据

        public bool[] _axisUseFlag;//轴启用Flag,false-不启用，true-启用

        public bool _motionCardInitialStatus = false;//运动控制卡初始化状态

        //毫米/脉冲
        public float[] _axismmPP;

        //public MotionCard.LeiSaiMotionCardDMC1000BSerial _axisMotionControlBoardVariate = null;//运动控制卡变量定义
        public MotionCard.GaoChuanMotionCardGCN400AS _axisMotionControlBoardVariate = null;//运动控制卡变量定义

        public float[] _absoluteCoor;//轴绝对坐标
        public bool _useRelativeFlag = false;//使用相对坐标标志，false-不使用，true-使用
        //*************************公共静态变量*******************************//
        public static bool _axisParFileInitialFinishFlag = false;//轴参数初始化Flag，false-初始化未完成，true-初始化完成
        public static bool _axisPosFileInitialFinishFlag = false;//轴点位数据初始化Flag，false-初始化未完成，true-初始化完成
        public static string _currentProductName = "Default";//当前产品名
        //*************************内部私有变量*******************************//
        private string[] _axisNameStrArray;//轴的名称用于界面显示
        private string[] _axisUnitArray;//轴单位数组，为mm或°

        private Thread _exchangeDataWithMotionCardThread = null;//与运动控制卡交换数据线程
        private Thread _statusUpdateThread = null;//状态实时更新线程
        private Thread _axisActionThread = null;//轴动作线程
        private Thread _ctrlUpdateThread = null;//控件更新线程

        //轴运动按钮使能标志
        private int _currentAxisIndex = 0;//当前轴索引号，0-SERVO 01, 1- SERVO 02, 2- SERVO 03, 3-SERVO 04;
        private int _currentPageIndex = 0;//当前页码
        private int _currentSelectPointNumber = 1;//当前选中点位号
        private int _oldSelectPointNumber = 0;//上一次选中点位号
        private bool[] _inchMoveFlag;//轴采用inch移动标志，false-jog运行，true-inch运行

        //极限
        //软负极限
        private float[] _axisSoftLimitMinus;
        //软正极限
        private float[] _axisSoftLimitPlus;

        //速度百分比(%)
        private float[] _axisManualSpeedPCT;
        private float[] _axisAutoSpeedPCT;

        //JogMinSpeed
        private float[] _axisJogMinSpeed_mmps;
        //JogMaxSpeed
        private float[] _axisJogMaxSpeed_mmps;
        //InchMinDistance
        private float[] _axisInchMinDistance_mm;
        //InchMaxDistance
        private float[] _axisInchMaxDistance_mm;

        //MoveMinSpeed
        private float[] _axisMoveMinSpeed_mmps;
        //MoveMaxSpeed
        private float[] _axisMoveMaxSpeed_mmps;
        //MoveminACCTime
        private float[] _axisMoveMinAccTime_s;
        //MoveMaxACCTime
        private float[] _axisMoveMaxAccTime_s;
        //MoveminDecTime
        private float[] _axisMoveMinDecTime_s;
        //MoveMaxDecTime
        private float[] _axisMoveMaxDecTime_s;
        //MinDeviation
        private float[] _axisMinDeviation_mm;
        //MaxDeviation
        private float[] _axisMaxDeviation_mm;
        //AxisMoveStartVelocity
        private float[] _axisMoveStartVelocity_mmps;
        //AxisMoveStopVelocity
        private float[] _axisMoveStopVelocity_mmps;

        //轴原点复归起始速度
        private float[] _axisHomeMoveStartVelocity_mmps;
        //轴原点复归最大速度
        private float[] _axisHomeMoveMaxVelocity_mmps;
        //轴原点复归停止速度
        private float[] _axisHomeMoveStopVelocity_mmps;
        //轴原点复归加速时间
        private float[] _axisHomeMoveAccTime_s;
        //轴原点复归减速时间
        private float[] _axisHomeMoveDecTime_s;
        //轴原点复归寻找原点传感器的速度
        private float[] _axisHomeFindSensorVelocity_mmps;
        //轴原点复归找到原点传感器后后退的距离
        private float[] _axisHomeBackDistance_mm;

        private float[] _axisJogInchVelocity_mmps;//轴JOG/INCH的速度
        private float[] _axisInchDistance_mm;//轴INCH的距离

        private int[] _axisPointQuantity;//轴点位个数
        private AxisPositionDataViewForm _axisPositionDataViewFormVariate = null;//轴点位数据显示页面变量 
        private string _oldProductName = "";//旧的产品名，用于判定产品名是否更改
        Stopwatch _motionCardErrorLampBlinkSW = null;//用于控制运动控制卡错误时的闪烁

        private AxisUnionForm _axisUnionFormVariate = null;//MaLi 20220310 Change 轴联合控制窗口变量

        //0:NA;1-900:点位号定位;901:Jogging;999:Homing;1000:实时位置数据定位;1001:减速停止
        private int[] _axisCommand;//从motion card处反馈回来的值

        //0:NA;1-900:当前点位号;999:Homed;1000:实时位置数据定位完成

        //错误代码
        private int[] _axisErrorNumber;
        //轴Servo On Flag
        private bool[] _axisServoOnOffFlag;
        //轴运行状态
        private int[] _axisCheckDoneStatus;
        //轴原点，正负极限状态
        private bool[] _axisELMinus;
        private bool[] _axisELPlus;
        private bool[] _axisORG;
        //轴回原点开始Flag
        private bool[] _axisHomeStartFlag;
        //轴运动开始Flag
        private bool[] _axisMoveStartFlag;

        private MotionCardIOForm _motionCardIOFormVaraite = null;//MaLi 20220312 Change io状态显示窗口

        #endregion

        #region VariateDataInitial:变量参数初始化
        /// <summary>
        /// VariateDataInitial：变量参数的初始化
        /// </summary>
        void VariateDataInitial()
        {
            for (int i = 0; i < _axisQuantity; i++)
            {
                //*************************外部可设定参数*******************************//

                //*************************外部可读取参数*******************************//
                //当前位置_mm
                _axisCurrentPosition_mm[i] = 0f;
                //当前轴速度_mmps
                _axisCurrentSpeed_mmps[i] = 0f;

                _axisActionCommand[i] = 0;
                //0:NA;1-900:点位号定位;901:Jogging;999:Homing;1000:实时位置数据定位;1001:减速停止
                _axisCommand[i] = 0;
                //轴Command保存
                _axisCommandSaved[i] = 0;
                //0:NA;1-900:当前点位号;999:Homed;1000:实时位置数据定位完成
                _axisCurrentPositionNumber[i] = 0;

                //原点标志
                _axisHomedFlag[i] = false;
                //错误代码
                _axisErrorNumber[i] = 0;
                //轴Servo On Flag
                _axisServoOnOffFlag[i] = false;
                //轴运行状态
                _axisCheckDoneStatus[i] = 1;
                //轴原点，正负极限状态
                _axisELMinus[i] = false;
                _axisELPlus[i] = false;
                _axisORG[i] = false;
                //轴回原点开始Flag
                _axisHomeStartFlag[i] = false;
                //轴运动开始Flag
                _axisMoveStartFlag[i] = false;


                //*************************私有变量，外部不可读取修改*******************************//
                //速度百分比(%)
                _axisManualSpeedPCT[i] = 30f;
                _axisAutoSpeedPCT[i] = 100f;

                //JogMinSpeed
                _axisJogMinSpeed_mmps[i] = 1f;
                //JogMaxSpeed
                _axisJogMaxSpeed_mmps[i] = 100f;
                //InchMinDistance
                _axisInchMinDistance_mm[i] = 0.01f;
                //InchMaxDistance
                _axisInchMaxDistance_mm[i] = 100f;

                //MoveMinSpeed
                _axisMoveMinSpeed_mmps[i] = 1f;
                //MoveMaxSpeed
                _axisMoveMaxSpeed_mmps[i] = 1000f;
                //MoveminACCTime
                _axisMoveMinAccTime_s[i] = 10f;
                //MoveMaxACCTime
                _axisMoveMaxAccTime_s[i] = 3000f;
                //MoveminDecTime
                _axisMoveMinDecTime_s[i] = 10f;
                //MoveMaxDecTime
                _axisMoveMaxDecTime_s[i] = 3000f;
                //MinDeviation
                _axisMinDeviation_mm[i] = 0f;
                //MaxDeviation
                _axisMaxDeviation_mm[i] = 0f;
                //AxisMoveStartVelocity
                _axisMoveStartVelocity_mmps[i] = 0f;
                //AxisMoveStopVelocity
                _axisMoveStopVelocity_mmps[i] = 0f;

                //轴原点复归起始速度
                _axisHomeMoveStartVelocity_mmps[i] = 0f;
                //轴原点复归最大速度
                _axisHomeMoveMaxVelocity_mmps[i] = 10.0f;
                //轴原点复归停止速度
                _axisHomeMoveStopVelocity_mmps[i] = 0f;
                //轴原点复归加速时间
                _axisHomeMoveAccTime_s[i] = 1;
                //轴原点复归减速时间
                _axisHomeMoveDecTime_s[i] = 1;
                //轴原点复归寻找原点传感器的速度
                _axisHomeFindSensorVelocity_mmps[i] = 1;
                //轴原点复归找到原点传感器后后退的距离
                _axisHomeBackDistance_mm[i] = 5;

                //轴点位个数
                _axisPointQuantity[i] = 100;
                //轴点位数据datatable
                _axisPointDataDT[i] = null;

                //轴JOGINCH速度
                _axisJogInchVelocity_mmps[i] = 10f;
                //轴joginch距离
                _axisInchDistance_mm[i] = 1f;
                //绝对位置坐标
                _absoluteCoor[i] = 0f;
            }
        }
        #endregion

        #region AxisControlMainForm
        public AxisControlMainForm()
        {
            InitializeComponent();

            if (_axisMotionControlBoardVariate != null)
            {
                _axisMotionControlBoardVariate.Close();
                _axisMotionControlBoardVariate = null;
            }
            _axisMotionControlBoardVariate = new GaoChuanMotionCardGCN400AS();//实例化运动控制卡

            #region 实例化运动控制卡后进行变量定义
            _axisQuantity = _axisMotionControlBoardVariate._axisQuantity;
            //0:NA;1-900:点位号定位;901:Jogging;999:Homing;1000:实时位置数据定位;1001:减速停止
            _axisActionCommand = new int[_axisQuantity];
            _axisCurrentPositionNumber = new int[_axisQuantity];//轴当前位置编号，用于反馈运动结果
            _axisCommandSaved = new int[_axisQuantity];//轴Command保存，用于记录运动过程中的指令
            //原点标志
            _axisHomedFlag = new bool[_axisQuantity];//轴原点复归状态
            _assignedPositionActionMaxVelocity_mmps = new float[_axisQuantity]; //float:当为指定目标运动时的最大速度
            _assignedPositionActionTargetPosition_mm = new float[_axisQuantity]; //float:当为指定目标运动时的目标坐标
            _assignedPositionActionAccTime_s = new float[_axisQuantity]; //float:当为指定目标运动时的加减速时间
            _assignedPositionActionDeviation = new float[_axisQuantity]; //float:当为指定目标运动时的deviation
            _assignedPositionActionSMoveFlag = new bool[_axisQuantity]; //bool:当为指定目标运动时的S移动启用与否，false-不启用，true-启用
            _assignedPositionActionSMoveTime = new double[_axisQuantity]; //double:当为指定目标运动启用S移动时S移动时间

            _axisActionSafeFlag = new bool[_axisQuantity];//bool:轴可以动作安全标志，用于安全门等运动限制状态

            _axisActionSafeFlag[0] = true;
            _axisActionSafeFlag[1] = true;
            _axisActionSafeFlag[2] = true;
            _axisActionSafeFlag[3] = true;

            //当前位置_mm
            _axisCurrentPosition_mm = new double[_axisQuantity];
            //当前轴速度_mmps
            _axisCurrentSpeed_mmps = new double[_axisQuantity];

            _maxIOQuantity = _axisMotionControlBoardVariate._maxIOQuantity;
            _ioStatus = new bool[2, _maxIOQuantity];

            _axisPointDataDT = new DataTable[_axisQuantity];//轴点位数据DATA TABLE，用于读取轴点位数据

            _axisUseFlag = new bool[_axisQuantity];//轴启用Flag,false-不启用，true-启用

            //毫米/脉冲
            _axismmPP = _axisMotionControlBoardVariate._axismmPP;

            //绝对补偿坐标
            _absoluteCoor = new float[_axisQuantity];
            //*************************内部私有变量*******************************//
            _axisNameStrArray = _axisMotionControlBoardVariate._axisNameStrArray;
            _axisUnitArray = _axisMotionControlBoardVariate._axisUnitArray;

            _inchMoveFlag = _axisMotionControlBoardVariate._inchMoveFlag;

            //极限
            //软负极限
            _axisSoftLimitMinus = _axisMotionControlBoardVariate._axisSoftLimitMinus;
            //软正极限
            _axisSoftLimitPlus = _axisMotionControlBoardVariate._axisSoftLimitPlus;

            //速度百分比(%)
            _axisManualSpeedPCT = new float[_axisQuantity];
            _axisAutoSpeedPCT = new float[_axisQuantity];

            //JogMinSpeed
            _axisJogMinSpeed_mmps = new float[_axisQuantity];
            //JogMaxSpeed
            _axisJogMaxSpeed_mmps = new float[_axisQuantity];
            //InchMinDistance
            _axisInchMinDistance_mm = new float[_axisQuantity];
            //InchMaxDistance
            _axisInchMaxDistance_mm = new float[_axisQuantity];

            //MoveMinSpeed
            _axisMoveMinSpeed_mmps = new float[_axisQuantity];
            //MoveMaxSpeed
            _axisMoveMaxSpeed_mmps = new float[_axisQuantity];
            //MoveminACCTime
            _axisMoveMinAccTime_s = new float[_axisQuantity];
            //MoveMaxACCTime
            _axisMoveMaxAccTime_s = new float[_axisQuantity];
            //MoveminDecTime
            _axisMoveMinDecTime_s = new float[_axisQuantity];
            //MoveMaxDecTime
            _axisMoveMaxDecTime_s = new float[_axisQuantity];
            //MinDeviation
            _axisMinDeviation_mm = new float[_axisQuantity];
            //MaxDeviation
            _axisMaxDeviation_mm = new float[_axisQuantity];
            //AxisMoveStartVelocity
            _axisMoveStartVelocity_mmps = new float[_axisQuantity];
            //AxisMoveStopVelocity
            _axisMoveStopVelocity_mmps = new float[_axisQuantity];

            //轴原点复归起始速度
            _axisHomeMoveStartVelocity_mmps = new float[_axisQuantity];
            //轴原点复归最大速度
            _axisHomeMoveMaxVelocity_mmps = new float[_axisQuantity];
            //轴原点复归停止速度
            _axisHomeMoveStopVelocity_mmps = new float[_axisQuantity];
            //轴原点复归加速时间
            _axisHomeMoveAccTime_s = new float[_axisQuantity];
            //轴原点复归减速时间
            _axisHomeMoveDecTime_s = new float[_axisQuantity];
            //轴原点复归寻找原点传感器的速度
            _axisHomeFindSensorVelocity_mmps = new float[_axisQuantity];
            //轴原点复归找到原点传感器后后退的距离
            _axisHomeBackDistance_mm = new float[_axisQuantity];

            _axisJogInchVelocity_mmps = new float[_axisQuantity];//轴JOG/INCH的速度
            _axisInchDistance_mm = new float[_axisQuantity];//轴INCH的距离

            _axisPointQuantity = new int[_axisQuantity];//轴点位个数

            //0:NA;1-900:点位号定位;901:Jogging;999:Homing;1000:实时位置数据定位;1001:减速停止
            _axisCommand = new int[_axisQuantity];//从motion card处反馈回来的值

            //0:NA;1-900:当前点位号;999:Homed;1000:实时位置数据定位完成

            //错误代码
            _axisErrorNumber = new int[_axisQuantity];
            //轴Servo On Flag
            _axisServoOnOffFlag = new bool[_axisQuantity];
            //轴运行状态
            _axisCheckDoneStatus = new int[_axisQuantity];
            //轴原点，正负极限状态
            _axisELMinus = new bool[_axisQuantity];
            _axisELPlus = new bool[_axisQuantity];
            _axisORG = new bool[_axisQuantity];
            //轴回原点开始Flag
            _axisHomeStartFlag = new bool[_axisQuantity];
            //轴运动开始Flag
            _axisMoveStartFlag = new bool[_axisQuantity];
            #endregion

            VariateDataInitial();//参数初始化

            AxisParFileInitial();//轴参数文件初始化

            AxisPosFileInitial();//轴位置文件初始化

            if (!_axisMotionControlBoardVariate._initialFlag)
            {
                MessageBox.Show("运动控制卡初始化失败!");
            }

            if (_exchangeDataWithMotionCardThread != null)
            {
                _exchangeDataWithMotionCardThread.Abort();
                _exchangeDataWithMotionCardThread = null;
            }
            _exchangeDataWithMotionCardThread = new Thread(ExchangeDataWithMotionCardThreadFunc);//与运动控制卡交换数据线程实例化
            _exchangeDataWithMotionCardThread.IsBackground = true;//设置isbackground为true，当程序退出时线程也退出
            _exchangeDataWithMotionCardThread.Start();//启动线程

            if (_statusUpdateThread != null)
            {
                _statusUpdateThread.Abort();
                _statusUpdateThread = null;
            }
            _statusUpdateThread = new Thread(AxisStatusUpdataThreadFunc);//状态更新线程实例化
            _statusUpdateThread.IsBackground = true;//设置isbackground为true，当程序退出时线程也退出
            _statusUpdateThread.Start();//启动线程

            if (_ctrlUpdateThread != null)
            {
                _ctrlUpdateThread.Abort();
                _ctrlUpdateThread = null;
            }
            _ctrlUpdateThread = new Thread(CtrlUpdataThreadFunc);//状态更新线程实例化
            _ctrlUpdateThread.IsBackground = true;//设置isbackground为true，当程序退出时线程也退出
            _ctrlUpdateThread.Start();//启动线程

            if (_axisActionThread != null)
            {
                _axisActionThread.Abort();
                _axisActionThread = null;
            }
            _axisActionThread = new Thread(AxisActionThreadFunc);//轴运动线程
            _axisActionThread.IsBackground = true;//设置isbackground为true，当程序退出时线程也退出
            _axisActionThread.Start();


        }
        #endregion

        #region AxisControlMainForm_Load
        private void AxisControlMainForm_Load(object sender, EventArgs e)
        {
            AxisControlBtnDisplayOrHideFunc();
        }
        #endregion

        #region ExchangeDataWithMotionCardThreadFunc:与运动控制卡数据交互线程
        private void ExchangeDataWithMotionCardThreadFunc()
        {
            while (true)
            {
                if (_axisMotionControlBoardVariate._initialFlag)//如果运动控制卡启动成功
                {
                    #region axis00
                    if (_axisQuantity >= 1)
                    {
                        //axismmPP
                        _axisMotionControlBoardVariate._axis00mmPP = _axismmPP[0];
                        //软负极限
                        _axisMotionControlBoardVariate._axis00softLimitMinus = _axisSoftLimitMinus[0];
                        //软正极限
                        _axisMotionControlBoardVariate._axis00softLimitPlus = _axisSoftLimitPlus[0];
                        //JogMinSpeed
                        _axisMotionControlBoardVariate._axis00JogMinSpeed_mmps = _axisJogMinSpeed_mmps[0];
                        //JogMaxSpeed
                        _axisMotionControlBoardVariate._axis00JogMaxSpeed_mmps = _axisJogMaxSpeed_mmps[0];
                        //InchMinDistance
                        _axisMotionControlBoardVariate._axis00InchMinDistance_mm = _axisInchMinDistance_mm[0];
                        //InchMaxDistance
                        _axisMotionControlBoardVariate._axis00InchMaxDistance_mm = _axisInchMaxDistance_mm[0];
                        //MoveMinSpeed
                        _axisMotionControlBoardVariate._axis00MoveMinSpeed_mmps = _axisMoveMinSpeed_mmps[0];
                        //MoveMaxSpeed
                        _axisMotionControlBoardVariate._axis00MoveMaxSpeed_mmps = _axisMoveMaxSpeed_mmps[0];
                        //MoveminACCTime
                        _axisMotionControlBoardVariate._axis00MoveMinAccTime_s = _axisMoveMinAccTime_s[0];
                        //MoveMaxACCTime
                        _axisMotionControlBoardVariate._axis00MoveMaxAccTime_s = _axisMoveMaxAccTime_s[0];
                        //MinDeviation
                        _axisMotionControlBoardVariate._axis00MinDeviation_mm = _axisMinDeviation_mm[0];
                        //MaxDeviation
                        _axisMotionControlBoardVariate._axis00MaxDeviation_mm = _axisMaxDeviation_mm[0];
                        //轴原点复归起始速度
                        _axisMotionControlBoardVariate._axis00HomeMoveStartVelocity_mmps = _axisHomeMoveStartVelocity_mmps[0];
                        //轴原点复归最大速度
                        _axisMotionControlBoardVariate._axis00HomeMoveMaxVelocity_mmps = _axisHomeMoveMaxVelocity_mmps[0];
                        //轴原点复归加速时间
                        _axisMotionControlBoardVariate._axis00HomeMoveAccTime_s = _axisHomeMoveAccTime_s[0];
                        //轴原点复归寻找原点传感器的速度
                        _axisMotionControlBoardVariate._axis00HomeFindSensorVelocity_mmps = _axisHomeFindSensorVelocity_mmps[0];
                        //轴原点复归找到原点传感器后后退的距离
                        _axisMotionControlBoardVariate._axis00HomeBackDistance_mm = _axisHomeBackDistance_mm[0];

                        //当前位置_脉冲数
                        _axisCurrentPosition_mm[0] = _axisMotionControlBoardVariate._axis00CurrentPosition_mm;
                        //当前轴速度_脉冲数
                        _axisCurrentSpeed_mmps[0] = _axisMotionControlBoardVariate._axis00CurrentSpeed_mmps;
                        //0:NA;1-900:点位号定位;901:Jogging;999:Homing;1000:实时位置数据定位;1001:减速停止
                        _axisCommand[0] = _axisMotionControlBoardVariate._axis00Command;
                        //轴Command保存
                        _axisCommandSaved[0] = _axisMotionControlBoardVariate._axis00CommandSaved;
                        //0:NA;1-900:当前点位号;999:Homed;1000:实时位置数据定位完成
                        _axisCurrentPositionNumber[0] = _axisMotionControlBoardVariate._axis00CurrentPositionNumber;
                        //原点标志
                        _axisHomedFlag[0] = _axisMotionControlBoardVariate._axis00HomedFlag;
                        //错误代码
                        _axisErrorNumber[0] = _axisMotionControlBoardVariate._axis00ErrorNumber;
                        //轴Servo On Flag
                        _axisServoOnOffFlag[0] = _axisMotionControlBoardVariate._axis00ServoOnOffFlag;
                        //轴运行状态
                        _axisCheckDoneStatus[0] = _axisMotionControlBoardVariate._axis00CheckDoneStatus;
                        //轴原点，正负极限状态
                        _axisELMinus[0] = _axisMotionControlBoardVariate._axis00ELMinus;
                        _axisELPlus[0] = _axisMotionControlBoardVariate._axis00ELPlus;
                        _axisORG[0] = _axisMotionControlBoardVariate._axis00ORG;
                        //轴回原点开始Flag
                        _axisHomeStartFlag[0] = _axisMotionControlBoardVariate._axis00HomeStartFlag;
                        //轴运动开始Flag
                        _axisMoveStartFlag[0] = _axisMotionControlBoardVariate._axis00MoveStartFlag;
                    }
                    #endregion

                    #region axis01
                    if (_axisQuantity >= 2)
                    {
                        //axismmPP
                        _axisMotionControlBoardVariate._axis01mmPP = _axismmPP[1];
                        //软负极限
                        _axisMotionControlBoardVariate._axis01softLimitMinus = _axisSoftLimitMinus[1];
                        //软正极限
                        _axisMotionControlBoardVariate._axis01softLimitPlus = _axisSoftLimitPlus[1];
                        //JogMinSpeed
                        _axisMotionControlBoardVariate._axis01JogMinSpeed_mmps = _axisJogMinSpeed_mmps[1];
                        //JogMaxSpeed
                        _axisMotionControlBoardVariate._axis01JogMaxSpeed_mmps = _axisJogMaxSpeed_mmps[1];
                        //InchMinDistance
                        _axisMotionControlBoardVariate._axis01InchMinDistance_mm = _axisInchMinDistance_mm[1];
                        //InchMaxDistance
                        _axisMotionControlBoardVariate._axis01InchMaxDistance_mm = _axisInchMaxDistance_mm[1];
                        //MoveMinSpeed
                        _axisMotionControlBoardVariate._axis01MoveMinSpeed_mmps = _axisMoveMinSpeed_mmps[1];
                        //MoveMaxSpeed
                        _axisMotionControlBoardVariate._axis01MoveMaxSpeed_mmps = _axisMoveMaxSpeed_mmps[1];
                        //MoveminACCTime
                        _axisMotionControlBoardVariate._axis01MoveMinAccTime_s = _axisMoveMinAccTime_s[1];
                        //MoveMaxACCTime
                        _axisMotionControlBoardVariate._axis01MoveMaxAccTime_s = _axisMoveMaxAccTime_s[1];
                        //MinDeviation
                        _axisMotionControlBoardVariate._axis01MinDeviation_mm = _axisMinDeviation_mm[1];
                        //MaxDeviation
                        _axisMotionControlBoardVariate._axis01MaxDeviation_mm = _axisMaxDeviation_mm[1];
                        //轴原点复归起始速度
                        _axisMotionControlBoardVariate._axis01HomeMoveStartVelocity_mmps = _axisHomeMoveStartVelocity_mmps[1];
                        //轴原点复归最大速度
                        _axisMotionControlBoardVariate._axis01HomeMoveMaxVelocity_mmps = _axisHomeMoveMaxVelocity_mmps[1];
                        //轴原点复归加速时间
                        _axisMotionControlBoardVariate._axis01HomeMoveAccTime_s = _axisHomeMoveAccTime_s[1];
                        //轴原点复归寻找原点传感器的速度
                        _axisMotionControlBoardVariate._axis01HomeFindSensorVelocity_mmps = _axisHomeFindSensorVelocity_mmps[1];
                        //轴原点复归找到原点传感器后后退的距离
                        _axisMotionControlBoardVariate._axis01HomeBackDistance_mm = _axisHomeBackDistance_mm[1];

                        //当前位置_脉冲数
                        _axisCurrentPosition_mm[1] = _axisMotionControlBoardVariate._axis01CurrentPosition_mm;
                        //当前轴速度_脉冲数
                        _axisCurrentSpeed_mmps[1] = _axisMotionControlBoardVariate._axis01CurrentSpeed_mmps;
                        //0:NA;1-900:点位号定位;901:Jogging;999:Homing;1000:实时位置数据定位;1001:减速停止
                        _axisCommand[1] = _axisMotionControlBoardVariate._axis01Command;
                        //轴Command保存
                        _axisCommandSaved[1] = _axisMotionControlBoardVariate._axis01CommandSaved;
                        //0:NA;1-900:当前点位号;999:Homed;1000:实时位置数据定位完成
                        _axisCurrentPositionNumber[1] = _axisMotionControlBoardVariate._axis01CurrentPositionNumber;
                        //原点标志
                        _axisHomedFlag[1] = _axisMotionControlBoardVariate._axis01HomedFlag;
                        //错误代码
                        _axisErrorNumber[1] = _axisMotionControlBoardVariate._axis01ErrorNumber;
                        //轴Servo On Flag
                        _axisServoOnOffFlag[1] = _axisMotionControlBoardVariate._axis01ServoOnOffFlag;
                        //轴运行状态
                        _axisCheckDoneStatus[1] = _axisMotionControlBoardVariate._axis01CheckDoneStatus;
                        //轴原点，正负极限状态
                        _axisELMinus[1] = _axisMotionControlBoardVariate._axis01ELMinus;
                        _axisELPlus[1] = _axisMotionControlBoardVariate._axis01ELPlus;
                        _axisORG[1] = _axisMotionControlBoardVariate._axis01ORG;
                        //轴回原点开始Flag
                        _axisHomeStartFlag[1] = _axisMotionControlBoardVariate._axis01HomeStartFlag;
                        //轴运动开始Flag
                        _axisMoveStartFlag[1] = _axisMotionControlBoardVariate._axis01MoveStartFlag;
                    }
                    #endregion

                    #region axis02
                    if (_axisQuantity >= 3)
                    {
                        //axismmPP
                        _axisMotionControlBoardVariate._axis02mmPP = _axismmPP[2];
                        //软负极限
                        _axisMotionControlBoardVariate._axis02softLimitMinus = _axisSoftLimitMinus[2];
                        //软正极限
                        _axisMotionControlBoardVariate._axis02softLimitPlus = _axisSoftLimitPlus[2];
                        //JogMinSpeed
                        _axisMotionControlBoardVariate._axis02JogMinSpeed_mmps = _axisJogMinSpeed_mmps[2];
                        //JogMaxSpeed
                        _axisMotionControlBoardVariate._axis02JogMaxSpeed_mmps = _axisJogMaxSpeed_mmps[2];
                        //InchMinDistance
                        _axisMotionControlBoardVariate._axis02InchMinDistance_mm = _axisInchMinDistance_mm[2];
                        //InchMaxDistance
                        _axisMotionControlBoardVariate._axis02InchMaxDistance_mm = _axisInchMaxDistance_mm[2];
                        //MoveMinSpeed
                        _axisMotionControlBoardVariate._axis02MoveMinSpeed_mmps = _axisMoveMinSpeed_mmps[2];
                        //MoveMaxSpeed
                        _axisMotionControlBoardVariate._axis02MoveMaxSpeed_mmps = _axisMoveMaxSpeed_mmps[2];
                        //MoveminACCTime
                        _axisMotionControlBoardVariate._axis02MoveMinAccTime_s = _axisMoveMinAccTime_s[2];
                        //MoveMaxACCTime
                        _axisMotionControlBoardVariate._axis02MoveMaxAccTime_s = _axisMoveMaxAccTime_s[2];
                        //MinDeviation
                        _axisMotionControlBoardVariate._axis02MinDeviation_mm = _axisMinDeviation_mm[2];
                        //MaxDeviation
                        _axisMotionControlBoardVariate._axis02MaxDeviation_mm = _axisMaxDeviation_mm[2];
                        //轴原点复归起始速度
                        _axisMotionControlBoardVariate._axis02HomeMoveStartVelocity_mmps = _axisHomeMoveStartVelocity_mmps[2];
                        //轴原点复归最大速度
                        _axisMotionControlBoardVariate._axis02HomeMoveMaxVelocity_mmps = _axisHomeMoveMaxVelocity_mmps[2];
                        //轴原点复归加速时间
                        _axisMotionControlBoardVariate._axis02HomeMoveAccTime_s = _axisHomeMoveAccTime_s[2];
                        //轴原点复归寻找原点传感器的速度
                        _axisMotionControlBoardVariate._axis02HomeFindSensorVelocity_mmps = _axisHomeFindSensorVelocity_mmps[2];
                        //轴原点复归找到原点传感器后后退的距离
                        _axisMotionControlBoardVariate._axis02HomeBackDistance_mm = _axisHomeBackDistance_mm[2];

                        //当前位置_脉冲数
                        _axisCurrentPosition_mm[2] = _axisMotionControlBoardVariate._axis02CurrentPosition_mm;
                        //当前轴速度_脉冲数
                        _axisCurrentSpeed_mmps[2] = _axisMotionControlBoardVariate._axis02CurrentSpeed_mmps;
                        //0:NA;1-900:点位号定位;901:Jogging;999:Homing;1000:实时位置数据定位;1001:减速停止
                        _axisCommand[2] = _axisMotionControlBoardVariate._axis02Command;
                        //轴Command保存
                        _axisCommandSaved[2] = _axisMotionControlBoardVariate._axis02CommandSaved;
                        //0:NA;1-900:当前点位号;999:Homed;1000:实时位置数据定位完成
                        _axisCurrentPositionNumber[2] = _axisMotionControlBoardVariate._axis02CurrentPositionNumber;
                        //原点标志
                        _axisHomedFlag[2] = _axisMotionControlBoardVariate._axis02HomedFlag;
                        //错误代码
                        _axisErrorNumber[2] = _axisMotionControlBoardVariate._axis02ErrorNumber;
                        //轴Servo On Flag
                        _axisServoOnOffFlag[2] = _axisMotionControlBoardVariate._axis02ServoOnOffFlag;
                        //轴运行状态
                        _axisCheckDoneStatus[2] = _axisMotionControlBoardVariate._axis02CheckDoneStatus;
                        //轴原点，正负极限状态
                        _axisELMinus[2] = _axisMotionControlBoardVariate._axis02ELMinus;
                        _axisELPlus[2] = _axisMotionControlBoardVariate._axis02ELPlus;
                        _axisORG[2] = _axisMotionControlBoardVariate._axis02ORG;
                        //轴回原点开始Flag
                        _axisHomeStartFlag[2] = _axisMotionControlBoardVariate._axis02HomeStartFlag;
                        //轴运动开始Flag
                        _axisMoveStartFlag[2] = _axisMotionControlBoardVariate._axis02MoveStartFlag;
                    }
                    #endregion

                    #region axis03
                    if (_axisQuantity >= 4)
                    {
                        //axismmPP
                        _axisMotionControlBoardVariate._axis03mmPP = _axismmPP[3];
                        //软负极限
                        _axisMotionControlBoardVariate._axis03softLimitMinus = _axisSoftLimitMinus[3];
                        //软正极限
                        _axisMotionControlBoardVariate._axis03softLimitPlus = _axisSoftLimitPlus[3];
                        //JogMinSpeed
                        _axisMotionControlBoardVariate._axis03JogMinSpeed_mmps = _axisJogMinSpeed_mmps[3];
                        //JogMaxSpeed
                        _axisMotionControlBoardVariate._axis03JogMaxSpeed_mmps = _axisJogMaxSpeed_mmps[3];
                        //InchMinDistance
                        _axisMotionControlBoardVariate._axis03InchMinDistance_mm = _axisInchMinDistance_mm[3];
                        //InchMaxDistance
                        _axisMotionControlBoardVariate._axis03InchMaxDistance_mm = _axisInchMaxDistance_mm[3];
                        //MoveMinSpeed
                        _axisMotionControlBoardVariate._axis03MoveMinSpeed_mmps = _axisMoveMinSpeed_mmps[3];
                        //MoveMaxSpeed
                        _axisMotionControlBoardVariate._axis03MoveMaxSpeed_mmps = _axisMoveMaxSpeed_mmps[3];
                        //MoveminACCTime
                        _axisMotionControlBoardVariate._axis03MoveMinAccTime_s = _axisMoveMinAccTime_s[3];
                        //MoveMaxACCTime
                        _axisMotionControlBoardVariate._axis03MoveMaxAccTime_s = _axisMoveMaxAccTime_s[3];
                        //MinDeviation
                        _axisMotionControlBoardVariate._axis03MinDeviation_mm = _axisMinDeviation_mm[3];
                        //MaxDeviation
                        _axisMotionControlBoardVariate._axis03MaxDeviation_mm = _axisMaxDeviation_mm[3];
                        //轴原点复归起始速度
                        _axisMotionControlBoardVariate._axis03HomeMoveStartVelocity_mmps = _axisHomeMoveStartVelocity_mmps[3];
                        //轴原点复归最大速度
                        _axisMotionControlBoardVariate._axis03HomeMoveMaxVelocity_mmps = _axisHomeMoveMaxVelocity_mmps[3];
                        //轴原点复归加速时间
                        _axisMotionControlBoardVariate._axis03HomeMoveAccTime_s = _axisHomeMoveAccTime_s[3];
                        //轴原点复归寻找原点传感器的速度
                        _axisMotionControlBoardVariate._axis03HomeFindSensorVelocity_mmps = _axisHomeFindSensorVelocity_mmps[3];
                        //轴原点复归找到原点传感器后后退的距离
                        _axisMotionControlBoardVariate._axis03HomeBackDistance_mm = _axisHomeBackDistance_mm[3];

                        //当前位置_脉冲数
                        _axisCurrentPosition_mm[3] = _axisMotionControlBoardVariate._axis03CurrentPosition_mm;
                        //当前轴速度_脉冲数
                        _axisCurrentSpeed_mmps[3] = _axisMotionControlBoardVariate._axis03CurrentSpeed_mmps;
                        //0:NA;1-900:点位号定位;901:Jogging;999:Homing;1000:实时位置数据定位;1001:减速停止
                        _axisCommand[3] = _axisMotionControlBoardVariate._axis03Command;
                        //轴Command保存
                        _axisCommandSaved[3] = _axisMotionControlBoardVariate._axis03CommandSaved;
                        //0:NA;1-900:当前点位号;999:Homed;1000:实时位置数据定位完成
                        _axisCurrentPositionNumber[3] = _axisMotionControlBoardVariate._axis03CurrentPositionNumber;
                        //原点标志
                        _axisHomedFlag[3] = _axisMotionControlBoardVariate._axis03HomedFlag;
                        //错误代码
                        _axisErrorNumber[3] = _axisMotionControlBoardVariate._axis03ErrorNumber;
                        //轴Servo On Flag
                        _axisServoOnOffFlag[3] = _axisMotionControlBoardVariate._axis03ServoOnOffFlag;
                        //轴运行状态
                        _axisCheckDoneStatus[3] = _axisMotionControlBoardVariate._axis03CheckDoneStatus;
                        //轴原点，正负极限状态
                        _axisELMinus[3] = _axisMotionControlBoardVariate._axis03ELMinus;
                        _axisELPlus[3] = _axisMotionControlBoardVariate._axis03ELPlus;
                        _axisORG[3] = _axisMotionControlBoardVariate._axis03ORG;
                        //轴回原点开始Flag
                        _axisHomeStartFlag[3] = _axisMotionControlBoardVariate._axis03HomeStartFlag;
                        //轴运动开始Flag
                        _axisMoveStartFlag[3] = _axisMotionControlBoardVariate._axis03MoveStartFlag;
                    }
                    #endregion

                    #region axis04
                    if (_axisQuantity >= 5)
                    {
                        //axismmPP
                        _axisMotionControlBoardVariate._axis04mmPP = _axismmPP[4];
                        //软负极限
                        _axisMotionControlBoardVariate._axis04softLimitMinus = _axisSoftLimitMinus[4];
                        //软正极限
                        _axisMotionControlBoardVariate._axis04softLimitPlus = _axisSoftLimitPlus[4];
                        //JogMinSpeed
                        _axisMotionControlBoardVariate._axis04JogMinSpeed_mmps = _axisJogMinSpeed_mmps[4];
                        //JogMaxSpeed
                        _axisMotionControlBoardVariate._axis04JogMaxSpeed_mmps = _axisJogMaxSpeed_mmps[4];
                        //InchMinDistance
                        _axisMotionControlBoardVariate._axis04InchMinDistance_mm = _axisInchMinDistance_mm[4];
                        //InchMaxDistance
                        _axisMotionControlBoardVariate._axis04InchMaxDistance_mm = _axisInchMaxDistance_mm[4];
                        //MoveMinSpeed
                        _axisMotionControlBoardVariate._axis04MoveMinSpeed_mmps = _axisMoveMinSpeed_mmps[4];
                        //MoveMaxSpeed
                        _axisMotionControlBoardVariate._axis04MoveMaxSpeed_mmps = _axisMoveMaxSpeed_mmps[4];
                        //MoveminACCTime
                        _axisMotionControlBoardVariate._axis04MoveMinAccTime_s = _axisMoveMinAccTime_s[4];
                        //MoveMaxACCTime
                        _axisMotionControlBoardVariate._axis04MoveMaxAccTime_s = _axisMoveMaxAccTime_s[4];
                        //MinDeviation
                        _axisMotionControlBoardVariate._axis04MinDeviation_mm = _axisMinDeviation_mm[4];
                        //MaxDeviation
                        _axisMotionControlBoardVariate._axis04MaxDeviation_mm = _axisMaxDeviation_mm[4];
                        //轴原点复归起始速度
                        _axisMotionControlBoardVariate._axis04HomeMoveStartVelocity_mmps = _axisHomeMoveStartVelocity_mmps[4];
                        //轴原点复归最大速度
                        _axisMotionControlBoardVariate._axis04HomeMoveMaxVelocity_mmps = _axisHomeMoveMaxVelocity_mmps[4];
                        //轴原点复归加速时间
                        _axisMotionControlBoardVariate._axis04HomeMoveAccTime_s = _axisHomeMoveAccTime_s[4];
                        //轴原点复归寻找原点传感器的速度
                        _axisMotionControlBoardVariate._axis04HomeFindSensorVelocity_mmps = _axisHomeFindSensorVelocity_mmps[4];
                        //轴原点复归找到原点传感器后后退的距离
                        _axisMotionControlBoardVariate._axis04HomeBackDistance_mm = _axisHomeBackDistance_mm[4];

                        //当前位置_脉冲数
                        _axisCurrentPosition_mm[4] = _axisMotionControlBoardVariate._axis04CurrentPosition_mm;
                        //当前轴速度_脉冲数
                        _axisCurrentSpeed_mmps[4] = _axisMotionControlBoardVariate._axis04CurrentSpeed_mmps;
                        //0:NA;1-900:点位号定位;901:Jogging;999:Homing;1000:实时位置数据定位;1001:减速停止
                        _axisCommand[4] = _axisMotionControlBoardVariate._axis04Command;
                        //轴Command保存
                        _axisCommandSaved[4] = _axisMotionControlBoardVariate._axis04CommandSaved;
                        //0:NA;1-900:当前点位号;999:Homed;1000:实时位置数据定位完成
                        _axisCurrentPositionNumber[4] = _axisMotionControlBoardVariate._axis04CurrentPositionNumber;
                        //原点标志
                        _axisHomedFlag[4] = _axisMotionControlBoardVariate._axis04HomedFlag;
                        //错误代码
                        _axisErrorNumber[4] = _axisMotionControlBoardVariate._axis04ErrorNumber;
                        //轴Servo On Flag
                        _axisServoOnOffFlag[4] = _axisMotionControlBoardVariate._axis04ServoOnOffFlag;
                        //轴运行状态
                        _axisCheckDoneStatus[4] = _axisMotionControlBoardVariate._axis04CheckDoneStatus;
                        //轴原点，正负极限状态
                        _axisELMinus[4] = _axisMotionControlBoardVariate._axis04ELMinus;
                        _axisELPlus[4] = _axisMotionControlBoardVariate._axis04ELPlus;
                        _axisORG[4] = _axisMotionControlBoardVariate._axis04ORG;
                        //轴回原点开始Flag
                        _axisHomeStartFlag[4] = _axisMotionControlBoardVariate._axis04HomeStartFlag;
                        //轴运动开始Flag
                        _axisMoveStartFlag[4] = _axisMotionControlBoardVariate._axis04MoveStartFlag;
                    }
                    #endregion

                    #region axis05
                    if (_axisQuantity >= 6)
                    {
                        //axismmPP
                        _axisMotionControlBoardVariate._axis05mmPP = _axismmPP[5];
                        //软负极限
                        _axisMotionControlBoardVariate._axis05softLimitMinus = _axisSoftLimitMinus[5];
                        //软正极限
                        _axisMotionControlBoardVariate._axis05softLimitPlus = _axisSoftLimitPlus[5];
                        //JogMinSpeed
                        _axisMotionControlBoardVariate._axis05JogMinSpeed_mmps = _axisJogMinSpeed_mmps[5];
                        //JogMaxSpeed
                        _axisMotionControlBoardVariate._axis05JogMaxSpeed_mmps = _axisJogMaxSpeed_mmps[5];
                        //InchMinDistance
                        _axisMotionControlBoardVariate._axis05InchMinDistance_mm = _axisInchMinDistance_mm[5];
                        //InchMaxDistance
                        _axisMotionControlBoardVariate._axis05InchMaxDistance_mm = _axisInchMaxDistance_mm[5];
                        //MoveMinSpeed
                        _axisMotionControlBoardVariate._axis05MoveMinSpeed_mmps = _axisMoveMinSpeed_mmps[5];
                        //MoveMaxSpeed
                        _axisMotionControlBoardVariate._axis05MoveMaxSpeed_mmps = _axisMoveMaxSpeed_mmps[5];
                        //MoveminACCTime
                        _axisMotionControlBoardVariate._axis05MoveMinAccTime_s = _axisMoveMinAccTime_s[5];
                        //MoveMaxACCTime
                        _axisMotionControlBoardVariate._axis05MoveMaxAccTime_s = _axisMoveMaxAccTime_s[5];
                        //MinDeviation
                        _axisMotionControlBoardVariate._axis05MinDeviation_mm = _axisMinDeviation_mm[5];
                        //MaxDeviation
                        _axisMotionControlBoardVariate._axis05MaxDeviation_mm = _axisMaxDeviation_mm[5];
                        //轴原点复归起始速度
                        _axisMotionControlBoardVariate._axis05HomeMoveStartVelocity_mmps = _axisHomeMoveStartVelocity_mmps[5];
                        //轴原点复归最大速度
                        _axisMotionControlBoardVariate._axis05HomeMoveMaxVelocity_mmps = _axisHomeMoveMaxVelocity_mmps[5];
                        //轴原点复归加速时间
                        _axisMotionControlBoardVariate._axis05HomeMoveAccTime_s = _axisHomeMoveAccTime_s[5];
                        //轴原点复归寻找原点传感器的速度
                        _axisMotionControlBoardVariate._axis05HomeFindSensorVelocity_mmps = _axisHomeFindSensorVelocity_mmps[5];
                        //轴原点复归找到原点传感器后后退的距离
                        _axisMotionControlBoardVariate._axis05HomeBackDistance_mm = _axisHomeBackDistance_mm[5];

                        //当前位置_脉冲数
                        _axisCurrentPosition_mm[5] = _axisMotionControlBoardVariate._axis05CurrentPosition_mm;
                        //当前轴速度_脉冲数
                        _axisCurrentSpeed_mmps[5] = _axisMotionControlBoardVariate._axis05CurrentSpeed_mmps;
                        //0:NA;1-900:点位号定位;901:Jogging;999:Homing;1000:实时位置数据定位;1001:减速停止
                        _axisCommand[5] = _axisMotionControlBoardVariate._axis05Command;
                        //轴Command保存
                        _axisCommandSaved[5] = _axisMotionControlBoardVariate._axis05CommandSaved;
                        //0:NA;1-900:当前点位号;999:Homed;1000:实时位置数据定位完成
                        _axisCurrentPositionNumber[5] = _axisMotionControlBoardVariate._axis05CurrentPositionNumber;
                        //原点标志
                        _axisHomedFlag[5] = _axisMotionControlBoardVariate._axis05HomedFlag;
                        //错误代码
                        _axisErrorNumber[5] = _axisMotionControlBoardVariate._axis05ErrorNumber;
                        //轴Servo On Flag
                        _axisServoOnOffFlag[5] = _axisMotionControlBoardVariate._axis05ServoOnOffFlag;
                        //轴运行状态
                        _axisCheckDoneStatus[5] = _axisMotionControlBoardVariate._axis05CheckDoneStatus;
                        //轴原点，正负极限状态
                        _axisELMinus[5] = _axisMotionControlBoardVariate._axis05ELMinus;
                        _axisELPlus[5] = _axisMotionControlBoardVariate._axis05ELPlus;
                        _axisORG[5] = _axisMotionControlBoardVariate._axis05ORG;
                        //轴回原点开始Flag
                        _axisHomeStartFlag[5] = _axisMotionControlBoardVariate._axis05HomeStartFlag;
                        //轴运动开始Flag
                        _axisMoveStartFlag[5] = _axisMotionControlBoardVariate._axis05MoveStartFlag;
                    }
                    #endregion

                    #region axis06
                    if (_axisQuantity >= 7)
                    {
                        //axismmPP
                        _axisMotionControlBoardVariate._axis06mmPP = _axismmPP[6];
                        //软负极限
                        _axisMotionControlBoardVariate._axis06softLimitMinus = _axisSoftLimitMinus[6];
                        //软正极限
                        _axisMotionControlBoardVariate._axis06softLimitPlus = _axisSoftLimitPlus[6];
                        //JogMinSpeed
                        _axisMotionControlBoardVariate._axis06JogMinSpeed_mmps = _axisJogMinSpeed_mmps[6];
                        //JogMaxSpeed
                        _axisMotionControlBoardVariate._axis06JogMaxSpeed_mmps = _axisJogMaxSpeed_mmps[6];
                        //InchMinDistance
                        _axisMotionControlBoardVariate._axis06InchMinDistance_mm = _axisInchMinDistance_mm[6];
                        //InchMaxDistance
                        _axisMotionControlBoardVariate._axis06InchMaxDistance_mm = _axisInchMaxDistance_mm[6];
                        //MoveMinSpeed
                        _axisMotionControlBoardVariate._axis06MoveMinSpeed_mmps = _axisMoveMinSpeed_mmps[6];
                        //MoveMaxSpeed
                        _axisMotionControlBoardVariate._axis06MoveMaxSpeed_mmps = _axisMoveMaxSpeed_mmps[6];
                        //MoveminACCTime
                        _axisMotionControlBoardVariate._axis06MoveMinAccTime_s = _axisMoveMinAccTime_s[6];
                        //MoveMaxACCTime
                        _axisMotionControlBoardVariate._axis06MoveMaxAccTime_s = _axisMoveMaxAccTime_s[6];
                        //MinDeviation
                        _axisMotionControlBoardVariate._axis06MinDeviation_mm = _axisMinDeviation_mm[6];
                        //MaxDeviation
                        _axisMotionControlBoardVariate._axis06MaxDeviation_mm = _axisMaxDeviation_mm[6];
                        //轴原点复归起始速度
                        _axisMotionControlBoardVariate._axis06HomeMoveStartVelocity_mmps = _axisHomeMoveStartVelocity_mmps[6];
                        //轴原点复归最大速度
                        _axisMotionControlBoardVariate._axis06HomeMoveMaxVelocity_mmps = _axisHomeMoveMaxVelocity_mmps[6];
                        //轴原点复归加速时间
                        _axisMotionControlBoardVariate._axis06HomeMoveAccTime_s = _axisHomeMoveAccTime_s[6];
                        //轴原点复归寻找原点传感器的速度
                        _axisMotionControlBoardVariate._axis06HomeFindSensorVelocity_mmps = _axisHomeFindSensorVelocity_mmps[6];
                        //轴原点复归找到原点传感器后后退的距离
                        _axisMotionControlBoardVariate._axis06HomeBackDistance_mm = _axisHomeBackDistance_mm[6];

                        //当前位置_脉冲数
                        _axisCurrentPosition_mm[6] = _axisMotionControlBoardVariate._axis06CurrentPosition_mm;
                        //当前轴速度_脉冲数
                        _axisCurrentSpeed_mmps[6] = _axisMotionControlBoardVariate._axis06CurrentSpeed_mmps;
                        //0:NA;1-900:点位号定位;901:Jogging;999:Homing;1000:实时位置数据定位;1001:减速停止
                        _axisCommand[6] = _axisMotionControlBoardVariate._axis06Command;
                        //轴Command保存
                        _axisCommandSaved[6] = _axisMotionControlBoardVariate._axis06CommandSaved;
                        //0:NA;1-900:当前点位号;999:Homed;1000:实时位置数据定位完成
                        _axisCurrentPositionNumber[6] = _axisMotionControlBoardVariate._axis06CurrentPositionNumber;
                        //原点标志
                        _axisHomedFlag[6] = _axisMotionControlBoardVariate._axis06HomedFlag;
                        //错误代码
                        _axisErrorNumber[6] = _axisMotionControlBoardVariate._axis06ErrorNumber;
                        //轴Servo On Flag
                        _axisServoOnOffFlag[6] = _axisMotionControlBoardVariate._axis06ServoOnOffFlag;
                        //轴运行状态
                        _axisCheckDoneStatus[6] = _axisMotionControlBoardVariate._axis06CheckDoneStatus;
                        //轴原点，正负极限状态
                        _axisELMinus[6] = _axisMotionControlBoardVariate._axis06ELMinus;
                        _axisELPlus[6] = _axisMotionControlBoardVariate._axis06ELPlus;
                        _axisORG[6] = _axisMotionControlBoardVariate._axis06ORG;
                        //轴回原点开始Flag
                        _axisHomeStartFlag[6] = _axisMotionControlBoardVariate._axis06HomeStartFlag;
                        //轴运动开始Flag
                        _axisMoveStartFlag[6] = _axisMotionControlBoardVariate._axis06MoveStartFlag;
                    }
                    #endregion

                    #region axis07
                    if (_axisQuantity >= 8)
                    {
                        //axismmPP
                        _axisMotionControlBoardVariate._axis07mmPP = _axismmPP[7];
                        //软负极限
                        _axisMotionControlBoardVariate._axis07softLimitMinus = _axisSoftLimitMinus[7];
                        //软正极限
                        _axisMotionControlBoardVariate._axis07softLimitPlus = _axisSoftLimitPlus[7];
                        //JogMinSpeed
                        _axisMotionControlBoardVariate._axis07JogMinSpeed_mmps = _axisJogMinSpeed_mmps[7];
                        //JogMaxSpeed
                        _axisMotionControlBoardVariate._axis07JogMaxSpeed_mmps = _axisJogMaxSpeed_mmps[7];
                        //InchMinDistance
                        _axisMotionControlBoardVariate._axis07InchMinDistance_mm = _axisInchMinDistance_mm[7];
                        //InchMaxDistance
                        _axisMotionControlBoardVariate._axis07InchMaxDistance_mm = _axisInchMaxDistance_mm[7];
                        //MoveMinSpeed
                        _axisMotionControlBoardVariate._axis07MoveMinSpeed_mmps = _axisMoveMinSpeed_mmps[7];
                        //MoveMaxSpeed
                        _axisMotionControlBoardVariate._axis07MoveMaxSpeed_mmps = _axisMoveMaxSpeed_mmps[7];
                        //MoveminACCTime
                        _axisMotionControlBoardVariate._axis07MoveMinAccTime_s = _axisMoveMinAccTime_s[7];
                        //MoveMaxACCTime
                        _axisMotionControlBoardVariate._axis07MoveMaxAccTime_s = _axisMoveMaxAccTime_s[7];
                        //MinDeviation
                        _axisMotionControlBoardVariate._axis07MinDeviation_mm = _axisMinDeviation_mm[7];
                        //MaxDeviation
                        _axisMotionControlBoardVariate._axis07MaxDeviation_mm = _axisMaxDeviation_mm[7];
                        //轴原点复归起始速度
                        _axisMotionControlBoardVariate._axis07HomeMoveStartVelocity_mmps = _axisHomeMoveStartVelocity_mmps[7];
                        //轴原点复归最大速度
                        _axisMotionControlBoardVariate._axis07HomeMoveMaxVelocity_mmps = _axisHomeMoveMaxVelocity_mmps[7];
                        //轴原点复归加速时间
                        _axisMotionControlBoardVariate._axis07HomeMoveAccTime_s = _axisHomeMoveAccTime_s[7];
                        //轴原点复归寻找原点传感器的速度
                        _axisMotionControlBoardVariate._axis07HomeFindSensorVelocity_mmps = _axisHomeFindSensorVelocity_mmps[7];
                        //轴原点复归找到原点传感器后后退的距离
                        _axisMotionControlBoardVariate._axis07HomeBackDistance_mm = _axisHomeBackDistance_mm[7];

                        //当前位置_脉冲数
                        _axisCurrentPosition_mm[7] = _axisMotionControlBoardVariate._axis07CurrentPosition_mm;
                        //当前轴速度_脉冲数
                        _axisCurrentSpeed_mmps[7] = _axisMotionControlBoardVariate._axis07CurrentSpeed_mmps;
                        //0:NA;1-900:点位号定位;901:Jogging;999:Homing;1000:实时位置数据定位;1001:减速停止
                        _axisCommand[7] = _axisMotionControlBoardVariate._axis07Command;
                        //轴Command保存
                        _axisCommandSaved[7] = _axisMotionControlBoardVariate._axis07CommandSaved;
                        //0:NA;1-900:当前点位号;999:Homed;1000:实时位置数据定位完成
                        _axisCurrentPositionNumber[7] = _axisMotionControlBoardVariate._axis07CurrentPositionNumber;
                        //原点标志
                        _axisHomedFlag[7] = _axisMotionControlBoardVariate._axis07HomedFlag;
                        //错误代码
                        _axisErrorNumber[7] = _axisMotionControlBoardVariate._axis07ErrorNumber;
                        //轴Servo On Flag
                        _axisServoOnOffFlag[7] = _axisMotionControlBoardVariate._axis07ServoOnOffFlag;
                        //轴运行状态
                        _axisCheckDoneStatus[7] = _axisMotionControlBoardVariate._axis07CheckDoneStatus;
                        //轴原点，正负极限状态
                        _axisELMinus[7] = _axisMotionControlBoardVariate._axis07ELMinus;
                        _axisELPlus[7] = _axisMotionControlBoardVariate._axis07ELPlus;
                        _axisORG[7] = _axisMotionControlBoardVariate._axis07ORG;
                        //轴回原点开始Flag
                        _axisHomeStartFlag[7] = _axisMotionControlBoardVariate._axis07HomeStartFlag;
                        //轴运动开始Flag
                        _axisMoveStartFlag[7] = _axisMotionControlBoardVariate._axis07MoveStartFlag;
                    }
                    #endregion

                    #region axis08
                    if (_axisQuantity >= 9)
                    {
                        //axismmPP
                        _axisMotionControlBoardVariate._axis08mmPP = _axismmPP[8];
                        //软负极限
                        _axisMotionControlBoardVariate._axis08softLimitMinus = _axisSoftLimitMinus[8];
                        //软正极限
                        _axisMotionControlBoardVariate._axis08softLimitPlus = _axisSoftLimitPlus[8];
                        //JogMinSpeed
                        _axisMotionControlBoardVariate._axis08JogMinSpeed_mmps = _axisJogMinSpeed_mmps[8];
                        //JogMaxSpeed
                        _axisMotionControlBoardVariate._axis08JogMaxSpeed_mmps = _axisJogMaxSpeed_mmps[8];
                        //InchMinDistance
                        _axisMotionControlBoardVariate._axis08InchMinDistance_mm = _axisInchMinDistance_mm[8];
                        //InchMaxDistance
                        _axisMotionControlBoardVariate._axis08InchMaxDistance_mm = _axisInchMaxDistance_mm[8];
                        //MoveMinSpeed
                        _axisMotionControlBoardVariate._axis08MoveMinSpeed_mmps = _axisMoveMinSpeed_mmps[8];
                        //MoveMaxSpeed
                        _axisMotionControlBoardVariate._axis08MoveMaxSpeed_mmps = _axisMoveMaxSpeed_mmps[8];
                        //MoveminACCTime
                        _axisMotionControlBoardVariate._axis08MoveMinAccTime_s = _axisMoveMinAccTime_s[8];
                        //MoveMaxACCTime
                        _axisMotionControlBoardVariate._axis08MoveMaxAccTime_s = _axisMoveMaxAccTime_s[8];
                        //MinDeviation
                        _axisMotionControlBoardVariate._axis08MinDeviation_mm = _axisMinDeviation_mm[8];
                        //MaxDeviation
                        _axisMotionControlBoardVariate._axis08MaxDeviation_mm = _axisMaxDeviation_mm[8];
                        //轴原点复归起始速度
                        _axisMotionControlBoardVariate._axis08HomeMoveStartVelocity_mmps = _axisHomeMoveStartVelocity_mmps[8];
                        //轴原点复归最大速度
                        _axisMotionControlBoardVariate._axis08HomeMoveMaxVelocity_mmps = _axisHomeMoveMaxVelocity_mmps[8];
                        //轴原点复归加速时间
                        _axisMotionControlBoardVariate._axis08HomeMoveAccTime_s = _axisHomeMoveAccTime_s[8];
                        //轴原点复归寻找原点传感器的速度
                        _axisMotionControlBoardVariate._axis08HomeFindSensorVelocity_mmps = _axisHomeFindSensorVelocity_mmps[8];
                        //轴原点复归找到原点传感器后后退的距离
                        _axisMotionControlBoardVariate._axis08HomeBackDistance_mm = _axisHomeBackDistance_mm[8];

                        //当前位置_脉冲数
                        _axisCurrentPosition_mm[8] = _axisMotionControlBoardVariate._axis08CurrentPosition_mm;
                        //当前轴速度_脉冲数
                        _axisCurrentSpeed_mmps[8] = _axisMotionControlBoardVariate._axis08CurrentSpeed_mmps;
                        //0:NA;1-900:点位号定位;901:Jogging;999:Homing;1000:实时位置数据定位;1001:减速停止
                        _axisCommand[8] = _axisMotionControlBoardVariate._axis08Command;
                        //轴Command保存
                        _axisCommandSaved[8] = _axisMotionControlBoardVariate._axis08CommandSaved;
                        //0:NA;1-900:当前点位号;999:Homed;1000:实时位置数据定位完成
                        _axisCurrentPositionNumber[8] = _axisMotionControlBoardVariate._axis08CurrentPositionNumber;
                        //原点标志
                        _axisHomedFlag[8] = _axisMotionControlBoardVariate._axis08HomedFlag;
                        //错误代码
                        _axisErrorNumber[8] = _axisMotionControlBoardVariate._axis08ErrorNumber;
                        //轴Servo On Flag
                        _axisServoOnOffFlag[8] = _axisMotionControlBoardVariate._axis08ServoOnOffFlag;
                        //轴运行状态
                        _axisCheckDoneStatus[8] = _axisMotionControlBoardVariate._axis08CheckDoneStatus;
                        //轴原点，正负极限状态
                        _axisELMinus[8] = _axisMotionControlBoardVariate._axis08ELMinus;
                        _axisELPlus[8] = _axisMotionControlBoardVariate._axis08ELPlus;
                        _axisORG[8] = _axisMotionControlBoardVariate._axis08ORG;
                        //轴回原点开始Flag
                        _axisHomeStartFlag[8] = _axisMotionControlBoardVariate._axis08HomeStartFlag;
                        //轴运动开始Flag
                        _axisMoveStartFlag[8] = _axisMotionControlBoardVariate._axis08MoveStartFlag;
                    }
                    #endregion

                    #region axis09
                    if (_axisQuantity >= 10)
                    {
                        //axismmPP
                        _axisMotionControlBoardVariate._axis09mmPP = _axismmPP[9];
                        //软负极限
                        _axisMotionControlBoardVariate._axis09softLimitMinus = _axisSoftLimitMinus[9];
                        //软正极限
                        _axisMotionControlBoardVariate._axis09softLimitPlus = _axisSoftLimitPlus[9];
                        //JogMinSpeed
                        _axisMotionControlBoardVariate._axis09JogMinSpeed_mmps = _axisJogMinSpeed_mmps[9];
                        //JogMaxSpeed
                        _axisMotionControlBoardVariate._axis09JogMaxSpeed_mmps = _axisJogMaxSpeed_mmps[9];
                        //InchMinDistance
                        _axisMotionControlBoardVariate._axis09InchMinDistance_mm = _axisInchMinDistance_mm[9];
                        //InchMaxDistance
                        _axisMotionControlBoardVariate._axis09InchMaxDistance_mm = _axisInchMaxDistance_mm[9];
                        //MoveMinSpeed
                        _axisMotionControlBoardVariate._axis09MoveMinSpeed_mmps = _axisMoveMinSpeed_mmps[9];
                        //MoveMaxSpeed
                        _axisMotionControlBoardVariate._axis09MoveMaxSpeed_mmps = _axisMoveMaxSpeed_mmps[9];
                        //MoveminACCTime
                        _axisMotionControlBoardVariate._axis09MoveMinAccTime_s = _axisMoveMinAccTime_s[9];
                        //MoveMaxACCTime
                        _axisMotionControlBoardVariate._axis09MoveMaxAccTime_s = _axisMoveMaxAccTime_s[9];
                        //MinDeviation
                        _axisMotionControlBoardVariate._axis09MinDeviation_mm = _axisMinDeviation_mm[9];
                        //MaxDeviation
                        _axisMotionControlBoardVariate._axis09MaxDeviation_mm = _axisMaxDeviation_mm[9];
                        //轴原点复归起始速度
                        _axisMotionControlBoardVariate._axis09HomeMoveStartVelocity_mmps = _axisHomeMoveStartVelocity_mmps[9];
                        //轴原点复归最大速度
                        _axisMotionControlBoardVariate._axis09HomeMoveMaxVelocity_mmps = _axisHomeMoveMaxVelocity_mmps[9];
                        //轴原点复归加速时间
                        _axisMotionControlBoardVariate._axis09HomeMoveAccTime_s = _axisHomeMoveAccTime_s[9];
                        //轴原点复归寻找原点传感器的速度
                        _axisMotionControlBoardVariate._axis09HomeFindSensorVelocity_mmps = _axisHomeFindSensorVelocity_mmps[9];
                        //轴原点复归找到原点传感器后后退的距离
                        _axisMotionControlBoardVariate._axis09HomeBackDistance_mm = _axisHomeBackDistance_mm[9];

                        //当前位置_脉冲数
                        _axisCurrentPosition_mm[9] = _axisMotionControlBoardVariate._axis09CurrentPosition_mm;
                        //当前轴速度_脉冲数
                        _axisCurrentSpeed_mmps[9] = _axisMotionControlBoardVariate._axis09CurrentSpeed_mmps;
                        //0:NA;1-900:点位号定位;901:Jogging;999:Homing;1000:实时位置数据定位;1001:减速停止
                        _axisCommand[9] = _axisMotionControlBoardVariate._axis09Command;
                        //轴Command保存
                        _axisCommandSaved[9] = _axisMotionControlBoardVariate._axis09CommandSaved;
                        //0:NA;1-900:当前点位号;999:Homed;1000:实时位置数据定位完成
                        _axisCurrentPositionNumber[9] = _axisMotionControlBoardVariate._axis09CurrentPositionNumber;
                        //原点标志
                        _axisHomedFlag[9] = _axisMotionControlBoardVariate._axis09HomedFlag;
                        //错误代码
                        _axisErrorNumber[9] = _axisMotionControlBoardVariate._axis09ErrorNumber;
                        //轴Servo On Flag
                        _axisServoOnOffFlag[9] = _axisMotionControlBoardVariate._axis09ServoOnOffFlag;
                        //轴运行状态
                        _axisCheckDoneStatus[9] = _axisMotionControlBoardVariate._axis09CheckDoneStatus;
                        //轴原点，正负极限状态
                        _axisELMinus[9] = _axisMotionControlBoardVariate._axis09ELMinus;
                        _axisELPlus[9] = _axisMotionControlBoardVariate._axis09ELPlus;
                        _axisORG[9] = _axisMotionControlBoardVariate._axis09ORG;
                        //轴回原点开始Flag
                        _axisHomeStartFlag[9] = _axisMotionControlBoardVariate._axis09HomeStartFlag;
                        //轴运动开始Flag
                        _axisMoveStartFlag[9] = _axisMotionControlBoardVariate._axis09MoveStartFlag;
                    }
                    #endregion

                    #region axis10
                    if (_axisQuantity >= 11)
                    {
                        //axismmPP
                        _axisMotionControlBoardVariate._axis10mmPP = _axismmPP[10];
                        //软负极限
                        _axisMotionControlBoardVariate._axis10softLimitMinus = _axisSoftLimitMinus[10];
                        //软正极限
                        _axisMotionControlBoardVariate._axis10softLimitPlus = _axisSoftLimitPlus[10];
                        //JogMinSpeed
                        _axisMotionControlBoardVariate._axis10JogMinSpeed_mmps = _axisJogMinSpeed_mmps[10];
                        //JogMaxSpeed
                        _axisMotionControlBoardVariate._axis10JogMaxSpeed_mmps = _axisJogMaxSpeed_mmps[10];
                        //InchMinDistance
                        _axisMotionControlBoardVariate._axis10InchMinDistance_mm = _axisInchMinDistance_mm[10];
                        //InchMaxDistance
                        _axisMotionControlBoardVariate._axis10InchMaxDistance_mm = _axisInchMaxDistance_mm[10];
                        //MoveMinSpeed
                        _axisMotionControlBoardVariate._axis10MoveMinSpeed_mmps = _axisMoveMinSpeed_mmps[10];
                        //MoveMaxSpeed
                        _axisMotionControlBoardVariate._axis10MoveMaxSpeed_mmps = _axisMoveMaxSpeed_mmps[10];
                        //MoveminACCTime
                        _axisMotionControlBoardVariate._axis10MoveMinAccTime_s = _axisMoveMinAccTime_s[10];
                        //MoveMaxACCTime
                        _axisMotionControlBoardVariate._axis10MoveMaxAccTime_s = _axisMoveMaxAccTime_s[10];
                        //MinDeviation
                        _axisMotionControlBoardVariate._axis10MinDeviation_mm = _axisMinDeviation_mm[10];
                        //MaxDeviation
                        _axisMotionControlBoardVariate._axis10MaxDeviation_mm = _axisMaxDeviation_mm[10];
                        //轴原点复归起始速度
                        _axisMotionControlBoardVariate._axis10HomeMoveStartVelocity_mmps = _axisHomeMoveStartVelocity_mmps[10];
                        //轴原点复归最大速度
                        _axisMotionControlBoardVariate._axis10HomeMoveMaxVelocity_mmps = _axisHomeMoveMaxVelocity_mmps[10];
                        //轴原点复归加速时间
                        _axisMotionControlBoardVariate._axis10HomeMoveAccTime_s = _axisHomeMoveAccTime_s[10];
                        //轴原点复归寻找原点传感器的速度
                        _axisMotionControlBoardVariate._axis10HomeFindSensorVelocity_mmps = _axisHomeFindSensorVelocity_mmps[10];
                        //轴原点复归找到原点传感器后后退的距离
                        _axisMotionControlBoardVariate._axis10HomeBackDistance_mm = _axisHomeBackDistance_mm[10];

                        //当前位置_脉冲数
                        _axisCurrentPosition_mm[10] = _axisMotionControlBoardVariate._axis10CurrentPosition_mm;
                        //当前轴速度_脉冲数
                        _axisCurrentSpeed_mmps[10] = _axisMotionControlBoardVariate._axis10CurrentSpeed_mmps;
                        //0:NA;1-900:点位号定位;901:Jogging;999:Homing;1000:实时位置数据定位;1001:减速停止
                        _axisCommand[10] = _axisMotionControlBoardVariate._axis10Command;
                        //轴Command保存
                        _axisCommandSaved[10] = _axisMotionControlBoardVariate._axis10CommandSaved;
                        //0:NA;1-900:当前点位号;999:Homed;1000:实时位置数据定位完成
                        _axisCurrentPositionNumber[10] = _axisMotionControlBoardVariate._axis10CurrentPositionNumber;
                        //原点标志
                        _axisHomedFlag[10] = _axisMotionControlBoardVariate._axis10HomedFlag;
                        //错误代码
                        _axisErrorNumber[10] = _axisMotionControlBoardVariate._axis10ErrorNumber;
                        //轴Servo On Flag
                        _axisServoOnOffFlag[10] = _axisMotionControlBoardVariate._axis10ServoOnOffFlag;
                        //轴运行状态
                        _axisCheckDoneStatus[10] = _axisMotionControlBoardVariate._axis10CheckDoneStatus;
                        //轴原点，正负极限状态
                        _axisELMinus[10] = _axisMotionControlBoardVariate._axis10ELMinus;
                        _axisELPlus[10] = _axisMotionControlBoardVariate._axis10ELPlus;
                        _axisORG[10] = _axisMotionControlBoardVariate._axis10ORG;
                        //轴回原点开始Flag
                        _axisHomeStartFlag[10] = _axisMotionControlBoardVariate._axis10HomeStartFlag;
                        //轴运动开始Flag
                        _axisMoveStartFlag[10] = _axisMotionControlBoardVariate._axis10MoveStartFlag;
                    }
                    #endregion

                    #region axis11
                    if (_axisQuantity >= 12)
                    {
                        //axismmPP
                        _axisMotionControlBoardVariate._axis11mmPP = _axismmPP[11];
                        //软负极限
                        _axisMotionControlBoardVariate._axis11softLimitMinus = _axisSoftLimitMinus[11];
                        //软正极限
                        _axisMotionControlBoardVariate._axis11softLimitPlus = _axisSoftLimitPlus[11];
                        //JogMinSpeed
                        _axisMotionControlBoardVariate._axis11JogMinSpeed_mmps = _axisJogMinSpeed_mmps[11];
                        //JogMaxSpeed
                        _axisMotionControlBoardVariate._axis11JogMaxSpeed_mmps = _axisJogMaxSpeed_mmps[11];
                        //InchMinDistance
                        _axisMotionControlBoardVariate._axis11InchMinDistance_mm = _axisInchMinDistance_mm[11];
                        //InchMaxDistance
                        _axisMotionControlBoardVariate._axis11InchMaxDistance_mm = _axisInchMaxDistance_mm[11];
                        //MoveMinSpeed
                        _axisMotionControlBoardVariate._axis11MoveMinSpeed_mmps = _axisMoveMinSpeed_mmps[11];
                        //MoveMaxSpeed
                        _axisMotionControlBoardVariate._axis11MoveMaxSpeed_mmps = _axisMoveMaxSpeed_mmps[11];
                        //MoveminACCTime
                        _axisMotionControlBoardVariate._axis11MoveMinAccTime_s = _axisMoveMinAccTime_s[11];
                        //MoveMaxACCTime
                        _axisMotionControlBoardVariate._axis11MoveMaxAccTime_s = _axisMoveMaxAccTime_s[11];
                        //MinDeviation
                        _axisMotionControlBoardVariate._axis11MinDeviation_mm = _axisMinDeviation_mm[11];
                        //MaxDeviation
                        _axisMotionControlBoardVariate._axis11MaxDeviation_mm = _axisMaxDeviation_mm[11];
                        //轴原点复归起始速度
                        _axisMotionControlBoardVariate._axis11HomeMoveStartVelocity_mmps = _axisHomeMoveStartVelocity_mmps[11];
                        //轴原点复归最大速度
                        _axisMotionControlBoardVariate._axis11HomeMoveMaxVelocity_mmps = _axisHomeMoveMaxVelocity_mmps[11];
                        //轴原点复归加速时间
                        _axisMotionControlBoardVariate._axis11HomeMoveAccTime_s = _axisHomeMoveAccTime_s[11];
                        //轴原点复归寻找原点传感器的速度
                        _axisMotionControlBoardVariate._axis11HomeFindSensorVelocity_mmps = _axisHomeFindSensorVelocity_mmps[11];
                        //轴原点复归找到原点传感器后后退的距离
                        _axisMotionControlBoardVariate._axis11HomeBackDistance_mm = _axisHomeBackDistance_mm[11];

                        //当前位置_脉冲数
                        _axisCurrentPosition_mm[11] = _axisMotionControlBoardVariate._axis11CurrentPosition_mm;
                        //当前轴速度_脉冲数
                        _axisCurrentSpeed_mmps[11] = _axisMotionControlBoardVariate._axis11CurrentSpeed_mmps;
                        //0:NA;1-900:点位号定位;901:Jogging;999:Homing;1000:实时位置数据定位;1001:减速停止
                        _axisCommand[11] = _axisMotionControlBoardVariate._axis11Command;
                        //轴Command保存
                        _axisCommandSaved[11] = _axisMotionControlBoardVariate._axis11CommandSaved;
                        //0:NA;1-900:当前点位号;999:Homed;1000:实时位置数据定位完成
                        _axisCurrentPositionNumber[11] = _axisMotionControlBoardVariate._axis11CurrentPositionNumber;
                        //原点标志
                        _axisHomedFlag[11] = _axisMotionControlBoardVariate._axis11HomedFlag;
                        //错误代码
                        _axisErrorNumber[11] = _axisMotionControlBoardVariate._axis11ErrorNumber;
                        //轴Servo On Flag
                        _axisServoOnOffFlag[11] = _axisMotionControlBoardVariate._axis11ServoOnOffFlag;
                        //轴运行状态
                        _axisCheckDoneStatus[11] = _axisMotionControlBoardVariate._axis11CheckDoneStatus;
                        //轴原点，正负极限状态
                        _axisELMinus[11] = _axisMotionControlBoardVariate._axis11ELMinus;
                        _axisELPlus[11] = _axisMotionControlBoardVariate._axis11ELPlus;
                        _axisORG[11] = _axisMotionControlBoardVariate._axis11ORG;
                        //轴回原点开始Flag
                        _axisHomeStartFlag[11] = _axisMotionControlBoardVariate._axis11HomeStartFlag;
                        //轴运动开始Flag
                        _axisMoveStartFlag[11] = _axisMotionControlBoardVariate._axis11MoveStartFlag;
                    }
                    #endregion

                    #region IO状态获取
                    for (int i = 0; i < _axisMotionControlBoardVariate._cardQuantity * _axisMotionControlBoardVariate._motionCardIOMaxQuantity_320; i++)
                    {
                        _ioStatus[0, i] = _axisMotionControlBoardVariate._inPutStatus[i / _axisMotionControlBoardVariate._motionCardIOMaxQuantity_320][i % _axisMotionControlBoardVariate._motionCardIOMaxQuantity_320];
                        if (_motionCardIOFormVaraite != null && _motionCardIOFormVaraite.Visible)
                            _motionCardIOFormVaraite._ioStatus[0, i] = _ioStatus[0, i];

                        _ioStatus[1, i] = _axisMotionControlBoardVariate._outPutStatus[i / _axisMotionControlBoardVariate._motionCardIOMaxQuantity_320][i % _axisMotionControlBoardVariate._motionCardIOMaxQuantity_320];
                        if (_motionCardIOFormVaraite != null && _motionCardIOFormVaraite.Visible)
                            _motionCardIOFormVaraite._ioStatus[1, i] = _ioStatus[1, i];
                    }
                    #endregion
                }
                Thread.Sleep(5);
            }
        }

        #endregion

        #region AxisStatusUpdataThreadFunc:轴状态更新线程
        /// <summary>
        /// AxisStatusUpdataThreadFunc:轴状态更新线程方法
        /// </summary>
        private void AxisStatusUpdataThreadFunc()
        {
            while (true)
            {
                //当产品名称修改了，对应的AxisPosition数据进行重新读取
                if (_oldProductName != _currentProductName)//如果旧的产品名不等于新的产品名
                {
                    AxisParFileInitial();
                    AxisPosFileInitial();
                }

                //MaLi 20220310 Change 增加轴联合控制页面状态更新程序
                if (_axisUnionFormVariate != null && _axisUnionFormVariate.Visible)//如果不为空并且窗口被创建（不只用不为空的目的是为了确保联合窗口中的数据初始化完成）
                {
                    _axisUnionFormVariate._motionCardInitialFlag = _axisMotionControlBoardVariate._initialFlag;
                    for (int i = 0; i < 4; i++)
                    {
                        _axisUnionFormVariate._axisCurrentPosition_mm[i] = _axisCurrentPosition_mm[i];//当前位置_mm
                        _axisUnionFormVariate._axisUnitArray[i] = _axisUnitArray[i];
                        //软负极限
                        _axisUnionFormVariate._axisSoftLimitMinus[i] = _axisSoftLimitMinus[i];
                        //软正极限
                        _axisUnionFormVariate._axisSoftLimitPlus[i] = _axisSoftLimitPlus[i];

                        //JogMinSpeed
                        _axisUnionFormVariate._axisJogMinSpeed_mmps[i] = _axisJogMinSpeed_mmps[i];
                        //JogMaxSpeed
                        _axisUnionFormVariate._axisJogMaxSpeed_mmps[i] = _axisJogMaxSpeed_mmps[i];
                        //InchMinDistance
                        _axisUnionFormVariate._axisInchMinDistance_mm[i] = _axisInchMinDistance_mm[i];
                        //InchMaxDistance
                        _axisUnionFormVariate._axisInchMaxDistance_mm[i] = _axisInchMaxDistance_mm[i];

                        //MoveMinSpeed
                        _axisUnionFormVariate._axisMoveMinSpeed_mmps[i] = _axisMoveMinSpeed_mmps[i];
                        //MoveMaxSpeed
                        _axisUnionFormVariate._axisMoveMaxSpeed_mmps[i] = _axisMoveMaxSpeed_mmps[i];
                        //MoveminACCTime
                        _axisUnionFormVariate._axisMoveMinAccTime_s[i] = _axisMoveMinAccTime_s[i];
                        //MoveMaxACCTime
                        _axisUnionFormVariate._axisMoveMaxAccTime_s[i] = _axisMoveMaxAccTime_s[i];
                        //MoveminDecTime
                        _axisUnionFormVariate._axisMoveMinDecTime_s[i] = _axisMoveMinDecTime_s[i];
                        //MoveMaxDecTime
                        _axisUnionFormVariate._axisMoveMaxDecTime_s[i] = _axisMoveMaxDecTime_s[i];
                        //AxisMoveStartVelocity
                        _axisUnionFormVariate._axisMoveStartVelocity_mmps[i] = _axisMoveStartVelocity_mmps[i];
                        //AxisMoveStopVelocity
                        _axisUnionFormVariate._axisMoveStopVelocity_mmps[i] = _axisMoveStopVelocity_mmps[i];

                        //轴原点复归标志
                        _axisUnionFormVariate._axisHomedFlag[i] = _axisHomedFlag[i];

                        //绝对位置数据
                        _axisUnionFormVariate._absoluteCoor[i] = _absoluteCoor[i];
                    }
                    //设备模式
                    _axisUnionFormVariate._deviceAutoModeFlag = _deviceAutoModeFlag;

                    _axisUnionFormVariate._useRelativeFlag = _useRelativeFlag;
                }

                //运动控制卡Initial状态
                if (_axisMotionControlBoardVariate != null)
                    _motionCardInitialStatus = _axisMotionControlBoardVariate._initialFlag;


                Thread.Sleep(5);
            }
        }
        #endregion

        #region CtrlUpdataThreadFunc:控件更新线程
        private void CtrlUpdataThreadFunc()
        {
            while (true)
            {
                if (this.Visible)
                {
                    string[] poisitionNumberSelectorNameArray = new string[ONE_PAGE_DISPLAY_POINT_QUANTITY];
                    for (int i = 0; i < ONE_PAGE_DISPLAY_POINT_QUANTITY; i++)
                        poisitionNumberSelectorNameArray[i] = "btnPositionNumberSelector" + (i + 1).ToString("000");

                    if (_axisMotionControlBoardVariate._initialFlag)//如果运动控制卡启动成功
                    {
                        lblAxisMotionCardStatus.Invoke(new Action(() =>
                        {
                            lblAxisMotionCardStatus.ForeColor = Color.LimeGreen;
                            lblAxisMotionCardStatus.BackColor = SystemColors.Control;
                            lblAxisMotionCardStatus.Text = "运动控制卡正常";
                        })); ;

                        if (_motionCardErrorLampBlinkSW != null)
                        {
                            _motionCardErrorLampBlinkSW.Stop();
                            _motionCardErrorLampBlinkSW = null;
                        }
                    }
                    else
                    {
                        if (_motionCardErrorLampBlinkSW == null)
                        {
                            _motionCardErrorLampBlinkSW = new Stopwatch();
                            _motionCardErrorLampBlinkSW.Start();
                        }
                        lblAxisMotionCardStatus.Invoke(new Action(() =>
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
                        #region 基于变量，更新界面按钮控件颜色等
                        //基于当前点位号更新按钮控件背景颜色和字体颜色
                        for (int i = 0; i < ONE_PAGE_DISPLAY_POINT_QUANTITY; i++)
                        {
                            object tempCtrl = this.GetType().GetField(poisitionNumberSelectorNameArray[i], System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);

                            if (_axisCurrentPositionNumber[_currentAxisIndex] > _currentPageIndex * ONE_PAGE_DISPLAY_POINT_QUANTITY
                           && _axisCurrentPositionNumber[_currentAxisIndex] <= (_currentPageIndex + 1) * ONE_PAGE_DISPLAY_POINT_QUANTITY
                                && i == (_axisCurrentPositionNumber[_currentAxisIndex] % ONE_PAGE_DISPLAY_POINT_QUANTITY - 1)
                                && _axisCurrentPositionNumber[_currentAxisIndex] > 0 && _axisCurrentPositionNumber[_currentAxisIndex] < _axisPointQuantity[_currentAxisIndex]
                                && _axisHomedFlag[_currentAxisIndex])//如果当前点位号处于当前页面，并且此control对应的上点位号
                            {
                                ((Button)tempCtrl).BackColor = Color.LimeGreen;
                                ((Button)tempCtrl).ForeColor = Color.Black;
                            }
                            else if (_currentSelectPointNumber > _currentPageIndex * ONE_PAGE_DISPLAY_POINT_QUANTITY
                                && _currentSelectPointNumber <= (_currentPageIndex + 1) * ONE_PAGE_DISPLAY_POINT_QUANTITY
                                 && i == (_currentSelectPointNumber % ONE_PAGE_DISPLAY_POINT_QUANTITY - 1)
                                && _currentSelectPointNumber != _axisCurrentPositionNumber[_currentAxisIndex])
                            {
                                ((Button)tempCtrl).BackColor = Color.Yellow;
                                ((Button)tempCtrl).ForeColor = Color.Black;
                            }
                            else
                            {
                                ((Button)tempCtrl).BackColor = SystemColors.ControlDark;
                                ((Button)tempCtrl).ForeColor = Color.White;
                            }
                        }

                        //基于轴负极限传感器状态对控件进行操作
                        if (_axisELMinus[_currentAxisIndex])
                        {
                            btnAxisMinusLimitSensor.BackColor = Color.Red;
                            btnAxisMinusLimitSensor.ForeColor = Color.Black;
                        }
                        else
                        {
                            btnAxisMinusLimitSensor.BackColor = Color.LimeGreen;
                            btnAxisMinusLimitSensor.ForeColor = Color.Black;
                        }

                        //基于轴正极限传感器状态对控件进行操作
                        if (_axisELPlus[_currentAxisIndex])
                        {
                            btnAxisPlusLimitSensor.BackColor = Color.Red;
                            btnAxisPlusLimitSensor.ForeColor = Color.Black;
                        }
                        else
                        {
                            btnAxisPlusLimitSensor.BackColor = Color.LimeGreen;
                            btnAxisPlusLimitSensor.ForeColor = Color.Black;
                        }

                        //基于轴原点传感器状态对控件进行操作
                        if (_axisORG[_currentAxisIndex])
                        {
                            btnAxisHomeSensor.BackColor = Color.LimeGreen;
                            btnAxisHomeSensor.ForeColor = Color.Black;
                        }
                        else
                        {
                            btnAxisHomeSensor.BackColor = SystemColors.ControlDark;
                            btnAxisHomeSensor.ForeColor = Color.White;
                        }

                        //基于轴的状态对报警复位按钮进行操作
                        if (_axisErrorNumber[_currentAxisIndex] == 0)//如果没有报警
                        {
                            btnAxisAlarmReset.BackColor = Color.Brown;
                            btnAxisAlarmReset.ForeColor = Color.White;
                        }
                        else
                        {
                            btnAxisAlarmReset.BackColor = Color.Red;
                            btnAxisAlarmReset.ForeColor = Color.Black;
                        }

                        //基于伺服状态对伺服ON/OFF按钮进行操作
                        if (_axisServoOnOffFlag[_currentAxisIndex])
                        {
                            btnAxisServoOnOff.BackColor = Color.LimeGreen;
                            btnAxisServoOnOff.ForeColor = Color.White;
                            btnAxisServoOnOff.Text = "伺服开";
                        }
                        else
                        {
                            btnAxisServoOnOff.BackColor = SystemColors.ControlDark;
                            btnAxisServoOnOff.ForeColor = Color.Black;
                            btnAxisServoOnOff.Text = "伺服关";
                        }

                        //基于原点复归状态更新原点复归按钮
                        if (_axisHomedFlag[_currentAxisIndex])
                        {
                            btnAxisHomeBack.BackColor = Color.LimeGreen;
                            btnAxisHomeBack.ForeColor = Color.White;
                            btnAxisHomeBack.Text = "回原点完成";
                        }
                        else
                        {
                            btnAxisHomeBack.BackColor = SystemColors.ControlDark;
                            btnAxisHomeBack.ForeColor = Color.Black;
                            btnAxisHomeBack.Text = "回原点";
                        }

                        //当前轴显示按钮
                        btnAxis01Selector.Text = _axisNameStrArray[0];
                        btnAxis02Selector.Text = _axisNameStrArray[1];
                        btnAxis03Selector.Text = _axisNameStrArray[2];
                        btnAxis04Selector.Text = _axisNameStrArray[3];
                        btnAxis05Selector.Text = _axisNameStrArray[4];
                        btnAxis06Selector.Text = _axisNameStrArray[5];
                        btnAxis07Selector.Text = _axisNameStrArray[6];
                        btnAxis08Selector.Text = _axisNameStrArray[7];
                        if (_currentAxisIndex == 0)
                        {
                            btnAxis01Selector.BackColor = Color.LimeGreen;
                            btnAxis01Selector.ForeColor = Color.Black;
                        }
                        else
                        {
                            btnAxis01Selector.BackColor = SystemColors.ControlDark;
                            btnAxis01Selector.ForeColor = Color.White;
                        }

                        if (_currentAxisIndex == 1)
                        {
                            btnAxis02Selector.BackColor = Color.LimeGreen;
                            btnAxis02Selector.ForeColor = Color.Black;
                        }
                        else
                        {
                            btnAxis02Selector.BackColor = SystemColors.ControlDark;
                            btnAxis02Selector.ForeColor = Color.White;
                        }

                        if (_currentAxisIndex == 2)
                        {
                            btnAxis03Selector.BackColor = Color.LimeGreen;
                            btnAxis03Selector.ForeColor = Color.Black;
                        }
                        else
                        {
                            btnAxis03Selector.BackColor = SystemColors.ControlDark;
                            btnAxis03Selector.ForeColor = Color.White;
                        }

                        if (_currentAxisIndex == 3)
                        {
                            btnAxis04Selector.BackColor = Color.LimeGreen;
                            btnAxis04Selector.ForeColor = Color.Black;
                        }
                        else
                        {
                            btnAxis04Selector.BackColor = SystemColors.ControlDark;
                            btnAxis04Selector.ForeColor = Color.White;
                        }

                        if (_currentAxisIndex == 4)
                        {
                            btnAxis05Selector.BackColor = Color.LimeGreen;
                            btnAxis05Selector.ForeColor = Color.Black;
                        }
                        else
                        {
                            btnAxis05Selector.BackColor = SystemColors.ControlDark;
                            btnAxis05Selector.ForeColor = Color.White;
                        }

                        if (_currentAxisIndex == 5)
                        {
                            btnAxis06Selector.BackColor = Color.LimeGreen;
                            btnAxis06Selector.ForeColor = Color.Black;
                        }
                        else
                        {
                            btnAxis06Selector.BackColor = SystemColors.ControlDark;
                            btnAxis06Selector.ForeColor = Color.White;
                        }

                        if (_currentAxisIndex == 6)
                        {
                            btnAxis07Selector.BackColor = Color.LimeGreen;
                            btnAxis07Selector.ForeColor = Color.Black;
                        }
                        else
                        {
                            btnAxis07Selector.BackColor = SystemColors.ControlDark;
                            btnAxis07Selector.ForeColor = Color.White;
                        }

                        if (_currentAxisIndex == 7)
                        {
                            btnAxis08Selector.BackColor = Color.LimeGreen;
                            btnAxis08Selector.ForeColor = Color.Black;
                        }
                        else
                        {
                            btnAxis08Selector.BackColor = SystemColors.ControlDark;
                            btnAxis08Selector.ForeColor = Color.White;
                        }

                        //点动寸动按钮状态更新
                        if (_inchMoveFlag[_currentAxisIndex])
                        {
                            btnAxisJogOrInchSelector.BackColor = Color.Yellow;
                            btnAxisJogOrInchSelector.ForeColor = Color.Black;
                            btnAxisJogOrInchSelector.Text = "寸动";
                        }
                        else
                        {
                            btnAxisJogOrInchSelector.BackColor = Color.Blue;
                            btnAxisJogOrInchSelector.ForeColor = Color.White;
                            btnAxisJogOrInchSelector.Text = "点动";
                        }

                        #endregion

                        #region 基于变量，更新界面textbox，label控件
                        lblAxisIndexName.Text = "Servo Axis-" + (_currentAxisIndex + 1).ToString("00") + " Servo Control";//更新轴索引名称label
                        lblAxisName.Text = _axisNameStrArray[_currentAxisIndex];//更新轴名称label
                        txtAxisMoveStatus.Text = _axisCheckDoneStatus[_currentAxisIndex].ToString("000");//更新轴运动状态
                        txtAxisCurrentVelocity.Text = _axisCurrentSpeed_mmps[_currentAxisIndex].ToString("f2");//更新轴当前速度
                        txtAxisCurrentMission.Text = _axisCommandSaved[_currentAxisIndex].ToString();//更新当前任务
                        txtAxisCurrentCommad.Text = _axisCommand[_currentAxisIndex].ToString();//更新当前指令
                        txtAxisAlarmCode.Text = _axisErrorNumber[_currentAxisIndex].ToString();//更新报警代码
                        lblAxisCurrentPosition.Text = "当前位置(" + _axisUnitArray[_currentAxisIndex] + ")";//基于单位，更新当前位置标签
                        txtAxisCurrentPosition.Text = _axisCurrentPosition_mm[_currentAxisIndex].ToString("f2");//实时更新当前位置
                        lblAxisJogInchVelocity.Text = "速度(" + _axisUnitArray[_currentAxisIndex] + "/s)";
                        lblAxisInchDistance.Text = "寸动距离(" + _axisUnitArray[_currentAxisIndex] + ")";
                        lblAxisSavedPosition.Text = "保存的位置(" + _axisUnitArray[_currentAxisIndex] + ")";
                        lblAxisSavedVelocity.Text = "保存的速度(" + _axisUnitArray[_currentAxisIndex] + "/s)";
                        lblAxisModifyPosition.Text = "修改的位置(" + _axisUnitArray[_currentAxisIndex] + ")";
                        lblAxisModifyVelocity.Text = "修改的速度(" + _axisUnitArray[_currentAxisIndex] + "/s)";
                        lblAxisManualVelocityPCT.Text = "手动速度:" + _axisManualSpeedPCT[_currentAxisIndex].ToString("f0") + "%";
                        lblAxisAutoVelocityPCT.Text = "自动速度:" + _axisAutoSpeedPCT[_currentAxisIndex].ToString("f0") + "%";

                        //显示当前点位页面号码
                        int maxPageQuantity = Convert.ToInt32((_axisPointQuantity[_currentAxisIndex] - (_axisPointQuantity[_currentAxisIndex] % ONE_PAGE_DISPLAY_POINT_QUANTITY)) / ONE_PAGE_DISPLAY_POINT_QUANTITY);
                        if (_axisPointQuantity[_currentAxisIndex] % ONE_PAGE_DISPLAY_POINT_QUANTITY > 0)
                            maxPageQuantity++;
                        lblPointPage.Text = "当前点位页面：" + (_currentPageIndex + 1).ToString() + @"/" + maxPageQuantity.ToString();

                        //显示当前选中的点位号的数据
                        if (_axisPointDataDT[_currentAxisIndex] != null && !_deviceAutoModeFlag)//如果数据不为空，且为手动模式
                        {
                            if (_currentSelectPointNumber > _axisPointQuantity[_currentAxisIndex])
                                _currentSelectPointNumber = 1;
                            if (_currentSelectPointNumber >= 1 && _currentSelectPointNumber <= _axisPointDataDT[_currentAxisIndex].Rows.Count)
                            {
                                txtAxisSelectedPointNumber.Text = _currentSelectPointNumber.ToString("000");
                                txtAxisSelectedPointName.Text = _axisPointDataDT[_currentAxisIndex].Rows[_currentSelectPointNumber - 1][6].ToString();//点位名称
                                txtAxisSavedPosition.Text = _axisPointDataDT[_currentAxisIndex].Rows[_currentSelectPointNumber - 1][1].ToString();//点位坐标
                                txtAxisSavedVelocity.Text = _axisPointDataDT[_currentAxisIndex].Rows[_currentSelectPointNumber - 1][2].ToString();//点位速度
                                txtAxisSavedAcc.Text = _axisPointDataDT[_currentAxisIndex].Rows[_currentSelectPointNumber - 1][3].ToString();//点位加减速
                                if (_oldSelectPointNumber != _currentSelectPointNumber)//确保修改数据只在变化选中点位号之后显示一次
                                {
                                    txtAxisModifyPosition.Text = _axisPointDataDT[_currentAxisIndex].Rows[_currentSelectPointNumber - 1][1].ToString();//点位坐标
                                    txtAxisModifyVelocity.Text = _axisPointDataDT[_currentAxisIndex].Rows[_currentSelectPointNumber - 1][2].ToString();//点位速度
                                    txtAxisModifyAcc.Text = _axisPointDataDT[_currentAxisIndex].Rows[_currentSelectPointNumber - 1][3].ToString();//点位加减速
                                    _oldSelectPointNumber = _currentSelectPointNumber;
                                }
                            }
                        }

                        //显示点位按钮的名称
                        for (int i = 0; i < ONE_PAGE_DISPLAY_POINT_QUANTITY; i++)
                        {
                            string ctrlName = "btnPositionNumberSelector" + (i + 1).ToString("000");
                            object tempCtrl = this.GetType().GetField(ctrlName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);
                            if (_axisPointDataDT[_currentAxisIndex] != null && (_currentPageIndex * ONE_PAGE_DISPLAY_POINT_QUANTITY + i + 1) < _axisPointDataDT[_currentAxisIndex].Rows.Count)
                            {
                                ((Button)tempCtrl).Text = _axisPointDataDT[_currentAxisIndex].Rows[_currentPageIndex * ONE_PAGE_DISPLAY_POINT_QUANTITY + i][6].ToString();
                            }
                        }

                        #endregion

                    })); ;
                }

                Thread.Sleep(100);
            }
        }
        #endregion

        #region MotionCardInitalFunc：运动控制卡初始化函数，主要用于外部调用
        public void MotionCardInitalFunc()
        {
            if (_axisMotionControlBoardVariate != null)
                _axisMotionControlBoardVariate.Initial();
        }
        #endregion

        #region AxisControlBtnDisplayOrHideFunc:基于当前页面和点位个数，设置点位移动按钮是否显示
        private void AxisControlBtnDisplayOrHideFunc()
        {
            #region 基于当前页面和点位个数，设置点位移动按钮是否显示
            if (ONE_PAGE_DISPLAY_POINT_QUANTITY * (_currentPageIndex + 1) > _axisPointQuantity[_currentAxisIndex])
            {
                string needHideCtrlName = "";
                object tempCtrl;
                int needHideCtrlQuantity = ONE_PAGE_DISPLAY_POINT_QUANTITY * (_currentPageIndex + 1) - _axisPointQuantity[_currentAxisIndex];
                if (needHideCtrlQuantity > ONE_PAGE_DISPLAY_POINT_QUANTITY)
                {
                    int maxPageQuantity = Convert.ToInt32((_axisPointQuantity[_currentAxisIndex] - (_axisPointQuantity[_currentAxisIndex] % ONE_PAGE_DISPLAY_POINT_QUANTITY)) / ONE_PAGE_DISPLAY_POINT_QUANTITY);
                    if (_axisPointQuantity[_currentAxisIndex] % ONE_PAGE_DISPLAY_POINT_QUANTITY > 0)
                        maxPageQuantity++;
                    _currentPageIndex = maxPageQuantity - 1;
                    needHideCtrlQuantity = ONE_PAGE_DISPLAY_POINT_QUANTITY * (_currentPageIndex + 1) - _axisPointQuantity[_currentAxisIndex];
                }
                for (int i = 0; i < needHideCtrlQuantity; i++)
                {
                    needHideCtrlName = "btnPositionNumberSelector" + (ONE_PAGE_DISPLAY_POINT_QUANTITY - i).ToString("000");
                    tempCtrl = this.GetType().GetField(needHideCtrlName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);
                    if (((Button)tempCtrl).Visible)
                        ((Button)tempCtrl).Visible = false;

                    needHideCtrlName = "btnMoveToPosition" + (ONE_PAGE_DISPLAY_POINT_QUANTITY - i).ToString("000");
                    tempCtrl = this.GetType().GetField(needHideCtrlName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);
                    if (((Button)tempCtrl).Visible)
                        ((Button)tempCtrl).Visible = false;
                }
            }
            else
            {
                string needShowCtrlName = "";
                object tempCtrl;
                for (int i = 0; i < ONE_PAGE_DISPLAY_POINT_QUANTITY; i++)
                {
                    needShowCtrlName = "btnPositionNumberSelector" + (i + 1).ToString("000");
                    tempCtrl = this.GetType().GetField(needShowCtrlName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);
                    if (((Button)tempCtrl).Visible == false)
                        ((Button)tempCtrl).Visible = true;


                    needShowCtrlName = "btnMoveToPosition" + (i + 1).ToString("000");
                    tempCtrl = this.GetType().GetField(needShowCtrlName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);
                    if (((Button)tempCtrl).Visible == false)
                        ((Button)tempCtrl).Visible = true;
                }
            }
            #endregion
        }
        #endregion

        #region AxisActionThreadFunc:轴运动判定线程
        private void AxisActionThreadFunc()
        {
            while (true)
            {
                if (_axisMotionControlBoardVariate._initialFlag)
                {
                    #region axis00
                    if (_axisActionCommand[0] == 0)
                    {
                        if (_axisMotionControlBoardVariate._axis00Command != 0 || _axisMotionControlBoardVariate._axis00CheckDoneStatus == 0)
                        {
                            _axisMotionControlBoardVariate.jogAxisImmediateStop(0);
                            if (_axisMotionControlBoardVariate != null)
                            {
                                _axisMotionControlBoardVariate._axis00CurrentPositionNumber = 0;
                                _axisCurrentPositionNumber[0] = 0;
                            }
                        }
                        else
                        {
                            _axisActionCommand[0] = -1;
                        }
                    }
                    else if ((_axisActionCommand[0] >= 1 && _axisActionCommand[0] <= _maxPointQuantity) || (_axisActionCommand[0] - 1000 >= 1 && _axisActionCommand[0] - 1000 <= _maxPointQuantity))
                    {
                        if (_axisMotionControlBoardVariate != null)
                        {
                            if (_axisMotionControlBoardVariate._axis00Command == 0 && _axisMotionControlBoardVariate._axis00CommandSaved == 0
                                && _axisMotionControlBoardVariate._axis00CheckDoneStatus != 0 &&
                                (_axisActionCommand[0] >= 1 && _axisActionCommand[0] <= _maxPointQuantity))//如果轴没有任何指令以及处于停止状态时
                            {
                                _axisMotionControlBoardVariate._axis00CurrentPositionNumber = 0;
                                _axisCurrentPositionNumber[0] = 0;
                                AxisNormalMoveFunction(_axisActionCommand[0], 0, 0, 0, 0, 0, false, 0);
                                _axisActionCommand[0] += 1000;
                            }
                            else if (_axisMotionControlBoardVariate._axis00CurrentPositionNumber == (_axisActionCommand[0] - 1000)
                                && _axisMotionControlBoardVariate._axis00CheckDoneStatus != 0 &&
                                (_axisActionCommand[0] - 1000 >= 1 && _axisActionCommand[0] - 1000 <= _maxPointQuantity))//如果运行完成
                            {
                                _axisActionCommand[0] = -1;
                            }
                        }
                    }
                    else if (_axisActionCommand[0] == 999 || _axisActionCommand[0] == 1999)//如果为回原点
                    {
                        if (_axisMotionControlBoardVariate != null)
                        {
                            if (_axisMotionControlBoardVariate._axis00Command == 0 && _axisMotionControlBoardVariate._axis00CommandSaved == 0
                                && _axisMotionControlBoardVariate._axis00CheckDoneStatus != 0 && _axisActionCommand[0] == 999)//如果轴没有任何指令以及处于停止状态时
                            {
                                _axisMotionControlBoardVariate._axis00HomedFlag = false;
                                _axisMotionControlBoardVariate._axis00CurrentPositionNumber = 0;
                                _axisCurrentPositionNumber[0] = 0;
                                AxisHomeBackFunction(0);
                                _axisActionCommand[0] += 1000;
                            }
                            else if (_axisMotionControlBoardVariate._axis00HomedFlag && _axisMotionControlBoardVariate._axis00CheckDoneStatus != 0 &&
                                    _axisActionCommand[0] == 1999)//如果运行完成
                            {
                                _axisActionCommand[0] = -1;
                            }
                        }
                    }
                    else if (_axisActionCommand[0] == 1000 || _axisActionCommand[0] == 2000)//如果为指定目标点位移动
                    {
                        if (_axisMotionControlBoardVariate != null)
                        {
                            if (_axisMotionControlBoardVariate._axis00Command == 0 && _axisMotionControlBoardVariate._axis00CommandSaved == 0
                                && _axisMotionControlBoardVariate._axis00CheckDoneStatus != 0 && _axisActionCommand[0] == 1000)//如果轴没有任何指令以及处于停止状态时
                            {
                                _axisMotionControlBoardVariate._axis00CurrentPositionNumber = 0;
                                _axisCurrentPositionNumber[0] = 0;
                                AxisNormalMoveFunction(_axisActionCommand[0], 0, _assignedPositionActionMaxVelocity_mmps[0], _assignedPositionActionTargetPosition_mm[0],
                                    _assignedPositionActionAccTime_s[0], _assignedPositionActionDeviation[0], _assignedPositionActionSMoveFlag[0], _assignedPositionActionSMoveTime[0]);
                                _axisActionCommand[0] += 1000;
                            }
                            else if (_axisMotionControlBoardVariate._axis00CurrentPositionNumber == (_axisActionCommand[0] - 1000)
                                    && _axisMotionControlBoardVariate._axis00CheckDoneStatus != 0 && _axisActionCommand[0] == 2000)//如果运行完成
                            {
                                _axisActionCommand[0] = -1;
                            }
                        }
                    }
                    #endregion

                    #region axis01
                    if (_axisActionCommand[1] == 0)
                    {
                        if (_axisMotionControlBoardVariate._axis01Command != 0 || _axisMotionControlBoardVariate._axis01CheckDoneStatus == 0)
                        {
                            _axisMotionControlBoardVariate.jogAxisImmediateStop(1);
                            if (_axisMotionControlBoardVariate != null)
                            {
                                _axisMotionControlBoardVariate._axis01CurrentPositionNumber = 0;
                                _axisCurrentPositionNumber[1] = 0;
                            }
                        }
                        else
                        {
                            _axisActionCommand[1] = -1;
                        }
                    }
                    else if ((_axisActionCommand[1] >= 1 && _axisActionCommand[1] <= _maxPointQuantity) || (_axisActionCommand[1] - 1000 >= 1 && _axisActionCommand[1] - 1000 <= _maxPointQuantity))
                    {
                        if (_axisMotionControlBoardVariate != null)
                        {
                            if (_axisMotionControlBoardVariate._axis01Command == 0 && _axisMotionControlBoardVariate._axis01CommandSaved == 0
                                && _axisMotionControlBoardVariate._axis01CheckDoneStatus != 0 &&
                                (_axisActionCommand[1] >= 1 && _axisActionCommand[1] <= _maxPointQuantity))//如果轴没有任何指令以及处于停止状态时
                            {
                                _axisMotionControlBoardVariate._axis01CurrentPositionNumber = 0;
                                _axisCurrentPositionNumber[1] = 0;
                                AxisNormalMoveFunction(_axisActionCommand[1], 1, 0, 0, 0, 0, false, 0);
                                _axisActionCommand[1] += 1000;
                            }
                            else if (_axisMotionControlBoardVariate._axis01CurrentPositionNumber == (_axisActionCommand[1] - 1000)
                                && _axisMotionControlBoardVariate._axis01CheckDoneStatus != 0 &&
                                (_axisActionCommand[1] - 1000 >= 1 && _axisActionCommand[1] - 1000 <= _maxPointQuantity))//如果运行完成
                            {
                                _axisActionCommand[1] = -1;
                            }
                        }
                    }
                    else if (_axisActionCommand[1] == 999 || _axisActionCommand[1] == 1999)//如果为回原点
                    {
                        if (_axisMotionControlBoardVariate != null)
                        {
                            if (_axisMotionControlBoardVariate._axis01Command == 0 && _axisMotionControlBoardVariate._axis01CommandSaved == 0
                                && _axisMotionControlBoardVariate._axis01CheckDoneStatus != 0 && _axisActionCommand[1] == 999)//如果轴没有任何指令以及处于停止状态时
                            {
                                _axisMotionControlBoardVariate._axis01HomedFlag = false;
                                _axisMotionControlBoardVariate._axis01CurrentPositionNumber = 0;
                                _axisCurrentPositionNumber[1] = 0;
                                AxisHomeBackFunction(1);
                                _axisActionCommand[1] += 1000;
                            }
                            else if (_axisMotionControlBoardVariate._axis01HomedFlag
                                    && _axisMotionControlBoardVariate._axis01CheckDoneStatus != 0 &&
                                    _axisActionCommand[1] == 1999)//如果运行完成
                            {
                                _axisActionCommand[1] = -1;
                            }
                        }
                    }
                    else if (_axisActionCommand[1] == 1000 || _axisActionCommand[1] == 2000)//如果为指定目标点位移动
                    {
                        if (_axisMotionControlBoardVariate != null)
                        {
                            if (_axisMotionControlBoardVariate._axis01Command == 0 && _axisMotionControlBoardVariate._axis01CommandSaved == 0
                                && _axisMotionControlBoardVariate._axis01CheckDoneStatus != 0 && _axisActionCommand[1] == 1000)//如果轴没有任何指令以及处于停止状态时
                            {
                                _axisMotionControlBoardVariate._axis01CurrentPositionNumber = 0;
                                _axisCurrentPositionNumber[1] = 0;
                                AxisNormalMoveFunction(_axisActionCommand[1], 1, _assignedPositionActionMaxVelocity_mmps[1], _assignedPositionActionTargetPosition_mm[1],
                                    _assignedPositionActionAccTime_s[1], _assignedPositionActionDeviation[1], _assignedPositionActionSMoveFlag[1], _assignedPositionActionSMoveTime[1]);
                                _axisActionCommand[1] += 1000;
                            }
                            else if (_axisMotionControlBoardVariate._axis01CurrentPositionNumber == (_axisActionCommand[1] - 1000)
                                    && _axisMotionControlBoardVariate._axis01CheckDoneStatus != 0 && _axisActionCommand[1] == 2000)//如果运行完成
                            {
                                _axisActionCommand[1] = -1;
                            }
                        }
                    }
                    #endregion

                    #region axis02
                    if (_axisActionCommand[2] == 0)
                    {
                        if (_axisMotionControlBoardVariate._axis02Command != 0 || _axisMotionControlBoardVariate._axis02CheckDoneStatus == 0)
                        {
                            _axisMotionControlBoardVariate.jogAxisImmediateStop(2);
                            if (_axisMotionControlBoardVariate != null)
                            {
                                _axisMotionControlBoardVariate._axis02CurrentPositionNumber = 0;
                                _axisCurrentPositionNumber[2] = 0;
                            }
                        }
                        else
                        {
                            _axisActionCommand[2] = -1;
                        }
                    }
                    else if ((_axisActionCommand[2] >= 1 && _axisActionCommand[2] <= _maxPointQuantity) || (_axisActionCommand[2] - 1000 >= 1 && _axisActionCommand[2] - 1000 <= _maxPointQuantity))
                    {
                        if (_axisMotionControlBoardVariate != null)
                        {
                            if (_axisMotionControlBoardVariate._axis02Command == 0 && _axisMotionControlBoardVariate._axis02CommandSaved == 0
                                && _axisMotionControlBoardVariate._axis02CheckDoneStatus != 0 &&
                                (_axisActionCommand[2] >= 1 && _axisActionCommand[2] <= _maxPointQuantity))//如果轴没有任何指令以及处于停止状态时
                            {
                                _axisMotionControlBoardVariate._axis02CurrentPositionNumber = 0;
                                _axisCurrentPositionNumber[2] = 0;
                                AxisNormalMoveFunction(_axisActionCommand[2], 2, 0, 0, 0, 0, false, 0);
                                _axisActionCommand[2] += 1000;
                            }
                            else if (_axisMotionControlBoardVariate._axis02CurrentPositionNumber == (_axisActionCommand[2] - 1000)
                                && _axisMotionControlBoardVariate._axis02CheckDoneStatus != 0 &&
                                (_axisActionCommand[2] - 1000 >= 1 && _axisActionCommand[2] - 1000 <= _maxPointQuantity))//如果运行完成
                            {
                                _axisActionCommand[2] = -1;
                            }
                        }
                    }
                    else if (_axisActionCommand[2] == 999 || _axisActionCommand[2] == 1999)//如果为回原点
                    {
                        if (_axisMotionControlBoardVariate != null)
                        {
                            if (_axisMotionControlBoardVariate._axis02Command == 0 && _axisMotionControlBoardVariate._axis02CommandSaved == 0
                                && _axisMotionControlBoardVariate._axis02CheckDoneStatus != 0 && _axisActionCommand[2] == 999)//如果轴没有任何指令以及处于停止状态时
                            {
                                _axisMotionControlBoardVariate._axis02HomedFlag = false;
                                _axisMotionControlBoardVariate._axis02CurrentPositionNumber = 0;
                                _axisCurrentPositionNumber[2] = 0;
                                AxisHomeBackFunction(2);
                                _axisActionCommand[2] += 1000;
                            }
                            else if (_axisMotionControlBoardVariate._axis02HomedFlag
                                    && _axisMotionControlBoardVariate._axis02CheckDoneStatus != 0 &&
                                    _axisActionCommand[2] == 1999)//如果运行完成
                            {
                                _axisActionCommand[2] = -1;
                            }
                        }
                    }
                    else if (_axisActionCommand[2] == 1000 || _axisActionCommand[2] == 2000)//如果为指定目标点位移动
                    {
                        if (_axisMotionControlBoardVariate != null)
                        {
                            if (_axisMotionControlBoardVariate._axis02Command == 0 && _axisMotionControlBoardVariate._axis02CommandSaved == 0
                                && _axisMotionControlBoardVariate._axis02CheckDoneStatus != 0 && _axisActionCommand[2] == 1000)//如果轴没有任何指令以及处于停止状态时
                            {
                                _axisMotionControlBoardVariate._axis02CurrentPositionNumber = 0;
                                _axisCurrentPositionNumber[2] = 0;
                                AxisNormalMoveFunction(_axisActionCommand[2], 2, _assignedPositionActionMaxVelocity_mmps[2], _assignedPositionActionTargetPosition_mm[2],
                                    _assignedPositionActionAccTime_s[2], _assignedPositionActionDeviation[2], _assignedPositionActionSMoveFlag[2], _assignedPositionActionSMoveTime[2]);
                                _axisActionCommand[2] += 1000;
                            }
                            else if (_axisMotionControlBoardVariate._axis02CurrentPositionNumber == (_axisActionCommand[2] - 1000)
                                    && _axisMotionControlBoardVariate._axis02CheckDoneStatus != 0 && _axisActionCommand[2] == 2000)//如果运行完成
                            {
                                _axisActionCommand[2] = -1;
                            }
                        }
                    }
                    #endregion

                    #region axis03
                    if (_axisActionCommand[3] == 0)
                    {
                        if (_axisMotionControlBoardVariate._axis03Command != 0 || _axisMotionControlBoardVariate._axis03CheckDoneStatus == 0)
                        {
                            _axisMotionControlBoardVariate.jogAxisImmediateStop(3);
                            if (_axisMotionControlBoardVariate != null)
                            {
                                _axisMotionControlBoardVariate._axis03CurrentPositionNumber = 0;
                                _axisCurrentPositionNumber[3] = 0;
                            }
                        }
                        else
                        {
                            _axisActionCommand[3] = -1;
                        }
                    }
                    else if ((_axisActionCommand[3] >= 1 && _axisActionCommand[3] <= _maxPointQuantity) || (_axisActionCommand[3] - 1000 >= 1 && _axisActionCommand[3] - 1000 <= _maxPointQuantity))
                    {
                        if (_axisMotionControlBoardVariate != null)
                        {
                            if (_axisMotionControlBoardVariate._axis03Command == 0 && _axisMotionControlBoardVariate._axis03CommandSaved == 0
                                && _axisMotionControlBoardVariate._axis03CheckDoneStatus != 0 &&
                                (_axisActionCommand[3] >= 1 && _axisActionCommand[3] <= _maxPointQuantity))//如果轴没有任何指令以及处于停止状态时
                            {
                                _axisMotionControlBoardVariate._axis03CurrentPositionNumber = 0;
                                _axisCurrentPositionNumber[3] = 0;
                                AxisNormalMoveFunction(_axisActionCommand[3], 3, 0, 0, 0, 0, false, 0);
                                _axisActionCommand[3] += 1000;
                            }
                            else if (_axisMotionControlBoardVariate._axis03CurrentPositionNumber == (_axisActionCommand[3] - 1000)
                                && _axisMotionControlBoardVariate._axis03CheckDoneStatus != 0 &&
                                (_axisActionCommand[3] - 1000 >= 1 && _axisActionCommand[3] - 1000 <= _maxPointQuantity))//如果运行完成
                            {
                                _axisActionCommand[3] = -1;
                            }
                        }
                    }
                    else if (_axisActionCommand[3] == 999 || _axisActionCommand[3] == 1999)//如果为回原点
                    {
                        if (_axisMotionControlBoardVariate != null)
                        {
                            if (_axisMotionControlBoardVariate._axis03Command == 0 && _axisMotionControlBoardVariate._axis03CommandSaved == 0
                                && _axisMotionControlBoardVariate._axis03CheckDoneStatus != 0 && _axisActionCommand[3] == 999)//如果轴没有任何指令以及处于停止状态时
                            {
                                _axisMotionControlBoardVariate._axis03HomedFlag = false;
                                _axisMotionControlBoardVariate._axis03CurrentPositionNumber = 0;
                                _axisCurrentPositionNumber[3] = 0;
                                AxisHomeBackFunction(3);
                                _axisActionCommand[3] += 1000;
                            }
                            else if (_axisMotionControlBoardVariate._axis03HomedFlag
                                    && _axisMotionControlBoardVariate._axis03CheckDoneStatus != 0 &&
                                    _axisActionCommand[3] == 1999)//如果运行完成
                            {
                                _axisActionCommand[3] = -1;
                            }
                        }
                    }
                    else if (_axisActionCommand[3] == 1000 || _axisActionCommand[3] == 2000)//如果为指定目标点位移动
                    {
                        if (_axisMotionControlBoardVariate != null)
                        {
                            if (_axisMotionControlBoardVariate._axis03Command == 0 && _axisMotionControlBoardVariate._axis03CommandSaved == 0
                                && _axisMotionControlBoardVariate._axis03CheckDoneStatus != 0 && _axisActionCommand[3] == 1000)//如果轴没有任何指令以及处于停止状态时
                            {
                                _axisMotionControlBoardVariate._axis03CurrentPositionNumber = 0;
                                _axisCurrentPositionNumber[3] = 0;
                                AxisNormalMoveFunction(_axisActionCommand[3], 3, _assignedPositionActionMaxVelocity_mmps[3], _assignedPositionActionTargetPosition_mm[3],
                                    _assignedPositionActionAccTime_s[3], _assignedPositionActionDeviation[3], _assignedPositionActionSMoveFlag[3], _assignedPositionActionSMoveTime[3]);
                                _axisActionCommand[3] += 1000;
                            }
                            else if (_axisMotionControlBoardVariate._axis03CurrentPositionNumber == (_axisActionCommand[3] - 1000)
                                    && _axisMotionControlBoardVariate._axis03CheckDoneStatus != 0 && _axisActionCommand[3] == 2000)//如果运行完成
                            {
                                _axisActionCommand[3] = -1;
                            }
                        }
                    }
                    #endregion

                    //#region axis04
                    //if (_axisActionCommand[4] == 0)
                    //{
                    //    if (_axisMotionControlBoardVariate._axis04Command != 0 || _axisMotionControlBoardVariate._axis04CheckDoneStatus == 0)
                    //    {
                    //        _axisMotionControlBoardVariate.jogAxisImmediateStop(4);
                    //        if (_axisMotionControlBoardVariate != null)
                    //        {
                    //            _axisMotionControlBoardVariate._axis04CurrentPositionNumber = 0;
                    //            _axisCurrentPositionNumber[4] = 0;
                    //        }
                    //    }
                    //    else
                    //    {
                    //        _axisActionCommand[4] = -1;
                    //    }
                    //}
                    //else if ((_axisActionCommand[4] >= 1 && _axisActionCommand[4] <= _maxPointQuantity) || (_axisActionCommand[4] - 1000 >= 1 && _axisActionCommand[4] - 1000 <= _maxPointQuantity))
                    //{
                    //    if (_axisMotionControlBoardVariate != null)
                    //    {
                    //        if (_axisMotionControlBoardVariate._axis04Command == 0 && _axisMotionControlBoardVariate._axis04CommandSaved == 0
                    //            && _axisMotionControlBoardVariate._axis04CheckDoneStatus != 0 &&
                    //            (_axisActionCommand[4] >= 1 && _axisActionCommand[4] <= _maxPointQuantity))//如果轴没有任何指令以及处于停止状态时
                    //        {
                    //            _axisMotionControlBoardVariate._axis04CurrentPositionNumber = 0;
                    //            _axisCurrentPositionNumber[4] = 0;
                    //            AxisNormalMoveFunction(_axisActionCommand[4], 4, 0, 0, 0, 0, false, 0);
                    //            _axisActionCommand[4] += 1000;
                    //        }
                    //        else if (_axisMotionControlBoardVariate._axis04CurrentPositionNumber == (_axisActionCommand[4] - 1000)
                    //            && _axisMotionControlBoardVariate._axis04CheckDoneStatus != 0 &&
                    //            (_axisActionCommand[4] - 1000 >= 1 && _axisActionCommand[4] - 1000 <= _maxPointQuantity))//如果运行完成
                    //        {
                    //            _axisActionCommand[4] = -1;
                    //        }
                    //    }
                    //}
                    //else if (_axisActionCommand[4] == 999 || _axisActionCommand[4] == 1999)//如果为回原点
                    //{
                    //    if (_axisMotionControlBoardVariate != null)
                    //    {
                    //        if (_axisMotionControlBoardVariate._axis04Command == 0 && _axisMotionControlBoardVariate._axis04CommandSaved == 0
                    //            && _axisMotionControlBoardVariate._axis04CheckDoneStatus != 0 && _axisActionCommand[4] == 999)//如果轴没有任何指令以及处于停止状态时
                    //        {
                    //            _axisMotionControlBoardVariate._axis04HomedFlag = false;
                    //            _axisMotionControlBoardVariate._axis04CurrentPositionNumber = 0;
                    //            _axisCurrentPositionNumber[4] = 0;
                    //            AxisHomeBackFunction(4);
                    //            _axisActionCommand[4] += 1000;
                    //        }
                    //        else if (_axisMotionControlBoardVariate._axis04HomedFlag
                    //                && _axisMotionControlBoardVariate._axis04CheckDoneStatus != 0 &&
                    //                _axisActionCommand[4] == 1999)//如果运行完成
                    //        {
                    //            _axisActionCommand[4] = -1;
                    //        }
                    //    }
                    //}
                    //else if (_axisActionCommand[4] == 1000 || _axisActionCommand[4] == 2000)//如果为指定目标点位移动
                    //{
                    //    if (_axisMotionControlBoardVariate != null)
                    //    {
                    //        if (_axisMotionControlBoardVariate._axis04Command == 0 && _axisMotionControlBoardVariate._axis04CommandSaved == 0
                    //            && _axisMotionControlBoardVariate._axis04CheckDoneStatus != 0 && _axisActionCommand[4] == 1000)//如果轴没有任何指令以及处于停止状态时
                    //        {
                    //            _axisMotionControlBoardVariate._axis04CurrentPositionNumber = 0;
                    //            _axisCurrentPositionNumber[4] = 0;
                    //            AxisNormalMoveFunction(_axisActionCommand[4], 4, _assignedPositionActionMaxVelocity_mmps[4], _assignedPositionActionTargetPosition_mm[4],
                    //                _assignedPositionActionAccTime_s[4], _assignedPositionActionDeviation[4], _assignedPositionActionSMoveFlag[4], _assignedPositionActionSMoveTime[4]);
                    //            _axisActionCommand[4] += 1000;
                    //        }
                    //        else if (_axisMotionControlBoardVariate._axis04CurrentPositionNumber == (_axisActionCommand[4] - 1000)
                    //                && _axisMotionControlBoardVariate._axis04CheckDoneStatus != 0 && _axisActionCommand[4] == 2000)//如果运行完成
                    //        {
                    //            _axisActionCommand[4] = -1;
                    //        }
                    //    }
                    //}
                    //#endregion

                    //#region axis05
                    //if (_axisActionCommand[5] == 0)
                    //{
                    //    if (_axisMotionControlBoardVariate._axis05Command != 0 || _axisMotionControlBoardVariate._axis05CheckDoneStatus == 0)
                    //    {
                    //        _axisMotionControlBoardVariate.jogAxisImmediateStop(5);
                    //        if (_axisMotionControlBoardVariate != null)
                    //        {
                    //            _axisMotionControlBoardVariate._axis05CurrentPositionNumber = 0;
                    //            _axisCurrentPositionNumber[5] = 0;
                    //        }
                    //    }
                    //    else
                    //    {
                    //        _axisActionCommand[5] = -1;
                    //    }
                    //}
                    //else if ((_axisActionCommand[5] >= 1 && _axisActionCommand[5] <= _maxPointQuantity) || (_axisActionCommand[5] - 1000 >= 1 && _axisActionCommand[5] - 1000 <= _maxPointQuantity))
                    //{
                    //    if (_axisMotionControlBoardVariate != null)
                    //    {
                    //        if (_axisMotionControlBoardVariate._axis05Command == 0 && _axisMotionControlBoardVariate._axis05CommandSaved == 0
                    //            && _axisMotionControlBoardVariate._axis05CheckDoneStatus != 0 &&
                    //            (_axisActionCommand[5] >= 1 && _axisActionCommand[5] <= _maxPointQuantity))//如果轴没有任何指令以及处于停止状态时
                    //        {
                    //            _axisMotionControlBoardVariate._axis05CurrentPositionNumber = 0;
                    //            _axisCurrentPositionNumber[5] = 0;
                    //            AxisNormalMoveFunction(_axisActionCommand[5], 5, 0, 0, 0, 0, false, 0);
                    //            _axisActionCommand[5] += 1000;
                    //        }
                    //        else if (_axisMotionControlBoardVariate._axis05HomedFlag
                    //            && _axisMotionControlBoardVariate._axis05CheckDoneStatus != 0 &&
                    //            (_axisActionCommand[5] - 1000 >= 1 && _axisActionCommand[5] - 1000 <= _maxPointQuantity))//如果运行完成
                    //        {
                    //            _axisActionCommand[5] = -1;
                    //        }
                    //    }
                    //}
                    //else if (_axisActionCommand[5] == 999 || _axisActionCommand[5] == 1999)//如果为回原点
                    //{
                    //    if (_axisMotionControlBoardVariate != null)
                    //    {
                    //        if (_axisMotionControlBoardVariate._axis05Command == 0 && _axisMotionControlBoardVariate._axis05CommandSaved == 0
                    //            && _axisMotionControlBoardVariate._axis05CheckDoneStatus != 0 && _axisActionCommand[5] == 999)//如果轴没有任何指令以及处于停止状态时
                    //        {
                    //            _axisMotionControlBoardVariate._axis05HomedFlag = false;
                    //            _axisMotionControlBoardVariate._axis05CurrentPositionNumber = 0;
                    //            _axisCurrentPositionNumber[5] = 0;
                    //            AxisHomeBackFunction(5);
                    //            _axisActionCommand[5] += 1000;
                    //        }
                    //        else if (_axisMotionControlBoardVariate._axis05CurrentPositionNumber == (_axisActionCommand[5] - 1000)
                    //                && _axisMotionControlBoardVariate._axis05CheckDoneStatus != 0 &&
                    //                _axisActionCommand[5] == 1999)//如果运行完成
                    //        {
                    //            _axisActionCommand[5] = -1;
                    //        }
                    //    }
                    //}
                    //else if (_axisActionCommand[5] == 1000 || _axisActionCommand[5] == 2000)//如果为指定目标点位移动
                    //{
                    //    if (_axisMotionControlBoardVariate != null)
                    //    {
                    //        if (_axisMotionControlBoardVariate._axis05Command == 0 && _axisMotionControlBoardVariate._axis05CommandSaved == 0
                    //            && _axisMotionControlBoardVariate._axis05CheckDoneStatus != 0 && _axisActionCommand[5] == 1000)//如果轴没有任何指令以及处于停止状态时
                    //        {
                    //            _axisMotionControlBoardVariate._axis05CurrentPositionNumber = 0;
                    //            _axisCurrentPositionNumber[5] = 0;
                    //            AxisNormalMoveFunction(_axisActionCommand[5], 5, _assignedPositionActionMaxVelocity_mmps[5], _assignedPositionActionTargetPosition_mm[5],
                    //                _assignedPositionActionAccTime_s[5], _assignedPositionActionDeviation[5], _assignedPositionActionSMoveFlag[5], _assignedPositionActionSMoveTime[5]);
                    //            _axisActionCommand[5] += 1000;
                    //        }
                    //        else if (_axisMotionControlBoardVariate._axis05CurrentPositionNumber == (_axisActionCommand[5] - 1000)
                    //                && _axisMotionControlBoardVariate._axis05CheckDoneStatus != 0 && _axisActionCommand[5] == 2000)//如果运行完成
                    //        {
                    //            _axisActionCommand[5] = -1;
                    //        }
                    //    }
                    //}
                    //#endregion

                    //#region axis06
                    //if (_axisActionCommand[6] == 0)
                    //{
                    //    if (_axisMotionControlBoardVariate._axis06Command != 0 || _axisMotionControlBoardVariate._axis06CheckDoneStatus == 0)
                    //    {
                    //        _axisMotionControlBoardVariate.jogAxisImmediateStop(6);
                    //        if (_axisMotionControlBoardVariate != null)
                    //        {
                    //            _axisMotionControlBoardVariate._axis06CurrentPositionNumber = 0;
                    //            _axisCurrentPositionNumber[6] = 0;
                    //        }
                    //    }
                    //    else
                    //    {
                    //        _axisActionCommand[6] = -1;
                    //    }
                    //}
                    //else if ((_axisActionCommand[6] >= 1 && _axisActionCommand[6] <= _maxPointQuantity) || (_axisActionCommand[6] - 1000 >= 1 && _axisActionCommand[6] - 1000 <= _maxPointQuantity))
                    //{
                    //    if (_axisMotionControlBoardVariate != null)
                    //    {
                    //        if (_axisMotionControlBoardVariate._axis06Command == 0 && _axisMotionControlBoardVariate._axis06CommandSaved == 0
                    //            && _axisMotionControlBoardVariate._axis06CheckDoneStatus != 0 &&
                    //            (_axisActionCommand[6] >= 1 && _axisActionCommand[6] <= _maxPointQuantity))//如果轴没有任何指令以及处于停止状态时
                    //        {
                    //            _axisMotionControlBoardVariate._axis06CurrentPositionNumber = 0;
                    //            _axisCurrentPositionNumber[6] = 0;
                    //            AxisNormalMoveFunction(_axisActionCommand[6], 6, 0, 0, 0, 0, false, 0);
                    //            _axisActionCommand[6] += 1000;
                    //        }
                    //        else if (_axisMotionControlBoardVariate._axis06CurrentPositionNumber == (_axisActionCommand[6] - 1000)
                    //            && _axisMotionControlBoardVariate._axis06CheckDoneStatus != 0 &&
                    //            (_axisActionCommand[6] - 1000 >= 1 && _axisActionCommand[6] - 1000 <= _maxPointQuantity))//如果运行完成
                    //        {
                    //            _axisActionCommand[6] = -1;
                    //        }
                    //    }
                    //}
                    //else if (_axisActionCommand[6] == 999 || _axisActionCommand[6] == 1999)//如果为回原点
                    //{
                    //    if (_axisMotionControlBoardVariate != null)
                    //    {
                    //        if (_axisMotionControlBoardVariate._axis06Command == 0 && _axisMotionControlBoardVariate._axis06CommandSaved == 0
                    //            && _axisMotionControlBoardVariate._axis06CheckDoneStatus != 0 && _axisActionCommand[6] == 999)//如果轴没有任何指令以及处于停止状态时
                    //        {
                    //            _axisMotionControlBoardVariate._axis06HomedFlag = false;
                    //            _axisMotionControlBoardVariate._axis06CurrentPositionNumber = 0;
                    //            _axisCurrentPositionNumber[6] = 0;
                    //            AxisHomeBackFunction(6);
                    //            _axisActionCommand[6] += 1000;
                    //        }
                    //        else if (_axisMotionControlBoardVariate._axis06HomedFlag
                    //                && _axisMotionControlBoardVariate._axis06CheckDoneStatus != 0 &&
                    //                _axisActionCommand[6] == 1999)//如果运行完成
                    //        {
                    //            _axisActionCommand[6] = -1;
                    //        }
                    //    }
                    //}
                    //else if (_axisActionCommand[6] == 1000 || _axisActionCommand[6] == 2000)//如果为指定目标点位移动
                    //{
                    //    if (_axisMotionControlBoardVariate != null)
                    //    {
                    //        if (_axisMotionControlBoardVariate._axis06Command == 0 && _axisMotionControlBoardVariate._axis06CommandSaved == 0
                    //            && _axisMotionControlBoardVariate._axis06CheckDoneStatus != 0 && _axisActionCommand[6] == 1000)//如果轴没有任何指令以及处于停止状态时
                    //        {
                    //            _axisMotionControlBoardVariate._axis06CurrentPositionNumber = 0;
                    //            _axisCurrentPositionNumber[6] = 0;
                    //            AxisNormalMoveFunction(_axisActionCommand[6], 6, _assignedPositionActionMaxVelocity_mmps[6], _assignedPositionActionTargetPosition_mm[6],
                    //                _assignedPositionActionAccTime_s[6], _assignedPositionActionDeviation[6], _assignedPositionActionSMoveFlag[6], _assignedPositionActionSMoveTime[6]);
                    //            _axisActionCommand[6] += 1000;
                    //        }
                    //        else if (_axisMotionControlBoardVariate._axis06CurrentPositionNumber == (_axisActionCommand[6] - 1000)
                    //                && _axisMotionControlBoardVariate._axis06CheckDoneStatus != 0 && _axisActionCommand[6] == 2000)//如果运行完成
                    //        {
                    //            _axisActionCommand[6] = -1;
                    //        }
                    //    }
                    //}
                    //#endregion

                    //#region axis07
                    //if (_axisActionCommand[7] == 0)
                    //{
                    //    if (_axisMotionControlBoardVariate._axis07Command != 0 || _axisMotionControlBoardVariate._axis07CheckDoneStatus == 0)
                    //    {
                    //        _axisMotionControlBoardVariate.jogAxisImmediateStop(7);
                    //        if (_axisMotionControlBoardVariate != null)
                    //        {
                    //            _axisMotionControlBoardVariate._axis07CurrentPositionNumber = 0;
                    //            _axisCurrentPositionNumber[7] = 0;
                    //        }
                    //    }
                    //    else
                    //    {
                    //        _axisActionCommand[7] = -1;
                    //    }
                    //}
                    //else if ((_axisActionCommand[7] >= 1 && _axisActionCommand[7] <= _maxPointQuantity) || (_axisActionCommand[7] - 1000 >= 1 && _axisActionCommand[7] - 1000 <= _maxPointQuantity))
                    //{
                    //    if (_axisMotionControlBoardVariate != null)
                    //    {
                    //        if (_axisMotionControlBoardVariate._axis07Command == 0 && _axisMotionControlBoardVariate._axis07CommandSaved == 0
                    //            && _axisMotionControlBoardVariate._axis07CheckDoneStatus != 0 &&
                    //            (_axisActionCommand[7] >= 1 && _axisActionCommand[7] <= _maxPointQuantity))//如果轴没有任何指令以及处于停止状态时
                    //        {
                    //            _axisMotionControlBoardVariate._axis07CurrentPositionNumber = 0;
                    //            _axisCurrentPositionNumber[7] = 0;
                    //            AxisNormalMoveFunction(_axisActionCommand[7], 7, 0, 0, 0, 0, false, 0);
                    //            _axisActionCommand[7] += 1000;
                    //        }
                    //        else if (_axisMotionControlBoardVariate._axis07CurrentPositionNumber == (_axisActionCommand[7] - 1000)
                    //            && _axisMotionControlBoardVariate._axis07CheckDoneStatus != 0 &&
                    //            (_axisActionCommand[7] - 1000 >= 1 && _axisActionCommand[7] - 1000 <= _maxPointQuantity))//如果运行完成
                    //        {
                    //            _axisActionCommand[7] = -1;
                    //        }
                    //    }
                    //}
                    //else if (_axisActionCommand[7] == 999 || _axisActionCommand[7] == 1999)//如果为回原点
                    //{
                    //    if (_axisMotionControlBoardVariate != null)
                    //    {
                    //        if (_axisMotionControlBoardVariate._axis07Command == 0 && _axisMotionControlBoardVariate._axis07CommandSaved == 0
                    //            && _axisMotionControlBoardVariate._axis07CheckDoneStatus != 0 && _axisActionCommand[7] == 999)//如果轴没有任何指令以及处于停止状态时
                    //        {
                    //            _axisMotionControlBoardVariate._axis07HomedFlag = false;
                    //            _axisMotionControlBoardVariate._axis07CurrentPositionNumber = 0;
                    //            _axisCurrentPositionNumber[7] = 0;
                    //            AxisHomeBackFunction(7);
                    //            _axisActionCommand[7] += 1000;
                    //        }
                    //        else if (_axisMotionControlBoardVariate._axis07HomedFlag
                    //                && _axisMotionControlBoardVariate._axis07CheckDoneStatus != 0 &&
                    //                _axisActionCommand[7] == 1999)//如果运行完成
                    //        {
                    //            _axisActionCommand[7] = -1;
                    //        }
                    //    }
                    //}
                    //else if (_axisActionCommand[7] == 1000 || _axisActionCommand[7] == 2000)//如果为指定目标点位移动
                    //{
                    //    if (_axisMotionControlBoardVariate != null)
                    //    {
                    //        if (_axisMotionControlBoardVariate._axis07Command == 0 && _axisMotionControlBoardVariate._axis07CommandSaved == 0
                    //            && _axisMotionControlBoardVariate._axis07CheckDoneStatus != 0 && _axisActionCommand[7] == 1000)//如果轴没有任何指令以及处于停止状态时
                    //        {
                    //            _axisMotionControlBoardVariate._axis07CurrentPositionNumber = 0;
                    //            _axisCurrentPositionNumber[7] = 0;
                    //            AxisNormalMoveFunction(_axisActionCommand[7], 7, _assignedPositionActionMaxVelocity_mmps[7], _assignedPositionActionTargetPosition_mm[7],
                    //                _assignedPositionActionAccTime_s[7], _assignedPositionActionDeviation[7], _assignedPositionActionSMoveFlag[7], _assignedPositionActionSMoveTime[7]);
                    //            _axisActionCommand[7] += 1000;
                    //        }
                    //        else if (_axisMotionControlBoardVariate._axis07CurrentPositionNumber == (_axisActionCommand[7] - 1000)
                    //                && _axisMotionControlBoardVariate._axis07CheckDoneStatus != 0 && _axisActionCommand[7] == 2000)//如果运行完成
                    //        {
                    //            _axisActionCommand[7] = -1;
                    //        }
                    //    }
                    //}
                    //#endregion

                    Thread.Sleep(2);
                }
            }
        }
        #endregion

        #region AxisParFileInitial：轴参数初始化
        private void AxisParFileInitial()
        {
            string axisParameterFilePath; //轴参数文件保存路径
            string axisParameterFileSaveFolderPath;//轴参数保存文件夹路径

            _axisParFileInitialFinishFlag = false;
            axisParameterFileSaveFolderPath = Directory.GetCurrentDirectory() + @"\" +
                MyTool.FolderAndFileManageClass.FilterPathIllegalChar("AxisParameterAndPositionData_" + _currentProductName) + @"\";//过滤掉作为Directory不合法的字符
            axisParameterFilePath = Path.Combine(axisParameterFileSaveFolderPath, "AxisPar" + ".csv");

            if (!Directory.Exists(axisParameterFileSaveFolderPath))
            {
                Directory.CreateDirectory(axisParameterFileSaveFolderPath);//创建文件夹
            }
            if (!File.Exists(axisParameterFilePath))
            {
                File.Create(axisParameterFilePath).Close();//创建文件
                //初始化文件内容
                CSVFileOperation.SaveCSV_Append(axisParameterFilePath, "AxisNumber(0-N),MinPosition(mm),MaxPosition(mm),MinSpeed(mm/s),MaxSpeed(mm/s),MinAccDecTime(s),MaxAccDecTime(s),MinDeviation(mm),MaxDeviation(mm),StartStopVelocity(mm/s),HomeStartStopVelocity(mm/s),HomeMaxVelocity(mm/s),HomeAccDecTime(s),HomeFindSensorVelocity(mm/s),HomeBackDistance(mm),JogMinSpeed(mm/s),JogMaxSpeed(mm/s),InchMinDistance(mm),InchMaxDistance(mm),ManualSpeedPCT(%),AutoSpeedPCT(%),OnePulseDistance(mm),PointQuantity,AxisUse,AxisName,AxisUnit");
                for (int i = 0; i < _axisQuantity; i++)
                {
                    CSVFileOperation.SaveCSV_Append(axisParameterFilePath, i.ToString("00") + ",0,1000,1,1000,0,10,0,0,10,1,-1,3,1,5,0.1,100,0.001,100,30,100,0.005,120,1,Axis" + i.ToString("00") + ",mm");
                }
                MessageBox.Show("轴参数文件不存在,已创建默认轴参数文件!请根据实际情况修改此文件!");
                if (_axisPositionDataViewFormVariate != null && _axisPositionDataViewFormVariate.Visible)
                {
                    _axisPositionDataViewFormVariate.PositionDataView(_currentProductName, _currentAxisIndex);
                }
            }

            //读取参数
            DataTable dt = new DataTable();
            try
            {
                dt = CSVFileOperation.ReadCSV(axisParameterFilePath);
            }
            catch
            {
                MessageBox.Show("读取轴参数文件失败!");
                return;
            }

            try
            {
                for (int i = 0; i < _axisQuantity; i++)
                {
                    _axisSoftLimitMinus[i] = Convert.ToSingle(dt.Rows[i][1]);
                    _axisSoftLimitPlus[i] = Convert.ToSingle(dt.Rows[i][2]);
                    _axisMoveMinSpeed_mmps[i] = Convert.ToSingle(dt.Rows[i][3]);
                    _axisMoveMaxSpeed_mmps[i] = Convert.ToSingle(dt.Rows[i][4]);
                    _axisMoveMinAccTime_s[i] = Convert.ToSingle(dt.Rows[i][5]);
                    _axisMoveMinDecTime_s[i] = Convert.ToSingle(dt.Rows[i][5]);
                    _axisMoveMaxAccTime_s[i] = Convert.ToSingle(dt.Rows[i][6]);
                    _axisMoveMaxDecTime_s[i] = Convert.ToSingle(dt.Rows[i][6]);
                    _axisMinDeviation_mm[i] = Convert.ToSingle(dt.Rows[i][7]);
                    _axisMaxDeviation_mm[i] = Convert.ToSingle(dt.Rows[i][8]);
                    _axisMoveStartVelocity_mmps[i] = Convert.ToSingle(dt.Rows[i][9]);
                    _axisMoveStopVelocity_mmps[i] = Convert.ToSingle(dt.Rows[i][9]);
                    _axisHomeMoveStartVelocity_mmps[i] = Convert.ToSingle(dt.Rows[i][10]);
                    _axisHomeMoveStopVelocity_mmps[i] = Convert.ToSingle(dt.Rows[i][11]);
                    _axisHomeMoveAccTime_s[i] = Convert.ToSingle(dt.Rows[i][12]);
                    _axisHomeMoveDecTime_s[i] = Convert.ToSingle(dt.Rows[i][12]);
                    _axisHomeFindSensorVelocity_mmps[i] = Convert.ToSingle(dt.Rows[i][13]);
                    _axisHomeBackDistance_mm[i] = Convert.ToSingle(dt.Rows[i][14]);

                    _axisJogMinSpeed_mmps[i] = Convert.ToSingle(dt.Rows[i][15]);
                    _axisJogMaxSpeed_mmps[i] = Convert.ToSingle(dt.Rows[i][16]);
                    _axisInchMinDistance_mm[i] = Convert.ToSingle(dt.Rows[i][17]);
                    _axisInchMaxDistance_mm[i] = Convert.ToSingle(dt.Rows[i][18]);
                    _axisManualSpeedPCT[i] = Convert.ToSingle(dt.Rows[i][19]);
                    _axisAutoSpeedPCT[i] = Convert.ToSingle(dt.Rows[i][20]);
                    _axismmPP[i] = Convert.ToSingle(dt.Rows[i][21]);
                    _axisPointQuantity[i] = Convert.ToInt32(dt.Rows[i][22]);
                    if (_axisPointQuantity[i] > _maxPointQuantity)
                        _axisPointQuantity[i] = _maxPointQuantity;
                    if (dt.Rows[i][23].ToString() == "0")
                        _axisUseFlag[i] = false;
                    else
                        _axisUseFlag[i] = true;
                    _axisNameStrArray[i] = dt.Rows[i][24].ToString();
                    _axisUnitArray[i] = dt.Rows[i][25].ToString();
                }
            }
            catch
            {
                MessageBox.Show("轴参数文件数据错误!");
                bool createParFile = false;
                while (!createParFile)
                {
                    try
                    {
                        File.Create(axisParameterFilePath).Close();//创建文件
                        createParFile = true;
                    }
                    catch
                    {
                        MessageBox.Show("轴数据错误时，重新修改数据文件错误！请检查数据文件是否打开，请关闭打开的数据文件！");
                    }
                }
                //初始化文件内容
                CSVFileOperation.SaveCSV_Append(axisParameterFilePath, "AxisNumber(0-N),MinPosition(mm),MaxPosition(mm),MinSpeed(mm/s),MaxSpeed(mm/s),MinAccDecTime(s),MaxAccDecTime(s),MinDeviation(mm),MaxDeviation(mm),StartStopVelocity(mm/s),HomeStartStopVelocity(mm/s),HomeMaxVelocity(mm/s),HomeAccDecTime(s),HomeFindSensorVelocity(mm/s),HomeBackDistance(mm),JogMinSpeed(mm/s),JogMaxSpeed(mm/s),InchMinDistance(mm),InchMaxDistance(mm),ManualSpeedPCT(%),AutoSpeedPCT(%),OnePulseDistance(mm),PointQuantity,AxisUse,AxisName,AxisUnit");
                for (int i = 0; i < _axisQuantity; i++)
                {
                    CSVFileOperation.SaveCSV_Append(axisParameterFilePath, i.ToString("00") + ",0,1000,1,1000,0,10,0,0,10,1,-1,3,1,5,0.1,100,0.001,100,30,100,0.005,120,1,Axis" + i.ToString("00") + ",mm");
                }
                MessageBox.Show("轴参数文件错误，已重新创建参数文件！");

                try
                {
                    dt = CSVFileOperation.ReadCSV(axisParameterFilePath);
                }
                catch
                {
                    return;
                }

                for (int i = 0; i < _axisQuantity; i++)
                {
                    _axisSoftLimitMinus[i] = Convert.ToSingle(dt.Rows[i][1]);
                    _axisSoftLimitPlus[i] = Convert.ToSingle(dt.Rows[i][2]);
                    _axisMoveMinSpeed_mmps[i] = Convert.ToSingle(dt.Rows[i][3]);
                    _axisMoveMaxSpeed_mmps[i] = Convert.ToSingle(dt.Rows[i][4]);
                    _axisMoveMinAccTime_s[i] = Convert.ToSingle(dt.Rows[i][5]);
                    _axisMoveMinDecTime_s[i] = Convert.ToSingle(dt.Rows[i][5]);
                    _axisMoveMaxAccTime_s[i] = Convert.ToSingle(dt.Rows[i][6]);
                    _axisMoveMaxDecTime_s[i] = Convert.ToSingle(dt.Rows[i][6]);
                    _axisMinDeviation_mm[i] = Convert.ToSingle(dt.Rows[i][7]);
                    _axisMaxDeviation_mm[i] = Convert.ToSingle(dt.Rows[i][8]);
                    _axisMoveStartVelocity_mmps[i] = Convert.ToSingle(dt.Rows[i][9]);
                    _axisMoveStopVelocity_mmps[i] = Convert.ToSingle(dt.Rows[i][9]);
                    _axisHomeMoveStartVelocity_mmps[i] = Convert.ToSingle(dt.Rows[i][10]);
                    _axisHomeMoveStopVelocity_mmps[i] = Convert.ToSingle(dt.Rows[i][11]);
                    _axisHomeMoveAccTime_s[i] = Convert.ToSingle(dt.Rows[i][12]);
                    _axisHomeMoveDecTime_s[i] = Convert.ToSingle(dt.Rows[i][12]);
                    _axisHomeFindSensorVelocity_mmps[i] = Convert.ToSingle(dt.Rows[i][13]);
                    _axisHomeBackDistance_mm[i] = Convert.ToSingle(dt.Rows[i][14]);

                    _axisJogMinSpeed_mmps[i] = Convert.ToSingle(dt.Rows[i][15]);
                    _axisJogMaxSpeed_mmps[i] = Convert.ToSingle(dt.Rows[i][16]);
                    _axisInchMinDistance_mm[i] = Convert.ToSingle(dt.Rows[i][17]);
                    _axisInchMaxDistance_mm[i] = Convert.ToSingle(dt.Rows[i][18]);
                    _axisManualSpeedPCT[i] = Convert.ToSingle(dt.Rows[i][19]);
                    _axisAutoSpeedPCT[i] = Convert.ToSingle(dt.Rows[i][20]);
                    _axismmPP[i] = Convert.ToSingle(dt.Rows[i][21]);
                    _axisPointQuantity[i] = Convert.ToInt32(dt.Rows[i][22]);
                    if (_axisPointQuantity[i] > _maxPointQuantity)
                        _axisPointQuantity[i] = _maxPointQuantity;
                    if (dt.Rows[i][23].ToString() == "0")
                        _axisUseFlag[i] = false;
                    else
                        _axisUseFlag[i] = true;
                    _axisNameStrArray[i] = dt.Rows[i][24].ToString();
                    _axisUnitArray[i] = dt.Rows[i][25].ToString();

                }

                if (_axisPositionDataViewFormVariate != null && _axisPositionDataViewFormVariate.Visible)
                {
                    _axisPositionDataViewFormVariate.PositionDataView(_currentProductName, _currentAxisIndex);
                }
                return;
            }
            AxisManualCtrlDataCheck();

            if (this.Visible)
            {
                this.Invoke(new Action(() =>
                    {
                        #region 根据轴参数进行部分控件的隐藏等
                        int minUsefulAxisCount = -1;
                        for (int i = 0; i < _axisQuantity; i++)
                        {
                            string ctrlName = "btnAxis" + (i + 1).ToString("00") + "Selector";
                            object tempCtrl = this.GetType().GetField(ctrlName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);

                            if (_axisUseFlag[i])//如果轴启用
                            {
                                ((Button)tempCtrl).Visible = true;
                                if (minUsefulAxisCount == -1)
                                    minUsefulAxisCount = i;
                            }
                            else
                                ((Button)tempCtrl).Visible = false;
                        }

                        if (!_axisUseFlag[0] && !_axisUseFlag[1] && !_axisUseFlag[2] && !_axisUseFlag[3])//如果所有轴不可用把对应的轴运动控制按钮无效化
                        {
                            MessageBox.Show("根据参数判定，无轴可以使用！");
                            for (int i = 0; i < ONE_PAGE_DISPLAY_POINT_QUANTITY; i++)
                            {
                                string ctrlName = "btnMoveToPosition" + (i + 1).ToString("000");
                                object tempCtrl = this.GetType().GetField(ctrlName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);
                                ((Button)tempCtrl).Enabled = false;
                            }
                            btnAxisServoOnOff.Enabled = false;
                            btnAxisHomeBack.Enabled = false;
                            btnAxisMinusMove.Enabled = false;
                            btnAxisPlusMove.Enabled = false;
                        }
                        else
                        {
                            for (int i = 0; i < ONE_PAGE_DISPLAY_POINT_QUANTITY; i++)
                            {
                                string ctrlName = "btnMoveToPosition" + (i + 1).ToString("000");
                                object tempCtrl = this.GetType().GetField(ctrlName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);
                                ((Button)tempCtrl).Enabled = true;
                            }
                            btnAxisServoOnOff.Enabled = true;
                            btnAxisHomeBack.Enabled = true;
                            btnAxisMinusMove.Enabled = true;
                            btnAxisPlusMove.Enabled = true;

                            if (!_axisUseFlag[0] || !_axisUseFlag[1] || !_axisUseFlag[2] || !_axisUseFlag[3])
                            {
                                string ctrlName = "btnAxis" + (minUsefulAxisCount + 1).ToString("00") + "Selector";
                                object tempCtrl = this.GetType().GetField(ctrlName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);

                                AxisChangeEvent((Button)tempCtrl, new EventArgs());
                            }
                        }
                        #endregion
                    })); ;
            }

            if (_axisUnionFormVariate != null && _axisUnionFormVariate.Visible)
            {
                _axisUnionFormVariate.ControlHideFunc(_axisUseFlag, _axisNameStrArray);
            }

            _axisParFileInitialFinishFlag = true;
        }
        #endregion

        #region AxisManualCtrlDataCheck:当轴参数发生变化的时候，手动控制，例如JOG，INCH速度距离等的有效范围会发生变化，因此对其进行更新
        private void AxisManualCtrlDataCheck()
        {
            for (int i = 0; i < _axisQuantity; i++)
            {
                if (_axisJogInchVelocity_mmps[i] < _axisJogMinSpeed_mmps[i])
                    _axisJogInchVelocity_mmps[i] = _axisJogMinSpeed_mmps[i];
                else if (_axisJogInchVelocity_mmps[i] > _axisJogMaxSpeed_mmps[i])
                    _axisJogInchVelocity_mmps[i] = _axisJogMaxSpeed_mmps[i];

                if (_axisInchDistance_mm[i] < _axisInchMinDistance_mm[i])
                    _axisInchDistance_mm[i] = _axisInchMinDistance_mm[i];
                else if (_axisInchDistance_mm[i] > _axisInchMaxDistance_mm[i])
                    _axisInchDistance_mm[i] = _axisInchMaxDistance_mm[i];
            }
            txtAxisJogInchVelocity.Text = _axisJogInchVelocity_mmps[_currentAxisIndex].ToString();
            txtAxisInchDistance.Text = _axisInchDistance_mm[_currentAxisIndex].ToString();

            //MaLi 20220310 Change 当主页面的手动控制数据发生变化，对应的union form的数据也进行更新
            if (_axisUnionFormVariate != null && _axisUnionFormVariate.Visible)
            {
                for (int i = 0; i < _axisQuantity; i++)
                {
                    _axisUnionFormVariate._axisJogInchVelocity_mmps[i] = _axisJogInchVelocity_mmps[i];
                    _axisUnionFormVariate._axisInchDistance_mm[i] = _axisInchDistance_mm[i];
                }
                _axisUnionFormVariate.RefreshManualCtrlData();
            }
        }
        #endregion

        #region AxisPosFileInitial:轴点位数据初始化
        private void AxisPosFileInitial()
        {
            _axisPosFileInitialFinishFlag = false;
            string axisPositionFilePath; //轴参数文件保存路径
            string axisPositionFileSaveFolderPath;//轴参数保存文件夹路径
            DataTable axisPositionDataDT = null;
            axisPositionFileSaveFolderPath = Directory.GetCurrentDirectory() + @"\" +
                MyTool.FolderAndFileManageClass.FilterPathIllegalChar("AxisParameterAndPositionData_" + _currentProductName) + @"\";//过滤掉作为Directory不合法的字符
            if (!Directory.Exists(axisPositionFileSaveFolderPath))
            {
                Directory.CreateDirectory(axisPositionFileSaveFolderPath);//创建文件夹
            }

            for (int i = 0; i < _axisQuantity; i++)
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
                if (axisPositionDataDT.Columns.Count < 8)//如果列数不对，全部重新初始化
                {
                    File.Create(axisPositionFilePath).Close();//创建文件
                    //初始化文件内容
                    CSVFileOperation.SaveCSV_Append(axisPositionFilePath, "PointNumber,Position(mm),Speed(mm/s),AccTime(s),Deviation(mm),联合(0/1),名字_中文,Name_English");
                    for (int j = 1; j <= _axisPointQuantity[i]; j++)
                    {
                        CSVFileOperation.SaveCSV_Append(axisPositionFilePath, j.ToString("000") + ",0.00,100,1,0," + "0," + "预留,Spare");
                    }
                    MessageBox.Show("轴" + i.ToString("00") + "点位文件格式不正确,已基于初始模板创重新创建！请根据实际情况修改此文件!");
                    axisPositionDataDT = CSVFileOperation.ReadCSV(axisPositionFilePath);//不包含第一行
                }


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

                try
                {
                    CSVFileOperation.SaveCSV(_axisPointDataDT[i], axisPositionFilePath);
                }
                catch
                {
                    MessageBox.Show("保存轴" + (_currentAxisIndex + 1).ToString("00") + "点位数据失败");
                }

            }

            if (_axisPositionDataViewFormVariate != null && _axisPositionDataViewFormVariate.Visible)
            {
                _axisPositionDataViewFormVariate.PositionDataView(_currentProductName, _currentAxisIndex);
            }
            //MaLi 20220310 Change 随着这里的设定变化，Union Form重新读取数据
            if (_axisUnionFormVariate != null && _axisUnionFormVariate.Visible)
            {
                _axisUnionFormVariate.AxisPosFileInitial(_currentProductName);
            }
            _oldProductName = _currentProductName;
            _axisPosFileInitialFinishFlag = true;
        }
        #endregion

        #region 轴页面切换事件
        private void AxisChangeEvent(object sender, EventArgs e)
        {
            string triggerCtrlName = ((Button)sender).Name;
            int destinationAxisIndex = Convert.ToInt32(((Button)sender).Name.Substring(7, 2)) - 1;
            int tempCurrentPageIndex = 0;
            if (_axisCurrentPositionNumber[destinationAxisIndex] > 0 &&
                _axisCurrentPositionNumber[destinationAxisIndex] < _axisPointQuantity[destinationAxisIndex]
                && _axisHomedFlag[destinationAxisIndex])//如果当前点位号处于当前页面，并且此control对应的上点位号
            {
                tempCurrentPageIndex = _axisCurrentPositionNumber[destinationAxisIndex] / ONE_PAGE_DISPLAY_POINT_QUANTITY;
            }

            _currentAxisIndex = destinationAxisIndex;
            _currentPageIndex = tempCurrentPageIndex;
            _currentSelectPointNumber = 1;
            _oldSelectPointNumber = 0;

            txtAxisJogInchVelocity.Text = _axisJogInchVelocity_mmps[_currentAxisIndex].ToString();
            txtAxisInchDistance.Text = _axisInchDistance_mm[_currentAxisIndex].ToString();

            AxisControlBtnDisplayOrHideFunc();
            //如果轴点位数据窗口处于打开状态，更新显示的点位数据
            if (_axisPositionDataViewFormVariate != null && _axisPositionDataViewFormVariate.Visible)
            {
                _axisPositionDataViewFormVariate.PositionDataView(_currentProductName, _currentAxisIndex);
            }
        }
        #endregion

        #region 轴点位页面切换事件
        private void PageChangeEvent(object sender, EventArgs e)
        {
            string triggerCtrlName = ((Button)sender).Name;

            int maxPageQuantity = Convert.ToInt32((_axisPointQuantity[_currentAxisIndex] - (_axisPointQuantity[_currentAxisIndex] % ONE_PAGE_DISPLAY_POINT_QUANTITY)) / ONE_PAGE_DISPLAY_POINT_QUANTITY);
            if (_axisPointQuantity[_currentAxisIndex] % ONE_PAGE_DISPLAY_POINT_QUANTITY > 0)
                maxPageQuantity++;

            if (triggerCtrlName == "btnPrevPage")//如果是点击上一页按钮触发此事件
            {
                if (_currentPageIndex == 0)
                    _currentPageIndex = maxPageQuantity - 1;
                else
                    _currentPageIndex--;
            }
            else if (triggerCtrlName == "btnNextPage")//如果是点击下一页触发此事件
            {
                if (_currentPageIndex >= maxPageQuantity - 1)
                    _currentPageIndex = 0;
                else
                    _currentPageIndex++;
            }
            AxisControlBtnDisplayOrHideFunc();
        }
        #endregion

        #region 轴点位号切换事件
        private void PointSelectEvent(object sender, EventArgs e)
        {
            int buttonIndex = Convert.ToInt32(((Button)sender).Name.Substring(((Button)sender).Name.Length - 3, 3));
            _currentSelectPointNumber = _currentPageIndex * ONE_PAGE_DISPLAY_POINT_QUANTITY + buttonIndex;

        }
        #endregion

        #region 轴普通运行相关函数

        #region 点位运行事件
        private void PointMoveEvent(object sender, EventArgs e)
        {
            int buttonIndex = Convert.ToInt32(((Button)sender).Name.Substring(((Button)sender).Name.Length - 3, 3));
            int pointNumber = buttonIndex + _currentPageIndex * ONE_PAGE_DISPLAY_POINT_QUANTITY;

            if (!_axisParFileInitialFinishFlag || !_axisPosFileInitialFinishFlag)//如果轴初始化未完成
            {
                if (MessageBox.Show("轴初始化未完成，初始化完成后，再次进行运动控制。是否进行初始化！", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    AxisParFileInitial();
                    return;
                }
                else { return; }
            }

            if (!_axisMotionControlBoardVariate._initialFlag)
            {
                if (MessageBox.Show("运动控制卡初始化失败，无法对轴进行控制！请检查运动控制卡！是否再次进行初始化", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    _axisMotionControlBoardVariate.Initial();
                    return;
                }
            }

            if (_axisCheckDoneStatus[_currentAxisIndex] == 0 || _axisCommand[_currentAxisIndex] != 0)//轴处于运动状态中
            {
                MessageBox.Show("当前轴繁忙，无法进行别的操作！请运行结束后重试！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (_deviceAutoModeFlag)
            {
                MessageBox.Show("当前处于自动状态，无法进行手动操作！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!_axisServoOnOffFlag[_currentAxisIndex])
            {
                MessageBox.Show("轴不处于伺服使能状态，无法进行动作！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!_axisHomedFlag[_currentAxisIndex])
            {
                MessageBox.Show("轴未完成原点回归，无法进行动作！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (_axisErrorNumber[_currentAxisIndex] != 0)
            {
                MessageBox.Show("当前轴处于报警状态，无法进行动作！请消除报警之后重试！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!_axisActionSafeFlag[_currentAxisIndex])
            {
                MessageBox.Show("当前轴处于无法动作状态，请消除限制条件后重试！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            AxisNormalMoveFunction(pointNumber, _currentAxisIndex, 0, 0, 0, 0, false, 0);
        }
        #endregion

        #region AxisNormalMoveFunction
        /// <summary>
        /// AxisNormalMoveFunction:轴运动相关
        /// </summary>
        /// <param name="command">int:轴运行指令，1-900为指定点位移动，1000为指定目标运动</param>
        /// <param name="axisIndex">int:轴索引号，0-轴1，1-轴2...</param>
        /// <param name="assignedPositionActionMaxVelocity_mmps">float:当为指定目标运动时的最大速度</param>
        /// <param name="assignedPositionActionTargetPosition_mm">float:当为指定目标运动时的目标坐标</param>
        /// <param name="assignedPositionActionAccTime_s">float:当为指定目标运动时的加减速时间</param>
        /// <param name="assignedPositionActionDeviation">float:当为指定目标运动时的deviation</param>
        /// <param name="assignedPositionActionSMoveFlag">bool:当为指定目标运动时的S移动启用与否，false-不启用，true-启用</param>
        /// <param name="assignedPositionActionSMoveTime">double:当为指定目标运动启用S移动时S移动时间</param>
        private void AxisNormalMoveFunction(int command, int axisIndex, float assignedPositionActionMaxVelocity_mmps,
            float assignedPositionActionTargetPosition_mm, float assignedPositionActionAccTime_s, float assignedPositionActionDeviation, bool assignedPositionActionSMoveFlag, double assignedPositionActionSMoveTime)
        {
            if (command < 1 && command > _axisPointQuantity[axisIndex] && command != 1000)
            {
                MessageBox.Show("指令命令号有误，无法进行动作！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (axisIndex == 3 || axisIndex == 4 || axisIndex == 5)//如果为传送线宽度调节轴
            {
                _axisMotionControlBoardVariate.OutputCtrl(22, true);
                Thread.Sleep(10);
                while (!_ioStatus[1, 22])
                {
                    Thread.Sleep(10);
                    if (!_ioStatus[0, 7])//如果按下急停按钮
                    {
                        return;
                    }
                }
            }

            if (command != 1000)//如果不为指定目标运动
            {
                if (_useRelativeFlag)//如果使用相对位置
                {
                    if (_deviceAutoModeFlag)
                        _axisMotionControlBoardVariate.AxisAction(command, axisIndex, _axisMoveStartVelocity_mmps[axisIndex], Convert.ToSingle(_axisPointDataDT[axisIndex].Rows[command - 1][2]) * _axisAutoSpeedPCT[axisIndex] / 100f,
                                    _axisMoveStopVelocity_mmps[axisIndex], Convert.ToSingle(_axisPointDataDT[axisIndex].Rows[command - 1][3]), Convert.ToSingle(_axisPointDataDT[axisIndex].Rows[command - 1][3]),
                                    Convert.ToSingle(_axisPointDataDT[axisIndex].Rows[command - 1][1]), false, false, Convert.ToSingle(_axisPointDataDT[axisIndex].Rows[command - 1][4]), false, 0);
                    else
                        _axisMotionControlBoardVariate.AxisAction(command, axisIndex, _axisMoveStartVelocity_mmps[axisIndex], Convert.ToSingle(_axisPointDataDT[axisIndex].Rows[command - 1][2]) * _axisManualSpeedPCT[axisIndex] / 100f,
                                    _axisMoveStopVelocity_mmps[axisIndex], Convert.ToSingle(_axisPointDataDT[axisIndex].Rows[command - 1][3]), Convert.ToSingle(_axisPointDataDT[axisIndex].Rows[command - 1][3]),
                                    Convert.ToSingle(_axisPointDataDT[axisIndex].Rows[command - 1][1]), false, false, Convert.ToSingle(_axisPointDataDT[axisIndex].Rows[command - 1][4]), false, 0);
                }
                else
                {
                    if (_deviceAutoModeFlag)
                        _axisMotionControlBoardVariate.AxisAction(command, axisIndex, _axisMoveStartVelocity_mmps[axisIndex], Convert.ToSingle(_axisPointDataDT[axisIndex].Rows[command - 1][2]) * _axisAutoSpeedPCT[axisIndex] / 100f,
                                    _axisMoveStopVelocity_mmps[axisIndex], Convert.ToSingle(_axisPointDataDT[axisIndex].Rows[command - 1][3]), Convert.ToSingle(_axisPointDataDT[axisIndex].Rows[command - 1][3]),
                                    Convert.ToSingle(_axisPointDataDT[axisIndex].Rows[command - 1][1]) + _absoluteCoor[axisIndex], false, false, Convert.ToSingle(_axisPointDataDT[axisIndex].Rows[command - 1][4]), false, 0);
                    else
                        _axisMotionControlBoardVariate.AxisAction(command, axisIndex, _axisMoveStartVelocity_mmps[axisIndex], Convert.ToSingle(_axisPointDataDT[axisIndex].Rows[command - 1][2]) * _axisManualSpeedPCT[axisIndex] / 100f,
                                   _axisMoveStopVelocity_mmps[axisIndex], Convert.ToSingle(_axisPointDataDT[axisIndex].Rows[command - 1][3]), Convert.ToSingle(_axisPointDataDT[axisIndex].Rows[command - 1][3]),
                                   Convert.ToSingle(_axisPointDataDT[axisIndex].Rows[command - 1][1]) + _absoluteCoor[axisIndex], false, false, Convert.ToSingle(_axisPointDataDT[axisIndex].Rows[command - 1][4]), false, 0);
                }
            }
            else
            {
                if (_deviceAutoModeFlag)//如果为自动模式
                {
                    assignedPositionActionMaxVelocity_mmps = assignedPositionActionMaxVelocity_mmps * _axisAutoSpeedPCT[axisIndex] / 100f;
                }
                else
                {
                    assignedPositionActionMaxVelocity_mmps = assignedPositionActionMaxVelocity_mmps * _axisManualSpeedPCT[axisIndex] / 100f;
                }

                if (_useRelativeFlag)//如果使用相对位置
                {
                    _axisMotionControlBoardVariate.AxisAction(command, axisIndex, _axisMoveStartVelocity_mmps[axisIndex], assignedPositionActionMaxVelocity_mmps,
                                    _axisMoveStartVelocity_mmps[axisIndex], assignedPositionActionAccTime_s, assignedPositionActionAccTime_s,
                                    assignedPositionActionTargetPosition_mm, false, false, assignedPositionActionDeviation, assignedPositionActionSMoveFlag, assignedPositionActionSMoveTime);
                }
                else
                {
                    _axisMotionControlBoardVariate.AxisAction(command, axisIndex, _axisMoveStartVelocity_mmps[axisIndex], assignedPositionActionMaxVelocity_mmps,
                                    _axisMoveStartVelocity_mmps[axisIndex], assignedPositionActionAccTime_s, assignedPositionActionAccTime_s,
                                    assignedPositionActionTargetPosition_mm + _absoluteCoor[axisIndex], false, false, assignedPositionActionDeviation, assignedPositionActionSMoveFlag, assignedPositionActionSMoveTime);
                }
            }

        }
        #endregion

        #endregion

        #region 点动寸动切换
        private void btnAxisJogOrInchSelector_Click(object sender, EventArgs e)
        {
            if (_inchMoveFlag[_currentAxisIndex])
            {
                _inchMoveFlag[_currentAxisIndex] = false;
            }
            else
            {
                _inchMoveFlag[_currentAxisIndex] = true;
            }
            //MaLi 20220310 Change 随着这里的设定变化，Union Form的也要跟着变
            if (_axisUnionFormVariate != null && _axisUnionFormVariate.Visible)
            {
                for (int i = 0; i < _axisQuantity; i++)
                    _axisUnionFormVariate._axisInchFlag[i] = _inchMoveFlag[i];
            }
        }
        #endregion

        #region Position Data View相关函数
        //点击点位预览按钮事件
        private void PositionDataViewEvent(object sender, EventArgs e)
        {
            if (_axisPositionDataViewFormVariate == null)
            {
                _axisPositionDataViewFormVariate = new AxisPositionDataViewForm(_currentProductName, _currentAxisIndex);
                _axisPositionDataViewFormVariate.Show();
                _axisPositionDataViewFormVariate.AxisPositionDataViewFormCloseEvent += new AxisPositionDataViewFormCloseDelegate(AxisPositionDataViewFormCloseEventFunction);
                _axisPositionDataViewFormVariate.AxisPositionDataModifiedEvent += new AxisPositionDataModifiedDelegate(AxisPositionDataModifiedEventFunction);
                _axisPositionDataViewFormVariate.AxisParameterDataModifiedEvent += new AxisParameterDataModifiedDelegate(AxisParameterDataModifiedEventFunction);
                return;
            }
            if (_axisPositionDataViewFormVariate != null && _axisPositionDataViewFormVariate.Visible)
            {
                _axisPositionDataViewFormVariate.Hide();//确保多线程中不会误判
                Thread.Sleep(100);
                _axisPositionDataViewFormVariate.Dispose();
                _axisPositionDataViewFormVariate = null;
            }
        }

        //点位预览窗口关闭事件
        private void AxisPositionDataViewFormCloseEventFunction()
        {
            if (_axisPositionDataViewFormVariate != null && _axisPositionDataViewFormVariate.Visible)
            {
                _axisPositionDataViewFormVariate.Dispose();
                _axisPositionDataViewFormVariate = null;
            }
        }

        //点位数据修改事件
        private void AxisPositionDataModifiedEventFunction()
        {
            AxisPosFileInitial();
            _oldSelectPointNumber = 0;
        }

        //轴参数修改事件
        private void AxisParameterDataModifiedEventFunction()
        {
            AxisParFileInitial();
        }

        #endregion

        #region Servo On/OFF change
        private void btnAxisServoOnOff_Click(object sender, EventArgs e)
        {
            if (_deviceAutoModeFlag && _axisServoOnOffFlag[_currentAxisIndex])
            {
                MessageBox.Show("当前轴处于自动状态，无法进行伺服去使能操作！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if ((_axisCommand[_currentAxisIndex] != 0 || _axisCheckDoneStatus[_currentAxisIndex] == 0) && _axisServoOnOffFlag[_currentAxisIndex])
            {
                MessageBox.Show("当前轴处于运动过程中，无法进行伺服去使能操作！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (_axisServoOnOffFlag[_currentAxisIndex])
                //_axisMotionControlBoardVariate.OutputCtrl((_currentAxisIndex / 4) * 64 + _currentAxisIndex % 4, false);
                _axisMotionControlBoardVariate.servoOffOn(_currentAxisIndex, false);

            else
                //_axisMotionControlBoardVariate.OutputCtrl((_currentAxisIndex / 4) * 64 + _currentAxisIndex % 4, true);
                _axisMotionControlBoardVariate.servoOffOn(_currentAxisIndex, true);
        }
        #endregion

        #region 轴报警复位
        private void btnAxisAlarmReset_Click(object sender, EventArgs e)
        {
            //_axisMotionControlBoardVariate.OutputCtrl(16, true);
            _axisMotionControlBoardVariate.servoAlarmClear(_currentAxisIndex, true);
        }
        #endregion

        #region 原点复归相关
        //Axis home back bt
        private void btnAxisHomeBack_Click(object sender, EventArgs e)
        {
            if (!_axisParFileInitialFinishFlag)//如果轴初始化未完成
            {
                if (MessageBox.Show("轴初始化未完成，初始化完成后，再次进行运动控制。是否进行初始化！", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    AxisParFileInitial();
                    return;
                }
                else { return; }
            }

            if (!_axisMotionControlBoardVariate._initialFlag)
            {
                if (MessageBox.Show("运动控制卡初始化失败，无法对轴进行控制！请检查运动控制卡！是否再次进行初始化", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    _axisMotionControlBoardVariate.Initial();
                    return;
                }
            }

            if (_axisCheckDoneStatus[_currentAxisIndex] == 0 || _axisCommand[_currentAxisIndex] != 0)//轴处于运动状态中
            {
                MessageBox.Show("当前轴繁忙，无法进行回原点操作！请运行结束后重试！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (_deviceAutoModeFlag)
            {
                MessageBox.Show("当前处于自动状态，无法进行回原点操作！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!_axisServoOnOffFlag[_currentAxisIndex])
            {
                MessageBox.Show("轴不处于伺服使能状态，无法进行回原点操作！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (_axisErrorNumber[_currentAxisIndex] != 0)
            {
                MessageBox.Show("当前轴处于报警状态，无法进行动作！请消除报警之后重试！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!_axisActionSafeFlag[_currentAxisIndex])
            {
                MessageBox.Show("当前轴处于无法动作状态，请消除限制条件后重试！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            AxisHomeBackFunction(_currentAxisIndex);
        }

        /// <summary>
        /// AxisHomeBackFunction:原点复归函数
        /// </summary>
        /// <param name="axisIndex">int:轴索引号，0-轴1，1-轴2...</param>
        private void AxisHomeBackFunction(int axisIndex)
        {
            if (axisIndex == 3 || axisIndex == 4 || axisIndex == 5)//如果为传送线宽度调节轴
            {
                _axisMotionControlBoardVariate.OutputCtrl(22, true);
                Thread.Sleep(10);
                while (!_ioStatus[1, 22])
                {
                    Thread.Sleep(10);
                    if (!_ioStatus[0, 7])//如果按下急停按钮
                    {
                        return;
                    }
                }
            }


            _axisMotionControlBoardVariate.AxisAction(999, axisIndex, _axisHomeMoveStartVelocity_mmps[axisIndex], _axisHomeMoveMaxVelocity_mmps[axisIndex],
                _axisHomeMoveStopVelocity_mmps[axisIndex], _axisHomeMoveAccTime_s[axisIndex], _axisHomeMoveDecTime_s[axisIndex],
               0, false, false, 0, false, 0);
        }
        #endregion

        #region JogInch正负移动按钮相关事件
        /// <summary>
        /// AxisJogInchMoveFunction：轴JOG或INCH移动事件
        /// </summary>
        /// <param name="axisIndex">int:轴索引号，0-轴1，1-轴2...</param>
        /// <param name="moveDirection">bool:轴移动方向，false-负方向，true-正方向</param>
        /// <param name="inchFlag">bool:inch标志，false-点动，true-寸动</param>
        private void AxisJogInchMoveFunction(int axisIndex, bool moveDirection, bool inchFlag)
        {
            if (!_axisParFileInitialFinishFlag)//如果轴初始化未完成
            {
                if (MessageBox.Show("轴初始化未完成，初始化完成后，再次进行运动控制。是否进行初始化！", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    AxisParFileInitial();
                    return;
                }
                else { return; }
            }

            if (!_axisMotionControlBoardVariate._initialFlag)
            {
                if (MessageBox.Show("运动控制卡初始化失败，无法对轴进行控制！请检查运动控制卡！是否再次进行初始化", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    _axisMotionControlBoardVariate.Initial();
                    return;
                }
            }

            if (_axisCheckDoneStatus[axisIndex] == 0)//轴处于运动状态中|| _axisCommand[axisIndex] != 0 || _axisCommandSaved[axisIndex] != 0
            {
                //MessageBox.Show("当前轴繁忙，无法进行别的操作！请运行结束后重试！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (_deviceAutoModeFlag)
            {
                MessageBox.Show("当前处于自动状态，无法进行操作！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!_axisServoOnOffFlag[axisIndex])
            {
                MessageBox.Show("轴不处于伺服使能状态，无法进行动作！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (_axisErrorNumber[axisIndex] != 0)
            {
                MessageBox.Show("当前轴处于报警状态，无法进行动作！请消除报警之后重试！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (axisIndex == 3 || axisIndex == 4 || axisIndex == 5)//如果为传送线宽度调节轴
            {
                _axisMotionControlBoardVariate.OutputCtrl(22, true);
                Thread.Sleep(10);
                while (!_ioStatus[1, 22])
                {
                    _axisMotionControlBoardVariate.OutputCtrl(22, true);
                    Thread.Sleep(10);
                    if (!_ioStatus[0, 7])//如果按下急停按钮
                    {
                        return;
                    }
                }
            }


            _axisMotionControlBoardVariate.AxisAction(901, axisIndex, _axisMoveStartVelocity_mmps[axisIndex], _axisJogInchVelocity_mmps[axisIndex],
            _axisMoveStopVelocity_mmps[axisIndex], 1, 1,
            _axisInchDistance_mm[axisIndex], moveDirection, inchFlag, 0, false, 0);
        }

        private void JogInchButtonMoveDownEvent(object sender, MouseEventArgs e)
        {
            bool axisPlusMoveFlag = false;//轴正向移动Flag，false-负向移动，true-正向移动
            if (((Button)sender).Name.ToString() == "btnAxisMinusMove")
                axisPlusMoveFlag = false;
            else
                axisPlusMoveFlag = true;

            AxisJogInchMoveFunction(_currentAxisIndex, axisPlusMoveFlag, _inchMoveFlag[_currentAxisIndex]);
        }

        private void JogInchButtonMoveLeaveEvent(object sender, EventArgs e)
        {
            if (!_axisParFileInitialFinishFlag)//如果轴初始化未完成
            {
                if (MessageBox.Show("轴初始化未完成，初始化完成后，再次进行运动控制。是否进行初始化！", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    AxisParFileInitial();
                    return;
                }
                else { return; }
            }

            if (!_axisMotionControlBoardVariate._initialFlag)
            {
                //if (MessageBox.Show("运动控制卡初始化失败，无法对轴进行控制！请检查运动控制卡！是否再次进行初始化", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                //{
                //    _axisMotionControlBoardVariate.Initial();
                //    return;
                //}
                // MessageBox.Show("Test");
                return;
            }

            //停顿0.1s，用于确保数据已经传递进运动控制卡线程中，以及axisCommandSaved已经获取到真实状态
            if (_axisCommand[_currentAxisIndex] != _axisCommandSaved[_currentAxisIndex])
                Thread.Sleep(100);

            if (!((_axisCommandSaved[_currentAxisIndex] == 901 && !_inchMoveFlag[_currentAxisIndex]) ||
             _axisCommand[_currentAxisIndex] == 0 && _axisCommandSaved[_currentAxisIndex] == 0))//轴处于运动状态中
            {
                //MessageBox.Show("当前轴繁忙，无法进行别的操作！请运行结束后重试！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (_deviceAutoModeFlag)
            {
                //MessageBox.Show("当前处于自动状态，无法进行操作！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            _axisMotionControlBoardVariate.jogAxisImmediateStop(_currentAxisIndex);

            if (_currentAxisIndex == 3 || _currentAxisIndex == 4 || _currentAxisIndex == 5)//如果为传送线宽度调节轴
            {
                _axisMotionControlBoardVariate.OutputCtrl(22, false);
                Thread.Sleep(10);
                while (_ioStatus[1, 22])
                {
                    _axisMotionControlBoardVariate.OutputCtrl(22, false);
                    Thread.Sleep(10);
                    if (!_ioStatus[0, 7])//如果按下急停按钮
                    {
                        return;
                    }
                }
            }
        }

        private void JogInchButtonMoveUpEvent(object sender, MouseEventArgs e)
        {
            if (!_axisParFileInitialFinishFlag)//如果轴初始化未完成
            {
                if (MessageBox.Show("轴初始化未完成，初始化完成后，再次进行运动控制。是否进行初始化！", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    AxisParFileInitial();
                    return;
                }
                else { return; }
            }

            if (!_axisMotionControlBoardVariate._initialFlag)
            {
                if (MessageBox.Show("运动控制卡初始化失败，无法对轴进行控制！请检查运动控制卡！是否再次进行初始化", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    _axisMotionControlBoardVariate.Initial();
                    return;
                }
            }
            //停顿0.1s，用于确保数据已经传递进运动控制卡线程中，以及axisCommandSaved已经获取到真实状态
            if (_axisCommand[_currentAxisIndex] != _axisCommandSaved[_currentAxisIndex])
                Thread.Sleep(100);

            if (!((_axisCommandSaved[_currentAxisIndex] == 901 && !_inchMoveFlag[_currentAxisIndex]) ||
               _axisCommand[_currentAxisIndex] == 0 && _axisCommandSaved[_currentAxisIndex] == 0))//轴处于运动状态中
            {
                //MessageBox.Show("当前轴繁忙，无法进行别的操作！请运行结束后重试！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //MessageBox.Show("Test");
                return;
            }

            if (_deviceAutoModeFlag)
            {
                //MessageBox.Show("当前处于自动状态，无法进行操作！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            _axisMotionControlBoardVariate.jogAxisImmediateStop(_currentAxisIndex);

            if (_currentAxisIndex == 3 || _currentAxisIndex == 4 || _currentAxisIndex == 5)//如果为传送线宽度调节轴
            {
                _axisMotionControlBoardVariate.OutputCtrl(22, false);
                Thread.Sleep(10);
                while (_ioStatus[1, 22])
                {
                    Thread.Sleep(10);
                    if (!_ioStatus[0, 7])//如果按下急停按钮
                    {
                        return;
                    }
                }
            }

        }

        #endregion

        #region 点位坐标及速度参数等的修改
        //点击保存当前坐标或编辑坐标数据按钮事件
        private void AxisSaveCurrentPositionOrModifiedPositionEvent(object sender, EventArgs e)
        {
            if (_deviceAutoModeFlag)//如果轴处于自动模式
            {
                MessageBox.Show("当前设备处于自动模式，无法保存轴点位参数！");
                return;
            }

            if (_axisHomedFlag[_currentAxisIndex])//如果完成原点复归
            {
                if (((Button)sender).Name == "btnAxisSaveCurrentPosition")
                {
                    if (_useRelativeFlag)//如果使用相对距离
                        txtAxisModifyPosition.Text = txtAxisCurrentPosition.Text;
                    else
                        txtAxisModifyPosition.Text = (Convert.ToSingle(txtAxisCurrentPosition.Text) - _absoluteCoor[_currentAxisIndex]).ToString("f2");
                }

                float minData = 0f;
                float maxData = 0f;

                AxisParFileInitial();

                minData = _axisSoftLimitMinus[_currentAxisIndex];
                maxData = _axisSoftLimitPlus[_currentAxisIndex];

                if (MyTool.DataProcessing.TextBoxDataCheck2(txtAxisModifyPosition.Text, minData, maxData, true, true, true, true))
                {
                    if (((Button)sender).Name == "btnAxisSaveCurrentPosition")
                    {
                        if (MessageBox.Show("确认要保存当前位置?" + "\n" + "轴:" + (_currentAxisIndex + 1).ToString("00") + "\n" + "点位号:" + txtAxisSelectedPointNumber.Text + "\n" +
                            "名称:" + txtAxisSelectedPointName.Text,
                            "保存当前位置", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.OK)
                            SavePointPosition();
                        else
                            return;
                    }
                    else
                    {
                        if (MessageBox.Show("确认要保存编辑位置?" + "\n" + "轴:" + (_currentAxisIndex + 1).ToString("00") + "\n" + "点位号:" + txtAxisSelectedPointNumber.Text + "\n" +
                            "名称:" + txtAxisSelectedPointName.Text,
                            "保存编辑位置", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.OK)
                            SavePointPosition();
                        else
                            return;
                    }
                    AxisPosFileInitial();

                    if (_axisPositionDataViewFormVariate != null && _axisPositionDataViewFormVariate.Visible)
                    {
                        _axisPositionDataViewFormVariate.PositionDataView(_currentProductName, _currentAxisIndex);
                    }
                    //MaLi 20220310 Change 随着这里的设定变化，Union Form重新读取数据
                    if (_axisUnionFormVariate != null && _axisUnionFormVariate.Visible)
                    {
                        _axisUnionFormVariate.AxisPosFileInitial(_currentProductName);
                    }
                    MessageBox.Show("点位位置数据保存完成！");
                }
                else
                {
                    MessageBox.Show("点位数据错误,不能保存!");
                    return;
                }
            }
            else
            {
                MessageBox.Show("请先回原点");
                return;
            }
        }

        //保存点位数据
        private void SavePointPosition()
        {
            AxisPosFileInitial();
            DataTable dt = new DataTable();
            string axisPositionFileSaveFolderPath = Directory.GetCurrentDirectory() + @"\" +
                MyTool.FolderAndFileManageClass.FilterPathIllegalChar("AxisParameterAndPositionData_" + _currentProductName) + @"\";//过滤掉作为Directory不合法的字符
            string axisPositionFilePath = Path.Combine(axisPositionFileSaveFolderPath,
                     MyTool.FolderAndFileManageClass.FilterPathIllegalChar("axis" + _currentAxisIndex.ToString("00") + "Pos" + "_" + _currentProductName)
                     + ".csv");

            _axisPointDataDT[_currentAxisIndex].Rows[Convert.ToInt32(txtAxisSelectedPointNumber.Text) - 1][1] = txtAxisModifyPosition.Text;

            try
            {
                CSVFileOperation.SaveCSV(_axisPointDataDT[_currentAxisIndex], axisPositionFilePath);
            }
            catch
            {
                MessageBox.Show("保存轴" + (_currentAxisIndex + 1).ToString("00") + "点位数据失败");
            }
        }

        //保存速度及其余参数按钮事件
        private void SaveSpeedAndOthersEvent(object sender, EventArgs e)
        {
            if (_deviceAutoModeFlag)//如果轴处于自动模式
            {
                MessageBox.Show("当前设备处于自动模式，无法保存轴点位参数！");
                return;
            }

            float minData = 0f;
            float maxData = 0f;

            AxisParFileInitial();
            minData = _axisMoveMinSpeed_mmps[_currentAxisIndex];
            maxData = _axisMoveMaxSpeed_mmps[_currentAxisIndex];

            if (!MyTool.DataProcessing.TextBoxDataCheck2(txtAxisModifyVelocity.Text, minData, maxData, true, true, true, true))
            {
                MessageBox.Show("速度数据错误,不能保存!");
                return;
            }

            minData = _axisMoveMinAccTime_s[_currentAxisIndex];
            maxData = _axisMoveMaxAccTime_s[_currentAxisIndex];
            if (!MyTool.DataProcessing.TextBoxDataCheck2(txtAxisModifyAcc.Text, minData, maxData, true, true, true, true))
            {
                MessageBox.Show("其它数据错误,不能保存!");
                return;
            }
            if (MessageBox.Show("确认要更改速度及其它?" + "\n" + "轴:" + (_currentAxisIndex + 1).ToString("00") + "\n" + "点位号:" + txtAxisSelectedPointNumber.Text + "\n" +
                "名称:" + txtAxisSelectedPointName.Text, "更改速度及其它", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.OK)
                SavePointSpeedAndOthers();
            else
                return;
            AxisPosFileInitial();
            if (_axisPositionDataViewFormVariate != null && _axisPositionDataViewFormVariate.Visible)
            {
                _axisPositionDataViewFormVariate.PositionDataView(_currentProductName, _currentAxisIndex);
            }
            MessageBox.Show("点位速度及其他数据保存完成！");
        }

        //保存速度及其余点位参数数据
        private void SavePointSpeedAndOthers()
        {
            AxisPosFileInitial();
            DataTable dt = new DataTable();
            string axisPositionFileSaveFolderPath = Directory.GetCurrentDirectory() + @"\" +
                MyTool.FolderAndFileManageClass.FilterPathIllegalChar("AxisParameterAndPositionData_" + _currentProductName) + @"\";//过滤掉作为Directory不合法的字符
            string axisPositionFilePath = Path.Combine(axisPositionFileSaveFolderPath,
                     MyTool.FolderAndFileManageClass.FilterPathIllegalChar("axis" + _currentAxisIndex.ToString("00") + "Pos" + "_" + _currentProductName)
                     + ".csv");

            _axisPointDataDT[_currentAxisIndex].Rows[Convert.ToInt32(txtAxisSelectedPointNumber.Text) - 1][2] = txtAxisModifyVelocity.Text;
            _axisPointDataDT[_currentAxisIndex].Rows[Convert.ToInt32(txtAxisSelectedPointNumber.Text) - 1][3] = txtAxisModifyAcc.Text;

            try
            {
                CSVFileOperation.SaveCSV(_axisPointDataDT[_currentAxisIndex], axisPositionFilePath);
            }
            catch
            {
                MessageBox.Show("保存轴" + (_currentAxisIndex + 1).ToString("00") + "速度及其它数据失败");
            }
        }

        #endregion

        #region 软键盘
        //双击按钮，打开数字软键盘，数据为正，浮点数
        private void TextBoxDoubleClickEventToOpenNumberKeyboard(object sender, EventArgs e)
        {
            SoftNumberKeyboard.LanguageFlag = 1;//设置语言为中文
            SoftNumberKeyboard numberKeyboard = new SoftNumberKeyboard();
            numberKeyboard.StartPosition = FormStartPosition.CenterScreen;
            numberKeyboard.PLUSMINUSBTHIDEFLAG = true;
            numberKeyboard.FULLSTOPBTHIDEFLAG = false;
            numberKeyboard.SOURCESTR = ((TextBox)sender).Text;
            numberKeyboard.SENDER = sender;
            numberKeyboard.SendInputDataIncludeObj += new SENDINPUTDATAINCLUDEOBJ(GetKeyboardInputData);//添加事件
            numberKeyboard.ShowDialog();
        }

        //键盘按下OK之后获取数据
        private void GetKeyboardInputData(object Sender, string GetDataStr)
        {

            ((TextBox)Sender).Text = GetDataStr;//将键盘按钮输入的数据传递给密码输入框
            ((TextBox)Sender).Focus();
            ((TextBox)Sender).SelectionLength = 0;
        }

        #endregion

        #region 数据检查
        //Textbox数据普通检查函数，正数，浮点数检查
        private void TextBoxDataPlusFloatCheckEvent(object sender, EventArgs e)
        {
            if (((TextBox)sender).Name == "txtAxisJogInchVelocity")
            {
                ((TextBox)sender).Text = MyTool.DataProcessing.TextBoxDataCheck(((TextBox)sender).Text, _axisJogMinSpeed_mmps[_currentAxisIndex], _axisJogMaxSpeed_mmps[_currentAxisIndex],
                    true, true, true, false);
            }
            else if (((TextBox)sender).Name == "txtAxisInchDistance")
            {
                ((TextBox)sender).Text = MyTool.DataProcessing.TextBoxDataCheck(((TextBox)sender).Text, _axisInchMinDistance_mm[_currentAxisIndex], _axisInchMaxDistance_mm[_currentAxisIndex],
                    true, true, true, false);
            }
            else if (((TextBox)sender).Name == "txtAxisModifyPosition")
            {
                ((TextBox)sender).Text = MyTool.DataProcessing.TextBoxDataCheck(((TextBox)sender).Text, _axisSoftLimitMinus[_currentAxisIndex], _axisSoftLimitPlus[_currentAxisIndex],
                    true, true, true, false);
            }
            else if (((TextBox)sender).Name == "txtAxisModifyVelocity")
            {
                ((TextBox)sender).Text = MyTool.DataProcessing.TextBoxDataCheck(((TextBox)sender).Text, _axisMoveMinSpeed_mmps[_currentAxisIndex], _axisMoveMaxSpeed_mmps[_currentAxisIndex],
                    true, true, true, false);
            }
            else if (((TextBox)sender).Name == "txtAxisModifyAcc")
            {
                ((TextBox)sender).Text = MyTool.DataProcessing.TextBoxDataCheck(((TextBox)sender).Text, _axisMoveMinAccTime_s[_currentAxisIndex], _axisMoveMaxAccTime_s[_currentAxisIndex],
                    true, true, true, false);
            }


            ((TextBox)sender).SelectionStart = ((TextBox)sender).TextLength;
            ((TextBox)sender).SelectionLength = 0;

        }

        //Textbox数据最后一次检查函数，正数，浮点数，整数检查
        private void TextBoxDataPlusFloatCheckLeaveEvent(object sender, EventArgs e)
        {
            if (((TextBox)sender).Name == "txtAxisJogInchVelocity")
            {
                ((TextBox)sender).Text = MyTool.DataProcessing.TextBoxDataCheck(((TextBox)sender).Text, _axisJogMinSpeed_mmps[_currentAxisIndex], _axisJogMaxSpeed_mmps[_currentAxisIndex],
                    true, true, true, true);
                _axisJogInchVelocity_mmps[_currentAxisIndex] = Convert.ToSingle(((TextBox)sender).Text);
            }
            else if (((TextBox)sender).Name == "txtAxisInchDistance")
            {
                ((TextBox)sender).Text = MyTool.DataProcessing.TextBoxDataCheck(((TextBox)sender).Text, _axisInchMinDistance_mm[_currentAxisIndex], _axisInchMaxDistance_mm[_currentAxisIndex],
                    true, true, true, true);
                _axisInchDistance_mm[_currentAxisIndex] = Convert.ToSingle(((TextBox)sender).Text);
            }
            else if (((TextBox)sender).Name == "txtAxisModifyPosition")
            {
                ((TextBox)sender).Text = MyTool.DataProcessing.TextBoxDataCheck(((TextBox)sender).Text, _axisSoftLimitMinus[_currentAxisIndex], _axisSoftLimitPlus[_currentAxisIndex],
                    true, true, true, true);
            }
            else if (((TextBox)sender).Name == "txtAxisModifyVelocity")
            {
                ((TextBox)sender).Text = MyTool.DataProcessing.TextBoxDataCheck(((TextBox)sender).Text, _axisMoveMinSpeed_mmps[_currentAxisIndex], _axisMoveMaxSpeed_mmps[_currentAxisIndex],
                    true, true, true, true);
            }
            else if (((TextBox)sender).Name == "txtAxisModifyAcc")
            {
                ((TextBox)sender).Text = MyTool.DataProcessing.TextBoxDataCheck(((TextBox)sender).Text, _axisMoveMinAccTime_s[_currentAxisIndex], _axisMoveMaxAccTime_s[_currentAxisIndex],
                    true, true, true, true);
            }

            //MaLi 20220310 Change 当主页面的手动控制数据发生变化，对应的union form的数据也进行更新
            if (_axisUnionFormVariate != null && _axisUnionFormVariate.Visible)
            {
                for (int i = 0; i < _axisQuantity; i++)
                {
                    _axisUnionFormVariate._axisJogInchVelocity_mmps[i] = _axisJogInchVelocity_mmps[i];
                    _axisUnionFormVariate._axisInchDistance_mm[i] = _axisInchDistance_mm[i];
                }
                _axisUnionFormVariate.RefreshManualCtrlData();
            }

            ((TextBox)sender).SelectionStart = ((TextBox)sender).TextLength;
            ((TextBox)sender).SelectionLength = 0;

        }
        #endregion

        #region MouseHover事件
        ToolTip _toolTip = new ToolTip();
        private void CheckDoneCodeMouseHover(object sender, EventArgs e)
        {
            //  设置伴随的对象.
            _toolTip.SetToolTip((TextBox)sender,
                "0:正在运行" + "\r" +
                "1:脉冲输出完毕停止"
                );//设置提示按钮和提示内容
        }

        private void SpeedGetDisplay(object sender, EventArgs e)
        {
            //  设置伴随的对象.
            _toolTip.SetToolTip((TextBox)sender, "实时速度");//设置提示按钮和提示内容
        }

        private void TaskDisplayMouseHover(object sender, EventArgs e)
        {
            //  设置伴随的对象.
            _toolTip.SetToolTip((TextBox)sender,
                "1-900:指定点位号运行" + "\r" +
                "901:点动或者寸动" + "\r" +
                "999:回原点" + "\r" +
                "1000:指定位置运行"
                );//设置提示按钮和提示内容
        }

        private void CommandDisplayMouseHover(object sender, EventArgs e)
        {
            //  设置伴随的对象.
            _toolTip.SetToolTip((TextBox)sender,
                "1-900:指定点位号运行" + "\r" +
                "901:点动或者寸动" + "\r" +
                "999:回原点" + "\r" +
                "1000:指定位置运行"
                );//设置提示按钮和提示内容
        }

        private void PositionModifyMouseHover(object sender, EventArgs e)
        {
            //  设置伴随的对象.
            string s = "[" + _axisSoftLimitMinus[_currentAxisIndex].ToString() + "," + _axisSoftLimitPlus[_currentAxisIndex].ToString() + "]";
            _toolTip.SetToolTip((TextBox)sender, s);//设置提示按钮和提示内容
        }

        private void SpeedModifyMouseHover(object sender, EventArgs e)
        {
            //  设置伴随的对象.
            string s = "[" + _axisMoveMinSpeed_mmps[_currentAxisIndex].ToString() + "," + _axisMoveMaxSpeed_mmps[_currentAxisIndex].ToString() + "]";
            _toolTip.SetToolTip((TextBox)sender, s);//设置提示按钮和提示内容
        }

        private void ACCTimeModifyMouseHover(object sender, EventArgs e)
        {
            //  设置伴随的对象.
            string s = "[" + _axisMoveMinAccTime_s[_currentAxisIndex].ToString() + "," + _axisMoveMaxAccTime_s[_currentAxisIndex].ToString() + "]";
            _toolTip.SetToolTip((TextBox)sender, s);//设置提示按钮和提示内容
        }

        private void JogInchSpeedMouseHover(object sender, EventArgs e)
        {
            //  设置伴随的对象.
            string s = "[" + _axisJogMinSpeed_mmps[_currentAxisIndex].ToString() + "," + _axisJogMaxSpeed_mmps[_currentAxisIndex].ToString() + "]";
            _toolTip.SetToolTip((TextBox)sender, s);//设置提示按钮和提示内容
        }

        private void InchLengthMouseHover(object sender, EventArgs e)
        {
            //  设置伴随的对象.
            string s = "[" + _axisInchMinDistance_mm[_currentAxisIndex].ToString() + "," + _axisInchMaxDistance_mm[_currentAxisIndex].ToString() + "]";
            _toolTip.SetToolTip((TextBox)sender, s);//设置提示按钮和提示内容
        }

        #endregion

        #region 轴联合调试画面相关函数
        private void btnAxisUnion_Click(object sender, EventArgs e)
        {
            if (_axisUnionFormVariate == null)
            {
                _axisUnionFormVariate = new AxisUnionForm(_inchMoveFlag, _axisJogInchVelocity_mmps, _axisInchDistance_mm, _axisPointQuantity, _currentProductName);
                _axisUnionFormVariate._motionCardInitialFlag = _axisMotionControlBoardVariate._initialFlag;
                for (int i = 0; i < 4; i++)
                {
                    _axisUnionFormVariate._axisCurrentPosition_mm[i] = _axisCurrentPosition_mm[i];//当前位置_mm
                    _axisUnionFormVariate._axisUnitArray[i] = _axisUnitArray[i];
                    //软负极限
                    _axisUnionFormVariate._axisSoftLimitMinus[i] = _axisSoftLimitMinus[i];
                    //软正极限
                    _axisUnionFormVariate._axisSoftLimitPlus[i] = _axisSoftLimitPlus[i];

                    //JogMinSpeed
                    _axisUnionFormVariate._axisJogMinSpeed_mmps[i] = _axisJogMinSpeed_mmps[i];
                    //JogMaxSpeed
                    _axisUnionFormVariate._axisJogMaxSpeed_mmps[i] = _axisJogMaxSpeed_mmps[i];
                    //InchMinDistance
                    _axisUnionFormVariate._axisInchMinDistance_mm[i] = _axisInchMinDistance_mm[i];
                    //InchMaxDistance
                    _axisUnionFormVariate._axisInchMaxDistance_mm[i] = _axisInchMaxDistance_mm[i];

                    //MoveMinSpeed
                    _axisUnionFormVariate._axisMoveMinSpeed_mmps[i] = _axisMoveMinSpeed_mmps[i];
                    //MoveMaxSpeed
                    _axisUnionFormVariate._axisMoveMaxSpeed_mmps[i] = _axisMoveMaxSpeed_mmps[i];
                    //MoveminACCTime
                    _axisUnionFormVariate._axisMoveMinAccTime_s[i] = _axisMoveMinAccTime_s[i];
                    //MoveMaxACCTime
                    _axisUnionFormVariate._axisMoveMaxAccTime_s[i] = _axisMoveMaxAccTime_s[i];
                    //MoveminDecTime
                    _axisUnionFormVariate._axisMoveMinDecTime_s[i] = _axisMoveMinDecTime_s[i];
                    //MoveMaxDecTime
                    _axisUnionFormVariate._axisMoveMaxDecTime_s[i] = _axisMoveMaxDecTime_s[i];
                    //AxisMoveStartVelocity
                    _axisUnionFormVariate._axisMoveStartVelocity_mmps[i] = _axisMoveStartVelocity_mmps[i];
                    //AxisMoveStopVelocity
                    _axisUnionFormVariate._axisMoveStopVelocity_mmps[i] = _axisMoveStopVelocity_mmps[i];

                    //轴原点复归标志
                    _axisUnionFormVariate._axisHomedFlag[i] = _axisHomedFlag[i];
                }
                //设备模式
                _axisUnionFormVariate._deviceAutoModeFlag = _deviceAutoModeFlag;

                _axisUnionFormVariate.Show();
                _axisUnionFormVariate.ControlHideFunc(_axisUseFlag, _axisNameStrArray);
                _axisUnionFormVariate.FormCloseRequestEvent += new AxisUnionFormCloseRequestDelegate(AxisUnionFormCloseEventFunction);
                _axisUnionFormVariate.ManualDataChangedEvent += new AxisUnionManualDataChangeDelegate(AxisUnionFormManualDataChangeEventFunction);
                _axisUnionFormVariate.AxisPositionDataModifiedEvent += new AxisUnionFormAxisPositionDataModifiedDelegate(AxisUnionFormAxisPositionDataModifiedEventFunction);
                _axisUnionFormVariate.AxisUnionMoveEvent += new AxisUnionFormAxisUnionMoveDelegate(AxisUnionFormAxiUnionMoveEventFunction);
                _axisUnionFormVariate.JogInchMoveEvent += new AxisUnionFormJogInchMoveDelegate(AxisUnionFormJogInchMoveEventFunction);
            }
            else
            {
                _axisUnionFormVariate.Hide();
                Thread.Sleep(100);
                _axisUnionFormVariate.Dispose();
                _axisUnionFormVariate = null;
            }

        }

        //轴联合画面请求关闭窗口对应的事件函数
        private void AxisUnionFormCloseEventFunction()
        {
            if (_axisUnionFormVariate != null)
            {
                _axisUnionFormVariate.Hide();
                Thread.Sleep(100);
                _axisUnionFormVariate.Dispose();
                _axisUnionFormVariate = null;
            }
        }

        //轴联合画面Inch标志发生变化，触发的事件
        private void AxisUnionFormManualDataChangeEventFunction()
        {
            for (int i = 0; i < 4; i++)
            {
                _inchMoveFlag[i] = _axisUnionFormVariate._axisInchFlag[i];
                _axisJogInchVelocity_mmps[i] = _axisUnionFormVariate._axisJogInchVelocity_mmps[i];
                _axisInchDistance_mm[i] = _axisUnionFormVariate._axisInchDistance_mm[i];
            }

            txtAxisJogInchVelocity.Text = _axisJogInchVelocity_mmps[_currentAxisIndex].ToString();
            txtAxisInchDistance.Text = _axisInchDistance_mm[_currentAxisIndex].ToString();
        }

        //轴联合画面点位数据发生变化触发的事件
        private void AxisUnionFormAxisPositionDataModifiedEventFunction()
        {
            AxisPosFileInitial();
            _oldSelectPointNumber = 0;
        }

        //轴联合画面轴联合动作事件
        private void AxisUnionFormAxiUnionMoveEventFunction(bool[] actionAxisFlag, int targetPointNumber)
        {
            for (int i = 0; i < 4; i++)
            {
                if (actionAxisFlag[i])
                {
                    _axisActionCommand[i] = targetPointNumber;
                }
            }
        }

        //轴联合画面JOG INCH移动事件
        private void AxisUnionFormJogInchMoveEventFunction(int axisIndex, bool moveDirection, bool stopFlag)
        {
            if (!stopFlag)
                AxisJogInchMoveFunction(axisIndex, moveDirection, _inchMoveFlag[axisIndex]);
            else
            {
                if (!_axisParFileInitialFinishFlag)//如果轴初始化未完成
                {
                    if (MessageBox.Show("轴初始化未完成，初始化完成后，再次进行运动控制。是否进行初始化！", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                    {
                        AxisParFileInitial();
                        return;
                    }
                    else { return; }
                }

                if (!_axisMotionControlBoardVariate._initialFlag)
                {
                    // if (MessageBox.Show("运动控制卡初始化失败，无法对轴进行控制！请检查运动控制卡！是否再次进行初始化", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                    //{
                    //    _axisMotionControlBoardVariate.Initial();
                    //    return;
                    //}
                    return;
                }

                //停顿0.1s，用于确保数据已经传递进运动控制卡线程中，以及axisCommandSaved已经获取到真实状态
                Thread.Sleep(100);
                if (!(_axisCommand[axisIndex] == 901 && _axisCommandSaved[axisIndex] == 901 && !_inchMoveFlag[axisIndex]))//轴处于运动状态中
                {
                    //MessageBox.Show("当前轴繁忙，无法进行别的操作！请运行结束后重试！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (_deviceAutoModeFlag)
                {
                    MessageBox.Show("当前处于自动状态，无法进行操作！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                _axisMotionControlBoardVariate.jogAxisImmediateStop(axisIndex);
            }
        }

        #endregion

        #region IO页面相关函数

        private void btnIODisplay_Click(object sender, EventArgs e)
        {
            if (_motionCardIOFormVaraite == null)
            {
                _motionCardIOFormVaraite = new MotionCardIOForm();
                _motionCardIOFormVaraite.Show();
                _motionCardIOFormVaraite.IOFormCloseRequestEvent += new IOFormCloseRequestDelegate(IOFormCloseRequestEventFunction);
            }
            else
            {
                _motionCardIOFormVaraite.Hide();
                Thread.Sleep(100);
                _motionCardIOFormVaraite.Dispose();
                _motionCardIOFormVaraite = null;
            }
        }

        private void IOFormCloseRequestEventFunction()
        {
            _motionCardIOFormVaraite.Hide();
            Thread.Sleep(100);
            _motionCardIOFormVaraite.Dispose();
            _motionCardIOFormVaraite = null;
        }

        #endregion

        #region IO控制相关函数
        //控制IO输出
        /// <summary>
        /// OutputControl：控制输出
        /// </summary>
        /// <param name="outputNumber">int:输出IO号码,从0开始</param>
        /// <param name="onOrOff">bool:控制信号ON或者OFF，true-控制输出为ON，false-控制输出为OFF</param>
        /// <returns>bool:返回执行结果</returns>
        public bool OutputControl(int outputNumber, bool onOrOff)
        {
            if (_axisMotionControlBoardVariate == null)
            {
                return false;
            }

            if (!_axisMotionControlBoardVariate._initialFlag)
            {
                return false;
            }

            if (_axisMotionControlBoardVariate.OutputCtrl(outputNumber, onOrOff) == -1) //如果输出指令失败
            {
                return false;
            }

            return true;
        }
        #endregion

        #region 编码器读取函数
        /// <summary>
        /// GetEncoder：读取光栅尺的数据，作为Y1轴编码器
        /// </summary>
        /// <returns></returns>
        //public int GetEncoder()
        //{
        //    if (_axisMotionControlBoardVariate != null)
        //    {
        //      return  _axisMotionControlBoardVariate.GetEncoderCount();
        //    }
        //    return 0;
        //}
        #endregion

        #region 设置轴记录的脉冲位置函数
        /// <summary>
        /// GetEncoder：读取光栅尺的数据，作为Y1轴编码器
        /// </summary>
        /// <returns></returns>
        //public void SetPositionOfAxis(int axisIndex, int newPosition)
        //{
        //    _axisMotionControlBoardVariate.SetPositionOfAxis(axisIndex, newPosition);
        //}   
        #endregion

        #region 窗口关闭事件
        private void AxisControlMainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            if (AxisControlMainFormRequestCloseEvent != null)
            {
                AxisControlMainFormRequestCloseEvent();
            }
            else
            {
                e.Cancel = false;
            }
        }
        #endregion

        #region 界面显示事件
        private void AxisControlMainForm_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                AxisParFileInitial();
            }
            else
            {
                if (_axisPositionDataViewFormVariate != null && _axisPositionDataViewFormVariate.Visible)
                {
                    _axisPositionDataViewFormVariate.Hide();//确保多线程中不会误判
                    Thread.Sleep(100);
                    _axisPositionDataViewFormVariate.Dispose();
                    _axisPositionDataViewFormVariate = null;
                }

                if (_axisUnionFormVariate != null && _axisUnionFormVariate.Visible)
                {
                    _axisUnionFormVariate.Hide();
                    Thread.Sleep(100);
                    _axisUnionFormVariate.Dispose();
                    _axisUnionFormVariate = null;
                }

                if (_motionCardIOFormVaraite != null && _motionCardIOFormVaraite.Visible)
                {
                    _motionCardIOFormVaraite.Hide();
                    Thread.Sleep(100);
                    _motionCardIOFormVaraite.Dispose();
                    _motionCardIOFormVaraite = null;
                }

            }
        }
        #endregion

        //当窗口再是活动界面事件
        private void AxisControlMainForm_Deactivate(object sender, EventArgs e)
        {
            //如果运动控制卡实例化并且初始化成功，如果界面action切换，如果有步进等存在，那么停止运动
            if (_axisMotionControlBoardVariate != null && _axisMotionControlBoardVariate._initialFlag)
            {
                if (!_deviceAutoModeFlag)
                {
                    //停顿0.1s，用于确保数据已经传递进运动控制卡线程中，以及axisCommandSaved已经获取到真实状态
                    Thread.Sleep(100);
                    if (!(_axisCommand[_currentAxisIndex] == 901 && _axisCommandSaved[_currentAxisIndex] == 901 && !_inchMoveFlag[_currentAxisIndex]))//轴处于运动状态中
                    {
                        //MessageBox.Show("当前轴繁忙，无法进行别的操作！请运行结束后重试！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    _axisMotionControlBoardVariate.jogAxisImmediateStop(_currentAxisIndex);

                    if (_currentAxisIndex == 3 || _currentAxisIndex == 4 || _currentAxisIndex == 5)//如果为传送线宽度调节轴
                    {
                        _axisMotionControlBoardVariate.OutputCtrl(22, false);
                        Thread.Sleep(10);
                        while (_ioStatus[1, 22])
                        {
                            Thread.Sleep(10);
                            if (!_ioStatus[0, 7])//如果按下急停按钮
                            {
                                return;
                            }
                        }
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (_axisMotionControlBoardVariate._axis00HomedFlag && _axisMotionControlBoardVariate._axis01HomedFlag)
            {
                //飞拍位置数组
                int[] ss = new int[82] { (int)(10/0.0005), (int)(10 / 0.0005), (int)(20 / 0.0005), (int)(20 / 0.0005), (int)(30 / 0.0005), (int)(30 / 0.0005),
                    (int)(40/0.0005), (int)(40/0.0005), (int)(50/0.0005), (int)(50/0.0005),
                    (int)(60/0.0005), (int)(60/0.0005), (int)(70/0.0005), (int)(70/0.0005),
                    (int)(80/0.0005), (int)(80/0.0005), (int)(90/0.0005), (int)(90/0.0005), (int)(100/0.0005), (int)(100/0.0005)
                , (int)(110/0.0005), (int)(110/0.0005), (int)(120/0.0005), (int)(120/0.0005), (int)(130/0.0005), (int)(130/0.0005)
                , (int)(140/0.0005), (int)(140/0.0005), (int)(150/0.0005), (int)(150/0.0005), (int)(160/0.0005), (int)(160/0.0005)
                , (int)(170/0.0005), (int)(170/0.0005), (int)(180/0.0005), (int)(180/0.0005), (int)(190/0.0005), (int)(190/0.0005)
                , (int)(200/0.0005), (int)(200/0.0005), (int)(210/0.0005), (int)(210/0.0005), (int)(220/0.0005), (int)(220/0.0005)
                , (int)(230/0.0005), (int)(230/0.0005), (int)(240/0.0005), (int)(240/0.0005), (int)(250/0.0005), (int)(250/0.0005)
                , (int)(260/0.0005), (int)(260/0.0005), (int)(270/0.0005), (int)(270/0.0005), (int)(280/0.0005), (int)(280/0.0005)
                , (int)(290/0.0005), (int)(290/0.0005), (int)(300/0.0005), (int)(300/0.0005), (int)(300/0.0005), (int)(300/0.0005)
                , (int)(310/0.0005), (int)(310/0.0005), (int)(320/0.0005), (int)(320/0.0005), (int)(330/0.0005), (int)(330/0.0005)
                , (int)(340/0.0005), (int)(340/0.0005), (int)(350/0.0005), (int)(350/0.0005), (int)(360/0.0005), (int)(360/0.0005)
                , (int)(370/0.0005), (int)(370/0.0005), (int)(380/0.0005), (int)(380/0.0005), (int)(390/0.0005), (int)(390/0.0005)
                , (int)(400/0.0005), (int)(400/0.0005)};
                while (true)
                {
                    //X1,Y1回到0
                    _axisMotionControlBoardVariate.AxisAction(1000, 0, 10, 1000, 10, 1, 1, 0, false, false, 0, false, 0);
                    _axisMotionControlBoardVariate.AxisAction(1000, 1, 10, 1000, 10, 1, 1, 0, false, false, 0, false, 0);
                    while (_axisMotionControlBoardVariate._axis00CurrentPositionNumber != 1000 || _axisMotionControlBoardVariate._axis01CurrentPositionNumber != 1000)
                    {
                        Application.DoEvents();
                    }

                    //启动飞拍
                    _axisMotionControlBoardVariate.Compare2Dimens(0, 0, 1, 5, 5, ss);
                    //X1,Y1 PTP移动到100
                    //_axisMotionControlBoardVariate.AxisAction(1000, 0, 10, 10, 10, 1, 1, 100, false, false, 0, false, 0);
                    //_axisMotionControlBoardVariate.AxisAction(1000, 1, 10, 10, 10, 1, 1, 100, false, false, 0, false, 0);
                    //X1,Y1 直线插补移动到400
                    _axisMotionControlBoardVariate.axisActionLineInterpolation(1000, 2, new ushort[] { 0, 1 }, new float[] { 410, 410 }, 10, 280, 1, new float[] { 0, 0 });
                    while (_axisMotionControlBoardVariate._axis00CurrentPositionNumber != 1000 || _axisMotionControlBoardVariate._axis01CurrentPositionNumber != 1000)
                    {
                        Application.DoEvents();
                    }
                }
            }
            else
            {
                MessageBox.Show("X1,Y1需要回原点");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            _axisMotionControlBoardVariate.OutputCtrl(68, !_axisMotionControlBoardVariate.getOutPut(68));
        }

        private void button3_Click(object sender, EventArgs e)
        {
            _axisMotionControlBoardVariate.feiPaiCameraTrigger(0, 30000);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (_axisMotionControlBoardVariate._axis00HomedFlag && _axisMotionControlBoardVariate._axis01HomedFlag)
            {
                //_axisMotionControlBoardVariate.lineInterpolation(0, 3, new int[] { 20000, 20000 }, 10, 50, 2, 0);
                _axisMotionControlBoardVariate.axisActionLineInterpolation(1000, 2, new ushort[] { 0, 1 }, new float[] { 150, 150 }, 10, 100, 1, new float[] { 0, 0 });
            }
            else
            {
                MessageBox.Show("X1,Y1需要回原点");
            }
        }
    }
}

