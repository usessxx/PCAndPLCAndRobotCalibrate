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

namespace ThreeDimensionAVI
{
    public partial class TrackDisplayBasePointData : Form
    {
        /*--------------------------------------------------------------------------------------
        //Copyright (C) 2022 深圳市利器精工科技有限公司
        //版权所有
        //
        //文件名：轨迹显示画面
        //文件功能描述：根据DataTable的点位数据生成轨迹图
        //
        //
        //创建标识：MaLi 20220521
        //
        //修改标识：MaLi 20220521 Change
        //修改描述：轨迹显示画面
        //
        //------------------------------------------------------------------------------------*/
        //
        /*******************************事件**************************************/

        /*************************外部可设定读取参数*******************************/

        //*************************公共静态变量*******************************//

        //*************************内部私有变量*******************************//
        bool _imageMoveFlag = false;//图像移动标志，当mousedown的时候此flag置为true，up的时候置为false
        Point _mouseSourcePosition = new Point(0, 0);//鼠标初始坐标位置，当mousedown的时候置值

        Point _picCtrlSourceLocation = new Point(0, 0);//picturebox控件最初的位置，主要用于切换图片时的初始化
        Size _picCtrlSourceSize = new Size(0, 0);//picturebox控件最初的尺寸，主要用于切换图片时的初始化

        Bitmap _trackBmp = null;//轨迹图

        //
        //

        //构造函数
        public TrackDisplayBasePointData(DataTable sourceDataDt)
        {
            InitializeComponent();


            //关联鼠标滚轮滚动事件
            picDispTrack.MouseWheel += new MouseEventHandler(ImageScaleChange);

            BaseDataTableDrawTrack(sourceDataDt);
        }

        //Load函数
        private void TrackDisplayBasePointData_Load(object sender, EventArgs e)
        {

            //初始化picturebox控件的尺寸和坐标
            _picCtrlSourceLocation = picDispTrack.Location;
            _picCtrlSourceSize = picDispTrack.Size;

            picDispTrack.Image = _trackBmp;
        }

        //基于datatable绘制轨迹图
        private void BaseDataTableDrawTrack(DataTable sourceDataDt)
        {
            try
            {
                float smallestXCoor = Convert.ToSingle(sourceDataDt.Rows[0][1].ToString()) * -1, smallestYCoor = Convert.ToSingle(sourceDataDt.Rows[0][2].ToString());
                float biggestXCoor = Convert.ToSingle(sourceDataDt.Rows[0][1].ToString()), biggestYCoor = Convert.ToSingle(sourceDataDt.Rows[0][2].ToString());
                float xBorderWidth = 10, yBorderWidth = 10;
                float imageXSize, imageYSize;
                int pixelQuantityOnemm = 10;//1毫米用100个像素代替
                int circleRadius = 2;
                int dispNameYOffset = 3;


                for (int i = 0; i < sourceDataDt.Rows.Count; i++)
                {
                    float tempX, tempY;
                    tempX = Convert.ToSingle(sourceDataDt.Rows[i][1].ToString()) * -1;
                    tempY = Convert.ToSingle(sourceDataDt.Rows[i][2].ToString());
                    if (tempX < smallestXCoor)
                        smallestXCoor = tempX;
                    if (tempX > biggestXCoor)
                        biggestXCoor = tempX;

                    if (tempY < smallestYCoor)
                        smallestYCoor = tempY;
                    if (tempY > biggestYCoor)
                        biggestYCoor = tempY;
                }
                imageXSize = biggestXCoor - smallestXCoor;
                imageYSize = biggestYCoor - smallestYCoor;
                imageXSize = imageXSize + xBorderWidth * 2;
                imageYSize = imageYSize + yBorderWidth * 2;

                _trackBmp = new Bitmap((int)(imageXSize * pixelQuantityOnemm), (int)(imageYSize * pixelQuantityOnemm));

                //设置显示参数
                Graphics g = Graphics.FromImage(_trackBmp);
                Brush circlrBrsh = new SolidBrush(Color.White);
                Pen circlePen = new Pen(Color.White, 1);
                Pen trackLinePen = new Pen(Color.Green, 3);
                Font font = new Font("Times New Roman", (int)(1.5f * pixelQuantityOnemm));

                g.Clear(Color.Black);//将图片背景设置为黑色
                for (int i = 0; i < sourceDataDt.Rows.Count; i++)
                {
                    float tempX, tempY;
                    tempX = Convert.ToSingle(sourceDataDt.Rows[i][1].ToString()) * -1;
                    tempY = Convert.ToSingle(sourceDataDt.Rows[i][2].ToString());

                    //以圆的形式绘制检测点位
                    g.DrawEllipse(circlePen, new Rectangle((int)((tempX - smallestXCoor + xBorderWidth - circleRadius) * pixelQuantityOnemm), (int)((tempY - smallestYCoor + yBorderWidth - circleRadius) * pixelQuantityOnemm),
                        2 * circleRadius * pixelQuantityOnemm, 2 * circleRadius * pixelQuantityOnemm));
                    g.FillEllipse(circlrBrsh, new Rectangle((int)((tempX - smallestXCoor + xBorderWidth - circleRadius) * pixelQuantityOnemm), (int)((tempY - smallestYCoor + yBorderWidth - circleRadius) * pixelQuantityOnemm),
                        2 * circleRadius * pixelQuantityOnemm, 2 * circleRadius * pixelQuantityOnemm));

                    //绘制轨迹直线
                    if (i >= 1)
                    {
                        float prePointX = Convert.ToSingle(sourceDataDt.Rows[i - 1][1].ToString()) * -1;
                        float prePointY = Convert.ToSingle(sourceDataDt.Rows[i - 1][2].ToString());
                        g.DrawLine(trackLinePen, new Point((int)((prePointX - smallestXCoor + xBorderWidth) * pixelQuantityOnemm), (int)((prePointY - smallestYCoor + yBorderWidth) * pixelQuantityOnemm)),
                            new Point((int)((tempX - smallestXCoor + xBorderWidth) * pixelQuantityOnemm), (int)((tempY - smallestYCoor + yBorderWidth) * pixelQuantityOnemm)));
                    }

                    //显示点位名称
                    string dispName = (i + 1).ToString() + ":" + sourceDataDt.Rows[i][5].ToString();
                    SizeF fontSize = g.MeasureString(dispName, font);
                    g.DrawString(dispName, font, new SolidBrush(Color.Yellow), new Point((int)((tempX - smallestXCoor + xBorderWidth - circleRadius) * pixelQuantityOnemm) - (int)fontSize.Width / 2,
                        (int)((tempY - smallestYCoor + yBorderWidth - circleRadius) * pixelQuantityOnemm) + dispNameYOffset * pixelQuantityOnemm + (int)fontSize.Height / 2));

                }
            }
            catch (Exception e)
            {
                MessageBox.Show("显示点位轨迹失败！" + e.ToString());
            }
        }

        #region 图像缩放，平移
        //图像尺寸缩放事件
        private void ImageScaleChange(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0)
            {
                picDispTrack.Width = picDispTrack.Width * 11 / 10;
                picDispTrack.Height = picDispTrack.Height * 11 / 10;

                //控件居中
                Point picCtrlNewLocation = new Point();
                picCtrlNewLocation.X = (pnlTrackDispRegion.Width - picDispTrack.Width) / 2;
                picCtrlNewLocation.Y = (pnlTrackDispRegion.Height - picDispTrack.Height) / 2;
                picDispTrack.Location = picCtrlNewLocation;
            }
            else
            {
                if (picDispTrack.Width > pnlTrackDispRegion.Width * 0.2 &&
                   picDispTrack.Height > pnlTrackDispRegion.Height * 0.2)//当picturebox控件尺寸大于panel尺寸的1/5时可以缩小，否则不能再缩小了
                {
                    picDispTrack.Width = picDispTrack.Width * 9 / 10;
                    picDispTrack.Height = picDispTrack.Height * 9 / 10;

                    //控件居中
                    Point picCtrlNewLocation = new Point();
                    picCtrlNewLocation.X = (pnlTrackDispRegion.Width - picDispTrack.Width) / 2;
                    picCtrlNewLocation.Y = (pnlTrackDispRegion.Height - picDispTrack.Height) / 2;
                    picDispTrack.Location = picCtrlNewLocation;
                }
            }
        }

        //picture box控件鼠标Down事件
        private void picDispTrack_MouseDown(object sender, MouseEventArgs e)
        {
            _imageMoveFlag = true;
            _mouseSourcePosition = new Point(e.X, e.Y);
        }

        //picture box控件鼠标UP事件
        private void picDispTrack_MouseUp(object sender, MouseEventArgs e)
        {
            _imageMoveFlag = false;
        }

        //picture box控件鼠标Leave事件
        private void picDispTrack_MouseLeave(object sender, EventArgs e)
        {
            _imageMoveFlag = false;
        }

        //picture box控件鼠标Move事件
        private void picDispTrack_MouseMove(object sender, MouseEventArgs e)
        {
            if (_imageMoveFlag)//如果图像移动
            {
                picDispTrack.Left += Convert.ToInt16(e.X - _mouseSourcePosition.X);//设置X坐标
                picDispTrack.Top += Convert.ToInt16(e.Y - _mouseSourcePosition.Y);//设置Y坐标
            }
        }

        //picture box控件鼠标Click事件
        private void picDispTrack_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)//如果点击的是鼠标右键
            {
                cmsTrackImageControl.Show((PictureBox)sender, new Point(e.X, e.Y));
            }
        }

        //恢复轨迹图尺寸
        private void 恢复原图大小ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            picDispTrack.Size = _picCtrlSourceSize;
            picDispTrack.Location = _picCtrlSourceLocation;
        }

        //导出轨迹图
        private void 导出轨迹图ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = "导出图片";
            sfd.Filter = "jpg(*.jpg)|*.jpg";
            sfd.FileName = "ExportImage";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    _trackBmp.Save(sfd.FileName);
                    MessageBox.Show("导出图片成功！");
                }
                catch
                {
                    MessageBox.Show("导出图片失败！");
                }
            }
        }

        #endregion

    }
}
