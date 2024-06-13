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
using MyTool;

namespace ROBCalibrate
{
    public delegate void SENDPRODUCTSELECTRESULT(string NewProductName,bool ChangeProductFlag,int ChangeSideIndex);//声明代理，用于获取
    public partial class ProductSelectForm : Form
    {
        public event SENDPRODUCTSELECTRESULT SendProductSelectResult;

        string SelectProductName = "";//用以记录选择了什么名称的产品
        int ChangeSideIndex = 0;//用于标识改变哪一侧的产品，0-传送线1，1-传送线2

        public ProductSelectForm(int SideIndex)
        {
            InitializeComponent();
            ChangeSideIndex = SideIndex;
            GetProductDataFileNameAndSetAvaliableProductComboBox();
        }

        private void ProductSelectForm_Load(object sender, EventArgs e)
        {
        }

        //获取指定文件夹中的文件名称并赋值到combobox中
        public void GetProductDataFileNameAndSetAvaliableProductComboBox()
        {
            string[] fileName;
            int currentProductIndex = -1;
            if (Directory.Exists(CalibrateForm._ProductParameterDirectoryPath))
            {
                fileName = FolderAndFileManageClass.ScanFolderAndGetAssignedFormatFileName(CalibrateForm._ProductParameterDirectoryPath, ".rcp");

                cboAvaliableProduct.Items.Clear();
                for (int i = 0; i < fileName.Length; i++)
                {
                    if (CalibrateForm._currentProductName == fileName[i] )
                        currentProductIndex = i;
                    cboAvaliableProduct.Items.Add(fileName[i]);
                }
            }
            cboAvaliableProduct.SelectedIndex = currentProductIndex;
        }

        //确认选择产品按钮
        private void Comfirm_Button_Click(object sender, EventArgs e)
        {
            if (SendProductSelectResult != null)
                SendProductSelectResult(SelectProductName, true, ChangeSideIndex);
        }

        //当点击了窗口关闭按钮
        private void ProductSelectForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;//取消关闭窗口事件
            if (SendProductSelectResult != null)
                SendProductSelectResult("", false, ChangeSideIndex);//取消切换产品
        }

        //取消产品选择按钮
        private void Cancel_Button_Click(object sender, EventArgs e)
        {
            if (SendProductSelectResult != null)
                SendProductSelectResult("", false, ChangeSideIndex);//取消切换产品
        }

        //当选择了产品
        private void Avaliable_Product_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectProductName = cboAvaliableProduct.SelectedItem.ToString();
        }

        private void Avaliable_Product_ComboBox_MouseClick(object sender, MouseEventArgs e)
        {
            GetProductDataFileNameAndSetAvaliableProductComboBox();
        }

    }
}
