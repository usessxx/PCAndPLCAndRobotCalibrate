using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using XMLFile;//关于xml文件的读写
using System.IO;
using SoftKeyBoard;
using System.Xml;


namespace ThreeDimensionAVI
{
    public partial class SettingsAdministratorForm : Form
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
        public static float[] _absoluteCoor = new float[8] { 0, 0, 0, 0, 0, 0, 0, 0 };//八个轴的绝对坐标
        public static string _leftCameraName;//左相机名称
        public static string _rightCameraName;//右相机名称
        public static string _backCameraName;//后相机名称
        public static int _leftAndRightCameraGrabImageInterval;//左右相机获取图像间隔
       
        public static string _machineName;//设备名称
        public static string _testLineResource;//设备测试线别
        public static string _workArea;//测试区域
        public static string _testType;//测试类型
        public static string _operatorType;//操作类型
        public static string _testMode;//测试模式
        public static string _site;//厂区名
        public static string _devicePhysicAddress;//设备物理地址
        public static string _programName;//程序名
        public static string _haveTrackFlag;//过站，0-不过站，1-过站

        public static bool _devicePauseWhenAccessRateOccurFlag = false;//当设备出现直通率报警时，设备暂停，false-不暂停，true-暂停
        public static float _smallestAccessRate = 0f;//设置的最小直通过率

        public static float _passThresholdValue = 0;//判定产品Pass的置信度阈值
        public static bool _checkProductIsConnector = false;//检测产品为连接器标志，如果为connector，那么为true，不然为false
        //*************************内部私有变量*******************************//
        System.Windows.Forms.Timer _formUpdateThread = null;//用于基于记录检测产量的变量数值，更新产量至界面

        string _xmlFileParentData = "Settings Administrator";//用于记录设定参数的xml文件中的父数据
        string _xmlFileRootNodeName = "ArrayOfClsObjectProperties";//用于记录设定参数的xml文件中的根节点名称
        string _xmlFileMainNodeName = "clsObjectProperties";//用于记录设定参数的xml文件中的主节点名称
        ////////////////////////////////////////////////////


        //构造函数
        public SettingsAdministratorForm()
        {
            InitializeComponent();

            LoadingDataFromXmlFile();//从settings.xml文件中读取出数据

            //实例化产量显示用timer
            _formUpdateThread = new System.Windows.Forms.Timer();
            _formUpdateThread.Tick += new EventHandler(FormUpdateThreadFunc);//关联事件
            _formUpdateThread.Interval = 100;//timer间隔

        }

        #region 参数xml文件的读取与保存

        /// <summary>
        /// 用于保存数据至Xml文件
        /// </summary>
        public void SavingDataToXmlFile()
        {
            MyTool.TxtFileProcess.CreateLog("管理员设置页面-保存设定数据" + "----用户：" + BaseForm._operatorName,
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

            for (int i = 0; i < pnlSettingsAdministratorFormDisplay.Controls.Count; i++)
            {
                needSaveDataCtrlFlag = false;
                tempCtrlName = pnlSettingsAdministratorFormDisplay.Controls[i].Name.ToString();
                try
                {
                    tempCtrlNameSuffix = tempCtrlName.Substring(0, 3);
                    switch (tempCtrlNameSuffix)
                    {
                        case "txt"://如果此控件为textbox控件
                        case "chk"://如果此控件为checkbox控件
                            {
                                tempValue = "";
                                if (tempCtrlNameSuffix == "txt")//如果为TextBox控件
                                    tempValue = pnlSettingsAdministratorFormDisplay.Controls[i].Text.ToString();
                                else
                                {
                                    CheckBox cb = pnlSettingsAdministratorFormDisplay.Controls[i] as CheckBox;
                                    tempValue = cb.Checked.ToString();
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
            MyTool.TxtFileProcess.CreateLog("管理员设置页面-读取设定数据" + "----用户：" + BaseForm._operatorName,
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

            for (int i = 0; i < pnlSettingsAdministratorFormDisplay.Controls.Count; i++)
            {
                needLoadDataCtrlFlag = false;
                tempCtrlName = pnlSettingsAdministratorFormDisplay.Controls[i].Name.ToString();
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
                            {
                                needLoadDataCtrlFlag = true;
                                if (elementExistFlag)//如果这个节点存在
                                {
                                    if (tempCtrlNameSuffix == "txt")
                                        pnlSettingsAdministratorFormDisplay.Controls[i].Text = tempValue[2];
                                    else
                                    {
                                        CheckBox cb = pnlSettingsAdministratorFormDisplay.Controls[i] as CheckBox;
                                        if (tempValue[2] == "true" || tempValue[2] == "True" || tempValue[2] == "TRUE")
                                        {
                                            cb.Checked = true;
                                        }
                                        else
                                        {
                                            cb.Checked = false;
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

        //基于控件状态更新参数变量
        private void ParameterRefresh()
        {
            MyTool.TxtFileProcess.CreateLog("管理员设置页面-设定数据更新" + "----用户：" + BaseForm._operatorName,
             BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

            //轴绝对坐标
            try
            {
                _absoluteCoor[0] = Convert.ToSingle(txtX1AboslutePosition.Text);
            }
            catch
            {
                _absoluteCoor[0] = 0f;
                txtX1AboslutePosition.Text = "0";
            }

            try
            {
                _absoluteCoor[1] = Convert.ToSingle(txtY1AboslutePosition.Text);
            }
            catch
            {
                _absoluteCoor[1] = 0f;
                txtY1AboslutePosition.Text = "0";
            }

            try
            {
                _absoluteCoor[2] = Convert.ToSingle(txtR1AboslutePosition.Text);
            }
            catch
            {
                _absoluteCoor[2] = 0f;
                txtR1AboslutePosition.Text = "0";
            }

            try
            {
                _absoluteCoor[3] = Convert.ToSingle(txtY2AboslutePosition.Text);
            }
            catch
            {
                _absoluteCoor[3] = 0f;
                txtY2AboslutePosition.Text = "0";
            }

            try
            {
                _absoluteCoor[4] = Convert.ToSingle(txtY3AboslutePosition.Text);
            }
            catch
            {
                _absoluteCoor[4] = 0f;
                txtY2AboslutePosition.Text = "0";
            }

            try
            {
                _absoluteCoor[5] = Convert.ToSingle(txtY4AboslutePosition.Text);
            }
            catch
            {
                _absoluteCoor[5] = 0f;
                txtY2AboslutePosition.Text = "0";
            }

         
            //相机名更新
            _leftCameraName = txtLeftCameraName.Text;
            _rightCameraName = txtRightCameraName.Text;
            _backCameraName = txtBackCameraName.Text;
            try
            {
                _leftAndRightCameraGrabImageInterval = Convert.ToInt32(txtLeftAndRightCameraGrabInterval.Text);
            }
            catch
            {
                _leftAndRightCameraGrabImageInterval = 0;
                txtLeftAndRightCameraGrabInterval.Text = "0";
            }

            #region MES相关参数
            //设备编号
            _machineName = txtMachineName.Text;
            //设备测试线别
            _testLineResource = txtTestLineResource.Text;
            //测试区域
            _workArea = txtWorkArea.Text;
            //测试类型
            _testType = txtTestType.Text;
            //操作类型
            _operatorType = txtOperatorType.Text;
            //测试模式
            _testMode = txtTestMode.Text;
            //厂区名
            _site = txtSite.Text;
            //设备物理地址
            _devicePhysicAddress = txtDevicePhysicAddress.Text;
            //程序名
            _programName = txtProgramName.Text;
            //过站，0-不过站，1-过站
            if (chkHaveTrack.Checked)
                _haveTrackFlag = "1";
            else
                _haveTrackFlag = "0";
            #endregion

            try
            {
                _smallestAccessRate = Convert.ToSingle(txtSmallestPassRate.Text);
                if (_smallestAccessRate < 0 || _smallestAccessRate > 1)
                {
                    _smallestAccessRate = 0f;
                    txtSmallestPassRate.Text = _smallestAccessRate.ToString("f2");
                }
            }
            catch
            {
                _smallestAccessRate = 0f;
                txtSmallestPassRate.Text = _smallestAccessRate.ToString("f2");
            }
            _devicePauseWhenAccessRateOccurFlag = chkDeivcePauseWhenAccessRateLow.Checked;

             //pass threshold
            try
            {
                _passThresholdValue = Convert.ToSingle(txtPassThresholdValue.Text);
                if (_passThresholdValue < 0 || _passThresholdValue > 1)
                {
                    _passThresholdValue = 0f;
                    txtPassThresholdValue.Text = _passThresholdValue.ToString("f2");
                }
            }
            catch
            {
                _passThresholdValue = 0f;
                txtPassThresholdValue.Text = _passThresholdValue.ToString("f2");
            }

            _checkProductIsConnector = chkCheckProductIsConnector.Checked;
        }

        #endregion

        #region SettingsAdministratorForm_VisibleChanged:窗口显示隐藏事件
        private void SettingsAdministratorForm_VisibleChanged(object sender, EventArgs e)
        {
            //当窗口不显示的时候保存一次
            if (!this.Visible)
            {
                SavingDataToXmlFile();
                if (_formUpdateThread != null)
                    _formUpdateThread.Stop();
            }
            else
            {
                if (_formUpdateThread != null)
                    _formUpdateThread.Start();
            }
        }
        #endregion

        #region FormUpdateThreadFunc:界面更新线程函数
        private void FormUpdateThreadFunc(object sender, EventArgs e)
        {
            if (BaseForm._autoModeFlag)//如果为自动模式，绝对补偿位置坐标控件disable
            {
                txtX1AboslutePosition.Enabled = false;
                txtY1AboslutePosition.Enabled = false;
                txtR1AboslutePosition.Enabled = false;
                txtY2AboslutePosition.Enabled = false;
                txtY3AboslutePosition.Enabled = false;
                txtY4AboslutePosition.Enabled = false;
                btnAboslutePositionTeach.Enabled = false;
            }
            else
            {
                txtX1AboslutePosition.Enabled = true;
                txtY1AboslutePosition.Enabled = true;
                txtR1AboslutePosition.Enabled = true;
                txtY2AboslutePosition.Enabled = true;
                txtY3AboslutePosition.Enabled = true;
                txtY4AboslutePosition.Enabled = true;
                btnAboslutePositionTeach.Enabled = true;
            }
        }

        #endregion

        #region 虚拟键盘相关函数
        //textbox 鼠标双击事件
        private void TextBoxDoubleClickEvent(object sender, EventArgs e)
        {
            MyTool.TxtFileProcess.CreateLog("管理员设置页面-打开软键盘" + "----用户：" + BaseForm._operatorName,
               BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

            if (((TextBox)sender).Name != "txtPasswordTimeOut" &&
                ((TextBox)sender).Name != "txtX1AboslutePosition" &&
                ((TextBox)sender).Name != "txtY1AboslutePosition" &&
                ((TextBox)sender).Name != "txtR1AboslutePosition" &&
                ((TextBox)sender).Name != "txtY2AboslutePosition" && 
                ((TextBox)sender).Name != "txtY3AboslutePosition" && 
                ((TextBox)sender).Name != "txtY4AboslutePosition" &&
                ((TextBox)sender).Name != "txtSmallestPassRate" &&
                ((TextBox)sender).Name != "txtPassThresholdValue")
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
                if (((TextBox)sender).Name == "txtPasswordTimeOut" || ((TextBox)sender).Name == "txtLeftAndRightCameraGrabInterval")
                {
                    numberKeyboard.PLUSMINUSBTHIDEFLAG = true;
                    numberKeyboard.FULLSTOPBTHIDEFLAG = true;
                }
                else if (((TextBox)sender).Name == "txtSmallestPassRate" || ((TextBox)sender).Name == "txtPassThresholdValue")
                {
                    numberKeyboard.PLUSMINUSBTHIDEFLAG = true;
                    numberKeyboard.FULLSTOPBTHIDEFLAG = false;
                }
                else
                {
                    numberKeyboard.PLUSMINUSBTHIDEFLAG = false;
                    numberKeyboard.FULLSTOPBTHIDEFLAG = false;
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
            if (((TextBox)sender).Name == "txtPasswordTimeOut")
            {
                ((TextBox)sender).Text = MyTool.DataProcessing.TextBoxDataCheck(((TextBox)sender).Text, 1, 9999999, true, true, false, false);
                ((TextBox)sender).SelectionStart = ((TextBox)sender).TextLength;
                ((TextBox)sender).SelectionLength = 0;
            }
            else if(((TextBox)sender).Name == "txtLeftAndRightCameraGrabInterval")
            {
                ((TextBox)sender).Text = MyTool.DataProcessing.TextBoxDataCheck(((TextBox)sender).Text, 0, 9999999, true, true, false, false);
                ((TextBox)sender).SelectionStart = ((TextBox)sender).TextLength;
                ((TextBox)sender).SelectionLength = 0;
            }
            else if (((TextBox)sender).Name == "txtSmallestPassRate" || ((TextBox)sender).Name == "txtPassThresholdValue")//如果为最小直通率或通过阈值
            {
                ((TextBox)sender).Text = MyTool.DataProcessing.TextBoxDataCheck(((TextBox)sender).Text, 0.01f, 1f, true, true, true, false);
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
            if (((TextBox)sender).Name == "txtPasswordTimeOut")
            {
                ((TextBox)sender).Text = MyTool.DataProcessing.TextBoxDataCheck(((TextBox)sender).Text, 1, 9999999, true, true, false, true);
                ((TextBox)sender).SelectionStart = ((TextBox)sender).TextLength;
                ((TextBox)sender).SelectionLength = 0;
            }
            else if (((TextBox)sender).Name == "txtLeftAndRightCameraGrabInterval")
            {
                ((TextBox)sender).Text = MyTool.DataProcessing.TextBoxDataCheck(((TextBox)sender).Text, 0, 9999999, true, true, false, false);
                ((TextBox)sender).SelectionStart = ((TextBox)sender).TextLength;
                ((TextBox)sender).SelectionLength = 0;
            }
            else if (((TextBox)sender).Name == "txtSmallestPassRate" || ((TextBox)sender).Name == "txtPassThresholdValue")//如果为最小直通率或通过阈值
            {
                ((TextBox)sender).Text = MyTool.DataProcessing.TextBoxDataCheck(((TextBox)sender).Text, 0.01f, 1f, true, true, true, false);
                ((TextBox)sender).SelectionStart = ((TextBox)sender).TextLength;
                ((TextBox)sender).SelectionLength = 0;
            }
            else
            {
                ((TextBox)sender).Text = MyTool.DataProcessing.TextBoxDataCheck(((TextBox)sender).Text, -9999999, 9999999, true, true, true, true);
                ((TextBox)sender).SelectionStart = ((TextBox)sender).TextLength;
                ((TextBox)sender).SelectionLength = 0;
                switch (((TextBox)sender).Name)
                {
                    case "txtX1AboslutePosition":
                        _absoluteCoor[0] = Convert.ToSingle(txtX1AboslutePosition.Text);
                        break;
                    case "txtY1AboslutePosition":
                        _absoluteCoor[1] = Convert.ToSingle(txtY1AboslutePosition.Text);
                        break;
                    case "txtR1AboslutePosition":
                        _absoluteCoor[2] = Convert.ToSingle(txtR1AboslutePosition.Text);
                        break;
                    case "txtY2AboslutePosition":
                        _absoluteCoor[3] = Convert.ToSingle(txtY2AboslutePosition.Text);
                        break;
                    case "txtY3AboslutePosition":
                        _absoluteCoor[4] = Convert.ToSingle(txtY3AboslutePosition.Text);
                        break;
                    case "txtY4AboslutePosition":
                        _absoluteCoor[5] = Convert.ToSingle(txtY4AboslutePosition.Text);
                        break;
                }
            }

            if (this.Visible)
            {
                MyTool.TxtFileProcess.CreateLog("管理员设置页面-修改参数“" + ((TextBox)sender).Name.Substring(3) + "”为" + ((TextBox)sender).Text + "----用户：" + BaseForm._operatorName,
                BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);
            }
        }
        #endregion

        //绝对位置Teach Button
        private void btnAboslutePositionTeach_Click(object sender, EventArgs e)
        {
            MyTool.TxtFileProcess.CreateLog("管理员设置页面-示教绝对左边为：" +
                "X1:" + txtX1AboslutePosition.Text +
                "Y1:" + txtY1AboslutePosition.Text +
                "R1:" + txtR1AboslutePosition.Text +
                "Y2:" + txtY2AboslutePosition.Text +
                "Y3:" + txtY3AboslutePosition.Text +
                "Y4:" + txtY4AboslutePosition.Text +
                 "----用户：" + BaseForm._operatorName,
            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

            txtX1AboslutePosition.Text = BaseForm._axisRealPosition[0].ToString("f2");
            _absoluteCoor[0] = BaseForm._axisRealPosition[0];

            txtY1AboslutePosition.Text = BaseForm._axisRealPosition[1].ToString("f2");
            _absoluteCoor[1] = BaseForm._axisRealPosition[1];

            txtR1AboslutePosition.Text = BaseForm._axisRealPosition[2].ToString("f2");
            _absoluteCoor[2] = BaseForm._axisRealPosition[2];

            txtY2AboslutePosition.Text = BaseForm._axisRealPosition[3].ToString("f2");
            _absoluteCoor[3] = BaseForm._axisRealPosition[3];

            txtY3AboslutePosition.Text = BaseForm._axisRealPosition[4].ToString("f2");
            _absoluteCoor[4] = BaseForm._axisRealPosition[4];

            txtY4AboslutePosition.Text = BaseForm._axisRealPosition[5].ToString("f2");
            _absoluteCoor[5] = BaseForm._axisRealPosition[5];
        }
    }
}
