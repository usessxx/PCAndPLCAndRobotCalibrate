using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Printing;//用于获取打印机列表
using XMLFile;//用于读取保存xml文件
using System.IO;
using SoftKeyBoard;
using System.Xml;

namespace ThreeDimensionAVI
{
    public partial class SettingsGeneralForm : Form
    {
        /*--------------------------------------------------------------------------------------
        //Copyright (C) 2022 深圳市利器精工科技有限公司
        //版权所有
        //
        //文件名：管理员参数设置界面
        //文件功能描述：用于设置一部分管理员相关的参数
        //
        //
        //创建标识：MaLi 20220316
        //
        //修改标识：MaLi 20220316 Change
        //修改描述：增加管理员参数设置界面
        //
        //------------------------------------------------------------------------------------*/
        //
        //******************************事件************************************//

        //*************************外部可读写变量*******************************//

        //*************************公共静态变量*******************************//
        public static string _dataDirectory;//数据文件路径
        public static string _samplePanelDirectory;//样品板文件路径
        public static string _productDirectory;//产品数据文件路径
        public static string _printerName;//打印机名称
        public static string _pdfPrinterName;//PDF打印机名称

        public static float[] _cameraOffsetCoor = new float[2];//相机offset坐标，0-X坐标，1-Y坐标

        public static string _conveyorControlCom;//扫码枪com口号

        public static bool _saveSourceImageFlag = false;//保存原始图片标志
        public static bool _saveOKInferredImageFlag = false;//保存推论OK图片标志
        public static bool _saveNGInferredImageFlag = false;//保存推论NG图片标志
        public static bool _saveResultListFileFlag = false;//保存结果表格文件标志

        public static bool _autoClearYieldUseFlag = false;//自动清除产量功能启用标志
        public static int _clearYieldTimeHour = 8;//产量清除时
        public static int _clearYieldTimeMinutes = 0;//产量清除分
        public static float _warningTimeAfterShiftWithoutSampleCheck = 0;//换班之后样品板未检测警告时长

        public static bool _conveyor1UsingFlag = true;//传送线1使用标志，false-不使用（那么传送线将会只作为传送线运动，不会进行产品检测），true-使用
        public static bool _conveyor2UsingFlag = true;//传送线1使用标志，false-不使用（那么传送线将会只作为传送线运动，不会进行产品检测），true-使用

        public static bool _addSampleDataToYieldFlag = false;//将样品板数据计入产量标志，false-不计入，true-计入
        public static bool _samplePanelCheckShieldFlag = false;//样品板必检功能屏蔽
        //*************************内部私有变量*******************************//
        string _xmlFileParentData = "Settings General";
        string _xmlFileRootNodeName = "ArrayOfClsObjectProperties";
        string _xmlFileMainNodeName = "clsObjectProperties";
        ////////////////////////////////////////////////////


        //构造函数
        public SettingsGeneralForm()
        {
            InitializeComponent();

            //初始化串口号选择combobox
            cboConveyorControlCOMNo.DropDownStyle = ComboBoxStyle.DropDownList;
            string tempComboItem;
            for(int i=0;i<100;i++)
            {
                tempComboItem = "COM" + (i + 1).ToString();
                cboConveyorControlCOMNo.Items.Add(tempComboItem);
            }

            //读取参数
            LoadingDataFromXmlFile();

        }

        #region DirectoryBrowseEvent:获取文件路径事件

        private void DirectoryBrowseEvent(object sender, EventArgs e)
        {
            MyTool.TxtFileProcess.CreateLog("通用参数设置页面-请求修改路径！" + "----用户：" + BaseForm._operatorName,
            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

            if (BaseForm._autoModeFlag)
            {
                MyTool.TxtFileProcess.CreateLog("通用参数设置页面-请求修改路径失败，设备当前处于自动模式！" + "----用户：" + BaseForm._operatorName,
                BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                MessageBox.Show("当前设备处于自动模式，无法修改路径！");

                return;
            }
            FolderBrowserDialog fb = new FolderBrowserDialog();
            string name = ((Button)sender).Name;
            switch (name)
            {
                case "btnBrowseDataDirectory":
                    {
                        fb.SelectedPath = txtDataDirectory.Text;
                        break;
                    }
                case "btnBrowseSamplePanelDirectory":
                    {
                        fb.SelectedPath = txtSamplePanelDirectory.Text;
                        break;
                    }
                case "btnBrowseProductDirectory":
                    {
                        fb.SelectedPath = txtProductDirectory.Text;
                        break;
                    }
            }
            if (fb.ShowDialog() == DialogResult.OK)
            {
                switch (name)
                {

                    case "btnBrowseDataDirectory":
                        {
                            MyTool.TxtFileProcess.CreateLog("通用参数设置页面-修改数据文件路径为"+fb.SelectedPath+"！" + "----用户：" + BaseForm._operatorName,
                            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                            txtDataDirectory.Text = fb.SelectedPath;
                            break;
                        }
                    case "btnBrowseSamplePanelDirectory":
                        {
                            MyTool.TxtFileProcess.CreateLog("通用参数设置页面-修改样品板文件路径为" + fb.SelectedPath + "！" + "----用户：" + BaseForm._operatorName,
                            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                            txtSamplePanelDirectory.Text = fb.SelectedPath;
                            break;
                        }
                    case "btnBrowseProductDirectory":
                        {
                            MyTool.TxtFileProcess.CreateLog("通用参数设置页面-修改产品文件路径为" + fb.SelectedPath + "！" + "----用户：" + BaseForm._operatorName,
                            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                            txtProductDirectory.Text = fb.SelectedPath;
                            break;
                        }
                }
            }
        }

        #endregion

        #region PrinterBrowseEvent:获取打印机名称等事件
        private void PrinterBrowseEvent(object sender, EventArgs e)
        {
            PrintDialog pd = new PrintDialog();
            string name = ((Button)sender).Name;
            if (pd.ShowDialog() == DialogResult.OK)
            {
                switch (name)
                {
                    case "btnBrowsePrinter":
                        {
                            MyTool.TxtFileProcess.CreateLog("通用参数设置页面-修改打印机为" + pd.PrinterSettings.PrinterName + "！" + "----用户：" + BaseForm._operatorName,
                            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                            txtPrinter.Text = pd.PrinterSettings.PrinterName;
                            break;
                        }
                    case "btnBrowsePDFPrinter":
                        {
                            MyTool.TxtFileProcess.CreateLog("通用参数设置页面-修改PDF打印机为" + pd.PrinterSettings.PrinterName + "！" + "----用户：" + BaseForm._operatorName,
                           BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                            txtPDFPrinter.Text = pd.PrinterSettings.PrinterName;
                            break;
                        }
                }
            }
        }
        #endregion

        #region 参数xml文件的读取与保存

        /// <summary>
        /// 用于保存数据至Xml文件
        /// </summary>
        public void SavingDataToXmlFile()
        {
            MyTool.TxtFileProcess.CreateLog("通用参数设置页面-保存数据至文件中！" + "----用户：" + BaseForm._operatorName,
            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

            string fileDirectory = Directory.GetCurrentDirectory() + "\\";
            string fileName = "Setting.xml";
            string filePath = fileDirectory + fileName;
            string tempCtrlName, tempCtrlNameSuffix;
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

            for (int i = 0; i < pnlSettingsGeneralFormDisplay.Controls.Count; i++)
            {
                needSaveDataCtrlFlag = false;
                tempCtrlName = pnlSettingsGeneralFormDisplay.Controls[i].Name.ToString();
                try
                {
                    tempCtrlNameSuffix = tempCtrlName.Substring(0, 3);
                    switch (tempCtrlNameSuffix)
                    {
                        case "txt"://如果此控件为textbox控件
                        case "chk"://如果此控件为checkbox控件
                        case "cbo"://如果此控件为ComboBox控件
                            {
                                tempValue = "";
                                if (tempCtrlNameSuffix == "txt")//如果为TextBox控件
                                    tempValue = pnlSettingsGeneralFormDisplay.Controls[i].Text.ToString();
                                else if (tempCtrlNameSuffix == "chk" && tempCtrlName != "chkShieldMES")//如果为checkbox控件
                                {
                                    CheckBox cb = pnlSettingsGeneralFormDisplay.Controls[i] as CheckBox;
                                    tempValue = cb.Checked.ToString();
                                }
                                else if (tempCtrlNameSuffix == "cbo")//如果此控件为ComboBox控件
                                {
                                    ComboBox cb = pnlSettingsGeneralFormDisplay.Controls[i] as ComboBox;
                                    tempValue = cb.SelectedIndex.ToString();
                                }
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

        /// <summary>
        /// 用于从Setting.xml中读取数据
        /// </summary>
        public void LoadingDataFromXmlFile()
        {

            MyTool.TxtFileProcess.CreateLog("通用参数设置页面-从文件中读取数据！" + "----用户：" + BaseForm._operatorName,
            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

            string fileDirectory = Directory.GetCurrentDirectory() + "\\";
            string fileName = "Setting.xml";
            string filePath = fileDirectory + fileName;
            string tempCtrlName, tempCtrlNameSuffix;
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

            for (int i = 0; i < pnlSettingsGeneralFormDisplay.Controls.Count; i++)
            {
                needLoadDataCtrlFlag = false;
                tempCtrlName = pnlSettingsGeneralFormDisplay.Controls[i].Name.ToString();
                try
                {
                    tempCtrlNameSuffix = tempCtrlName.Substring(0, 3);
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

                    switch (tempCtrlNameSuffix)
                    {
                        case "txt":
                        case "chk":
                        case "cbo":
                            {
                                needLoadDataCtrlFlag = true;
                                if (elementExistFlag)//如果这个节点存在
                                {
                                    if (tempCtrlNameSuffix == "txt")
                                        pnlSettingsGeneralFormDisplay.Controls[i].Text = tempValue[2];
                                    else if (tempCtrlNameSuffix == "chk" && tempCtrlName != "chkShieldMES")
                                    {
                                        CheckBox cb = pnlSettingsGeneralFormDisplay.Controls[i] as CheckBox;
                                        if (tempValue[2] == "true" || tempValue[2] == "True" || tempValue[2] == "TRUE")
                                        {
                                            cb.Checked = true;
                                        }
                                        else
                                        {
                                            cb.Checked = false;
                                        }
                                    }
                                    else if (tempCtrlNameSuffix == "cbo")
                                    {
                                        ComboBox cb = pnlSettingsGeneralFormDisplay.Controls[i] as ComboBox;
                                        try
                                        {
                                            cb.SelectedIndex = Convert.ToInt32(tempValue[2]);
                                        }
                                        catch
                                        {
                                            cb.SelectedIndex = 0;
                                        }
                                    }
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

        /// <summary>
        /// 基于控件状态更新参数变量
        /// </summary>
        private void ParameterRefresh()
        {
            MyTool.TxtFileProcess.CreateLog("通用参数设置页面-数据更新！" + "----用户：" + BaseForm._operatorName,
            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

            _dataDirectory = txtDataDirectory.Text;//闭门器数据文件路径
            _samplePanelDirectory = txtSamplePanelDirectory.Text;//闭门器图像处理图像及相机标定数据文件路径
            _productDirectory = txtProductDirectory.Text;//闭门器产品数据文件路径
            _printerName = txtPrinter.Text;//闭门器打印机名称
            _pdfPrinterName = txtPDFPrinter.Text;//闭门器PDF打印机名称
            _cameraOffsetCoor[0] = Convert.ToSingle(txtCameraXDirectionOffset.Text);
            _cameraOffsetCoor[1] = Convert.ToSingle(txtCameraYDirectionOffset.Text);
            _conveyorControlCom = "com" + (cboConveyorControlCOMNo.SelectedIndex + 1).ToString();

            _saveSourceImageFlag = chkSaveSourceImage.Checked;
            _saveOKInferredImageFlag = chkSaveOKInferredImage.Checked;
            _saveNGInferredImageFlag = chkSaveNGInferredImage.Checked;
            _saveResultListFileFlag = chkSaveResultListFile.Checked;
            _autoClearYieldUseFlag = chkAutoClearYieldUse.Checked;

            _conveyor1UsingFlag = chkConveyor1Using.Checked;
            _conveyor2UsingFlag = chkConveyor2Using.Checked;

            _addSampleDataToYieldFlag = chkSampleYieldAddToRecord.Checked;

            _samplePanelCheckShieldFlag = chkDisableSamplePanelCheck.Checked;

            try
            {
                _clearYieldTimeHour = Convert.ToInt32(txtShiftTimeHour.Text);
            }
            catch
            {
                _clearYieldTimeHour = 8;
                txtShiftTimeHour.Text = "8";
            }

            try
            {
                _clearYieldTimeMinutes = Convert.ToInt32(txtShiftTimeMinutes.Text);
            }
            catch
            {
                _clearYieldTimeMinutes = 0;
                txtShiftTimeMinutes.Text = "0";

            }

            try
            {
                _warningTimeAfterShiftWithoutSampleCheck = Convert.ToSingle(txtWarningTimeAfterShift.Text);
            }
            catch
            {
                _warningTimeAfterShiftWithoutSampleCheck = 0;
                txtWarningTimeAfterShift.Text = "0";
            }
        }

        #endregion

        private void SettingsGeneralForm_VisibleChanged(object sender, EventArgs e)
        {
            if (!this.Visible)
            {
                SavingDataToXmlFile();
            }
            else
            {
                //读取参数
                LoadingDataFromXmlFile();
            }
        }

        #region 虚拟键盘相关函数
        //textbox 鼠标双击事件
        private void TextBoxDoubleClickEvent(object sender, EventArgs e)
        {
            MyTool.TxtFileProcess.CreateLog("通用参数设置页面-打开软键盘！" + "----用户：" + BaseForm._operatorName,
            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

            if (((TextBox)sender).Name != "txtMESIPAddress" &&
                ((TextBox)sender).Name != "txtCameraXDirectionOffset" &&
                ((TextBox)sender).Name != "txtCameraYDirectionOffset" &&
                ((TextBox)sender).Name != "txtShiftTimeHour" &&
                ((TextBox)sender).Name != "txtShiftTimeMinutes" &&
                ((TextBox)sender).Name != "txtWarningTimeAfterShift")
            {
                SoftMainKeyboard.LanguageFlag = 1;
                SoftMainKeyboard mainKeyboard = new SoftMainKeyboard();
                mainKeyboard.StartPosition = FormStartPosition.CenterScreen;
                mainKeyboard.SOURCESTR = ((TextBox)sender).Text;
                mainKeyboard.SENDER = sender;
                mainKeyboard.SendInputDataIncludeObj += new SENDINPUTDATAINCLUDEOBJ(GetKeyboardInputData);//添加事件
                mainKeyboard.ShowDialog();
            }
            else
            {
                SoftNumberKeyboard.LanguageFlag = 1;
                SoftNumberKeyboard numberKeyboard = new SoftNumberKeyboard();
                numberKeyboard.StartPosition = FormStartPosition.CenterScreen;
                if (((TextBox)sender).Name == "txtMESIPAddress")
                {
                    numberKeyboard.PLUSMINUSBTHIDEFLAG = true;
                    numberKeyboard.FULLSTOPBTHIDEFLAG = false;
                    numberKeyboard.IPADDRESSFLAG = true;
                }
                else if (((TextBox)sender).Name == "txtShiftTimeHour" || ((TextBox)sender).Name == "txtShiftTimeMinutes")
                {
                    numberKeyboard.PLUSMINUSBTHIDEFLAG = true;
                    numberKeyboard.FULLSTOPBTHIDEFLAG = true;
                    numberKeyboard.IPADDRESSFLAG = false;
                }
                else if (((TextBox)sender).Name == "txtWarningTimeAfterShift")
                {
                    numberKeyboard.PLUSMINUSBTHIDEFLAG = true;
                    numberKeyboard.FULLSTOPBTHIDEFLAG = false;
                    numberKeyboard.IPADDRESSFLAG = false;
                }
                else
                {
                    numberKeyboard.PLUSMINUSBTHIDEFLAG = false;
                    numberKeyboard.FULLSTOPBTHIDEFLAG = false;
                    numberKeyboard.IPADDRESSFLAG = false;
                }
                numberKeyboard.SOURCESTR = ((TextBox)sender).Text;
                numberKeyboard.SENDER = sender;
                numberKeyboard.SendInputDataIncludeObj += new SENDINPUTDATAINCLUDEOBJ(GetKeyboardInputData);//添加事件
                numberKeyboard.ShowDialog();
            }

        }

        //键盘按下OK之后获取数据
        private void GetKeyboardInputData(object Sender, string GetDataStr)
        {
            ((TextBox)Sender).Text = GetDataStr;//将键盘按钮输入的数据传递给密码输入框
            ((TextBox)Sender).Focus();
            ((TextBox)Sender).SelectionLength = 0;
        }

        //Textbox数据检查函数
        private void TextBoxDataCheckEvent(object sender, EventArgs e)
        {
            if (((TextBox)sender).Name == "txtShiftTimeHour")
            {
                ((TextBox)sender).Text = MyTool.DataProcessing.TextBoxDataCheck(((TextBox)sender).Text, 0, 12, true, true, false, false);
                ((TextBox)sender).SelectionStart = ((TextBox)sender).TextLength;
                ((TextBox)sender).SelectionLength = 0;
            }
            else if (((TextBox)sender).Name == "txtShiftTimeMinutes")
            {
                ((TextBox)sender).Text = MyTool.DataProcessing.TextBoxDataCheck(((TextBox)sender).Text, 0, 59, true, true, false, false);
                ((TextBox)sender).SelectionStart = ((TextBox)sender).TextLength;
                ((TextBox)sender).SelectionLength = 0;
            }
            else if (((TextBox)sender).Name == "txtWarningTimeAfterShift")
            {
                ((TextBox)sender).Text = MyTool.DataProcessing.TextBoxDataCheck(((TextBox)sender).Text, 0, 3, true, true, true, false);
                ((TextBox)sender).SelectionStart = ((TextBox)sender).TextLength;
                ((TextBox)sender).SelectionLength = 0;
            }
            else
            {
                ((TextBox)sender).Text = MyTool.DataProcessing.TextBoxDataCheck(((TextBox)sender).Text, -9999999, 9999999, true, true, true, false);
                ((TextBox)sender).SelectionStart = ((TextBox)sender).TextLength;
                ((TextBox)sender).SelectionLength = 0;
            }

        }

        //Textbox数据检查函数
        private void TextBoxLeaveEvent(object sender, EventArgs e)
        {
            if (((TextBox)sender).Name == "txtShiftTimeHour")
            {
                ((TextBox)sender).Text = MyTool.DataProcessing.TextBoxDataCheck(((TextBox)sender).Text, 0, 12, true, true, false, true);
                ((TextBox)sender).SelectionStart = ((TextBox)sender).TextLength;
                ((TextBox)sender).SelectionLength = 0;
                if (Convert.ToInt32(((TextBox)sender).Text) == 12)//如果为12点，那么分钟只能为0
                {
                    txtShiftTimeMinutes.Text = "0";
                }
            }
            else if (((TextBox)sender).Name == "txtShiftTimeMinutes")
            {
                ((TextBox)sender).Text = MyTool.DataProcessing.TextBoxDataCheck(((TextBox)sender).Text, 0, 59, true, true, false, true);
                ((TextBox)sender).SelectionStart = ((TextBox)sender).TextLength;
                ((TextBox)sender).SelectionLength = 0;
            }
            else if (((TextBox)sender).Name == "txtWarningTimeAfterShift")
            {
                ((TextBox)sender).Text = MyTool.DataProcessing.TextBoxDataCheck(((TextBox)sender).Text, 0, 3, true, true, true, true);
                ((TextBox)sender).SelectionStart = ((TextBox)sender).TextLength;
                ((TextBox)sender).SelectionLength = 0;
            }
            else
            {
                ((TextBox)sender).Text = MyTool.DataProcessing.TextBoxDataCheck(((TextBox)sender).Text, -9999999, 9999999, true, true, true, true);
                ((TextBox)sender).SelectionStart = ((TextBox)sender).TextLength;
                ((TextBox)sender).SelectionLength = 0;
            }
            MyTool.TxtFileProcess.CreateLog("通用参数设置页面-修改“" + ((TextBox)sender).Name.Substring(3) + "”数据为" + ((TextBox)sender).Text + "----用户：" + BaseForm._operatorName,
            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);
        }

        //检测address格式输入框
        private void AddressCheckEvent(object sender, EventArgs e)
        {
            ((TextBox)sender).Text = MyTool.DataProcessing.TextBoxIPFormatCheck(((TextBox)sender).Text, false);
            ((TextBox)sender).SelectionStart = ((TextBox)sender).TextLength;
            ((TextBox)sender).SelectionLength = 0;
        }

        //检测address格式输入框
        private void AddressCheckLeaveEvent(object sender, EventArgs e)
        {
            ((TextBox)sender).Text = MyTool.DataProcessing.TextBoxIPFormatCheck(((TextBox)sender).Text, true);

        }

        #endregion

    }
}
