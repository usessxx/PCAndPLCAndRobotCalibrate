using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SoftKeyBoard
{
    //public delegate void SENDINPUTDATAINCLUDEOBJ(object Sender, string Text);//声明代理
    public partial class SoftNumberKeyboard : Form
    {
        public static int LanguageFlag;//用于判定语言,0-英文，1-中文
        public string TextBoxData;//用于存储当前textbox中的数据，用于传输

        bool PlusMinusBTHideFlag, FullStopBTHideFlag;//用于控制正负号按钮和小数点按钮是否被隐藏，false-显示不被隐藏，true-隐藏
        bool IpAddressFlag;//用于标识输入的数据为不为IP Address，true-为IP Address格式，false-不为IP Address格式
        public bool IPADDRESSFLAG
        {
            set { IpAddressFlag = value; }
        }

        private string SourceStr;//初始TextBox中显示的数据
        //初始TextBox中显示的数据,set
        public string SOURCESTR
        {
            set { SourceStr = value; }
        }

        private object Sender;//用于获取启动此窗口的object
        public object SENDER
        {
            set { Sender = value; }
            get { return Sender; }
        }

        public event SENDINPUTDATAINCLUDEOBJ SendInputDataIncludeObj;//声明事件
        public event SENDINPUTDATA SendInputData;//声明事件

        public bool PLUSMINUSBTHIDEFLAG
        {
            set { PlusMinusBTHideFlag = value; }
        }

        public bool FULLSTOPBTHIDEFLAG
        {
            set { FullStopBTHideFlag = value; }
        }

        private void SoftNumberKeyboard_Load(object sender, EventArgs e)
        {
            InputMsg_TextBox.Text = "";
            TextBoxData = "";

            if (LanguageFlag == 0)
            {
                this.Text = "Soft Number Keyboard";
                Delete_BT.Text = "Delete";
                Clear_BT.Text = "CE";
            }
            else
            {
                this.Text = "数字软键盘";
                Delete_BT.Text = "删除";
                Clear_BT.Text = "清空";
            }

            if (PlusMinusBTHideFlag)
                PlusOrMinus_BT.Hide();

            if (FullStopBTHideFlag)
                Dot_BT.Hide();

            InputMsg_TextBox.Text = SourceStr;
        }

        public SoftNumberKeyboard()
        {
            this.TopMost = true;//设置为最前显示
            InitializeComponent();
            InputMsg_TextBox.Focus();
            InputMsg_TextBox.SelectionStart = InputMsg_TextBox.TextLength;
            InputMsg_TextBox.SelectionLength = 0;
        }

        private void Delete_BT_Click(object sender, EventArgs e)
        {
            TextBoxData = InputMsg_TextBox.Text;
            if (TextBoxData.Length >= 1)//当显示框中的string长度大于1，那么进行裁剪
            {
                TextBoxData = TextBoxData.Substring(0, TextBoxData.Length - 1);
                InputMsg_TextBox.Text = TextBoxData;
            }
            InputMsg_TextBox.Focus();
            InputMsg_TextBox.SelectionStart = InputMsg_TextBox.TextLength;
            InputMsg_TextBox.SelectionLength = 0;
        }

        private void Clear_BT_Click(object sender, EventArgs e)
        {
            InputMsg_TextBox.Text = "";
            TextBoxData = "";
            InputMsg_TextBox.Focus();
            InputMsg_TextBox.SelectionStart = InputMsg_TextBox.TextLength;
            InputMsg_TextBox.SelectionLength = 0;
        }
        #region 当按下主按键，统一链接此函数
        private void MainBtClick(object sender, EventArgs e)
        {
            string text = ((Button)sender).Text;

            TextBoxData = InputMsg_TextBox.Text;
            if (InputMsg_TextBox.SelectionLength == InputMsg_TextBox.TextLength)
            {
                TextBoxData = "";
            }
            
            switch (text)
            {
                case "0":
                    {
                        TextBoxData += "0";
                        break;
                    }
                case "1":
                    {
                        TextBoxData += "1";
                        break;
                    }
                case "2":
                    {
                        TextBoxData += "2";
                        break;
                    }
                case "3":
                    {
                        TextBoxData += "3";
                        break;
                    }
                case "4":
                    {
                        TextBoxData += "4";
                        break;
                    }
                case "5":
                    {
                        TextBoxData += "5";
                        break;
                    }
                case "6":
                    {
                        TextBoxData += "6";
                        break;
                    }
                case "7":
                    {
                        TextBoxData += "7";
                        break;
                    }
                case "8":
                    {
                        TextBoxData += "8";
                        break;
                    }
                case "9":
                    {
                        TextBoxData += "9";
                        break;
                    }

            }
          
            if ((TextBoxData.IndexOf("-", 0) == 0 && TextBoxData.IndexOf("0", 0) == 1 && TextBoxData.Length>2 && TextBoxData.IndexOf(".",0) != 2))
            {
                TextBoxData = "-" + TextBoxData.Substring(2, TextBoxData.Length - 2);
            }
            else if ((TextBoxData.IndexOf("-", 0) < 0 && TextBoxData.IndexOf("0", 0) == 0 && TextBoxData.Length>1 && TextBoxData.IndexOf(".",0) != 1))
            {
                TextBoxData = TextBoxData.Substring(1, TextBoxData.Length - 1);
            }
            InputMsg_TextBox.Text = TextBoxData;
            InputMsg_TextBox.Focus();
            InputMsg_TextBox.SelectionStart = InputMsg_TextBox.TextLength;
            InputMsg_TextBox.SelectionLength = 0;
        }
        #endregion

        private void Dot_BT_Click(object sender, EventArgs e)
        {
            TextBoxData = InputMsg_TextBox.Text;
            if (TextBoxData.IndexOf(".", 0) < 0 && !IpAddressFlag)
                TextBoxData += ".";
            else if(IpAddressFlag)
                TextBoxData += ".";

            InputMsg_TextBox.Text = TextBoxData;
            InputMsg_TextBox.Focus();
            InputMsg_TextBox.SelectionStart = InputMsg_TextBox.TextLength;
            InputMsg_TextBox.SelectionLength = 0;
        }

        private void PlusOrMinus_BT_Click(object sender, EventArgs e)
        {
            TextBoxData = InputMsg_TextBox.Text;
            if (TextBoxData.IndexOf("-", 0) < 0)//如果没有负号
            {
                TextBoxData = "-" + TextBoxData;
            }
            else
            {
                TextBoxData = TextBoxData.Substring(1, TextBoxData.Length - 1);
            }
            InputMsg_TextBox.Text = TextBoxData;
            InputMsg_TextBox.Focus();
            InputMsg_TextBox.SelectionStart = InputMsg_TextBox.TextLength;
            InputMsg_TextBox.SelectionLength = 0;
        }

        private void Cancel_BT_Click(object sender, EventArgs e)
        {
            InputMsg_TextBox.Text = "";
            TextBoxData = "";
            this.Close();
        }

        private void OK_BT_Click(object sender, EventArgs e)
        {
            if (SendInputDataIncludeObj != null)
                SendInputDataIncludeObj(Sender, InputMsg_TextBox.Text);
            if (SendInputData != null)
                SendInputData(InputMsg_TextBox.Text);
            this.Close();
        }

        //当按下Enter键或Esc键之后
        private void InputMsg_TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)//如果按下Enter键
                OK_BT_Click(this, new EventArgs());
            else if (e.KeyCode == Keys.Escape)//如果按下ESC键
                Cancel_BT_Click(this, new EventArgs());
        }

        private void SoftNumberKeyboard_Activated(object sender, EventArgs e)
        {
            InputMsg_TextBox.Focus();
            InputMsg_TextBox.SelectionStart = InputMsg_TextBox.TextLength;
            InputMsg_TextBox.SelectionLength = 0;
        }

        //检测address格式输入框
        private void AddressCheck()
        {
            string[] tempStr = new string[4]{"","","",""};
            string tempTextStr = InputMsg_TextBox.Text;
            int count = 0;

            while (tempTextStr.IndexOf(".") != -1)
            {
                if (count <= 2 && count >= 0)
                {
                    tempStr[count] = tempTextStr.Substring(0, tempTextStr.IndexOf("."));
                    tempTextStr = tempTextStr.Substring(tempTextStr.IndexOf(".") + 1, tempTextStr.Length - tempTextStr.IndexOf(".") - 1);
                    count++;
                }
            }

            if (count == 3)//如果前三个都放置入textstr中
            {
                tempStr[count] = tempTextStr;
            }
            else if (count < 3)
            {
                tempStr[count] = tempTextStr;
            }

            int data=0;
            for (int i = 0; i < 4; i++)
            {
                try
                {
                    data = Convert.ToInt32(tempStr[i]);
                }
                catch
                {
                    if (tempStr[i] != "")
                        tempStr[i] = "";
                    
                }
                if (data > 255 && tempStr[i] != "")
                {
                    tempStr[i] = "255";
                }
                else if (data < 0 && tempStr[i] != "")
                {
                    tempStr[i] = "0";
                }
            }

            tempTextStr = "";
            for (int i = 0; i < count; i++)
            {
                tempTextStr += tempStr[i] + ".";
            }
            tempTextStr += tempStr[count];
            InputMsg_TextBox.Text = tempTextStr;

        }

        //ip地址输入检测
        private void InputMsg_TextBox_TextChanged(object sender, EventArgs e)
        {
            if (IpAddressFlag)
            {
                AddressCheck();
            }
        }

    }
}
