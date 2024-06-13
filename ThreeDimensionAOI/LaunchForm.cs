using System;
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
    public partial class LaunchForm : Form
    {
        const int MSG_QUANTITY=1;
        string _versionMsg = "3D AVI V1.00";
        string[] _infoMsg; 


        public LaunchForm()
        {
            InitializeComponent();
            //指定需要显示的提示信息的内容
            _infoMsg = new string[MSG_QUANTITY];
            _infoMsg[0] = "启动中....";

            DispInfoOnPicBox(0);
        }

        private void LaunchForm_Load(object sender, EventArgs e)
        {
        }

        #region 显示版本信息和指定index的提示信息至picturebox
        public void DispInfoOnPicBox(int index)
        {
            //创建一个bitmap，用于将绘制出来的图形等传递给picturebox
            Bitmap tempBmp = new Bitmap(picLaunchMsg.Width, picLaunchMsg.Height);
            //创建一个Graphics，用于绘制文字图像等
            Graphics tempG = Graphics.FromImage(tempBmp);
            try
            {
                tempG.Clear(Color.White);//清除界面
                Font versionMsgFont = new Font("宋体", 30, FontStyle.Bold);//设置版本信息字体
                Font infoMsgFont = new Font("宋体", 15);//设置进度提示信息字体
                Brush backClr = new SolidBrush(SystemColors.Control);//设置picturebox控件背景颜色
                Brush versionMsgBrush = new SolidBrush(Color.Green);//创建版本信息画刷
                Brush infoMsgBrush = new SolidBrush(Color.Black);//创建提示信息画刷

                tempG.FillRectangle(backClr, 0, 0, picLaunchMsg.Width, picLaunchMsg.Height);//填充picturebox背景颜色至bitmap
                SizeF versionMsgSize = tempG.MeasureString(_versionMsg, versionMsgFont);//测量版本信息的尺寸
                SizeF infoMsgSize = tempG.MeasureString(_infoMsg[index], infoMsgFont);//测量提示信息的尺寸

                Point versionMsgDrawP = new Point(0, 0);//声明显示版本信息的坐标变量
                Point infoMsgDrawP = new Point(0, 0);//声明显示提示信息的坐标变量

                versionMsgDrawP.X = (int)((picLaunchMsg.Width - versionMsgSize.Width) / 2);//设置显示版本信息的坐标X位置
                versionMsgDrawP.Y = (int)((picLaunchMsg.Height * 1 / 2 - versionMsgSize.Height) / 2);//设置显示版本信息的坐标Y位置
                tempG.DrawString(_versionMsg, versionMsgFont, versionMsgBrush, versionMsgDrawP);//显示版本信息

                infoMsgDrawP.X = (int)((picLaunchMsg.Width - infoMsgSize.Width) / 2);//设置显示提示信息的坐标X位置
                infoMsgDrawP.Y = (int)((picLaunchMsg.Height * 1 / 2 - versionMsgSize.Height) / 2 + picLaunchMsg.Height * 1 / 2);//设置显示提示信息的坐标Y位置
                tempG.DrawString(_infoMsg[index], infoMsgFont, infoMsgBrush, infoMsgDrawP);//显示提示信息

                picLaunchMsg.Image = tempBmp;//将生成的bmp图贴附到picturebox上
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

    }
}
