using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using csDmc1000;

namespace MotionCard
{
    public class LeiSaiMotionCardDMC1000BSerial
    {
        #region 变量声明

        #region 请根据项目需求修改
        public int _cardQuantity = 2;//指定卡的个数
        public int _axisQuantity;//轴的个数
        public int _motionCardIOMaxQuantity_64 = 64;//运动控制卡的最大IO个数:输入或者输出的最大个数(包含通用IO及专用IO:输入为64点,输出为27点)
        public int _motioncardCommonIOMAXQuantity_32 = 32;//运动控制卡的最大通用IO点数(通用输入为32点,通用输出为27点)
        public int _motionCardOutputQuantity_27 = 27;//运动控制卡的输出点数(只有通用输出)
        public int _maxIOQuantity;//输入或者输出的IO个数(一块卡为输入及输出都为64点,两块卡为128)

        public float[] _axismmPP = new float[] { 0.005f, 0.005f, 0.01f, 0.01f, 0.01f, 0.01f, 0.01f, 0.01f };
        public string[] _axisNameStrArray = new string[] { "X1轴", "Y1轴", "R1轴", "X2_CNV_L轴", "X3_CNV_R轴", "Y2_宽度调节轴", "Y3_宽度调节轴", "Y4_宽度调节轴" };
        public string[] _axisUnitArray = new string[] { "mm", "mm", "°", "mm", "mm", "mm", "mm", "mm" };//轴单位数组，为mm或°
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
        private int[][] axisIOStatus;
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

        private Thread td_axisStatusUpdata;//轴状态更新线程

        #endregion

        #region 构造函数
        public LeiSaiMotionCardDMC1000BSerial()
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
            _axisQuantity = _cardQuantity * 4;
            _maxIOQuantity = _cardQuantity * _motionCardIOMaxQuantity_64;

            int n = Dmc1000.d1000_board_init();
            if (n == _cardQuantity)
            {
                axisIOStatus = new int[_cardQuantity][];
                axisIOStatus[0] = new int[4];
                axisIOStatus[1] = new int[4];
                _inPutStatus = new bool[_cardQuantity][];
                _inPutStatus[0] = new bool[_motionCardIOMaxQuantity_64];
                _inPutStatus[1] = new bool[_motionCardIOMaxQuantity_64];
                _outPutStatus = new bool[_cardQuantity][];
                _outPutStatus[0] = new bool[_motionCardIOMaxQuantity_64];
                _outPutStatus[1] = new bool[_motionCardIOMaxQuantity_64];

                for (int i = 0; i < _cardQuantity; i++)
                {
                    for (int j = 0; j < _motionCardIOMaxQuantity_64; j++)
                    {
                        _inPutStatus[i][j] = false;
                        _outPutStatus[i][j] = false;
                    }
                }

                    if (n >= 1)
                    {
                        Dmc1000.d1000_set_pls_outmode(0, 3);//0：pulse/dir模式，脉冲上升沿有效
                        Dmc1000.d1000_set_pls_outmode(1, 3);//1：pulse/dir模式，脉冲下降沿有效
                        Dmc1000.d1000_set_pls_outmode(2, 3);//2：CW/CCW模式，脉冲上升沿有效
                        Dmc1000.d1000_set_pls_outmode(3, 0);//3：CW/CCW模式，脉冲下降沿有效

                        Dmc1000.d1000_set_sd(0, 0);
                        Dmc1000.d1000_set_sd(1, 0);
                        Dmc1000.d1000_set_sd(2, 0);
                        Dmc1000.d1000_set_sd(3, 0);

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
                        OutputCtrl(0, true);
                        OutputCtrl(1, true);
                        OutputCtrl(2, true);
                        OutputCtrl(3, true);
                    }
                if (n >= 2)
                {
                    Dmc1000.d1000_set_pls_outmode(4, 0);//0：pulse/dir模式，脉冲上升沿有效
                    Dmc1000.d1000_set_pls_outmode(5, 0);//1：pulse/dir模式，脉冲下降沿有效
                    Dmc1000.d1000_set_pls_outmode(6, 3);//2：CW/CCW模式，脉冲上升沿有效
                    Dmc1000.d1000_set_pls_outmode(7, 3);//3：CW/CCW模式，脉冲下降沿有效

                    Dmc1000.d1000_set_sd(4, 0);
                    Dmc1000.d1000_set_sd(5, 0);
                    Dmc1000.d1000_set_sd(6, 0);
                    Dmc1000.d1000_set_sd(7, 0);

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
                    OutputCtrl(0 + 64, true);
                    OutputCtrl(1 + 64, true);
                    OutputCtrl(2 + 64, true);
                    OutputCtrl(3 + 64, true);
                }
                if (n >= 3)
                {
                    Dmc1000.d1000_set_pls_outmode(8, 3);//0：pulse/dir模式，脉冲上升沿有效
                    Dmc1000.d1000_set_pls_outmode(9, 3);//1：pulse/dir模式，脉冲下降沿有效
                    Dmc1000.d1000_set_pls_outmode(10, 3);//2：CW/CCW模式，脉冲上升沿有效
                    Dmc1000.d1000_set_pls_outmode(11, 3);//3：CW/CCW模式，脉冲下降沿有效

                    Dmc1000.d1000_set_sd(8, 0);
                    Dmc1000.d1000_set_sd(9, 0);
                    Dmc1000.d1000_set_sd(10, 0);
                    Dmc1000.d1000_set_sd(11, 0);

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
                    OutputCtrl(0 + 64 + 64, true);
                    OutputCtrl(1 + 64 + 64, true);
                    OutputCtrl(2 + 64 + 64, true);
                    OutputCtrl(3 + 64 + 64, true);
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
            int resetOutputOnCount = 0;

            while (true)
            {
                if (_cardQuantity >= 1)
                {
                    //轴当前位置
                    axis00CurrentPosition_Pulse = Dmc1000.d1000_get_command_pos(0);
                    axis01CurrentPosition_Pulse = Dmc1000.d1000_get_command_pos(1);
                    axis02CurrentPosition_Pulse = Dmc1000.d1000_get_command_pos(2);
                    axis03CurrentPosition_Pulse = Dmc1000.d1000_get_command_pos(3);
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
                    _axis00CheckDoneStatus = Dmc1000.d1000_check_done(0);
                    _axis01CheckDoneStatus = Dmc1000.d1000_check_done(1);
                    _axis02CheckDoneStatus = Dmc1000.d1000_check_done(2);
                    _axis03CheckDoneStatus = Dmc1000.d1000_check_done(3);
                    //轴当前速度读取
                    _axis00CurrentSpeed_mmps = Dmc1000.d1000_get_speed(0) * _axis00mmPP;
                    _axis01CurrentSpeed_mmps = Dmc1000.d1000_get_speed(1) * _axis01mmPP;
                    _axis02CurrentSpeed_mmps = Dmc1000.d1000_get_speed(2) * _axis02mmPP;
                    _axis03CurrentSpeed_mmps = Dmc1000.d1000_get_speed(3) * _axis03mmPP;
                    //999:Homing
                    if (_axis00Command == 999 && _axis00CheckDoneStatus == 4 && _axis00HomeStartFlag)
                    {
                        _axis00HomeStartFlag = false;
                        _axis00Command = 0;
                        _axis00CommandSaved = 0;
                        _axis00HomedFlag = true;
                        Dmc1000.d1000_set_command_pos(0, 0);
                    }
                    if (_axis01Command == 999 && _axis01CheckDoneStatus == 4 && _axis01HomeStartFlag)
                    {
                        _axis01HomeStartFlag = false;
                        _axis01Command = 0;
                        _axis01CommandSaved = 0;
                        _axis01HomedFlag = true;
                        Dmc1000.d1000_set_command_pos(1, 0);
                    }
                    if (_axis02Command == 999 && _axis02CheckDoneStatus == 4 && _axis02HomeStartFlag)
                    {
                        _axis02HomeStartFlag = false;
                        _axis02Command = 0;
                        _axis02CommandSaved = 0;
                        _axis02HomedFlag = true;
                        Dmc1000.d1000_set_command_pos(2, 0);
                    }
                    if (_axis03Command == 999 && _axis03CheckDoneStatus == 4 && _axis03HomeStartFlag)
                    {
                        _axis03HomeStartFlag = false;
                        _axis03Command = 0;
                        _axis03CommandSaved = 0;
                        _axis03HomedFlag = true;
                        Dmc1000.d1000_set_command_pos(3, 0);
                    }
                    //1-900:点位号定位 或者 1000:实时位置数据定位
                    axis00CurrentPosition_Pulse = Dmc1000.d1000_get_command_pos(0);
                    _axis00CurrentPosition_mm = axis00CurrentPosition_Pulse * _axis00mmPP;
                    axis00CurrentDeviationValue_mm = (axis00CurrentPosition_Pulse - axis00TargetPosition_Pulse) * _axis00mmPP;
                    if (_axis00HomedFlag && _axis00MoveStartFlag && ((_axis00Command >= 1 && _axis00Command <= 900) || _axis00Command == 1000) && 
                        (Dmc1000.d1000_check_done(0) != 3 && axis00CurrentDeviationValue_mm >= axis00TargetDeviationValue_mm * -1 && axis00CurrentDeviationValue_mm <= axis00TargetDeviationValue_mm))//
                    {
                        _axis00CurrentPositionNumber = _axis00Command;
                        _axis00Command = 0;
                        _axis00CommandSaved = 0;
                        _axis00MoveStartFlag = false;
                    }
                    axis01CurrentPosition_Pulse = Dmc1000.d1000_get_command_pos(1);
                    _axis01CurrentPosition_mm = axis01CurrentPosition_Pulse * _axis01mmPP;
                    axis01CurrentDeviationValue_mm = (axis01CurrentPosition_Pulse - axis01TargetPosition_Pulse) * _axis01mmPP;
                    if (_axis01HomedFlag && _axis01MoveStartFlag && ((_axis01Command >= 1 && _axis01Command <= 900) || _axis01Command == 1000) &&
                        (Dmc1000.d1000_check_done(1) != 3 && axis01CurrentDeviationValue_mm >= axis01TargetDeviationValue_mm * -1 && axis01CurrentDeviationValue_mm <= axis01TargetDeviationValue_mm))//Dmc1000.d1000_check_done(1) == 1 || Dmc1000.d1000_check_done(1) == 0 && 
                    {
                        _axis01CurrentPositionNumber = _axis01Command;
                        _axis01Command = 0;
                        _axis01CommandSaved = 0;
                        _axis01MoveStartFlag = false;
                    }
                    axis02CurrentPosition_Pulse = Dmc1000.d1000_get_command_pos(2);
                    _axis02CurrentPosition_mm = axis02CurrentPosition_Pulse * _axis02mmPP;
                    axis02CurrentDeviationValue_mm = (axis02CurrentPosition_Pulse - axis02TargetPosition_Pulse) * _axis02mmPP;
                    if (_axis02HomedFlag && _axis02MoveStartFlag && ((_axis02Command >= 1 && _axis02Command <= 900) || _axis02Command == 1000) &&
                        (Dmc1000.d1000_check_done(2) != 3 && axis02CurrentDeviationValue_mm >= axis02TargetDeviationValue_mm * -1 && axis02CurrentDeviationValue_mm <= axis02TargetDeviationValue_mm))//Dmc1000.d1000_check_done(2) == 1 || Dmc1000.d1000_check_done(2) == 0 && 
                    {
                        _axis02CurrentPositionNumber = _axis02Command;
                        _axis02Command = 0;
                        _axis02CommandSaved = 0;
                        _axis02MoveStartFlag = false;
                    }
                    axis03CurrentPosition_Pulse = Dmc1000.d1000_get_command_pos(3);
                    _axis03CurrentPosition_mm = axis03CurrentPosition_Pulse * _axis03mmPP;
                    axis03CurrentDeviationValue_mm = (axis03CurrentPosition_Pulse - axis03TargetPosition_Pulse) * _axis03mmPP;
                    if (_axis03HomedFlag && _axis03MoveStartFlag && ((_axis03Command >= 1 && _axis03Command <= 900) || _axis03Command == 1000) &&
                        (Dmc1000.d1000_check_done(3) != 3 && axis03CurrentDeviationValue_mm >= axis03TargetDeviationValue_mm * -1 && axis03CurrentDeviationValue_mm <= axis03TargetDeviationValue_mm))//Dmc1000.d1000_check_done(3) == 1 || Dmc1000.d1000_check_done(3) == 0 && 
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
                }

                if (_cardQuantity >= 2)
                {
                    //轴当前位置
                    axis04CurrentPosition_Pulse = Dmc1000.d1000_get_command_pos(4);
                    axis05CurrentPosition_Pulse = Dmc1000.d1000_get_command_pos(5);
                    axis06CurrentPosition_Pulse = Dmc1000.d1000_get_command_pos(6);
                    axis07CurrentPosition_Pulse = Dmc1000.d1000_get_command_pos(7);
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
                    _axis04CheckDoneStatus = Dmc1000.d1000_check_done(4);
                    _axis05CheckDoneStatus = Dmc1000.d1000_check_done(5);
                    _axis06CheckDoneStatus = Dmc1000.d1000_check_done(6);
                    _axis07CheckDoneStatus = Dmc1000.d1000_check_done(7);
                    //轴当前速度读取
                    _axis04CurrentSpeed_mmps = Dmc1000.d1000_get_speed(4) * _axis04mmPP;
                    _axis05CurrentSpeed_mmps = Dmc1000.d1000_get_speed(5) * _axis05mmPP;
                    _axis06CurrentSpeed_mmps = Dmc1000.d1000_get_speed(6) * _axis06mmPP;
                    _axis07CurrentSpeed_mmps = Dmc1000.d1000_get_speed(7) * _axis07mmPP;
                    //999:Homing
                    if (_axis04Command == 999 && _axis04CheckDoneStatus == 4 && _axis04HomeStartFlag)
                    {
                        _axis04HomeStartFlag = false;
                        _axis04Command = 0;
                        _axis04CommandSaved = 0;
                        _axis04HomedFlag = true;
                        Dmc1000.d1000_set_command_pos(4, 0);
                    }
                    if (_axis05Command == 999 && _axis05CheckDoneStatus == 4 && _axis05HomeStartFlag)
                    {
                        _axis05HomeStartFlag = false;
                        _axis05Command = 0;
                        _axis05CommandSaved = 0;
                        _axis05HomedFlag = true;
                        Dmc1000.d1000_set_command_pos(5, 0);
                    }
                    if (_axis06Command == 999 && _axis06CheckDoneStatus == 4 && _axis06HomeStartFlag)
                    {
                        _axis06HomeStartFlag = false;
                        _axis06Command = 0;
                        _axis06CommandSaved = 0;
                        _axis06HomedFlag = true;
                        Dmc1000.d1000_set_command_pos(6, 0);
                    }
                    if (_axis07Command == 999 && _axis07CheckDoneStatus == 4 && _axis07HomeStartFlag)
                    {
                        _axis07HomeStartFlag = false;
                        _axis07Command = 0;
                        _axis07CommandSaved = 0;
                        _axis07HomedFlag = true;
                        Dmc1000.d1000_set_command_pos(7, 0);
                    }
                    //1-900:点位号定位 或者 1000:实时位置数据定位
                    axis04CurrentPosition_Pulse = Dmc1000.d1000_get_command_pos(4);
                    _axis04CurrentPosition_mm = axis04CurrentPosition_Pulse * _axis04mmPP;
                    axis04CurrentDeviationValue_mm = (axis04CurrentPosition_Pulse - axis04TargetPosition_Pulse) * _axis04mmPP;
                    if (_axis04HomedFlag && _axis04MoveStartFlag && ((_axis04Command >= 1 && _axis04Command <= 900) || _axis04Command == 1000) &&
                        (Dmc1000.d1000_check_done(4) != 3 && axis04CurrentDeviationValue_mm >= axis04TargetDeviationValue_mm * -1 && axis04CurrentDeviationValue_mm <= axis04TargetDeviationValue_mm))//Dmc1000.d1000_check_done(4) == 1 || Dmc1000.d1000_check_done(4) == 0 && 
                    {
                        _axis04CurrentPositionNumber = _axis04Command;
                        _axis04Command = 0;
                        _axis04CommandSaved = 0;
                        _axis04MoveStartFlag = false;
                    }
                    axis05CurrentPosition_Pulse = Dmc1000.d1000_get_command_pos(5);
                    _axis05CurrentPosition_mm = axis05CurrentPosition_Pulse * _axis05mmPP;
                    axis05CurrentDeviationValue_mm = (axis05CurrentPosition_Pulse - axis05TargetPosition_Pulse) * _axis05mmPP;
                    if (_axis05HomedFlag && _axis05MoveStartFlag && ((_axis05Command >= 1 && _axis05Command <= 900) || _axis05Command == 1000) &&
                        (Dmc1000.d1000_check_done(5) != 3 && axis05CurrentDeviationValue_mm >= axis05TargetDeviationValue_mm * -1 && axis05CurrentDeviationValue_mm <= axis05TargetDeviationValue_mm))//Dmc1000.d1000_check_done(5) == 1 || Dmc1000.d1000_check_done(5) == 0 && 
                    {
                        _axis05CurrentPositionNumber = _axis05Command;
                        _axis05Command = 0;
                        _axis05CommandSaved = 0;
                        _axis05MoveStartFlag = false;
                    }
                    axis06CurrentPosition_Pulse = Dmc1000.d1000_get_command_pos(6);
                    _axis06CurrentPosition_mm = axis06CurrentPosition_Pulse * _axis06mmPP;
                    axis06CurrentDeviationValue_mm = (axis06CurrentPosition_Pulse - axis06TargetPosition_Pulse) * _axis06mmPP;
                    if (_axis06HomedFlag && _axis06MoveStartFlag && ((_axis06Command >= 1 && _axis06Command <= 900) || _axis06Command == 1000) &&
                        (Dmc1000.d1000_check_done(6) != 3 && axis06CurrentDeviationValue_mm >= axis06TargetDeviationValue_mm * -1 && axis06CurrentDeviationValue_mm <= axis06TargetDeviationValue_mm))//Dmc1000.d1000_check_done(6) == 1 || Dmc1000.d1000_check_done(6) == 0 && 
                    {
                        _axis06CurrentPositionNumber = _axis06Command;
                        _axis06Command = 0;
                        _axis06CommandSaved = 0;
                        _axis06MoveStartFlag = false;
                    }
                    axis07CurrentPosition_Pulse = Dmc1000.d1000_get_command_pos(7);
                    _axis07CurrentPosition_mm = axis07CurrentPosition_Pulse * _axis07mmPP;
                    axis07CurrentDeviationValue_mm = (axis07CurrentPosition_Pulse - axis07TargetPosition_Pulse) * _axis07mmPP;
                    if (_axis07HomedFlag && _axis07MoveStartFlag && ((_axis07Command >= 1 && _axis07Command <= 900) || _axis07Command == 1000) &&
                        (Dmc1000.d1000_check_done(7) != 3 && axis07CurrentDeviationValue_mm >= axis07TargetDeviationValue_mm * -1 && axis07CurrentDeviationValue_mm <= axis07TargetDeviationValue_mm))//Dmc1000.d1000_check_done(7) == 1 || Dmc1000.d1000_check_done(7) == 0 && 
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
                }

                if (_cardQuantity >= 3)
                {
                    //轴当前位置
                    axis08CurrentPosition_Pulse = Dmc1000.d1000_get_command_pos(8);
                    axis09CurrentPosition_Pulse = Dmc1000.d1000_get_command_pos(9);
                    axis10CurrentPosition_Pulse = Dmc1000.d1000_get_command_pos(10);
                    axis11CurrentPosition_Pulse = Dmc1000.d1000_get_command_pos(11);
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
                    _axis08CheckDoneStatus = Dmc1000.d1000_check_done(8);
                    _axis09CheckDoneStatus = Dmc1000.d1000_check_done(9);
                    _axis10CheckDoneStatus = Dmc1000.d1000_check_done(10);
                    _axis11CheckDoneStatus = Dmc1000.d1000_check_done(11);
                    //轴当前速度读取
                    _axis08CurrentSpeed_mmps = Dmc1000.d1000_get_speed(8) * _axis08mmPP;
                    _axis09CurrentSpeed_mmps = Dmc1000.d1000_get_speed(9) * _axis09mmPP;
                    _axis10CurrentSpeed_mmps = Dmc1000.d1000_get_speed(10) * _axis10mmPP;
                    _axis11CurrentSpeed_mmps = Dmc1000.d1000_get_speed(11) * _axis11mmPP;
                    //999:Homing
                    if (_axis08Command == 999 && _axis08CheckDoneStatus == 4 && _axis08HomeStartFlag)
                    {
                        _axis08HomeStartFlag = false;
                        _axis08Command = 0;
                        _axis08CommandSaved = 0;
                        _axis08HomedFlag = true;
                        Dmc1000.d1000_set_command_pos(8, 0);
                    }
                    if (_axis09Command == 999 && _axis09CheckDoneStatus == 4 && _axis09HomeStartFlag)
                    {
                        _axis09HomeStartFlag = false;
                        _axis09Command = 0;
                        _axis09CommandSaved = 0;
                        _axis09HomedFlag = true;
                        Dmc1000.d1000_set_command_pos(9, 0);
                    }
                    if (_axis10Command == 999 && _axis10CheckDoneStatus == 4 && _axis10HomeStartFlag)
                    {
                        _axis10HomeStartFlag = false;
                        _axis10Command = 0;
                        _axis10CommandSaved = 0;
                        _axis10HomedFlag = true;
                        Dmc1000.d1000_set_command_pos(10, 0);
                    }
                    if (_axis11Command == 999 && _axis11CheckDoneStatus == 4 && _axis11HomeStartFlag)
                    {
                        _axis11HomeStartFlag = false;
                        _axis11Command = 0;
                        _axis11CommandSaved = 0;
                        _axis11HomedFlag = true;
                        Dmc1000.d1000_set_command_pos(11, 0);
                    }
                    //1-900:点位号定位 或者 1000:实时位置数据定位
                    axis08CurrentPosition_Pulse = Dmc1000.d1000_get_command_pos(8);
                    _axis08CurrentPosition_mm = axis08CurrentPosition_Pulse * _axis08mmPP;
                    axis08CurrentDeviationValue_mm = (axis08CurrentPosition_Pulse - axis08TargetPosition_Pulse) * _axis08mmPP;
                    if (_axis08HomedFlag && _axis08MoveStartFlag && ((_axis08Command >= 1 && _axis08Command <= 900) || _axis08Command == 1000) && (Dmc1000.d1000_check_done(8) == 1 || Dmc1000.d1000_check_done(8) == 0 && axis08CurrentDeviationValue_mm > axis08TargetDeviationValue_mm * -1 && axis08CurrentDeviationValue_mm <= axis08TargetDeviationValue_mm))
                    {
                        _axis08CurrentPositionNumber = _axis08Command;
                        _axis08Command = 0;
                        _axis08CommandSaved = 0;
                        _axis08MoveStartFlag = false;
                    }
                    axis09CurrentPosition_Pulse = Dmc1000.d1000_get_command_pos(9);
                    _axis09CurrentPosition_mm = axis09CurrentPosition_Pulse * _axis09mmPP;
                    axis09CurrentDeviationValue_mm = (axis09CurrentPosition_Pulse - axis09TargetPosition_Pulse) * _axis09mmPP;
                    if (_axis09HomedFlag && _axis09MoveStartFlag && ((_axis09Command >= 1 && _axis09Command <= 900) || _axis09Command == 1000) && (Dmc1000.d1000_check_done(9) == 1 || Dmc1000.d1000_check_done(9) == 0 && axis09CurrentDeviationValue_mm > axis09TargetDeviationValue_mm * -1 && axis09CurrentDeviationValue_mm <= axis09TargetDeviationValue_mm))
                    {
                        _axis09CurrentPositionNumber = _axis09Command;
                        _axis09Command = 0;
                        _axis09CommandSaved = 0;
                        _axis09MoveStartFlag = false;
                    }
                    axis10CurrentPosition_Pulse = Dmc1000.d1000_get_command_pos(10);
                    _axis10CurrentPosition_mm = axis10CurrentPosition_Pulse * _axis10mmPP;
                    axis10CurrentDeviationValue_mm = (axis10CurrentPosition_Pulse - axis10TargetPosition_Pulse) * _axis10mmPP;
                    if (_axis10HomedFlag && _axis10MoveStartFlag && ((_axis10Command >= 1 && _axis10Command <= 900) || _axis10Command == 1000) && (Dmc1000.d1000_check_done(10) == 1 || Dmc1000.d1000_check_done(10) == 0 && axis10CurrentDeviationValue_mm > axis10TargetDeviationValue_mm * -1 && axis10CurrentDeviationValue_mm <= axis10TargetDeviationValue_mm))
                    {
                        _axis10CurrentPositionNumber = _axis10Command;
                        _axis10Command = 0;
                        _axis10CommandSaved = 0;
                        _axis10MoveStartFlag = false;
                    }
                    axis11CurrentPosition_Pulse = Dmc1000.d1000_get_command_pos(11);
                    _axis11CurrentPosition_mm = axis11CurrentPosition_Pulse * _axis11mmPP;
                    axis11CurrentDeviationValue_mm = (axis11CurrentPosition_Pulse - axis11TargetPosition_Pulse) * _axis11mmPP;
                    if (_axis11HomedFlag && _axis11MoveStartFlag && ((_axis11Command >= 1 && _axis11Command <= 900) || _axis11Command == 1100) && (Dmc1000.d1000_check_done(11) == 1 || Dmc1000.d1000_check_done(11) == 0 && axis11CurrentDeviationValue_mm > axis11TargetDeviationValue_mm * -1 && axis11CurrentDeviationValue_mm <= axis11TargetDeviationValue_mm))
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
                }

                //读取IO状态
                //输入:前motioncardCommonIOMAXQuantity点固定为通用IO
                for (int i = 0; i < _cardQuantity; i++)
                {
                    for (int j = 0; j < _motioncardCommonIOMAXQuantity_32; j++)
                    {
                        _inPutStatus[i][j] = getInPut(i * _motioncardCommonIOMAXQuantity_32 + j + 1);
                    }
                }
                //输入:后motionCardIOMaxQuantity - motioncardCommonIOMAXQuantity点固定为轴专用IO
                for (int i = 0; i < _cardQuantity; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        axisIOStatus[i][j] = Dmc1000.d1000_get_axis_status(i * 4 + j);
                    }
                }
                if (_cardQuantity >= 1)
                {
                    _axis00ELMinus = GetBitValue(axisIOStatus[0][0], 0);
                    _axis00ELPlus = GetBitValue(axisIOStatus[0][0], 1);
                    _axis00ORG = GetBitValue(axisIOStatus[0][0], 2);
                    _axis00STP = GetBitValue(axisIOStatus[0][0], 3);
                    _axis00STA = GetBitValue(axisIOStatus[0][0], 4);
                    _axis00SDMinus = GetBitValue(axisIOStatus[0][0], 5);
                    _axis00SDPlus = GetBitValue(axisIOStatus[0][0], 6);
                    _axis00Reserved = false;

                    _axis01ELMinus = GetBitValue(axisIOStatus[0][1], 0);
                    _axis01ELPlus = GetBitValue(axisIOStatus[0][1], 1);
                    _axis01ORG = GetBitValue(axisIOStatus[0][1], 2);
                    _axis01STP = GetBitValue(axisIOStatus[0][1], 3);
                    _axis01STA = GetBitValue(axisIOStatus[0][1], 4);
                    _axis01SDMinus = GetBitValue(axisIOStatus[0][1], 5);
                    _axis01SDPlus = GetBitValue(axisIOStatus[0][1], 6);
                    _axis01Reserved = false;

                    _axis02ELMinus = GetBitValue(axisIOStatus[0][2], 0);
                    _axis02ELPlus = GetBitValue(axisIOStatus[0][2], 1);
                    _axis02ORG = GetBitValue(axisIOStatus[0][2], 2);
                    _axis02STP = GetBitValue(axisIOStatus[0][2], 3);
                    _axis02STA = GetBitValue(axisIOStatus[0][2], 4);
                    _axis02SDMinus = GetBitValue(axisIOStatus[0][2], 5);
                    _axis02SDPlus = GetBitValue(axisIOStatus[0][2], 6);
                    _axis02Reserved = false;

                    _axis03ELMinus = GetBitValue(axisIOStatus[0][3], 0);
                    _axis03ELPlus = GetBitValue(axisIOStatus[0][3], 1);
                    _axis03ORG = GetBitValue(axisIOStatus[0][3], 2);
                    _axis03STP = GetBitValue(axisIOStatus[0][3], 3);
                    _axis03STA = GetBitValue(axisIOStatus[0][3], 4);
                    _axis03SDMinus = GetBitValue(axisIOStatus[0][3], 5);
                    _axis03SDPlus = GetBitValue(axisIOStatus[0][3], 6);
                    _axis03Reserved = false;
                }
                if (_cardQuantity >= 2)
                {
                    _axis04ELMinus = GetBitValue(axisIOStatus[1][0], 0);
                    _axis04ELPlus = GetBitValue(axisIOStatus[1][0], 1);
                    _axis04ORG = GetBitValue(axisIOStatus[1][0], 2);
                    _axis04STP = GetBitValue(axisIOStatus[1][0], 3);
                    _axis04STA = GetBitValue(axisIOStatus[1][0], 4);
                    _axis04SDMinus = GetBitValue(axisIOStatus[1][0], 5);
                    _axis04SDPlus = GetBitValue(axisIOStatus[1][0], 6);
                    _axis04Reserved = false;

                    _axis05ELMinus = GetBitValue(axisIOStatus[1][1], 0);
                    _axis05ELPlus = GetBitValue(axisIOStatus[1][1], 1);
                    _axis05ORG = GetBitValue(axisIOStatus[1][1], 2);
                    _axis05STP = GetBitValue(axisIOStatus[1][1], 3);
                    _axis05STA = GetBitValue(axisIOStatus[1][1], 4);
                    _axis05SDMinus = GetBitValue(axisIOStatus[1][1], 5);
                    _axis05SDPlus = GetBitValue(axisIOStatus[1][1], 6);
                    _axis05Reserved = false;

                    _axis06ELMinus = GetBitValue(axisIOStatus[1][2], 0);
                    _axis06ELPlus = GetBitValue(axisIOStatus[1][2], 1);
                    _axis06ORG = GetBitValue(axisIOStatus[1][2], 2);
                    _axis06STP = GetBitValue(axisIOStatus[1][2], 3);
                    _axis06STA = GetBitValue(axisIOStatus[1][2], 4);
                    _axis06SDMinus = GetBitValue(axisIOStatus[1][2], 5);
                    _axis06SDPlus = GetBitValue(axisIOStatus[1][2], 6);
                    _axis06Reserved = false;

                    _axis07ELMinus = GetBitValue(axisIOStatus[1][3], 0);
                    _axis07ELPlus = GetBitValue(axisIOStatus[1][3], 1);
                    _axis07ORG = GetBitValue(axisIOStatus[1][3], 2);
                    _axis07STP = GetBitValue(axisIOStatus[1][3], 3);
                    _axis07STA = GetBitValue(axisIOStatus[1][3], 4);
                    _axis07SDMinus = GetBitValue(axisIOStatus[1][3], 5);
                    _axis07SDPlus = GetBitValue(axisIOStatus[1][3], 6);
                    _axis07Reserved = false;
                }
                if (_cardQuantity >= 3)
                {
                    _axis08ELMinus = GetBitValue(axisIOStatus[2][0], 0);
                    _axis08ELPlus = GetBitValue(axisIOStatus[2][0], 1);
                    _axis08ORG = GetBitValue(axisIOStatus[2][0], 2);
                    _axis08STP = GetBitValue(axisIOStatus[2][0], 3);
                    _axis08STA = GetBitValue(axisIOStatus[2][0], 4);
                    _axis08SDMinus = GetBitValue(axisIOStatus[2][0], 5);
                    _axis08SDPlus = GetBitValue(axisIOStatus[2][0], 6);
                    _axis08Reserved = false;

                    _axis09ELMinus = GetBitValue(axisIOStatus[2][1], 0);
                    _axis09ELPlus = GetBitValue(axisIOStatus[2][1], 1);
                    _axis09ORG = GetBitValue(axisIOStatus[2][1], 2);
                    _axis09STP = GetBitValue(axisIOStatus[2][1], 3);
                    _axis09STA = GetBitValue(axisIOStatus[2][1], 4);
                    _axis09SDMinus = GetBitValue(axisIOStatus[2][1], 5);
                    _axis09SDPlus = GetBitValue(axisIOStatus[2][1], 6);
                    _axis09Reserved = false;

                    _axis10ELMinus = GetBitValue(axisIOStatus[2][2], 0);
                    _axis10ELPlus = GetBitValue(axisIOStatus[2][2], 1);
                    _axis10ORG = GetBitValue(axisIOStatus[2][2], 2);
                    _axis10STP = GetBitValue(axisIOStatus[2][2], 3);
                    _axis10STA = GetBitValue(axisIOStatus[2][2], 4);
                    _axis10SDMinus = GetBitValue(axisIOStatus[2][2], 5);
                    _axis10SDPlus = GetBitValue(axisIOStatus[2][2], 6);
                    _axis10Reserved = false;

                    _axis11ELMinus = GetBitValue(axisIOStatus[2][3], 0);
                    _axis11ELPlus = GetBitValue(axisIOStatus[2][3], 1);
                    _axis11ORG = GetBitValue(axisIOStatus[2][3], 2);
                    _axis11STP = GetBitValue(axisIOStatus[2][3], 3);
                    _axis11STA = GetBitValue(axisIOStatus[2][3], 4);
                    _axis11SDMinus = GetBitValue(axisIOStatus[2][3], 5);
                    _axis11SDPlus = GetBitValue(axisIOStatus[2][3], 6);
                    _axis11Reserved = false;
                }

                for (int i = 0; i < _cardQuantity; i++)
                {
                    for (ushort j = 0; j < _motionCardIOMaxQuantity_64 - _motioncardCommonIOMAXQuantity_32; j++)
                    {
                        _inPutStatus[i][_motionCardIOMaxQuantity_64 - _motioncardCommonIOMAXQuantity_32 + j] = GetBitValue(axisIOStatus[i][j / 8], Convert.ToUInt16(j % 8));
                        if (j == 7 || j == 15 || j == 23 || j == 31)
                            _inPutStatus[i][_motionCardIOMaxQuantity_64 - _motioncardCommonIOMAXQuantity_32 + j] = false;
                    }
                }

                //输出:前motionCardOutputQuqntity点位为通用IO
                for (int i = 0; i < _cardQuantity; i++)
                {
                    for (int j = 0; j < _motionCardOutputQuantity_27; j++)
                    {
                        _outPutStatus[i][j] = getOutPut(i * _motioncardCommonIOMAXQuantity_32 + j + 1);
                    }
                }
                //输出:无后motionCardIOMaxQuantity - motionCardOutputQuantity点位
                for (int i = 0; i < _cardQuantity; i++)
                {
                    for (int j = _motionCardOutputQuantity_27; j < _motionCardIOMaxQuantity_64; j++)
                    {
                        _outPutStatus[i][j] = false;
                    }
                }

                if (_axisQuantity >= 1)
                {
                    _axis00ServoOnOffFlag = getOutPut(1);
                    if (!_axis00ServoOnOffFlag || !getInPut(1))
                        _axis00HomedFlag = false;

                    //Axis00 Alarm
                    if (getInPut(1))
                        _axis00ErrorNumber = 0;
                    else
                    {
                        _axis00ErrorNumber = 1;
                        axisDecelStop(0);
                        //if (_axis00CheckDoneStatus != 0)
                        //{
                            _axis00HomedFlag = false;
                            _axis00Command = 0;
                            _axis00CommandSaved = 0;
                        //}
                    }

                    //增加在Jog时,因为其它情况导致的停止(例如极限传感器),对标志位的清零
                    if (_axis00CommandSaved == 901 && _axis00CheckDoneStatus != 0 && _axis00CheckDoneStatus != 1 && _axis00CheckDoneStatus != 2)
                    {
                        jogAxisImmediateStop(0);
                    }
                }

                if (_axisQuantity >= 2)
                {
                    _axis01ServoOnOffFlag = getOutPut(2);
                    if (!_axis01ServoOnOffFlag || !getInPut(2))
                        _axis01HomedFlag = false;

                    //Axis01 Alarm
                    if (getInPut(2))
                        _axis01ErrorNumber = 0;
                    else
                    {
                        _axis01ErrorNumber = 1;
                        axisDecelStop(1);
                        //if (_axis01CheckDoneStatus != 0)
                        //{
                            _axis01HomedFlag = false;
                            _axis01Command = 0;
                            _axis01CommandSaved = 0;
                        //}
                    }

                    //增加在Jog时,因为其它情况导致的停止(例如极限传感器),对标志位的清零
                    if (_axis01CommandSaved == 901 && _axis01CheckDoneStatus != 0 && _axis01CheckDoneStatus != 1 && _axis01CheckDoneStatus != 2)
                    {
                        jogAxisImmediateStop(1);
                    }
                }

                if (_axisQuantity >= 3)
                {
                    _axis02ServoOnOffFlag = getOutPut(3);
                    if (!_axis02ServoOnOffFlag || !getInPut(3))
                        _axis02HomedFlag = false;

                    //Axis02 Alarm
                    if (getInPut(3))
                        _axis02ErrorNumber = 0;
                    else
                    {
                        _axis02ErrorNumber = 1;
                        axisDecelStop(2);
                        //if (_axis02CheckDoneStatus != 0)
                        //{
                            _axis02HomedFlag = false;
                            _axis02Command = 0;
                            _axis02CommandSaved = 0;
                        //}
                    }

                    //增加在Jog时,因为其它情况导致的停止(例如极限传感器),对标志位的清零
                    if (_axis02CommandSaved == 901 && _axis02CheckDoneStatus != 0 && _axis02CheckDoneStatus != 1 && _axis02CheckDoneStatus != 2)
                    {
                        jogAxisImmediateStop(2);
                    }
                }

                if (_axisQuantity >= 4)
                {
                    _axis03ServoOnOffFlag = getOutPut(4);
                    //if (!_axis03ServoOnOffFlag || !getInPut(4))
                    //    _axis03HomedFlag = false;

                    //Axis03 Alarm
                    if (true)//getInPut(4))
                        _axis03ErrorNumber = 0;
                    else
                    {
                        _axis03ErrorNumber = 1;
                        axisDecelStop(3);
                        //if (_axis03CheckDoneStatus != 0)
                        //{
                            _axis03HomedFlag = false;
                            _axis03Command = 0;
                            _axis03CommandSaved = 0;
                        //}
                    }

                    //增加在Jog时,因为其它情况导致的停止(例如极限传感器),对标志位的清零
                    if (_axis03CommandSaved == 901 && _axis03CheckDoneStatus != 0 && _axis03CheckDoneStatus != 1 && _axis03CheckDoneStatus != 2)
                    {
                        jogAxisImmediateStop(3);
                    }
                }

                if (_axisQuantity >= 5)
                {
                    _axis04ServoOnOffFlag = getOutPut(1 + 32);
                    //if (!_axis04ServoOnOffFlag || !getInPut(1 + 32))
                    //    _axis04HomedFlag = false;

                    //Axis04 Alarm
                    if(true) //(getInPut(1 + 32))
                        _axis04ErrorNumber = 0;
                    else
                    {
                        _axis04ErrorNumber = 1;
                        axisDecelStop(4);
                        //if (_axis04CheckDoneStatus != 0)
                        //{
                            _axis04HomedFlag = false;
                            _axis04Command = 0;
                            _axis04CommandSaved = 0;
                        //}
                    }
                    //增加在Jog时,因为其它情况导致的停止(例如极限传感器),对标志位的清零
                    if (_axis04CommandSaved == 901 && _axis04CheckDoneStatus != 0 && _axis04CheckDoneStatus != 1 && _axis04CheckDoneStatus != 2)
                    {
                        jogAxisImmediateStop(4);
                    }
                }

                if (_axisQuantity >= 6)
                {
                    _axis05ServoOnOffFlag = getOutPut(2 + 32);
                    //if (!_axis05ServoOnOffFlag || !getInPut(2 + 32))
                    //    _axis05HomedFlag = false;

                    //Axis05 Alarm
                    if(true) //(getInPut(2 + 32))
                        _axis05ErrorNumber = 0;
                    else
                    {
                        _axis05ErrorNumber = 1;
                        axisDecelStop(5);
                        //if (_axis05CheckDoneStatus != 0)
                        //{
                            _axis05HomedFlag = false;
                            _axis05Command = 0;
                            _axis05CommandSaved = 0;
                        //}
                    }
                    //增加在Jog时,因为其它情况导致的停止(例如极限传感器),对标志位的清零
                    if (_axis05CommandSaved == 901 && _axis05CheckDoneStatus != 0 && _axis05CheckDoneStatus != 1 && _axis05CheckDoneStatus != 2)
                    {
                        jogAxisImmediateStop(5);
                    }
                }

                if (_axisQuantity >= 7)
                {
                    _axis06ServoOnOffFlag = getOutPut(3 + 32);
                    if (!_axis06ServoOnOffFlag || !getInPut(3 + 32))
                        _axis06HomedFlag = false;

                    //Axis06 Alarm
                    if(getInPut(3 + 32))
                        _axis06ErrorNumber = 0;
                    else
                    {
                        _axis06ErrorNumber = 1;
                        axisDecelStop(6);
                        //if (_axis06CheckDoneStatus != 0)
                        //{
                            _axis06HomedFlag = false;
                            _axis06Command = 0;
                            _axis06CommandSaved = 0;
                        //}
                    }
                    //增加在Jog时,因为其它情况导致的停止(例如极限传感器),对标志位的清零
                    if (_axis06CommandSaved == 901 && _axis06CheckDoneStatus != 0 && _axis06CheckDoneStatus != 1 && _axis06CheckDoneStatus != 2)
                    {
                        jogAxisImmediateStop(6);
                    }
                }

                if (_axisQuantity >= 8)
                {
                    _axis07ServoOnOffFlag = getOutPut(4 + 32);
                    if (!_axis07ServoOnOffFlag || !getInPut(4 + 32))
                        _axis07HomedFlag = false;

                    //Axis07 Alarm
                    if (getInPut(4 + 32))
                        _axis07ErrorNumber = 0;
                    else
                    {
                        _axis07ErrorNumber = 1;
                        axisDecelStop(7);
                        //if (_axis07CheckDoneStatus != 0)
                        //{
                            _axis07HomedFlag = false;
                            _axis07Command = 0;
                            _axis07CommandSaved = 0;
                        //}
                    }
                    //增加在Jog时,因为其它情况导致的停止(例如极限传感器),对标志位的清零
                    if (_axis07CommandSaved == 901 && _axis07CheckDoneStatus != 0 && _axis07CheckDoneStatus != 1 && _axis07CheckDoneStatus != 2)
                    {
                        jogAxisImmediateStop(7);
                    }
                }

                if (_axisQuantity >= 9)
                {
                    _axis08ServoOnOffFlag = getOutPut(1 + 32 + 32);
                    if (!_axis08ServoOnOffFlag || !getInPut(1 + 32 + 32))
                        _axis08HomedFlag = false;

                    //Axis08 Alarm
                    if (getInPut(1 + 32 + 32))
                        _axis08ErrorNumber = 0;
                    else
                    {
                        _axis08ErrorNumber = 1;
                        axisDecelStop(8);
                        //if (_axis08CheckDoneStatus != 0)
                        //{
                            _axis08HomedFlag = false;
                            _axis08Command = 0;
                            _axis08CommandSaved = 0;
                        //}
                    }
                    //增加在Jog时,因为其它情况导致的停止(例如极限传感器),对标志位的清零
                    if (_axis08CommandSaved == 901 && _axis08CheckDoneStatus != 0 && _axis08CheckDoneStatus != 1 && _axis08CheckDoneStatus != 2)
                    {
                        jogAxisImmediateStop(8);
                    }
                }

                if (_axisQuantity >= 10)
                {
                    _axis09ServoOnOffFlag = getOutPut(2 + 32 + 32);
                    if (!_axis09ServoOnOffFlag || !getInPut(2 + 32 + 32))
                        _axis09HomedFlag = false;

                    //Axis09 Alarm
                    if (getInPut(2 + 32 + 32))
                        _axis09ErrorNumber = 0;
                    else
                    {
                        _axis09ErrorNumber = 1;
                        axisDecelStop(9);
                        //if (_axis09CheckDoneStatus != 0)
                        //{
                            _axis09HomedFlag = false;
                            _axis09Command = 0;
                            _axis09CommandSaved = 0;
                        //}
                    }
                    //增加在Jog时,因为其它情况导致的停止(例如极限传感器),对标志位的清零
                    if (_axis09CommandSaved == 901 && _axis09CheckDoneStatus != 0 && _axis09CheckDoneStatus != 1 && _axis09CheckDoneStatus != 2)
                    {
                        jogAxisImmediateStop(9);
                    }
                }

                if (_axisQuantity >= 11)
                {
                    _axis10ServoOnOffFlag = getOutPut(3 + 32 + 32);
                    if (!_axis10ServoOnOffFlag || !getInPut(3 + 32 + 32))
                        _axis10HomedFlag = false;

                    //Axis010 Alarm
                    if (getInPut(3 + 32 + 32))
                        _axis10ErrorNumber = 0;
                    else
                    {
                        _axis10ErrorNumber = 1;
                        axisDecelStop(10);
                        //if (_axis10CheckDoneStatus != 0)
                        //{
                            _axis10HomedFlag = false;
                            _axis10Command = 0;
                            _axis10CommandSaved = 0;
                        //}
                    }
                    //增加在Jog时,因为其它情况导致的停止(例如极限传感器),对标志位的清零
                    if (_axis10CommandSaved == 1001 && _axis10CheckDoneStatus != 0 && _axis10CheckDoneStatus != 1 && _axis10CheckDoneStatus != 2)
                    {
                        jogAxisImmediateStop(10);
                    }
                }

                if (_axisQuantity >= 12)
                {
                    _axis11ServoOnOffFlag = getOutPut(4 + 32 + 32);
                    if (!_axis11ServoOnOffFlag || !getInPut(4 + 32 + 32))
                        _axis11HomedFlag = false;

                    //Axis011 Alarm
                    if (getInPut(4 + 32 + 32))
                        _axis11ErrorNumber = 0;
                    else
                    {
                        _axis11ErrorNumber = 1;
                        axisDecelStop(11);
                        //if (_axis11CheckDoneStatus != 0)
                        //{
                            _axis11HomedFlag = false;
                            _axis11Command = 0;
                            _axis11CommandSaved = 0;
                        //}
                    }
                    //增加在Jog时,因为其它情况导致的停止(例如极限传感器),对标志位的清零
                    if (_axis11CommandSaved == 1101 && _axis11CheckDoneStatus != 0 && _axis11CheckDoneStatus != 1 && _axis11CheckDoneStatus != 2)
                    {
                        jogAxisImmediateStop(11);
                    }
                }

                //报警复位信号自动OFF
                if (getOutPut(17))
                    resetOutputOnCount++;
                if (resetOutputOnCount > 100)
                {
                    OutputCtrl(16, false);
                    resetOutputOnCount = 0;
                }

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
                                Dmc1000.d1000_start_sa_move(axisNumber, axis00TargetPosition_Pulse, axis00StartVelocity_Temp_pps, axis00MaxVelocity_Temp_pps, axis00TargetAccTime_s);
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
                                Dmc1000.d1000_start_sa_move(axisNumber, axis01TargetPosition_Pulse, axis01StartVelocity_Temp_pps, axis01MaxVelocity_Temp_pps, axis01TargetAccTime_s);
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
                                Dmc1000.d1000_start_sa_move(axisNumber, axis02TargetPosition_Pulse, axis02StartVelocity_Temp_pps, axis02MaxVelocity_Temp_pps, axis02TargetAccTime_s);
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
                                Dmc1000.d1000_start_sa_move(axisNumber, axis03TargetPosition_Pulse, axis03StartVelocity_Temp_pps, axis03MaxVelocity_Temp_pps, axis03TargetAccTime_s);
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
                                Dmc1000.d1000_start_sa_move(axisNumber, axis04TargetPosition_Pulse, axis04StartVelocity_Temp_pps, axis04MaxVelocity_Temp_pps, axis04TargetAccTime_s);
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
                                Dmc1000.d1000_start_sa_move(axisNumber, axis05TargetPosition_Pulse, axis05StartVelocity_Temp_pps, axis05MaxVelocity_Temp_pps, axis05TargetAccTime_s);
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
                                Dmc1000.d1000_start_sa_move(axisNumber, axis06TargetPosition_Pulse, axis06StartVelocity_Temp_pps, axis06MaxVelocity_Temp_pps, axis06TargetAccTime_s);
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
                                Dmc1000.d1000_start_sa_move(axisNumber, axis07TargetPosition_Pulse, axis07StartVelocity_Temp_pps, axis07MaxVelocity_Temp_pps, axis07TargetAccTime_s);
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
                                Dmc1000.d1000_start_sa_move(axisNumber, axis08TargetPosition_Pulse, axis08StartVelocity_Temp_pps, axis08MaxVelocity_Temp_pps, axis08TargetAccTime_s);
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
                                Dmc1000.d1000_start_sa_move(axisNumber, axis09TargetPosition_Pulse, axis09StartVelocity_Temp_pps, axis09MaxVelocity_Temp_pps, axis09TargetAccTime_s);
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
                                Dmc1000.d1000_start_sa_move(axisNumber, axis10TargetPosition_Pulse, axis10StartVelocity_Temp_pps, axis10MaxVelocity_Temp_pps, axis10TargetAccTime_s);
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
                                Dmc1000.d1000_start_sa_move(axisNumber, axis11TargetPosition_Pulse, axis11StartVelocity_Temp_pps, axis11MaxVelocity_Temp_pps, axis11TargetAccTime_s);
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
                                Dmc1000.d1000_start_sa_move(axisNumber, axis00TargetPosition_Pulse, axis00StartVelocity_Temp_pps, axis00MaxVelocity_Temp_pps, axis00TargetAccTime_s);
                            }
                        }
                        else//没有回原点
                        {
                            if (!jogDirection)//负方向
                            {
                                if (!inch)//点动
                                    Dmc1000.d1000_start_tv_move(axisNumber, axis00StartVelocity_Temp_pps, axis00MaxVelocity_Temp_pps * -1, axis00TargetAccTime_s);
                                else//寸动
                                {
                                    axis00TargetPosition_Pulse = Convert.ToInt32(axis00TargetPosition_mm * -1 / _axis00mmPP);
                                    Dmc1000.d1000_start_t_move(axisNumber, axis00TargetPosition_Pulse, axis00StartVelocity_Temp_pps, axis00MaxVelocity_Temp_pps, axis00TargetAccTime_s);
                                }
                            }
                            else//正方向
                            {
                                if (!inch)//点动
                                    Dmc1000.d1000_start_tv_move(axisNumber, axis00StartVelocity_Temp_pps, axis00MaxVelocity_Temp_pps, axis00TargetAccTime_s);
                                else//寸动
                                {
                                    axis00TargetPosition_Pulse = Convert.ToInt32(axis00TargetPosition_mm / _axis00mmPP);
                                    Dmc1000.d1000_start_t_move(axisNumber, axis00TargetPosition_Pulse, axis00StartVelocity_Temp_pps, axis00MaxVelocity_Temp_pps, axis00TargetAccTime_s);
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
                                Dmc1000.d1000_start_sa_move(axisNumber, axis01TargetPosition_Pulse, axis01StartVelocity_Temp_pps, axis01MaxVelocity_Temp_pps, axis01TargetAccTime_s);
                            }
                        }
                        else//没有回原点
                        {
                            if (!jogDirection)//负方向
                            {
                                if (!inch)//点动
                                    Dmc1000.d1000_start_tv_move(axisNumber, axis01StartVelocity_Temp_pps, axis01MaxVelocity_Temp_pps * -1, axis01TargetAccTime_s);
                                else//寸动
                                {
                                    axis01TargetPosition_Pulse = Convert.ToInt32(axis01TargetPosition_mm * -1 / _axis01mmPP);
                                    Dmc1000.d1000_start_t_move(axisNumber, axis01TargetPosition_Pulse, axis01StartVelocity_Temp_pps, axis01MaxVelocity_Temp_pps, axis01TargetAccTime_s);
                                }
                            }
                            else//正方向
                            {
                                if (!inch)//点动
                                    Dmc1000.d1000_start_tv_move(axisNumber, axis01StartVelocity_Temp_pps, axis01MaxVelocity_Temp_pps, axis01TargetAccTime_s);
                                else//寸动
                                {
                                    axis01TargetPosition_Pulse = Convert.ToInt32(axis01TargetPosition_mm / _axis01mmPP);
                                    Dmc1000.d1000_start_t_move(axisNumber, axis01TargetPosition_Pulse, axis01StartVelocity_Temp_pps, axis01MaxVelocity_Temp_pps, axis01TargetAccTime_s);
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
                                Dmc1000.d1000_start_sa_move(axisNumber, axis02TargetPosition_Pulse, axis02StartVelocity_Temp_pps, axis02MaxVelocity_Temp_pps, axis02TargetAccTime_s);
                            }
                        }
                        else//没有回原点
                        {
                            if (!jogDirection)//负方向
                            {
                                if (!inch)//点动
                                    Dmc1000.d1000_start_tv_move(axisNumber, axis02StartVelocity_Temp_pps, axis02MaxVelocity_Temp_pps * -1, axis02TargetAccTime_s);
                                else//寸动
                                {
                                    axis02TargetPosition_Pulse = Convert.ToInt32(axis02TargetPosition_mm * -1 / _axis02mmPP);
                                    Dmc1000.d1000_start_t_move(axisNumber, axis02TargetPosition_Pulse, axis02StartVelocity_Temp_pps, axis02MaxVelocity_Temp_pps, axis02TargetAccTime_s);
                                }
                            }
                            else//正方向
                            {
                                if (!inch)//点动
                                    Dmc1000.d1000_start_tv_move(axisNumber, axis02StartVelocity_Temp_pps, axis02MaxVelocity_Temp_pps, axis02TargetAccTime_s);
                                else//寸动
                                {
                                    axis02TargetPosition_Pulse = Convert.ToInt32(axis02TargetPosition_mm / _axis02mmPP);
                                    Dmc1000.d1000_start_t_move(axisNumber, axis02TargetPosition_Pulse, axis02StartVelocity_Temp_pps, axis02MaxVelocity_Temp_pps, axis02TargetAccTime_s);
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
                                Dmc1000.d1000_start_sa_move(axisNumber, axis03TargetPosition_Pulse, axis03StartVelocity_Temp_pps, axis03MaxVelocity_Temp_pps, axis03TargetAccTime_s);
                            }
                        }
                        else//没有回原点
                        {
                            if (!jogDirection)//负方向
                            {
                                if (!inch)//点动
                                    Dmc1000.d1000_start_tv_move(axisNumber, axis03StartVelocity_Temp_pps, axis03MaxVelocity_Temp_pps * -1, axis03TargetAccTime_s);
                                else//寸动
                                {
                                    axis03TargetPosition_Pulse = Convert.ToInt32(axis03TargetPosition_mm * -1 / _axis03mmPP);
                                    Dmc1000.d1000_start_t_move(axisNumber, axis03TargetPosition_Pulse, axis03StartVelocity_Temp_pps, axis03MaxVelocity_Temp_pps, axis03TargetAccTime_s);
                                }
                            }
                            else//正方向
                            {
                                if (!inch)//点动
                                    Dmc1000.d1000_start_tv_move(axisNumber, axis03StartVelocity_Temp_pps, axis03MaxVelocity_Temp_pps, axis03TargetAccTime_s);
                                else//寸动
                                {
                                    axis03TargetPosition_Pulse = Convert.ToInt32(axis03TargetPosition_mm / _axis03mmPP);
                                    Dmc1000.d1000_start_t_move(axisNumber, axis03TargetPosition_Pulse, axis03StartVelocity_Temp_pps, axis03MaxVelocity_Temp_pps, axis03TargetAccTime_s);
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
                                Dmc1000.d1000_start_sa_move(axisNumber, axis04TargetPosition_Pulse, axis04StartVelocity_Temp_pps, axis04MaxVelocity_Temp_pps, axis04TargetAccTime_s);
                            }
                        }
                        else//没有回原点
                        {
                            if (!jogDirection)//负方向
                            {
                                if (!inch)//点动
                                    Dmc1000.d1000_start_tv_move(axisNumber, axis04StartVelocity_Temp_pps, axis04MaxVelocity_Temp_pps * -1, axis04TargetAccTime_s);
                                else//寸动
                                {
                                    axis04TargetPosition_Pulse = Convert.ToInt32(axis04TargetPosition_mm * -1 / _axis04mmPP);
                                    Dmc1000.d1000_start_t_move(axisNumber, axis04TargetPosition_Pulse, axis04StartVelocity_Temp_pps, axis04MaxVelocity_Temp_pps, axis04TargetAccTime_s);
                                }
                            }
                            else//正方向
                            {
                                if (!inch)//点动
                                    Dmc1000.d1000_start_tv_move(axisNumber, axis04StartVelocity_Temp_pps, axis04MaxVelocity_Temp_pps, axis04TargetAccTime_s);
                                else//寸动
                                {
                                    axis04TargetPosition_Pulse = Convert.ToInt32(axis04TargetPosition_mm / _axis04mmPP);
                                    Dmc1000.d1000_start_t_move(axisNumber, axis04TargetPosition_Pulse, axis04StartVelocity_Temp_pps, axis04MaxVelocity_Temp_pps, axis04TargetAccTime_s);
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
                                Dmc1000.d1000_start_sa_move(axisNumber, axis05TargetPosition_Pulse, axis05StartVelocity_Temp_pps, axis05MaxVelocity_Temp_pps, axis05TargetAccTime_s);
                            }
                        }
                        else//没有回原点
                        {
                            if (!jogDirection)//负方向
                            {
                                if (!inch)//点动
                                    Dmc1000.d1000_start_tv_move(axisNumber, axis05StartVelocity_Temp_pps, axis05MaxVelocity_Temp_pps * -1, axis05TargetAccTime_s);
                                else//寸动
                                {
                                    axis05TargetPosition_Pulse = Convert.ToInt32(axis05TargetPosition_mm * -1 / _axis05mmPP);
                                    Dmc1000.d1000_start_t_move(axisNumber, axis05TargetPosition_Pulse, axis05StartVelocity_Temp_pps, axis05MaxVelocity_Temp_pps, axis05TargetAccTime_s);
                                }
                            }
                            else//正方向
                            {
                                if (!inch)//点动
                                    Dmc1000.d1000_start_tv_move(axisNumber, axis05StartVelocity_Temp_pps, axis05MaxVelocity_Temp_pps, axis05TargetAccTime_s);
                                else//寸动
                                {
                                    axis05TargetPosition_Pulse = Convert.ToInt32(axis05TargetPosition_mm / _axis05mmPP);
                                    Dmc1000.d1000_start_t_move(axisNumber, axis05TargetPosition_Pulse, axis05StartVelocity_Temp_pps, axis05MaxVelocity_Temp_pps, axis05TargetAccTime_s);
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
                                Dmc1000.d1000_start_sa_move(axisNumber, axis06TargetPosition_Pulse, axis06StartVelocity_Temp_pps, axis06MaxVelocity_Temp_pps, axis06TargetAccTime_s);
                            }
                        }
                        else//没有回原点
                        {
                            if (!jogDirection)//负方向
                            {
                                if (!inch)//点动
                                    Dmc1000.d1000_start_tv_move(axisNumber, axis06StartVelocity_Temp_pps, axis06MaxVelocity_Temp_pps * -1, axis06TargetAccTime_s);
                                else//寸动
                                {
                                    axis06TargetPosition_Pulse = Convert.ToInt32(axis06TargetPosition_mm * -1 / _axis06mmPP);
                                    Dmc1000.d1000_start_t_move(axisNumber, axis06TargetPosition_Pulse, axis06StartVelocity_Temp_pps, axis06MaxVelocity_Temp_pps, axis06TargetAccTime_s);
                                }
                            }
                            else//正方向
                            {
                                if (!inch)//点动
                                    Dmc1000.d1000_start_tv_move(axisNumber, axis06StartVelocity_Temp_pps, axis06MaxVelocity_Temp_pps, axis06TargetAccTime_s);
                                else//寸动
                                {
                                    axis06TargetPosition_Pulse = Convert.ToInt32(axis06TargetPosition_mm / _axis06mmPP);
                                    Dmc1000.d1000_start_t_move(axisNumber, axis06TargetPosition_Pulse, axis06StartVelocity_Temp_pps, axis06MaxVelocity_Temp_pps, axis06TargetAccTime_s);
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
                                Dmc1000.d1000_start_sa_move(axisNumber, axis07TargetPosition_Pulse, axis07StartVelocity_Temp_pps, axis07MaxVelocity_Temp_pps, axis07TargetAccTime_s);
                            }
                        }
                        else//没有回原点
                        {
                            if (!jogDirection)//负方向
                            {
                                if (!inch)//点动
                                    Dmc1000.d1000_start_tv_move(axisNumber, axis07StartVelocity_Temp_pps, axis07MaxVelocity_Temp_pps * -1, axis07TargetAccTime_s);
                                else//寸动
                                {
                                    axis07TargetPosition_Pulse = Convert.ToInt32(axis07TargetPosition_mm * -1 / _axis07mmPP);
                                    Dmc1000.d1000_start_t_move(axisNumber, axis07TargetPosition_Pulse, axis07StartVelocity_Temp_pps, axis07MaxVelocity_Temp_pps, axis07TargetAccTime_s);
                                }
                            }
                            else//正方向
                            {
                                if (!inch)//点动
                                    Dmc1000.d1000_start_tv_move(axisNumber, axis07StartVelocity_Temp_pps, axis07MaxVelocity_Temp_pps, axis07TargetAccTime_s);
                                else//寸动
                                {
                                    axis07TargetPosition_Pulse = Convert.ToInt32(axis07TargetPosition_mm / _axis07mmPP);
                                    Dmc1000.d1000_start_t_move(axisNumber, axis07TargetPosition_Pulse, axis07StartVelocity_Temp_pps, axis07MaxVelocity_Temp_pps, axis07TargetAccTime_s);
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
                                Dmc1000.d1000_start_sa_move(axisNumber, axis08TargetPosition_Pulse, axis08StartVelocity_Temp_pps, axis08MaxVelocity_Temp_pps, axis08TargetAccTime_s);
                            }
                        }
                        else//没有回原点
                        {
                            if (!jogDirection)//负方向
                            {
                                if (!inch)//点动
                                    Dmc1000.d1000_start_tv_move(axisNumber, axis08StartVelocity_Temp_pps, axis08MaxVelocity_Temp_pps * -1, axis08TargetAccTime_s);
                                else//寸动
                                {
                                    axis08TargetPosition_Pulse = Convert.ToInt32(axis08TargetPosition_mm * -1 / _axis08mmPP);
                                    Dmc1000.d1000_start_t_move(axisNumber, axis08TargetPosition_Pulse, axis08StartVelocity_Temp_pps, axis08MaxVelocity_Temp_pps, axis08TargetAccTime_s);
                                }
                            }
                            else//正方向
                            {
                                if (!inch)//点动
                                    Dmc1000.d1000_start_tv_move(axisNumber, axis08StartVelocity_Temp_pps, axis08MaxVelocity_Temp_pps, axis08TargetAccTime_s);
                                else//寸动
                                {
                                    axis08TargetPosition_Pulse = Convert.ToInt32(axis08TargetPosition_mm / _axis08mmPP);
                                    Dmc1000.d1000_start_t_move(axisNumber, axis08TargetPosition_Pulse, axis08StartVelocity_Temp_pps, axis08MaxVelocity_Temp_pps, axis08TargetAccTime_s);
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
                                Dmc1000.d1000_start_sa_move(axisNumber, axis09TargetPosition_Pulse, axis09StartVelocity_Temp_pps, axis09MaxVelocity_Temp_pps, axis09TargetAccTime_s);
                            }
                        }
                        else//没有回原点
                        {
                            if (!jogDirection)//负方向
                            {
                                if (!inch)//点动
                                    Dmc1000.d1000_start_tv_move(axisNumber, axis09StartVelocity_Temp_pps, axis09MaxVelocity_Temp_pps * -1, axis09TargetAccTime_s);
                                else//寸动
                                {
                                    axis09TargetPosition_Pulse = Convert.ToInt32(axis09TargetPosition_mm * -1 / _axis09mmPP);
                                    Dmc1000.d1000_start_t_move(axisNumber, axis09TargetPosition_Pulse, axis09StartVelocity_Temp_pps, axis09MaxVelocity_Temp_pps, axis09TargetAccTime_s);
                                }
                            }
                            else//正方向
                            {
                                if (!inch)//点动
                                    Dmc1000.d1000_start_tv_move(axisNumber, axis09StartVelocity_Temp_pps, axis09MaxVelocity_Temp_pps, axis09TargetAccTime_s);
                                else//寸动
                                {
                                    axis09TargetPosition_Pulse = Convert.ToInt32(axis09TargetPosition_mm / _axis09mmPP);
                                    Dmc1000.d1000_start_t_move(axisNumber, axis09TargetPosition_Pulse, axis09StartVelocity_Temp_pps, axis09MaxVelocity_Temp_pps, axis09TargetAccTime_s);
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
                                Dmc1000.d1000_start_sa_move(axisNumber, axis10TargetPosition_Pulse, axis10StartVelocity_Temp_pps, axis10MaxVelocity_Temp_pps, axis10TargetAccTime_s);
                            }
                        }
                        else//没有回原点
                        {
                            if (!jogDirection)//负方向
                            {
                                if (!inch)//点动
                                    Dmc1000.d1000_start_tv_move(axisNumber, axis10StartVelocity_Temp_pps, axis10MaxVelocity_Temp_pps * -1, axis10TargetAccTime_s);
                                else//寸动
                                {
                                    axis10TargetPosition_Pulse = Convert.ToInt32(axis10TargetPosition_mm * -1 / _axis10mmPP);
                                    Dmc1000.d1000_start_t_move(axisNumber, axis10TargetPosition_Pulse, axis10StartVelocity_Temp_pps, axis10MaxVelocity_Temp_pps, axis10TargetAccTime_s);
                                }
                            }
                            else//正方向
                            {
                                if (!inch)//点动
                                    Dmc1000.d1000_start_tv_move(axisNumber, axis10StartVelocity_Temp_pps, axis10MaxVelocity_Temp_pps, axis10TargetAccTime_s);
                                else//寸动
                                {
                                    axis10TargetPosition_Pulse = Convert.ToInt32(axis10TargetPosition_mm / _axis10mmPP);
                                    Dmc1000.d1000_start_t_move(axisNumber, axis10TargetPosition_Pulse, axis10StartVelocity_Temp_pps, axis10MaxVelocity_Temp_pps, axis10TargetAccTime_s);
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
                                Dmc1000.d1000_start_sa_move(axisNumber, axis11TargetPosition_Pulse, axis11StartVelocity_Temp_pps, axis11MaxVelocity_Temp_pps, axis11TargetAccTime_s);
                            }
                        }
                        else//没有回原点
                        {
                            if (!jogDirection)//负方向
                            {
                                if (!inch)//点动
                                    Dmc1000.d1000_start_tv_move(axisNumber, axis11StartVelocity_Temp_pps, axis11MaxVelocity_Temp_pps * -1, axis11TargetAccTime_s);
                                else//寸动
                                {
                                    axis11TargetPosition_Pulse = Convert.ToInt32(axis11TargetPosition_mm * -1 / _axis11mmPP);
                                    Dmc1000.d1000_start_t_move(axisNumber, axis11TargetPosition_Pulse, axis11StartVelocity_Temp_pps, axis11MaxVelocity_Temp_pps, axis11TargetAccTime_s);
                                }
                            }
                            else//正方向
                            {
                                if (!inch)//点动
                                    Dmc1000.d1000_start_tv_move(axisNumber, axis11StartVelocity_Temp_pps, axis11MaxVelocity_Temp_pps, axis11TargetAccTime_s);
                                else//寸动
                                {
                                    axis11TargetPosition_Pulse = Convert.ToInt32(axis11TargetPosition_mm / _axis11mmPP);
                                    Dmc1000.d1000_start_t_move(axisNumber, axis11TargetPosition_Pulse, axis11StartVelocity_Temp_pps, axis11MaxVelocity_Temp_pps, axis11TargetAccTime_s);
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

        #region axisActionLineInterpolation:轴直线插补运行(axisCommand取值范围 0:NA;1-900:点位号定位;1000:实时位置数据定位)
        /// <summary>
        /// axisActionLineInterpolation:轴直线插补运行
        /// </summary>
        /// <param name="axisCommand">
        /// <param name="axisCount">int:轴个数2-4</param>
        /// <param name="axisNumberArray">int[]:轴号0-N</param>
        /// <param name="targetPositionArray">float[]:对应轴号列表各轴的绝对坐标的位置列表</param>
        /// <param name="startVelocity">float:运动初始速度(换算到第一个轴号),单位:mmps</param>
        /// <param name="maxVelocity">float:运动速度(换算到第一个轴号),单位:mmps</param>
        /// <param name="accTime">double:加速时间(用第一个轴的),单位:s</param>
        /// <param name="deviationValueArray">float[]:定位偏差值,单位:mm</param>
        public void axisActionLineInterpolation(int axisCommand, int axisCount, UInt16[] axisNumberArray, float[] targetPositionArray, float startVelocity, float maxVelocity, double accTime, float[] deviationValueArray)
        {
            int startVelocityPPS = 1;
            int maxVelocityPPS = 1;
            int[] targetPositionArrayPulse = new int[axisCount];
            bool needRunFlag = false;

            for (int i = 0; i < axisCount; i++)
            {
                if (i == 0)
                {
                    switch (axisNumberArray[i])
                    {
                        case 0:
                            startVelocityPPS = Convert.ToInt32(startVelocity / _axis00mmPP);
                            maxVelocityPPS = Convert.ToInt32(maxVelocity / _axis00mmPP);
                            break;
                        case 1:
                            startVelocityPPS = Convert.ToInt32(startVelocity / _axis01mmPP);
                            maxVelocityPPS = Convert.ToInt32(maxVelocity / _axis01mmPP);
                            break;
                        case 2:
                            startVelocityPPS = Convert.ToInt32(startVelocity / _axis02mmPP);
                            maxVelocityPPS = Convert.ToInt32(maxVelocity / _axis02mmPP);
                            break;
                        case 3:
                            startVelocityPPS = Convert.ToInt32(startVelocity / _axis03mmPP);
                            maxVelocityPPS = Convert.ToInt32(maxVelocity / _axis03mmPP);
                            break;
                    }
                }
                switch (axisNumberArray[i])
                {
                    case 0:
                        _axis00Command = axisCommand;
                        _axis00CommandSaved = _axis00Command;
                        _axis00CurrentPositionNumber = 0;
                        axis00TargetPosition_mm = targetPositionArray[i];
                        axis00TargetDeviationValue_mm = deviationValueArray[i];
                        axis00TargetPosition_Pulse = Convert.ToInt32(axis00TargetPosition_mm / _axis00mmPP);
                        targetPositionArrayPulse[i] = axis00TargetPosition_Pulse;
                        if (targetPositionArrayPulse[i] != axis00CurrentPosition_Pulse)
                            needRunFlag = true;
                        break;
                    case 1:
                        _axis01Command = axisCommand;
                        _axis01CommandSaved = _axis01Command;
                        _axis01CurrentPositionNumber = 0;
                        axis01TargetPosition_mm = targetPositionArray[i];
                        axis01TargetDeviationValue_mm = deviationValueArray[i];
                        axis01TargetPosition_Pulse = Convert.ToInt32(axis01TargetPosition_mm / _axis01mmPP);
                        targetPositionArrayPulse[i] = axis01TargetPosition_Pulse;
                        if (targetPositionArrayPulse[i] != axis01CurrentPosition_Pulse)
                            needRunFlag = true;
                        break;
                    case 2:
                        _axis02Command = axisCommand;
                        _axis02CommandSaved = _axis02Command;
                        _axis02CurrentPositionNumber = 0;
                        axis02TargetPosition_mm = targetPositionArray[i];
                        axis02TargetDeviationValue_mm = deviationValueArray[i];
                        axis02TargetPosition_Pulse = Convert.ToInt32(axis02TargetPosition_mm / _axis02mmPP);
                        targetPositionArrayPulse[i] = axis02TargetPosition_Pulse;
                        if (targetPositionArrayPulse[i] != axis02CurrentPosition_Pulse)
                            needRunFlag = true;
                        break;
                    case 3:
                        _axis03Command = axisCommand;
                        _axis03CommandSaved = _axis03Command;
                        _axis03CurrentPositionNumber = 0;
                        axis03TargetPosition_mm = targetPositionArray[i];
                        axis03TargetDeviationValue_mm = deviationValueArray[i];
                        axis03TargetPosition_Pulse = Convert.ToInt32(axis03TargetPosition_mm / _axis03mmPP);
                        targetPositionArrayPulse[i] = axis03TargetPosition_Pulse;
                        if (targetPositionArrayPulse[i] != axis03CurrentPosition_Pulse)
                            needRunFlag = true;
                        break;
                }
            }

            if (needRunFlag)
            {
                Dmc1000.d1000_start_ta_line(axisCount, axisNumberArray, targetPositionArrayPulse, startVelocityPPS, maxVelocityPPS, accTime);
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
                            Thread.Sleep(10);
                        }
                        _axis00Command = 0;
                        _axis00CommandSaved = 0;
                        break;
                    case 1:
                        axisDecelStop(1);
                        while (_axis01CheckDoneStatus == 0)
                        {
                            Thread.Sleep(10);
                        }
                        _axis01Command = 0;
                        _axis01CommandSaved = 0;
                        break;
                    case 2:
                        axisDecelStop(2);
                        while (_axis02CheckDoneStatus == 0)
                        {
                            Thread.Sleep(10);
                        }
                        _axis02Command = 0;
                        _axis02CommandSaved = 0;
                        break;
                    case 3:
                        axisDecelStop(3);
                        while (_axis03CheckDoneStatus == 0)
                        {
                            Thread.Sleep(10);
                        }
                        _axis03Command = 0;
                        _axis03CommandSaved = 0;
                        break;
                    case 4:
                        axisDecelStop(4);
                        while (_axis04CheckDoneStatus == 0)
                        {
                            Thread.Sleep(10);
                        }
                        _axis04Command = 0;
                        _axis04CommandSaved = 0;
                        break;
                    case 5:
                        axisDecelStop(5);
                        while (_axis05CheckDoneStatus == 0)
                        {
                            Thread.Sleep(10);
                        }
                        _axis05Command = 0;
                        _axis05CommandSaved = 0;
                        break;
                    case 6:
                        axisDecelStop(6);
                        while (_axis06CheckDoneStatus == 0)
                        {
                            Thread.Sleep(10);
                        }
                        _axis06Command = 0;
                        _axis06CommandSaved = 0;
                        break;
                    case 7:
                        axisDecelStop(7);
                        while (_axis07CheckDoneStatus == 0)
                        {
                            Thread.Sleep(10);
                        }
                        _axis07Command = 0;
                        _axis07CommandSaved = 0;
                        break;
                    case 8:
                        axisDecelStop(8);
                        while (_axis08CheckDoneStatus == 0)
                        {
                            Thread.Sleep(10);
                        }
                        _axis08Command = 0;
                        _axis08CommandSaved = 0;
                        break;
                    case 9:
                        axisDecelStop(9);
                        while (_axis09CheckDoneStatus == 0)
                        {
                            Thread.Sleep(10);
                        }
                        _axis09Command = 0;
                        _axis09CommandSaved = 0;
                        break;
                    case 10:
                        axisDecelStop(10);
                        while (_axis10CheckDoneStatus == 0)
                        {
                            Thread.Sleep(10);
                        }
                        _axis10Command = 0;
                        _axis10CommandSaved = 0;
                        break;
                    case 11:
                        axisDecelStop(11);
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

        #region axisDecelStop:轴减速停止
        /// <summary>
        /// axisDecelStop:轴减速停止
        /// </summary>
        /// <param name="axisNumber">int:轴号</param>
        public int axisDecelStop(int axisNumber)
        {
            return Dmc1000.d1000_decel_stop(axisNumber);
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
                            Thread.Sleep(10);
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
            return Dmc1000.d1000_immediate_stop(axisNumber);
        }
        #endregion

        #region homeAction:回原点
        private async void homeAction(int axisCommand, int axisNumber, float startVelocity, float maxVelocity, double accTime, float targetPosition_mm, bool jogDirection, bool inch, float deviationValue)
        {
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
                while (Dmc1000.d1000_check_done(axisNumber) == 0)//等待停止完成
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
                    while (Dmc1000.d1000_check_done(axisNumber) == 0)//等待停止完成
                    {
                        Thread.Sleep(10);
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
                    while (Dmc1000.d1000_check_done(axisNumber) == 0)//等待停止完成
                    {
                        Thread.Sleep(10);
                    }
                }
                //向正方向Inch移动固定距离
                AxisAction(901, axisNumber, startVelocity, axisHomeFindSensorVelocity, 0, accTime, 0, axisHomeBackDistance, true, true, 0, false, 0);
                while (Dmc1000.d1000_check_done(axisNumber) == 0)//等待Inch移动完成
                {
                    Thread.Sleep(10);
                }

                //回原点
                switch (axisNumber)
                {
                    case 0:
                        _axis00Command = 999;
                        _axis00HomedFlag = false;
                        axis00MaxVelocity_Temp_pps = Convert.ToInt32(maxVelocity / _axis00mmPP);
                        if (axis00MaxVelocity_Temp_pps > 0)
                            axis00MaxVelocity_Temp_pps = axis00MaxVelocity_Temp_pps * -1;
                        Dmc1000.d1000_home_move(axisNumber, axis00StartVelocity_Temp_pps, axis00MaxVelocity_Temp_pps, axis00TargetAccTime_s);
                        _axis00HomeStartFlag = true;
                        break;
                    case 1:
                        _axis01Command = 999;
                        _axis01HomedFlag = false;
                        axis01MaxVelocity_Temp_pps = Convert.ToInt32(maxVelocity / _axis01mmPP);
                        if (axis01MaxVelocity_Temp_pps > 0)
                            axis01MaxVelocity_Temp_pps = axis01MaxVelocity_Temp_pps * -1;
                        Dmc1000.d1000_home_move(axisNumber, axis01StartVelocity_Temp_pps, axis01MaxVelocity_Temp_pps, axis01TargetAccTime_s);
                        _axis01HomeStartFlag = true;
                        break;
                    case 2:
                        _axis02Command = 999;
                        _axis02HomedFlag = false;
                        axis02MaxVelocity_Temp_pps = Convert.ToInt32(maxVelocity / _axis02mmPP);
                        if (axis02MaxVelocity_Temp_pps > 0)
                            axis02MaxVelocity_Temp_pps = axis02MaxVelocity_Temp_pps * -1;
                        Dmc1000.d1000_home_move(axisNumber, axis02StartVelocity_Temp_pps, axis02MaxVelocity_Temp_pps, axis02TargetAccTime_s);
                        _axis02HomeStartFlag = true;
                        break;
                    case 3:
                        _axis03Command = 999;
                        _axis03HomedFlag = false;
                        axis03MaxVelocity_Temp_pps = Convert.ToInt32(maxVelocity / _axis03mmPP);
                        if (axis03MaxVelocity_Temp_pps > 0)
                            axis03MaxVelocity_Temp_pps = axis03MaxVelocity_Temp_pps * -1;
                        Dmc1000.d1000_home_move(axisNumber, axis03StartVelocity_Temp_pps, axis03MaxVelocity_Temp_pps, axis03TargetAccTime_s);
                        _axis03HomeStartFlag = true;
                        break;
                    case 4:
                        _axis04Command = 999;
                        _axis04HomedFlag = false;
                        axis04MaxVelocity_Temp_pps = Convert.ToInt32(maxVelocity / _axis04mmPP);
                        if (axis04MaxVelocity_Temp_pps > 0)
                            axis04MaxVelocity_Temp_pps = axis04MaxVelocity_Temp_pps * -1;
                        Dmc1000.d1000_home_move(axisNumber, axis04StartVelocity_Temp_pps, axis04MaxVelocity_Temp_pps, axis04TargetAccTime_s);
                        _axis04HomeStartFlag = true;
                        break;
                    case 5:
                        _axis05Command = 999;
                        _axis05HomedFlag = false;
                        axis05MaxVelocity_Temp_pps = Convert.ToInt32(maxVelocity / _axis05mmPP);
                        if (axis05MaxVelocity_Temp_pps > 0)
                            axis05MaxVelocity_Temp_pps = axis05MaxVelocity_Temp_pps * -1;
                        Dmc1000.d1000_home_move(axisNumber, axis05StartVelocity_Temp_pps, axis05MaxVelocity_Temp_pps, axis05TargetAccTime_s);
                        _axis05HomeStartFlag = true;
                        break;
                    case 6:
                        _axis06Command = 999;
                        _axis06HomedFlag = false;
                        axis06MaxVelocity_Temp_pps = Convert.ToInt32(maxVelocity / _axis06mmPP);
                        if (axis06MaxVelocity_Temp_pps > 0)
                            axis06MaxVelocity_Temp_pps = axis06MaxVelocity_Temp_pps * -1;
                        Dmc1000.d1000_home_move(axisNumber, axis06StartVelocity_Temp_pps, axis06MaxVelocity_Temp_pps, axis06TargetAccTime_s);
                        _axis06HomeStartFlag = true;
                        break;
                    case 7:
                        _axis07Command = 999;
                        _axis07HomedFlag = false;
                        axis07MaxVelocity_Temp_pps = Convert.ToInt32(maxVelocity / _axis07mmPP);
                        if (axis07MaxVelocity_Temp_pps > 0)
                            axis07MaxVelocity_Temp_pps = axis07MaxVelocity_Temp_pps * -1;
                        Dmc1000.d1000_home_move(axisNumber, axis07StartVelocity_Temp_pps, axis07MaxVelocity_Temp_pps, axis07TargetAccTime_s);
                        _axis07HomeStartFlag = true;
                        break;
                    case 8:
                        _axis08Command = 999;
                        _axis08HomedFlag = false;
                        axis08MaxVelocity_Temp_pps = Convert.ToInt32(maxVelocity / _axis08mmPP);
                        if (axis08MaxVelocity_Temp_pps > 0)
                            axis08MaxVelocity_Temp_pps = axis08MaxVelocity_Temp_pps * -1;
                        Dmc1000.d1000_home_move(axisNumber, axis08StartVelocity_Temp_pps, axis08MaxVelocity_Temp_pps, axis08TargetAccTime_s);
                        _axis08HomeStartFlag = true;
                        break;
                    case 9:
                        _axis09Command = 999;
                        _axis09HomedFlag = false;
                        axis09MaxVelocity_Temp_pps = Convert.ToInt32(maxVelocity / _axis09mmPP);
                        if (axis09MaxVelocity_Temp_pps > 0)
                            axis09MaxVelocity_Temp_pps = axis09MaxVelocity_Temp_pps * -1;
                        Dmc1000.d1000_home_move(axisNumber, axis09StartVelocity_Temp_pps, axis09MaxVelocity_Temp_pps, axis09TargetAccTime_s);
                        _axis09HomeStartFlag = true;
                        break;
                    case 10:
                        _axis10Command = 999;
                        _axis10HomedFlag = false;
                        axis10MaxVelocity_Temp_pps = Convert.ToInt32(maxVelocity / _axis10mmPP);
                        if (axis10MaxVelocity_Temp_pps > 0)
                            axis10MaxVelocity_Temp_pps = axis10MaxVelocity_Temp_pps * -1;
                        Dmc1000.d1000_home_move(axisNumber, axis10StartVelocity_Temp_pps, axis10MaxVelocity_Temp_pps, axis10TargetAccTime_s);
                        _axis10HomeStartFlag = true;
                        break;
                    case 11:
                        _axis11Command = 999;
                        _axis11HomedFlag = false;
                        axis11MaxVelocity_Temp_pps = Convert.ToInt32(maxVelocity / _axis11mmPP);
                        if (axis11MaxVelocity_Temp_pps > 0)
                            axis11MaxVelocity_Temp_pps = axis11MaxVelocity_Temp_pps * -1;
                        Dmc1000.d1000_home_move(axisNumber, axis11StartVelocity_Temp_pps, axis11MaxVelocity_Temp_pps, axis11TargetAccTime_s);
                        _axis11HomeStartFlag = true;
                        break;
                    default:
                        break;
                }

            });

        }
        #endregion

        #region OutputCtrl:输出点ON/OFF
        /// <summary>
        /// outPut:输出点ON/OFF
        /// </summary>
        /// <param name="outPutNo">int:输出点号码</param>
        /// <param name="onOrOff">bool:输出点ON/OFF设置</param>
        public int OutputCtrl(int outPutNo, bool onOrOff)
        {
            if (outPutNo >= 0 && outPutNo <= 26)
            {
                if (onOrOff)
                    return Dmc1000.d1000_out_bit(outPutNo + 1, 0);
                else
                    return Dmc1000.d1000_out_bit(outPutNo + 1, 1);
            }
            else if (outPutNo >= 64 && outPutNo <= 91)
            {
                if (onOrOff)
                    return Dmc1000.d1000_out_bit(outPutNo - 64 + 1 + 32, 0);
                else
                    return Dmc1000.d1000_out_bit(outPutNo - 64 + 1 + 32, 1);
            }
            return -1;

        }
        #endregion

        #region getOutPut:读取输出点状态
        /// <summary>
        /// outPut:getOutPut:读取输出点状态
        /// </summary>
        /// <param name="outPutNo">int:输出点号码</param>
        public bool getOutPut(int outPutNo)
        {
            if (Dmc1000.d1000_get_outbit(outPutNo) == 0)
                return true;
            else
                return false;
        }
        #endregion

        #region getInPut:读取输入点状态
        /// <summary>
        /// getInPut:读取输入点状态
        /// </summary>
        /// <param name="inPutNo">int:输入点号码</param>
        public bool getInPut(int inPutNo)
        {
            if (Dmc1000.d1000_in_bit(inPutNo) == 0)
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
                }

                //停止轴状态更新线程
                if (td_axisStatusUpdata != null)
                {
                    td_axisStatusUpdata.Abort();
                    td_axisStatusUpdata = null;
                }

                initialFlag = false;

                return Dmc1000.d1000_board_close();
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
