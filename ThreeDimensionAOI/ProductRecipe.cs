using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.IO;
using System.Xml;
using MyTool;
using System.Windows.Forms;
using CSVFile;

namespace ThreeDimensionAVI
{
    public class ProductRecipe
    {
        /*--------------------------------------------------------------------------------------
        //Copyright (C) 2022 深圳市利器精工科技有限公司
        //版权所有
        //
        //文件名：产品配方类
        //文件功能描述：用于产品参数保存读取类
        //
        //
        //创建标识：MaLi 20220331
        //
        //修改标识：MaLi 20220331 Change
        //修改描述：增加产品配方类
        //
        //------------------------------------------------------------------------------------*/

        //******************************事件************************************//

        //*************************外部可读写变量*******************************//
        public DataTable _pointPositionDT = null;//轴运行点位坐标DataTable

        public float _x1AxisLocationVelocity = 0f;//X1轴定位运行速度
        public float _y1AxisLocationVelocity = 0f;//Y1轴定位运行速度
        public float _r1AxisLocationVelocity = 0f;//R1轴定位运行速度
        public float _y2AxisLocationVelocity = 0f;//Y2轴定位运行速度
        public float _y3AxisLocationVelocity = 0f;//Y3轴定位运行速度
        public float _y4AxisLocationVelocity = 0f;//Y4轴定位运行速度
        public float _x1AxisLocationAccTime = 0f;//X1轴定位运行加减速时间
        public float _y1AxisLocationAccTime = 0f;//Y1轴定位运行加减速时间
        public float _r1AxisLocationAccTime = 0f;//R1轴定位运行加减速时间
        public float _y2AxisLocationAccTime = 0f;//Y2轴定位运行加减速时间
        public float _y3AxisLocationAccTime = 0f;//Y3轴定位运行加减速时间
        public float _y4AxisLocationAccTime = 0f;//Y4轴定位运行加减速时间

        public float _y2ConveyorPosition = 0f;//传送线Y2轴定位位置
        public float _y3ConveyorPosition = 0f;//传送线Y3轴定位位置
        public float _y4ConveyorPosition = 0f;//传送线Y4轴定位位置
        public float _conveyor1InHighSpeed = 0f;//传送线1流入高速
        public float _conveyor1InLowSpeed = 0f;//传送线1流入低速
        public float _conveyor1OutSpeed = 0f;//传送线1流出速度
        public float _conveyor2InHighSpeed = 0f;//传送线2流入高速
        public float _conveyor2InLowSpeed = 0f;//传送线2流入低速
        public float _conveyor2OutSpeed = 0f;//传送线2流出速度

        public float _conveyor1LoadFinishTime = 0f;//传送线1上料完成延时时间
        public float _conveyor1UnloadFinishTime = 0f;//传送线1下料完成延时时间
        public float _conveyor1StopCylinderUpTime = 0f;//传送线1下料时阻挡气缸上升延时时间
        public float _conveyor1LoadTimeOutTime = 0f;//传送线1上料超时延时时间
        public float _conveyor1UnloadTimeOutTime = 0f;//传送线1下料超时延时时间

        public float _conveyor2LoadFinishTime = 0f;//传送线2上料完成延时时间
        public float _conveyor2UnloadFinishTime = 0f;//传送线2下料完成延时时间
        public float _conveyor2StopCylinderUpTime = 0f;//传送线2下料时阻挡气缸上升延时时间
        public float _conveyor2LoadTimeOutTime = 0f;//传送线2上料超时延时时间
        public float _conveyor2UnloadTimeOutTime = 0f;//传送线2下料超时延时时间

        public int _track1TeachedPointNo = 1;//传送线1已示教的基准点位号
        public float _track1X1TeachedCoor = 0f;//传送线1 X1轴示教坐标
        public float _track1Y1TeachedCoor = 0f;//传送线1 Y1轴示教坐标
        public float _track1R1TeachedCoor = 0f;//传送线1 R1轴示教坐标

        public int _track2TeachedPointNo = 1;//传送线2已示教的基准点位号
        public float _track2X1TeachedCoor = 0f;//传送线2 X1轴示教坐标
        public float _track2Y1TeachedCoor = 0f;//传送线2 Y1轴示教坐标
        public float _track2R1TeachedCoor = 0f;//传送线2 R1轴示教坐标

        public string _preprocessParameterFilePath = "";//图像推断预处理参数文件路径
        public string _dimensionFilePath = "";//图像推断标注文件路径
        public string _modelFilePath = "";//图像推断模型文件路径

        public bool _parameterLoadSuccessfulFlag = false;//参数读取成功标志，false-失败，true-成功

        public string _productNameUseForMES = "";//当前产品用于MES传输数据的名称

        public int _markSearchThresholdMin = 0;//mark点识别二值化最小值
        public int _markSearchThresholdMax = 255;//mark点识别二值化最大值
        public float _markSelectShapeMin = 0f;//mark点识别面积选择最小值
        public float _markSelectShapeMax = 0f;//mark点识别面积选择最大值

        public float _conveyor1Mark1SearchRowResult = 0f;//识别的传送线1Mark点1的Row值
        public float _conveyor1Mark1SearchColumnResult = 0f;//识别的传送线1Mark点1的Column值
        public float _conveyor1Mark2SearchRowResult = 0f;//识别的传送线1Mark点2的Row值
        public float _conveyor1Mark2SearchColumnResult = 0f;//识别的传送线1Mark点2的Columnn值

        public float _conveyor2Mark1SearchRowResult = 0f;//识别的传送线2Mark点1的Row值
        public float _conveyor2Mark1SearchColumnResult = 0f;//识别的传送线2Mark点1的Column值
        public float _conveyor2Mark2SearchRowResult = 0f;//识别的传送线2Mark点2的Row值
        public float _conveyor2Mark2SearchColumnResult = 0f;//识别的传送线2Mark点2的Column值

        public string _shieldProduct1Barcode = "";//屏蔽的产品的二维码数据1
        public string _shieldProduct2Barcode = "";//屏蔽的产品的二维码数据2
        public string _shieldProduct3Barcode = "";//屏蔽的产品的二维码数据3

        public float _markSearchMinCircularity = 0f;//mark点识别最小圆度

        public string _sampleProduct1Barcode = "";//样品板的二维码数据1
        public string _sampleProduct2Barcode = "";//样品板的二维码数据1
        public string _sampleProduct3Barcode = "";//样品板的二维码数据1

        //深度学习推断结果OK判定条件参数
        public int _inferResultKind1Number = 0;
        public int _inferResultKind2Number = 0;
        public int _inferResultKind3Number = 0;
        public int _inferResultKind4Number = 0;
        public int _inferResultKind5Number = 0;

        public int _inferResultKind1Quantity = 0;
        public int _inferResultKind2Quantity = 0;
        public int _inferResultKind3Quantity = 0;
        public int _inferResultKind4Quantity = 0;
        public int _inferResultKind5Quantity = 0;

        //*************************公共静态变量*******************************//

        //*************************内部私有变量*******************************//
        const int LOAD_SAVE_DATA_ELEMENT_QUANTITY = 72;
        //用于存储需要保存读取的参数变量名和xml名称对应数组,0-xml文件名称,2-控件对应的变量名称,4-控件对应的变量的类型
        string[,] _ctrlNameAndXmlFileNameStr = new string[3, LOAD_SAVE_DATA_ELEMENT_QUANTITY];
        //
        //
        //
        //

        #region InitialCtrlBoxCorrespondingStringArr:初始化变量名和xml文件保存名称对应表
        void InitialCtrlBoxCorrespondingStringArr()
        {
            MyTool.TxtFileProcess.CreateLog("产品配方类-初始化变量名和xml文件保存名称对应表" + "----用户：" + BaseForm._operatorName,
            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

            #region 在XML中保存读取的参数名称
            //general parameter
            _ctrlNameAndXmlFileNameStr[0, 0] = "X1AxisLocationVelocity";
            _ctrlNameAndXmlFileNameStr[0, 1] = "Y1AxisLocationVelocity";
            _ctrlNameAndXmlFileNameStr[0, 2] = "R1AxisLocationVelocity";
            _ctrlNameAndXmlFileNameStr[0, 3] = "Y2AxisLocationVelocity";
            _ctrlNameAndXmlFileNameStr[0, 4] = "Y3AxisLocationVelocity";
            _ctrlNameAndXmlFileNameStr[0, 5] = "Y4AxisLocationVelocity";
            _ctrlNameAndXmlFileNameStr[0, 6] = "X1AxisLocationAccTime";
            _ctrlNameAndXmlFileNameStr[0, 7] = "Y1AxisLocationAccTime";
            _ctrlNameAndXmlFileNameStr[0, 8] = "R1AxisLocationAccTime";
            _ctrlNameAndXmlFileNameStr[0, 9] = "Y2AxisLocationAccTime";
            _ctrlNameAndXmlFileNameStr[0, 10] = "Y3AxisLocationAccTime";
            _ctrlNameAndXmlFileNameStr[0, 11] = "Y4AxisLocationAccTime";
            //conveyor
            _ctrlNameAndXmlFileNameStr[0, 12] = "Y2Position";
            _ctrlNameAndXmlFileNameStr[0, 13] = "Y3Position";
            _ctrlNameAndXmlFileNameStr[0, 14] = "Y4Position";
            _ctrlNameAndXmlFileNameStr[0, 15] = "Conveyor1InHighSpeed";
            _ctrlNameAndXmlFileNameStr[0, 16] = "Conveyor1InLowSpeed";
            _ctrlNameAndXmlFileNameStr[0, 17] = "Conveyor1OutSpeed";
            _ctrlNameAndXmlFileNameStr[0, 18] = "Conveyor2InHighSpeed";
            _ctrlNameAndXmlFileNameStr[0, 19] = "Conveyor2InLowSpeed";
            _ctrlNameAndXmlFileNameStr[0, 20] = "Conveyor2OutSpeed";

            _ctrlNameAndXmlFileNameStr[0, 21] = "Conveyor1LoadFinishTime";
            _ctrlNameAndXmlFileNameStr[0, 22] = "Conveyor1UnloadFinishTime";
            _ctrlNameAndXmlFileNameStr[0, 23] = "Conveyor1LoadTimeOutTime";
            _ctrlNameAndXmlFileNameStr[0, 24] = "Conveyor1UnloadTimeOutTime";
            _ctrlNameAndXmlFileNameStr[0, 25] = "Conveyor2LoadFinishTime";
            _ctrlNameAndXmlFileNameStr[0, 26] = "Conveyor2UnloadFinishTime";
            _ctrlNameAndXmlFileNameStr[0, 27] = "Conveyor2LoadTimeOutTime";
            _ctrlNameAndXmlFileNameStr[0, 28] = "Conveyor2UnloadTimeOutTime";

            _ctrlNameAndXmlFileNameStr[0, 29] = "Conveyor1StopCylinderUpTime";
            _ctrlNameAndXmlFileNameStr[0, 30] = "Conveyor2StopCylinderUpTime";
            //point position
            _ctrlNameAndXmlFileNameStr[0, 31] = "Track1TeachedPointNo";
            _ctrlNameAndXmlFileNameStr[0, 32] = "Track1X1TeachedCoor";
            _ctrlNameAndXmlFileNameStr[0, 33] = "Track1Y1TeachedCoor";
            _ctrlNameAndXmlFileNameStr[0, 34] = "Track1R1TeachedCoor";

            _ctrlNameAndXmlFileNameStr[0, 35] = "Track2TeachedPointNo";
            _ctrlNameAndXmlFileNameStr[0, 36] = "Track2X1TeachedCoor";
            _ctrlNameAndXmlFileNameStr[0, 37] = "Track2Y1TeachedCoor";
            _ctrlNameAndXmlFileNameStr[0, 38] = "Track2R1TeachedCoor";
            //Deep learning
            _ctrlNameAndXmlFileNameStr[0, 39] = "PreprocessParameterFilePath";
            _ctrlNameAndXmlFileNameStr[0, 40] = "DimensionFilePath";
            _ctrlNameAndXmlFileNameStr[0, 41] = "ModelFilePath";

            _ctrlNameAndXmlFileNameStr[0, 42] = "ProductNameUseForMES";

            //mark search
            _ctrlNameAndXmlFileNameStr[0, 43] = "MarkSearchThresholdMin";
            _ctrlNameAndXmlFileNameStr[0, 44] = "MarkSearchThresholdMax";
            _ctrlNameAndXmlFileNameStr[0, 45] = "MarkSelectShapeMin";
            _ctrlNameAndXmlFileNameStr[0, 46] = "MarkSelectShapeMax";

            _ctrlNameAndXmlFileNameStr[0, 47] = "Conveyor1Mark1SearchRowResult";
            _ctrlNameAndXmlFileNameStr[0, 48] = "Conveyor1Mark1SearchColumnResult";
            _ctrlNameAndXmlFileNameStr[0, 49] = "Conveyor1Mark2SearchRowResult";
            _ctrlNameAndXmlFileNameStr[0, 50] = "Conveyor1Mark2SearchColumnResult";

            _ctrlNameAndXmlFileNameStr[0, 51] = "Conveyor2Mark1SearchRowResult";
            _ctrlNameAndXmlFileNameStr[0, 52] = "Conveyor2Mark1SearchColumnResult";
            _ctrlNameAndXmlFileNameStr[0, 53] = "Conveyor2Mark2SearchRowResult";
            _ctrlNameAndXmlFileNameStr[0, 54] = "Conveyor2Mark2SearchColumnResult";

            _ctrlNameAndXmlFileNameStr[0, 55] = "ShieldProduct1Barcode";
            _ctrlNameAndXmlFileNameStr[0, 56] = "ShieldProduct2Barcode";
            _ctrlNameAndXmlFileNameStr[0, 57] = "ShieldProduct3Barcode";

            _ctrlNameAndXmlFileNameStr[0, 58] = "MarkSearchMinCircularity";

            _ctrlNameAndXmlFileNameStr[0, 59] = "SampleProduct1Barcode";
            _ctrlNameAndXmlFileNameStr[0, 60] = "SampleProduct2Barcode";
            _ctrlNameAndXmlFileNameStr[0, 61] = "SampleProduct3Barcode";

            //深度学习推断结果OK判定条件参数
            _ctrlNameAndXmlFileNameStr[0, 62] = "InferResultKind1Number";
            _ctrlNameAndXmlFileNameStr[0, 63] = "InferResultKind2Number";
            _ctrlNameAndXmlFileNameStr[0, 64] = "InferResultKind3Number";
            _ctrlNameAndXmlFileNameStr[0, 65] = "InferResultKind4Number";
            _ctrlNameAndXmlFileNameStr[0, 66] = "InferResultKind5Number";

            _ctrlNameAndXmlFileNameStr[0, 67] = "InferResultKind1Quantity";
            _ctrlNameAndXmlFileNameStr[0, 68] = "InferResultKind2Quantity";
            _ctrlNameAndXmlFileNameStr[0, 69] = "InferResultKind3Quantity";
            _ctrlNameAndXmlFileNameStr[0, 70] = "InferResultKind4Quantity";
            _ctrlNameAndXmlFileNameStr[0, 71] = "InferResultKind5Quantity";

            #endregion

            #region 控件对应的ActualProduct变量名称
            //general parameter
            _ctrlNameAndXmlFileNameStr[1, 0] = "_x1AxisLocationVelocity";
            _ctrlNameAndXmlFileNameStr[1, 1] = "_y1AxisLocationVelocity";
            _ctrlNameAndXmlFileNameStr[1, 2] = "_r1AxisLocationVelocity";
            _ctrlNameAndXmlFileNameStr[1, 3] = "_y2AxisLocationVelocity";
            _ctrlNameAndXmlFileNameStr[1, 4] = "_y3AxisLocationVelocity";
            _ctrlNameAndXmlFileNameStr[1, 5] = "_y4AxisLocationVelocity";
            _ctrlNameAndXmlFileNameStr[1, 6] = "_x1AxisLocationAccTime";
            _ctrlNameAndXmlFileNameStr[1, 7] = "_y1AxisLocationAccTime";
            _ctrlNameAndXmlFileNameStr[1, 8] = "_r1AxisLocationAccTime";
            _ctrlNameAndXmlFileNameStr[1, 9] = "_y2AxisLocationAccTime";
            _ctrlNameAndXmlFileNameStr[1, 10] = "_y3AxisLocationAccTime";
            _ctrlNameAndXmlFileNameStr[1, 11] = "_y4AxisLocationAccTime";
            //conveyor parameter
            _ctrlNameAndXmlFileNameStr[1, 12] = "_y2ConveyorPosition";
            _ctrlNameAndXmlFileNameStr[1, 13] = "_y3ConveyorPosition";
            _ctrlNameAndXmlFileNameStr[1, 14] = "_y4ConveyorPosition";
            _ctrlNameAndXmlFileNameStr[1, 15] = "_conveyor1InHighSpeed";
            _ctrlNameAndXmlFileNameStr[1, 16] = "_conveyor1InLowSpeed";
            _ctrlNameAndXmlFileNameStr[1, 17] = "_conveyor1OutSpeed";
            _ctrlNameAndXmlFileNameStr[1, 18] = "_conveyor2InHighSpeed";
            _ctrlNameAndXmlFileNameStr[1, 19] = "_conveyor2InLowSpeed";
            _ctrlNameAndXmlFileNameStr[1, 20] = "_conveyor2OutSpeed";

            _ctrlNameAndXmlFileNameStr[1, 21] = "_conveyor1LoadFinishTime";
            _ctrlNameAndXmlFileNameStr[1, 22] = "_conveyor1UnloadFinishTime";
            _ctrlNameAndXmlFileNameStr[1, 23] = "_conveyor1LoadTimeOutTime";
            _ctrlNameAndXmlFileNameStr[1, 24] = "_conveyor1UnloadTimeOutTime";
            _ctrlNameAndXmlFileNameStr[1, 25] = "_conveyor2LoadFinishTime";
            _ctrlNameAndXmlFileNameStr[1, 26] = "_conveyor2UnloadFinishTime";
            _ctrlNameAndXmlFileNameStr[1, 27] = "_conveyor2LoadTimeOutTime";
            _ctrlNameAndXmlFileNameStr[1, 28] = "_conveyor2UnloadTimeOutTime";
            _ctrlNameAndXmlFileNameStr[1, 29] = "_conveyor1StopCylinderUpTime";
            _ctrlNameAndXmlFileNameStr[1, 30] = "_conveyor2StopCylinderUpTime";
            
            //point position
            _ctrlNameAndXmlFileNameStr[1, 31] = "_track1TeachedPointNo";
            _ctrlNameAndXmlFileNameStr[1, 32] = "_track1X1TeachedCoor";
            _ctrlNameAndXmlFileNameStr[1, 33] = "_track1Y1TeachedCoor";
            _ctrlNameAndXmlFileNameStr[1, 34] = "_track1R1TeachedCoor";

            _ctrlNameAndXmlFileNameStr[1, 35] = "_track2TeachedPointNo";
            _ctrlNameAndXmlFileNameStr[1, 36] = "_track2X1TeachedCoor";
            _ctrlNameAndXmlFileNameStr[1, 37] = "_track2Y1TeachedCoor";
            _ctrlNameAndXmlFileNameStr[1, 38] = "_track2R1TeachedCoor";

            //Deep learning
            _ctrlNameAndXmlFileNameStr[1, 39] = "_preprocessParameterFilePath";
            _ctrlNameAndXmlFileNameStr[1, 40] = "_dimensionFilePath";
            _ctrlNameAndXmlFileNameStr[1, 41] = "_modelFilePath";

            _ctrlNameAndXmlFileNameStr[1, 42] = "_productNameUseForMES";


            //mark search
            _ctrlNameAndXmlFileNameStr[1, 43] = "_markSearchThresholdMin";
            _ctrlNameAndXmlFileNameStr[1, 44] = "_markSearchThresholdMax";
            _ctrlNameAndXmlFileNameStr[1, 45] = "_markSelectShapeMin";
            _ctrlNameAndXmlFileNameStr[1, 46] = "_markSelectShapeMax";

            _ctrlNameAndXmlFileNameStr[1, 47] = "_conveyor1Mark1SearchRowResult";
            _ctrlNameAndXmlFileNameStr[1, 48] = "_conveyor1Mark1SearchColumnResult";
            _ctrlNameAndXmlFileNameStr[1, 49] = "_conveyor1Mark2SearchRowResult";
            _ctrlNameAndXmlFileNameStr[1, 50] = "_conveyor1Mark2SearchColumnResult";

            _ctrlNameAndXmlFileNameStr[1, 51] = "_conveyor2Mark1SearchRowResult";
            _ctrlNameAndXmlFileNameStr[1, 52] = "_conveyor2Mark1SearchColumnResult";
            _ctrlNameAndXmlFileNameStr[1, 53] = "_conveyor2Mark2SearchRowResult";
            _ctrlNameAndXmlFileNameStr[1, 54] = "_conveyor2Mark2SearchColumnResult";

            _ctrlNameAndXmlFileNameStr[1, 55] = "_shieldProduct1Barcode";
            _ctrlNameAndXmlFileNameStr[1, 56] = "_shieldProduct2Barcode";
            _ctrlNameAndXmlFileNameStr[1, 57] = "_shieldProduct3Barcode";

            _ctrlNameAndXmlFileNameStr[1, 58] = "_markSearchMinCircularity";

            _ctrlNameAndXmlFileNameStr[1, 59] = "_sampleProduct1Barcode";
            _ctrlNameAndXmlFileNameStr[1, 60] = "_sampleProduct2Barcode";
            _ctrlNameAndXmlFileNameStr[1, 61] = "_sampleProduct3Barcode";

             //深度学习推断结果OK判定条件参数
            _ctrlNameAndXmlFileNameStr[1, 62] = "_inferResultKind1Number";
            _ctrlNameAndXmlFileNameStr[1, 63] = "_inferResultKind2Number";
            _ctrlNameAndXmlFileNameStr[1, 64] = "_inferResultKind3Number";
            _ctrlNameAndXmlFileNameStr[1, 65] = "_inferResultKind4Number";
            _ctrlNameAndXmlFileNameStr[1, 66] = "_inferResultKind5Number";

            _ctrlNameAndXmlFileNameStr[1, 67] = "_inferResultKind1Quantity";
            _ctrlNameAndXmlFileNameStr[1, 68] = "_inferResultKind2Quantity";
            _ctrlNameAndXmlFileNameStr[1, 69] = "_inferResultKind3Quantity";
            _ctrlNameAndXmlFileNameStr[1, 70] = "_inferResultKind4Quantity";
            _ctrlNameAndXmlFileNameStr[1, 71] = "_inferResultKind5Quantity";
            #endregion

            #region 控件对应的变量类型
            //general parameter
            _ctrlNameAndXmlFileNameStr[2, 0] = "float";
            _ctrlNameAndXmlFileNameStr[2, 1] = "float";
            _ctrlNameAndXmlFileNameStr[2, 2] = "float";
            _ctrlNameAndXmlFileNameStr[2, 3] = "float";
            _ctrlNameAndXmlFileNameStr[2, 4] = "float";
            _ctrlNameAndXmlFileNameStr[2, 5] = "float";
            _ctrlNameAndXmlFileNameStr[2, 6] = "float";
            _ctrlNameAndXmlFileNameStr[2, 7] = "float";
            _ctrlNameAndXmlFileNameStr[2, 8] = "float";
            _ctrlNameAndXmlFileNameStr[2, 9] = "float";
            _ctrlNameAndXmlFileNameStr[2, 10] = "float";
            _ctrlNameAndXmlFileNameStr[2, 11] = "float";
            //conveyor parameter
            _ctrlNameAndXmlFileNameStr[2, 12] = "float";
            _ctrlNameAndXmlFileNameStr[2, 13] = "float";
            _ctrlNameAndXmlFileNameStr[2, 14] = "float";
            _ctrlNameAndXmlFileNameStr[2, 15] = "float";
            _ctrlNameAndXmlFileNameStr[2, 16] = "float";
            _ctrlNameAndXmlFileNameStr[2, 17] = "float";
            _ctrlNameAndXmlFileNameStr[2, 18] = "float";
            _ctrlNameAndXmlFileNameStr[2, 19] = "float";
            _ctrlNameAndXmlFileNameStr[2, 20] = "float";

            _ctrlNameAndXmlFileNameStr[2, 21] = "float";
            _ctrlNameAndXmlFileNameStr[2, 22] = "float";
            _ctrlNameAndXmlFileNameStr[2, 23] = "float";
            _ctrlNameAndXmlFileNameStr[2, 24] = "float";
            _ctrlNameAndXmlFileNameStr[2, 25] = "float";
            _ctrlNameAndXmlFileNameStr[2, 26] = "float";
            _ctrlNameAndXmlFileNameStr[2, 27] = "float";
            _ctrlNameAndXmlFileNameStr[2, 28] = "float";
            _ctrlNameAndXmlFileNameStr[2, 29] = "float";
            _ctrlNameAndXmlFileNameStr[2, 30] = "float";
            //point position
            _ctrlNameAndXmlFileNameStr[2, 31] = "int";
            _ctrlNameAndXmlFileNameStr[2, 32] = "float";
            _ctrlNameAndXmlFileNameStr[2, 33] = "float";
            _ctrlNameAndXmlFileNameStr[2, 34] = "float";

            _ctrlNameAndXmlFileNameStr[2, 35] = "int";
            _ctrlNameAndXmlFileNameStr[2, 36] = "float";
            _ctrlNameAndXmlFileNameStr[2, 37] = "float";
            _ctrlNameAndXmlFileNameStr[2, 38] = "float";

            //Deep learning
            _ctrlNameAndXmlFileNameStr[2, 39] = "string";
            _ctrlNameAndXmlFileNameStr[2, 40] = "string";
            _ctrlNameAndXmlFileNameStr[2, 41] = "string";
            _ctrlNameAndXmlFileNameStr[2, 42] = "string";

            //mark search
            _ctrlNameAndXmlFileNameStr[2, 43] = "int";
            _ctrlNameAndXmlFileNameStr[2, 44] = "int";
            _ctrlNameAndXmlFileNameStr[2, 45] = "float";
            _ctrlNameAndXmlFileNameStr[2, 46] = "float";

            _ctrlNameAndXmlFileNameStr[2, 47] = "float";
            _ctrlNameAndXmlFileNameStr[2, 48] = "float";
            _ctrlNameAndXmlFileNameStr[2, 49] = "float";
            _ctrlNameAndXmlFileNameStr[2, 50] = "float";

            _ctrlNameAndXmlFileNameStr[2, 51] = "float";
            _ctrlNameAndXmlFileNameStr[2, 52] = "float";
            _ctrlNameAndXmlFileNameStr[2, 53] = "float";
            _ctrlNameAndXmlFileNameStr[2, 54] = "float";

            _ctrlNameAndXmlFileNameStr[2, 55] = "string";
            _ctrlNameAndXmlFileNameStr[2, 56] = "string";
            _ctrlNameAndXmlFileNameStr[2, 57] = "string";

            _ctrlNameAndXmlFileNameStr[2, 58] = "float";

            _ctrlNameAndXmlFileNameStr[2, 59] = "string";
            _ctrlNameAndXmlFileNameStr[2, 60] = "string";
            _ctrlNameAndXmlFileNameStr[2, 61] = "string";

            //深度学习推断结果OK判定条件参数
            _ctrlNameAndXmlFileNameStr[2, 62] = "int";
            _ctrlNameAndXmlFileNameStr[2, 63] = "int";
            _ctrlNameAndXmlFileNameStr[2, 64] = "int";
            _ctrlNameAndXmlFileNameStr[2, 65] = "int";
            _ctrlNameAndXmlFileNameStr[2, 66] = "int";

            _ctrlNameAndXmlFileNameStr[2, 67] = "int";
            _ctrlNameAndXmlFileNameStr[2, 68] = "int";
            _ctrlNameAndXmlFileNameStr[2, 69] = "int";
            _ctrlNameAndXmlFileNameStr[2, 70] = "int";
            _ctrlNameAndXmlFileNameStr[2, 71] = "int";
            #endregion
        }
        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="filePath">string:需要读取的rcp文件全路径</param>
        public ProductRecipe()
        {
            InitialCtrlBoxCorrespondingStringArr();
        }

        /// <summary>
        /// ProductDataLoadToVariate:读取数据至变量中
        /// </summary>
        /// <param name="FilePath">string:文件路径</param>
        /// <returns>bool: true-读取成功，false-读取失败</returns>
        public bool ProductDataLoadToVariate(string filePath)
        {
            MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据至变量" + "----用户：" + BaseForm._operatorName,
            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

            string tempValue;
            bool loadResult = true;
            string rootName = "clsRecipeData";

            _parameterLoadSuccessfulFlag = false;
            if (!File.Exists(filePath))
            {
                MessageBox.Show("指定的读取保存产品参数的文件不存在，无法读取数据至变量!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                loadResult = false;
                return loadResult;
            }
            else
            {
                string[] tempNodeName = new string[LOAD_SAVE_DATA_ELEMENT_QUANTITY];
                int loadUsefulNodeQuantity = 0;
                bool[] dataLoadReultFlag = new bool[LOAD_SAVE_DATA_ELEMENT_QUANTITY];//用于判定是否读取数据OK，false-读取失败，true-读取成功

                for (int i = 0; i < LOAD_SAVE_DATA_ELEMENT_QUANTITY; i++)
                {
                    tempNodeName[i] = _ctrlNameAndXmlFileNameStr[0, i];
                    dataLoadReultFlag[i] = false;
                }

                XmlDocument xmlDoc = new XmlDocument();
                try
                {
                    xmlDoc.Load(filePath);
                }
                catch
                {
                    //加入XML的声明段落
                    xmlDoc = new XmlDocument();
                    XmlNode xmlNode = xmlDoc.CreateNode(XmlNodeType.XmlDeclaration, "", "");
                    xmlDoc.AppendChild(xmlNode);
                    //加入一个根元素
                    XmlElement xmlElem = xmlDoc.CreateElement("", rootName, "");
                    xmlDoc.AppendChild(xmlElem);

                    //修改根节点的属性
                    XmlNode node = xmlDoc.SelectSingleNode(rootName);
                    XmlElement xmlEle = (XmlElement)node;
                    if ("xmlns:xsi".Equals(""))
                        xmlEle.InnerText = "http://www.w3.org/2001/XMLSchema-instance";
                    else
                        xmlEle.SetAttribute("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");

                    if ("xmlns:xsd".Equals(""))
                        xmlEle.InnerText = "http://www.w3.org/2001/XMLSchema";
                    else
                        xmlEle.SetAttribute("xmlns:xsd", "http://www.w3.org/2001/XMLSchema");
                    xmlDoc.Save(filePath);
                    xmlDoc.Load(filePath);
                }

                
                XmlNodeList topM = xmlDoc.DocumentElement.ChildNodes;
                foreach (XmlElement element in topM)
                {
                    int index = Array.IndexOf(tempNodeName, element.Name.ToString());
                    if (index != -1)
                    {
                        if (_ctrlNameAndXmlFileNameStr[2, index] != "bool")
                        {
                            MyTool.DataProcessing.SetDataToOneVariateBaseVariateStringName(_ctrlNameAndXmlFileNameStr[1, index], _ctrlNameAndXmlFileNameStr[2, index], this, element.InnerText);
                        }
                        else if (_ctrlNameAndXmlFileNameStr[2, index] == "bool")
                        {
                            tempValue = element.InnerText.ToString().ToLower();
                            if (tempValue == "true")
                            {
                                MyTool.DataProcessing.SetDataToOneVariateBaseVariateStringName(_ctrlNameAndXmlFileNameStr[1, index], _ctrlNameAndXmlFileNameStr[2, index], this, "1");
                            }
                            else
                            {
                                MyTool.DataProcessing.SetDataToOneVariateBaseVariateStringName(_ctrlNameAndXmlFileNameStr[1, index], _ctrlNameAndXmlFileNameStr[2, index], this, "0");
                            }
                        }
                        else
                        {
                            continue;
                        }
                        loadUsefulNodeQuantity++;
                        dataLoadReultFlag[index] = true;
                    }
                }

               // xmlDoc.Save(filePath);
                xmlDoc = null;
                if (loadUsefulNodeQuantity < LOAD_SAVE_DATA_ELEMENT_QUANTITY)
                {
                    for (int i = 0; i < LOAD_SAVE_DATA_ELEMENT_QUANTITY; i++)
                    {
                        if ((!SettingsAdministratorForm._checkProductIsConnector && (!(i >= 62 && i <= 71))) ||
                            SettingsAdministratorForm._checkProductIsConnector)
                        {
                            if (!dataLoadReultFlag[i])
                            {
                                MessageBox.Show("在产品参数文件" + filePath.Substring(filePath.LastIndexOf("\\") + 1, filePath.Length - filePath.LastIndexOf("\\") - 1) + "中，无法找到参数" + _ctrlNameAndXmlFileNameStr[1, i] + "的数据，读取产品参数至变量中失败！");
                                loadResult = false;
                            }
                        }
                    }
                }
            }

            if (!LoadPointPositionData(filePath.Substring(0, filePath.LastIndexOf(".")) + ".csv"))
            {
                loadResult = false;
            }

            if (!LoadProductDataCheck())
            {
                loadResult = false;
            }

            _parameterLoadSuccessfulFlag = loadResult;

            return _parameterLoadSuccessfulFlag;
        }

        /// <summary>
        /// LoadProductDataCheck:获取的参数检查
        /// </summary>
        /// <returns></returns>
        private bool LoadProductDataCheck()
        {
            MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查" + "----用户：" + BaseForm._operatorName,
            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

            bool dataCheckResult = true;//false:Check failed,true:check successful

            if (_track1TeachedPointNo > _pointPositionDT.Rows.Count)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，作为传送线1示教基准的点位号大于产品所有点位个数！" + "----用户：" + BaseForm._operatorName,
            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                MessageBox.Show("读取的数据中，作为传送线1示教基准的点位号大于产品所有点位个数！");
                dataCheckResult = false;
            }

            if (_track2TeachedPointNo > _pointPositionDT.Rows.Count)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，作为传送线2示教基准的点位号大于产品所有点位个数！" + "----用户：" + BaseForm._operatorName,
            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                MessageBox.Show("读取的数据中，作为传送线2示教基准的点位号大于产品所有点位个数！");
                dataCheckResult = false;
            }

            if (!File.Exists(_preprocessParameterFilePath))
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，图像推断所用预处理参数文件不存在！" + "----用户：" + BaseForm._operatorName,
            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                MessageBox.Show("读取的数据中，图像推断所用预处理参数文件不存在！");
                dataCheckResult = false;
            }

            if (!File.Exists(_dimensionFilePath))
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，图像推断所用标注文件不存在！" + "----用户：" + BaseForm._operatorName,
            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                MessageBox.Show("读取的数据中，图像推断所用标注文件不存在！");
                dataCheckResult = false;
            }

            if (!File.Exists(_modelFilePath))
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，图像推断所用模型文件不存在！" + "----用户：" + BaseForm._operatorName,
            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                MessageBox.Show("读取的数据中，图像推断所用模型文件不存在！");
                dataCheckResult = false;
            }

            if (_markSearchThresholdMin < 0 || _markSearchThresholdMin > 255)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，mark点识别二值化最小值范围有误，为" + _markSearchThresholdMin.ToString() + "----用户：" + BaseForm._operatorName,
                BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                MessageBox.Show("读取的数据中，mark点识别二值化最小值范围有误(应当大于0小于255)！");
                dataCheckResult = false;
            }

            if (_markSearchThresholdMax < 0 || _markSearchThresholdMax > 255)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，mark点识别二值化最小大值范围有误，为" + _markSearchThresholdMax.ToString() + "----用户：" + BaseForm._operatorName,
                BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                MessageBox.Show("读取的数据中，mark点识别二值化最大值范围有误(应当大于0小于255)！");
                dataCheckResult = false;
            }

            if (_markSelectShapeMin < 0)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，mark点识别region特征选择范围下限值有误，为" + _markSelectShapeMin.ToString() + "----用户：" + BaseForm._operatorName,
                BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                MessageBox.Show("读取的数据中，mark点识别region特征选择范围下限值有误(应当大于0)！");
                dataCheckResult = false;
            }

            if (_markSelectShapeMax < 0)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，mark点识别region特征选择范围上限值有误，为" + _markSelectShapeMax.ToString() + "----用户：" + BaseForm._operatorName,
                BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                MessageBox.Show("读取的数据中，mark点识别region特征选择范围上限值有误(应当大于0)！");
                dataCheckResult = false;
            }

            if (_conveyor1Mark1SearchRowResult < 0 || _conveyor1Mark1SearchColumnResult < 0 ||
                _conveyor1Mark2SearchRowResult < 0 || _conveyor1Mark2SearchColumnResult < 0 ||
                _conveyor2Mark1SearchRowResult < 0 || _conveyor2Mark1SearchColumnResult < 0 ||
                _conveyor2Mark2SearchRowResult < 0 || _conveyor2Mark2SearchColumnResult < 0)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，mark点识别结果有误，所有值应当大于0！" + "----用户：" + BaseForm._operatorName,
                BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                MessageBox.Show("读取的数据中，mark点识别结果有误，所有值应当大于0！");
                dataCheckResult = false;
            }

            if (_markSearchMinCircularity < 0f || _markSearchMinCircularity > 1f)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，mark点识别最小圆度值有误，值应当大于等于0小于等于1！" + "----用户：" + BaseForm._operatorName,
                BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                MessageBox.Show("读取的数据中，mark点识别最小圆度值有误，值应当大于等于0小于等于1！");
                dataCheckResult = false;
            }

            if ((_inferResultKind1Number < 0 || _inferResultKind1Quantity < 0 ||
                 _inferResultKind2Number < 0 || _inferResultKind2Quantity < 0 ||
                 _inferResultKind3Number < 0 || _inferResultKind3Quantity < 0 ||
                 _inferResultKind4Number < 0 || _inferResultKind4Quantity < 0 ||
                 _inferResultKind5Number < 0 || _inferResultKind5Quantity < 0) && SettingsAdministratorForm._checkProductIsConnector)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，AOI推断结果OK的判定条件参数中，所有值应当大于0！" + "----用户：" + BaseForm._operatorName,
                BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                MessageBox.Show("读取的数据中，AOI推断结果OK的判定条件参数中，所有值应当大于0！");
                dataCheckResult = false;
            }


            if ((_inferResultKind1Number != 0 &&
                (_inferResultKind1Number == _inferResultKind2Number ||
                _inferResultKind1Number == _inferResultKind3Number ||
                _inferResultKind1Number == _inferResultKind4Number ||
                _inferResultKind1Number == _inferResultKind5Number)) ||
                (_inferResultKind2Number != 0 &&
                (_inferResultKind2Number == _inferResultKind3Number ||
                _inferResultKind2Number == _inferResultKind4Number ||
                _inferResultKind2Number == _inferResultKind5Number)) ||
                (_inferResultKind3Number != 0 &&
                (_inferResultKind3Number == _inferResultKind4Number ||
                _inferResultKind3Number == _inferResultKind5Number)) ||
                (_inferResultKind4Number != 0 &&
                _inferResultKind4Number == _inferResultKind5Number))
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，AOI推断结果OK的判定条件参数中，有相同不为0的种类号参数！" + "----用户：" + BaseForm._operatorName,
                BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                MessageBox.Show("读取的数据中，AOI推断结果OK的判定条件参数中，有相同不为0的种类号参数！");
                dataCheckResult = false;
            }
            
            return dataCheckResult;
        }

        /// <summary>
        /// LoadPointPositionData:读取点位坐标数据
        /// </summary>
        /// <param name="filePath">string:文件路径</param>
        /// <returns>bool:返回值，false-读取失败，true-读取成功</returns>
        private bool LoadPointPositionData(string filePath)
        {
            MyTool.TxtFileProcess.CreateLog("产品配方类-读取点位数据文件！" + "----用户：" + BaseForm._operatorName,
            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

            DataTable pointPositionDataDT = null;//点位坐标数据datatable

            if (!File.Exists(filePath))
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取点位数据文件失败，保存产品点位坐标数据的文件不存在！" + "----用户：" + BaseForm._operatorName,
            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                MessageBox.Show("保存产品点位坐标数据的文件不存在，无法读取点位坐标数据至控件!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            //读取数据至DT中
            pointPositionDataDT = CSVFileOperation.ReadCSV(filePath);//不包含第一行
            if (pointPositionDataDT.Columns.Count != 6)//如果列数不对，全部重新初始化
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取点位数据文件失败，保存产品点位坐标数据的文件格式不正确！" + "----用户：" + BaseForm._operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                MessageBox.Show("保存产品点位坐标数据的文件格式不正确，无法读取点位坐标数据!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            else
            {
                if (PointPositionDataCheck(pointPositionDataDT))
                {
                    _pointPositionDT = pointPositionDataDT;
                    return true;
                }

                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// PointPositionDataCheck:点位数据检查
        /// </summary>
        /// <param name="sourceDT">datatable：需要检查的数据Datatable</param>
        /// <returns></returns>
        private bool PointPositionDataCheck(DataTable sourceDT)
        {
            MyTool.TxtFileProcess.CreateLog("产品配方类-产品点位数据检查！" + "----用户：" + BaseForm._operatorName,
            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

            for (int i = 0; i < sourceDT.Rows.Count; i++)
            {
                //检查X轴坐标数据
                try
                {
                    Convert.ToSingle(sourceDT.Rows[i][1]);
                }
                catch
                {
                    MyTool.TxtFileProcess.CreateLog("产品配方类-产品点位数据检查失败，非法数据坐标为第2列，第" + (i + 1).ToString() + "行！" + "----用户：" + BaseForm._operatorName,
            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                    MessageBox.Show("检测产品点位坐标数据失败，非法数据坐标为第2列，第" + (i + 1).ToString() + "行！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                //检查Y轴坐标数据
                try
                {
                    Convert.ToSingle(sourceDT.Rows[i][2]);
                }
                catch
                {
                    MyTool.TxtFileProcess.CreateLog("产品配方类-产品点位数据检查失败，非法数据坐标为第3列，第" + (i + 1).ToString() + "行！" + "----用户：" + BaseForm._operatorName,
            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                    MessageBox.Show("检测产品点位坐标数据失败，非法数据坐标为第3列，第" + (i + 1).ToString() + "行！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                //检查R轴坐标数据
                try
                {
                    Convert.ToSingle(sourceDT.Rows[i][3]);
                }
                catch
                {
                    MyTool.TxtFileProcess.CreateLog("产品配方类-产品点位数据检查失败，非法数据坐标为第4列，第" + (i + 1).ToString() + "行！" + "----用户：" + BaseForm._operatorName,
            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                    MessageBox.Show("检测产品点位坐标数据失败，非法数据坐标为第4列，第" + (i + 1).ToString() + "行！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                //检查检测类型
                try
                {
                    if (Convert.ToInt32(sourceDT.Rows[i][4]) < 0 || Convert.ToInt32(sourceDT.Rows[i][4]) > 1)
                    {
                        MyTool.TxtFileProcess.CreateLog("产品配方类-产品点位数据检查失败，数据超出合法范围（0-1），非法数据坐标为第5列，第" + (i + 1).ToString() + "行！" + "----用户：" + BaseForm._operatorName,
            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                        MessageBox.Show("检测产品点位坐标数据失败，数据超出合法范围（0-1），非法数据坐标为第5列，第" + (i + 1).ToString() + "行！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }
                catch
                {
                    MyTool.TxtFileProcess.CreateLog("产品配方类-产品点位数据检查失败，非法数据坐标为第5列，第" + (i + 1).ToString() + "行！" + "----用户：" + BaseForm._operatorName,
            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                    MessageBox.Show("检测产品点位坐标数据失败，非法数据坐标为第5列，第" + (i + 1).ToString() + "行！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                //检查产品名称格式是否正确
                try
                {
                    string pointName = sourceDT.Rows[i][5].ToString();
                    int pcsNo = Convert.ToInt32(pointName.Substring(pointName.IndexOf("_") + 1, pointName.IndexOf("-") - pointName.IndexOf("_") - 1));
                    int elementNo = Convert.ToInt32(pointName.Substring(pointName.Length - 3, 3));
                }
                catch
                {
                    MyTool.TxtFileProcess.CreateLog("产品配方类-检测产品点位名称失败，点位名称格式非法，非法数据坐标为第6列，第" + (i + 1).ToString() + "行！" + "----用户：" + BaseForm._operatorName,
                       BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);
                    MessageBox.Show("检测产品点位名称失败，点位名称格式非法，非法数据坐标为第6列，第" + (i + 1).ToString() + "行！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                //检查产品名称是否有相同项存在
                for (int j = i + 1; j < sourceDT.Rows.Count; j++)
                {
                    if (sourceDT.Rows[i][5].ToString() == sourceDT.Rows[j][5].ToString())//如果有名称相同项存在
                    {
                        MyTool.TxtFileProcess.CreateLog("产品配方类-检测产品点位名称失败，有点位的名称相同，非法数据坐标为第6列，第" + (i + 1).ToString() + "行和第6列，第" + (j + 1).ToString() + "行！" + "----用户：" + BaseForm._operatorName,
                        BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);
                        MessageBox.Show("检测产品点位名称失败，有点位的名称相同，非法数据坐标为第6列，第" + (i + 1).ToString() + "行和第6列，第" + (j + 1).ToString() + "行！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
