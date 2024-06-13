using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using csNMCLib20;

namespace MotionCard
{
    public class GaoChuanMotionCardGCN400AS
    {
        #region 变量声明

        #region 请根据项目需求修改
        public int _cardQuantity = 1;//指定卡的个数
        public int _axisQuantity;//轴的个数
        public int _motionCardIOMaxQuantity_320 = 320;//运动控制卡的最大IO个数:输入或者输出的最大个数,本地加扩展最多为320
        //public int _motioncardCommonIOMAXQuantity_32 = 32;//运动控制卡的最大通用IO点数(通用输入为32点,通用输出为27点)
        //public int _motionCardOutputQuantity_27 = 27;//运动控制卡的输出点数(只有通用输出)
        public int _maxIOQuantity;//输入或者输出的IO个数(一块卡为输入及输出都为320点,两块卡为640)
        public int _axisQuantityPerCard_4 = 4;//每块卡的轴的个数

        public float[] _axismmPP = new float[] { 0.0005f, 0.0005f, 0.001f, 0.01f, 0.01f, 0.01f, 0.01f, 0.01f };
        public string[] _axisNameStrArray = new string[] { "X1轴", "Y1轴", " X2轴", "???轴", "???轴", "???轴", "???轴", "???轴" };
        public string[] _axisUnitArray = new string[] { "mm", "mm", "mm", "mm", "mm", "mm", "mm", "mm" };//轴单位数组，为mm或°
        public bool[] _inchMoveFlag = new bool[] { false, false, false, false, false, false, false, false };//轴采用inch移动标志，false-jog运行，true-inch运行
        public float[] _axisSoftLimitMinus = new float[] { 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f };
        public float[] _axisSoftLimitPlus = new float[] { 500f, 500f, 100f, 100f, 100f, 100f, 100f, 100f };
        #endregion

        private bool initialFlag = false;//运动卡初始化标志
        public bool _initialFlag
        {
            get { return initialFlag; }
            set { }
        }

        //毫米/脉冲
        public float _axis00mmPP = 0.01f;
        public float _axis01mmPP = 0.01f;
        public float _axis02mmPP = 0.01f;
        public float _axis03mmPP = 0.01f;
        public float _axis04mmPP = 0.01f;
        public float _axis05mmPP = 0.01f;
        public float _axis06mmPP = 0.01f;
        public float _axis07mmPP = 0.01f;
        public float _axis08mmPP = 0.01f;
        public float _axis09mmPP = 0.01f;
        public float _axis10mmPP = 0.01f;
        public float _axis11mmPP = 0.01f;
        //当前位置_脉冲数
        public int axis00CurrentPosition_Pulse = 0;
        public int axis01CurrentPosition_Pulse = 0;
        public int axis02CurrentPosition_Pulse = 0;
        public int axis03CurrentPosition_Pulse = 0;
        public int axis04CurrentPosition_Pulse = 0;
        public int axis05CurrentPosition_Pulse = 0;
        public int axis06CurrentPosition_Pulse = 0;
        public int axis07CurrentPosition_Pulse = 0;
        public int axis08CurrentPosition_Pulse = 0;
        public int axis09CurrentPosition_Pulse = 0;
        public int axis10CurrentPosition_Pulse = 0;
        public int axis11CurrentPosition_Pulse = 0;
        //当前位置_毫米
        public float _axis00CurrentPosition_mm = 0.0f;
        public float _axis01CurrentPosition_mm = 0.0f;
        public float _axis02CurrentPosition_mm = 0.0f;
        public float _axis03CurrentPosition_mm = 0.0f;
        public float _axis04CurrentPosition_mm = 0.0f;
        public float _axis05CurrentPosition_mm = 0.0f;
        public float _axis06CurrentPosition_mm = 0.0f;
        public float _axis07CurrentPosition_mm = 0.0f;
        public float _axis08CurrentPosition_mm = 0.0f;
        public float _axis09CurrentPosition_mm = 0.0f;
        public float _axis10CurrentPosition_mm = 0.0f;
        public float _axis11CurrentPosition_mm = 0.0f;
        //当前定位偏差值_毫米
        private float axis00CurrentDeviationValue_mm;
        private float axis01CurrentDeviationValue_mm;
        private float axis02CurrentDeviationValue_mm;
        private float axis03CurrentDeviationValue_mm;
        private float axis04CurrentDeviationValue_mm;
        private float axis05CurrentDeviationValue_mm;
        private float axis06CurrentDeviationValue_mm;
        private float axis07CurrentDeviationValue_mm;
        private float axis08CurrentDeviationValue_mm;
        private float axis09CurrentDeviationValue_mm;
        private float axis10CurrentDeviationValue_mm;
        private float axis11CurrentDeviationValue_mm;
        //目标位置_脉冲数
        private int axis00TargetPosition_Pulse;
        private int axis01TargetPosition_Pulse;
        private int axis02TargetPosition_Pulse;
        private int axis03TargetPosition_Pulse;
        private int axis04TargetPosition_Pulse;
        private int axis05TargetPosition_Pulse;
        private int axis06TargetPosition_Pulse;
        private int axis07TargetPosition_Pulse;
        private int axis08TargetPosition_Pulse;
        private int axis09TargetPosition_Pulse;
        private int axis10TargetPosition_Pulse;
        private int axis11TargetPosition_Pulse;
        //目标位置_毫米
        public float axis00TargetPosition_mm = 0.0f;
        public float axis01TargetPosition_mm = 0.0f;
        public float axis02TargetPosition_mm = 0.0f;
        public float axis03TargetPosition_mm = 0.0f;
        public float axis04TargetPosition_mm = 0.0f;
        public float axis05TargetPosition_mm = 0.0f;
        public float axis06TargetPosition_mm = 0.0f;
        public float axis07TargetPosition_mm = 0.0f;
        public float axis08TargetPosition_mm = 0.0f;
        public float axis09TargetPosition_mm = 0.0f;
        public float axis10TargetPosition_mm = 0.0f;
        public float axis11TargetPosition_mm = 0.0f;
        //目标定位偏差值_毫米
        public float axis00TargetDeviationValue_mm = 0.0f;
        public float axis01TargetDeviationValue_mm = 0.0f;
        public float axis02TargetDeviationValue_mm = 0.0f;
        public float axis03TargetDeviationValue_mm = 0.0f;
        public float axis04TargetDeviationValue_mm = 0.0f;
        public float axis05TargetDeviationValue_mm = 0.0f;
        public float axis06TargetDeviationValue_mm = 0.0f;
        public float axis07TargetDeviationValue_mm = 0.0f;
        public float axis08TargetDeviationValue_mm = 0.0f;
        public float axis09TargetDeviationValue_mm = 0.0f;
        public float axis10TargetDeviationValue_mm = 0.0f;
        public float axis11TargetDeviationValue_mm = 0.0f;
        //目标起始速度
        public float axis00TargetStartVelocity_mmps;
        public float axis01TargetStartVelocity_mmps;
        public float axis02TargetStartVelocity_mmps;
        public float axis03TargetStartVelocity_mmps;
        public float axis04TargetStartVelocity_mmps;
        public float axis05TargetStartVelocity_mmps;
        public float axis06TargetStartVelocity_mmps;
        public float axis07TargetStartVelocity_mmps;
        public float axis08TargetStartVelocity_mmps;
        public float axis09TargetStartVelocity_mmps;
        public float axis10TargetStartVelocity_mmps;
        public float axis11TargetStartVelocity_mmps;
        //目标最大速度
        public float axis00TargetMaxVelocity_mmps;
        public float axis01TargetMaxVelocity_mmps;
        public float axis02TargetMaxVelocity_mmps;
        public float axis03TargetMaxVelocity_mmps;
        public float axis04TargetMaxVelocity_mmps;
        public float axis05TargetMaxVelocity_mmps;
        public float axis06TargetMaxVelocity_mmps;
        public float axis07TargetMaxVelocity_mmps;
        public float axis08TargetMaxVelocity_mmps;
        public float axis09TargetMaxVelocity_mmps;
        public float axis10TargetMaxVelocity_mmps;
        public float axis11TargetMaxVelocity_mmps;
        //目标加速时间
        public double axis00TargetAccTime_s;
        public double axis01TargetAccTime_s;
        public double axis02TargetAccTime_s;
        public double axis03TargetAccTime_s;
        public double axis04TargetAccTime_s;
        public double axis05TargetAccTime_s;
        public double axis06TargetAccTime_s;
        public double axis07TargetAccTime_s;
        public double axis08TargetAccTime_s;
        public double axis09TargetAccTime_s;
        public double axis10TargetAccTime_s;
        public double axis11TargetAccTime_s;
        //极限
        //软负极限
        public float _axis00softLimitMinus = 0.0f;
        public float _axis01softLimitMinus = 0.0f;
        public float _axis02softLimitMinus = 0.0f;
        public float _axis03softLimitMinus = 0.0f;
        public float _axis04softLimitMinus = 0.0f;
        public float _axis05softLimitMinus = 0.0f;
        public float _axis06softLimitMinus = 0.0f;
        public float _axis07softLimitMinus = 0.0f;
        public float _axis08softLimitMinus = 0.0f;
        public float _axis09softLimitMinus = 0.0f;
        public float _axis10softLimitMinus = 0.0f;
        public float _axis11softLimitMinus = 0.0f;
        //软正极限
        public float _axis00softLimitPlus = 0.0f;
        public float _axis01softLimitPlus = 0.0f;
        public float _axis02softLimitPlus = 0.0f;
        public float _axis03softLimitPlus = 0.0f;
        public float _axis04softLimitPlus = 0.0f;
        public float _axis05softLimitPlus = 0.0f;
        public float _axis06softLimitPlus = 0.0f;
        public float _axis07softLimitPlus = 0.0f;
        public float _axis08softLimitPlus = 0.0f;
        public float _axis09softLimitPlus = 0.0f;
        public float _axis10softLimitPlus = 0.0f;
        public float _axis11softLimitPlus = 0.0f;
        //最小速度
        public float _axis00MoveMinSpeed_mmps = 1.0f;
        public float _axis01MoveMinSpeed_mmps = 1.0f;
        public float _axis02MoveMinSpeed_mmps = 1.0f;
        public float _axis03MoveMinSpeed_mmps = 1.0f;
        public float _axis04MoveMinSpeed_mmps = 1.0f;
        public float _axis05MoveMinSpeed_mmps = 1.0f;
        public float _axis06MoveMinSpeed_mmps = 1.0f;
        public float _axis07MoveMinSpeed_mmps = 1.0f;
        public float _axis08MoveMinSpeed_mmps = 1.0f;
        public float _axis09MoveMinSpeed_mmps = 1.0f;
        public float _axis10MoveMinSpeed_mmps = 1.0f;
        public float _axis11MoveMinSpeed_mmps = 1.0f;
        //最大速度
        public float _axis00MoveMaxSpeed_mmps = 1000.0f;
        public float _axis01MoveMaxSpeed_mmps = 1000.0f;
        public float _axis02MoveMaxSpeed_mmps = 1000.0f;
        public float _axis03MoveMaxSpeed_mmps = 1000.0f;
        public float _axis04MoveMaxSpeed_mmps = 1000.0f;
        public float _axis05MoveMaxSpeed_mmps = 1000.0f;
        public float _axis06MoveMaxSpeed_mmps = 1000.0f;
        public float _axis07MoveMaxSpeed_mmps = 1000.0f;
        public float _axis08MoveMaxSpeed_mmps = 1000.0f;
        public float _axis09MoveMaxSpeed_mmps = 1000.0f;
        public float _axis10MoveMaxSpeed_mmps = 1000.0f;
        public float _axis11MoveMaxSpeed_mmps = 1000.0f;
        //最小加速时间
        public float _axis00MoveMinAccTime_s = 0.0f;
        public float _axis01MoveMinAccTime_s = 0.0f;
        public float _axis02MoveMinAccTime_s = 0.0f;
        public float _axis03MoveMinAccTime_s = 0.0f;
        public float _axis04MoveMinAccTime_s = 0.0f;
        public float _axis05MoveMinAccTime_s = 0.0f;
        public float _axis06MoveMinAccTime_s = 0.0f;
        public float _axis07MoveMinAccTime_s = 0.0f;
        public float _axis08MoveMinAccTime_s = 0.0f;
        public float _axis09MoveMinAccTime_s = 0.0f;
        public float _axis10MoveMinAccTime_s = 0.0f;
        public float _axis11MoveMinAccTime_s = 0.0f;
        //最大加速时间
        public float _axis00MoveMaxAccTime_s = 0.0f;
        public float _axis01MoveMaxAccTime_s = 0.0f;
        public float _axis02MoveMaxAccTime_s = 0.0f;
        public float _axis03MoveMaxAccTime_s = 0.0f;
        public float _axis04MoveMaxAccTime_s = 0.0f;
        public float _axis05MoveMaxAccTime_s = 0.0f;
        public float _axis06MoveMaxAccTime_s = 0.0f;
        public float _axis07MoveMaxAccTime_s = 0.0f;
        public float _axis08MoveMaxAccTime_s = 0.0f;
        public float _axis09MoveMaxAccTime_s = 0.0f;
        public float _axis10MoveMaxAccTime_s = 0.0f;
        public float _axis11MoveMaxAccTime_s = 0.0f;
        //最小定位误差
        public float _axis00MinDeviation_mm = 0.0f;
        public float _axis01MinDeviation_mm = 0.0f;
        public float _axis02MinDeviation_mm = 0.0f;
        public float _axis03MinDeviation_mm = 0.0f;
        public float _axis04MinDeviation_mm = 0.0f;
        public float _axis05MinDeviation_mm = 0.0f;
        public float _axis06MinDeviation_mm = 0.0f;
        public float _axis07MinDeviation_mm = 0.0f;
        public float _axis08MinDeviation_mm = 0.0f;
        public float _axis09MinDeviation_mm = 0.0f;
        public float _axis10MinDeviation_mm = 0.0f;
        public float _axis11MinDeviation_mm = 0.0f;
        //最大定位误差
        public float _axis00MaxDeviation_mm = 0.0f;
        public float _axis01MaxDeviation_mm = 0.0f;
        public float _axis02MaxDeviation_mm = 0.0f;
        public float _axis03MaxDeviation_mm = 0.0f;
        public float _axis04MaxDeviation_mm = 0.0f;
        public float _axis05MaxDeviation_mm = 0.0f;
        public float _axis06MaxDeviation_mm = 0.0f;
        public float _axis07MaxDeviation_mm = 0.0f;
        public float _axis08MaxDeviation_mm = 0.0f;
        public float _axis09MaxDeviation_mm = 0.0f;
        public float _axis10MaxDeviation_mm = 0.0f;
        public float _axis11MaxDeviation_mm = 0.0f;

        //初始速度
        public float axis00StartVelocity = 1.0f;
        public float axis01StartVelocity = 1.0f;
        public float axis02StartVelocity = 1.0f;
        public float axis03StartVelocity = 1.0f;
        public float axis04StartVelocity = 1.0f;
        public float axis05StartVelocity = 1.0f;
        public float axis06StartVelocity = 1.0f;
        public float axis07StartVelocity = 1.0f;
        public float axis08StartVelocity = 1.0f;
        public float axis09StartVelocity = 1.0f;
        public float axis10StartVelocity = 1.0f;
        public float axis11StartVelocity = 1.0f;

        //回原点初始速度
        public float _axis00HomeMoveStartVelocity_mmps = 1.0f;
        public float _axis01HomeMoveStartVelocity_mmps = 1.0f;
        public float _axis02HomeMoveStartVelocity_mmps = 1.0f;
        public float _axis03HomeMoveStartVelocity_mmps = 1.0f;
        public float _axis04HomeMoveStartVelocity_mmps = 1.0f;
        public float _axis05HomeMoveStartVelocity_mmps = 1.0f;
        public float _axis06HomeMoveStartVelocity_mmps = 1.0f;
        public float _axis07HomeMoveStartVelocity_mmps = 1.0f;
        public float _axis08HomeMoveStartVelocity_mmps = 1.0f;
        public float _axis09HomeMoveStartVelocity_mmps = 1.0f;
        public float _axis10HomeMoveStartVelocity_mmps = 1.0f;
        public float _axis11HomeMoveStartVelocity_mmps = 1.0f;

        //回原点运行速度(负值表示往负方向找原点，正值表示往正方向找原点)
        public float _axis00HomeMoveMaxVelocity_mmps = 1.0f;
        public float _axis01HomeMoveMaxVelocity_mmps = 1.0f;
        public float _axis02HomeMoveMaxVelocity_mmps = 1.0f;
        public float _axis03HomeMoveMaxVelocity_mmps = 1.0f;
        public float _axis04HomeMoveMaxVelocity_mmps = 1.0f;
        public float _axis05HomeMoveMaxVelocity_mmps = 1.0f;
        public float _axis06HomeMoveMaxVelocity_mmps = 1.0f;
        public float _axis07HomeMoveMaxVelocity_mmps = 1.0f;
        public float _axis08HomeMoveMaxVelocity_mmps = 1.0f;
        public float _axis09HomeMoveMaxVelocity_mmps = 1.0f;
        public float _axis10HomeMoveMaxVelocity_mmps = 1.0f;
        public float _axis11HomeMoveMaxVelocity_mmps = 1.0f;

        //回原点加速时间
        public float _axis00HomeMoveAccTime_s = 0.0f;
        public float _axis01HomeMoveAccTime_s = 0.0f;
        public float _axis02HomeMoveAccTime_s = 0.0f;
        public float _axis03HomeMoveAccTime_s = 0.0f;
        public float _axis04HomeMoveAccTime_s = 0.0f;
        public float _axis05HomeMoveAccTime_s = 0.0f;
        public float _axis06HomeMoveAccTime_s = 0.0f;
        public float _axis07HomeMoveAccTime_s = 0.0f;
        public float _axis08HomeMoveAccTime_s = 0.0f;
        public float _axis09HomeMoveAccTime_s = 0.0f;
        public float _axis10HomeMoveAccTime_s = 0.0f;
        public float _axis11HomeMoveAccTime_s = 0.0f;

        //HomeFindSensorVelocity
        public float _axis00HomeFindSensorVelocity_mmps = 1.0f;
        public float _axis01HomeFindSensorVelocity_mmps = 1.0f;
        public float _axis02HomeFindSensorVelocity_mmps = 1.0f;
        public float _axis03HomeFindSensorVelocity_mmps = 1.0f;
        public float _axis04HomeFindSensorVelocity_mmps = 1.0f;
        public float _axis05HomeFindSensorVelocity_mmps = 1.0f;
        public float _axis06HomeFindSensorVelocity_mmps = 1.0f;
        public float _axis07HomeFindSensorVelocity_mmps = 1.0f;
        public float _axis08HomeFindSensorVelocity_mmps = 1.0f;
        public float _axis09HomeFindSensorVelocity_mmps = 1.0f;
        public float _axis10HomeFindSensorVelocity_mmps = 1.0f;
        public float _axis11HomeFindSensorVelocity_mmps = 1.0f;

        //HomeBackDistance
        public float _axis00HomeBackDistance_mm = 3.0f;
        public float _axis01HomeBackDistance_mm = 3.0f;
        public float _axis02HomeBackDistance_mm = 3.0f;
        public float _axis03HomeBackDistance_mm = 3.0f;
        public float _axis04HomeBackDistance_mm = 3.0f;
        public float _axis05HomeBackDistance_mm = 3.0f;
        public float _axis06HomeBackDistance_mm = 3.0f;
        public float _axis07HomeBackDistance_mm = 3.0f;
        public float _axis08HomeBackDistance_mm = 3.0f;
        public float _axis09HomeBackDistance_mm = 3.0f;
        public float _axis10HomeBackDistance_mm = 3.0f;
        public float _axis11HomeBackDistance_mm = 3.0f;

        //JogMinSpeed
        public float _axis00JogMinSpeed_mmps = 0.1f;
        public float _axis01JogMinSpeed_mmps = 0.1f;
        public float _axis02JogMinSpeed_mmps = 0.1f;
        public float _axis03JogMinSpeed_mmps = 0.1f;
        public float _axis04JogMinSpeed_mmps = 0.1f;
        public float _axis05JogMinSpeed_mmps = 0.1f;
        public float _axis06JogMinSpeed_mmps = 0.1f;
        public float _axis07JogMinSpeed_mmps = 0.1f;
        public float _axis08JogMinSpeed_mmps = 0.1f;
        public float _axis09JogMinSpeed_mmps = 0.1f;
        public float _axis10JogMinSpeed_mmps = 0.1f;
        public float _axis11JogMinSpeed_mmps = 0.1f;
        //JogMaxSpeed
        public float _axis00JogMaxSpeed_mmps = 100f;
        public float _axis01JogMaxSpeed_mmps = 100f;
        public float _axis02JogMaxSpeed_mmps = 100f;
        public float _axis03JogMaxSpeed_mmps = 100f;
        public float _axis04JogMaxSpeed_mmps = 100f;
        public float _axis05JogMaxSpeed_mmps = 100f;
        public float _axis06JogMaxSpeed_mmps = 100f;
        public float _axis07JogMaxSpeed_mmps = 100f;
        public float _axis08JogMaxSpeed_mmps = 100f;
        public float _axis09JogMaxSpeed_mmps = 100f;
        public float _axis10JogMaxSpeed_mmps = 100f;
        public float _axis11JogMaxSpeed_mmps = 100f;
        //InchMinDistance
        public float _axis00InchMinDistance_mm = 0.001f;
        public float _axis01InchMinDistance_mm = 0.001f;
        public float _axis02InchMinDistance_mm = 0.001f;
        public float _axis03InchMinDistance_mm = 0.001f;
        public float _axis04InchMinDistance_mm = 0.001f;
        public float _axis05InchMinDistance_mm = 0.001f;
        public float _axis06InchMinDistance_mm = 0.001f;
        public float _axis07InchMinDistance_mm = 0.001f;
        public float _axis08InchMinDistance_mm = 0.001f;
        public float _axis09InchMinDistance_mm = 0.001f;
        public float _axis10InchMinDistance_mm = 0.001f;
        public float _axis11InchMinDistance_mm = 0.001f;
        //InchMaxDistance
        public float _axis00InchMaxDistance_mm = 100f;
        public float _axis01InchMaxDistance_mm = 100f;
        public float _axis02InchMaxDistance_mm = 100f;
        public float _axis03InchMaxDistance_mm = 100f;
        public float _axis04InchMaxDistance_mm = 100f;
        public float _axis05InchMaxDistance_mm = 100f;
        public float _axis06InchMaxDistance_mm = 100f;
        public float _axis07InchMaxDistance_mm = 100f;
        public float _axis08InchMaxDistance_mm = 100f;
        public float _axis09InchMaxDistance_mm = 100f;
        public float _axis10InchMaxDistance_mm = 100f;
        public float _axis11InchMaxDistance_mm = 100f;

        //原点标志
        public bool _axis00HomedFlag = false;
        public bool _axis01HomedFlag = false;
        public bool _axis02HomedFlag = false;
        public bool _axis03HomedFlag = false;
        public bool _axis04HomedFlag = false;
        public bool _axis05HomedFlag = false;
        public bool _axis06HomedFlag = false;
        public bool _axis07HomedFlag = false;
        public bool _axis08HomedFlag = false;
        public bool _axis09HomedFlag = false;
        public bool _axis10HomedFlag = false;
        public bool _axis11HomedFlag = false;
        //错误代码
        public int _axis00ErrorNumber = 0;
        public int _axis01ErrorNumber = 0;
        public int _axis02ErrorNumber = 0;
        public int _axis03ErrorNumber = 0;
        public int _axis04ErrorNumber = 0;
        public int _axis05ErrorNumber = 0;
        public int _axis06ErrorNumber = 0;
        public int _axis07ErrorNumber = 0;
        public int _axis08ErrorNumber = 0;
        public int _axis09ErrorNumber = 0;
        public int _axis10ErrorNumber = 0;
        public int _axis11ErrorNumber = 0;
        //0:NA;1-900:点位号定位;901:Jogging;999:Homing;1000:实时位置数据定位;1001:减速停止
        public int _axis00Command = 0;
        public int _axis01Command = 0;
        public int _axis02Command = 0;
        public int _axis03Command = 0;
        public int _axis04Command = 0;
        public int _axis05Command = 0;
        public int _axis06Command = 0;
        public int _axis07Command = 0;
        public int _axis08Command = 0;
        public int _axis09Command = 0;
        public int _axis10Command = 0;
        public int _axis11Command = 0;
        //轴Command保存
        public int _axis00CommandSaved = 0;
        public int _axis01CommandSaved = 0;
        public int _axis02CommandSaved = 0;
        public int _axis03CommandSaved = 0;
        public int _axis04CommandSaved = 0;
        public int _axis05CommandSaved = 0;
        public int _axis06CommandSaved = 0;
        public int _axis07CommandSaved = 0;
        public int _axis08CommandSaved = 0;
        public int _axis09CommandSaved = 0;
        public int _axis10CommandSaved = 0;
        public int _axis11CommandSaved = 0;
        //0:NA;1-900:当前点位号;999:Homed;1000:实时位置数据定位完成
        public int _axis00CurrentPositionNumber = 0;
        public int _axis01CurrentPositionNumber = 0;
        public int _axis02CurrentPositionNumber = 0;
        public int _axis03CurrentPositionNumber = 0;
        public int _axis04CurrentPositionNumber = 0;
        public int _axis05CurrentPositionNumber = 0;
        public int _axis06CurrentPositionNumber = 0;
        public int _axis07CurrentPositionNumber = 0;
        public int _axis08CurrentPositionNumber = 0;
        public int _axis09CurrentPositionNumber = 0;
        public int _axis10CurrentPositionNumber = 0;
        public int _axis11CurrentPositionNumber = 0;
        //移动超时计数
        public int axis00MoveTimeOutCount = 0;
        public int axis01MoveTimeOutCount = 0;
        public int axis02MoveTimeOutCount = 0;
        public int axis03MoveTimeOutCount = 0;
        public int axis04MoveTimeOutCount = 0;
        public int axis05MoveTimeOutCount = 0;
        public int axis06MoveTimeOutCount = 0;
        public int axis07MoveTimeOutCount = 0;
        public int axis08MoveTimeOutCount = 0;
        public int axis09MoveTimeOutCount = 0;
        public int axis10MoveTimeOutCount = 0;
        public int axis11MoveTimeOutCount = 0;

        //轴Servo On Flag
        public bool _axis00ServoOnOffFlag = false;
        public bool _axis01ServoOnOffFlag = false;
        public bool _axis02ServoOnOffFlag = false;
        public bool _axis03ServoOnOffFlag = false;
        public bool _axis04ServoOnOffFlag = false;
        public bool _axis05ServoOnOffFlag = false;
        public bool _axis06ServoOnOffFlag = false;
        public bool _axis07ServoOnOffFlag = false;
        public bool _axis08ServoOnOffFlag = false;
        public bool _axis09ServoOnOffFlag = false;
        public bool _axis10ServoOnOffFlag = false;
        public bool _axis11ServoOnOffFlag = false;
        //轴运行状态
        public int _axis00CheckDoneStatus = 0;
        public int _axis01CheckDoneStatus = 0;
        public int _axis02CheckDoneStatus = 0;
        public int _axis03CheckDoneStatus = 0;
        public int _axis04CheckDoneStatus = 0;
        public int _axis05CheckDoneStatus = 0;
        public int _axis06CheckDoneStatus = 0;
        public int _axis07CheckDoneStatus = 0;
        public int _axis08CheckDoneStatus = 0;
        public int _axis09CheckDoneStatus = 0;
        public int _axis10CheckDoneStatus = 0;
        public int _axis11CheckDoneStatus = 0;
        //轴当前速度读取
        public float _axis00CurrentSpeed_mmps = 0;
        public float _axis01CurrentSpeed_mmps = 0;
        public float _axis02CurrentSpeed_mmps = 0;
        public float _axis03CurrentSpeed_mmps = 0;
        public float _axis04CurrentSpeed_mmps = 0;
        public float _axis05CurrentSpeed_mmps = 0;
        public float _axis06CurrentSpeed_mmps = 0;
        public float _axis07CurrentSpeed_mmps = 0;
        public float _axis08CurrentSpeed_mmps = 0;
        public float _axis09CurrentSpeed_mmps = 0;
        public float _axis10CurrentSpeed_mmps = 0;
        public float _axis11CurrentSpeed_mmps = 0;

        //temp
        private int axis00StartVelocity_Temp_pps;
        private int axis00MaxVelocity_Temp_pps;
        private int axis01StartVelocity_Temp_pps;
        private int axis01MaxVelocity_Temp_pps;
        private int axis02StartVelocity_Temp_pps;
        private int axis02MaxVelocity_Temp_pps;
        private int axis03StartVelocity_Temp_pps;
        private int axis03MaxVelocity_Temp_pps;
        private int axis04StartVelocity_Temp_pps;
        private int axis04MaxVelocity_Temp_pps;
        private int axis05StartVelocity_Temp_pps;
        private int axis05MaxVelocity_Temp_pps;
        private int axis06StartVelocity_Temp_pps;
        private int axis06MaxVelocity_Temp_pps;
        private int axis07StartVelocity_Temp_pps;
        private int axis07MaxVelocity_Temp_pps;
        private int axis08StartVelocity_Temp_pps;
        private int axis08MaxVelocity_Temp_pps;
        private int axis09StartVelocity_Temp_pps;
        private int axis09MaxVelocity_Temp_pps;
        private int axis10StartVelocity_Temp_pps;
        private int axis10MaxVelocity_Temp_pps;
        private int axis11StartVelocity_Temp_pps;
        private int axis11MaxVelocity_Temp_pps;

        public bool[][] _inPutStatus;
        public bool[][] _outPutStatus;
        //private int[][] axisIOStatus;
        public bool _axis00ELMinus, _axis00ELPlus, _axis00ORG, _axis00STP, _axis00STA, _axis00SDMinus, _axis00SDPlus, _axis00Reserved;
        public bool _axis01ELMinus, _axis01ELPlus, _axis01ORG, _axis01STP, _axis01STA, _axis01SDMinus, _axis01SDPlus, _axis01Reserved;
        public bool _axis02ELMinus, _axis02ELPlus, _axis02ORG, _axis02STP, _axis02STA, _axis02SDMinus, _axis02SDPlus, _axis02Reserved;
        public bool _axis03ELMinus, _axis03ELPlus, _axis03ORG, _axis03STP, _axis03STA, _axis03SDMinus, _axis03SDPlus, _axis03Reserved;
        public bool _axis04ELMinus, _axis04ELPlus, _axis04ORG, _axis04STP, _axis04STA, _axis04SDMinus, _axis04SDPlus, _axis04Reserved;
        public bool _axis05ELMinus, _axis05ELPlus, _axis05ORG, _axis05STP, _axis05STA, _axis05SDMinus, _axis05SDPlus, _axis05Reserved;
        public bool _axis06ELMinus, _axis06ELPlus, _axis06ORG, _axis06STP, _axis06STA, _axis06SDMinus, _axis06SDPlus, _axis06Reserved;
        public bool _axis07ELMinus, _axis07ELPlus, _axis07ORG, _axis07STP, _axis07STA, _axis07SDMinus, _axis07SDPlus, _axis07Reserved;
        public bool _axis08ELMinus, _axis08ELPlus, _axis08ORG, _axis08STP, _axis08STA, _axis08SDMinus, _axis08SDPlus, _axis08Reserved;
        public bool _axis09ELMinus, _axis09ELPlus, _axis09ORG, _axis09STP, _axis09STA, _axis09SDMinus, _axis09SDPlus, _axis09Reserved;
        public bool _axis10ELMinus, _axis10ELPlus, _axis10ORG, _axis10STP, _axis10STA, _axis10SDMinus, _axis10SDPlus, _axis10Reserved;
        public bool _axis11ELMinus, _axis11ELPlus, _axis11ORG, _axis11STP, _axis11STA, _axis11SDMinus, _axis11SDPlus, _axis11Reserved;

        public bool _axis00HomeStartFlag = false;
        public bool _axis01HomeStartFlag = false;
        public bool _axis02HomeStartFlag = false;
        public bool _axis03HomeStartFlag = false;
        public bool _axis04HomeStartFlag = false;
        public bool _axis05HomeStartFlag = false;
        public bool _axis06HomeStartFlag = false;
        public bool _axis07HomeStartFlag = false;
        public bool _axis08HomeStartFlag = false;
        public bool _axis09HomeStartFlag = false;
        public bool _axis10HomeStartFlag = false;
        public bool _axis11HomeStartFlag = false;

        public bool _axis00MoveStartFlag = false;
        public bool _axis01MoveStartFlag = false;
        public bool _axis02MoveStartFlag = false;
        public bool _axis03MoveStartFlag = false;
        public bool _axis04MoveStartFlag = false;
        public bool _axis05MoveStartFlag = false;
        public bool _axis06MoveStartFlag = false;
        public bool _axis07MoveStartFlag = false;
        public bool _axis08MoveStartFlag = false;
        public bool _axis09MoveStartFlag = false;
        public bool _axis10MoveStartFlag = false;
        public bool _axis11MoveStartFlag = false;

        public bool _axis00ServoAlarmClearFlag = false;//记录伺服驱动器报警清除输出的状态:false/true(OFF/ON)
        public bool _axis01ServoAlarmClearFlag = false;
        public bool _axis02ServoAlarmClearFlag = false;
        public bool _axis03ServoAlarmClearFlag = false;
        public bool _axis04ServoAlarmClearFlag = false;
        public bool _axis05ServoAlarmClearFlag = false;
        public bool _axis06ServoAlarmClearFlag = false;
        public bool _axis07ServoAlarmClearFlag = false;
        public bool _axis08ServoAlarmClearFlag = false;
        public bool _axis09ServoAlarmClearFlag = false;
        public bool _axis10ServoAlarmClearFlag = false;
        public bool _axis11ServoAlarmClearFlag = false;

        private ushort[] Devhandle;//运动控制器句柄
        private ushort[] axishandle;//轴句柄

        private Thread td_axisStatusUpdata;//轴状态更新线程

        //ADD 20221015 轴暂停时,AxisAction函数的参数保存
        private int _axis00_axisCommand_SaveForPause;
        private int _axis00_axisNumber_SaveForPause;
        private float _axis00_startVelocity_SaveForPause;
        private float _axis00_maxVelocity_SaveForPause;
        private float _axis00_stopVelocity_SaveForPause;
        private double _axis00_accTime_SaveForPause;
        private double _axis00_decTime_SaveForPause;
        private float _axis00_targetPosition_SaveForPause;
        private bool _axis00_jogDirection_SaveForPause;
        private bool _axis00_inch_SaveForPause;
        private float _axis00_deviationValue_SaveForPause;
        private bool _axis00_useSModeMoveFlag_SaveForPause;
        private double _axis00_sMoveTime_SaveForPause;
        public bool _axis00PauseRequestFlag = false;
        public bool _axis00ResumeRequestFlag = false;
        public bool _axis00PausedFlag = false;

        private int _axis01_axisCommand_SaveForPause;
        private int _axis01_axisNumber_SaveForPause;
        private float _axis01_startVelocity_SaveForPause;
        private float _axis01_maxVelocity_SaveForPause;
        private float _axis01_stopVelocity_SaveForPause;
        private double _axis01_accTime_SaveForPause;
        private double _axis01_decTime_SaveForPause;
        private float _axis01_targetPosition_SaveForPause;
        private bool _axis01_jogDirection_SaveForPause;
        private bool _axis01_inch_SaveForPause;
        private float _axis01_deviationValue_SaveForPause;
        private bool _axis01_useSModeMoveFlag_SaveForPause;
        private double _axis01_sMoveTime_SaveForPause;
        public bool _axis01PauseRequestFlag = false;
        public bool _axis01ResumeRequestFlag = false;
        public bool _axis01PausedFlag = false;

        private int _axis02_axisCommand_SaveForPause;
        private int _axis02_axisNumber_SaveForPause;
        private float _axis02_startVelocity_SaveForPause;
        private float _axis02_maxVelocity_SaveForPause;
        private float _axis02_stopVelocity_SaveForPause;
        private double _axis02_accTime_SaveForPause;
        private double _axis02_decTime_SaveForPause;
        private float _axis02_targetPosition_SaveForPause;
        private bool _axis02_jogDirection_SaveForPause;
        private bool _axis02_inch_SaveForPause;
        private float _axis02_deviationValue_SaveForPause;
        private bool _axis02_useSModeMoveFlag_SaveForPause;
        private double _axis02_sMoveTime_SaveForPause;
        public bool _axis02PauseRequestFlag = false;
        public bool _axis02ResumeRequestFlag = false;
        public bool _axis02PausedFlag = false;

        private int _axis03_axisCommand_SaveForPause;
        private int _axis03_axisNumber_SaveForPause;
        private float _axis03_startVelocity_SaveForPause;
        private float _axis03_maxVelocity_SaveForPause;
        private float _axis03_stopVelocity_SaveForPause;
        private double _axis03_accTime_SaveForPause;
        private double _axis03_decTime_SaveForPause;
        private float _axis03_targetPosition_SaveForPause;
        private bool _axis03_jogDirection_SaveForPause;
        private bool _axis03_inch_SaveForPause;
        private float _axis03_deviationValue_SaveForPause;
        private bool _axis03_useSModeMoveFlag_SaveForPause;
        private double _axis03_sMoveTime_SaveForPause;
        public bool _axis03PauseRequestFlag = false;
        public bool _axis03ResumeRequestFlag = false;
        public bool _axis03PausedFlag = false;

        private int _axis04_axisCommand_SaveForPause;
        private int _axis04_axisNumber_SaveForPause;
        private float _axis04_startVelocity_SaveForPause;
        private float _axis04_maxVelocity_SaveForPause;
        private float _axis04_stopVelocity_SaveForPause;
        private double _axis04_accTime_SaveForPause;
        private double _axis04_decTime_SaveForPause;
        private float _axis04_targetPosition_SaveForPause;
        private bool _axis04_jogDirection_SaveForPause;
        private bool _axis04_inch_SaveForPause;
        private float _axis04_deviationValue_SaveForPause;
        private bool _axis04_useSModeMoveFlag_SaveForPause;
        private double _axis04_sMoveTime_SaveForPause;
        public bool _axis04PauseRequestFlag = false;
        public bool _axis04ResumeRequestFlag = false;
        public bool _axis04PausedFlag = false;

        private int _axis05_axisCommand_SaveForPause;
        private int _axis05_axisNumber_SaveForPause;
        private float _axis05_startVelocity_SaveForPause;
        private float _axis05_maxVelocity_SaveForPause;
        private float _axis05_stopVelocity_SaveForPause;
        private double _axis05_accTime_SaveForPause;
        private double _axis05_decTime_SaveForPause;
        private float _axis05_targetPosition_SaveForPause;
        private bool _axis05_jogDirection_SaveForPause;
        private bool _axis05_inch_SaveForPause;
        private float _axis05_deviationValue_SaveForPause;
        private bool _axis05_useSModeMoveFlag_SaveForPause;
        private double _axis05_sMoveTime_SaveForPause;
        public bool _axis05PauseRequestFlag = false;
        public bool _axis05ResumeRequestFlag = false;
        public bool _axis05PausedFlag = false;

        private int _axis06_axisCommand_SaveForPause;
        private int _axis06_axisNumber_SaveForPause;
        private float _axis06_startVelocity_SaveForPause;
        private float _axis06_maxVelocity_SaveForPause;
        private float _axis06_stopVelocity_SaveForPause;
        private double _axis06_accTime_SaveForPause;
        private double _axis06_decTime_SaveForPause;
        private float _axis06_targetPosition_SaveForPause;
        private bool _axis06_jogDirection_SaveForPause;
        private bool _axis06_inch_SaveForPause;
        private float _axis06_deviationValue_SaveForPause;
        private bool _axis06_useSModeMoveFlag_SaveForPause;
        private double _axis06_sMoveTime_SaveForPause;
        public bool _axis06PauseRequestFlag = false;
        public bool _axis06ResumeRequestFlag = false;
        public bool _axis06PausedFlag = false;

        private int _axis07_axisCommand_SaveForPause;
        private int _axis07_axisNumber_SaveForPause;
        private float _axis07_startVelocity_SaveForPause;
        private float _axis07_maxVelocity_SaveForPause;
        private float _axis07_stopVelocity_SaveForPause;
        private double _axis07_accTime_SaveForPause;
        private double _axis07_decTime_SaveForPause;
        private float _axis07_targetPosition_SaveForPause;
        private bool _axis07_jogDirection_SaveForPause;
        private bool _axis07_inch_SaveForPause;
        private float _axis07_deviationValue_SaveForPause;
        private bool _axis07_useSModeMoveFlag_SaveForPause;
        private double _axis07_sMoveTime_SaveForPause;
        public bool _axis07PauseRequestFlag = false;
        public bool _axis07ResumeRequestFlag = false;
        public bool _axis07PausedFlag = false;

        private int _axis08_axisCommand_SaveForPause;
        private int _axis08_axisNumber_SaveForPause;
        private float _axis08_startVelocity_SaveForPause;
        private float _axis08_maxVelocity_SaveForPause;
        private float _axis08_stopVelocity_SaveForPause;
        private double _axis08_accTime_SaveForPause;
        private double _axis08_decTime_SaveForPause;
        private float _axis08_targetPosition_SaveForPause;
        private bool _axis08_jogDirection_SaveForPause;
        private bool _axis08_inch_SaveForPause;
        private float _axis08_deviationValue_SaveForPause;
        private bool _axis08_useSModeMoveFlag_SaveForPause;
        private double _axis08_sMoveTime_SaveForPause;
        public bool _axis08PauseRequestFlag = false;
        public bool _axis08ResumeRequestFlag = false;
        public bool _axis08PausedFlag = false;

        private int _axis09_axisCommand_SaveForPause;
        private int _axis09_axisNumber_SaveForPause;
        private float _axis09_startVelocity_SaveForPause;
        private float _axis09_maxVelocity_SaveForPause;
        private float _axis09_stopVelocity_SaveForPause;
        private double _axis09_accTime_SaveForPause;
        private double _axis09_decTime_SaveForPause;
        private float _axis09_targetPosition_SaveForPause;
        private bool _axis09_jogDirection_SaveForPause;
        private bool _axis09_inch_SaveForPause;
        private float _axis09_deviationValue_SaveForPause;
        private bool _axis09_useSModeMoveFlag_SaveForPause;
        private double _axis09_sMoveTime_SaveForPause;
        public bool _axis09PauseRequestFlag = false;
        public bool _axis09ResumeRequestFlag = false;
        public bool _axis09PausedFlag = false;

        private int _axis10_axisCommand_SaveForPause;
        private int _axis10_axisNumber_SaveForPause;
        private float _axis10_startVelocity_SaveForPause;
        private float _axis10_maxVelocity_SaveForPause;
        private float _axis10_stopVelocity_SaveForPause;
        private double _axis10_accTime_SaveForPause;
        private double _axis10_decTime_SaveForPause;
        private float _axis10_targetPosition_SaveForPause;
        private bool _axis10_jogDirection_SaveForPause;
        private bool _axis10_inch_SaveForPause;
        private float _axis10_deviationValue_SaveForPause;
        private bool _axis10_useSModeMoveFlag_SaveForPause;
        private double _axis10_sMoveTime_SaveForPause;
        public bool _axis10PauseRequestFlag = false;
        public bool _axis10ResumeRequestFlag = false;
        public bool _axis10PausedFlag = false;

        private int _axis11_axisCommand_SaveForPause;
        private int _axis11_axisNumber_SaveForPause;
        private float _axis11_startVelocity_SaveForPause;
        private float _axis11_maxVelocity_SaveForPause;
        private float _axis11_stopVelocity_SaveForPause;
        private double _axis11_accTime_SaveForPause;
        private double _axis11_decTime_SaveForPause;
        private float _axis11_targetPosition_SaveForPause;
        private bool _axis11_jogDirection_SaveForPause;
        private bool _axis11_inch_SaveForPause;
        private float _axis11_deviationValue_SaveForPause;
        private bool _axis11_useSModeMoveFlag_SaveForPause;
        private double _axis11_sMoveTime_SaveForPause;
        public bool _axis11PauseRequestFlag = false;
        public bool _axis11ResumeRequestFlag = false;
        public bool _axis11PausedFlag = false;

        #endregion

        #region 构造函数
        public GaoChuanMotionCardGCN400AS()
        {
            Initial();
        }
        #endregion

        #region Initial:卡及轴初始化
        /// <summary>
        /// Initial:卡及轴初始化
        /// </summary>
        public int Initial()
        {
            _axisQuantity = _cardQuantity * _axisQuantityPerCard_4;
            _maxIOQuantity = _cardQuantity * _motionCardIOMaxQuantity_320;

            //int n = LTDMC.dmc_board_init();
            ushort n = 0;
            byte[] bt = new byte[128];
            NMCLib20.NMC_DevSearch(NMCLib20.TSearchMode.Ethernet, ref n, bt);//搜索已连接的运动控制器
            if (n == 0)//没有搜索到已连接的运动控制器
                return -1;
            if (n != _cardQuantity)//搜索到已连接的运动控制器的数量与设定数量不符
                return -2;
            Devhandle = new ushort[_cardQuantity];//运动控制器句柄
            axishandle = new ushort[_axisQuantityPerCard_4 * _cardQuantity];//轴句柄
            for (short i = 0; i < _cardQuantity; i++)
            {
                if (NMCLib20.NMC_DevOpenByID("CARD" + (i + 1).ToString(), ref Devhandle[i]) != 0)//根据ID打开运动控制器
                    return (i + 10) * -1;//如果第0个运动控制器打开失败则返回-10
                NMCLib20.NMC_DevReset(Devhandle[i]);//复位控制器
                NMCLib20.NMC_IOModuleSetEn(Devhandle[i], 2, 1);//扩展模块使能
            }
            for (ushort i = 0; i < _cardQuantity; i++)
            {
                for (short k = 0; k < _axisQuantityPerCard_4; k++)
                {
                    if (NMCLib20.NMC_MtOpen(Devhandle[i], k, ref axishandle[i * _axisQuantityPerCard_4 + k]) != 0)//打开轴
                        return ((i + 1) * 100 + k) * -1;//如果第0个运动控制器第0轴打开失败则返回-100,第1个运动控制器第0轴打开失败则返回-200
                }
            }

            if (n == _cardQuantity)
            {
                _inPutStatus = new bool[_cardQuantity][];//输入,按照卡的数量分开
                _inPutStatus[0] = new bool[_motionCardIOMaxQuantity_320];
                //_inPutStatus[1] = new bool[_motionCardIOMaxQuantity_320];
                _outPutStatus = new bool[_cardQuantity][];//输出,按照卡的数量分开
                _outPutStatus[0] = new bool[_motionCardIOMaxQuantity_320];
                //_outPutStatus[1] = new bool[_motionCardIOMaxQuantity_320];

                for (int i = 0; i < _cardQuantity; i++)
                {
                    for (int j = 0; j < _motionCardIOMaxQuantity_320; j++)
                    {
                        _inPutStatus[i][j] = false;
                        _outPutStatus[i][j] = false;
                    }
                }

                int L0 = -1000;
                int L1 = -2000;
                int L2 = -3000;
                int L3 = -4000;
                int L4 = -5000;
                int L5 = -6000;
                int L6 = -7000;
                if (_cardQuantity >= 1)
                {
                    //编码器模式配置
                    NMCLib20.NMC_SetEncMode(Devhandle[0], 0, 2304);
                    NMCLib20.NMC_SetEncMode(Devhandle[0], 1, 2304);
                    NMCLib20.NMC_SetEncMode(Devhandle[0], 2, 2304);
                    NMCLib20.NMC_SetEncMode(Devhandle[0], 3, 2304);


                    //伺服报警配置:轴句柄;伺服报警触是否有效，1 为有效，0 为无效;伺服报警触发电平，1 为高电平触发，0为低电平触发
                    if (NMCLib20.NMC_MtSetAlarmCfg(axishandle[0], 1, 1) != 0)
                        return L0 - 0;
                    if (NMCLib20.NMC_MtSetAlarmCfg(axishandle[1], 1, 1) != 0)
                        return L0 - 1;
                    if (NMCLib20.NMC_MtSetAlarmCfg(axishandle[2], 1, 1) != 0)
                        return L0 - 2;
                    if (NMCLib20.NMC_MtSetAlarmCfg(axishandle[3], 1, 1) != 0)
                        return L0 - 3;

                    //限位配置:轴句柄;正向限位是否有效，1 为有效，0 为无效;负向限位是否有效，1 为有效，0 为无效;正向限位触发电平，1 为高电平触发，0为低电平触发;正向限位触发电平，1 为高电平触发，0为低电平触发
                    if (NMCLib20.NMC_MtSetLmtCfg(axishandle[0], 1, 1, 1, 1) != 0)
                        return L1 - 0;
                    if (NMCLib20.NMC_MtSetLmtCfg(axishandle[1], 1, 1, 1, 1) != 0)
                        return L1 - 1;
                    if (NMCLib20.NMC_MtSetLmtCfg(axishandle[2], 1, 1, 1, 1) != 0)
                        return L1 - 2;
                    if (NMCLib20.NMC_MtSetLmtCfg(axishandle[3], 1, 1, 1, 1) != 0)
                        return L1 - 3;

                    //软限位配置:轴句柄;软件限位是否有效，1 为有效，0 为无效
                    if (NMCLib20.NMC_MtSwLmtOnOff(axishandle[0], 0) != 0)
                        return L2 - 0;
                    if (NMCLib20.NMC_MtSwLmtOnOff(axishandle[1], 0) != 0)
                        return L2 - 1;
                    if (NMCLib20.NMC_MtSwLmtOnOff(axishandle[2], 0) != 0)
                        return L2 - 2;
                    if (NMCLib20.NMC_MtSwLmtOnOff(axishandle[3], 0) != 0)
                        return L2 - 3;

                    //脉冲输出配置:轴句柄;输出取反，0 为不取反，1 为取反;输出模式，0 为脉冲+方向，1 为正负脉冲
                    if (NMCLib20.NMC_MtSetStepMode(axishandle[0], 1, 1) != 0)
                        return L3 - 0;
                    if (NMCLib20.NMC_MtSetStepMode(axishandle[1], 0, 1) != 0)
                        return L3 - 1;
                    if (NMCLib20.NMC_MtSetStepMode(axishandle[2], 0, 1) != 0)
                        return L3 - 2;
                    if (NMCLib20.NMC_MtSetStepMode(axishandle[3], 0, 1) != 0)
                        return L3 - 3;

                    //设置单轴急停DI:轴句柄;通用输入序号,取值范围[0,n]，设置为-1，则表示取消急停DI;触发电平,0：低电平,1：高电平
                    if (NMCLib20.NMC_MtSetEstopDI(axishandle[0], -1, 1) != 0)
                        return L4 - 0;
                    if (NMCLib20.NMC_MtSetEstopDI(axishandle[1], -1, 1) != 0)
                        return L4 - 1;
                    if (NMCLib20.NMC_MtSetEstopDI(axishandle[2], -1, 1) != 0)
                        return L4 - 2;
                    if (NMCLib20.NMC_MtSetEstopDI(axishandle[3], -1, 1) != 0)
                        return L4 - 3;

                    //NMCLib20.THomeSetting homepara;//回零参数
                    //homepara.mode = 1;//模式:1.原点开关触发;2.限位开关触发;3.(单Z相)编码器的Index信号触发;4.(原点+Z相)原点触发后,正向寻找到Index信号触发;5.(原点-Z相)原点触发后,反向寻找到Index信号触发;6.(原点-Z相)限位触发后,反向寻找到Index信号触发
                    //homepara.dir = 0;//搜寻零点方向（必须）0:负向;1:正向,其它值无意义
                    //homepara.offset = 0;//原点偏移（必须）
                    //homepara.scan1stVel = 10;//基本搜寻速度（必须）
                    //homepara.scan2ndVel = 10;//低速（两次搜寻时需要）
                    //homepara.acc = 0.1;//加速度
                    //homepara.reScanEn = 1;//是否两次搜寻零点（可选,不用时设为0）
                    //homepara.homeEdge = 1;//原点触发沿（默认0下降沿触发）,与原点触发电平有关
                    //homepara.lmtEdge = 0;//限位触发沿（默认0下降沿触发）,与限位触发电平有关
                    //homepara.zEdge = 1;//限位触发沿（默认0下降沿触发）,与Z向触发电平有关
                    //homepara.iniRetPos = 0;//起始时反向离开当前位置的运动距离（可选,不用时设为0）
                    //homepara.retSwOffset = 0;//反向运动时离开开关距离（可选,不用时设为0）
                    //homepara.safeLen = 0;//安全距离,回零时最远搜寻距离（可选,0表示不限制距离）
                    //homepara.usePreSetPtpPara = 0;//1表示需要在回零前,自己设置回零运动（点到点）的参数.为0时,回零运动的减加速度默认等于acc,起跳速度、终点速度、平滑系数默认为0
                    //homepara.reserved0 = 0;//保留
                    //homepara.reserved1 = 0;//保留
                    //homepara.reserved2 = 0;//保留
                    ////设置回零参数:轴句柄;回零参数结构
                    //if (NMCLib20.NMC_MtSetHomePara(axishandle[0], ref homepara) != 0)
                    //    return L5 - 0;
                    //if (NMCLib20.NMC_MtSetHomePara(axishandle[1], ref homepara) != 0)
                    //    return L5 - 1;
                    //if (NMCLib20.NMC_MtSetHomePara(axishandle[2], ref homepara) != 0)
                    //    return L5 - 2;
                    //if (NMCLib20.NMC_MtSetHomePara(axishandle[3], ref homepara) != 0)
                    //    return L5 - 3;

                    //清除轴错误状态
                    if (NMCLib20.NMC_MtClrError(axishandle[0]) != 0)
                        return L6 - 0;
                    if (NMCLib20.NMC_MtClrError(axishandle[1]) != 0)
                        return L6 - 1;
                    if (NMCLib20.NMC_MtClrError(axishandle[2]) != 0)
                        return L6 - 2;
                    if (NMCLib20.NMC_MtClrError(axishandle[3]) != 0)
                        return L6 - 3;

                    axisImmediateStop(0);
                    axisImmediateStop(1);
                    axisImmediateStop(2);
                    axisImmediateStop(3);

                    _axis00Command = 0;
                    _axis01Command = 0;
                    _axis02Command = 0;
                    _axis03Command = 0;
                    _axis00CommandSaved = 0;
                    _axis01CommandSaved = 0;
                    _axis02CommandSaved = 0;
                    _axis03CommandSaved = 0;
                    _axis00HomedFlag = false;
                    _axis01HomedFlag = false;
                    _axis02HomedFlag = false;
                    _axis03HomedFlag = false;
                    _axis00CurrentPositionNumber = 0;
                    _axis01CurrentPositionNumber = 0;
                    _axis02CurrentPositionNumber = 0;
                    _axis03CurrentPositionNumber = 0;
                    axis00MoveTimeOutCount = 0;
                    axis01MoveTimeOutCount = 0;
                    axis02MoveTimeOutCount = 0;
                    axis03MoveTimeOutCount = 0;
                    _axis00ErrorNumber = 0;
                    _axis01ErrorNumber = 0;
                    _axis02ErrorNumber = 0;
                    _axis03ErrorNumber = 0;
                    //SERVO ON
                    NMCLib20.NMC_MtSetSvOn(axishandle[0]);
                    NMCLib20.NMC_MtSetSvOn(axishandle[1]);
                    NMCLib20.NMC_MtSetSvOn(axishandle[2]);
                    NMCLib20.NMC_MtSetSvOn(axishandle[3]);

                    //ADD 202210151953 Pause
                    _axis00PauseRequestFlag = false;
                    _axis00ResumeRequestFlag = false;
                    _axis01PauseRequestFlag = false;
                    _axis01ResumeRequestFlag = false;
                    _axis02PauseRequestFlag = false;
                    _axis02ResumeRequestFlag = false;
                    _axis03PauseRequestFlag = false;
                    _axis03ResumeRequestFlag = false;
                }
                if (_cardQuantity >= 2)
                {
                    L0 = L0 - 100;
                    L1 = L1 - 100;
                    L2 = L2 - 100;
                    L3 = L3 - 100;
                    L4 = L4 - 100;
                    L5 = L5 - 100;
                    L6 = L6 - 100;

                    //编码器模式配置
                    NMCLib20.NMC_SetEncMode(Devhandle[1], 0, 2304);
                    NMCLib20.NMC_SetEncMode(Devhandle[1], 1, 2304);
                    NMCLib20.NMC_SetEncMode(Devhandle[1], 2, 2304);
                    NMCLib20.NMC_SetEncMode(Devhandle[1], 3, 2304);

                    //伺服报警配置:轴句柄;伺服报警触是否有效，1 为有效，0 为无效;伺服报警触发电平，1 为高电平触发，0为低电平触发
                    if (NMCLib20.NMC_MtSetAlarmCfg(axishandle[4], 1, 1) != 0)
                        return L0 - 0;
                    if (NMCLib20.NMC_MtSetAlarmCfg(axishandle[5], 1, 1) != 0)
                        return L0 - 1;
                    if (NMCLib20.NMC_MtSetAlarmCfg(axishandle[6], 1, 1) != 0)
                        return L0 - 2;
                    if (NMCLib20.NMC_MtSetAlarmCfg(axishandle[7], 1, 1) != 0)
                        return L0 - 3;

                    //限位配置:轴句柄;正向限位是否有效，1 为有效，0 为无效;负向限位是否有效，1 为有效，0 为无效;正向限位触发电平，1 为高电平触发，0为低电平触发;正向限位触发电平，1 为高电平触发，0为低电平触发
                    if (NMCLib20.NMC_MtSetLmtCfg(axishandle[4], 1, 1, 1, 1) != 0)
                        return L1 - 0;
                    if (NMCLib20.NMC_MtSetLmtCfg(axishandle[5], 1, 1, 1, 1) != 0)
                        return L1 - 1;
                    if (NMCLib20.NMC_MtSetLmtCfg(axishandle[6], 1, 1, 1, 1) != 0)
                        return L1 - 2;
                    if (NMCLib20.NMC_MtSetLmtCfg(axishandle[7], 1, 1, 1, 1) != 0)
                        return L1 - 3;

                    //软限位配置:轴句柄;软件限位是否有效，1 为有效，0 为无效
                    if (NMCLib20.NMC_MtSwLmtOnOff(axishandle[4], 0) != 0)
                        return L2 - 0;
                    if (NMCLib20.NMC_MtSwLmtOnOff(axishandle[5], 0) != 0)
                        return L2 - 1;
                    if (NMCLib20.NMC_MtSwLmtOnOff(axishandle[6], 0) != 0)
                        return L2 - 2;
                    if (NMCLib20.NMC_MtSwLmtOnOff(axishandle[7], 0) != 0)
                        return L2 - 3;

                    //脉冲输出配置:轴句柄;输出取反，0 为不取反，1 为取反;输出模式，0 为脉冲+方向，1 为正负脉冲
                    if (NMCLib20.NMC_MtSetStepMode(axishandle[4], 0, 1) != 0)
                        return L3 - 0;
                    if (NMCLib20.NMC_MtSetStepMode(axishandle[5], 0, 1) != 0)
                        return L3 - 1;
                    if (NMCLib20.NMC_MtSetStepMode(axishandle[6], 0, 1) != 0)
                        return L3 - 2;
                    if (NMCLib20.NMC_MtSetStepMode(axishandle[7], 0, 1) != 0)
                        return L3 - 3;

                    //设置单轴急停DI:轴句柄;通用输入序号,取值范围[0,n]，设置为-1，则表示取消急停DI;触发电平,0：低电平,1：高电平
                    if (NMCLib20.NMC_MtSetEstopDI(axishandle[4], -1, 1) != 0)
                        return L4 - 0;
                    if (NMCLib20.NMC_MtSetEstopDI(axishandle[5], -1, 1) != 0)
                        return L4 - 1;
                    if (NMCLib20.NMC_MtSetEstopDI(axishandle[6], -1, 1) != 0)
                        return L4 - 2;
                    if (NMCLib20.NMC_MtSetEstopDI(axishandle[7], -1, 1) != 0)
                        return L4 - 3;

                    //NMCLib20.THomeSetting homepara;//回零参数
                    //homepara.mode = 1;//模式:1.原点开关触发;2.限位开关触发;3.(单Z相)编码器的Index信号触发;4.(原点+Z相)原点触发后,正向寻找到Index信号触发;5.(原点-Z相)原点触发后,反向寻找到Index信号触发;6.(原点-Z相)限位触发后,反向寻找到Index信号触发
                    //homepara.dir = 0;//搜寻零点方向（必须）0:负向;1:正向,其它值无意义
                    //homepara.offset = 0;//原点偏移（必须）
                    //homepara.scan1stVel = 10;//基本搜寻速度（必须）
                    //homepara.scan2ndVel = 10;//低速（两次搜寻时需要）
                    //homepara.acc = 0.1;//加速度
                    //homepara.reScanEn = 1;//是否两次搜寻零点（可选,不用时设为0）
                    //homepara.homeEdge = 1;//原点触发沿（默认0下降沿触发）,与原点触发电平有关
                    //homepara.lmtEdge = 0;//限位触发沿（默认0下降沿触发）,与限位触发电平有关
                    //homepara.zEdge = 1;//限位触发沿（默认0下降沿触发）,与Z向触发电平有关
                    //homepara.iniRetPos = 0;//起始时反向离开当前位置的运动距离（可选,不用时设为0）
                    //homepara.retSwOffset = 0;//反向运动时离开开关距离（可选,不用时设为0）
                    //homepara.safeLen = 0;//安全距离,回零时最远搜寻距离（可选,0表示不限制距离）
                    //homepara.usePreSetPtpPara = 0;//1表示需要在回零前,自己设置回零运动（点到点）的参数.为0时,回零运动的减加速度默认等于acc,起跳速度、终点速度、平滑系数默认为0
                    //homepara.reserved0 = 0;//保留
                    //homepara.reserved1 = 0;//保留
                    //homepara.reserved2 = 0;//保留
                    ////设置回零参数:轴句柄;回零参数结构
                    //if (NMCLib20.NMC_MtSetHomePara(axishandle[4], ref homepara) != 0)
                    //    return L5 - 0;
                    //if (NMCLib20.NMC_MtSetHomePara(axishandle[5], ref homepara) != 0)
                    //    return L5 - 1;
                    //if (NMCLib20.NMC_MtSetHomePara(axishandle[6], ref homepara) != 0)
                    //    return L5 - 2;
                    //if (NMCLib20.NMC_MtSetHomePara(axishandle[7], ref homepara) != 0)
                    //    return L5 - 3;

                    //清除轴错误状态
                    if (NMCLib20.NMC_MtClrError(axishandle[4]) != 0)
                        return L6 - 0;
                    if (NMCLib20.NMC_MtClrError(axishandle[5]) != 0)
                        return L6 - 1;
                    if (NMCLib20.NMC_MtClrError(axishandle[6]) != 0)
                        return L6 - 2;
                    if (NMCLib20.NMC_MtClrError(axishandle[7]) != 0)
                        return L6 - 3;

                    axisImmediateStop(4);
                    axisImmediateStop(5);
                    axisImmediateStop(6);
                    axisImmediateStop(7);

                    _axis04Command = 0;
                    _axis05Command = 0;
                    _axis06Command = 0;
                    _axis07Command = 0;
                    _axis04CommandSaved = 0;
                    _axis05CommandSaved = 0;
                    _axis06CommandSaved = 0;
                    _axis07CommandSaved = 0;
                    _axis04HomedFlag = false;
                    _axis05HomedFlag = false;
                    _axis06HomedFlag = false;
                    _axis07HomedFlag = false;
                    _axis04CurrentPositionNumber = 0;
                    _axis05CurrentPositionNumber = 0;
                    _axis06CurrentPositionNumber = 0;
                    _axis07CurrentPositionNumber = 0;
                    axis04MoveTimeOutCount = 0;
                    axis05MoveTimeOutCount = 0;
                    axis06MoveTimeOutCount = 0;
                    axis07MoveTimeOutCount = 0;
                    _axis04ErrorNumber = 0;
                    _axis05ErrorNumber = 0;
                    _axis06ErrorNumber = 0;
                    _axis07ErrorNumber = 0;
                    //SERVO ON
                    NMCLib20.NMC_MtSetSvOn(axishandle[4]);
                    NMCLib20.NMC_MtSetSvOn(axishandle[5]);
                    NMCLib20.NMC_MtSetSvOn(axishandle[6]);
                    NMCLib20.NMC_MtSetSvOn(axishandle[7]);

                    //ADD 202210151953 Pause
                    _axis04PauseRequestFlag = false;
                    _axis04ResumeRequestFlag = false;
                    _axis05PauseRequestFlag = false;
                    _axis05ResumeRequestFlag = false;
                    _axis06PauseRequestFlag = false;
                    _axis06ResumeRequestFlag = false;
                    _axis07PauseRequestFlag = false;
                    _axis07ResumeRequestFlag = false;
                }
                if (_cardQuantity >= 3)
                {
                    L0 = L0 - 100;
                    L1 = L1 - 100;
                    L2 = L2 - 100;
                    L3 = L3 - 100;
                    L4 = L4 - 100;
                    L5 = L5 - 100;
                    L6 = L6 - 100;

                    //编码器模式配置
                    NMCLib20.NMC_SetEncMode(Devhandle[2], 0, 2304);
                    NMCLib20.NMC_SetEncMode(Devhandle[2], 1, 2304);
                    NMCLib20.NMC_SetEncMode(Devhandle[2], 2, 2304);
                    NMCLib20.NMC_SetEncMode(Devhandle[2], 3, 2304);

                    //伺服报警配置:轴句柄;伺服报警触是否有效，1 为有效，0 为无效;伺服报警触发电平，1 为高电平触发，0为低电平触发
                    if (NMCLib20.NMC_MtSetAlarmCfg(axishandle[8], 1, 1) != 0)
                        return L0 - 0;
                    if (NMCLib20.NMC_MtSetAlarmCfg(axishandle[9], 1, 1) != 0)
                        return L0 - 1;
                    if (NMCLib20.NMC_MtSetAlarmCfg(axishandle[10], 1, 1) != 0)
                        return L0 - 2;
                    if (NMCLib20.NMC_MtSetAlarmCfg(axishandle[11], 1, 1) != 0)
                        return L0 - 3;

                    //限位配置:轴句柄;正向限位是否有效，1 为有效，0 为无效;负向限位是否有效，1 为有效，0 为无效;正向限位触发电平，1 为高电平触发，0为低电平触发;正向限位触发电平，1 为高电平触发，0为低电平触发
                    if (NMCLib20.NMC_MtSetLmtCfg(axishandle[8], 1, 1, 1, 1) != 0)
                        return L1 - 0;
                    if (NMCLib20.NMC_MtSetLmtCfg(axishandle[9], 1, 1, 1, 1) != 0)
                        return L1 - 1;
                    if (NMCLib20.NMC_MtSetLmtCfg(axishandle[10], 1, 1, 1, 1) != 0)
                        return L1 - 2;
                    if (NMCLib20.NMC_MtSetLmtCfg(axishandle[11], 1, 1, 1, 1) != 0)
                        return L1 - 3;

                    //软限位配置:轴句柄;软件限位是否有效，1 为有效，0 为无效
                    if (NMCLib20.NMC_MtSwLmtOnOff(axishandle[8], 0) != 0)
                        return L2 - 0;
                    if (NMCLib20.NMC_MtSwLmtOnOff(axishandle[9], 0) != 0)
                        return L2 - 1;
                    if (NMCLib20.NMC_MtSwLmtOnOff(axishandle[10], 0) != 0)
                        return L2 - 2;
                    if (NMCLib20.NMC_MtSwLmtOnOff(axishandle[11], 0) != 0)
                        return L2 - 3;

                    //脉冲输出配置:轴句柄;输出取反，0 为不取反，1 为取反;输出模式，0 为脉冲+方向，1 为正负脉冲
                    if (NMCLib20.NMC_MtSetStepMode(axishandle[8], 0, 1) != 0)
                        return L3 - 0;
                    if (NMCLib20.NMC_MtSetStepMode(axishandle[9], 0, 1) != 0)
                        return L3 - 1;
                    if (NMCLib20.NMC_MtSetStepMode(axishandle[10], 0, 1) != 0)
                        return L3 - 2;
                    if (NMCLib20.NMC_MtSetStepMode(axishandle[11], 0, 1) != 0)
                        return L3 - 3;

                    //设置单轴急停DI:轴句柄;通用输入序号,取值范围[0,n]，设置为-1，则表示取消急停DI;触发电平,0：低电平,1：高电平
                    if (NMCLib20.NMC_MtSetEstopDI(axishandle[8], -1, 1) != 0)
                        return L4 - 0;
                    if (NMCLib20.NMC_MtSetEstopDI(axishandle[9], -1, 1) != 0)
                        return L4 - 1;
                    if (NMCLib20.NMC_MtSetEstopDI(axishandle[10], -1, 1) != 0)
                        return L4 - 2;
                    if (NMCLib20.NMC_MtSetEstopDI(axishandle[11], -1, 1) != 0)
                        return L4 - 3;

                    //NMCLib20.THomeSetting homepara;//回零参数
                    //homepara.mode = 1;//模式:1.原点开关触发;2.限位开关触发;3.(单Z相)编码器的Index信号触发;4.(原点+Z相)原点触发后,正向寻找到Index信号触发;5.(原点-Z相)原点触发后,反向寻找到Index信号触发;6.(原点-Z相)限位触发后,反向寻找到Index信号触发
                    //homepara.dir = 0;//搜寻零点方向（必须）0:负向;1:正向,其它值无意义
                    //homepara.offset = 0;//原点偏移（必须）
                    //homepara.scan1stVel = 10;//基本搜寻速度（必须）
                    //homepara.scan2ndVel = 10;//低速（两次搜寻时需要）
                    //homepara.acc = 0.1;//加速度
                    //homepara.reScanEn = 1;//是否两次搜寻零点（可选,不用时设为0）
                    //homepara.homeEdge = 1;//原点触发沿（默认0下降沿触发）,与原点触发电平有关
                    //homepara.lmtEdge = 0;//限位触发沿（默认0下降沿触发）,与限位触发电平有关
                    //homepara.zEdge = 1;//限位触发沿（默认0下降沿触发）,与Z向触发电平有关
                    //homepara.iniRetPos = 0;//起始时反向离开当前位置的运动距离（可选,不用时设为0）
                    //homepara.retSwOffset = 0;//反向运动时离开开关距离（可选,不用时设为0）
                    //homepara.safeLen = 0;//安全距离,回零时最远搜寻距离（可选,0表示不限制距离）
                    //homepara.usePreSetPtpPara = 0;//1表示需要在回零前,自己设置回零运动（点到点）的参数.为0时,回零运动的减加速度默认等于acc,起跳速度、终点速度、平滑系数默认为0
                    //homepara.reserved0 = 0;//保留
                    //homepara.reserved1 = 0;//保留
                    //homepara.reserved2 = 0;//保留
                    ////设置回零参数:轴句柄;回零参数结构
                    //if (NMCLib20.NMC_MtSetHomePara(axishandle[8], ref homepara) != 0)
                    //    return L5 - 0;
                    //if (NMCLib20.NMC_MtSetHomePara(axishandle[9], ref homepara) != 0)
                    //    return L5 - 1;
                    //if (NMCLib20.NMC_MtSetHomePara(axishandle[10], ref homepara) != 0)
                    //    return L5 - 2;
                    //if (NMCLib20.NMC_MtSetHomePara(axishandle[11], ref homepara) != 0)
                    //    return L5 - 3;

                    //清除轴错误状态
                    if (NMCLib20.NMC_MtClrError(axishandle[8]) != 0)
                        return L6 - 0;
                    if (NMCLib20.NMC_MtClrError(axishandle[9]) != 0)
                        return L6 - 1;
                    if (NMCLib20.NMC_MtClrError(axishandle[10]) != 0)
                        return L6 - 2;
                    if (NMCLib20.NMC_MtClrError(axishandle[11]) != 0)
                        return L6 - 3;

                    axisImmediateStop(8);
                    axisImmediateStop(9);
                    axisImmediateStop(10);
                    axisImmediateStop(11);

                    _axis08Command = 0;
                    _axis09Command = 0;
                    _axis10Command = 0;
                    _axis11Command = 0;
                    _axis08CommandSaved = 0;
                    _axis09CommandSaved = 0;
                    _axis10CommandSaved = 0;
                    _axis11CommandSaved = 0;
                    _axis08HomedFlag = false;
                    _axis09HomedFlag = false;
                    _axis10HomedFlag = false;
                    _axis11HomedFlag = false;
                    _axis08CurrentPositionNumber = 0;
                    _axis09CurrentPositionNumber = 0;
                    _axis10CurrentPositionNumber = 0;
                    _axis11CurrentPositionNumber = 0;
                    axis08MoveTimeOutCount = 0;
                    axis09MoveTimeOutCount = 0;
                    axis10MoveTimeOutCount = 0;
                    axis11MoveTimeOutCount = 0;
                    _axis08ErrorNumber = 0;
                    _axis09ErrorNumber = 0;
                    _axis10ErrorNumber = 0;
                    _axis11ErrorNumber = 0;
                    //SERVO ON
                    NMCLib20.NMC_MtSetSvOn(axishandle[8]);
                    NMCLib20.NMC_MtSetSvOn(axishandle[9]);
                    NMCLib20.NMC_MtSetSvOn(axishandle[10]);
                    NMCLib20.NMC_MtSetSvOn(axishandle[11]);

                    //ADD 202210151953 Pause
                    _axis08PauseRequestFlag = false;
                    _axis08ResumeRequestFlag = false;
                    _axis09PauseRequestFlag = false;
                    _axis09ResumeRequestFlag = false;
                    _axis10PauseRequestFlag = false;
                    _axis10ResumeRequestFlag = false;
                    _axis11PauseRequestFlag = false;
                    _axis11ResumeRequestFlag = false;
                }

                //轴状态更新线程
                td_axisStatusUpdata = new Thread(axisStatusUpdataThread);
                td_axisStatusUpdata.IsBackground = true;
                td_axisStatusUpdata.Start();

                initialFlag = true;
            }
            return n;
        }
        #endregion

        #region axisMoveTimeOut_MT:轴运动超时
        /// <summary>
        /// axisMoveTimeOut_MT:轴运动超时
        /// </summary>
        private void axisMoveTimeOut_MT(object sender, long JumpPeriod, long interval)
        {
            if (_cardQuantity >= 1)
            {
                if ((_axis00Command >= 1 && _axis00Command <= 900) || _axis00Command == 999 || _axis00Command == 1000)
                    axis00MoveTimeOutCount++;
                else
                    axis00MoveTimeOutCount = 0;

                if ((_axis01Command >= 1 && _axis01Command <= 900) || _axis01Command == 999 || _axis01Command == 1000)
                    axis01MoveTimeOutCount++;
                else
                    axis01MoveTimeOutCount = 0;

                if ((_axis02Command >= 1 && _axis02Command <= 900) || _axis02Command == 999 || _axis02Command == 1000)
                    axis02MoveTimeOutCount++;
                else
                    axis02MoveTimeOutCount = 0;

                if ((_axis03Command >= 1 && _axis03Command <= 900) || _axis03Command == 999 || _axis03Command == 1000)
                    axis03MoveTimeOutCount++;
                else
                    axis03MoveTimeOutCount = 0;
            }

            if (_cardQuantity >= 2)
            {
                if ((_axis04Command >= 1 && _axis04Command <= 900) || _axis04Command == 999 || _axis04Command == 1000)
                    axis04MoveTimeOutCount++;
                else
                    axis04MoveTimeOutCount = 0;

                if ((_axis05Command >= 1 && _axis05Command <= 900) || _axis05Command == 999 || _axis05Command == 1000)
                    axis05MoveTimeOutCount++;
                else
                    axis05MoveTimeOutCount = 0;

                if ((_axis06Command >= 1 && _axis06Command <= 900) || _axis06Command == 999 || _axis06Command == 1000)
                    axis06MoveTimeOutCount++;
                else
                    axis06MoveTimeOutCount = 0;

                if ((_axis07Command >= 1 && _axis07Command <= 900) || _axis07Command == 999 || _axis07Command == 1000)
                    axis07MoveTimeOutCount++;
                else
                    axis07MoveTimeOutCount = 0;
            }

            if (_cardQuantity >= 3)
            {
                if ((_axis08Command >= 1 && _axis08Command <= 900) || _axis08Command == 999 || _axis08Command == 1000)
                    axis08MoveTimeOutCount++;
                else
                    axis08MoveTimeOutCount = 0;

                if ((_axis09Command >= 1 && _axis09Command <= 900) || _axis09Command == 999 || _axis09Command == 1000)
                    axis09MoveTimeOutCount++;
                else
                    axis09MoveTimeOutCount = 0;

                if ((_axis10Command >= 1 && _axis10Command <= 900) || _axis10Command == 999 || _axis10Command == 1000)
                    axis10MoveTimeOutCount++;
                else
                    axis10MoveTimeOutCount = 0;

                if ((_axis11Command >= 1 && _axis11Command <= 900) || _axis11Command == 999 || _axis11Command == 1000)
                    axis11MoveTimeOutCount++;
                else
                    axis11MoveTimeOutCount = 0;
            }
        }
        #endregion

        #region axisStatusUpdataThread:轴状态更新线程
        /// <summary>
        /// axisStatusUpdataThread:轴状态更新线程
        /// </summary>
        private void axisStatusUpdataThread()
        {
            //报警复位信号自动OFF计时
            int _axis00resetOutputOnCount = 0;
            int _axis01resetOutputOnCount = 0;
            int _axis02resetOutputOnCount = 0;
            int _axis03resetOutputOnCount = 0;
            int _axis04resetOutputOnCount = 0;
            int _axis05resetOutputOnCount = 0;
            int _axis06resetOutputOnCount = 0;
            int _axis07resetOutputOnCount = 0;
            int _axis08resetOutputOnCount = 0;
            int _axis09resetOutputOnCount = 0;
            int _axis10resetOutputOnCount = 0;
            int _axis11resetOutputOnCount = 0;

            while (true)
            {
                if (_cardQuantity >= 1)
                {
                    //轴当前位置
                    NMCLib20.NMC_MtGetPrfPos(axishandle[0], ref axis00CurrentPosition_Pulse);
                    NMCLib20.NMC_MtGetPrfPos(axishandle[1], ref axis01CurrentPosition_Pulse);
                    NMCLib20.NMC_MtGetPrfPos(axishandle[2], ref axis02CurrentPosition_Pulse);
                    NMCLib20.NMC_MtGetPrfPos(axishandle[3], ref axis03CurrentPosition_Pulse);
                    _axis00CurrentPosition_mm = axis00CurrentPosition_Pulse * _axis00mmPP;
                    _axis01CurrentPosition_mm = axis01CurrentPosition_Pulse * _axis01mmPP;
                    _axis02CurrentPosition_mm = axis02CurrentPosition_Pulse * _axis02mmPP;
                    _axis03CurrentPosition_mm = axis03CurrentPosition_Pulse * _axis03mmPP;
                    //轴当前定位偏差值
                    axis00CurrentDeviationValue_mm = (axis00CurrentPosition_Pulse - axis00TargetPosition_Pulse) * _axis00mmPP;
                    axis01CurrentDeviationValue_mm = (axis01CurrentPosition_Pulse - axis01TargetPosition_Pulse) * _axis01mmPP;
                    axis02CurrentDeviationValue_mm = (axis02CurrentPosition_Pulse - axis02TargetPosition_Pulse) * _axis02mmPP;
                    axis03CurrentDeviationValue_mm = (axis03CurrentPosition_Pulse - axis03TargetPosition_Pulse) * _axis03mmPP;
                    //CheckDoneStatus
                    short axsists = 0;
                    NMCLib20.NMC_MtGetSts(axishandle[0], ref axsists);
                    _axis00CheckDoneStatus = (axsists & (1 << 0)) == 0 ? 1 : 0;//取bit0,0静止,1运动
                    NMCLib20.NMC_MtGetSts(axishandle[1], ref axsists);
                    _axis01CheckDoneStatus = (axsists & (1 << 0)) == 0 ? 1 : 0;//取bit0,0静止,1运动
                    NMCLib20.NMC_MtGetSts(axishandle[2], ref axsists);
                    _axis02CheckDoneStatus = (axsists & (1 << 0)) == 0 ? 1 : 0;//取bit0,0静止,1运动
                    NMCLib20.NMC_MtGetSts(axishandle[3], ref axsists);
                    _axis03CheckDoneStatus = (axsists & (1 << 0)) == 0 ? 1 : 0;//取bit0,0静止,1运动
                    //轴当前速度读取
                    double axsivel = 0;
                    NMCLib20.NMC_MtGetPrfVel(axishandle[0], ref axsivel);
                    _axis00CurrentSpeed_mmps = (float)axsivel * _axis00mmPP * 1000;
                    NMCLib20.NMC_MtGetPrfVel(axishandle[1], ref axsivel);
                    _axis01CurrentSpeed_mmps = (float)axsivel * _axis01mmPP * 1000;
                    NMCLib20.NMC_MtGetPrfVel(axishandle[2], ref axsivel);
                    _axis02CurrentSpeed_mmps = (float)axsivel * _axis02mmPP * 1000;
                    NMCLib20.NMC_MtGetPrfVel(axishandle[3], ref axsivel);
                    _axis03CurrentSpeed_mmps = (float)axsivel * _axis03mmPP * 1000;
                    //999:Homing
                    if (_axis00Command == 999 && _axis00CheckDoneStatus == 1 && _axis00HomeStartFlag && axis00CurrentPosition_Pulse == 0)
                    {
                        _axis00HomeStartFlag = false;
                        _axis00Command = 0;
                        _axis00CommandSaved = 0;
                        short homeState = 0;
                        NMCLib20.NMC_MtGetHomeSts(axishandle[0], ref homeState);
                        if ((homeState & (1 << 0)) == 0 && (homeState & (1 << 1)) != 0 && (homeState & (1 << 2)) == 0 && (homeState & (1 << 3)) == 0 && (homeState & (1 << 4)) == 0)//bit 0,该位为1:回零中;bit 1,该位为1:回零成功;bit 2,该位为1:回零失败;bit 3,该位为1:回零错误,运动参数出错导致不动;bit 4,该位为1:回零错误,搜寻过程中开关没触发
                            _axis00HomedFlag = true;
                        else
                            _axis00HomedFlag = false;
                    }
                    if (_axis01Command == 999 && _axis01CheckDoneStatus == 1 && _axis01HomeStartFlag && axis01CurrentPosition_Pulse == 0)
                    {
                        _axis01HomeStartFlag = false;
                        _axis01Command = 0;
                        _axis01CommandSaved = 0;
                        short homeState = 0;
                        NMCLib20.NMC_MtGetHomeSts(axishandle[1], ref homeState);
                        if ((homeState & (1 << 0)) == 0 && (homeState & (1 << 1)) != 0 && (homeState & (1 << 2)) == 0 && (homeState & (1 << 3)) == 0 && (homeState & (1 << 4)) == 0)//bit 0,该位为1:回零中;bit 1,该位为1:回零成功;bit 2,该位为1:回零失败;bit 3,该位为1:回零错误,运动参数出错导致不动;bit 4,该位为1:回零错误,搜寻过程中开关没触发
                            _axis01HomedFlag = true;
                        else
                            _axis01HomedFlag = false;
                    }
                    if (_axis02Command == 999 && _axis02CheckDoneStatus == 1 && _axis02HomeStartFlag && axis02CurrentPosition_Pulse == 0)
                    {
                        _axis02HomeStartFlag = false;
                        _axis02Command = 0;
                        _axis02CommandSaved = 0;
                        short homeState = 0;
                        NMCLib20.NMC_MtGetHomeSts(axishandle[2], ref homeState);
                        if ((homeState & (1 << 0)) == 0 && (homeState & (1 << 1)) != 0 && (homeState & (1 << 2)) == 0 && (homeState & (1 << 3)) == 0 && (homeState & (1 << 4)) == 0)//bit 0,该位为1:回零中;bit 1,该位为1:回零成功;bit 2,该位为1:回零失败;bit 3,该位为1:回零错误,运动参数出错导致不动;bit 4,该位为1:回零错误,搜寻过程中开关没触发
                            _axis02HomedFlag = true;
                        else
                            _axis02HomedFlag = false;
                    }
                    if (_axis03Command == 999 && _axis03CheckDoneStatus == 1 && _axis03HomeStartFlag && axis03CurrentPosition_Pulse == 0)
                    {
                        _axis03HomeStartFlag = false;
                        _axis03Command = 0;
                        _axis03CommandSaved = 0;
                        short homeState = 0;
                        NMCLib20.NMC_MtGetHomeSts(axishandle[3], ref homeState);
                        if ((homeState & (1 << 0)) == 0 && (homeState & (1 << 1)) != 0 && (homeState & (1 << 2)) == 0 && (homeState & (1 << 3)) == 0 && (homeState & (1 << 4)) == 0)//bit 0,该位为1:回零中;bit 1,该位为1:回零成功;bit 2,该位为1:回零失败;bit 3,该位为1:回零错误,运动参数出错导致不动;bit 4,该位为1:回零错误,搜寻过程中开关没触发
                            _axis03HomedFlag = true;
                        else
                            _axis03HomedFlag = false;
                    }
                    //1-900:点位号定位 或者 1000:实时位置数据定位
                    NMCLib20.NMC_MtGetPrfPos(axishandle[0], ref axis00CurrentPosition_Pulse);
                    _axis00CurrentPosition_mm = axis00CurrentPosition_Pulse * _axis00mmPP;
                    axis00CurrentDeviationValue_mm = (axis00CurrentPosition_Pulse - axis00TargetPosition_Pulse) * _axis00mmPP;
                    if (_axis00HomedFlag && _axis00MoveStartFlag && ((_axis00Command >= 1 && _axis00Command <= 900) || _axis00Command == 1000) &&
                        (!_axis00ELMinus && !_axis00ELPlus && axis00CurrentDeviationValue_mm >= axis00TargetDeviationValue_mm * -1 && axis00CurrentDeviationValue_mm <= axis00TargetDeviationValue_mm))
                    {
                        _axis00CurrentPositionNumber = _axis00Command;
                        _axis00Command = 0;
                        _axis00CommandSaved = 0;
                        _axis00MoveStartFlag = false;
                    }
                    NMCLib20.NMC_MtGetPrfPos(axishandle[1], ref axis01CurrentPosition_Pulse);
                    _axis01CurrentPosition_mm = axis01CurrentPosition_Pulse * _axis01mmPP;
                    axis01CurrentDeviationValue_mm = (axis01CurrentPosition_Pulse - axis01TargetPosition_Pulse) * _axis01mmPP;
                    if (_axis01HomedFlag && _axis01MoveStartFlag && ((_axis01Command >= 1 && _axis01Command <= 900) || _axis01Command == 1000) &&
                        (!_axis01ELMinus && !_axis01ELPlus && axis01CurrentDeviationValue_mm >= axis01TargetDeviationValue_mm * -1 && axis01CurrentDeviationValue_mm <= axis01TargetDeviationValue_mm))
                    {
                        _axis01CurrentPositionNumber = _axis01Command;
                        _axis01Command = 0;
                        _axis01CommandSaved = 0;
                        _axis01MoveStartFlag = false;
                    }
                    NMCLib20.NMC_MtGetPrfPos(axishandle[2], ref axis02CurrentPosition_Pulse);
                    _axis02CurrentPosition_mm = axis02CurrentPosition_Pulse * _axis02mmPP;
                    axis02CurrentDeviationValue_mm = (axis02CurrentPosition_Pulse - axis02TargetPosition_Pulse) * _axis02mmPP;
                    if (_axis02HomedFlag && _axis02MoveStartFlag && ((_axis02Command >= 1 && _axis02Command <= 900) || _axis02Command == 1000) &&
                        (!_axis02ELMinus && !_axis02ELPlus && axis02CurrentDeviationValue_mm >= axis02TargetDeviationValue_mm * -1 && axis02CurrentDeviationValue_mm <= axis02TargetDeviationValue_mm))
                    {
                        _axis02CurrentPositionNumber = _axis02Command;
                        _axis02Command = 0;
                        _axis02CommandSaved = 0;
                        _axis02MoveStartFlag = false;
                    }
                    NMCLib20.NMC_MtGetPrfPos(axishandle[3], ref axis03CurrentPosition_Pulse);
                    _axis03CurrentPosition_mm = axis03CurrentPosition_Pulse * _axis03mmPP;
                    axis03CurrentDeviationValue_mm = (axis03CurrentPosition_Pulse - axis03TargetPosition_Pulse) * _axis03mmPP;
                    if (_axis03HomedFlag && _axis03MoveStartFlag && ((_axis03Command >= 1 && _axis03Command <= 900) || _axis03Command == 1000) &&
                        (!_axis03ELMinus && !_axis03ELPlus && axis03CurrentDeviationValue_mm >= axis03TargetDeviationValue_mm * -1 && axis03CurrentDeviationValue_mm <= axis03TargetDeviationValue_mm))
                    {
                        _axis03CurrentPositionNumber = _axis03Command;
                        _axis03Command = 0;
                        _axis03CommandSaved = 0;
                        _axis03MoveStartFlag = false;
                    }
                    //Inch
                    if (_axis00CommandSaved == 901 && _axis00CheckDoneStatus == 1)
                    {
                        _axis00Command = 0;
                        _axis00CommandSaved = 0;
                    }
                    if (_axis01CommandSaved == 901 && _axis01CheckDoneStatus == 1)
                    {
                        _axis01Command = 0;
                        _axis01CommandSaved = 0;
                    }
                    if (_axis02CommandSaved == 901 && _axis02CheckDoneStatus == 1)
                    {
                        _axis02Command = 0;
                        _axis02CommandSaved = 0;
                    }
                    if (_axis03CommandSaved == 901 && _axis03CheckDoneStatus == 1)
                    {
                        _axis03Command = 0;
                        _axis03CommandSaved = 0;
                    }

                    //ADD 202210151953 Pause
                    if (((_axis00Command >= 1 && _axis00Command <= 900) || _axis00Command == 1000) && _axis00PauseRequestFlag)
                    {
                        _axis00PauseRequestFlag = false;
                        _axis00ResumeRequestFlag = false;
                        axisImmediateStop(0);
                        _axis00PausedFlag = true;
                    }
                    if (((_axis01Command >= 1 && _axis01Command <= 900) || _axis01Command == 1000) && _axis01PauseRequestFlag)
                    {
                        _axis01PauseRequestFlag = false;
                        _axis01ResumeRequestFlag = false;
                        axisImmediateStop(1);
                        _axis01PausedFlag = true;
                    }
                    if (((_axis02Command >= 1 && _axis02Command <= 900) || _axis02Command == 1000) && _axis02PauseRequestFlag)
                    {
                        _axis02PauseRequestFlag = false;
                        _axis02ResumeRequestFlag = false;
                        axisImmediateStop(2);
                        _axis02PausedFlag = true;
                    }
                    if (((_axis03Command >= 1 && _axis03Command <= 900) || _axis03Command == 1000) && _axis03PauseRequestFlag)
                    {
                        _axis03PauseRequestFlag = false;
                        _axis03ResumeRequestFlag = false;
                        axisImmediateStop(3);
                        _axis03PausedFlag = true;
                    }
                    if (_axis00PausedFlag && _axis00ResumeRequestFlag && !_axis00PauseRequestFlag)
                    {
                        AxisAction(_axis00_axisCommand_SaveForPause, _axis00_axisNumber_SaveForPause, _axis00_startVelocity_SaveForPause,
                            _axis00_maxVelocity_SaveForPause, _axis00_stopVelocity_SaveForPause, _axis00_accTime_SaveForPause,
                            _axis00_decTime_SaveForPause, _axis00_targetPosition_SaveForPause, _axis00_jogDirection_SaveForPause,
                            _axis00_inch_SaveForPause, _axis00_deviationValue_SaveForPause, _axis00_useSModeMoveFlag_SaveForPause, _axis00_sMoveTime_SaveForPause);
                        _axis00ResumeRequestFlag = false;
                        _axis00PausedFlag = false;
                    }
                    if (_axis01PausedFlag && _axis01ResumeRequestFlag && !_axis01PauseRequestFlag)
                    {
                        AxisAction(_axis01_axisCommand_SaveForPause, _axis01_axisNumber_SaveForPause, _axis01_startVelocity_SaveForPause,
                            _axis01_maxVelocity_SaveForPause, _axis01_stopVelocity_SaveForPause, _axis01_accTime_SaveForPause,
                            _axis01_decTime_SaveForPause, _axis01_targetPosition_SaveForPause, _axis01_jogDirection_SaveForPause,
                            _axis01_inch_SaveForPause, _axis01_deviationValue_SaveForPause, _axis01_useSModeMoveFlag_SaveForPause, _axis01_sMoveTime_SaveForPause);
                        _axis01ResumeRequestFlag = false;
                        _axis01PausedFlag = false;
                    }
                    if (_axis02PausedFlag && _axis02ResumeRequestFlag && !_axis02PauseRequestFlag)
                    {
                        AxisAction(_axis02_axisCommand_SaveForPause, _axis02_axisNumber_SaveForPause, _axis02_startVelocity_SaveForPause,
                            _axis02_maxVelocity_SaveForPause, _axis02_stopVelocity_SaveForPause, _axis02_accTime_SaveForPause,
                            _axis02_decTime_SaveForPause, _axis02_targetPosition_SaveForPause, _axis02_jogDirection_SaveForPause,
                            _axis02_inch_SaveForPause, _axis02_deviationValue_SaveForPause, _axis02_useSModeMoveFlag_SaveForPause, _axis02_sMoveTime_SaveForPause);
                        _axis02ResumeRequestFlag = false;
                        _axis02PausedFlag = false;
                    }
                    if (_axis03PausedFlag && _axis03ResumeRequestFlag && !_axis03PauseRequestFlag)
                    {
                        AxisAction(_axis03_axisCommand_SaveForPause, _axis03_axisNumber_SaveForPause, _axis03_startVelocity_SaveForPause,
                            _axis03_maxVelocity_SaveForPause, _axis03_stopVelocity_SaveForPause, _axis03_accTime_SaveForPause,
                            _axis03_decTime_SaveForPause, _axis03_targetPosition_SaveForPause, _axis03_jogDirection_SaveForPause,
                            _axis03_inch_SaveForPause, _axis03_deviationValue_SaveForPause, _axis03_useSModeMoveFlag_SaveForPause, _axis03_sMoveTime_SaveForPause);
                        _axis03ResumeRequestFlag = false;
                        _axis03PausedFlag = false;
                    }
                }

                if (_cardQuantity >= 2)
                {
                    //轴当前位置
                    NMCLib20.NMC_MtGetPrfPos(axishandle[4], ref axis04CurrentPosition_Pulse);
                    NMCLib20.NMC_MtGetPrfPos(axishandle[5], ref axis05CurrentPosition_Pulse);
                    NMCLib20.NMC_MtGetPrfPos(axishandle[6], ref axis06CurrentPosition_Pulse);
                    NMCLib20.NMC_MtGetPrfPos(axishandle[7], ref axis07CurrentPosition_Pulse);
                    _axis04CurrentPosition_mm = axis04CurrentPosition_Pulse * _axis04mmPP;
                    _axis05CurrentPosition_mm = axis05CurrentPosition_Pulse * _axis05mmPP;
                    _axis06CurrentPosition_mm = axis06CurrentPosition_Pulse * _axis06mmPP;
                    _axis07CurrentPosition_mm = axis07CurrentPosition_Pulse * _axis07mmPP;
                    //轴当前定位偏差值
                    axis04CurrentDeviationValue_mm = (axis04CurrentPosition_Pulse - axis04TargetPosition_Pulse) * _axis04mmPP;
                    axis05CurrentDeviationValue_mm = (axis05CurrentPosition_Pulse - axis05TargetPosition_Pulse) * _axis05mmPP;
                    axis06CurrentDeviationValue_mm = (axis06CurrentPosition_Pulse - axis06TargetPosition_Pulse) * _axis06mmPP;
                    axis07CurrentDeviationValue_mm = (axis07CurrentPosition_Pulse - axis07TargetPosition_Pulse) * _axis07mmPP;
                    //CheckDoneStatus
                    short axsists = 0;
                    NMCLib20.NMC_MtGetSts(axishandle[4], ref axsists);
                    _axis04CheckDoneStatus = (axsists & (1 << 0)) == 0 ? 1 : 0;//取bit0,0静止,1运动
                    NMCLib20.NMC_MtGetSts(axishandle[5], ref axsists);
                    _axis05CheckDoneStatus = (axsists & (1 << 0)) == 0 ? 1 : 0;//取bit0,0静止,1运动
                    NMCLib20.NMC_MtGetSts(axishandle[6], ref axsists);
                    _axis06CheckDoneStatus = (axsists & (1 << 0)) == 0 ? 1 : 0;//取bit0,0静止,1运动
                    NMCLib20.NMC_MtGetSts(axishandle[7], ref axsists);
                    _axis07CheckDoneStatus = (axsists & (1 << 0)) == 0 ? 1 : 0;//取bit0,0静止,1运动
                    //轴当前速度读取
                    double axsivel = 0;
                    NMCLib20.NMC_MtGetPrfVel(axishandle[4], ref axsivel);
                    _axis04CurrentSpeed_mmps = (float)axsivel * _axis04mmPP * 1000;
                    NMCLib20.NMC_MtGetPrfVel(axishandle[5], ref axsivel);
                    _axis05CurrentSpeed_mmps = (float)axsivel * _axis05mmPP * 1000;
                    NMCLib20.NMC_MtGetPrfVel(axishandle[6], ref axsivel);
                    _axis06CurrentSpeed_mmps = (float)axsivel * _axis06mmPP * 1000;
                    NMCLib20.NMC_MtGetPrfVel(axishandle[7], ref axsivel);
                    _axis07CurrentSpeed_mmps = (float)axsivel * _axis07mmPP * 1000;
                    //999:Homing
                    if (_axis04Command == 999 && _axis04CheckDoneStatus == 1 && _axis04HomeStartFlag && axis04CurrentPosition_Pulse == 0)
                    {
                        _axis04HomeStartFlag = false;
                        _axis04Command = 0;
                        _axis04CommandSaved = 0;
                        short homeState = 0;
                        NMCLib20.NMC_MtGetHomeSts(axishandle[4], ref homeState);
                        if ((homeState & (1 << 0)) == 0 && (homeState & (1 << 1)) != 0 && (homeState & (1 << 2)) == 0 && (homeState & (1 << 3)) == 0 && (homeState & (1 << 4)) == 0)//bit 0,该位为1:回零中;bit 1,该位为1:回零成功;bit 2,该位为1:回零失败;bit 3,该位为1:回零错误,运动参数出错导致不动;bit 4,该位为1:回零错误,搜寻过程中开关没触发
                            _axis04HomedFlag = true;
                        else
                            _axis04HomedFlag = false;
                    }
                    if (_axis05Command == 999 && _axis05CheckDoneStatus == 1 && _axis05HomeStartFlag && axis05CurrentPosition_Pulse == 0)
                    {
                        _axis05HomeStartFlag = false;
                        _axis05Command = 0;
                        _axis05CommandSaved = 0;
                        short homeState = 0;
                        NMCLib20.NMC_MtGetHomeSts(axishandle[5], ref homeState);
                        if ((homeState & (1 << 0)) == 0 && (homeState & (1 << 1)) != 0 && (homeState & (1 << 2)) == 0 && (homeState & (1 << 3)) == 0 && (homeState & (1 << 4)) == 0)//bit 0,该位为1:回零中;bit 1,该位为1:回零成功;bit 2,该位为1:回零失败;bit 3,该位为1:回零错误,运动参数出错导致不动;bit 4,该位为1:回零错误,搜寻过程中开关没触发
                            _axis05HomedFlag = true;
                        else
                            _axis05HomedFlag = false;
                    }
                    if (_axis06Command == 999 && _axis06CheckDoneStatus == 1 && _axis06HomeStartFlag && axis06CurrentPosition_Pulse == 0)
                    {
                        _axis06HomeStartFlag = false;
                        _axis06Command = 0;
                        _axis06CommandSaved = 0;
                        short homeState = 0;
                        NMCLib20.NMC_MtGetHomeSts(axishandle[6], ref homeState);
                        if ((homeState & (1 << 0)) == 0 && (homeState & (1 << 1)) != 0 && (homeState & (1 << 2)) == 0 && (homeState & (1 << 3)) == 0 && (homeState & (1 << 4)) == 0)//bit 0,该位为1:回零中;bit 1,该位为1:回零成功;bit 2,该位为1:回零失败;bit 3,该位为1:回零错误,运动参数出错导致不动;bit 4,该位为1:回零错误,搜寻过程中开关没触发
                            _axis06HomedFlag = true;
                        else
                            _axis06HomedFlag = false;
                    }
                    if (_axis07Command == 999 && _axis07CheckDoneStatus == 1 && _axis07HomeStartFlag && axis07CurrentPosition_Pulse == 0)
                    {
                        _axis07HomeStartFlag = false;
                        _axis07Command = 0;
                        _axis07CommandSaved = 0;
                        short homeState = 0;
                        NMCLib20.NMC_MtGetHomeSts(axishandle[7], ref homeState);
                        if ((homeState & (1 << 0)) == 0 && (homeState & (1 << 1)) != 0 && (homeState & (1 << 2)) == 0 && (homeState & (1 << 3)) == 0 && (homeState & (1 << 4)) == 0)//bit 0,该位为1:回零中;bit 1,该位为1:回零成功;bit 2,该位为1:回零失败;bit 3,该位为1:回零错误,运动参数出错导致不动;bit 4,该位为1:回零错误,搜寻过程中开关没触发
                            _axis07HomedFlag = true;
                        else
                            _axis07HomedFlag = false;
                    }
                    //1-900:点位号定位 或者 1000:实时位置数据定位
                    NMCLib20.NMC_MtGetPrfPos(axishandle[4], ref axis04CurrentPosition_Pulse);
                    _axis04CurrentPosition_mm = axis04CurrentPosition_Pulse * _axis04mmPP;
                    axis04CurrentDeviationValue_mm = (axis04CurrentPosition_Pulse - axis04TargetPosition_Pulse) * _axis04mmPP;
                    if (_axis04HomedFlag && _axis04MoveStartFlag && ((_axis04Command >= 1 && _axis04Command <= 900) || _axis04Command == 1000) &&
                        (!_axis04ELMinus && !_axis04ELPlus && axis04CurrentDeviationValue_mm >= axis04TargetDeviationValue_mm * -1 && axis04CurrentDeviationValue_mm <= axis04TargetDeviationValue_mm))//Dmc1000.d1000_check_done(4) == 1 || Dmc1000.d1000_check_done(4) == 0 && 
                    {
                        _axis04CurrentPositionNumber = _axis04Command;
                        _axis04Command = 0;
                        _axis04CommandSaved = 0;
                        _axis04MoveStartFlag = false;
                    }
                    NMCLib20.NMC_MtGetPrfPos(axishandle[5], ref axis05CurrentPosition_Pulse);
                    _axis05CurrentPosition_mm = axis05CurrentPosition_Pulse * _axis05mmPP;
                    axis05CurrentDeviationValue_mm = (axis05CurrentPosition_Pulse - axis05TargetPosition_Pulse) * _axis05mmPP;
                    if (_axis05HomedFlag && _axis05MoveStartFlag && ((_axis05Command >= 1 && _axis05Command <= 900) || _axis05Command == 1000) &&
                        (!_axis05ELMinus && !_axis05ELPlus && axis05CurrentDeviationValue_mm >= axis05TargetDeviationValue_mm * -1 && axis05CurrentDeviationValue_mm <= axis05TargetDeviationValue_mm))
                    {
                        _axis05CurrentPositionNumber = _axis05Command;
                        _axis05Command = 0;
                        _axis05CommandSaved = 0;
                        _axis05MoveStartFlag = false;
                    }
                    NMCLib20.NMC_MtGetPrfPos(axishandle[6], ref axis06CurrentPosition_Pulse);
                    _axis06CurrentPosition_mm = axis06CurrentPosition_Pulse * _axis06mmPP;
                    axis06CurrentDeviationValue_mm = (axis06CurrentPosition_Pulse - axis06TargetPosition_Pulse) * _axis06mmPP;
                    if (_axis06HomedFlag && _axis06MoveStartFlag && ((_axis06Command >= 1 && _axis06Command <= 900) || _axis06Command == 1000) &&
                        (!_axis06ELMinus && !_axis06ELPlus && axis06CurrentDeviationValue_mm >= axis06TargetDeviationValue_mm * -1 && axis06CurrentDeviationValue_mm <= axis06TargetDeviationValue_mm))
                    {
                        _axis06CurrentPositionNumber = _axis06Command;
                        _axis06Command = 0;
                        _axis06CommandSaved = 0;
                        _axis06MoveStartFlag = false;
                    }
                    NMCLib20.NMC_MtGetPrfPos(axishandle[7], ref axis07CurrentPosition_Pulse);
                    _axis07CurrentPosition_mm = axis07CurrentPosition_Pulse * _axis07mmPP;
                    axis07CurrentDeviationValue_mm = (axis07CurrentPosition_Pulse - axis07TargetPosition_Pulse) * _axis07mmPP;
                    if (_axis07HomedFlag && _axis07MoveStartFlag && ((_axis07Command >= 1 && _axis07Command <= 900) || _axis07Command == 1000) &&
                        (!_axis07ELMinus && !_axis07ELPlus && axis07CurrentDeviationValue_mm >= axis07TargetDeviationValue_mm * -1 && axis07CurrentDeviationValue_mm <= axis07TargetDeviationValue_mm))
                    {
                        _axis07CurrentPositionNumber = _axis07Command;
                        _axis07Command = 0;
                        _axis07CommandSaved = 0;
                        _axis07MoveStartFlag = false;
                    }
                    //Inch
                    if (_axis04CommandSaved == 901 && _axis04CheckDoneStatus == 1)
                    {
                        _axis04Command = 0;
                        _axis04CommandSaved = 0;
                    }
                    if (_axis05CommandSaved == 901 && _axis05CheckDoneStatus == 1)
                    {
                        _axis05Command = 0;
                        _axis05CommandSaved = 0;
                    }
                    if (_axis06CommandSaved == 901 && _axis06CheckDoneStatus == 1)
                    {
                        _axis06Command = 0;
                        _axis06CommandSaved = 0;
                    }
                    if (_axis07CommandSaved == 901 && _axis07CheckDoneStatus == 1)
                    {
                        _axis07Command = 0;
                        _axis07CommandSaved = 0;
                    }

                    //ADD 202210151953 Pause
                    if (((_axis04Command >= 1 && _axis04Command <= 900) || _axis04Command == 1000) && _axis04PauseRequestFlag)
                    {
                        _axis04PauseRequestFlag = false;
                        _axis04ResumeRequestFlag = false;
                        axisImmediateStop(4);
                        _axis04PausedFlag = true;
                    }
                    if (((_axis05Command >= 1 && _axis05Command <= 900) || _axis05Command == 1000) && _axis05PauseRequestFlag)
                    {
                        _axis05PauseRequestFlag = false;
                        _axis05ResumeRequestFlag = false;
                        axisImmediateStop(5);
                        _axis05PausedFlag = true;
                    }
                    if (((_axis06Command >= 1 && _axis06Command <= 900) || _axis06Command == 1000) && _axis06PauseRequestFlag)
                    {
                        _axis06PauseRequestFlag = false;
                        _axis06ResumeRequestFlag = false;
                        axisImmediateStop(6);
                        _axis06PausedFlag = true;
                    }
                    if (((_axis07Command >= 1 && _axis07Command <= 900) || _axis07Command == 1000) && _axis07PauseRequestFlag)
                    {
                        _axis07PauseRequestFlag = false;
                        _axis07ResumeRequestFlag = false;
                        axisImmediateStop(7);
                        _axis07PausedFlag = true;
                    }
                    if (_axis04PausedFlag && _axis04ResumeRequestFlag && !_axis04PauseRequestFlag)
                    {
                        AxisAction(_axis04_axisCommand_SaveForPause, _axis04_axisNumber_SaveForPause, _axis04_startVelocity_SaveForPause,
                            _axis04_maxVelocity_SaveForPause, _axis04_stopVelocity_SaveForPause, _axis04_accTime_SaveForPause,
                            _axis04_decTime_SaveForPause, _axis04_targetPosition_SaveForPause, _axis04_jogDirection_SaveForPause,
                            _axis04_inch_SaveForPause, _axis04_deviationValue_SaveForPause, _axis04_useSModeMoveFlag_SaveForPause, _axis04_sMoveTime_SaveForPause);
                        _axis04ResumeRequestFlag = false;
                        _axis04PausedFlag = false;
                    }
                    if (_axis05PausedFlag && _axis05ResumeRequestFlag && !_axis05PauseRequestFlag)
                    {
                        AxisAction(_axis05_axisCommand_SaveForPause, _axis05_axisNumber_SaveForPause, _axis05_startVelocity_SaveForPause,
                            _axis05_maxVelocity_SaveForPause, _axis05_stopVelocity_SaveForPause, _axis05_accTime_SaveForPause,
                            _axis05_decTime_SaveForPause, _axis05_targetPosition_SaveForPause, _axis05_jogDirection_SaveForPause,
                            _axis05_inch_SaveForPause, _axis05_deviationValue_SaveForPause, _axis05_useSModeMoveFlag_SaveForPause, _axis05_sMoveTime_SaveForPause);
                        _axis05ResumeRequestFlag = false;
                        _axis05PausedFlag = false;
                    }
                    if (_axis06PausedFlag && _axis06ResumeRequestFlag && !_axis06PauseRequestFlag)
                    {
                        AxisAction(_axis06_axisCommand_SaveForPause, _axis06_axisNumber_SaveForPause, _axis06_startVelocity_SaveForPause,
                            _axis06_maxVelocity_SaveForPause, _axis06_stopVelocity_SaveForPause, _axis06_accTime_SaveForPause,
                            _axis06_decTime_SaveForPause, _axis06_targetPosition_SaveForPause, _axis06_jogDirection_SaveForPause,
                            _axis06_inch_SaveForPause, _axis06_deviationValue_SaveForPause, _axis06_useSModeMoveFlag_SaveForPause, _axis06_sMoveTime_SaveForPause);
                        _axis06ResumeRequestFlag = false;
                        _axis06PausedFlag = false;
                    }
                    if (_axis07PausedFlag && _axis07ResumeRequestFlag && !_axis07PauseRequestFlag)
                    {
                        AxisAction(_axis07_axisCommand_SaveForPause, _axis07_axisNumber_SaveForPause, _axis07_startVelocity_SaveForPause,
                            _axis07_maxVelocity_SaveForPause, _axis07_stopVelocity_SaveForPause, _axis07_accTime_SaveForPause,
                            _axis07_decTime_SaveForPause, _axis07_targetPosition_SaveForPause, _axis07_jogDirection_SaveForPause,
                            _axis07_inch_SaveForPause, _axis07_deviationValue_SaveForPause, _axis07_useSModeMoveFlag_SaveForPause, _axis07_sMoveTime_SaveForPause);
                        _axis07ResumeRequestFlag = false;
                        _axis07PausedFlag = false;
                    }
                }

                if (_cardQuantity >= 3)
                {
                    //轴当前位置
                    NMCLib20.NMC_MtGetPrfPos(axishandle[8], ref axis08CurrentPosition_Pulse);
                    NMCLib20.NMC_MtGetPrfPos(axishandle[9], ref axis09CurrentPosition_Pulse);
                    NMCLib20.NMC_MtGetPrfPos(axishandle[10], ref axis10CurrentPosition_Pulse);
                    NMCLib20.NMC_MtGetPrfPos(axishandle[11], ref axis11CurrentPosition_Pulse);
                    _axis08CurrentPosition_mm = axis08CurrentPosition_Pulse * _axis08mmPP;
                    _axis09CurrentPosition_mm = axis09CurrentPosition_Pulse * _axis09mmPP;
                    _axis10CurrentPosition_mm = axis10CurrentPosition_Pulse * _axis10mmPP;
                    _axis11CurrentPosition_mm = axis11CurrentPosition_Pulse * _axis11mmPP;
                    //轴当前定位偏差值
                    axis08CurrentDeviationValue_mm = (axis08CurrentPosition_Pulse - axis08TargetPosition_Pulse) * _axis08mmPP;
                    axis09CurrentDeviationValue_mm = (axis09CurrentPosition_Pulse - axis09TargetPosition_Pulse) * _axis09mmPP;
                    axis10CurrentDeviationValue_mm = (axis10CurrentPosition_Pulse - axis10TargetPosition_Pulse) * _axis10mmPP;
                    axis11CurrentDeviationValue_mm = (axis11CurrentPosition_Pulse - axis11TargetPosition_Pulse) * _axis11mmPP;
                    //CheckDoneStatus
                    short axsists = 0;
                    NMCLib20.NMC_MtGetSts(axishandle[8], ref axsists);
                    _axis08CheckDoneStatus = (axsists & (1 << 0)) == 0 ? 1 : 0;//取bit0,0静止,1运动
                    NMCLib20.NMC_MtGetSts(axishandle[9], ref axsists);
                    _axis09CheckDoneStatus = (axsists & (1 << 0)) == 0 ? 1 : 0;//取bit0,0静止,1运动
                    NMCLib20.NMC_MtGetSts(axishandle[10], ref axsists);
                    _axis10CheckDoneStatus = (axsists & (1 << 0)) == 0 ? 1 : 0;//取bit0,0静止,1运动
                    NMCLib20.NMC_MtGetSts(axishandle[11], ref axsists);
                    _axis11CheckDoneStatus = (axsists & (1 << 0)) == 0 ? 1 : 0;//取bit0,0静止,1运动
                    //轴当前速度读取
                    double axsivel = 0;
                    NMCLib20.NMC_MtGetPrfVel(axishandle[8], ref axsivel);
                    _axis08CurrentSpeed_mmps = (float)axsivel * _axis08mmPP * 1000;
                    NMCLib20.NMC_MtGetPrfVel(axishandle[9], ref axsivel);
                    _axis09CurrentSpeed_mmps = (float)axsivel * _axis09mmPP * 1000;
                    NMCLib20.NMC_MtGetPrfVel(axishandle[10], ref axsivel);
                    _axis10CurrentSpeed_mmps = (float)axsivel * _axis10mmPP * 1000;
                    NMCLib20.NMC_MtGetPrfVel(axishandle[11], ref axsivel);
                    _axis11CurrentSpeed_mmps = (float)axsivel * _axis11mmPP * 1000;
                    //999:Homing
                    if (_axis08Command == 999 && _axis08CheckDoneStatus == 1 && _axis08HomeStartFlag && axis08CurrentPosition_Pulse == 0)
                    {
                        _axis08HomeStartFlag = false;
                        _axis08Command = 0;
                        _axis08CommandSaved = 0;
                        short homeState = 0;
                        NMCLib20.NMC_MtGetHomeSts(axishandle[8], ref homeState);
                        if ((homeState & (1 << 0)) == 0 && (homeState & (1 << 1)) != 0 && (homeState & (1 << 2)) == 0 && (homeState & (1 << 3)) == 0 && (homeState & (1 << 4)) == 0)//bit 0,该位为1:回零中;bit 1,该位为1:回零成功;bit 2,该位为1:回零失败;bit 3,该位为1:回零错误,运动参数出错导致不动;bit 4,该位为1:回零错误,搜寻过程中开关没触发
                            _axis08HomedFlag = true;
                        else
                            _axis08HomedFlag = false;
                    }
                    if (_axis09Command == 999 && _axis09CheckDoneStatus == 1 && _axis09HomeStartFlag && axis09CurrentPosition_Pulse == 0)
                    {
                        _axis09HomeStartFlag = false;
                        _axis09Command = 0;
                        _axis09CommandSaved = 0;
                        short homeState = 0;
                        NMCLib20.NMC_MtGetHomeSts(axishandle[9], ref homeState);
                        if ((homeState & (1 << 0)) == 0 && (homeState & (1 << 1)) != 0 && (homeState & (1 << 2)) == 0 && (homeState & (1 << 3)) == 0 && (homeState & (1 << 4)) == 0)//bit 0,该位为1:回零中;bit 1,该位为1:回零成功;bit 2,该位为1:回零失败;bit 3,该位为1:回零错误,运动参数出错导致不动;bit 4,该位为1:回零错误,搜寻过程中开关没触发
                            _axis09HomedFlag = true;
                        else
                            _axis09HomedFlag = false;
                    }
                    if (_axis10Command == 999 && _axis10CheckDoneStatus == 1 && _axis10HomeStartFlag && axis10CurrentPosition_Pulse == 0)
                    {
                        _axis10HomeStartFlag = false;
                        _axis10Command = 0;
                        _axis10CommandSaved = 0;
                        short homeState = 0;
                        NMCLib20.NMC_MtGetHomeSts(axishandle[10], ref homeState);
                        if ((homeState & (1 << 0)) == 0 && (homeState & (1 << 1)) != 0 && (homeState & (1 << 2)) == 0 && (homeState & (1 << 3)) == 0 && (homeState & (1 << 4)) == 0)//bit 0,该位为1:回零中;bit 1,该位为1:回零成功;bit 2,该位为1:回零失败;bit 3,该位为1:回零错误,运动参数出错导致不动;bit 4,该位为1:回零错误,搜寻过程中开关没触发
                            _axis10HomedFlag = true;
                        else
                            _axis10HomedFlag = false;
                    }
                    if (_axis11Command == 999 && _axis11CheckDoneStatus == 1 && _axis11HomeStartFlag && axis11CurrentPosition_Pulse == 0)
                    {
                        _axis11HomeStartFlag = false;
                        _axis11Command = 0;
                        _axis11CommandSaved = 0;
                        short homeState = 0;
                        NMCLib20.NMC_MtGetHomeSts(axishandle[11], ref homeState);
                        if ((homeState & (1 << 0)) == 0 && (homeState & (1 << 1)) != 0 && (homeState & (1 << 2)) == 0 && (homeState & (1 << 3)) == 0 && (homeState & (1 << 4)) == 0)//bit 0,该位为1:回零中;bit 1,该位为1:回零成功;bit 2,该位为1:回零失败;bit 3,该位为1:回零错误,运动参数出错导致不动;bit 4,该位为1:回零错误,搜寻过程中开关没触发
                            _axis11HomedFlag = true;
                        else
                            _axis11HomedFlag = false;
                    }
                    //1-900:点位号定位 或者 1000:实时位置数据定位
                    NMCLib20.NMC_MtGetPrfPos(axishandle[8], ref axis08CurrentPosition_Pulse);
                    _axis08CurrentPosition_mm = axis08CurrentPosition_Pulse * _axis08mmPP;
                    axis08CurrentDeviationValue_mm = (axis08CurrentPosition_Pulse - axis08TargetPosition_Pulse) * _axis08mmPP;
                    if (_axis08HomedFlag && _axis08MoveStartFlag && ((_axis08Command >= 1 && _axis08Command <= 900) || _axis08Command == 1000) &&
                        (!_axis08ELMinus && !_axis08ELPlus && axis08CurrentDeviationValue_mm >= axis08TargetDeviationValue_mm * -1 && axis08CurrentDeviationValue_mm <= axis08TargetDeviationValue_mm))
                    {
                        _axis08CurrentPositionNumber = _axis08Command;
                        _axis08Command = 0;
                        _axis08CommandSaved = 0;
                        _axis08MoveStartFlag = false;
                    }
                    NMCLib20.NMC_MtGetPrfPos(axishandle[9], ref axis09CurrentPosition_Pulse);
                    _axis09CurrentPosition_mm = axis09CurrentPosition_Pulse * _axis09mmPP;
                    axis09CurrentDeviationValue_mm = (axis09CurrentPosition_Pulse - axis09TargetPosition_Pulse) * _axis09mmPP;
                    if (_axis09HomedFlag && _axis09MoveStartFlag && ((_axis09Command >= 1 && _axis09Command <= 900) || _axis09Command == 1000) &&
                        (!_axis09ELMinus && !_axis09ELPlus && axis09CurrentDeviationValue_mm >= axis09TargetDeviationValue_mm * -1 && axis09CurrentDeviationValue_mm <= axis09TargetDeviationValue_mm))
                    {
                        _axis09CurrentPositionNumber = _axis09Command;
                        _axis09Command = 0;
                        _axis09CommandSaved = 0;
                        _axis09MoveStartFlag = false;
                    }
                    NMCLib20.NMC_MtGetPrfPos(axishandle[10], ref axis10CurrentPosition_Pulse);
                    _axis10CurrentPosition_mm = axis10CurrentPosition_Pulse * _axis10mmPP;
                    axis10CurrentDeviationValue_mm = (axis10CurrentPosition_Pulse - axis10TargetPosition_Pulse) * _axis10mmPP;
                    if (_axis10HomedFlag && _axis10MoveStartFlag && ((_axis10Command >= 1 && _axis10Command <= 900) || _axis10Command == 1000) &&
                        (!_axis10ELMinus && !_axis10ELPlus && axis10CurrentDeviationValue_mm >= axis10TargetDeviationValue_mm * -1 && axis10CurrentDeviationValue_mm <= axis10TargetDeviationValue_mm))
                    {
                        _axis10CurrentPositionNumber = _axis10Command;
                        _axis10Command = 0;
                        _axis10CommandSaved = 0;
                        _axis10MoveStartFlag = false;
                    }
                    NMCLib20.NMC_MtGetPrfPos(axishandle[11], ref axis11CurrentPosition_Pulse);
                    _axis11CurrentPosition_mm = axis11CurrentPosition_Pulse * _axis11mmPP;
                    axis11CurrentDeviationValue_mm = (axis11CurrentPosition_Pulse - axis11TargetPosition_Pulse) * _axis11mmPP;
                    if (_axis11HomedFlag && _axis11MoveStartFlag && ((_axis11Command >= 1 && _axis11Command <= 900) || _axis11Command == 1100) &&
                        (!_axis11ELMinus && !_axis11ELPlus && axis11CurrentDeviationValue_mm >= axis11TargetDeviationValue_mm * -1 && axis11CurrentDeviationValue_mm <= axis11TargetDeviationValue_mm))
                    {
                        _axis11CurrentPositionNumber = _axis11Command;
                        _axis11Command = 0;
                        _axis11CommandSaved = 0;
                        _axis11MoveStartFlag = false;
                    }
                    //Inch
                    if (_axis08CommandSaved == 901 && _axis08CheckDoneStatus == 1)
                    {
                        _axis08Command = 0;
                        _axis08CommandSaved = 0;
                    }
                    if (_axis09CommandSaved == 901 && _axis09CheckDoneStatus == 1)
                    {
                        _axis09Command = 0;
                        _axis09CommandSaved = 0;
                    }
                    if (_axis10CommandSaved == 901 && _axis10CheckDoneStatus == 1)
                    {
                        _axis10Command = 0;
                        _axis10CommandSaved = 0;
                    }
                    if (_axis11CommandSaved == 901 && _axis11CheckDoneStatus == 1)
                    {
                        _axis11Command = 0;
                        _axis11CommandSaved = 0;
                    }

                    //ADD 202210151953 Pause
                    if (((_axis08Command >= 1 && _axis08Command <= 900) || _axis08Command == 1000) && _axis08PauseRequestFlag)
                    {
                        _axis08PauseRequestFlag = false;
                        _axis08ResumeRequestFlag = false;
                        axisImmediateStop(8);
                        _axis08PausedFlag = true;
                    }
                    if (((_axis09Command >= 1 && _axis09Command <= 900) || _axis09Command == 1000) && _axis09PauseRequestFlag)
                    {
                        _axis09PauseRequestFlag = false;
                        _axis09ResumeRequestFlag = false;
                        axisImmediateStop(9);
                        _axis09PausedFlag = true;
                    }
                    if (((_axis10Command >= 1 && _axis10Command <= 900) || _axis10Command == 1000) && _axis10PauseRequestFlag)
                    {
                        _axis10PauseRequestFlag = false;
                        _axis10ResumeRequestFlag = false;
                        axisImmediateStop(10);
                        _axis10PausedFlag = true;
                    }
                    if (((_axis11Command >= 1 && _axis11Command <= 900) || _axis11Command == 1000) && _axis11PauseRequestFlag)
                    {
                        _axis11PauseRequestFlag = false;
                        _axis11ResumeRequestFlag = false;
                        axisImmediateStop(11);
                        _axis11PausedFlag = true;
                    }
                    if (_axis08PausedFlag && _axis08ResumeRequestFlag && !_axis08PauseRequestFlag)
                    {
                        AxisAction(_axis08_axisCommand_SaveForPause, _axis08_axisNumber_SaveForPause, _axis08_startVelocity_SaveForPause,
                            _axis08_maxVelocity_SaveForPause, _axis08_stopVelocity_SaveForPause, _axis08_accTime_SaveForPause,
                            _axis08_decTime_SaveForPause, _axis08_targetPosition_SaveForPause, _axis08_jogDirection_SaveForPause,
                            _axis08_inch_SaveForPause, _axis08_deviationValue_SaveForPause, _axis08_useSModeMoveFlag_SaveForPause, _axis08_sMoveTime_SaveForPause);
                        _axis08ResumeRequestFlag = false;
                        _axis08PausedFlag = false;
                    }
                    if (_axis09PausedFlag && _axis09ResumeRequestFlag && !_axis09PauseRequestFlag)
                    {
                        AxisAction(_axis09_axisCommand_SaveForPause, _axis09_axisNumber_SaveForPause, _axis09_startVelocity_SaveForPause,
                            _axis09_maxVelocity_SaveForPause, _axis09_stopVelocity_SaveForPause, _axis09_accTime_SaveForPause,
                            _axis09_decTime_SaveForPause, _axis09_targetPosition_SaveForPause, _axis09_jogDirection_SaveForPause,
                            _axis09_inch_SaveForPause, _axis09_deviationValue_SaveForPause, _axis09_useSModeMoveFlag_SaveForPause, _axis09_sMoveTime_SaveForPause);
                        _axis09ResumeRequestFlag = false;
                        _axis09PausedFlag = false;
                    }
                    if (_axis10PausedFlag && _axis10ResumeRequestFlag && !_axis10PauseRequestFlag)
                    {
                        AxisAction(_axis10_axisCommand_SaveForPause, _axis10_axisNumber_SaveForPause, _axis10_startVelocity_SaveForPause,
                            _axis10_maxVelocity_SaveForPause, _axis10_stopVelocity_SaveForPause, _axis10_accTime_SaveForPause,
                            _axis10_decTime_SaveForPause, _axis10_targetPosition_SaveForPause, _axis10_jogDirection_SaveForPause,
                            _axis10_inch_SaveForPause, _axis10_deviationValue_SaveForPause, _axis10_useSModeMoveFlag_SaveForPause, _axis10_sMoveTime_SaveForPause);
                        _axis10ResumeRequestFlag = false;
                        _axis10PausedFlag = false;
                    }
                    if (_axis11PausedFlag && _axis11ResumeRequestFlag && !_axis11PauseRequestFlag)
                    {
                        AxisAction(_axis11_axisCommand_SaveForPause, _axis11_axisNumber_SaveForPause, _axis11_startVelocity_SaveForPause,
                            _axis11_maxVelocity_SaveForPause, _axis11_stopVelocity_SaveForPause, _axis11_accTime_SaveForPause,
                            _axis11_decTime_SaveForPause, _axis11_targetPosition_SaveForPause, _axis11_jogDirection_SaveForPause,
                            _axis11_inch_SaveForPause, _axis11_deviationValue_SaveForPause, _axis11_useSModeMoveFlag_SaveForPause, _axis11_sMoveTime_SaveForPause);
                        _axis11ResumeRequestFlag = false;
                        _axis11PausedFlag = false;
                    }
                }

                //读取IO状态
                //读取输入IO状态
                for (int i = 0; i < _cardQuantity; i++)
                {
                    for (int j = 0; j < _motionCardIOMaxQuantity_320; j++)
                    {
                        _inPutStatus[i][j] = getInPut(i * _motionCardIOMaxQuantity_320 + j);
                    }
                }
                if (_cardQuantity >= 1)
                {
                    int axis = 0;
                    NMCLib20.NMC_MtGetMotionIO(axishandle[0], ref axis);
                    _axis00ELMinus = (axis & (1 << 0)) == 0 ? false : true;
                    _axis00ELPlus = (axis & (1 << 1)) == 0 ? false : true;
                    _axis00ORG = (axis & (1 << 2)) == 0 ? true : false;
                    _axis00STP = false;
                    _axis00STA = false;
                    _axis00SDMinus = false;
                    _axis00SDPlus = false;
                    _axis00Reserved = false;

                    NMCLib20.NMC_MtGetMotionIO(axishandle[1], ref axis);
                    _axis01ELMinus = (axis & (1 << 0)) == 0 ? false : true;
                    _axis01ELPlus = (axis & (1 << 1)) == 0 ? false : true;
                    _axis01ORG = (axis & (1 << 2)) == 0 ? true : false;
                    _axis01STP = false;
                    _axis01STA = false;
                    _axis01SDMinus = false;
                    _axis01SDPlus = false;
                    _axis01Reserved = false;

                    NMCLib20.NMC_MtGetMotionIO(axishandle[2], ref axis);
                    _axis02ELMinus = (axis & (1 << 0)) == 0 ? false : true;
                    _axis02ELPlus = (axis & (1 << 1)) == 0 ? false : true;
                    _axis02ORG = (axis & (1 << 2)) == 0 ? true : false;
                    _axis02STP = false;
                    _axis02STA = false;
                    _axis02SDMinus = false;
                    _axis02SDPlus = false;
                    _axis02Reserved = false;

                    NMCLib20.NMC_MtGetMotionIO(axishandle[3], ref axis);
                    _axis03ELMinus = (axis & (1 << 0)) == 0 ? false : true;
                    _axis03ELPlus = (axis & (1 << 1)) == 0 ? false : true;
                    _axis03ORG = (axis & (1 << 2)) == 0 ? true : false;
                    _axis03STP = false;
                    _axis03STA = false;
                    _axis03SDMinus = false;
                    _axis03SDPlus = false;
                    _axis03Reserved = false;
                }
                if (_cardQuantity >= 2)
                {
                    int axis = 0;
                    NMCLib20.NMC_MtGetMotionIO(axishandle[4], ref axis);
                    _axis04ELMinus = (axis & (1 << 0)) == 0 ? false : true;
                    _axis04ELPlus = (axis & (1 << 1)) == 0 ? false : true;
                    _axis04ORG = (axis & (1 << 2)) == 0 ? true : false;
                    _axis04STP = false;
                    _axis04STA = false;
                    _axis04SDMinus = false;
                    _axis04SDPlus = false;
                    _axis04Reserved = false;

                    NMCLib20.NMC_MtGetMotionIO(axishandle[5], ref axis);
                    _axis05ELMinus = (axis & (1 << 0)) == 0 ? false : true;
                    _axis05ELPlus = (axis & (1 << 1)) == 0 ? false : true;
                    _axis05ORG = (axis & (1 << 2)) == 0 ? true : false;
                    _axis05STP = false;
                    _axis05STA = false;
                    _axis05SDMinus = false;
                    _axis05SDPlus = false;
                    _axis05Reserved = false;

                    NMCLib20.NMC_MtGetMotionIO(axishandle[6], ref axis);
                    _axis06ELMinus = (axis & (1 << 0)) == 0 ? false : true;
                    _axis06ELPlus = (axis & (1 << 1)) == 0 ? false : true;
                    _axis06ORG = (axis & (1 << 2)) == 0 ? true : false;
                    _axis06STP = false;
                    _axis06STA = false;
                    _axis06SDMinus = false;
                    _axis06SDPlus = false;
                    _axis06Reserved = false;

                    NMCLib20.NMC_MtGetMotionIO(axishandle[7], ref axis);
                    _axis07ELMinus = (axis & (1 << 0)) == 0 ? false : true;
                    _axis07ELPlus = (axis & (1 << 1)) == 0 ? false : true;
                    _axis07ORG = (axis & (1 << 2)) == 0 ? true : false;
                    _axis07STP = false;
                    _axis07STA = false;
                    _axis07SDMinus = false;
                    _axis07SDPlus = false;
                    _axis07Reserved = false;
                }
                if (_cardQuantity >= 3)
                {
                    int axis = 0;
                    NMCLib20.NMC_MtGetMotionIO(axishandle[8], ref axis);
                    _axis08ELMinus = (axis & (1 << 0)) == 0 ? false : true;
                    _axis08ELPlus = (axis & (1 << 1)) == 0 ? false : true;
                    _axis08ORG = (axis & (1 << 2)) == 0 ? true : false;
                    _axis08STP = false;
                    _axis08STA = false;
                    _axis08SDMinus = false;
                    _axis08SDPlus = false;
                    _axis08Reserved = false;

                    NMCLib20.NMC_MtGetMotionIO(axishandle[9], ref axis);
                    _axis09ELMinus = (axis & (1 << 0)) == 0 ? false : true;
                    _axis09ELPlus = (axis & (1 << 1)) == 0 ? false : true;
                    _axis09ORG = (axis & (1 << 2)) == 0 ? true : false;
                    _axis09STP = false;
                    _axis09STA = false;
                    _axis09SDMinus = false;
                    _axis09SDPlus = false;
                    _axis09Reserved = false;

                    NMCLib20.NMC_MtGetMotionIO(axishandle[10], ref axis);
                    _axis10ELMinus = (axis & (1 << 0)) == 0 ? false : true;
                    _axis10ELPlus = (axis & (1 << 1)) == 0 ? false : true;
                    _axis10ORG = (axis & (1 << 2)) == 0 ? true : false;
                    _axis10STP = false;
                    _axis10STA = false;
                    _axis10SDMinus = false;
                    _axis10SDPlus = false;
                    _axis10Reserved = false;

                    NMCLib20.NMC_MtGetMotionIO(axishandle[11], ref axis);
                    _axis11ELMinus = (axis & (1 << 0)) == 0 ? false : true;
                    _axis11ELPlus = (axis & (1 << 1)) == 0 ? false : true;
                    _axis11ORG = (axis & (1 << 2)) == 0 ? true : false;
                    _axis11STP = false;
                    _axis11STA = false;
                    _axis11SDMinus = false;
                    _axis11SDPlus = false;
                    _axis11Reserved = false;
                }

                //读取输出IO状态
                for (int i = 0; i < _cardQuantity; i++)
                {
                    for (int j = 0; j < _motionCardIOMaxQuantity_320; j++)
                    {
                        _outPutStatus[i][j] = getOutPut(i * _motionCardIOMaxQuantity_320 + j);
                    }
                }

                if (_axisQuantity >= 1)
                {
                    short axsists = 0;
                    NMCLib20.NMC_MtGetSts(axishandle[0], ref axsists);
                    _axis00ServoOnOffFlag = (axsists & (1 << 3)) == 0 ? false : true;//取bit3,是否使能中
                    if (!_axis00ServoOnOffFlag)
                        _axis00HomedFlag = false;

                    //Axis00 Alarm
                    if ((axsists & (1 << 10)) == 0 ? true : false)//取bit10,驱动器是否报警
                        _axis00ErrorNumber = 0;
                    else
                    {
                        _axis00ErrorNumber = 1;
                        axisDecelStop(0);
                        _axis00HomedFlag = false;
                        _axis00Command = 0;
                        _axis00CommandSaved = 0;
                    }

                    //增加在Jog时,因为其它情况导致的停止(例如极限传感器)
                    if (_axis00CommandSaved == 901 && (_axis00ELMinus || _axis00ELPlus))
                    {
                        jogAxisImmediateStop(0);
                    }
                }

                if (_axisQuantity >= 2)
                {
                    short axsists = 0;
                    NMCLib20.NMC_MtGetSts(axishandle[1], ref axsists);
                    _axis01ServoOnOffFlag = (axsists & (1 << 3)) == 0 ? false : true;//取bit3,是否使能中
                    if (!_axis01ServoOnOffFlag)
                        _axis01HomedFlag = false;

                    //Axis01 Alarm
                    if ((axsists & (1 << 10)) == 0 ? true : false)//取bit10,驱动器是否报警
                        _axis01ErrorNumber = 0;
                    else
                    {
                        _axis01ErrorNumber = 1;
                        axisDecelStop(1);
                        _axis01HomedFlag = false;
                        _axis01Command = 0;
                        _axis01CommandSaved = 0;
                    }

                    //增加在Jog时,因为其它情况导致的停止(例如极限传感器)
                    if (_axis01CommandSaved == 901 && (_axis01ELMinus || _axis01ELPlus))
                    {
                        jogAxisImmediateStop(1);
                    }
                }

                if (_axisQuantity >= 3)
                {
                    short axsists = 0;
                    NMCLib20.NMC_MtGetSts(axishandle[2], ref axsists);
                    _axis02ServoOnOffFlag = (axsists & (1 << 3)) == 0 ? false : true;//取bit3,是否使能中
                    if (!_axis02ServoOnOffFlag)
                        _axis02HomedFlag = false;

                    //Axis02 Alarm
                    if ((axsists & (1 << 10)) == 0 ? true : false)//取bit10,驱动器是否报警
                        _axis02ErrorNumber = 0;
                    else
                    {
                        _axis02ErrorNumber = 1;
                        axisDecelStop(2);
                        _axis02HomedFlag = false;
                        _axis02Command = 0;
                        _axis02CommandSaved = 0;
                    }

                    //增加在Jog时,因为其它情况导致的停止(例如极限传感器)
                    if (_axis02CommandSaved == 901 && (_axis02ELMinus || _axis02ELPlus))
                    {
                        jogAxisImmediateStop(2);
                    }
                }

                if (_axisQuantity >= 4)
                {
                    short axsists = 0;
                    NMCLib20.NMC_MtGetSts(axishandle[3], ref axsists);
                    _axis03ServoOnOffFlag = (axsists & (1 << 3)) == 0 ? false : true;//取bit3,是否使能中
                    if (!_axis03ServoOnOffFlag)
                        _axis03HomedFlag = false;

                    //Axis03 Alarm
                    if ((axsists & (1 << 10)) == 0 ? true : false)//取bit10,驱动器是否报警
                        _axis03ErrorNumber = 0;
                    else
                    {
                        _axis03ErrorNumber = 1;
                        axisDecelStop(3);
                        _axis03HomedFlag = false;
                        _axis03Command = 0;
                        _axis03CommandSaved = 0;
                    }

                    //增加在Jog时,因为其它情况导致的停止(例如极限传感器)
                    if (_axis03CommandSaved == 901 && (_axis03ELMinus || _axis03ELPlus))
                    {
                        jogAxisImmediateStop(3);
                    }
                }

                if (_axisQuantity >= 5)
                {
                    short axsists = 0;
                    NMCLib20.NMC_MtGetSts(axishandle[4], ref axsists);
                    _axis04ServoOnOffFlag = (axsists & (1 << 3)) == 0 ? false : true;//取bit3,是否使能中
                    if (!_axis04ServoOnOffFlag)
                        _axis04HomedFlag = false;

                    //Axis04 Alarm
                    if ((axsists & (1 << 10)) == 0 ? true : false)//取bit10,驱动器是否报警
                        _axis04ErrorNumber = 0;
                    else
                    {
                        _axis04ErrorNumber = 1;
                        axisDecelStop(4);
                        _axis04HomedFlag = false;
                        _axis04Command = 0;
                        _axis04CommandSaved = 0;
                    }
                    //增加在Jog时,因为其它情况导致的停止(例如极限传感器)
                    if (_axis04CommandSaved == 901 && (_axis04ELMinus || _axis04ELPlus))
                    {
                        jogAxisImmediateStop(4);
                    }
                }

                if (_axisQuantity >= 6)
                {
                    short axsists = 0;
                    NMCLib20.NMC_MtGetSts(axishandle[5], ref axsists);
                    _axis05ServoOnOffFlag = (axsists & (1 << 3)) == 0 ? false : true;//取bit3,是否使能中
                    if (!_axis05ServoOnOffFlag)
                        _axis05HomedFlag = false;

                    //Axis05 Alarm
                    if ((axsists & (1 << 10)) == 0 ? true : false)//取bit10,驱动器是否报警
                        _axis05ErrorNumber = 0;
                    else
                    {
                        _axis05ErrorNumber = 1;
                        axisDecelStop(5);
                        _axis05HomedFlag = false;
                        _axis05Command = 0;
                        _axis05CommandSaved = 0;
                    }
                    //增加在Jog时,因为其它情况导致的停止(例如极限传感器)
                    if (_axis05CommandSaved == 901 && (_axis05ELMinus || _axis05ELPlus))
                    {
                        jogAxisImmediateStop(5);
                    }
                }

                if (_axisQuantity >= 7)
                {
                    short axsists = 0;
                    NMCLib20.NMC_MtGetSts(axishandle[6], ref axsists);
                    _axis06ServoOnOffFlag = (axsists & (1 << 3)) == 0 ? false : true;//取bit3,是否使能中
                    if (!_axis06ServoOnOffFlag)
                        _axis06HomedFlag = false;

                    //Axis06 Alarm
                    if ((axsists & (1 << 10)) == 0 ? true : false)//取bit10,驱动器是否报警
                        _axis06ErrorNumber = 0;
                    else
                    {
                        _axis06ErrorNumber = 1;
                        axisDecelStop(6);
                        _axis06HomedFlag = false;
                        _axis06Command = 0;
                        _axis06CommandSaved = 0;
                    }
                    //增加在Jog时,因为其它情况导致的停止(例如极限传感器)
                    if (_axis06CommandSaved == 901 && (_axis06ELMinus || _axis06ELPlus))
                    {
                        jogAxisImmediateStop(6);
                    }
                }

                if (_axisQuantity >= 8)
                {
                    short axsists = 0;
                    NMCLib20.NMC_MtGetSts(axishandle[7], ref axsists);
                    _axis07ServoOnOffFlag = (axsists & (1 << 3)) == 0 ? false : true;//取bit3,是否使能中
                    if (!_axis07ServoOnOffFlag)
                        _axis07HomedFlag = false;

                    //Axis07 Alarm
                    if ((axsists & (1 << 10)) == 0 ? true : false)//取bit10,驱动器是否报警
                        _axis07ErrorNumber = 0;
                    else
                    {
                        _axis07ErrorNumber = 1;
                        axisDecelStop(7);
                        _axis07HomedFlag = false;
                        _axis07Command = 0;
                        _axis07CommandSaved = 0;
                    }
                    //增加在Jog时,因为其它情况导致的停止(例如极限传感器)
                    if (_axis07CommandSaved == 901 && (_axis07ELMinus || _axis07ELPlus))
                    {
                        jogAxisImmediateStop(7);
                    }
                }

                if (_axisQuantity >= 9)
                {
                    short axsists = 0;
                    NMCLib20.NMC_MtGetSts(axishandle[8], ref axsists);
                    _axis08ServoOnOffFlag = (axsists & (1 << 3)) == 0 ? false : true;//取bit3,是否使能中
                    if (!_axis08ServoOnOffFlag)
                        _axis08HomedFlag = false;

                    //Axis08 Alarm
                    if ((axsists & (1 << 10)) == 0 ? true : false)//取bit10,驱动器是否报警
                        _axis08ErrorNumber = 0;
                    else
                    {
                        _axis08ErrorNumber = 1;
                        axisDecelStop(8);
                        _axis08HomedFlag = false;
                        _axis08Command = 0;
                        _axis08CommandSaved = 0;
                    }
                    //增加在Jog时,因为其它情况导致的停止(例如极限传感器)
                    if (_axis08CommandSaved == 901 && (_axis08ELMinus || _axis08ELPlus))
                    {
                        jogAxisImmediateStop(8);
                    }
                }

                if (_axisQuantity >= 10)
                {
                    short axsists = 0;
                    NMCLib20.NMC_MtGetSts(axishandle[9], ref axsists);
                    _axis09ServoOnOffFlag = (axsists & (1 << 3)) == 0 ? false : true;//取bit3,是否使能中
                    if (!_axis09ServoOnOffFlag)
                        _axis09HomedFlag = false;

                    //Axis09 Alarm
                    if ((axsists & (1 << 10)) == 0 ? true : false)//取bit10,驱动器是否报警
                        _axis09ErrorNumber = 0;
                    else
                    {
                        _axis09ErrorNumber = 1;
                        axisDecelStop(9);
                        _axis09HomedFlag = false;
                        _axis09Command = 0;
                        _axis09CommandSaved = 0;
                    }
                    //增加在Jog时,因为其它情况导致的停止(例如极限传感器)
                    if (_axis09CommandSaved == 901 && (_axis09ELMinus || _axis09ELPlus))
                    {
                        jogAxisImmediateStop(9);
                    }
                }

                if (_axisQuantity >= 11)
                {
                    short axsists = 0;
                    NMCLib20.NMC_MtGetSts(axishandle[10], ref axsists);
                    _axis10ServoOnOffFlag = (axsists & (1 << 3)) == 0 ? false : true;//取bit3,是否使能中
                    if (!_axis10ServoOnOffFlag)
                        _axis10HomedFlag = false;

                    //Axis010 Alarm
                    if ((axsists & (1 << 10)) == 0 ? true : false)//取bit10,驱动器是否报警
                        _axis10ErrorNumber = 0;
                    else
                    {
                        _axis10ErrorNumber = 1;
                        axisDecelStop(10);
                        _axis10HomedFlag = false;
                        _axis10Command = 0;
                        _axis10CommandSaved = 0;
                    }
                    //增加在Jog时,因为其它情况导致的停止(例如极限传感器)
                    if (_axis10CommandSaved == 1001 && (_axis10ELMinus || _axis10ELPlus))
                    {
                        jogAxisImmediateStop(10);
                    }
                }

                if (_axisQuantity >= 12)
                {
                    short axsists = 0;
                    NMCLib20.NMC_MtGetSts(axishandle[11], ref axsists);
                    _axis11ServoOnOffFlag = (axsists & (1 << 3)) == 0 ? false : true;//取bit3,是否使能中
                    if (!_axis11ServoOnOffFlag)
                        _axis11HomedFlag = false;

                    //Axis11 Alarm
                    if ((axsists & (1 << 10)) == 0 ? true : false)//取bit10,驱动器是否报警
                        _axis11ErrorNumber = 0;
                    else
                    {
                        _axis11ErrorNumber = 1;
                        axisDecelStop(11);
                        _axis11HomedFlag = false;
                        _axis11Command = 0;
                        _axis11CommandSaved = 0;
                    }
                    //增加在Jog时,因为其它情况导致的停止(例如极限传感器)
                    if (_axis11CommandSaved == 1101 && (_axis11ELMinus || _axis11ELPlus))
                    {
                        jogAxisImmediateStop(11);
                    }
                }
                #region 报警复位信号自动OFF
                //报警复位信号自动OFF
                if (_axisQuantity >= 1)
                {
                    if (_axis00ServoAlarmClearFlag)
                        _axis00resetOutputOnCount++;
                    if (_axis00resetOutputOnCount > 20)
                    {
                        NMCLib20.NMC_MtSetSvClr(axishandle[0], 1);
                        _axis00resetOutputOnCount = 0;
                        _axis00ServoAlarmClearFlag = false;
                        NMCLib20.NMC_MtClrError(axishandle[0]);//清除轴错误状态
                    }
                }
                if (_axisQuantity >= 2)
                {
                    if (_axis01ServoAlarmClearFlag)
                        _axis01resetOutputOnCount++;
                    if (_axis01resetOutputOnCount > 20)
                    {
                        NMCLib20.NMC_MtSetSvClr(axishandle[1], 1);
                        _axis01resetOutputOnCount = 0;
                        _axis01ServoAlarmClearFlag = false;
                        NMCLib20.NMC_MtClrError(axishandle[1]);//清除轴错误状态
                    }
                }
                if (_axisQuantity >= 3)
                {
                    if (_axis02ServoAlarmClearFlag)
                        _axis02resetOutputOnCount++;
                    if (_axis02resetOutputOnCount > 20)
                    {
                        NMCLib20.NMC_MtSetSvClr(axishandle[2], 1);
                        _axis02resetOutputOnCount = 0;
                        _axis02ServoAlarmClearFlag = false;
                        NMCLib20.NMC_MtClrError(axishandle[2]);//清除轴错误状态
                    }
                }
                if (_axisQuantity >= 4)
                {
                    if (_axis03ServoAlarmClearFlag)
                        _axis03resetOutputOnCount++;
                    if (_axis03resetOutputOnCount > 20)
                    {
                        NMCLib20.NMC_MtSetSvClr(axishandle[3], 1);
                        _axis03resetOutputOnCount = 0;
                        _axis03ServoAlarmClearFlag = false;
                        NMCLib20.NMC_MtClrError(axishandle[3]);//清除轴错误状态
                    }
                }
                if (_axisQuantity >= 5)
                {
                    if (_axis04ServoAlarmClearFlag)
                        _axis04resetOutputOnCount++;
                    if (_axis04resetOutputOnCount > 20)
                    {
                        NMCLib20.NMC_MtSetSvClr(axishandle[4], 1);
                        _axis04resetOutputOnCount = 0;
                        _axis04ServoAlarmClearFlag = false;
                        NMCLib20.NMC_MtClrError(axishandle[4]);//清除轴错误状态
                    }
                }
                if (_axisQuantity >= 6)
                {
                    if (_axis05ServoAlarmClearFlag)
                        _axis05resetOutputOnCount++;
                    if (_axis05resetOutputOnCount > 20)
                    {
                        NMCLib20.NMC_MtSetSvClr(axishandle[5], 1);
                        _axis05resetOutputOnCount = 0;
                        _axis05ServoAlarmClearFlag = false;
                        NMCLib20.NMC_MtClrError(axishandle[5]);//清除轴错误状态
                    }
                }
                if (_axisQuantity >= 7)
                {
                    if (_axis06ServoAlarmClearFlag)
                        _axis06resetOutputOnCount++;
                    if (_axis06resetOutputOnCount > 20)
                    {
                        NMCLib20.NMC_MtSetSvClr(axishandle[6], 1);
                        _axis06resetOutputOnCount = 0;
                        _axis06ServoAlarmClearFlag = false;
                        NMCLib20.NMC_MtClrError(axishandle[6]);//清除轴错误状态
                    }
                }
                if (_axisQuantity >= 8)
                {
                    if (_axis07ServoAlarmClearFlag)
                        _axis07resetOutputOnCount++;
                    if (_axis07resetOutputOnCount > 20)
                    {
                        NMCLib20.NMC_MtSetSvClr(axishandle[7], 1);
                        _axis07resetOutputOnCount = 0;
                        _axis07ServoAlarmClearFlag = false;
                        NMCLib20.NMC_MtClrError(axishandle[7]);//清除轴错误状态
                    }
                }
                if (_axisQuantity >= 9)
                {
                    if (_axis08ServoAlarmClearFlag)
                        _axis08resetOutputOnCount++;
                    if (_axis08resetOutputOnCount > 20)
                    {
                        NMCLib20.NMC_MtSetSvClr(axishandle[8], 1);
                        _axis08resetOutputOnCount = 0;
                        _axis08ServoAlarmClearFlag = false;
                        NMCLib20.NMC_MtClrError(axishandle[8]);//清除轴错误状态
                    }
                }
                if (_axisQuantity >= 10)
                {
                    if (_axis09ServoAlarmClearFlag)
                        _axis09resetOutputOnCount++;
                    if (_axis09resetOutputOnCount > 20)
                    {
                        NMCLib20.NMC_MtSetSvClr(axishandle[9], 1);
                        _axis09resetOutputOnCount = 0;
                        _axis09ServoAlarmClearFlag = false;
                        NMCLib20.NMC_MtClrError(axishandle[9]);//清除轴错误状态
                    }
                }
                if (_axisQuantity >= 11)
                {
                    if (_axis10ServoAlarmClearFlag)
                        _axis10resetOutputOnCount++;
                    if (_axis10resetOutputOnCount > 20)
                    {
                        NMCLib20.NMC_MtSetSvClr(axishandle[10], 1);
                        _axis10resetOutputOnCount = 0;
                        _axis10ServoAlarmClearFlag = false;
                        NMCLib20.NMC_MtClrError(axishandle[10]);//清除轴错误状态
                    }
                }
                if (_axisQuantity >= 12)
                {
                    if (_axis11ServoAlarmClearFlag)
                        _axis11resetOutputOnCount++;
                    if (_axis11resetOutputOnCount > 20)
                    {
                        NMCLib20.NMC_MtSetSvClr(axishandle[11], 1);
                        _axis11resetOutputOnCount = 0;
                        _axis11ServoAlarmClearFlag = false;
                        NMCLib20.NMC_MtClrError(axishandle[11]);//清除轴错误状态
                    }
                }
                #endregion

                Thread.Sleep(5);
            }
        }
        #endregion

        #region axisAction:轴运行(axisCommand取值范围 0:NA;1-900:点位号定位;901:Jogging;999:Homing;1000:实时位置数据定位;1001:减速停止;1002:立即停止)
        /// <summary>
        /// axisAction:轴运行
        /// </summary>
        /// <param name="axisCommand">
        /// <param name="axisNumber">int:轴号0-N</param>
        /// <param name="startVelocity">float:运动初始速度,单位:mmps</param>
        /// <param name="maxVelocity">float:运动速度,负值表示往负方向找原点/Jog,正值表示往正方向找原点/Jog,单位:mmps</param>
        /// <param name="stopVelocity">float:停止速度，单位mmps</param>----不使用设定为0
        /// <param name="accTime">double:加速时间,单位:s</param>
        /// <param name="decTime">double:减速时间，单位：s</param>----不使用设定为0
        /// <param name="targetPosition">float:目标位置,单位:mm</param>
        /// <param name="jogDirection">bool:Jog方向,false/true(负向/正向)</param>
        /// <param name="inch">bool:点动/寸动,false/true(点动/寸动)</param>
        /// <param name="deviationValue">float:定位偏差值,单位:mm</param>
        /// <param name="useSModeMoveFlag">bool:是否使用S形速度曲线移动，false-不采用，true-采用</param>----不使用设定为false
        /// <param name="sMoveTime">double：当选择采用S形速度曲线时，S曲线的时间，单位s，在0-0.5之间</param>----不使用设定为0
        public void AxisAction(int axisCommand, int axisNumber, float startVelocity, float maxVelocity, float stopVelocity, double accTime, double decTime, float targetPosition, bool jogDirection, bool inch, float deviationValue, bool useSModeMoveFlag, double sMoveTime)
        {
            switch (axisNumber)
            {
                case 0:
                    //ADD 202210151953 Pause
                    _axis00_axisCommand_SaveForPause = axisCommand;
                    _axis00_axisNumber_SaveForPause = axisNumber;
                    _axis00_startVelocity_SaveForPause = startVelocity;
                    _axis00_maxVelocity_SaveForPause = maxVelocity;
                    _axis00_stopVelocity_SaveForPause = stopVelocity;
                    _axis00_accTime_SaveForPause = accTime;
                    _axis00_decTime_SaveForPause = decTime;
                    _axis00_targetPosition_SaveForPause = targetPosition;
                    _axis00_jogDirection_SaveForPause = jogDirection;
                    _axis00_inch_SaveForPause = inch;
                    _axis00_deviationValue_SaveForPause = deviationValue;
                    _axis00_useSModeMoveFlag_SaveForPause = useSModeMoveFlag;
                    _axis00_sMoveTime_SaveForPause = sMoveTime;
                    _axis00PauseRequestFlag = false;
                    _axis00ResumeRequestFlag = false;
                    _axis00PausedFlag = false;

                    _axis00Command = axisCommand;
                    if (_axis00CommandSaved != 999)
                        _axis00CommandSaved = _axis00Command;
                    _axis00CurrentPositionNumber = 0;
                    axis00TargetStartVelocity_mmps = startVelocity;
                    axis00TargetMaxVelocity_mmps = maxVelocity;
                    axis00TargetAccTime_s = accTime;
                    axis00TargetPosition_mm = targetPosition;
                    axis00TargetDeviationValue_mm = deviationValue;
                    axis00StartVelocity_Temp_pps = Convert.ToInt32(axis00TargetStartVelocity_mmps / _axis00mmPP);
                    axis00MaxVelocity_Temp_pps = Convert.ToInt32(axis00TargetMaxVelocity_mmps / _axis00mmPP);
                    break;
                case 1:
                    //ADD 202210151953 Pause
                    _axis01_axisCommand_SaveForPause = axisCommand;
                    _axis01_axisNumber_SaveForPause = axisNumber;
                    _axis01_startVelocity_SaveForPause = startVelocity;
                    _axis01_maxVelocity_SaveForPause = maxVelocity;
                    _axis01_stopVelocity_SaveForPause = stopVelocity;
                    _axis01_accTime_SaveForPause = accTime;
                    _axis01_decTime_SaveForPause = decTime;
                    _axis01_targetPosition_SaveForPause = targetPosition;
                    _axis01_jogDirection_SaveForPause = jogDirection;
                    _axis01_inch_SaveForPause = inch;
                    _axis01_deviationValue_SaveForPause = deviationValue;
                    _axis01_useSModeMoveFlag_SaveForPause = useSModeMoveFlag;
                    _axis01_sMoveTime_SaveForPause = sMoveTime;
                    _axis01PauseRequestFlag = false;
                    _axis01ResumeRequestFlag = false;
                    _axis01PausedFlag = false;

                    _axis01Command = axisCommand;
                    if (_axis01CommandSaved != 999)
                        _axis01CommandSaved = _axis01Command;
                    _axis01CurrentPositionNumber = 0;
                    axis01TargetStartVelocity_mmps = startVelocity;
                    axis01TargetMaxVelocity_mmps = maxVelocity;
                    axis01TargetAccTime_s = accTime;
                    axis01TargetPosition_mm = targetPosition;
                    axis01TargetDeviationValue_mm = deviationValue;
                    axis01StartVelocity_Temp_pps = Convert.ToInt32(axis01TargetStartVelocity_mmps / _axis01mmPP);
                    axis01MaxVelocity_Temp_pps = Convert.ToInt32(axis01TargetMaxVelocity_mmps / _axis01mmPP);
                    break;
                case 2:
                    //ADD 202210151953 Pause
                    _axis02_axisCommand_SaveForPause = axisCommand;
                    _axis02_axisNumber_SaveForPause = axisNumber;
                    _axis02_startVelocity_SaveForPause = startVelocity;
                    _axis02_maxVelocity_SaveForPause = maxVelocity;
                    _axis02_stopVelocity_SaveForPause = stopVelocity;
                    _axis02_accTime_SaveForPause = accTime;
                    _axis02_decTime_SaveForPause = decTime;
                    _axis02_targetPosition_SaveForPause = targetPosition;
                    _axis02_jogDirection_SaveForPause = jogDirection;
                    _axis02_inch_SaveForPause = inch;
                    _axis02_deviationValue_SaveForPause = deviationValue;
                    _axis02_useSModeMoveFlag_SaveForPause = useSModeMoveFlag;
                    _axis02_sMoveTime_SaveForPause = sMoveTime;
                    _axis02PauseRequestFlag = false;
                    _axis02ResumeRequestFlag = false;
                    _axis02PausedFlag = false;

                    _axis02Command = axisCommand;
                    if (_axis02CommandSaved != 999)
                        _axis02CommandSaved = _axis02Command;
                    _axis02CurrentPositionNumber = 0;
                    axis02TargetStartVelocity_mmps = startVelocity;
                    axis02TargetMaxVelocity_mmps = maxVelocity;
                    axis02TargetAccTime_s = accTime;
                    axis02TargetPosition_mm = targetPosition;
                    axis02TargetDeviationValue_mm = deviationValue;
                    axis02StartVelocity_Temp_pps = Convert.ToInt32(axis02TargetStartVelocity_mmps / _axis02mmPP);
                    axis02MaxVelocity_Temp_pps = Convert.ToInt32(axis02TargetMaxVelocity_mmps / _axis02mmPP);
                    break;
                case 3:
                    //ADD 202210151953 Pause
                    _axis03_axisCommand_SaveForPause = axisCommand;
                    _axis03_axisNumber_SaveForPause = axisNumber;
                    _axis03_startVelocity_SaveForPause = startVelocity;
                    _axis03_maxVelocity_SaveForPause = maxVelocity;
                    _axis03_stopVelocity_SaveForPause = stopVelocity;
                    _axis03_accTime_SaveForPause = accTime;
                    _axis03_decTime_SaveForPause = decTime;
                    _axis03_targetPosition_SaveForPause = targetPosition;
                    _axis03_jogDirection_SaveForPause = jogDirection;
                    _axis03_inch_SaveForPause = inch;
                    _axis03_deviationValue_SaveForPause = deviationValue;
                    _axis03_useSModeMoveFlag_SaveForPause = useSModeMoveFlag;
                    _axis03_sMoveTime_SaveForPause = sMoveTime;
                    _axis03PauseRequestFlag = false;
                    _axis03ResumeRequestFlag = false;
                    _axis03PausedFlag = false;

                    _axis03Command = axisCommand;
                    if (_axis03CommandSaved != 999)
                        _axis03CommandSaved = _axis03Command;
                    _axis03CurrentPositionNumber = 0;
                    axis03TargetStartVelocity_mmps = startVelocity;
                    axis03TargetMaxVelocity_mmps = maxVelocity;
                    axis03TargetAccTime_s = accTime;
                    axis03TargetPosition_mm = targetPosition;
                    axis03TargetDeviationValue_mm = deviationValue;
                    axis03StartVelocity_Temp_pps = Convert.ToInt32(axis03TargetStartVelocity_mmps / _axis03mmPP);
                    axis03MaxVelocity_Temp_pps = Convert.ToInt32(axis03TargetMaxVelocity_mmps / _axis03mmPP);
                    break;
                case 4:
                    //ADD 202210151953 Pause
                    _axis04_axisCommand_SaveForPause = axisCommand;
                    _axis04_axisNumber_SaveForPause = axisNumber;
                    _axis04_startVelocity_SaveForPause = startVelocity;
                    _axis04_maxVelocity_SaveForPause = maxVelocity;
                    _axis04_stopVelocity_SaveForPause = stopVelocity;
                    _axis04_accTime_SaveForPause = accTime;
                    _axis04_decTime_SaveForPause = decTime;
                    _axis04_targetPosition_SaveForPause = targetPosition;
                    _axis04_jogDirection_SaveForPause = jogDirection;
                    _axis04_inch_SaveForPause = inch;
                    _axis04_deviationValue_SaveForPause = deviationValue;
                    _axis04_useSModeMoveFlag_SaveForPause = useSModeMoveFlag;
                    _axis04_sMoveTime_SaveForPause = sMoveTime;
                    _axis04PauseRequestFlag = false;
                    _axis04ResumeRequestFlag = false;
                    _axis04PausedFlag = false;

                    _axis04Command = axisCommand;
                    if (_axis04CommandSaved != 999)
                        _axis04CommandSaved = _axis04Command;
                    _axis04CurrentPositionNumber = 0;
                    axis04TargetStartVelocity_mmps = startVelocity;
                    axis04TargetMaxVelocity_mmps = maxVelocity;
                    axis04TargetAccTime_s = accTime;
                    axis04TargetPosition_mm = targetPosition;
                    axis04TargetDeviationValue_mm = deviationValue;
                    axis04StartVelocity_Temp_pps = Convert.ToInt32(axis04TargetStartVelocity_mmps / _axis04mmPP);
                    axis04MaxVelocity_Temp_pps = Convert.ToInt32(axis04TargetMaxVelocity_mmps / _axis04mmPP);
                    break;
                case 5:
                    //ADD 202210151953 Pause
                    _axis05_axisCommand_SaveForPause = axisCommand;
                    _axis05_axisNumber_SaveForPause = axisNumber;
                    _axis05_startVelocity_SaveForPause = startVelocity;
                    _axis05_maxVelocity_SaveForPause = maxVelocity;
                    _axis05_stopVelocity_SaveForPause = stopVelocity;
                    _axis05_accTime_SaveForPause = accTime;
                    _axis05_decTime_SaveForPause = decTime;
                    _axis05_targetPosition_SaveForPause = targetPosition;
                    _axis05_jogDirection_SaveForPause = jogDirection;
                    _axis05_inch_SaveForPause = inch;
                    _axis05_deviationValue_SaveForPause = deviationValue;
                    _axis05_useSModeMoveFlag_SaveForPause = useSModeMoveFlag;
                    _axis05_sMoveTime_SaveForPause = sMoveTime;
                    _axis05PauseRequestFlag = false;
                    _axis05ResumeRequestFlag = false;
                    _axis05PausedFlag = false;

                    _axis05Command = axisCommand;
                    if (_axis05CommandSaved != 999)
                        _axis05CommandSaved = _axis05Command;
                    _axis05CurrentPositionNumber = 0;
                    axis05TargetStartVelocity_mmps = startVelocity;
                    axis05TargetMaxVelocity_mmps = maxVelocity;
                    axis05TargetAccTime_s = accTime;
                    axis05TargetPosition_mm = targetPosition;
                    axis05TargetDeviationValue_mm = deviationValue;
                    axis05StartVelocity_Temp_pps = Convert.ToInt32(axis05TargetStartVelocity_mmps / _axis05mmPP);
                    axis05MaxVelocity_Temp_pps = Convert.ToInt32(axis05TargetMaxVelocity_mmps / _axis05mmPP);
                    break;
                case 6:
                    //ADD 202210151953 Pause
                    _axis06_axisCommand_SaveForPause = axisCommand;
                    _axis06_axisNumber_SaveForPause = axisNumber;
                    _axis06_startVelocity_SaveForPause = startVelocity;
                    _axis06_maxVelocity_SaveForPause = maxVelocity;
                    _axis06_stopVelocity_SaveForPause = stopVelocity;
                    _axis06_accTime_SaveForPause = accTime;
                    _axis06_decTime_SaveForPause = decTime;
                    _axis06_targetPosition_SaveForPause = targetPosition;
                    _axis06_jogDirection_SaveForPause = jogDirection;
                    _axis06_inch_SaveForPause = inch;
                    _axis06_deviationValue_SaveForPause = deviationValue;
                    _axis06_useSModeMoveFlag_SaveForPause = useSModeMoveFlag;
                    _axis06_sMoveTime_SaveForPause = sMoveTime;
                    _axis06PauseRequestFlag = false;
                    _axis06ResumeRequestFlag = false;
                    _axis06PausedFlag = false;

                    _axis06Command = axisCommand;
                    if (_axis06CommandSaved != 999)
                        _axis06CommandSaved = _axis06Command;
                    _axis06CurrentPositionNumber = 0;
                    axis06TargetStartVelocity_mmps = startVelocity;
                    axis06TargetMaxVelocity_mmps = maxVelocity;
                    axis06TargetAccTime_s = accTime;
                    axis06TargetPosition_mm = targetPosition;
                    axis06TargetDeviationValue_mm = deviationValue;
                    axis06StartVelocity_Temp_pps = Convert.ToInt32(axis06TargetStartVelocity_mmps / _axis06mmPP);
                    axis06MaxVelocity_Temp_pps = Convert.ToInt32(axis06TargetMaxVelocity_mmps / _axis06mmPP);
                    break;
                case 7:
                    //ADD 202210151953 Pause
                    _axis07_axisCommand_SaveForPause = axisCommand;
                    _axis07_axisNumber_SaveForPause = axisNumber;
                    _axis07_startVelocity_SaveForPause = startVelocity;
                    _axis07_maxVelocity_SaveForPause = maxVelocity;
                    _axis07_stopVelocity_SaveForPause = stopVelocity;
                    _axis07_accTime_SaveForPause = accTime;
                    _axis07_decTime_SaveForPause = decTime;
                    _axis07_targetPosition_SaveForPause = targetPosition;
                    _axis07_jogDirection_SaveForPause = jogDirection;
                    _axis07_inch_SaveForPause = inch;
                    _axis07_deviationValue_SaveForPause = deviationValue;
                    _axis07_useSModeMoveFlag_SaveForPause = useSModeMoveFlag;
                    _axis07_sMoveTime_SaveForPause = sMoveTime;
                    _axis07PauseRequestFlag = false;
                    _axis07ResumeRequestFlag = false;
                    _axis07PausedFlag = false;

                    _axis07Command = axisCommand;
                    if (_axis07CommandSaved != 999)
                        _axis07CommandSaved = _axis07Command;
                    _axis07CurrentPositionNumber = 0;
                    axis07TargetStartVelocity_mmps = startVelocity;
                    axis07TargetMaxVelocity_mmps = maxVelocity;
                    axis07TargetAccTime_s = accTime;
                    axis07TargetPosition_mm = targetPosition;
                    axis07TargetDeviationValue_mm = deviationValue;
                    axis07StartVelocity_Temp_pps = Convert.ToInt32(axis07TargetStartVelocity_mmps / _axis07mmPP);
                    axis07MaxVelocity_Temp_pps = Convert.ToInt32(axis07TargetMaxVelocity_mmps / _axis07mmPP);
                    break;
                case 8:
                    //ADD 202210151953 Pause
                    _axis08_axisCommand_SaveForPause = axisCommand;
                    _axis08_axisNumber_SaveForPause = axisNumber;
                    _axis08_startVelocity_SaveForPause = startVelocity;
                    _axis08_maxVelocity_SaveForPause = maxVelocity;
                    _axis08_stopVelocity_SaveForPause = stopVelocity;
                    _axis08_accTime_SaveForPause = accTime;
                    _axis08_decTime_SaveForPause = decTime;
                    _axis08_targetPosition_SaveForPause = targetPosition;
                    _axis08_jogDirection_SaveForPause = jogDirection;
                    _axis08_inch_SaveForPause = inch;
                    _axis08_deviationValue_SaveForPause = deviationValue;
                    _axis08_useSModeMoveFlag_SaveForPause = useSModeMoveFlag;
                    _axis08_sMoveTime_SaveForPause = sMoveTime;
                    _axis08PauseRequestFlag = false;
                    _axis08ResumeRequestFlag = false;
                    _axis08PausedFlag = false;

                    _axis08Command = axisCommand;
                    if (_axis08CommandSaved != 999)
                        _axis08CommandSaved = _axis08Command;
                    _axis08CurrentPositionNumber = 0;
                    axis08TargetStartVelocity_mmps = startVelocity;
                    axis08TargetMaxVelocity_mmps = maxVelocity;
                    axis08TargetAccTime_s = accTime;
                    axis08TargetPosition_mm = targetPosition;
                    axis08TargetDeviationValue_mm = deviationValue;
                    axis08StartVelocity_Temp_pps = Convert.ToInt32(axis08TargetStartVelocity_mmps / _axis08mmPP);
                    axis08MaxVelocity_Temp_pps = Convert.ToInt32(axis08TargetMaxVelocity_mmps / _axis08mmPP);
                    break;
                case 9:
                    //ADD 202210151953 Pause
                    _axis09_axisCommand_SaveForPause = axisCommand;
                    _axis09_axisNumber_SaveForPause = axisNumber;
                    _axis09_startVelocity_SaveForPause = startVelocity;
                    _axis09_maxVelocity_SaveForPause = maxVelocity;
                    _axis09_stopVelocity_SaveForPause = stopVelocity;
                    _axis09_accTime_SaveForPause = accTime;
                    _axis09_decTime_SaveForPause = decTime;
                    _axis09_targetPosition_SaveForPause = targetPosition;
                    _axis09_jogDirection_SaveForPause = jogDirection;
                    _axis09_inch_SaveForPause = inch;
                    _axis09_deviationValue_SaveForPause = deviationValue;
                    _axis09_useSModeMoveFlag_SaveForPause = useSModeMoveFlag;
                    _axis09_sMoveTime_SaveForPause = sMoveTime;
                    _axis09PauseRequestFlag = false;
                    _axis09ResumeRequestFlag = false;
                    _axis09PausedFlag = false;

                    _axis09Command = axisCommand;
                    if (_axis09CommandSaved != 999)
                        _axis09CommandSaved = _axis09Command;
                    _axis09CurrentPositionNumber = 0;
                    axis09TargetStartVelocity_mmps = startVelocity;
                    axis09TargetMaxVelocity_mmps = maxVelocity;
                    axis09TargetAccTime_s = accTime;
                    axis09TargetPosition_mm = targetPosition;
                    axis09TargetDeviationValue_mm = deviationValue;
                    axis09StartVelocity_Temp_pps = Convert.ToInt32(axis09TargetStartVelocity_mmps / _axis09mmPP);
                    axis09MaxVelocity_Temp_pps = Convert.ToInt32(axis09TargetMaxVelocity_mmps / _axis09mmPP);
                    break;
                case 10:
                    //ADD 202210151953 Pause
                    _axis10_axisCommand_SaveForPause = axisCommand;
                    _axis10_axisNumber_SaveForPause = axisNumber;
                    _axis10_startVelocity_SaveForPause = startVelocity;
                    _axis10_maxVelocity_SaveForPause = maxVelocity;
                    _axis10_stopVelocity_SaveForPause = stopVelocity;
                    _axis10_accTime_SaveForPause = accTime;
                    _axis10_decTime_SaveForPause = decTime;
                    _axis10_targetPosition_SaveForPause = targetPosition;
                    _axis10_jogDirection_SaveForPause = jogDirection;
                    _axis10_inch_SaveForPause = inch;
                    _axis10_deviationValue_SaveForPause = deviationValue;
                    _axis10_useSModeMoveFlag_SaveForPause = useSModeMoveFlag;
                    _axis10_sMoveTime_SaveForPause = sMoveTime;
                    _axis10PauseRequestFlag = false;
                    _axis10ResumeRequestFlag = false;
                    _axis10PausedFlag = false;

                    _axis10Command = axisCommand;
                    if (_axis10CommandSaved != 999)
                        _axis10CommandSaved = _axis10Command;
                    _axis10CurrentPositionNumber = 0;
                    axis10TargetStartVelocity_mmps = startVelocity;
                    axis10TargetMaxVelocity_mmps = maxVelocity;
                    axis10TargetAccTime_s = accTime;
                    axis10TargetPosition_mm = targetPosition;
                    axis10TargetDeviationValue_mm = deviationValue;
                    axis10StartVelocity_Temp_pps = Convert.ToInt32(axis10TargetStartVelocity_mmps / _axis10mmPP);
                    axis10MaxVelocity_Temp_pps = Convert.ToInt32(axis10TargetMaxVelocity_mmps / _axis10mmPP);
                    break;
                case 11:
                    //ADD 202210151953 Pause
                    _axis11_axisCommand_SaveForPause = axisCommand;
                    _axis11_axisNumber_SaveForPause = axisNumber;
                    _axis11_startVelocity_SaveForPause = startVelocity;
                    _axis11_maxVelocity_SaveForPause = maxVelocity;
                    _axis11_stopVelocity_SaveForPause = stopVelocity;
                    _axis11_accTime_SaveForPause = accTime;
                    _axis11_decTime_SaveForPause = decTime;
                    _axis11_targetPosition_SaveForPause = targetPosition;
                    _axis11_jogDirection_SaveForPause = jogDirection;
                    _axis11_inch_SaveForPause = inch;
                    _axis11_deviationValue_SaveForPause = deviationValue;
                    _axis11_useSModeMoveFlag_SaveForPause = useSModeMoveFlag;
                    _axis11_sMoveTime_SaveForPause = sMoveTime;
                    _axis11PauseRequestFlag = false;
                    _axis11ResumeRequestFlag = false;
                    _axis11PausedFlag = false;

                    _axis11Command = axisCommand;
                    if (_axis11CommandSaved != 999)
                        _axis11CommandSaved = _axis11Command;
                    _axis11CurrentPositionNumber = 0;
                    axis11TargetStartVelocity_mmps = startVelocity;
                    axis11TargetMaxVelocity_mmps = maxVelocity;
                    axis11TargetAccTime_s = accTime;
                    axis11TargetPosition_mm = targetPosition;
                    axis11TargetDeviationValue_mm = deviationValue;
                    axis11StartVelocity_Temp_pps = Convert.ToInt32(axis11TargetStartVelocity_mmps / _axis11mmPP);
                    axis11MaxVelocity_Temp_pps = Convert.ToInt32(axis11TargetMaxVelocity_mmps / _axis11mmPP);
                    break;
                default:
                    break;
            }
            //999:Home
            if (axisCommand == 999)
            {
                homeAction(axisCommand, axisNumber, startVelocity, maxVelocity, accTime, targetPosition, jogDirection, inch, deviationValue);
            }
            //1-900:点位号定位;1000:实时位置数据定位
            if ((axisCommand >= 1 && axisCommand <= 900) || axisCommand == 1000)
            {
                switch (axisNumber)
                {
                    case 0:
                        if (_axis00HomedFlag)
                        {
                            axis00TargetPosition_Pulse = Convert.ToInt32(axis00TargetPosition_mm / _axis00mmPP);
                            if (axis00TargetPosition_Pulse != axis00CurrentPosition_Pulse)
                            {
                                //单轴点位运动(绝对位置)
                                //ushort,轴句柄
                                //double,加速度,脉冲/ms^2
                                //double,减速度,脉冲/ms^2
                                //double,起跳速度,脉冲/ms
                                //double,终止速度,脉冲/ms
                                //double,最大速度,脉冲/ms
                                //short,平滑系数,[0,199],单位ms
                                //int,目标位置,单位:脉冲
                                short smoothCoef = 0;
                                if (useSModeMoveFlag)
                                    smoothCoef = (short)(sMoveTime / 1000);
                                else
                                    smoothCoef = 0;
                                NMCLib20.NMC_MtMovePtpAbs(axishandle[0], ((double)axis00MaxVelocity_Temp_pps / 1000) / ((double)axis00TargetAccTime_s * 1000), ((double)axis00MaxVelocity_Temp_pps / 1000) / ((double)axis00TargetAccTime_s * 1000), (double)axis00StartVelocity_Temp_pps / 1000,
                                    (double)axis00StartVelocity_Temp_pps / 1000, (double)axis00MaxVelocity_Temp_pps / 1000, smoothCoef, axis00TargetPosition_Pulse);
                                _axis00MoveStartFlag = true;
                            }
                            else
                            {
                                _axis00CurrentPositionNumber = _axis00Command;
                                _axis00Command = 0;
                                _axis00CommandSaved = 0;
                            }
                        }
                        break;
                    case 1:
                        if (_axis01HomedFlag)
                        {
                            axis01TargetPosition_Pulse = Convert.ToInt32(axis01TargetPosition_mm / _axis01mmPP);
                            if (axis01TargetPosition_Pulse != axis01CurrentPosition_Pulse)
                            {
                                short smoothCoef = 0;
                                if (useSModeMoveFlag)
                                    smoothCoef = (short)(sMoveTime / 1000);
                                else
                                    smoothCoef = 0;
                                NMCLib20.NMC_MtMovePtpAbs(axishandle[1], ((double)axis01MaxVelocity_Temp_pps / 1000) / ((double)axis01TargetAccTime_s * 1000), ((double)axis01MaxVelocity_Temp_pps / 1000) / ((double)axis01TargetAccTime_s * 1000), (double)axis01StartVelocity_Temp_pps / 1000,
                                    (double)axis01StartVelocity_Temp_pps / 1000, (double)axis01MaxVelocity_Temp_pps / 1000, smoothCoef, axis01TargetPosition_Pulse);
                                _axis01MoveStartFlag = true;
                            }
                            else
                            {
                                _axis01CurrentPositionNumber = _axis01Command;
                                _axis01Command = 0;
                                _axis01CommandSaved = 0;
                            }
                        }
                        break;
                    case 2:
                        if (_axis02HomedFlag)
                        {
                            axis02TargetPosition_Pulse = Convert.ToInt32(axis02TargetPosition_mm / _axis02mmPP);
                            if (axis02TargetPosition_Pulse != axis02CurrentPosition_Pulse)
                            {
                                short smoothCoef = 0;
                                if (useSModeMoveFlag)
                                    smoothCoef = (short)(sMoveTime / 1000);
                                else
                                    smoothCoef = 0;
                                NMCLib20.NMC_MtMovePtpAbs(axishandle[2], ((double)axis02MaxVelocity_Temp_pps / 1000) / ((double)axis02TargetAccTime_s * 1000), ((double)axis02MaxVelocity_Temp_pps / 1000) / ((double)axis02TargetAccTime_s * 1000), (double)axis02StartVelocity_Temp_pps / 1000,
                                    (double)axis02StartVelocity_Temp_pps / 1000, (double)axis02MaxVelocity_Temp_pps / 1000, smoothCoef, axis02TargetPosition_Pulse);
                                _axis02MoveStartFlag = true;
                            }
                            else
                            {
                                _axis02CurrentPositionNumber = _axis02Command;
                                _axis02Command = 0;
                                _axis02CommandSaved = 0;
                            }
                        }
                        break;
                    case 3:
                        if (_axis03HomedFlag)
                        {
                            axis03TargetPosition_Pulse = Convert.ToInt32(axis03TargetPosition_mm / _axis03mmPP);
                            if (axis03TargetPosition_Pulse != axis03CurrentPosition_Pulse)
                            {
                                short smoothCoef = 0;
                                if (useSModeMoveFlag)
                                    smoothCoef = (short)(sMoveTime / 1000);
                                else
                                    smoothCoef = 0;
                                NMCLib20.NMC_MtMovePtpAbs(axishandle[3], ((double)axis03MaxVelocity_Temp_pps / 1000) / ((double)axis03TargetAccTime_s * 1000), ((double)axis03MaxVelocity_Temp_pps / 1000) / ((double)axis03TargetAccTime_s * 1000), (double)axis03StartVelocity_Temp_pps / 1000,
                                    (double)axis03StartVelocity_Temp_pps / 1000, (double)axis03MaxVelocity_Temp_pps / 1000, smoothCoef, axis03TargetPosition_Pulse);
                                _axis03MoveStartFlag = true;
                            }
                            else
                            {
                                _axis03CurrentPositionNumber = _axis03Command;
                                _axis03Command = 0;
                                _axis03CommandSaved = 0;
                            }
                        }
                        break;
                    case 4:
                        if (_axis04HomedFlag)
                        {
                            axis04TargetPosition_Pulse = Convert.ToInt32(axis04TargetPosition_mm / _axis04mmPP);
                            if (axis04TargetPosition_Pulse != axis04CurrentPosition_Pulse)
                            {
                                short smoothCoef = 0;
                                if (useSModeMoveFlag)
                                    smoothCoef = (short)(sMoveTime / 1000);
                                else
                                    smoothCoef = 0;
                                NMCLib20.NMC_MtMovePtpAbs(axishandle[4], ((double)axis04MaxVelocity_Temp_pps / 1000) / ((double)axis04TargetAccTime_s * 1000), ((double)axis04MaxVelocity_Temp_pps / 1000) / ((double)axis04TargetAccTime_s * 1000), (double)axis04StartVelocity_Temp_pps / 1000,
                                    (double)axis04StartVelocity_Temp_pps / 1000, (double)axis04MaxVelocity_Temp_pps / 1000, smoothCoef, axis04TargetPosition_Pulse);
                                _axis04MoveStartFlag = true;
                            }
                            else
                            {
                                _axis04CurrentPositionNumber = _axis04Command;
                                _axis04Command = 0;
                                _axis04CommandSaved = 0;
                            }
                        }
                        break;
                    case 5:
                        if (_axis05HomedFlag)
                        {
                            axis05TargetPosition_Pulse = Convert.ToInt32(axis05TargetPosition_mm / _axis05mmPP);
                            if (axis05TargetPosition_Pulse != axis05CurrentPosition_Pulse)
                            {
                                short smoothCoef = 0;
                                if (useSModeMoveFlag)
                                    smoothCoef = (short)(sMoveTime / 1000);
                                else
                                    smoothCoef = 0;
                                NMCLib20.NMC_MtMovePtpAbs(axishandle[5], ((double)axis05MaxVelocity_Temp_pps / 1000) / ((double)axis05TargetAccTime_s * 1000), ((double)axis05MaxVelocity_Temp_pps / 1000) / ((double)axis05TargetAccTime_s * 1000), (double)axis05StartVelocity_Temp_pps / 1000,
                                    (double)axis05StartVelocity_Temp_pps / 1000, (double)axis05MaxVelocity_Temp_pps / 1000, smoothCoef, axis05TargetPosition_Pulse);
                                _axis05MoveStartFlag = true;
                            }
                            else
                            {
                                _axis05CurrentPositionNumber = _axis05Command;
                                _axis05Command = 0;
                                _axis05CommandSaved = 0;
                            }
                        }
                        break;
                    case 6:
                        if (_axis06HomedFlag)
                        {
                            axis06TargetPosition_Pulse = Convert.ToInt32(axis06TargetPosition_mm / _axis06mmPP);
                            if (axis06TargetPosition_Pulse != axis06CurrentPosition_Pulse)
                            {
                                short smoothCoef = 0;
                                if (useSModeMoveFlag)
                                    smoothCoef = (short)(sMoveTime / 1000);
                                else
                                    smoothCoef = 0;
                                NMCLib20.NMC_MtMovePtpAbs(axishandle[6], ((double)axis06MaxVelocity_Temp_pps / 1000) / ((double)axis06TargetAccTime_s * 1000), ((double)axis06MaxVelocity_Temp_pps / 1000) / ((double)axis06TargetAccTime_s * 1000), (double)axis06StartVelocity_Temp_pps / 1000,
                                    (double)axis06StartVelocity_Temp_pps / 1000, (double)axis06MaxVelocity_Temp_pps / 1000, smoothCoef, axis06TargetPosition_Pulse);
                                _axis06MoveStartFlag = true;
                            }
                            else
                            {
                                _axis06CurrentPositionNumber = _axis06Command;
                                _axis06Command = 0;
                                _axis06CommandSaved = 0;
                            }
                        }
                        break;
                    case 7:
                        if (_axis07HomedFlag)
                        {
                            axis07TargetPosition_Pulse = Convert.ToInt32(axis07TargetPosition_mm / _axis07mmPP);
                            if (axis07TargetPosition_Pulse != axis07CurrentPosition_Pulse)
                            {
                                short smoothCoef = 0;
                                if (useSModeMoveFlag)
                                    smoothCoef = (short)(sMoveTime / 1000);
                                else
                                    smoothCoef = 0;
                                NMCLib20.NMC_MtMovePtpAbs(axishandle[7], ((double)axis07MaxVelocity_Temp_pps / 1000) / ((double)axis07TargetAccTime_s * 1000), ((double)axis07MaxVelocity_Temp_pps / 1000) / ((double)axis07TargetAccTime_s * 1000), (double)axis07StartVelocity_Temp_pps / 1000,
                                    (double)axis07StartVelocity_Temp_pps / 1000, (double)axis07MaxVelocity_Temp_pps / 1000, smoothCoef, axis07TargetPosition_Pulse);
                                _axis07MoveStartFlag = true;
                            }
                            else
                            {
                                _axis07CurrentPositionNumber = _axis07Command;
                                _axis07Command = 0;
                                _axis07CommandSaved = 0;
                            }
                        }
                        break;
                    case 8:
                        if (_axis08HomedFlag)
                        {
                            axis08TargetPosition_Pulse = Convert.ToInt32(axis08TargetPosition_mm / _axis08mmPP);
                            if (axis08TargetPosition_Pulse != axis08CurrentPosition_Pulse)
                            {
                                short smoothCoef = 0;
                                if (useSModeMoveFlag)
                                    smoothCoef = (short)(sMoveTime / 1000);
                                else
                                    smoothCoef = 0;
                                NMCLib20.NMC_MtMovePtpAbs(axishandle[8], ((double)axis08MaxVelocity_Temp_pps / 1000) / ((double)axis08TargetAccTime_s * 1000), ((double)axis08MaxVelocity_Temp_pps / 1000) / ((double)axis08TargetAccTime_s * 1000), (double)axis08StartVelocity_Temp_pps / 1000,
                                    (double)axis08StartVelocity_Temp_pps / 1000, (double)axis08MaxVelocity_Temp_pps / 1000, smoothCoef, axis08TargetPosition_Pulse);
                                _axis08MoveStartFlag = true;
                            }
                            else
                            {
                                _axis08CurrentPositionNumber = _axis08Command;
                                _axis08Command = 0;
                                _axis08CommandSaved = 0;
                            }
                        }
                        break;
                    case 9:
                        if (_axis09HomedFlag)
                        {
                            axis09TargetPosition_Pulse = Convert.ToInt32(axis09TargetPosition_mm / _axis09mmPP);
                            if (axis09TargetPosition_Pulse != axis09CurrentPosition_Pulse)
                            {
                                short smoothCoef = 0;
                                if (useSModeMoveFlag)
                                    smoothCoef = (short)(sMoveTime / 1000);
                                else
                                    smoothCoef = 0;
                                NMCLib20.NMC_MtMovePtpAbs(axishandle[9], ((double)axis09MaxVelocity_Temp_pps / 1000) / ((double)axis09TargetAccTime_s * 1000), ((double)axis09MaxVelocity_Temp_pps / 1000) / ((double)axis09TargetAccTime_s * 1000), (double)axis09StartVelocity_Temp_pps / 1000,
                                    (double)axis09StartVelocity_Temp_pps / 1000, (double)axis09MaxVelocity_Temp_pps / 1000, smoothCoef, axis09TargetPosition_Pulse);
                                _axis09MoveStartFlag = true;
                            }
                            else
                            {
                                _axis09CurrentPositionNumber = _axis09Command;
                                _axis09Command = 0;
                                _axis09CommandSaved = 0;
                            }
                        }
                        break;
                    case 10:
                        if (_axis10HomedFlag)
                        {
                            axis10TargetPosition_Pulse = Convert.ToInt32(axis10TargetPosition_mm / _axis10mmPP);
                            if (axis10TargetPosition_Pulse != axis10CurrentPosition_Pulse)
                            {
                                short smoothCoef = 0;
                                if (useSModeMoveFlag)
                                    smoothCoef = (short)(sMoveTime / 1000);
                                else
                                    smoothCoef = 0;
                                NMCLib20.NMC_MtMovePtpAbs(axishandle[10], ((double)axis10MaxVelocity_Temp_pps / 1000) / ((double)axis10TargetAccTime_s * 1000), ((double)axis10MaxVelocity_Temp_pps / 1000) / ((double)axis10TargetAccTime_s * 1000), (double)axis10StartVelocity_Temp_pps / 1000,
                                    (double)axis10StartVelocity_Temp_pps / 1000, (double)axis10MaxVelocity_Temp_pps / 1000, smoothCoef, axis10TargetPosition_Pulse);
                                _axis10MoveStartFlag = true;
                            }
                            else
                            {
                                _axis10CurrentPositionNumber = _axis10Command;
                                _axis10Command = 0;
                                _axis10CommandSaved = 0;
                            }
                        }
                        break;
                    case 11:
                        if (_axis11HomedFlag)
                        {
                            axis11TargetPosition_Pulse = Convert.ToInt32(axis11TargetPosition_mm / _axis11mmPP);
                            if (axis11TargetPosition_Pulse != axis11CurrentPosition_Pulse)
                            {
                                short smoothCoef = 0;
                                if (useSModeMoveFlag)
                                    smoothCoef = (short)(sMoveTime / 1000);
                                else
                                    smoothCoef = 0;
                                NMCLib20.NMC_MtMovePtpAbs(axishandle[11], ((double)axis11MaxVelocity_Temp_pps / 1000) / ((double)axis11TargetAccTime_s * 1000), ((double)axis11MaxVelocity_Temp_pps / 1000) / ((double)axis11TargetAccTime_s * 1000), (double)axis11StartVelocity_Temp_pps / 1000,
                                    (double)axis11StartVelocity_Temp_pps / 1000, (double)axis11MaxVelocity_Temp_pps / 1000, smoothCoef, axis11TargetPosition_Pulse);
                                _axis11MoveStartFlag = true;
                            }
                            else
                            {
                                _axis11CurrentPositionNumber = _axis11Command;
                                _axis11Command = 0;
                                _axis11CommandSaved = 0;
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            //901:Jog
            if (axisCommand == 901)
            {
                switch (axisNumber)
                {
                    case 0:
                        if (_axis00HomedFlag)//已回原点
                        {
                            if (!jogDirection)//负方向
                            {
                                if (!inch)//点动
                                    axis00TargetPosition_Pulse = Convert.ToInt32(_axis00softLimitMinus / _axis00mmPP);
                                else//寸动
                                {
                                    axis00TargetPosition_mm = _axis00CurrentPosition_mm - axis00TargetPosition_mm;
                                    if (axis00TargetPosition_mm < _axis00softLimitMinus)
                                        axis00TargetPosition_mm = _axis00softLimitMinus;
                                    axis00TargetPosition_Pulse = Convert.ToInt32(axis00TargetPosition_mm / _axis00mmPP);
                                }
                            }
                            else//正方向
                            {
                                if (!inch)//点动
                                    axis00TargetPosition_Pulse = Convert.ToInt32(_axis00softLimitPlus / _axis00mmPP);
                                else//寸动
                                {
                                    axis00TargetPosition_mm = _axis00CurrentPosition_mm + axis00TargetPosition_mm;
                                    if (axis00TargetPosition_mm > _axis00softLimitPlus)
                                        axis00TargetPosition_mm = _axis00softLimitPlus;
                                    axis00TargetPosition_Pulse = Convert.ToInt32(axis00TargetPosition_mm / _axis00mmPP);
                                }
                            }
                            if (axis00TargetPosition_Pulse != axis00CurrentPosition_Pulse)
                            {
                                short smoothCoef = 0;
                                if (useSModeMoveFlag)
                                    smoothCoef = (short)(sMoveTime / 1000);
                                else
                                    smoothCoef = 0;
                                NMCLib20.NMC_MtMovePtpAbs(axishandle[0], ((double)axis00MaxVelocity_Temp_pps / 1000) / ((double)axis00TargetAccTime_s * 1000), ((double)axis00MaxVelocity_Temp_pps / 1000) / ((double)axis00TargetAccTime_s * 1000), (double)axis00StartVelocity_Temp_pps / 1000,
                                    (double)axis00StartVelocity_Temp_pps / 1000, (double)axis00MaxVelocity_Temp_pps / 1000, smoothCoef, axis00TargetPosition_Pulse);
                            }
                        }
                        else//没有回原点
                        {
                            if (!jogDirection)//负方向
                            {
                                if (!inch)//点动
                                {
                                    short smoothCoef = 0;
                                    if (useSModeMoveFlag)
                                        smoothCoef = (short)(sMoveTime / 1000);
                                    else
                                        smoothCoef = 0;
                                    NMCLib20.NMC_MtMoveJog(axishandle[0], ((double)axis00MaxVelocity_Temp_pps / 1000) / ((double)axis00TargetAccTime_s * 1000), ((double)axis00MaxVelocity_Temp_pps / 1000) / ((double)axis00TargetAccTime_s * 1000), (double)axis00MaxVelocity_Temp_pps / 1000 * -1, smoothCoef, 0);
                                }
                                else//寸动
                                {
                                    axis00TargetPosition_Pulse = Convert.ToInt32(axis00TargetPosition_mm * -1 / _axis00mmPP);
                                    short smoothCoef = 0;
                                    if (useSModeMoveFlag)
                                        smoothCoef = (short)(sMoveTime / 1000);
                                    else
                                        smoothCoef = 0;
                                    NMCLib20.NMC_MtMovePtpRel(axishandle[0], ((double)axis00MaxVelocity_Temp_pps / 1000) / ((double)axis00TargetAccTime_s * 1000), ((double)axis00MaxVelocity_Temp_pps / 1000) / ((double)axis00TargetAccTime_s * 1000), (double)axis00StartVelocity_Temp_pps / 1000,
                                        (double)axis00StartVelocity_Temp_pps / 1000, (double)axis00MaxVelocity_Temp_pps / 1000, smoothCoef, axis00TargetPosition_Pulse);
                                }
                            }
                            else//正方向
                            {
                                if (!inch)//点动
                                {
                                    short smoothCoef = 0;
                                    if (useSModeMoveFlag)
                                        smoothCoef = (short)(sMoveTime / 1000);
                                    else
                                        smoothCoef = 0;
                                    NMCLib20.NMC_MtMoveJog(axishandle[0], ((double)axis00MaxVelocity_Temp_pps / 1000) / ((double)axis00TargetAccTime_s * 1000), ((double)axis00MaxVelocity_Temp_pps / 1000) / ((double)axis00TargetAccTime_s * 1000), (double)axis00MaxVelocity_Temp_pps / 1000, smoothCoef, 0);
                                }
                                else//寸动
                                {
                                    axis00TargetPosition_Pulse = Convert.ToInt32(axis00TargetPosition_mm / _axis00mmPP);
                                    short smoothCoef = 0;
                                    if (useSModeMoveFlag)
                                        smoothCoef = (short)(sMoveTime / 1000);
                                    else
                                        smoothCoef = 0;
                                    NMCLib20.NMC_MtMovePtpRel(axishandle[0], ((double)axis00MaxVelocity_Temp_pps / 1000) / ((double)axis00TargetAccTime_s * 1000), ((double)axis00MaxVelocity_Temp_pps / 1000) / ((double)axis00TargetAccTime_s * 1000), (double)axis00StartVelocity_Temp_pps / 1000,
                                        (double)axis00StartVelocity_Temp_pps / 1000, (double)axis00MaxVelocity_Temp_pps / 1000, smoothCoef, axis00TargetPosition_Pulse);
                                }
                            }
                        }
                        break;
                    case 1:
                        if (_axis01HomedFlag)//已回原点
                        {
                            if (!jogDirection)//负方向
                            {
                                if (!inch)//点动
                                    axis01TargetPosition_Pulse = Convert.ToInt32(_axis01softLimitMinus / _axis01mmPP);
                                else//寸动
                                {
                                    axis01TargetPosition_mm = _axis01CurrentPosition_mm - axis01TargetPosition_mm;
                                    if (axis01TargetPosition_mm < _axis01softLimitMinus)
                                        axis01TargetPosition_mm = _axis01softLimitMinus;
                                    axis01TargetPosition_Pulse = Convert.ToInt32(axis01TargetPosition_mm / _axis01mmPP);
                                }
                            }
                            else//正方向
                            {
                                if (!inch)//点动
                                    axis01TargetPosition_Pulse = Convert.ToInt32(_axis01softLimitPlus / _axis01mmPP);
                                else//寸动
                                {
                                    axis01TargetPosition_mm = _axis01CurrentPosition_mm + axis01TargetPosition_mm;
                                    if (axis01TargetPosition_mm > _axis01softLimitPlus)
                                        axis01TargetPosition_mm = _axis01softLimitPlus;
                                    axis01TargetPosition_Pulse = Convert.ToInt32(axis01TargetPosition_mm / _axis01mmPP);
                                }
                            }
                            if (axis01TargetPosition_Pulse != axis01CurrentPosition_Pulse)
                            {
                                short smoothCoef = 0;
                                if (useSModeMoveFlag)
                                    smoothCoef = (short)(sMoveTime / 1000);
                                else
                                    smoothCoef = 0;
                                NMCLib20.NMC_MtMovePtpAbs(axishandle[1], ((double)axis01MaxVelocity_Temp_pps / 1000) / ((double)axis01TargetAccTime_s * 1000), ((double)axis01MaxVelocity_Temp_pps / 1000) / ((double)axis01TargetAccTime_s * 1000), (double)axis01StartVelocity_Temp_pps / 1000,
                                    (double)axis01StartVelocity_Temp_pps / 1000, (double)axis01MaxVelocity_Temp_pps / 1000, smoothCoef, axis01TargetPosition_Pulse);
                            }
                        }
                        else//没有回原点
                        {
                            if (!jogDirection)//负方向
                            {
                                if (!inch)//点动
                                {
                                    short smoothCoef = 0;
                                    if (useSModeMoveFlag)
                                        smoothCoef = (short)(sMoveTime / 1000);
                                    else
                                        smoothCoef = 0;
                                    NMCLib20.NMC_MtMoveJog(axishandle[1], ((double)axis01MaxVelocity_Temp_pps / 1000) / ((double)axis01TargetAccTime_s * 1000), ((double)axis01MaxVelocity_Temp_pps / 1000) / ((double)axis01TargetAccTime_s * 1000), (double)axis01MaxVelocity_Temp_pps / 1000 * -1, smoothCoef, 0);
                                }
                                else//寸动
                                {
                                    axis01TargetPosition_Pulse = Convert.ToInt32(axis01TargetPosition_mm * -1 / _axis01mmPP);
                                    short smoothCoef = 0;
                                    if (useSModeMoveFlag)
                                        smoothCoef = (short)(sMoveTime / 1000);
                                    else
                                        smoothCoef = 0;
                                    NMCLib20.NMC_MtMovePtpRel(axishandle[1], ((double)axis01MaxVelocity_Temp_pps / 1000) / ((double)axis01TargetAccTime_s * 1000), ((double)axis01MaxVelocity_Temp_pps / 1000) / ((double)axis01TargetAccTime_s * 1000), (double)axis01StartVelocity_Temp_pps / 1000,
                                        (double)axis01StartVelocity_Temp_pps / 1000, (double)axis01MaxVelocity_Temp_pps / 1000, smoothCoef, axis01TargetPosition_Pulse);
                                }
                            }
                            else//正方向
                            {
                                if (!inch)//点动
                                {
                                    short smoothCoef = 0;
                                    if (useSModeMoveFlag)
                                        smoothCoef = (short)(sMoveTime / 1000);
                                    else
                                        smoothCoef = 0;
                                    NMCLib20.NMC_MtMoveJog(axishandle[1], ((double)axis01MaxVelocity_Temp_pps / 1000) / ((double)axis01TargetAccTime_s * 1000), ((double)axis01MaxVelocity_Temp_pps / 1000) / ((double)axis01TargetAccTime_s * 1000), (double)axis01MaxVelocity_Temp_pps / 1000, smoothCoef, 0);
                                }
                                else//寸动
                                {
                                    axis01TargetPosition_Pulse = Convert.ToInt32(axis01TargetPosition_mm / _axis01mmPP);
                                    short smoothCoef = 0;
                                    if (useSModeMoveFlag)
                                        smoothCoef = (short)(sMoveTime / 1000);
                                    else
                                        smoothCoef = 0;
                                    NMCLib20.NMC_MtMovePtpRel(axishandle[1], ((double)axis01MaxVelocity_Temp_pps / 1000) / ((double)axis01TargetAccTime_s * 1000), ((double)axis01MaxVelocity_Temp_pps / 1000) / ((double)axis01TargetAccTime_s * 1000), (double)axis01StartVelocity_Temp_pps / 1000,
                                        (double)axis01StartVelocity_Temp_pps / 1000, (double)axis01MaxVelocity_Temp_pps / 1000, smoothCoef, axis01TargetPosition_Pulse);
                                }
                            }
                        }
                        break;
                    case 2:
                        if (_axis02HomedFlag)//已回原点
                        {
                            if (!jogDirection)//负方向
                            {
                                if (!inch)//点动
                                    axis02TargetPosition_Pulse = Convert.ToInt32(_axis02softLimitMinus / _axis02mmPP);
                                else//寸动
                                {
                                    axis02TargetPosition_mm = _axis02CurrentPosition_mm - axis02TargetPosition_mm;
                                    if (axis02TargetPosition_mm < _axis02softLimitMinus)
                                        axis02TargetPosition_mm = _axis02softLimitMinus;
                                    axis02TargetPosition_Pulse = Convert.ToInt32(axis02TargetPosition_mm / _axis02mmPP);
                                }
                            }
                            else//正方向
                            {
                                if (!inch)//点动
                                    axis02TargetPosition_Pulse = Convert.ToInt32(_axis02softLimitPlus / _axis02mmPP);
                                else//寸动
                                {
                                    axis02TargetPosition_mm = _axis02CurrentPosition_mm + axis02TargetPosition_mm;
                                    if (axis02TargetPosition_mm > _axis02softLimitPlus)
                                        axis02TargetPosition_mm = _axis02softLimitPlus;
                                    axis02TargetPosition_Pulse = Convert.ToInt32(axis02TargetPosition_mm / _axis02mmPP);
                                }
                            }
                            if (axis02TargetPosition_Pulse != axis02CurrentPosition_Pulse)
                            {
                                short smoothCoef = 0;
                                if (useSModeMoveFlag)
                                    smoothCoef = (short)(sMoveTime / 1000);
                                else
                                    smoothCoef = 0;
                                NMCLib20.NMC_MtMovePtpAbs(axishandle[2], ((double)axis02MaxVelocity_Temp_pps / 1000) / ((double)axis02TargetAccTime_s * 1000), ((double)axis02MaxVelocity_Temp_pps / 1000) / ((double)axis02TargetAccTime_s * 1000), (double)axis02StartVelocity_Temp_pps / 1000,
                                    (double)axis02StartVelocity_Temp_pps / 1000, (double)axis02MaxVelocity_Temp_pps / 1000, smoothCoef, axis02TargetPosition_Pulse);
                            }
                        }
                        else//没有回原点
                        {
                            if (!jogDirection)//负方向
                            {
                                if (!inch)//点动
                                {
                                    short smoothCoef = 0;
                                    if (useSModeMoveFlag)
                                        smoothCoef = (short)(sMoveTime / 1000);
                                    else
                                        smoothCoef = 0;
                                    NMCLib20.NMC_MtMoveJog(axishandle[2], ((double)axis02MaxVelocity_Temp_pps / 1000) / ((double)axis02TargetAccTime_s * 1000), ((double)axis02MaxVelocity_Temp_pps / 1000) / ((double)axis02TargetAccTime_s * 1000), (double)axis02MaxVelocity_Temp_pps / 1000 * -1, smoothCoef, 0);
                                }
                                else//寸动
                                {
                                    axis02TargetPosition_Pulse = Convert.ToInt32(axis02TargetPosition_mm * -1 / _axis02mmPP);
                                    short smoothCoef = 0;
                                    if (useSModeMoveFlag)
                                        smoothCoef = (short)(sMoveTime / 1000);
                                    else
                                        smoothCoef = 0;
                                    NMCLib20.NMC_MtMovePtpRel(axishandle[2], ((double)axis02MaxVelocity_Temp_pps / 1000) / ((double)axis02TargetAccTime_s * 1000), ((double)axis02MaxVelocity_Temp_pps / 1000) / ((double)axis02TargetAccTime_s * 1000), (double)axis02StartVelocity_Temp_pps / 1000,
                                        (double)axis02StartVelocity_Temp_pps / 1000, (double)axis02MaxVelocity_Temp_pps / 1000, smoothCoef, axis02TargetPosition_Pulse);
                                }
                            }
                            else//正方向
                            {
                                if (!inch)//点动
                                {
                                    short smoothCoef = 0;
                                    if (useSModeMoveFlag)
                                        smoothCoef = (short)(sMoveTime / 1000);
                                    else
                                        smoothCoef = 0;
                                    NMCLib20.NMC_MtMoveJog(axishandle[2], ((double)axis02MaxVelocity_Temp_pps / 1000) / ((double)axis02TargetAccTime_s * 1000), ((double)axis02MaxVelocity_Temp_pps / 1000) / ((double)axis02TargetAccTime_s * 1000), (double)axis02MaxVelocity_Temp_pps / 1000, smoothCoef, 0);
                                }
                                else//寸动
                                {
                                    axis02TargetPosition_Pulse = Convert.ToInt32(axis02TargetPosition_mm / _axis02mmPP);
                                    short smoothCoef = 0;
                                    if (useSModeMoveFlag)
                                        smoothCoef = (short)(sMoveTime / 1000);
                                    else
                                        smoothCoef = 0;
                                    NMCLib20.NMC_MtMovePtpRel(axishandle[2], ((double)axis02MaxVelocity_Temp_pps / 1000) / ((double)axis02TargetAccTime_s * 1000), ((double)axis02MaxVelocity_Temp_pps / 1000) / ((double)axis02TargetAccTime_s * 1000), (double)axis02StartVelocity_Temp_pps / 1000,
                                        (double)axis02StartVelocity_Temp_pps / 1000, (double)axis02MaxVelocity_Temp_pps / 1000, smoothCoef, axis02TargetPosition_Pulse);
                                }
                            }
                        }
                        break;
                    case 3:
                        if (_axis03HomedFlag)//已回原点
                        {
                            if (!jogDirection)//负方向
                            {
                                if (!inch)//点动
                                    axis03TargetPosition_Pulse = Convert.ToInt32(_axis03softLimitMinus / _axis03mmPP);
                                else//寸动
                                {
                                    axis03TargetPosition_mm = _axis03CurrentPosition_mm - axis03TargetPosition_mm;
                                    if (axis03TargetPosition_mm < _axis03softLimitMinus)
                                        axis03TargetPosition_mm = _axis03softLimitMinus;
                                    axis03TargetPosition_Pulse = Convert.ToInt32(axis03TargetPosition_mm / _axis03mmPP);
                                }
                            }
                            else//正方向
                            {
                                if (!inch)//点动
                                    axis03TargetPosition_Pulse = Convert.ToInt32(_axis03softLimitPlus / _axis03mmPP);
                                else//寸动
                                {
                                    axis03TargetPosition_mm = _axis03CurrentPosition_mm + axis03TargetPosition_mm;
                                    if (axis03TargetPosition_mm > _axis03softLimitPlus)
                                        axis03TargetPosition_mm = _axis03softLimitPlus;
                                    axis03TargetPosition_Pulse = Convert.ToInt32(axis03TargetPosition_mm / _axis03mmPP);
                                }
                            }
                            if (axis03TargetPosition_Pulse != axis03CurrentPosition_Pulse)
                            {
                                short smoothCoef = 0;
                                if (useSModeMoveFlag)
                                    smoothCoef = (short)(sMoveTime / 1000);
                                else
                                    smoothCoef = 0;
                                NMCLib20.NMC_MtMovePtpAbs(axishandle[3], ((double)axis03MaxVelocity_Temp_pps / 1000) / ((double)axis03TargetAccTime_s * 1000), ((double)axis03MaxVelocity_Temp_pps / 1000) / ((double)axis03TargetAccTime_s * 1000), (double)axis03StartVelocity_Temp_pps / 1000,
                                    (double)axis03StartVelocity_Temp_pps / 1000, (double)axis03MaxVelocity_Temp_pps / 1000, smoothCoef, axis03TargetPosition_Pulse);
                            }
                        }
                        else//没有回原点
                        {
                            if (!jogDirection)//负方向
                            {
                                if (!inch)//点动
                                {
                                    short smoothCoef = 0;
                                    if (useSModeMoveFlag)
                                        smoothCoef = (short)(sMoveTime / 1000);
                                    else
                                        smoothCoef = 0;
                                    NMCLib20.NMC_MtMoveJog(axishandle[3], ((double)axis03MaxVelocity_Temp_pps / 1000) / ((double)axis03TargetAccTime_s * 1000), ((double)axis03MaxVelocity_Temp_pps / 1000) / ((double)axis03TargetAccTime_s * 1000), (double)axis03MaxVelocity_Temp_pps / 1000 * -1, smoothCoef, 0);
                                }
                                else//寸动
                                {
                                    axis03TargetPosition_Pulse = Convert.ToInt32(axis03TargetPosition_mm * -1 / _axis03mmPP);
                                    short smoothCoef = 0;
                                    if (useSModeMoveFlag)
                                        smoothCoef = (short)(sMoveTime / 1000);
                                    else
                                        smoothCoef = 0;
                                    NMCLib20.NMC_MtMovePtpRel(axishandle[3], ((double)axis03MaxVelocity_Temp_pps / 1000) / ((double)axis03TargetAccTime_s * 1000), ((double)axis03MaxVelocity_Temp_pps / 1000) / ((double)axis03TargetAccTime_s * 1000), (double)axis03StartVelocity_Temp_pps / 1000,
                                        (double)axis03StartVelocity_Temp_pps / 1000, (double)axis03MaxVelocity_Temp_pps / 1000, smoothCoef, axis03TargetPosition_Pulse);
                                }
                            }
                            else//正方向
                            {
                                if (!inch)//点动
                                {
                                    short smoothCoef = 0;
                                    if (useSModeMoveFlag)
                                        smoothCoef = (short)(sMoveTime / 1000);
                                    else
                                        smoothCoef = 0;
                                    NMCLib20.NMC_MtMoveJog(axishandle[3], ((double)axis02MaxVelocity_Temp_pps / 1000) / ((double)axis02TargetAccTime_s * 1000), ((double)axis02MaxVelocity_Temp_pps / 1000) / ((double)axis02TargetAccTime_s * 1000), (double)axis02MaxVelocity_Temp_pps / 1000, smoothCoef, 0);
                                }
                                else//寸动
                                {
                                    axis02TargetPosition_Pulse = Convert.ToInt32(axis02TargetPosition_mm / _axis02mmPP);
                                    short smoothCoef = 0;
                                    if (useSModeMoveFlag)
                                        smoothCoef = (short)(sMoveTime / 1000);
                                    else
                                        smoothCoef = 0;
                                    NMCLib20.NMC_MtMovePtpRel(axishandle[3], ((double)axis02MaxVelocity_Temp_pps / 1000) / ((double)axis02TargetAccTime_s * 1000), ((double)axis02MaxVelocity_Temp_pps / 1000) / ((double)axis02TargetAccTime_s * 1000), (double)axis02StartVelocity_Temp_pps / 1000,
                                        (double)axis02StartVelocity_Temp_pps / 1000, (double)axis02MaxVelocity_Temp_pps / 1000, smoothCoef, axis02TargetPosition_Pulse);
                                }
                            }
                        }
                        break;
                    case 4:
                        if (_axis04HomedFlag)//已回原点
                        {
                            if (!jogDirection)//负方向
                            {
                                if (!inch)//点动
                                    axis04TargetPosition_Pulse = Convert.ToInt32(_axis04softLimitMinus / _axis04mmPP);
                                else//寸动
                                {
                                    axis04TargetPosition_mm = _axis04CurrentPosition_mm - axis04TargetPosition_mm;
                                    if (axis04TargetPosition_mm < _axis04softLimitMinus)
                                        axis04TargetPosition_mm = _axis04softLimitMinus;
                                    axis04TargetPosition_Pulse = Convert.ToInt32(axis04TargetPosition_mm / _axis04mmPP);
                                }
                            }
                            else//正方向
                            {
                                if (!inch)//点动
                                    axis04TargetPosition_Pulse = Convert.ToInt32(_axis04softLimitPlus / _axis04mmPP);
                                else//寸动
                                {
                                    axis04TargetPosition_mm = _axis04CurrentPosition_mm + axis04TargetPosition_mm;
                                    if (axis04TargetPosition_mm > _axis04softLimitPlus)
                                        axis04TargetPosition_mm = _axis04softLimitPlus;
                                    axis04TargetPosition_Pulse = Convert.ToInt32(axis04TargetPosition_mm / _axis04mmPP);
                                }
                            }
                            if (axis04TargetPosition_Pulse != axis04CurrentPosition_Pulse)
                            {
                                short smoothCoef = 0;
                                if (useSModeMoveFlag)
                                    smoothCoef = (short)(sMoveTime / 1000);
                                else
                                    smoothCoef = 0;
                                NMCLib20.NMC_MtMovePtpAbs(axishandle[4], ((double)axis04MaxVelocity_Temp_pps / 1000) / ((double)axis04TargetAccTime_s * 1000), ((double)axis04MaxVelocity_Temp_pps / 1000) / ((double)axis04TargetAccTime_s * 1000), (double)axis04StartVelocity_Temp_pps / 1000,
                                    (double)axis04StartVelocity_Temp_pps / 1000, (double)axis04MaxVelocity_Temp_pps / 1000, smoothCoef, axis04TargetPosition_Pulse);
                            }
                        }
                        else//没有回原点
                        {
                            if (!jogDirection)//负方向
                            {
                                if (!inch)//点动
                                {
                                    short smoothCoef = 0;
                                    if (useSModeMoveFlag)
                                        smoothCoef = (short)(sMoveTime / 1000);
                                    else
                                        smoothCoef = 0;
                                    NMCLib20.NMC_MtMoveJog(axishandle[4], ((double)axis04MaxVelocity_Temp_pps / 1000) / ((double)axis04TargetAccTime_s * 1000), ((double)axis04MaxVelocity_Temp_pps / 1000) / ((double)axis04TargetAccTime_s * 1000), (double)axis04MaxVelocity_Temp_pps / 1000 * -1, smoothCoef, 0);
                                }
                                else//寸动
                                {
                                    axis04TargetPosition_Pulse = Convert.ToInt32(axis04TargetPosition_mm * -1 / _axis04mmPP);
                                    short smoothCoef = 0;
                                    if (useSModeMoveFlag)
                                        smoothCoef = (short)(sMoveTime / 1000);
                                    else
                                        smoothCoef = 0;
                                    NMCLib20.NMC_MtMovePtpRel(axishandle[4], ((double)axis04MaxVelocity_Temp_pps / 1000) / ((double)axis04TargetAccTime_s * 1000), ((double)axis04MaxVelocity_Temp_pps / 1000) / ((double)axis04TargetAccTime_s * 1000), (double)axis04StartVelocity_Temp_pps / 1000,
                                        (double)axis04StartVelocity_Temp_pps / 1000, (double)axis04MaxVelocity_Temp_pps / 1000, smoothCoef, axis04TargetPosition_Pulse);
                                }
                            }
                            else//正方向
                            {
                                if (!inch)//点动
                                {
                                    short smoothCoef = 0;
                                    if (useSModeMoveFlag)
                                        smoothCoef = (short)(sMoveTime / 1000);
                                    else
                                        smoothCoef = 0;
                                    NMCLib20.NMC_MtMoveJog(axishandle[4], ((double)axis04MaxVelocity_Temp_pps / 1000) / ((double)axis04TargetAccTime_s * 1000), ((double)axis04MaxVelocity_Temp_pps / 1000) / ((double)axis04TargetAccTime_s * 1000), (double)axis04MaxVelocity_Temp_pps / 1000, smoothCoef, 0);
                                }
                                else//寸动
                                {
                                    axis04TargetPosition_Pulse = Convert.ToInt32(axis04TargetPosition_mm / _axis04mmPP);
                                    short smoothCoef = 0;
                                    if (useSModeMoveFlag)
                                        smoothCoef = (short)(sMoveTime / 1000);
                                    else
                                        smoothCoef = 0;
                                    NMCLib20.NMC_MtMovePtpRel(axishandle[4], ((double)axis04MaxVelocity_Temp_pps / 1000) / ((double)axis04TargetAccTime_s * 1000), ((double)axis04MaxVelocity_Temp_pps / 1000) / ((double)axis04TargetAccTime_s * 1000), (double)axis04StartVelocity_Temp_pps / 1000,
                                        (double)axis04StartVelocity_Temp_pps / 1000, (double)axis04MaxVelocity_Temp_pps / 1000, smoothCoef, axis04TargetPosition_Pulse);
                                }
                            }
                        }
                        break;
                    case 5:
                        if (_axis05HomedFlag)//已回原点
                        {
                            if (!jogDirection)//负方向
                            {
                                if (!inch)//点动
                                    axis05TargetPosition_Pulse = Convert.ToInt32(_axis05softLimitMinus / _axis05mmPP);
                                else//寸动
                                {
                                    axis05TargetPosition_mm = _axis05CurrentPosition_mm - axis05TargetPosition_mm;
                                    if (axis05TargetPosition_mm < _axis05softLimitMinus)
                                        axis05TargetPosition_mm = _axis05softLimitMinus;
                                    axis05TargetPosition_Pulse = Convert.ToInt32(axis05TargetPosition_mm / _axis05mmPP);
                                }
                            }
                            else//正方向
                            {
                                if (!inch)//点动
                                    axis05TargetPosition_Pulse = Convert.ToInt32(_axis05softLimitPlus / _axis05mmPP);
                                else//寸动
                                {
                                    axis05TargetPosition_mm = _axis05CurrentPosition_mm + axis05TargetPosition_mm;
                                    if (axis05TargetPosition_mm > _axis05softLimitPlus)
                                        axis05TargetPosition_mm = _axis05softLimitPlus;
                                    axis05TargetPosition_Pulse = Convert.ToInt32(axis05TargetPosition_mm / _axis05mmPP);
                                }
                            }
                            if (axis05TargetPosition_Pulse != axis05CurrentPosition_Pulse)
                            {
                                short smoothCoef = 0;
                                if (useSModeMoveFlag)
                                    smoothCoef = (short)(sMoveTime / 1000);
                                else
                                    smoothCoef = 0;
                                NMCLib20.NMC_MtMovePtpAbs(axishandle[5], ((double)axis05MaxVelocity_Temp_pps / 1000) / ((double)axis05TargetAccTime_s * 1000), ((double)axis05MaxVelocity_Temp_pps / 1000) / ((double)axis05TargetAccTime_s * 1000), (double)axis05StartVelocity_Temp_pps / 1000,
                                    (double)axis05StartVelocity_Temp_pps / 1000, (double)axis05MaxVelocity_Temp_pps / 1000, smoothCoef, axis05TargetPosition_Pulse);
                            }
                        }
                        else//没有回原点
                        {
                            if (!jogDirection)//负方向
                            {
                                if (!inch)//点动
                                {
                                    short smoothCoef = 0;
                                    if (useSModeMoveFlag)
                                        smoothCoef = (short)(sMoveTime / 1000);
                                    else
                                        smoothCoef = 0;
                                    NMCLib20.NMC_MtMoveJog(axishandle[5], ((double)axis05MaxVelocity_Temp_pps / 1000) / ((double)axis05TargetAccTime_s * 1000), ((double)axis05MaxVelocity_Temp_pps / 1000) / ((double)axis05TargetAccTime_s * 1000), (double)axis05MaxVelocity_Temp_pps / 1000 * -1, smoothCoef, 0);
                                }
                                else//寸动
                                {
                                    axis05TargetPosition_Pulse = Convert.ToInt32(axis05TargetPosition_mm * -1 / _axis05mmPP);
                                    short smoothCoef = 0;
                                    if (useSModeMoveFlag)
                                        smoothCoef = (short)(sMoveTime / 1000);
                                    else
                                        smoothCoef = 0;
                                    NMCLib20.NMC_MtMovePtpRel(axishandle[5], ((double)axis05MaxVelocity_Temp_pps / 1000) / ((double)axis05TargetAccTime_s * 1000), ((double)axis05MaxVelocity_Temp_pps / 1000) / ((double)axis05TargetAccTime_s * 1000), (double)axis05StartVelocity_Temp_pps / 1000,
                                        (double)axis05StartVelocity_Temp_pps / 1000, (double)axis05MaxVelocity_Temp_pps / 1000, smoothCoef, axis05TargetPosition_Pulse);
                                }
                            }
                            else//正方向
                            {
                                if (!inch)//点动
                                {
                                    short smoothCoef = 0;
                                    if (useSModeMoveFlag)
                                        smoothCoef = (short)(sMoveTime / 1000);
                                    else
                                        smoothCoef = 0;
                                    NMCLib20.NMC_MtMoveJog(axishandle[5], ((double)axis05MaxVelocity_Temp_pps / 1000) / ((double)axis05TargetAccTime_s * 1000), ((double)axis05MaxVelocity_Temp_pps / 1000) / ((double)axis05TargetAccTime_s * 1000), (double)axis05MaxVelocity_Temp_pps / 1000, smoothCoef, 0);
                                }
                                else//寸动
                                {
                                    axis05TargetPosition_Pulse = Convert.ToInt32(axis05TargetPosition_mm / _axis05mmPP);
                                    short smoothCoef = 0;
                                    if (useSModeMoveFlag)
                                        smoothCoef = (short)(sMoveTime / 1000);
                                    else
                                        smoothCoef = 0;
                                    NMCLib20.NMC_MtMovePtpRel(axishandle[5], ((double)axis05MaxVelocity_Temp_pps / 1000) / ((double)axis05TargetAccTime_s * 1000), ((double)axis05MaxVelocity_Temp_pps / 1000) / ((double)axis05TargetAccTime_s * 1000), (double)axis05StartVelocity_Temp_pps / 1000,
                                        (double)axis05StartVelocity_Temp_pps / 1000, (double)axis05MaxVelocity_Temp_pps / 1000, smoothCoef, axis05TargetPosition_Pulse);
                                }
                            }
                        }
                        break;
                    case 6:
                        if (_axis06HomedFlag)//已回原点
                        {
                            if (!jogDirection)//负方向
                            {
                                if (!inch)//点动
                                    axis06TargetPosition_Pulse = Convert.ToInt32(_axis06softLimitMinus / _axis06mmPP);
                                else//寸动
                                {
                                    axis06TargetPosition_mm = _axis06CurrentPosition_mm - axis06TargetPosition_mm;
                                    if (axis06TargetPosition_mm < _axis06softLimitMinus)
                                        axis06TargetPosition_mm = _axis06softLimitMinus;
                                    axis06TargetPosition_Pulse = Convert.ToInt32(axis06TargetPosition_mm / _axis06mmPP);
                                }
                            }
                            else//正方向
                            {
                                if (!inch)//点动
                                    axis06TargetPosition_Pulse = Convert.ToInt32(_axis06softLimitPlus / _axis06mmPP);
                                else//寸动
                                {
                                    axis06TargetPosition_mm = _axis06CurrentPosition_mm + axis06TargetPosition_mm;
                                    if (axis06TargetPosition_mm > _axis06softLimitPlus)
                                        axis06TargetPosition_mm = _axis06softLimitPlus;
                                    axis06TargetPosition_Pulse = Convert.ToInt32(axis06TargetPosition_mm / _axis06mmPP);
                                }
                            }
                            if (axis06TargetPosition_Pulse != axis06CurrentPosition_Pulse)
                            {
                                short smoothCoef = 0;
                                if (useSModeMoveFlag)
                                    smoothCoef = (short)(sMoveTime / 1000);
                                else
                                    smoothCoef = 0;
                                NMCLib20.NMC_MtMovePtpAbs(axishandle[6], ((double)axis06MaxVelocity_Temp_pps / 1000) / ((double)axis06TargetAccTime_s * 1000), ((double)axis06MaxVelocity_Temp_pps / 1000) / ((double)axis06TargetAccTime_s * 1000), (double)axis06StartVelocity_Temp_pps / 1000,
                                    (double)axis06StartVelocity_Temp_pps / 1000, (double)axis06MaxVelocity_Temp_pps / 1000, smoothCoef, axis06TargetPosition_Pulse);
                            }
                        }
                        else//没有回原点
                        {
                            if (!jogDirection)//负方向
                            {
                                if (!inch)//点动
                                {
                                    short smoothCoef = 0;
                                    if (useSModeMoveFlag)
                                        smoothCoef = (short)(sMoveTime / 1000);
                                    else
                                        smoothCoef = 0;
                                    NMCLib20.NMC_MtMoveJog(axishandle[6], ((double)axis06MaxVelocity_Temp_pps / 1000) / ((double)axis06TargetAccTime_s * 1000), ((double)axis06MaxVelocity_Temp_pps / 1000) / ((double)axis06TargetAccTime_s * 1000), (double)axis06MaxVelocity_Temp_pps / 1000 * -1, smoothCoef, 0);
                                }
                                else//寸动
                                {
                                    axis06TargetPosition_Pulse = Convert.ToInt32(axis06TargetPosition_mm * -1 / _axis06mmPP);
                                    short smoothCoef = 0;
                                    if (useSModeMoveFlag)
                                        smoothCoef = (short)(sMoveTime / 1000);
                                    else
                                        smoothCoef = 0;
                                    NMCLib20.NMC_MtMovePtpRel(axishandle[6], ((double)axis06MaxVelocity_Temp_pps / 1000) / ((double)axis06TargetAccTime_s * 1000), ((double)axis06MaxVelocity_Temp_pps / 1000) / ((double)axis06TargetAccTime_s * 1000), (double)axis06StartVelocity_Temp_pps / 1000,
                                        (double)axis06StartVelocity_Temp_pps / 1000, (double)axis06MaxVelocity_Temp_pps / 1000, smoothCoef, axis06TargetPosition_Pulse);
                                }
                            }
                            else//正方向
                            {
                                if (!inch)//点动
                                {
                                    short smoothCoef = 0;
                                    if (useSModeMoveFlag)
                                        smoothCoef = (short)(sMoveTime / 1000);
                                    else
                                        smoothCoef = 0;
                                    NMCLib20.NMC_MtMoveJog(axishandle[6], ((double)axis06MaxVelocity_Temp_pps / 1000) / ((double)axis06TargetAccTime_s * 1000), ((double)axis06MaxVelocity_Temp_pps / 1000) / ((double)axis06TargetAccTime_s * 1000), (double)axis06MaxVelocity_Temp_pps / 1000, smoothCoef, 0);
                                }
                                else//寸动
                                {
                                    axis06TargetPosition_Pulse = Convert.ToInt32(axis06TargetPosition_mm / _axis06mmPP);
                                    short smoothCoef = 0;
                                    if (useSModeMoveFlag)
                                        smoothCoef = (short)(sMoveTime / 1000);
                                    else
                                        smoothCoef = 0;
                                    NMCLib20.NMC_MtMovePtpRel(axishandle[6], ((double)axis06MaxVelocity_Temp_pps / 1000) / ((double)axis06TargetAccTime_s * 1000), ((double)axis06MaxVelocity_Temp_pps / 1000) / ((double)axis06TargetAccTime_s * 1000), (double)axis06StartVelocity_Temp_pps / 1000,
                                        (double)axis06StartVelocity_Temp_pps / 1000, (double)axis06MaxVelocity_Temp_pps / 1000, smoothCoef, axis06TargetPosition_Pulse);
                                }
                            }
                        }
                        break;
                    case 7:
                        if (_axis07HomedFlag)//已回原点
                        {
                            if (!jogDirection)//负方向
                            {
                                if (!inch)//点动
                                    axis07TargetPosition_Pulse = Convert.ToInt32(_axis07softLimitMinus / _axis07mmPP);
                                else//寸动
                                {
                                    axis07TargetPosition_mm = _axis07CurrentPosition_mm - axis07TargetPosition_mm;
                                    if (axis07TargetPosition_mm < _axis07softLimitMinus)
                                        axis07TargetPosition_mm = _axis07softLimitMinus;
                                    axis07TargetPosition_Pulse = Convert.ToInt32(axis07TargetPosition_mm / _axis07mmPP);
                                }
                            }
                            else//正方向
                            {
                                if (!inch)//点动
                                    axis07TargetPosition_Pulse = Convert.ToInt32(_axis07softLimitPlus / _axis07mmPP);
                                else//寸动
                                {
                                    axis07TargetPosition_mm = _axis07CurrentPosition_mm + axis07TargetPosition_mm;
                                    if (axis07TargetPosition_mm > _axis07softLimitPlus)
                                        axis07TargetPosition_mm = _axis07softLimitPlus;
                                    axis07TargetPosition_Pulse = Convert.ToInt32(axis07TargetPosition_mm / _axis07mmPP);
                                }
                            }
                            if (axis07TargetPosition_Pulse != axis07CurrentPosition_Pulse)
                            {
                                short smoothCoef = 0;
                                if (useSModeMoveFlag)
                                    smoothCoef = (short)(sMoveTime / 1000);
                                else
                                    smoothCoef = 0;
                                NMCLib20.NMC_MtMovePtpAbs(axishandle[7], ((double)axis07MaxVelocity_Temp_pps / 1000) / ((double)axis07TargetAccTime_s * 1000), ((double)axis07MaxVelocity_Temp_pps / 1000) / ((double)axis07TargetAccTime_s * 1000), (double)axis07StartVelocity_Temp_pps / 1000,
                                    (double)axis07StartVelocity_Temp_pps / 1000, (double)axis07MaxVelocity_Temp_pps / 1000, smoothCoef, axis07TargetPosition_Pulse);
                            }
                        }
                        else//没有回原点
                        {
                            if (!jogDirection)//负方向
                            {
                                if (!inch)//点动
                                {
                                    short smoothCoef = 0;
                                    if (useSModeMoveFlag)
                                        smoothCoef = (short)(sMoveTime / 1000);
                                    else
                                        smoothCoef = 0;
                                    NMCLib20.NMC_MtMoveJog(axishandle[7], ((double)axis07MaxVelocity_Temp_pps / 1000) / ((double)axis07TargetAccTime_s * 1000), ((double)axis07MaxVelocity_Temp_pps / 1000) / ((double)axis07TargetAccTime_s * 1000), (double)axis07MaxVelocity_Temp_pps / 1000 * -1, smoothCoef, 0);
                                }
                                else//寸动
                                {
                                    axis07TargetPosition_Pulse = Convert.ToInt32(axis07TargetPosition_mm * -1 / _axis07mmPP);
                                    short smoothCoef = 0;
                                    if (useSModeMoveFlag)
                                        smoothCoef = (short)(sMoveTime / 1000);
                                    else
                                        smoothCoef = 0;
                                    NMCLib20.NMC_MtMovePtpRel(axishandle[7], ((double)axis07MaxVelocity_Temp_pps / 1000) / ((double)axis07TargetAccTime_s * 1000), ((double)axis07MaxVelocity_Temp_pps / 1000) / ((double)axis07TargetAccTime_s * 1000), (double)axis07StartVelocity_Temp_pps / 1000,
                                        (double)axis07StartVelocity_Temp_pps / 1000, (double)axis07MaxVelocity_Temp_pps / 1000, smoothCoef, axis07TargetPosition_Pulse);
                                }
                            }
                            else//正方向
                            {
                                if (!inch)//点动
                                {
                                    short smoothCoef = 0;
                                    if (useSModeMoveFlag)
                                        smoothCoef = (short)(sMoveTime / 1000);
                                    else
                                        smoothCoef = 0;
                                    NMCLib20.NMC_MtMoveJog(axishandle[7], ((double)axis07MaxVelocity_Temp_pps / 1000) / ((double)axis07TargetAccTime_s * 1000), ((double)axis07MaxVelocity_Temp_pps / 1000) / ((double)axis07TargetAccTime_s * 1000), (double)axis07MaxVelocity_Temp_pps / 1000, smoothCoef, 0);
                                }
                                else//寸动
                                {
                                    axis07TargetPosition_Pulse = Convert.ToInt32(axis07TargetPosition_mm / _axis07mmPP);
                                    short smoothCoef = 0;
                                    if (useSModeMoveFlag)
                                        smoothCoef = (short)(sMoveTime / 1000);
                                    else
                                        smoothCoef = 0;
                                    NMCLib20.NMC_MtMovePtpRel(axishandle[7], ((double)axis07MaxVelocity_Temp_pps / 1000) / ((double)axis07TargetAccTime_s * 1000), ((double)axis07MaxVelocity_Temp_pps / 1000) / ((double)axis07TargetAccTime_s * 1000), (double)axis07StartVelocity_Temp_pps / 1000,
                                        (double)axis07StartVelocity_Temp_pps / 1000, (double)axis07MaxVelocity_Temp_pps / 1000, smoothCoef, axis07TargetPosition_Pulse);
                                }
                            }
                        }
                        break;
                    case 8:
                        if (_axis08HomedFlag)//已回原点
                        {
                            if (!jogDirection)//负方向
                            {
                                if (!inch)//点动
                                    axis08TargetPosition_Pulse = Convert.ToInt32(_axis08softLimitMinus / _axis08mmPP);
                                else//寸动
                                {
                                    axis08TargetPosition_mm = _axis08CurrentPosition_mm - axis08TargetPosition_mm;
                                    if (axis08TargetPosition_mm < _axis08softLimitMinus)
                                        axis08TargetPosition_mm = _axis08softLimitMinus;
                                    axis08TargetPosition_Pulse = Convert.ToInt32(axis08TargetPosition_mm / _axis08mmPP);
                                }
                            }
                            else//正方向
                            {
                                if (!inch)//点动
                                    axis08TargetPosition_Pulse = Convert.ToInt32(_axis08softLimitPlus / _axis08mmPP);
                                else//寸动
                                {
                                    axis08TargetPosition_mm = _axis08CurrentPosition_mm + axis08TargetPosition_mm;
                                    if (axis08TargetPosition_mm > _axis08softLimitPlus)
                                        axis08TargetPosition_mm = _axis08softLimitPlus;
                                    axis08TargetPosition_Pulse = Convert.ToInt32(axis08TargetPosition_mm / _axis08mmPP);
                                }
                            }
                            if (axis08TargetPosition_Pulse != axis08CurrentPosition_Pulse)
                            {
                                short smoothCoef = 0;
                                if (useSModeMoveFlag)
                                    smoothCoef = (short)(sMoveTime / 1000);
                                else
                                    smoothCoef = 0;
                                NMCLib20.NMC_MtMovePtpAbs(axishandle[8], ((double)axis08MaxVelocity_Temp_pps / 1000) / ((double)axis08TargetAccTime_s * 1000), ((double)axis08MaxVelocity_Temp_pps / 1000) / ((double)axis08TargetAccTime_s * 1000), (double)axis08StartVelocity_Temp_pps / 1000,
                                    (double)axis08StartVelocity_Temp_pps / 1000, (double)axis08MaxVelocity_Temp_pps / 1000, smoothCoef, axis08TargetPosition_Pulse);
                            }
                        }
                        else//没有回原点
                        {
                            if (!jogDirection)//负方向
                            {
                                if (!inch)//点动
                                {
                                    short smoothCoef = 0;
                                    if (useSModeMoveFlag)
                                        smoothCoef = (short)(sMoveTime / 1000);
                                    else
                                        smoothCoef = 0;
                                    NMCLib20.NMC_MtMoveJog(axishandle[8], ((double)axis08MaxVelocity_Temp_pps / 1000) / ((double)axis08TargetAccTime_s * 1000), ((double)axis08MaxVelocity_Temp_pps / 1000) / ((double)axis08TargetAccTime_s * 1000), (double)axis08MaxVelocity_Temp_pps / 1000 * -1, smoothCoef, 0);
                                }
                                else//寸动
                                {
                                    axis08TargetPosition_Pulse = Convert.ToInt32(axis08TargetPosition_mm * -1 / _axis08mmPP);
                                    short smoothCoef = 0;
                                    if (useSModeMoveFlag)
                                        smoothCoef = (short)(sMoveTime / 1000);
                                    else
                                        smoothCoef = 0;
                                    NMCLib20.NMC_MtMovePtpRel(axishandle[8], ((double)axis08MaxVelocity_Temp_pps / 1000) / ((double)axis08TargetAccTime_s * 1000), ((double)axis08MaxVelocity_Temp_pps / 1000) / ((double)axis08TargetAccTime_s * 1000), (double)axis08StartVelocity_Temp_pps / 1000,
                                        (double)axis08StartVelocity_Temp_pps / 1000, (double)axis08MaxVelocity_Temp_pps / 1000, smoothCoef, axis08TargetPosition_Pulse);
                                }
                            }
                            else//正方向
                            {
                                if (!inch)//点动
                                {
                                    short smoothCoef = 0;
                                    if (useSModeMoveFlag)
                                        smoothCoef = (short)(sMoveTime / 1000);
                                    else
                                        smoothCoef = 0;
                                    NMCLib20.NMC_MtMoveJog(axishandle[8], ((double)axis08MaxVelocity_Temp_pps / 1000) / ((double)axis08TargetAccTime_s * 1000), ((double)axis08MaxVelocity_Temp_pps / 1000) / ((double)axis08TargetAccTime_s * 1000), (double)axis08MaxVelocity_Temp_pps / 1000, smoothCoef, 0);
                                }
                                else//寸动
                                {
                                    axis08TargetPosition_Pulse = Convert.ToInt32(axis08TargetPosition_mm / _axis08mmPP);
                                    short smoothCoef = 0;
                                    if (useSModeMoveFlag)
                                        smoothCoef = (short)(sMoveTime / 1000);
                                    else
                                        smoothCoef = 0;
                                    NMCLib20.NMC_MtMovePtpRel(axishandle[8], ((double)axis08MaxVelocity_Temp_pps / 1000) / ((double)axis08TargetAccTime_s * 1000), ((double)axis08MaxVelocity_Temp_pps / 1000) / ((double)axis08TargetAccTime_s * 1000), (double)axis08StartVelocity_Temp_pps / 1000,
                                        (double)axis08StartVelocity_Temp_pps / 1000, (double)axis08MaxVelocity_Temp_pps / 1000, smoothCoef, axis08TargetPosition_Pulse);
                                }
                            }
                        }
                        break;
                    case 9:
                        if (_axis09HomedFlag)//已回原点
                        {
                            if (!jogDirection)//负方向
                            {
                                if (!inch)//点动
                                    axis09TargetPosition_Pulse = Convert.ToInt32(_axis09softLimitMinus / _axis09mmPP);
                                else//寸动
                                {
                                    axis09TargetPosition_mm = _axis09CurrentPosition_mm - axis09TargetPosition_mm;
                                    if (axis09TargetPosition_mm < _axis09softLimitMinus)
                                        axis09TargetPosition_mm = _axis09softLimitMinus;
                                    axis09TargetPosition_Pulse = Convert.ToInt32(axis09TargetPosition_mm / _axis09mmPP);
                                }
                            }
                            else//正方向
                            {
                                if (!inch)//点动
                                    axis09TargetPosition_Pulse = Convert.ToInt32(_axis09softLimitPlus / _axis09mmPP);
                                else//寸动
                                {
                                    axis09TargetPosition_mm = _axis09CurrentPosition_mm + axis09TargetPosition_mm;
                                    if (axis09TargetPosition_mm > _axis09softLimitPlus)
                                        axis09TargetPosition_mm = _axis09softLimitPlus;
                                    axis09TargetPosition_Pulse = Convert.ToInt32(axis09TargetPosition_mm / _axis09mmPP);
                                }
                            }
                            if (axis09TargetPosition_Pulse != axis09CurrentPosition_Pulse)
                            {
                                short smoothCoef = 0;
                                if (useSModeMoveFlag)
                                    smoothCoef = (short)(sMoveTime / 1000);
                                else
                                    smoothCoef = 0;
                                NMCLib20.NMC_MtMovePtpAbs(axishandle[9], ((double)axis09MaxVelocity_Temp_pps / 1000) / ((double)axis09TargetAccTime_s * 1000), ((double)axis09MaxVelocity_Temp_pps / 1000) / ((double)axis09TargetAccTime_s * 1000), (double)axis09StartVelocity_Temp_pps / 1000,
                                    (double)axis09StartVelocity_Temp_pps / 1000, (double)axis09MaxVelocity_Temp_pps / 1000, smoothCoef, axis09TargetPosition_Pulse);
                            }
                        }
                        else//没有回原点
                        {
                            if (!jogDirection)//负方向
                            {
                                if (!inch)//点动
                                {
                                    short smoothCoef = 0;
                                    if (useSModeMoveFlag)
                                        smoothCoef = (short)(sMoveTime / 1000);
                                    else
                                        smoothCoef = 0;
                                    NMCLib20.NMC_MtMoveJog(axishandle[9], ((double)axis09MaxVelocity_Temp_pps / 1000) / ((double)axis09TargetAccTime_s * 1000), ((double)axis09MaxVelocity_Temp_pps / 1000) / ((double)axis09TargetAccTime_s * 1000), (double)axis09MaxVelocity_Temp_pps / 1000 * -1, smoothCoef, 0);
                                }
                                else//寸动
                                {
                                    axis09TargetPosition_Pulse = Convert.ToInt32(axis09TargetPosition_mm * -1 / _axis09mmPP);
                                    short smoothCoef = 0;
                                    if (useSModeMoveFlag)
                                        smoothCoef = (short)(sMoveTime / 1000);
                                    else
                                        smoothCoef = 0;
                                    NMCLib20.NMC_MtMovePtpRel(axishandle[9], ((double)axis09MaxVelocity_Temp_pps / 1000) / ((double)axis09TargetAccTime_s * 1000), ((double)axis09MaxVelocity_Temp_pps / 1000) / ((double)axis09TargetAccTime_s * 1000), (double)axis09StartVelocity_Temp_pps / 1000,
                                        (double)axis09StartVelocity_Temp_pps / 1000, (double)axis09MaxVelocity_Temp_pps / 1000, smoothCoef, axis09TargetPosition_Pulse);
                                }
                            }
                            else//正方向
                            {
                                if (!inch)//点动
                                {
                                    short smoothCoef = 0;
                                    if (useSModeMoveFlag)
                                        smoothCoef = (short)(sMoveTime / 1000);
                                    else
                                        smoothCoef = 0;
                                    NMCLib20.NMC_MtMoveJog(axishandle[9], ((double)axis09MaxVelocity_Temp_pps / 1000) / ((double)axis09TargetAccTime_s * 1000), ((double)axis09MaxVelocity_Temp_pps / 1000) / ((double)axis09TargetAccTime_s * 1000), (double)axis09MaxVelocity_Temp_pps / 1000, smoothCoef, 0);
                                }
                                else//寸动
                                {
                                    axis09TargetPosition_Pulse = Convert.ToInt32(axis09TargetPosition_mm / _axis09mmPP);
                                    short smoothCoef = 0;
                                    if (useSModeMoveFlag)
                                        smoothCoef = (short)(sMoveTime / 1000);
                                    else
                                        smoothCoef = 0;
                                    NMCLib20.NMC_MtMovePtpRel(axishandle[9], ((double)axis09MaxVelocity_Temp_pps / 1000) / ((double)axis09TargetAccTime_s * 1000), ((double)axis09MaxVelocity_Temp_pps / 1000) / ((double)axis09TargetAccTime_s * 1000), (double)axis09StartVelocity_Temp_pps / 1000,
                                        (double)axis09StartVelocity_Temp_pps / 1000, (double)axis09MaxVelocity_Temp_pps / 1000, smoothCoef, axis09TargetPosition_Pulse);
                                }
                            }
                        }
                        break;
                    case 10:
                        if (_axis10HomedFlag)//已回原点
                        {
                            if (!jogDirection)//负方向
                            {
                                if (!inch)//点动
                                    axis10TargetPosition_Pulse = Convert.ToInt32(_axis10softLimitMinus / _axis10mmPP);
                                else//寸动
                                {
                                    axis10TargetPosition_mm = _axis10CurrentPosition_mm - axis10TargetPosition_mm;
                                    if (axis10TargetPosition_mm < _axis10softLimitMinus)
                                        axis10TargetPosition_mm = _axis10softLimitMinus;
                                    axis10TargetPosition_Pulse = Convert.ToInt32(axis10TargetPosition_mm / _axis10mmPP);
                                }
                            }
                            else//正方向
                            {
                                if (!inch)//点动
                                    axis10TargetPosition_Pulse = Convert.ToInt32(_axis10softLimitPlus / _axis10mmPP);
                                else//寸动
                                {
                                    axis10TargetPosition_mm = _axis10CurrentPosition_mm + axis10TargetPosition_mm;
                                    if (axis10TargetPosition_mm > _axis10softLimitPlus)
                                        axis10TargetPosition_mm = _axis10softLimitPlus;
                                    axis10TargetPosition_Pulse = Convert.ToInt32(axis10TargetPosition_mm / _axis10mmPP);
                                }
                            }
                            if (axis10TargetPosition_Pulse != axis10CurrentPosition_Pulse)
                            {
                                short smoothCoef = 0;
                                if (useSModeMoveFlag)
                                    smoothCoef = (short)(sMoveTime / 1000);
                                else
                                    smoothCoef = 0;
                                NMCLib20.NMC_MtMovePtpAbs(axishandle[10], ((double)axis10MaxVelocity_Temp_pps / 1000) / ((double)axis10TargetAccTime_s * 1000), ((double)axis10MaxVelocity_Temp_pps / 1000) / ((double)axis10TargetAccTime_s * 1000), (double)axis10StartVelocity_Temp_pps / 1000,
                                    (double)axis10StartVelocity_Temp_pps / 1000, (double)axis10MaxVelocity_Temp_pps / 1000, smoothCoef, axis10TargetPosition_Pulse);
                            }
                        }
                        else//没有回原点
                        {
                            if (!jogDirection)//负方向
                            {
                                if (!inch)//点动
                                {
                                    short smoothCoef = 0;
                                    if (useSModeMoveFlag)
                                        smoothCoef = (short)(sMoveTime / 1000);
                                    else
                                        smoothCoef = 0;
                                    NMCLib20.NMC_MtMoveJog(axishandle[10], ((double)axis10MaxVelocity_Temp_pps / 1000) / ((double)axis10TargetAccTime_s * 1000), ((double)axis10MaxVelocity_Temp_pps / 1000) / ((double)axis10TargetAccTime_s * 1000), (double)axis10MaxVelocity_Temp_pps / 1000 * -1, smoothCoef, 0);
                                }
                                else//寸动
                                {
                                    axis10TargetPosition_Pulse = Convert.ToInt32(axis10TargetPosition_mm * -1 / _axis10mmPP);
                                    short smoothCoef = 0;
                                    if (useSModeMoveFlag)
                                        smoothCoef = (short)(sMoveTime / 1000);
                                    else
                                        smoothCoef = 0;
                                    NMCLib20.NMC_MtMovePtpRel(axishandle[10], ((double)axis10MaxVelocity_Temp_pps / 1000) / ((double)axis10TargetAccTime_s * 1000), ((double)axis10MaxVelocity_Temp_pps / 1000) / ((double)axis10TargetAccTime_s * 1000), (double)axis10StartVelocity_Temp_pps / 1000,
                                        (double)axis10StartVelocity_Temp_pps / 1000, (double)axis10MaxVelocity_Temp_pps / 1000, smoothCoef, axis10TargetPosition_Pulse);
                                }
                            }
                            else//正方向
                            {
                                if (!inch)//点动
                                {
                                    short smoothCoef = 0;
                                    if (useSModeMoveFlag)
                                        smoothCoef = (short)(sMoveTime / 1000);
                                    else
                                        smoothCoef = 0;
                                    NMCLib20.NMC_MtMoveJog(axishandle[10], ((double)axis10MaxVelocity_Temp_pps / 1000) / ((double)axis10TargetAccTime_s * 1000), ((double)axis10MaxVelocity_Temp_pps / 1000) / ((double)axis10TargetAccTime_s * 1000), (double)axis10MaxVelocity_Temp_pps / 1000, smoothCoef, 0);
                                }
                                else//寸动
                                {
                                    axis10TargetPosition_Pulse = Convert.ToInt32(axis10TargetPosition_mm / _axis10mmPP);
                                    short smoothCoef = 0;
                                    if (useSModeMoveFlag)
                                        smoothCoef = (short)(sMoveTime / 1000);
                                    else
                                        smoothCoef = 0;
                                    NMCLib20.NMC_MtMovePtpRel(axishandle[10], ((double)axis10MaxVelocity_Temp_pps / 1000) / ((double)axis10TargetAccTime_s * 1000), ((double)axis10MaxVelocity_Temp_pps / 1000) / ((double)axis10TargetAccTime_s * 1000), (double)axis10StartVelocity_Temp_pps / 1000,
                                        (double)axis10StartVelocity_Temp_pps / 1000, (double)axis10MaxVelocity_Temp_pps / 1000, smoothCoef, axis10TargetPosition_Pulse);
                                }
                            }
                        }
                        break;
                    case 11:
                        if (_axis11HomedFlag)//已回原点
                        {
                            if (!jogDirection)//负方向
                            {
                                if (!inch)//点动
                                    axis11TargetPosition_Pulse = Convert.ToInt32(_axis11softLimitMinus / _axis11mmPP);
                                else//寸动
                                {
                                    axis11TargetPosition_mm = _axis11CurrentPosition_mm - axis11TargetPosition_mm;
                                    if (axis11TargetPosition_mm < _axis11softLimitMinus)
                                        axis11TargetPosition_mm = _axis11softLimitMinus;
                                    axis11TargetPosition_Pulse = Convert.ToInt32(axis11TargetPosition_mm / _axis11mmPP);
                                }
                            }
                            else//正方向
                            {
                                if (!inch)//点动
                                    axis11TargetPosition_Pulse = Convert.ToInt32(_axis11softLimitPlus / _axis11mmPP);
                                else//寸动
                                {
                                    axis11TargetPosition_mm = _axis11CurrentPosition_mm + axis11TargetPosition_mm;
                                    if (axis11TargetPosition_mm > _axis11softLimitPlus)
                                        axis11TargetPosition_mm = _axis11softLimitPlus;
                                    axis11TargetPosition_Pulse = Convert.ToInt32(axis11TargetPosition_mm / _axis11mmPP);
                                }
                            }
                            if (axis11TargetPosition_Pulse != axis11CurrentPosition_Pulse)
                            {
                                short smoothCoef = 0;
                                if (useSModeMoveFlag)
                                    smoothCoef = (short)(sMoveTime / 1000);
                                else
                                    smoothCoef = 0;
                                NMCLib20.NMC_MtMovePtpAbs(axishandle[11], ((double)axis11MaxVelocity_Temp_pps / 1000) / ((double)axis11TargetAccTime_s * 1000), ((double)axis11MaxVelocity_Temp_pps / 1000) / ((double)axis11TargetAccTime_s * 1000), (double)axis11StartVelocity_Temp_pps / 1000,
                                    (double)axis11StartVelocity_Temp_pps / 1000, (double)axis11MaxVelocity_Temp_pps / 1000, smoothCoef, axis11TargetPosition_Pulse);
                            }
                        }
                        else//没有回原点
                        {
                            if (!jogDirection)//负方向
                            {
                                if (!inch)//点动
                                {
                                    short smoothCoef = 0;
                                    if (useSModeMoveFlag)
                                        smoothCoef = (short)(sMoveTime / 1000);
                                    else
                                        smoothCoef = 0;
                                    NMCLib20.NMC_MtMoveJog(axishandle[11], ((double)axis11MaxVelocity_Temp_pps / 1000) / ((double)axis11TargetAccTime_s * 1000), ((double)axis11MaxVelocity_Temp_pps / 1000) / ((double)axis11TargetAccTime_s * 1000), (double)axis11MaxVelocity_Temp_pps / 1000 * -1, smoothCoef, 0);
                                }
                                else//寸动
                                {
                                    axis11TargetPosition_Pulse = Convert.ToInt32(axis11TargetPosition_mm * -1 / _axis11mmPP);
                                    short smoothCoef = 0;
                                    if (useSModeMoveFlag)
                                        smoothCoef = (short)(sMoveTime / 1000);
                                    else
                                        smoothCoef = 0;
                                    NMCLib20.NMC_MtMovePtpRel(axishandle[11], ((double)axis11MaxVelocity_Temp_pps / 1000) / ((double)axis11TargetAccTime_s * 1000), ((double)axis11MaxVelocity_Temp_pps / 1000) / ((double)axis11TargetAccTime_s * 1000), (double)axis11StartVelocity_Temp_pps / 1000,
                                        (double)axis11StartVelocity_Temp_pps / 1000, (double)axis11MaxVelocity_Temp_pps / 1000, smoothCoef, axis11TargetPosition_Pulse);
                                }
                            }
                            else//正方向
                            {
                                if (!inch)//点动
                                {
                                    short smoothCoef = 0;
                                    if (useSModeMoveFlag)
                                        smoothCoef = (short)(sMoveTime / 1000);
                                    else
                                        smoothCoef = 0;
                                    NMCLib20.NMC_MtMoveJog(axishandle[11], ((double)axis11MaxVelocity_Temp_pps / 1000) / ((double)axis11TargetAccTime_s * 1000), ((double)axis11MaxVelocity_Temp_pps / 1000) / ((double)axis11TargetAccTime_s * 1000), (double)axis11MaxVelocity_Temp_pps / 1000, smoothCoef, 0);
                                }
                                else//寸动
                                {
                                    axis11TargetPosition_Pulse = Convert.ToInt32(axis11TargetPosition_mm / _axis11mmPP);
                                    short smoothCoef = 0;
                                    if (useSModeMoveFlag)
                                        smoothCoef = (short)(sMoveTime / 1000);
                                    else
                                        smoothCoef = 0;
                                    NMCLib20.NMC_MtMovePtpRel(axishandle[11], ((double)axis11MaxVelocity_Temp_pps / 1000) / ((double)axis11TargetAccTime_s * 1000), ((double)axis11MaxVelocity_Temp_pps / 1000) / ((double)axis11TargetAccTime_s * 1000), (double)axis11StartVelocity_Temp_pps / 1000,
                                        (double)axis11StartVelocity_Temp_pps / 1000, (double)axis11MaxVelocity_Temp_pps / 1000, smoothCoef, axis11TargetPosition_Pulse);
                                }
                            }
                        }
                        break;

                    default:
                        break;
                }
            }
            //1001:减速停止
            if (axisCommand == 1001)
            {
                jogAxisDecelStop(axisNumber);
            }
            //1002:立即停止
            if (axisCommand == 1002)
            {
                jogAxisImmediateStop(axisNumber);
            }
        }
        #endregion

        #region axisActionLineInterpolation:轴直线插补运行(axisCommand取值范围 0:NA;1-900:点位号定位;1000:实时位置数据定位)----只能用于第一块卡
        /// <summary>
        /// axisActionLineInterpolation:轴直线插补运行
        /// </summary>
        /// <param name="axisCommand">
        /// <param name="axisCount">int:轴个数2-4</param>
        /// <param name="axisNumberArray">int[]:轴号0-N,第一个运动控制器的轴号是0-3,第二个是4-7,第三个是8-11,不能跨控制器!</param>
        /// <param name="targetPositionArray">float[]:对应轴号列表各轴的绝对坐标的位置列表</param>
        /// <param name="startOrEndVelocity">float:运动初始或者结束速度(换算到第一个轴号),单位:mmps</param>
        /// <param name="maxVelocity">float:运动速度(换算到第一个轴号),单位:mmps</param>
        /// <param name="accTime">double:加速时间(用第一个轴号的),单位:s</param>
        /// <param name="deviationValueArray">float[]:定位偏差值,单位:mm</param>
        public void axisActionLineInterpolation(int axisCommand, int axisCount, UInt16[] axisNumberArray, float[] targetPositionArray, float startOrEndVelocity, float maxVelocity, double accTime, float[] deviationValueArray)
        {
            int startOrEndVelocityPPS = 1;
            int maxVelocityPPS = 1;
            int[] targetPositionArrayPulse = new int[axisCount];
            bool needRunFlag = false;

            if (axisNumberArray[0] >= 0 && axisNumberArray[0] <= 3)//第一个运动控制器
            {

                for (int i = 0; i < axisCount; i++)
                {
                    if (i == 0)
                    {
                        switch (axisNumberArray[i])
                        {
                            case 0:
                                startOrEndVelocityPPS = Convert.ToInt32(startOrEndVelocity / _axis00mmPP);
                                maxVelocityPPS = Convert.ToInt32(maxVelocity / _axis00mmPP);
                                break;
                            case 1:
                                startOrEndVelocityPPS = Convert.ToInt32(startOrEndVelocity / _axis01mmPP);
                                maxVelocityPPS = Convert.ToInt32(maxVelocity / _axis01mmPP);
                                break;
                            case 2:
                                startOrEndVelocityPPS = Convert.ToInt32(startOrEndVelocity / _axis02mmPP);
                                maxVelocityPPS = Convert.ToInt32(maxVelocity / _axis02mmPP);
                                break;
                            case 3:
                                startOrEndVelocityPPS = Convert.ToInt32(startOrEndVelocity / _axis03mmPP);
                                maxVelocityPPS = Convert.ToInt32(maxVelocity / _axis03mmPP);
                                break;
                        }
                    }
                    switch (axisNumberArray[i])
                    {
                        case 0:
                            axis00TargetPosition_mm = targetPositionArray[i];
                            axis00TargetDeviationValue_mm = deviationValueArray[i];
                            axis00TargetPosition_Pulse = Convert.ToInt32(axis00TargetPosition_mm / _axis00mmPP);
                            targetPositionArrayPulse[i] = axis00TargetPosition_Pulse;
                            if (targetPositionArrayPulse[i] != axis00CurrentPosition_Pulse)
                            {
                                _axis00Command = axisCommand;
                                _axis00CommandSaved = _axis00Command;
                                _axis00CurrentPositionNumber = 0;
                                needRunFlag = true;
                            }
                            break;
                        case 1:
                            axis01TargetPosition_mm = targetPositionArray[i];
                            axis01TargetDeviationValue_mm = deviationValueArray[i];
                            axis01TargetPosition_Pulse = Convert.ToInt32(axis01TargetPosition_mm / _axis01mmPP);
                            targetPositionArrayPulse[i] = axis01TargetPosition_Pulse;
                            if (targetPositionArrayPulse[i] != axis01CurrentPosition_Pulse)
                            {
                                _axis01Command = axisCommand;
                                _axis01CommandSaved = _axis01Command;
                                _axis01CurrentPositionNumber = 0;
                                needRunFlag = true;
                            }
                            break;
                        case 2:
                            axis02TargetPosition_mm = targetPositionArray[i];
                            axis02TargetDeviationValue_mm = deviationValueArray[i];
                            axis02TargetPosition_Pulse = Convert.ToInt32(axis02TargetPosition_mm / _axis02mmPP);
                            targetPositionArrayPulse[i] = axis02TargetPosition_Pulse;
                            if (targetPositionArrayPulse[i] != axis02CurrentPosition_Pulse)
                            {
                                _axis02Command = axisCommand;
                                _axis02CommandSaved = _axis02Command;
                                _axis02CurrentPositionNumber = 0;
                                needRunFlag = true;
                            }
                            break;
                        case 3:
                            axis03TargetPosition_mm = targetPositionArray[i];
                            axis03TargetDeviationValue_mm = deviationValueArray[i];
                            axis03TargetPosition_Pulse = Convert.ToInt32(axis03TargetPosition_mm / _axis03mmPP);
                            targetPositionArrayPulse[i] = axis03TargetPosition_Pulse;
                            if (targetPositionArrayPulse[i] != axis03CurrentPosition_Pulse)
                            {
                                _axis03Command = axisCommand;
                                _axis03CommandSaved = _axis03Command;
                                _axis03CurrentPositionNumber = 0;
                                needRunFlag = true;
                            }
                            break;
                    }
                }

                if (needRunFlag)
                {
                    int _axisMask = 0;
                    for (int i = 0; i < axisNumberArray.Length; i++)
                    {
                        _axisMask = _axisMask | (1 << axisNumberArray[i]);
                    }
                    lineInterpolation(0, (short)_axisMask, targetPositionArrayPulse, startOrEndVelocityPPS / 1000, maxVelocityPPS / 1000, (maxVelocityPPS / 1000) / (accTime * 1000), 0);
                    for (int i = 0; i < axisNumberArray.Length; i++)
                    {
                        switch (axisNumberArray[i])
                        {
                            case 0:
                                _axis00MoveStartFlag = true;
                                break;
                            case 1:
                                _axis01MoveStartFlag = true;
                                break;
                            case 2:
                                _axis02MoveStartFlag = true;
                                break;
                            case 3:
                                _axis03MoveStartFlag = true;
                                break;
                        }
                    }
                }
            }
            if (axisNumberArray[0] >= 4 && axisNumberArray[0] <= 7)//第二个运动控制器
            {

                for (int i = 0; i < axisCount; i++)
                {
                    if (i == 0)
                    {
                        switch (axisNumberArray[i])
                        {
                            case 0:
                                startOrEndVelocityPPS = Convert.ToInt32(startOrEndVelocity / _axis04mmPP);
                                maxVelocityPPS = Convert.ToInt32(maxVelocity / _axis04mmPP);
                                break;
                            case 1:
                                startOrEndVelocityPPS = Convert.ToInt32(startOrEndVelocity / _axis05mmPP);
                                maxVelocityPPS = Convert.ToInt32(maxVelocity / _axis05mmPP);
                                break;
                            case 2:
                                startOrEndVelocityPPS = Convert.ToInt32(startOrEndVelocity / _axis06mmPP);
                                maxVelocityPPS = Convert.ToInt32(maxVelocity / _axis06mmPP);
                                break;
                            case 3:
                                startOrEndVelocityPPS = Convert.ToInt32(startOrEndVelocity / _axis07mmPP);
                                maxVelocityPPS = Convert.ToInt32(maxVelocity / _axis07mmPP);
                                break;
                        }
                    }
                    switch (axisNumberArray[i])
                    {
                        case 0:
                            axis04TargetPosition_mm = targetPositionArray[i];
                            axis04TargetDeviationValue_mm = deviationValueArray[i];
                            axis04TargetPosition_Pulse = Convert.ToInt32(axis04TargetPosition_mm / _axis04mmPP);
                            targetPositionArrayPulse[i] = axis04TargetPosition_Pulse;
                            if (targetPositionArrayPulse[i] != axis04CurrentPosition_Pulse)
                            {
                                _axis04Command = axisCommand;
                                _axis04CommandSaved = _axis04Command;
                                _axis04CurrentPositionNumber = 0;
                                needRunFlag = true;
                            }
                            break;
                        case 1:
                            axis05TargetPosition_mm = targetPositionArray[i];
                            axis05TargetDeviationValue_mm = deviationValueArray[i];
                            axis05TargetPosition_Pulse = Convert.ToInt32(axis05TargetPosition_mm / _axis05mmPP);
                            targetPositionArrayPulse[i] = axis05TargetPosition_Pulse;
                            if (targetPositionArrayPulse[i] != axis05CurrentPosition_Pulse)
                            {
                                _axis05Command = axisCommand;
                                _axis05CommandSaved = _axis05Command;
                                _axis05CurrentPositionNumber = 0;
                                needRunFlag = true;
                            }
                            break;
                        case 2:
                            axis06TargetPosition_mm = targetPositionArray[i];
                            axis06TargetDeviationValue_mm = deviationValueArray[i];
                            axis06TargetPosition_Pulse = Convert.ToInt32(axis06TargetPosition_mm / _axis06mmPP);
                            targetPositionArrayPulse[i] = axis06TargetPosition_Pulse;
                            if (targetPositionArrayPulse[i] != axis06CurrentPosition_Pulse)
                            {
                                _axis06Command = axisCommand;
                                _axis06CommandSaved = _axis06Command;
                                _axis06CurrentPositionNumber = 0;
                                needRunFlag = true;
                            }
                            break;
                        case 3:
                            axis07TargetPosition_mm = targetPositionArray[i];
                            axis07TargetDeviationValue_mm = deviationValueArray[i];
                            axis07TargetPosition_Pulse = Convert.ToInt32(axis07TargetPosition_mm / _axis07mmPP);
                            targetPositionArrayPulse[i] = axis07TargetPosition_Pulse;
                            if (targetPositionArrayPulse[i] != axis07CurrentPosition_Pulse)
                            {
                                _axis07Command = axisCommand;
                                _axis07CommandSaved = _axis07Command;
                                _axis07CurrentPositionNumber = 0;
                                needRunFlag = true;
                            }
                            break;
                    }
                }

                if (needRunFlag)
                {
                    int _axisMask = 0;
                    for (int i = 0; i < axisNumberArray.Length; i++)
                    {
                        _axisMask = _axisMask | (1 << (axisNumberArray[i] - 4));
                    }
                    lineInterpolation(1, (short)_axisMask, targetPositionArrayPulse, startOrEndVelocityPPS / 1000, maxVelocityPPS / 1000, (maxVelocityPPS / 1000) / (accTime * 1000), 0);
                    for (int i = 0; i < axisNumberArray.Length; i++)
                    {
                        switch (axisNumberArray[i])
                        {
                            case 0:
                                _axis04MoveStartFlag = true;
                                break;
                            case 1:
                                _axis05MoveStartFlag = true;
                                break;
                            case 2:
                                _axis06MoveStartFlag = true;
                                break;
                            case 3:
                                _axis07MoveStartFlag = true;
                                break;
                        }
                    }
                }
            }
            if (axisNumberArray[0] >= 8 && axisNumberArray[0] <= 11)//第三个运动控制器
            {

                for (int i = 0; i < axisCount; i++)
                {
                    if (i == 0)
                    {
                        switch (axisNumberArray[i])
                        {
                            case 0:
                                startOrEndVelocityPPS = Convert.ToInt32(startOrEndVelocity / _axis08mmPP);
                                maxVelocityPPS = Convert.ToInt32(maxVelocity / _axis08mmPP);
                                break;
                            case 1:
                                startOrEndVelocityPPS = Convert.ToInt32(startOrEndVelocity / _axis09mmPP);
                                maxVelocityPPS = Convert.ToInt32(maxVelocity / _axis09mmPP);
                                break;
                            case 2:
                                startOrEndVelocityPPS = Convert.ToInt32(startOrEndVelocity / _axis10mmPP);
                                maxVelocityPPS = Convert.ToInt32(maxVelocity / _axis10mmPP);
                                break;
                            case 3:
                                startOrEndVelocityPPS = Convert.ToInt32(startOrEndVelocity / _axis11mmPP);
                                maxVelocityPPS = Convert.ToInt32(maxVelocity / _axis11mmPP);
                                break;
                        }
                    }
                    switch (axisNumberArray[i])
                    {
                        case 0:
                            axis08TargetPosition_mm = targetPositionArray[i];
                            axis08TargetDeviationValue_mm = deviationValueArray[i];
                            axis08TargetPosition_Pulse = Convert.ToInt32(axis08TargetPosition_mm / _axis08mmPP);
                            targetPositionArrayPulse[i] = axis08TargetPosition_Pulse;
                            if (targetPositionArrayPulse[i] != axis08CurrentPosition_Pulse)
                            {
                                _axis08Command = axisCommand;
                                _axis08CommandSaved = _axis08Command;
                                _axis08CurrentPositionNumber = 0;
                                needRunFlag = true;
                            }
                            break;
                        case 1:
                            axis09TargetPosition_mm = targetPositionArray[i];
                            axis09TargetDeviationValue_mm = deviationValueArray[i];
                            axis09TargetPosition_Pulse = Convert.ToInt32(axis09TargetPosition_mm / _axis09mmPP);
                            targetPositionArrayPulse[i] = axis09TargetPosition_Pulse;
                            if (targetPositionArrayPulse[i] != axis09CurrentPosition_Pulse)
                            {
                                _axis09Command = axisCommand;
                                _axis09CommandSaved = _axis09Command;
                                _axis09CurrentPositionNumber = 0;
                                needRunFlag = true;
                            }
                            break;
                        case 2:
                            axis10TargetPosition_mm = targetPositionArray[i];
                            axis10TargetDeviationValue_mm = deviationValueArray[i];
                            axis10TargetPosition_Pulse = Convert.ToInt32(axis10TargetPosition_mm / _axis10mmPP);
                            targetPositionArrayPulse[i] = axis10TargetPosition_Pulse;
                            if (targetPositionArrayPulse[i] != axis10CurrentPosition_Pulse)
                            {
                                _axis10Command = axisCommand;
                                _axis10CommandSaved = _axis10Command;
                                _axis10CurrentPositionNumber = 0;
                                needRunFlag = true;
                            }
                            break;
                        case 3:
                            axis11TargetPosition_mm = targetPositionArray[i];
                            axis11TargetDeviationValue_mm = deviationValueArray[i];
                            axis11TargetPosition_Pulse = Convert.ToInt32(axis11TargetPosition_mm / _axis11mmPP);
                            targetPositionArrayPulse[i] = axis11TargetPosition_Pulse;
                            if (targetPositionArrayPulse[i] != axis11CurrentPosition_Pulse)
                            {
                                _axis11Command = axisCommand;
                                _axis11CommandSaved = _axis11Command;
                                _axis11CurrentPositionNumber = 0;
                                needRunFlag = true;
                            }
                            break;
                    }
                }

                if (needRunFlag)
                {
                    int _axisMask = 0;
                    for (int i = 0; i < axisNumberArray.Length; i++)
                    {
                        _axisMask = _axisMask | (1 << (axisNumberArray[i] - 8));
                    }
                    lineInterpolation(2, (short)_axisMask, targetPositionArrayPulse, startOrEndVelocityPPS / 1000, maxVelocityPPS / 1000, (maxVelocityPPS / 1000) / (accTime * 1000), 0);
                    for (int i = 0; i < axisNumberArray.Length; i++)
                    {
                        switch (axisNumberArray[i])
                        {
                            case 0:
                                _axis08MoveStartFlag = true;
                                break;
                            case 1:
                                _axis09MoveStartFlag = true;
                                break;
                            case 2:
                                _axis10MoveStartFlag = true;
                                break;
                            case 3:
                                _axis11MoveStartFlag = true;
                                break;
                        }
                    }
                }
            }
        }
        #endregion

        #region jogAxisDecelStop
        private async void jogAxisDecelStop(int axisNumber)
        {
            await Task.Run(() =>
            {
                switch (axisNumber)
                {
                    case 0:
                        axisDecelStop(0);
                        while (_axis00CheckDoneStatus == 0)
                        {
                        }
                        _axis00Command = 0;
                        _axis00CommandSaved = 0;
                        break;
                    case 1:
                        axisDecelStop(1);
                        while (_axis01CheckDoneStatus == 0)
                        {
                        }
                        _axis01Command = 0;
                        _axis01CommandSaved = 0;
                        break;
                    case 2:
                        axisDecelStop(2);
                        while (_axis02CheckDoneStatus == 0)
                        {
                        }
                        _axis02Command = 0;
                        _axis02CommandSaved = 0;
                        break;
                    case 3:
                        axisDecelStop(3);
                        while (_axis03CheckDoneStatus == 0)
                        {
                        }
                        _axis03Command = 0;
                        _axis03CommandSaved = 0;
                        break;
                    case 4:
                        axisDecelStop(4);
                        while (_axis04CheckDoneStatus == 0)
                        {
                        }
                        _axis04Command = 0;
                        _axis04CommandSaved = 0;
                        break;
                    case 5:
                        axisDecelStop(5);
                        while (_axis05CheckDoneStatus == 0)
                        {
                        }
                        _axis05Command = 0;
                        _axis05CommandSaved = 0;
                        break;
                    case 6:
                        axisDecelStop(6);
                        while (_axis06CheckDoneStatus == 0)
                        {
                        }
                        _axis06Command = 0;
                        _axis06CommandSaved = 0;
                        break;
                    case 7:
                        axisDecelStop(7);
                        while (_axis07CheckDoneStatus == 0)
                        {
                        }
                        _axis07Command = 0;
                        _axis07CommandSaved = 0;
                        break;
                    case 8:
                        axisDecelStop(8);
                        while (_axis08CheckDoneStatus == 0)
                        {
                        }
                        _axis08Command = 0;
                        _axis08CommandSaved = 0;
                        break;
                    case 9:
                        axisDecelStop(9);
                        while (_axis09CheckDoneStatus == 0)
                        {
                        }
                        _axis09Command = 0;
                        _axis09CommandSaved = 0;
                        break;
                    case 10:
                        axisDecelStop(10);
                        while (_axis10CheckDoneStatus == 0)
                        {
                        }
                        _axis10Command = 0;
                        _axis10CommandSaved = 0;
                        break;
                    case 11:
                        axisDecelStop(11);
                        while (_axis11CheckDoneStatus == 0)
                        {
                        }
                        _axis11Command = 0;
                        _axis11CommandSaved = 0;
                        break;
                }
            });
        }
        #endregion

        #region axisDecelStop:轴减速停止
        /// <summary>
        /// axisDecelStop:轴减速停止
        /// </summary>
        /// <param name="axisNumber">int:轴号</param>
        public int axisDecelStop(int axisNumber)
        {
            return NMCLib20.NMC_MtStop(axishandle[axisNumber]);//减速停止.根据用户设置的减速度进行停止,如果用户没有设置则默认为最大值
        }
        #endregion

        #region jogAxisImmediateStop
        public async void jogAxisImmediateStop(int axisNumber)
        {
            await Task.Run(() =>
            {
                switch (axisNumber)
                {
                    case 0:
                        axisImmediateStop(0);
                        while (_axis00CheckDoneStatus == 0)
                        {
                            Thread.Sleep(10);//不加这个sleep有时候快速停止会出现cpu占比超高的问题
                        }
                        Thread.Sleep(10);
                        _axis00Command = 0;
                        _axis00CommandSaved = 0;
                        break;
                    case 1:
                        axisImmediateStop(1);
                        while (_axis01CheckDoneStatus == 0)
                        {
                            Thread.Sleep(10);
                        }
                        _axis01Command = 0;
                        _axis01CommandSaved = 0;
                        break;
                    case 2:
                        axisImmediateStop(2);
                        while (_axis02CheckDoneStatus == 0)
                        {
                            Thread.Sleep(10);
                        }
                        _axis02Command = 0;
                        _axis02CommandSaved = 0;
                        break;
                    case 3:
                        axisImmediateStop(3);
                        while (_axis03CheckDoneStatus == 0)
                        {
                            Thread.Sleep(10);
                        }
                        _axis03Command = 0;
                        _axis03CommandSaved = 0;
                        break;
                    case 4:
                        axisImmediateStop(4);
                        while (_axis04CheckDoneStatus == 0)
                        {
                            Thread.Sleep(10);
                        }
                        _axis04Command = 0;
                        _axis04CommandSaved = 0;
                        break;
                    case 5:
                        axisImmediateStop(5);
                        while (_axis05CheckDoneStatus == 0)
                        {
                            Thread.Sleep(10);
                        }
                        _axis05Command = 0;
                        _axis05CommandSaved = 0;
                        break;
                    case 6:
                        axisImmediateStop(6);
                        while (_axis06CheckDoneStatus == 0)
                        {
                            Thread.Sleep(10);
                        }
                        _axis06Command = 0;
                        _axis06CommandSaved = 0;
                        break;
                    case 7:
                        axisImmediateStop(7);
                        while (_axis07CheckDoneStatus == 0)
                        {
                            Thread.Sleep(10);
                        }
                        _axis07Command = 0;
                        _axis07CommandSaved = 0;
                        break;
                    case 8:
                        axisImmediateStop(8);
                        while (_axis08CheckDoneStatus == 0)
                        {
                            Thread.Sleep(10);
                        }
                        _axis08Command = 0;
                        _axis08CommandSaved = 0;
                        break;
                    case 9:
                        axisImmediateStop(9);
                        while (_axis09CheckDoneStatus == 0)
                        {
                            Thread.Sleep(10);
                        }
                        _axis09Command = 0;
                        _axis09CommandSaved = 0;
                        break;
                    case 10:
                        axisImmediateStop(10);
                        while (_axis10CheckDoneStatus == 0)
                        {
                            Thread.Sleep(10);
                        }
                        _axis10Command = 0;
                        _axis10CommandSaved = 0;
                        break;
                    case 11:
                        axisImmediateStop(11);
                        while (_axis11CheckDoneStatus == 0)
                        {
                            Thread.Sleep(10);
                        }
                        _axis11Command = 0;
                        _axis11CommandSaved = 0;
                        break;
                }
            });
        }
        #endregion

        #region axisImmediateStop:轴立即停止
        /// <summary>
        /// axisImmediateStop:轴立即停止
        /// </summary>
        /// <param name="axisNumber">int:轴号</param>
        public int axisImmediateStop(int axisNumber)
        {
            return NMCLib20.NMC_MtAbruptStop(axishandle[axisNumber]);//立即停止.不会置起急停标志位,根据用户设置的减速度进行停止,如果用户没有设置则默认为最大值
            //return NMCLib20.NMC_MtEstp(axishandle[axisNumber]);//立即停止.会置起急停标志位,根据用户设置的减速度进行停止,如果用户没有设置则默认为最大值.，需调用NMC_MtClrError(HAND axisHandle)清除状态才能继续运动.
        }
        #endregion

        #region homeAction:回原点
        private async void homeAction(int axisCommand, int axisNumber, float startVelocity, float maxVelocity, double accTime, float targetPosition_mm, bool jogDirection, bool inch, float deviationValue)
        {
            short axsists = 0;
            int _axisCheckDoneStatus = 0;
            NMCLib20.THomeSetting homepara;//回零参数

            float axisHomeFindSensorVelocity = 1;
            float axisHomeBackDistance = 3;

            switch (axisNumber)
            {
                case 0:
                    _axis00HomedFlag = false;
                    axisHomeFindSensorVelocity = _axis00HomeFindSensorVelocity_mmps;
                    axisHomeBackDistance = _axis00HomeBackDistance_mm;
                    break;
                case 1:
                    _axis01HomedFlag = false;
                    axisHomeFindSensorVelocity = _axis01HomeFindSensorVelocity_mmps;
                    axisHomeBackDistance = _axis01HomeBackDistance_mm;
                    break;
                case 2:
                    _axis02HomedFlag = false;
                    axisHomeFindSensorVelocity = _axis02HomeFindSensorVelocity_mmps;
                    axisHomeBackDistance = _axis02HomeBackDistance_mm;
                    break;
                case 3:
                    _axis03HomedFlag = false;
                    axisHomeFindSensorVelocity = _axis03HomeFindSensorVelocity_mmps;
                    axisHomeBackDistance = _axis03HomeBackDistance_mm;
                    break;
                case 4:
                    _axis04HomedFlag = false;
                    axisHomeFindSensorVelocity = _axis04HomeFindSensorVelocity_mmps;
                    axisHomeBackDistance = _axis04HomeBackDistance_mm;
                    break;
                case 5:
                    _axis05HomedFlag = false;
                    axisHomeFindSensorVelocity = _axis05HomeFindSensorVelocity_mmps;
                    axisHomeBackDistance = _axis05HomeBackDistance_mm;
                    break;
                case 6:
                    _axis06HomedFlag = false;
                    axisHomeFindSensorVelocity = _axis06HomeFindSensorVelocity_mmps;
                    axisHomeBackDistance = _axis06HomeBackDistance_mm;
                    break;
                case 7:
                    _axis07HomedFlag = false;
                    axisHomeFindSensorVelocity = _axis07HomeFindSensorVelocity_mmps;
                    axisHomeBackDistance = _axis07HomeBackDistance_mm;
                    break;
                case 8:
                    _axis08HomedFlag = false;
                    axisHomeFindSensorVelocity = _axis08HomeFindSensorVelocity_mmps;
                    axisHomeBackDistance = _axis08HomeBackDistance_mm;
                    break;
                case 9:
                    _axis09HomedFlag = false;
                    axisHomeFindSensorVelocity = _axis09HomeFindSensorVelocity_mmps;
                    axisHomeBackDistance = _axis09HomeBackDistance_mm;
                    break;
                case 10:
                    _axis10HomedFlag = false;
                    axisHomeFindSensorVelocity = _axis10HomeFindSensorVelocity_mmps;
                    axisHomeBackDistance = _axis10HomeBackDistance_mm;
                    break;
                case 11:
                    _axis11HomedFlag = false;
                    axisHomeFindSensorVelocity = _axis11HomeFindSensorVelocity_mmps;
                    axisHomeBackDistance = _axis11HomeBackDistance_mm;
                    break;
            }
            await Task.Run(() =>
            {
                //向负方向Jog移动,寻找原点传感器或者负极限传感器
                AxisAction(901, axisNumber, startVelocity, axisHomeFindSensorVelocity, 0, accTime, 0, 0, false, false, 0, false, 0);
                //等待找到原点传感器或者负极限传感器
                while ((axisNumber == 0 && !_axis00ELMinus && !_axis00ORG) || (axisNumber == 1 && !_axis01ELMinus && !_axis01ORG)
                    || (axisNumber == 2 && !_axis02ELMinus && !_axis02ORG) || (axisNumber == 3 && !_axis03ELMinus && !_axis03ORG)
                    || (axisNumber == 4 && !_axis04ELMinus && !_axis04ORG) || (axisNumber == 5 && !_axis05ELMinus && !_axis05ORG)
                    || (axisNumber == 6 && !_axis06ELMinus && !_axis06ORG) || (axisNumber == 7 && !_axis07ELMinus && !_axis07ORG)
                    || (axisNumber == 8 && !_axis08ELMinus && !_axis08ORG) || (axisNumber == 9 && !_axis09ELMinus && !_axis09ORG)
                    || (axisNumber == 10 && !_axis10ELMinus && !_axis10ORG) || (axisNumber == 11 && !_axis11ELMinus && !_axis11ORG))
                {
                    Thread.Sleep(10);
                }
                axisDecelStop(axisNumber);//找到原点传感器或者负极限传感器,停止移动
                _axisCheckDoneStatus = 0;
                while (_axisCheckDoneStatus == 0)//等待停止完成
                {
                    Thread.Sleep(10);
                    NMCLib20.NMC_MtGetSts(axishandle[axisNumber], ref axsists);
                    _axisCheckDoneStatus = (axsists & (1 << 0)) == 0 ? 1 : 0;//取bit0,0静止,1运动
                }

                //增加以下延时,确保最后停止位置是否处于负极限传感器处
                int i = 0;
                while (i < 30)
                {
                    i++;
                    Thread.Sleep(10);
                }

                bool axis_ELMinus = false;
                bool axis_ORG = false;
                switch (axisNumber)
                {
                    case 0:
                        axis_ELMinus = _axis00ELMinus;
                        axis_ORG = _axis00ORG;
                        break;
                    case 1:
                        axis_ELMinus = _axis01ELMinus;
                        axis_ORG = _axis01ORG;
                        break;
                    case 2:
                        axis_ELMinus = _axis02ELMinus;
                        axis_ORG = _axis02ORG;
                        break;
                    case 3:
                        axis_ELMinus = _axis03ELMinus;
                        axis_ORG = _axis03ORG;
                        break;
                    case 4:
                        axis_ELMinus = _axis04ELMinus;
                        axis_ORG = _axis04ORG;
                        break;
                    case 5:
                        axis_ELMinus = _axis05ELMinus;
                        axis_ORG = _axis05ORG;
                        break;
                    case 6:
                        axis_ELMinus = _axis06ELMinus;
                        axis_ORG = _axis06ORG;
                        break;
                    case 7:
                        axis_ELMinus = _axis07ELMinus;
                        axis_ORG = _axis07ORG;
                        break;
                    case 8:
                        axis_ELMinus = _axis08ELMinus;
                        axis_ORG = _axis08ORG;
                        break;
                    case 9:
                        axis_ELMinus = _axis09ELMinus;
                        axis_ORG = _axis09ORG;
                        break;
                    case 10:
                        axis_ELMinus = _axis10ELMinus;
                        axis_ORG = _axis10ORG;
                        break;
                    case 11:
                        axis_ELMinus = _axis11ELMinus;
                        axis_ORG = _axis11ORG;
                        break;
                }

                //如果找到的是负极限传感器
                if (axis_ELMinus)
                {
                    //向正方向Jog移动,寻找原点传感器
                    AxisAction(901, axisNumber, startVelocity, axisHomeFindSensorVelocity, 0, accTime, 0, 0, true, false, 0, false, 0);
                    while ((axisNumber == 0 && !_axis00ORG) || (axisNumber == 1 && !_axis01ORG)
                        || (axisNumber == 2 && !_axis02ORG) || (axisNumber == 3 && !_axis03ORG)
                        || (axisNumber == 4 && !_axis04ORG) || (axisNumber == 5 && !_axis05ORG)
                        || (axisNumber == 6 && !_axis06ORG) || (axisNumber == 7 && !_axis07ORG)
                        || (axisNumber == 8 && !_axis08ORG) || (axisNumber == 9 && !_axis09ORG)
                        || (axisNumber == 10 && !_axis10ORG) || (axisNumber == 11 && !_axis11ORG))//等待找到原点传感器
                    {
                        Thread.Sleep(10);
                    }
                    i = 0;
                    while (i < 10)//通过原点传感器
                    {
                        if ((axisNumber == 0 && _axis00ORG) || (axisNumber == 1 && _axis01ORG)
                            || (axisNumber == 2 && _axis02ORG) || (axisNumber == 3 && _axis03ORG)
                            || (axisNumber == 4 && _axis04ORG) || (axisNumber == 5 && _axis05ORG)
                            || (axisNumber == 6 && _axis06ORG) || (axisNumber == 7 && _axis07ORG)
                            || (axisNumber == 8 && _axis08ORG) || (axisNumber == 9 && _axis09ORG)
                            || (axisNumber == 10 && _axis10ORG) || (axisNumber == 11 && _axis11ORG))
                            i++;
                        Thread.Sleep(10);
                    }
                    i = 0;
                    while (i < 10)//离开原点传感器
                    {
                        if ((axisNumber == 0 && !_axis00ORG) || (axisNumber == 1 && !_axis01ORG)
                            || (axisNumber == 2 && !_axis02ORG) || (axisNumber == 3 && !_axis03ORG)
                            || (axisNumber == 4 && !_axis04ORG) || (axisNumber == 5 && !_axis05ORG)
                            || (axisNumber == 6 && !_axis06ORG) || (axisNumber == 7 && !_axis07ORG)
                            || (axisNumber == 8 && !_axis08ORG) || (axisNumber == 9 && !_axis09ORG)
                            || (axisNumber == 10 && !_axis10ORG) || (axisNumber == 11 && !_axis11ORG))
                            i++;
                        else
                            i = 0;
                        Thread.Sleep(10);
                    }
                    while ((axisNumber == 0 && _axis00ORG) || (axisNumber == 1 && _axis01ORG) || (axisNumber == 2 && _axis02ORG) || (axisNumber == 3 && _axis03ORG)
                           || (axisNumber == 4 && _axis04ORG) || (axisNumber == 5 && _axis05ORG) || (axisNumber == 6 && _axis06ORG) || (axisNumber == 7 && _axis07ORG)
                           || (axisNumber == 8 && _axis08ORG) || (axisNumber == 9 && _axis09ORG) || (axisNumber == 10 && _axis10ORG) || (axisNumber == 11 && _axis11ORG))
                    {
                        Thread.Sleep(10);
                    }
                    axisDecelStop(axisNumber);//停止移动
                    _axisCheckDoneStatus = 0;
                    while (_axisCheckDoneStatus == 0)//等待停止完成
                    {
                        Thread.Sleep(10);
                        NMCLib20.NMC_MtGetSts(axishandle[axisNumber], ref axsists);
                        _axisCheckDoneStatus = (axsists & (1 << 0)) == 0 ? 1 : 0;//取bit0,0静止,1运动
                    }
                }
                //如果找到的是原点传感器
                if (axis_ORG)
                {
                    //向正方向Jog移动
                    AxisAction(901, axisNumber, startVelocity, axisHomeFindSensorVelocity, 0, accTime, 0, 0, true, false, 0, false, 0);
                    i = 0;
                    while (i < 10)//离开原点传感器
                    {
                        if ((axisNumber == 0 && !_axis00ORG) || (axisNumber == 1 && !_axis01ORG)
                            || (axisNumber == 2 && !_axis02ORG) || (axisNumber == 3 && !_axis03ORG)
                            || (axisNumber == 4 && !_axis04ORG) || (axisNumber == 5 && !_axis05ORG)
                            || (axisNumber == 6 && !_axis06ORG) || (axisNumber == 7 && !_axis07ORG)
                            || (axisNumber == 8 && !_axis08ORG) || (axisNumber == 9 && !_axis09ORG)
                            || (axisNumber == 10 && !_axis10ORG) || (axisNumber == 11 && !_axis11ORG))
                            i++;
                        else
                            i = 0;
                        Thread.Sleep(10);
                    }
                    while ((axisNumber == 0 && _axis00ORG) || (axisNumber == 1 && _axis01ORG) || (axisNumber == 2 && _axis02ORG) || (axisNumber == 3 && _axis03ORG)
                           || (axisNumber == 4 && _axis04ORG) || (axisNumber == 5 && _axis05ORG) || (axisNumber == 6 && _axis06ORG) || (axisNumber == 7 && _axis07ORG)
                           || (axisNumber == 8 && _axis08ORG) || (axisNumber == 9 && _axis09ORG) || (axisNumber == 10 && _axis10ORG) || (axisNumber == 11 && _axis11ORG))
                    {
                        Thread.Sleep(10);
                    }
                    axisDecelStop(axisNumber);//停止移动
                    _axisCheckDoneStatus = 0;
                    while (_axisCheckDoneStatus == 0)//等待停止完成
                    {
                        Thread.Sleep(10);
                        NMCLib20.NMC_MtGetSts(axishandle[axisNumber], ref axsists);
                        _axisCheckDoneStatus = (axsists & (1 << 0)) == 0 ? 1 : 0;//取bit0,0静止,1运动
                    }
                }
                //向正方向Inch移动固定距离
                AxisAction(901, axisNumber, startVelocity, axisHomeFindSensorVelocity, 0, accTime, 0, axisHomeBackDistance, true, true, 0, false, 0);
                _axisCheckDoneStatus = 0;
                while (_axisCheckDoneStatus == 0)//等待停止完成
                {
                    Thread.Sleep(10);
                    NMCLib20.NMC_MtGetSts(axishandle[axisNumber], ref axsists);
                    _axisCheckDoneStatus = (axsists & (1 << 0)) == 0 ? 1 : 0;//取bit0,0静止,1运动
                }

                //回原点
                switch (axisNumber)
                {
                    case 0:
                        _axis00Command = 999;
                        _axis00HomedFlag = false;
                        axis00MaxVelocity_Temp_pps = Convert.ToInt32(maxVelocity / _axis00mmPP);
                        if (axis00MaxVelocity_Temp_pps < 0)
                            axis00MaxVelocity_Temp_pps = axis00MaxVelocity_Temp_pps * -1;
                        homepara.mode = 0;//模式:0.原点开关触发;1.限位开关触发;2.(单Z相)编码器的Index信号触发;3.(原点+Z相)原点触发后,正向寻找到Index信号触发;4.(原点-Z相)原点触发后,反向寻找到Index信号触发;5.(原点-Z相)限位触发后,反向寻找到Index信号触发
                        homepara.dir = 0;//搜寻零点方向（必须）0:负向;1:正向,其它值无意义
                        homepara.offset = 0;//原点偏移（必须）
                        homepara.scan1stVel = (double)axis00MaxVelocity_Temp_pps / 1000;//基本搜寻速度（必须）
                        homepara.scan2ndVel = (double)axis00MaxVelocity_Temp_pps / 1000;//低速（两次搜寻时需要）
                        homepara.acc = ((double)axis00MaxVelocity_Temp_pps / 1000) / ((double)axis00TargetAccTime_s * 1000);//加速度
                        if (homepara.acc < 0.1)
                            homepara.acc = 0.1;
                        homepara.reScanEn = 0;//是否两次搜寻零点（可选,不用时设为0）
                        homepara.homeEdge = 0;//原点触发沿（默认0下降沿触发）,与原点触发电平有关
                        homepara.lmtEdge = 0;//限位触发沿（默认0下降沿触发）,与限位触发电平有关
                        homepara.zEdge = 0;//限位触发沿（默认0下降沿触发）,与Z向触发电平有关
                        homepara.iniRetPos = 0;//起始时反向离开当前位置的运动距离（可选,不用时设为0）
                        homepara.retSwOffset = 0;//反向运动时离开开关距离（可选,不用时设为0）
                        homepara.safeLen = 0;//安全距离,回零时最远搜寻距离（可选,0表示不限制距离）
                        homepara.usePreSetPtpPara = 0;//1表示需要在回零前,自己设置回零运动（点到点）的参数.为0时,回零运动的减加速度默认等于acc,起跳速度、终点速度、平滑系数默认为0
                        homepara.reserved0 = 0;//保留
                        homepara.reserved1 = 0;//保留
                        homepara.reserved2 = 0;//保留
                        //设置回零参数:轴句柄;回零参数结构
                        NMCLib20.NMC_MtSetHomePara(axishandle[0], ref homepara);
                        //启动回零
                        NMCLib20.NMC_MtHome(axishandle[0]);
                        _axis00HomeStartFlag = true;
                        break;
                    case 1:
                        _axis01Command = 999;
                        _axis01HomedFlag = false;
                        axis01MaxVelocity_Temp_pps = Convert.ToInt32(maxVelocity / _axis01mmPP);
                        if (axis01MaxVelocity_Temp_pps < 0)
                            axis01MaxVelocity_Temp_pps = axis01MaxVelocity_Temp_pps * -1;
                        homepara.mode = 0;//模式:0.原点开关触发;1.限位开关触发;2.(单Z相)编码器的Index信号触发;3.(原点+Z相)原点触发后,正向寻找到Index信号触发;4.(原点-Z相)原点触发后,反向寻找到Index信号触发;5.(原点-Z相)限位触发后,反向寻找到Index信号触发
                        homepara.dir = 0;//搜寻零点方向（必须）0:负向;1:正向,其它值无意义
                        homepara.offset = 0;//原点偏移（必须）
                        homepara.scan1stVel = (double)axis01MaxVelocity_Temp_pps / 1000;//基本搜寻速度（必须）
                        homepara.scan2ndVel = (double)axis01MaxVelocity_Temp_pps / 1000;//低速（两次搜寻时需要）
                        homepara.acc = ((double)axis01MaxVelocity_Temp_pps / 1000) / ((double)axis01TargetAccTime_s * 1000);//加速度
                        if (homepara.acc < 0.1)
                            homepara.acc = 0.1;
                        homepara.reScanEn = 0;//是否两次搜寻零点（可选,不用时设为0）
                        homepara.homeEdge = 0;//原点触发沿（默认0下降沿触发）,与原点触发电平有关
                        homepara.lmtEdge = 0;//限位触发沿（默认0下降沿触发）,与限位触发电平有关
                        homepara.zEdge = 0;//限位触发沿（默认0下降沿触发）,与Z向触发电平有关
                        homepara.iniRetPos = 0;//起始时反向离开当前位置的运动距离（可选,不用时设为0）
                        homepara.retSwOffset = 0;//反向运动时离开开关距离（可选,不用时设为0）
                        homepara.safeLen = 0;//安全距离,回零时最远搜寻距离（可选,0表示不限制距离）
                        homepara.usePreSetPtpPara = 0;//1表示需要在回零前,自己设置回零运动（点到点）的参数.为0时,回零运动的减加速度默认等于acc,起跳速度、终点速度、平滑系数默认为0
                        homepara.reserved0 = 0;//保留
                        homepara.reserved1 = 0;//保留
                        homepara.reserved2 = 0;//保留
                        //设置回零参数:轴句柄;回零参数结构
                        NMCLib20.NMC_MtSetHomePara(axishandle[1], ref homepara);
                        //启动回零
                        NMCLib20.NMC_MtHome(axishandle[1]);
                        _axis01HomeStartFlag = true;
                        break;
                    case 2:
                        _axis02Command = 999;
                        _axis02HomedFlag = false;
                        axis02MaxVelocity_Temp_pps = Convert.ToInt32(maxVelocity / _axis02mmPP);
                        if (axis02MaxVelocity_Temp_pps < 0)
                            axis02MaxVelocity_Temp_pps = axis02MaxVelocity_Temp_pps * -1;
                        homepara.mode = 0;//模式:0.原点开关触发;1.限位开关触发;2.(单Z相)编码器的Index信号触发;3.(原点+Z相)原点触发后,正向寻找到Index信号触发;4.(原点-Z相)原点触发后,反向寻找到Index信号触发;5.(原点-Z相)限位触发后,反向寻找到Index信号触发
                        homepara.dir = 0;//搜寻零点方向（必须）0:负向;1:正向,其它值无意义
                        homepara.offset = 0;//原点偏移（必须）
                        homepara.scan1stVel = (double)axis02MaxVelocity_Temp_pps / 1000;//基本搜寻速度（必须）
                        homepara.scan2ndVel = (double)axis02MaxVelocity_Temp_pps / 1000;//低速（两次搜寻时需要）
                        homepara.acc = ((double)axis02MaxVelocity_Temp_pps / 1000) / ((double)axis02TargetAccTime_s * 1000);//加速度
                        if (homepara.acc < 0.1)
                            homepara.acc = 0.1;
                        homepara.reScanEn = 0;//是否两次搜寻零点（可选,不用时设为0）
                        homepara.homeEdge = 0;//原点触发沿（默认0下降沿触发）,与原点触发电平有关
                        homepara.lmtEdge = 0;//限位触发沿（默认0下降沿触发）,与限位触发电平有关
                        homepara.zEdge = 0;//限位触发沿（默认0下降沿触发）,与Z向触发电平有关
                        homepara.iniRetPos = 0;//起始时反向离开当前位置的运动距离（可选,不用时设为0）
                        homepara.retSwOffset = 0;//反向运动时离开开关距离（可选,不用时设为0）
                        homepara.safeLen = 0;//安全距离,回零时最远搜寻距离（可选,0表示不限制距离）
                        homepara.usePreSetPtpPara = 0;//1表示需要在回零前,自己设置回零运动（点到点）的参数.为0时,回零运动的减加速度默认等于acc,起跳速度、终点速度、平滑系数默认为0
                        homepara.reserved0 = 0;//保留
                        homepara.reserved1 = 0;//保留
                        homepara.reserved2 = 0;//保留
                        //设置回零参数:轴句柄;回零参数结构
                        NMCLib20.NMC_MtSetHomePara(axishandle[2], ref homepara);
                        //启动回零
                        NMCLib20.NMC_MtHome(axishandle[2]);
                        _axis02HomeStartFlag = true;
                        break;
                    case 3:
                        _axis03Command = 999;
                        _axis03HomedFlag = false;
                        axis03MaxVelocity_Temp_pps = Convert.ToInt32(maxVelocity / _axis03mmPP);
                        if (axis03MaxVelocity_Temp_pps < 0)
                            axis03MaxVelocity_Temp_pps = axis03MaxVelocity_Temp_pps * -1;
                        homepara.mode = 0;//模式:0.原点开关触发;1.限位开关触发;2.(单Z相)编码器的Index信号触发;3.(原点+Z相)原点触发后,正向寻找到Index信号触发;4.(原点-Z相)原点触发后,反向寻找到Index信号触发;5.(原点-Z相)限位触发后,反向寻找到Index信号触发
                        homepara.dir = 0;//搜寻零点方向（必须）0:负向;1:正向,其它值无意义
                        homepara.offset = 0;//原点偏移（必须）
                        homepara.scan1stVel = (double)axis03MaxVelocity_Temp_pps / 1000;//基本搜寻速度（必须）
                        homepara.scan2ndVel = (double)axis03MaxVelocity_Temp_pps / 1000;//低速（两次搜寻时需要）
                        homepara.acc = ((double)axis03MaxVelocity_Temp_pps / 1000) / ((double)axis03TargetAccTime_s * 1000);//加速度
                        if (homepara.acc < 0.1)
                            homepara.acc = 0.1;
                        homepara.reScanEn = 0;//是否两次搜寻零点（可选,不用时设为0）
                        homepara.homeEdge = 0;//原点触发沿（默认0下降沿触发）,与原点触发电平有关
                        homepara.lmtEdge = 0;//限位触发沿（默认0下降沿触发）,与限位触发电平有关
                        homepara.zEdge = 0;//限位触发沿（默认0下降沿触发）,与Z向触发电平有关
                        homepara.iniRetPos = 0;//起始时反向离开当前位置的运动距离（可选,不用时设为0）
                        homepara.retSwOffset = 0;//反向运动时离开开关距离（可选,不用时设为0）
                        homepara.safeLen = 0;//安全距离,回零时最远搜寻距离（可选,0表示不限制距离）
                        homepara.usePreSetPtpPara = 0;//1表示需要在回零前,自己设置回零运动（点到点）的参数.为0时,回零运动的减加速度默认等于acc,起跳速度、终点速度、平滑系数默认为0
                        homepara.reserved0 = 0;//保留
                        homepara.reserved1 = 0;//保留
                        homepara.reserved2 = 0;//保留
                        //设置回零参数:轴句柄;回零参数结构
                        NMCLib20.NMC_MtSetHomePara(axishandle[3], ref homepara);
                        //启动回零
                        NMCLib20.NMC_MtHome(axishandle[3]);
                        _axis03HomeStartFlag = true;
                        break;
                    case 4:
                        _axis04Command = 999;
                        _axis04HomedFlag = false;
                        axis04MaxVelocity_Temp_pps = Convert.ToInt32(maxVelocity / _axis04mmPP);
                        if (axis04MaxVelocity_Temp_pps < 0)
                            axis04MaxVelocity_Temp_pps = axis04MaxVelocity_Temp_pps * -1;
                        homepara.mode = 0;//模式:0.原点开关触发;1.限位开关触发;2.(单Z相)编码器的Index信号触发;3.(原点+Z相)原点触发后,正向寻找到Index信号触发;4.(原点-Z相)原点触发后,反向寻找到Index信号触发;5.(原点-Z相)限位触发后,反向寻找到Index信号触发
                        homepara.dir = 0;//搜寻零点方向（必须）0:负向;1:正向,其它值无意义
                        homepara.offset = 0;//原点偏移（必须）
                        homepara.scan1stVel = (double)axis04MaxVelocity_Temp_pps / 1000;//基本搜寻速度（必须）
                        homepara.scan2ndVel = (double)axis04MaxVelocity_Temp_pps / 1000;//低速（两次搜寻时需要）
                        homepara.acc = ((double)axis04MaxVelocity_Temp_pps / 1000) / ((double)axis04TargetAccTime_s * 1000);//加速度
                        if (homepara.acc < 0.1)
                            homepara.acc = 0.1;
                        homepara.reScanEn = 0;//是否两次搜寻零点（可选,不用时设为0）
                        homepara.homeEdge = 0;//原点触发沿（默认0下降沿触发）,与原点触发电平有关
                        homepara.lmtEdge = 0;//限位触发沿（默认0下降沿触发）,与限位触发电平有关
                        homepara.zEdge = 0;//限位触发沿（默认0下降沿触发）,与Z向触发电平有关
                        homepara.iniRetPos = 0;//起始时反向离开当前位置的运动距离（可选,不用时设为0）
                        homepara.retSwOffset = 0;//反向运动时离开开关距离（可选,不用时设为0）
                        homepara.safeLen = 0;//安全距离,回零时最远搜寻距离（可选,0表示不限制距离）
                        homepara.usePreSetPtpPara = 0;//1表示需要在回零前,自己设置回零运动（点到点）的参数.为0时,回零运动的减加速度默认等于acc,起跳速度、终点速度、平滑系数默认为0
                        homepara.reserved0 = 0;//保留
                        homepara.reserved1 = 0;//保留
                        homepara.reserved2 = 0;//保留
                        //设置回零参数:轴句柄;回零参数结构
                        NMCLib20.NMC_MtSetHomePara(axishandle[4], ref homepara);
                        //启动回零
                        NMCLib20.NMC_MtHome(axishandle[4]);
                        _axis04HomeStartFlag = true;
                        break;
                    case 5:
                        _axis05Command = 999;
                        _axis05HomedFlag = false;
                        axis05MaxVelocity_Temp_pps = Convert.ToInt32(maxVelocity / _axis05mmPP);
                        if (axis05MaxVelocity_Temp_pps < 0)
                            axis05MaxVelocity_Temp_pps = axis05MaxVelocity_Temp_pps * -1;
                        homepara.mode = 0;//模式:0.原点开关触发;1.限位开关触发;2.(单Z相)编码器的Index信号触发;3.(原点+Z相)原点触发后,正向寻找到Index信号触发;4.(原点-Z相)原点触发后,反向寻找到Index信号触发;5.(原点-Z相)限位触发后,反向寻找到Index信号触发
                        homepara.dir = 0;//搜寻零点方向（必须）0:负向;1:正向,其它值无意义
                        homepara.offset = 0;//原点偏移（必须）
                        homepara.scan1stVel = (double)axis05MaxVelocity_Temp_pps / 1000;//基本搜寻速度（必须）
                        homepara.scan2ndVel = (double)axis05MaxVelocity_Temp_pps / 1000;//低速（两次搜寻时需要）
                        homepara.acc = ((double)axis05MaxVelocity_Temp_pps / 1000) / ((double)axis05TargetAccTime_s * 1000);//加速度
                        if (homepara.acc < 0.1)
                            homepara.acc = 0.1;
                        homepara.reScanEn = 0;//是否两次搜寻零点（可选,不用时设为0）
                        homepara.homeEdge = 0;//原点触发沿（默认0下降沿触发）,与原点触发电平有关
                        homepara.lmtEdge = 0;//限位触发沿（默认0下降沿触发）,与限位触发电平有关
                        homepara.zEdge = 0;//限位触发沿（默认0下降沿触发）,与Z向触发电平有关
                        homepara.iniRetPos = 0;//起始时反向离开当前位置的运动距离（可选,不用时设为0）
                        homepara.retSwOffset = 0;//反向运动时离开开关距离（可选,不用时设为0）
                        homepara.safeLen = 0;//安全距离,回零时最远搜寻距离（可选,0表示不限制距离）
                        homepara.usePreSetPtpPara = 0;//1表示需要在回零前,自己设置回零运动（点到点）的参数.为0时,回零运动的减加速度默认等于acc,起跳速度、终点速度、平滑系数默认为0
                        homepara.reserved0 = 0;//保留
                        homepara.reserved1 = 0;//保留
                        homepara.reserved2 = 0;//保留
                        //设置回零参数:轴句柄;回零参数结构
                        NMCLib20.NMC_MtSetHomePara(axishandle[5], ref homepara);
                        //启动回零
                        NMCLib20.NMC_MtHome(axishandle[5]);
                        _axis05HomeStartFlag = true;
                        break;
                    case 6:
                        _axis06Command = 999;
                        _axis06HomedFlag = false;
                        axis06MaxVelocity_Temp_pps = Convert.ToInt32(maxVelocity / _axis06mmPP);
                        if (axis06MaxVelocity_Temp_pps < 0)
                            axis06MaxVelocity_Temp_pps = axis06MaxVelocity_Temp_pps * -1;
                        homepara.mode = 0;//模式:0.原点开关触发;1.限位开关触发;2.(单Z相)编码器的Index信号触发;3.(原点+Z相)原点触发后,正向寻找到Index信号触发;4.(原点-Z相)原点触发后,反向寻找到Index信号触发;5.(原点-Z相)限位触发后,反向寻找到Index信号触发
                        homepara.dir = 0;//搜寻零点方向（必须）0:负向;1:正向,其它值无意义
                        homepara.offset = 0;//原点偏移（必须）
                        homepara.scan1stVel = (double)axis06MaxVelocity_Temp_pps / 1000;//基本搜寻速度（必须）
                        homepara.scan2ndVel = (double)axis06MaxVelocity_Temp_pps / 1000;//低速（两次搜寻时需要）
                        homepara.acc = ((double)axis06MaxVelocity_Temp_pps / 1000) / ((double)axis06TargetAccTime_s * 1000);//加速度
                        if (homepara.acc < 0.1)
                            homepara.acc = 0.1;
                        homepara.reScanEn = 0;//是否两次搜寻零点（可选,不用时设为0）
                        homepara.homeEdge = 0;//原点触发沿（默认0下降沿触发）,与原点触发电平有关
                        homepara.lmtEdge = 0;//限位触发沿（默认0下降沿触发）,与限位触发电平有关
                        homepara.zEdge = 0;//限位触发沿（默认0下降沿触发）,与Z向触发电平有关
                        homepara.iniRetPos = 0;//起始时反向离开当前位置的运动距离（可选,不用时设为0）
                        homepara.retSwOffset = 0;//反向运动时离开开关距离（可选,不用时设为0）
                        homepara.safeLen = 0;//安全距离,回零时最远搜寻距离（可选,0表示不限制距离）
                        homepara.usePreSetPtpPara = 0;//1表示需要在回零前,自己设置回零运动（点到点）的参数.为0时,回零运动的减加速度默认等于acc,起跳速度、终点速度、平滑系数默认为0
                        homepara.reserved0 = 0;//保留
                        homepara.reserved1 = 0;//保留
                        homepara.reserved2 = 0;//保留
                        //设置回零参数:轴句柄;回零参数结构
                        NMCLib20.NMC_MtSetHomePara(axishandle[6], ref homepara);
                        //启动回零
                        NMCLib20.NMC_MtHome(axishandle[6]);
                        _axis06HomeStartFlag = true;
                        break;
                    case 7:
                        _axis07Command = 999;
                        _axis07HomedFlag = false;
                        axis07MaxVelocity_Temp_pps = Convert.ToInt32(maxVelocity / _axis07mmPP);
                        if (axis07MaxVelocity_Temp_pps < 0)
                            axis07MaxVelocity_Temp_pps = axis07MaxVelocity_Temp_pps * -1;
                        homepara.mode = 0;//模式:0.原点开关触发;1.限位开关触发;2.(单Z相)编码器的Index信号触发;3.(原点+Z相)原点触发后,正向寻找到Index信号触发;4.(原点-Z相)原点触发后,反向寻找到Index信号触发;5.(原点-Z相)限位触发后,反向寻找到Index信号触发
                        homepara.dir = 0;//搜寻零点方向（必须）0:负向;1:正向,其它值无意义
                        homepara.offset = 0;//原点偏移（必须）
                        homepara.scan1stVel = (double)axis07MaxVelocity_Temp_pps / 1000;//基本搜寻速度（必须）
                        homepara.scan2ndVel = (double)axis07MaxVelocity_Temp_pps / 1000;//低速（两次搜寻时需要）
                        homepara.acc = ((double)axis07MaxVelocity_Temp_pps / 1000) / ((double)axis07TargetAccTime_s * 1000);//加速度
                        if (homepara.acc < 0.1)
                            homepara.acc = 0.1;
                        homepara.reScanEn = 0;//是否两次搜寻零点（可选,不用时设为0）
                        homepara.homeEdge = 0;//原点触发沿（默认0下降沿触发）,与原点触发电平有关
                        homepara.lmtEdge = 0;//限位触发沿（默认0下降沿触发）,与限位触发电平有关
                        homepara.zEdge = 0;//限位触发沿（默认0下降沿触发）,与Z向触发电平有关
                        homepara.iniRetPos = 0;//起始时反向离开当前位置的运动距离（可选,不用时设为0）
                        homepara.retSwOffset = 0;//反向运动时离开开关距离（可选,不用时设为0）
                        homepara.safeLen = 0;//安全距离,回零时最远搜寻距离（可选,0表示不限制距离）
                        homepara.usePreSetPtpPara = 0;//1表示需要在回零前,自己设置回零运动（点到点）的参数.为0时,回零运动的减加速度默认等于acc,起跳速度、终点速度、平滑系数默认为0
                        homepara.reserved0 = 0;//保留
                        homepara.reserved1 = 0;//保留
                        homepara.reserved2 = 0;//保留
                        //设置回零参数:轴句柄;回零参数结构
                        NMCLib20.NMC_MtSetHomePara(axishandle[7], ref homepara);
                        //启动回零
                        NMCLib20.NMC_MtHome(axishandle[7]);
                        _axis07HomeStartFlag = true;
                        break;
                    case 8:
                        _axis08Command = 999;
                        _axis08HomedFlag = false;
                        axis08MaxVelocity_Temp_pps = Convert.ToInt32(maxVelocity / _axis08mmPP);
                        if (axis08MaxVelocity_Temp_pps < 0)
                            axis08MaxVelocity_Temp_pps = axis08MaxVelocity_Temp_pps * -1;
                        homepara.mode = 0;//模式:0.原点开关触发;1.限位开关触发;2.(单Z相)编码器的Index信号触发;3.(原点+Z相)原点触发后,正向寻找到Index信号触发;4.(原点-Z相)原点触发后,反向寻找到Index信号触发;5.(原点-Z相)限位触发后,反向寻找到Index信号触发
                        homepara.dir = 0;//搜寻零点方向（必须）0:负向;1:正向,其它值无意义
                        homepara.offset = 0;//原点偏移（必须）
                        homepara.scan1stVel = (double)axis08MaxVelocity_Temp_pps / 1000;//基本搜寻速度（必须）
                        homepara.scan2ndVel = (double)axis08MaxVelocity_Temp_pps / 1000;//低速（两次搜寻时需要）
                        homepara.acc = ((double)axis08MaxVelocity_Temp_pps / 1000) / ((double)axis08TargetAccTime_s * 1000);//加速度
                        if (homepara.acc < 0.1)
                            homepara.acc = 0.1;
                        homepara.reScanEn = 0;//是否两次搜寻零点（可选,不用时设为0）
                        homepara.homeEdge = 0;//原点触发沿（默认0下降沿触发）,与原点触发电平有关
                        homepara.lmtEdge = 0;//限位触发沿（默认0下降沿触发）,与限位触发电平有关
                        homepara.zEdge = 0;//限位触发沿（默认0下降沿触发）,与Z向触发电平有关
                        homepara.iniRetPos = 0;//起始时反向离开当前位置的运动距离（可选,不用时设为0）
                        homepara.retSwOffset = 0;//反向运动时离开开关距离（可选,不用时设为0）
                        homepara.safeLen = 0;//安全距离,回零时最远搜寻距离（可选,0表示不限制距离）
                        homepara.usePreSetPtpPara = 0;//1表示需要在回零前,自己设置回零运动（点到点）的参数.为0时,回零运动的减加速度默认等于acc,起跳速度、终点速度、平滑系数默认为0
                        homepara.reserved0 = 0;//保留
                        homepara.reserved1 = 0;//保留
                        homepara.reserved2 = 0;//保留
                        //设置回零参数:轴句柄;回零参数结构
                        NMCLib20.NMC_MtSetHomePara(axishandle[8], ref homepara);
                        //启动回零
                        NMCLib20.NMC_MtHome(axishandle[8]);
                        _axis08HomeStartFlag = true;
                        break;
                    case 9:
                        _axis09Command = 999;
                        _axis09HomedFlag = false;
                        axis09MaxVelocity_Temp_pps = Convert.ToInt32(maxVelocity / _axis09mmPP);
                        if (axis09MaxVelocity_Temp_pps < 0)
                            axis09MaxVelocity_Temp_pps = axis09MaxVelocity_Temp_pps * -1;
                        homepara.mode = 0;//模式:0.原点开关触发;1.限位开关触发;2.(单Z相)编码器的Index信号触发;3.(原点+Z相)原点触发后,正向寻找到Index信号触发;4.(原点-Z相)原点触发后,反向寻找到Index信号触发;5.(原点-Z相)限位触发后,反向寻找到Index信号触发
                        homepara.dir = 0;//搜寻零点方向（必须）0:负向;1:正向,其它值无意义
                        homepara.offset = 0;//原点偏移（必须）
                        homepara.scan1stVel = (double)axis09MaxVelocity_Temp_pps / 1000;//基本搜寻速度（必须）
                        homepara.scan2ndVel = (double)axis09MaxVelocity_Temp_pps / 1000;//低速（两次搜寻时需要）
                        homepara.acc = ((double)axis09MaxVelocity_Temp_pps / 1000) / ((double)axis09TargetAccTime_s * 1000);//加速度
                        if (homepara.acc < 0.1)
                            homepara.acc = 0.1;
                        homepara.reScanEn = 0;//是否两次搜寻零点（可选,不用时设为0）
                        homepara.homeEdge = 0;//原点触发沿（默认0下降沿触发）,与原点触发电平有关
                        homepara.lmtEdge = 0;//限位触发沿（默认0下降沿触发）,与限位触发电平有关
                        homepara.zEdge = 0;//限位触发沿（默认0下降沿触发）,与Z向触发电平有关
                        homepara.iniRetPos = 0;//起始时反向离开当前位置的运动距离（可选,不用时设为0）
                        homepara.retSwOffset = 0;//反向运动时离开开关距离（可选,不用时设为0）
                        homepara.safeLen = 0;//安全距离,回零时最远搜寻距离（可选,0表示不限制距离）
                        homepara.usePreSetPtpPara = 0;//1表示需要在回零前,自己设置回零运动（点到点）的参数.为0时,回零运动的减加速度默认等于acc,起跳速度、终点速度、平滑系数默认为0
                        homepara.reserved0 = 0;//保留
                        homepara.reserved1 = 0;//保留
                        homepara.reserved2 = 0;//保留
                        //设置回零参数:轴句柄;回零参数结构
                        NMCLib20.NMC_MtSetHomePara(axishandle[9], ref homepara);
                        //启动回零
                        NMCLib20.NMC_MtHome(axishandle[9]);
                        _axis09HomeStartFlag = true;
                        break;
                    case 10:
                        _axis10Command = 999;
                        _axis10HomedFlag = false;
                        axis10MaxVelocity_Temp_pps = Convert.ToInt32(maxVelocity / _axis10mmPP);
                        if (axis10MaxVelocity_Temp_pps < 0)
                            axis10MaxVelocity_Temp_pps = axis10MaxVelocity_Temp_pps * -1;
                        homepara.mode = 0;//模式:0.原点开关触发;1.限位开关触发;2.(单Z相)编码器的Index信号触发;3.(原点+Z相)原点触发后,正向寻找到Index信号触发;4.(原点-Z相)原点触发后,反向寻找到Index信号触发;5.(原点-Z相)限位触发后,反向寻找到Index信号触发
                        homepara.dir = 0;//搜寻零点方向（必须）0:负向;1:正向,其它值无意义
                        homepara.offset = 0;//原点偏移（必须）
                        homepara.scan1stVel = (double)axis10MaxVelocity_Temp_pps / 1000;//基本搜寻速度（必须）
                        homepara.scan2ndVel = (double)axis10MaxVelocity_Temp_pps / 1000;//低速（两次搜寻时需要）
                        homepara.acc = ((double)axis10MaxVelocity_Temp_pps / 1000) / ((double)axis10TargetAccTime_s * 1000);//加速度
                        if (homepara.acc < 0.1)
                            homepara.acc = 0.1;
                        homepara.reScanEn = 0;//是否两次搜寻零点（可选,不用时设为0）
                        homepara.homeEdge = 0;//原点触发沿（默认0下降沿触发）,与原点触发电平有关
                        homepara.lmtEdge = 0;//限位触发沿（默认0下降沿触发）,与限位触发电平有关
                        homepara.zEdge = 0;//限位触发沿（默认0下降沿触发）,与Z向触发电平有关
                        homepara.iniRetPos = 0;//起始时反向离开当前位置的运动距离（可选,不用时设为0）
                        homepara.retSwOffset = 0;//反向运动时离开开关距离（可选,不用时设为0）
                        homepara.safeLen = 0;//安全距离,回零时最远搜寻距离（可选,0表示不限制距离）
                        homepara.usePreSetPtpPara = 0;//1表示需要在回零前,自己设置回零运动（点到点）的参数.为0时,回零运动的减加速度默认等于acc,起跳速度、终点速度、平滑系数默认为0
                        homepara.reserved0 = 0;//保留
                        homepara.reserved1 = 0;//保留
                        homepara.reserved2 = 0;//保留
                        //设置回零参数:轴句柄;回零参数结构
                        NMCLib20.NMC_MtSetHomePara(axishandle[10], ref homepara);
                        //启动回零
                        NMCLib20.NMC_MtHome(axishandle[10]);
                        _axis10HomeStartFlag = true;
                        break;
                    case 11:
                        _axis11Command = 999;
                        _axis11HomedFlag = false;
                        axis11MaxVelocity_Temp_pps = Convert.ToInt32(maxVelocity / _axis11mmPP);
                        if (axis11MaxVelocity_Temp_pps < 0)
                            axis11MaxVelocity_Temp_pps = axis11MaxVelocity_Temp_pps * -1;
                        homepara.mode = 0;//模式:0.原点开关触发;1.限位开关触发;2.(单Z相)编码器的Index信号触发;3.(原点+Z相)原点触发后,正向寻找到Index信号触发;4.(原点-Z相)原点触发后,反向寻找到Index信号触发;5.(原点-Z相)限位触发后,反向寻找到Index信号触发
                        homepara.dir = 0;//搜寻零点方向（必须）0:负向;1:正向,其它值无意义
                        homepara.offset = 0;//原点偏移（必须）
                        homepara.scan1stVel = (double)axis11MaxVelocity_Temp_pps / 1000;//基本搜寻速度（必须）
                        homepara.scan2ndVel = (double)axis11MaxVelocity_Temp_pps / 1000;//低速（两次搜寻时需要）
                        homepara.acc = ((double)axis11MaxVelocity_Temp_pps / 1000) / ((double)axis11TargetAccTime_s * 1000);//加速度
                        if (homepara.acc < 0.1)
                            homepara.acc = 0.1;
                        homepara.reScanEn = 0;//是否两次搜寻零点（可选,不用时设为0）
                        homepara.homeEdge = 0;//原点触发沿（默认0下降沿触发）,与原点触发电平有关
                        homepara.lmtEdge = 0;//限位触发沿（默认0下降沿触发）,与限位触发电平有关
                        homepara.zEdge = 0;//限位触发沿（默认0下降沿触发）,与Z向触发电平有关
                        homepara.iniRetPos = 0;//起始时反向离开当前位置的运动距离（可选,不用时设为0）
                        homepara.retSwOffset = 0;//反向运动时离开开关距离（可选,不用时设为0）
                        homepara.safeLen = 0;//安全距离,回零时最远搜寻距离（可选,0表示不限制距离）
                        homepara.usePreSetPtpPara = 0;//1表示需要在回零前,自己设置回零运动（点到点）的参数.为0时,回零运动的减加速度默认等于acc,起跳速度、终点速度、平滑系数默认为0
                        homepara.reserved0 = 0;//保留
                        homepara.reserved1 = 0;//保留
                        homepara.reserved2 = 0;//保留
                        //设置回零参数:轴句柄;回零参数结构
                        NMCLib20.NMC_MtSetHomePara(axishandle[11], ref homepara);
                        //启动回零
                        NMCLib20.NMC_MtHome(axishandle[11]);
                        _axis11HomeStartFlag = true;
                        break;
                    default:
                        break;
                }

            });

        }
        #endregion

        #region OutputCtrl:输出点ON/OFF(0-63为本地,64-319为扩展.第一个运动控制器为0-319,第二个为320-639.)
        /// <summary>
        /// outPut:输出点ON/OFF
        /// </summary>
        /// <param name="outPutNo">int:输出点号码(0-N)(0-63为本地,64-319为扩展.第一个运动控制器为0-319,第二个为320-639.)</param>
        /// <param name="onOrOff">bool:输出点ON/OFF设置</param>
        public int OutputCtrl(int outPutNo, bool onOrOff)
        {
            short bitStatus = (short)(onOrOff ? 0 : 1);
            return NMCLib20.NMC_SetDOBit(Devhandle[outPutNo / _motionCardIOMaxQuantity_320], (short)(outPutNo % _motionCardIOMaxQuantity_320), bitStatus);
        }
        #endregion

        #region getOutPut:读取输出点状态(0-63为本地,64-319为扩展.第一个运动控制器为0-319,第二个为320-639.)
        /// <summary>
        /// outPut:getOutPut:读取输出点状态
        /// </summary>
        /// <param name="outPutNo">int:输出点号码(0-N)</param>
        public bool getOutPut(int outPutNo)
        {
            int bitStatus;
            NMCLib20.NMC_GetDOGroup(Devhandle[outPutNo / _motionCardIOMaxQuantity_320], out bitStatus, (short)(outPutNo / 32));
            if ((bitStatus & (1 << (outPutNo % 32))) == 0 ? true : false)
                return true;
            else
                return false;
        }
        #endregion

        #region getInPut:读取输入点状态(0-63为本地,64-319为扩展.第一个运动控制器为0-319,第二个为320-639.)
        /// <summary>
        /// getInPut:读取输入点状态
        /// </summary>
        /// <param name="inPutNo">int:输入点号码(0-N)</param>
        public bool getInPut(int inPutNo)
        {
            short bitStatus = 0;
            NMCLib20.NMC_GetDIBit(Devhandle[inPutNo / _motionCardIOMaxQuantity_320], (short)(inPutNo % _motionCardIOMaxQuantity_320), ref bitStatus);
            if (bitStatus == 0)
                return true;
            else
                return false;
        }
        #endregion

        #region Close:卡及轴关闭
        /// <summary>
        /// Close:卡及轴关闭
        /// </summary>
        public int Close()
        {
            if (initialFlag)
            {
                if (_cardQuantity >= 1)
                {
                    axisImmediateStop(0);
                    axisImmediateStop(1);
                    axisImmediateStop(2);
                    axisImmediateStop(3);
                    _axis00Command = 0;
                    _axis01Command = 0;
                    _axis02Command = 0;
                    _axis03Command = 0;
                    _axis00HomedFlag = false;
                    _axis01HomedFlag = false;
                    _axis02HomedFlag = false;
                    _axis03HomedFlag = false;
                    _axis00CurrentPositionNumber = 0;
                    _axis01CurrentPositionNumber = 0;
                    _axis02CurrentPositionNumber = 0;
                    _axis03CurrentPositionNumber = 0;
                    axis00MoveTimeOutCount = 0;
                    axis01MoveTimeOutCount = 0;
                    axis02MoveTimeOutCount = 0;
                    axis03MoveTimeOutCount = 0;
                    _axis00ErrorNumber = 0;
                    _axis01ErrorNumber = 0;
                    _axis02ErrorNumber = 0;
                    _axis03ErrorNumber = 0;

                    //ADD 202210151953 Pause
                    _axis00PauseRequestFlag = false;
                    _axis00ResumeRequestFlag = false;
                    _axis00PausedFlag = false;
                    _axis01PauseRequestFlag = false;
                    _axis01ResumeRequestFlag = false;
                    _axis01PausedFlag = false;
                    _axis02PauseRequestFlag = false;
                    _axis02ResumeRequestFlag = false;
                    _axis02PausedFlag = false;
                    _axis03PauseRequestFlag = false;
                    _axis03ResumeRequestFlag = false;
                    _axis03PausedFlag = false;
                }
                if (_cardQuantity >= 2)
                {
                    axisImmediateStop(4);
                    axisImmediateStop(5);
                    axisImmediateStop(6);
                    axisImmediateStop(7);
                    _axis04Command = 0;
                    _axis05Command = 0;
                    _axis06Command = 0;
                    _axis07Command = 0;
                    _axis04HomedFlag = false;
                    _axis05HomedFlag = false;
                    _axis06HomedFlag = false;
                    _axis07HomedFlag = false;
                    _axis04CurrentPositionNumber = 0;
                    _axis05CurrentPositionNumber = 0;
                    _axis06CurrentPositionNumber = 0;
                    _axis07CurrentPositionNumber = 0;
                    axis04MoveTimeOutCount = 0;
                    axis05MoveTimeOutCount = 0;
                    axis06MoveTimeOutCount = 0;
                    axis07MoveTimeOutCount = 0;
                    _axis04ErrorNumber = 0;
                    _axis05ErrorNumber = 0;
                    _axis06ErrorNumber = 0;
                    _axis07ErrorNumber = 0;

                    //ADD 202210151953 Pause
                    _axis04PauseRequestFlag = false;
                    _axis04ResumeRequestFlag = false;
                    _axis04PausedFlag = false;
                    _axis05PauseRequestFlag = false;
                    _axis05ResumeRequestFlag = false;
                    _axis05PausedFlag = false;
                    _axis06PauseRequestFlag = false;
                    _axis06ResumeRequestFlag = false;
                    _axis06PausedFlag = false;
                    _axis07PauseRequestFlag = false;
                    _axis07ResumeRequestFlag = false;
                    _axis07PausedFlag = false;
                }
                if (_cardQuantity >= 3)
                {
                    axisImmediateStop(8);
                    axisImmediateStop(9);
                    axisImmediateStop(10);
                    axisImmediateStop(11);
                    _axis08Command = 0;
                    _axis09Command = 0;
                    _axis10Command = 0;
                    _axis11Command = 0;
                    _axis08HomedFlag = false;
                    _axis09HomedFlag = false;
                    _axis10HomedFlag = false;
                    _axis11HomedFlag = false;
                    _axis08CurrentPositionNumber = 0;
                    _axis09CurrentPositionNumber = 0;
                    _axis10CurrentPositionNumber = 0;
                    _axis11CurrentPositionNumber = 0;
                    axis08MoveTimeOutCount = 0;
                    axis09MoveTimeOutCount = 0;
                    axis10MoveTimeOutCount = 0;
                    axis11MoveTimeOutCount = 0;
                    _axis08ErrorNumber = 0;
                    _axis09ErrorNumber = 0;
                    _axis10ErrorNumber = 0;
                    _axis11ErrorNumber = 0;

                    //ADD 202210151953 Pause
                    _axis08PauseRequestFlag = false;
                    _axis08ResumeRequestFlag = false;
                    _axis08PausedFlag = false;
                    _axis09PauseRequestFlag = false;
                    _axis09ResumeRequestFlag = false;
                    _axis09PausedFlag = false;
                    _axis10PauseRequestFlag = false;
                    _axis10ResumeRequestFlag = false;
                    _axis10PausedFlag = false;
                    _axis11PauseRequestFlag = false;
                    _axis11ResumeRequestFlag = false;
                    _axis11PausedFlag = false;
                }

                //停止轴状态更新线程
                if (td_axisStatusUpdata != null)
                {
                    td_axisStatusUpdata.Abort();
                    td_axisStatusUpdata = null;
                }

                initialFlag = false;

                for (int i = 0; i < axishandle.Length; i++)
                {
                    NMCLib20.NMC_MtClose(ref axishandle[i]);//关闭轴                                   
                }
                for (int i = 0; i < Devhandle.Length; i++)
                {
                    NMCLib20.NMC_DevClose(ref Devhandle[i]);//关闭运动控制器
                }
                return 1;
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

        #region Compare2Dimens:二维高速位置比较
        /// <summary>
        /// Compare2Dimens:二维高速位置比较
        /// </summary>
        /// <param name="_Devhandle">ushort:控制器句柄0-N</param>
        /// <param name="_dir1No">short:方向1的位置源轴号(0-11)等于-1 就是二维比较的一维模式</param>
        /// <param name="_dir2No">short:方向2的位置源轴号(0-11)等于-1 就是二维比较的一维模式</param>
        /// <param name="_gateTime">short:脉冲方式的脉冲时间,单位ms</param>
        /// <param name="_errZone">short:进入比较点容差半径范围(pulse)</param>
        /// <param name="_compos">int[]://x1,y1,x2,y2,x3,y3...数组数据是一对一对取的,前面那个作为_dir1No的,后面这个作为_dir2No的</param>
        /// <returns>short:返回指令执行结果</returns>
        public short Compare2Dimens(ushort _Devhandle, short _dir1No, short _dir2No, short _gateTime, short _errZone, int[] _compos)
        {
            short rtn = 0;
            //二维位置比较参数
            NMCLib20.TComp2DimensParam comp2Dimens = new NMCLib20.TComp2DimensParam(true);
            comp2Dimens.outputchn = new short[3] { 0, -1, -1 };//比较输出的通道,-1表示不输出处理
            comp2Dimens.outputType = new short[3] { 0, 0, 0 };//输出方式,0:脉冲1:电平
            comp2Dimens.chnType = new short[3] { 1, 0, 0 };//通道类型,0:GPO,1:GATE
            comp2Dimens.dir1No = _dir1No;//方向1的位置源轴号(0-11)等于-1 就是二维比较的一维模式
            comp2Dimens.dir2No = _dir2No;//方向2的位置源轴号(0-11)等于-1 就是二维比较的一维模式
            comp2Dimens.posSrc = 0;//轴位置类型,0:规划1:编码器
            comp2Dimens.stLevel = new short[3] { 0, 0, 0 };//电平模式下的起始电平(0或1:低或高)
            comp2Dimens.gateTime = new short[3] { _gateTime, 100, 100 };//脉冲方式的脉冲时间:单位ms
            comp2Dimens.errZone = _errZone;//进入比较点容差半径范围(pulse)
            //设置二维位置比较的参数
            rtn = NMCLib20.NMC_Comp2DimensSetParam(Devhandle[_Devhandle], 0, ref comp2Dimens, 0);//控制器句柄;组号,0 或者1;参数;保留,设为0

            int[] compos = _compos;//x1,y1,x2,y2,x3,y3.......数组数据是一对一对的取的，前面那个作为dir1no的，后面这个dir2no的

            //先清空
            int[] compos2 = new int[0];
            rtn = NMCLib20.NMC_Comp2DimensOnoff(Devhandle[_Devhandle], 0, 0, 0);//控制器句柄;组号,0 或者1;0停止,1输出;2手动;保留,设为0
            rtn = NMCLib20.NMC_Comp2DimensSetData(Devhandle[_Devhandle], 0, compos2, 0, 0);//控制器句柄;组号,0或者1;比较数组地址;比较数量;保留,设为0
            rtn = NMCLib20.NMC_Comp2DimensOnoff(Devhandle[_Devhandle], 0, 0, 0);//控制器句柄;组号,0 或者1;0停止,1输出;2手动;保留,设为0
            //设置二维比较数据
            rtn = NMCLib20.NMC_Comp2DimensSetData(Devhandle[_Devhandle], 0, compos, (short)(compos.Length / 2), 0);//控制器句柄;组号,0或者1;比较数组地址;比较数量;保留,设为0
            //二维位置比较使能
            rtn = NMCLib20.NMC_Comp2DimensOnoff(Devhandle[_Devhandle], 0, 1, 0);//控制器句柄;组号,0 或者1;0停止,1输出;2手动;保留,设为0

            return rtn;
        }
        #endregion

        #region Compare2DimensClear:二维高速位置比较停止并清空
        /// <summary>
        /// Compare2DimensClear:二维高速位置比较停止并清空
        /// </summary>
        /// <param name="_Devhandle">ushort:控制器句柄0-N</param>
        /// <returns>short:返回指令执行结果</returns>
        public short Compare2DimensClear(ushort _Devhandle)
        {
            short rtn = 0;
            int[] compos2 = new int[0];
            rtn = NMCLib20.NMC_Comp2DimensOnoff(Devhandle[_Devhandle], 0, 0, 0);//控制器句柄;组号,0 或者1;0停止,1输出;2手动;保留,设为0
            rtn = NMCLib20.NMC_Comp2DimensSetData(Devhandle[_Devhandle], 0, compos2, 0, 0);//控制器句柄;组号,0或者1;比较数组地址;比较数量;保留,设为0
            rtn = NMCLib20.NMC_Comp2DimensOnoff(Devhandle[_Devhandle], 0, 0, 0);//控制器句柄;组号,0 或者1;0停止,1输出;2手动;保留,设为0
            return rtn;
        }
        #endregion

        #region servoOffOn
        /// <summary>
        /// servoOffOn
        /// </summary>
        ///<param name = "axisNumber" > int:轴号0-N</param>
        ///<param name = "offOn" > bool:off/On(false/true)</param>
        public void servoOffOn(int axisNumber, bool offOn)
        {
            if (offOn)
                NMCLib20.NMC_MtSetSvOn(axishandle[axisNumber]);
            else
                NMCLib20.NMC_MtSetSvOff(axishandle[axisNumber]);
        }
        #endregion

        #region servoAlarmClear
        /// <summary>
        /// servoAlarmClear
        /// </summary>
        ///<param name = "axisNumber" > int:轴号0-N</param>
        ///<param name = "offOn" > bool:off/On(false/true)</param>
        public void servoAlarmClear(int axisNumber, bool offOn)
        {
            short clear = 0;
            if (offOn)
                clear = 0;
            else
                clear = 1;
            NMCLib20.NMC_MtSetSvClr(axishandle[axisNumber], clear);
            NMCLib20.NMC_MtClrError(axishandle[axisNumber]);//清除轴错误状态
            switch (axisNumber)//把ServoAlarmClear输出点状态Flag更新
            {
                case 0:
                    _axis00ServoAlarmClearFlag = offOn;
                    break;
                case 1:
                    _axis01ServoAlarmClearFlag = offOn;
                    break;
                case 2:
                    _axis02ServoAlarmClearFlag = offOn;
                    break;
                case 3:
                    _axis03ServoAlarmClearFlag = offOn;
                    break;
                case 4:
                    _axis04ServoAlarmClearFlag = offOn;
                    break;
                case 5:
                    _axis05ServoAlarmClearFlag = offOn;
                    break;
                case 6:
                    _axis06ServoAlarmClearFlag = offOn;
                    break;
                case 7:
                    _axis07ServoAlarmClearFlag = offOn;
                    break;
                case 8:
                    _axis08ServoAlarmClearFlag = offOn;
                    break;
                case 9:
                    _axis09ServoAlarmClearFlag = offOn;
                    break;
                case 10:
                    _axis10ServoAlarmClearFlag = offOn;
                    break;
                case 11:
                    _axis11ServoAlarmClearFlag = offOn;
                    break;
            }
        }
        #endregion

        #region feiPaiCameraTrigger:飞拍相机触发
        /// <summary>
        /// feiPaiCameraTrigger:飞拍相机触发
        /// </summary>
        /// <param name="_Devhandle">ushort:控制器句柄0-N</param>
        /// <param name="_gateTime">short:脉冲方式的脉冲时间,单位ms</param>
        /// <returns>short:返回指令执行结果</returns>
        public short feiPaiCameraTrigger(ushort _Devhandle, short _gateTime)
        {
            short rtn = 0;
            //二维位置比较参数
            NMCLib20.TComp2DimensParam comp2Dimens = new NMCLib20.TComp2DimensParam(true);
            comp2Dimens.outputchn = new short[3] { 0, -1, -1 };//比较输出的通道,-1表示不输出处理
            comp2Dimens.outputType = new short[3] { 0, 0, 0 };//输出方式,0:脉冲1:电平
            comp2Dimens.chnType = new short[3] { 1, 0, 0 };//通道类型,0:GPO,1:GATE
            comp2Dimens.dir1No = 0;//方向1的位置源轴号(0-11)等于-1 就是二维比较的一维模式
            comp2Dimens.dir2No = 0;//方向2的位置源轴号(0-11)等于-1 就是二维比较的一维模式
            comp2Dimens.posSrc = 0;//轴位置类型,0:规划1:编码器
            comp2Dimens.stLevel = new short[3] { 0, 0, 0 };//电平模式下的起始电平(0或1:低或高)
            comp2Dimens.gateTime = new short[3] { _gateTime, 100, 100 };//脉冲方式的脉冲时间:单位ms
            comp2Dimens.errZone = 5;//进入比较点容差半径范围(pulse)
            //设置二维位置比较的参数
            rtn = NMCLib20.NMC_Comp2DimensSetParam(Devhandle[_Devhandle], 0, ref comp2Dimens, 0);//控制器句柄;组号,0 或者1;参数;保留,设为0

            //int[] compos = _compos;//x1,y1,x2,y2,x3,y3.......数组数据是一对一对的取的，前面那个作为dir1no的，后面这个dir2no的

            ////先清空
            //int[] compos2 = new int[0];
            //rtn = NMCLib20.NMC_Comp2DimensOnoff(Devhandle[_Devhandle], 0, 0, 0);//控制器句柄;组号,0 或者1;0停止,1输出;2手动;保留,设为0
            //rtn = NMCLib20.NMC_Comp2DimensSetData(Devhandle[_Devhandle], 0, compos2, 0, 0);//控制器句柄;组号,0或者1;比较数组地址;比较数量;保留,设为0
            //rtn = NMCLib20.NMC_Comp2DimensOnoff(Devhandle[_Devhandle], 0, 0, 0);//控制器句柄;组号,0 或者1;0停止,1输出;2手动;保留,设为0
            ////设置二维比较数据
            //rtn = NMCLib20.NMC_Comp2DimensSetData(Devhandle[_Devhandle], 0, compos, (short)(compos.Length / 2), 0);//控制器句柄;组号,0或者1;比较数组地址;比较数量;保留,设为0
            //二维位置比较使能
            rtn = NMCLib20.NMC_Comp2DimensOnoff(Devhandle[_Devhandle], 0, 2, 0);//控制器句柄;组号,0 或者1;0停止,1输出;2手动;保留,设为0

            return rtn;
        }
        #endregion

        #region lineInterpolation:直线插补(最多四轴)
        /// <summary>
        /// lineInterpolation:直线插补(最多四轴)
        /// </summary>
        /// <param name="_Devhandle">ushort:控制器句柄0-N</param>
        /// <param name="_axisMask">short:参与的轴,按位表示(参与作为1)</param>
        /// <param name="_targetPosition">int[]:目标位置数组(最大长度为4)</param>
        /// <param name="_endVel">double:终点速度,单位 pulse/ms</param>
        /// <param name="_maxVel">double:最大速度,单位 pulse/ms</param>
        /// <param name="_synAcc">double:合成加速度,单位 pulse/ms^2</param>
        /// <param name="_lookAhead">short:是否使用前瞻,0：使用前瞻,则控制器自动计算终点速度,1：禁用前瞻,使用设定的终点速度（_endVel）</param>
        /// <returns>short:返回指令执行结果</returns>
        public short lineInterpolation(ushort _Devhandle, short _axisMask, int[] _targetPosition, double _endVel, double _maxVel, double _synAcc, short _lookAhead)
        {
            short rtn = 0;

            //建立坐标系
            ushort lineInterpolationCrdhandle = 0;//直线插补坐标系句柄
            rtn = NMCLib20.NMC_CrdOpen(Devhandle[_Devhandle], ref lineInterpolationCrdhandle);//打开一个新的坐标系
            NMCLib20.TCrdConfig crd = new NMCLib20.TCrdConfig(true);//定义坐标系配置参数crd
            crd.axCnts = 2;//轴数,目前坐标系最多支持4个轴
            if (_axisMask == 3)
                crd.axCnts = 2;
            if (_axisMask == 7)
                crd.axCnts = 3;
            if (_axisMask == 15)
                crd.axCnts = 4;
            crd.pAxis[0] = 0;//轴映射表,轴映射表中轴号取值范围为[0,n]
            crd.pAxis[1] = 1;
            crd.pAxis[2] = 2;
            crd.pAxis[3] = 3;
            crd.port[0] = 0;//端口映射表,端口均设为0
            crd.port[1] = 0;
            crd.port[2] = 0;
            crd.port[3] = 0;
            rtn = NMCLib20.NMC_CrdConfig(lineInterpolationCrdhandle, ref crd);//配置坐标系

            //设置坐标系运动参数
            NMCLib20.TCrdPara crdPara = new NMCLib20.TCrdPara(true);//定义坐标系运动参数crdPara
            crdPara.orgFlag = 1;//默认原点（0，0，0）
            crdPara.offset[0] = 0;
            crdPara.offset[1] = 0;
            crdPara.offset[2] = 0;
            crdPara.offset[3] = 0;
            crdPara.synAccMax = 50;//最大合成加速度p/ms^2,加速度为10已经是比较大的加速度了
            crdPara.synVelMax = 800;//最大合成速度p/ms,在轴允许范围内,这两个参数可以尽量大一些
            rtn = NMCLib20.NMC_CrdSetPara(lineInterpolationCrdhandle, ref crdPara);//设置坐标系运动参数

            //清除坐标系错误状态
            rtn = NMCLib20.NMC_CrdClrError(lineInterpolationCrdhandle);

            //压入指令之前需要清空缓存区
            rtn = NMCLib20.NMC_CrdBufClr(lineInterpolationCrdhandle);

            //插入运动指令
            //参数
            //坐标系句柄
            //段号
            //参与的轴,按位表示(参与作为1)
            //目标位置数组(最大长度为3)
            //终点速度,单位 pulse/msS
            //最大速度,单位 pulse/ms
            //合成加速度,单位 pulse/ms^2
            //是否使用前瞻,0：使用前瞻,则控制器自动计算终点速度,1：禁用前瞻,使用设定的终点速度(endVel)
            rtn = NMCLib20.NMC_CrdLineXYZA(lineInterpolationCrdhandle, 0, _axisMask, _targetPosition, _endVel, _maxVel, _synAcc, _lookAhead);//四轴直线插补(带前瞻开关)
            //rtn = NMCLib20.NMC_CrdLineXYZ(lineInterpolationCrdhandle, 0, _axisMask, _targetPosition, _endVel, _maxVel, _synAcc);//直线插补
            //rtn = NMCLib20.NMC_CrdLineXYZEx(lineInterpolationCrdhandle, 0, _axisMask, _targetPosition, _endVel, _maxVel, _synAcc, _lookAhead);//直线插补(带前瞻开关)

            //启动运动
            rtn = NMCLib20.NMC_CrdEndMtn(lineInterpolationCrdhandle);
            rtn = NMCLib20.NMC_CrdStartMtn(lineInterpolationCrdhandle);

            return rtn;
        }
        #endregion
    }
}