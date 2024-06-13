using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CSVFile;
using System.IO;
using SoftKeyBoard;

namespace AxisAndIOForm
{
    public delegate void AxisPositionDataViewFormCloseDelegate();//声明委托,用于关闭轴点位数据显示窗口
    public delegate void AxisPositionDataModifiedDelegate();//声明委托,用于轴点位数据修改后主窗口显示更新
    public delegate void AxisParamterMidifiedDelegate();//声明委托，用于轴参数数据修改后主窗口的显示更新

    public partial class AxisPositionDataViewForm : Form
    {
        /*--------------------------------------------------------------------------------------
        //Copyright (C) 2022 深圳市利器精工科技有限公司
        //版权所有
        //
        //文件名：轴点位数据显示编辑页面
        //文件功能描述：显示轴的点位数据，便于编辑设定轴参数
        //
        //
        //创建标识：MaLi 20220308
        //
        //修改标识：MaLi 20220308 Change
        //修改描述：增加轴点位数据显示编辑窗口
        //
        //
        //------------------------------------------------------------------------------------*/
        public event AxisPositionDataViewFormCloseDelegate AxisPositionDataViewFormCloseEvent;//声明关闭轴点位数据显示窗口事件
        public event AxisPositionDataModifiedDelegate AxisPositionDataModifiedEvent;//声明轴点位数据修改事件
        public event AxisParameterDataModifiedDelegate AxisParameterDataModifiedEvent;//声明轴参数数据修改事件

        public int _axisQuantity;//轴个数
        public int _maxPointQuantity;//轴点位个数上限 

        private int _currentAxisIndex = 0;//当前轴Index
        private string _currentProductName = "Default";//产品名称

        //软负极限
        private float[] _axisSoftLimitMinus;
        //软正极限
        private float[] _axisSoftLimitPlus;
        //MoveMinSpeed
        private float[] _axisMoveMinSpeed_mmps;
        //MoveMaxSpeed
        private float[] _axisMoveMaxSpeed_mmps;
        //MoveminACCTime
        private float[] _axisMoveMinAccTime_s;
        //MoveMaxACCTime
        private float[] _axisMoveMaxAccTime_s;
        //MinDeviation
        private float[] _axisMinDeviation_mm;
        //MaxDeviation
        private float[] _axisMaxDeviation_mm;
        //轴点位个数
        private int[] _axisPointQuantity;

        /////////////////////////////////////////////////////

        #region 构造函数
        /// <summary>
        /// AxisPositionDataViewForm：构造函数
        /// </summary>
        /// <param name="ProductName">string：产品名称，用于根据产品名称选择数据表</param>
        /// <param name="CurrentAxisIndex">int:当前轴索引，用于根据索引获取到具体哪个轴的坐标数据</param>
        public AxisPositionDataViewForm(string productName, int currentAxisIndex)
        {
            InitializeComponent();

            #region
            _axisQuantity = AxisControlMainForm._axisQuantity;//轴个数
            _maxPointQuantity = AxisControlMainForm._maxPointQuantity;//轴点位个数上限 
            //软负极限
            _axisSoftLimitMinus = new float[_axisQuantity];
            //软正极限
            _axisSoftLimitPlus = new float[_axisQuantity];
            //MoveMinSpeed
            _axisMoveMinSpeed_mmps = new float[_axisQuantity];
            //MoveMaxSpeed
            _axisMoveMaxSpeed_mmps = new float[_axisQuantity];
            //MoveminACCTime
            _axisMoveMinAccTime_s = new float[_axisQuantity];
            //MoveMaxACCTime
            _axisMoveMaxAccTime_s = new float[_axisQuantity];
            //MinDeviation
            _axisMinDeviation_mm = new float[_axisQuantity];
            //MaxDeviation
            _axisMaxDeviation_mm = new float[_axisQuantity];
            //轴点位个数
            _axisPointQuantity = new int[_axisQuantity];
            #endregion

            _currentProductName = productName;
            _currentAxisIndex = currentAxisIndex;

            dgrdvAxisPositionDataShow.CellDoubleClick += DataGridViewCellDoubleClickEvent;//声明事件，用于打开软键盘
            dgrdvAxisPositionDataShow.CellLeave += LastDataCheckEvent;//声明事件，用于最后检查数据
            dgrdvAxisPositionDataShow.CellValueChanged += DataCheckEvent;//声明事件，用于检查数据
        }
        #endregion

        private void AxisPositionDataViewForm_Load(object sender, EventArgs e)
        {
            //双缓冲
            Type type = dgrdvAxisPositionDataShow.GetType();
            System.Reflection.PropertyInfo pi = type.GetProperty("DoubleBuffered",
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            pi.SetValue(dgrdvAxisPositionDataShow, true, null);

            PositionDataView(_currentProductName, _currentAxisIndex);
        }

        #region PositionDataView：轴点位数据显示
        /// <summary>
        /// PositionDataView：轴点位数据显示
        /// </summary>
        /// <param name="productName">string:产品名称</param>
        /// <param name="currentAxisIndex">int:当前轴索引</param>
        public void PositionDataView(string productName, int currentAxisIndex)
        {
            string axisPositionDataFilePath = Directory.GetCurrentDirectory() + @"\" +
                MyTool.FolderAndFileManageClass.FilterPathIllegalChar("AxisParameterAndPositionData_" + productName) + @"\" +
                MyTool.FolderAndFileManageClass.FilterPathIllegalChar("axis" + currentAxisIndex.ToString("00") + "Pos" + "_" + productName)
                + ".csv";//过滤掉作为Directory不合法的字符
            DataTable dt = new DataTable();

            //更新当前轴序号和当前产品名
            _currentAxisIndex = currentAxisIndex;
            _currentProductName = productName;

            if (_axisParameterDataViewFormVariate != null && _axisParameterDataViewFormVariate.Visible)//为了确保轴参数文件中显示的数据也是最新的，在这里加一个这个，确保如果产品名称变更了，数据也变更
            {
                _axisParameterDataViewFormVariate.AxisParameterDataDisplay(_currentProductName);
            }

            lblAxisPositionDataNotification.Text = "轴" + (currentAxisIndex + 1).ToString() + "点位数据";

            if (AxisParFileInitial())
            {
                try
                {
                    dgrdvAxisPositionDataShow.Invoke(new Action(() =>
                    {
                        dt = CSVFileOperation.ReadCSV(axisPositionDataFilePath);
                        if (dt.Rows.Count < _axisPointQuantity[currentAxisIndex])//如果点位个数小于设定的点位个数
                        {
                            MessageBox.Show("轴" + (currentAxisIndex + 1).ToString("00") + "点位文件中读取的点位数据个数小于设定的轴点位个数,利用默认数据填充，请根据实际情况修改此文件!");
                            int sourceDTRowCount = dt.Rows.Count;
                            for (int j = 0; j < _axisPointQuantity[currentAxisIndex] - sourceDTRowCount; j++)
                            {
                                DataRow dr = dt.NewRow();
                                dr[0] = (sourceDTRowCount + j + 1).ToString("000");
                                dr[1] = "0.00";
                                dr[2] = "100";
                                dr[3] = "1";
                                dr[4] = "0";
                                dr[5] = "0";
                                dr[6] = "预留";
                                dr[7] = "Spare";
                                dt.Rows.Add(dr.ItemArray);
                            }
                        }
                        DataTable finalAxisPositionDataDT = new DataTable();
                        for (int j = 0; j < dt.Columns.Count; j++)
                        {
                            finalAxisPositionDataDT.Columns.Add(dt.Columns[j].ColumnName);
                        }
                        for (int j = 0; j < _axisPointQuantity[currentAxisIndex]; j++)
                        {
                            DataRow dr = dt.Rows[j];
                            finalAxisPositionDataDT.Rows.Add(dr.ItemArray);
                        }
                        UpdateGV(finalAxisPositionDataDT);
                        dgrdvAxisPositionDataShow.Columns[0].ReadOnly = true;
                        dgrdvAxisPositionDataShow.Columns[0].Frozen = true;
                        dgrdvAxisPositionDataShow.AllowUserToOrderColumns = false;
                        //禁止排序
                        for (int i = 0; i < dgrdvAxisPositionDataShow.Columns.Count; i++)
                        {
                            dgrdvAxisPositionDataShow.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                        }
                    })); ;
                }
                catch
                {
                    MessageBox.Show("读取轴" + (currentAxisIndex + 1).ToString("00") + "点位文件失败!");
                    return;
                }
                try
                {
                    float temp = 0;
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        for (int j = 1; j < dt.Columns.Count - 2; j++)
                        {
                            temp = Convert.ToSingle(dt.Rows[i][j]);
                        }
                    }
                }
                catch
                {
                    MessageBox.Show("轴" + (currentAxisIndex + 1).ToString("00") + "点位文件数据错误!");
                    return;
                }
            }
            else
            {
                MessageBox.Show("轴" + (currentAxisIndex + 1).ToString("00") + "参数文件数据错误!");
                return;
            }
        }
        #endregion

        #region PositionDataSaveEvent：轴点位数据保存
        private void PositionDataSaveEvent(object sender, EventArgs e)
        {
            string axisPositionDataFilePath = Directory.GetCurrentDirectory() + @"\" +
                MyTool.FolderAndFileManageClass.FilterPathIllegalChar("AxisParameterAndPositionData_" + _currentProductName) + @"\" +
                MyTool.FolderAndFileManageClass.FilterPathIllegalChar("axis" + _currentAxisIndex.ToString("00") + "Pos" + "_" + _currentProductName)
                + ".csv";//过滤掉作为Directory不合法的字符
            DataTable dt = GetDgvToTable(dgrdvAxisPositionDataShow);
            if (dt.Rows.Count < _axisPointQuantity[_currentAxisIndex])//如果点位个数小于设定的点位个数
            {
                MessageBox.Show("轴" + (_currentAxisIndex + 1).ToString("00") + "点位文件中读取的点位数据个数小于设定的轴点位个数,利用默认数据填充，请根据实际情况修改此文件!");
                for (int j = 0; j < _axisPointQuantity[_currentAxisIndex] - dt.Rows.Count; j++)
                {
                    DataRow dr = dt.NewRow();
                    dr[0] = (dt.Rows.Count + 1).ToString("000");
                    dr[1] = "0.00";
                    dr[2] = "100";
                    dr[3] = "1";
                    dr[4] = "0";
                    dr[5] = "0";
                    dr[6] = "预留";
                    dr[7] = "Spare";
                    dt.Rows.Add(dr.ItemArray);
                }
            }
            DataTable finalAxisPositionDataDT = new DataTable();
            for (int j = 0; j < dt.Columns.Count; j++)
            {
                finalAxisPositionDataDT.Columns.Add(dt.Columns[j].ColumnName);
            }
            for (int j = 0; j < _axisPointQuantity[_currentAxisIndex]; j++)
            {
                DataRow dr = dt.Rows[j];
                finalAxisPositionDataDT.Rows.Add(dr.ItemArray);
            }

            if (AxisPositionDataCheck(_currentAxisIndex, GetDgvToTable(dgrdvAxisPositionDataShow)))
            {
                if (MessageBox.Show("确认要保存轴点位文件?", "保存轴点位文件", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.OK)
                {
                    try
                    {
                        CSVFileOperation.SaveCSV(finalAxisPositionDataDT, axisPositionDataFilePath);
                        PositionDataView(_currentProductName, _currentAxisIndex);
                    }
                    catch
                    {
                        MessageBox.Show("保存轴" + (_currentAxisIndex + 1).ToString("00") + "点位文件失败!");
                        return;
                    }
                }
                else
                    return;
            }
            else
            {
                MessageBox.Show("数据检测失败，保存轴" + (_currentAxisIndex + 1).ToString("00") + "点位文件失败!");
                return;
            }

            if (AxisPositionDataModifiedEvent != null)
                AxisPositionDataModifiedEvent();
            MessageBox.Show("轴" + (_currentAxisIndex + 1).ToString("00") + "点位文件保存完成！");
        }
        #endregion

        #region AxisPositionDataCheck：轴点位数据检测
        private bool AxisPositionDataCheck(int axisNumber, DataTable dt)
        {
            bool checkResult = false;

            //读取轴参数
            if (AxisParFileInitial())
            {

                float temp = 0;
                float minData = 0;
                float maxData = 0;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    for (int j = 1; j < 5; j++)
                    {
                        //数据格式检查(是否能正常转换为浮点数)
                        try
                        {
                            temp = Convert.ToSingle(dt.Rows[i][j]);
                        }
                        catch
                        {
                            checkResult = false;
                            MessageBox.Show("轴" + (axisNumber + 1).ToString("00") +
                                "点位文件数据错误!错误在第" + (i + 1).ToString("000") + "行," + "第" + (j + 1).ToString("00") + "个数据");
                            return checkResult;
                        }

                        //数据范围检查
                        switch (j)
                        {
                            case 1:
                                minData = _axisSoftLimitMinus[axisNumber];
                                maxData = _axisSoftLimitPlus[axisNumber];
                                break;
                            case 2:
                                minData = _axisMoveMinSpeed_mmps[axisNumber];
                                maxData = _axisMoveMaxSpeed_mmps[axisNumber];
                                break;
                            case 3:
                                minData = _axisMoveMinAccTime_s[axisNumber];
                                maxData = _axisMoveMaxAccTime_s[axisNumber];
                                break;
                            case 4:
                                minData = _axisMinDeviation_mm[axisNumber];
                                maxData = _axisMaxDeviation_mm[axisNumber];
                                break;
                        }

                        if (temp < minData || temp > maxData)
                        {
                            checkResult = false;
                            MessageBox.Show("轴" + (axisNumber + 1).ToString("00") +
                            "点位文件数据超范围!超范围在第" + (i + 1).ToString("000") + "行," + "第" + (j + 1).ToString("00") + "个数据");
                            return checkResult;
                        }
                    }

                    //检测联合FLAG
                    if (dt.Rows[i][5].ToString() != "1" && dt.Rows[i][5].ToString() != "0")
                    {
                        checkResult = false;
                        MessageBox.Show("轴" + (axisNumber + 1).ToString("00") +
                        "点位文件数据超范围!超范围在第" + (i + 1).ToString("000") + "行," + "第6个数据");
                        return checkResult;
                    }
                }
                checkResult = true;
                return checkResult;
            }
            else
            {
                checkResult = false;
                return checkResult;
            }
        }
        #endregion

        #region 轴参数数据显示相关函数
        AxisParameterDataViewForm _axisParameterDataViewFormVariate = null;//轴参数显示画面变量
        private void AxisParameterDisplay(object sender, EventArgs e)
        {
            if (_axisParameterDataViewFormVariate == null)
            {
                _axisParameterDataViewFormVariate = new AxisParameterDataViewForm(_currentProductName);
                _axisParameterDataViewFormVariate.Show();
                _axisParameterDataViewFormVariate.AxisParameterDataViewFormCloseEvent += new AxisParameterDataViewFormCloseDelegate(AxisParameterDataViewFormCloseEventFunction);
                //静态事件
                _axisParameterDataViewFormVariate.AxisParameterDataModifiedEvent += new AxisParameterDataModifiedDelegate(AxisParameterDataModifiedEventFunction);
                return;
            }
            if (_axisParameterDataViewFormVariate != null && _axisParameterDataViewFormVariate.Visible)
            {
                _axisParameterDataViewFormVariate.Dispose();
                _axisParameterDataViewFormVariate = null;
            }
        }

        //轴参数显示页面关闭事件
        private void AxisParameterDataViewFormCloseEventFunction()
        {
            _axisParameterDataViewFormVariate.Dispose();
            _axisParameterDataViewFormVariate = null;
        }

        //轴参数修改事件
        private void AxisParameterDataModifiedEventFunction()
        {
            PositionDataView(_currentProductName, _currentAxisIndex);
            if (AxisParameterDataModifiedEvent != null)
                AxisParameterDataModifiedEvent();
        }
        #endregion

        #region AxisParFileInitial：轴参数初始化
        private bool AxisParFileInitial()
        {
            string axisParameterFilePath; //轴参数文件保存路径
            string axisParameterFileSaveFolderPath;//轴参数保存文件夹路径

            axisParameterFileSaveFolderPath = Directory.GetCurrentDirectory() + @"\" +
                MyTool.FolderAndFileManageClass.FilterPathIllegalChar("AxisParameterAndPositionData_" + _currentProductName) + @"\";//过滤掉作为Directory不合法的字符
            axisParameterFilePath = Path.Combine(axisParameterFileSaveFolderPath, "AxisPar" + ".csv");

            if (!Directory.Exists(axisParameterFileSaveFolderPath))
            {
                Directory.CreateDirectory(axisParameterFileSaveFolderPath);//创建文件夹
            }
            if (!File.Exists(axisParameterFilePath))
            {
                File.Create(axisParameterFilePath).Close();//创建文件
                //初始化文件内容
                CSVFileOperation.SaveCSV_Append(axisParameterFilePath, "AxisNumber(0-N),MinPosition(mm),MaxPosition(mm),MinSpeed(mm/s),MaxSpeed(mm/s),MinAccDecTime(s),MaxAccDecTime(s),MinDeviation(mm),MaxDeviation(mm),StartStopVelocity(mm/s),HomeStartStopVelocity(mm/s),HomeMaxVelocity(mm/s),HomeAccDecTime(s),JogMinSpeed(mm/s),JogMaxSpeed(mm/s),InchMinDistance(mm),InchMaxDistance(mm),ManualSpeedPCT(%),AutoSpeedPCT(%),OnePulseDistance(mm),PointQuantity,AxisUse,AxisName,AxisUnit");
                for (int i = 0; i < _axisQuantity; i++)
                {
                    CSVFileOperation.SaveCSV_Append(axisParameterFilePath, "000,0,1000,1,1000,0,10,0,0,10,1,-1,3,0.1,100,0.001,100,30,100,0.005,120,1,Axis" + i.ToString("00") + ",mm");
                }
                MessageBox.Show("轴参数文件不存在,已创建默认轴参数文件!请根据实际情况修改此文件!");
            }

            //读取参数
            DataTable dt = new DataTable();
            try
            {
                dt = CSVFileOperation.ReadCSV(axisParameterFilePath);
            }
            catch
            {
                MessageBox.Show("读取轴参数文件失败!");
                return false;
            }

            try
            {
                for (int i = 0; i < _axisQuantity; i++)
                {
                    _axisSoftLimitMinus[i] = Convert.ToSingle(dt.Rows[i][1]);
                    _axisSoftLimitPlus[i] = Convert.ToSingle(dt.Rows[i][2]);
                    _axisMoveMinSpeed_mmps[i] = Convert.ToSingle(dt.Rows[i][3]);
                    _axisMoveMaxSpeed_mmps[i] = Convert.ToSingle(dt.Rows[i][4]);
                    _axisMoveMinAccTime_s[i] = Convert.ToSingle(dt.Rows[i][5]);
                    _axisMoveMaxAccTime_s[i] = Convert.ToSingle(dt.Rows[i][6]);
                    _axisMinDeviation_mm[i] = Convert.ToSingle(dt.Rows[i][7]);
                    _axisMaxDeviation_mm[i] = Convert.ToSingle(dt.Rows[i][8]);
                    _axisPointQuantity[i] = Convert.ToInt32(dt.Rows[i][22]);
                    if (_axisPointQuantity[i] > _maxPointQuantity)
                        _axisPointQuantity[i] = _maxPointQuantity;
                }
            }
            catch
            {
                MessageBox.Show("轴参数文件数据错误!");
                return false;
            }
            return true;
        }
        #endregion

        #region 用委托更新data,防止卡顿
        private delegate void UpdateDataGridView(DataTable dt);
        private void UpdateGV(DataTable dt)
        {
            if (dgrdvAxisPositionDataShow.InvokeRequired)
            {
                this.BeginInvoke(new UpdateDataGridView(UpdateGV), new object[] { dt });
            }
            else
            {
                dgrdvAxisPositionDataShow.DataSource = dt;
            }
        }
        #endregion

        //关闭窗口事件
        private void AxisPositionDataViewForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            if (AxisPositionDataViewFormCloseEvent != null)
                AxisPositionDataViewFormCloseEvent();
        }

        //将dataGridView的数据全部放到DataTable
        public DataTable GetDgvToTable(DataGridView dgv)
        {
            //将dataGridView的数据全部放到DataTable
            DataTable dt = new DataTable();
            //强制转换所有列 
            for (int count = 0; count < dgv.Columns.Count; count++)
            {
                DataColumn dc = new DataColumn(dgv.Columns[count].Name.ToString());
                dt.Columns.Add(dc);
            }
            //将所有数据放到DataTable 
            for (int count = 0; count < dgv.Rows.Count; count++)
            {
                DataRow dr = dt.NewRow();
                for (int countsub = 0; countsub < dgv.Columns.Count; countsub++)
                {
                    dr[countsub] = Convert.ToString(dgv.Rows[count].Cells[countsub].Value);
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }

        #region 虚拟键盘相关函数
        //textbox 鼠标双击事件
        private void DataGridViewCellDoubleClickEvent(Object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex >= 1 && e.ColumnIndex <= 5 && e.RowIndex >= 0 && ((DataGridView)sender).IsCurrentCellInEditMode)
            {
                SoftNumberKeyboard.LanguageFlag = 1;//设置语言为中文
                SoftNumberKeyboard numberKeyboard = new SoftNumberKeyboard();
                numberKeyboard.StartPosition = FormStartPosition.CenterScreen;

                if (e.ColumnIndex == 1)//如果为序号为1的列（Position）
                    numberKeyboard.PLUSMINUSBTHIDEFLAG = false;
                else
                    numberKeyboard.PLUSMINUSBTHIDEFLAG = true;

                if (e.ColumnIndex == 5)//如果为序号为5的列（联合）
                    numberKeyboard.FULLSTOPBTHIDEFLAG = true;
                else
                    numberKeyboard.FULLSTOPBTHIDEFLAG = false;

                numberKeyboard.SOURCESTR = ((DataGridView)sender).CurrentCell.Value.ToString();
                numberKeyboard.SENDER = sender;
                numberKeyboard.SendInputDataIncludeObj += new SENDINPUTDATAINCLUDEOBJ(GetKeyboardInputData);//添加事件
                numberKeyboard.Show();
            }
            else if (e.ColumnIndex > 5 && e.RowIndex >= 0 && ((DataGridView)sender).IsCurrentCellInEditMode)
            {
                SoftMainKeyboard.LanguageFlag = 1;
                SoftMainKeyboard mainKeyboard = new SoftMainKeyboard();
                mainKeyboard.StartPosition = FormStartPosition.CenterScreen;
                mainKeyboard.SOURCESTR = ((DataGridView)sender).CurrentCell.Value.ToString();
                mainKeyboard.SENDER = sender;
                mainKeyboard.SendInputDataIncludeObj += new SENDINPUTDATAINCLUDEOBJ(GetKeyboardInputData);//添加事件
                mainKeyboard.ShowDialog();
            }
        }

        //键盘按下OK之后获取数据
        private void GetKeyboardInputData(object sender, string GetDataStr)
        {
            ((DataGridView)sender).CurrentCell.Value = GetDataStr;
            ((DataGridView)sender).RefreshEdit();//不加上这句话的话，那么当currentcell的值发生变化之后，edit的值不会变化
        }

        //Datagridview cell数据检查函数
        private void DataCheckEvent(object sender, DataGridViewCellEventArgs e)
        {
            if (((DataGridViewCellEventArgs)e).RowIndex >= 0 && ((DataGridViewCellEventArgs)e).ColumnIndex >= 1 && ((DataGridViewCellEventArgs)e).ColumnIndex <= 5)
            {
                switch (((DataGridViewCellEventArgs)e).ColumnIndex)
                {
                    case 1://为Position
                        {
                            ((DataGridView)sender).Rows[((DataGridViewCellEventArgs)e).RowIndex].Cells[((DataGridViewCellEventArgs)e).ColumnIndex].Value =
                                MyTool.DataProcessing.TextBoxDataCheck(((DataGridView)sender).Rows[((DataGridViewCellEventArgs)e).RowIndex].Cells[((DataGridViewCellEventArgs)e).ColumnIndex].Value.ToString(),
                                _axisSoftLimitMinus[_currentAxisIndex], _axisSoftLimitPlus[_currentAxisIndex], true, true, true, false);
                            break;
                        }
                    case 2://为Speed
                        {
                            ((DataGridView)sender).Rows[((DataGridViewCellEventArgs)e).RowIndex].Cells[((DataGridViewCellEventArgs)e).ColumnIndex].Value =
                                MyTool.DataProcessing.TextBoxDataCheck(((DataGridView)sender).Rows[((DataGridViewCellEventArgs)e).RowIndex].Cells[((DataGridViewCellEventArgs)e).ColumnIndex].Value.ToString(),
                                _axisMoveMinSpeed_mmps[_currentAxisIndex], _axisMoveMaxSpeed_mmps[_currentAxisIndex], true, true, true, false);
                            break;
                        }
                    case 3://为AccTime
                        {
                            ((DataGridView)sender).Rows[((DataGridViewCellEventArgs)e).RowIndex].Cells[((DataGridViewCellEventArgs)e).ColumnIndex].Value =
                                MyTool.DataProcessing.TextBoxDataCheck(((DataGridView)sender).Rows[((DataGridViewCellEventArgs)e).RowIndex].Cells[((DataGridViewCellEventArgs)e).ColumnIndex].Value.ToString(),
                                _axisMoveMinAccTime_s[_currentAxisIndex], _axisMoveMaxAccTime_s[_currentAxisIndex], true, true, true, false);
                            break;
                        }
                    case 4://Deviation
                        {
                            ((DataGridView)sender).Rows[((DataGridViewCellEventArgs)e).RowIndex].Cells[((DataGridViewCellEventArgs)e).ColumnIndex].Value =
                                MyTool.DataProcessing.TextBoxDataCheck(((DataGridView)sender).Rows[((DataGridViewCellEventArgs)e).RowIndex].Cells[((DataGridViewCellEventArgs)e).ColumnIndex].Value.ToString(),
                                -9999999, 9999999, true, true, true, false);
                            break;
                        }
                    case 5://Union
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
        private void LastDataCheckEvent(object sender, DataGridViewCellEventArgs e)
        {
            if (((DataGridViewCellEventArgs)e).RowIndex >= 0 && ((DataGridViewCellEventArgs)e).ColumnIndex >= 1 && ((DataGridViewCellEventArgs)e).ColumnIndex <= 5)
            {
                switch (((DataGridViewCellEventArgs)e).ColumnIndex)
                {
                    case 1://为Position
                        {
                            ((DataGridView)sender).Rows[((DataGridViewCellEventArgs)e).RowIndex].Cells[((DataGridViewCellEventArgs)e).ColumnIndex].Value =
                                MyTool.DataProcessing.TextBoxDataCheck(((DataGridView)sender).Rows[((DataGridViewCellEventArgs)e).RowIndex].Cells[((DataGridViewCellEventArgs)e).ColumnIndex].Value.ToString(),
                                _axisSoftLimitMinus[_currentAxisIndex], _axisSoftLimitPlus[_currentAxisIndex], true, true, true, true);
                            break;
                        }
                    case 2://为Speed
                        {
                            ((DataGridView)sender).Rows[((DataGridViewCellEventArgs)e).RowIndex].Cells[((DataGridViewCellEventArgs)e).ColumnIndex].Value =
                                MyTool.DataProcessing.TextBoxDataCheck(((DataGridView)sender).Rows[((DataGridViewCellEventArgs)e).RowIndex].Cells[((DataGridViewCellEventArgs)e).ColumnIndex].Value.ToString(),
                                _axisMoveMinSpeed_mmps[_currentAxisIndex], _axisMoveMaxSpeed_mmps[_currentAxisIndex], true, true, true, true);
                            break;
                        }
                    case 3://为AccTime
                        {
                            ((DataGridView)sender).Rows[((DataGridViewCellEventArgs)e).RowIndex].Cells[((DataGridViewCellEventArgs)e).ColumnIndex].Value =
                                MyTool.DataProcessing.TextBoxDataCheck(((DataGridView)sender).Rows[((DataGridViewCellEventArgs)e).RowIndex].Cells[((DataGridViewCellEventArgs)e).ColumnIndex].Value.ToString(),
                                _axisMoveMinAccTime_s[_currentAxisIndex], _axisMoveMaxAccTime_s[_currentAxisIndex], true, true, true, true);
                            break;
                        }
                    case 4://Deviation
                        {
                            ((DataGridView)sender).Rows[((DataGridViewCellEventArgs)e).RowIndex].Cells[((DataGridViewCellEventArgs)e).ColumnIndex].Value =
                                MyTool.DataProcessing.TextBoxDataCheck(((DataGridView)sender).Rows[((DataGridViewCellEventArgs)e).RowIndex].Cells[((DataGridViewCellEventArgs)e).ColumnIndex].Value.ToString(),
                                -9999999, 9999999, true, true, true, true);
                            break;
                        }
                    case 5://Union
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

        #region 参数文件的导入导出
        //轴参数文件的导入
        private void btnImport_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            DataTable dt = null;

            ofd.Title = "导入轴参数文件";
            ofd.FileName = MyTool.FolderAndFileManageClass.FilterPathIllegalChar("axis" + _currentAxisIndex.ToString("00") + "Pos_" + _currentProductName) + ".csv";
            ofd.Filter = "csv(*.csv)|*.csv";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                if (AxisParFileInitial())
                {
                    try
                    {
                        dgrdvAxisPositionDataShow.Invoke(new Action(() =>
                        {
                            dt = CSVFileOperation.ReadCSV(ofd.FileName);
                            if (dt.Rows.Count < _axisPointQuantity[_currentAxisIndex])//如果点位个数小于设定的点位个数
                            {
                                MessageBox.Show("导入的轴" + (_currentAxisIndex + 1).ToString("00") + "点位文件中读取的点位数据个数小于设定的轴点位个数,利用默认数据填充，请根据实际情况修改此文件!");
                                for (int j = 0; j < _axisPointQuantity[_currentAxisIndex] - dt.Rows.Count; j++)
                                {
                                    DataRow dr = dt.NewRow();
                                    dr[0] = (dt.Rows.Count + j + 1).ToString("000");
                                    dr[1] = "0.00";
                                    dr[2] = "100";
                                    dr[3] = "1";
                                    dr[4] = "0";
                                    dr[5] = "0";
                                    dr[6] = "预留";
                                    dr[7] = "Spare";
                                    dt.Rows.Add(dr.ItemArray);
                                }
                            }
                            DataTable finalAxisPositionDataDT = new DataTable();
                            for (int j = 0; j < dt.Columns.Count; j++)
                            {
                                finalAxisPositionDataDT.Columns.Add(dt.Columns[j].ColumnName);
                            }
                            for (int j = 0; j < _axisPointQuantity[_currentAxisIndex]; j++)
                            {
                                DataRow dr = dt.Rows[j];
                                finalAxisPositionDataDT.Rows.Add(dr.ItemArray);
                            }
                            UpdateGV(finalAxisPositionDataDT);
                            dgrdvAxisPositionDataShow.Columns[0].ReadOnly = true;
                            dgrdvAxisPositionDataShow.Columns[0].Frozen = true;
                            dgrdvAxisPositionDataShow.AllowUserToOrderColumns = false;
                            //禁止排序
                            for (int i = 0; i < dgrdvAxisPositionDataShow.Columns.Count; i++)
                            {
                                dgrdvAxisPositionDataShow.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                            }
                        })); ;
                    }
                    catch
                    {
                        MessageBox.Show("导入轴" + (_currentAxisIndex + 1).ToString("00") + "点位文件失败!");
                        return;
                    }
                    try
                    {
                        float temp = 0;
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            for (int j = 1; j < dt.Columns.Count - 2; j++)
                            {
                                temp = Convert.ToSingle(dt.Rows[i][j]);
                            }
                        }
                    }
                    catch
                    {
                        MessageBox.Show("导入的轴" + (_currentAxisIndex + 1).ToString("00") + "的点位文件数据错误!");
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("轴" + (_currentAxisIndex + 1).ToString("00") + "的参数文件数据错误!");
                }

                MessageBox.Show("导入轴" + (_currentAxisIndex + 1).ToString("00") + "点位文件成功!");
            }
        }

        //轴参数文件的导出
        private void btnOutport_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = "导出轴参数文件";
            sfd.Filter = "csv(*.csv)|*.csv";
            sfd.FileName = MyTool.FolderAndFileManageClass.FilterPathIllegalChar("axis" + _currentAxisIndex.ToString("00") + "Pos_" + _currentProductName) + ".csv";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                DataTable dt = GetDgvToTable(dgrdvAxisPositionDataShow);
                if (dt.Rows.Count < _axisPointQuantity[_currentAxisIndex])//如果点位个数小于设定的点位个数
                {
                    MessageBox.Show("要导出的轴" + (_currentAxisIndex + 1).ToString("00") + "点位文件中读取的点位数据个数小于设定的轴点位个数,利用默认数据填充，请根据实际情况修改此文件!");
                    for (int j = 0; j < _axisPointQuantity[_currentAxisIndex] - dt.Rows.Count; j++)
                    {
                        DataRow dr = dt.NewRow();
                        dr[0] = (dt.Rows.Count + j + 1).ToString("000");
                        dr[1] = "0.00";
                        dr[2] = "100";
                        dr[3] = "1";
                        dr[4] = "0";
                        dr[5] = "0";
                        dr[6] = "预留";
                        dr[7] = "Spare";
                        dt.Rows.Add(dr.ItemArray);
                    }
                }
                DataTable finalAxisPositionDataDT = new DataTable();
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    finalAxisPositionDataDT.Columns.Add(dt.Columns[j].ColumnName);
                }
                for (int j = 0; j < _axisPointQuantity[_currentAxisIndex]; j++)
                {
                    DataRow dr = dt.Rows[j];
                    finalAxisPositionDataDT.Rows.Add(dr.ItemArray);
                }
                try
                {
                    CSVFileOperation.SaveCSV(finalAxisPositionDataDT, sfd.FileName);
                }
                catch
                {
                    MessageBox.Show("导出轴" + (_currentAxisIndex + 1).ToString("00") + "点位文件失败!");
                    return;
                }


                MessageBox.Show("导出轴" + (_currentAxisIndex + 1).ToString("00") + "点位文件完成！");
            }
        }


        #endregion



    }
}
