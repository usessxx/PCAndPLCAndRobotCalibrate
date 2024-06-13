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
using System.Threading;

namespace AxisAndIOForm
{
    public delegate void IOFormCloseRequestDelegate();//声明委托—窗口关闭
    public partial class MotionCardIOForm : Form
    {
        /*--------------------------------------------------------------------------------------
       //Copyright (C) 2022 深圳市利器精工科技有限公司
       //版权所有
       //
       //文件名：IO显示窗口
       //文件功能描述：用于显示运动控制卡的IO状态
       //
       //
       //创建标识：MaLi 20220312
       //
       //修改标识：MaLi 20220312 Change
       //修改描述：增加IO显示窗口
       //
       //
       //------------------------------------------------------------------------------------*/

        public event IOFormCloseRequestDelegate IOFormCloseRequestEvent;//声明事件—窗口关闭

        const int ONE_PAGE_IO_QUANTITY = 32;//一个页面显示的IO个数

        public int _maxIOQuantity;//最大IO个数，根据输入输出IO个数进行设定，哪个大此参数设为哪个
        public int _inputIOQuantity;//运动控制卡的输入点数
        public int _outputIOQuantity;//运动控制卡的输出点数
        public int _pageQuantity;//输入输出页面最大页数

        //*************************外部可设定读取变量*******************************//
        public bool[,] _ioStatus;//io状态，用于记录当前IO的状态，为2重数组，第一组0为输入，第二组1为输出
        //*************************内部私有变量*******************************//
        bool _inputFormFlag = true;//当前页面处于输入还是输出页面标志，false-处于输出页面，true-处于输入页面
        int _currentPageNumber = 1;//当前页码
        Thread _ioStatusUpdateThread = null;//io状态实时更新线程
        IOTitleAndName _ioTitleAndNameCs = null;//io标题及名称类

        public MotionCardIOForm()
        {
            InitializeComponent();


            #region
            _maxIOQuantity = AxisControlMainForm._maxIOQuantity;
            _inputIOQuantity = _maxIOQuantity;
            _outputIOQuantity = _maxIOQuantity;
            _pageQuantity = AxisControlMainForm._maxIOQuantity / ONE_PAGE_IO_QUANTITY;
            _ioStatus = new bool[2, _maxIOQuantity];//io状态，用于记录当前IO的状态，为2重数组，第一组0为输入，第二组1为输出
            _ioTitleAndNameCs = new IOTitleAndName(_maxIOQuantity);
            #endregion

            VariateInitial();
        }

        private void MotionCardIOForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            if (IOFormCloseRequestEvent != null)
                IOFormCloseRequestEvent();
        }

        #region  VariateInitial:变量初始化
        private void VariateInitial()
        {
            for (int i = 0; i < _maxIOQuantity; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    _ioStatus[j, i] = false;
                }
            }

            _pageQuantity = _inputIOQuantity / ONE_PAGE_IO_QUANTITY;
            if (_inputIOQuantity % ONE_PAGE_IO_QUANTITY != 0)
                _pageQuantity++;

            _pageQuantity = _outputIOQuantity / ONE_PAGE_IO_QUANTITY;
            if (_outputIOQuantity % ONE_PAGE_IO_QUANTITY != 0)
                _pageQuantity++;

            CtrlHideAndShowSetting();
            btnInput.BackColor = Color.LimeGreen;
            btnInput.ForeColor = Color.Black;

            btnOutput.BackColor = SystemColors.ControlDark;
            btnOutput.ForeColor = Color.White;
        }
        #endregion

        #region IOStatusUpdateThreadFunc:IO状态更新线程
        private void IOStatusUpdateThreadFunc()
        {
            string ctrlName = "";
            object tempCtrl = null;
            int formKindIndex = 0;//如果为输入页面，为0，如果为输出页面，为1
            int tempCurrentPage = 0;//临时当前页码

            while (true)
            {
                this.Invoke(new Action(() =>
                {
                    //上下页显示隐藏控制
                    if (_currentPageNumber <= 1)//如果为第一页
                        btnPrevPage.Visible = false;
                    else
                        btnPrevPage.Visible = true;

                    if (_currentPageNumber >= _pageQuantity && _inputFormFlag)
                        btnNextPage.Visible = false;
                    else if (_currentPageNumber < _pageQuantity && _inputFormFlag)
                        btnNextPage.Visible = true;

                    if (_currentPageNumber >= _pageQuantity && !_inputFormFlag)
                        btnNextPage.Visible = false;
                    else if (_currentPageNumber < _pageQuantity && !_inputFormFlag)
                        btnNextPage.Visible = true;

                    #region IO状态以及标签名的更新
                    //状态更新
                    if (_inputFormFlag)
                        formKindIndex = 0;
                    else
                        formKindIndex = 1;
                    tempCurrentPage = _currentPageNumber;


                    for (int i = 0; i < ONE_PAGE_IO_QUANTITY; i++)
                    {
                        ctrlName = "picIOStatus" + (i + 1).ToString("000");
                        tempCtrl = this.GetType().GetField(ctrlName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance
                            | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);

                        if ((i + (tempCurrentPage - 1) * ONE_PAGE_IO_QUANTITY < _inputIOQuantity && formKindIndex == 0) ||
                            (i + (tempCurrentPage - 1) * ONE_PAGE_IO_QUANTITY < _outputIOQuantity && formKindIndex == 1))
                        {
                            if (_ioStatus[formKindIndex, i + (tempCurrentPage - 1) * ONE_PAGE_IO_QUANTITY])
                                MyTool.PictureBoxControl.DrawWordsInTheCenterOfPictureBoxSuitSize((PictureBox)tempCtrl, new Font("Times New Roman", 9, FontStyle.Regular), _ioTitleAndNameCs._signalClr[formKindIndex * 2 + 1]
                                    , _ioTitleAndNameCs._ioTitleName[formKindIndex, i + (tempCurrentPage - 1) * ONE_PAGE_IO_QUANTITY], _ioTitleAndNameCs._textClr[formKindIndex * 2 + 1]);
                            else
                                MyTool.PictureBoxControl.DrawWordsInTheCenterOfPictureBoxSuitSize((PictureBox)tempCtrl, new Font("Times New Roman", 9, FontStyle.Regular), _ioTitleAndNameCs._signalClr[formKindIndex * 2]
                                        , _ioTitleAndNameCs._ioTitleName[formKindIndex, i + (tempCurrentPage - 1) * ONE_PAGE_IO_QUANTITY], _ioTitleAndNameCs._textClr[formKindIndex * 2]);

                            //极限传感器
                            if (formKindIndex == 0 && tempCurrentPage % 2 == 0 && (i % 8 == 0 || i % 8 == 1))
                            {
                                if (!_ioStatus[formKindIndex, i + (tempCurrentPage - 1) * ONE_PAGE_IO_QUANTITY])
                                    MyTool.PictureBoxControl.DrawWordsInTheCenterOfPictureBoxSuitSize((PictureBox)tempCtrl, new Font("Times New Roman", 9, FontStyle.Regular), _ioTitleAndNameCs._signalClr[formKindIndex * 2 + 1]
                                        , _ioTitleAndNameCs._ioTitleName[formKindIndex, i + (tempCurrentPage - 1) * ONE_PAGE_IO_QUANTITY], _ioTitleAndNameCs._textClr[formKindIndex * 2 + 1]);
                                else
                                    MyTool.PictureBoxControl.DrawWordsInTheCenterOfPictureBoxSuitSize((PictureBox)tempCtrl, new Font("Times New Roman", 9, FontStyle.Regular), _ioTitleAndNameCs._signalClr[formKindIndex * 2]
                                            , _ioTitleAndNameCs._ioTitleName[formKindIndex, i + (tempCurrentPage - 1) * ONE_PAGE_IO_QUANTITY], _ioTitleAndNameCs._textClr[formKindIndex * 2]);
                            }

                        }

                        ctrlName = "lblIOName" + (i + 1).ToString("000");
                        tempCtrl = this.GetType().GetField(ctrlName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance
                            | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);

                        if ((i + (tempCurrentPage - 1) * ONE_PAGE_IO_QUANTITY < _inputIOQuantity && formKindIndex == 0) ||
                            (i + (tempCurrentPage - 1) * ONE_PAGE_IO_QUANTITY < _outputIOQuantity && formKindIndex == 1))
                            ((Label)tempCtrl).Text = _ioTitleAndNameCs._ioName[formKindIndex, i + (tempCurrentPage - 1) * ONE_PAGE_IO_QUANTITY];
                    }
                    #endregion

                    if (_inputFormFlag)
                        lblCurrentPage.Text = "当前页码:" + _currentPageNumber.ToString() + @"/" + _pageQuantity.ToString();
                    else
                        lblCurrentPage.Text = "当前页码:" + _currentPageNumber.ToString() + @"/" + _pageQuantity.ToString();
                })); ;
                Thread.Sleep(50);
            }
        }
        #endregion

        #region 标签相关函数
        //点击input按钮
        private void SelectInputFormEvent(object sender, EventArgs e)
        {
            //此处使用invoke是防止按钮此时正在重绘，导致系统出错
            btnInput.Invoke(new Action(() =>
            {
                btnInput.BackColor = Color.LimeGreen;
                btnInput.ForeColor = Color.Black;
            })); ;
            btnOutput.Invoke(new Action(() =>
            {
                btnOutput.BackColor = SystemColors.ControlDark;
                btnOutput.ForeColor = Color.White;
            })); ;
            _currentPageNumber = 1;
            _inputFormFlag = true;
            CtrlHideAndShowSetting();
        }

        //点击output按钮
        private void SelectOutputFormEvent(object sender, EventArgs e)
        {
            //此处使用invoke是防止按钮此时正在重绘，导致系统出错
            btnInput.Invoke(new Action(() =>
            {
                btnInput.BackColor = SystemColors.ControlDark;
                btnInput.ForeColor = Color.White;
            })); ;
            btnOutput.Invoke(new Action(() =>
            {
                btnOutput.BackColor = Color.LimeGreen;
                btnOutput.ForeColor = Color.Black;
            })); ;
            _currentPageNumber = 1;
            _inputFormFlag = false;
            CtrlHideAndShowSetting();
        }
        #endregion

        //根据当前页码和页面类型，对控件进行隐藏和显示
        private void CtrlHideAndShowSetting()
        {
            int maxIOQuantity = _inputIOQuantity;
            string ctrlName = "";
            object tempCtrl = null;

            if (!_inputFormFlag)
                maxIOQuantity = _outputIOQuantity;

            //控件隐藏
            for (int i = 0; i < ONE_PAGE_IO_QUANTITY; i++)
            {
                if ((i + (_currentPageNumber - 1) * ONE_PAGE_IO_QUANTITY >= maxIOQuantity))
                {
                    ctrlName = "picIOStatus" + (i + 1).ToString("000");
                    tempCtrl = this.GetType().GetField(ctrlName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance
                        | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);
                    if (((PictureBox)tempCtrl).Visible)
                        ((PictureBox)tempCtrl).Visible = false;

                    ctrlName = "lblIOName" + (i + 1).ToString("000");
                    tempCtrl = this.GetType().GetField(ctrlName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance
                        | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);
                    if (((Label)tempCtrl).Visible)
                        ((Label)tempCtrl).Visible = false;
                }
                else
                {
                    ctrlName = "picIOStatus" + (i + 1).ToString("000");
                    tempCtrl = this.GetType().GetField(ctrlName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance
                        | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);
                    if (!((PictureBox)tempCtrl).Visible)
                        ((PictureBox)tempCtrl).Visible = true;

                    ctrlName = "lblIOName" + (i + 1).ToString("000");
                    tempCtrl = this.GetType().GetField(ctrlName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance
                        | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);
                    if (!((Label)tempCtrl).Visible)
                        ((Label)tempCtrl).Visible = true;
                }
            }
        }

        //下一页按钮
        private void btnNextPage_Click(object sender, EventArgs e)
        {
            if (_inputFormFlag)//如果为输入页面
            {
                if (_currentPageNumber < _pageQuantity)
                {
                    _currentPageNumber++;
                    CtrlHideAndShowSetting();
                }
            }
            else
            {
                if (_currentPageNumber < _pageQuantity)
                {
                    _currentPageNumber++;
                    CtrlHideAndShowSetting();
                }
            }
        }

        //上一页按钮
        private void btnPrevPage_Click(object sender, EventArgs e)
        {
            if (_currentPageNumber > 1)
                _currentPageNumber--;
            CtrlHideAndShowSetting();
        }

        private void MotionCardIOForm_Load(object sender, EventArgs e)
        {
            if (_ioStatusUpdateThread != null)
            {
                _ioStatusUpdateThread.Abort();
                _ioStatusUpdateThread = null;
            }
            _ioStatusUpdateThread = new Thread(IOStatusUpdateThreadFunc);
            _ioStatusUpdateThread.IsBackground = true;
            _ioStatusUpdateThread.Start();
        }


    }
}
