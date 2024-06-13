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
using SoftKeyBoard;

namespace ThreeDimensionAVI
{
    public delegate void CloseRequestDelegate();//声明委托-请求关闭窗口
    public partial class CreateNewProductDataFileForm : Form
    {
        public event Action<string> SenderName;//声明事件委托变量Action,参数为string,无返回值，有返回值那种是Func
        public event CloseRequestDelegate CloseRequest;//声明委托事件-请求关闭窗口

        public CreateNewProductDataFileForm(string title,string content,string defaultName)
        {
            InitializeComponent();

            this.Text = title;
            lblInfoMsg.Text = content;
            lblInfoMsg.Font = new Font("宋体", lblInfoMsg.Font.Size, lblInfoMsg.Font.Style);

            txtNewProductName.Text = defaultName;

        }

        private void OK_BT_Click(object sender, EventArgs e)
        {
            MyTool.TxtFileProcess.CreateLog(this.Text + "-点击确定按钮，新产品名称为：" + txtNewProductName.Text + "----用户：" + BaseForm._operatorName,
            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);
            if (SenderName != null)
                SenderName(txtNewProductName.Text);
            Dispose();
        }

        private void Cancel_BT_Click(object sender, EventArgs e)
        {
            MyTool.TxtFileProcess.CreateLog(this.Text + "-点击取消按钮！" + "----用户：" + BaseForm._operatorName,
            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);
            if (CloseRequest != null)
                CloseRequest();
            Dispose();
        }

        //双击按钮，打开数字软键盘，数据无正负限制，浮点数
        private void TextBoxDoubleClickEventToOpenNumberKeyboard(object sender, EventArgs e)
        {
            MyTool.TxtFileProcess.CreateLog(this.Text + "-打开字母软键盘！" + "----用户：" + BaseForm._operatorName,
            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

            SoftMainKeyboard.LanguageFlag = 1;
            SoftMainKeyboard mainKeyboard = new SoftMainKeyboard();
            mainKeyboard.StartPosition = FormStartPosition.CenterScreen;
            mainKeyboard.SOURCESTR = ((TextBox)sender).Text;
            mainKeyboard.SENDER = sender;
            mainKeyboard.SendInputDataIncludeObj += new SENDINPUTDATAINCLUDEOBJ(GetKeyboardInputData);//添加事件
            mainKeyboard.ShowDialog();
        }

        //键盘按下OK之后获取数据
        private void GetKeyboardInputData(object Sender, string GetDataStr)
        {
            ((TextBox)Sender).Text = GetDataStr;//将键盘按钮输入的数据传递给密码输入框
            ((TextBox)Sender).Focus();
            ((TextBox)Sender).SelectionLength = 0;
        }
    }
}
