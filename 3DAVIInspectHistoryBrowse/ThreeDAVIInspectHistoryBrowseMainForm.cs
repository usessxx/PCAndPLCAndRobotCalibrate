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
using UserManager;
using System.Threading;

namespace ThreeDAVIInspectHistoryBrowse
{
    #region 构造体
    public struct YIELD
    {
        public int _totalYield;
        public int _NGYield;
        public int _OKYield;
    }
    #endregion

    public delegate void InspectHistoryBrowseFormRequestCloseDelegate();//声明委托，检测历史记录界面请求关闭
    public delegate void SPCFormLogInFinishDelegate(int accessLevel, string userName, bool hideFormFlag);//声明委托，SPC界面登录完成委托，为了与baseform的UserManger联系起来
    public delegate void SPCFormUserManagerFormCloseDelegate();//声明委托，SPC界面用户管理界面关闭委托，为了与baseform的UserManger联系起来

    public partial class ThreeDAVIInspectHistoryBrowseMainForm : Form
    {
        /*--------------------------------------------------------------------------------------
        //Copyright (C) 2022 深圳市利器精工科技有限公司
        //版权所有
        //
        //文件名：检测历史记录浏览基础界面
        //文件功能描述：检测历史记录浏览界面的基础界面
        //
        //
        //创建标识：MaLi 20220515
        //
        //修改标识：MaLi 20220515 Change
        //修改描述：增肌检测历史记录浏览基础界面
        //
        //------------------------------------------------------------------------------------*/
        //
        /*******************************事件**************************************/
        public event InspectHistoryBrowseFormRequestCloseDelegate InspectHistoryBrowseFormRequestCloseEvent;//声明事件，检测历史记录界面请求关闭

        public event SPCFormLogInFinishDelegate SPCFormLogInFinishEvent;//声明事件，SPC界面登录完成事件，为了与baseform的UserManger联系起来
        public event SPCFormUserManagerFormCloseDelegate SPCFormUserManagerFormCloseEvent;//声明事件，SPC界面用户管理界面关闭事件，为了与baseform的UserManger联系起来
        /*************************外部可设定读取参数*******************************/
        public string _normalProductResultSavePath = @"E:\3D AOI 生产数据\数据文件";//常规产品检测结果保存路径
        public string _samplePanelResultSavePath = @"E:\3D AOI 生产数据\数据文件\样品板数据";//样品板检测结果保存路径
        //*************************公共静态变量*******************************//

        //*************************内部私有变量*******************************//
        bool _dispOKProductFlag = true;//显示OK结果的产品标志，false-不显示，true-显示
        bool _dispNGProductFlag = true;//显示NG结果的产品标志，false-不显示，true-显示
        DataTable _searchResultDataTable = null;//搜索结果datatable
        YIELD _searchResultPcsYield = new YIELD();//搜索结果PC产量
        YIELD _searchResultPanelYield = new YIELD();//搜索结果Panel产量
        YIELD _searchResultElementYield = new YIELD();//搜索结果元件产量

        string _dispInspectResultProductPath = "";//显示检测结果的产品文件夹路径

        ImageDispForm _picDispFormVariate = null;//图片显示界面

        /// <summary>
        /// _userManagerVariate:用户管理登录页面变量
        /// </summary>
        UserCreateAndLogTotalForm _userManagerVariate = null;
        /// <summary>
        /// _operatorName:当前操作人员名称
        /// </summary>
        public string _operatorName = "";
        /// <summary>
        /// _accessLevel:权限等级0-未登录，100-操作员权限，101-工程师权限，102-厂家权限
        /// </summary>
        public int _accessLevel = 0;

        //
        //
        public ThreeDAVIInspectHistoryBrowseMainForm()
        {
            InitializeComponent();

            //用户登录编辑页面
            if (_userManagerVariate != null)
            {
                _userManagerVariate.Dispose();
                _userManagerVariate = null;
            }
            _userManagerVariate = new UserCreateAndLogTotalForm();
            _userManagerVariate.StartPosition = FormStartPosition.CenterParent;
            _accessLevel = _userManagerVariate._accessLevel;
            _operatorName = _userManagerVariate._currentUserName;
            if (_accessLevel == 0)
            {
                lblAccessLevel.Text = "未登录";
                lblAccessLevel.ForeColor = Color.Black;
            }
            else if (_accessLevel == 100)
            {
                lblAccessLevel.Text = "操作员";
                lblAccessLevel.ForeColor = Color.LimeGreen;
            }
            else if (_accessLevel == 101)
            {
                lblAccessLevel.Text = "工程师";
                lblAccessLevel.ForeColor = Color.Orange;
            }
            else if (_accessLevel == 102)
            {
                lblAccessLevel.Text = "厂家";
                lblAccessLevel.ForeColor = Color.Red;
            }
            lblUserName.Text = _operatorName;
            _userManagerVariate.LogInFinishEvent += UserFormUserLogInFinishEventFunc;
            _userManagerVariate.requestCloseEvent += UserFormRequestCloseEventFunc;
        }

        private void ThreeDAVIInspectHistoryBrowseMainForm_Load(object sender, EventArgs e)
        {
            #region 初始化查询时间范围的设定
            rbtnSearchRegionSettime.Checked = true;//初始化，选中设定搜索时间
            dtpSearchStopDateTime.Value = DateTime.Now;
            dtpSearchStopHourTime.Text = "23:59:59";
            dtpSearchStartDateTime.Value = DateTime.Now;
            dtpSearchStartHourTime.Text = "00:00:00";
            #endregion

            #region 初始化路径及数据筛选
            txtNormalProductCheckResultSavePath.Text = _normalProductResultSavePath;
            txtSamplePanelCheckResultSavePath.Text = _samplePanelResultSavePath;
            #endregion

            if (_dispOKProductFlag)//如果显示OK产品
            {
                btnSearchKindOKProduct.BackColor = SystemColors.Control;
            }
            else
            {
                btnSearchKindOKProduct.BackColor = SystemColors.ButtonShadow;
            }

            if (_dispNGProductFlag)//如果显示NG产品
            {
                btnSearchKindNGProduct.BackColor = SystemColors.Control;
            }
            else
            {
                btnSearchKindNGProduct.BackColor = SystemColors.ButtonShadow;
            }

            //启动计时器
            tmrRefreshTime.Interval = 1000;
            tmrRefreshTime.Start();

            //初始化产量显示DataGridView控件
            dgrdvPassRateRecord.RowTemplate.Height = 35;
            dgrdvPassRateRecord.Rows.Clear();
            for (int i = 0; i < 4; i++)
            {
                dgrdvPassRateRecord.Rows.Add();
            }

            //产量显示选项初始化
            rbtnPcsYield.Checked = true;
            DispPassRate();

            //检测结果显示初始化
            rbtnPCSResult.Checked = true;

            this.WindowState = FormWindowState.Maximized;

            lblSelectedProductName.Text = "名称: NULL";

            //检测文件夹与否初始化
            chkCheckNormalProduct.Checked = true;
            chkCheckSampleProduct.Checked = false;

            pnlSearching.Visible = false;
            lblSearching.Visible = false;
        }

        //3D AVI检测记录浏览界面关闭事件，如果有外部调用，使得此界面为某个界面的子界面，那么就反馈事件，如果不是，那么就关闭
        private void ThreeDAVIInspectHistoryBrowseMainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (InspectHistoryBrowseFormRequestCloseEvent != null)
            {
                e.Cancel = true;
                InspectHistoryBrowseFormRequestCloseEvent();
            }
            else
            {
                e.Cancel = false;
            }
        }

        #region 查询时间范围的设定

        //选择的查询区间发生变化事件
        private void SelectSearchRegionChangeEvent(object sender, EventArgs e)
        {
            DateTime startTime = new DateTime();
            switch (((RadioButton)sender).Name)
            {
                case "rbtnSearchRegionOneDay":
                    dtpSearchStartHourTime.Enabled = false;
                    dtpSearchStopHourTime.Enabled = false;
                    startTime = DateTime.Now;
                    break;
                case "rbtnSearchRegionOneWeek":
                    dtpSearchStartHourTime.Enabled = false;
                    dtpSearchStopHourTime.Enabled = false;

                    startTime = DateTime.Now.AddDays(-6);
                    break;
                case "rbtnSearchRegionOneMonth":
                    dtpSearchStartHourTime.Enabled = false;
                    dtpSearchStopHourTime.Enabled = false;

                    startTime = DateTime.Now.AddMonths(-1);
                    break;
                case "rbtnSearchRegionSettime":
                    dtpSearchStartHourTime.Enabled = true;
                    dtpSearchStopHourTime.Enabled = true;
                    break;
                default:
                    return;
            }

            if (!dtpSearchStartHourTime.Enabled)
            {
                dtpSearchStartDateTime.Text = startTime.ToString("yyyy/MM/dd");
                dtpSearchStartHourTime.Text = "00:00:00";

                dtpSearchStopDateTime.Text = DateTime.Now.ToString("yyyy/MM/dd");
                dtpSearchStopHourTime.Text = "23:59:59";
            }

        }

        //当datetimepicker控件的值发生变化时进行检测功能
        private void DateTimePickerControlValueChangedDataCheckEvent(object sender, EventArgs e)
        {
            //检查数据是否超出上限，即当前日期
            DateTime tempSelectTime = new DateTime(((DateTimePicker)sender).Value.Year, ((DateTimePicker)sender).Value.Month, ((DateTimePicker)sender).Value.Day, 0, 0, 0);
            DateTime tempCurrentTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
            if (tempSelectTime.CompareTo(tempCurrentTime) > 0)
            {
                ((DateTimePicker)sender).Value = tempCurrentTime;
            }

            //根据选中一天还是一周还是一月确定上限
            if (rbtnSearchRegionOneDay.Checked)//如果选中一天
            {
                if (((DateTimePicker)sender).Name == "dtpSearchStartDateTime")//如果为查询开始时间
                {
                    DateTime newStopTime = new DateTime(((DateTimePicker)sender).Value.Year, ((DateTimePicker)sender).Value.Month, ((DateTimePicker)sender).Value.Day, 0, 0, 0);
                    if (dtpSearchStopDateTime.Value != newStopTime)
                        dtpSearchStopDateTime.Value = newStopTime;
                }
                else
                {
                    DateTime newStartTime = new DateTime(((DateTimePicker)sender).Value.Year, ((DateTimePicker)sender).Value.Month, ((DateTimePicker)sender).Value.Day, 0, 0, 0);
                    if (dtpSearchStartDateTime.Value != newStartTime)
                        dtpSearchStartDateTime.Value = newStartTime;
                }
                return;//不做任何处理，选中一天时上限就是当天
            }
            else if (rbtnSearchRegionOneWeek.Checked)//如果选中一周
            {
                if (((DateTimePicker)sender).Name == "dtpSearchStartDateTime")//如果为查询开始时间
                {
                    DateTime oneWeekLimitTime = new DateTime(DateTime.Now.AddDays(-6).Year, DateTime.Now.AddDays(-6).Month, DateTime.Now.AddDays(-6).Day, 0, 0, 0);
                    if (tempSelectTime.CompareTo(oneWeekLimitTime) > 0)
                    {
                        ((DateTimePicker)sender).Value = oneWeekLimitTime;
                    }

                    DateTime newStopTime = new DateTime(((DateTimePicker)sender).Value.AddDays(6).Year, ((DateTimePicker)sender).Value.AddDays(6).Month, ((DateTimePicker)sender).Value.AddDays(6).Day, 0, 0, 0);
                    if (dtpSearchStopDateTime.Value != newStopTime)
                        dtpSearchStopDateTime.Value = newStopTime;
                }
                else
                {
                    DateTime newStartTime = new DateTime(((DateTimePicker)sender).Value.AddDays(-6).Year, ((DateTimePicker)sender).Value.AddDays(-6).Month, ((DateTimePicker)sender).Value.AddDays(-6).Day, 0, 0, 0);
                    if (dtpSearchStartDateTime.Value != newStartTime)
                        dtpSearchStartDateTime.Value = newStartTime;
                }
            }
            else if (rbtnSearchRegionOneMonth.Checked)//如果选中一月
            {
                if (((DateTimePicker)sender).Name == "dtpSearchStartDateTime")//如果为查询开始时间
                {
                    DateTime oneMonthLimitTime = new DateTime(DateTime.Now.AddMonths(-1).Year, DateTime.Now.AddMonths(-1).Month, DateTime.Now.AddMonths(-1).Day, 0, 0, 0);
                    if (tempSelectTime.CompareTo(oneMonthLimitTime) > 0)
                    {
                        ((DateTimePicker)sender).Value = oneMonthLimitTime;
                    }

                    DateTime newStopTime = new DateTime(((DateTimePicker)sender).Value.AddMonths(1).Year, ((DateTimePicker)sender).Value.AddMonths(1).Month, ((DateTimePicker)sender).Value.AddMonths(1).Day, 0, 0, 0);
                    if (dtpSearchStopDateTime.Value != newStopTime)
                        dtpSearchStopDateTime.Value = newStopTime;
                }
                else
                {
                    DateTime newStartTime = new DateTime(((DateTimePicker)sender).Value.AddMonths(-1).Year, ((DateTimePicker)sender).Value.AddMonths(-1).Month, ((DateTimePicker)sender).Value.AddMonths(-1).Day, 0, 0, 0);
                    if (dtpSearchStartDateTime.Value != newStartTime)
                        dtpSearchStartDateTime.Value = newStartTime;
                }
            }
            else if (rbtnSearchRegionSettime.Checked)//如果选中的是指定时间
            {
                if (((DateTimePicker)sender).Name == "dtpSearchStartDateTime")//如果为查询开始时间
                {
                    if (((DateTimePicker)sender).Value.CompareTo(dtpSearchStopDateTime.Value) > 0)//如果设定的查询开始时间大于查询结束时间
                    {
                        ((DateTimePicker)sender).Value = dtpSearchStopDateTime.Value;
                    }
                }
                else
                {
                    if (((DateTimePicker)sender).Value.CompareTo(dtpSearchStartDateTime.Value) < 0)//如果设定的查询开始时间大于查询结束时间
                    {
                        ((DateTimePicker)sender).Value = dtpSearchStartDateTime.Value;
                    }
                }
            }
        }

        #endregion

        #region 路径及数据筛选设定

        //获取路径事件函数
        private void DirectoryBrowseEvent(object sender, EventArgs e)
        {
            FolderBrowserDialog fb = new FolderBrowserDialog();
            string name = ((Button)sender).Name;
            switch (name)
            {
                case "btnBrowseNormalProductCheckResultSavePath":
                    {
                        fb.SelectedPath = txtNormalProductCheckResultSavePath.Text;
                        break;
                    }
                case "btnBrowseSamplePanelCheckResultSavePath":
                    {
                        fb.SelectedPath = txtSamplePanelCheckResultSavePath.Text;
                        break;
                    }
            }
            if (fb.ShowDialog() == DialogResult.OK)
            {
                switch (name)
                {

                    case "btnBrowseNormalProductCheckResultSavePath":
                        {
                            txtNormalProductCheckResultSavePath.Text = fb.SelectedPath;
                            _normalProductResultSavePath = txtNormalProductCheckResultSavePath.Text;
                            break;
                        }
                    case "btnBrowseSamplePanelCheckResultSavePath":
                        {
                            txtSamplePanelCheckResultSavePath.Text = fb.SelectedPath;
                            _samplePanelResultSavePath = txtSamplePanelCheckResultSavePath.Text;
                            break;
                        }
                }
            }
        }

        //产品名筛选checkbox
        private void chkProductNameFilter_CheckedChanged(object sender, EventArgs e)
        {
            if (chkProductNameFilter.Checked)
            {
                cboProductName.Enabled = true;
                cboProductName_Click(new object(), new EventArgs());
            }
            else
            {
                cboProductName.Enabled = false;
            }
        }

        //二维码筛选checkbox
        private void chkBarcodeFilter_CheckedChanged(object sender, EventArgs e)
        {
            if (chkBarcodeFilter.Checked)
            {
                txtBarcodeFilterBarcode.Enabled = true;
            }
            else
            {
                txtBarcodeFilterBarcode.Enabled = false;
            }
        }

        //点击了combo box那么更新一下combobox的items
        private void cboProductName_Click(object sender, EventArgs e)
        {
            List<string> tempProductFolderPath = new List<string>();
            if (Directory.Exists(_normalProductResultSavePath))
            {
                tempProductFolderPath = MyTool.FolderAndFileManageClass.GetFolderPathInAssignedFolder(_normalProductResultSavePath);
            
                cboProductName.Items.Clear();
                foreach (string path in tempProductFolderPath)
                {
                    if (path != _samplePanelResultSavePath)
                    {
                        cboProductName.Items.Add(path.Substring(path.LastIndexOf("\\") + 1));
                    }
                }
            }

            if (Directory.Exists(_samplePanelResultSavePath))
            {
                tempProductFolderPath = MyTool.FolderAndFileManageClass.GetFolderPathInAssignedFolder(_samplePanelResultSavePath);
                foreach (string path in tempProductFolderPath)
                {
                    if(cboProductName.FindString(path.Substring(path.LastIndexOf("\\")+1)) == -1)
                        cboProductName.Items.Add(path.Substring(path.LastIndexOf("\\") + 1));
                }
            }
        }

        //点击良品按钮
        private void btnSearchKindOKProduct_Click(object sender, EventArgs e)
        {
            _dispOKProductFlag = !_dispOKProductFlag;
            if (_dispOKProductFlag)//如果显示OK产品
            {
                btnSearchKindOKProduct.BackColor = SystemColors.Control;
            }
            else
            {
                btnSearchKindOKProduct.BackColor = SystemColors.ButtonShadow;
            }
        }

        //点击不良品按钮
        private void btnSearchKindNGProduct_Click(object sender, EventArgs e)
        {
            _dispNGProductFlag = !_dispNGProductFlag;
            if (_dispNGProductFlag)//如果显示OK产品
            {
                btnSearchKindNGProduct.BackColor = SystemColors.Control;
            }
            else
            {
                btnSearchKindNGProduct.BackColor = SystemColors.ButtonShadow;
            }
        }

        #endregion

        #region 搜索按钮

        private void btnSearch_Click(object sender, EventArgs e)
        {
            //pnlSearching.Visible = true;
            //lblSearching.Visible = true;


            this.Invoke(new Action(() =>
                {
                    List<string> myList = new List<string>();
                    List<string> productList = new List<string>();

                    //数据统计datatable初始化
                    if (_searchResultDataTable != null)
                    {
                        _searchResultDataTable.Dispose();
                        _searchResultDataTable = new DataTable();
                    }

                    _searchResultDataTable = new DataTable();
                    for (int i = 0; i < dgrdvSearchResultDisp.Columns.Count; i++)
                    {
                        _searchResultDataTable.Columns.Add(dgrdvSearchResultDisp.Columns[i].Name.ToString(), typeof(string));
                    }

                    //显示搜索结果Datagirdview初始化
                    dgrdvSearchResultDisp.Rows.Clear();

                    //产量统计结构体初始化
                    _searchResultPcsYield._totalYield = 0;
                    _searchResultPcsYield._NGYield = 0;
                    _searchResultPcsYield._OKYield = 0;

                    _searchResultPanelYield._totalYield = 0;
                    _searchResultPanelYield._NGYield = 0;
                    _searchResultPanelYield._OKYield = 0;

                    _searchResultElementYield._totalYield = 0;
                    _searchResultElementYield._NGYield = 0;
                    _searchResultElementYield._OKYield = 0;

                    //获取常规产品路径的文件夹中的数据
                    if (chkCheckNormalProduct.Checked && Directory.Exists(_normalProductResultSavePath))
                    {
                        myList = MyTool.FolderAndFileManageClass.GetFolderPathInAssignedFolder(_normalProductResultSavePath);
                        foreach (string folderPath in myList)
                        {
                            if (chkProductNameFilter.Checked)//如果启动了产品名筛选
                            {
                                if (cboProductName.SelectedIndex != -1 && cboProductName.SelectedIndex < cboProductName.Items.Count)//如果产品名筛选了有效名称
                                {
                                    if (folderPath.Substring(folderPath.LastIndexOf("\\") + 1) != cboProductName.SelectedItem.ToString())//如果当前产品名不等于选定产品名
                                    {
                                        continue;//跳过当前文件
                                    }
                                }
                                else
                                {
                                    continue;
                                }
                            }

                            if (folderPath != _samplePanelResultSavePath)
                            {
                                if (Directory.Exists(folderPath + "\\Result\\"))
                                {
                                    productList = MyTool.FolderAndFileManageClass.GetFolderPathInAssignedFolder(folderPath + "\\Result\\");
                                    foreach (string product in productList)
                                    {
                                        DataRow dr = _searchResultDataTable.NewRow();
                                        string tempstr = "";
                                        string subItem = "";

                                        YIELD tempSearchResultPcsYield = new YIELD();//为了确保良品不良品筛选时可以不被误加入，用一个中转
                                        YIELD tempSearchResultPanelYield = new YIELD();//为了确保良品不良品筛选时可以不被误加入，用一个中转
                                        YIELD tempSearchResultElementYield = new YIELD();//为了确保良品不良品筛选时可以不被误加入，用一个中转

                                        tempSearchResultPcsYield._totalYield = 0;
                                        tempSearchResultPcsYield._NGYield = 0;
                                        tempSearchResultPcsYield._OKYield = 0;

                                        tempSearchResultPanelYield._totalYield = 0;
                                        tempSearchResultPanelYield._NGYield = 0;
                                        tempSearchResultPanelYield._OKYield = 0;

                                        tempSearchResultElementYield._totalYield = 0;
                                        tempSearchResultElementYield._OKYield = 0;
                                        tempSearchResultElementYield._NGYield = 0;

                                        try
                                        {
                                            tempstr = product.Replace("__", "_");//清除多余的"_"

                                            dr[0] = (dgrdvSearchResultDisp.Rows.Count + 1).ToString();//增加序号

                                            tempstr = tempstr.Substring(tempstr.LastIndexOf("\\") + 1);//获取文件夹名称
                                            subItem = tempstr.Substring(tempstr.IndexOf("_") + 1, tempstr.LastIndexOf("_") - tempstr.IndexOf("_") - 1);//获取产品名
                                            if (chkProductNameFilter.Checked)//如果启动了产品名筛选
                                            {
                                                if (cboProductName.SelectedIndex != -1 && cboProductName.SelectedIndex < cboProductName.Items.Count)//如果产品名筛选了有效名称
                                                {
                                                    if (subItem != cboProductName.SelectedItem.ToString())//如果当前产品名不等于选定产品名
                                                    {
                                                        continue;//跳过当前文件
                                                    }
                                                }
                                                else
                                                {
                                                    continue;
                                                }
                                            }
                                            dr[1] = subItem;

                                            subItem = tempstr.Substring(0, tempstr.IndexOf("_"));//获取二维码数据
                                            if (chkBarcodeFilter.Checked)//如果启用了二维码搜索
                                            {
                                                if (txtBarcodeFilterBarcode.Text != subItem)//如果条形码不一致
                                                {
                                                    continue;//跳过当前文件
                                                }
                                            }
                                            dr[2] = subItem;

                                            subItem = tempstr.Substring(tempstr.LastIndexOf("_") + 1);//获取文件夹创建时间，即检测开始时间
                                            //检测产品检测开始时间是否处于设定时间范围内
                                            DateTime inspectTime = new DateTime(Convert.ToInt32(subItem.Substring(0, 4)), Convert.ToInt32(subItem.Substring(4, 2)), Convert.ToInt32(subItem.Substring(6, 2)),
                                                Convert.ToInt32(subItem.Substring(8, 2)), Convert.ToInt32(subItem.Substring(10, 2)), Convert.ToInt32(subItem.Substring(12, 2)));
                                            DateTime searchRegionStartTime = new DateTime(dtpSearchStartDateTime.Value.Year, dtpSearchStartDateTime.Value.Month, dtpSearchStartDateTime.Value.Day,
                                                dtpSearchStartHourTime.Value.Hour, dtpSearchStartHourTime.Value.Minute, dtpSearchStartHourTime.Value.Second);//结果文件筛选开始时间
                                            DateTime searchRegionStopTime = new DateTime(dtpSearchStopDateTime.Value.Year, dtpSearchStopDateTime.Value.Month, dtpSearchStopDateTime.Value.Day,
                                                dtpSearchStopHourTime.Value.Hour, dtpSearchStopHourTime.Value.Minute, dtpSearchStopHourTime.Value.Second);//结果文件筛选开始时间
                                            //如果当前产品检测时间不处于筛选时间范围内
                                            if (!(inspectTime.CompareTo(searchRegionStartTime) >= 0 && inspectTime.CompareTo(searchRegionStopTime) <= 0))
                                            {
                                                continue;
                                            }
                                            dr[3] = inspectTime.ToString("yyyy/MM/dd HH:mm:ss");

                                            if (File.Exists(product + "\\CheckResult.csv") && File.Exists(product + "\\PCSResult.csv"))//如果检测结果csv文件存在
                                            {
                                                FileInfo finfo = new FileInfo(product + "\\CheckResult.csv");
                                                DateTime createTime = finfo.CreationTime;
                                                subItem = createTime.ToString("yyyy/MM/dd HH:mm:ss");//获取结果文件创建时间，即检测结束时间
                                                dr[4] = subItem;

                                                //获取结果数据
                                                DataTable pcsInspectResult = CSVFile.CSVFileOperation.ReadCSV(product + "\\PCSResult.csv");
                                                if (pcsInspectResult == null)//如果没有数据
                                                {
                                                    dr[5] = "NULL";//设置结果为NULL
                                                    dr[6] = "NULL";//设置良率为空
                                                }
                                                else
                                                {
                                                    bool haveNGFlag = false;//数据中有NG项标志
                                                    int OKPointQuantity = 0;
                                                    int NGPointQuantity = 0;
                                                    for (int i = 0; i < pcsInspectResult.Rows.Count; i++)
                                                    {
                                                        if (pcsInspectResult.Rows[i][2].ToString() == "PASS")//如果为PASS
                                                        {
                                                            OKPointQuantity++;

                                                            tempSearchResultPcsYield._totalYield++;
                                                            tempSearchResultPcsYield._OKYield++;
                                                        }
                                                        else if (pcsInspectResult.Rows[i][2].ToString() == "FAIL")//如果为FAIL
                                                        {
                                                            haveNGFlag = true;
                                                            NGPointQuantity++;

                                                            tempSearchResultPcsYield._totalYield++;
                                                            tempSearchResultPcsYield._NGYield++;
                                                        }
                                                    }
                                                    if (haveNGFlag)//如果有NG
                                                    {
                                                        dr[5] = "NG";//设置结果为NG
                                                        if (OKPointQuantity + NGPointQuantity == 0)
                                                        {
                                                            dr[6] = "0.0%";//设置良率为空
                                                        }
                                                        else
                                                        {
                                                            //良率
                                                            dr[6] = ((Math.Floor(((float)OKPointQuantity / ((float)OKPointQuantity + (float)NGPointQuantity)) * 100f) * 10) / 10).ToString() + "%";
                                                        }
                                                        tempSearchResultPanelYield._totalYield++;
                                                        tempSearchResultPanelYield._NGYield++;
                                                    }
                                                    else
                                                    {
                                                        dr[5] = "OK";//设置结果为OK
                                                        dr[6] = "100.0%";//设置良率为空
                                                        tempSearchResultPanelYield._totalYield++;
                                                        tempSearchResultPanelYield._OKYield++;
                                                    }
                                                }

                                                //获取元件结果数据
                                                DataTable elementInspectResult = CSVFile.CSVFileOperation.ReadCSV(product + "\\CheckResult.csv");
                                                if (elementInspectResult != null)//如果有数据
                                                {
                                                    for (int i = 0; i < elementInspectResult.Rows.Count; i++)
                                                    {
                                                        if (elementInspectResult.Rows[i][4].ToString() == "OK")//如果为OK
                                                        {
                                                            tempSearchResultElementYield._totalYield++;
                                                            tempSearchResultElementYield._OKYield++;
                                                        }
                                                        else if (elementInspectResult.Rows[i][4].ToString() == "NG")//如果为NG
                                                        {
                                                            tempSearchResultElementYield._totalYield++;
                                                            tempSearchResultElementYield._NGYield++;
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                subItem = "NULL";
                                                dr[4] = subItem;
                                                dr[5] = subItem;
                                                dr[6] = subItem;
                                            }

                                            if (!_dispOKProductFlag && dr[5].ToString() == "OK")//如果不显示OK项，并且结果为OK
                                            {
                                                continue;
                                            }
                                            if (!_dispNGProductFlag && dr[5].ToString() != "OK")//如果不显示NG项，并且结果为NG或NULL
                                            {
                                                continue;
                                            }

                                            dr[7] = product;

                                            _searchResultPcsYield._totalYield += tempSearchResultPcsYield._totalYield;
                                            _searchResultPcsYield._OKYield += tempSearchResultPcsYield._OKYield;
                                            _searchResultPcsYield._NGYield += tempSearchResultPcsYield._NGYield;

                                            _searchResultPanelYield._totalYield += tempSearchResultPanelYield._totalYield;
                                            _searchResultPanelYield._OKYield += tempSearchResultPanelYield._OKYield;
                                            _searchResultPanelYield._NGYield += tempSearchResultPanelYield._NGYield;

                                            _searchResultElementYield._totalYield += tempSearchResultElementYield._totalYield;
                                            _searchResultElementYield._OKYield += tempSearchResultElementYield._OKYield;
                                            _searchResultElementYield._NGYield += tempSearchResultElementYield._NGYield;

                                            dgrdvSearchResultDisp.Rows.Add(dr.ItemArray);
                                            _searchResultDataTable.Rows.Add(dr.ItemArray);
                                        }
                                        catch
                                        {
                                            continue;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else if (!Directory.Exists(_normalProductResultSavePath))
                    {
                        MessageBox.Show("常规产品检测结果路径文件夹不存在！");
                    }

                    //如果选择显示样品板数据
                    if (chkCheckSampleProduct.Checked && Directory.Exists(_samplePanelResultSavePath))
                    {
                        //获取样品板文件夹中的产品数据
                        myList = MyTool.FolderAndFileManageClass.GetFolderPathInAssignedFolder(_samplePanelResultSavePath);
                        foreach (string folderPath in myList)
                        {
                            if (chkProductNameFilter.Checked)//如果启动了产品名筛选
                            {
                                if (cboProductName.SelectedIndex != -1 && cboProductName.SelectedIndex < cboProductName.Items.Count)//如果产品名筛选了有效名称
                                {
                                    if (folderPath.Substring(folderPath.LastIndexOf("\\") + 1) != cboProductName.SelectedItem.ToString())//如果当前产品名不等于选定产品名
                                    {
                                        continue;//跳过当前文件
                                    }
                                }
                                else
                                {
                                    continue;
                                }
                            }

                            if (Directory.Exists(folderPath + "\\Result\\"))
                            {
                                productList = MyTool.FolderAndFileManageClass.GetFolderPathInAssignedFolder(folderPath + "\\Result\\");
                                foreach (string product in productList)
                                {
                                    DataRow dr = _searchResultDataTable.NewRow();
                                    string tempstr = "";
                                    string subItem = "";
                                    YIELD tempSearchResultPcsYield = new YIELD();//为了确保良品不良品筛选时可以不被误加入，用一个中转
                                    YIELD tempSearchResultPanelYield = new YIELD();//为了确保良品不良品筛选时可以不被误加入，用一个中转
                                    YIELD tempSearchResultElementYield = new YIELD();//为了确保良品不良品筛选时可以不被误加入，用一个中转

                                    tempSearchResultPcsYield._totalYield = 0;
                                    tempSearchResultPcsYield._NGYield = 0;
                                    tempSearchResultPcsYield._OKYield = 0;

                                    tempSearchResultPanelYield._totalYield = 0;
                                    tempSearchResultPanelYield._NGYield = 0;
                                    tempSearchResultPanelYield._OKYield = 0;

                                    tempSearchResultElementYield._totalYield = 0;
                                    tempSearchResultElementYield._OKYield = 0;
                                    tempSearchResultElementYield._NGYield = 0;

                                    try
                                    {
                                        tempstr = product.Replace("__", "_");//清除多余的"_"

                                        dr[0] = (dgrdvSearchResultDisp.Rows.Count + 1).ToString();//增加序号

                                        tempstr = tempstr.Substring(tempstr.LastIndexOf("\\") + 1);//获取文件夹名称

                                        subItem = tempstr.Substring(tempstr.IndexOf("_") + 1, tempstr.LastIndexOf("_") - tempstr.IndexOf("_") - 1);//获取产品名
                                        if (chkProductNameFilter.Checked)//如果启动了产品名筛选
                                        {
                                            if (cboProductName.SelectedIndex != -1 && cboProductName.SelectedIndex < cboProductName.Items.Count)//如果产品名筛选了有效名称
                                            {
                                                if (subItem != cboProductName.SelectedItem.ToString())//如果当前产品名不等于选定产品名
                                                {
                                                    continue;//跳过当前文件
                                                }
                                            }
                                            else
                                            {
                                                continue;
                                            }
                                        }
                                        dr[1] = subItem;

                                        subItem = tempstr.Substring(0, tempstr.IndexOf("_"));//获取二维码数据
                                        if (chkBarcodeFilter.Checked)//如果启用了二维码搜索
                                        {
                                            if (txtBarcodeFilterBarcode.Text != subItem)//如果条形码不一致
                                            {
                                                continue;//跳过当前文件
                                            }
                                        }
                                        dr[2] = subItem;

                                        subItem = tempstr.Substring(tempstr.LastIndexOf("_") + 1);//获取文件夹创建时间，即检测开始时间
                                        //检测产品检测开始时间是否处于设定时间范围内
                                        DateTime inspectTime = new DateTime(Convert.ToInt32(subItem.Substring(0, 4)), Convert.ToInt32(subItem.Substring(4, 2)), Convert.ToInt32(subItem.Substring(6, 2)),
                                            Convert.ToInt32(subItem.Substring(8, 2)), Convert.ToInt32(subItem.Substring(10, 2)), Convert.ToInt32(subItem.Substring(12, 2)));
                                        DateTime searchRegionStartTime = new DateTime(dtpSearchStartDateTime.Value.Year, dtpSearchStartDateTime.Value.Month, dtpSearchStartDateTime.Value.Day,
                                            dtpSearchStartHourTime.Value.Hour, dtpSearchStartHourTime.Value.Minute, dtpSearchStartHourTime.Value.Second);//结果文件筛选开始时间
                                        DateTime searchRegionStopTime = new DateTime(dtpSearchStopDateTime.Value.Year, dtpSearchStopDateTime.Value.Month, dtpSearchStopDateTime.Value.Day,
                                            dtpSearchStopHourTime.Value.Hour, dtpSearchStopHourTime.Value.Minute, dtpSearchStopHourTime.Value.Second);//结果文件筛选开始时间
                                        //如果当前产品检测时间不处于筛选时间范围内
                                        if (!(inspectTime.CompareTo(searchRegionStartTime) >= 0 && inspectTime.CompareTo(searchRegionStopTime) <= 0))
                                        {
                                            continue;
                                        }
                                        dr[3] = inspectTime.ToString("yyyy/MM/dd HH:mm:ss");

                                        if (File.Exists(product + "\\CheckResult.csv") && File.Exists(product + "\\PCSResult.csv"))//如果检测结果csv文件存在
                                        {
                                            FileInfo finfo = new FileInfo(product + "\\CheckResult.csv");
                                            DateTime createTime = finfo.CreationTime;
                                            subItem = createTime.ToString("yyyy/MM/dd HH:mm:ss");//获取结果文件创建时间，即检测结束时间
                                            dr[4] = subItem;


                                            //获取结果数据
                                            DataTable pcsInspectResult = CSVFile.CSVFileOperation.ReadCSV(product + "\\PCSResult.csv");
                                            if (pcsInspectResult == null)//如果没有数据
                                            {
                                                dr[5] = "NULL";//设置结果为NULL
                                                dr[6] = "NULL";//设置良率为空
                                            }
                                            else
                                            {
                                                bool haveNGFlag = false;//数据中有NG项标志
                                                int OKPointQuantity = 0;
                                                int NGPointQuantity = 0;
                                                for (int i = 0; i < pcsInspectResult.Rows.Count; i++)
                                                {
                                                    if (pcsInspectResult.Rows[i][2].ToString() == "PASS")//如果为PASS
                                                    {
                                                        OKPointQuantity++;

                                                        tempSearchResultPcsYield._totalYield++;
                                                        tempSearchResultPcsYield._OKYield++;
                                                    }
                                                    else if (pcsInspectResult.Rows[i][2].ToString() == "FAIL")//如果为FAIL
                                                    {
                                                        haveNGFlag = true;
                                                        NGPointQuantity++;

                                                        tempSearchResultPcsYield._totalYield++;
                                                        tempSearchResultPcsYield._NGYield++;
                                                    }
                                                }
                                                if (haveNGFlag)//如果有NG
                                                {
                                                    dr[5] = "NG";//设置结果为NG
                                                    if (OKPointQuantity + NGPointQuantity == 0)
                                                    {
                                                        dr[6] = "0.0%";//设置良率为空
                                                    }
                                                    else
                                                    {
                                                        //良率
                                                        dr[6] = ((Math.Floor(((float)OKPointQuantity / ((float)OKPointQuantity + (float)NGPointQuantity)) * 100f) * 10) / 10).ToString() + "%";
                                                    }
                                                    tempSearchResultPanelYield._totalYield++;
                                                    tempSearchResultPanelYield._NGYield++;
                                                }
                                                else
                                                {
                                                    dr[5] = "OK";//设置结果为OK
                                                    dr[6] = "100.0%";//设置良率为空
                                                    tempSearchResultPanelYield._totalYield++;
                                                    tempSearchResultPanelYield._OKYield++;
                                                }
                                            }

                                            //获取元件结果数据
                                            DataTable elementInspectResult = CSVFile.CSVFileOperation.ReadCSV(product + "\\CheckResult.csv");
                                            if (elementInspectResult != null)//如果有数据
                                            {
                                                for (int i = 0; i < elementInspectResult.Rows.Count; i++)
                                                {
                                                    if (elementInspectResult.Rows[i][4].ToString() == "OK")//如果为OK
                                                    {
                                                        tempSearchResultElementYield._totalYield++;
                                                        tempSearchResultElementYield._OKYield++;
                                                    }
                                                    else if (elementInspectResult.Rows[i][4].ToString() == "NG")//如果为NG
                                                    {
                                                        tempSearchResultElementYield._totalYield++;
                                                        tempSearchResultElementYield._NGYield++;
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            subItem = "NULL";
                                            dr[4] = subItem;
                                            dr[5] = "NULL";//设置结果为NULL
                                            dr[6] = "NULL";//设置良率为空
                                        }

                                        if (!_dispOKProductFlag && dr[5].ToString() == "OK")//如果不显示OK项，并且结果为OK
                                        {
                                            continue;
                                        }
                                        if (!_dispNGProductFlag && dr[5].ToString() != "OK")//如果不显示NG项，并且结果为NG或NULL
                                        {
                                            continue;
                                        }

                                        dr[7] = product;

                                        _searchResultPcsYield._totalYield += tempSearchResultPcsYield._totalYield;
                                        _searchResultPcsYield._OKYield += tempSearchResultPcsYield._OKYield;
                                        _searchResultPcsYield._NGYield += tempSearchResultPcsYield._NGYield;

                                        _searchResultPanelYield._totalYield += tempSearchResultPanelYield._totalYield;
                                        _searchResultPanelYield._OKYield += tempSearchResultPanelYield._OKYield;
                                        _searchResultPanelYield._NGYield += tempSearchResultPanelYield._NGYield;

                                        _searchResultElementYield._totalYield += tempSearchResultElementYield._totalYield;
                                        _searchResultElementYield._OKYield += tempSearchResultElementYield._OKYield;
                                        _searchResultElementYield._NGYield += tempSearchResultElementYield._NGYield;

                                        dgrdvSearchResultDisp.Rows.Add(dr.ItemArray);
                                        _searchResultDataTable.Rows.Add(dr.ItemArray);
                                    }
                                    catch
                                    {
                                        continue;
                                    }
                                }
                            }
                        }
                    }
                    else if (!Directory.Exists(_samplePanelResultSavePath))
                    {
                        MessageBox.Show("样品板检测结果路径文件夹不存在！");
                    }

                    dgrdvSearchResultDisp.DefaultCellStyle.Font = new Font("Times New Roman", 10);
                    lblSearchResultQuantity.Text = "搜索结果个数: " + dgrdvSearchResultDisp.Rows.Count;
                    DispPassRate();

                    pnlSearching.Visible = false;
                    lblSearching.Visible = false;
                })); ;
        }

        #endregion

        //计时器更新时间
        private void tmrRefreshTime_Tick(object sender, EventArgs e)
        {
            lblCurrentTime.Text = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
        }

        #region 搜索结果显示界面点击事件

        //如果点击了cell项目
        private void dgrdvSearchResultDisp_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (chkBarcodeFilter.Checked)//如果启用了二维码筛选
            {
                if (e.RowIndex >= 0)
                {
                    txtBarcodeFilterBarcode.Text = dgrdvSearchResultDisp.Rows[e.RowIndex].Cells[2].Value.ToString();
                }
            }
        }

        //如果点击了搜索界面双击按钮
        private void dgrdvSearchResultDisp_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1)//如果没有点击标题栏
            {
                _dispInspectResultProductPath = dgrdvSearchResultDisp.Rows[e.RowIndex].Cells[7].Value.ToString();//获取产品路径
                InspectResultDisp();
            }
        }

        //如果搜索结果显示界面鼠标Down事件发生
        private void dgrdvSearchResultDisp_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)//如果点击的是鼠标右键
            {
                if (_accessLevel == 102 || _accessLevel == 101)//如果权限为工程师或是厂家，才可以删除
                cmsSearchResultDispRightClick.Show((DataGridView)sender, new Point(e.X, e.Y));
            }
        }

        #endregion

        //产量显示相关Radio按钮点击事件
        private void YieldDispRadioButtonClickEvent(object sender, EventArgs e)
        {
            DispPassRate();
        }

        //显示良率数据
        private void DispPassRate()
        {
            if (rbtnPcsYield.Checked)//如果选中了PCS数量
            {
                dgrdvPassRateRecord.Rows[0].Cells[0].Value = "检测PCS数";
                dgrdvPassRateRecord.Rows[1].Cells[0].Value = "PCS良品数";
                dgrdvPassRateRecord.Rows[2].Cells[0].Value = "PCS不良品数";
                dgrdvPassRateRecord.Rows[3].Cells[0].Value = "PCS良品率";

                dgrdvPassRateRecord.Rows[0].Cells[1].Value = _searchResultPcsYield._totalYield.ToString();
                dgrdvPassRateRecord.Rows[1].Cells[1].Value = _searchResultPcsYield._OKYield.ToString();
                dgrdvPassRateRecord.Rows[2].Cells[1].Value = _searchResultPcsYield._NGYield.ToString();
                if (_searchResultPcsYield._totalYield == 0)
                    dgrdvPassRateRecord.Rows[3].Cells[1].Value = "0.0%";
                else
                    dgrdvPassRateRecord.Rows[3].Cells[1].Value = (Math.Floor((((float)_searchResultPcsYield._OKYield / (float)_searchResultPcsYield._totalYield) * 100f) * 10f) / 10).ToString() + "%";//Element良率
            }
            else if (rbtnPanelYield.Checked)//如果选中了Panel数量
            {
                dgrdvPassRateRecord.Rows[0].Cells[0].Value = "检测Panel数";
                dgrdvPassRateRecord.Rows[1].Cells[0].Value = "Panel良品数";
                dgrdvPassRateRecord.Rows[2].Cells[0].Value = "Panel不良品数";
                dgrdvPassRateRecord.Rows[3].Cells[0].Value = "Panel良品率";

                dgrdvPassRateRecord.Rows[0].Cells[1].Value = _searchResultPanelYield._totalYield.ToString();
                dgrdvPassRateRecord.Rows[1].Cells[1].Value = _searchResultPanelYield._OKYield.ToString();
                dgrdvPassRateRecord.Rows[2].Cells[1].Value = _searchResultPanelYield._NGYield.ToString();
                if (_searchResultPcsYield._totalYield == 0)
                    dgrdvPassRateRecord.Rows[3].Cells[1].Value = "0.0%";
                else
                    dgrdvPassRateRecord.Rows[3].Cells[1].Value = (Math.Floor((((float)_searchResultPanelYield._OKYield / (float)_searchResultPanelYield._totalYield) * 100f) * 10f) / 10).ToString() + "%";//Element良率
            }
            else if (rbtnElementYield.Checked)//如果选中了Element数量
            {
                dgrdvPassRateRecord.Rows[0].Cells[0].Value = "检测元件数";
                dgrdvPassRateRecord.Rows[1].Cells[0].Value = "元件良品数";
                dgrdvPassRateRecord.Rows[2].Cells[0].Value = "元件不良品数";
                dgrdvPassRateRecord.Rows[3].Cells[0].Value = "元件良品率";

                dgrdvPassRateRecord.Rows[0].Cells[1].Value = _searchResultElementYield._totalYield.ToString();
                dgrdvPassRateRecord.Rows[1].Cells[1].Value = _searchResultElementYield._OKYield.ToString();
                dgrdvPassRateRecord.Rows[2].Cells[1].Value = _searchResultElementYield._NGYield.ToString();
                if (_searchResultPcsYield._totalYield == 0)
                    dgrdvPassRateRecord.Rows[3].Cells[1].Value = "0.0%";
                else
                    dgrdvPassRateRecord.Rows[3].Cells[1].Value = (Math.Floor((((float)_searchResultElementYield._OKYield / (float)_searchResultElementYield._totalYield) * 100f) * 10f) / 10).ToString() + "%";//Element良率
            }
        }

        //检测结果相关相关Radio按钮点击事件
        private void InspectResultDispKindRadioButtonClickEvent(object sender, EventArgs e)
        {
            InspectResultDisp();
        }

        //检测结果显示
        private void InspectResultDisp()
        {
            if (Directory.Exists(_dispInspectResultProductPath))
            {
                string tempName = _dispInspectResultProductPath;
                try
                {
                    tempName = tempName.Replace("__", "_");
                    tempName = tempName.Substring(tempName.LastIndexOf("\\") + 1);

                    lblSelectedProductName.Text = "名称: " + tempName.Substring(tempName.IndexOf("_") + 1, tempName.LastIndexOf("_") - tempName.IndexOf("_") - 1)
                        + " " + tempName.Substring(0, tempName.IndexOf("_"));
                }
                catch
                {
                    MessageBox.Show("获取检测结果数据失败！");
                    return;
                }

                if (rbtnPCSResult.Checked)//如果选中了PCS结果显示
                {
                    if (File.Exists(_dispInspectResultProductPath + "\\PCSResult.csv"))//如果PCS结果文件存在
                    {
                        DataTable dt = CSVFile.CSVFileOperation.ReadCSV(_dispInspectResultProductPath + "\\PCSResult.csv");
                        dgrdvCheckResultDisp.DataSource = dt;
                        for (int i = 0; i < dt.Columns.Count; i++)
                        {
                            dgrdvCheckResultDisp.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                            dgrdvCheckResultDisp.Columns[i].FillWeight = 30;
                            dgrdvCheckResultDisp.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                        }

                        for (int i = 0; i < dgrdvCheckResultDisp.Rows.Count; i++)
                        {
                            if (dgrdvCheckResultDisp.Rows[i].Cells[dgrdvCheckResultDisp.Columns.Count - 1].Value.ToString() == "PASS")//如果为OK
                            {
                                dgrdvCheckResultDisp.Rows[i].DefaultCellStyle.ForeColor = Color.DarkGreen;
                            }
                            if (dgrdvCheckResultDisp.Rows[i].Cells[dgrdvCheckResultDisp.Columns.Count - 1].Value.ToString() == "FAIL")//如果为NG
                            {
                                dgrdvCheckResultDisp.Rows[i].DefaultCellStyle.ForeColor = Color.Red;
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("PCS检测结果CSV文件不存在！");
                    }
                }
                else if (rbtnElementResult.Checked)//如果选中了Element结果显示
                {
                    if (File.Exists(_dispInspectResultProductPath + "\\CheckResult.csv"))//如果PCS结果文件存在
                    {
                        DataTable dt = CSVFile.CSVFileOperation.ReadCSV(_dispInspectResultProductPath + "\\CheckResult.csv");
                        dgrdvCheckResultDisp.DataSource = dt;
                        for (int i = 0; i < dt.Columns.Count; i++)
                        {
                            dgrdvCheckResultDisp.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                            if (i == 0 || i == 4)
                                dgrdvCheckResultDisp.Columns[i].FillWeight = 10;
                            else if (i == 1)
                                dgrdvCheckResultDisp.Columns[i].FillWeight = 20;
                            else
                                dgrdvCheckResultDisp.Columns[i].FillWeight = 20;
                            dgrdvCheckResultDisp.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                        }

                        for (int i = 0; i < dgrdvCheckResultDisp.Rows.Count; i++)
                        {
                            try
                            {
                                if (dgrdvCheckResultDisp.Rows[i].Cells[2].Value.ToString().Substring(0, 2) == "OK")
                                {
                                    dgrdvCheckResultDisp.Rows[i].Cells[2].Value = "OK";
                                }
                                else if (dgrdvCheckResultDisp.Rows[i].Cells[2].Value.ToString().Substring(0, 2) == "NG")
                                {
                                    dgrdvCheckResultDisp.Rows[i].Cells[2].Value = "NG";
                                }

                                if (dgrdvCheckResultDisp.Rows[i].Cells[3].Value.ToString().Substring(0, 2) == "OK")
                                {
                                    dgrdvCheckResultDisp.Rows[i].Cells[3].Value = "OK";
                                }
                                else if (dgrdvCheckResultDisp.Rows[i].Cells[3].Value.ToString().Substring(0, 2) == "NG")
                                {
                                    dgrdvCheckResultDisp.Rows[i].Cells[3].Value = "NG";
                                }
                            }
                            catch
                            { }

                            if (dgrdvCheckResultDisp.Rows[i].Cells[dgrdvCheckResultDisp.Columns.Count - 1].Value.ToString() == "OK")//如果为OK
                            {
                                dgrdvCheckResultDisp.Rows[i].DefaultCellStyle.ForeColor = Color.DarkGreen;
                            }
                            if (dgrdvCheckResultDisp.Rows[i].Cells[dgrdvCheckResultDisp.Columns.Count - 1].Value.ToString() == "NG")//如果为NG
                            {
                                dgrdvCheckResultDisp.Rows[i].DefaultCellStyle.ForeColor = Color.Red;
                            }
                        }

                    }
                    else
                    {
                        MessageBox.Show("元件检测结果CSV文件不存在！");
                    }
                }
            }
        }

        //如果在搜索结果显示界面点击了删除按钮，删除对应的文件
        private void 删除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<string> selectedPathList = new List<string>();
            List<DataGridViewRow> selectedPathRow = new List<DataGridViewRow>();
            for (int i = 0; i < dgrdvSearchResultDisp.SelectedRows.Count; i++)
            {
                selectedPathList.Add(dgrdvSearchResultDisp.SelectedRows[i].Cells[7].Value.ToString());
                selectedPathRow.Add(dgrdvSearchResultDisp.SelectedRows[i]);
            }

            for (int i = 0; i < selectedPathList.Count; i++)
            {
                try
                {
                    string resultFileFolderPath = selectedPathList[i];
                    string baseFolderPath = resultFileFolderPath.Substring(0, resultFileFolderPath.LastIndexOf("\\"));
                    baseFolderPath = baseFolderPath.Substring(0, baseFolderPath.LastIndexOf("\\"));
                    string panelFolderName = resultFileFolderPath.Substring(resultFileFolderPath.LastIndexOf("\\") + 1);

                    string sourceImageFileFolderPath = baseFolderPath + "\\" + "SourceImage\\" + panelFolderName;
                    string inferredOKImageFileFolderPath = baseFolderPath + "\\" + "InferredOK\\" + panelFolderName;
                    string inferredNGImageFileFolderPath = baseFolderPath + "\\" + "InferredNG\\" + panelFolderName;

                    if (Directory.Exists(resultFileFolderPath))//如果文件夹存在
                    {
                        DirectoryInfo dinfo = new DirectoryInfo(resultFileFolderPath);
                        dinfo.Delete(true);
                    }

                    if (Directory.Exists(sourceImageFileFolderPath))//如果文件夹存在
                    {
                        DirectoryInfo dinfo = new DirectoryInfo(sourceImageFileFolderPath);
                        dinfo.Delete(true);
                    }

                    if (Directory.Exists(inferredOKImageFileFolderPath))//如果文件夹存在
                    {
                        DirectoryInfo dinfo = new DirectoryInfo(inferredOKImageFileFolderPath);
                        dinfo.Delete(true);
                    }

                    if (Directory.Exists(inferredNGImageFileFolderPath))//如果文件夹存在
                    {
                        DirectoryInfo dinfo = new DirectoryInfo(inferredNGImageFileFolderPath);
                        dinfo.Delete(true);
                    }

                }
                catch
                {
                    continue;
                }

                dgrdvSearchResultDisp.Rows.Remove(selectedPathRow[i]);
            }
            lblSearchResultQuantity.Text = "搜索结果个数: " + dgrdvSearchResultDisp.Rows.Count;
            //btnSearch_Click(new object(), new EventArgs());
        }

        //如果结果表格datagridview里面双击
        private void dgrdvCheckResultDisp_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1 && e.RowIndex < dgrdvCheckResultDisp.Rows.Count)
            {
                string baseFolderPath = _dispInspectResultProductPath.Substring(0, _dispInspectResultProductPath.LastIndexOf("\\"));
                baseFolderPath = baseFolderPath.Substring(0, baseFolderPath.LastIndexOf("\\"));
                string panelFolderName = _dispInspectResultProductPath.Substring(_dispInspectResultProductPath.LastIndexOf("\\") + 1);
                string tempName = dgrdvCheckResultDisp.Rows[e.RowIndex].Cells[1].Value.ToString();
                int PCSIndex = 0;


                if (_picDispFormVariate != null)
                {
                    _picDispFormVariate.Dispose();
                    _picDispFormVariate = null;
                }

                if (rbtnPCSResult.Checked)
                {
                    PCSIndex = Convert.ToInt32(tempName);
                    _picDispFormVariate = new ImageDispForm(baseFolderPath, panelFolderName, PCSIndex, tempName.Substring(tempName.IndexOf("-") + 1), false);
                    _picDispFormVariate.StartPosition = FormStartPosition.CenterScreen;
                    _picDispFormVariate.Show();
                }
                else
                {
                    PCSIndex = Convert.ToInt32(tempName.Substring(tempName.IndexOf("_") + 1, tempName.IndexOf("-") - tempName.IndexOf("_") - 1));
                    _picDispFormVariate = new ImageDispForm(baseFolderPath, panelFolderName, PCSIndex, tempName.Substring(tempName.IndexOf("-") + 1), true);
                    _picDispFormVariate.StartPosition = FormStartPosition.CenterScreen;
                    _picDispFormVariate.Show();
                }
            }
        }

        #region 用户登录等
        /// <summary>
        /// lblAccessLevel_Click:点击权限按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lblAccessLevel_Click(object sender, EventArgs e)
        {
            _userManagerVariate.StartPosition = FormStartPosition.CenterScreen;
            _userManagerVariate.Show();
            _userManagerVariate.Focus();
        }

        /// <summary>
        /// UserFormUserLogInFinishEventFunc:用户界面登录完成事件
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="accessLevel"></param>
        private void UserFormUserLogInFinishEventFunc(int accessLevel, string userName, bool hideFormFlag)
        {
            if(SPCFormLogInFinishEvent != null)
                SPCFormLogInFinishEvent(accessLevel, userName, hideFormFlag);

            UserLog(accessLevel, userName, hideFormFlag);
        }

        //用户登录函数
        public void UserLog(int accessLevel, string userName, bool hideFormFlag)
        {
            _accessLevel = accessLevel;
            _operatorName = userName;
            if (_accessLevel == 0)
            {
                lblAccessLevel.Text = "未登录";
                lblAccessLevel.ForeColor = Color.Black;
            }
            else if (_accessLevel == 100)
            {
                lblAccessLevel.Text = "操作员";
                lblAccessLevel.ForeColor = Color.LimeGreen;
            }
            else if (_accessLevel == 101)
            {
                lblAccessLevel.Text = "工程师";
                lblAccessLevel.ForeColor = Color.Orange;
            }
            else if (_accessLevel == 102)
            {
                lblAccessLevel.Text = "厂家";
                lblAccessLevel.ForeColor = Color.Red;
            }
            lblUserName.Text = _operatorName;

            if (hideFormFlag)
                _userManagerVariate.Hide();
        }

        /// <summary>
        /// UserFormRequestCloseEventFunc:用户界面请求关闭事件
        /// </summary>
        private void UserFormRequestCloseEventFunc()
        {
            if (SPCFormUserManagerFormCloseEvent != null)
                SPCFormUserManagerFormCloseEvent();

            UserManagerHide();
        }

        //用户管理界面隐藏函数
        public void UserManagerHide()
        {
            _userManagerVariate.Hide();
        }

        #endregion
    }
}
