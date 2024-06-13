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
using UCArrow;

namespace ThreeDAVIInspectHistoryBrowse
{
    public partial class ImageDispForm : Form
    {
        /*--------------------------------------------------------------------------------------
      //Copyright (C) 2022 深圳市利器精工科技有限公司
      //版权所有
      //
      //文件名：检测历史记录图片浏览页面
      //文件功能描述：检测历史记录图片浏览页面
      //
      //
      //创建标识：MaLi 20220518
      //
      //修改标识：MaLi 20220518 Change
      //修改描述：增加检测历史记录图片浏览页面
      //
      //------------------------------------------------------------------------------------*/
        //
        /*******************************事件**************************************/

        /*************************外部可设定读取参数*******************************/

        //*************************公共静态变量*******************************//

        //*************************内部私有变量*******************************//
        List<string> _imageFilePath = new List<string>();//找到的图片路径
        int _currentDispImageIndex = 0;//当前显示的图片的索引号
        bool _imageMoveFlag = false;//图像移动标志，当mousedown的时候此flag置为true，up的时候置为false
        Point _mouseSourcePosition = new Point(0, 0);//鼠标初始坐标位置，当mousedown的时候置值
        string _barcode = "";//获取产品Barcode，用于导出图像命名

        Point _picCtrlSourceLocation = new Point(0, 0);//picturebox控件最初的位置，主要用于切换图片时的初始化
        Size _picCtrlSourceSize = new Size(0, 0);//picturebox控件最初的尺寸，主要用于切换图片时的初始化

        string[] _imageFileFolderPath = new string[3] { "", "", "" };//0-sourceimage文件夹路径，1-inferredOK文件夹路径，2-inferredNG文件夹路径
        //
        //


        /// <summary>
        /// ImageDispForm:构造函数
        /// </summary>
        /// <param name="baseFolderPath">string:基础文件夹路径，主要用于基于此路径获取SourceImageFolder，inferredImagFolder</param>
        /// <param name="panelFolderName">string:产品板的文件夹名，和_baseFolderPath配合使用</param>
        /// <param name="PCSIndex">int:图片的PCS索引号</param>
        /// <param name="elementKeyWord">string:图片的元件索引关键名</param>
        /// <param name="usingElementKeyWordFlag">bool:是否使用元件索引关健名，如果为false，意味着要找到所有这个pcs号的图片（包含Infer图和Source图），如果为true那么只需找到对应的pcs对应的索引名的图片</param>
        public ImageDispForm(string baseFolderPath,string panelFolderName,int PCSIndex,string elementKeyWord,bool usingElementKeyWordFlag)
        {
            InitializeComponent();

            //关联鼠标滚轮滚动事件
            picDispInspectImage.MouseWheel += new MouseEventHandler(ImageScaleChange);

            //解析出SourceImage和InferredImage文件夹
           
            _imageFileFolderPath[0] = baseFolderPath + "\\" + "SourceImage\\" + panelFolderName + "\\";
            _imageFileFolderPath[1] = baseFolderPath + "\\" + "InferredOK\\" + panelFolderName + "\\";
            _imageFileFolderPath[2] = baseFolderPath + "\\" + "InferredNG\\" + panelFolderName + "\\";

            try
            {
                _barcode = panelFolderName.Substring(0, panelFolderName.IndexOf("_"));//获取产品条形码
            }
            catch { MessageBox.Show("打开图像失败！"); }
            //获取图片路径
            for (int i = 1; i < 3; i++)
            {
                if (Directory.Exists(_imageFileFolderPath[i]))
                {
                    string[] strDataFiles = Directory.GetFiles(_imageFileFolderPath[i]);

                    for (int j = 0; j < strDataFiles.Length; j++)
                    {
                        string tempStr = strDataFiles[j].Substring(strDataFiles[j].LastIndexOf("\\") + 1);//获取图片名
                        tempStr = tempStr.Replace("__", "_");//以防有多余"_"
                        int findPCSIndex = Convert.ToInt32(tempStr.Substring(tempStr.IndexOf("_") + 1, tempStr.IndexOf("-") - tempStr.IndexOf("_") - 1));//获取pcs号

                        //获取元件的索引名
                        tempStr = tempStr.Substring(tempStr.IndexOf("-") + 1);
                        string findElementKeyWord = tempStr.Substring(tempStr.IndexOf("-") + 1, tempStr.IndexOf("_") - tempStr.IndexOf("-") - 1);

                        if (findPCSIndex == PCSIndex &&
                            ((elementKeyWord == findElementKeyWord && usingElementKeyWordFlag) || !usingElementKeyWordFlag))
                        {
                            if (File.Exists(strDataFiles[j]))
                            {
                                _imageFilePath.Add(strDataFiles[j]);
                            }
                        }
                    }
                }
                else
                {
                    continue;
                }
            }
        }

        //窗口创建事件
        private void ImageDispForm_Load(object sender, EventArgs e)
        {
            if (_imageFilePath.Count <= 0)
            {
                MessageBox.Show("无法找到图片！");
                this.Dispose();
            }

            try
            {
                _currentDispImageIndex = 0;
                Bitmap btp = new Bitmap(_imageFilePath[_currentDispImageIndex]);
                picDispInspectImage.Image = btp;
                lblImageName.Text = _imageFilePath[_currentDispImageIndex].Substring(_imageFilePath[_currentDispImageIndex].LastIndexOf("\\") + 1);
            }
            catch
            {
                MessageBox.Show("打开图片失败！");
                this.Dispose();
            }

            //初始化picturebox控件的尺寸和坐标
            _picCtrlSourceLocation = picDispInspectImage.Location;
            _picCtrlSourceSize = picDispInspectImage.Size;
        }

        #region 图像缩放，平移

        //图像尺寸缩放事件
        private void ImageScaleChange(object sender,MouseEventArgs e)
        {
            if (e.Delta > 0)
            {
                picDispInspectImage.Width = picDispInspectImage.Width * 11 / 10;
                picDispInspectImage.Height = picDispInspectImage.Height * 11 / 10;

                //控件居中
                Point picCtrlNewLocation = new Point();
                picCtrlNewLocation.X = (pnlImageDispRegion.Width - picDispInspectImage.Width) / 2;
                picCtrlNewLocation.Y = (pnlImageDispRegion.Height - picDispInspectImage.Height) / 2;
                picDispInspectImage.Location = picCtrlNewLocation;
            }
            else
            {
                if (picDispInspectImage.Width > pnlImageDispRegion.Width * 0.2 &&
                   picDispInspectImage.Height > pnlImageDispRegion.Height * 0.2)//当picturebox控件尺寸大于panel尺寸的1/5时可以缩小，否则不能再缩小了
                {
                    picDispInspectImage.Width = picDispInspectImage.Width * 9 / 10;
                    picDispInspectImage.Height = picDispInspectImage.Height * 9 / 10;

                    //控件居中
                    Point picCtrlNewLocation = new Point();
                    picCtrlNewLocation.X = (pnlImageDispRegion.Width - picDispInspectImage.Width) / 2;
                    picCtrlNewLocation.Y = (pnlImageDispRegion.Height - picDispInspectImage.Height) / 2;
                    picDispInspectImage.Location = picCtrlNewLocation;
                }
            }
        }

        //picture box控件鼠标Down事件
        private void picDispInspectImage_MouseDown(object sender, MouseEventArgs e)
        {
            _imageMoveFlag = true;
            _mouseSourcePosition = new Point(e.X, e.Y);
        }

        //picture box控件鼠标UP事件
        private void picDispInspectImage_MouseUp(object sender, MouseEventArgs e)
        {
            _imageMoveFlag = false;
        }

        //picture box控件鼠标Leave事件
        private void picDispInspectImage_MouseLeave(object sender, EventArgs e)
        {
            _imageMoveFlag = false;
        }

        //picture box控件鼠标Move事件
        private void picDispInspectImage_MouseMove(object sender, MouseEventArgs e)
        {
            if (_imageMoveFlag)//如果图像移动
            {
                picDispInspectImage.Left += Convert.ToInt16(e.X - _mouseSourcePosition.X);//设置X坐标
                picDispInspectImage.Top += Convert.ToInt16(e.Y - _mouseSourcePosition.Y);//设置Y坐标
            }
        }

        //picture box控件鼠标Click事件
        private void picDispInspectImage_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)//如果点击的是鼠标右键
            {
                cmsImageExport.Show((PictureBox)sender, new Point(e.X, e.Y));
            }
        }

        //导出当前图片
        private void 导出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = "导出图片";
            sfd.Filter = "jpg(*.jpg)|*.jpg";
            try
            {
                sfd.FileName = _barcode + "_" + lblImageName.Text;
            }
            catch
            {
                MessageBox.Show("无法找到原图！");
            }
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                FileInfo finfo = new FileInfo(_imageFilePath[_currentDispImageIndex]);
                try
                {
                    finfo.CopyTo(sfd.FileName, true);
                    MessageBox.Show("导出图片成功！");
                }
                catch
                {
                    MessageBox.Show("导出图片失败！");
                }
            }
        }

        //导出当前图片对应的原图
        private void 导出当前图片对应的原图ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = "导出图片";
            sfd.Filter = "jpg(*.jpg)|*.jpg";
            try
            {
                sfd.FileName = _barcode + "_" + lblImageName.Text.Substring(0, lblImageName.Text.LastIndexOf("_"));
            }
            catch
            {
                MessageBox.Show("无法找到原图！");
            }
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                string imageName = lblImageName.Text;
                imageName = imageName.Substring(imageName.LastIndexOf("\\") + 1);
                imageName = imageName.Substring(0, imageName.LastIndexOf("_"));
                imageName = imageName + ".jpg";

                if (File.Exists(_imageFileFolderPath[0] + imageName))
                {
                    FileInfo finfo = new FileInfo(_imageFileFolderPath[0] + imageName);
                    try
                    {
                        finfo.CopyTo(sfd.FileName, true);
                        MessageBox.Show("导出图片成功！");
                    }
                    catch
                    {
                        MessageBox.Show("导出图片失败！");
                    }
                }
                else
                {
                    MessageBox.Show("原图不存在，无法导出！");
                }
            }
        }

        //显示当前图片对应的原图
        private void 显示当前图片的原图ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string imageName = _imageFilePath[_currentDispImageIndex];
            imageName = imageName.Substring(imageName.LastIndexOf("\\") + 1);
            imageName = imageName.Substring(0, imageName.LastIndexOf("_"));
            imageName = imageName + ".jpg";

            if (File.Exists(_imageFileFolderPath[0] + imageName))
            {
                Bitmap btp = new Bitmap(_imageFileFolderPath[0] + imageName);
                picDispInspectImage.Image = btp;
            }
            else
            {
                MessageBox.Show("原图不存在，无法打开！");
            }
        }

        #endregion

        #region 切换图片

        //下一张图片
        private void btnNextImageClick(object sender, EventArgs e)
        {
            picDispInspectImage.Size = _picCtrlSourceSize;
            picDispInspectImage.Location = _picCtrlSourceLocation;

            if (_imageFilePath.Count > _currentDispImageIndex + 1)
            {
                try
                {
                    _currentDispImageIndex++;
                    Bitmap btp = new Bitmap(_imageFilePath[_currentDispImageIndex]);
                    picDispInspectImage.Image = btp;
                    lblImageName.Text = _imageFilePath[_currentDispImageIndex].Substring(_imageFilePath[_currentDispImageIndex].LastIndexOf("\\") + 1);
                }
                catch
                {
                    MessageBox.Show("打开图片失败！");
                }
            }
            else
            {
                try
                {
                    _currentDispImageIndex = 0;
                    Bitmap btp = new Bitmap(_imageFilePath[_currentDispImageIndex]);
                    picDispInspectImage.Image = btp;
                    lblImageName.Text = _imageFilePath[_currentDispImageIndex].Substring(_imageFilePath[_currentDispImageIndex].LastIndexOf("\\") + 1);

                }
                catch
                {
                    MessageBox.Show("打开图片失败！");
                }
            }
        }

        //上一张图片
        private void btnPreviousImageClick(object sender, EventArgs e)
        {
            picDispInspectImage.Size = _picCtrlSourceSize;
            picDispInspectImage.Location = _picCtrlSourceLocation;

            if (_currentDispImageIndex >= 1)
            {
                try
                {
                    _currentDispImageIndex--;
                    Bitmap btp = new Bitmap(_imageFilePath[_currentDispImageIndex]);
                    picDispInspectImage.Image = btp;
                    lblImageName.Text = _imageFilePath[_currentDispImageIndex].Substring(_imageFilePath[_currentDispImageIndex].LastIndexOf("\\") + 1);
                }
                catch
                {
                    MessageBox.Show("打开图片失败！");
                }
            }
            else
            {
                try
                {
                    _currentDispImageIndex = _imageFilePath.Count - 1;
                    Bitmap btp = new Bitmap(_imageFilePath[_currentDispImageIndex]);
                    picDispInspectImage.Image = btp;
                    lblImageName.Text = _imageFilePath[_currentDispImageIndex].Substring(_imageFilePath[_currentDispImageIndex].LastIndexOf("\\") + 1);
                }
                catch
                {
                    MessageBox.Show("打开图片失败！");
                }
            }
        }

        #endregion

        


    }
}
