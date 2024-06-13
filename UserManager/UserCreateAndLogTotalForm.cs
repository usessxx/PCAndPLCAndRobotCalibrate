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
using MyTool;
using System.Xml;
using XMLFile;
using SoftKeyBoard;
using System.Diagnostics;//用于stopwatch


namespace UserManager
{
    public delegate void LogInFinishDelegate(int accessLevel,string currentUserName,bool hideFormFlag);//声明委托-登录完成
    public delegate void RequestCloseDelegate();//声明委托-请求关闭

    public partial class UserCreateAndLogTotalForm : Form
    {
        /*--------------------------------------------------------------------------------------
       //Copyright (C) 2022 深圳市利器精工科技有限公司
       //版权所有
       //
       //文件名：用户创建及登录页面
       //文件功能描述：用于创建用户及登录用户的界面
       //
       //
       //创建标识：MaLi 20220419
       //
       //修改标识：MaLi 20220419 Change
       //修改描述：增加用户创建及登录页面
       //
       //------------------------------------------------------------------------------------*/
        //
        //******************************事件************************************//
        public event LogInFinishDelegate LogInFinishEvent;//声明事件-登录完成 
        public event RequestCloseDelegate requestCloseEvent;//声明事件-请求关闭
        //*************************外部可读写变量*******************************//
        public int _accessLevel = 0;//权限等级0-无任何权限（未登录），100-操作员权限，101-工程师权限，102-厂家权限
        public string _currentUserName = "";//当前用户名
        //*************************公共静态变量*******************************//
        
        //*************************内部私有变量*******************************//
        DataTable _userAndPassWordDT = new DataTable();//用户及密码DATATABLE

        ListViewGroup _factoryGroup = null; //创建厂家分组
        ListViewGroup _engineerGroup = null; //创建工程师分组
        ListViewGroup _operatorGroup = null; //创建操作员分组

        CreateNewOrEditUserForm _createNewOrEditUser = null;//新建或编辑用户窗口
        DeleteUserForm _deleteUser = null;//删除用户窗口

        string _lastTimeUserName = "3DAVI";//上次启用时的用户名

        Stopwatch _timeoutCountSW = null;//权限超时计时stopwatch

        int _accessLevelTimeoutTime = 1;//权限超时时间
        string _changeToUserNameAfterTimeout = "";//达到超时时间之后切换的目标用户名
        bool _destinationUserNameAfterTimeoutExist = false;//超时后切换成为的用户存在标志
        
        //
        //
        //

        /// <summary>
        /// UserCreateAndLogTotalForm:构造函数
        /// </summary>
        public UserCreateAndLogTotalForm()
        {
            InitializeComponent();

            _currentUserName = "";
            lblCurrentUserName.Text = _currentUserName;

            //初始化ListView的控件，添加imagelist
            ImageList imageList = new ImageList();
            imageList.ImageSize = new Size(40, 40);//在list view中显示的图标大小由这里的imageSize来决定
            try
            {
                imageList.Images.Add(Bitmap.FromFile(Directory.GetCurrentDirectory() + "\\" + "操作员.png"));
                imageList.Images.Add(Bitmap.FromFile(Directory.GetCurrentDirectory() + "\\" + "管理员.png"));
                imageList.Images.Add(Bitmap.FromFile(Directory.GetCurrentDirectory() + "\\" + "厂家.png"));
            }
            catch(Exception e)
            {
                MessageBox.Show(e.ToString());
            }
            this.lvUserDisplay.LargeImageList = imageList;

            _factoryGroup = new ListViewGroup(); //厂家分组
            _factoryGroup.HeaderAlignment = HorizontalAlignment.Left;
            _factoryGroup.Header = "厂家权限级别用户";
            _engineerGroup = new ListViewGroup(); //工程师分组
            _engineerGroup.HeaderAlignment = HorizontalAlignment.Left;
            _engineerGroup.Header = "工程师权限级别用户";
            _operatorGroup = new ListViewGroup(); //操作员分组
            _operatorGroup.HeaderAlignment = HorizontalAlignment.Left;
            _operatorGroup.Header = "操作员权限级别用户";

            lvUserDisplay.Groups.Add(_factoryGroup);
            lvUserDisplay.Groups.Add(_engineerGroup);
            lvUserDisplay.Groups.Add(_operatorGroup);
            lvUserDisplay.ShowGroups = true;

            //初始化用户及密码DATATABLE
            if(_userAndPassWordDT!=null)
            {
                _userAndPassWordDT.Dispose();
                _userAndPassWordDT = null;
            }
            _userAndPassWordDT = new DataTable();
            _userAndPassWordDT.Columns.Add("No", typeof(int));//序号列
            _userAndPassWordDT.Columns.Add("UserName", typeof(string));//用户名
            _userAndPassWordDT.Columns.Add("Password", typeof(string));//用户名
            _userAndPassWordDT.Columns.Add("AccessLevel", typeof(int));//权限等级，100-操作员权限，101-工程师权限，102-厂家权限

            if (!UserPasswordListLoading())//如果没有厂家权限项
            {
                //添加厂家权限账户及密码
                DataRow dr = _userAndPassWordDT.NewRow();
                dr[0] = (_userAndPassWordDT.Rows.Count + 1).ToString();
                dr[1] = "3DAVI";
                dr[2] = "3DAVI";
                dr[3] = 102;
                _userAndPassWordDT.Rows.Add(dr.ItemArray);
                UserPasswordListSaving();
            }
            RefreshListViewCtrlToDispUser();

            btnCreateUser.Enabled = false;
            btnEditUser.Enabled = false;
            btnDeleteUser.Enabled = false;

            //设置最初权限，根据上一次的用户名进行设定
            for (int i = 0; i < _userAndPassWordDT.Rows.Count; i++)
            {
                if (_lastTimeUserName == _userAndPassWordDT.Rows[i][1].ToString())//如果有同名的用户存在
                {
                    _accessLevel = Convert.ToInt32(_userAndPassWordDT.Rows[i][3]);
                    if (_accessLevel == 100)
                    {
                        btnCreateUser.Enabled = false;
                        btnEditUser.Enabled = true;
                        btnDeleteUser.Enabled = false;
                        txtTimeoutTime.Enabled = false;
                        cboChangeUserNameAfterTimeout.Enabled = false;
                    }
                    else if (_accessLevel > 100)
                    {
                        btnCreateUser.Enabled = true;
                        btnEditUser.Enabled = true;
                        btnDeleteUser.Enabled = true;
                        txtTimeoutTime.Enabled = true;
                        cboChangeUserNameAfterTimeout.Enabled = true;
                    }
                    _currentUserName = _lastTimeUserName;
                    lblCurrentUserName.Text = _currentUserName;
                }
            }

            //权限超时计时器初始化
            if (_timeoutCountSW != null)
            {
                _timeoutCountSW.Stop();
                _timeoutCountSW = null;
            }
            _timeoutCountSW = new Stopwatch();

            //超时时间显示
            txtTimeoutTime.Text = _accessLevelTimeoutTime.ToString();

            //向启动combobox添加可以选的用户
            cboChangeUserNameAfterTimeout.Items.Clear();
            _destinationUserNameAfterTimeoutExist = false;//想要切换成为的用户名存在标志
            int selectedIndex = -1;
            for (int i = 0; i < _userAndPassWordDT.Rows.Count; i++)
            {
                if (_userAndPassWordDT.Rows[i][3].ToString() == "100")//如果为操作员权限
                {
                    
                    cboChangeUserNameAfterTimeout.Items.Add(_userAndPassWordDT.Rows[i][1].ToString());
                    if (_userAndPassWordDT.Rows[i][1].ToString() == _changeToUserNameAfterTimeout)
                    {
                        _destinationUserNameAfterTimeoutExist = true;
                        selectedIndex = cboChangeUserNameAfterTimeout.Items.Count - 1;
                    }
                }
            }
            if (!_destinationUserNameAfterTimeoutExist)
            {
                MessageBox.Show("设定的超时后切换的用户名非法，请重新设定！");
                _changeToUserNameAfterTimeout = "";
            }
            cboChangeUserNameAfterTimeout.SelectedIndex = selectedIndex;

            //计时器启动
            tmrJudgeAccessLevelChange.Interval = 100;
            tmrJudgeAccessLevelChange.Start();
        }

        #region 用户名密码表读取保存

        //保存用户名密码表
        private void UserPasswordListSaving()
        {
            string fileDirectory = Directory.GetCurrentDirectory();
            string fileName = "User.user";
            string filePath = fileDirectory + "\\" + fileName;
            string xmlFileParentData = "UserAndPassword";//用于记录设定参数的xml文件中的父数据
            string xmlFileRootNodeName = "ArrayOfClsObjectProperties";//用于记录设定参数的xml文件中的根节点名称
            string xmlFileMainNodeName = "clsObjectProperties";//用于记录设定参数的xml文件中的主节点名称

            //获取要保存的参数值
            if (!File.Exists(filePath))
            {
                XMLFileOperation obj = new XMLFileOperation();
                obj.CreateXmlRoot(xmlFileRootNodeName);//创建声明及根节点
                obj.XmlSave(filePath);
                obj.Dispose();
                //修改根节点的属性
                XMLFileOperation.updateNodeValueOrAttribute(filePath, xmlFileRootNodeName, "xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");
                XMLFileOperation.updateNodeValueOrAttribute(filePath, xmlFileRootNodeName, "xmlns:xsd", "http://www.w3.org/2001/XMLSchema");
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
                XmlElement xmlElem = xmlDoc.CreateElement("", xmlFileRootNodeName, "");
                xmlDoc.AppendChild(xmlElem);

                //修改根节点的属性
                XmlNode node = xmlDoc.SelectSingleNode(xmlFileRootNodeName);
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

            for (int i = 0; i < _userAndPassWordDT.Rows.Count; i++)
            {
                XmlNodeList nodes = xmlDoc.DocumentElement.SelectNodes(@"/" + xmlFileRootNodeName + @"/" + xmlFileMainNodeName + @"/UserName");//获取所有的Name的值
                bool elementExistFlag = false;//用于标志是否存在此元素
                foreach (XmlNode nodeChild in nodes)
                {
                    if (nodeChild.InnerText == _userAndPassWordDT.Rows[i][1].ToString())//如果此用户名存在
                    {
                        foreach (XmlNode nodeChild2 in nodeChild.ParentNode)//返回到父级别
                        {
                            if (nodeChild2.Name == "Password")//如果为密码
                            {
                                nodeChild2.InnerText = _userAndPassWordDT.Rows[i][2].ToString();//更新密码
                            }
                            if (nodeChild2.Name == "AccessLevel")//如果为权限等级
                            {
                                nodeChild2.InnerText = _userAndPassWordDT.Rows[i][3].ToString();//更新权限等级
                            }
                        }
                        elementExistFlag = true;
                        break;
                    }
                }

                if (!elementExistFlag)//如果节点不存在
                {
                    string[] s1 = { "Parent", "UserName", "Password", "AccessLevel" };
                    string[] s2 = { xmlFileParentData, _userAndPassWordDT.Rows[i][1].ToString(), _userAndPassWordDT.Rows[i][2].ToString(), _userAndPassWordDT.Rows[i][3].ToString() };
                    XmlNode objRootNode = xmlDoc.SelectSingleNode(xmlFileRootNodeName);//或者当前节点
                    XmlElement objChildNode = xmlDoc.CreateElement(xmlFileMainNodeName);//新建新插入节点
                    objRootNode.AppendChild(objChildNode);//新插入节点Append到当前节点
                    for (int j = 0; j < s1.Length; j++)
                    {
                        XmlElement objElement = xmlDoc.CreateElement(s1[j]);//新建新插入节点的子节点
                        objElement.InnerText = s2[j];

                        objChildNode.AppendChild(objElement);//子节点Append到新插入节点
                    }
                }
            }

            //设置lastTimeUserName
            XmlNodeList lastTimeUserNameNodes = xmlDoc.DocumentElement.SelectNodes(@"/" + xmlFileRootNodeName + @"/" + xmlFileMainNodeName + @"/LastUserName");
            if (lastTimeUserNameNodes.Count == 0)//如果没有这一节点
            {
                string[] s1 = { "LastUserName" };
                string[] s2 = { _lastTimeUserName };
                XmlNode objRootNode = xmlDoc.SelectSingleNode(xmlFileRootNodeName);//或者当前节点
                XmlElement objChildNode = xmlDoc.CreateElement(xmlFileMainNodeName);//新建新插入节点
                objRootNode.AppendChild(objChildNode);//新插入节点Append到当前节点
                for (int j = 0; j < s1.Length; j++)
                {
                    XmlElement objElement = xmlDoc.CreateElement(s1[j]);//新建新插入节点的子节点
                    objElement.InnerText = s2[j];

                    objChildNode.AppendChild(objElement);//子节点Append到新插入节点
                }
            }
            else
            {
                foreach (XmlNode nodeChild in lastTimeUserNameNodes)
                {
                    nodeChild.InnerText = _lastTimeUserName;
                }
            }

            //保存timeout设置时间
            lastTimeUserNameNodes = xmlDoc.DocumentElement.SelectNodes(@"/" + xmlFileRootNodeName + @"/" + xmlFileMainNodeName + @"/TimeoutTime");
            if (lastTimeUserNameNodes.Count == 0)//如果没有这一节点
            {
                string[] s1 = { "TimeoutTime" };
                string[] s2 = { _accessLevelTimeoutTime.ToString() };
                XmlNode objRootNode = xmlDoc.SelectSingleNode(xmlFileRootNodeName);//或者当前节点
                XmlElement objChildNode = xmlDoc.CreateElement(xmlFileMainNodeName);//新建新插入节点
                objRootNode.AppendChild(objChildNode);//新插入节点Append到当前节点
                for (int j = 0; j < s1.Length; j++)
                {
                    XmlElement objElement = xmlDoc.CreateElement(s1[j]);//新建新插入节点的子节点
                    objElement.InnerText = s2[j];

                    objChildNode.AppendChild(objElement);//子节点Append到新插入节点
                }
            }
            else
            {
                foreach (XmlNode nodeChild in lastTimeUserNameNodes)
                {
                    nodeChild.InnerText = _accessLevelTimeoutTime.ToString();
                }
            }

            //保存timeout之后切换成为的用户名
            lastTimeUserNameNodes = xmlDoc.DocumentElement.SelectNodes(@"/" + xmlFileRootNodeName + @"/" + xmlFileMainNodeName + @"/ChangeToUserNameAfterTimeout");
            if (lastTimeUserNameNodes.Count == 0)//如果没有这一节点
            {
                string[] s1 = { "ChangeToUserNameAfterTimeout" };
                string[] s2 = { _changeToUserNameAfterTimeout };
                XmlNode objRootNode = xmlDoc.SelectSingleNode(xmlFileRootNodeName);//或者当前节点
                XmlElement objChildNode = xmlDoc.CreateElement(xmlFileMainNodeName);//新建新插入节点
                objRootNode.AppendChild(objChildNode);//新插入节点Append到当前节点
                for (int j = 0; j < s1.Length; j++)
                {
                    XmlElement objElement = xmlDoc.CreateElement(s1[j]);//新建新插入节点的子节点
                    objElement.InnerText = s2[j];

                    objChildNode.AppendChild(objElement);//子节点Append到新插入节点
                }
            }
            else
            {
                foreach (XmlNode nodeChild in lastTimeUserNameNodes)
                {
                    nodeChild.InnerText = _changeToUserNameAfterTimeout;
                }
            }

            xmlDoc.Save(filePath);
            xmlDoc = null;
        }

        //读取用户名密码表,返回是否找到厂家权限用户，如果没有需要自建一个
        private bool UserPasswordListLoading()
        {
            string fileDirectory = Directory.GetCurrentDirectory();
            string fileName = "User.user";
            string filePath = fileDirectory + "\\" + fileName;
            string xmlFileRootNodeName = "ArrayOfClsObjectProperties";//用于记录设定参数的xml文件中的根节点名称
            string xmlFileMainNodeName = "clsObjectProperties";//用于记录设定参数的xml文件中的主节点名称

            if (!File.Exists(filePath))
            {
                XMLFileOperation obj = new XMLFileOperation();
                obj.CreateXmlRoot(xmlFileRootNodeName);//创建声明及根节点
                obj.XmlSave(filePath);
                obj.Dispose();
                //修改根节点的属性
                XMLFileOperation.updateNodeValueOrAttribute(filePath, xmlFileRootNodeName, "xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");
                XMLFileOperation.updateNodeValueOrAttribute(filePath, xmlFileRootNodeName, "xmlns:xsd", "http://www.w3.org/2001/XMLSchema");

                UserPasswordListSaving();
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
                XmlElement xmlElem = xmlDoc.CreateElement("", xmlFileRootNodeName, "");
                xmlDoc.AppendChild(xmlElem);

                //修改根节点的属性
                XmlNode node = xmlDoc.SelectSingleNode(xmlFileRootNodeName);
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

            XmlNodeList nodes = xmlDoc.DocumentElement.SelectNodes(@"/" + xmlFileRootNodeName + @"/" + xmlFileMainNodeName + @"/UserName");
            bool findFactoryFlag = false;//当找到厂家权限时置位true
            foreach (XmlNode nodeChild in nodes)
            {
                foreach (XmlNode nodeChild2 in nodeChild.ParentNode)//返回到父级别
                {
                    if (nodeChild2.Name == "UserName")//如果为用户名
                    {
                        _userAndPassWordDT.Rows.Add();
                        _userAndPassWordDT.Rows[_userAndPassWordDT.Rows.Count - 1][0] = _userAndPassWordDT.Rows.Count;
                        _userAndPassWordDT.Rows[_userAndPassWordDT.Rows.Count - 1][1] = nodeChild2.InnerText;//用户名
                    }

                    if (nodeChild2.Name == "Password")
                    {
                        _userAndPassWordDT.Rows[_userAndPassWordDT.Rows.Count - 1][2] = nodeChild2.InnerText;//密码
                    }

                    if (nodeChild2.Name == "AccessLevel")
                    {
                        if (nodeChild2.InnerText != "100" && nodeChild2.InnerText != "101" && nodeChild2.InnerText != "102")
                        {
                            _userAndPassWordDT.Rows[_userAndPassWordDT.Rows.Count - 1][3] = 100;//权限等级
                        }
                        else
                        {
                            _userAndPassWordDT.Rows[_userAndPassWordDT.Rows.Count - 1][3] = nodeChild2.InnerText;//权限等级
                            if (nodeChild2.InnerText == "102")//如果有厂家权限
                                findFactoryFlag = true;
                        }
                    }
                }

            }

            nodes = xmlDoc.DocumentElement.SelectNodes(@"/" + xmlFileRootNodeName + @"/" + xmlFileMainNodeName + @"/LastUserName");
            if (nodes.Count == 0)//如果没有这一节点
            {
                string[] s1 = { "LastUserName" };
                string[] s2 = { _lastTimeUserName };
                XmlNode objRootNode = xmlDoc.SelectSingleNode(xmlFileRootNodeName);//或者当前节点
                XmlElement objChildNode = xmlDoc.CreateElement(xmlFileMainNodeName);//新建新插入节点
                objRootNode.AppendChild(objChildNode);//新插入节点Append到当前节点
                for (int j = 0; j < s1.Length; j++)
                {
                    XmlElement objElement = xmlDoc.CreateElement(s1[j]);//新建新插入节点的子节点
                    objElement.InnerText = s2[j];

                    objChildNode.AppendChild(objElement);//子节点Append到新插入节点
                }
            }
            else
            {
                foreach (XmlNode nodeChild in nodes)
                {
                    _lastTimeUserName = nodeChild.InnerText;
                }
            }

            //读取timeout设置时间
            nodes = xmlDoc.DocumentElement.SelectNodes(@"/" + xmlFileRootNodeName + @"/" + xmlFileMainNodeName + @"/TimeoutTime");
            if (nodes.Count == 0)//如果没有这一节点
            {
                string[] s1 = { "TimeoutTime" };
                string[] s2 = { _accessLevelTimeoutTime.ToString() };
                XmlNode objRootNode = xmlDoc.SelectSingleNode(xmlFileRootNodeName);//或者当前节点
                XmlElement objChildNode = xmlDoc.CreateElement(xmlFileMainNodeName);//新建新插入节点
                objRootNode.AppendChild(objChildNode);//新插入节点Append到当前节点
                for (int j = 0; j < s1.Length; j++)
                {
                    XmlElement objElement = xmlDoc.CreateElement(s1[j]);//新建新插入节点的子节点
                    objElement.InnerText = s2[j];

                    objChildNode.AppendChild(objElement);//子节点Append到新插入节点
                }
            }
            else
            {
                foreach (XmlNode nodeChild in nodes)
                {
                    try
                    {
                        _accessLevelTimeoutTime = Convert.ToInt32(nodeChild.InnerText);
                    }
                    catch
                    {
                        string[] s1 = { "TimeoutTime" };
                        string[] s2 = { _accessLevelTimeoutTime.ToString() };
                        XmlNode objRootNode = xmlDoc.SelectSingleNode(xmlFileRootNodeName);//或者当前节点
                        XmlElement objChildNode = xmlDoc.CreateElement(xmlFileMainNodeName);//新建新插入节点
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

            //读取timeout之后切换成为的用户名
            nodes = xmlDoc.DocumentElement.SelectNodes(@"/" + xmlFileRootNodeName + @"/" + xmlFileMainNodeName + @"/ChangeToUserNameAfterTimeout");
            if (nodes.Count == 0)//如果没有这一节点
            {
                string[] s1 = { "ChangeToUserNameAfterTimeout" };
                string[] s2 = { _changeToUserNameAfterTimeout };
                XmlNode objRootNode = xmlDoc.SelectSingleNode(xmlFileRootNodeName);//或者当前节点
                XmlElement objChildNode = xmlDoc.CreateElement(xmlFileMainNodeName);//新建新插入节点
                objRootNode.AppendChild(objChildNode);//新插入节点Append到当前节点
                for (int j = 0; j < s1.Length; j++)
                {
                    XmlElement objElement = xmlDoc.CreateElement(s1[j]);//新建新插入节点的子节点
                    objElement.InnerText = s2[j];

                    objChildNode.AppendChild(objElement);//子节点Append到新插入节点
                }
            }
            else
            {
                foreach (XmlNode nodeChild in nodes)
                {
                    _changeToUserNameAfterTimeout = nodeChild.InnerText;
                }
            }

            xmlDoc.Save(filePath);
            xmlDoc = null;

            return findFactoryFlag;
        }

        //更新listview控件用于显示用户
        private void RefreshListViewCtrlToDispUser()
        {
            lvUserDisplay.Clear();
            for (int i = 0; i < _userAndPassWordDT.Rows.Count; i++)
            {
                ListViewItem lvi = new ListViewItem();
                lvi.ImageIndex = Convert.ToInt32(_userAndPassWordDT.Rows[i][3]) % 100;
                lvi.Text = _userAndPassWordDT.Rows[i][1].ToString();
                lvi.Tag = _userAndPassWordDT.Rows[i][0];
                if (lvi.ImageIndex == 0)
                    _operatorGroup.Items.Add(lvi);
                else if (lvi.ImageIndex == 1)
                    _engineerGroup.Items.Add(lvi);
                else if (lvi.ImageIndex == 2)
                    _factoryGroup.Items.Add(lvi);
                else
                    continue;

                lvUserDisplay.Items.Add(lvi);
            }
        }

        //编辑用户名密码表
        private void UserPasswordListEdit(string sourceUsrName, string newUserName, string newPassword, string newAccessLevel)
        {
            string fileDirectory = Directory.GetCurrentDirectory();
            string fileName = "User.user";
            string filePath = fileDirectory + "\\" + fileName;
            string xmlFileParentData = "UserAndPassword";//用于记录设定参数的xml文件中的父数据
            string xmlFileRootNodeName = "ArrayOfClsObjectProperties";//用于记录设定参数的xml文件中的根节点名称
            string xmlFileMainNodeName = "clsObjectProperties";//用于记录设定参数的xml文件中的主节点名称

            //获取要保存的参数值
            if (!File.Exists(filePath))
            {
                XMLFileOperation obj = new XMLFileOperation();
                obj.CreateXmlRoot(xmlFileRootNodeName);//创建声明及根节点
                obj.XmlSave(filePath);
                obj.Dispose();
                //修改根节点的属性
                XMLFileOperation.updateNodeValueOrAttribute(filePath, xmlFileRootNodeName, "xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");
                XMLFileOperation.updateNodeValueOrAttribute(filePath, xmlFileRootNodeName, "xmlns:xsd", "http://www.w3.org/2001/XMLSchema");
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
                XmlElement xmlElem = xmlDoc.CreateElement("", xmlFileRootNodeName, "");
                xmlDoc.AppendChild(xmlElem);

                //修改根节点的属性
                XmlNode node = xmlDoc.SelectSingleNode(xmlFileRootNodeName);
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



            XmlNodeList nodes = xmlDoc.DocumentElement.SelectNodes(@"/" + xmlFileRootNodeName + @"/" + xmlFileMainNodeName + @"/UserName");//获取所有的Name的值
            bool elementExistFlag = false;//用于标志是否存在此元素
            foreach (XmlNode nodeChild in nodes)
            {
                if (nodeChild.InnerText == sourceUsrName)//如果此用户名存在
                {
                    foreach (XmlNode nodeChild2 in nodeChild.ParentNode)//返回到父级别
                    {
                        if (nodeChild2.Name == "UserName")//如果为密码
                        {
                            nodeChild2.InnerText = newUserName;//更新密码
                        }
                        if (nodeChild2.Name == "Password")//如果为密码
                        {
                            nodeChild2.InnerText = newPassword;//更新密码
                        }
                        if (nodeChild2.Name == "AccessLevel")//如果为权限等级
                        {
                            nodeChild2.InnerText = newAccessLevel;//更新权限等级
                        }
                    }
                    elementExistFlag = true;
                    break;
                }
            }

            if (!elementExistFlag)//如果节点不存在
            {
                string[] s1 = { "Parent", "UserName", "Password", "AccessLevel" };
                string[] s2 = { xmlFileParentData, newUserName, newPassword, newAccessLevel };
                XmlNode objRootNode = xmlDoc.SelectSingleNode(xmlFileRootNodeName);//或者当前节点
                XmlElement objChildNode = xmlDoc.CreateElement(xmlFileMainNodeName);//新建新插入节点
                objRootNode.AppendChild(objChildNode);//新插入节点Append到当前节点
                for (int j = 0; j < s1.Length; j++)
                {
                    XmlElement objElement = xmlDoc.CreateElement(s1[j]);//新建新插入节点的子节点
                    objElement.InnerText = s2[j];

                    objChildNode.AppendChild(objElement);//子节点Append到新插入节点
                }
            }

            xmlDoc.Save(filePath);
            xmlDoc = null;
        }

        //从密码表中删除指定用户名
        private void UserPasswordListDelete(string deleteUserName)
        {
            string fileDirectory = Directory.GetCurrentDirectory();
            string fileName = "User.user";
            string filePath = fileDirectory + "\\" + fileName;
            string xmlFileRootNodeName = "ArrayOfClsObjectProperties";//用于记录设定参数的xml文件中的根节点名称
            string xmlFileMainNodeName = "clsObjectProperties";//用于记录设定参数的xml文件中的主节点名称

            //获取要保存的参数值
            if (!File.Exists(filePath))
            {
                XMLFileOperation obj = new XMLFileOperation();
                obj.CreateXmlRoot(xmlFileRootNodeName);//创建声明及根节点
                obj.XmlSave(filePath);
                obj.Dispose();
                //修改根节点的属性
                XMLFileOperation.updateNodeValueOrAttribute(filePath, xmlFileRootNodeName, "xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");
                XMLFileOperation.updateNodeValueOrAttribute(filePath, xmlFileRootNodeName, "xmlns:xsd", "http://www.w3.org/2001/XMLSchema");
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
                XmlElement xmlElem = xmlDoc.CreateElement("", xmlFileRootNodeName, "");
                xmlDoc.AppendChild(xmlElem);

                //修改根节点的属性
                XmlNode node = xmlDoc.SelectSingleNode(xmlFileRootNodeName);
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


            XmlNodeList nodes = xmlDoc.SelectNodes(@"/" + xmlFileRootNodeName + @"/" + xmlFileMainNodeName);//获取所有的Name的值 + @"/UserName"
            XmlNode xmlnode = xmlDoc.SelectSingleNode(@"/" + xmlFileRootNodeName);
            foreach (XmlNode nodeChild in nodes)
            {
                foreach (XmlNode nodeChild2 in nodeChild.ChildNodes)//返回到父级别
                {
                    if (nodeChild2.InnerXml == deleteUserName)
                    {
                        xmlnode.RemoveChild(nodeChild);
                        break;
                    }
                }
      
            }


            xmlDoc.Save(filePath);
            xmlDoc = null;
        }

        #endregion

        //点击登录按钮
        private void btnLogIn_Click(object sender, EventArgs e)
        {
            try
            {
                if ((int)lvUserDisplay.SelectedItems[0].Tag > 0 && (int)lvUserDisplay.SelectedItems[0].Tag <= _userAndPassWordDT.Rows.Count)
                {
                    if (txtPasswordInput.Text == _userAndPassWordDT.Rows[(int)lvUserDisplay.SelectedItems[0].Tag - 1][2].ToString())//如果密码相同
                    {
                        if (LogInFinishEvent == null)
                        {
                            MessageBox.Show("登录用户\"" + _userAndPassWordDT.Rows[(int)lvUserDisplay.SelectedItems[0].Tag - 1][1].ToString() + "\"成功！");
                        }
                        _accessLevel = (int)_userAndPassWordDT.Rows[(int)lvUserDisplay.SelectedItems[0].Tag - 1][3];
                        if (_accessLevel == 100)
                        {
                            btnCreateUser.Enabled = false;
                            btnEditUser.Enabled = true;
                            btnDeleteUser.Enabled = false;
                            txtTimeoutTime.Enabled = false;
                            cboChangeUserNameAfterTimeout.Enabled = false;
                        }
                        else if (_accessLevel > 100)
                        {
                            btnCreateUser.Enabled = true;
                            btnEditUser.Enabled = true;
                            btnDeleteUser.Enabled = true;
                            txtTimeoutTime.Enabled = true;
                            cboChangeUserNameAfterTimeout.Enabled = true;
                        }

                        _currentUserName = _userAndPassWordDT.Rows[(int)lvUserDisplay.SelectedItems[0].Tag - 1][1].ToString();
                        lblCurrentUserName.Text = _currentUserName;
                        _lastTimeUserName = _currentUserName;
                        txtPasswordInput.Text = "";
                    }
                    else
                    {
                        MessageBox.Show("密码不正确，登录用户\"" + _userAndPassWordDT.Rows[(int)lvUserDisplay.SelectedItems[0].Tag - 1][1].ToString() + "\"失败！");
                    }

                    if (LogInFinishEvent != null)
                    {
                        LogInFinishEvent(_accessLevel, _currentUserName, true);
                    }
                }
                else
                {
                    MessageBox.Show("请选中想要登录的用户！");
                }
            }
            catch
            { }
        }

        //新建用户
        private void btnCreateUser_Click(object sender, EventArgs e)
        {
            //初始化创建新用户界面变量
            if (_createNewOrEditUser != null)
            {
                _createNewOrEditUser.Dispose();
                _createNewOrEditUser = null;
            }
            _createNewOrEditUser = new CreateNewOrEditUserForm(_accessLevel, "", "", 0, true);
            _createNewOrEditUser.CreateNewUserEvent += CreateNewUserEventFunc;
            _createNewOrEditUser.StartPosition = FormStartPosition.CenterParent;
            _createNewOrEditUser.Show();
        }

        //新建用户事件函数
        private void CreateNewUserEventFunc(string userName, string password, string accessLevel, bool giveUpFlag)
        {
            if (!giveUpFlag)
            {
                foreach (DataRow dr in _userAndPassWordDT.Rows)
                {
                    if (dr[1].ToString() == userName)//如果现有用户中有此用户了
                    {
                        MessageBox.Show("用户\"" + userName + "\"已存在，创建失败！");
                        _createNewOrEditUser.Dispose();
                        _createNewOrEditUser = null;
                        return;
                    }
                }

                _userAndPassWordDT.Rows.Add();
                _userAndPassWordDT.Rows[_userAndPassWordDT.Rows.Count - 1][0] = _userAndPassWordDT.Rows.Count;
                _userAndPassWordDT.Rows[_userAndPassWordDT.Rows.Count - 1][1] = userName;
                _userAndPassWordDT.Rows[_userAndPassWordDT.Rows.Count - 1][2] = password;
                _userAndPassWordDT.Rows[_userAndPassWordDT.Rows.Count - 1][3] = accessLevel;
                UserPasswordListSaving();
                RefreshListViewCtrlToDispUser();
   
                //向启动combobox添加可以选的用户
                cboChangeUserNameAfterTimeout.Items.Clear();
                _destinationUserNameAfterTimeoutExist = false;//想要切换成为的用户名存在标志
                int selectedIndex = -1;
                for (int i = 0; i < _userAndPassWordDT.Rows.Count; i++)
                {
                    if (_userAndPassWordDT.Rows[i][3].ToString() == "100")//如果为操作员权限
                    {

                        cboChangeUserNameAfterTimeout.Items.Add(_userAndPassWordDT.Rows[i][1].ToString());
                        if (_userAndPassWordDT.Rows[i][1].ToString() == _changeToUserNameAfterTimeout)
                        {
                            _destinationUserNameAfterTimeoutExist = true;
                            selectedIndex = cboChangeUserNameAfterTimeout.Items.Count - 1;
                        }
                    }
                }
                if (!_destinationUserNameAfterTimeoutExist)
                {
                    MessageBox.Show("设定的超时后切换的用户名非法，请重新设定！");
                    _changeToUserNameAfterTimeout = "";
                }
                cboChangeUserNameAfterTimeout.SelectedIndex = selectedIndex;
            }
            else
            {
                MessageBox.Show("放弃创建新用户！");
            }

            _createNewOrEditUser.Dispose();
            _createNewOrEditUser = null;
        }

        //编辑当前用户密码
        private void btnEditUser_Click(object sender, EventArgs e)
        {
            int currentUserIndex = 0;
            for (int i = 0; i < _userAndPassWordDT.Rows.Count; i++)
            {
                if (_currentUserName == _userAndPassWordDT.Rows[i][1].ToString())
                {
                    currentUserIndex = i;
                    break;
                }
            }
            //初始化编辑户界面变量
            if (_createNewOrEditUser != null)
            {
                _createNewOrEditUser.Dispose();
                _createNewOrEditUser = null;
            }
            _createNewOrEditUser = new CreateNewOrEditUserForm(_accessLevel, _userAndPassWordDT.Rows[currentUserIndex][1].ToString(),
                _userAndPassWordDT.Rows[currentUserIndex][2].ToString(), currentUserIndex, false);
            _createNewOrEditUser.EditUserEvent += EditUserEventFunc;
            _createNewOrEditUser.StartPosition = FormStartPosition.CenterParent;
            _createNewOrEditUser.Text = "编辑用户";
            _createNewOrEditUser.Show();
        }

        //编辑用户事件
        private void EditUserEventFunc(string userName, string password, string accessLevel, int userIndex, bool giveUpFlag)
        {
            if (userIndex >= _userAndPassWordDT.Rows.Count)
            {
                _createNewOrEditUser.Dispose();
                _createNewOrEditUser = null;
                return;
            }
            if (!giveUpFlag)
            {
                if (userIndex >= _userAndPassWordDT.Rows.Count)
                {
                    _createNewOrEditUser.Dispose();
                    _createNewOrEditUser = null;
                    return;
                }
                if (_userAndPassWordDT.Rows[userIndex][3].ToString() == "102")
                    accessLevel = "102";

                UserPasswordListEdit(_userAndPassWordDT.Rows[userIndex][1].ToString(), userName, password, accessLevel);

                _userAndPassWordDT.Rows.Clear();
                if (!UserPasswordListLoading())
                {
                    //添加厂家权限账户及密码
                    DataRow dr = _userAndPassWordDT.NewRow();
                    dr[0] = (_userAndPassWordDT.Rows.Count + 1).ToString();
                    dr[1] = "3DAVI";
                    dr[2] = "3DAVI";
                    dr[3] = 102;
                    _userAndPassWordDT.Rows.Add(dr.ItemArray);
                    UserPasswordListSaving();
                }
                RefreshListViewCtrlToDispUser();

                _accessLevel = Convert.ToInt32(accessLevel);
                if (_accessLevel == 100)
                {
                    btnCreateUser.Enabled = false;
                    btnEditUser.Enabled = true;
                    btnDeleteUser.Enabled = false;
                    txtTimeoutTime.Enabled = false;
                    cboChangeUserNameAfterTimeout.Enabled = false;
                }
                else if (_accessLevel > 100)
                {
                    btnCreateUser.Enabled = true;
                    btnEditUser.Enabled = true;
                    btnDeleteUser.Enabled = true;
                    txtTimeoutTime.Enabled = true;
                    cboChangeUserNameAfterTimeout.Enabled = true;
                }

                _currentUserName = userName;
                lblCurrentUserName.Text = _currentUserName;
                _lastTimeUserName = _currentUserName;

                //向启动combobox添加可以选的用户
                cboChangeUserNameAfterTimeout.Items.Clear();
                _destinationUserNameAfterTimeoutExist = false;//想要切换成为的用户名存在标志
                int selectedIndex = -1;
                for (int i = 0; i < _userAndPassWordDT.Rows.Count; i++)
                {
                    if (_userAndPassWordDT.Rows[i][3].ToString() == "100")//如果为操作员权限
                    {

                        cboChangeUserNameAfterTimeout.Items.Add(_userAndPassWordDT.Rows[i][1].ToString());
                        if (_userAndPassWordDT.Rows[i][1].ToString() == _changeToUserNameAfterTimeout)
                        {
                            _destinationUserNameAfterTimeoutExist = true;
                            selectedIndex = cboChangeUserNameAfterTimeout.Items.Count - 1;
                        }
                    }
                }
                if (!_destinationUserNameAfterTimeoutExist)
                {
                    MessageBox.Show("设定的超时后切换的用户名非法，请重新设定！");
                    _changeToUserNameAfterTimeout = "";
                }
                cboChangeUserNameAfterTimeout.SelectedIndex = selectedIndex;
            }
            else
            {
                MessageBox.Show("放弃用户编辑！");
            }

            _createNewOrEditUser.Dispose();
            _createNewOrEditUser = null;
        }

        //删除用户
        private void btnDeleteUser_Click(object sender, EventArgs e)
        {
            //初始化删除用户界面变量
            if (_deleteUser != null)
            {
                _deleteUser.Dispose();
                _deleteUser = null;
            }
            _deleteUser = new DeleteUserForm(_accessLevel, _userAndPassWordDT);
            _deleteUser.DeleteUserEvent += DeleteUserEventFunc;
            _deleteUser.StartPosition = FormStartPosition.CenterParent;
            _deleteUser.Show();
        }

        //删除用户事件
        private void DeleteUserEventFunc(string deleteUserName, bool giveUpFlag)
        {
            if (!giveUpFlag)//如果没有放弃删除用户
            {
                UserPasswordListDelete(deleteUserName);
                _userAndPassWordDT.Rows.Clear();
                if (!UserPasswordListLoading())
                {
                    //添加厂家权限账户及密码
                    DataRow dr = _userAndPassWordDT.NewRow();
                    dr[0] = (_userAndPassWordDT.Rows.Count + 1).ToString();
                    dr[1] = "3DAVI";
                    dr[2] = "3DAVI";
                    dr[3] = 102;
                    _userAndPassWordDT.Rows.Add(dr.ItemArray);
                    UserPasswordListSaving();
                }
                RefreshListViewCtrlToDispUser();

                //向启动combobox添加可以选的用户
                cboChangeUserNameAfterTimeout.Items.Clear();
                _destinationUserNameAfterTimeoutExist = false;//想要切换成为的用户名存在标志
                int selectedIndex = -1;
                for (int i = 0; i < _userAndPassWordDT.Rows.Count; i++)
                {
                    if (_userAndPassWordDT.Rows[i][3].ToString() == "100")//如果为操作员权限
                    {

                        cboChangeUserNameAfterTimeout.Items.Add(_userAndPassWordDT.Rows[i][1].ToString());
                        if (_userAndPassWordDT.Rows[i][1].ToString() == _changeToUserNameAfterTimeout)
                        {
                            _destinationUserNameAfterTimeoutExist = true;
                            selectedIndex = cboChangeUserNameAfterTimeout.Items.Count - 1;
                        }
                    }
                }
                if (!_destinationUserNameAfterTimeoutExist)
                {
                    MessageBox.Show("设定的超时后切换的用户名非法，请重新设定！");
                    _changeToUserNameAfterTimeout = "";
                }
                cboChangeUserNameAfterTimeout.SelectedIndex = selectedIndex;
            }
            else
            {
                MessageBox.Show("放弃删除用户！");
            }

            _deleteUser.Dispose();
            _deleteUser = null;
        }

        //关闭窗口
        private void UserCreateAndLogTotalForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (requestCloseEvent != null)
            {
                e.Cancel = true;
                requestCloseEvent();
            }
        }

        #region 虚拟键盘相关函数
        //textbox 鼠标双击事件
        private void TextBoxDoubleClickEvent(object sender, EventArgs e)
        {
            if (((TextBox)sender).Name != "txtTimeoutTime")
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
                numberKeyboard.PLUSMINUSBTHIDEFLAG = true;
                numberKeyboard.FULLSTOPBTHIDEFLAG = true;
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
            if (((TextBox)sender).Name == "txtTimeoutTime")
            {
                ((TextBox)sender).Text = MyTool.DataProcessing.TextBoxDataCheck(((TextBox)sender).Text, 1, 10000, true, true, false, false);
                ((TextBox)sender).SelectionStart = ((TextBox)sender).TextLength;
                ((TextBox)sender).SelectionLength = 0;
            }
        }

        //Textbox数据检查函数
        private void TextBoxLeaveEvent(object sender, EventArgs e)
        {
            if (((TextBox)sender).Name == "txtTimeoutTime")
            {
                ((TextBox)sender).Text = MyTool.DataProcessing.TextBoxDataCheck(((TextBox)sender).Text, 1, 10000, true, true, false, true);
                ((TextBox)sender).SelectionStart = ((TextBox)sender).TextLength;
                ((TextBox)sender).SelectionLength = 0;

                _accessLevelTimeoutTime = Convert.ToInt32(((TextBox)sender).Text);
                UserPasswordListSaving();
            }         
        }
        #endregion

        //timer:用于判定是否应当变换权限
        private void tmrJudgeAccessLevelChange_Tick(object sender, EventArgs e)
        {
            if (_accessLevel == 101 || _accessLevel == 102)//如果当前权限为工程师或厂家
            {
                if (!_timeoutCountSW.IsRunning)
                {
                    _timeoutCountSW.Reset();
                    _timeoutCountSW.Start();
                }
                if (Convert.ToInt32(_timeoutCountSW.ElapsedMilliseconds) >= _accessLevelTimeoutTime * 1000)//如果即使时间大于超时设定时间
                {
                    bool findDestinatedUser = false;
                    for (int i = 0; i < _userAndPassWordDT.Rows.Count; i++)
                    {
                        if (_userAndPassWordDT.Rows[i][1].ToString() == _changeToUserNameAfterTimeout)
                        {
                            _accessLevel = Convert.ToInt32(_userAndPassWordDT.Rows[i][3]);
                            _currentUserName = _changeToUserNameAfterTimeout;
                            _lastTimeUserName = _changeToUserNameAfterTimeout;
                            if (_accessLevel == 100)
                            {
                                btnCreateUser.Enabled = false;
                                btnEditUser.Enabled = true;
                                btnDeleteUser.Enabled = false;
                                txtTimeoutTime.Enabled = false;
                                cboChangeUserNameAfterTimeout.Enabled = false;
                            }
                            else if (_accessLevel > 100)
                            {
                                btnCreateUser.Enabled = true;
                                btnEditUser.Enabled = true;
                                btnDeleteUser.Enabled = true;
                                txtTimeoutTime.Enabled = true;
                                cboChangeUserNameAfterTimeout.Enabled = true;
                            }
                            lblCurrentUserName.Text = _currentUserName;
                            if (LogInFinishEvent != null)
                            {
                                LogInFinishEvent(_accessLevel, _currentUserName, false);
                            }
                            findDestinatedUser = true;
                            break;
                        }
                    }

                    if (!findDestinatedUser)
                    {
                        //MessageBox.Show("无法找到超时后切换的用户名，请设置！");
                    }

                    _timeoutCountSW.Reset();
                }
            }
            else
            {
                if (_timeoutCountSW.IsRunning)
                {
                    _timeoutCountSW.Reset();
                    _timeoutCountSW.Stop();
                }
            }
        }

        //combo box的选项发生变化时
        private void cboChangeUserNameAfterTimeout_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboChangeUserNameAfterTimeout.Items.Count > 0 && cboChangeUserNameAfterTimeout.SelectedIndex != -1)
            {
                _changeToUserNameAfterTimeout = cboChangeUserNameAfterTimeout.Items[cboChangeUserNameAfterTimeout.SelectedIndex].ToString();
                UserPasswordListSaving();
            }
        }

        
    }
}
