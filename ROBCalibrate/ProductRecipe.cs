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

namespace ROBCalibrate
{
    public class ProductRecipe
    {
        public bool _parameterLoadSuccessfulFlag = false;//参数读取成功标志,false-失败,true-成功     
        const int LOAD_SAVE_DATA_ELEMENT_QUANTITY = 52 * 3 + 10;//变量个数
        string[,] _ctrlNameAndXmlFileNameStr = new string[3, LOAD_SAVE_DATA_ELEMENT_QUANTITY];//用于存储需要保存读取的参数变量名和xml名称对应数组,0-在XML中保存读取的参数名称,1-控件对应的ActualProduct变量名称,2-控件对应的变量类型

        //Station1
        public float ROB_XPickupTeachPosition_S1 = 0f;
        public float ROB_YPickupTeachPosition_S1 = 0f;
        public float ROB_ZPickupTeachPosition_S1 = 0f;
        public float ROB_RXPickupTeachPosition_S1 = 0f;
        public float ROB_RWPickupTeachPosition_S1 = 0f;
        public float ROB_RZPickupTeachPosition_S1 = 0f;
        public float Pixel_XPickupTeachPosition_RT_S1 = 0f;
        public float Pixel_YPickupTeachPosition_RT_S1 = 0f;
        public float Pixel_XPickupTeachPosition_LT_S1 = 0f;
        public float Pixel_YPickupTeachPosition_LT_S1 = 0f;
        public float Pixel_XPickupTeachPosition_LB_S1 = 0f;
        public float Pixel_YPickupTeachPosition_LB_S1 = 0f;
        public float Pixel_XPickupTeachPosition_RB_S1 = 0f;
        public float Pixel_YPickupTeachPosition_RB_S1 = 0f;
        public float Pixel_AnglePickupTeach_S1 = 0f;

        public float Pixel_XPickupCurrentPositionLimitMin_RT_S1 = 0f;
        public float Pixel_XPickupCurrentPositionLimitMax_RT_S1 = 0f;
        public float Pixel_YPickupCurrentPositionLimitMin_RT_S1 = 0f;
        public float Pixel_YPickupCurrentPositionLimitMax_RT_S1 = 0f;
        public float Pixel_AnglePickupCurrentLimitMin_S1 = 0f;
        public float Pixel_AnglePickupCurrentLimitMax_S1 = 0f;
        public float ROB_XPickupCurrentPositionLimitMin_S1 = 0f;
        public float ROB_XPickupCurrentPositionLimitMax_S1 = 0f;
        public float ROB_YPickupCurrentPositionLimitMin_S1 = 0f;
        public float ROB_YPickupCurrentPositionLimitMax_S1 = 0f;
        public float ROB_RZPickupCurrentPositionLimitMin_S1 = 0f;
        public float ROB_RZPickupCurrentPositionLimitMax_S1 = 0f;

        public float ThresholdMinGray_S1 = 0f;
        public float ThresholdMaxGray_S1 = 0f;
        public float OpeningWAndH_S1 = 0f;
        public float ShapeMinArea_S1 = 0f;
        public float ShapeMaxArea_S1 = 0f;
        public float ShapeMinRectangularity_S1 = 0f;
        public float ShapeMaxRectangularity_S1 = 0f;
        public float DilationRectangleWidth_S1 = 0f;
        public float DilationRectangleHeight_S1 = 0f;
        public float EdgesSubPixCannyAlpha_S1 = 0f;
        public float EdgesSubPixCannyLow_S1 = 0f;
        public float EdgesSubPixCannyHigh_S1 = 0f;
        public float UnionCollinearContoursXldMaxDistAbs_S1;
        public float UnionCollinearContoursXldMaxDistRel_S1;
        public float UnionCollinearContoursXldMaxShift_S1;
        public float UnionCollinearContoursXldMaxAngle_S1;
        public float XldContlengthMin_S1 = 0f;
        public float XldContlengthMax_S1 = 0f;
        public float XLDMinArea_S1 = 0f;
        public float XLDMaxArea_S1 = 0f;
        public float XLDMinRectangularity_S1 = 0f;
        public float XLDMaxRectangularity_S1 = 0f;
        public float UnionAdjacentContoursXldMaxDistAbs_S1;
        public float UnionAdjacentContoursXldMaxDistRel_S1;

        //Station2
        public float ROB_XPickupTeachPosition_S2 = 0f;
        public float ROB_YPickupTeachPosition_S2 = 0f;
        public float ROB_ZPickupTeachPosition_S2 = 0f;
        public float ROB_RXPickupTeachPosition_S2 = 0f;
        public float ROB_RWPickupTeachPosition_S2 = 0f;
        public float ROB_RZPickupTeachPosition_S2 = 0f;
        public float Pixel_XPickupTeachPosition_RT_S2 = 0f;
        public float Pixel_YPickupTeachPosition_RT_S2 = 0f;
        public float Pixel_XPickupTeachPosition_LT_S2 = 0f;
        public float Pixel_YPickupTeachPosition_LT_S2 = 0f;
        public float Pixel_XPickupTeachPosition_LB_S2 = 0f;
        public float Pixel_YPickupTeachPosition_LB_S2 = 0f;
        public float Pixel_XPickupTeachPosition_RB_S2 = 0f;
        public float Pixel_YPickupTeachPosition_RB_S2 = 0f;
        public float Pixel_AnglePickupTeach_S2 = 0f;

        public float Pixel_XPickupCurrentPositionLimitMin_RT_S2 = 0f;
        public float Pixel_XPickupCurrentPositionLimitMax_RT_S2 = 0f;
        public float Pixel_YPickupCurrentPositionLimitMin_RT_S2 = 0f;
        public float Pixel_YPickupCurrentPositionLimitMax_RT_S2 = 0f;
        public float Pixel_AnglePickupCurrentLimitMin_S2 = 0f;
        public float Pixel_AnglePickupCurrentLimitMax_S2 = 0f;
        public float ROB_XPickupCurrentPositionLimitMin_S2 = 0f;
        public float ROB_XPickupCurrentPositionLimitMax_S2 = 0f;
        public float ROB_YPickupCurrentPositionLimitMin_S2 = 0f;
        public float ROB_YPickupCurrentPositionLimitMax_S2 = 0f;
        public float ROB_RZPickupCurrentPositionLimitMin_S2 = 0f;
        public float ROB_RZPickupCurrentPositionLimitMax_S2 = 0f;

        public float ThresholdMinGray_S2 = 0f;
        public float ThresholdMaxGray_S2 = 0f;
        public float OpeningWAndH_S2 = 0f;
        public float ShapeMinArea_S2 = 0f;
        public float ShapeMaxArea_S2 = 0f;
        public float ShapeMinRectangularity_S2 = 0f;
        public float ShapeMaxRectangularity_S2 = 0f;
        public float DilationRectangleWidth_S2 = 0f;
        public float DilationRectangleHeight_S2 = 0f;
        public float EdgesSubPixCannyAlpha_S2 = 0f;
        public float EdgesSubPixCannyLow_S2 = 0f;
        public float EdgesSubPixCannyHigh_S2 = 0f;
        public float UnionCollinearContoursXldMaxDistAbs_S2;
        public float UnionCollinearContoursXldMaxDistRel_S2;
        public float UnionCollinearContoursXldMaxShift_S2;
        public float UnionCollinearContoursXldMaxAngle_S2;
        public float XldContlengthMin_S2 = 0f;
        public float XldContlengthMax_S2 = 0f;
        public float XLDMinArea_S2 = 0f;
        public float XLDMaxArea_S2 = 0f;
        public float XLDMinRectangularity_S2 = 0f;
        public float XLDMaxRectangularity_S2 = 0f;
        public float UnionAdjacentContoursXldMaxDistAbs_S2;
        public float UnionAdjacentContoursXldMaxDistRel_S2;

        //Station3
        public float ROB_XPickupTeachPosition_S3 = 0f;
        public float ROB_YPickupTeachPosition_S3 = 0f;
        public float ROB_ZPickupTeachPosition_S3 = 0f;
        public float ROB_RXPickupTeachPosition_S3 = 0f;
        public float ROB_RWPickupTeachPosition_S3 = 0f;
        public float ROB_RZPickupTeachPosition_S3 = 0f;
        public float Pixel_XPickupTeachPosition_RT_S3 = 0f;
        public float Pixel_YPickupTeachPosition_RT_S3 = 0f;
        public float Pixel_XPickupTeachPosition_LT_S3 = 0f;
        public float Pixel_YPickupTeachPosition_LT_S3 = 0f;
        public float Pixel_XPickupTeachPosition_LB_S3 = 0f;
        public float Pixel_YPickupTeachPosition_LB_S3 = 0f;
        public float Pixel_XPickupTeachPosition_RB_S3 = 0f;
        public float Pixel_YPickupTeachPosition_RB_S3 = 0f;
        public float Pixel_AnglePickupTeach_S3 = 0f;

        public float Pixel_XPickupCurrentPositionLimitMin_RT_S3 = 0f;
        public float Pixel_XPickupCurrentPositionLimitMax_RT_S3 = 0f;
        public float Pixel_YPickupCurrentPositionLimitMin_RT_S3 = 0f;
        public float Pixel_YPickupCurrentPositionLimitMax_RT_S3 = 0f;
        public float Pixel_AnglePickupCurrentLimitMin_S3 = 0f;
        public float Pixel_AnglePickupCurrentLimitMax_S3 = 0f;
        public float ROB_XPickupCurrentPositionLimitMin_S3 = 0f;
        public float ROB_XPickupCurrentPositionLimitMax_S3 = 0f;
        public float ROB_YPickupCurrentPositionLimitMin_S3 = 0f;
        public float ROB_YPickupCurrentPositionLimitMax_S3 = 0f;
        public float ROB_RZPickupCurrentPositionLimitMin_S3 = 0f;
        public float ROB_RZPickupCurrentPositionLimitMax_S3 = 0f;

        public float ThresholdMinGray_S3 = 0f;
        public float ThresholdMaxGray_S3 = 0f;
        public float OpeningWAndH_S3 = 0f;
        public float ShapeMinArea_S3 = 0f;
        public float ShapeMaxArea_S3 = 0f;
        public float ShapeMinRectangularity_S3 = 0f;
        public float ShapeMaxRectangularity_S3 = 0f;
        public float DilationRectangleWidth_S3 = 0f;
        public float DilationRectangleHeight_S3 = 0f;
        public float EdgesSubPixCannyAlpha_S3 = 0f;
        public float EdgesSubPixCannyLow_S3 = 0f;
        public float EdgesSubPixCannyHigh_S3 = 0f;
        public float UnionCollinearContoursXldMaxDistAbs_S3;
        public float UnionCollinearContoursXldMaxDistRel_S3;
        public float UnionCollinearContoursXldMaxShift_S3;
        public float UnionCollinearContoursXldMaxAngle_S3;
        public float XldContlengthMin_S3 = 0f;
        public float XldContlengthMax_S3 = 0f;
        public float XLDMinArea_S3 = 0f;
        public float XLDMaxArea_S3 = 0f;
        public float XLDMinRectangularity_S3 = 0f;
        public float XLDMaxRectangularity_S3 = 0f;
        public float UnionAdjacentContoursXldMaxDistAbs_S3;
        public float UnionAdjacentContoursXldMaxDistRel_S3;

        public float ClassificationThresholdMin_S3 = 0f;
        public float ClassificationThresholdMax_S3 = 0f;
        public float ProductAreaMin_S3 = 0f;
        public float ProductAreaMax_S3 = 0f;
        public float CoverAreaMin_S3 = 0f;
        public float CoverAreaMax_S3 = 0f;
        public float EmptyAreaMin_S3 = 0f;
        public float EmptyAreaMax_S3 = 0f;
        public float ClassificationThresholdCoverMin_S3 = 0f;
        public float ClassificationThresholdCoverMax_S3 = 0f;

        public bool AngleRotate_S1 = false;
        public bool AngleRotate_S2 = false;
        public bool AngleRotate_S3 = false;

        #region InitialCtrlBoxCorrespondingStringArr:初始化变量名和xml文件保存名称对应表
        void InitialCtrlBoxCorrespondingStringArr()
        {
            MyTool.TxtFileProcess.CreateLog("产品配方类-初始化变量名和xml文件保存名称对应表" + "----用户：" + ActionForm._operatorName,
            ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);

            try
            {
                #region 在XML中保存读取的参数名称
                //Station1
                _ctrlNameAndXmlFileNameStr[0, 0] = "ROB_XPickupTeachPosition_S1";
                _ctrlNameAndXmlFileNameStr[0, 1] = "ROB_YPickupTeachPosition_S1";
                _ctrlNameAndXmlFileNameStr[0, 2] = "ROB_ZPickupTeachPosition_S1";
                _ctrlNameAndXmlFileNameStr[0, 3] = "ROB_RXPickupTeachPosition_S1";
                _ctrlNameAndXmlFileNameStr[0, 4] = "ROB_RWPickupTeachPosition_S1";
                _ctrlNameAndXmlFileNameStr[0, 5] = "ROB_RZPickupTeachPosition_S1";
                _ctrlNameAndXmlFileNameStr[0, 6] = "Pixel_XPickupTeachPosition_RT_S1";
                _ctrlNameAndXmlFileNameStr[0, 7] = "Pixel_YPickupTeachPosition_RT_S1";
                _ctrlNameAndXmlFileNameStr[0, 8] = "Pixel_XPickupTeachPosition_LT_S1";
                _ctrlNameAndXmlFileNameStr[0, 9] = "Pixel_YPickupTeachPosition_LT_S1";
                _ctrlNameAndXmlFileNameStr[0, 10] = "Pixel_XPickupTeachPosition_LB_S1";
                _ctrlNameAndXmlFileNameStr[0, 11] = "Pixel_YPickupTeachPosition_LB_S1";
                _ctrlNameAndXmlFileNameStr[0, 12] = "Pixel_XPickupTeachPosition_RB_S1";
                _ctrlNameAndXmlFileNameStr[0, 13] = "Pixel_YPickupTeachPosition_RB_S1";
                _ctrlNameAndXmlFileNameStr[0, 14] = "Pixel_AnglePickupTeach_S1";

                _ctrlNameAndXmlFileNameStr[0, 15] = "Pixel_XPickupCurrentPositionLimitMin_RT_S1";
                _ctrlNameAndXmlFileNameStr[0, 16] = "Pixel_XPickupCurrentPositionLimitMax_RT_S1";
                _ctrlNameAndXmlFileNameStr[0, 17] = "Pixel_YPickupCurrentPositionLimitMin_RT_S1";
                _ctrlNameAndXmlFileNameStr[0, 18] = "Pixel_YPickupCurrentPositionLimitMax_RT_S1";
                _ctrlNameAndXmlFileNameStr[0, 19] = "Pixel_AnglePickupCurrentLimitMin_S1";
                _ctrlNameAndXmlFileNameStr[0, 20] = "Pixel_AnglePickupCurrentLimitMax_S1";
                _ctrlNameAndXmlFileNameStr[0, 21] = "ROB_XPickupCurrentPositionLimitMin_S1";
                _ctrlNameAndXmlFileNameStr[0, 22] = "ROB_XPickupCurrentPositionLimitMax_S1";
                _ctrlNameAndXmlFileNameStr[0, 23] = "ROB_YPickupCurrentPositionLimitMin_S1";
                _ctrlNameAndXmlFileNameStr[0, 24] = "ROB_YPickupCurrentPositionLimitMax_S1";
                _ctrlNameAndXmlFileNameStr[0, 25] = "ROB_RZPickupCurrentPositionLimitMin_S1";
                _ctrlNameAndXmlFileNameStr[0, 26] = "ROB_RZPickupCurrentPositionLimitMax_S1";

                _ctrlNameAndXmlFileNameStr[0, 27] = "ThresholdMinGray_S1";
                _ctrlNameAndXmlFileNameStr[0, 28] = "ThresholdMaxGray_S1";
                _ctrlNameAndXmlFileNameStr[0, 29] = "OpeningWAndH_S1";
                _ctrlNameAndXmlFileNameStr[0, 30] = "ShapeMinArea_S1";
                _ctrlNameAndXmlFileNameStr[0, 31] = "ShapeMaxArea_S1";
                _ctrlNameAndXmlFileNameStr[0, 32] = "ShapeMinRectangularity_S1";
                _ctrlNameAndXmlFileNameStr[0, 33] = "ShapeMaxRectangularity_S1";
                _ctrlNameAndXmlFileNameStr[0, 34] = "DilationRectangleWidth_S1";
                _ctrlNameAndXmlFileNameStr[0, 35] = "DilationRectangleHeight_S1";
                _ctrlNameAndXmlFileNameStr[0, 36] = "EdgesSubPixCannyAlpha_S1";
                _ctrlNameAndXmlFileNameStr[0, 37] = "EdgesSubPixCannyLow_S1";
                _ctrlNameAndXmlFileNameStr[0, 38] = "EdgesSubPixCannyHigh_S1";
                _ctrlNameAndXmlFileNameStr[0, 39] = "UnionCollinearContoursXldMaxDistAbs_S1";
                _ctrlNameAndXmlFileNameStr[0, 40] = "UnionCollinearContoursXldMaxDistRel_S1";
                _ctrlNameAndXmlFileNameStr[0, 41] = "UnionCollinearContoursXldMaxShift_S1";
                _ctrlNameAndXmlFileNameStr[0, 42] = "UnionCollinearContoursXldMaxAngle_S1";
                _ctrlNameAndXmlFileNameStr[0, 43] = "XldContlengthMin_S1";
                _ctrlNameAndXmlFileNameStr[0, 44] = "XldContlengthMax_S1";
                _ctrlNameAndXmlFileNameStr[0, 45] = "XLDMinArea_S1";
                _ctrlNameAndXmlFileNameStr[0, 46] = "XLDMaxArea_S1";
                _ctrlNameAndXmlFileNameStr[0, 47] = "XLDMinRectangularity_S1";
                _ctrlNameAndXmlFileNameStr[0, 48] = "XLDMaxRectangularity_S1";
                _ctrlNameAndXmlFileNameStr[0, 49] = "UnionAdjacentContoursXldMaxDistAbs_S1";
                _ctrlNameAndXmlFileNameStr[0, 50] = "UnionAdjacentContoursXldMaxDistRel_S1";
                //Station2
                _ctrlNameAndXmlFileNameStr[0, 51] = "ROB_XPickupTeachPosition_S2";
                _ctrlNameAndXmlFileNameStr[0, 52] = "ROB_YPickupTeachPosition_S2";
                _ctrlNameAndXmlFileNameStr[0, 53] = "ROB_ZPickupTeachPosition_S2";
                _ctrlNameAndXmlFileNameStr[0, 54] = "ROB_RXPickupTeachPosition_S2";
                _ctrlNameAndXmlFileNameStr[0, 55] = "ROB_RWPickupTeachPosition_S2";
                _ctrlNameAndXmlFileNameStr[0, 56] = "ROB_RZPickupTeachPosition_S2";
                _ctrlNameAndXmlFileNameStr[0, 57] = "Pixel_XPickupTeachPosition_RT_S2";
                _ctrlNameAndXmlFileNameStr[0, 58] = "Pixel_YPickupTeachPosition_RT_S2";
                _ctrlNameAndXmlFileNameStr[0, 59] = "Pixel_XPickupTeachPosition_LT_S2";
                _ctrlNameAndXmlFileNameStr[0, 60] = "Pixel_YPickupTeachPosition_LT_S2";
                _ctrlNameAndXmlFileNameStr[0, 61] = "Pixel_XPickupTeachPosition_LB_S2";
                _ctrlNameAndXmlFileNameStr[0, 62] = "Pixel_YPickupTeachPosition_LB_S2";
                _ctrlNameAndXmlFileNameStr[0, 63] = "Pixel_XPickupTeachPosition_RB_S2";
                _ctrlNameAndXmlFileNameStr[0, 64] = "Pixel_YPickupTeachPosition_RB_S2";
                _ctrlNameAndXmlFileNameStr[0, 65] = "Pixel_AnglePickupTeach_S2";

                _ctrlNameAndXmlFileNameStr[0, 66] = "Pixel_XPickupCurrentPositionLimitMin_RT_S2";
                _ctrlNameAndXmlFileNameStr[0, 67] = "Pixel_XPickupCurrentPositionLimitMax_RT_S2";
                _ctrlNameAndXmlFileNameStr[0, 68] = "Pixel_YPickupCurrentPositionLimitMin_RT_S2";
                _ctrlNameAndXmlFileNameStr[0, 69] = "Pixel_YPickupCurrentPositionLimitMax_RT_S2";
                _ctrlNameAndXmlFileNameStr[0, 70] = "Pixel_AnglePickupCurrentLimitMin_S2";
                _ctrlNameAndXmlFileNameStr[0, 71] = "Pixel_AnglePickupCurrentLimitMax_S2";
                _ctrlNameAndXmlFileNameStr[0, 72] = "ROB_XPickupCurrentPositionLimitMin_S2";
                _ctrlNameAndXmlFileNameStr[0, 73] = "ROB_XPickupCurrentPositionLimitMax_S2";
                _ctrlNameAndXmlFileNameStr[0, 74] = "ROB_YPickupCurrentPositionLimitMin_S2";
                _ctrlNameAndXmlFileNameStr[0, 75] = "ROB_YPickupCurrentPositionLimitMax_S2";
                _ctrlNameAndXmlFileNameStr[0, 76] = "ROB_RZPickupCurrentPositionLimitMin_S2";
                _ctrlNameAndXmlFileNameStr[0, 77] = "ROB_RZPickupCurrentPositionLimitMax_S2";

                _ctrlNameAndXmlFileNameStr[0, 78] = "ThresholdMinGray_S2";
                _ctrlNameAndXmlFileNameStr[0, 79] = "ThresholdMaxGray_S2";
                _ctrlNameAndXmlFileNameStr[0, 80] = "OpeningWAndH_S2";
                _ctrlNameAndXmlFileNameStr[0, 81] = "ShapeMinArea_S2";
                _ctrlNameAndXmlFileNameStr[0, 82] = "ShapeMaxArea_S2";
                _ctrlNameAndXmlFileNameStr[0, 83] = "ShapeMinRectangularity_S2";
                _ctrlNameAndXmlFileNameStr[0, 84] = "ShapeMaxRectangularity_S2";
                _ctrlNameAndXmlFileNameStr[0, 85] = "DilationRectangleWidth_S2";
                _ctrlNameAndXmlFileNameStr[0, 86] = "DilationRectangleHeight_S2";
                _ctrlNameAndXmlFileNameStr[0, 87] = "EdgesSubPixCannyAlpha_S2";
                _ctrlNameAndXmlFileNameStr[0, 88] = "EdgesSubPixCannyLow_S2";
                _ctrlNameAndXmlFileNameStr[0, 89] = "EdgesSubPixCannyHigh_S2";
                _ctrlNameAndXmlFileNameStr[0, 90] = "UnionCollinearContoursXldMaxDistAbs_S2";
                _ctrlNameAndXmlFileNameStr[0, 91] = "UnionCollinearContoursXldMaxDistRel_S2";
                _ctrlNameAndXmlFileNameStr[0, 92] = "UnionCollinearContoursXldMaxShift_S2";
                _ctrlNameAndXmlFileNameStr[0, 93] = "UnionCollinearContoursXldMaxAngle_S2";
                _ctrlNameAndXmlFileNameStr[0, 94] = "XldContlengthMin_S2";
                _ctrlNameAndXmlFileNameStr[0, 95] = "XldContlengthMax_S2";
                _ctrlNameAndXmlFileNameStr[0, 96] = "XLDMinArea_S2";
                _ctrlNameAndXmlFileNameStr[0, 97] = "XLDMaxArea_S2";
                _ctrlNameAndXmlFileNameStr[0, 98] = "XLDMinRectangularity_S2";
                _ctrlNameAndXmlFileNameStr[0, 99] = "XLDMaxRectangularity_S2";
                _ctrlNameAndXmlFileNameStr[0, 100] = "UnionAdjacentContoursXldMaxDistAbs_S2";
                _ctrlNameAndXmlFileNameStr[0, 101] = "UnionAdjacentContoursXldMaxDistRel_S2";
                //Station3
                _ctrlNameAndXmlFileNameStr[0, 102] = "ROB_XPickupTeachPosition_S3";
                _ctrlNameAndXmlFileNameStr[0, 103] = "ROB_YPickupTeachPosition_S3";
                _ctrlNameAndXmlFileNameStr[0, 104] = "ROB_ZPickupTeachPosition_S3";
                _ctrlNameAndXmlFileNameStr[0, 105] = "ROB_RXPickupTeachPosition_S3";
                _ctrlNameAndXmlFileNameStr[0, 106] = "ROB_RWPickupTeachPosition_S3";
                _ctrlNameAndXmlFileNameStr[0, 107] = "ROB_RZPickupTeachPosition_S3";
                _ctrlNameAndXmlFileNameStr[0, 108] = "Pixel_XPickupTeachPosition_RT_S3";
                _ctrlNameAndXmlFileNameStr[0, 109] = "Pixel_YPickupTeachPosition_RT_S3";
                _ctrlNameAndXmlFileNameStr[0, 110] = "Pixel_XPickupTeachPosition_LT_S3";
                _ctrlNameAndXmlFileNameStr[0, 111] = "Pixel_YPickupTeachPosition_LT_S3";
                _ctrlNameAndXmlFileNameStr[0, 112] = "Pixel_XPickupTeachPosition_LB_S3";
                _ctrlNameAndXmlFileNameStr[0, 113] = "Pixel_YPickupTeachPosition_LB_S3";
                _ctrlNameAndXmlFileNameStr[0, 114] = "Pixel_XPickupTeachPosition_RB_S3";
                _ctrlNameAndXmlFileNameStr[0, 115] = "Pixel_YPickupTeachPosition_RB_S3";
                _ctrlNameAndXmlFileNameStr[0, 116] = "Pixel_AnglePickupTeach_S3";

                _ctrlNameAndXmlFileNameStr[0, 117] = "Pixel_XPickupCurrentPositionLimitMin_RT_S3";
                _ctrlNameAndXmlFileNameStr[0, 118] = "Pixel_XPickupCurrentPositionLimitMax_RT_S3";
                _ctrlNameAndXmlFileNameStr[0, 119] = "Pixel_YPickupCurrentPositionLimitMin_RT_S3";
                _ctrlNameAndXmlFileNameStr[0, 120] = "Pixel_YPickupCurrentPositionLimitMax_RT_S3";
                _ctrlNameAndXmlFileNameStr[0, 121] = "Pixel_AnglePickupCurrentLimitMin_S3";
                _ctrlNameAndXmlFileNameStr[0, 122] = "Pixel_AnglePickupCurrentLimitMax_S3";
                _ctrlNameAndXmlFileNameStr[0, 123] = "ROB_XPickupCurrentPositionLimitMin_S3";
                _ctrlNameAndXmlFileNameStr[0, 124] = "ROB_XPickupCurrentPositionLimitMax_S3";
                _ctrlNameAndXmlFileNameStr[0, 125] = "ROB_YPickupCurrentPositionLimitMin_S3";
                _ctrlNameAndXmlFileNameStr[0, 126] = "ROB_YPickupCurrentPositionLimitMax_S3";
                _ctrlNameAndXmlFileNameStr[0, 127] = "ROB_RZPickupCurrentPositionLimitMin_S3";
                _ctrlNameAndXmlFileNameStr[0, 128] = "ROB_RZPickupCurrentPositionLimitMax_S3";

                _ctrlNameAndXmlFileNameStr[0, 129] = "ThresholdMinGray_S3";
                _ctrlNameAndXmlFileNameStr[0, 130] = "ThresholdMaxGray_S3";
                _ctrlNameAndXmlFileNameStr[0, 131] = "OpeningWAndH_S3";
                _ctrlNameAndXmlFileNameStr[0, 132] = "ShapeMinArea_S3";
                _ctrlNameAndXmlFileNameStr[0, 133] = "ShapeMaxArea_S3";
                _ctrlNameAndXmlFileNameStr[0, 134] = "ShapeMinRectangularity_S3";
                _ctrlNameAndXmlFileNameStr[0, 135] = "ShapeMaxRectangularity_S3";
                _ctrlNameAndXmlFileNameStr[0, 136] = "DilationRectangleWidth_S3";
                _ctrlNameAndXmlFileNameStr[0, 137] = "DilationRectangleHeight_S3";
                _ctrlNameAndXmlFileNameStr[0, 138] = "EdgesSubPixCannyAlpha_S3";
                _ctrlNameAndXmlFileNameStr[0, 139] = "EdgesSubPixCannyLow_S3";
                _ctrlNameAndXmlFileNameStr[0, 140] = "EdgesSubPixCannyHigh_S3";
                _ctrlNameAndXmlFileNameStr[0, 141] = "UnionCollinearContoursXldMaxDistAbs_S3";
                _ctrlNameAndXmlFileNameStr[0, 142] = "UnionCollinearContoursXldMaxDistRel_S3";
                _ctrlNameAndXmlFileNameStr[0, 143] = "UnionCollinearContoursXldMaxShift_S3";
                _ctrlNameAndXmlFileNameStr[0, 144] = "UnionCollinearContoursXldMaxAngle_S3";
                _ctrlNameAndXmlFileNameStr[0, 145] = "XldContlengthMin_S3";
                _ctrlNameAndXmlFileNameStr[0, 146] = "XldContlengthMax_S3";
                _ctrlNameAndXmlFileNameStr[0, 147] = "XLDMinArea_S3";
                _ctrlNameAndXmlFileNameStr[0, 148] = "XLDMaxArea_S3";
                _ctrlNameAndXmlFileNameStr[0, 149] = "XLDMinRectangularity_S3";
                _ctrlNameAndXmlFileNameStr[0, 150] = "XLDMaxRectangularity_S3";
                _ctrlNameAndXmlFileNameStr[0, 151] = "UnionAdjacentContoursXldMaxDistAbs_S3";
                _ctrlNameAndXmlFileNameStr[0, 152] = "UnionAdjacentContoursXldMaxDistRel_S3";

                _ctrlNameAndXmlFileNameStr[0, 153] = "ClassificationThresholdMin_S3";
                _ctrlNameAndXmlFileNameStr[0, 154] = "ClassificationThresholdMax_S3";
                _ctrlNameAndXmlFileNameStr[0, 155] = "ProductAreaMin_S3";
                _ctrlNameAndXmlFileNameStr[0, 156] = "ProductAreaMax_S3";
                _ctrlNameAndXmlFileNameStr[0, 157] = "CoverAreaMin_S3";
                _ctrlNameAndXmlFileNameStr[0, 158] = "CoverAreaMax_S3";
                _ctrlNameAndXmlFileNameStr[0, 159] = "EmptyAreaMin_S3";
                _ctrlNameAndXmlFileNameStr[0, 160] = "EmptyAreaMax_S3";
                _ctrlNameAndXmlFileNameStr[0, 161] = "ClassificationThresholdCoverMin_S3";
                _ctrlNameAndXmlFileNameStr[0, 162] = "ClassificationThresholdCoverMax_S3";

                _ctrlNameAndXmlFileNameStr[0, 163] = "AngleRotate_S1";
                _ctrlNameAndXmlFileNameStr[0, 164] = "AngleRotate_S2";
                _ctrlNameAndXmlFileNameStr[0, 165] = "AngleRotate_S3";
                #endregion

                #region 控件对应的ActualProduct变量名称
                //Station1
                _ctrlNameAndXmlFileNameStr[1, 0] = "ROB_XPickupTeachPosition_S1";
                _ctrlNameAndXmlFileNameStr[1, 1] = "ROB_YPickupTeachPosition_S1";
                _ctrlNameAndXmlFileNameStr[1, 2] = "ROB_ZPickupTeachPosition_S1";
                _ctrlNameAndXmlFileNameStr[1, 3] = "ROB_RXPickupTeachPosition_S1";
                _ctrlNameAndXmlFileNameStr[1, 4] = "ROB_RWPickupTeachPosition_S1";
                _ctrlNameAndXmlFileNameStr[1, 5] = "ROB_RZPickupTeachPosition_S1";
                _ctrlNameAndXmlFileNameStr[1, 6] = "Pixel_XPickupTeachPosition_RT_S1";
                _ctrlNameAndXmlFileNameStr[1, 7] = "Pixel_YPickupTeachPosition_RT_S1";
                _ctrlNameAndXmlFileNameStr[1, 8] = "Pixel_XPickupTeachPosition_LT_S1";
                _ctrlNameAndXmlFileNameStr[1, 9] = "Pixel_YPickupTeachPosition_LT_S1";
                _ctrlNameAndXmlFileNameStr[1, 10] = "Pixel_XPickupTeachPosition_LB_S1";
                _ctrlNameAndXmlFileNameStr[1, 11] = "Pixel_YPickupTeachPosition_LB_S1";
                _ctrlNameAndXmlFileNameStr[1, 12] = "Pixel_XPickupTeachPosition_RB_S1";
                _ctrlNameAndXmlFileNameStr[1, 13] = "Pixel_YPickupTeachPosition_RB_S1";
                _ctrlNameAndXmlFileNameStr[1, 14] = "Pixel_AnglePickupTeach_S1";

                _ctrlNameAndXmlFileNameStr[1, 15] = "Pixel_XPickupCurrentPositionLimitMin_RT_S1";
                _ctrlNameAndXmlFileNameStr[1, 16] = "Pixel_XPickupCurrentPositionLimitMax_RT_S1";
                _ctrlNameAndXmlFileNameStr[1, 17] = "Pixel_YPickupCurrentPositionLimitMin_RT_S1";
                _ctrlNameAndXmlFileNameStr[1, 18] = "Pixel_YPickupCurrentPositionLimitMax_RT_S1";
                _ctrlNameAndXmlFileNameStr[1, 19] = "Pixel_AnglePickupCurrentLimitMin_S1";
                _ctrlNameAndXmlFileNameStr[1, 20] = "Pixel_AnglePickupCurrentLimitMax_S1";
                _ctrlNameAndXmlFileNameStr[1, 21] = "ROB_XPickupCurrentPositionLimitMin_S1";
                _ctrlNameAndXmlFileNameStr[1, 22] = "ROB_XPickupCurrentPositionLimitMax_S1";
                _ctrlNameAndXmlFileNameStr[1, 23] = "ROB_YPickupCurrentPositionLimitMin_S1";
                _ctrlNameAndXmlFileNameStr[1, 24] = "ROB_YPickupCurrentPositionLimitMax_S1";
                _ctrlNameAndXmlFileNameStr[1, 25] = "ROB_RZPickupCurrentPositionLimitMin_S1";
                _ctrlNameAndXmlFileNameStr[1, 26] = "ROB_RZPickupCurrentPositionLimitMax_S1";

                _ctrlNameAndXmlFileNameStr[1, 27] = "ThresholdMinGray_S1";
                _ctrlNameAndXmlFileNameStr[1, 28] = "ThresholdMaxGray_S1";
                _ctrlNameAndXmlFileNameStr[1, 29] = "OpeningWAndH_S1";
                _ctrlNameAndXmlFileNameStr[1, 30] = "ShapeMinArea_S1";
                _ctrlNameAndXmlFileNameStr[1, 31] = "ShapeMaxArea_S1";
                _ctrlNameAndXmlFileNameStr[1, 32] = "ShapeMinRectangularity_S1";
                _ctrlNameAndXmlFileNameStr[1, 33] = "ShapeMaxRectangularity_S1";
                _ctrlNameAndXmlFileNameStr[1, 34] = "DilationRectangleWidth_S1";
                _ctrlNameAndXmlFileNameStr[1, 35] = "DilationRectangleHeight_S1";
                _ctrlNameAndXmlFileNameStr[1, 36] = "EdgesSubPixCannyAlpha_S1";
                _ctrlNameAndXmlFileNameStr[1, 37] = "EdgesSubPixCannyLow_S1";
                _ctrlNameAndXmlFileNameStr[1, 38] = "EdgesSubPixCannyHigh_S1";
                _ctrlNameAndXmlFileNameStr[1, 39] = "UnionCollinearContoursXldMaxDistAbs_S1";
                _ctrlNameAndXmlFileNameStr[1, 40] = "UnionCollinearContoursXldMaxDistRel_S1";
                _ctrlNameAndXmlFileNameStr[1, 41] = "UnionCollinearContoursXldMaxShift_S1";
                _ctrlNameAndXmlFileNameStr[1, 42] = "UnionCollinearContoursXldMaxAngle_S1";
                _ctrlNameAndXmlFileNameStr[1, 43] = "XldContlengthMin_S1";
                _ctrlNameAndXmlFileNameStr[1, 44] = "XldContlengthMax_S1";
                _ctrlNameAndXmlFileNameStr[1, 45] = "XLDMinArea_S1";
                _ctrlNameAndXmlFileNameStr[1, 46] = "XLDMaxArea_S1";
                _ctrlNameAndXmlFileNameStr[1, 47] = "XLDMinRectangularity_S1";
                _ctrlNameAndXmlFileNameStr[1, 48] = "XLDMaxRectangularity_S1";
                _ctrlNameAndXmlFileNameStr[1, 49] = "UnionAdjacentContoursXldMaxDistAbs_S1";
                _ctrlNameAndXmlFileNameStr[1, 50] = "UnionAdjacentContoursXldMaxDistRel_S1";
                //Station2
                _ctrlNameAndXmlFileNameStr[1, 51] = "ROB_XPickupTeachPosition_S2";
                _ctrlNameAndXmlFileNameStr[1, 52] = "ROB_YPickupTeachPosition_S2";
                _ctrlNameAndXmlFileNameStr[1, 53] = "ROB_ZPickupTeachPosition_S2";
                _ctrlNameAndXmlFileNameStr[1, 54] = "ROB_RXPickupTeachPosition_S2";
                _ctrlNameAndXmlFileNameStr[1, 55] = "ROB_RWPickupTeachPosition_S2";
                _ctrlNameAndXmlFileNameStr[1, 56] = "ROB_RZPickupTeachPosition_S2";
                _ctrlNameAndXmlFileNameStr[1, 57] = "Pixel_XPickupTeachPosition_RT_S2";
                _ctrlNameAndXmlFileNameStr[1, 58] = "Pixel_YPickupTeachPosition_RT_S2";
                _ctrlNameAndXmlFileNameStr[1, 59] = "Pixel_XPickupTeachPosition_LT_S2";
                _ctrlNameAndXmlFileNameStr[1, 60] = "Pixel_YPickupTeachPosition_LT_S2";
                _ctrlNameAndXmlFileNameStr[1, 61] = "Pixel_XPickupTeachPosition_LB_S2";
                _ctrlNameAndXmlFileNameStr[1, 62] = "Pixel_YPickupTeachPosition_LB_S2";
                _ctrlNameAndXmlFileNameStr[1, 63] = "Pixel_XPickupTeachPosition_RB_S2";
                _ctrlNameAndXmlFileNameStr[1, 64] = "Pixel_YPickupTeachPosition_RB_S2";
                _ctrlNameAndXmlFileNameStr[1, 65] = "Pixel_AnglePickupTeach_S2";

                _ctrlNameAndXmlFileNameStr[1, 66] = "Pixel_XPickupCurrentPositionLimitMin_RT_S2";
                _ctrlNameAndXmlFileNameStr[1, 67] = "Pixel_XPickupCurrentPositionLimitMax_RT_S2";
                _ctrlNameAndXmlFileNameStr[1, 68] = "Pixel_YPickupCurrentPositionLimitMin_RT_S2";
                _ctrlNameAndXmlFileNameStr[1, 69] = "Pixel_YPickupCurrentPositionLimitMax_RT_S2";
                _ctrlNameAndXmlFileNameStr[1, 70] = "Pixel_AnglePickupCurrentLimitMin_S2";
                _ctrlNameAndXmlFileNameStr[1, 71] = "Pixel_AnglePickupCurrentLimitMax_S2";
                _ctrlNameAndXmlFileNameStr[1, 72] = "ROB_XPickupCurrentPositionLimitMin_S2";
                _ctrlNameAndXmlFileNameStr[1, 73] = "ROB_XPickupCurrentPositionLimitMax_S2";
                _ctrlNameAndXmlFileNameStr[1, 74] = "ROB_YPickupCurrentPositionLimitMin_S2";
                _ctrlNameAndXmlFileNameStr[1, 75] = "ROB_YPickupCurrentPositionLimitMax_S2";
                _ctrlNameAndXmlFileNameStr[1, 76] = "ROB_RZPickupCurrentPositionLimitMin_S2";
                _ctrlNameAndXmlFileNameStr[1, 77] = "ROB_RZPickupCurrentPositionLimitMax_S2";

                _ctrlNameAndXmlFileNameStr[1, 78] = "ThresholdMinGray_S2";
                _ctrlNameAndXmlFileNameStr[1, 79] = "ThresholdMaxGray_S2";
                _ctrlNameAndXmlFileNameStr[1, 80] = "OpeningWAndH_S2";
                _ctrlNameAndXmlFileNameStr[1, 81] = "ShapeMinArea_S2";
                _ctrlNameAndXmlFileNameStr[1, 82] = "ShapeMaxArea_S2";
                _ctrlNameAndXmlFileNameStr[1, 83] = "ShapeMinRectangularity_S2";
                _ctrlNameAndXmlFileNameStr[1, 84] = "ShapeMaxRectangularity_S2";
                _ctrlNameAndXmlFileNameStr[1, 85] = "DilationRectangleWidth_S2";
                _ctrlNameAndXmlFileNameStr[1, 86] = "DilationRectangleHeight_S2";
                _ctrlNameAndXmlFileNameStr[1, 87] = "EdgesSubPixCannyAlpha_S2";
                _ctrlNameAndXmlFileNameStr[1, 88] = "EdgesSubPixCannyLow_S2";
                _ctrlNameAndXmlFileNameStr[1, 89] = "EdgesSubPixCannyHigh_S2";
                _ctrlNameAndXmlFileNameStr[1, 90] = "UnionCollinearContoursXldMaxDistAbs_S2";
                _ctrlNameAndXmlFileNameStr[1, 91] = "UnionCollinearContoursXldMaxDistRel_S2";
                _ctrlNameAndXmlFileNameStr[1, 92] = "UnionCollinearContoursXldMaxShift_S2";
                _ctrlNameAndXmlFileNameStr[1, 93] = "UnionCollinearContoursXldMaxAngle_S2";
                _ctrlNameAndXmlFileNameStr[1, 94] = "XldContlengthMin_S2";
                _ctrlNameAndXmlFileNameStr[1, 95] = "XldContlengthMax_S2";
                _ctrlNameAndXmlFileNameStr[1, 96] = "XLDMinArea_S2";
                _ctrlNameAndXmlFileNameStr[1, 97] = "XLDMaxArea_S2";
                _ctrlNameAndXmlFileNameStr[1, 98] = "XLDMinRectangularity_S2";
                _ctrlNameAndXmlFileNameStr[1, 99] = "XLDMaxRectangularity_S2";
                _ctrlNameAndXmlFileNameStr[1, 100] = "UnionAdjacentContoursXldMaxDistAbs_S2";
                _ctrlNameAndXmlFileNameStr[1, 101] = "UnionAdjacentContoursXldMaxDistRel_S2";
                //Station3
                _ctrlNameAndXmlFileNameStr[1, 102] = "ROB_XPickupTeachPosition_S3";
                _ctrlNameAndXmlFileNameStr[1, 103] = "ROB_YPickupTeachPosition_S3";
                _ctrlNameAndXmlFileNameStr[1, 104] = "ROB_ZPickupTeachPosition_S3";
                _ctrlNameAndXmlFileNameStr[1, 105] = "ROB_RXPickupTeachPosition_S3";
                _ctrlNameAndXmlFileNameStr[1, 106] = "ROB_RWPickupTeachPosition_S3";
                _ctrlNameAndXmlFileNameStr[1, 107] = "ROB_RZPickupTeachPosition_S3";
                _ctrlNameAndXmlFileNameStr[1, 108] = "Pixel_XPickupTeachPosition_RT_S3";
                _ctrlNameAndXmlFileNameStr[1, 109] = "Pixel_YPickupTeachPosition_RT_S3";
                _ctrlNameAndXmlFileNameStr[1, 110] = "Pixel_XPickupTeachPosition_LT_S3";
                _ctrlNameAndXmlFileNameStr[1, 111] = "Pixel_YPickupTeachPosition_LT_S3";
                _ctrlNameAndXmlFileNameStr[1, 112] = "Pixel_XPickupTeachPosition_LB_S3";
                _ctrlNameAndXmlFileNameStr[1, 113] = "Pixel_YPickupTeachPosition_LB_S3";
                _ctrlNameAndXmlFileNameStr[1, 114] = "Pixel_XPickupTeachPosition_RB_S3";
                _ctrlNameAndXmlFileNameStr[1, 115] = "Pixel_YPickupTeachPosition_RB_S3";
                _ctrlNameAndXmlFileNameStr[1, 116] = "Pixel_AnglePickupTeach_S3";

                _ctrlNameAndXmlFileNameStr[1, 117] = "Pixel_XPickupCurrentPositionLimitMin_RT_S3";
                _ctrlNameAndXmlFileNameStr[1, 118] = "Pixel_XPickupCurrentPositionLimitMax_RT_S3";
                _ctrlNameAndXmlFileNameStr[1, 119] = "Pixel_YPickupCurrentPositionLimitMin_RT_S3";
                _ctrlNameAndXmlFileNameStr[1, 120] = "Pixel_YPickupCurrentPositionLimitMax_RT_S3";
                _ctrlNameAndXmlFileNameStr[1, 121] = "Pixel_AnglePickupCurrentLimitMin_S3";
                _ctrlNameAndXmlFileNameStr[1, 122] = "Pixel_AnglePickupCurrentLimitMax_S3";
                _ctrlNameAndXmlFileNameStr[1, 123] = "ROB_XPickupCurrentPositionLimitMin_S3";
                _ctrlNameAndXmlFileNameStr[1, 124] = "ROB_XPickupCurrentPositionLimitMax_S3";
                _ctrlNameAndXmlFileNameStr[1, 125] = "ROB_YPickupCurrentPositionLimitMin_S3";
                _ctrlNameAndXmlFileNameStr[1, 126] = "ROB_YPickupCurrentPositionLimitMax_S3";
                _ctrlNameAndXmlFileNameStr[1, 127] = "ROB_RZPickupCurrentPositionLimitMin_S3";
                _ctrlNameAndXmlFileNameStr[1, 128] = "ROB_RZPickupCurrentPositionLimitMax_S3";

                _ctrlNameAndXmlFileNameStr[1, 129] = "ThresholdMinGray_S3";
                _ctrlNameAndXmlFileNameStr[1, 130] = "ThresholdMaxGray_S3";
                _ctrlNameAndXmlFileNameStr[1, 131] = "OpeningWAndH_S3";
                _ctrlNameAndXmlFileNameStr[1, 132] = "ShapeMinArea_S3";
                _ctrlNameAndXmlFileNameStr[1, 133] = "ShapeMaxArea_S3";
                _ctrlNameAndXmlFileNameStr[1, 134] = "ShapeMinRectangularity_S3";
                _ctrlNameAndXmlFileNameStr[1, 135] = "ShapeMaxRectangularity_S3";
                _ctrlNameAndXmlFileNameStr[1, 136] = "DilationRectangleWidth_S3";
                _ctrlNameAndXmlFileNameStr[1, 137] = "DilationRectangleHeight_S3";
                _ctrlNameAndXmlFileNameStr[1, 138] = "EdgesSubPixCannyAlpha_S3";
                _ctrlNameAndXmlFileNameStr[1, 139] = "EdgesSubPixCannyLow_S3";
                _ctrlNameAndXmlFileNameStr[1, 140] = "EdgesSubPixCannyHigh_S3";
                _ctrlNameAndXmlFileNameStr[1, 141] = "UnionCollinearContoursXldMaxDistAbs_S3";
                _ctrlNameAndXmlFileNameStr[1, 142] = "UnionCollinearContoursXldMaxDistRel_S3";
                _ctrlNameAndXmlFileNameStr[1, 143] = "UnionCollinearContoursXldMaxShift_S3";
                _ctrlNameAndXmlFileNameStr[1, 144] = "UnionCollinearContoursXldMaxAngle_S3";
                _ctrlNameAndXmlFileNameStr[1, 145] = "XldContlengthMin_S3";
                _ctrlNameAndXmlFileNameStr[1, 146] = "XldContlengthMax_S3";
                _ctrlNameAndXmlFileNameStr[1, 147] = "XLDMinArea_S3";
                _ctrlNameAndXmlFileNameStr[1, 148] = "XLDMaxArea_S3";
                _ctrlNameAndXmlFileNameStr[1, 149] = "XLDMinRectangularity_S3";
                _ctrlNameAndXmlFileNameStr[1, 150] = "XLDMaxRectangularity_S3";
                _ctrlNameAndXmlFileNameStr[1, 151] = "UnionAdjacentContoursXldMaxDistAbs_S3";
                _ctrlNameAndXmlFileNameStr[1, 152] = "UnionAdjacentContoursXldMaxDistRel_S3";

                _ctrlNameAndXmlFileNameStr[1, 153] = "ClassificationThresholdMin_S3";
                _ctrlNameAndXmlFileNameStr[1, 154] = "ClassificationThresholdMax_S3";
                _ctrlNameAndXmlFileNameStr[1, 155] = "ProductAreaMin_S3";
                _ctrlNameAndXmlFileNameStr[1, 156] = "ProductAreaMax_S3";
                _ctrlNameAndXmlFileNameStr[1, 157] = "CoverAreaMin_S3";
                _ctrlNameAndXmlFileNameStr[1, 158] = "CoverAreaMax_S3";
                _ctrlNameAndXmlFileNameStr[1, 159] = "EmptyAreaMin_S3";
                _ctrlNameAndXmlFileNameStr[1, 160] = "EmptyAreaMax_S3";
                _ctrlNameAndXmlFileNameStr[1, 161] = "ClassificationThresholdCoverMin_S3";
                _ctrlNameAndXmlFileNameStr[1, 162] = "ClassificationThresholdCoverMax_S3";

                _ctrlNameAndXmlFileNameStr[1, 163] = "AngleRotate_S1";
                _ctrlNameAndXmlFileNameStr[1, 164] = "AngleRotate_S2";
                _ctrlNameAndXmlFileNameStr[1, 165] = "AngleRotate_S3";
                #endregion

                #region 控件对应的变量类型
                //Station1
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
                _ctrlNameAndXmlFileNameStr[2, 31] = "float";
                _ctrlNameAndXmlFileNameStr[2, 32] = "float";
                _ctrlNameAndXmlFileNameStr[2, 33] = "float";
                _ctrlNameAndXmlFileNameStr[2, 34] = "float";
                _ctrlNameAndXmlFileNameStr[2, 35] = "float";
                _ctrlNameAndXmlFileNameStr[2, 36] = "float";
                _ctrlNameAndXmlFileNameStr[2, 37] = "float";
                _ctrlNameAndXmlFileNameStr[2, 38] = "float";
                _ctrlNameAndXmlFileNameStr[2, 39] = "float";
                _ctrlNameAndXmlFileNameStr[2, 40] = "float";
                _ctrlNameAndXmlFileNameStr[2, 41] = "float";
                _ctrlNameAndXmlFileNameStr[2, 42] = "float";
                _ctrlNameAndXmlFileNameStr[2, 43] = "float";
                _ctrlNameAndXmlFileNameStr[2, 44] = "float";
                _ctrlNameAndXmlFileNameStr[2, 45] = "float";
                _ctrlNameAndXmlFileNameStr[2, 46] = "float";
                _ctrlNameAndXmlFileNameStr[2, 47] = "float";
                _ctrlNameAndXmlFileNameStr[2, 48] = "float";
                _ctrlNameAndXmlFileNameStr[2, 49] = "float";
                _ctrlNameAndXmlFileNameStr[2, 50] = "float";
                //Station2
                _ctrlNameAndXmlFileNameStr[2, 51] = "float";
                _ctrlNameAndXmlFileNameStr[2, 52] = "float";
                _ctrlNameAndXmlFileNameStr[2, 53] = "float";
                _ctrlNameAndXmlFileNameStr[2, 54] = "float";
                _ctrlNameAndXmlFileNameStr[2, 55] = "float";
                _ctrlNameAndXmlFileNameStr[2, 56] = "float";
                _ctrlNameAndXmlFileNameStr[2, 57] = "float";
                _ctrlNameAndXmlFileNameStr[2, 58] = "float";
                _ctrlNameAndXmlFileNameStr[2, 59] = "float";
                _ctrlNameAndXmlFileNameStr[2, 60] = "float";
                _ctrlNameAndXmlFileNameStr[2, 61] = "float";
                _ctrlNameAndXmlFileNameStr[2, 62] = "float";
                _ctrlNameAndXmlFileNameStr[2, 63] = "float";
                _ctrlNameAndXmlFileNameStr[2, 64] = "float";
                _ctrlNameAndXmlFileNameStr[2, 65] = "float";
                _ctrlNameAndXmlFileNameStr[2, 66] = "float";
                _ctrlNameAndXmlFileNameStr[2, 67] = "float";
                _ctrlNameAndXmlFileNameStr[2, 68] = "float";
                _ctrlNameAndXmlFileNameStr[2, 69] = "float";
                _ctrlNameAndXmlFileNameStr[2, 70] = "float";
                _ctrlNameAndXmlFileNameStr[2, 71] = "float";
                _ctrlNameAndXmlFileNameStr[2, 72] = "float";
                _ctrlNameAndXmlFileNameStr[2, 73] = "float";
                _ctrlNameAndXmlFileNameStr[2, 74] = "float";
                _ctrlNameAndXmlFileNameStr[2, 75] = "float";
                _ctrlNameAndXmlFileNameStr[2, 76] = "float";
                _ctrlNameAndXmlFileNameStr[2, 77] = "float";
                _ctrlNameAndXmlFileNameStr[2, 78] = "float";
                _ctrlNameAndXmlFileNameStr[2, 79] = "float";
                _ctrlNameAndXmlFileNameStr[2, 80] = "float";
                _ctrlNameAndXmlFileNameStr[2, 81] = "float";
                _ctrlNameAndXmlFileNameStr[2, 82] = "float";
                _ctrlNameAndXmlFileNameStr[2, 83] = "float";
                _ctrlNameAndXmlFileNameStr[2, 84] = "float";
                _ctrlNameAndXmlFileNameStr[2, 85] = "float";
                _ctrlNameAndXmlFileNameStr[2, 86] = "float";
                _ctrlNameAndXmlFileNameStr[2, 87] = "float";
                _ctrlNameAndXmlFileNameStr[2, 88] = "float";
                _ctrlNameAndXmlFileNameStr[2, 89] = "float";
                _ctrlNameAndXmlFileNameStr[2, 90] = "float";
                _ctrlNameAndXmlFileNameStr[2, 91] = "float";
                _ctrlNameAndXmlFileNameStr[2, 92] = "float";
                _ctrlNameAndXmlFileNameStr[2, 93] = "float";
                _ctrlNameAndXmlFileNameStr[2, 94] = "float";
                _ctrlNameAndXmlFileNameStr[2, 95] = "float";
                _ctrlNameAndXmlFileNameStr[2, 96] = "float";
                _ctrlNameAndXmlFileNameStr[2, 97] = "float";
                _ctrlNameAndXmlFileNameStr[2, 98] = "float";
                _ctrlNameAndXmlFileNameStr[2, 99] = "float";
                _ctrlNameAndXmlFileNameStr[2, 100] = "float";
                _ctrlNameAndXmlFileNameStr[2, 101] = "float";
                //Station3
                _ctrlNameAndXmlFileNameStr[2, 102] = "float";
                _ctrlNameAndXmlFileNameStr[2, 103] = "float";
                _ctrlNameAndXmlFileNameStr[2, 104] = "float";
                _ctrlNameAndXmlFileNameStr[2, 105] = "float";
                _ctrlNameAndXmlFileNameStr[2, 106] = "float";
                _ctrlNameAndXmlFileNameStr[2, 107] = "float";
                _ctrlNameAndXmlFileNameStr[2, 108] = "float";
                _ctrlNameAndXmlFileNameStr[2, 109] = "float";
                _ctrlNameAndXmlFileNameStr[2, 110] = "float";
                _ctrlNameAndXmlFileNameStr[2, 111] = "float";
                _ctrlNameAndXmlFileNameStr[2, 112] = "float";
                _ctrlNameAndXmlFileNameStr[2, 113] = "float";
                _ctrlNameAndXmlFileNameStr[2, 114] = "float";
                _ctrlNameAndXmlFileNameStr[2, 115] = "float";
                _ctrlNameAndXmlFileNameStr[2, 116] = "float";
                _ctrlNameAndXmlFileNameStr[2, 117] = "float";
                _ctrlNameAndXmlFileNameStr[2, 118] = "float";
                _ctrlNameAndXmlFileNameStr[2, 119] = "float";
                _ctrlNameAndXmlFileNameStr[2, 120] = "float";
                _ctrlNameAndXmlFileNameStr[2, 121] = "float";
                _ctrlNameAndXmlFileNameStr[2, 122] = "float";
                _ctrlNameAndXmlFileNameStr[2, 123] = "float";
                _ctrlNameAndXmlFileNameStr[2, 124] = "float";
                _ctrlNameAndXmlFileNameStr[2, 125] = "float";
                _ctrlNameAndXmlFileNameStr[2, 126] = "float";
                _ctrlNameAndXmlFileNameStr[2, 127] = "float";
                _ctrlNameAndXmlFileNameStr[2, 128] = "float";
                _ctrlNameAndXmlFileNameStr[2, 129] = "float";
                _ctrlNameAndXmlFileNameStr[2, 130] = "float";
                _ctrlNameAndXmlFileNameStr[2, 131] = "float";
                _ctrlNameAndXmlFileNameStr[2, 132] = "float";
                _ctrlNameAndXmlFileNameStr[2, 133] = "float";
                _ctrlNameAndXmlFileNameStr[2, 134] = "float";
                _ctrlNameAndXmlFileNameStr[2, 135] = "float";
                _ctrlNameAndXmlFileNameStr[2, 136] = "float";
                _ctrlNameAndXmlFileNameStr[2, 137] = "float";
                _ctrlNameAndXmlFileNameStr[2, 138] = "float";
                _ctrlNameAndXmlFileNameStr[2, 139] = "float";
                _ctrlNameAndXmlFileNameStr[2, 140] = "float";
                _ctrlNameAndXmlFileNameStr[2, 141] = "float";
                _ctrlNameAndXmlFileNameStr[2, 142] = "float";
                _ctrlNameAndXmlFileNameStr[2, 143] = "float";
                _ctrlNameAndXmlFileNameStr[2, 144] = "float";
                _ctrlNameAndXmlFileNameStr[2, 145] = "float";
                _ctrlNameAndXmlFileNameStr[2, 146] = "float";
                _ctrlNameAndXmlFileNameStr[2, 147] = "float";
                _ctrlNameAndXmlFileNameStr[2, 148] = "float";
                _ctrlNameAndXmlFileNameStr[2, 149] = "float";
                _ctrlNameAndXmlFileNameStr[2, 150] = "float";
                _ctrlNameAndXmlFileNameStr[2, 151] = "float";
                _ctrlNameAndXmlFileNameStr[2, 152] = "float";

                _ctrlNameAndXmlFileNameStr[2, 153] = "float";
                _ctrlNameAndXmlFileNameStr[2, 154] = "float";
                _ctrlNameAndXmlFileNameStr[2, 155] = "float";
                _ctrlNameAndXmlFileNameStr[2, 156] = "float";
                _ctrlNameAndXmlFileNameStr[2, 157] = "float";
                _ctrlNameAndXmlFileNameStr[2, 158] = "float";
                _ctrlNameAndXmlFileNameStr[2, 159] = "float";
                _ctrlNameAndXmlFileNameStr[2, 160] = "float";
                _ctrlNameAndXmlFileNameStr[2, 161] = "float";
                _ctrlNameAndXmlFileNameStr[2, 162] = "float";

                _ctrlNameAndXmlFileNameStr[2, 163] = "bool";
                _ctrlNameAndXmlFileNameStr[2, 164] = "bool";
                _ctrlNameAndXmlFileNameStr[2, 165] = "bool";
                #endregion
            }
            catch (Exception ex)
            {
                MessageBox.Show("ProductRecipe的数组_ctrlNameAndXmlFileNameStr初始化失败! " + ex.ToString());
            }
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        public ProductRecipe()
        {
            InitialCtrlBoxCorrespondingStringArr();
        }
        #endregion

        #region ProductDataLoadToVariate:读取数据至变量中
        /// <summary>
        /// ProductDataLoadToVariate:读取数据至变量中
        /// </summary>
        /// <param name="FilePath">string:需要读取的rcp文件全路径</param>
        /// <returns>bool: true-读取成功，false-读取失败</returns>
        public bool ProductDataLoadToVariate(string filePath)
        {
            MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据至变量" + "----用户：" + ActionForm._operatorName,
            ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);

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
                            try
                            {
                                MyTool.DataProcessing.SetDataToOneVariateBaseVariateStringName(_ctrlNameAndXmlFileNameStr[1, index], _ctrlNameAndXmlFileNameStr[2, index], this, element.InnerText);
                            }
                            catch
                            {
                                MessageBox.Show("ProductRecipe里变量名数组_ctrlNameAndXmlFileNameStr错误,请检查并修改代码!");
                            }
                        }
                        else if (_ctrlNameAndXmlFileNameStr[2, index] == "bool")
                        {
                            tempValue = element.InnerText.ToString().ToLower();
                            if (tempValue == "true")
                            {
                                try
                                {
                                    MyTool.DataProcessing.SetDataToOneVariateBaseVariateStringName(_ctrlNameAndXmlFileNameStr[1, index], _ctrlNameAndXmlFileNameStr[2, index], this, "1");
                                }
                                catch
                                {
                                    MessageBox.Show("ProductRecipe里变量名数组_ctrlNameAndXmlFileNameStr错误,请检查并修改代码!");
                                }
                            }
                            else
                            {
                                try
                                {
                                    MyTool.DataProcessing.SetDataToOneVariateBaseVariateStringName(_ctrlNameAndXmlFileNameStr[1, index], _ctrlNameAndXmlFileNameStr[2, index], this, "0");
                                }
                                catch
                                {
                                    MessageBox.Show("ProductRecipe里变量名数组_ctrlNameAndXmlFileNameStr错误,请检查并修改代码!");
                                }
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
                //if (loadUsefulNodeQuantity < LOAD_SAVE_DATA_ELEMENT_QUANTITY)
                //{
                //    for (int i = 0; i < LOAD_SAVE_DATA_ELEMENT_QUANTITY; i++)
                //    {
                //        if ((!SettingsAdministratorForm._checkProductIsConnector && (!(i >= 62 && i <= 71))) ||
                //            SettingsAdministratorForm._checkProductIsConnector)
                //        {
                //            if (!dataLoadReultFlag[i])
                //            {
                //                MessageBox.Show("在产品参数文件" + filePath.Substring(filePath.LastIndexOf("\\") + 1, filePath.Length - filePath.LastIndexOf("\\") - 1) + "中，无法找到参数" + _ctrlNameAndXmlFileNameStr[1, i] + "的数据，读取产品参数至变量中失败！");
                //                loadResult = false;
                //            }
                //        }
                //    }
                //}
            }

            //if (!LoadPointPositionData(filePath.Substring(0, filePath.LastIndexOf(".")) + ".csv"))
            //{
            //    loadResult = false;
            //}

            //获取的参数检查----用于读取数据至变量中
            if (!LoadProductDataCheck())
            {
                loadResult = false;
            }

            _parameterLoadSuccessfulFlag = loadResult;

            return _parameterLoadSuccessfulFlag;
        }
        #endregion

        #region 获取的参数检查----用于读取数据至变量中
        /// <summary>
        /// LoadProductDataCheck:获取的参数检查----用于读取数据至变量中
        /// </summary>
        /// <returns></returns>
        private bool LoadProductDataCheck()
        {
            MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查" + "----用户：" + ActionForm._operatorName,
            ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);

            bool dataCheckResult = true;//false:Check failed,true:check successful

            //Station1
            if (ROB_XPickupTeachPosition_S1 < -2000 || ROB_XPickupTeachPosition_S1 > 2000)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，ROB_XPickupTeachPosition_S1错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，ROB_XPickupTeachPosition_S1错误！");
                dataCheckResult = false;
            }
            if (ROB_YPickupTeachPosition_S1 < -2000 || ROB_YPickupTeachPosition_S1 > 2000)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，ROB_YPickupTeachPosition_S1错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，ROB_YPickupTeachPosition_S1错误！");
                dataCheckResult = false;
            }
            if (ROB_ZPickupTeachPosition_S1 < -1000 || ROB_ZPickupTeachPosition_S1 > 1000)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，ROB_ZPickupTeachPosition_S1错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，ROB_ZPickupTeachPosition_S1错误！");
                dataCheckResult = false;
            }
            if (ROB_RXPickupTeachPosition_S1 < -360 || ROB_RXPickupTeachPosition_S1 > 360)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，ROB_RXPickupTeachPosition_S1错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，ROB_RXPickupTeachPosition_S1错误！");
                dataCheckResult = false;
            }
            if (ROB_RWPickupTeachPosition_S1 < -360 || ROB_RWPickupTeachPosition_S1 > 360)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，ROB_RWPickupTeachPosition_S1错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，ROB_RWPickupTeachPosition_S1错误！");
                dataCheckResult = false;
            }
            if (ROB_RZPickupTeachPosition_S1 < -360 || ROB_RZPickupTeachPosition_S1 > 360)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，ROB_RZPickupTeachPosition_S1错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，ROB_RZPickupTeachPosition_S1错误！");
                dataCheckResult = false;
            }

            if (Pixel_XPickupTeachPosition_RT_S1 < 0 || Pixel_XPickupTeachPosition_RT_S1 > 5000)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，Pixel_XPickupTeachPosition_RT_S1错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，Pixel_XPickupTeachPosition_RT_S1错误！");
                dataCheckResult = false;
            }
            if (Pixel_YPickupTeachPosition_RT_S1 < 0 || Pixel_YPickupTeachPosition_RT_S1 > 5000)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，Pixel_YPickupTeachPosition_RT_S1错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，Pixel_YPickupTeachPosition_RT_S1错误！");
                dataCheckResult = false;
            }
            if (Pixel_XPickupTeachPosition_LT_S1 < 0 || Pixel_XPickupTeachPosition_LT_S1 > 5000)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，Pixel_XPickupTeachPosition_LT_S1错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，Pixel_XPickupTeachPosition_LT_S1错误！");
                dataCheckResult = false;
            }
            if (Pixel_YPickupTeachPosition_LT_S1 < 0 || Pixel_YPickupTeachPosition_LT_S1 > 5000)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，Pixel_YPickupTeachPosition_LT_S1错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，Pixel_YPickupTeachPosition_LT_S1错误！");
                dataCheckResult = false;
            }
            if (Pixel_XPickupTeachPosition_LB_S1 < 0 || Pixel_XPickupTeachPosition_LB_S1 > 5000)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，Pixel_XPickupTeachPosition_LB_S1错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，Pixel_XPickupTeachPosition_LB_S1错误！");
                dataCheckResult = false;
            }
            if (Pixel_YPickupTeachPosition_LB_S1 < 0 || Pixel_YPickupTeachPosition_LB_S1 > 5000)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，Pixel_YPickupTeachPosition_LB_S1错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，Pixel_YPickupTeachPosition_LB_S1错误！");
                dataCheckResult = false;
            }
            if (Pixel_XPickupTeachPosition_RB_S1 < 0 || Pixel_XPickupTeachPosition_RB_S1 > 5000)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，Pixel_XPickupTeachPosition_RB_S1错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，Pixel_XPickupTeachPosition_RB_S1错误！");
                dataCheckResult = false;
            }
            if (Pixel_YPickupTeachPosition_RB_S1 < 0 || Pixel_YPickupTeachPosition_RB_S1 > 5000)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，Pixel_YPickupTeachPosition_RB_S1错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，Pixel_YPickupTeachPosition_RB_S1错误！");
                dataCheckResult = false;
            }
            if (Pixel_AnglePickupTeach_S1 < -360 || Pixel_AnglePickupTeach_S1 > 360)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，Pixel_AnglPickupTeache错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，Pixel_AnglePickupTeach_S1错误！");
                dataCheckResult = false;
            }

            if (Pixel_XPickupCurrentPositionLimitMax_RT_S1 < Pixel_XPickupCurrentPositionLimitMin_RT_S1)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，Pixel_XPickupCurrentPositionLimitMax_RT_S1 < Pixel_XPickupCurrentPositionLimitMin_RT_S1错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，Pixel_XPickupCurrentPositionLimitMax_RT_S1 < Pixel_XPickupCurrentPositionLimitMin_RT_S1错误！");
                dataCheckResult = false;
            }
            if (Pixel_YPickupCurrentPositionLimitMax_RT_S1 < Pixel_YPickupCurrentPositionLimitMin_RT_S1)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，Pixel_YPickupCurrentPositionLimitMax_RT_S1 < Pixel_YPickupCurrentPositionLimitMin_RT_S1错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，Pixel_YPickupCurrentPositionLimitMax_RT_S1 < Pixel_YPickupCurrentPositionLimitMin_RT_S1错误！");
                dataCheckResult = false;
            }
            if (Pixel_AnglePickupCurrentLimitMax_S1 < Pixel_AnglePickupCurrentLimitMin_S1)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，Pixel_AnglePickupCurrentLimitMax_S1 < Pixel_AnglePickupCurrentLimitMin_S1错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，Pixel_AnglePickupCurrentLimitMax_S1 < Pixel_AnglePickupCurrentLimitMin_S1错误！");
                dataCheckResult = false;
            }
            if (ROB_XPickupCurrentPositionLimitMax_S1 < ROB_XPickupCurrentPositionLimitMin_S1)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，ROB_XPickupCurrentPositionLimitMax_S1 < ROB_XPickupCurrentPositionLimitMin_S1错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，ROB_XPickupCurrentPositionLimitMax_S1 < ROB_XPickupCurrentPositionLimitMin_S1错误！");
                dataCheckResult = false;
            }
            if (ROB_YPickupCurrentPositionLimitMax_S1 < ROB_YPickupCurrentPositionLimitMin_S1)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，ROB_YPickupCurrentPositionLimitMax_S1 < ROB_YPickupCurrentPositionLimitMin_S1错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，ROB_YPickupCurrentPositionLimitMax_S1 < ROB_YPickupCurrentPositionLimitMin_S1错误！");
                dataCheckResult = false;
            }
            if (ROB_RZPickupCurrentPositionLimitMax_S1 < ROB_RZPickupCurrentPositionLimitMin_S1)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，ROB_RZPickupCurrentPositionLimitMax_S1 < ROB_RZPickupCurrentPositionLimitMin_S1错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，ROB_RZPickupCurrentPositionLimitMax_S1 < ROB_RZPickupCurrentPositionLimitMin_S1错误！");
                dataCheckResult = false;
            }

            if (ThresholdMinGray_S1 < 0 || ThresholdMinGray_S1 > 255)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，ThresholdMinGray_S1错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，ThresholdMinGray_S1错误！");
                dataCheckResult = false;
            }
            if (ThresholdMaxGray_S1 < 5 || ThresholdMaxGray_S1 > 255)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，ThresholdMaxGray_S1错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，ThresholdMaxGray_S1错误！");
                dataCheckResult = false;
            }
            if (ThresholdMaxGray_S1 < ThresholdMinGray_S1)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，ThresholdMaxGray_S1 < ThresholdMinGray_S1错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，ThresholdMaxGray_S1 < ThresholdMinGray_S1错误！");
                dataCheckResult = false;
            }

            if (OpeningWAndH_S1 < 1 || OpeningWAndH_S1 > 511)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，OpeningWAndH_S1错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，OpeningWAndH_S1错误！");
                dataCheckResult = false;
            }

            if (ShapeMinArea_S1 < 0 || ShapeMinArea_S1 > 100000000)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，ShapeMinArea_S1错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，ShapeMinArea_S1错误！");
                dataCheckResult = false;
            }
            if (ShapeMaxArea_S1 < 0 || ShapeMaxArea_S1 > 100000000)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，ShapeMaxArea_S1错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，ShapeMaxArea_S1错误！");
                dataCheckResult = false;
            }
            if (ShapeMaxArea_S1 < ShapeMinArea_S1)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，ShapeMaxArea_S1 < ShapeMinArea_S1错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，ShapeMaxArea_S1 < ShapeMinArea_S1错误！");
                dataCheckResult = false;
            }
            if (ShapeMinRectangularity_S1 < 0 || ShapeMinRectangularity_S1 > 1)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，ShapeMinRectangularity_S1错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，ShapeMinRectangularity_S1错误！");
                dataCheckResult = false;
            }
            if (ShapeMaxRectangularity_S1 < 0 || ShapeMaxRectangularity_S1 > 1)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，ShapeMaxRectangularity_S1错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，ShapeMaxRectangularity_S1错误！");
                dataCheckResult = false;
            }
            if (ShapeMaxRectangularity_S1 < ShapeMinRectangularity_S1)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，ShapeMaxRectangularity_S1 < ShapeMinRectangularity_S1错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，ShapeMaxRectangularity_S1 < ShapeMinRectangularity_S1错误！");
                dataCheckResult = false;
            }
            if (DilationRectangleWidth_S1 < 1 || DilationRectangleWidth_S1 > 511)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，DilationRectangleWidth_S1错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，DilationRectangleWidth_S1错误！");
                dataCheckResult = false;
            }
            if (DilationRectangleHeight_S1 < 1 || DilationRectangleHeight_S1 > 511)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，DilationRectangleHeight_S1错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，DilationRectangleHeight_S1错误！");
                dataCheckResult = false;
            }
            if (EdgesSubPixCannyAlpha_S1 < 0.2 || EdgesSubPixCannyAlpha_S1 > 50)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，EdgesSubPixCannyAlpha_S1错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，EdgesSubPixCannyAlpha_S1错误！");
                dataCheckResult = false;
            }
            if (EdgesSubPixCannyLow_S1 < 1 || EdgesSubPixCannyLow_S1 > 255)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，EdgesSubPixCannyLow_S1错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，EdgesSubPixCannyLow_S1错误！");
                dataCheckResult = false;
            }
            if (EdgesSubPixCannyHigh_S1 < 1 || EdgesSubPixCannyHigh_S1 > 255)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，EdgesSubPixCannyHigh_S1错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，EdgesSubPixCannyHigh_S1错误！");
                dataCheckResult = false;
            }
            if (EdgesSubPixCannyHigh_S1 < EdgesSubPixCannyLow_S1)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，EdgesSubPixCannyHigh_S1 < EdgesSubPixCannyLow_S1错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，EdgesSubPixCannyHigh_S1 < EdgesSubPixCannyLow_S1错误！");
                dataCheckResult = false;
            }


            if (UnionCollinearContoursXldMaxDistAbs_S1 < 0 || UnionCollinearContoursXldMaxDistAbs_S1 > 100000000)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，UnionCollinearContoursXldMaxDistAbs_S1错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，UnionCollinearContoursXldMaxDistAbs_S1错误！");
                dataCheckResult = false;
            }
            if (UnionCollinearContoursXldMaxDistRel_S1 < 0 || UnionCollinearContoursXldMaxDistRel_S1 > 100000000)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，UnionCollinearContoursXldMaxDistRel_S1错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，UnionCollinearContoursXldMaxDistRel_S1错误！");
                dataCheckResult = false;
            }
            if (UnionCollinearContoursXldMaxShift_S1 < 0 || UnionCollinearContoursXldMaxShift_S1 > 100000000)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，UnionCollinearContoursXldMaxShift_S1错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，UnionCollinearContoursXldMaxShift_S1错误！");
                dataCheckResult = false;
            }
            if (UnionCollinearContoursXldMaxAngle_S1 < 0 || UnionCollinearContoursXldMaxAngle_S1 > 0.78539816339)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，UnionCollinearContoursXldMaxAngle_S1错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，UnionCollinearContoursXldMaxAngle_S1错误！");
                dataCheckResult = false;
            }


            if (XldContlengthMin_S1 < 0 || XldContlengthMin_S1 > 100000000)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，XldContlengthMin_S1错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，XldContlengthMin_S1错误！");
                dataCheckResult = false;
            }
            if (XldContlengthMax_S1 < 0 || XldContlengthMax_S1 > 100000000)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，XldContlengthMax_S1错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，XldContlengthMax_S1错误！");
                dataCheckResult = false;
            }
            if (XldContlengthMax_S1 < XldContlengthMin_S1)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，XldContlengthMax_S1 < XldContlengthMin_S1错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，XldContlengthMax_S1 < XldContlengthMin_S1错误！");
                dataCheckResult = false;
            }
            if (XLDMinArea_S1 < 0 || XLDMinArea_S1 > 100000000)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，XLDMinArea_S1错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，XLDMinArea_S1错误！");
                dataCheckResult = false;
            }
            if (XLDMaxArea_S1 < 0 || XLDMaxArea_S1 > 100000000)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，XLDMaxArea_S1错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，XLDMaxArea_S1错误！");
                dataCheckResult = false;
            }
            if (XLDMaxArea_S1 < XLDMinArea_S1)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，XLDMaxArea_S1 < XLDMinArea_S1错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，XLDMaxArea_S1 < XLDMinArea_S1错误！");
                dataCheckResult = false;
            }
            if (XLDMinRectangularity_S1 < 0 || XLDMinRectangularity_S1 > 1)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，XLDMinRectangularity_S1错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，XLDMinRectangularity_S1错误！");
                dataCheckResult = false;
            }
            if (XLDMaxRectangularity_S1 < 0 || XLDMaxRectangularity_S1 > 1)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，XLDMaxRectangularity_S1错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，XLDMaxRectangularity_S1错误！");
                dataCheckResult = false;
            }
            if (XLDMaxRectangularity_S1 < XLDMinRectangularity_S1)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，XLDMaxRectangularity_S1 < XLDMinRectangularity_S1错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，XLDMaxRectangularity_S1 < XLDMinRectangularity_S1错误！");
                dataCheckResult = false;
            }

            if (UnionAdjacentContoursXldMaxDistAbs_S1 < 0 || UnionAdjacentContoursXldMaxDistAbs_S1 > 100000000)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，UnionAdjacentContoursXldMaxDistAbs_S1错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，UnionAdjacentContoursXldMaxDistAbs_S1错误！");
                dataCheckResult = false;
            }
            if (UnionAdjacentContoursXldMaxDistRel_S1 < 0 || UnionAdjacentContoursXldMaxDistRel_S1 > 100000000)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，UnionAdjacentContoursXldMaxDistRel_S1错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，UnionAdjacentContoursXldMaxDistRel_S1错误！");
                dataCheckResult = false;
            }

            //Station2
            if (ROB_XPickupTeachPosition_S2 < -2000 || ROB_XPickupTeachPosition_S2 > 2000)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，ROB_XPickupTeachPosition_S2错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，ROB_XPickupTeachPosition_S2错误！");
                dataCheckResult = false;
            }
            if (ROB_YPickupTeachPosition_S2 < -2000 || ROB_YPickupTeachPosition_S2 > 2000)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，ROB_YPickupTeachPosition_S2错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，ROB_YPickupTeachPosition_S2错误！");
                dataCheckResult = false;
            }
            if (ROB_ZPickupTeachPosition_S2 < -1000 || ROB_ZPickupTeachPosition_S2 > 1000)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，ROB_ZPickupTeachPosition_S2错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，ROB_ZPickupTeachPosition_S2错误！");
                dataCheckResult = false;
            }
            if (ROB_RXPickupTeachPosition_S2 < -360 || ROB_RXPickupTeachPosition_S2 > 360)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，ROB_RXPickupTeachPosition_S2错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，ROB_RXPickupTeachPosition_S2错误！");
                dataCheckResult = false;
            }
            if (ROB_RWPickupTeachPosition_S2 < -360 || ROB_RWPickupTeachPosition_S2 > 360)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，ROB_RWPickupTeachPosition_S2错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，ROB_RWPickupTeachPosition_S2错误！");
                dataCheckResult = false;
            }
            if (ROB_RZPickupTeachPosition_S2 < -360 || ROB_RZPickupTeachPosition_S2 > 360)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，ROB_RZPickupTeachPosition_S2错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，ROB_RZPickupTeachPosition_S2错误！");
                dataCheckResult = false;
            }

            if (Pixel_XPickupTeachPosition_RT_S2 < 0 || Pixel_XPickupTeachPosition_RT_S2 > 5000)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，Pixel_XPickupTeachPosition_RT_S2错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，Pixel_XPickupTeachPosition_RT_S2错误！");
                dataCheckResult = false;
            }
            if (Pixel_YPickupTeachPosition_RT_S2 < 0 || Pixel_YPickupTeachPosition_RT_S2 > 5000)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，Pixel_YPickupTeachPosition_RT_S2错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，Pixel_YPickupTeachPosition_RT_S2错误！");
                dataCheckResult = false;
            }
            if (Pixel_XPickupTeachPosition_LT_S2 < 0 || Pixel_XPickupTeachPosition_LT_S2 > 5000)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，Pixel_XPickupTeachPosition_LT_S2错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，Pixel_XPickupTeachPosition_LT_S2错误！");
                dataCheckResult = false;
            }
            if (Pixel_YPickupTeachPosition_LT_S2 < 0 || Pixel_YPickupTeachPosition_LT_S2 > 5000)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，Pixel_YPickupTeachPosition_LT_S2错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，Pixel_YPickupTeachPosition_LT_S2错误！");
                dataCheckResult = false;
            }
            if (Pixel_XPickupTeachPosition_LB_S2 < 0 || Pixel_XPickupTeachPosition_LB_S2 > 5000)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，Pixel_XPickupTeachPosition_LB_S2错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，Pixel_XPickupTeachPosition_LB_S2错误！");
                dataCheckResult = false;
            }
            if (Pixel_YPickupTeachPosition_LB_S2 < 0 || Pixel_YPickupTeachPosition_LB_S2 > 5000)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，Pixel_YPickupTeachPosition_LB_S2错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，Pixel_YPickupTeachPosition_LB_S2错误！");
                dataCheckResult = false;
            }
            if (Pixel_XPickupTeachPosition_RB_S2 < 0 || Pixel_XPickupTeachPosition_RB_S2 > 5000)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，Pixel_XPickupTeachPosition_RB_S2错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，Pixel_XPickupTeachPosition_RB_S2错误！");
                dataCheckResult = false;
            }
            if (Pixel_YPickupTeachPosition_RB_S2 < 0 || Pixel_YPickupTeachPosition_RB_S2 > 5000)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，Pixel_YPickupTeachPosition_RB_S2错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，Pixel_YPickupTeachPosition_RB_S2错误！");
                dataCheckResult = false;
            }
            if (Pixel_AnglePickupTeach_S2 < -360 || Pixel_AnglePickupTeach_S2 > 360)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，Pixel_AnglPickupTeache错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，Pixel_AnglePickupTeach_S2错误！");
                dataCheckResult = false;
            }

            if (Pixel_XPickupCurrentPositionLimitMax_RT_S2 < Pixel_XPickupCurrentPositionLimitMin_RT_S2)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，Pixel_XPickupCurrentPositionLimitMax_RT_S2 < Pixel_XPickupCurrentPositionLimitMin_RT_S2错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，Pixel_XPickupCurrentPositionLimitMax_RT_S2 < Pixel_XPickupCurrentPositionLimitMin_RT_S2错误！");
                dataCheckResult = false;
            }
            if (Pixel_YPickupCurrentPositionLimitMax_RT_S2 < Pixel_YPickupCurrentPositionLimitMin_RT_S2)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，Pixel_YPickupCurrentPositionLimitMax_RT_S2 < Pixel_YPickupCurrentPositionLimitMin_RT_S2错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，Pixel_YPickupCurrentPositionLimitMax_RT_S2 < Pixel_YPickupCurrentPositionLimitMin_RT_S2错误！");
                dataCheckResult = false;
            }
            if (Pixel_AnglePickupCurrentLimitMax_S2 < Pixel_AnglePickupCurrentLimitMin_S2)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，Pixel_AnglePickupCurrentLimitMax_S2 < Pixel_AnglePickupCurrentLimitMin_S2错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，Pixel_AnglePickupCurrentLimitMax_S2 < Pixel_AnglePickupCurrentLimitMin_S2错误！");
                dataCheckResult = false;
            }
            if (ROB_XPickupCurrentPositionLimitMax_S2 < ROB_XPickupCurrentPositionLimitMin_S2)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，ROB_XPickupCurrentPositionLimitMax_S2 < ROB_XPickupCurrentPositionLimitMin_S2错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，ROB_XPickupCurrentPositionLimitMax_S2 < ROB_XPickupCurrentPositionLimitMin_S2错误！");
                dataCheckResult = false;
            }
            if (ROB_YPickupCurrentPositionLimitMax_S2 < ROB_YPickupCurrentPositionLimitMin_S2)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，ROB_YPickupCurrentPositionLimitMax_S2 < ROB_YPickupCurrentPositionLimitMin_S2错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，ROB_YPickupCurrentPositionLimitMax_S2 < ROB_YPickupCurrentPositionLimitMin_S2错误！");
                dataCheckResult = false;
            }
            if (ROB_RZPickupCurrentPositionLimitMax_S2 < ROB_RZPickupCurrentPositionLimitMin_S2)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，ROB_RZPickupCurrentPositionLimitMax_S2 < ROB_RZPickupCurrentPositionLimitMin_S2错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，ROB_RZPickupCurrentPositionLimitMax_S2 < ROB_RZPickupCurrentPositionLimitMin_S2错误！");
                dataCheckResult = false;
            }

            if (ThresholdMinGray_S2 < 0 || ThresholdMinGray_S2 > 255)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，ThresholdMinGray_S2错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，ThresholdMinGray_S2错误！");
                dataCheckResult = false;
            }
            if (ThresholdMaxGray_S2 < 5 || ThresholdMaxGray_S2 > 255)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，ThresholdMaxGray_S2错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，ThresholdMaxGray_S2错误！");
                dataCheckResult = false;
            }
            if (ThresholdMaxGray_S2 < ThresholdMinGray_S2)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，ThresholdMaxGray_S2 < ThresholdMinGray_S2错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，ThresholdMaxGray_S2 < ThresholdMinGray_S2错误！");
                dataCheckResult = false;
            }

            if (OpeningWAndH_S2 < 1 || OpeningWAndH_S2 > 511)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，OpeningWAndH_S2错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，OpeningWAndH_S2错误！");
                dataCheckResult = false;
            }

            if (ShapeMinArea_S2 < 0 || ShapeMinArea_S2 > 100000000)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，ShapeMinArea_S2错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，ShapeMinArea_S2错误！");
                dataCheckResult = false;
            }
            if (ShapeMaxArea_S2 < 0 || ShapeMaxArea_S2 > 100000000)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，ShapeMaxArea_S2错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，ShapeMaxArea_S2错误！");
                dataCheckResult = false;
            }
            if (ShapeMaxArea_S2 < ShapeMinArea_S2)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，ShapeMaxArea_S2 < ShapeMinArea_S2错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，ShapeMaxArea_S2 < ShapeMinArea_S2错误！");
                dataCheckResult = false;
            }
            if (ShapeMinRectangularity_S2 < 0 || ShapeMinRectangularity_S2 > 1)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，ShapeMinRectangularity_S2错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，ShapeMinRectangularity_S2错误！");
                dataCheckResult = false;
            }
            if (ShapeMaxRectangularity_S2 < 0 || ShapeMaxRectangularity_S2 > 1)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，ShapeMaxRectangularity_S2错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，ShapeMaxRectangularity_S2错误！");
                dataCheckResult = false;
            }
            if (ShapeMaxRectangularity_S2 < ShapeMinRectangularity_S2)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，ShapeMaxRectangularity_S2 < ShapeMinRectangularity_S2错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，ShapeMaxRectangularity_S2 < ShapeMinRectangularity_S2错误！");
                dataCheckResult = false;
            }
            if (DilationRectangleWidth_S2 < 1 || DilationRectangleWidth_S2 > 511)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，DilationRectangleWidth_S2错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，DilationRectangleWidth_S2错误！");
                dataCheckResult = false;
            }
            if (DilationRectangleHeight_S2 < 1 || DilationRectangleHeight_S2 > 511)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，DilationRectangleHeight_S2错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，DilationRectangleHeight_S2错误！");
                dataCheckResult = false;
            }
            if (EdgesSubPixCannyAlpha_S2 < 0.2 || EdgesSubPixCannyAlpha_S2 > 50)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，EdgesSubPixCannyAlpha_S2错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，EdgesSubPixCannyAlpha_S2错误！");
                dataCheckResult = false;
            }
            if (EdgesSubPixCannyLow_S2 < 1 || EdgesSubPixCannyLow_S2 > 255)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，EdgesSubPixCannyLow_S2错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，EdgesSubPixCannyLow_S2错误！");
                dataCheckResult = false;
            }
            if (EdgesSubPixCannyHigh_S2 < 1 || EdgesSubPixCannyHigh_S2 > 255)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，EdgesSubPixCannyHigh_S2错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，EdgesSubPixCannyHigh_S2错误！");
                dataCheckResult = false;
            }
            if (EdgesSubPixCannyHigh_S2 < EdgesSubPixCannyLow_S2)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，EdgesSubPixCannyHigh_S2 < EdgesSubPixCannyLow_S2错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，EdgesSubPixCannyHigh_S2 < EdgesSubPixCannyLow_S2错误！");
                dataCheckResult = false;
            }


            if (UnionCollinearContoursXldMaxDistAbs_S2 < 0 || UnionCollinearContoursXldMaxDistAbs_S2 > 100000000)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，UnionCollinearContoursXldMaxDistAbs_S2错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，UnionCollinearContoursXldMaxDistAbs_S2错误！");
                dataCheckResult = false;
            }
            if (UnionCollinearContoursXldMaxDistRel_S2 < 0 || UnionCollinearContoursXldMaxDistRel_S2 > 100000000)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，UnionCollinearContoursXldMaxDistRel_S2错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，UnionCollinearContoursXldMaxDistRel_S2错误！");
                dataCheckResult = false;
            }
            if (UnionCollinearContoursXldMaxShift_S2 < 0 || UnionCollinearContoursXldMaxShift_S2 > 100000000)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，UnionCollinearContoursXldMaxShift_S2错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，UnionCollinearContoursXldMaxShift_S2错误！");
                dataCheckResult = false;
            }
            if (UnionCollinearContoursXldMaxAngle_S2 < 0 || UnionCollinearContoursXldMaxAngle_S2 > 0.78539816339)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，UnionCollinearContoursXldMaxAngle_S2错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，UnionCollinearContoursXldMaxAngle_S2错误！");
                dataCheckResult = false;
            }


            if (XldContlengthMin_S2 < 0 || XldContlengthMin_S2 > 100000000)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，XldContlengthMin_S2错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，XldContlengthMin_S2错误！");
                dataCheckResult = false;
            }
            if (XldContlengthMax_S2 < 0 || XldContlengthMax_S2 > 100000000)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，XldContlengthMax_S2错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，XldContlengthMax_S2错误！");
                dataCheckResult = false;
            }
            if (XldContlengthMax_S2 < XldContlengthMin_S2)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，XldContlengthMax_S2 < XldContlengthMin_S2错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，XldContlengthMax_S2 < XldContlengthMin_S2错误！");
                dataCheckResult = false;
            }
            if (XLDMinArea_S2 < 0 || XLDMinArea_S2 > 100000000)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，XLDMinArea_S2错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，XLDMinArea_S2错误！");
                dataCheckResult = false;
            }
            if (XLDMaxArea_S2 < 0 || XLDMaxArea_S2 > 100000000)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，XLDMaxArea_S2错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，XLDMaxArea_S2错误！");
                dataCheckResult = false;
            }
            if (XLDMaxArea_S2 < XLDMinArea_S2)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，XLDMaxArea_S2 < XLDMinArea_S2错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，XLDMaxArea_S2 < XLDMinArea_S2错误！");
                dataCheckResult = false;
            }
            if (XLDMinRectangularity_S2 < 0 || XLDMinRectangularity_S2 > 1)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，XLDMinRectangularity_S2错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，XLDMinRectangularity_S2错误！");
                dataCheckResult = false;
            }
            if (XLDMaxRectangularity_S2 < 0 || XLDMaxRectangularity_S2 > 1)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，XLDMaxRectangularity_S2错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，XLDMaxRectangularity_S2错误！");
                dataCheckResult = false;
            }
            if (XLDMaxRectangularity_S2 < XLDMinRectangularity_S2)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，XLDMaxRectangularity_S2 < XLDMinRectangularity_S2错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，XLDMaxRectangularity_S2 < XLDMinRectangularity_S2错误！");
                dataCheckResult = false;
            }

            if (UnionAdjacentContoursXldMaxDistAbs_S2 < 0 || UnionAdjacentContoursXldMaxDistAbs_S2 > 100000000)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，UnionAdjacentContoursXldMaxDistAbs_S2错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，UnionAdjacentContoursXldMaxDistAbs_S2错误！");
                dataCheckResult = false;
            }
            if (UnionAdjacentContoursXldMaxDistRel_S2 < 0 || UnionAdjacentContoursXldMaxDistRel_S2 > 100000000)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，UnionAdjacentContoursXldMaxDistRel_S2错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，UnionAdjacentContoursXldMaxDistRel_S2错误！");
                dataCheckResult = false;
            }

            //Station3
            if (ROB_XPickupTeachPosition_S3 < -2000 || ROB_XPickupTeachPosition_S3 > 2000)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，ROB_XPickupTeachPosition_S3错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，ROB_XPickupTeachPosition_S3错误！");
                dataCheckResult = false;
            }
            if (ROB_YPickupTeachPosition_S3 < -2000 || ROB_YPickupTeachPosition_S3 > 2000)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，ROB_YPickupTeachPosition_S3错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，ROB_YPickupTeachPosition_S3错误！");
                dataCheckResult = false;
            }
            if (ROB_ZPickupTeachPosition_S3 < -1000 || ROB_ZPickupTeachPosition_S3 > 1000)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，ROB_ZPickupTeachPosition_S3错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，ROB_ZPickupTeachPosition_S3错误！");
                dataCheckResult = false;
            }
            if (ROB_RXPickupTeachPosition_S3 < -360 || ROB_RXPickupTeachPosition_S3 > 360)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，ROB_RXPickupTeachPosition_S3错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，ROB_RXPickupTeachPosition_S3错误！");
                dataCheckResult = false;
            }
            if (ROB_RWPickupTeachPosition_S3 < -360 || ROB_RWPickupTeachPosition_S3 > 360)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，ROB_RWPickupTeachPosition_S3错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，ROB_RWPickupTeachPosition_S3错误！");
                dataCheckResult = false;
            }
            if (ROB_RZPickupTeachPosition_S3 < -360 || ROB_RZPickupTeachPosition_S3 > 360)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，ROB_RZPickupTeachPosition_S3错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，ROB_RZPickupTeachPosition_S3错误！");
                dataCheckResult = false;
            }

            if (Pixel_XPickupTeachPosition_RT_S3 < 0 || Pixel_XPickupTeachPosition_RT_S3 > 5000)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，Pixel_XPickupTeachPosition_RT_S3错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，Pixel_XPickupTeachPosition_RT_S3错误！");
                dataCheckResult = false;
            }
            if (Pixel_YPickupTeachPosition_RT_S3 < 0 || Pixel_YPickupTeachPosition_RT_S3 > 5000)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，Pixel_YPickupTeachPosition_RT_S3错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，Pixel_YPickupTeachPosition_RT_S3错误！");
                dataCheckResult = false;
            }
            if (Pixel_XPickupTeachPosition_LT_S3 < 0 || Pixel_XPickupTeachPosition_LT_S3 > 5000)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，Pixel_XPickupTeachPosition_LT_S3错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，Pixel_XPickupTeachPosition_LT_S3错误！");
                dataCheckResult = false;
            }
            if (Pixel_YPickupTeachPosition_LT_S3 < 0 || Pixel_YPickupTeachPosition_LT_S3 > 5000)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，Pixel_YPickupTeachPosition_LT_S3错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，Pixel_YPickupTeachPosition_LT_S3错误！");
                dataCheckResult = false;
            }
            if (Pixel_XPickupTeachPosition_LB_S3 < 0 || Pixel_XPickupTeachPosition_LB_S3 > 5000)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，Pixel_XPickupTeachPosition_LB_S3错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，Pixel_XPickupTeachPosition_LB_S3错误！");
                dataCheckResult = false;
            }
            if (Pixel_YPickupTeachPosition_LB_S3 < 0 || Pixel_YPickupTeachPosition_LB_S3 > 5000)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，Pixel_YPickupTeachPosition_LB_S3错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，Pixel_YPickupTeachPosition_LB_S3错误！");
                dataCheckResult = false;
            }
            if (Pixel_XPickupTeachPosition_RB_S3 < 0 || Pixel_XPickupTeachPosition_RB_S3 > 5000)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，Pixel_XPickupTeachPosition_RB_S3错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，Pixel_XPickupTeachPosition_RB_S3错误！");
                dataCheckResult = false;
            }
            if (Pixel_YPickupTeachPosition_RB_S3 < 0 || Pixel_YPickupTeachPosition_RB_S3 > 5000)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，Pixel_YPickupTeachPosition_RB_S3错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，Pixel_YPickupTeachPosition_RB_S3错误！");
                dataCheckResult = false;
            }
            if (Pixel_AnglePickupTeach_S3 < -360 || Pixel_AnglePickupTeach_S3 > 360)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，Pixel_AnglPickupTeache错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，Pixel_AnglePickupTeach_S3错误！");
                dataCheckResult = false;
            }

            if (Pixel_XPickupCurrentPositionLimitMax_RT_S3 < Pixel_XPickupCurrentPositionLimitMin_RT_S3)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，Pixel_XPickupCurrentPositionLimitMax_RT_S3 < Pixel_XPickupCurrentPositionLimitMin_RT_S3错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，Pixel_XPickupCurrentPositionLimitMax_RT_S3 < Pixel_XPickupCurrentPositionLimitMin_RT_S3错误！");
                dataCheckResult = false;
            }
            if (Pixel_YPickupCurrentPositionLimitMax_RT_S3 < Pixel_YPickupCurrentPositionLimitMin_RT_S3)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，Pixel_YPickupCurrentPositionLimitMax_RT_S3 < Pixel_YPickupCurrentPositionLimitMin_RT_S3错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，Pixel_YPickupCurrentPositionLimitMax_RT_S3 < Pixel_YPickupCurrentPositionLimitMin_RT_S3错误！");
                dataCheckResult = false;
            }
            if (Pixel_AnglePickupCurrentLimitMax_S3 < Pixel_AnglePickupCurrentLimitMin_S3)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，Pixel_AnglePickupCurrentLimitMax_S3 < Pixel_AnglePickupCurrentLimitMin_S3错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，Pixel_AnglePickupCurrentLimitMax_S3 < Pixel_AnglePickupCurrentLimitMin_S3错误！");
                dataCheckResult = false;
            }
            if (ROB_XPickupCurrentPositionLimitMax_S3 < ROB_XPickupCurrentPositionLimitMin_S3)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，ROB_XPickupCurrentPositionLimitMax_S3 < ROB_XPickupCurrentPositionLimitMin_S3错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，ROB_XPickupCurrentPositionLimitMax_S3 < ROB_XPickupCurrentPositionLimitMin_S3错误！");
                dataCheckResult = false;
            }
            if (ROB_YPickupCurrentPositionLimitMax_S3 < ROB_YPickupCurrentPositionLimitMin_S3)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，ROB_YPickupCurrentPositionLimitMax_S3 < ROB_YPickupCurrentPositionLimitMin_S3错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，ROB_YPickupCurrentPositionLimitMax_S3 < ROB_YPickupCurrentPositionLimitMin_S3错误！");
                dataCheckResult = false;
            }
            if (ROB_RZPickupCurrentPositionLimitMax_S3 < ROB_RZPickupCurrentPositionLimitMin_S3)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，ROB_RZPickupCurrentPositionLimitMax_S3 < ROB_RZPickupCurrentPositionLimitMin_S3错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，ROB_RZPickupCurrentPositionLimitMax_S3 < ROB_RZPickupCurrentPositionLimitMin_S3错误！");
                dataCheckResult = false;
            }

            if (ThresholdMinGray_S3 < 0 || ThresholdMinGray_S3 > 255)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，ThresholdMinGray_S3错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，ThresholdMinGray_S3错误！");
                dataCheckResult = false;
            }
            if (ThresholdMaxGray_S3 < 5 || ThresholdMaxGray_S3 > 255)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，ThresholdMaxGray_S3错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，ThresholdMaxGray_S3错误！");
                dataCheckResult = false;
            }
            if (ThresholdMaxGray_S3 < ThresholdMinGray_S3)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，ThresholdMaxGray_S3 < ThresholdMinGray_S3错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，ThresholdMaxGray_S3 < ThresholdMinGray_S3错误！");
                dataCheckResult = false;
            }

            if (OpeningWAndH_S3 < 1 || OpeningWAndH_S3 > 511)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，OpeningWAndH_S3错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，OpeningWAndH_S3错误！");
                dataCheckResult = false;
            }

            if (ShapeMinArea_S3 < 0 || ShapeMinArea_S3 > 100000000)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，ShapeMinArea_S3错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，ShapeMinArea_S3错误！");
                dataCheckResult = false;
            }
            if (ShapeMaxArea_S3 < 0 || ShapeMaxArea_S3 > 100000000)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，ShapeMaxArea_S3错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，ShapeMaxArea_S3错误！");
                dataCheckResult = false;
            }
            if (ShapeMaxArea_S3 < ShapeMinArea_S3)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，ShapeMaxArea_S3 < ShapeMinArea_S3错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，ShapeMaxArea_S3 < ShapeMinArea_S3错误！");
                dataCheckResult = false;
            }
            if (ShapeMinRectangularity_S3 < 0 || ShapeMinRectangularity_S3 > 1)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，ShapeMinRectangularity_S3错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，ShapeMinRectangularity_S3错误！");
                dataCheckResult = false;
            }
            if (ShapeMaxRectangularity_S3 < 0 || ShapeMaxRectangularity_S3 > 1)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，ShapeMaxRectangularity_S3错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，ShapeMaxRectangularity_S3错误！");
                dataCheckResult = false;
            }
            if (ShapeMaxRectangularity_S3 < ShapeMinRectangularity_S3)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，ShapeMaxRectangularity_S3 < ShapeMinRectangularity_S3错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，ShapeMaxRectangularity_S3 < ShapeMinRectangularity_S3错误！");
                dataCheckResult = false;
            }
            if (DilationRectangleWidth_S3 < 1 || DilationRectangleWidth_S3 > 511)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，DilationRectangleWidth_S3错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，DilationRectangleWidth_S3错误！");
                dataCheckResult = false;
            }
            if (DilationRectangleHeight_S3 < 1 || DilationRectangleHeight_S3 > 511)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，DilationRectangleHeight_S3错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，DilationRectangleHeight_S3错误！");
                dataCheckResult = false;
            }
            if (EdgesSubPixCannyAlpha_S3 < 0.2 || EdgesSubPixCannyAlpha_S3 > 50)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，EdgesSubPixCannyAlpha_S3错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，EdgesSubPixCannyAlpha_S3错误！");
                dataCheckResult = false;
            }
            if (EdgesSubPixCannyLow_S3 < 1 || EdgesSubPixCannyLow_S3 > 255)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，EdgesSubPixCannyLow_S3错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，EdgesSubPixCannyLow_S3错误！");
                dataCheckResult = false;
            }
            if (EdgesSubPixCannyHigh_S3 < 1 || EdgesSubPixCannyHigh_S3 > 255)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，EdgesSubPixCannyHigh_S3错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，EdgesSubPixCannyHigh_S3错误！");
                dataCheckResult = false;
            }
            if (EdgesSubPixCannyHigh_S3 < EdgesSubPixCannyLow_S3)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，EdgesSubPixCannyHigh_S3 < EdgesSubPixCannyLow_S3错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，EdgesSubPixCannyHigh_S3 < EdgesSubPixCannyLow_S3错误！");
                dataCheckResult = false;
            }


            if (UnionCollinearContoursXldMaxDistAbs_S3 < 0 || UnionCollinearContoursXldMaxDistAbs_S3 > 100000000)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，UnionCollinearContoursXldMaxDistAbs_S3错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，UnionCollinearContoursXldMaxDistAbs_S3错误！");
                dataCheckResult = false;
            }
            if (UnionCollinearContoursXldMaxDistRel_S3 < 0 || UnionCollinearContoursXldMaxDistRel_S3 > 100000000)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，UnionCollinearContoursXldMaxDistRel_S3错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，UnionCollinearContoursXldMaxDistRel_S3错误！");
                dataCheckResult = false;
            }
            if (UnionCollinearContoursXldMaxShift_S3 < 0 || UnionCollinearContoursXldMaxShift_S3 > 100000000)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，UnionCollinearContoursXldMaxShift_S3错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，UnionCollinearContoursXldMaxShift_S3错误！");
                dataCheckResult = false;
            }
            if (UnionCollinearContoursXldMaxAngle_S3 < 0 || UnionCollinearContoursXldMaxAngle_S3 > 0.78539816339)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，UnionCollinearContoursXldMaxAngle_S3错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，UnionCollinearContoursXldMaxAngle_S3错误！");
                dataCheckResult = false;
            }


            if (XldContlengthMin_S3 < 0 || XldContlengthMin_S3 > 100000000)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，XldContlengthMin_S3错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，XldContlengthMin_S3错误！");
                dataCheckResult = false;
            }
            if (XldContlengthMax_S3 < 0 || XldContlengthMax_S3 > 100000000)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，XldContlengthMax_S3错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，XldContlengthMax_S3错误！");
                dataCheckResult = false;
            }
            if (XldContlengthMax_S3 < XldContlengthMin_S3)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，XldContlengthMax_S3 < XldContlengthMin_S3错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，XldContlengthMax_S3 < XldContlengthMin_S3错误！");
                dataCheckResult = false;
            }
            if (XLDMinArea_S3 < 0 || XLDMinArea_S3 > 100000000)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，XLDMinArea_S3错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，XLDMinArea_S3错误！");
                dataCheckResult = false;
            }
            if (XLDMaxArea_S3 < 0 || XLDMaxArea_S3 > 100000000)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，XLDMaxArea_S3错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，XLDMaxArea_S3错误！");
                dataCheckResult = false;
            }
            if (XLDMaxArea_S3 < XLDMinArea_S3)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，XLDMaxArea_S3 < XLDMinArea_S3错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，XLDMaxArea_S3 < XLDMinArea_S3错误！");
                dataCheckResult = false;
            }
            if (XLDMinRectangularity_S3 < 0 || XLDMinRectangularity_S3 > 1)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，XLDMinRectangularity_S3错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，XLDMinRectangularity_S3错误！");
                dataCheckResult = false;
            }
            if (XLDMaxRectangularity_S3 < 0 || XLDMaxRectangularity_S3 > 1)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，XLDMaxRectangularity_S3错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，XLDMaxRectangularity_S3错误！");
                dataCheckResult = false;
            }
            if (XLDMaxRectangularity_S3 < XLDMinRectangularity_S3)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，XLDMaxRectangularity_S3 < XLDMinRectangularity_S3错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，XLDMaxRectangularity_S3 < XLDMinRectangularity_S3错误！");
                dataCheckResult = false;
            }

            if (UnionAdjacentContoursXldMaxDistAbs_S3 < 0 || UnionAdjacentContoursXldMaxDistAbs_S3 > 100000000)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，UnionAdjacentContoursXldMaxDistAbs_S3错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，UnionAdjacentContoursXldMaxDistAbs_S3错误！");
                dataCheckResult = false;
            }
            if (UnionAdjacentContoursXldMaxDistRel_S3 < 0 || UnionAdjacentContoursXldMaxDistRel_S3 > 100000000)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，UnionAdjacentContoursXldMaxDistRel_S3错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，UnionAdjacentContoursXldMaxDistRel_S3错误！");
                dataCheckResult = false;
            }


            if (ClassificationThresholdMin_S3 < 0 || ClassificationThresholdMin_S3 > 255)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，ClassificationThresholdMin_S3错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，ClassificationThresholdMin_S3错误！");
                dataCheckResult = false;
            }
            if (ClassificationThresholdMax_S3 < 5 || ClassificationThresholdMax_S3 > 255)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，ClassificationThresholdMax_S3错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，ClassificationThresholdMax_S3错误！");
                dataCheckResult = false;
            }
            if (ClassificationThresholdMax_S3 < ClassificationThresholdMin_S3)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，ClassificationThresholdMax_S3 < ClassificationThresholdMin_S3错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，ClassificationThresholdMax_S3 < ClassificationThresholdMin_S3错误！");
                dataCheckResult = false;
            }

            if (ProductAreaMin_S3 < 0 || ProductAreaMin_S3 > 100000000)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，ProductAreaMin_S3错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，ProductAreaMin_S3错误！");
                dataCheckResult = false;
            }
            if (ProductAreaMax_S3 < 0 || ProductAreaMax_S3 > 100000000)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，ProductAreaMax_S3错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，ProductAreaMax_S3错误！");
                dataCheckResult = false;
            }
            if (ProductAreaMax_S3 < ProductAreaMin_S3)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，ProductAreaMax_S3 < ProductAreaMin_S3错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，ProductAreaMax_S3 < ProductAreaMin_S3错误！");
                dataCheckResult = false;
            }

            if (CoverAreaMin_S3 < 0 || CoverAreaMin_S3 > 100000000)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，CoverAreaMin_S3错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，CoverAreaMin_S3错误！");
                dataCheckResult = false;
            }
            if (CoverAreaMax_S3 < 0 || CoverAreaMax_S3 > 100000000)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，CoverAreaMax_S3错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，CoverAreaMax_S3错误！");
                dataCheckResult = false;
            }
            if (CoverAreaMax_S3 < CoverAreaMin_S3)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，CoverAreaMax_S3 < CoverAreaMin_S3错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，CoverAreaMax_S3 < CoverAreaMin_S3错误！");
                dataCheckResult = false;
            }

            if (EmptyAreaMin_S3 < 0 || EmptyAreaMin_S3 > 100000000)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，EmptyAreaMin_S3错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，EmptyAreaMin_S3错误！");
                dataCheckResult = false;
            }
            if (EmptyAreaMax_S3 < 0 || EmptyAreaMax_S3 > 100000000)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，EmptyAreaMax_S3错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，EmptyAreaMax_S3错误！");
                dataCheckResult = false;
            }
            if (EmptyAreaMax_S3 < EmptyAreaMin_S3)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，EmptyAreaMax_S3 < EmptyAreaMin_S3错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，EmptyAreaMax_S3 < EmptyAreaMin_S3错误！");
                dataCheckResult = false;
            }

            if (ClassificationThresholdCoverMin_S3 < 0 || ClassificationThresholdCoverMin_S3 > 255)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，ClassificationThresholdCoverMin_S3错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，ClassificationThresholdCoverMin_S3错误！");
                dataCheckResult = false;
            }
            if (ClassificationThresholdCoverMax_S3 < 5 || ClassificationThresholdCoverMax_S3 > 255)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，ClassificationThresholdCoverMax_S3错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，ClassificationThresholdCoverMax_S3错误！");
                dataCheckResult = false;
            }
            if (ClassificationThresholdCoverMax_S3 < ClassificationThresholdCoverMin_S3)
            {
                MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，ClassificationThresholdCoverMax_S3 < ClassificationThresholdCoverMin_S3错误！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取的数据中，ClassificationThresholdCoverMax_S3 < ClassificationThresholdCoverMin_S3错误！");
                dataCheckResult = false;
            }

            return dataCheckResult;
        }
        #endregion

    }
}
