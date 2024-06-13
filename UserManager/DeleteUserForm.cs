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

namespace UserManager
{
    public delegate void DeleteUserDelegate(string deleteUserName,bool giveUpFlag);//声明委托-删除用户
    public partial class DeleteUserForm : Form
    {
        /*--------------------------------------------------------------------------------------
        //Copyright (C) 2022 深圳市利器精工科技有限公司
        //版权所有
        //
        //文件名：用户删除页面
        //文件功能描述：用于提供用户的删除功能
        //
        //
        //创建标识：MaLi 20220419
        //
        //修改标识：MaLi 20220419 Change
        //修改描述：增加用户删除页面
        //
        //------------------------------------------------------------------------------------*/
        //
        //******************************事件************************************//
        public event DeleteUserDelegate DeleteUserEvent;//声明事件-删除用户
        //*************************外部可读写变量*******************************//

        //*************************公共静态变量*******************************//

        //*************************内部私有变量*******************************//
        DataTable _userAndPassWordDT = new DataTable();//用户及密码DATATABLE
        int _accessLevel = 0;//主窗口当前权限等级

        ListViewGroup _engineerGroup = null; //创建工程师分组
        ListViewGroup _operatorGroup = null; //创建操作员分组

        CreateNewOrEditUserForm _createNewOrEditUser = null;//新建用户窗口
        //
        //
        //

        /// <summary>
        /// DeleteUserForm:构造函数
        /// </summary>
        /// <param name="accessLevel">int:权限等级，用于判定哪些用户是可以被删除的</param>
        /// <param name="userAndPasswordDt">DataTable:用户及密码等Datatable</param>
        public DeleteUserForm(int accessLevel,DataTable userAndPasswordDt)
        {
            InitializeComponent();

            _userAndPassWordDT = userAndPasswordDt;
            _accessLevel = accessLevel;

            //初始化ListView的控件，添加imagelist
            ImageList imageList = new ImageList();
            imageList.ImageSize = new Size(40, 40);//在list view中显示的图标大小由这里的imageSize来决定
            imageList.Images.Add(Bitmap.FromFile(Directory.GetCurrentDirectory() + "\\" + "操作员.png"));
            imageList.Images.Add(Bitmap.FromFile(Directory.GetCurrentDirectory() + "\\" + "管理员.png"));
            this.lvCouldBeDeleteUserDisp.LargeImageList = imageList;

            _engineerGroup = new ListViewGroup(); //工程师分组
            _engineerGroup.HeaderAlignment = HorizontalAlignment.Left;
            _engineerGroup.Header = "工程师权限级别用户";
            _operatorGroup = new ListViewGroup(); //操作员分组
            _operatorGroup.HeaderAlignment = HorizontalAlignment.Left;
            _operatorGroup.Header = "操作员权限级别用户";

            lvCouldBeDeleteUserDisp.Groups.Add(_engineerGroup);
            lvCouldBeDeleteUserDisp.Groups.Add(_operatorGroup);
            lvCouldBeDeleteUserDisp.ShowGroups = true;

            lvCouldBeDeleteUserDisp.Clear();
            for (int i = 0; i < _userAndPassWordDT.Rows.Count; i++)
            {
                if (Convert.ToInt32(_userAndPassWordDT.Rows[i][3]) < accessLevel)
                {
                    ListViewItem lvi = new ListViewItem();
                    lvi.ImageIndex = Convert.ToInt32(_userAndPassWordDT.Rows[i][3]) % 100;
                    lvi.Text = _userAndPassWordDT.Rows[i][1].ToString();
                    lvi.Tag = _userAndPassWordDT.Rows[i][0];
                    if (lvi.ImageIndex == 0)
                        _operatorGroup.Items.Add(lvi);
                    else if (lvi.ImageIndex == 1)
                        _engineerGroup.Items.Add(lvi);
                    else
                        continue;
                    lvCouldBeDeleteUserDisp.Items.Add(lvi);
                }
                else
                {
                    continue;
                }
            }
        }

        //删除按钮
        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if ((int)lvCouldBeDeleteUserDisp.SelectedItems[0].Tag > 0 && (int)lvCouldBeDeleteUserDisp.SelectedItems[0].Tag <= _userAndPassWordDT.Rows.Count)
                {
                    if (MessageBox.Show("确认是否删除用户\"" + _userAndPassWordDT.Rows[(int)lvCouldBeDeleteUserDisp.SelectedItems[0].Tag - 1][1].ToString() + "\"?", "删除用户",
                        MessageBoxButtons.OKCancel) == DialogResult.OK)
                    {
                        if (DeleteUserEvent != null)
                            DeleteUserEvent(_userAndPassWordDT.Rows[(int)lvCouldBeDeleteUserDisp.SelectedItems[0].Tag - 1][1].ToString(), false);
                    }
                    else
                    {
                        return;
                    }
                }
            }
            catch
            { }
        }

        //取消按钮
        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (DeleteUserEvent != null)
                DeleteUserEvent("", true);
        }
    }
}
