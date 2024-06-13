using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using XMLFile;
using System.IO;
using SoftKeyBoard;
using System.Threading;
using HalconDotNet;
using HalconMVTec;
using System.Xml;

namespace ThreeDimensionAVI
{
        
    public partial class SettingsCameraCalibrationForm : Form
    {
        /*--------------------------------------------------------------------------------------
        //Copyright (C) 2022 深圳市利器精工科技有限公司
        //版权所有
        //
        //文件名：相机标定界面
        //文件功能描述：用于标定相机
        //
        //
        //创建标识：MaLi 20220427
        //
        //修改标识：MaLi 20220427 Change
        //修改描述：增加相机标定界面
        //
        //------------------------------------------------------------------------------------*/
        //
        //******************************事件************************************//
        public event MaintenanceCameraFormRequestGrabImageAndDispDelegate SettingsCameraCalibrationFormRequestGrabImageAndDispEvent;//请求相机抓取图像
        public event MaintenanceCameraFormRequestGrabImageAndDispVideoDelegate SettingsCameraCalibrationFormRequestGrabImageAndDispVideoEvent;//请求相机实时显示图像
        //****************************静态变量**********************************//

        //*************************外部可读写变量*******************************//
        /// <summary>
        /// 用于记录像素尺寸，0-代表像元宽，1-代表像元高，均为一个像素多少mm
        /// </summary>

        public float[] _pixelSize = new float[2];
        //*************************公共静态变量*******************************//

        //*************************内部私有变量*******************************//
        /// <summary>
        /// 用于记录在图片1中的mark点像素坐标，0-代表Column坐标，1-代表Row坐标
        /// </summary>
        float[] _markPointCoorInPic1 = new float[2];

        /// <summary>
        /// 用于记录在图片2中的mark点像素坐标，0-代表Column坐标，1-代表Row坐标
        /// </summary>
        float[] _markPointCoorInPic2 = new float[2];

        /// <summary>
        /// 用于记录拍摄图片1的时候，相机的坐标，0-代表X坐标，1-代表Y坐标
        /// </summary>
        float[] _takePic1Coor = new float[2];

        /// <summary>
        /// 用于记录拍摄图片1的时候，相机的坐标，0-代表X坐标，1-代表Y坐标
        /// </summary>
        float[] _takePic2Coor = new float[2];

        /// <summary>
        /// 定义提示信息string
        /// </summary>
        string[] _infoMsg = new string[4];
        string _xmlFileParentData = "Settings Administrator";//用于记录设定参数的xml文件中的父数据
        string _xmlFileRootNodeName = "ArrayOfClsObjectProperties";//用于记录设定参数的xml文件中的根节点名称
        string _xmlFileMainNodeName = "clsObjectProperties";//用于记录设定参数的xml文件中的主节点名称
        int _calibrationStepNo = 0;//标定步骤

        private int _cameraXPixels = 640;//相机照片X像素 //Add by YGk 20211020 ME2L-16161U3M
        private int _cameraYPixels = 480;//相机照片Y像素 //Add by YGk 20211020

        private float _drawCircleRadius = 8;//画实心圆的半径
        private Color _drawCircleclr = Color.Red;//画实心圆的颜色
        //
        //
        //

        //构造函数
        public SettingsCameraCalibrationForm()
        {
            InitializeComponent();
            //初始化需要显示的提示信息

            _infoMsg[0] = "移动相机直至这个标定需要的Mark点显示在图片的左上角处";
            _infoMsg[1] = "在图像中用鼠标标识出标定所用的Mark点的中心坐标";
            _infoMsg[2] = "移动相机直至这个标定需要的Mark点显示在图片的右下角处";
            _infoMsg[3] = "在图像中用鼠标标识出标定所用的Mark点的中心坐标";

            FinishMarkCoorSettingCtrlShowHideSetting(1, false);
            //隐藏像元计算所用控件
            lblPixelSizeResult.Visible = false;
            lblPixelColumnSizeTitle.Visible = false;
            lblPixelRowSizeTitle.Visible = false;
            txtPixelColumnSize.Visible = false;
            txtPixelRowSize.Visible = false;
            lblPixelColumnSizeUnitTitle.Visible = false;
            lblPixelRowSizeUnitTitle.Visible = false;
            btnCalibrationCalculate.Visible = false;

            //隐藏实时显示图像和抓取图像按钮
            btnStartStopRealTimeVideo.Visible = true;
            btnGetImage.Visible = false;
            chkDisCrossLine.Visible = true;

            lblCameraCalibrationInfo.Text = _infoMsg[0];

            _calibrationStepNo = 1;

            LoadingDataFromXmlFile();
        }

        //显示状态变化
        private void SettingsCameraCalibrationForm_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                //设置那些按钮那些提示信息应当显示
                FinishMarkCoorSettingCtrlShowHideSetting(1, false);

                //隐藏像元计算所用控件
                lblPixelSizeResult.Visible = false;
                lblPixelColumnSizeTitle.Visible = false;
                lblPixelRowSizeTitle.Visible = false;
                txtPixelColumnSize.Visible = false;
                txtPixelRowSize.Visible = false;
                lblPixelColumnSizeUnitTitle.Visible = false;
                lblPixelRowSizeUnitTitle.Visible = false;
                btnCalibrationCalculate.Visible = false;

                //隐藏实时显示图像和抓取图像按钮
                btnStartStopRealTimeVideo.Visible = true;
                btnGetImage.Visible = false;
                chkDisCrossLine.Visible = true;

                lblCameraCalibrationInfo.Text = _infoMsg[0];

                _calibrationStepNo = 1;
            }
        }

        //点击第一步按钮
        private void btnMoveMarkToPosition1_Click(object sender, EventArgs e)
        {
            MyTool.TxtFileProcess.CreateLog("相机标定画面-进行标定第一步操作" + "----用户：" + BaseForm._operatorName,
            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

            if (BaseForm._autoModeFlag)
            {
                MyTool.TxtFileProcess.CreateLog("相机标定画面-进行标定第一步操作失败，当前设备处于自动模式" + "----用户：" + BaseForm._operatorName,
                BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                MessageBox.Show("设备当前处于自动状态，无法进行相机标定！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            lblCameraCalibrationInfo.Text = _infoMsg[0];

            FinishMarkCoorSettingCtrlShowHideSetting(1, false);

            //隐藏像元计算所用控件
            lblPixelSizeResult.Visible = false;
            lblPixelColumnSizeTitle.Visible = false;
            lblPixelRowSizeTitle.Visible = false;
            txtPixelColumnSize.Visible = false;
            txtPixelRowSize.Visible = false;
            lblPixelColumnSizeUnitTitle.Visible = false;
            lblPixelRowSizeUnitTitle.Visible = false;
            btnCalibrationCalculate.Visible = false;

            //隐藏实时显示图像和抓取图像按钮
            btnStartStopRealTimeVideo.Visible = true;
            chkDisCrossLine.Visible = true;
            btnGetImage.Visible = false;

            _calibrationStepNo = 1;

            picBackCameraImageDisp.Refresh();

        }

        //进行第二步操作
        private void btnMarkPos1OnImage_Click(object sender, EventArgs e)
        {
            MyTool.TxtFileProcess.CreateLog("相机标定画面-进行标定第二步操作" + "----用户：" + BaseForm._operatorName,
            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

            if (BaseForm._autoModeFlag)
            {
                MyTool.TxtFileProcess.CreateLog("相机标定画面-进行标定第二步操作失败，当前设备处于自动模式" + "----用户：" + BaseForm._operatorName,
                BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                MessageBox.Show("设备当前处于自动状态，无法进行相机标定！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            lblCameraCalibrationInfo.Text = _infoMsg[1];

            FinishMarkCoorSettingCtrlShowHideSetting(1, true);

            //隐藏像元计算所用控件
            lblPixelSizeResult.Visible = false;
            lblPixelColumnSizeTitle.Visible = false;
            lblPixelRowSizeTitle.Visible = false;
            txtPixelColumnSize.Visible = false;
            txtPixelRowSize.Visible = false;
            lblPixelColumnSizeUnitTitle.Visible = false;
            lblPixelRowSizeUnitTitle.Visible = false;
            btnCalibrationCalculate.Visible = false;

            //隐藏实时显示图像和抓取图像按钮
            btnStartStopRealTimeVideo.Visible = false;
            chkDisCrossLine.Visible = true;
            btnGetImage.Visible = true;

            _calibrationStepNo = 2;

            if (btnStartStopRealTimeVideo.BackColor == Color.Green)//如果为实时状态
            {
                if (SettingsCameraCalibrationFormRequestGrabImageAndDispVideoEvent != null)
                {
                    SettingsCameraCalibrationFormRequestGrabImageAndDispVideoEvent(2, false);
                }
            }
        }

        //进行第三步操作
        private void btnMoveMarkToPosition2_Click(object sender, EventArgs e)
        {
            MyTool.TxtFileProcess.CreateLog("相机标定画面-进行标定第三步操作" + "----用户：" + BaseForm._operatorName,
          BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

            if (BaseForm._autoModeFlag)
            {
                MyTool.TxtFileProcess.CreateLog("相机标定画面-进行标定第三步操作失败，当前设备处于自动模式" + "----用户：" + BaseForm._operatorName,
                BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                MessageBox.Show("设备当前处于自动状态，无法进行相机标定！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            lblCameraCalibrationInfo.Text = _infoMsg[2];

            FinishMarkCoorSettingCtrlShowHideSetting(1, false);

            //隐藏像元计算所用控件
            lblPixelSizeResult.Visible = false;
            lblPixelColumnSizeTitle.Visible = false;
            lblPixelRowSizeTitle.Visible = false;
            txtPixelColumnSize.Visible = false;
            txtPixelRowSize.Visible = false;
            lblPixelColumnSizeUnitTitle.Visible = false;
            lblPixelRowSizeUnitTitle.Visible = false;
            btnCalibrationCalculate.Visible = false;

            //隐藏实时显示图像和抓取图像按钮
            btnStartStopRealTimeVideo.Visible = true;
            chkDisCrossLine.Visible = true;
            btnGetImage.Visible = false;

            _calibrationStepNo = 3;

            picBackCameraImageDisp.Refresh();
        }

        //进行第四步操作
        private void btnMarkPos2OnImage_Click(object sender, EventArgs e)
        {
            MyTool.TxtFileProcess.CreateLog("相机标定画面-进行标定第四步操作" + "----用户：" + BaseForm._operatorName,
            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

            if (BaseForm._autoModeFlag)
            {
                MyTool.TxtFileProcess.CreateLog("相机标定画面-进行标定第四步操作失败，当前设备处于自动模式" + "----用户：" + BaseForm._operatorName,
                BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                MessageBox.Show("设备当前处于自动状态，无法进行相机标定！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            lblCameraCalibrationInfo.Text = _infoMsg[3];

            FinishMarkCoorSettingCtrlShowHideSetting(2, true);

            //隐藏像元计算所用控件
            lblPixelSizeResult.Visible = true;
            lblPixelColumnSizeTitle.Visible = true;
            lblPixelRowSizeTitle.Visible = true;
            txtPixelColumnSize.Visible = true;
            txtPixelRowSize.Visible = true;
            lblPixelColumnSizeUnitTitle.Visible = true;
            lblPixelRowSizeUnitTitle.Visible = true;
            btnCalibrationCalculate.Visible = true;

            //隐藏实时显示图像和抓取图像按钮
            btnStartStopRealTimeVideo.Visible = false;
            chkDisCrossLine.Visible = true;
            btnGetImage.Visible = true;

            _calibrationStepNo = 4;

            if (btnStartStopRealTimeVideo.BackColor == Color.Green)//如果为实时状态
            {
                if (SettingsCameraCalibrationFormRequestGrabImageAndDispVideoEvent != null)
                {
                    SettingsCameraCalibrationFormRequestGrabImageAndDispVideoEvent(2, false);
                }
            }
        }

        #region 控件显示与否控制相关函数
        //用于设定在图上标记完孔位位置后图像坐标和机器人坐标设定控件的显示与隐藏
        //Index用于表明是第一组点位数据获取还是第二组，1-第一组，2-第二组
        private void FinishMarkCoorSettingCtrlShowHideSetting(int Index, bool ShowFlag)
        {
            lblMarkPositionInImageAndPositionOfTakePictureTitle.Visible = ShowFlag;
            lblXTitle.Visible = ShowFlag;
            lblYTitle.Visible = ShowFlag;
            lblCameraTitle.Visible = ShowFlag;
            lblDeviceTitle.Visible = ShowFlag;

            if (!ShowFlag)
            {
                txtMarkColumnPosInImage1.Visible = ShowFlag;
                txtMarkRowPosInImage1.Visible = ShowFlag;
                txtMarkDeviceXPos1.Visible = ShowFlag;
                txtMarkDeviceYPos1.Visible = ShowFlag;
                txtMarkColumnPosInImage2.Visible = ShowFlag;
                txtMarkRowPosInImage2.Visible = ShowFlag;
                txtMarkDeviceXPos2.Visible = ShowFlag;
                txtMarkDeviceYPos2.Visible = ShowFlag;
            }
            else if(Index == 1)
            {
                txtMarkColumnPosInImage1.Visible = ShowFlag;
                txtMarkRowPosInImage1.Visible = ShowFlag;
                txtMarkDeviceXPos1.Visible = ShowFlag;
                txtMarkDeviceYPos1.Visible = ShowFlag;
                txtMarkColumnPosInImage2.Visible = !ShowFlag;
                txtMarkRowPosInImage2.Visible = !ShowFlag;
                txtMarkDeviceXPos2.Visible = !ShowFlag;
                txtMarkDeviceYPos2.Visible = !ShowFlag;
            }
            else if (Index == 2)
            {
                txtMarkColumnPosInImage1.Visible = !ShowFlag;
                txtMarkRowPosInImage1.Visible = !ShowFlag;
                txtMarkDeviceXPos1.Visible = !ShowFlag;
                txtMarkDeviceYPos1.Visible = !ShowFlag;
                txtMarkColumnPosInImage2.Visible = ShowFlag;
                txtMarkRowPosInImage2.Visible = ShowFlag;
                txtMarkDeviceXPos2.Visible = ShowFlag;
                txtMarkDeviceYPos2.Visible = ShowFlag;
            }

        }

        #endregion

        #region 参数xml文件的读取与保存

        /// <summary>
        /// 用于保存数据至Xml文件
        /// </summary>
        public void SavingDataToXmlFile()
        {
            MyTool.TxtFileProcess.CreateLog("相机标定画面-保存标定参数" + "----用户：" + BaseForm._operatorName,
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

            for (int i = 0; i < pnlSettingsCameraCalibrationFormDisplay.Controls.Count; i++)
            {
                needSaveDataCtrlFlag = false;
                tempCtrlName = pnlSettingsCameraCalibrationFormDisplay.Controls[i].Name.ToString();
                try
                {
                    tempCtrlNameSuffix = tempCtrlName.Substring(0, 3);
                    switch (tempCtrlNameSuffix)
                    {
                        case "txt"://如果此控件为textbox控件
                            {
                                tempValue = "";
                                if (tempCtrlNameSuffix == "txt")//如果为TextBox控件
                                    tempValue = pnlSettingsCameraCalibrationFormDisplay.Controls[i].Text.ToString();
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
            MyTool.TxtFileProcess.CreateLog("相机标定画面-读取标定参数" + "----用户：" + BaseForm._operatorName,
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

            for (int i = 0; i < pnlSettingsCameraCalibrationFormDisplay.Controls.Count; i++)
            {
                needLoadDataCtrlFlag = false;
                tempCtrlName = pnlSettingsCameraCalibrationFormDisplay.Controls[i].Name.ToString();
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
                            {
                                needLoadDataCtrlFlag = true;
                                if (elementExistFlag)//如果这个节点存在
                                {
                                    if (tempCtrlNameSuffix == "txt")
                                        pnlSettingsCameraCalibrationFormDisplay.Controls[i].Text = tempValue[2];
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
             MyTool.TxtFileProcess.CreateLog("相机标定画面-标定数据更新" + "----用户：" + BaseForm._operatorName,
             BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

            //像元尺寸更新
            try
            {
                _pixelSize[0] = Convert.ToSingle(txtPixelColumnSize.Text);
            }
            catch
            {
                _pixelSize[0] = 0f;
                txtPixelColumnSize.Text = "0";
            }

            try
            {
                _pixelSize[1] = Convert.ToSingle(txtPixelRowSize.Text);
            }
            catch
            {
                _pixelSize[1] = 0f;
                txtPixelRowSize.Text = "0";
            }

            //mark点在图像1中的像素坐标
            try
            {
                _markPointCoorInPic1[0] = Convert.ToSingle(txtMarkColumnPosInImage1.Text);
            }
            catch
            {
                _markPointCoorInPic1[0] = 0f;
                txtMarkColumnPosInImage1.Text = "0";
            }

            try
            {
                _markPointCoorInPic1[1] = Convert.ToSingle(txtMarkRowPosInImage1.Text);
            }
            catch
            {
                _markPointCoorInPic1[1] = 0f;
                txtMarkRowPosInImage1.Text = "0";
            }

            //获取图像1时的机械坐标
            try
            {
                _takePic1Coor[0] = Convert.ToSingle(txtMarkDeviceXPos1.Text);
            }
            catch
            {
                _takePic1Coor[0] = 0f;
                txtMarkDeviceXPos1.Text = "0";
            }

            try
            {
                _takePic1Coor[1] = Convert.ToSingle(txtMarkDeviceYPos1.Text);
            }
            catch
            {
                _takePic1Coor[1] = 0f;
                txtMarkDeviceYPos1.Text = "0";
            }

            //mark点在图像2中的像素坐标
            try
            {
                _markPointCoorInPic2[0] = Convert.ToSingle(txtMarkColumnPosInImage2.Text);
            }
            catch
            {
                _markPointCoorInPic2[0] = 0f;
                txtMarkColumnPosInImage2.Text = "0";
            }

            try
            {
                _markPointCoorInPic2[1] = Convert.ToSingle(txtMarkRowPosInImage2.Text);
            }
            catch
            {
                _markPointCoorInPic2[1] = 0f;
                txtMarkRowPosInImage2.Text = "0";
            }

            //获取图像2时的机械坐标
            try
            {
                _takePic2Coor[0] = Convert.ToSingle(txtMarkDeviceXPos2.Text);
            }
            catch
            {
                _takePic2Coor[0] = 0f;
                txtMarkDeviceXPos2.Text = "0";
            }

            try
            {
                _takePic2Coor[1] = Convert.ToSingle(txtMarkDeviceYPos2.Text);
            }
            catch
            {
                _takePic2Coor[1] = 0f;
                txtMarkDeviceYPos2.Text = "0";
            }
        }

        #endregion

        //读取标定参数
        private void btnLoad_Click(object sender, EventArgs e)
        {
            MyTool.TxtFileProcess.CreateLog("相机标定画面-点击读取相机标定参数按钮" + "----用户：" + BaseForm._operatorName,
            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);
            
            LoadingDataFromXmlFile();
        }

        //保存标定数据
        private void btnSave_Click(object sender, EventArgs e)
        {
            MyTool.TxtFileProcess.CreateLog("相机标定画面-点击保存相机标定参数按钮" + "----用户：" + BaseForm._operatorName,
            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

            ParameterRefresh();
            CalculatePixelSize();

            SavingDataToXmlFile();
        }

        #region 虚拟键盘相关函数
        //textbox 鼠标双击事件
        private void TextBoxDoubleClickEvent(object sender, EventArgs e)
        {
            MyTool.TxtFileProcess.CreateLog("相机标定画面-打开软键盘" + "----用户：" + BaseForm._operatorName,
            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

            SoftNumberKeyboard.LanguageFlag = 1;
            SoftNumberKeyboard numberKeyboard = new SoftNumberKeyboard();
            numberKeyboard.StartPosition = FormStartPosition.CenterScreen;
            if (((TextBox)sender).Name == "txtMarkColumnPosInImage2" || ((TextBox)sender).Name == "txtMarkColumnPosInImage1"
                || ((TextBox)sender).Name == "txtMarkRowPosInImage2" || ((TextBox)sender).Name == "txtMarkRowPosInImage1")
                numberKeyboard.PLUSMINUSBTHIDEFLAG = true;
            else
                numberKeyboard.PLUSMINUSBTHIDEFLAG = false;
            numberKeyboard.FULLSTOPBTHIDEFLAG = false;
            numberKeyboard.SOURCESTR = ((TextBox)sender).Text;
            numberKeyboard.SENDER = sender;
            numberKeyboard.SendInputDataIncludeObj += new SENDINPUTDATAINCLUDEOBJ(GetKeyboardInputData);//添加事件
            numberKeyboard.ShowDialog();
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
            if (((TextBox)sender).Name == "txtMarkColumnPosInImage2" || ((TextBox)sender).Name == "txtMarkColumnPosInImage1"
                || ((TextBox)sender).Name == "txtMarkRowPosInImage2" || ((TextBox)sender).Name == "txtMarkRowPosInImage1")
                ((TextBox)sender).Text = MyTool.DataProcessing.TextBoxDataCheck(((TextBox)sender).Text, 0, 9999999, true, true, true, false);
            else
                ((TextBox)sender).Text = MyTool.DataProcessing.TextBoxDataCheck(((TextBox)sender).Text, -9999999, 9999999, true, true, true, false);
            ((TextBox)sender).SelectionStart = ((TextBox)sender).TextLength;
            ((TextBox)sender).SelectionLength = 0;

        }

        //Textbox数据检查函数
        private void TextBoxLeaveEvent(object sender, EventArgs e)
        {
            if (((TextBox)sender).Name == "txtMarkColumnPosInImage2" || ((TextBox)sender).Name == "txtMarkColumnPosInImage1"
                || ((TextBox)sender).Name == "txtMarkRowPosInImage2" || ((TextBox)sender).Name == "txtMarkRowPosInImage1")
                ((TextBox)sender).Text = MyTool.DataProcessing.TextBoxDataCheck(((TextBox)sender).Text, 0, 9999999, true, false, true, true);
            else
                ((TextBox)sender).Text = MyTool.DataProcessing.TextBoxDataCheck(((TextBox)sender).Text, -9999999, 9999999, true, true, true, true);
            ((TextBox)sender).SelectionStart = ((TextBox)sender).TextLength;
            ((TextBox)sender).SelectionLength = 0;
        }
        #endregion

        //在控件上点击按钮绘制圆事件
        private void picBackCameraImageDisp_MouseClick(object sender, MouseEventArgs e)
        {          
            if (_calibrationStepNo == 2 || _calibrationStepNo == 4)
            {
                int picBoxMouseClick_X = e.Location.X;
                int picBoxMouseClick_Y = e.Location.Y;

                //MyTool.TxtFileProcess.CreateLog("相机标定画面-在显示控件上绘制圆" + "----用户：" + BaseForm._operatorName,
                //BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                DrawSolidCircleOnPicBox((PictureBox)sender, picBoxMouseClick_X, picBoxMouseClick_Y, _drawCircleclr, _drawCircleRadius);
                if (_calibrationStepNo == 2)//如果为获取第一个图片的坐标
                {
                    _markPointCoorInPic1[0] = (float)picBoxMouseClick_X * (float)_cameraXPixels / (float)picBackCameraImageDisp.Width;
                    txtMarkColumnPosInImage1.Text = _markPointCoorInPic1[0].ToString("f2");
                    _markPointCoorInPic1[1] = (float)picBoxMouseClick_Y * (float)_cameraYPixels / (float)picBackCameraImageDisp.Height;
                    txtMarkRowPosInImage1.Text = _markPointCoorInPic1[1].ToString();

                    _takePic1Coor[0] = BaseForm._axisAbsolutePosition[0];
                    txtMarkDeviceXPos1.Text = _takePic1Coor[0].ToString("f2");
                    _takePic1Coor[1] = BaseForm._axisAbsolutePosition[1];
                    txtMarkDeviceYPos1.Text = _takePic1Coor[1].ToString("f2");
                }
                if (_calibrationStepNo == 4)
                {
                    _markPointCoorInPic2[0] = (float)picBoxMouseClick_X * (float)_cameraXPixels / (float)picBackCameraImageDisp.Width;
                    txtMarkColumnPosInImage2.Text = _markPointCoorInPic2[0].ToString("f2");
                    _markPointCoorInPic2[1] = (float)picBoxMouseClick_Y * (float)_cameraYPixels / (float)picBackCameraImageDisp.Height;
                    txtMarkRowPosInImage2.Text = _markPointCoorInPic2[1].ToString();

                    _takePic2Coor[0] = BaseForm._axisAbsolutePosition[0];
                    txtMarkDeviceXPos2.Text = _takePic2Coor[0].ToString("f2");
                    _takePic2Coor[1] = BaseForm._axisAbsolutePosition[1];
                    txtMarkDeviceYPos2.Text = _takePic2Coor[1].ToString("f2");
                }
            }
        }

        #region DrawSolidCircleOnPicBox:在PictureBox中画实心圆
        /// <summary>
        /// DrawSolidCircleOnPicBox:在PictureBox中画实心圆
        /// </summary>
        /// <param name="PicBox">PictureBox</param>
        /// <param name="x">float:圆心在PictureBox中X坐标</param>
        /// <param name="y">float:圆心在PictureBox中Y坐标</param>
        /// <param name="clr">Color:圆颜色</param>
        /// <param name="r">float:圆半径</param>
        /// <returns></returns>
        private void DrawSolidCircleOnPicBox(PictureBox PicBox, float x, float y, Color clr, float r)
        {
            PicBox.Refresh();//如果不Refresh的话那么之前绘制的就不会消失，会一直存在
            using (var g = PicBox.CreateGraphics())
            {
                g.FillEllipse(new SolidBrush(clr), x - r, y - r, 2 * r, 2 * r);//画实心椭圆
                g.Flush();
            }
        }
        #endregion

        //请求相机实时显示
        private void btnStartStopRealTimeVideo_Click(object sender, EventArgs e)
        {
            MyTool.TxtFileProcess.CreateLog("相机标定画面-请求相机显示实时图像" + "----用户：" + BaseForm._operatorName,
           BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

            if (BaseForm._autoModeFlag)
            {
                MyTool.TxtFileProcess.CreateLog("相机标定画面-请求相机显示实时图像失败，当前设备处于自动模式" + "----用户：" + BaseForm._operatorName,
                BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                MessageBox.Show("当前设备处于自动模式，无法进行手动控制！");
                return;
            }

            if (SettingsCameraCalibrationFormRequestGrabImageAndDispVideoEvent != null)
            {
                SettingsCameraCalibrationFormRequestGrabImageAndDispVideoEvent(2, chkDisCrossLine.Checked);
            }
        }

        //请求相机抓取并实时显示图像
        private void btnGetImage_Click(object sender, EventArgs e)
        {
            MyTool.TxtFileProcess.CreateLog("相机标定画面-请求相机抓取并显示图像" + "----用户：" + BaseForm._operatorName,
            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

            if (BaseForm._autoModeFlag)
            {
                MyTool.TxtFileProcess.CreateLog("相机标定画面-请求相机抓取并显示图像失败，当前设备处于自动模式" + "----用户：" + BaseForm._operatorName,
                BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                MessageBox.Show("当前设备处于自动模式，无法进行手动控制！");
                return;
            }

            if (SettingsCameraCalibrationFormRequestGrabImageAndDispEvent != null)
            {
                SettingsCameraCalibrationFormRequestGrabImageAndDispEvent(2, false);
            }
        }

        //显示十字线与否发生变化
        private void chkDisCrossLine_CheckedChanged(object sender, EventArgs e)
        {
            MyTool.TxtFileProcess.CreateLog("手动相机控制页面-切换显示十字线" + "----用户：" + BaseForm._operatorName,
            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

            if (btnStartStopRealTimeVideo.BackColor == Color.Green)//如果处于显示视频阶段
            {
                if (SettingsCameraCalibrationFormRequestGrabImageAndDispVideoEvent != null && !BaseForm._autoModeFlag)
                {
                    SettingsCameraCalibrationFormRequestGrabImageAndDispVideoEvent(3, chkDisCrossLine.Checked);
                }
            }
        }

        //基于设定值计算像元尺寸
        private void CalculatePixelSize()
        {
            MyTool.TxtFileProcess.CreateLog("相机标定画面-计算像元尺寸" + "----用户：" + BaseForm._operatorName,
            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

            //像元宽度
            if (_markPointCoorInPic1[0] - _markPointCoorInPic2[0] == 0)//如果为0
            {
                _pixelSize[0] = 0;
            }
            else
            {
                _pixelSize[0] = Math.Abs((_takePic2Coor[0] - _takePic1Coor[0]) / (_markPointCoorInPic1[0] - _markPointCoorInPic2[0]));
            }
            txtPixelColumnSize.Text = _pixelSize[0].ToString();

            //像元高度
            if (_markPointCoorInPic1[1] - _markPointCoorInPic2[1] == 0)//如果为0
            {
                _pixelSize[1] = 0;
            }
            else
            {
                _pixelSize[1] = Math.Abs((_takePic2Coor[1] - _takePic1Coor[1]) / (_markPointCoorInPic1[1] - _markPointCoorInPic2[1]));
            }
            txtPixelRowSize.Text = _pixelSize[1].ToString();

            MyTool.TxtFileProcess.CreateLog("相机标定画面-计算所得像元宽度为\"" + _pixelSize[0].ToString() + "\"像元高度为\"" + _pixelSize[0].ToString() + "\"" + "----用户：" + BaseForm._operatorName,
            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);
        }

        //点击计算像元尺寸按钮
        private void btnCalibrationCalculate_Click(object sender, EventArgs e)
        {
            MyTool.TxtFileProcess.CreateLog("相机标定画面-点击像元尺寸计算按钮" + "----用户：" + BaseForm._operatorName,
            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

            ParameterRefresh();
            CalculatePixelSize();
        }
    }
}
