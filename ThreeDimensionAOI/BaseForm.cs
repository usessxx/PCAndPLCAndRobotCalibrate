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
using System.Diagnostics;//用于stopwatch
using System.Drawing.Printing;//用于启用printdocument类，来打印文件，pdf等
using System.Drawing.Drawing2D;
using CSVFile;
using AxisAndIOForm;//轴运动控制画面
using DAM02AIAO;
using HalconDotNet;
using HalconMVTec;
using MES;//使用MES相关命名空间，用于MES通讯
using Newtonsoft.Json;
using UserManager;
using ThreeDAVIInspectHistoryBrowse;//SPC界面


namespace ThreeDimensionAVI
{
    #region 构造体
    //计时器构造体，模拟PLC程序的T
    public struct T
    {
        long _timerSetTimeQuantity;//计时器计时时长，单位ms
        long _timerCurrentTime;//计时器当前时长，单位ms，当这个时长大于

        //计时器状态
        public bool STATUS
        {
            get
            {
                return _timerCurrentTime > _timerSetTimeQuantity ? true : false;
            }
        }

        //计时器设定时间
        public long SETTIME
        {
            set { _timerSetTimeQuantity = value; }
        }

        public long CURRENTTIME
        {
            set { _timerCurrentTime = value; }
            get { return _timerCurrentTime; }
        }
    };

    //相机图片推断用数据构造体
    public struct CameraImageInfer
    {
        /// <summary>
        /// _cameraPictureSavePath:记录图像的保存路径
        /// </summary>
        public string _cameraPictureSavePath;//记录图像的保存路径
        /// <summary>
        /// _cameraPicturePointName:用于记录相机图片对应的产品的点位号的名称
        /// </summary>
        public string _cameraPicturePointName;//用于记录相机图片对应的产品的点位号的名称
        /// <summary>
        /// _cameraPicturePointNumber:用于记录相机图片对应的产品的点位号，第一个点位为1
        /// </summary>
        public int _cameraPicturePointNumber;//用于记录相机图片对应的产品的点位号，第一个点位为1
        /// <summary>
        /// _cameraInferredResultOKPictureSavePath:记录相机图像推断完成并且结果为OK的保存路径
        /// </summary>
        public string _cameraInferredResultOKPictureSavePath;//记录相机图像推断完成并且结果为OK的保存路径
        /// <summary>
        /// _cameraInferredResultNGPictureSavePath:记录相机图像推断完成并且结果为NG的保存路径
        /// </summary>
        public string _cameraInferredResultNGPictureSavePath;//记录相机图像推断完成并且结果为NG的保存路径
        /// <summary>
        /// ho_CameraImage:内存中存储左侧相机获取到的图像
        /// </summary>
        public HObject ho_CameraImage;//内存中存储左侧相机获取到的图像
        /// <summary>
        /// _cameraPictureSavedFlag:相机拍摄的图片已经保存FLAG，false-还未保存，true-已经保存，和InferredFlag配套判定存储照片的Object是否要清为NULL
        /// </summary>
        public bool _cameraPictureSavedFlag;//相机拍摄的图片已经保存FLAG，false-还未保存，true-已经保存，和InferredFlag配套判定存储照片的Object是否要清为NULL
        /// <summary>
        /// _cameraPictureInferredFlag:相机拍摄的图片已经完成推断标志，false-还未判定，true-已经判定，和savedFlag配套判定存储照片的Object是否要清为NULL
        /// </summary>
        public bool _cameraPictureInferredFlag;//相机拍摄的图片已经完成推断标志，false-还未判定，true-已经判定，和savedFlag配套判定存储照片的Object是否要清为NULL
        /// <summary>
        /// _cameraPictureRequestInferFlag:相机拍摄的图片请求进行深度学习判定FLAG，false-不请求判定，true-请求判定，主要是确保该赋值的参数都完成赋值然后才进行infer
        /// </summary>
        public bool _cameraPictureRequestInferFlag;//相机拍摄的图片请求进行深度学习判定FLAG，false-不请求判定，true-请求判定，主要是确保该赋值的参数都完成赋值然后才进行infer
        /// <summary>
        /// _productIndex:用于标识此图像是属于哪一个产品的，0-传送线1产品，1-传送线2产品
        /// </summary>
        public int _productIndex;//用于标识此图像是属于哪一个产品的，0-传送线1产品，1-传送线2产品
    };

    /// <summary>
    /// InferResult：推断结果用数据构造体
    /// </summary>
    public struct InferResult
    {
        /// <summary>
        /// _elementNumber：元件序号
        /// </summary>
        public string _elementNumber;
        /// <summary>
        /// _elementNumber：元件名
        /// </summary>
        public string _elementName;
        /// <summary>
        /// _elementLeftResult：元件左侧推断结果
        /// </summary>
        public string _elementLeftResult;
        /// <summary>
        /// _elementRightResult：元件右侧推断结果
        /// </summary>
        public string _elementRightResult;
        /// <summary>
        /// _elementTotalResult：元件推断结果
        /// </summary>
        public string _elementTotalResult;
    };

    #endregion

    public partial class BaseForm : Form
    {
        /*--------------------------------------------------------------------------------------
        //Copyright (C) 2022 深圳市利器精工科技有限公司
        //版权所有
        //
        //文件名：界面主窗口
        //文件功能描述：作为整个程序的基础，最基础的控制都在此窗口中完成，其余自窗口的动作最终都会汇总到此窗口进行
        //
        //
        //创建标识：MaLi 20220316
        //
        //修改标识：MaLi 20220316 Change
        //修改描述：增加界面主窗口
        //
        //------------------------------------------------------------------------------------*/
        //
        /************************************静态变量******************************************/
        public int _axisQuantity;//轴的个数
        public int _maxIOQuantity;//输入或者输出的IO个数(一块卡为输入及输出都为64点,两块卡为128)

        const int ALARM_KIND_QUANTITY = 700;//报警，分为致命报警（index:0-99），重度报警(index:100-299)，中度报警(index:300-499)，轻度报警(index:500-699)
        const int STATE_KIND_QUANTITY = 100;//状态种类计数
        const int TIME_COUNT_TIME_INVERTAL = 10;//计时器计时间隔

        const int START_BT_LAMP_OUT_IO_NO = 12;//启动按钮灯输出io号
        const int STOP_BT_LAMP_OUT_IO_NO = 13;//停止按钮灯输出io号
        const int RESET_BT_LAMP_OUT_IO_NO = 14;//复位按钮灯输出io号
        const int MEASURE_START_BT_LAMP_OUT_IO_NO = 21;//测量启动按钮灯输出io号

        const int TOWER_LAMP_RED_OUT_IO_NO = 17;//三色灯，红灯输出io号
        const int TOWER_LAMP_YELLOW_OUT_IO_NO = 18;//三色灯，黄灯输出io号
        const int TOWER_LAMP_GREEN_OUT_IO_NO = 19;//三色灯，绿灯输出io号
        const int TOWER_LAMP_BUZZER_OUT_IO_NO = 20;//三色灯，蜂鸣器输出io号

        const int START_BT_IN_IO_NO = 4;//启动按钮灯输入io号
        const int STOP_BT_IN_IO_NO = 5;//停止按钮灯输入io号
        const int RESET_BT_IN_IO_NO = 6;//复位按钮灯输入io号
        const int MEASURE_START_BT_IN_IO_NO = 8;//测量启动按钮灯输入io号

        const int EMERGENCY_BT_IN_IO = 7;//急停按钮输入IO号
        const int AIR_PRESSURE_IN_IO = 9;//气压检测信号IO号

        const int LEFT_CAMERA_TRIGGER_OUT_IO = 15;//左侧相机触发输出IO
        const int RIGHT_CAMERA_TRIGGER_OUT_IO = 23;//右侧相机触发输出IO
        const int BACK_CAMERA_LIGHT_TRIGGER_OUT_IO = 84;//后侧相机光源触发输出IO

        //两台机所用IO不同，导致设置两组IO用于兼容
        const int LEFT_CAMERA_TRIGGER_OUT_IO2 = 25;//左侧相机触发输出IO2
        const int RIGHT_CAMERA_TRIGGER_OUT_IO2 = 26;//右侧相机触发输出IO2
        const int BACK_CAMERA_LIGHT_TRIGGER_OUT_IO2 = 24;//后侧相机光源触发输出IO2


        const int SAVE_IMAGE_QUANTITY_IN_RAM_FOR_RUNNING = 30;//存储在内存中的图片数量
        /*************************外部可设定读取参数*******************************/

        //*************************公共静态变量*******************************//
        public static int _logFileMaxSize = 10240;//log文件的大小上限
        public static float[] _axisRealPosition;//轴真实坐标
        public static float[] _axisAbsolutePosition;//轴绝对坐标
        public static bool _autoModeFlag = false;//自动模式标志
        public static StateAndAlarmClass _stateAndAlarmCs = null;//状态及报警类
        public static T[] _inputIOT;//用于输入IO的计时T
        public static T[] _outputIOT;//用于输出IO的计时T

        /// <summary>
        /// _deviceControlLogFolderPath:设备控制LOG文件夹路径
        /// </summary>
        public static string _deviceControlLogFolderPath = Directory.GetCurrentDirectory() + "\\LogFile\\DeviceControlLog";
        /// <summary>
        /// _mesCommunicationLogFolderPath:Mes通讯LOG文件文件夹路径
        /// </summary>
        public static string _mesCommunicationLogFolderPath = Directory.GetCurrentDirectory() + "\\LogFile\\MESLog";
        //*************************内部私有变量*******************************//
        private MainForm _mainFormVariate = null;//定义主窗口界面变量
        private SettingBaseForm _baseSettingFormVariate = null;//定义参数设定窗口界面变量
        private LaunchForm _launchFormVariate = null;//开始启动画面
        private AxisAndIOForm.AxisControlMainForm _axisControlFormVariate = null;//定义轴控制界面变量
        private AlarmForm _alarmFormVariate = null;//报警form变量定义
        private ThreeDAVIInspectHistoryBrowseMainForm _SPCFormVariate = null;//定义spc界面变量

        //基础页面状态状态显示框相关参数
        string _baseFormStateOrAlarmMsg;
        string _baseFormOldStateOrAlarmMsg;//用于判定Base页面报警信息是否发生变化
        int _baseFormStateStartIndex;//由于提示状态信息或是报警信息长度会大于显示框，这种情况下就会需要显示信息滚动播放，此处就是用于标识滚动播放时的Index
        System.Threading.Timer _stateAndAlarmDispTimer = null;//状态及报警信息动态显示Timer

        string _softVersion = "程序版本：V1.00-20220508";

        Thread _formUpdateThread = null;//定义变量，界面状态更新线程

        bool _closeSoftWithOutInfoFlag = false;//当有另一个程序运行时，不需要弹出是否要退出再次确定按钮

        /// <summary>
        /// 虚拟及实体按钮点击Index，0-点击自动按钮，1-点击手动按钮，2-上料检测按钮，10-启动按钮，11-停止按钮，12-复位按钮
        /// </summary>
        int _virtualAndRealBtnIndex = -1;

        #region 报警相关
        bool _alarmOccurFlag = false;//报警发生标志
        bool _pauseAlarmOccurFlag = false;//暂停报警发生标志
        bool _deadlyAlarmOccurFlag = false;//致命报警发生标志
        bool _heavyAlarmOccurFlag = false;//重度报警发生标志
        bool _moderateAlarmOccurFlag = false;//中度报警发生标志
        bool _slightAlarmOccurFlag = false;//轻度报警发生标志
        bool _afterAlarmNeedPressResetBtnFlag = false;//当出现报警之后，需要点击复位按钮标志
        /// <summary>
        /// _afterPressResetBtnNeedPressStartBtnFlag:当出现报警之后，复位报警之后，需要点击启动按钮标志,false-不需要，true-需要点击start按钮
        /// </summary>
        bool _afterPressResetBtnNeedPressStartBtnFlag = false;

        //报警延时计时器
        T _airPressureLowAlarmT;//气压低报警超时计时器
        T _cameraMoveCylinderBackAlarmT;//相机移动气缸回超时报警计时器 
        T _cameraMoveCylinderOutAlarmT;//相机移动气缸出超时报警计时器 
        T _conveyor1StopCylinderDownAlarmT;//传送线1(里侧)阻挡气缸下超时报警计时器 
        T _conveyor1StopCylinderUpAlarmT;//传送线1(里侧)阻挡气缸上超时报警计时器  
        T _conveyor1ClampCylinderReleaseAlarmT;//传送线1(里侧)夹紧气缸松开超时报警计时器 
        T _conveyor1ClampCylinderClampAlarmT;//传送线1(里侧)夹紧气缸夹紧超时报警计时器  
        T _conveyor2StopCylinderDownAlarmT;//传送线2(外侧)阻挡气缸下超时报警计时器 
        T _conveyor2StopCylinderUpAlarmT;//传送线2(外侧)阻挡气缸上超时报警计时器  
        T _conveyor2ClampCylinderReleaseAlarmT;//传送线2(外侧)夹紧气缸松开超时报警计时器 
        T _conveyor2ClampCylinderClampAlarmT;//传送线2(外侧)夹紧气缸夹紧超时报警计时器

        #endregion

        Thread _timerUseInSoftCountThread = null;//线程，专门用于计时，用于模拟PLC T的功能

        T _changeDeviceToManualModeT;//切换设备至自动模式延时计时器
        T _device500TwinkleOnT, _device500TwinkleOffT;//启动按钮ON/OFF延时计时器,500ms闪烁
        bool _device500TwinkeOnOffFlag = false;//设备500ms闪烁ON OFF标志，false-OFF,true-ON

        T _device200TwinkleOnT, _device200TwinkleOffT;//启动按钮ON/OFF延时计时器,200ms闪烁
        bool _device200TwinkeOnOffFlag = false;//设备200ms闪烁ON OFF标志，false-OFF,true-ON

        /// <summary>
        /// _startBtnLampOnOffFlag：启动按钮灯ON OFF标志，0-灭，1-亮，2-500ms间隔闪烁，3-200ms间隔闪烁
        /// </summary>
        int _startBtnLampOnOffFlag = 0;
        /// <summary>
        /// _stopBtnLampOnOffFlag：停止按钮灯ON OFF标志，0-灭，1-亮，2-500ms间隔闪烁，3-200ms间隔闪烁
        /// </summary>
        int _stopBtnLampOnOffFlag = 0;
        /// <summary>
        /// _resetBtnLampOnOffFlag：复位按钮灯ON OFF标志，0-灭，1-亮，2-500ms间隔闪烁，3-200ms间隔闪烁
        /// </summary>
        int _resetBtnLampOnOffFlag = 0;
        /// <summary>
        /// _measureStartBtnLampOnOffFlag：测量启动按钮灯ON OFF标志，0-灭，1-亮，2-500ms间隔闪烁，3-200ms间隔闪烁
        /// </summary>
        int _measureStartBtnLampOnOffFlag = 0;
        /// <summary>
        /// _towerLampRedOnOffFlag：三色灯红灯ON OFF标志，0-灭，1-亮，2-500ms间隔闪烁，3-200ms间隔闪烁
        /// </summary>
        int _towerLampRedOnOffFlag = 0;
        /// <summary>
        /// _towerLampGreenOnOffFlag：三色灯绿灯ON OFF标志，0-灭，1-亮，2-500ms间隔闪烁，3-200ms间隔闪烁
        /// </summary>
        int _towerLampGreenOnOffFlag = 0;
        /// <summary>
        /// _towerLampYellowOnOffFlag：三色灯黄灯ON OFF标志，0-灭，1-亮，2-500ms间隔闪烁，3-200ms间隔闪烁
        /// </summary>
        int _towerLampYellowOnOffFlag = 0;
        /// <summary>
        /// _towerLampBuzzerOnOffFlag：三色灯蜂鸣器ON OFF标志，0-不响，1-常响，2-500ms间隔响，3-200ms间隔响
        /// </summary>
        int _towerLampBuzzerOnOffFlag = 0;
        /// <summary>
        /// _towerLampBuzzerNeedActionFlag:三色灯蜂鸣器应当响Flag，用于确保出现报警时响，当按下启动，复位，停止按钮之后停止响，直至下一次再由无报警变成有报警
        /// </summary>
        bool _towerLampBuzzerNeedActionFlag = true;

        #region 运动控制相关变量
        //*************************运动控制相关变量*******************************//

        #region 模块相关
        /// <summary>
        /// _xyMoveToAssignedPointPositionModuleActionFlag:xy轴移动至指定点位号点位模块启用标签，-1-没有启用，0-启用中，其余大于0的数，当前点位号
        /// </summary>
        int _xyMoveToAssignedPointPositionModuleActionFlag = -1;
        /// <summary>
        ///_allAxisHomeBackModuleActionFlag: 所有轴回原点模块启用标签，-1-没有启用，0-启用中，1-启用完成
        /// </summary>
        int _allAxisHomeBackModuleActionFlag = -1;
        /// <summary>
        ///_axesGotoStandbyModuleActionFlag: X1Y1R1轴回等待位模块启用标签，-1-没有启用，0-启用中，1-启用完成
        /// </summary>
        int _axesGotoStandbyModuleActionFlag = -1;
        /// <summary>
        ///_conveyorChangeToAssignedWidthModuleActionFlag: 调整传送线至对应宽度模块启用标签，-1-没有启用，0-启用中，1-启用完成
        /// </summary>
        int _conveyorChangeToAssignedWidthModuleActionFlag = -1;
        /// <summary>
        ///_conveyor1ActionModuleActionFlag: 传送线1动作模块启用标签，-1-没有启用，0-启用中，1-启用完成-动作，2-启用完成-停止
        /// </summary>
        public static int _conveyor1ActionModuleActionFlag = -1;
        /// <summary>
        ///_conveyor2ActionModuleActionFlag: 传送线2动作模块启用标签，-1-没有启用，0-启用中，1-启用完成-动作，2-启用完成-停止
        /// </summary>
        public static int _conveyor2ActionModuleActionFlag = -1;
        /// <summary>
        ///_conveyor1LoadUnloadModuleActionFlag: 传送线1上下料模块启用标签，-1-没有启用，0-启用中，1-启用完成
        /// </summary>
        int _conveyor1LoadUnloadModuleActionFlag = -1;
        /// <summary>
        /// _conveyor1HaveProductFlag:当传送线1下料时，有产品进入标志，false-无，true-有
        /// </summary>
        bool _conveyor1HaveProductFlag = false;
        /// <summary>
        ///_conveyor2LoadUnloadModuleActionFlag: 传送线2上下料模块启用标签，-1-没有启用，0-启用中，1-启用完成
        /// </summary>
        int _conveyor2LoadUnloadModuleActionFlag = -1;
        /// <summary>
        /// _conveyor2HaveProductFlag:当传送线2下料时，有产品进入标志，false-无，true-有
        /// </summary>
        bool _conveyor2HaveProductFlag = false;
        /// <summary>
        /// _deviceTotalWorkModuleOffDlyT:设备整体工作模块启用标签，-1-没有启用，0-启用中，1-启用完成
        /// </summary>
        int _deviceTotalWorkModuleActionFlag = -1;
        /// <summary>
        /// _barcodeScanModuleActionFlag:扫码及数据模块启用标签，-1-没有启用，0-启用中，1-扫码成功，2-扫码失败
        /// </summary>
        int _barcodeScanModuleActionFlag = -1;
        /// <summary>
        /// _getOrSendProductInfoFromMESModuleActionFlag:获取或传输数据至MES模块启用标签，-1-没有启用，0-启用中，1-获取数据成功，2-获取数据失败，11-传输数据成功，12-传输数据失败
        /// </summary>
        int _getOrSendProductInfoFromMESModuleActionFlag = -1;
        /// <summary>
        /// _deviceRecognizeMarkPositionAndGetCorrectMatrixModuleActionFlag:设备识别MARK点并计算出纠正矩阵模块启用标签，-1-没有启用，0-启用中，1-获取传送线1上产品纠正矩阵成功，2-获取传送线1上产品纠正矩阵失败，11-获取传送线2上产品纠正矩阵成功，12-获取传送线2上产品纠正矩阵失败
        /// </summary>
        int _deviceRecognizeMarkPositionAndGetCorrectMatrixModuleActionFlag = -1;
        /// <summary>
        /// _autoModuleActionFlag:自动模块启用标签，-1-没有启用，0-启用中，1-启动完成
        /// </summary>
        int _autoModuleActionFlag = -1;
        /// <summary>
        /// _conveyor1ProductForcedUnloadModuleActionFlag:传送线1产品强制下料模块启用标签，-1-没有启用，0-启用中，1-启动完成
        /// </summary>
        int _conveyor1ProductForcedUnloadModuleActionFlag = -1;
        /// <summary>
        /// _conveyor2ProductForcedUnloadModuleActionFlag:传送线2产品强制下料模块启用标签，-1-没有启用，0-启用中，1-启动完成
        /// </summary>
        int _conveyor2ProductForcedUnloadModuleActionFlag = -1;

        /// <summary>
        /// _leftAndRightCameraGrabImageModuleActionFlag:左右相机采集模块启用标签，-1-没有启用，0-启用中，1-启动完成，2-相机采集失败
        /// </summary>
        int _leftAndRightCameraGrabImageModuleActionFlag = -1;
        /// <summary>
        /// bool:为了提高节拍，在采集完毕之后就直接进行下一点的移动
        /// </summary>
        bool _triggerGrabImageFinishFlag = false;//为了提高节拍，在采集完毕之后就直接进行下一点的移动
        /// <summary>
        /// _leftAndRightCameraGrabImageModuleControlFlag:为了避免异步方法中调用异步方法，将此模块从DeviceTotalWorkModule中移出，然后在Automodule中调用，利用这个Flag来判定是否要开始，0-空，1-请求抓取图像，2-开始抓取图像模块
        /// </summary>
        int _leftAndRightCameraGrabImageModuleControlFlag = 0;
        /// <summary>
        /// int: 抓取图像失败重试次数，用于判定是否多次出现抓取不了图像的问题
        /// </summary>
        int _grabImageFailedRetryCount = 0;
        /// <summary>
        /// _leftCameraImageInferModuleActionFlag:左侧相机采集图片推断模块启用标签，-1-没有启用，0-启用中，1-启用完成
        /// </summary>
        int _leftCameraImageInferModuleActionFlag = -1;
        /// <summary>
        /// _rightCameraImageInferModuleActionFlag:右侧相机采集图片推断模块启用标签，-1-没有启用，0-启用中，1-启用完成
        /// </summary>
        int _rightCameraImageInferModuleActionFlag = -1;
        /// <summary>
        /// _leftCameraSourceImageSaveModuleActionFlag:左侧相机采集原始图片保存模块启用标签，-1-没有启用，0-启用中，1-启动完成
        /// </summary>
        int _leftCameraSourceImageSaveModuleActionFlag = -1;
        /// <summary>
        /// _rightCameraSourceImageSaveModuleActionFlag:右侧相机采集原始图片保存模块启用标签，-1-没有启用，0-启用中，1-启动完成
        /// </summary>
        int _rightCameraSourceImageSaveModuleActionFlag = -1;
        /// <summary>
        /// _leftCameraInferredImageSaveModuleActionFlag:左侧相机采集图片的推断结果图片保存模块启用标签，-1-没有启用，0-启用中，1-启动完成
        /// </summary>
        int _leftCameraInferredImageSaveModuleActionFlag = -1;
        /// <summary>
        /// _rightCameraInferredImageSaveModuleActionFlag:左侧相机采集图片的推断结果图片保存模块启用标签，-1-没有启用，0-启用中，1-启动完成
        /// </summary>
        int _rightCameraInferredImageSaveModuleActionFlag = -1;
        /// <summary>
        /// _cameraInferDataClearModuleActionFlag:相机推断用数据清除模块启用标签，-1-没有启用，0-启用中，1-启动完成
        /// </summary>
        int _cameraInferDataClearModuleActionFlag = -1;


        /// <summary>
        /// _xyMoveToAssignedPointPositionModuleOffDlyT:XY移动至指定点位号模块OFF延时计时器
        /// </summary>
        public static T _xyMoveToAssignedPointPositionModuleOffDlyT;
        /// <summary>
        /// _allAxisHomeBackModuleOffDlyT:所有轴原点回归模块OFF延时计时器
        /// </summary>
        T _allAxisHomeBackModuleOffDlyT;
        /// <summary>
        /// _axesGotoStandbyModuleOffDlyT:X1Y1R1轴回等待位模块OFF延时计时器
        /// </summary>
        T _axesGotoStandbyModuleOffDlyT;
        /// <summary>
        /// _conveyorChangeToAssignedWidthModuleOffDlyT:调整传送线至对应宽度模块OFF延时计时器
        /// </summary>
        public static T _conveyorChangeToAssignedWidthModuleOffDlyT;
        /// <summary>
        /// _conveyor1ActionModuleOffDlyT:传送线1动作模块OFF延时计时器
        /// </summary>
        T _conveyor1ActionModuleOffDlyT;
        /// <summary>
        /// _conveyor2ActionModuleOffDlyT:传送线2动作模块OFF延时计时器
        /// </summary>
        T _conveyor2ActionModuleOffDlyT;
        /// <summary>
        /// _conveyor1LoadUnloadModuleOffDlyT:传送线1上下料模块OFF延时计时器
        /// </summary>
        T _conveyor1LoadUnloadModuleOffDlyT;
        /// <summary>
        /// _conveyor2LoadUnloadModuleOffDlyT:传送线2上下料模块OFF延时计时器
        /// </summary>
        T _conveyor2LoadUnloadModuleOffDlyT;
        /// <summary>
        /// _deviceTotalWorkModuleOffDlyT:设备整体工作模块OFF延时计时器
        /// </summary>
        T _deviceTotalWorkModuleOffDlyT;
        /// <summary>
        /// _barcodeScanModuleOffDlyT:扫码及数据模块OFF延时计时器
        /// </summary>
        T _barcodeScanModuleOffDlyT;
        /// <summary>
        /// _getOrSendProductInfoFromMESModuleOffDlyT:获取或传输数据至MES模块OFF延时计时器
        /// </summary>
        T _getOrSendProductInfoFromMESModuleOffDlyT;
        /// <summary>
        /// _deviceRecognizeMarkPositionAndGetCorrectMatrixModuleOffDlyT:设备识别MARK点并计算出纠正矩阵模块OFF延时计时器
        /// </summary>
        T _deviceRecognizeMarkPositionAndGetCorrectMatrixModuleOffDlyT;
        /// <summary>
        /// _autoModuleOffDlyT:自动模块OFF延时计时器
        /// </summary>
        T _autoModuleOffDlyT;

        /// <summary>
        /// _leftAndRightCameraGrabImageModuleOffDlyT:左右相机采集模块OFF延时计时器
        /// </summary>
        T _leftAndRightCameraGrabImageModuleOffDlyT;
        /// <summary>
        /// _leftCameraImageInferModuleOffDlyT:左侧相机采集图片推断模块OFF延时计时器
        /// </summary>
        T _leftCameraImageInferModuleOffDlyT;
        /// <summary>
        /// _rightCameraImageInferModuleOffDlyT:右侧相机采集图片推断模块OFF延时计时器
        /// </summary>
        T _rightCameraImageInferModuleOffDlyT;
        /// <summary>
        /// _leftCameraSourceImageSaveModuleOffDlyT:左侧相机采集原始图片保存模块OFF延时计时器
        /// </summary>
        T _leftCameraSourceImageSaveModuleOffDlyT;
        /// <summary>
        /// _rightCameraSourceImageSaveModuleOffDlyT:右侧相机采集原始图片保存模块OFF延时计时器
        /// </summary>
        T _rightCameraSourceImageSaveModuleOffDlyT;
        /// <summary>
        /// _leftCameraInferredImageSaveModuleOffDlyT:左侧相机采集图片的推断结果图片保存模块OFF延时计时器
        /// </summary>
        T _leftCameraInferredImageSaveModuleOffDlyT;
        /// <summary>
        /// _rightCameraInferredImageSaveModuleOffDlyT:左侧相机采集图片的推断结果图片保存模块OFF延时计时器
        /// </summary>
        T _rightCameraInferredImageSaveModuleOffDlyT;
        /// <summary>
        /// _cameraInferDataClearModuleOffDlyT:相机推断用数据清除模块OFF延时计时器
        /// </summary>
        T _cameraInferDataClearModuleOffDlyT;
        #endregion

        /// <summary>
        /// _deviceWorkTriggerFlag: 轴运行Trigger标志，false-设备无法继续往下运行，true-可以继续往下运行
        /// </summary>
        bool _deviceWorkTriggerFlag = true;
        /// <summary>
        /// _deviceWorkTriggerFlagTriggerOneCircleFlag：设备工作Trigger信号，触发一次标志，用于提取start按钮上升沿
        /// </summary>
        bool _deviceWorkTriggerFlagTriggerOneCircleFlag = false;

        /// <summary>
        /// _currentDestinationPointIndex:当前工作点位工作索引
        /// </summary>
        int _currentDestinationPointIndex = 0;

        /// <summary>
        /// _deviceStartDelay:设备启动延时
        /// </summary>
        T _deviceStartDelay;

        /// <summary>
        /// _conveyor1FinishLoadT:传送线1上料完成延时，用于对传送线1减速信号延时，当延时时间达到，认定为上料到位
        /// </summary>
        T _conveyor1FinishLoadT;
        /// <summary>
        /// _conveyor1FinishUnloadT:传送线1下料完成延时，用于对传送线流出信号延时，当延时时间达到，认定为下料完毕
        /// </summary>
        T _conveyor1FinishUnloadT;
        /// <summary>
        /// _conveyor1UnloadStopCylinderOnT:传送线1下料时阻挡气缸上升延时，当开始下料时，为了防止后面需要检测的产品也一起流出，当离开产品检测传感器（减速传感器）一定时间之后，阻挡气缸上升。用于阻挡后面的产品
        /// </summary>
        T _conveyor1UnloadStopCylinderUpT;
        /// <summary>
        /// _conveyor1LoadTimeOutT:传送线1上料超时延时计时器，用于判定是否上料超时
        /// </summary>
        T _conveyor1LoadTimeOutT;
        /// <summary>
        /// _conveyor1UnloadTimeOutT:传送线1下料超时延时计时器，用于判定是否下料超时
        /// </summary>
        T _conveyor1UnloadTimeOutT;

        /// <summary>
        /// _conveyor2FinishLoadT:传送线2上料完成延时，用于对传送线1减速信号延时，当延时时间达到，认定为上料到位
        /// </summary>
        T _conveyor2FinishLoadT;
        /// <summary>
        /// _conveyor2FinishUnloadT:传送线2下料完成延时，用于对传送线流出信号延时，当延时时间达到，认定为下料完毕
        /// </summary>
        T _conveyor2FinishUnloadT;
        /// <summary>
        /// _conveyor2UnloadStopCylinderOnT:传送线2下料时阻挡气缸上升延时，当开始下料时，为了防止后面需要检测的产品也一起流出，当离开产品检测传感器（减速传感器）一定时间之后，阻挡气缸上升。用于阻挡后面的产品
        /// </summary>
        T _conveyor2UnloadStopCylinderUpT;
        /// <summary>
        /// _conveyor2LoadTimeOutT:传送线2上料超时延时计时器，用于判定是否上料超时
        /// </summary>
        T _conveyor2LoadTimeOutT;
        /// <summary>
        /// _conveyor2UnloadTimeOutT:传送线2下料超时延时计时器，用于判定是否下料超时
        /// </summary>
        T _conveyor2UnloadTimeOutT;

        Thread _deviceActionThread = null;//设备运行线程

        /// <summary>
        /// productParameter:产品数据类，0-传送线1，1-传送线2
        /// </summary>
        ProductRecipe[] _productParameter = new ProductRecipe[2];

        /// <summary>
        /// _conveyor1Status:传送线1状态，0-需要上料，1-需要检测，2-需要下料，
        /// 100-传送线1上产品上料完成，拍摄mark点，纠正，101-纠正完成，扫码，
        /// 102-扫码完成，与MES入站通讯，103-MES入站通讯完成，运动拍摄，
        /// 104-运动拍摄完成，等待检测完成，105-检测完成，传输结果数据至MES，106-传输结果数据至MES成功
        /// </summary>
        uint _conveyor1Status = 0;
        /// <summary>
        /// _conveyor2Status:传送线1状态，0-需要上料，1-需要检测，2-需要下料，
        /// 100-传送线2上产品上料完成，拍摄mark点，纠正，101-纠正完成，扫码，
        /// 102-扫码完成，与MES入站通讯，103-MES入站通讯完成，运动拍摄，
        /// 104-运动拍摄完成，等待检测完成，105-检测完成，传输结果数据至MES，106-传输结果数据至MES成功
        /// </summary>
        uint _conveyor2Status = 0;//传送线2状态，0-需要上料，1-需要检测，2-需要下料
        /// <summary>
        /// _barcodeData：从条形码中获取的数据,0-传送线1，1-传送线2
        /// </summary>
        string[] _barcodeData = new string[2] { "", "" };

        DAM02AIAOOperation _newDAM02AIAOOperation;//传送线控制类
        #endregion

        #region 图像处理相关变量

        HalconMVTec.HalconCameraControl _leftCameraCs = null;//左相机类
        HalconMVTec.HalconCameraControl _rightCameraCs = null;//右相机类
        HalconMVTec.HalconCameraControl _backCameraCs = null;//后相机类

        Thread _cameraImageGrabAndDispThread = null;//相机获取并显示图像线程
        bool _leftCameraVideo = false, _rightCameraVideo = false, _backCameraVideo = false;//相机实时显示标志
        bool _leftCameraGrab = false, _rightCameraGrab = false, _backCameraGrab = false;//相机抓取图像标志
        bool _backCamraDispCrossLineFlag = false;//后侧相机显示十字线标志

        CameraImageInfer[] _leftCameraInfer = new CameraImageInfer[SAVE_IMAGE_QUANTITY_IN_RAM_FOR_RUNNING];//左侧相机图像采集推断数据存储数组
        CameraImageInfer[] _rightCameraInfer = new CameraImageInfer[SAVE_IMAGE_QUANTITY_IN_RAM_FOR_RUNNING];//右侧相机图像采集推断数据存储数组

        HalconMVTec.DeepLearningObjectDetectionRectangle1Infer _newLeftDeepLearningObjectDetectionRectangle1Infer = new DeepLearningObjectDetectionRectangle1Infer();//左侧图像深度学习推断
        HalconMVTec.DeepLearningObjectDetectionRectangle1Infer _newRightDeepLearningObjectDetectionRectangle1Infer = new DeepLearningObjectDetectionRectangle1Infer();//右侧图像深度学习推断

        bool _leftInferNeedReadParameter = true;//左侧推断需要读取参数，第一次启用的时候，推断需要深度学习读取参数，建立模型
        bool _rightInferNeedReadParameter = true;//右侧推断需要读取参数，第一次启用的时候，推断需要深度学习读取参数，建立模型

        HObject[] ho_LeftInferredResultPictureStoreArray = new HObject[SAVE_IMAGE_QUANTITY_IN_RAM_FOR_RUNNING];//存储左侧推断结果的图片
        HObject[] ho_RightInferredResultPictureStoreArray = new HObject[SAVE_IMAGE_QUANTITY_IN_RAM_FOR_RUNNING];//存储右侧推断结果的图片
        string[] _leftInferredResultPictureSavePathArray = new string[SAVE_IMAGE_QUANTITY_IN_RAM_FOR_RUNNING];//存储左侧推断结果图片的保存路径
        string[] _rightInferredResultPictureSavePathArray = new string[SAVE_IMAGE_QUANTITY_IN_RAM_FOR_RUNNING];//存储右侧推断结果图片的保存路径


        //是由于多个线程同时访问一个datatable会导致错误，所以分为左右两侧
        DataTable[] _leftInferResultDt = new DataTable[2];//传送线1，传送线2左侧推断结果DataTable，包含序号，名称，左侧结果，右侧结果，整体结果，0-传送线1，1-传送线2
        DataTable[] _rightInferResultDt = new DataTable[2];//传送线1，传送线2右侧推断结果DataTable，包含序号，名称，左侧结果，右侧结果，整体结果，0-传送线1，1-传送线2

        //最近一次推断所需数据路径，用于判定是否需要重新读取AI推断所需参数
        string _lastTimepreprocessFilePath = "";
        string _lastTimeDimensionFilePath = "";
        string _lastTimeModelFilePath = "";

        //用于记录推断的结果,由于Datatable不能多线程使用，因此这里使用dictionary来尝试
        Dictionary<int, InferResult> _conveyor1InferResultDic = new Dictionary<int, InferResult>();
        Dictionary<int, InferResult> _conveyor2InferResultDic = new Dictionary<int, InferResult>();

        //Mark点识别结果XY偏移量
        float _markSearchXOffsetData = 0f;
        float _markSearchYOffsetData = 0f;

        #endregion

        #region MES
        MESOperation _newMesOperation;//定义MES通讯用操作类变量

        DataTable _needInferOrNotDt = new DataTable();
        #endregion

        #region 计时
        Stopwatch _conveyor1CheckTimeCountSW = new Stopwatch();//传送线1检测用时，从上料开始到流出为止用时
        Stopwatch _conveyor2CheckTimeCountSW = new Stopwatch();//传送线2检测用时，从上料开始到流出为止用时
        Stopwatch _inferTimCountSW = new Stopwatch();//推断用时
        #endregion

        #region Other
        /// <summary>
        /// 产量记录datatable
        /// </summary>
        DataTable _yieldRecordDt = new DataTable();
        /// <summary>
        /// _needResetYieldFlag：从ini文件中读取出清除标志，当时间超过设定时限后需要对产量自动进行清理，但是在运行过程中有可能有软件重启的情况，重启之后如果之前清除过，不能再进行清除。
        /// 0-需要清除晚班数据，1-需要清除白班数据，当为0时，如果超过早上设定的时间，自动清除数据，清除之后置为1，如果读取值为1，现在处于白班时间，那么不清除
        /// </summary>
        uint _yieldResetFlag = 0;

        /// <summary>
        /// _userManagerVariate:用户管理登录页面变量
        /// </summary>
        UserCreateAndLogTotalForm _userManagerVariate = null;
        /// <summary>
        /// _operatorName:当前操作人员名称
        /// </summary>
        public static string _operatorName = "";
        /// <summary>
        /// _accessLevel:权限等级0-未登录，100-操作员权限，101-工程师权限，102-厂家权限
        /// </summary>
        int _accessLevel = 0;

        /// <summary>
        /// _barcodeRecheckFormVariate：二维码二次检测页面变量
        /// </summary>
        RecheckOrChangeBarcodeForm _barcodeRecheckFormVariate = null;
        /// <summary>
        /// tempBarcode:用于接受识别到的二维码数据或是二次检测或是修改后的数据
        /// </summary>
        string _tempBarcode = "";
        /// <summary>
        /// _giveUpFlag:放弃标签，用于标识二维码二次确认框的操作结果，true-取消，false-确认
        /// </summary>
        bool _barcodeRecheckGiveUpFlag = false;

        /// <summary>
        /// _informVariate:消息提示界面变量
        /// </summary>
        InfoForm _informVariate = null;
        /// <summary>
        /// _informclickBtnIndex：弹出的消息提示框点击按钮索引，0-点击确定按钮，1-点击流出按钮，2-点击人工取走按钮
        /// </summary>
        int _informclickBtnIndex = -1;

        /// <summary>
        /// _conveyorLoadUnloadTimeoutInfoFormVariate:传送线上下料超时报警消息提示框界面
        /// </summary>
        InfoForm _conveyorLoadUnloadTimeoutInfoFormVariate = null;


        /// <summary>
        /// _conveyor1TimeoutAlarmDealWithWayIndexWhenLoadAndUnload:传送线1上下料时超时报警时解决方案索引，0-重试，1-忽略，2-无产品
        /// </summary>
        int _conveyor1TimeoutAlarmDealWithWayIndexWhenLoadAndUnload = -1;

        /// <summary>
        /// _conveyor2TimeoutAlarmDealWithWayIndexWhenLoadAndUnload:传送线2上下料时超时报警时解决方案索引，0-重试，1-忽略，2-无产品
        /// </summary>
        int _conveyor2TimeoutAlarmDealWithWayIndexWhenLoadAndUnload = -1;


        /// <summary>
        /// _conveyorHaveExcessProductInfoFormVariate:传送线中有多余产品报警提示框
        /// </summary>
        InfoForm _conveyorHaveExcessProductInfoFormVariate = null;

        /// <summary>
        /// _conveyor1HaveExcessProductAlarmDealWithWayIndex:传送线1上有多余产品报警处理方式索引，-1-空，0-人工取出，1-流出
        /// </summary>
        int _conveyor1HaveExcessProductAlarmDealWithWayIndex = -1;

        /// <summary>
        /// _conveyor2HaveExcessProductAlarmDealWithWayIndex:传送线2上有多余产品报警处理方式索引，-1-空，0-人工取出，1-流出
        /// </summary>
        int _conveyor2HaveExcessProductAlarmDealWithWayIndex = -1;

        #endregion

        //
        //
        //

        //构造函数
        public BaseForm()
        {
            //LogFileName
            MyTool.TxtFileProcess.CreateLog("启动程序", BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
            InitializeComponent();
            if (System.Diagnostics.Process.GetProcessesByName("3D AVI").ToList().Count > 1)//检测是否有多余的程序运行
            {
                MessageBox.Show("另一个程序运行中!");
                _closeSoftWithOutInfoFlag = true;
                MyTool.TxtFileProcess.CreateLog("当前有另一个程序运行中，停止当前程序", BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                Application.Exit();
                this.Dispose();
            }
            else
            {
                #region 启动初始化程序

                MyTool.TxtFileProcess.CreateLog("初始化参数！", BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

                tmrRefreshBottomLabel.Interval = 100;//设置刷新底部label控件用timer间隔为100ms
                tmrRefreshBottomLabel.Start();//启动

                //创建一个Thread，用于启动显示启动窗口
                Thread launchFormThread = new Thread(LaunchFormThreadFunc);
                launchFormThread.IsBackground = true;
                launchFormThread.Start();

                //创建并打开轴控制窗口
                MyTool.TxtFileProcess.CreateLog("实例化轴控制界面！", BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                _axisControlFormVariate = new AxisAndIOForm.AxisControlMainForm();
                _axisControlFormVariate.AxisControlMainFormRequestCloseEvent += new AxisControlMainFormRequestCloseDelegate(AxisControlFormRequestCloseEventFunc);
                _axisControlFormVariate.Hide();

                #region
                while (_axisControlFormVariate._axisMotionControlBoardVariate == null)
                { }
                _axisQuantity = AxisControlMainForm._axisQuantity;
                _maxIOQuantity = AxisControlMainForm._maxIOQuantity;
                _axisRealPosition = new float[_axisQuantity];//轴真实坐标
                _axisAbsolutePosition = new float[_axisQuantity];//轴绝对坐标
                _inputIOT = new T[_maxIOQuantity];//用于输入IO的计时T
                _outputIOT = new T[_maxIOQuantity];//用于输出IO的计时T
                #endregion

                //创建并隐藏basesettingform窗口
                MyTool.TxtFileProcess.CreateLog("实例化手动设置相关页面！", BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                _baseSettingFormVariate = new SettingBaseForm();
                _baseSettingFormVariate.TopLevel = false;
                pnlChildFormDisplay.Controls.Add(_baseSettingFormVariate);
                _baseSettingFormVariate.Hide();
                _baseSettingFormVariate.CloseSoftwareRequestEvent += new CloseSoftwareRequestDelegate(BaseSettingsFormRequestCloseSoftwareEventFunc);
                _baseSettingFormVariate.RequetChangeFormToMainFormEvent += new RequetChangeFormToMainFormDelegate(BaseSettingsFormRequestChangeFormEventFunc);
                _baseSettingFormVariate.RequestOpenEachAxisControlFormEvent += new RequestOpenEachAxisControlFormDelegate(BaseSettingsFormRequestOpenAxisControlFormEventFunc);
                _baseSettingFormVariate.SettingsBaseFormRequestXYMoveToAssignedPointEvent += new SettingsBaseFormRequestXYMoveToAssignedPointDelegate(SettingsBaseFormRequestXYMoveToAssignedPointEventFunc);
                _baseSettingFormVariate._maintenanceAxesControlFormVariate.MaintenanceAxesControlFormRequestChangeConveyorWidthEvent += MaintenanceAxesControlFormRequestChangeConveyorWidthEventFunc;
                _baseSettingFormVariate._maintenanceCylinderControlFormVariate.CylinderControlEvent += MaintenanceCylinderControlFormCylinderControlEventFunc;
                _baseSettingFormVariate._maintenanceAxesControlFormVariate.MaintenanceAxesControlFormRequestConveyorActionEvent += MaintenanceAxesControlFormRequestConveyorActionEventFunc;

                _baseSettingFormVariate._maintenanceCameraFormVariate.MaintenanceCameraFormRequestOpenOrCloseCameraEvent += MaintenanceCameraFormRequestOpenOrCloseCameraEventFunc;
                _baseSettingFormVariate._maintenanceCameraFormVariate.MaintenanceCameraFormRequestGrabImageAndDispEvent += MaintenanceCameraFormRequestGrabImageAndDispEventFunc;
                _baseSettingFormVariate._maintenanceCameraFormVariate.MaintenanceCameraFormRequestGrabImageAndDispVideoEvent += MaintenanceCameraFormRequestGrabImageAndDispVideoEventFunc;
                _baseSettingFormVariate._maintenanceCameraFormVariate.MaintenanceCameraFormRequestSaveImageEvent += MaintenanceCameraFormRequestSaveImageEventFunc;
                _baseSettingFormVariate._maintenanceCameraFormVariate.MaintenanceCameraFormRequestBackCameraScanBarcodeEvent += MaintenanceCameraFormRequestBackCameraScanBarcodeFunc;

                _baseSettingFormVariate._settingsCameraCalibrationVariate.SettingsCameraCalibrationFormRequestGrabImageAndDispEvent += MaintenanceCameraFormRequestGrabImageAndDispEventFunc;
                _baseSettingFormVariate._settingsCameraCalibrationVariate.SettingsCameraCalibrationFormRequestGrabImageAndDispVideoEvent += MaintenanceCameraFormRequestGrabImageAndDispVideoEventFunc;

                _baseSettingFormVariate._settingsProductFormVariate.SettingsProductFormRequestGrabImageAndDispEvent += MaintenanceCameraFormRequestGrabImageAndDispEventFunc;
                //设置参数读取数据
                _baseSettingFormVariate._settingsGeneralFormVariate.LoadingDataFromXmlFile();
                _baseSettingFormVariate._settingsAdminFormVariate.LoadingDataFromXmlFile();
                _baseSettingFormVariate._settingsProductFormVariate.MainFormDataLoadingSettingData();
                _baseSettingFormVariate._settingsCameraCalibrationVariate.LoadingDataFromXmlFile();

                //创建并打开mainform窗口
                MyTool.TxtFileProcess.CreateLog("实例化主界面！", BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                _mainFormVariate = new MainForm();
                _mainFormVariate.TopLevel = false;
                pnlChildFormDisplay.Controls.Add(_mainFormVariate);
                _mainFormVariate.Hide();
                _mainFormVariate.MainFormRequestCloseSoftwareEvent += new MainFormRequestCloseSoftwareDelegate(MainFormRequestCloseSoftwareEventFunc);
                _mainFormVariate.MainFormRequestChangeFormToMaintenanceEvent += new MainFormRequestChangeFormToMaintenanceDelegate(MainFormRequestChangeFormEventFunc);
                _mainFormVariate.MainFormChangeProductEvent += new MainFormChangeProductDelegate(MainFormChangeProductEventFunc);
                _mainFormVariate.MainFormClickVirtualButtonEvent += new MainFormClickVirtualButtonDelegate(MainFormClickVirtualButtonEventFunc);
                _mainFormVariate.MainFormConveyorForcedMoveOutEvent += MainFormConveyorForcedMoveOutEventFunc;

                _mainFormVariate.lblProgramVersion.Text = _softVersion;

                MyTool.TxtFileProcess.CreateLog("显示主界面！", BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                _mainFormVariate.Show();//显示主窗口

                _stateAndAlarmCs = new StateAndAlarmClass();//实例化状态及报警信息类

                //创建并打开Alarm窗口
                MyTool.TxtFileProcess.CreateLog("实例化报警窗口!", BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                _alarmFormVariate = new AlarmForm();
                _alarmFormVariate.AlarmFormClickAlarmRestButtonEvent += new AlarmFormClickAlarmResetButtonDelegate(MainFormClickVirtualButtonEventFunc);
                _alarmFormVariate.Hide();

                //基础页面状态显示框相关
                _baseFormStateOrAlarmMsg = "";
                _baseFormOldStateOrAlarmMsg = "";
                _baseFormStateStartIndex = 0;

                _stateAndAlarmDispTimer = new System.Threading.Timer(new System.Threading.TimerCallback(DeviceStateDisplayEvent), null, 100, 300);//设备状态及报警信息界面更新

                //创建一个Thread，用于计时，模拟PLC T寄存器的功能
                MyTool.TxtFileProcess.CreateLog("启动计时器线程！", BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                if (_timerUseInSoftCountThread != null)
                {
                    _timerUseInSoftCountThread.Abort();
                    _timerUseInSoftCountThread = null;
                }
                _timerUseInSoftCountThread = new Thread(TimerUseInSoftCountThreadFunc);
                _timerUseInSoftCountThread.IsBackground = true;
                _timerUseInSoftCountThread.Start();


                //创建一个Thread，用于设备动作
                MyTool.TxtFileProcess.CreateLog("启动设备动作线程！", BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                if (_deviceActionThread != null)
                {
                    _deviceActionThread.Abort();
                    _deviceActionThread = null;
                }
                _deviceActionThread = new Thread(DeviceActionThreadFunc);
                _deviceActionThread.IsBackground = true;
                _deviceActionThread.Start();

                //实例化模拟量控制变量
                MyTool.TxtFileProcess.CreateLog("实例化模拟量控制类！", BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                _newDAM02AIAOOperation = new DAM02AIAOOperation();
                if (!_newDAM02AIAOOperation.comm.IsOpen)
                {
                    if (!_newDAM02AIAOOperation.OpenSerialPort(SettingsGeneralForm._conveyorControlCom))
                    {
                        MessageBox.Show("打开串口失败");
                        MyTool.TxtFileProcess.CreateLog("打开串口失败！", BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                    }
                }

                //相机控制类实例化
                MyTool.TxtFileProcess.CreateLog("实例化相机控制类！", BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                _leftCameraCs = new HalconCameraControl();
                _rightCameraCs = new HalconCameraControl();
                _backCameraCs = new HalconCameraControl();
                if (_cameraImageGrabAndDispThread != null)
                {
                    _cameraImageGrabAndDispThread.Abort();
                    _cameraImageGrabAndDispThread = null;
                }
                _cameraImageGrabAndDispThread = new Thread(CameraImageGrabAndDispThreadFunc);
                _cameraImageGrabAndDispThread.IsBackground = true;
                _cameraImageGrabAndDispThread.Start();

                _productParameter[0] = new ProductRecipe();
                _productParameter[1] = new ProductRecipe();

                MyTool.TxtFileProcess.CreateLog("实例化MES通讯类！", BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                _newMesOperation = new MESOperation();//实力化MES操作类

                //产量显示datagridview初始化
                MyTool.TxtFileProcess.CreateLog("初始化产量显示控件！", BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                dgrdvYieldRecord.Rows.Add("检测PCS数");
                dgrdvYieldRecord.Rows.Add("PCS良品数");
                dgrdvYieldRecord.Rows.Add("PCS不良品数");
                dgrdvYieldRecord.Rows.Add("PCS良品率");
                dgrdvYieldRecord.RowTemplate.Height = 35;

                //设置最初设置项
                rbtnPcsYield.Checked = true;
                rbtnPanelYield.Checked = false;
                rbtnElementYield.Checked = false;

                //产量记录用DataTable初始化
                MyTool.TxtFileProcess.CreateLog("初始化产量记录DATATABLE", BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                if (_yieldRecordDt != null)
                {
                    _yieldRecordDt.Dispose();
                    _yieldRecordDt = null;
                }
                _yieldRecordDt = new DataTable();
                for (int i = 0; i < dgrdvYieldRecord.Columns.Count; i++)
                {
                    _yieldRecordDt.Columns.Add(dgrdvYieldRecord.Columns[i].Name.ToString(), typeof(string));
                }
                DataRow dr = _yieldRecordDt.NewRow();
                for (int i = 0; i < 12; i++)
                {
                    switch (i)
                    {
                        case 0:
                            dr[0] = "检测PCS数";
                            break;
                        case 1:
                            dr[0] = "PCS良品数";
                            break;
                        case 2:
                            dr[0] = "PCS不良品数";
                            break;
                        case 3:
                            dr[0] = "PCS良品率";
                            break;
                        case 4:
                            dr[0] = "检测Panel数";
                            break;
                        case 5:
                            dr[0] = "Panel良品数";
                            break;
                        case 6:
                            dr[0] = "Panel不良品数";
                            break;
                        case 7:
                            dr[0] = "Panel良品率";
                            break;
                        case 8:
                            dr[0] = "检测元件数";
                            break;
                        case 9:
                            dr[0] = "元件良品数";
                            break;
                        case 10:
                            dr[0] = "元件不良品数";
                            break;
                        case 11:
                            dr[0] = "元件良品率";
                            break;
                    }

                    dr[1] = "0";
                    dr[2] = "0";
                    dr[3] = "0";
                    _yieldRecordDt.Rows.Add(dr.ItemArray);
                }

                int currentTimeHour = Convert.ToInt32(System.DateTime.Now.TimeOfDay.ToString("hh"));
                int currentTimeMinutes = Convert.ToInt32(System.DateTime.Now.TimeOfDay.ToString("mm"));

                if ((currentTimeHour > SettingsGeneralForm._clearYieldTimeHour || 
                    (currentTimeHour == SettingsGeneralForm._clearYieldTimeHour && currentTimeMinutes > SettingsGeneralForm._clearYieldTimeMinutes))
                    && (currentTimeHour < SettingsGeneralForm._clearYieldTimeHour + 12 || 
                     (currentTimeHour == SettingsGeneralForm._clearYieldTimeHour + 12 && currentTimeMinutes <= SettingsGeneralForm._clearYieldTimeMinutes)))
                {
                    _yieldResetFlag = 1;//设置需要清除白班，当前处于白班，故而不清除
                }
                else
                {
                    _yieldResetFlag = 0;//设置需要清除晚班，当前处于晚班，故而不清除
                }

                LoadRecordYieldData();

                //用户登录编辑页面
                MyTool.TxtFileProcess.CreateLog("实例化用户管理页面！", BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
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

                //绑定委托事件，当添加了LOG信息至LOG文件后，触发对应事件
                MyTool.TxtFileProcess.AddMessageToLOGFileFinishEvent += AddMessageToLOGFileFinishEventFunc;

                //等待读取路径中的文件中的路径读取成功
                while (_launchFormVariate == null)
                { }

                //invoke，控制另一个线程中的LaunchFormVariate的关闭
                _launchFormVariate.Invoke(new Action(() =>
                {
                    _launchFormVariate.Close();
                })); ;

                //关闭启动窗口Thread
                if (launchFormThread != null)
                {
                    launchFormThread.Abort();
                    launchFormThread = null;
                }
                MyTool.TxtFileProcess.CreateLog("初始化参数完成！", BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                #endregion
            }
        }

        //启动弹出窗口的启用
        private void LaunchFormThreadFunc()
        {
            //创建并打开启动窗口
            _launchFormVariate = new LaunchForm();
            _launchFormVariate.TopLevel = true;
            _launchFormVariate.Focus();
            _launchFormVariate.TopMost = false;
            _launchFormVariate.DispInfoOnPicBox(0);
            _launchFormVariate.ShowDialog();
            _launchFormVariate = null;
        }

        //界面LOAD事件
        private void BaseForm_Load(object sender, EventArgs e)
        {
            //界面更新线程
            MyTool.TxtFileProcess.CreateLog("启动界面更新线程！", BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
            if (_formUpdateThread != null)
            {
                _formUpdateThread.Abort();
                _formUpdateThread = null;
            }
            _formUpdateThread = new Thread(FormUpdateThreadFunc);
            _formUpdateThread.IsBackground = true;
            _formUpdateThread.Start();

            this.WindowState = FormWindowState.Maximized;
        }

        //轴控制界面请求关闭事件函数
        private void AxisControlFormRequestCloseEventFunc()
        {
            MyTool.TxtFileProcess.CreateLog("轴手动控制界面请求关闭轴手动控制界面！" + "----用户：" + _operatorName, BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
            if (_axisControlFormVariate != null)
            {
                _axisControlFormVariate.Hide();
            }
        }

        #region FormUpdateThreadFunc:界面更新线程函数

        private void FormUpdateThreadFunc()
        {
            //用于控制运动控制卡显示框闪烁stopwatch初始化
            Stopwatch motionCardStatusTwinkleCountSW = null;//stopwatch，用于控制运动控制卡显示框闪烁
            if (motionCardStatusTwinkleCountSW != null)
            {
                motionCardStatusTwinkleCountSW.Stop();
                motionCardStatusTwinkleCountSW = null;
            }
            motionCardStatusTwinkleCountSW = new Stopwatch();

            //用于控制MES状态显示框闪烁stopwatch初始化
            Stopwatch MESStatusTwinkleCountSW = null;//stopwatch，用于控制MES状态显示框闪烁
            if (MESStatusTwinkleCountSW != null)
            {
                MESStatusTwinkleCountSW.Stop();
                MESStatusTwinkleCountSW = null;
            }
            MESStatusTwinkleCountSW = new Stopwatch();

            while (true)
            {
                #region 更新当前位置
                string tempRealPositionDispStr = "";
                string tempAbsPositionDispStr = "ABS: ";
                if (_axisControlFormVariate != null)
                {
                    for (int i = 0; i < _axisQuantity; i++)
                    {
                        _axisRealPosition[i] = Convert.ToSingle(_axisControlFormVariate._axisCurrentPosition_mm[i]);
                        _axisAbsolutePosition[i] = _axisRealPosition[i] - SettingsAdministratorForm._absoluteCoor[i];
                        if (_axisControlFormVariate._axisUseFlag[i])
                        {
                            switch (i)
                            {
                                case 0:
                                    tempRealPositionDispStr += " X1= " + _axisRealPosition[i];
                                    tempAbsPositionDispStr += " X1= " + _axisAbsolutePosition[i];
                                    break;
                                case 1:
                                    tempRealPositionDispStr += " Y1= " + _axisRealPosition[i];
                                    tempAbsPositionDispStr += " Y1= " + _axisAbsolutePosition[i];
                                    break;
                                case 2:
                                    tempRealPositionDispStr += " R1= " + _axisRealPosition[i];
                                    tempAbsPositionDispStr += " R1= " + _axisAbsolutePosition[i];
                                    break;
                                case 3:
                                    tempRealPositionDispStr += " Y2= " + _axisRealPosition[i];
                                    tempAbsPositionDispStr += " Y2= " + _axisAbsolutePosition[i];
                                    break;
                                case 4:
                                    tempRealPositionDispStr += " Y3= " + _axisRealPosition[i];
                                    tempAbsPositionDispStr += " Y3= " + _axisAbsolutePosition[i];
                                    break;
                                case 5:
                                    tempRealPositionDispStr += " Y4= " + _axisRealPosition[i];
                                    tempAbsPositionDispStr += " Y4= " + _axisAbsolutePosition[i];
                                    break;
                                case 6:
                                    tempRealPositionDispStr += " Y5= " + _axisRealPosition[i];
                                    tempAbsPositionDispStr += " Y5= " + _axisAbsolutePosition[i];
                                    break;
                            }
                        }
                    }
                }
                lblAxisCoordination.Invoke(new Action(() =>
                {
                    lblAxisCoordination.Text = tempRealPositionDispStr;// +"; " + tempAbsPositionDispStr;

                })); ;
                #endregion

                #region 实时刷新运动控制卡状态
                if (_axisControlFormVariate != null)
                {
                    if (_axisControlFormVariate._motionCardInitialStatus)
                    {
                        if (motionCardStatusTwinkleCountSW.IsRunning)
                        {
                            motionCardStatusTwinkleCountSW.Stop();
                        }
                        lblMotionCardStatus.Invoke(new Action(() =>
                            {
                                lblMotionCardStatus.Text = "运动控制卡正常";
                                lblMotionCardStatus.ForeColor = Color.LimeGreen;
                                lblMotionCardStatus.BackColor = SystemColors.Control;
                            })); ;
                    }
                }
                if (_axisControlFormVariate == null || (_axisControlFormVariate != null && !_axisControlFormVariate._motionCardInitialStatus))
                {
                    if (!motionCardStatusTwinkleCountSW.IsRunning)
                    {
                        motionCardStatusTwinkleCountSW.Start();
                    }
                    if (motionCardStatusTwinkleCountSW.ElapsedMilliseconds < 1000)
                    {
                        lblMotionCardStatus.Invoke(new Action(() =>
                        {
                            lblMotionCardStatus.Text = "运动控制卡异常";
                            lblMotionCardStatus.ForeColor = Color.Red;
                            lblMotionCardStatus.BackColor = SystemColors.Control;
                        })); ;
                    }
                    else if (motionCardStatusTwinkleCountSW.ElapsedMilliseconds >= 1000 && motionCardStatusTwinkleCountSW.ElapsedMilliseconds < 2000)
                    {
                        lblMotionCardStatus.Invoke(new Action(() =>
                        {
                            lblMotionCardStatus.Text = "运动控制卡异常";
                            lblMotionCardStatus.ForeColor = Color.White;
                            lblMotionCardStatus.BackColor = Color.Red;
                        })); ;
                    }
                    else if (motionCardStatusTwinkleCountSW.ElapsedMilliseconds >= 2000)
                    {
                        motionCardStatusTwinkleCountSW.Restart();
                    }
                }
                #endregion

                #region 实时刷新MES通讯状态
                if (_baseSettingFormVariate._settingsGeneralFormVariate != null)
                {
                    if (!_baseSettingFormVariate._settingsGeneralFormVariate.chkShieldMES.Checked)
                    {
                        if (MESStatusTwinkleCountSW.IsRunning)
                        {
                            MESStatusTwinkleCountSW.Stop();
                        }

                        lblMESStatus.Invoke(new Action(() =>
                        {
                            lblMESStatus.Text = "MES通讯正常";
                            lblMESStatus.ForeColor = Color.LimeGreen;
                            lblMESStatus.BackColor = SystemColors.Control;
                        })); ;
                    }
                    else
                    {
                        if (!MESStatusTwinkleCountSW.IsRunning)
                        {
                            MESStatusTwinkleCountSW.Start();
                        }
                        if (MESStatusTwinkleCountSW.ElapsedMilliseconds < 1000)
                        {
                            lblMESStatus.Invoke(new Action(() =>
                            {
                                lblMESStatus.Text = "MES通讯屏蔽";
                                lblMESStatus.ForeColor = Color.Red;
                                lblMESStatus.BackColor = SystemColors.Control;
                            })); ;
                        }
                        else if (MESStatusTwinkleCountSW.ElapsedMilliseconds >= 1000 && MESStatusTwinkleCountSW.ElapsedMilliseconds < 2000)
                        {
                            lblMESStatus.Invoke(new Action(() =>
                            {
                                lblMESStatus.Text = "MES通讯屏蔽";
                                lblMESStatus.ForeColor = Color.White;
                                lblMESStatus.BackColor = Color.Red;
                            })); ;
                        }
                        else if (MESStatusTwinkleCountSW.ElapsedMilliseconds >= 2000)
                        {
                            MESStatusTwinkleCountSW.Restart();
                        }
                    }
                }

                #endregion

                #region 与运动控制卡交互数据
                #region 传输给运动控制卡数据
                _axisControlFormVariate._deviceAutoModeFlag = _autoModeFlag;//传递当前模式
                _axisControlFormVariate._useRelativeFlag = SettingsProductForm._useRelativePos;//传递使用相对位置与否标志
                for (int i = 0; i < _axisQuantity; i++)
                    _axisControlFormVariate._absoluteCoor[i] = SettingsAdministratorForm._absoluteCoor[i];//传递绝对补偿坐标

                if (!_inputIOT[10].STATUS || !_inputIOT[11].STATUS)//当安全门打开时
                {
                    for (int i = 0; i < _axisQuantity; i++)
                        _axisControlFormVariate._axisActionSafeFlag[i] = true;
                }
                else
                {
                    for (int i = 0; i < _axisQuantity; i++)
                        _axisControlFormVariate._axisActionSafeFlag[i] = true;
                }
                #endregion
                #endregion

                #region 报警Flag更新Module
                AlarmRefreshModule();
                #endregion

                #region 更新闪烁Flag
                if (_device500TwinkleOnT.STATUS)
                    _device500TwinkeOnOffFlag = false;
                if (_device500TwinkleOffT.STATUS)
                    _device500TwinkeOnOffFlag = true;

                if (_device200TwinkleOnT.STATUS)
                    _device200TwinkeOnOffFlag = false;
                if (_device200TwinkleOffT.STATUS)
                    _device200TwinkeOnOffFlag = true;
                #endregion

                #region 更新自动手动按钮显示
                this.Invoke(new Action(() =>
                {
                    if (_autoModeFlag)
                    {
                        _mainFormVariate.btnAuto.BackColor = Color.LimeGreen;

                        _mainFormVariate.btnManual.BackColor = SystemColors.Control;
                    }
                    else
                    {
                        _mainFormVariate.btnAuto.BackColor = SystemColors.Control;

                        _mainFormVariate.btnManual.BackColor = Color.LimeGreen;
                    }
                })); ;
                #endregion

                #region 更新虚拟启动，停止，复位，测量启动按钮显示
                this.Invoke(new Action(() =>
                    {
                        //启动按钮
                        switch (_startBtnLampOnOffFlag)
                        {
                            case 0://如果启动按钮不亮
                                _mainFormVariate.btnStart.BackColor = SystemColors.Control;
                                if (_axisControlFormVariate != null)
                                {
                                    _axisControlFormVariate.OutputControl(START_BT_LAMP_OUT_IO_NO, false);
                                }
                                break;
                            case 1://如果启动按钮常亮
                                _mainFormVariate.btnStart.BackColor = Color.LimeGreen;
                                if (_axisControlFormVariate != null)
                                {
                                    _axisControlFormVariate.OutputControl(START_BT_LAMP_OUT_IO_NO, true);
                                }
                                break;
                            case 2://如果为间隔500ms闪烁
                                if (_device500TwinkeOnOffFlag)//500ms闪烁标志为ON
                                {
                                    _mainFormVariate.btnStart.BackColor = Color.LimeGreen;
                                    if (_axisControlFormVariate != null)
                                    {
                                        _axisControlFormVariate.OutputControl(START_BT_LAMP_OUT_IO_NO, true);
                                    }
                                }
                                else
                                {
                                    _mainFormVariate.btnStart.BackColor = SystemColors.Control;
                                    if (_axisControlFormVariate != null)
                                    {
                                        _axisControlFormVariate.OutputControl(START_BT_LAMP_OUT_IO_NO, false);
                                    }
                                }
                                break;
                            case 3://如果为间隔200ms闪烁
                                if (_device200TwinkeOnOffFlag)//500ms闪烁标志为ON
                                {
                                    _mainFormVariate.btnStart.BackColor = Color.LimeGreen;
                                    if (_axisControlFormVariate != null)
                                    {
                                        _axisControlFormVariate.OutputControl(START_BT_LAMP_OUT_IO_NO, true);
                                    }
                                }
                                else
                                {
                                    _mainFormVariate.btnStart.BackColor = SystemColors.Control;
                                    if (_axisControlFormVariate != null)
                                    {
                                        _axisControlFormVariate.OutputControl(START_BT_LAMP_OUT_IO_NO, false);
                                    }
                                }
                                break;
                        }

                        //停止按钮
                        switch (_stopBtnLampOnOffFlag)
                        {
                            case 0://如果停止按钮不亮
                                _mainFormVariate.btnStop.BackColor = SystemColors.Control;
                                if (_axisControlFormVariate != null)
                                {
                                    _axisControlFormVariate.OutputControl(STOP_BT_LAMP_OUT_IO_NO, false);
                                }
                                break;
                            case 1://如果停止按钮常亮
                                _mainFormVariate.btnStop.BackColor = Color.Red;
                                if (_axisControlFormVariate != null)
                                {
                                    _axisControlFormVariate.OutputControl(STOP_BT_LAMP_OUT_IO_NO, true);
                                }
                                break;
                            case 2://如果为间隔500ms闪烁
                                if (_device500TwinkeOnOffFlag)//500ms闪烁标志为ON
                                {
                                    _mainFormVariate.btnStop.BackColor = Color.Red;
                                    if (_axisControlFormVariate != null)
                                    {
                                        _axisControlFormVariate.OutputControl(STOP_BT_LAMP_OUT_IO_NO, true);
                                    }
                                }
                                else
                                {
                                    _mainFormVariate.btnStop.BackColor = SystemColors.Control;
                                    if (_axisControlFormVariate != null)
                                    {
                                        _axisControlFormVariate.OutputControl(STOP_BT_LAMP_OUT_IO_NO, false);
                                    }
                                }
                                break;
                            case 3://如果为间隔200ms闪烁
                                if (_device200TwinkeOnOffFlag)//500ms闪烁标志为ON
                                {
                                    _mainFormVariate.btnStop.BackColor = Color.Red;
                                    if (_axisControlFormVariate != null)
                                    {
                                        _axisControlFormVariate.OutputControl(STOP_BT_LAMP_OUT_IO_NO, true);
                                    }
                                }
                                else
                                {
                                    _mainFormVariate.btnStop.BackColor = SystemColors.Control;
                                    if (_axisControlFormVariate != null)
                                    {
                                        _axisControlFormVariate.OutputControl(STOP_BT_LAMP_OUT_IO_NO, false);
                                    }
                                }
                                break;
                        }

                        //复位按钮
                        switch (_resetBtnLampOnOffFlag)
                        {
                            case 0://如果复位按钮不亮
                                _mainFormVariate.btnReset.BackColor = SystemColors.Control;
                                if (_axisControlFormVariate != null)
                                {
                                    _axisControlFormVariate.OutputControl(RESET_BT_LAMP_OUT_IO_NO, false);
                                }
                                break;
                            case 1://如果复位按钮常亮
                                _mainFormVariate.btnReset.BackColor = Color.Orange;
                                if (_axisControlFormVariate != null)
                                {
                                    _axisControlFormVariate.OutputControl(RESET_BT_LAMP_OUT_IO_NO, true);
                                }
                                break;
                            case 2://如果为间隔500ms闪烁
                                if (_device500TwinkeOnOffFlag)//500ms闪烁标志为ON
                                {
                                    _mainFormVariate.btnReset.BackColor = Color.Orange;
                                    if (_axisControlFormVariate != null)
                                    {
                                        _axisControlFormVariate.OutputControl(RESET_BT_LAMP_OUT_IO_NO, true);
                                    }
                                }
                                else
                                {
                                    _mainFormVariate.btnReset.BackColor = SystemColors.Control;
                                    if (_axisControlFormVariate != null)
                                    {
                                        _axisControlFormVariate.OutputControl(RESET_BT_LAMP_OUT_IO_NO, false);
                                    }
                                }
                                break;
                            case 3://如果为间隔200ms闪烁
                                if (_device200TwinkeOnOffFlag)//500ms闪烁标志为ON
                                {
                                    _mainFormVariate.btnReset.BackColor = Color.Orange;
                                    if (_axisControlFormVariate != null)
                                    {
                                        _axisControlFormVariate.OutputControl(RESET_BT_LAMP_OUT_IO_NO, true);
                                    }
                                }
                                else
                                {
                                    _mainFormVariate.btnReset.BackColor = SystemColors.Control;
                                    if (_axisControlFormVariate != null)
                                    {
                                        _axisControlFormVariate.OutputControl(RESET_BT_LAMP_OUT_IO_NO, false);
                                    }
                                }
                                break;
                        }

                        //测量启动按钮
                        switch (_measureStartBtnLampOnOffFlag)
                        {
                            case 0://如果测量启动按钮不亮
                                _mainFormVariate.btnMeasureStart.BackColor = SystemColors.Control;
                                if (_axisControlFormVariate != null)
                                {
                                    _axisControlFormVariate.OutputControl(MEASURE_START_BT_LAMP_OUT_IO_NO, false);
                                }
                                break;
                            case 1://如果测量启动按钮常亮
                                _mainFormVariate.btnMeasureStart.BackColor = Color.LimeGreen;
                                if (_axisControlFormVariate != null)
                                {
                                    _axisControlFormVariate.OutputControl(MEASURE_START_BT_LAMP_OUT_IO_NO, true);
                                }
                                break;
                            case 2://如果为间隔500ms闪烁
                                if (_device500TwinkeOnOffFlag)//500ms闪烁标志为ON
                                {
                                    _mainFormVariate.btnMeasureStart.BackColor = Color.LimeGreen;
                                    if (_axisControlFormVariate != null)
                                    {
                                        _axisControlFormVariate.OutputControl(MEASURE_START_BT_LAMP_OUT_IO_NO, true);
                                    }
                                }
                                else
                                {
                                    _mainFormVariate.btnMeasureStart.BackColor = SystemColors.Control;
                                    if (_axisControlFormVariate != null)
                                    {
                                        _axisControlFormVariate.OutputControl(MEASURE_START_BT_LAMP_OUT_IO_NO, false);
                                    }
                                }
                                break;
                            case 3://如果为间隔200ms闪烁
                                if (_device200TwinkeOnOffFlag)//500ms闪烁标志为ON
                                {
                                    _mainFormVariate.btnMeasureStart.BackColor = Color.LimeGreen;
                                    if (_axisControlFormVariate != null)
                                    {
                                        _axisControlFormVariate.OutputControl(MEASURE_START_BT_LAMP_OUT_IO_NO, true);
                                    }
                                }
                                else
                                {
                                    _mainFormVariate.btnMeasureStart.BackColor = SystemColors.Control;
                                    if (_axisControlFormVariate != null)
                                    {
                                        _axisControlFormVariate.OutputControl(MEASURE_START_BT_LAMP_OUT_IO_NO, false);
                                    }
                                }
                                break;
                        }
                    })); ;
                #endregion

                #region 更新三色灯，ON,OFF,闪烁等
                if (_axisControlFormVariate != null)
                {
                    //三色灯红灯
                    switch (_towerLampRedOnOffFlag)
                    {
                        case 0://如果启动按钮不亮
                            _axisControlFormVariate.OutputControl(TOWER_LAMP_RED_OUT_IO_NO, false);
                            break;
                        case 1://如果启动按钮常亮
                            _axisControlFormVariate.OutputControl(TOWER_LAMP_RED_OUT_IO_NO, true);
                            break;
                        case 2://如果为间隔500ms闪烁
                            if (_device500TwinkeOnOffFlag)//500ms闪烁标志为ON
                                _axisControlFormVariate.OutputControl(TOWER_LAMP_RED_OUT_IO_NO, true);
                            else
                                _axisControlFormVariate.OutputControl(TOWER_LAMP_RED_OUT_IO_NO, false);
                            break;
                        case 3://如果为间隔200ms闪烁
                            if (_device200TwinkeOnOffFlag)//500ms闪烁标志为ON
                                _axisControlFormVariate.OutputControl(TOWER_LAMP_RED_OUT_IO_NO, true);
                            else
                                _axisControlFormVariate.OutputControl(TOWER_LAMP_RED_OUT_IO_NO, false);
                            break;
                    }

                    //三色灯绿灯
                    switch (_towerLampGreenOnOffFlag)
                    {
                        case 0://如果启动按钮不亮
                            _axisControlFormVariate.OutputControl(TOWER_LAMP_GREEN_OUT_IO_NO, false);
                            break;
                        case 1://如果启动按钮常亮
                            _axisControlFormVariate.OutputControl(TOWER_LAMP_GREEN_OUT_IO_NO, true);
                            break;
                        case 2://如果为间隔500ms闪烁
                            if (_device500TwinkeOnOffFlag)//500ms闪烁标志为ON
                                _axisControlFormVariate.OutputControl(TOWER_LAMP_GREEN_OUT_IO_NO, true);
                            else
                                _axisControlFormVariate.OutputControl(TOWER_LAMP_GREEN_OUT_IO_NO, false);
                            break;
                        case 3://如果为间隔200ms闪烁
                            if (_device200TwinkeOnOffFlag)//500ms闪烁标志为ON
                                _axisControlFormVariate.OutputControl(TOWER_LAMP_GREEN_OUT_IO_NO, true);
                            else
                                _axisControlFormVariate.OutputControl(TOWER_LAMP_GREEN_OUT_IO_NO, false);
                            break;
                    }

                    //三色灯黄灯
                    switch (_towerLampYellowOnOffFlag)
                    {
                        case 0://如果启动按钮不亮
                            _axisControlFormVariate.OutputControl(TOWER_LAMP_YELLOW_OUT_IO_NO, false);
                            break;
                        case 1://如果启动按钮常亮
                            _axisControlFormVariate.OutputControl(TOWER_LAMP_YELLOW_OUT_IO_NO, true);
                            break;
                        case 2://如果为间隔500ms闪烁
                            if (_device500TwinkeOnOffFlag)//500ms闪烁标志为ON
                                _axisControlFormVariate.OutputControl(TOWER_LAMP_YELLOW_OUT_IO_NO, true);
                            else
                                _axisControlFormVariate.OutputControl(TOWER_LAMP_YELLOW_OUT_IO_NO, false);
                            break;
                        case 3://如果为间隔200ms闪烁
                            if (_device200TwinkeOnOffFlag)//500ms闪烁标志为ON
                                _axisControlFormVariate.OutputControl(TOWER_LAMP_YELLOW_OUT_IO_NO, true);
                            else
                                _axisControlFormVariate.OutputControl(TOWER_LAMP_YELLOW_OUT_IO_NO, false);
                            break;
                    }

                    //三色灯蜂鸣器
                    switch (_towerLampBuzzerOnOffFlag)
                    {
                        case 0://如果启动按钮不亮
                            _axisControlFormVariate.OutputControl(TOWER_LAMP_BUZZER_OUT_IO_NO, false);
                            break;
                        case 1://如果启动按钮常亮
                            _axisControlFormVariate.OutputControl(TOWER_LAMP_BUZZER_OUT_IO_NO, true);
                            break;
                        case 2://如果为间隔500ms闪烁
                            if (_device500TwinkeOnOffFlag)//500ms闪烁标志为ON
                                _axisControlFormVariate.OutputControl(TOWER_LAMP_BUZZER_OUT_IO_NO, true);
                            else
                                _axisControlFormVariate.OutputControl(TOWER_LAMP_BUZZER_OUT_IO_NO, false);
                            break;
                        case 3://如果为间隔200ms闪烁
                            if (_device200TwinkeOnOffFlag)//500ms闪烁标志为ON
                                _axisControlFormVariate.OutputControl(TOWER_LAMP_BUZZER_OUT_IO_NO, true);
                            else
                                _axisControlFormVariate.OutputControl(TOWER_LAMP_BUZZER_OUT_IO_NO, false);
                            break;
                    }
                }
                #endregion

                #region 相机页面按钮状态更新
                if (_baseSettingFormVariate != null)
                {
                    if (_baseSettingFormVariate._maintenanceCameraFormVariate != null)
                    {
                        _baseSettingFormVariate._maintenanceCameraFormVariate.Invoke(new Action(() =>
                        {
                            //左相机按钮
                            if (_leftCameraCs.hv_AcqHandle != null)
                            {
                                if (_baseSettingFormVariate._maintenanceCameraFormVariate.btnLeftCameraOpen.Text != "左相机关闭")
                                {
                                    _baseSettingFormVariate._maintenanceCameraFormVariate.btnLeftCameraOpen.Text = "左相机关闭";
                                    _baseSettingFormVariate._maintenanceCameraFormVariate.btnLeftCameraOpen.BackColor = Color.Green;
                                    _baseSettingFormVariate._maintenanceCameraFormVariate.btnLeftCameraOpen.ForeColor = Color.Black;
                                }
                            }
                            else
                            {
                                if (_baseSettingFormVariate._maintenanceCameraFormVariate.btnLeftCameraOpen.Text != "左相机打开")
                                {
                                    _baseSettingFormVariate._maintenanceCameraFormVariate.btnLeftCameraOpen.Text = "左相机打开";
                                    _baseSettingFormVariate._maintenanceCameraFormVariate.btnLeftCameraOpen.BackColor = SystemColors.ControlDark;
                                    _baseSettingFormVariate._maintenanceCameraFormVariate.btnLeftCameraOpen.ForeColor = Color.White;
                                }
                            }

                            if (_leftCameraVideo)
                            {
                                if (_baseSettingFormVariate._maintenanceCameraFormVariate.btnLeftCameraVideo.BackColor != Color.Green)
                                {
                                    _baseSettingFormVariate._maintenanceCameraFormVariate.btnLeftCameraVideo.BackColor = Color.Green;
                                    _baseSettingFormVariate._maintenanceCameraFormVariate.btnLeftCameraVideo.ForeColor = Color.Black;
                                }
                            }
                            else
                            {
                                if (_baseSettingFormVariate._maintenanceCameraFormVariate.btnLeftCameraVideo.BackColor != SystemColors.ControlDark)
                                {
                                    _baseSettingFormVariate._maintenanceCameraFormVariate.btnLeftCameraVideo.BackColor = SystemColors.ControlDark;
                                    _baseSettingFormVariate._maintenanceCameraFormVariate.btnLeftCameraVideo.ForeColor = Color.White;
                                }
                            }

                            //右相机按钮
                            if (_rightCameraCs.hv_AcqHandle != null)
                            {
                                if (_baseSettingFormVariate._maintenanceCameraFormVariate.btnRightCameraOpen.Text != "右相机关闭")
                                {
                                    _baseSettingFormVariate._maintenanceCameraFormVariate.btnRightCameraOpen.Text = "右相机关闭";
                                    _baseSettingFormVariate._maintenanceCameraFormVariate.btnRightCameraOpen.BackColor = Color.Green;
                                    _baseSettingFormVariate._maintenanceCameraFormVariate.btnRightCameraOpen.ForeColor = Color.Black;
                                }
                            }
                            else
                            {
                                if (_baseSettingFormVariate._maintenanceCameraFormVariate.btnRightCameraOpen.Text != "右相机打开")
                                {
                                    _baseSettingFormVariate._maintenanceCameraFormVariate.btnRightCameraOpen.Text = "右相机打开";
                                    _baseSettingFormVariate._maintenanceCameraFormVariate.btnRightCameraOpen.BackColor = SystemColors.ControlDark;
                                    _baseSettingFormVariate._maintenanceCameraFormVariate.btnRightCameraOpen.ForeColor = Color.White;
                                }
                            }

                            if (_rightCameraVideo)
                            {
                                if (_baseSettingFormVariate._maintenanceCameraFormVariate.btnRightCameraVideo.BackColor != Color.Green)
                                {
                                    _baseSettingFormVariate._maintenanceCameraFormVariate.btnRightCameraVideo.BackColor = Color.Green;
                                    _baseSettingFormVariate._maintenanceCameraFormVariate.btnRightCameraVideo.ForeColor = Color.Black;
                                }
                            }
                            else
                            {
                                if (_baseSettingFormVariate._maintenanceCameraFormVariate.btnRightCameraVideo.BackColor != SystemColors.ControlDark)
                                {
                                    _baseSettingFormVariate._maintenanceCameraFormVariate.btnRightCameraVideo.BackColor = SystemColors.ControlDark;
                                    _baseSettingFormVariate._maintenanceCameraFormVariate.btnRightCameraVideo.ForeColor = Color.White;
                                }
                            }

                            //后侧相机
                            if (_backCameraCs.hv_AcqHandle != null)
                            {
                                if (_baseSettingFormVariate._maintenanceCameraFormVariate.btnBackCameraOpen.Text != "后相机关闭")
                                {
                                    _baseSettingFormVariate._maintenanceCameraFormVariate.btnBackCameraOpen.Text = "后相机关闭";
                                    _baseSettingFormVariate._maintenanceCameraFormVariate.btnBackCameraOpen.BackColor = Color.Green;
                                    _baseSettingFormVariate._maintenanceCameraFormVariate.btnBackCameraOpen.ForeColor = Color.Black;
                                }
                            }
                            else
                            {
                                if (_baseSettingFormVariate._maintenanceCameraFormVariate.btnBackCameraOpen.Text != "后相机打开")
                                {
                                    _baseSettingFormVariate._maintenanceCameraFormVariate.btnBackCameraOpen.Text = "后相机打开";
                                    _baseSettingFormVariate._maintenanceCameraFormVariate.btnBackCameraOpen.BackColor = SystemColors.ControlDark;
                                    _baseSettingFormVariate._maintenanceCameraFormVariate.btnBackCameraOpen.ForeColor = Color.White;
                                }
                            }

                            if (_backCameraVideo)
                            {
                                if (_baseSettingFormVariate._maintenanceCameraFormVariate.btnBackCameraVideo.BackColor != Color.Green)
                                {
                                    _baseSettingFormVariate._maintenanceCameraFormVariate.btnBackCameraVideo.BackColor = Color.Green;
                                    _baseSettingFormVariate._maintenanceCameraFormVariate.btnBackCameraVideo.ForeColor = Color.Black;
                                }
                            }
                            else
                            {
                                if (_baseSettingFormVariate._maintenanceCameraFormVariate.btnBackCameraVideo.BackColor != SystemColors.ControlDark)
                                {
                                    _baseSettingFormVariate._maintenanceCameraFormVariate.btnBackCameraVideo.BackColor = SystemColors.ControlDark;
                                    _baseSettingFormVariate._maintenanceCameraFormVariate.btnBackCameraVideo.ForeColor = Color.White;
                                }
                            }
                        })); ;
                    }

                    if (_baseSettingFormVariate._settingsCameraCalibrationVariate != null)
                    {
                        _baseSettingFormVariate._settingsCameraCalibrationVariate.Invoke(new Action(() =>
                            {
                                if (_backCameraVideo)
                                {
                                    if (_baseSettingFormVariate._settingsCameraCalibrationVariate.btnStartStopRealTimeVideo.BackColor != Color.Green)
                                    {
                                        _baseSettingFormVariate._settingsCameraCalibrationVariate.btnStartStopRealTimeVideo.BackColor = Color.Green;
                                        _baseSettingFormVariate._settingsCameraCalibrationVariate.btnStartStopRealTimeVideo.ForeColor = Color.Black;
                                    }
                                }
                                else
                                {
                                    if (_baseSettingFormVariate._settingsCameraCalibrationVariate.btnStartStopRealTimeVideo.BackColor != SystemColors.ControlDark)
                                    {
                                        _baseSettingFormVariate._settingsCameraCalibrationVariate.btnStartStopRealTimeVideo.BackColor = SystemColors.ControlDark;
                                        _baseSettingFormVariate._settingsCameraCalibrationVariate.btnStartStopRealTimeVideo.ForeColor = Color.White;
                                    }
                                }
                            })); ;
                    }
                }
                #endregion

                #region 实时显示测试结果
                if (_autoModeFlag && _conveyor1Status != 0)// && _conveyor1Status > 100 && _conveyor1Status < 106
                {
                    dgrdvConveyor1InferResultDisp.Invoke(new Action(() =>
                    {
                        Dictionary<int, InferResult> tempConveyorInferResultDic = null;
                        lock (_conveyor1InferResultDic)
                        {
                            tempConveyorInferResultDic = new Dictionary<int, InferResult>(_conveyor1InferResultDic);
                        }

                        //传送线1
                        if (tempConveyorInferResultDic.Count > 0)
                        {
                            DataTable dt = new DataTable();
                            for (int i = 0; i < dgrdvConveyor1InferResultDisp.Columns.Count; i++)
                            {
                                dt.Columns.Add(i.ToString(), typeof(string));
                            }
                            DataRow dr = dt.NewRow();
                            DataRow dgrdvDr = dt.NewRow();
                            for (int i = 0; i < tempConveyorInferResultDic.Count; i++)
                            {
                                dr[0] = tempConveyorInferResultDic[i]._elementNumber;
                                dr[1] = tempConveyorInferResultDic[i]._elementName;
                                dr[2] = tempConveyorInferResultDic[i]._elementLeftResult;
                                dr[3] = tempConveyorInferResultDic[i]._elementRightResult;
                                dr[4] = tempConveyorInferResultDic[i]._elementTotalResult;

                                if (dgrdvConveyor1InferResultDisp.Rows.Count > i)
                                {
                                    dgrdvDr[0] = dgrdvConveyor1InferResultDisp.Rows[i].Cells[0].Value;
                                    dgrdvDr[1] = dgrdvConveyor1InferResultDisp.Rows[i].Cells[1].Value;
                                    dgrdvDr[2] = dgrdvConveyor1InferResultDisp.Rows[i].Cells[2].Value;
                                    dgrdvDr[3] = dgrdvConveyor1InferResultDisp.Rows[i].Cells[3].Value;
                                    dgrdvDr[4] = dgrdvConveyor1InferResultDisp.Rows[i].Cells[4].Value;

                                    if (dgrdvDr[0] != dr[0] || dgrdvDr[1] != dr[1] || dgrdvDr[2] != dr[2] || dgrdvDr[3] != dr[3] || dgrdvDr[4] != dr[4])//如果数据不相同
                                    {
                                        dgrdvConveyor1InferResultDisp.Rows[i].SetValues(dr.ItemArray);
                                        if (dr[4].ToString() == "PASS")
                                        {
                                            dgrdvConveyor1InferResultDisp.Rows[i].DefaultCellStyle.ForeColor = Color.LimeGreen;
                                        }
                                        else if (dr[4].ToString() == "FAIL")
                                        {
                                            dgrdvConveyor1InferResultDisp.Rows[i].DefaultCellStyle.ForeColor = Color.Red;
                                        }
                                    }
                                }
                                else
                                {
                                    dgrdvConveyor1InferResultDisp.Rows.Add(dr.ItemArray);
                                    if (dr[4].ToString() == "PASS")
                                    {
                                        dgrdvConveyor1InferResultDisp.Rows[i].DefaultCellStyle.ForeColor = Color.LimeGreen;
                                    }
                                    else if (dr[4].ToString() == "FAIL")
                                    {
                                        dgrdvConveyor1InferResultDisp.Rows[i].DefaultCellStyle.ForeColor = Color.Red;
                                    }
                                }
                                // dt.Rows.Add(dr.ItemArray);
                            }
                        }
                    })); ;
                }

                if (_autoModeFlag && _conveyor2Status != 0)//_conveyor2Status > 100 && _conveyor2Status < 106
                {
                    dgrdvConveyor2InferResultDisp.Invoke(new Action(() =>
                    {
                        Dictionary<int, InferResult> tempConveyorInferResultDic = null;
                        lock (_conveyor2InferResultDic)
                        {
                            tempConveyorInferResultDic = new Dictionary<int, InferResult>(_conveyor2InferResultDic);
                        }

                        //传送线1
                        if (tempConveyorInferResultDic.Count > 0)
                        {
                            DataTable dt = new DataTable();
                            for (int i = 0; i < dgrdvConveyor2InferResultDisp.Columns.Count; i++)
                            {
                                dt.Columns.Add(i.ToString(), typeof(string));
                            }
                            DataRow dr = dt.NewRow();
                            DataRow dgrdvDr = dt.NewRow();
                            for (int i = 0; i < tempConveyorInferResultDic.Count; i++)
                            {
                                dr[0] = tempConveyorInferResultDic[i]._elementNumber;
                                dr[1] = tempConveyorInferResultDic[i]._elementName;
                                dr[2] = tempConveyorInferResultDic[i]._elementLeftResult;
                                dr[3] = tempConveyorInferResultDic[i]._elementRightResult;
                                dr[4] = tempConveyorInferResultDic[i]._elementTotalResult;

                                if (dgrdvConveyor2InferResultDisp.Rows.Count > i)
                                {
                                    dgrdvDr[0] = dgrdvConveyor2InferResultDisp.Rows[i].Cells[0].Value;
                                    dgrdvDr[1] = dgrdvConveyor2InferResultDisp.Rows[i].Cells[1].Value;
                                    dgrdvDr[2] = dgrdvConveyor2InferResultDisp.Rows[i].Cells[2].Value;
                                    dgrdvDr[3] = dgrdvConveyor2InferResultDisp.Rows[i].Cells[3].Value;
                                    dgrdvDr[4] = dgrdvConveyor2InferResultDisp.Rows[i].Cells[4].Value;

                                    if (dgrdvDr[0] != dr[0] || dgrdvDr[1] != dr[1] || dgrdvDr[2] != dr[2] || dgrdvDr[3] != dr[3] || dgrdvDr[4] != dr[4])//如果数据不相同
                                    {
                                        dgrdvConveyor2InferResultDisp.Rows[i].SetValues(dr.ItemArray);
                                        if (dr[4].ToString() == "PASS")
                                        {
                                            dgrdvConveyor2InferResultDisp.Rows[i].DefaultCellStyle.ForeColor = Color.LimeGreen;
                                        }
                                        else if (dr[4].ToString() == "FAIL")
                                        {
                                            dgrdvConveyor2InferResultDisp.Rows[i].DefaultCellStyle.ForeColor = Color.Red;
                                        }
                                    }
                                }
                                else
                                {
                                    dgrdvConveyor2InferResultDisp.Rows.Add(dr.ItemArray);
                                    if (dr[4].ToString() == "PASS")
                                    {
                                        dgrdvConveyor2InferResultDisp.Rows[i].DefaultCellStyle.ForeColor = Color.LimeGreen;
                                    }
                                    else if (dr[4].ToString() == "FAIL")
                                    {
                                        dgrdvConveyor2InferResultDisp.Rows[i].DefaultCellStyle.ForeColor = Color.Red;
                                    }
                                }
                                // dt.Rows.Add(dr.ItemArray);
                            }
                        }
                    })); ;
                }
                #endregion

                #region 更新产量
                lock (_yieldRecordDt)
                {

                    int currentTimeHour = Convert.ToInt32(System.DateTime.Now.TimeOfDay.ToString("hh"));
                    int currentTimeMinutes = Convert.ToInt32(System.DateTime.Now.TimeOfDay.ToString("mm"));

                    if ((currentTimeHour > SettingsGeneralForm._clearYieldTimeHour ||
                        (currentTimeHour == SettingsGeneralForm._clearYieldTimeHour && currentTimeMinutes > SettingsGeneralForm._clearYieldTimeMinutes))
                        && (currentTimeHour < SettingsGeneralForm._clearYieldTimeHour + 12 ||
                         (currentTimeHour == SettingsGeneralForm._clearYieldTimeHour + 12 && currentTimeMinutes <= SettingsGeneralForm._clearYieldTimeMinutes))
                         && _yieldResetFlag == 0 && SettingsGeneralForm._autoClearYieldUseFlag)//如果为需要清除晚班数据，当前为白班的话，清除
                    {
                        ClearYieldData();
                    }
                    else if (((currentTimeHour >= 0 && (currentTimeHour < SettingsGeneralForm._clearYieldTimeHour || currentTimeHour == SettingsGeneralForm._clearYieldTimeHour && currentTimeMinutes <= SettingsGeneralForm._clearYieldTimeMinutes))
                        || ((currentTimeHour > SettingsGeneralForm._clearYieldTimeHour + 12 || (currentTimeHour == SettingsGeneralForm._clearYieldTimeHour + 12 && currentTimeMinutes > SettingsGeneralForm._clearYieldTimeMinutes)) && currentTimeHour < 24))
                        && _yieldResetFlag == 1 && SettingsGeneralForm._autoClearYieldUseFlag)//如果为需要清除白班数据，当前为晚班的话，清除
                    {
                        ClearYieldData();
                    }
                    RefreshYield();
                    SaveRecordYieldData();
                }


                btnClearYield.Invoke(new Action(() =>
                    {
                        if (_accessLevel == 101 || _accessLevel == 102)
                        {
                            if (!btnClearYield.Enabled)
                                btnClearYield.Enabled = true;
                        }
                        else
                        {
                            if (btnClearYield.Enabled)
                                btnClearYield.Enabled = false;
                        }
                    })); ;
                #endregion

                #region 权限等级变化对控件进行处理
                if (_accessLevel == 0 || _accessLevel == 100)
                {
                    //权限切换回来之后，自动回到main界面
                    this.Invoke(new Action(() =>
                    {
                        if (!_mainFormVariate.Visible)
                            _mainFormVariate.Show();
                        if (_baseSettingFormVariate.Visible)
                            _baseSettingFormVariate.Hide();
                        if (_axisControlFormVariate.Visible)
                            _axisControlFormVariate.Hide();
                    })); ;
                }

                if (_baseSettingFormVariate._settingsAdminFormVariate.Visible)
                {
                    if (_accessLevel == 102 && !_baseSettingFormVariate._settingsAdminFormVariate.chkPassThresholdUsing.Visible)//如果为厂家权限
                    {
                        _baseSettingFormVariate._settingsAdminFormVariate.chkPassThresholdUsing.Invoke(new Action(() =>
                            {
                                _baseSettingFormVariate._settingsAdminFormVariate.chkPassThresholdUsing.Visible = true;
                            })); ;
                    }
                    else if (_accessLevel != 102 && _baseSettingFormVariate._settingsAdminFormVariate.chkPassThresholdUsing.Visible)
                    {
                        _baseSettingFormVariate._settingsAdminFormVariate.chkPassThresholdUsing.Invoke(new Action(() =>
                        {
                            _baseSettingFormVariate._settingsAdminFormVariate.chkPassThresholdUsing.Visible = false;
                        })); ;
                    }

                    if (_accessLevel == 102 && !_baseSettingFormVariate._settingsAdminFormVariate.chkCheckProductIsConnector.Visible)//如果为厂家权限
                    {
                        _baseSettingFormVariate._settingsAdminFormVariate.chkCheckProductIsConnector.Invoke(new Action(() =>
                            {
                                _baseSettingFormVariate._settingsAdminFormVariate.chkCheckProductIsConnector.Visible = true;
                            })); ;
                    }
                    else if (_accessLevel != 102 && _baseSettingFormVariate._settingsAdminFormVariate.chkCheckProductIsConnector.Visible)
                    {
                        _baseSettingFormVariate._settingsAdminFormVariate.chkCheckProductIsConnector.Invoke(new Action(() =>
                        {
                            _baseSettingFormVariate._settingsAdminFormVariate.chkCheckProductIsConnector.Visible = false;
                        })); ;
                    }

                    //自动模式下检测种类选择框不可选择
                    if (_autoModeFlag && _baseSettingFormVariate._settingsAdminFormVariate.chkCheckProductIsConnector.Enabled)//如果自动模式下，选择检测种类为可选择
                    {
                        _baseSettingFormVariate._settingsAdminFormVariate.chkCheckProductIsConnector.Invoke(new Action(() =>
                        {
                            _baseSettingFormVariate._settingsAdminFormVariate.chkCheckProductIsConnector.Enabled = false;
                        })); ;
                    }
                    else if (!_autoModeFlag && !_baseSettingFormVariate._settingsAdminFormVariate.chkCheckProductIsConnector.Enabled)//如果为非自动模式，选择检测种类为不可选择
                    {
                        _baseSettingFormVariate._settingsAdminFormVariate.chkCheckProductIsConnector.Invoke(new Action(() =>
                        {
                            _baseSettingFormVariate._settingsAdminFormVariate.chkCheckProductIsConnector.Enabled = true;
                        })); ;
                    }
                    

                }

                if (_baseSettingFormVariate._maintenanceCylinderControlFormVariate.Visible)
                {
                    if (_accessLevel == 102 && !_baseSettingFormVariate._maintenanceCylinderControlFormVariate.btnCameraTransferCylinder.Visible)//如果为厂家权限
                    {
                        _baseSettingFormVariate._maintenanceCylinderControlFormVariate.btnCameraTransferCylinder.Invoke(new Action(() =>
                           {
                               _baseSettingFormVariate._maintenanceCylinderControlFormVariate.btnCameraTransferCylinder.Visible = true;
                               _baseSettingFormVariate._maintenanceCylinderControlFormVariate.btnCameraTransferCylinderBackStatus.Visible = true;
                               _baseSettingFormVariate._maintenanceCylinderControlFormVariate.btnCameraTransferCylinderOutStatus.Visible = true;
                           })); ;
                    }
                    else if(_accessLevel != 102 && _baseSettingFormVariate._maintenanceCylinderControlFormVariate.btnCameraTransferCylinder.Visible)//如果不为厂家权限
                    {
                         _baseSettingFormVariate._maintenanceCylinderControlFormVariate.btnCameraTransferCylinder.Invoke(new Action(() =>
                           {
                               _baseSettingFormVariate._maintenanceCylinderControlFormVariate.btnCameraTransferCylinder.Visible = false;
                               _baseSettingFormVariate._maintenanceCylinderControlFormVariate.btnCameraTransferCylinderBackStatus.Visible = false;
                               _baseSettingFormVariate._maintenanceCylinderControlFormVariate.btnCameraTransferCylinderOutStatus.Visible = false;
                           })); ;
                    }
                }
                
                //在操作员权限下，不予显示手动按钮和步进按钮，如果处于步进状态，解除步进
                if (_mainFormVariate.Visible)//如果主界面显示
                {
                    _mainFormVariate.Invoke(new Action(() =>
                        {
                            if ((_accessLevel == 100 || _accessLevel == 0) && _mainFormVariate.btnManual.Visible)//如果为操作员权限并且手动按钮为显示状态
                            {
                                _mainFormVariate.btnManual.Visible = false;
                            }
                            else if (_accessLevel != 100 && _accessLevel != 0 && !_mainFormVariate.btnManual.Visible)//如果不为操作员权限并且手动按钮为不显示状态
                            {
                                _mainFormVariate.btnManual.Visible = true;
                            }

                            if ((_accessLevel == 100 || _accessLevel == 0) && _mainFormVariate.btnStep.Visible)//如果为操作员权限并且步进按钮为显示状态
                            {
                                _mainFormVariate.btnStep.Visible = false;
                            }
                            else if (_accessLevel != 100 && _accessLevel != 0 && !_mainFormVariate.btnStep.Visible)//如果不为操作员权限并且步进按钮为不显示状态
                            {
                                _mainFormVariate.btnStep.Visible = true;
                            }

                            if ((_accessLevel == 100 || _accessLevel == 0) && MainForm._stepModeFlag)//如果为操作员权限并且步进模式为启用
                            {
                                MainForm._stepModeFlag = false;//关闭步进模式
                            }

                            //操作员权限下，如果传送线1强制下料可见，设置为不可见
                            if ((_accessLevel == 100 || _accessLevel == 0) && 
                                (_mainFormVariate.btnConveyor1ProductForcedMoveOut.Visible || _mainFormVariate.lblConveyor1ProductForcedMoveOutTitle.Visible))
                            {
                                _mainFormVariate.btnConveyor1ProductForcedMoveOut.Visible = false;
                                _mainFormVariate.lblConveyor1ProductForcedMoveOutTitle.Visible = false;
                            }
                            else if (_accessLevel != 100 && _accessLevel != 0 &&
                                (!_mainFormVariate.btnConveyor1ProductForcedMoveOut.Visible || !_mainFormVariate.lblConveyor1ProductForcedMoveOutTitle.Visible))
                            {
                                _mainFormVariate.btnConveyor1ProductForcedMoveOut.Visible = true;
                                _mainFormVariate.lblConveyor1ProductForcedMoveOutTitle.Visible = true;
                            }

                            //操作员权限下，如果传送线2强制下料可见，设置为不可见
                            if ((_accessLevel == 100 || _accessLevel == 0) &&
                                (_mainFormVariate.btnConveyor2ProductForcedMoveOut.Visible || _mainFormVariate.lblConveyor2ProductForcedMoveOutTitle.Visible))
                            {
                                _mainFormVariate.btnConveyor2ProductForcedMoveOut.Visible = false;
                                _mainFormVariate.lblConveyor2ProductForcedMoveOutTitle.Visible = false;
                            }
                            else if (_accessLevel != 100 && _accessLevel != 0 &&
                                (!_mainFormVariate.btnConveyor2ProductForcedMoveOut.Visible || !_mainFormVariate.lblConveyor2ProductForcedMoveOutTitle.Visible))
                            {
                                _mainFormVariate.btnConveyor2ProductForcedMoveOut.Visible = true;
                                _mainFormVariate.lblConveyor2ProductForcedMoveOutTitle.Visible = true;
                            }

                            //在自动模式下手动强制下料无效
                            if (_autoModeFlag && (_mainFormVariate.btnConveyor1ProductForcedMoveOut.Enabled || _mainFormVariate.btnConveyor2ProductForcedMoveOut.Enabled))
                            {
                                _mainFormVariate.btnConveyor1ProductForcedMoveOut.Enabled = false;
                                _mainFormVariate.btnConveyor2ProductForcedMoveOut.Enabled = false;
                            }
                            else if (!_autoModeFlag && (!_mainFormVariate.btnConveyor1ProductForcedMoveOut.Enabled || !_mainFormVariate.btnConveyor2ProductForcedMoveOut.Enabled))
                            {
                                _mainFormVariate.btnConveyor1ProductForcedMoveOut.Enabled = true;
                                _mainFormVariate.btnConveyor2ProductForcedMoveOut.Enabled = true;
                            }

                        })); ;
                }

                //如果切换为操作员权限，那么关闭轨迹显示窗口
                if (_baseSettingFormVariate._settingsProductFormVariate._trackDispFormVariate != null)
                {
                    if (_baseSettingFormVariate._settingsProductFormVariate._trackDispFormVariate.Visible && (_accessLevel == 0 || _accessLevel == 100))
                    {
                        _baseSettingFormVariate._settingsProductFormVariate._trackDispFormVariate.Invoke(new Action(() =>
                            {
                                _baseSettingFormVariate._settingsProductFormVariate._trackDispFormVariate.Dispose();
                            })); ;
                    }
                }
                #endregion

                #region 设备模块状态等显示

                ActionVariateDisp();

                #endregion

                #region ByPass页面显示
                //如果为自动模式并且完成原点复归并且不启用传送线1和传送线2
                if (!SettingsGeneralForm._conveyor1UsingFlag && !SettingsGeneralForm._conveyor2UsingFlag)//&& _allAxisHomeBackModuleActionFlag == 1)_autoModeFlag && 
                {
                    if (_mainFormVariate.Visible)
                    {
                        _mainFormVariate.Invoke(new Action(() =>
                            {
                                if (_mainFormVariate.picLeftCameraResultImageDisp.Visible)
                                    _mainFormVariate.picLeftCameraResultImageDisp.Visible = false;
                                if (_mainFormVariate.picLeftCameraSourceImageDisp.Visible)
                                    _mainFormVariate.picLeftCameraSourceImageDisp.Visible = false;
                                if (_mainFormVariate.picRightCameraResultImageDisp.Visible)
                                    _mainFormVariate.picRightCameraResultImageDisp.Visible = false;
                                if (_mainFormVariate.picRightCameraSourceImageDisp.Visible)
                                    _mainFormVariate.picRightCameraSourceImageDisp.Visible = false;
                                if (_mainFormVariate.lblResultImageTitle.Visible)
                                    _mainFormVariate.lblResultImageTitle.Visible = false;
                                if (_mainFormVariate.lblSourceImageTitle.Visible)
                                    _mainFormVariate.lblSourceImageTitle.Visible = false;
                                if (!_mainFormVariate.lblInfo.Visible)
                                    _mainFormVariate.lblInfo.Visible = true;
                            })); ;
                    }
                }
                else
                {
                    if (_mainFormVariate.Visible)
                    {
                        _mainFormVariate.Invoke(new Action(() =>
                        {
                            if (_mainFormVariate.lblInfo.Visible)
                                _mainFormVariate.lblInfo.Visible = false;
                            if (!_mainFormVariate.picLeftCameraResultImageDisp.Visible)
                                _mainFormVariate.picLeftCameraResultImageDisp.Visible = true;
                            if (!_mainFormVariate.picLeftCameraSourceImageDisp.Visible)
                                _mainFormVariate.picLeftCameraSourceImageDisp.Visible = true;
                            if (!_mainFormVariate.picRightCameraResultImageDisp.Visible)
                                _mainFormVariate.picRightCameraResultImageDisp.Visible = true;
                            if (!_mainFormVariate.picRightCameraSourceImageDisp.Visible)
                                _mainFormVariate.picRightCameraSourceImageDisp.Visible = true;
                            if (!_mainFormVariate.lblResultImageTitle.Visible)
                                _mainFormVariate.lblResultImageTitle.Visible = true;
                            if (!_mainFormVariate.lblSourceImageTitle.Visible)
                                _mainFormVariate.lblSourceImageTitle.Visible = true;
                        })); ;
                    }

                }

                if (!SettingsGeneralForm._conveyor1UsingFlag && _mainFormVariate.Visible && _mainFormVariate.lblConveyor1Status.Text != "传送线1状态：By Pass")//如果传送线1不启用
                {
                    _mainFormVariate.Invoke(new Action(() =>
                        {
                            _mainFormVariate.lblConveyor1Status.Text = "传送线1状态：By Pass";
                            _mainFormVariate.lblConveyor1Status.ForeColor = Color.Red;
                        })); ;
                }
                else if (SettingsGeneralForm._conveyor1UsingFlag && _mainFormVariate.Visible && _mainFormVariate.lblConveyor1Status.Text != "传送线1状态：正常运行")
                {
                    _mainFormVariate.Invoke(new Action(() =>
                    {
                        _mainFormVariate.lblConveyor1Status.Text = "传送线1状态：正常运行";
                        _mainFormVariate.lblConveyor1Status.ForeColor = Color.LimeGreen;
                    })); ;
                }

                if (!SettingsGeneralForm._conveyor2UsingFlag && _mainFormVariate.Visible && _mainFormVariate.lblConveyor2Status.Text != "传送线2状态：By Pass")//如果传送线2不启用
                {
                    _mainFormVariate.Invoke(new Action(() =>
                    {
                        _mainFormVariate.lblConveyor2Status.Text = "传送线2状态：By Pass";
                        _mainFormVariate.lblConveyor2Status.ForeColor = Color.Red;
                    })); ;
                }
                else if (SettingsGeneralForm._conveyor2UsingFlag && _mainFormVariate.Visible && _mainFormVariate.lblConveyor2Status.Text != "传送线2状态：正常运行")
                {
                    _mainFormVariate.Invoke(new Action(() =>
                    {
                        _mainFormVariate.lblConveyor2Status.Text = "传送线2状态：正常运行";
                        _mainFormVariate.lblConveyor2Status.ForeColor = Color.LimeGreen;
                    })); ;
                }

                #endregion

                #region 推断结果OK条件设定控件的显示与隐藏，当为connector的时候显示

                if (_baseSettingFormVariate._settingsProductFormVariate.Visible && !SettingsAdministratorForm._checkProductIsConnector &&
                    _baseSettingFormVariate._settingsProductFormVariate.gbxInspectOKConditionSetting.Visible)
                {
                    _baseSettingFormVariate._settingsProductFormVariate.gbxInspectOKConditionSetting.Invoke(new Action(() =>
                        {
                            _baseSettingFormVariate._settingsProductFormVariate.gbxInspectOKConditionSetting.Visible = false;
                        })); ;
                }
                else if (_baseSettingFormVariate._settingsProductFormVariate.Visible && SettingsAdministratorForm._checkProductIsConnector &&
                    !_baseSettingFormVariate._settingsProductFormVariate.gbxInspectOKConditionSetting.Visible)
                {
                    _baseSettingFormVariate._settingsProductFormVariate.gbxInspectOKConditionSetting.Invoke(new Action(() =>
                        {
                            _baseSettingFormVariate._settingsProductFormVariate.gbxInspectOKConditionSetting.Visible = true;
                        })); ;
                }

                #endregion

                Thread.Sleep(50);
            }
        }

        #endregion

        #region MainForm相关事件

        //主界面请求关闭软件事件函数
        private void MainFormRequestCloseSoftwareEventFunc()
        {
            MyTool.TxtFileProcess.CreateLog("从主界面请求退出软件！" + "----用户：" + _operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

            this.Dispose();
        }

        //主界面请求切换界面事件函数
        private void MainFormRequestChangeFormEventFunc()
        {
            MyTool.TxtFileProcess.CreateLog("主界面请求切换界面至手动设置界面！" + "----用户：" + _operatorName, BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
            if (_accessLevel == 0 || _accessLevel == 100)
            {
                MessageBox.Show("当前权限等级不够，请切换高权限用户！");
                return;
            }
            _baseSettingFormVariate.Visible = true;
            _mainFormVariate.Visible = false;
        }

        //主界面请求切换产品事件函数
        private void MainFormChangeProductEventFunc(string productName, int changeIndex)
        {

            if (changeIndex == 0)//如果为改传送线1产品
            {
                SettingsProductForm._track1ActualsProductName = productName;
                MyTool.TxtFileProcess.CreateLog("切换产品：从产品“" + SettingsProductForm._track1ActualsProductName + "”切换为“" + productName + "”----用户：" + _operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                SettingsProductForm._track1ActualsProductName = productName;
            }
            else
            {
                SettingsProductForm._track2ActualsProductName = productName;
                MyTool.TxtFileProcess.CreateLog("切换产品：从产品“" + SettingsProductForm._track2ActualsProductName + "”切换为“" + productName + "”----用户：" + _operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                SettingsProductForm._track2ActualsProductName = productName;
            }

            _baseSettingFormVariate._settingsProductFormVariate.MainFormDataSavingSettingData();

            //基于EDIT product name设置combo box
            _baseSettingFormVariate._settingsProductFormVariate.cboAvaliableProduct_MouseClick(new object(), new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0));
            _baseSettingFormVariate._settingsProductFormVariate.cboAvaliableProduct.SelectedIndex =
                _baseSettingFormVariate._settingsProductFormVariate.cboAvaliableProduct.FindString(SettingsProductForm._editProductName);

            SamplePanelCheckTimeAndStatusSaveFunc(changeIndex, false);
            ProductShiftTimeSaveFunc(changeIndex);

            _mainFormVariate.lblTrack1ActualProduct1.Text = SettingsProductForm._track1ActualsProductName;
            _mainFormVariate.lblTrack2ActualProduct1.Text = SettingsProductForm._track2ActualsProductName;
        }

        //主界面按下虚拟按钮事件函数
        bool _autoStartingFlag = false;
        private void MainFormClickVirtualButtonEventFunc(int virtualButtonIndex)
        {
            if (_deviceStartDelay.STATUS)//设备启动延时之后有效
            {
                if (virtualButtonIndex == 0 && !_autoStartingFlag)//如果为自动按钮
                {
                    _autoStartingFlag = true;

                    MyTool.TxtFileProcess.CreateLog("请求启动自动模式" + "----用户：" + _operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                    if (_accessLevel == 0)
                    {
                        MyTool.TxtFileProcess.CreateLog("请求启动自动模式失败，用户未登录！" + "----用户：" + _operatorName,
                        BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                        MessageBox.Show("未登录用户，请登陆后操作！");
                        _autoStartingFlag = false;
                        return;
                    }

                    if (SettingsProductForm._track1ActualsProductName != "")//如果传送线1产品名不为空
                    {
                        _productParameter[0].ProductDataLoadToVariate(SettingsGeneralForm._productDirectory + "\\" + SettingsProductForm._track1ActualsProductName + ".rcp");
                        if (!_productParameter[0]._parameterLoadSuccessfulFlag)
                        {
                            MyTool.TxtFileProcess.CreateLog("请求启动自动模式失败，" + "传送线1运行设定的配方文件“" + SettingsProductForm._track1ActualsProductName + ".rcp”中有非法参数！" + "----用户：" + _operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                            MessageBox.Show("传送线1运行设定的配方文件" + SettingsProductForm._track1ActualsProductName + ".rcp中有非法参数，无法启动自动模式！");
                            _autoStartingFlag = false;
                            return;
                        }
                    }

                    if (SettingsProductForm._track2ActualsProductName != "")//如果传送线2产品名不为空
                    {
                        _productParameter[1].ProductDataLoadToVariate(SettingsGeneralForm._productDirectory + "\\" + SettingsProductForm._track2ActualsProductName + ".rcp");
                        if (!_productParameter[1]._parameterLoadSuccessfulFlag)
                        {
                            MyTool.TxtFileProcess.CreateLog("请求启动自动模式失败，" + "传送线2运行设定的配方文件“" + SettingsProductForm._track1ActualsProductName + ".rcp”中有非法参数！" + "----用户：" + _operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                            MessageBox.Show("传送线2运行设定的配方文件" + SettingsProductForm._track2ActualsProductName + ".rcp中有非法参数，无法启动自动模式！");
                            _autoStartingFlag = false;
                            return;
                        }
                    }

                    if (!AxisParCheck(SettingsProductForm._track1ActualsProductName) || !AxisPosDataCheck(SettingsProductForm._track1ActualsProductName))
                    {
                        MyTool.TxtFileProcess.CreateLog("请求启动自动模式失败，" + "产品“" + SettingsProductForm._track1ActualsProductName + "”的轴参数文件有异常！" + "----用户：" + _operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                        MessageBox.Show("产品" + SettingsProductForm._track1ActualsProductName + "的轴参数文件有异常，无法启动自动模式！");
                        _autoStartingFlag = false;
                        return;
                    }

                    if (!AxisParCheck(SettingsProductForm._track2ActualsProductName) || !AxisPosDataCheck(SettingsProductForm._track2ActualsProductName))
                    {
                        MyTool.TxtFileProcess.CreateLog("请求启动自动模式失败，" + "产品“" + SettingsProductForm._track2ActualsProductName + "”的轴参数文件有异常！" + "----用户：" + _operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                        MessageBox.Show("产品" + SettingsProductForm._track2ActualsProductName + "的轴参数文件有异常，无法启动自动模式！");
                        _autoStartingFlag = false;
                        return;
                    }

                    if (_deadlyAlarmOccurFlag || _heavyAlarmOccurFlag || _moderateAlarmOccurFlag)
                    {
                        MyTool.TxtFileProcess.CreateLog("请求启动自动模式失败，当前设备处于报警状态！" + "----用户：" + _operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                        MessageBox.Show("当前设备处于报警状态，无法切换模式至自动！");
                        _autoStartingFlag = false;
                        return;
                    }


                    //关闭相机
                    _leftCameraCs.CloseCamera2();
                    _rightCameraCs.CloseCamera2();
                    _backCameraCs.CloseCamera2();

                    if (_leftCameraCs.hv_AcqHandle == null)
                    {
                        if (!OpenCamera(ref _leftCameraCs, SettingsAdministratorForm._leftCameraName, true, true))
                        {
                            MyTool.TxtFileProcess.CreateLog("请求启动自动模式失败，打开左侧相机失败！" + "----用户：" + _operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                            MessageBox.Show("打开左侧相机失败！");
                            _autoStartingFlag = false;
                            return;
                        }
                    }

                    if (_rightCameraCs.hv_AcqHandle == null)
                    {
                        if (!OpenCamera(ref _rightCameraCs, SettingsAdministratorForm._rightCameraName, true, true))
                        {
                            MyTool.TxtFileProcess.CreateLog("请求启动自动模式失败，打开右侧相机失败！" + "----用户：" + _operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                            MessageBox.Show("打开右侧相机失败！");
                            _autoStartingFlag = false;
                            return;
                        }
                    }

                    if (_backCameraCs.hv_AcqHandle == null)
                    {
                        if (!OpenCamera(ref _backCameraCs, SettingsAdministratorForm._backCameraName, false, true))
                        {
                            MyTool.TxtFileProcess.CreateLog("请求启动自动模式失败，打开后侧相机失败！" + "----用户：" + _operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                            MessageBox.Show("打开后侧相机失败！");
                            _autoStartingFlag = false;
                            return;
                        }
                    }

                    //判断指定的路径是否合法
                    try
                    {
                        Directory.Exists(SettingsGeneralForm._dataDirectory);
                    }
                    catch
                    {
                        MyTool.TxtFileProcess.CreateLog("请求启动自动模式失败，指定的常规检测结果路径不合法！" + "----用户：" + _operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                            MessageBox.Show("请求启动自动模式失败，指定的常规检测结果路径不合法！");
                            _autoStartingFlag = false;
                            return;
                    }

                    try
                    {
                        Directory.Exists(SettingsGeneralForm._samplePanelDirectory);
                    }
                    catch
                    {
                        MyTool.TxtFileProcess.CreateLog("请求启动自动模式失败，指定的样品板检测结果路径不合法！" + "----用户：" + _operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                        MessageBox.Show("请求启动自动模式失败，指定的样品板检测结果路径不合法！");
                        _autoStartingFlag = false;
                        return;
                    }
  
                }
                _virtualAndRealBtnIndex = virtualButtonIndex;
            }
        }

        //主界面点击传送线强制流出产品事件
        private void MainFormConveyorForcedMoveOutEventFunc(int conveyorIndex)
        {
            if (_autoModeFlag)//如果自动模式
            {
                MyTool.TxtFileProcess.CreateLog("当前设备处于自动模式，主界面请求传送线"+(conveyorIndex+1).ToString()+"强制下料失败！" + "----用户：" + _operatorName,
                BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

                MessageBox.Show("当前传送线1强制下料模块繁忙！");
                return;
            }


            if (conveyorIndex == 0)//如果是传送线1
            {
                if (_conveyor1ProductForcedUnloadModuleActionFlag == 0)//如果传送线1强制下料模块不为空闲
                {
                    MyTool.TxtFileProcess.CreateLog("当前传送线1强制下料模块繁忙，主界面请求传送线" + (conveyorIndex + 1).ToString() + "强制下料失败！" + "----用户：" + _operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

                    MessageBox.Show("当前传送线1强制下料模块繁忙！");
                    return;
                }
                else
                {
                    Conveyor1ForceOutProduct();
                }
            }
            else
            {
                if (_conveyor2ProductForcedUnloadModuleActionFlag == 0)//如果传送线1强制下料模块不为空闲
                {
                    MyTool.TxtFileProcess.CreateLog("当前传送线2强制下料模块繁忙，主界面请求传送线" + (conveyorIndex + 1).ToString() + "强制下料失败！" + "----用户：" + _operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

                    MessageBox.Show("当前传送线2强制下料模块繁忙！");
                    return;
                }
                else
                {
                    Conveyor2ForceOutProduct();
                }
            }
        }

        #endregion

        #region AxisParCheck：轴参数文件检测
        private bool AxisParCheck(string productName)
        {
            string axisParameterFilePath; //轴参数文件保存路径
            string axisParameterFileSaveFolderPath;//轴参数保存文件夹路径

            axisParameterFileSaveFolderPath = Directory.GetCurrentDirectory() + @"\" +
                MyTool.FolderAndFileManageClass.FilterPathIllegalChar("AxisParameterAndPositionData_" + productName) + @"\";//过滤掉作为Directory不合法的字符
            axisParameterFilePath = Path.Combine(axisParameterFileSaveFolderPath, "AxisPar" + ".csv");

            if (!Directory.Exists(axisParameterFileSaveFolderPath))
            {
                Directory.CreateDirectory(axisParameterFileSaveFolderPath);//创建文件夹
            }
            if (!File.Exists(axisParameterFilePath))
            {
                MyTool.TxtFileProcess.CreateLog("轴参数文件检测——产品“" + productName + "”的轴参数文件不存在！" + "----用户：" + _operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                return false;
            }

            //读取参数
            DataTable dt = new DataTable();
            try
            {
                dt = CSVFileOperation.ReadCSV(axisParameterFilePath);
            }
            catch
            {
                MyTool.TxtFileProcess.CreateLog("轴参数文件检测——读取产品“" + productName + "”的轴参数文件失败！" + "----用户：" + _operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                MessageBox.Show("无法读取轴参数文件!");
                return false;
            }

            try
            {
                for (int i = 0; i < _axisQuantity; i++)
                {
                    float testFltData;
                    int testIntData;
                    testFltData = Convert.ToSingle(dt.Rows[i][1]);
                    testFltData = Convert.ToSingle(dt.Rows[i][2]);
                    testFltData = Convert.ToSingle(dt.Rows[i][3]);
                    testFltData = Convert.ToSingle(dt.Rows[i][4]);
                    testFltData = Convert.ToSingle(dt.Rows[i][5]);
                    testFltData = Convert.ToSingle(dt.Rows[i][5]);
                    testFltData = Convert.ToSingle(dt.Rows[i][6]);
                    testFltData = Convert.ToSingle(dt.Rows[i][6]);
                    testFltData = Convert.ToSingle(dt.Rows[i][7]);
                    testFltData = Convert.ToSingle(dt.Rows[i][8]);
                    testFltData = Convert.ToSingle(dt.Rows[i][9]);
                    testFltData = Convert.ToSingle(dt.Rows[i][9]);
                    testFltData = Convert.ToSingle(dt.Rows[i][10]);
                    testFltData = Convert.ToSingle(dt.Rows[i][11]);
                    testFltData = Convert.ToSingle(dt.Rows[i][12]);
                    testFltData = Convert.ToSingle(dt.Rows[i][12]);
                    testFltData = Convert.ToSingle(dt.Rows[i][13]);
                    testFltData = Convert.ToSingle(dt.Rows[i][14]);

                    testFltData = Convert.ToSingle(dt.Rows[i][15]);
                    testFltData = Convert.ToSingle(dt.Rows[i][16]);
                    testFltData = Convert.ToSingle(dt.Rows[i][17]);
                    testFltData = Convert.ToSingle(dt.Rows[i][18]);
                    testFltData = Convert.ToSingle(dt.Rows[i][19]);
                    testFltData = Convert.ToSingle(dt.Rows[i][20]);
                    testFltData = Convert.ToSingle(dt.Rows[i][21]);
                    testIntData = Convert.ToInt32(dt.Rows[i][22]);
                    if (testIntData < 7 && (i == 0 || i == 1 || i == 2))
                    {
                        MyTool.TxtFileProcess.CreateLog("轴参数文件检测——产品“" + productName + "”的轴" + (i + 1).ToString() + "点位个数小于7个，无法支持程序的正常运行！" + "----用户：" + _operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                        MessageBox.Show("轴" + (i + 1).ToString() + "点位个数小于7个，无法支持程序的正常运行！");
                        return false;
                    }
                    if (dt.Rows[i][23].ToString() == "0" && i <= 5)//如果程序运行必须的6个轴中有轴被屏蔽，数据检测失败
                    {
                        MyTool.TxtFileProcess.CreateLog("轴参数文件检测——产品“" + productName + "”的轴" + (i + 1).ToString() + "被屏蔽，无法支持程序的正常运行！" + "----用户：" + _operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                        MessageBox.Show("轴" + (i + 1).ToString() + "被屏蔽，无法支持程序的正常运行！");
                        return false;
                    }
                }
            }
            catch
            {
                MyTool.TxtFileProcess.CreateLog("轴参数文件检测——产品“" + productName + "”的轴参数文件错误！" + "----用户：" + _operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                MessageBox.Show("轴参数文件数据错误!");
                return false;
            }

            return true;
        }
        #endregion

        #region AxisPosDataCheck:轴点位数据检查
        private bool AxisPosDataCheck(string productName)
        {
            string axisPositionFilePath; //轴参数文件保存路径
            string axisPositionFileSaveFolderPath;//轴参数保存文件夹路径
            DataTable axisPositionDataDT = null;
            axisPositionFileSaveFolderPath = Directory.GetCurrentDirectory() + @"\" +
                MyTool.FolderAndFileManageClass.FilterPathIllegalChar("AxisParameterAndPositionData_" + productName) + @"\";//过滤掉作为Directory不合法的字符
            if (!Directory.Exists(axisPositionFileSaveFolderPath))
            {
                MyTool.TxtFileProcess.CreateLog("轴点位数据文件检测——产品“" + productName + "”的轴点位数据文件不存在！" + "----用户：" + _operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                return false;
            }

            for (int i = 0; i < _axisQuantity; i++)
            {
                axisPositionFilePath = Path.Combine(axisPositionFileSaveFolderPath,
                    MyTool.FolderAndFileManageClass.FilterPathIllegalChar("axis" + i.ToString("00") + "Pos" + "_" + productName)
                    + ".csv");
                if (!File.Exists(axisPositionFilePath))
                {
                    return false;
                }

                //读取数据至DT中
                axisPositionDataDT = CSVFileOperation.ReadCSV(axisPositionFilePath);//不包含第一行
                if (axisPositionDataDT.Columns.Count < 8)//如果列数不对，全部重新初始化
                {
                    MyTool.TxtFileProcess.CreateLog("轴点位数据文件检测——产品“" + productName + "”的轴" + (i + 1).ToString() + "的点位数据列数小于8个，无法支持程序的正常运行！" + "----用户：" + _operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

                    return false;
                }

                if (axisPositionDataDT.Rows.Count < 7 && (i == 0 || i == 1 || i == 2))//如果点位个数小于程序运行所需的最小点位个数
                {
                    MyTool.TxtFileProcess.CreateLog("轴点位数据文件检测——产品“" + productName + "”的轴" + (i + 1).ToString() + "点位个数小于7个，无法支持程序的正常运行！" + "----用户：" + _operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

                    MessageBox.Show("轴" + (i + 1).ToString() + "点位个数小于7个，无法支持程序的正常运行！");
                    return false;
                }
            }
            return true;
        }
        #endregion

        #region BaseSettingsForm相关事件

        //base setting form请求关闭程序事件函数
        private void BaseSettingsFormRequestCloseSoftwareEventFunc()
        {
            MyTool.TxtFileProcess.CreateLog("从手动设置界面请求退出程序！" + "----用户：" + _operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

            this.Dispose();
        }

        //base setting form请求切换界面事件函数
        private void BaseSettingsFormRequestChangeFormEventFunc()
        {
            MyTool.TxtFileProcess.CreateLog("请求切换界面至主界面！" + "----用户：" + _operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

            _baseSettingFormVariate.Visible = false;
            _mainFormVariate.Visible = true;
        }

        //base setting form请求打开各轴控制界面函数
        private void BaseSettingsFormRequestOpenAxisControlFormEventFunc()
        {
            if (_axisControlFormVariate != null)
            {
                if (!_axisControlFormVariate.Visible)
                {
                    MyTool.TxtFileProcess.CreateLog("从手动设置界面请求打开轴手动控制界面！" + "----用户：" + _operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

                    _axisControlFormVariate.Show();
                    _axisControlFormVariate.Focus();
                }
                else
                {
                    MyTool.TxtFileProcess.CreateLog("从手动设置界面请求关闭轴手动控制界面！" + "----用户：" + _operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                    _axisControlFormVariate.Hide();
                }
            }
        }

        //maintenance camera form请求打开或关闭相机
        private void MaintenanceCameraFormRequestOpenOrCloseCameraEventFunc(int cameraIndex)
        {
            switch (cameraIndex)
            {
                case 0://如果为左侧相机
                    if (_leftCameraCs.hv_AcqHandle == null)
                    {
                        MyTool.TxtFileProcess.CreateLog("从手动设置界面请求打开左侧相机！" + "----用户：" + _operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

                        _backCameraGrab = false;
                        _backCameraVideo = false;
                        Thread.Sleep(100);//等待
                        if (!OpenCamera(ref _leftCameraCs, SettingsAdministratorForm._leftCameraName, true, true))
                        {
                            MyTool.TxtFileProcess.CreateLog("打开左侧相机失败！" + "----用户：" + _operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

                            MessageBox.Show("打开左侧相机失败");
                        }
                    }
                    else
                    {
                        MyTool.TxtFileProcess.CreateLog("从手动设置界面请求关闭左侧相机！" + "----用户：" + _operatorName,
                   BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

                        _leftCameraGrab = false;
                        _leftCameraVideo = false;
                        Thread.Sleep(100);//等待
                        CloseCamera(ref _leftCameraCs);
                    }
                    break;
                case 1://如果为右侧相机
                    if (_rightCameraCs.hv_AcqHandle == null)
                    {
                        MyTool.TxtFileProcess.CreateLog("从手动设置界面请求打开右侧相机！" + "----用户：" + _operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

                        _backCameraGrab = false;
                        _backCameraVideo = false;
                        Thread.Sleep(100);//等待
                        if (!OpenCamera(ref _rightCameraCs, SettingsAdministratorForm._rightCameraName, true, true))
                        {
                            MyTool.TxtFileProcess.CreateLog("打开右侧相机失败！" + "----用户：" + _operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

                            MessageBox.Show("打开右侧相机失败");
                        }
                    }
                    else
                    {
                        MyTool.TxtFileProcess.CreateLog("从手动设置界面请求关闭右侧相机！" + "----用户：" + _operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

                        _rightCameraGrab = false;
                        _rightCameraVideo = false;
                        Thread.Sleep(100);//等待
                        CloseCamera(ref _rightCameraCs);
                    }
                    break;
                case 2://如果为后侧相机
                    if (_backCameraCs.hv_AcqHandle == null)
                    {
                        MyTool.TxtFileProcess.CreateLog("从手动设置界面请求打开后侧相机！" + "----用户：" + _operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

                        _backCameraGrab = false;
                        _backCameraVideo = false;
                        Thread.Sleep(100);//等待
                        if (!OpenCamera(ref _backCameraCs, SettingsAdministratorForm._backCameraName, false,true))
                        {
                            MyTool.TxtFileProcess.CreateLog("打开后侧相机失败！" + "----用户：" + _operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

                            MessageBox.Show("打开后侧相机失败");
                        }
                    }
                    else
                    {
                        MyTool.TxtFileProcess.CreateLog("从手动设置界面请求关闭后侧相机！" + "----用户：" + _operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

                        _backCameraGrab = false;
                        _backCameraVideo = false;
                        Thread.Sleep(100);//等待
                        CloseCamera(ref _backCameraCs);
                    }
                    break;
            }
        }

        //maintenance camera form请求抓取图像并显示
        private void MaintenanceCameraFormRequestGrabImageAndDispEventFunc(int cameraIndex, bool dispCrossLineFlag)
        {
            string tempLogStr = "手动设置界面";
            if (_baseSettingFormVariate._settingsCameraCalibrationVariate.Visible)
                tempLogStr = "相机标定界面";
            switch (cameraIndex)
            {
                case 0://如果为左侧相机
                    MyTool.TxtFileProcess.CreateLog(tempLogStr + "请求左侧相机抓取图像！" + "----用户：" + _operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

                    _leftCameraVideo = false;
                    Thread.Sleep(100);//等待相机抓取图像及显示线程运行完一个周期以上
                    _leftCameraGrab = true;
                    break;
                case 1://如果为右侧相机
                    MyTool.TxtFileProcess.CreateLog(tempLogStr + "请求右侧相机抓取图像！" + "----用户：" + _operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

                    _rightCameraVideo = false;
                    Thread.Sleep(100);//等待相机抓取图像及显示线程运行完一个周期以上
                    _rightCameraGrab = true;
                    break;
                case 2://如果为后侧相机
                    MyTool.TxtFileProcess.CreateLog(tempLogStr + "请求后侧相机抓取图像！" + "----用户：" + _operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

                    _backCameraVideo = false;
                    Thread.Sleep(100);//等待相机抓取图像及显示线程运行完一个周期以上
                    _backCamraDispCrossLineFlag = dispCrossLineFlag;
                    _backCameraGrab = true;
                    _baseSettingFormVariate._maintenanceCameraFormVariate.chkDisCrossLine.Checked = dispCrossLineFlag;
                    _baseSettingFormVariate._settingsCameraCalibrationVariate.chkDisCrossLine.Checked = dispCrossLineFlag;
                    break;
            }
        }

        //maintenance camera form请求抓取图像并实时显示
        private void MaintenanceCameraFormRequestGrabImageAndDispVideoEventFunc(int cameraIndex, bool dispCrossLineFlag)
        {
            string tempLogStr = "手动设置界面";
            if (_baseSettingFormVariate._settingsCameraCalibrationVariate.Visible)
                tempLogStr = "相机标定界面";

            switch (cameraIndex)
            {
                case 0://如果为左侧相机
                    if (_leftCameraVideo)
                        MyTool.TxtFileProcess.CreateLog(tempLogStr + "请求左侧相机停止实时显示！" + "----用户：" + _operatorName,
                        BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                    else
                        MyTool.TxtFileProcess.CreateLog(tempLogStr + "请求左侧相机启动实时显示！" + "----用户：" + _operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

                    _leftCameraGrab = false;
                    Thread.Sleep(100);//等待相机抓取图像及显示线程运行完一个周期以上
                    _leftCameraVideo = !_leftCameraVideo;
                    break;
                case 1://如果为右侧相机
                    if (_rightCameraVideo)
                        MyTool.TxtFileProcess.CreateLog(tempLogStr + "请求右侧相机停止实时显示！" + "----用户：" + _operatorName,
                        BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                    else
                        MyTool.TxtFileProcess.CreateLog(tempLogStr + "请求右侧相机启动实时显示！" + "----用户：" + _operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

                    _rightCameraGrab = false;
                    Thread.Sleep(100);//等待相机抓取图像及显示线程运行完一个周期以上
                    _rightCameraVideo = !_rightCameraVideo;
                    break;
                case 2://如果为后侧相机
                    if (_backCameraVideo)
                        MyTool.TxtFileProcess.CreateLog(tempLogStr + "请求后侧相机停止实时显示！" + "----用户：" + _operatorName,
                        BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                    else
                        MyTool.TxtFileProcess.CreateLog(tempLogStr + "请求后侧相机启动实时显示！" + "----用户：" + _operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

                    _backCameraGrab = false;
                    Thread.Sleep(100);//等待相机抓取图像及显示线程运行完一个周期以上
                    _backCamraDispCrossLineFlag = dispCrossLineFlag;
                    _backCameraVideo = !_backCameraVideo;

                    _baseSettingFormVariate._maintenanceCameraFormVariate.chkDisCrossLine.Checked = dispCrossLineFlag;
                    _baseSettingFormVariate._settingsCameraCalibrationVariate.chkDisCrossLine.Checked = dispCrossLineFlag;
                    break;
                case 3://如果为显示十字线与否发生变化
                    if (dispCrossLineFlag)
                        MyTool.TxtFileProcess.CreateLog(tempLogStr + "请求后侧相机显示十字线！" + "----用户：" + _operatorName,
                        BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                    else
                        MyTool.TxtFileProcess.CreateLog(tempLogStr + "请求后侧相机隐藏十字线！" + "----用户：" + _operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

                    _backCamraDispCrossLineFlag = dispCrossLineFlag;

                    _baseSettingFormVariate._maintenanceCameraFormVariate.chkDisCrossLine.Checked = dispCrossLineFlag;
                    _baseSettingFormVariate._settingsCameraCalibrationVariate.chkDisCrossLine.Checked = dispCrossLineFlag;
                    break;
            }
        }

        //maintenance camera form请求保存图片
        private void MaintenanceCameraFormRequestSaveImageEventFunc(int cameraIndex)
        {
            switch (cameraIndex)
            {
                case 0:
                    MyTool.TxtFileProcess.CreateLog("从手动设置界面请求左侧相机保存抓取到的图片！" + "----用户：" + _operatorName,
                        BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                    break;
                case 1:
                    MyTool.TxtFileProcess.CreateLog("从手动设置界面请求右侧相机保存抓取到的图片！" + "----用户：" + _operatorName,
                        BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                    break;
                case 2:
                    MyTool.TxtFileProcess.CreateLog("从手动设置界面请求后侧相机保存抓取到的图片！" + "----用户：" + _operatorName,
                        BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                    break;
            }


            if (cameraIndex == 0 && _leftCameraCs.hv_AcqHandle == null ||
                cameraIndex == 1 && _rightCameraCs.hv_AcqHandle == null ||
                cameraIndex == 2 && _backCameraCs.hv_AcqHandle == null)
            {
                MyTool.TxtFileProcess.CreateLog("从手动设置界面请求相机保存抓取到的图片失败，相机未打开！" + "----用户：" + _operatorName,
                        BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

                MessageBox.Show("请打开相机！");
                return;
            }
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = "保存相机采集图像";
            sfd.Filter = "jpg(*.jpg)|*.jpg";
            sfd.RestoreDirectory = true;
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                switch (cameraIndex)
                {
                    case 0://如果为左侧相机
                        _leftCameraGrab = false;
                        _leftCameraVideo = false;
                        Thread.Sleep(100);//等待相机抓取图像及显示线程运行完一个周期以上
                        if (HalconCameraControl.SaveImage(_leftCameraCs.ho_Image, "jpg", sfd.FileName.Substring(0, sfd.FileName.LastIndexOf("."))))//默认的filename路径为带后缀的
                        {
                            MyTool.TxtFileProcess.CreateLog("从手动设置界面请求左侧相机保存抓取到的图片成功，保存路径为" + sfd.FileName + "----用户：" + _operatorName,
                        BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                            MessageBox.Show("保存左侧相机图像成功！");
                        }
                        else
                        {
                            MyTool.TxtFileProcess.CreateLog("从手动设置界面请求左侧相机保存抓取到的图片失败！" + "----用户：" + _operatorName,
                        BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                            MessageBox.Show("保存左侧相机图像失败！");
                        }
                        break;
                    case 1://如果为右侧相机
                        _rightCameraGrab = false;
                        _rightCameraVideo = false;
                        Thread.Sleep(100);//等待相机抓取图像及显示线程运行完一个周期以上
                        if (HalconCameraControl.SaveImage(_rightCameraCs.ho_Image, "jpg", sfd.FileName.Substring(0, sfd.FileName.LastIndexOf("."))))//默认的filename路径为带后缀的
                        {
                            MyTool.TxtFileProcess.CreateLog("从手动设置界面请求右侧相机保存抓取到的图片成功，保存路径为" + sfd.FileName + "----用户：" + _operatorName,
                        BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                            MessageBox.Show("保存右侧相机图像成功！");
                        }
                        else
                        {
                            MyTool.TxtFileProcess.CreateLog("从手动设置界面请求右侧相机保存抓取到的图片失败！" + "----用户：" + _operatorName,
                        BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                            MessageBox.Show("保存右侧相机图像失败！");
                        }
                        break;
                    case 2://如果为后侧相机
                        _backCameraGrab = false;
                        _backCameraVideo = false;
                        Thread.Sleep(100);//等待相机抓取图像及显示线程运行完一个周期以上
                        if (HalconCameraControl.SaveImage(_backCameraCs.ho_Image, "jpg", sfd.FileName.Substring(0, sfd.FileName.LastIndexOf("."))))//默认的filename路径为带后缀的
                        {
                            MyTool.TxtFileProcess.CreateLog("从手动设置界面请求后侧相机保存抓取到的图片成功，保存路径为" + sfd.FileName + "----用户：" + _operatorName,
                        BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                            MessageBox.Show("保存后侧相机图像成功！");
                        }
                        else
                        {
                            MyTool.TxtFileProcess.CreateLog("从手动设置界面请求后侧相机保存抓取到的图片失败！" + "----用户：" + _operatorName,
                        BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                            MessageBox.Show("保存后侧相机图像失败！");
                        }
                        break;
                }
            }
        }

        //maintenance camera form请求后相机扫描二维码保存图片
        private void MaintenanceCameraFormRequestBackCameraScanBarcodeFunc()
        {
            MyTool.TxtFileProcess.CreateLog("从手动设置界面请求扫码！" + "----用户：" + _operatorName,
                        BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
            if (_backCameraCs.hv_AcqHandle == null)
            {
                MyTool.TxtFileProcess.CreateLog("从手动设置界面请求扫码失败，未打开相机！" + "----用户：" + _operatorName,
                        BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                MessageBox.Show("请打开相机！");
                return;
            }

            _backCameraGrab = false;
            _backCameraVideo = false;
            Thread.Sleep(100);//等待相机抓取图像及显示线程运行完一个周期以上

            if (_backCameraCs.ho_Image == null)
            {
                MyTool.TxtFileProcess.CreateLog("从手动设置界面请求扫码失败，未采集图像！" + "----用户：" + _operatorName,
                        BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                MessageBox.Show("请采集图像！");
                return;
            }
            string barcodeData = "";
            HObject ho_resultRectangle = null;
            if (HalconPictureProcess.BarcodeAcquire(_backCameraCs.ho_Image, out barcodeData, out ho_resultRectangle))
            {
                HalconCameraControl.DispImageAdaptively(_baseSettingFormVariate._maintenanceCameraFormVariate.picBackCameraImageDisp,
                    _backCameraCs.ho_Image, false);
                HOperatorSet.DispText(_baseSettingFormVariate._maintenanceCameraFormVariate.picBackCameraImageDisp.HalconWindow, barcodeData, "window", "top",
                "left", "black", new HTuple(), new HTuple());
                HOperatorSet.DispObj(ho_resultRectangle, _baseSettingFormVariate._maintenanceCameraFormVariate.picBackCameraImageDisp.HalconWindow);
                MyTool.TxtFileProcess.CreateLog("从手动设置界面请求扫码成功，扫出的码为：" + barcodeData + "----用户：" + _operatorName,
                        BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
            }
            else
            {
                HalconCameraControl.DispImageAdaptively(_baseSettingFormVariate._maintenanceCameraFormVariate.picBackCameraImageDisp,
                    _backCameraCs.ho_Image, false);
                HOperatorSet.DispText(_baseSettingFormVariate._maintenanceCameraFormVariate.picBackCameraImageDisp.HalconWindow, "获取条形码数据失败！", "window", "top",
                "left", "black", new HTuple(), new HTuple());
                MyTool.TxtFileProcess.CreateLog("从手动设置界面请求扫码失败，未能获取到条形码数据！" + "----用户：" + _operatorName,
                       BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
            }

        }

        #endregion

        #region TimerUseInSoftCountThreadFunc:计时器线程

        private void TimerUseInSoftCountThreadFunc()
        {
            #region IO T
            for (int i = 0; i < _maxIOQuantity; i++)
            {
                _inputIOT[i].SETTIME = 10;//统一设置为输入触发为10ms
                _inputIOT[i].CURRENTTIME = 0;
                _outputIOT[i].SETTIME = 10;//统一设置为输出触发为10ms
                _outputIOT[i].CURRENTTIME = 0;
            }
            #endregion

            #region _airPressureLowAlarmT:气压低报警超时计时器 初始化
            _airPressureLowAlarmT.SETTIME = 1000;//超时时间1s
            _airPressureLowAlarmT.CURRENTTIME = 0;
            #endregion

            #region _cameraMoveCylinderBackAlarmT:相机移动气缸回超时报警计时器 初始化
            _cameraMoveCylinderBackAlarmT.SETTIME = 3000;//超时时间3s
            _cameraMoveCylinderBackAlarmT.CURRENTTIME = 0;
            #endregion

            #region _cameraMoveCylinderOutAlarmT:相机移动气缸出超时报警计时器 初始化
            _cameraMoveCylinderOutAlarmT.SETTIME = 3000;//超时时间3s
            _cameraMoveCylinderOutAlarmT.CURRENTTIME = 0;
            #endregion

            #region _conveyor1StopCylinderDownAlarmT:传送线1(里侧)阻挡气缸下超时报警计时器 初始化
            _conveyor1StopCylinderDownAlarmT.SETTIME = 3000;//超时时间3s
            _conveyor1StopCylinderDownAlarmT.CURRENTTIME = 0;
            #endregion

            #region _conveyor1StopCylinderUpAlarmT:传送线1(里侧)阻挡气缸上超时报警计时器  初始化
            _conveyor1StopCylinderUpAlarmT.SETTIME = 3000;//超时时间3s
            _conveyor1StopCylinderUpAlarmT.CURRENTTIME = 0;
            #endregion

            #region _conveyor1ClampCylinderReleaseAlarmT:传送线1(里侧)夹紧气缸松开超时报警计时器  初始化
            _conveyor1ClampCylinderReleaseAlarmT.SETTIME = 3000;//超时时间3s
            _conveyor1ClampCylinderReleaseAlarmT.CURRENTTIME = 0;
            #endregion

            #region _conveyor1ClampCylinderClampAlarmT:传送线1(里侧)夹紧气缸夹紧超时报警计时器   初始化
            _conveyor1ClampCylinderClampAlarmT.SETTIME = 3000;//超时时间3s
            _conveyor1ClampCylinderClampAlarmT.CURRENTTIME = 0;
            #endregion

            #region _conveyor2StopCylinderDownAlarmT:传送线2(外侧)阻挡气缸下超时报警计时器 初始化
            _conveyor2StopCylinderDownAlarmT.SETTIME = 3000;//超时时间3s
            _conveyor2StopCylinderDownAlarmT.CURRENTTIME = 0;
            #endregion

            #region _conveyor2StopCylinderUpAlarmT:传送线2(外侧)阻挡气缸上超时报警计时器  初始化
            _conveyor2StopCylinderUpAlarmT.SETTIME = 3000;//超时时间3s
            _conveyor2StopCylinderUpAlarmT.CURRENTTIME = 0;
            #endregion

            #region _conveyor2ClampCylinderReleaseAlarmT:传送线2(外侧)夹紧气缸松开超时报警计时器  初始化
            _conveyor2ClampCylinderReleaseAlarmT.SETTIME = 3000;//超时时间3s
            _conveyor2ClampCylinderReleaseAlarmT.CURRENTTIME = 0;
            #endregion

            #region _conveyor2ClampCylinderClampAlarmT:传送线2(外侧)夹紧气缸夹紧超时报警计时器   初始化
            _conveyor2ClampCylinderClampAlarmT.SETTIME = 3000;//超时时间3s
            _conveyor2ClampCylinderClampAlarmT.CURRENTTIME = 0;
            #endregion

            #region _changeDeviceToManualModeT:切换设备至手动模式计时器 初始化
            _changeDeviceToManualModeT.SETTIME = 3000;//切换为手动模式延时3s
            _changeDeviceToManualModeT.CURRENTTIME = 0;
            #endregion

            #region _device500TwinkleOnT:设备500ms闪烁ON计时器 初始化
            _device500TwinkleOnT.SETTIME = 500;//设备500ms闪烁计时器延时时间为0.5s
            _device500TwinkleOnT.CURRENTTIME = 0;
            #endregion

            #region _device500TwinkleOffT:设备500ms闪烁OFF计时器 初始化
            _device500TwinkleOffT.SETTIME = 500;//设备500ms闪烁计时器延时时间为0.5s
            _device500TwinkleOffT.CURRENTTIME = 0;
            #endregion

            #region _device200TwinkleOnT:设备200ms闪烁ON计时器 初始化
            _device200TwinkleOnT.SETTIME = 200;//设备200ms闪烁计时器延时时间为0.2s
            _device200TwinkleOnT.CURRENTTIME = 0;
            #endregion

            #region _device200TwinkleOffT:设备200ms闪烁OFF计时器 初始化
            _device200TwinkleOffT.SETTIME = 200;//设备500ms闪烁计时器延时时间为0.2s
            _device200TwinkleOffT.CURRENTTIME = 0;
            #endregion

            #region _xyMoveToAssignedPointPositionModuleOffDlyT:XY移动至指定点位号模块OFF延时计时器 初始化
            _xyMoveToAssignedPointPositionModuleOffDlyT.SETTIME = 10;
            _xyMoveToAssignedPointPositionModuleOffDlyT.CURRENTTIME = 0;
            #endregion

            #region _allAxisHomeBackModuleOffDlyT:整机原点复归模块OFF延时计时器 初始化
            _allAxisHomeBackModuleOffDlyT.SETTIME = 10;
            _allAxisHomeBackModuleOffDlyT.CURRENTTIME = 0;
            #endregion

            #region _axesGotoStandbyModuleOffDlyT:轴移动至等待位置模块OFF延时计时器 初始化
            _axesGotoStandbyModuleOffDlyT.SETTIME = 10;
            _axesGotoStandbyModuleOffDlyT.CURRENTTIME = 0;
            #endregion

            #region _conveyorChangeToAssignedWidthModuleOffDlyT:传送线宽度变换模块OFF延时计时器 初始化
            _conveyorChangeToAssignedWidthModuleOffDlyT.SETTIME = 10;
            _conveyorChangeToAssignedWidthModuleOffDlyT.CURRENTTIME = 0;
            #endregion

            #region _conveyor1ActionModuleOffDlyT:传送线1动作模块OFF延时计时器 初始化
            _conveyor1ActionModuleOffDlyT.SETTIME = 10;
            _conveyor1ActionModuleOffDlyT.CURRENTTIME = 0;
            #endregion

            #region _conveyor2ActionModuleOffDlyT:传送线2动作模块OFF延时计时器 初始化
            _conveyor2ActionModuleOffDlyT.SETTIME = 10;
            _conveyor2ActionModuleOffDlyT.CURRENTTIME = 0;
            #endregion

            #region _deviceStartDelay:设备启动延时 初始化
            _deviceStartDelay.SETTIME = 3000;
            _deviceStartDelay.CURRENTTIME = 0;
            #endregion

            #region _conveyor1FinishLoadT:传送线1上料完成延时 初始化
            _conveyor1FinishLoadT.SETTIME = 100;
            _conveyor1FinishLoadT.CURRENTTIME = 0;
            #endregion

            #region _conveyor1FinishUnloadT:传送线1下料完成延时 初始化
            _conveyor1FinishUnloadT.SETTIME = 100;
            _conveyor1FinishUnloadT.CURRENTTIME = 0;
            #endregion

            #region _conveyor1UnloadStopCylinderUpT:传送线1下料时阻挡气缸上升延时 初始化
            _conveyor1UnloadStopCylinderUpT.SETTIME = 500;
            _conveyor1UnloadStopCylinderUpT.CURRENTTIME = 0;
            #endregion

            #region _conveyor1LoadTimeOutT:传送线1上料超时延时计时器，用于判定是否上料超时 初始化
            _conveyor1LoadTimeOutT.SETTIME = 500;
            _conveyor1LoadTimeOutT.CURRENTTIME = 0;
            #endregion

            #region _conveyor1UnloadTimeOutT:传送线1下料超时延时计时器，用于判定是否下料超时 初始化
            _conveyor1UnloadTimeOutT.SETTIME = 500;
            _conveyor1UnloadTimeOutT.CURRENTTIME = 0;
            #endregion

            #region _conveyor2FinishLoadT:传送线2上料完成延时 初始化
            _conveyor2FinishLoadT.SETTIME = 100;
            _conveyor2FinishLoadT.CURRENTTIME = 0;
            #endregion

            #region _conveyor2FinishUnloadT:传送线2下料完成延时 初始化
            _conveyor2FinishUnloadT.SETTIME = 100;
            _conveyor2FinishUnloadT.CURRENTTIME = 0;
            #endregion

            #region _conveyor2UnloadStopCylinderUpT:传送线2下料时阻挡气缸上升延时 初始化
            _conveyor2UnloadStopCylinderUpT.SETTIME = 500;
            _conveyor2UnloadStopCylinderUpT.CURRENTTIME = 0;
            #endregion

            #region _conveyor2LoadTimeOutT:传送线2上料超时延时计时器，用于判定是否上料超时 初始化
            _conveyor2LoadTimeOutT.SETTIME = 500;
            _conveyor2LoadTimeOutT.CURRENTTIME = 0;
            #endregion

            #region _conveyor2UnloadTimeOutT:传送线2下料超时延时计时器，用于判定是否下料超时 初始化
            _conveyor2UnloadTimeOutT.SETTIME = 500;
            _conveyor2UnloadTimeOutT.CURRENTTIME = 0;
            #endregion

            #region _conveyor1LoadUnloadModuleOffDlyT:传送线1上下料模块OFF延时计时器 初始化
            _conveyor1LoadUnloadModuleOffDlyT.SETTIME = 10;
            _conveyor1LoadUnloadModuleOffDlyT.CURRENTTIME = 0;
            #endregion

            #region _conveyor2LoadUnloadModuleOffDlyT:传送线2上下料模块OFF延时计时器 初始化
            _conveyor2LoadUnloadModuleOffDlyT.SETTIME = 10;
            _conveyor2LoadUnloadModuleOffDlyT.CURRENTTIME = 0;
            #endregion

            #region _barcodeScanModuleOffDlyT:传送线1动作模块OFF延时计时器 初始化
            _barcodeScanModuleOffDlyT.SETTIME = 10;
            _barcodeScanModuleOffDlyT.CURRENTTIME = 0;
            #endregion

            #region _getOrSendProductInfoFromMESModuleOffDlyT:获取或传输数据至MES模块OFF延时计时器 初始化
            _getOrSendProductInfoFromMESModuleOffDlyT.SETTIME = 10;
            _getOrSendProductInfoFromMESModuleOffDlyT.CURRENTTIME = 0;
            #endregion

            #region _deviceRecognizeMarkPositionAndGetCorrectMatrixModuleOffDlyT:设备识别MARK点并计算出纠正矩阵模块OFF延时计时器 初始化
            _deviceRecognizeMarkPositionAndGetCorrectMatrixModuleOffDlyT.SETTIME = 10;
            _deviceRecognizeMarkPositionAndGetCorrectMatrixModuleOffDlyT.CURRENTTIME = 0;
            #endregion

            #region _deviceTotalWorkModuleOffDlyT:设备整体工作模块OFF延时计时器 初始化
            _deviceTotalWorkModuleOffDlyT.SETTIME = 10;
            _deviceTotalWorkModuleOffDlyT.CURRENTTIME = 0;
            #endregion

            #region _autoModuleOffDlyT:自动模块OFF延时计时器 初始化
            _autoModuleOffDlyT.SETTIME = 10;
            _autoModuleOffDlyT.CURRENTTIME = 0;
            #endregion

            #region _leftAndRightCameraGrabImageModuleOffDlyT:左右相机采集模块OFF延时计时器 初始化
            _leftAndRightCameraGrabImageModuleOffDlyT.SETTIME = 10;
            _leftAndRightCameraGrabImageModuleOffDlyT.CURRENTTIME = 0;
            #endregion

            #region _leftCameraImageInferModuleOffDlyT:左侧相机采集图片推断模块OFF延时计时器 初始化
            _leftCameraImageInferModuleOffDlyT.SETTIME = 10;
            _leftCameraImageInferModuleOffDlyT.CURRENTTIME = 0;
            #endregion

            #region _rightCameraImageInferModuleOffDlyT:右侧相机采集图片推断模块OFF延时计时器 初始化
            _rightCameraImageInferModuleOffDlyT.SETTIME = 10;
            _rightCameraImageInferModuleOffDlyT.CURRENTTIME = 0;
            #endregion

            #region _leftCameraSourceImageSaveModuleOffDlyT:左侧相机采集原始图片保存模块OFF延时计时器 初始化
            _leftCameraSourceImageSaveModuleOffDlyT.SETTIME = 10;
            _leftCameraSourceImageSaveModuleOffDlyT.CURRENTTIME = 0;
            #endregion

            #region _rightCameraSourceImageSaveModuleOffDlyT:右侧相机采集原始图片保存模块OFF延时计时器 初始化
            _rightCameraSourceImageSaveModuleOffDlyT.SETTIME = 10;
            _rightCameraSourceImageSaveModuleOffDlyT.CURRENTTIME = 0;
            #endregion

            #region _leftCameraInferredImageSaveModuleOffDlyT:左侧相机采集图片的推断结果图片保存模块OFF延时计时器 初始化
            _leftCameraInferredImageSaveModuleOffDlyT.SETTIME = 10;
            _leftCameraInferredImageSaveModuleOffDlyT.CURRENTTIME = 0;
            #endregion

            #region _rightCameraInferredImageSaveModuleOffDlyT:左侧相机采集图片的推断结果图片保存模块OFF延时计时器 初始化
            _rightCameraInferredImageSaveModuleOffDlyT.SETTIME = 10;
            _rightCameraInferredImageSaveModuleOffDlyT.CURRENTTIME = 0;
            #endregion

            #region _cameraInferDataClearModuleOffDlyT:相机推断用数据清除模块OFF延时计时器 初始化
            _cameraInferDataClearModuleOffDlyT.SETTIME = 10;
            _cameraInferDataClearModuleOffDlyT.CURRENTTIME = 0;
            #endregion

            while (true)
            {
                if (_axisControlFormVariate != null)
                {
                    #region 启动按钮
                    //如果按下实体启动按钮或虚拟启动按钮
                    if ((_axisControlFormVariate._ioStatus[0, START_BT_IN_IO_NO] || _virtualAndRealBtnIndex == 10)
                        && !_inputIOT[START_BT_IN_IO_NO].STATUS)
                    {
                        _inputIOT[START_BT_IN_IO_NO].CURRENTTIME += TIME_COUNT_TIME_INVERTAL;//每次加10ms
                    }
                    else if (!_axisControlFormVariate._ioStatus[0, START_BT_IN_IO_NO] && _virtualAndRealBtnIndex != 10)
                    {
                        _inputIOT[START_BT_IN_IO_NO].CURRENTTIME = 0;//如果为OFF,那么直接置为0
                    }
                    #endregion

                    #region 停止按钮
                    //如果按下实体停止按钮或虚拟停止按钮
                    if ((_axisControlFormVariate._ioStatus[0, STOP_BT_IN_IO_NO] || _virtualAndRealBtnIndex == 11)
                        && !_inputIOT[STOP_BT_IN_IO_NO].STATUS)
                    {
                        _inputIOT[STOP_BT_IN_IO_NO].CURRENTTIME += TIME_COUNT_TIME_INVERTAL;//每次加10ms
                    }
                    else if (!_axisControlFormVariate._ioStatus[0, STOP_BT_IN_IO_NO] && _virtualAndRealBtnIndex != 11)
                    {
                        _inputIOT[STOP_BT_IN_IO_NO].CURRENTTIME = 0;//如果为OFF,那么直接置为0
                    }
                    #endregion

                    #region 复位按钮
                    //如果按下实体复位按钮或虚拟复位按钮
                    if ((_axisControlFormVariate._ioStatus[0, RESET_BT_IN_IO_NO] || _virtualAndRealBtnIndex == 12)
                        && !_inputIOT[RESET_BT_IN_IO_NO].STATUS)
                    {
                        _inputIOT[RESET_BT_IN_IO_NO].CURRENTTIME += TIME_COUNT_TIME_INVERTAL;//每次加10ms
                    }
                    else if (!_axisControlFormVariate._ioStatus[0, RESET_BT_IN_IO_NO] && _virtualAndRealBtnIndex != 12)
                    {
                        _inputIOT[RESET_BT_IN_IO_NO].CURRENTTIME = 0;//如果为OFF,那么直接置为0
                    }
                    #endregion

                    #region 加工启动按钮
                    //如果按下实体加工启动按钮或虚拟加工启动按钮
                    if ((_axisControlFormVariate._ioStatus[0, MEASURE_START_BT_IN_IO_NO] || _virtualAndRealBtnIndex == 2)
                        && !_inputIOT[MEASURE_START_BT_IN_IO_NO].STATUS)
                    {
                        _inputIOT[MEASURE_START_BT_IN_IO_NO].CURRENTTIME += TIME_COUNT_TIME_INVERTAL;//每次加10ms
                    }
                    else if (!_axisControlFormVariate._ioStatus[0, MEASURE_START_BT_IN_IO_NO] && _virtualAndRealBtnIndex != 2)
                    {
                        _inputIOT[MEASURE_START_BT_IN_IO_NO].CURRENTTIME = 0;//如果为OFF,那么直接置为0
                    }
                    #endregion

                    #region IO
                    for (int i = 0; i < _maxIOQuantity; i++)
                    {
                        if (i != START_BT_IN_IO_NO && i != STOP_BT_IN_IO_NO &&
                            i != RESET_BT_IN_IO_NO && i != MEASURE_START_BT_IN_IO_NO)
                        {
                            if (_axisControlFormVariate._ioStatus[0, i] && !_inputIOT[i].STATUS)
                            {
                                _inputIOT[i].CURRENTTIME += TIME_COUNT_TIME_INVERTAL;//每次加10ms
                            }
                            else if (!_axisControlFormVariate._ioStatus[0, i])
                            {
                                _inputIOT[i].CURRENTTIME = 0;//如果为OFF,那么直接置为0
                            }
                        }

                        if (_axisControlFormVariate._ioStatus[1, i] && !_outputIOT[i].STATUS)
                        {
                            _outputIOT[i].CURRENTTIME += TIME_COUNT_TIME_INVERTAL;//每次加10ms
                        }
                        else if (!_axisControlFormVariate._ioStatus[1, i])
                        {
                            _outputIOT[i].CURRENTTIME = 0;//如果为OFF,那么直接置为0
                        }
                    }
                    #endregion

                    #region _airPressureLowAlarmT:气压低报警超时计时器
                    if (!_axisControlFormVariate._ioStatus[0, AIR_PRESSURE_IN_IO] && !_airPressureLowAlarmT.STATUS)
                    {
                        _airPressureLowAlarmT.CURRENTTIME += TIME_COUNT_TIME_INVERTAL;
                    }
                    else if (_axisControlFormVariate._ioStatus[0, AIR_PRESSURE_IN_IO])
                    {
                        _airPressureLowAlarmT.CURRENTTIME = 0;
                    }
                    #endregion

                    #region _cameraMoveCylinderBackAlarmT:相机移动气缸回超时报警计时器
                    if (_outputIOT[4].STATUS && !_inputIOT[13].STATUS && !_cameraMoveCylinderBackAlarmT.STATUS)
                    {
                        _cameraMoveCylinderBackAlarmT.CURRENTTIME += TIME_COUNT_TIME_INVERTAL;
                    }
                    else if ((_outputIOT[4].STATUS && _inputIOT[13].STATUS) || !_outputIOT[4].STATUS)
                    {
                        _cameraMoveCylinderBackAlarmT.CURRENTTIME = 0;
                    }
                    #endregion

                    #region _cameraMoveCylinderOutAlarmT:相机移动气缸出超时报警计时器
                    if (!_outputIOT[4].STATUS && !_inputIOT[14].STATUS && !_cameraMoveCylinderOutAlarmT.STATUS)
                    {
                        _cameraMoveCylinderOutAlarmT.CURRENTTIME += TIME_COUNT_TIME_INVERTAL;
                    }
                    else if ((!_outputIOT[4].STATUS && _inputIOT[14].STATUS) || _outputIOT[4].STATUS)
                    {
                        _cameraMoveCylinderOutAlarmT.CURRENTTIME = 0;
                    }
                    #endregion

                    #region _conveyor1StopCylinderDownAlarmT:传送线1(外侧)阻挡气缸下超时报警计时器
                    if (!_outputIOT[81].STATUS && !_inputIOT[84].STATUS && !_conveyor1StopCylinderDownAlarmT.STATUS)
                    {
                        _conveyor1StopCylinderDownAlarmT.CURRENTTIME += TIME_COUNT_TIME_INVERTAL;
                    }
                    else if ((!_outputIOT[81].STATUS && _inputIOT[84].STATUS) || _outputIOT[81].STATUS)
                    {
                        _conveyor1StopCylinderDownAlarmT.CURRENTTIME = 0;
                    }
                    #endregion

                    #region _conveyor1StopCylinderUpAlarmT:传送线1(里侧)阻挡气缸上超时报警计时器
                    if (_outputIOT[81].STATUS && !_inputIOT[85].STATUS && !_conveyor1StopCylinderUpAlarmT.STATUS)
                    {
                        _conveyor1StopCylinderUpAlarmT.CURRENTTIME += TIME_COUNT_TIME_INVERTAL;
                    }
                    else if ((_outputIOT[81].STATUS && _inputIOT[85].STATUS) || !_outputIOT[81].STATUS)
                    {
                        _conveyor1StopCylinderUpAlarmT.CURRENTTIME = 0;
                    }
                    #endregion

                    #region _conveyor1ClampCylinderReleaseAlarmT:传送线1(里侧)夹紧气缸松开超时报警计时器
                    if (!_outputIOT[83].STATUS && !_inputIOT[86].STATUS && !_conveyor1ClampCylinderReleaseAlarmT.STATUS)
                    {
                        _conveyor1ClampCylinderReleaseAlarmT.CURRENTTIME += TIME_COUNT_TIME_INVERTAL;
                    }
                    else if ((!_outputIOT[83].STATUS && _inputIOT[86].STATUS) || _outputIOT[83].STATUS)
                    {
                        _conveyor1ClampCylinderReleaseAlarmT.CURRENTTIME = 0;
                    }
                    #endregion

                    #region _conveyor1ClampCylinderClampAlarmT:传送线1(里侧)夹紧气缸夹紧超时报警计时器
                    if (_outputIOT[83].STATUS && !_inputIOT[87].STATUS && !_conveyor1ClampCylinderClampAlarmT.STATUS)
                    {
                        _conveyor1ClampCylinderClampAlarmT.CURRENTTIME += TIME_COUNT_TIME_INVERTAL;
                    }
                    else if ((_outputIOT[83].STATUS && _inputIOT[87].STATUS) || !_outputIOT[83].STATUS)
                    {
                        _conveyor1ClampCylinderClampAlarmT.CURRENTTIME = 0;
                    }
                    #endregion

                    #region _conveyor2StopCylinderDownAlarmT:传送线2(外侧)阻挡气缸下超时报警计时器
                    if (!_outputIOT[77].STATUS && !_inputIOT[80].STATUS && !_conveyor2StopCylinderDownAlarmT.STATUS)
                    {
                        _conveyor2StopCylinderDownAlarmT.CURRENTTIME += TIME_COUNT_TIME_INVERTAL;
                    }
                    else if ((!_outputIOT[77].STATUS && _inputIOT[80].STATUS) || _outputIOT[77].STATUS)
                    {
                        _conveyor2StopCylinderDownAlarmT.CURRENTTIME = 0;
                    }
                    #endregion

                    #region _conveyor2StopCylinderUpAlarmT:传送线2(外侧)阻挡气缸上超时报警计时器
                    if (_outputIOT[77].STATUS && !_inputIOT[81].STATUS && !_conveyor2StopCylinderUpAlarmT.STATUS)
                    {
                        _conveyor2StopCylinderUpAlarmT.CURRENTTIME += TIME_COUNT_TIME_INVERTAL;
                    }
                    else if ((_outputIOT[77].STATUS && _inputIOT[81].STATUS) || !_outputIOT[77].STATUS)
                    {
                        _conveyor2StopCylinderUpAlarmT.CURRENTTIME = 0;
                    }
                    #endregion

                    #region _conveyor2ClampCylinderReleaseAlarmT:传送线2(外侧)夹紧气缸松开超时报警计时器
                    if (!_outputIOT[79].STATUS && !_inputIOT[82].STATUS && !_conveyor2ClampCylinderReleaseAlarmT.STATUS)
                    {
                        _conveyor2ClampCylinderReleaseAlarmT.CURRENTTIME += TIME_COUNT_TIME_INVERTAL;
                    }
                    else if ((!_outputIOT[79].STATUS && _inputIOT[82].STATUS) || _outputIOT[79].STATUS)
                    {
                        _conveyor2ClampCylinderReleaseAlarmT.CURRENTTIME = 0;
                    }
                    #endregion

                    #region _conveyor2ClampCylinderClampAlarmT:传送线2(外侧)夹紧气缸夹紧超时报警计时器
                    if (_outputIOT[79].STATUS && !_inputIOT[83].STATUS && !_conveyor2ClampCylinderClampAlarmT.STATUS)
                    {
                        _conveyor2ClampCylinderClampAlarmT.CURRENTTIME += TIME_COUNT_TIME_INVERTAL;
                    }
                    else if ((_outputIOT[79].STATUS && _inputIOT[83].STATUS) || !_outputIOT[79].STATUS)
                    {
                        _conveyor2ClampCylinderClampAlarmT.CURRENTTIME = 0;
                    }
                    #endregion

                }

                #region _changeDeviceToManualModeT:切换设备至手动模式计时器
                if (_virtualAndRealBtnIndex == 1 && !_changeDeviceToManualModeT.STATUS)
                {
                    _changeDeviceToManualModeT.CURRENTTIME += TIME_COUNT_TIME_INVERTAL;
                }
                else if (_virtualAndRealBtnIndex != 1)
                {
                    _changeDeviceToManualModeT.CURRENTTIME = 0;
                }
                #endregion

                #region _device500TwinkleOnT:设备500ms闪烁ON计时器
                if (_device500TwinkeOnOffFlag && !_device500TwinkleOnT.STATUS)
                {
                    _device500TwinkleOnT.CURRENTTIME += TIME_COUNT_TIME_INVERTAL;
                }
                else if (!_device500TwinkeOnOffFlag)
                {
                    _device500TwinkleOnT.CURRENTTIME = 0;
                }
                #endregion

                #region _device500TwinkleOffT:设备500ms闪烁OFF计时器
                if (!_device500TwinkeOnOffFlag && !_device500TwinkleOffT.STATUS)
                {
                    _device500TwinkleOffT.CURRENTTIME += TIME_COUNT_TIME_INVERTAL;
                }
                else if (_device500TwinkeOnOffFlag)
                {
                    _device500TwinkleOffT.CURRENTTIME = 0;
                }
                #endregion

                #region _device200TwinkleOnT:设备200ms闪烁ON计时器
                if (_device200TwinkeOnOffFlag && !_device200TwinkleOnT.STATUS)
                {
                    _device200TwinkleOnT.CURRENTTIME += TIME_COUNT_TIME_INVERTAL;
                }
                else if (!_device200TwinkeOnOffFlag)
                {
                    _device200TwinkleOnT.CURRENTTIME = 0;
                }
                #endregion

                #region _device200TwinkleOffT:设备200ms闪烁OFF计时器
                if (!_device200TwinkeOnOffFlag && !_device200TwinkleOffT.STATUS)
                {
                    _device200TwinkleOffT.CURRENTTIME += TIME_COUNT_TIME_INVERTAL;
                }
                else if (_device200TwinkeOnOffFlag)
                {
                    _device200TwinkleOffT.CURRENTTIME = 0;
                }
                #endregion

                #region _xyMoveToAssignedPointPositionModuleOffDlyT:XY移动至指定点位号模块OFF延时计时器
                if (_xyMoveToAssignedPointPositionModuleActionFlag != 0 && !_xyMoveToAssignedPointPositionModuleOffDlyT.STATUS)
                {
                    _xyMoveToAssignedPointPositionModuleOffDlyT.CURRENTTIME += TIME_COUNT_TIME_INVERTAL;
                }
                else if (_xyMoveToAssignedPointPositionModuleActionFlag == 0)
                {
                    _xyMoveToAssignedPointPositionModuleOffDlyT.CURRENTTIME = 0;
                }
                #endregion

                #region _allAxisHomeBackModuleOffDlyT:整机原点复归模块OFF延时计时器
                if (_allAxisHomeBackModuleActionFlag != 0 && !_allAxisHomeBackModuleOffDlyT.STATUS)
                {
                    _allAxisHomeBackModuleOffDlyT.CURRENTTIME += TIME_COUNT_TIME_INVERTAL;
                }
                else if (_allAxisHomeBackModuleActionFlag == 0)
                {
                    _allAxisHomeBackModuleOffDlyT.CURRENTTIME = 0;
                }
                #endregion

                #region _axesGotoStandbyModuleOffDlyT:轴移动至等待位置模块OFF延时计时器
                if (_axesGotoStandbyModuleActionFlag != 0 && !_axesGotoStandbyModuleOffDlyT.STATUS)
                {
                    _axesGotoStandbyModuleOffDlyT.CURRENTTIME += TIME_COUNT_TIME_INVERTAL;
                }
                else if (_axesGotoStandbyModuleActionFlag == 0)
                {
                    _axesGotoStandbyModuleOffDlyT.CURRENTTIME = 0;
                }
                #endregion

                #region _conveyorChangeToAssignedWidthModuleOffDlyT:传送线宽度变换模块OFF延时计时器
                if (_conveyorChangeToAssignedWidthModuleActionFlag != 0 && !_conveyorChangeToAssignedWidthModuleOffDlyT.STATUS)
                {
                    _conveyorChangeToAssignedWidthModuleOffDlyT.CURRENTTIME += TIME_COUNT_TIME_INVERTAL;
                }
                else if (_conveyorChangeToAssignedWidthModuleActionFlag == 0)
                {
                    _conveyorChangeToAssignedWidthModuleOffDlyT.CURRENTTIME = 0;
                }
                #endregion

                #region _conveyor1ActionModuleOffDlyT:传送线1动作模块OFF延时计时器
                if (_conveyor1ActionModuleActionFlag != 0 && !_conveyor1ActionModuleOffDlyT.STATUS)
                {
                    _conveyor1ActionModuleOffDlyT.CURRENTTIME += TIME_COUNT_TIME_INVERTAL;
                }
                else if (_conveyor1ActionModuleActionFlag == 0)
                {
                    _conveyor1ActionModuleOffDlyT.CURRENTTIME = 0;
                }
                #endregion

                #region _conveyor2ActionModuleOffDlyT:传送线2动作模块OFF延时计时器
                if (_conveyor2ActionModuleActionFlag != 0 && !_conveyor2ActionModuleOffDlyT.STATUS)
                {
                    _conveyor2ActionModuleOffDlyT.CURRENTTIME += TIME_COUNT_TIME_INVERTAL;
                }
                else if (_conveyor2ActionModuleActionFlag == 0)
                {
                    _conveyor2ActionModuleOffDlyT.CURRENTTIME = 0;
                }
                #endregion

                #region _deviceStartDelay:设备启动延时
                if (!_deviceStartDelay.STATUS)
                {
                    _deviceStartDelay.CURRENTTIME += TIME_COUNT_TIME_INVERTAL;
                }
                #endregion

                #region _conveyor1FinishLoadT:传送线1上料完成延时
                if (_productParameter[0] != null)
                {
                    _conveyor1FinishLoadT.SETTIME = (int)(_productParameter[0]._conveyor1LoadFinishTime * 1000);
                }
                if (_axisControlFormVariate._ioStatus[0, 89] && !_conveyor1FinishLoadT.STATUS)
                {
                    _conveyor1FinishLoadT.CURRENTTIME += TIME_COUNT_TIME_INVERTAL;
                }
                else if (!_axisControlFormVariate._ioStatus[0, 89])
                {
                    _conveyor1FinishLoadT.CURRENTTIME = 0;
                }
                #endregion

                #region _conveyor1FinishUnloadT:传送线1下料完成延时

                if (_productParameter[0] != null)
                {
                    _conveyor1FinishUnloadT.SETTIME = (int)(_productParameter[0]._conveyor1UnloadFinishTime * 1000);
                }

                if (!_axisControlFormVariate._ioStatus[0, 93] && !_conveyor1FinishUnloadT.STATUS)
                {
                    _conveyor1FinishUnloadT.CURRENTTIME += TIME_COUNT_TIME_INVERTAL;
                }
                else if (_axisControlFormVariate._ioStatus[0, 93])
                {
                    _conveyor1FinishUnloadT.CURRENTTIME = 0;
                }
                #endregion

                #region _conveyor1UnloadStopCylinderUpT:传送线1下料时阻挡气缸上升延时

                if (_productParameter[0] != null)
                {
                    _conveyor1UnloadStopCylinderUpT.SETTIME = (int)(_productParameter[0]._conveyor1StopCylinderUpTime * 1000);
                }

                if (!_axisControlFormVariate._ioStatus[0, 89] && !_conveyor1UnloadStopCylinderUpT.STATUS)
                {
                    _conveyor1UnloadStopCylinderUpT.CURRENTTIME += TIME_COUNT_TIME_INVERTAL;
                }
                else if (_axisControlFormVariate._ioStatus[0, 89])
                {
                    _conveyor1UnloadStopCylinderUpT.CURRENTTIME = 0;
                }
                #endregion

                #region _conveyor1LoadTimeOutT:传送线1上料超时延时计时器，用于判定是否上料超时
                if (_productParameter[0] != null)
                {
                    _conveyor1LoadTimeOutT.SETTIME = (int)(_productParameter[0]._conveyor1LoadTimeOutTime * 1000);
                }

                if (_autoModeFlag && _conveyor1Status == 0 && _conveyor1ActionModuleActionFlag == 1 && !_conveyor1LoadTimeOutT.STATUS && _conveyor1TimeoutAlarmDealWithWayIndexWhenLoadAndUnload == -1)
                {
                    _conveyor1LoadTimeOutT.CURRENTTIME += TIME_COUNT_TIME_INVERTAL;
                }
                else if (!_autoModeFlag || _conveyor1Status != 0 || _conveyor1ActionModuleActionFlag != 1 ||
                    (_conveyor1LoadTimeOutT.STATUS && _inputIOT[RESET_BT_IN_IO_NO].STATUS))
                {
                    _conveyor1LoadTimeOutT.CURRENTTIME = 0;
                }
                #endregion

                #region _conveyor1UnloadTimeOutT:传送线1下料超时延时计时器，用于判定是否下料超时

                if (_autoModeFlag)
                {
                    if (_productParameter[0] != null)
                    {
                        _conveyor1UnloadTimeOutT.SETTIME = (int)(_productParameter[0]._conveyor1UnloadTimeOutTime * 1000);
                    }
                }
                else
                {
                    if (SettingsProductForm._editProductParameter != null)
                    {
                        _conveyor1UnloadTimeOutT.SETTIME = 10000;//指定10s
                    }
                }

                if ((_autoModeFlag && _conveyor1Status == 2 && _conveyor1ActionModuleActionFlag == 1 && !_conveyor1UnloadTimeOutT.STATUS && _conveyor1TimeoutAlarmDealWithWayIndexWhenLoadAndUnload == -1)
                    || (!_autoModeFlag && _conveyor1ProductForcedUnloadModuleActionFlag == 0 && !_conveyor1UnloadTimeOutT.STATUS && _conveyor1TimeoutAlarmDealWithWayIndexWhenLoadAndUnload == -1))
                {
                    _conveyor1UnloadTimeOutT.CURRENTTIME += TIME_COUNT_TIME_INVERTAL;
                }
                else if ((_autoModeFlag && (_conveyor1Status != 2 || _conveyor1ActionModuleActionFlag != 1)) ||
                   (!_autoModeFlag && _conveyor1ProductForcedUnloadModuleActionFlag != 0) ||
                   (_conveyor1UnloadTimeOutT.STATUS && _inputIOT[RESET_BT_IN_IO_NO].STATUS))
                {
                    _conveyor1UnloadTimeOutT.CURRENTTIME = 0;
                }

                #endregion

                #region _conveyor2FinishLoadT:传送线2上料完成延时
                if (_productParameter[1] != null)
                {
                    _conveyor2FinishLoadT.SETTIME = (int)(_productParameter[1]._conveyor2LoadFinishTime * 1000);
                }
                if (_axisControlFormVariate._ioStatus[0, 88] && !_conveyor2FinishLoadT.STATUS)
                {
                    _conveyor2FinishLoadT.CURRENTTIME += TIME_COUNT_TIME_INVERTAL;
                }
                else if (!_axisControlFormVariate._ioStatus[0, 88])
                {
                    _conveyor2FinishLoadT.CURRENTTIME = 0;
                }
                #endregion

                #region _conveyor2FinishUnloadT:传送线2下料完成延时

                if (_productParameter[1] != null)
                {
                    _conveyor2FinishUnloadT.SETTIME = (int)(_productParameter[1]._conveyor2UnloadFinishTime * 1000);
                }

                if (!_axisControlFormVariate._ioStatus[0, 91] && !_conveyor2FinishUnloadT.STATUS)
                {
                    _conveyor2FinishUnloadT.CURRENTTIME += TIME_COUNT_TIME_INVERTAL;
                }
                else if (_axisControlFormVariate._ioStatus[0, 91])
                {
                    _conveyor2FinishUnloadT.CURRENTTIME = 0;
                }
                #endregion

                #region _conveyor2UnloadStopCylinderUpT:传送线2下料时阻挡气缸上升延时

                if (_productParameter[1] != null)
                {
                    _conveyor2UnloadStopCylinderUpT.SETTIME = (int)(_productParameter[1]._conveyor2StopCylinderUpTime * 1000);
                }

                if (!_axisControlFormVariate._ioStatus[0, 88] && !_conveyor2UnloadStopCylinderUpT.STATUS)
                {
                    _conveyor2UnloadStopCylinderUpT.CURRENTTIME += TIME_COUNT_TIME_INVERTAL;
                }
                else if (_axisControlFormVariate._ioStatus[0, 88])
                {
                    _conveyor2UnloadStopCylinderUpT.CURRENTTIME = 0;
                }
                #endregion

                #region _conveyor2LoadTimeOutT:传送线2上料超时延时计时器，用于判定是否上料超时
                if (_productParameter[1] != null)
                {
                    _conveyor2LoadTimeOutT.SETTIME = (int)(_productParameter[1]._conveyor2LoadTimeOutTime * 1000);
                }

                if (_autoModeFlag && _conveyor2Status == 0 && _conveyor2ActionModuleActionFlag == 1 && !_conveyor2LoadTimeOutT.STATUS && _conveyor2TimeoutAlarmDealWithWayIndexWhenLoadAndUnload == -1)
                {
                    _conveyor2LoadTimeOutT.CURRENTTIME += TIME_COUNT_TIME_INVERTAL;
                }
                else if (!_autoModeFlag || _conveyor2Status != 0 || _conveyor2ActionModuleActionFlag != 1 ||
                    (_conveyor2LoadTimeOutT.STATUS && _inputIOT[RESET_BT_IN_IO_NO].STATUS))
                {
                    _conveyor2LoadTimeOutT.CURRENTTIME = 0;
                }
                #endregion

                #region _conveyor2UnloadTimeOutT:传送线2下料超时延时计时器，用于判定是否下料超时

                if (_autoModeFlag)
                {
                    if (_productParameter[1] != null)
                    {
                        _conveyor2UnloadTimeOutT.SETTIME = (int)(_productParameter[1]._conveyor2UnloadTimeOutTime * 1000);
                    }
                }
                else
                {
                    if (SettingsProductForm._editProductParameter != null)
                    {
                        _conveyor2UnloadTimeOutT.SETTIME = 10000;
                    }
                }

                if ((_autoModeFlag && _conveyor2Status == 2 && _conveyor2ActionModuleActionFlag == 1 && !_conveyor2UnloadTimeOutT.STATUS && _conveyor2TimeoutAlarmDealWithWayIndexWhenLoadAndUnload == -1)
                    || (!_autoModeFlag && _conveyor2ProductForcedUnloadModuleActionFlag == 0 && !_conveyor2UnloadTimeOutT.STATUS && _conveyor2TimeoutAlarmDealWithWayIndexWhenLoadAndUnload == -1))
                {
                    _conveyor2UnloadTimeOutT.CURRENTTIME += TIME_COUNT_TIME_INVERTAL;
                }
                else if ((_autoModeFlag && (_conveyor2Status != 2 || _conveyor2ActionModuleActionFlag != 1)) ||
                   (!_autoModeFlag && _conveyor2ProductForcedUnloadModuleActionFlag != 0) ||
                   (_conveyor2UnloadTimeOutT.STATUS && _inputIOT[RESET_BT_IN_IO_NO].STATUS))
                {
                    _conveyor2UnloadTimeOutT.CURRENTTIME = 0;
                }

                #endregion

                #region _conveyor1LoadUnloadModuleOffDlyT:传送线1上下料模块OFF延时计时器
                if (_conveyor1LoadUnloadModuleActionFlag != 0 && !_conveyor1LoadUnloadModuleOffDlyT.STATUS)
                {
                    _conveyor1LoadUnloadModuleOffDlyT.CURRENTTIME += TIME_COUNT_TIME_INVERTAL;
                }
                else if (_conveyor1LoadUnloadModuleActionFlag == 0)
                {
                    _conveyor1LoadUnloadModuleOffDlyT.CURRENTTIME = 0;
                }
                #endregion

                #region _conveyor2LoadUnloadModuleOffDlyT:传送线2上下料模块OFF延时计时器
                if (_conveyor2LoadUnloadModuleActionFlag != 0 && !_conveyor2LoadUnloadModuleOffDlyT.STATUS)
                {
                    _conveyor2LoadUnloadModuleOffDlyT.CURRENTTIME += TIME_COUNT_TIME_INVERTAL;
                }
                else if (_conveyor2LoadUnloadModuleActionFlag == 0)
                {
                    _conveyor2LoadUnloadModuleOffDlyT.CURRENTTIME = 0;
                }
                #endregion

                #region _barcodeScanModuleOffDlyT:传送线1动作模块OFF延时计时器
                if (_barcodeScanModuleActionFlag != 0 && !_barcodeScanModuleOffDlyT.STATUS)
                {
                    _barcodeScanModuleOffDlyT.CURRENTTIME += TIME_COUNT_TIME_INVERTAL;
                }
                else if (_barcodeScanModuleActionFlag == 0)
                {
                    _barcodeScanModuleOffDlyT.CURRENTTIME = 0;
                }
                #endregion

                #region _getOrSendProductInfoFromMESModuleOffDlyT:获取或传输数据至MES模块OFF延时计时器
                if (_getOrSendProductInfoFromMESModuleActionFlag != 0 && !_getOrSendProductInfoFromMESModuleOffDlyT.STATUS)
                {
                    _getOrSendProductInfoFromMESModuleOffDlyT.CURRENTTIME += TIME_COUNT_TIME_INVERTAL;
                }
                else if (_getOrSendProductInfoFromMESModuleActionFlag == 0)
                {
                    _getOrSendProductInfoFromMESModuleOffDlyT.CURRENTTIME = 0;
                }
                #endregion

                #region _deviceRecognizeMarkPositionAndGetCorrectMatrixModuleOffDlyT:设备识别MARK点并计算出纠正矩阵模块OFF延时计时器
                if (_deviceRecognizeMarkPositionAndGetCorrectMatrixModuleActionFlag != 0 && !_deviceRecognizeMarkPositionAndGetCorrectMatrixModuleOffDlyT.STATUS)
                {
                    _deviceRecognizeMarkPositionAndGetCorrectMatrixModuleOffDlyT.CURRENTTIME += TIME_COUNT_TIME_INVERTAL;
                }
                else if (_deviceRecognizeMarkPositionAndGetCorrectMatrixModuleActionFlag == 0)
                {
                    _deviceRecognizeMarkPositionAndGetCorrectMatrixModuleOffDlyT.CURRENTTIME = 0;
                }
                #endregion

                #region _deviceTotalWorkModuleOffDlyT:设备整体工作模块OFF延时计时器
                if (_deviceTotalWorkModuleActionFlag != 0 && !_deviceTotalWorkModuleOffDlyT.STATUS)
                {
                    _deviceTotalWorkModuleOffDlyT.CURRENTTIME += TIME_COUNT_TIME_INVERTAL;
                }
                else if (_deviceTotalWorkModuleActionFlag == 0)
                {
                    _deviceTotalWorkModuleOffDlyT.CURRENTTIME = 0;
                }
                #endregion

                #region _autoModuleOffDlyT:自动模块OFF延时计时器
                if (_autoModuleActionFlag != 0 && !_autoModuleOffDlyT.STATUS)
                {
                    _autoModuleOffDlyT.CURRENTTIME += TIME_COUNT_TIME_INVERTAL;
                }
                else if (_autoModuleActionFlag == 0)
                {
                    _autoModuleOffDlyT.CURRENTTIME = 0;
                }
                #endregion

                #region _leftAndRightCameraGrabImageModuleOffDlyT:左右相机采集模块OFF延时计时器
                if (_leftAndRightCameraGrabImageModuleActionFlag != 0 && !_leftAndRightCameraGrabImageModuleOffDlyT.STATUS)
                {
                    _leftAndRightCameraGrabImageModuleOffDlyT.CURRENTTIME += TIME_COUNT_TIME_INVERTAL;
                }
                else if (_leftAndRightCameraGrabImageModuleActionFlag == 0)
                {
                    _leftAndRightCameraGrabImageModuleOffDlyT.CURRENTTIME = 0;
                }
                #endregion

                #region _leftCameraImageInferModuleOffDlyT:左侧相机采集图片推断模块OFF延时计时器
                if (_leftCameraImageInferModuleActionFlag != 0 && !_leftCameraImageInferModuleOffDlyT.STATUS)
                {
                    _leftCameraImageInferModuleOffDlyT.CURRENTTIME += TIME_COUNT_TIME_INVERTAL;
                }
                else if (_leftCameraImageInferModuleActionFlag == 0)
                {
                    _leftCameraImageInferModuleOffDlyT.CURRENTTIME = 0;
                }
                #endregion

                #region _rightCameraImageInferModuleOffDlyT:右侧相机采集图片推断模块OFF延时计时器
                if (_rightCameraImageInferModuleActionFlag != 0 && !_rightCameraImageInferModuleOffDlyT.STATUS)
                {
                    _rightCameraImageInferModuleOffDlyT.CURRENTTIME += TIME_COUNT_TIME_INVERTAL;
                }
                else if (_rightCameraImageInferModuleActionFlag == 0)
                {
                    _rightCameraImageInferModuleOffDlyT.CURRENTTIME = 0;
                }
                #endregion

                #region _leftCameraSourceImageSaveModuleOffDlyT:左侧相机采集原始图片保存模块OFF延时计时器
                if (_leftCameraSourceImageSaveModuleActionFlag != 0 && !_leftCameraSourceImageSaveModuleOffDlyT.STATUS)
                {
                    _leftCameraSourceImageSaveModuleOffDlyT.CURRENTTIME += TIME_COUNT_TIME_INVERTAL;
                }
                else if (_leftCameraSourceImageSaveModuleActionFlag == 0)
                {
                    _leftCameraSourceImageSaveModuleOffDlyT.CURRENTTIME = 0;
                }
                #endregion

                #region _rightCameraSourceImageSaveModuleOffDlyT:右侧相机采集原始图片保存模块OFF延时计时器
                if (_rightCameraSourceImageSaveModuleActionFlag != 0 && !_rightCameraSourceImageSaveModuleOffDlyT.STATUS)
                {
                    _rightCameraSourceImageSaveModuleOffDlyT.CURRENTTIME += TIME_COUNT_TIME_INVERTAL;
                }
                else if (_rightCameraSourceImageSaveModuleActionFlag == 0)
                {
                    _rightCameraSourceImageSaveModuleOffDlyT.CURRENTTIME = 0;
                }
                #endregion

                #region _leftCameraInferredImageSaveModuleOffDlyT:左侧相机采集图片的推断结果图片保存模块OFF延时计时器
                if (_leftCameraInferredImageSaveModuleActionFlag != 0 && !_leftCameraInferredImageSaveModuleOffDlyT.STATUS)
                {
                    _leftCameraInferredImageSaveModuleOffDlyT.CURRENTTIME += TIME_COUNT_TIME_INVERTAL;
                }
                else if (_leftCameraInferredImageSaveModuleActionFlag == 0)
                {
                    _leftCameraInferredImageSaveModuleOffDlyT.CURRENTTIME = 0;
                }
                #endregion

                #region _rightCameraInferredImageSaveModuleOffDlyT:左侧相机采集图片的推断结果图片保存模块OFF延时计时器
                if (_rightCameraInferredImageSaveModuleActionFlag != 0 && !_rightCameraInferredImageSaveModuleOffDlyT.STATUS)
                {
                    _rightCameraInferredImageSaveModuleOffDlyT.CURRENTTIME += TIME_COUNT_TIME_INVERTAL;
                }
                else if (_rightCameraInferredImageSaveModuleActionFlag == 0)
                {
                    _rightCameraInferredImageSaveModuleOffDlyT.CURRENTTIME = 0;
                }
                #endregion

                #region _cameraInferDataClearModuleOffDlyT:相机推断用数据清除模块OFF延时计时器 初始化
                if (_cameraInferDataClearModuleActionFlag != 0 && !_cameraInferDataClearModuleOffDlyT.STATUS)
                {
                    _cameraInferDataClearModuleOffDlyT.CURRENTTIME += TIME_COUNT_TIME_INVERTAL;
                }
                else if (_cameraInferDataClearModuleActionFlag == 0)
                {
                    _cameraInferDataClearModuleOffDlyT.CURRENTTIME = 0;
                }
                #endregion


                if ((_changeDeviceToManualModeT.STATUS || _deadlyAlarmOccurFlag) && _autoModeFlag)//如果点击切换手动按钮3S
                {
                    MyTool.TxtFileProcess.CreateLog("设备动作-切换设备模式为手动模式，参数重置！" + "----用户：" + _operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

                    _autoModeFlag = false;
                    _baseSettingFormVariate._settingsProductFormVariate.Invoke(new Action(() =>
                    {
                        _baseSettingFormVariate._settingsProductFormVariate.chkUseUseRelativePosition.Enabled = true;
                    })); ;
                    Thread.Sleep(100);//等待异步线程由于_deadlyAlarmOccurFlag的停止
                    for (int i = 0; i < _axisQuantity; i++)
                    {
                        _axisControlFormVariate._axisActionCommand[i] = 0;
                    }

                    _axisControlFormVariate.OutputControl(69, false);//控制传送线1停止
                    _axisControlFormVariate.OutputControl(71, false);//控制传送线2停止

                    _xyMoveToAssignedPointPositionModuleActionFlag = -1;//xy运动模块状态清为-1
                    _allAxisHomeBackModuleActionFlag = -1;//回原点模块状态清为-1
                    _axesGotoStandbyModuleActionFlag = -1;//轴回等待位置模块状态清为-1
                    _conveyorChangeToAssignedWidthModuleActionFlag = -1;//传送线宽度调节模块状态清为-1
                    _conveyor1ActionModuleActionFlag = -1;//传送线1动作模块状态清为-1
                    _conveyor2ActionModuleActionFlag = -1;//传送线2动作模块状态清为-1
                    _conveyor1LoadUnloadModuleActionFlag = -1;//传送线1上下料模块状态清为-1
                    _conveyor2LoadUnloadModuleActionFlag = -1;//传送线2上下料模块状态清为-1
                    _deviceTotalWorkModuleActionFlag = -1;//设备整体工作模块状态清为-1
                    _barcodeScanModuleActionFlag = -1;//扫码模块状态清为-1
                    _getOrSendProductInfoFromMESModuleActionFlag = -1;//数据与MES交互模块状态清为-1
                    _deviceRecognizeMarkPositionAndGetCorrectMatrixModuleActionFlag = -1;//设备识别MARK点并计算变换矩阵模块状态清为-1
                    _autoModuleActionFlag = -1;

                    _startBtnLampOnOffFlag = 0;//如果切换为手动模式，设置灯为OFF
                    _currentDestinationPointIndex = 1;//当前指令坐标号清为0
                    _measureStartBtnLampOnOffFlag = 0;//切换回手动时设置上料启动按钮为OFF

                    //停止传送线
                    if (!_newDAM02AIAOOperation.comm.IsOpen)
                    {
                        if (!_newDAM02AIAOOperation.OpenSerialPort(SettingsGeneralForm._conveyorControlCom))
                        {
                            MyTool.TxtFileProcess.CreateLog("设备动作-切换设备模式为手动模式，参数重置，停止传送线，打开模拟量串口失败！" + "----用户：" + _operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                            MessageBox.Show("打开串口失败");
                        }
                    }
                    if (_newDAM02AIAOOperation.comm.IsOpen)
                    {
                        _newDAM02AIAOOperation.analogOutput(0, 0);
                        _newDAM02AIAOOperation.analogOutput(1, 0);
                    }
                    _axisControlFormVariate.OutputControl(69, false);
                    _axisControlFormVariate.OutputControl(71, false);

                    //停止向上下游发送流入流出请求
                    _axisControlFormVariate.OutputControl(72, false);
                    _axisControlFormVariate.OutputControl(73, false);
                    _axisControlFormVariate.OutputControl(74, false);
                    _axisControlFormVariate.OutputControl(75, false);

                    AxisControlMainForm._currentProductName = SettingsProductForm._editProductName;//切换回手动模式时，轴控制页面的参数切换回编辑产品点位号

                    //左右相机触发信号OFF
                    _axisControlFormVariate.OutputControl(BACK_CAMERA_LIGHT_TRIGGER_OUT_IO, false);
                    _axisControlFormVariate.OutputControl(LEFT_CAMERA_TRIGGER_OUT_IO, false);
                    _axisControlFormVariate.OutputControl(RIGHT_CAMERA_TRIGGER_OUT_IO, false);

                    _axisControlFormVariate.OutputControl(BACK_CAMERA_LIGHT_TRIGGER_OUT_IO2, false);
                    _axisControlFormVariate.OutputControl(LEFT_CAMERA_TRIGGER_OUT_IO2, false);
                    _axisControlFormVariate.OutputControl(RIGHT_CAMERA_TRIGGER_OUT_IO2, false);

                    _leftAndRightCameraGrabImageModuleActionFlag = -1;
                    _leftCameraImageInferModuleActionFlag = -1;
                    _rightCameraImageInferModuleActionFlag = -1;
                    _leftCameraSourceImageSaveModuleActionFlag = -1;
                    _rightCameraSourceImageSaveModuleActionFlag = -1;
                    _leftCameraInferredImageSaveModuleActionFlag = -1;
                    _rightCameraInferredImageSaveModuleActionFlag = -1;
                    _cameraInferDataClearModuleActionFlag = -1;

                    //释放内存
                    //_newLeftDeepLearningObjectDetectionRectangle1Infer.Dispose();
                    //_newRightDeepLearningObjectDetectionRectangle1Infer.Dispose();

                    _leftInferNeedReadParameter = true;
                    _rightInferNeedReadParameter = true;

                    _lastTimepreprocessFilePath = "";
                    _lastTimeDimensionFilePath = "";
                    _lastTimeModelFilePath = "";

                    //停止计时
                    if (_inferTimCountSW.IsRunning)
                    {
                        _inferTimCountSW.Stop();
                    }

                    if (_conveyor1CheckTimeCountSW.IsRunning)
                    {
                        _conveyor1CheckTimeCountSW.Stop();
                    }

                    if (_conveyor2CheckTimeCountSW.IsRunning)
                    {
                        _conveyor2CheckTimeCountSW.Stop();
                    }

                    //松开产品夹紧气缸
                    //控制传送线1夹紧气缸松开
                    _axisControlFormVariate.OutputControl(82, true);
                    _axisControlFormVariate.OutputControl(83, false);

                    //控制传送线2夹紧气缸松开
                    _axisControlFormVariate.OutputControl(78, true);
                    _axisControlFormVariate.OutputControl(79, false);

                    //切换手动时，关闭相机
                    if (_leftCameraCs.hv_AcqHandle != null)
                        CloseCamera(ref _leftCameraCs);
                    if (_rightCameraCs.hv_AcqHandle != null)
                        CloseCamera(ref _rightCameraCs);
                    if (_backCameraCs.hv_AcqHandle != null)
                        CloseCamera(ref _backCameraCs);

                    _autoStartingFlag = false;

                    MyTool.TxtFileProcess.CreateLog("设备动作-切换设备模式为手动模式，参数重置完成！" + "----用户：" + _operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                }

                Thread.Sleep(TIME_COUNT_TIME_INVERTAL);
            }
        }

        #endregion

        //设备状态及报警信息更新事件
        private void DeviceStateDisplayEvent(object sender)
        {
            try
            {
                string _tempAlarmMsg = "";
                for (int i = 0; i < ALARM_KIND_QUANTITY; i++)
                {
                    if (_stateAndAlarmCs._alarmFlag[i])//如果为报警
                    {
                        _tempAlarmMsg += _stateAndAlarmCs._alarmMsg[i] + "  ";
                    }
                }

                if (_tempAlarmMsg == "")//如果没有发生报警
                {
                    if (!_autoModeFlag)
                        _tempAlarmMsg = _stateAndAlarmCs._stateMsg[0];//如果没有报警并且不处于自动模式
                    else
                        _tempAlarmMsg = _stateAndAlarmCs._stateMsg[1];//如果没有报警并且处于自动模式
                }

                if (_baseFormStateOrAlarmMsg != _tempAlarmMsg)//如果当前报警信息不等于最新的报警信息
                    _baseFormStateOrAlarmMsg = _tempAlarmMsg;

                if (_baseFormStateOrAlarmMsg != _baseFormOldStateOrAlarmMsg)
                {
                    _baseFormOldStateOrAlarmMsg = _baseFormStateOrAlarmMsg;
                    _baseFormStateStartIndex = 0;
                }

                if (_baseFormStateOrAlarmMsg.Length > 0)
                {
                    Color backColor, fontColor;
                    if (_alarmOccurFlag)
                    {
                        backColor = Color.Orange;
                        fontColor = Color.Red;
                    }
                    else if (_autoModeFlag)
                    {
                        backColor = Color.Green;
                        fontColor = Color.Black;
                    }
                    else
                    {
                        backColor = SystemColors.Control;
                        fontColor = Color.Black;
                    }

                    this.Invoke(new Action(() =>
                        {
                            _baseFormStateStartIndex = MyTool.TextDisplay.AchieveDynamicDisplayTextInLabelControl(lblDeviceStatus, _baseFormStateOrAlarmMsg, "宋体", _baseFormStateStartIndex, 20, backColor, fontColor);
                        })); ;
                }
            }
            catch
            {

            }
        }

        //刷新底部label控件，主要更新时间
        private void tmrRefreshBottomLabel_Tick(object sender, EventArgs e)
        {
            //实时更新时间
            lblTime.Text = System.DateTime.Now.ToString();
        }

        //双击状态显示框，将会弹出报警页面
        private void Step_Status_Label_DoubleClick(object sender, EventArgs e)
        {

            if (_alarmFormVariate != null)
            {
                if (_alarmFormVariate.Visible == false)
                {

                    MyTool.TxtFileProcess.CreateLog("请求打开报警及报警记录界面！" + "----用户：" + _operatorName,
                        BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

                    _alarmFormVariate.StartPosition = FormStartPosition.CenterScreen;
                    _alarmFormVariate.Focus();
                    _alarmFormVariate.Show();

                }
                else
                {
                    MyTool.TxtFileProcess.CreateLog("请求关闭报警及报警记录界面！" + "----用户：" + _operatorName,
                        BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

                    _alarmFormVariate.Hide();

                }
            }

        }

        //点击motion card状态框
        private void lblMotionCardStatus_Click(object sender, EventArgs e)
        {
            if (_axisControlFormVariate != null)
            {
                if (!_axisControlFormVariate._motionCardInitialStatus)
                {
                    MyTool.TxtFileProcess.CreateLog("请求再次初始化轴运动控制卡！" + "----用户：" + _operatorName,
                        BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

                    _axisControlFormVariate.MotionCardInitalFunc();
                }
            }
        }

        #region About Alarm Module

        /// <summary>
        /// AlarmRefreshModule:报警更新模块
        /// </summary>
        private void AlarmRefreshModule()
        {
            bool[] tempAlarmOccurFlag = new bool[4] { false, false, false, false };//用于暂存报警标志，0-deadly报警，1-heavy报警，2-moderate报警，3-slight报警
            if (_deviceStartDelay.STATUS)//设备启动延时之后有效
            {
                #region 报警FLAG设置

                #region 致命报警
                if (!_inputIOT[EMERGENCY_BT_IN_IO].STATUS)//如果急停按钮信号为OFF
                    _stateAndAlarmCs._alarmFlag[0] = true;//急停报警FLAG为ON

                if (!_axisControlFormVariate._motionCardInitialStatus)
                    _stateAndAlarmCs._alarmFlag[1] = true;//运动控制卡异常报警
                #endregion

                #region 重度报警

                if (!_inputIOT[0].STATUS)//X1轴伺服报警信号为OFF
                    _stateAndAlarmCs._alarmFlag[100] = true;//伺服报警FLAG-ON

                if (!_inputIOT[1].STATUS)//Y1轴伺服报警信号为OFF
                    _stateAndAlarmCs._alarmFlag[101] = true;//伺服报警FLAG-ON

                if (!_inputIOT[2].STATUS)//R1轴伺服报警信号为OFF
                    _stateAndAlarmCs._alarmFlag[102] = true;//伺服报警FLAG-ON

                if (_axisControlFormVariate._ioStatus[0, 32] && _axisControlFormVariate._axisCommandSaved[0] != 999)//如果X1轴负限位传感为OFF，并且不为回原点时
                    _stateAndAlarmCs._alarmFlag[132] = true;//X1轴负限位报警FLAG-ON

                if (_axisControlFormVariate._ioStatus[0, 33])//如果X1轴正限位传感为OFF
                    _stateAndAlarmCs._alarmFlag[133] = true;//X1轴正限位报警FLAG-ON

                if (_axisControlFormVariate._ioStatus[0, 40] && _axisControlFormVariate._axisCommandSaved[1] != 999)//如果Y1轴负限位传感为OFF，并且不为回原点时
                    _stateAndAlarmCs._alarmFlag[134] = true;//Y1轴负限位报警FLAG-ON

                if (_axisControlFormVariate._ioStatus[0, 41])//如果Y1轴正限位传感为OFF
                    _stateAndAlarmCs._alarmFlag[135] = true;//Y1轴正限位报警FLAG-ON

                if (_axisControlFormVariate._ioStatus[0, 48] && _axisControlFormVariate._axisCommandSaved[2] != 999)//如果R1轴负限位传感为OFF，并且不为回原点时
                    _stateAndAlarmCs._alarmFlag[136] = true;//R1轴负限位报警FLAG-ON

                if (_axisControlFormVariate._ioStatus[0, 49])//如果R1轴正限位传感为OFF
                    _stateAndAlarmCs._alarmFlag[137] = true;//R1轴正限位报警FLAG-ON

                if (_axisControlFormVariate._ioStatus[0, 56] && _axisControlFormVariate._axisCommandSaved[3] != 999)//如果Y2轴负限位传感为OFF，并且不为回原点时
                    _stateAndAlarmCs._alarmFlag[138] = true;//Y2轴负限位报警FLAG-ON

                if (_axisControlFormVariate._ioStatus[0, 57])//如果Y2轴正限位传感为OFF
                    _stateAndAlarmCs._alarmFlag[139] = true;//Y2轴正限位报警FLAG-ON

                if (_axisControlFormVariate._ioStatus[0, 96] && _axisControlFormVariate._axisCommandSaved[4] != 999)//如果Y3轴负限位传感为OFF，并且不为回原点时
                    _stateAndAlarmCs._alarmFlag[140] = true;//Y3轴负限位报警FLAG-ON

                if (_axisControlFormVariate._ioStatus[0, 97])//如果Y3轴正限位传感为OFF
                    _stateAndAlarmCs._alarmFlag[141] = true;//Y3轴正限位报警FLAG-ON

                if (_axisControlFormVariate._ioStatus[0, 104] && _axisControlFormVariate._axisCommandSaved[5] != 999)//如果Y4轴负限位传感为OFF，并且不为回原点时
                    _stateAndAlarmCs._alarmFlag[142] = true;//Y4轴负限位报警FLAG-ON

                if (_axisControlFormVariate._ioStatus[0, 105])//如果Y4轴正限位传感为OFF
                    _stateAndAlarmCs._alarmFlag[143] = true;//Y4轴正限位报警FLAG-ON
                #endregion

                #region 中度报警
                //if (_airPressureLowAlarmT.STATUS)//如果气压传感器为OFF持续1s
                //    _stateAndAlarmCs._alarmFlag[300] = true;

                if (!_inputIOT[10].STATUS)//如果前安全门传感器没有ON，那么就报警 && false
                    _stateAndAlarmCs._alarmFlag[310] = true;

                if (!_inputIOT[11].STATUS)//如果后安全门传感器没有ON，那么就报警 && false
                    _stateAndAlarmCs._alarmFlag[311] = true;

                if (_cameraMoveCylinderBackAlarmT.STATUS)//如果相机回超时报警延时计时器为ON
                    _stateAndAlarmCs._alarmFlag[320] = true;

                if (_cameraMoveCylinderOutAlarmT.STATUS)//如果相机出超时报警延时计时器为ON
                    _stateAndAlarmCs._alarmFlag[321] = true;

                if (_conveyor1StopCylinderDownAlarmT.STATUS)//传送线1(外侧)阻挡气缸下超时计时器为ON
                    _stateAndAlarmCs._alarmFlag[322] = true;
                if (_conveyor1StopCylinderUpAlarmT.STATUS)//传送线1(外侧)阻挡气缸上超时计时器为ON
                    _stateAndAlarmCs._alarmFlag[323] = true;
                if (_conveyor1ClampCylinderReleaseAlarmT.STATUS)//传送线1(外侧)夹紧气缸松开超时计时器为ON
                    _stateAndAlarmCs._alarmFlag[324] = true;
                if (_conveyor1ClampCylinderClampAlarmT.STATUS)//传送线1(外侧)夹紧气缸夹紧超时计时器为ON
                    _stateAndAlarmCs._alarmFlag[325] = true;

                if (_conveyor2StopCylinderDownAlarmT.STATUS)//传送线2(里侧)阻挡气缸下超时计时器为ON
                    _stateAndAlarmCs._alarmFlag[326] = true;
                if (_conveyor2StopCylinderUpAlarmT.STATUS)//传送线2(里侧)阻挡气缸上超时计时器为ON
                    _stateAndAlarmCs._alarmFlag[327] = true;
                if (_conveyor2ClampCylinderReleaseAlarmT.STATUS)//传送线2(里侧)夹紧气缸松开超时计时器为ON
                    _stateAndAlarmCs._alarmFlag[328] = true;
                if (_conveyor2ClampCylinderClampAlarmT.STATUS)//传送线2(里侧)夹紧气缸夹紧超时计时器为ON
                    _stateAndAlarmCs._alarmFlag[329] = true;

                if (_conveyor1LoadTimeOutT.STATUS)//传送线1上料超时延时
                    _stateAndAlarmCs._alarmFlag[340] = true;
                if (_conveyor1UnloadTimeOutT.STATUS)//传送线1下料超时延时
                    _stateAndAlarmCs._alarmFlag[341] = true;
                if (_conveyor2LoadTimeOutT.STATUS)//传送线2上料超时延时
                    _stateAndAlarmCs._alarmFlag[342] = true;
                if (_conveyor2UnloadTimeOutT.STATUS)//传送线2下料超时延时
                    _stateAndAlarmCs._alarmFlag[343] = true;

                //传送线上有多余产品报警
                if (_autoModeFlag)//如果为自动模式
                {
                    //如果传送线1中有多余物料触发了三个传感器中的任意两个
                    if (((_inputIOT[89].STATUS && (_inputIOT[92].STATUS || _inputIOT[93].STATUS)) ||
                        (_inputIOT[92].STATUS && _inputIOT[93].STATUS)) && _conveyor1HaveExcessProductAlarmDealWithWayIndex != 1 && _conveyor1Status != 2)
                    {
                        _stateAndAlarmCs._alarmFlag[400] = true;//传送线1有多余产品报警
                    }

                    //如果传送线2中有多余物料触发了三个传感器中的任意两个
                    if (((_inputIOT[88].STATUS && (_inputIOT[90].STATUS || _inputIOT[91].STATUS)) ||
                        (_inputIOT[90].STATUS && _inputIOT[91].STATUS)) && _conveyor2HaveExcessProductAlarmDealWithWayIndex != 1 && _conveyor2Status != 2)
                    {
                        _stateAndAlarmCs._alarmFlag[401] = true;//传送线2有多余产品报警
                    }
                }

                #endregion

                #endregion
            }

            #region 报警复位相关程序
            if (_inputIOT[RESET_BT_IN_IO_NO].STATUS)//如果报警复位按钮被点击
            {
                //实现点击复位按钮如同点击了弹出界面的确定按钮
                if (_informVariate != null)
                {
                    if (_informVariate.btnOK.Visible)
                    {
                        InformationFormClickEnsureButtonEventFunc();
                    }
                }

                if (_conveyorHaveExcessProductInfoFormVariate != null)
                {
                    if (_conveyorHaveExcessProductInfoFormVariate.btnReleaseAndTakeOut.Visible)
                    {
                        ConveyorHaveExcessProductInfoFormTakeOutButtonEventFunc();
                    }
                }

                if (!_autoModeFlag)
                {
                    #region 致命报警
                    if (_inputIOT[EMERGENCY_BT_IN_IO].STATUS)//如果急停按钮信号为ON
                        _stateAndAlarmCs._alarmFlag[0] = false;//急停报警FLAG为false

                    if (_axisControlFormVariate._motionCardInitialStatus)
                        _stateAndAlarmCs._alarmFlag[1] = false;//运动控制卡异常报警
                    #endregion

                    #region 重度报警

                    if (_inputIOT[0].STATUS)//X1轴伺服报警信号为ON
                        _stateAndAlarmCs._alarmFlag[100] = false;//伺服报警FLAG-OFF

                    if (_inputIOT[1].STATUS)//Y1轴伺服报警信号为ON
                        _stateAndAlarmCs._alarmFlag[101] = false;//伺服报警FLAG-OFF

                    if (_inputIOT[2].STATUS)//R1轴伺服报警信号为ON
                        _stateAndAlarmCs._alarmFlag[102] = false;//伺服报警FLAG-Off

                    if (!_axisControlFormVariate._ioStatus[0, 32])//如果X1轴负限位传感为OFF
                        _stateAndAlarmCs._alarmFlag[132] = false;//X1轴负限位报警FLAG-Off

                    if (!_axisControlFormVariate._ioStatus[0, 33])//如果X1轴正限位传感为OFF
                        _stateAndAlarmCs._alarmFlag[133] = false;//X1轴正限位报警FLAG-OFF

                    if (!_axisControlFormVariate._ioStatus[0, 40])//如果Y1轴负限位传感为OFF
                        _stateAndAlarmCs._alarmFlag[134] = false;//Y1轴负限位报警FLAG-OFF

                    if (!_axisControlFormVariate._ioStatus[0, 41])//如果Y1轴正限位传感为OFF
                        _stateAndAlarmCs._alarmFlag[135] = false;//Y1轴正限位报警FLAG-OFF

                    if (!_axisControlFormVariate._ioStatus[0, 48])//如果R1轴负限位传感为OFF
                        _stateAndAlarmCs._alarmFlag[136] = false;//R1轴负限位报警FLAG-OFF

                    if (!_axisControlFormVariate._ioStatus[0, 49])//如果R1轴正限位传感为OFF
                        _stateAndAlarmCs._alarmFlag[137] = false;//R1轴正限位报警FLAG-OFF

                    if (!_axisControlFormVariate._ioStatus[0, 56])//如果Y2轴负限位传感为OFF
                        _stateAndAlarmCs._alarmFlag[138] = false;//Y2轴负限位报警FLAG-OFF

                    if (!_axisControlFormVariate._ioStatus[0, 57])//如果Y2轴正限位传感为OFF
                        _stateAndAlarmCs._alarmFlag[139] = false;//Y2轴正限位报警FLAG-OFF

                    if (!_axisControlFormVariate._ioStatus[0, 96])//如果Y3轴负限位传感为OFF
                        _stateAndAlarmCs._alarmFlag[140] = false;//Y3轴负限位报警FLAG-OFF

                    if (!_axisControlFormVariate._ioStatus[0, 97])//如果Y3轴正限位传感为OFF
                        _stateAndAlarmCs._alarmFlag[141] = false;//Y3轴正限位报警FLAG-OFF

                    if (!_axisControlFormVariate._ioStatus[0, 104])//如果Y4轴负限位传感为OFF
                        _stateAndAlarmCs._alarmFlag[142] = false;//Y4轴负限位报警FLAG-OFF

                    if (!_axisControlFormVariate._ioStatus[0, 105])//如果Y4轴正限位传感为OFF
                        _stateAndAlarmCs._alarmFlag[143] = false;//Y4轴正限位报警FLAG-OFF
                    #endregion
                }

                #region 中度报警
                if (!_airPressureLowAlarmT.STATUS)//如果气压传感器为OFF未持续1s
                    _stateAndAlarmCs._alarmFlag[300] = false;

                if (_inputIOT[10].STATUS)//如果安全传感器恢复信号，那么可以复位
                    _stateAndAlarmCs._alarmFlag[310] = false;
                 if (_inputIOT[11].STATUS)//如果安全传感器恢复信号，那么可以复位
                    _stateAndAlarmCs._alarmFlag[311] = false;

                if (!_cameraMoveCylinderBackAlarmT.STATUS)//如果相机回超时报警延时计时器为OFF
                    _stateAndAlarmCs._alarmFlag[320] = false;
                if (!_cameraMoveCylinderOutAlarmT.STATUS)//如果相机出超时报警延时计时器为OFF
                    _stateAndAlarmCs._alarmFlag[321] = false;

                if (!_conveyor1StopCylinderDownAlarmT.STATUS)//传送线1(里侧)阻挡气缸下超时计时器为OFF
                    _stateAndAlarmCs._alarmFlag[322] = false;
                if (!_conveyor1StopCylinderUpAlarmT.STATUS)//传送线1(里侧)阻挡气缸上超时计时器为OFF
                    _stateAndAlarmCs._alarmFlag[323] = false;
                if (!_conveyor1ClampCylinderReleaseAlarmT.STATUS)//传送线1(里侧)夹紧气缸松开超时计时器为OFF
                    _stateAndAlarmCs._alarmFlag[324] = false;
                if (!_conveyor1ClampCylinderClampAlarmT.STATUS)//传送线1(里侧)夹紧气缸夹紧超时计时器为OFF
                    _stateAndAlarmCs._alarmFlag[325] = false;

                if (!_conveyor1StopCylinderDownAlarmT.STATUS)//传送线2(外侧)阻挡气缸下超时计时器为OFF
                    _stateAndAlarmCs._alarmFlag[326] = false;
                if (!_conveyor1StopCylinderUpAlarmT.STATUS)//传送线2(外侧)阻挡气缸上超时计时器为OFF
                    _stateAndAlarmCs._alarmFlag[327] = false;
                if (!_conveyor1ClampCylinderReleaseAlarmT.STATUS)//传送线2(外侧)夹紧气缸松开超时计时器为OFF
                    _stateAndAlarmCs._alarmFlag[328] = false;
                if (!_conveyor1ClampCylinderClampAlarmT.STATUS)//传送线2(外侧)夹紧气缸夹紧超时计时器为OFF
                    _stateAndAlarmCs._alarmFlag[329] = false;

                if (_conveyorLoadUnloadTimeoutInfoFormVariate != null)
                {
                    //if (_stateAndAlarmCs._alarmFlag[340])//传送线1上料超时延时
                    //    _stateAndAlarmCs._alarmFlag[340] = false;
                    //if (_stateAndAlarmCs._alarmFlag[341])//传送线1下料超时延时
                    //    _stateAndAlarmCs._alarmFlag[341] = false;
                    //if (_stateAndAlarmCs._alarmFlag[342])//传送线2上料超时延时
                    //    _stateAndAlarmCs._alarmFlag[342] = false;
                    //if (_stateAndAlarmCs._alarmFlag[343])//传送线2下料超时延时
                    //    _stateAndAlarmCs._alarmFlag[343] = false;
                    ConveyorLoadUnloadTimeoutInfoFormClickEnsureButtonEventFunc();//按下复位按钮等同于点击重试按钮
                }

                if (_stateAndAlarmCs._alarmFlag[350])//传送线1产品扫码失败报警
                    _stateAndAlarmCs._alarmFlag[350] = false;
                if (_stateAndAlarmCs._alarmFlag[351])//传送线2产品扫码失败报警
                    _stateAndAlarmCs._alarmFlag[351] = false;

                if ((!_inputIOT[89].STATUS && _informclickBtnIndex == 2) || !_autoModeFlag || _informclickBtnIndex == 1)//传送线1产品扫码失败，请求取走产品报警
                    _stateAndAlarmCs._alarmFlag[352] = false;
                if ((!_inputIOT[88].STATUS && _informclickBtnIndex == 2) || !_autoModeFlag || _informclickBtnIndex == 1)//传送线2产品扫码失败，请求取走产品报警
                    _stateAndAlarmCs._alarmFlag[353] = false;

                if (_stateAndAlarmCs._alarmFlag[360])//相机抓取图像报警
                {
                    _grabImageFailedRetryCount = 0;
                    _stateAndAlarmCs._alarmFlag[360] = false;
                }

                if ((((!_inputIOT[89].STATUS && (_conveyor1Status == 102 || _conveyor1Status == 105)) || (!_inputIOT[88].STATUS && (_conveyor2Status == 102 || _conveyor2Status == 105)))
                    && _informclickBtnIndex == 2) || !_autoModeFlag || _informclickBtnIndex == 1 || _informclickBtnIndex == 0)
                {
                    _stateAndAlarmCs._alarmFlag[370] = false;
                    _stateAndAlarmCs._alarmFlag[371] = false;
                }

                if ((((_conveyor1Status > 100 && !_inputIOT[89].STATUS) || (_conveyor2Status > 100 && !_inputIOT[88].STATUS)) && _informclickBtnIndex == 2)
                    || _informclickBtnIndex == 0 || _informclickBtnIndex == 1)//如果发生直通率报警，中度
                    _stateAndAlarmCs._alarmFlag[380] = false;

                if ((!_inputIOT[89].STATUS && _informclickBtnIndex == 2) || !_autoModeFlag || _informclickBtnIndex == 1 ||_informclickBtnIndex == 0)//传送线1产品Mark点识别失败，请求取走产品或流出
                {
                    _stateAndAlarmCs._alarmFlag[390] = false;
                    _stateAndAlarmCs._alarmFlag[392] = false;
                }

                if ((!_inputIOT[88].STATUS && _informclickBtnIndex == 2) || !_autoModeFlag || _informclickBtnIndex == 1 || _informclickBtnIndex == 0)//传送线2产品Mark点识别失败，请求取走产品或流出
                {
                    _stateAndAlarmCs._alarmFlag[391] = false;
                    _stateAndAlarmCs._alarmFlag[393] = false;
                }

                //如果只有一个传感器信号为ON，并且选择了人工取走，或者选择了流出按钮
                if ((((_inputIOT[89].STATUS && !_inputIOT[92].STATUS && !_inputIOT[93].STATUS) ||
                    (!_inputIOT[89].STATUS && _inputIOT[92].STATUS && !_inputIOT[93].STATUS) ||
                    (!_inputIOT[89].STATUS && !_inputIOT[92].STATUS && _inputIOT[93].STATUS) ||
                    (!_inputIOT[89].STATUS && !_inputIOT[92].STATUS && !_inputIOT[93].STATUS) ) &&
                    _conveyor1HaveExcessProductAlarmDealWithWayIndex == 0) || _conveyor1HaveExcessProductAlarmDealWithWayIndex == 1)
                {
                    _stateAndAlarmCs._alarmFlag[400] = false;
                }

                //如果只有一个传感器信号为ON，并且选择了人工取走，或者选择了流出按钮
                if ((((_inputIOT[88].STATUS && !_inputIOT[90].STATUS && !_inputIOT[91].STATUS) ||
                    (!_inputIOT[88].STATUS && _inputIOT[90].STATUS && !_inputIOT[91].STATUS) ||
                    (!_inputIOT[88].STATUS && !_inputIOT[90].STATUS && _inputIOT[91].STATUS) ||
                    (!_inputIOT[88].STATUS && !_inputIOT[90].STATUS && !_inputIOT[91].STATUS)) &&
                    _conveyor2HaveExcessProductAlarmDealWithWayIndex == 0) || _conveyor2HaveExcessProductAlarmDealWithWayIndex == 1)
                {
                    _stateAndAlarmCs._alarmFlag[401] = false;
                }

                //如果选择了人工取出，点击复位按钮之后即置为-1，如果选择的是流出，由于要控制模块结束，所以不能在此结束
                if (_conveyor1HaveExcessProductAlarmDealWithWayIndex == 0)
                {
                    _conveyor1HaveExcessProductAlarmDealWithWayIndex = -1;
                }


                //如果选择了人工取出，点击复位按钮之后即置为-1，如果选择的是流出，由于要控制模块结束，所以不能在此结束
                if (_conveyor2HaveExcessProductAlarmDealWithWayIndex == 0)
                {
                    _conveyor2HaveExcessProductAlarmDealWithWayIndex = -1;
                }

                //样品板检测超时报警，如果选择了人工取出，那么传感器要OFF才能复位，如果选择流出，那么选择了流出就可复位
                if ((!_inputIOT[89].STATUS && _informclickBtnIndex == 2) || !_autoModeFlag || _informclickBtnIndex == 1 )//传送线1样品板检测超时报警，请求取走或流出产品
                {
                    _stateAndAlarmCs._alarmFlag[410] = false;
                }

                //样品板检测超时报警，如果选择了人工取出，那么传感器要OFF才能复位，如果选择流出，那么选择了流出就可复位
                if ((!_inputIOT[88].STATUS && _informclickBtnIndex == 2) || !_autoModeFlag || _informclickBtnIndex == 1 )//传送线2样品板检测超时报警，请求取走或流出产品
                {
                    _stateAndAlarmCs._alarmFlag[411] = false;
                }


                //相机启动报警复位
                if ((_stateAndAlarmCs._alarmFlag[420] || _stateAndAlarmCs._alarmFlag[421]))
                {
                    _stateAndAlarmCs._alarmFlag[420] = false;
                    _stateAndAlarmCs._alarmFlag[421] = false;
                    InformationFormClickEnsureButtonEventFunc();
                }
               
                #endregion

                #region 轻度报警
                if (_stateAndAlarmCs._alarmFlag[500])//如果发生直通率报警，轻度
                    _stateAndAlarmCs._alarmFlag[500] = false;
                #endregion
            }

            //轴报警复位
            if (_inputIOT[3].STATUS && !_autoModeFlag)//如果报警复位按钮被点击
            {
                _axisControlFormVariate.OutputControl(16, true);//报警复位



            }

            #endregion

            #region 设置报警Flag
            //判定致命报警发生与否
            for (int i = 0; i < 100; i++)
            {
                if (_stateAndAlarmCs._alarmFlag[i])
                {
                    tempAlarmOccurFlag[0] = true;
                    break;
                }
            }

            //判定其余报警发生与否
            for (int i = 0; i < 200; i++)
            {
                if (_stateAndAlarmCs._alarmFlag[100 + i])
                    tempAlarmOccurFlag[1] = true;
                if (_stateAndAlarmCs._alarmFlag[300 + i])
                    tempAlarmOccurFlag[2] = true;
                if (_stateAndAlarmCs._alarmFlag[500 + i])
                    tempAlarmOccurFlag[3] = true;

                if (tempAlarmOccurFlag[1] && tempAlarmOccurFlag[2] && tempAlarmOccurFlag[3])
                {
                    break;
                }
            }

            //分别分配报警Flag
            _deadlyAlarmOccurFlag = tempAlarmOccurFlag[0];
            _heavyAlarmOccurFlag = tempAlarmOccurFlag[1];
            _moderateAlarmOccurFlag = tempAlarmOccurFlag[2];
            _slightAlarmOccurFlag = tempAlarmOccurFlag[3];

            if (_deadlyAlarmOccurFlag || _heavyAlarmOccurFlag || _moderateAlarmOccurFlag || _slightAlarmOccurFlag)
                _alarmOccurFlag = true;
            else
                _alarmOccurFlag = false;

            if (_deadlyAlarmOccurFlag || _heavyAlarmOccurFlag || _moderateAlarmOccurFlag)
                _pauseAlarmOccurFlag = true;
            else
                _pauseAlarmOccurFlag = false;

            #endregion

            //启动按钮灯
            if (_pauseAlarmOccurFlag)//如果发生值得设备暂停的报警，关于实现Trigger功能
            {
                _afterAlarmNeedPressResetBtnFlag = true;
            }

            if (!_pauseAlarmOccurFlag && _afterAlarmNeedPressResetBtnFlag)//当清除了报警，需要按下Start按钮
            {
                _afterPressResetBtnNeedPressStartBtnFlag = true;
                _startBtnLampOnOffFlag = 2;
            }

            if (_afterPressResetBtnNeedPressStartBtnFlag && _inputIOT[START_BT_IN_IO_NO].STATUS && !_pauseAlarmOccurFlag)
            {
                _afterPressResetBtnNeedPressStartBtnFlag = false;
                _afterAlarmNeedPressResetBtnFlag = false;
                if (_autoModeFlag && _allAxisHomeBackModuleActionFlag != 0 && _startBtnLampOnOffFlag != 1)
                {
                    _startBtnLampOnOffFlag = 1;
                }
                else if (!_autoModeFlag && _startBtnLampOnOffFlag != 0)
                {
                    _startBtnLampOnOffFlag = 0;
                }
            }

            if (_afterPressResetBtnNeedPressStartBtnFlag && _pauseAlarmOccurFlag)
            {
                _afterPressResetBtnNeedPressStartBtnFlag = false;
                if (_autoModeFlag && _allAxisHomeBackModuleActionFlag != 0 && _startBtnLampOnOffFlag != 1)
                {
                    _startBtnLampOnOffFlag = 1;
                }
                else if (!_autoModeFlag && _startBtnLampOnOffFlag != 0)
                {
                    _startBtnLampOnOffFlag = 0;
                }
            }

            //reset 按钮灯
            if (_alarmOccurFlag)//如果发生报警
            {
                _resetBtnLampOnOffFlag = 2;//设置reset button为间隔500ms闪烁
            }
            else
            {
                _resetBtnLampOnOffFlag = 0;//设置reset button为不亮
            }

            #region 三色灯控制
            if (_pauseAlarmOccurFlag && _towerLampRedOnOffFlag != 1)//如果发生值得暂停的报警
            {
                _towerLampRedOnOffFlag = 1;
            }
            else if (!_alarmOccurFlag && _towerLampRedOnOffFlag != 0)
            {
                _towerLampRedOnOffFlag = 0;
            }
            else if (_alarmOccurFlag && !_pauseAlarmOccurFlag && _towerLampRedOnOffFlag != 2)//如果发生警告
            {
                _towerLampRedOnOffFlag = 2;
            }

            if (!_autoModeFlag && _towerLampYellowOnOffFlag != 1)//如果不为自动模式
            {
                _towerLampYellowOnOffFlag = 1;
            }
            else if (_autoModeFlag && _conveyor1Status == 0 && _conveyor2Status == 0 && _towerLampYellowOnOffFlag != 3 && _allAxisHomeBackModuleActionFlag == 1)
            {
                _towerLampYellowOnOffFlag = 3;
            }
            else if (_autoModeFlag && (_conveyor1Status != 0 || _conveyor2Status != 0) && _towerLampYellowOnOffFlag != 0)
            {
                _towerLampYellowOnOffFlag = 0;
            }

            if (_autoModeFlag && _towerLampGreenOnOffFlag != 1)//如果为自动模式三色灯绿灯没有亮
            {
                _towerLampGreenOnOffFlag = 1;
            }
            else if (!_autoModeFlag && _towerLampGreenOnOffFlag != 0)
            {
                _towerLampGreenOnOffFlag = 0;
            }

            if (!_alarmOccurFlag)
            {
                _towerLampBuzzerNeedActionFlag = true;
            }

            if (_alarmOccurFlag && _towerLampBuzzerNeedActionFlag)//如果发生报警，且为无报警转换为有报警
            {
                _towerLampBuzzerOnOffFlag = 2;
                _towerLampBuzzerNeedActionFlag = false;
            }

            if (_towerLampBuzzerOnOffFlag != 0 &&
                (_inputIOT[START_BT_IN_IO_NO].STATUS || _inputIOT[STOP_BT_IN_IO_NO].STATUS ||
                _inputIOT[RESET_BT_IN_IO_NO].STATUS || _inputIOT[MEASURE_START_BT_IN_IO_NO].STATUS))
            {
                _towerLampBuzzerOnOffFlag = 0;
            }

            #endregion

            #region 发生致命报警重置

            if (_deadlyAlarmOccurFlag)//如果发生致命报警
            {
                if (_axisControlFormVariate != null && _axisControlFormVariate._motionCardInitialStatus)
                {
                    for (int i = 0; i < _axisQuantity; i++)
                    {
                        _axisControlFormVariate._axisMotionControlBoardVariate.jogAxisImmediateStop(i);
                    }

                }
                for (int i = 0; i < _axisQuantity; i++)
                {
                    _axisControlFormVariate._axisActionCommand[i] = 0;
                }

                _xyMoveToAssignedPointPositionModuleActionFlag = -1;//xy运动模块状态清为-1
                _allAxisHomeBackModuleActionFlag = -1;//回原点模块状态清为-1
                _axesGotoStandbyModuleActionFlag = -1;//轴回等待位置模块状态清为-1
                _conveyorChangeToAssignedWidthModuleActionFlag = -1;//传送线宽度调节模块状态清为-1
                _conveyor1ActionModuleActionFlag = -1;//传送线1动作模块状态清为-1
                _conveyor2ActionModuleActionFlag = -1;//传送线2动作模块状态清为-1
                _conveyor1LoadUnloadModuleActionFlag = -1;//传送线1上下料模块状态清为-1
                _conveyor2LoadUnloadModuleActionFlag = -1;//传送线2上下料模块状态清为-1
                _deviceTotalWorkModuleActionFlag = -1;//设备整体工作模块状态清为-1
                _barcodeScanModuleActionFlag = -1;//扫码模块状态清为-1
                _getOrSendProductInfoFromMESModuleActionFlag = -1;//数据与MES交互模块状态清为-1
                _deviceRecognizeMarkPositionAndGetCorrectMatrixModuleActionFlag = -1;//设备识别MARK点并计算变换矩阵模块状态清为-1
                _autoModuleActionFlag = -1;

                _startBtnLampOnOffFlag = 0;//如果切换为手动模式，设置灯为OFF
                _currentDestinationPointIndex = 1;//当前指令坐标号清为0
                _measureStartBtnLampOnOffFlag = 0;//切换回手动时设置上料启动按钮为OFF

                //停止传送线
                if (!_newDAM02AIAOOperation.comm.IsOpen)
                {
                    if (!_newDAM02AIAOOperation.OpenSerialPort(SettingsGeneralForm._conveyorControlCom))
                    {
                        //MessageBox.Show("打开串口失败");
                    }
                }

                if (_newDAM02AIAOOperation.comm.IsOpen)
                {
                    _newDAM02AIAOOperation.analogOutput(0, 0);
                    _newDAM02AIAOOperation.analogOutput(1, 0);
                }
                _axisControlFormVariate.OutputControl(69, false);
                _axisControlFormVariate.OutputControl(71, false);

                //停止向上下游发送流入流出请求
                _axisControlFormVariate.OutputControl(72, false);
                _axisControlFormVariate.OutputControl(73, false);
                _axisControlFormVariate.OutputControl(74, false);
                _axisControlFormVariate.OutputControl(75, false);

                AxisControlMainForm._currentProductName = SettingsProductForm._editProductName;//切换回手动模式时，轴控制页面的参数切换回编辑产品点位号

                //左右相机触发信号OFF
                _axisControlFormVariate.OutputControl(LEFT_CAMERA_TRIGGER_OUT_IO, false);
                _axisControlFormVariate.OutputControl(RIGHT_CAMERA_TRIGGER_OUT_IO, false);

                _axisControlFormVariate.OutputControl(LEFT_CAMERA_TRIGGER_OUT_IO2, false);
                _axisControlFormVariate.OutputControl(RIGHT_CAMERA_TRIGGER_OUT_IO2, false);

                //关闭后相机光源
                _axisControlFormVariate.OutputControl(BACK_CAMERA_LIGHT_TRIGGER_OUT_IO, false);
                _axisControlFormVariate.OutputControl(BACK_CAMERA_LIGHT_TRIGGER_OUT_IO2, false);

                _leftAndRightCameraGrabImageModuleActionFlag = -1;
                _leftCameraImageInferModuleActionFlag = -1;
                _rightCameraImageInferModuleActionFlag = -1;
                _leftCameraSourceImageSaveModuleActionFlag = -1;
                _rightCameraSourceImageSaveModuleActionFlag = -1;
                _leftCameraInferredImageSaveModuleActionFlag = -1;
                _rightCameraInferredImageSaveModuleActionFlag = -1;
                _cameraInferDataClearModuleActionFlag = -1;

                //释放内存
                //_newLeftDeepLearningObjectDetectionRectangle1Infer.Dispose();
                //_newRightDeepLearningObjectDetectionRectangle1Infer.Dispose();

                _leftInferNeedReadParameter = true;
                _rightInferNeedReadParameter = true;

                _lastTimepreprocessFilePath = "";
                _lastTimeDimensionFilePath = "";
                _lastTimeModelFilePath = "";

                ////当出现急停报警时，关闭相机
                //CloseCamera(ref _leftCameraCs);
                //CloseCamera(ref _rightCameraCs);
                //CloseCamera(ref _backCameraCs);

                //清除传送线强制流出产品状态
                _conveyor1ProductForcedUnloadModuleActionFlag = -1;
                _conveyor2ProductForcedUnloadModuleActionFlag = -1;
            }

            #endregion

            #region 如果发生了上下料超时报警，弹出框

            if (_stateAndAlarmCs._alarmFlag[340] || _stateAndAlarmCs._alarmFlag[341] || _stateAndAlarmCs._alarmFlag[342] || _stateAndAlarmCs._alarmFlag[343])
            {
                if (_conveyorLoadUnloadTimeoutInfoFormVariate == null)
                {
                    this.Invoke(new Action(() =>
                            {
                                string tempTextStr = "";
                                string tempContentStr = "";
                                _conveyorLoadUnloadTimeoutInfoFormVariate = new InfoForm();
                                _conveyorLoadUnloadTimeoutInfoFormVariate.ClickEnsureEvent += ConveyorLoadUnloadTimeoutInfoFormClickEnsureButtonEventFunc;
                                _conveyorLoadUnloadTimeoutInfoFormVariate.ClickTakeOutEvent += ConveyorLoadUnloadTimeoutInfoFormClickOutButtonEventFunc;
                                _conveyorLoadUnloadTimeoutInfoFormVariate.ClickOutEvent += ConveyorLoadUnloadTimeoutInfoFormClickTakeOutButtonEventFunc;
                                _conveyorLoadUnloadTimeoutInfoFormVariate.StartPosition = FormStartPosition.CenterScreen;
                                _conveyorLoadUnloadTimeoutInfoFormVariate.Show();
                                if (_stateAndAlarmCs._alarmFlag[340])
                                {
                                    tempTextStr = "传送线1上料超时报警";
                                    tempContentStr = _stateAndAlarmCs._alarmMsg[340];
                                    _conveyor1TimeoutAlarmDealWithWayIndexWhenLoadAndUnload = -1;//重置传送线1超时报警处理方式
                                }
                                else if (_stateAndAlarmCs._alarmFlag[341])
                                {
                                    tempTextStr = "传送线1下料超时报警";
                                    tempContentStr = _stateAndAlarmCs._alarmMsg[341];
                                    _conveyor1TimeoutAlarmDealWithWayIndexWhenLoadAndUnload = -1;//重置传送线1超时报警处理方式
                                }
                                else if (_stateAndAlarmCs._alarmFlag[342])
                                {
                                    tempTextStr = "传送线2上料超时报警";
                                    tempContentStr = _stateAndAlarmCs._alarmMsg[342];
                                    _conveyor2TimeoutAlarmDealWithWayIndexWhenLoadAndUnload = -1;//重置传送线2超时报警处理方式
                                }
                                else if (_stateAndAlarmCs._alarmFlag[343])
                                {
                                    tempTextStr = "传送线2下料超时报警";
                                    tempContentStr = _stateAndAlarmCs._alarmMsg[343];
                                    _conveyor2TimeoutAlarmDealWithWayIndexWhenLoadAndUnload = -1;//重置传送线2超时报警处理方式
                                }
                                _conveyorLoadUnloadTimeoutInfoFormVariate.Text = tempTextStr;
                                _conveyorLoadUnloadTimeoutInfoFormVariate.txtInfoMessageDisp.Text = tempContentStr;
                                _conveyorLoadUnloadTimeoutInfoFormVariate.btnOK.Text = "重试";
                                _conveyorLoadUnloadTimeoutInfoFormVariate.btnOK.Visible = true;
                                _conveyorLoadUnloadTimeoutInfoFormVariate.btnReleaseAndTakeOut.Text = "忽略";
                                _conveyorLoadUnloadTimeoutInfoFormVariate.btnReleaseAndTakeOut.Visible = true;
                                _conveyorLoadUnloadTimeoutInfoFormVariate.btnOut.Text = "无物料";
                                _conveyorLoadUnloadTimeoutInfoFormVariate.btnOut.Visible = true;
                            })); ;
                }
            }

            #endregion

            #region 如果发生了传送线中有多余产品报警,弹出框
            if (!_autoModeFlag)
            {

                if (_conveyor1HaveExcessProductAlarmDealWithWayIndex != -1)
                {
                    //初始化传送线1有多余产品报警处理方式
                    _conveyor1HaveExcessProductAlarmDealWithWayIndex = -1;
                }
                if (_conveyor2HaveExcessProductAlarmDealWithWayIndex != -1)
                {
                    //初始化传送线2有多余产品报警处理方式
                    _conveyor2HaveExcessProductAlarmDealWithWayIndex = -1;
                }
            }

            if ((_stateAndAlarmCs._alarmFlag[400] && _conveyor1HaveExcessProductAlarmDealWithWayIndex == -1)
                || (_stateAndAlarmCs._alarmFlag[401] && _conveyor2HaveExcessProductAlarmDealWithWayIndex == -1))
            {
                if (_conveyorHaveExcessProductInfoFormVariate == null)//如果传送线有多余产品报警提示框界面变量为空
                {
                    this.Invoke(new Action(() =>
                    {
                        string tempTextStr = "";
                        string tempContentStr = "";
                        _conveyorHaveExcessProductInfoFormVariate = new InfoForm();
                        _conveyorHaveExcessProductInfoFormVariate.ClickOutEvent += ConveyorHaveExcessProductInfoFormClickOutButtonEventFunc;
                        _conveyorHaveExcessProductInfoFormVariate.ClickTakeOutEvent += ConveyorHaveExcessProductInfoFormTakeOutButtonEventFunc;
                        _conveyorHaveExcessProductInfoFormVariate.StartPosition = FormStartPosition.CenterScreen;
                        _conveyorHaveExcessProductInfoFormVariate.Show();
                        if (_stateAndAlarmCs._alarmFlag[400] && _conveyor1HaveExcessProductAlarmDealWithWayIndex == -1)
                        {
                            tempTextStr = "传送线1有多余产品报警";
                            tempContentStr = "传送线1中有多余产品报警，请人工取走前面的产品或选择流出产品！";
                            _conveyor1HaveExcessProductAlarmDealWithWayIndex = -1;//重置传送线1超时报警处理方式
                        }
                        else if (_stateAndAlarmCs._alarmFlag[401] && _conveyor2HaveExcessProductAlarmDealWithWayIndex == -1)
                        {
                            tempTextStr = "传送线2有多余产品报警";
                            tempContentStr = "传送线2中有多余产品报警，请人工取走前面的产品或选择流出产品！";
                            _conveyor2HaveExcessProductAlarmDealWithWayIndex = -1;//重置传送线1超时报警处理方式
                        }

                        _conveyorHaveExcessProductInfoFormVariate.Text = tempTextStr;
                        _conveyorHaveExcessProductInfoFormVariate.txtInfoMessageDisp.Text = tempContentStr;
                        _conveyorHaveExcessProductInfoFormVariate.btnOK.Visible = false;
                        _conveyorHaveExcessProductInfoFormVariate.btnReleaseAndTakeOut.Text = "人工取出";
                        _conveyorHaveExcessProductInfoFormVariate.btnReleaseAndTakeOut.Visible = true;
                        _conveyorHaveExcessProductInfoFormVariate.btnOut.Text = "流出";
                        _conveyorHaveExcessProductInfoFormVariate.btnOut.Visible = true;
                    })); ;
                }
            }

            #endregion

            #region 如果没有测量样品板警告

                string currentTime = DateTime.Now.ToString("yyyyMMddHHmm");
                int currentTimeYear = Convert.ToInt32(currentTime.Substring(0, 4));
                int currentTimeMonth = Convert.ToInt32(currentTime.Substring(4, 2));
                int currentTimeDay = Convert.ToInt32(currentTime.Substring(6, 2));
                int currentTimeHour = Convert.ToInt32(currentTime.Substring(8, 2));
                int currentTimeMinutes = Convert.ToInt32(currentTime.Substring(10, 2));
                bool samplePanelCheckResult = false;
                int samplePanelCheckTimeYear = 0;
                int samplePanelCheckTimeMonth = 0;
                int samplePanelCheckTimeDay = 0;
                int samplePanelCheckTimeHour = 0;
                int samplePanelCheckTimeMinutes = 0;
                int productShiftTimeYear = 0;
                int productShiftTimeMonth = 0;
                int productShiftTimeDay = 0;
                int productShiftTimeHour = 0;
                int productShiftTimeMinutes = 0;
                bool samplePanelCheckStatus = false;

                //警告截至时间
                int warningEndYear, warningEndMonth, warningEndDay, warningEndHour, warningEndMinutes;

                //如果当前是白班
                if ((currentTimeHour > SettingsGeneralForm._clearYieldTimeHour || (currentTimeHour == SettingsGeneralForm._clearYieldTimeHour && currentTimeMinutes >= SettingsGeneralForm._clearYieldTimeMinutes))
                    && (currentTimeHour < SettingsGeneralForm._clearYieldTimeHour + 12 || (currentTimeHour == SettingsGeneralForm._clearYieldTimeHour + 12 && currentTimeMinutes < SettingsGeneralForm._clearYieldTimeMinutes)))
                {
                    MyTool.Other.TimeAddition(currentTimeYear, currentTimeMonth, currentTimeDay,
                    SettingsGeneralForm._clearYieldTimeHour, SettingsGeneralForm._clearYieldTimeMinutes, SettingsGeneralForm._warningTimeAfterShiftWithoutSampleCheck,
                    out warningEndYear, out warningEndMonth, out warningEndDay, out warningEndHour, out warningEndMinutes);
                }
                else//如果为夜班
                {
                    if (currentTimeHour <= SettingsGeneralForm._clearYieldTimeHour)//如果小于换班时间，表明已经换了一天了
                    {
                        int tempYesterdayYear, tempYesterdayMonth, tempYesterdayDay;
                        MyTool.Other.YesterdayCalculate(currentTimeYear, currentTimeMonth, currentTimeDay,
                            out tempYesterdayYear, out tempYesterdayMonth, out tempYesterdayDay);//获取上一天的日期

                        MyTool.Other.TimeAddition(tempYesterdayYear, tempYesterdayMonth, tempYesterdayDay,
                        SettingsGeneralForm._clearYieldTimeHour + 12, SettingsGeneralForm._clearYieldTimeMinutes, SettingsGeneralForm._warningTimeAfterShiftWithoutSampleCheck,
                        out warningEndYear, out warningEndMonth, out warningEndDay, out warningEndHour, out warningEndMinutes);
                    }
                    else
                    {
                        MyTool.Other.TimeAddition(currentTimeYear, currentTimeMonth, currentTimeDay,
                        SettingsGeneralForm._clearYieldTimeHour + 12, SettingsGeneralForm._clearYieldTimeMinutes, SettingsGeneralForm._warningTimeAfterShiftWithoutSampleCheck,
                        out warningEndYear, out warningEndMonth, out warningEndDay, out warningEndHour, out warningEndMinutes);
                    }
                }

                //获取上次切换产品的时间
                ProductShiftTimeLoadFunc(0, out productShiftTimeYear, out productShiftTimeMonth, out productShiftTimeDay, out productShiftTimeHour, out productShiftTimeMinutes);
                //获取记录的上次样品板检测的时间及状态
                samplePanelCheckStatus = SamplePanelCheckTimeAndStatusLoadFunc(0, out samplePanelCheckResult, out samplePanelCheckTimeYear,
                    out samplePanelCheckTimeMonth, out samplePanelCheckTimeDay, out samplePanelCheckTimeHour, out samplePanelCheckTimeMinutes);

                /*
                 * 如果为自动状态，并且传送线使用，并且样品板检测结果对于当前时间段来说无效
                 * 并且当前时间段处于警告时间段
                 * 并且样品板切换时间小于样品板检测时间
                 * 并且样品板的检测结果是有效的，为false时有可能是第一次创建，也有可能是切换了产品
                */
                if ((_autoModeFlag && SettingsGeneralForm._conveyor1UsingFlag && !samplePanelCheckStatus &&
                    MyTool.Other.CompareTime(currentTimeYear, currentTimeMonth, currentTimeDay, currentTimeHour, currentTimeMinutes,
                    warningEndYear, warningEndMonth, warningEndDay, warningEndHour, warningEndMinutes) == 1 &&
                    MyTool.Other.CompareTime(productShiftTimeYear, productShiftTimeMonth, productShiftTimeDay, productShiftTimeHour, productShiftTimeMinutes,
                    samplePanelCheckTimeYear, samplePanelCheckTimeMonth, samplePanelCheckTimeDay, samplePanelCheckTimeHour, samplePanelCheckTimeMinutes) == 1)
                     && !SettingsGeneralForm._samplePanelCheckShieldFlag)//如果没有验证，并且当前轨道使用并且处于自动模式，并且当前时间没有超过警告上限
                {
                    _stateAndAlarmCs._alarmFlag[510] = true;
                }
                else
                {
                    _stateAndAlarmCs._alarmFlag[510] = false;
                }

                //获取上次切换产品的时间
                ProductShiftTimeLoadFunc(1, out productShiftTimeYear, out productShiftTimeMonth, out productShiftTimeDay, out productShiftTimeHour, out productShiftTimeMinutes);
                //获取记录的上次样品板检测的时间及状态
                samplePanelCheckStatus = SamplePanelCheckTimeAndStatusLoadFunc(1, out samplePanelCheckResult, out samplePanelCheckTimeYear,
                    out samplePanelCheckTimeMonth, out samplePanelCheckTimeDay, out samplePanelCheckTimeHour, out samplePanelCheckTimeMinutes);

                /*
                 * 如果为自动状态，并且传送线使用，并且样品板检测结果对于当前时间段来说无效
                 * 并且当前时间段处于警告时间段
                 * 并且样品板切换时间小于样品板检测时间
                 * 并且样品板的检测结果是有效的，为false时有可能是第一次创建，也有可能是切换了产品
                */
                if ((_autoModeFlag && SettingsGeneralForm._conveyor2UsingFlag && !samplePanelCheckStatus &&
                    MyTool.Other.CompareTime(currentTimeYear, currentTimeMonth, currentTimeDay, currentTimeHour, currentTimeMinutes,
                    warningEndYear, warningEndMonth, warningEndDay, warningEndHour, warningEndMinutes) == 1 &&
                     MyTool.Other.CompareTime(productShiftTimeYear, productShiftTimeMonth, productShiftTimeDay, productShiftTimeHour, productShiftTimeMinutes,
                    samplePanelCheckTimeYear, samplePanelCheckTimeMonth, samplePanelCheckTimeDay, samplePanelCheckTimeHour, samplePanelCheckTimeMinutes) == 1)
                    && !SettingsGeneralForm._samplePanelCheckShieldFlag)//如果没有验证，并且当前轨道使用并且处于自动模式，并且当前时间没有超过警告上限
                {
                    _stateAndAlarmCs._alarmFlag[511] = true;
                }
                else
                {
                    _stateAndAlarmCs._alarmFlag[511] = false;
                }

            #endregion
        }

        /// <summary>
        /// ConveyorLoadUnloadTimeoutInfoFormClickEnsureButtonEventFunc：传送线上下料报警消息提示框点击重试按钮事件函数
        /// </summary>
        private void ConveyorLoadUnloadTimeoutInfoFormClickEnsureButtonEventFunc()
        {
            switch (_conveyorLoadUnloadTimeoutInfoFormVariate.Text)//通过提示框的名称来判定是属于哪一个报警导致的
            {
                case "传送线1上料超时报警":
                    {
                        _conveyor1TimeoutAlarmDealWithWayIndexWhenLoadAndUnload = 0;//设置索引为0（传送线1超时报警点击重试按钮）
                        _conveyor1LoadTimeOutT.CURRENTTIME = 0;
                        _stateAndAlarmCs._alarmFlag[340] = false;
                        break;
                    }
                case "传送线1下料超时报警":
                    {
                        _conveyor1TimeoutAlarmDealWithWayIndexWhenLoadAndUnload = 0;//设置索引为0（传送线1超时报警点击重试按钮）
                        _conveyor1UnloadTimeOutT.CURRENTTIME = 0;
                        _stateAndAlarmCs._alarmFlag[341] = false;
                        break;
                    }
                case "传送线2上料超时报警":
                    {
                        _conveyor2TimeoutAlarmDealWithWayIndexWhenLoadAndUnload = 0;//设置索引为0（传送线2超时报警点击重试按钮）
                        _conveyor2LoadTimeOutT.CURRENTTIME = 0;
                        _stateAndAlarmCs._alarmFlag[342] = false;
                        break;
                    }
                case "传送线2下料超时报警":
                    {
                        _conveyor2TimeoutAlarmDealWithWayIndexWhenLoadAndUnload = 0;//设置索引为0（传送线2超时报警点击重试按钮）
                        _conveyor2UnloadTimeOutT.CURRENTTIME = 0;
                        _stateAndAlarmCs._alarmFlag[343] = false;
                        break;
                    }
            }

            if (_conveyorLoadUnloadTimeoutInfoFormVariate != null)
            {
                _conveyorLoadUnloadTimeoutInfoFormVariate.Invoke(new Action(() =>
                {
                    _conveyorLoadUnloadTimeoutInfoFormVariate.Dispose();
                    _conveyorLoadUnloadTimeoutInfoFormVariate = null;
                })); ;
            }
        }

        /// <summary>
        /// ConveyorLoadUnloadTimeoutInfoFormClickOutButtonEventFunc: 传送线上下料报警消息提示框点击忽略按钮事件函数
        /// </summary>
        private void ConveyorLoadUnloadTimeoutInfoFormClickOutButtonEventFunc()
        {
            switch (_conveyorLoadUnloadTimeoutInfoFormVariate.Text)
            {
                case "传送线1上料超时报警":
                    {
                        _conveyor1TimeoutAlarmDealWithWayIndexWhenLoadAndUnload = 1;//设置索引为1（传送线1超时报警点击忽略按钮）
                        _conveyor1LoadTimeOutT.CURRENTTIME = 0;
                        _stateAndAlarmCs._alarmFlag[340] = false;
                        break;
                    }
                case "传送线1下料超时报警":
                    {
                        _conveyor1TimeoutAlarmDealWithWayIndexWhenLoadAndUnload = 1;//设置索引为1（传送线1超时报警点击忽略按钮）
                        _conveyor1UnloadTimeOutT.CURRENTTIME = 0;
                        _stateAndAlarmCs._alarmFlag[341] = false;
                        break;
                    }
                case "传送线2上料超时报警":
                    {
                        _conveyor2TimeoutAlarmDealWithWayIndexWhenLoadAndUnload = 1;//设置索引为1（传送线2超时报警点击忽略按钮）
                        _conveyor2LoadTimeOutT.CURRENTTIME = 0;
                        _stateAndAlarmCs._alarmFlag[342] = false;
                        break;
                    }
                case "传送线2下料超时报警":
                    {
                        _conveyor2TimeoutAlarmDealWithWayIndexWhenLoadAndUnload = 1;//设置索引为1（传送线2超时报警点击忽略按钮）
                        _conveyor2UnloadTimeOutT.CURRENTTIME = 0;
                        _stateAndAlarmCs._alarmFlag[343] = false;
                        break;
                    }
            }

            if (_conveyorLoadUnloadTimeoutInfoFormVariate != null)
            {
                _conveyorLoadUnloadTimeoutInfoFormVariate.Invoke(new Action(() =>
                {
                    _conveyorLoadUnloadTimeoutInfoFormVariate.Dispose();
                    _conveyorLoadUnloadTimeoutInfoFormVariate = null;
                })); ;
            }
        }

        /// <summary>
        /// ConveyorLoadUnloadTimeoutInfoFormClickOutButtonEventFunc: 传送线上下料报警消息提示框点击无物料按钮事件函数
        /// </summary>
        private void ConveyorLoadUnloadTimeoutInfoFormClickTakeOutButtonEventFunc()
        {
            switch (_conveyorLoadUnloadTimeoutInfoFormVariate.Text)
            {
                case "传送线1上料超时报警":
                    {
                        _conveyor1TimeoutAlarmDealWithWayIndexWhenLoadAndUnload = 2;//设置索引为2（传送线1超时报警点击无产品按钮）
                        _conveyor1LoadTimeOutT.CURRENTTIME = 0;
                        _stateAndAlarmCs._alarmFlag[340] = false;
                        _conveyor1HaveProductFlag = false;
                        break;
                    }
                case "传送线1下料超时报警":
                    {
                        _conveyor1TimeoutAlarmDealWithWayIndexWhenLoadAndUnload = 2;//设置索引为2（传送线1超时报警点击无产品按钮）
                        _conveyor1UnloadTimeOutT.CURRENTTIME = 0;
                        _stateAndAlarmCs._alarmFlag[341] = false;
                        break;
                    }
                case "传送线2上料超时报警":
                    {
                        _conveyor2TimeoutAlarmDealWithWayIndexWhenLoadAndUnload = 2;//设置索引为2（传送线2超时报警点击无产品按钮）
                        _conveyor2LoadTimeOutT.CURRENTTIME = 0;
                        _stateAndAlarmCs._alarmFlag[342] = false;
                        _conveyor2HaveProductFlag = false;
                        break;
                    }
                case "传送线2下料超时报警":
                    {
                        _conveyor2TimeoutAlarmDealWithWayIndexWhenLoadAndUnload = 2;//设置索引为2（传送线2超时报警点击无产品按钮）
                        _conveyor2UnloadTimeOutT.CURRENTTIME = 0;
                        _stateAndAlarmCs._alarmFlag[343] = false;
                        break;
                    }
            }

            if (_conveyorLoadUnloadTimeoutInfoFormVariate != null)
            {
                _conveyorLoadUnloadTimeoutInfoFormVariate.Invoke(new Action(() =>
                {
                    _conveyorLoadUnloadTimeoutInfoFormVariate.Dispose();
                    _conveyorLoadUnloadTimeoutInfoFormVariate = null;
                })); ;
            }
        }

        /// <summary>
        /// ConveyorHaveExcessProductInfoFormClickOutButtonEventFunc:传送线有多余物料报警提示框点击流出按钮事件函数
        /// </summary>
        private void ConveyorHaveExcessProductInfoFormClickOutButtonEventFunc()
        {
            switch (_conveyorHaveExcessProductInfoFormVariate.Text)
            {
                case "传送线1有多余产品报警":
                    _conveyor1HaveExcessProductAlarmDealWithWayIndex = 1;
                    Thread.Sleep(100);
                    //停止传送线1
                    if (!_newDAM02AIAOOperation.comm.IsOpen)
                    {
                        if (!_newDAM02AIAOOperation.OpenSerialPort(SettingsGeneralForm._conveyorControlCom))
                        {
                            //MessageBox.Show("打开串口失败");
                        }
                    }

                    lock (this)
                    {
                        if (_newDAM02AIAOOperation.comm.IsOpen)
                        {
                            _newDAM02AIAOOperation.analogOutput(1, 0);
                        }
                        _axisControlFormVariate.OutputControl(71, false);
                    }

                    //如果传送线1上产品正处于检测状态
                    if (_conveyor1Status >= 100 && _conveyor1Status <= 104)
                    {
                        if (_axisControlFormVariate != null && _axisControlFormVariate._motionCardInitialStatus)
                        {
                            for (int i = 0; i < _axisQuantity; i++)
                            {
                                _axisControlFormVariate._axisMotionControlBoardVariate.jogAxisImmediateStop(i);
                            }

                        }
                        for (int i = 0; i < _axisQuantity; i++)
                        {
                            _axisControlFormVariate._axisActionCommand[i] = 0;
                        }
                    }
                    _conveyor1Status = 2;//设置传送线状态为需要下料

                    break;
                case "传送线2有多余产品报警":
                    _conveyor2HaveExcessProductAlarmDealWithWayIndex = 1;
                    Thread.Sleep(100);
                    //停止传送线2
                    if (!_newDAM02AIAOOperation.comm.IsOpen)
                    {
                        if (!_newDAM02AIAOOperation.OpenSerialPort(SettingsGeneralForm._conveyorControlCom))
                        {
                            //MessageBox.Show("打开串口失败");
                        }
                    }
                    lock (this)
                    {
                        if (_newDAM02AIAOOperation.comm.IsOpen)
                        {
                            _newDAM02AIAOOperation.analogOutput(0, 0);
                        }
                        _axisControlFormVariate.OutputControl(69, false);
                    }

                    //如果传送线2上产品正处于检测状态
                    if (_conveyor2Status >= 100 && _conveyor2Status <= 104)
                    {
                        if (_axisControlFormVariate != null && _axisControlFormVariate._motionCardInitialStatus)
                        {
                            for (int i = 0; i < _axisQuantity; i++)
                            {
                                _axisControlFormVariate._axisMotionControlBoardVariate.jogAxisImmediateStop(i);
                            }

                        }
                        for (int i = 0; i < _axisQuantity; i++)
                        {
                            _axisControlFormVariate._axisActionCommand[i] = 0;
                        }
                    }

                    _conveyor2Status = 2;//设置传送线状态为需要下料
                    break;
            }
            
            if (_conveyorHaveExcessProductInfoFormVariate != null)
            {
                _conveyorHaveExcessProductInfoFormVariate.Invoke(new Action(() =>
                {
                    _conveyorHaveExcessProductInfoFormVariate.Dispose();
                    _conveyorHaveExcessProductInfoFormVariate = null;
                })); ;
            }
        }

        /// <summary>
        /// ConveyorHaveExcessProductInfoFormTakeOutButtonEventFunc:传送线有多余物料报警提示框点击人工取走按钮事件函数
        /// </summary>
        private void ConveyorHaveExcessProductInfoFormTakeOutButtonEventFunc()
        {
            switch (_conveyorHaveExcessProductInfoFormVariate.Text)
            {
                case "传送线1有多余产品报警":
                    _conveyor1HaveExcessProductAlarmDealWithWayIndex = 0;
                    break;
                case "传送线2有多余产品报警":
                    _conveyor2HaveExcessProductAlarmDealWithWayIndex = 0;
                    break;
            }

            if (_conveyorHaveExcessProductInfoFormVariate != null)
            {
                _conveyorHaveExcessProductInfoFormVariate.Invoke(new Action(() =>
                {
                    _conveyorHaveExcessProductInfoFormVariate.Dispose();
                    _conveyorHaveExcessProductInfoFormVariate = null;
                })); ;
            }
        }

        #endregion

        #region Action Module

        /// <summary>
        /// MoveXYAxisToAssignedPointModule:移动XY轴至目标点位号
        /// </summary>
        /// <param name="destinationPointNo">int:目标点位号，实际点位号，从1开始</param>
        /// <param name="sourceDataIndex">int:初始数据索引，0：传送线1产品数据，1：传送线2产品数据，100-编辑产品数据</param>
        /// <param name="conveyorIndex">int:传送线索引,0-传送线1，1-传送线2</param>
        private void MoveXYAxisToAssignedPointModule(int destinationPointNo, int sourceDataIndex, int conveyorIndex)
        {
            _xyMoveToAssignedPointPositionModuleActionFlag = 0;//XY移动至指定点位启用标志FLAG

            //if (sourceDataIndex == 0)//如果为传送线1产品
            //    MyTool.TxtFileProcess.CreateLog("设备动作-移动检测头部至传送线" + (conveyorIndex + 1).ToString() + "上，产品“"
            //        + SettingsProductForm._track1ActualsProductName + "”的第" + destinationPointNo.ToString() + "个点位" +
            //        "，当前位置" + _axisControlFormVariate._axisMotionControlBoardVariate._axis00CurrentPosition_mm.ToString("f2") + "," + _axisControlFormVariate._axisMotionControlBoardVariate._axis01CurrentPosition_mm.ToString("f2")
            //        + "," + _axisControlFormVariate._axisMotionControlBoardVariate._axis02CurrentPosition_mm.ToString("f2")
            //        + "目标位置：" + _axisControlFormVariate._axisMotionControlBoardVariate.axis00TargetPosition_mm.ToString("f2") + "," + _axisControlFormVariate._axisMotionControlBoardVariate.axis01TargetPosition_mm.ToString("f2")
            //        + "," + _axisControlFormVariate._axisMotionControlBoardVariate.axis02TargetPosition_mm.ToString("f2") + "----用户：" + _operatorName,
            //    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize, false);
            //else if (sourceDataIndex == 1)//如果为传送线2产品
            //    MyTool.TxtFileProcess.CreateLog("设备动作-移动检测头部至传送线" + (conveyorIndex + 1).ToString() + "上，产品“"
            //       + SettingsProductForm._track2ActualsProductName + "”的第" + destinationPointNo.ToString() + "个点位" +
            //       "，当前位置" + _axisControlFormVariate._axisMotionControlBoardVariate._axis00CurrentPosition_mm.ToString("f2") + "," + _axisControlFormVariate._axisMotionControlBoardVariate._axis01CurrentPosition_mm.ToString("f2")
            //       + "," + _axisControlFormVariate._axisMotionControlBoardVariate._axis02CurrentPosition_mm.ToString("f2")
            //       + "目标位置：" + _axisControlFormVariate._axisMotionControlBoardVariate.axis00TargetPosition_mm.ToString("f2") + "," + _axisControlFormVariate._axisMotionControlBoardVariate.axis01TargetPosition_mm.ToString("f2")
            //       + "," + _axisControlFormVariate._axisMotionControlBoardVariate.axis02TargetPosition_mm.ToString("f2") + "----用户：" + _operatorName,
            //       BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize, false);
            //else
            //    MyTool.TxtFileProcess.CreateLog("设备动作-移动检测头部至传送线" + (conveyorIndex + 1).ToString() + "上，产品“"
            //       + SettingsProductForm._editProductName + "”的第" + destinationPointNo.ToString() + "个点位" +
            //       "，当前位置" + _axisControlFormVariate._axisMotionControlBoardVariate._axis00CurrentPosition_mm.ToString("f2") + "," + _axisControlFormVariate._axisMotionControlBoardVariate._axis01CurrentPosition_mm.ToString("f2")
            //       + "," + _axisControlFormVariate._axisMotionControlBoardVariate._axis02CurrentPosition_mm.ToString("f2")
            //       + "目标位置：" + _axisControlFormVariate._axisMotionControlBoardVariate.axis00TargetPosition_mm.ToString("f2") + "," + _axisControlFormVariate._axisMotionControlBoardVariate.axis01TargetPosition_mm.ToString("f2")
            //       + "," + _axisControlFormVariate._axisMotionControlBoardVariate.axis02TargetPosition_mm.ToString("f2") + "----用户：" + _operatorName,
            //       BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize, false);

            float xAxisActionVelocity_mmps, xAxisActionAcc_s;
            float yAxisActionVelocity_mmps, yAxisActionAcc_s;
            float rAxisActionVelocity_mmps, rAxisActionAcc_s;
            float xAxisDestinationPosition_mm = 0, yAxisDestinationPosition_mm = 0, rAxisDestinationPosition_mm = 0;
            bool manualActionFlag = false;//用于判别是自动模式触发还是手动模式

            lock (this)
            {
                switch (sourceDataIndex)
                {
                    case 0://如果为传送线1
                    case 1://如果为传送线2
                        xAxisActionVelocity_mmps = _productParameter[sourceDataIndex]._x1AxisLocationVelocity;
                        yAxisActionVelocity_mmps = _productParameter[sourceDataIndex]._y1AxisLocationVelocity;
                        rAxisActionVelocity_mmps = _productParameter[sourceDataIndex]._r1AxisLocationVelocity;
                        xAxisActionAcc_s = _productParameter[sourceDataIndex]._x1AxisLocationAccTime;
                        yAxisActionAcc_s = _productParameter[sourceDataIndex]._y1AxisLocationAccTime;
                        rAxisActionAcc_s = _productParameter[sourceDataIndex]._r1AxisLocationAccTime;
                        xAxisDestinationPosition_mm = Convert.ToSingle(_productParameter[sourceDataIndex]._pointPositionDT.Rows[destinationPointNo - 1][1]);
                        yAxisDestinationPosition_mm = Convert.ToSingle(_productParameter[sourceDataIndex]._pointPositionDT.Rows[destinationPointNo - 1][2]);
                        rAxisDestinationPosition_mm = Convert.ToSingle(_productParameter[sourceDataIndex]._pointPositionDT.Rows[destinationPointNo - 1][3]);
                        if (conveyorIndex == 0)//如果为传送线1
                        {
                            xAxisDestinationPosition_mm += _productParameter[sourceDataIndex]._track1X1TeachedCoor -
                                Convert.ToSingle(_productParameter[sourceDataIndex]._pointPositionDT.Rows[_productParameter[sourceDataIndex]._track1TeachedPointNo - 1][1]);//添加上X偏移值
                            yAxisDestinationPosition_mm += _productParameter[sourceDataIndex]._track1Y1TeachedCoor -
                                Convert.ToSingle(_productParameter[sourceDataIndex]._pointPositionDT.Rows[_productParameter[sourceDataIndex]._track1TeachedPointNo - 1][2]);//添加上Y偏移值
                            rAxisDestinationPosition_mm += _productParameter[sourceDataIndex]._track1R1TeachedCoor -
                                Convert.ToSingle(_productParameter[sourceDataIndex]._pointPositionDT.Rows[_productParameter[sourceDataIndex]._track1TeachedPointNo - 1][3]);//添加上R偏移值
                        }
                        else if (conveyorIndex == 1)//如果为传送线2
                        {
                            xAxisDestinationPosition_mm += _productParameter[sourceDataIndex]._track2X1TeachedCoor -
                                Convert.ToSingle(_productParameter[sourceDataIndex]._pointPositionDT.Rows[_productParameter[sourceDataIndex]._track2TeachedPointNo - 1][1]);//添加上X偏移值
                            yAxisDestinationPosition_mm += _productParameter[sourceDataIndex]._track2Y1TeachedCoor -
                                Convert.ToSingle(_productParameter[sourceDataIndex]._pointPositionDT.Rows[_productParameter[sourceDataIndex]._track2TeachedPointNo - 1][2]);//添加上Y偏移值
                            rAxisDestinationPosition_mm += _productParameter[sourceDataIndex]._track2R1TeachedCoor -
                                Convert.ToSingle(_productParameter[sourceDataIndex]._pointPositionDT.Rows[_productParameter[sourceDataIndex]._track2TeachedPointNo - 1][3]);//添加上R偏移值
                        }

                        xAxisDestinationPosition_mm = xAxisDestinationPosition_mm + _markSearchXOffsetData;
                        yAxisDestinationPosition_mm = yAxisDestinationPosition_mm + _markSearchYOffsetData;
                        break;
                    case 100:
                        xAxisActionVelocity_mmps = SettingsProductForm._editProductParameter._x1AxisLocationVelocity;
                        yAxisActionVelocity_mmps = SettingsProductForm._editProductParameter._y1AxisLocationVelocity;
                        rAxisActionVelocity_mmps = SettingsProductForm._editProductParameter._r1AxisLocationVelocity;
                        xAxisActionAcc_s = SettingsProductForm._editProductParameter._x1AxisLocationAccTime;
                        yAxisActionAcc_s = SettingsProductForm._editProductParameter._y1AxisLocationAccTime;
                        rAxisActionAcc_s = SettingsProductForm._editProductParameter._r1AxisLocationAccTime;
                        xAxisDestinationPosition_mm = Convert.ToSingle(SettingsProductForm._editProductParameter._pointPositionDT.Rows[destinationPointNo - 1][1]);
                        yAxisDestinationPosition_mm = Convert.ToSingle(SettingsProductForm._editProductParameter._pointPositionDT.Rows[destinationPointNo - 1][2]);
                        rAxisDestinationPosition_mm = Convert.ToSingle(SettingsProductForm._editProductParameter._pointPositionDT.Rows[destinationPointNo - 1][3]);
                        if (conveyorIndex == 0)//如果为传送线1
                        {
                            xAxisDestinationPosition_mm += SettingsProductForm._editProductParameter._track1X1TeachedCoor -
                                Convert.ToSingle(SettingsProductForm._editProductParameter._pointPositionDT.Rows[SettingsProductForm._editProductParameter._track1TeachedPointNo - 1][1]);//添加上X偏移值
                            yAxisDestinationPosition_mm += SettingsProductForm._editProductParameter._track1Y1TeachedCoor -
                                Convert.ToSingle(SettingsProductForm._editProductParameter._pointPositionDT.Rows[SettingsProductForm._editProductParameter._track1TeachedPointNo - 1][2]);//添加上Y偏移值
                            rAxisDestinationPosition_mm += SettingsProductForm._editProductParameter._track1R1TeachedCoor -
                                Convert.ToSingle(SettingsProductForm._editProductParameter._pointPositionDT.Rows[SettingsProductForm._editProductParameter._track1TeachedPointNo - 1][3]);//添加上R偏移值
                        }
                        else if (conveyorIndex == 1)//如果为传送线2
                        {
                            xAxisDestinationPosition_mm += SettingsProductForm._editProductParameter._track2X1TeachedCoor -
                                Convert.ToSingle(SettingsProductForm._editProductParameter._pointPositionDT.Rows[SettingsProductForm._editProductParameter._track2TeachedPointNo - 1][1]);//添加上X偏移值
                            yAxisDestinationPosition_mm += SettingsProductForm._editProductParameter._track2Y1TeachedCoor -
                                Convert.ToSingle(SettingsProductForm._editProductParameter._pointPositionDT.Rows[SettingsProductForm._editProductParameter._track2TeachedPointNo - 1][2]);//添加上Y偏移值
                            rAxisDestinationPosition_mm += SettingsProductForm._editProductParameter._track2R1TeachedCoor -
                                Convert.ToSingle(SettingsProductForm._editProductParameter._pointPositionDT.Rows[SettingsProductForm._editProductParameter._track2TeachedPointNo - 1][3]);//添加上R偏移值
                        }
                        manualActionFlag = true;
                        break;
                    default:
                        return;
                }

                if (!SettingsProductForm._useRelativePos)//如果使用相对位置
                {
                    xAxisDestinationPosition_mm += SettingsAdministratorForm._absoluteCoor[0];//添加上相对位置X坐标
                    yAxisDestinationPosition_mm += SettingsAdministratorForm._absoluteCoor[1];//添加上相对位置Y坐标
                    rAxisDestinationPosition_mm += SettingsAdministratorForm._absoluteCoor[2];//添加上相对位置R坐标
                }
            }

            //while ((_axisControlFormVariate._axisMotionControlBoardVariate._axis00CheckDoneStatus == 0 || _axisControlFormVariate._axisMotionControlBoardVariate._axis00Command != 0 || _axisControlFormVariate._axisMotionControlBoardVariate._axis00CommandSaved != 0) ||
            //(_axisControlFormVariate._axisMotionControlBoardVariate._axis01CheckDoneStatus == 0 || _axisControlFormVariate._axisMotionControlBoardVariate._axis01Command != 0 || _axisControlFormVariate._axisMotionControlBoardVariate._axis01CommandSaved != 0) ||
            //(_axisControlFormVariate._axisMotionControlBoardVariate._axis02CheckDoneStatus == 0 || _axisControlFormVariate._axisMotionControlBoardVariate._axis02Command != 0 || _axisControlFormVariate._axisMotionControlBoardVariate._axis02CommandSaved != 0) ||
            //!_deviceWorkTriggerFlag)//等待轴运行结束
            //while ((_axisControlFormVariate._axisMotionControlBoardVariate._axis00CurrentPositionNumber == 0 || _axisControlFormVariate._axisMotionControlBoardVariate._axis00Command == 1000) ||
            //        (_axisControlFormVariate._axisMotionControlBoardVariate._axis01CurrentPositionNumber == 0 || _axisControlFormVariate._axisMotionControlBoardVariate._axis01Command == 1000) ||
            //        (_axisControlFormVariate._axisMotionControlBoardVariate._axis02CurrentPositionNumber == 0 || _axisControlFormVariate._axisMotionControlBoardVariate._axis02Command == 1000) ||
            //        !_deviceWorkTriggerFlag)//等待轴运行结束

            while (_axisControlFormVariate._axisActionCommand[0] != -1 ||
            _axisControlFormVariate._axisActionCommand[1] != -1 ||
            _axisControlFormVariate._axisActionCommand[2] != -1 ||
            !_deviceWorkTriggerFlag ||
            (conveyorIndex == 0 && _conveyor1HaveExcessProductAlarmDealWithWayIndex == 1) ||
            (conveyorIndex == 1 && _conveyor2HaveExcessProductAlarmDealWithWayIndex == 1))
            {
                if (_deadlyAlarmOccurFlag || (!_autoModeFlag && !manualActionFlag) || _heavyAlarmOccurFlag ||
                    (conveyorIndex == 0 && _conveyor1HaveExcessProductAlarmDealWithWayIndex == 1) ||
                    (conveyorIndex == 1 && _conveyor2HaveExcessProductAlarmDealWithWayIndex == 1))//如果触发了致命报警
                {
                    _xyMoveToAssignedPointPositionModuleActionFlag = -1;
                    return;
                }

                Thread.Sleep(1);
            }

            lock (this)//确保数据传输的时候不会被别的线程中断
            {
                if (!_deadlyAlarmOccurFlag && (_autoModeFlag || manualActionFlag) && !_heavyAlarmOccurFlag)//X轴移动至指定坐标位置
                {
                    _axisControlFormVariate._axisMotionControlBoardVariate._axis00CurrentPositionNumber = 0;
                    _axisControlFormVariate._axisCurrentPositionNumber[0] = 0;
                    _axisControlFormVariate._assignedPositionActionTargetPosition_mm[0] = xAxisDestinationPosition_mm;
                    _axisControlFormVariate._assignedPositionActionMaxVelocity_mmps[0] = xAxisActionVelocity_mmps;
                    _axisControlFormVariate._assignedPositionActionAccTime_s[0] = xAxisActionAcc_s;
                    _axisControlFormVariate._assignedPositionActionDeviation[0] = 0f;
                    _axisControlFormVariate._assignedPositionActionSMoveFlag[0] = false;
                    _axisControlFormVariate._assignedPositionActionSMoveTime[0] = 0.01f;
                    _axisControlFormVariate._axisActionCommand[0] = 1000;//指定点位移动
                }

                if (!_deadlyAlarmOccurFlag && (_autoModeFlag || manualActionFlag) && !_heavyAlarmOccurFlag)//Y轴移动至指定坐标位置
                {
                    _axisControlFormVariate._axisMotionControlBoardVariate._axis01CurrentPositionNumber = 0;
                    _axisControlFormVariate._axisCurrentPositionNumber[1] = 0;
                    _axisControlFormVariate._assignedPositionActionTargetPosition_mm[1] = yAxisDestinationPosition_mm;
                    _axisControlFormVariate._assignedPositionActionMaxVelocity_mmps[1] = yAxisActionVelocity_mmps;
                    _axisControlFormVariate._assignedPositionActionAccTime_s[1] = yAxisActionAcc_s;
                    _axisControlFormVariate._assignedPositionActionDeviation[1] = 0f;
                    _axisControlFormVariate._assignedPositionActionSMoveFlag[1] = false;
                    _axisControlFormVariate._assignedPositionActionSMoveTime[1] = 0.01f;
                    _axisControlFormVariate._axisActionCommand[1] = 1000;//指定点位移动
                }

                if (!_deadlyAlarmOccurFlag && (_autoModeFlag || manualActionFlag) && !_heavyAlarmOccurFlag)//R轴移动至指定坐标位置
                {
                    _axisControlFormVariate._axisMotionControlBoardVariate._axis02CurrentPositionNumber = 0;
                    _axisControlFormVariate._axisCurrentPositionNumber[2] = 0;
                    _axisControlFormVariate._assignedPositionActionTargetPosition_mm[2] = rAxisDestinationPosition_mm;
                    _axisControlFormVariate._assignedPositionActionMaxVelocity_mmps[2] = rAxisActionVelocity_mmps;
                    _axisControlFormVariate._assignedPositionActionAccTime_s[2] = rAxisActionAcc_s;
                    _axisControlFormVariate._assignedPositionActionDeviation[2] = 0f;
                    _axisControlFormVariate._assignedPositionActionSMoveFlag[2] = false;
                    _axisControlFormVariate._assignedPositionActionSMoveTime[2] = 0.01f;
                    _axisControlFormVariate._axisActionCommand[2] = 1000;//指定点位移动
                }
            }

            //while ((_axisControlFormVariate._axisMotionControlBoardVariate._axis00CheckDoneStatus == 0 || _axisControlFormVariate._axisMotionControlBoardVariate._axis00Command != 0 || _axisControlFormVariate._axisMotionControlBoardVariate._axis00CommandSaved != 0) ||
            //(_axisControlFormVariate._axisMotionControlBoardVariate._axis01CheckDoneStatus == 0 || _axisControlFormVariate._axisMotionControlBoardVariate._axis01Command != 0 || _axisControlFormVariate._axisMotionControlBoardVariate._axis01CommandSaved != 0) ||
            //(_axisControlFormVariate._axisMotionControlBoardVariate._axis02CheckDoneStatus == 0 || _axisControlFormVariate._axisMotionControlBoardVariate._axis02Command != 0 || _axisControlFormVariate._axisMotionControlBoardVariate._axis02CommandSaved != 0) ||
            // //!_deviceWorkTriggerFlag)//等待轴运行结束
            // while ((_axisControlFormVariate._axisMotionControlBoardVariate._axis00CurrentPositionNumber == 0 || _axisControlFormVariate._axisMotionControlBoardVariate._axis00Command == 1000) ||
            //(_axisControlFormVariate._axisMotionControlBoardVariate._axis01CurrentPositionNumber == 0 || _axisControlFormVariate._axisMotionControlBoardVariate._axis01Command == 1000) ||
            //(_axisControlFormVariate._axisMotionControlBoardVariate._axis02CurrentPositionNumber == 0 || _axisControlFormVariate._axisMotionControlBoardVariate._axis02Command == 1000) ||
            ////!_deviceWorkTriggerFlag)//等待轴运行结束
            //while ((_axisControlFormVariate._axisCurrentPositionNumber[0] == 0 || _axisControlFormVariate._axisActionCommand[0] != -1) ||
            //    (_axisControlFormVariate._axisCurrentPositionNumber[1] == 0 || _axisControlFormVariate._axisActionCommand[1] != -1) ||
            //    (_axisControlFormVariate._axisCurrentPositionNumber[2] == 0 || _axisControlFormVariate._axisActionCommand[2] != -1) ||
            //    !_deviceWorkTriggerFlag)//等待XY轴运行结束
            //Thread.Sleep(10);//增加一个10ms延时，用于等待数据确实传输至了运动控制卡，并且启动了运动
            while (Math.Abs(_axisControlFormVariate._axisMotionControlBoardVariate._axis00CurrentPosition_mm - xAxisDestinationPosition_mm) > 0.1 ||
                Math.Abs(_axisControlFormVariate._axisMotionControlBoardVariate._axis01CurrentPosition_mm - yAxisDestinationPosition_mm) > 0.1 ||
                Math.Abs(_axisControlFormVariate._axisMotionControlBoardVariate._axis02CurrentPosition_mm - rAxisDestinationPosition_mm) > 0.1 ||
                _axisControlFormVariate._axisMotionControlBoardVariate._axis00CheckDoneStatus == 0 ||
                _axisControlFormVariate._axisMotionControlBoardVariate._axis01CheckDoneStatus == 0 ||
                _axisControlFormVariate._axisMotionControlBoardVariate._axis02CheckDoneStatus == 0 ||
                _axisControlFormVariate._axisMotionControlBoardVariate._axis00CurrentPositionNumber == 0 ||
                _axisControlFormVariate._axisMotionControlBoardVariate._axis01CurrentPositionNumber == 0 ||
                _axisControlFormVariate._axisMotionControlBoardVariate._axis02CurrentPositionNumber == 0 ||
                !_deviceWorkTriggerFlag ||
                (conveyorIndex == 0 && _conveyor1HaveExcessProductAlarmDealWithWayIndex == 1) ||
                (conveyorIndex == 1 && _conveyor2HaveExcessProductAlarmDealWithWayIndex == 1))//等待轴运行结束
            {
                if (_deadlyAlarmOccurFlag || (!_autoModeFlag && !manualActionFlag) || _heavyAlarmOccurFlag ||
                    (conveyorIndex == 0 && _conveyor1HaveExcessProductAlarmDealWithWayIndex == 1) ||
                    (conveyorIndex == 1 && _conveyor2HaveExcessProductAlarmDealWithWayIndex == 1))//如果触发了致命报警
                {
                    _xyMoveToAssignedPointPositionModuleActionFlag = -1;
                    return;
                }
                Thread.Sleep(1);
            }

            lock (this)
            {
                if (_deadlyAlarmOccurFlag || (!_autoModeFlag && !manualActionFlag) || _heavyAlarmOccurFlag)//如果触发了致命报警
                {
                    _xyMoveToAssignedPointPositionModuleActionFlag = -1;
                    return; 
                }

                if (!_deadlyAlarmOccurFlag && (_autoModeFlag || manualActionFlag) && !_heavyAlarmOccurFlag)
                {

                    //if (sourceDataIndex == 0)//如果为传送线1产品
                    //    MyTool.TxtFileProcess.CreateLog("设备动作-移动检测头部至传送线" + (conveyorIndex + 1).ToString() + "上，产品“"
                    //        + SettingsProductForm._track1ActualsProductName + "”的第" + destinationPointNo.ToString() + "个点位完成" +
                    //        "，当前位置" + _axisControlFormVariate._axisMotionControlBoardVariate._axis00CurrentPosition_mm.ToString("f2") + "," + _axisControlFormVariate._axisMotionControlBoardVariate._axis01CurrentPosition_mm.ToString("f2")
                    //        + "," + _axisControlFormVariate._axisMotionControlBoardVariate._axis02CurrentPosition_mm.ToString("f2")
                    //        + "目标位置：" + _axisControlFormVariate._axisMotionControlBoardVariate.axis00TargetPosition_mm.ToString("f2") + "," + _axisControlFormVariate._axisMotionControlBoardVariate.axis01TargetPosition_mm.ToString("f2")
                    //        + "," + _axisControlFormVariate._axisMotionControlBoardVariate.axis02TargetPosition_mm.ToString("f2") + "----用户：" + _operatorName,
                    //        BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize, false);
                    //else if (sourceDataIndex == 1)//如果为传送线2产品
                    //    MyTool.TxtFileProcess.CreateLog("设备动作-移动检测头部至传送线" + (conveyorIndex + 1).ToString() + "上，产品“"
                    //       + SettingsProductForm._track2ActualsProductName + "”的第" + destinationPointNo.ToString() + "个点位完成" +
                    //       "，当前位置" + _axisControlFormVariate._axisMotionControlBoardVariate._axis00CurrentPosition_mm.ToString("f2") + "," + _axisControlFormVariate._axisMotionControlBoardVariate._axis01CurrentPosition_mm.ToString("f2")
                    //       + "," + _axisControlFormVariate._axisMotionControlBoardVariate._axis02CurrentPosition_mm.ToString("f2")
                    //       + "目标位置：" + _axisControlFormVariate._axisMotionControlBoardVariate.axis00TargetPosition_mm.ToString("f2") + "," + _axisControlFormVariate._axisMotionControlBoardVariate.axis01TargetPosition_mm.ToString("f2")
                    //       + "," + _axisControlFormVariate._axisMotionControlBoardVariate.axis02TargetPosition_mm.ToString("f2") + "----用户：" + _operatorName,
                    //       BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize, false);
                    //else
                    //    MyTool.TxtFileProcess.CreateLog("设备动作-移动检测头部至传送线" + (conveyorIndex + 1).ToString() + "上，产品“"
                    //       + SettingsProductForm._editProductName + "”的第" + destinationPointNo.ToString() + "个点位完成" +
                    //       "，当前位置" + _axisControlFormVariate._axisMotionControlBoardVariate._axis00CurrentPosition_mm.ToString("f2") + "," + _axisControlFormVariate._axisMotionControlBoardVariate._axis01CurrentPosition_mm.ToString("f2")
                    //    + "," + _axisControlFormVariate._axisMotionControlBoardVariate._axis02CurrentPosition_mm.ToString("f2")
                    //    + "目标位置：" + _axisControlFormVariate._axisMotionControlBoardVariate.axis00TargetPosition_mm.ToString("f2") + "," + _axisControlFormVariate._axisMotionControlBoardVariate.axis01TargetPosition_mm.ToString("f2")
                    //    + "," + _axisControlFormVariate._axisMotionControlBoardVariate.axis02TargetPosition_mm.ToString("f2") + "----用户：" + _operatorName,
                    //    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize, false);

                    _xyMoveToAssignedPointPositionModuleActionFlag = destinationPointNo;
                }
            }
        }

        /// <summary>
        /// AllAxesHomeBackModule:所有轴原点复归模块
        /// </summary>
        private async void AllAxesHomeBackModule()
        {
            await Task.Run(() =>
            {
                _allAxisHomeBackModuleActionFlag = 0;
                MyTool.TxtFileProcess.CreateLog("设备动作-设备整机回原！" + "----用户：" + _operatorName,
                        BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

                bool homeBackPressedStartBtnFlag = false;//原点复归请求按下启动按钮后标志

                while (!homeBackPressedStartBtnFlag)
                {
                    if (_startBtnLampOnOffFlag != 2 && !_afterPressResetBtnNeedPressStartBtnFlag)
                        _startBtnLampOnOffFlag = 2;

                    if (_inputIOT[START_BT_IN_IO_NO].STATUS)
                    {
                        homeBackPressedStartBtnFlag = true;
                    }

                    if (_deadlyAlarmOccurFlag || !_autoModeFlag)//如果触发了紧急停止
                    {
                        return;
                    }
                    Thread.Sleep(1);
                }
                _startBtnLampOnOffFlag = 1;//切换回常亮

                #region 气缸回原点位

                if (!SettingsAdministratorForm._checkProductIsConnector)//如果为switch
                {
                    //相机移动气缸出
                    _axisControlFormVariate.OutputControl(4, false);
                    _axisControlFormVariate.OutputControl(5, true);
                }
                else
                {
                    //相机移动气缸回
                    _axisControlFormVariate.OutputControl(4, true);
                    _axisControlFormVariate.OutputControl(5, false);
                }

                if (!_conveyor1HaveProductFlag)//防止传送线上有产品的时候阻挡气缸降下
                {
                    //传送线1阻挡气缸下
                    _axisControlFormVariate.OutputControl(76, true);
                    _axisControlFormVariate.OutputControl(77, false);

                    //传送线1夹紧气缸松开
                    _axisControlFormVariate.OutputControl(78, true);
                    _axisControlFormVariate.OutputControl(79, false);
                }

                if (!_conveyor2HaveProductFlag)//防止传送线上有产品的时候阻挡气缸降下
                {
                    //传送线2阻挡气缸下
                    _axisControlFormVariate.OutputControl(80, true);
                    _axisControlFormVariate.OutputControl(81, false);

                    //传送线2夹紧气缸松开
                    _axisControlFormVariate.OutputControl(82, true);
                    _axisControlFormVariate.OutputControl(83, false);
                }

                while (((!_inputIOT[80].STATUS || !_inputIOT[82].STATUS) && !_conveyor1HaveProductFlag) ||
                    ((!_inputIOT[84].STATUS || !_inputIOT[86].STATUS) && !_conveyor2HaveProductFlag) ||
                    (!_inputIOT[14].STATUS && !SettingsAdministratorForm._checkProductIsConnector) ||
                    (!_inputIOT[13].STATUS && SettingsAdministratorForm._checkProductIsConnector) || !_deviceWorkTriggerFlag)
                {
                    if (_deadlyAlarmOccurFlag || !_autoModeFlag)//如果触发了紧急停止
                    {
                        return;
                    }
                    Thread.Sleep(1);
                }
                #endregion

                while (!_outputIOT[0].STATUS || !_outputIOT[1].STATUS || !_outputIOT[2].STATUS)//当XYR轴有伺服未使能
                {
                    _axisControlFormVariate.OutputControl(0, true);
                    _axisControlFormVariate.OutputControl(1, true);
                    _axisControlFormVariate.OutputControl(2, true);
                    if (_deadlyAlarmOccurFlag || !_autoModeFlag)//如果触发了紧急停止
                    {
                        return;
                    }
                    Thread.Sleep(1);
                }

                #region 请求X1Y1R1Y2Y3Y4轴回原点

                while (_axisControlFormVariate._axisActionCommand[0] != -1 ||
                    _axisControlFormVariate._axisActionCommand[1] != -1 ||
                    _axisControlFormVariate._axisActionCommand[2] != -1 ||
                    _axisControlFormVariate._axisActionCommand[3] != -1 ||
                    _axisControlFormVariate._axisActionCommand[4] != -1 ||
                    _axisControlFormVariate._axisActionCommand[5] != -1 ||
                    !_deviceWorkTriggerFlag)//当X1Y1R1Y2Y3Y4轴处于运动指令状态,循环等待
                {
                    if (_deadlyAlarmOccurFlag || !_autoModeFlag)//如果触发了紧急停止
                    {
                        return;
                    }
                    Thread.Sleep(1);
                }

                //请求XY轴回原点
                //X1
                if (!_axisControlFormVariate._axisHomedFlag[0])
                {
                    _axisControlFormVariate._axisCurrentPositionNumber[0] = 0;
                    _axisControlFormVariate._axisActionCommand[0] = 999;
                }
                //Y1
                if (!_axisControlFormVariate._axisHomedFlag[1])
                {
                    _axisControlFormVariate._axisCurrentPositionNumber[1] = 0;
                    _axisControlFormVariate._axisActionCommand[1] = 999;
                }
                //R1
                if (!_axisControlFormVariate._axisHomedFlag[2])
                {
                    _axisControlFormVariate._axisCurrentPositionNumber[2] = 0;
                    _axisControlFormVariate._axisActionCommand[2] = 999;
                }
                //if (!_axisControlFormVariate._axisHomedFlag[3] || !_axisControlFormVariate._axisHomedFlag[4] || !_axisControlFormVariate._axisHomedFlag[5])
                //{
                //    _axisControlFormVariate.OutputControl(22, true);//打开Y2,Y3,Y4的抱闸
                //    while (!_outputIOT[22].STATUS)
                //    {
                //        if (_deadlyAlarmOccurFlag || !_autoModeFlag)//如果触发了紧急停止
                //        {
                //            return;
                //        }
                //        Thread.Sleep(10);
                //    }
                //}
                ////Y2
                //if (!_axisControlFormVariate._axisHomedFlag[3])
                //{
                //    _axisControlFormVariate._axisCurrentPositionNumber[3] = 0;
                //    _axisControlFormVariate._axisActionCommand[3] = 999;
                //}
                ////Y3
                //if (!_axisControlFormVariate._axisHomedFlag[4])
                //{
                //    _axisControlFormVariate._axisCurrentPositionNumber[4] = 0;
                //    _axisControlFormVariate._axisActionCommand[4] = 999;
                //}
                ////Y4
                //if (!_axisControlFormVariate._axisHomedFlag[5])
                //{
                //    _axisControlFormVariate._axisCurrentPositionNumber[5] = 0;
                //    _axisControlFormVariate._axisActionCommand[5] = 999;
                //}
                //(_axisControlFormVariate._axisCurrentPositionNumber[3] == 0 || _axisControlFormVariate._axisActionCommand[3] == 999) ||
                //   (_axisControlFormVariate._axisCurrentPositionNumber[4] == 0 || _axisControlFormVariate._axisActionCommand[4] == 999) ||
                //   (_axisControlFormVariate._axisCurrentPositionNumber[5] == 0 || _axisControlFormVariate._axisActionCommand[5] == 999) ||
                while (!_axisControlFormVariate._axisHomedFlag[0] || _axisControlFormVariate._axisActionCommand[0] != -1
                    || !_axisControlFormVariate._axisHomedFlag[1] || _axisControlFormVariate._axisActionCommand[1] != -1
                    || !_axisControlFormVariate._axisHomedFlag[2] || _axisControlFormVariate._axisActionCommand[2] != -1 ||
                    !_deviceWorkTriggerFlag)//等待X1Y1R1Y2Y3Y4轴运行结束
                {
                    if (_deadlyAlarmOccurFlag || !_autoModeFlag)//如果触发了紧急停止
                    {
                        return;
                    }
                    Thread.Sleep(1);
                }

                #endregion

                //_axisControlFormVariate.OutputControl(22, false);//关闭Y2Y3Y4的抱闸
                //while (_outputIOT[22].STATUS)
                //{
                //    if (_deadlyAlarmOccurFlag || !_autoModeFlag)//如果触发了紧急停止
                //    {
                //        return;
                //    }
                //    Thread.Sleep(10);
                //}

                MyTool.TxtFileProcess.CreateLog("设备动作-设备整机回原完成！" + "----用户：" + _operatorName,
                BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                _allAxisHomeBackModuleActionFlag = 1;

                if (!_conveyor1HaveProductFlag && !_conveyor2HaveProductFlag)//当传送线上没有物料时
                {
                    //完成原点复归之后，各轴回等待位置
                    while (_axesGotoStandbyModuleActionFlag == 0 || !_deviceWorkTriggerFlag)
                    {
                        if (_deadlyAlarmOccurFlag || !_autoModeFlag)//如果触发了紧急停止
                        {
                            return;
                        }
                        Thread.Sleep(1);
                    }

                    _axesGotoStandbyModuleActionFlag = 0;
                    AxesGotoStandbyModule();

                    while (_axesGotoStandbyModuleActionFlag == 0 || !_deviceWorkTriggerFlag)
                    {
                        if (_deadlyAlarmOccurFlag || !_autoModeFlag)//如果触发了紧急停止
                        {
                            return;
                        }
                        Thread.Sleep(1);
                    }
                }

            });
        }

        /// <summary>
        /// AxesGotoStandbyModule:轴返回等待位置模块
        /// </summary>
        private void AxesGotoStandbyModule()
        {
            _axesGotoStandbyModuleActionFlag = 0;
            MyTool.TxtFileProcess.CreateLog("设备动作-设备移动至待机位置！" + "----用户：" + _operatorName,
            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

            while (_axisControlFormVariate._axisActionCommand[0] != -1 ||
                    _axisControlFormVariate._axisActionCommand[1] != -1 ||
                    _axisControlFormVariate._axisActionCommand[2] != -1 ||
                    !_deviceWorkTriggerFlag)//当XY轴处于运动指令状态,循环等待
            {
                if (_deadlyAlarmOccurFlag || !_autoModeFlag)//如果触发了紧急停止
                {
                    return;
                }
                Thread.Sleep(1);
            }

            //请求XY轴回等待位置
            //X1
            _axisControlFormVariate._axisCurrentPositionNumber[0] = 0;
            _axisControlFormVariate._axisActionCommand[0] = 1;
            //Y1
            _axisControlFormVariate._axisCurrentPositionNumber[1] = 0;
            _axisControlFormVariate._axisActionCommand[1] = 1;
            //R1
            _axisControlFormVariate._axisCurrentPositionNumber[2] = 0;
            _axisControlFormVariate._axisActionCommand[2] = 1;

            while ((_axisControlFormVariate._axisCurrentPositionNumber[0] == 0 || _axisControlFormVariate._axisActionCommand[0] == 1) ||
                (_axisControlFormVariate._axisCurrentPositionNumber[1] == 0 || _axisControlFormVariate._axisActionCommand[1] == 1) ||
                (_axisControlFormVariate._axisCurrentPositionNumber[2] == 0 || _axisControlFormVariate._axisActionCommand[2] == 1) ||
                !_deviceWorkTriggerFlag)//等待XY轴运行结束
            {
                if (_deadlyAlarmOccurFlag || !_autoModeFlag)//如果触发了紧急停止
                {
                    return;
                }
                Thread.Sleep(1);
            }

            MyTool.TxtFileProcess.CreateLog("设备动作-设备移动至待机位置完成！" + "----用户：" + _operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
            _axesGotoStandbyModuleActionFlag = 1;
        }

        /// <summary>
        /// ConveyorChangeToAssignedWidthModule:传送线调整至指定宽度模块
        /// </summary>
        /// <param name="controlConveyorFlag">int:控制传送线的标志，0-控制传送线1，1-控制传送线2，2-控制两个传送线</param>
        private async void ConveyorChangeToAssignedWidthModule(int controlConveyorFlag)
        {
            await Task.Run(() =>
            {
                _conveyorChangeToAssignedWidthModuleActionFlag = 0;
                switch (controlConveyorFlag)
                {
                    case 0:
                        MyTool.TxtFileProcess.CreateLog("设备动作-调节传送线1宽度！" + "----用户：" + _operatorName,
                        BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                        break;
                    case 1:
                        MyTool.TxtFileProcess.CreateLog("设备动作-调节传送线2宽度！" + "----用户：" + _operatorName,
                        BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                        break;
                    case 2:
                        MyTool.TxtFileProcess.CreateLog("设备动作-同时调节传送线1，传送线2宽度！" + "----用户：" + _operatorName,
                        BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                        break;
                }

                float y2AxisActionVelocity_mmps, y2AxisActionAcc_s;
                float y3AxisActionVelocity_mmps, y3AxisActionAcc_s;
                float y4AxisActionVelocity_mmps, y4AxisActionAcc_s;
                float y2AxisDestinationPosition_mm = 0, y3AxisDestinationPosition_mm = 0, y4AxisDestinationPosition_mm = 0;

                _axisControlFormVariate.OutputControl(22, true);//关闭Y2Y3Y4的抱闸
                while (!_outputIOT[22].STATUS)//等待抱闸打开
                {
                    if (_deadlyAlarmOccurFlag || _changeDeviceToManualModeT.STATUS || _heavyAlarmOccurFlag)//如果触发了紧急停止或者切换成为手动模式或者轴报警
                    {
                        _conveyorChangeToAssignedWidthModuleActionFlag = -1;
                        return;
                    }
                    Thread.Sleep(1);
                }

                if (_autoModeFlag)//如果为自动模式
                {
                    y2AxisActionVelocity_mmps = _productParameter[1]._y2AxisLocationVelocity;
                    y3AxisActionVelocity_mmps = _productParameter[1]._y3AxisLocationVelocity;
                    y4AxisActionVelocity_mmps = _productParameter[0]._y4AxisLocationVelocity;
                    y2AxisActionAcc_s = _productParameter[1]._y2AxisLocationAccTime;
                    y3AxisActionAcc_s = _productParameter[1]._y3AxisLocationAccTime;
                    y4AxisActionAcc_s = _productParameter[0]._y4AxisLocationAccTime;
                    y2AxisDestinationPosition_mm = Convert.ToSingle(_productParameter[1]._y2ConveyorPosition);
                    y3AxisDestinationPosition_mm = Convert.ToSingle(_productParameter[1]._y3ConveyorPosition);
                    y4AxisDestinationPosition_mm = Convert.ToSingle(_productParameter[0]._y4ConveyorPosition);
                }
                else
                {
                    y2AxisActionVelocity_mmps = SettingsProductForm._editProductParameter._y2AxisLocationVelocity;
                    y3AxisActionVelocity_mmps = SettingsProductForm._editProductParameter._y3AxisLocationVelocity;
                    y4AxisActionVelocity_mmps = SettingsProductForm._editProductParameter._y4AxisLocationVelocity;
                    y2AxisActionAcc_s = SettingsProductForm._editProductParameter._y2AxisLocationAccTime;
                    y3AxisActionAcc_s = SettingsProductForm._editProductParameter._y3AxisLocationAccTime;
                    y4AxisActionAcc_s = SettingsProductForm._editProductParameter._y4AxisLocationAccTime;
                    y2AxisDestinationPosition_mm = Convert.ToSingle(SettingsProductForm._editProductParameter._y2ConveyorPosition);
                    y3AxisDestinationPosition_mm = Convert.ToSingle(SettingsProductForm._editProductParameter._y3ConveyorPosition);
                    y4AxisDestinationPosition_mm = Convert.ToSingle(SettingsProductForm._editProductParameter._y4ConveyorPosition);
                }

                if (!SettingsProductForm._useRelativePos)//如果使用相对位置
                {
                    y2AxisDestinationPosition_mm += SettingsAdministratorForm._absoluteCoor[3];//添加上相对位置Y2坐标
                    y2AxisDestinationPosition_mm += SettingsAdministratorForm._absoluteCoor[4];//添加上相对位置Y3坐标
                    y2AxisDestinationPosition_mm += SettingsAdministratorForm._absoluteCoor[5];//添加上相对位置Y4坐标
                }

                while ((_axisControlFormVariate._axisActionCommand[3] != -1 && (controlConveyorFlag == 0 || controlConveyorFlag == 2)) ||
                      ((_axisControlFormVariate._axisActionCommand[4] != -1 || _axisControlFormVariate._axisActionCommand[5] != -1) &&
                      (controlConveyorFlag == 1 || controlConveyorFlag == 2)) || !_deviceWorkTriggerFlag)//当Y2,Y3,Y4轴处于运动指令状态,循环等待
                {
                    if (_deadlyAlarmOccurFlag || _changeDeviceToManualModeT.STATUS || _heavyAlarmOccurFlag)//如果触发了紧急停止或者切换成为手动模式或者轴报警
                    {
                        _conveyorChangeToAssignedWidthModuleActionFlag = -1;
                        return;
                    }
                    Thread.Sleep(1);
                }

                //控制传送线宽度调节电机移动
                if (controlConveyorFlag == 0 || controlConveyorFlag == 2)
                {
                    //Y4
                    _axisControlFormVariate._axisCurrentPositionNumber[5] = 0;
                    _axisControlFormVariate._assignedPositionActionTargetPosition_mm[5] = y4AxisDestinationPosition_mm;
                    _axisControlFormVariate._assignedPositionActionMaxVelocity_mmps[5] = y4AxisActionVelocity_mmps;
                    _axisControlFormVariate._assignedPositionActionAccTime_s[5] = y4AxisActionAcc_s;
                    _axisControlFormVariate._assignedPositionActionDeviation[5] = 0f;
                    _axisControlFormVariate._assignedPositionActionSMoveFlag[5] = false;
                    _axisControlFormVariate._assignedPositionActionSMoveTime[5] = 0.01f;
                    _axisControlFormVariate._axisActionCommand[5] = 1000;//指定点位移动
                }

                if (controlConveyorFlag == 1 || controlConveyorFlag == 2)
                {
                    //Y2
                    _axisControlFormVariate._axisCurrentPositionNumber[3] = 0;
                    _axisControlFormVariate._assignedPositionActionTargetPosition_mm[3] = y2AxisDestinationPosition_mm;
                    _axisControlFormVariate._assignedPositionActionMaxVelocity_mmps[3] = y2AxisActionVelocity_mmps;
                    _axisControlFormVariate._assignedPositionActionAccTime_s[3] = y2AxisActionAcc_s;
                    _axisControlFormVariate._assignedPositionActionDeviation[3] = 0f;
                    _axisControlFormVariate._assignedPositionActionSMoveFlag[3] = false;
                    _axisControlFormVariate._assignedPositionActionSMoveTime[3] = 0.01f;
                    _axisControlFormVariate._axisActionCommand[3] = 1000;//指定点位移动

                    //Y3
                    _axisControlFormVariate._axisCurrentPositionNumber[4] = 0;
                    _axisControlFormVariate._assignedPositionActionTargetPosition_mm[4] = y3AxisDestinationPosition_mm;
                    _axisControlFormVariate._assignedPositionActionMaxVelocity_mmps[4] = y3AxisActionVelocity_mmps;
                    _axisControlFormVariate._assignedPositionActionAccTime_s[4] = y3AxisActionAcc_s;
                    _axisControlFormVariate._assignedPositionActionDeviation[4] = 0f;
                    _axisControlFormVariate._assignedPositionActionSMoveFlag[4] = false;
                    _axisControlFormVariate._assignedPositionActionSMoveTime[4] = 0.01f;
                    _axisControlFormVariate._axisActionCommand[4] = 1000;//指定点位移动


                }

                while (((_axisControlFormVariate._axisCurrentPositionNumber[5] == 0 || _axisControlFormVariate._axisActionCommand[5] == 1000) && (controlConveyorFlag == 0 || controlConveyorFlag == 2)) ||
                    ((_axisControlFormVariate._axisCurrentPositionNumber[3] == 0 || _axisControlFormVariate._axisActionCommand[3] == 1000 ||
                    _axisControlFormVariate._axisCurrentPositionNumber[4] == 0 || _axisControlFormVariate._axisActionCommand[4] == 1000) && (controlConveyorFlag == 1 || controlConveyorFlag == 2))
                    || !_deviceWorkTriggerFlag)//等待Y2Y3Y4轴运行结束
                {
                    if (_deadlyAlarmOccurFlag || _changeDeviceToManualModeT.STATUS || _heavyAlarmOccurFlag)//如果触发了紧急停止或者切换成为手动模式或者轴报警
                    {
                        _conveyorChangeToAssignedWidthModuleActionFlag = -1;
                        return;
                    }
                    Thread.Sleep(1);
                }

                _axisControlFormVariate.OutputControl(22, false);//关闭Y2Y3Y4的抱闸
                while (_outputIOT[22].STATUS)
                {
                    if (_deadlyAlarmOccurFlag || _changeDeviceToManualModeT.STATUS || _heavyAlarmOccurFlag)//如果触发了紧急停止或者切换成为手动模式或者轴报警
                    {
                        _conveyorChangeToAssignedWidthModuleActionFlag = -1;
                        return;
                    }
                    Thread.Sleep(1);
                }

                switch (controlConveyorFlag)
                {
                    case 0:
                        MyTool.TxtFileProcess.CreateLog("设备动作-调节传送线1宽度完成！" + "----用户：" + _operatorName,
                        BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                        break;
                    case 1:
                        MyTool.TxtFileProcess.CreateLog("设备动作-调节传送线2宽度完成！" + "----用户：" + _operatorName,
                        BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                        break;
                    case 2:
                        MyTool.TxtFileProcess.CreateLog("设备动作-同时调节传送线1，传送线2宽度完成！" + "----用户：" + _operatorName,
                        BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                        break;
                }
                _conveyorChangeToAssignedWidthModuleActionFlag = 1;
            });
        }

        /// <summary>
        /// Conveyor1ActionModule:传送线1动作模块
        /// </summary>
        /// <param name="moveSpeed">float：传送线1动作速度，小于0-停止，其余-运行速度</param>
        private void Conveyor1ActionModule(float moveSpeed)
        {
            _conveyor1ActionModuleActionFlag = 0;

            if (moveSpeed <= 0)
            {
                MyTool.TxtFileProcess.CreateLog("设备动作-控制传送线1停止" + "----用户：" + _operatorName,
                BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

                lock (this)
                {
                    _axisControlFormVariate.OutputControl(71, false);
                }
                while (_axisControlFormVariate._ioStatus[1, 71])
                {
                    if (_deadlyAlarmOccurFlag)
                    {
                        return;
                    }
                    Thread.Sleep(1);
                }
                if (!_newDAM02AIAOOperation.comm.IsOpen)
                {
                    if (!_newDAM02AIAOOperation.OpenSerialPort(SettingsGeneralForm._conveyorControlCom))
                    {
                        MyTool.TxtFileProcess.CreateLog("设备动作-控制传送线1停止失败，打开控制串口失败！" + "----用户：" + _operatorName,
                        BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

                        MessageBox.Show("打开串口失败");
                    }
                }
                if (_newDAM02AIAOOperation.comm.IsOpen)
                {
                    lock (this)
                    {
                        _newDAM02AIAOOperation.analogOutput(1, 0);
                    }
                }

                MyTool.TxtFileProcess.CreateLog("设备动作-控制传送线1停止成功！" + "----用户：" + _operatorName,
                BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                _conveyor1ActionModuleActionFlag = 2;
            }
            else
            {
                MyTool.TxtFileProcess.CreateLog("设备动作-控制传送线1以速度" + moveSpeed.ToString() + "动作" + "----用户：" + _operatorName,
                BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

                if (!_newDAM02AIAOOperation.comm.IsOpen)
                {
                    if (!_newDAM02AIAOOperation.OpenSerialPort(SettingsGeneralForm._conveyorControlCom))
                    {
                        MyTool.TxtFileProcess.CreateLog("设备动作-控制传送线1以速度" + moveSpeed.ToString() + "动作失败，打开串口失败！" + "----用户：" + _operatorName,
                        BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

                        MessageBox.Show("打开串口失败");
                    }
                }
                if (_newDAM02AIAOOperation.comm.IsOpen)
                {
                    int outputVoltage = (int)(moveSpeed / 508 * 10);
                    if (outputVoltage > 10)
                    {
                        outputVoltage = 10;
                    }
                    else if (outputVoltage < 0)
                    {
                        outputVoltage = 0;
                    }
                    lock (this)
                    {
                        _newDAM02AIAOOperation.analogOutput(1, outputVoltage);
                    }
                }

                lock (this)
                {
                    _axisControlFormVariate.OutputControl(71, true);
                }

                while (!_axisControlFormVariate._ioStatus[1, 71])
                {
                    if (_deadlyAlarmOccurFlag)
                    {
                        return;
                    }
                    Thread.Sleep(1);
                }

                MyTool.TxtFileProcess.CreateLog("设备动作-控制传送线1以速度" + moveSpeed.ToString() + "动作成功！" + "----用户：" + _operatorName,
                BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                _conveyor1ActionModuleActionFlag = 1;
            }
        }

        /// <summary>
        /// Conveyor2ActionModule:传送线1动作模块
        /// </summary>
        /// <param name="moveSpeed">float：传送线1动作速度，小于0-停止，其余-运行速度</param>
        private void Conveyor2ActionModule(float moveSpeed)
        {
            _conveyor2ActionModuleActionFlag = 0;

            if (moveSpeed <= 0)
            {
                MyTool.TxtFileProcess.CreateLog("设备动作-控制传送线2停止" + "----用户：" + _operatorName,
                BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

                lock (this)
                {
                    _axisControlFormVariate.OutputControl(69, false);
                }
                while (_axisControlFormVariate._ioStatus[1, 69])
                {
                    if (_deadlyAlarmOccurFlag)
                    {
                        return;
                    }
                    Thread.Sleep(1);
                }
                if (!_newDAM02AIAOOperation.comm.IsOpen)
                {
                    if (!_newDAM02AIAOOperation.OpenSerialPort(SettingsGeneralForm._conveyorControlCom))
                    {
                        MyTool.TxtFileProcess.CreateLog("设备动作-控制传送线2停止失败，打开控制串口失败！" + "----用户：" + _operatorName,
                        BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

                        MessageBox.Show("打开串口失败");
                    }
                }
                if (_newDAM02AIAOOperation.comm.IsOpen)
                {
                    lock (this)
                    {
                        _newDAM02AIAOOperation.analogOutput(0, 0);
                    }
                }

                MyTool.TxtFileProcess.CreateLog("设备动作-控制传送线2停止成功！" + "----用户：" + _operatorName,
                BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                _conveyor2ActionModuleActionFlag = 2;
            }
            else
            {
                MyTool.TxtFileProcess.CreateLog("设备动作-控制传送线2以速度" + moveSpeed.ToString() + "动作" + "----用户：" + _operatorName,
                BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

                if (!_newDAM02AIAOOperation.comm.IsOpen)
                {
                    if (!_newDAM02AIAOOperation.OpenSerialPort(SettingsGeneralForm._conveyorControlCom))
                    {
                        MyTool.TxtFileProcess.CreateLog("设备动作-控制传送线2以速度" + moveSpeed.ToString() + "动作失败，打开串口失败！" + "----用户：" + _operatorName,
                        BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

                        MessageBox.Show("打开串口失败");
                    }
                }

                // 输出模拟量
                if (_newDAM02AIAOOperation.comm.IsOpen)
                {
                    int outputVoltage = (int)(moveSpeed / 508 * 10);
                    if (outputVoltage > 10)
                    {
                        outputVoltage = 10;
                    }
                    else if (outputVoltage < 0)
                    {
                        outputVoltage = 0;
                    }
                    lock (this)
                    {
                        _newDAM02AIAOOperation.analogOutput(0, outputVoltage);
                    }
                }

                lock (this)
                {
                    _axisControlFormVariate.OutputControl(69, true);
                }
                while (!_axisControlFormVariate._ioStatus[1, 69])
                {
                    if (_deadlyAlarmOccurFlag)
                    {
                        return;
                    }
                    Thread.Sleep(1);
                }

                MyTool.TxtFileProcess.CreateLog("设备动作-控制传送线2以速度" + moveSpeed.ToString() + "动作成功！" + "----用户：" + _operatorName,
                BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                _conveyor2ActionModuleActionFlag = 1;
            }
        }

        /// <summary>
        /// Conveyor1LoadUnloadModule:传送线1上下料模块
        /// </summary>
        private async void Conveyor1LoadUnloadModule()
        {
            await Task.Run(() =>
            {
                _conveyor1LoadUnloadModuleActionFlag = 0;
                if (_conveyor1Status == 0)//如果为需要上料
                {
                    //为了防止同一时间有多个调用Outputcontrol指令
                    lock (this)
                    {
                        if (!_conveyor1HaveProductFlag)
                        {
                            while (!_axisControlFormVariate._ioStatus[1, 73])//如果设备发出允许上游出板信号
                            {
                                if (_deadlyAlarmOccurFlag || !_autoModeFlag)
                                {
                                    return;
                                }
                                _axisControlFormVariate.OutputControl(73, true);
                            }
                        }
                        else
                        {
                            while (_axisControlFormVariate._ioStatus[1, 73])//如果设备有发出允许上游出板信号
                            {
                                if (_deadlyAlarmOccurFlag || !_autoModeFlag)
                                {
                                    return;
                                }
                                _axisControlFormVariate.OutputControl(73, false);
                            }
                        }

                        while (_axisControlFormVariate._ioStatus[1, 75])//如果设备发出过本机出板请求信号
                        {
                            if (_deadlyAlarmOccurFlag || !_autoModeFlag)
                            {
                                return;
                            }
                            _axisControlFormVariate.OutputControl(75, false);
                        }

                    }

                    if (_inputIOT[92].STATUS || _conveyor1HaveProductFlag)//如果入口传感器ON或者已经记录在下料时有产品进入传送线
                    {
                        //计时
                        if (_conveyor1CheckTimeCountSW.IsRunning)
                            _conveyor1CheckTimeCountSW.Reset();
                        else
                            _conveyor1CheckTimeCountSW.Restart();

                        MyTool.TxtFileProcess.CreateLog("设备动作-传送线1上料动作！" + "----用户：" + _operatorName,
                        BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);


                        //清除允许上游出板信号
                        while (_axisControlFormVariate._ioStatus[1, 73])//如果设备有发出允许上游出板信号
                        {
                            if (_deadlyAlarmOccurFlag || !_autoModeFlag)
                            {
                                return;
                            }
                            _axisControlFormVariate.OutputControl(73, false);
                        }

                        lock (this)
                        {
                            //清除timeout deal with way index
                            _conveyor1TimeoutAlarmDealWithWayIndexWhenLoadAndUnload = -1;

                            //控制阻挡气缸上
                            _axisControlFormVariate.OutputControl(80, false);
                            _axisControlFormVariate.OutputControl(81, true);
                        }

                        while (!_inputIOT[85].STATUS || _stateAndAlarmCs._alarmFlag[400] || _conveyor1HaveExcessProductAlarmDealWithWayIndex == 1)// || !_deviceWorkTriggerFlag)//等待阻挡气缸上
                        {
                            if (_deadlyAlarmOccurFlag || !_autoModeFlag ||
                                _conveyor1HaveExcessProductAlarmDealWithWayIndex == 1)
                            {
                                _conveyor1LoadUnloadModuleActionFlag = -1;
                                return;
                            }

                            Thread.Sleep(1);
                        }

                        lock (this)
                        {
                            //控制夹紧气缸松开
                            _axisControlFormVariate.OutputControl(82, true);
                            _axisControlFormVariate.OutputControl(83, false);
                        }

                        while (!_inputIOT[86].STATUS )// || !_deviceWorkTriggerFlag)//等待夹紧气缸松开
                        {
                            if (_deadlyAlarmOccurFlag || !_autoModeFlag )
                            {
                                _conveyor1LoadUnloadModuleActionFlag = -1;
                                return;
                            }

                            Thread.Sleep(1);
                        }

                        while (_conveyor1ActionModuleActionFlag == 0 || _stateAndAlarmCs._alarmFlag[340] || 
                            _conveyor1TimeoutAlarmDealWithWayIndexWhenLoadAndUnload != -1 || _stateAndAlarmCs._alarmFlag[400] || 
                            _conveyor1HaveExcessProductAlarmDealWithWayIndex == 1)
                        {
                            if (_deadlyAlarmOccurFlag || !_autoModeFlag ||
                                _conveyor1HaveExcessProductAlarmDealWithWayIndex == 1)
                            {
                                _conveyor1LoadUnloadModuleActionFlag = -1;
                                return;
                            }

                            if (_conveyor1TimeoutAlarmDealWithWayIndexWhenLoadAndUnload == 1)
                            {
                                _conveyor1TimeoutAlarmDealWithWayIndexWhenLoadAndUnload = -1;
                                break;
                            }

                            switch (_conveyor1TimeoutAlarmDealWithWayIndexWhenLoadAndUnload)
                            {
                                case 0://如果为重试，不进行任何动作
                                    _conveyor1TimeoutAlarmDealWithWayIndexWhenLoadAndUnload = -1;
                                    break;
                                //case 1://如果为忽略
                                //    _conveyor1TimeoutAlarmDealWithWayIndexWhenLoadAndUnload = -1;
                                //    continue;
                                case 2://如果为无物料
                                    _conveyor1Status = 0;
                                    _conveyor1LoadUnloadModuleActionFlag = 1;
                                    _conveyor1TimeoutAlarmDealWithWayIndexWhenLoadAndUnload = -1;
                                    return;
                            }

                            Thread.Sleep(1);
                        }

                        _conveyor1ActionModuleActionFlag = 0;
                        Conveyor1ActionModule(_productParameter[0]._conveyor1InHighSpeed);

                        while (_conveyor1ActionModuleActionFlag != 1)//如果传送线未完成速度设定
                        {
                            if (_deadlyAlarmOccurFlag || !_autoModeFlag)
                            {
                                return;
                            }
                            Thread.Sleep(1);
                        }

                        while (_inputIOT[92].STATUS || _conveyor1ActionModuleActionFlag == 0 || _stateAndAlarmCs._alarmFlag[340] || 
                            _conveyor1TimeoutAlarmDealWithWayIndexWhenLoadAndUnload != -1 || _stateAndAlarmCs._alarmFlag[400] ||
                            _conveyor1HaveExcessProductAlarmDealWithWayIndex == 1)//等待入口传感器信号灭掉
                        {
                            if (_deadlyAlarmOccurFlag || !_autoModeFlag ||
                                _conveyor1HaveExcessProductAlarmDealWithWayIndex == 1)
                            {
                                _conveyor1LoadUnloadModuleActionFlag = -1;
                                return;
                            }

                            if (_conveyor1TimeoutAlarmDealWithWayIndexWhenLoadAndUnload == 1)
                            {
                                _conveyor1TimeoutAlarmDealWithWayIndexWhenLoadAndUnload = -1;
                                break;
                            }

                            switch (_conveyor1TimeoutAlarmDealWithWayIndexWhenLoadAndUnload)
                            {
                                case 0://如果为重试，不进行任何动作
                                    _conveyor1TimeoutAlarmDealWithWayIndexWhenLoadAndUnload = -1;
                                    break;
                                //case 1://如果为忽略
                                //    _conveyor1TimeoutAlarmDealWithWayIndexWhenLoadAndUnload = -1;
                                //    continue;
                                case 2://如果为无物料
                                    while ( _conveyor1ActionModuleActionFlag == 0)//等待控制传送线动作模块空闲 !_deviceWorkTriggerFlag ||
                                    {
                                        if (_deadlyAlarmOccurFlag || !_autoModeFlag)
                                        {
                                            return;
                                        }
                                        Thread.Sleep(1);
                                    }
                                    _conveyor1ActionModuleActionFlag = 0;
                                    Conveyor1ActionModule(-1);//控制传送线停止
                                    while (_conveyor1ActionModuleActionFlag != 2)//如果传送线未完成速度设定
                                    {
                                        if (_deadlyAlarmOccurFlag || !_autoModeFlag)
                                        {
                                            return;
                                        }
                                        Thread.Sleep(1);
                                    }
                                    _conveyor1Status = 0;
                                    _conveyor1LoadUnloadModuleActionFlag = 1;
                                    _conveyor1TimeoutAlarmDealWithWayIndexWhenLoadAndUnload = -1;
                                    return;
                            }

                            Thread.Sleep(1);
                        }

                        while (!_inputIOT[89].STATUS || _conveyor1ActionModuleActionFlag == 0 || _stateAndAlarmCs._alarmFlag[340] ||
                            _conveyor1TimeoutAlarmDealWithWayIndexWhenLoadAndUnload != -1 || _stateAndAlarmCs._alarmFlag[400]
                             || _conveyor1HaveExcessProductAlarmDealWithWayIndex == 1)//如果传送线减速信号没有被触发，或者传送线1动作控制模块不为off，等待
                        {
                            if (_deadlyAlarmOccurFlag || !_autoModeFlag ||
                                _conveyor1HaveExcessProductAlarmDealWithWayIndex == 1)
                            {
                                _conveyor1LoadUnloadModuleActionFlag = -1;
                                return;
                            }
                            if (_conveyor1TimeoutAlarmDealWithWayIndexWhenLoadAndUnload == 1)
                            {
                                _conveyor1TimeoutAlarmDealWithWayIndexWhenLoadAndUnload = -1;
                                break;
                            }
                            switch (_conveyor1TimeoutAlarmDealWithWayIndexWhenLoadAndUnload)
                            {
                                case 0://如果为重试，不进行任何动作
                                    _conveyor1TimeoutAlarmDealWithWayIndexWhenLoadAndUnload = -1;
                                    break;
                                //case 1://如果为忽略
                                    //_conveyor1TimeoutAlarmDealWithWayIndexWhenLoadAndUnload = -1;
                                    //continue;
                                case 2://如果为无物料
                                    while (_conveyor1ActionModuleActionFlag == 0)//等待控制传送线动作模块空闲 !_deviceWorkTriggerFlag || 
                                    {
                                        if (_deadlyAlarmOccurFlag || !_autoModeFlag)
                                        {
                                            return;
                                        }
                                        Thread.Sleep(1);
                                    }
                                    _conveyor1ActionModuleActionFlag = 0;
                                    Conveyor1ActionModule(-1);//控制传送线停止
                                    while (_conveyor1ActionModuleActionFlag != 2)//如果传送线未完成速度设定
                                    {
                                        if (_deadlyAlarmOccurFlag || !_autoModeFlag)
                                        {
                                            return;
                                        }
                                        Thread.Sleep(1);
                                    }
                                    _conveyor1Status = 0;
                                    _conveyor1LoadUnloadModuleActionFlag = 1;
                                    _conveyor1TimeoutAlarmDealWithWayIndexWhenLoadAndUnload = -1;
                                    return;
                            }

                            Thread.Sleep(1);
                        }

                        _conveyor1ActionModuleActionFlag = 0;
                        Conveyor1ActionModule(_productParameter[0]._conveyor1InLowSpeed);//切换低速

                        while (_conveyor1ActionModuleActionFlag != 1)//如果传送线未完成速度设定
                        {
                            if (_deadlyAlarmOccurFlag || !_autoModeFlag)
                            {
                                return;
                            }
                            Thread.Sleep(1);
                        }

                        while (!_conveyor1FinishLoadT.STATUS || _conveyor1ActionModuleActionFlag == 0 || _stateAndAlarmCs._alarmFlag[340] || 
                            _conveyor1TimeoutAlarmDealWithWayIndexWhenLoadAndUnload != -1 || _stateAndAlarmCs._alarmFlag[400]
                             || _conveyor1HaveExcessProductAlarmDealWithWayIndex == 1)//如果上料信号未被触发至完成，或传送线1动作控制模块不为OFF，等待
                        {
                            if (_deadlyAlarmOccurFlag || !_autoModeFlag ||
                                _conveyor1HaveExcessProductAlarmDealWithWayIndex == 1)
                            {                     
                                _conveyor1LoadUnloadModuleActionFlag = -1;
                                return;
                            }

                            if (_conveyor1TimeoutAlarmDealWithWayIndexWhenLoadAndUnload == 1)
                            {
                                _conveyor1TimeoutAlarmDealWithWayIndexWhenLoadAndUnload = -1;
                                break;
                            }
                            switch (_conveyor1TimeoutAlarmDealWithWayIndexWhenLoadAndUnload)
                            {
                                case 0://如果为重试，不进行任何动作
                                    _conveyor1TimeoutAlarmDealWithWayIndexWhenLoadAndUnload = -1;
                                    break;
                                //case 1://如果为忽略
                                    //_conveyor1TimeoutAlarmDealWithWayIndexWhenLoadAndUnload = -1;
                                    //continue;
                                case 2://如果为无物料
                                    while (_conveyor1ActionModuleActionFlag == 0)//等待控制传送线动作模块空闲 !_deviceWorkTriggerFlag || 
                                    {
                                        if (_deadlyAlarmOccurFlag || !_autoModeFlag)
                                        {
                                            return;
                                        }
                                        Thread.Sleep(1);
                                    }
                                    _conveyor1ActionModuleActionFlag = 0;
                                    Conveyor1ActionModule(-1);//控制传送线停止
                                    while (_conveyor1ActionModuleActionFlag != 2)//如果传送线未完成速度设定
                                    {
                                        if (_deadlyAlarmOccurFlag || !_autoModeFlag)
                                        {
                                            return;
                                        }
                                        Thread.Sleep(1);
                                    }
                                    _conveyor1Status = 0;
                                    _conveyor1LoadUnloadModuleActionFlag = 1;
                                    _conveyor1TimeoutAlarmDealWithWayIndexWhenLoadAndUnload = -1;
                                    return;
                            }

                            Thread.Sleep(1);
                        }

                        _conveyor1ActionModuleActionFlag = 0;
                        Conveyor1ActionModule(-1);//传送线停止

                        while (_conveyor1ActionModuleActionFlag != 2)//如果传送线未完成停止
                        {
                            if (_deadlyAlarmOccurFlag || !_autoModeFlag)
                            {
                                return;
                            }
                            Thread.Sleep(1);
                        }

                        lock (this)
                        {
                            //控制夹紧气缸夹紧
                            _axisControlFormVariate.OutputControl(82, false);
                            _axisControlFormVariate.OutputControl(83, true);
                        }

                        while (!_inputIOT[87].STATUS || _stateAndAlarmCs._alarmFlag[400] || _conveyor1HaveExcessProductAlarmDealWithWayIndex == 1)// || !_deviceWorkTriggerFlag)//等待夹紧气缸夹紧
                        {
                            if (_deadlyAlarmOccurFlag || !_autoModeFlag
                                || _conveyor1HaveExcessProductAlarmDealWithWayIndex == 1)
                            {
                                _conveyor1LoadUnloadModuleActionFlag = -1;
                                return;
                            }
                            Thread.Sleep(1);
                        }

                        MyTool.TxtFileProcess.CreateLog("设备动作-传送线1上料完成！" + "----用户：" + _operatorName,
                        BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

                        if (_conveyor1HaveExcessProductAlarmDealWithWayIndex != 1)
                        {
                            _conveyor1Status = 1;
                        }
                        _conveyor1LoadUnloadModuleActionFlag = 1;

                    }
                    else
                        _conveyor1LoadUnloadModuleActionFlag = 1;
                }
                else if (_conveyor1Status == 1)//如果为需要检测
                {
                    lock (this)
                    {
                        if (_axisControlFormVariate._ioStatus[1, 73])//如果设备发出允许上游出板信号
                            _axisControlFormVariate.OutputControl(73, false);

                        if (_axisControlFormVariate._ioStatus[1, 75])//如果设备发出本机出板请求信号
                            _axisControlFormVariate.OutputControl(75, false);
                    }
                    _conveyor1LoadUnloadModuleActionFlag = 1;
                }
                else if (_conveyor1Status == 2)//如果为需要下料
                {

                    while (_axisControlFormVariate._ioStatus[1, 73])//如果设备发出允许上游出板信号
                    {
                        if (_deadlyAlarmOccurFlag || !_autoModeFlag)
                        {
                            return;
                        }
                        _axisControlFormVariate.OutputControl(73, false);
                    }
                    while (!_axisControlFormVariate._ioStatus[1, 75])//如果设备没有发出过本机出板请求信号
                    {
                        if (_deadlyAlarmOccurFlag || !_autoModeFlag)
                        {
                            return;
                        }
                        _axisControlFormVariate.OutputControl(75, true);
                    }

                    if (_inputIOT[71].STATUS)//如果接收到下游允许出板请求信号
                    {
                        MyTool.TxtFileProcess.CreateLog("设备动作-传送线1下料动作！" + "----用户：" + _operatorName,
                        BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

                        //清除timeout deal with way index
                        _conveyor1TimeoutAlarmDealWithWayIndexWhenLoadAndUnload = -1;

                        _conveyor1HaveProductFlag = false;

                        //如果传送线1有多余产品处理方式为1（流出），等到正式开始流出时置为-1，等待报警数据被关闭
                        while (_conveyor1HaveExcessProductAlarmDealWithWayIndex == 1 && _stateAndAlarmCs._alarmFlag[400])
                        {
                            if (_deadlyAlarmOccurFlag || !_autoModeFlag)
                            {
                                return;
                            }

                            Thread.Sleep(1);
                        }
                        if (_conveyor1HaveExcessProductAlarmDealWithWayIndex == 1)
                            _conveyor1HaveExcessProductAlarmDealWithWayIndex = -1;

                        lock (this)
                        {
                            //控制夹紧气缸松开
                            _axisControlFormVariate.OutputControl(82, true);
                            _axisControlFormVariate.OutputControl(83, false);
                        }

                        while (!_inputIOT[86].STATUS)// || !_deviceWorkTriggerFlag)//等待夹紧气缸松开
                        {
                            if (_deadlyAlarmOccurFlag || !_autoModeFlag)
                            {
                                return;
                            }

                            Thread.Sleep(1);
                        }

                        lock (this)
                        {
                            //控制阻挡气缸下
                            _axisControlFormVariate.OutputControl(80, true);
                            _axisControlFormVariate.OutputControl(81, false);
                        }

                        while (!_inputIOT[84].STATUS)// || !_deviceWorkTriggerFlag)//等待阻挡气缸下
                        {
                            if (_deadlyAlarmOccurFlag || !_autoModeFlag)
                            {
                                return;
                            }
                            Thread.Sleep(1);
                        }

                        while (_conveyor1ActionModuleActionFlag == 0 || _stateAndAlarmCs._alarmFlag[341] || _conveyor1TimeoutAlarmDealWithWayIndexWhenLoadAndUnload != -1)//!_deviceWorkTriggerFlag ||
                        {
                            if (_deadlyAlarmOccurFlag || !_autoModeFlag)
                            {
                                return;
                            }
                            if (_conveyor1TimeoutAlarmDealWithWayIndexWhenLoadAndUnload == 1)
                            {
                                _conveyor1TimeoutAlarmDealWithWayIndexWhenLoadAndUnload = -1;
                                break;
                            }
                            switch (_conveyor1TimeoutAlarmDealWithWayIndexWhenLoadAndUnload)
                            {
                                case 0://如果为重试，不进行任何动作
                                    _conveyor1TimeoutAlarmDealWithWayIndexWhenLoadAndUnload = -1;
                                    break;
                                //case 1://如果为忽略
                                   // _conveyor1TimeoutAlarmDealWithWayIndexWhenLoadAndUnload = -1;
                                    //continue;
                                case 2://如果为无物料
                                    _conveyor1Status = 0;
                                    _conveyor1LoadUnloadModuleActionFlag = 1;
                                    _conveyor1TimeoutAlarmDealWithWayIndexWhenLoadAndUnload = -1;
                                    return;
                            }

                            Thread.Sleep(1);
                        }

                        _conveyor1ActionModuleActionFlag = 0;
                        Conveyor1ActionModule(_productParameter[0]._conveyor1OutSpeed);

                        while (_conveyor1ActionModuleActionFlag != 1)//如果传送线未完成速度设定
                        {
                            if (_deadlyAlarmOccurFlag || !_autoModeFlag)
                            {
                                return;
                            }
                            Thread.Sleep(1);
                        }

                        while (!_inputIOT[93].STATUS || _stateAndAlarmCs._alarmFlag[341] || _conveyor1TimeoutAlarmDealWithWayIndexWhenLoadAndUnload != -1)//等待出口传感器触发
                        {
                            if (_deadlyAlarmOccurFlag || !_autoModeFlag)
                            {
                                return;
                            }

                            if (_conveyor1UnloadStopCylinderUpT.STATUS && !_inputIOT[85].STATUS)//如果下料阻挡气缸应当上升
                            {
                                lock (this)
                                {
                                    _axisControlFormVariate.OutputControl(80, false);
                                    _axisControlFormVariate.OutputControl(81, true);
                                }
                            }

                            if (_inputIOT[92].STATUS && !_conveyor1HaveProductFlag)//如果下料时入口传感器被触发
                            {
                                _conveyor1HaveProductFlag = true;//设置标志为true，防止由于下料时，上料传感器被触发，后面不在触发导致的死机
                            }
                            if (_conveyor1TimeoutAlarmDealWithWayIndexWhenLoadAndUnload == 1)
                            {
                                _conveyor1TimeoutAlarmDealWithWayIndexWhenLoadAndUnload = -1;
                                break;
                            }
                            switch (_conveyor1TimeoutAlarmDealWithWayIndexWhenLoadAndUnload)
                            {
                                case 0://如果为重试，不进行任何动作
                                    _conveyor1TimeoutAlarmDealWithWayIndexWhenLoadAndUnload = -1;
                                    break;
                                //case 1://如果为忽略
                                    //_conveyor1TimeoutAlarmDealWithWayIndexWhenLoadAndUnload = -1;
                                    //continue;
                                case 2://如果为无物料
                                    while (_conveyor1ActionModuleActionFlag == 0)//等待控制传送线动作模块空闲 !_deviceWorkTriggerFlag || 
                                    {
                                        if (_deadlyAlarmOccurFlag || !_autoModeFlag)
                                        {
                                            return;
                                        }
                                        Thread.Sleep(1);
                                    }

                                    _conveyor1ActionModuleActionFlag = 0;
                                    Conveyor1ActionModule(-1);//控制传送线停止

                                    while (_conveyor1ActionModuleActionFlag != 2)//如果传送线未完成速度设定
                                    {
                                        if (_deadlyAlarmOccurFlag || !_autoModeFlag)
                                        {
                                            return;
                                        }
                                        Thread.Sleep(1);
                                    }
                                    _conveyor1Status = 0;
                                    _conveyor1LoadUnloadModuleActionFlag = 1;
                                    _conveyor1TimeoutAlarmDealWithWayIndexWhenLoadAndUnload = -1;
                                    return;
                            }

                            Thread.Sleep(1);
                        }

                        while (!_conveyor1FinishUnloadT.STATUS || _stateAndAlarmCs._alarmFlag[341] || _conveyor1TimeoutAlarmDealWithWayIndexWhenLoadAndUnload != -1)//如果传送线未触发下料完成
                        {
                            if (_deadlyAlarmOccurFlag || !_autoModeFlag)
                            {
                                return;
                            }

                            if (_conveyor1UnloadStopCylinderUpT.STATUS && !_inputIOT[85].STATUS)//如果下料阻挡气缸应当上升
                            {
                                lock (this)
                                {
                                    _axisControlFormVariate.OutputControl(80, false);
                                    _axisControlFormVariate.OutputControl(81, true);
                                }
                            }

                            if (_inputIOT[92].STATUS && !_conveyor1HaveProductFlag)//如果下料时入口传感器被触发
                            {
                                _conveyor1HaveProductFlag = true;//设置标志为true，防止由于下料时，上料传感器被触发，后面不在触发导致的死机
                            }
                            if (_conveyor1TimeoutAlarmDealWithWayIndexWhenLoadAndUnload == 1)
                            {
                                _conveyor1TimeoutAlarmDealWithWayIndexWhenLoadAndUnload = -1;
                                break;
                            }
                            switch (_conveyor1TimeoutAlarmDealWithWayIndexWhenLoadAndUnload)
                            {
                                case 0://如果为重试，不进行任何动作
                                    _conveyor1TimeoutAlarmDealWithWayIndexWhenLoadAndUnload = -1;
                                    break;
                                //case 1://如果为忽略
                                   // _conveyor1TimeoutAlarmDealWithWayIndexWhenLoadAndUnload = -1;
                                    //continue;
                                case 2://如果为无物料
                                    while (_conveyor1ActionModuleActionFlag == 0)//等待控制传送线动作模块空闲 !_deviceWorkTriggerFlag || 
                                    {
                                        if (_deadlyAlarmOccurFlag || !_autoModeFlag)
                                        {
                                            return;
                                        }
                                        Thread.Sleep(1);
                                    }
                                    _conveyor1ActionModuleActionFlag = 0;
                                    Conveyor1ActionModule(-1);//控制传送线停止
                                    while (_conveyor1ActionModuleActionFlag != 2)//如果传送线未完成速度设定
                                    {
                                        if (_deadlyAlarmOccurFlag || !_autoModeFlag)
                                        {
                                            return;
                                        }
                                        Thread.Sleep(1);
                                    }
                                    _conveyor1Status = 0;
                                    _conveyor1LoadUnloadModuleActionFlag = 1;
                                    _conveyor1TimeoutAlarmDealWithWayIndexWhenLoadAndUnload = -1;
                                    return;
                            }

                            Thread.Sleep(1);
                        }

                        while (_conveyor1ActionModuleActionFlag == 0)
                        {
                            if (_deadlyAlarmOccurFlag || !_autoModeFlag)
                            {
                                return;
                            }
                            Thread.Sleep(1);
                        }

                        //控制传送线停止
                        _conveyor1ActionModuleActionFlag = 0;
                        Conveyor1ActionModule(-1);

                        while (_conveyor1ActionModuleActionFlag != 2)//如果传送线未完成停止
                        {
                            if (_deadlyAlarmOccurFlag || !_autoModeFlag)
                            {
                                return;
                            }

                            if (_inputIOT[92].STATUS && !_conveyor1HaveProductFlag)//如果下料时入口传感器被触发
                            {
                                _conveyor1HaveProductFlag = true;//设置标志为true，防止由于下料时，上料传感器被触发，后面不在触发导致的死机
                            }
                            Thread.Sleep(1);
                        }

                        MyTool.TxtFileProcess.CreateLog("设备动作-传送线1下料完成！" + "----用户：" + _operatorName,
                        BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

                        lblConveyor1MeasurementUsingTime.Invoke(new Action(() =>
                        {
                            lblConveyor1MeasurementUsingTime.Text = "传送线1检测用时：" + (((float)_conveyor1CheckTimeCountSW.ElapsedMilliseconds) / 1000f).ToString("f3") + "s";
                            _conveyor1CheckTimeCountSW.Stop();
                        })); ;

                        if (_conveyor1HaveExcessProductAlarmDealWithWayIndex == 1)
                            _conveyor1HaveExcessProductAlarmDealWithWayIndex = -1;
                        _conveyor1Status = 0;//修改传送线状态为需要上料
                        _conveyor1LoadUnloadModuleActionFlag = 1;
                    }
                    else
                        _conveyor1LoadUnloadModuleActionFlag = 1;
                }
                else
                    _conveyor1LoadUnloadModuleActionFlag = 1;

            });
        }

        /// <summary>
        /// Conveyor2LoadUnloadModule:传送线2上下料模块
        /// </summary>
        private async void Conveyor2LoadUnloadModule()
        {
            await Task.Run(() =>
            {
                _conveyor2LoadUnloadModuleActionFlag = 0;

                if (_conveyor2Status == 0)//如果为需要上料
                {
                    lock (this)
                    {

                        if (!_conveyor2HaveProductFlag)//如果程序未记录有产品
                        {
                            while (!_axisControlFormVariate._ioStatus[1, 72])//如果设备没有发出允许上游出板信号
                            {
                                if (_deadlyAlarmOccurFlag || !_autoModeFlag)
                                {
                                    return;
                                }
                                _axisControlFormVariate.OutputControl(72, true);
                            }
                        }
                        else
                        {
                            while (_axisControlFormVariate._ioStatus[1, 72])//如果设备没有发出允许上游出板信号
                            {
                                if (_deadlyAlarmOccurFlag || !_autoModeFlag)
                                {
                                    return;
                                }
                                _axisControlFormVariate.OutputControl(72, false);
                            }
                        }

                        while (_axisControlFormVariate._ioStatus[1, 74])//如果设备发出过本机出板请求信号
                        {
                            if (_deadlyAlarmOccurFlag || !_autoModeFlag)
                            {
                                return;
                            }
                            _axisControlFormVariate.OutputControl(74, false);
                        }
                    }

                    if (_inputIOT[90].STATUS || _conveyor2HaveProductFlag)//如果入口传感器ON或者已经记录在下料时有产品进入传送线
                    {
                        //计时
                        if (_conveyor2CheckTimeCountSW.IsRunning)
                            _conveyor2CheckTimeCountSW.Reset();
                        else
                            _conveyor2CheckTimeCountSW.Restart();

                        MyTool.TxtFileProcess.CreateLog("设备动作-传送线2上料动作！" + "----用户：" + _operatorName,
                        BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

                        while (_axisControlFormVariate._ioStatus[1, 72])//如果设备发出允许上游出板信号
                        {
                            if (_deadlyAlarmOccurFlag || !_autoModeFlag)
                            {
                                return;
                            }
                            _axisControlFormVariate.OutputControl(72, false);
                        }

                        lock (this)
                        {
                            //清除timeout deal with way index
                            _conveyor2TimeoutAlarmDealWithWayIndexWhenLoadAndUnload = -1;

                            //控制阻挡气缸上
                            _axisControlFormVariate.OutputControl(76, false);
                            _axisControlFormVariate.OutputControl(77, true);
                        }

                        while (!_inputIOT[81].STATUS || _stateAndAlarmCs._alarmFlag[401] || _conveyor2HaveExcessProductAlarmDealWithWayIndex == 1)// || !_deviceWorkTriggerFlag)//等待阻挡气缸上
                        {
                            if (_deadlyAlarmOccurFlag || !_autoModeFlag ||
                                _conveyor2HaveExcessProductAlarmDealWithWayIndex == 1)
                            {
                                _conveyor2LoadUnloadModuleActionFlag = -1;
                                return;
                            }
                            Thread.Sleep(1);
                        }

                        lock (this)
                        {
                            //控制夹紧气缸松开
                            _axisControlFormVariate.OutputControl(78, true);
                            _axisControlFormVariate.OutputControl(79, false);
                        }

                        while (!_inputIOT[82].STATUS)// || !_deviceWorkTriggerFlag)//等待夹紧气缸松开
                        {
                            if (_deadlyAlarmOccurFlag || !_autoModeFlag)
                            {
                                return;
                            }
                            Thread.Sleep(1);
                        }

                        while (_conveyor2ActionModuleActionFlag == 0 || _stateAndAlarmCs._alarmFlag[342] || 
                            _conveyor2TimeoutAlarmDealWithWayIndexWhenLoadAndUnload != -1 || _stateAndAlarmCs._alarmFlag[401]
                             || _conveyor2HaveExcessProductAlarmDealWithWayIndex == 1)
                        {
                            if (_deadlyAlarmOccurFlag || !_autoModeFlag ||
                                _conveyor2HaveExcessProductAlarmDealWithWayIndex == 1)
                            {
                                _conveyor2LoadUnloadModuleActionFlag = -1;
                                return;
                            }

                            if (_conveyor2TimeoutAlarmDealWithWayIndexWhenLoadAndUnload == 1)
                            {
                                _conveyor2TimeoutAlarmDealWithWayIndexWhenLoadAndUnload = -1;
                                break;
                            }

                            switch (_conveyor2TimeoutAlarmDealWithWayIndexWhenLoadAndUnload)
                            {
                                case 0://如果为重试，不进行任何动作
                                    _conveyor2TimeoutAlarmDealWithWayIndexWhenLoadAndUnload = -1;
                                    break;
                               // case 1://如果为忽略
                                    //_conveyor2TimeoutAlarmDealWithWayIndexWhenLoadAndUnload = -1;
                                    //continue;
                                case 2://如果为无物料
                                    _conveyor2Status = 0;
                                    _conveyor2LoadUnloadModuleActionFlag = 1;
                                    _conveyor2TimeoutAlarmDealWithWayIndexWhenLoadAndUnload = -1;
                                    return;
                            }

                            Thread.Sleep(1);
                        }

                        _conveyor2ActionModuleActionFlag = 0;
                        Conveyor2ActionModule(_productParameter[1]._conveyor2InHighSpeed);

                        while (_conveyor2ActionModuleActionFlag != 1)//如果传送线未完成速度设定
                        {
                            if (_deadlyAlarmOccurFlag || !_autoModeFlag)
                            {
                                return;
                            }
                            Thread.Sleep(1);
                        }

                        while (_inputIOT[90].STATUS || _conveyor1ActionModuleActionFlag == 0 || _stateAndAlarmCs._alarmFlag[342] ||
                            _conveyor2TimeoutAlarmDealWithWayIndexWhenLoadAndUnload != -1 || _stateAndAlarmCs._alarmFlag[401]
                             || _conveyor2HaveExcessProductAlarmDealWithWayIndex == 1)//等待入口传感器信号灭掉
                        {
                            if (_deadlyAlarmOccurFlag || !_autoModeFlag ||
                                _conveyor2HaveExcessProductAlarmDealWithWayIndex == 1)
                            {
                                _conveyor2LoadUnloadModuleActionFlag = -1;
                                return;
                            }

                            if (_conveyor2TimeoutAlarmDealWithWayIndexWhenLoadAndUnload == 1)
                            {
                                _conveyor2TimeoutAlarmDealWithWayIndexWhenLoadAndUnload = -1;
                                break;
                            }

                            switch (_conveyor2TimeoutAlarmDealWithWayIndexWhenLoadAndUnload)
                            {
                                case 0://如果为重试，不进行任何动作
                                    _conveyor2TimeoutAlarmDealWithWayIndexWhenLoadAndUnload = -1;
                                    break;
                                //case 1://如果为忽略
                                    //_conveyor2TimeoutAlarmDealWithWayIndexWhenLoadAndUnload = -1;
                                    //continue;
                                case 2://如果为无物料
                                    while (_conveyor2ActionModuleActionFlag == 0)//等待控制传送线动作模块空闲 !_deviceWorkTriggerFlag || 
                                    {
                                        if (_deadlyAlarmOccurFlag || !_autoModeFlag)
                                        {
                                            return;
                                        }
                                        Thread.Sleep(1);
                                    }
                                    _conveyor2ActionModuleActionFlag = 0;
                                    Conveyor2ActionModule(-1);//控制传送线停止
                                    while (_conveyor2ActionModuleActionFlag != 2)//如果传送线未完成速度设定
                                    {
                                        if (_deadlyAlarmOccurFlag || !_autoModeFlag)
                                        {
                                            return;
                                        }
                                        Thread.Sleep(1);
                                    }
                                    _conveyor2Status = 0;
                                    _conveyor2LoadUnloadModuleActionFlag = 1;
                                    _conveyor2TimeoutAlarmDealWithWayIndexWhenLoadAndUnload = -1;
                                    return;
                            }

                            Thread.Sleep(1);
                        }

                        while (!_inputIOT[88].STATUS || _conveyor2ActionModuleActionFlag == 0 || _stateAndAlarmCs._alarmFlag[342] ||
                            _conveyor2TimeoutAlarmDealWithWayIndexWhenLoadAndUnload != -1 || _stateAndAlarmCs._alarmFlag[401]
                             || _conveyor2HaveExcessProductAlarmDealWithWayIndex == 1)//如果传送线减速信号没有被触发，或者传送线2动作控制模块不为off，等待
                        {
                            if (_deadlyAlarmOccurFlag || !_autoModeFlag ||
                                _conveyor2HaveExcessProductAlarmDealWithWayIndex == 1)
                            {
                                _conveyor2LoadUnloadModuleActionFlag = -1;
                                return;
                            }

                            if (_conveyor2TimeoutAlarmDealWithWayIndexWhenLoadAndUnload == 1)
                            {
                                _conveyor2TimeoutAlarmDealWithWayIndexWhenLoadAndUnload = -1;
                                break;
                            }

                            switch (_conveyor2TimeoutAlarmDealWithWayIndexWhenLoadAndUnload)
                            {
                                case 0://如果为重试，不进行任何动作
                                    _conveyor2TimeoutAlarmDealWithWayIndexWhenLoadAndUnload = -1;
                                    break;
                                //case 1://如果为忽略
                                    //_conveyor2TimeoutAlarmDealWithWayIndexWhenLoadAndUnload = -1;
                                    //continue;
                                case 2://如果为无物料
                                    while (_conveyor2ActionModuleActionFlag == 0)//等待控制传送线动作模块空闲!_deviceWorkTriggerFlag ||
                                    {
                                        if (_deadlyAlarmOccurFlag || !_autoModeFlag)
                                        {
                                            return;
                                        }
                                        Thread.Sleep(1);
                                    }
                                    _conveyor2ActionModuleActionFlag = 0;
                                    Conveyor2ActionModule(-1);//控制传送线停止
                                    while (_conveyor2ActionModuleActionFlag != 2)//如果传送线未完成速度设定
                                    {
                                        if (_deadlyAlarmOccurFlag || !_autoModeFlag)
                                        {
                                            return;
                                        }
                                        Thread.Sleep(1);
                                    }
                                    _conveyor2Status = 0;
                                    _conveyor2LoadUnloadModuleActionFlag = 1;
                                    _conveyor2TimeoutAlarmDealWithWayIndexWhenLoadAndUnload = -1;
                                    return;
                            }

                            Thread.Sleep(1);
                        }

                        _conveyor2ActionModuleActionFlag = 0;
                        Conveyor2ActionModule(_productParameter[1]._conveyor2InLowSpeed);//切换低速

                        while (_conveyor2ActionModuleActionFlag != 1)//如果传送线未完成速度设定
                        {
                            if (_deadlyAlarmOccurFlag || !_autoModeFlag)
                            {
                                return;
                            }
                            Thread.Sleep(1);
                        }

                        while (!_conveyor2FinishLoadT.STATUS || _conveyor2ActionModuleActionFlag == 0 || _stateAndAlarmCs._alarmFlag[342] ||
                            _conveyor1TimeoutAlarmDealWithWayIndexWhenLoadAndUnload != -1 || _stateAndAlarmCs._alarmFlag[401]
                             || _conveyor2HaveExcessProductAlarmDealWithWayIndex == 1)//如果上料信号未被触发至完成，或传送线2动作控制模块不为OFF，等待
                        {
                            if (_deadlyAlarmOccurFlag || !_autoModeFlag ||
                                _conveyor2HaveExcessProductAlarmDealWithWayIndex == 1)
                            {
                                _conveyor2LoadUnloadModuleActionFlag = -1;
                                return;
                            }

                            if (_conveyor2TimeoutAlarmDealWithWayIndexWhenLoadAndUnload == 1)
                            {
                                _conveyor2TimeoutAlarmDealWithWayIndexWhenLoadAndUnload = -1;
                                break;
                            }

                            switch (_conveyor2TimeoutAlarmDealWithWayIndexWhenLoadAndUnload)
                            {
                                case 0://如果为重试，不进行任何动作
                                    _conveyor2TimeoutAlarmDealWithWayIndexWhenLoadAndUnload = -1;
                                    break;
                                //case 1://如果为忽略
                                    //_conveyor2TimeoutAlarmDealWithWayIndexWhenLoadAndUnload = -1;
                                    //continue;
                                case 2://如果为无物料
                                    while ( _conveyor2ActionModuleActionFlag == 0)//等待控制传送线动作模块空闲!_deviceWorkTriggerFlag ||
                                    {
                                        if (_deadlyAlarmOccurFlag || !_autoModeFlag)
                                        {
                                            return;
                                        }
                                        Thread.Sleep(1);
                                    }

                                    _conveyor2ActionModuleActionFlag = 0;
                                    Conveyor2ActionModule(-1);//控制传送线停止

                                    while (_conveyor2ActionModuleActionFlag != 2)//如果传送线未完成速度设定
                                    {
                                        if (_deadlyAlarmOccurFlag || !_autoModeFlag)
                                        {
                                            return;
                                        }
                                        Thread.Sleep(1);
                                    }
                                    _conveyor2Status = 0;
                                    _conveyor2LoadUnloadModuleActionFlag = 1;
                                    _conveyor2TimeoutAlarmDealWithWayIndexWhenLoadAndUnload = -1;
                                    return;
                            }

                            Thread.Sleep(1);
                        }

                        _conveyor2ActionModuleActionFlag = 0;
                        Conveyor2ActionModule(-1);//传送线停止

                        while (_conveyor2ActionModuleActionFlag != 2)//如果传送线未完成停止
                        {
                            if (_deadlyAlarmOccurFlag || !_autoModeFlag)
                            {
                                return;
                            }
                            Thread.Sleep(1);
                        }

                        lock (this)
                        {
                            //控制夹紧气缸夹紧
                            _axisControlFormVariate.OutputControl(78, false);
                            _axisControlFormVariate.OutputControl(79, true);
                        }

                        while (!_inputIOT[83].STATUS || _stateAndAlarmCs._alarmFlag[401] || _conveyor2HaveExcessProductAlarmDealWithWayIndex == 1)// || !_deviceWorkTriggerFlag)//等待夹紧气缸夹紧
                        {
                            if (_deadlyAlarmOccurFlag || !_autoModeFlag
                                || _conveyor2HaveExcessProductAlarmDealWithWayIndex == 1)
                            {
                                _conveyor2LoadUnloadModuleActionFlag = -1;
                                return;
                            }
                            Thread.Sleep(1);
                        }

                        MyTool.TxtFileProcess.CreateLog("设备动作-传送线2上料完成！" + "----用户：" + _operatorName,
                        BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

                        if (_conveyor2HaveExcessProductAlarmDealWithWayIndex != 1)
                        {
                            _conveyor2Status = 1;
                        }
                        _conveyor2LoadUnloadModuleActionFlag = 1;
                    }
                    else
                        _conveyor2LoadUnloadModuleActionFlag = 1;
                }
                else if (_conveyor2Status == 1)//如果为需要检测
                {
                    lock (this)
                    {
                        if (_axisControlFormVariate._ioStatus[1, 72])//如果设备发出允许上游出板信号
                            _axisControlFormVariate.OutputControl(72, false);

                        if (_axisControlFormVariate._ioStatus[1, 74])//如果设备发出本机出板请求信号
                            _axisControlFormVariate.OutputControl(74, false);
                    }

                    _conveyor2LoadUnloadModuleActionFlag = 1;
                }
                else if (_conveyor2Status == 2)//如果为需要下料
                {
                    lock (this)
                    {
                        while (_axisControlFormVariate._ioStatus[1, 72])//如果设备发出允许上游出板信号
                        {
                            if (_deadlyAlarmOccurFlag || !_autoModeFlag)
                            {
                                return;
                            }
                            _axisControlFormVariate.OutputControl(72, false);
                        }

                        while (!_axisControlFormVariate._ioStatus[1, 74])//如果设备没有发出过本机出板请求信号
                        {
                            if (_deadlyAlarmOccurFlag || !_autoModeFlag)
                            {
                                return;
                            }
                            _axisControlFormVariate.OutputControl(74, true);
                        }
                    }

                    if (_inputIOT[70].STATUS)//如果接收到下游允许出板请求信号
                    {
                        MyTool.TxtFileProcess.CreateLog("设备动作-传送线2下料动作！" + "----用户：" + _operatorName,
                        BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

                        //清除timeout deal with way index
                        _conveyor2TimeoutAlarmDealWithWayIndexWhenLoadAndUnload = -1;

                        _conveyor2HaveProductFlag = false;

                        //如果传送线2有多余产品处理方式为1（流出），等到正式开始流出时置为-1，等待报警数据被关闭
                        while (_conveyor2HaveExcessProductAlarmDealWithWayIndex == 1 && _stateAndAlarmCs._alarmFlag[401])
                        {
                            if (_deadlyAlarmOccurFlag || !_autoModeFlag)
                            {
                                return;
                            }

                            Thread.Sleep(1);
                        }
                        if (_conveyor2HaveExcessProductAlarmDealWithWayIndex == 1)
                            _conveyor2HaveExcessProductAlarmDealWithWayIndex = -1;

                        lock (this)
                        {
                            //控制夹紧气缸松开
                            _axisControlFormVariate.OutputControl(78, true);
                            _axisControlFormVariate.OutputControl(79, false);
                        }

                        while (!_inputIOT[82].STATUS || !_deviceWorkTriggerFlag)//等待夹紧气缸松开
                        {
                            if (_deadlyAlarmOccurFlag || !_autoModeFlag)
                            {
                                return;
                            }
                            Thread.Sleep(1);
                        }

                        lock (this)
                        {
                            //控制阻挡气缸下
                            _axisControlFormVariate.OutputControl(76, true);
                            _axisControlFormVariate.OutputControl(77, false);
                        }

                        while (!_inputIOT[80].STATUS || !_deviceWorkTriggerFlag)//等待阻挡气缸下
                        {
                            if (_deadlyAlarmOccurFlag || !_autoModeFlag)
                            {
                                return;
                            }
                            Thread.Sleep(1);
                        }

                        while (_conveyor2ActionModuleActionFlag == 0 || _stateAndAlarmCs._alarmFlag[343] || _conveyor2TimeoutAlarmDealWithWayIndexWhenLoadAndUnload != -1)//!_deviceWorkTriggerFlag || 
                        {
                            if (_deadlyAlarmOccurFlag || !_autoModeFlag)
                            {
                                return;
                            }
                            if (_conveyor2TimeoutAlarmDealWithWayIndexWhenLoadAndUnload == 1)
                            {
                                _conveyor2TimeoutAlarmDealWithWayIndexWhenLoadAndUnload = -1;
                                break;
                            }
                            switch (_conveyor2TimeoutAlarmDealWithWayIndexWhenLoadAndUnload)
                            {
                                case 0://如果为重试，不进行任何动作
                                    _conveyor2TimeoutAlarmDealWithWayIndexWhenLoadAndUnload = -1;
                                    break;
                                //case 1://如果为忽略
                                   // _conveyor2TimeoutAlarmDealWithWayIndexWhenLoadAndUnload = -1;
                                    //continue;
                                case 2://如果为无物料
                                    _conveyor2Status = 0;
                                    _conveyor2LoadUnloadModuleActionFlag = 1;
                                    _conveyor2TimeoutAlarmDealWithWayIndexWhenLoadAndUnload = -1;
                                    return;
                            }

                            Thread.Sleep(1);
                        }

                        _conveyor2ActionModuleActionFlag = 0;
                        Conveyor2ActionModule(_productParameter[1]._conveyor2OutSpeed);

                        while (_conveyor2ActionModuleActionFlag != 1)//如果传送线未完成速度设定
                        {
                            if (_deadlyAlarmOccurFlag || !_autoModeFlag)
                            {
                                return;
                            }
                            Thread.Sleep(1);
                        }

                        while (!_inputIOT[91].STATUS || _stateAndAlarmCs._alarmFlag[343] || _conveyor2TimeoutAlarmDealWithWayIndexWhenLoadAndUnload != -1)//等待出口传感器触发
                        {
                            if (_deadlyAlarmOccurFlag || !_autoModeFlag)
                            {
                                return;
                            }

                            if (_conveyor2UnloadStopCylinderUpT.STATUS && !_inputIOT[81].STATUS)//如果下料阻挡气缸应当上升
                            {
                                lock (this)
                                {
                                    _axisControlFormVariate.OutputControl(76, false);
                                    _axisControlFormVariate.OutputControl(77, true);
                                }
                            }

                            if (_inputIOT[90].STATUS && !_conveyor2HaveProductFlag)//如果下料时入口传感器被触发
                            {
                                _conveyor2HaveProductFlag = true;//设置标志为true，防止由于下料时，上料传感器被触发，后面不在触发导致的死机
                            }
                            if (_conveyor2TimeoutAlarmDealWithWayIndexWhenLoadAndUnload == 1)
                            {
                                _conveyor2TimeoutAlarmDealWithWayIndexWhenLoadAndUnload = -1;
                                break;
                            }
                            switch (_conveyor2TimeoutAlarmDealWithWayIndexWhenLoadAndUnload)
                            {
                                case 0://如果为重试，不进行任何动作
                                    _conveyor2TimeoutAlarmDealWithWayIndexWhenLoadAndUnload = -1;
                                    break;
                                //case 1://如果为忽略
                                    //_conveyor2TimeoutAlarmDealWithWayIndexWhenLoadAndUnload = -1;
                                    //continue;
                                case 2://如果为无物料
                                    while (_conveyor2ActionModuleActionFlag == 0)//等待控制传送线动作模块空闲!_deviceWorkTriggerFlag || 
                                    {
                                        if (_deadlyAlarmOccurFlag || !_autoModeFlag)
                                        {
                                            return;
                                        }
                                        Thread.Sleep(1);
                                    }
                                    _conveyor2ActionModuleActionFlag = 0;
                                    Conveyor2ActionModule(-1);//控制传送线停止
                                    while (_conveyor2ActionModuleActionFlag != 2)//如果传送线未完成速度设定
                                    {
                                        if (_deadlyAlarmOccurFlag || !_autoModeFlag)
                                        {
                                            return;
                                        }
                                        Thread.Sleep(1);
                                    }
                                    _conveyor2Status = 0;
                                    _conveyor2LoadUnloadModuleActionFlag = 1;
                                    _conveyor2TimeoutAlarmDealWithWayIndexWhenLoadAndUnload = -1;
                                    return;
                            }


                            Thread.Sleep(1);
                        }

                        while (!_conveyor2FinishUnloadT.STATUS || _stateAndAlarmCs._alarmFlag[343] || _conveyor2TimeoutAlarmDealWithWayIndexWhenLoadAndUnload != -1)//如果传送线未触发下料完成
                        {
                            if (_deadlyAlarmOccurFlag || !_autoModeFlag)
                            {
                                return;
                            }

                            if (_conveyor2UnloadStopCylinderUpT.STATUS && !_inputIOT[81].STATUS)//如果下料阻挡气缸应当上升
                            {
                                lock (this)
                                {
                                    _axisControlFormVariate.OutputControl(76, false);
                                    _axisControlFormVariate.OutputControl(77, true);
                                }
                            }

                            if (_inputIOT[90].STATUS && !_conveyor2HaveProductFlag)//如果下料时入口传感器被触发
                            {
                                _conveyor2HaveProductFlag = true;//设置标志为true，防止由于下料时，上料传感器被触发，后面不在触发导致的死机
                            }
                            if (_conveyor2TimeoutAlarmDealWithWayIndexWhenLoadAndUnload == 1)
                            {
                                _conveyor2TimeoutAlarmDealWithWayIndexWhenLoadAndUnload = -1;
                                break;
                            }
                            switch (_conveyor2TimeoutAlarmDealWithWayIndexWhenLoadAndUnload)
                            {
                                case 0://如果为重试，不进行任何动作
                                    _conveyor2TimeoutAlarmDealWithWayIndexWhenLoadAndUnload = -1;
                                    break;
                                //case 1://如果为忽略
                                   // _conveyor2TimeoutAlarmDealWithWayIndexWhenLoadAndUnload = -1;
                                    //continue;
                                case 2://如果为无物料
                                    while (_conveyor2ActionModuleActionFlag == 0)//等待控制传送线动作模块空闲!_deviceWorkTriggerFlag || 
                                    {
                                        if (_deadlyAlarmOccurFlag || !_autoModeFlag)
                                        {
                                            return;
                                        }
                                        Thread.Sleep(1);
                                    }
                                    _conveyor2ActionModuleActionFlag = 0;
                                    Conveyor2ActionModule(-1);//控制传送线停止
                                    while (_conveyor2ActionModuleActionFlag != 2)//如果传送线未完成速度设定
                                    {
                                        if (_deadlyAlarmOccurFlag || !_autoModeFlag)
                                        {
                                            return;
                                        }
                                        Thread.Sleep(1);
                                    }
                                    _conveyor2Status = 0;
                                    _conveyor2LoadUnloadModuleActionFlag = 1;
                                    _conveyor2TimeoutAlarmDealWithWayIndexWhenLoadAndUnload = -1;
                                    return;
                            }

                            Thread.Sleep(1);
                        }

                        while (!_conveyor2ActionModuleOffDlyT.STATUS)
                        {
                            if (_deadlyAlarmOccurFlag || !_autoModeFlag)
                            {
                                return;
                            }
                            Thread.Sleep(1);
                        }

                        //控制传送线停止
                        _conveyor2ActionModuleActionFlag = 0;
                        Conveyor2ActionModule(-1);

                        while (_conveyor2ActionModuleActionFlag != 2)//如果传送线未完成停止
                        {
                            if (_deadlyAlarmOccurFlag || !_autoModeFlag)
                            {
                                return;
                            }

                            if (_inputIOT[90].STATUS && !_conveyor2HaveProductFlag)//如果下料时入口传感器被触发
                            {
                                _conveyor2HaveProductFlag = true;//设置标志为true，防止由于下料时，上料传感器被触发，后面不在触发导致的死机
                            }
                            Thread.Sleep(1);
                        }

                        MyTool.TxtFileProcess.CreateLog("设备动作-传送线2下料完成！" + "----用户：" + _operatorName,
                        BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

                        lblConveyor2MeasurementUsingTime.Invoke(new Action(() =>
                        {
                            lblConveyor2MeasurementUsingTime.Text = "传送线2检测用时：" + (((float)_conveyor2CheckTimeCountSW.ElapsedMilliseconds) / 1000f).ToString("f3") + "s";
                            _conveyor2CheckTimeCountSW.Stop();
                        })); ;

                        if (_conveyor2HaveExcessProductAlarmDealWithWayIndex == 1)
                            _conveyor2HaveExcessProductAlarmDealWithWayIndex = -1;
                        _conveyor2Status = 0;//修改传送线状态为需要上料
                        _conveyor2LoadUnloadModuleActionFlag = 1;
                    }
                    else
                        _conveyor2LoadUnloadModuleActionFlag = 1;
                }
                else
                    _conveyor2LoadUnloadModuleActionFlag = 1;

            });
        }

        /// <summary>
        /// DeviceTotalWorkModule:设备整体工作模块
        /// </summary>
        /// <param name="workIndex">int:设备工作索引，0-检测传送线1上的产品，1-检测传送线2上的产品</param>
        private async void DeviceTotalWorkModule(int conveyorIndex)
        {
            await Task.Run(() =>
            {
                _deviceTotalWorkModuleActionFlag = 0;
                _leftAndRightCameraGrabImageModuleControlFlag = 0;

                MyTool.TxtFileProcess.CreateLog("设备动作-传送线" + (conveyorIndex + 1).ToString() + "上产品整体动作模块启用" + "----用户：" + _operatorName,
                BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);


                //提前打开后相机光源
                if (_axisControlFormVariate._ioStatus[1, BACK_CAMERA_LIGHT_TRIGGER_OUT_IO] || _axisControlFormVariate._ioStatus[1, BACK_CAMERA_LIGHT_TRIGGER_OUT_IO2])
                {
                    _axisControlFormVariate.OutputControl(BACK_CAMERA_LIGHT_TRIGGER_OUT_IO, false);//关闭光源
                    _axisControlFormVariate.OutputControl(BACK_CAMERA_LIGHT_TRIGGER_OUT_IO2, false);//关闭光源
                }

                bool currentPointNeedInfer = false;
                if (_inferTimCountSW.IsRunning)
                    _inferTimCountSW.Reset();
                else
                    _inferTimCountSW.Restart();

                _baseSourceImageFolderPath = "";
                _baseInferredOKImageFolderPath = "";
                _baseInferredNGImageFolderPath = "";
                _baseResultCsvFileFolderPath = "";
                for (int i = 0; i < _productParameter[conveyorIndex]._pointPositionDT.Rows.Count; i++)
                {
                    //判定当前点位是否需要检测，当needInferOrNotDt为空，表明不与MES通讯，由屏蔽数据来控制点位启用与否。反之以MES反馈数据为准
                    if (_needInferOrNotDt != null)
                    {
                        if (_needInferOrNotDt.Rows.Count > 0)
                        {
                            int elementIndex = Convert.ToInt32(_productParameter[conveyorIndex]._pointPositionDT.Rows[i][5].ToString().Substring(
                                _productParameter[conveyorIndex]._pointPositionDT.Rows[i][5].ToString().IndexOf("_") + 1,
                                _productParameter[conveyorIndex]._pointPositionDT.Rows[i][5].ToString().IndexOf("-") - _productParameter[conveyorIndex]._pointPositionDT.Rows[i][5].ToString().IndexOf("_") - 1));
                            if (_needInferOrNotDt.Rows.Count >= elementIndex)
                            {
                                if (_needInferOrNotDt.Rows[elementIndex - 1][1].ToString() == "OK")
                                    currentPointNeedInfer = true;
                                else
                                    currentPointNeedInfer = false;
                            }
                            else
                            {
                                currentPointNeedInfer = true;
                            }
                        }
                        else
                        {
                            if (Convert.ToInt32(_productParameter[conveyorIndex]._pointPositionDT.Rows[i][4]) == 0)
                            {
                                currentPointNeedInfer = true;
                            }
                            else
                            {
                                currentPointNeedInfer = false;
                            }
                        }
                    }
                    else
                    {
                        if (Convert.ToInt32(_productParameter[conveyorIndex]._pointPositionDT.Rows[i][4]) == 0)
                        {
                            currentPointNeedInfer = true;
                        }
                        else
                        {
                            currentPointNeedInfer = false;
                        }
                    }

                    _currentDestinationPointIndex = i + 1;
                    if (currentPointNeedInfer)//如果点位需要动作
                    {
                        while (_xyMoveToAssignedPointPositionModuleActionFlag == 0 || !_deviceWorkTriggerFlag ||
                                (conveyorIndex == 0 && _conveyor1HaveExcessProductAlarmDealWithWayIndex == 1) ||
                                (conveyorIndex == 1 && _conveyor2HaveExcessProductAlarmDealWithWayIndex == 1))
                        {
                            if (_deadlyAlarmOccurFlag || !_autoModeFlag ||
                               (conveyorIndex == 0 && _conveyor1HaveExcessProductAlarmDealWithWayIndex == 1) ||
                               (conveyorIndex == 1 && _conveyor2HaveExcessProductAlarmDealWithWayIndex == 1))
                            {
                                _deviceTotalWorkModuleActionFlag = -1;
                                return;
                            }
                            Thread.Sleep(1);
                        }

                        _xyMoveToAssignedPointPositionModuleActionFlag = 0;
                        MoveXYAxisToAssignedPointModule(_currentDestinationPointIndex, conveyorIndex, conveyorIndex);//运动至指定位置

                        while (_xyMoveToAssignedPointPositionModuleActionFlag == 0 || !_deviceWorkTriggerFlag
                                || _currentDestinationPointIndex != _xyMoveToAssignedPointPositionModuleActionFlag ||
                                (conveyorIndex == 0 && _conveyor1HaveExcessProductAlarmDealWithWayIndex == 1) ||
                                (conveyorIndex == 1 && _conveyor2HaveExcessProductAlarmDealWithWayIndex == 1))//运动至指定位置
                        {
                            if (_deadlyAlarmOccurFlag || !_autoModeFlag ||
                               (conveyorIndex == 0 && _conveyor1HaveExcessProductAlarmDealWithWayIndex == 1) ||
                               (conveyorIndex == 1 && _conveyor2HaveExcessProductAlarmDealWithWayIndex == 1))
                            {
                                _deviceTotalWorkModuleActionFlag = -1;
                                return;
                            }
                            Thread.Sleep(1);
                        }

                        while (_leftAndRightCameraGrabImageModuleActionFlag == 0 ||
                                (conveyorIndex == 0 && _conveyor1HaveExcessProductAlarmDealWithWayIndex == 1) ||
                                (conveyorIndex == 1 && _conveyor2HaveExcessProductAlarmDealWithWayIndex == 1))//等待图像抓取模块是否空闲
                        {
                            if (_deadlyAlarmOccurFlag || !_autoModeFlag ||
                               (conveyorIndex == 0 && _conveyor1HaveExcessProductAlarmDealWithWayIndex == 1) ||
                               (conveyorIndex == 1 && _conveyor2HaveExcessProductAlarmDealWithWayIndex == 1))
                            {
                                _deviceTotalWorkModuleActionFlag = -1;
                                return;
                            }
                            Thread.Sleep(1);
                        }

                        if (_leftAndRightCameraGrabImageModuleActionFlag == 2)//如果反馈的抓取结果为NG（为了提高节拍，在触发相机拍摄之后就进行下一个点位的移动，极端情况下有可能会出现抓取失败，抓取失败的时候就反馈值为2，这里再次进行抓取）
                        {
                            i--;//点位号减1，用于判定上一个点是否需要检测
                            if (_needInferOrNotDt != null)
                            {
                                if (_needInferOrNotDt.Rows.Count > 0)
                                {
                                    int elementIndex = Convert.ToInt32(_productParameter[conveyorIndex]._pointPositionDT.Rows[i][5].ToString().Substring(
                                        _productParameter[conveyorIndex]._pointPositionDT.Rows[i][5].ToString().IndexOf("_") + 1,
                                        _productParameter[conveyorIndex]._pointPositionDT.Rows[i][5].ToString().IndexOf("-") - _productParameter[conveyorIndex]._pointPositionDT.Rows[i][5].ToString().IndexOf("_") - 1));
                                    if (_needInferOrNotDt.Rows.Count >= elementIndex)
                                    {
                                        if (_needInferOrNotDt.Rows[elementIndex - 1][1].ToString() == "OK")
                                            currentPointNeedInfer = true;
                                        else
                                            currentPointNeedInfer = false;
                                    }
                                    else
                                    {
                                        currentPointNeedInfer = true;
                                    }
                                }
                                else
                                {
                                    if (Convert.ToInt32(_productParameter[conveyorIndex]._pointPositionDT.Rows[i][4]) == 0)
                                    {
                                        currentPointNeedInfer = true;
                                    }
                                    else
                                    {
                                        currentPointNeedInfer = false;
                                    }
                                }
                            }
                            else
                            {
                                if (Convert.ToInt32(_productParameter[conveyorIndex]._pointPositionDT.Rows[i][4]) == 0)
                                {
                                    currentPointNeedInfer = true;
                                }
                                else
                                {
                                    currentPointNeedInfer = false;
                                }
                            }

                            while (!currentPointNeedInfer)//当图片不检测时
                            {
                                i--;//点位号继续减1，直至找到要检测的点位号
                                if (_needInferOrNotDt != null)
                                {
                                    if (_needInferOrNotDt.Rows.Count > 0)
                                    {
                                        int elementIndex = Convert.ToInt32(_productParameter[conveyorIndex]._pointPositionDT.Rows[i][5].ToString().Substring(
                                            _productParameter[conveyorIndex]._pointPositionDT.Rows[i][5].ToString().IndexOf("_") + 1,
                                            _productParameter[conveyorIndex]._pointPositionDT.Rows[i][5].ToString().IndexOf("-") - _productParameter[conveyorIndex]._pointPositionDT.Rows[i][5].ToString().IndexOf("_") - 1));
                                        if (_needInferOrNotDt.Rows.Count >= elementIndex)
                                        {
                                            if (_needInferOrNotDt.Rows[elementIndex - 1][1].ToString() == "OK")
                                                currentPointNeedInfer = true;
                                            else
                                                currentPointNeedInfer = false;
                                        }
                                        else
                                        {
                                            currentPointNeedInfer = true;
                                        }
                                    }
                                    else
                                    {
                                        if (Convert.ToInt32(_productParameter[conveyorIndex]._pointPositionDT.Rows[i][4]) == 0)
                                        {
                                            currentPointNeedInfer = true;
                                        }
                                        else
                                        {
                                            currentPointNeedInfer = false;
                                        }
                                    }
                                }
                                else
                                {
                                    if (Convert.ToInt32(_productParameter[conveyorIndex]._pointPositionDT.Rows[i][4]) == 0)
                                    {
                                        currentPointNeedInfer = true;
                                    }
                                    else
                                    {
                                        currentPointNeedInfer = false;
                                    }
                                }
                            }
                            //由于for循环会再加1，因此这里进行一次减1，确保能走的点位为上一个需要检测的点位
                            i--;

                            //抓取图像失败重试次数加一，用于报警
                            _grabImageFailedRetryCount++;
                            if (_grabImageFailedRetryCount > 3)//如果三次连续失败，报警
                            {
                                _stateAndAlarmCs._alarmFlag[360] = true;
                                while (_stateAndAlarmCs._alarmFlag[360] ||
                                (conveyorIndex == 0 && _conveyor1HaveExcessProductAlarmDealWithWayIndex == 1) ||
                                (conveyorIndex == 1 && _conveyor2HaveExcessProductAlarmDealWithWayIndex == 1))
                                {
                                    if (_deadlyAlarmOccurFlag || !_autoModeFlag ||
                               (conveyorIndex == 0 && _conveyor1HaveExcessProductAlarmDealWithWayIndex == 1) ||
                               (conveyorIndex == 1 && _conveyor2HaveExcessProductAlarmDealWithWayIndex == 1))
                                    {
                                        _deviceTotalWorkModuleActionFlag = -1;
                                        return;
                                    }
                                    Thread.Sleep(1);
                                }
                                _grabImageFailedRetryCount = 0;
                            }

                            //清除相机缓存
                            //_leftCameraCs.ClearCameraImage();
                            //_rightCameraCs.ClearCameraImage();

                            //完成点位重新设定工作后将抓取结果标志重新置位-1
                            _leftAndRightCameraGrabImageModuleActionFlag = -1;
                            continue;
                        }
                        else if(_leftAndRightCameraGrabImageModuleActionFlag == 1)
                        {
                            //抓取成功后，将抓取失败计数置为0
                            _grabImageFailedRetryCount = 0;
                        }

                        _triggerGrabImageFinishFlag = false;
                        _leftAndRightCameraGrabImageModuleControlFlag = 1;

                        while (_leftAndRightCameraGrabImageModuleControlFlag == 1 ||
                                (conveyorIndex == 0 && _conveyor1HaveExcessProductAlarmDealWithWayIndex == 1) ||
                                (conveyorIndex == 1 && _conveyor2HaveExcessProductAlarmDealWithWayIndex == 1))//等待抓取图像模块启动
                        {
                            if (_deadlyAlarmOccurFlag || !_autoModeFlag ||
                               (conveyorIndex == 0 && _conveyor1HaveExcessProductAlarmDealWithWayIndex == 1) ||
                               (conveyorIndex == 1 && _conveyor2HaveExcessProductAlarmDealWithWayIndex == 1))
                            {
                                _deviceTotalWorkModuleActionFlag = -1;
                                return;
                            }
                            Thread.Sleep(1);
                        }

                        //判定是否触发完成抓取图像，如果不为最后一点，那么只要触发相机就行，如果为最后一点，那么需要完成整个抓取
                        while (!(_leftAndRightCameraGrabImageModuleActionFlag != 0 || (_triggerGrabImageFinishFlag && i != _productParameter[conveyorIndex]._pointPositionDT.Rows.Count - 1)) ||
                                (conveyorIndex == 0 && _conveyor1HaveExcessProductAlarmDealWithWayIndex == 1) ||
                                (conveyorIndex == 1 && _conveyor2HaveExcessProductAlarmDealWithWayIndex == 1))
                        {
                            if (_deadlyAlarmOccurFlag || !_autoModeFlag ||
                               (conveyorIndex == 0 && _conveyor1HaveExcessProductAlarmDealWithWayIndex == 1) ||
                               (conveyorIndex == 1 && _conveyor2HaveExcessProductAlarmDealWithWayIndex == 1))
                            {
                                _deviceTotalWorkModuleActionFlag = -1;
                                return;
                            }
                            Thread.Sleep(1);
                        }

                        //如果为最后一个点（由于for循环到最后一个点之后就会完成循环结束动作，因此最后一个点位需要等到抓取图像彻底完成之后才能结束，同样也要判定抓取图像是否NG)
                        if (i == _productParameter[conveyorIndex]._pointPositionDT.Rows.Count - 1)
                        {
                            //如果反馈的抓取结果为NG（为了提高节拍，在触发相机拍摄之后就进行下一个点位的移动，极端情况下有可能会出现抓取失败，抓取失败的时候就反馈值为2，这里再次进行抓取）
                            if (_leftAndRightCameraGrabImageModuleActionFlag == 2)
                            {
                                i--;
                                if (_needInferOrNotDt != null)//MES有反馈数据的时候
                                {
                                    if (_needInferOrNotDt.Rows.Count > 0)
                                    {
                                        int elementIndex = Convert.ToInt32(_productParameter[conveyorIndex]._pointPositionDT.Rows[i][5].ToString().Substring(
                                            _productParameter[conveyorIndex]._pointPositionDT.Rows[i][5].ToString().IndexOf("_") + 1,
                                            _productParameter[conveyorIndex]._pointPositionDT.Rows[i][5].ToString().IndexOf("-") - _productParameter[conveyorIndex]._pointPositionDT.Rows[i][5].ToString().IndexOf("_") - 1));
                                        if (_needInferOrNotDt.Rows.Count >= elementIndex)
                                        {
                                            if (_needInferOrNotDt.Rows[elementIndex - 1][1].ToString() == "OK")
                                                currentPointNeedInfer = true;
                                            else
                                                currentPointNeedInfer = false;
                                        }
                                        else
                                        {
                                            currentPointNeedInfer = true;
                                        }
                                    }
                                    else
                                    {
                                        if (Convert.ToInt32(_productParameter[conveyorIndex]._pointPositionDT.Rows[i][4]) == 0)
                                        {
                                            currentPointNeedInfer = true;
                                        }
                                        else
                                        {
                                            currentPointNeedInfer = false;
                                        }
                                    }
                                }
                                else
                                {
                                    if (Convert.ToInt32(_productParameter[conveyorIndex]._pointPositionDT.Rows[i][4]) == 0)
                                    {
                                        currentPointNeedInfer = true;
                                    }
                                    else
                                    {
                                        currentPointNeedInfer = false;
                                    }
                                }

                                while (!currentPointNeedInfer)//当图片不检测时
                                {
                                    i--;
                                    if (_needInferOrNotDt != null)
                                    {
                                        if (_needInferOrNotDt.Rows.Count > 0)
                                        {
                                            int elementIndex = Convert.ToInt32(_productParameter[conveyorIndex]._pointPositionDT.Rows[i][5].ToString().Substring(
                                                _productParameter[conveyorIndex]._pointPositionDT.Rows[i][5].ToString().IndexOf("_") + 1,
                                                _productParameter[conveyorIndex]._pointPositionDT.Rows[i][5].ToString().IndexOf("-") - _productParameter[conveyorIndex]._pointPositionDT.Rows[i][5].ToString().IndexOf("_") - 1));
                                            if (_needInferOrNotDt.Rows.Count >= elementIndex)
                                            {
                                                if (_needInferOrNotDt.Rows[elementIndex - 1][1].ToString() == "OK")
                                                    currentPointNeedInfer = true;
                                                else
                                                    currentPointNeedInfer = false;
                                            }
                                            else
                                            {
                                                currentPointNeedInfer = true;
                                            }
                                        }
                                        else
                                        {
                                            if (Convert.ToInt32(_productParameter[conveyorIndex]._pointPositionDT.Rows[i][4]) == 0)
                                            {
                                                currentPointNeedInfer = true;
                                            }
                                            else
                                            {
                                                currentPointNeedInfer = false;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (Convert.ToInt32(_productParameter[conveyorIndex]._pointPositionDT.Rows[i][4]) == 0)
                                        {
                                            currentPointNeedInfer = true;
                                        }
                                        else
                                        {
                                            currentPointNeedInfer = false;
                                        }
                                    }
                                }
                                i--;

                                _grabImageFailedRetryCount++;
                                if (_grabImageFailedRetryCount > 3)//如果三次连续失败，报警
                                {
                                    _stateAndAlarmCs._alarmFlag[360] = true;
                                    while (_stateAndAlarmCs._alarmFlag[360] ||
            (conveyorIndex == 0 && _conveyor1HaveExcessProductAlarmDealWithWayIndex == 1) ||
            (conveyorIndex == 1 && _conveyor2HaveExcessProductAlarmDealWithWayIndex == 1))
                                    {
                                        if (_deadlyAlarmOccurFlag || !_autoModeFlag ||
                               (conveyorIndex == 0 && _conveyor1HaveExcessProductAlarmDealWithWayIndex == 1) ||
                               (conveyorIndex == 1 && _conveyor2HaveExcessProductAlarmDealWithWayIndex == 1))
                                        {
                                            _deviceTotalWorkModuleActionFlag = -1;
                                            return;
                                        }
                                        Thread.Sleep(1);
                                    }
                                }

                                _leftAndRightCameraGrabImageModuleActionFlag = -1;
                                continue;
                            }
                            else if (_leftAndRightCameraGrabImageModuleActionFlag == 1)
                            {
                                _grabImageFailedRetryCount = 0;
                            }
                        }
                    }
                    else
                    {
                        //如果最后一个点为不检测的点（由于for循环到最后一个点之后就会完成循环结束动作，因此最后一个点位需要等到抓取图像彻底完成之后才能结束，同样也要判定抓取图像是否NG)
                        if (i == _productParameter[conveyorIndex]._pointPositionDT.Rows.Count - 1)
                        {
                            //等待相机抓取图像完成
                            while (_leftAndRightCameraGrabImageModuleActionFlag == 0 ||
                                    (conveyorIndex == 0 && _conveyor1HaveExcessProductAlarmDealWithWayIndex == 1) ||
                                    (conveyorIndex == 1 && _conveyor2HaveExcessProductAlarmDealWithWayIndex == 1))
                            {
                                if (_deadlyAlarmOccurFlag || !_autoModeFlag ||
                               (conveyorIndex == 0 && _conveyor1HaveExcessProductAlarmDealWithWayIndex == 1) ||
                               (conveyorIndex == 1 && _conveyor2HaveExcessProductAlarmDealWithWayIndex == 1))
                                {
                                    _deviceTotalWorkModuleActionFlag = -1;
                                    return;
                                }
                                Thread.Sleep(1);
                            }

                            //如果抓取图像失败，那么就回退到前一个要抓取图像的点，和上面的步骤一致
                            if (_leftAndRightCameraGrabImageModuleActionFlag == 2)
                            {
                                i--;
                                if (_needInferOrNotDt != null)
                                {
                                    if (_needInferOrNotDt.Rows.Count > 0)
                                    {
                                        int elementIndex = Convert.ToInt32(_productParameter[conveyorIndex]._pointPositionDT.Rows[i][5].ToString().Substring(
                                            _productParameter[conveyorIndex]._pointPositionDT.Rows[i][5].ToString().IndexOf("_") + 1,
                                            _productParameter[conveyorIndex]._pointPositionDT.Rows[i][5].ToString().IndexOf("-") - _productParameter[conveyorIndex]._pointPositionDT.Rows[i][5].ToString().IndexOf("_") - 1));
                                        if (_needInferOrNotDt.Rows.Count >= elementIndex)
                                        {
                                            if (_needInferOrNotDt.Rows[elementIndex - 1][1].ToString() == "OK")
                                                currentPointNeedInfer = true;
                                            else
                                                currentPointNeedInfer = false;
                                        }
                                        else
                                        {
                                            currentPointNeedInfer = true;
                                        }
                                    }
                                    else
                                    {
                                        if (Convert.ToInt32(_productParameter[conveyorIndex]._pointPositionDT.Rows[i][4]) == 0)
                                        {
                                            currentPointNeedInfer = true;
                                        }
                                        else
                                        {
                                            currentPointNeedInfer = false;
                                        }
                                    }
                                }
                                else
                                {
                                    if (Convert.ToInt32(_productParameter[conveyorIndex]._pointPositionDT.Rows[i][4]) == 0)
                                    {
                                        currentPointNeedInfer = true;
                                    }
                                    else
                                    {
                                        currentPointNeedInfer = false;
                                    }
                                }
                                while (!currentPointNeedInfer)//当图片不检测时
                                {
                                    i--;
                                    if (_needInferOrNotDt != null)
                                    {
                                        if (_needInferOrNotDt.Rows.Count > 0)
                                        {
                                            int elementIndex = Convert.ToInt32(_productParameter[conveyorIndex]._pointPositionDT.Rows[i][5].ToString().Substring(
                                                _productParameter[conveyorIndex]._pointPositionDT.Rows[i][5].ToString().IndexOf("_") + 1,
                                                _productParameter[conveyorIndex]._pointPositionDT.Rows[i][5].ToString().IndexOf("-") - _productParameter[conveyorIndex]._pointPositionDT.Rows[i][5].ToString().IndexOf("_") - 1));
                                            if (_needInferOrNotDt.Rows.Count >= elementIndex)
                                            {
                                                if (_needInferOrNotDt.Rows[elementIndex - 1][1].ToString() == "OK")
                                                    currentPointNeedInfer = true;
                                                else
                                                    currentPointNeedInfer = false;
                                            }
                                            else
                                            {
                                                currentPointNeedInfer = true;
                                            }
                                        }
                                        else
                                        {
                                            if (Convert.ToInt32(_productParameter[conveyorIndex]._pointPositionDT.Rows[i][4]) == 0)
                                            {
                                                currentPointNeedInfer = true;
                                            }
                                            else
                                            {
                                                currentPointNeedInfer = false;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (Convert.ToInt32(_productParameter[conveyorIndex]._pointPositionDT.Rows[i][4]) == 0)
                                        {
                                            currentPointNeedInfer = true;
                                        }
                                        else
                                        {
                                            currentPointNeedInfer = false;
                                        }
                                    }
                                }
                                i--;

                                _grabImageFailedRetryCount++;
                                if (_grabImageFailedRetryCount > 3)//如果三次连续失败，报警
                                {
                                    _stateAndAlarmCs._alarmFlag[360] = true;
                                    while (_stateAndAlarmCs._alarmFlag[360] ||
            (conveyorIndex == 0 && _conveyor1HaveExcessProductAlarmDealWithWayIndex == 1) ||
            (conveyorIndex == 1 && _conveyor2HaveExcessProductAlarmDealWithWayIndex == 1))
                                    {
                                        if (_deadlyAlarmOccurFlag || !_autoModeFlag ||
                               (conveyorIndex == 0 && _conveyor1HaveExcessProductAlarmDealWithWayIndex == 1) ||
                               (conveyorIndex == 1 && _conveyor2HaveExcessProductAlarmDealWithWayIndex == 1))
                                        {
                                            _deviceTotalWorkModuleActionFlag = -1;
                                            return;
                                        }
                                        Thread.Sleep(1);
                                    }
                                }

                                _leftAndRightCameraGrabImageModuleActionFlag = -1;
                                continue;
                            }
                            else if (_leftAndRightCameraGrabImageModuleActionFlag == 1)
                            {
                                _grabImageFailedRetryCount = 0;
                            }
                        }
                        continue;
                    }
                }

                MyTool.TxtFileProcess.CreateLog("设备动作-传送线" + (conveyorIndex + 1).ToString() + "上产品整体动作模块启用完成" + "----用户：" + _operatorName,
                BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

                if (conveyorIndex == 0 && _conveyor1HaveExcessProductAlarmDealWithWayIndex != 1)
                    _conveyor1Status = 104;
                else if (conveyorIndex == 1 && _conveyor2HaveExcessProductAlarmDealWithWayIndex != 1)
                    _conveyor2Status = 104;

                _deviceTotalWorkModuleActionFlag = 1;
            });
        }

        #region 左右相机采集推断相关模块函数

        string _baseSourceImageFolderPath = "";//保存原始图片基础路径
        string _baseInferredOKImageFolderPath = "";//保存推断OK图片的基础路径
        string _baseInferredNGImageFolderPath = "";//保存推断NG图片的基础路径
        string _baseResultCsvFileFolderPath = "";//保存结果文件基础路径
        /// <summary>
        /// LeftAndRightCameraGrabImageModule:左右相机采集图像模块
        /// </summary>
        /// <param name="currentPointNumber">int:当前点位号，第一个点为1号点</param>
        private async void LeftAndRightCameraGrabImageModule(int currentPointNumber, int conveyorIndex)
        {
            await Task.Run(() =>
            {
                _leftAndRightCameraGrabImageModuleActionFlag = 0;

                #region 发现程序漏洞，有出现过不在位置拍照的情况，为了避免这种情况，这里再进行一次判定，如果位置到达才拍照

                float xAxisDestinationPosition_mm = 0, yAxisDestinationPosition_mm = 0, rAxisDestinationPosition_mm = 0;
                lock (this)
                {
                    xAxisDestinationPosition_mm = Convert.ToSingle(_productParameter[conveyorIndex]._pointPositionDT.Rows[currentPointNumber - 1][1]);
                    yAxisDestinationPosition_mm = Convert.ToSingle(_productParameter[conveyorIndex]._pointPositionDT.Rows[currentPointNumber - 1][2]);
                    rAxisDestinationPosition_mm = Convert.ToSingle(_productParameter[conveyorIndex]._pointPositionDT.Rows[currentPointNumber - 1][3]);

                    if (conveyorIndex == 0)
                    {
                        xAxisDestinationPosition_mm += _productParameter[conveyorIndex]._track1X1TeachedCoor -
                            Convert.ToSingle(_productParameter[conveyorIndex]._pointPositionDT.Rows[_productParameter[conveyorIndex]._track1TeachedPointNo - 1][1]);//添加上X偏移值
                        yAxisDestinationPosition_mm += _productParameter[conveyorIndex]._track1Y1TeachedCoor -
                            Convert.ToSingle(_productParameter[conveyorIndex]._pointPositionDT.Rows[_productParameter[conveyorIndex]._track1TeachedPointNo - 1][2]);//添加上Y偏移值
                        rAxisDestinationPosition_mm += _productParameter[conveyorIndex]._track1R1TeachedCoor -
                            Convert.ToSingle(_productParameter[conveyorIndex]._pointPositionDT.Rows[_productParameter[conveyorIndex]._track1TeachedPointNo - 1][3]);//添加上R偏移值
                    }
                    else
                    {
                        xAxisDestinationPosition_mm += _productParameter[conveyorIndex]._track2X1TeachedCoor -
                                Convert.ToSingle(_productParameter[conveyorIndex]._pointPositionDT.Rows[_productParameter[conveyorIndex]._track2TeachedPointNo - 1][1]);//添加上X偏移值
                        yAxisDestinationPosition_mm += _productParameter[conveyorIndex]._track2Y1TeachedCoor -
                            Convert.ToSingle(_productParameter[conveyorIndex]._pointPositionDT.Rows[_productParameter[conveyorIndex]._track2TeachedPointNo - 1][2]);//添加上Y偏移值
                        rAxisDestinationPosition_mm += _productParameter[conveyorIndex]._track2R1TeachedCoor -
                            Convert.ToSingle(_productParameter[conveyorIndex]._pointPositionDT.Rows[_productParameter[conveyorIndex]._track2TeachedPointNo - 1][3]);//添加上R偏移值
                    }

                    if (!SettingsProductForm._useRelativePos)//如果使用相对位置
                    {
                        xAxisDestinationPosition_mm += SettingsAdministratorForm._absoluteCoor[0];//添加上相对位置X坐标
                        yAxisDestinationPosition_mm += SettingsAdministratorForm._absoluteCoor[1];//添加上相对位置Y坐标
                        rAxisDestinationPosition_mm += SettingsAdministratorForm._absoluteCoor[2];//添加上相对位置R坐标
                    }

                    xAxisDestinationPosition_mm = xAxisDestinationPosition_mm + _markSearchXOffsetData;
                    yAxisDestinationPosition_mm = yAxisDestinationPosition_mm + _markSearchYOffsetData;
                }

                while (Math.Abs(_axisControlFormVariate._axisMotionControlBoardVariate._axis00CurrentPosition_mm - xAxisDestinationPosition_mm) > 0.1 ||
                Math.Abs(_axisControlFormVariate._axisMotionControlBoardVariate._axis01CurrentPosition_mm - yAxisDestinationPosition_mm) > 0.1 ||
                Math.Abs(_axisControlFormVariate._axisMotionControlBoardVariate._axis02CurrentPosition_mm - rAxisDestinationPosition_mm) > 0.1 ||
                         _axisControlFormVariate._axisMotionControlBoardVariate._axis00CheckDoneStatus == 0 ||
                         _axisControlFormVariate._axisMotionControlBoardVariate._axis01CheckDoneStatus == 0 ||
                         _axisControlFormVariate._axisMotionControlBoardVariate._axis02CheckDoneStatus == 0 ||
                         _axisControlFormVariate._axisMotionControlBoardVariate._axis00CurrentPositionNumber == 0 ||
                         _axisControlFormVariate._axisMotionControlBoardVariate._axis01CurrentPositionNumber == 0 ||
                         _axisControlFormVariate._axisMotionControlBoardVariate._axis02CurrentPositionNumber == 0 ||
                         !_deviceWorkTriggerFlag ||
                         (conveyorIndex == 0 && _conveyor1HaveExcessProductAlarmDealWithWayIndex == 1) ||
                         (conveyorIndex == 1 && _conveyor2HaveExcessProductAlarmDealWithWayIndex == 1))//等待轴运行结束)
                {
                    if (_deadlyAlarmOccurFlag || !_autoModeFlag ||
                                   (conveyorIndex == 0 && _conveyor1HaveExcessProductAlarmDealWithWayIndex == 1) ||
                                   (conveyorIndex == 1 && _conveyor2HaveExcessProductAlarmDealWithWayIndex == 1))
                    {
                        _leftAndRightCameraGrabImageModuleActionFlag = -1;
                        return;
                    }
                }

                #endregion

                //if (conveyorIndex == 0)
                //    MyTool.TxtFileProcess.CreateLog("设备动作-设备开始采集传送线" + (conveyorIndex + 1).ToString() + "上，产品“"
                //     + SettingsProductForm._track1ActualsProductName + "”的第" + currentPointNumber.ToString() + "个点位的图像" +
                //     "，当前位置" + _axisControlFormVariate._axisMotionControlBoardVariate._axis00CurrentPosition_mm.ToString("f2") + "," + _axisControlFormVariate._axisMotionControlBoardVariate._axis01CurrentPosition_mm.ToString("f2")
                //     + "," + _axisControlFormVariate._axisMotionControlBoardVariate._axis02CurrentPosition_mm.ToString("f2")
                //     + "目标位置：" + _axisControlFormVariate._axisMotionControlBoardVariate.axis00TargetPosition_mm.ToString("f2") + "," + _axisControlFormVariate._axisMotionControlBoardVariate.axis01TargetPosition_mm.ToString("f2")
                //     + "," + _axisControlFormVariate._axisMotionControlBoardVariate.axis02TargetPosition_mm.ToString("f2") + "----用户：" + _operatorName,
                //    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize, false);
                //else
                //    MyTool.TxtFileProcess.CreateLog("设备动作-设备开始采集传送线" + (conveyorIndex + 1).ToString() + "上，产品“"
                //        + SettingsProductForm._track2ActualsProductName + "”的第" + currentPointNumber.ToString() + "个点位的图像" +
                //        "，当前位置" + _axisControlFormVariate._axisMotionControlBoardVariate._axis00CurrentPosition_mm.ToString("f2") + "," + _axisControlFormVariate._axisMotionControlBoardVariate._axis01CurrentPosition_mm.ToString("f2")
                //        + "," + _axisControlFormVariate._axisMotionControlBoardVariate._axis02CurrentPosition_mm.ToString("f2")
                //        + "目标位置：" + _axisControlFormVariate._axisMotionControlBoardVariate.axis00TargetPosition_mm.ToString("f2") + "," + _axisControlFormVariate._axisMotionControlBoardVariate.axis01TargetPosition_mm.ToString("f2")
                //        + "," + _axisControlFormVariate._axisMotionControlBoardVariate.axis02TargetPosition_mm.ToString("f2") + "----用户：" + _operatorName,
                //        BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize, false);

                _triggerGrabImageFinishFlag = false;
                Stopwatch grabIntervelCountSW = new Stopwatch();

                if (_axisControlFormVariate._motionCardInitialStatus)
                {
                    //判定当前产品是否是样品板
                    bool currentPanelIsSampleFlag = false;
                    if ((_barcodeData[conveyorIndex] == _productParameter[conveyorIndex]._sampleProduct1Barcode && _productParameter[conveyorIndex]._sampleProduct1Barcode != "") ||
                          (_barcodeData[conveyorIndex] == _productParameter[conveyorIndex]._sampleProduct2Barcode && _productParameter[conveyorIndex]._sampleProduct2Barcode != "") ||
                        (_barcodeData[conveyorIndex] == _productParameter[conveyorIndex]._sampleProduct3Barcode && _productParameter[conveyorIndex]._sampleProduct3Barcode != ""))
                    {
                        currentPanelIsSampleFlag = true;
                    }

                    //指定文件保存详细路径
                    if (_baseSourceImageFolderPath == "" || _baseInferredOKImageFolderPath == "" || _baseInferredNGImageFolderPath == "" || _baseResultCsvFileFolderPath == "")
                    {
                        string tempTimeStr = System.DateTime.Now.ToString("_yyyyMMdd") + System.DateTime.Now.TimeOfDay.ToString("hhmmssfff");
                        string baseFolder = "";

                        if (!currentPanelIsSampleFlag)
                            baseFolder = SettingsGeneralForm._dataDirectory;
                        else
                            baseFolder = SettingsGeneralForm._samplePanelDirectory;

                        //原始图像保存路径
                        if (conveyorIndex == 0 && _baseSourceImageFolderPath == "")//如果设备工作状态为1号轨道
                        {
                            _baseSourceImageFolderPath = baseFolder + "\\" + SettingsProductForm._track1ActualsProductName + "\\" + "SourceImage" + "\\"
                                + MyTool.FolderAndFileManageClass.FilterPathIllegalChar(_barcodeData[0]) + "_" + SettingsProductForm._track1ActualsProductName + "_" + tempTimeStr + "\\";
                        }
                        else if (conveyorIndex == 1 && _baseSourceImageFolderPath == "")//如果设备工作状态为2号轨道
                        {
                            _baseSourceImageFolderPath = baseFolder + "\\" + SettingsProductForm._track2ActualsProductName + "\\" + "SourceImage" + "\\"
                                + MyTool.FolderAndFileManageClass.FilterPathIllegalChar(_barcodeData[1]) + "_" + SettingsProductForm._track2ActualsProductName + "_" + tempTimeStr + "\\";
                        }

                        //推断OK图片保存路径
                        if (conveyorIndex == 0 && _baseInferredOKImageFolderPath == "")//如果设备工作状态为1号轨道
                        {
                            _baseInferredOKImageFolderPath = baseFolder + "\\" + SettingsProductForm._track1ActualsProductName + "\\" + "InferredOK" + "\\"
                                + MyTool.FolderAndFileManageClass.FilterPathIllegalChar(_barcodeData[0]) + "_" + SettingsProductForm._track1ActualsProductName + "_" + tempTimeStr + "\\";
                        }
                        else if (conveyorIndex == 1 && _baseInferredOKImageFolderPath == "")//如果设备工作状态为2号轨道
                        {
                            _baseInferredOKImageFolderPath = baseFolder + "\\" + SettingsProductForm._track2ActualsProductName + "\\" + "InferredOK" + "\\"
                                + MyTool.FolderAndFileManageClass.FilterPathIllegalChar(_barcodeData[1]) + "_" + SettingsProductForm._track2ActualsProductName + "_" + tempTimeStr + "\\";
                        }

                        //推断NG图片保存路径
                        if (conveyorIndex == 0 && _baseInferredNGImageFolderPath == "")//如果设备工作状态为1号轨道
                        {
                            _baseInferredNGImageFolderPath = baseFolder + "\\" + SettingsProductForm._track1ActualsProductName + "\\" + "InferredNG" + "\\"
                                + MyTool.FolderAndFileManageClass.FilterPathIllegalChar(_barcodeData[0]) + "_" + SettingsProductForm._track1ActualsProductName + "_" + tempTimeStr + "\\";
                        }
                        else if (conveyorIndex == 1 && _baseInferredNGImageFolderPath == "")//如果设备工作状态为2号轨道
                        {
                            _baseInferredNGImageFolderPath = baseFolder + "\\" + SettingsProductForm._track2ActualsProductName + "\\" + "InferredNG" + "\\"
                                + MyTool.FolderAndFileManageClass.FilterPathIllegalChar(_barcodeData[1]) + "_" + SettingsProductForm._track2ActualsProductName + "_" + tempTimeStr + "\\";
                        }

                        //结果CSV文件保存路径
                        if (conveyorIndex == 0 && _baseResultCsvFileFolderPath == "")//如果设备工作状态为1号轨道
                        {
                            _baseResultCsvFileFolderPath = baseFolder + "\\" + SettingsProductForm._track1ActualsProductName + "\\" + "Result" + "\\"
                                + MyTool.FolderAndFileManageClass.FilterPathIllegalChar(_barcodeData[0]) + "_" + SettingsProductForm._track1ActualsProductName + "_" + tempTimeStr + "\\";
                        }
                        else if (conveyorIndex == 1 && _baseResultCsvFileFolderPath == "")//如果设备工作状态为2号轨道
                        {
                            _baseResultCsvFileFolderPath = baseFolder + "\\" + SettingsProductForm._track2ActualsProductName + "\\" + "Result" + "\\"
                                + MyTool.FolderAndFileManageClass.FilterPathIllegalChar(_barcodeData[1]) + "_" + SettingsProductForm._track2ActualsProductName + "_" + tempTimeStr + "\\";
                        }
                    }

                    //右侧相机图像采集
                    if (true)
                    {
                        while (_axisControlFormVariate._ioStatus[1, RIGHT_CAMERA_TRIGGER_OUT_IO] || _axisControlFormVariate._ioStatus[1, RIGHT_CAMERA_TRIGGER_OUT_IO2])
                        {
                            _axisControlFormVariate.OutputControl(RIGHT_CAMERA_TRIGGER_OUT_IO, false);//关闭右侧相机拍照指令
                            _axisControlFormVariate.OutputControl(RIGHT_CAMERA_TRIGGER_OUT_IO2, false);//关闭右侧相机拍照指令

                            if (_deadlyAlarmOccurFlag || !_autoModeFlag ||
                                (conveyorIndex == 0 && _conveyor1HaveExcessProductAlarmDealWithWayIndex == 1) ||
                                (conveyorIndex == 1 && _conveyor2HaveExcessProductAlarmDealWithWayIndex == 1))
                            {
                                //_rightCameraCs.ClearCameraImage();
                                _leftAndRightCameraGrabImageModuleActionFlag = -1;
                                return;
                            }
                            Thread.Sleep(1);
                        }

                        _axisControlFormVariate.OutputControl(RIGHT_CAMERA_TRIGGER_OUT_IO, true);//触发右侧相机拍照指令
                        _axisControlFormVariate.OutputControl(RIGHT_CAMERA_TRIGGER_OUT_IO2, true);//触发右侧相机拍照指令
                        //while (!_axisControlFormVariate._ioStatus[1, RIGHT_CAMERA_TRIGGER_OUT_IO])
                        //{
                        //    if (_deadlyAlarmOccurFlag || !_autoModeFlag)
                        //    {
                        //        return;
                        //    }
                        //    //Thread.Sleep(1);
                        //}
                        //Thread.Sleep(10);


                    }
                    grabIntervelCountSW.Start();
                    while (grabIntervelCountSW.ElapsedMilliseconds < SettingsAdministratorForm._leftAndRightCameraGrabImageInterval)
                    {
                        if (_deadlyAlarmOccurFlag || !_autoModeFlag ||
                                (conveyorIndex == 0 && _conveyor1HaveExcessProductAlarmDealWithWayIndex == 1) ||
                                (conveyorIndex == 1 && _conveyor2HaveExcessProductAlarmDealWithWayIndex == 1))
                        {
                            //_rightCameraCs.ClearCameraImage();
                            _leftAndRightCameraGrabImageModuleActionFlag = -1;
                            return;
                        }
                        // Thread.Sleep(1);
                    }
                    grabIntervelCountSW.Stop();
                    //Thread.Sleep(SettingsAdministratorForm._leftAndRightCameraGrabImageInterval);//等待相机抓取图像延时
                    //左侧相机图像采集
                    if (true)
                    {
                        while (_axisControlFormVariate._ioStatus[1, LEFT_CAMERA_TRIGGER_OUT_IO] || _axisControlFormVariate._ioStatus[1, LEFT_CAMERA_TRIGGER_OUT_IO2])
                        {
                            _axisControlFormVariate.OutputControl(LEFT_CAMERA_TRIGGER_OUT_IO, false);//关闭左侧相机拍照指令
                            _axisControlFormVariate.OutputControl(LEFT_CAMERA_TRIGGER_OUT_IO2, false);//关闭左侧相机拍照指令
                            if (_deadlyAlarmOccurFlag || !_autoModeFlag ||
                                (conveyorIndex == 0 && _conveyor1HaveExcessProductAlarmDealWithWayIndex == 1) ||
                                (conveyorIndex == 1 && _conveyor2HaveExcessProductAlarmDealWithWayIndex == 1))
                            {
                                //_leftCameraCs.ClearCameraImage();
                                _leftAndRightCameraGrabImageModuleActionFlag = -1;
                                return;
                            }
                            Thread.Sleep(1);
                        }

                        _axisControlFormVariate.OutputControl(LEFT_CAMERA_TRIGGER_OUT_IO, true);//触发左侧相机拍照指令
                        _axisControlFormVariate.OutputControl(LEFT_CAMERA_TRIGGER_OUT_IO2, true);//触发左侧相机拍照指令
                        //while (!_axisControlFormVariate._ioStatus[1, LEFT_CAMERA_TRIGGER_OUT_IO])
                        //{
                        //    if (_deadlyAlarmOccurFlag || !_autoModeFlag)
                        //    {
                        //        return;
                        //    }
                        //    Thread.Sleep(1);
                        //}
                        //Thread.Sleep(3);

                    }
                    _triggerGrabImageFinishFlag = true;


                    if (true)//GrabImage
                    {
                        bool rightImageGrabResult = false;
                        bool leftImageGrabResult = false;

                        #region 右相机图像采集
                        if (_rightCameraCs.ho_Image != null)
                        {
                            _rightCameraCs.ho_Image.Dispose();
                            _rightCameraCs.ho_Image = null;
                        }
                        rightImageGrabResult = _rightCameraCs.GrabImage(false);
                        //关闭触发IO
                        _axisControlFormVariate.OutputControl(RIGHT_CAMERA_TRIGGER_OUT_IO, false);
                        _axisControlFormVariate.OutputControl(RIGHT_CAMERA_TRIGGER_OUT_IO2, false);
                        #endregion

                        #region 左相机图像采集
                        if (_leftCameraCs.ho_Image != null)
                        {
                            _leftCameraCs.ho_Image.Dispose();
                            _leftCameraCs.ho_Image = null;
                        }
                        leftImageGrabResult = _leftCameraCs.GrabImage(false);
                        //关闭触发IO
                        _axisControlFormVariate.OutputControl(LEFT_CAMERA_TRIGGER_OUT_IO, false);
                        _axisControlFormVariate.OutputControl(LEFT_CAMERA_TRIGGER_OUT_IO2, false);
                        #endregion

                        #region 右相机采集图像处理

                        if (!rightImageGrabResult)//如果右相机采集失败
                        {
                            //重启相机
                            int retryTimes = 0;
                            bool restartImageFlag = false;
                            while (!restartImageFlag ||
                               (conveyorIndex == 0 && _conveyor1HaveExcessProductAlarmDealWithWayIndex == 1) ||
                               (conveyorIndex == 1 && _conveyor2HaveExcessProductAlarmDealWithWayIndex == 1))
                            {
                                //如果获取图像失败，那么关闭相机
                                MyTool.TxtFileProcess.CreateLog("设备动作-右侧相机抓取图像失败，关闭相机！" + "----用户：" + _operatorName,
                                BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize, true);
                                if (_rightCameraCs.CloseCamera3())//如果关闭成功
                                {
                                    //Thread.Sleep(1000);

                                    restartImageFlag = OpenCamera(ref _rightCameraCs, SettingsAdministratorForm._rightCameraName, true, false);
                                }
                                else
                                {
                                    restartImageFlag = false;
                                }

                                if (_deadlyAlarmOccurFlag || !_autoModeFlag ||
                                   (conveyorIndex == 0 && _conveyor1HaveExcessProductAlarmDealWithWayIndex == 1) ||
                                   (conveyorIndex == 1 && _conveyor2HaveExcessProductAlarmDealWithWayIndex == 1))
                                {
                                    _leftAndRightCameraGrabImageModuleActionFlag = -1;
                                    return;
                                }

                                if (!restartImageFlag)
                                    retryTimes++;

                                if (retryTimes >= 3)//如果尝试重连3次都失败
                                {
                                    MyTool.TxtFileProcess.CreateLog("设备动作-右侧相机抓取图像失败，关闭相机后重启相机失败！" + "----用户：" + _operatorName,
                                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize, true);

                                    _stateAndAlarmCs._alarmFlag[421] = true;//报警
                                    //显示消息提示框
                                    this.Invoke(new Action(() =>
                                    {
                                        if (_informVariate == null)
                                        {
                                            _informVariate = new InfoForm();
                                            _informVariate.ClickEnsureEvent += InformationFormClickEnsureButtonEventFunc;
                                            _informVariate.ClickOutEvent += InformationFormClickOutButtonEventFunc;
                                            _informVariate.ClickTakeOutEvent += InformationFormClickTakeOutButtonEventFunc;
                                            _informVariate.StartPosition = FormStartPosition.CenterScreen;
                                            _informVariate.Focus();
                                            _informVariate.Show();
                                        }
                                        _informVariate.Text = "相机启动失败报警";
                                        _informVariate.txtInfoMessageDisp.Text = _stateAndAlarmCs._alarmMsg[421] + "请检查相机是否正常，并重试!";

                                        _informVariate.btnOK.Visible = true;
                                        _informVariate.btnOut.Visible = false;
                                        _informVariate.btnReleaseAndTakeOut.Visible = false;

                                        _informVariate.btnOK.Text = "确定";
                                        _informclickBtnIndex = -1;
                                    })); ;

                                    while (_stateAndAlarmCs._alarmFlag[421] || _informclickBtnIndex == -1 || !_deviceWorkTriggerFlag ||
                                     (conveyorIndex == 0 && (_conveyor1HaveExcessProductAlarmDealWithWayIndex == 1 || _stateAndAlarmCs._alarmFlag[400])) ||
                                     (conveyorIndex == 1 && (_conveyor2HaveExcessProductAlarmDealWithWayIndex == 1 || _stateAndAlarmCs._alarmFlag[401])))//如果没有点击任何按钮，并且处报警阶段，等待
                                    {
                                        if (_deadlyAlarmOccurFlag || !_autoModeFlag ||
                                            (conveyorIndex == 0 && _conveyor1HaveExcessProductAlarmDealWithWayIndex == 1) ||
                                            (conveyorIndex == 1 && _conveyor2HaveExcessProductAlarmDealWithWayIndex == 1))
                                        {
                                            _leftAndRightCameraGrabImageModuleActionFlag = -1;
                                            return;
                                        }
                                        Thread.Sleep(10);
                                    }

                                    retryTimes = 0;
                                }
                                Thread.Sleep(100);
                            }

                            MyTool.TxtFileProcess.CreateLog("设备动作-右侧相机抓取图像失败，关闭相机后重启相机成功！" + "----用户：" + _operatorName,
                            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize, true);
                        }

                        if (rightImageGrabResult)//如果采集成功，保存图片至内存
                        {
                            bool rightGrabImageSavedFlag = false;
                            while (!rightGrabImageSavedFlag)
                            {
                                for (int i = 0; i < SAVE_IMAGE_QUANTITY_IN_RAM_FOR_RUNNING; i++)
                                {
                                    if (_rightCameraInfer[i]._cameraPictureSavePath == "" && _rightCameraInfer[i].ho_CameraImage == null)
                                    {
                                        _rightCameraInfer[i]._cameraPicturePointName = _productParameter[conveyorIndex]._pointPositionDT.Rows[currentPointNumber - 1][5].ToString();
                                        _rightCameraInfer[i]._cameraPicturePointNumber = currentPointNumber;

                                        _rightCameraInfer[i]._cameraPictureSavedFlag = false;
                                        _rightCameraInfer[i]._cameraPictureInferredFlag = false;

                                        HOperatorSet.CopyImage(_rightCameraCs.ho_Image, out _rightCameraInfer[i].ho_CameraImage);
                                        _rightCameraInfer[i]._cameraPictureSavePath = _baseSourceImageFolderPath
                                            + "R" + _rightCameraInfer[i]._cameraPicturePointName + System.DateTime.Now.ToString("_yyyyMMdd")
                                            + System.DateTime.Now.TimeOfDay.ToString("hhmmssfff");

                                        _rightCameraInfer[i]._cameraInferredResultOKPictureSavePath = _baseInferredOKImageFolderPath +
                                            _rightCameraInfer[i]._cameraPictureSavePath.Substring(_rightCameraInfer[i]._cameraPictureSavePath.LastIndexOf("\\") + 1) + "_Inferred";

                                        _rightCameraInfer[i]._cameraInferredResultNGPictureSavePath = _baseInferredNGImageFolderPath +
                                            _rightCameraInfer[i]._cameraPictureSavePath.Substring(_rightCameraInfer[i]._cameraPictureSavePath.LastIndexOf("\\") + 1) + "_Inferred";

                                        if (conveyorIndex == 0)
                                            _rightCameraInfer[i]._productIndex = 0;
                                        else
                                            _rightCameraInfer[i]._productIndex = 1;

                                        _rightCameraInfer[i]._cameraPictureRequestInferFlag = true;
                                        rightGrabImageSavedFlag = true;
                                        break;
                                    }
                                }
                                if (_deadlyAlarmOccurFlag || !_autoModeFlag ||
                                    (conveyorIndex == 0 && _conveyor1HaveExcessProductAlarmDealWithWayIndex == 1) ||
                                    (conveyorIndex == 1 && _conveyor2HaveExcessProductAlarmDealWithWayIndex == 1))
                                {
                                    //_leftCameraCs.ClearCameraImage();
                                    //_rightCameraCs.ClearCameraImage();
                                    _leftAndRightCameraGrabImageModuleActionFlag = -1;
                                    return;
                                }
                                Thread.Sleep(1);
                            }
                        }

                        #endregion

                        #region 左相机采集图像处理

                        if (!leftImageGrabResult)//如果左相机采集失败，重启相机
                        {
                            //重新打开相机
                            int retryTimes = 0;
                            bool restartImageFlag = false;

                            while (!restartImageFlag ||
                               (conveyorIndex == 0 && _conveyor1HaveExcessProductAlarmDealWithWayIndex == 1) ||
                               (conveyorIndex == 1 && _conveyor2HaveExcessProductAlarmDealWithWayIndex == 1))
                            {
                                //如果获取图像失败，那么关闭相机
                                MyTool.TxtFileProcess.CreateLog("设备动作-左侧相机抓取图像失败，关闭相机！" + "----用户：" + _operatorName,
                                BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize, true);
                                if (_leftCameraCs.CloseCamera3())//如果关闭成功
                                {
                                    //Thread.Sleep(1000);

                                    restartImageFlag = OpenCamera(ref _leftCameraCs, SettingsAdministratorForm._leftCameraName, true, false);
                                }
                                else
                                {
                                    restartImageFlag = false;
                                }

                                if (_deadlyAlarmOccurFlag || !_autoModeFlag ||
                                   (conveyorIndex == 0 && _conveyor1HaveExcessProductAlarmDealWithWayIndex == 1) ||
                                   (conveyorIndex == 1 && _conveyor2HaveExcessProductAlarmDealWithWayIndex == 1))
                                {
                                    _leftAndRightCameraGrabImageModuleActionFlag = -1;
                                    return;
                                }

                                if (!restartImageFlag)
                                    retryTimes++;

                                if (retryTimes >= 3)//如果尝试重连3次都失败
                                {
                                    MyTool.TxtFileProcess.CreateLog("设备动作-左侧相机抓取图像失败，关闭相机后重启相机失败！" + "----用户：" + _operatorName,
                                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize, true);
                                    _stateAndAlarmCs._alarmFlag[420] = true;//报警
                                    //显示消息提示框
                                    this.Invoke(new Action(() =>
                                    {
                                        if (_informVariate == null)
                                        {
                                            _informVariate = new InfoForm();
                                            _informVariate.ClickEnsureEvent += InformationFormClickEnsureButtonEventFunc;
                                            _informVariate.ClickOutEvent += InformationFormClickOutButtonEventFunc;
                                            _informVariate.ClickTakeOutEvent += InformationFormClickTakeOutButtonEventFunc;
                                            _informVariate.StartPosition = FormStartPosition.CenterScreen;
                                            _informVariate.Focus();
                                            _informVariate.Show();
                                        }
                                        _informVariate.Text = "相机启动失败报警";
                                        _informVariate.txtInfoMessageDisp.Text = _stateAndAlarmCs._alarmMsg[420] + "请检查相机是否正常，并重试!";

                                        _informVariate.btnOK.Visible = true;
                                        _informVariate.btnOut.Visible = false;
                                        _informVariate.btnReleaseAndTakeOut.Visible = false;

                                        _informVariate.btnOK.Text = "确定";
                                        _informclickBtnIndex = -1;
                                    })); ;

                                    while (_stateAndAlarmCs._alarmFlag[420] || _informclickBtnIndex == -1 || !_deviceWorkTriggerFlag ||
                                     (conveyorIndex == 0 && (_conveyor1HaveExcessProductAlarmDealWithWayIndex == 1 || _stateAndAlarmCs._alarmFlag[400])) ||
                                     (conveyorIndex == 1 && (_conveyor2HaveExcessProductAlarmDealWithWayIndex == 1 || _stateAndAlarmCs._alarmFlag[401])))//如果没有点击任何按钮，并且处报警阶段，等待
                                    {
                                        if (_deadlyAlarmOccurFlag || !_autoModeFlag ||
                                            (conveyorIndex == 0 && _conveyor1HaveExcessProductAlarmDealWithWayIndex == 1) ||
                                            (conveyorIndex == 1 && _conveyor2HaveExcessProductAlarmDealWithWayIndex == 1))
                                        {
                                            _leftAndRightCameraGrabImageModuleActionFlag = -1;
                                            return;
                                        }
                                        Thread.Sleep(10);
                                    }

                                    retryTimes = 0;
                                }
                                Thread.Sleep(100);
                            }

                            MyTool.TxtFileProcess.CreateLog("设备动作-左侧相机抓取图像失败，关闭相机后重启相机成功！" + "----用户：" + _operatorName,
                            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize, true);
                        }

                        if (leftImageGrabResult)
                        {
                            bool leftGrabImageSavedFlag = false;
                            while (!leftGrabImageSavedFlag)
                            {
                                for (int i = 0; i < SAVE_IMAGE_QUANTITY_IN_RAM_FOR_RUNNING; i++)
                                {
                                    if (_leftCameraInfer[i]._cameraPictureSavePath == "" && _leftCameraInfer[i].ho_CameraImage == null)
                                    {
                                        _leftCameraInfer[i]._cameraPicturePointName = _productParameter[conveyorIndex]._pointPositionDT.Rows[currentPointNumber - 1][5].ToString();
                                        _leftCameraInfer[i]._cameraPicturePointNumber = currentPointNumber;

                                        _leftCameraInfer[i]._cameraPictureSavedFlag = false;
                                        _leftCameraInfer[i]._cameraPictureInferredFlag = false;

                                        HOperatorSet.CopyImage(_leftCameraCs.ho_Image, out _leftCameraInfer[i].ho_CameraImage);

                                        _leftCameraInfer[i]._cameraPictureSavePath = _baseSourceImageFolderPath
                                            + "L" + _leftCameraInfer[i]._cameraPicturePointName + System.DateTime.Now.ToString("_yyyyMMdd")
                                            + System.DateTime.Now.TimeOfDay.ToString("hhmmssfff");

                                        _leftCameraInfer[i]._cameraInferredResultOKPictureSavePath = _baseInferredOKImageFolderPath +
                                            _leftCameraInfer[i]._cameraPictureSavePath.Substring(_leftCameraInfer[i]._cameraPictureSavePath.LastIndexOf("\\") + 1) + "_Inferred";

                                        _leftCameraInfer[i]._cameraInferredResultNGPictureSavePath = _baseInferredNGImageFolderPath +
                                            _leftCameraInfer[i]._cameraPictureSavePath.Substring(_leftCameraInfer[i]._cameraPictureSavePath.LastIndexOf("\\") + 1) + "_Inferred";

                                        if (conveyorIndex == 0)
                                            _leftCameraInfer[i]._productIndex = 0;
                                        else
                                            _leftCameraInfer[i]._productIndex = 1;

                                        _leftCameraInfer[i]._cameraPictureRequestInferFlag = true;
                                        leftGrabImageSavedFlag = true;
                                        break;
                                    }
                                }
                                if (_deadlyAlarmOccurFlag || !_autoModeFlag ||
                                    (conveyorIndex == 0 && _conveyor1HaveExcessProductAlarmDealWithWayIndex == 1) ||
                                    (conveyorIndex == 1 && _conveyor2HaveExcessProductAlarmDealWithWayIndex == 1))
                                {
                                    _leftAndRightCameraGrabImageModuleActionFlag = -1;
                                    return;
                                }
                                Thread.Sleep(1);
                            }
                        }

                        #endregion

                        if (!leftImageGrabResult || !rightImageGrabResult)
                        {
                            _leftAndRightCameraGrabImageModuleActionFlag = 2;
                            return;
                        }
                    }

                }

                //if (conveyorIndex == 0)
                //    MyTool.TxtFileProcess.CreateLog("设备动作-设备开始采集传送线" + (conveyorIndex + 1).ToString() + "上，产品“"
                //    + SettingsProductForm._track1ActualsProductName + "”的第" + currentPointNumber.ToString() + "个点位的图像完成" +
                //    "，当前位置" + _axisControlFormVariate._axisMotionControlBoardVariate._axis00CurrentPosition_mm.ToString("f2") + "," + _axisControlFormVariate._axisMotionControlBoardVariate._axis01CurrentPosition_mm.ToString("f2")
                //     + "," + _axisControlFormVariate._axisMotionControlBoardVariate._axis02CurrentPosition_mm.ToString("f2")
                //     + "目标位置：" + _axisControlFormVariate._axisMotionControlBoardVariate.axis00TargetPosition_mm.ToString("f2") + "," + _axisControlFormVariate._axisMotionControlBoardVariate.axis01TargetPosition_mm.ToString("f2")
                //     + "," + _axisControlFormVariate._axisMotionControlBoardVariate.axis02TargetPosition_mm.ToString("f2") + "----用户：" + _operatorName,
                //    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize, false);
                //else
                //    MyTool.TxtFileProcess.CreateLog("设备动作-设备开始采集传送线" + (conveyorIndex + 1).ToString() + "上，产品“"
                //    + SettingsProductForm._track2ActualsProductName + "”的第" + currentPointNumber.ToString() + "个点位的图像完成" +
                //    "，当前位置" + _axisControlFormVariate._axisMotionControlBoardVariate._axis00CurrentPosition_mm.ToString("f2") + "," + _axisControlFormVariate._axisMotionControlBoardVariate._axis01CurrentPosition_mm.ToString("f2")
                //     + "," + _axisControlFormVariate._axisMotionControlBoardVariate._axis02CurrentPosition_mm.ToString("f2")
                //     + "目标位置：" + _axisControlFormVariate._axisMotionControlBoardVariate.axis00TargetPosition_mm.ToString("f2") + "," + _axisControlFormVariate._axisMotionControlBoardVariate.axis01TargetPosition_mm.ToString("f2")
                //     + "," + _axisControlFormVariate._axisMotionControlBoardVariate.axis02TargetPosition_mm.ToString("f2") + "----用户：" + _operatorName,
                //    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize, false);

                _leftAndRightCameraGrabImageModuleActionFlag = 1;
            });
        }

        /// <summary>
        /// LeftCameraImageInferModule：左侧相机采集图像推断模块
        /// </summary>
        /// 
        private async void LeftCameraImageInferModule()
        {
            await Task.Run(() =>
            {
                _leftCameraImageInferModuleActionFlag = 0;
                for (int i = 0; i < SAVE_IMAGE_QUANTITY_IN_RAM_FOR_RUNNING; i++)
                {
                    if (_leftCameraInfer[i]._cameraPictureRequestInferFlag && _leftCameraInfer[i].ho_CameraImage != null)
                    {
                        //infer左侧图片
                        _leftCameraInfer[i]._cameraPictureRequestInferFlag = false;
                        string preprocessFileFolderPath = _productParameter[_leftCameraInfer[i]._productIndex]._preprocessParameterFilePath.Substring(0, _productParameter[_leftCameraInfer[i]._productIndex]._preprocessParameterFilePath.LastIndexOf("\\"));
                        string preprocessFileName = _productParameter[_leftCameraInfer[i]._productIndex]._preprocessParameterFilePath.Substring(_productParameter[_leftCameraInfer[i]._productIndex]._preprocessParameterFilePath.LastIndexOf("\\") + 1);
                        string dimensionFilePath = _productParameter[_leftCameraInfer[i]._productIndex]._dimensionFilePath;
                        string modelFilePath = _productParameter[_leftCameraInfer[i]._productIndex]._modelFilePath.Substring(0, _productParameter[_leftCameraInfer[i]._productIndex]._modelFilePath.LastIndexOf("."));

                        _newLeftDeepLearningObjectDetectionRectangle1Infer.InferAction(preprocessFileFolderPath, preprocessFileName, dimensionFilePath, modelFilePath,
                             1, true, _leftCameraInfer[i].ho_CameraImage, _leftInferNeedReadParameter, false);

                        if (_leftInferNeedReadParameter)
                            _leftInferNeedReadParameter = false;

                        HalconCameraControl.DispImageAdaptively(_mainFormVariate.picLeftCameraResultImageDisp, _leftCameraInfer[i].ho_CameraImage, false);

                        string resultStr = "";
                        if (!SettingsAdministratorForm._checkProductIsConnector)//如果检测产品为switch
                        {
                            //筛选出infer找到的所有结果中confidence最大的那一个
                            int maxCofidenceIndex = 0;
                            float maxCofidence = 0f;
                            try
                            {
                                maxCofidence = _newLeftDeepLearningObjectDetectionRectangle1Infer.inferResult.confidence[maxCofidenceIndex].F;
                                //for (int j = 0; j < _newLeftDeepLearningObjectDetectionRectangle1Infer.inferResult.class_id.TupleLength(); j++)
                                //{
                                //    if (maxCofidence < _newLeftDeepLearningObjectDetectionRectangle1Infer.inferResult.confidence[j])
                                //    {
                                //        maxCofidenceIndex = j;
                                //        maxCofidence = _newLeftDeepLearningObjectDetectionRectangle1Infer.inferResult.confidence[j];
                                //    }
                                //}
                            }
                            catch (Exception e)
                            {
                                MessageBox.Show(e.ToString());
                            }

                            //判定结果
                            int classNamesIndex = 0;
                            try
                            {
                                if (_newLeftDeepLearningObjectDetectionRectangle1Infer.inferResult.ho_Rectangle != null)//如果有找到产品并且只有一个
                                {
                                    if (_newLeftDeepLearningObjectDetectionRectangle1Infer.inferResult.ho_Rectangle.CountObj() == 1)
                                    {
                                        classNamesIndex = (int)_newLeftDeepLearningObjectDetectionRectangle1Infer.inferResult.class_id[maxCofidenceIndex];
                                        resultStr = _newLeftDeepLearningObjectDetectionRectangle1Infer.inferResult.classNames[classNamesIndex];
                                        if (resultStr.Substring(0, 2) == "OK")
                                            resultStr = _newLeftDeepLearningObjectDetectionRectangle1Infer.inferResult.classNames[classNamesIndex] + ": " + maxCofidence.ToString();
                                        //resultStr = (classNamesIndex - 1).ToString("00") + ": "
                                        //+ _newLeftDeepLearningObjectDetectionRectangle1Infer.inferResult.classNames[classNamesIndex] + ": " + maxCofidence.ToString();
                                        else
                                            resultStr = "NG: NG";// resultStr.Substring(0, 2) + ": " + maxCofidence.ToString();

                                        HOperatorSet.SetDraw(_mainFormVariate.picLeftCameraResultImageDisp.HalconWindow, "margin");
                                        if (classNamesIndex == 2 && (maxCofidence >= SettingsAdministratorForm._passThresholdValue ||
                                            !_baseSettingFormVariate._settingsAdminFormVariate.chkPassThresholdUsing.Checked))
                                            HOperatorSet.SetColor(_mainFormVariate.picLeftCameraResultImageDisp.HalconWindow, "green");
                                        else if (classNamesIndex == 2)
                                        {
                                            resultStr = "NG" + ": " + maxCofidence.ToString();// +resultStr.Substring(resultStr.IndexOf(":"));
                                            classNamesIndex = 0;
                                            HOperatorSet.SetColor(_mainFormVariate.picLeftCameraResultImageDisp.HalconWindow, "red");
                                        }
                                        else
                                        {
                                            HOperatorSet.SetColor(_mainFormVariate.picLeftCameraResultImageDisp.HalconWindow, "red");
                                        }
                                        HOperatorSet.DispObj(_newLeftDeepLearningObjectDetectionRectangle1Infer.inferResult.ho_Rectangle[maxCofidenceIndex + 1], _mainFormVariate.picLeftCameraResultImageDisp.HalconWindow);
                                    }
                                    else
                                    {
                                        resultStr = "NG: NG2";
                                        HOperatorSet.SetColor(_mainFormVariate.picLeftCameraResultImageDisp.HalconWindow, "red");
                                    }
                                }
                                else
                                {
                                    resultStr = "NG: NG";
                                    HOperatorSet.SetColor(_mainFormVariate.picLeftCameraResultImageDisp.HalconWindow, "red");
                                }

                            }
                            catch (Exception e)
                            {
                                MessageBox.Show(e.Message + _leftCameraInfer[i]._cameraPicturePointNumber.ToString());
                            }
                        }
                        else//如果检测产品为Connector
                        {
                            int[] inferKindQuantity = new int[5]{0,0,0,0,0};//推断结果种类个数，用于判定是否OK，索引号代表的是种类1-5
                            //设置显示为框
                            HOperatorSet.SetDraw(_mainFormVariate.picLeftCameraResultImageDisp.HalconWindow, "margin");
                            try
                            {
                                for (int j = 0; j < _newLeftDeepLearningObjectDetectionRectangle1Infer.inferResult.class_id.TupleLength(); j++)//检查所有类
                                {
                                    int inferKind = (int)_newLeftDeepLearningObjectDetectionRectangle1Infer.inferResult.class_id[j];//获取当前种类
                                    string rectangleClr = "red";//设置显示rectangle的颜色
                                    if (inferKind == _productParameter[_leftCameraInfer[i]._productIndex]._inferResultKind1Number - 1 ||
                                        inferKind == _productParameter[_leftCameraInfer[i]._productIndex]._inferResultKind2Number - 1 ||
                                        inferKind == _productParameter[_leftCameraInfer[i]._productIndex]._inferResultKind3Number - 1 ||
                                        inferKind == _productParameter[_leftCameraInfer[i]._productIndex]._inferResultKind4Number - 1 ||
                                        inferKind == _productParameter[_leftCameraInfer[i]._productIndex]._inferResultKind5Number - 1)//如果当前的种类号与设定的种类号对应上了
                                    {
                                        if (_newLeftDeepLearningObjectDetectionRectangle1Infer.inferResult.confidence[j] >= SettingsAdministratorForm._passThresholdValue ||
                                            !_baseSettingFormVariate._settingsAdminFormVariate.chkPassThresholdUsing.Checked)//如果启用了PASS阈值功能，那么如果大于设定阈值或者没有启用PASS阈值功能，那么算是推断OK
                                        {
                                            if (inferKind == _productParameter[_leftCameraInfer[i]._productIndex]._inferResultKind1Number - 1)
                                                inferKindQuantity[0]++;
                                            else if (inferKind == _productParameter[_leftCameraInfer[i]._productIndex]._inferResultKind2Number - 1)
                                                inferKindQuantity[1]++;
                                            else if (inferKind == _productParameter[_leftCameraInfer[i]._productIndex]._inferResultKind3Number - 1)
                                                inferKindQuantity[2]++;
                                            else if (inferKind == _productParameter[_leftCameraInfer[i]._productIndex]._inferResultKind4Number - 1)
                                                inferKindQuantity[3]++;
                                            else if (inferKind == _productParameter[_leftCameraInfer[i]._productIndex]._inferResultKind5Number - 1)
                                                inferKindQuantity[4]++;
                                            rectangleClr = "green";//设置显示rectangle的颜色
                                        }
                                    }

                                    //设置显示框的颜色
                                    HOperatorSet.SetColor(_mainFormVariate.picLeftCameraResultImageDisp.HalconWindow, rectangleClr);
                                    //显示框
                                    HOperatorSet.DispObj(_newLeftDeepLearningObjectDetectionRectangle1Infer.inferResult.ho_Rectangle[j + 1], _mainFormVariate.picLeftCameraResultImageDisp.HalconWindow);//在halcon中，object的计数是从1开始的，所以索引号要+1

                                }

                                //获取需要检测到的种类总数
                                int needCheckKindTotalQuantity = 0;
                                if (_productParameter[_leftCameraInfer[i]._productIndex]._inferResultKind1Number != 0)//为零不计入数据
                                {
                                    needCheckKindTotalQuantity += _productParameter[_leftCameraInfer[i]._productIndex]._inferResultKind1Quantity;
                                }
                                if (_productParameter[_leftCameraInfer[i]._productIndex]._inferResultKind2Number != 0)//为零不计入数据
                                {
                                    needCheckKindTotalQuantity += _productParameter[_leftCameraInfer[i]._productIndex]._inferResultKind2Quantity;
                                }
                                if (_productParameter[_leftCameraInfer[i]._productIndex]._inferResultKind3Number != 0)//为零不计入数据
                                {
                                    needCheckKindTotalQuantity += _productParameter[_leftCameraInfer[i]._productIndex]._inferResultKind3Quantity;
                                }
                                if (_productParameter[_leftCameraInfer[i]._productIndex]._inferResultKind4Number != 0)//为零不计入数据
                                {
                                    needCheckKindTotalQuantity += _productParameter[_leftCameraInfer[i]._productIndex]._inferResultKind4Quantity;
                                }
                                if (_productParameter[_leftCameraInfer[i]._productIndex]._inferResultKind5Number != 0)//为零不计入数据
                                {
                                    needCheckKindTotalQuantity += _productParameter[_leftCameraInfer[i]._productIndex]._inferResultKind5Quantity;
                                }

                                //判定结果是否OK
                                if (needCheckKindTotalQuantity != _newLeftDeepLearningObjectDetectionRectangle1Infer.inferResult.class_id.TupleLength())//如果找到的区域个数不为需要的个数
                                {
                                    resultStr = "NG: NG";
                                }
                                else//如果总数对应的OK
                                {
                                    if ((_productParameter[_leftCameraInfer[i]._productIndex]._inferResultKind1Number != 0 && _productParameter[_leftCameraInfer[i]._productIndex]._inferResultKind1Quantity != inferKindQuantity[0]) ||
                                       (_productParameter[_leftCameraInfer[i]._productIndex]._inferResultKind2Number != 0 && _productParameter[_leftCameraInfer[i]._productIndex]._inferResultKind2Quantity != inferKindQuantity[1]) ||
                                       (_productParameter[_leftCameraInfer[i]._productIndex]._inferResultKind3Number != 0 && _productParameter[_leftCameraInfer[i]._productIndex]._inferResultKind3Quantity != inferKindQuantity[2]) ||
                                       (_productParameter[_leftCameraInfer[i]._productIndex]._inferResultKind4Number != 0 && _productParameter[_leftCameraInfer[i]._productIndex]._inferResultKind4Quantity != inferKindQuantity[3]) ||
                                       (_productParameter[_leftCameraInfer[i]._productIndex]._inferResultKind5Number != 0 && _productParameter[_leftCameraInfer[i]._productIndex]._inferResultKind5Quantity != inferKindQuantity[4]))//如果种类没有设置为0，即有效的话
                                        resultStr = "NG: NG";
                                    else
                                        resultStr = "OK: OK";
                                }
                            }
                            catch
                            {
                                resultStr = "NG: NG";
                            }
                        }

                        _leftInferResultDt[_leftCameraInfer[i]._productIndex].Rows[_leftCameraInfer[i]._cameraPicturePointNumber - 1][2] = resultStr;

                        if (_leftCameraInfer[i]._productIndex == 0)//如果为传送线1上的产品
                        {
                            lock (_conveyor1InferResultDic)
                            {
                                if (_conveyor1InferResultDic.Count > 0)
                                {
                                    bool projectExistFlag = false;//用于标志要存储数据的项是否存在，false-不存在，true-存在
                                    int existIndex = -1;
                                    for (int j = 0; j < _conveyor1InferResultDic.Count; j++)
                                    {
                                        if (_conveyor1InferResultDic[j]._elementName == _leftCameraInfer[i]._cameraPicturePointName)//如果同名项存在
                                        {
                                            projectExistFlag = true;
                                            existIndex = j;
                                            break;
                                        }
                                    }
                                    if (projectExistFlag)
                                    {
                                        //判定当前产品是否是样品板
                                        bool currentPanelIsSampleFlag = false;
                                        int conveyorIndex = 0;
                                        if (_conveyor2Status == 103 || _conveyor2Status == 104)
                                            conveyorIndex = 1;
                                        if ((_barcodeData[conveyorIndex] == _productParameter[conveyorIndex]._sampleProduct1Barcode && _productParameter[conveyorIndex]._sampleProduct1Barcode != "") ||
                                                  (_barcodeData[conveyorIndex] == _productParameter[conveyorIndex]._sampleProduct2Barcode && _productParameter[conveyorIndex]._sampleProduct2Barcode != "") ||
                                                (_barcodeData[conveyorIndex] == _productParameter[conveyorIndex]._sampleProduct3Barcode && _productParameter[conveyorIndex]._sampleProduct3Barcode != ""))
                                            currentPanelIsSampleFlag = true;

                                        InferResult tempInferResult = new InferResult();
                                        tempInferResult = _conveyor1InferResultDic[existIndex];
                                        tempInferResult._elementLeftResult = resultStr.Substring(0, 2);
                                        if (tempInferResult._elementRightResult.Length >= 2 && tempInferResult._elementLeftResult.Length >= 2)
                                        {
                                            lock (_yieldRecordDt)
                                            {
                                                if ((SettingsGeneralForm._addSampleDataToYieldFlag && currentPanelIsSampleFlag) || !currentPanelIsSampleFlag)
                                                {
                                                    _yieldRecordDt.Rows[8][1] = (Convert.ToInt32(_yieldRecordDt.Rows[8][1].ToString()) + 1).ToString();
                                                    _yieldRecordDt.Rows[8][2] = (Convert.ToInt32(_yieldRecordDt.Rows[8][2].ToString()) + 1).ToString();
                                                }
                                                if (tempInferResult._elementRightResult.Substring(0, 2) == "OK" && tempInferResult._elementLeftResult.Substring(0, 2) == "OK")
                                                {
                                                    tempInferResult._elementTotalResult = "PASS";
                                                    if ((SettingsGeneralForm._addSampleDataToYieldFlag && currentPanelIsSampleFlag) || !currentPanelIsSampleFlag)
                                                    {
                                                        _yieldRecordDt.Rows[9][1] = (Convert.ToInt32(_yieldRecordDt.Rows[9][1].ToString()) + 1).ToString();
                                                        _yieldRecordDt.Rows[9][2] = (Convert.ToInt32(_yieldRecordDt.Rows[9][2].ToString()) + 1).ToString();
                                                    }
                                                }
                                                else
                                                {
                                                    tempInferResult._elementTotalResult = "FAIL";
                                                    if ((SettingsGeneralForm._addSampleDataToYieldFlag && currentPanelIsSampleFlag) || !currentPanelIsSampleFlag)
                                                    {
                                                        _yieldRecordDt.Rows[10][1] = (Convert.ToInt32(_yieldRecordDt.Rows[10][1].ToString()) + 1).ToString();
                                                        _yieldRecordDt.Rows[10][2] = (Convert.ToInt32(_yieldRecordDt.Rows[10][2].ToString()) + 1).ToString();
                                                    }
                                                }
                                            }
                                        }
                                        _conveyor1InferResultDic[existIndex] = tempInferResult;
                                    }
                                    else
                                    {
                                        InferResult tempInferResult = new InferResult();
                                        tempInferResult._elementNumber = (_conveyor1InferResultDic.Count + 1).ToString();
                                        tempInferResult._elementName = _leftCameraInfer[i]._cameraPicturePointName;
                                        tempInferResult._elementLeftResult = resultStr.Substring(0, 2);
                                        tempInferResult._elementRightResult = "";
                                        tempInferResult._elementTotalResult = "";
                                        _conveyor1InferResultDic.Add(_conveyor1InferResultDic.Count, tempInferResult);
                                    }
                                }
                                else
                                {
                                    InferResult tempInferResult = new InferResult();
                                    tempInferResult._elementNumber = (_conveyor1InferResultDic.Count + 1).ToString();
                                    tempInferResult._elementName = _leftCameraInfer[i]._cameraPicturePointName;
                                    tempInferResult._elementLeftResult = resultStr.Substring(0, 2);
                                    tempInferResult._elementRightResult = "";
                                    tempInferResult._elementTotalResult = "";
                                    _conveyor1InferResultDic.Add(_conveyor1InferResultDic.Count, tempInferResult);
                                }
                            }
                        }
                        else//如果为传送线2上的产品
                        {
                            lock (_conveyor2InferResultDic)
                            {
                                if (_conveyor2InferResultDic.Count > 0)
                                {
                                    bool projectExistFlag = false;//用于标志要存储数据的项是否存在，false-不存在，true-存在
                                    int existIndex = -1;
                                    for (int j = 0; j < _conveyor2InferResultDic.Count; j++)
                                    {
                                        if (_conveyor2InferResultDic[j]._elementName == _leftCameraInfer[i]._cameraPicturePointName)//如果同名项存在
                                        {
                                            projectExistFlag = true;
                                            existIndex = j;
                                            break;
                                        }
                                    }
                                    if (projectExistFlag)
                                    {
                                        //判定当前产品是否是样品板
                                        bool currentPanelIsSampleFlag = false;
                                        int conveyorIndex = 0;
                                        if (_conveyor2Status == 103 || _conveyor2Status == 104)
                                            conveyorIndex = 1;
                                        if ((_barcodeData[conveyorIndex] == _productParameter[conveyorIndex]._sampleProduct1Barcode && _productParameter[conveyorIndex]._sampleProduct1Barcode != "") ||
                                                  (_barcodeData[conveyorIndex] == _productParameter[conveyorIndex]._sampleProduct2Barcode && _productParameter[conveyorIndex]._sampleProduct2Barcode != "") ||
                                                (_barcodeData[conveyorIndex] == _productParameter[conveyorIndex]._sampleProduct3Barcode && _productParameter[conveyorIndex]._sampleProduct3Barcode != ""))
                                            currentPanelIsSampleFlag = true;

                                        InferResult tempInferResult = new InferResult();
                                        tempInferResult = _conveyor2InferResultDic[existIndex];
                                        tempInferResult._elementLeftResult = resultStr.Substring(0, 2);
                                        if (tempInferResult._elementRightResult.Length >= 2 && tempInferResult._elementLeftResult.Length >= 2)
                                        {
                                            lock (_yieldRecordDt)
                                            {
                                                if ((SettingsGeneralForm._addSampleDataToYieldFlag && currentPanelIsSampleFlag) || !currentPanelIsSampleFlag)
                                                {
                                                    _yieldRecordDt.Rows[8][1] = (Convert.ToInt32(_yieldRecordDt.Rows[8][1].ToString()) + 1).ToString();
                                                    _yieldRecordDt.Rows[8][3] = (Convert.ToInt32(_yieldRecordDt.Rows[8][3].ToString()) + 1).ToString();
                                                }
                                                if (tempInferResult._elementRightResult.Substring(0, 2) == "OK" && tempInferResult._elementLeftResult.Substring(0, 2) == "OK")
                                                {
                                                    tempInferResult._elementTotalResult = "PASS";
                                                    if ((SettingsGeneralForm._addSampleDataToYieldFlag && currentPanelIsSampleFlag) || !currentPanelIsSampleFlag)
                                                    {
                                                        _yieldRecordDt.Rows[9][1] = (Convert.ToInt32(_yieldRecordDt.Rows[9][1].ToString()) + 1).ToString();
                                                        _yieldRecordDt.Rows[9][3] = (Convert.ToInt32(_yieldRecordDt.Rows[9][3].ToString()) + 1).ToString();
                                                    }
                                                }
                                                else
                                                {
                                                    tempInferResult._elementTotalResult = "FAIL";
                                                    if ((SettingsGeneralForm._addSampleDataToYieldFlag && currentPanelIsSampleFlag) || !currentPanelIsSampleFlag)
                                                    {
                                                        _yieldRecordDt.Rows[10][1] = (Convert.ToInt32(_yieldRecordDt.Rows[10][1].ToString()) + 1).ToString();
                                                        _yieldRecordDt.Rows[10][3] = (Convert.ToInt32(_yieldRecordDt.Rows[10][3].ToString()) + 1).ToString();
                                                    }
                                                }
                                            }
                                        }
                                        _conveyor2InferResultDic[existIndex] = tempInferResult;
                                    }
                                    else
                                    {
                                        InferResult tempInferResult = new InferResult();
                                        tempInferResult._elementNumber = (_conveyor2InferResultDic.Count + 1).ToString();
                                        tempInferResult._elementName = _leftCameraInfer[i]._cameraPicturePointName;
                                        tempInferResult._elementLeftResult = resultStr.Substring(0, 2);
                                        tempInferResult._elementRightResult = "";
                                        tempInferResult._elementTotalResult = "";
                                        _conveyor2InferResultDic.Add(_conveyor2InferResultDic.Count, tempInferResult);
                                    }
                                }
                                else
                                {
                                    InferResult tempInferResult = new InferResult();
                                    tempInferResult._elementNumber = (_conveyor2InferResultDic.Count + 1).ToString();
                                    tempInferResult._elementName = _leftCameraInfer[i]._cameraPicturePointName;
                                    tempInferResult._elementLeftResult = resultStr.Substring(0, 2);
                                    tempInferResult._elementRightResult = "";
                                    tempInferResult._elementTotalResult = "";
                                    _conveyor2InferResultDic.Add(_conveyor2InferResultDic.Count, tempInferResult);
                                }
                            }
                        }

                        //显示文字
                        HalconCameraControl.SetDisplayFont(_mainFormVariate.picLeftCameraResultImageDisp.HalconWindow, 16, "mono", "false", "false");
                        if (resultStr.Substring(0, 2) == "OK")
                        {
                            HOperatorSet.DispText(_mainFormVariate.picLeftCameraResultImageDisp.HalconWindow, "L" + _leftCameraInfer[i]._cameraPicturePointName + ": " + resultStr.Substring(0, 2),
                                "image", 12, 12, "green", "box", "false");
                        }
                        else
                        {
                            resultStr = "NG: NG";
                            HOperatorSet.DispText(_mainFormVariate.picLeftCameraResultImageDisp.HalconWindow, "L" + _leftCameraInfer[i]._cameraPicturePointName + ": " + resultStr.Substring(0, 2),
                            "image", 12, 12, "red", "box", "false");
                        }

                        _leftCameraInfer[i]._cameraPictureInferredFlag = true;

                        //将要保存的Inferred结果图片保存至对应的Array中
                        bool saveResultPicArrayHaveSpaceFlag = false;
                        while (!saveResultPicArrayHaveSpaceFlag)
                        {
                            for (int j = 0; j < SAVE_IMAGE_QUANTITY_IN_RAM_FOR_RUNNING; j++)
                            {
                                if (ho_LeftInferredResultPictureStoreArray[j] == null)
                                {
                                    if (resultStr.Substring(0, 2) == "OK")
                                    {
                                        _leftInferredResultPictureSavePathArray[j] = _leftCameraInfer[i]._cameraInferredResultOKPictureSavePath;
                                    }
                                    else
                                    {
                                        _leftInferredResultPictureSavePathArray[j] = _leftCameraInfer[i]._cameraInferredResultNGPictureSavePath;
                                    }
                                    HOperatorSet.DumpWindowImage(out ho_LeftInferredResultPictureStoreArray[j], _mainFormVariate.picLeftCameraResultImageDisp.HalconWindow);
                                    saveResultPicArrayHaveSpaceFlag = true;
                                    break;
                                }
                            }
                            if (!saveResultPicArrayHaveSpaceFlag)
                                Thread.Sleep(5);
                        }

                    }
                }
                _leftCameraImageInferModuleActionFlag = 1;
            });
        }

        /// <summary>
        /// RightCameraImageInferModule：右侧相机采集图像推断模块
        /// </summary>
        private async void RightCameraImageInferModule()
        {
            await Task.Run(() =>
            {
                _rightCameraImageInferModuleActionFlag = 0;
                for (int i = 0; i < SAVE_IMAGE_QUANTITY_IN_RAM_FOR_RUNNING; i++)
                {
                    if (_rightCameraInfer[i]._cameraPictureRequestInferFlag && _rightCameraInfer[i].ho_CameraImage != null)
                    {
                        //infer右侧图片
                        _rightCameraInfer[i]._cameraPictureRequestInferFlag = false;
                        string preprocessFileFolderPath = _productParameter[_rightCameraInfer[i]._productIndex]._preprocessParameterFilePath.Substring(0, _productParameter[_rightCameraInfer[i]._productIndex]._preprocessParameterFilePath.LastIndexOf("\\"));
                        string preprocessFileName = _productParameter[_rightCameraInfer[i]._productIndex]._preprocessParameterFilePath.Substring(_productParameter[_rightCameraInfer[i]._productIndex]._preprocessParameterFilePath.LastIndexOf("\\") + 1);
                        string dimensionFilePath = _productParameter[_rightCameraInfer[i]._productIndex]._dimensionFilePath;
                        string modelFilePath = _productParameter[_rightCameraInfer[i]._productIndex]._modelFilePath.Substring(0, _productParameter[_rightCameraInfer[i]._productIndex]._modelFilePath.LastIndexOf("."));

                        _newRightDeepLearningObjectDetectionRectangle1Infer.InferAction(preprocessFileFolderPath, preprocessFileName, dimensionFilePath, modelFilePath,
                             1, true, _rightCameraInfer[i].ho_CameraImage, _rightInferNeedReadParameter, false);

                        if (_rightInferNeedReadParameter)
                            _rightInferNeedReadParameter = false;

                        HalconCameraControl.DispImageAdaptively(_mainFormVariate.picRightCameraResultImageDisp, _rightCameraInfer[i].ho_CameraImage, false);
                        string resultStr = "";
                        if (!SettingsAdministratorForm._checkProductIsConnector)//如果检测产品为switch
                        {
                            //筛选出infer找到的所有结果中confidence最大的那一个
                            int maxCofidenceIndex = 0;
                            float maxCofidence = 0f;
                            try
                            {
                                maxCofidence = _newRightDeepLearningObjectDetectionRectangle1Infer.inferResult.confidence[maxCofidenceIndex].F;
                                //for (int j = 0; j < _newRightDeepLearningObjectDetectionRectangle1Infer.inferResult.class_id.TupleLength(); j++)
                                //{
                                //    if (maxCofidence < _newRightDeepLearningObjectDetectionRectangle1Infer.inferResult.confidence[j])
                                //    {
                                //        maxCofidenceIndex = j;
                                //        maxCofidence = _newRightDeepLearningObjectDetectionRectangle1Infer.inferResult.confidence[j];
                                //    }
                                //}
                            }
                            catch (Exception e)
                            {
                                MessageBox.Show(e.ToString());
                            }

                            //判定结果
                            int classNamesIndex = 0;
                            try
                            {
                                if (_newRightDeepLearningObjectDetectionRectangle1Infer.inferResult.ho_Rectangle != null)
                                {
                                    if (_newRightDeepLearningObjectDetectionRectangle1Infer.inferResult.ho_Rectangle.CountObj() == 1)
                                    {
                                        classNamesIndex = (int)_newRightDeepLearningObjectDetectionRectangle1Infer.inferResult.class_id[maxCofidenceIndex];
                                        resultStr = _newRightDeepLearningObjectDetectionRectangle1Infer.inferResult.classNames[classNamesIndex];
                                        if (resultStr.Substring(0, 2) == "OK")
                                            resultStr = _newRightDeepLearningObjectDetectionRectangle1Infer.inferResult.classNames[classNamesIndex] + ": " + maxCofidence.ToString();
                                        //resultStr = (classNamesIndex - 1).ToString("00") + ": "
                                        //+ _newRightDeepLearningObjectDetectionRectangle1Infer.inferResult.classNames[classNamesIndex] + ": " + maxCofidence.ToString();
                                        else
                                            resultStr = "NG: NG";// resultStr.Substring(0, 2) + ": " + maxCofidence.ToString();

                                        HOperatorSet.SetDraw(_mainFormVariate.picRightCameraResultImageDisp.HalconWindow, "margin");
                                        if (classNamesIndex == 2 && (maxCofidence >= SettingsAdministratorForm._passThresholdValue ||
                                            !_baseSettingFormVariate._settingsAdminFormVariate.chkPassThresholdUsing.Checked))
                                            HOperatorSet.SetColor(_mainFormVariate.picRightCameraResultImageDisp.HalconWindow, "green");
                                        else if (classNamesIndex == 2)
                                        {
                                            resultStr = "NG" + ": " + maxCofidence.ToString();// +resultStr.Substring(resultStr.IndexOf(":"));
                                            classNamesIndex = 0;
                                            HOperatorSet.SetColor(_mainFormVariate.picRightCameraResultImageDisp.HalconWindow, "red");
                                        }
                                        else
                                        {
                                            HOperatorSet.SetColor(_mainFormVariate.picRightCameraResultImageDisp.HalconWindow, "red");
                                        }

                                        HOperatorSet.DispObj(_newRightDeepLearningObjectDetectionRectangle1Infer.inferResult.ho_Rectangle[maxCofidenceIndex + 1], _mainFormVariate.picRightCameraResultImageDisp.HalconWindow);

                                    }
                                    else
                                    { 
                                        resultStr = "NG: NG2";
                                        HOperatorSet.SetColor(_mainFormVariate.picRightCameraResultImageDisp.HalconWindow, "red");
                                    }
                                }
                                else
                                {
                                    resultStr = "NG: NG";
                                    HOperatorSet.SetColor(_mainFormVariate.picRightCameraResultImageDisp.HalconWindow, "red");
                                }
                            }
                            catch (Exception e)
                            {
                                MessageBox.Show(e.Message + _rightCameraInfer[i]._cameraPicturePointNumber.ToString());
                            }
                        }
                        else//如果检测产品为Connector
                        {
                            int[] inferKindQuantity = new int[5] { 0, 0, 0, 0, 0 };//推断结果种类个数，用于判定是否OK，索引号代表的是种类1-5
                            //设置显示为框
                            HOperatorSet.SetDraw(_mainFormVariate.picRightCameraResultImageDisp.HalconWindow, "margin");
                            try
                            {
                                for (int j = 0; j < _newRightDeepLearningObjectDetectionRectangle1Infer.inferResult.class_id.TupleLength(); j++)//检查所有类
                                {
                                    int inferKind = (int)_newRightDeepLearningObjectDetectionRectangle1Infer.inferResult.class_id[j];//获取当前种类
                                    string rectangleClr = "red";//设置显示rectangle的颜色
                                    if (inferKind == _productParameter[_rightCameraInfer[i]._productIndex]._inferResultKind1Number - 1 ||
                                        inferKind == _productParameter[_rightCameraInfer[i]._productIndex]._inferResultKind2Number - 1 ||
                                        inferKind == _productParameter[_rightCameraInfer[i]._productIndex]._inferResultKind3Number - 1 ||
                                        inferKind == _productParameter[_rightCameraInfer[i]._productIndex]._inferResultKind4Number - 1 ||
                                        inferKind == _productParameter[_rightCameraInfer[i]._productIndex]._inferResultKind5Number - 1)//如果当前的种类号与设定的种类号对应上了
                                    {
                                        if (_newRightDeepLearningObjectDetectionRectangle1Infer.inferResult.confidence[j] >= SettingsAdministratorForm._passThresholdValue ||
                                            !_baseSettingFormVariate._settingsAdminFormVariate.chkPassThresholdUsing.Checked)//如果启用了PASS阈值功能，那么如果大于设定阈值或者没有启用PASS阈值功能，那么算是推断OK
                                        {
                                            if (inferKind == _productParameter[_rightCameraInfer[i]._productIndex]._inferResultKind1Number - 1)
                                                inferKindQuantity[0]++;
                                            else if (inferKind == _productParameter[_rightCameraInfer[i]._productIndex]._inferResultKind2Number - 1)
                                                inferKindQuantity[1]++;
                                            else if (inferKind == _productParameter[_rightCameraInfer[i]._productIndex]._inferResultKind3Number - 1)
                                                inferKindQuantity[2]++;
                                            else if (inferKind == _productParameter[_rightCameraInfer[i]._productIndex]._inferResultKind4Number - 1)
                                                inferKindQuantity[3]++;
                                            else if (inferKind == _productParameter[_rightCameraInfer[i]._productIndex]._inferResultKind5Number - 1)
                                                inferKindQuantity[4]++;
                                            rectangleClr = "green";//设置显示rectangle的颜色
                                        }
                                    }

                                    //设置显示框的颜色
                                    HOperatorSet.SetColor(_mainFormVariate.picRightCameraResultImageDisp.HalconWindow, rectangleClr);
                                    //显示框
                                    HOperatorSet.DispObj(_newRightDeepLearningObjectDetectionRectangle1Infer.inferResult.ho_Rectangle[j + 1], _mainFormVariate.picRightCameraResultImageDisp.HalconWindow);//在halcon中，object的计数是从1开始的，所以索引号要+1

                                }

                                //获取需要检测到的种类总数
                                int needCheckKindTotalQuantity = 0;
                                if (_productParameter[_rightCameraInfer[i]._productIndex]._inferResultKind1Number != 0)//为零不计入数据
                                {
                                    needCheckKindTotalQuantity += _productParameter[_rightCameraInfer[i]._productIndex]._inferResultKind1Quantity;
                                }
                                if (_productParameter[_rightCameraInfer[i]._productIndex]._inferResultKind2Number != 0)//为零不计入数据
                                {
                                    needCheckKindTotalQuantity += _productParameter[_rightCameraInfer[i]._productIndex]._inferResultKind2Quantity;
                                }
                                if (_productParameter[_rightCameraInfer[i]._productIndex]._inferResultKind3Number != 0)//为零不计入数据
                                {
                                    needCheckKindTotalQuantity += _productParameter[_rightCameraInfer[i]._productIndex]._inferResultKind3Quantity;
                                }
                                if (_productParameter[_rightCameraInfer[i]._productIndex]._inferResultKind4Number != 0)//为零不计入数据
                                {
                                    needCheckKindTotalQuantity += _productParameter[_rightCameraInfer[i]._productIndex]._inferResultKind4Quantity;
                                }
                                if (_productParameter[_rightCameraInfer[i]._productIndex]._inferResultKind5Number != 0)//为零不计入数据
                                {
                                    needCheckKindTotalQuantity += _productParameter[_rightCameraInfer[i]._productIndex]._inferResultKind5Quantity;
                                }

                                //判定结果是否OK
                                if (needCheckKindTotalQuantity != _newRightDeepLearningObjectDetectionRectangle1Infer.inferResult.class_id.TupleLength())//如果找到的区域个数不为需要的个数
                                {
                                    resultStr = "NG: NG";
                                }
                                else//如果总数对应的OK
                                {
                                    if ((_productParameter[_rightCameraInfer[i]._productIndex]._inferResultKind1Number != 0 && _productParameter[_rightCameraInfer[i]._productIndex]._inferResultKind1Quantity != inferKindQuantity[0]) ||
                                       (_productParameter[_rightCameraInfer[i]._productIndex]._inferResultKind2Number != 0 && _productParameter[_rightCameraInfer[i]._productIndex]._inferResultKind2Quantity != inferKindQuantity[1]) ||
                                       (_productParameter[_rightCameraInfer[i]._productIndex]._inferResultKind3Number != 0 && _productParameter[_rightCameraInfer[i]._productIndex]._inferResultKind3Quantity != inferKindQuantity[2]) ||
                                       (_productParameter[_rightCameraInfer[i]._productIndex]._inferResultKind4Number != 0 && _productParameter[_rightCameraInfer[i]._productIndex]._inferResultKind4Quantity != inferKindQuantity[3]) ||
                                       (_productParameter[_rightCameraInfer[i]._productIndex]._inferResultKind5Number != 0 && _productParameter[_rightCameraInfer[i]._productIndex]._inferResultKind5Quantity != inferKindQuantity[4]))//如果种类没有设置为0，即有效的话
                                        resultStr = "NG: NG";
                                    else
                                        resultStr = "OK: OK";
                                }
                            }
                            catch
                            {
                                resultStr = "NG: NG";
                            }
                        }

                        _rightInferResultDt[_rightCameraInfer[i]._productIndex].Rows[_rightCameraInfer[i]._cameraPicturePointNumber - 1][3] = resultStr;

                        if (_rightCameraInfer[i]._productIndex == 0)//如果为传送线1上的产品
                        {
                            lock (_conveyor1InferResultDic)
                            {
                                if (_conveyor1InferResultDic.Count > 0)
                                {
                                    bool projectExistFlag = false;//用于标志要存储数据的项是否存在，false-不存在，true-存在
                                    int existIndex = -1;
                                    for (int j = 0; j < _conveyor1InferResultDic.Count; j++)
                                    {
                                        if (_conveyor1InferResultDic[j]._elementName == _rightCameraInfer[i]._cameraPicturePointName)//如果同名项存在
                                        {
                                            projectExistFlag = true;
                                            existIndex = j;
                                            break;
                                        }
                                    }
                                    if (projectExistFlag)//如果同名项存在
                                    {
                                        //判定当前产品是否是样品板
                                        bool currentPanelIsSampleFlag = false;
                                        int conveyorIndex = 0;
                                        if (_conveyor2Status == 103 || _conveyor2Status == 104)
                                            conveyorIndex = 1;
                                        if ((_barcodeData[conveyorIndex] == _productParameter[conveyorIndex]._sampleProduct1Barcode && _productParameter[conveyorIndex]._sampleProduct1Barcode != "") ||
                                                  (_barcodeData[conveyorIndex] == _productParameter[conveyorIndex]._sampleProduct2Barcode && _productParameter[conveyorIndex]._sampleProduct2Barcode != "") ||
                                                (_barcodeData[conveyorIndex] == _productParameter[conveyorIndex]._sampleProduct3Barcode && _productParameter[conveyorIndex]._sampleProduct3Barcode != ""))
                                            currentPanelIsSampleFlag = true;

                                        InferResult tempInferResult = new InferResult();
                                        tempInferResult = _conveyor1InferResultDic[existIndex];
                                        tempInferResult._elementRightResult = resultStr.Substring(0, 2);
                                        if (tempInferResult._elementRightResult.Length >= 2 && tempInferResult._elementLeftResult.Length >= 2)
                                        {
                                            lock (_yieldRecordDt)
                                            {
                                                if ((SettingsGeneralForm._addSampleDataToYieldFlag && currentPanelIsSampleFlag) || !currentPanelIsSampleFlag)
                                                {
                                                    _yieldRecordDt.Rows[8][1] = (Convert.ToInt32(_yieldRecordDt.Rows[8][1].ToString()) + 1).ToString();
                                                    _yieldRecordDt.Rows[8][2] = (Convert.ToInt32(_yieldRecordDt.Rows[8][2].ToString()) + 1).ToString();
                                                }
                                                if (tempInferResult._elementRightResult.Substring(0, 2) == "OK" && tempInferResult._elementLeftResult.Substring(0, 2) == "OK")
                                                {
                                                    tempInferResult._elementTotalResult = "PASS";
                                                    if ((SettingsGeneralForm._addSampleDataToYieldFlag && currentPanelIsSampleFlag) || !currentPanelIsSampleFlag)
                                                    {
                                                        _yieldRecordDt.Rows[9][1] = (Convert.ToInt32(_yieldRecordDt.Rows[9][1].ToString()) + 1).ToString();
                                                        _yieldRecordDt.Rows[9][2] = (Convert.ToInt32(_yieldRecordDt.Rows[9][2].ToString()) + 1).ToString();
                                                    }
                                                }
                                                else
                                                {
                                                    tempInferResult._elementTotalResult = "FAIL";
                                                    if ((SettingsGeneralForm._addSampleDataToYieldFlag && currentPanelIsSampleFlag) || !currentPanelIsSampleFlag)
                                                    {
                                                        _yieldRecordDt.Rows[10][1] = (Convert.ToInt32(_yieldRecordDt.Rows[10][1].ToString()) + 1).ToString();
                                                        _yieldRecordDt.Rows[10][2] = (Convert.ToInt32(_yieldRecordDt.Rows[10][2].ToString()) + 1).ToString();
                                                    }
                                                }
                                            }
                                        }
                                        _conveyor1InferResultDic[existIndex] = tempInferResult;
                                    }
                                    else
                                    {
                                        InferResult tempInferResult = new InferResult();
                                        tempInferResult._elementNumber = (_conveyor1InferResultDic.Count + 1).ToString();
                                        tempInferResult._elementName = _rightCameraInfer[i]._cameraPicturePointName;
                                        tempInferResult._elementRightResult = resultStr.Substring(0, 2);
                                        tempInferResult._elementLeftResult = "";
                                        tempInferResult._elementTotalResult = "";
                                        _conveyor1InferResultDic.Add(_conveyor1InferResultDic.Count, tempInferResult);
                                    }
                                }
                                else
                                {
                                    InferResult tempInferResult = new InferResult();
                                    tempInferResult._elementNumber = (_conveyor1InferResultDic.Count + 1).ToString();
                                    tempInferResult._elementName = _rightCameraInfer[i]._cameraPicturePointName;
                                    tempInferResult._elementRightResult = resultStr.Substring(0, 2);
                                    tempInferResult._elementLeftResult = "";
                                    tempInferResult._elementTotalResult = "";
                                    _conveyor1InferResultDic.Add(_conveyor1InferResultDic.Count, tempInferResult);
                                }
                            }
                        }
                        else
                        {
                            lock (_conveyor2InferResultDic)
                            {
                                if (_conveyor2InferResultDic.Count > 0)
                                {
                                    bool projectExistFlag = false;//用于标志要存储数据的项是否存在，false-不存在，true-存在
                                    int existIndex = -1;
                                    for (int j = 0; j < _conveyor2InferResultDic.Count; j++)
                                    {
                                        if (_conveyor2InferResultDic[j]._elementName == _rightCameraInfer[i]._cameraPicturePointName)//如果同名项存在
                                        {
                                            projectExistFlag = true;
                                            existIndex = j;
                                            break;
                                        }
                                    }

                                    if (projectExistFlag)
                                    {
                                        //判定当前产品是否是样品板
                                        bool currentPanelIsSampleFlag = false;
                                        int conveyorIndex = 0;
                                        if (_conveyor2Status == 103 || _conveyor2Status == 104)
                                            conveyorIndex = 1;
                                        if ((_barcodeData[conveyorIndex] == _productParameter[conveyorIndex]._sampleProduct1Barcode && _productParameter[conveyorIndex]._sampleProduct1Barcode != "") ||
                                                  (_barcodeData[conveyorIndex] == _productParameter[conveyorIndex]._sampleProduct2Barcode && _productParameter[conveyorIndex]._sampleProduct2Barcode != "") ||
                                                (_barcodeData[conveyorIndex] == _productParameter[conveyorIndex]._sampleProduct3Barcode && _productParameter[conveyorIndex]._sampleProduct3Barcode != ""))
                                            currentPanelIsSampleFlag = true;

                                        InferResult tempInferResult = new InferResult();
                                        tempInferResult = _conveyor2InferResultDic[existIndex];
                                        tempInferResult._elementRightResult = resultStr.Substring(0, 2);
                                        if (tempInferResult._elementRightResult.Length >= 2 && tempInferResult._elementLeftResult.Length >= 2)
                                        {
                                            lock (_yieldRecordDt)
                                            {
                                                if ((SettingsGeneralForm._addSampleDataToYieldFlag && currentPanelIsSampleFlag) || !currentPanelIsSampleFlag)
                                                {
                                                    _yieldRecordDt.Rows[8][1] = (Convert.ToInt32(_yieldRecordDt.Rows[8][1].ToString()) + 1).ToString();
                                                    _yieldRecordDt.Rows[8][3] = (Convert.ToInt32(_yieldRecordDt.Rows[8][3].ToString()) + 1).ToString();
                                                }
                                                if (tempInferResult._elementRightResult.Substring(0, 2) == "OK" && tempInferResult._elementLeftResult.Substring(0, 2) == "OK")
                                                {
                                                    tempInferResult._elementTotalResult = "PASS";
                                                    if ((SettingsGeneralForm._addSampleDataToYieldFlag && currentPanelIsSampleFlag) || !currentPanelIsSampleFlag)
                                                    {
                                                        _yieldRecordDt.Rows[9][1] = (Convert.ToInt32(_yieldRecordDt.Rows[9][1].ToString()) + 1).ToString();
                                                        _yieldRecordDt.Rows[9][3] = (Convert.ToInt32(_yieldRecordDt.Rows[9][3].ToString()) + 1).ToString();
                                                    }
                                                }
                                                else
                                                {
                                                    tempInferResult._elementTotalResult = "FAIL";
                                                    if ((SettingsGeneralForm._addSampleDataToYieldFlag && currentPanelIsSampleFlag) || !currentPanelIsSampleFlag)
                                                    {
                                                        _yieldRecordDt.Rows[10][1] = (Convert.ToInt32(_yieldRecordDt.Rows[10][1].ToString()) + 1).ToString();
                                                        _yieldRecordDt.Rows[10][3] = (Convert.ToInt32(_yieldRecordDt.Rows[10][3].ToString()) + 1).ToString();
                                                    }
                                                }
                                            }
                                        }
                                        _conveyor2InferResultDic[existIndex] = tempInferResult;
                                    }
                                    else
                                    {
                                        InferResult tempInferResult = new InferResult();
                                        tempInferResult._elementNumber = (_conveyor2InferResultDic.Count + 1).ToString();
                                        tempInferResult._elementName = _rightCameraInfer[i]._cameraPicturePointName;
                                        tempInferResult._elementRightResult = resultStr.Substring(0, 2);
                                        tempInferResult._elementLeftResult = "";
                                        tempInferResult._elementTotalResult = "";
                                        _conveyor2InferResultDic.Add(_conveyor2InferResultDic.Count, tempInferResult);
                                    }
                                }
                                else
                                {
                                    InferResult tempInferResult = new InferResult();
                                    tempInferResult._elementNumber = (_conveyor2InferResultDic.Count + 1).ToString();
                                    tempInferResult._elementName = _rightCameraInfer[i]._cameraPicturePointName;
                                    tempInferResult._elementRightResult = resultStr.Substring(0, 2);
                                    tempInferResult._elementLeftResult = "";
                                    tempInferResult._elementTotalResult = "";
                                    _conveyor2InferResultDic.Add(_conveyor2InferResultDic.Count, tempInferResult);
                                }
                            }
                        }

                        //显示文字
                        HalconCameraControl.SetDisplayFont(_mainFormVariate.picRightCameraResultImageDisp.HalconWindow, 16, "mono", "false", "false");
                        if (resultStr.Substring(0, 2) == "OK")
                        {
                            HOperatorSet.DispText(_mainFormVariate.picRightCameraResultImageDisp.HalconWindow, "R" + _rightCameraInfer[i]._cameraPicturePointName + ": " + resultStr.Substring(0, 2),
                                "image", 12, 12, "green", "box", "false");
                        }
                        else
                        {
                            resultStr = "NG: NG";
                            HOperatorSet.DispText(_mainFormVariate.picRightCameraResultImageDisp.HalconWindow, "R" + _rightCameraInfer[i]._cameraPicturePointName + ": " + resultStr.Substring(0, 2),
                            "image", 12, 12, "red", "box", "false");
                        }

                        _rightCameraInfer[i]._cameraPictureInferredFlag = true;

                        //将要保存的Inferred结果图片保存至对应的Array中
                        bool saveResultPicArrayHaveSpaceFlag = false;
                        while (!saveResultPicArrayHaveSpaceFlag)
                        {
                            for (int j = 0; j < SAVE_IMAGE_QUANTITY_IN_RAM_FOR_RUNNING; j++)
                            {
                                if (ho_RightInferredResultPictureStoreArray[j] == null)
                                {
                                    if (resultStr.Substring(0, 2) == "OK")
                                    {
                                        _rightInferredResultPictureSavePathArray[j] = _rightCameraInfer[i]._cameraInferredResultOKPictureSavePath;
                                    }
                                    else
                                    {
                                        _rightInferredResultPictureSavePathArray[j] = _rightCameraInfer[i]._cameraInferredResultNGPictureSavePath;
                                    }

                                    HOperatorSet.DumpWindowImage(out ho_RightInferredResultPictureStoreArray[j], _mainFormVariate.picRightCameraResultImageDisp.HalconWindow);
                                    saveResultPicArrayHaveSpaceFlag = true;
                                    break;
                                }
                            }
                            if (!saveResultPicArrayHaveSpaceFlag)
                                Thread.Sleep(5);
                        }
                    }
                }
                _rightCameraImageInferModuleActionFlag = 1;
            });
        }

        /// <summary>
        /// LeftCameraImageSaveModule：左侧相机原始图片保存模块
        /// </summary>
        private async void LeftCameraSourceImageSaveModule()
        {
            await Task.Run(() =>
            {
                _leftCameraSourceImageSaveModuleActionFlag = 0;

                for (int i = 0; i < SAVE_IMAGE_QUANTITY_IN_RAM_FOR_RUNNING; i++)
                {

                    if (_leftCameraInfer[i]._cameraPictureSavePath != "")//如果相机保存路径不为空
                    {
                        if (SettingsGeneralForm._saveSourceImageFlag)//如果需要保存原始图片
                        {
                            if (!Directory.Exists(_leftCameraInfer[i]._cameraPictureSavePath.Substring(0, _leftCameraInfer[i]._cameraPictureSavePath.LastIndexOf("\\"))))
                                Directory.CreateDirectory(_leftCameraInfer[i]._cameraPictureSavePath.Substring(0, _leftCameraInfer[i]._cameraPictureSavePath.LastIndexOf("\\")));

                            if (_leftCameraInfer[i].ho_CameraImage != null)
                            {
                                HOperatorSet.WriteImage(_leftCameraInfer[i].ho_CameraImage, "jpg", 0, _leftCameraInfer[i]._cameraPictureSavePath);
                            }
                        }
                        HalconCameraControl.DispImageAdaptively(_mainFormVariate.picLeftCameraSourceImageDisp, _leftCameraInfer[i].ho_CameraImage, false);

                        _leftCameraInfer[i]._cameraPictureSavePath = "";
                        _leftCameraInfer[i]._cameraPictureSavedFlag = true;
                    }
                }

                _leftCameraSourceImageSaveModuleActionFlag = 1;
            });
        }

        /// <summary>
        /// LeftCameraInferredImageSaveModule:左侧相机推断结果图片保存模块
        /// </summary>
        private async void LeftCameraInferredImageSaveModule()
        {
            await Task.Run(() =>
            {
                _leftCameraInferredImageSaveModuleActionFlag = 0;
                string imageOKOrNGFlagString = "";
                for (int i = 0; i < SAVE_IMAGE_QUANTITY_IN_RAM_FOR_RUNNING; i++)
                {
                    if (ho_LeftInferredResultPictureStoreArray[i] != null)
                    {
                        imageOKOrNGFlagString = _leftInferredResultPictureSavePathArray[i].Substring(0, _leftInferredResultPictureSavePathArray[i].LastIndexOf("\\"));
                        imageOKOrNGFlagString = imageOKOrNGFlagString.Substring(0, imageOKOrNGFlagString.LastIndexOf("\\"));
                        imageOKOrNGFlagString = imageOKOrNGFlagString.Substring(imageOKOrNGFlagString.Length - 2, 2);
                        if ((imageOKOrNGFlagString == "OK" && SettingsGeneralForm._saveOKInferredImageFlag) ||
                            (imageOKOrNGFlagString == "NG" && SettingsGeneralForm._saveNGInferredImageFlag))
                        {
                            if (!Directory.Exists(_leftInferredResultPictureSavePathArray[i].Substring(0, _leftInferredResultPictureSavePathArray[i].LastIndexOf("\\"))))
                                Directory.CreateDirectory(_leftInferredResultPictureSavePathArray[i].Substring(0, _leftInferredResultPictureSavePathArray[i].LastIndexOf("\\")));

                            HOperatorSet.WriteImage(ho_LeftInferredResultPictureStoreArray[i], "jpg", 0, _leftInferredResultPictureSavePathArray[i]);
                        }

                        ho_LeftInferredResultPictureStoreArray[i] = null;
                    }
                }
                _leftCameraInferredImageSaveModuleActionFlag = 1;

            });
        }

        /// <summary>
        /// RightCameraImageSaveModule：右侧相机图片保存模块
        /// </summary>
        private async void RightCameraSourceImageSaveModule()
        {
            await Task.Run(() =>
            {
                _rightCameraSourceImageSaveModuleActionFlag = 0;

                for (int i = 0; i < SAVE_IMAGE_QUANTITY_IN_RAM_FOR_RUNNING; i++)
                {
                    if (_rightCameraInfer[i]._cameraPictureSavePath != "")//如果相机保存路径不为空
                    {
                        if (SettingsGeneralForm._saveSourceImageFlag)//如果需要保存原始图片
                        {
                            if (!Directory.Exists(_rightCameraInfer[i]._cameraPictureSavePath.Substring(0, _rightCameraInfer[i]._cameraPictureSavePath.LastIndexOf("\\"))))
                                Directory.CreateDirectory(_rightCameraInfer[i]._cameraPictureSavePath.Substring(0, _rightCameraInfer[i]._cameraPictureSavePath.LastIndexOf("\\")));

                            if (_rightCameraInfer[i].ho_CameraImage != null)
                            {
                                HOperatorSet.WriteImage(_rightCameraInfer[i].ho_CameraImage, "jpg", 0, _rightCameraInfer[i]._cameraPictureSavePath);

                            }
                        }
                        HalconCameraControl.DispImageAdaptively(_mainFormVariate.picRightCameraSourceImageDisp, _rightCameraInfer[i].ho_CameraImage, false);
                        _rightCameraInfer[i]._cameraPictureSavePath = "";
                        _rightCameraInfer[i]._cameraPictureSavedFlag = true;
                    }
                }

                _rightCameraSourceImageSaveModuleActionFlag = 1;
            });
        }

        /// <summary>
        /// RightCameraInferredImageSaveModule:右侧相机推断结果图片保存模块
        /// </summary>
        private async void RightCameraInferredImageSaveModule()
        {
            await Task.Run(() =>
            {
                _rightCameraInferredImageSaveModuleActionFlag = 0;
                string imageOKOrNGFlagString = "";
                for (int i = 0; i < SAVE_IMAGE_QUANTITY_IN_RAM_FOR_RUNNING; i++)
                {
                    if (ho_RightInferredResultPictureStoreArray[i] != null)
                    {
                        imageOKOrNGFlagString = _rightInferredResultPictureSavePathArray[i].Substring(0, _rightInferredResultPictureSavePathArray[i].LastIndexOf("\\"));
                        imageOKOrNGFlagString = imageOKOrNGFlagString.Substring(0, imageOKOrNGFlagString.LastIndexOf("\\"));
                        imageOKOrNGFlagString = imageOKOrNGFlagString.Substring(imageOKOrNGFlagString.Length - 2, 2);
                        if ((imageOKOrNGFlagString == "OK" && SettingsGeneralForm._saveOKInferredImageFlag) ||
                            (imageOKOrNGFlagString == "NG" && SettingsGeneralForm._saveNGInferredImageFlag))
                        {
                            if (!Directory.Exists(_rightInferredResultPictureSavePathArray[i].Substring(0, _rightInferredResultPictureSavePathArray[i].LastIndexOf("\\"))))
                                Directory.CreateDirectory(_rightInferredResultPictureSavePathArray[i].Substring(0, _rightInferredResultPictureSavePathArray[i].LastIndexOf("\\")));

                            HOperatorSet.WriteImage(ho_RightInferredResultPictureStoreArray[i], "jpg", 0, _rightInferredResultPictureSavePathArray[i]);
                        }
                        ho_RightInferredResultPictureStoreArray[i] = null;
                    }
                }
                _rightCameraInferredImageSaveModuleActionFlag = 1;
            });
        }

        /// <summary>
        /// CameraInferDataClearModule：根据flag对左右相机采集图像等数据的结构体进行数据清零
        /// </summary>
        private async void CameraInferDataClearModule()
        {
            await Task.Run(() =>
            {
                _cameraInferDataClearModuleActionFlag = 0;
                bool leftCameraInferFinishFlag = true;
                bool rightCameraInferFinishFlag = true;
                for (int i = 0; i < SAVE_IMAGE_QUANTITY_IN_RAM_FOR_RUNNING; i++)
                {
                    if (_leftCameraInfer[i]._cameraPictureSavedFlag && _leftCameraInfer[i]._cameraPictureInferredFlag)
                    {
                        _leftCameraInfer[i].ho_CameraImage = null;
                        _leftCameraInfer[i]._cameraPictureSavedFlag = false;
                        _leftCameraInfer[i]._cameraPictureInferredFlag = false;
                        _leftCameraInfer[i]._productIndex = -1;
                    }

                    if (_rightCameraInfer[i]._cameraPictureSavedFlag && _rightCameraInfer[i]._cameraPictureInferredFlag)
                    {
                        _rightCameraInfer[i].ho_CameraImage = null;
                        _rightCameraInfer[i]._cameraPictureSavedFlag = false;
                        _rightCameraInfer[i]._cameraPictureInferredFlag = false;
                        _rightCameraInfer[i]._productIndex = -1;
                    }

                    if (_leftCameraInfer[i].ho_CameraImage != null)
                    {
                        leftCameraInferFinishFlag = false;
                    }

                    if (_rightCameraInfer[i].ho_CameraImage != null)
                    {
                        rightCameraInferFinishFlag = false;
                    }
                }

                if (leftCameraInferFinishFlag && rightCameraInferFinishFlag && _conveyor1Status == 104)
                {
                    if (_conveyor1HaveExcessProductAlarmDealWithWayIndex != 1)
                    {
                        _conveyor1Status = 105;
                    }
                    _mainFormVariate.lblUsingTime.Invoke(new Action(() =>
                    {
                        _mainFormVariate.lblUsingTime.Text = "用时:" + ((float)_inferTimCountSW.ElapsedMilliseconds / 1000f).ToString("f3") + "s";
                    })); ;
                    _inferTimCountSW.Stop();
                }

                if (leftCameraInferFinishFlag && rightCameraInferFinishFlag && _conveyor2Status == 104)
                {
                    if (_conveyor2HaveExcessProductAlarmDealWithWayIndex != 1)
                    {
                        _conveyor2Status = 105;
                    }
                    _mainFormVariate.lblUsingTime.Invoke(new Action(() =>
                    {
                        _mainFormVariate.lblUsingTime.Text = "用时:" + ((float)_inferTimCountSW.ElapsedMilliseconds / 1000f).ToString("f3") + "s";
                    })); ;
                    _inferTimCountSW.Stop();
                }

                _cameraInferDataClearModuleActionFlag = 1;
            });
        }

        #endregion

        /// <summary>
        /// BarcodeScanModule:条形码扫码模块
        /// </summary>
        /// <param name="conveyorIndex">int:传送线索引,0-传送线1，1-传送线2</param>
        private async void BarcodeScanModule(int conveyorIndex)
        {
            await Task.Run(() =>
            {
                _barcodeScanModuleActionFlag = 0;

                if (conveyorIndex == 0)
                    MyTool.TxtFileProcess.CreateLog("设备动作-设备扫描传送线" + (conveyorIndex + 1).ToString() + "上，产品“"
                    + SettingsProductForm._track1ActualsProductName + "”的条形码" + "----用户：" + _operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                else
                    MyTool.TxtFileProcess.CreateLog("设备动作-设备扫描传送线" + (conveyorIndex + 1).ToString() + "上，产品“"
                    + SettingsProductForm._track2ActualsProductName + "”的条形码" + "----用户：" + _operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

                //清理二维码二次检测界面变量为空
                if (_barcodeRecheckFormVariate != null)
                {
                    _barcodeRecheckFormVariate.Dispose();
                    _barcodeRecheckFormVariate = null;
                }

                int scanBarcodePosition = 2;
                if (conveyorIndex == 1)
                {
                    scanBarcodePosition = 3;
                }

                if (!_axisControlFormVariate._ioStatus[1, BACK_CAMERA_LIGHT_TRIGGER_OUT_IO] || !_axisControlFormVariate._ioStatus[1, BACK_CAMERA_LIGHT_TRIGGER_OUT_IO2])
                {
                    _axisControlFormVariate.OutputControl(BACK_CAMERA_LIGHT_TRIGGER_OUT_IO, true);//打开光源
                    _axisControlFormVariate.OutputControl(BACK_CAMERA_LIGHT_TRIGGER_OUT_IO2, true);//打开光源
                }

                while (_axisControlFormVariate._axisActionCommand[0] != -1 ||
                        _axisControlFormVariate._axisActionCommand[1] != -1 ||
                        _axisControlFormVariate._axisActionCommand[2] != -1 ||
                        !_deviceWorkTriggerFlag ||
                        (conveyorIndex == 0 && _conveyor1HaveExcessProductAlarmDealWithWayIndex == 1) ||
                        (conveyorIndex == 1 && _conveyor2HaveExcessProductAlarmDealWithWayIndex == 1))//当XY轴处于运动指令状态,循环等待
                {
                    if (_deadlyAlarmOccurFlag || !_autoModeFlag ||
                       (conveyorIndex == 0 && _conveyor1HaveExcessProductAlarmDealWithWayIndex == 1) ||
                       (conveyorIndex == 1 && _conveyor2HaveExcessProductAlarmDealWithWayIndex == 1))//如果触发了紧急停止
                    {
                        _barcodeScanModuleActionFlag = -1;
                        return;
                    }
                    Thread.Sleep(1);
                }

                //请求XYR轴去扫码位置，2号点
                //X1
                _axisControlFormVariate._axisMotionControlBoardVariate._axis00CurrentPositionNumber = 0;
                _axisControlFormVariate._axisCurrentPositionNumber[0] = 0;
                _axisControlFormVariate._assignedPositionActionTargetPosition_mm[0] = Convert.ToSingle(_axisControlFormVariate._axisPointDataDT[0].Rows[scanBarcodePosition - 1][1].ToString()) + _markSearchXOffsetData;
                _axisControlFormVariate._assignedPositionActionMaxVelocity_mmps[0] = Convert.ToSingle(_axisControlFormVariate._axisPointDataDT[0].Rows[scanBarcodePosition - 1][2].ToString());
                _axisControlFormVariate._assignedPositionActionAccTime_s[0] = Convert.ToSingle(_axisControlFormVariate._axisPointDataDT[0].Rows[scanBarcodePosition - 1][3].ToString());
                _axisControlFormVariate._assignedPositionActionDeviation[0] = 0f;
                _axisControlFormVariate._assignedPositionActionSMoveFlag[0] = false;
                _axisControlFormVariate._assignedPositionActionSMoveTime[0] = 0.01f;
                _axisControlFormVariate._axisActionCommand[0] = 1000;//指定点位移动
                //Y1
                _axisControlFormVariate._axisMotionControlBoardVariate._axis01CurrentPositionNumber = 0;
                _axisControlFormVariate._axisCurrentPositionNumber[1] = 0;
                _axisControlFormVariate._assignedPositionActionTargetPosition_mm[1] = Convert.ToSingle(_axisControlFormVariate._axisPointDataDT[1].Rows[scanBarcodePosition - 1][1].ToString()) + _markSearchYOffsetData;
                _axisControlFormVariate._assignedPositionActionMaxVelocity_mmps[1] = Convert.ToSingle(_axisControlFormVariate._axisPointDataDT[1].Rows[scanBarcodePosition - 1][2].ToString());
                _axisControlFormVariate._assignedPositionActionAccTime_s[1] = Convert.ToSingle(_axisControlFormVariate._axisPointDataDT[1].Rows[scanBarcodePosition - 1][3].ToString());
                _axisControlFormVariate._assignedPositionActionDeviation[1] = 0f;
                _axisControlFormVariate._assignedPositionActionSMoveFlag[1] = false;
                _axisControlFormVariate._assignedPositionActionSMoveTime[1] = 0.01f;
                _axisControlFormVariate._axisActionCommand[1] = 1000;//指定点位移动
                //R1
                _axisControlFormVariate._axisCurrentPositionNumber[2] = 0;
                _axisControlFormVariate._axisActionCommand[2] = scanBarcodePosition;

                while ((_axisControlFormVariate._axisCurrentPositionNumber[0] == 0 || _axisControlFormVariate._axisActionCommand[0] != -1) ||
                    (_axisControlFormVariate._axisCurrentPositionNumber[1] == 0 || _axisControlFormVariate._axisActionCommand[1] != -1) ||
                    (_axisControlFormVariate._axisCurrentPositionNumber[2] == 0 || _axisControlFormVariate._axisActionCommand[2] != -1) ||
                    !_deviceWorkTriggerFlag ||
                    (conveyorIndex == 0 && _conveyor1HaveExcessProductAlarmDealWithWayIndex == 1) ||
                    (conveyorIndex == 1 && _conveyor2HaveExcessProductAlarmDealWithWayIndex == 1))//等待XY轴运行结束
                {
                    if (_deadlyAlarmOccurFlag || !_autoModeFlag ||
                       (conveyorIndex == 0 && _conveyor1HaveExcessProductAlarmDealWithWayIndex == 1) ||
                       (conveyorIndex == 1 && _conveyor2HaveExcessProductAlarmDealWithWayIndex == 1))//如果触发了紧急停止
                    {
                        _barcodeScanModuleActionFlag = -1;
                        return;
                    }
                    Thread.Sleep(1);
                }
                //采集图像
                bool requireBarcodeResult = false;
                HObject ho_resultRectangle = null;
                bool scanFailed = false;
                
                if (_backCameraCs.ho_Image != null)
                {
                    _backCameraCs.ho_Image.Dispose();
                    _backCameraCs.ho_Image = null;
                }
                if (_backCameraCs.GrabImage2(false))
                {
                    _tempBarcode = "";
                    if (HalconPictureProcess.BarcodeAcquire(_backCameraCs.ho_Image, out _tempBarcode, out ho_resultRectangle))
                    {
                        try
                        {
                            if (conveyorIndex == 0)//如果为传送线1
                            {
                                HOperatorSet.ClearWindow(picConveyor1BarcodeScanResultImageDisp.HalconWindow);
                                HalconCameraControl.DispImageAdaptively(picConveyor1BarcodeScanResultImageDisp, _backCameraCs.ho_Image, false);
                                HOperatorSet.DispText(picConveyor1BarcodeScanResultImageDisp.HalconWindow, _tempBarcode, "window", "top",
                                "left", "black", new HTuple(), new HTuple());
                                HOperatorSet.DispObj(ho_resultRectangle, picConveyor1BarcodeScanResultImageDisp.HalconWindow);
                            }
                            else
                            {
                                HOperatorSet.ClearWindow(picConveyor2BarcodeScanResultImageDisp.HalconWindow);
                                HalconCameraControl.DispImageAdaptively(picConveyor2BarcodeScanResultImageDisp, _backCameraCs.ho_Image, false);
                                HOperatorSet.DispText(picConveyor2BarcodeScanResultImageDisp.HalconWindow, _tempBarcode, "window", "top",
                                "left", "black", new HTuple(), new HTuple());
                                HOperatorSet.DispObj(ho_resultRectangle, picConveyor2BarcodeScanResultImageDisp.HalconWindow);
                            }
                        }
                        catch (Exception e)
                        {
                            if (conveyorIndex == 0)
                                MyTool.TxtFileProcess.CreateLog("设备动作-设备扫描传送线" + (conveyorIndex + 1).ToString() + "上，产品“"
                                 + SettingsProductForm._track1ActualsProductName + "”的条形码失败，错误信息为：" + e.ToString() + "----用户：" + _operatorName,
                                BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                            else
                                MyTool.TxtFileProcess.CreateLog("设备动作-设备扫描传送线" + (conveyorIndex + 1).ToString() + "上，产品“"
                                + SettingsProductForm._track2ActualsProductName + "”的条形码失败，错误信息为：" + e.ToString() + "----用户：" + _operatorName,
                                BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

                            scanFailed = true;
                        }

                        if (scanFailed)//如果扫码完毕之后显示失败
                        {
                            if (conveyorIndex == 0)//如果为传送线1
                                _stateAndAlarmCs._alarmFlag[350] = true;
                            else
                                _stateAndAlarmCs._alarmFlag[351] = true;

                            //弹出二维码手动输入框
                            _barcodeRecheckGiveUpFlag = false;//如果点击了取消按钮，这个FLAG置为ON
                            this.Invoke(new Action(() =>
                            {
                                _barcodeRecheckFormVariate = new RecheckOrChangeBarcodeForm(_tempBarcode);
                                _barcodeRecheckFormVariate.RecheckOrChangeBarcodeFormEnsureEvent += RecheckOrChangeBarcodeFormEnsureEventFunc;
                                _barcodeRecheckFormVariate.RecheckOrChangeBarcodeFormCancelEvent += RecheckOrChangeBarcodeFormCancelEvent;
                                _barcodeRecheckFormVariate.Focus();
                                _barcodeRecheckFormVariate.StartPosition = FormStartPosition.CenterParent;
                                _barcodeRecheckFormVariate.Show();
                            })); ;

                            while (_barcodeRecheckFormVariate != null || _stateAndAlarmCs._alarmFlag[350] || _stateAndAlarmCs._alarmFlag[351] ||
                                  (conveyorIndex == 0 && (_conveyor1HaveExcessProductAlarmDealWithWayIndex == 1 || _stateAndAlarmCs._alarmFlag[400])) ||
                                  (conveyorIndex == 1 && (_conveyor2HaveExcessProductAlarmDealWithWayIndex == 1 || _stateAndAlarmCs._alarmFlag[401])))//等待清除报警以及点击提示框
                            {
                                if (_deadlyAlarmOccurFlag || !_autoModeFlag ||
                                (conveyorIndex == 0 && _conveyor1HaveExcessProductAlarmDealWithWayIndex == 1) ||
                                (conveyorIndex == 1 && _conveyor2HaveExcessProductAlarmDealWithWayIndex == 1))//如果触发了紧急停止
                                {
                                    _barcodeScanModuleActionFlag = -1;
                                    return;
                                }
                                Thread.Sleep(1);
                            }

                            //如果没有点击取消按钮，那么就算是扫码成功
                            if (!_barcodeRecheckGiveUpFlag)
                            {
                                scanFailed = false;
                            }
                        }
                        else if (!scanFailed && _baseSettingFormVariate._settingsGeneralFormVariate.chkBarcodeRecheck.Checked)//如果扫码成功，但是选中了二次检测，弹出提示框，让人工进行检测
                        {
                            //弹出二维码二次确认框
                            _barcodeRecheckGiveUpFlag = false;//如果点击了取消按钮，这个FLAG置为ON
                            this.Invoke(new Action(() =>
                                {
                                    _barcodeRecheckFormVariate = new RecheckOrChangeBarcodeForm(_tempBarcode);
                                    _barcodeRecheckFormVariate.RecheckOrChangeBarcodeFormEnsureEvent += RecheckOrChangeBarcodeFormEnsureEventFunc;
                                    _barcodeRecheckFormVariate.RecheckOrChangeBarcodeFormCancelEvent += RecheckOrChangeBarcodeFormCancelEvent;
                                    _barcodeRecheckFormVariate.Focus();
                                    _barcodeRecheckFormVariate.StartPosition = FormStartPosition.CenterScreen;
                                    _barcodeRecheckFormVariate.Show();
                                })); ;

                            while (_barcodeRecheckFormVariate != null ||
                                  (conveyorIndex == 0 && (_conveyor1HaveExcessProductAlarmDealWithWayIndex == 1 || _stateAndAlarmCs._alarmFlag[400])) ||
                                  (conveyorIndex == 1 && (_conveyor2HaveExcessProductAlarmDealWithWayIndex == 1 || _stateAndAlarmCs._alarmFlag[401])))//等待人工干预，关闭窗口
                            {
                                if (_deadlyAlarmOccurFlag || !_autoModeFlag ||
                                (conveyorIndex == 0 && _conveyor1HaveExcessProductAlarmDealWithWayIndex == 1) ||
                                (conveyorIndex == 1 && _conveyor2HaveExcessProductAlarmDealWithWayIndex == 1))//如果触发了紧急停止
                                {
                                    _barcodeScanModuleActionFlag = -1;
                                    return;
                                }
                                Thread.Sleep(1);
                            }
                        }

                        //如果人工输入了二维码或是人工二次确认了二维码
                        if (!scanFailed)
                        {
                            _barcodeData[conveyorIndex] = _tempBarcode;//将二维码数据传输至保存的数组中
                            _mainFormVariate.Invoke(new Action(() =>
                                {
                                    if (conveyorIndex == 0)
                                        _mainFormVariate.lblConveyor1Barcode.Text = _tempBarcode;
                                    else
                                        _mainFormVariate.lblConveyor2Barcode.Text = _tempBarcode;
                                })); ;

                            requireBarcodeResult = true;
                        }
                    }
                    else//如果扫码失败
                    {
                        if (conveyorIndex == 0)
                            MyTool.TxtFileProcess.CreateLog("设备动作-设备扫描传送线" + (conveyorIndex + 1).ToString() + "上，产品“"
                             + SettingsProductForm._track1ActualsProductName + "”的条形码失败" + "----用户：" + _operatorName,
                            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                        else
                            MyTool.TxtFileProcess.CreateLog("设备动作-设备扫描传送线" + (conveyorIndex + 1).ToString() + "上，产品“"
                            + SettingsProductForm._track2ActualsProductName + "”的条形码失败" + "----用户：" + _operatorName,
                            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

                        if (conveyorIndex == 0)//如果为传送线1
                        {
                            HOperatorSet.ClearWindow(picConveyor1BarcodeScanResultImageDisp.HalconWindow);
                            HalconCameraControl.DispImageAdaptively(picConveyor1BarcodeScanResultImageDisp, _backCameraCs.ho_Image, false);
                        }
                        else
                        {
                            HOperatorSet.ClearWindow(picConveyor2BarcodeScanResultImageDisp.HalconWindow);
                            HalconCameraControl.DispImageAdaptively(picConveyor2BarcodeScanResultImageDisp, _backCameraCs.ho_Image, false);
                        }

                        //置报警信息为ON
                        if (conveyorIndex == 0)//如果为传送线1
                            _stateAndAlarmCs._alarmFlag[350] = true;
                        else
                            _stateAndAlarmCs._alarmFlag[351] = true;

                        //弹出二维码手动输入框
                        _barcodeRecheckGiveUpFlag = false;//如果点击了取消按钮，这个FLAG置为ON
                        this.Invoke(new Action(() =>
                        {
                            _barcodeRecheckFormVariate = new RecheckOrChangeBarcodeForm(_tempBarcode);
                            _barcodeRecheckFormVariate.RecheckOrChangeBarcodeFormEnsureEvent += RecheckOrChangeBarcodeFormEnsureEventFunc;
                            _barcodeRecheckFormVariate.RecheckOrChangeBarcodeFormCancelEvent += RecheckOrChangeBarcodeFormCancelEvent;
                            _barcodeRecheckFormVariate.Focus();
                            _barcodeRecheckFormVariate.StartPosition = FormStartPosition.CenterParent;
                            _barcodeRecheckFormVariate.Show();
                        })); ;

                        while (_barcodeRecheckFormVariate != null || _stateAndAlarmCs._alarmFlag[350] || _stateAndAlarmCs._alarmFlag[351] ||
                              (conveyorIndex == 0 && (_conveyor1HaveExcessProductAlarmDealWithWayIndex == 1 || _stateAndAlarmCs._alarmFlag[400])) ||
                              (conveyorIndex == 1 && (_conveyor2HaveExcessProductAlarmDealWithWayIndex == 1 || _stateAndAlarmCs._alarmFlag[401])))//等待清除报警以及点击提示框
                        {
                            if (_deadlyAlarmOccurFlag || !_autoModeFlag ||
                                (conveyorIndex == 0 && _conveyor1HaveExcessProductAlarmDealWithWayIndex == 1) ||
                                (conveyorIndex == 1 && _conveyor2HaveExcessProductAlarmDealWithWayIndex == 1))//如果触发了紧急停止
                            {
                                _barcodeScanModuleActionFlag = -1;
                                return;
                            }
                            Thread.Sleep(1);
                        }

                        //如果没有点击取消按钮，那么就算是人工输入二维码
                        if (!_barcodeRecheckGiveUpFlag)
                        {
                            requireBarcodeResult = true;
                            _barcodeData[conveyorIndex] = _tempBarcode;
                            _mainFormVariate.Invoke(new Action(() =>
                            {
                                if (conveyorIndex == 0)
                                    _mainFormVariate.lblConveyor1Barcode.Text = _tempBarcode;
                                else
                                    _mainFormVariate.lblConveyor2Barcode.Text = _tempBarcode;
                            })); ;
                        }

                        //显示二维码图片
                        if (_backCameraCs.ho_Image != null)
                        {
                            if (conveyorIndex == 0)
                            {
                                HOperatorSet.ClearWindow(picConveyor1BarcodeScanResultImageDisp.HalconWindow);
                                HalconCameraControl.DispImageAdaptively(picConveyor1BarcodeScanResultImageDisp,
                                    _backCameraCs.ho_Image, false);
                            }
                            else
                            {
                                HOperatorSet.ClearWindow(picConveyor2BarcodeScanResultImageDisp.HalconWindow);
                                HalconCameraControl.DispImageAdaptively(picConveyor2BarcodeScanResultImageDisp,
                                _backCameraCs.ho_Image, false);
                            }
                        }
                    }
                }

                //如果获取二维码成功
                if (requireBarcodeResult)
                {
                    if (conveyorIndex == 0)
                        MyTool.TxtFileProcess.CreateLog("设备动作-设备扫描传送线" + (conveyorIndex + 1).ToString() + "上，产品“"
                        + SettingsProductForm._track1ActualsProductName + "”的条形码成功，条形码值为：" + _tempBarcode + "----用户：" + _operatorName,
                         BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                    else
                        MyTool.TxtFileProcess.CreateLog("设备动作-设备扫描传送线" + (conveyorIndex + 1).ToString() + "上，产品“"
                        + SettingsProductForm._track2ActualsProductName + "”的条形码成功，条形码值为：" + _tempBarcode + "----用户：" + _operatorName,
                        BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

                    //判定此二维码的产品是否是被屏蔽的产品
                    if ((_barcodeData[conveyorIndex] == _productParameter[conveyorIndex]._shieldProduct1Barcode && _productParameter[conveyorIndex]._shieldProduct1Barcode != "") ||
                        (_barcodeData[conveyorIndex] == _productParameter[conveyorIndex]._shieldProduct2Barcode && _productParameter[conveyorIndex]._shieldProduct2Barcode != "") ||
                        (_barcodeData[conveyorIndex] == _productParameter[conveyorIndex]._shieldProduct3Barcode && _productParameter[conveyorIndex]._shieldProduct3Barcode != ""))
                    {

                        if (conveyorIndex == 0)
                            MyTool.TxtFileProcess.CreateLog("设备动作-传送线" + (conveyorIndex + 1).ToString() + "上，产品“"
                            + SettingsProductForm._track1ActualsProductName + "”的条形码与屏蔽产品条形码一致，产品不测试跳过！条形码值为：" + _tempBarcode + "----用户：" + _operatorName,
                             BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                        else
                            MyTool.TxtFileProcess.CreateLog("设备动作-传送线" + (conveyorIndex + 1).ToString() + "上，产品“"
                            + SettingsProductForm._track2ActualsProductName + "”的条形码与屏蔽产品条形码一致，产品不测试跳过！条形码值为：" + _tempBarcode + "----用户：" + _operatorName,
                             BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

                        if (conveyorIndex == 0 && _conveyor1HaveExcessProductAlarmDealWithWayIndex != 1)
                            _conveyor1Status = 2;
                        else if (conveyorIndex == 1 && _conveyor2HaveExcessProductAlarmDealWithWayIndex != 1)
                            _conveyor2Status = 2;
                    }
                    else//如果不是被屏蔽的产品，往下进行
                    {
                         //判定此二维码的产品是否是样品板
                        if ((_barcodeData[conveyorIndex] == _productParameter[conveyorIndex]._sampleProduct1Barcode && _productParameter[conveyorIndex]._sampleProduct1Barcode != "") ||
                            (_barcodeData[conveyorIndex] == _productParameter[conveyorIndex]._sampleProduct2Barcode && _productParameter[conveyorIndex]._sampleProduct2Barcode != "") ||
                            (_barcodeData[conveyorIndex] == _productParameter[conveyorIndex]._sampleProduct3Barcode && _productParameter[conveyorIndex]._sampleProduct3Barcode != ""))//如果当前产品是样品板
                        {
                            if (conveyorIndex == 0 && _conveyor1HaveExcessProductAlarmDealWithWayIndex != 1)
                                _conveyor1Status = 102;
                            else if (conveyorIndex == 1 && _conveyor2HaveExcessProductAlarmDealWithWayIndex != 1)
                                _conveyor2Status = 102;
                        }
                        else//如果当前产品不是样品板，那么判定样品板检测是否超时，借此判定是否需要停机
                        {
                            #region 检测当前样品板检测是否超时

                            //获取当前时间
                            string currentTime = DateTime.Now.ToString("yyyyMMddHHmm");//此处使用这种方式是为了确保不会出现极端的时间和年月日不符的情况。
                            int currentTimeYear = Convert.ToInt32(currentTime.Substring(0, 4));
                            int currentTimeMonth = Convert.ToInt32(currentTime.Substring(4, 2));
                            int currentTimeDay = Convert.ToInt32(currentTime.Substring(6, 2));
                            int currentTimeHour = Convert.ToInt32(currentTime.Substring(8, 2));
                            int currentTimeMinutes = Convert.ToInt32(currentTime.Substring(10, 2));

                            bool samplePanelCheckResult = false;
                            int samplePanelCheckTimeYear = 0;
                            int samplePanelCheckTimeMonth = 0;
                            int samplePanelCheckTimeDay = 0;
                            int samplePanelCheckTimeHour = 0;
                            int samplePanelCheckTimeMinutes = 0;


                            int productShiftTimeYear = 0;
                            int productShiftTimeMonth = 0;
                            int productShiftTimeDay = 0;
                            int productShiftTimeHour = 0;
                            int productShiftTimeMinutes = 0;
                            //警告截至时间
                            int warningEndYear, warningEndMonth, warningEndDay, warningEndHour, warningEndMinutes;

                            /////获取警告截至时间/////
                            //如果当前是白班
                            if ((currentTimeHour > SettingsGeneralForm._clearYieldTimeHour || (currentTimeHour == SettingsGeneralForm._clearYieldTimeHour && currentTimeMinutes >= SettingsGeneralForm._clearYieldTimeMinutes))
                                && (currentTimeHour < SettingsGeneralForm._clearYieldTimeHour + 12 || (currentTimeHour == SettingsGeneralForm._clearYieldTimeHour + 12 && currentTimeMinutes < SettingsGeneralForm._clearYieldTimeMinutes)))
                            {
                                MyTool.Other.TimeAddition(currentTimeYear, currentTimeMonth, currentTimeDay,
                                SettingsGeneralForm._clearYieldTimeHour, SettingsGeneralForm._clearYieldTimeMinutes, SettingsGeneralForm._warningTimeAfterShiftWithoutSampleCheck,
                                out warningEndYear, out warningEndMonth, out warningEndDay, out warningEndHour, out warningEndMinutes);
                            }
                            else//如果为夜班
                            {
                                if (currentTimeHour <= SettingsGeneralForm._clearYieldTimeHour)//如果小于换班时间，表明已经换了一天了
                                {
                                    int tempYesterdayYear, tempYesterdayMonth, tempYesterdayDay;
                                    MyTool.Other.YesterdayCalculate(currentTimeYear, currentTimeMonth, currentTimeDay,
                                        out tempYesterdayYear, out tempYesterdayMonth, out tempYesterdayDay);//获取上一天的日期

                                    MyTool.Other.TimeAddition(tempYesterdayYear, tempYesterdayMonth, tempYesterdayDay,
                                    SettingsGeneralForm._clearYieldTimeHour + 12, SettingsGeneralForm._clearYieldTimeMinutes, SettingsGeneralForm._warningTimeAfterShiftWithoutSampleCheck,
                                    out warningEndYear, out warningEndMonth, out warningEndDay, out warningEndHour, out warningEndMinutes);
                                }
                                else
                                {
                                    MyTool.Other.TimeAddition(currentTimeYear, currentTimeMonth, currentTimeDay,
                                    SettingsGeneralForm._clearYieldTimeHour + 12, SettingsGeneralForm._clearYieldTimeMinutes, SettingsGeneralForm._warningTimeAfterShiftWithoutSampleCheck,
                                    out warningEndYear, out warningEndMonth, out warningEndDay, out warningEndHour, out warningEndMinutes);
                                }
                            }

                            //获取上次切换产品的时间
                            ProductShiftTimeLoadFunc(conveyorIndex, out productShiftTimeYear, out productShiftTimeMonth, out productShiftTimeDay, out productShiftTimeHour, out productShiftTimeMinutes);
                            //获取记录的上次样品板检测的时间及状态
                            bool samplePanelCheckStatus = SamplePanelCheckTimeAndStatusLoadFunc(conveyorIndex, out samplePanelCheckResult, out samplePanelCheckTimeYear,
                                 out samplePanelCheckTimeMonth, out samplePanelCheckTimeDay, out samplePanelCheckTimeHour, out samplePanelCheckTimeMinutes);

                            //判断是否超时
                            if (((!samplePanelCheckStatus && MyTool.Other.CompareTime(currentTimeYear, currentTimeMonth, currentTimeDay, currentTimeHour, currentTimeMinutes,
                                warningEndYear, warningEndMonth, warningEndDay, warningEndHour, warningEndMinutes) != 1) ||
                                MyTool.Other.CompareTime(productShiftTimeYear, productShiftTimeMonth, productShiftTimeDay, productShiftTimeHour, productShiftTimeMinutes,
                                samplePanelCheckTimeYear, samplePanelCheckTimeMonth, samplePanelCheckTimeDay, samplePanelCheckTimeHour, samplePanelCheckTimeMinutes) != 1
                                ) && !SettingsGeneralForm._samplePanelCheckShieldFlag)//如果没有验证，并且当前时间超过警告上限
                            {

                                MyTool.TxtFileProcess.CreateLog("设备动作-传送线" + (conveyorIndex + 1).ToString() + "产品“"
                                 + SettingsProductForm._track1ActualsProductName + "”样品板检测时限已过，无法开始测试样品！" + "----用户：" + _operatorName,
                                BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

                                _stateAndAlarmCs._alarmFlag[410 + conveyorIndex] = true;

                                //显示消息提示框
                                this.Invoke(new Action(() =>
                                {
                                    if (_informVariate == null)
                                    {
                                        _informVariate = new InfoForm();
                                        _informVariate.ClickEnsureEvent += InformationFormClickEnsureButtonEventFunc;
                                        _informVariate.ClickOutEvent += InformationFormClickOutButtonEventFunc;
                                        _informVariate.ClickTakeOutEvent += InformationFormClickTakeOutButtonEventFunc;
                                        _informVariate.StartPosition = FormStartPosition.CenterScreen;
                                        _informVariate.Focus();
                                        _informVariate.Show();
                                    }
                                    _informVariate.Text = "样品板检测超时报警";
                                    _informVariate.txtInfoMessageDisp.Text = _stateAndAlarmCs._alarmMsg[410 + conveyorIndex];

                                    _informVariate.btnOK.Visible = false;
                                    _informVariate.btnOut.Visible = true;
                                    _informVariate.btnReleaseAndTakeOut.Visible = true;
                                    _informclickBtnIndex = -1;
                                })); ;

                                while (_stateAndAlarmCs._alarmFlag[410] || _stateAndAlarmCs._alarmFlag[411] || _informclickBtnIndex == -1 || !_deviceWorkTriggerFlag ||
                                       (conveyorIndex == 0 && (_conveyor1HaveExcessProductAlarmDealWithWayIndex == 1 || _stateAndAlarmCs._alarmFlag[400])) ||
                                       (conveyorIndex == 1 && (_conveyor2HaveExcessProductAlarmDealWithWayIndex == 1 || _stateAndAlarmCs._alarmFlag[401])))//如果没有点击任何按钮，并且处报警阶段，等待
                                {
                                    if (_deadlyAlarmOccurFlag || !_autoModeFlag ||
                                        (conveyorIndex == 0 && _conveyor1HaveExcessProductAlarmDealWithWayIndex == 1) ||
                                        (conveyorIndex == 1 && _conveyor2HaveExcessProductAlarmDealWithWayIndex == 1))
                                    {
                                        _barcodeScanModuleActionFlag = -1;
                                        return;
                                    }
                                    Thread.Sleep(10);
                                }

                                if (_informclickBtnIndex == 1)//如果为流出按钮
                                {
                                    if (conveyorIndex == 0 && _conveyor1HaveExcessProductAlarmDealWithWayIndex != 1)
                                    {
                                        _conveyor1Status = 2;
                                    }
                                    else if (conveyorIndex == 1 && _conveyor2HaveExcessProductAlarmDealWithWayIndex != 1)
                                    {
                                        _conveyor2Status = 2;
                                    }
                                    _barcodeScanModuleActionFlag = -1;
                                    return;
                                }
                                else//如果为人工取出按钮
                                {
                                    if (conveyorIndex == 0 && _conveyor1HaveExcessProductAlarmDealWithWayIndex != 1)
                                    {
                                        _conveyor1HaveProductFlag = false;
                                        _conveyor1Status = 0;
                                    }
                                    else if (conveyorIndex == 1 && _conveyor2HaveExcessProductAlarmDealWithWayIndex != 1)
                                    {
                                        _conveyor2HaveProductFlag = false;
                                        _conveyor2Status = 0;
                                    }
                                    _barcodeScanModuleActionFlag = -1;
                                    return;
                                }
                            }
                            else//如果没有超时
                            {
                                if (conveyorIndex == 0 && _conveyor1HaveExcessProductAlarmDealWithWayIndex != 1)
                                    _conveyor1Status = 102;
                                else if (conveyorIndex == 1 && _conveyor2HaveExcessProductAlarmDealWithWayIndex != 1)
                                    _conveyor2Status = 102;
                            }

                            #endregion
                        }

                    }
                    _barcodeScanModuleActionFlag = 1;
                }
                else//如果获取二维码失败
                {
                    if (conveyorIndex == 0)//如果为传送线1
                        _stateAndAlarmCs._alarmFlag[352] = true;
                    else
                        _stateAndAlarmCs._alarmFlag[353] = true;

                    if (conveyorIndex == 0)
                        MyTool.TxtFileProcess.CreateLog("设备动作-设备扫描传送线" + (conveyorIndex + 1).ToString() + "上，产品“"
                         + SettingsProductForm._track1ActualsProductName + "”的条形码失败，取走产品" + "----用户：" + _operatorName,
                        BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                    else
                        MyTool.TxtFileProcess.CreateLog("设备动作-设备扫描传送线" + (conveyorIndex + 1).ToString() + "上，产品“"
                        + SettingsProductForm._track2ActualsProductName + "”的条形码失败，取走产品" + "----用户：" + _operatorName,
                        BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

                    //显示消息提示框
                    this.Invoke(new Action(() =>
                    {
                        if (_informVariate == null)
                        {
                            _informVariate = new InfoForm();
                            _informVariate.ClickEnsureEvent += InformationFormClickEnsureButtonEventFunc;
                            _informVariate.ClickOutEvent += InformationFormClickOutButtonEventFunc;
                            _informVariate.ClickTakeOutEvent += InformationFormClickTakeOutButtonEventFunc;
                            _informVariate.StartPosition = FormStartPosition.CenterScreen;
                            _informVariate.Focus();
                            _informVariate.Show();
                        }
                        _informVariate.Text = "扫码失败报警！";
                        if (conveyorIndex == 0)
                            _informVariate.txtInfoMessageDisp.Text = _stateAndAlarmCs._alarmMsg[352];
                        else
                            _informVariate.txtInfoMessageDisp.Text = _stateAndAlarmCs._alarmMsg[353];

                        _informVariate.btnOK.Visible = false;
                        _informVariate.btnOut.Visible = true;
                        _informVariate.btnReleaseAndTakeOut.Visible = true;
                        _informclickBtnIndex = -1;
                    })); ;

                    while (_stateAndAlarmCs._alarmFlag[352] || _stateAndAlarmCs._alarmFlag[353] || _informclickBtnIndex == -1 || !_deviceWorkTriggerFlag ||
                           (conveyorIndex == 0 && (_conveyor1HaveExcessProductAlarmDealWithWayIndex == 1 || _stateAndAlarmCs._alarmFlag[400])) ||
                           (conveyorIndex == 1 && (_conveyor2HaveExcessProductAlarmDealWithWayIndex == 1 || _stateAndAlarmCs._alarmFlag[401])))//如果没有点击任何按钮，并且处报警阶段，等待
                    {
                        if (_deadlyAlarmOccurFlag || !_autoModeFlag ||
                            (conveyorIndex == 0 && _conveyor1HaveExcessProductAlarmDealWithWayIndex == 1) ||
                            (conveyorIndex == 1 && _conveyor2HaveExcessProductAlarmDealWithWayIndex == 1))
                        {
                            _barcodeScanModuleActionFlag = -1;
                            return;
                        }
                        Thread.Sleep(10);
                    }

                    if (_informclickBtnIndex == 1)//如果为流出按钮
                    {
                        if (conveyorIndex == 0 && _conveyor1HaveExcessProductAlarmDealWithWayIndex != 1)
                        {
                            _conveyor1Status = 2;
                        }
                        else if(conveyorIndex == 1 && _conveyor2HaveExcessProductAlarmDealWithWayIndex != 1)
                        {
                            _conveyor2Status = 2;
                        }
                    }
                    else//如果为人工取出按钮
                    {
                        if (conveyorIndex == 0 && _conveyor1HaveExcessProductAlarmDealWithWayIndex != 1)
                        {
                            _conveyor1HaveProductFlag = false;
                            _conveyor1Status = 0;
                        }
                        else if (conveyorIndex == 1 && _conveyor2HaveExcessProductAlarmDealWithWayIndex != 1)
                        {
                            _conveyor2HaveProductFlag = false;
                            _conveyor2Status = 0;
                        }
                    }
                    _barcodeScanModuleActionFlag = 2;
                }
            });
        }

        /// <summary>
        /// RecheckOrChangeBarcodeFormEnsureEventFunc：条形码二次检测确定按钮事件
        /// </summary>
        /// <param name="newBarcode"></param>
        private void RecheckOrChangeBarcodeFormEnsureEventFunc(string newBarcode)
        {
            _barcodeRecheckGiveUpFlag = false;
            _tempBarcode = newBarcode;
            if (_barcodeRecheckFormVariate != null)
            {
                _barcodeRecheckFormVariate.Dispose();
                _barcodeRecheckFormVariate = null;
            }
        }

        /// <summary>
        /// RecheckOrChangeBarcodeFormCancelEvent：条形码二次检测取消按钮事件
        /// </summary>
        private void RecheckOrChangeBarcodeFormCancelEvent()
        {
            _barcodeRecheckGiveUpFlag = true;
            if (_barcodeRecheckFormVariate != null)
            {
                _barcodeRecheckFormVariate.Dispose();
                _barcodeRecheckFormVariate = null;
            }
        }

        /// <summary>
        /// GetOrSendProductInfoFromMESModule:从MES系统中获取数据或传输数据至MES系统
        /// </summary>
        /// <param name="productBarcode">string:产品的条形码或二维码数据</param>
        /// <param name="getOrSendInfoFlag">bool:获取或传输数据至MES标志，false-从mes获取数据，true-传输数据至mes
        /// <param name="conveyorIndex">int:传送线索引,0-传送线1，1-传送线2</param>
        /// <param name="resultDT">DataTable:记录了产品检测结果的datatable文件</param>
        /// </param>
        private async void GetOrSendProductInfoFromMESModule(string productBarcode, bool getOrSendInfoFlag, int conveyorIndex, DataTable resultDT)
        {
            await Task.Run(() =>
            {
                _getOrSendProductInfoFromMESModuleActionFlag = 0;

                string result = "000";
                if (!getOrSendInfoFlag)//如果需要从MES获取数据
                {
                    if (!_baseSettingFormVariate._settingsGeneralFormVariate.chkShieldMES.Checked)//如果没有屏蔽
                    {
                        if (conveyorIndex == 0)
                            MyTool.TxtFileProcess.CreateLog("设备动作-传送线" + (conveyorIndex + 1).ToString() + "上的产品“" + SettingsProductForm._track1ActualsProductName +
                            "(" + productBarcode + ")”入站扫描完毕请求向MES发送入站请求" + "----用户：" + _operatorName,
                            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                        else
                            MyTool.TxtFileProcess.CreateLog("设备动作-传送线" + (conveyorIndex + 1).ToString() + "上的产品“" + SettingsProductForm._track2ActualsProductName +
                            "(" + productBarcode + ")”入站扫描完毕请求向MES发送入站请求" + "----用户：" + _operatorName,
                            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                    }

                    while (!((!_baseSettingFormVariate._settingsGeneralFormVariate.chkShieldMES.Checked && GetDataFromMES(productBarcode, conveyorIndex, out result)) ||//"2022022406483104525Y"
                       _baseSettingFormVariate._settingsGeneralFormVariate.chkShieldMES.Checked))//如果MES通讯成功
                    {
                        _stateAndAlarmCs._alarmFlag[370] = true;

                        if (conveyorIndex == 0)
                            MyTool.TxtFileProcess.CreateLog("设备动作-传送线" + (conveyorIndex + 1).ToString() + "上的产品“" + SettingsProductForm._track1ActualsProductName +
                            "(" + productBarcode + ")”入站扫描完毕请求向MES发送入站请求失败(" + result + ")" + "----用户：" + _operatorName,
                            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                        else
                            MyTool.TxtFileProcess.CreateLog("设备动作-传送线" + (conveyorIndex + 1).ToString() + "上的产品“" + SettingsProductForm._track2ActualsProductName +
                            "(" + productBarcode + ")”入站扫描完毕请求向MES发送入站请求失败(" + result + ")" + "----用户：" + _operatorName,
                            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

                        //显示消息提示框
                        this.Invoke(new Action(() =>
                        {
                            if (_informVariate == null)
                            {
                                _informVariate = new InfoForm();
                                _informVariate.ClickEnsureEvent += InformationFormClickEnsureButtonEventFunc;
                                _informVariate.ClickOutEvent += InformationFormClickOutButtonEventFunc;
                                _informVariate.ClickTakeOutEvent += InformationFormClickTakeOutButtonEventFunc;
                                _informVariate.StartPosition = FormStartPosition.CenterScreen;
                                _informVariate.Show();
                                _informVariate.Focus();
                            }
                            _informVariate.Text = "入站MES通讯失败报警！";
                            _informVariate.txtInfoMessageDisp.Text = result;
                            _informVariate.btnOK.Visible = true;
                            _informVariate.btnOut.Visible = true;
                            _informVariate.btnReleaseAndTakeOut.Visible = true;
                            _informclickBtnIndex = -1;
                        })); ;

                        while (_stateAndAlarmCs._alarmFlag[370] || _informclickBtnIndex == -1 || !_deviceWorkTriggerFlag ||
                           (conveyorIndex == 0 && (_conveyor1HaveExcessProductAlarmDealWithWayIndex == 1 || _stateAndAlarmCs._alarmFlag[400])) ||
                           (conveyorIndex == 1 && (_conveyor2HaveExcessProductAlarmDealWithWayIndex == 1 || _stateAndAlarmCs._alarmFlag[401])))//如果没有点击任何按钮，并且处报警阶段，等待
                        {
                            if (_deadlyAlarmOccurFlag || !_autoModeFlag ||
                               (conveyorIndex == 0 && _conveyor1HaveExcessProductAlarmDealWithWayIndex == 1) ||
                               (conveyorIndex == 1 && _conveyor2HaveExcessProductAlarmDealWithWayIndex == 1))
                            {
                                _getOrSendProductInfoFromMESModuleActionFlag = -1;
                                return;
                            }
                            Thread.Sleep(10);
                        }

                        if (_informclickBtnIndex == 1)//如果选择流出
                        {
                            if (conveyorIndex == 0)
                                MyTool.TxtFileProcess.CreateLog("设备动作-传送线" + (conveyorIndex + 1).ToString() + "上的产品“" + SettingsProductForm._track1ActualsProductName +
                                "(" + productBarcode + ")”入站扫描完毕请求向MES发送入站请求失败(" + result + ")" + "选择流出按钮！" + "----用户：" + _operatorName,
                                BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                            else
                                MyTool.TxtFileProcess.CreateLog("设备动作-传送线" + (conveyorIndex + 1).ToString() + "上的产品“" + SettingsProductForm._track2ActualsProductName +
                                "(" + productBarcode + ")”入站扫描完毕请求向MES发送入站请求失败(" + result + ")" + "选择流出按钮！" + "----用户：" + _operatorName,
                                BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

                            if (conveyorIndex == 0 && _conveyor1HaveExcessProductAlarmDealWithWayIndex != 1)
                            {
                                _conveyor1Status = 2;
                            }
                            else if(conveyorIndex == 1 && _conveyor2HaveExcessProductAlarmDealWithWayIndex != 1)
                            {
                                _conveyor2Status = 2;
                            }
                            _getOrSendProductInfoFromMESModuleActionFlag = 2;
                            return;
                        }
                        else if (_informclickBtnIndex == 2)//如果选择人工取出
                        {
                            if (conveyorIndex == 0)
                                MyTool.TxtFileProcess.CreateLog("设备动作-传送线" + (conveyorIndex + 1).ToString() + "上的产品“" + SettingsProductForm._track1ActualsProductName +
                                "(" + productBarcode + ")”入站扫描完毕请求向MES发送入站请求失败(" + result + ")" + "人工取出产品！" + "----用户：" + _operatorName,
                                BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                            else
                                MyTool.TxtFileProcess.CreateLog("设备动作-传送线" + (conveyorIndex + 1).ToString() + "上的产品“" + SettingsProductForm._track2ActualsProductName +
                                "(" + productBarcode + ")”入站扫描完毕请求向MES发送入站请求失败(" + result + ")" + "人工取出产品！" + "----用户：" + _operatorName,
                                BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

                            if (conveyorIndex == 0 && _conveyor1HaveExcessProductAlarmDealWithWayIndex != 1)
                            {
                                _conveyor1HaveProductFlag = false;
                                _conveyor1Status = 0;
                            }
                            else if (conveyorIndex == 1 && _conveyor2HaveExcessProductAlarmDealWithWayIndex != 1)
                            {
                                _conveyor2HaveProductFlag = false;
                                _conveyor2Status = 0;
                            }
                            _getOrSendProductInfoFromMESModuleActionFlag = 2;
                            return;
                        }

                        Thread.Sleep(10);
                    }

                    if (!_baseSettingFormVariate._settingsGeneralFormVariate.chkShieldMES.Checked)
                    {
                        string tempStr = result.Substring(7, result.Length - 7 - 3);
                        if (_needInferOrNotDt != null)
                        {
                            _needInferOrNotDt.Dispose();
                            _needInferOrNotDt = null;
                        }
                        _needInferOrNotDt = new DataTable();
                        _needInferOrNotDt.Columns.Add("Number", typeof(string));
                        _needInferOrNotDt.Columns.Add("CheckFlag", typeof(string));

                        tempStr = tempStr.Replace("|", "");
                        string findResultStr;
                        int index = 0;
                        while (tempStr.Length > 0)
                        {
                            index++;
                            findResultStr = tempStr.Substring(0, 2);
                            tempStr = tempStr.Substring(2);
                            _needInferOrNotDt.Rows.Add();
                            _needInferOrNotDt.Rows[_needInferOrNotDt.Rows.Count - 1][0] = index.ToString();
                            _needInferOrNotDt.Rows[_needInferOrNotDt.Rows.Count - 1][1] = findResultStr;
                        }
                    }

                    if (conveyorIndex == 0)
                        MyTool.TxtFileProcess.CreateLog("设备动作-传送线" + (conveyorIndex + 1).ToString() + "上的产品“" + SettingsProductForm._track1ActualsProductName +
                        "(" + productBarcode + ")”入站扫描完毕请求向MES发送入站请求成功" + "----用户：" + _operatorName,
                        BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                    else
                        MyTool.TxtFileProcess.CreateLog("设备动作-传送线" + (conveyorIndex + 1).ToString() + "上的产品“" + SettingsProductForm._track2ActualsProductName +
                        "(" + productBarcode + ")”入站扫描完毕请求向MES发送入站请求成功" + "----用户：" + _operatorName,
                        BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

                    if (conveyorIndex == 0 && _conveyor1HaveExcessProductAlarmDealWithWayIndex != 1)
                        _conveyor1Status = 103;
                    else if (conveyorIndex == 1 && _conveyor2HaveExcessProductAlarmDealWithWayIndex != 1)
                        _conveyor2Status = 103;
                    _getOrSendProductInfoFromMESModuleActionFlag = 1;

                }
                else
                {
                    int totalPcsYield = 0;
                    int pcsOKYield = 0;
                    float currentAccessRate = 0;
                    bool jumpNextStep = false;//用于标识是否跳过接下来的动作，false-不跳过，true-跳过

                       //判定此二维码的产品是否是样品板
                    if ((_barcodeData[conveyorIndex] == _productParameter[conveyorIndex]._sampleProduct1Barcode && _productParameter[conveyorIndex]._sampleProduct1Barcode != "") ||
                        (_barcodeData[conveyorIndex] == _productParameter[conveyorIndex]._sampleProduct2Barcode && _productParameter[conveyorIndex]._sampleProduct2Barcode != "") ||
                        (_barcodeData[conveyorIndex] == _productParameter[conveyorIndex]._sampleProduct3Barcode && _productParameter[conveyorIndex]._sampleProduct3Barcode != ""))//如果当前产品是样品板
                    {
                        SamplePanelCheckTimeAndStatusSaveFunc(conveyorIndex, true);
                    }

                    lock (_yieldRecordDt)
                    {
                        totalPcsYield = Convert.ToInt32(_yieldRecordDt.Rows[0][1]);
                        pcsOKYield = Convert.ToInt32(_yieldRecordDt.Rows[1][1]);
                    }

                    if (totalPcsYield > 0)
                    {
                        currentAccessRate = (float)pcsOKYield / (float)totalPcsYield;
                        if (currentAccessRate < SettingsAdministratorForm._smallestAccessRate)//如果小于最小直通率
                        {
                            MyTool.TxtFileProcess.CreateLog("设备动作-当前设备直通率" + currentAccessRate.ToString("f4") + "低于设定的最低直通率"
                            + SettingsAdministratorForm._smallestAccessRate.ToString("f4") + "报警" + "----用户：" + _operatorName,
                            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

                            this.Invoke(new Action(() =>
                            {
                                if (_informVariate == null)
                                {
                                    _informVariate = new InfoForm();
                                    _informVariate.ClickEnsureEvent += InformationFormClickEnsureButtonEventFunc;
                                    _informVariate.ClickOutEvent += InformationFormClickOutButtonEventFunc;
                                    _informVariate.ClickTakeOutEvent += InformationFormClickTakeOutButtonEventFunc;
                                    _informVariate.StartPosition = FormStartPosition.CenterScreen;
                                    _informVariate.Focus();
                                    _informVariate.Show();
                                }
                                _informVariate.Text = "直通率报警！";
                                _informVariate.txtInfoMessageDisp.Text = "当前设备直通率" + currentAccessRate.ToString("f4") + "低于设定的最低直通率"
                                    + SettingsAdministratorForm._smallestAccessRate.ToString("f4") + "报警";
                                _informVariate.btnOK.Visible = false;
                                _informVariate.btnOut.Visible = false;
                                _informVariate.btnReleaseAndTakeOut.Visible = false;
                                _informclickBtnIndex = -1;
                                if (SettingsAdministratorForm._devicePauseWhenAccessRateOccurFlag)//如果选中出现直通率报警暂停
                                {
                                    _stateAndAlarmCs._alarmFlag[380] = true;//触发直通率报警
                                    _informVariate.btnOK.Visible = true;
                                    _informVariate.btnOut.Visible = true;
                                    _informVariate.btnReleaseAndTakeOut.Visible = true;
                                }
                                else
                                {
                                    _stateAndAlarmCs._alarmFlag[500] = true;//触发直通率报警
                                    _informVariate.btnOK.Visible = true;
                                }
                            })); ;

                            while ((_stateAndAlarmCs._alarmFlag[380] || _informclickBtnIndex == -1) && !_stateAndAlarmCs._alarmFlag[500] ||
                           (conveyorIndex == 0 && (_conveyor1HaveExcessProductAlarmDealWithWayIndex == 1 || _stateAndAlarmCs._alarmFlag[400])) ||
                           (conveyorIndex == 1 && (_conveyor2HaveExcessProductAlarmDealWithWayIndex == 1 || _stateAndAlarmCs._alarmFlag[401])))//如果没有点击任何按钮，并且处于暂停阶段时
                            {
                                if (_deadlyAlarmOccurFlag || !_autoModeFlag ||
                                   (conveyorIndex == 0 && _conveyor1HaveExcessProductAlarmDealWithWayIndex == 1) ||
                                   (conveyorIndex == 1 && _conveyor2HaveExcessProductAlarmDealWithWayIndex == 1))
                                {
                                    _getOrSendProductInfoFromMESModuleActionFlag = -1;
                                    return;
                                }
                                Thread.Sleep(10);
                            }

                            if (_informclickBtnIndex == 2 || _informclickBtnIndex == 1)//如果为人工取出或流出产品
                            {
                                jumpNextStep = true;
                            }
                        }
                    }

                    if (!jumpNextStep)
                    {
                        if (!_baseSettingFormVariate._settingsGeneralFormVariate.chkShieldMES.Checked)//如果没有屏蔽
                        {
                            if (conveyorIndex == 0)
                                MyTool.TxtFileProcess.CreateLog("设备动作-传送线" + (conveyorIndex + 1).ToString() + "上的产品“" + SettingsProductForm._track1ActualsProductName +
                                "(" + productBarcode + ")”检测完毕，请求向MES发送检测结果数据" + "----用户：" + _operatorName,
                                BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                            else
                                MyTool.TxtFileProcess.CreateLog("设备动作-传送线" + (conveyorIndex + 1).ToString() + "上的产品“" + SettingsProductForm._track2ActualsProductName +
                                "(" + productBarcode + ")”检测完毕，请求向MES发送检测结果数据" + "----用户：" + _operatorName,
                                BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                        }

                        while (!((!_baseSettingFormVariate._settingsGeneralFormVariate.chkShieldMES.Checked && SendDataToMES(productBarcode, conveyorIndex, resultDT, out result)) ||
                           _baseSettingFormVariate._settingsGeneralFormVariate.chkShieldMES.Checked))//如果MES通讯成功
                        {
                            _stateAndAlarmCs._alarmFlag[371] = true;

                            if (conveyorIndex == 0)
                                MyTool.TxtFileProcess.CreateLog("设备动作-传送线" + (conveyorIndex + 1).ToString() + "上的产品“" + SettingsProductForm._track1ActualsProductName +
                                "(" + productBarcode + ")”检测完毕，请求向MES发送检测结果数据失败（" + result + ")" + "----用户：" + _operatorName,
                                BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                            else
                                MyTool.TxtFileProcess.CreateLog("设备动作-传送线" + (conveyorIndex + 1).ToString() + "上的产品“" + SettingsProductForm._track2ActualsProductName +
                                "(" + productBarcode + ")”检测完毕，请求向MES发送检测结果数据失败（" + result + ")" + "----用户：" + _operatorName,
                                BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

                            //显示消息提示框
                            this.Invoke(new Action(() =>
                            {
                                if (_informVariate == null)
                                {
                                    _informVariate = new InfoForm();
                                    _informVariate.ClickEnsureEvent += InformationFormClickEnsureButtonEventFunc;
                                    _informVariate.ClickOutEvent += InformationFormClickOutButtonEventFunc;
                                    _informVariate.ClickTakeOutEvent += InformationFormClickTakeOutButtonEventFunc;
                                    _informVariate.StartPosition = FormStartPosition.CenterScreen;
                                    _informVariate.Focus();
                                    _informVariate.Show();
                                }
                                _informVariate.Text = "检测完毕，传输检测结果给MES通讯失败报警！";
                                _informVariate.txtInfoMessageDisp.Text = result;
                                _informVariate.btnOK.Visible = true;
                                _informVariate.btnOut.Visible = true;
                                _informVariate.btnReleaseAndTakeOut.Visible = true;
                                _informclickBtnIndex = -1;
                            })); ;

                            while (_stateAndAlarmCs._alarmFlag[371] || _informclickBtnIndex == -1 || !_deviceWorkTriggerFlag ||
                           (conveyorIndex == 0 && (_conveyor1HaveExcessProductAlarmDealWithWayIndex == 1 || _stateAndAlarmCs._alarmFlag[400])) ||
                           (conveyorIndex == 1 && (_conveyor2HaveExcessProductAlarmDealWithWayIndex == 1 || _stateAndAlarmCs._alarmFlag[401])))//如果没有点击任何按钮，并且处报警阶段，等待
                            {
                                if (_deadlyAlarmOccurFlag || !_autoModeFlag ||
                                   (conveyorIndex == 0 && _conveyor1HaveExcessProductAlarmDealWithWayIndex == 1) ||
                                   (conveyorIndex == 1 && _conveyor2HaveExcessProductAlarmDealWithWayIndex == 1))
                                {
                                    _getOrSendProductInfoFromMESModuleActionFlag = -1;
                                    return;
                                }
                                Thread.Sleep(10);
                            }


                            if (_informclickBtnIndex == 1)//如果选择流出
                            {
                                if (conveyorIndex == 0)
                                    MyTool.TxtFileProcess.CreateLog("设备动作-传送线" + (conveyorIndex + 1).ToString() + "上的产品“" + SettingsProductForm._track1ActualsProductName +
                                    "(" + productBarcode + ")”检测完毕，请求向MES发送检测结果数据失败（" + result + ")" + "选择流出产品" + "----用户：" + _operatorName,
                                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                                else
                                    MyTool.TxtFileProcess.CreateLog("设备动作-传送线" + (conveyorIndex + 1).ToString() + "上的产品“" + SettingsProductForm._track2ActualsProductName +
                                    "(" + productBarcode + ")”检测完毕，请求向MES发送检测结果数据失败（" + result + ")" + "选择流出产品" + "----用户：" + _operatorName,
                                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

                                if (conveyorIndex == 0 && _conveyor1HaveExcessProductAlarmDealWithWayIndex != 1)
                                {
                                    _conveyor1Status = 2;
                                }
                                else if (conveyorIndex == 1 && _conveyor2HaveExcessProductAlarmDealWithWayIndex != 1)
                                {
                                    _conveyor2Status = 2;
                                }

                                _getOrSendProductInfoFromMESModuleActionFlag = 12;
                                return;
                            }
                            else if (_informclickBtnIndex == 2)//如果选择人工取出
                            {
                                if (conveyorIndex == 0)
                                    MyTool.TxtFileProcess.CreateLog("设备动作-传送线" + (conveyorIndex + 1).ToString() + "上的产品“" + SettingsProductForm._track1ActualsProductName +
                                    "(" + productBarcode + ")”检测完毕，请求向MES发送检测结果数据失败（" + result + ")" + "人工取走产品！" + "----用户：" + _operatorName,
                                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                                else
                                    MyTool.TxtFileProcess.CreateLog("设备动作-传送线" + (conveyorIndex + 1).ToString() + "上的产品“" + SettingsProductForm._track2ActualsProductName +
                                    "(" + productBarcode + ")”检测完毕，请求向MES发送检测结果数据失败（" + result + ")" + "人工取走产品！" + "----用户：" + _operatorName,
                                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

                                if (conveyorIndex == 0 && _conveyor1HaveExcessProductAlarmDealWithWayIndex != 1)
                                {
                                    _conveyor1HaveProductFlag = false;
                                    _conveyor1Status = 0;
                                }
                                else if (conveyorIndex == 1 && _conveyor2HaveExcessProductAlarmDealWithWayIndex != 1)
                                {
                                    _conveyor2HaveProductFlag = false;
                                    _conveyor2Status = 0;
                                }

                                _getOrSendProductInfoFromMESModuleActionFlag = 12;
                                return;
                            }

                            Thread.Sleep(10);
                        }

                        if (conveyorIndex == 0)
                            MyTool.TxtFileProcess.CreateLog("设备动作-传送线" + (conveyorIndex + 1).ToString() + "上的产品“" + SettingsProductForm._track1ActualsProductName +
                            "(" + productBarcode + ")”检测完毕，请求向MES发送检测结果数据成功" + "----用户：" + _operatorName,
                            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                        else
                            MyTool.TxtFileProcess.CreateLog("设备动作-传送线" + (conveyorIndex + 1).ToString() + "上的产品“" + SettingsProductForm._track2ActualsProductName +
                            "(" + productBarcode + ")”检测完毕，请求向MES发送检测结果数据成功" + "----用户：" + _operatorName,
                            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

                        if (conveyorIndex == 0 && _conveyor1HaveExcessProductAlarmDealWithWayIndex != 1)
                            _conveyor1Status = 106;
                        else if (conveyorIndex == 1 && _conveyor2HaveExcessProductAlarmDealWithWayIndex != 1)
                            _conveyor2Status = 106;

                        _getOrSendProductInfoFromMESModuleActionFlag = 11;

                    }
                    else
                    {
                        if (_informclickBtnIndex == 2)
                        {
                            MyTool.TxtFileProcess.CreateLog("设备动作-产品直通率低于设定下限，人工取出产品，跳过接下来的步骤！" + "----用户：" + _operatorName,
                            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

                            if (conveyorIndex == 0 && _conveyor1HaveExcessProductAlarmDealWithWayIndex != 1)
                                _conveyor1Status = 0;
                            else if (conveyorIndex == 1 && _conveyor2HaveExcessProductAlarmDealWithWayIndex != 1)
                                _conveyor2Status = 0;
                        }
                        else
                        {
                            MyTool.TxtFileProcess.CreateLog("设备动作-产品直通率低于设定下限，流出产品！" + "----用户：" + _operatorName,
                            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

                            if (conveyorIndex == 0 && _conveyor1HaveExcessProductAlarmDealWithWayIndex != 1)
                                _conveyor1Status = 2;
                            else if (conveyorIndex == 1 && _conveyor2HaveExcessProductAlarmDealWithWayIndex != 1)
                                _conveyor2Status = 2;
                        }

                        _getOrSendProductInfoFromMESModuleActionFlag = 12;
                    }
                }
            });
        }

        /// <summary>
        /// InformationFormClickEnsureButtonEventFunc：消息提示框点击确定按钮事件函数
        /// </summary>
        private void InformationFormClickEnsureButtonEventFunc()
        {
            if (_informVariate != null)
            {
                _informVariate.Invoke(new Action(() =>
                    {
                        _informVariate.Dispose();
                        _informVariate = null;
                    })); ;
            }
            _informclickBtnIndex = 0;
        }

        /// <summary>
        /// InformationFormClickOutButtonEventFunc：消息提示框点击流出按钮事件函数
        /// </summary>
        private void InformationFormClickOutButtonEventFunc()
        {
            if (_informVariate != null)
            {
                _informVariate.Invoke(new Action(() =>
                {
                    _informVariate.Dispose();
                    _informVariate = null;
                })); ;
            }
            _informclickBtnIndex = 1;
        }

        /// <summary>
        /// InformationFormClickTakeOutButtonEventFunc：消息提示框点击人工取出按钮事件函数
        /// </summary>
        private void InformationFormClickTakeOutButtonEventFunc()
        {
            if (_informVariate != null)
            {
                _informVariate.Invoke(new Action(() =>
                {
                    _informVariate.Dispose();
                    _informVariate = null;
                })); ;
            }
            //为了取走产品，打开夹紧气缸
            if (_conveyor1Status >= 100)//如果为传送线1
            {
                //控制夹紧气缸松开
                _axisControlFormVariate.OutputControl(82, true);
                _axisControlFormVariate.OutputControl(83, false);

                while (!_inputIOT[86].STATUS)//等待夹紧气缸松开
                {
                    if (_deadlyAlarmOccurFlag || !_autoModeFlag)
                    {
                        return;
                    }
                    Thread.Sleep(1);
                }
            }
            else
            {
                //控制夹紧气缸松开
                _axisControlFormVariate.OutputControl(78, true);
                _axisControlFormVariate.OutputControl(79, false);

                while (!_inputIOT[82].STATUS)//等待夹紧气缸松开
                {
                    if (_deadlyAlarmOccurFlag || !_autoModeFlag)
                    {
                        return;
                    }
                    Thread.Sleep(1);
                }
            }

            while ((_inputIOT[88].STATUS && _conveyor1Status >= 100) || (_inputIOT[89].STATUS && _conveyor2Status >= 100))//当取走产品之后，减速传感器应当OFF
            {
                //MessageBox.Show("请取走产品！");
                if (_deadlyAlarmOccurFlag || !_autoModeFlag)
                {
                    return;
                }
                Thread.Sleep(1);
            }

            _informclickBtnIndex = 2;
        }

       // Stopwatch markSearchSW = new Stopwatch();
        /// <summary>
        /// DeviceRecognizeMarkPositionAndGetCorrectMatrixModule:设备识别MARK点并计算出纠正矩阵模块
        /// </summary>
        /// <param name="conveyorIndex">int:传送线索引,0-传送线1，1-传送线2</param>
        private async void DeviceRecognizeMarkPositionAndGetCorrectMatrixModule(int conveyorIndex)
        {
            await Task.Run(() =>
            {


                _deviceRecognizeMarkPositionAndGetCorrectMatrixModuleActionFlag = 0;

                bool markSearchResult = false;
                float mark1RowCoor = 0f, mark1ColumnCoor = 0f;//MARK点1的坐标
                float mark2RowCoor = 0f, mark2ColumnCoor = 0f;//MARK点2的坐标
                HObject ho_returnRectangle = null;
                _markSearchXOffsetData = 0;
                _markSearchYOffsetData = 0;

                if (conveyorIndex == 0)
                    MyTool.TxtFileProcess.CreateLog("设备动作-识别传送线" + (conveyorIndex + 1).ToString() + "上的产品“" + SettingsProductForm._track1ActualsProductName + "”" +
                    "的Mark点并补正" + "----用户：" + _operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                else
                    MyTool.TxtFileProcess.CreateLog("设备动作-识别传送线" + (conveyorIndex + 1).ToString() + "上的产品“" + SettingsProductForm._track2ActualsProductName + "”" +
                    "的Mark点并补正" + "----用户：" + _operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

                if (!_axisControlFormVariate._ioStatus[1, BACK_CAMERA_LIGHT_TRIGGER_OUT_IO] || !_axisControlFormVariate._ioStatus[1, BACK_CAMERA_LIGHT_TRIGGER_OUT_IO2])//如果没有打开光源那么就打开光源
                {
                    _axisControlFormVariate.OutputControl(BACK_CAMERA_LIGHT_TRIGGER_OUT_IO, true);//打开光源
                    _axisControlFormVariate.OutputControl(BACK_CAMERA_LIGHT_TRIGGER_OUT_IO2, true);//打开光源
                }

                int takePictureOfMarkPosition = 4;
                if (conveyorIndex == 1)
                {
                    takePictureOfMarkPosition = 6;
                }

                while (_axisControlFormVariate._axisActionCommand[0] != -1 ||
                        _axisControlFormVariate._axisActionCommand[1] != -1 ||
                        _axisControlFormVariate._axisActionCommand[2] != -1 ||
                        !_deviceWorkTriggerFlag ||
                        (conveyorIndex == 0 && (_conveyor1HaveExcessProductAlarmDealWithWayIndex == 1 || _stateAndAlarmCs._alarmFlag[400])) ||
                        (conveyorIndex == 1 && (_conveyor2HaveExcessProductAlarmDealWithWayIndex == 1 || _stateAndAlarmCs._alarmFlag[401])))//当XY轴处于运动指令状态,循环等待
                {
                    if (_deadlyAlarmOccurFlag || !_autoModeFlag ||
                       (conveyorIndex == 0 && _conveyor1HaveExcessProductAlarmDealWithWayIndex == 1) ||
                       (conveyorIndex == 1 && _conveyor2HaveExcessProductAlarmDealWithWayIndex == 1))//如果触发了紧急停止
                    {
                        _deviceRecognizeMarkPositionAndGetCorrectMatrixModuleActionFlag = -1;
                        return;
                    }
                    Thread.Sleep(1);
                }

                //请求XYR轴去第一个mark点拍照位置
                //X1
                _axisControlFormVariate._axisCurrentPositionNumber[0] = 0;
                _axisControlFormVariate._axisActionCommand[0] = takePictureOfMarkPosition;
                //Y1
                _axisControlFormVariate._axisCurrentPositionNumber[1] = 0;
                _axisControlFormVariate._axisActionCommand[1] = takePictureOfMarkPosition;
                //R1
                _axisControlFormVariate._axisCurrentPositionNumber[2] = 0;
                _axisControlFormVariate._axisActionCommand[2] = takePictureOfMarkPosition;

                while ((_axisControlFormVariate._axisCurrentPositionNumber[0] == 0 || _axisControlFormVariate._axisActionCommand[0] == takePictureOfMarkPosition) ||
                    (_axisControlFormVariate._axisCurrentPositionNumber[1] == 0 || _axisControlFormVariate._axisActionCommand[1] == takePictureOfMarkPosition) ||
                    (_axisControlFormVariate._axisCurrentPositionNumber[2] == 0 || _axisControlFormVariate._axisActionCommand[2] == takePictureOfMarkPosition) ||
                    _axisControlFormVariate._axisMotionControlBoardVariate._axis00CheckDoneStatus == 0 ||
                    _axisControlFormVariate._axisMotionControlBoardVariate._axis01CheckDoneStatus == 0 ||
                    _axisControlFormVariate._axisMotionControlBoardVariate._axis02CheckDoneStatus == 0 ||
                    !_deviceWorkTriggerFlag ||
                    (conveyorIndex == 0 && (_conveyor1HaveExcessProductAlarmDealWithWayIndex == 1 || _stateAndAlarmCs._alarmFlag[400])) ||
                    (conveyorIndex == 1 && (_conveyor2HaveExcessProductAlarmDealWithWayIndex == 1 || _stateAndAlarmCs._alarmFlag[401])))//等待XY轴运行结束
                {
                    if (_deadlyAlarmOccurFlag || !_autoModeFlag ||
                       (conveyorIndex == 0 && _conveyor1HaveExcessProductAlarmDealWithWayIndex == 1) ||
                       (conveyorIndex == 1 && _conveyor2HaveExcessProductAlarmDealWithWayIndex == 1))//如果触发了紧急停止
                    {
                        _deviceRecognizeMarkPositionAndGetCorrectMatrixModuleActionFlag = -1;
                        return;
                    }
                    Thread.Sleep(1);
                }
                Thread.Sleep(100);

                /*
                * 第一个Mark点位拍照获取Mark点坐标
                */
                if (true)
                {
                    ////if (markSearchSW.IsRunning)
                    ////    markSearchSW.Reset();
                    ////else
                    ////    markSearchSW.Restart();
                    if (_backCameraCs.ho_Image != null)
                    {
                        _backCameraCs.ho_Image.Dispose();
                        _backCameraCs.ho_Image = null;
                    }

                    if (_backCameraCs.GrabImage2(false))
                    {
                        //lblMarkSearchTime.Invoke(new Action(() =>
                        //{
                        //    lblMarkSearchTime.Text = ((float)markSearchSW.ElapsedMilliseconds / 1000f).ToString("f3") + "s";
                        //    if (markSearchSW.IsRunning)
                        //        markSearchSW.Stop();
                        //})); ;

                        if (HalconPictureProcess.BlobGetCircleCenterPosition(ref _backCameraCs.ho_Image, _productParameter[conveyorIndex]._markSearchThresholdMin, _productParameter[conveyorIndex]._markSearchThresholdMax,
                            _productParameter[conveyorIndex]._markSelectShapeMin, _productParameter[conveyorIndex]._markSelectShapeMax, _productParameter[conveyorIndex]._markSearchMinCircularity, 2, out mark1RowCoor, out mark1ColumnCoor, out ho_returnRectangle) == 0)
                        {
                            try
                            {
                                if (conveyorIndex == 0)//如果为传送线1
                                {
                                    HOperatorSet.ClearWindow(picConveyor1Mark1SearchResultImageDisp.HalconWindow);
                                    HalconCameraControl.DispImageAdaptively(picConveyor1Mark1SearchResultImageDisp, _backCameraCs.ho_Image, false);//显示图像

                                    //显示设置
                                    HOperatorSet.SetColor(picConveyor1Mark1SearchResultImageDisp.HalconWindow, "green");
                                    HOperatorSet.SetDraw(picConveyor1Mark1SearchResultImageDisp.HalconWindow, "margin");
                                    HOperatorSet.SetLineWidth(picConveyor1Mark1SearchResultImageDisp.HalconWindow, 3);

                                    HOperatorSet.DispObj(ho_returnRectangle, picConveyor1Mark1SearchResultImageDisp.HalconWindow);//显示mark点区域
                                    HOperatorSet.DispCross(picConveyor1Mark1SearchResultImageDisp.HalconWindow, mark1RowCoor, mark1ColumnCoor, 20, 0);//显示十字线，标识mark点数据
                                    HOperatorSet.DispText(picConveyor1Mark1SearchResultImageDisp.HalconWindow, mark1RowCoor.ToString("f2") + ","
                                        + mark1ColumnCoor.ToString("f2"), "window", 0, 0, "green", new HTuple(), new HTuple());//显示坐标数据
                                }
                                else
                                {
                                    HOperatorSet.ClearWindow(picConveyor2Mark1SearchResultImageDisp.HalconWindow);
                                    HalconCameraControl.DispImageAdaptively(picConveyor2Mark1SearchResultImageDisp, _backCameraCs.ho_Image, false);//显示图像

                                    //显示设置
                                    HOperatorSet.SetColor(picConveyor2Mark1SearchResultImageDisp.HalconWindow, "green");
                                    HOperatorSet.SetDraw(picConveyor2Mark1SearchResultImageDisp.HalconWindow, "margin");
                                    HOperatorSet.SetLineWidth(picConveyor2Mark1SearchResultImageDisp.HalconWindow, 3);

                                    HOperatorSet.DispObj(ho_returnRectangle, picConveyor2Mark1SearchResultImageDisp.HalconWindow);//显示mark点区域
                                    HOperatorSet.DispCross(picConveyor2Mark1SearchResultImageDisp.HalconWindow, mark1RowCoor, mark1ColumnCoor, 20, 0);//显示十字线，标识mark点数据
                                    HOperatorSet.DispText(picConveyor2Mark1SearchResultImageDisp.HalconWindow, mark1RowCoor.ToString("f2") + ","
                                        + mark1ColumnCoor.ToString("f2"), "window", 0, 0, "green", new HTuple(), new HTuple());//显示坐标数据
                                }

                                if (conveyorIndex == 0)
                                    MyTool.TxtFileProcess.CreateLog("设备动作-识别传送线" + (conveyorIndex + 1).ToString() + "上的产品“" + SettingsProductForm._track1ActualsProductName + "”" +
                                    "的Mark点1识别成功" + "----用户：" + _operatorName,
                                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                                else
                                    MyTool.TxtFileProcess.CreateLog("设备动作-识别传送线" + (conveyorIndex + 1).ToString() + "上的产品“" + SettingsProductForm._track2ActualsProductName + "”" +
                                    "的Mark点1识别成功" + "----用户：" + _operatorName,
                                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

                                markSearchResult = true;
                            }
                            catch
                            {
                                if (conveyorIndex == 0)//如果为传送线1
                                {
                                    HOperatorSet.ClearWindow(picConveyor1Mark1SearchResultImageDisp.HalconWindow);
                                    HalconCameraControl.DispImageAdaptively(picConveyor1Mark1SearchResultImageDisp, _backCameraCs.ho_Image, false);//显示图像
                                }
                                else
                                {
                                    HOperatorSet.ClearWindow(picConveyor2Mark1SearchResultImageDisp.HalconWindow);
                                    HalconCameraControl.DispImageAdaptively(picConveyor2Mark1SearchResultImageDisp, _backCameraCs.ho_Image, false);//显示图像
                                }
                                markSearchResult = false;
                            }

                        }
                        else
                        {
                            if (conveyorIndex == 0)//如果为传送线1
                            {
                                HOperatorSet.ClearWindow(picConveyor1Mark1SearchResultImageDisp.HalconWindow);
                                HalconCameraControl.DispImageAdaptively(picConveyor1Mark1SearchResultImageDisp, _backCameraCs.ho_Image, false);//显示图像
                            }
                            else
                            {
                                HOperatorSet.ClearWindow(picConveyor2Mark1SearchResultImageDisp.HalconWindow);
                                HalconCameraControl.DispImageAdaptively(picConveyor2Mark1SearchResultImageDisp, _backCameraCs.ho_Image, false);//显示图像
                            }
                        }
                    }

                    if (!markSearchResult)
                    {
                        if (conveyorIndex == 0)//如果为传送线1
                            _stateAndAlarmCs._alarmFlag[390] = true;//传送线1的Mark点1识别失败报警
                        else
                            _stateAndAlarmCs._alarmFlag[392] = true;//传送线2的Mark点1识别失败报警
                    }
                }

                if (markSearchResult)//如果Mark点1识别成功，那么移动到Mark点2，识别第二个Mark点
                {
                    takePictureOfMarkPosition = 5;
                    if (conveyorIndex == 1)
                    {
                        takePictureOfMarkPosition = 7;
                    }

                    while (_axisControlFormVariate._axisActionCommand[0] != -1 ||
                            _axisControlFormVariate._axisActionCommand[1] != -1 ||
                            _axisControlFormVariate._axisActionCommand[2] != -1 ||
                            !_deviceWorkTriggerFlag ||
                            (conveyorIndex == 0 && (_conveyor1HaveExcessProductAlarmDealWithWayIndex == 1 || _stateAndAlarmCs._alarmFlag[400])) ||
                            (conveyorIndex == 1 && (_conveyor2HaveExcessProductAlarmDealWithWayIndex == 1 || _stateAndAlarmCs._alarmFlag[401])))//当XY轴处于运动指令状态,循环等待
                    {
                        if (_deadlyAlarmOccurFlag || !_autoModeFlag ||
                       (conveyorIndex == 0 && _conveyor1HaveExcessProductAlarmDealWithWayIndex == 1) ||
                       (conveyorIndex == 1 && _conveyor2HaveExcessProductAlarmDealWithWayIndex == 1))//如果触发了紧急停止
                        {
                            _deviceRecognizeMarkPositionAndGetCorrectMatrixModuleActionFlag = -1;
                            return;
                        }
                        Thread.Sleep(1);
                    }

                    //请求XYR轴去第二个mark点拍照位置
                    //X1
                    _axisControlFormVariate._axisCurrentPositionNumber[0] = 0;
                    _axisControlFormVariate._axisActionCommand[0] = takePictureOfMarkPosition;
                    //Y1
                    _axisControlFormVariate._axisCurrentPositionNumber[1] = 0;
                    _axisControlFormVariate._axisActionCommand[1] = takePictureOfMarkPosition;
                    //R1
                    _axisControlFormVariate._axisCurrentPositionNumber[2] = 0;
                    _axisControlFormVariate._axisActionCommand[2] = takePictureOfMarkPosition;

                    while ((_axisControlFormVariate._axisCurrentPositionNumber[0] == 0 || _axisControlFormVariate._axisActionCommand[0] == takePictureOfMarkPosition) ||
                        (_axisControlFormVariate._axisCurrentPositionNumber[1] == 0 || _axisControlFormVariate._axisActionCommand[1] == takePictureOfMarkPosition) ||
                        (_axisControlFormVariate._axisCurrentPositionNumber[2] == 0 || _axisControlFormVariate._axisActionCommand[2] == takePictureOfMarkPosition) ||
                        !_deviceWorkTriggerFlag ||
                        _axisControlFormVariate._axisMotionControlBoardVariate._axis00CheckDoneStatus == 0 ||
                        _axisControlFormVariate._axisMotionControlBoardVariate._axis01CheckDoneStatus == 0 ||
                        _axisControlFormVariate._axisMotionControlBoardVariate._axis02CheckDoneStatus == 0 ||
                        (conveyorIndex == 0 && (_conveyor1HaveExcessProductAlarmDealWithWayIndex == 1 || _stateAndAlarmCs._alarmFlag[400])) ||
                        (conveyorIndex == 1 && (_conveyor2HaveExcessProductAlarmDealWithWayIndex == 1 || _stateAndAlarmCs._alarmFlag[401])))//等待XY轴运行结束
                    {
                        if (_deadlyAlarmOccurFlag || !_autoModeFlag ||
                       (conveyorIndex == 0 && _conveyor1HaveExcessProductAlarmDealWithWayIndex == 1) ||
                       (conveyorIndex == 1 && _conveyor2HaveExcessProductAlarmDealWithWayIndex == 1))//如果触发了紧急停止
                        {
                            _deviceRecognizeMarkPositionAndGetCorrectMatrixModuleActionFlag = -1;
                            return;
                        }
                        Thread.Sleep(1);
                    }
                    Thread.Sleep(100);
                    /*
                    * 第二个Mark点位拍照获取Mark点坐标
                    */
                    if (true)
                    {
                        if (_backCameraCs.ho_Image != null)
                        {
                            _backCameraCs.ho_Image.Dispose();
                            _backCameraCs.ho_Image = null;
                        }

                        if (_backCameraCs.GrabImage2(false))
                        {
                            if (HalconPictureProcess.BlobGetCircleCenterPosition(ref _backCameraCs.ho_Image, _productParameter[conveyorIndex]._markSearchThresholdMin, _productParameter[conveyorIndex]._markSearchThresholdMax,
                                _productParameter[conveyorIndex]._markSelectShapeMin, _productParameter[conveyorIndex]._markSelectShapeMax, _productParameter[conveyorIndex]._markSearchMinCircularity, 2, out mark2RowCoor, out mark2ColumnCoor, out ho_returnRectangle) == 0)
                            {
                                try
                                {
                                    if (conveyorIndex == 0)//如果为传送线1
                                    {
                                        HOperatorSet.ClearWindow(picConveyor1Mark2SearchResultImageDisp.HalconWindow);
                                        HalconCameraControl.DispImageAdaptively(picConveyor1Mark2SearchResultImageDisp, _backCameraCs.ho_Image, false);//显示图像

                                        //显示设置
                                        HOperatorSet.SetColor(picConveyor1Mark2SearchResultImageDisp.HalconWindow, "green");
                                        HOperatorSet.SetDraw(picConveyor1Mark2SearchResultImageDisp.HalconWindow, "margin");
                                        HOperatorSet.SetLineWidth(picConveyor1Mark2SearchResultImageDisp.HalconWindow, 3);

                                        HOperatorSet.DispObj(ho_returnRectangle, picConveyor1Mark2SearchResultImageDisp.HalconWindow);//显示mark点区域
                                        HOperatorSet.DispCross(picConveyor1Mark2SearchResultImageDisp.HalconWindow, mark2RowCoor, mark2ColumnCoor, 20, 0);//显示十字线，标识mark点数据
                                        HOperatorSet.DispText(picConveyor1Mark2SearchResultImageDisp.HalconWindow, mark2RowCoor.ToString("f2") + ","
                                            + mark2ColumnCoor.ToString("f2"), "window", 0, 0, "green", new HTuple(), new HTuple());//显示坐标数据
                                    }
                                    else
                                    {
                                        HOperatorSet.ClearWindow(picConveyor2Mark2SearchResultImageDisp.HalconWindow);
                                        HalconCameraControl.DispImageAdaptively(picConveyor2Mark2SearchResultImageDisp, _backCameraCs.ho_Image, false);//显示图像

                                        //显示设置
                                        HOperatorSet.SetColor(picConveyor2Mark2SearchResultImageDisp.HalconWindow, "green");
                                        HOperatorSet.SetDraw(picConveyor2Mark2SearchResultImageDisp.HalconWindow, "margin");
                                        HOperatorSet.SetLineWidth(picConveyor2Mark2SearchResultImageDisp.HalconWindow, 3);

                                        HOperatorSet.DispObj(ho_returnRectangle, picConveyor2Mark2SearchResultImageDisp.HalconWindow);//显示mark点区域
                                        HOperatorSet.DispCross(picConveyor2Mark2SearchResultImageDisp.HalconWindow, mark2RowCoor, mark2ColumnCoor, 20, 0);//显示十字线，标识mark点数据
                                        HOperatorSet.DispText(picConveyor2Mark2SearchResultImageDisp.HalconWindow, mark2RowCoor.ToString("f2") + ","
                                            + mark2ColumnCoor.ToString("f2"), "window", 0, 0, "green", new HTuple(), new HTuple());//显示坐标数据
                                    }

                                    if (conveyorIndex == 0)
                                        MyTool.TxtFileProcess.CreateLog("设备动作-识别传送线" + (conveyorIndex + 1).ToString() + "上的产品“" + SettingsProductForm._track1ActualsProductName + "”" +
                                        "的Mark点2识别成功" + "----用户：" + _operatorName,
                                        BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                                    else
                                        MyTool.TxtFileProcess.CreateLog("设备动作-识别传送线" + (conveyorIndex + 1).ToString() + "上的产品“" + SettingsProductForm._track2ActualsProductName + "”" +
                                        "的Mark点2识别成功" + "----用户：" + _operatorName,
                                        BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

                                    markSearchResult = true;
                                }
                                catch
                                {
                                    if (conveyorIndex == 0)//如果为传送线1
                                    {
                                        HOperatorSet.ClearWindow(picConveyor1Mark2SearchResultImageDisp.HalconWindow);
                                        HalconCameraControl.DispImageAdaptively(picConveyor1Mark2SearchResultImageDisp, _backCameraCs.ho_Image, false);//显示图像
                                    }
                                    else
                                    {
                                        HOperatorSet.ClearWindow(picConveyor2Mark2SearchResultImageDisp.HalconWindow);
                                        HalconCameraControl.DispImageAdaptively(picConveyor2Mark2SearchResultImageDisp, _backCameraCs.ho_Image, false);//显示图像
                                    }
                                    markSearchResult = false;
                                }
                            }
                            else
                            {
                                if (conveyorIndex == 0)//如果为传送线1
                                {
                                    HOperatorSet.ClearWindow(picConveyor1Mark2SearchResultImageDisp.HalconWindow);
                                    HalconCameraControl.DispImageAdaptively(picConveyor1Mark2SearchResultImageDisp, _backCameraCs.ho_Image, false);//显示图像
                                }
                                else
                                {
                                    HOperatorSet.ClearWindow(picConveyor2Mark2SearchResultImageDisp.HalconWindow);
                                    HalconCameraControl.DispImageAdaptively(picConveyor2Mark2SearchResultImageDisp, _backCameraCs.ho_Image, false);//显示图像
                                }
                            }
                        }

                        if (!markSearchResult)
                        {
                            if (conveyorIndex == 0)//如果为传送线1
                                _stateAndAlarmCs._alarmFlag[391] = true;//传送线1的Mark点2识别失败报警
                            else
                                _stateAndAlarmCs._alarmFlag[393] = true;//传送线2的Mark点2识别失败报警
                        }
                    }
                }

                if (markSearchResult)//纠正成功
                {
                    if (conveyorIndex == 0)
                    {
                        _markSearchXOffsetData = (mark2ColumnCoor - _productParameter[conveyorIndex]._conveyor1Mark2SearchColumnResult) * _baseSettingFormVariate._settingsCameraCalibrationVariate._pixelSize[0] * -1;
                        _markSearchYOffsetData = (mark2RowCoor - _productParameter[conveyorIndex]._conveyor1Mark2SearchRowResult) * _baseSettingFormVariate._settingsCameraCalibrationVariate._pixelSize[1];
                    }
                    else
                    {
                        _markSearchXOffsetData = (mark2ColumnCoor - _productParameter[conveyorIndex]._conveyor2Mark2SearchColumnResult) * _baseSettingFormVariate._settingsCameraCalibrationVariate._pixelSize[0] * -1;
                        _markSearchYOffsetData = (mark2RowCoor - _productParameter[conveyorIndex]._conveyor2Mark2SearchRowResult) * _baseSettingFormVariate._settingsCameraCalibrationVariate._pixelSize[1];
                    }

                    if (conveyorIndex == 0 && _conveyor1HaveExcessProductAlarmDealWithWayIndex != 1)
                    {
                        _conveyor1Status = 101;
                    }
                    else if (conveyorIndex == 1 && _conveyor2HaveExcessProductAlarmDealWithWayIndex != 1)
                    {
                        _conveyor2Status = 101;
                    }

                    if (conveyorIndex == 0)
                        MyTool.TxtFileProcess.CreateLog("设备动作-识别传送线" + (conveyorIndex + 1).ToString() + "上的产品“" + SettingsProductForm._track1ActualsProductName + "”" +
                        "的Mark点并补正成功" + "----用户：" + _operatorName,
                        BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                    else
                        MyTool.TxtFileProcess.CreateLog("设备动作-识别传送线" + (conveyorIndex + 1).ToString() + "上的产品“" + SettingsProductForm._track2ActualsProductName + "”" +
                        "的Mark点并补正成功" + "----用户：" + _operatorName,
                        BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

                   _deviceRecognizeMarkPositionAndGetCorrectMatrixModuleActionFlag = conveyorIndex * 10 + 1;
                }
                else//如果识别Mark点失败时
                {
                    if (conveyorIndex == 0)
                        MyTool.TxtFileProcess.CreateLog("设备动作-识别传送线" + (conveyorIndex + 1).ToString() + "上的产品“" + SettingsProductForm._track1ActualsProductName + "”" +
                        "的Mark点并补正失败" + "----用户：" + _operatorName,
                        BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                    else
                        MyTool.TxtFileProcess.CreateLog("设备动作-识别传送线" + (conveyorIndex + 1).ToString() + "上的产品“" + SettingsProductForm._track2ActualsProductName + "”" +
                        "的Mark点并补正失败" + "----用户：" + _operatorName,
                        BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

                    //显示消息提示框
                    this.Invoke(new Action(() =>
                    {
                        if (_informVariate == null)
                        {
                            _informVariate = new InfoForm();
                            _informVariate.ClickEnsureEvent += InformationFormClickEnsureButtonEventFunc;
                            _informVariate.ClickOutEvent += InformationFormClickOutButtonEventFunc;
                            _informVariate.ClickTakeOutEvent += InformationFormClickTakeOutButtonEventFunc;
                            _informVariate.StartPosition = FormStartPosition.CenterScreen;
                            _informVariate.Focus();
                            _informVariate.Show();
                        }
                        _informVariate.Text = "Mark点识别坐标纠正失败报警！";
                        if (_stateAndAlarmCs._alarmFlag[390])//如果传送线1产品识别MARK点1失败报警，请人工取走或流出无法扫码的产品报警
                            _informVariate.txtInfoMessageDisp.Text = _stateAndAlarmCs._alarmMsg[390];
                        else if (_stateAndAlarmCs._alarmFlag[391])//传送线1产品识别MARK点2失败报警，请人工取走或流出无法扫码的产品！
                            _informVariate.txtInfoMessageDisp.Text = _stateAndAlarmCs._alarmMsg[391];
                        else if (_stateAndAlarmCs._alarmFlag[392])//如果传送线2产品识别MARK点1失败报警，请人工取走或流出无法扫码的产品报警
                            _informVariate.txtInfoMessageDisp.Text = _stateAndAlarmCs._alarmMsg[392];
                        else if (_stateAndAlarmCs._alarmFlag[393])//传送线2产品识别MARK点2失败报警，请人工取走或流出无法扫码的产品！
                            _informVariate.txtInfoMessageDisp.Text = _stateAndAlarmCs._alarmMsg[393];

                        _informVariate.btnOK.Visible = true;
                        _informVariate.btnOK.Text = "重试";
                        _informVariate.btnOut.Visible = true;
                        _informVariate.btnReleaseAndTakeOut.Visible = true;
                        _informclickBtnIndex = -1;
                    })); ;

                    while (_stateAndAlarmCs._alarmFlag[390] || _stateAndAlarmCs._alarmFlag[391] ||
                        _stateAndAlarmCs._alarmFlag[392] || _stateAndAlarmCs._alarmFlag[393] ||
                        _informclickBtnIndex == -1 || !_deviceWorkTriggerFlag ||
                            (conveyorIndex == 0 && (_conveyor1HaveExcessProductAlarmDealWithWayIndex == 1 || _stateAndAlarmCs._alarmFlag[400])) ||
                            (conveyorIndex == 1 && (_conveyor2HaveExcessProductAlarmDealWithWayIndex == 1 || _stateAndAlarmCs._alarmFlag[401])))//如果没有点击任何按钮，并且处报警阶段，等待
                    {
                        if (_deadlyAlarmOccurFlag || !_autoModeFlag ||
                       (conveyorIndex == 0 && _conveyor1HaveExcessProductAlarmDealWithWayIndex == 1) ||
                       (conveyorIndex == 1 && _conveyor2HaveExcessProductAlarmDealWithWayIndex == 1))
                        {
                            _deviceRecognizeMarkPositionAndGetCorrectMatrixModuleActionFlag = -1;
                            return;
                        }
                        Thread.Sleep(10);
                    }

                    if (_informclickBtnIndex == 0)//如果为重试按钮
                    {
                        if (conveyorIndex == 0)
                            MyTool.TxtFileProcess.CreateLog("设备动作-识别传送线" + (conveyorIndex + 1).ToString() + "上的产品“" + SettingsProductForm._track1ActualsProductName + "”" +
                            "的Mark点并补正失败" + "选择重试" + "----用户：" + _operatorName,
                            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                        else
                            MyTool.TxtFileProcess.CreateLog("设备动作-识别传送线" + (conveyorIndex + 1).ToString() + "上的产品“" + SettingsProductForm._track2ActualsProductName + "”" +
                            "的Mark点并补正失败" + "选择重试" + "----用户：" + _operatorName,
                            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                    }
                    else if (_informclickBtnIndex == 1)//如果为流出按钮
                    {
                        if (conveyorIndex == 0)
                            MyTool.TxtFileProcess.CreateLog("设备动作-识别传送线" + (conveyorIndex + 1).ToString() + "上的产品“" + SettingsProductForm._track1ActualsProductName + "”" +
                            "的Mark点并补正失败" + "选择流出产品" + "----用户：" + _operatorName,
                            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                        else
                            MyTool.TxtFileProcess.CreateLog("设备动作-识别传送线" + (conveyorIndex + 1).ToString() + "上的产品“" + SettingsProductForm._track2ActualsProductName + "”" +
                            "的Mark点并补正失败" + "选择流出产品" + "----用户：" + _operatorName,
                            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

                        if (conveyorIndex == 0 && _conveyor1HaveExcessProductAlarmDealWithWayIndex != 1)
                        {
                            _conveyor1Status = 2;
                        }
                        else if (conveyorIndex == 1 && _conveyor2HaveExcessProductAlarmDealWithWayIndex != 1)
                        {
                            _conveyor2Status = 2;
                        }
                    }
                    else//如果为人工取出按钮
                    {
                        if (conveyorIndex == 0)
                            MyTool.TxtFileProcess.CreateLog("设备动作-识别传送线" + (conveyorIndex + 1).ToString() + "上的产品“" + SettingsProductForm._track1ActualsProductName + "”" +
                            "的Mark点并补正失败" + "人工取走产品" + "----用户：" + _operatorName,
                            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                        else
                            MyTool.TxtFileProcess.CreateLog("设备动作-识别传送线" + (conveyorIndex + 1).ToString() + "上的产品“" + SettingsProductForm._track2ActualsProductName + "”" +
                            "的Mark点并补正失败" + "人工取走产品" + "----用户：" + _operatorName,
                            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

                        if (conveyorIndex == 0 && _conveyor1HaveExcessProductAlarmDealWithWayIndex != 1)
                        {
                            _conveyor1HaveProductFlag = false;
                            _conveyor1Status = 0;
                        }
                        else if (conveyorIndex == 1 && _conveyor2HaveExcessProductAlarmDealWithWayIndex != 1)
                        {
                            _conveyor2HaveProductFlag = false;
                            _conveyor2Status = 0;
                        }
                    }

                    _deviceRecognizeMarkPositionAndGetCorrectMatrixModuleActionFlag = conveyorIndex * 10 + 2;
                }
            });
        }

        /// <summary>
        /// AutoModule:自动模块
        /// </summary>
        private void AutoModule()
        {
            lock (this)
            {
                _autoModuleActionFlag = 0;

                if (_allAxisHomeBackModuleOffDlyT.STATUS && _deviceWorkTriggerFlag && _allAxisHomeBackModuleActionFlag != 1)//如果没有完成原点复归
                {
                    MyTool.TxtFileProcess.CreateLog("设备动作-请求整机回原！" + "----用户：" + _operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

                    _allAxisHomeBackModuleActionFlag = 0;
                    AllAxesHomeBackModule();//整机回原
                    _currentDestinationPointIndex = 1;
                }
                else if (_allAxisHomeBackModuleActionFlag == 1)//如果已经完成原点复归
                {
                    if (_conveyor1Status == 1 && (_conveyor2Status < 100 || _conveyor2Status > 104) && 
                        SettingsGeneralForm._conveyor1UsingFlag && _conveyor1HaveExcessProductAlarmDealWithWayIndex!=1)  //如果传送线1上状态为等待检查，传送线2不为工作状态
                    {
                        MyTool.TxtFileProcess.CreateLog("设备动作-传送线1新上料，启动检测前自检!" + "----用户：" + _operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

                        //给轴运动画面请求更新产品数据，并等待数据等更新完成
                        if (AxisControlMainForm._currentProductName != SettingsProductForm._track1ActualsProductName)
                        {
                            AxisControlMainForm._currentProductName = SettingsProductForm._track1ActualsProductName;
                            AxisControlMainForm._axisParFileInitialFinishFlag = false;
                            AxisControlMainForm._axisPosFileInitialFinishFlag = false;
                        }

                        while (!AxisParCheck(SettingsProductForm._track1ActualsProductName) || !AxisPosDataCheck(SettingsProductForm._track1ActualsProductName))
                        {
                            MyTool.TxtFileProcess.CreateLog("设备动作-传送线1新上料，启动检测前自检失败--产品" + SettingsProductForm._track1ActualsProductName + "的轴参数中有非法数!" + "----用户：" + _operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

                            MessageBox.Show("产品" + SettingsProductForm._track1ActualsProductName + "的轴参数中有非法数，请切换自手动模式修改参数后重试！");
                            if (_deadlyAlarmOccurFlag || !_autoModeFlag || _conveyor1HaveExcessProductAlarmDealWithWayIndex == 1)//如果触发了紧急停止
                            {
                                return;
                            }
                            Thread.Sleep(10);
                        }

                        while (!AxisControlMainForm._axisParFileInitialFinishFlag || !AxisControlMainForm._axisPosFileInitialFinishFlag)//等待数据初始化
                        {
                            if (_deadlyAlarmOccurFlag || !_autoModeFlag || _conveyor1HaveExcessProductAlarmDealWithWayIndex == 1)//如果触发了紧急停止
                            {
                                return;
                            }
                            Thread.Sleep(10);
                        }

                        _productParameter[0]._parameterLoadSuccessfulFlag = false;
                        _productParameter[0].ProductDataLoadToVariate(SettingsGeneralForm._productDirectory + "\\" + SettingsProductForm._track1ActualsProductName + ".rcp");
                        while (!_productParameter[0]._parameterLoadSuccessfulFlag)
                        {
                            MyTool.TxtFileProcess.CreateLog("设备动作-传送线1新上料，启动检测前自检失败--产品" + SettingsProductForm._track1ActualsProductName + "的配方文件中有非法数!" + "----用户：" + _operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

                            MessageBox.Show("传送线1运行设定的配方文件" + SettingsProductForm._track1ActualsProductName + ".rcp中有非法参数，无法启动检查！");
                            if (_deadlyAlarmOccurFlag || !_autoModeFlag || _conveyor1HaveExcessProductAlarmDealWithWayIndex == 1)//如果触发了紧急停止
                            {
                                return;
                            }
                            Thread.Sleep(10);
                        }

                        //如果新的传送线1图像深度学习所需文件与之前的不一致
                        if (_lastTimepreprocessFilePath != _productParameter[0]._preprocessParameterFilePath || _lastTimeDimensionFilePath != _productParameter[0]._dimensionFilePath || _lastTimeModelFilePath != _productParameter[0]._modelFilePath)
                        {
                            _newLeftDeepLearningObjectDetectionRectangle1Infer.Dispose();
                            _newRightDeepLearningObjectDetectionRectangle1Infer.Dispose();

                            _leftInferNeedReadParameter = true;
                            _rightInferNeedReadParameter = true;

                            _lastTimepreprocessFilePath = _productParameter[0]._preprocessParameterFilePath;
                            _lastTimeDimensionFilePath = _productParameter[0]._dimensionFilePath;
                            _lastTimeModelFilePath = _productParameter[0]._modelFilePath;
                        }

                        //初始化存储检查结果的Datatable
                        if (_leftInferResultDt[0] != null)
                        {
                            _leftInferResultDt[0].Dispose();
                            _leftInferResultDt[0] = null;
                        }
                        if (_rightInferResultDt[0] != null)
                        {
                            _rightInferResultDt[0].Dispose();
                            _rightInferResultDt[0] = null;
                        }
                        //对传送线1实时显示用dictionary进行重置初始化等
                        if (_conveyor1InferResultDic != null)
                        {
                            _conveyor1InferResultDic.Clear();
                            _conveyor1InferResultDic = null;
                        }
                        _conveyor1InferResultDic = new Dictionary<int, InferResult>();

                        _leftInferResultDt[0] = new DataTable();

                        _leftInferResultDt[0].Columns.Add("序号", typeof(string));
                        _leftInferResultDt[0].Columns.Add("名称", typeof(string));
                        _leftInferResultDt[0].Columns.Add("左侧结果", typeof(string));
                        _leftInferResultDt[0].Columns.Add("右侧结果", typeof(string));

                        InferResult tempInferResult = new InferResult();

                        for (int i = 0; i < _productParameter[0]._pointPositionDT.Rows.Count; i++)
                        {
                            _leftInferResultDt[0].Rows.Add();
                            _leftInferResultDt[0].Rows[i][0] = (i + 1).ToString();
                            _leftInferResultDt[0].Rows[i][1] = _productParameter[0]._pointPositionDT.Rows[i][5].ToString();

                            tempInferResult._elementNumber = (i + 1).ToString();
                            tempInferResult._elementName = _productParameter[0]._pointPositionDT.Rows[i][5].ToString();
                            tempInferResult._elementLeftResult = "";
                            tempInferResult._elementRightResult = "";
                            tempInferResult._elementTotalResult = "";
                        }

                        _rightInferResultDt[0] = _leftInferResultDt[0].Copy();

                        for (int i = 0; i < SAVE_IMAGE_QUANTITY_IN_RAM_FOR_RUNNING; i++)
                        {
                            _leftCameraInfer[i]._cameraPicturePointName = "";
                            _leftCameraInfer[i]._cameraInferredResultNGPictureSavePath = "";
                            _leftCameraInfer[i]._cameraInferredResultOKPictureSavePath = "";
                            _leftCameraInfer[i]._cameraPictureInferredFlag = false;
                            _leftCameraInfer[i]._cameraPicturePointNumber = -1;
                            _leftCameraInfer[i]._cameraPictureRequestInferFlag = false;
                            _leftCameraInfer[i]._cameraPictureSavedFlag = false;
                            _leftCameraInfer[i]._cameraPictureSavePath = "";
                            _leftCameraInfer[i]._productIndex = -1;
                            _leftCameraInfer[i].ho_CameraImage = null;

                            _rightCameraInfer[i]._cameraPicturePointName = "";
                            _rightCameraInfer[i]._cameraInferredResultNGPictureSavePath = "";
                            _rightCameraInfer[i]._cameraInferredResultOKPictureSavePath = "";
                            _rightCameraInfer[i]._cameraPictureInferredFlag = false;
                            _rightCameraInfer[i]._cameraPicturePointNumber = -1;
                            _rightCameraInfer[i]._cameraPictureRequestInferFlag = false;
                            _rightCameraInfer[i]._cameraPictureSavedFlag = false;
                            _rightCameraInfer[i]._cameraPictureSavePath = "";
                            _rightCameraInfer[i]._productIndex = -1;
                            _rightCameraInfer[i].ho_CameraImage = null;
                        }
                        if (_needInferOrNotDt != null)
                        {
                            _needInferOrNotDt.Dispose();
                            _needInferOrNotDt = null;
                        }

                        //提前打开后相机光源
                        if (!_axisControlFormVariate._ioStatus[1, BACK_CAMERA_LIGHT_TRIGGER_OUT_IO] || !_axisControlFormVariate._ioStatus[1, BACK_CAMERA_LIGHT_TRIGGER_OUT_IO2])
                        {
                            _axisControlFormVariate.OutputControl(BACK_CAMERA_LIGHT_TRIGGER_OUT_IO, true);//打开光源
                            _axisControlFormVariate.OutputControl(BACK_CAMERA_LIGHT_TRIGGER_OUT_IO2, true);//打开光源
                        }

                        //初始化Mark点补正值
                        _markSearchXOffsetData = 0;
                        _markSearchYOffsetData = 0;

                        MyTool.TxtFileProcess.CreateLog("设备动作-传送线1新上料，启动检测前自检成功！" + "----用户：" + _operatorName,
                        BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

                        _conveyor1Status = 100;//设置传送线1上料完毕，等待扫码识别功能
                    }
                    else if (_conveyor2Status == 1 && (_conveyor1Status < 100 || _conveyor1Status > 104) && 
                        SettingsGeneralForm._conveyor2UsingFlag && _conveyor2HaveExcessProductAlarmDealWithWayIndex != 1)//如果传送线2上状态为等待检查，传送线1不为工作状态
                    {
                        MyTool.TxtFileProcess.CreateLog("设备动作-传送线2新上料，启动检测前自检!" + "----用户：" + _operatorName,
                        BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

                        //给轴运动画面请求更新产品数据，并等待数据等更新完成
                        if (AxisControlMainForm._currentProductName != SettingsProductForm._track2ActualsProductName)
                        {
                            AxisControlMainForm._currentProductName = SettingsProductForm._track2ActualsProductName;
                            AxisControlMainForm._axisParFileInitialFinishFlag = false;
                            AxisControlMainForm._axisPosFileInitialFinishFlag = false;
                        }
                        while (!AxisParCheck(SettingsProductForm._track2ActualsProductName) || !AxisPosDataCheck(SettingsProductForm._track2ActualsProductName))
                        {
                            MyTool.TxtFileProcess.CreateLog("设备动作-传送线2新上料，启动检测前自检失败--产品" + SettingsProductForm._track2ActualsProductName + "的轴参数中有非法数!" + "----用户：" + _operatorName,
                            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

                            MessageBox.Show("产品" + SettingsProductForm._track1ActualsProductName + "的轴参数中有非法数，请切换自手动模式修改参数后重试！");
                            if (_deadlyAlarmOccurFlag || !_autoModeFlag || _conveyor2HaveExcessProductAlarmDealWithWayIndex == 1)//如果触发了紧急停止
                            {
                                return;
                            }
                            Thread.Sleep(10);
                        }

                        while (!AxisControlMainForm._axisParFileInitialFinishFlag || !AxisControlMainForm._axisPosFileInitialFinishFlag)//等待数据初始化
                        {
                            if (_deadlyAlarmOccurFlag || !_autoModeFlag || _conveyor2HaveExcessProductAlarmDealWithWayIndex == 1)//如果触发了紧急停止
                            {
                                return;
                            }
                            Thread.Sleep(10);
                        }

                        _productParameter[1]._parameterLoadSuccessfulFlag = false;
                        _productParameter[1].ProductDataLoadToVariate(SettingsGeneralForm._productDirectory + "\\" + SettingsProductForm._track2ActualsProductName + ".rcp");
                        while (!_productParameter[1]._parameterLoadSuccessfulFlag)
                        {
                            MyTool.TxtFileProcess.CreateLog("设备动作-传送线2新上料，启动检测前自检失败--产品" + SettingsProductForm._track2ActualsProductName + "的配方文件中有非法数!" + "----用户：" + _operatorName,
                            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

                            MessageBox.Show("传送线1运行设定的配方文件" + SettingsProductForm._track1ActualsProductName + ".rcp中有非法参数，无法启动自动模式！");
                            if (_deadlyAlarmOccurFlag || !_autoModeFlag || _conveyor2HaveExcessProductAlarmDealWithWayIndex == 1)//如果触发了紧急停止
                            {
                                return;
                            }
                            Thread.Sleep(10);
                        }

                        //如果新的传送线2图像深度学习所需文件与之前的不一致
                        if (_lastTimepreprocessFilePath != _productParameter[1]._preprocessParameterFilePath || _lastTimeDimensionFilePath != _productParameter[1]._dimensionFilePath || _lastTimeModelFilePath != _productParameter[1]._modelFilePath)
                        {
                            _newLeftDeepLearningObjectDetectionRectangle1Infer.Dispose();
                            _newRightDeepLearningObjectDetectionRectangle1Infer.Dispose();

                            _leftInferNeedReadParameter = true;
                            _rightInferNeedReadParameter = true;

                            _lastTimepreprocessFilePath = _productParameter[1]._preprocessParameterFilePath;
                            _lastTimeDimensionFilePath = _productParameter[1]._dimensionFilePath;
                            _lastTimeModelFilePath = _productParameter[1]._modelFilePath;
                        }


                        //初始化存储检查结果的Datatable
                        if (_leftInferResultDt[1] != null)
                        {
                            _leftInferResultDt[1].Dispose();
                            _leftInferResultDt[1] = null;
                        }
                        if (_rightInferResultDt[1] != null)
                        {
                            _rightInferResultDt[1].Dispose();
                            _rightInferResultDt[1] = null;
                        }
                        //对传送线2实时显示用dictionary进行重置初始化等
                        if (_conveyor2InferResultDic != null)
                        {
                            _conveyor2InferResultDic.Clear();
                            _conveyor2InferResultDic = null;
                        }
                        _conveyor2InferResultDic = new Dictionary<int, InferResult>();


                        _leftInferResultDt[1] = new DataTable();

                        _leftInferResultDt[1].Columns.Add("序号", typeof(string));
                        _leftInferResultDt[1].Columns.Add("名称", typeof(string));
                        _leftInferResultDt[1].Columns.Add("左侧结果", typeof(string));
                        _leftInferResultDt[1].Columns.Add("右侧结果", typeof(string));

                        InferResult tempInferResult = new InferResult();

                        for (int i = 0; i < _productParameter[1]._pointPositionDT.Rows.Count; i++)
                        {
                            _leftInferResultDt[1].Rows.Add();
                            _leftInferResultDt[1].Rows[i][0] = (i + 1).ToString();
                            _leftInferResultDt[1].Rows[i][1] = _productParameter[1]._pointPositionDT.Rows[i][5].ToString();

                            tempInferResult._elementNumber = (i + 1).ToString();
                            tempInferResult._elementName = _productParameter[1]._pointPositionDT.Rows[i][5].ToString();
                            tempInferResult._elementLeftResult = "";
                            tempInferResult._elementRightResult = "";
                            tempInferResult._elementTotalResult = "";
                        }
                        _rightInferResultDt[1] = _leftInferResultDt[1].Copy();

                        for (int i = 0; i < SAVE_IMAGE_QUANTITY_IN_RAM_FOR_RUNNING; i++)
                        {
                            _leftCameraInfer[i]._cameraPicturePointName = "";
                            _leftCameraInfer[i]._cameraInferredResultNGPictureSavePath = "";
                            _leftCameraInfer[i]._cameraInferredResultOKPictureSavePath = "";
                            _leftCameraInfer[i]._cameraPictureInferredFlag = false;
                            _leftCameraInfer[i]._cameraPicturePointNumber = -1;
                            _leftCameraInfer[i]._cameraPictureRequestInferFlag = false;
                            _leftCameraInfer[i]._cameraPictureSavedFlag = false;
                            _leftCameraInfer[i]._cameraPictureSavePath = "";
                            _leftCameraInfer[i]._productIndex = -1;
                            _leftCameraInfer[i].ho_CameraImage = null;

                            _rightCameraInfer[i]._cameraPicturePointName = "";
                            _rightCameraInfer[i]._cameraInferredResultNGPictureSavePath = "";
                            _rightCameraInfer[i]._cameraInferredResultOKPictureSavePath = "";
                            _rightCameraInfer[i]._cameraPictureInferredFlag = false;
                            _rightCameraInfer[i]._cameraPicturePointNumber = -1;
                            _rightCameraInfer[i]._cameraPictureRequestInferFlag = false;
                            _rightCameraInfer[i]._cameraPictureSavedFlag = false;
                            _rightCameraInfer[i]._cameraPictureSavePath = "";
                            _rightCameraInfer[i]._productIndex = -1;
                            _rightCameraInfer[i].ho_CameraImage = null;
                        }
                        if (_needInferOrNotDt != null)
                        {
                            _needInferOrNotDt.Dispose();
                            _needInferOrNotDt = null;
                        }


                        //提前打开后相机光源
                        if (!_axisControlFormVariate._ioStatus[1, BACK_CAMERA_LIGHT_TRIGGER_OUT_IO] || !_axisControlFormVariate._ioStatus[1, BACK_CAMERA_LIGHT_TRIGGER_OUT_IO2])
                        {
                            _axisControlFormVariate.OutputControl(BACK_CAMERA_LIGHT_TRIGGER_OUT_IO, true);//打开光源
                            _axisControlFormVariate.OutputControl(BACK_CAMERA_LIGHT_TRIGGER_OUT_IO2, true);//打开光源
                        }

                        //初始化Mark点补正值
                        _markSearchXOffsetData = 0;
                        _markSearchYOffsetData = 0;

                        MyTool.TxtFileProcess.CreateLog("设备动作-传送线2新上料，启动检测前自检成功！" + "----用户：" + _operatorName,
                        BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

                        _conveyor2Status = 100;//设置传送线1上料完毕，等待扫码识别功能
                    }
                    else if (_conveyor1Status == 1 && !SettingsGeneralForm._conveyor1UsingFlag)
                    {
                        MyTool.TxtFileProcess.CreateLog("设备动作-传送线1直通模式，切换为需要下料！" + "----用户：" + _operatorName,
                        BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                        _conveyor1Status = 2;
                    }
                    else if (_conveyor2Status == 1 && !SettingsGeneralForm._conveyor2UsingFlag)
                    {
                        MyTool.TxtFileProcess.CreateLog("设备动作-传送线2直通模式，切换为需要下料！" + "----用户：" + _operatorName,
                        BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                        _conveyor2Status = 2;
                    }

                    //识别mark点并获取到矫正矩阵
                    if (((_conveyor1Status == 100 && _conveyor1HaveExcessProductAlarmDealWithWayIndex != 1) ||
                        (_conveyor2Status == 100 && _conveyor2HaveExcessProductAlarmDealWithWayIndex != 1)) && 
                        _deviceRecognizeMarkPositionAndGetCorrectMatrixModuleActionFlag != 0)//增加出现设备中有多余产品报警并选择流出时不会启动模块条件
                    {
                        MyTool.TxtFileProcess.CreateLog("设备动作-请求Mark点识别矫正！" + "----用户：" + _operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

                        _deviceRecognizeMarkPositionAndGetCorrectMatrixModuleActionFlag = 0;

                        if (_conveyor1Status == 100)
                            DeviceRecognizeMarkPositionAndGetCorrectMatrixModule(0);
                        else
                            DeviceRecognizeMarkPositionAndGetCorrectMatrixModule(1);
                    }

                    //扫码工作
                    if (((_conveyor1Status == 101 && _conveyor1HaveExcessProductAlarmDealWithWayIndex != 1) || 
                        (_conveyor2Status == 101 && _conveyor2HaveExcessProductAlarmDealWithWayIndex != 1)) && 
                        _barcodeScanModuleActionFlag != 0)//增加出现设备中有多余产品报警并选择流出时不会启动模块条件
                    {
                        MyTool.TxtFileProcess.CreateLog("设备动作-请求扫码！" + "----用户：" + _operatorName,
                        BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

                        _barcodeScanModuleActionFlag = 0;
                        if (_conveyor1Status == 101)
                            BarcodeScanModule(0);
                        else
                            BarcodeScanModule(1);
                    }

                    //与MES通讯获取数据，为了防止阻塞通讯，要求另一个传送线不允许处于传送数据状态
                    if (((_conveyor1Status == 102 && _conveyor2Status != 105 && _conveyor1HaveExcessProductAlarmDealWithWayIndex != 1) ||
                        (_conveyor2Status == 102 && _conveyor1Status != 105 && _conveyor2HaveExcessProductAlarmDealWithWayIndex != 1)) && 
                        _getOrSendProductInfoFromMESModuleActionFlag != 0)//增加出现设备中有多余产品报警并选择流出时不会启动模块条件
                    {
                        MyTool.TxtFileProcess.CreateLog("设备动作-请求入站MES通讯！" + "----用户：" + _operatorName,
                        BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

                        _getOrSendProductInfoFromMESModuleActionFlag = 0;

                        if (_conveyor1Status == 102)
                            GetOrSendProductInfoFromMESModule(_barcodeData[0], false, 0, new DataTable());
                        else if (_conveyor2Status == 102)
                            GetOrSendProductInfoFromMESModule(_barcodeData[1], false, 1, new DataTable());
                    }

                    //轴整体点位移动
                    if (((_conveyor1Status == 103 && _conveyor1HaveExcessProductAlarmDealWithWayIndex != 1) ||
                        (_conveyor2Status == 103 && _conveyor2HaveExcessProductAlarmDealWithWayIndex != 1)) && 
                        _deviceTotalWorkModuleActionFlag != 0)//增加出现设备中有多余产品报警并选择流出时不会启动模块条件
                    {
                        MyTool.TxtFileProcess.CreateLog("设备动作-请求整机动作！" + "----用户：" + _operatorName,
                        BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

                        _deviceTotalWorkModuleActionFlag = 0;

                        if (_conveyor1Status == 103)
                        {
                            dgrdvConveyor1InferResultDisp.Invoke(new Action(() =>
                            {
                                dgrdvConveyor1InferResultDisp.Rows.Clear();
                            })); ;
                        }
                        else
                        {
                            dgrdvConveyor2InferResultDisp.Invoke(new Action(() =>
                            {
                                dgrdvConveyor2InferResultDisp.Rows.Clear();
                            })); ;
                        }

                        if (_conveyor1Status == 103)
                            DeviceTotalWorkModule(0);
                        else
                            DeviceTotalWorkModule(1);
                    }

                    //检查完毕传输数据至MES
                    if (((_conveyor1Status == 105 && _conveyor2Status != 102 && _conveyor1HaveExcessProductAlarmDealWithWayIndex != 1) ||
                        (_conveyor2Status == 105 && _conveyor1Status != 102 && _conveyor2HaveExcessProductAlarmDealWithWayIndex != 1)) && 
                        _getOrSendProductInfoFromMESModuleActionFlag != 0)//增加出现设备中有多余产品报警并选择流出时不会启动模块条件
                    {
                        MyTool.TxtFileProcess.CreateLog("设备动作-请求传输检测结果数据至MES！" + "----用户：" + _operatorName,
                        BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

                        _getOrSendProductInfoFromMESModuleActionFlag = 0;

                        DataTable sorttedResultDT = null;
                        CombineLeftAndRightResultDTAndRecordYieldAndSaveResultCSVFile(out sorttedResultDT);//数据处理及保存

                        if (_conveyor1Status == 105)
                            GetOrSendProductInfoFromMESModule(_barcodeData[0], true, 0, sorttedResultDT);
                        else if (_conveyor2Status == 105)
                            GetOrSendProductInfoFromMESModule(_barcodeData[1], true, 1, sorttedResultDT);
                    }

                    //传输检查结果数据至MES完毕，设置传送线产品状态
                    if ((_conveyor1Status == 106 && _conveyor1HaveExcessProductAlarmDealWithWayIndex != 1) ||
                        (_conveyor2Status == 106 && _conveyor2HaveExcessProductAlarmDealWithWayIndex != 1))
                    {

                        if (_conveyor1Status == 106)
                        {
                            MyTool.TxtFileProcess.CreateLog("设备动作-传输检测结果数据至MES后，切换传送线1模式为须下料模式！" + "----用户：" + _operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

                            _conveyor1Status = 2;//设置传送线1为需要下料
                        }
                        else
                        {
                            MyTool.TxtFileProcess.CreateLog("设备动作-传输检测结果数据至MES后，切换传送线2模式为须下料模式！" + "----用户：" + _operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

                            _conveyor2Status = 2;//设置传送线2为需要下料
                        }
                    }

                    if (_conveyor1LoadUnloadModuleActionFlag != 0)//一旦传送线1上下料模块可以启动，那么就启动
                    {
                        _conveyor1LoadUnloadModuleActionFlag = 0;
                        Conveyor1LoadUnloadModule();
                    }

                    if (_conveyor2LoadUnloadModuleActionFlag != 0)//一旦传送线2上下料模块可以启动，那么就启动
                    {
                        _conveyor2LoadUnloadModuleActionFlag = 0;
                        Conveyor2LoadUnloadModule();
                    }

                    if (_conveyor1Status == 103 || _conveyor1Status == 104 || _conveyor2Status == 103 || _conveyor2Status == 104)//某一传送线处于需要推断状态
                    {
                        if (_cameraInferDataClearModuleActionFlag != 0)//如果相机图像推断结果清除模块为OFF
                        {
                            _cameraInferDataClearModuleActionFlag = 0;
                            CameraInferDataClearModule();
                        }

                        if (_leftCameraImageInferModuleActionFlag != 0)//如果左侧相机结果推断模块为OFF
                        {
                            _leftCameraImageInferModuleActionFlag = 0;
                            LeftCameraImageInferModule();
                        }

                        if (_rightCameraImageInferModuleActionFlag != 0)//如果右侧相机结果推断模块为OFF
                        {
                            _rightCameraImageInferModuleActionFlag = 0;
                            RightCameraImageInferModule();
                        }

                        if (_leftCameraSourceImageSaveModuleActionFlag != 0)//如果左侧相机原始图像保存模块为OFF
                        {
                            _leftCameraSourceImageSaveModuleActionFlag = 0;
                            LeftCameraSourceImageSaveModule();
                        }

                        if (_rightCameraSourceImageSaveModuleActionFlag != 0)//如果右侧相机原始图像保存模块为OFF
                        {
                            _rightCameraSourceImageSaveModuleActionFlag = 0;
                            RightCameraSourceImageSaveModule();
                        }

                        if (_leftAndRightCameraGrabImageModuleActionFlag != 0 && _leftAndRightCameraGrabImageModuleControlFlag == 1)//如果相机抓取模块为OFF，并且获取到了启动请求模块
                        {
                            _leftAndRightCameraGrabImageModuleActionFlag = 0;
                            if (_conveyor1Status == 103 || _conveyor1Status == 104)
                                LeftAndRightCameraGrabImageModule(_currentDestinationPointIndex, 0);
                            else
                                LeftAndRightCameraGrabImageModule(_currentDestinationPointIndex, 1);
                            _leftAndRightCameraGrabImageModuleControlFlag = 2;
                        }
                    }

                    if (_leftCameraInferredImageSaveModuleActionFlag != 0)//如果左侧相机推断结果图片保存模块为OFF
                    {
                        _leftCameraInferredImageSaveModuleActionFlag = 0;
                        LeftCameraInferredImageSaveModule();
                    }

                    if (_rightCameraInferredImageSaveModuleActionFlag != 0)//如果右侧相机推断结果图片保存模块为OFF
                    {
                        _rightCameraInferredImageSaveModuleActionFlag = 0;
                        RightCameraInferredImageSaveModule();
                    }
                }

                _autoModuleActionFlag = 1;
            }
        }

        /// <summary>
        /// DeviceActionThreadFunc：设备动作线程
        /// </summary>
        private void DeviceActionThreadFunc()
        {
            while (true)
            {
                if (_autoModeFlag)//如果为自动模式
                {
                    if (_autoModuleOffDlyT.STATUS)
                    {
                        _autoModuleActionFlag = 0;
                        AutoModule();
                    }
                }
                else//如果不为自动模式
                {
                    if (_virtualAndRealBtnIndex == 0)//如果点击自动
                    {
                        MyTool.TxtFileProcess.CreateLog("设备动作-切换设备模式为自动模式，参数初始化！" + "----用户：" + _operatorName,
                        BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

                        _baseSettingFormVariate._settingsProductFormVariate.Invoke(new Action(() =>
                            {
                                _baseSettingFormVariate._settingsProductFormVariate.chkUseUseRelativePosition.Enabled = false;
                            })); ;

                        _xyMoveToAssignedPointPositionModuleActionFlag = -1;//xy运动模块状态清为-1
                        _allAxisHomeBackModuleActionFlag = -1;//回原点模块状态清为-1
                        _axesGotoStandbyModuleActionFlag = -1;//轴回等待位置模块状态清为-1
                        _conveyorChangeToAssignedWidthModuleActionFlag = -1;//传送线宽度调节模块状态清为-1
                        _conveyor1ActionModuleActionFlag = -1;//传送线1动作模块状态清为-1
                        _conveyor2ActionModuleActionFlag = -1;//传送线2动作模块状态清为-1
                        _conveyor1LoadUnloadModuleActionFlag = -1;//传送线1上下料模块状态清为-1
                        _conveyor2LoadUnloadModuleActionFlag = -1;//传送线2上下料模块状态清为-1
                        _deviceTotalWorkModuleActionFlag = -1;//设备整体工作模块状态清为-1
                        _barcodeScanModuleActionFlag = -1;//扫码模块状态清为-1
                        _getOrSendProductInfoFromMESModuleActionFlag = -1;//数据与MES交互模块状态清为-1
                        _deviceRecognizeMarkPositionAndGetCorrectMatrixModuleActionFlag = -1;//设备识别MARK点并计算变换矩阵模块状态清为-1
                        _autoModuleActionFlag = -1;
                        _currentDestinationPointIndex = 1;//当前指令坐标号清为0

                        //停止传送线
                        if (!_newDAM02AIAOOperation.comm.IsOpen)
                        {
                            if (!_newDAM02AIAOOperation.OpenSerialPort(SettingsGeneralForm._conveyorControlCom))
                            {
                                MyTool.TxtFileProcess.CreateLog("设备动作-切换设备模式为自动模式，参数初始化，打开传送线控制模拟量串口失败！" + "----用户：" + _operatorName,
                                BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                                MessageBox.Show("打开串口失败");
                            }
                        }
                        if (_newDAM02AIAOOperation.comm.IsOpen)
                        {
                            _newDAM02AIAOOperation.analogOutput(0, 0);
                            _newDAM02AIAOOperation.analogOutput(1, 0);
                        }
                        _axisControlFormVariate.OutputControl(69, false);
                        _axisControlFormVariate.OutputControl(71, false);

                        //停止向上下游发送流入流出请求
                        _axisControlFormVariate.OutputControl(72, false);
                        _axisControlFormVariate.OutputControl(73, false);
                        _axisControlFormVariate.OutputControl(74, false);
                        _axisControlFormVariate.OutputControl(75, false);

                        //左右相机触发信号OFF
                        _leftCameraGrab = false;
                        _leftCameraVideo = false;
                        _rightCameraGrab = false;
                        _rightCameraVideo = false;
                        Thread.Sleep(100);
                        _axisControlFormVariate.OutputControl(BACK_CAMERA_LIGHT_TRIGGER_OUT_IO, false);
                        _axisControlFormVariate.OutputControl(LEFT_CAMERA_TRIGGER_OUT_IO, false);
                        _axisControlFormVariate.OutputControl(RIGHT_CAMERA_TRIGGER_OUT_IO, false);
                        _axisControlFormVariate.OutputControl(BACK_CAMERA_LIGHT_TRIGGER_OUT_IO2, false);
                        _axisControlFormVariate.OutputControl(LEFT_CAMERA_TRIGGER_OUT_IO2, false);
                        _axisControlFormVariate.OutputControl(RIGHT_CAMERA_TRIGGER_OUT_IO2, false);


                        _leftAndRightCameraGrabImageModuleActionFlag = -1;
                        _leftCameraImageInferModuleActionFlag = -1;
                        _rightCameraImageInferModuleActionFlag = -1;
                        _leftCameraSourceImageSaveModuleActionFlag = -1;
                        _rightCameraSourceImageSaveModuleActionFlag = -1;
                        _leftCameraInferredImageSaveModuleActionFlag = -1;
                        _rightCameraInferredImageSaveModuleActionFlag = -1;
                        _cameraInferDataClearModuleActionFlag = -1;

                        if (_lastTimepreprocessFilePath != "" || _lastTimeDimensionFilePath != "" || _lastTimeModelFilePath != "")
                        {
                            _newLeftDeepLearningObjectDetectionRectangle1Infer.Dispose();
                            _newRightDeepLearningObjectDetectionRectangle1Infer.Dispose();

                            _leftInferNeedReadParameter = true;
                            _rightInferNeedReadParameter = true;
                        }

                        //初始化推断及保存图片数据结构体
                        for (int i = 0; i < SAVE_IMAGE_QUANTITY_IN_RAM_FOR_RUNNING; i++)
                        {
                            _leftCameraInfer[i]._cameraPicturePointName = "";
                            _leftCameraInfer[i]._cameraInferredResultNGPictureSavePath = "";
                            _leftCameraInfer[i]._cameraInferredResultOKPictureSavePath = "";
                            _leftCameraInfer[i]._cameraPictureInferredFlag = false;
                            _leftCameraInfer[i]._cameraPicturePointNumber = 0;
                            _leftCameraInfer[i]._cameraPictureRequestInferFlag = false;
                            _leftCameraInfer[i]._cameraPictureSavedFlag = false;
                            _leftCameraInfer[i]._cameraPictureSavePath = "";
                            _leftCameraInfer[i]._productIndex = -1;
                            _leftCameraInfer[i].ho_CameraImage = null;

                            _rightCameraInfer[i]._cameraPicturePointName = "";
                            _rightCameraInfer[i]._cameraInferredResultNGPictureSavePath = "";
                            _rightCameraInfer[i]._cameraInferredResultOKPictureSavePath = "";
                            _rightCameraInfer[i]._cameraPictureInferredFlag = false;
                            _rightCameraInfer[i]._cameraPicturePointNumber = 0;
                            _rightCameraInfer[i]._cameraPictureRequestInferFlag = false;
                            _rightCameraInfer[i]._cameraPictureSavedFlag = false;
                            _rightCameraInfer[i]._cameraPictureSavePath = "";
                            _rightCameraInfer[i]._productIndex = -1;
                            _rightCameraInfer[i].ho_CameraImage = null;

                            ho_LeftInferredResultPictureStoreArray[i] = null;
                            ho_RightInferredResultPictureStoreArray[i] = null;
                            _leftInferredResultPictureSavePathArray[i] = "";
                            _rightInferredResultPictureSavePathArray[i] = "";
                        }


                        //如果记录的为需要检测，并且减速传感器有信号
                        if (_conveyor1Status != 2 && _axisControlFormVariate._ioStatus[0, 89])//
                        {
                            _conveyor1HaveProductFlag = true;
                            _conveyor1Status = 0;
                        }
                        else if (_conveyor1Status != 2 && !_axisControlFormVariate._ioStatus[0, 89])//
                        {
                            _conveyor1HaveProductFlag = false;
                            _conveyor1Status = 0;
                        }

                        //如果记录的为需要检测，并且减速传感器有信号
                        if (_conveyor2Status != 2 && _axisControlFormVariate._ioStatus[0, 88])//
                        {
                            _conveyor2HaveProductFlag = true;
                            _conveyor2Status = 0;
                        }
                        else if (_conveyor2Status != 2 && !_axisControlFormVariate._ioStatus[0, 88])//
                        {
                            _conveyor2HaveProductFlag = false;
                            _conveyor2Status = 0;
                        }
                        //_leftCameraCs.ClearCameraImage();
                       //_rightCameraCs.ClearCameraImage();
                        MyTool.TxtFileProcess.CreateLog("设备动作-切换设备模式为自动模式，参数初始化完成！" + "----用户：" + _operatorName,
                        BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

                        _autoModeFlag = true;

                        //清除传送线强制流出产品状态
                        _conveyor1ProductForcedUnloadModuleActionFlag = -1;
                        _conveyor2ProductForcedUnloadModuleActionFlag = -1;

                        //_autoStartingFlag = false;
                    }
                }

                #region 步进，trigger功能
                if (MainForm._stepModeFlag && !_pauseAlarmOccurFlag && !_afterPressResetBtnNeedPressStartBtnFlag && _autoModeFlag)//如果为步进模式
                {
                    _startBtnLampOnOffFlag = 2;//如果为步进设置启动按钮灯模式为500ms闪烁模式
                }
                else if (!MainForm._stepModeFlag && !_pauseAlarmOccurFlag && !_afterPressResetBtnNeedPressStartBtnFlag && _autoModeFlag)
                {
                    _startBtnLampOnOffFlag = 1;//如果不为步进模式，并且无达到暂停级别的报警，设置启动按钮为常亮
                }

                #region trigger flag的设定
                if (MainForm._stepModeFlag)
                {
                    _deviceWorkTriggerFlag = false;
                    if (_inputIOT[START_BT_IN_IO_NO].STATUS && !_deviceWorkTriggerFlagTriggerOneCircleFlag &&
                        !_pauseAlarmOccurFlag && !_afterPressResetBtnNeedPressStartBtnFlag
                         &&_inputIOT[10].STATUS && _inputIOT[11].STATUS)//如果启动按钮ON，并且没有报警以及以及按下启动按钮，以及安全光栅为ON
                    {
                        _deviceWorkTriggerFlag = true;
                        _deviceWorkTriggerFlagTriggerOneCircleFlag = true;
                    }

                    if (!_inputIOT[START_BT_IN_IO_NO].STATUS)
                    {
                        _deviceWorkTriggerFlagTriggerOneCircleFlag = false;
                    }
                }
                else
                {
                    if (!_pauseAlarmOccurFlag && !_afterPressResetBtnNeedPressStartBtnFlag
                         &&_inputIOT[10].STATUS && _inputIOT[11].STATUS)//io 10和11为安全门
                    {
                        _deviceWorkTriggerFlag = true;
                    }
                    else
                    {
                        _deviceWorkTriggerFlag = false;
                    }
                }
                #endregion

                #endregion
                Thread.Sleep(2);
            }
        }

        #endregion

        #region 手动运动

        /// <summary>
        /// SettingsBaseFormRequestXYMoveToAssignedPointEventFunc:XY轴移动至指定点位坐标事件函数，来自maintenance axes control form
        /// </summary>
        /// <param name="assignedPointNo">int:目标点位号</param>
        private void SettingsBaseFormRequestXYMoveToAssignedPointEventFunc(int assignedPointNo, int trackIndex)
        {
            if (_axisControlFormVariate != null && _axisControlFormVariate._motionCardInitialStatus)
            {
                if (_deadlyAlarmOccurFlag)
                {
                    MyTool.TxtFileProcess.CreateLog("轴手动控制页面，移动XYR至指定点位失败，当前设备有致命报警发生！" + "----用户：" + _operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);
                    MessageBox.Show("有致命报警发生，无法控制XYR轴移动至指定点位号！");
                    return;
                }

                if (!_axisControlFormVariate._axisUseFlag[0] || !_axisControlFormVariate._axisUseFlag[1] || !_axisControlFormVariate._axisUseFlag[2])
                {
                    MyTool.TxtFileProcess.CreateLog("轴手动控制页面，移动XYR至指定点位失败，XYR轴中至少有一轴处于禁用状态！" + "----用户：" + _operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);
                    MessageBox.Show("XYR轴中至少有一个轴未被设置为启用状态，无法控制XYR轴移动至指定点位号！");
                    return;
                }

                if (!_axisControlFormVariate._axisHomedFlag[0] || !_axisControlFormVariate._axisHomedFlag[1] || !_axisControlFormVariate._axisHomedFlag[2])
                {
                    MyTool.TxtFileProcess.CreateLog("轴手动控制页面，移动XYR至指定点位失败，XYR轴中至少有一轴处于未回原点状态！" + "----用户：" + _operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                    MessageBox.Show("XYR轴中至少有一个轴完成原点复归，无法控制XYR轴移动至指定点位号！");
                    return;
                }

                if (_stateAndAlarmCs._alarmFlag[100] || _stateAndAlarmCs._alarmFlag[101] || _stateAndAlarmCs._alarmFlag[102])
                {
                    MyTool.TxtFileProcess.CreateLog("轴手动控制页面，移动XYR至指定点位失败，XYR轴中至少有一轴处于伺服报警状态！" + "----用户：" + _operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);
                    MessageBox.Show("有轴处于伺服报警状态，无法控制XYR轴移动至指定点位号！");
                    return;
                }

                if (_stateAndAlarmCs._alarmFlag[132] || _stateAndAlarmCs._alarmFlag[133] || _stateAndAlarmCs._alarmFlag[134] || _stateAndAlarmCs._alarmFlag[135] || _stateAndAlarmCs._alarmFlag[136] || _stateAndAlarmCs._alarmFlag[137])
                {
                    MyTool.TxtFileProcess.CreateLog("轴手动控制页面，移动XYR至指定点位失败，XYR轴中至少有一轴处于极限传感器状态！" + "----用户：" + _operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);
                    MessageBox.Show("有轴处于极限报警状态，无法控制XYR轴移动至指定点位号！");
                    return;
                }

                if (_axisControlFormVariate._axisActionCommand[0] != -1 || _axisControlFormVariate._axisActionCommand[1] != -1 || _axisControlFormVariate._axisActionCommand[2] != -1)
                {
                    MyTool.TxtFileProcess.CreateLog("轴手动控制页面，移动XYR至指定点位失败，XYR轴中至少有一轴处于繁忙状态！" + "----用户：" + _operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);
                    MessageBox.Show("X轴或Y轴处于繁忙状态，无法控制XYR轴移动至指定点位号！");
                    return;
                }

                if (_autoModeFlag)
                {
                    MyTool.TxtFileProcess.CreateLog("轴手动控制页面，移动XYR至指定点位失败，设备处于自动模式！" + "----用户：" + _operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);
                    MessageBox.Show("设备处于自动模式，无法控制XY轴移动至指定点位号！");
                    return;
                }

                if (!_xyMoveToAssignedPointPositionModuleOffDlyT.STATUS)
                {
                    MyTool.TxtFileProcess.CreateLog("轴手动控制页面，移动XYR至指定点位失败，轴定位模块处于繁忙状态！" + "----用户：" + _operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);
                    MessageBox.Show("模块处于繁忙状态，无法控制XY轴移动至指定点位号！");
                    return;
                }

                //异步执行运动
                Task.Run(() =>
                {
                    MoveXYAxisToAssignedPointModule(assignedPointNo, 100, trackIndex);
                });
            }
            else
            {

                MyTool.TxtFileProcess.CreateLog("轴手动控制页面，移动XYR至指定点位失败，运动控制卡异常！" + "----用户：" + _operatorName,
                BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);
                MessageBox.Show("运动控制卡异常，无法控制XY轴移动至指定点位号！");
                return;
            }
        }

        /// <summary>
        /// 轴控制页面，请求调节传送线宽度事件
        /// </summary>
        /// <param name="conveyorControlFlag">int:控制传送线标志，0-调节传送线1，1-调节传送线2，2-所有传送线都调节</param>
        private void MaintenanceAxesControlFormRequestChangeConveyorWidthEventFunc(int conveyorControlFlag)
        {
            string conveyorName = "";
            switch (conveyorControlFlag)
            {
                case 0:
                    conveyorName = "传送线1";
                    break;
                case 1:
                    conveyorName = "传送线2";
                    break;
                case 2:
                    conveyorName = "传送线1和传送线2";
                    break;
            }

            if (_axisControlFormVariate != null && _axisControlFormVariate._motionCardInitialStatus)
            {
                if (_deadlyAlarmOccurFlag)
                {

                    MyTool.TxtFileProcess.CreateLog("轴手动控制页面，调节" + conveyorName + "失败，当前有致命报警发生！" + "----用户：" + _operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                    MessageBox.Show("有致命报警发生，无法调节传送线宽度！");
                    return;
                }

                if (!_axisControlFormVariate._axisUseFlag[3] || !_axisControlFormVariate._axisUseFlag[4] || !_axisControlFormVariate._axisUseFlag[5])
                {
                    MyTool.TxtFileProcess.CreateLog("轴手动控制页面，调节" + conveyorName + "失败，当前有传送线宽带控制轴处于禁用状态！" + "----用户：" + _operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                    MessageBox.Show("Y2,Y3,Y4轴中至少有一个轴未被设置为启用状态，无法调节传送线宽度！");
                    return;
                }

                if (!_axisControlFormVariate._axisHomedFlag[3] || !_axisControlFormVariate._axisHomedFlag[4] || !_axisControlFormVariate._axisHomedFlag[5])
                {
                    MyTool.TxtFileProcess.CreateLog("轴手动控制页面，调节" + conveyorName + "失败，当前有传送线宽带控制轴处于未完成原点复归状态！" + "----用户：" + _operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                    MessageBox.Show("Y2,Y3,Y4轴中至少有一个轴完成原点复归，无法调节传送线宽度！");
                    return;
                }


                if (_axisControlFormVariate._axisActionCommand[3] != -1 || _axisControlFormVariate._axisActionCommand[4] != -1 || _axisControlFormVariate._axisActionCommand[5] != -1)
                {
                    MyTool.TxtFileProcess.CreateLog("轴手动控制页面，调节" + conveyorName + "失败，当前有传送线宽带控制轴处于繁忙状态！" + "----用户：" + _operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                    MessageBox.Show("Y2,Y3,Y4轴中至少有一个轴处于繁忙状态，无法调节传送线宽度！");
                    return;
                }

                if (BaseForm._autoModeFlag)
                {
                    MyTool.TxtFileProcess.CreateLog("轴手动控制页面，调节" + conveyorName + "失败，设备处于自动模式！" + "----用户：" + _operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);
                    MessageBox.Show("设备处于自动模式，无法调节传送线宽度！");
                    return;
                }

                if (!_conveyorChangeToAssignedWidthModuleOffDlyT.STATUS)
                {
                    MyTool.TxtFileProcess.CreateLog("轴手动控制页面，调节" + conveyorName + "失败，传送线宽度调节模块处于繁忙状态！" + "----用户：" + _operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);
                    MessageBox.Show("模块处于繁忙状态，无法调节传送线宽度！");
                    return;
                }

                //异步执行运动
                ConveyorChangeToAssignedWidthModule(conveyorControlFlag);
            }
            else
            {
                MyTool.TxtFileProcess.CreateLog("轴手动控制页面，调节" + conveyorName + "失败，运动控制卡异常！" + "----用户：" + _operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);
                MessageBox.Show("运动控制卡异常，无法调节传送线宽度！");
                return;
            }

        }

        /// <summary>
        /// MaintenanceCylinderControlFormCylinderControlEventFunc:手动气缸控制界面，气缸控制事件函数
        /// </summary>
        /// <param name="cylinderControlIndex">cylinderControlIndex:气缸控制索引，0-相机移动气缸回，1-相机移动气缸出，
        /// 2-传送线1阻挡气缸下，3-传送线1阻挡气缸上，4-传送线1夹紧气缸松开，5-传送线1夹紧气缸夹紧，
        /// 6-传送线2阻挡气缸下，7-传送线2阻挡气缸上，8-传送线2夹紧气缸松开，9-传送线2夹紧气缸夹紧</param>
        private void MaintenanceCylinderControlFormCylinderControlEventFunc(int cylinderControlIndex)
        {
            switch (cylinderControlIndex)
            {
                case 0:
                    MyTool.TxtFileProcess.CreateLog("气缸手动控制页面，控制相机移动气缸回！" + "----用户：" + _operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                    _axisControlFormVariate.OutputControl(4, true);
                    _axisControlFormVariate.OutputControl(5, false);
                    break;
                case 1:
                    MyTool.TxtFileProcess.CreateLog("气缸手动控制页面，控制相机移动气缸出！" + "----用户：" + _operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                    _axisControlFormVariate.OutputControl(4, false);
                    _axisControlFormVariate.OutputControl(5, true);
                    break;
                case 2:
                    MyTool.TxtFileProcess.CreateLog("气缸手动控制页面，控制传送线1阻挡气缸下！" + "----用户：" + _operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                    _axisControlFormVariate.OutputControl(80, true);
                    _axisControlFormVariate.OutputControl(81, false);
                    break;
                case 3:
                    MyTool.TxtFileProcess.CreateLog("气缸手动控制页面，控制传送线1阻挡气缸上！" + "----用户：" + _operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                    _axisControlFormVariate.OutputControl(80, false);
                    _axisControlFormVariate.OutputControl(81, true);
                    break;
                case 4:
                    MyTool.TxtFileProcess.CreateLog("气缸手动控制页面，控制传送线1夹紧气缸松开！" + "----用户：" + _operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                    _axisControlFormVariate.OutputControl(82, true);
                    _axisControlFormVariate.OutputControl(83, false);
                    break;
                case 5:
                    MyTool.TxtFileProcess.CreateLog("气缸手动控制页面，控制传送线1夹紧气缸夹紧！" + "----用户：" + _operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                    _axisControlFormVariate.OutputControl(82, false);
                    _axisControlFormVariate.OutputControl(83, true);
                    break;
                case 6:
                    MyTool.TxtFileProcess.CreateLog("气缸手动控制页面，控制传送线2阻挡气缸下！" + "----用户：" + _operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                    _axisControlFormVariate.OutputControl(76, true);
                    _axisControlFormVariate.OutputControl(77, false);
                    break;
                case 7:
                    MyTool.TxtFileProcess.CreateLog("气缸手动控制页面，控制传送线2阻挡气缸上！" + "----用户：" + _operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                    _axisControlFormVariate.OutputControl(76, false);
                    _axisControlFormVariate.OutputControl(77, true);
                    break;
                case 8:
                    MyTool.TxtFileProcess.CreateLog("气缸手动控制页面，控制传送线2夹紧气缸松开！" + "----用户：" + _operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                    _axisControlFormVariate.OutputControl(78, true);
                    _axisControlFormVariate.OutputControl(79, false);
                    break;
                case 9:
                    MyTool.TxtFileProcess.CreateLog("气缸手动控制页面，控制传送线2夹紧气缸夹紧！" + "----用户：" + _operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                    _axisControlFormVariate.OutputControl(78, false);
                    _axisControlFormVariate.OutputControl(79, true);
                    break;
            }
        }

        /// <summary>
        /// MaintenanceAxesControlFormRequestConveyorActionDelegate:声明委托，轴控制手动画面请求控制传送线
        /// </summary>
        /// <param name="conveyorActionSpeed">float:传送线动作速度，小于等于0-停止，其余-其余动作速度</param>
        /// <param name="conveyorIndex">int:控制的传送线索引，0-传送线1，1-传送线2</param>
        private void MaintenanceAxesControlFormRequestConveyorActionEventFunc(float conveyorActionSpeed, int conveyorIndex)
        {
            if (_deadlyAlarmOccurFlag)
            {
                MyTool.TxtFileProcess.CreateLog("轴手动控制页面，控制传送线" + (conveyorIndex + 1).ToString() + "动作失败，当前设备有致命报警发生！" + "----用户：" + _operatorName,
                BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);
                MessageBox.Show("有致命报警发生，无法控制传送线！");
                return;
            }

            if (_autoModeFlag)
            {
                MyTool.TxtFileProcess.CreateLog("轴手动控制页面，控制传送线" + (conveyorIndex + 1).ToString() + "动作失败，当前设备处于自动模式！" + "----用户：" + _operatorName,
                BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);
                MessageBox.Show("当前设备处于自动模式，无法控制传送线！");
                return;
            }

            if ((!_conveyor1ActionModuleOffDlyT.STATUS && conveyorIndex == 0) ||
               (!_conveyor2ActionModuleOffDlyT.STATUS && conveyorIndex == 1))
            {
                MyTool.TxtFileProcess.CreateLog("轴手动控制页面，控制传送线" + (conveyorIndex + 1).ToString() + "动作失败，当前控制模块繁忙！" + "----用户：" + _operatorName,
                BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                MessageBox.Show("传送线控制模块繁忙，无法控制传送线！");
                return;
            }

            if (conveyorIndex == 0)
                Conveyor1ActionModule(conveyorActionSpeed);
            else if (conveyorIndex == 1)
                Conveyor2ActionModule(conveyorActionSpeed);

        }

        /// <summary>
        /// Conveyor1ForceOutProduct:传送线1强制下料模块
        /// </summary>
        private async void Conveyor1ForceOutProduct()
        {
            await Task.Run(() =>
            {
                _conveyor1ProductForcedUnloadModuleActionFlag = 0;

                MyTool.TxtFileProcess.CreateLog("设备手动动作-强制流出传送线1的产品！" + "----用户：" + _operatorName,
                BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

                //清除timeout deal with way index
                _conveyor1TimeoutAlarmDealWithWayIndexWhenLoadAndUnload = -1;

                _conveyor1HaveProductFlag = false;

                lock (this)
                {
                    //控制夹紧气缸松开
                    _axisControlFormVariate.OutputControl(82, true);
                    _axisControlFormVariate.OutputControl(83, false);
                }

                while (!_inputIOT[86].STATUS)//等待夹紧气缸松开
                {
                    if (_deadlyAlarmOccurFlag || _autoModeFlag)
                    {
                        return;
                    }

                    Thread.Sleep(1);
                }

                lock (this)
                {
                    //控制阻挡气缸下
                    _axisControlFormVariate.OutputControl(80, true);
                    _axisControlFormVariate.OutputControl(81, false);
                }

                while (!_inputIOT[84].STATUS)//等待阻挡气缸下
                {
                    if (_deadlyAlarmOccurFlag || _autoModeFlag)
                    {
                        return;
                    }
                    Thread.Sleep(1);
                }

                while (_conveyor1ActionModuleActionFlag == 0 || _stateAndAlarmCs._alarmFlag[341] || _conveyor1TimeoutAlarmDealWithWayIndexWhenLoadAndUnload != -1)
                {
                    if (_deadlyAlarmOccurFlag || _autoModeFlag)
                    {
                        return;
                    }
                    if (_conveyor1TimeoutAlarmDealWithWayIndexWhenLoadAndUnload == 1)
                    {
                        _conveyor1TimeoutAlarmDealWithWayIndexWhenLoadAndUnload = -1;
                        break;
                    }
                    switch (_conveyor1TimeoutAlarmDealWithWayIndexWhenLoadAndUnload)
                    {
                        case 0://如果为重试，不进行任何动作
                            _conveyor1TimeoutAlarmDealWithWayIndexWhenLoadAndUnload = -1;
                            break;
                        //case 1://如果为忽略
                        // _conveyor1TimeoutAlarmDealWithWayIndexWhenLoadAndUnload = -1;
                        //continue;
                        case 2://如果为无物料
                            _conveyor1Status = 0;
                            _conveyor1ProductForcedUnloadModuleActionFlag = 1;
                            _conveyor1TimeoutAlarmDealWithWayIndexWhenLoadAndUnload = -1;
                            return;
                    }

                    Thread.Sleep(1);
                }

                _conveyor1ActionModuleActionFlag = 0;
                Conveyor1ActionModule (200);

                while (_conveyor1ActionModuleActionFlag != 1)//如果传送线未完成速度设定
                {
                    if (_deadlyAlarmOccurFlag || _autoModeFlag)
                    {
                        return;
                    }
                    Thread.Sleep(1);
                }

                while (!_axisControlFormVariate._ioStatus[0,93] || _stateAndAlarmCs._alarmFlag[341] || _conveyor1TimeoutAlarmDealWithWayIndexWhenLoadAndUnload != -1)//等待出口传感器触发
                {
                    if (_deadlyAlarmOccurFlag || _autoModeFlag)
                    {
                        return;
                    }

                    if (_conveyor1TimeoutAlarmDealWithWayIndexWhenLoadAndUnload == 1)
                    {
                        _conveyor1TimeoutAlarmDealWithWayIndexWhenLoadAndUnload = -1;
                        break;
                    }
                    switch (_conveyor1TimeoutAlarmDealWithWayIndexWhenLoadAndUnload)
                    {
                        case 0://如果为重试，不进行任何动作
                            _conveyor1TimeoutAlarmDealWithWayIndexWhenLoadAndUnload = -1;
                            break;
                        //case 1://如果为忽略
                        //_conveyor1TimeoutAlarmDealWithWayIndexWhenLoadAndUnload = -1;
                        //continue;
                        case 2://如果为无物料
                            while (_conveyor1ActionModuleActionFlag == 0)//等待控制传送线动作模块空闲 !_deviceWorkTriggerFlag || 
                            {
                                if (_deadlyAlarmOccurFlag || _autoModeFlag)
                                {
                                    return;
                                }
                                Thread.Sleep(1);
                            }

                            _conveyor1ActionModuleActionFlag = 0;
                            Conveyor1ActionModule(-1);//控制传送线停止

                            while (_conveyor1ActionModuleActionFlag != 2)//如果传送线未完成速度设定
                            {
                                if (_deadlyAlarmOccurFlag || _autoModeFlag)
                                {
                                    return;
                                }
                                Thread.Sleep(1);
                            }
                            _conveyor1Status = 0;
                            _conveyor1ProductForcedUnloadModuleActionFlag = 1;
                            _conveyor1TimeoutAlarmDealWithWayIndexWhenLoadAndUnload = -1;
                            return;
                    }

                    Thread.Sleep(1);
                }

                if (_inputIOT[71].STATUS)//如果接收到下游允许出板请求信号
                {
                    while (_axisControlFormVariate._ioStatus[0, 93] || _stateAndAlarmCs._alarmFlag[341] || _conveyor1TimeoutAlarmDealWithWayIndexWhenLoadAndUnload != -1)//如果传送线未触发下料完成
                    {
                        if (_deadlyAlarmOccurFlag || _autoModeFlag)
                        {
                            return;
                        }

                        if (_conveyor1TimeoutAlarmDealWithWayIndexWhenLoadAndUnload == 1)
                        {
                            _conveyor1TimeoutAlarmDealWithWayIndexWhenLoadAndUnload = -1;
                            break;
                        }
                        switch (_conveyor1TimeoutAlarmDealWithWayIndexWhenLoadAndUnload)
                        {
                            case 0://如果为重试，不进行任何动作
                                _conveyor1TimeoutAlarmDealWithWayIndexWhenLoadAndUnload = -1;
                                break;
                            //case 1://如果为忽略
                            // _conveyor1TimeoutAlarmDealWithWayIndexWhenLoadAndUnload = -1;
                            //continue;
                            case 2://如果为无物料
                                while (_conveyor1ActionModuleActionFlag == 0)//等待控制传送线动作模块空闲 !_deviceWorkTriggerFlag || 
                                {
                                    if (_deadlyAlarmOccurFlag || _autoModeFlag)
                                    {
                                        return;
                                    }
                                    Thread.Sleep(1);
                                }
                                _conveyor1ActionModuleActionFlag = 0;
                                Conveyor1ActionModule(-1);//控制传送线停止
                                while (_conveyor1ActionModuleActionFlag != 2)//如果传送线未完成速度设定
                                {
                                    if (_deadlyAlarmOccurFlag || _autoModeFlag)
                                    {
                                        return;
                                    }
                                    Thread.Sleep(1);
                                }
                                _conveyor1Status = 0;
                                _conveyor1ProductForcedUnloadModuleActionFlag = 1;
                                _conveyor1TimeoutAlarmDealWithWayIndexWhenLoadAndUnload = -1;
                                return;
                        }

                        Thread.Sleep(1);
                    }
                }

                while (_conveyor1ActionModuleActionFlag == 0)
                {
                    if (_deadlyAlarmOccurFlag || _autoModeFlag)
                    {
                        return;
                    }
                    Thread.Sleep(1);
                }

                //控制传送线停止
                _conveyor1ActionModuleActionFlag = 0;
                Conveyor1ActionModule(-1);

                while (_conveyor1ActionModuleActionFlag != 2)//如果传送线未完成停止
                {
                    if (_deadlyAlarmOccurFlag || _autoModeFlag)
                    {
                        return;
                    }

                    Thread.Sleep(1);
                }

                MyTool.TxtFileProcess.CreateLog("设备手动动作-强制流出传送线1的产品成功！" + "----用户：" + _operatorName,
                BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

                _conveyor1Status = 0;//修改传送线状态为需要上料
                _conveyor1ProductForcedUnloadModuleActionFlag = 1;
            });
        }

        /// <summary>
        /// Conveyor2ForceOutProduct:传送线2强制下料模块
        /// </summary>
        private async void Conveyor2ForceOutProduct()
        {
            await Task.Run(() =>
            {
                _conveyor2ProductForcedUnloadModuleActionFlag = 0;

                MyTool.TxtFileProcess.CreateLog("设备手动动作-强制流出传送线2的产品！" + "----用户：" + _operatorName,
                BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

                //清除timeout deal with way index
                _conveyor2TimeoutAlarmDealWithWayIndexWhenLoadAndUnload = -1;

                _conveyor2HaveProductFlag = false;

                lock (this)
                {
                    //控制夹紧气缸松开
                    _axisControlFormVariate.OutputControl(78, true);
                    _axisControlFormVariate.OutputControl(79, false);
                }

                while (!_inputIOT[82].STATUS)//等待夹紧气缸松开
                {
                    if (_deadlyAlarmOccurFlag || _autoModeFlag)
                    {
                        return;
                    }

                    Thread.Sleep(1);
                }

                lock (this)
                {
                    //控制阻挡气缸下
                    _axisControlFormVariate.OutputControl(76, true);
                    _axisControlFormVariate.OutputControl(77, false);
                }

                while (!_inputIOT[80].STATUS)//等待阻挡气缸下
                {
                    if (_deadlyAlarmOccurFlag || _autoModeFlag)
                    {
                        return;
                    }
                    Thread.Sleep(1);
                }

                while (_conveyor1ActionModuleActionFlag == 0 || _stateAndAlarmCs._alarmFlag[343] || _conveyor2TimeoutAlarmDealWithWayIndexWhenLoadAndUnload != -1)
                {
                    if (_deadlyAlarmOccurFlag || _autoModeFlag)
                    {
                        return;
                    }
                    if (_conveyor2TimeoutAlarmDealWithWayIndexWhenLoadAndUnload == 1)
                    {
                        _conveyor2TimeoutAlarmDealWithWayIndexWhenLoadAndUnload = -1;
                        break;
                    }
                    switch (_conveyor2TimeoutAlarmDealWithWayIndexWhenLoadAndUnload)
                    {
                        case 0://如果为重试，不进行任何动作
                            _conveyor2TimeoutAlarmDealWithWayIndexWhenLoadAndUnload = -1;
                            break;
                        //case 1://如果为忽略
                        // _conveyor1TimeoutAlarmDealWithWayIndexWhenLoadAndUnload = -1;
                        //continue;
                        case 2://如果为无物料
                            _conveyor2Status = 0;
                            _conveyor2ProductForcedUnloadModuleActionFlag = 1;
                            _conveyor2TimeoutAlarmDealWithWayIndexWhenLoadAndUnload = -1;
                            return;
                    }

                    Thread.Sleep(1);
                }

                _conveyor2ActionModuleActionFlag = 0;
                Conveyor2ActionModule(200);

                while (_conveyor2ActionModuleActionFlag != 1)//如果传送线未完成速度设定
                {
                    if (_deadlyAlarmOccurFlag || _autoModeFlag)
                    {
                        return;
                    }
                    Thread.Sleep(1);
                }

                while (!_axisControlFormVariate._ioStatus[0, 91] || _stateAndAlarmCs._alarmFlag[343] || _conveyor2TimeoutAlarmDealWithWayIndexWhenLoadAndUnload != -1)//等待出口传感器触发
                {
                    if (_deadlyAlarmOccurFlag || _autoModeFlag)
                    {
                        return;
                    }

                    if (_conveyor2TimeoutAlarmDealWithWayIndexWhenLoadAndUnload == 1)
                    {
                        _conveyor2TimeoutAlarmDealWithWayIndexWhenLoadAndUnload = -1;
                        break;
                    }
                    switch (_conveyor2TimeoutAlarmDealWithWayIndexWhenLoadAndUnload)
                    {
                        case 0://如果为重试，不进行任何动作
                            _conveyor2TimeoutAlarmDealWithWayIndexWhenLoadAndUnload = -1;
                            break;
                        //case 1://如果为忽略
                        //_conveyor1TimeoutAlarmDealWithWayIndexWhenLoadAndUnload = -1;
                        //continue;
                        case 2://如果为无物料
                            while (_conveyor2ActionModuleActionFlag == 0)//等待控制传送线动作模块空闲 !_deviceWorkTriggerFlag || 
                            {
                                if (_deadlyAlarmOccurFlag || _autoModeFlag)
                                {
                                    return;
                                }
                                Thread.Sleep(1);
                            }

                            _conveyor2ActionModuleActionFlag = 0;
                            Conveyor2ActionModule(-1);//控制传送线停止

                            while (_conveyor2ActionModuleActionFlag != 2)//如果传送线未完成速度设定
                            {
                                if (_deadlyAlarmOccurFlag || _autoModeFlag)
                                {
                                    return;
                                }
                                Thread.Sleep(1);
                            }
                            _conveyor2Status = 0;
                            _conveyor2ProductForcedUnloadModuleActionFlag = 1;
                            _conveyor2TimeoutAlarmDealWithWayIndexWhenLoadAndUnload = -1;
                            return;
                    }

                    Thread.Sleep(1);
                }

                if (_inputIOT[70].STATUS)//如果接收到下游允许出板请求信号
                {
                    while (_axisControlFormVariate._ioStatus[0, 91] || _stateAndAlarmCs._alarmFlag[343] || _conveyor2TimeoutAlarmDealWithWayIndexWhenLoadAndUnload != -1)//如果传送线未触发下料完成
                    {
                        if (_deadlyAlarmOccurFlag || _autoModeFlag)
                        {
                            return;
                        }

                        if (_conveyor2TimeoutAlarmDealWithWayIndexWhenLoadAndUnload == 1)
                        {
                            _conveyor2TimeoutAlarmDealWithWayIndexWhenLoadAndUnload = -1;
                            break;
                        }
                        switch (_conveyor2TimeoutAlarmDealWithWayIndexWhenLoadAndUnload)
                        {
                            case 0://如果为重试，不进行任何动作
                                _conveyor2TimeoutAlarmDealWithWayIndexWhenLoadAndUnload = -1;
                                break;
                            //case 1://如果为忽略
                            // _conveyor1TimeoutAlarmDealWithWayIndexWhenLoadAndUnload = -1;
                            //continue;
                            case 2://如果为无物料
                                while (_conveyor2ActionModuleActionFlag == 0)//等待控制传送线动作模块空闲 !_deviceWorkTriggerFlag || 
                                {
                                    if (_deadlyAlarmOccurFlag || _autoModeFlag)
                                    {
                                        return;
                                    }
                                    Thread.Sleep(1);
                                }
                                _conveyor2ActionModuleActionFlag = 0;
                                Conveyor2ActionModule(-1);//控制传送线停止
                                while (_conveyor2ActionModuleActionFlag != 2)//如果传送线未完成速度设定
                                {
                                    if (_deadlyAlarmOccurFlag || _autoModeFlag)
                                    {
                                        return;
                                    }
                                    Thread.Sleep(1);
                                }
                                _conveyor2Status = 0;
                                _conveyor2ProductForcedUnloadModuleActionFlag = 1;
                                _conveyor2TimeoutAlarmDealWithWayIndexWhenLoadAndUnload = -1;
                                return;
                        }

                        Thread.Sleep(1);
                    }
                }

                while (_conveyor2ActionModuleActionFlag == 0)
                {
                    if (_deadlyAlarmOccurFlag || _autoModeFlag)
                    {
                        return;
                    }
                    Thread.Sleep(1);
                }

                //控制传送线停止
                _conveyor2ActionModuleActionFlag = 0;
                Conveyor2ActionModule(-1);

                while (_conveyor2ActionModuleActionFlag != 2)//如果传送线未完成停止
                {
                    if (_deadlyAlarmOccurFlag || _autoModeFlag)
                    {
                        return;
                    }

                    Thread.Sleep(1);
                }

                MyTool.TxtFileProcess.CreateLog("设备手动动作-强制流出传送线2的产品成功！" + "----用户：" + _operatorName,
                BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

                _conveyor2Status = 0;//修改传送线状态为需要上料
                _conveyor2ProductForcedUnloadModuleActionFlag = 1;
            });
        }

        #endregion

        #region 相机
        /// <summary>
        /// OpenCamera:打开相机
        /// </summary>
        /// <param name="cameraControlCs">HalconCameraControl:指定相机控制类名</param>
        /// <param name="deviceName">HTuple:相机名称</param>
        /// <param name="externalTriggerFlag">bool:是否使用外部触发，如果使用外部触发，那么调用外部触发打开相机函数</param>
        /// <returns>bool:返回相机打开结果</returns>
        private bool OpenCamera(ref HalconCameraControl cameraControlCs, HTuple deviceName, bool externalTriggerFlag,bool dispErrorMsgFlag)
        {
            cameraControlCs.hv_AcqHandle = null;
            if (externalTriggerFlag)
            {
                if (cameraControlCs.OpenCameraInExternalTrigger("GigEVision2", 0, 0, 0, 0, 0, 0,
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
                if (cameraControlCs.OpenCameraInSoftTrigger("GigEVision2", 0, 0, 0, 0, 0, 0,
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

        /// <summary>
        /// CloseCamera:关闭相机
        /// </summary>
        /// <param name="cameraControlCs">HalconCameraControl:指定相机控制类名</param>
        /// <returns>bool:返回相机关闭结果</returns>
        private bool CloseCamera(ref HalconCameraControl cameraControlCs)
        {
            return cameraControlCs.CloseCamera();
        }

        /// <summary>
        /// CameraImageGrabAndDispThreadFunc:相机图像抓取显示线程函数
        /// </summary>
        private void CameraImageGrabAndDispThreadFunc()
        {
            bool tempbool = false;//用于跳出某些步骤
            while (true)
            {
                if (_leftCameraVideo || _leftCameraGrab)//如果触发左侧相机实时显示标志或抓取图像标志
                {
                    if (_leftCameraCs.hv_AcqHandle == null)
                    {
                        if (!OpenCamera(ref _leftCameraCs, SettingsAdministratorForm._leftCameraName, true, true))
                        {
                            MyTool.TxtFileProcess.CreateLog("相机手动控制页面，控制左侧相机动作失败！无法打开相机！" + "----用户：" + _operatorName,
                            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                            MessageBox.Show("打开左侧相机失败");
                            _leftCameraVideo = false;
                            _leftCameraGrab = false;
                        }
                        Thread.Sleep(100);//打开相机之后等待100ms
                    }

                    if (_leftCameraCs.hv_AcqHandle != null)
                    {
                        if (_axisControlFormVariate._motionCardInitialStatus)
                        {
                            tempbool = true;
                            if (_axisControlFormVariate._ioStatus[1, LEFT_CAMERA_TRIGGER_OUT_IO] || _axisControlFormVariate._ioStatus[1, LEFT_CAMERA_TRIGGER_OUT_IO2])
                            {
                                _axisControlFormVariate.OutputControl(LEFT_CAMERA_TRIGGER_OUT_IO, false);
                                _axisControlFormVariate.OutputControl(LEFT_CAMERA_TRIGGER_OUT_IO2, false);
                                while (_axisControlFormVariate._ioStatus[1, LEFT_CAMERA_TRIGGER_OUT_IO] || _axisControlFormVariate._ioStatus[1, LEFT_CAMERA_TRIGGER_OUT_IO2])
                                {
                                    if (_deadlyAlarmOccurFlag)
                                    {
                                        _leftCameraVideo = false;
                                        _leftCameraGrab = false;
                                        tempbool = false;
                                    }
                                    Thread.Sleep(10);
                                }
                            }

                            if (tempbool)
                            {
                                _axisControlFormVariate.OutputControl(LEFT_CAMERA_TRIGGER_OUT_IO, true);
                                _axisControlFormVariate.OutputControl(LEFT_CAMERA_TRIGGER_OUT_IO2, true);

                                Thread.Sleep(10);
                                if (_leftCameraCs.ho_Image != null)
                                {
                                    _leftCameraCs.ho_Image.Dispose();
                                    _leftCameraCs.ho_Image = null;
                                }
                                if (!_leftCameraCs.GrabImage(true))
                                {
                                    _leftCameraVideo = false;
                                    _leftCameraGrab = false;
                                    tempbool = false;
                                }

                                if (tempbool)
                                {
                                    _axisControlFormVariate.OutputControl(LEFT_CAMERA_TRIGGER_OUT_IO, false);
                                    _axisControlFormVariate.OutputControl(LEFT_CAMERA_TRIGGER_OUT_IO2, false);

                                    if (tempbool)
                                    {
                                        HalconCameraControl.DispImageAdaptively(_baseSettingFormVariate._maintenanceCameraFormVariate.picLeftCameraImageDisp, _leftCameraCs.ho_Image, false);
                                        if (_leftCameraGrab)
                                            _leftCameraGrab = false;
                                        Thread.Sleep(SettingsAdministratorForm._leftAndRightCameraGrabImageInterval);//等待相机抓取图像延时
                                    }
                                }
                            }
                        }
                        else
                        {
                            MyTool.TxtFileProcess.CreateLog("相机手动控制页面，控制左侧相机动作失败！未完成运动控制卡初始化！" + "----用户：" + _operatorName,
                            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                            MessageBox.Show("未完成运动控制卡初始化，无法触发左侧相机！");
                            _leftCameraVideo = false;
                            _leftCameraGrab = false;
                        }
                    }
                }

                if (_rightCameraVideo || _rightCameraGrab)//如果触发右侧相机实时显示标志或抓取图像标志
                {
                    if (_rightCameraCs.hv_AcqHandle == null)
                    {
                        if (!OpenCamera(ref _rightCameraCs, SettingsAdministratorForm._rightCameraName, true, true))
                        {
                            MyTool.TxtFileProcess.CreateLog("相机手动控制页面，控制右侧相机动作失败！无法打开相机！" + "----用户：" + _operatorName,
                            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                            MessageBox.Show("打开右侧相机失败");
                            _rightCameraVideo = false;
                            _rightCameraGrab = false;
                        }
                        Thread.Sleep(100);//打开相机之后等待100ms
                    }
                    if (_rightCameraCs.hv_AcqHandle != null)
                    {
                        if (_axisControlFormVariate._motionCardInitialStatus)
                        {
                            tempbool = true;
                            if (_axisControlFormVariate._ioStatus[1, RIGHT_CAMERA_TRIGGER_OUT_IO] || _axisControlFormVariate._ioStatus[1, RIGHT_CAMERA_TRIGGER_OUT_IO2])
                            {
                                _axisControlFormVariate.OutputControl(RIGHT_CAMERA_TRIGGER_OUT_IO, false);
                                _axisControlFormVariate.OutputControl(RIGHT_CAMERA_TRIGGER_OUT_IO2, false);

                                while (_axisControlFormVariate._ioStatus[1, RIGHT_CAMERA_TRIGGER_OUT_IO] || _axisControlFormVariate._ioStatus[1, RIGHT_CAMERA_TRIGGER_OUT_IO2])
                                {
                                    if (_deadlyAlarmOccurFlag)
                                    {
                                        _leftCameraVideo = false;
                                        _leftCameraGrab = false;
                                        tempbool = false;
                                    }
                                    Thread.Sleep(10);
                                }
                            }

                            if (tempbool)
                            {
                                _axisControlFormVariate.OutputControl(RIGHT_CAMERA_TRIGGER_OUT_IO, true);
                                _axisControlFormVariate.OutputControl(RIGHT_CAMERA_TRIGGER_OUT_IO2, true);

                                Thread.Sleep(10);
                                if (_rightCameraCs.ho_Image != null)
                                {
                                    _rightCameraCs.ho_Image.Dispose();
                                    _rightCameraCs.ho_Image = null;
                                }
                                if (!_rightCameraCs.GrabImage(true))
                                {
                                    _rightCameraVideo = false;
                                    _rightCameraGrab = false;
                                    tempbool = false;
                                }

                                if (tempbool)
                                {
                                    _axisControlFormVariate.OutputControl(RIGHT_CAMERA_TRIGGER_OUT_IO, false);
                                    _axisControlFormVariate.OutputControl(RIGHT_CAMERA_TRIGGER_OUT_IO2, false);

                                    if (tempbool)
                                    {
                                        HalconCameraControl.DispImageAdaptively(_baseSettingFormVariate._maintenanceCameraFormVariate.picRightCameraImageDisp, _rightCameraCs.ho_Image, false);
                                        if (_rightCameraGrab)
                                            _rightCameraGrab = false;
                                    }
                                }
                            }
                        }
                        else
                        {
                            MyTool.TxtFileProcess.CreateLog("相机手动控制页面，控制右侧相机动作失败！未完成运动控制卡初始化！" + "----用户：" + _operatorName,
                            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                            MessageBox.Show("未完成运动控制卡初始化，无法触发右侧相机！");
                            _rightCameraVideo = false;
                            _rightCameraGrab = false;
                            return;
                        }
                    }
                }

                if (_backCameraVideo || _backCameraGrab)//如果触发后侧相机实时显示标志或抓取图像标志
                {
                    if (_backCameraCs.hv_AcqHandle == null)
                    {
                        if (!OpenCamera(ref _backCameraCs, SettingsAdministratorForm._backCameraName, false, true))
                        {
                            MyTool.TxtFileProcess.CreateLog("相机手动控制页面，控制后侧相机动作失败！无法打开相机！" + "----用户：" + _operatorName,
                            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                            MessageBox.Show("打开后侧相机失败");
                            _backCameraGrab = false;
                            _backCameraVideo = false;
                        }
                        Thread.Sleep(100);//打开相机之后等待100ms
                    }
                    if (_backCameraCs.hv_AcqHandle != null)
                    {
                        if (_axisControlFormVariate._motionCardInitialStatus)
                        {
                            tempbool = true;
                            if (!_axisControlFormVariate._ioStatus[1, BACK_CAMERA_LIGHT_TRIGGER_OUT_IO] || !_axisControlFormVariate._ioStatus[1, BACK_CAMERA_LIGHT_TRIGGER_OUT_IO2])
                            {
                                _axisControlFormVariate.OutputControl(BACK_CAMERA_LIGHT_TRIGGER_OUT_IO, true);
                                _axisControlFormVariate.OutputControl(BACK_CAMERA_LIGHT_TRIGGER_OUT_IO2, true);
                                Thread.Sleep(100);//等待光源打开
                            }

                            if (_backCameraCs.ho_Image != null)
                            {
                                _backCameraCs.ho_Image.Dispose();
                                _backCameraCs.ho_Image = null;
                            }
                            if (!_backCameraCs.GrabImage2(true))
                            {
                                _backCameraGrab = false;
                                _backCameraVideo = false;
                                tempbool = false;
                            }
                            Thread.Sleep(10);

                            if (tempbool)
                            {
                                if (_baseSettingFormVariate._maintenanceCameraFormVariate.Visible)
                                {
                                    HalconCameraControl.DispImageAdaptively(_baseSettingFormVariate._maintenanceCameraFormVariate.picBackCameraImageDisp, _backCameraCs.ho_Image, _backCamraDispCrossLineFlag);
                                }
                                else if (_baseSettingFormVariate._settingsCameraCalibrationVariate.Visible)
                                {
                                    _baseSettingFormVariate._settingsCameraCalibrationVariate.picBackCameraImageDisp.Invoke(new Action(() =>
                                        {
                                            Bitmap tempBmp;
                                            HalconMVTec.ImageDispose.ConvertHalconGrayByteImageToBitmap(_backCameraCs.ho_Image, out tempBmp);
                                            tempBmp = HalconMVTec.ImageDispose.ResizeBitmapImage(tempBmp, _baseSettingFormVariate._settingsCameraCalibrationVariate.picBackCameraImageDisp.Width, _baseSettingFormVariate._settingsCameraCalibrationVariate.picBackCameraImageDisp.Height);
                                            if (_backCamraDispCrossLineFlag && _backCameraVideo)
                                            {
                                                int width = tempBmp.Width;
                                                int height = tempBmp.Height;
                                                Graphics gf = Graphics.FromImage(tempBmp);
                                                gf.DrawLine(new Pen(new SolidBrush(Color.Green)), new Point(0, height / 2), new Point(width, height / 2));
                                                gf.DrawLine(new Pen(new SolidBrush(Color.Green)), new Point(width / 2, 0), new Point(width / 2, height));
                                            }
                                            _baseSettingFormVariate._settingsCameraCalibrationVariate.picBackCameraImageDisp.Image = tempBmp;
                                        })); ;
                                }
                                else if (_baseSettingFormVariate._settingsProductFormVariate.Visible)//如果是产品设置界面
                                {
                                    HalconCameraControl.DispImageAdaptively(_baseSettingFormVariate._settingsProductFormVariate.picBackCameraImageDisp, _backCameraCs.ho_Image, false);
                                    _baseSettingFormVariate._settingsProductFormVariate.ho_backCameraImage = _backCameraCs.ho_Image;
                                    _backCameraVideo = false;
                                }
                                if (_backCameraGrab)
                                {
                                    _backCameraGrab = false;
                                    _axisControlFormVariate.OutputControl(BACK_CAMERA_LIGHT_TRIGGER_OUT_IO, false);
                                    _axisControlFormVariate.OutputControl(BACK_CAMERA_LIGHT_TRIGGER_OUT_IO2, false);
                                }
                            }
                        }
                        else
                        {
                            MyTool.TxtFileProcess.CreateLog("相机手动控制页面，控制后侧相机动作失败！未完成运动控制卡初始化！" + "----用户：" + _operatorName,
                            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                            MessageBox.Show("未完成运动控制卡初始化，无法触发后侧相机！");
                            _backCameraVideo = false;
                            _backCameraGrab = false;
                            return;
                        }
                    }
                }

                if (!_backCameraVideo && !_backCameraGrab && !_autoModeFlag)
                {
                    if (_axisControlFormVariate._ioStatus[1, BACK_CAMERA_LIGHT_TRIGGER_OUT_IO] || _axisControlFormVariate._ioStatus[1, BACK_CAMERA_LIGHT_TRIGGER_OUT_IO2])
                    {
                        _axisControlFormVariate.OutputControl(BACK_CAMERA_LIGHT_TRIGGER_OUT_IO, false);
                        _axisControlFormVariate.OutputControl(BACK_CAMERA_LIGHT_TRIGGER_OUT_IO2, false);
                    }
                }

                Thread.Sleep(10);
            }
        }

        #endregion

        #region Others

        /// <summary>
        /// SortResultDTFromSmallToBig:排序结果文件从小到大
        /// </summary>
        /// <param name="sourceDT"></param>
        /// <returns></returns>
        private DataTable SortResultDTFromSmallToBig(DataTable sourceDT)
        {
            DataTable resultDT = sourceDT.Copy();
            DataTable tempResultDT = sourceDT.Copy();
            int comparePcsNo = 0;
            int compareProductNo = 0;
            int comparedPcsNo = 0;
            int comparedProductNo = 0;
            for (int i = 1; i < resultDT.Rows.Count - 1; i++)
            {
                for (int j = 1; j < resultDT.Rows.Count - i; j++)
                {
                    comparePcsNo = Convert.ToInt32(resultDT.Rows[j][1].ToString().Substring(resultDT.Rows[j][1].ToString().IndexOf("_") + 1, resultDT.Rows[j][1].ToString().IndexOf("-") - resultDT.Rows[j][1].ToString().IndexOf("_") - 1));
                    comparedPcsNo = Convert.ToInt32(resultDT.Rows[j + 1][1].ToString().Substring(resultDT.Rows[j + 1][1].ToString().IndexOf("_") + 1, resultDT.Rows[j + 1][1].ToString().IndexOf("-") - resultDT.Rows[j + 1][1].ToString().IndexOf("_") - 1));

                    //if (!SettingsAdministratorForm._checkProductIsConnector)//如果为Switch
                    //{
                    compareProductNo = Convert.ToInt32(resultDT.Rows[j][1].ToString().Substring(resultDT.Rows[j][1].ToString().Length - 3, 3));
                    comparedProductNo = Convert.ToInt32(resultDT.Rows[j + 1][1].ToString().Substring(resultDT.Rows[j + 1][1].ToString().Length - 3, 3));
                    //}
                    //else//如果为Connector
                    //{
                    //    compareProductNo = Convert.ToInt32(resultDT.Rows[j][1].ToString().Substring(resultDT.Rows[j][1].ToString().IndexOf("J") + 1));
                    //    comparedProductNo = Convert.ToInt32(resultDT.Rows[j + 1][1].ToString().Substring(resultDT.Rows[j + 1][1].ToString().IndexOf("J") + 1));
                    //}

                    if (comparePcsNo > comparedPcsNo)
                    {
                        resultDT.Rows[j].ItemArray = tempResultDT.Rows[j + 1].ItemArray;
                        resultDT.Rows[j + 1].ItemArray = tempResultDT.Rows[j].ItemArray;
                        tempResultDT = resultDT.Copy();
                    }
                    else if (comparePcsNo == comparedPcsNo)
                    {
                        if (compareProductNo > comparedProductNo)
                        {
                            resultDT.Rows[j].ItemArray = tempResultDT.Rows[j + 1].ItemArray;
                            resultDT.Rows[j + 1].ItemArray = tempResultDT.Rows[j].ItemArray;
                            tempResultDT = resultDT.Copy();
                        }
                    }
                }
            }

            for (int i = 1; i < resultDT.Rows.Count; i++)
            {
                resultDT.Rows[i][0] = i.ToString();
            }

            return resultDT;
        }

        /// <summary>
        /// GetDataFromMES : 从MES中获取数据
        /// </summary>
        /// <param name="productBarcode">string:二维码数据</param>
        /// <param name="conveyorIndex">int:传送线索引，0-传送线1，1-传送线2</param>
        /// <param name="result">string:通讯反馈数据</param>
        /// <returns>bool:通讯结果，ture-成功，false-失败</returns>
        private bool GetDataFromMES(string productBarcode, int conveyorIndex, out string result)
        {
            MyTool.TxtFileProcess.CreateLog("向MES发送入站请求!" + "----用户：" + _operatorName,
            BaseForm._mesCommunicationLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

            MESOperation.PanelsCheckIn newPanelsCheckIn = new MESOperation.PanelsCheckIn();


            newPanelsCheckIn.Panel = productBarcode;
            newPanelsCheckIn.Resource = SettingsAdministratorForm._testLineResource;
            newPanelsCheckIn.Machine = SettingsAdministratorForm._machineName;
            newPanelsCheckIn.Product = _productParameter[conveyorIndex]._productNameUseForMES;
            newPanelsCheckIn.OperatorName = _operatorName;
            newPanelsCheckIn.WorkArea = SettingsAdministratorForm._workArea;
            newPanelsCheckIn.TestType = SettingsAdministratorForm._testType;
            newPanelsCheckIn.Site = SettingsAdministratorForm._site;
            newPanelsCheckIn.Mac = SettingsAdministratorForm._devicePhysicAddress;
            newPanelsCheckIn.ProgramName = SettingsAdministratorForm._programName;
            newPanelsCheckIn.IpAddress = "10.2.38.12";
            newPanelsCheckIn.OptTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff");

            string panelsCheckIn = _newMesOperation.serializationPanelsCheckIn(newPanelsCheckIn);
            MyTool.TxtFileProcess.CreateLog(panelsCheckIn,
            BaseForm._mesCommunicationLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

            if (_newMesOperation.HttpPost(_newMesOperation.serverlURL + _newMesOperation.paneleCheckEntryPath, panelsCheckIn, out result))
            {
                MyTool.TxtFileProcess.CreateLog("反馈数据：" + "----用户：" + _operatorName,
                BaseForm._mesCommunicationLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                MyTool.TxtFileProcess.CreateLog(result,
                BaseForm._mesCommunicationLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                if (result.Length > 2)
                {
                    if (result.Substring(2, 1) == "0")
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
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// SendDataToMES:传输数据至MES
        /// </summary>
        /// <param name="productBarcode">string:二维码数据</param>
        /// <param name="conveyorIndex">int:传送线索引，0-传送线1，1-传送线2</param>
        /// <param name="resultDT">DataTable:存储结果数据的DATATABLE</param>
        /// <param name="result">string:通讯反馈数据</param>
        /// <returns>bool：成功为true，失败为false</returns>
        private bool SendDataToMES(string productBarcode, int conveyorIndex, DataTable resultDT, out string result)
        {
            MyTool.TxtFileProcess.CreateLog("向MES发送测试完数据!" + "----用户：" + _operatorName,
            BaseForm._mesCommunicationLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

            MESOperation.PanelsCheckOut newPanelsCheckOut = new MESOperation.PanelsCheckOut();

            newPanelsCheckOut.Panel = productBarcode;
            newPanelsCheckOut.Resource = SettingsAdministratorForm._testLineResource;
            newPanelsCheckOut.Machine = SettingsAdministratorForm._machineName;
            newPanelsCheckOut.Product = _productParameter[conveyorIndex]._productNameUseForMES;
            newPanelsCheckOut.WorkArea = SettingsAdministratorForm._workArea;
            newPanelsCheckOut.TestType = SettingsAdministratorForm._testType;
            newPanelsCheckOut.OperatorName = _operatorName;
            newPanelsCheckOut.OperatorType = SettingsAdministratorForm._operatorType;
            newPanelsCheckOut.testMode = SettingsAdministratorForm._testMode;
            newPanelsCheckOut.Site = SettingsAdministratorForm._site;
            newPanelsCheckOut.Mac = SettingsAdministratorForm._devicePhysicAddress;
            newPanelsCheckOut.ProgramName = SettingsAdministratorForm._programName;
            if (conveyorIndex == 0)
                newPanelsCheckOut.trackType = "R";
            else
                newPanelsCheckOut.trackType = "L";
            newPanelsCheckOut.hasTrackFlag = SettingsAdministratorForm._haveTrackFlag;
            newPanelsCheckOut.IpAddress = "";
            newPanelsCheckOut.CarrierId = "";
            newPanelsCheckOut.CoverPlate = "";

            string mesSendMainInfoStr = _newMesOperation.serializationPanelsCheckOut(newPanelsCheckOut);
            mesSendMainInfoStr = mesSendMainInfoStr.Substring(0, mesSendMainInfoStr.Length - 1);
            string mesSenderStr = mesSendMainInfoStr;

            //需要传输的detailList数据转成string
            MESOperation.detaillist detailList = new MESOperation.detaillist();
            mesSenderStr = mesSenderStr + ",\"detaillist\":[";
            for (int i = 1; i < resultDT.Rows.Count; i++)//第一行为标题栏
            {
                detailList.PanelId = productBarcode;
                detailList.testType = SettingsAdministratorForm._testType;
                detailList.pcsSeq = Convert.ToUInt32(resultDT.Rows[i][1].ToString().Substring(resultDT.Rows[i][1].ToString().IndexOf("_") + 1,
                    resultDT.Rows[i][1].ToString().IndexOf("-") - resultDT.Rows[i][1].ToString().IndexOf("_") - 1)).ToString();
                detailList.partSeq = resultDT.Rows[i][1].ToString().Substring(resultDT.Rows[i][1].ToString().IndexOf("-") + 1);
                detailList.PinSeq = "L/R";
                //根据左右判定OK还是NG
                if (resultDT.Rows[i][2].ToString().Length < 2)
                {
                    detailList.testResult = "FAIL/";
                }
                else
                {
                    if (resultDT.Rows[i][2].ToString().Substring(0, 2) == "OK")
                        detailList.testResult = "PASS/";
                    else if (resultDT.Rows[i][2].ToString().Substring(0, 2) == "NG")
                        detailList.testResult = "FAIL/";
                    else
                        detailList.testResult = "SKIP/";
                }

                if (resultDT.Rows[i][3].ToString().Length < 2)
                {
                    detailList.testResult += "FAIL";
                }
                else
                {
                    if (resultDT.Rows[i][3].ToString().Substring(0, 2) == "OK")
                        detailList.testResult += "PASS";
                    else if (resultDT.Rows[i][3].ToString().Substring(0, 2) == "NG")
                        detailList.testResult += "FAIL";
                    else
                        detailList.testResult += "SKIP";
                }

                detailList.operatorName = _operatorName;
                detailList.defectCode = "";
                detailList.testFile = "";
                detailList.imagePath = "";

                string tempDetailListStr = JsonConvert.SerializeObject(detailList);

                if (i != resultDT.Rows.Count - 1)
                    mesSenderStr = mesSenderStr + tempDetailListStr + ",";
                else
                    mesSenderStr = mesSenderStr + tempDetailListStr + "],";
            }

            //需要传输的SummaryList数据转成string
            MESOperation.summarylist summaryList = new MESOperation.summarylist();
            mesSenderStr += "\"summarylist\":[";


            int kindStartIndex = 1;
            string nowPcsNo = "";
            string nextPcsNo = "";
            int tempResult = -1;//1-OK,2-NG,3-NULL
            for (int i = 1; i < resultDT.Rows.Count; i++)//第一行为标题栏
            {
                nowPcsNo = Convert.ToInt32(resultDT.Rows[i][1].ToString().Substring(resultDT.Rows[i][1].ToString().IndexOf("_") + 1,
                    resultDT.Rows[i][1].ToString().IndexOf("-") - resultDT.Rows[i][1].ToString().IndexOf("_") - 1)).ToString(); ;
                if (i + 1 < resultDT.Rows.Count)
                {
                    nextPcsNo = Convert.ToInt32(resultDT.Rows[i + 1][1].ToString().Substring(resultDT.Rows[i + 1][1].ToString().IndexOf("_") + 1,
                        resultDT.Rows[i + 1][1].ToString().IndexOf("-") - resultDT.Rows[i + 1][1].ToString().IndexOf("_") - 1)).ToString();
                    if (nowPcsNo != nextPcsNo)//如果接下来的那个产品名称与当前的不相等
                    {
                        for (int j = kindStartIndex; j <= i; j++)
                        {
                            if (resultDT.Rows[j][4].ToString() == "NG")
                            {
                                tempResult = 2;
                                break;
                            }
                            else if (resultDT.Rows[j][4].ToString() == "OK" && tempResult == -1)//如果结果为OK，并且记录的状态不为NG
                            {
                                tempResult = 1;
                            }
                            else if (resultDT.Rows[j][4].ToString() != "NG" && resultDT.Rows[j][4].ToString() != "OK")//如果不为OK，也不为NG，并且记录结果不为NG
                            {
                                tempResult = 3;
                            }
                        }

                        summaryList.PanelId = productBarcode;
                        summaryList.pcsSeq = nowPcsNo.ToString();
                        if (tempResult == 1)//如果为OK
                            summaryList.testResult = "PASS";
                        else if (tempResult == 2)//如果为NG
                            summaryList.testResult = "FAIL";
                        else if (tempResult == 3)//如果为NULL
                            summaryList.testResult = "SKIP";
                        summaryList.operatorName = _operatorName;
                        summaryList.operatorTime = "";

                        string tempSummaryListStr = JsonConvert.SerializeObject(summaryList);
                        mesSenderStr = mesSenderStr + tempSummaryListStr + ",";
                        tempResult = -1;
                        kindStartIndex = i + 1;
                    }
                }
                else
                {
                    for (int j = kindStartIndex; j <= i; j++)
                    {
                        if (resultDT.Rows[j][4].ToString() == "NG")
                        {
                            tempResult = 2;
                            break;
                        }
                        else if (resultDT.Rows[j][4].ToString() == "OK" && tempResult == -1)//如果结果为OK，并且记录的状态不为NG
                        {
                            tempResult = 1;
                        }
                        else if (resultDT.Rows[j][4].ToString() != "NG" && resultDT.Rows[j][4].ToString() != "OK")//如果不为OK，也不为NG，并且记录结果不为NG
                        {
                            tempResult = 3;
                        }
                    }

                    summaryList.PanelId = productBarcode;
                    summaryList.pcsSeq = nowPcsNo.ToString();
                    if (tempResult == 1)//如果为OK
                        summaryList.testResult = "PASS";
                    else if (tempResult == 2)//如果为NG
                        summaryList.testResult = "FAIL";
                    else if (tempResult == 3)//如果为NULL
                        summaryList.testResult = "SKIP";
                    summaryList.operatorName = _operatorName;
                    summaryList.operatorTime = "";

                    string tempSummaryListStr = JsonConvert.SerializeObject(summaryList);
                    mesSenderStr = mesSenderStr + tempSummaryListStr + "]";
                    tempResult = -1;
                }
            }

            //传输数据
            mesSenderStr += "}";

            MyTool.TxtFileProcess.CreateLog(mesSenderStr,
            BaseForm._mesCommunicationLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

            if (_newMesOperation.HttpPost(_newMesOperation.serverlURL + _newMesOperation.paneleCheckExitPath, mesSenderStr, out result))
            {
                MyTool.TxtFileProcess.CreateLog("反馈数据：" + "----用户：" + _operatorName,
                BaseForm._mesCommunicationLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
                MyTool.TxtFileProcess.CreateLog(result,
                BaseForm._mesCommunicationLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);

                if (result.Length > 2)
                {
                    if (result.Substring(2, 1) == "0")
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
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// CombineLeftAndRightResultDTAndRecordYieldAndSaveResultCSVFile:联合左右侧结果并记录产量，生产结果CSV文件函数
        /// </summary>
        /// <param name="sorttedResultDT"></param>
        private void CombineLeftAndRightResultDTAndRecordYieldAndSaveResultCSVFile(out DataTable sorttedResultDT)
        {

            DataTable finalResult = new DataTable();
            DataTable ngResultDt = null;
            DataTable pcsResultDt = new DataTable();
            bool haveNGPointFlag = false;//panel上有NG位存在标志

            //判定当前产品是否是样品板
            bool currentPanelIsSampleFlag = false;
            int conveyorIndex = 0;
            if (_conveyor2Status == 105)
                conveyorIndex = 1;
            if ((_barcodeData[conveyorIndex] == _productParameter[conveyorIndex]._sampleProduct1Barcode && _productParameter[conveyorIndex]._sampleProduct1Barcode != "") ||
                      (_barcodeData[conveyorIndex] == _productParameter[conveyorIndex]._sampleProduct2Barcode && _productParameter[conveyorIndex]._sampleProduct2Barcode != "") ||
                    (_barcodeData[conveyorIndex] == _productParameter[conveyorIndex]._sampleProduct3Barcode && _productParameter[conveyorIndex]._sampleProduct3Barcode != ""))
                currentPanelIsSampleFlag = true;

            finalResult.Columns.Add("No", typeof(string));
            finalResult.Columns.Add("Name", typeof(string));
            finalResult.Columns.Add("Left result", typeof(string));
            finalResult.Columns.Add("Right result", typeof(string));
            finalResult.Columns.Add("Product result", typeof(string));
            finalResult.Rows.Add();
            finalResult.Rows[0][0] = "No";
            finalResult.Rows[0][1] = "Name";
            finalResult.Rows[0][2] = "Left result";
            finalResult.Rows[0][3] = "Right result";
            finalResult.Rows[0][4] = "Product result";
            ngResultDt = finalResult.Copy();
            for (int i = 0; i < _leftInferResultDt[conveyorIndex].Rows.Count; i++)
            {
                finalResult.Rows.Add();
                finalResult.Rows[i + 1][0] = _leftInferResultDt[conveyorIndex].Rows[i][0].ToString();
                finalResult.Rows[i + 1][1] = _leftInferResultDt[conveyorIndex].Rows[i][1].ToString();
                finalResult.Rows[i + 1][2] = _leftInferResultDt[conveyorIndex].Rows[i][2].ToString();
                finalResult.Rows[i + 1][3] = _rightInferResultDt[conveyorIndex].Rows[i][3].ToString();
                if (_leftInferResultDt[conveyorIndex].Rows[i][2].ToString().Length >= 2 && _rightInferResultDt[conveyorIndex].Rows[i][3].ToString().Length >= 2)
                {
                    if (_leftInferResultDt[conveyorIndex].Rows[i][2].ToString().Substring(0, 2) == "NG" || _rightInferResultDt[conveyorIndex].Rows[i][3].ToString().Substring(0, 2) == "NG")
                    {
                        finalResult.Rows[i + 1][4] = "NG";
                        ngResultDt.Rows.Add();
                        ngResultDt.Rows[ngResultDt.Rows.Count - 1][0] = finalResult.Rows[i + 1][0];
                        ngResultDt.Rows[ngResultDt.Rows.Count - 1][1] = finalResult.Rows[i + 1][1];
                        ngResultDt.Rows[ngResultDt.Rows.Count - 1][2] = finalResult.Rows[i + 1][2];
                        ngResultDt.Rows[ngResultDt.Rows.Count - 1][3] = finalResult.Rows[i + 1][3];
                        ngResultDt.Rows[ngResultDt.Rows.Count - 1][4] = finalResult.Rows[i + 1][4];
                        if (!haveNGPointFlag)
                            haveNGPointFlag = true;
                    }
                    else if (_leftInferResultDt[conveyorIndex].Rows[i][2].ToString().Substring(0, 2) == "OK" && _rightInferResultDt[conveyorIndex].Rows[i][3].ToString().Substring(0, 2) == "OK")
                    {
                        finalResult.Rows[i + 1][4] = "OK";
                    }
                }
                else if (_leftInferResultDt[conveyorIndex].Rows[i][2].ToString().Length < 2 && _rightInferResultDt[conveyorIndex].Rows[i][3].ToString().Length >= 2)
                {
                    finalResult.Rows[i + 1][4] = "NG";
                    ngResultDt.Rows.Add();
                    ngResultDt.Rows[ngResultDt.Rows.Count - 1][0] = finalResult.Rows[i + 1][0];
                    ngResultDt.Rows[ngResultDt.Rows.Count - 1][1] = finalResult.Rows[i + 1][1];
                    ngResultDt.Rows[ngResultDt.Rows.Count - 1][2] = finalResult.Rows[i + 1][2];
                    ngResultDt.Rows[ngResultDt.Rows.Count - 1][3] = finalResult.Rows[i + 1][3];
                    ngResultDt.Rows[ngResultDt.Rows.Count - 1][4] = finalResult.Rows[i + 1][4];
                    if (!haveNGPointFlag)
                        haveNGPointFlag = true;

                }
                else if (_leftInferResultDt[conveyorIndex].Rows[i][2].ToString().Length >= 2 && _rightInferResultDt[conveyorIndex].Rows[i][3].ToString().Length < 2)
                {
                    finalResult.Rows[i + 1][4] = "NG";
                    ngResultDt.Rows.Add();
                    ngResultDt.Rows[ngResultDt.Rows.Count - 1][0] = finalResult.Rows[i + 1][0];
                    ngResultDt.Rows[ngResultDt.Rows.Count - 1][1] = finalResult.Rows[i + 1][1];
                    ngResultDt.Rows[ngResultDt.Rows.Count - 1][2] = finalResult.Rows[i + 1][2];
                    ngResultDt.Rows[ngResultDt.Rows.Count - 1][3] = finalResult.Rows[i + 1][3];
                    ngResultDt.Rows[ngResultDt.Rows.Count - 1][4] = finalResult.Rows[i + 1][4];
                    if (!haveNGPointFlag)
                        haveNGPointFlag = true;
                }
                else
                {
                    finalResult.Rows[i + 1][4] = "NG";
                }

                //判断当前位置是否应该是跳过
                if (_needInferOrNotDt != null)
                {
                    int currentPositionPcsNo = Convert.ToInt32(_leftInferResultDt[conveyorIndex].Rows[i][1].ToString().Substring(_leftInferResultDt[conveyorIndex].Rows[i][1].ToString().IndexOf("_") + 1,
                        _leftInferResultDt[conveyorIndex].Rows[i][1].ToString().IndexOf("-") - _leftInferResultDt[conveyorIndex].Rows[i][1].ToString().IndexOf("_") - 1));
                    if (_needInferOrNotDt.Rows[currentPositionPcsNo - 1][1].ToString() == "NG")//当前点位被屏蔽
                    {
                        finalResult.Rows[i + 1][2] = "SKIP";
                        finalResult.Rows[i + 1][3] = "SKIP";
                        finalResult.Rows[i + 1][4] = "SKIP";
                    }
                }
                else
                {
                    if (_productParameter[conveyorIndex]._pointPositionDT.Rows[i][4].ToString() == "1")//当前点位被屏蔽
                    {
                        finalResult.Rows[i + 1][2] = "SKIP";
                        finalResult.Rows[i + 1][3] = "SKIP";
                        finalResult.Rows[i + 1][4] = "SKIP";
                    }
                }

            }

            sorttedResultDT = SortResultDTFromSmallToBig(finalResult);
            if (SettingsGeneralForm._saveResultListFileFlag)//如果要保存结果表格
            {
                if (Directory.Exists(_baseResultCsvFileFolderPath))
                    Directory.CreateDirectory(_baseResultCsvFileFolderPath);

                CSVFile.CSVFileOperation.SaveCSV3(finalResult, _baseResultCsvFileFolderPath + "CheckResult.csv");
                CSVFile.CSVFileOperation.SaveCSV3(ngResultDt, _baseResultCsvFileFolderPath + "CheckNGResult.csv");
                //CSVFile.CSVFileOperation.SaveCSV3(sorttedResultDT, _baseResultCsvFileFolderPath + "SorttedResult.csv");
            }

            //记录PCS数
            pcsResultDt.Columns.Add("No", typeof(string));
            pcsResultDt.Columns.Add("PCS Name", typeof(string));
            pcsResultDt.Columns.Add("PCS Result", typeof(string));
            pcsResultDt.Rows.Add();
            pcsResultDt.Rows[0][0] = "No";
            pcsResultDt.Rows[0][1] = "PCS Name";
            pcsResultDt.Rows[0][2] = "PCS Result";
            lock (_yieldRecordDt)
            {
                int kindStartIndex = 1;//第一行为名称标题栏，所以这里用1作为初始值
                string nowPcsNo = "";
                string nextPcsNo = "";
                int tempResult = -1;//1-OK,2-NG,3-NULL
                for (int i = 1; i < sorttedResultDT.Rows.Count; i++)//第一行为标题栏
                {
                    nowPcsNo = Convert.ToInt32(sorttedResultDT.Rows[i][1].ToString().Substring(sorttedResultDT.Rows[i][1].ToString().IndexOf("_") + 1,
                        sorttedResultDT.Rows[i][1].ToString().IndexOf("-") - sorttedResultDT.Rows[i][1].ToString().IndexOf("_") - 1)).ToString(); ;
                    if (i + 1 < sorttedResultDT.Rows.Count)
                    {
                        nextPcsNo = Convert.ToInt32(sorttedResultDT.Rows[i + 1][1].ToString().Substring(sorttedResultDT.Rows[i + 1][1].ToString().IndexOf("_") + 1,
                            sorttedResultDT.Rows[i + 1][1].ToString().IndexOf("-") - sorttedResultDT.Rows[i + 1][1].ToString().IndexOf("_") - 1)).ToString();
                        if (nowPcsNo != nextPcsNo)//如果接下来的那个产品名称与当前的不相等
                        {
                            for (int j = kindStartIndex; j <= i; j++)
                            {
                                if (sorttedResultDT.Rows[j][4].ToString() == "NG")
                                {
                                    tempResult = 2;
                                    break;
                                }
                                else if (sorttedResultDT.Rows[j][4].ToString() == "OK" && tempResult == -1)//如果结果为OK，并且记录的状态不为NG
                                {
                                    tempResult = 1;
                                }
                                else if (sorttedResultDT.Rows[j][4].ToString() != "NG" && sorttedResultDT.Rows[j][4].ToString() != "OK")//如果不为OK，也不为NG，并且记录结果不为NG
                                {
                                    tempResult = 3;
                                }
                            }

                            pcsResultDt.Rows.Add();
                            pcsResultDt.Rows[pcsResultDt.Rows.Count - 1][0] = (pcsResultDt.Rows.Count - 1).ToString();
                            pcsResultDt.Rows[pcsResultDt.Rows.Count - 1][1] = nowPcsNo.ToString();
                            switch (tempResult)
                            {
                                case 1:
                                    pcsResultDt.Rows[pcsResultDt.Rows.Count - 1][2] = "PASS";
                                    break;
                                case 2:
                                    pcsResultDt.Rows[pcsResultDt.Rows.Count - 1][2] = "FAIL";
                                    break;
                                case 3:
                                    pcsResultDt.Rows[pcsResultDt.Rows.Count - 1][2] = "SKIP";
                                    break;
                            }

                            if (SettingsGeneralForm._addSampleDataToYieldFlag && currentPanelIsSampleFlag || !currentPanelIsSampleFlag)
                            {
                                if (tempResult != 3)
                                {
                                    _yieldRecordDt.Rows[0][1] = (Convert.ToInt32(_yieldRecordDt.Rows[0][1]) + 1).ToString();
                                    if (_conveyor1Status == 105)
                                    {
                                        _yieldRecordDt.Rows[0][2] = (Convert.ToInt32(_yieldRecordDt.Rows[0][2]) + 1).ToString();
                                        if (tempResult == 2)
                                        {
                                            _yieldRecordDt.Rows[2][2] = (Convert.ToInt32(_yieldRecordDt.Rows[2][2]) + 1).ToString();
                                            _yieldRecordDt.Rows[2][1] = (Convert.ToInt32(_yieldRecordDt.Rows[2][1]) + 1).ToString();
                                        }
                                        else
                                        {
                                            _yieldRecordDt.Rows[1][2] = (Convert.ToInt32(_yieldRecordDt.Rows[1][2]) + 1).ToString();
                                            _yieldRecordDt.Rows[1][1] = (Convert.ToInt32(_yieldRecordDt.Rows[1][1]) + 1).ToString();
                                        }
                                    }
                                    else
                                    {
                                        _yieldRecordDt.Rows[0][3] = (Convert.ToInt32(_yieldRecordDt.Rows[0][3]) + 1).ToString();
                                        if (tempResult == 2)
                                        {
                                            _yieldRecordDt.Rows[2][3] = (Convert.ToInt32(_yieldRecordDt.Rows[2][3]) + 1).ToString();
                                            _yieldRecordDt.Rows[2][1] = (Convert.ToInt32(_yieldRecordDt.Rows[2][1]) + 1).ToString();
                                        }
                                        else
                                        {
                                            _yieldRecordDt.Rows[1][3] = (Convert.ToInt32(_yieldRecordDt.Rows[1][3]) + 1).ToString();
                                            _yieldRecordDt.Rows[1][1] = (Convert.ToInt32(_yieldRecordDt.Rows[1][1]) + 1).ToString();
                                        }

                                    }
                                }
                            }
                            kindStartIndex = i + 1;
                            tempResult = -1;
                        }

                    }
                    else
                    {
                        for (int j = kindStartIndex; j <= i; j++)
                        {
                            if (sorttedResultDT.Rows[j][4].ToString() == "NG")
                            {
                                tempResult = 2;
                                break;
                            }
                            else if (sorttedResultDT.Rows[j][4].ToString() == "OK" && tempResult == -1)//如果结果为OK，并且记录的状态不为NG
                            {
                                tempResult = 1;
                            }
                            else if (sorttedResultDT.Rows[j][4].ToString() != "NG" && sorttedResultDT.Rows[j][4].ToString() != "OK")//如果不为OK，也不为NG，并且记录结果不为NG
                            {
                                tempResult = 3;
                            }
                        }

                        pcsResultDt.Rows.Add();
                        pcsResultDt.Rows[pcsResultDt.Rows.Count - 1][0] = (pcsResultDt.Rows.Count - 1).ToString();
                        pcsResultDt.Rows[pcsResultDt.Rows.Count - 1][1] = nowPcsNo.ToString();
                        switch (tempResult)
                        {
                            case 1:
                                pcsResultDt.Rows[pcsResultDt.Rows.Count - 1][2] = "PASS";
                                break;
                            case 2:
                                pcsResultDt.Rows[pcsResultDt.Rows.Count - 1][2] = "FAIL";
                                break;
                            case 3:
                                pcsResultDt.Rows[pcsResultDt.Rows.Count - 1][2] = "SKIP";
                                break;
                        }
                        if (SettingsGeneralForm._addSampleDataToYieldFlag && currentPanelIsSampleFlag || !currentPanelIsSampleFlag)
                        {
                            if (tempResult != 3)
                            {
                                _yieldRecordDt.Rows[0][1] = (Convert.ToInt32(_yieldRecordDt.Rows[0][1]) + 1).ToString();
                                if (_conveyor1Status == 105)
                                {
                                    _yieldRecordDt.Rows[0][2] = (Convert.ToInt32(_yieldRecordDt.Rows[0][2]) + 1).ToString();
                                    if (tempResult == 2)
                                    {
                                        _yieldRecordDt.Rows[2][2] = (Convert.ToInt32(_yieldRecordDt.Rows[2][2]) + 1).ToString();
                                        _yieldRecordDt.Rows[2][1] = (Convert.ToInt32(_yieldRecordDt.Rows[2][1]) + 1).ToString();
                                    }
                                    else
                                    {
                                        _yieldRecordDt.Rows[1][2] = (Convert.ToInt32(_yieldRecordDt.Rows[1][2]) + 1).ToString();
                                        _yieldRecordDt.Rows[1][1] = (Convert.ToInt32(_yieldRecordDt.Rows[1][1]) + 1).ToString();
                                    }
                                }
                                else
                                {
                                    _yieldRecordDt.Rows[0][3] = (Convert.ToInt32(_yieldRecordDt.Rows[0][3]) + 1).ToString();
                                    if (tempResult == 2)
                                    {
                                        _yieldRecordDt.Rows[2][3] = (Convert.ToInt32(_yieldRecordDt.Rows[2][3]) + 1).ToString();
                                        _yieldRecordDt.Rows[2][1] = (Convert.ToInt32(_yieldRecordDt.Rows[2][1]) + 1).ToString();
                                    }
                                    else
                                    {
                                        _yieldRecordDt.Rows[1][3] = (Convert.ToInt32(_yieldRecordDt.Rows[1][3]) + 1).ToString();
                                        _yieldRecordDt.Rows[1][1] = (Convert.ToInt32(_yieldRecordDt.Rows[1][1]) + 1).ToString();
                                    }

                                }
                            }
                        }

                        tempResult = -1;
                    }


                }
            }

            CSVFile.CSVFileOperation.SaveCSV3(pcsResultDt, _baseResultCsvFileFolderPath + "PCSResult.csv");

            if (SettingsGeneralForm._addSampleDataToYieldFlag && currentPanelIsSampleFlag || !currentPanelIsSampleFlag)
            {
                //记录Panel数
                lock (_yieldRecordDt)
                {
                    _yieldRecordDt.Rows[4][1] = (Convert.ToInt32(_yieldRecordDt.Rows[4][1]) + 1).ToString();
                    if (_conveyor1Status == 105)
                    {
                        _yieldRecordDt.Rows[4][2] = (Convert.ToInt32(_yieldRecordDt.Rows[4][2]) + 1).ToString();
                        if (!haveNGPointFlag)
                        {
                            _yieldRecordDt.Rows[5][1] = (Convert.ToInt32(_yieldRecordDt.Rows[5][1]) + 1).ToString();
                            _yieldRecordDt.Rows[5][2] = (Convert.ToInt32(_yieldRecordDt.Rows[5][2]) + 1).ToString();
                        }
                        else
                        {
                            _yieldRecordDt.Rows[6][1] = (Convert.ToInt32(_yieldRecordDt.Rows[6][1]) + 1).ToString();
                            _yieldRecordDt.Rows[6][2] = (Convert.ToInt32(_yieldRecordDt.Rows[6][2]) + 1).ToString();
                        }
                    }
                    else
                    {
                        _yieldRecordDt.Rows[4][3] = (Convert.ToInt32(_yieldRecordDt.Rows[4][3]) + 1).ToString();
                        if (!haveNGPointFlag)
                        {
                            _yieldRecordDt.Rows[5][1] = (Convert.ToInt32(_yieldRecordDt.Rows[5][1]) + 1).ToString();
                            _yieldRecordDt.Rows[5][3] = (Convert.ToInt32(_yieldRecordDt.Rows[5][3]) + 1).ToString();
                        }
                        else
                        {
                            _yieldRecordDt.Rows[6][1] = (Convert.ToInt32(_yieldRecordDt.Rows[6][1]) + 1).ToString();
                            _yieldRecordDt.Rows[6][3] = (Convert.ToInt32(_yieldRecordDt.Rows[6][3]) + 1).ToString();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// AddMessageToLOGFileFinishEventFunc:添加信息至LOG文件完成事件函数
        /// </summary>
        /// <param name="addMsg">string:需要添加的内容</param>
        /// <param name="logFilePath">string:用于辨别属于什么类型的LOG</param>
        private void AddMessageToLOGFileFinishEventFunc(string addMsg, string logFilePath)
        {
            if (logFilePath == BaseForm._deviceControlLogFolderPath)
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
            else if (logFilePath == BaseForm._mesCommunicationLogFolderPath)
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
            else if (logFilePath == Directory.GetCurrentDirectory() + "\\Alarm Record")//如果为报警记录路径
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

        /// <summary>
        /// ActionVariateDisp:动作变量显示函数
        /// </summary>
        private void ActionVariateDisp()
        {
            lvDeviceStatus.Invoke(new Action(() =>
            {
                lvDeviceStatus.BeginUpdate();//与EndUpdate配合使用，使得界面不闪烁

                ListViewItem foundItem = null;

                //模块标志
                foundItem = this.lvDeviceStatus.FindItemWithText("XYRMoveToAssignedPointPositionModuleActionFlag");

                if (foundItem != null)
                {
                    foundItem.SubItems[1].Text = _xyMoveToAssignedPointPositionModuleActionFlag.ToString();
                }
                else
                {
                    ListViewItem lstItem = new ListViewItem();
                    lstItem.Text = "XYRMoveToAssignedPointPositionModuleActionFlag";
                    lstItem.SubItems.Add(_xyMoveToAssignedPointPositionModuleActionFlag.ToString());
                    this.lvDeviceStatus.Items.Insert(0, lstItem);//保证最新的显示在第一条
                }

                //_conveyor1ProductForcedUnloadModuleActionFlag:传送线1强制下料模块状态
                foundItem = this.lvDeviceStatus.FindItemWithText("Conveyor1ProductForcedUnloadModuleActionFlag");

                if (foundItem != null)
                {
                    foundItem.SubItems[1].Text = _conveyor1ProductForcedUnloadModuleActionFlag.ToString();
                }
                else
                {
                    ListViewItem lstItem = new ListViewItem();
                    lstItem.Text = "Conveyor1ProductForcedUnloadModuleActionFlag";
                    lstItem.SubItems.Add(_conveyor1ProductForcedUnloadModuleActionFlag.ToString());
                    this.lvDeviceStatus.Items.Insert(0, lstItem);//保证最新的显示在第一条
                }

                //_conveyor2ProductForcedUnloadModuleActionFlag:传送线2强制下料模块状态
                foundItem = this.lvDeviceStatus.FindItemWithText("Conveyor2ProductForcedUnloadModuleActionFlag");

                if (foundItem != null)
                {
                    foundItem.SubItems[1].Text = _conveyor2ProductForcedUnloadModuleActionFlag.ToString();
                }
                else
                {
                    ListViewItem lstItem = new ListViewItem();
                    lstItem.Text = "Conveyor2ProductForcedUnloadModuleActionFlag";
                    lstItem.SubItems.Add(_conveyor2ProductForcedUnloadModuleActionFlag.ToString());
                    this.lvDeviceStatus.Items.Insert(0, lstItem);//保证最新的显示在第一条
                }

                //_allAxisHomeBackModuleActionFlag:回原点模块状态
                foundItem = this.lvDeviceStatus.FindItemWithText("AllAxisHomeBackModuleActionFlag");

                if (foundItem != null)
                {
                    foundItem.SubItems[1].Text = _allAxisHomeBackModuleActionFlag.ToString();
                }
                else
                {
                    ListViewItem lstItem = new ListViewItem();
                    lstItem.Text = "AllAxisHomeBackModuleActionFlag";
                    lstItem.SubItems.Add(_allAxisHomeBackModuleActionFlag.ToString());
                    this.lvDeviceStatus.Items.Insert(0, lstItem);//保证最新的显示在第一条
                }

                //_axesGotoStandbyModuleActionFlag：轴回等待位置模块状态
                foundItem = this.lvDeviceStatus.FindItemWithText("AxesGotoStandbyModuleActionFlag");

                if (foundItem != null)
                {
                    foundItem.SubItems[1].Text = _axesGotoStandbyModuleActionFlag.ToString();
                }
                else
                {
                    ListViewItem lstItem = new ListViewItem();
                    lstItem.Text = "AxesGotoStandbyModuleActionFlag";
                    lstItem.SubItems.Add(_axesGotoStandbyModuleActionFlag.ToString());
                    this.lvDeviceStatus.Items.Insert(0, lstItem);//保证最新的显示在第一条
                }

                //_conveyorChangeToAssignedWidthModuleActionFlag:传送线宽度调节模块状态
                foundItem = this.lvDeviceStatus.FindItemWithText("ConveyorChangeToAssignedWidthModuleActionFlag");

                if (foundItem != null)
                {
                    foundItem.SubItems[1].Text = _conveyorChangeToAssignedWidthModuleActionFlag.ToString();
                }
                else
                {
                    ListViewItem lstItem = new ListViewItem();
                    lstItem.Text = "ConveyorChangeToAssignedWidthModuleActionFlag";
                    lstItem.SubItems.Add(_conveyorChangeToAssignedWidthModuleActionFlag.ToString());
                    this.lvDeviceStatus.Items.Insert(0, lstItem);//保证最新的显示在第一条
                }

                //_conveyor1ActionModuleActionFlag:传送线1动作模块状态
                foundItem = this.lvDeviceStatus.FindItemWithText("Conveyor1ActionModuleActionFlag");

                if (foundItem != null)
                {
                    foundItem.SubItems[1].Text = _conveyor1ActionModuleActionFlag.ToString();
                }
                else
                {
                    ListViewItem lstItem = new ListViewItem();
                    lstItem.Text = "Conveyor1ActionModuleActionFlag";
                    lstItem.SubItems.Add(_conveyor1ActionModuleActionFlag.ToString());
                    this.lvDeviceStatus.Items.Insert(0, lstItem);//保证最新的显示在第一条
                }

                //_conveyor2ActionModuleActionFlag:传送线2动作模块状态
                foundItem = this.lvDeviceStatus.FindItemWithText("Conveyor2ActionModuleActionFlag");

                if (foundItem != null)
                {
                    foundItem.SubItems[1].Text = _conveyor2ActionModuleActionFlag.ToString();
                }
                else
                {
                    ListViewItem lstItem = new ListViewItem();
                    lstItem.Text = "Conveyor2ActionModuleActionFlag";
                    lstItem.SubItems.Add(_conveyor2ActionModuleActionFlag.ToString());
                    this.lvDeviceStatus.Items.Insert(0, lstItem);//保证最新的显示在第一条
                }

                //_conveyor1LoadUnloadModuleActionFlag:传送线1上下料模块状态
                foundItem = this.lvDeviceStatus.FindItemWithText("Conveyor1LoadUnloadModuleActionFlag");

                if (foundItem != null)
                {
                    foundItem.SubItems[1].Text = _conveyor1LoadUnloadModuleActionFlag.ToString();
                }
                else
                {
                    ListViewItem lstItem = new ListViewItem();
                    lstItem.Text = "Conveyor1LoadUnloadModuleActionFlag";
                    lstItem.SubItems.Add(_conveyor1LoadUnloadModuleActionFlag.ToString());
                    this.lvDeviceStatus.Items.Insert(0, lstItem);//保证最新的显示在第一条
                }

                //_conveyor2LoadUnloadModuleActionFlag:传送线2上下料模块状态
                foundItem = this.lvDeviceStatus.FindItemWithText("Conveyor2LoadUnloadModuleActionFlag");

                if (foundItem != null)
                {
                    foundItem.SubItems[1].Text = _conveyor2LoadUnloadModuleActionFlag.ToString();
                }
                else
                {
                    ListViewItem lstItem = new ListViewItem();
                    lstItem.Text = "Conveyor2LoadUnloadModuleActionFlag";
                    lstItem.SubItems.Add(_conveyor2LoadUnloadModuleActionFlag.ToString());
                    this.lvDeviceStatus.Items.Insert(0, lstItem);//保证最新的显示在第一条
                }

                //_deviceTotalWorkModuleActionFlag:设备整体工作模块状态
                foundItem = this.lvDeviceStatus.FindItemWithText("DeviceTotalWorkModuleActionFlag");

                if (foundItem != null)
                {
                    foundItem.SubItems[1].Text = _deviceTotalWorkModuleActionFlag.ToString();
                }
                else
                {
                    ListViewItem lstItem = new ListViewItem();
                    lstItem.Text = "DeviceTotalWorkModuleActionFlag";
                    lstItem.SubItems.Add(_deviceTotalWorkModuleActionFlag.ToString());
                    this.lvDeviceStatus.Items.Insert(0, lstItem);//保证最新的显示在第一条
                }

                //_barcodeScanModuleActionFlag:扫码模块状态
                foundItem = this.lvDeviceStatus.FindItemWithText("BarcodeScanModuleActionFlag");

                if (foundItem != null)
                {
                    foundItem.SubItems[1].Text = _barcodeScanModuleActionFlag.ToString();
                }
                else
                {
                    ListViewItem lstItem = new ListViewItem();
                    lstItem.Text = "BarcodeScanModuleActionFlag";
                    lstItem.SubItems.Add(_barcodeScanModuleActionFlag.ToString());
                    this.lvDeviceStatus.Items.Insert(0, lstItem);//保证最新的显示在第一条
                }

                //_getOrSendProductInfoFromMESModuleActionFlag:数据与MES交互模块状态
                foundItem = this.lvDeviceStatus.FindItemWithText("GetOrSendProductInfoFromMESModuleActionFlag");

                if (foundItem != null)
                {
                    foundItem.SubItems[1].Text = _getOrSendProductInfoFromMESModuleActionFlag.ToString();
                }
                else
                {
                    ListViewItem lstItem = new ListViewItem();
                    lstItem.Text = "GetOrSendProductInfoFromMESModuleActionFlag";
                    lstItem.SubItems.Add(_getOrSendProductInfoFromMESModuleActionFlag.ToString());
                    this.lvDeviceStatus.Items.Insert(0, lstItem);//保证最新的显示在第一条
                }


                //_deviceRecognizeMarkPositionAndGetCorrectMatrixModuleActionFlag:设备识别MARK点并计算变换矩阵模块状态
                foundItem = this.lvDeviceStatus.FindItemWithText("DeviceRecognizeMarkPositionAndGetCorrectMatrixModuleActionFlag");

                if (foundItem != null)
                {
                    foundItem.SubItems[1].Text = _deviceRecognizeMarkPositionAndGetCorrectMatrixModuleActionFlag.ToString();
                }
                else
                {
                    ListViewItem lstItem = new ListViewItem();
                    lstItem.Text = "DeviceRecognizeMarkPositionAndGetCorrectMatrixModuleActionFlag";
                    lstItem.SubItems.Add(_deviceRecognizeMarkPositionAndGetCorrectMatrixModuleActionFlag.ToString());
                    this.lvDeviceStatus.Items.Insert(0, lstItem);//保证最新的显示在第一条
                }

                //_autoModuleActionFlag = -1;
                foundItem = this.lvDeviceStatus.FindItemWithText("AutoModuleActionFlag");

                if (foundItem != null)
                {
                    foundItem.SubItems[1].Text = _autoModuleActionFlag.ToString();
                }
                else
                {
                    ListViewItem lstItem = new ListViewItem();
                    lstItem.Text = "AutoModuleActionFlag";
                    lstItem.SubItems.Add(_autoModuleActionFlag.ToString());
                    this.lvDeviceStatus.Items.Insert(0, lstItem);//保证最新的显示在第一条
                }

                //_currentDestinationPointIndex:当前指令坐标号
                foundItem = this.lvDeviceStatus.FindItemWithText("CurrentDestinationPointIndex");

                if (foundItem != null)
                {
                    foundItem.SubItems[1].Text = _currentDestinationPointIndex.ToString();
                }
                else
                {
                    ListViewItem lstItem = new ListViewItem();
                    lstItem.Text = "CurrentDestinationPointIndex";
                    lstItem.SubItems.Add(_currentDestinationPointIndex.ToString());
                    this.lvDeviceStatus.Items.Insert(0, lstItem);//保证最新的显示在第一条
                }

                //_leftAndRightCameraGrabImageModuleActionFlag = -1;
                foundItem = this.lvDeviceStatus.FindItemWithText("LeftAndRightCameraGrabImageModuleActionFlag");

                if (foundItem != null)
                {
                    foundItem.SubItems[1].Text = _leftAndRightCameraGrabImageModuleActionFlag.ToString();
                }
                else
                {
                    ListViewItem lstItem = new ListViewItem();
                    lstItem.Text = "LeftAndRightCameraGrabImageModuleActionFlag";
                    lstItem.SubItems.Add(_leftAndRightCameraGrabImageModuleActionFlag.ToString());
                    this.lvDeviceStatus.Items.Insert(0, lstItem);//保证最新的显示在第一条
                }

                //_leftCameraImageInferModuleActionFlag
                foundItem = this.lvDeviceStatus.FindItemWithText("LeftCameraImageInferModuleActionFlag");

                if (foundItem != null)
                {
                    foundItem.SubItems[1].Text = _leftCameraImageInferModuleActionFlag.ToString();
                }
                else
                {
                    ListViewItem lstItem = new ListViewItem();
                    lstItem.Text = "LeftCameraImageInferModuleActionFlag";
                    lstItem.SubItems.Add(_leftCameraImageInferModuleActionFlag.ToString());
                    this.lvDeviceStatus.Items.Insert(0, lstItem);//保证最新的显示在第一条
                }

                //_rightCameraImageInferModuleActionFlag
                foundItem = this.lvDeviceStatus.FindItemWithText("RightCameraImageInferModuleActionFlag");

                if (foundItem != null)
                {
                    foundItem.SubItems[1].Text = _rightCameraImageInferModuleActionFlag.ToString();
                }
                else
                {
                    ListViewItem lstItem = new ListViewItem();
                    lstItem.Text = "RightCameraImageInferModuleActionFlag";
                    lstItem.SubItems.Add(_rightCameraImageInferModuleActionFlag.ToString());
                    this.lvDeviceStatus.Items.Insert(0, lstItem);//保证最新的显示在第一条
                }

                //_leftCameraSourceImageSaveModuleActionFlag
                foundItem = this.lvDeviceStatus.FindItemWithText("LeftCameraSourceImageSaveModuleActionFlag");

                if (foundItem != null)
                {
                    foundItem.SubItems[1].Text = _leftCameraSourceImageSaveModuleActionFlag.ToString();
                }
                else
                {
                    ListViewItem lstItem = new ListViewItem();
                    lstItem.Text = "LeftCameraSourceImageSaveModuleActionFlag";
                    lstItem.SubItems.Add(_leftCameraSourceImageSaveModuleActionFlag.ToString());
                    this.lvDeviceStatus.Items.Insert(0, lstItem);//保证最新的显示在第一条
                }

                //_rightCameraSourceImageSaveModuleActionFlag
                foundItem = this.lvDeviceStatus.FindItemWithText("RightCameraSourceImageSaveModuleActionFlag");

                if (foundItem != null)
                {
                    foundItem.SubItems[1].Text = _rightCameraSourceImageSaveModuleActionFlag.ToString();
                }
                else
                {
                    ListViewItem lstItem = new ListViewItem();
                    lstItem.Text = "RightCameraSourceImageSaveModuleActionFlag";
                    lstItem.SubItems.Add(_rightCameraSourceImageSaveModuleActionFlag.ToString());
                    this.lvDeviceStatus.Items.Insert(0, lstItem);//保证最新的显示在第一条
                }

                //_leftCameraInferredImageSaveModuleActionFlag
                foundItem = this.lvDeviceStatus.FindItemWithText("LeftCameraInferredImageSaveModuleActionFlag");

                if (foundItem != null)
                {
                    foundItem.SubItems[1].Text = _leftCameraInferredImageSaveModuleActionFlag.ToString();
                }
                else
                {
                    ListViewItem lstItem = new ListViewItem();
                    lstItem.Text = "LeftCameraInferredImageSaveModuleActionFlag";
                    lstItem.SubItems.Add(_leftCameraInferredImageSaveModuleActionFlag.ToString());
                    this.lvDeviceStatus.Items.Insert(0, lstItem);//保证最新的显示在第一条
                }

                //_rightCameraInferredImageSaveModuleActionFlag
                foundItem = this.lvDeviceStatus.FindItemWithText("RightCameraInferredImageSaveModuleActionFlag");

                if (foundItem != null)
                {
                    foundItem.SubItems[1].Text = _rightCameraInferredImageSaveModuleActionFlag.ToString();
                }
                else
                {
                    ListViewItem lstItem = new ListViewItem();
                    lstItem.Text = "RightCameraInferredImageSaveModuleActionFlag";
                    lstItem.SubItems.Add(_rightCameraInferredImageSaveModuleActionFlag.ToString());
                    this.lvDeviceStatus.Items.Insert(0, lstItem);//保证最新的显示在第一条
                }


                //_cameraInferDataClearModuleActionFlag
                foundItem = this.lvDeviceStatus.FindItemWithText("CameraInferDataClearModuleActionFlag");

                if (foundItem != null)
                {
                    foundItem.SubItems[1].Text = _cameraInferDataClearModuleActionFlag.ToString();
                }
                else
                {
                    ListViewItem lstItem = new ListViewItem();
                    lstItem.Text = "CameraInferDataClearModuleActionFlag";
                    lstItem.SubItems.Add(_cameraInferDataClearModuleActionFlag.ToString());
                    this.lvDeviceStatus.Items.Insert(0, lstItem);//保证最新的显示在第一条
                }

                //_leftInferNeedReadParameter
                foundItem = this.lvDeviceStatus.FindItemWithText("LeftInferNeedReadParameter");

                if (foundItem != null)
                {
                    foundItem.SubItems[1].Text = _leftInferNeedReadParameter.ToString();
                }
                else
                {
                    ListViewItem lstItem = new ListViewItem();
                    lstItem.Text = "LeftInferNeedReadParameter";
                    lstItem.SubItems.Add(_leftInferNeedReadParameter.ToString());
                    this.lvDeviceStatus.Items.Insert(0, lstItem);//保证最新的显示在第一条
                }

                //_rightInferNeedReadParameter = true;
                foundItem = this.lvDeviceStatus.FindItemWithText("RightInferNeedReadParameter");

                if (foundItem != null)
                {
                    foundItem.SubItems[1].Text = _rightInferNeedReadParameter.ToString();
                }
                else
                {
                    ListViewItem lstItem = new ListViewItem();
                    lstItem.Text = "RightInferNeedReadParameter";
                    lstItem.SubItems.Add(_rightInferNeedReadParameter.ToString());
                    this.lvDeviceStatus.Items.Insert(0, lstItem);//保证最新的显示在第一条
                }

                //_lastTimepreprocessFilePath = "";
                foundItem = this.lvDeviceStatus.FindItemWithText("LastTimepreprocessFilePath");

                if (foundItem != null)
                {
                    foundItem.SubItems[1].Text = _lastTimepreprocessFilePath.ToString();
                }
                else
                {
                    ListViewItem lstItem = new ListViewItem();
                    lstItem.Text = "LastTimepreprocessFilePath";
                    lstItem.SubItems.Add(_lastTimepreprocessFilePath.ToString());
                    this.lvDeviceStatus.Items.Insert(0, lstItem);//保证最新的显示在第一条
                }

                //_lastTimeDimensionFilePath = "";
                foundItem = this.lvDeviceStatus.FindItemWithText("LastTimeDimensionFilePath");

                if (foundItem != null)
                {
                    foundItem.SubItems[1].Text = _lastTimeDimensionFilePath.ToString();
                }
                else
                {
                    ListViewItem lstItem = new ListViewItem();
                    lstItem.Text = "LastTimeDimensionFilePath";
                    lstItem.SubItems.Add(_lastTimeDimensionFilePath.ToString());
                    this.lvDeviceStatus.Items.Insert(0, lstItem);//保证最新的显示在第一条
                }

                //_lastTimeModelFilePath = "";
                foundItem = this.lvDeviceStatus.FindItemWithText("LastTimeModelFilePath");

                if (foundItem != null)
                {
                    foundItem.SubItems[1].Text = _lastTimeModelFilePath.ToString();
                }
                else
                {
                    ListViewItem lstItem = new ListViewItem();
                    lstItem.Text = "LastTimeModelFilePath";
                    lstItem.SubItems.Add(_lastTimeModelFilePath.ToString());
                    this.lvDeviceStatus.Items.Insert(0, lstItem);//保证最新的显示在第一条
                }

                //automode
                foundItem = this.lvDeviceStatus.FindItemWithText("AutoModeFlag:");

                if (foundItem != null)
                {
                    foundItem.SubItems[1].Text = _autoModeFlag.ToString();
                }
                else
                {
                    ListViewItem lstItem = new ListViewItem();
                    lstItem.Text = "AutoModeFlag:";
                    lstItem.SubItems.Add(_autoModeFlag.ToString());
                    this.lvDeviceStatus.Items.Insert(0, lstItem);//保证最新的显示在第一条
                }

                //轴控制状态
                for (int i = _axisQuantity - 1; i >= 0; i--)
                {
                    foundItem = this.lvDeviceStatus.FindItemWithText("AxisAction" + i.ToString("00") + "Command");

                    if (foundItem != null)
                    {
                        foundItem.SubItems[1].Text = _axisControlFormVariate._axisActionCommand[i].ToString();
                    }
                    else
                    {
                        ListViewItem lstItem = new ListViewItem();
                        lstItem.Text = "AxisAction" + i.ToString("00") + "Command";
                        lstItem.SubItems.Add(_axisControlFormVariate._axisActionCommand[i].ToString());
                        this.lvDeviceStatus.Items.Insert(0, lstItem);//保证最新的显示在第一条
                    }

                    foundItem = this.lvDeviceStatus.FindItemWithText("Axis" + i.ToString("00") + "CurrentPositionNumber");

                    if (foundItem != null)
                    {
                        foundItem.SubItems[1].Text = _axisControlFormVariate._axisCurrentPositionNumber[i].ToString();
                    }
                    else
                    {
                        ListViewItem lstItem = new ListViewItem();
                        lstItem.Text = "Axis" + i.ToString("00") + "CurrentPositionNumber";
                        lstItem.SubItems.Add(_axisControlFormVariate._axisCurrentPositionNumber[i].ToString());
                        this.lvDeviceStatus.Items.Insert(0, lstItem);//保证最新的显示在第一条
                    }
                }

                //_conveyor1Status
                foundItem = this.lvDeviceStatus.FindItemWithText("Conveyor1Status");

                if (foundItem != null)
                {
                    foundItem.SubItems[1].Text = _conveyor1Status.ToString();
                }
                else
                {
                    ListViewItem lstItem = new ListViewItem();
                    lstItem.Text = "Conveyor1Status";
                    lstItem.SubItems.Add(_conveyor1Status.ToString());
                    this.lvDeviceStatus.Items.Insert(0, lstItem);//保证最新的显示在第一条
                }

                //_conveyor2Status
                foundItem = this.lvDeviceStatus.FindItemWithText("Conveyor2Status");

                if (foundItem != null)
                {
                    foundItem.SubItems[1].Text = _conveyor2Status.ToString();
                }
                else
                {
                    ListViewItem lstItem = new ListViewItem();
                    lstItem.Text = "Conveyor2Status";
                    lstItem.SubItems.Add(_conveyor2Status.ToString());
                    this.lvDeviceStatus.Items.Insert(0, lstItem);//保证最新的显示在第一条
                }

                lvDeviceStatus.EndUpdate();
            })); ;
        }
        #endregion

        #region 外围界面相关程序

        #region 产量相关
        /// <summary>
        /// LoadRecordYieldData：读取记录的产量数据
        /// </summary>
        private void LoadRecordYieldData()
        {
            MyTool.TxtFileProcess.CreateLog("读取记录的产量数据！" + "----用户：" + _operatorName,
            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

            string filePath = Directory.GetCurrentDirectory() + @"\" + "Yield.ini";
            StringBuilder getData = new StringBuilder(1024);
            if (!File.Exists(filePath))
            {
                File.Create(filePath);
            }
            MyTool.IniFunc.ReadIni("Yield", "YieldResetFlag", "Error", filePath, out getData);
            if (getData.ToString() == "Error")//如果没有读到数据
            {
                MyTool.IniFunc.WriteIni("Yield", "YieldResetFlag", _yieldResetFlag.ToString(), filePath);
            }
            try
            {
                int tempData = Convert.ToInt32(getData.ToString());
                if (tempData == 0 || tempData == 1)
                {
                    _yieldResetFlag = (uint)tempData;
                }
                else
                {
                    MyTool.IniFunc.WriteIni("Yield", "YieldResetFlag", _yieldResetFlag.ToString(), filePath);
                }
            }
            catch
            {
                MyTool.IniFunc.WriteIni("Yield", "YieldResetFlag", _yieldResetFlag.ToString(), filePath);
            }

            for (int i = 1; i < _yieldRecordDt.Columns.Count; i++)
            {
                string prefix = "Device";
                if (i == 2)//如果为传送线1数据
                {
                    prefix = "Conveyor1";
                }
                else if (i == 3)
                {
                    prefix = "Conveyor2";
                }
                //Element数据
                MyTool.IniFunc.ReadIni("Yield", prefix + "CheckPcsQuantity", "Error", filePath, out getData);
                if (getData.ToString() == "Error")//如果没有读到数据
                {
                    MyTool.IniFunc.WriteIni("Yield", prefix + "CheckPcsQuantity", _yieldRecordDt.Rows[0][i].ToString(), filePath);
                }
                try
                {
                    int tempData = Convert.ToInt32(getData.ToString());
                    _yieldRecordDt.Rows[0][i] = tempData.ToString();
                }
                catch
                {
                    MyTool.IniFunc.WriteIni("Yield", prefix + "CheckPcsQuantity", _yieldRecordDt.Rows[0][i].ToString(), filePath);
                }

                MyTool.IniFunc.ReadIni("Yield", prefix + "OKPcsQuantity", "Error", filePath, out getData);
                if (getData.ToString() == "Error")//如果没有读到数据
                {
                    MyTool.IniFunc.WriteIni("Yield", prefix + "OKPcsQuantity", _yieldRecordDt.Rows[1][i].ToString(), filePath);
                }
                try
                {
                    int tempData = Convert.ToInt32(getData.ToString());
                    _yieldRecordDt.Rows[1][i] = tempData.ToString();
                }
                catch
                {
                    MyTool.IniFunc.WriteIni("Yield", prefix + "OKPcsQuantity", _yieldRecordDt.Rows[1][i].ToString(), filePath);
                }

                MyTool.IniFunc.ReadIni("Yield", prefix + "NGPcsQuantity", "Error", filePath, out getData);
                if (getData.ToString() == "Error")//如果没有读到数据
                {
                    MyTool.IniFunc.WriteIni("Yield", prefix + "NGPcsQuantity", _yieldRecordDt.Rows[2][i].ToString(), filePath);
                }
                try
                {
                    int tempData = Convert.ToInt32(getData.ToString());
                    _yieldRecordDt.Rows[2][i] = tempData.ToString();
                }
                catch
                {
                    MyTool.IniFunc.WriteIni("Yield", prefix + "NGPcsQuantity", _yieldRecordDt.Rows[2][i].ToString(), filePath);
                }

                //Panel数据
                MyTool.IniFunc.ReadIni("Yield", prefix + "CheckPanelQuantity", "Error", filePath, out getData);
                if (getData.ToString() == "Error")//如果没有读到数据
                {
                    MyTool.IniFunc.WriteIni("Yield", prefix + "CheckPanelQuantity", _yieldRecordDt.Rows[4][i].ToString(), filePath);
                }
                try
                {
                    int tempData = Convert.ToInt32(getData.ToString());
                    _yieldRecordDt.Rows[4][i] = tempData.ToString();
                }
                catch
                {
                    MyTool.IniFunc.WriteIni("Yield", prefix + "CheckPanelQuantity", _yieldRecordDt.Rows[4][i].ToString(), filePath);
                }

                MyTool.IniFunc.ReadIni("Yield", prefix + "OKPanelQuantity", "Error", filePath, out getData);
                if (getData.ToString() == "Error")//如果没有读到数据
                {
                    MyTool.IniFunc.WriteIni("Yield", prefix + "OKPanelQuantity", _yieldRecordDt.Rows[5][i].ToString(), filePath);
                }
                try
                {
                    int tempData = Convert.ToInt32(getData.ToString());
                    _yieldRecordDt.Rows[5][i] = tempData.ToString();
                }
                catch
                {
                    MyTool.IniFunc.WriteIni("Yield", prefix + "OKPanelQuantity", _yieldRecordDt.Rows[5][i].ToString(), filePath);
                }

                MyTool.IniFunc.ReadIni("Yield", prefix + "NGPanelQuantity", "Error", filePath, out getData);
                if (getData.ToString() == "Error")//如果没有读到数据
                {
                    MyTool.IniFunc.WriteIni("Yield", prefix + "NGPanelQuantity", _yieldRecordDt.Rows[6][i].ToString(), filePath);
                }
                try
                {
                    int tempData = Convert.ToInt32(getData.ToString());
                    _yieldRecordDt.Rows[6][i] = tempData.ToString();
                }
                catch
                {
                    MyTool.IniFunc.WriteIni("Yield", prefix + "NGPanelQuantity", _yieldRecordDt.Rows[6][i].ToString(), filePath);
                }

                //Element数据
                MyTool.IniFunc.ReadIni("Yield", prefix + "CheckElementQuantity", "Error", filePath, out getData);
                if (getData.ToString() == "Error")//如果没有读到数据
                {
                    MyTool.IniFunc.WriteIni("Yield", prefix + "CheckElementQuantity", _yieldRecordDt.Rows[8][i].ToString(), filePath);
                }
                try
                {
                    int tempData = Convert.ToInt32(getData.ToString());
                    _yieldRecordDt.Rows[8][i] = tempData.ToString();
                }
                catch
                {
                    MyTool.IniFunc.WriteIni("Yield", prefix + "CheckElementQuantity", _yieldRecordDt.Rows[8][i].ToString(), filePath);
                }

                MyTool.IniFunc.ReadIni("Yield", prefix + "OKElementQuantity", "Error", filePath, out getData);
                if (getData.ToString() == "Error")//如果没有读到数据
                {
                    MyTool.IniFunc.WriteIni("Yield", prefix + "OKElementQuantity", _yieldRecordDt.Rows[9][i].ToString(), filePath);
                }
                try
                {
                    int tempData = Convert.ToInt32(getData.ToString());
                    _yieldRecordDt.Rows[9][i] = tempData.ToString();
                }
                catch
                {
                    MyTool.IniFunc.WriteIni("Yield", prefix + "OKElementQuantity", _yieldRecordDt.Rows[9][i].ToString(), filePath);
                }

                MyTool.IniFunc.ReadIni("Yield", prefix + "NGElementQuantity", "Error", filePath, out getData);
                if (getData.ToString() == "Error")//如果没有读到数据
                {
                    MyTool.IniFunc.WriteIni("Yield", prefix + "NGElementQuantity", _yieldRecordDt.Rows[10][i].ToString(), filePath);
                }
                try
                {
                    int tempData = Convert.ToInt32(getData.ToString());
                    _yieldRecordDt.Rows[10][i] = tempData.ToString();
                }
                catch
                {
                    MyTool.IniFunc.WriteIni("Yield", prefix + "NGElementQuantity", _yieldRecordDt.Rows[10][i].ToString(), filePath);
                }


            }

            int currentTimeHour = Convert.ToInt32(System.DateTime.Now.TimeOfDay.ToString("hh"));
            int currentTimeMinutes = Convert.ToInt32(System.DateTime.Now.TimeOfDay.ToString("mm"));

            if ((currentTimeHour > SettingsGeneralForm._clearYieldTimeHour ||
                (currentTimeHour == SettingsGeneralForm._clearYieldTimeHour && currentTimeMinutes > SettingsGeneralForm._clearYieldTimeMinutes))
                && (currentTimeHour < SettingsGeneralForm._clearYieldTimeHour + 12 ||
                 (currentTimeHour == SettingsGeneralForm._clearYieldTimeHour + 12 && currentTimeMinutes <= SettingsGeneralForm._clearYieldTimeMinutes))
                 && _yieldResetFlag == 0 && SettingsGeneralForm._autoClearYieldUseFlag)//如果为需要清除晚班数据，当前为白班的话，清除
            {
                ClearYieldData();
            }
            else if (((currentTimeHour >= 0 && (currentTimeHour < SettingsGeneralForm._clearYieldTimeHour || currentTimeHour == SettingsGeneralForm._clearYieldTimeHour && currentTimeMinutes <= SettingsGeneralForm._clearYieldTimeMinutes))
                || ((currentTimeHour > SettingsGeneralForm._clearYieldTimeHour + 12 || (currentTimeHour == SettingsGeneralForm._clearYieldTimeHour + 12 && currentTimeMinutes > SettingsGeneralForm._clearYieldTimeMinutes)) && currentTimeHour < 24))
                && _yieldResetFlag == 1 && SettingsGeneralForm._autoClearYieldUseFlag)//如果为需要清除白班数据，当前为晚班的话，清除
            {
                ClearYieldData();
            }
        }

        /// <summary>
        /// SaveRecordYieldData：保存产量数据
        /// </summary>
        private void SaveRecordYieldData()
        {
            string filePath = Directory.GetCurrentDirectory() + @"\" + "Yield.ini";
            StringBuilder getData = new StringBuilder(1024);
            if (!File.Exists(filePath))
            {
                File.Create(filePath);
            }

            while (MyTool.IniFunc.WriteIni("Yield", "YieldResetFlag", _yieldResetFlag.ToString(), filePath) == -1)
            {
                if (_deadlyAlarmOccurFlag || !_autoModeFlag)
                {
                    return;
                }
                Thread.Sleep(1);
            }

            for (int i = 1; i < _yieldRecordDt.Columns.Count; i++)
            {
                string prefix = "Device";
                if (i == 2)//如果为传送线1数据
                {
                    prefix = "Conveyor1";
                }
                else if (i == 3)
                {
                    prefix = "Conveyor2";
                }
                //Pcs数据
                while (MyTool.IniFunc.WriteIni("Yield", prefix + "CheckPcsQuantity", _yieldRecordDt.Rows[0][i].ToString(), filePath) == -1)
                {
                    if (_deadlyAlarmOccurFlag || !_autoModeFlag)
                    {
                        return;
                    }
                    Thread.Sleep(1);
                }

                while (MyTool.IniFunc.WriteIni("Yield", prefix + "OKPcsQuantity", _yieldRecordDt.Rows[1][i].ToString(), filePath) == -1)
                {
                    if (_deadlyAlarmOccurFlag || !_autoModeFlag)
                    {
                        return;
                    }
                    Thread.Sleep(1);
                }

                while (MyTool.IniFunc.WriteIni("Yield", prefix + "NGPcsQuantity", _yieldRecordDt.Rows[2][i].ToString(), filePath) == -1)
                {
                    if (_deadlyAlarmOccurFlag || !_autoModeFlag)
                    {
                        return;
                    }
                    Thread.Sleep(1);
                }

                //Panel数据
                while (MyTool.IniFunc.WriteIni("Yield", prefix + "CheckPanelQuantity", _yieldRecordDt.Rows[4][i].ToString(), filePath) == -1)
                {
                    if (_deadlyAlarmOccurFlag || !_autoModeFlag)
                    {
                        return;
                    }
                    Thread.Sleep(1);
                }

                while (MyTool.IniFunc.WriteIni("Yield", prefix + "OKPanelQuantity", _yieldRecordDt.Rows[5][i].ToString(), filePath) == -1)
                {
                    if (_deadlyAlarmOccurFlag || !_autoModeFlag)
                    {
                        return;
                    }
                    Thread.Sleep(1);
                }

                while (MyTool.IniFunc.WriteIni("Yield", prefix + "NGPanelQuantity", _yieldRecordDt.Rows[6][i].ToString(), filePath) == -1)
                {
                    if (_deadlyAlarmOccurFlag || !_autoModeFlag)
                    {
                        return;
                    }
                    Thread.Sleep(1);
                }

                //Element数据
                while (MyTool.IniFunc.WriteIni("Yield", prefix + "CheckElementQuantity", _yieldRecordDt.Rows[8][i].ToString(), filePath) == -1)
                {
                    if (_deadlyAlarmOccurFlag || !_autoModeFlag)
                    {
                        return;
                    }
                    Thread.Sleep(1);
                }

                while (MyTool.IniFunc.WriteIni("Yield", prefix + "OKElementQuantity", _yieldRecordDt.Rows[9][i].ToString(), filePath) == -1)
                {
                    if (_deadlyAlarmOccurFlag || !_autoModeFlag)
                    {
                        return;
                    }
                    Thread.Sleep(1);
                }

                while (MyTool.IniFunc.WriteIni("Yield", prefix + "NGElementQuantity", _yieldRecordDt.Rows[10][i].ToString(), filePath) == -1)
                {
                    if (_deadlyAlarmOccurFlag || !_autoModeFlag)
                    {
                        return;
                    }
                    Thread.Sleep(1);
                }

            }
        }

        /// <summary>
        /// ClearYieldData：清除产量数据
        /// </summary>
        private void ClearYieldData()
        {
            MyTool.TxtFileProcess.CreateLog("产量清除！" + "----用户：" + _operatorName,
            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

            for (int i = 1; i < _yieldRecordDt.Columns.Count; i++)
            {
                _yieldRecordDt.Rows[0][i] = "0";//PCS总产量清0
                _yieldRecordDt.Rows[1][i] = "0";//OK PCS数量清为0
                _yieldRecordDt.Rows[2][i] = "0";//NG PCS数量清为0
                _yieldRecordDt.Rows[3][i] = "0.0%";//PCS良率清为0
                _yieldRecordDt.Rows[4][i] = "0";//PANEL总产量清0
                _yieldRecordDt.Rows[5][i] = "0";//OK PANEL数量清为0
                _yieldRecordDt.Rows[6][i] = "0";//NG PANEL数量清为0
                _yieldRecordDt.Rows[7][i] = "0.0%";//PANEL 良率清为
                _yieldRecordDt.Rows[8][i] = "0";//Elemnt总产量清0
                _yieldRecordDt.Rows[9][i] = "0";//OK Elemnt数量清为0
                _yieldRecordDt.Rows[10][i] = "0";//NG Elemnt数量清为0
                _yieldRecordDt.Rows[11][i] = "0.0%";//Elemnt 良率清为
            }

            int currentTimeHour = Convert.ToInt32(System.DateTime.Now.TimeOfDay.ToString("hh"));
            int currentTimeMinutes = Convert.ToInt32(System.DateTime.Now.TimeOfDay.ToString("mm"));

            if ((currentTimeHour > SettingsGeneralForm._clearYieldTimeHour ||
            (currentTimeHour == SettingsGeneralForm._clearYieldTimeHour && currentTimeMinutes > SettingsGeneralForm._clearYieldTimeMinutes))
            && (currentTimeHour < SettingsGeneralForm._clearYieldTimeHour + 12 ||
            (currentTimeHour == SettingsGeneralForm._clearYieldTimeHour + 12 && currentTimeMinutes <= SettingsGeneralForm._clearYieldTimeMinutes)))//如果当前为白班的话
            {
                _yieldResetFlag = 1;//设置需要清除白班，当前处于白班，故而不清除
            }
            else
            {
                _yieldResetFlag = 0;//设置需要清除晚班，当前处于晚班，故而不清除
            }

            SaveRecordYieldData();
        }

        /// <summary>
        /// RefreshYield：基于数据更新产量良率等
        /// </summary>
        private void RefreshYield()
        {

            for (int i = 1; i < _yieldRecordDt.Columns.Count; i++)
            {
                if (Convert.ToInt32(_yieldRecordDt.Rows[0][i].ToString()) == 0)//如果PCS总产量
                {
                    _yieldRecordDt.Rows[0][i] = "0";//PCS总产量清0
                    _yieldRecordDt.Rows[1][i] = "0";//OK PCS数量清为0
                    _yieldRecordDt.Rows[2][i] = "0";//NG PCS数量清为0
                    _yieldRecordDt.Rows[3][i] = "0.0%";//PCS良率清为0
                }
                else
                {
                    //利用math.floor可以取数据不四舍五入
                    _yieldRecordDt.Rows[3][i] = (Math.Floor(((Convert.ToSingle(_yieldRecordDt.Rows[1][i]) / Convert.ToSingle(_yieldRecordDt.Rows[0][i])) * 100f) * 10) / 10).ToString()+"%";//元件良率
                }

                if (Convert.ToInt32(_yieldRecordDt.Rows[4][i].ToString()) == 0)//如果PANEL总产量为0
                {
                    _yieldRecordDt.Rows[4][i] = "0";//PANEL总产量清0
                    _yieldRecordDt.Rows[5][i] = "0";//OK PANEL数量清为0
                    _yieldRecordDt.Rows[6][i] = "0";//NG PANEL数量清为0
                    _yieldRecordDt.Rows[7][i] = "0.0%";//PANEL 良率清为0
                }
                else
                {
                    //利用math.floor可以取数据不四舍五入
                    _yieldRecordDt.Rows[7][i] = (Math.Floor(((Convert.ToSingle(_yieldRecordDt.Rows[5][i]) / Convert.ToSingle(_yieldRecordDt.Rows[4][i])) * 100f) * 10) / 10).ToString() + "%";//Panel良率
                }


                if (Convert.ToInt32(_yieldRecordDt.Rows[8][i].ToString()) == 0)//如果ELEMENT总产量为0
                {
                    _yieldRecordDt.Rows[8][i] = "0";//PANEL总产量清0
                    _yieldRecordDt.Rows[9][i] = "0";//OK PANEL数量清为0
                    _yieldRecordDt.Rows[10][i] = "0";//NG PANEL数量清为0
                    _yieldRecordDt.Rows[11][i] = "0.0%";//PANEL 良率清为0
                }
                else
                {
                    //利用math.floor可以取数据不四舍五入
                    _yieldRecordDt.Rows[11][i] = (Math.Floor(((Convert.ToSingle(_yieldRecordDt.Rows[9][i]) / Convert.ToSingle(_yieldRecordDt.Rows[8][i])) * 100f) * 10) / 10).ToString() + "%";//Element良率
                }
            }

            if (Convert.ToInt32(_yieldRecordDt.Rows[0][1].ToString()) == 0)//如果PCS总产量为0
            {
                for (int i = 1; i < _yieldRecordDt.Columns.Count; i++)
                {
                    _yieldRecordDt.Rows[0][i] = "0";//PCS总产量清0
                    _yieldRecordDt.Rows[1][i] = "0";//OK PCS数量清为0
                    _yieldRecordDt.Rows[2][i] = "0";//NG PCS数量清为0
                    _yieldRecordDt.Rows[3][i] = "0.0%";//PCS良率清为0
                }
            }

            if (Convert.ToInt32(_yieldRecordDt.Rows[4][1].ToString()) == 0)//如果PANEL总产量为0
            {
                for (int i = 1; i < _yieldRecordDt.Columns.Count; i++)
                {
                    _yieldRecordDt.Rows[4][i] = "0";//PANEL总产量清0
                    _yieldRecordDt.Rows[5][i] = "0";//OK PANEL数量清为0
                    _yieldRecordDt.Rows[6][i] = "0";//NG PANEL数量清为0
                    _yieldRecordDt.Rows[7][i] = "0.0%";//PANEL 良率清为0
                }
            }

            if (Convert.ToInt32(_yieldRecordDt.Rows[8][1].ToString()) == 0)//如果ELEMENT总产量为0
            {
                for (int i = 1; i < _yieldRecordDt.Columns.Count; i++)
                {
                    _yieldRecordDt.Rows[8][i] = "0";//ELEMENT总产量清0
                    _yieldRecordDt.Rows[9][i] = "0";//OK ELEMENT数量清为0
                    _yieldRecordDt.Rows[10][i] = "0";//NG ELEMENT数量清为0
                    _yieldRecordDt.Rows[11][i] = "0.0%";//ELEMENT 良率清为0
                }
            }

            dgrdvYieldRecord.Invoke(new Action(() =>
                    {
                        for (int i = 0; i < _yieldRecordDt.Columns.Count; i++)
                        {
                            if (rbtnPcsYield.Checked)
                            {
                                for (int j = 0; j < dgrdvYieldRecord.Rows.Count; j++)
                                {
                                    dgrdvYieldRecord.Rows[j].Cells[i].Value = _yieldRecordDt.Rows[j][i];
                                }
                            }
                            else if (rbtnPanelYield.Checked)
                            {
                                for (int j = 0; j < dgrdvYieldRecord.Rows.Count; j++)
                                {
                                    dgrdvYieldRecord.Rows[j].Cells[i].Value = _yieldRecordDt.Rows[4 + j][i];
                                }
                            }
                            else if (rbtnElementYield.Checked)
                            {
                                for (int j = 0; j < dgrdvYieldRecord.Rows.Count; j++)
                                {
                                    dgrdvYieldRecord.Rows[j].Cells[i].Value = _yieldRecordDt.Rows[8 + j][i];
                                }
                            }
                        }
                    })); ;

        }

        //显示PCS生产计数及良率选项发生变化事件
        private void rbtnPcsYield_CheckedChanged(object sender, EventArgs e)
        {
            if (rbtnPcsYield.Checked)
            {
                MyTool.TxtFileProcess.CreateLog("选中显示以PCS为单位的产量计数！" + "----用户：" + _operatorName,
                BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                rbtnPanelYield.Checked = false;
                rbtnElementYield.Checked = false;
            }
        }

        //显示Panel生产计数及良率选项发生变化事件
        private void rbtnPanelYield_CheckedChanged(object sender, EventArgs e)
        {
            if (rbtnPanelYield.Checked)
            {
                MyTool.TxtFileProcess.CreateLog("选中显示以Panel为单位的产量计数！" + "----用户：" + _operatorName,
                BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                rbtnPcsYield.Checked = false;
                rbtnElementYield.Checked = false;
            }
        }

        //显示Element生产计数及良率选项发生变化事件
        private void rbtnElementYield_CheckedChanged(object sender, EventArgs e)
        {
            if (rbtnElementYield.Checked)
            {
                MyTool.TxtFileProcess.CreateLog("选中显示以元件为单位的产量计数！" + "----用户：" + _operatorName,
                BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                rbtnPcsYield.Checked = false;
                rbtnPanelYield.Checked = false;
            }
        }

        //点击产量清除按钮
        private void btnClearYield_Click(object sender, EventArgs e)
        {
            MyTool.TxtFileProcess.CreateLog("点击产量清除按钮！" + "----用户：" + _operatorName,
            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);
             ClearYieldData();
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
            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

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
            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

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

            //if (_SPCFormVariate != null)
            //{
                //_SPCFormVariate.UserLog(accessLevel, userName, hideFormFlag);
            //}
 
        }

        /// <summary>
        /// UserFormRequestCloseEventFunc:用户界面请求关闭事件
        /// </summary>
        private void UserFormRequestCloseEventFunc()
        {
            MyTool.TxtFileProcess.CreateLog("关闭用户登录编辑页面！" + "----用户：" + _operatorName,
            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);
            _userManagerVariate.Hide();

            //if (_SPCFormVariate != null)
           // {
              //  _SPCFormVariate.UserManagerHide();
            //}
        }

        #endregion

        #region 样品板检测与否及时间状态读写

        /// <summary>
        /// SamplePanelCheckTimeAndStatusSaveFunc：样品板检测时间及状态保存函数
        /// </summary>
        /// <param name="conveyorIndex">int：传送线索引，0-传送线1，1-传送线2</param>
        /// <param name="sampleCheckStatus">bool：传送线样品板检测检测状态，false-未检测，true-检测成功</param>
        private void SamplePanelCheckTimeAndStatusSaveFunc(int conveyorIndex, bool sampleCheckStatus)
        {
            MyTool.TxtFileProcess.CreateLog("保存传送线"+(conveyorIndex+1).ToString()+ "的样品板检测状态和时间！" + "----用户：" + _operatorName,
            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

            string filePath = Directory.GetCurrentDirectory() + @"\" + "SamplePanelCheckStatus.ini";

            string currentTime = System.DateTime.Now.ToString("yyyyMMddHHmm");
            if (!File.Exists(filePath))
            {
                File.Create(filePath);
            }

            while (MyTool.IniFunc.WriteIni("SamplePanelCheckStatus", "Conveyor" + (conveyorIndex + 1).ToString() + "SamplePanelCheckTimeAndStatus", sampleCheckStatus.ToString() + "_" + currentTime, filePath) == -1)
            {
                if (_deadlyAlarmOccurFlag || !_autoModeFlag)
                {
                    return;
                }
                Thread.Sleep(1);
            }
        }

        /// <summary>
        /// SamplePanelCheckTimeAndStatusLoadFunc:样品板检测时间及状态读取函数
        /// </summary>
        /// <param name="conveyorIndex"></param>
        /// <returns></returns>
        private bool SamplePanelCheckTimeAndStatusLoadFunc(int conveyorIndex, out bool samplePanelCheckResult, 
            out int samplePanelCheckYear, out int samplePanelCheckMonth, out int samplePanelCheckDay, out int samplePanelCheckHour, out int samplePanelCheckMinutes)
        {
            //MyTool.TxtFileProcess.CreateLog("读取传送线" + (conveyorIndex + 1).ToString() + "的样品板检测状态和时间！" + "----用户：" + _operatorName,
            //BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

            StringBuilder getData = new StringBuilder(1024);
            string filePath = Directory.GetCurrentDirectory() + @"\" + "SamplePanelCheckStatus.ini";
            samplePanelCheckResult = false;
            samplePanelCheckYear = 0;
            samplePanelCheckMonth = 0;
            samplePanelCheckDay = 0;
            samplePanelCheckHour = 0;
            samplePanelCheckMinutes = 0;

            if (!File.Exists(filePath))
            {
                File.Create(filePath);
                SamplePanelCheckTimeAndStatusSaveFunc(conveyorIndex, false);
                return false;
            }

            MyTool.IniFunc.ReadIni("SamplePanelCheckStatus", "Conveyor" + (conveyorIndex + 1).ToString() + "SamplePanelCheckTimeAndStatus", "Error", filePath, out getData);
            if (getData.ToString() == "Error")//如果没有读到数据
            {
                SamplePanelCheckTimeAndStatusSaveFunc(conveyorIndex, false);
                return false;
            }

            try
            {
                if (getData.ToString().Substring(0, getData.ToString().IndexOf("_")) == "True")//如果为验证过了
                    samplePanelCheckResult = true;
                else
                    samplePanelCheckResult = false;

                if (samplePanelCheckResult)//如果为验证过，接下来就判定这个验证时间是否有效
                {
                    string SamplePanelCheckTime = getData.ToString().Substring(getData.ToString().IndexOf("_") + 1);
                    samplePanelCheckYear = Convert.ToInt32(SamplePanelCheckTime.Substring(0, 4));
                    samplePanelCheckMonth = Convert.ToInt32(SamplePanelCheckTime.Substring(4, 2));
                    samplePanelCheckDay = Convert.ToInt32(SamplePanelCheckTime.Substring(6, 2));
                    samplePanelCheckHour = Convert.ToInt32(SamplePanelCheckTime.Substring(8, 2));
                    samplePanelCheckMinutes = Convert.ToInt32(SamplePanelCheckTime.Substring(10, 2));

                    bool currentTimeBelongDay = false;//当前时间属于白班标志，false-属于夜班，true-属于白班
                    string tempCurrentTime = DateTime.Now.ToString("yyyyMMddHHmm");
                    int currentYear = Convert.ToInt32(tempCurrentTime.Substring(0, 4));
                    int currentMonth = Convert.ToInt32(tempCurrentTime.Substring(4, 2));
                    int currentDay = Convert.ToInt32(tempCurrentTime.Substring(6, 2));
                    int currentHour = Convert.ToInt32(tempCurrentTime.Substring(8, 2));
                    int currentMinutes = Convert.ToInt32(tempCurrentTime.Substring(10, 2));

                    //检测当前是否属于白班
                    if ((currentHour > SettingsGeneralForm._clearYieldTimeHour || (currentHour == SettingsGeneralForm._clearYieldTimeHour && currentMinutes >= SettingsGeneralForm._clearYieldTimeMinutes))
                        && (currentHour < SettingsGeneralForm._clearYieldTimeHour + 12 || (currentHour == SettingsGeneralForm._clearYieldTimeHour + 12 && currentMinutes < SettingsGeneralForm._clearYieldTimeMinutes)))
                    {
                        currentTimeBelongDay = true;
                    }

                    if (currentTimeBelongDay)//如果为白班
                    {
                        if (samplePanelCheckYear == currentYear &&
                            samplePanelCheckMonth == currentMonth &&
                            samplePanelCheckDay == currentDay &&
                            (samplePanelCheckHour > SettingsGeneralForm._clearYieldTimeHour || (samplePanelCheckHour == SettingsGeneralForm._clearYieldTimeHour && samplePanelCheckMinutes >= SettingsGeneralForm._clearYieldTimeMinutes)) &&
                            (samplePanelCheckHour < SettingsGeneralForm._clearYieldTimeHour + 12 || (samplePanelCheckHour == SettingsGeneralForm._clearYieldTimeHour + 12 && samplePanelCheckMinutes < SettingsGeneralForm._clearYieldTimeMinutes)))
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
                        int yesterdayYear, yesterdayMonth, yesterdayDay;
                        MyTool.Other.YesterdayCalculate(currentYear, currentMonth, currentDay, out yesterdayYear, out yesterdayMonth, out yesterdayDay);

                        if (currentHour >= SettingsGeneralForm._clearYieldTimeHour + 12 && currentHour < 24)//如果时间为当前夜
                        {
                            if (samplePanelCheckYear == currentYear &&
                            samplePanelCheckMonth == currentMonth &&
                            samplePanelCheckDay == currentDay &&
                            (samplePanelCheckHour > SettingsGeneralForm._clearYieldTimeHour + 12 || (samplePanelCheckHour == SettingsGeneralForm._clearYieldTimeHour + 12 && samplePanelCheckMinutes >= SettingsGeneralForm._clearYieldTimeMinutes)))
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else if (currentHour >= 0 && currentHour <= SettingsGeneralForm._clearYieldTimeHour)//如果为第二天凌晨
                        {
                            if ((samplePanelCheckYear == currentYear && samplePanelCheckMonth == currentMonth && samplePanelCheckDay == currentDay && (samplePanelCheckHour >= 0 && samplePanelCheckHour <= SettingsGeneralForm._clearYieldTimeHour)) ||
                                (samplePanelCheckYear == yesterdayYear && samplePanelCheckMonth == yesterdayMonth && samplePanelCheckDay == yesterdayDay &&
                                (samplePanelCheckHour > SettingsGeneralForm._clearYieldTimeHour + 12 || (samplePanelCheckHour == SettingsGeneralForm._clearYieldTimeHour + 12 && samplePanelCheckMinutes >= SettingsGeneralForm._clearYieldTimeMinutes))))
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
                            return false;
                        }
                    }

                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// ProductShiftTimeSaveFunc:产品切换时间保存函数
        /// </summary>
        /// <param name="conveyorIndex">int:传送线索引</param>
        private void ProductShiftTimeSaveFunc(int conveyorIndex)
        {

            MyTool.TxtFileProcess.CreateLog("保存传送线" + (conveyorIndex + 1).ToString() + "的产品切换时间！" + "----用户：" + _operatorName,
            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

            string filePath = Directory.GetCurrentDirectory() + @"\" + "SamplePanelCheckStatus.ini";

            string currentTime = System.DateTime.Now.ToString("yyyyMMddHHmm");
            if (!File.Exists(filePath))
            {
                File.Create(filePath);
            }

            while (MyTool.IniFunc.WriteIni("ProductChange", "Conveyor" + (conveyorIndex + 1).ToString() + "ProductChangeTime", currentTime, filePath) == -1)
            {
                if (_deadlyAlarmOccurFlag || !_autoModeFlag)
                {
                    return;
                }
                Thread.Sleep(1);
            }
        }

        /// <summary>
        /// ProductShiftTimeLoadFunc:产品切换时间读取函数
        /// </summary>
        /// <param name="conveyorIndex">int:传送线索引</param>
        /// <returns>bool：切换时间是否属于警告时间，false-不属于，true-属于</returns>
        private void ProductShiftTimeLoadFunc(int conveyorIndex,out int productShiftYear,out int productShiftMonth,out int productShiftDay,out int productShiftHour,out int productShiftMinutes)
        {
            StringBuilder getData = new StringBuilder(1024);
            string filePath = Directory.GetCurrentDirectory() + @"\" + "SamplePanelCheckStatus.ini";

            if (!File.Exists(filePath))
            {
                File.Create(filePath);
                ProductShiftTimeSaveFunc(conveyorIndex);

                string currentTime = System.DateTime.Now.ToString("yyyyMMddHHmm");
                productShiftYear = Convert.ToInt32(currentTime.Substring(0, 4));
                productShiftMonth = Convert.ToInt32(currentTime.Substring(4, 2));
                productShiftDay = Convert.ToInt32(currentTime.Substring(6, 2));
                productShiftHour = Convert.ToInt32(currentTime.Substring(8, 2));
                productShiftMinutes = Convert.ToInt32(currentTime.Substring(10, 2));
            }

            MyTool.IniFunc.ReadIni("ProductChange", "Conveyor" + (conveyorIndex + 1).ToString() + "ProductChangeTime", "Error", filePath, out getData);
            if (getData.ToString() == "Error")//如果没有读到数据
            {
                ProductShiftTimeSaveFunc(conveyorIndex);

                string currentTime = System.DateTime.Now.ToString("yyyyMMddHHmm");
                productShiftYear = Convert.ToInt32(currentTime.Substring(0, 4));
                productShiftMonth = Convert.ToInt32(currentTime.Substring(4, 2));
                productShiftDay = Convert.ToInt32(currentTime.Substring(6, 2));
                productShiftHour = Convert.ToInt32(currentTime.Substring(8, 2));
                productShiftMinutes = Convert.ToInt32(currentTime.Substring(10, 2));
            }

            try
            {
                    productShiftYear = Convert.ToInt32(getData.ToString().Substring(0, 4));
                    productShiftMonth = Convert.ToInt32(getData.ToString().Substring(4, 2));
                    productShiftDay = Convert.ToInt32(getData.ToString().Substring(6, 2));
                    productShiftHour = Convert.ToInt32(getData.ToString().Substring(8, 2));
                    productShiftMinutes = Convert.ToInt32(getData.ToString().Substring(10, 2));

            }
            catch
            {
                ProductShiftTimeSaveFunc(conveyorIndex);
                string currentTime = System.DateTime.Now.ToString("yyyyMMddHHmm");
                productShiftYear = Convert.ToInt32(currentTime.Substring(0, 4));
                productShiftMonth = Convert.ToInt32(currentTime.Substring(4, 2));
                productShiftDay = Convert.ToInt32(currentTime.Substring(6, 2));
                productShiftHour = Convert.ToInt32(currentTime.Substring(8, 2));
                productShiftMinutes = Convert.ToInt32(currentTime.Substring(10, 2));
            }
                    
        }

        #endregion

        #region SPC界面
        //打开SPC界面
        private void btnSPCOpen_Click(object sender, EventArgs e)
        {

            MyTool.TxtFileProcess.CreateLog("打开SPC窗口！", BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, _logFileMaxSize);
            if (_SPCFormVariate != null)
            {
                _SPCFormVariate.Dispose();
                _SPCFormVariate = null;
            }

            _SPCFormVariate = new ThreeDAVIInspectHistoryBrowseMainForm();
            _SPCFormVariate._normalProductResultSavePath = SettingsGeneralForm._dataDirectory;
            _SPCFormVariate._samplePanelResultSavePath = SettingsGeneralForm._samplePanelDirectory;
            _SPCFormVariate.SPCFormLogInFinishEvent += SPCFormLogInFinishEventFunc;
            _SPCFormVariate.SPCFormUserManagerFormCloseEvent += SPCFormUserManagerFormCloseEventFunc;
            _SPCFormVariate.Show();

           // _SPCFormVariate.UserLog(_accessLevel, _operatorName, true);
        }

        //SPC界面用户登录事件函数
        private void SPCFormLogInFinishEventFunc(int accessLevel, string userName, bool hideFormFlag)
        {
            //MyTool.TxtFileProcess.CreateLog("从SPC界面切换用户至" + userName + ",权限等级为" + accessLevel.ToString() + "----用户：" + _operatorName,
            //BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

            //UserFormUserLogInFinishEventFunc(accessLevel, userName, hideFormFlag);
        }

        //SPC界面用户管理界面关闭事件函数
        private void SPCFormUserManagerFormCloseEventFunc()
        {
            //MyTool.TxtFileProcess.CreateLog("从SPC界面关闭用户登录编辑页面！" + "----用户：" + _operatorName,
           // BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

            //UserFormRequestCloseEventFunc();
        }

        #endregion

        #endregion
    }
}

