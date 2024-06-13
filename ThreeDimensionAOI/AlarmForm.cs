using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.IO;

namespace ThreeDimensionAVI
{
    public delegate void AlarmFormClickAlarmResetButtonDelegate(int virtualButtonIndex);//声明委托-点击报警复位按钮
    public partial class AlarmForm : Form
    {
        /*--------------------------------------------------------------------------------------
        //Copyright (C) 2022 深圳市利器精工科技有限公司
        //版权所有
        //
        //文件名：报警窗口
        //文件功能描述：用于显示报警信息和报警记录信息
        //
        //
        //创建标识：MaLi 20220317
        //
        //修改标识：MaLi 20220317 Change
        //修改描述：增加报警窗口
        //
        //------------------------------------------------------------------------------------*/
        //
        /*******************************事件**************************************/
        public event AlarmFormClickAlarmResetButtonDelegate AlarmFormClickAlarmRestButtonEvent;//声明事件-点击报警复位按钮
        /*************************外部可设定读取参数*******************************/

        //*************************公共静态变量*******************************//

        //*************************内部私有变量*******************************//
        const int ALARM_KIND_QUANTITY = 700;//报警，分为致命报警（index:0-99），重度报警(index:100-299)，中度报警(index:300-499)，轻度报警(index:500-699)
        DataTable _alarmDT = null;//用于临时存储报警信息
        DataTable _alarmRecordDT = null;//报警记录读取参数临时存储读取到的数据的DataTable
        //设定报警记录文件保存路径及保存名称
        string _alarmLogFileSavePath = Directory.GetCurrentDirectory() + "\\Alarm Record";
        string _alarmLogFileSaveName = "AlarmRecord.txt";
        int _alarmLogFileSizeLimit = 1024;//报警记录文件限制大小，1024KB
        int _alarmFormFlag = 0;//0-代表当前为显示当前报警的界面，1-代表为显示报警记录的页面

        System.Windows.Forms.Timer _alarmInfoRefreshTimer = null;//创建Form的Timer用于更新报警信息
        //
        //

        public AlarmForm()
        {
            InitializeComponent();

            //初始化报警Datatable
            _alarmDT = new DataTable();
            for (int i = 0; i < dgrdvAlarmInfo.ColumnCount; i++)
            {
                _alarmDT.Columns.Add(dgrdvAlarmInfo.Columns[i].Name, typeof(string));
            }
            _alarmDT.Columns.Add("AlarmIndex", typeof(int));

            //设置整行选中
            this.dgrdvAlarmInfo.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            //设置选中颜色
            dgrdvAlarmInfo.DefaultCellStyle.SelectionBackColor = Color.LightSkyBlue;
            //设置cell高度
            dgrdvAlarmInfo.RowTemplate.Height = 28;

            _alarmInfoRefreshTimer = new System.Windows.Forms.Timer();
            _alarmInfoRefreshTimer.Interval = 100;
            _alarmInfoRefreshTimer.Tick += new System.EventHandler(Refresh);
            _alarmInfoRefreshTimer.Start();

        }

        private void AlarmForm_Load(object sender, EventArgs e)
        {
            
        }

        //实时更新报警信息以及标签等
        private void Refresh(object sender, EventArgs e)
        {
            #region 实时更新当前报警信息

            int rowIndex = _alarmDT.Rows.Count;
            bool alarmStatusChangeFlag = false;

            for (int i = 0; i < ALARM_KIND_QUANTITY; i++)
            {
                if (BaseForm._stateAndAlarmCs._alarmFlag[i] != BaseForm._stateAndAlarmCs._oldAlarmFlag[i])
                {
                    alarmStatusChangeFlag = true;
                    //OldAlarmFlag[i] = AlarmFlag[i];
                    break;
                }
            }
            if (alarmStatusChangeFlag)
            {
                for (int i = 0; i < ALARM_KIND_QUANTITY; i++)
                {
                    if (BaseForm._stateAndAlarmCs._alarmFlag[i] != BaseForm._stateAndAlarmCs._oldAlarmFlag[i] && BaseForm._stateAndAlarmCs._alarmFlag[i])
                    {
                        _alarmDT.Rows.Add();
                        _alarmDT.Rows[rowIndex][0] = (rowIndex + 1).ToString("00000");
                        _alarmDT.Rows[rowIndex][1] = BaseForm._stateAndAlarmCs._alarmMsg[i];
                        _alarmDT.Rows[rowIndex][2] = string.Format("{0:dd/MM/yyyy HH:mm:ss}", DateTime.Now);
                        _alarmDT.Rows[rowIndex][3] = i;

                        //添加至alarmlog文件中
                        MyTool.TxtFileProcess.CreateLog(_alarmDT.Rows[rowIndex][2] + "->" + _alarmDT.Rows[rowIndex][1] + "->" + i.ToString(), 
                            _alarmLogFileSavePath, _alarmLogFileSaveName, true, _alarmLogFileSizeLimit);
                        rowIndex++;
                        BaseForm._stateAndAlarmCs._oldAlarmFlag[i] = BaseForm._stateAndAlarmCs._alarmFlag[i];
                        
                    }
                }
                for (int i = 0; i < _alarmDT.Rows.Count; i++)
                {
                    if (!BaseForm._stateAndAlarmCs._alarmFlag[Convert.ToInt32(_alarmDT.Rows[i][3])])
                    {
                        BaseForm._stateAndAlarmCs._oldAlarmFlag[Convert.ToInt32(_alarmDT.Rows[i][3])] = BaseForm._stateAndAlarmCs._alarmFlag[Convert.ToInt32(_alarmDT.Rows[i][3])];
                        _alarmDT.Rows[i].Delete();   
                    }
                }
            }

            if (alarmStatusChangeFlag  && _alarmFormFlag==0)
            {
                dgrdvAlarmInfo.RowCount = 0;
                for (int i = 0; i < _alarmDT.Rows.Count; i++)
                {
                    dgrdvAlarmInfo.Rows.Add();
                    for (int j = 0; j < 3; j++)
                    {
                        if (j == 0)
                            dgrdvAlarmInfo.Rows[i].Cells[j].Value = (i + 1).ToString("D5");
                        else
                            dgrdvAlarmInfo.Rows[i].Cells[j].Value = _alarmDT.Rows[i][j];
                    }
                }
            }

            #endregion
        }

        private void AlarmForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            MyTool.TxtFileProcess.CreateLog("报警界面-关闭页面！" + "----用户：" + BaseForm._operatorName,
            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

            this.Hide();
            e.Cancel = true;
        }

        //format事件，确保选中行之后，字体颜色依旧为之前的颜色
        private void Alarm_Disp_DataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            e.CellStyle.SelectionForeColor = e.CellStyle.ForeColor;
        }

        #region 虚拟按钮控制相关

        //当按下虚拟按钮事件
        private void PressVirtualBTEvent(object sender, MouseEventArgs e)
        {
            MyTool.TxtFileProcess.CreateLog("报警界面-点击复位按钮！" + "----用户：" + BaseForm._operatorName,
            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

            if (AlarmFormClickAlarmRestButtonEvent != null)
            {
                AlarmFormClickAlarmRestButtonEvent(12);
            }
        }

        //当鼠标离开虚拟按钮事件
        private void LeaveVirtualBTEvet(object sender, MouseEventArgs e)
        {
            if (AlarmFormClickAlarmRestButtonEvent != null)
            {
                AlarmFormClickAlarmRestButtonEvent(-1);
            }
        }

        #endregion

        //报警页面隐藏显示变换事件
        private void AlarmForm_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                _alarmFormFlag = 0;
                dgrdvAlarmInfo.RowCount = 0;
                for (int i = 0; i < _alarmDT.Rows.Count; i++)
                {
                    dgrdvAlarmInfo.Rows.Add();
                    for (int j = 0; j < 3; j++)
                    {
                        if (j == 0)
                            dgrdvAlarmInfo.Rows[i].Cells[j].Value = (i + 1).ToString("D5");
                        else
                            dgrdvAlarmInfo.Rows[i].Cells[j].Value = _alarmDT.Rows[i][j];
                    }
                }
            }
        }

        //报警记录和报警页面切换
        private void btnAlarmAndAlarmRecordExchange_Click(object sender, EventArgs e)
        {
         if (_alarmFormFlag == 0)
            {
                MyTool.TxtFileProcess.CreateLog("报警界面-切换页面至报警记录页面！" + "----用户：" + BaseForm._operatorName,
               BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);
                
                _alarmFormFlag = 1;
                dgrdvAlarmInfo.RowCount = 0;
            }
            else
            {
                MyTool.TxtFileProcess.CreateLog("报警界面-切换页面至报警页面！" + "----用户：" + BaseForm._operatorName,
                BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);
               
                _alarmFormFlag = 0;
                dgrdvAlarmInfo.RowCount = 0;
                for (int i = 0; i < _alarmDT.Rows.Count; i++)
                {
                    dgrdvAlarmInfo.Rows.Add();
                    for (int j = 0; j < 3; j++)
                    {
                        if (j == 0)
                            dgrdvAlarmInfo.Rows[i].Cells[j].Value = (i + 1).ToString("D5");
                        else
                            dgrdvAlarmInfo.Rows[i].Cells[j].Value = _alarmDT.Rows[i][j];
                    }
                }
                _alarmRecordDT = null;
            }
            
            if (_alarmFormFlag == 0)
                btnLoadAlarmRecord.Visible = false;
            else
                btnLoadAlarmRecord.Visible = true;
        }

        //点击读取报警记录按钮
        private void btnLoadAlarmRecord_Click(object sender, EventArgs e)
        {
            MyTool.TxtFileProcess.CreateLog("报警界面-读取报警记录文件！" + "----用户：" + BaseForm._operatorName,
            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);
            
            FileDialog openFile = new OpenFileDialog();
            openFile.AddExtension = true;
            openFile.InitialDirectory = _alarmLogFileSavePath;
            openFile.Filter = "*.txt|*.TXT";
            openFile.Title = "请选择想要读取的文件";
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                MyTool.TxtFileProcess.CreateLog("报警界面-读取报警记录文件" + openFile.FileName + "----用户：" + BaseForm._operatorName,
                BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                _alarmRecordDT = MyTool.TxtFileProcess.LoadAlarmRecordData(openFile.FileName);
                dgrdvAlarmInfo.RowCount = 0;
                for (int i = 0; i < _alarmRecordDT.Rows.Count; i++)
                {
                    dgrdvAlarmInfo.Rows.Add();
                    dgrdvAlarmInfo.Rows[i].Cells[0].Value = _alarmRecordDT.Rows[i][0];
                    dgrdvAlarmInfo.Rows[i].Cells[1].Value = BaseForm._stateAndAlarmCs._alarmMsg[Convert.ToInt32(_alarmRecordDT.Rows[i][3])];
                    dgrdvAlarmInfo.Rows[i].Cells[2].Value = _alarmRecordDT.Rows[i][2];
                }
            }
            else
            {
                MyTool.TxtFileProcess.CreateLog("报警界面-放弃读取报警记录文件！" + "----用户：" + BaseForm._operatorName,
                BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

                return;
            }
        }
    }
}
