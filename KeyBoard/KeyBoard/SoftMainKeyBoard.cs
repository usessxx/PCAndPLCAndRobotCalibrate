using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace SoftKeyBoard
{
    public delegate void SENDINPUTDATA(string Text);//声明代理
    public delegate void SENDINPUTDATAINCLUDEOBJ(object Sender,string Text);//声明代理
    public partial class SoftMainKeyboard : Form
    {
        public static int LanguageFlag;//用于判定语言,0-英文，1-中文
        private bool ShiftBTPressedFlag;//为true时表明shift按钮被按下
        private int CursorIndexInTextBox;//用于记录光标在Textbox中的位置，相对于text右侧的距离
        private int OldTextBoxTextLength;//用于保存变换之前的Text box的text的长度
        private int OldSelectionLength;//用于保存记录变换之前的Selection Length
        private string SourceStr;//初始TextBox中显示的数据

        private object Sender;//用于获取启动此窗口的object

        public TextBox KEYBOARDTEXTTEXTBOX
        {
            get { return Keyborad_Text_TextBox; }
        }

        //初始TextBox中显示的数据,set
        public string SOURCESTR
        {
            set { SourceStr = value; }
        }

        public object SENDER
        {
            set { Sender = value; }
            get { return Sender; }
        }

        public event SENDINPUTDATA SendInputData;//声明事件
        public event SENDINPUTDATAINCLUDEOBJ SendInputDataIncludeObj;//声明事件

        
        //构造函数
        public SoftMainKeyboard()
        {
            this.TopMost = true;//设置为最前显示
            InitializeComponent();
            Font tempFont;
            if (LanguageFlag == 0)
            {
                tempFont = new Font("Times New Roman", Backsqace_BT.Font.Size, Backsqace_BT.Font.Style);
                Backsqace_BT.Text = "Backsqace";
                Backsqace_BT.Font = tempFont;

                tempFont = new Font("Times New Roman", Shift_BT.Font.Size, Shift_BT.Font.Style);
                Shift_BT.Text = "SHIFT";
                Shift_BT.Font = tempFont;
            }
            else
            {
                tempFont = new Font("宋体", Backsqace_BT.Font.Size, Backsqace_BT.Font.Style);
                Backsqace_BT.Text = "删除";
                Backsqace_BT.Font = tempFont;

                tempFont = new Font("宋体", Shift_BT.Font.Size, Shift_BT.Font.Style);
                Shift_BT.Text = "转换";
                Shift_BT.Font = tempFont;
            }

            ShiftBTPressedFlag = false;
        }

        //窗口加载
        private void SoftMainKeyboard_Load(object sender, EventArgs e)
        {
            //初始化textbox为焦点，选中所有的TEXT，并将初始的slectionstart放置进变量
            Keyborad_Text_TextBox.Text = SourceStr;
        
            CursorIndexInTextBox = Keyborad_Text_TextBox.Text.Length - Keyborad_Text_TextBox.SelectionStart;//cursor index记录的是从右到左的光标距离
            OldTextBoxTextLength = Keyborad_Text_TextBox.Text.Length;
            OldSelectionLength = Keyborad_Text_TextBox.SelectionLength;
        }

        //当点击了shift按钮，那么对应的按键名称进行变换，按钮颜色也进行切换
        private void Shift_BT_Click(object sender, EventArgs e)
        {
            if (!ShiftBTPressedFlag)
            {
                Shift_BT.BackColor = SystemColors.GrayText;
                ShiftBTPressedFlag = true;
                A_BT.Text = "A";
                B_BT.Text = "B";
                C_BT.Text = "C";
                D_BT.Text = "D";
                E_BT.Text = "E";
                F_BT.Text = "F";
                G_BT.Text = "G";
                H_BT.Text = "H";
                I_BT.Text = "I";
                J_BT.Text = "J";
                K_BT.Text = "K";
                L_BT.Text = "L";
                M_BT.Text = "M";
                N_BT.Text = "N";
                O_BT.Text = "O";
                P_BT.Text = "P";
                Q_BT.Text = "Q";
                R_BT.Text = "R";
                S_BT.Text = "S";
                T_BT.Text = "T";
                U_BT.Text = "U";
                V_BT.Text = "V";
                W_BT.Text = "W";
                X_BT.Text = "X";
                Y_BT.Text = "Y";
                Z_BT.Text = "Z";

                Symbol1_BT.Text = "-";
                Symbol2_BT.Text = "￥";
                Symbol3_BT.Text = "/";
                Symbol4_BT.Text = "!";
                Symbol5_BT.Text = "'";
                Colon_BT.Text = ";";
                Full_Stop_BT.Text = "%";
                Comma_BT.Text = "$";

                if (LanguageFlag == 0)
                    Backsqace_BT.Text = "Delete";
                else
                    Backsqace_BT.Text = "回删";
            }
            else
            {
                Shift_BT.BackColor = SystemColors.Control;
                ShiftBTPressedFlag = false;
                A_BT.Text = "a";
                B_BT.Text = "b";
                C_BT.Text = "c";
                D_BT.Text = "d";
                E_BT.Text = "e";
                F_BT.Text = "f";
                G_BT.Text = "g";
                H_BT.Text = "h";
                I_BT.Text = "i";
                J_BT.Text = "j";
                K_BT.Text = "k";
                L_BT.Text = "l";
                M_BT.Text = "m";
                N_BT.Text = "n";
                O_BT.Text = "o";
                P_BT.Text = "p";
                Q_BT.Text = "q";
                R_BT.Text = "r";
                S_BT.Text = "s";
                T_BT.Text = "t";
                U_BT.Text = "u";
                V_BT.Text = "v";
                W_BT.Text = "w";
                X_BT.Text = "x";
                Y_BT.Text = "y";
                Z_BT.Text = "z";

                Symbol1_BT.Text = "_";
                Symbol2_BT.Text = "@";
                Symbol3_BT.Text = "\\";
                Symbol4_BT.Text = "?";
                Symbol5_BT.Text = "*";
                Colon_BT.Text = ":";
                Full_Stop_BT.Text = ".";
                Comma_BT.Text = ",";

                if (LanguageFlag == 0)
                    Backsqace_BT.Text = "Backsqace";
                else
                    Backsqace_BT.Text = "删除";
            }
            //将焦点重置到textbox上，并设定光标位置
            Keyborad_Text_TextBox.Focus();
            Keyborad_Text_TextBox.SelectionLength = 0;
            Keyborad_Text_TextBox.SelectionStart = Keyborad_Text_TextBox.TextLength - CursorIndexInTextBox;
        }

        //按下输入按钮之后对应的事件
        private void InputBTPressFunc(object sender, EventArgs e)
        {
            string tempStr;
            
            if (Keyborad_Text_TextBox.SelectedText.Length > 0)
            {
                tempStr = Keyborad_Text_TextBox.Text;
                int selectStart = Keyborad_Text_TextBox.SelectionStart;
                int textLength = Keyborad_Text_TextBox.Text.Length;
                int selectLength =  Keyborad_Text_TextBox.SelectionLength;
                tempStr = tempStr.Substring(0, selectStart) + ((Button)sender).Text +
                    tempStr.Substring(selectStart + selectLength, textLength - selectStart - selectLength);
            }
            else
            {
                tempStr = Keyborad_Text_TextBox.Text;
                tempStr = tempStr.Insert(Keyborad_Text_TextBox.SelectionStart, ((Button)sender).Text);
            }

            Keyborad_Text_TextBox.Text = tempStr;

        }

        //当TEXTBOX控件内容变换之后，为了确保光标依旧保持在textbox控件中，有此程序
        private void Keyborad_Text_TextBox_TextChanged(object sender, EventArgs e)
        {
            Keyborad_Text_TextBox.Focus();

            if (OldTextBoxTextLength - Keyborad_Text_TextBox.TextLength == 1 && CursorIndexInTextBox - 1 >= 0 && OldSelectionLength == 0 && ShiftBTPressedFlag)
                CursorIndexInTextBox = CursorIndexInTextBox - 1;
            if (Keyborad_Text_TextBox.Text.Length - CursorIndexInTextBox >= 0)
                Keyborad_Text_TextBox.SelectionStart = Keyborad_Text_TextBox.Text.Length - CursorIndexInTextBox;
            else
                Keyborad_Text_TextBox.SelectionStart = 0;

            Keyborad_Text_TextBox.SelectionLength = 0;
            OldTextBoxTextLength = Keyborad_Text_TextBox.Text.Length;
            OldSelectionLength = Keyborad_Text_TextBox.SelectionLength;
        }

        //当点击了textbox控件之后，设定新的光标位置，记录光标距离TEXT末尾的距离，便于后面数据变化之后依旧保持光标位置
        private void Keyborad_Text_TextBox_Click(object sender, EventArgs e)
        {
            CursorIndexInTextBox = Keyborad_Text_TextBox.Text.Length - Keyborad_Text_TextBox.SelectionStart - Keyborad_Text_TextBox.SelectionLength;//计算光标的位置是从text的从左往右移动，间隔多少实现的
            OldTextBoxTextLength = Keyborad_Text_TextBox.Text.Length;
            OldSelectionLength = Keyborad_Text_TextBox.SelectionLength;
        }

       //当鼠标离开textbox控件之后，设定新的光标位置，记录光标距离TEXT末尾的距离，便于后面数据变化之后依旧保持光标位置
        private void Keyborad_Text_TextBox_MouseLeave(object sender, EventArgs e)
        {
            CursorIndexInTextBox = Keyborad_Text_TextBox.Text.Length - Keyborad_Text_TextBox.SelectionStart - Keyborad_Text_TextBox.SelectionLength;//计算光标的位置是从text的从左往右移动，间隔多少实现的
            OldTextBoxTextLength = Keyborad_Text_TextBox.Text.Length;
            OldSelectionLength = Keyborad_Text_TextBox.SelectionLength;
        }

        //点击删除/回删按钮
        private void Backsqace_BT_Click(object sender, EventArgs e)
        {
            string tempStr;

            tempStr = Keyborad_Text_TextBox.Text;
            if (Keyborad_Text_TextBox.SelectionLength == 0)//如果不处于多个选中状态
            {
                if (CursorIndexInTextBox - 1 >= 0 && ShiftBTPressedFlag)//如果为回删按钮，并且不处于最末尾
                {
                    tempStr = tempStr.Substring(0, Keyborad_Text_TextBox.SelectionStart) +
                        tempStr.Substring(Keyborad_Text_TextBox.SelectionStart + 1, Keyborad_Text_TextBox.TextLength - Keyborad_Text_TextBox.SelectionStart - 1);
                    CursorIndexInTextBox = CursorIndexInTextBox - 1;
                    OldTextBoxTextLength = tempStr.Length;
                    Keyborad_Text_TextBox.Text = tempStr;
                }
                else if (Keyborad_Text_TextBox.SelectionStart >= 1 && !ShiftBTPressedFlag)//如果为删除按钮，并且不处于最开头
                {
                    tempStr = tempStr.Substring(0, Keyborad_Text_TextBox.SelectionStart - 1) +
                        tempStr.Substring(Keyborad_Text_TextBox.SelectionStart, Keyborad_Text_TextBox.TextLength - Keyborad_Text_TextBox.SelectionStart);
                    OldTextBoxTextLength = tempStr.Length;
                    Keyborad_Text_TextBox.Text = tempStr;
                }
                else//重置焦点
                {
                    Keyborad_Text_TextBox.Focus();
                    Keyborad_Text_TextBox.SelectionStart = Keyborad_Text_TextBox.TextLength - CursorIndexInTextBox;
                    Keyborad_Text_TextBox.SelectionLength = 0;
                }

            }
            else//如果处于多个选中状态
            {
                tempStr = Keyborad_Text_TextBox.Text;
                tempStr = tempStr.Substring(0, Keyborad_Text_TextBox.SelectionStart) +
                    tempStr.Substring(Keyborad_Text_TextBox.SelectionStart + Keyborad_Text_TextBox.SelectionLength,
                    Keyborad_Text_TextBox.TextLength - Keyborad_Text_TextBox.SelectionStart - Keyborad_Text_TextBox.SelectionLength);
                CursorIndexInTextBox = Keyborad_Text_TextBox.TextLength - Keyborad_Text_TextBox.SelectionStart - Keyborad_Text_TextBox.SelectionLength;
            }
                
            

        }

        //点击确定按钮
        private void OK_BT_Click(object sender, EventArgs e)
        {
            if (SendInputData != null)//判定事件是否为空
            {
                SendInputData(Keyborad_Text_TextBox.Text);//触发事件
            }
            if(SendInputDataIncludeObj != null)
            {
                SendInputDataIncludeObj(Sender, Keyborad_Text_TextBox.Text);//触发事件
            }
            this.Close();
        }

        //点击取消按钮
        private void Cancel_BT_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //启动时，让窗口的焦点在TEXTBOX上
        private void SoftMainKeyboard_Activated(object sender, EventArgs e)
        {
            Keyborad_Text_TextBox.Focus();
            Keyborad_Text_TextBox.SelectAll();
            Keyborad_Text_TextBox.SelectionStart = Keyborad_Text_TextBox.TextLength;
        }

        //按下键盘的Enter键代表确定按钮，按下键盘的esc键代表cancel
        private void SoftMainKeyboard_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                OK_BT_Click(this, new EventArgs());
            }
            else if (e.KeyCode == Keys.Escape)
            {
                Cancel_BT_Click(this, new EventArgs());
            }
        }

        //按下键盘的Enter键代表确定按钮，按下键盘的esc键代表cancel
        private void Keyborad_Text_TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                OK_BT_Click(this, new EventArgs());
            }
            else if (e.KeyCode == Keys.Escape)
            {
                Cancel_BT_Click(this, new EventArgs());
            }
        }

        //clear按钮被点击
        private void Clear_BT_Click(object sender, EventArgs e)
        {
            Keyborad_Text_TextBox.Text = "";

            Keyborad_Text_TextBox.Focus();
            Keyborad_Text_TextBox.SelectionStart = 0;
        }
    }
}
