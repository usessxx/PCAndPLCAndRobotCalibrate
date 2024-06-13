using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using XMLFile;
using MyTool;
using SoftKeyBoard;
using System.Xml;
using CSVFile;
using AxisAndIOForm;
using HalconMVTec;
using HalconDotNet;

namespace ThreeDimensionAVI
{
    public partial class SettingsProductForm : Form
    {
        /*--------------------------------------------------------------------------------------
        //Copyright (C) 2022 深圳市利器精工科技有限公司
        //版权所有
        //
        //文件名：产品参数设定画面
        //文件功能描述：用于设置产品参数的画面
        //
        //
        //创建标识：MaLi 20220316
        //
        //修改标识：MaLi 20220316 Change
        //修改描述：增加产品参数设定画面
        //
        //------------------------------------------------------------------------------------*/
        //
        //******************************事件************************************//
        public event MaintenanceCameraFormRequestGrabImageAndDispDelegate SettingsProductFormRequestGrabImageAndDispEvent;//请求相机抓取图像
        //*************************外部可读写变量*******************************//
        public static ProductRecipe _editProductParameter = new ProductRecipe();//编辑产品的数据
        public HObject ho_backCameraImage = null;//后相机获取到的图片HObject
        public TrackDisplayBasePointData _trackDispFormVariate = null;//轨迹显示窗口变量函数
        //*************************公共静态变量*******************************//
        public static string _track1ActualsProductName;//传送线1产品名称
        public static string _track2ActualsProductName;//传送线2产品名称
        public static string _editProductName = "";//当前编辑页面产品名称
        public static bool _useRelativePos;//选择使用绝对位置还是基于校准工具的相对位置
        //*************************内部私有变量*******************************//


        //需要保存参数的变量
        const int LOAD_SAVE_DATA_ELEMENT_QUANTITY = 72;
        //用于存储需要保存读取的参数变量名和xml名称对应数组,0-控件名称，1-xml文件名称
        string[,] _ctrlNameAndXmlFileNameStr = new string[2, LOAD_SAVE_DATA_ELEMENT_QUANTITY];

        string[] _loadSaveDataName = { "Track1ActualProduct","Track2ActualProduct", "UseRelativePositions" };//当前页面应当保存的参数节点名

        CreateNewProductDataFileForm _createNewProductFormVariate = null;//创建新产品form变量

        ////////////////////////////////////////////////////

        #region InitialCtrlBoxCorrespondingStringArr:初始化控件名和xml文件保存名称对应表
        void InitialCtrlBoxCorrespondingStringArr()
        {
            MyTool.TxtFileProcess.CreateLog("产品设置页面-初始化控件名和变量名对应表！" + "----用户：" + BaseForm._operatorName,
            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

            #region 控件名称
            //general parameter
            _ctrlNameAndXmlFileNameStr[0, 0] = "txtX1AxisLocationVelocity";
            _ctrlNameAndXmlFileNameStr[0, 1] = "txtY1AxisLocationVelocity";
            _ctrlNameAndXmlFileNameStr[0, 2] = "txtR1AxisLocationVelocity";
            _ctrlNameAndXmlFileNameStr[0, 3] = "txtY2AxisLocationVelocity";
            _ctrlNameAndXmlFileNameStr[0, 4] = "txtY3AxisLocationVelocity";
            _ctrlNameAndXmlFileNameStr[0, 5] = "txtY4AxisLocationVelocity";
            _ctrlNameAndXmlFileNameStr[0, 6] = "txtX1AxisLocationAccTime";
            _ctrlNameAndXmlFileNameStr[0, 7] = "txtY1AxisLocationAccTime";
            _ctrlNameAndXmlFileNameStr[0, 8] = "txtR1AxisLocationAccTime";
            _ctrlNameAndXmlFileNameStr[0, 9] = "txtY2AxisLocationAccTime";
            _ctrlNameAndXmlFileNameStr[0, 10] = "txtY3AxisLocationAccTime";
            _ctrlNameAndXmlFileNameStr[0, 11] = "txtY4AxisLocationAccTime";
            //conveyor parameter
            _ctrlNameAndXmlFileNameStr[0, 12] = "txtY2Position";
            _ctrlNameAndXmlFileNameStr[0, 13] = "txtY3Position";
            _ctrlNameAndXmlFileNameStr[0, 14] = "txtY4Position";
            _ctrlNameAndXmlFileNameStr[0, 15] = "txtConveyor1InHighSpeed";
            _ctrlNameAndXmlFileNameStr[0, 16] = "txtConveyor1InLowSpeed";
            _ctrlNameAndXmlFileNameStr[0, 17] = "txtConveyor1OutSpeed";
            _ctrlNameAndXmlFileNameStr[0, 18] = "txtConveyor2InHighSpeed";
            _ctrlNameAndXmlFileNameStr[0, 19] = "txtConveyor2InLowSpeed";
            _ctrlNameAndXmlFileNameStr[0, 20] = "txtConveyor2OutSpeed";

            _ctrlNameAndXmlFileNameStr[0, 21] = "txtConveyor1LoadFinishTime";
            _ctrlNameAndXmlFileNameStr[0, 22] = "txtConveyor1UnloadFinishTime";
            _ctrlNameAndXmlFileNameStr[0, 23] = "txtConveyor1LoadTimeOutTime";
            _ctrlNameAndXmlFileNameStr[0, 24] = "txtConveyor1UnloadTimeOutTime";
            _ctrlNameAndXmlFileNameStr[0, 25] = "txtConveyor2LoadFinishTime";
            _ctrlNameAndXmlFileNameStr[0, 26] = "txtConveyor2UnloadFinishTime";
            _ctrlNameAndXmlFileNameStr[0, 27] = "txtConveyor2LoadTimeOutTime";
            _ctrlNameAndXmlFileNameStr[0, 28] = "txtConveyor2UnloadTimeOutTime";
            _ctrlNameAndXmlFileNameStr[0, 29] = "txtConveyor1StopCylinderUpTime";
            _ctrlNameAndXmlFileNameStr[0, 30] = "txtConveyor2StopCylinderUpTime";
            //point position
            _ctrlNameAndXmlFileNameStr[0, 31] = "txtTrack1TeachedPointNo";
            _ctrlNameAndXmlFileNameStr[0, 32] = "txtTrack1X1TeachedCoor";
            _ctrlNameAndXmlFileNameStr[0, 33] = "txtTrack1Y1TeachedCoor";
            _ctrlNameAndXmlFileNameStr[0, 34] = "txtTrack1R1TeachedCoor";

            _ctrlNameAndXmlFileNameStr[0, 35] = "txtTrack2TeachedPointNo";
            _ctrlNameAndXmlFileNameStr[0, 36] = "txtTrack2X1TeachedCoor";
            _ctrlNameAndXmlFileNameStr[0, 37] = "txtTrack2Y1TeachedCoor";
            _ctrlNameAndXmlFileNameStr[0, 38] = "txtTrack2R1TeachedCoor";
            //Deep learning
            _ctrlNameAndXmlFileNameStr[0, 39] = "txtPreprocessParameterFilePath";
            _ctrlNameAndXmlFileNameStr[0, 40] = "txtDimensionFilePath";
            _ctrlNameAndXmlFileNameStr[0, 41] = "txtModelFilePath";

            _ctrlNameAndXmlFileNameStr[0, 42] = "txtProductNameUseForMES";
            //mark search
            _ctrlNameAndXmlFileNameStr[0, 43] = "txtMarkSearchThresholdMin";
            _ctrlNameAndXmlFileNameStr[0, 44] = "txtMarkSearchThresholdMax";
            _ctrlNameAndXmlFileNameStr[0, 45] = "txtMarkSelectShapeMin";
            _ctrlNameAndXmlFileNameStr[0, 46] = "txtMarkSelectShapeMax";

            _ctrlNameAndXmlFileNameStr[0, 47] = "txtConveyor1Mark1SearchRowResult";
            _ctrlNameAndXmlFileNameStr[0, 48] = "txtConveyor1Mark1SearchColumnResult";
            _ctrlNameAndXmlFileNameStr[0, 49] = "txtConveyor1Mark2SearchRowResult";
            _ctrlNameAndXmlFileNameStr[0, 50] = "txtConveyor1Mark2SearchColumnResult";

            _ctrlNameAndXmlFileNameStr[0, 51] = "txtConveyor2Mark1SearchRowResult";
            _ctrlNameAndXmlFileNameStr[0, 52] = "txtConveyor2Mark1SearchColumnResult";
            _ctrlNameAndXmlFileNameStr[0, 53] = "txtConveyor2Mark2SearchRowResult";
            _ctrlNameAndXmlFileNameStr[0, 54] = "txtConveyor2Mark2SearchColumnResult";

            _ctrlNameAndXmlFileNameStr[0, 55] = "txtShieldProduct1Barcode";
            _ctrlNameAndXmlFileNameStr[0, 56] = "txtShieldProduct2Barcode";
            _ctrlNameAndXmlFileNameStr[0, 57] = "txtShieldProduct3Barcode";

            _ctrlNameAndXmlFileNameStr[0, 58] = "txtMarkSearchMinCircularity";

            _ctrlNameAndXmlFileNameStr[0, 59] = "txtSampleProduct1Barcode";
            _ctrlNameAndXmlFileNameStr[0, 60] = "txtSampleProduct2Barcode";
            _ctrlNameAndXmlFileNameStr[0, 61] = "txtSampleProduct3Barcode";

            //深度学习推断
            _ctrlNameAndXmlFileNameStr[0, 62] = "txtInferResultKind1Number";
            _ctrlNameAndXmlFileNameStr[0, 63] = "txtInferResultKind2Number";
            _ctrlNameAndXmlFileNameStr[0, 64] = "txtInferResultKind3Number";
            _ctrlNameAndXmlFileNameStr[0, 65] = "txtInferResultKind4Number";
            _ctrlNameAndXmlFileNameStr[0, 66] = "txtInferResultKind5Number";

            _ctrlNameAndXmlFileNameStr[0, 67] = "txtInferResultKind1Quantity";
            _ctrlNameAndXmlFileNameStr[0, 68] = "txtInferResultKind2Quantity";
            _ctrlNameAndXmlFileNameStr[0, 69] = "txtInferResultKind3Quantity";
            _ctrlNameAndXmlFileNameStr[0, 70] = "txtInferResultKind4Quantity";
            _ctrlNameAndXmlFileNameStr[0, 71] = "txtInferResultKind5Quantity";

            #endregion

            #region 在XML中保存读取的参数名称
            //general parameter
            _ctrlNameAndXmlFileNameStr[1, 0] = "X1AxisLocationVelocity";
            _ctrlNameAndXmlFileNameStr[1, 1] = "Y1AxisLocationVelocity";
            _ctrlNameAndXmlFileNameStr[1, 2] = "R1AxisLocationVelocity";
            _ctrlNameAndXmlFileNameStr[1, 3] = "Y2AxisLocationVelocity";
            _ctrlNameAndXmlFileNameStr[1, 4] = "Y3AxisLocationVelocity";
            _ctrlNameAndXmlFileNameStr[1, 5] = "Y4AxisLocationVelocity";
            _ctrlNameAndXmlFileNameStr[1, 6] = "X1AxisLocationAccTime";
            _ctrlNameAndXmlFileNameStr[1, 7] = "Y1AxisLocationAccTime";
            _ctrlNameAndXmlFileNameStr[1, 8] = "R1AxisLocationAccTime";
            _ctrlNameAndXmlFileNameStr[1, 9] = "Y2AxisLocationAccTime";
            _ctrlNameAndXmlFileNameStr[1, 10] = "Y3AxisLocationAccTime";
            _ctrlNameAndXmlFileNameStr[1, 11] = "Y4AxisLocationAccTime";
            //conveyor
            _ctrlNameAndXmlFileNameStr[1, 12] = "Y2Position";
            _ctrlNameAndXmlFileNameStr[1, 13] = "Y3Position";
            _ctrlNameAndXmlFileNameStr[1, 14] = "Y4Position";
            _ctrlNameAndXmlFileNameStr[1, 15] = "Conveyor1InHighSpeed";
            _ctrlNameAndXmlFileNameStr[1, 16] = "Conveyor1InLowSpeed";
            _ctrlNameAndXmlFileNameStr[1, 17] = "Conveyor1OutSpeed";
            _ctrlNameAndXmlFileNameStr[1, 18] = "Conveyor2InHighSpeed";
            _ctrlNameAndXmlFileNameStr[1, 19] = "Conveyor2InLowSpeed";
            _ctrlNameAndXmlFileNameStr[1, 20] = "Conveyor2OutSpeed";

            _ctrlNameAndXmlFileNameStr[1, 21] = "Conveyor1LoadFinishTime";
            _ctrlNameAndXmlFileNameStr[1, 22] = "Conveyor1UnloadFinishTime";
            _ctrlNameAndXmlFileNameStr[1, 23] = "Conveyor1LoadTimeOutTime";
            _ctrlNameAndXmlFileNameStr[1, 24] = "Conveyor1UnloadTimeOutTime";
            _ctrlNameAndXmlFileNameStr[1, 25] = "Conveyor2LoadFinishTime";
            _ctrlNameAndXmlFileNameStr[1, 26] = "Conveyor2UnloadFinishTime";
            _ctrlNameAndXmlFileNameStr[1, 27] = "Conveyor2LoadTimeOutTime";
            _ctrlNameAndXmlFileNameStr[1, 28] = "Conveyor2UnloadTimeOutTime";

            _ctrlNameAndXmlFileNameStr[1, 29] = "Conveyor1StopCylinderUpTime";
            _ctrlNameAndXmlFileNameStr[1, 30] = "Conveyor2StopCylinderUpTime";
            //point position
            _ctrlNameAndXmlFileNameStr[1, 31] = "Track1TeachedPointNo";
            _ctrlNameAndXmlFileNameStr[1, 32] = "Track1X1TeachedCoor";
            _ctrlNameAndXmlFileNameStr[1, 33] = "Track1Y1TeachedCoor";
            _ctrlNameAndXmlFileNameStr[1, 34] = "Track1R1TeachedCoor";

            _ctrlNameAndXmlFileNameStr[1, 35] = "Track2TeachedPointNo";
            _ctrlNameAndXmlFileNameStr[1, 36] = "Track2X1TeachedCoor";
            _ctrlNameAndXmlFileNameStr[1, 37] = "Track2Y1TeachedCoor";
            _ctrlNameAndXmlFileNameStr[1, 38] = "Track2R1TeachedCoor";
            //Deep learning
            _ctrlNameAndXmlFileNameStr[1, 39] = "PreprocessParameterFilePath";
            _ctrlNameAndXmlFileNameStr[1, 40] = "DimensionFilePath";
            _ctrlNameAndXmlFileNameStr[1, 41] = "ModelFilePath";

            _ctrlNameAndXmlFileNameStr[1, 42] = "ProductNameUseForMES";

            //mark search
            _ctrlNameAndXmlFileNameStr[1, 43] = "MarkSearchThresholdMin";
            _ctrlNameAndXmlFileNameStr[1, 44] = "MarkSearchThresholdMax";
            _ctrlNameAndXmlFileNameStr[1, 45] = "MarkSelectShapeMin";
            _ctrlNameAndXmlFileNameStr[1, 46] = "MarkSelectShapeMax";

            _ctrlNameAndXmlFileNameStr[1, 47] = "Conveyor1Mark1SearchRowResult";
            _ctrlNameAndXmlFileNameStr[1, 48] = "Conveyor1Mark1SearchColumnResult";
            _ctrlNameAndXmlFileNameStr[1, 49] = "Conveyor1Mark2SearchRowResult";
            _ctrlNameAndXmlFileNameStr[1, 50] = "Conveyor1Mark2SearchColumnResult";

            _ctrlNameAndXmlFileNameStr[1, 51] = "Conveyor2Mark1SearchRowResult";
            _ctrlNameAndXmlFileNameStr[1, 52] = "Conveyor2Mark1SearchColumnResult";
            _ctrlNameAndXmlFileNameStr[1, 53] = "Conveyor2Mark2SearchRowResult";
            _ctrlNameAndXmlFileNameStr[1, 54] = "Conveyor2Mark2SearchColumnResult";

            _ctrlNameAndXmlFileNameStr[1, 55] = "ShieldProduct1Barcode";
            _ctrlNameAndXmlFileNameStr[1, 56] = "ShieldProduct2Barcode";
            _ctrlNameAndXmlFileNameStr[1, 57] = "ShieldProduct3Barcode";

            _ctrlNameAndXmlFileNameStr[1, 58] = "MarkSearchMinCircularity";

            _ctrlNameAndXmlFileNameStr[1, 59] = "SampleProduct1Barcode";
            _ctrlNameAndXmlFileNameStr[1, 60] = "SampleProduct2Barcode";
            _ctrlNameAndXmlFileNameStr[1, 61] = "SampleProduct3Barcode";

            //深度学习推断结果OK判定条件参数
            _ctrlNameAndXmlFileNameStr[1, 62] = "InferResultKind1Number";
            _ctrlNameAndXmlFileNameStr[1, 63] = "InferResultKind2Number";
            _ctrlNameAndXmlFileNameStr[1, 64] = "InferResultKind3Number";
            _ctrlNameAndXmlFileNameStr[1, 65] = "InferResultKind4Number";
            _ctrlNameAndXmlFileNameStr[1, 66] = "InferResultKind5Number";

            _ctrlNameAndXmlFileNameStr[1, 67] = "InferResultKind1Quantity";
            _ctrlNameAndXmlFileNameStr[1, 68] = "InferResultKind2Quantity";
            _ctrlNameAndXmlFileNameStr[1, 69] = "InferResultKind3Quantity";
            _ctrlNameAndXmlFileNameStr[1, 70] = "InferResultKind4Quantity";
            _ctrlNameAndXmlFileNameStr[1, 71] = "InferResultKind5Quantity";

            #endregion
        }
        #endregion

        //构造函数
        public SettingsProductForm()
        {
            InitializeComponent();

            InitialCtrlBoxCorrespondingStringArr();//初始化控件名和xml文件保存名称对应表

            MainFormDataLoadingSettingData();//从settings.xml文件中读取参数
            GetProductDataFileNameAndSetAvaliableProductComboBox();//获取可用的产品名称
            if (CheckCurrentProductUsefulOrNot(ref _track1ActualsProductName, true))//检测当前产品是否可用
            {
                _editProductName = _track1ActualsProductName;
            }
            else//如果track1的产品不可以使用
            {
                if (CheckCurrentProductUsefulOrNot(ref _track2ActualsProductName, true))//检测track2的产品是否可以使用
                {
                    _editProductName = _track2ActualsProductName;
                }
                else
                {
                    _editProductName = _track1ActualsProductName;//如果track1，track2都不可以使用，那么将
                }
            }

            if (ProductDataLoadingToCtrl(_editProductName))
            {
                _editProductParameter.ProductDataLoadToVariate(SettingsGeneralForm._productDirectory + "\\" + _editProductName + ".rcp");
            }
            else
            {
                _editProductParameter._parameterLoadSuccessfulFlag = false;
            }

            AxisControlMainForm._currentProductName = _editProductName;

            dgrdvProductPointPosition.CellDoubleClick += DataGridViewDoubleClickEvent;//声明事件，用于打开软键盘
            dgrdvProductPointPosition.CellLeave += DataGridViewDataCheckEvent;//声明事件，用于最后检查数据
            dgrdvProductPointPosition.CellValueChanged += DataGridViewLastDataCheckEvent;//声明事件，用于检查数据
            dgrdvProductPointPosition.CellMouseClick += DataGridViewSelectTeachPointEvent;//声明事件，用于设定作为基准的点位号
            
            for(int i=0;i<dgrdvProductPointPosition.Columns.Count;i++)
            {
                dgrdvProductPointPosition.Columns[i].ValueType = typeof(string);
            }
        }

        //界面显示或隐藏变换事件
        private void SettingsProductForm_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                MainFormDataLoadingSettingData();
            }
            else
            {
                MainFormDataSavingSettingData();
            }

            //显示当前产品名称
            lblActualProduct.Text = "传送线1产品: " + _track1ActualsProductName + "   传送线2产品："+_track2ActualsProductName;
        }

        //获取指定文件夹中的文件名称并赋值到combobox中
        public void GetProductDataFileNameAndSetAvaliableProductComboBox()
        {
            bool currentProductUsefulFlag = false;
            string[] fileName;
            int currentProductIndex = -1;
            if (Directory.Exists(SettingsGeneralForm._productDirectory))
            {
                fileName = FolderAndFileManageClass.ScanFolderAndGetAssignedFormatFileName(SettingsGeneralForm._productDirectory, ".rcp");

                cboAvaliableProduct.Items.Clear();
                for (int i = 0; i < fileName.Length; i++)
                {
                    if (_track1ActualsProductName == fileName[i])
                    {
                        currentProductIndex = i;
                        currentProductUsefulFlag = true;
                    }

                    if (_track2ActualsProductName == fileName[i] && !currentProductUsefulFlag)
                    {
                        currentProductIndex = i;
                        currentProductUsefulFlag = true;
                    }
                    cboAvaliableProduct.Items.Add(fileName[i]);
                }
            }

            if (!currentProductUsefulFlag)
            {
                _track1ActualsProductName = "";
                _track2ActualsProductName = "";
            }
            cboAvaliableProduct.SelectedIndex = currentProductIndex;
        }

        /// <summary>
        /// 检测当前名称的产品数据是否存在
        /// </summary>
        /// <param name="ProductName">产品名称</param>
        /// <param name="EmptyFlag">不存在的话情况与否标签，true-清空，false-不清空</param>
        /// <returns>返回存不存在，true-存在，false-不存在</returns>
        public bool CheckCurrentProductUsefulOrNot(ref string ProductName, bool EmptyFlag)
        {
            MyTool.TxtFileProcess.CreateLog("产品设置页面-检查当前产品是否可用！" + "----用户：" + BaseForm._operatorName,
            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

            bool currentProductUsefulFlag = false;
            string[] fileName;
            if (Directory.Exists(SettingsGeneralForm._productDirectory))
            {
                fileName = MyTool.FolderAndFileManageClass.ScanFolderAndGetAssignedFormatFileName(SettingsGeneralForm._productDirectory, ".rcp");
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

        #region 读取保存产品参数

        /// <summary>
        /// ProductDataSavingSettingData：保存设置参数文件
        /// </summary>
        /// <param name="FileName">string：保存的文件的名称</param>
        /// <param name="DataCheckFlag">bool：是否进行数据检查，在创建新的产品数据的时候不需要进行数据检查</param>
        /// <returns>bool: 反馈保存结果，true-保存成功，false-保存失败</returns>
        private bool ProductDataSavingFromCtrl(string FileName, bool DataCheckFlag)
        {
            MyTool.TxtFileProcess.CreateLog("产品设置页面-从控件中读取数据保存至文件中！" + "----用户：" + BaseForm._operatorName,
            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

            string rootName = "clsRecipeData";
            string filePath = SettingsGeneralForm._productDirectory + "\\" + MyTool.FolderAndFileManageClass.FilterPathIllegalChar(FileName) + ".rcp";

            if (Directory.Exists(SettingsGeneralForm._productDirectory))
            {
                if (DataCheckFlag)
                {
                    if (!DataCheckFunc())//如果数据不合法，不允许保存
                    {
                        MyTool.TxtFileProcess.CreateLog("产品设置页面-从控件中读取数据保存至文件中失败，有非法数据存在！" + "----用户：" + BaseForm._operatorName,
                        BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

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
                string tempValue;//用于存储要保存的数据值

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
                    if ((!SettingsAdministratorForm._checkProductIsConnector && (!(i >= 62 && i <= 71))) ||
                                SettingsAdministratorForm._checkProductIsConnector)
                    {
                        if (ctrlNameSuffix == "txt")
                        {
                            tempValue = ((TextBox)this.Controls.Find(ctrlName, true)[0]).Text;
                        }
                        else if (ctrlNameSuffix == "chk")
                        {
                            if (((CheckBox)this.Controls.Find(ctrlName, true)[0]).Checked) //tempCtrl
                            {
                                tempValue = "true";
                            }
                            else
                            {
                                tempValue = "false";
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
                }
                xmlDoc.Save(filePath);
                xmlDoc = null;

                if (!SavePointPositionData(SettingsGeneralForm._productDirectory, FileName))
                {
                    return false;
                }
                return true;
            }
            else
            {
                MyTool.TxtFileProcess.CreateLog("产品设置页面-从控件中读取数据保存至文件中失败，指定的文件保存路径不存在！" + "----用户：" + BaseForm._operatorName,
                BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                MessageBox.Show("指定的读取保存产品参数的路径不存在，无法保存数据!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

        }

        /// <summary>
        /// ProductDataLoadingToCtrl:读取数据至控件函数
        /// </summary>
        /// <param name="FileName">string：产品名称</param>
        /// <returns>bool:false-读取失败，true-读取成功</returns>
        public bool ProductDataLoadingToCtrl(string FileName)
        {
            MyTool.TxtFileProcess.CreateLog("产品设置页面-从文件读取参数至控件！" + "----用户：" + BaseForm._operatorName,
            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

            string filePath = SettingsGeneralForm._productDirectory + "\\" + MyTool.FolderAndFileManageClass.FilterPathIllegalChar(FileName) + ".rcp";
            string tempValue;
            bool result = true;
            string rootName = "clsRecipeData";

            if (Directory.Exists(SettingsGeneralForm._productDirectory))
            {
                if (!File.Exists(filePath))
                {
                    //ProductDataSavingSettingData(_actualsProductName);
                    MyTool.TxtFileProcess.CreateLog("产品设置页面-产品参数文件不存在，从文件读取参数至控件失败！" + "----用户：" + BaseForm._operatorName,
            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                    MessageBox.Show("产品参数文件不存在，读取产品数据至控件失败!");
                    result = false;
                }
                else
                {
                    string ctrlNameSuffix;//用以存储控件后缀名
                    string ctrlName;//用以存储控件名
                    string[] tempNodeName = new string[LOAD_SAVE_DATA_ELEMENT_QUANTITY];
                    int loadUsefulNodeQuantity = 0;
                    bool[] dataLoadResultFlag = new bool[LOAD_SAVE_DATA_ELEMENT_QUANTITY];//用于判定是否读取数据OK，false-读取失败，true-读取成功

                    for (int i = 0; i < LOAD_SAVE_DATA_ELEMENT_QUANTITY; i++)
                    {
                        tempNodeName[i] = _ctrlNameAndXmlFileNameStr[1, i];
                        dataLoadResultFlag[i] = false;
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
                                ((TextBox)this.Controls.Find(ctrlName, true)[0]).Text = element.InnerText;
                            }
                            else if (ctrlNameSuffix == "chk")
                            {
                                tempValue = element.InnerText.ToString().ToLower();
                                if (tempValue == "true")
                                {
                                    ((CheckBox)this.Controls.Find(ctrlName, true)[0]).Checked = true;
                                }
                                else
                                {
                                    ((CheckBox)this.Controls.Find(ctrlName, true)[0]).Checked = false;

                                }
                            }
                            else
                            {
                                continue;
                            }
                            loadUsefulNodeQuantity++;
                            dataLoadResultFlag[index] = true;
                        }
                    }

                    //xmlDoc.Save(filePath);
                    xmlDoc = null;

                    if (loadUsefulNodeQuantity < LOAD_SAVE_DATA_ELEMENT_QUANTITY)
                    {
                        for (int i = 0; i < LOAD_SAVE_DATA_ELEMENT_QUANTITY; i++)
                        {
                            if ((!SettingsAdministratorForm._checkProductIsConnector && (!(i >= 62 && i <= 71))) ||
                                SettingsAdministratorForm._checkProductIsConnector)
                            {
                                if (!dataLoadResultFlag[i])
                                {
                                    MyTool.TxtFileProcess.CreateLog("产品设置页面-从文件读取参数至控件失败，在产品参数文件" + FileName + ".rcp中，无法找到参数" + _ctrlNameAndXmlFileNameStr[1, i] + "的数据" + "----用户：" + BaseForm._operatorName,
                                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                                    MessageBox.Show("在产品参数文件" + FileName + ".rcp中，无法找到参数" + _ctrlNameAndXmlFileNameStr[1, i] + "的数据，读取产品参数至控件中失败！");
                                    result = false;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                MyTool.TxtFileProcess.CreateLog("产品设置页面-产品参数文件夹路径不存在，从文件读取参数至控件失败！" + "----用户：" + BaseForm._operatorName,
                BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                MessageBox.Show("指定的读取保存产品参数的路径不存在，无法读取数据至控件!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                result = false;
            }

            if (!LoadPointPositionData(SettingsGeneralForm._productDirectory, FileName))
            {
                result = false;
            }

            if (!DataCheckFunc())
            {
                result = false;
            }
            return result;
        }

        //在读取参数完毕，refresh参数之前判定，数据是否符合要求
        private bool DataCheckFunc()
        {
            MyTool.TxtFileProcess.CreateLog("产品设置页面-数据检测！" + "----用户：" + BaseForm._operatorName,
            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

            bool dataCheckResult = true;//false:Check failed,true:check successful

            try
            {
                if (Convert.ToInt32(txtTrack1TeachedPointNo.Text) > dgrdvProductPointPosition.RowCount)
                {
                    MyTool.TxtFileProcess.CreateLog("产品设置页面-数据检测失败，读取的数据中，作为传送线1示教基准的点位号大于产品所有点位个数！" + "----用户：" + BaseForm._operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                    MessageBox.Show("读取的数据中，作为传送线1示教基准的点位号大于产品所有点位个数！");
                    dataCheckResult = false;
                }

                if (Convert.ToInt32(txtTrack2TeachedPointNo.Text) > dgrdvProductPointPosition.RowCount)
                {
                    MyTool.TxtFileProcess.CreateLog("产品设置页面-数据检测失败，读取的数据中，作为传送线2示教基准的点位号大于产品所有点位个数！" + "----用户：" + BaseForm._operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                    MessageBox.Show("读取的数据中，作为传送线2示教基准的点位号大于产品所有点位个数！");
                    dataCheckResult = false;
                }


                if (!File.Exists(txtPreprocessParameterFilePath.Text))
                {
                    MyTool.TxtFileProcess.CreateLog("产品设置页面-数据检测失败，读取的数据中，图像推断所用预处理参数文件不存在！" + "----用户：" + BaseForm._operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                    MessageBox.Show("读取的数据中，图像推断所用预处理参数文件不存在！");
                    dataCheckResult = false;
                }

                if (!File.Exists(txtDimensionFilePath.Text))
                {
                    MyTool.TxtFileProcess.CreateLog("产品设置页面-数据检测失败，读取的数据中，图像推断所用标注文件不存在！" + "----用户：" + BaseForm._operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                    MessageBox.Show("读取的数据中，图像推断所用标注文件不存在！");
                    dataCheckResult = false;
                }

                if (!File.Exists(txtModelFilePath.Text))
                {
                    MyTool.TxtFileProcess.CreateLog("产品设置页面-数据检测失败，读取的数据中，图像推断所用模型文件不存在！" + "----用户：" + BaseForm._operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                    MessageBox.Show("读取的数据中，图像推断所用模型文件不存在！");
                    dataCheckResult = false;
                }

                if (Convert.ToInt32(txtMarkSearchThresholdMin.Text) < 0 || Convert.ToInt32(txtMarkSearchThresholdMin.Text) > 255)
                {
                    MyTool.TxtFileProcess.CreateLog("产品设置页面-数据检测失败，读取的数据中，mark点识别二值化最小值范围有误，为" + txtMarkSearchThresholdMin.Text.ToString() + "----用户：" + BaseForm._operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                    MessageBox.Show("读取的数据中，mark点识别二值化最小值范围有误(应当大于0小于255)！");
                    dataCheckResult = false;
                }

                if (Convert.ToInt32(txtMarkSearchThresholdMax.Text) < 0 || Convert.ToInt32(txtMarkSearchThresholdMax.Text) > 255)
                {
                    MyTool.TxtFileProcess.CreateLog("产品设置页面-数据检测失败，读取的数据中，mark点识别二值化最小大值范围有误，为" + txtMarkSearchThresholdMax.Text.ToString() + "----用户：" + BaseForm._operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                    MessageBox.Show("读取的数据中，mark点识别二值化最大值范围有误(应当大于0小于255)！");
                    dataCheckResult = false;
                }

                if (Convert.ToSingle(txtMarkSelectShapeMin.Text) < 0)
                {
                    MyTool.TxtFileProcess.CreateLog("产品设置页面-数据检测失败，读取的数据中，mark点识别region特征选择范围下限值有误，为" + txtMarkSelectShapeMin.Text.ToString() + "----用户：" + BaseForm._operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                    MessageBox.Show("读取的数据中，mark点识别region特征选择范围下限值有误(应当大于0)！");
                    dataCheckResult = false;
                }

                if (Convert.ToSingle(txtMarkSelectShapeMax.Text) < 0)
                {
                    MyTool.TxtFileProcess.CreateLog("产品设置页面-数据检测失败，读取的数据中，mark点识别region特征选择范围上限值有误，为" + txtMarkSelectShapeMin.Text.ToString() + "----用户：" + BaseForm._operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                    MessageBox.Show("读取的数据中，mark点识别region特征选择范围上限值有误(应当大于0)！");
                    dataCheckResult = false;
                }

                if (Convert.ToSingle(txtConveyor1Mark1SearchRowResult.Text) < 0 || Convert.ToSingle(txtConveyor1Mark1SearchColumnResult.Text) < 0 ||
                    Convert.ToSingle(txtConveyor1Mark2SearchRowResult.Text) < 0 || Convert.ToSingle(txtConveyor1Mark2SearchColumnResult.Text) < 0 ||
                    Convert.ToSingle(txtConveyor2Mark1SearchRowResult.Text) < 0 || Convert.ToSingle(txtConveyor2Mark1SearchColumnResult.Text) < 0 ||
                    Convert.ToSingle(txtConveyor2Mark2SearchRowResult.Text) < 0 || Convert.ToSingle(txtConveyor2Mark2SearchColumnResult.Text) < 0)
                {
                    MyTool.TxtFileProcess.CreateLog("产品设置页面-数据检测失败，读取的数据中，mark点识别结果有误，所有值应当大于0！" + "----用户：" + BaseForm._operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                    MessageBox.Show("读取的数据中，mark点识别结果有误，所有值应当大于0！");
                    dataCheckResult = false;
                }


                if (Convert.ToSingle(txtMarkSearchMinCircularity.Text) < 0f || Convert.ToSingle(txtMarkSearchMinCircularity.Text) > 1f)
                {
                    MyTool.TxtFileProcess.CreateLog("产品配方类-读取配方数据检查，读取的数据中，mark点识别最小圆度值有误，值应当大于等于0小于等于1！" + "----用户：" + BaseForm._operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                    MessageBox.Show("读取的数据中，mark点识别最小圆度值有误，值应当大于等于0小于等于1！");
                    dataCheckResult = false;
                }

                if ((Convert.ToInt32(txtInferResultKind1Number.Text) < 0 || Convert.ToInt32(txtInferResultKind1Quantity.Text) < 0 ||
                    Convert.ToInt32(txtInferResultKind1Number.Text) < 0 || Convert.ToInt32(txtInferResultKind1Quantity.Text) < 0 ||
                    Convert.ToInt32(txtInferResultKind1Number.Text) < 0 || Convert.ToInt32(txtInferResultKind1Quantity.Text) < 0 ||
                    Convert.ToInt32(txtInferResultKind1Number.Text) < 0 || Convert.ToInt32(txtInferResultKind1Quantity.Text) < 0 ||
                    Convert.ToInt32(txtInferResultKind1Number.Text) < 0 || Convert.ToInt32(txtInferResultKind1Quantity.Text) < 0) && SettingsAdministratorForm._checkProductIsConnector)
                {
                    MyTool.TxtFileProcess.CreateLog("产品设置页面-数据检测失败，读取的数据中，AOI推断结果OK的判定条件参数中，所有值应当大于0！" + "----用户：" + BaseForm._operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                    MessageBox.Show("读取的数据中，AOI推断结果OK的判定条件参数中，所有值应当大于0！");
                    dataCheckResult = false;
                }

                if((Convert.ToInt32(txtInferResultKind1Number.Text) != 0 && 
                    (Convert.ToInt32(txtInferResultKind1Number.Text) == Convert.ToInt32(txtInferResultKind2Number.Text) ||
                    Convert.ToInt32(txtInferResultKind1Number.Text) == Convert.ToInt32(txtInferResultKind3Number.Text) || 
                    Convert.ToInt32(txtInferResultKind1Number.Text) == Convert.ToInt32(txtInferResultKind4Number.Text) || 
                    Convert.ToInt32(txtInferResultKind1Number.Text) == Convert.ToInt32(txtInferResultKind5Number.Text))) || 
                    (Convert.ToInt32(txtInferResultKind2Number.Text) != 0 && 
                    (Convert.ToInt32(txtInferResultKind2Number.Text) == Convert.ToInt32(txtInferResultKind3Number.Text) ||
                    Convert.ToInt32(txtInferResultKind2Number.Text) == Convert.ToInt32(txtInferResultKind4Number.Text) || 
                    Convert.ToInt32(txtInferResultKind2Number.Text) == Convert.ToInt32(txtInferResultKind5Number.Text))) ||
                    (Convert.ToInt32(txtInferResultKind3Number.Text) != 0 && 
                    (Convert.ToInt32(txtInferResultKind3Number.Text) == Convert.ToInt32(txtInferResultKind4Number.Text) || 
                    Convert.ToInt32(txtInferResultKind3Number.Text) == Convert.ToInt32(txtInferResultKind5Number.Text))) ||
                    (Convert.ToInt32(txtInferResultKind4Number.Text) != 0 && 
                    Convert.ToInt32(txtInferResultKind4Number.Text) == Convert.ToInt32(txtInferResultKind5Number.Text)) )
                {
                    MyTool.TxtFileProcess.CreateLog("产品设置页面-数据检测失败，读取的数据中，AOI推断结果OK的判定条件参数中，有相同不为0的种类号参数！" + "----用户：" + BaseForm._operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                    MessageBox.Show("读取的数据中，AOI推断结果OK的判定条件参数中，有相同不为0的种类号参数！");
                    dataCheckResult = false;
                }
            
            }
            catch
            {
                dataCheckResult = false;
            }

            if (dataCheckResult)
                MyTool.TxtFileProcess.CreateLog("产品设置页面-数据检测成功" + "----用户：" + BaseForm._operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);


            return dataCheckResult;
        }

        #region 产品运动点位坐标表读取保存事件
        /// <summary>
        /// LoadPointPositionData:读取点位坐标数据
        /// </summary>
        /// <param name="pointPositionFileloadFolder">string:读取文件文件夹路径</param>
        /// <param name="fileName">string:读取文件名</param>
        /// <returns>bool:返回值，false-读取失败，true-读取成功</returns>
        private bool LoadPointPositionData(string pointPositionFileloadFolder, string fileName)
        {
            MyTool.TxtFileProcess.CreateLog("产品设置页面-读取产品点位数据！" + "----用户：" + BaseForm._operatorName,
            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

            string pointPositionFilePath; //点位参数文件读取路径
            DataTable pointPositionDataDT = null;//点位坐标数据datatable
            string fileSuffix = "";
            try
            {
                fileSuffix = MyTool.FolderAndFileManageClass.FilterPathIllegalChar(fileName);
                fileSuffix = fileSuffix.Substring(fileSuffix.LastIndexOf("."));
                if (fileSuffix.ToLower() == ".csv")
                    pointPositionFilePath = pointPositionFileloadFolder + "\\" + MyTool.FolderAndFileManageClass.FilterPathIllegalChar(fileName);
                else
                    pointPositionFilePath = pointPositionFileloadFolder + "\\" + MyTool.FolderAndFileManageClass.FilterPathIllegalChar(fileName) + ".csv";
            }
            catch
            {
                pointPositionFilePath = pointPositionFileloadFolder + "\\" + MyTool.FolderAndFileManageClass.FilterPathIllegalChar(fileName) + ".csv";
            }

            dgrdvProductPointPosition.Rows.Clear();
            if (!Directory.Exists(pointPositionFileloadFolder))
            {
                MyTool.TxtFileProcess.CreateLog("产品设置页面-读取产品点位数据失败，保存产品点位坐标数据的路径不存在！" + "----用户：" + BaseForm._operatorName,
                BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                MessageBox.Show("保存产品点位坐标数据的路径不存在，无法读取点位坐标数据至控件!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            if (!File.Exists(pointPositionFilePath))
            {
                MyTool.TxtFileProcess.CreateLog("产品设置页面-读取产品点位数据失败，保存产品点位坐标数据的文件不存在！" + "----用户：" + BaseForm._operatorName,
                BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                MessageBox.Show("保存产品点位坐标数据的文件不存在，无法读取点位坐标数据至控件!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            //读取数据至DT中
            pointPositionDataDT = CSVFileOperation.ReadCSV(pointPositionFilePath);//不包含第一行
            if (pointPositionDataDT.Columns.Count != 6)//如果列数不对，全部重新初始化
            {
                MyTool.TxtFileProcess.CreateLog("产品设置页面-读取产品点位数据失败，保存产品点位坐标数据的文件格式不正确！" + "----用户：" + BaseForm._operatorName,
                BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                MessageBox.Show("保存产品点位坐标数据的文件格式不正确，无法读取点位坐标数据至控件!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            else
            {

                //更新数据至窗口界面
                dgrdvProductPointPosition.Rows.Clear();
                for (int i = 0; i < pointPositionDataDT.Rows.Count; i++)
                {
                    DataRow dr = pointPositionDataDT.Rows[i];
                    dgrdvProductPointPosition.Rows.Add(dr.ItemArray);
                }
                if (PointPositionDataCheck(pointPositionDataDT))
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
        /// SavePointPositionData:保存点位坐标数据
        /// </summary>
        /// <param name="pointPositionFileSaveFolder">string:保存文件文件夹路径</param>
        /// <param name="fileName">string:保存文件名</param>
        /// <returns>bool:返回值，false-保存失败，true-保存成功</returns>
        private bool SavePointPositionData(string pointPositionFileSaveFolder, string fileName)
        {
            MyTool.TxtFileProcess.CreateLog("产品设置页面-保存产品点位数据！" + "----用户：" + BaseForm._operatorName,
            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

            string pointPositionFilePath; //点位参数文件保存路径
            DataTable pointPositionDataDT = null;//点位坐标数据datatable
            string fileSuffix = "";
            try
            {
                fileSuffix = MyTool.FolderAndFileManageClass.FilterPathIllegalChar(fileName);
                fileSuffix = fileSuffix.Substring(fileSuffix.LastIndexOf("."));
                if (fileSuffix.ToLower() == ".csv")
                    pointPositionFilePath = pointPositionFileSaveFolder + "\\" + MyTool.FolderAndFileManageClass.FilterPathIllegalChar(fileName);
                else
                    pointPositionFilePath = pointPositionFileSaveFolder + "\\" + MyTool.FolderAndFileManageClass.FilterPathIllegalChar(fileName) + ".csv";
            }
            catch
            {
                pointPositionFilePath = pointPositionFileSaveFolder + "\\" + MyTool.FolderAndFileManageClass.FilterPathIllegalChar(fileName) + ".csv";
            }

            if (!Directory.Exists(pointPositionFileSaveFolder))
            {
                MyTool.TxtFileProcess.CreateLog("产品设置页面-保存产品点位坐标数据的路径不存在，无法保存点位坐标数据文件！" + "----用户：" + BaseForm._operatorName,
            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                MessageBox.Show("保存产品点位坐标数据的路径不存在，无法保存点位坐标数据文件!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            //从datagridview中读取数据
            pointPositionDataDT = new DataTable();
            for (int i = 0; i < dgrdvProductPointPosition.ColumnCount; i++)
            {
                pointPositionDataDT.Columns.Add(dgrdvProductPointPosition.Columns[i].Name, typeof(string));
            }
            for (int i = 0; i < dgrdvProductPointPosition.Rows.Count; i++)
            {
                DataRow dr = pointPositionDataDT.NewRow();
                for (int j = 0; j < dgrdvProductPointPosition.ColumnCount; j++)
                {
                    dr[j] = dgrdvProductPointPosition.Rows[i].Cells[j].Value;
                }
                pointPositionDataDT.Rows.Add(dr.ItemArray);
            }

            //如果检测数据失败
            if (!PointPositionDataCheck(pointPositionDataDT))
            {
                MyTool.TxtFileProcess.CreateLog("产品设置页面-控件中有非法参数，无法保存点位坐标数据文件!" + "----用户：" + BaseForm._operatorName,
            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                MessageBox.Show("控件中有非法参数，无法保存点位坐标数据文件!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            //保存数据文件
            try
            {
                CSVFileOperation.SaveCSV(pointPositionDataDT, pointPositionFilePath);
                return true;
            }
            catch
            {
                MyTool.TxtFileProcess.CreateLog("产品设置页面-保存点位坐标数据文件失败!" + "----用户：" + BaseForm._operatorName,
                BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                return false;
            }
        }

        /// <summary>
        /// PointPositionDataCheck：点位坐标数据检测
        /// </summary>
        /// <param name="sourceDT">DataTable:需要检测的数据DT</param>
        /// <returns>bool:返回值，false-失败，true-成功</returns>
        private bool PointPositionDataCheck(DataTable sourceDT)
        {
            MyTool.TxtFileProcess.CreateLog("产品设置页面-点位数据检测!" + "----用户：" + BaseForm._operatorName,
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
                    MyTool.TxtFileProcess.CreateLog("产品设置页面-检测产品点位坐标数据失败，非法数据坐标为第2列，第" + (i + 1).ToString() + "行！" + "----用户：" + BaseForm._operatorName,
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
                    MyTool.TxtFileProcess.CreateLog("产品设置页面-检测产品点位坐标数据失败，非法数据坐标为第3列，第" + (i + 1).ToString() + "行！" + "----用户：" + BaseForm._operatorName,
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
                    MyTool.TxtFileProcess.CreateLog("产品设置页面-检测产品点位坐标数据失败，非法数据坐标为第4列，第" + (i + 1).ToString() + "行！" + "----用户：" + BaseForm._operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                    MessageBox.Show("检测产品点位坐标数据失败，非法数据坐标为第4列，第" + (i + 1).ToString() + "行！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                //检查检测类型
                try
                {
                    if (Convert.ToInt32(sourceDT.Rows[i][4]) < 0 || Convert.ToInt32(sourceDT.Rows[i][4]) > 1)
                    {
                        MyTool.TxtFileProcess.CreateLog("产品设置页面-检测产品点位坐标数据失败，数据超出合法范围（0-1），非法数据坐标为第5列，第" + (i + 1).ToString() + "行！" + "----用户：" + BaseForm._operatorName,
                        BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);
                        MessageBox.Show("检测产品点位坐标数据失败，数据超出合法范围（0-1），非法数据坐标为第5列，第" + (i + 1).ToString() + "行！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }
                catch
                {
                    MyTool.TxtFileProcess.CreateLog("产品设置页面-检测产品点位坐标数据失败，非法数据坐标为第5列，第" + (i + 1).ToString() + "行！" + "----用户：" + BaseForm._operatorName,
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
                    MyTool.TxtFileProcess.CreateLog("产品设置页面-检测产品点位名称失败，点位名称格式非法，非法数据坐标为第6列，第" + (i + 1).ToString() + "行！" + "----用户：" + BaseForm._operatorName,
                       BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);
                    MessageBox.Show("检测产品点位名称失败，点位名称格式非法，非法数据坐标为第6列，第" + (i + 1).ToString() + "行！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                //检查产品名称是否有相同项存在
                for (int j = i + 1; j < sourceDT.Rows.Count; j++)
                {
                    if (sourceDT.Rows[i][5].ToString() == sourceDT.Rows[j][5].ToString())//如果有名称相同项存在
                    {
                        MyTool.TxtFileProcess.CreateLog("产品设置页面-检测产品点位名称失败，有点位的名称相同，非法数据坐标为第6列，第" + (i + 1).ToString() + "行和第6列，第" + (j + 1).ToString() + "行！" + "----用户：" + BaseForm._operatorName,
                        BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);
                        MessageBox.Show("检测产品点位名称失败，有点位的名称相同，非法数据坐标为第6列，第" + (i + 1).ToString() + "行和第6列，第" + (j + 1).ToString() + "行！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }
            }

            return true;
        }

        //产品点位坐标表数据导出保存事件
        private void SaveOrExportPointPositionDataEvent(object sender, EventArgs e)
        {
            if (((Button)sender).Name == "btnSavePointPosition")//如果为保存按钮
            {
                //if (BaseForm._autoModeFlag)
                //{
                //    MessageBox.Show("处于自动模式，无法保存点位数据文件！");
                //    return;
                //}
                MyTool.TxtFileProcess.CreateLog("产品设置页面-保存产品点位数据！" + "----用户：" + BaseForm._operatorName,
                BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                if (SavePointPositionData(SettingsGeneralForm._productDirectory, _editProductName))
                {
                    MyTool.TxtFileProcess.CreateLog("产品设置页面-保存产品点位数据成功！" + "----用户：" + BaseForm._operatorName,
                BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                    MessageBox.Show("保存点位数据文件成功！");
                    //保存完参数到控件之后，读取到编辑产品变量中来
                    _editProductParameter.ProductDataLoadToVariate(SettingsGeneralForm._productDirectory + "\\" + _editProductName + ".rcp");
                }
                else
                {
                    MyTool.TxtFileProcess.CreateLog("产品设置页面-保存产品点位数据失败！" + "----用户：" + BaseForm._operatorName,
                BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                    MessageBox.Show("保存点位数据文件失败！");
                }
            }
            else if (((Button)sender).Name == "btnExportPointPosition")//如果为导出按钮
            {
                MyTool.TxtFileProcess.CreateLog("产品设置页面-导出产品点位数据！" + "----用户：" + BaseForm._operatorName,
                BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Title = "导出点位坐标数据文件";
                sfd.Filter = "csv(*.csv)|*.csv";
                sfd.FileName = MyTool.FolderAndFileManageClass.FilterPathIllegalChar(_editProductName + ".csv");
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    string fileName = sfd.FileName.Substring(sfd.FileName.LastIndexOf("\\") + 1);
                    string folderPath = sfd.FileName.Substring(0, sfd.FileName.LastIndexOf("\\"));
                    if (SavePointPositionData(folderPath, fileName))
                    {
                        MyTool.TxtFileProcess.CreateLog("产品设置页面-导出产品点位数据至" + sfd.FileName + "成功！" + "----用户：" + BaseForm._operatorName,
                        BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                        MessageBox.Show("导出点位数据文件成功！");
                    }
                    else
                    {
                        MyTool.TxtFileProcess.CreateLog("产品设置页面-导出产品点位数据至" + sfd.FileName + "失败！" + "----用户：" + BaseForm._operatorName,
                        BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                        MessageBox.Show("导出点位数据文件失败！");
                    }
                }
            }

        }

        //产品点位坐标数据导入读取事件
        private void LoadOrImportPointPositionDataEvent(object sender, EventArgs e)
        {
            if (((Button)sender).Name == "btnLoadPointPosition")//如果为读取按钮
            {
                MyTool.TxtFileProcess.CreateLog("产品设置页面-读取产品点位数据！" + "----用户：" + BaseForm._operatorName,
                BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                LoadPointPositionData(SettingsGeneralForm._productDirectory, _editProductName);
            }
            else if (((Button)sender).Name == "btnImportPointPosition")//如果为导入按钮
            {
                MyTool.TxtFileProcess.CreateLog("产品设置页面-导入产品点位数据！" + "----用户：" + BaseForm._operatorName,
                BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Title = "导入点位坐标数据文件";
                ofd.Filter = "csv(*.csv)|*.csv";
                ofd.FileName = MyTool.FolderAndFileManageClass.FilterPathIllegalChar(_editProductName + ".csv");
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    MyTool.TxtFileProcess.CreateLog("产品设置页面-导入产品点位数据文件" + ofd.FileName + "成功！" + "----用户：" + BaseForm._operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                    string fileName = ofd.FileName.Substring(ofd.FileName.LastIndexOf("\\") + 1);
                    string folderPath = ofd.FileName.Substring(0, ofd.FileName.LastIndexOf("\\"));
                    LoadPointPositionData(folderPath, fileName);
                }
            }
        }
        #endregion

        #endregion

        #region 界面参数保存读取相关函数

        //保存设置参数文件
        public void MainFormDataSavingSettingData()
        {
            MyTool.TxtFileProcess.CreateLog("产品设置页面-保存主界面参数！" + "----用户：" + BaseForm._operatorName,
                BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);


            string directoryOfExe = Directory.GetCurrentDirectory();
            string settingFileName = "Actuals.rcpActuals";
            string settingFilePath = directoryOfExe + "\\" + settingFileName;
            string[] tempValue = { "", "" ,""};
            string rootName = "clsActualsData";

            //获取要保存的参数值
            tempValue[0] = _track1ActualsProductName;
            tempValue[1] = _track2ActualsProductName;
            if (chkUseUseRelativePosition.Checked)
                tempValue[2] = "true";
            else
                tempValue[2] = "false";


            if (!File.Exists(settingFilePath))
            {
                XMLFileOperation obj = new XMLFileOperation();
                obj.CreateXmlRoot(rootName);//创建声明及根节点
                obj.XmlSave(settingFilePath);
                obj.Dispose();
                //修改根节点的属性
                XMLFileOperation.updateNodeValueOrAttribute(settingFilePath, rootName, "xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");
                XMLFileOperation.updateNodeValueOrAttribute(settingFilePath, rootName, "xmlns:xsd", "http://www.w3.org/2001/XMLSchema");

                for (int i = 0; i < _loadSaveDataName.Length; i++)
                {
                    XMLFileOperation.XmlInsertElement(settingFilePath, rootName, _loadSaveDataName[i], tempValue[i]);
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

                for (int i = 0; i < _loadSaveDataName.Length; i++)
                {
                    if (XMLFileOperation.checkNodeExistOrNot(settingFilePath, _loadSaveDataName[i]))
                    {
                        XMLFileOperation.XmlNodeReplace(settingFilePath, rootName + "/" + _loadSaveDataName[i], tempValue[i]);
                    }
                    else
                    {
                        XMLFileOperation.XmlInsertElement(settingFilePath, rootName, _loadSaveDataName[i], tempValue[i]);
                    }
                }
            }

            ParameterRefresh();
        }

        //读取数据
        public void MainFormDataLoadingSettingData()
        {
            MyTool.TxtFileProcess.CreateLog("产品设置页面-读取主界面参数！" + "----用户：" + BaseForm._operatorName,
            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

            string directoryOfExe = Directory.GetCurrentDirectory();
            string settingFileName = "Actuals.rcpActuals";
            string settingFilePath = directoryOfExe + "\\" + settingFileName;
            string[] tempValue = { "", "" ,""};
            string rootName = "clsActualsData";

            //获取要读取的参数值
            tempValue[0] = _track1ActualsProductName;
            tempValue[1] = _track2ActualsProductName;
            if (chkUseUseRelativePosition.Checked)
                tempValue[2] = "true";
            else
                tempValue[2] = "false";

            if (!File.Exists(settingFilePath))
            {
                MainFormDataSavingSettingData();
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
                for (int i = 0; i < _loadSaveDataName.Length; i++)
                {
                    if (XMLFileOperation.checkNodeExistOrNot(settingFilePath, _loadSaveDataName[i]))
                    {
                        tempValue[i] = XMLFileOperation.findElementValue(settingFilePath, rootName + "/" + _loadSaveDataName[i]);
                    }
                    else
                    {
                        XMLFileOperation.XmlInsertElement(settingFilePath, rootName, _loadSaveDataName[i], tempValue[i]);
                    }
                }

                _track1ActualsProductName = tempValue[0];
                _track2ActualsProductName = tempValue[1];
                tempValue[2] = tempValue[2].ToLower();
                if (tempValue[2] == "true")
                    chkUseUseRelativePosition.Checked = true;
                else
                    chkUseUseRelativePosition.Checked = false;

            }

            ParameterRefresh();
        }

        //基于控件状态更新参数变量
        private void ParameterRefresh()
        {
            if (_useRelativePos)
                MyTool.TxtFileProcess.CreateLog("产品设置页面-使用相对位置！" + "----用户：" + BaseForm._operatorName,
                BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);
            else
                MyTool.TxtFileProcess.CreateLog("产品设置页面-不使用相对位置！" + "----用户：" + BaseForm._operatorName,
                BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize); ;

            //显示当前产品名称
            lblActualProduct.Text = "传送线1产品: " + _track1ActualsProductName + "   传送线2产品：" + _track2ActualsProductName;
            _useRelativePos = chkUseUseRelativePosition.Checked;
        }

        #endregion

        //点击读取参数按钮事件
        private void btnLoadProductData_Click(object sender, EventArgs e)
        {
            MyTool.TxtFileProcess.CreateLog("产品设置页面-点击产品数据读取按钮！" + "----用户：" + BaseForm._operatorName,
            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

            if (CheckCurrentProductUsefulOrNot(ref _editProductName, true))
            {
                _editProductParameter._parameterLoadSuccessfulFlag = false;
                if (ProductDataLoadingToCtrl(_editProductName))
                {
                    MyTool.TxtFileProcess.CreateLog("产品设置页面-读取产品" + _editProductName + "数据成功！" + "----用户：" + BaseForm._operatorName,
             BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);
                    MessageBox.Show("读取数据成功！", "提示", MessageBoxButtons.OK);

                    //读取完参数到控件之后，读取到编辑产品变量中来
                    _editProductParameter.ProductDataLoadToVariate(SettingsGeneralForm._productDirectory + "\\" + _editProductName + ".rcp");
                }

            }
            else
            {
                MyTool.TxtFileProcess.CreateLog("产品设置页面-无法在指定文件夹中找到当前设定的产品" + _editProductName + "的数据，读取产品数据失败！" + "----用户：" + BaseForm._operatorName,
             BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                MessageBox.Show("无法在指定文件夹中找到当前设定的产品的数据，无法读取参数！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //点击保存参数按钮事件
        private void btnSaveProductData_Click(object sender, EventArgs e)
        {
            MyTool.TxtFileProcess.CreateLog("产品设置页面-点击产品数据保存按钮！" + "----用户：" + BaseForm._operatorName,
             BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

            //if ((_editProductName == _track1ActualsProductName || _editProductName == _track2ActualsProductName) && BaseForm._autoModeFlag)
            //{
            //    MessageBox.Show("当前设备处于自动模式，无法保存当前使用的产品的参数！");
            //    return;
            //}

            if (CheckCurrentProductUsefulOrNot(ref _editProductName, true))
            {
                _editProductParameter._parameterLoadSuccessfulFlag = false;
                if (ProductDataSavingFromCtrl(_editProductName, true))
                {
                    MyTool.TxtFileProcess.CreateLog("产品设置页面-产品" + _editProductName + "数据保存成功！" + "----用户：" + BaseForm._operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                    MessageBox.Show("保存数据成功！", "提示", MessageBoxButtons.OK);

                    //保存完参数到控件之后，读取到编辑产品变量中来
                    _editProductParameter.ProductDataLoadToVariate(SettingsGeneralForm._productDirectory + "\\" + _editProductName + ".rcp");
                }
                else
                {
                    MyTool.TxtFileProcess.CreateLog("产品设置页面-产品" + _editProductName + "数据保存失败！" + "----用户：" + BaseForm._operatorName,
                     BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                    MessageBox.Show("保存数据失败！", "提示", MessageBoxButtons.OK);
                }
            }
            else
            {
                MyTool.TxtFileProcess.CreateLog("产品设置页面-无法在指定文件夹中找到当前设定的产品的数据，产品" + _editProductName + "数据保存失败！" + "----用户：" + BaseForm._operatorName,
                      BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                MessageBox.Show("无法在指定文件夹中找到当前设定的产品的数据，无法保存参数！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #region 创建新的产品
        private void btnCreateNewProduct_Click(object sender, EventArgs e)
        {
            MyTool.TxtFileProcess.CreateLog("产品设置页面-创建新产品数据文件！" + "----用户：" + BaseForm._operatorName,
                     BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

            this.Invoke(new Action(() =>
                {
                    if (_createNewProductFormVariate != null)
                    {
                        _createNewProductFormVariate.Dispose();
                        _createNewProductFormVariate = null;
                    }
                    _createNewProductFormVariate = new CreateNewProductDataFileForm("创建新产品","产品名？","新产品");
                    _createNewProductFormVariate.Focus();
                    _createNewProductFormVariate.StartPosition = FormStartPosition.CenterParent;
                    _createNewProductFormVariate.SenderName += GetNewName;
                    _createNewProductFormVariate.CloseRequest += new CloseRequestDelegate(CloseRequest);
                    _createNewProductFormVariate.Show();
                })); ;
        }

        private void GetNewName(string arg)
        {
            string newProductName = arg.ToString();
            if (!CheckCurrentProductUsefulOrNot(ref newProductName, false))
            {
                if (ProductDataSavingFromCtrl(newProductName, false))
                {
                    MyTool.TxtFileProcess.CreateLog("产品设置页面-创建新产品" + newProductName + "数据文件成功！" + "----用户：" + BaseForm._operatorName,
                     BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                    MessageBox.Show("创建新产品完成！", "提示", MessageBoxButtons.OK);
                }
            }
            else
            {
                MyTool.TxtFileProcess.CreateLog("产品设置页面-创建新产品" + newProductName + "数据失败，当前产品路径中已存在此名称产品数据文件！" + "----用户：" + BaseForm._operatorName,
                BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

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
            MyTool.TxtFileProcess.CreateLog("产品设置页面-放弃创建新产品！" + "----用户：" + BaseForm._operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

            MessageBox.Show("放弃创建新的产品！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            //关闭窗口
            if (_createNewProductFormVariate != null)
            {
                _createNewProductFormVariate.Dispose();
                _createNewProductFormVariate = null;
            }
        }
        #endregion

        #region 参数文件另存为
        private void btnSaveProductAs_Click(object sender, EventArgs e)
        {
            MyTool.TxtFileProcess.CreateLog("产品设置页面-产品数据另存为！" + "----用户：" + BaseForm._operatorName,
                     BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

            this.Invoke(new Action(() =>
            {
                if (_createNewProductFormVariate != null)
                {
                    _createNewProductFormVariate.Dispose();
                    _createNewProductFormVariate = null;
                }
                _createNewProductFormVariate = new CreateNewProductDataFileForm("产品另存为","另存为名字？","新产品");
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
                    MyTool.TxtFileProcess.CreateLog("产品设置页面-产品" + _editProductName + "数据另存为" + saveAsProductName + "成功！" + "----用户：" + BaseForm._operatorName,
                  BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                    MessageBox.Show("产品另存为完成！", "提示", MessageBoxButtons.OK);
                }
                else
                {
                    MyTool.TxtFileProcess.CreateLog("产品设置页面-有非法数据存在，产品" + _editProductName + "数据另存为" + saveAsProductName + "失败！" + "----用户：" + BaseForm._operatorName,
                  BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                    MessageBox.Show("产品另存为失败！", "提示", MessageBoxButtons.OK);
                }
            }
            else
            {
                MyTool.TxtFileProcess.CreateLog("产品设置页面-产品" + _editProductName + "数据另存为" + saveAsProductName + "失败，产品" + saveAsProductName + "已存在！" + "----用户：" + BaseForm._operatorName,
                  BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                MessageBox.Show("产品" + saveAsProductName + "已经存在！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            //关闭窗口
            if (_createNewProductFormVariate != null)
            {
                _createNewProductFormVariate.Dispose();
                _createNewProductFormVariate = null;
            }

            //轴点位数据另存为
            string axisParAndPositionFileFolder = Directory.GetCurrentDirectory() + "\\" + "AxisParameterAndPositionData_" + saveAsProductName;
            string oldAxisParAndPositionFileFolder = Directory.GetCurrentDirectory() + "\\" + "AxisParameterAndPositionData_" + _editProductName;

            if (!Directory.Exists(axisParAndPositionFileFolder))
            {
                Directory.CreateDirectory(axisParAndPositionFileFolder);
            }

            try
            {
                for (int i = 0; i < AxisControlMainForm._axisQuantity; i++)
                {
                    File.Copy(oldAxisParAndPositionFileFolder + "\\" + "axis" + i.ToString("00") + "Pos_" + _editProductName + ".csv"
                        , axisParAndPositionFileFolder + "\\" + "axis" + i.ToString("00") + "Pos_" + saveAsProductName + ".csv");
                }
                File.Copy(oldAxisParAndPositionFileFolder + "\\" + "AxisPar.csv", axisParAndPositionFileFolder + "\\" + "AxisPar.csv");
            }
            catch(Exception e)
            {
                MessageBox.Show(e.ToString());
            }
            _editProductName = saveAsProductName;
            //基于EDIT product name设置combo box
            cboAvaliableProduct_MouseClick(new object(), new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0));
            cboAvaliableProduct.SelectedIndex = cboAvaliableProduct.FindString(_editProductName);
        }

        private void ProductSaveAsCloseRequest()
        {
            MyTool.TxtFileProcess.CreateLog("产品设置页面-放弃产品" + _editProductName + "数据的另存为！" + "----用户：" + BaseForm._operatorName,
                 BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

            MessageBox.Show("放弃产品另存为！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            //关闭窗口
            if (_createNewProductFormVariate != null)
            {
                _createNewProductFormVariate.Dispose();
                _createNewProductFormVariate = null;
            }
        }
        #endregion

        #region combo box控件相关事件

        public void cboAvaliableProduct_MouseClick(object sender, MouseEventArgs e)
        {
            string[] fileName;
            if (Directory.Exists(SettingsGeneralForm._productDirectory))
            {
                fileName = FolderAndFileManageClass.ScanFolderAndGetAssignedFormatFileName(SettingsGeneralForm._productDirectory, ".rcp");

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
                _editProductParameter._parameterLoadSuccessfulFlag = false;
                if (ProductDataLoadingToCtrl(_editProductName))
                    _editProductParameter.ProductDataLoadToVariate(SettingsGeneralForm._productDirectory + "\\" + _editProductName + ".rcp");

                MyTool.TxtFileProcess.CreateLog("产品设置页面-切换当前编辑产品为" + _editProductName + "----用户：" + BaseForm._operatorName,
                 BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);
                if (this.Visible)
                {
                    MessageBox.Show("切换编辑产品成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                if (!BaseForm._autoModeFlag)
                    AxisControlMainForm._currentProductName = _editProductName;
            }
        }
        #endregion
        
        #region 虚拟键盘相关函数

        #region TextBox双击事件
        //textbox 鼠标双击事件，打开主键盘
        private void TextBoxDoubleClickEventToOpenMainKeyboard(object sender, EventArgs e)
        {
            MyTool.TxtFileProcess.CreateLog("产品设置页面-打开虚拟主键盘!" + "----用户：" + BaseForm._operatorName,
                 BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

            SoftMainKeyboard.LanguageFlag = 1;
            SoftMainKeyboard mainKeyboard = new SoftMainKeyboard();
            mainKeyboard.StartPosition = FormStartPosition.CenterScreen;
            mainKeyboard.SOURCESTR = ((TextBox)sender).Text;
            mainKeyboard.SENDER = sender;
            mainKeyboard.SendInputDataIncludeObj += new SENDINPUTDATAINCLUDEOBJ(GetKeyboardInputData);//添加事件
            mainKeyboard.ShowDialog();
        }

        //双击按钮，打开数字软键盘，数据无正负限制，浮点数
        private void TextBoxDoubleClickEventToOpenNumberKeyboard(object sender, EventArgs e)
        {
            MyTool.TxtFileProcess.CreateLog("产品设置页面-打开虚拟数字键盘，包含正负号及小数点!" + "----用户：" + BaseForm._operatorName,
                 BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

            SoftNumberKeyboard.LanguageFlag = 1;
            SoftNumberKeyboard numberKeyboard = new SoftNumberKeyboard();
            numberKeyboard.StartPosition = FormStartPosition.CenterScreen;
            numberKeyboard.PLUSMINUSBTHIDEFLAG = false;
            numberKeyboard.FULLSTOPBTHIDEFLAG = false;
            numberKeyboard.SOURCESTR = ((TextBox)sender).Text;
            numberKeyboard.SENDER = sender;
            numberKeyboard.SendInputDataIncludeObj += new SENDINPUTDATAINCLUDEOBJ(GetKeyboardInputData);//添加事件
            numberKeyboard.ShowDialog();
        }

        //双击按钮，打开数字软键盘，数据有正限制，浮点数，和整数
        private void TextBoxDoubleClickEventToOpenNumberKeyboardForPlusData(object sender, EventArgs e)
        {
            MyTool.TxtFileProcess.CreateLog("产品设置页面-打开虚拟数字键盘，不包含负号，包含小数点!" + "----用户：" + BaseForm._operatorName,
                BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

            SoftNumberKeyboard.LanguageFlag = 1;
            SoftNumberKeyboard numberKeyboard = new SoftNumberKeyboard();
            numberKeyboard.StartPosition = FormStartPosition.CenterScreen;

            numberKeyboard.PLUSMINUSBTHIDEFLAG = true;
            numberKeyboard.FULLSTOPBTHIDEFLAG = false;
            numberKeyboard.SOURCESTR = ((TextBox)sender).Text;
            numberKeyboard.SENDER = sender;
            numberKeyboard.SendInputDataIncludeObj += new SENDINPUTDATAINCLUDEOBJ(GetKeyboardInputData);//添加事件
            numberKeyboard.ShowDialog();
        }

        //双击按钮，打开数字软键盘，数据为正数，整数
        private void TextBoxDoubleClickEventToOpenNumberKeyboardForPlusIntData(object sender, EventArgs e)
        {
            MyTool.TxtFileProcess.CreateLog("产品设置页面-打开虚拟数字键盘，不包含负号，不包含小数点!" + "----用户：" + BaseForm._operatorName,
                BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

            SoftNumberKeyboard.LanguageFlag = 1;
            SoftNumberKeyboard numberKeyboard = new SoftNumberKeyboard();
            numberKeyboard.StartPosition = FormStartPosition.CenterScreen;

            numberKeyboard.PLUSMINUSBTHIDEFLAG = true;
            numberKeyboard.FULLSTOPBTHIDEFLAG = true;
            numberKeyboard.SOURCESTR = ((TextBox)sender).Text;
            numberKeyboard.SENDER = sender;
            numberKeyboard.SendInputDataIncludeObj += new SENDINPUTDATAINCLUDEOBJ(GetKeyboardInputData);//添加事件
            numberKeyboard.ShowDialog();
        }


        //双击按钮，打开DataGridView控件所用软键盘
        private void DataGridViewDoubleClickEvent(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex >= 1 && e.ColumnIndex <= 4 && e.RowIndex >= 0 && ((DataGridView)sender).IsCurrentCellInEditMode)
            {
                MyTool.TxtFileProcess.CreateLog("产品设置页面-打开Datagridview所用虚拟数字键盘！" + "----用户：" + BaseForm._operatorName,
                BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                SoftNumberKeyboard.LanguageFlag = 1;//设置语言为中文
                SoftNumberKeyboard numberKeyboard = new SoftNumberKeyboard();
                numberKeyboard.StartPosition = FormStartPosition.CenterScreen;

                if (e.ColumnIndex == 4)//如果为序号为3的列（检测类型）
                {
                    numberKeyboard.PLUSMINUSBTHIDEFLAG = true;
                    numberKeyboard.FULLSTOPBTHIDEFLAG = true;
                }
                else
                {
                    numberKeyboard.PLUSMINUSBTHIDEFLAG = false;
                    numberKeyboard.FULLSTOPBTHIDEFLAG = false;
                }
                numberKeyboard.SOURCESTR = ((DataGridView)sender).CurrentCell.Value.ToString();
                numberKeyboard.SENDER = sender;
                numberKeyboard.SendInputDataIncludeObj += new SENDINPUTDATAINCLUDEOBJ(DataGridViewGetKeyboardInputData);//添加事件
                numberKeyboard.Show();
            }
            else if (e.ColumnIndex > 4 && e.RowIndex >= 0 && ((DataGridView)sender).IsCurrentCellInEditMode)
            {
                MyTool.TxtFileProcess.CreateLog("产品设置页面-打开Datagridview所用虚拟主键盘！" + "----用户：" + BaseForm._operatorName,
                BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                SoftMainKeyboard.LanguageFlag = 1;
                SoftMainKeyboard mainKeyboard = new SoftMainKeyboard();
                mainKeyboard.StartPosition = FormStartPosition.CenterScreen;
                mainKeyboard.SOURCESTR = ((DataGridView)sender).CurrentCell.Value.ToString();
                mainKeyboard.SENDER = sender;
                mainKeyboard.SendInputDataIncludeObj += new SENDINPUTDATAINCLUDEOBJ(DataGridViewGetKeyboardInputData);//添加事件
                mainKeyboard.ShowDialog();
            }
        }

        #endregion

        //键盘按下OK之后获取数据
        private void GetKeyboardInputData(object Sender, string GetDataStr)
        {
            ((TextBox)Sender).Text = GetDataStr;//将键盘按钮输入的数据传递给密码输入框
            ((TextBox)Sender).Focus();
            ((TextBox)Sender).SelectionLength = 0;
        }

        //键盘按下之后获取数据
        private void DataGridViewGetKeyboardInputData(object Sender, string GetDataStr)
        {
            ((DataGridView)Sender).CurrentCell.Value = GetDataStr;
            ((DataGridView)Sender).RefreshEdit();//不加上这句话的话，那么当currentcell的值发生变化之后，edit的值不会变化
        }

        #endregion

        #region 数据检查事件
        //Textbox数据普通检查函数，无正负号，浮点数检查
        private void TextBoxDataFloatCheckEvent(object sender, EventArgs e)
        {
            ((TextBox)sender).Text = MyTool.DataProcessing.TextBoxDataCheck(((TextBox)sender).Text, -9999999, 9999999, true, true, true, false);
            ((TextBox)sender).SelectionStart = ((TextBox)sender).TextLength;
            ((TextBox)sender).SelectionLength = 0;

        }

        //Textbox数据最后一次检查函数，无正负号，浮点数检查
        private void TextBoxDataFloatCheckLeaveEvent(object sender, EventArgs e)
        {
            ((TextBox)sender).Text = MyTool.DataProcessing.TextBoxDataCheck(((TextBox)sender).Text, -9999999, 9999999, true, true, true, true);
            ((TextBox)sender).SelectionStart = ((TextBox)sender).TextLength;
            ((TextBox)sender).SelectionLength = 0;
            MyTool.TxtFileProcess.CreateLog("产品设置页面-修改" + ((TextBox)sender).Name.Substring(3) + "数据为" + ((TextBox)sender).Text + "----用户：" + BaseForm._operatorName,
                BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);
        }

        //Textbox数据普通检查函数，正数，浮点数，整数检查
        private void TextBoxDataPlusFloatOrIntCheckEvent(object sender, EventArgs e)
        {
            ((TextBox)sender).Text = MyTool.DataProcessing.TextBoxDataCheck(((TextBox)sender).Text, 0, 9999999, true, true, true, false);
            ((TextBox)sender).SelectionStart = ((TextBox)sender).TextLength;
            ((TextBox)sender).SelectionLength = 0;
        }

        //Textbox数据最后一次检查函数，正数，浮点数，整数检查
        private void TextBoxDataPlusFloatOrIntCheckLeaveEvent(object sender, EventArgs e)
        {
            ((TextBox)sender).Text = MyTool.DataProcessing.TextBoxDataCheck(((TextBox)sender).Text, 0, 9999999, true, true, true, true);
            ((TextBox)sender).SelectionStart = ((TextBox)sender).TextLength;
            ((TextBox)sender).SelectionLength = 0;
            MyTool.TxtFileProcess.CreateLog("产品设置页面-修改" + ((TextBox)sender).Name.Substring(3) + "数据为" + ((TextBox)sender).Text + "----用户：" + BaseForm._operatorName,
                BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);
        }

        //Textbox Mark点识别数据检查函数
        private void TextBoxMarkSearchDataCheckEvent(object sender, EventArgs e)
        {
            //除去二值化参数为0-255整数以外，其余的都为浮点数正数
            if (((TextBox)sender).Name == "txtMarkSearchThresholdMin" || ((TextBox)sender).Name == "txtMarkSearchThresholdMax")
            {
                ((TextBox)sender).Text = MyTool.DataProcessing.TextBoxDataCheck(((TextBox)sender).Text, 0, 255, true, true, false, false);
            }
            else if (((TextBox)sender).Name == "txtMarkSearchMinCircularity")
            {
                ((TextBox)sender).Text = MyTool.DataProcessing.TextBoxDataCheck(((TextBox)sender).Text, 0, 1, true, true, true, false);
            }
            else
            {
                ((TextBox)sender).Text = MyTool.DataProcessing.TextBoxDataCheck(((TextBox)sender).Text, 0, 9999999, true, true, true, false);
            }

            ((TextBox)sender).SelectionStart = ((TextBox)sender).TextLength;
            ((TextBox)sender).SelectionLength = 0;
        }

        //Textbox Mark点识别数据最后一次检查函数
        private void TextBoxMarkSearchDataCheckLeaveEvent(object sender, EventArgs e)
        {
            HObject ho_retrunRegion = null;
            float rowCoor, columnCoor;
            //除去二值化参数为0-255整数以外，其余的都为浮点数正数
            if (((TextBox)sender).Name == "txtMarkSearchThresholdMin" || ((TextBox)sender).Name == "txtMarkSearchThresholdMax")
            {
                ((TextBox)sender).Text = MyTool.DataProcessing.TextBoxDataCheck(((TextBox)sender).Text, 0, 255, true, true, false, true);
            }
            else if (((TextBox)sender).Name == "txtMarkSearchMinCircularity")
            {
                ((TextBox)sender).Text = MyTool.DataProcessing.TextBoxDataCheck(((TextBox)sender).Text, 0, 1, true, true, true, true);
            }
            else
            {
                ((TextBox)sender).Text = MyTool.DataProcessing.TextBoxDataCheck(((TextBox)sender).Text, 0, 9999999, true, true, true, true);
            }
            ((TextBox)sender).SelectionStart = ((TextBox)sender).TextLength;
            ((TextBox)sender).SelectionLength = 0;

            if (ho_backCameraImage != null && !BaseForm._autoModeFlag)//如果图像object并不为空
            {
                if (((TextBox)sender).Name == "txtMarkSearchThresholdMin" || ((TextBox)sender).Name == "txtMarkSearchThresholdMax")
                {
                    if (HalconPictureProcess.BlobGetCircleCenterPosition(ref ho_backCameraImage, Convert.ToInt32(txtMarkSearchThresholdMin.Text), Convert.ToInt32(txtMarkSearchThresholdMax.Text),
                          0, 0, 0, 0, out rowCoor, out columnCoor, out ho_retrunRegion) == 0)
                    {
                        try
                        {
                            //显示设置
                            HOperatorSet.SetColor(picBackCameraImageDisp.HalconWindow, "green");
                            HOperatorSet.SetDraw(picBackCameraImageDisp.HalconWindow, "fill");
                            HOperatorSet.SetLineWidth(picBackCameraImageDisp.HalconWindow, 3);
                            HOperatorSet.DispObj(ho_backCameraImage, picBackCameraImageDisp.HalconWindow);
                            HOperatorSet.DispObj(ho_retrunRegion, picBackCameraImageDisp.HalconWindow);
                        }
                        catch
                        {
                            MessageBox.Show("图像二值化失败！");
                        }
                    }
                    else
                    {
                        MessageBox.Show("图像二值化失败！");
                    }
                }
                else if (((TextBox)sender).Name == "txtMarkSelectShapeMin" || ((TextBox)sender).Name == "txtMarkSelectShapeMax")
                {
                    if (HalconPictureProcess.BlobGetCircleCenterPosition(ref ho_backCameraImage, Convert.ToInt32(txtMarkSearchThresholdMin.Text), Convert.ToInt32(txtMarkSearchThresholdMax.Text),
                        Convert.ToSingle(txtMarkSelectShapeMin.Text), Convert.ToSingle(txtMarkSelectShapeMax.Text), 0, 1, out rowCoor, out columnCoor, out ho_retrunRegion) == 0)
                    {
                        try
                        {
                            //显示设置
                            HOperatorSet.SetColor(picBackCameraImageDisp.HalconWindow, "green");
                            HOperatorSet.SetDraw(picBackCameraImageDisp.HalconWindow, "fill");
                            HOperatorSet.SetLineWidth(picBackCameraImageDisp.HalconWindow, 3);
                            HOperatorSet.DispObj(ho_backCameraImage, picBackCameraImageDisp.HalconWindow);
                            HOperatorSet.DispObj(ho_retrunRegion, picBackCameraImageDisp.HalconWindow);
                        }
                        catch
                        {
                            MessageBox.Show("图像特征选择失败！");
                        }
                    }
                    else
                    {
                        MessageBox.Show("图像特征选择失败！");
                    }
                }
                else if (((TextBox)sender).Name == "txtMarkSearchMinCircularity")
                {
                    if (HalconPictureProcess.BlobGetCircleCenterPosition(ref ho_backCameraImage, Convert.ToInt32(txtMarkSearchThresholdMin.Text), Convert.ToInt32(txtMarkSearchThresholdMax.Text),
                        Convert.ToSingle(txtMarkSelectShapeMin.Text), Convert.ToSingle(txtMarkSelectShapeMax.Text), Convert.ToSingle(txtMarkSearchMinCircularity.Text), 2, out rowCoor, out columnCoor, out ho_retrunRegion) == 0)
                    {
                        try
                        {
                            HOperatorSet.SetColor(picBackCameraImageDisp.HalconWindow, "green");
                            HOperatorSet.SetDraw(picBackCameraImageDisp.HalconWindow, "margin");
                            HOperatorSet.SetLineWidth(picBackCameraImageDisp.HalconWindow, 3);
                            HOperatorSet.DispObj(ho_backCameraImage, picBackCameraImageDisp.HalconWindow);
                            HOperatorSet.DispObj(ho_retrunRegion, picBackCameraImageDisp.HalconWindow);
                            HOperatorSet.DispCross(picBackCameraImageDisp.HalconWindow, rowCoor, columnCoor, 100, 0);
                            HOperatorSet.DispText(picBackCameraImageDisp.HalconWindow, "Mark点坐标(" + rowCoor.ToString("f2") + ","
                                + columnCoor.ToString("f2") + ")", "image", 0, 0, "green", new HTuple(), new HTuple());
                        }
                        catch
                        {
                            MessageBox.Show("图像特征选择失败！");
                        }
                    }
                    else
                    {
                        MessageBox.Show("图像特征选择失败！");
                    }
                }
            }

            if(ho_retrunRegion != null)
            {
                ho_retrunRegion.Dispose();
            }
        }

        //AOI inspect Textbox数据普通检查函数，正整数检测
        private void TextBoxDataPlusIntCheckEvent(object sender, EventArgs e)
        {
            ((TextBox)sender).Text = MyTool.DataProcessing.TextBoxDataCheck(((TextBox)sender).Text, 0, 9999999, true, true, false, false);
            ((TextBox)sender).SelectionStart = ((TextBox)sender).TextLength;
            ((TextBox)sender).SelectionLength = 0;
        }

        //AOI inspect Textbox数据最后一次检查函数，正整数检测
        private void TextBoxDataPlusIntCheckLeaveEvent(object sender, EventArgs e)
        {
            ((TextBox)sender).Text = MyTool.DataProcessing.TextBoxDataCheck(((TextBox)sender).Text, 0, 9999999, true, true, false, true);
            ((TextBox)sender).SelectionStart = ((TextBox)sender).TextLength;
            ((TextBox)sender).SelectionLength = 0;
            MyTool.TxtFileProcess.CreateLog("产品设置页面-修改" + ((TextBox)sender).Name.Substring(3) + "数据为" + ((TextBox)sender).Text + "----用户：" + BaseForm._operatorName,
                BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);
        }

        //Datagridview cell数据检查函数
        private void DataGridViewDataCheckEvent(object sender, DataGridViewCellEventArgs e)
        {
            if (((DataGridViewCellEventArgs)e).RowIndex >= 0 && ((DataGridViewCellEventArgs)e).ColumnIndex >= 1 && ((DataGridViewCellEventArgs)e).ColumnIndex <= 4)
            {
                switch (((DataGridViewCellEventArgs)e).ColumnIndex)
                {
                    case 1://为XYR轴坐标
                    case 2:
                    case 3:
                        {
                            ((DataGridView)sender).Rows[((DataGridViewCellEventArgs)e).RowIndex].Cells[((DataGridViewCellEventArgs)e).ColumnIndex].Value =
                                MyTool.DataProcessing.TextBoxDataCheck(((DataGridView)sender).Rows[((DataGridViewCellEventArgs)e).RowIndex].Cells[((DataGridViewCellEventArgs)e).ColumnIndex].Value.ToString(),
                                -9999999, 9999999, true, true, true, false);
                            break;
                        }
                    case 4://为检测类型
                        {
                            ((DataGridView)sender).Rows[((DataGridViewCellEventArgs)e).RowIndex].Cells[((DataGridViewCellEventArgs)e).ColumnIndex].Value =
                                MyTool.DataProcessing.TextBoxDataCheck(((DataGridView)sender).Rows[((DataGridViewCellEventArgs)e).RowIndex].Cells[((DataGridViewCellEventArgs)e).ColumnIndex].Value.ToString(),
                                0, 1, true, true, false, false);
                            break;
                        }
                }
            }
        }

        //Datagridview cell数据检查函数
        private void DataGridViewLastDataCheckEvent(object sender, DataGridViewCellEventArgs e)
        {
            if (((DataGridViewCellEventArgs)e).RowIndex >= 0 && ((DataGridViewCellEventArgs)e).ColumnIndex >= 1 && ((DataGridViewCellEventArgs)e).ColumnIndex <= 4)
            {
                switch (((DataGridViewCellEventArgs)e).ColumnIndex)
                {
                    case 1://为XY轴坐标
                    case 2:
                    case 3:
                        {
                            ((DataGridView)sender).Rows[((DataGridViewCellEventArgs)e).RowIndex].Cells[((DataGridViewCellEventArgs)e).ColumnIndex].Value =
                                MyTool.DataProcessing.TextBoxDataCheck(((DataGridView)sender).Rows[((DataGridViewCellEventArgs)e).RowIndex].Cells[((DataGridViewCellEventArgs)e).ColumnIndex].Value.ToString(),
                                -9999999, 9999999, true, true, true, true);
                            break;
                        }
                    case 4://为检测类型
                        {
                            ((DataGridView)sender).Rows[((DataGridViewCellEventArgs)e).RowIndex].Cells[((DataGridViewCellEventArgs)e).ColumnIndex].Value =
                                MyTool.DataProcessing.TextBoxDataCheck(((DataGridView)sender).Rows[((DataGridViewCellEventArgs)e).RowIndex].Cells[((DataGridViewCellEventArgs)e).ColumnIndex].Value.ToString(),
                                0, 1, true, true, false, true);
                            break;
                        }
                }
            }
        }
        #endregion

        //示教按钮点击事件
        private void TeachBtEvent(object sender, EventArgs e)
        {
            switch (((Button)sender).Name)
            {
                case "btnTrack1PointPositionTeach":
                    

                    txtTrack1TeachedPointNo.Text = txtCurrentSelectedPointNo.Text;
                    if (_useRelativePos)//如果使用相对位置
                    {
                        txtTrack1X1TeachedCoor.Text = BaseForm._axisRealPosition[0].ToString("f2");
                        txtTrack1Y1TeachedCoor.Text = BaseForm._axisRealPosition[1].ToString("f2");
                        txtTrack1R1TeachedCoor.Text = BaseForm._axisRealPosition[2].ToString("f2");
                        
                    }
                    else
                    {
                        txtTrack1X1TeachedCoor.Text = BaseForm._axisAbsolutePosition[0].ToString("f2");
                        txtTrack1Y1TeachedCoor.Text = BaseForm._axisAbsolutePosition[1].ToString("f2");
                        txtTrack1R1TeachedCoor.Text = BaseForm._axisAbsolutePosition[2].ToString("f2");
                    }
                    MyTool.TxtFileProcess.CreateLog("产品设置页面-点击传送线1点位示教按钮，修改第" + txtCurrentSelectedPointNo.Text + "点位的数据为" +
                        "X1:" + txtTrack1X1TeachedCoor.Text + "Y1:" + txtTrack1Y1TeachedCoor.Text + "R1:" + txtTrack1R1TeachedCoor.Text + "----用户：" + BaseForm._operatorName,
                        BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);
                    break;
                case "btnTrack2PointPositionTeach":
                    txtTrack2TeachedPointNo.Text = txtCurrentSelectedPointNo.Text;
                    if (_useRelativePos)//如果使用相对位置
                    {
                        txtTrack2X1TeachedCoor.Text = BaseForm._axisRealPosition[0].ToString("f2");
                        txtTrack2Y1TeachedCoor.Text = BaseForm._axisRealPosition[1].ToString("f2");
                        txtTrack2R1TeachedCoor.Text = BaseForm._axisRealPosition[2].ToString("f2");
                    }
                    else
                    {
                        txtTrack2X1TeachedCoor.Text = BaseForm._axisAbsolutePosition[0].ToString("f2");
                        txtTrack2Y1TeachedCoor.Text = BaseForm._axisAbsolutePosition[1].ToString("f2");
                        txtTrack2R1TeachedCoor.Text = BaseForm._axisAbsolutePosition[2].ToString("f2");
                    }
                    MyTool.TxtFileProcess.CreateLog("产品设置页面-点击传送线2点位示教按钮，修改第" + txtCurrentSelectedPointNo.Text + "点位的数据为" +
                        "X1:" + txtTrack2X1TeachedCoor.Text + "Y1:" + txtTrack2Y1TeachedCoor.Text + "R1:" + txtTrack2R1TeachedCoor.Text + "----用户：" + BaseForm._operatorName,
                        BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);
                    break;
            }
        }

        //选择使用相对位置checkbox发生变化事件
        private void chkUseUseRelativePosition_CheckedChanged(object sender, EventArgs e)
        {
            MainFormDataSavingSettingData();
        }

        #region DataGridViewSelectTeachPointEvent:datagridview鼠标单击事件，用于指定作为基准的点位号
        private void DataGridViewSelectTeachPointEvent(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                txtCurrentSelectedPointNo.Text = (e.RowIndex + 1).ToString();
            }
        }
        #endregion

        //深度学习文件浏览按钮点击事件
        private void DeepLearningFileBrowseBtEvent(object sender, EventArgs e)
        {
            string objectName = ((Button)sender).Name;
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "浏览获取深度学习文件路径";
            if (objectName == "btnPreprocessParameterFilePathBrowse" || objectName == "btnDimensionFilePathBrowse")
            {
                ofd.Filter = "*.hdict(*.hdict)|*.hdict";
            }
            else if (objectName == "btnModelFilePathBrowse")
            {
                ofd.Filter = "*.hdl(*.hdl)|*.hdl";
            }
            ofd.RestoreDirectory = true;

            if (ofd.ShowDialog() == DialogResult.OK)//如果点击确定按钮
            {
                switch (objectName)
                {
                    case "btnPreprocessParameterFilePathBrowse":
                        txtPreprocessParameterFilePath.Text = ofd.FileName;

                        MyTool.TxtFileProcess.CreateLog("产品设置页面-修改深度学习预处理参数文件路径为" + txtPreprocessParameterFilePath.Text + "----用户：" + BaseForm._operatorName,
                        BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);
                        break;
                    case "btnDimensionFilePathBrowse":
                        txtDimensionFilePath.Text = ofd.FileName;

                        MyTool.TxtFileProcess.CreateLog("产品设置页面-修改深度学习标注文件路径为" + txtPreprocessParameterFilePath.Text + "----用户：" + BaseForm._operatorName,
                        BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);
                        break;
                    case "btnModelFilePathBrowse":
                        txtModelFilePath.Text = ofd.FileName;

                        MyTool.TxtFileProcess.CreateLog("产品设置页面-修改深度学习模型文件路径为" + txtPreprocessParameterFilePath.Text + "----用户：" + BaseForm._operatorName,
                        BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);
                        break;
                    default:
                        return;
                }
            }
        }

        #region Mark点识别相关函数
        
        //点击图像识别按钮
        private void btnGetImage_Click(object sender, EventArgs e)
        {
            MyTool.TxtFileProcess.CreateLog("产品参数设定画面-请求相机抓取并显示图像" + "----用户：" + BaseForm._operatorName,
            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

            if (BaseForm._autoModeFlag)
            {
                MyTool.TxtFileProcess.CreateLog("产品参数设定画面-请求相机抓取并显示图像失败，当前设备处于自动模式" + "----用户：" + BaseForm._operatorName,
                BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                MessageBox.Show("当前设备处于自动模式，无法进行手动控制！");
                return;
            }

            ho_backCameraImage = null;
            if (SettingsProductFormRequestGrabImageAndDispEvent != null)
            {
                SettingsProductFormRequestGrabImageAndDispEvent(2, false);
            }
        }

        //mark点识别事件函数
        private void MarkSearchEvent(object sender, EventArgs e)
        {
            HObject ho_retrunRegion = null;
            float rowCoor, columnCoor;
            bool searchResult = false;
            string searchResultStr = "失败";

            switch(((Button)sender).Name)
            {
                case "btnSearchConveyor1Mark1Position":
                {
                    MyTool.TxtFileProcess.CreateLog("产品参数设定画面-传送线1的Mark点1识别" + "----用户：" + BaseForm._operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                    break;
                }
                case "btnSearchConveyor1Mark2Position":
                {
                    MyTool.TxtFileProcess.CreateLog("产品参数设定画面-传送线1的Mark点2识别" + "----用户：" + BaseForm._operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                    break;
                }
                case "btnSearchConveyor2Mark1Position":
                {
                    MyTool.TxtFileProcess.CreateLog("产品参数设定画面-传送线2的Mark点1识别" + "----用户：" + BaseForm._operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                    break;
                }
                case "btnSearchConveyor2Mark2Position":
                {
                    MyTool.TxtFileProcess.CreateLog("产品参数设定画面-传送线2的Mark点2识别" + "----用户：" + BaseForm._operatorName,
                    BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                    break;
                }
                default:
                return;
            }

            if (ho_backCameraImage==null)
            {
                MyTool.TxtFileProcess.CreateLog("产品参数设定画面-识别Mark点失败，未采集有效图像" + "----用户：" + BaseForm._operatorName,
                BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                MessageBox.Show("请采集图像后识别！");
                return;
            }

            if (BaseForm._autoModeFlag)
            {
                MyTool.TxtFileProcess.CreateLog("产品参数设定画面-识别Mark点失败，当前设备处于自动模式" + "----用户：" + BaseForm._operatorName,
                BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                MessageBox.Show("当前设备处于自动模式，无法进行手动Mark点识别控制！");
                return;
            }


            if (HalconPictureProcess.BlobGetCircleCenterPosition(ref ho_backCameraImage, Convert.ToInt32(txtMarkSearchThresholdMin.Text), Convert.ToInt32(txtMarkSearchThresholdMax.Text),
                Convert.ToSingle(txtMarkSelectShapeMin.Text), Convert.ToSingle(txtMarkSelectShapeMax.Text), Convert.ToSingle(txtMarkSearchMinCircularity.Text), 2, out rowCoor, out columnCoor, out ho_retrunRegion) == 0)
            {
                try
                {
                    //显示设置
                    HOperatorSet.SetColor(picBackCameraImageDisp.HalconWindow, "green");
                    HOperatorSet.SetDraw(picBackCameraImageDisp.HalconWindow, "margin");
                    HOperatorSet.SetLineWidth(picBackCameraImageDisp.HalconWindow, 3);
                    HOperatorSet.DispObj(ho_backCameraImage, picBackCameraImageDisp.HalconWindow);
                    HOperatorSet.DispObj(ho_retrunRegion, picBackCameraImageDisp.HalconWindow);
                    HOperatorSet.DispCross(picBackCameraImageDisp.HalconWindow, rowCoor, columnCoor, 100, 0);
                    HOperatorSet.DispText(picBackCameraImageDisp.HalconWindow, "Mark点坐标(" + rowCoor.ToString("f2") + "," 
                        + columnCoor.ToString("f2") + ")", "image", 0, 0, "green", new HTuple(), new HTuple());
                    searchResult = true;
                    searchResultStr ="成功！";
                }
                catch
                {
                    searchResult = false;
                    searchResultStr ="失败！";
                    MessageBox.Show("Mark点识别失败！");
                }
            }
            else
            {
                searchResult = false;
                searchResultStr ="失败！";
                MessageBox.Show("Mark点识别失败！");
            }

            switch (((Button)sender).Name)
            {
                case "btnSearchConveyor1Mark1Position":
                    {
                        MyTool.TxtFileProcess.CreateLog("产品参数设定画面-传送线1的Mark点1识别" + searchResultStr + "----用户：" + BaseForm._operatorName,
                        BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);
                        if (searchResult)//如果成功
                        {
                            txtConveyor1Mark1SearchRowResult.Text = rowCoor.ToString("f2");
                            txtConveyor1Mark1SearchColumnResult.Text = columnCoor.ToString("f2");
                        }
                        break;
                    }
                case "btnSearchConveyor1Mark2Position":
                    {
                        MyTool.TxtFileProcess.CreateLog("产品参数设定画面-传送线1的Mark点2识别" + searchResultStr + "----用户：" + BaseForm._operatorName,
                        BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);
                        if (searchResult)//如果成功
                        {
                            txtConveyor1Mark2SearchRowResult.Text = rowCoor.ToString("f2");
                            txtConveyor1Mark2SearchColumnResult.Text = columnCoor.ToString("f2");
                        }
                        break;
                    }
                case "btnSearchConveyor2Mark1Position":
                    {
                        MyTool.TxtFileProcess.CreateLog("产品参数设定画面-传送线2的Mark点1识别" + searchResultStr + "----用户：" + BaseForm._operatorName,
                        BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);
                        if (searchResult)//如果成功
                        {
                            txtConveyor2Mark1SearchRowResult.Text = rowCoor.ToString("f2");
                            txtConveyor2Mark1SearchColumnResult.Text = columnCoor.ToString("f2");
                        }
                        break;
                    }
                case "btnSearchConveyor2Mark2Position":
                    {
                        MyTool.TxtFileProcess.CreateLog("产品参数设定画面-传送线2的Mark点2识别" + searchResultStr + "----用户：" + BaseForm._operatorName,
                        BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);
                        if (searchResult)//如果成功
                        {
                            txtConveyor2Mark2SearchRowResult.Text = rowCoor.ToString("f2");
                            txtConveyor2Mark2SearchColumnResult.Text = columnCoor.ToString("f2");
                        }
                        break;
                    }
                default:
                    return;
            }

            if (ho_retrunRegion != null)
            {
                ho_retrunRegion.Dispose();
            }
        }

        #endregion

        //显示路径轨迹
        private void btnShowTrack_Click(object sender, EventArgs e)
        {
            if (_trackDispFormVariate != null)
            {
                _trackDispFormVariate.Dispose();
                _trackDispFormVariate = null;
            }

            if (dgrdvProductPointPosition.Rows.Count <= 0)
            {
                MyTool.TxtFileProcess.CreateLog("产品设置页面-控件中无点位数据，无法显示点位轨迹图!" + "----用户：" + BaseForm._operatorName,
            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                MessageBox.Show("控件中无点位数据，无法显示点位轨迹图!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            //从datagridview中读取数据
            DataTable pointPositionDataDT = new DataTable();
            for (int i = 0; i < dgrdvProductPointPosition.ColumnCount; i++)
            {
                pointPositionDataDT.Columns.Add(dgrdvProductPointPosition.Columns[i].Name, typeof(string));
            }
            for (int i = 0; i < dgrdvProductPointPosition.Rows.Count; i++)
            {
                DataRow dr = pointPositionDataDT.NewRow();
                for (int j = 0; j < dgrdvProductPointPosition.ColumnCount; j++)
                {
                    dr[j] = dgrdvProductPointPosition.Rows[i].Cells[j].Value;
                }
                pointPositionDataDT.Rows.Add(dr.ItemArray);
            }

            //如果检测数据失败
            if (!PointPositionDataCheck(pointPositionDataDT))
            {
                MyTool.TxtFileProcess.CreateLog("产品设置页面-控件中有非法参数，无法显示点位轨迹图!" + "----用户：" + BaseForm._operatorName,
            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                MessageBox.Show("控件中有非法参数，无法显示点位轨迹图!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _trackDispFormVariate = new TrackDisplayBasePointData(pointPositionDataDT);
            _trackDispFormVariate.Show();
        }

    }

}
