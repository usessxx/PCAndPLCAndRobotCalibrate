using System
;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ThreeDimensionAVI
{
    /// <summary>
    /// ClickEnsureDelagate:点击确认按钮委托
    /// </summary>
    public delegate void ClickEnsureDelagate();

    /// <summary>
    /// ClickOutDelagate:点击流出按钮委托
    /// </summary>
    public delegate void ClickOutDelagate();
    /// <summary>
    /// ClickOutDelagate:点击手动取出按钮委托
    /// </summary>
    public delegate void ClickTakeOutManualDelagate();


    public partial class InfoForm : Form
    {
        public event ClickEnsureDelagate ClickEnsureEvent;//声明事件-点击确认按钮事件
        public event ClickOutDelagate ClickOutEvent;//声明事件-点击流出按钮事件
        public event ClickTakeOutManualDelagate ClickTakeOutEvent;//声明事件-点击手动取出按钮事件

        public InfoForm()
        {
            InitializeComponent();
        }

        //点击确认按钮
        private void btnOK_Click(object sender, EventArgs e)
        {
            MyTool.TxtFileProcess.CreateLog("消息提示界面-点击确定按钮！" + "----用户：" + BaseForm._operatorName,
            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

            if (ClickEnsureEvent != null)
                ClickEnsureEvent();
        }

        //点击手动取出按钮
        private void btnReleaseAndTakeOut_Click(object sender, EventArgs e)
        {
            MyTool.TxtFileProcess.CreateLog("消息提示界面-点击手动取出按钮！" + "----用户：" + BaseForm._operatorName,
            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

            if (ClickTakeOutEvent != null)
                ClickTakeOutEvent();
        }

        //点击流出按钮
        private void btnOut_Click(object sender, EventArgs e)
        {
            MyTool.TxtFileProcess.CreateLog("消息提示界面-点击产品流出按钮！" + "----用户：" + BaseForm._operatorName,
            BaseForm._deviceControlLogFolderPath, string.Format("{0:yyyyMMdd}", DateTime.Now), false, BaseForm._logFileMaxSize);

            if (ClickOutEvent != null)
                ClickOutEvent();
        }

        private void InfoForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
        } 
    }
}
