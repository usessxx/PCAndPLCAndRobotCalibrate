using ChoiceTech.Halcon.Control;
using HalconDotNet;
using HalconMVTec;
using MatchModel.Halcon;
using MyTool;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using XMLFile;

namespace ROBCalibrate
{
    /// <summary>
    /// MaintenanceCameraFormRequestOpenOrCloseCameraDelegate:界面请求打开或关闭相机
    /// </summary>
    /// <param name="cameraIndex">int:相机索引，0-左侧相机，1-右侧相机，2-后侧相机</param>
    public delegate void MaintenanceCameraFormRequestOpenOrCloseCameraDelegate(int cameraIndex);

    /// <summary>
    /// MaintenanceCameraFormRequestGrabImageAndDispDelegate:界面请求抓取图像并显示委托
    /// </summary>
    /// <param name="cameraIndex">int:相机索引，0-左侧相机，1-右侧相机，2-后侧相机</param>
    /// <param name="dispCrossLineFlag">bool:显示十字线标志，false-不显示，true-显示</param>
    public delegate void MaintenanceCameraFormRequestGrabImageAndDispDelegate(int cameraIndex, bool dispCrossLineFlag);

    /// <summary>
    /// MaintenanceCameraFormRequestGrabImageAndDispVideoDelegate:界面请求抓取图像并显示视频委托
    /// </summary>
    /// <param name="cameraIndex">int:相机索引，0-左侧相机，1-右侧相机，2-后侧相机,3-由于十字线显示与否切换引发的索引</param>
    /// <param name="dispCrossLineFlag">bool:显示十字线标志，false-不显示，true-显示</param>
    public delegate void MaintenanceCameraFormRequestGrabImageAndDispVideoDelegate(int cameraIndex, bool dispCrossLineFlag);

    /// <summary>
    /// MaintenanceCameraFormRequestSaveImageDelegate：界面请求保存图片委托
    /// </summary>
    /// <param name="cameraIndex">int:相机索引，0-左侧相机，1-右侧相机，2-后侧相机</param>  
    public delegate void MaintenanceCameraFormRequestSaveImageDelegate(int cameraIndex);

    /// <summary>
    /// MaintenanceCameraFormRequestBackCameraScanBarcode:请求后相机识别条码
    /// </summary>
    public delegate void MaintenanceCameraFormRequestBackCameraScanBarcodeDelegate();

    public partial class CalibrateForm : Form
    {
        public event MaintenanceCameraFormRequestOpenOrCloseCameraDelegate MaintenanceCameraFormRequestOpenOrCloseCameraEvent;
        public event MaintenanceCameraFormRequestGrabImageAndDispDelegate MaintenanceCameraFormRequestGrabImageAndDispEvent;
        public event MaintenanceCameraFormRequestGrabImageAndDispVideoDelegate MaintenanceCameraFormRequestGrabImageAndDispVideoEvent;
        public event MaintenanceCameraFormRequestSaveImageDelegate MaintenanceCameraFormRequestSaveImageEvent;

        #region 公共变量
        public static string _GeneralParameterDirectoryPath;//通用参数保存文件夹路径
        public static string _ProductParameterDirectoryPath;//产品参数保存文件夹路径
        public static string _ProductDataDirectoryPath;//产品数据保存文件夹路径

        public static ProductRecipe _editProductRecipe = new ProductRecipe();//编辑产品的数据
        public static ProductRecipe _currentProductRecipe = new ProductRecipe();//当前产品的数据
        public static string _currentProductName = "";//当前产品
        public static string _editProductName = "";//当前编辑产品名称

        public static int _CameraGrabImageInterval = 10;//相机抓取图像延时
        public HObject ho_Image_S1;
        public HObject ho_Image_S2;
        public HObject ho_Image_S3;
        //实时显示标志
        public static bool RTDFlag_S1 = false;
        public static bool RTDFlag_S2 = false;
        public static bool RTDFlag_S3 = false;
        /// <summary>
        /// bool:用于记录图像处理是否处于BUSY状态，如果是有些按钮点击无效
        /// </summary>
        public static bool CameraIsBusyFlag_S1 = false;
        public static bool CameraIsBusyFlag_S2 = false;
        public static bool CameraIsBusyFlag_S3 = false;

        //窗体声明
        public HWindow_Final hw_S1;
        public HWindow_Final hw_S2;
        public HWindow_Final hw_S3;

        //模板调试页面
        public static ShapeModelFrm smf_S1 = new ShapeModelFrm($"{_GeneralParameterDirectoryPath}/{ModelPath_S1}");
        public static ShapeModelFrm smf_S2 = new ShapeModelFrm($"{_GeneralParameterDirectoryPath}/{ModelPath_S2}");
        public static ShapeModelFrm smf_S3 = new ShapeModelFrm($"{_GeneralParameterDirectoryPath}/{ModelPath_S3}");
        //找圆调试页面
        public static FindLineFrm dsf_S1 = new FindLineFrm($"{_GeneralParameterDirectoryPath}/{ModelPath_S1}");
        public static FindLineFrm dsf_S2 = new FindLineFrm($"{_GeneralParameterDirectoryPath}/{ModelPath_S2}");
        public static FindLineFrm dsf_S3 = new FindLineFrm($"{_GeneralParameterDirectoryPath}/{ModelPath_S3}");
        public const string ModelPath_S1 = "Shape_S1/";
        public const string ModelPath_S2 = "Shape_S2/";
        public const string ModelPath_S3 = "Shape_S3/";

        #endregion

        #region 私有变量
        RobCamCalibration newRobCamCalibration = new RobCamCalibration();
        MinimumEnclosingRectangle newMinimumEnclosingRectangle = new MinimumEnclosingRectangle();
        bool _generalParameterLoadingToCtrlFirstTimeFlag = true;//是否第一次读取通用参数到控件Flag:第一次读取时在构造函数里,会报错;"在创建窗口句柄之前，不能在控件上调用 Invoke 或 BeginInvoke。"
        //定义一个Control类型的队列allCtrlsQueue
        private static Queue<Control> allCtrlsQueue = new Queue<Control>();
        //定义一个Control类型的队列allCtrlsQueue的复制数组
        Control[] allCtrlsArray;
        #endregion

        #region 公共变量
        //Station1
        //9点标定参数
        public float ROB_X9PTeachPosition_P1_S1 = 0.0f;
        public float ROB_X9PTeachPosition_P2_S1 = 0.0f;
        public float ROB_X9PTeachPosition_P3_S1 = 0.0f;
        public float ROB_X9PTeachPosition_P4_S1 = 0.0f;
        public float ROB_X9PTeachPosition_P5_S1 = 0.0f;
        public float ROB_X9PTeachPosition_P6_S1 = 0.0f;
        public float ROB_X9PTeachPosition_P7_S1 = 0.0f;
        public float ROB_X9PTeachPosition_P8_S1 = 0.0f;
        public float ROB_X9PTeachPosition_P9_S1 = 0.0f;

        public float ROB_Y9PTeachPosition_P1_S1 = 0.0f;
        public float ROB_Y9PTeachPosition_P2_S1 = 0.0f;
        public float ROB_Y9PTeachPosition_P3_S1 = 0.0f;
        public float ROB_Y9PTeachPosition_P4_S1 = 0.0f;
        public float ROB_Y9PTeachPosition_P5_S1 = 0.0f;
        public float ROB_Y9PTeachPosition_P6_S1 = 0.0f;
        public float ROB_Y9PTeachPosition_P7_S1 = 0.0f;
        public float ROB_Y9PTeachPosition_P8_S1 = 0.0f;
        public float ROB_Y9PTeachPosition_P9_S1 = 0.0f;

        public float ROB_Z9PTeachPosition_P1_S1 = 0.0f;
        public float ROB_Z9PTeachPosition_P2_S1 = 0.0f;
        public float ROB_Z9PTeachPosition_P3_S1 = 0.0f;
        public float ROB_Z9PTeachPosition_P4_S1 = 0.0f;
        public float ROB_Z9PTeachPosition_P5_S1 = 0.0f;
        public float ROB_Z9PTeachPosition_P6_S1 = 0.0f;
        public float ROB_Z9PTeachPosition_P7_S1 = 0.0f;
        public float ROB_Z9PTeachPosition_P8_S1 = 0.0f;
        public float ROB_Z9PTeachPosition_P9_S1 = 0.0f;

        public float ROB_RX9PTeachPosition_P1_S1 = 0.0f;
        public float ROB_RX9PTeachPosition_P2_S1 = 0.0f;
        public float ROB_RX9PTeachPosition_P3_S1 = 0.0f;
        public float ROB_RX9PTeachPosition_P4_S1 = 0.0f;
        public float ROB_RX9PTeachPosition_P5_S1 = 0.0f;
        public float ROB_RX9PTeachPosition_P6_S1 = 0.0f;
        public float ROB_RX9PTeachPosition_P7_S1 = 0.0f;
        public float ROB_RX9PTeachPosition_P8_S1 = 0.0f;
        public float ROB_RX9PTeachPosition_P9_S1 = 0.0f;

        public float ROB_RW9PTeachPosition_P1_S1 = 0.0f;
        public float ROB_RW9PTeachPosition_P2_S1 = 0.0f;
        public float ROB_RW9PTeachPosition_P3_S1 = 0.0f;
        public float ROB_RW9PTeachPosition_P4_S1 = 0.0f;
        public float ROB_RW9PTeachPosition_P5_S1 = 0.0f;
        public float ROB_RW9PTeachPosition_P6_S1 = 0.0f;
        public float ROB_RW9PTeachPosition_P7_S1 = 0.0f;
        public float ROB_RW9PTeachPosition_P8_S1 = 0.0f;
        public float ROB_RW9PTeachPosition_P9_S1 = 0.0f;

        public float ROB_RZ9PTeachPosition_P1_S1 = 0.0f;
        public float ROB_RZ9PTeachPosition_P2_S1 = 0.0f;
        public float ROB_RZ9PTeachPosition_P3_S1 = 0.0f;
        public float ROB_RZ9PTeachPosition_P4_S1 = 0.0f;
        public float ROB_RZ9PTeachPosition_P5_S1 = 0.0f;
        public float ROB_RZ9PTeachPosition_P6_S1 = 0.0f;
        public float ROB_RZ9PTeachPosition_P7_S1 = 0.0f;
        public float ROB_RZ9PTeachPosition_P8_S1 = 0.0f;
        public float ROB_RZ9PTeachPosition_P9_S1 = 0.0f;

        public float Pixel_X9PTeachPosition_P1_S1 = 0.0f;
        public float Pixel_X9PTeachPosition_P2_S1 = 0.0f;
        public float Pixel_X9PTeachPosition_P3_S1 = 0.0f;
        public float Pixel_X9PTeachPosition_P4_S1 = 0.0f;
        public float Pixel_X9PTeachPosition_P5_S1 = 0.0f;
        public float Pixel_X9PTeachPosition_P6_S1 = 0.0f;
        public float Pixel_X9PTeachPosition_P7_S1 = 0.0f;
        public float Pixel_X9PTeachPosition_P8_S1 = 0.0f;
        public float Pixel_X9PTeachPosition_P9_S1 = 0.0f;

        public float Pixel_Y9PTeachPosition_P1_S1 = 0.0f;
        public float Pixel_Y9PTeachPosition_P2_S1 = 0.0f;
        public float Pixel_Y9PTeachPosition_P3_S1 = 0.0f;
        public float Pixel_Y9PTeachPosition_P4_S1 = 0.0f;
        public float Pixel_Y9PTeachPosition_P5_S1 = 0.0f;
        public float Pixel_Y9PTeachPosition_P6_S1 = 0.0f;
        public float Pixel_Y9PTeachPosition_P7_S1 = 0.0f;
        public float Pixel_Y9PTeachPosition_P8_S1 = 0.0f;
        public float Pixel_Y9PTeachPosition_P9_S1 = 0.0f;

        public static string _cameraName_S1;//相机出厂名字

        public float Pixel_X1_9PCalibrate_S1 = 0.0f;//9点标定点中心点1 X像素坐标
        public float Pixel_Y1_9PCalibrate_S1 = 0.0f;//9点标定点中心点1 Y像素坐标
        public float Pixel_X2_9PCalibrate_S1 = 0.0f;//9点标定点中心点2 X像素坐标
        public float Pixel_Y2_9PCalibrate_S1 = 0.0f;//9点标定点中心点2 Y像素坐标
        public float Pixel_X3_9PCalibrate_S1 = 0.0f;//9点标定点中心点3 X像素坐标
        public float Pixel_Y3_9PCalibrate_S1 = 0.0f;//9点标定点中心点3 Y像素坐标
        public float Pixel_X4_9PCalibrate_S1 = 0.0f;//9点标定点中心点4 X像素坐标
        public float Pixel_Y4_9PCalibrate_S1 = 0.0f;//9点标定点中心点4 Y像素坐标
        public float Pixel_X5_9PCalibrate_S1 = 0.0f;//9点标定点中心点5 X像素坐标
        public float Pixel_Y5_9PCalibrate_S1 = 0.0f;//9点标定点中心点5 Y像素坐标
        public float Pixel_X6_9PCalibrate_S1 = 0.0f;//9点标定点中心点6 X像素坐标
        public float Pixel_Y6_9PCalibrate_S1 = 0.0f;//9点标定点中心点6 Y像素坐标
        public float Pixel_X7_9PCalibrate_S1 = 0.0f;//9点标定点中心点7 X像素坐标
        public float Pixel_Y7_9PCalibrate_S1 = 0.0f;//9点标定点中心点7 Y像素坐标
        public float Pixel_X8_9PCalibrate_S1 = 0.0f;//9点标定点中心点8 X像素坐标
        public float Pixel_Y8_9PCalibrate_S1 = 0.0f;//9点标定点中心点8 Y像素坐标
        public float Pixel_X9_9PCalibrate_S1 = 0.0f;//9点标定点中心点9 X像素坐标
        public float Pixel_Y9_9PCalibrate_S1 = 0.0f;//9点标定点中心点9 Y像素坐标

        //示教物料拍照获取数据的变量
        public float ROB_XPickupTeachPosition_S1 = 0.0f;//示教的ROB取料位置X坐标
        public float ROB_YPickupTeachPosition_S1 = 0.0f;//示教的ROB取料位置Y坐标
        public float ROB_ZPickupTeachPosition_S1 = 0.0f;//示教的ROB取料位置Z坐标
        public float ROB_RXPickupTeachPosition_S1 = 0.0f;//示教的ROB取料位置RX坐标
        public float ROB_RWPickupTeachPosition_S1 = 0.0f;//示教的ROB取料位置RW坐标
        public float ROB_RZPickupTeachPosition_S1 = 0.0f;//示教的ROB取料位置RZ坐标
        public float Pixel_XPickupTeachPosition_RT_S1 = 0.0f;//示教物料的右上顶点X像素坐标
        public float Pixel_YPickupTeachPosition_RT_S1 = 0.0f;//示教物料的右上顶点Y像素坐标
        public float Pixel_XPickupTeachPosition_LT_S1 = 0.0f;//示教物料的左上顶点X像素坐标
        public float Pixel_YPickupTeachPosition_LT_S1 = 0.0f;//示教物料的左上顶点Y像素坐标
        public float Pixel_XPickupTeachPosition_LB_S1 = 0.0f;//示教物料的左下顶点X像素坐标
        public float Pixel_YPickupTeachPosition_LB_S1 = 0.0f;//示教物料的左下顶点Y像素坐标
        public float Pixel_XPickupTeachPosition_RB_S1 = 0.0f;//示教物料的右下顶点X像素坐标
        public float Pixel_YPickupTeachPosition_RB_S1 = 0.0f;//示教物料的右下顶点Y像素坐标
        public float Pixel_AnglePickupTeach_S1 = 0.0f;//示教物料的像素坐标系角度
        public float ROB_XPickupTeachPosition_RT_S1 = 0.0f;//示教物料的右上顶点X ROB坐标----暂时没有用到
        public float ROB_YPickupTeachPosition_RT_S1 = 0.0f;//示教物料的右上顶点Y ROB坐标----暂时没有用到
        public float ROB_XPickupTeachPosition_LT_S1 = 0.0f;//示教物料的左上顶点X ROB坐标----暂时没有用到
        public float ROB_YPickupTeachPosition_LT_S1 = 0.0f;//示教物料的左上顶点Y ROB坐标----暂时没有用到
        public float ROB_XPickupTeachPosition_LB_S1 = 0.0f;//示教物料的左下顶点X ROB坐标----暂时没有用到
        public float ROB_YPickupTeachPosition_LB_S1 = 0.0f;//示教物料的左下顶点Y ROB坐标----暂时没有用到
        public float ROB_XPickupTeachPosition_RB_S1 = 0.0f;//示教物料的右下顶点X ROB坐标----暂时没有用到
        public float ROB_YPickupTeachPosition_RB_S1 = 0.0f;//示教物料的右下顶点Y ROB坐标----暂时没有用到

        //临时变量
        public float Pixel_XCenterPickupCurrentPosition_S1;//当前物料的中心点X坐标
        public float Pixel_YCenterPickupCurrentPosition_S1;//当前物料的中心点Y坐标
        public float Pixel_AnglePickupCurrent_S1 = 0.0f;//当前物料的像素坐标系角度
        public float Pixel_WidthPickupCurrent_S1;//当前物料的宽度
        public float Pixel_HeightPickupCurrent_S1;//当前物料的高度
        public float Pixel_XPickupCurrentPosition_RT_S1 = 0.0f;//当前物料的右上顶点X像素坐标
        public float Pixel_YPickupCurrentPosition_RT_S1 = 0.0f;//当前物料的右上顶点Y像素坐标
        public float Pixel_XPickupCurrentPosition_LT_S1 = 0.0f;//当前物料的左上顶点X像素坐标
        public float Pixel_YPickupCurrentPosition_LT_S1 = 0.0f;//当前物料的左上顶点Y像素坐标
        public float Pixel_XPickupCurrentPosition_LB_S1 = 0.0f;//当前物料的左下顶点X像素坐标
        public float Pixel_YPickupCurrentPosition_LB_S1 = 0.0f;//当前物料的左下顶点Y像素坐标
        public float Pixel_XPickupCurrentPosition_RB_S1 = 0.0f;//当前物料的右下顶点X像素坐标
        public float Pixel_YPickupCurrentPosition_RB_S1 = 0.0f;//当前物料的右下顶点Y像素坐标

        public float ROB_XPickupCurrentPosition_RT_S1 = 0.0f;//当前物料的右上顶点X ROB坐标
        public float ROB_YPickupCurrentPosition_RT_S1 = 0.0f;//当前物料的右上顶点Y ROB坐标
        public float ROB_XPickupCurrentPosition_LT_S1 = 0.0f;//当前物料的左上顶点X ROB坐标
        public float ROB_YPickupCurrentPosition_LT_S1 = 0.0f;//当前物料的左上顶点Y ROB坐标
        public float ROB_XPickupCurrentPosition_LB_S1 = 0.0f;//当前物料的左下顶点X ROB坐标
        public float ROB_YPickupCurrentPosition_LB_S1 = 0.0f;//当前物料的左下顶点Y ROB坐标
        public float ROB_XPickupCurrentPosition_RB_S1 = 0.0f;//当前物料的右下顶点X ROB坐标
        public float ROB_YPickupCurrentPosition_RB_S1 = 0.0f;//当前物料的右下顶点Y ROB坐标

        HTuple hv_HomMat2D_9P_S1 = new HTuple();//9点标定数据

        public float Pixel_AnglePickupOffset_S1 = 0.0f;//物料的角度偏移
        public float ROB_XPickupTeachPosition_RT_CurrentProduct_S1 = 0.0f;//当前产品的示教物料的右上顶点X ROB坐标
        public float ROB_YPickupTeachPosition_RT_CurrentProduct_S1 = 0.0f;//当前产品的示教物料的右上顶点Y ROB坐标
        public float ROB_XPickupTeachPosition_LT_CurrentProduct_S1 = 0.0f;//当前产品的示教物料的左上顶点X ROB坐标
        public float ROB_YPickupTeachPosition_LT_CurrentProduct_S1 = 0.0f;//当前产品的示教物料的左上顶点Y ROB坐标
        public float ROB_XPickupTeachPosition_LB_CurrentProduct_S1 = 0.0f;//当前产品的示教物料的左下顶点X ROB坐标
        public float ROB_YPickupTeachPosition_LB_CurrentProduct_S1 = 0.0f;//当前产品的示教物料的左下顶点Y ROB坐标
        public float ROB_XPickupTeachPosition_RB_CurrentProduct_S1 = 0.0f;//当前产品的示教物料的右下顶点X ROB坐标
        public float ROB_YPickupTeachPosition_RB_CurrentProduct_S1 = 0.0f;//当前产品的示教物料的右下顶点Y ROB坐标
        public HTuple hv_HomMat2D_CurrentTranslateMatrix_S1 = new HTuple();//用4个顶点坐标计算示教物料位置变换到当前物料位置的矩阵hv_HomMat2D_CurrentTranslateMatrix
        public HTuple hv_HomMat2D_CurrentTranslateMatrix_Pixel_S1 = new HTuple();//用4个顶点像素坐标计算示教物料位置变换到当前物料位置的矩阵hv_HomMat2D_CurrentTranslateMatrix_Pixel
        //最终的当前取料位置
        public static float ROB_XPickupCurrentPosition_S1 = 0.0f;
        public static float ROB_YPickupCurrentPosition_S1 = 0.0f;
        public static float ROB_ZPickupCurrentPosition_S1 = 0.0f;
        public static float ROB_RXPickupCurrentPosition_S1 = 0.0f;
        public static float ROB_RWPickupCurrentPosition_S1 = 0.0f;
        public static float ROB_RZPickupCurrentPosition_S1 = 0.0f;

        //产品定位检测算子变量
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

        //限制条件变量
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

        //Station2
        //9点标定参数
        public float ROB_X9PTeachPosition_P1_S2 = 0.0f;
        public float ROB_X9PTeachPosition_P2_S2 = 0.0f;
        public float ROB_X9PTeachPosition_P3_S2 = 0.0f;
        public float ROB_X9PTeachPosition_P4_S2 = 0.0f;
        public float ROB_X9PTeachPosition_P5_S2 = 0.0f;
        public float ROB_X9PTeachPosition_P6_S2 = 0.0f;
        public float ROB_X9PTeachPosition_P7_S2 = 0.0f;
        public float ROB_X9PTeachPosition_P8_S2 = 0.0f;
        public float ROB_X9PTeachPosition_P9_S2 = 0.0f;

        public float ROB_Y9PTeachPosition_P1_S2 = 0.0f;
        public float ROB_Y9PTeachPosition_P2_S2 = 0.0f;
        public float ROB_Y9PTeachPosition_P3_S2 = 0.0f;
        public float ROB_Y9PTeachPosition_P4_S2 = 0.0f;
        public float ROB_Y9PTeachPosition_P5_S2 = 0.0f;
        public float ROB_Y9PTeachPosition_P6_S2 = 0.0f;
        public float ROB_Y9PTeachPosition_P7_S2 = 0.0f;
        public float ROB_Y9PTeachPosition_P8_S2 = 0.0f;
        public float ROB_Y9PTeachPosition_P9_S2 = 0.0f;

        public float ROB_Z9PTeachPosition_P1_S2 = 0.0f;
        public float ROB_Z9PTeachPosition_P2_S2 = 0.0f;
        public float ROB_Z9PTeachPosition_P3_S2 = 0.0f;
        public float ROB_Z9PTeachPosition_P4_S2 = 0.0f;
        public float ROB_Z9PTeachPosition_P5_S2 = 0.0f;
        public float ROB_Z9PTeachPosition_P6_S2 = 0.0f;
        public float ROB_Z9PTeachPosition_P7_S2 = 0.0f;
        public float ROB_Z9PTeachPosition_P8_S2 = 0.0f;
        public float ROB_Z9PTeachPosition_P9_S2 = 0.0f;

        public float ROB_RX9PTeachPosition_P1_S2 = 0.0f;
        public float ROB_RX9PTeachPosition_P2_S2 = 0.0f;
        public float ROB_RX9PTeachPosition_P3_S2 = 0.0f;
        public float ROB_RX9PTeachPosition_P4_S2 = 0.0f;
        public float ROB_RX9PTeachPosition_P5_S2 = 0.0f;
        public float ROB_RX9PTeachPosition_P6_S2 = 0.0f;
        public float ROB_RX9PTeachPosition_P7_S2 = 0.0f;
        public float ROB_RX9PTeachPosition_P8_S2 = 0.0f;
        public float ROB_RX9PTeachPosition_P9_S2 = 0.0f;

        public float ROB_RW9PTeachPosition_P1_S2 = 0.0f;
        public float ROB_RW9PTeachPosition_P2_S2 = 0.0f;
        public float ROB_RW9PTeachPosition_P3_S2 = 0.0f;
        public float ROB_RW9PTeachPosition_P4_S2 = 0.0f;
        public float ROB_RW9PTeachPosition_P5_S2 = 0.0f;
        public float ROB_RW9PTeachPosition_P6_S2 = 0.0f;
        public float ROB_RW9PTeachPosition_P7_S2 = 0.0f;
        public float ROB_RW9PTeachPosition_P8_S2 = 0.0f;
        public float ROB_RW9PTeachPosition_P9_S2 = 0.0f;

        public float ROB_RZ9PTeachPosition_P1_S2 = 0.0f;
        public float ROB_RZ9PTeachPosition_P2_S2 = 0.0f;
        public float ROB_RZ9PTeachPosition_P3_S2 = 0.0f;
        public float ROB_RZ9PTeachPosition_P4_S2 = 0.0f;
        public float ROB_RZ9PTeachPosition_P5_S2 = 0.0f;
        public float ROB_RZ9PTeachPosition_P6_S2 = 0.0f;
        public float ROB_RZ9PTeachPosition_P7_S2 = 0.0f;
        public float ROB_RZ9PTeachPosition_P8_S2 = 0.0f;
        public float ROB_RZ9PTeachPosition_P9_S2 = 0.0f;

        public float Pixel_X9PTeachPosition_P1_S2 = 0.0f;
        public float Pixel_X9PTeachPosition_P2_S2 = 0.0f;
        public float Pixel_X9PTeachPosition_P3_S2 = 0.0f;
        public float Pixel_X9PTeachPosition_P4_S2 = 0.0f;
        public float Pixel_X9PTeachPosition_P5_S2 = 0.0f;
        public float Pixel_X9PTeachPosition_P6_S2 = 0.0f;
        public float Pixel_X9PTeachPosition_P7_S2 = 0.0f;
        public float Pixel_X9PTeachPosition_P8_S2 = 0.0f;
        public float Pixel_X9PTeachPosition_P9_S2 = 0.0f;

        public float Pixel_Y9PTeachPosition_P1_S2 = 0.0f;
        public float Pixel_Y9PTeachPosition_P2_S2 = 0.0f;
        public float Pixel_Y9PTeachPosition_P3_S2 = 0.0f;
        public float Pixel_Y9PTeachPosition_P4_S2 = 0.0f;
        public float Pixel_Y9PTeachPosition_P5_S2 = 0.0f;
        public float Pixel_Y9PTeachPosition_P6_S2 = 0.0f;
        public float Pixel_Y9PTeachPosition_P7_S2 = 0.0f;
        public float Pixel_Y9PTeachPosition_P8_S2 = 0.0f;
        public float Pixel_Y9PTeachPosition_P9_S2 = 0.0f;

        public static string _cameraName_S2;//相机出厂名字

        public float Pixel_X1_9PCalibrate_S2 = 0.0f;//9点标定点中心点1 X像素坐标
        public float Pixel_Y1_9PCalibrate_S2 = 0.0f;//9点标定点中心点1 Y像素坐标
        public float Pixel_X2_9PCalibrate_S2 = 0.0f;//9点标定点中心点2 X像素坐标
        public float Pixel_Y2_9PCalibrate_S2 = 0.0f;//9点标定点中心点2 Y像素坐标
        public float Pixel_X3_9PCalibrate_S2 = 0.0f;//9点标定点中心点3 X像素坐标
        public float Pixel_Y3_9PCalibrate_S2 = 0.0f;//9点标定点中心点3 Y像素坐标
        public float Pixel_X4_9PCalibrate_S2 = 0.0f;//9点标定点中心点4 X像素坐标
        public float Pixel_Y4_9PCalibrate_S2 = 0.0f;//9点标定点中心点4 Y像素坐标
        public float Pixel_X5_9PCalibrate_S2 = 0.0f;//9点标定点中心点5 X像素坐标
        public float Pixel_Y5_9PCalibrate_S2 = 0.0f;//9点标定点中心点5 Y像素坐标
        public float Pixel_X6_9PCalibrate_S2 = 0.0f;//9点标定点中心点6 X像素坐标
        public float Pixel_Y6_9PCalibrate_S2 = 0.0f;//9点标定点中心点6 Y像素坐标
        public float Pixel_X7_9PCalibrate_S2 = 0.0f;//9点标定点中心点7 X像素坐标
        public float Pixel_Y7_9PCalibrate_S2 = 0.0f;//9点标定点中心点7 Y像素坐标
        public float Pixel_X8_9PCalibrate_S2 = 0.0f;//9点标定点中心点8 X像素坐标
        public float Pixel_Y8_9PCalibrate_S2 = 0.0f;//9点标定点中心点8 Y像素坐标
        public float Pixel_X9_9PCalibrate_S2 = 0.0f;//9点标定点中心点9 X像素坐标
        public float Pixel_Y9_9PCalibrate_S2 = 0.0f;//9点标定点中心点9 Y像素坐标

        //示教物料拍照获取数据的变量
        public float ROB_XPickupTeachPosition_S2 = 0.0f;//示教的ROB取料位置X坐标
        public float ROB_YPickupTeachPosition_S2 = 0.0f;//示教的ROB取料位置Y坐标
        public float ROB_ZPickupTeachPosition_S2 = 0.0f;//示教的ROB取料位置Z坐标
        public float ROB_RXPickupTeachPosition_S2 = 0.0f;//示教的ROB取料位置RX坐标
        public float ROB_RWPickupTeachPosition_S2 = 0.0f;//示教的ROB取料位置RW坐标
        public float ROB_RZPickupTeachPosition_S2 = 0.0f;//示教的ROB取料位置RZ坐标
        public float Pixel_XPickupTeachPosition_RT_S2 = 0.0f;//示教物料的右上顶点X像素坐标
        public float Pixel_YPickupTeachPosition_RT_S2 = 0.0f;//示教物料的右上顶点Y像素坐标
        public float Pixel_XPickupTeachPosition_LT_S2 = 0.0f;//示教物料的左上顶点X像素坐标
        public float Pixel_YPickupTeachPosition_LT_S2 = 0.0f;//示教物料的左上顶点Y像素坐标
        public float Pixel_XPickupTeachPosition_LB_S2 = 0.0f;//示教物料的左下顶点X像素坐标
        public float Pixel_YPickupTeachPosition_LB_S2 = 0.0f;//示教物料的左下顶点Y像素坐标
        public float Pixel_XPickupTeachPosition_RB_S2 = 0.0f;//示教物料的右下顶点X像素坐标
        public float Pixel_YPickupTeachPosition_RB_S2 = 0.0f;//示教物料的右下顶点Y像素坐标
        public float Pixel_AnglePickupTeach_S2 = 0.0f;//示教物料的像素坐标系角度
        public float ROB_XPickupTeachPosition_RT_S2 = 0.0f;//示教物料的右上顶点X ROB坐标----暂时没有用到
        public float ROB_YPickupTeachPosition_RT_S2 = 0.0f;//示教物料的右上顶点Y ROB坐标----暂时没有用到
        public float ROB_XPickupTeachPosition_LT_S2 = 0.0f;//示教物料的左上顶点X ROB坐标----暂时没有用到
        public float ROB_YPickupTeachPosition_LT_S2 = 0.0f;//示教物料的左上顶点Y ROB坐标----暂时没有用到
        public float ROB_XPickupTeachPosition_LB_S2 = 0.0f;//示教物料的左下顶点X ROB坐标----暂时没有用到
        public float ROB_YPickupTeachPosition_LB_S2 = 0.0f;//示教物料的左下顶点Y ROB坐标----暂时没有用到
        public float ROB_XPickupTeachPosition_RB_S2 = 0.0f;//示教物料的右下顶点X ROB坐标----暂时没有用到
        public float ROB_YPickupTeachPosition_RB_S2 = 0.0f;//示教物料的右下顶点Y ROB坐标----暂时没有用到

        //临时变量
        public float Pixel_XCenterPickupCurrentPosition_S2;//当前物料的中心点X坐标
        public float Pixel_YCenterPickupCurrentPosition_S2;//当前物料的中心点Y坐标
        public float Pixel_AnglePickupCurrent_S2 = 0.0f;//当前物料的像素坐标系角度
        public float Pixel_WidthPickupCurrent_S2;//当前物料的宽度
        public float Pixel_HeightPickupCurrent_S2;//当前物料的高度
        public float Pixel_XPickupCurrentPosition_RT_S2 = 0.0f;//当前物料的右上顶点X像素坐标
        public float Pixel_YPickupCurrentPosition_RT_S2 = 0.0f;//当前物料的右上顶点Y像素坐标
        public float Pixel_XPickupCurrentPosition_LT_S2 = 0.0f;//当前物料的左上顶点X像素坐标
        public float Pixel_YPickupCurrentPosition_LT_S2 = 0.0f;//当前物料的左上顶点Y像素坐标
        public float Pixel_XPickupCurrentPosition_LB_S2 = 0.0f;//当前物料的左下顶点X像素坐标
        public float Pixel_YPickupCurrentPosition_LB_S2 = 0.0f;//当前物料的左下顶点Y像素坐标
        public float Pixel_XPickupCurrentPosition_RB_S2 = 0.0f;//当前物料的右下顶点X像素坐标
        public float Pixel_YPickupCurrentPosition_RB_S2 = 0.0f;//当前物料的右下顶点Y像素坐标

        public float ROB_XPickupCurrentPosition_RT_S2 = 0.0f;//当前物料的右上顶点X ROB坐标
        public float ROB_YPickupCurrentPosition_RT_S2 = 0.0f;//当前物料的右上顶点Y ROB坐标
        public float ROB_XPickupCurrentPosition_LT_S2 = 0.0f;//当前物料的左上顶点X ROB坐标
        public float ROB_YPickupCurrentPosition_LT_S2 = 0.0f;//当前物料的左上顶点Y ROB坐标
        public float ROB_XPickupCurrentPosition_LB_S2 = 0.0f;//当前物料的左下顶点X ROB坐标
        public float ROB_YPickupCurrentPosition_LB_S2 = 0.0f;//当前物料的左下顶点Y ROB坐标
        public float ROB_XPickupCurrentPosition_RB_S2 = 0.0f;//当前物料的右下顶点X ROB坐标
        public float ROB_YPickupCurrentPosition_RB_S2 = 0.0f;//当前物料的右下顶点Y ROB坐标

        HTuple hv_HomMat2D_9P_S2 = new HTuple();//9点标定数据

        public float Pixel_AnglePickupOffset_S2 = 0.0f;//物料的角度偏移
        public float ROB_XPickupTeachPosition_RT_CurrentProduct_S2 = 0.0f;//当前产品的示教物料的右上顶点X ROB坐标
        public float ROB_YPickupTeachPosition_RT_CurrentProduct_S2 = 0.0f;//当前产品的示教物料的右上顶点Y ROB坐标
        public float ROB_XPickupTeachPosition_LT_CurrentProduct_S2 = 0.0f;//当前产品的示教物料的左上顶点X ROB坐标
        public float ROB_YPickupTeachPosition_LT_CurrentProduct_S2 = 0.0f;//当前产品的示教物料的左上顶点Y ROB坐标
        public float ROB_XPickupTeachPosition_LB_CurrentProduct_S2 = 0.0f;//当前产品的示教物料的左下顶点X ROB坐标
        public float ROB_YPickupTeachPosition_LB_CurrentProduct_S2 = 0.0f;//当前产品的示教物料的左下顶点Y ROB坐标
        public float ROB_XPickupTeachPosition_RB_CurrentProduct_S2 = 0.0f;//当前产品的示教物料的右下顶点X ROB坐标
        public float ROB_YPickupTeachPosition_RB_CurrentProduct_S2 = 0.0f;//当前产品的示教物料的右下顶点Y ROB坐标
        public HTuple hv_HomMat2D_CurrentTranslateMatrix_S2 = new HTuple();//用4个顶点坐标计算示教物料位置变换到当前物料位置的矩阵hv_HomMat2D_CurrentTranslateMatrix
        public HTuple hv_HomMat2D_CurrentTranslateMatrix_Pixel_S2 = new HTuple();//用4个顶点像素坐标计算示教物料位置变换到当前物料位置的矩阵hv_HomMat2D_CurrentTranslateMatrix_Pixel
        //最终的当前取料位置
        public static float ROB_XPickupCurrentPosition_S2 = 0.0f;
        public static float ROB_YPickupCurrentPosition_S2 = 0.0f;
        public static float ROB_ZPickupCurrentPosition_S2 = 0.0f;
        public static float ROB_RXPickupCurrentPosition_S2 = 0.0f;
        public static float ROB_RWPickupCurrentPosition_S2 = 0.0f;
        public static float ROB_RZPickupCurrentPosition_S2 = 0.0f;

        //产品定位检测算子变量
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

        //限制条件变量
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

        //Station3
        //9点标定参数
        public float ROB_X9PTeachPosition_P1_S3 = 0.0f;
        public float ROB_X9PTeachPosition_P2_S3 = 0.0f;
        public float ROB_X9PTeachPosition_P3_S3 = 0.0f;
        public float ROB_X9PTeachPosition_P4_S3 = 0.0f;
        public float ROB_X9PTeachPosition_P5_S3 = 0.0f;
        public float ROB_X9PTeachPosition_P6_S3 = 0.0f;
        public float ROB_X9PTeachPosition_P7_S3 = 0.0f;
        public float ROB_X9PTeachPosition_P8_S3 = 0.0f;
        public float ROB_X9PTeachPosition_P9_S3 = 0.0f;

        public float ROB_Y9PTeachPosition_P1_S3 = 0.0f;
        public float ROB_Y9PTeachPosition_P2_S3 = 0.0f;
        public float ROB_Y9PTeachPosition_P3_S3 = 0.0f;
        public float ROB_Y9PTeachPosition_P4_S3 = 0.0f;
        public float ROB_Y9PTeachPosition_P5_S3 = 0.0f;
        public float ROB_Y9PTeachPosition_P6_S3 = 0.0f;
        public float ROB_Y9PTeachPosition_P7_S3 = 0.0f;
        public float ROB_Y9PTeachPosition_P8_S3 = 0.0f;
        public float ROB_Y9PTeachPosition_P9_S3 = 0.0f;

        public float ROB_Z9PTeachPosition_P1_S3 = 0.0f;
        public float ROB_Z9PTeachPosition_P2_S3 = 0.0f;
        public float ROB_Z9PTeachPosition_P3_S3 = 0.0f;
        public float ROB_Z9PTeachPosition_P4_S3 = 0.0f;
        public float ROB_Z9PTeachPosition_P5_S3 = 0.0f;
        public float ROB_Z9PTeachPosition_P6_S3 = 0.0f;
        public float ROB_Z9PTeachPosition_P7_S3 = 0.0f;
        public float ROB_Z9PTeachPosition_P8_S3 = 0.0f;
        public float ROB_Z9PTeachPosition_P9_S3 = 0.0f;

        public float ROB_RX9PTeachPosition_P1_S3 = 0.0f;
        public float ROB_RX9PTeachPosition_P2_S3 = 0.0f;
        public float ROB_RX9PTeachPosition_P3_S3 = 0.0f;
        public float ROB_RX9PTeachPosition_P4_S3 = 0.0f;
        public float ROB_RX9PTeachPosition_P5_S3 = 0.0f;
        public float ROB_RX9PTeachPosition_P6_S3 = 0.0f;
        public float ROB_RX9PTeachPosition_P7_S3 = 0.0f;
        public float ROB_RX9PTeachPosition_P8_S3 = 0.0f;
        public float ROB_RX9PTeachPosition_P9_S3 = 0.0f;

        public float ROB_RW9PTeachPosition_P1_S3 = 0.0f;
        public float ROB_RW9PTeachPosition_P2_S3 = 0.0f;
        public float ROB_RW9PTeachPosition_P3_S3 = 0.0f;
        public float ROB_RW9PTeachPosition_P4_S3 = 0.0f;
        public float ROB_RW9PTeachPosition_P5_S3 = 0.0f;
        public float ROB_RW9PTeachPosition_P6_S3 = 0.0f;
        public float ROB_RW9PTeachPosition_P7_S3 = 0.0f;
        public float ROB_RW9PTeachPosition_P8_S3 = 0.0f;
        public float ROB_RW9PTeachPosition_P9_S3 = 0.0f;

        public float ROB_RZ9PTeachPosition_P1_S3 = 0.0f;
        public float ROB_RZ9PTeachPosition_P2_S3 = 0.0f;
        public float ROB_RZ9PTeachPosition_P3_S3 = 0.0f;
        public float ROB_RZ9PTeachPosition_P4_S3 = 0.0f;
        public float ROB_RZ9PTeachPosition_P5_S3 = 0.0f;
        public float ROB_RZ9PTeachPosition_P6_S3 = 0.0f;
        public float ROB_RZ9PTeachPosition_P7_S3 = 0.0f;
        public float ROB_RZ9PTeachPosition_P8_S3 = 0.0f;
        public float ROB_RZ9PTeachPosition_P9_S3 = 0.0f;

        public float Pixel_X9PTeachPosition_P1_S3 = 0.0f;
        public float Pixel_X9PTeachPosition_P2_S3 = 0.0f;
        public float Pixel_X9PTeachPosition_P3_S3 = 0.0f;
        public float Pixel_X9PTeachPosition_P4_S3 = 0.0f;
        public float Pixel_X9PTeachPosition_P5_S3 = 0.0f;
        public float Pixel_X9PTeachPosition_P6_S3 = 0.0f;
        public float Pixel_X9PTeachPosition_P7_S3 = 0.0f;
        public float Pixel_X9PTeachPosition_P8_S3 = 0.0f;
        public float Pixel_X9PTeachPosition_P9_S3 = 0.0f;

        public float Pixel_Y9PTeachPosition_P1_S3 = 0.0f;
        public float Pixel_Y9PTeachPosition_P2_S3 = 0.0f;
        public float Pixel_Y9PTeachPosition_P3_S3 = 0.0f;
        public float Pixel_Y9PTeachPosition_P4_S3 = 0.0f;
        public float Pixel_Y9PTeachPosition_P5_S3 = 0.0f;
        public float Pixel_Y9PTeachPosition_P6_S3 = 0.0f;
        public float Pixel_Y9PTeachPosition_P7_S3 = 0.0f;
        public float Pixel_Y9PTeachPosition_P8_S3 = 0.0f;
        public float Pixel_Y9PTeachPosition_P9_S3 = 0.0f;

        public static string _cameraName_S3;//相机出厂名字

        public float Pixel_X1_9PCalibrate_S3 = 0.0f;//9点标定点中心点1 X像素坐标
        public float Pixel_Y1_9PCalibrate_S3 = 0.0f;//9点标定点中心点1 Y像素坐标
        public float Pixel_X2_9PCalibrate_S3 = 0.0f;//9点标定点中心点2 X像素坐标
        public float Pixel_Y2_9PCalibrate_S3 = 0.0f;//9点标定点中心点2 Y像素坐标
        public float Pixel_X3_9PCalibrate_S3 = 0.0f;//9点标定点中心点3 X像素坐标
        public float Pixel_Y3_9PCalibrate_S3 = 0.0f;//9点标定点中心点3 Y像素坐标
        public float Pixel_X4_9PCalibrate_S3 = 0.0f;//9点标定点中心点4 X像素坐标
        public float Pixel_Y4_9PCalibrate_S3 = 0.0f;//9点标定点中心点4 Y像素坐标
        public float Pixel_X5_9PCalibrate_S3 = 0.0f;//9点标定点中心点5 X像素坐标
        public float Pixel_Y5_9PCalibrate_S3 = 0.0f;//9点标定点中心点5 Y像素坐标
        public float Pixel_X6_9PCalibrate_S3 = 0.0f;//9点标定点中心点6 X像素坐标
        public float Pixel_Y6_9PCalibrate_S3 = 0.0f;//9点标定点中心点6 Y像素坐标
        public float Pixel_X7_9PCalibrate_S3 = 0.0f;//9点标定点中心点7 X像素坐标
        public float Pixel_Y7_9PCalibrate_S3 = 0.0f;//9点标定点中心点7 Y像素坐标
        public float Pixel_X8_9PCalibrate_S3 = 0.0f;//9点标定点中心点8 X像素坐标
        public float Pixel_Y8_9PCalibrate_S3 = 0.0f;//9点标定点中心点8 Y像素坐标
        public float Pixel_X9_9PCalibrate_S3 = 0.0f;//9点标定点中心点9 X像素坐标
        public float Pixel_Y9_9PCalibrate_S3 = 0.0f;//9点标定点中心点9 Y像素坐标

        //示教物料拍照获取数据的变量
        public float ROB_XPickupTeachPosition_S3 = 0.0f;//示教的ROB取料位置X坐标
        public float ROB_YPickupTeachPosition_S3 = 0.0f;//示教的ROB取料位置Y坐标
        public float ROB_ZPickupTeachPosition_S3 = 0.0f;//示教的ROB取料位置Z坐标
        public float ROB_RXPickupTeachPosition_S3 = 0.0f;//示教的ROB取料位置RX坐标
        public float ROB_RWPickupTeachPosition_S3 = 0.0f;//示教的ROB取料位置RW坐标
        public float ROB_RZPickupTeachPosition_S3 = 0.0f;//示教的ROB取料位置RZ坐标
        public float Pixel_XPickupTeachPosition_RT_S3 = 0.0f;//示教物料的右上顶点X像素坐标
        public float Pixel_YPickupTeachPosition_RT_S3 = 0.0f;//示教物料的右上顶点Y像素坐标
        public float Pixel_XPickupTeachPosition_LT_S3 = 0.0f;//示教物料的左上顶点X像素坐标
        public float Pixel_YPickupTeachPosition_LT_S3 = 0.0f;//示教物料的左上顶点Y像素坐标
        public float Pixel_XPickupTeachPosition_LB_S3 = 0.0f;//示教物料的左下顶点X像素坐标
        public float Pixel_YPickupTeachPosition_LB_S3 = 0.0f;//示教物料的左下顶点Y像素坐标
        public float Pixel_XPickupTeachPosition_RB_S3 = 0.0f;//示教物料的右下顶点X像素坐标
        public float Pixel_YPickupTeachPosition_RB_S3 = 0.0f;//示教物料的右下顶点Y像素坐标
        public float Pixel_AnglePickupTeach_S3 = 0.0f;//示教物料的像素坐标系角度
        public float ROB_XPickupTeachPosition_RT_S3 = 0.0f;//示教物料的右上顶点X ROB坐标----暂时没有用到
        public float ROB_YPickupTeachPosition_RT_S3 = 0.0f;//示教物料的右上顶点Y ROB坐标----暂时没有用到
        public float ROB_XPickupTeachPosition_LT_S3 = 0.0f;//示教物料的左上顶点X ROB坐标----暂时没有用到
        public float ROB_YPickupTeachPosition_LT_S3 = 0.0f;//示教物料的左上顶点Y ROB坐标----暂时没有用到
        public float ROB_XPickupTeachPosition_LB_S3 = 0.0f;//示教物料的左下顶点X ROB坐标----暂时没有用到
        public float ROB_YPickupTeachPosition_LB_S3 = 0.0f;//示教物料的左下顶点Y ROB坐标----暂时没有用到
        public float ROB_XPickupTeachPosition_RB_S3 = 0.0f;//示教物料的右下顶点X ROB坐标----暂时没有用到
        public float ROB_YPickupTeachPosition_RB_S3 = 0.0f;//示教物料的右下顶点Y ROB坐标----暂时没有用到

        //临时变量
        public float Pixel_XCenterPickupCurrentPosition_S3;//当前物料的中心点X坐标
        public float Pixel_YCenterPickupCurrentPosition_S3;//当前物料的中心点Y坐标
        public float Pixel_AnglePickupCurrent_S3 = 0.0f;//当前物料的像素坐标系角度
        public float Pixel_WidthPickupCurrent_S3;//当前物料的宽度
        public float Pixel_HeightPickupCurrent_S3;//当前物料的高度
        public float Pixel_XPickupCurrentPosition_RT_S3 = 0.0f;//当前物料的右上顶点X像素坐标
        public float Pixel_YPickupCurrentPosition_RT_S3 = 0.0f;//当前物料的右上顶点Y像素坐标
        public float Pixel_XPickupCurrentPosition_LT_S3 = 0.0f;//当前物料的左上顶点X像素坐标
        public float Pixel_YPickupCurrentPosition_LT_S3 = 0.0f;//当前物料的左上顶点Y像素坐标
        public float Pixel_XPickupCurrentPosition_LB_S3 = 0.0f;//当前物料的左下顶点X像素坐标
        public float Pixel_YPickupCurrentPosition_LB_S3 = 0.0f;//当前物料的左下顶点Y像素坐标
        public float Pixel_XPickupCurrentPosition_RB_S3 = 0.0f;//当前物料的右下顶点X像素坐标
        public float Pixel_YPickupCurrentPosition_RB_S3 = 0.0f;//当前物料的右下顶点Y像素坐标

        public float ROB_XPickupCurrentPosition_RT_S3 = 0.0f;//当前物料的右上顶点X ROB坐标
        public float ROB_YPickupCurrentPosition_RT_S3 = 0.0f;//当前物料的右上顶点Y ROB坐标
        public float ROB_XPickupCurrentPosition_LT_S3 = 0.0f;//当前物料的左上顶点X ROB坐标
        public float ROB_YPickupCurrentPosition_LT_S3 = 0.0f;//当前物料的左上顶点Y ROB坐标
        public float ROB_XPickupCurrentPosition_LB_S3 = 0.0f;//当前物料的左下顶点X ROB坐标
        public float ROB_YPickupCurrentPosition_LB_S3 = 0.0f;//当前物料的左下顶点Y ROB坐标
        public float ROB_XPickupCurrentPosition_RB_S3 = 0.0f;//当前物料的右下顶点X ROB坐标
        public float ROB_YPickupCurrentPosition_RB_S3 = 0.0f;//当前物料的右下顶点Y ROB坐标

        HTuple hv_HomMat2D_9P_S3 = new HTuple();//9点标定数据

        public float Pixel_AnglePickupOffset_S3 = 0.0f;//物料的角度偏移
        public float ROB_XPickupTeachPosition_RT_CurrentProduct_S3 = 0.0f;//当前产品的示教物料的右上顶点X ROB坐标
        public float ROB_YPickupTeachPosition_RT_CurrentProduct_S3 = 0.0f;//当前产品的示教物料的右上顶点Y ROB坐标
        public float ROB_XPickupTeachPosition_LT_CurrentProduct_S3 = 0.0f;//当前产品的示教物料的左上顶点X ROB坐标
        public float ROB_YPickupTeachPosition_LT_CurrentProduct_S3 = 0.0f;//当前产品的示教物料的左上顶点Y ROB坐标
        public float ROB_XPickupTeachPosition_LB_CurrentProduct_S3 = 0.0f;//当前产品的示教物料的左下顶点X ROB坐标
        public float ROB_YPickupTeachPosition_LB_CurrentProduct_S3 = 0.0f;//当前产品的示教物料的左下顶点Y ROB坐标
        public float ROB_XPickupTeachPosition_RB_CurrentProduct_S3 = 0.0f;//当前产品的示教物料的右下顶点X ROB坐标
        public float ROB_YPickupTeachPosition_RB_CurrentProduct_S3 = 0.0f;//当前产品的示教物料的右下顶点Y ROB坐标
        public HTuple hv_HomMat2D_CurrentTranslateMatrix_S3 = new HTuple();//用4个顶点坐标计算示教物料位置变换到当前物料位置的矩阵hv_HomMat2D_CurrentTranslateMatrix
        public HTuple hv_HomMat2D_CurrentTranslateMatrix_Pixel_S3 = new HTuple();//用4个顶点像素坐标计算示教物料位置变换到当前物料位置的矩阵hv_HomMat2D_CurrentTranslateMatrix_Pixel
        //最终的当前取料位置
        public static float ROB_XPickupCurrentPosition_S3 = 0.0f;
        public static float ROB_YPickupCurrentPosition_S3 = 0.0f;
        public static float ROB_ZPickupCurrentPosition_S3 = 0.0f;
        public static float ROB_RXPickupCurrentPosition_S3 = 0.0f;
        public static float ROB_RWPickupCurrentPosition_S3 = 0.0f;
        public static float ROB_RZPickupCurrentPosition_S3 = 0.0f;

        //产品定位检测算子变量
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

        //限制条件变量
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

        public int _classificationResult_S3 = -1;//(-1:分类运算错误;-2:不属于这3类;1:产品;2:牛皮纸;3:空)

        public bool AngleRotate_S1 = false;
        public bool AngleRotate_S2 = false;
        public bool AngleRotate_S3 = false;
        #endregion

        #region 构造函数
        public CalibrateForm()
        {
            InitializeComponent();

            CheckAllCtrls(this);
            allCtrlsArray = allCtrlsQueue.ToArray();

            InitialCtrlBoxCorrespondingStringArr();

            LoadingDataFromXmlFile();//最基本参数的Setting.xml文件读取
            LoadingVariateParameterFromXmlFile();//最基本参数的Current.rcpActuals文件读取

            if (!File.Exists(_GeneralParameterDirectoryPath))
                generalParameterLoadingToCtrl();//通用参数的读取
            else
            {
                MyTool.TxtFileProcess.CreateLog("通用参数文件不存在: " + _GeneralParameterDirectoryPath + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("通用参数文件不存在: " + _GeneralParameterDirectoryPath);
            }

            //获取指定文件夹中的文件名称并赋值到编辑产品combobox中
            GetProductDataFileNameAndSetAvaliableProductComboBox();
            if (CheckCurrentProductUsefulOrNot(ref _currentProductName, true))//检测当前产品是否可用
                _editProductName = _currentProductName;

            if (ProductDataLoadingToCtrl(_editProductName))
            {
                _editProductRecipe.ProductDataLoadToVariate(_ProductParameterDirectoryPath + "\\" + _editProductName + ".rcp");
            }
            else
            {
                _editProductRecipe._parameterLoadSuccessfulFlag = false;
            }

            ////初始化模板
            smf_S1.Init($"{_GeneralParameterDirectoryPath}/{ModelPath_S1}");
            smf_S2.Init($"{_GeneralParameterDirectoryPath}/{ModelPath_S2}");
            smf_S3.Init($"{_GeneralParameterDirectoryPath}/{ModelPath_S3}");
            //初始化检测参数
            dsf_S1.Init($"{_GeneralParameterDirectoryPath}/{ModelPath_S1}", smf_S1.OrigImage, smf_S1.AffineModelContour);
            dsf_S2.Init($"{_GeneralParameterDirectoryPath}/{ModelPath_S2}", smf_S2.OrigImage, smf_S2.AffineModelContour);
            dsf_S3.Init($"{_GeneralParameterDirectoryPath}/{ModelPath_S3}", smf_S3.OrigImage, smf_S3.AffineModelContour);

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

        #region 获取指定文件夹中的文件名称并赋值到编辑产品combobox中
        //获取指定文件夹中的文件名称并赋值到产品combobox中
        public void GetProductDataFileNameAndSetAvaliableProductComboBox()
        {
            bool currentProductUsefulFlag = false;
            string[] fileName;
            int currentProductIndex = -1;
            if (Directory.Exists(_ProductParameterDirectoryPath))
            {
                fileName = FolderAndFileManageClass.ScanFolderAndGetAssignedFormatFileName(_ProductParameterDirectoryPath, ".rcp");

                cboAvaliableProduct.Items.Clear();
                for (int i = 0; i < fileName.Length; i++)
                {
                    if (_currentProductName == fileName[i])
                    {
                        currentProductIndex = i;
                        currentProductUsefulFlag = true;
                    }
                    cboAvaliableProduct.Items.Add(fileName[i]);
                }
            }

            if (!currentProductUsefulFlag)
            {
                _currentProductName = "";
            }
            cboAvaliableProduct.SelectedIndex = currentProductIndex;
        }
        #endregion

        //需要检查更新到变量的情况是否有遗漏
        #region 最基本参数的Setting.xml文件的保存与读取----控件

        string _xmlFileParentData = "Settings General";
        string _xmlFileRootNodeName = "ArrayOfClsObjectProperties";
        string _xmlFileMainNodeName = "clsObjectProperties";

        #region 保存
        /// <summary>
        /// 保存
        /// </summary>
        public void SavingDataToXmlFile()
        {
            MyTool.TxtFileProcess.CreateLog("参数设置-保存数据至文件中！" + "----用户：" + ActionForm._operatorName,
            ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);

            string fileDirectory = Directory.GetCurrentDirectory() + "\\";
            string fileName = "Setting.xml";
            string filePath = fileDirectory + fileName;
            string tempCtrlName;
            string tempValue = "";
            bool needSaveDataCtrlFlag = false;//用于标识当前控件是否用于保存数据

            if (!File.Exists(filePath))
            {
                XMLFileOperation obj = new XMLFileOperation();
                obj.CreateXmlRoot(_xmlFileRootNodeName);//创建声明及根节点
                obj.XmlSave(filePath);
                obj.Dispose();
                //修改根节点的属性
                XMLFileOperation.updateNodeValueOrAttribute(filePath, _xmlFileRootNodeName, "xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");
                XMLFileOperation.updateNodeValueOrAttribute(filePath, _xmlFileRootNodeName, "xmlns:xsd", "http://www.w3.org/2001/XMLSchema");
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
                XmlElement xmlElem = xmlDoc.CreateElement("", _xmlFileRootNodeName, "");
                xmlDoc.AppendChild(xmlElem);

                //修改根节点的属性
                XmlNode node = xmlDoc.SelectSingleNode(_xmlFileRootNodeName);
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

            for (int i = 0; i < allCtrlsArray.Length; i++)
            {
                needSaveDataCtrlFlag = false;
                tempCtrlName = allCtrlsArray[i].Name.ToString();
                try
                {
                    switch (tempCtrlName)
                    {
                        case "txtGeneralParameterDirectory":
                        case "txtProductParameterDirectory":
                        case "txtProductDataDirectory":
                            {
                                tempValue = "";
                                tempValue = allCtrlsArray[i].Text.ToString();

                                needSaveDataCtrlFlag = true;
                                break;
                            }
                    }

                    if (needSaveDataCtrlFlag)
                    {
                        XmlNodeList nodes = xmlDoc.DocumentElement.SelectNodes(@"/" + _xmlFileRootNodeName + @"/" + _xmlFileMainNodeName + @"/Name");//获取所有的Name的值
                        bool elementExistFlag = false;//用于标志是否存在此元素
                        foreach (XmlNode nodeChild in nodes)
                        {
                            if (nodeChild.InnerText == tempCtrlName)//如果此元素存在
                            {
                                foreach (XmlNode nodeChild2 in nodeChild.ParentNode)
                                {
                                    if (nodeChild2.Name == "Value")
                                    {
                                        nodeChild2.InnerText = tempValue;
                                        break;
                                    }
                                }
                                elementExistFlag = true;
                                break;
                            }
                        }

                        if (!elementExistFlag)//如果节点不存在
                        {
                            string[] s1 = { "Parent", "Name", "Value", "Tag" };
                            string[] s2 = { _xmlFileParentData, tempCtrlName, tempValue, tempCtrlName.Substring(3, tempCtrlName.Length - 3) };
                            XmlNode objRootNode = xmlDoc.SelectSingleNode(_xmlFileRootNodeName);//或者当前节点
                            XmlElement objChildNode = xmlDoc.CreateElement(_xmlFileMainNodeName);//新建新插入节点
                            objRootNode.AppendChild(objChildNode);//新插入节点Append到当前节点
                            for (int j = 0; j < s1.Length; j++)
                            {
                                XmlElement objElement = xmlDoc.CreateElement(s1[j]);//新建新插入节点的子节点
                                objElement.InnerText = s2[j];

                                objChildNode.AppendChild(objElement);//子节点Append到新插入节点
                            }
                        }
                    }
                }
                catch
                { }
            }
            xmlDoc.Save(filePath);
            xmlDoc = null;

            ParameterRefresh();
        }
        #endregion

        #region 读取
        /// <summary>
        /// 读取
        /// </summary>
        public void LoadingDataFromXmlFile()
        {

            MyTool.TxtFileProcess.CreateLog("参数设置-从文件中读取数据！" + "----用户：" + ActionForm._operatorName,
            ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);

            string fileDirectory = Directory.GetCurrentDirectory() + "\\";
            string fileName = "Setting.xml";
            string filePath = fileDirectory + fileName;
            string tempCtrlName;
            string[] tempValue = { "", "", "", "" };
            bool needLoadDataCtrlFlag = false;//用于标识当前控件是否用于读取数据

            if (!File.Exists(filePath))
            {
                XMLFileOperation obj = new XMLFileOperation();
                obj.CreateXmlRoot(_xmlFileRootNodeName);//创建声明及根节点
                obj.XmlSave(filePath);
                obj.Dispose();
                //修改根节点的属性
                XMLFileOperation.updateNodeValueOrAttribute(filePath, _xmlFileRootNodeName, "xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");
                XMLFileOperation.updateNodeValueOrAttribute(filePath, _xmlFileRootNodeName, "xmlns:xsd", "http://www.w3.org/2001/XMLSchema");

                SavingDataToXmlFile();
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
                XmlElement xmlElem = xmlDoc.CreateElement("", _xmlFileRootNodeName, "");
                xmlDoc.AppendChild(xmlElem);

                //修改根节点的属性
                XmlNode node = xmlDoc.SelectSingleNode(_xmlFileRootNodeName);
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

            for (int i = 0; i < allCtrlsArray.Length; i++)
            {
                needLoadDataCtrlFlag = false;
                tempCtrlName = allCtrlsArray[i].Name.ToString();
                try
                {
                    XmlNodeList nodes = xmlDoc.DocumentElement.SelectNodes(@"/" + _xmlFileRootNodeName + @"/" + _xmlFileMainNodeName + @"/Name");//获取所有的Name的值
                    bool elementExistFlag = false;//用于标志是否存在此元素
                    foreach (XmlNode nodeChild in nodes)
                    {
                        if (nodeChild.InnerText == tempCtrlName)//如果此元素存在
                        {
                            System.Collections.ArrayList al = new System.Collections.ArrayList();
                            foreach (XmlNode nodeChild2 in nodeChild.ParentNode)
                            {
                                al.Add(nodeChild2.InnerText);
                            }
                            for (int j = 0; j < al.Count; j++)
                                tempValue[j] = al[j].ToString();
                            elementExistFlag = true;
                            break;
                        }
                    }

                    switch (tempCtrlName)
                    {
                        case "txtGeneralParameterDirectory":
                        case "txtProductParameterDirectory":
                        case "txtProductDataDirectory":
                            {
                                needLoadDataCtrlFlag = true;
                                if (elementExistFlag)//如果这个节点存在
                                {
                                    allCtrlsArray[i].Text = tempValue[2];
                                }

                                break;
                            }
                    }

                    if (!elementExistFlag && needLoadDataCtrlFlag)//如果元素不存在，并且这个控件需要进行读取数据
                    {
                        string[] s1 = { "Parent", "Name", "Value", "Tag" };
                        string[] s2 = { _xmlFileParentData, tempCtrlName, tempValue[2], tempCtrlName.Substring(3, tempCtrlName.Length - 3) };
                        XmlNode objRootNode = xmlDoc.SelectSingleNode(_xmlFileRootNodeName);//或者当前节点
                        XmlElement objChildNode = xmlDoc.CreateElement(_xmlFileMainNodeName);//新建新插入节点
                        objRootNode.AppendChild(objChildNode);//新插入节点Append到当前节点
                        for (int j = 0; j < s1.Length; j++)
                        {
                            XmlElement objElement = xmlDoc.CreateElement(s1[j]);//新建新插入节点的子节点
                            objElement.InnerText = s2[j];

                            objChildNode.AppendChild(objElement);//子节点Append到新插入节点
                        }
                    }
                }
                catch
                { }
            }
            xmlDoc.Save(filePath);
            xmlDoc = null;

            ParameterRefresh();
        }
        #endregion

        #region 把控件的状态更新到变量
        /// <summary>
        /// 保把控件的状态更新到变量
        /// </summary>
        private void ParameterRefresh()
        {
            MyTool.TxtFileProcess.CreateLog("参数设置-数据更新！" + "----用户：" + ActionForm._operatorName,
            ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);

            _GeneralParameterDirectoryPath = txtGeneralParameterDirectory.Text;//更新通用参数保存路径到变量
            _ProductParameterDirectoryPath = txtProductParameterDirectory.Text;//更新产品参数保存路径到变量
            _ProductDataDirectoryPath = txtProductDataDirectory.Text;//更新产品数据保存路径到变量
        }
        #endregion

        #endregion

        //需要检查更新到变量的情况是否有遗漏
        #region 最基本参数的Current.rcpActuals文件的保存与读取----变量

        string[] _LoadSaveVariateParameterName = { "_currentProductName", "_editProductName" };//用于最基本参数的Current.rcpActuals文件的读取与保存----变量

        #region 保存
        //保存
        public void SavingVariateParameterToXmlFile()
        {
            MyTool.TxtFileProcess.CreateLog("产品设置页面-保存主界面参数！" + "----用户：" + ActionForm._operatorName,
            ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);


            string directoryOfExe = Directory.GetCurrentDirectory();
            string settingFileName = "Current.rcpActuals";
            string settingFilePath = directoryOfExe + "\\" + settingFileName;
            string[] tempValue = new string[_LoadSaveVariateParameterName.Length];
            string rootName = "clsActualsData";

            //获取要保存的参数值
            tempValue[0] = _currentProductName;
            tempValue[1] = _editProductName;

            if (!File.Exists(settingFilePath))
            {
                XMLFileOperation obj = new XMLFileOperation();
                obj.CreateXmlRoot(rootName);//创建声明及根节点
                obj.XmlSave(settingFilePath);
                obj.Dispose();
                //修改根节点的属性
                XMLFileOperation.updateNodeValueOrAttribute(settingFilePath, rootName, "xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");
                XMLFileOperation.updateNodeValueOrAttribute(settingFilePath, rootName, "xmlns:xsd", "http://www.w3.org/2001/XMLSchema");

                for (int i = 0; i < _LoadSaveVariateParameterName.Length; i++)
                {
                    XMLFileOperation.XmlInsertElement(settingFilePath, rootName, _LoadSaveVariateParameterName[i], tempValue[i]);
                }
            }
            else
            {
                //判定格式是否正确，不正确重新创建
                XmlDocument xmlDoc = new XmlDocument();
                try
                {
                    xmlDoc.Load(settingFilePath);
                }
                catch
                {
                    XMLFileOperation obj = new XMLFileOperation();
                    obj.CreateXmlRoot(rootName);//创建声明及根节点
                    obj.XmlSave(settingFilePath);
                    obj.Dispose();
                    //修改根节点的属性
                    XMLFileOperation.updateNodeValueOrAttribute(settingFilePath, rootName, "xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");
                    XMLFileOperation.updateNodeValueOrAttribute(settingFilePath, rootName, "xmlns:xsd", "http://www.w3.org/2001/XMLSchema");
                }

                for (int i = 0; i < _LoadSaveVariateParameterName.Length; i++)
                {
                    if (XMLFileOperation.checkNodeExistOrNot(settingFilePath, _LoadSaveVariateParameterName[i]))
                    {
                        XMLFileOperation.XmlNodeReplace(settingFilePath, rootName + "/" + _LoadSaveVariateParameterName[i], tempValue[i]);
                    }
                    else
                    {
                        XMLFileOperation.XmlInsertElement(settingFilePath, rootName, _LoadSaveVariateParameterName[i], tempValue[i]);
                    }
                }
            }

            VariateParameterRefresh();
        }
        #endregion

        #region 读取
        //读取
        public void LoadingVariateParameterFromXmlFile()
        {
            MyTool.TxtFileProcess.CreateLog("产品设置页面-读取主界面参数！" + "----用户：" + ActionForm._operatorName,
            ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);

            string directoryOfExe = Directory.GetCurrentDirectory();
            string settingFileName = "Current.rcpActuals";
            string settingFilePath = directoryOfExe + "\\" + settingFileName;
            string[] tempValue = new string[_LoadSaveVariateParameterName.Length];
            string rootName = "clsActualsData";

            //获取要读取的参数值
            tempValue[0] = _currentProductName;
            tempValue[1] = _editProductName;

            if (!File.Exists(settingFilePath))
            {
                SavingVariateParameterToXmlFile();
            }
            else
            {
                //判定格式是否正确，不正确重新创建
                XmlDocument xmlDoc = new XmlDocument();
                try
                {
                    xmlDoc.Load(settingFilePath);
                }
                catch
                {
                    XMLFileOperation obj = new XMLFileOperation();
                    obj.CreateXmlRoot(rootName);//创建声明及根节点
                    obj.XmlSave(settingFilePath);
                    obj.Dispose();
                    //修改根节点的属性
                    XMLFileOperation.updateNodeValueOrAttribute(settingFilePath, rootName, "xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");
                    XMLFileOperation.updateNodeValueOrAttribute(settingFilePath, rootName, "xmlns:xsd", "http://www.w3.org/2001/XMLSchema");
                }
                for (int i = 0; i < _LoadSaveVariateParameterName.Length; i++)
                {
                    if (XMLFileOperation.checkNodeExistOrNot(settingFilePath, _LoadSaveVariateParameterName[i]))
                    {
                        tempValue[i] = XMLFileOperation.findElementValue(settingFilePath, rootName + "/" + _LoadSaveVariateParameterName[i]);
                    }
                    else
                    {
                        XMLFileOperation.XmlInsertElement(settingFilePath, rootName, _LoadSaveVariateParameterName[i], tempValue[i]);
                    }
                }

                _currentProductName = tempValue[0];
                _editProductName = tempValue[1];

            }

            VariateParameterRefresh();
        }
        #endregion

        #region 把变量更新到控件的状态
        //把变量更新到控件的状态
        private void VariateParameterRefresh()
        {
            //显示当前产品名称
            lblCurrentProduct.Text = "当前产品: " + _currentProductName;
        }
        #endregion 把变量更新到控件的状态

        #endregion

        //需要检查更新到变量的情况是否有遗漏
        #region 通用参数的保存与读取----控件

        //const int _generalParameterQuantity = 60;//通用参数个数
        //string[,] _generalParameterCtrlNameAndXmlFileNameStr = new string[2, _generalParameterQuantity];//用于存储需要保存读取的参数变量名和xml名称对应数组,0-控件名称，1-xml文件名称

        #region 保存
        /// <summary>
        /// generalParameterSavingFromCtrl_9PCalibrate:保存通用参数之9点标定(先对控件数据进行9点标定执行,如果OK则把控件数据保存)
        /// </summary>
        /// <param name="FileName">string：保存的文件的名称</param>
        /// <param name="DataCheckFlag">bool：是否进行数据检查，在创建新的产品数据的时候不需要进行数据检查</param>
        /// <returns>bool: 反馈保存结果，true-保存成功，false-保存失败</returns>
        private void generalParameterSavingFromCtrl_9PCalibrate_S(object sender, EventArgs e)
        {
            if (ActionForm._accessLevel < 101)
            {
                MyTool.TxtFileProcess.CreateLog("操作权限不合格:操作权限为工程师或者厂家!" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("操作权限不合格:操作权限为工程师或者厂家!");
                return;
            }

            MyTool.TxtFileProcess.CreateLog("参数设置-保存数据至文件中！" + "----用户：" + ActionForm._operatorName,
            ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);

            string _contorlName = ((Button)sender).Name;
            int _stationNumber = Convert.ToInt32(_contorlName.Substring(_contorlName.Length - 1, 1));

            if (!btn9PCalibrateExecute_ClickEvent_S(_stationNumber))
            {
                MyTool.TxtFileProcess.CreateLog("参数设置-" + "S" + _stationNumber.ToString() + " " + "9点标定数据错误,不能保存！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                return;
            }
            if (!_9PCalibrateResultSave_S(_stationNumber))
            {
                MyTool.TxtFileProcess.CreateLog("参数设置-" + "S" + _stationNumber.ToString() + " " + "9点标定结果保存失败！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                return;
            }
            _9PTeach_ROBAndPixelPositionDataSave_S(_stationNumber);
        }

        private void btn9PTeach_ROBPositionDataSave_S1_Click(object sender, EventArgs e)
        {
            _9PTeach_ROBAndPixelPositionDataSave_S(1);
        }
        private void btn9PTeach_ROBPositionDataSave_S2_Click(object sender, EventArgs e)
        {
            _9PTeach_ROBAndPixelPositionDataSave_S(2);
        }
        private void btn9PTeach_ROBPositionDataSave_S3_Click(object sender, EventArgs e)
        {
            _9PTeach_ROBAndPixelPositionDataSave_S(3);
        }
        private void _9PTeach_ROBAndPixelPositionDataSave_S(int _stationNumber)
        {
            string fileDirectory = @_GeneralParameterDirectoryPath + "\\";
            string fileName = "GeneralParameter.xml";
            string filePath = fileDirectory + fileName;
            string tempCtrlName;
            string tempValue = "";
            bool needSaveDataCtrlFlag = false;//用于标识当前控件是否用于保存数据

            if (!File.Exists(filePath))
            {
                XMLFileOperation obj = new XMLFileOperation();
                obj.CreateXmlRoot(_xmlFileRootNodeName);//创建声明及根节点
                obj.XmlSave(filePath);
                obj.Dispose();
                //修改根节点的属性
                XMLFileOperation.updateNodeValueOrAttribute(filePath, _xmlFileRootNodeName, "xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");
                XMLFileOperation.updateNodeValueOrAttribute(filePath, _xmlFileRootNodeName, "xmlns:xsd", "http://www.w3.org/2001/XMLSchema");
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
                XmlElement xmlElem = xmlDoc.CreateElement("", _xmlFileRootNodeName, "");
                xmlDoc.AppendChild(xmlElem);

                //修改根节点的属性
                XmlNode node = xmlDoc.SelectSingleNode(_xmlFileRootNodeName);
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

            for (int i = 0; i < allCtrlsArray.Length; i++)
            {
                needSaveDataCtrlFlag = false;
                tempCtrlName = allCtrlsArray[i].Name.ToString();
                try
                {
                    if (_stationNumber == 1)
                    {
                        if (tempCtrlName == "txtROB_X9PTeachPosition_P1_S1" ||
                            tempCtrlName == "txtROB_X9PTeachPosition_P2_S1" ||
                            tempCtrlName == "txtROB_X9PTeachPosition_P3_S1" ||
                            tempCtrlName == "txtROB_X9PTeachPosition_P4_S1" ||
                            tempCtrlName == "txtROB_X9PTeachPosition_P5_S1" ||
                            tempCtrlName == "txtROB_X9PTeachPosition_P6_S1" ||
                            tempCtrlName == "txtROB_X9PTeachPosition_P7_S1" ||
                            tempCtrlName == "txtROB_X9PTeachPosition_P8_S1" ||
                            tempCtrlName == "txtROB_X9PTeachPosition_P9_S1" ||
                            tempCtrlName == "txtROB_Y9PTeachPosition_P1_S1" ||
                            tempCtrlName == "txtROB_Y9PTeachPosition_P2_S1" ||
                            tempCtrlName == "txtROB_Y9PTeachPosition_P3_S1" ||
                            tempCtrlName == "txtROB_Y9PTeachPosition_P4_S1" ||
                            tempCtrlName == "txtROB_Y9PTeachPosition_P5_S1" ||
                            tempCtrlName == "txtROB_Y9PTeachPosition_P6_S1" ||
                            tempCtrlName == "txtROB_Y9PTeachPosition_P7_S1" ||
                            tempCtrlName == "txtROB_Y9PTeachPosition_P8_S1" ||
                            tempCtrlName == "txtROB_Y9PTeachPosition_P9_S1" ||
                            tempCtrlName == "txtROB_Z9PTeachPosition_P1_S1" ||
                            tempCtrlName == "txtROB_Z9PTeachPosition_P2_S1" ||
                            tempCtrlName == "txtROB_Z9PTeachPosition_P3_S1" ||
                            tempCtrlName == "txtROB_Z9PTeachPosition_P4_S1" ||
                            tempCtrlName == "txtROB_Z9PTeachPosition_P5_S1" ||
                            tempCtrlName == "txtROB_Z9PTeachPosition_P6_S1" ||
                            tempCtrlName == "txtROB_Z9PTeachPosition_P7_S1" ||
                            tempCtrlName == "txtROB_Z9PTeachPosition_P8_S1" ||
                            tempCtrlName == "txtROB_Z9PTeachPosition_P9_S1" ||
                            tempCtrlName == "txtROB_RX9PTeachPosition_P1_S1" ||
                            tempCtrlName == "txtROB_RX9PTeachPosition_P2_S1" ||
                            tempCtrlName == "txtROB_RX9PTeachPosition_P3_S1" ||
                            tempCtrlName == "txtROB_RX9PTeachPosition_P4_S1" ||
                            tempCtrlName == "txtROB_RX9PTeachPosition_P5_S1" ||
                            tempCtrlName == "txtROB_RX9PTeachPosition_P6_S1" ||
                            tempCtrlName == "txtROB_RX9PTeachPosition_P7_S1" ||
                            tempCtrlName == "txtROB_RX9PTeachPosition_P8_S1" ||
                            tempCtrlName == "txtROB_RX9PTeachPosition_P9_S1" ||
                            tempCtrlName == "txtROB_RW9PTeachPosition_P1_S1" ||
                            tempCtrlName == "txtROB_RW9PTeachPosition_P2_S1" ||
                            tempCtrlName == "txtROB_RW9PTeachPosition_P3_S1" ||
                            tempCtrlName == "txtROB_RW9PTeachPosition_P4_S1" ||
                            tempCtrlName == "txtROB_RW9PTeachPosition_P5_S1" ||
                            tempCtrlName == "txtROB_RW9PTeachPosition_P6_S1" ||
                            tempCtrlName == "txtROB_RW9PTeachPosition_P7_S1" ||
                            tempCtrlName == "txtROB_RW9PTeachPosition_P8_S1" ||
                            tempCtrlName == "txtROB_RW9PTeachPosition_P9_S1" ||
                            tempCtrlName == "txtROB_RZ9PTeachPosition_P1_S1" ||
                            tempCtrlName == "txtROB_RZ9PTeachPosition_P2_S1" ||
                            tempCtrlName == "txtROB_RZ9PTeachPosition_P3_S1" ||
                            tempCtrlName == "txtROB_RZ9PTeachPosition_P4_S1" ||
                            tempCtrlName == "txtROB_RZ9PTeachPosition_P5_S1" ||
                            tempCtrlName == "txtROB_RZ9PTeachPosition_P6_S1" ||
                            tempCtrlName == "txtROB_RZ9PTeachPosition_P7_S1" ||
                            tempCtrlName == "txtROB_RZ9PTeachPosition_P8_S1" ||
                            tempCtrlName == "txtROB_RZ9PTeachPosition_P9_S1" ||
                            tempCtrlName == "txtPixel_X9PTeachPosition_P1_S1" ||
                            tempCtrlName == "txtPixel_X9PTeachPosition_P2_S1" ||
                            tempCtrlName == "txtPixel_X9PTeachPosition_P3_S1" ||
                            tempCtrlName == "txtPixel_X9PTeachPosition_P4_S1" ||
                            tempCtrlName == "txtPixel_X9PTeachPosition_P5_S1" ||
                            tempCtrlName == "txtPixel_X9PTeachPosition_P6_S1" ||
                            tempCtrlName == "txtPixel_X9PTeachPosition_P7_S1" ||
                            tempCtrlName == "txtPixel_X9PTeachPosition_P8_S1" ||
                            tempCtrlName == "txtPixel_X9PTeachPosition_P9_S1" ||
                            tempCtrlName == "txtPixel_Y9PTeachPosition_P1_S1" ||
                            tempCtrlName == "txtPixel_Y9PTeachPosition_P2_S1" ||
                            tempCtrlName == "txtPixel_Y9PTeachPosition_P3_S1" ||
                            tempCtrlName == "txtPixel_Y9PTeachPosition_P4_S1" ||
                            tempCtrlName == "txtPixel_Y9PTeachPosition_P5_S1" ||
                            tempCtrlName == "txtPixel_Y9PTeachPosition_P6_S1" ||
                            tempCtrlName == "txtPixel_Y9PTeachPosition_P7_S1" ||
                            tempCtrlName == "txtPixel_Y9PTeachPosition_P8_S1" ||
                            tempCtrlName == "txtPixel_Y9PTeachPosition_P9_S1")
                        {
                            tempValue = "";
                            tempValue = allCtrlsArray[i].Text.ToString();

                            needSaveDataCtrlFlag = true;
                        }
                    }

                    if (_stationNumber == 2)
                    {
                        if (tempCtrlName == "txtROB_X9PTeachPosition_P1_S2" ||
                        tempCtrlName == "txtROB_X9PTeachPosition_P2_S2" ||
                        tempCtrlName == "txtROB_X9PTeachPosition_P3_S2" ||
                        tempCtrlName == "txtROB_X9PTeachPosition_P4_S2" ||
                        tempCtrlName == "txtROB_X9PTeachPosition_P5_S2" ||
                        tempCtrlName == "txtROB_X9PTeachPosition_P6_S2" ||
                        tempCtrlName == "txtROB_X9PTeachPosition_P7_S2" ||
                        tempCtrlName == "txtROB_X9PTeachPosition_P8_S2" ||
                        tempCtrlName == "txtROB_X9PTeachPosition_P9_S2" ||
                        tempCtrlName == "txtROB_Y9PTeachPosition_P1_S2" ||
                        tempCtrlName == "txtROB_Y9PTeachPosition_P2_S2" ||
                        tempCtrlName == "txtROB_Y9PTeachPosition_P3_S2" ||
                        tempCtrlName == "txtROB_Y9PTeachPosition_P4_S2" ||
                        tempCtrlName == "txtROB_Y9PTeachPosition_P5_S2" ||
                        tempCtrlName == "txtROB_Y9PTeachPosition_P6_S2" ||
                        tempCtrlName == "txtROB_Y9PTeachPosition_P7_S2" ||
                        tempCtrlName == "txtROB_Y9PTeachPosition_P8_S2" ||
                        tempCtrlName == "txtROB_Y9PTeachPosition_P9_S2" ||
                        tempCtrlName == "txtROB_Z9PTeachPosition_P1_S2" ||
                        tempCtrlName == "txtROB_Z9PTeachPosition_P2_S2" ||
                        tempCtrlName == "txtROB_Z9PTeachPosition_P3_S2" ||
                        tempCtrlName == "txtROB_Z9PTeachPosition_P4_S2" ||
                        tempCtrlName == "txtROB_Z9PTeachPosition_P5_S2" ||
                        tempCtrlName == "txtROB_Z9PTeachPosition_P6_S2" ||
                        tempCtrlName == "txtROB_Z9PTeachPosition_P7_S2" ||
                        tempCtrlName == "txtROB_Z9PTeachPosition_P8_S2" ||
                        tempCtrlName == "txtROB_Z9PTeachPosition_P9_S2" ||
                        tempCtrlName == "txtROB_RX9PTeachPosition_P1_S2" ||
                        tempCtrlName == "txtROB_RX9PTeachPosition_P2_S2" ||
                        tempCtrlName == "txtROB_RX9PTeachPosition_P3_S2" ||
                        tempCtrlName == "txtROB_RX9PTeachPosition_P4_S2" ||
                        tempCtrlName == "txtROB_RX9PTeachPosition_P5_S2" ||
                        tempCtrlName == "txtROB_RX9PTeachPosition_P6_S2" ||
                        tempCtrlName == "txtROB_RX9PTeachPosition_P7_S2" ||
                        tempCtrlName == "txtROB_RX9PTeachPosition_P8_S2" ||
                        tempCtrlName == "txtROB_RX9PTeachPosition_P9_S2" ||
                        tempCtrlName == "txtROB_RW9PTeachPosition_P1_S2" ||
                        tempCtrlName == "txtROB_RW9PTeachPosition_P2_S2" ||
                        tempCtrlName == "txtROB_RW9PTeachPosition_P3_S2" ||
                        tempCtrlName == "txtROB_RW9PTeachPosition_P4_S2" ||
                        tempCtrlName == "txtROB_RW9PTeachPosition_P5_S2" ||
                        tempCtrlName == "txtROB_RW9PTeachPosition_P6_S2" ||
                        tempCtrlName == "txtROB_RW9PTeachPosition_P7_S2" ||
                        tempCtrlName == "txtROB_RW9PTeachPosition_P8_S2" ||
                        tempCtrlName == "txtROB_RW9PTeachPosition_P9_S2" ||
                        tempCtrlName == "txtROB_RZ9PTeachPosition_P1_S2" ||
                        tempCtrlName == "txtROB_RZ9PTeachPosition_P2_S2" ||
                        tempCtrlName == "txtROB_RZ9PTeachPosition_P3_S2" ||
                        tempCtrlName == "txtROB_RZ9PTeachPosition_P4_S2" ||
                        tempCtrlName == "txtROB_RZ9PTeachPosition_P5_S2" ||
                        tempCtrlName == "txtROB_RZ9PTeachPosition_P6_S2" ||
                        tempCtrlName == "txtROB_RZ9PTeachPosition_P7_S2" ||
                        tempCtrlName == "txtROB_RZ9PTeachPosition_P8_S2" ||
                        tempCtrlName == "txtROB_RZ9PTeachPosition_P9_S2" ||
                        tempCtrlName == "txtPixel_X9PTeachPosition_P1_S2" ||
                        tempCtrlName == "txtPixel_X9PTeachPosition_P2_S2" ||
                        tempCtrlName == "txtPixel_X9PTeachPosition_P3_S2" ||
                        tempCtrlName == "txtPixel_X9PTeachPosition_P4_S2" ||
                        tempCtrlName == "txtPixel_X9PTeachPosition_P5_S2" ||
                        tempCtrlName == "txtPixel_X9PTeachPosition_P6_S2" ||
                        tempCtrlName == "txtPixel_X9PTeachPosition_P7_S2" ||
                        tempCtrlName == "txtPixel_X9PTeachPosition_P8_S2" ||
                        tempCtrlName == "txtPixel_X9PTeachPosition_P9_S2" ||
                        tempCtrlName == "txtPixel_Y9PTeachPosition_P1_S2" ||
                        tempCtrlName == "txtPixel_Y9PTeachPosition_P2_S2" ||
                        tempCtrlName == "txtPixel_Y9PTeachPosition_P3_S2" ||
                        tempCtrlName == "txtPixel_Y9PTeachPosition_P4_S2" ||
                        tempCtrlName == "txtPixel_Y9PTeachPosition_P5_S2" ||
                        tempCtrlName == "txtPixel_Y9PTeachPosition_P6_S2" ||
                        tempCtrlName == "txtPixel_Y9PTeachPosition_P7_S2" ||
                        tempCtrlName == "txtPixel_Y9PTeachPosition_P8_S2" ||
                        tempCtrlName == "txtPixel_Y9PTeachPosition_P9_S2")
                        {
                            tempValue = "";
                            tempValue = allCtrlsArray[i].Text.ToString();

                            needSaveDataCtrlFlag = true;
                        }
                    }

                    if (_stationNumber == 3)
                    {
                        if (tempCtrlName == "txtROB_X9PTeachPosition_P1_S3" ||
                        tempCtrlName == "txtROB_X9PTeachPosition_P2_S3" ||
                        tempCtrlName == "txtROB_X9PTeachPosition_P3_S3" ||
                        tempCtrlName == "txtROB_X9PTeachPosition_P4_S3" ||
                        tempCtrlName == "txtROB_X9PTeachPosition_P5_S3" ||
                        tempCtrlName == "txtROB_X9PTeachPosition_P6_S3" ||
                        tempCtrlName == "txtROB_X9PTeachPosition_P7_S3" ||
                        tempCtrlName == "txtROB_X9PTeachPosition_P8_S3" ||
                        tempCtrlName == "txtROB_X9PTeachPosition_P9_S3" ||
                        tempCtrlName == "txtROB_Y9PTeachPosition_P1_S3" ||
                        tempCtrlName == "txtROB_Y9PTeachPosition_P2_S3" ||
                        tempCtrlName == "txtROB_Y9PTeachPosition_P3_S3" ||
                        tempCtrlName == "txtROB_Y9PTeachPosition_P4_S3" ||
                        tempCtrlName == "txtROB_Y9PTeachPosition_P5_S3" ||
                        tempCtrlName == "txtROB_Y9PTeachPosition_P6_S3" ||
                        tempCtrlName == "txtROB_Y9PTeachPosition_P7_S3" ||
                        tempCtrlName == "txtROB_Y9PTeachPosition_P8_S3" ||
                        tempCtrlName == "txtROB_Y9PTeachPosition_P9_S3" ||
                        tempCtrlName == "txtROB_Z9PTeachPosition_P1_S3" ||
                        tempCtrlName == "txtROB_Z9PTeachPosition_P2_S3" ||
                        tempCtrlName == "txtROB_Z9PTeachPosition_P3_S3" ||
                        tempCtrlName == "txtROB_Z9PTeachPosition_P4_S3" ||
                        tempCtrlName == "txtROB_Z9PTeachPosition_P5_S3" ||
                        tempCtrlName == "txtROB_Z9PTeachPosition_P6_S3" ||
                        tempCtrlName == "txtROB_Z9PTeachPosition_P7_S3" ||
                        tempCtrlName == "txtROB_Z9PTeachPosition_P8_S3" ||
                        tempCtrlName == "txtROB_Z9PTeachPosition_P9_S3" ||
                        tempCtrlName == "txtROB_RX9PTeachPosition_P1_S3" ||
                        tempCtrlName == "txtROB_RX9PTeachPosition_P2_S3" ||
                        tempCtrlName == "txtROB_RX9PTeachPosition_P3_S3" ||
                        tempCtrlName == "txtROB_RX9PTeachPosition_P4_S3" ||
                        tempCtrlName == "txtROB_RX9PTeachPosition_P5_S3" ||
                        tempCtrlName == "txtROB_RX9PTeachPosition_P6_S3" ||
                        tempCtrlName == "txtROB_RX9PTeachPosition_P7_S3" ||
                        tempCtrlName == "txtROB_RX9PTeachPosition_P8_S3" ||
                        tempCtrlName == "txtROB_RX9PTeachPosition_P9_S3" ||
                        tempCtrlName == "txtROB_RW9PTeachPosition_P1_S3" ||
                        tempCtrlName == "txtROB_RW9PTeachPosition_P2_S3" ||
                        tempCtrlName == "txtROB_RW9PTeachPosition_P3_S3" ||
                        tempCtrlName == "txtROB_RW9PTeachPosition_P4_S3" ||
                        tempCtrlName == "txtROB_RW9PTeachPosition_P5_S3" ||
                        tempCtrlName == "txtROB_RW9PTeachPosition_P6_S3" ||
                        tempCtrlName == "txtROB_RW9PTeachPosition_P7_S3" ||
                        tempCtrlName == "txtROB_RW9PTeachPosition_P8_S3" ||
                        tempCtrlName == "txtROB_RW9PTeachPosition_P9_S3" ||
                        tempCtrlName == "txtROB_RZ9PTeachPosition_P1_S3" ||
                        tempCtrlName == "txtROB_RZ9PTeachPosition_P2_S3" ||
                        tempCtrlName == "txtROB_RZ9PTeachPosition_P3_S3" ||
                        tempCtrlName == "txtROB_RZ9PTeachPosition_P4_S3" ||
                        tempCtrlName == "txtROB_RZ9PTeachPosition_P5_S3" ||
                        tempCtrlName == "txtROB_RZ9PTeachPosition_P6_S3" ||
                        tempCtrlName == "txtROB_RZ9PTeachPosition_P7_S3" ||
                        tempCtrlName == "txtROB_RZ9PTeachPosition_P8_S3" ||
                        tempCtrlName == "txtROB_RZ9PTeachPosition_P9_S3" ||
                        tempCtrlName == "txtPixel_X9PTeachPosition_P1_S3" ||
                        tempCtrlName == "txtPixel_X9PTeachPosition_P2_S3" ||
                        tempCtrlName == "txtPixel_X9PTeachPosition_P3_S3" ||
                        tempCtrlName == "txtPixel_X9PTeachPosition_P4_S3" ||
                        tempCtrlName == "txtPixel_X9PTeachPosition_P5_S3" ||
                        tempCtrlName == "txtPixel_X9PTeachPosition_P6_S3" ||
                        tempCtrlName == "txtPixel_X9PTeachPosition_P7_S3" ||
                        tempCtrlName == "txtPixel_X9PTeachPosition_P8_S3" ||
                        tempCtrlName == "txtPixel_X9PTeachPosition_P9_S3" ||
                        tempCtrlName == "txtPixel_Y9PTeachPosition_P1_S3" ||
                        tempCtrlName == "txtPixel_Y9PTeachPosition_P2_S3" ||
                        tempCtrlName == "txtPixel_Y9PTeachPosition_P3_S3" ||
                        tempCtrlName == "txtPixel_Y9PTeachPosition_P4_S3" ||
                        tempCtrlName == "txtPixel_Y9PTeachPosition_P5_S3" ||
                        tempCtrlName == "txtPixel_Y9PTeachPosition_P6_S3" ||
                        tempCtrlName == "txtPixel_Y9PTeachPosition_P7_S3" ||
                        tempCtrlName == "txtPixel_Y9PTeachPosition_P8_S3" ||
                        tempCtrlName == "txtPixel_Y9PTeachPosition_P9_S3")
                        {
                            tempValue = "";
                            tempValue = allCtrlsArray[i].Text.ToString();

                            needSaveDataCtrlFlag = true;
                        }
                    }

                    if (needSaveDataCtrlFlag)
                    {
                        XmlNodeList nodes = xmlDoc.DocumentElement.SelectNodes(@"/" + _xmlFileRootNodeName + @"/" + _xmlFileMainNodeName + @"/Name");//获取所有的Name的值
                        bool elementExistFlag = false;//用于标志是否存在此元素
                        foreach (XmlNode nodeChild in nodes)
                        {
                            if (nodeChild.InnerText == tempCtrlName)//如果此元素存在
                            {
                                foreach (XmlNode nodeChild2 in nodeChild.ParentNode)
                                {
                                    if (nodeChild2.Name == "Value")
                                    {
                                        nodeChild2.InnerText = tempValue;
                                        break;
                                    }
                                }
                                elementExistFlag = true;
                                break;
                            }
                        }

                        if (!elementExistFlag)//如果节点不存在
                        {
                            string[] s1 = { "Parent", "Name", "Value", "Tag" };
                            string[] s2 = { _xmlFileParentData, tempCtrlName, tempValue, tempCtrlName.Substring(3, tempCtrlName.Length - 3) };
                            XmlNode objRootNode = xmlDoc.SelectSingleNode(_xmlFileRootNodeName);//或者当前节点
                            XmlElement objChildNode = xmlDoc.CreateElement(_xmlFileMainNodeName);//新建新插入节点
                            objRootNode.AppendChild(objChildNode);//新插入节点Append到当前节点
                            for (int j = 0; j < s1.Length; j++)
                            {
                                XmlElement objElement = xmlDoc.CreateElement(s1[j]);//新建新插入节点的子节点
                                objElement.InnerText = s2[j];

                                objChildNode.AppendChild(objElement);//子节点Append到新插入节点
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                }

            }
            xmlDoc.Save(filePath);
            xmlDoc = null;

            generalParameterRefresh();
        }


        private void generalParameterSavingFromCtrl_Others_S(object sender, EventArgs e)
        {
            if (ActionForm._accessLevel < 101)
            {
                MyTool.TxtFileProcess.CreateLog("操作权限不合格:操作权限为工程师或者厂家!" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("操作权限不合格:操作权限为工程师或者厂家!");
                return;
            }
            MyTool.TxtFileProcess.CreateLog("参数设置-保存数据至文件中！" + "----用户：" + ActionForm._operatorName,
            ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);

            string fileDirectory = @_GeneralParameterDirectoryPath + "\\";
            string fileName = "GeneralParameter.xml";
            string filePath = fileDirectory + fileName;
            string tempCtrlName;
            string tempValue = "";
            bool needSaveDataCtrlFlag = false;//用于标识当前控件是否用于保存数据

            if (!File.Exists(filePath))
            {
                XMLFileOperation obj = new XMLFileOperation();
                obj.CreateXmlRoot(_xmlFileRootNodeName);//创建声明及根节点
                obj.XmlSave(filePath);
                obj.Dispose();
                //修改根节点的属性
                XMLFileOperation.updateNodeValueOrAttribute(filePath, _xmlFileRootNodeName, "xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");
                XMLFileOperation.updateNodeValueOrAttribute(filePath, _xmlFileRootNodeName, "xmlns:xsd", "http://www.w3.org/2001/XMLSchema");
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
                XmlElement xmlElem = xmlDoc.CreateElement("", _xmlFileRootNodeName, "");
                xmlDoc.AppendChild(xmlElem);

                //修改根节点的属性
                XmlNode node = xmlDoc.SelectSingleNode(_xmlFileRootNodeName);
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

            for (int i = 0; i < allCtrlsArray.Length; i++)
            {
                needSaveDataCtrlFlag = false;
                tempCtrlName = allCtrlsArray[i].Name.ToString();
                try
                {
                    if (tempCtrlName == "txtCameraName_S1" || tempCtrlName == "txtCameraName_S2" || tempCtrlName == "txtCameraName_S3")
                    {
                        tempValue = "";
                        tempValue = allCtrlsArray[i].Text.ToString();

                        needSaveDataCtrlFlag = true;
                    }


                    if (needSaveDataCtrlFlag)
                    {
                        XmlNodeList nodes = xmlDoc.DocumentElement.SelectNodes(@"/" + _xmlFileRootNodeName + @"/" + _xmlFileMainNodeName + @"/Name");//获取所有的Name的值
                        bool elementExistFlag = false;//用于标志是否存在此元素
                        foreach (XmlNode nodeChild in nodes)
                        {
                            if (nodeChild.InnerText == tempCtrlName)//如果此元素存在
                            {
                                foreach (XmlNode nodeChild2 in nodeChild.ParentNode)
                                {
                                    if (nodeChild2.Name == "Value")
                                    {
                                        nodeChild2.InnerText = tempValue;
                                        break;
                                    }
                                }
                                elementExistFlag = true;
                                break;
                            }
                        }

                        if (!elementExistFlag)//如果节点不存在
                        {
                            string[] s1 = { "Parent", "Name", "Value", "Tag" };
                            string[] s2 = { _xmlFileParentData, tempCtrlName, tempValue, tempCtrlName.Substring(3, tempCtrlName.Length - 3) };
                            XmlNode objRootNode = xmlDoc.SelectSingleNode(_xmlFileRootNodeName);//或者当前节点
                            XmlElement objChildNode = xmlDoc.CreateElement(_xmlFileMainNodeName);//新建新插入节点
                            objRootNode.AppendChild(objChildNode);//新插入节点Append到当前节点
                            for (int j = 0; j < s1.Length; j++)
                            {
                                XmlElement objElement = xmlDoc.CreateElement(s1[j]);//新建新插入节点的子节点
                                objElement.InnerText = s2[j];

                                objChildNode.AppendChild(objElement);//子节点Append到新插入节点
                            }
                        }
                    }
                }
                catch
                {

                }
            }
            xmlDoc.Save(filePath);
            xmlDoc = null;

            generalParameterRefresh();

        }

        #endregion

        #region 读取
        /// <summary>
        /// ProductDataPickupingToCtrl:读取数据至控件
        /// </summary>
        /// <param name="FileName">string：产品名称</param>
        /// <returns>bool:false-读取失败，true-读取成功</returns>
        public void generalParameterLoadingToCtrl()
        {
            MyTool.TxtFileProcess.CreateLog("参数设置-从文件中读取数据！" + "----用户：" + ActionForm._operatorName,
                       ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);

            string fileDirectory = @_GeneralParameterDirectoryPath + "\\";
            string fileName = "GeneralParameter.xml";
            string filePath = fileDirectory + fileName;
            string tempCtrlName;
            string[] tempValue = { "", "", "", "" };
            bool needLoadDataCtrlFlag = false;//用于标识当前控件是否用于读取数据

            if (!File.Exists(filePath))
            {
                XMLFileOperation obj = new XMLFileOperation();
                obj.CreateXmlRoot(_xmlFileRootNodeName);//创建声明及根节点
                obj.XmlSave(filePath);
                obj.Dispose();
                //修改根节点的属性
                XMLFileOperation.updateNodeValueOrAttribute(filePath, _xmlFileRootNodeName, "xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");
                XMLFileOperation.updateNodeValueOrAttribute(filePath, _xmlFileRootNodeName, "xmlns:xsd", "http://www.w3.org/2001/XMLSchema");

                SavingDataToXmlFile();
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
                XmlElement xmlElem = xmlDoc.CreateElement("", _xmlFileRootNodeName, "");
                xmlDoc.AppendChild(xmlElem);

                //修改根节点的属性
                XmlNode node = xmlDoc.SelectSingleNode(_xmlFileRootNodeName);
                XmlElement xmlEle = (XmlElement)node;
                if ("xmlns:xsi".Equals(""))
                    xmlEle.InnerText = "http://www.w3.org/2001/XMLSchema-instance";
                else
                    xmlEle.SetAttribute("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");

                if ("xmlns:xsd".Equals(""))
                    xmlEle.InnerText = "http://www.w3.org/2001/XMLSchema";
                else
                    xmlEle.SetAttribute("xmlns:xsd", "http://www.w3.org/2001/XMLSchema");
                try
                {
                    xmlDoc.Save(filePath);
                    xmlDoc.Load(filePath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

            for (int i = 0; i < allCtrlsArray.Length; i++)
            {
                needLoadDataCtrlFlag = false;
                tempCtrlName = allCtrlsArray[i].Name.ToString();
                try
                {
                    XmlNodeList nodes = xmlDoc.DocumentElement.SelectNodes(@"/" + _xmlFileRootNodeName + @"/" + _xmlFileMainNodeName + @"/Name");//获取所有的Name的值
                    bool elementExistFlag = false;//用于标志是否存在此元素
                    foreach (XmlNode nodeChild in nodes)
                    {
                        if (nodeChild.InnerText == tempCtrlName)//如果此元素存在
                        {
                            System.Collections.ArrayList al = new System.Collections.ArrayList();
                            foreach (XmlNode nodeChild2 in nodeChild.ParentNode)
                            {
                                al.Add(nodeChild2.InnerText);
                            }
                            for (int j = 0; j < al.Count; j++)
                                tempValue[j] = al[j].ToString();
                            elementExistFlag = true;
                            break;
                        }
                    }

                    if (tempCtrlName == "txtCameraName_S1" ||
                        tempCtrlName == "txtROB_X9PTeachPosition_P1_S1" ||
                        tempCtrlName == "txtROB_X9PTeachPosition_P2_S1" ||
                        tempCtrlName == "txtROB_X9PTeachPosition_P3_S1" ||
                        tempCtrlName == "txtROB_X9PTeachPosition_P4_S1" ||
                        tempCtrlName == "txtROB_X9PTeachPosition_P5_S1" ||
                        tempCtrlName == "txtROB_X9PTeachPosition_P6_S1" ||
                        tempCtrlName == "txtROB_X9PTeachPosition_P7_S1" ||
                        tempCtrlName == "txtROB_X9PTeachPosition_P8_S1" ||
                        tempCtrlName == "txtROB_X9PTeachPosition_P9_S1" ||
                        tempCtrlName == "txtROB_Y9PTeachPosition_P1_S1" ||
                        tempCtrlName == "txtROB_Y9PTeachPosition_P2_S1" ||
                        tempCtrlName == "txtROB_Y9PTeachPosition_P3_S1" ||
                        tempCtrlName == "txtROB_Y9PTeachPosition_P4_S1" ||
                        tempCtrlName == "txtROB_Y9PTeachPosition_P5_S1" ||
                        tempCtrlName == "txtROB_Y9PTeachPosition_P6_S1" ||
                        tempCtrlName == "txtROB_Y9PTeachPosition_P7_S1" ||
                        tempCtrlName == "txtROB_Y9PTeachPosition_P8_S1" ||
                        tempCtrlName == "txtROB_Y9PTeachPosition_P9_S1" ||
                        tempCtrlName == "txtROB_Z9PTeachPosition_P1_S1" ||
                        tempCtrlName == "txtROB_Z9PTeachPosition_P2_S1" ||
                        tempCtrlName == "txtROB_Z9PTeachPosition_P3_S1" ||
                        tempCtrlName == "txtROB_Z9PTeachPosition_P4_S1" ||
                        tempCtrlName == "txtROB_Z9PTeachPosition_P5_S1" ||
                        tempCtrlName == "txtROB_Z9PTeachPosition_P6_S1" ||
                        tempCtrlName == "txtROB_Z9PTeachPosition_P7_S1" ||
                        tempCtrlName == "txtROB_Z9PTeachPosition_P8_S1" ||
                        tempCtrlName == "txtROB_Z9PTeachPosition_P9_S1" ||
                        tempCtrlName == "txtROB_RX9PTeachPosition_P1_S1" ||
                        tempCtrlName == "txtROB_RX9PTeachPosition_P2_S1" ||
                        tempCtrlName == "txtROB_RX9PTeachPosition_P3_S1" ||
                        tempCtrlName == "txtROB_RX9PTeachPosition_P4_S1" ||
                        tempCtrlName == "txtROB_RX9PTeachPosition_P5_S1" ||
                        tempCtrlName == "txtROB_RX9PTeachPosition_P6_S1" ||
                        tempCtrlName == "txtROB_RX9PTeachPosition_P7_S1" ||
                        tempCtrlName == "txtROB_RX9PTeachPosition_P8_S1" ||
                        tempCtrlName == "txtROB_RX9PTeachPosition_P9_S1" ||
                        tempCtrlName == "txtROB_RW9PTeachPosition_P1_S1" ||
                        tempCtrlName == "txtROB_RW9PTeachPosition_P2_S1" ||
                        tempCtrlName == "txtROB_RW9PTeachPosition_P3_S1" ||
                        tempCtrlName == "txtROB_RW9PTeachPosition_P4_S1" ||
                        tempCtrlName == "txtROB_RW9PTeachPosition_P5_S1" ||
                        tempCtrlName == "txtROB_RW9PTeachPosition_P6_S1" ||
                        tempCtrlName == "txtROB_RW9PTeachPosition_P7_S1" ||
                        tempCtrlName == "txtROB_RW9PTeachPosition_P8_S1" ||
                        tempCtrlName == "txtROB_RW9PTeachPosition_P9_S1" ||
                        tempCtrlName == "txtROB_RZ9PTeachPosition_P1_S1" ||
                        tempCtrlName == "txtROB_RZ9PTeachPosition_P2_S1" ||
                        tempCtrlName == "txtROB_RZ9PTeachPosition_P3_S1" ||
                        tempCtrlName == "txtROB_RZ9PTeachPosition_P4_S1" ||
                        tempCtrlName == "txtROB_RZ9PTeachPosition_P5_S1" ||
                        tempCtrlName == "txtROB_RZ9PTeachPosition_P6_S1" ||
                        tempCtrlName == "txtROB_RZ9PTeachPosition_P7_S1" ||
                        tempCtrlName == "txtROB_RZ9PTeachPosition_P8_S1" ||
                        tempCtrlName == "txtROB_RZ9PTeachPosition_P9_S1" ||
                        tempCtrlName == "txtPixel_X9PTeachPosition_P1_S1" ||
                        tempCtrlName == "txtPixel_X9PTeachPosition_P2_S1" ||
                        tempCtrlName == "txtPixel_X9PTeachPosition_P3_S1" ||
                        tempCtrlName == "txtPixel_X9PTeachPosition_P4_S1" ||
                        tempCtrlName == "txtPixel_X9PTeachPosition_P5_S1" ||
                        tempCtrlName == "txtPixel_X9PTeachPosition_P6_S1" ||
                        tempCtrlName == "txtPixel_X9PTeachPosition_P7_S1" ||
                        tempCtrlName == "txtPixel_X9PTeachPosition_P8_S1" ||
                        tempCtrlName == "txtPixel_X9PTeachPosition_P9_S1" ||
                        tempCtrlName == "txtPixel_Y9PTeachPosition_P1_S1" ||
                        tempCtrlName == "txtPixel_Y9PTeachPosition_P2_S1" ||
                        tempCtrlName == "txtPixel_Y9PTeachPosition_P3_S1" ||
                        tempCtrlName == "txtPixel_Y9PTeachPosition_P4_S1" ||
                        tempCtrlName == "txtPixel_Y9PTeachPosition_P5_S1" ||
                        tempCtrlName == "txtPixel_Y9PTeachPosition_P6_S1" ||
                        tempCtrlName == "txtPixel_Y9PTeachPosition_P7_S1" ||
                        tempCtrlName == "txtPixel_Y9PTeachPosition_P8_S1" ||
                        tempCtrlName == "txtPixel_Y9PTeachPosition_P9_S1" ||

                        tempCtrlName == "txtCameraName_S2" ||
                        tempCtrlName == "txtROB_X9PTeachPosition_P1_S2" ||
                        tempCtrlName == "txtROB_X9PTeachPosition_P2_S2" ||
                        tempCtrlName == "txtROB_X9PTeachPosition_P3_S2" ||
                        tempCtrlName == "txtROB_X9PTeachPosition_P4_S2" ||
                        tempCtrlName == "txtROB_X9PTeachPosition_P5_S2" ||
                        tempCtrlName == "txtROB_X9PTeachPosition_P6_S2" ||
                        tempCtrlName == "txtROB_X9PTeachPosition_P7_S2" ||
                        tempCtrlName == "txtROB_X9PTeachPosition_P8_S2" ||
                        tempCtrlName == "txtROB_X9PTeachPosition_P9_S2" ||
                        tempCtrlName == "txtROB_Y9PTeachPosition_P1_S2" ||
                        tempCtrlName == "txtROB_Y9PTeachPosition_P2_S2" ||
                        tempCtrlName == "txtROB_Y9PTeachPosition_P3_S2" ||
                        tempCtrlName == "txtROB_Y9PTeachPosition_P4_S2" ||
                        tempCtrlName == "txtROB_Y9PTeachPosition_P5_S2" ||
                        tempCtrlName == "txtROB_Y9PTeachPosition_P6_S2" ||
                        tempCtrlName == "txtROB_Y9PTeachPosition_P7_S2" ||
                        tempCtrlName == "txtROB_Y9PTeachPosition_P8_S2" ||
                        tempCtrlName == "txtROB_Y9PTeachPosition_P9_S2" ||
                        tempCtrlName == "txtROB_Z9PTeachPosition_P1_S2" ||
                        tempCtrlName == "txtROB_Z9PTeachPosition_P2_S2" ||
                        tempCtrlName == "txtROB_Z9PTeachPosition_P3_S2" ||
                        tempCtrlName == "txtROB_Z9PTeachPosition_P4_S2" ||
                        tempCtrlName == "txtROB_Z9PTeachPosition_P5_S2" ||
                        tempCtrlName == "txtROB_Z9PTeachPosition_P6_S2" ||
                        tempCtrlName == "txtROB_Z9PTeachPosition_P7_S2" ||
                        tempCtrlName == "txtROB_Z9PTeachPosition_P8_S2" ||
                        tempCtrlName == "txtROB_Z9PTeachPosition_P9_S2" ||
                        tempCtrlName == "txtROB_RX9PTeachPosition_P1_S2" ||
                        tempCtrlName == "txtROB_RX9PTeachPosition_P2_S2" ||
                        tempCtrlName == "txtROB_RX9PTeachPosition_P3_S2" ||
                        tempCtrlName == "txtROB_RX9PTeachPosition_P4_S2" ||
                        tempCtrlName == "txtROB_RX9PTeachPosition_P5_S2" ||
                        tempCtrlName == "txtROB_RX9PTeachPosition_P6_S2" ||
                        tempCtrlName == "txtROB_RX9PTeachPosition_P7_S2" ||
                        tempCtrlName == "txtROB_RX9PTeachPosition_P8_S2" ||
                        tempCtrlName == "txtROB_RX9PTeachPosition_P9_S2" ||
                        tempCtrlName == "txtROB_RW9PTeachPosition_P1_S2" ||
                        tempCtrlName == "txtROB_RW9PTeachPosition_P2_S2" ||
                        tempCtrlName == "txtROB_RW9PTeachPosition_P3_S2" ||
                        tempCtrlName == "txtROB_RW9PTeachPosition_P4_S2" ||
                        tempCtrlName == "txtROB_RW9PTeachPosition_P5_S2" ||
                        tempCtrlName == "txtROB_RW9PTeachPosition_P6_S2" ||
                        tempCtrlName == "txtROB_RW9PTeachPosition_P7_S2" ||
                        tempCtrlName == "txtROB_RW9PTeachPosition_P8_S2" ||
                        tempCtrlName == "txtROB_RW9PTeachPosition_P9_S2" ||
                        tempCtrlName == "txtROB_RZ9PTeachPosition_P1_S2" ||
                        tempCtrlName == "txtROB_RZ9PTeachPosition_P2_S2" ||
                        tempCtrlName == "txtROB_RZ9PTeachPosition_P3_S2" ||
                        tempCtrlName == "txtROB_RZ9PTeachPosition_P4_S2" ||
                        tempCtrlName == "txtROB_RZ9PTeachPosition_P5_S2" ||
                        tempCtrlName == "txtROB_RZ9PTeachPosition_P6_S2" ||
                        tempCtrlName == "txtROB_RZ9PTeachPosition_P7_S2" ||
                        tempCtrlName == "txtROB_RZ9PTeachPosition_P8_S2" ||
                        tempCtrlName == "txtROB_RZ9PTeachPosition_P9_S2" ||
                        tempCtrlName == "txtPixel_X9PTeachPosition_P1_S2" ||
                        tempCtrlName == "txtPixel_X9PTeachPosition_P2_S2" ||
                        tempCtrlName == "txtPixel_X9PTeachPosition_P3_S2" ||
                        tempCtrlName == "txtPixel_X9PTeachPosition_P4_S2" ||
                        tempCtrlName == "txtPixel_X9PTeachPosition_P5_S2" ||
                        tempCtrlName == "txtPixel_X9PTeachPosition_P6_S2" ||
                        tempCtrlName == "txtPixel_X9PTeachPosition_P7_S2" ||
                        tempCtrlName == "txtPixel_X9PTeachPosition_P8_S2" ||
                        tempCtrlName == "txtPixel_X9PTeachPosition_P9_S2" ||
                        tempCtrlName == "txtPixel_Y9PTeachPosition_P1_S2" ||
                        tempCtrlName == "txtPixel_Y9PTeachPosition_P2_S2" ||
                        tempCtrlName == "txtPixel_Y9PTeachPosition_P3_S2" ||
                        tempCtrlName == "txtPixel_Y9PTeachPosition_P4_S2" ||
                        tempCtrlName == "txtPixel_Y9PTeachPosition_P5_S2" ||
                        tempCtrlName == "txtPixel_Y9PTeachPosition_P6_S2" ||
                        tempCtrlName == "txtPixel_Y9PTeachPosition_P7_S2" ||
                        tempCtrlName == "txtPixel_Y9PTeachPosition_P8_S2" ||
                        tempCtrlName == "txtPixel_Y9PTeachPosition_P9_S2" ||

                        tempCtrlName == "txtCameraName_S3" ||
                        tempCtrlName == "txtROB_X9PTeachPosition_P1_S3" ||
                        tempCtrlName == "txtROB_X9PTeachPosition_P2_S3" ||
                        tempCtrlName == "txtROB_X9PTeachPosition_P3_S3" ||
                        tempCtrlName == "txtROB_X9PTeachPosition_P4_S3" ||
                        tempCtrlName == "txtROB_X9PTeachPosition_P5_S3" ||
                        tempCtrlName == "txtROB_X9PTeachPosition_P6_S3" ||
                        tempCtrlName == "txtROB_X9PTeachPosition_P7_S3" ||
                        tempCtrlName == "txtROB_X9PTeachPosition_P8_S3" ||
                        tempCtrlName == "txtROB_X9PTeachPosition_P9_S3" ||
                        tempCtrlName == "txtROB_Y9PTeachPosition_P1_S3" ||
                        tempCtrlName == "txtROB_Y9PTeachPosition_P2_S3" ||
                        tempCtrlName == "txtROB_Y9PTeachPosition_P3_S3" ||
                        tempCtrlName == "txtROB_Y9PTeachPosition_P4_S3" ||
                        tempCtrlName == "txtROB_Y9PTeachPosition_P5_S3" ||
                        tempCtrlName == "txtROB_Y9PTeachPosition_P6_S3" ||
                        tempCtrlName == "txtROB_Y9PTeachPosition_P7_S3" ||
                        tempCtrlName == "txtROB_Y9PTeachPosition_P8_S3" ||
                        tempCtrlName == "txtROB_Y9PTeachPosition_P9_S3" ||
                        tempCtrlName == "txtROB_Z9PTeachPosition_P1_S3" ||
                        tempCtrlName == "txtROB_Z9PTeachPosition_P2_S3" ||
                        tempCtrlName == "txtROB_Z9PTeachPosition_P3_S3" ||
                        tempCtrlName == "txtROB_Z9PTeachPosition_P4_S3" ||
                        tempCtrlName == "txtROB_Z9PTeachPosition_P5_S3" ||
                        tempCtrlName == "txtROB_Z9PTeachPosition_P6_S3" ||
                        tempCtrlName == "txtROB_Z9PTeachPosition_P7_S3" ||
                        tempCtrlName == "txtROB_Z9PTeachPosition_P8_S3" ||
                        tempCtrlName == "txtROB_Z9PTeachPosition_P9_S3" ||
                        tempCtrlName == "txtROB_RX9PTeachPosition_P1_S3" ||
                        tempCtrlName == "txtROB_RX9PTeachPosition_P2_S3" ||
                        tempCtrlName == "txtROB_RX9PTeachPosition_P3_S3" ||
                        tempCtrlName == "txtROB_RX9PTeachPosition_P4_S3" ||
                        tempCtrlName == "txtROB_RX9PTeachPosition_P5_S3" ||
                        tempCtrlName == "txtROB_RX9PTeachPosition_P6_S3" ||
                        tempCtrlName == "txtROB_RX9PTeachPosition_P7_S3" ||
                        tempCtrlName == "txtROB_RX9PTeachPosition_P8_S3" ||
                        tempCtrlName == "txtROB_RX9PTeachPosition_P9_S3" ||
                        tempCtrlName == "txtROB_RW9PTeachPosition_P1_S3" ||
                        tempCtrlName == "txtROB_RW9PTeachPosition_P2_S3" ||
                        tempCtrlName == "txtROB_RW9PTeachPosition_P3_S3" ||
                        tempCtrlName == "txtROB_RW9PTeachPosition_P4_S3" ||
                        tempCtrlName == "txtROB_RW9PTeachPosition_P5_S3" ||
                        tempCtrlName == "txtROB_RW9PTeachPosition_P6_S3" ||
                        tempCtrlName == "txtROB_RW9PTeachPosition_P7_S3" ||
                        tempCtrlName == "txtROB_RW9PTeachPosition_P8_S3" ||
                        tempCtrlName == "txtROB_RW9PTeachPosition_P9_S3" ||
                        tempCtrlName == "txtROB_RZ9PTeachPosition_P1_S3" ||
                        tempCtrlName == "txtROB_RZ9PTeachPosition_P2_S3" ||
                        tempCtrlName == "txtROB_RZ9PTeachPosition_P3_S3" ||
                        tempCtrlName == "txtROB_RZ9PTeachPosition_P4_S3" ||
                        tempCtrlName == "txtROB_RZ9PTeachPosition_P5_S3" ||
                        tempCtrlName == "txtROB_RZ9PTeachPosition_P6_S3" ||
                        tempCtrlName == "txtROB_RZ9PTeachPosition_P7_S3" ||
                        tempCtrlName == "txtROB_RZ9PTeachPosition_P8_S3" ||
                        tempCtrlName == "txtROB_RZ9PTeachPosition_P9_S3" ||
                        tempCtrlName == "txtPixel_X9PTeachPosition_P1_S3" ||
                        tempCtrlName == "txtPixel_X9PTeachPosition_P2_S3" ||
                        tempCtrlName == "txtPixel_X9PTeachPosition_P3_S3" ||
                        tempCtrlName == "txtPixel_X9PTeachPosition_P4_S3" ||
                        tempCtrlName == "txtPixel_X9PTeachPosition_P5_S3" ||
                        tempCtrlName == "txtPixel_X9PTeachPosition_P6_S3" ||
                        tempCtrlName == "txtPixel_X9PTeachPosition_P7_S3" ||
                        tempCtrlName == "txtPixel_X9PTeachPosition_P8_S3" ||
                        tempCtrlName == "txtPixel_X9PTeachPosition_P9_S3" ||
                        tempCtrlName == "txtPixel_Y9PTeachPosition_P1_S3" ||
                        tempCtrlName == "txtPixel_Y9PTeachPosition_P2_S3" ||
                        tempCtrlName == "txtPixel_Y9PTeachPosition_P3_S3" ||
                        tempCtrlName == "txtPixel_Y9PTeachPosition_P4_S3" ||
                        tempCtrlName == "txtPixel_Y9PTeachPosition_P5_S3" ||
                        tempCtrlName == "txtPixel_Y9PTeachPosition_P6_S3" ||
                        tempCtrlName == "txtPixel_Y9PTeachPosition_P7_S3" ||
                        tempCtrlName == "txtPixel_Y9PTeachPosition_P8_S3" ||
                        tempCtrlName == "txtPixel_Y9PTeachPosition_P9_S3")
                    {
                        needLoadDataCtrlFlag = true;
                        if (elementExistFlag)//如果这个节点存在
                        {
                            if (_generalParameterLoadingToCtrlFirstTimeFlag)
                                allCtrlsArray[i].Text = tempValue[2];
                            else
                            {
                                allCtrlsArray[i].Invoke(new Action(() =>
                                {
                                    allCtrlsArray[i].Text = tempValue[2];
                                }));
                            }
                        }
                    }

                    if (!elementExistFlag && needLoadDataCtrlFlag)//如果元素不存在，并且这个控件需要进行读取数据
                    {
                        string[] s1 = { "Parent", "Name", "Value", "Tag" };
                        string[] s2 = { _xmlFileParentData, tempCtrlName, tempValue[2], tempCtrlName.Substring(3, tempCtrlName.Length - 3) };
                        XmlNode objRootNode = xmlDoc.SelectSingleNode(_xmlFileRootNodeName);//或者当前节点
                        XmlElement objChildNode = xmlDoc.CreateElement(_xmlFileMainNodeName);//新建新插入节点
                        objRootNode.AppendChild(objChildNode);//新插入节点Append到当前节点
                        for (int j = 0; j < s1.Length; j++)
                        {
                            XmlElement objElement = xmlDoc.CreateElement(s1[j]);//新建新插入节点的子节点
                            objElement.InnerText = s2[j];

                            objChildNode.AppendChild(objElement);//子节点Append到新插入节点
                        }
                    }
                }
                catch (Exception ex)
                {

                }
            }
            try
            {
                xmlDoc.Save(filePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            xmlDoc = null;

            generalParameterRefresh();
            _generalParameterLoadingToCtrlFirstTimeFlag = false;

        }

        #region _generalParameterLoadingToCtrl:读取通用参数到控件的异步函数
        private async void _generalParameterLoadingToCtrl()
        {
            await Task.Run(() =>
            {
                generalParameterLoadingToCtrl();
            });
        }
        #endregion

        #endregion

        #region 把控件的状态更新到变量
        /// <summary>
        /// 保把控件的状态更新到变量
        /// </summary>
        private void generalParameterRefresh()
        {
            MyTool.TxtFileProcess.CreateLog("参数设置-数据更新！" + "----用户：" + ActionForm._operatorName,
            ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);

            //Station1
            try
            {
                ROB_X9PTeachPosition_P1_S1 = Convert.ToSingle(txtROB_X9PTeachPosition_P1_S1.Text);
                ROB_X9PTeachPosition_P2_S1 = Convert.ToSingle(txtROB_X9PTeachPosition_P2_S1.Text);
                ROB_X9PTeachPosition_P3_S1 = Convert.ToSingle(txtROB_X9PTeachPosition_P3_S1.Text);
                ROB_X9PTeachPosition_P4_S1 = Convert.ToSingle(txtROB_X9PTeachPosition_P4_S1.Text);
                ROB_X9PTeachPosition_P5_S1 = Convert.ToSingle(txtROB_X9PTeachPosition_P5_S1.Text);
                ROB_X9PTeachPosition_P6_S1 = Convert.ToSingle(txtROB_X9PTeachPosition_P6_S1.Text);
                ROB_X9PTeachPosition_P7_S1 = Convert.ToSingle(txtROB_X9PTeachPosition_P7_S1.Text);
                ROB_X9PTeachPosition_P8_S1 = Convert.ToSingle(txtROB_X9PTeachPosition_P8_S1.Text);
                ROB_X9PTeachPosition_P9_S1 = Convert.ToSingle(txtROB_X9PTeachPosition_P9_S1.Text);

                ROB_Y9PTeachPosition_P1_S1 = Convert.ToSingle(txtROB_Y9PTeachPosition_P1_S1.Text);
                ROB_Y9PTeachPosition_P2_S1 = Convert.ToSingle(txtROB_Y9PTeachPosition_P2_S1.Text);
                ROB_Y9PTeachPosition_P3_S1 = Convert.ToSingle(txtROB_Y9PTeachPosition_P3_S1.Text);
                ROB_Y9PTeachPosition_P4_S1 = Convert.ToSingle(txtROB_Y9PTeachPosition_P4_S1.Text);
                ROB_Y9PTeachPosition_P5_S1 = Convert.ToSingle(txtROB_Y9PTeachPosition_P5_S1.Text);
                ROB_Y9PTeachPosition_P6_S1 = Convert.ToSingle(txtROB_Y9PTeachPosition_P6_S1.Text);
                ROB_Y9PTeachPosition_P7_S1 = Convert.ToSingle(txtROB_Y9PTeachPosition_P7_S1.Text);
                ROB_Y9PTeachPosition_P8_S1 = Convert.ToSingle(txtROB_Y9PTeachPosition_P8_S1.Text);
                ROB_Y9PTeachPosition_P9_S1 = Convert.ToSingle(txtROB_Y9PTeachPosition_P9_S1.Text);

                ROB_Z9PTeachPosition_P1_S1 = Convert.ToSingle(txtROB_Z9PTeachPosition_P1_S1.Text);
                ROB_Z9PTeachPosition_P2_S1 = Convert.ToSingle(txtROB_Z9PTeachPosition_P2_S1.Text);
                ROB_Z9PTeachPosition_P3_S1 = Convert.ToSingle(txtROB_Z9PTeachPosition_P3_S1.Text);
                ROB_Z9PTeachPosition_P4_S1 = Convert.ToSingle(txtROB_Z9PTeachPosition_P4_S1.Text);
                ROB_Z9PTeachPosition_P5_S1 = Convert.ToSingle(txtROB_Z9PTeachPosition_P5_S1.Text);
                ROB_Z9PTeachPosition_P6_S1 = Convert.ToSingle(txtROB_Z9PTeachPosition_P6_S1.Text);
                ROB_Z9PTeachPosition_P7_S1 = Convert.ToSingle(txtROB_Z9PTeachPosition_P7_S1.Text);
                ROB_Z9PTeachPosition_P8_S1 = Convert.ToSingle(txtROB_Z9PTeachPosition_P8_S1.Text);
                ROB_Z9PTeachPosition_P9_S1 = Convert.ToSingle(txtROB_Z9PTeachPosition_P9_S1.Text);

                ROB_RX9PTeachPosition_P1_S1 = Convert.ToSingle(txtROB_RX9PTeachPosition_P1_S1.Text);
                ROB_RX9PTeachPosition_P2_S1 = Convert.ToSingle(txtROB_RX9PTeachPosition_P2_S1.Text);
                ROB_RX9PTeachPosition_P3_S1 = Convert.ToSingle(txtROB_RX9PTeachPosition_P3_S1.Text);
                ROB_RX9PTeachPosition_P4_S1 = Convert.ToSingle(txtROB_RX9PTeachPosition_P4_S1.Text);
                ROB_RX9PTeachPosition_P5_S1 = Convert.ToSingle(txtROB_RX9PTeachPosition_P5_S1.Text);
                ROB_RX9PTeachPosition_P6_S1 = Convert.ToSingle(txtROB_RX9PTeachPosition_P6_S1.Text);
                ROB_RX9PTeachPosition_P7_S1 = Convert.ToSingle(txtROB_RX9PTeachPosition_P7_S1.Text);
                ROB_RX9PTeachPosition_P8_S1 = Convert.ToSingle(txtROB_RX9PTeachPosition_P8_S1.Text);
                ROB_RX9PTeachPosition_P9_S1 = Convert.ToSingle(txtROB_RX9PTeachPosition_P9_S1.Text);

                ROB_RW9PTeachPosition_P1_S1 = Convert.ToSingle(txtROB_RW9PTeachPosition_P1_S1.Text);
                ROB_RW9PTeachPosition_P2_S1 = Convert.ToSingle(txtROB_RW9PTeachPosition_P2_S1.Text);
                ROB_RW9PTeachPosition_P3_S1 = Convert.ToSingle(txtROB_RW9PTeachPosition_P3_S1.Text);
                ROB_RW9PTeachPosition_P4_S1 = Convert.ToSingle(txtROB_RW9PTeachPosition_P4_S1.Text);
                ROB_RW9PTeachPosition_P5_S1 = Convert.ToSingle(txtROB_RW9PTeachPosition_P5_S1.Text);
                ROB_RW9PTeachPosition_P6_S1 = Convert.ToSingle(txtROB_RW9PTeachPosition_P6_S1.Text);
                ROB_RW9PTeachPosition_P7_S1 = Convert.ToSingle(txtROB_RW9PTeachPosition_P7_S1.Text);
                ROB_RW9PTeachPosition_P8_S1 = Convert.ToSingle(txtROB_RW9PTeachPosition_P8_S1.Text);
                ROB_RW9PTeachPosition_P9_S1 = Convert.ToSingle(txtROB_RW9PTeachPosition_P9_S1.Text);

                ROB_RZ9PTeachPosition_P1_S1 = Convert.ToSingle(txtROB_RZ9PTeachPosition_P1_S1.Text);
                ROB_RZ9PTeachPosition_P2_S1 = Convert.ToSingle(txtROB_RZ9PTeachPosition_P2_S1.Text);
                ROB_RZ9PTeachPosition_P3_S1 = Convert.ToSingle(txtROB_RZ9PTeachPosition_P3_S1.Text);
                ROB_RZ9PTeachPosition_P4_S1 = Convert.ToSingle(txtROB_RZ9PTeachPosition_P4_S1.Text);
                ROB_RZ9PTeachPosition_P5_S1 = Convert.ToSingle(txtROB_RZ9PTeachPosition_P5_S1.Text);
                ROB_RZ9PTeachPosition_P6_S1 = Convert.ToSingle(txtROB_RZ9PTeachPosition_P6_S1.Text);
                ROB_RZ9PTeachPosition_P7_S1 = Convert.ToSingle(txtROB_RZ9PTeachPosition_P7_S1.Text);
                ROB_RZ9PTeachPosition_P8_S1 = Convert.ToSingle(txtROB_RZ9PTeachPosition_P8_S1.Text);
                ROB_RZ9PTeachPosition_P9_S1 = Convert.ToSingle(txtROB_RZ9PTeachPosition_P9_S1.Text);

                Pixel_X9PTeachPosition_P1_S1 = Convert.ToSingle(txtPixel_X9PTeachPosition_P1_S1.Text);
                Pixel_X9PTeachPosition_P2_S1 = Convert.ToSingle(txtPixel_X9PTeachPosition_P2_S1.Text);
                Pixel_X9PTeachPosition_P3_S1 = Convert.ToSingle(txtPixel_X9PTeachPosition_P3_S1.Text);
                Pixel_X9PTeachPosition_P4_S1 = Convert.ToSingle(txtPixel_X9PTeachPosition_P4_S1.Text);
                Pixel_X9PTeachPosition_P5_S1 = Convert.ToSingle(txtPixel_X9PTeachPosition_P5_S1.Text);
                Pixel_X9PTeachPosition_P6_S1 = Convert.ToSingle(txtPixel_X9PTeachPosition_P6_S1.Text);
                Pixel_X9PTeachPosition_P7_S1 = Convert.ToSingle(txtPixel_X9PTeachPosition_P7_S1.Text);
                Pixel_X9PTeachPosition_P8_S1 = Convert.ToSingle(txtPixel_X9PTeachPosition_P8_S1.Text);
                Pixel_X9PTeachPosition_P9_S1 = Convert.ToSingle(txtPixel_X9PTeachPosition_P9_S1.Text);

                Pixel_Y9PTeachPosition_P1_S1 = Convert.ToSingle(txtPixel_Y9PTeachPosition_P1_S1.Text);
                Pixel_Y9PTeachPosition_P2_S1 = Convert.ToSingle(txtPixel_Y9PTeachPosition_P2_S1.Text);
                Pixel_Y9PTeachPosition_P3_S1 = Convert.ToSingle(txtPixel_Y9PTeachPosition_P3_S1.Text);
                Pixel_Y9PTeachPosition_P4_S1 = Convert.ToSingle(txtPixel_Y9PTeachPosition_P4_S1.Text);
                Pixel_Y9PTeachPosition_P5_S1 = Convert.ToSingle(txtPixel_Y9PTeachPosition_P5_S1.Text);
                Pixel_Y9PTeachPosition_P6_S1 = Convert.ToSingle(txtPixel_Y9PTeachPosition_P6_S1.Text);
                Pixel_Y9PTeachPosition_P7_S1 = Convert.ToSingle(txtPixel_Y9PTeachPosition_P7_S1.Text);
                Pixel_Y9PTeachPosition_P8_S1 = Convert.ToSingle(txtPixel_Y9PTeachPosition_P8_S1.Text);
                Pixel_Y9PTeachPosition_P9_S1 = Convert.ToSingle(txtPixel_Y9PTeachPosition_P9_S1.Text);
            }
            catch
            {
                MyTool.TxtFileProcess.CreateLog("参数设置-S1数据更新到变量出错！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("参数设置-S1数据更新到变量出错！");
            }
            try
            {
                _cameraName_S1 = txtCameraName_S1.Text;
                if (_cameraName_S1 == "")
                {
                    MyTool.TxtFileProcess.CreateLog("请设定S1相机出厂名!" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("请设定S1相机出厂名!");
                }
            }
            catch (Exception ex)
            {
                MyTool.TxtFileProcess.CreateLog("读取S1相机出厂名出错!" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取S1相机出厂名出错!");
            }

            //Station2
            try
            {
                ROB_X9PTeachPosition_P1_S2 = Convert.ToSingle(txtROB_X9PTeachPosition_P1_S2.Text);
                ROB_X9PTeachPosition_P2_S2 = Convert.ToSingle(txtROB_X9PTeachPosition_P2_S2.Text);
                ROB_X9PTeachPosition_P3_S2 = Convert.ToSingle(txtROB_X9PTeachPosition_P3_S2.Text);
                ROB_X9PTeachPosition_P4_S2 = Convert.ToSingle(txtROB_X9PTeachPosition_P4_S2.Text);
                ROB_X9PTeachPosition_P5_S2 = Convert.ToSingle(txtROB_X9PTeachPosition_P5_S2.Text);
                ROB_X9PTeachPosition_P6_S2 = Convert.ToSingle(txtROB_X9PTeachPosition_P6_S2.Text);
                ROB_X9PTeachPosition_P7_S2 = Convert.ToSingle(txtROB_X9PTeachPosition_P7_S2.Text);
                ROB_X9PTeachPosition_P8_S2 = Convert.ToSingle(txtROB_X9PTeachPosition_P8_S2.Text);
                ROB_X9PTeachPosition_P9_S2 = Convert.ToSingle(txtROB_X9PTeachPosition_P9_S2.Text);

                ROB_Y9PTeachPosition_P1_S2 = Convert.ToSingle(txtROB_Y9PTeachPosition_P1_S2.Text);
                ROB_Y9PTeachPosition_P2_S2 = Convert.ToSingle(txtROB_Y9PTeachPosition_P2_S2.Text);
                ROB_Y9PTeachPosition_P3_S2 = Convert.ToSingle(txtROB_Y9PTeachPosition_P3_S2.Text);
                ROB_Y9PTeachPosition_P4_S2 = Convert.ToSingle(txtROB_Y9PTeachPosition_P4_S2.Text);
                ROB_Y9PTeachPosition_P5_S2 = Convert.ToSingle(txtROB_Y9PTeachPosition_P5_S2.Text);
                ROB_Y9PTeachPosition_P6_S2 = Convert.ToSingle(txtROB_Y9PTeachPosition_P6_S2.Text);
                ROB_Y9PTeachPosition_P7_S2 = Convert.ToSingle(txtROB_Y9PTeachPosition_P7_S2.Text);
                ROB_Y9PTeachPosition_P8_S2 = Convert.ToSingle(txtROB_Y9PTeachPosition_P8_S2.Text);
                ROB_Y9PTeachPosition_P9_S2 = Convert.ToSingle(txtROB_Y9PTeachPosition_P9_S2.Text);

                ROB_Z9PTeachPosition_P1_S2 = Convert.ToSingle(txtROB_Z9PTeachPosition_P1_S2.Text);
                ROB_Z9PTeachPosition_P2_S2 = Convert.ToSingle(txtROB_Z9PTeachPosition_P2_S2.Text);
                ROB_Z9PTeachPosition_P3_S2 = Convert.ToSingle(txtROB_Z9PTeachPosition_P3_S2.Text);
                ROB_Z9PTeachPosition_P4_S2 = Convert.ToSingle(txtROB_Z9PTeachPosition_P4_S2.Text);
                ROB_Z9PTeachPosition_P5_S2 = Convert.ToSingle(txtROB_Z9PTeachPosition_P5_S2.Text);
                ROB_Z9PTeachPosition_P6_S2 = Convert.ToSingle(txtROB_Z9PTeachPosition_P6_S2.Text);
                ROB_Z9PTeachPosition_P7_S2 = Convert.ToSingle(txtROB_Z9PTeachPosition_P7_S2.Text);
                ROB_Z9PTeachPosition_P8_S2 = Convert.ToSingle(txtROB_Z9PTeachPosition_P8_S2.Text);
                ROB_Z9PTeachPosition_P9_S2 = Convert.ToSingle(txtROB_Z9PTeachPosition_P9_S2.Text);

                ROB_RX9PTeachPosition_P1_S2 = Convert.ToSingle(txtROB_RX9PTeachPosition_P1_S2.Text);
                ROB_RX9PTeachPosition_P2_S2 = Convert.ToSingle(txtROB_RX9PTeachPosition_P2_S2.Text);
                ROB_RX9PTeachPosition_P3_S2 = Convert.ToSingle(txtROB_RX9PTeachPosition_P3_S2.Text);
                ROB_RX9PTeachPosition_P4_S2 = Convert.ToSingle(txtROB_RX9PTeachPosition_P4_S2.Text);
                ROB_RX9PTeachPosition_P5_S2 = Convert.ToSingle(txtROB_RX9PTeachPosition_P5_S2.Text);
                ROB_RX9PTeachPosition_P6_S2 = Convert.ToSingle(txtROB_RX9PTeachPosition_P6_S2.Text);
                ROB_RX9PTeachPosition_P7_S2 = Convert.ToSingle(txtROB_RX9PTeachPosition_P7_S2.Text);
                ROB_RX9PTeachPosition_P8_S2 = Convert.ToSingle(txtROB_RX9PTeachPosition_P8_S2.Text);
                ROB_RX9PTeachPosition_P9_S2 = Convert.ToSingle(txtROB_RX9PTeachPosition_P9_S2.Text);

                ROB_RW9PTeachPosition_P1_S2 = Convert.ToSingle(txtROB_RW9PTeachPosition_P1_S2.Text);
                ROB_RW9PTeachPosition_P2_S2 = Convert.ToSingle(txtROB_RW9PTeachPosition_P2_S2.Text);
                ROB_RW9PTeachPosition_P3_S2 = Convert.ToSingle(txtROB_RW9PTeachPosition_P3_S2.Text);
                ROB_RW9PTeachPosition_P4_S2 = Convert.ToSingle(txtROB_RW9PTeachPosition_P4_S2.Text);
                ROB_RW9PTeachPosition_P5_S2 = Convert.ToSingle(txtROB_RW9PTeachPosition_P5_S2.Text);
                ROB_RW9PTeachPosition_P6_S2 = Convert.ToSingle(txtROB_RW9PTeachPosition_P6_S2.Text);
                ROB_RW9PTeachPosition_P7_S2 = Convert.ToSingle(txtROB_RW9PTeachPosition_P7_S2.Text);
                ROB_RW9PTeachPosition_P8_S2 = Convert.ToSingle(txtROB_RW9PTeachPosition_P8_S2.Text);
                ROB_RW9PTeachPosition_P9_S2 = Convert.ToSingle(txtROB_RW9PTeachPosition_P9_S2.Text);

                ROB_RZ9PTeachPosition_P1_S2 = Convert.ToSingle(txtROB_RZ9PTeachPosition_P1_S2.Text);
                ROB_RZ9PTeachPosition_P2_S2 = Convert.ToSingle(txtROB_RZ9PTeachPosition_P2_S2.Text);
                ROB_RZ9PTeachPosition_P3_S2 = Convert.ToSingle(txtROB_RZ9PTeachPosition_P3_S2.Text);
                ROB_RZ9PTeachPosition_P4_S2 = Convert.ToSingle(txtROB_RZ9PTeachPosition_P4_S2.Text);
                ROB_RZ9PTeachPosition_P5_S2 = Convert.ToSingle(txtROB_RZ9PTeachPosition_P5_S2.Text);
                ROB_RZ9PTeachPosition_P6_S2 = Convert.ToSingle(txtROB_RZ9PTeachPosition_P6_S2.Text);
                ROB_RZ9PTeachPosition_P7_S2 = Convert.ToSingle(txtROB_RZ9PTeachPosition_P7_S2.Text);
                ROB_RZ9PTeachPosition_P8_S2 = Convert.ToSingle(txtROB_RZ9PTeachPosition_P8_S2.Text);
                ROB_RZ9PTeachPosition_P9_S2 = Convert.ToSingle(txtROB_RZ9PTeachPosition_P9_S2.Text);

                Pixel_X9PTeachPosition_P1_S2 = Convert.ToSingle(txtPixel_X9PTeachPosition_P1_S2.Text);
                Pixel_X9PTeachPosition_P2_S2 = Convert.ToSingle(txtPixel_X9PTeachPosition_P2_S2.Text);
                Pixel_X9PTeachPosition_P3_S2 = Convert.ToSingle(txtPixel_X9PTeachPosition_P3_S2.Text);
                Pixel_X9PTeachPosition_P4_S2 = Convert.ToSingle(txtPixel_X9PTeachPosition_P4_S2.Text);
                Pixel_X9PTeachPosition_P5_S2 = Convert.ToSingle(txtPixel_X9PTeachPosition_P5_S2.Text);
                Pixel_X9PTeachPosition_P6_S2 = Convert.ToSingle(txtPixel_X9PTeachPosition_P6_S2.Text);
                Pixel_X9PTeachPosition_P7_S2 = Convert.ToSingle(txtPixel_X9PTeachPosition_P7_S2.Text);
                Pixel_X9PTeachPosition_P8_S2 = Convert.ToSingle(txtPixel_X9PTeachPosition_P8_S2.Text);
                Pixel_X9PTeachPosition_P9_S2 = Convert.ToSingle(txtPixel_X9PTeachPosition_P9_S2.Text);

                Pixel_Y9PTeachPosition_P1_S2 = Convert.ToSingle(txtPixel_Y9PTeachPosition_P1_S2.Text);
                Pixel_Y9PTeachPosition_P2_S2 = Convert.ToSingle(txtPixel_Y9PTeachPosition_P2_S2.Text);
                Pixel_Y9PTeachPosition_P3_S2 = Convert.ToSingle(txtPixel_Y9PTeachPosition_P3_S2.Text);
                Pixel_Y9PTeachPosition_P4_S2 = Convert.ToSingle(txtPixel_Y9PTeachPosition_P4_S2.Text);
                Pixel_Y9PTeachPosition_P5_S2 = Convert.ToSingle(txtPixel_Y9PTeachPosition_P5_S2.Text);
                Pixel_Y9PTeachPosition_P6_S2 = Convert.ToSingle(txtPixel_Y9PTeachPosition_P6_S2.Text);
                Pixel_Y9PTeachPosition_P7_S2 = Convert.ToSingle(txtPixel_Y9PTeachPosition_P7_S2.Text);
                Pixel_Y9PTeachPosition_P8_S2 = Convert.ToSingle(txtPixel_Y9PTeachPosition_P8_S2.Text);
                Pixel_Y9PTeachPosition_P9_S2 = Convert.ToSingle(txtPixel_Y9PTeachPosition_P9_S2.Text);
            }
            catch
            {
                MyTool.TxtFileProcess.CreateLog("参数设置-S2数据更新到变量出错！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("参数设置-S2数据更新到变量出错！");
            }
            try
            {
                _cameraName_S2 = txtCameraName_S2.Text;
                if (_cameraName_S2 == "")
                {
                    MyTool.TxtFileProcess.CreateLog("请设定S2相机出厂名!" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("请设定S2相机出厂名!");
                }
            }
            catch (Exception ex)
            {
                MyTool.TxtFileProcess.CreateLog("读取S2相机出厂名出错!" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取S2相机出厂名出错!");
            }

            //Station3
            try
            {
                ROB_X9PTeachPosition_P1_S3 = Convert.ToSingle(txtROB_X9PTeachPosition_P1_S3.Text);
                ROB_X9PTeachPosition_P2_S3 = Convert.ToSingle(txtROB_X9PTeachPosition_P2_S3.Text);
                ROB_X9PTeachPosition_P3_S3 = Convert.ToSingle(txtROB_X9PTeachPosition_P3_S3.Text);
                ROB_X9PTeachPosition_P4_S3 = Convert.ToSingle(txtROB_X9PTeachPosition_P4_S3.Text);
                ROB_X9PTeachPosition_P5_S3 = Convert.ToSingle(txtROB_X9PTeachPosition_P5_S3.Text);
                ROB_X9PTeachPosition_P6_S3 = Convert.ToSingle(txtROB_X9PTeachPosition_P6_S3.Text);
                ROB_X9PTeachPosition_P7_S3 = Convert.ToSingle(txtROB_X9PTeachPosition_P7_S3.Text);
                ROB_X9PTeachPosition_P8_S3 = Convert.ToSingle(txtROB_X9PTeachPosition_P8_S3.Text);
                ROB_X9PTeachPosition_P9_S3 = Convert.ToSingle(txtROB_X9PTeachPosition_P9_S3.Text);

                ROB_Y9PTeachPosition_P1_S3 = Convert.ToSingle(txtROB_Y9PTeachPosition_P1_S3.Text);
                ROB_Y9PTeachPosition_P2_S3 = Convert.ToSingle(txtROB_Y9PTeachPosition_P2_S3.Text);
                ROB_Y9PTeachPosition_P3_S3 = Convert.ToSingle(txtROB_Y9PTeachPosition_P3_S3.Text);
                ROB_Y9PTeachPosition_P4_S3 = Convert.ToSingle(txtROB_Y9PTeachPosition_P4_S3.Text);
                ROB_Y9PTeachPosition_P5_S3 = Convert.ToSingle(txtROB_Y9PTeachPosition_P5_S3.Text);
                ROB_Y9PTeachPosition_P6_S3 = Convert.ToSingle(txtROB_Y9PTeachPosition_P6_S3.Text);
                ROB_Y9PTeachPosition_P7_S3 = Convert.ToSingle(txtROB_Y9PTeachPosition_P7_S3.Text);
                ROB_Y9PTeachPosition_P8_S3 = Convert.ToSingle(txtROB_Y9PTeachPosition_P8_S3.Text);
                ROB_Y9PTeachPosition_P9_S3 = Convert.ToSingle(txtROB_Y9PTeachPosition_P9_S3.Text);

                ROB_Z9PTeachPosition_P1_S3 = Convert.ToSingle(txtROB_Z9PTeachPosition_P1_S3.Text);
                ROB_Z9PTeachPosition_P2_S3 = Convert.ToSingle(txtROB_Z9PTeachPosition_P2_S3.Text);
                ROB_Z9PTeachPosition_P3_S3 = Convert.ToSingle(txtROB_Z9PTeachPosition_P3_S3.Text);
                ROB_Z9PTeachPosition_P4_S3 = Convert.ToSingle(txtROB_Z9PTeachPosition_P4_S3.Text);
                ROB_Z9PTeachPosition_P5_S3 = Convert.ToSingle(txtROB_Z9PTeachPosition_P5_S3.Text);
                ROB_Z9PTeachPosition_P6_S3 = Convert.ToSingle(txtROB_Z9PTeachPosition_P6_S3.Text);
                ROB_Z9PTeachPosition_P7_S3 = Convert.ToSingle(txtROB_Z9PTeachPosition_P7_S3.Text);
                ROB_Z9PTeachPosition_P8_S3 = Convert.ToSingle(txtROB_Z9PTeachPosition_P8_S3.Text);
                ROB_Z9PTeachPosition_P9_S3 = Convert.ToSingle(txtROB_Z9PTeachPosition_P9_S3.Text);

                ROB_RX9PTeachPosition_P1_S3 = Convert.ToSingle(txtROB_RX9PTeachPosition_P1_S3.Text);
                ROB_RX9PTeachPosition_P2_S3 = Convert.ToSingle(txtROB_RX9PTeachPosition_P2_S3.Text);
                ROB_RX9PTeachPosition_P3_S3 = Convert.ToSingle(txtROB_RX9PTeachPosition_P3_S3.Text);
                ROB_RX9PTeachPosition_P4_S3 = Convert.ToSingle(txtROB_RX9PTeachPosition_P4_S3.Text);
                ROB_RX9PTeachPosition_P5_S3 = Convert.ToSingle(txtROB_RX9PTeachPosition_P5_S3.Text);
                ROB_RX9PTeachPosition_P6_S3 = Convert.ToSingle(txtROB_RX9PTeachPosition_P6_S3.Text);
                ROB_RX9PTeachPosition_P7_S3 = Convert.ToSingle(txtROB_RX9PTeachPosition_P7_S3.Text);
                ROB_RX9PTeachPosition_P8_S3 = Convert.ToSingle(txtROB_RX9PTeachPosition_P8_S3.Text);
                ROB_RX9PTeachPosition_P9_S3 = Convert.ToSingle(txtROB_RX9PTeachPosition_P9_S3.Text);

                ROB_RW9PTeachPosition_P1_S3 = Convert.ToSingle(txtROB_RW9PTeachPosition_P1_S3.Text);
                ROB_RW9PTeachPosition_P2_S3 = Convert.ToSingle(txtROB_RW9PTeachPosition_P2_S3.Text);
                ROB_RW9PTeachPosition_P3_S3 = Convert.ToSingle(txtROB_RW9PTeachPosition_P3_S3.Text);
                ROB_RW9PTeachPosition_P4_S3 = Convert.ToSingle(txtROB_RW9PTeachPosition_P4_S3.Text);
                ROB_RW9PTeachPosition_P5_S3 = Convert.ToSingle(txtROB_RW9PTeachPosition_P5_S3.Text);
                ROB_RW9PTeachPosition_P6_S3 = Convert.ToSingle(txtROB_RW9PTeachPosition_P6_S3.Text);
                ROB_RW9PTeachPosition_P7_S3 = Convert.ToSingle(txtROB_RW9PTeachPosition_P7_S3.Text);
                ROB_RW9PTeachPosition_P8_S3 = Convert.ToSingle(txtROB_RW9PTeachPosition_P8_S3.Text);
                ROB_RW9PTeachPosition_P9_S3 = Convert.ToSingle(txtROB_RW9PTeachPosition_P9_S3.Text);

                ROB_RZ9PTeachPosition_P1_S3 = Convert.ToSingle(txtROB_RZ9PTeachPosition_P1_S3.Text);
                ROB_RZ9PTeachPosition_P2_S3 = Convert.ToSingle(txtROB_RZ9PTeachPosition_P2_S3.Text);
                ROB_RZ9PTeachPosition_P3_S3 = Convert.ToSingle(txtROB_RZ9PTeachPosition_P3_S3.Text);
                ROB_RZ9PTeachPosition_P4_S3 = Convert.ToSingle(txtROB_RZ9PTeachPosition_P4_S3.Text);
                ROB_RZ9PTeachPosition_P5_S3 = Convert.ToSingle(txtROB_RZ9PTeachPosition_P5_S3.Text);
                ROB_RZ9PTeachPosition_P6_S3 = Convert.ToSingle(txtROB_RZ9PTeachPosition_P6_S3.Text);
                ROB_RZ9PTeachPosition_P7_S3 = Convert.ToSingle(txtROB_RZ9PTeachPosition_P7_S3.Text);
                ROB_RZ9PTeachPosition_P8_S3 = Convert.ToSingle(txtROB_RZ9PTeachPosition_P8_S3.Text);
                ROB_RZ9PTeachPosition_P9_S3 = Convert.ToSingle(txtROB_RZ9PTeachPosition_P9_S3.Text);

                Pixel_X9PTeachPosition_P1_S3 = Convert.ToSingle(txtPixel_X9PTeachPosition_P1_S3.Text);
                Pixel_X9PTeachPosition_P2_S3 = Convert.ToSingle(txtPixel_X9PTeachPosition_P2_S3.Text);
                Pixel_X9PTeachPosition_P3_S3 = Convert.ToSingle(txtPixel_X9PTeachPosition_P3_S3.Text);
                Pixel_X9PTeachPosition_P4_S3 = Convert.ToSingle(txtPixel_X9PTeachPosition_P4_S3.Text);
                Pixel_X9PTeachPosition_P5_S3 = Convert.ToSingle(txtPixel_X9PTeachPosition_P5_S3.Text);
                Pixel_X9PTeachPosition_P6_S3 = Convert.ToSingle(txtPixel_X9PTeachPosition_P6_S3.Text);
                Pixel_X9PTeachPosition_P7_S3 = Convert.ToSingle(txtPixel_X9PTeachPosition_P7_S3.Text);
                Pixel_X9PTeachPosition_P8_S3 = Convert.ToSingle(txtPixel_X9PTeachPosition_P8_S3.Text);
                Pixel_X9PTeachPosition_P9_S3 = Convert.ToSingle(txtPixel_X9PTeachPosition_P9_S3.Text);

                Pixel_Y9PTeachPosition_P1_S3 = Convert.ToSingle(txtPixel_Y9PTeachPosition_P1_S3.Text);
                Pixel_Y9PTeachPosition_P2_S3 = Convert.ToSingle(txtPixel_Y9PTeachPosition_P2_S3.Text);
                Pixel_Y9PTeachPosition_P3_S3 = Convert.ToSingle(txtPixel_Y9PTeachPosition_P3_S3.Text);
                Pixel_Y9PTeachPosition_P4_S3 = Convert.ToSingle(txtPixel_Y9PTeachPosition_P4_S3.Text);
                Pixel_Y9PTeachPosition_P5_S3 = Convert.ToSingle(txtPixel_Y9PTeachPosition_P5_S3.Text);
                Pixel_Y9PTeachPosition_P6_S3 = Convert.ToSingle(txtPixel_Y9PTeachPosition_P6_S3.Text);
                Pixel_Y9PTeachPosition_P7_S3 = Convert.ToSingle(txtPixel_Y9PTeachPosition_P7_S3.Text);
                Pixel_Y9PTeachPosition_P8_S3 = Convert.ToSingle(txtPixel_Y9PTeachPosition_P8_S3.Text);
                Pixel_Y9PTeachPosition_P9_S3 = Convert.ToSingle(txtPixel_Y9PTeachPosition_P9_S3.Text);
            }
            catch
            {
                MyTool.TxtFileProcess.CreateLog("参数设置-S3数据更新到变量出错！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("参数设置-S3数据更新到变量出错！");
            }
            try
            {
                _cameraName_S3 = txtCameraName_S3.Text;
                if (_cameraName_S3 == "")
                {
                    MyTool.TxtFileProcess.CreateLog("请设定S3相机出厂名!" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("请设定S3相机出厂名!");
                }
            }
            catch (Exception ex)
            {
                MyTool.TxtFileProcess.CreateLog("读取S3相机出厂名出错!" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("读取S3相机出厂名出错!");
            }

        }

        #endregion

        #endregion

        //需要增加更新到变量_currentProductRecipe
        #region 产品参数的保存与读取----控件

        const int LOAD_SAVE_DATA_ELEMENT_QUANTITY = 52 * 3 + 10;//产品参数个数
        string[,] _ctrlNameAndXmlFileNameStr = new string[2, LOAD_SAVE_DATA_ELEMENT_QUANTITY];//用于存储需要保存读取的参数变量名和xml名称对应数组,0-控件名称，1-xml文件名称
        CreateNewProductDataFileForm _createNewProductFormVariate = null;//创建新产品form变量

        #region 保存
        /// <summary>
        /// ProductDataSavingSettingData：保存设置参数文件
        /// </summary>
        /// <param name="FileName">string：保存的文件的名称</param>
        /// <param name="DataCheckFlag">bool：是否进行数据检查，在创建新的产品数据的时候不需要进行数据检查</param>
        /// <returns>bool: 反馈保存结果，true-保存成功，false-保存失败</returns>
        private bool ProductDataSavingFromCtrl(string FileName, bool DataCheckFlag)
        {
            MyTool.TxtFileProcess.CreateLog("产品设置页面-从控件中读取数据保存至文件中！" + "----用户：" + ActionForm._operatorName,
            ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);

            string rootName = "clsRecipeData";
            string filePath = _ProductParameterDirectoryPath + "\\" + MyTool.FolderAndFileManageClass.FilterPathIllegalChar(FileName) + ".rcp";

            if (Directory.Exists(_ProductParameterDirectoryPath))
            {
                if (DataCheckFlag)
                {
                    if (!DataCheckFunc())//如果数据不合法，不允许保存
                    {
                        MyTool.TxtFileProcess.CreateLog("产品设置页面-从控件中读取数据保存至文件中失败，有非法数据存在！" + "----用户：" + ActionForm._operatorName,
                        ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                        MessageBox.Show("部分当前产品的测试用数据存在不合法，无法保存参数！请检查并修改非法参数！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                }
                if (!File.Exists(filePath))
                {
                    XMLFileOperation obj = new XMLFileOperation();
                    obj.CreateXmlRoot(rootName);//创建声明及根节点
                    obj.XmlSave(filePath);
                    obj.Dispose();
                    //修改根节点的属性
                    XMLFileOperation.updateNodeValueOrAttribute(filePath, rootName, "xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");
                    XMLFileOperation.updateNodeValueOrAttribute(filePath, rootName, "xmlns:xsd", "http://www.w3.org/2001/XMLSchema");
                }
                string ctrlName;//用以存储控件名
                string ctrlNameSuffix;//用以存储控件后缀名
                string tempValue = null;//用于存储要保存的数据值

                XmlDocument xmlDoc = new XmlDocument();
                try
                {
                    xmlDoc.Load(filePath);
                }
                catch
                {
                    XMLFileOperation obj = new XMLFileOperation();
                    obj.CreateXmlRoot(rootName);//创建声明及根节点
                    obj.XmlSave(filePath);
                    obj.Dispose();
                    //修改根节点的属性
                    XMLFileOperation.updateNodeValueOrAttribute(filePath, rootName, "xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");
                    XMLFileOperation.updateNodeValueOrAttribute(filePath, rootName, "xmlns:xsd", "http://www.w3.org/2001/XMLSchema");
                    xmlDoc = new XmlDocument();
                    xmlDoc.Load(filePath);
                }

                for (int i = 0; i < LOAD_SAVE_DATA_ELEMENT_QUANTITY; i++)
                {
                    ctrlName = _ctrlNameAndXmlFileNameStr[0, i];
                    ctrlNameSuffix = ctrlName.Substring(0, 3);
                    //如果检测产品为swtich，那么有关检测项目种类的部分参数就可以不用保存
                    if (ctrlNameSuffix == "txt")
                    {
                        //tempValue = ((TextBox)allCtrlsArray.Find(ctrlName, true)[0]).Text;
                        foreach (Control ctr in allCtrlsArray)
                        {
                            if (ctr.Name == ctrlName)
                            {
                                tempValue = ((TextBox)ctr).Text;
                                break;
                            }
                        }
                    }
                    else if (ctrlNameSuffix == "chk")
                    {
                        //if (((CheckBox)allCtrlsArray.Find(ctrlName, true)[0]).Checked) //tempCtrl
                        //{
                        //    tempValue = "true";
                        //}
                        //else
                        //{
                        //    tempValue = "false";
                        //}
                        foreach (Control ctr in allCtrlsArray)
                        {
                            if (ctr.Name == ctrlName)
                            {
                                if (((CheckBox)ctr).Checked)
                                    tempValue = "true";
                                else
                                    tempValue = "false";
                            }
                        }
                    }
                    else
                    {
                        continue;
                    }

                    try
                    {
                        xmlDoc.SelectSingleNode(rootName + "/" + _ctrlNameAndXmlFileNameStr[1, i]).InnerText = tempValue;
                    }
                    catch
                    {
                        XmlNode fnode = xmlDoc.SelectSingleNode(rootName);
                        XmlElement newNode = xmlDoc.CreateElement(_ctrlNameAndXmlFileNameStr[1, i]);
                        newNode.InnerText = tempValue;
                        fnode.AppendChild(newNode);
                    }
                }
                xmlDoc.Save(filePath);
                xmlDoc = null;

                //保存点位表格
                //if (!SavePointPositionData(_ProductParameterDirectoryPath, FileName))
                //{
                //    return false;
                //}
                return true;
            }
            else
            {
                MyTool.TxtFileProcess.CreateLog("参数设置-从控件中读取数据保存至文件中失败，指定的文件保存路径不存在！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("指定的读取保存产品参数的路径不存在，无法保存数据!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

        }
        #endregion

        #region 读取
        /// <summary>
        /// ProductDataLoadingToCtrl:读取数据至控件
        /// </summary>
        /// <param name="FileName">string：产品名称</param>
        /// <returns>bool:false-读取失败，true-读取成功</returns>
        public bool ProductDataLoadingToCtrl(string FileName)
        {
            MyTool.TxtFileProcess.CreateLog("产品设置页面-从文件读取参数至控件！" + "----用户：" + ActionForm._operatorName,
            ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);

            string filePath = _ProductParameterDirectoryPath + "\\" + MyTool.FolderAndFileManageClass.FilterPathIllegalChar(FileName) + ".rcp";
            string tempValue;
            bool result = true;
            string rootName = "clsRecipeData";

            if (Directory.Exists(_ProductParameterDirectoryPath))
            {
                if (!File.Exists(filePath))
                {
                    //ProductDataSavingSettingData(_actualsProductName);
                    MyTool.TxtFileProcess.CreateLog("产品设置页面-产品参数文件不存在，从文件读取参数至控件失败！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("产品参数文件不存在，读取产品数据至控件失败!");
                    result = false;
                }
                else
                {
                    string ctrlNameSuffix;//用以存储控件后缀名
                    string ctrlName;//用以存储控件名
                    string[] tempNodeName = new string[LOAD_SAVE_DATA_ELEMENT_QUANTITY];
                    int PickupUsefulNodeQuantity = 0;
                    bool[] dataPickupResultFlag = new bool[LOAD_SAVE_DATA_ELEMENT_QUANTITY];//用于判定是否读取数据OK，false-读取失败，true-读取成功

                    for (int i = 0; i < LOAD_SAVE_DATA_ELEMENT_QUANTITY; i++)
                    {
                        tempNodeName[i] = _ctrlNameAndXmlFileNameStr[1, i];
                        dataPickupResultFlag[i] = false;
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
                            ctrlName = _ctrlNameAndXmlFileNameStr[0, index];
                            ctrlNameSuffix = ctrlName.Substring(0, 3);
                            if (ctrlNameSuffix == "txt")
                            {
                                //((TextBox)allCtrlsArray.Find(ctrlName, true)[0]).Text = element.InnerText;
                                foreach (Control ctr in allCtrlsArray)
                                {
                                    if (ctr.Name == ctrlName)
                                    {
                                        ((TextBox)ctr).Text = element.InnerText;
                                        break;
                                    }
                                }
                            }
                            else if (ctrlNameSuffix == "chk")
                            {
                                tempValue = element.InnerText.ToString().ToLower();
                                if (tempValue == "true")
                                {
                                    //((CheckBox)allCtrlsArray.Find(ctrlName, true)[0]).Checked = true;
                                    foreach (Control ctr in allCtrlsArray)
                                    {
                                        if (ctr.Name == ctrlName)
                                        {
                                            ((CheckBox)ctr).Checked = true;
                                        }
                                    }
                                }
                                else
                                {
                                    //((CheckBox)allCtrlsArray.Find(ctrlName, true)[0]).Checked = false;
                                    foreach (Control ctr in allCtrlsArray)
                                    {
                                        if (ctr.Name == ctrlName)
                                        {
                                            ((CheckBox)ctr).Checked = false;
                                        }
                                    }

                                }
                            }
                            else
                            {
                                continue;
                            }
                            PickupUsefulNodeQuantity++;
                            dataPickupResultFlag[index] = true;
                        }
                    }

                    //xmlDoc.Save(filePath);
                    xmlDoc = null;

                    if (PickupUsefulNodeQuantity < LOAD_SAVE_DATA_ELEMENT_QUANTITY)
                    {
                        for (int i = 0; i < LOAD_SAVE_DATA_ELEMENT_QUANTITY; i++)
                        {
                            if (!dataPickupResultFlag[i])
                            {
                                MyTool.TxtFileProcess.CreateLog("产品设置页面-从文件读取参数至控件失败，在产品参数文件" + FileName + ".rcp中，无法找到参数" + _ctrlNameAndXmlFileNameStr[1, i] + "的数据" + "----用户：" + ActionForm._operatorName,
                                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                                MessageBox.Show("在产品参数文件" + FileName + ".rcp中，无法找到参数" + _ctrlNameAndXmlFileNameStr[1, i] + "的数据，读取产品参数至控件中失败！");
                                result = false;
                            }
                        }
                    }
                }
            }
            else
            {
                MyTool.TxtFileProcess.CreateLog("产品设置页面-产品参数文件夹路径不存在，从文件读取参数至控件失败！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("指定的读取保存产品参数的路径不存在，无法读取数据至控件!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                result = false;
            }

            //读取点位表格
            //if (!PickupPointPositionData(ActionForm._ProductParameterDirectoryPath, FileName))
            //{
            //    result = false;
            //}

            if (!DataCheckFunc())
            {
                result = false;
            }
            return result;
        }
        #endregion

        #region DataCheckFunc:保存参数之前,读取参数之后,判定数据是否符合要求
        //保存参数之前,读取参数之后,判定数据是否符合要求
        //保存参数之前如果判定数据不符合要求则不保存
        //读取参数之后如果判定数据不符合要求如何处理?
        private bool DataCheckFunc()
        {
            MyTool.TxtFileProcess.CreateLog("产品参数-数据检测！" + "----用户：" + ActionForm._operatorName,
            ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);

            bool dataCheckResult = true;//false:Check failed,true:check successful

            string tempValue = "";
            int k = 0;
            try
            {
                for (int i = 0; i < LOAD_SAVE_DATA_ELEMENT_QUANTITY; i++)
                {
                    k = i;
                    //string tempValue = ((TextBox)allCtrlsArray.Find(_ctrlNameAndXmlFileNameStr[0, i], true)[0]).Text;
                    foreach (Control ctr in allCtrlsArray)
                    {
                        if (ctr.Name == _ctrlNameAndXmlFileNameStr[0, i] && ctr.Name.Substring(0, 3) == "txt")
                        {
                            tempValue = ((TextBox)ctr).Text;
                            break;
                        }
                    }
                    Convert.ToSingle(tempValue);
                }
            }
            catch (Exception ex)
            {
                MyTool.TxtFileProcess.CreateLog("产品参数-数据检测失败:" + _ctrlNameAndXmlFileNameStr[0, k] + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                dataCheckResult = false;
            }

            if (dataCheckResult)//产品数据格式检查正确则进行数据逻辑检查
            {
                if (!productDatataLogicCheckFunc())
                    dataCheckResult = false;
            }

            if (dataCheckResult)
                MyTool.TxtFileProcess.CreateLog("产品参数-数据检测成功" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);

            return dataCheckResult;
        }
        #endregion

        #region productDatataLogicCheckFunc:产品数据逻辑检查
        private bool productDatataLogicCheckFunc()
        {
            bool dataCheckResult = true;

            //Station1
            if (Convert.ToSingle(txtROB_XPickupTeachPosition_S1.Text) < -2000 || Convert.ToSingle(txtROB_XPickupTeachPosition_S1.Text) > 2000)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtROB_XPickupTeachPosition_S1的数据范围错误,取值范围是[-2000,2000]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtROB_XPickupTeachPosition_S1的数据范围错误,取值范围是[-2000, 2000]！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtROB_YPickupTeachPosition_S1.Text) < -2000 || Convert.ToSingle(txtROB_YPickupTeachPosition_S1.Text) > 2000)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtROB_YPickupTeachPosition_S1的数据范围错误,取值范围是[-2000,2000]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtROB_YPickupTeachPosition_S1的数据范围错误,取值范围是[-2000,2000]！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtROB_ZPickupTeachPosition_S1.Text) < -1000 || Convert.ToSingle(txtROB_ZPickupTeachPosition_S1.Text) > 1000)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtROB_ZPickupTeachPosition_S1的数据范围错误,取值范围是[-1000,1000]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtROB_ZPickupTeachPosition_S1的数据范围错误,取值范围是[-1000,1000]！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtROB_RXPickupTeachPosition_S1.Text) < -360 || Convert.ToSingle(txtROB_RXPickupTeachPosition_S1.Text) > 360)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtROB_RXPickupTeachPosition_S1的数据范围错误,取值范围是[-360,360]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtROB_RXPickupTeachPosition_S1的数据范围错误,取值范围是[-360,360]！！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtROB_RWPickupTeachPosition_S1.Text) < -360 || Convert.ToSingle(txtROB_RWPickupTeachPosition_S1.Text) > 360)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtROB_RWPickupTeachPosition_S1的数据范围错误,取值范围是[-360,360]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtROB_RWPickupTeachPosition_S1的数据范围错误,取值范围是[-360,360]！！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtROB_RZPickupTeachPosition_S1.Text) < -360 || Convert.ToSingle(txtROB_RZPickupTeachPosition_S1.Text) > 360)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtROB_RZPickupTeachPosition_S1的数据范围错误,取值范围是[-360,360]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtROB_RZPickupTeachPosition_S1的数据范围错误,取值范围是[-360,360]！！");
                dataCheckResult = false;
            }

            if (Convert.ToSingle(txtPixel_XPickupTeachPosition_RT_S1.Text) < 0 || Convert.ToSingle(txtPixel_XPickupTeachPosition_RT_S1.Text) > 5000)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtPixel_XPickupTeachPosition_RT_S1的数据范围错误,取值范围是[0,5000]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtPixel_XPickupTeachPosition_RT_S1的数据范围错误,取值范围是[0,5000]！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtPixel_YPickupTeachPosition_RT_S1.Text) < 0 || Convert.ToSingle(txtPixel_YPickupTeachPosition_RT_S1.Text) > 5000)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtPixel_YPickupTeachPosition_RT_S1的数据范围错误,取值范围是[0,5000]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtPixel_YPickupTeachPosition_RT_S1的数据范围错误,取值范围是[0,5000]！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtPixel_XPickupTeachPosition_LT_S1.Text) < 0 || Convert.ToSingle(txtPixel_XPickupTeachPosition_LT_S1.Text) > 5000)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtPixel_XPickupTeachPosition_LT_S1的数据范围错误,取值范围是[0,5000]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtPixel_XPickupTeachPosition_LT_S1的数据范围错误,取值范围是[0,5000]！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtPixel_YPickupTeachPosition_LT_S1.Text) < 0 || Convert.ToSingle(txtPixel_YPickupTeachPosition_LT_S1.Text) > 5000)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtPixel_YPickupTeachPosition_LT_S1的数据范围错误,取值范围是[0,5000]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtPixel_YPickupTeachPosition_LT_S1的数据范围错误,取值范围是[0,5000]！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtPixel_XPickupTeachPosition_LB_S1.Text) < 0 || Convert.ToSingle(txtPixel_XPickupTeachPosition_LB_S1.Text) > 5000)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtPixel_XPickupTeachPosition_LB_S1的数据范围错误,取值范围是[0,5000]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtPixel_XPickupTeachPosition_LB_S1的数据范围错误,取值范围是[0,5000]！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtPixel_YPickupTeachPosition_LB_S1.Text) < 0 || Convert.ToSingle(txtPixel_YPickupTeachPosition_LB_S1.Text) > 5000)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtPixel_YPickupTeachPosition_LB_S1的数据范围错误,取值范围是[0,5000]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtPixel_YPickupTeachPosition_LB_S1的数据范围错误,取值范围是[0,5000]！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtPixel_XPickupTeachPosition_RB_S1.Text) < 0 || Convert.ToSingle(txtPixel_XPickupTeachPosition_RB_S1.Text) > 5000)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtPixel_XPickupTeachPosition_RB_S1的数据范围错误,取值范围是[0,5000]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtPixel_XPickupTeachPosition_RB_S1的数据范围错误,取值范围是[0,5000]！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtPixel_YPickupTeachPosition_RB_S1.Text) < 0 || Convert.ToSingle(txtPixel_YPickupTeachPosition_RB_S1.Text) > 5000)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtPixel_YPickupTeachPosition_RB_S1的数据范围错误,取值范围是[0,5000]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtPixel_YPickupTeachPosition_RB_S1的数据范围错误,取值范围是[0,5000]！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtPixel_AnglePickupTeach_S1.Text) < -360 || Convert.ToSingle(txtPixel_AnglePickupTeach_S1.Text) > 360)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtPixel_AnglePickupTeach_S1的数据范围错误,取值范围是[-360,360]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtPixel_AnglePickupTeach_S1的数据范围错误,取值范围是[-360,360]！");
                dataCheckResult = false;
            }

            if (Convert.ToSingle(txtPixel_XPickupCurrentPositionLimitMax_RT_S1.Text) < Convert.ToSingle(txtPixel_XPickupCurrentPositionLimitMin_RT_S1.Text))
            {
                MyTool.TxtFileProcess.CreateLog("控件txtPixel_XPickupCurrentPositionLimitMax_RT_S1的数据不能小于控件txtPixel_XPickupCurrentPositionLimitMin_RT_S1的数据！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtPixel_XPickupCurrentPositionLimitMax_RT_S1的数据不能小于控件txtPixel_XPickupCurrentPositionLimitMin_RT_S1的数据！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtPixel_YPickupCurrentPositionLimitMax_RT_S1.Text) < Convert.ToSingle(txtPixel_YPickupCurrentPositionLimitMin_RT_S1.Text))
            {
                MyTool.TxtFileProcess.CreateLog("控件txtPixel_YPickupCurrentPositionLimitMax_RT_S1的数据不能小于控件txtPixel_YPickupCurrentPositionLimitMin_RT_S1的数据！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtPixel_YPickupCurrentPositionLimitMax_RT_S1的数据不能小于控件txtPixel_YPickupCurrentPositionLimitMin_RT_S1的数据！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtPixel_AnglePickupCurrentLimitMax_S1.Text) < Convert.ToSingle(txtPixel_AnglePickupCurrentLimitMin_S1.Text))
            {
                MyTool.TxtFileProcess.CreateLog("控件txtPixel_AnglePickupCurrentLimitMax_S1的数据不能小于控件txtPixel_AnglePickupCurrentLimitMin_S1的数据！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtPixel_AnglePickupCurrentLimitMax_S1的数据不能小于控件txtPixel_AnglePickupCurrentLimitMin_S1的数据！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtROB_XPickupCurrentPositionLimitMax_S1.Text) < Convert.ToSingle(txtROB_XPickupCurrentPositionLimitMin_S1.Text))
            {
                MyTool.TxtFileProcess.CreateLog("控件txtROB_XPickupCurrentPositionLimitMax_S1的数据不能小于控件txtROB_XPickupCurrentPositionLimitMin_S1的数据！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtROB_XPickupCurrentPositionLimitMax_S1的数据不能小于控件txtROB_XPickupCurrentPositionLimitMin_S1的数据！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtROB_YPickupCurrentPositionLimitMax_S1.Text) < Convert.ToSingle(txtROB_YPickupCurrentPositionLimitMin_S1.Text))
            {
                MyTool.TxtFileProcess.CreateLog("控件txtROB_YPickupCurrentPositionLimitMax_S1的数据不能小于控件txtROB_YPickupCurrentPositionLimitMin_S1的数据！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtROB_YPickupCurrentPositionLimitMax_S1的数据不能小于控件txtROB_YPickupCurrentPositionLimitMin_S1的数据！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtROB_RZPickupCurrentPositionLimitMax_S1.Text) < Convert.ToSingle(txtROB_RZPickupCurrentPositionLimitMin_S1.Text))
            {
                MyTool.TxtFileProcess.CreateLog("控件txtROB_RZPickupCurrentPositionLimitMax_S1的数据不能小于控件txtROB_RZPickupCurrentPositionLimitMin_S1的数据！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtROB_RZPickupCurrentPositionLimitMax_S1的数据不能小于控件txtROB_RZPickupCurrentPositionLimitMin_S1的数据！");
                dataCheckResult = false;
            }

            if (Convert.ToSingle(txtThresholdMinGray_S1.Text) < 0 || Convert.ToSingle(txtThresholdMinGray_S1.Text) > 255)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtThresholdMinGray_S1的数据范围错误,取值范围是[0,255]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtThresholdMinGray_S1的数据范围错误,取值范围是[0,255]！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtThresholdMaxGray_S1.Text) < 0 || Convert.ToSingle(txtThresholdMaxGray_S1.Text) > 255)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtThresholdMaxGray_S1的数据范围错误,取值范围是[0,255]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtThresholdMaxGray_S1的数据范围错误,取值范围是[0,255]！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtThresholdMaxGray_S1.Text) < Convert.ToSingle(txtThresholdMinGray_S1.Text))
            {
                MyTool.TxtFileProcess.CreateLog("控件txtThresholdMaxGray_S1的数据不能小于控件txtThresholdMinGray_S1的数据！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtThresholdMaxGray_S1的数据不能小于控件txtThresholdMinGray_S1的数据！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtOpeningWAndH_S1.Text) < 1 || Convert.ToSingle(txtOpeningWAndH_S1.Text) > 511)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtOpeningWAndH_S1的数据范围错误,取值范围是[1,511]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtOpeningWAndH_S1的数据范围错误,取值范围是[1,511]！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtShapeMinArea_S1.Text) < 0 || Convert.ToSingle(txtShapeMinArea_S1.Text) > 100000000)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtShapeMinArea_S1的数据范围错误,取值范围是[0,100000000]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtShapeMinArea_S1的数据范围错误,取值范围是[0,100000000]！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtShapeMaxArea_S1.Text) < 0 || Convert.ToSingle(txtShapeMaxArea_S1.Text) > 100000000)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtShapeMaxArea_S1的数据范围错误,取值范围是[0,100000000]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtShapeMaxArea_S1的数据范围错误,取值范围是[0,100000000]！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtShapeMaxArea_S1.Text) < Convert.ToSingle(txtShapeMinArea_S1.Text))
            {
                MyTool.TxtFileProcess.CreateLog("控件txtShapeMaxArea_S1的数据不能小于控件txtShapeMinArea的数据！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtShapeMaxArea_S1的数据不能小于控件txtShapeMinArea的数据！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtShapeMinRectangularity_S1.Text) < 0 || Convert.ToSingle(txtShapeMinRectangularity_S1.Text) > 1)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtShapeMinRectangularity_S1的数据范围错误, 取值范围是[0, 1]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtShapeMinRectangularity_S1的数据范围错误, 取值范围是[0, 1]！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtShapeMaxRectangularity_S1.Text) < 0 || Convert.ToSingle(txtShapeMaxRectangularity_S1.Text) > 1)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtShapeMaxRectangularity_S1的数据范围错误, 取值范围是[0, 1]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtShapeMaxRectangularity_S1的数据范围错误, 取值范围是[0, 1]！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtShapeMaxRectangularity_S1.Text) < Convert.ToSingle(txtShapeMinRectangularity_S1.Text))
            {
                MyTool.TxtFileProcess.CreateLog("控件txtShapeMaxRectangularity_S1的数据不能小于控件txtShapeMinRectangularity_S1的数据！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtShapeMaxRectangularity_S1的数据不能小于控件txtShapeMinRectangularity_S1的数据！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtDilationRectangleWidth_S1.Text) < 1 || Convert.ToSingle(txtDilationRectangleWidth_S1.Text) > 511)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtDilationRectangleWidth_S1的数据范围错误, 取值范围是[1, 511]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtDilationRectangleWidth_S1的数据范围错误, 取值范围是[1, 511]！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtDilationRectangleHeight_S1.Text) < 1 || Convert.ToSingle(txtDilationRectangleHeight_S1.Text) > 511)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtDilationRectangleHeight_S1的数据范围错误, 取值范围是[1, 511]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtDilationRectangleHeight_S1的数据范围错误, 取值范围是[1, 511]！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtEdgesSubPixCannyAlpha_S1.Text) < 0.2 || Convert.ToSingle(txtEdgesSubPixCannyAlpha_S1.Text) > 50)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtEdgesSubPixCannyAlpha_S1的数据范围错误, 取值范围是[0.2, 50]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtEdgesSubPixCannyAlpha_S1的数据范围错误, 取值范围是[0.2, 50]！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtEdgesSubPixCannyLow_S1.Text) < 1 || Convert.ToSingle(txtEdgesSubPixCannyLow_S1.Text) > 255)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtEdgesSubPixCannyLow_S1的数据范围错误, 取值范围是[1, 255]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtEdgesSubPixCannyLow_S1的数据范围错误, 取值范围是[1, 255]！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtEdgesSubPixCannyHigh_S1.Text) < 1 || Convert.ToSingle(txtEdgesSubPixCannyHigh_S1.Text) > 255)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtEdgesSubPixCannyHigh_S1的数据范围错误, 取值范围是[1, 255]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtEdgesSubPixCannyHigh_S1的数据范围错误, 取值范围是[1, 255]！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtEdgesSubPixCannyHigh_S1.Text) < Convert.ToSingle(txtEdgesSubPixCannyLow_S1.Text))
            {
                MyTool.TxtFileProcess.CreateLog("控件txtEdgesSubPixCannyHigh_S1的数据不能小于控件txtEdgesSubPixCannyLow_S1的数据！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtEdgesSubPixCannyHigh_S1的数据不能小于控件txtEdgesSubPixCannyLow_S1的数据！");
                dataCheckResult = false;
            }

            if (Convert.ToSingle(txtUnionCollinearContoursXldMaxDistAbs_S1.Text) < 0 || Convert.ToSingle(txtUnionCollinearContoursXldMaxDistAbs_S1.Text) > 100000000)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtUnionCollinearContoursXldMaxDistAbs_S1的数据范围错误, 取值范围是[0, 100000000]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtUnionCollinearContoursXldMaxDistAbs_S1的数据范围错误, 取值范围是[0, 100000000]！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtUnionCollinearContoursXldMaxDistRel_S1.Text) < 0 || Convert.ToSingle(txtUnionCollinearContoursXldMaxDistRel_S1.Text) > 100000000)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtUnionCollinearContoursXldMaxDistRel_S1的数据范围错误, 取值范围是[0, 100000000]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtUnionCollinearContoursXldMaxDistRel_S1的数据范围错误, 取值范围是[0, 100000000]！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtUnionCollinearContoursXldMaxShift_S1.Text) < 0 || Convert.ToSingle(txtUnionCollinearContoursXldMaxShift_S1.Text) > 100000000)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtUnionCollinearContoursXldMaxShift_S1的数据范围错误, 取值范围是[0, 100000000]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtUnionCollinearContoursXldMaxShift_S1的数据范围错误, 取值范围是[0, 100000000]！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtUnionCollinearContoursXldMaxAngle_S1.Text) < 0 || Convert.ToSingle(txtUnionCollinearContoursXldMaxAngle_S1.Text) > 0.78539816339)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtUnionCollinearContoursXldMaxAngle_S1的数据范围错误, 取值范围是[0, 0.78539816339]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtUnionCollinearContoursXldMaxAngle_S1的数据范围错误, 取值范围是[0, 0.78539816339]！");
                dataCheckResult = false;
            }

            if (Convert.ToSingle(txtXldContlengthMin_S1.Text) < 0 || Convert.ToSingle(txtXldContlengthMin_S1.Text) > 100000000)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtXldContlengthMin_S1的数据范围错误, 取值范围是[0, 100000000]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtXldContlengthMin_S1的数据范围错误, 取值范围是[0, 100000000]！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtXldContlengthMax_S1.Text) < 0 || Convert.ToSingle(txtXldContlengthMax_S1.Text) > 100000000)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtXldContlengthMax_S1的数据范围错误, 取值范围是[0, 99999]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtXldContlengthMax_S1的数据范围错误, 取值范围是[0, 100000000]！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtXldContlengthMax_S1.Text) < Convert.ToSingle(txtXldContlengthMin_S1.Text))
            {
                MyTool.TxtFileProcess.CreateLog("控件txtXldContlengthMax_S1的数据不能小于控件txtXldContlengthMin_S1的数据！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtXldContlengthMax_S1的数据不能小于控件txtXldContlengthMin_S1的数据！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtXLDMinArea_S1.Text) < 0 || Convert.ToSingle(txtXLDMinArea_S1.Text) > 100000000)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtXLDMinArea_S1的数据范围错误, 取值范围是[0, 100000000]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtXLDMinArea_S1的数据范围错误, 取值范围是[0, 100000000]！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtXLDMaxArea_S1.Text) < 0 || Convert.ToSingle(txtXLDMaxArea_S1.Text) > 100000000)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtXLDMaxArea_S1的数据范围错误, 取值范围是[0, 100000000]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtXLDMaxArea_S1的数据范围错误, 取值范围是[0, 100000000]！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtXLDMaxArea_S1.Text) < Convert.ToSingle(txtXLDMinArea_S1.Text))
            {
                MyTool.TxtFileProcess.CreateLog("控件txtXLDMaxArea_S1的数据不能小于控件txtXLDMinArea_S1的数据！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtXLDMaxArea_S1的数据不能小于控件txtXLDMinArea_S1的数据！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtXLDMinRectangularity_S1.Text) < 0 || Convert.ToSingle(txtXLDMinRectangularity_S1.Text) > 1)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtXLDMinRectangularity_S1的数据范围错误, 取值范围是[0, 1]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtXLDMinRectangularity_S1的数据范围错误, 取值范围是[0, 1]！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtXLDMaxRectangularity_S1.Text) < 0 || Convert.ToSingle(txtXLDMaxRectangularity_S1.Text) > 1)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtXLDMaxRectangularity_S1的数据范围错误, 取值范围是[0, 1]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtXLDMaxRectangularity_S1的数据范围错误, 取值范围是[0, 1]！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtXLDMaxRectangularity_S1.Text) < Convert.ToSingle(txtXLDMinRectangularity_S1.Text))
            {
                MyTool.TxtFileProcess.CreateLog("控件txtXLDMaxRectangularity_S1的数据不能小于控件txtXLDMinRectangularity_S1的数据！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtXLDMaxRectangularity_S1的数据不能小于控件txtXLDMinRectangularity_S1的数据！");
                dataCheckResult = false;
            }

            if (Convert.ToSingle(txtUnionAdjacentContoursXldMaxDistAbs_S1.Text) < 0 || Convert.ToSingle(txtUnionAdjacentContoursXldMaxDistAbs_S1.Text) > 100000000)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtUnionAdjacentContoursXldMaxDistAbs_S1的数据范围错误, 取值范围是[0, 100000000]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtUnionAdjacentContoursXldMaxDistAbs_S1的数据范围错误, 取值范围是[0, 100000000]！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtUnionAdjacentContoursXldMaxDistRel_S1.Text) < 0 || Convert.ToSingle(txtUnionAdjacentContoursXldMaxDistRel_S1.Text) > 100000000)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtUnionAdjacentContoursXldMaxDistRel_S1的数据范围错误, 取值范围是[0, 100000000]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtUnionAdjacentContoursXldMaxDistRel_S1的数据范围错误, 取值范围是[0, 100000000]！");
                dataCheckResult = false;
            }

            //Station2
            if (Convert.ToSingle(txtROB_XPickupTeachPosition_S2.Text) < -2000 || Convert.ToSingle(txtROB_XPickupTeachPosition_S2.Text) > 2000)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtROB_XPickupTeachPosition_S2的数据范围错误,取值范围是[-2000,2000]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtROB_XPickupTeachPosition_S2的数据范围错误,取值范围是[-2000, 2000]！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtROB_YPickupTeachPosition_S2.Text) < -2000 || Convert.ToSingle(txtROB_YPickupTeachPosition_S2.Text) > 2000)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtROB_YPickupTeachPosition_S2的数据范围错误,取值范围是[-2000,2000]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtROB_YPickupTeachPosition_S2的数据范围错误,取值范围是[-2000,2000]！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtROB_ZPickupTeachPosition_S2.Text) < -1000 || Convert.ToSingle(txtROB_ZPickupTeachPosition_S2.Text) > 1000)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtROB_ZPickupTeachPosition_S2的数据范围错误,取值范围是[-1000,1000]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtROB_ZPickupTeachPosition_S2的数据范围错误,取值范围是[-1000,1000]！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtROB_RXPickupTeachPosition_S2.Text) < -360 || Convert.ToSingle(txtROB_RXPickupTeachPosition_S2.Text) > 360)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtROB_RXPickupTeachPosition_S2的数据范围错误,取值范围是[-360,360]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtROB_RXPickupTeachPosition_S2的数据范围错误,取值范围是[-360,360]！！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtROB_RWPickupTeachPosition_S2.Text) < -360 || Convert.ToSingle(txtROB_RWPickupTeachPosition_S2.Text) > 360)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtROB_RWPickupTeachPosition_S2的数据范围错误,取值范围是[-360,360]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtROB_RWPickupTeachPosition_S2的数据范围错误,取值范围是[-360,360]！！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtROB_RZPickupTeachPosition_S2.Text) < -360 || Convert.ToSingle(txtROB_RZPickupTeachPosition_S2.Text) > 360)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtROB_RZPickupTeachPosition_S2的数据范围错误,取值范围是[-360,360]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtROB_RZPickupTeachPosition_S2的数据范围错误,取值范围是[-360,360]！！");
                dataCheckResult = false;
            }

            if (Convert.ToSingle(txtPixel_XPickupTeachPosition_RT_S2.Text) < 0 || Convert.ToSingle(txtPixel_XPickupTeachPosition_RT_S2.Text) > 5000)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtPixel_XPickupTeachPosition_RT_S2的数据范围错误,取值范围是[0,5000]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtPixel_XPickupTeachPosition_RT_S2的数据范围错误,取值范围是[0,5000]！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtPixel_YPickupTeachPosition_RT_S2.Text) < 0 || Convert.ToSingle(txtPixel_YPickupTeachPosition_RT_S2.Text) > 5000)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtPixel_YPickupTeachPosition_RT_S2的数据范围错误,取值范围是[0,5000]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtPixel_YPickupTeachPosition_RT_S2的数据范围错误,取值范围是[0,5000]！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtPixel_XPickupTeachPosition_LT_S2.Text) < 0 || Convert.ToSingle(txtPixel_XPickupTeachPosition_LT_S2.Text) > 5000)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtPixel_XPickupTeachPosition_LT_S2的数据范围错误,取值范围是[0,5000]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtPixel_XPickupTeachPosition_LT_S2的数据范围错误,取值范围是[0,5000]！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtPixel_YPickupTeachPosition_LT_S2.Text) < 0 || Convert.ToSingle(txtPixel_YPickupTeachPosition_LT_S2.Text) > 5000)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtPixel_YPickupTeachPosition_LT_S2的数据范围错误,取值范围是[0,5000]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtPixel_YPickupTeachPosition_LT_S2的数据范围错误,取值范围是[0,5000]！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtPixel_XPickupTeachPosition_LB_S2.Text) < 0 || Convert.ToSingle(txtPixel_XPickupTeachPosition_LB_S2.Text) > 5000)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtPixel_XPickupTeachPosition_LB_S2的数据范围错误,取值范围是[0,5000]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtPixel_XPickupTeachPosition_LB_S2的数据范围错误,取值范围是[0,5000]！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtPixel_YPickupTeachPosition_LB_S2.Text) < 0 || Convert.ToSingle(txtPixel_YPickupTeachPosition_LB_S2.Text) > 5000)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtPixel_YPickupTeachPosition_LB_S2的数据范围错误,取值范围是[0,5000]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtPixel_YPickupTeachPosition_LB_S2的数据范围错误,取值范围是[0,5000]！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtPixel_XPickupTeachPosition_RB_S2.Text) < 0 || Convert.ToSingle(txtPixel_XPickupTeachPosition_RB_S2.Text) > 5000)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtPixel_XPickupTeachPosition_RB_S2的数据范围错误,取值范围是[0,5000]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtPixel_XPickupTeachPosition_RB_S2的数据范围错误,取值范围是[0,5000]！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtPixel_YPickupTeachPosition_RB_S2.Text) < 0 || Convert.ToSingle(txtPixel_YPickupTeachPosition_RB_S2.Text) > 5000)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtPixel_YPickupTeachPosition_RB_S2的数据范围错误,取值范围是[0,5000]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtPixel_YPickupTeachPosition_RB_S2的数据范围错误,取值范围是[0,5000]！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtPixel_AnglePickupTeach_S2.Text) < -360 || Convert.ToSingle(txtPixel_AnglePickupTeach_S2.Text) > 360)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtPixel_AnglePickupTeach_S2的数据范围错误,取值范围是[-360,360]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtPixel_AnglePickupTeach_S2的数据范围错误,取值范围是[-360,360]！");
                dataCheckResult = false;
            }

            if (Convert.ToSingle(txtPixel_XPickupCurrentPositionLimitMax_RT_S2.Text) < Convert.ToSingle(txtPixel_XPickupCurrentPositionLimitMin_RT_S2.Text))
            {
                MyTool.TxtFileProcess.CreateLog("控件txtPixel_XPickupCurrentPositionLimitMax_RT_S2的数据不能小于控件txtPixel_XPickupCurrentPositionLimitMin_RT_S2的数据！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtPixel_XPickupCurrentPositionLimitMax_RT_S2的数据不能小于控件txtPixel_XPickupCurrentPositionLimitMin_RT_S2的数据！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtPixel_YPickupCurrentPositionLimitMax_RT_S2.Text) < Convert.ToSingle(txtPixel_YPickupCurrentPositionLimitMin_RT_S2.Text))
            {
                MyTool.TxtFileProcess.CreateLog("控件txtPixel_YPickupCurrentPositionLimitMax_RT_S2的数据不能小于控件txtPixel_YPickupCurrentPositionLimitMin_RT_S2的数据！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtPixel_YPickupCurrentPositionLimitMax_RT_S2的数据不能小于控件txtPixel_YPickupCurrentPositionLimitMin_RT_S2的数据！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtPixel_AnglePickupCurrentLimitMax_S2.Text) < Convert.ToSingle(txtPixel_AnglePickupCurrentLimitMin_S2.Text))
            {
                MyTool.TxtFileProcess.CreateLog("控件txtPixel_AnglePickupCurrentLimitMax_S2的数据不能小于控件txtPixel_AnglePickupCurrentLimitMin_S2的数据！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtPixel_AnglePickupCurrentLimitMax_S2的数据不能小于控件txtPixel_AnglePickupCurrentLimitMin_S2的数据！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtROB_XPickupCurrentPositionLimitMax_S2.Text) < Convert.ToSingle(txtROB_XPickupCurrentPositionLimitMin_S2.Text))
            {
                MyTool.TxtFileProcess.CreateLog("控件txtROB_XPickupCurrentPositionLimitMax_S2的数据不能小于控件txtROB_XPickupCurrentPositionLimitMin_S2的数据！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtROB_XPickupCurrentPositionLimitMax_S2的数据不能小于控件txtROB_XPickupCurrentPositionLimitMin_S2的数据！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtROB_YPickupCurrentPositionLimitMax_S2.Text) < Convert.ToSingle(txtROB_YPickupCurrentPositionLimitMin_S2.Text))
            {
                MyTool.TxtFileProcess.CreateLog("控件txtROB_YPickupCurrentPositionLimitMax_S2的数据不能小于控件txtROB_YPickupCurrentPositionLimitMin_S2的数据！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtROB_YPickupCurrentPositionLimitMax_S2的数据不能小于控件txtROB_YPickupCurrentPositionLimitMin_S2的数据！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtROB_RZPickupCurrentPositionLimitMax_S2.Text) < Convert.ToSingle(txtROB_RZPickupCurrentPositionLimitMin_S2.Text))
            {
                MyTool.TxtFileProcess.CreateLog("控件txtROB_RZPickupCurrentPositionLimitMax_S2的数据不能小于控件txtROB_RZPickupCurrentPositionLimitMin_S2的数据！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtROB_RZPickupCurrentPositionLimitMax_S2的数据不能小于控件txtROB_RZPickupCurrentPositionLimitMin_S2的数据！");
                dataCheckResult = false;
            }

            if (Convert.ToSingle(txtThresholdMinGray_S2.Text) < 0 || Convert.ToSingle(txtThresholdMinGray_S2.Text) > 255)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtThresholdMinGray_S2的数据范围错误,取值范围是[0,255]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtThresholdMinGray_S2的数据范围错误,取值范围是[0,255]！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtThresholdMaxGray_S2.Text) < 0 || Convert.ToSingle(txtThresholdMaxGray_S2.Text) > 255)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtThresholdMaxGray_S2的数据范围错误,取值范围是[0,255]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtThresholdMaxGray_S2的数据范围错误,取值范围是[0,255]！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtThresholdMaxGray_S2.Text) < Convert.ToSingle(txtThresholdMinGray_S2.Text))
            {
                MyTool.TxtFileProcess.CreateLog("控件txtThresholdMaxGray_S2的数据不能小于控件txtThresholdMinGray_S2的数据！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtThresholdMaxGray_S2的数据不能小于控件txtThresholdMinGray_S2的数据！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtOpeningWAndH_S2.Text) < 1 || Convert.ToSingle(txtOpeningWAndH_S2.Text) > 511)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtOpeningWAndH_S2的数据范围错误,取值范围是[1,511]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtOpeningWAndH_S2的数据范围错误,取值范围是[1,511]！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtShapeMinArea_S2.Text) < 0 || Convert.ToSingle(txtShapeMinArea_S2.Text) > 100000000)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtShapeMinArea_S2的数据范围错误,取值范围是[0,100000000]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtShapeMinArea_S2的数据范围错误,取值范围是[0,100000000]！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtShapeMaxArea_S2.Text) < 0 || Convert.ToSingle(txtShapeMaxArea_S2.Text) > 100000000)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtShapeMaxArea_S2的数据范围错误,取值范围是[0,100000000]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtShapeMaxArea_S2的数据范围错误,取值范围是[0,100000000]！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtShapeMaxArea_S2.Text) < Convert.ToSingle(txtShapeMinArea_S2.Text))
            {
                MyTool.TxtFileProcess.CreateLog("控件txtShapeMaxArea_S2的数据不能小于控件txtShapeMinArea的数据！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtShapeMaxArea_S2的数据不能小于控件txtShapeMinArea的数据！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtShapeMinRectangularity_S2.Text) < 0 || Convert.ToSingle(txtShapeMinRectangularity_S2.Text) > 1)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtShapeMinRectangularity_S2的数据范围错误, 取值范围是[0, 1]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtShapeMinRectangularity_S2的数据范围错误, 取值范围是[0, 1]！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtShapeMaxRectangularity_S2.Text) < 0 || Convert.ToSingle(txtShapeMaxRectangularity_S2.Text) > 1)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtShapeMaxRectangularity_S2的数据范围错误, 取值范围是[0, 1]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtShapeMaxRectangularity_S2的数据范围错误, 取值范围是[0, 1]！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtShapeMaxRectangularity_S2.Text) < Convert.ToSingle(txtShapeMinRectangularity_S2.Text))
            {
                MyTool.TxtFileProcess.CreateLog("控件txtShapeMaxRectangularity_S2的数据不能小于控件txtShapeMinRectangularity_S2的数据！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtShapeMaxRectangularity_S2的数据不能小于控件txtShapeMinRectangularity_S2的数据！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtDilationRectangleWidth_S2.Text) < 1 || Convert.ToSingle(txtDilationRectangleWidth_S2.Text) > 511)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtDilationRectangleWidth_S2的数据范围错误, 取值范围是[1, 511]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtDilationRectangleWidth_S2的数据范围错误, 取值范围是[1, 511]！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtDilationRectangleHeight_S2.Text) < 1 || Convert.ToSingle(txtDilationRectangleHeight_S2.Text) > 511)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtDilationRectangleHeight_S2的数据范围错误, 取值范围是[1, 511]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtDilationRectangleHeight_S2的数据范围错误, 取值范围是[1, 511]！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtEdgesSubPixCannyAlpha_S2.Text) < 0.2 || Convert.ToSingle(txtEdgesSubPixCannyAlpha_S2.Text) > 50)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtEdgesSubPixCannyAlpha_S2的数据范围错误, 取值范围是[0.2, 50]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtEdgesSubPixCannyAlpha_S2的数据范围错误, 取值范围是[0.2, 50]！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtEdgesSubPixCannyLow_S2.Text) < 1 || Convert.ToSingle(txtEdgesSubPixCannyLow_S2.Text) > 255)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtEdgesSubPixCannyLow_S2的数据范围错误, 取值范围是[1, 255]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtEdgesSubPixCannyLow_S2的数据范围错误, 取值范围是[1, 255]！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtEdgesSubPixCannyHigh_S2.Text) < 1 || Convert.ToSingle(txtEdgesSubPixCannyHigh_S2.Text) > 255)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtEdgesSubPixCannyHigh_S2的数据范围错误, 取值范围是[1, 255]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtEdgesSubPixCannyHigh_S2的数据范围错误, 取值范围是[1, 255]！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtEdgesSubPixCannyHigh_S2.Text) < Convert.ToSingle(txtEdgesSubPixCannyLow_S2.Text))
            {
                MyTool.TxtFileProcess.CreateLog("控件txtEdgesSubPixCannyHigh_S2的数据不能小于控件txtEdgesSubPixCannyLow_S2的数据！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtEdgesSubPixCannyHigh_S2的数据不能小于控件txtEdgesSubPixCannyLow_S2的数据！");
                dataCheckResult = false;
            }

            if (Convert.ToSingle(txtUnionCollinearContoursXldMaxDistAbs_S2.Text) < 0 || Convert.ToSingle(txtUnionCollinearContoursXldMaxDistAbs_S2.Text) > 100000000)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtUnionCollinearContoursXldMaxDistAbs_S2的数据范围错误, 取值范围是[0, 100000000]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtUnionCollinearContoursXldMaxDistAbs_S2的数据范围错误, 取值范围是[0, 100000000]！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtUnionCollinearContoursXldMaxDistRel_S2.Text) < 0 || Convert.ToSingle(txtUnionCollinearContoursXldMaxDistRel_S2.Text) > 100000000)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtUnionCollinearContoursXldMaxDistRel_S2的数据范围错误, 取值范围是[0, 100000000]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtUnionCollinearContoursXldMaxDistRel_S2的数据范围错误, 取值范围是[0, 100000000]！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtUnionCollinearContoursXldMaxShift_S2.Text) < 0 || Convert.ToSingle(txtUnionCollinearContoursXldMaxShift_S2.Text) > 100000000)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtUnionCollinearContoursXldMaxShift_S2的数据范围错误, 取值范围是[0, 100000000]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtUnionCollinearContoursXldMaxShift_S2的数据范围错误, 取值范围是[0, 100000000]！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtUnionCollinearContoursXldMaxAngle_S2.Text) < 0 || Convert.ToSingle(txtUnionCollinearContoursXldMaxAngle_S2.Text) > 0.78539816339)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtUnionCollinearContoursXldMaxAngle_S2的数据范围错误, 取值范围是[0, 0.78539816339]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtUnionCollinearContoursXldMaxAngle_S2的数据范围错误, 取值范围是[0, 0.78539816339]！");
                dataCheckResult = false;
            }

            if (Convert.ToSingle(txtXldContlengthMin_S2.Text) < 0 || Convert.ToSingle(txtXldContlengthMin_S2.Text) > 100000000)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtXldContlengthMin_S2的数据范围错误, 取值范围是[0, 100000000]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtXldContlengthMin_S2的数据范围错误, 取值范围是[0, 100000000]！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtXldContlengthMax_S2.Text) < 0 || Convert.ToSingle(txtXldContlengthMax_S2.Text) > 100000000)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtXldContlengthMax_S2的数据范围错误, 取值范围是[0, 99999]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtXldContlengthMax_S2的数据范围错误, 取值范围是[0, 100000000]！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtXldContlengthMax_S2.Text) < Convert.ToSingle(txtXldContlengthMin_S2.Text))
            {
                MyTool.TxtFileProcess.CreateLog("控件txtXldContlengthMax_S2的数据不能小于控件txtXldContlengthMin_S2的数据！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtXldContlengthMax_S2的数据不能小于控件txtXldContlengthMin_S2的数据！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtXLDMinArea_S2.Text) < 0 || Convert.ToSingle(txtXLDMinArea_S2.Text) > 100000000)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtXLDMinArea_S2的数据范围错误, 取值范围是[0, 100000000]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtXLDMinArea_S2的数据范围错误, 取值范围是[0, 100000000]！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtXLDMaxArea_S2.Text) < 0 || Convert.ToSingle(txtXLDMaxArea_S2.Text) > 100000000)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtXLDMaxArea_S2的数据范围错误, 取值范围是[0, 100000000]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtXLDMaxArea_S2的数据范围错误, 取值范围是[0, 100000000]！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtXLDMaxArea_S2.Text) < Convert.ToSingle(txtXLDMinArea_S2.Text))
            {
                MyTool.TxtFileProcess.CreateLog("控件txtXLDMaxArea_S2的数据不能小于控件txtXLDMinArea_S2的数据！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtXLDMaxArea_S2的数据不能小于控件txtXLDMinArea_S2的数据！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtXLDMinRectangularity_S2.Text) < 0 || Convert.ToSingle(txtXLDMinRectangularity_S2.Text) > 1)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtXLDMinRectangularity_S2的数据范围错误, 取值范围是[0, 1]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtXLDMinRectangularity_S2的数据范围错误, 取值范围是[0, 1]！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtXLDMaxRectangularity_S2.Text) < 0 || Convert.ToSingle(txtXLDMaxRectangularity_S2.Text) > 1)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtXLDMaxRectangularity_S2的数据范围错误, 取值范围是[0, 1]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtXLDMaxRectangularity_S2的数据范围错误, 取值范围是[0, 1]！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtXLDMaxRectangularity_S2.Text) < Convert.ToSingle(txtXLDMinRectangularity_S2.Text))
            {
                MyTool.TxtFileProcess.CreateLog("控件txtXLDMaxRectangularity_S2的数据不能小于控件txtXLDMinRectangularity_S2的数据！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtXLDMaxRectangularity_S2的数据不能小于控件txtXLDMinRectangularity_S2的数据！");
                dataCheckResult = false;
            }

            if (Convert.ToSingle(txtUnionAdjacentContoursXldMaxDistAbs_S2.Text) < 0 || Convert.ToSingle(txtUnionAdjacentContoursXldMaxDistAbs_S2.Text) > 100000000)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtUnionAdjacentContoursXldMaxDistAbs_S2的数据范围错误, 取值范围是[0, 100000000]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtUnionAdjacentContoursXldMaxDistAbs_S2的数据范围错误, 取值范围是[0, 100000000]！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtUnionAdjacentContoursXldMaxDistRel_S2.Text) < 0 || Convert.ToSingle(txtUnionAdjacentContoursXldMaxDistRel_S2.Text) > 100000000)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtUnionAdjacentContoursXldMaxDistRel_S2的数据范围错误, 取值范围是[0, 100000000]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtUnionAdjacentContoursXldMaxDistRel_S2的数据范围错误, 取值范围是[0, 100000000]！");
                dataCheckResult = false;
            }

            //Station3
            if (Convert.ToSingle(txtROB_XPickupTeachPosition_S3.Text) < -2000 || Convert.ToSingle(txtROB_XPickupTeachPosition_S3.Text) > 2000)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtROB_XPickupTeachPosition_S3的数据范围错误,取值范围是[-2000,2000]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtROB_XPickupTeachPosition_S3的数据范围错误,取值范围是[-2000, 2000]！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtROB_YPickupTeachPosition_S3.Text) < -2000 || Convert.ToSingle(txtROB_YPickupTeachPosition_S3.Text) > 2000)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtROB_YPickupTeachPosition_S3的数据范围错误,取值范围是[-2000,2000]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtROB_YPickupTeachPosition_S3的数据范围错误,取值范围是[-2000,2000]！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtROB_ZPickupTeachPosition_S3.Text) < -1000 || Convert.ToSingle(txtROB_ZPickupTeachPosition_S3.Text) > 1000)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtROB_ZPickupTeachPosition_S3的数据范围错误,取值范围是[-1000,1000]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtROB_ZPickupTeachPosition_S3的数据范围错误,取值范围是[-1000,1000]！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtROB_RXPickupTeachPosition_S3.Text) < -360 || Convert.ToSingle(txtROB_RXPickupTeachPosition_S3.Text) > 360)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtROB_RXPickupTeachPosition_S3的数据范围错误,取值范围是[-360,360]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtROB_RXPickupTeachPosition_S3的数据范围错误,取值范围是[-360,360]！！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtROB_RWPickupTeachPosition_S3.Text) < -360 || Convert.ToSingle(txtROB_RWPickupTeachPosition_S3.Text) > 360)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtROB_RWPickupTeachPosition_S3的数据范围错误,取值范围是[-360,360]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtROB_RWPickupTeachPosition_S3的数据范围错误,取值范围是[-360,360]！！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtROB_RZPickupTeachPosition_S3.Text) < -360 || Convert.ToSingle(txtROB_RZPickupTeachPosition_S3.Text) > 360)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtROB_RZPickupTeachPosition_S3的数据范围错误,取值范围是[-360,360]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtROB_RZPickupTeachPosition_S3的数据范围错误,取值范围是[-360,360]！！");
                dataCheckResult = false;
            }

            if (Convert.ToSingle(txtPixel_XPickupTeachPosition_RT_S3.Text) < 0 || Convert.ToSingle(txtPixel_XPickupTeachPosition_RT_S3.Text) > 5000)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtPixel_XPickupTeachPosition_RT_S3的数据范围错误,取值范围是[0,5000]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtPixel_XPickupTeachPosition_RT_S3的数据范围错误,取值范围是[0,5000]！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtPixel_YPickupTeachPosition_RT_S3.Text) < 0 || Convert.ToSingle(txtPixel_YPickupTeachPosition_RT_S3.Text) > 5000)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtPixel_YPickupTeachPosition_RT_S3的数据范围错误,取值范围是[0,5000]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtPixel_YPickupTeachPosition_RT_S3的数据范围错误,取值范围是[0,5000]！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtPixel_XPickupTeachPosition_LT_S3.Text) < 0 || Convert.ToSingle(txtPixel_XPickupTeachPosition_LT_S3.Text) > 5000)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtPixel_XPickupTeachPosition_LT_S3的数据范围错误,取值范围是[0,5000]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtPixel_XPickupTeachPosition_LT_S3的数据范围错误,取值范围是[0,5000]！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtPixel_YPickupTeachPosition_LT_S3.Text) < 0 || Convert.ToSingle(txtPixel_YPickupTeachPosition_LT_S3.Text) > 5000)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtPixel_YPickupTeachPosition_LT_S3的数据范围错误,取值范围是[0,5000]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtPixel_YPickupTeachPosition_LT_S3的数据范围错误,取值范围是[0,5000]！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtPixel_XPickupTeachPosition_LB_S3.Text) < 0 || Convert.ToSingle(txtPixel_XPickupTeachPosition_LB_S3.Text) > 5000)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtPixel_XPickupTeachPosition_LB_S3的数据范围错误,取值范围是[0,5000]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtPixel_XPickupTeachPosition_LB_S3的数据范围错误,取值范围是[0,5000]！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtPixel_YPickupTeachPosition_LB_S3.Text) < 0 || Convert.ToSingle(txtPixel_YPickupTeachPosition_LB_S3.Text) > 5000)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtPixel_YPickupTeachPosition_LB_S3的数据范围错误,取值范围是[0,5000]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtPixel_YPickupTeachPosition_LB_S3的数据范围错误,取值范围是[0,5000]！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtPixel_XPickupTeachPosition_RB_S3.Text) < 0 || Convert.ToSingle(txtPixel_XPickupTeachPosition_RB_S3.Text) > 5000)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtPixel_XPickupTeachPosition_RB_S3的数据范围错误,取值范围是[0,5000]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtPixel_XPickupTeachPosition_RB_S3的数据范围错误,取值范围是[0,5000]！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtPixel_YPickupTeachPosition_RB_S3.Text) < 0 || Convert.ToSingle(txtPixel_YPickupTeachPosition_RB_S3.Text) > 5000)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtPixel_YPickupTeachPosition_RB_S3的数据范围错误,取值范围是[0,5000]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtPixel_YPickupTeachPosition_RB_S3的数据范围错误,取值范围是[0,5000]！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtPixel_AnglePickupTeach_S3.Text) < -360 || Convert.ToSingle(txtPixel_AnglePickupTeach_S3.Text) > 360)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtPixel_AnglePickupTeach_S3的数据范围错误,取值范围是[-360,360]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtPixel_AnglePickupTeach_S3的数据范围错误,取值范围是[-360,360]！");
                dataCheckResult = false;
            }

            if (Convert.ToSingle(txtPixel_XPickupCurrentPositionLimitMax_RT_S3.Text) < Convert.ToSingle(txtPixel_XPickupCurrentPositionLimitMin_RT_S3.Text))
            {
                MyTool.TxtFileProcess.CreateLog("控件txtPixel_XPickupCurrentPositionLimitMax_RT_S3的数据不能小于控件txtPixel_XPickupCurrentPositionLimitMin_RT_S3的数据！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtPixel_XPickupCurrentPositionLimitMax_RT_S3的数据不能小于控件txtPixel_XPickupCurrentPositionLimitMin_RT_S3的数据！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtPixel_YPickupCurrentPositionLimitMax_RT_S3.Text) < Convert.ToSingle(txtPixel_YPickupCurrentPositionLimitMin_RT_S3.Text))
            {
                MyTool.TxtFileProcess.CreateLog("控件txtPixel_YPickupCurrentPositionLimitMax_RT_S3的数据不能小于控件txtPixel_YPickupCurrentPositionLimitMin_RT_S3的数据！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtPixel_YPickupCurrentPositionLimitMax_RT_S3的数据不能小于控件txtPixel_YPickupCurrentPositionLimitMin_RT_S3的数据！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtPixel_AnglePickupCurrentLimitMax_S3.Text) < Convert.ToSingle(txtPixel_AnglePickupCurrentLimitMin_S3.Text))
            {
                MyTool.TxtFileProcess.CreateLog("控件txtPixel_AnglePickupCurrentLimitMax_S3的数据不能小于控件txtPixel_AnglePickupCurrentLimitMin_S3的数据！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtPixel_AnglePickupCurrentLimitMax_S3的数据不能小于控件txtPixel_AnglePickupCurrentLimitMin_S3的数据！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtROB_XPickupCurrentPositionLimitMax_S3.Text) < Convert.ToSingle(txtROB_XPickupCurrentPositionLimitMin_S3.Text))
            {
                MyTool.TxtFileProcess.CreateLog("控件txtROB_XPickupCurrentPositionLimitMax_S3的数据不能小于控件txtROB_XPickupCurrentPositionLimitMin_S3的数据！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtROB_XPickupCurrentPositionLimitMax_S3的数据不能小于控件txtROB_XPickupCurrentPositionLimitMin_S3的数据！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtROB_YPickupCurrentPositionLimitMax_S3.Text) < Convert.ToSingle(txtROB_YPickupCurrentPositionLimitMin_S3.Text))
            {
                MyTool.TxtFileProcess.CreateLog("控件txtROB_YPickupCurrentPositionLimitMax_S3的数据不能小于控件txtROB_YPickupCurrentPositionLimitMin_S3的数据！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtROB_YPickupCurrentPositionLimitMax_S3的数据不能小于控件txtROB_YPickupCurrentPositionLimitMin_S3的数据！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtROB_RZPickupCurrentPositionLimitMax_S3.Text) < Convert.ToSingle(txtROB_RZPickupCurrentPositionLimitMin_S3.Text))
            {
                MyTool.TxtFileProcess.CreateLog("控件txtROB_RZPickupCurrentPositionLimitMax_S3的数据不能小于控件txtROB_RZPickupCurrentPositionLimitMin_S3的数据！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtROB_RZPickupCurrentPositionLimitMax_S3的数据不能小于控件txtROB_RZPickupCurrentPositionLimitMin_S3的数据！");
                dataCheckResult = false;
            }

            if (Convert.ToSingle(txtThresholdMinGray_S3.Text) < 0 || Convert.ToSingle(txtThresholdMinGray_S3.Text) > 255)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtThresholdMinGray_S3的数据范围错误,取值范围是[0,255]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtThresholdMinGray_S3的数据范围错误,取值范围是[0,255]！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtThresholdMaxGray_S3.Text) < 0 || Convert.ToSingle(txtThresholdMaxGray_S3.Text) > 255)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtThresholdMaxGray_S3的数据范围错误,取值范围是[0,255]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtThresholdMaxGray_S3的数据范围错误,取值范围是[0,255]！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtThresholdMaxGray_S3.Text) < Convert.ToSingle(txtThresholdMinGray_S3.Text))
            {
                MyTool.TxtFileProcess.CreateLog("控件txtThresholdMaxGray_S3的数据不能小于控件txtThresholdMinGray_S3的数据！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtThresholdMaxGray_S3的数据不能小于控件txtThresholdMinGray_S3的数据！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtOpeningWAndH_S3.Text) < 1 || Convert.ToSingle(txtOpeningWAndH_S3.Text) > 511)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtOpeningWAndH_S3的数据范围错误,取值范围是[1,511]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtOpeningWAndH_S3的数据范围错误,取值范围是[1,511]！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtShapeMinArea_S3.Text) < 0 || Convert.ToSingle(txtShapeMinArea_S3.Text) > 100000000)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtShapeMinArea_S3的数据范围错误,取值范围是[0,100000000]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtShapeMinArea_S3的数据范围错误,取值范围是[0,100000000]！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtShapeMaxArea_S3.Text) < 0 || Convert.ToSingle(txtShapeMaxArea_S3.Text) > 100000000)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtShapeMaxArea_S3的数据范围错误,取值范围是[0,100000000]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtShapeMaxArea_S3的数据范围错误,取值范围是[0,100000000]！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtShapeMaxArea_S3.Text) < Convert.ToSingle(txtShapeMinArea_S3.Text))
            {
                MyTool.TxtFileProcess.CreateLog("控件txtShapeMaxArea_S3的数据不能小于控件txtShapeMinArea的数据！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtShapeMaxArea_S3的数据不能小于控件txtShapeMinArea的数据！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtShapeMinRectangularity_S3.Text) < 0 || Convert.ToSingle(txtShapeMinRectangularity_S3.Text) > 1)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtShapeMinRectangularity_S3的数据范围错误, 取值范围是[0, 1]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtShapeMinRectangularity_S3的数据范围错误, 取值范围是[0, 1]！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtShapeMaxRectangularity_S3.Text) < 0 || Convert.ToSingle(txtShapeMaxRectangularity_S3.Text) > 1)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtShapeMaxRectangularity_S3的数据范围错误, 取值范围是[0, 1]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtShapeMaxRectangularity_S3的数据范围错误, 取值范围是[0, 1]！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtShapeMaxRectangularity_S3.Text) < Convert.ToSingle(txtShapeMinRectangularity_S3.Text))
            {
                MyTool.TxtFileProcess.CreateLog("控件txtShapeMaxRectangularity_S3的数据不能小于控件txtShapeMinRectangularity_S3的数据！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtShapeMaxRectangularity_S3的数据不能小于控件txtShapeMinRectangularity_S3的数据！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtDilationRectangleWidth_S3.Text) < 1 || Convert.ToSingle(txtDilationRectangleWidth_S3.Text) > 511)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtDilationRectangleWidth_S3的数据范围错误, 取值范围是[1, 511]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtDilationRectangleWidth_S3的数据范围错误, 取值范围是[1, 511]！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtDilationRectangleHeight_S3.Text) < 1 || Convert.ToSingle(txtDilationRectangleHeight_S3.Text) > 511)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtDilationRectangleHeight_S3的数据范围错误, 取值范围是[1, 511]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtDilationRectangleHeight_S3的数据范围错误, 取值范围是[1, 511]！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtEdgesSubPixCannyAlpha_S3.Text) < 0.2 || Convert.ToSingle(txtEdgesSubPixCannyAlpha_S3.Text) > 50)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtEdgesSubPixCannyAlpha_S3的数据范围错误, 取值范围是[0.2, 50]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtEdgesSubPixCannyAlpha_S3的数据范围错误, 取值范围是[0.2, 50]！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtEdgesSubPixCannyLow_S3.Text) < 1 || Convert.ToSingle(txtEdgesSubPixCannyLow_S3.Text) > 255)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtEdgesSubPixCannyLow_S3的数据范围错误, 取值范围是[1, 255]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtEdgesSubPixCannyLow_S3的数据范围错误, 取值范围是[1, 255]！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtEdgesSubPixCannyHigh_S3.Text) < 1 || Convert.ToSingle(txtEdgesSubPixCannyHigh_S3.Text) > 255)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtEdgesSubPixCannyHigh_S3的数据范围错误, 取值范围是[1, 255]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtEdgesSubPixCannyHigh_S3的数据范围错误, 取值范围是[1, 255]！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtEdgesSubPixCannyHigh_S3.Text) < Convert.ToSingle(txtEdgesSubPixCannyLow_S3.Text))
            {
                MyTool.TxtFileProcess.CreateLog("控件txtEdgesSubPixCannyHigh_S3的数据不能小于控件txtEdgesSubPixCannyLow_S3的数据！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtEdgesSubPixCannyHigh_S3的数据不能小于控件txtEdgesSubPixCannyLow_S3的数据！");
                dataCheckResult = false;
            }

            if (Convert.ToSingle(txtUnionCollinearContoursXldMaxDistAbs_S3.Text) < 0 || Convert.ToSingle(txtUnionCollinearContoursXldMaxDistAbs_S3.Text) > 100000000)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtUnionCollinearContoursXldMaxDistAbs_S3的数据范围错误, 取值范围是[0, 100000000]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtUnionCollinearContoursXldMaxDistAbs_S3的数据范围错误, 取值范围是[0, 100000000]！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtUnionCollinearContoursXldMaxDistRel_S3.Text) < 0 || Convert.ToSingle(txtUnionCollinearContoursXldMaxDistRel_S3.Text) > 100000000)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtUnionCollinearContoursXldMaxDistRel_S3的数据范围错误, 取值范围是[0, 100000000]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtUnionCollinearContoursXldMaxDistRel_S3的数据范围错误, 取值范围是[0, 100000000]！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtUnionCollinearContoursXldMaxShift_S3.Text) < 0 || Convert.ToSingle(txtUnionCollinearContoursXldMaxShift_S3.Text) > 100000000)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtUnionCollinearContoursXldMaxShift_S3的数据范围错误, 取值范围是[0, 100000000]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtUnionCollinearContoursXldMaxShift_S3的数据范围错误, 取值范围是[0, 100000000]！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtUnionCollinearContoursXldMaxAngle_S3.Text) < 0 || Convert.ToSingle(txtUnionCollinearContoursXldMaxAngle_S3.Text) > 0.78539816339)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtUnionCollinearContoursXldMaxAngle_S3的数据范围错误, 取值范围是[0, 0.78539816339]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtUnionCollinearContoursXldMaxAngle_S3的数据范围错误, 取值范围是[0, 0.78539816339]！");
                dataCheckResult = false;
            }

            if (Convert.ToSingle(txtXldContlengthMin_S3.Text) < 0 || Convert.ToSingle(txtXldContlengthMin_S3.Text) > 100000000)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtXldContlengthMin_S3的数据范围错误, 取值范围是[0, 100000000]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtXldContlengthMin_S3的数据范围错误, 取值范围是[0, 100000000]！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtXldContlengthMax_S3.Text) < 0 || Convert.ToSingle(txtXldContlengthMax_S3.Text) > 100000000)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtXldContlengthMax_S3的数据范围错误, 取值范围是[0, 99999]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtXldContlengthMax_S3的数据范围错误, 取值范围是[0, 100000000]！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtXldContlengthMax_S3.Text) < Convert.ToSingle(txtXldContlengthMin_S3.Text))
            {
                MyTool.TxtFileProcess.CreateLog("控件txtXldContlengthMax_S3的数据不能小于控件txtXldContlengthMin_S3的数据！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtXldContlengthMax_S3的数据不能小于控件txtXldContlengthMin_S3的数据！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtXLDMinArea_S3.Text) < 0 || Convert.ToSingle(txtXLDMinArea_S3.Text) > 100000000)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtXLDMinArea_S3的数据范围错误, 取值范围是[0, 100000000]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtXLDMinArea_S3的数据范围错误, 取值范围是[0, 100000000]！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtXLDMaxArea_S3.Text) < 0 || Convert.ToSingle(txtXLDMaxArea_S3.Text) > 100000000)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtXLDMaxArea_S3的数据范围错误, 取值范围是[0, 100000000]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtXLDMaxArea_S3的数据范围错误, 取值范围是[0, 100000000]！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtXLDMaxArea_S3.Text) < Convert.ToSingle(txtXLDMinArea_S3.Text))
            {
                MyTool.TxtFileProcess.CreateLog("控件txtXLDMaxArea_S3的数据不能小于控件txtXLDMinArea_S3的数据！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtXLDMaxArea_S3的数据不能小于控件txtXLDMinArea_S3的数据！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtXLDMinRectangularity_S3.Text) < 0 || Convert.ToSingle(txtXLDMinRectangularity_S3.Text) > 1)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtXLDMinRectangularity_S3的数据范围错误, 取值范围是[0, 1]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtXLDMinRectangularity_S3的数据范围错误, 取值范围是[0, 1]！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtXLDMaxRectangularity_S3.Text) < 0 || Convert.ToSingle(txtXLDMaxRectangularity_S3.Text) > 1)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtXLDMaxRectangularity_S3的数据范围错误, 取值范围是[0, 1]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtXLDMaxRectangularity_S3的数据范围错误, 取值范围是[0, 1]！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtXLDMaxRectangularity_S3.Text) < Convert.ToSingle(txtXLDMinRectangularity_S3.Text))
            {
                MyTool.TxtFileProcess.CreateLog("控件txtXLDMaxRectangularity_S3的数据不能小于控件txtXLDMinRectangularity_S3的数据！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtXLDMaxRectangularity_S3的数据不能小于控件txtXLDMinRectangularity_S3的数据！");
                dataCheckResult = false;
            }

            if (Convert.ToSingle(txtUnionAdjacentContoursXldMaxDistAbs_S3.Text) < 0 || Convert.ToSingle(txtUnionAdjacentContoursXldMaxDistAbs_S3.Text) > 100000000)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtUnionAdjacentContoursXldMaxDistAbs_S3的数据范围错误, 取值范围是[0, 100000000]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtUnionAdjacentContoursXldMaxDistAbs_S3的数据范围错误, 取值范围是[0, 100000000]！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtUnionAdjacentContoursXldMaxDistRel_S3.Text) < 0 || Convert.ToSingle(txtUnionAdjacentContoursXldMaxDistRel_S3.Text) > 100000000)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtUnionAdjacentContoursXldMaxDistRel_S3的数据范围错误, 取值范围是[0, 100000000]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtUnionAdjacentContoursXldMaxDistRel_S3的数据范围错误, 取值范围是[0, 100000000]！");
                dataCheckResult = false;
            }


            if (Convert.ToSingle(txtClassificationThresholdMin_S3.Text) < 0 || Convert.ToSingle(txtClassificationThresholdMin_S3.Text) > 255)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtClassificationThresholdMin_S3的数据范围错误,取值范围是[0,255]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtClassificationThresholdMin_S3的数据范围错误,取值范围是[0,255]！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtClassificationThresholdMax_S3.Text) < 0 || Convert.ToSingle(txtClassificationThresholdMax_S3.Text) > 255)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtClassificationThresholdMax_S3的数据范围错误,取值范围是[0,255]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtClassificationThresholdMax_S3的数据范围错误,取值范围是[0,255]！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtClassificationThresholdMax_S3.Text) < Convert.ToSingle(txtClassificationThresholdMin_S3.Text))
            {
                MyTool.TxtFileProcess.CreateLog("控件txtClassificationThresholdMax_S3的数据不能小于控件txtClassificationThresholdMin_S3的数据！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtClassificationThresholdMax_S3的数据不能小于控件txtClassificationThresholdMin_S3的数据！");
                dataCheckResult = false;
            }

            if (Convert.ToSingle(txtProductAreaMin_S3.Text) < 0 || Convert.ToSingle(txtProductAreaMin_S3.Text) > 100000000)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtProductAreaMin_S3的数据范围错误,取值范围是[0,100000000]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtProductAreaMin_S3的数据范围错误,取值范围是[0,100000000]！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtProductAreaMax_S3.Text) < 0 || Convert.ToSingle(txtProductAreaMax_S3.Text) > 100000000)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtProductAreaMax_S3的数据范围错误,取值范围是[0,100000000]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtProductAreaMax_S3的数据范围错误,取值范围是[0,100000000]！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtProductAreaMax_S3.Text) < Convert.ToSingle(txtProductAreaMin_S3.Text))
            {
                MyTool.TxtFileProcess.CreateLog("控件txtProductAreaMax_S3的数据不能小于控件txtProductAreaMin_S3的数据！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtProductAreaMax_S3的数据不能小于控件txtProductAreaMin_S3的数据！");
                dataCheckResult = false;
            }

            if (Convert.ToSingle(txtCoverAreaMin_S3.Text) < 0 || Convert.ToSingle(txtCoverAreaMin_S3.Text) > 100000000)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtProductAreaMin_S3的数据范围错误,取值范围是[0,100000000]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtProductAreaMin_S3的数据范围错误,取值范围是[0,100000000]！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtCoverAreaMax_S3.Text) < 0 || Convert.ToSingle(txtCoverAreaMax_S3.Text) > 100000000)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtProductAreaMax_S3的数据范围错误,取值范围是[0,100000000]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtProductAreaMax_S3的数据范围错误,取值范围是[0,100000000]！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtCoverAreaMax_S3.Text) < Convert.ToSingle(txtCoverAreaMin_S3.Text))
            {
                MyTool.TxtFileProcess.CreateLog("控件txtCoverAreaMax_S3的数据不能小于控件txtCoverAreaMin_S3的数据！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtCoverAreaMax_S3的数据不能小于控件txtCoverAreaMin_S3的数据！");
                dataCheckResult = false;
            }

            if (Convert.ToSingle(txtEmptyAreaMin_S3.Text) < 0 || Convert.ToSingle(txtEmptyAreaMin_S3.Text) > 100000000)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtEmptyAreaMin_S3的数据范围错误,取值范围是[0,100000000]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtEmptyAreaMin_S3的数据范围错误,取值范围是[0,100000000]！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtEmptyAreaMax_S3.Text) < 0 || Convert.ToSingle(txtEmptyAreaMax_S3.Text) > 100000000)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtProductAreaMax_S3的数据范围错误,取值范围是[0,100000000]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtProductAreaMax_S3的数据范围错误,取值范围是[0,100000000]！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtEmptyAreaMax_S3.Text) < Convert.ToSingle(txtEmptyAreaMin_S3.Text))
            {
                MyTool.TxtFileProcess.CreateLog("控件txtEmptyAreaMax_S3的数据不能小于控件txtEmptyAreaMin_S3的数据！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtEmptyAreaMax_S3的数据不能小于控件txtEmptyAreaMin_S3的数据！");
                dataCheckResult = false;
            }

            if (Convert.ToSingle(txtClassificationThresholdCoverMin_S3.Text) < 0 || Convert.ToSingle(txtClassificationThresholdCoverMin_S3.Text) > 255)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtClassificationThresholdCoverMin_S3的数据范围错误,取值范围是[0,255]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtClassificationThresholdCoverMin_S3的数据范围错误,取值范围是[0,255]！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtClassificationThresholdCoverMax_S3.Text) < 0 || Convert.ToSingle(txtClassificationThresholdCoverMax_S3.Text) > 255)
            {
                MyTool.TxtFileProcess.CreateLog("控件txtClassificationThresholdCoverMax_S3的数据范围错误,取值范围是[0,255]！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtClassificationThresholdCoverMax_S3的数据范围错误,取值范围是[0,255]！");
                dataCheckResult = false;
            }
            if (Convert.ToSingle(txtClassificationThresholdCoverMax_S3.Text) < Convert.ToSingle(txtClassificationThresholdCoverMin_S3.Text))
            {
                MyTool.TxtFileProcess.CreateLog("控件txtClassificationThresholdCoverMax_S3的数据不能小于控件txtClassificationThresholdCoverMin_S3的数据！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("控件txtClassificationThresholdCoverMax_S3的数据不能小于控件txtClassificationThresholdCoverMin_S3的数据！");
                dataCheckResult = false;
            }

            return dataCheckResult;
        }
        #endregion

        #region InitialCtrlBoxCorrespondingStringArr:初始化控件名和xml文件保存名称对应表----产品参数
        void InitialCtrlBoxCorrespondingStringArr()
        {
            MyTool.TxtFileProcess.CreateLog("产品设置-初始化控件名和变量名对应表！" + "----用户：" + ActionForm._operatorName,
            ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);

            #region 控件名称

            _ctrlNameAndXmlFileNameStr[0, 0] = "txtROB_XPickupTeachPosition_S1";
            _ctrlNameAndXmlFileNameStr[0, 1] = "txtROB_YPickupTeachPosition_S1";
            _ctrlNameAndXmlFileNameStr[0, 2] = "txtROB_ZPickupTeachPosition_S1";
            _ctrlNameAndXmlFileNameStr[0, 3] = "txtROB_RXPickupTeachPosition_S1";
            _ctrlNameAndXmlFileNameStr[0, 4] = "txtROB_RWPickupTeachPosition_S1";
            _ctrlNameAndXmlFileNameStr[0, 5] = "txtROB_RZPickupTeachPosition_S1";
            _ctrlNameAndXmlFileNameStr[0, 6] = "txtPixel_XPickupTeachPosition_RT_S1";
            _ctrlNameAndXmlFileNameStr[0, 7] = "txtPixel_YPickupTeachPosition_RT_S1";
            _ctrlNameAndXmlFileNameStr[0, 8] = "txtPixel_XPickupTeachPosition_LT_S1";
            _ctrlNameAndXmlFileNameStr[0, 9] = "txtPixel_YPickupTeachPosition_LT_S1";
            _ctrlNameAndXmlFileNameStr[0, 10] = "txtPixel_XPickupTeachPosition_LB_S1";
            _ctrlNameAndXmlFileNameStr[0, 11] = "txtPixel_YPickupTeachPosition_LB_S1";
            _ctrlNameAndXmlFileNameStr[0, 12] = "txtPixel_XPickupTeachPosition_RB_S1";
            _ctrlNameAndXmlFileNameStr[0, 13] = "txtPixel_YPickupTeachPosition_RB_S1";
            _ctrlNameAndXmlFileNameStr[0, 14] = "txtPixel_AnglePickupTeach_S1";

            _ctrlNameAndXmlFileNameStr[0, 15] = "txtPixel_XPickupCurrentPositionLimitMin_RT_S1";
            _ctrlNameAndXmlFileNameStr[0, 16] = "txtPixel_XPickupCurrentPositionLimitMax_RT_S1";
            _ctrlNameAndXmlFileNameStr[0, 17] = "txtPixel_YPickupCurrentPositionLimitMin_RT_S1";
            _ctrlNameAndXmlFileNameStr[0, 18] = "txtPixel_YPickupCurrentPositionLimitMax_RT_S1";
            _ctrlNameAndXmlFileNameStr[0, 19] = "txtPixel_AnglePickupCurrentLimitMin_S1";
            _ctrlNameAndXmlFileNameStr[0, 20] = "txtPixel_AnglePickupCurrentLimitMax_S1";
            _ctrlNameAndXmlFileNameStr[0, 21] = "txtROB_XPickupCurrentPositionLimitMin_S1";
            _ctrlNameAndXmlFileNameStr[0, 22] = "txtROB_XPickupCurrentPositionLimitMax_S1";
            _ctrlNameAndXmlFileNameStr[0, 23] = "txtROB_YPickupCurrentPositionLimitMin_S1";
            _ctrlNameAndXmlFileNameStr[0, 24] = "txtROB_YPickupCurrentPositionLimitMax_S1";
            _ctrlNameAndXmlFileNameStr[0, 25] = "txtROB_RZPickupCurrentPositionLimitMin_S1";
            _ctrlNameAndXmlFileNameStr[0, 26] = "txtROB_RZPickupCurrentPositionLimitMax_S1";

            _ctrlNameAndXmlFileNameStr[0, 27] = "txtThresholdMinGray_S1";
            _ctrlNameAndXmlFileNameStr[0, 28] = "txtThresholdMaxGray_S1";
            _ctrlNameAndXmlFileNameStr[0, 29] = "txtOpeningWAndH_S1";
            _ctrlNameAndXmlFileNameStr[0, 30] = "txtShapeMinArea_S1";
            _ctrlNameAndXmlFileNameStr[0, 31] = "txtShapeMaxArea_S1";
            _ctrlNameAndXmlFileNameStr[0, 32] = "txtShapeMinRectangularity_S1";
            _ctrlNameAndXmlFileNameStr[0, 33] = "txtShapeMaxRectangularity_S1";
            _ctrlNameAndXmlFileNameStr[0, 34] = "txtDilationRectangleWidth_S1";
            _ctrlNameAndXmlFileNameStr[0, 35] = "txtDilationRectangleHeight_S1";
            _ctrlNameAndXmlFileNameStr[0, 36] = "txtEdgesSubPixCannyAlpha_S1";
            _ctrlNameAndXmlFileNameStr[0, 37] = "txtEdgesSubPixCannyLow_S1";
            _ctrlNameAndXmlFileNameStr[0, 38] = "txtEdgesSubPixCannyHigh_S1";
            _ctrlNameAndXmlFileNameStr[0, 39] = "txtUnionCollinearContoursXldMaxDistAbs_S1";
            _ctrlNameAndXmlFileNameStr[0, 40] = "txtUnionCollinearContoursXldMaxDistRel_S1";
            _ctrlNameAndXmlFileNameStr[0, 41] = "txtUnionCollinearContoursXldMaxShift_S1";
            _ctrlNameAndXmlFileNameStr[0, 42] = "txtUnionCollinearContoursXldMaxAngle_S1";
            _ctrlNameAndXmlFileNameStr[0, 43] = "txtXldContlengthMin_S1";
            _ctrlNameAndXmlFileNameStr[0, 44] = "txtXldContlengthMax_S1";
            _ctrlNameAndXmlFileNameStr[0, 45] = "txtXLDMinArea_S1";
            _ctrlNameAndXmlFileNameStr[0, 46] = "txtXLDMaxArea_S1";
            _ctrlNameAndXmlFileNameStr[0, 47] = "txtXLDMinRectangularity_S1";
            _ctrlNameAndXmlFileNameStr[0, 48] = "txtXLDMaxRectangularity_S1";
            _ctrlNameAndXmlFileNameStr[0, 49] = "txtUnionAdjacentContoursXldMaxDistAbs_S1";
            _ctrlNameAndXmlFileNameStr[0, 50] = "txtUnionAdjacentContoursXldMaxDistRel_S1";
            //Station2
            _ctrlNameAndXmlFileNameStr[0, 51] = "txtROB_XPickupTeachPosition_S2";
            _ctrlNameAndXmlFileNameStr[0, 52] = "txtROB_YPickupTeachPosition_S2";
            _ctrlNameAndXmlFileNameStr[0, 53] = "txtROB_ZPickupTeachPosition_S2";
            _ctrlNameAndXmlFileNameStr[0, 54] = "txtROB_RXPickupTeachPosition_S2";
            _ctrlNameAndXmlFileNameStr[0, 55] = "txtROB_RWPickupTeachPosition_S2";
            _ctrlNameAndXmlFileNameStr[0, 56] = "txtROB_RZPickupTeachPosition_S2";
            _ctrlNameAndXmlFileNameStr[0, 57] = "txtPixel_XPickupTeachPosition_RT_S2";
            _ctrlNameAndXmlFileNameStr[0, 58] = "txtPixel_YPickupTeachPosition_RT_S2";
            _ctrlNameAndXmlFileNameStr[0, 59] = "txtPixel_XPickupTeachPosition_LT_S2";
            _ctrlNameAndXmlFileNameStr[0, 60] = "txtPixel_YPickupTeachPosition_LT_S2";
            _ctrlNameAndXmlFileNameStr[0, 61] = "txtPixel_XPickupTeachPosition_LB_S2";
            _ctrlNameAndXmlFileNameStr[0, 62] = "txtPixel_YPickupTeachPosition_LB_S2";
            _ctrlNameAndXmlFileNameStr[0, 63] = "txtPixel_XPickupTeachPosition_RB_S2";
            _ctrlNameAndXmlFileNameStr[0, 64] = "txtPixel_YPickupTeachPosition_RB_S2";
            _ctrlNameAndXmlFileNameStr[0, 65] = "txtPixel_AnglePickupTeach_S2";

            _ctrlNameAndXmlFileNameStr[0, 66] = "txtPixel_XPickupCurrentPositionLimitMin_RT_S2";
            _ctrlNameAndXmlFileNameStr[0, 67] = "txtPixel_XPickupCurrentPositionLimitMax_RT_S2";
            _ctrlNameAndXmlFileNameStr[0, 68] = "txtPixel_YPickupCurrentPositionLimitMin_RT_S2";
            _ctrlNameAndXmlFileNameStr[0, 69] = "txtPixel_YPickupCurrentPositionLimitMax_RT_S2";
            _ctrlNameAndXmlFileNameStr[0, 70] = "txtPixel_AnglePickupCurrentLimitMin_S2";
            _ctrlNameAndXmlFileNameStr[0, 71] = "txtPixel_AnglePickupCurrentLimitMax_S2";
            _ctrlNameAndXmlFileNameStr[0, 72] = "txtROB_XPickupCurrentPositionLimitMin_S2";
            _ctrlNameAndXmlFileNameStr[0, 73] = "txtROB_XPickupCurrentPositionLimitMax_S2";
            _ctrlNameAndXmlFileNameStr[0, 74] = "txtROB_YPickupCurrentPositionLimitMin_S2";
            _ctrlNameAndXmlFileNameStr[0, 75] = "txtROB_YPickupCurrentPositionLimitMax_S2";
            _ctrlNameAndXmlFileNameStr[0, 76] = "txtROB_RZPickupCurrentPositionLimitMin_S2";
            _ctrlNameAndXmlFileNameStr[0, 77] = "txtROB_RZPickupCurrentPositionLimitMax_S2";

            _ctrlNameAndXmlFileNameStr[0, 78] = "txtThresholdMinGray_S2";
            _ctrlNameAndXmlFileNameStr[0, 79] = "txtThresholdMaxGray_S2";
            _ctrlNameAndXmlFileNameStr[0, 80] = "txtOpeningWAndH_S2";
            _ctrlNameAndXmlFileNameStr[0, 81] = "txtShapeMinArea_S2";
            _ctrlNameAndXmlFileNameStr[0, 82] = "txtShapeMaxArea_S2";
            _ctrlNameAndXmlFileNameStr[0, 83] = "txtShapeMinRectangularity_S2";
            _ctrlNameAndXmlFileNameStr[0, 84] = "txtShapeMaxRectangularity_S2";
            _ctrlNameAndXmlFileNameStr[0, 85] = "txtDilationRectangleWidth_S2";
            _ctrlNameAndXmlFileNameStr[0, 86] = "txtDilationRectangleHeight_S2";
            _ctrlNameAndXmlFileNameStr[0, 87] = "txtEdgesSubPixCannyAlpha_S2";
            _ctrlNameAndXmlFileNameStr[0, 88] = "txtEdgesSubPixCannyLow_S2";
            _ctrlNameAndXmlFileNameStr[0, 89] = "txtEdgesSubPixCannyHigh_S2";
            _ctrlNameAndXmlFileNameStr[0, 90] = "txtUnionCollinearContoursXldMaxDistAbs_S2";
            _ctrlNameAndXmlFileNameStr[0, 91] = "txtUnionCollinearContoursXldMaxDistRel_S2";
            _ctrlNameAndXmlFileNameStr[0, 92] = "txtUnionCollinearContoursXldMaxShift_S2";
            _ctrlNameAndXmlFileNameStr[0, 93] = "txtUnionCollinearContoursXldMaxAngle_S2";
            _ctrlNameAndXmlFileNameStr[0, 94] = "txtXldContlengthMin_S2";
            _ctrlNameAndXmlFileNameStr[0, 95] = "txtXldContlengthMax_S2";
            _ctrlNameAndXmlFileNameStr[0, 96] = "txtXLDMinArea_S2";
            _ctrlNameAndXmlFileNameStr[0, 97] = "txtXLDMaxArea_S2";
            _ctrlNameAndXmlFileNameStr[0, 98] = "txtXLDMinRectangularity_S2";
            _ctrlNameAndXmlFileNameStr[0, 99] = "txtXLDMaxRectangularity_S2";
            _ctrlNameAndXmlFileNameStr[0, 100] = "txtUnionAdjacentContoursXldMaxDistAbs_S2";
            _ctrlNameAndXmlFileNameStr[0, 101] = "txtUnionAdjacentContoursXldMaxDistRel_S2";
            //Station3
            _ctrlNameAndXmlFileNameStr[0, 102] = "txtROB_XPickupTeachPosition_S3";
            _ctrlNameAndXmlFileNameStr[0, 103] = "txtROB_YPickupTeachPosition_S3";
            _ctrlNameAndXmlFileNameStr[0, 104] = "txtROB_ZPickupTeachPosition_S3";
            _ctrlNameAndXmlFileNameStr[0, 105] = "txtROB_RXPickupTeachPosition_S3";
            _ctrlNameAndXmlFileNameStr[0, 106] = "txtROB_RWPickupTeachPosition_S3";
            _ctrlNameAndXmlFileNameStr[0, 107] = "txtROB_RZPickupTeachPosition_S3";
            _ctrlNameAndXmlFileNameStr[0, 108] = "txtPixel_XPickupTeachPosition_RT_S3";
            _ctrlNameAndXmlFileNameStr[0, 109] = "txtPixel_YPickupTeachPosition_RT_S3";
            _ctrlNameAndXmlFileNameStr[0, 110] = "txtPixel_XPickupTeachPosition_LT_S3";
            _ctrlNameAndXmlFileNameStr[0, 111] = "txtPixel_YPickupTeachPosition_LT_S3";
            _ctrlNameAndXmlFileNameStr[0, 112] = "txtPixel_XPickupTeachPosition_LB_S3";
            _ctrlNameAndXmlFileNameStr[0, 113] = "txtPixel_YPickupTeachPosition_LB_S3";
            _ctrlNameAndXmlFileNameStr[0, 114] = "txtPixel_XPickupTeachPosition_RB_S3";
            _ctrlNameAndXmlFileNameStr[0, 115] = "txtPixel_YPickupTeachPosition_RB_S3";
            _ctrlNameAndXmlFileNameStr[0, 116] = "txtPixel_AnglePickupTeach_S3";

            _ctrlNameAndXmlFileNameStr[0, 117] = "txtPixel_XPickupCurrentPositionLimitMin_RT_S3";
            _ctrlNameAndXmlFileNameStr[0, 118] = "txtPixel_XPickupCurrentPositionLimitMax_RT_S3";
            _ctrlNameAndXmlFileNameStr[0, 119] = "txtPixel_YPickupCurrentPositionLimitMin_RT_S3";
            _ctrlNameAndXmlFileNameStr[0, 120] = "txtPixel_YPickupCurrentPositionLimitMax_RT_S3";
            _ctrlNameAndXmlFileNameStr[0, 121] = "txtPixel_AnglePickupCurrentLimitMin_S3";
            _ctrlNameAndXmlFileNameStr[0, 122] = "txtPixel_AnglePickupCurrentLimitMax_S3";
            _ctrlNameAndXmlFileNameStr[0, 123] = "txtROB_XPickupCurrentPositionLimitMin_S3";
            _ctrlNameAndXmlFileNameStr[0, 124] = "txtROB_XPickupCurrentPositionLimitMax_S3";
            _ctrlNameAndXmlFileNameStr[0, 125] = "txtROB_YPickupCurrentPositionLimitMin_S3";
            _ctrlNameAndXmlFileNameStr[0, 126] = "txtROB_YPickupCurrentPositionLimitMax_S3";
            _ctrlNameAndXmlFileNameStr[0, 127] = "txtROB_RZPickupCurrentPositionLimitMin_S3";
            _ctrlNameAndXmlFileNameStr[0, 128] = "txtROB_RZPickupCurrentPositionLimitMax_S3";

            _ctrlNameAndXmlFileNameStr[0, 129] = "txtThresholdMinGray_S3";
            _ctrlNameAndXmlFileNameStr[0, 130] = "txtThresholdMaxGray_S3";
            _ctrlNameAndXmlFileNameStr[0, 131] = "txtOpeningWAndH_S3";
            _ctrlNameAndXmlFileNameStr[0, 132] = "txtShapeMinArea_S3";
            _ctrlNameAndXmlFileNameStr[0, 133] = "txtShapeMaxArea_S3";
            _ctrlNameAndXmlFileNameStr[0, 134] = "txtShapeMinRectangularity_S3";
            _ctrlNameAndXmlFileNameStr[0, 135] = "txtShapeMaxRectangularity_S3";
            _ctrlNameAndXmlFileNameStr[0, 136] = "txtDilationRectangleWidth_S3";
            _ctrlNameAndXmlFileNameStr[0, 137] = "txtDilationRectangleHeight_S3";
            _ctrlNameAndXmlFileNameStr[0, 138] = "txtEdgesSubPixCannyAlpha_S3";
            _ctrlNameAndXmlFileNameStr[0, 139] = "txtEdgesSubPixCannyLow_S3";
            _ctrlNameAndXmlFileNameStr[0, 140] = "txtEdgesSubPixCannyHigh_S3";
            _ctrlNameAndXmlFileNameStr[0, 141] = "txtUnionCollinearContoursXldMaxDistAbs_S3";
            _ctrlNameAndXmlFileNameStr[0, 142] = "txtUnionCollinearContoursXldMaxDistRel_S3";
            _ctrlNameAndXmlFileNameStr[0, 143] = "txtUnionCollinearContoursXldMaxShift_S3";
            _ctrlNameAndXmlFileNameStr[0, 144] = "txtUnionCollinearContoursXldMaxAngle_S3";
            _ctrlNameAndXmlFileNameStr[0, 145] = "txtXldContlengthMin_S3";
            _ctrlNameAndXmlFileNameStr[0, 146] = "txtXldContlengthMax_S3";
            _ctrlNameAndXmlFileNameStr[0, 147] = "txtXLDMinArea_S3";
            _ctrlNameAndXmlFileNameStr[0, 148] = "txtXLDMaxArea_S3";
            _ctrlNameAndXmlFileNameStr[0, 149] = "txtXLDMinRectangularity_S3";
            _ctrlNameAndXmlFileNameStr[0, 150] = "txtXLDMaxRectangularity_S3";
            _ctrlNameAndXmlFileNameStr[0, 151] = "txtUnionAdjacentContoursXldMaxDistAbs_S3";
            _ctrlNameAndXmlFileNameStr[0, 152] = "txtUnionAdjacentContoursXldMaxDistRel_S3";

            _ctrlNameAndXmlFileNameStr[0, 153] = "txtClassificationThresholdMin_S3";
            _ctrlNameAndXmlFileNameStr[0, 154] = "txtClassificationThresholdMax_S3";
            _ctrlNameAndXmlFileNameStr[0, 155] = "txtProductAreaMin_S3";
            _ctrlNameAndXmlFileNameStr[0, 156] = "txtProductAreaMax_S3";
            _ctrlNameAndXmlFileNameStr[0, 157] = "txtCoverAreaMin_S3";
            _ctrlNameAndXmlFileNameStr[0, 158] = "txtCoverAreaMax_S3";
            _ctrlNameAndXmlFileNameStr[0, 159] = "txtEmptyAreaMin_S3";
            _ctrlNameAndXmlFileNameStr[0, 160] = "txtEmptyAreaMax_S3";
            _ctrlNameAndXmlFileNameStr[0, 161] = "txtClassificationThresholdCoverMin_S3";
            _ctrlNameAndXmlFileNameStr[0, 162] = "txtClassificationThresholdCoverMax_S3";

            _ctrlNameAndXmlFileNameStr[0, 163] = "chkAngleRotate_S1";
            _ctrlNameAndXmlFileNameStr[0, 164] = "chkAngleRotate_S2";
            _ctrlNameAndXmlFileNameStr[0, 165] = "chkAngleRotate_S3";
            #endregion

            #region 在XML中保存读取的参数名称

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
        }

        #endregion

        //点击读取参数按钮事件
        private void btnLoadProductData_Click(object sender, EventArgs e)
        {
            MyTool.TxtFileProcess.CreateLog("产品设置页面-点击产品数据读取按钮！" + "----用户：" + ActionForm._operatorName,
            ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);

            if (CheckCurrentProductUsefulOrNot(ref _editProductName, true))
            {
                _editProductRecipe._parameterLoadSuccessfulFlag = false;
                if (ProductDataLoadingToCtrl(_editProductName))
                {
                    MyTool.TxtFileProcess.CreateLog("产品设置页面-读取产品" + _editProductName + "数据成功！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    //MessageBox.Show("读取数据成功！", "提示", MessageBoxButtons.OK);----因为增加了读取通用参数,所以放到最后执行

                    //读取完参数到控件之后，读取到编辑产品变量中来
                    _editProductRecipe.ProductDataLoadToVariate(_ProductParameterDirectoryPath + "\\" + _editProductName + ".rcp");
                }

            }
            else
            {
                MyTool.TxtFileProcess.CreateLog("产品设置页面-无法在指定文件夹中找到当前设定的产品" + _editProductName + "的数据，读取产品数据失败！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("无法在指定文件夹中找到当前设定的产品的数据，无法读取参数！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            _generalParameterLoadingToCtrl();//读取通用参数到控件的异步函数
            MessageBox.Show("读取数据成功！", "提示", MessageBoxButtons.OK);
        }

        //点击保存参数按钮事件
        private void btnSaveProductData_Click(object sender, EventArgs e)
        {
            if (ActionForm._accessLevel < 101)
            {
                MyTool.TxtFileProcess.CreateLog("操作权限不合格:操作权限为工程师或者厂家!" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("操作权限不合格:操作权限为工程师或者厂家!");
                return;
            }

            MyTool.TxtFileProcess.CreateLog("产品设置页面-点击产品数据保存按钮！" + "----用户：" + ActionForm._operatorName,
             ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);

            //if ((_editProductName == _track1ActualsProductName || _editProductName == _track2ActualsProductName) && ActionForm._autoModeFlag)
            //{
            //    MessageBox.Show("当前设备处于自动模式，无法保存当前使用的产品的参数！");
            //    return;
            //}

            if (CheckCurrentProductUsefulOrNot(ref _editProductName, true))
            {
                _editProductRecipe._parameterLoadSuccessfulFlag = false;
                if (ProductDataSavingFromCtrl(_editProductName, true))
                {
                    MyTool.TxtFileProcess.CreateLog("产品设置页面-产品" + _editProductName + "数据保存成功！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("保存数据成功！", "提示", MessageBoxButtons.OK);

                    //保存完参数到控件之后，读取到编辑产品变量中来
                    _editProductRecipe.ProductDataLoadToVariate(_ProductParameterDirectoryPath + "\\" + _editProductName + ".rcp");
                }
                else
                {
                    MyTool.TxtFileProcess.CreateLog("产品设置页面-产品" + _editProductName + "数据保存失败！" + "----用户：" + ActionForm._operatorName,
                     ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("保存数据失败！", "提示", MessageBoxButtons.OK);
                }
            }
            else
            {
                MyTool.TxtFileProcess.CreateLog("产品设置页面-无法在指定文件夹中找到当前设定的产品的数据，产品" + _editProductName + "数据保存失败！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("无法在指定文件夹中找到当前设定的产品的数据，无法保存参数！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region 创建产品

        #region 新建
        private void btnCreateNewProduct_Click(object sender, EventArgs e)
        {
            MyTool.TxtFileProcess.CreateLog("参数设置-创建新产品数据文件！" + "----用户：" + ActionForm._operatorName,
            ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);

            this.Invoke(new Action(() =>
            {
                if (_createNewProductFormVariate != null)
                {
                    _createNewProductFormVariate.Dispose();
                    _createNewProductFormVariate = null;
                }
                _createNewProductFormVariate = new CreateNewProductDataFileForm("创建新产品", "产品名？", "新产品");
                _createNewProductFormVariate.Focus();
                _createNewProductFormVariate.StartPosition = FormStartPosition.CenterParent;
                _createNewProductFormVariate.SenderName += GetNewName;
                _createNewProductFormVariate.CloseRequest += new CloseRequestDelegate(CloseRequest);
                _createNewProductFormVariate.Show();
            }));
        }
        private void GetNewName(string arg)
        {
            string newProductName = arg.ToString();
            if (!CheckCurrentProductUsefulOrNot(ref newProductName, false))
            {
                if (ProductDataSavingFromCtrl(newProductName, false))
                {
                    MyTool.TxtFileProcess.CreateLog("参数设置-创建新产品" + newProductName + "数据文件成功！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("创建新产品完成！", "提示", MessageBoxButtons.OK);
                }
            }
            else
            {
                MyTool.TxtFileProcess.CreateLog("参数设置-创建新产品" + newProductName + "数据失败，当前产品路径中已存在此名称产品数据文件！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("产品" + newProductName + "已经存在！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            //关闭窗口
            if (_createNewProductFormVariate != null)
            {
                _createNewProductFormVariate.Dispose();
                _createNewProductFormVariate = null;
            }

            _editProductName = newProductName;
            //基于EDIT product name设置combo box
            cboAvaliableProduct_MouseClick(new object(), new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0));
            cboAvaliableProduct.SelectedIndex = cboAvaliableProduct.FindString(_editProductName);
        }
        private void CloseRequest()
        {
            MyTool.TxtFileProcess.CreateLog("参数设置-放弃创建新产品！" + "----用户：" + ActionForm._operatorName,
            ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
            MessageBox.Show("放弃创建新的产品！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            //关闭窗口
            if (_createNewProductFormVariate != null)
            {
                _createNewProductFormVariate.Dispose();
                _createNewProductFormVariate = null;
            }
        }

        #endregion

        #region 另存为
        private void btnSaveProductAs_Click(object sender, EventArgs e)
        {
            if (ActionForm._accessLevel < 101)
            {
                MyTool.TxtFileProcess.CreateLog("操作权限不合格:操作权限为工程师或者厂家!" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("操作权限不合格:操作权限为工程师或者厂家!");
                return;
            }

            MyTool.TxtFileProcess.CreateLog("参数设置-产品数据另存为！" + "----用户：" + ActionForm._operatorName,
            ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);

            this.Invoke(new Action(() =>
            {
                if (_createNewProductFormVariate != null)
                {
                    _createNewProductFormVariate.Dispose();
                    _createNewProductFormVariate = null;
                }
                _createNewProductFormVariate = new CreateNewProductDataFileForm("产品另存为", "另存为名字？", "新产品");
                _createNewProductFormVariate.Focus();
                _createNewProductFormVariate.StartPosition = FormStartPosition.CenterParent;
                _createNewProductFormVariate.SenderName += GetSaveAsFileName;
                _createNewProductFormVariate.CloseRequest += new CloseRequestDelegate(ProductSaveAsCloseRequest);
                _createNewProductFormVariate.Show();
            })); ;
        }

        private void GetSaveAsFileName(string arg)
        {
            string saveAsProductName = arg.ToString();
            if (!CheckCurrentProductUsefulOrNot(ref saveAsProductName, false))
            {
                if (ProductDataSavingFromCtrl(saveAsProductName, true))
                {
                    MyTool.TxtFileProcess.CreateLog("参数设置-产品另存为" + _editProductName + "数据另存为" + saveAsProductName + "成功！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("产品另存为完成！", "提示", MessageBoxButtons.OK);
                }
                else
                {
                    MyTool.TxtFileProcess.CreateLog("产品设置页面-有非法数据存在，产品" + _editProductName + "数据另存为" + saveAsProductName + "失败！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("产品另存为失败！", "提示", MessageBoxButtons.OK);
                }
            }
            else
            {
                MyTool.TxtFileProcess.CreateLog("参数设置-产品另存为" + _editProductName + "数据另存为" + saveAsProductName + "失败，产品" + saveAsProductName + "已存在！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("产品" + saveAsProductName + "已经存在！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            //关闭窗口
            if (_createNewProductFormVariate != null)
            {
                _createNewProductFormVariate.Dispose();
                _createNewProductFormVariate = null;
            }

            ////轴点位数据另存为
            //string axisParAndPositionFileFolder = Directory.GetCurrentDirectory() + "\\" + "AxisParameterAndPositionData_" + saveAsProductName;
            //string oldAxisParAndPositionFileFolder = Directory.GetCurrentDirectory() + "\\" + "AxisParameterAndPositionData_" + _editProductName;

            //if (!Directory.Exists(axisParAndPositionFileFolder))
            //{
            //    Directory.CreateDirectory(axisParAndPositionFileFolder);
            //}

            //try
            //{
            //    for (int i = 0; i < AxisControlMainForm._axisQuantity; i++)
            //    {
            //        File.Copy(oldAxisParAndPositionFileFolder + "\\" + "axis" + i.ToString("00") + "Pos_" + _editProductName + ".csv"
            //            , axisParAndPositionFileFolder + "\\" + "axis" + i.ToString("00") + "Pos_" + saveAsProductName + ".csv");
            //    }
            //    File.Copy(oldAxisParAndPositionFileFolder + "\\" + "AxisPar.csv", axisParAndPositionFileFolder + "\\" + "AxisPar.csv");
            //}
            //catch (Exception e)
            //{
            //    MessageBox.Show(e.ToString());
            //}

            _editProductName = saveAsProductName;
            //基于EDIT product name设置combo box
            cboAvaliableProduct_MouseClick(new object(), new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0));
            cboAvaliableProduct.SelectedIndex = cboAvaliableProduct.FindString(_editProductName);
        }

        private void ProductSaveAsCloseRequest()
        {
            MyTool.TxtFileProcess.CreateLog("参数设置-放弃产品另存为" + _editProductName + "数据的另存为！" + "----用户：" + ActionForm._operatorName,
            ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
            MessageBox.Show("放弃产品另存为！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            //关闭窗口
            if (_createNewProductFormVariate != null)
            {
                _createNewProductFormVariate.Dispose();
                _createNewProductFormVariate = null;
            }
        }

        #endregion 

        #region CheckCurrentProductUsefulOrNot:检测当前名称的产品数据文件是否存在
        /// <summary>
        /// 检测当前名称的产品数据文件是否存在
        /// </summary>
        /// <param name="ProductName">产品名称</param>
        /// <param name="EmptyFlag">不存在的话情况与否标签，true-清空，false-不清空</param>
        /// <returns>返回存不存在，true-存在，false-不存在</returns>
        public bool CheckCurrentProductUsefulOrNot(ref string ProductName, bool EmptyFlag)
        {
            MyTool.TxtFileProcess.CreateLog("参数设置-检查当前产品是否可用！" + "----用户：" + ActionForm._operatorName,
            ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);

            bool currentProductUsefulFlag = false;
            string[] fileName;
            if (Directory.Exists(_ProductParameterDirectoryPath))
            {
                fileName = MyTool.FolderAndFileManageClass.ScanFolderAndGetAssignedFormatFileName(_ProductParameterDirectoryPath, ".rcp");
                for (int i = 0; i < fileName.Length; i++)
                {
                    if (ProductName == fileName[i])
                    {
                        currentProductUsefulFlag = true;
                        break;
                    }
                }
            }

            if (!currentProductUsefulFlag && EmptyFlag)
                ProductName = "";

            return currentProductUsefulFlag;
        }
        #endregion

        #endregion

        #region comboBoxEvent
        public void cboAvaliableProduct_MouseClick(object sender, MouseEventArgs e)
        {
            string[] fileName;
            if (Directory.Exists(_ProductParameterDirectoryPath))
            {
                fileName = MyTool.FolderAndFileManageClass.ScanFolderAndGetAssignedFormatFileName(_ProductParameterDirectoryPath, ".rcp");

                cboAvaliableProduct.Items.Clear();
                for (int i = 0; i < fileName.Length; i++)
                {
                    cboAvaliableProduct.Items.Add(fileName[i]);
                }
            }
        }

        public void cboAvaliableProduct_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.Visible)//主要用于初始化的时候，不会弹出以下框
            {
                _editProductName = cboAvaliableProduct.SelectedItem.ToString();
                _editProductRecipe._parameterLoadSuccessfulFlag = false;
                if (ProductDataLoadingToCtrl(_editProductName))
                    _editProductRecipe.ProductDataLoadToVariate(_ProductParameterDirectoryPath + "\\" + _editProductName + ".rcp");

                MyTool.TxtFileProcess.CreateLog("参数设置-切换当前编辑产品为" + _editProductName + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                if (this.Visible)
                {
                    MessageBox.Show("切换编辑产品成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                if (!ActionForm._autoModeFlag)
                    _currentProductName = _editProductName;
            }
        }
        #endregion

        #region btnDirectoryBrowseEvent
        private void btnDirectoryBrowseEvent(object sender, EventArgs e)
        {
            MyTool.TxtFileProcess.CreateLog("参数设置-请求修改路径！" + "----用户：" + ActionForm._operatorName,
            ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);

            if (ActionForm._autoModeFlag)
            {
                MyTool.TxtFileProcess.CreateLog("参数设置-请求修改路径失败，设备当前处于自动模式！" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("当前设备处于自动模式，无法修改路径！");
                return;
            }
            FolderBrowserDialog fb = new FolderBrowserDialog();
            string name = ((Button)sender).Name;
            switch (name)
            {
                case "btnBrowseGeneralParameterDirectory":
                    {
                        fb.SelectedPath = txtGeneralParameterDirectory.Text;
                        break;
                    }
                case "btnBrowseProductParameterDirectory":
                    {
                        fb.SelectedPath = txtProductParameterDirectory.Text;
                        break;
                    }
                case "btnBrowseProductDataDirectory":
                    {
                        fb.SelectedPath = txtProductDataDirectory.Text;
                        break;
                    }
            }
            if (fb.ShowDialog() == DialogResult.OK)
            {
                switch (name)
                {

                    case "btnBrowseGeneralParameterDirectory":
                        {
                            MyTool.TxtFileProcess.CreateLog("参数设置-修改 通用参数保存路径 为" + fb.SelectedPath + "！" + "----用户：" + ActionForm._operatorName,
                            ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);

                            txtGeneralParameterDirectory.Text = fb.SelectedPath;
                            break;
                        }
                    case "btnBrowseProductParameterDirectory":
                        {
                            MyTool.TxtFileProcess.CreateLog("参数设置-修改 产品参数保存路径 为" + fb.SelectedPath + "！" + "----用户：" + ActionForm._operatorName,
                            ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);

                            txtProductParameterDirectory.Text = fb.SelectedPath;
                            break;
                        }
                    case "btnBrowseProductDataDirectory":
                        {
                            MyTool.TxtFileProcess.CreateLog("参数设置-修改 产品数据保存路径 为" + fb.SelectedPath + "！" + "----用户：" + ActionForm._operatorName,
                            ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);

                            txtProductDataDirectory.Text = fb.SelectedPath;
                            break;
                        }
                }
                SavingDataToXmlFile();
            }
        }
        #endregion

        #region RequetCameraOpenOrCloseEventFunc：请求相机打开或关闭事件函数
        /// <summary>
        /// RequetCameraOpenOrCloseEventFunc：请求相机打开或关闭事件函数
        /// </summary>
        private void RequetCameraOpenOrCloseEventFunc(object sender, EventArgs e)
        {
            string _contorlName = ((Button)sender).Name;
            int _stationNumber = Convert.ToInt32(_contorlName.Substring(_contorlName.Length - 1, 1));

            MyTool.TxtFileProcess.CreateLog("S" + _stationNumber.ToString("0") + "手动相机控制页面-请求开闭相机" + "----用户：" + ActionForm._operatorName,
            ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);

            if (ActionForm._autoModeFlag)
            {
                MyTool.TxtFileProcess.CreateLog("S" + _stationNumber.ToString("0") + "手动相机控制页面-请求开闭相机失败，当前设备处于自动模式" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("当前设备处于自动模式，无法进行手动控制！");
                return;
            }

            int cameraIndex = -1;
            cameraIndex = _stationNumber - 1;

            if (MaintenanceCameraFormRequestOpenOrCloseCameraEvent != null)
            {
                MaintenanceCameraFormRequestOpenOrCloseCameraEvent(cameraIndex);
            }
        }
        #endregion

        #region RequetCameraGrabImageAndDispEventFunc：请求相机拍照并显示事件函数
        /// <summary>
        /// RequetCameraGrabImageAndDispEventFunc：请求相机拍照并显示事件函数
        /// </summary>
        private void RequetCameraGrabImageAndDispEventFunc(object sender, EventArgs e)
        {
            if (ActionForm._accessLevel < 101)
            {
                MyTool.TxtFileProcess.CreateLog("操作权限不合格:操作权限为工程师或者厂家!" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("操作权限不合格:操作权限为工程师或者厂家!");
                return;
            }

            string _contorlName = ((Button)sender).Name;
            int _stationNumber = Convert.ToInt32(_contorlName.Substring(_contorlName.Length - 1, 1));

            MyTool.TxtFileProcess.CreateLog("S" + _stationNumber.ToString("0") + "手动相机控制页面-请求相机抓取并显示图像" + "----用户：" + ActionForm._operatorName,
            ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);

            if (ActionForm._autoModeFlag)
            {
                MyTool.TxtFileProcess.CreateLog("S" + _stationNumber.ToString("0") + "手动相机控制页面-请求相机抓取并显示图像失败，当前设备处于自动模式" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("当前设备处于自动模式，无法进行手动控制！");
                return;
            }

            if (MaintenanceCameraFormRequestGrabImageAndDispEvent != null)
            {
                if (_stationNumber == 1)
                {
                    if (ho_Image_S1 != null)
                    {
                        ho_Image_S1.Dispose();
                        ho_Image_S1 = null;
                    }
                    MaintenanceCameraFormRequestGrabImageAndDispEvent(0, false);
                }

                if (_stationNumber == 2)
                {
                    if (ho_Image_S2 != null)
                    {
                        ho_Image_S2.Dispose();
                        ho_Image_S2 = null;
                    }
                    MaintenanceCameraFormRequestGrabImageAndDispEvent(1, false);
                }

                if (_stationNumber == 3)
                {
                    if (ho_Image_S3 != null)
                    {
                        ho_Image_S3.Dispose();
                        ho_Image_S3 = null;
                    }
                    MaintenanceCameraFormRequestGrabImageAndDispEvent(2, false);
                }
            }
        }
        #endregion

        #region RequetCameraGrabImageAndAndDispVideoFunc：请求相机拍照并显示视频事件函数
        /// <summary>
        /// RequetCameraGrabImageAndAndDispVideoFunc：请求相机拍照并显示视频事件函数
        /// </summary>
        private void RequetCameraGrabImageAndAndDispVideoFunc(object sender, EventArgs e)
        {
            string _contorlName = ((Button)sender).Name;
            int _stationNumber = Convert.ToInt32(_contorlName.Substring(_contorlName.Length - 1, 1));

            MyTool.TxtFileProcess.CreateLog("S" + _stationNumber.ToString("0") + "手动相机控制页面-请求相机显示实时图像" + "----用户：" + ActionForm._operatorName,
            ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);

            if (ActionForm._autoModeFlag)
            {
                MyTool.TxtFileProcess.CreateLog("S" + _stationNumber.ToString("0") + "手动相机控制页面-请求相机显示实时图像失败，当前设备处于自动模式" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("当前设备处于自动模式，无法进行手动控制！");
                return;
            }

            if (MaintenanceCameraFormRequestGrabImageAndDispVideoEvent != null)
            {
                MaintenanceCameraFormRequestGrabImageAndDispVideoEvent(_stationNumber - 1, false);
            }
        }
        #endregion

        #region RequestSaveImageEventFunc：请求保存图片事件函数
        /// <summary>
        /// RequestSaveImageEventFunc：请求保存图片事件函数
        /// </summary>
        private void RequestSaveImageEventFunc(object sender, EventArgs e)
        {
            string _contorlName = ((Button)sender).Name;
            int _stationNumber = Convert.ToInt32(_contorlName.Substring(_contorlName.Length - 1, 1));

            MyTool.TxtFileProcess.CreateLog("S" + _stationNumber.ToString("0") + "手动相机控制页面-请求保存图像" + "----用户：" + ActionForm._operatorName,
            ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);

            if (ActionForm._autoModeFlag)
            {
                MyTool.TxtFileProcess.CreateLog("S" + _stationNumber.ToString("0") + "手动相机控制页面-请求保存图像失败，当前设备处于自动模式" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("当前设备处于自动模式，无法进行手动控制！");
                return;
            }

            int cameraIndex = -1;
            cameraIndex = _stationNumber - 1;

            if (MaintenanceCameraFormRequestSaveImageEvent != null)
            {
                MaintenanceCameraFormRequestSaveImageEvent(cameraIndex);
            }
        }
        #endregion

        #region 标定相关
        #region btn9PCalibrateFindCircle_Click
        private void btn9PCalibrateFindCircle_S1_Click(object sender, EventArgs e)
        {
            if (ActionForm._accessLevel < 101)
            {
                MyTool.TxtFileProcess.CreateLog("操作权限不合格:操作权限为工程师或者厂家!" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("操作权限不合格:操作权限为工程师或者厂家!");
                return;
            }

            //拍照获取9点标定点中心坐标
            if (!_9PCalibrateFindCircle_S(1, out Pixel_X1_9PCalibrate_S1, out Pixel_Y1_9PCalibrate_S1, out Pixel_X2_9PCalibrate_S1, out Pixel_Y2_9PCalibrate_S1, out Pixel_X3_9PCalibrate_S1, out Pixel_Y3_9PCalibrate_S1,
                                           out Pixel_X4_9PCalibrate_S1, out Pixel_Y4_9PCalibrate_S1, out Pixel_X5_9PCalibrate_S1, out Pixel_Y5_9PCalibrate_S1, out Pixel_X6_9PCalibrate_S1, out Pixel_Y6_9PCalibrate_S1,
                                           out Pixel_X7_9PCalibrate_S1, out Pixel_Y7_9PCalibrate_S1, out Pixel_X8_9PCalibrate_S1, out Pixel_Y8_9PCalibrate_S1, out Pixel_X9_9PCalibrate_S1, out Pixel_Y9_9PCalibrate_S1))
            {
                MyTool.TxtFileProcess.CreateLog("S1获取9点标定点中心坐标失败!" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("S1获取9点标定点中心坐标失败!");
                return;
            }
            txtPixel_X9PTeachPosition_P1_S1.Text = Pixel_X1_9PCalibrate_S1.ToString("0000.000");
            txtPixel_Y9PTeachPosition_P1_S1.Text = Pixel_Y1_9PCalibrate_S1.ToString("0000.000");
            txtPixel_X9PTeachPosition_P2_S1.Text = Pixel_X2_9PCalibrate_S1.ToString("0000.000");
            txtPixel_Y9PTeachPosition_P2_S1.Text = Pixel_Y2_9PCalibrate_S1.ToString("0000.000");
            txtPixel_X9PTeachPosition_P3_S1.Text = Pixel_X3_9PCalibrate_S1.ToString("0000.000");
            txtPixel_Y9PTeachPosition_P3_S1.Text = Pixel_Y3_9PCalibrate_S1.ToString("0000.000");
            txtPixel_X9PTeachPosition_P4_S1.Text = Pixel_X4_9PCalibrate_S1.ToString("0000.000");
            txtPixel_Y9PTeachPosition_P4_S1.Text = Pixel_Y4_9PCalibrate_S1.ToString("0000.000");
            txtPixel_X9PTeachPosition_P5_S1.Text = Pixel_X5_9PCalibrate_S1.ToString("0000.000");
            txtPixel_Y9PTeachPosition_P5_S1.Text = Pixel_Y5_9PCalibrate_S1.ToString("0000.000");
            txtPixel_X9PTeachPosition_P6_S1.Text = Pixel_X6_9PCalibrate_S1.ToString("0000.000");
            txtPixel_Y9PTeachPosition_P6_S1.Text = Pixel_Y6_9PCalibrate_S1.ToString("0000.000");
            txtPixel_X9PTeachPosition_P7_S1.Text = Pixel_X7_9PCalibrate_S1.ToString("0000.000");
            txtPixel_Y9PTeachPosition_P7_S1.Text = Pixel_Y7_9PCalibrate_S1.ToString("0000.000");
            txtPixel_X9PTeachPosition_P8_S1.Text = Pixel_X8_9PCalibrate_S1.ToString("0000.000");
            txtPixel_Y9PTeachPosition_P8_S1.Text = Pixel_Y8_9PCalibrate_S1.ToString("0000.000");
            txtPixel_X9PTeachPosition_P9_S1.Text = Pixel_X9_9PCalibrate_S1.ToString("0000.000");
            txtPixel_Y9PTeachPosition_P9_S1.Text = Pixel_Y9_9PCalibrate_S1.ToString("0000.000");
        }

        private void btn9PCalibrateFindCircle_S2_Click(object sender, EventArgs e)
        {
            if (ActionForm._accessLevel < 101)
            {
                MyTool.TxtFileProcess.CreateLog("操作权限不合格:操作权限为工程师或者厂家!" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("操作权限不合格:操作权限为工程师或者厂家!");
                return;
            }

            //拍照获取9点标定点中心坐标
            if (!_9PCalibrateFindCircle_S(2, out Pixel_X1_9PCalibrate_S2, out Pixel_Y1_9PCalibrate_S2, out Pixel_X2_9PCalibrate_S2, out Pixel_Y2_9PCalibrate_S2, out Pixel_X3_9PCalibrate_S2, out Pixel_Y3_9PCalibrate_S2,
                                           out Pixel_X4_9PCalibrate_S2, out Pixel_Y4_9PCalibrate_S2, out Pixel_X5_9PCalibrate_S2, out Pixel_Y5_9PCalibrate_S2, out Pixel_X6_9PCalibrate_S2, out Pixel_Y6_9PCalibrate_S2,
                                           out Pixel_X7_9PCalibrate_S2, out Pixel_Y7_9PCalibrate_S2, out Pixel_X8_9PCalibrate_S2, out Pixel_Y8_9PCalibrate_S2, out Pixel_X9_9PCalibrate_S2, out Pixel_Y9_9PCalibrate_S2))
            {
                MyTool.TxtFileProcess.CreateLog("S2获取9点标定点中心坐标失败!" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("S2获取9点标定点中心坐标失败!");
                return;
            }
            txtPixel_X9PTeachPosition_P1_S2.Text = Pixel_X1_9PCalibrate_S2.ToString("0000.000");
            txtPixel_Y9PTeachPosition_P1_S2.Text = Pixel_Y1_9PCalibrate_S2.ToString("0000.000");
            txtPixel_X9PTeachPosition_P2_S2.Text = Pixel_X2_9PCalibrate_S2.ToString("0000.000");
            txtPixel_Y9PTeachPosition_P2_S2.Text = Pixel_Y2_9PCalibrate_S2.ToString("0000.000");
            txtPixel_X9PTeachPosition_P3_S2.Text = Pixel_X3_9PCalibrate_S2.ToString("0000.000");
            txtPixel_Y9PTeachPosition_P3_S2.Text = Pixel_Y3_9PCalibrate_S2.ToString("0000.000");
            txtPixel_X9PTeachPosition_P4_S2.Text = Pixel_X4_9PCalibrate_S2.ToString("0000.000");
            txtPixel_Y9PTeachPosition_P4_S2.Text = Pixel_Y4_9PCalibrate_S2.ToString("0000.000");
            txtPixel_X9PTeachPosition_P5_S2.Text = Pixel_X5_9PCalibrate_S2.ToString("0000.000");
            txtPixel_Y9PTeachPosition_P5_S2.Text = Pixel_Y5_9PCalibrate_S2.ToString("0000.000");
            txtPixel_X9PTeachPosition_P6_S2.Text = Pixel_X6_9PCalibrate_S2.ToString("0000.000");
            txtPixel_Y9PTeachPosition_P6_S2.Text = Pixel_Y6_9PCalibrate_S2.ToString("0000.000");
            txtPixel_X9PTeachPosition_P7_S2.Text = Pixel_X7_9PCalibrate_S2.ToString("0000.000");
            txtPixel_Y9PTeachPosition_P7_S2.Text = Pixel_Y7_9PCalibrate_S2.ToString("0000.000");
            txtPixel_X9PTeachPosition_P8_S2.Text = Pixel_X8_9PCalibrate_S2.ToString("0000.000");
            txtPixel_Y9PTeachPosition_P8_S2.Text = Pixel_Y8_9PCalibrate_S2.ToString("0000.000");
            txtPixel_X9PTeachPosition_P9_S2.Text = Pixel_X9_9PCalibrate_S2.ToString("0000.000");
            txtPixel_Y9PTeachPosition_P9_S2.Text = Pixel_Y9_9PCalibrate_S2.ToString("0000.000");
        }

        private void btn9PCalibrateFindCircle_S3_Click(object sender, EventArgs e)
        {
            if (ActionForm._accessLevel < 101)
            {
                MyTool.TxtFileProcess.CreateLog("操作权限不合格:操作权限为工程师或者厂家!" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("操作权限不合格:操作权限为工程师或者厂家!");
                return;
            }

            //拍照获取9点标定点中心坐标
            if (!_9PCalibrateFindCircle_S(3, out Pixel_X1_9PCalibrate_S3, out Pixel_Y1_9PCalibrate_S3, out Pixel_X2_9PCalibrate_S3, out Pixel_Y2_9PCalibrate_S3, out Pixel_X3_9PCalibrate_S3, out Pixel_Y3_9PCalibrate_S3,
                                           out Pixel_X4_9PCalibrate_S3, out Pixel_Y4_9PCalibrate_S3, out Pixel_X5_9PCalibrate_S3, out Pixel_Y5_9PCalibrate_S3, out Pixel_X6_9PCalibrate_S3, out Pixel_Y6_9PCalibrate_S3,
                                           out Pixel_X7_9PCalibrate_S3, out Pixel_Y7_9PCalibrate_S3, out Pixel_X8_9PCalibrate_S3, out Pixel_Y8_9PCalibrate_S3, out Pixel_X9_9PCalibrate_S3, out Pixel_Y9_9PCalibrate_S3))
            {
                MyTool.TxtFileProcess.CreateLog("S3获取9点标定点中心坐标失败!" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("S3获取9点标定点中心坐标失败!");
                return;
            }
            txtPixel_X9PTeachPosition_P1_S3.Text = Pixel_X1_9PCalibrate_S3.ToString("0000.000");
            txtPixel_Y9PTeachPosition_P1_S3.Text = Pixel_Y1_9PCalibrate_S3.ToString("0000.000");
            txtPixel_X9PTeachPosition_P2_S3.Text = Pixel_X2_9PCalibrate_S3.ToString("0000.000");
            txtPixel_Y9PTeachPosition_P2_S3.Text = Pixel_Y2_9PCalibrate_S3.ToString("0000.000");
            txtPixel_X9PTeachPosition_P3_S3.Text = Pixel_X3_9PCalibrate_S3.ToString("0000.000");
            txtPixel_Y9PTeachPosition_P3_S3.Text = Pixel_Y3_9PCalibrate_S3.ToString("0000.000");
            txtPixel_X9PTeachPosition_P4_S3.Text = Pixel_X4_9PCalibrate_S3.ToString("0000.000");
            txtPixel_Y9PTeachPosition_P4_S3.Text = Pixel_Y4_9PCalibrate_S3.ToString("0000.000");
            txtPixel_X9PTeachPosition_P5_S3.Text = Pixel_X5_9PCalibrate_S3.ToString("0000.000");
            txtPixel_Y9PTeachPosition_P5_S3.Text = Pixel_Y5_9PCalibrate_S3.ToString("0000.000");
            txtPixel_X9PTeachPosition_P6_S3.Text = Pixel_X6_9PCalibrate_S3.ToString("0000.000");
            txtPixel_Y9PTeachPosition_P6_S3.Text = Pixel_Y6_9PCalibrate_S3.ToString("0000.000");
            txtPixel_X9PTeachPosition_P7_S3.Text = Pixel_X7_9PCalibrate_S3.ToString("0000.000");
            txtPixel_Y9PTeachPosition_P7_S3.Text = Pixel_Y7_9PCalibrate_S3.ToString("0000.000");
            txtPixel_X9PTeachPosition_P8_S3.Text = Pixel_X8_9PCalibrate_S3.ToString("0000.000");
            txtPixel_Y9PTeachPosition_P8_S3.Text = Pixel_Y8_9PCalibrate_S3.ToString("0000.000");
            txtPixel_X9PTeachPosition_P9_S3.Text = Pixel_X9_9PCalibrate_S3.ToString("0000.000");
            txtPixel_Y9PTeachPosition_P9_S3.Text = Pixel_Y9_9PCalibrate_S3.ToString("0000.000");
        }
        #endregion

        #region btn9PTeach_P1-9_ClickEvent----没有用到
        public bool manualGetPositionToRobRequest = false;
        public bool manualGetPositionToRobRequest_Respond = false;
        int waitmanualGetPositionToRobRequest_Respond_Count;
        private void btn9PTeach_P1To9_ClickEvent(object sender, EventArgs e)
        {
            if (ActionForm._accessLevel < 101)
            {
                MyTool.TxtFileProcess.CreateLog("操作权限不合格:操作权限为工程师或者厂家!" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("操作权限不合格:操作权限为工程师或者厂家!");
                return;
            }

            //检查与ROB通讯是否正常
            if (!ActionForm._socketServerStartOKAndConnectROBOKFlag)
            {
                MessageBox.Show("与机械手通讯失败!");
                manualGetPositionToRobRequest = false;
                return;
            }

            manualGetPositionToRobRequest_Respond = false;
            manualGetPositionToRobRequest = true;
            waitmanualGetPositionToRobRequest_Respond_Count = 0;

            while (waitmanualGetPositionToRobRequest_Respond_Count < 1000000 && !manualGetPositionToRobRequest_Respond)
            {
                waitmanualGetPositionToRobRequest_Respond_Count++;
                Application.DoEvents();
            }
            if (!manualGetPositionToRobRequest_Respond)
            {
                MessageBox.Show("等待接收ROB位置数据超时!");
            }
            if (manualGetPositionToRobRequest_Respond)
            {
                //把数据填入textbox
                string ctrlName;
                object tempCtrl;

                ctrlName = "txtROB_X9PTeachPosition_P" + ((Button)sender).Name.Substring(((Button)sender).Name.Length - 1);
                tempCtrl = this.GetType().GetField(ctrlName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);
                ((TextBox)tempCtrl).Text = ActionForm.ROB_XCurrentPosition.ToString("0000.000");

                ctrlName = "txtROB_Y9PTeachPosition_P" + ((Button)sender).Name.Substring(((Button)sender).Name.Length - 1);
                tempCtrl = this.GetType().GetField(ctrlName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);
                ((TextBox)tempCtrl).Text = ActionForm.ROB_YCurrentPosition.ToString("0000.000");

                ctrlName = "txtROB_Z9PTeachPosition_P" + ((Button)sender).Name.Substring(((Button)sender).Name.Length - 1);
                tempCtrl = this.GetType().GetField(ctrlName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);
                ((TextBox)tempCtrl).Text = ActionForm.ROB_ZCurrentPosition.ToString("0000.000");

                ctrlName = "txtROB_RX9PTeachPosition_P" + ((Button)sender).Name.Substring(((Button)sender).Name.Length - 1);
                tempCtrl = this.GetType().GetField(ctrlName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);
                ((TextBox)tempCtrl).Text = ActionForm.ROB_RXCurrentPosition.ToString("0000.000");

                ctrlName = "txtROB_RW9PTeachPosition_P" + ((Button)sender).Name.Substring(((Button)sender).Name.Length - 1);
                tempCtrl = this.GetType().GetField(ctrlName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);
                ((TextBox)tempCtrl).Text = ActionForm.ROB_RWCurrentPosition.ToString("0000.000");

                ctrlName = "txtROB_RZ9PTeachPosition_P" + ((Button)sender).Name.Substring(((Button)sender).Name.Length - 1);
                tempCtrl = this.GetType().GetField(ctrlName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);
                ((TextBox)tempCtrl).Text = ActionForm.ROB_RZCurrentPosition.ToString("0000.000");
            }
            manualGetPositionToRobRequest_Respond = false;

        }
        #endregion

        #region 9点标定执行按钮----读取控件里的数据进行操作,不是变量
        private void btn9PCalibrateExecute_S1_Click(object sender, EventArgs e)
        {
            if (ActionForm._accessLevel < 101)
            {
                MessageBox.Show("操作权限不合格:操作权限为工程师或者厂家!");
                return;
            }

            btn9PCalibrateExecute_ClickEvent_S(1);
        }

        private void btn9PCalibrateExecute_S2_Click(object sender, EventArgs e)
        {
            if (ActionForm._accessLevel < 101)
            {
                MessageBox.Show("操作权限不合格:操作权限为工程师或者厂家!");
                return;
            }

            btn9PCalibrateExecute_ClickEvent_S(2);
        }

        private void btn9PCalibrateExecute_S3_Click(object sender, EventArgs e)
        {
            if (ActionForm._accessLevel < 101)
            {
                MessageBox.Show("操作权限不合格:操作权限为工程师或者厂家!");
                return;
            }

            btn9PCalibrateExecute_ClickEvent_S(3);
        }

        private bool btn9PCalibrateExecute_ClickEvent_S(int _stationNumber)
        {
            object tempCtrl;
            string ctrlName = "";

            float[] _x_ROB = new float[9];
            float[] _y_ROB = new float[9];
            float[] _z_ROB = new float[9];
            float[] _rx_ROB = new float[9];
            float[] _rw_ROB = new float[9];
            float[] _rz_ROB = new float[9];
            float[] _x_Pixel = new float[9];
            float[] _y_Pixel = new float[9];

            for (int i = 0; i < 9; i++)
            {
                try
                {
                    ctrlName = "txtROB_X9PTeachPosition_P" + (i + 1).ToString("0") + "_S" + _stationNumber.ToString("0");
                    tempCtrl = this.GetType().GetField(ctrlName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);
                    _x_ROB[i] = Convert.ToSingle(((TextBox)tempCtrl).Text);
                }
                catch
                {
                    MessageBox.Show("第" + (i + 1).ToString("0") + "点X坐标数据格式错误!");
                    return false;
                }
                try
                {
                    ctrlName = "txtROB_Y9PTeachPosition_P" + (i + 1).ToString("0") + "_S" + _stationNumber.ToString("0");
                    tempCtrl = this.GetType().GetField(ctrlName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);
                    _y_ROB[i] = Convert.ToSingle(((TextBox)tempCtrl).Text);
                }
                catch
                {
                    MessageBox.Show("第" + (i + 1).ToString("0") + "点Y坐标数据格式错误!");
                    return false;
                }
                try
                {
                    ctrlName = "txtROB_Z9PTeachPosition_P" + (i + 1).ToString("0") + "_S" + _stationNumber.ToString("0");
                    tempCtrl = this.GetType().GetField(ctrlName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);
                    _z_ROB[i] = Convert.ToSingle(((TextBox)tempCtrl).Text);
                }
                catch
                {
                    MessageBox.Show("第" + (i + 1).ToString("0") + "点Z坐标数据格式错误!");
                    return false;
                }
                try
                {
                    ctrlName = "txtROB_RX9PTeachPosition_P" + (i + 1).ToString("0") + "_S" + _stationNumber.ToString("0");
                    tempCtrl = this.GetType().GetField(ctrlName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);
                    _rx_ROB[i] = Convert.ToSingle(((TextBox)tempCtrl).Text);
                }
                catch
                {
                    MessageBox.Show("第" + (i + 1).ToString("0") + "点RX坐标数据格式错误!");
                    return false;
                }
                try
                {
                    ctrlName = "txtROB_RW9PTeachPosition_P" + (i + 1).ToString("0") + "_S" + _stationNumber.ToString("0");
                    tempCtrl = this.GetType().GetField(ctrlName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);
                    _rw_ROB[i] = Convert.ToSingle(((TextBox)tempCtrl).Text);
                }
                catch
                {
                    MessageBox.Show("第" + (i + 1).ToString("0") + "点RW坐标数据格式错误!");
                    return false;
                }
                try
                {
                    ctrlName = "txtROB_RZ9PTeachPosition_P" + (i + 1).ToString("0") + "_S" + _stationNumber.ToString("0");
                    tempCtrl = this.GetType().GetField(ctrlName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);
                    _rz_ROB[i] = Convert.ToSingle(((TextBox)tempCtrl).Text);
                }
                catch
                {
                    MessageBox.Show("第" + (i + 1).ToString("0") + "点RZ坐标数据格式错误!");
                    return false;
                }

                try
                {
                    ctrlName = "txtPixel_X9PTeachPosition_P" + (i + 1).ToString("0") + "_S" + _stationNumber.ToString("0");
                    tempCtrl = this.GetType().GetField(ctrlName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);
                    _x_Pixel[i] = Convert.ToSingle(((TextBox)tempCtrl).Text);
                }
                catch
                {
                    MessageBox.Show("第" + (i + 1).ToString("0") + "点X像素坐标数据格式错误!");
                    return false;
                }
                try
                {
                    ctrlName = "txtPixel_Y9PTeachPosition_P" + (i + 1).ToString("0") + "_S" + _stationNumber.ToString("0");
                    tempCtrl = this.GetType().GetField(ctrlName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);
                    _y_Pixel[i] = Convert.ToSingle(((TextBox)tempCtrl).Text);
                }
                catch
                {
                    MessageBox.Show("第" + (i + 1).ToString("0") + "点Y像素坐标数据格式错误!");
                    return false;
                }
            }
            int k;
            bool ValuesAreEqual;
            ValuesAreEqual = false;
            for (k = 0; k < _rx_ROB.Length; k++)
            {
                if (k >= 1)
                {
                    ValuesAreEqual = _rx_ROB[0].Equals(_rx_ROB[k]);
                    if (!ValuesAreEqual)
                    {
                        break;
                    }
                }
            }
            if (!ValuesAreEqual)
            {
                MessageBox.Show("RX坐标没有保持不变!");
                return false;
            }
            ValuesAreEqual = false;
            for (k = 0; k < _rw_ROB.Length; k++)
            {
                if (k >= 1)
                {
                    ValuesAreEqual = _rw_ROB[0].Equals(_rw_ROB[k]);
                    if (!ValuesAreEqual)
                    {
                        break;
                    }
                }
            }
            if (!ValuesAreEqual)
            {
                MessageBox.Show("RW坐标没有保持不变!");
                return false;
            }
            ValuesAreEqual = false;
            for (k = 0; k < _rz_ROB.Length; k++)
            {
                if (k >= 1)
                {
                    ValuesAreEqual = _rz_ROB[0].Equals(_rz_ROB[k]);
                    if (!ValuesAreEqual)
                    {
                        break;
                    }
                }
            }
            if (!ValuesAreEqual)
            {
                MessageBox.Show("RZ坐标没有保持不变!");
                return false;
            }
            if (_stationNumber == 1)
            {
                hv_HomMat2D_9P_S1 = newRobCamCalibration.robAndCamCalib_9P(_x_Pixel, _y_Pixel, _x_ROB, _y_ROB);
                if (hv_HomMat2D_9P_S1 == null)
                {
                    MessageBox.Show("S1 9点标定运算错误!");
                    return false;
                }
                MessageBox.Show("S1 9点标定运算完成!");
            }
            if (_stationNumber == 2)
            {
                hv_HomMat2D_9P_S2 = newRobCamCalibration.robAndCamCalib_9P(_x_Pixel, _y_Pixel, _x_ROB, _y_ROB);
                if (hv_HomMat2D_9P_S2 == null)
                {
                    MessageBox.Show("S2 9点标定运算错误!");
                    return false;
                }
                MessageBox.Show("S2 9点标定运算完成!");
            }
            if (_stationNumber == 3)
            {
                hv_HomMat2D_9P_S3 = newRobCamCalibration.robAndCamCalib_9P(_x_Pixel, _y_Pixel, _x_ROB, _y_ROB);
                if (hv_HomMat2D_9P_S3 == null)
                {
                    MessageBox.Show("S3 9点标定运算错误!");
                    return false;
                }
                MessageBox.Show("S3 9点标定运算完成!");
            }
            return true;
        }

        //保存9点标定的结果数据
        private bool _9PCalibrateResultSave_S(int _stationNumber)
        {
            if (_stationNumber == 1)
            {
                if (!newRobCamCalibration.saveDateRobAndCamCalib_9P(hv_HomMat2D_9P_S1, _GeneralParameterDirectoryPath + "\\" + "9PCalibrate_S1.mat"))
                {
                    MessageBox.Show("S1 9点标定结果保存失败!");
                    return false;
                }
                else
                {
                    MessageBox.Show("S1 9点标定结果成功保存!");
                }
            }
            if (_stationNumber == 2)
            {
                if (!newRobCamCalibration.saveDateRobAndCamCalib_9P(hv_HomMat2D_9P_S2, _GeneralParameterDirectoryPath + "\\" + "9PCalibrate_S2.mat"))
                {
                    MessageBox.Show("S2 9点标定结果保存失败!");
                    return false;
                }
                else
                {
                    MessageBox.Show("S2 9点标定结果成功保存!");
                }
            }
            if (_stationNumber == 3)
            {
                if (!newRobCamCalibration.saveDateRobAndCamCalib_9P(hv_HomMat2D_9P_S3, _GeneralParameterDirectoryPath + "\\" + "9PCalibrate_S3.mat"))
                {
                    MessageBox.Show("S3 9点标定结果保存失败!");
                    return false;
                }
                else
                {
                    MessageBox.Show("S3 9点标定结果成功保存!");
                }
            }
            return true;
        }

        #endregion

        #region _9PCalibrateFindCircle:获取标定点中心坐标
        private bool _9PCalibrateFindCircle_S(int _stationNumber, out float Pixel_X1_9PCalibrate, out float Pixel_Y1_9PCalibrate, out float Pixel_X2_9PCalibrate, out float Pixel_Y2_9PCalibrate,
                                               out float Pixel_X3_9PCalibrate, out float Pixel_Y3_9PCalibrate, out float Pixel_X4_9PCalibrate, out float Pixel_Y4_9PCalibrate,
                                               out float Pixel_X5_9PCalibrate, out float Pixel_Y5_9PCalibrate, out float Pixel_X6_9PCalibrate, out float Pixel_Y6_9PCalibrate,
                                               out float Pixel_X7_9PCalibrate, out float Pixel_Y7_9PCalibrate, out float Pixel_X8_9PCalibrate, out float Pixel_Y8_9PCalibrate,
                                               out float Pixel_X9_9PCalibrate, out float Pixel_Y9_9PCalibrate)
        {
            Pixel_X1_9PCalibrate = 0f;
            Pixel_Y1_9PCalibrate = 0f;
            Pixel_X2_9PCalibrate = 0f;
            Pixel_Y2_9PCalibrate = 0f;
            Pixel_X3_9PCalibrate = 0f;
            Pixel_Y3_9PCalibrate = 0f;
            Pixel_X4_9PCalibrate = 0f;
            Pixel_Y4_9PCalibrate = 0f;
            Pixel_X5_9PCalibrate = 0f;
            Pixel_Y5_9PCalibrate = 0f;
            Pixel_X6_9PCalibrate = 0f;
            Pixel_Y6_9PCalibrate = 0f;
            Pixel_X7_9PCalibrate = 0f;
            Pixel_Y7_9PCalibrate = 0f;
            Pixel_X8_9PCalibrate = 0f;
            Pixel_Y8_9PCalibrate = 0f;
            Pixel_X9_9PCalibrate = 0f;
            Pixel_Y9_9PCalibrate = 0f;

            List<HTuple> CircleParamList = new List<HTuple>();

            //Station1
            if (_stationNumber == 1)
            {
                if (ho_Image_S1 == null)
                {
                    MessageBox.Show("S1 请先拍照或者打开图片!");
                    return false;
                }

                //清除显示
                hw_S1.ClearWindow();
                ActionForm.hw_S1.ClearWindow();
                hw_S1.HobjectToHimage(ho_Image_S1);
                ActionForm.hw_S1.HobjectToHimage(ho_Image_S1);

                //获取标定点中心坐标
                try
                {
                    HDevelopExport.Instance.MatchAllModel(ho_Image_S1, smf_S1.ModelId, smf_S1.NccModelId, smf_S1.InputDict, smf_S1.searchRoiList, smf_S1.findModelRoiCfgList, out HTuple MatchRstDict, false);
                    HDevelopExport.Instance.GetCircleCenter(ho_Image_S1, smf_S1.ModelId, smf_S1.NccModelId, smf_S1.InputDict, smf_S1.searchRoiList, smf_S1.findModelRoiCfgList,
                        smf_S1.ModelOutputTuple, dsf_S1.FindEdgeRoiList, dsf_S1.FindLineCfgList, out CircleParamList);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("S1 9点标定获取标定点中心坐标失败!");
                    return false;
                }
                if (CircleParamList.Count != 9)
                {
                    MessageBox.Show("S1 9点标定获取标定点个数不是9个!");
                    return false;
                }

                //显示
                try
                {
                    newMinimumEnclosingRectangle.set_display_font(hw_S1.hWindowControl.HalconWindow, 20, "mono", "false", "false");
                    hw_S1.viewWindow.dispMessage("1", "green", CircleParamList[0][0].F, CircleParamList[0][1].F, "false");
                    hw_S1.viewWindow.dispMessage("2", "green", CircleParamList[1][0].F, CircleParamList[1][1].F, "false");
                    hw_S1.viewWindow.dispMessage("3", "green", CircleParamList[2][0].F, CircleParamList[2][1].F, "false");
                    hw_S1.viewWindow.dispMessage("4", "green", CircleParamList[3][0].F, CircleParamList[3][1].F, "false");
                    hw_S1.viewWindow.dispMessage("5", "green", CircleParamList[4][0].F, CircleParamList[4][1].F, "false");
                    hw_S1.viewWindow.dispMessage("6", "green", CircleParamList[5][0].F, CircleParamList[5][1].F, "false");
                    hw_S1.viewWindow.dispMessage("7", "green", CircleParamList[6][0].F, CircleParamList[6][1].F, "false");
                    hw_S1.viewWindow.dispMessage("8", "green", CircleParamList[7][0].F, CircleParamList[7][1].F, "false");
                    hw_S1.viewWindow.dispMessage("9", "green", CircleParamList[8][0].F, CircleParamList[8][1].F, "false");
                    newMinimumEnclosingRectangle.set_display_font(hw_S1.hWindowControl.HalconWindow, 16, "mono", "false", "false");
                    hw_S1.viewWindow.dispMessage("[" + CircleParamList[0][0].F.ToString() + "," + CircleParamList[0][1].F.ToString() + "]", "green", CircleParamList[0][0].F + 30, CircleParamList[0][1].F + 0, "false");
                    hw_S1.viewWindow.dispMessage("[" + CircleParamList[1][0].F.ToString() + "," + CircleParamList[1][1].F.ToString() + "]", "green", CircleParamList[1][0].F + 30, CircleParamList[1][1].F + 0, "false");
                    hw_S1.viewWindow.dispMessage("[" + CircleParamList[2][0].F.ToString() + "," + CircleParamList[2][1].F.ToString() + "]", "green", CircleParamList[2][0].F + 30, CircleParamList[2][1].F + 0, "false");
                    hw_S1.viewWindow.dispMessage("[" + CircleParamList[3][0].F.ToString() + "," + CircleParamList[3][1].F.ToString() + "]", "green", CircleParamList[3][0].F + 30, CircleParamList[3][1].F + 0, "false");
                    hw_S1.viewWindow.dispMessage("[" + CircleParamList[4][0].F.ToString() + "," + CircleParamList[4][1].F.ToString() + "]", "green", CircleParamList[4][0].F + 30, CircleParamList[4][1].F + 0, "false");
                    hw_S1.viewWindow.dispMessage("[" + CircleParamList[5][0].F.ToString() + "," + CircleParamList[5][1].F.ToString() + "]", "green", CircleParamList[5][0].F + 30, CircleParamList[5][1].F + 0, "false");
                    hw_S1.viewWindow.dispMessage("[" + CircleParamList[6][0].F.ToString() + "," + CircleParamList[6][1].F.ToString() + "]", "green", CircleParamList[6][0].F + 30, CircleParamList[6][1].F + 0, "false");
                    hw_S1.viewWindow.dispMessage("[" + CircleParamList[7][0].F.ToString() + "," + CircleParamList[7][1].F.ToString() + "]", "green", CircleParamList[7][0].F + 30, CircleParamList[7][1].F + 0, "false");
                    hw_S1.viewWindow.dispMessage("[" + CircleParamList[8][0].F.ToString() + "," + CircleParamList[8][1].F.ToString() + "]", "green", CircleParamList[8][0].F + 30, CircleParamList[8][1].F + 0, "false");

                    HObject _circle = new HObject();
                    HOperatorSet.GenCircle(out _circle, CircleParamList[0][0].F, CircleParamList[0][1].F, CircleParamList[0][2].F);
                    hw_S1.viewWindow.displayHobject(_circle, "green");
                    _circle.Dispose();
                    HOperatorSet.GenCircle(out _circle, CircleParamList[1][0].F, CircleParamList[1][1].F, CircleParamList[1][2].F);
                    hw_S1.viewWindow.displayHobject(_circle, "green");
                    _circle.Dispose();
                    HOperatorSet.GenCircle(out _circle, CircleParamList[2][0].F, CircleParamList[2][1].F, CircleParamList[2][2].F);
                    hw_S1.viewWindow.displayHobject(_circle, "green");
                    _circle.Dispose();
                    HOperatorSet.GenCircle(out _circle, CircleParamList[3][0].F, CircleParamList[3][1].F, CircleParamList[3][2].F);
                    hw_S1.viewWindow.displayHobject(_circle, "green");
                    _circle.Dispose();
                    HOperatorSet.GenCircle(out _circle, CircleParamList[4][0].F, CircleParamList[4][1].F, CircleParamList[4][2].F);
                    hw_S1.viewWindow.displayHobject(_circle, "green");
                    _circle.Dispose();
                    HOperatorSet.GenCircle(out _circle, CircleParamList[5][0].F, CircleParamList[5][1].F, CircleParamList[5][2].F);
                    hw_S1.viewWindow.displayHobject(_circle, "green");
                    _circle.Dispose();
                    HOperatorSet.GenCircle(out _circle, CircleParamList[6][0].F, CircleParamList[6][1].F, CircleParamList[6][2].F);
                    hw_S1.viewWindow.displayHobject(_circle, "green");
                    _circle.Dispose();
                    HOperatorSet.GenCircle(out _circle, CircleParamList[7][0].F, CircleParamList[7][1].F, CircleParamList[7][2].F);
                    hw_S1.viewWindow.displayHobject(_circle, "green");
                    _circle.Dispose();
                    HOperatorSet.GenCircle(out _circle, CircleParamList[8][0].F, CircleParamList[8][1].F, CircleParamList[8][2].F);
                    hw_S1.viewWindow.displayHobject(_circle, "green");
                    _circle.Dispose();


                    newMinimumEnclosingRectangle.set_display_font(ActionForm.hw_S1.hWindowControl.HalconWindow, 20, "mono", "false", "false");
                    ActionForm.hw_S1.viewWindow.dispMessage("1", "green", CircleParamList[0][0].F, CircleParamList[0][1].F, "false");
                    ActionForm.hw_S1.viewWindow.dispMessage("2", "green", CircleParamList[1][0].F, CircleParamList[1][1].F, "false");
                    ActionForm.hw_S1.viewWindow.dispMessage("3", "green", CircleParamList[2][0].F, CircleParamList[2][1].F, "false");
                    ActionForm.hw_S1.viewWindow.dispMessage("4", "green", CircleParamList[3][0].F, CircleParamList[3][1].F, "false");
                    ActionForm.hw_S1.viewWindow.dispMessage("5", "green", CircleParamList[4][0].F, CircleParamList[4][1].F, "false");
                    ActionForm.hw_S1.viewWindow.dispMessage("6", "green", CircleParamList[5][0].F, CircleParamList[5][1].F, "false");
                    ActionForm.hw_S1.viewWindow.dispMessage("7", "green", CircleParamList[6][0].F, CircleParamList[6][1].F, "false");
                    ActionForm.hw_S1.viewWindow.dispMessage("8", "green", CircleParamList[7][0].F, CircleParamList[7][1].F, "false");
                    ActionForm.hw_S1.viewWindow.dispMessage("9", "green", CircleParamList[8][0].F, CircleParamList[8][1].F, "false");
                    newMinimumEnclosingRectangle.set_display_font(ActionForm.hw_S1.hWindowControl.HalconWindow, 16, "mono", "false", "false");
                    ActionForm.hw_S1.viewWindow.dispMessage("[" + CircleParamList[0][0].F.ToString() + "," + CircleParamList[0][1].F.ToString() + "]", "green", CircleParamList[0][0].F + 30, CircleParamList[0][1].F + 0, "false");
                    ActionForm.hw_S1.viewWindow.dispMessage("[" + CircleParamList[1][0].F.ToString() + "," + CircleParamList[1][1].F.ToString() + "]", "green", CircleParamList[1][0].F + 30, CircleParamList[1][1].F + 0, "false");
                    ActionForm.hw_S1.viewWindow.dispMessage("[" + CircleParamList[2][0].F.ToString() + "," + CircleParamList[2][1].F.ToString() + "]", "green", CircleParamList[2][0].F + 30, CircleParamList[2][1].F + 0, "false");
                    ActionForm.hw_S1.viewWindow.dispMessage("[" + CircleParamList[3][0].F.ToString() + "," + CircleParamList[3][1].F.ToString() + "]", "green", CircleParamList[3][0].F + 30, CircleParamList[3][1].F + 0, "false");
                    ActionForm.hw_S1.viewWindow.dispMessage("[" + CircleParamList[4][0].F.ToString() + "," + CircleParamList[4][1].F.ToString() + "]", "green", CircleParamList[4][0].F + 30, CircleParamList[4][1].F + 0, "false");
                    ActionForm.hw_S1.viewWindow.dispMessage("[" + CircleParamList[5][0].F.ToString() + "," + CircleParamList[5][1].F.ToString() + "]", "green", CircleParamList[5][0].F + 30, CircleParamList[5][1].F + 0, "false");
                    ActionForm.hw_S1.viewWindow.dispMessage("[" + CircleParamList[6][0].F.ToString() + "," + CircleParamList[6][1].F.ToString() + "]", "green", CircleParamList[6][0].F + 30, CircleParamList[6][1].F + 0, "false");
                    ActionForm.hw_S1.viewWindow.dispMessage("[" + CircleParamList[7][0].F.ToString() + "," + CircleParamList[7][1].F.ToString() + "]", "green", CircleParamList[7][0].F + 30, CircleParamList[7][1].F + 0, "false");
                    ActionForm.hw_S1.viewWindow.dispMessage("[" + CircleParamList[8][0].F.ToString() + "," + CircleParamList[8][1].F.ToString() + "]", "green", CircleParamList[8][0].F + 30, CircleParamList[8][1].F + 0, "false");

                    HOperatorSet.GenCircle(out _circle, CircleParamList[0][0].F, CircleParamList[0][1].F, CircleParamList[0][2].F);
                    ActionForm.hw_S1.viewWindow.displayHobject(_circle, "green");
                    _circle.Dispose();
                    HOperatorSet.GenCircle(out _circle, CircleParamList[1][0].F, CircleParamList[1][1].F, CircleParamList[1][2].F);
                    ActionForm.hw_S1.viewWindow.displayHobject(_circle, "green");
                    _circle.Dispose();
                    HOperatorSet.GenCircle(out _circle, CircleParamList[2][0].F, CircleParamList[2][1].F, CircleParamList[2][2].F);
                    ActionForm.hw_S1.viewWindow.displayHobject(_circle, "green");
                    _circle.Dispose();
                    HOperatorSet.GenCircle(out _circle, CircleParamList[3][0].F, CircleParamList[3][1].F, CircleParamList[3][2].F);
                    ActionForm.hw_S1.viewWindow.displayHobject(_circle, "green");
                    _circle.Dispose();
                    HOperatorSet.GenCircle(out _circle, CircleParamList[4][0].F, CircleParamList[4][1].F, CircleParamList[4][2].F);
                    ActionForm.hw_S1.viewWindow.displayHobject(_circle, "green");
                    _circle.Dispose();
                    HOperatorSet.GenCircle(out _circle, CircleParamList[5][0].F, CircleParamList[5][1].F, CircleParamList[5][2].F);
                    ActionForm.hw_S1.viewWindow.displayHobject(_circle, "green");
                    _circle.Dispose();
                    HOperatorSet.GenCircle(out _circle, CircleParamList[6][0].F, CircleParamList[6][1].F, CircleParamList[6][2].F);
                    ActionForm.hw_S1.viewWindow.displayHobject(_circle, "green");
                    _circle.Dispose();
                    HOperatorSet.GenCircle(out _circle, CircleParamList[7][0].F, CircleParamList[7][1].F, CircleParamList[7][2].F);
                    ActionForm.hw_S1.viewWindow.displayHobject(_circle, "green");
                    _circle.Dispose();
                    HOperatorSet.GenCircle(out _circle, CircleParamList[8][0].F, CircleParamList[8][1].F, CircleParamList[8][2].F);
                    ActionForm.hw_S1.viewWindow.displayHobject(_circle, "green");
                    _circle.Dispose();

                }
                catch (Exception ex)
                {
                    MessageBox.Show("S1 9点标定显示找到的圆失败!");
                }
            }

            //Station2
            if (_stationNumber == 2)
            {
                if (ho_Image_S2 == null)
                {
                    MessageBox.Show("S2 请先拍照或者打开图片!");
                    return false;
                }

                //清除显示
                hw_S2.ClearWindow();
                ActionForm.hw_S2.ClearWindow();
                hw_S2.HobjectToHimage(ho_Image_S2);
                ActionForm.hw_S2.HobjectToHimage(ho_Image_S2);

                //获取标定点中心坐标
                try
                {
                    HDevelopExport.Instance.MatchAllModel(ho_Image_S2, smf_S2.ModelId, smf_S2.NccModelId, smf_S2.InputDict, smf_S2.searchRoiList, smf_S2.findModelRoiCfgList, out HTuple MatchRstDict, false);
                    HDevelopExport.Instance.GetCircleCenter(ho_Image_S2, smf_S2.ModelId, smf_S2.NccModelId, smf_S2.InputDict, smf_S2.searchRoiList, smf_S2.findModelRoiCfgList,
                        smf_S2.ModelOutputTuple, dsf_S2.FindEdgeRoiList, dsf_S2.FindLineCfgList, out CircleParamList);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("S2 9点标定获取标定点中心坐标失败!");
                    return false;
                }
                if (CircleParamList.Count != 9)
                {
                    MessageBox.Show("S2 9点标定获取标定点个数不是9个!");
                    return false;
                }

                //显示
                try
                {
                    newMinimumEnclosingRectangle.set_display_font(hw_S2.hWindowControl.HalconWindow, 20, "mono", "false", "false");
                    hw_S2.viewWindow.dispMessage("1", "green", CircleParamList[0][0].F, CircleParamList[0][1].F, "false");
                    hw_S2.viewWindow.dispMessage("2", "green", CircleParamList[1][0].F, CircleParamList[1][1].F, "false");
                    hw_S2.viewWindow.dispMessage("3", "green", CircleParamList[2][0].F, CircleParamList[2][1].F, "false");
                    hw_S2.viewWindow.dispMessage("4", "green", CircleParamList[3][0].F, CircleParamList[3][1].F, "false");
                    hw_S2.viewWindow.dispMessage("5", "green", CircleParamList[4][0].F, CircleParamList[4][1].F, "false");
                    hw_S2.viewWindow.dispMessage("6", "green", CircleParamList[5][0].F, CircleParamList[5][1].F, "false");
                    hw_S2.viewWindow.dispMessage("7", "green", CircleParamList[6][0].F, CircleParamList[6][1].F, "false");
                    hw_S2.viewWindow.dispMessage("8", "green", CircleParamList[7][0].F, CircleParamList[7][1].F, "false");
                    hw_S2.viewWindow.dispMessage("9", "green", CircleParamList[8][0].F, CircleParamList[8][1].F, "false");
                    newMinimumEnclosingRectangle.set_display_font(hw_S2.hWindowControl.HalconWindow, 16, "mono", "false", "false");
                    hw_S2.viewWindow.dispMessage("[" + CircleParamList[0][0].F.ToString() + "," + CircleParamList[0][1].F.ToString() + "]", "green", CircleParamList[0][0].F + 30, CircleParamList[0][1].F + 0, "false");
                    hw_S2.viewWindow.dispMessage("[" + CircleParamList[1][0].F.ToString() + "," + CircleParamList[1][1].F.ToString() + "]", "green", CircleParamList[1][0].F + 30, CircleParamList[1][1].F + 0, "false");
                    hw_S2.viewWindow.dispMessage("[" + CircleParamList[2][0].F.ToString() + "," + CircleParamList[2][1].F.ToString() + "]", "green", CircleParamList[2][0].F + 30, CircleParamList[2][1].F + 0, "false");
                    hw_S2.viewWindow.dispMessage("[" + CircleParamList[3][0].F.ToString() + "," + CircleParamList[3][1].F.ToString() + "]", "green", CircleParamList[3][0].F + 30, CircleParamList[3][1].F + 0, "false");
                    hw_S2.viewWindow.dispMessage("[" + CircleParamList[4][0].F.ToString() + "," + CircleParamList[4][1].F.ToString() + "]", "green", CircleParamList[4][0].F + 30, CircleParamList[4][1].F + 0, "false");
                    hw_S2.viewWindow.dispMessage("[" + CircleParamList[5][0].F.ToString() + "," + CircleParamList[5][1].F.ToString() + "]", "green", CircleParamList[5][0].F + 30, CircleParamList[5][1].F + 0, "false");
                    hw_S2.viewWindow.dispMessage("[" + CircleParamList[6][0].F.ToString() + "," + CircleParamList[6][1].F.ToString() + "]", "green", CircleParamList[6][0].F + 30, CircleParamList[6][1].F + 0, "false");
                    hw_S2.viewWindow.dispMessage("[" + CircleParamList[7][0].F.ToString() + "," + CircleParamList[7][1].F.ToString() + "]", "green", CircleParamList[7][0].F + 30, CircleParamList[7][1].F + 0, "false");
                    hw_S2.viewWindow.dispMessage("[" + CircleParamList[8][0].F.ToString() + "," + CircleParamList[8][1].F.ToString() + "]", "green", CircleParamList[8][0].F + 30, CircleParamList[8][1].F + 0, "false");

                    HObject _circle = new HObject();
                    HOperatorSet.GenCircle(out _circle, CircleParamList[0][0].F, CircleParamList[0][1].F, CircleParamList[0][2].F);
                    hw_S2.viewWindow.displayHobject(_circle, "green");
                    _circle.Dispose();
                    HOperatorSet.GenCircle(out _circle, CircleParamList[1][0].F, CircleParamList[1][1].F, CircleParamList[1][2].F);
                    hw_S2.viewWindow.displayHobject(_circle, "green");
                    _circle.Dispose();
                    HOperatorSet.GenCircle(out _circle, CircleParamList[2][0].F, CircleParamList[2][1].F, CircleParamList[2][2].F);
                    hw_S2.viewWindow.displayHobject(_circle, "green");
                    _circle.Dispose();
                    HOperatorSet.GenCircle(out _circle, CircleParamList[3][0].F, CircleParamList[3][1].F, CircleParamList[3][2].F);
                    hw_S2.viewWindow.displayHobject(_circle, "green");
                    _circle.Dispose();
                    HOperatorSet.GenCircle(out _circle, CircleParamList[4][0].F, CircleParamList[4][1].F, CircleParamList[4][2].F);
                    hw_S2.viewWindow.displayHobject(_circle, "green");
                    _circle.Dispose();
                    HOperatorSet.GenCircle(out _circle, CircleParamList[5][0].F, CircleParamList[5][1].F, CircleParamList[5][2].F);
                    hw_S2.viewWindow.displayHobject(_circle, "green");
                    _circle.Dispose();
                    HOperatorSet.GenCircle(out _circle, CircleParamList[6][0].F, CircleParamList[6][1].F, CircleParamList[6][2].F);
                    hw_S2.viewWindow.displayHobject(_circle, "green");
                    _circle.Dispose();
                    HOperatorSet.GenCircle(out _circle, CircleParamList[7][0].F, CircleParamList[7][1].F, CircleParamList[7][2].F);
                    hw_S2.viewWindow.displayHobject(_circle, "green");
                    _circle.Dispose();
                    HOperatorSet.GenCircle(out _circle, CircleParamList[8][0].F, CircleParamList[8][1].F, CircleParamList[8][2].F);
                    hw_S2.viewWindow.displayHobject(_circle, "green");
                    _circle.Dispose();


                    newMinimumEnclosingRectangle.set_display_font(ActionForm.hw_S2.hWindowControl.HalconWindow, 20, "mono", "false", "false");
                    ActionForm.hw_S2.viewWindow.dispMessage("1", "green", CircleParamList[0][0].F, CircleParamList[0][1].F, "false");
                    ActionForm.hw_S2.viewWindow.dispMessage("2", "green", CircleParamList[1][0].F, CircleParamList[1][1].F, "false");
                    ActionForm.hw_S2.viewWindow.dispMessage("3", "green", CircleParamList[2][0].F, CircleParamList[2][1].F, "false");
                    ActionForm.hw_S2.viewWindow.dispMessage("4", "green", CircleParamList[3][0].F, CircleParamList[3][1].F, "false");
                    ActionForm.hw_S2.viewWindow.dispMessage("5", "green", CircleParamList[4][0].F, CircleParamList[4][1].F, "false");
                    ActionForm.hw_S2.viewWindow.dispMessage("6", "green", CircleParamList[5][0].F, CircleParamList[5][1].F, "false");
                    ActionForm.hw_S2.viewWindow.dispMessage("7", "green", CircleParamList[6][0].F, CircleParamList[6][1].F, "false");
                    ActionForm.hw_S2.viewWindow.dispMessage("8", "green", CircleParamList[7][0].F, CircleParamList[7][1].F, "false");
                    ActionForm.hw_S2.viewWindow.dispMessage("9", "green", CircleParamList[8][0].F, CircleParamList[8][1].F, "false");
                    newMinimumEnclosingRectangle.set_display_font(ActionForm.hw_S2.hWindowControl.HalconWindow, 16, "mono", "false", "false");
                    ActionForm.hw_S2.viewWindow.dispMessage("[" + CircleParamList[0][0].F.ToString() + "," + CircleParamList[0][1].F.ToString() + "]", "green", CircleParamList[0][0].F + 30, CircleParamList[0][1].F + 0, "false");
                    ActionForm.hw_S2.viewWindow.dispMessage("[" + CircleParamList[1][0].F.ToString() + "," + CircleParamList[1][1].F.ToString() + "]", "green", CircleParamList[1][0].F + 30, CircleParamList[1][1].F + 0, "false");
                    ActionForm.hw_S2.viewWindow.dispMessage("[" + CircleParamList[2][0].F.ToString() + "," + CircleParamList[2][1].F.ToString() + "]", "green", CircleParamList[2][0].F + 30, CircleParamList[2][1].F + 0, "false");
                    ActionForm.hw_S2.viewWindow.dispMessage("[" + CircleParamList[3][0].F.ToString() + "," + CircleParamList[3][1].F.ToString() + "]", "green", CircleParamList[3][0].F + 30, CircleParamList[3][1].F + 0, "false");
                    ActionForm.hw_S2.viewWindow.dispMessage("[" + CircleParamList[4][0].F.ToString() + "," + CircleParamList[4][1].F.ToString() + "]", "green", CircleParamList[4][0].F + 30, CircleParamList[4][1].F + 0, "false");
                    ActionForm.hw_S2.viewWindow.dispMessage("[" + CircleParamList[5][0].F.ToString() + "," + CircleParamList[5][1].F.ToString() + "]", "green", CircleParamList[5][0].F + 30, CircleParamList[5][1].F + 0, "false");
                    ActionForm.hw_S2.viewWindow.dispMessage("[" + CircleParamList[6][0].F.ToString() + "," + CircleParamList[6][1].F.ToString() + "]", "green", CircleParamList[6][0].F + 30, CircleParamList[6][1].F + 0, "false");
                    ActionForm.hw_S2.viewWindow.dispMessage("[" + CircleParamList[7][0].F.ToString() + "," + CircleParamList[7][1].F.ToString() + "]", "green", CircleParamList[7][0].F + 30, CircleParamList[7][1].F + 0, "false");
                    ActionForm.hw_S2.viewWindow.dispMessage("[" + CircleParamList[8][0].F.ToString() + "," + CircleParamList[8][1].F.ToString() + "]", "green", CircleParamList[8][0].F + 30, CircleParamList[8][1].F + 0, "false");

                    HOperatorSet.GenCircle(out _circle, CircleParamList[0][0].F, CircleParamList[0][1].F, CircleParamList[0][2].F);
                    ActionForm.hw_S2.viewWindow.displayHobject(_circle, "green");
                    _circle.Dispose();
                    HOperatorSet.GenCircle(out _circle, CircleParamList[1][0].F, CircleParamList[1][1].F, CircleParamList[1][2].F);
                    ActionForm.hw_S2.viewWindow.displayHobject(_circle, "green");
                    _circle.Dispose();
                    HOperatorSet.GenCircle(out _circle, CircleParamList[2][0].F, CircleParamList[2][1].F, CircleParamList[2][2].F);
                    ActionForm.hw_S2.viewWindow.displayHobject(_circle, "green");
                    _circle.Dispose();
                    HOperatorSet.GenCircle(out _circle, CircleParamList[3][0].F, CircleParamList[3][1].F, CircleParamList[3][2].F);
                    ActionForm.hw_S2.viewWindow.displayHobject(_circle, "green");
                    _circle.Dispose();
                    HOperatorSet.GenCircle(out _circle, CircleParamList[4][0].F, CircleParamList[4][1].F, CircleParamList[4][2].F);
                    ActionForm.hw_S2.viewWindow.displayHobject(_circle, "green");
                    _circle.Dispose();
                    HOperatorSet.GenCircle(out _circle, CircleParamList[5][0].F, CircleParamList[5][1].F, CircleParamList[5][2].F);
                    ActionForm.hw_S2.viewWindow.displayHobject(_circle, "green");
                    _circle.Dispose();
                    HOperatorSet.GenCircle(out _circle, CircleParamList[6][0].F, CircleParamList[6][1].F, CircleParamList[6][2].F);
                    ActionForm.hw_S2.viewWindow.displayHobject(_circle, "green");
                    _circle.Dispose();
                    HOperatorSet.GenCircle(out _circle, CircleParamList[7][0].F, CircleParamList[7][1].F, CircleParamList[7][2].F);
                    ActionForm.hw_S2.viewWindow.displayHobject(_circle, "green");
                    _circle.Dispose();
                    HOperatorSet.GenCircle(out _circle, CircleParamList[8][0].F, CircleParamList[8][1].F, CircleParamList[8][2].F);
                    ActionForm.hw_S2.viewWindow.displayHobject(_circle, "green");
                    _circle.Dispose();

                }
                catch (Exception ex)
                {
                    MessageBox.Show("S2 9点标定显示找到的圆失败!");
                }
            }

            //Station3
            if (_stationNumber == 3)
            {
                if (ho_Image_S3 == null)
                {
                    MessageBox.Show("S3 请先拍照或者打开图片!");
                    return false;
                }

                //清除显示
                hw_S3.ClearWindow();
                ActionForm.hw_S3.ClearWindow();
                hw_S3.HobjectToHimage(ho_Image_S3);
                ActionForm.hw_S3.HobjectToHimage(ho_Image_S3);

                //获取标定点中心坐标
                try
                {
                    HDevelopExport.Instance.MatchAllModel(ho_Image_S3, smf_S3.ModelId, smf_S3.NccModelId, smf_S3.InputDict, smf_S3.searchRoiList, smf_S3.findModelRoiCfgList, out HTuple MatchRstDict, false);
                    HDevelopExport.Instance.GetCircleCenter(ho_Image_S3, smf_S3.ModelId, smf_S3.NccModelId, smf_S3.InputDict, smf_S3.searchRoiList, smf_S3.findModelRoiCfgList,
                        smf_S3.ModelOutputTuple, dsf_S3.FindEdgeRoiList, dsf_S3.FindLineCfgList, out CircleParamList);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("S3 9点标定获取标定点中心坐标失败!");
                    return false;
                }
                if (CircleParamList.Count != 9)
                {
                    MessageBox.Show("S3 9点标定获取标定点个数不是9个!");
                    return false;
                }

                //显示
                try
                {
                    newMinimumEnclosingRectangle.set_display_font(hw_S3.hWindowControl.HalconWindow, 20, "mono", "false", "false");
                    hw_S3.viewWindow.dispMessage("1", "green", CircleParamList[0][0].F, CircleParamList[0][1].F, "false");
                    hw_S3.viewWindow.dispMessage("2", "green", CircleParamList[1][0].F, CircleParamList[1][1].F, "false");
                    hw_S3.viewWindow.dispMessage("3", "green", CircleParamList[2][0].F, CircleParamList[2][1].F, "false");
                    hw_S3.viewWindow.dispMessage("4", "green", CircleParamList[3][0].F, CircleParamList[3][1].F, "false");
                    hw_S3.viewWindow.dispMessage("5", "green", CircleParamList[4][0].F, CircleParamList[4][1].F, "false");
                    hw_S3.viewWindow.dispMessage("6", "green", CircleParamList[5][0].F, CircleParamList[5][1].F, "false");
                    hw_S3.viewWindow.dispMessage("7", "green", CircleParamList[6][0].F, CircleParamList[6][1].F, "false");
                    hw_S3.viewWindow.dispMessage("8", "green", CircleParamList[7][0].F, CircleParamList[7][1].F, "false");
                    hw_S3.viewWindow.dispMessage("9", "green", CircleParamList[8][0].F, CircleParamList[8][1].F, "false");
                    newMinimumEnclosingRectangle.set_display_font(hw_S3.hWindowControl.HalconWindow, 16, "mono", "false", "false");
                    hw_S3.viewWindow.dispMessage("[" + CircleParamList[0][0].F.ToString() + "," + CircleParamList[0][1].F.ToString() + "]", "green", CircleParamList[0][0].F + 30, CircleParamList[0][1].F + 0, "false");
                    hw_S3.viewWindow.dispMessage("[" + CircleParamList[1][0].F.ToString() + "," + CircleParamList[1][1].F.ToString() + "]", "green", CircleParamList[1][0].F + 30, CircleParamList[1][1].F + 0, "false");
                    hw_S3.viewWindow.dispMessage("[" + CircleParamList[2][0].F.ToString() + "," + CircleParamList[2][1].F.ToString() + "]", "green", CircleParamList[2][0].F + 30, CircleParamList[2][1].F + 0, "false");
                    hw_S3.viewWindow.dispMessage("[" + CircleParamList[3][0].F.ToString() + "," + CircleParamList[3][1].F.ToString() + "]", "green", CircleParamList[3][0].F + 30, CircleParamList[3][1].F + 0, "false");
                    hw_S3.viewWindow.dispMessage("[" + CircleParamList[4][0].F.ToString() + "," + CircleParamList[4][1].F.ToString() + "]", "green", CircleParamList[4][0].F + 30, CircleParamList[4][1].F + 0, "false");
                    hw_S3.viewWindow.dispMessage("[" + CircleParamList[5][0].F.ToString() + "," + CircleParamList[5][1].F.ToString() + "]", "green", CircleParamList[5][0].F + 30, CircleParamList[5][1].F + 0, "false");
                    hw_S3.viewWindow.dispMessage("[" + CircleParamList[6][0].F.ToString() + "," + CircleParamList[6][1].F.ToString() + "]", "green", CircleParamList[6][0].F + 30, CircleParamList[6][1].F + 0, "false");
                    hw_S3.viewWindow.dispMessage("[" + CircleParamList[7][0].F.ToString() + "," + CircleParamList[7][1].F.ToString() + "]", "green", CircleParamList[7][0].F + 30, CircleParamList[7][1].F + 0, "false");
                    hw_S3.viewWindow.dispMessage("[" + CircleParamList[8][0].F.ToString() + "," + CircleParamList[8][1].F.ToString() + "]", "green", CircleParamList[8][0].F + 30, CircleParamList[8][1].F + 0, "false");

                    HObject _circle = new HObject();
                    HOperatorSet.GenCircle(out _circle, CircleParamList[0][0].F, CircleParamList[0][1].F, CircleParamList[0][2].F);
                    hw_S3.viewWindow.displayHobject(_circle, "green");
                    _circle.Dispose();
                    HOperatorSet.GenCircle(out _circle, CircleParamList[1][0].F, CircleParamList[1][1].F, CircleParamList[1][2].F);
                    hw_S3.viewWindow.displayHobject(_circle, "green");
                    _circle.Dispose();
                    HOperatorSet.GenCircle(out _circle, CircleParamList[2][0].F, CircleParamList[2][1].F, CircleParamList[2][2].F);
                    hw_S3.viewWindow.displayHobject(_circle, "green");
                    _circle.Dispose();
                    HOperatorSet.GenCircle(out _circle, CircleParamList[3][0].F, CircleParamList[3][1].F, CircleParamList[3][2].F);
                    hw_S3.viewWindow.displayHobject(_circle, "green");
                    _circle.Dispose();
                    HOperatorSet.GenCircle(out _circle, CircleParamList[4][0].F, CircleParamList[4][1].F, CircleParamList[4][2].F);
                    hw_S3.viewWindow.displayHobject(_circle, "green");
                    _circle.Dispose();
                    HOperatorSet.GenCircle(out _circle, CircleParamList[5][0].F, CircleParamList[5][1].F, CircleParamList[5][2].F);
                    hw_S3.viewWindow.displayHobject(_circle, "green");
                    _circle.Dispose();
                    HOperatorSet.GenCircle(out _circle, CircleParamList[6][0].F, CircleParamList[6][1].F, CircleParamList[6][2].F);
                    hw_S3.viewWindow.displayHobject(_circle, "green");
                    _circle.Dispose();
                    HOperatorSet.GenCircle(out _circle, CircleParamList[7][0].F, CircleParamList[7][1].F, CircleParamList[7][2].F);
                    hw_S3.viewWindow.displayHobject(_circle, "green");
                    _circle.Dispose();
                    HOperatorSet.GenCircle(out _circle, CircleParamList[8][0].F, CircleParamList[8][1].F, CircleParamList[8][2].F);
                    hw_S3.viewWindow.displayHobject(_circle, "green");
                    _circle.Dispose();


                    newMinimumEnclosingRectangle.set_display_font(ActionForm.hw_S3.hWindowControl.HalconWindow, 20, "mono", "false", "false");
                    ActionForm.hw_S3.viewWindow.dispMessage("1", "green", CircleParamList[0][0].F, CircleParamList[0][1].F, "false");
                    ActionForm.hw_S3.viewWindow.dispMessage("2", "green", CircleParamList[1][0].F, CircleParamList[1][1].F, "false");
                    ActionForm.hw_S3.viewWindow.dispMessage("3", "green", CircleParamList[2][0].F, CircleParamList[2][1].F, "false");
                    ActionForm.hw_S3.viewWindow.dispMessage("4", "green", CircleParamList[3][0].F, CircleParamList[3][1].F, "false");
                    ActionForm.hw_S3.viewWindow.dispMessage("5", "green", CircleParamList[4][0].F, CircleParamList[4][1].F, "false");
                    ActionForm.hw_S3.viewWindow.dispMessage("6", "green", CircleParamList[5][0].F, CircleParamList[5][1].F, "false");
                    ActionForm.hw_S3.viewWindow.dispMessage("7", "green", CircleParamList[6][0].F, CircleParamList[6][1].F, "false");
                    ActionForm.hw_S3.viewWindow.dispMessage("8", "green", CircleParamList[7][0].F, CircleParamList[7][1].F, "false");
                    ActionForm.hw_S3.viewWindow.dispMessage("9", "green", CircleParamList[8][0].F, CircleParamList[8][1].F, "false");
                    newMinimumEnclosingRectangle.set_display_font(ActionForm.hw_S3.hWindowControl.HalconWindow, 16, "mono", "false", "false");
                    ActionForm.hw_S3.viewWindow.dispMessage("[" + CircleParamList[0][0].F.ToString() + "," + CircleParamList[0][1].F.ToString() + "]", "green", CircleParamList[0][0].F + 30, CircleParamList[0][1].F + 0, "false");
                    ActionForm.hw_S3.viewWindow.dispMessage("[" + CircleParamList[1][0].F.ToString() + "," + CircleParamList[1][1].F.ToString() + "]", "green", CircleParamList[1][0].F + 30, CircleParamList[1][1].F + 0, "false");
                    ActionForm.hw_S3.viewWindow.dispMessage("[" + CircleParamList[2][0].F.ToString() + "," + CircleParamList[2][1].F.ToString() + "]", "green", CircleParamList[2][0].F + 30, CircleParamList[2][1].F + 0, "false");
                    ActionForm.hw_S3.viewWindow.dispMessage("[" + CircleParamList[3][0].F.ToString() + "," + CircleParamList[3][1].F.ToString() + "]", "green", CircleParamList[3][0].F + 30, CircleParamList[3][1].F + 0, "false");
                    ActionForm.hw_S3.viewWindow.dispMessage("[" + CircleParamList[4][0].F.ToString() + "," + CircleParamList[4][1].F.ToString() + "]", "green", CircleParamList[4][0].F + 30, CircleParamList[4][1].F + 0, "false");
                    ActionForm.hw_S3.viewWindow.dispMessage("[" + CircleParamList[5][0].F.ToString() + "," + CircleParamList[5][1].F.ToString() + "]", "green", CircleParamList[5][0].F + 30, CircleParamList[5][1].F + 0, "false");
                    ActionForm.hw_S3.viewWindow.dispMessage("[" + CircleParamList[6][0].F.ToString() + "," + CircleParamList[6][1].F.ToString() + "]", "green", CircleParamList[6][0].F + 30, CircleParamList[6][1].F + 0, "false");
                    ActionForm.hw_S3.viewWindow.dispMessage("[" + CircleParamList[7][0].F.ToString() + "," + CircleParamList[7][1].F.ToString() + "]", "green", CircleParamList[7][0].F + 30, CircleParamList[7][1].F + 0, "false");
                    ActionForm.hw_S3.viewWindow.dispMessage("[" + CircleParamList[8][0].F.ToString() + "," + CircleParamList[8][1].F.ToString() + "]", "green", CircleParamList[8][0].F + 30, CircleParamList[8][1].F + 0, "false");

                    HOperatorSet.GenCircle(out _circle, CircleParamList[0][0].F, CircleParamList[0][1].F, CircleParamList[0][2].F);
                    ActionForm.hw_S3.viewWindow.displayHobject(_circle, "green");
                    _circle.Dispose();
                    HOperatorSet.GenCircle(out _circle, CircleParamList[1][0].F, CircleParamList[1][1].F, CircleParamList[1][2].F);
                    ActionForm.hw_S3.viewWindow.displayHobject(_circle, "green");
                    _circle.Dispose();
                    HOperatorSet.GenCircle(out _circle, CircleParamList[2][0].F, CircleParamList[2][1].F, CircleParamList[2][2].F);
                    ActionForm.hw_S3.viewWindow.displayHobject(_circle, "green");
                    _circle.Dispose();
                    HOperatorSet.GenCircle(out _circle, CircleParamList[3][0].F, CircleParamList[3][1].F, CircleParamList[3][2].F);
                    ActionForm.hw_S3.viewWindow.displayHobject(_circle, "green");
                    _circle.Dispose();
                    HOperatorSet.GenCircle(out _circle, CircleParamList[4][0].F, CircleParamList[4][1].F, CircleParamList[4][2].F);
                    ActionForm.hw_S3.viewWindow.displayHobject(_circle, "green");
                    _circle.Dispose();
                    HOperatorSet.GenCircle(out _circle, CircleParamList[5][0].F, CircleParamList[5][1].F, CircleParamList[5][2].F);
                    ActionForm.hw_S3.viewWindow.displayHobject(_circle, "green");
                    _circle.Dispose();
                    HOperatorSet.GenCircle(out _circle, CircleParamList[6][0].F, CircleParamList[6][1].F, CircleParamList[6][2].F);
                    ActionForm.hw_S3.viewWindow.displayHobject(_circle, "green");
                    _circle.Dispose();
                    HOperatorSet.GenCircle(out _circle, CircleParamList[7][0].F, CircleParamList[7][1].F, CircleParamList[7][2].F);
                    ActionForm.hw_S3.viewWindow.displayHobject(_circle, "green");
                    _circle.Dispose();
                    HOperatorSet.GenCircle(out _circle, CircleParamList[8][0].F, CircleParamList[8][1].F, CircleParamList[8][2].F);
                    ActionForm.hw_S3.viewWindow.displayHobject(_circle, "green");
                    _circle.Dispose();

                }
                catch (Exception ex)
                {
                    MessageBox.Show("S3 9点标定显示找到的圆失败!");
                }
            }

            Pixel_X1_9PCalibrate = CircleParamList[0][0].F;
            Pixel_Y1_9PCalibrate = CircleParamList[0][1].F;
            Pixel_X2_9PCalibrate = CircleParamList[1][0].F;
            Pixel_Y2_9PCalibrate = CircleParamList[1][1].F;
            Pixel_X3_9PCalibrate = CircleParamList[2][0].F;
            Pixel_Y3_9PCalibrate = CircleParamList[2][1].F;
            Pixel_X4_9PCalibrate = CircleParamList[3][0].F;
            Pixel_Y4_9PCalibrate = CircleParamList[3][1].F;
            Pixel_X5_9PCalibrate = CircleParamList[4][0].F;
            Pixel_Y5_9PCalibrate = CircleParamList[4][1].F;
            Pixel_X6_9PCalibrate = CircleParamList[5][0].F;
            Pixel_Y6_9PCalibrate = CircleParamList[5][1].F;
            Pixel_X7_9PCalibrate = CircleParamList[6][0].F;
            Pixel_Y7_9PCalibrate = CircleParamList[6][1].F;
            Pixel_X8_9PCalibrate = CircleParamList[7][0].F;
            Pixel_Y8_9PCalibrate = CircleParamList[7][1].F;
            Pixel_X9_9PCalibrate = CircleParamList[8][0].F;
            Pixel_Y9_9PCalibrate = CircleParamList[8][1].F;
            return true;
        }
        #endregion

        #endregion

        #region 产品参数示教相关
        #region btnROB_PickupTeachPosition_Click
        private void btnROB_PickupTeachPosition_S1_Click(object sender, EventArgs e)
        {
            if (ActionForm._accessLevel < 101)
            {
                MessageBox.Show("操作权限不合格:操作权限为工程师或者厂家!");
                return;
            }

            //检查与ROB通讯是否正常
            if (!ActionForm._socketServerStartOKAndConnectROBOKFlag)
            {
                MessageBox.Show("与机械手通讯失败!");
                return;
            }
            ROB_XPickupTeachPosition_S1 = ActionForm.ROB_XCurrentPosition;
            ROB_YPickupTeachPosition_S1 = ActionForm.ROB_YCurrentPosition;
            ROB_ZPickupTeachPosition_S1 = ActionForm.ROB_ZCurrentPosition;
            ROB_RXPickupTeachPosition_S1 = ActionForm.ROB_RXCurrentPosition;
            ROB_RWPickupTeachPosition_S1 = ActionForm.ROB_RWCurrentPosition;
            ROB_RZPickupTeachPosition_S1 = ActionForm.ROB_RZCurrentPosition;
            txtROB_XPickupTeachPosition_S1.Text = ROB_XPickupTeachPosition_S1.ToString("0000.000");
            txtROB_YPickupTeachPosition_S1.Text = ROB_YPickupTeachPosition_S1.ToString("0000.000");
            txtROB_ZPickupTeachPosition_S1.Text = ROB_ZPickupTeachPosition_S1.ToString("0000.000");
            txtROB_RXPickupTeachPosition_S1.Text = ROB_RXPickupTeachPosition_S1.ToString("0000.000");
            txtROB_RWPickupTeachPosition_S1.Text = ROB_RWPickupTeachPosition_S1.ToString("0000.000");
            txtROB_RZPickupTeachPosition_S1.Text = ROB_RZPickupTeachPosition_S1.ToString("0000.000");
        }

        private void btnROB_PickupTeachPosition_S2_Click(object sender, EventArgs e)
        {
            if (ActionForm._accessLevel < 101)
            {
                MessageBox.Show("操作权限不合格:操作权限为工程师或者厂家!");
                return;
            }

            //检查与ROB通讯是否正常
            if (!ActionForm._socketServerStartOKAndConnectROBOKFlag)
            {
                MessageBox.Show("与机械手通讯失败!");
                return;
            }
            ROB_XPickupTeachPosition_S2 = ActionForm.ROB_XCurrentPosition;
            ROB_YPickupTeachPosition_S2 = ActionForm.ROB_YCurrentPosition;
            ROB_ZPickupTeachPosition_S2 = ActionForm.ROB_ZCurrentPosition;
            ROB_RXPickupTeachPosition_S2 = ActionForm.ROB_RXCurrentPosition;
            ROB_RWPickupTeachPosition_S2 = ActionForm.ROB_RWCurrentPosition;
            ROB_RZPickupTeachPosition_S2 = ActionForm.ROB_RZCurrentPosition;
            txtROB_XPickupTeachPosition_S2.Text = ROB_XPickupTeachPosition_S2.ToString("0000.000");
            txtROB_YPickupTeachPosition_S2.Text = ROB_YPickupTeachPosition_S2.ToString("0000.000");
            txtROB_ZPickupTeachPosition_S2.Text = ROB_ZPickupTeachPosition_S2.ToString("0000.000");
            txtROB_RXPickupTeachPosition_S2.Text = ROB_RXPickupTeachPosition_S2.ToString("0000.000");
            txtROB_RWPickupTeachPosition_S2.Text = ROB_RWPickupTeachPosition_S2.ToString("0000.000");
            txtROB_RZPickupTeachPosition_S2.Text = ROB_RZPickupTeachPosition_S2.ToString("0000.000");
        }

        private void btnROB_PickupTeachPosition_S3_Click(object sender, EventArgs e)
        {
            if (ActionForm._accessLevel < 101)
            {
                MessageBox.Show("操作权限不合格:操作权限为工程师或者厂家!");
                return;
            }

            //检查与ROB通讯是否正常
            if (!ActionForm._socketServerStartOKAndConnectROBOKFlag)
            {
                MessageBox.Show("与机械手通讯失败!");
                return;
            }
            ROB_XPickupTeachPosition_S3 = ActionForm.ROB_XCurrentPosition;
            ROB_YPickupTeachPosition_S3 = ActionForm.ROB_YCurrentPosition;
            ROB_ZPickupTeachPosition_S3 = ActionForm.ROB_ZCurrentPosition;
            ROB_RXPickupTeachPosition_S3 = ActionForm.ROB_RXCurrentPosition;
            ROB_RWPickupTeachPosition_S3 = ActionForm.ROB_RWCurrentPosition;
            ROB_RZPickupTeachPosition_S3 = ActionForm.ROB_RZCurrentPosition;
            txtROB_XPickupTeachPosition_S3.Text = ROB_XPickupTeachPosition_S3.ToString("0000.000");
            txtROB_YPickupTeachPosition_S3.Text = ROB_YPickupTeachPosition_S3.ToString("0000.000");
            txtROB_ZPickupTeachPosition_S3.Text = ROB_ZPickupTeachPosition_S3.ToString("0000.000");
            txtROB_RXPickupTeachPosition_S3.Text = ROB_RXPickupTeachPosition_S3.ToString("0000.000");
            txtROB_RWPickupTeachPosition_S3.Text = ROB_RWPickupTeachPosition_S3.ToString("0000.000");
            txtROB_RZPickupTeachPosition_S3.Text = ROB_RZPickupTeachPosition_S3.ToString("0000.000");
        }
        #endregion

        #region btnPixel_PickupTeachPosition_Click
        private void btnPixel_PickupTeachPosition_S1_Click(object sender, EventArgs e)
        {
            if (ActionForm._accessLevel < 101)
            {
                MessageBox.Show("操作权限不合格:操作权限为工程师或者厂家!");
                return;
            }

            //计算示教物料位置及角度数据
            if (teachProductPositionCalculate_S1())
            {
                txtPixel_XPickupTeachPosition_RT_S1.Text = Pixel_XPickupTeachPosition_RT_S1.ToString("0000.000");
                txtPixel_YPickupTeachPosition_RT_S1.Text = Pixel_YPickupTeachPosition_RT_S1.ToString("0000.000");
                txtPixel_XPickupTeachPosition_LT_S1.Text = Pixel_XPickupTeachPosition_LT_S1.ToString("0000.000");
                txtPixel_YPickupTeachPosition_LT_S1.Text = Pixel_YPickupTeachPosition_LT_S1.ToString("0000.000");
                txtPixel_XPickupTeachPosition_LB_S1.Text = Pixel_XPickupTeachPosition_LB_S1.ToString("0000.000");
                txtPixel_YPickupTeachPosition_LB_S1.Text = Pixel_YPickupTeachPosition_LB_S1.ToString("0000.000");
                txtPixel_XPickupTeachPosition_RB_S1.Text = Pixel_XPickupTeachPosition_RB_S1.ToString("0000.000");
                txtPixel_YPickupTeachPosition_RB_S1.Text = Pixel_YPickupTeachPosition_RB_S1.ToString("0000.000");
                txtPixel_AnglePickupTeach_S1.Text = Pixel_AnglePickupTeach_S1.ToString("0000.000");
            }
        }

        private void btnPixel_PickupTeachPosition_S2_Click(object sender, EventArgs e)
        {
            if (ActionForm._accessLevel < 101)
            {
                MessageBox.Show("操作权限不合格:操作权限为工程师或者厂家!");
                return;
            }

            //计算示教物料位置及角度数据
            if (teachProductPositionCalculate_S2())
            {
                txtPixel_XPickupTeachPosition_RT_S2.Text = Pixel_XPickupTeachPosition_RT_S2.ToString("0000.000");
                txtPixel_YPickupTeachPosition_RT_S2.Text = Pixel_YPickupTeachPosition_RT_S2.ToString("0000.000");
                txtPixel_XPickupTeachPosition_LT_S2.Text = Pixel_XPickupTeachPosition_LT_S2.ToString("0000.000");
                txtPixel_YPickupTeachPosition_LT_S2.Text = Pixel_YPickupTeachPosition_LT_S2.ToString("0000.000");
                txtPixel_XPickupTeachPosition_LB_S2.Text = Pixel_XPickupTeachPosition_LB_S2.ToString("0000.000");
                txtPixel_YPickupTeachPosition_LB_S2.Text = Pixel_YPickupTeachPosition_LB_S2.ToString("0000.000");
                txtPixel_XPickupTeachPosition_RB_S2.Text = Pixel_XPickupTeachPosition_RB_S2.ToString("0000.000");
                txtPixel_YPickupTeachPosition_RB_S2.Text = Pixel_YPickupTeachPosition_RB_S2.ToString("0000.000");
                txtPixel_AnglePickupTeach_S2.Text = Pixel_AnglePickupTeach_S2.ToString("0000.000");
            }
        }

        private void btnPixel_PickupTeachPosition_S3_Click(object sender, EventArgs e)
        {
            if (ActionForm._accessLevel < 101)
            {
                MessageBox.Show("操作权限不合格:操作权限为工程师或者厂家!");
                return;
            }

            //计算示教物料位置及角度数据
            if (teachProductPositionCalculate_S3())
            {
                txtPixel_XPickupTeachPosition_RT_S3.Text = Pixel_XPickupTeachPosition_RT_S3.ToString("0000.000");
                txtPixel_YPickupTeachPosition_RT_S3.Text = Pixel_YPickupTeachPosition_RT_S3.ToString("0000.000");
                txtPixel_XPickupTeachPosition_LT_S3.Text = Pixel_XPickupTeachPosition_LT_S3.ToString("0000.000");
                txtPixel_YPickupTeachPosition_LT_S3.Text = Pixel_YPickupTeachPosition_LT_S3.ToString("0000.000");
                txtPixel_XPickupTeachPosition_LB_S3.Text = Pixel_XPickupTeachPosition_LB_S3.ToString("0000.000");
                txtPixel_YPickupTeachPosition_LB_S3.Text = Pixel_YPickupTeachPosition_LB_S3.ToString("0000.000");
                txtPixel_XPickupTeachPosition_RB_S3.Text = Pixel_XPickupTeachPosition_RB_S3.ToString("0000.000");
                txtPixel_YPickupTeachPosition_RB_S3.Text = Pixel_YPickupTeachPosition_RB_S3.ToString("0000.000");
                txtPixel_AnglePickupTeach_S3.Text = Pixel_AnglePickupTeach_S3.ToString("0000.000");
            }
        }

        #endregion
        #endregion

        #region findRectangle:找矩形
        float[] findRectangleTemp_S1 = new float[13];
        float[] findRectangleTemp_S2 = new float[13];
        float[] findRectangleTemp_S3 = new float[13];
        private bool findRectangle_S1()
        {
            if (!ActionForm._autoModeFlag)//手动时读取控件里的数据作为参数
            {
                try
                {
                    ThresholdMinGray_S1 = Convert.ToSingle(txtThresholdMinGray_S1.Text);
                    ThresholdMaxGray_S1 = Convert.ToSingle(txtThresholdMaxGray_S1.Text);
                    OpeningWAndH_S1 = Convert.ToSingle(txtOpeningWAndH_S1.Text);
                    ShapeMinArea_S1 = Convert.ToSingle(txtShapeMinArea_S1.Text);
                    ShapeMaxArea_S1 = Convert.ToSingle(txtShapeMaxArea_S1.Text);
                    ShapeMinRectangularity_S1 = Convert.ToSingle(txtShapeMinRectangularity_S1.Text);
                    ShapeMaxRectangularity_S1 = Convert.ToSingle(txtShapeMaxRectangularity_S1.Text);
                    DilationRectangleWidth_S1 = Convert.ToSingle(txtDilationRectangleWidth_S1.Text);
                    DilationRectangleHeight_S1 = Convert.ToSingle(txtDilationRectangleHeight_S1.Text);
                    EdgesSubPixCannyAlpha_S1 = Convert.ToSingle(txtEdgesSubPixCannyAlpha_S1.Text);
                    EdgesSubPixCannyLow_S1 = Convert.ToSingle(txtEdgesSubPixCannyLow_S1.Text);
                    EdgesSubPixCannyHigh_S1 = Convert.ToSingle(txtEdgesSubPixCannyHigh_S1.Text);
                    UnionCollinearContoursXldMaxDistAbs_S1 = Convert.ToSingle(txtUnionCollinearContoursXldMaxDistAbs_S1.Text);
                    UnionCollinearContoursXldMaxDistRel_S1 = Convert.ToSingle(txtUnionCollinearContoursXldMaxDistRel_S1.Text);
                    UnionCollinearContoursXldMaxShift_S1 = Convert.ToSingle(txtUnionCollinearContoursXldMaxShift_S1.Text);
                    UnionCollinearContoursXldMaxAngle_S1 = Convert.ToSingle(txtUnionCollinearContoursXldMaxAngle_S1.Text);
                    XldContlengthMin_S1 = Convert.ToSingle(txtXldContlengthMin_S1.Text);
                    XldContlengthMax_S1 = Convert.ToSingle(txtXldContlengthMax_S1.Text);
                    XLDMinArea_S1 = Convert.ToSingle(txtXLDMinArea_S1.Text);
                    XLDMaxArea_S1 = Convert.ToSingle(txtXLDMaxArea_S1.Text);
                    XLDMinRectangularity_S1 = Convert.ToSingle(txtXLDMinRectangularity_S1.Text);
                    XLDMaxRectangularity_S1 = Convert.ToSingle(txtXLDMaxRectangularity_S1.Text);
                    UnionAdjacentContoursXldMaxDistAbs_S1 = Convert.ToSingle(txtUnionAdjacentContoursXldMaxDistAbs_S1.Text);
                    UnionAdjacentContoursXldMaxDistRel_S1 = Convert.ToSingle(txtUnionAdjacentContoursXldMaxDistRel_S1.Text);
                    AngleRotate_S1 = chkAngleRotate_S1.Checked;
                }
                catch (Exception ex)
                {
                    MyTool.TxtFileProcess.CreateLog("S1控件里输入的算子参数错误!" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("S1控件里输入的算子参数错误!");
                    return false;
                }
            }
            else//自动时读取产品参数文件数据
            {
                ThresholdMinGray_S1 = _currentProductRecipe.ThresholdMinGray_S1;
                ThresholdMaxGray_S1 = _currentProductRecipe.ThresholdMaxGray_S1;
                OpeningWAndH_S1 = _currentProductRecipe.OpeningWAndH_S1;
                ShapeMinArea_S1 = _currentProductRecipe.ShapeMinArea_S1;
                ShapeMaxArea_S1 = _currentProductRecipe.ShapeMaxArea_S1;
                ShapeMinRectangularity_S1 = _currentProductRecipe.ShapeMinRectangularity_S1;
                ShapeMaxRectangularity_S1 = _currentProductRecipe.ShapeMaxRectangularity_S1;
                DilationRectangleWidth_S1 = _currentProductRecipe.DilationRectangleWidth_S1;
                DilationRectangleHeight_S1 = _currentProductRecipe.DilationRectangleHeight_S1;
                EdgesSubPixCannyAlpha_S1 = _currentProductRecipe.EdgesSubPixCannyAlpha_S1;
                EdgesSubPixCannyLow_S1 = _currentProductRecipe.EdgesSubPixCannyLow_S1;
                EdgesSubPixCannyHigh_S1 = _currentProductRecipe.EdgesSubPixCannyHigh_S1;
                UnionCollinearContoursXldMaxDistAbs_S1 = _currentProductRecipe.UnionCollinearContoursXldMaxDistAbs_S1;
                UnionCollinearContoursXldMaxDistRel_S1 = _currentProductRecipe.UnionCollinearContoursXldMaxDistRel_S1;
                UnionCollinearContoursXldMaxShift_S1 = _currentProductRecipe.UnionCollinearContoursXldMaxShift_S1;
                UnionCollinearContoursXldMaxAngle_S1 = _currentProductRecipe.UnionCollinearContoursXldMaxAngle_S1;
                XldContlengthMin_S1 = _currentProductRecipe.XldContlengthMin_S1;
                XldContlengthMax_S1 = _currentProductRecipe.XldContlengthMax_S1;
                XLDMinArea_S1 = _currentProductRecipe.XLDMinArea_S1;
                XLDMaxArea_S1 = _currentProductRecipe.XLDMaxArea_S1;
                XLDMinRectangularity_S1 = _currentProductRecipe.XLDMinRectangularity_S1;
                XLDMaxRectangularity_S1 = _currentProductRecipe.XLDMaxRectangularity_S1;
                UnionAdjacentContoursXldMaxDistAbs_S1 = _currentProductRecipe.UnionAdjacentContoursXldMaxDistAbs_S1;
                UnionAdjacentContoursXldMaxDistRel_S1 = _currentProductRecipe.UnionAdjacentContoursXldMaxDistRel_S1;
                AngleRotate_S1 = _currentProductRecipe.AngleRotate_S1;
            }

            int result = newMinimumEnclosingRectangle.MERCal(hw_S1, ho_Image_S1, ThresholdMinGray_S1, ThresholdMaxGray_S1, OpeningWAndH_S1,
                ShapeMinArea_S1, ShapeMaxArea_S1, ShapeMinRectangularity_S1, ShapeMaxRectangularity_S1,
                DilationRectangleWidth_S1, DilationRectangleHeight_S1,
                EdgesSubPixCannyAlpha_S1, EdgesSubPixCannyLow_S1, EdgesSubPixCannyHigh_S1,
                UnionCollinearContoursXldMaxDistAbs_S1, UnionCollinearContoursXldMaxDistRel_S1, UnionCollinearContoursXldMaxShift_S1, UnionCollinearContoursXldMaxAngle_S1,
                XldContlengthMin_S1, XldContlengthMax_S1,
                XLDMinArea_S1, XLDMaxArea_S1, XLDMinRectangularity_S1, XLDMaxRectangularity_S1,
                UnionAdjacentContoursXldMaxDistAbs_S1, UnionAdjacentContoursXldMaxDistRel_S1,
                ref findRectangleTemp_S1, AngleRotate_S1, true);

            MyTool.TxtFileProcess.CreateLog("S1找矩形结果:" + result.ToString() + "----用户：" + ActionForm._operatorName,
            ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);

            if (result != 0)
            {
                if (result == 1)
                {
                    MyTool.TxtFileProcess.CreateLog("S1找到多个矩形!" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    //MessageBox.Show("S1找到多个矩形!");
                }
                else
                {
                    MyTool.TxtFileProcess.CreateLog("S1找矩形失败!" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    //MessageBox.Show("S1找矩形失败!");
                }
                try
                {
                    string path = _ProductDataDirectoryPath + "\\" + "S1_" + string.Format("{0:yyyyMMddHHmmss}", DateTime.Now);
                    if (HalconCameraControl.SaveImage(ho_Image_S1, "jpg", path))
                    {
                        MyTool.TxtFileProcess.CreateLog("S1已保存找矩形失败的图片:" + path + ".jpg" + "----用户：" + ActionForm._operatorName,
                        ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    }
                    else
                    {
                        MyTool.TxtFileProcess.CreateLog("S1保存找矩形失败的图片不成功!" + "----用户：" + ActionForm._operatorName,
                        ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    }
                }
                catch (Exception ex)
                {

                }
                return false;
            }
            return true;
        }
        private bool findRectangle_S2()
        {
            if (!ActionForm._autoModeFlag)//手动时读取控件里的数据作为参数
            {
                try
                {
                    ThresholdMinGray_S2 = Convert.ToSingle(txtThresholdMinGray_S2.Text);
                    ThresholdMaxGray_S2 = Convert.ToSingle(txtThresholdMaxGray_S2.Text);
                    OpeningWAndH_S2 = Convert.ToSingle(txtOpeningWAndH_S2.Text);
                    ShapeMinArea_S2 = Convert.ToSingle(txtShapeMinArea_S2.Text);
                    ShapeMaxArea_S2 = Convert.ToSingle(txtShapeMaxArea_S2.Text);
                    ShapeMinRectangularity_S2 = Convert.ToSingle(txtShapeMinRectangularity_S2.Text);
                    ShapeMaxRectangularity_S2 = Convert.ToSingle(txtShapeMaxRectangularity_S2.Text);
                    DilationRectangleWidth_S2 = Convert.ToSingle(txtDilationRectangleWidth_S2.Text);
                    DilationRectangleHeight_S2 = Convert.ToSingle(txtDilationRectangleHeight_S2.Text);
                    EdgesSubPixCannyAlpha_S2 = Convert.ToSingle(txtEdgesSubPixCannyAlpha_S2.Text);
                    EdgesSubPixCannyLow_S2 = Convert.ToSingle(txtEdgesSubPixCannyLow_S2.Text);
                    EdgesSubPixCannyHigh_S2 = Convert.ToSingle(txtEdgesSubPixCannyHigh_S2.Text);
                    UnionCollinearContoursXldMaxDistAbs_S2 = Convert.ToSingle(txtUnionCollinearContoursXldMaxDistAbs_S2.Text);
                    UnionCollinearContoursXldMaxDistRel_S2 = Convert.ToSingle(txtUnionCollinearContoursXldMaxDistRel_S2.Text);
                    UnionCollinearContoursXldMaxShift_S2 = Convert.ToSingle(txtUnionCollinearContoursXldMaxShift_S2.Text);
                    UnionCollinearContoursXldMaxAngle_S2 = Convert.ToSingle(txtUnionCollinearContoursXldMaxAngle_S2.Text);
                    XldContlengthMin_S2 = Convert.ToSingle(txtXldContlengthMin_S2.Text);
                    XldContlengthMax_S2 = Convert.ToSingle(txtXldContlengthMax_S2.Text);
                    XLDMinArea_S2 = Convert.ToSingle(txtXLDMinArea_S2.Text);
                    XLDMaxArea_S2 = Convert.ToSingle(txtXLDMaxArea_S2.Text);
                    XLDMinRectangularity_S2 = Convert.ToSingle(txtXLDMinRectangularity_S2.Text);
                    XLDMaxRectangularity_S2 = Convert.ToSingle(txtXLDMaxRectangularity_S2.Text);
                    UnionAdjacentContoursXldMaxDistAbs_S2 = Convert.ToSingle(txtUnionAdjacentContoursXldMaxDistAbs_S2.Text);
                    UnionAdjacentContoursXldMaxDistRel_S2 = Convert.ToSingle(txtUnionAdjacentContoursXldMaxDistRel_S2.Text);
                    AngleRotate_S2 = chkAngleRotate_S2.Checked;
                }
                catch (Exception ex)
                {
                    MyTool.TxtFileProcess.CreateLog("S2控件里输入的算子参数错误!" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("S2控件里输入的算子参数错误!");
                    return false;
                }
            }
            else//自动时读取产品参数文件数据
            {
                ThresholdMinGray_S2 = _currentProductRecipe.ThresholdMinGray_S2;
                ThresholdMaxGray_S2 = _currentProductRecipe.ThresholdMaxGray_S2;
                OpeningWAndH_S2 = _currentProductRecipe.OpeningWAndH_S2;
                ShapeMinArea_S2 = _currentProductRecipe.ShapeMinArea_S2;
                ShapeMaxArea_S2 = _currentProductRecipe.ShapeMaxArea_S2;
                ShapeMinRectangularity_S2 = _currentProductRecipe.ShapeMinRectangularity_S2;
                ShapeMaxRectangularity_S2 = _currentProductRecipe.ShapeMaxRectangularity_S2;
                DilationRectangleWidth_S2 = _currentProductRecipe.DilationRectangleWidth_S2;
                DilationRectangleHeight_S2 = _currentProductRecipe.DilationRectangleHeight_S2;
                EdgesSubPixCannyAlpha_S2 = _currentProductRecipe.EdgesSubPixCannyAlpha_S2;
                EdgesSubPixCannyLow_S2 = _currentProductRecipe.EdgesSubPixCannyLow_S2;
                EdgesSubPixCannyHigh_S2 = _currentProductRecipe.EdgesSubPixCannyHigh_S2;
                UnionCollinearContoursXldMaxDistAbs_S2 = _currentProductRecipe.UnionCollinearContoursXldMaxDistAbs_S2;
                UnionCollinearContoursXldMaxDistRel_S2 = _currentProductRecipe.UnionCollinearContoursXldMaxDistRel_S2;
                UnionCollinearContoursXldMaxShift_S2 = _currentProductRecipe.UnionCollinearContoursXldMaxShift_S2;
                UnionCollinearContoursXldMaxAngle_S2 = _currentProductRecipe.UnionCollinearContoursXldMaxAngle_S2;
                XldContlengthMin_S2 = _currentProductRecipe.XldContlengthMin_S2;
                XldContlengthMax_S2 = _currentProductRecipe.XldContlengthMax_S2;
                XLDMinArea_S2 = _currentProductRecipe.XLDMinArea_S2;
                XLDMaxArea_S2 = _currentProductRecipe.XLDMaxArea_S2;
                XLDMinRectangularity_S2 = _currentProductRecipe.XLDMinRectangularity_S2;
                XLDMaxRectangularity_S2 = _currentProductRecipe.XLDMaxRectangularity_S2;
                UnionAdjacentContoursXldMaxDistAbs_S2 = _currentProductRecipe.UnionAdjacentContoursXldMaxDistAbs_S2;
                UnionAdjacentContoursXldMaxDistRel_S2 = _currentProductRecipe.UnionAdjacentContoursXldMaxDistRel_S2;
                AngleRotate_S2 = _currentProductRecipe.AngleRotate_S2;
            }

            int result = newMinimumEnclosingRectangle.MERCal(hw_S2, ho_Image_S2, ThresholdMinGray_S2, ThresholdMaxGray_S2, OpeningWAndH_S2,
                ShapeMinArea_S2, ShapeMaxArea_S2, ShapeMinRectangularity_S2, ShapeMaxRectangularity_S2,
                DilationRectangleWidth_S2, DilationRectangleHeight_S2,
                EdgesSubPixCannyAlpha_S2, EdgesSubPixCannyLow_S2, EdgesSubPixCannyHigh_S2,
                UnionCollinearContoursXldMaxDistAbs_S2, UnionCollinearContoursXldMaxDistRel_S2, UnionCollinearContoursXldMaxShift_S2, UnionCollinearContoursXldMaxAngle_S2,
                XldContlengthMin_S2, XldContlengthMax_S2,
                XLDMinArea_S2, XLDMaxArea_S2, XLDMinRectangularity_S2, XLDMaxRectangularity_S2,
                UnionAdjacentContoursXldMaxDistAbs_S2, UnionAdjacentContoursXldMaxDistRel_S2,
                ref findRectangleTemp_S2, AngleRotate_S2, true);

            MyTool.TxtFileProcess.CreateLog("S2找矩形结果:" + result.ToString() + "----用户：" + ActionForm._operatorName,
            ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);

            if (result != 0)
            {
                if (result == 1)
                {
                    MyTool.TxtFileProcess.CreateLog("S2找到多个矩形!" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    //MessageBox.Show("S2找到多个矩形!");
                }
                else
                {
                    MyTool.TxtFileProcess.CreateLog("S2找矩形失败!" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    //MessageBox.Show("S2找矩形失败!");
                }
                try
                {
                    string path = _ProductDataDirectoryPath + "\\" + "S2_" + string.Format("{0:yyyyMMddHHmmss}", DateTime.Now);
                    if (HalconCameraControl.SaveImage(ho_Image_S2, "jpg", path))
                    {
                        MyTool.TxtFileProcess.CreateLog("S2已保存找矩形失败的图片:" + path + ".jpg" + "----用户：" + ActionForm._operatorName,
                        ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    }
                    else
                    {
                        MyTool.TxtFileProcess.CreateLog("S2保存找矩形失败的图片不成功!" + "----用户：" + ActionForm._operatorName,
                        ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    }
                }
                catch (Exception ex)
                {

                }
                return false;
            }
            return true;
        }
        private bool findRectangle_S3()
        {
            if (!ActionForm._autoModeFlag)//手动时读取控件里的数据作为参数
            {
                try
                {
                    ThresholdMinGray_S3 = Convert.ToSingle(txtThresholdMinGray_S3.Text);
                    ThresholdMaxGray_S3 = Convert.ToSingle(txtThresholdMaxGray_S3.Text);
                    OpeningWAndH_S3 = Convert.ToSingle(txtOpeningWAndH_S3.Text);
                    ShapeMinArea_S3 = Convert.ToSingle(txtShapeMinArea_S3.Text);
                    ShapeMaxArea_S3 = Convert.ToSingle(txtShapeMaxArea_S3.Text);
                    ShapeMinRectangularity_S3 = Convert.ToSingle(txtShapeMinRectangularity_S3.Text);
                    ShapeMaxRectangularity_S3 = Convert.ToSingle(txtShapeMaxRectangularity_S3.Text);
                    DilationRectangleWidth_S3 = Convert.ToSingle(txtDilationRectangleWidth_S3.Text);
                    DilationRectangleHeight_S3 = Convert.ToSingle(txtDilationRectangleHeight_S3.Text);
                    EdgesSubPixCannyAlpha_S3 = Convert.ToSingle(txtEdgesSubPixCannyAlpha_S3.Text);
                    EdgesSubPixCannyLow_S3 = Convert.ToSingle(txtEdgesSubPixCannyLow_S3.Text);
                    EdgesSubPixCannyHigh_S3 = Convert.ToSingle(txtEdgesSubPixCannyHigh_S3.Text);
                    UnionCollinearContoursXldMaxDistAbs_S3 = Convert.ToSingle(txtUnionCollinearContoursXldMaxDistAbs_S3.Text);
                    UnionCollinearContoursXldMaxDistRel_S3 = Convert.ToSingle(txtUnionCollinearContoursXldMaxDistRel_S3.Text);
                    UnionCollinearContoursXldMaxShift_S3 = Convert.ToSingle(txtUnionCollinearContoursXldMaxShift_S3.Text);
                    UnionCollinearContoursXldMaxAngle_S3 = Convert.ToSingle(txtUnionCollinearContoursXldMaxAngle_S3.Text);
                    XldContlengthMin_S3 = Convert.ToSingle(txtXldContlengthMin_S3.Text);
                    XldContlengthMax_S3 = Convert.ToSingle(txtXldContlengthMax_S3.Text);
                    XLDMinArea_S3 = Convert.ToSingle(txtXLDMinArea_S3.Text);
                    XLDMaxArea_S3 = Convert.ToSingle(txtXLDMaxArea_S3.Text);
                    XLDMinRectangularity_S3 = Convert.ToSingle(txtXLDMinRectangularity_S3.Text);
                    XLDMaxRectangularity_S3 = Convert.ToSingle(txtXLDMaxRectangularity_S3.Text);
                    UnionAdjacentContoursXldMaxDistAbs_S3 = Convert.ToSingle(txtUnionAdjacentContoursXldMaxDistAbs_S3.Text);
                    UnionAdjacentContoursXldMaxDistRel_S3 = Convert.ToSingle(txtUnionAdjacentContoursXldMaxDistRel_S3.Text);
                    AngleRotate_S3 = chkAngleRotate_S3.Checked;
                }
                catch (Exception ex)
                {
                    MyTool.TxtFileProcess.CreateLog("S3控件里输入的算子参数错误!" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("S3控件里输入的算子参数错误!");
                    return false;
                }
            }
            else//自动时读取产品参数文件数据
            {
                ThresholdMinGray_S3 = _currentProductRecipe.ThresholdMinGray_S3;
                ThresholdMaxGray_S3 = _currentProductRecipe.ThresholdMaxGray_S3;
                OpeningWAndH_S3 = _currentProductRecipe.OpeningWAndH_S3;
                ShapeMinArea_S3 = _currentProductRecipe.ShapeMinArea_S3;
                ShapeMaxArea_S3 = _currentProductRecipe.ShapeMaxArea_S3;
                ShapeMinRectangularity_S3 = _currentProductRecipe.ShapeMinRectangularity_S3;
                ShapeMaxRectangularity_S3 = _currentProductRecipe.ShapeMaxRectangularity_S3;
                DilationRectangleWidth_S3 = _currentProductRecipe.DilationRectangleWidth_S3;
                DilationRectangleHeight_S3 = _currentProductRecipe.DilationRectangleHeight_S3;
                EdgesSubPixCannyAlpha_S3 = _currentProductRecipe.EdgesSubPixCannyAlpha_S3;
                EdgesSubPixCannyLow_S3 = _currentProductRecipe.EdgesSubPixCannyLow_S3;
                EdgesSubPixCannyHigh_S3 = _currentProductRecipe.EdgesSubPixCannyHigh_S3;
                UnionCollinearContoursXldMaxDistAbs_S3 = _currentProductRecipe.UnionCollinearContoursXldMaxDistAbs_S3;
                UnionCollinearContoursXldMaxDistRel_S3 = _currentProductRecipe.UnionCollinearContoursXldMaxDistRel_S3;
                UnionCollinearContoursXldMaxShift_S3 = _currentProductRecipe.UnionCollinearContoursXldMaxShift_S3;
                UnionCollinearContoursXldMaxAngle_S3 = _currentProductRecipe.UnionCollinearContoursXldMaxAngle_S3;
                XldContlengthMin_S3 = _currentProductRecipe.XldContlengthMin_S3;
                XldContlengthMax_S3 = _currentProductRecipe.XldContlengthMax_S3;
                XLDMinArea_S3 = _currentProductRecipe.XLDMinArea_S3;
                XLDMaxArea_S3 = _currentProductRecipe.XLDMaxArea_S3;
                XLDMinRectangularity_S3 = _currentProductRecipe.XLDMinRectangularity_S3;
                XLDMaxRectangularity_S3 = _currentProductRecipe.XLDMaxRectangularity_S3;
                UnionAdjacentContoursXldMaxDistAbs_S3 = _currentProductRecipe.UnionAdjacentContoursXldMaxDistAbs_S3;
                UnionAdjacentContoursXldMaxDistRel_S3 = _currentProductRecipe.UnionAdjacentContoursXldMaxDistRel_S3;
                AngleRotate_S3 = _currentProductRecipe.AngleRotate_S3;
            }

            int result = newMinimumEnclosingRectangle.MERCal(hw_S3, ho_Image_S3, ThresholdMinGray_S3, ThresholdMaxGray_S3, OpeningWAndH_S3,
                ShapeMinArea_S3, ShapeMaxArea_S3, ShapeMinRectangularity_S3, ShapeMaxRectangularity_S3,
                DilationRectangleWidth_S3, DilationRectangleHeight_S3,
                EdgesSubPixCannyAlpha_S3, EdgesSubPixCannyLow_S3, EdgesSubPixCannyHigh_S3,
                UnionCollinearContoursXldMaxDistAbs_S3, UnionCollinearContoursXldMaxDistRel_S3, UnionCollinearContoursXldMaxShift_S3, UnionCollinearContoursXldMaxAngle_S3,
                XldContlengthMin_S3, XldContlengthMax_S3,
                XLDMinArea_S3, XLDMaxArea_S3, XLDMinRectangularity_S3, XLDMaxRectangularity_S3,
                UnionAdjacentContoursXldMaxDistAbs_S3, UnionAdjacentContoursXldMaxDistRel_S3,
                ref findRectangleTemp_S3, AngleRotate_S3, false);

            MyTool.TxtFileProcess.CreateLog("S3找矩形结果:" + result.ToString() + "----用户：" + ActionForm._operatorName,
            ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);

            if (result != 0)
            {
                if (result == 1)
                {
                    MyTool.TxtFileProcess.CreateLog("S3找到多个矩形!" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    //MessageBox.Show("S3找到多个矩形!");
                }
                else
                {
                    MyTool.TxtFileProcess.CreateLog("S3找矩形失败!" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    //MessageBox.Show("S3找矩形失败!");
                }
                try
                {
                    string path = _ProductDataDirectoryPath + "\\" + "S3_" + string.Format("{0:yyyyMMddHHmmss}", DateTime.Now);
                    if (HalconCameraControl.SaveImage(ho_Image_S3, "jpg", path))
                    {
                        MyTool.TxtFileProcess.CreateLog("S3已保存找矩形失败的图片:" + path + ".jpg" + "----用户：" + ActionForm._operatorName,
                        ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    }
                    else
                    {
                        MyTool.TxtFileProcess.CreateLog("S3保存找矩形失败的图片不成功!" + "----用户：" + ActionForm._operatorName,
                        ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    }
                }
                catch (Exception ex)
                {

                }
                return false;
            }
            return true;
        }
        #endregion

        #region ProductPositionCalculate:计算物料位置及角度数据
        private bool ProductPositionCalculate_S1(out float Pixel_XPosition_RT, out float Pixel_YPosition_RT, out float Pixel_XPosition_LT, out float Pixel_YPosition_LT,
                                                 out float Pixel_XPosition_LB, out float Pixel_YPosition_LB, out float Pixel_XPosition_RB, out float Pixel_YPosition_RB, out float Pixel_AnglePickup)
        {
            Pixel_XPosition_RT = 0;//物料的右上顶点X像素坐标
            Pixel_YPosition_RT = 0;//物料的右上顶点Y像素坐标
            Pixel_XPosition_LT = 0;//物料的左上顶点X像素坐标
            Pixel_YPosition_LT = 0;//物料的左上顶点Y像素坐标
            Pixel_XPosition_LB = 0;//物料的左下顶点X像素坐标
            Pixel_YPosition_LB = 0;//物料的左下顶点Y像素坐标
            Pixel_XPosition_RB = 0;//物料的右下顶点X像素坐标
            Pixel_YPosition_RB = 0;//物料的右下顶点Y像素坐标
            Pixel_AnglePickup = 0;//当前物料的像素坐标系角度

            if (!ActionForm._autoModeFlag && ho_Image_S1 == null)
            {
                MessageBox.Show("S1请先拍照或者打开照片");
                return false;
            }

            if (findRectangle_S1())
            {
                Pixel_XPosition_RT = findRectangleTemp_S1[5];//物料的右上顶点X像素坐标
                Pixel_YPosition_RT = findRectangleTemp_S1[9];//物料的右上顶点Y像素坐标
                Pixel_XPosition_LT = findRectangleTemp_S1[6];//物料的左上顶点X像素坐标
                Pixel_YPosition_LT = findRectangleTemp_S1[10];//物料的左上顶点Y像素坐标
                Pixel_XPosition_LB = findRectangleTemp_S1[7];//物料的左下顶点X像素坐标
                Pixel_YPosition_LB = findRectangleTemp_S1[11];//物料的左下顶点Y像素坐标
                Pixel_XPosition_RB = findRectangleTemp_S1[8];//物料的右下顶点X像素坐标
                Pixel_YPosition_RB = findRectangleTemp_S1[12];//物料的右下顶点Y像素坐标
                Pixel_AnglePickup = findRectangleTemp_S1[2];//当前物料的像素坐标系角度
                if (!newMinimumEnclosingRectangle.rectangleDataDisplay(hw_S1, ho_Image_S1, findRectangleTemp_S1, _GeneralParameterDirectoryPath + "\\" + "9PCalibrate_S1.mat", true))
                {
                    MessageBox.Show("S1 CalibrateForm画面矩形数据显示失败!");
                    return false;
                }
                if (!newMinimumEnclosingRectangle.rectangleDataDisplay(ActionForm.hw_S1, ho_Image_S1, findRectangleTemp_S1, _GeneralParameterDirectoryPath + "\\" + "9PCalibrate_S1.mat", true))
                {
                    MessageBox.Show("S1 ActionForm画面矩形数据显示失败!");
                    return false;
                }
                return true;
            }

            return false;
        }
        private bool ProductPositionCalculate_S2(out float Pixel_XPosition_RT, out float Pixel_YPosition_RT, out float Pixel_XPosition_LT, out float Pixel_YPosition_LT,
                                                 out float Pixel_XPosition_LB, out float Pixel_YPosition_LB, out float Pixel_XPosition_RB, out float Pixel_YPosition_RB, out float Pixel_AnglePickup)
        {
            Pixel_XPosition_RT = 0;//物料的右上顶点X像素坐标
            Pixel_YPosition_RT = 0;//物料的右上顶点Y像素坐标
            Pixel_XPosition_LT = 0;//物料的左上顶点X像素坐标
            Pixel_YPosition_LT = 0;//物料的左上顶点Y像素坐标
            Pixel_XPosition_LB = 0;//物料的左下顶点X像素坐标
            Pixel_YPosition_LB = 0;//物料的左下顶点Y像素坐标
            Pixel_XPosition_RB = 0;//物料的右下顶点X像素坐标
            Pixel_YPosition_RB = 0;//物料的右下顶点Y像素坐标
            Pixel_AnglePickup = 0;//当前物料的像素坐标系角度

            if (!ActionForm._autoModeFlag && ho_Image_S2 == null)
            {
                MessageBox.Show("S2请先拍照或者打开照片");
                return false;
            }

            if (findRectangle_S2())
            {
                Pixel_XPosition_RT = findRectangleTemp_S2[5];//物料的右上顶点X像素坐标
                Pixel_YPosition_RT = findRectangleTemp_S2[9];//物料的右上顶点Y像素坐标
                Pixel_XPosition_LT = findRectangleTemp_S2[6];//物料的左上顶点X像素坐标
                Pixel_YPosition_LT = findRectangleTemp_S2[10];//物料的左上顶点Y像素坐标
                Pixel_XPosition_LB = findRectangleTemp_S2[7];//物料的左下顶点X像素坐标
                Pixel_YPosition_LB = findRectangleTemp_S2[11];//物料的左下顶点Y像素坐标
                Pixel_XPosition_RB = findRectangleTemp_S2[8];//物料的右下顶点X像素坐标
                Pixel_YPosition_RB = findRectangleTemp_S2[12];//物料的右下顶点Y像素坐标
                Pixel_AnglePickup = findRectangleTemp_S2[2];//当前物料的像素坐标系角度
                if (!newMinimumEnclosingRectangle.rectangleDataDisplay(hw_S2, ho_Image_S2, findRectangleTemp_S2, _GeneralParameterDirectoryPath + "\\" + "9PCalibrate_S2.mat", true))
                {
                    MessageBox.Show("S2 CalibrateForm画面矩形数据显示失败!");
                    return false;
                }
                if (!newMinimumEnclosingRectangle.rectangleDataDisplay(ActionForm.hw_S2, ho_Image_S2, findRectangleTemp_S2, _GeneralParameterDirectoryPath + "\\" + "9PCalibrate_S2.mat", true))
                {
                    MessageBox.Show("S2 ActionForm画面矩形数据显示失败!");
                    return false;
                }
                return true;
            }

            return false;
        }
        private bool ProductPositionCalculate_S3(out float Pixel_XPosition_RT, out float Pixel_YPosition_RT, out float Pixel_XPosition_LT, out float Pixel_YPosition_LT,
                                                 out float Pixel_XPosition_LB, out float Pixel_YPosition_LB, out float Pixel_XPosition_RB, out float Pixel_YPosition_RB, out float Pixel_AnglePickup)
        {
            Pixel_XPosition_RT = 0;//物料的右上顶点X像素坐标
            Pixel_YPosition_RT = 0;//物料的右上顶点Y像素坐标
            Pixel_XPosition_LT = 0;//物料的左上顶点X像素坐标
            Pixel_YPosition_LT = 0;//物料的左上顶点Y像素坐标
            Pixel_XPosition_LB = 0;//物料的左下顶点X像素坐标
            Pixel_YPosition_LB = 0;//物料的左下顶点Y像素坐标
            Pixel_XPosition_RB = 0;//物料的右下顶点X像素坐标
            Pixel_YPosition_RB = 0;//物料的右下顶点Y像素坐标
            Pixel_AnglePickup = 0;//当前物料的像素坐标系角度

            if (!ActionForm._autoModeFlag && ho_Image_S3 == null)
            {
                MessageBox.Show("S3请先拍照或者打开照片");
                return false;
            }

            if (findRectangle_S3())
            {
                Pixel_XPosition_RT = findRectangleTemp_S3[5];//物料的右上顶点X像素坐标
                Pixel_YPosition_RT = findRectangleTemp_S3[9];//物料的右上顶点Y像素坐标
                Pixel_XPosition_LT = findRectangleTemp_S3[6];//物料的左上顶点X像素坐标
                Pixel_YPosition_LT = findRectangleTemp_S3[10];//物料的左上顶点Y像素坐标
                Pixel_XPosition_LB = findRectangleTemp_S3[7];//物料的左下顶点X像素坐标
                Pixel_YPosition_LB = findRectangleTemp_S3[11];//物料的左下顶点Y像素坐标
                Pixel_XPosition_RB = findRectangleTemp_S3[8];//物料的右下顶点X像素坐标
                Pixel_YPosition_RB = findRectangleTemp_S3[12];//物料的右下顶点Y像素坐标
                Pixel_AnglePickup = findRectangleTemp_S3[2];//当前物料的像素坐标系角度
                if (!newMinimumEnclosingRectangle.rectangleDataDisplay(hw_S3, ho_Image_S3, findRectangleTemp_S3, _GeneralParameterDirectoryPath + "\\" + "9PCalibrate_S3.mat", true))
                {
                    MessageBox.Show("S3 CalibrateForm画面矩形数据显示失败!");
                    return false;
                }
                if (!newMinimumEnclosingRectangle.rectangleDataDisplay(ActionForm.hw_S3, ho_Image_S3, findRectangleTemp_S3, _GeneralParameterDirectoryPath + "\\" + "9PCalibrate_S3.mat", true))
                {
                    MessageBox.Show("S3 ActionForm画面矩形数据显示失败!");
                    return false;
                }
                return true;
            }

            return false;
        }
        #endregion

        #region productPositionCoordTransform:把物料位置像素坐标转化为ROB坐标
        private bool productPositionCoordTransform_S1(float[] _XPrePos, float[] _YPrePos, ref float[] _XTransPos, ref float[] _YTransPos)
        {
            try
            {
                newRobCamCalibration.coordTransform(newRobCamCalibration.readDateRobAndCamCalib_9P(_GeneralParameterDirectoryPath + "//" + "9PCalibrate_S1.mat"), _XPrePos, _YPrePos, ref _XTransPos, ref _YTransPos);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
        private bool productPositionCoordTransform_S2(float[] _XPrePos, float[] _YPrePos, ref float[] _XTransPos, ref float[] _YTransPos)
        {
            try
            {
                newRobCamCalibration.coordTransform(newRobCamCalibration.readDateRobAndCamCalib_9P(_GeneralParameterDirectoryPath + "//" + "9PCalibrate_S2.mat"), _XPrePos, _YPrePos, ref _XTransPos, ref _YTransPos);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
        private bool productPositionCoordTransform_S3(float[] _XPrePos, float[] _YPrePos, ref float[] _XTransPos, ref float[] _YTransPos)
        {
            try
            {
                newRobCamCalibration.coordTransform(newRobCamCalibration.readDateRobAndCamCalib_9P(_GeneralParameterDirectoryPath + "//" + "9PCalibrate_S3.mat"), _XPrePos, _YPrePos, ref _XTransPos, ref _YTransPos);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
        #endregion

        #region teachProductCameraTrigger:计算示教物料的像素位置及角度数据
        private bool teachProductPositionCalculate_S1()
        {
            if (!ProductPositionCalculate_S1(out Pixel_XPickupTeachPosition_RT_S1, out Pixel_YPickupTeachPosition_RT_S1, out Pixel_XPickupTeachPosition_LT_S1, out Pixel_YPickupTeachPosition_LT_S1,
                                out Pixel_XPickupTeachPosition_LB_S1, out Pixel_YPickupTeachPosition_LB_S1, out Pixel_XPickupTeachPosition_RB_S1, out Pixel_YPickupTeachPosition_RB_S1, out Pixel_AnglePickupTeach_S1))
            {
                //MessageBox.Show("S1计算示教物料的像素位置及角度数据失败!");
                MyTool.TxtFileProcess.CreateLog("S1计算示教物料的像素位置及角度数据失败!" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                return false;
            }
            return true;
        }
        private bool teachProductPositionCalculate_S2()
        {
            if (!ProductPositionCalculate_S2(out Pixel_XPickupTeachPosition_RT_S2, out Pixel_YPickupTeachPosition_RT_S2, out Pixel_XPickupTeachPosition_LT_S2, out Pixel_YPickupTeachPosition_LT_S2,
                                out Pixel_XPickupTeachPosition_LB_S2, out Pixel_YPickupTeachPosition_LB_S2, out Pixel_XPickupTeachPosition_RB_S2, out Pixel_YPickupTeachPosition_RB_S2, out Pixel_AnglePickupTeach_S2))
            {
                //MessageBox.Show("S2计算示教物料的像素位置及角度数据失败!");
                MyTool.TxtFileProcess.CreateLog("S2计算示教物料的像素位置及角度数据失败!" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                return false;
            }
            return true;
        }
        private bool teachProductPositionCalculate_S3()
        {
            if (!ProductPositionCalculate_S3(out Pixel_XPickupTeachPosition_RT_S3, out Pixel_YPickupTeachPosition_RT_S3, out Pixel_XPickupTeachPosition_LT_S3, out Pixel_YPickupTeachPosition_LT_S3,
                                out Pixel_XPickupTeachPosition_LB_S3, out Pixel_YPickupTeachPosition_LB_S3, out Pixel_XPickupTeachPosition_RB_S3, out Pixel_YPickupTeachPosition_RB_S3, out Pixel_AnglePickupTeach_S3))
            {
                //MessageBox.Show("S3计算示教物料的像素位置及角度数据失败!");
                MyTool.TxtFileProcess.CreateLog("S3计算示教物料的像素位置及角度数据失败!" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                return false;
            }
            return true;
        }
        #endregion

        #region currentProductPositionCalculate:计算当前物料的像素位置及角度数据
        private bool currentProductPositionCalculate_S1()
        {
            if (!ProductPositionCalculate_S1(out Pixel_XPickupCurrentPosition_RT_S1, out Pixel_YPickupCurrentPosition_RT_S1, out Pixel_XPickupCurrentPosition_LT_S1, out Pixel_YPickupCurrentPosition_LT_S1,
                                out Pixel_XPickupCurrentPosition_LB_S1, out Pixel_YPickupCurrentPosition_LB_S1, out Pixel_XPickupCurrentPosition_RB_S1, out Pixel_YPickupCurrentPosition_RB_S1, out Pixel_AnglePickupCurrent_S1))
            {
                //MessageBox.Show("S1计算当前物料的像素位置及角度数据失败!");
                MyTool.TxtFileProcess.CreateLog("S1计算当前物料的像素位置及角度数据失败!" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                return false;
            }
            return true;
        }
        private bool currentProductPositionCalculate_S2()
        {
            if (!ProductPositionCalculate_S2(out Pixel_XPickupCurrentPosition_RT_S2, out Pixel_YPickupCurrentPosition_RT_S2, out Pixel_XPickupCurrentPosition_LT_S2, out Pixel_YPickupCurrentPosition_LT_S2,
                                out Pixel_XPickupCurrentPosition_LB_S2, out Pixel_YPickupCurrentPosition_LB_S2, out Pixel_XPickupCurrentPosition_RB_S2, out Pixel_YPickupCurrentPosition_RB_S2, out Pixel_AnglePickupCurrent_S2))
            {
                //MessageBox.Show("S2计算当前物料的像素位置及角度数据失败!");
                MyTool.TxtFileProcess.CreateLog("S2计算当前物料的像素位置及角度数据失败!" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                return false;
            }
            return true;
        }
        private bool currentProductPositionCalculate_S3()
        {
            if (!ProductPositionCalculate_S3(out Pixel_XPickupCurrentPosition_RT_S3, out Pixel_YPickupCurrentPosition_RT_S3, out Pixel_XPickupCurrentPosition_LT_S3, out Pixel_YPickupCurrentPosition_LT_S3,
                                out Pixel_XPickupCurrentPosition_LB_S3, out Pixel_YPickupCurrentPosition_LB_S3, out Pixel_XPickupCurrentPosition_RB_S3, out Pixel_YPickupCurrentPosition_RB_S3, out Pixel_AnglePickupCurrent_S3))
            {
                //MessageBox.Show("S3计算当前物料的像素位置及角度数据失败!");
                MyTool.TxtFileProcess.CreateLog("S3计算当前物料的像素位置及角度数据失败!" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                return false;
            }
            return true;
        }
        #endregion

        #region currentProductPositionCheck:当前物料的像素位置及角度数据检查
        private int currentProductPositionCheck_S1()
        {
            if (!ActionForm._autoModeFlag)//手动时读取控件里的数据作为参数
            {
                try
                {
                    Pixel_XPickupCurrentPositionLimitMin_RT_S1 = Convert.ToSingle(txtPixel_XPickupCurrentPositionLimitMin_RT_S1.Text);
                    Pixel_XPickupCurrentPositionLimitMax_RT_S1 = Convert.ToSingle(txtPixel_XPickupCurrentPositionLimitMax_RT_S1.Text);
                    Pixel_YPickupCurrentPositionLimitMin_RT_S1 = Convert.ToSingle(txtPixel_YPickupCurrentPositionLimitMin_RT_S1.Text);
                    Pixel_YPickupCurrentPositionLimitMax_RT_S1 = Convert.ToSingle(txtPixel_YPickupCurrentPositionLimitMax_RT_S1.Text);
                    Pixel_AnglePickupCurrentLimitMin_S1 = Convert.ToSingle(txtPixel_AnglePickupCurrentLimitMin_S1.Text);
                    Pixel_AnglePickupCurrentLimitMax_S1 = Convert.ToSingle(txtPixel_AnglePickupCurrentLimitMax_S1.Text);
                }
                catch (Exception ex)
                {
                    //MessageBox.Show("S1控件里输入的来料像素位置限制参数错误!");
                    MyTool.TxtFileProcess.CreateLog("S1控件里输入的来料像素位置限制参数错误!" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    return -1;
                }
            }
            else//自动时读取产品参数文件数据
            {
                Pixel_XPickupCurrentPositionLimitMin_RT_S1 = _currentProductRecipe.Pixel_XPickupCurrentPositionLimitMin_RT_S1;
                Pixel_XPickupCurrentPositionLimitMax_RT_S1 = _currentProductRecipe.Pixel_XPickupCurrentPositionLimitMax_RT_S1;
                Pixel_YPickupCurrentPositionLimitMin_RT_S1 = _currentProductRecipe.Pixel_YPickupCurrentPositionLimitMin_RT_S1;
                Pixel_YPickupCurrentPositionLimitMax_RT_S1 = _currentProductRecipe.Pixel_YPickupCurrentPositionLimitMax_RT_S1;
                Pixel_AnglePickupCurrentLimitMin_S1 = _currentProductRecipe.Pixel_AnglePickupCurrentLimitMin_S1;
                Pixel_AnglePickupCurrentLimitMax_S1 = _currentProductRecipe.Pixel_AnglePickupCurrentLimitMax_S1;
            }

            if (Pixel_XPickupCurrentPosition_RT_S1 < Pixel_XPickupCurrentPositionLimitMin_RT_S1 || Pixel_XPickupCurrentPosition_RT_S1 > Pixel_XPickupCurrentPositionLimitMax_RT_S1)
            {
                //MessageBox.Show("S1当前物料右上角X像素坐标值超出限制条件的范围." + "当前值为:" +
                //    Pixel_XPickupCurrentPosition_RT_S1.ToString() + ",限制条件的范围是:" + "[" + Pixel_XPickupCurrentPositionLimitMin_RT_S1.ToString() + ","
                //    + Pixel_XPickupCurrentPositionLimitMax_RT_S1.ToString()
                //    + "]");
                MyTool.TxtFileProcess.CreateLog("S1当前物料右上角X像素坐标值超出限制条件的范围." + "当前值为:" +
                    Pixel_XPickupCurrentPosition_RT_S1.ToString() + ",限制条件的范围是:" + "[" + Pixel_XPickupCurrentPositionLimitMin_RT_S1.ToString() + ","
                    + Pixel_XPickupCurrentPositionLimitMax_RT_S1.ToString()
                    + "]" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                return 1;
            }
            if (Pixel_YPickupCurrentPosition_RT_S1 < Pixel_YPickupCurrentPositionLimitMin_RT_S1 || Pixel_YPickupCurrentPosition_RT_S1 > Pixel_YPickupCurrentPositionLimitMax_RT_S1)
            {
                //MessageBox.Show("S1当前物料右上角像Y素坐标值超出限制条件的范围." + "当前值为:" +
                //    Pixel_YPickupCurrentPosition_RT_S1.ToString() + ",限制条件的范围是:" + "[" + Pixel_YPickupCurrentPositionLimitMin_RT_S1.ToString() + ","
                //    + Pixel_YPickupCurrentPositionLimitMax_RT_S1.ToString()
                //    + "]");
                MyTool.TxtFileProcess.CreateLog("S1当前物料右上角像Y素坐标值超出限制条件的范围." + "当前值为:" +
                    Pixel_YPickupCurrentPosition_RT_S1.ToString() + ",限制条件的范围是:" + "[" + Pixel_YPickupCurrentPositionLimitMin_RT_S1.ToString() + ","
                    + Pixel_YPickupCurrentPositionLimitMax_RT_S1.ToString()
                    + "]" + "----用户：" + ActionForm._operatorName,
               ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                return 2;
            }
            if (Pixel_AnglePickupCurrent_S1 < Pixel_AnglePickupCurrentLimitMin_S1 || Pixel_AnglePickupCurrent_S1 > Pixel_AnglePickupCurrentLimitMax_S1)
            {
                //MessageBox.Show("S1当前物料角度值超出限制条件的范围." + "当前值为:" +
                //    Pixel_AnglePickupCurrent_S1.ToString() + ",限制条件的范围是:" + "[" + Pixel_AnglePickupCurrentLimitMin_S1.ToString() + ","
                //    + Pixel_AnglePickupCurrentLimitMax_S1.ToString()
                //    + "]");
                MyTool.TxtFileProcess.CreateLog("S1当前物料角度值超出限制条件的范围." + "当前值为:" +
                    Pixel_AnglePickupCurrent_S1.ToString() + ",限制条件的范围是:" + "[" + Pixel_AnglePickupCurrentLimitMin_S1.ToString() + ","
                    + Pixel_AnglePickupCurrentLimitMax_S1.ToString()
                    + "]" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                return 3;
            }
            return 0;
        }

        private int currentProductPositionCheck_S2()
        {
            if (!ActionForm._autoModeFlag)//手动时读取控件里的数据作为参数
            {
                try
                {
                    Pixel_XPickupCurrentPositionLimitMin_RT_S2 = Convert.ToSingle(txtPixel_XPickupCurrentPositionLimitMin_RT_S2.Text);
                    Pixel_XPickupCurrentPositionLimitMax_RT_S2 = Convert.ToSingle(txtPixel_XPickupCurrentPositionLimitMax_RT_S2.Text);
                    Pixel_YPickupCurrentPositionLimitMin_RT_S2 = Convert.ToSingle(txtPixel_YPickupCurrentPositionLimitMin_RT_S2.Text);
                    Pixel_YPickupCurrentPositionLimitMax_RT_S2 = Convert.ToSingle(txtPixel_YPickupCurrentPositionLimitMax_RT_S2.Text);
                    Pixel_AnglePickupCurrentLimitMin_S2 = Convert.ToSingle(txtPixel_AnglePickupCurrentLimitMin_S2.Text);
                    Pixel_AnglePickupCurrentLimitMax_S2 = Convert.ToSingle(txtPixel_AnglePickupCurrentLimitMax_S2.Text);
                }
                catch (Exception ex)
                {
                    //MessageBox.Show("S2控件里输入的来料像素位置限制参数错误!");
                    MyTool.TxtFileProcess.CreateLog("S2控件里输入的来料像素位置限制参数错误!" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    return -1;
                }
            }
            else//自动时读取产品参数文件数据
            {
                Pixel_XPickupCurrentPositionLimitMin_RT_S2 = _currentProductRecipe.Pixel_XPickupCurrentPositionLimitMin_RT_S2;
                Pixel_XPickupCurrentPositionLimitMax_RT_S2 = _currentProductRecipe.Pixel_XPickupCurrentPositionLimitMax_RT_S2;
                Pixel_YPickupCurrentPositionLimitMin_RT_S2 = _currentProductRecipe.Pixel_YPickupCurrentPositionLimitMin_RT_S2;
                Pixel_YPickupCurrentPositionLimitMax_RT_S2 = _currentProductRecipe.Pixel_YPickupCurrentPositionLimitMax_RT_S2;
                Pixel_AnglePickupCurrentLimitMin_S2 = _currentProductRecipe.Pixel_AnglePickupCurrentLimitMin_S2;
                Pixel_AnglePickupCurrentLimitMax_S2 = _currentProductRecipe.Pixel_AnglePickupCurrentLimitMax_S2;
            }

            if (Pixel_XPickupCurrentPosition_RT_S2 < Pixel_XPickupCurrentPositionLimitMin_RT_S2 || Pixel_XPickupCurrentPosition_RT_S2 > Pixel_XPickupCurrentPositionLimitMax_RT_S2)
            {
                //MessageBox.Show("S2当前物料右上角X像素坐标值超出限制条件的范围." + "当前值为:" +
                //    Pixel_XPickupCurrentPosition_RT_S2.ToString() + ",限制条件的范围是:" + "[" + Pixel_XPickupCurrentPositionLimitMin_RT_S2.ToString() + ","
                //    + Pixel_XPickupCurrentPositionLimitMax_RT_S2.ToString()
                //    + "]");
                MyTool.TxtFileProcess.CreateLog("S2当前物料右上角X像素坐标值超出限制条件的范围." + "当前值为:" +
                    Pixel_XPickupCurrentPosition_RT_S2.ToString() + ",限制条件的范围是:" + "[" + Pixel_XPickupCurrentPositionLimitMin_RT_S2.ToString() + ","
                    + Pixel_XPickupCurrentPositionLimitMax_RT_S2.ToString()
                    + "]" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                return 1;
            }
            if (Pixel_YPickupCurrentPosition_RT_S2 < Pixel_YPickupCurrentPositionLimitMin_RT_S2 || Pixel_YPickupCurrentPosition_RT_S2 > Pixel_YPickupCurrentPositionLimitMax_RT_S2)
            {
                //MessageBox.Show("S2当前物料右上角像Y素坐标值超出限制条件的范围." + "当前值为:" +
                //    Pixel_YPickupCurrentPosition_RT_S2.ToString() + ",限制条件的范围是:" + "[" + Pixel_YPickupCurrentPositionLimitMin_RT_S2.ToString() + ","
                //    + Pixel_YPickupCurrentPositionLimitMax_RT_S2.ToString()
                //    + "]");
                MyTool.TxtFileProcess.CreateLog("S2当前物料右上角像Y素坐标值超出限制条件的范围." + "当前值为:" +
                    Pixel_YPickupCurrentPosition_RT_S2.ToString() + ",限制条件的范围是:" + "[" + Pixel_YPickupCurrentPositionLimitMin_RT_S2.ToString() + ","
                    + Pixel_YPickupCurrentPositionLimitMax_RT_S2.ToString()
                    + "]" + "----用户：" + ActionForm._operatorName,
               ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                return 2;
            }
            if (Pixel_AnglePickupCurrent_S2 < Pixel_AnglePickupCurrentLimitMin_S2 || Pixel_AnglePickupCurrent_S2 > Pixel_AnglePickupCurrentLimitMax_S2)
            {
                //MessageBox.Show("S2当前物料角度值超出限制条件的范围." + "当前值为:" +
                //    Pixel_AnglePickupCurrent_S2.ToString() + ",限制条件的范围是:" + "[" + Pixel_AnglePickupCurrentLimitMin_S2.ToString() + ","
                //    + Pixel_AnglePickupCurrentLimitMax_S2.ToString()
                //    + "]");
                MyTool.TxtFileProcess.CreateLog("S2当前物料角度值超出限制条件的范围." + "当前值为:" +
                    Pixel_AnglePickupCurrent_S2.ToString() + ",限制条件的范围是:" + "[" + Pixel_AnglePickupCurrentLimitMin_S2.ToString() + ","
                    + Pixel_AnglePickupCurrentLimitMax_S2.ToString()
                    + "]" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                return 3;
            }
            return 0;
        }

        private int currentProductPositionCheck_S3()
        {
            if (!ActionForm._autoModeFlag)//手动时读取控件里的数据作为参数
            {
                try
                {
                    Pixel_XPickupCurrentPositionLimitMin_RT_S3 = Convert.ToSingle(txtPixel_XPickupCurrentPositionLimitMin_RT_S3.Text);
                    Pixel_XPickupCurrentPositionLimitMax_RT_S3 = Convert.ToSingle(txtPixel_XPickupCurrentPositionLimitMax_RT_S3.Text);
                    Pixel_YPickupCurrentPositionLimitMin_RT_S3 = Convert.ToSingle(txtPixel_YPickupCurrentPositionLimitMin_RT_S3.Text);
                    Pixel_YPickupCurrentPositionLimitMax_RT_S3 = Convert.ToSingle(txtPixel_YPickupCurrentPositionLimitMax_RT_S3.Text);
                    Pixel_AnglePickupCurrentLimitMin_S3 = Convert.ToSingle(txtPixel_AnglePickupCurrentLimitMin_S3.Text);
                    Pixel_AnglePickupCurrentLimitMax_S3 = Convert.ToSingle(txtPixel_AnglePickupCurrentLimitMax_S3.Text);
                }
                catch (Exception ex)
                {
                    //MessageBox.Show("S3控件里输入的来料像素位置限制参数错误!");
                    MyTool.TxtFileProcess.CreateLog("S3控件里输入的来料像素位置限制参数错误!" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    return -1;
                }
            }
            else//自动时读取产品参数文件数据
            {
                Pixel_XPickupCurrentPositionLimitMin_RT_S3 = _currentProductRecipe.Pixel_XPickupCurrentPositionLimitMin_RT_S3;
                Pixel_XPickupCurrentPositionLimitMax_RT_S3 = _currentProductRecipe.Pixel_XPickupCurrentPositionLimitMax_RT_S3;
                Pixel_YPickupCurrentPositionLimitMin_RT_S3 = _currentProductRecipe.Pixel_YPickupCurrentPositionLimitMin_RT_S3;
                Pixel_YPickupCurrentPositionLimitMax_RT_S3 = _currentProductRecipe.Pixel_YPickupCurrentPositionLimitMax_RT_S3;
                Pixel_AnglePickupCurrentLimitMin_S3 = _currentProductRecipe.Pixel_AnglePickupCurrentLimitMin_S3;
                Pixel_AnglePickupCurrentLimitMax_S3 = _currentProductRecipe.Pixel_AnglePickupCurrentLimitMax_S3;
            }

            if (Pixel_XPickupCurrentPosition_RT_S3 < Pixel_XPickupCurrentPositionLimitMin_RT_S3 || Pixel_XPickupCurrentPosition_RT_S3 > Pixel_XPickupCurrentPositionLimitMax_RT_S3)
            {
                //MessageBox.Show("S3当前物料右上角X像素坐标值超出限制条件的范围." + "当前值为:" +
                //    Pixel_XPickupCurrentPosition_RT_S3.ToString() + ",限制条件的范围是:" + "[" + Pixel_XPickupCurrentPositionLimitMin_RT_S3.ToString() + ","
                //    + Pixel_XPickupCurrentPositionLimitMax_RT_S3.ToString()
                //    + "]");
                MyTool.TxtFileProcess.CreateLog("S3当前物料右上角X像素坐标值超出限制条件的范围." + "当前值为:" +
                    Pixel_XPickupCurrentPosition_RT_S3.ToString() + ",限制条件的范围是:" + "[" + Pixel_XPickupCurrentPositionLimitMin_RT_S3.ToString() + ","
                    + Pixel_XPickupCurrentPositionLimitMax_RT_S3.ToString()
                    + "]" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                return 1;
            }
            if (Pixel_YPickupCurrentPosition_RT_S3 < Pixel_YPickupCurrentPositionLimitMin_RT_S3 || Pixel_YPickupCurrentPosition_RT_S3 > Pixel_YPickupCurrentPositionLimitMax_RT_S3)
            {
                //MessageBox.Show("S3当前物料右上角像Y素坐标值超出限制条件的范围." + "当前值为:" +
                //    Pixel_YPickupCurrentPosition_RT_S3.ToString() + ",限制条件的范围是:" + "[" + Pixel_YPickupCurrentPositionLimitMin_RT_S3.ToString() + ","
                //    + Pixel_YPickupCurrentPositionLimitMax_RT_S3.ToString()
                //    + "]");
                MyTool.TxtFileProcess.CreateLog("S3当前物料右上角像Y素坐标值超出限制条件的范围." + "当前值为:" +
                    Pixel_YPickupCurrentPosition_RT_S3.ToString() + ",限制条件的范围是:" + "[" + Pixel_YPickupCurrentPositionLimitMin_RT_S3.ToString() + ","
                    + Pixel_YPickupCurrentPositionLimitMax_RT_S3.ToString()
                    + "]" + "----用户：" + ActionForm._operatorName,
               ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                return 2;
            }
            if (Pixel_AnglePickupCurrent_S3 < Pixel_AnglePickupCurrentLimitMin_S3 || Pixel_AnglePickupCurrent_S3 > Pixel_AnglePickupCurrentLimitMax_S3)
            {
                //MessageBox.Show("S3当前物料角度值超出限制条件的范围." + "当前值为:" +
                //    Pixel_AnglePickupCurrent_S3.ToString() + ",限制条件的范围是:" + "[" + Pixel_AnglePickupCurrentLimitMin_S3.ToString() + ","
                //    + Pixel_AnglePickupCurrentLimitMax_S3.ToString()
                //    + "]");
                MyTool.TxtFileProcess.CreateLog("S3当前物料角度值超出限制条件的范围." + "当前值为:" +
                    Pixel_AnglePickupCurrent_S3.ToString() + ",限制条件的范围是:" + "[" + Pixel_AnglePickupCurrentLimitMin_S3.ToString() + ","
                    + Pixel_AnglePickupCurrentLimitMax_S3.ToString()
                    + "]" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                return 3;
            }
            return 0;
        }
        #endregion

        #region ROB_CurrentProduct:把当前产品的示教物料的4个顶点的像素坐标转换为ROB坐标        
        private bool ROB_CurrentProduct_S1()
        {
            float[] xtmp = new float[4];
            float[] ytmp = new float[4];

            if (!ActionForm._autoModeFlag)//手动时读取控件里的数据作为参数
            {
                float _Pixel_XPickupControlPosition_RT;
                float _Pixel_XPickupControlPosition_LT;
                float _Pixel_XPickupControlPosition_LB;
                float _Pixel_XPickupControlPosition_RB;
                float _Pixel_YPickupControlPosition_RT;
                float _Pixel_YPickupControlPosition_LT;
                float _Pixel_YPickupControlPosition_LB;
                float _Pixel_YPickupControlPosition_RB;
                try
                {
                    _Pixel_XPickupControlPosition_RT = Convert.ToSingle(txtPixel_XPickupTeachPosition_RT_S1.Text);
                    _Pixel_XPickupControlPosition_LT = Convert.ToSingle(txtPixel_XPickupTeachPosition_LT_S1.Text);
                    _Pixel_XPickupControlPosition_LB = Convert.ToSingle(txtPixel_XPickupTeachPosition_LB_S1.Text);
                    _Pixel_XPickupControlPosition_RB = Convert.ToSingle(txtPixel_XPickupTeachPosition_RB_S1.Text);
                    _Pixel_YPickupControlPosition_RT = Convert.ToSingle(txtPixel_YPickupTeachPosition_RT_S1.Text);
                    _Pixel_YPickupControlPosition_LT = Convert.ToSingle(txtPixel_YPickupTeachPosition_LT_S1.Text);
                    _Pixel_YPickupControlPosition_LB = Convert.ToSingle(txtPixel_YPickupTeachPosition_LB_S1.Text);
                    _Pixel_YPickupControlPosition_RB = Convert.ToSingle(txtPixel_YPickupTeachPosition_RB_S1.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("S1控件里输入的顶点像素坐标参数错误!");
                    return false;
                }
                if (!productPositionCoordTransform_S1(new float[] { _Pixel_XPickupControlPosition_RT, _Pixel_XPickupControlPosition_LT, _Pixel_XPickupControlPosition_LB, _Pixel_XPickupControlPosition_RB }, new float[] { _Pixel_YPickupControlPosition_RT, _Pixel_YPickupControlPosition_LT, _Pixel_YPickupControlPosition_LB, _Pixel_YPickupControlPosition_RB }, ref xtmp, ref ytmp))
                {
                    MessageBox.Show("S1把示教的像素坐标转换为ROB坐标失败!");
                    return false;
                }
            }
            else//自动时读取产品参数文件数据
            {
                if (!productPositionCoordTransform_S1(new float[] { _currentProductRecipe.Pixel_XPickupTeachPosition_RT_S1, _currentProductRecipe.Pixel_XPickupTeachPosition_LT_S1, _currentProductRecipe.Pixel_XPickupTeachPosition_LB_S1, _currentProductRecipe.Pixel_XPickupTeachPosition_RB_S1 }, new float[] { _currentProductRecipe.Pixel_YPickupTeachPosition_RT_S1, _currentProductRecipe.Pixel_YPickupTeachPosition_LT_S1, _currentProductRecipe.Pixel_YPickupTeachPosition_LB_S1, _currentProductRecipe.Pixel_YPickupTeachPosition_RB_S1 }, ref xtmp, ref ytmp))
                {
                    MessageBox.Show("S1把当前产品的示教物料的像素坐标转换为ROB坐标失败!");
                    return false;
                }
            }
            ROB_XPickupTeachPosition_RT_CurrentProduct_S1 = xtmp[0];
            ROB_YPickupTeachPosition_RT_CurrentProduct_S1 = ytmp[0];
            ROB_XPickupTeachPosition_LT_CurrentProduct_S1 = xtmp[1];
            ROB_YPickupTeachPosition_LT_CurrentProduct_S1 = ytmp[1];
            ROB_XPickupTeachPosition_LB_CurrentProduct_S1 = xtmp[2];
            ROB_YPickupTeachPosition_LB_CurrentProduct_S1 = ytmp[2];
            ROB_XPickupTeachPosition_RB_CurrentProduct_S1 = xtmp[3];
            ROB_YPickupTeachPosition_RB_CurrentProduct_S1 = ytmp[3];
            return true;
        }
        private bool ROB_CurrentProduct_S2()
        {
            float[] xtmp = new float[4];
            float[] ytmp = new float[4];

            if (!ActionForm._autoModeFlag)//手动时读取控件里的数据作为参数
            {
                float _Pixel_XPickupControlPosition_RT;
                float _Pixel_XPickupControlPosition_LT;
                float _Pixel_XPickupControlPosition_LB;
                float _Pixel_XPickupControlPosition_RB;
                float _Pixel_YPickupControlPosition_RT;
                float _Pixel_YPickupControlPosition_LT;
                float _Pixel_YPickupControlPosition_LB;
                float _Pixel_YPickupControlPosition_RB;
                try
                {
                    _Pixel_XPickupControlPosition_RT = Convert.ToSingle(txtPixel_XPickupTeachPosition_RT_S2.Text);
                    _Pixel_XPickupControlPosition_LT = Convert.ToSingle(txtPixel_XPickupTeachPosition_LT_S2.Text);
                    _Pixel_XPickupControlPosition_LB = Convert.ToSingle(txtPixel_XPickupTeachPosition_LB_S2.Text);
                    _Pixel_XPickupControlPosition_RB = Convert.ToSingle(txtPixel_XPickupTeachPosition_RB_S2.Text);
                    _Pixel_YPickupControlPosition_RT = Convert.ToSingle(txtPixel_YPickupTeachPosition_RT_S2.Text);
                    _Pixel_YPickupControlPosition_LT = Convert.ToSingle(txtPixel_YPickupTeachPosition_LT_S2.Text);
                    _Pixel_YPickupControlPosition_LB = Convert.ToSingle(txtPixel_YPickupTeachPosition_LB_S2.Text);
                    _Pixel_YPickupControlPosition_RB = Convert.ToSingle(txtPixel_YPickupTeachPosition_RB_S2.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("S2控件里输入的顶点像素坐标参数错误!");
                    return false;
                }
                if (!productPositionCoordTransform_S2(new float[] { _Pixel_XPickupControlPosition_RT, _Pixel_XPickupControlPosition_LT, _Pixel_XPickupControlPosition_LB, _Pixel_XPickupControlPosition_RB }, new float[] { _Pixel_YPickupControlPosition_RT, _Pixel_YPickupControlPosition_LT, _Pixel_YPickupControlPosition_LB, _Pixel_YPickupControlPosition_RB }, ref xtmp, ref ytmp))
                {
                    MessageBox.Show("S2把示教的像素坐标转换为ROB坐标失败!");
                    return false;
                }
            }
            else//自动时读取产品参数文件数据
            {
                if (!productPositionCoordTransform_S2(new float[] { _currentProductRecipe.Pixel_XPickupTeachPosition_RT_S2, _currentProductRecipe.Pixel_XPickupTeachPosition_LT_S2, _currentProductRecipe.Pixel_XPickupTeachPosition_LB_S2, _currentProductRecipe.Pixel_XPickupTeachPosition_RB_S2 }, new float[] { _currentProductRecipe.Pixel_YPickupTeachPosition_RT_S2, _currentProductRecipe.Pixel_YPickupTeachPosition_LT_S2, _currentProductRecipe.Pixel_YPickupTeachPosition_LB_S2, _currentProductRecipe.Pixel_YPickupTeachPosition_RB_S2 }, ref xtmp, ref ytmp))
                {
                    MessageBox.Show("S2把当前产品的示教物料的像素坐标转换为ROB坐标失败!");
                    return false;
                }
            }
            ROB_XPickupTeachPosition_RT_CurrentProduct_S2 = xtmp[0];
            ROB_YPickupTeachPosition_RT_CurrentProduct_S2 = ytmp[0];
            ROB_XPickupTeachPosition_LT_CurrentProduct_S2 = xtmp[1];
            ROB_YPickupTeachPosition_LT_CurrentProduct_S2 = ytmp[1];
            ROB_XPickupTeachPosition_LB_CurrentProduct_S2 = xtmp[2];
            ROB_YPickupTeachPosition_LB_CurrentProduct_S2 = ytmp[2];
            ROB_XPickupTeachPosition_RB_CurrentProduct_S2 = xtmp[3];
            ROB_YPickupTeachPosition_RB_CurrentProduct_S2 = ytmp[3];
            return true;
        }
        private bool ROB_CurrentProduct_S3()
        {
            float[] xtmp = new float[4];
            float[] ytmp = new float[4];

            if (!ActionForm._autoModeFlag)//手动时读取控件里的数据作为参数
            {
                float _Pixel_XPickupControlPosition_RT;
                float _Pixel_XPickupControlPosition_LT;
                float _Pixel_XPickupControlPosition_LB;
                float _Pixel_XPickupControlPosition_RB;
                float _Pixel_YPickupControlPosition_RT;
                float _Pixel_YPickupControlPosition_LT;
                float _Pixel_YPickupControlPosition_LB;
                float _Pixel_YPickupControlPosition_RB;
                try
                {
                    _Pixel_XPickupControlPosition_RT = Convert.ToSingle(txtPixel_XPickupTeachPosition_RT_S3.Text);
                    _Pixel_XPickupControlPosition_LT = Convert.ToSingle(txtPixel_XPickupTeachPosition_LT_S3.Text);
                    _Pixel_XPickupControlPosition_LB = Convert.ToSingle(txtPixel_XPickupTeachPosition_LB_S3.Text);
                    _Pixel_XPickupControlPosition_RB = Convert.ToSingle(txtPixel_XPickupTeachPosition_RB_S3.Text);
                    _Pixel_YPickupControlPosition_RT = Convert.ToSingle(txtPixel_YPickupTeachPosition_RT_S3.Text);
                    _Pixel_YPickupControlPosition_LT = Convert.ToSingle(txtPixel_YPickupTeachPosition_LT_S3.Text);
                    _Pixel_YPickupControlPosition_LB = Convert.ToSingle(txtPixel_YPickupTeachPosition_LB_S3.Text);
                    _Pixel_YPickupControlPosition_RB = Convert.ToSingle(txtPixel_YPickupTeachPosition_RB_S3.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("S3控件里输入的顶点像素坐标参数错误!");
                    return false;
                }
                if (!productPositionCoordTransform_S3(new float[] { _Pixel_XPickupControlPosition_RT, _Pixel_XPickupControlPosition_LT, _Pixel_XPickupControlPosition_LB, _Pixel_XPickupControlPosition_RB }, new float[] { _Pixel_YPickupControlPosition_RT, _Pixel_YPickupControlPosition_LT, _Pixel_YPickupControlPosition_LB, _Pixel_YPickupControlPosition_RB }, ref xtmp, ref ytmp))
                {
                    MessageBox.Show("S3把示教的像素坐标转换为ROB坐标失败!");
                    return false;
                }
            }
            else//自动时读取产品参数文件数据
            {
                if (!productPositionCoordTransform_S3(new float[] { _currentProductRecipe.Pixel_XPickupTeachPosition_RT_S3, _currentProductRecipe.Pixel_XPickupTeachPosition_LT_S3, _currentProductRecipe.Pixel_XPickupTeachPosition_LB_S3, _currentProductRecipe.Pixel_XPickupTeachPosition_RB_S3 }, new float[] { _currentProductRecipe.Pixel_YPickupTeachPosition_RT_S3, _currentProductRecipe.Pixel_YPickupTeachPosition_LT_S3, _currentProductRecipe.Pixel_YPickupTeachPosition_LB_S3, _currentProductRecipe.Pixel_YPickupTeachPosition_RB_S3 }, ref xtmp, ref ytmp))
                {
                    MessageBox.Show("S3把当前产品的示教物料的像素坐标转换为ROB坐标失败!");
                    return false;
                }
            }
            ROB_XPickupTeachPosition_RT_CurrentProduct_S3 = xtmp[0];
            ROB_YPickupTeachPosition_RT_CurrentProduct_S3 = ytmp[0];
            ROB_XPickupTeachPosition_LT_CurrentProduct_S3 = xtmp[1];
            ROB_YPickupTeachPosition_LT_CurrentProduct_S3 = ytmp[1];
            ROB_XPickupTeachPosition_LB_CurrentProduct_S3 = xtmp[2];
            ROB_YPickupTeachPosition_LB_CurrentProduct_S3 = ytmp[2];
            ROB_XPickupTeachPosition_RB_CurrentProduct_S3 = xtmp[3];
            ROB_YPickupTeachPosition_RB_CurrentProduct_S3 = ytmp[3];
            return true;
        }
        #endregion

        #region currentProductPositionCoordTransform:把当前物料位置数据像素坐标转化为ROB坐标
        private bool currentProductPositionCoordTransform_S1()
        {
            float[] xtmp = new float[4];
            float[] ytmp = new float[4];
            if (!productPositionCoordTransform_S1(new float[] { Pixel_XPickupCurrentPosition_RT_S1, Pixel_XPickupCurrentPosition_LT_S1, Pixel_XPickupCurrentPosition_LB_S1, Pixel_XPickupCurrentPosition_RB_S1 }, new float[] { Pixel_YPickupCurrentPosition_RT_S1, Pixel_YPickupCurrentPosition_LT_S1, Pixel_YPickupCurrentPosition_LB_S1, Pixel_YPickupCurrentPosition_RB_S1 }, ref xtmp, ref ytmp))
            {
                MessageBox.Show("S1把当前物料位置数据像素坐标转化为ROB坐标失败!");
                return false;
            }
            ROB_XPickupCurrentPosition_RT_S1 = xtmp[0];
            ROB_YPickupCurrentPosition_RT_S1 = ytmp[0];
            ROB_XPickupCurrentPosition_LT_S1 = xtmp[1];
            ROB_YPickupCurrentPosition_LT_S1 = ytmp[1];
            ROB_XPickupCurrentPosition_LB_S1 = xtmp[2];
            ROB_YPickupCurrentPosition_LB_S1 = ytmp[2];
            ROB_XPickupCurrentPosition_RB_S1 = xtmp[3];
            ROB_YPickupCurrentPosition_RB_S1 = ytmp[3];
            return true;
        }
        private bool currentProductPositionCoordTransform_S2()
        {
            float[] xtmp = new float[4];
            float[] ytmp = new float[4];
            if (!productPositionCoordTransform_S2(new float[] { Pixel_XPickupCurrentPosition_RT_S2, Pixel_XPickupCurrentPosition_LT_S2, Pixel_XPickupCurrentPosition_LB_S2, Pixel_XPickupCurrentPosition_RB_S2 }, new float[] { Pixel_YPickupCurrentPosition_RT_S2, Pixel_YPickupCurrentPosition_LT_S2, Pixel_YPickupCurrentPosition_LB_S2, Pixel_YPickupCurrentPosition_RB_S2 }, ref xtmp, ref ytmp))
            {
                MessageBox.Show("S2把当前物料位置数据像素坐标转化为ROB坐标失败!");
                return false;
            }
            ROB_XPickupCurrentPosition_RT_S2 = xtmp[0];
            ROB_YPickupCurrentPosition_RT_S2 = ytmp[0];
            ROB_XPickupCurrentPosition_LT_S2 = xtmp[1];
            ROB_YPickupCurrentPosition_LT_S2 = ytmp[1];
            ROB_XPickupCurrentPosition_LB_S2 = xtmp[2];
            ROB_YPickupCurrentPosition_LB_S2 = ytmp[2];
            ROB_XPickupCurrentPosition_RB_S2 = xtmp[3];
            ROB_YPickupCurrentPosition_RB_S2 = ytmp[3];
            return true;
        }
        private bool currentProductPositionCoordTransform_S3()
        {
            float[] xtmp = new float[4];
            float[] ytmp = new float[4];
            if (!productPositionCoordTransform_S3(new float[] { Pixel_XPickupCurrentPosition_RT_S3, Pixel_XPickupCurrentPosition_LT_S3, Pixel_XPickupCurrentPosition_LB_S3, Pixel_XPickupCurrentPosition_RB_S3 }, new float[] { Pixel_YPickupCurrentPosition_RT_S3, Pixel_YPickupCurrentPosition_LT_S3, Pixel_YPickupCurrentPosition_LB_S3, Pixel_YPickupCurrentPosition_RB_S3 }, ref xtmp, ref ytmp))
            {
                MessageBox.Show("S3把当前物料位置数据像素坐标转化为ROB坐标失败!");
                return false;
            }
            ROB_XPickupCurrentPosition_RT_S3 = xtmp[0];
            ROB_YPickupCurrentPosition_RT_S3 = ytmp[0];
            ROB_XPickupCurrentPosition_LT_S3 = xtmp[1];
            ROB_YPickupCurrentPosition_LT_S3 = ytmp[1];
            ROB_XPickupCurrentPosition_LB_S3 = xtmp[2];
            ROB_YPickupCurrentPosition_LB_S3 = ytmp[2];
            ROB_XPickupCurrentPosition_RB_S3 = xtmp[3];
            ROB_YPickupCurrentPosition_RB_S3 = ytmp[3];
            return true;
        }
        #endregion

        #region currentTranslateMatrix:用4个顶点坐标计算示教物料位置变换到当前物料位置的矩阵hv_HomMat2D_CurrentTranslateMatrix
        private bool currentTranslateMatrix_S1()
        {
            hv_HomMat2D_CurrentTranslateMatrix_S1 = newRobCamCalibration.coordTransAndRotMat(
                new float[] { ROB_XPickupTeachPosition_RT_CurrentProduct_S1, ROB_XPickupTeachPosition_LT_CurrentProduct_S1, ROB_XPickupTeachPosition_LB_CurrentProduct_S1, ROB_XPickupTeachPosition_RB_CurrentProduct_S1 },
                new float[] { ROB_YPickupTeachPosition_RT_CurrentProduct_S1, ROB_YPickupTeachPosition_LT_CurrentProduct_S1, ROB_YPickupTeachPosition_LB_CurrentProduct_S1, ROB_YPickupTeachPosition_RB_CurrentProduct_S1 },
                new float[] { ROB_XPickupCurrentPosition_RT_S1, ROB_XPickupCurrentPosition_LT_S1, ROB_XPickupCurrentPosition_LB_S1, ROB_XPickupCurrentPosition_RB_S1 },
                new float[] { ROB_YPickupCurrentPosition_RT_S1, ROB_YPickupCurrentPosition_LT_S1, ROB_YPickupCurrentPosition_LB_S1, ROB_YPickupCurrentPosition_RB_S1 });
            if (hv_HomMat2D_CurrentTranslateMatrix_S1 == null)
            {
                MessageBox.Show("S1计算矩阵hv_HomMat2D_CurrentTranslateMatrix失败!");
                return false;
            }
            return true;
        }
        private bool currentTranslateMatrix_S2()
        {
            hv_HomMat2D_CurrentTranslateMatrix_S2 = newRobCamCalibration.coordTransAndRotMat(
                new float[] { ROB_XPickupTeachPosition_RT_CurrentProduct_S2, ROB_XPickupTeachPosition_LT_CurrentProduct_S2, ROB_XPickupTeachPosition_LB_CurrentProduct_S2, ROB_XPickupTeachPosition_RB_CurrentProduct_S2 },
                new float[] { ROB_YPickupTeachPosition_RT_CurrentProduct_S2, ROB_YPickupTeachPosition_LT_CurrentProduct_S2, ROB_YPickupTeachPosition_LB_CurrentProduct_S2, ROB_YPickupTeachPosition_RB_CurrentProduct_S2 },
                new float[] { ROB_XPickupCurrentPosition_RT_S2, ROB_XPickupCurrentPosition_LT_S2, ROB_XPickupCurrentPosition_LB_S2, ROB_XPickupCurrentPosition_RB_S2 },
                new float[] { ROB_YPickupCurrentPosition_RT_S2, ROB_YPickupCurrentPosition_LT_S2, ROB_YPickupCurrentPosition_LB_S2, ROB_YPickupCurrentPosition_RB_S2 });
            if (hv_HomMat2D_CurrentTranslateMatrix_S2 == null)
            {
                MessageBox.Show("S2计算矩阵hv_HomMat2D_CurrentTranslateMatrix失败!");
                return false;
            }
            return true;
        }
        private bool currentTranslateMatrix_S3()
        {
            hv_HomMat2D_CurrentTranslateMatrix_S3 = newRobCamCalibration.coordTransAndRotMat(
                new float[] { ROB_XPickupTeachPosition_RT_CurrentProduct_S3, ROB_XPickupTeachPosition_LT_CurrentProduct_S3, ROB_XPickupTeachPosition_LB_CurrentProduct_S3, ROB_XPickupTeachPosition_RB_CurrentProduct_S3 },
                new float[] { ROB_YPickupTeachPosition_RT_CurrentProduct_S3, ROB_YPickupTeachPosition_LT_CurrentProduct_S3, ROB_YPickupTeachPosition_LB_CurrentProduct_S3, ROB_YPickupTeachPosition_RB_CurrentProduct_S3 },
                new float[] { ROB_XPickupCurrentPosition_RT_S3, ROB_XPickupCurrentPosition_LT_S3, ROB_XPickupCurrentPosition_LB_S3, ROB_XPickupCurrentPosition_RB_S3 },
                new float[] { ROB_YPickupCurrentPosition_RT_S3, ROB_YPickupCurrentPosition_LT_S3, ROB_YPickupCurrentPosition_LB_S3, ROB_YPickupCurrentPosition_RB_S3 });
            if (hv_HomMat2D_CurrentTranslateMatrix_S3 == null)
            {
                MessageBox.Show("S3计算矩阵hv_HomMat2D_CurrentTranslateMatrix失败!");
                return false;
            }
            return true;
        }
        #endregion

        #region currentTranslateMatrix:用4个顶点像素坐标计算示教物料位置变换到当前物料位置的矩阵hv_HomMat2D_CurrentTranslateMatrix_Pixel
        private bool currentTranslateMatrix_Pixel_S1()
        {
            float _Pixel_XPickupControlPosition_RT;
            float _Pixel_XPickupControlPosition_LT;
            float _Pixel_XPickupControlPosition_LB;
            float _Pixel_XPickupControlPosition_RB;
            float _Pixel_YPickupControlPosition_RT;
            float _Pixel_YPickupControlPosition_LT;
            float _Pixel_YPickupControlPosition_LB;
            float _Pixel_YPickupControlPosition_RB;
            if (!ActionForm._autoModeFlag)//手动时读取控件里的数据作为参数
            {
                try
                {
                    _Pixel_XPickupControlPosition_RT = Convert.ToSingle(txtPixel_XPickupTeachPosition_RT_S1.Text);
                    _Pixel_XPickupControlPosition_LT = Convert.ToSingle(txtPixel_XPickupTeachPosition_LT_S1.Text);
                    _Pixel_XPickupControlPosition_LB = Convert.ToSingle(txtPixel_XPickupTeachPosition_LB_S1.Text);
                    _Pixel_XPickupControlPosition_RB = Convert.ToSingle(txtPixel_XPickupTeachPosition_RB_S1.Text);
                    _Pixel_YPickupControlPosition_RT = Convert.ToSingle(txtPixel_YPickupTeachPosition_RT_S1.Text);
                    _Pixel_YPickupControlPosition_LT = Convert.ToSingle(txtPixel_YPickupTeachPosition_LT_S1.Text);
                    _Pixel_YPickupControlPosition_LB = Convert.ToSingle(txtPixel_YPickupTeachPosition_LB_S1.Text);
                    _Pixel_YPickupControlPosition_RB = Convert.ToSingle(txtPixel_YPickupTeachPosition_RB_S1.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("S1控件里输入的顶点像素坐标参数错误!");
                    return false;
                }
            }
            else//自动时读取产品参数文件数据
            {
                try
                {
                    _Pixel_XPickupControlPosition_RT = _currentProductRecipe.Pixel_XPickupTeachPosition_RT_S1;
                    _Pixel_XPickupControlPosition_LT = _currentProductRecipe.Pixel_XPickupTeachPosition_LT_S1;
                    _Pixel_XPickupControlPosition_LB = _currentProductRecipe.Pixel_XPickupTeachPosition_LB_S1;
                    _Pixel_XPickupControlPosition_RB = _currentProductRecipe.Pixel_XPickupTeachPosition_RB_S1;
                    _Pixel_YPickupControlPosition_RT = _currentProductRecipe.Pixel_YPickupTeachPosition_RT_S1;
                    _Pixel_YPickupControlPosition_LT = _currentProductRecipe.Pixel_YPickupTeachPosition_LT_S1;
                    _Pixel_YPickupControlPosition_LB = _currentProductRecipe.Pixel_YPickupTeachPosition_LB_S1;
                    _Pixel_YPickupControlPosition_RB = _currentProductRecipe.Pixel_YPickupTeachPosition_RB_S1;
                }
                catch (Exception)
                {
                    return false;
                }
            }

            hv_HomMat2D_CurrentTranslateMatrix_Pixel_S1 = newRobCamCalibration.coordTransAndRotMat(
                new float[] { _Pixel_XPickupControlPosition_RT, _Pixel_XPickupControlPosition_LT, _Pixel_XPickupControlPosition_LB, _Pixel_XPickupControlPosition_RB },
                new float[] { _Pixel_YPickupControlPosition_RT, _Pixel_YPickupControlPosition_LT, _Pixel_YPickupControlPosition_LB, _Pixel_YPickupControlPosition_RB },
                new float[] { Pixel_XPickupCurrentPosition_RT_S1, Pixel_XPickupCurrentPosition_LT_S1, Pixel_XPickupCurrentPosition_LB_S1, Pixel_XPickupCurrentPosition_RB_S1 },
                new float[] { Pixel_YPickupCurrentPosition_RT_S1, Pixel_YPickupCurrentPosition_LT_S1, Pixel_YPickupCurrentPosition_LB_S1, Pixel_YPickupCurrentPosition_RB_S1 });
            if (hv_HomMat2D_CurrentTranslateMatrix_Pixel_S1 == null)
            {
                MessageBox.Show("S1计算矩阵hv_HomMat2D_CurrentTranslateMatrix_Pixel失败!");
                return false;
            }
            return true;
        }
        private bool currentTranslateMatrix_Pixel_S2()
        {
            float _Pixel_XPickupControlPosition_RT;
            float _Pixel_XPickupControlPosition_LT;
            float _Pixel_XPickupControlPosition_LB;
            float _Pixel_XPickupControlPosition_RB;
            float _Pixel_YPickupControlPosition_RT;
            float _Pixel_YPickupControlPosition_LT;
            float _Pixel_YPickupControlPosition_LB;
            float _Pixel_YPickupControlPosition_RB;
            if (!ActionForm._autoModeFlag)//手动时读取控件里的数据作为参数
            {
                try
                {
                    _Pixel_XPickupControlPosition_RT = Convert.ToSingle(txtPixel_XPickupTeachPosition_RT_S2.Text);
                    _Pixel_XPickupControlPosition_LT = Convert.ToSingle(txtPixel_XPickupTeachPosition_LT_S2.Text);
                    _Pixel_XPickupControlPosition_LB = Convert.ToSingle(txtPixel_XPickupTeachPosition_LB_S2.Text);
                    _Pixel_XPickupControlPosition_RB = Convert.ToSingle(txtPixel_XPickupTeachPosition_RB_S2.Text);
                    _Pixel_YPickupControlPosition_RT = Convert.ToSingle(txtPixel_YPickupTeachPosition_RT_S2.Text);
                    _Pixel_YPickupControlPosition_LT = Convert.ToSingle(txtPixel_YPickupTeachPosition_LT_S2.Text);
                    _Pixel_YPickupControlPosition_LB = Convert.ToSingle(txtPixel_YPickupTeachPosition_LB_S2.Text);
                    _Pixel_YPickupControlPosition_RB = Convert.ToSingle(txtPixel_YPickupTeachPosition_RB_S2.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("S2控件里输入的顶点像素坐标参数错误!");
                    return false;
                }
            }
            else//自动时读取产品参数文件数据
            {
                try
                {
                    _Pixel_XPickupControlPosition_RT = _currentProductRecipe.Pixel_XPickupTeachPosition_RT_S2;
                    _Pixel_XPickupControlPosition_LT = _currentProductRecipe.Pixel_XPickupTeachPosition_LT_S2;
                    _Pixel_XPickupControlPosition_LB = _currentProductRecipe.Pixel_XPickupTeachPosition_LB_S2;
                    _Pixel_XPickupControlPosition_RB = _currentProductRecipe.Pixel_XPickupTeachPosition_RB_S2;
                    _Pixel_YPickupControlPosition_RT = _currentProductRecipe.Pixel_YPickupTeachPosition_RT_S2;
                    _Pixel_YPickupControlPosition_LT = _currentProductRecipe.Pixel_YPickupTeachPosition_LT_S2;
                    _Pixel_YPickupControlPosition_LB = _currentProductRecipe.Pixel_YPickupTeachPosition_LB_S2;
                    _Pixel_YPickupControlPosition_RB = _currentProductRecipe.Pixel_YPickupTeachPosition_RB_S2;
                }
                catch (Exception)
                {
                    return false;
                }
            }

            hv_HomMat2D_CurrentTranslateMatrix_Pixel_S2 = newRobCamCalibration.coordTransAndRotMat(
                new float[] { _Pixel_XPickupControlPosition_RT, _Pixel_XPickupControlPosition_LT, _Pixel_XPickupControlPosition_LB, _Pixel_XPickupControlPosition_RB },
                new float[] { _Pixel_YPickupControlPosition_RT, _Pixel_YPickupControlPosition_LT, _Pixel_YPickupControlPosition_LB, _Pixel_YPickupControlPosition_RB },
                new float[] { Pixel_XPickupCurrentPosition_RT_S2, Pixel_XPickupCurrentPosition_LT_S2, Pixel_XPickupCurrentPosition_LB_S2, Pixel_XPickupCurrentPosition_RB_S2 },
                new float[] { Pixel_YPickupCurrentPosition_RT_S2, Pixel_YPickupCurrentPosition_LT_S2, Pixel_YPickupCurrentPosition_LB_S2, Pixel_YPickupCurrentPosition_RB_S2 });
            if (hv_HomMat2D_CurrentTranslateMatrix_Pixel_S2 == null)
            {
                MessageBox.Show("S2计算矩阵hv_HomMat2D_CurrentTranslateMatrix_Pixel失败!");
                return false;
            }
            return true;
        }
        private bool currentTranslateMatrix_Pixel_S3()
        {
            float _Pixel_XPickupControlPosition_RT;
            float _Pixel_XPickupControlPosition_LT;
            float _Pixel_XPickupControlPosition_LB;
            float _Pixel_XPickupControlPosition_RB;
            float _Pixel_YPickupControlPosition_RT;
            float _Pixel_YPickupControlPosition_LT;
            float _Pixel_YPickupControlPosition_LB;
            float _Pixel_YPickupControlPosition_RB;
            if (!ActionForm._autoModeFlag)//手动时读取控件里的数据作为参数
            {
                try
                {
                    _Pixel_XPickupControlPosition_RT = Convert.ToSingle(txtPixel_XPickupTeachPosition_RT_S3.Text);
                    _Pixel_XPickupControlPosition_LT = Convert.ToSingle(txtPixel_XPickupTeachPosition_LT_S3.Text);
                    _Pixel_XPickupControlPosition_LB = Convert.ToSingle(txtPixel_XPickupTeachPosition_LB_S3.Text);
                    _Pixel_XPickupControlPosition_RB = Convert.ToSingle(txtPixel_XPickupTeachPosition_RB_S3.Text);
                    _Pixel_YPickupControlPosition_RT = Convert.ToSingle(txtPixel_YPickupTeachPosition_RT_S3.Text);
                    _Pixel_YPickupControlPosition_LT = Convert.ToSingle(txtPixel_YPickupTeachPosition_LT_S3.Text);
                    _Pixel_YPickupControlPosition_LB = Convert.ToSingle(txtPixel_YPickupTeachPosition_LB_S3.Text);
                    _Pixel_YPickupControlPosition_RB = Convert.ToSingle(txtPixel_YPickupTeachPosition_RB_S3.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("S3控件里输入的顶点像素坐标参数错误!");
                    return false;
                }
            }
            else//自动时读取产品参数文件数据
            {
                try
                {
                    _Pixel_XPickupControlPosition_RT = _currentProductRecipe.Pixel_XPickupTeachPosition_RT_S3;
                    _Pixel_XPickupControlPosition_LT = _currentProductRecipe.Pixel_XPickupTeachPosition_LT_S3;
                    _Pixel_XPickupControlPosition_LB = _currentProductRecipe.Pixel_XPickupTeachPosition_LB_S3;
                    _Pixel_XPickupControlPosition_RB = _currentProductRecipe.Pixel_XPickupTeachPosition_RB_S3;
                    _Pixel_YPickupControlPosition_RT = _currentProductRecipe.Pixel_YPickupTeachPosition_RT_S3;
                    _Pixel_YPickupControlPosition_LT = _currentProductRecipe.Pixel_YPickupTeachPosition_LT_S3;
                    _Pixel_YPickupControlPosition_LB = _currentProductRecipe.Pixel_YPickupTeachPosition_LB_S3;
                    _Pixel_YPickupControlPosition_RB = _currentProductRecipe.Pixel_YPickupTeachPosition_RB_S3;
                }
                catch (Exception)
                {
                    return false;
                }
            }

            hv_HomMat2D_CurrentTranslateMatrix_Pixel_S3 = newRobCamCalibration.coordTransAndRotMat(
                new float[] { _Pixel_XPickupControlPosition_RT, _Pixel_XPickupControlPosition_LT, _Pixel_XPickupControlPosition_LB, _Pixel_XPickupControlPosition_RB },
                new float[] { _Pixel_YPickupControlPosition_RT, _Pixel_YPickupControlPosition_LT, _Pixel_YPickupControlPosition_LB, _Pixel_YPickupControlPosition_RB },
                new float[] { Pixel_XPickupCurrentPosition_RT_S3, Pixel_XPickupCurrentPosition_LT_S3, Pixel_XPickupCurrentPosition_LB_S3, Pixel_XPickupCurrentPosition_RB_S3 },
                new float[] { Pixel_YPickupCurrentPosition_RT_S3, Pixel_YPickupCurrentPosition_LT_S3, Pixel_YPickupCurrentPosition_LB_S3, Pixel_YPickupCurrentPosition_RB_S3 });
            if (hv_HomMat2D_CurrentTranslateMatrix_Pixel_S3 == null)
            {
                MessageBox.Show("S3计算矩阵hv_HomMat2D_CurrentTranslateMatrix_Pixel失败!");
                return false;
            }
            return true;
        }
        #endregion

        #region ROB_PickupCurrentPosition:计算ROB新的取料位置
        private bool ROB_PickupCurrentPosition_S1()
        {
            HTuple hv_HomMat2dInvertCalib_9P;
            try
            {
                HOperatorSet.HomMat2dInvert(newRobCamCalibration.readDateRobAndCamCalib_9P(_GeneralParameterDirectoryPath + "//" + "9PCalibrate_S1.mat"), out hv_HomMat2dInvertCalib_9P);
            }
            catch (Exception ex)
            {
                return false;
            }
            if (!ActionForm._autoModeFlag)//手动时读取控件里的数据作为参数
            {
                float _Pixel_AnglePickupControl;
                float _ROB_XPickupControlPosition;
                float _ROB_YPickupControlPosition;
                float _ROB_ZPickupControlPosition;
                float _ROB_RXPickupTeachPosition;
                float _ROB_RWPickupTeachPosition;
                float _ROB_RZPickupTeachPosition;

                try
                {
                    _Pixel_AnglePickupControl = Convert.ToSingle(txtPixel_AnglePickupTeach_S1.Text);
                    _ROB_XPickupControlPosition = Convert.ToSingle(txtROB_XPickupTeachPosition_S1.Text);
                    _ROB_YPickupControlPosition = Convert.ToSingle(txtROB_YPickupTeachPosition_S1.Text);
                    _ROB_ZPickupControlPosition = Convert.ToSingle(txtROB_ZPickupTeachPosition_S1.Text);
                    _ROB_RXPickupTeachPosition = Convert.ToSingle(txtROB_RXPickupTeachPosition_S1.Text);
                    _ROB_RWPickupTeachPosition = Convert.ToSingle(txtROB_RWPickupTeachPosition_S1.Text);
                    _ROB_RZPickupTeachPosition = Convert.ToSingle(txtROB_RZPickupTeachPosition_S1.Text);

                }
                catch (Exception ex)
                {
                    MessageBox.Show("S1控件里输入的角度数据或者机械手取料位置坐标参数错误!");
                    return false;
                }

                Pixel_AnglePickupOffset_S1 = _Pixel_AnglePickupControl - Pixel_AnglePickupCurrent_S1;

                //当前下料位置的X坐标值
                //当前下料位置的Y坐标值
                ////方法1
                //if (!newRobCamCalibration.coordTransform(hv_HomMat2D_CurrentTranslateMatrix, _ROB_XPickupControlPosition, _ROB_YPickupControlPosition, ref ROB_XPickupCurrentPosition, ref ROB_YPickupCurrentPosition))
                //    return false;
                ////方法2
                //if (!newRobCamCalibration.coordTransform(hv_HomMat2dInvertCalib_9P, _ROB_XPickupControlPosition, _ROB_YPickupControlPosition, ref ROB_XPickupCurrentPosition, ref ROB_YPickupCurrentPosition))//ROB取料位置转换为像素坐标
                //    return false;
                //if (!newRobCamCalibration.coordTransform(hv_HomMat2D_CurrentTranslateMatrix_Pixel, ROB_XPickupCurrentPosition, ROB_YPickupCurrentPosition, ref ROB_XPickupCurrentPosition, ref ROB_YPickupCurrentPosition))// 
                //    return false;
                //if (!newRobCamCalibration.coordTransform(newRobCamCalibration.readDateRobAndCamCalib_9P(_GeneralParameterDirectoryPath + "//" + "9PCalibrate.mat"), ROB_XPickupCurrentPosition, ROB_YPickupCurrentPosition, ref ROB_XPickupCurrentPosition, ref ROB_YPickupCurrentPosition))
                //    return false;
                //方法3
                if (!newRobCamCalibration.coordTransform(hv_HomMat2dInvertCalib_9P, _ROB_XPickupControlPosition, _ROB_YPickupControlPosition, ref ROB_XPickupCurrentPosition_S1, ref ROB_YPickupCurrentPosition_S1))//ROB取料位置转换为像素坐标
                    return false;
                HTuple hv_HomMatRotate = newRobCamCalibration.coordTransAndRotMat(ROB_XPickupCurrentPosition_S1, ROB_YPickupCurrentPosition_S1, _Pixel_AnglePickupControl, ROB_XPickupCurrentPosition_S1, ROB_YPickupCurrentPosition_S1, Pixel_AnglePickupCurrent_S1);
                float x = 0;
                float y = 0;
                newRobCamCalibration.coordTransform(hv_HomMatRotate, Convert.ToSingle(txtPixel_XPickupTeachPosition_RT_S1.Text), Convert.ToSingle(txtPixel_YPickupTeachPosition_RT_S1.Text), ref x, ref y);
                if (!newRobCamCalibration.coordTransform(newRobCamCalibration.readDateRobAndCamCalib_9P(_GeneralParameterDirectoryPath + "//" + "9PCalibrate_S1.mat"), x, y, ref x, ref y))
                    return false;
                float x1 = 0;
                float y1 = 0;
                if (!newRobCamCalibration.coordTransform(newRobCamCalibration.readDateRobAndCamCalib_9P(_GeneralParameterDirectoryPath + "//" + "9PCalibrate_S1.mat"), Pixel_XPickupCurrentPosition_RT_S1, Pixel_YPickupCurrentPosition_RT_S1, ref x1, ref y1))
                    return false;
                float xOffset = x1 - x;
                float yOffset = y1 - y;
                ROB_XPickupCurrentPosition_S1 = _ROB_XPickupControlPosition + xOffset;
                ROB_YPickupCurrentPosition_S1 = _ROB_YPickupControlPosition + yOffset;
                ////方法4----错误
                //float xOffset4 = 0;
                //float yOffset4 = 0;
                //xOffset4 = ROB_XPickupCurrentPosition_RT - ROB_XPickupTeachPosition_RT_CurrentProduct;
                //yOffset4 = ROB_YPickupCurrentPosition_RT - ROB_YPickupTeachPosition_RT_CurrentProduct;
                //ROB_XPickupCurrentPosition = _ROB_XPickupControlPosition + xOffset4;
                //ROB_YPickupCurrentPosition = _ROB_YPickupControlPosition + yOffset4;
                ////方法5----错误
                //float x5 = 0;
                //float y5 = 0;
                //float x6 = 0;
                //float y6 = 0;
                //float xOffset5 = 0;
                //float yOffset5 = 0;
                //if (!newRobCamCalibration.coordTransform(newRobCamCalibration.readDateRobAndCamCalib_9P(_GeneralParameterDirectoryPath + "//" + "9PCalibrate.mat"), Pixel_XPickupCurrentPosition_RT, Pixel_YPickupCurrentPosition_RT, ref x5, ref y5))
                //    return false;
                //if (!newRobCamCalibration.coordTransform(newRobCamCalibration.readDateRobAndCamCalib_9P(_GeneralParameterDirectoryPath + "//" + "9PCalibrate.mat"), Convert.ToSingle(txtPixel_XPickupTeachPosition_RT.Text), Convert.ToSingle(txtPixel_YPickupTeachPosition_RT.Text), ref x6, ref y6))
                //    return false;
                //xOffset5 = x5 - x6;
                //yOffset5 = y5 - y6;
                //ROB_XPickupCurrentPosition = _ROB_XPickupControlPosition + xOffset5;
                //ROB_YPickupCurrentPosition = _ROB_YPickupControlPosition + yOffset5;

                //当前下料位置的Z坐标值
                ROB_ZPickupCurrentPosition_S1 = _ROB_ZPickupControlPosition;

                //当前下料位置的轴角度值
                ROB_RXPickupCurrentPosition_S1 = _ROB_RXPickupTeachPosition;
                ROB_RWPickupCurrentPosition_S1 = _ROB_RWPickupTeachPosition;
                ROB_RZPickupCurrentPosition_S1 = _ROB_RZPickupTeachPosition - Pixel_AnglePickupOffset_S1;
            }
            else//自动时读取产品参数文件数据
            {
                Pixel_AnglePickupOffset_S1 = _currentProductRecipe.Pixel_AnglePickupTeach_S1 - Pixel_AnglePickupCurrent_S1;
                //当前下料位置的X坐标值
                //当前下料位置的Y坐标值
                ////方法1
                //if (!newRobCamCalibration.coordTransform(hv_HomMat2D_CurrentTranslateMatrix, _currentProductRecipe.ROB_XPickupTeachPosition, _currentProductRecipe.ROB_YPickupTeachPosition, ref ROB_XPickupCurrentPosition, ref ROB_YPickupCurrentPosition))
                //    return false;
                //方法3
                if (!newRobCamCalibration.coordTransform(hv_HomMat2dInvertCalib_9P, _currentProductRecipe.ROB_XPickupTeachPosition_S1, _currentProductRecipe.ROB_YPickupTeachPosition_S1, ref ROB_XPickupCurrentPosition_S1, ref ROB_YPickupCurrentPosition_S1))//ROB取料位置转换为像素坐标
                    return false;
                HTuple hv_HomMatRotate = newRobCamCalibration.coordTransAndRotMat(ROB_XPickupCurrentPosition_S1, ROB_YPickupCurrentPosition_S1, _currentProductRecipe.Pixel_AnglePickupTeach_S1, ROB_XPickupCurrentPosition_S1, ROB_YPickupCurrentPosition_S1, Pixel_AnglePickupCurrent_S1);
                float x = 0;
                float y = 0;
                newRobCamCalibration.coordTransform(hv_HomMatRotate, _currentProductRecipe.Pixel_XPickupTeachPosition_RT_S1, _currentProductRecipe.Pixel_YPickupTeachPosition_RT_S1, ref x, ref y);
                if (!newRobCamCalibration.coordTransform(newRobCamCalibration.readDateRobAndCamCalib_9P(_GeneralParameterDirectoryPath + "//" + "9PCalibrate_S1.mat"), x, y, ref x, ref y))
                    return false;
                float x1 = 0;
                float y1 = 0;
                if (!newRobCamCalibration.coordTransform(newRobCamCalibration.readDateRobAndCamCalib_9P(_GeneralParameterDirectoryPath + "//" + "9PCalibrate_S1.mat"), Pixel_XPickupCurrentPosition_RT_S1, Pixel_YPickupCurrentPosition_RT_S1, ref x1, ref y1))
                    return false;
                float xOffset = x1 - x;
                float yOffset = y1 - y;
                ROB_XPickupCurrentPosition_S1 = _currentProductRecipe.ROB_XPickupTeachPosition_S1 + xOffset;
                ROB_YPickupCurrentPosition_S1 = _currentProductRecipe.ROB_YPickupTeachPosition_S1 + yOffset;

                //当前下料位置的Z坐标值
                ROB_ZPickupCurrentPosition_S1 = _currentProductRecipe.ROB_ZPickupTeachPosition_S1;

                //当前下料位置的轴角度值
                ROB_RXPickupCurrentPosition_S1 = _currentProductRecipe.ROB_RXPickupTeachPosition_S1;
                ROB_RWPickupCurrentPosition_S1 = _currentProductRecipe.ROB_RWPickupTeachPosition_S1;
                ROB_RZPickupCurrentPosition_S1 = _currentProductRecipe.ROB_RZPickupTeachPosition_S1 - Pixel_AnglePickupOffset_S1;
            }
            if (this.IsHandleCreated)
            {
                txtROB_PickupCurrentPositionDisplay_1_S1.BeginInvoke(new Action(() =>
                {
                    txtROB_PickupCurrentPositionDisplay_1_S1.Text = "X:" + ROB_XPickupCurrentPosition_S1.ToString("0.000") + ",Y:" + ROB_YPickupCurrentPosition_S1.ToString("0.000") + ",Z:" + ROB_ZPickupCurrentPosition_S1.ToString("0.000");
                    txtROB_PickupCurrentPositionDisplay_2_S1.Text = "RX:" + ROB_RXPickupCurrentPosition_S1.ToString("0.000") + ",RW:" + ROB_RWPickupCurrentPosition_S1.ToString("0.000") + ",RZ:" + ROB_RZPickupCurrentPosition_S1.ToString("0.000");
                }));

            }
            return true;
        }
        private bool ROB_PickupCurrentPosition_S2()
        {
            HTuple hv_HomMat2dInvertCalib_9P;
            try
            {
                HOperatorSet.HomMat2dInvert(newRobCamCalibration.readDateRobAndCamCalib_9P(_GeneralParameterDirectoryPath + "//" + "9PCalibrate_S2.mat"), out hv_HomMat2dInvertCalib_9P);
            }
            catch (Exception ex)
            {
                return false;
            }
            if (!ActionForm._autoModeFlag)//手动时读取控件里的数据作为参数
            {
                float _Pixel_AnglePickupControl;
                float _ROB_XPickupControlPosition;
                float _ROB_YPickupControlPosition;
                float _ROB_ZPickupControlPosition;
                float _ROB_RXPickupTeachPosition;
                float _ROB_RWPickupTeachPosition;
                float _ROB_RZPickupTeachPosition;

                try
                {
                    _Pixel_AnglePickupControl = Convert.ToSingle(txtPixel_AnglePickupTeach_S2.Text);
                    _ROB_XPickupControlPosition = Convert.ToSingle(txtROB_XPickupTeachPosition_S2.Text);
                    _ROB_YPickupControlPosition = Convert.ToSingle(txtROB_YPickupTeachPosition_S2.Text);
                    _ROB_ZPickupControlPosition = Convert.ToSingle(txtROB_ZPickupTeachPosition_S2.Text);
                    _ROB_RXPickupTeachPosition = Convert.ToSingle(txtROB_RXPickupTeachPosition_S2.Text);
                    _ROB_RWPickupTeachPosition = Convert.ToSingle(txtROB_RWPickupTeachPosition_S2.Text);
                    _ROB_RZPickupTeachPosition = Convert.ToSingle(txtROB_RZPickupTeachPosition_S2.Text);

                }
                catch (Exception ex)
                {
                    MessageBox.Show("S2控件里输入的角度数据或者机械手取料位置坐标参数错误!");
                    return false;
                }

                Pixel_AnglePickupOffset_S2 = _Pixel_AnglePickupControl - Pixel_AnglePickupCurrent_S2;

                //当前下料位置的X坐标值
                //当前下料位置的Y坐标值
                ////方法1
                //if (!newRobCamCalibration.coordTransform(hv_HomMat2D_CurrentTranslateMatrix, _ROB_XPickupControlPosition, _ROB_YPickupControlPosition, ref ROB_XPickupCurrentPosition, ref ROB_YPickupCurrentPosition))
                //    return false;
                ////方法2
                //if (!newRobCamCalibration.coordTransform(hv_HomMat2dInvertCalib_9P, _ROB_XPickupControlPosition, _ROB_YPickupControlPosition, ref ROB_XPickupCurrentPosition, ref ROB_YPickupCurrentPosition))//ROB取料位置转换为像素坐标
                //    return false;
                //if (!newRobCamCalibration.coordTransform(hv_HomMat2D_CurrentTranslateMatrix_Pixel, ROB_XPickupCurrentPosition, ROB_YPickupCurrentPosition, ref ROB_XPickupCurrentPosition, ref ROB_YPickupCurrentPosition))// 
                //    return false;
                //if (!newRobCamCalibration.coordTransform(newRobCamCalibration.readDateRobAndCamCalib_9P(_GeneralParameterDirectoryPath + "//" + "9PCalibrate.mat"), ROB_XPickupCurrentPosition, ROB_YPickupCurrentPosition, ref ROB_XPickupCurrentPosition, ref ROB_YPickupCurrentPosition))
                //    return false;
                //方法3
                if (!newRobCamCalibration.coordTransform(hv_HomMat2dInvertCalib_9P, _ROB_XPickupControlPosition, _ROB_YPickupControlPosition, ref ROB_XPickupCurrentPosition_S2, ref ROB_YPickupCurrentPosition_S2))//ROB取料位置转换为像素坐标
                    return false;
                HTuple hv_HomMatRotate = newRobCamCalibration.coordTransAndRotMat(ROB_XPickupCurrentPosition_S2, ROB_YPickupCurrentPosition_S2, _Pixel_AnglePickupControl, ROB_XPickupCurrentPosition_S2, ROB_YPickupCurrentPosition_S2, Pixel_AnglePickupCurrent_S2);
                float x = 0;
                float y = 0;
                newRobCamCalibration.coordTransform(hv_HomMatRotate, Convert.ToSingle(txtPixel_XPickupTeachPosition_RT_S2.Text), Convert.ToSingle(txtPixel_YPickupTeachPosition_RT_S2.Text), ref x, ref y);
                if (!newRobCamCalibration.coordTransform(newRobCamCalibration.readDateRobAndCamCalib_9P(_GeneralParameterDirectoryPath + "//" + "9PCalibrate_S2.mat"), x, y, ref x, ref y))
                    return false;
                float x1 = 0;
                float y1 = 0;
                if (!newRobCamCalibration.coordTransform(newRobCamCalibration.readDateRobAndCamCalib_9P(_GeneralParameterDirectoryPath + "//" + "9PCalibrate_S2.mat"), Pixel_XPickupCurrentPosition_RT_S2, Pixel_YPickupCurrentPosition_RT_S2, ref x1, ref y1))
                    return false;
                float xOffset = x1 - x;
                float yOffset = y1 - y;
                ROB_XPickupCurrentPosition_S2 = _ROB_XPickupControlPosition + xOffset;
                ROB_YPickupCurrentPosition_S2 = _ROB_YPickupControlPosition + yOffset;
                ////方法4----错误
                //float xOffset4 = 0;
                //float yOffset4 = 0;
                //xOffset4 = ROB_XPickupCurrentPosition_RT - ROB_XPickupTeachPosition_RT_CurrentProduct;
                //yOffset4 = ROB_YPickupCurrentPosition_RT - ROB_YPickupTeachPosition_RT_CurrentProduct;
                //ROB_XPickupCurrentPosition = _ROB_XPickupControlPosition + xOffset4;
                //ROB_YPickupCurrentPosition = _ROB_YPickupControlPosition + yOffset4;
                ////方法5----错误
                //float x5 = 0;
                //float y5 = 0;
                //float x6 = 0;
                //float y6 = 0;
                //float xOffset5 = 0;
                //float yOffset5 = 0;
                //if (!newRobCamCalibration.coordTransform(newRobCamCalibration.readDateRobAndCamCalib_9P(_GeneralParameterDirectoryPath + "//" + "9PCalibrate.mat"), Pixel_XPickupCurrentPosition_RT, Pixel_YPickupCurrentPosition_RT, ref x5, ref y5))
                //    return false;
                //if (!newRobCamCalibration.coordTransform(newRobCamCalibration.readDateRobAndCamCalib_9P(_GeneralParameterDirectoryPath + "//" + "9PCalibrate.mat"), Convert.ToSingle(txtPixel_XPickupTeachPosition_RT.Text), Convert.ToSingle(txtPixel_YPickupTeachPosition_RT.Text), ref x6, ref y6))
                //    return false;
                //xOffset5 = x5 - x6;
                //yOffset5 = y5 - y6;
                //ROB_XPickupCurrentPosition = _ROB_XPickupControlPosition + xOffset5;
                //ROB_YPickupCurrentPosition = _ROB_YPickupControlPosition + yOffset5;

                //当前下料位置的Z坐标值
                ROB_ZPickupCurrentPosition_S2 = _ROB_ZPickupControlPosition;

                //当前下料位置的轴角度值
                ROB_RXPickupCurrentPosition_S2 = _ROB_RXPickupTeachPosition;
                ROB_RWPickupCurrentPosition_S2 = _ROB_RWPickupTeachPosition;
                ROB_RZPickupCurrentPosition_S2 = _ROB_RZPickupTeachPosition - Pixel_AnglePickupOffset_S2;
            }
            else//自动时读取产品参数文件数据
            {
                Pixel_AnglePickupOffset_S2 = _currentProductRecipe.Pixel_AnglePickupTeach_S2 - Pixel_AnglePickupCurrent_S2;
                //当前下料位置的X坐标值
                //当前下料位置的Y坐标值
                ////方法1
                //if (!newRobCamCalibration.coordTransform(hv_HomMat2D_CurrentTranslateMatrix, _currentProductRecipe.ROB_XPickupTeachPosition, _currentProductRecipe.ROB_YPickupTeachPosition, ref ROB_XPickupCurrentPosition, ref ROB_YPickupCurrentPosition))
                //    return false;
                //方法3
                if (!newRobCamCalibration.coordTransform(hv_HomMat2dInvertCalib_9P, _currentProductRecipe.ROB_XPickupTeachPosition_S2, _currentProductRecipe.ROB_YPickupTeachPosition_S2, ref ROB_XPickupCurrentPosition_S2, ref ROB_YPickupCurrentPosition_S2))//ROB取料位置转换为像素坐标
                    return false;
                HTuple hv_HomMatRotate = newRobCamCalibration.coordTransAndRotMat(ROB_XPickupCurrentPosition_S2, ROB_YPickupCurrentPosition_S2, _currentProductRecipe.Pixel_AnglePickupTeach_S2, ROB_XPickupCurrentPosition_S2, ROB_YPickupCurrentPosition_S2, Pixel_AnglePickupCurrent_S2);
                float x = 0;
                float y = 0;
                newRobCamCalibration.coordTransform(hv_HomMatRotate, _currentProductRecipe.Pixel_XPickupTeachPosition_RT_S2, _currentProductRecipe.Pixel_YPickupTeachPosition_RT_S2, ref x, ref y);
                if (!newRobCamCalibration.coordTransform(newRobCamCalibration.readDateRobAndCamCalib_9P(_GeneralParameterDirectoryPath + "//" + "9PCalibrate_S2.mat"), x, y, ref x, ref y))
                    return false;
                float x1 = 0;
                float y1 = 0;
                if (!newRobCamCalibration.coordTransform(newRobCamCalibration.readDateRobAndCamCalib_9P(_GeneralParameterDirectoryPath + "//" + "9PCalibrate_S2.mat"), Pixel_XPickupCurrentPosition_RT_S2, Pixel_YPickupCurrentPosition_RT_S2, ref x1, ref y1))
                    return false;
                float xOffset = x1 - x;
                float yOffset = y1 - y;
                ROB_XPickupCurrentPosition_S2 = _currentProductRecipe.ROB_XPickupTeachPosition_S2 + xOffset;
                ROB_YPickupCurrentPosition_S2 = _currentProductRecipe.ROB_YPickupTeachPosition_S2 + yOffset;

                //当前下料位置的Z坐标值
                ROB_ZPickupCurrentPosition_S2 = _currentProductRecipe.ROB_ZPickupTeachPosition_S2;

                //当前下料位置的轴角度值
                ROB_RXPickupCurrentPosition_S2 = _currentProductRecipe.ROB_RXPickupTeachPosition_S2;
                ROB_RWPickupCurrentPosition_S2 = _currentProductRecipe.ROB_RWPickupTeachPosition_S2;
                ROB_RZPickupCurrentPosition_S2 = _currentProductRecipe.ROB_RZPickupTeachPosition_S2 - Pixel_AnglePickupOffset_S2;
            }
            if (this.IsHandleCreated)
            {
                txtROB_PickupCurrentPositionDisplay_1_S2.BeginInvoke(new Action(() =>
                {
                    txtROB_PickupCurrentPositionDisplay_1_S2.Text = "X:" + ROB_XPickupCurrentPosition_S2.ToString("0.000") + ",Y:" + ROB_YPickupCurrentPosition_S2.ToString("0.000") + ",Z:" + ROB_ZPickupCurrentPosition_S2.ToString("0.000");
                    txtROB_PickupCurrentPositionDisplay_2_S2.Text = "RX:" + ROB_RXPickupCurrentPosition_S2.ToString("0.000") + ",RW:" + ROB_RWPickupCurrentPosition_S2.ToString("0.000") + ",RZ:" + ROB_RZPickupCurrentPosition_S2.ToString("0.000");
                }));

            }
            return true;
        }
        private bool ROB_PickupCurrentPosition_S3()
        {
            HTuple hv_HomMat2dInvertCalib_9P;
            try
            {
                HOperatorSet.HomMat2dInvert(newRobCamCalibration.readDateRobAndCamCalib_9P(_GeneralParameterDirectoryPath + "//" + "9PCalibrate_S3.mat"), out hv_HomMat2dInvertCalib_9P);
            }
            catch (Exception ex)
            {
                return false;
            }
            if (!ActionForm._autoModeFlag)//手动时读取控件里的数据作为参数
            {
                float _Pixel_AnglePickupControl;
                float _ROB_XPickupControlPosition;
                float _ROB_YPickupControlPosition;
                float _ROB_ZPickupControlPosition;
                float _ROB_RXPickupTeachPosition;
                float _ROB_RWPickupTeachPosition;
                float _ROB_RZPickupTeachPosition;

                try
                {
                    _Pixel_AnglePickupControl = Convert.ToSingle(txtPixel_AnglePickupTeach_S3.Text);
                    _ROB_XPickupControlPosition = Convert.ToSingle(txtROB_XPickupTeachPosition_S3.Text);
                    _ROB_YPickupControlPosition = Convert.ToSingle(txtROB_YPickupTeachPosition_S3.Text);
                    _ROB_ZPickupControlPosition = Convert.ToSingle(txtROB_ZPickupTeachPosition_S3.Text);
                    _ROB_RXPickupTeachPosition = Convert.ToSingle(txtROB_RXPickupTeachPosition_S3.Text);
                    _ROB_RWPickupTeachPosition = Convert.ToSingle(txtROB_RWPickupTeachPosition_S3.Text);
                    _ROB_RZPickupTeachPosition = Convert.ToSingle(txtROB_RZPickupTeachPosition_S3.Text);

                }
                catch (Exception ex)
                {
                    MessageBox.Show("S3控件里输入的角度数据或者机械手取料位置坐标参数错误!");
                    return false;
                }

                Pixel_AnglePickupOffset_S3 = _Pixel_AnglePickupControl - Pixel_AnglePickupCurrent_S3;

                //当前下料位置的X坐标值
                //当前下料位置的Y坐标值
                ////方法1
                //if (!newRobCamCalibration.coordTransform(hv_HomMat2D_CurrentTranslateMatrix, _ROB_XPickupControlPosition, _ROB_YPickupControlPosition, ref ROB_XPickupCurrentPosition, ref ROB_YPickupCurrentPosition))
                //    return false;
                ////方法2
                //if (!newRobCamCalibration.coordTransform(hv_HomMat2dInvertCalib_9P, _ROB_XPickupControlPosition, _ROB_YPickupControlPosition, ref ROB_XPickupCurrentPosition, ref ROB_YPickupCurrentPosition))//ROB取料位置转换为像素坐标
                //    return false;
                //if (!newRobCamCalibration.coordTransform(hv_HomMat2D_CurrentTranslateMatrix_Pixel, ROB_XPickupCurrentPosition, ROB_YPickupCurrentPosition, ref ROB_XPickupCurrentPosition, ref ROB_YPickupCurrentPosition))// 
                //    return false;
                //if (!newRobCamCalibration.coordTransform(newRobCamCalibration.readDateRobAndCamCalib_9P(_GeneralParameterDirectoryPath + "//" + "9PCalibrate.mat"), ROB_XPickupCurrentPosition, ROB_YPickupCurrentPosition, ref ROB_XPickupCurrentPosition, ref ROB_YPickupCurrentPosition))
                //    return false;
                //方法3
                if (!newRobCamCalibration.coordTransform(hv_HomMat2dInvertCalib_9P, _ROB_XPickupControlPosition, _ROB_YPickupControlPosition, ref ROB_XPickupCurrentPosition_S3, ref ROB_YPickupCurrentPosition_S3))//ROB取料位置转换为像素坐标
                    return false;
                HTuple hv_HomMatRotate = newRobCamCalibration.coordTransAndRotMat(ROB_XPickupCurrentPosition_S3, ROB_YPickupCurrentPosition_S3, _Pixel_AnglePickupControl, ROB_XPickupCurrentPosition_S3, ROB_YPickupCurrentPosition_S3, Pixel_AnglePickupCurrent_S3);
                float x = 0;
                float y = 0;
                newRobCamCalibration.coordTransform(hv_HomMatRotate, Convert.ToSingle(txtPixel_XPickupTeachPosition_RT_S3.Text), Convert.ToSingle(txtPixel_YPickupTeachPosition_RT_S3.Text), ref x, ref y);
                if (!newRobCamCalibration.coordTransform(newRobCamCalibration.readDateRobAndCamCalib_9P(_GeneralParameterDirectoryPath + "//" + "9PCalibrate_S3.mat"), x, y, ref x, ref y))
                    return false;
                float x1 = 0;
                float y1 = 0;
                if (!newRobCamCalibration.coordTransform(newRobCamCalibration.readDateRobAndCamCalib_9P(_GeneralParameterDirectoryPath + "//" + "9PCalibrate_S3.mat"), Pixel_XPickupCurrentPosition_RT_S3, Pixel_YPickupCurrentPosition_RT_S3, ref x1, ref y1))
                    return false;
                float xOffset = x1 - x;
                float yOffset = y1 - y;
                ROB_XPickupCurrentPosition_S3 = _ROB_XPickupControlPosition + xOffset;
                ROB_YPickupCurrentPosition_S3 = _ROB_YPickupControlPosition + yOffset;
                ////方法4----错误
                //float xOffset4 = 0;
                //float yOffset4 = 0;
                //xOffset4 = ROB_XPickupCurrentPosition_RT - ROB_XPickupTeachPosition_RT_CurrentProduct;
                //yOffset4 = ROB_YPickupCurrentPosition_RT - ROB_YPickupTeachPosition_RT_CurrentProduct;
                //ROB_XPickupCurrentPosition = _ROB_XPickupControlPosition + xOffset4;
                //ROB_YPickupCurrentPosition = _ROB_YPickupControlPosition + yOffset4;
                ////方法5----错误
                //float x5 = 0;
                //float y5 = 0;
                //float x6 = 0;
                //float y6 = 0;
                //float xOffset5 = 0;
                //float yOffset5 = 0;
                //if (!newRobCamCalibration.coordTransform(newRobCamCalibration.readDateRobAndCamCalib_9P(_GeneralParameterDirectoryPath + "//" + "9PCalibrate.mat"), Pixel_XPickupCurrentPosition_RT, Pixel_YPickupCurrentPosition_RT, ref x5, ref y5))
                //    return false;
                //if (!newRobCamCalibration.coordTransform(newRobCamCalibration.readDateRobAndCamCalib_9P(_GeneralParameterDirectoryPath + "//" + "9PCalibrate.mat"), Convert.ToSingle(txtPixel_XPickupTeachPosition_RT.Text), Convert.ToSingle(txtPixel_YPickupTeachPosition_RT.Text), ref x6, ref y6))
                //    return false;
                //xOffset5 = x5 - x6;
                //yOffset5 = y5 - y6;
                //ROB_XPickupCurrentPosition = _ROB_XPickupControlPosition + xOffset5;
                //ROB_YPickupCurrentPosition = _ROB_YPickupControlPosition + yOffset5;

                //当前下料位置的Z坐标值
                ROB_ZPickupCurrentPosition_S3 = _ROB_ZPickupControlPosition;

                //当前下料位置的轴角度值
                ROB_RXPickupCurrentPosition_S3 = _ROB_RXPickupTeachPosition;
                ROB_RWPickupCurrentPosition_S3 = _ROB_RWPickupTeachPosition;
                ROB_RZPickupCurrentPosition_S3 = _ROB_RZPickupTeachPosition - Pixel_AnglePickupOffset_S3;
            }
            else//自动时读取产品参数文件数据
            {
                Pixel_AnglePickupOffset_S3 = _currentProductRecipe.Pixel_AnglePickupTeach_S3 - Pixel_AnglePickupCurrent_S3;
                //当前下料位置的X坐标值
                //当前下料位置的Y坐标值
                ////方法1
                //if (!newRobCamCalibration.coordTransform(hv_HomMat2D_CurrentTranslateMatrix, _currentProductRecipe.ROB_XPickupTeachPosition, _currentProductRecipe.ROB_YPickupTeachPosition, ref ROB_XPickupCurrentPosition, ref ROB_YPickupCurrentPosition))
                //    return false;
                //方法3
                if (!newRobCamCalibration.coordTransform(hv_HomMat2dInvertCalib_9P, _currentProductRecipe.ROB_XPickupTeachPosition_S3, _currentProductRecipe.ROB_YPickupTeachPosition_S3, ref ROB_XPickupCurrentPosition_S3, ref ROB_YPickupCurrentPosition_S3))//ROB取料位置转换为像素坐标
                    return false;
                HTuple hv_HomMatRotate = newRobCamCalibration.coordTransAndRotMat(ROB_XPickupCurrentPosition_S3, ROB_YPickupCurrentPosition_S3, _currentProductRecipe.Pixel_AnglePickupTeach_S3, ROB_XPickupCurrentPosition_S3, ROB_YPickupCurrentPosition_S3, Pixel_AnglePickupCurrent_S3);
                float x = 0;
                float y = 0;
                newRobCamCalibration.coordTransform(hv_HomMatRotate, _currentProductRecipe.Pixel_XPickupTeachPosition_RT_S3, _currentProductRecipe.Pixel_YPickupTeachPosition_RT_S3, ref x, ref y);
                if (!newRobCamCalibration.coordTransform(newRobCamCalibration.readDateRobAndCamCalib_9P(_GeneralParameterDirectoryPath + "//" + "9PCalibrate_S3.mat"), x, y, ref x, ref y))
                    return false;
                float x1 = 0;
                float y1 = 0;
                if (!newRobCamCalibration.coordTransform(newRobCamCalibration.readDateRobAndCamCalib_9P(_GeneralParameterDirectoryPath + "//" + "9PCalibrate_S3.mat"), Pixel_XPickupCurrentPosition_RT_S3, Pixel_YPickupCurrentPosition_RT_S3, ref x1, ref y1))
                    return false;
                float xOffset = x1 - x;
                float yOffset = y1 - y;
                ROB_XPickupCurrentPosition_S3 = _currentProductRecipe.ROB_XPickupTeachPosition_S3 + xOffset;
                ROB_YPickupCurrentPosition_S3 = _currentProductRecipe.ROB_YPickupTeachPosition_S3 + yOffset;

                //当前下料位置的Z坐标值
                ROB_ZPickupCurrentPosition_S3 = _currentProductRecipe.ROB_ZPickupTeachPosition_S3;

                //当前下料位置的轴角度值
                ROB_RXPickupCurrentPosition_S3 = _currentProductRecipe.ROB_RXPickupTeachPosition_S3;
                ROB_RWPickupCurrentPosition_S3 = _currentProductRecipe.ROB_RWPickupTeachPosition_S3;
                ROB_RZPickupCurrentPosition_S3 = _currentProductRecipe.ROB_RZPickupTeachPosition_S3 - Pixel_AnglePickupOffset_S3;
            }
            if (this.IsHandleCreated)
            {
                txtROB_PickupCurrentPositionDisplay_1_S3.BeginInvoke(new Action(() =>
                {
                    txtROB_PickupCurrentPositionDisplay_1_S3.Text = "X:" + ROB_XPickupCurrentPosition_S3.ToString("0.000") + ",Y:" + ROB_YPickupCurrentPosition_S3.ToString("0.000") + ",Z:" + ROB_ZPickupCurrentPosition_S3.ToString("0.000");
                    txtROB_PickupCurrentPositionDisplay_2_S3.Text = "RX:" + ROB_RXPickupCurrentPosition_S3.ToString("0.000") + ",RW:" + ROB_RWPickupCurrentPosition_S3.ToString("0.000") + ",RZ:" + ROB_RZPickupCurrentPosition_S3.ToString("0.000");
                }));

            }
            return true;
        }
        #endregion

        #region ROB_PickupCurrentPositionCheck:机械手取料位置数据检查
        private int ROB_PickupCurrentPositionCheck_S1()
        {
            if (!ActionForm._autoModeFlag)//手动时读取控件里的数据作为参数
            {
                try
                {
                    ROB_XPickupCurrentPositionLimitMin_S1 = Convert.ToSingle(txtROB_XPickupCurrentPositionLimitMin_S1.Text);
                    ROB_XPickupCurrentPositionLimitMax_S1 = Convert.ToSingle(txtROB_XPickupCurrentPositionLimitMax_S1.Text);
                    ROB_YPickupCurrentPositionLimitMin_S1 = Convert.ToSingle(txtROB_YPickupCurrentPositionLimitMin_S1.Text);
                    ROB_YPickupCurrentPositionLimitMax_S1 = Convert.ToSingle(txtROB_YPickupCurrentPositionLimitMax_S1.Text);
                    ROB_RZPickupCurrentPositionLimitMin_S1 = Convert.ToSingle(txtROB_RZPickupCurrentPositionLimitMin_S1.Text);
                    ROB_RZPickupCurrentPositionLimitMax_S1 = Convert.ToSingle(txtROB_RZPickupCurrentPositionLimitMax_S1.Text);
                }
                catch (Exception ex)
                {
                    //MessageBox.Show("S1控件里输入的机械手取料位置限制参数错误!");
                    MyTool.TxtFileProcess.CreateLog("S1控件里输入的机械手取料位置限制参数错误!" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    return -1;
                }
            }
            else//自动时读取产品参数文件数据
            {
                ROB_XPickupCurrentPositionLimitMin_S1 = _currentProductRecipe.ROB_XPickupCurrentPositionLimitMin_S1;
                ROB_XPickupCurrentPositionLimitMax_S1 = _currentProductRecipe.ROB_XPickupCurrentPositionLimitMax_S1;
                ROB_YPickupCurrentPositionLimitMin_S1 = _currentProductRecipe.ROB_YPickupCurrentPositionLimitMin_S1;
                ROB_YPickupCurrentPositionLimitMax_S1 = _currentProductRecipe.ROB_YPickupCurrentPositionLimitMax_S1;
                ROB_RZPickupCurrentPositionLimitMin_S1 = _currentProductRecipe.ROB_RZPickupCurrentPositionLimitMin_S1;
                ROB_RZPickupCurrentPositionLimitMax_S1 = _currentProductRecipe.ROB_RZPickupCurrentPositionLimitMax_S1;
            }

            if (ROB_XPickupCurrentPosition_S1 < ROB_XPickupCurrentPositionLimitMin_S1 || ROB_XPickupCurrentPosition_S1 > ROB_XPickupCurrentPositionLimitMax_S1)
            {
                //MessageBox.Show("S1机械手取料位置X坐标值超出限制条件的范围." + "当前值为:" +
                //    ROB_XPickupCurrentPosition_S1.ToString() + ",限制条件的范围是:" + "[" + ROB_XPickupCurrentPositionLimitMin_S1.ToString() + ","
                //    + ROB_XPickupCurrentPositionLimitMax_S1.ToString()
                //    + "]");
                MyTool.TxtFileProcess.CreateLog("S1机械手取料位置X坐标值超出限制条件的范围." + "当前值为:" +
                    ROB_XPickupCurrentPosition_S1.ToString() + ",限制条件的范围是:" + "[" + ROB_XPickupCurrentPositionLimitMin_S1.ToString() + ","
                    + ROB_XPickupCurrentPositionLimitMax_S1.ToString()
                    + "]" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                return 1;
            }
            if (ROB_YPickupCurrentPosition_S1 < ROB_YPickupCurrentPositionLimitMin_S1 || ROB_YPickupCurrentPosition_S1 > ROB_YPickupCurrentPositionLimitMax_S1)
            {
                //MessageBox.Show("S1机械手取料位置Y坐标值超出限制条件的范围." + "当前值为:" +
                //    ROB_YPickupCurrentPosition_S1.ToString() + ",限制条件的范围是:" + "[" + ROB_YPickupCurrentPositionLimitMin_S1.ToString() + ","
                //    + ROB_YPickupCurrentPositionLimitMax_S1.ToString()
                //    + "]");
                MyTool.TxtFileProcess.CreateLog("S1机械手取料位置Y坐标值超出限制条件的范围." + "当前值为:" +
                    ROB_YPickupCurrentPosition_S1.ToString() + ",限制条件的范围是:" + "[" + ROB_YPickupCurrentPositionLimitMin_S1.ToString() + ","
                    + ROB_YPickupCurrentPositionLimitMax_S1.ToString()
                    + "]" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                return 2;
            }
            if (ROB_RZPickupCurrentPosition_S1 < ROB_RZPickupCurrentPositionLimitMin_S1 || ROB_RZPickupCurrentPosition_S1 > ROB_RZPickupCurrentPositionLimitMax_S1)
            {
                //MessageBox.Show("S1机械手取料位置RZ坐标值超出限制条件的范围." + "当前值为:" +
                //    ROB_RZPickupCurrentPosition_S1.ToString() + ",限制条件的范围是:" + "[" + ROB_RZPickupCurrentPositionLimitMin_S1.ToString() + ","
                //    + ROB_RZPickupCurrentPositionLimitMax_S1.ToString()
                //    + "]");
                MyTool.TxtFileProcess.CreateLog("S1机械手取料位置RZ坐标值超出限制条件的范围." + "当前值为:" +
                    ROB_RZPickupCurrentPosition_S1.ToString() + ",限制条件的范围是:" + "[" + ROB_RZPickupCurrentPositionLimitMin_S1.ToString() + ","
                    + ROB_RZPickupCurrentPositionLimitMax_S1.ToString()
                    + "]" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                return 3;
            }
            return 0;
        }

        private int ROB_PickupCurrentPositionCheck_S2()
        {
            if (!ActionForm._autoModeFlag)//手动时读取控件里的数据作为参数
            {
                try
                {
                    ROB_XPickupCurrentPositionLimitMin_S2 = Convert.ToSingle(txtROB_XPickupCurrentPositionLimitMin_S2.Text);
                    ROB_XPickupCurrentPositionLimitMax_S2 = Convert.ToSingle(txtROB_XPickupCurrentPositionLimitMax_S2.Text);
                    ROB_YPickupCurrentPositionLimitMin_S2 = Convert.ToSingle(txtROB_YPickupCurrentPositionLimitMin_S2.Text);
                    ROB_YPickupCurrentPositionLimitMax_S2 = Convert.ToSingle(txtROB_YPickupCurrentPositionLimitMax_S2.Text);
                    ROB_RZPickupCurrentPositionLimitMin_S2 = Convert.ToSingle(txtROB_RZPickupCurrentPositionLimitMin_S2.Text);
                    ROB_RZPickupCurrentPositionLimitMax_S2 = Convert.ToSingle(txtROB_RZPickupCurrentPositionLimitMax_S2.Text);
                }
                catch (Exception ex)
                {
                    //MessageBox.Show("S2控件里输入的机械手取料位置限制参数错误!");
                    MyTool.TxtFileProcess.CreateLog("S2控件里输入的机械手取料位置限制参数错误!" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    return -1;
                }
            }
            else//自动时读取产品参数文件数据
            {
                ROB_XPickupCurrentPositionLimitMin_S2 = _currentProductRecipe.ROB_XPickupCurrentPositionLimitMin_S2;
                ROB_XPickupCurrentPositionLimitMax_S2 = _currentProductRecipe.ROB_XPickupCurrentPositionLimitMax_S2;
                ROB_YPickupCurrentPositionLimitMin_S2 = _currentProductRecipe.ROB_YPickupCurrentPositionLimitMin_S2;
                ROB_YPickupCurrentPositionLimitMax_S2 = _currentProductRecipe.ROB_YPickupCurrentPositionLimitMax_S2;
                ROB_RZPickupCurrentPositionLimitMin_S2 = _currentProductRecipe.ROB_RZPickupCurrentPositionLimitMin_S2;
                ROB_RZPickupCurrentPositionLimitMax_S2 = _currentProductRecipe.ROB_RZPickupCurrentPositionLimitMax_S2;
            }

            if (ROB_XPickupCurrentPosition_S2 < ROB_XPickupCurrentPositionLimitMin_S2 || ROB_XPickupCurrentPosition_S2 > ROB_XPickupCurrentPositionLimitMax_S2)
            {
                //MessageBox.Show("S2机械手取料位置X坐标值超出限制条件的范围." + "当前值为:" +
                //    ROB_XPickupCurrentPosition_S2.ToString() + ",限制条件的范围是:" + "[" + ROB_XPickupCurrentPositionLimitMin_S2.ToString() + ","
                //    + ROB_XPickupCurrentPositionLimitMax_S2.ToString()
                //    + "]");
                MyTool.TxtFileProcess.CreateLog("S2机械手取料位置X坐标值超出限制条件的范围." + "当前值为:" +
                    ROB_XPickupCurrentPosition_S2.ToString() + ",限制条件的范围是:" + "[" + ROB_XPickupCurrentPositionLimitMin_S2.ToString() + ","
                    + ROB_XPickupCurrentPositionLimitMax_S2.ToString()
                    + "]" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                return 1;
            }
            if (ROB_YPickupCurrentPosition_S2 < ROB_YPickupCurrentPositionLimitMin_S2 || ROB_YPickupCurrentPosition_S2 > ROB_YPickupCurrentPositionLimitMax_S2)
            {
                //MessageBox.Show("S2机械手取料位置Y坐标值超出限制条件的范围." + "当前值为:" +
                //    ROB_YPickupCurrentPosition_S2.ToString() + ",限制条件的范围是:" + "[" + ROB_YPickupCurrentPositionLimitMin_S2.ToString() + ","
                //    + ROB_YPickupCurrentPositionLimitMax_S2.ToString()
                //    + "]");
                MyTool.TxtFileProcess.CreateLog("S2机械手取料位置Y坐标值超出限制条件的范围." + "当前值为:" +
                    ROB_YPickupCurrentPosition_S2.ToString() + ",限制条件的范围是:" + "[" + ROB_YPickupCurrentPositionLimitMin_S2.ToString() + ","
                    + ROB_YPickupCurrentPositionLimitMax_S2.ToString()
                    + "]" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                return 2;
            }
            if (ROB_RZPickupCurrentPosition_S2 < ROB_RZPickupCurrentPositionLimitMin_S2 || ROB_RZPickupCurrentPosition_S2 > ROB_RZPickupCurrentPositionLimitMax_S2)
            {
                //MessageBox.Show("S2机械手取料位置RZ坐标值超出限制条件的范围." + "当前值为:" +
                //    ROB_RZPickupCurrentPosition_S2.ToString() + ",限制条件的范围是:" + "[" + ROB_RZPickupCurrentPositionLimitMin_S2.ToString() + ","
                //    + ROB_RZPickupCurrentPositionLimitMax_S2.ToString()
                //    + "]");
                MyTool.TxtFileProcess.CreateLog("S2机械手取料位置RZ坐标值超出限制条件的范围." + "当前值为:" +
                    ROB_RZPickupCurrentPosition_S2.ToString() + ",限制条件的范围是:" + "[" + ROB_RZPickupCurrentPositionLimitMin_S2.ToString() + ","
                    + ROB_RZPickupCurrentPositionLimitMax_S2.ToString()
                    + "]" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                return 3;
            }
            return 0;
        }

        private int ROB_PickupCurrentPositionCheck_S3()
        {
            if (!ActionForm._autoModeFlag)//手动时读取控件里的数据作为参数
            {
                try
                {
                    ROB_XPickupCurrentPositionLimitMin_S3 = Convert.ToSingle(txtROB_XPickupCurrentPositionLimitMin_S3.Text);
                    ROB_XPickupCurrentPositionLimitMax_S3 = Convert.ToSingle(txtROB_XPickupCurrentPositionLimitMax_S3.Text);
                    ROB_YPickupCurrentPositionLimitMin_S3 = Convert.ToSingle(txtROB_YPickupCurrentPositionLimitMin_S3.Text);
                    ROB_YPickupCurrentPositionLimitMax_S3 = Convert.ToSingle(txtROB_YPickupCurrentPositionLimitMax_S3.Text);
                    ROB_RZPickupCurrentPositionLimitMin_S3 = Convert.ToSingle(txtROB_RZPickupCurrentPositionLimitMin_S3.Text);
                    ROB_RZPickupCurrentPositionLimitMax_S3 = Convert.ToSingle(txtROB_RZPickupCurrentPositionLimitMax_S3.Text);
                }
                catch (Exception ex)
                {
                    //MessageBox.Show("S3控件里输入的机械手取料位置限制参数错误!");
                    MyTool.TxtFileProcess.CreateLog("S3控件里输入的机械手取料位置限制参数错误!" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    return -1;
                }
            }
            else//自动时读取产品参数文件数据
            {
                ROB_XPickupCurrentPositionLimitMin_S3 = _currentProductRecipe.ROB_XPickupCurrentPositionLimitMin_S3;
                ROB_XPickupCurrentPositionLimitMax_S3 = _currentProductRecipe.ROB_XPickupCurrentPositionLimitMax_S3;
                ROB_YPickupCurrentPositionLimitMin_S3 = _currentProductRecipe.ROB_YPickupCurrentPositionLimitMin_S3;
                ROB_YPickupCurrentPositionLimitMax_S3 = _currentProductRecipe.ROB_YPickupCurrentPositionLimitMax_S3;
                ROB_RZPickupCurrentPositionLimitMin_S3 = _currentProductRecipe.ROB_RZPickupCurrentPositionLimitMin_S3;
                ROB_RZPickupCurrentPositionLimitMax_S3 = _currentProductRecipe.ROB_RZPickupCurrentPositionLimitMax_S3;
            }

            if (ROB_XPickupCurrentPosition_S3 < ROB_XPickupCurrentPositionLimitMin_S3 || ROB_XPickupCurrentPosition_S3 > ROB_XPickupCurrentPositionLimitMax_S3)
            {
                //MessageBox.Show("S3机械手取料位置X坐标值超出限制条件的范围." + "当前值为:" +
                //    ROB_XPickupCurrentPosition_S3.ToString() + ",限制条件的范围是:" + "[" + ROB_XPickupCurrentPositionLimitMin_S3.ToString() + ","
                //    + ROB_XPickupCurrentPositionLimitMax_S3.ToString()
                //    + "]");
                MyTool.TxtFileProcess.CreateLog("S3机械手取料位置X坐标值超出限制条件的范围." + "当前值为:" +
                    ROB_XPickupCurrentPosition_S3.ToString() + ",限制条件的范围是:" + "[" + ROB_XPickupCurrentPositionLimitMin_S3.ToString() + ","
                    + ROB_XPickupCurrentPositionLimitMax_S3.ToString()
                    + "]" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                return 1;
            }
            if (ROB_YPickupCurrentPosition_S3 < ROB_YPickupCurrentPositionLimitMin_S3 || ROB_YPickupCurrentPosition_S3 > ROB_YPickupCurrentPositionLimitMax_S3)
            {
                //MessageBox.Show("S3机械手取料位置Y坐标值超出限制条件的范围." + "当前值为:" +
                //    ROB_YPickupCurrentPosition_S3.ToString() + ",限制条件的范围是:" + "[" + ROB_YPickupCurrentPositionLimitMin_S3.ToString() + ","
                //    + ROB_YPickupCurrentPositionLimitMax_S3.ToString()
                //    + "]");
                MyTool.TxtFileProcess.CreateLog("S3机械手取料位置Y坐标值超出限制条件的范围." + "当前值为:" +
                    ROB_YPickupCurrentPosition_S3.ToString() + ",限制条件的范围是:" + "[" + ROB_YPickupCurrentPositionLimitMin_S3.ToString() + ","
                    + ROB_YPickupCurrentPositionLimitMax_S3.ToString()
                    + "]" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                return 2;
            }
            if (ROB_RZPickupCurrentPosition_S3 < ROB_RZPickupCurrentPositionLimitMin_S3 || ROB_RZPickupCurrentPosition_S3 > ROB_RZPickupCurrentPositionLimitMax_S3)
            {
                //MessageBox.Show("S3机械手取料位置RZ坐标值超出限制条件的范围." + "当前值为:" +
                //    ROB_RZPickupCurrentPosition_S3.ToString() + ",限制条件的范围是:" + "[" + ROB_RZPickupCurrentPositionLimitMin_S3.ToString() + ","
                //    + ROB_RZPickupCurrentPositionLimitMax_S3.ToString()
                //    + "]");
                MyTool.TxtFileProcess.CreateLog("S3机械手取料位置RZ坐标值超出限制条件的范围." + "当前值为:" +
                    ROB_RZPickupCurrentPosition_S3.ToString() + ",限制条件的范围是:" + "[" + ROB_RZPickupCurrentPositionLimitMin_S3.ToString() + ","
                    + ROB_RZPickupCurrentPositionLimitMax_S3.ToString()
                    + "]" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                return 3;
            }
            return 0;
        }
        #endregion

        #region 判断是产品,牛皮纸,空
        private bool classification_S3()
        {
            float temp1;
            float temp2;
            float temp3;
            float temp4;
            float temp5;
            float temp6;
            float temp7;
            float temp8;
            float temp9;
            float temp10;
            if (!ActionForm._autoModeFlag)//手动时读取控件里的数据作为参数
            {
                try
                {
                    temp1 = Convert.ToSingle(txtClassificationThresholdMin_S3.Text);
                    temp2 = Convert.ToSingle(txtClassificationThresholdMax_S3.Text);
                    temp3 = Convert.ToSingle(txtProductAreaMin_S3.Text);
                    temp4 = Convert.ToSingle(txtProductAreaMax_S3.Text);
                    temp5 = Convert.ToSingle(txtCoverAreaMin_S3.Text);
                    temp6 = Convert.ToSingle(txtCoverAreaMax_S3.Text);
                    temp7 = Convert.ToSingle(txtEmptyAreaMin_S3.Text);
                    temp8 = Convert.ToSingle(txtEmptyAreaMax_S3.Text);
                    temp9 = Convert.ToSingle(txtClassificationThresholdCoverMin_S3.Text);
                    temp10 = Convert.ToSingle(txtClassificationThresholdCoverMax_S3.Text);
                }
                catch (Exception ex)
                {
                    //MessageBox.Show("S3分类参数格式错误!");
                    MyTool.TxtFileProcess.CreateLog("S3分类参数格式错误!" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    return false;
                }
            }
            else//自动时读取产品参数文件数据
            {
                try
                {
                    temp1 = _currentProductRecipe.ClassificationThresholdMin_S3;
                    temp2 = _currentProductRecipe.ClassificationThresholdMax_S3;
                    temp3 = _currentProductRecipe.ProductAreaMin_S3;
                    temp4 = _currentProductRecipe.ProductAreaMax_S3;
                    temp5 = _currentProductRecipe.CoverAreaMin_S3;
                    temp6 = _currentProductRecipe.CoverAreaMax_S3;
                    temp7 = _currentProductRecipe.EmptyAreaMin_S3;
                    temp8 = _currentProductRecipe.EmptyAreaMax_S3;
                    temp9 = _currentProductRecipe.ClassificationThresholdCoverMin_S3;
                    temp10 = _currentProductRecipe.ClassificationThresholdCoverMax_S3;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            _classificationResult_S3 = classification(hw_S3, temp1, temp2, temp3, temp4, temp5, temp6, temp7, temp8, temp9, temp10);

            return true;
        }
        #endregion

        #region 模板设定按钮
        private void btnModelCreate_S1_Click(object sender, EventArgs e)
        {
            if (ActionForm._accessLevel < 101)
            {
                MessageBox.Show("操作权限不合格:操作权限为工程师或者厂家!");
                return;
            }

            smf_S1.ShowDialog();

            //设置完模板后，同步更新找圆参数
            dsf_S1.Init($"{_GeneralParameterDirectoryPath}/{ModelPath_S1}", smf_S1.OrigImage, smf_S1.AffineModelContour);
        }
        private void btnModelCreate_S2_Click(object sender, EventArgs e)
        {
            if (ActionForm._accessLevel < 101)
            {
                MessageBox.Show("操作权限不合格:操作权限为工程师或者厂家!");
                return;
            }

            smf_S2.ShowDialog();

            //设置完模板后，同步更新找圆参数
            dsf_S2.Init($"{_GeneralParameterDirectoryPath}/{ModelPath_S2}", smf_S2.OrigImage, smf_S2.AffineModelContour);
        }
        private void btnModelCreate_S3_Click(object sender, EventArgs e)
        {
            if (ActionForm._accessLevel < 101)
            {
                MessageBox.Show("操作权限不合格:操作权限为工程师或者厂家!");
                return;
            }

            smf_S3.ShowDialog();

            //设置完模板后，同步更新找圆参数
            dsf_S3.Init($"{_GeneralParameterDirectoryPath}/{ModelPath_S3}", smf_S3.OrigImage, smf_S3.AffineModelContour);
        }
        #endregion

        #region 找圆设定按钮
        private void btnModelFindCircle_S1_Click(object sender, EventArgs e)
        {
            if (ActionForm._accessLevel < 101)
            {
                MessageBox.Show("操作权限不合格:操作权限为工程师或者厂家!");
                return;
            }
            dsf_S1.ShowDialog();
        }

        private void btnModelFindCircle_S2_Click(object sender, EventArgs e)
        {

            if (ActionForm._accessLevel < 101)
            {
                MessageBox.Show("操作权限不合格:操作权限为工程师或者厂家!");
                return;
            }
            dsf_S2.ShowDialog();
        }

        private void btnModelFindCircle_S3_Click(object sender, EventArgs e)
        {
            if (ActionForm._accessLevel < 101)
            {
                MessageBox.Show("操作权限不合格:操作权限为工程师或者厂家!");
                return;
            }
            dsf_S3.ShowDialog();
        }
        #endregion

        #region 打开图片按钮
        private void btnImageOpen_S1_Click(object sender, EventArgs e)
        {
            MyTool.TxtFileProcess.CreateLog("S1手动相机控制页面-打开图片" + "----用户：" + ActionForm._operatorName,
            ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);

            if (ActionForm._autoModeFlag)
            {
                MyTool.TxtFileProcess.CreateLog("S1手动相机控制页面-设备当前处于自动模式，无法读取测试图片" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("S1设备当前处于自动模式，无法读取测试图片！");
                return;
            }

            if (!RTDFlag_S1)
            {
                if (!CameraIsBusyFlag_S1)
                {
                    CameraIsBusyFlag_S1 = true;
                    OpenFileDialog dialog = new OpenFileDialog();
                    dialog.Title = "请选择测试图片";
                    dialog.Filter = "Format(*.jpg,*.gif,*.bmp,*.png,*tiff)|*.jpg;*.gif;*.bmp;*.png;*tiff";
                    if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        try
                        {
                            HOperatorSet.ReadImage(out ho_Image_S1, dialog.FileName);
                            hw_S1.HobjectToHimage(ho_Image_S1);
                            ActionForm.hw_S1.HobjectToHimage(ho_Image_S1);//在ActionForm显示打开的图片
                        }
                        catch
                        {
                            MyTool.TxtFileProcess.CreateLog("S1手动相机控制页面-读取测试图片失败" + "----用户：" + ActionForm._operatorName,
                            ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                            MessageBox.Show("S1读取测试图片失败!");
                        }
                    }
                    CameraIsBusyFlag_S1 = false;
                }
                else
                {
                    MyTool.TxtFileProcess.CreateLog("S1手动相机控制页面-由于图像处理正在进行一些步骤，无法读取图片" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("S1由于图像处理正在进行一些步骤，无法读取测量图片!");
                }
            }
            else
            {
                MyTool.TxtFileProcess.CreateLog("S1手动相机控制页面-由于相机当前正工作于实时显示状态，无法读取测试图片" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("S1由于相机当前正工作于实时显示状态，无法读取测试图片!");
            }
        }
        private void btnImageOpen_S2_Click(object sender, EventArgs e)
        {
            MyTool.TxtFileProcess.CreateLog("S2手动相机控制页面-打开图片" + "----用户：" + ActionForm._operatorName,
            ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);

            if (ActionForm._autoModeFlag)
            {
                MyTool.TxtFileProcess.CreateLog("S2手动相机控制页面-设备当前处于自动模式，无法读取测试图片" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("S2设备当前处于自动模式，无法读取测试图片！");
                return;
            }

            if (!RTDFlag_S2)
            {
                if (!CameraIsBusyFlag_S2)
                {
                    CameraIsBusyFlag_S2 = true;
                    OpenFileDialog dialog = new OpenFileDialog();
                    dialog.Title = "请选择测试图片";
                    dialog.Filter = "Format(*.jpg,*.gif,*.bmp,*.png,*tiff)|*.jpg;*.gif;*.bmp;*.png;*tiff";
                    if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        try
                        {
                            HOperatorSet.ReadImage(out ho_Image_S2, dialog.FileName);
                            hw_S2.HobjectToHimage(ho_Image_S2);
                            ActionForm.hw_S2.HobjectToHimage(ho_Image_S2);//在ActionForm显示打开的图片
                        }
                        catch
                        {
                            MyTool.TxtFileProcess.CreateLog("S2手动相机控制页面-读取测试图片失败" + "----用户：" + ActionForm._operatorName,
                            ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                            MessageBox.Show("S2读取测试图片失败!");
                        }
                    }
                    CameraIsBusyFlag_S2 = false;
                }
                else
                {
                    MyTool.TxtFileProcess.CreateLog("S2手动相机控制页面-由于图像处理正在进行一些步骤，无法读取图片" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("S2由于图像处理正在进行一些步骤，无法读取测量图片!");
                }
            }
            else
            {
                MyTool.TxtFileProcess.CreateLog("S2手动相机控制页面-由于相机当前正工作于实时显示状态，无法读取测试图片" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("S2由于相机当前正工作于实时显示状态，无法读取测试图片!");
            }
        }
        private void btnImageOpen_S3_Click(object sender, EventArgs e)
        {
            MyTool.TxtFileProcess.CreateLog("S3手动相机控制页面-打开图片" + "----用户：" + ActionForm._operatorName,
            ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);

            if (ActionForm._autoModeFlag)
            {
                MyTool.TxtFileProcess.CreateLog("S3手动相机控制页面-设备当前处于自动模式，无法读取测试图片" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("S3设备当前处于自动模式，无法读取测试图片！");
                return;
            }

            if (!RTDFlag_S3)
            {
                if (!CameraIsBusyFlag_S3)
                {
                    CameraIsBusyFlag_S3 = true;
                    OpenFileDialog dialog = new OpenFileDialog();
                    dialog.Title = "请选择测试图片";
                    dialog.Filter = "Format(*.jpg,*.gif,*.bmp,*.png,*tiff)|*.jpg;*.gif;*.bmp;*.png;*tiff";
                    if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        try
                        {
                            HOperatorSet.ReadImage(out ho_Image_S3, dialog.FileName);
                            hw_S3.HobjectToHimage(ho_Image_S3);
                            ActionForm.hw_S3.HobjectToHimage(ho_Image_S3);//在ActionForm显示打开的图片
                        }
                        catch
                        {
                            MyTool.TxtFileProcess.CreateLog("S3手动相机控制页面-读取测试图片失败" + "----用户：" + ActionForm._operatorName,
                            ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                            MessageBox.Show("S3读取测试图片失败!");
                        }
                    }
                    CameraIsBusyFlag_S3 = false;
                }
                else
                {
                    MyTool.TxtFileProcess.CreateLog("S3手动相机控制页面-由于图像处理正在进行一些步骤，无法读取图片" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("S3由于图像处理正在进行一些步骤，无法读取测量图片!");
                }
            }
            else
            {
                MyTool.TxtFileProcess.CreateLog("S3手动相机控制页面-由于相机当前正工作于实时显示状态，无法读取测试图片" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                MessageBox.Show("S3由于相机当前正工作于实时显示状态，无法读取测试图片!");
            }
        }
        #endregion

        #region 找矩形按钮
        private void btnFindRectangle_S1_Click(object sender, EventArgs e)
        {
            MyTool.TxtFileProcess.CreateLog("S1手动 找矩形按钮" + "----用户：" + ActionForm._operatorName,
            ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
            if (!ActionForm._CameraVideo_S1)
                currentProductPositionCalculate_S1();//计算当前物料的像素位置及角度数据
            else
            {
                MessageBox.Show("S1-请停止相机视频再执行!");
            }
        }
        private void btnFindRectangle_S2_Click(object sender, EventArgs e)
        {
            MyTool.TxtFileProcess.CreateLog("S2手动 找矩形按钮" + "----用户：" + ActionForm._operatorName,
            ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
            if (!ActionForm._CameraVideo_S2)
                currentProductPositionCalculate_S2();//计算当前物料的像素位置及角度数据
            else
            {
                MessageBox.Show("S2-请停止相机视频再执行!");
            }
        }
        private void btnFindRectangle_S3_Click(object sender, EventArgs e)
        {
            MyTool.TxtFileProcess.CreateLog("S3手动 找矩形按钮" + "----用户：" + ActionForm._operatorName,
            ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
            if (!ActionForm._CameraVideo_S3)
                currentProductPositionCalculate_S3();//计算当前物料的像素位置及角度数据
            else
            {
                MessageBox.Show("S3-请停止相机视频再执行!");
            }
        }
        #endregion

        #region 定位计算按钮
        private void btnPosition_S1_Click(object sender, EventArgs e)
        {
            MyTool.TxtFileProcess.CreateLog("S1手动 定位计算按钮" + "----用户：" + ActionForm._operatorName,
            ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
            if (!ActionForm._CameraVideo_S1)
            {
                txtROB_PickupCurrentPositionDisplay_1_S1.Text = "";
                txtROB_PickupCurrentPositionDisplay_2_S1.Text = "";
                if (mainProcess_S1() != 0)
                    MessageBox.Show("S1定位计算失败!");
            }
            else
            {
                MessageBox.Show("S1-请停止相机视频再执行!");
            }
        }
        private void btnPosition_S2_Click(object sender, EventArgs e)
        {
            MyTool.TxtFileProcess.CreateLog("S2手动 定位计算按钮" + "----用户：" + ActionForm._operatorName,
            ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
            if (!ActionForm._CameraVideo_S2)
            {
                txtROB_PickupCurrentPositionDisplay_1_S2.Text = "";
                txtROB_PickupCurrentPositionDisplay_2_S2.Text = "";
                if (mainProcess_S2() != 0)
                    MessageBox.Show("S2定位计算失败!");
            }
            else
            {
                MessageBox.Show("S2-请停止相机视频再执行!");
            }
        }
        private void btnPosition_S3_Click(object sender, EventArgs e)
        {
            MyTool.TxtFileProcess.CreateLog("S3手动 定位计算按钮" + "----用户：" + ActionForm._operatorName,
            ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
            if (!ActionForm._CameraVideo_S3)
            {
                txtROB_PickupCurrentPositionDisplay_1_S3.Text = "";
                txtROB_PickupCurrentPositionDisplay_2_S3.Text = "";

                int kk = mainProcess_S3();
                if (kk == -1)
                {
                    MessageBox.Show("S3分类出错!");
                    return;
                }

                if (kk != 0)
                    MessageBox.Show("S3定位计算失败!");
            }
            else
            {
                MessageBox.Show("S3-请停止相机视频再执行!");
            }
        }
        #endregion

        #region 发送定位数据到ROB按钮
        public bool manualSendPositionToRobRequest_S1 = false;
        public bool manualSendPositionToRobRequest_S2 = false;
        public bool manualSendPositionToRobRequest_S3 = false;
        private void btnSendPositionData_S1_Click(object sender, EventArgs e)
        {
            MyTool.TxtFileProcess.CreateLog("S1手动 发送按钮" + "----用户：" + ActionForm._operatorName,
            ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
            if (ActionForm._socketServerStartOKAndConnectROBOKFlag)
                manualSendPositionToRobRequest_S1 = true;
            else
            {
                MessageBox.Show("没有与ROB1连接!");
            }
        }
        private void btnSendPositionData_S2_Click(object sender, EventArgs e)
        {
            MyTool.TxtFileProcess.CreateLog("S2手动 发送按钮" + "----用户：" + ActionForm._operatorName,
            ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
            if (ActionForm._socketServerStartOKAndConnectROBOKFlag)
                manualSendPositionToRobRequest_S2 = true;
            else
            {
                MessageBox.Show("没有与ROB1连接!");
            }
        }
        private void btnSendPositionData_S3_Click(object sender, EventArgs e)
        {
            MyTool.TxtFileProcess.CreateLog("S3手动 发送按钮" + "----用户：" + ActionForm._operatorName,
            ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
            if (ActionForm._socketServerStartOKAndConnectROBOKFlag)
                manualSendPositionToRobRequest_S3 = true;
            else
            {
                MessageBox.Show("没有与ROB1连接!");
            }
        }
        #endregion

        #region mainProcess:主流程
        public int mainProcess_S1()
        {
            //读取当前产品的参数
            if (ActionForm._autoModeFlag)
            {
                if (!_currentProductRecipe.ProductDataLoadToVariate(_ProductParameterDirectoryPath + "\\" + _currentProductName + ".rcp"))
                    return 1;
            }

            //计算当前物料的像素位置及角度数据
            if (!currentProductPositionCalculate_S1())
                return 2;

            //当前物料的像素位置及角度数据检查
            if (currentProductPositionCheck_S1() != 0)
                return 7;

            //把当前物料位置数据像素坐标转化为ROB坐标
            if (!currentProductPositionCoordTransform_S1())
                return 3;

            //把当前产品的示教物料的像素坐标转换为ROB坐标
            if (!ROB_CurrentProduct_S1())
                return 4;

            //用4个顶点坐标计算示教物料位置变换到当前物料位置的矩阵hv_HomMat2D_CurrentTranslateMatrix
            if (!currentTranslateMatrix_S1())
                return 5;
            if (!currentTranslateMatrix_Pixel_S1())
                return 5;

            //计算ROB新的取料位置
            if (!ROB_PickupCurrentPosition_S1())
                return 6;

            //ROB新的取料位置检查
            if (ROB_PickupCurrentPositionCheck_S1() != 0)
                return 8;

            return 0;
        }
        public int mainProcess_S2()
        {
            //读取当前产品的参数
            if (ActionForm._autoModeFlag)
            {
                if (!_currentProductRecipe.ProductDataLoadToVariate(_ProductParameterDirectoryPath + "\\" + _currentProductName + ".rcp"))
                    return 1;
            }

            //计算当前物料的像素位置及角度数据
            if (!currentProductPositionCalculate_S2())
                return 2;

            //当前物料的像素位置及角度数据检查
            if (currentProductPositionCheck_S2() != 0)
                return 7;

            //把当前物料位置数据像素坐标转化为ROB坐标
            if (!currentProductPositionCoordTransform_S2())
                return 3;

            //把当前产品的示教物料的像素坐标转换为ROB坐标
            if (!ROB_CurrentProduct_S2())
                return 4;

            //用4个顶点坐标计算示教物料位置变换到当前物料位置的矩阵hv_HomMat2D_CurrentTranslateMatrix
            if (!currentTranslateMatrix_S2())
                return 5;
            if (!currentTranslateMatrix_Pixel_S2())
                return 5;

            //计算ROB新的取料位置
            if (!ROB_PickupCurrentPosition_S2())
                return 6;

            //ROB新的取料位置检查
            if (ROB_PickupCurrentPositionCheck_S2() != 0)
                return 8;

            return 0;
        }
        public int mainProcess_S3()
        {
            //读取当前产品的参数
            if (ActionForm._autoModeFlag)
            {
                if (!_currentProductRecipe.ProductDataLoadToVariate(_ProductParameterDirectoryPath + "\\" + _currentProductName + ".rcp"))
                    return 1;
            }

            //判断是产品,牛皮纸,空
            if (!classification_S3())
                return -1;

            if (_classificationResult_S3 == 1)
            {
                //计算当前物料的像素位置及角度数据
                if (!currentProductPositionCalculate_S3())
                    return 2;

                //当前物料的像素位置及角度数据检查
                if (currentProductPositionCheck_S3() != 0)
                    return 7;

                //把当前物料位置数据像素坐标转化为ROB坐标
                if (!currentProductPositionCoordTransform_S3())
                    return 3;

                //把当前产品的示教物料的像素坐标转换为ROB坐标
                if (!ROB_CurrentProduct_S3())
                    return 4;

                //用4个顶点坐标计算示教物料位置变换到当前物料位置的矩阵hv_HomMat2D_CurrentTranslateMatrix
                if (!currentTranslateMatrix_S3())
                    return 5;
                if (!currentTranslateMatrix_Pixel_S3())
                    return 5;

                //计算ROB新的取料位置
                if (!ROB_PickupCurrentPosition_S3())
                    return 6;

                //ROB新的取料位置检查
                if (ROB_PickupCurrentPositionCheck_S3() != 0)
                    return 8;
            }
            return 0;
        }
        #endregion

        #region CalibrateForm_FormClosing
        private void CalibrateForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Visible = false;
            e.Cancel = true;
        }
        #endregion

        #region textBox数据输入限制
        //只允许输入数字
        private void keyInNumber(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsNumber(e.KeyChar)) && e.KeyChar != (char)8)
            {
                e.Handled = true;
            }
        }

        //只允许输入数字与负号
        private void keyInNumberMinus(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsNumber(e.KeyChar)) && e.KeyChar != (char)8 && (e.KeyChar != '-'))
            {
                e.Handled = true;
            }
            //负号只能在第一位
            if (e.KeyChar == (char)('-') && ((TextBox)sender).SelectionStart != 0)
                e.Handled = true;
        }

        //只允许输入浮点数
        public void keyInFloat(object sender, KeyPressEventArgs e)
        {
            //只允许输入数字,小数点,负号,退格键
            if ((!(Char.IsNumber(e.KeyChar)) && (e.KeyChar != '.') && (e.KeyChar != '-') && (e.KeyChar != '\b')))
                e.Handled = true;
            //小数点不能在第一位
            if (e.KeyChar == (char)('.') && ((TextBox)sender).SelectionStart == 0)
                e.Handled = true;
            //小数点只能有一个
            if (e.KeyChar == (char)('.') && ((TextBox)sender).Text.IndexOf('.') != -1)
                e.Handled = true;
            //第一位为负号时,第二位不能输入小数点
            if (e.KeyChar == (char)('.') && ((TextBox)sender).SelectionStart == 1 && ((TextBox)sender).Text.IndexOf('-') == 0)
                e.Handled = true;
            //负号只能在第一位
            if (e.KeyChar == (char)('-') && ((TextBox)sender).SelectionStart != 0)
                e.Handled = true;
            //CTRLC,V
            if (e.KeyChar == 3 || e.KeyChar == 22)
                e.Handled = false;
        }

        //只允许输入+浮点数
        public void keyInFloatPlus(object sender, KeyPressEventArgs e)
        {
            //只允许输入数字,小数点,退格键
            if ((!(Char.IsNumber(e.KeyChar)) && (e.KeyChar != '.') && (e.KeyChar != '\b')))
                e.Handled = true;
            //小数点不能在第一位
            if (e.KeyChar == (char)('.') && ((TextBox)sender).SelectionStart == 0)
                e.Handled = true;
            //小数点只能有一个
            if (e.KeyChar == (char)('.') && ((TextBox)sender).Text.IndexOf('.') != -1)
                e.Handled = true;
        }

        #endregion

        #region 找矩形测试按钮
        #region btnThresholdTest
        HObject ho_ConnectedRegions_ThresholdTest_S1 = new HObject();
        HObject ho_ConnectedRegions_ThresholdTest_S2 = new HObject();
        HObject ho_ConnectedRegions_ThresholdTest_S3 = new HObject();
        private void btnThresholdTest_S1_Click(object sender, EventArgs e)
        {
            if (!ActionForm._CameraVideo_S1)
            {
                float temp1;
                float temp2;
                float temp3;
                try
                {
                    temp1 = Convert.ToSingle(txtThresholdMinGray_S1.Text);
                    temp2 = Convert.ToSingle(txtThresholdMaxGray_S1.Text);
                    temp3 = Convert.ToSingle(txtOpeningWAndH_S1.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("S1参数格式错误!");
                    return;
                }

                bool dataCheckResult = true;
                if (Convert.ToSingle(txtThresholdMinGray_S1.Text) < 0 || Convert.ToSingle(txtThresholdMinGray_S1.Text) > 255)
                {
                    MyTool.TxtFileProcess.CreateLog("S1控件txtThresholdMinGray的数据范围错误,取值范围是[0,255]！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("S1控件txtThresholdMinGray的数据范围错误,取值范围是[0,255]！");
                    dataCheckResult = false;
                }
                if (Convert.ToSingle(txtThresholdMaxGray_S1.Text) < 0 || Convert.ToSingle(txtThresholdMaxGray_S1.Text) > 255)
                {
                    MyTool.TxtFileProcess.CreateLog("S1控件txtThresholdMaxGray的数据范围错误,取值范围是[0,255]！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("S1控件txtThresholdMaxGray的数据范围错误,取值范围是[0,255]！");
                    dataCheckResult = false;
                }
                if (Convert.ToSingle(txtThresholdMaxGray_S1.Text) < Convert.ToSingle(txtThresholdMinGray_S1.Text))
                {
                    MyTool.TxtFileProcess.CreateLog("S1控件txtThresholdMaxGray的数据不能小于控件txtThresholdMinGray的数据！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("S1控件txtThresholdMaxGray的数据不能小于控件txtThresholdMinGray的数据！");
                    dataCheckResult = false;
                }
                if (Convert.ToSingle(txtOpeningWAndH_S1.Text) < 1 || Convert.ToSingle(txtOpeningWAndH_S1.Text) > 511)
                {
                    MyTool.TxtFileProcess.CreateLog("S1控件txtOpeningWAndH_S1的数据范围错误,取值范围是[1,511]！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("S1控件txtOpeningWAndH_S1的数据范围错误,取值范围是[1,511]！");
                    dataCheckResult = false;
                }
                if (!dataCheckResult)
                    return;

                ho_ConnectedRegions_ThresholdTest_S1 = newMinimumEnclosingRectangle.ThresholdTest(hw_S1, ho_Image_S1, temp1, temp2, temp3, true);
            }
            else
            {
                MessageBox.Show("S1请停止相机视频再执行!");
            }
        }
        private void btnThresholdTest_S2_Click(object sender, EventArgs e)
        {
            if (!ActionForm._CameraVideo_S2)
            {
                float temp1;
                float temp2;
                float temp3;
                try
                {
                    temp1 = Convert.ToSingle(txtThresholdMinGray_S2.Text);
                    temp2 = Convert.ToSingle(txtThresholdMaxGray_S2.Text);
                    temp3 = Convert.ToSingle(txtOpeningWAndH_S2.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("S2参数格式错误!");
                    return;
                }

                bool dataCheckResult = true;
                if (Convert.ToSingle(txtThresholdMinGray_S2.Text) < 0 || Convert.ToSingle(txtThresholdMinGray_S2.Text) > 255)
                {
                    MyTool.TxtFileProcess.CreateLog("S2控件txtThresholdMinGray的数据范围错误,取值范围是[0,255]！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("S2控件txtThresholdMinGray的数据范围错误,取值范围是[0,255]！");
                    dataCheckResult = false;
                }
                if (Convert.ToSingle(txtThresholdMaxGray_S2.Text) < 0 || Convert.ToSingle(txtThresholdMaxGray_S2.Text) > 255)
                {
                    MyTool.TxtFileProcess.CreateLog("S2控件txtThresholdMaxGray的数据范围错误,取值范围是[0,255]！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("S2控件txtThresholdMaxGray的数据范围错误,取值范围是[0,255]！");
                    dataCheckResult = false;
                }
                if (Convert.ToSingle(txtThresholdMaxGray_S2.Text) < Convert.ToSingle(txtThresholdMinGray_S2.Text))
                {
                    MyTool.TxtFileProcess.CreateLog("S2控件txtThresholdMaxGray的数据不能小于控件txtThresholdMinGray的数据！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("S2控件txtThresholdMaxGray的数据不能小于控件txtThresholdMinGray的数据！");
                    dataCheckResult = false;
                }
                if (Convert.ToSingle(txtOpeningWAndH_S2.Text) < 1 || Convert.ToSingle(txtOpeningWAndH_S2.Text) > 511)
                {
                    MyTool.TxtFileProcess.CreateLog("S2控件txtOpeningWAndH_S2的数据范围错误,取值范围是[1,511]！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("S2控件txtOpeningWAndH_S2的数据范围错误,取值范围是[1,511]！");
                    dataCheckResult = false;
                }
                if (!dataCheckResult)
                    return;

                ho_ConnectedRegions_ThresholdTest_S2 = newMinimumEnclosingRectangle.ThresholdTest(hw_S2, ho_Image_S2, temp1, temp2, temp3, true);
            }
            else
            {
                MessageBox.Show("S2请停止相机视频再执行!");
            }
        }
        private void btnThresholdTest_S3_Click(object sender, EventArgs e)
        {
            if (!ActionForm._CameraVideo_S3)
            {
                float temp1;
                float temp2;
                float temp3;
                try
                {
                    temp1 = Convert.ToSingle(txtThresholdMinGray_S3.Text);
                    temp2 = Convert.ToSingle(txtThresholdMaxGray_S3.Text);
                    temp3 = Convert.ToSingle(txtOpeningWAndH_S3.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("S3参数格式错误!");
                    return;
                }

                bool dataCheckResult = true;
                if (Convert.ToSingle(txtThresholdMinGray_S3.Text) < 0 || Convert.ToSingle(txtThresholdMinGray_S3.Text) > 255)
                {
                    MyTool.TxtFileProcess.CreateLog("S3控件txtThresholdMinGray的数据范围错误,取值范围是[0,255]！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("S3控件txtThresholdMinGray的数据范围错误,取值范围是[0,255]！");
                    dataCheckResult = false;
                }
                if (Convert.ToSingle(txtThresholdMaxGray_S3.Text) < 0 || Convert.ToSingle(txtThresholdMaxGray_S3.Text) > 255)
                {
                    MyTool.TxtFileProcess.CreateLog("S3控件txtThresholdMaxGray的数据范围错误,取值范围是[0,255]！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("S3控件txtThresholdMaxGray的数据范围错误,取值范围是[0,255]！");
                    dataCheckResult = false;
                }
                if (Convert.ToSingle(txtThresholdMaxGray_S3.Text) < Convert.ToSingle(txtThresholdMinGray_S3.Text))
                {
                    MyTool.TxtFileProcess.CreateLog("S3控件txtThresholdMaxGray的数据不能小于控件txtThresholdMinGray的数据！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("S3控件txtThresholdMaxGray的数据不能小于控件txtThresholdMinGray的数据！");
                    dataCheckResult = false;
                }
                if (Convert.ToSingle(txtOpeningWAndH_S3.Text) < 1 || Convert.ToSingle(txtOpeningWAndH_S3.Text) > 511)
                {
                    MyTool.TxtFileProcess.CreateLog("S3控件txtOpeningWAndH_S3的数据范围错误,取值范围是[1,511]！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("S3控件txtOpeningWAndH_S3的数据范围错误,取值范围是[1,511]！");
                    dataCheckResult = false;
                }
                if (!dataCheckResult)
                    return;

                ho_ConnectedRegions_ThresholdTest_S3 = newMinimumEnclosingRectangle.ThresholdTest(hw_S3, ho_Image_S3, temp1, temp2, temp3, false);
            }
            else
            {
                MessageBox.Show("S3请停止相机视频再执行!");
            }
        }
        #endregion

        #region btnSelectShapeTest
        HObject ho_RegionBorder_S1 = new HObject();
        HObject ho_RegionBorder_S2 = new HObject();
        HObject ho_RegionBorder_S3 = new HObject();
        private void btnSelectShapeTest_S1_Click(object sender, EventArgs e)
        {
            if (!ActionForm._CameraVideo_S1)
            {
                float temp1;
                float temp2;
                float temp3;
                float temp4;
                try
                {
                    temp1 = Convert.ToSingle(txtShapeMinArea_S1.Text);
                    temp2 = Convert.ToSingle(txtShapeMaxArea_S1.Text);
                    temp3 = Convert.ToSingle(txtShapeMinRectangularity_S1.Text);
                    temp4 = Convert.ToSingle(txtShapeMaxRectangularity_S1.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("S1参数格式错误!");
                    return;
                }

                bool dataCheckResult = true;
                if (Convert.ToSingle(txtShapeMinArea_S1.Text) < 0 || Convert.ToSingle(txtShapeMinArea_S1.Text) > 100000000)
                {
                    MyTool.TxtFileProcess.CreateLog("S1控件txtShapeMinArea的数据范围错误,取值范围是[0,100000000]！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("S1控件txtShapeMinArea的数据范围错误,取值范围是[0,100000000]！");
                    dataCheckResult = false;
                }
                if (Convert.ToSingle(txtShapeMaxArea_S1.Text) < 0 || Convert.ToSingle(txtShapeMaxArea_S1.Text) > 100000000)
                {
                    MyTool.TxtFileProcess.CreateLog("S1控件txtShapeMaxArea的数据范围错误,取值范围是[0,100000000]！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("S1控件txtShapeMaxArea的数据范围错误,取值范围是[0,100000000]！");
                    dataCheckResult = false;
                }
                if (Convert.ToSingle(txtShapeMaxArea_S1.Text) < Convert.ToSingle(txtShapeMinArea_S1.Text))
                {
                    MyTool.TxtFileProcess.CreateLog("S1控件txtShapeMaxArea的数据不能小于控件txtShapeMinArea的数据！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("S1控件txtShapeMaxArea的数据不能小于控件txtShapeMinArea的数据！");
                    dataCheckResult = false;
                }
                if (Convert.ToSingle(txtShapeMinRectangularity_S1.Text) < 0 || Convert.ToSingle(txtShapeMinRectangularity_S1.Text) > 1)
                {
                    MyTool.TxtFileProcess.CreateLog("S1控件txtShapeMinRectangularity的数据范围错误, 取值范围是[0, 1]！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("S1控件txtShapeMinRectangularity的数据范围错误, 取值范围是[0, 1]！");
                    dataCheckResult = false;
                }
                if (Convert.ToSingle(txtShapeMaxRectangularity_S1.Text) < 0 || Convert.ToSingle(txtShapeMaxRectangularity_S1.Text) > 1)
                {
                    MyTool.TxtFileProcess.CreateLog("S1控件txtShapeMaxRectangularity的数据范围错误, 取值范围是[0, 1]！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("S1控件txtShapeMaxRectangularity的数据范围错误, 取值范围是[0, 1]！");
                    dataCheckResult = false;
                }
                if (Convert.ToSingle(txtShapeMaxRectangularity_S1.Text) < Convert.ToSingle(txtShapeMinRectangularity_S1.Text))
                {
                    MyTool.TxtFileProcess.CreateLog("S1控件txtShapeMaxRectangularity的数据不能小于控件txtShapeMinArea.Text的数据！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("S1控件txtShapeMaxRectangularity的数据不能小于控件txtShapeMinArea.Text的数据！");
                    dataCheckResult = false;
                }
                if (!dataCheckResult)
                    return;

                ho_RegionBorder_S1 = newMinimumEnclosingRectangle.SelectShapeTest(hw_S1, ho_Image_S1, ho_ConnectedRegions_ThresholdTest_S1, temp1, temp2, temp3, temp4);
            }
            else
            {
                MessageBox.Show("S1请停止相机视频再执行!");
            }
        }
        private void btnSelectShapeTest_S2_Click(object sender, EventArgs e)
        {
            if (!ActionForm._CameraVideo_S2)
            {
                float temp1;
                float temp2;
                float temp3;
                float temp4;
                try
                {
                    temp1 = Convert.ToSingle(txtShapeMinArea_S2.Text);
                    temp2 = Convert.ToSingle(txtShapeMaxArea_S2.Text);
                    temp3 = Convert.ToSingle(txtShapeMinRectangularity_S2.Text);
                    temp4 = Convert.ToSingle(txtShapeMaxRectangularity_S2.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("S2参数格式错误!");
                    return;
                }

                bool dataCheckResult = true;
                if (Convert.ToSingle(txtShapeMinArea_S2.Text) < 0 || Convert.ToSingle(txtShapeMinArea_S2.Text) > 100000000)
                {
                    MyTool.TxtFileProcess.CreateLog("S2控件txtShapeMinArea的数据范围错误,取值范围是[0,100000000]！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("S2控件txtShapeMinArea的数据范围错误,取值范围是[0,100000000]！");
                    dataCheckResult = false;
                }
                if (Convert.ToSingle(txtShapeMaxArea_S2.Text) < 0 || Convert.ToSingle(txtShapeMaxArea_S2.Text) > 100000000)
                {
                    MyTool.TxtFileProcess.CreateLog("S2控件txtShapeMaxArea的数据范围错误,取值范围是[0,100000000]！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("S2控件txtShapeMaxArea的数据范围错误,取值范围是[0,100000000]！");
                    dataCheckResult = false;
                }
                if (Convert.ToSingle(txtShapeMaxArea_S2.Text) < Convert.ToSingle(txtShapeMinArea_S2.Text))
                {
                    MyTool.TxtFileProcess.CreateLog("S2控件txtShapeMaxArea的数据不能小于控件txtShapeMinArea的数据！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("S2控件txtShapeMaxArea的数据不能小于控件txtShapeMinArea的数据！");
                    dataCheckResult = false;
                }
                if (Convert.ToSingle(txtShapeMinRectangularity_S2.Text) < 0 || Convert.ToSingle(txtShapeMinRectangularity_S2.Text) > 1)
                {
                    MyTool.TxtFileProcess.CreateLog("S2控件txtShapeMinRectangularity的数据范围错误, 取值范围是[0, 1]！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("S2控件txtShapeMinRectangularity的数据范围错误, 取值范围是[0, 1]！");
                    dataCheckResult = false;
                }
                if (Convert.ToSingle(txtShapeMaxRectangularity_S2.Text) < 0 || Convert.ToSingle(txtShapeMaxRectangularity_S2.Text) > 1)
                {
                    MyTool.TxtFileProcess.CreateLog("S2控件txtShapeMaxRectangularity的数据范围错误, 取值范围是[0, 1]！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("S2控件txtShapeMaxRectangularity的数据范围错误, 取值范围是[0, 1]！");
                    dataCheckResult = false;
                }
                if (Convert.ToSingle(txtShapeMaxRectangularity_S2.Text) < Convert.ToSingle(txtShapeMinRectangularity_S2.Text))
                {
                    MyTool.TxtFileProcess.CreateLog("S2控件txtShapeMaxRectangularity的数据不能小于控件txtShapeMinArea.Text的数据！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("S2控件txtShapeMaxRectangularity的数据不能小于控件txtShapeMinArea.Text的数据！");
                    dataCheckResult = false;
                }
                if (!dataCheckResult)
                    return;

                ho_RegionBorder_S2 = newMinimumEnclosingRectangle.SelectShapeTest(hw_S2, ho_Image_S2, ho_ConnectedRegions_ThresholdTest_S2, temp1, temp2, temp3, temp4);
            }
            else
            {
                MessageBox.Show("S2请停止相机视频再执行!");
            }
        }
        private void btnSelectShapeTest_S3_Click(object sender, EventArgs e)
        {
            if (!ActionForm._CameraVideo_S3)
            {
                float temp1;
                float temp2;
                float temp3;
                float temp4;
                try
                {
                    temp1 = Convert.ToSingle(txtShapeMinArea_S3.Text);
                    temp2 = Convert.ToSingle(txtShapeMaxArea_S3.Text);
                    temp3 = Convert.ToSingle(txtShapeMinRectangularity_S3.Text);
                    temp4 = Convert.ToSingle(txtShapeMaxRectangularity_S3.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("S3参数格式错误!");
                    return;
                }

                bool dataCheckResult = true;
                if (Convert.ToSingle(txtShapeMinArea_S3.Text) < 0 || Convert.ToSingle(txtShapeMinArea_S3.Text) > 100000000)
                {
                    MyTool.TxtFileProcess.CreateLog("S3控件txtShapeMinArea的数据范围错误,取值范围是[0,100000000]！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("S3控件txtShapeMinArea的数据范围错误,取值范围是[0,100000000]！");
                    dataCheckResult = false;
                }
                if (Convert.ToSingle(txtShapeMaxArea_S3.Text) < 0 || Convert.ToSingle(txtShapeMaxArea_S3.Text) > 100000000)
                {
                    MyTool.TxtFileProcess.CreateLog("S3控件txtShapeMaxArea的数据范围错误,取值范围是[0,100000000]！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("S3控件txtShapeMaxArea的数据范围错误,取值范围是[0,100000000]！");
                    dataCheckResult = false;
                }
                if (Convert.ToSingle(txtShapeMaxArea_S3.Text) < Convert.ToSingle(txtShapeMinArea_S3.Text))
                {
                    MyTool.TxtFileProcess.CreateLog("S3控件txtShapeMaxArea的数据不能小于控件txtShapeMinArea的数据！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("S3控件txtShapeMaxArea的数据不能小于控件txtShapeMinArea的数据！");
                    dataCheckResult = false;
                }
                if (Convert.ToSingle(txtShapeMinRectangularity_S3.Text) < 0 || Convert.ToSingle(txtShapeMinRectangularity_S3.Text) > 1)
                {
                    MyTool.TxtFileProcess.CreateLog("S3控件txtShapeMinRectangularity的数据范围错误, 取值范围是[0, 1]！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("S3控件txtShapeMinRectangularity的数据范围错误, 取值范围是[0, 1]！");
                    dataCheckResult = false;
                }
                if (Convert.ToSingle(txtShapeMaxRectangularity_S3.Text) < 0 || Convert.ToSingle(txtShapeMaxRectangularity_S3.Text) > 1)
                {
                    MyTool.TxtFileProcess.CreateLog("S3控件txtShapeMaxRectangularity的数据范围错误, 取值范围是[0, 1]！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("S3控件txtShapeMaxRectangularity的数据范围错误, 取值范围是[0, 1]！");
                    dataCheckResult = false;
                }
                if (Convert.ToSingle(txtShapeMaxRectangularity_S3.Text) < Convert.ToSingle(txtShapeMinRectangularity_S3.Text))
                {
                    MyTool.TxtFileProcess.CreateLog("S3控件txtShapeMaxRectangularity的数据不能小于控件txtShapeMinArea.Text的数据！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("S3控件txtShapeMaxRectangularity的数据不能小于控件txtShapeMinArea.Text的数据！");
                    dataCheckResult = false;
                }
                if (!dataCheckResult)
                    return;

                ho_RegionBorder_S3 = newMinimumEnclosingRectangle.SelectShapeTest(hw_S3, ho_Image_S3, ho_ConnectedRegions_ThresholdTest_S3, temp1, temp2, temp3, temp4);
            }
            else
            {
                MessageBox.Show("S3请停止相机视频再执行!");
            }
        }
        #endregion

        #region btnDilationRectangle1Test
        HObject ho_ImageReduced1_S1 = new HObject();
        HObject ho_ImageReduced1_S2 = new HObject();
        HObject ho_ImageReduced1_S3 = new HObject();
        private void btnDilationRectangle1Test_S1_Click(object sender, EventArgs e)
        {
            if (!ActionForm._CameraVideo_S1)
            {
                float temp1;
                float temp2;
                try
                {
                    temp1 = Convert.ToSingle(txtDilationRectangleWidth_S1.Text);
                    temp2 = Convert.ToSingle(txtDilationRectangleHeight_S1.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("S1参数格式错误!");
                    return;
                }

                bool dataCheckResult = true;
                if (Convert.ToSingle(txtDilationRectangleWidth_S1.Text) < 1 || Convert.ToSingle(txtDilationRectangleWidth_S1.Text) > 511)
                {
                    MyTool.TxtFileProcess.CreateLog("S1控件txtDilationRectangleWidth的数据范围错误, 取值范围是[1, 511]！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("S1控件txtDilationRectangleWidth的数据范围错误, 取值范围是[1, 511]！");
                    dataCheckResult = false;
                }
                if (Convert.ToSingle(txtDilationRectangleHeight_S1.Text) < 1 || Convert.ToSingle(txtDilationRectangleHeight_S1.Text) > 511)
                {
                    MyTool.TxtFileProcess.CreateLog("S1控件txtDilationRectangleHeight的数据范围错误, 取值范围是[1, 511]！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("S1控件txtDilationRectangleHeight的数据范围错误, 取值范围是[1, 511]！");
                    dataCheckResult = false;
                }
                if (!dataCheckResult)
                    return;

                ho_ImageReduced1_S1 = newMinimumEnclosingRectangle.DilationRectangle1Test(hw_S1, ho_Image_S1, ho_RegionBorder_S1, temp1, temp2);
            }
            else
            {
                MessageBox.Show("S1请停止相机视频再执行!");
            }
        }
        private void btnDilationRectangle1Test_S2_Click(object sender, EventArgs e)
        {
            if (!ActionForm._CameraVideo_S2)
            {
                float temp1;
                float temp2;
                try
                {
                    temp1 = Convert.ToSingle(txtDilationRectangleWidth_S2.Text);
                    temp2 = Convert.ToSingle(txtDilationRectangleHeight_S2.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("S2参数格式错误!");
                    return;
                }

                bool dataCheckResult = true;
                if (Convert.ToSingle(txtDilationRectangleWidth_S2.Text) < 1 || Convert.ToSingle(txtDilationRectangleWidth_S2.Text) > 511)
                {
                    MyTool.TxtFileProcess.CreateLog("S2控件txtDilationRectangleWidth的数据范围错误, 取值范围是[1, 511]！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("S2控件txtDilationRectangleWidth的数据范围错误, 取值范围是[1, 511]！");
                    dataCheckResult = false;
                }
                if (Convert.ToSingle(txtDilationRectangleHeight_S2.Text) < 1 || Convert.ToSingle(txtDilationRectangleHeight_S2.Text) > 511)
                {
                    MyTool.TxtFileProcess.CreateLog("S2控件txtDilationRectangleHeight的数据范围错误, 取值范围是[1, 511]！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("S2控件txtDilationRectangleHeight的数据范围错误, 取值范围是[1, 511]！");
                    dataCheckResult = false;
                }
                if (!dataCheckResult)
                    return;

                ho_ImageReduced1_S2 = newMinimumEnclosingRectangle.DilationRectangle1Test(hw_S2, ho_Image_S2, ho_RegionBorder_S2, temp1, temp2);
            }
            else
            {
                MessageBox.Show("S2请停止相机视频再执行!");
            }
        }
        private void btnDilationRectangle1Test_S3_Click(object sender, EventArgs e)
        {
            if (!ActionForm._CameraVideo_S3)
            {
                float temp1;
                float temp2;
                try
                {
                    temp1 = Convert.ToSingle(txtDilationRectangleWidth_S3.Text);
                    temp2 = Convert.ToSingle(txtDilationRectangleHeight_S3.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("S3参数格式错误!");
                    return;
                }

                bool dataCheckResult = true;
                if (Convert.ToSingle(txtDilationRectangleWidth_S3.Text) < 1 || Convert.ToSingle(txtDilationRectangleWidth_S3.Text) > 511)
                {
                    MyTool.TxtFileProcess.CreateLog("S3控件txtDilationRectangleWidth的数据范围错误, 取值范围是[1, 511]！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("S3控件txtDilationRectangleWidth的数据范围错误, 取值范围是[1, 511]！");
                    dataCheckResult = false;
                }
                if (Convert.ToSingle(txtDilationRectangleHeight_S3.Text) < 1 || Convert.ToSingle(txtDilationRectangleHeight_S3.Text) > 511)
                {
                    MyTool.TxtFileProcess.CreateLog("S3控件txtDilationRectangleHeight的数据范围错误, 取值范围是[1, 511]！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("S3控件txtDilationRectangleHeight的数据范围错误, 取值范围是[1, 511]！");
                    dataCheckResult = false;
                }
                if (!dataCheckResult)
                    return;

                ho_ImageReduced1_S3 = newMinimumEnclosingRectangle.DilationRectangle1Test(hw_S3, ho_Image_S3, ho_RegionBorder_S3, temp1, temp2);
            }
            else
            {
                MessageBox.Show("S3请停止相机视频再执行!");
            }
        }
        #endregion

        #region btnEdgesSubPixTest
        HObject ho_Edges_S1 = new HObject();
        HObject ho_Edges_S2 = new HObject();
        HObject ho_Edges_S3 = new HObject();
        private void btnEdgesSubPixTest_S1_Click(object sender, EventArgs e)
        {
            if (!ActionForm._CameraVideo_S1)
            {
                float temp1;
                float temp2;
                float temp3;
                try
                {
                    temp1 = Convert.ToSingle(txtEdgesSubPixCannyAlpha_S1.Text);
                    temp2 = Convert.ToSingle(txtEdgesSubPixCannyLow_S1.Text);
                    temp3 = Convert.ToSingle(txtEdgesSubPixCannyHigh_S1.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("S1参数格式错误!");
                    return;
                }

                bool dataCheckResult = true;
                if (Convert.ToSingle(txtEdgesSubPixCannyAlpha_S1.Text) < 0.2 || Convert.ToSingle(txtEdgesSubPixCannyAlpha_S1.Text) > 50)
                {
                    MyTool.TxtFileProcess.CreateLog("S1控件txtEdgesSubPixCannyAlpha的数据范围错误, 取值范围是[0.2, 50]！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("S1控件txtEdgesSubPixCannyAlpha的数据范围错误, 取值范围是[0.2, 50]！");
                    dataCheckResult = false;
                }
                if (Convert.ToSingle(txtEdgesSubPixCannyLow_S1.Text) < 1 || Convert.ToSingle(txtEdgesSubPixCannyLow_S1.Text) > 255)
                {
                    MyTool.TxtFileProcess.CreateLog("S1控件txtEdgesSubPixCannyLow的数据范围错误, 取值范围是[1, 255]！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("S1控件txtEdgesSubPixCannyLow的数据范围错误, 取值范围是[1, 255]！");
                    dataCheckResult = false;
                }
                if (Convert.ToSingle(txtEdgesSubPixCannyHigh_S1.Text) < 1 || Convert.ToSingle(txtEdgesSubPixCannyHigh_S1.Text) > 255)
                {
                    MyTool.TxtFileProcess.CreateLog("S1控件txtEdgesSubPixCannyHigh的数据范围错误, 取值范围是[1, 255]！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("S1控件txtEdgesSubPixCannyHigh的数据范围错误, 取值范围是[1, 255]！");
                    dataCheckResult = false;
                }
                if (Convert.ToSingle(txtEdgesSubPixCannyHigh_S1.Text) < Convert.ToSingle(txtEdgesSubPixCannyLow_S1.Text))
                {
                    MyTool.TxtFileProcess.CreateLog("S1控件txtEdgesSubPixCannyHigh的数据不能小于控件txtEdgesSubPixCannyLow的数据！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("S1控件txtEdgesSubPixCannyHigh的数据不能小于控件txtEdgesSubPixCannyLow的数据！");
                    dataCheckResult = false;
                }
                if (!dataCheckResult)
                    return;

                ho_Edges_S1 = newMinimumEnclosingRectangle.EdgesSubPixTest(hw_S1, ho_Image_S1, ho_ImageReduced1_S1, temp1, temp2, temp3);
            }
            else
            {
                MessageBox.Show("S1请停止相机视频再执行!");
            }
        }
        private void btnEdgesSubPixTest_S2_Click(object sender, EventArgs e)
        {
            if (!ActionForm._CameraVideo_S2)
            {
                float temp1;
                float temp2;
                float temp3;
                try
                {
                    temp1 = Convert.ToSingle(txtEdgesSubPixCannyAlpha_S2.Text);
                    temp2 = Convert.ToSingle(txtEdgesSubPixCannyLow_S2.Text);
                    temp3 = Convert.ToSingle(txtEdgesSubPixCannyHigh_S2.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("S2参数格式错误!");
                    return;
                }

                bool dataCheckResult = true;
                if (Convert.ToSingle(txtEdgesSubPixCannyAlpha_S2.Text) < 0.2 || Convert.ToSingle(txtEdgesSubPixCannyAlpha_S2.Text) > 50)
                {
                    MyTool.TxtFileProcess.CreateLog("S2控件txtEdgesSubPixCannyAlpha的数据范围错误, 取值范围是[0.2, 50]！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("S2控件txtEdgesSubPixCannyAlpha的数据范围错误, 取值范围是[0.2, 50]！");
                    dataCheckResult = false;
                }
                if (Convert.ToSingle(txtEdgesSubPixCannyLow_S2.Text) < 1 || Convert.ToSingle(txtEdgesSubPixCannyLow_S2.Text) > 255)
                {
                    MyTool.TxtFileProcess.CreateLog("S2控件txtEdgesSubPixCannyLow的数据范围错误, 取值范围是[1, 255]！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("S2控件txtEdgesSubPixCannyLow的数据范围错误, 取值范围是[1, 255]！");
                    dataCheckResult = false;
                }
                if (Convert.ToSingle(txtEdgesSubPixCannyHigh_S2.Text) < 1 || Convert.ToSingle(txtEdgesSubPixCannyHigh_S2.Text) > 255)
                {
                    MyTool.TxtFileProcess.CreateLog("S2控件txtEdgesSubPixCannyHigh的数据范围错误, 取值范围是[1, 255]！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("S2控件txtEdgesSubPixCannyHigh的数据范围错误, 取值范围是[1, 255]！");
                    dataCheckResult = false;
                }
                if (Convert.ToSingle(txtEdgesSubPixCannyHigh_S2.Text) < Convert.ToSingle(txtEdgesSubPixCannyLow_S2.Text))
                {
                    MyTool.TxtFileProcess.CreateLog("S2控件txtEdgesSubPixCannyHigh的数据不能小于控件txtEdgesSubPixCannyLow的数据！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("S2控件txtEdgesSubPixCannyHigh的数据不能小于控件txtEdgesSubPixCannyLow的数据！");
                    dataCheckResult = false;
                }
                if (!dataCheckResult)
                    return;

                ho_Edges_S2 = newMinimumEnclosingRectangle.EdgesSubPixTest(hw_S2, ho_Image_S2, ho_ImageReduced1_S2, temp1, temp2, temp3);
            }
            else
            {
                MessageBox.Show("S2请停止相机视频再执行!");
            }
        }
        private void btnEdgesSubPixTest_S3_Click(object sender, EventArgs e)
        {
            if (!ActionForm._CameraVideo_S3)
            {
                float temp1;
                float temp2;
                float temp3;
                try
                {
                    temp1 = Convert.ToSingle(txtEdgesSubPixCannyAlpha_S3.Text);
                    temp2 = Convert.ToSingle(txtEdgesSubPixCannyLow_S3.Text);
                    temp3 = Convert.ToSingle(txtEdgesSubPixCannyHigh_S3.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("S3参数格式错误!");
                    return;
                }

                bool dataCheckResult = true;
                if (Convert.ToSingle(txtEdgesSubPixCannyAlpha_S3.Text) < 0.2 || Convert.ToSingle(txtEdgesSubPixCannyAlpha_S3.Text) > 50)
                {
                    MyTool.TxtFileProcess.CreateLog("S3控件txtEdgesSubPixCannyAlpha的数据范围错误, 取值范围是[0.2, 50]！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("S3控件txtEdgesSubPixCannyAlpha的数据范围错误, 取值范围是[0.2, 50]！");
                    dataCheckResult = false;
                }
                if (Convert.ToSingle(txtEdgesSubPixCannyLow_S3.Text) < 1 || Convert.ToSingle(txtEdgesSubPixCannyLow_S3.Text) > 255)
                {
                    MyTool.TxtFileProcess.CreateLog("S3控件txtEdgesSubPixCannyLow的数据范围错误, 取值范围是[1, 255]！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("S3控件txtEdgesSubPixCannyLow的数据范围错误, 取值范围是[1, 255]！");
                    dataCheckResult = false;
                }
                if (Convert.ToSingle(txtEdgesSubPixCannyHigh_S3.Text) < 1 || Convert.ToSingle(txtEdgesSubPixCannyHigh_S3.Text) > 255)
                {
                    MyTool.TxtFileProcess.CreateLog("S3控件txtEdgesSubPixCannyHigh的数据范围错误, 取值范围是[1, 255]！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("S3控件txtEdgesSubPixCannyHigh的数据范围错误, 取值范围是[1, 255]！");
                    dataCheckResult = false;
                }
                if (Convert.ToSingle(txtEdgesSubPixCannyHigh_S3.Text) < Convert.ToSingle(txtEdgesSubPixCannyLow_S3.Text))
                {
                    MyTool.TxtFileProcess.CreateLog("S3控件txtEdgesSubPixCannyHigh的数据不能小于控件txtEdgesSubPixCannyLow的数据！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("S3控件txtEdgesSubPixCannyHigh的数据不能小于控件txtEdgesSubPixCannyLow的数据！");
                    dataCheckResult = false;
                }
                if (!dataCheckResult)
                    return;

                ho_Edges_S3 = newMinimumEnclosingRectangle.EdgesSubPixTest(hw_S3, ho_Image_S3, ho_ImageReduced1_S3, temp1, temp2, temp3);
            }
            else
            {
                MessageBox.Show("S3请停止相机视频再执行!");
            }
        }
        #endregion

        #region btntxtUnionCollinearContoursXldTest
        HObject ho_Edges2_S1 = new HObject();
        HObject ho_Edges2_S2 = new HObject();
        HObject ho_Edges2_S3 = new HObject();
        private void btntxtUnionCollinearContoursXldTest_S1_Click(object sender, EventArgs e)
        {
            if (!ActionForm._CameraVideo_S1)
            {
                float temp1;
                float temp2;
                float temp3;
                float temp4;
                try
                {
                    temp1 = Convert.ToSingle(txtUnionCollinearContoursXldMaxDistAbs_S1.Text);
                    temp2 = Convert.ToSingle(txtUnionCollinearContoursXldMaxDistRel_S1.Text);
                    temp3 = Convert.ToSingle(txtUnionCollinearContoursXldMaxShift_S1.Text);
                    temp4 = Convert.ToSingle(txtUnionCollinearContoursXldMaxAngle_S1.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("S1参数格式错误!");
                    return;
                }

                bool dataCheckResult = true;
                if (Convert.ToSingle(txtUnionCollinearContoursXldMaxDistAbs_S1.Text) < 0 || Convert.ToSingle(txtUnionCollinearContoursXldMaxDistAbs_S1.Text) > 100000000)
                {
                    MyTool.TxtFileProcess.CreateLog("S1控件txtUnionCollinearContoursXldMaxDistAbs的数据范围错误, 取值范围是[0, 100000000]！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("S1控件txtUnionCollinearContoursXldMaxDistAbs的数据范围错误, 取值范围是[0, 100000000]！");
                    dataCheckResult = false;
                }
                if (Convert.ToSingle(txtUnionCollinearContoursXldMaxDistRel_S1.Text) < 0 || Convert.ToSingle(txtUnionCollinearContoursXldMaxDistRel_S1.Text) > 100000000)
                {
                    MyTool.TxtFileProcess.CreateLog("S1控件txtUnionCollinearContoursXldMaxDistRel的数据范围错误, 取值范围是[0, 100000000]！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("S1控件txtUnionCollinearContoursXldMaxDistRel的数据范围错误, 取值范围是[0, 100000000]！");
                    dataCheckResult = false;
                }
                if (Convert.ToSingle(txtUnionCollinearContoursXldMaxShift_S1.Text) < 0 || Convert.ToSingle(txtUnionCollinearContoursXldMaxShift_S1.Text) > 100000000)
                {
                    MyTool.TxtFileProcess.CreateLog("S1控件txtUnionCollinearContoursXldMaxShift的数据范围错误, 取值范围是[0, 100000000]！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("S1控件txtUnionCollinearContoursXldMaxShift的数据范围错误, 取值范围是[0, 100000000]！");
                    dataCheckResult = false;
                }
                if (Convert.ToSingle(txtUnionCollinearContoursXldMaxAngle_S1.Text) < 0 || Convert.ToSingle(txtUnionCollinearContoursXldMaxAngle_S1.Text) > 0.78539816339)
                {
                    MyTool.TxtFileProcess.CreateLog("S1控件txtUnionCollinearContoursXldMaxAngle的数据范围错误, 取值范围是[0, 0.78539816339]！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("S1控件txtUnionCollinearContoursXldMaxAngle的数据范围错误, 取值范围是[0, 0.78539816339]！");
                    dataCheckResult = false;
                }
                if (!dataCheckResult)
                    return;

                ho_Edges2_S1 = newMinimumEnclosingRectangle.UnionCollinearContoursXld(hw_S1, ho_Image_S1, ho_Edges_S1, temp1, temp2, temp3, temp4);
            }
            else
            {
                MessageBox.Show("S1请停止相机视频再执行!");
            }

        }
        private void btntxtUnionCollinearContoursXldTest_S2_Click(object sender, EventArgs e)
        {
            if (!ActionForm._CameraVideo_S2)
            {
                float temp1;
                float temp2;
                float temp3;
                float temp4;
                try
                {
                    temp1 = Convert.ToSingle(txtUnionCollinearContoursXldMaxDistAbs_S2.Text);
                    temp2 = Convert.ToSingle(txtUnionCollinearContoursXldMaxDistRel_S2.Text);
                    temp3 = Convert.ToSingle(txtUnionCollinearContoursXldMaxShift_S2.Text);
                    temp4 = Convert.ToSingle(txtUnionCollinearContoursXldMaxAngle_S2.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("S2参数格式错误!");
                    return;
                }

                bool dataCheckResult = true;
                if (Convert.ToSingle(txtUnionCollinearContoursXldMaxDistAbs_S2.Text) < 0 || Convert.ToSingle(txtUnionCollinearContoursXldMaxDistAbs_S2.Text) > 100000000)
                {
                    MyTool.TxtFileProcess.CreateLog("S2控件txtUnionCollinearContoursXldMaxDistAbs的数据范围错误, 取值范围是[0, 100000000]！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("S2控件txtUnionCollinearContoursXldMaxDistAbs的数据范围错误, 取值范围是[0, 100000000]！");
                    dataCheckResult = false;
                }
                if (Convert.ToSingle(txtUnionCollinearContoursXldMaxDistRel_S2.Text) < 0 || Convert.ToSingle(txtUnionCollinearContoursXldMaxDistRel_S2.Text) > 100000000)
                {
                    MyTool.TxtFileProcess.CreateLog("S2控件txtUnionCollinearContoursXldMaxDistRel的数据范围错误, 取值范围是[0, 100000000]！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("S2控件txtUnionCollinearContoursXldMaxDistRel的数据范围错误, 取值范围是[0, 100000000]！");
                    dataCheckResult = false;
                }
                if (Convert.ToSingle(txtUnionCollinearContoursXldMaxShift_S2.Text) < 0 || Convert.ToSingle(txtUnionCollinearContoursXldMaxShift_S2.Text) > 100000000)
                {
                    MyTool.TxtFileProcess.CreateLog("S2控件txtUnionCollinearContoursXldMaxShift的数据范围错误, 取值范围是[0, 100000000]！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("S2控件txtUnionCollinearContoursXldMaxShift的数据范围错误, 取值范围是[0, 100000000]！");
                    dataCheckResult = false;
                }
                if (Convert.ToSingle(txtUnionCollinearContoursXldMaxAngle_S2.Text) < 0 || Convert.ToSingle(txtUnionCollinearContoursXldMaxAngle_S2.Text) > 0.78539816339)
                {
                    MyTool.TxtFileProcess.CreateLog("S2控件txtUnionCollinearContoursXldMaxAngle的数据范围错误, 取值范围是[0, 0.78539816339]！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("S2控件txtUnionCollinearContoursXldMaxAngle的数据范围错误, 取值范围是[0, 0.78539816339]！");
                    dataCheckResult = false;
                }
                if (!dataCheckResult)
                    return;

                ho_Edges2_S2 = newMinimumEnclosingRectangle.UnionCollinearContoursXld(hw_S2, ho_Image_S2, ho_Edges_S2, temp1, temp2, temp3, temp4);
            }
            else
            {
                MessageBox.Show("S2请停止相机视频再执行!");
            }

        }
        private void btntxtUnionCollinearContoursXldTest_S3_Click(object sender, EventArgs e)
        {
            if (!ActionForm._CameraVideo_S3)
            {
                float temp1;
                float temp2;
                float temp3;
                float temp4;
                try
                {
                    temp1 = Convert.ToSingle(txtUnionCollinearContoursXldMaxDistAbs_S3.Text);
                    temp2 = Convert.ToSingle(txtUnionCollinearContoursXldMaxDistRel_S3.Text);
                    temp3 = Convert.ToSingle(txtUnionCollinearContoursXldMaxShift_S3.Text);
                    temp4 = Convert.ToSingle(txtUnionCollinearContoursXldMaxAngle_S3.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("S3参数格式错误!");
                    return;
                }

                bool dataCheckResult = true;
                if (Convert.ToSingle(txtUnionCollinearContoursXldMaxDistAbs_S3.Text) < 0 || Convert.ToSingle(txtUnionCollinearContoursXldMaxDistAbs_S3.Text) > 100000000)
                {
                    MyTool.TxtFileProcess.CreateLog("S3控件txtUnionCollinearContoursXldMaxDistAbs的数据范围错误, 取值范围是[0, 100000000]！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("S3控件txtUnionCollinearContoursXldMaxDistAbs的数据范围错误, 取值范围是[0, 100000000]！");
                    dataCheckResult = false;
                }
                if (Convert.ToSingle(txtUnionCollinearContoursXldMaxDistRel_S3.Text) < 0 || Convert.ToSingle(txtUnionCollinearContoursXldMaxDistRel_S3.Text) > 100000000)
                {
                    MyTool.TxtFileProcess.CreateLog("S3控件txtUnionCollinearContoursXldMaxDistRel的数据范围错误, 取值范围是[0, 100000000]！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("S3控件txtUnionCollinearContoursXldMaxDistRel的数据范围错误, 取值范围是[0, 100000000]！");
                    dataCheckResult = false;
                }
                if (Convert.ToSingle(txtUnionCollinearContoursXldMaxShift_S3.Text) < 0 || Convert.ToSingle(txtUnionCollinearContoursXldMaxShift_S3.Text) > 100000000)
                {
                    MyTool.TxtFileProcess.CreateLog("S3控件txtUnionCollinearContoursXldMaxShift的数据范围错误, 取值范围是[0, 100000000]！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("S3控件txtUnionCollinearContoursXldMaxShift的数据范围错误, 取值范围是[0, 100000000]！");
                    dataCheckResult = false;
                }
                if (Convert.ToSingle(txtUnionCollinearContoursXldMaxAngle_S3.Text) < 0 || Convert.ToSingle(txtUnionCollinearContoursXldMaxAngle_S3.Text) > 0.78539816339)
                {
                    MyTool.TxtFileProcess.CreateLog("S3控件txtUnionCollinearContoursXldMaxAngle的数据范围错误, 取值范围是[0, 0.78539816339]！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("S3控件txtUnionCollinearContoursXldMaxAngle的数据范围错误, 取值范围是[0, 0.78539816339]！");
                    dataCheckResult = false;
                }
                if (!dataCheckResult)
                    return;

                ho_Edges2_S3 = newMinimumEnclosingRectangle.UnionCollinearContoursXld(hw_S3, ho_Image_S3, ho_Edges_S3, temp1, temp2, temp3, temp4);
            }
            else
            {
                MessageBox.Show("S3请停止相机视频再执行!");
            }

        }
        #endregion

        #region btnSelectShapeXld_ContlengthTest
        HObject ho_RectangleEdges_S1 = new HObject();
        HObject ho_RectangleEdges_S2 = new HObject();
        HObject ho_RectangleEdges_S3 = new HObject();
        private void btnSelectShapeXld_ContlengthTest_S1_Click(object sender, EventArgs e)
        {
            if (!ActionForm._CameraVideo_S1)
            {
                float temp1;
                float temp2;
                try
                {
                    temp1 = Convert.ToSingle(txtXldContlengthMin_S1.Text);
                    temp2 = Convert.ToSingle(txtXldContlengthMax_S1.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("S1参数格式错误!");
                    return;
                }

                bool dataCheckResult = true;
                if (Convert.ToSingle(txtXldContlengthMin_S1.Text) < 0 || Convert.ToSingle(txtXldContlengthMin_S1.Text) > 100000000)
                {
                    MyTool.TxtFileProcess.CreateLog("S1控件txtXldContlengthMin的数据范围错误, 取值范围是[0, 100000000]！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("S1控件txtXldContlengthMin的数据范围错误, 取值范围是[0, 100000000]！");
                    dataCheckResult = false;
                }
                if (Convert.ToSingle(txtXldContlengthMax_S1.Text) < 0 || Convert.ToSingle(txtXldContlengthMax_S1.Text) > 100000000)
                {
                    MyTool.TxtFileProcess.CreateLog("S1控件txtXldContlengthMax的数据范围错误, 取值范围是[0, 100000000]！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("S1控件txtXldContlengthMax的数据范围错误, 取值范围是[0, 100000000]！");
                    dataCheckResult = false;
                }
                if (Convert.ToSingle(txtXldContlengthMax_S1.Text) < Convert.ToSingle(txtXldContlengthMin_S1.Text))
                {
                    MyTool.TxtFileProcess.CreateLog("S1控件txtXldContlengthMax的数据不能小于控件txtXldContlengthMin的数据！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("S1控件txtXldContlengthMax的数据不能小于控件txtXldContlengthMin的数据！");
                    dataCheckResult = false;
                }
                if (!dataCheckResult)
                    return;

                ho_RectangleEdges_S1 = newMinimumEnclosingRectangle.SelectShapeXld_ContlengthTest(hw_S1, ho_Image_S1, ho_Edges2_S1, temp1, temp2);
            }
            else
            {
                MessageBox.Show("S1请停止相机视频再执行!");
            }
        }
        private void btnSelectShapeXld_ContlengthTest_S2_Click(object sender, EventArgs e)
        {
            if (!ActionForm._CameraVideo_S2)
            {
                float temp1;
                float temp2;
                try
                {
                    temp1 = Convert.ToSingle(txtXldContlengthMin_S2.Text);
                    temp2 = Convert.ToSingle(txtXldContlengthMax_S2.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("S2参数格式错误!");
                    return;
                }

                bool dataCheckResult = true;
                if (Convert.ToSingle(txtXldContlengthMin_S2.Text) < 0 || Convert.ToSingle(txtXldContlengthMin_S2.Text) > 100000000)
                {
                    MyTool.TxtFileProcess.CreateLog("S2控件txtXldContlengthMin的数据范围错误, 取值范围是[0, 100000000]！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("S2控件txtXldContlengthMin的数据范围错误, 取值范围是[0, 100000000]！");
                    dataCheckResult = false;
                }
                if (Convert.ToSingle(txtXldContlengthMax_S2.Text) < 0 || Convert.ToSingle(txtXldContlengthMax_S2.Text) > 100000000)
                {
                    MyTool.TxtFileProcess.CreateLog("S2控件txtXldContlengthMax的数据范围错误, 取值范围是[0, 100000000]！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("S2控件txtXldContlengthMax的数据范围错误, 取值范围是[0, 100000000]！");
                    dataCheckResult = false;
                }
                if (Convert.ToSingle(txtXldContlengthMax_S2.Text) < Convert.ToSingle(txtXldContlengthMin_S2.Text))
                {
                    MyTool.TxtFileProcess.CreateLog("S2控件txtXldContlengthMax的数据不能小于控件txtXldContlengthMin的数据！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("S2控件txtXldContlengthMax的数据不能小于控件txtXldContlengthMin的数据！");
                    dataCheckResult = false;
                }
                if (!dataCheckResult)
                    return;

                ho_RectangleEdges_S2 = newMinimumEnclosingRectangle.SelectShapeXld_ContlengthTest(hw_S2, ho_Image_S2, ho_Edges2_S2, temp1, temp2);
            }
            else
            {
                MessageBox.Show("S2请停止相机视频再执行!");
            }
        }
        private void btnSelectShapeXld_ContlengthTest_S3_Click(object sender, EventArgs e)
        {
            if (!ActionForm._CameraVideo_S3)
            {
                float temp1;
                float temp2;
                try
                {
                    temp1 = Convert.ToSingle(txtXldContlengthMin_S3.Text);
                    temp2 = Convert.ToSingle(txtXldContlengthMax_S3.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("S3参数格式错误!");
                    return;
                }

                bool dataCheckResult = true;
                if (Convert.ToSingle(txtXldContlengthMin_S3.Text) < 0 || Convert.ToSingle(txtXldContlengthMin_S3.Text) > 100000000)
                {
                    MyTool.TxtFileProcess.CreateLog("S3控件txtXldContlengthMin的数据范围错误, 取值范围是[0, 100000000]！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("S3控件txtXldContlengthMin的数据范围错误, 取值范围是[0, 100000000]！");
                    dataCheckResult = false;
                }
                if (Convert.ToSingle(txtXldContlengthMax_S3.Text) < 0 || Convert.ToSingle(txtXldContlengthMax_S3.Text) > 100000000)
                {
                    MyTool.TxtFileProcess.CreateLog("S3控件txtXldContlengthMax的数据范围错误, 取值范围是[0, 100000000]！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("S3控件txtXldContlengthMax的数据范围错误, 取值范围是[0, 100000000]！");
                    dataCheckResult = false;
                }
                if (Convert.ToSingle(txtXldContlengthMax_S3.Text) < Convert.ToSingle(txtXldContlengthMin_S3.Text))
                {
                    MyTool.TxtFileProcess.CreateLog("S3控件txtXldContlengthMax的数据不能小于控件txtXldContlengthMin的数据！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("S3控件txtXldContlengthMax的数据不能小于控件txtXldContlengthMin的数据！");
                    dataCheckResult = false;
                }
                if (!dataCheckResult)
                    return;

                ho_RectangleEdges_S3 = newMinimumEnclosingRectangle.SelectShapeXld_ContlengthTest(hw_S3, ho_Image_S3, ho_Edges2_S3, temp1, temp2);
            }
            else
            {
                MessageBox.Show("S3请停止相机视频再执行!");
            }
        }
        #endregion

        #region btnSelectShapeXld_AreaAndRectangularityTest
        HObject ho_RectangleEdges2_S1 = new HObject();
        HObject ho_RectangleEdges2_S2 = new HObject();
        HObject ho_RectangleEdges2_S3 = new HObject();
        private void btnSelectShapeXld_AreaAndRectangularityTest_S1_Click(object sender, EventArgs e)
        {
            if (!ActionForm._CameraVideo_S1)
            {
                float temp1;
                float temp2;
                float temp3;
                float temp4;
                try
                {
                    temp1 = Convert.ToSingle(txtXLDMinArea_S1.Text);
                    temp2 = Convert.ToSingle(txtXLDMaxArea_S1.Text);
                    temp3 = Convert.ToSingle(txtXLDMinRectangularity_S1.Text);
                    temp4 = Convert.ToSingle(txtXLDMaxRectangularity_S1.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("S1参数格式错误!");
                    return;
                }

                bool dataCheckResult = true;
                if (Convert.ToSingle(txtXLDMinArea_S1.Text) < 0 || Convert.ToSingle(txtXLDMinArea_S1.Text) > 100000000)
                {
                    MyTool.TxtFileProcess.CreateLog("S1控件txtXLDMinArea的数据范围错误, 取值范围是[0, 100000000]！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("S1控件txtXLDMinArea的数据范围错误, 取值范围是[0, 100000000]！");
                    dataCheckResult = false;
                }
                if (Convert.ToSingle(txtXLDMaxArea_S1.Text) < 0 || Convert.ToSingle(txtXLDMaxArea_S1.Text) > 100000000)
                {
                    MyTool.TxtFileProcess.CreateLog("S1控件txtXLDMaxArea的数据范围错误, 取值范围是[0, 100000000]！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("S1控件txtXLDMaxArea的数据范围错误, 取值范围是[0, 100000000]！");
                    dataCheckResult = false;
                }
                if (Convert.ToSingle(txtXLDMaxArea_S1.Text) < Convert.ToSingle(txtXLDMinArea_S1.Text))
                {
                    MyTool.TxtFileProcess.CreateLog("S1控件txtXLDMaxArea的数据不能小于控件txtXLDMinArea的数据！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("S1控件txtXLDMaxArea的数据不能小于控件txtXLDMinArea的数据！");
                    dataCheckResult = false;
                }
                if (Convert.ToSingle(txtXLDMinRectangularity_S1.Text) < 0 || Convert.ToSingle(txtXLDMinRectangularity_S1.Text) > 1)
                {
                    MyTool.TxtFileProcess.CreateLog("S1控件txtXLDMinRectangularity的数据范围错误, 取值范围是[0, 1]！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("S1控件txtXLDMinRectangularity的数据范围错误, 取值范围是[0, 1]！");
                    dataCheckResult = false;
                }
                if (Convert.ToSingle(txtXLDMaxRectangularity_S1.Text) < 0 || Convert.ToSingle(txtXLDMaxRectangularity_S1.Text) > 1)
                {
                    MyTool.TxtFileProcess.CreateLog("S1控件txtXLDMaxRectangularity的数据范围错误, 取值范围是[0, 1]！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("S1控件txtXLDMaxRectangularity的数据范围错误, 取值范围是[0, 1]！");
                    dataCheckResult = false;
                }
                if (Convert.ToSingle(txtXLDMaxRectangularity_S1.Text) < Convert.ToSingle(txtXLDMinRectangularity_S1.Text))
                {
                    MyTool.TxtFileProcess.CreateLog("S1控件txtXLDMaxRectangularity的数据不能小于控件txtXLDMinRectangularity的数据！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("S1控件txtXLDMaxRectangularity的数据不能小于控件txtXLDMinRectangularity的数据！");
                    dataCheckResult = false;
                }
                if (!dataCheckResult)
                    return;

                ho_RectangleEdges2_S1 = newMinimumEnclosingRectangle.SelectShapeXld_AreaAndRectangularityTest(hw_S1, ho_Image_S1, ho_RectangleEdges_S1, temp1, temp2, temp3, temp4);
            }
            else
            {
                MessageBox.Show("S1请停止相机视频再执行!");
            }
        }
        private void btnSelectShapeXld_AreaAndRectangularityTest_S2_Click(object sender, EventArgs e)
        {
            if (!ActionForm._CameraVideo_S2)
            {
                float temp1;
                float temp2;
                float temp3;
                float temp4;
                try
                {
                    temp1 = Convert.ToSingle(txtXLDMinArea_S2.Text);
                    temp2 = Convert.ToSingle(txtXLDMaxArea_S2.Text);
                    temp3 = Convert.ToSingle(txtXLDMinRectangularity_S2.Text);
                    temp4 = Convert.ToSingle(txtXLDMaxRectangularity_S2.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("S2参数格式错误!");
                    return;
                }

                bool dataCheckResult = true;
                if (Convert.ToSingle(txtXLDMinArea_S2.Text) < 0 || Convert.ToSingle(txtXLDMinArea_S2.Text) > 100000000)
                {
                    MyTool.TxtFileProcess.CreateLog("S2控件txtXLDMinArea的数据范围错误, 取值范围是[0, 100000000]！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("S2控件txtXLDMinArea的数据范围错误, 取值范围是[0, 100000000]！");
                    dataCheckResult = false;
                }
                if (Convert.ToSingle(txtXLDMaxArea_S2.Text) < 0 || Convert.ToSingle(txtXLDMaxArea_S2.Text) > 100000000)
                {
                    MyTool.TxtFileProcess.CreateLog("S2控件txtXLDMaxArea的数据范围错误, 取值范围是[0, 100000000]！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("S2控件txtXLDMaxArea的数据范围错误, 取值范围是[0, 100000000]！");
                    dataCheckResult = false;
                }
                if (Convert.ToSingle(txtXLDMaxArea_S2.Text) < Convert.ToSingle(txtXLDMinArea_S2.Text))
                {
                    MyTool.TxtFileProcess.CreateLog("S2控件txtXLDMaxArea的数据不能小于控件txtXLDMinArea的数据！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("S2控件txtXLDMaxArea的数据不能小于控件txtXLDMinArea的数据！");
                    dataCheckResult = false;
                }
                if (Convert.ToSingle(txtXLDMinRectangularity_S2.Text) < 0 || Convert.ToSingle(txtXLDMinRectangularity_S2.Text) > 1)
                {
                    MyTool.TxtFileProcess.CreateLog("S2控件txtXLDMinRectangularity的数据范围错误, 取值范围是[0, 1]！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("S2控件txtXLDMinRectangularity的数据范围错误, 取值范围是[0, 1]！");
                    dataCheckResult = false;
                }
                if (Convert.ToSingle(txtXLDMaxRectangularity_S2.Text) < 0 || Convert.ToSingle(txtXLDMaxRectangularity_S2.Text) > 1)
                {
                    MyTool.TxtFileProcess.CreateLog("S2控件txtXLDMaxRectangularity的数据范围错误, 取值范围是[0, 1]！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("S2控件txtXLDMaxRectangularity的数据范围错误, 取值范围是[0, 1]！");
                    dataCheckResult = false;
                }
                if (Convert.ToSingle(txtXLDMaxRectangularity_S2.Text) < Convert.ToSingle(txtXLDMinRectangularity_S2.Text))
                {
                    MyTool.TxtFileProcess.CreateLog("S2控件txtXLDMaxRectangularity的数据不能小于控件txtXLDMinRectangularity的数据！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("S2控件txtXLDMaxRectangularity的数据不能小于控件txtXLDMinRectangularity的数据！");
                    dataCheckResult = false;
                }
                if (!dataCheckResult)
                    return;

                ho_RectangleEdges2_S2 = newMinimumEnclosingRectangle.SelectShapeXld_AreaAndRectangularityTest(hw_S2, ho_Image_S2, ho_RectangleEdges_S2, temp1, temp2, temp3, temp4);
            }
            else
            {
                MessageBox.Show("S2请停止相机视频再执行!");
            }
        }
        private void btnSelectShapeXld_AreaAndRectangularityTest_S3_Click(object sender, EventArgs e)
        {
            if (!ActionForm._CameraVideo_S3)
            {
                float temp1;
                float temp2;
                float temp3;
                float temp4;
                try
                {
                    temp1 = Convert.ToSingle(txtXLDMinArea_S3.Text);
                    temp2 = Convert.ToSingle(txtXLDMaxArea_S3.Text);
                    temp3 = Convert.ToSingle(txtXLDMinRectangularity_S3.Text);
                    temp4 = Convert.ToSingle(txtXLDMaxRectangularity_S3.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("S3参数格式错误!");
                    return;
                }

                bool dataCheckResult = true;
                if (Convert.ToSingle(txtXLDMinArea_S3.Text) < 0 || Convert.ToSingle(txtXLDMinArea_S3.Text) > 100000000)
                {
                    MyTool.TxtFileProcess.CreateLog("S3控件txtXLDMinArea的数据范围错误, 取值范围是[0, 100000000]！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("S3控件txtXLDMinArea的数据范围错误, 取值范围是[0, 100000000]！");
                    dataCheckResult = false;
                }
                if (Convert.ToSingle(txtXLDMaxArea_S3.Text) < 0 || Convert.ToSingle(txtXLDMaxArea_S3.Text) > 100000000)
                {
                    MyTool.TxtFileProcess.CreateLog("S3控件txtXLDMaxArea的数据范围错误, 取值范围是[0, 100000000]！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("S3控件txtXLDMaxArea的数据范围错误, 取值范围是[0, 100000000]！");
                    dataCheckResult = false;
                }
                if (Convert.ToSingle(txtXLDMaxArea_S3.Text) < Convert.ToSingle(txtXLDMinArea_S3.Text))
                {
                    MyTool.TxtFileProcess.CreateLog("S3控件txtXLDMaxArea的数据不能小于控件txtXLDMinArea的数据！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("S3控件txtXLDMaxArea的数据不能小于控件txtXLDMinArea的数据！");
                    dataCheckResult = false;
                }
                if (Convert.ToSingle(txtXLDMinRectangularity_S3.Text) < 0 || Convert.ToSingle(txtXLDMinRectangularity_S3.Text) > 1)
                {
                    MyTool.TxtFileProcess.CreateLog("S3控件txtXLDMinRectangularity的数据范围错误, 取值范围是[0, 1]！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("S3控件txtXLDMinRectangularity的数据范围错误, 取值范围是[0, 1]！");
                    dataCheckResult = false;
                }
                if (Convert.ToSingle(txtXLDMaxRectangularity_S3.Text) < 0 || Convert.ToSingle(txtXLDMaxRectangularity_S3.Text) > 1)
                {
                    MyTool.TxtFileProcess.CreateLog("S3控件txtXLDMaxRectangularity的数据范围错误, 取值范围是[0, 1]！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("S3控件txtXLDMaxRectangularity的数据范围错误, 取值范围是[0, 1]！");
                    dataCheckResult = false;
                }
                if (Convert.ToSingle(txtXLDMaxRectangularity_S3.Text) < Convert.ToSingle(txtXLDMinRectangularity_S3.Text))
                {
                    MyTool.TxtFileProcess.CreateLog("S3控件txtXLDMaxRectangularity的数据不能小于控件txtXLDMinRectangularity的数据！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("S3控件txtXLDMaxRectangularity的数据不能小于控件txtXLDMinRectangularity的数据！");
                    dataCheckResult = false;
                }
                if (!dataCheckResult)
                    return;

                ho_RectangleEdges2_S3 = newMinimumEnclosingRectangle.SelectShapeXld_AreaAndRectangularityTest(hw_S3, ho_Image_S3, ho_RectangleEdges_S3, temp1, temp2, temp3, temp4);
            }
            else
            {
                MessageBox.Show("S3请停止相机视频再执行!");
            }
        }
        #endregion

        #region btnUnionAdjacentContoursXldTest
        HObject ho_RectangleEdges3_S1 = new HObject();
        HObject ho_RectangleEdges3_S2 = new HObject();
        HObject ho_RectangleEdges3_S3 = new HObject();
        private void btnUnionAdjacentContoursXldTest_S1_Click(object sender, EventArgs e)
        {
            if (!ActionForm._CameraVideo_S1)
            {
                float temp1;
                float temp2;
                try
                {
                    temp1 = Convert.ToSingle(txtUnionAdjacentContoursXldMaxDistAbs_S1.Text);
                    temp2 = Convert.ToSingle(txtUnionAdjacentContoursXldMaxDistRel_S1.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("S1参数格式错误!");
                    return;
                }

                bool dataCheckResult = true;
                if (Convert.ToSingle(txtUnionAdjacentContoursXldMaxDistAbs_S1.Text) < 0 || Convert.ToSingle(txtUnionAdjacentContoursXldMaxDistAbs_S1.Text) > 100000000)
                {
                    MyTool.TxtFileProcess.CreateLog("S1控件txtUnionAdjacentContoursXldMaxDistAbs的数据范围错误, 取值范围是[0, 100000000]！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("S1控件txtUnionAdjacentContoursXldMaxDistAbs的数据范围错误, 取值范围是[0, 100000000]！");
                    dataCheckResult = false;
                }
                if (Convert.ToSingle(txtUnionAdjacentContoursXldMaxDistRel_S1.Text) < 0 || Convert.ToSingle(txtUnionAdjacentContoursXldMaxDistRel_S1.Text) > 100000000)
                {
                    MyTool.TxtFileProcess.CreateLog("S1控件txtUnionAdjacentContoursXldMaxDistRel的数据范围错误, 取值范围是[0, 100000000]！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("S1控件txtUnionAdjacentContoursXldMaxDistRel的数据范围错误, 取值范围是[0, 100000000]！");
                    dataCheckResult = false;
                }
                if (!dataCheckResult)
                    return;

                ho_RectangleEdges3_S1 = newMinimumEnclosingRectangle.UnionAdjacentContoursXldTest(hw_S1, ho_Image_S1, ho_RectangleEdges2_S1, temp1, temp2);
            }
            else
            {
                MessageBox.Show("S1请停止相机视频再执行!");
            }
        }
        private void btnUnionAdjacentContoursXldTest_S2_Click(object sender, EventArgs e)
        {
            if (!ActionForm._CameraVideo_S2)
            {
                float temp1;
                float temp2;
                try
                {
                    temp1 = Convert.ToSingle(txtUnionAdjacentContoursXldMaxDistAbs_S2.Text);
                    temp2 = Convert.ToSingle(txtUnionAdjacentContoursXldMaxDistRel_S2.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("S2参数格式错误!");
                    return;
                }

                bool dataCheckResult = true;
                if (Convert.ToSingle(txtUnionAdjacentContoursXldMaxDistAbs_S2.Text) < 0 || Convert.ToSingle(txtUnionAdjacentContoursXldMaxDistAbs_S2.Text) > 100000000)
                {
                    MyTool.TxtFileProcess.CreateLog("S2控件txtUnionAdjacentContoursXldMaxDistAbs的数据范围错误, 取值范围是[0, 100000000]！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("S2控件txtUnionAdjacentContoursXldMaxDistAbs的数据范围错误, 取值范围是[0, 100000000]！");
                    dataCheckResult = false;
                }
                if (Convert.ToSingle(txtUnionAdjacentContoursXldMaxDistRel_S2.Text) < 0 || Convert.ToSingle(txtUnionAdjacentContoursXldMaxDistRel_S2.Text) > 100000000)
                {
                    MyTool.TxtFileProcess.CreateLog("S2控件txtUnionAdjacentContoursXldMaxDistRel的数据范围错误, 取值范围是[0, 100000000]！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("S2控件txtUnionAdjacentContoursXldMaxDistRel的数据范围错误, 取值范围是[0, 100000000]！");
                    dataCheckResult = false;
                }
                if (!dataCheckResult)
                    return;

                ho_RectangleEdges3_S2 = newMinimumEnclosingRectangle.UnionAdjacentContoursXldTest(hw_S2, ho_Image_S2, ho_RectangleEdges2_S2, temp1, temp2);
            }
            else
            {
                MessageBox.Show("S2请停止相机视频再执行!");
            }
        }
        private void btnUnionAdjacentContoursXldTest_S3_Click(object sender, EventArgs e)
        {
            if (!ActionForm._CameraVideo_S3)
            {
                float temp1;
                float temp2;
                try
                {
                    temp1 = Convert.ToSingle(txtUnionAdjacentContoursXldMaxDistAbs_S3.Text);
                    temp2 = Convert.ToSingle(txtUnionAdjacentContoursXldMaxDistRel_S3.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("S3参数格式错误!");
                    return;
                }

                bool dataCheckResult = true;
                if (Convert.ToSingle(txtUnionAdjacentContoursXldMaxDistAbs_S3.Text) < 0 || Convert.ToSingle(txtUnionAdjacentContoursXldMaxDistAbs_S3.Text) > 100000000)
                {
                    MyTool.TxtFileProcess.CreateLog("S3控件txtUnionAdjacentContoursXldMaxDistAbs的数据范围错误, 取值范围是[0, 100000000]！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("S3控件txtUnionAdjacentContoursXldMaxDistAbs的数据范围错误, 取值范围是[0, 100000000]！");
                    dataCheckResult = false;
                }
                if (Convert.ToSingle(txtUnionAdjacentContoursXldMaxDistRel_S3.Text) < 0 || Convert.ToSingle(txtUnionAdjacentContoursXldMaxDistRel_S3.Text) > 100000000)
                {
                    MyTool.TxtFileProcess.CreateLog("S3控件txtUnionAdjacentContoursXldMaxDistRel的数据范围错误, 取值范围是[0, 100000000]！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("S3控件txtUnionAdjacentContoursXldMaxDistRel的数据范围错误, 取值范围是[0, 100000000]！");
                    dataCheckResult = false;
                }
                if (!dataCheckResult)
                    return;

                ho_RectangleEdges3_S3 = newMinimumEnclosingRectangle.UnionAdjacentContoursXldTest(hw_S3, ho_Image_S3, ho_RectangleEdges2_S3, temp1, temp2);
            }
            else
            {
                MessageBox.Show("S3请停止相机视频再执行!");
            }
        }
        #endregion

        #region btnFitRectangle2ContourXldTest
        HObject ho_Rectangle_S1 = new HObject();
        HObject ho_Rectangle_S2 = new HObject();
        HObject ho_Rectangle_S3 = new HObject();
        private void btnFitRectangle2ContourXldTest_S1_Click(object sender, EventArgs e)
        {
            if (!ActionForm._CameraVideo_S1)
            {
                ho_Rectangle_S1 = newMinimumEnclosingRectangle.FitRectangle2ContourXldTest(hw_S1, ho_Image_S1, ho_RectangleEdges3_S1);
            }
            else
            {
                MessageBox.Show("S1请停止相机视频再执行!");
            }
        }
        private void btnFitRectangle2ContourXldTest_S2_Click(object sender, EventArgs e)
        {
            if (!ActionForm._CameraVideo_S2)
            {
                ho_Rectangle_S2 = newMinimumEnclosingRectangle.FitRectangle2ContourXldTest(hw_S2, ho_Image_S2, ho_RectangleEdges3_S2);
            }
            else
            {
                MessageBox.Show("S2请停止相机视频再执行!");
            }
        }
        private void btnFitRectangle2ContourXldTest_S3_Click(object sender, EventArgs e)
        {
            if (!ActionForm._CameraVideo_S3)
            {
                ho_Rectangle_S3 = newMinimumEnclosingRectangle.FitRectangle2ContourXldTest(hw_S3, ho_Image_S3, ho_RectangleEdges3_S3);
            }
            else
            {
                MessageBox.Show("S3请停止相机视频再执行!");
            }
        }
        #endregion

        #endregion

        #region btnDebug
        private void btnDebug_S1_Click(object sender, EventArgs e)
        {
            mainProcess_S1();
        }
        private void btnDebug_S2_Click(object sender, EventArgs e)
        {
            mainProcess_S2();
        }
        private void btnDebug_S3_Click(object sender, EventArgs e)
        {
            mainProcess_S3();
        }
        #endregion

        #region classification:区分是产品,牛皮纸,空
        private void btnClassification_S3_Click(object sender, EventArgs e)
        {
            if (!ActionForm._CameraVideo_S3)
            {
                if (ho_Image_S3 == null)
                {
                    MessageBox.Show("请先打开照片或者拍照");
                    return;
                }

                float temp1;
                float temp2;
                float temp3;
                float temp4;
                float temp5;
                float temp6;
                float temp7;
                float temp8;
                float temp9;
                float temp10;
                try
                {
                    temp1 = Convert.ToSingle(txtClassificationThresholdMin_S3.Text);
                    temp2 = Convert.ToSingle(txtClassificationThresholdMax_S3.Text);
                    temp3 = Convert.ToSingle(txtProductAreaMin_S3.Text);
                    temp4 = Convert.ToSingle(txtProductAreaMax_S3.Text);
                    temp5 = Convert.ToSingle(txtCoverAreaMin_S3.Text);
                    temp6 = Convert.ToSingle(txtCoverAreaMax_S3.Text);
                    temp7 = Convert.ToSingle(txtEmptyAreaMin_S3.Text);
                    temp8 = Convert.ToSingle(txtEmptyAreaMax_S3.Text);
                    temp9 = Convert.ToSingle(txtClassificationThresholdCoverMin_S3.Text);
                    temp10 = Convert.ToSingle(txtClassificationThresholdCoverMax_S3.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("S3分类参数格式错误!");
                    return;
                }

                bool dataCheckResult = true;
                if (Convert.ToSingle(txtClassificationThresholdMin_S3.Text) < 0 || Convert.ToSingle(txtClassificationThresholdMin_S3.Text) > 255)
                {
                    MyTool.TxtFileProcess.CreateLog("控件txtClassificationThresholdMin_S3的数据范围错误,取值范围是[0,255]！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("控件txtClassificationThresholdMin_S3的数据范围错误,取值范围是[0,255]！");
                    dataCheckResult = false;
                }
                if (Convert.ToSingle(txtClassificationThresholdMax_S3.Text) < 0 || Convert.ToSingle(txtClassificationThresholdMax_S3.Text) > 255)
                {
                    MyTool.TxtFileProcess.CreateLog("控件txtClassificationThresholdMax_S3的数据范围错误,取值范围是[0,255]！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("控件txtClassificationThresholdMax_S3的数据范围错误,取值范围是[0,255]！");
                    dataCheckResult = false;
                }
                if (Convert.ToSingle(txtClassificationThresholdMax_S3.Text) < Convert.ToSingle(txtClassificationThresholdMin_S3.Text))
                {
                    MyTool.TxtFileProcess.CreateLog("控件txtClassificationThresholdMax_S3的数据不能小于控件txtClassificationThresholdMin_S3的数据！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("控件txtClassificationThresholdMax_S3的数据不能小于控件txtClassificationThresholdMin_S3的数据！");
                    dataCheckResult = false;
                }

                if (Convert.ToSingle(txtProductAreaMin_S3.Text) < 0 || Convert.ToSingle(txtProductAreaMin_S3.Text) > 100000000)
                {
                    MyTool.TxtFileProcess.CreateLog("控件txtProductAreaMin_S3的数据范围错误,取值范围是[0,100000000]！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("控件txtProductAreaMin_S3的数据范围错误,取值范围是[0,100000000]！");
                    dataCheckResult = false;
                }
                if (Convert.ToSingle(txtProductAreaMax_S3.Text) < 0 || Convert.ToSingle(txtProductAreaMax_S3.Text) > 100000000)
                {
                    MyTool.TxtFileProcess.CreateLog("控件txtProductAreaMax_S3的数据范围错误,取值范围是[0,100000000]！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("控件txtProductAreaMax_S3的数据范围错误,取值范围是[0,100000000]！");
                    dataCheckResult = false;
                }
                if (Convert.ToSingle(txtProductAreaMax_S3.Text) < Convert.ToSingle(txtProductAreaMin_S3.Text))
                {
                    MyTool.TxtFileProcess.CreateLog("控件txtProductAreaMax_S3的数据不能小于控件txtProductAreaMin_S3的数据！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("控件txtProductAreaMax_S3的数据不能小于控件txtProductAreaMin_S3的数据！");
                    dataCheckResult = false;
                }

                if (Convert.ToSingle(txtCoverAreaMin_S3.Text) < 0 || Convert.ToSingle(txtCoverAreaMin_S3.Text) > 100000000)
                {
                    MyTool.TxtFileProcess.CreateLog("控件txtProductAreaMin_S3的数据范围错误,取值范围是[0,100000000]！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("控件txtProductAreaMin_S3的数据范围错误,取值范围是[0,100000000]！");
                    dataCheckResult = false;
                }
                if (Convert.ToSingle(txtCoverAreaMax_S3.Text) < 0 || Convert.ToSingle(txtCoverAreaMax_S3.Text) > 100000000)
                {
                    MyTool.TxtFileProcess.CreateLog("控件txtProductAreaMax_S3的数据范围错误,取值范围是[0,100000000]！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("控件txtProductAreaMax_S3的数据范围错误,取值范围是[0,100000000]！");
                    dataCheckResult = false;
                }
                if (Convert.ToSingle(txtCoverAreaMax_S3.Text) < Convert.ToSingle(txtCoverAreaMin_S3.Text))
                {
                    MyTool.TxtFileProcess.CreateLog("控件txtCoverAreaMax_S3的数据不能小于控件txtCoverAreaMin_S3的数据！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("控件txtCoverAreaMax_S3的数据不能小于控件txtCoverAreaMin_S3的数据！");
                    dataCheckResult = false;
                }

                if (Convert.ToSingle(txtEmptyAreaMin_S3.Text) < 0 || Convert.ToSingle(txtEmptyAreaMin_S3.Text) > 100000000)
                {
                    MyTool.TxtFileProcess.CreateLog("控件txtEmptyAreaMin_S3的数据范围错误,取值范围是[0,100000000]！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("控件txtEmptyAreaMin_S3的数据范围错误,取值范围是[0,100000000]！");
                    dataCheckResult = false;
                }
                if (Convert.ToSingle(txtEmptyAreaMax_S3.Text) < 0 || Convert.ToSingle(txtEmptyAreaMax_S3.Text) > 100000000)
                {
                    MyTool.TxtFileProcess.CreateLog("控件txtProductAreaMax_S3的数据范围错误,取值范围是[0,100000000]！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("控件txtProductAreaMax_S3的数据范围错误,取值范围是[0,100000000]！");
                    dataCheckResult = false;
                }
                if (Convert.ToSingle(txtEmptyAreaMax_S3.Text) < Convert.ToSingle(txtEmptyAreaMin_S3.Text))
                {
                    MyTool.TxtFileProcess.CreateLog("控件txtEmptyAreaMax_S3的数据不能小于控件txtEmptyAreaMin_S3的数据！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("控件txtEmptyAreaMax_S3的数据不能小于控件txtEmptyAreaMin_S3的数据！");
                    dataCheckResult = false;
                }

                if (Convert.ToSingle(txtClassificationThresholdCoverMin_S3.Text) < 0 || Convert.ToSingle(txtClassificationThresholdCoverMin_S3.Text) > 255)
                {
                    MyTool.TxtFileProcess.CreateLog("控件txtClassificationThresholdCoverMin_S3的数据范围错误,取值范围是[0,255]！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("控件txtClassificationThresholdCoverMin_S3的数据范围错误,取值范围是[0,255]！");
                    dataCheckResult = false;
                }
                if (Convert.ToSingle(txtClassificationThresholdCoverMax_S3.Text) < 0 || Convert.ToSingle(txtClassificationThresholdCoverMax_S3.Text) > 255)
                {
                    MyTool.TxtFileProcess.CreateLog("控件txtClassificationThresholdCoverMax_S3的数据范围错误,取值范围是[0,255]！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("控件txtClassificationThresholdCoverMax_S3的数据范围错误,取值范围是[0,255]！");
                    dataCheckResult = false;
                }
                if (Convert.ToSingle(txtClassificationThresholdCoverMax_S3.Text) < Convert.ToSingle(txtClassificationThresholdCoverMin_S3.Text))
                {
                    MyTool.TxtFileProcess.CreateLog("控件txtClassificationThresholdCoverMax_S3的数据不能小于控件txtClassificationThresholdCoverMin_S3的数据！" + "----用户：" + ActionForm._operatorName,
                    ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                    MessageBox.Show("控件txtClassificationThresholdCoverMax_S3的数据不能小于控件txtClassificationThresholdCoverMin_S3的数据！");
                    dataCheckResult = false;
                }

                if (!dataCheckResult)
                    return;
                _classificationResult_S3 = classification(hw_S3, temp1, temp2, temp3, temp4, temp5, temp6, temp7, temp8, temp9, temp10);
            }
            else
            {
                MessageBox.Show("S3请停止相机视频再执行!");
            }
        }

        #region classification:区分是产品,牛皮纸,空
        /// <summary>
        /// classification:区分是产品,牛皮纸,空
        /// </summary>
        /// <param name="_ClassificationThresholdMin">float</param>
        /// <param name="_ClassificationThresholdMax">float</param>
        /// <param name="_ProductAreaMin">float</param>
        /// <param name="_ProductAreaMax">float</param>
        /// <param name="_CoverAreaMin">float</param>
        /// <param name="_CoverAreaMax">float</param>
        /// <param name="_EmptyAreaMin">float</param>
        /// <param name="_EmptyAreaMax">float</param>
        /// <param name="_ClassificationThresholdEmptyMin">float</param>
        /// <param name="_ClassificationThresholdEmptyMax">float</param>
        /// <returns>int:-1,-2,1,2,3(-1:分类运算错误;-2:不属于这3类;1:产品;2:牛皮纸;3:空)</returns>
        public int classification(HWindow_Final hw, float _ClassificationThresholdMin, float _ClassificationThresholdMax, float _ProductAreaMin, float _ProductAreaMax, float _CoverAreaMin, float _CoverAreaMax, float _EmptyAreaMin, float _EmptyAreaMax, float _ClassificationThresholdCoverMin, float _ClassificationThresholdCoverMax)
        {
            HObject region;
            HTuple area;
            HTuple row;
            HTuple column;
            int result = -1;
            string _classificationName = "";
            try
            {
                result = -2;
                _classificationName = "NA";

                HOperatorSet.Threshold(ho_Image_S3, out region, _ClassificationThresholdCoverMin, _ClassificationThresholdCoverMax);
                HOperatorSet.AreaCenter(region, out area, out row, out column);
                if (area >= _CoverAreaMin && area <= _CoverAreaMax)
                {
                    result = 2;
                    _classificationName = "纸";
                }

                if (result != 2)
                {
                    HOperatorSet.Threshold(ho_Image_S3, out region, _ClassificationThresholdMin, _ClassificationThresholdMax);
                    HOperatorSet.AreaCenter(region, out area, out row, out column);
                    if (area >= _ProductAreaMin && area <= _ProductAreaMax)
                    {
                        result = 1;
                        _classificationName = "产品";
                    }
                    if (area >= _EmptyAreaMin && area <= _EmptyAreaMax)
                    {
                        result = 3;
                        _classificationName = "空";
                    }
                }

                hw.viewWindow.displayHobject(region, "green", true);
                hw.viewWindow.dispMessage(area.ToString() + ":" + _classificationName, "red", 0, 0, "false");

                MyTool.TxtFileProcess.CreateLog("S3分类结果是 " + area.ToString() + ":" + _classificationName + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
            }
            catch (Exception ex)
            {
                hw.viewWindow.dispMessage("S3分类运算错误!", "red", 0, 0, "false");
                MyTool.TxtFileProcess.CreateLog("S3分类运算错误!" + "----用户：" + ActionForm._operatorName,
                ActionForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, ActionForm._logFileMaxSize);
                return -1;
            }
            return result;
        }
        #endregion

        #endregion

        #region CheckAllCtrls:递归遍历所有的子孙控件包括容器类
        private static void CheckAllCtrls(Control item)
        {
            for (int i = 0; i < item.Controls.Count; i++)
            {
                if (item.Controls[i].HasChildren)
                {
                    CheckAllCtrls(item.Controls[i]);
                }
                //else{allCtrls.Enqueue (item.Controls[i]);}//如果只要子控件,那么这个语句在else里
                allCtrlsQueue.Enqueue(item.Controls[i]);
            }
        }
        #endregion

        #region tabControl_SelectedIndexChanged
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 0 || tabControl1.SelectedIndex == 1 || tabControl1.SelectedIndex == 2)
                tabControl2.SelectedIndex = tabControl1.SelectedIndex;
        }

        private void tabControl2_SelectedIndexChanged(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = tabControl2.SelectedIndex;
        }
        #endregion
    }
}