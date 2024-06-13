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

namespace UserManager
{
    /// <summary>
    /// CreateNewUserDelegate:创建新用户委托
    /// </summary>
    /// <param name="userName">string:新用户用户名</param>
    /// <param name="password">string:新用户密码</param>
    /// <param name="accessLevel">string:新用户用户等级</param>
    /// <param name="giveUpFlag">bool:是否放弃标志，false-不放弃添加，true-放弃添加</param>
    public delegate void CreateNewUserDelegate(string userName, string password, string accessLevel, bool giveUpFlag);

    /// <summary>
    /// EditUserDelegate:编辑用户委托
    /// </summary>
    /// <param name="userName">string:新用户用户名</param>
    /// <param name="password">string:新用户密码</param>
    /// <param name="accessLevel">string:新用户用户等级</param>
    /// <param name="userIndex">int:用户索引</param>
    /// <param name="giveUpFlag">bool:是否放弃标志，false-不放弃添加，true-放弃添加</param>
    public delegate void EditUserDelegate(string userName, string password, string accessLevel, int userIndex, bool giveUpFlag);

    public partial class CreateNewOrEditUserForm : Form
    {
        /*--------------------------------------------------------------------------------------
        //Copyright (C) 2022 深圳市利器精工科技有限公司
        //版权所有
        //
        //文件名：创建新用户页面
        //文件功能描述：用于创建新用户的界面
        //
        //
        //创建标识：MaLi 20220419
        //
        //修改标识：MaLi 20220419 Change
        //修改描述：增加创建新用户页面
        //
        //------------------------------------------------------------------------------------*/
        //
        //******************************事件************************************//
        public event CreateNewUserDelegate CreateNewUserEvent;//声明事件，创建新用户
        public event EditUserDelegate EditUserEvent;//声明事件，编辑用户
        //*************************外部可读写变量*******************************//

        //*************************公共静态变量*******************************//

        //*************************内部私有变量*******************************//
        bool _createNewUserFlag = false;//创建新用户标志,false-不创建新用户，仅编辑，true-创建新用户
        int _userIndex = 0;//用户索引，用以标识修改哪一个用户
        //
        //
        //

        //构造函数
        public CreateNewOrEditUserForm(int accessLevel, string wanneEditUserName, string wanneEditUserPassword, int currentUserIndex, bool createNewUserFlag)
        {
            InitializeComponent();

            _createNewUserFlag = createNewUserFlag;
            if (createNewUserFlag)//如果是创建新的用户
            {
                if (accessLevel == 100)
                {
                    this.Dispose();
                }
                else if (accessLevel == 101)//如果为工程师权限
                {
                    rbtnEngineerLevel.Enabled = false;
                }

                rbtnEngineerLevel.Checked = false;
                rbtnOperatorLevel.Checked = true;
            }
            else//如果是编辑用户
            {
                if (accessLevel == 100)//如果为操作员权限
                {
                    rbtnEngineerLevel.Checked = false;
                    rbtnOperatorLevel.Checked = true;

                    rbtnEngineerLevel.Enabled = false;
                    rbtnOperatorLevel.Enabled = false;
                }
                else if (accessLevel == 101)//如果为工程师权限
                {
                    rbtnEngineerLevel.Checked = true;
                    rbtnOperatorLevel.Checked = false;

                    rbtnEngineerLevel.Enabled = true;
                    rbtnOperatorLevel.Enabled = true;
                }
                else//如果为厂家权限
                {
                    rbtnEngineerLevel.Checked = false;
                    rbtnOperatorLevel.Checked = false;

                    rbtnEngineerLevel.Enabled = false;
                    rbtnOperatorLevel.Enabled = false;
                }
                txtUserName.Text = wanneEditUserName;
                txtPassword.Text = wanneEditUserPassword;
                txtPassword2.Text = wanneEditUserPassword;

                _userIndex = currentUserIndex;
            }
        }


        //选中工程师权限选项
        private void rbtnEngineerLevel_CheckedChanged(object sender, EventArgs e)
        {
            if (rbtnEngineerLevel.Checked)
                rbtnOperatorLevel.Checked = false;
        }

        //选中操作员权限
        private void rbtnOperatorLevel_CheckedChanged(object sender, EventArgs e)
        {
            if (rbtnOperatorLevel.Checked)
               rbtnEngineerLevel.Checked = false;
        }

        //点击确认新建按钮
        private void btnOK_Click(object sender, EventArgs e)
        {
            if(txtUserName.Text =="")
            {
                MessageBox.Show("请输入用户名！");
                return;
            }

            if(txtPassword.Text == "")
            {
                MessageBox.Show("密码不能为空！");
                return;
            }

            if(txtPassword.Text != txtPassword2.Text)
            {
                MessageBox.Show("两次输入的密码不相同，请确认！");
                return;
            }

            if (CreateNewUserEvent != null && _createNewUserFlag)
            {
                if (rbtnEngineerLevel.Checked)
                    CreateNewUserEvent(txtUserName.Text, txtPassword.Text, "101", false);
                else
                    CreateNewUserEvent(txtUserName.Text, txtPassword.Text, "100", false);
            }
            else if (EditUserEvent != null && !_createNewUserFlag)
            {
                if (rbtnEngineerLevel.Checked)
                    EditUserEvent(txtUserName.Text, txtPassword.Text, "101", _userIndex, false);
                else
                    EditUserEvent(txtUserName.Text, txtPassword.Text, "100", _userIndex, false);
            }

        }

        //点击取消新建按钮
        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (CreateNewUserEvent != null && _createNewUserFlag)
            {
                CreateNewUserEvent("", "", "", true);
            }
            else if (EditUserEvent != null && !_createNewUserFlag)
            {
                EditUserEvent("", "", "", 0, true);
            }
            
        }

        #region 虚拟键盘相关函数
        //textbox 鼠标双击事件
        private void TextBoxDoubleClickEvent(object sender, EventArgs e)
        {
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

        #endregion
    }
}
