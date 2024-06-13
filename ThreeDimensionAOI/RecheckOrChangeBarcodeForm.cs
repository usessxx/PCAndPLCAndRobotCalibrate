using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SoftKeyBoard;

namespace ThreeDimensionAVI
{
    public delegate void RecheckOrChangeBarcodeFormEnsureDelegate(string newBarcodeData);//声明委托-二维码修改或检测确定按钮委托
    public delegate void RecheckOrChangeBarcodeFormCancelDelegate();//声明委托-二维码修改或检测取消按钮事件

    public partial class RecheckOrChangeBarcodeForm : Form
    {

        public event RecheckOrChangeBarcodeFormEnsureDelegate RecheckOrChangeBarcodeFormEnsureEvent;//声明事件-二维码修改或检测确定按钮事件
        public event RecheckOrChangeBarcodeFormCancelDelegate RecheckOrChangeBarcodeFormCancelEvent;//声明事件-二维码修改或检测取消按钮事件

        string _sourceBarcodeData = "";
        public RecheckOrChangeBarcodeForm(string sourceBarcodeData)
        {
            _sourceBarcodeData = sourceBarcodeData;
            InitializeComponent();
        }


        #region 虚拟键盘相关函数
        //textbox 鼠标双击事件
        private void TextBoxDoubleClickEvent(object sender, EventArgs e)
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

        //键盘按下OK之后获取数据
        private void GetKeyboardInputData(object Sender, string GetDataStr)
        {
            ((TextBox)Sender).Text = GetDataStr;//将键盘按钮输入的数据传递给密码输入框
            ((TextBox)Sender).Focus();
            ((TextBox)Sender).SelectionLength = 0;
        }

        #endregion

        //确定按钮
        private void btnEnsure_Click(object sender, EventArgs e)
        {
            if (txtBarcodeData.Text == "")
            {
                MessageBox.Show("请输入正确的条形码数据！");
                return;
            }

            if (RecheckOrChangeBarcodeFormEnsureEvent != null)
            {
                RecheckOrChangeBarcodeFormEnsureEvent(txtBarcodeData.Text);
            }
        }

        //点击取消按钮
        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (RecheckOrChangeBarcodeFormCancelEvent != null)
            {
                RecheckOrChangeBarcodeFormCancelEvent();
            }
        }

        private void RecheckOrChangeBarcodeForm_Load(object sender, EventArgs e)
        {
            txtBarcodeData.Text = _sourceBarcodeData;
        }

        private void RecheckOrChangeBarcodeForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
        }
    }
}
