using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using csLTDMC;//MaLi 20220301 Change(增加using，用于LeiSaiMotionCardDMC3400ASerial类)

namespace MotionCard
{
    public class LeiSaiMotionCardDMC3400ASerial//MaLi 20220301 Change(增加雷赛运动控制卡D3400A系列的轴控制类)
    {
        //指定轴的个数
        const int AXIS_QUANTITY = 4;

        //*************************外部可设定参数*******************************//
        //记录当前设备处于自动状态Flag，false-手动状态，true-自动状态
        public bool _deviceAutoModeFlag = false;

        //毫米/脉冲
        public float[] _axismmPP = new float[AXIS_QUANTITY] { 0.005f, 0.005f, 0.01f, 0.01f };

        //极限
        //软负极限，外部设定
        public float[] _axisSoftLimitMinus = new float[AXIS_QUANTITY];
        //软正极限
        public float[] _axisSoftLimitPlus = new float[AXIS_QUANTITY];

        //速度百分比(%)
        public float[] _axisManualSpeedPCT = new float[AXIS_QUANTITY];
        public float[] _axisAutoSpeedPCT = new float[AXIS_QUANTITY];

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
        //MinDeviation
        public float[] _axisMinDeviation_mm = new float[AXIS_QUANTITY];
        //MaxDeviation
        public float[] _axisMaxDeviation_mm = new float[AXIS_QUANTITY];

        //轴原点复归起始速度
        public float[] _axisHomeMoveStartVelocity_mmps = new float[AXIS_QUANTITY];
        //轴原点复归最大速度
        public float[] _axisHomeMoveMaxVelocity_mmps = new float[AXIS_QUANTITY];
        //轴原点复归停止速度
        public float[] _axisHomeMoveStopVelocity_mmps = new float[AXIS_QUANTITY];
        //轴原点复归加速时间
        public float[] _axisHomeMoveAccTime_s = new float[AXIS_QUANTITY];
        //轴原点复归减速时间
        public float[] _axisHomeMoveDecTime_s = new float[AXIS_QUANTITY];

        //*************************外部可读取参数*******************************//

        //运动卡初始化标志
        public bool _initialFlag = false;

        //当前位置_mm
        public double[] _axisCurrentPosition_mm = new double[AXIS_QUANTITY];
        //当前轴速度_mmps
        public double[] _axisCurrentSpeed_mmps = new double[AXIS_QUANTITY];

        //0:NA;1-900:点位号定位;901:Jogging;999:Homing;1000:实时位置数据定位;1001:减速停止
        public int[] _axisCommand = new int[AXIS_QUANTITY];
        //轴Command保存
        public int[] _axisCommandSaved = new int[AXIS_QUANTITY];
        //0:NA;1-900:当前点位号;999:Homed;1000:实时位置数据定位完成
        public int[] _axisCurrentPositionNumber = new int[AXIS_QUANTITY];

        //原点标志
        public bool[] _axisHomedFlag = new bool[AXIS_QUANTITY];
        //错误代码
        public int[] _axisErrorNumber = new int[AXIS_QUANTITY];
        //轴Servo On Flag
        public bool[] _axisServoOnOffFlag = new bool[AXIS_QUANTITY];
        //轴运行状态
        public int[] _axisCheckDoneStatus = new int[AXIS_QUANTITY];
        //轴原点，正负极限状态
        public bool[] _axisELMinus = new bool[AXIS_QUANTITY];
        public bool[] _axisELPlus = new bool[AXIS_QUANTITY];
        public bool[] _axisORG = new bool[AXIS_QUANTITY];
        //轴回原点开始Flag
        public bool[] _axisHomeStartFlag = new bool[AXIS_QUANTITY];
        //轴运动开始Flag
        public bool[] _axisMoveStartFlag = new bool[AXIS_QUANTITY];

        //运动控制卡的输入点数
        public int _motionCardInputQuantity = 64;
        //运动控制卡的输出点数
        public int _motionCardOutputQuantity = 32;
        //IO相关
        public bool[] _inputStatus;
        public bool[] _outputStatus;

        //*************************私有变量，外部不可读取修改*******************************//

        //当前位置_脉冲数
        private int[] _axisCurrentPosition_pulse = new int[AXIS_QUANTITY];
        //当前轴速度_脉冲数
        private double[] _axisCurrentSpeed_pps = new double[AXIS_QUANTITY];

        //目标位置_脉冲数
        int[] _axisTargetPosition_pulse = new int[AXIS_QUANTITY];
        //当前运行Deviation
        private float[] _axisCurrentDeviationValue_pulse = new float[AXIS_QUANTITY];
        //目标位置的Deviation
        float[] _axisTargetDeviationValue_mm = new float[AXIS_QUANTITY];

        private Thread _axisStatusUpdataThread = null;//轴状态更新线程

        //  
        //构造函数
        public LeiSaiMotionCardDMC3400ASerial()
        {
            VariateDataInitial();
            Initial();
        }

        #region VariateDataInitial:变量参数初始化
        /// <summary>
        /// VariateDataInitial：变量参数的初始化
        /// </summary>
        void VariateDataInitial()
        {
            for (int i = 0; i < AXIS_QUANTITY; i++)
            {
                //*************************外部可设定参数*******************************//
                //极限
                //软负极限
                _axisSoftLimitMinus[i] = 0f;
                //软正极限
                _axisSoftLimitPlus[i] = 0f;

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
                //*************************外部可读取参数*******************************//
                //当前位置_脉冲数
                _axisCurrentPosition_mm[i] = 0;
                //当前轴速度_脉冲数
                _axisCurrentSpeed_mmps[i] = 0;

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

                //当前位置_脉冲数
                _axisCurrentPosition_pulse[i] = 0;
                //当前轴速度_脉冲数
                _axisCurrentSpeed_pps[i] = 0;
                //目标位置_脉冲数
                _axisTargetPosition_pulse[i] = 0;
                //当前运行Deviation
                _axisCurrentDeviationValue_pulse[i] = 0f;
                //目标位置的deviation
                _axisTargetDeviationValue_mm[i] = 0f;
            }
        }
        #endregion

        #region Initial:卡及轴初始化
        /// <summary>
        /// Initial:卡及轴初始化
        /// </summary>
        public int Initial()
        {
            int initialResult = LTDMC.dmc_board_init();
            if (initialResult == 1)
            {
                //设置编码器（光栅尺）读取相关参数
                LTDMC.dmc_set_counter_inmode(0, 1, 1);//设置0号卡，01轴（Y轴），单倍A/B向

                //运动控制相关初始化
                LTDMC.dmc_set_pulse_outmode(0, 0, 5);//设置0号卡，00轴脉冲模式为双脉冲，低电平模式
                LTDMC.dmc_set_pulse_outmode(0, 1, 5);//设置0号卡，01轴脉冲模式为双脉冲，低电平模式
                LTDMC.dmc_set_pulse_outmode(0, 2, 5);//设置0号卡，02轴脉冲模式为双脉冲，低电平模式
                LTDMC.dmc_set_pulse_outmode(0, 3, 5);//设置0号卡，03轴脉冲模式为双脉冲，低电平模式

                //极限传感器相关设定
                LTDMC.dmc_set_el_mode(0, 0, 1, 1, 0);//设置0号卡，00轴极限传感器为正负极限都有效，高电平有效，立即停止
                LTDMC.dmc_set_el_mode(0, 1, 1, 1, 0);//设置0号卡，01轴极限传感器为正负极限都有效，高电平有效，立即停止
                LTDMC.dmc_set_el_mode(0, 2, 3, 1, 0);//设置0号卡，02轴极限传感器为正极限有效，高电平有效，立即停止
                LTDMC.dmc_set_el_mode(0, 3, 1, 1, 0);//设置0号卡，03轴极限传感器为正负极限都有效，高电平有效，立即停止

                //原点复归相关初始化
                LTDMC.dmc_set_home_pin_logic(0, 0, 0, 0);//设置0号卡，00轴原点信号为低电平有效
                LTDMC.dmc_set_home_pin_logic(0, 1, 0, 0);//设置0号卡，01轴原点信号为低电平有效
                LTDMC.dmc_set_home_pin_logic(0, 2, 0, 0);//设置0号卡，02轴原点信号为低电平有效
                LTDMC.dmc_set_home_pin_logic(0, 3, 0, 0);//设置0号卡，03轴原点信号为低电平有效

                LTDMC.dmc_set_homemode(0, 0, 0, 1, 2, 0);//设置0号卡，00轴原点回归，方向为负向，高速回零（以set_profile设定的最大速度回零），模式为两次原点回零模式
                LTDMC.dmc_set_homemode(0, 1, 0, 1, 2, 0);//设置0号卡，01轴原点回归，方向为负向，高速回零（以set_profile设定的最大速度回零），模式为两次原点回零模式
                LTDMC.dmc_set_homemode(0, 2, 0, 1, 2, 0);//设置0号卡，02轴原点回归，方向为负向，高速回零（以set_profile设定的最大速度回零），模式为两次原点回零模式
                LTDMC.dmc_set_homemode(0, 3, 0, 1, 2, 0);//设置0号卡，03轴原点回归，方向为负向，高速回零（以set_profile设定的最大速度回零），模式为两次原点回零模式

                LTDMC.dmc_set_home_el_return(0, 0, 0);//设置0号卡，00轴原点回归遇限位不反找
                LTDMC.dmc_set_home_el_return(0, 1, 0);//设置0号卡，01轴原点回归遇限位不反找
                LTDMC.dmc_set_home_el_return(0, 2, 0);//设置0号卡，02轴原点回归遇限位不反找
                LTDMC.dmc_set_home_el_return(0, 3, 0);//设置0号卡，03轴原点回归遇限位不反找

                LTDMC.dmc_set_home_position(0, 0, 1, 0);//设置0号卡，00轴原点回归后清零，再无偏移
                LTDMC.dmc_set_home_position(0, 1, 1, 0);//设置0号卡，01轴原点回归后清零，再无偏移
                LTDMC.dmc_set_home_position(0, 2, 1, 0);//设置0号卡，02轴原点回归后清零，再无偏移
                LTDMC.dmc_set_home_position(0, 3, 1, 0);//设置0号卡，03轴原点回归后清零，再无偏移

                AxisStop(0, 0, 1);//设置0号卡，00轴立即停止
                AxisStop(0, 1, 1);//设置0号卡，01轴立即停止
                AxisStop(0, 2, 1);//设置0号卡，02轴立即停止
                AxisStop(0, 3, 1);//设置0号卡，03轴立即停止

                //SERVO ON
                OutputCtrl(0, 16, true);//轴00伺服使能
                OutputCtrl(0, 17, true);//轴01伺服使能
                OutputCtrl(0, 18, true);//轴02伺服使能
                OutputCtrl(0, 19, true);//轴03伺服使能

                //轴状态更新线程
                _axisStatusUpdataThread = new Thread(AxisStatusUpdataThreadFunc);
                _axisStatusUpdataThread.IsBackground = true;
                _axisStatusUpdataThread.Start();

                //初始化完成，初始化标志置为true
                _initialFlag = true;

                //存储输入输出状态数组初始化
                _inputStatus = new bool[_motionCardInputQuantity];
                _outputStatus = new bool[_motionCardOutputQuantity];
            }
            return initialResult;
        }
        #endregion

        #region AxisMoveTimeOut_MT:轴运动超时
        /// <summary>
        /// AxisMoveTimeOut_MT:轴运动超时
        /// </summary>
        private void AxisMoveTimeOut_MT(object sender, long JumpPeriod, long interval)
        {

        }
        #endregion

        #region AxisStatusUpdataThreadFunc:轴状态更新线程
        /// <summary>
        /// AxisStatusUpdataThreadFunc:轴状态更新线程方法
        /// </summary>
        private void AxisStatusUpdataThreadFunc()
        {
            //报警复位信号自动OFF计时
            int axis00ResetOutputOnCount = 0;
            int axis01ResetOutputOnCount = 0;
            int axis02ResetOutputOnCount = 0;
            int axis03ResetOutputOnCount = 0;

            float axisCurrentPosition_mm = 0f;//轴当前坐标_mm
            float axisCurrentDeviation_mm = 0f;//当前deviation_mm

            while (true)
            {
                for (ushort i = 0; i < AXIS_QUANTITY; i++)
                {
                    //轴当前位置
                    _axisCurrentPosition_pulse[i] = LTDMC.dmc_get_position((ushort)0, i);
                    _axisCurrentPosition_mm[i] = (float)_axisCurrentPosition_pulse[i] * _axismmPP[i];
                    //轴当前速度
                    _axisCurrentSpeed_pps[i] = LTDMC.dmc_read_current_speed(0, i);
                    _axisCurrentSpeed_mmps[i] = _axisCurrentSpeed_pps[i] * _axismmPP[i];

                    //轴当前定位偏差值
                    _axisCurrentDeviationValue_pulse[i] = _axisCurrentPosition_pulse[i] - _axisTargetPosition_pulse[i];

                    //轴当前状态
                    _axisCheckDoneStatus[i] = LTDMC.dmc_check_done(0, i);//获取00轴的运动状态，0-正在运行，1-轴已经停止，适用于单轴以及PVT运动

                    //999:Homing，结束判定
                    if (_axisCommand[i] == 999 && _axisCheckDoneStatus[i] == 1 &&
                        _axisHomeStartFlag[i] && _axisCurrentPosition_pulse[i] == 0)
                    {
                        _axisHomeStartFlag[i] = false;
                        _axisCommand[i] = 0;
                        _axisCommandSaved[i] = 0;
                        _axisHomedFlag[i] = true;
                        if (i == 1)
                            SetEncoderCount(0);
                    }

                    //1-900:点位号定位 或者 1000:实时位置数据定位
                    axisCurrentPosition_mm = Convert.ToSingle(_axisCurrentPosition_pulse[i]) * _axismmPP[i];
                    axisCurrentDeviation_mm = _axisCurrentDeviationValue_pulse[i] * _axismmPP[i];
                    if (_axisHomedFlag[i] && _axisMoveStartFlag[i] && ((_axisCommand[i] >= 1 && _axisCommand[i] <= 900) || _axisCommand[i] == 1000) &&
                        (LTDMC.dmc_check_done(0, i) == 1 ||
                        (LTDMC.dmc_check_done(0, i) == 0 && axisCurrentDeviation_mm > _axisTargetDeviationValue_mm[i] * -1 && axisCurrentDeviation_mm <= _axisTargetDeviationValue_mm[i])))
                    {
                        _axisCurrentPositionNumber[i] = _axisCommand[i];
                        _axisCommand[i] = 0;
                        _axisCommandSaved[i] = 0;
                        _axisMoveStartFlag[i] = false;
                    }

                    //Inch
                    if (_axisCommandSaved[i] == 901 && LTDMC.dmc_check_done(0, i) == 1 && _axisMoveStartFlag[i])
                    {
                        _axisCommand[i] = 0;
                        _axisCommandSaved[i] = 0;
                        _axisMoveStartFlag[i] = false;
                    }
                }

                //读取IO状态
                //输入
                for (int i = 0; i < _motionCardInputQuantity; i++)
                {
                    _inputStatus[i] = GetInput(0, i);
                }

                //输出
                for (int i = 0; i < _motionCardOutputQuantity; i++)
                {
                    _outputStatus[i] = GetOutput(0, i);
                }

                for (int i = 0; i < AXIS_QUANTITY; i++)
                {
                    //原点以及极限传感器信号更新
                    _axisELMinus[i] = _inputStatus[24 + i];
                    _axisELPlus[i] = _inputStatus[16 + i];
                    _axisORG[i] = _inputStatus[32 + i];

                    //增加在非回原点时,因为其它情况导致的停止(例如极限传感器),对轴进行停止，并对axisCommand以及axisSavedCommand清零
                    ushort elEnableFlag = 0;//临时变量，用于判定EL的类型
                    ushort elLogicFlag = 0;//临时变量，用于判定EL使能电位
                    ushort elModeFLag = 0;//临时变量，用于判定EL停止模式
                    bool enableELPlusCoverFlag = false;//临时变量，用于判别正极限启用，并且被遮挡与否
                    bool enableELMinusCoverFlag = false;//临时变量，用于判别正极限启用，并且被遮挡与否

                    //elEnableFlag-0:正负限位禁止，1:正负限位允许，2:正限位禁止，负限位允许，3:正限位允许，负限位禁止
                    //elLogicFlag-0：正负限位低电平有效，1：正负限位高电平有效，2:正限位低有效，负限位高有效，3:正限位高有效，负限位低有效
                    //elModeFLag-0:正负限位立即停止，1:正负限位减速停止，2:正限位立即停止，负限位减速停止，3:正限位减速停止，负限位立即停止
                    LTDMC.dmc_get_el_mode(0, (ushort)i, ref elEnableFlag, ref elLogicFlag, ref elModeFLag);

                    if ((!_axisELPlus[i] && (elEnableFlag == 1 || elEnableFlag == 3) && (elLogicFlag == 1 || elLogicFlag == 3)) ||//如果当前为OFF，启用了正极限传感器，为高电平有效的话，那么表明被遮挡
                        (_axisELPlus[i] && (elEnableFlag == 1 || elEnableFlag == 3) && (elLogicFlag == 0 || elLogicFlag == 2)))//如果当前为ON，启用了正极限传感器，为低电平有效的话，那么表明被遮挡
                    {
                        enableELPlusCoverFlag = true;
                    }

                    if ((!_axisELMinus[i] && (elEnableFlag == 1 || elEnableFlag == 2) && (elLogicFlag == 1 || elLogicFlag == 2)) ||//如果当前为OFF，启用了负极限传感器，为高电平有效的话，那么表明被遮挡
                        (_axisELMinus[i] && (elEnableFlag == 1 || elEnableFlag == 2) && (elLogicFlag == 0 || elLogicFlag == 3)))//如果当前为ON，启用了负极限传感器，为低电平有效的话，那么表明被遮挡
                    {
                        enableELMinusCoverFlag = true;
                    }

                    if ((_axisCommandSaved[i] != 999 && (enableELPlusCoverFlag || enableELMinusCoverFlag)) ||
                        (_axisCommandSaved[i] == 999 && enableELPlusCoverFlag))//除了为回原点时，负极限传感器被遮挡不停止外，其余的只要正负极限传感器被遮挡都停止
                    {
                        JogAxisStop(0, i, 1);
                    }

                    //伺服ON OFF状态更新
                    _axisServoOnOffFlag[i] = _outputStatus[16 + i];

                    if (!_axisServoOnOffFlag[i] || !_inputStatus[40 + i])//当伺服OFF的时候，HOMED FLAG变为FALSE
                        _axisHomedFlag[i] = false;


                    //Axis00 Alarm
                    if (_inputStatus[40 + i])
                        _axisErrorNumber[i] = 0;
                    else
                    {
                        _axisErrorNumber[i] = 1;
                        AxisStop(0, i, 0);
                        if (_axisCheckDoneStatus[i] != 0)//等待轴停止之后对变量进行赋值
                        {
                            _axisHomedFlag[i] = false;
                            _axisCommand[i] = 0;
                            _axisCommandSaved[i] = 0;
                        }
                    }
                }

                //报警复位信号自动OFF
                //Axis 00
                if (_outputStatus[24])
                    axis00ResetOutputOnCount++;
                if (axis00ResetOutputOnCount > 100)
                {
                    OutputCtrl(0, 24, false);
                    axis00ResetOutputOnCount = 0;
                }
                //Axis 01
                if (_outputStatus[25])
                    axis01ResetOutputOnCount++;
                if (axis01ResetOutputOnCount > 100)
                {
                    OutputCtrl(0, 25, false);
                    axis01ResetOutputOnCount = 0;
                }
                //Axis 02
                if (_outputStatus[26])
                    axis02ResetOutputOnCount++;
                if (axis02ResetOutputOnCount > 100)
                {
                    OutputCtrl(0, 26, false);
                    axis02ResetOutputOnCount = 0;
                }
                //Axis 03
                if (_outputStatus[27])
                    axis03ResetOutputOnCount++;
                if (axis03ResetOutputOnCount > 100)
                {
                    OutputCtrl(0, 27, false);
                    axis03ResetOutputOnCount = 0;
                }

                Thread.Sleep(10);
            }
        }
        #endregion

        #region AxisAction:轴运行(axisCommand取值范围 0:NA;1-900:点位号定位;901:Jogging;999:Homing;1000:实时位置数据定位;1001:减速停止;1002:立即停止)
        /// <summary>
        /// AxisAction:轴运行
        /// </summary>
        /// <param name="axisCommand">int:运行指令，0:NA;1-900:点位号定位;901:Jogging;999:Homing;1000:实时位置数据定位;1001:减速停止;1002:立即停止</param>
        /// <param name="axisNumber">int:轴号0-N</param>
        /// <param name="startVelocity">float:运动初始速度,单位:mmps</param>
        /// <param name="maxVelocity">float:运动速度,负值表示往负方向找原点/Jog,正值表示往正方向找原点/Jog,单位:mmps</param>
        /// <param name="stopVelocity">float:停止速度，单位mmps</param>
        /// <param name="accTime">double:加速时间,单位:s</param>
        /// <param name="decTime">double:减速时间，单位：s</param>
        /// <param name="targetPosition">float:目标位置,单位:mm</param>
        /// <param name="jogDirection">bool:Jog方向,false/true(负向/正向)</param>
        /// <param name="inch">bool:点动/寸动,false/true(点动/寸动)</param>
        /// <param name="deviationValue">float:定位偏差值,单位:mm</param>
        /// <param name="useSModeMoveFlag">bool:是否使用S形速度曲线移动，false-不采用，true-采用</param>
        /// <param name="sMoveTime">double：当选择采用S形速度曲线时，S曲线的时间，单位s，在0-0.5之间</param>
        public void AxisAction(int axisCommand, int axisNumber, float startVelocity, float maxVelocity, float stopVelocity,
            double accTime, double decTime, float targetPosition, bool jogDirection, bool inch, float deviationValue, bool useSModeMoveFlag, double sMoveTime)
        {
            int axisMoveStartVelocity_pps, axisMoveMaxVelocity_pps, axisMoveStopVelocity_pps;
            double axisMoveSMoveTime = sMoveTime;
            float axisCurrentPosition_mm, axisTargetPosition_mm;//用于换算目标位置为mm单位，以及当前位置为mm单位
            double axisMoveAccTime, axisMoveDecTime;

            axisMoveStartVelocity_pps = Convert.ToInt32(startVelocity / _axismmPP[axisNumber]);//转换设定的开始速度单位为pulse/s
            axisMoveMaxVelocity_pps = Convert.ToInt32(maxVelocity / _axismmPP[axisNumber]);//转换设定的最高速度单位为pulse/s
            axisMoveStopVelocity_pps = Convert.ToInt32(stopVelocity / _axismmPP[axisNumber]);//转换设定的终止速度单位为pulse/s

            //关于加减速时间数据有效性检测
            axisMoveAccTime = accTime;//将加速时间赋值至变量中
            axisMoveDecTime = decTime;//将减速时间赋值至变量中
            if (axisMoveAccTime < _axisMoveMinAccTime_s[axisNumber])//如果加速时间小于设定的加速时间上限
                axisMoveAccTime = _axisMoveMinAccTime_s[axisNumber];
            else if (axisMoveAccTime > _axisMoveMaxAccTime_s[axisNumber])//如果加速时间大于设定的加速时间上限
                axisMoveAccTime = _axisMoveMaxAccTime_s[axisNumber];

            if (axisMoveDecTime < _axisMoveMinDecTime_s[axisNumber])//如果减速时间小于设定的加速时间上限
                axisMoveDecTime = _axisMoveMinDecTime_s[axisNumber];
            else if (axisMoveDecTime > _axisMoveMaxDecTime_s[axisNumber])//如果减速时间大于设定的加速时间上限
                axisMoveDecTime = _axisMoveMaxDecTime_s[axisNumber];

            axisTargetPosition_mm = targetPosition;//将目标位置赋值给轴目标位置变量

            //判定S形状曲线运动时间是否合理
            if (axisMoveSMoveTime < 0)
                axisMoveSMoveTime = 0;
            else if (axisMoveSMoveTime > 0.5)
                axisMoveSMoveTime = 0.5;

            //保证deviation数值为正
            if (deviationValue < _axisMinDeviation_mm[axisNumber])
                _axisTargetDeviationValue_mm[axisNumber] = _axisMinDeviation_mm[axisNumber];
            if (deviationValue > _axisMaxDeviation_mm[axisNumber])
                _axisTargetDeviationValue_mm[axisNumber] = _axisMaxDeviation_mm[axisNumber];

            if (deviationValue < 0)
                _axisTargetDeviationValue_mm[axisNumber] = deviationValue * -1f;
            else
                _axisTargetDeviationValue_mm[axisNumber] = deviationValue;


            _axisCommand[axisNumber] = axisCommand;
            if (_axisCommandSaved[axisNumber] != 999)//如果不为回原点，更新axis command saved，因为回原点过程中会控制轴至负向位
                _axisCommandSaved[axisNumber] = axisCommand;
            _axisCurrentPositionNumber[axisNumber] = 0;//重置axisCurrentPositionNumber

            //999:Home
            if (axisCommand == 999)
            {
                HomeAction(axisCommand, axisNumber, startVelocity, maxVelocity, stopVelocity, accTime, decTime);
            }
            //1-900:点位号定位;1000:实时位置数据定位
            if ((axisCommand >= 1 && axisCommand <= 900) || axisCommand == 1000)
            {

                if (_axisHomedFlag[axisNumber])//如果轴完成原点复归
                {
                    /*
                    if (axisTargetPosition_mm < _axisSoftLimitMinus[axisNumber])//判定目标位置是否小于负极限，如果小于负极限，那么目标位置修改为负极限
                        axisTargetPosition_mm = _axisSoftLimitMinus[axisNumber];
                    if (axisTargetPosition_mm > _axisSoftLimitPlus[axisNumber])//判定目标位置是否大于正极限，如果大于负极限，那么目标位置修改为正极限
                        axisTargetPosition_mm = _axisSoftLimitPlus[axisNumber];
                     */

                    _axisTargetPosition_pulse[axisNumber] = Convert.ToInt32(axisTargetPosition_mm / _axismmPP[axisNumber]);//换算目标位置为pulse单位
                    if (_axisTargetPosition_pulse[axisNumber] != _axisCurrentPosition_pulse[axisNumber])//如果当前位置不等于目标位置，那么启动运动
                    {
                        if (maxVelocity > _axisMoveMaxSpeed_mmps[axisNumber])
                        {
                            if (_deviceAutoModeFlag)//当设备处于自动模式时
                                axisMoveMaxVelocity_pps = Convert.ToInt32(_axisMoveMaxSpeed_mmps[axisNumber] * _axisAutoSpeedPCT[axisNumber] / 100f / _axismmPP[axisNumber]);//转换设定的最高速度单位为pulse/s
                            else//当设备处于手动模式时
                                axisMoveMaxVelocity_pps = Convert.ToInt32(_axisMoveMaxSpeed_mmps[axisNumber] * _axisManualSpeedPCT[axisNumber] / 100f / _axismmPP[axisNumber]);//转换设定的最高速度单位为pulse/s
                        }
                        else if (maxVelocity < _axisMoveMinSpeed_mmps[axisNumber])
                        {
                            if (_deviceAutoModeFlag)//当设备处于自动模式时
                                axisMoveMaxVelocity_pps = Convert.ToInt32(_axisMoveMinSpeed_mmps[axisNumber] * _axisAutoSpeedPCT[axisNumber] / 100f / _axismmPP[axisNumber]);//转换设定的最高速度单位为pulse/s
                            else//当设备处于手动模式时
                                axisMoveMaxVelocity_pps = Convert.ToInt32(_axisMoveMinSpeed_mmps[axisNumber] * _axisManualSpeedPCT[axisNumber] / 100f / _axismmPP[axisNumber]);//转换设定的最高速度单位为pulse/s
                        }
                        else
                        {
                            if (_deviceAutoModeFlag)//当设备处于自动模式时
                                axisMoveMaxVelocity_pps = Convert.ToInt32(maxVelocity * _axisAutoSpeedPCT[axisNumber] / 100f / _axismmPP[axisNumber]);//转换设定的最高速度单位为pulse/s
                            else//当设备处于手动模式时
                                axisMoveMaxVelocity_pps = Convert.ToInt32(maxVelocity * _axisManualSpeedPCT[axisNumber] / 100f / _axismmPP[axisNumber]);//转换设定的最高速度单位为pulse/s
                        }

                        if (LTDMC.dmc_check_done(0, (ushort)axisNumber) != 0)//如果轴处于静止状态
                        {
                            LTDMC.dmc_set_profile(0, (ushort)axisNumber, axisMoveStartVelocity_pps, axisMoveMaxVelocity_pps, axisMoveAccTime, axisMoveDecTime, axisMoveStopVelocity_pps);
                            if (useSModeMoveFlag)
                                LTDMC.dmc_set_s_profile(0, (ushort)axisNumber, 0, axisMoveSMoveTime);
                            else
                                LTDMC.dmc_set_s_profile(0, (ushort)axisNumber, 0, 0f);
                            LTDMC.dmc_pmove(0, (ushort)axisNumber, _axisTargetPosition_pulse[axisNumber], 1);
                        }
                        else
                        {
                            LTDMC.dmc_change_speed(0, (ushort)axisNumber, axisMoveMaxVelocity_pps, 0);//改变速度
                            LTDMC.dmc_update_target_position(0, (ushort)axisNumber, _axisTargetPosition_pulse[axisNumber], 0);//改变目标位置（轴处于停止状态或运动状态都OK）
                        }

                        while (LTDMC.dmc_check_done(0, (ushort)axisNumber) == 1)//等待获取到设备已经运动指令
                        {
                        }
                        _axisMoveStartFlag[axisNumber] = true;
                    }
                    else
                    {
                        _axisCurrentPositionNumber[axisNumber] = _axisCommand[axisNumber];
                        _axisCommand[axisNumber] = 0;
                        _axisCommandSaved[axisNumber] = 0;
                    }
                }
            }
            //901:Jog
            if (axisCommand == 901)
            {
                if (_axisHomedFlag[axisNumber])//已回原点
                {
                    if (!jogDirection)//负方向
                    {
                        if (!inch)//点动
                            _axisTargetPosition_pulse[axisNumber] = Convert.ToInt32(_axisSoftLimitMinus[axisNumber] / _axismmPP[axisNumber]);
                        else//寸动
                        {
                            axisCurrentPosition_mm = ((float)_axisCurrentPosition_pulse[axisNumber]) / _axismmPP[axisNumber];

                            if (axisTargetPosition_mm < _axisInchMinDistance_mm[axisNumber])//如果设定的寸动距离小于下限
                                axisTargetPosition_mm = _axisInchMinDistance_mm[axisNumber];
                            else if (axisTargetPosition_mm > _axisInchMaxDistance_mm[axisNumber])//如果设定的寸动距离大于上限
                                axisTargetPosition_mm = _axisInchMaxDistance_mm[axisNumber];

                            axisTargetPosition_mm = axisCurrentPosition_mm - axisTargetPosition_mm;
                            if (axisTargetPosition_mm < _axisSoftLimitMinus[axisNumber])
                                axisTargetPosition_mm = _axisSoftLimitMinus[axisNumber];
                            _axisTargetPosition_pulse[axisNumber] = Convert.ToInt32(axisTargetPosition_mm / _axismmPP[axisNumber]);
                        }
                    }
                    else//正方向
                    {
                        if (!inch)//点动
                            _axisTargetPosition_pulse[axisNumber] = Convert.ToInt32(_axisSoftLimitPlus[axisNumber] / _axismmPP[axisNumber]);
                        else//寸动
                        {
                            axisCurrentPosition_mm = ((float)_axisCurrentPosition_pulse[axisNumber]) / _axismmPP[axisNumber];

                            if (axisTargetPosition_mm < _axisInchMinDistance_mm[axisNumber])//如果设定的寸动距离小于下限
                                axisTargetPosition_mm = _axisInchMinDistance_mm[axisNumber];
                            else if (axisTargetPosition_mm > _axisInchMaxDistance_mm[axisNumber])//如果设定的寸动距离大于上限
                                axisTargetPosition_mm = _axisInchMaxDistance_mm[axisNumber];

                            axisTargetPosition_mm = axisCurrentPosition_mm + axisTargetPosition_mm;
                            if (axisTargetPosition_mm > _axisSoftLimitPlus[axisNumber])
                                axisTargetPosition_mm = _axisSoftLimitPlus[axisNumber];
                            _axisTargetPosition_pulse[axisNumber] = Convert.ToInt32(axisTargetPosition_mm / _axismmPP[axisNumber]);
                        }
                    }
                    if (_axisTargetPosition_pulse[axisNumber] != _axisCurrentPosition_pulse[axisNumber])
                    {
                        if (maxVelocity > _axisJogMaxSpeed_mmps[axisNumber])
                            axisMoveMaxVelocity_pps = Convert.ToInt32(_axisJogMaxSpeed_mmps[axisNumber] / _axismmPP[axisNumber]);//转换设定的最高速度单位为pulse/s
                        else if (maxVelocity < _axisJogMinSpeed_mmps[axisNumber])
                            axisMoveMaxVelocity_pps = Convert.ToInt32(_axisJogMinSpeed_mmps[axisNumber] / _axismmPP[axisNumber]);//转换设定的最高速度单位为pulse/s

                        LTDMC.dmc_set_profile(0, (ushort)axisNumber, axisMoveStartVelocity_pps, axisMoveMaxVelocity_pps, axisMoveAccTime, axisMoveDecTime, axisMoveStopVelocity_pps);
                        if (useSModeMoveFlag)
                            LTDMC.dmc_set_s_profile(0, (ushort)axisNumber, 0, axisMoveSMoveTime);
                        else
                            LTDMC.dmc_set_s_profile(0, (ushort)axisNumber, 0, 0f);
                        LTDMC.dmc_pmove(0, (ushort)axisNumber, _axisTargetPosition_pulse[axisNumber], 1);
                        while (LTDMC.dmc_check_done(0, (ushort)axisNumber) == 1)//等待获取到设备已经运动指令
                        {
                        }
                        _axisMoveStartFlag[axisNumber] = true;
                        _axisCurrentPositionNumber[axisNumber] = 0;
                    }
                    else
                    {
                        _axisCommand[axisNumber] = 0;
                        _axisCommandSaved[axisNumber] = 0;
                    }
                }
                else//没有回原点
                {
                    if (!jogDirection)//负方向
                    {
                        if (!inch)//点动
                        {
                            if (maxVelocity > _axisJogMaxSpeed_mmps[axisNumber])
                                axisMoveMaxVelocity_pps = Convert.ToInt32(_axisJogMaxSpeed_mmps[axisNumber] / _axismmPP[axisNumber]);//转换设定的最高速度单位为pulse/s
                            else if (maxVelocity < _axisJogMinSpeed_mmps[axisNumber])
                                axisMoveMaxVelocity_pps = Convert.ToInt32(_axisJogMinSpeed_mmps[axisNumber] / _axismmPP[axisNumber]);//转换设定的最高速度单位为pulse/s

                            LTDMC.dmc_set_profile(0, (ushort)axisNumber, axisMoveStartVelocity_pps, axisMoveMaxVelocity_pps, axisMoveAccTime, axisMoveDecTime, axisMoveStopVelocity_pps);
                            if (useSModeMoveFlag)
                                LTDMC.dmc_set_s_profile(0, (ushort)axisNumber, 0, axisMoveSMoveTime);
                            else
                                LTDMC.dmc_set_s_profile(0, (ushort)axisNumber, 0, 0f);
                            LTDMC.dmc_vmove(0, (ushort)axisNumber, 0);//负方向连续运动
                            _axisMoveStartFlag[axisNumber] = true;
                            _axisCurrentPositionNumber[axisNumber] = 0;
                        }
                        else//寸动
                        {
                            if (axisTargetPosition_mm < _axisInchMinDistance_mm[axisNumber])//如果设定的寸动距离小于下限
                                axisTargetPosition_mm = _axisInchMinDistance_mm[axisNumber];
                            else if (axisTargetPosition_mm > _axisInchMaxDistance_mm[axisNumber])//如果设定的寸动距离大于上限
                                axisTargetPosition_mm = _axisInchMaxDistance_mm[axisNumber];
                            _axisTargetPosition_pulse[axisNumber] = Convert.ToInt32(axisTargetPosition_mm * -1 / _axismmPP[axisNumber]);

                            if (_axisTargetPosition_pulse[axisNumber] != _axisCurrentPosition_pulse[axisNumber])
                            {
                                LTDMC.dmc_set_profile(0, (ushort)axisNumber, axisMoveStartVelocity_pps, axisMoveMaxVelocity_pps, axisMoveAccTime, axisMoveDecTime, axisMoveStopVelocity_pps);
                                if (useSModeMoveFlag)
                                    LTDMC.dmc_set_s_profile(0, (ushort)axisNumber, 0, axisMoveSMoveTime);
                                else
                                    LTDMC.dmc_set_s_profile(0, (ushort)axisNumber, 0, 0f);
                                LTDMC.dmc_pmove(0, (ushort)axisNumber, _axisTargetPosition_pulse[axisNumber], 0);//相对移动
                                while (LTDMC.dmc_check_done(0, (ushort)axisNumber) == 1)//等待获取到设备已经运动指令
                                {
                                }
                                _axisMoveStartFlag[axisNumber] = true;
                                _axisCurrentPositionNumber[axisNumber] = 0;
                            }
                            else
                            {
                                _axisCommand[axisNumber] = 0;
                                _axisCommandSaved[axisNumber] = 0;
                            }
                        }
                    }
                    else//正方向
                    {
                        if (!inch)//点动
                        {
                            if (maxVelocity > _axisJogMaxSpeed_mmps[axisNumber])
                                axisMoveMaxVelocity_pps = Convert.ToInt32(_axisJogMaxSpeed_mmps[axisNumber] / _axismmPP[axisNumber]);//转换设定的最高速度单位为pulse/s
                            else if (maxVelocity < _axisJogMinSpeed_mmps[axisNumber])
                                axisMoveMaxVelocity_pps = Convert.ToInt32(_axisJogMinSpeed_mmps[axisNumber] / _axismmPP[axisNumber]);//转换设定的最高速度单位为pulse/s

                            LTDMC.dmc_set_profile(0, (ushort)axisNumber, axisMoveStartVelocity_pps, axisMoveMaxVelocity_pps, axisMoveAccTime, axisMoveDecTime, axisMoveStopVelocity_pps);
                            if (useSModeMoveFlag)
                                LTDMC.dmc_set_s_profile(0, (ushort)axisNumber, 0, axisMoveSMoveTime);
                            else
                                LTDMC.dmc_set_s_profile(0, (ushort)axisNumber, 0, 0f);
                            LTDMC.dmc_vmove(0, (ushort)axisNumber, 1);//正方向连续运动
                            _axisMoveStartFlag[axisNumber] = true;
                            _axisCurrentPositionNumber[axisNumber] = 0;
                        }
                        else//寸动
                        {
                            if (axisTargetPosition_mm < _axisInchMinDistance_mm[axisNumber])//如果设定的寸动距离小于下限
                                axisTargetPosition_mm = _axisInchMinDistance_mm[axisNumber];
                            else if (axisTargetPosition_mm > _axisInchMaxDistance_mm[axisNumber])//如果设定的寸动距离大于上限
                                axisTargetPosition_mm = _axisInchMaxDistance_mm[axisNumber];
                            _axisTargetPosition_pulse[axisNumber] = Convert.ToInt32(axisTargetPosition_mm * 1 / _axismmPP[axisNumber]);

                            if (_axisTargetPosition_pulse[axisNumber] != _axisCurrentPosition_pulse[axisNumber])
                            {
                                LTDMC.dmc_set_profile(0, (ushort)axisNumber, axisMoveStartVelocity_pps, axisMoveMaxVelocity_pps, axisMoveAccTime, axisMoveDecTime, axisMoveStopVelocity_pps);
                                if (useSModeMoveFlag)
                                    LTDMC.dmc_set_s_profile(0, (ushort)axisNumber, 0, axisMoveSMoveTime);
                                else
                                    LTDMC.dmc_set_s_profile(0, (ushort)axisNumber, 0, 0f);
                                LTDMC.dmc_pmove(0, (ushort)axisNumber, _axisTargetPosition_pulse[axisNumber], 0);//相对移动
                                while (LTDMC.dmc_check_done(0, (ushort)axisNumber) == 1)//等待获取到设备已经运动指令
                                {
                                }
                                _axisMoveStartFlag[axisNumber] = true;
                                _axisCurrentPositionNumber[axisNumber] = 0;
                            }
                            else
                            {
                                _axisCommand[axisNumber] = 0;
                                _axisCommandSaved[axisNumber] = 0;
                            }
                        }
                    }

                }
            }

            //1001:减速停止
            if (axisCommand == 1001)
            {
                JogAxisStop(0, axisNumber, 0);
            }

            //1002:立即停止
            if (axisCommand == 1002)
            {
                JogAxisStop(0, axisNumber, 1);
            }
        }
        #endregion

        #region AxisActionLineInterpolation:轴直线插补运行(axisCommand取值范围 0:NA;1-900:点位号定位;1000:实时位置数据定位)
        /// <summary>
        /// AxisActionLineInterpolation:轴直线插补运行
        /// </summary>
        /// <param name="axisCommand">
        /// <param name="axisCount">int:轴个数2-4</param>
        /// <param name="axisNumberArray">int[]:轴号0-N</param>
        /// <param name="targetPositionArray">float[]:对应轴号列表各轴的绝对坐标的位置列表</param>
        /// <param name="maxVelocity">float:运动速度(换算到第一个轴号),单位:mmps</param>
        /// <param name="accDecTime">double:加减速时间(矢量速度的加速时间),单位:s</param>
        /// <param name="deviationValueArray">float[]:定位偏差值,单位:mm</param>
        /// <returns>-1:直线插补轴的个数有误，-2：指定的插补轴个数与设定的插补轴个数不匹配，-3：指定的插补轴轴号有误，-4：指令代码有误,-5:当前有轴处于繁忙中, 0:成功运行</returns>
        public int AxisActionLineInterpolation(int axisCommand, int axisCount, UInt16[] axisNumberArray, float[] targetPositionArray, float maxVelocity, double accDecTime, float[] deviationValueArray)
        {
            int axisMoveMaxVelocity_pps;
            int[] targetPositionArray_pulse = new int[axisCount];
            bool needRunFlag = false;//判定是否有必要运动标志

            //参数检测
            if (axisCount < 2 || axisCount > 4)
                return -1;
            else if (axisNumberArray.Length != axisCount)
                return -2;

            for (int i = 0; i < axisCount; i++)
            {
                if (axisNumberArray[i] < 0 || axisNumberArray[i] > 3)
                    return -3;
                if (_axisCheckDoneStatus[axisNumberArray[i]] == 0)
                    return -5;
            }

            if (!(axisCommand != 1000 || (axisCommand >= 1 && axisCommand <= 900)))
                return -4;


            //对速度参数赋值
            if (_deviceAutoModeFlag)
                axisMoveMaxVelocity_pps = Convert.ToInt32(maxVelocity * _axisAutoSpeedPCT[axisNumberArray[0]] / 100f / _axismmPP[axisNumberArray[0]]);
            else
                axisMoveMaxVelocity_pps = Convert.ToInt32(maxVelocity * _axisManualSpeedPCT[axisNumberArray[0]] / 100f / _axismmPP[axisNumberArray[0]]);

            //变量赋值
            for (int i = 0; i < axisCount; i++)
            {
                _axisCommand[axisNumberArray[i]] = axisCommand;
                _axisCommandSaved[axisNumberArray[i]] = axisCommand;
                _axisCurrentPositionNumber[axisNumberArray[i]] = 0;
                _axisTargetDeviationValue_mm[axisNumberArray[i]] = deviationValueArray[i];
                _axisTargetPosition_pulse[axisNumberArray[i]] = Convert.ToInt32(targetPositionArray[i] / _axismmPP[axisNumberArray[i]]);
                targetPositionArray_pulse[i] = Convert.ToInt32(targetPositionArray[i] / _axismmPP[axisNumberArray[i]]);
                if (_axisTargetPosition_pulse[axisNumberArray[i]] != _axisCurrentPosition_pulse[axisNumberArray[i]])
                    needRunFlag = true;
            }

            if (needRunFlag)//如果判定为需要移动
            {
                LTDMC.dmc_set_vector_profile_multicoor(0, 0, 0, axisMoveMaxVelocity_pps, accDecTime, 0, 0);
                LTDMC.dmc_line_multicoor(0, 0, (ushort)axisCount, (ushort[])axisNumberArray, targetPositionArray_pulse, 1);//绝对运动

                for (int i = 0; i < axisNumberArray.Length; i++)
                {
                    _axisMoveStartFlag[axisNumberArray[i]] = true;
                }
            }

            return 0;
        }
        #endregion

        #region JogAxisStop
        /// <summary>
        /// JogAxisStop:JOG轴停止
        /// </summary>
        /// <param name="cardNumber">int:运动控制卡号</param>
        /// <param name="axisNumber">int:轴号</param>
        /// <param name="axisStopMode">int:轴停止模式，0-减速停止，1-紧急停止</param>
        public async void JogAxisStop(int cardNumber, int axisNumber, int stopMode)
        {
            await Task.Run(() =>
            {
                AxisStop(cardNumber, axisNumber, stopMode);
                while (_axisCheckDoneStatus[axisNumber] == 0)
                {
                }
                _axisCommand[axisNumber] = 0;
                _axisCommandSaved[axisNumber] = 0;
            });
        }
        #endregion

        #region AxisStop:轴停止
        /// <summary>
        /// AxisStop:轴停止
        /// </summary>
        /// <param name="cardNumber">int:运动控制卡号</param>
        /// <param name="axisNumber">int:轴号</param>
        /// <param name="axisStopMode">int:轴停止模式，0-减速停止，1-紧急停止</param>
        public int AxisStop(int cardNumber, int axisNumber, int axisStopMode)
        {
            return LTDMC.dmc_stop((ushort)cardNumber, (ushort)axisNumber, (ushort)axisStopMode);//控制轴停止
        }
        #endregion

        #region HomeAction:回原点
        /// <summary>
        /// HomeAction:回原点
        /// </summary>
        /// <param name="axisCommand">int:运行指令，0:NA;1-900:点位号定位;901:Jogging;999:Homing;1000:实时位置数据定位;1001:减速停止;1002:立即停止</param>
        /// <param name="axisNumber">int:轴号0-N</param>
        /// <param name="startVelocity">float:运动初始速度,单位:mmps</param>
        /// <param name="maxVelocity">float:运动速度,负值表示往负方向找原点/Jog,正值表示往正方向找原点/Jog,单位:mmps</param>
        /// <param name="stopVelocity">float:停止速度，单位mmps</param>
        /// <param name="accTime">double:加速时间,单位:s</param>
        /// <param name="decTime">double:减速时间，单位：s</param>
        private async void HomeAction(int axisCommand, int axisNumber, float startVelocity, float maxVelocity, float stopVelocity, double accTime, double decTime)
        {
            float axisHomeMoveStartVelocity_pps, axisHomeMoveMaxVelocity_pps, axisHomeMoveStopVelocity_pps;
            axisHomeMoveStartVelocity_pps = Convert.ToInt32(_axisHomeMoveStartVelocity_mmps[axisNumber] / _axismmPP[axisNumber]);
            axisHomeMoveMaxVelocity_pps = Convert.ToInt32(_axisHomeMoveMaxVelocity_mmps[axisNumber] / _axismmPP[axisNumber]);
            axisHomeMoveStopVelocity_pps = Convert.ToInt32(_axisHomeMoveStopVelocity_mmps[axisNumber] / _axismmPP[axisNumber]);

            await Task.Run(() =>
            {
                //向负方向Jog移动,寻找原点传感器或者负极限传感器
                AxisAction(901, axisNumber, startVelocity, maxVelocity, stopVelocity, accTime, decTime, 0, false, false, 0, false, 0);
                //等待找到原点传感器或者负极限传感器
                ushort elEnableFlag = 0;//临时变量，用于判定EL的类型
                ushort elLogicFlag = 0;//临时变量，用于判定EL使能电位
                ushort elModeFLag = 0;//临时变量，用于判定EL停止模式
                LTDMC.dmc_get_el_mode(0, (ushort)axisNumber, ref elEnableFlag, ref elLogicFlag, ref elModeFLag);
                while (((_axisELMinus[axisNumber] && (elEnableFlag == 1 || elEnableFlag == 2) && (elLogicFlag == 0 || elLogicFlag == 3))//如果启用了负限位并且负限位为低电平有效，当负限位传感器为ON
                    || (!_axisELMinus[axisNumber] && (elEnableFlag == 1 || elEnableFlag == 2) && (elLogicFlag == 1 || elLogicFlag == 2)))//如果启用了负限位并且负限位为高电平有效，当负限位传感器为OFF
                    && !_axisORG[axisNumber])//当负极限传感器为ON以及原点传感器为OFF（表明没有遮挡传感器）时，等待
                {
                    Thread.Sleep(10);
                }
                AxisStop(0, axisNumber, 0);//找到原点传感器或者负极限传感器,停止移动
                while (LTDMC.dmc_check_done(0, (ushort)axisNumber) == 0)//等待停止完成
                {
                    Thread.Sleep(10);
                }

                //增加以下延时,确保最后停止位置是否处于负极限传感器处
                int i = 0;
                while (i < 30)
                {
                    i++;
                    Thread.Sleep(10);
                }

                //如果找到的是负极限传感器
                if ((!_axisELMinus[axisNumber] && (elEnableFlag == 1 || elEnableFlag == 2) && (elLogicFlag == 0 || elLogicFlag == 3))//如果启用了负限位并且负限位为低电平有效，当负限位传感器为OFF，表明负极限传感器被遮挡
                    || (_axisELMinus[axisNumber] && (elEnableFlag == 1 || elEnableFlag == 2) && (elLogicFlag == 1 || elLogicFlag == 2))//如果启用了负限位并且负限位为高电平有效，当负限位传感器为ON，表明负极限传感器被遮挡
                    )
                {
                    //向正方向Jog移动,寻找原点传感器
                    AxisAction(901, axisNumber, startVelocity, maxVelocity, stopVelocity, accTime, decTime, 0, true, false, 0, false, 0);
                    while (!_axisORG[axisNumber])//等待找到原点传感器
                    {
                        Thread.Sleep(10);
                    }
                    i = 0;
                    while (i < 10)//通过原点传感器
                    {
                        if (_axisORG[axisNumber])
                            i++;
                        Thread.Sleep(10);
                    }
                    i = 0;
                    while (i < 10)//离开原点传感器
                    {
                        if (!_axisORG[axisNumber])
                            i++;
                        else
                            i = 0;
                        Thread.Sleep(10);
                    }
                    while (_axisORG[axisNumber])
                    {
                        Thread.Sleep(10);
                    }
                    AxisStop(0, axisNumber, 0);//停止移动
                    while (LTDMC.dmc_check_done(0, (ushort)axisNumber) == 0)//等待停止完成
                    {
                        Thread.Sleep(10);
                    }
                }
                //如果找到的是原点传感器
                else if (_axisORG[axisNumber])
                {
                    //向正方向Jog移动
                    AxisAction(901, axisNumber, startVelocity, maxVelocity, stopVelocity, accTime, decTime, 0, true, false, 0, false, 0);
                    i = 0;
                    while (i < 10)//离开原点传感器
                    {
                        if (!_axisORG[axisNumber])
                            i++;
                        else
                            i = 0;
                        Thread.Sleep(10);
                    }
                    while (_axisORG[axisNumber])
                    {
                        Thread.Sleep(10);
                    }
                    AxisStop(0, axisNumber, 0);//停止移动
                    while (LTDMC.dmc_check_done(0, (ushort)axisNumber) == 0)//等待停止完成
                    {
                        Thread.Sleep(10);
                    }
                }

                //回原点
                _axisHomedFlag[axisNumber] = false;
                LTDMC.dmc_set_profile(0, (ushort)axisNumber, axisHomeMoveStartVelocity_pps, axisHomeMoveMaxVelocity_pps, _axisHomeMoveAccTime_s[axisNumber], _axisHomeMoveDecTime_s[axisNumber], axisHomeMoveStopVelocity_pps);
                LTDMC.dmc_home_move(0, (ushort)axisNumber);
                _axisHomeStartFlag[axisNumber] = true;

            });

        }
        #endregion

        /*******输出IO，输出口号和输出口名称对应表*******
        0-OUT0, 1-OUT1, 2-OUT2, 3-OUT3, 4-OUT4, 5-OUT5
        6-OUT6, 7-OUT7, 8-OUT8, 9-OUT9, 10-OUT10, 11-OUT11
        12-OUT12/CMP0, 13-OUT13/CMP1, 14-OUT14/CMP2, 15-OUT15/CMP3
        16-SEVON0, 17-SEVON1, 18-SEVON2, 19-SEVON3, 20-SEVON4
        21-SEVON5, 22-SEVON6, 23-SEVON7, 24-ERC0, 25-ERC1, 26-ERC2
        27-ERC3, 28-ERC4, 29-ERC5, 30-ERC6, 31-ERC7
        ***********************************************/

        #region outPut:输出点ON/OFF
        /// <summary>
        /// OutputCtrl:输出点ON/OFF
        /// </summary>
        /// <param name="cardNumber">int:控制输出的卡编号</param>
        /// <param name="outputNumber">int:输出点号码</param>
        /// <param name="onOrOff">bool:输出点ON/OFF设置</param>
        /// 
        public int OutputCtrl(int cardNumber, int outputNumber, bool onOrOff)
        {
            int currentOutputData = Convert.ToInt32(LTDMC.dmc_read_outport((ushort)cardNumber, 0));//获取到当前的IO状态
            int setOutportData = 0;//需要设定的IO状态
            if (outputNumber <= 31)
            {
                if (onOrOff)
                    setOutportData = currentOutputData & (1 << outputNumber);
                else
                    setOutportData = currentOutputData & (~(1 << outputNumber));
            }
            else
            {
                return -1;
            }
            return Convert.ToInt32(LTDMC.dmc_write_outport((ushort)cardNumber, 0, (ushort)setOutportData));
        }
        #endregion

        #region GetOutput:读取输出点状态
        /// <summary>
        /// GetOutput:读取输出点状态
        /// </summary>
        /// <param name="cardNumber">int:读取输出状态的卡编号</param>
        /// <param name="outputNumber">int:输出点号码</param>
        public bool GetOutput(int cardNumber, int outputNumber)
        {
            int currentOutputStatus = Convert.ToInt32(LTDMC.dmc_read_outport((ushort)cardNumber, 0));
            if (outputNumber <= 31)
            {
                if (((currentOutputStatus >> outputNumber) & 1) == 0)
                    return false;
                else
                    return true;
            }
            else
            {
                throw new ArgumentOutOfRangeException("index"); //索引出错
            }
        }
        #endregion

        /*******输入IO，输入口号和输入口名称对应表*******
         * 第0组
        0-IN0, 1-IN1, 2-IN2, 3-IN3, 4-IN4, 5-IN5
        6-IN6, 7-IN7, 8-IN8, 9-IN9, 10-IN10, 11-IN11
        12-IN12, 13-IN13, 14-IN14/LTC0, 15-IN15/LTC1
        16-EL0+, 17-EL1+, 18-EL2+, 19-EL3+, 20-EL4+
        21-EL5+, 22-EL6+, 23-EL7+, 24-EL0-, 25-EL1-, 26-EL2-
        27-EL3-, 28-EL4-, 29-EL5-, 30-EL6-, 31-EL7-
         * 第1组
        0-ORG0, 1-ORG1, 2-ORG2, 3-ORG3, 4-ORG4, 5-ORG5
        6-ORG6, 7-ORG7, 8-ALM0, 9-ALM1, 10-ALM2, 11-ALM3
        12-ALM4, 13-ALM5, 14-ALM6, 15-ALM7
        16-RDY0, 17-RDY1, 18-RDY2, 19-RDY3, 20-RDY4
        21-RDY5, 22-RDY6, 23-RDY7, 24-INP0, 25-INP1, 26-INP2
        27-INP3, 28-INP4, 29-INP5, 30-INP6, 31-INP7
        ***********************************************/

        #region GetInput:读取输入点状态
        /// <summary>
        /// GetInput:读取输入点状态
        /// </summary>
        /// <param name="cardNumber">int:读取输入状态的卡编号</param>
        /// <param name="inputNumber">int:输入点号码</param>
        public bool GetInput(int cardNumber, int inputNumber)
        {
            int currentInputIOStatus = 0;
            int needGetIOIndex = inputNumber;
            if (inputNumber >= 0 && inputNumber <= 31)
                currentInputIOStatus = Convert.ToInt32(LTDMC.dmc_read_inport((ushort)cardNumber, 0));
            else if (inputNumber > 31 && inputNumber <= 63)
            {
                currentInputIOStatus = Convert.ToInt32(LTDMC.dmc_read_inport((ushort)cardNumber, 1));
                needGetIOIndex = inputNumber - 32;
            }
            else
            {
                throw new ArgumentOutOfRangeException("index"); //索引出错
            }

            if (((currentInputIOStatus >> needGetIOIndex) & 1) == 0)
                return false;
            else
                return true;
        }
        #endregion

        #region GetEncoderCount:读取编码器的计数值
        /// <summary>
        /// GetEncoderCount:读取编码器的计数值
        /// </summary>
        /// <param name="cardNumber">int:读取输入状态的卡编号</param>
        /// <param name="inputNumber">int:输入点号码</param>
        public int GetEncoderCount()
        {
            return LTDMC.dmc_get_encoder(0, 1);
        }
        #endregion

        #region SetEncoderCount:设置编码器的计数值
        /// <summary>
        /// SetEncoderCount:设置编码器的计数值
        /// </summary>
        public void SetEncoderCount(int setValue)
        {
            LTDMC.dmc_set_encoder(0, 1, setValue);
        }
        #endregion

        #region SetPositionOfAxis:设置轴的位置
        /// <summary>
        /// SetPositionOfAxis:设置轴的位置
        /// </summary>
        /// <param name="axisIndex">int:轴索引</param>
        /// <param name="newPosition">int:新的位置（pulse）</param>
        public void SetPositionOfAxis(int axisIndex, int newPosition)
        {
            LTDMC.dmc_set_position(0, (ushort)axisIndex, newPosition);
        }
        #endregion

        #region Close:卡及轴关闭
        /// <summary>
        /// Close:卡及轴关闭
        /// </summary>
        public int Close()
        {
            if (_initialFlag)
            {
                for (int i = 0; i < AXIS_QUANTITY; i++)
                {
                    JogAxisStop(0, i, 0);
                    _axisCommand[i] = 0;
                    _axisCommandSaved[i] = 0;
                    _axisCurrentPositionNumber[i] = 0;
                    _axisHomedFlag[i] = false;
                }

                //停止轴状态更新线程
                if (_axisStatusUpdataThread != null)
                {
                    _axisStatusUpdataThread.Abort();
                    _axisStatusUpdataThread = null;
                }

                _initialFlag = false;

                return LTDMC.dmc_board_close();
            }
            else
                return 0;
        }
        #endregion

        #region 辅助功能
        /// <summary>
        /// 根据Int类型的值，返回用1或0(对应True或Flase)填充的数组
        /// <remarks>从右侧开始向左索引(0~31)</remarks>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IEnumerable<bool> GetBitList(int value)
        {
            var list = new List<bool>(32);
            for (var i = 0; i <= 31; i++)
            {
                var val = 1 << i;
                list.Add((value & val) == val);
            }
            return list;
        }

        /// <summary>
        /// 返回Int数据中某一位是否为1
        /// </summary>S
        /// <param name="value"></param>
        /// <param name="index">32位数据的从右向左的偏移位索引(0~31)</param>
        /// <returns>true表示该位为1，false表示该位为0</returns>
        public static bool GetBitValue(int value, ushort index)
        {
            if (index > 31) throw new ArgumentOutOfRangeException("index"); //索引出错
            var val = 1 << index;
            return (value & val) == val;
        }

        public static bool GetBitValue16(short value, ushort index)
        {
            if (index > 15) throw new ArgumentOutOfRangeException("index"); //索引出错
            var val = (short)(1 << index);
            return (value & val) == val;
        }

        /// <summary>
        /// 设定Int数据中某一位的值
        /// </summary>
        /// <param name="value">位设定前的值</param>
        /// <param name="index">32位数据的从右向左的偏移位索引(0~31)</param>
        /// <param name="bitValue">true设该位为1,false设为0</param>
        /// <returns>返回位设定后的值</returns>
        public static int SetBitValue(int value, ushort index, bool bitValue)
        {
            if (index > 31) throw new ArgumentOutOfRangeException("index"); //索引出错
            var val = 1 << index;
            return bitValue ? (value | val) : (value & ~val);
        }

        public static short SetBitValue16(short value, ushort index, bool bitValue)
        {
            if (index > 15) throw new ArgumentOutOfRangeException("index"); //索引出错
            var val = (short)(1 << index);
            return (short)(bitValue ? (value | val) : (value & ~val));
        }
        #endregion
    }
}
