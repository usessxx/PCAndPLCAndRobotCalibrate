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
    public delegate void AxisParameterDataViewFormCloseDelegate();//声明委托,用于关闭轴参数数据显示窗口
    public delegate void AxisParameterDataModifiedDelegate();//声明委托,用于轴参数数据修改后主窗口显示更新

    public partial class AxisParameterDataViewForm : Form
    {

        /*--------------------------------------------------------------------------------------
        //Copyright (C) 2022 深圳市利器精工科技有限公司
        //版权所有
        //
        //文件名：轴参数显示编辑页面
        //文件功能描述：显示轴的设定参数，便于编辑设定轴参数
        //
        //
        //创建标识：MaLi 20220308
        //
        //修改标识：MaLi 20220308 Change
        //修改描述：增加轴参数显示编辑窗口
        //
        //
        //------------------------------------------------------------------------------------*/
        public event AxisParameterDataViewFormCloseDelegate AxisParameterDataViewFormCloseEvent;//声明关闭轴参数数据显示窗口事件
        public  event AxisParameterDataModifiedDelegate AxisParameterDataModifiedEvent;//声明轴参数数据修改事件
        const int MAX_POINT_QUANTITY = 900;//最大点位个数
        string _currentProductName = "Default";//当前产品名称

        public AxisParameterDataViewForm(string currentProductName)
        {
            InitializeComponent();
            _currentProductName = currentProductName;

            dgrdvAxisParameterShow.CellDoubleClick += DataGridViewCellDoubleClickEvent;//声明事件，用于打开软键盘
            dgrdvAxisParameterShow.CellLeave += LastDataCheckEvent;//声明事件，用于最后检查数据
            dgrdvAxisParameterShow.CellValueChanged += DataCheckEvent;//声明事件，用于检查数据
        }

        //轴参数数据显示
        public void AxisParameterDataDisplay(string currentProductName)
        {
            string axisParameterFilePath = Directory.GetCurrentDirectory() + @"\" +
               MyTool.FolderAndFileManageClass.FilterPathIllegalChar("AxisParameterAndPositionData_" + currentProductName) + @"\AxisPar.csv";//过滤掉作为Directory不合法的字符

            DataTable dt = new DataTable();
            try
            {
                dt = CSVFileOperation.ReadCSV(axisParameterFilePath);
                dgrdvAxisParameterShow.DataSource = dt;
                dgrdvAxisParameterShow.Columns[0].ReadOnly = true;
                dgrdvAxisParameterShow.Columns[0].Frozen = true;
                dgrdvAxisParameterShow.AllowUserToOrderColumns = false;
                //禁止排序
                for (int i = 0; i < dgrdvAxisParameterShow.Columns.Count; i++)
                {
                    dgrdvAxisParameterShow.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                }
            }
            catch
            {
                MessageBox.Show("读取轴参数文件失败!");
                dgrdvAxisParameterShow.DataSource = dt;
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
                MessageBox.Show("轴参数文件数据错误!");
                dgrdvAxisParameterShow.DataSource = dt;
                return;
            }
        }

        private void AxisParameterDataViewForm_Load(object sender, EventArgs e)
        {
            AxisParameterDataDisplay(_currentProductName);
        }

        //轴参数保存事件
        private void AxisParameterDataSaveEvent(object sender, EventArgs e)
        {
            string axisParameterFilePath = Directory.GetCurrentDirectory() + @"\" +
              MyTool.FolderAndFileManageClass.FilterPathIllegalChar("AxisParameterAndPositionData_" + _currentProductName) + @"\AxisPar.csv";//过滤掉作为Directory不合法的字符
            DataTable dt = GetDgvToTable(dgrdvAxisParameterShow);
            if (AxisParameterDataCheck(GetDgvToTable(dgrdvAxisParameterShow)))
            {
                if (MessageBox.Show("确认要保存轴参数文件?", "保存轴参数文件", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                {
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
                        MessageBox.Show("轴参数文件数据错误!");
                        return;
                    }
                    try
                    {
                        CSVFileOperation.SaveCSV(dt, axisParameterFilePath);
                        AxisParameterDataDisplay(_currentProductName);

                        if (AxisParameterDataModifiedEvent != null)
                            AxisParameterDataModifiedEvent();
                        MessageBox.Show("轴参数保存完成！");
                    }
                    catch
                    {
                        MessageBox.Show("保存轴参数文件失败!");
                    }
                }
            }
        }

        //轴参数数据检查
        private bool AxisParameterDataCheck(DataTable dt)
        {
            bool checkResult = false;

            float temp = 0;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                for (int j = 1; j < dt.Columns.Count-5; j++)
                {
                    //数据格式检查(是否能正常转换为浮点数)
                    try
                    {
                        temp = Convert.ToSingle(dt.Rows[i][j]);
                    }
                    catch
                    {
                        checkResult = false;
                        MessageBox.Show("轴参数文件数据错误!错误在第" + (i + 1).ToString("000") + "行," + 
                            "第" + (j + 1).ToString("00") + "个数据");
                        return checkResult;
                    }
                }
                //数据格式检查(是否能正常转换为浮点数)，点位个数
                try
                {
                    temp = Convert.ToInt32(dt.Rows[i][20]);
                    if (temp < 0 || temp > MAX_POINT_QUANTITY)
                    {
                        checkResult = false;
                        MessageBox.Show("轴参数文件数据错误!错误在第" + (i + 1).ToString("000") + "行," +
                            "第" + (dt.Columns.Count + 1).ToString("00") + "个数据，数据应当大于0，小于"+MAX_POINT_QUANTITY.ToString());
                        return checkResult;
                    }
                }
                catch
                {
                    checkResult = false;
                    MessageBox.Show("轴参数文件数据错误!错误在第" + (i + 1).ToString("000") + "行," +
                        "第" + (dt.Columns.Count + 1).ToString("00") + "个数据");
                    return checkResult;
                }
            }
            checkResult = true;
            return checkResult;
        }

        //将dataGridView的数据全部放到DataTable
        public DataTable GetDgvToTable(DataGridView dgv)
        {
            //将dataGridView的数据全部放到DataTable 
            DataTable dt = new DataTable();
            //强制转换所有列 
            for (int count = 0; count < dgv.Columns.Count; count++)// dgv.Columns.Count是所有列 
            {
                DataColumn dc = new DataColumn(dgv.Columns[count].Name.ToString());
                dt.Columns.Add(dc);
            }
            //将所有数据放到DataTable 
            for (int count = 0; count < dgv.Rows.Count; count++)//dgv.Rows.Count所有列数 
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

        private void AxisParameterDataViewForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            if (AxisParameterDataViewFormCloseEvent != null)
                AxisParameterDataViewFormCloseEvent();
        }

        #region 虚拟键盘相关函数
        //textbox 鼠标双击事件
        private void DataGridViewCellDoubleClickEvent(Object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex >= 1 && e.ColumnIndex <= 21 && e.RowIndex >= 0 && ((DataGridView)sender).IsCurrentCellInEditMode)
            {
                SoftNumberKeyboard.LanguageFlag = 1;//设置语言为中文
                SoftNumberKeyboard numberKeyboard = new SoftNumberKeyboard();
                numberKeyboard.StartPosition = FormStartPosition.CenterScreen;

                if (e.ColumnIndex == 1 || e.ColumnIndex == 2)//如果为序号为1的列（Position）
                    numberKeyboard.PLUSMINUSBTHIDEFLAG = false;
                else
                    numberKeyboard.PLUSMINUSBTHIDEFLAG = true;

                if (e.ColumnIndex != 20 && e.ColumnIndex != 21)//如果不为点位个数
                    numberKeyboard.FULLSTOPBTHIDEFLAG = false;
                else
                    numberKeyboard.FULLSTOPBTHIDEFLAG = true;

                numberKeyboard.SOURCESTR = ((DataGridView)sender).CurrentCell.Value.ToString();
                numberKeyboard.SENDER = sender;
                numberKeyboard.SendInputDataIncludeObj += new SENDINPUTDATAINCLUDEOBJ(GetKeyboardInputData);//添加事件
                numberKeyboard.Show();
            }
            else if (e.ColumnIndex > 21 && e.RowIndex >= 0 && ((DataGridView)sender).IsCurrentCellInEditMode)
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
            if (((DataGridViewCellEventArgs)e).RowIndex >= 0 && ((DataGridViewCellEventArgs)e).ColumnIndex >= 1 && ((DataGridViewCellEventArgs)e).ColumnIndex <= 20)
            {
                if (((DataGridViewCellEventArgs)e).ColumnIndex == 1)//如果为位置
                    ((DataGridView)sender).Rows[((DataGridViewCellEventArgs)e).RowIndex].Cells[((DataGridViewCellEventArgs)e).ColumnIndex].Value =
                        MyTool.DataProcessing.TextBoxDataCheck(((DataGridView)sender).Rows[((DataGridViewCellEventArgs)e).RowIndex].Cells[((DataGridViewCellEventArgs)e).ColumnIndex].Value.ToString(),
                        -9999999, 9999999, true, true, true, false);
                else if (((DataGridViewCellEventArgs)e).ColumnIndex > 1 && ((DataGridViewCellEventArgs)e).ColumnIndex < 20)
                    ((DataGridView)sender).Rows[((DataGridViewCellEventArgs)e).RowIndex].Cells[((DataGridViewCellEventArgs)e).ColumnIndex].Value =
                        MyTool.DataProcessing.TextBoxDataCheck(((DataGridView)sender).Rows[((DataGridViewCellEventArgs)e).RowIndex].Cells[((DataGridViewCellEventArgs)e).ColumnIndex].Value.ToString(),
                        0, 9999999, true, true, true, false);
                else if (((DataGridViewCellEventArgs)e).ColumnIndex == 20)
                    ((DataGridView)sender).Rows[((DataGridViewCellEventArgs)e).RowIndex].Cells[((DataGridViewCellEventArgs)e).ColumnIndex].Value =
                                   MyTool.DataProcessing.TextBoxDataCheck(((DataGridView)sender).Rows[((DataGridViewCellEventArgs)e).RowIndex].Cells[((DataGridViewCellEventArgs)e).ColumnIndex].Value.ToString(),
                                   1, MAX_POINT_QUANTITY, true, true, false, false);
                else if (((DataGridViewCellEventArgs)e).ColumnIndex == 21)
                    ((DataGridView)sender).Rows[((DataGridViewCellEventArgs)e).RowIndex].Cells[((DataGridViewCellEventArgs)e).ColumnIndex].Value =
                    MyTool.DataProcessing.TextBoxDataCheck(((DataGridView)sender).Rows[((DataGridViewCellEventArgs)e).RowIndex].Cells[((DataGridViewCellEventArgs)e).ColumnIndex].Value.ToString(),
                    0, 1, true, true, false, false);
            }
        }

        //Datagridview cell数据检查函数
        private void LastDataCheckEvent(object sender, DataGridViewCellEventArgs e)
        {
            if (((DataGridViewCellEventArgs)e).RowIndex >= 0 && ((DataGridViewCellEventArgs)e).ColumnIndex >= 1 && ((DataGridViewCellEventArgs)e).ColumnIndex <= 20)
            {
                if (((DataGridViewCellEventArgs)e).ColumnIndex == 1)//如果为位置
                    ((DataGridView)sender).Rows[((DataGridViewCellEventArgs)e).RowIndex].Cells[((DataGridViewCellEventArgs)e).ColumnIndex].Value =
                        MyTool.DataProcessing.TextBoxDataCheck(((DataGridView)sender).Rows[((DataGridViewCellEventArgs)e).RowIndex].Cells[((DataGridViewCellEventArgs)e).ColumnIndex].Value.ToString(),
                        -9999999, 9999999, true, true, true, true);
                else if (((DataGridViewCellEventArgs)e).ColumnIndex > 1 && ((DataGridViewCellEventArgs)e).ColumnIndex < 20)
                    ((DataGridView)sender).Rows[((DataGridViewCellEventArgs)e).RowIndex].Cells[((DataGridViewCellEventArgs)e).ColumnIndex].Value =
                        MyTool.DataProcessing.TextBoxDataCheck(((DataGridView)sender).Rows[((DataGridViewCellEventArgs)e).RowIndex].Cells[((DataGridViewCellEventArgs)e).ColumnIndex].Value.ToString(),
                        0, 9999999, true, true, true, true);
                else if (((DataGridViewCellEventArgs)e).ColumnIndex == 20)
                    ((DataGridView)sender).Rows[((DataGridViewCellEventArgs)e).RowIndex].Cells[((DataGridViewCellEventArgs)e).ColumnIndex].Value =
                    MyTool.DataProcessing.TextBoxDataCheck(((DataGridView)sender).Rows[((DataGridViewCellEventArgs)e).RowIndex].Cells[((DataGridViewCellEventArgs)e).ColumnIndex].Value.ToString(),
                    1, MAX_POINT_QUANTITY, true, true, false, true);
                else if (((DataGridViewCellEventArgs)e).ColumnIndex == 21)
                    ((DataGridView)sender).Rows[((DataGridViewCellEventArgs)e).RowIndex].Cells[((DataGridViewCellEventArgs)e).ColumnIndex].Value =
                    MyTool.DataProcessing.TextBoxDataCheck(((DataGridView)sender).Rows[((DataGridViewCellEventArgs)e).RowIndex].Cells[((DataGridViewCellEventArgs)e).ColumnIndex].Value.ToString(),
                    0, 1, true, true, false, true);
            }
        }
        #endregion

        #region 参数文件的导入导出
        //轴参数文件的导入
        private void btnImport_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "导入轴参数文件";
            ofd.Filter = "csv(*.csv)|*.csv";
            ofd.FileName = "AxisPar";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                DataTable dt = new DataTable();
                try
                {
                    dt = CSVFileOperation.ReadCSV(ofd.FileName);
                    dgrdvAxisParameterShow.DataSource = dt;
                    dgrdvAxisParameterShow.Columns[0].ReadOnly = true;
                    dgrdvAxisParameterShow.Columns[0].Frozen = true;
                    dgrdvAxisParameterShow.AllowUserToOrderColumns = false;
                    //禁止排序
                    for (int i = 0; i < dgrdvAxisParameterShow.Columns.Count; i++)
                    {
                        dgrdvAxisParameterShow.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                    }
                }
                catch
                {
                    MessageBox.Show("导入轴参数文件失败!");
                    dgrdvAxisParameterShow.DataSource = dt;
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
                    MessageBox.Show("导入的轴参数文件数据错误，导入失败!");
                    dgrdvAxisParameterShow.DataSource = dt;
                    return;
                }
            }
        }

        //轴参数文件的导出
        private void btnOutport_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = "导出轴参数文件";
            sfd.Filter = "csv(*.csv)|*.csv";
            sfd.FileName = "AxisPar";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                DataTable dt = GetDgvToTable(dgrdvAxisParameterShow);
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
                    MessageBox.Show("轴参数文件数据错误，无法导出!");
                    return;
                }
                try
                {
                    CSVFileOperation.SaveCSV(dt, sfd.FileName);
                    MessageBox.Show("轴参数导出完成！");
                }
                catch
                {
                    MessageBox.Show("导出轴参数文件失败!");
                }
            }
        }


        #endregion
    }
    
}
