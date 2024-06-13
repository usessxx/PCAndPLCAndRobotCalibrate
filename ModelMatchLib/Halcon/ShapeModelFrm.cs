
using ChoiceTech.Halcon.Control;
using HalconDotNet;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ViewWindow.Model;

namespace MatchModel.Halcon
{
    public partial class ShapeModelFrm : Form
    {
        //修改导入模板的方式，根据产品的名称来分配


        private string ShapeModelSavePath;

        public ShapeModelCfg shapeModelCfg { get; set; } = new ShapeModelCfg();

        public string ModelCfgSavePath
        {
            get
            {
                return $"{ShapeModelSavePath}/ModelCfg.xml";
            }
        }
        public string ModelSearchROISavePath
        {
            get
            {
                return $"{ShapeModelSavePath}/SearchRoiCfg.roi";
            }
        }

        public string ModelSearchROICfgSavePath
        {
            get
            {
                return $"{ShapeModelSavePath}/SearchRoiCfg.xml";
            }
        }
        public string ModelCreateROISavePath
        {
            get
            {
                return $"{ShapeModelSavePath}/CreateRoiCfg.roi";
            }
        }
        public string ModelIdSavePath
        {
            get
            {
                return $"{ShapeModelSavePath}/modelId.shm";
            }
        }
        public string NccModelIdSavePath
        {
            get
            {
                return $"{ShapeModelSavePath}/nccModelId.shm";
            }
        }
        public string ModelImageSavePath
        {
            get
            {
                return $"{ShapeModelSavePath}/ModelImage.tiff";
            }
        }

        public string ModelOutputDictSavePath
        {
            get
            {
                return $"{ShapeModelSavePath}/OutputDict.tup";
            }
        }

        public static event Action clearModelEvent;//清楚模板事件


        HWindow_Final hw;//窗体
        ROIController roiController;//ROI控制器
        public List<ROI> createRoiList = new List<ROI>();//创建模板ROI集合
        public List<ROI> searchRoiList = new List<ROI>();//搜索模板ROI集合
        public List<FindModelROICfg> findModelRoiCfgList = new List<FindModelROICfg>();//模板搜索ROI参数
        private const string FIND_MODEL_ID_PR = "ModelROI_";//ROI名称前缀

        ShapeModelCfgEnum[] CvInputArr;

        private HObject ModelImage;//胶水图像
        public HObject OrigImage;//模板图像
        public HObject AffineModelContour;//创建模板时的轮廓
        public HTuple ModelId;//模板句柄
        public HTuple NccModelId;//Ncc模板句柄
        public HTuple InputDict;//创建查找模板的输入参数
        public HTuple ModelOutputTuple;//创建模板时输出参数

        public HObject TestImage;

        HTuple hv_WindowHandle;//窗口句柄
        public ShapeModelFrm(string ShapeModelSavePath)
        {
            InitializeComponent();
            this.ShapeModelSavePath = ShapeModelSavePath;

            hw = new HWindow_Final();
            splitContainer1.Panel1.Controls.Add(hw);
            hw.Dock = DockStyle.Fill;
            //hw.HobjectToHimage(Manager.EmptyImage);
            roiController = hw.viewWindow._roiController;
            roiController.NotifyRCObserver += new IconicDelegate(RoiMove);

            HOperatorSet.GenEmptyObj(out OrigImage);
            HOperatorSet.GenEmptyObj(out ModelImage);
            HOperatorSet.GenEmptyObj(out TestImage);
            HOperatorSet.CreateDict(out InputDict);
            HOperatorSet.GenEmptyObj(out AffineModelContour);
            hv_WindowHandle = hw.hWindowControl.HalconWindow;
            //NdArr = new NumericUpDown[] { NdAngleStart, NdAngleExtent, NdAngleStep, NdMinScore, NdNumMatches, NdOverLap,
            //    NdNumLevel, NdMinContrast, NdMaxContrast, NdMinComponentSize };
            CvInputArr = new ShapeModelCfgEnum[]
            {
                ShapeModelCfgEnum.AngleStart,ShapeModelCfgEnum.AngleExtent,ShapeModelCfgEnum.AngleStep,
                ShapeModelCfgEnum.MinScore,ShapeModelCfgEnum.NumMatches,ShapeModelCfgEnum.MaxOverLap,
                ShapeModelCfgEnum.NumLevels,ShapeModelCfgEnum.MinContrast,ShapeModelCfgEnum.MaxContrast,ShapeModelCfgEnum.MinComponentLength
            };
        }



        

        private void ShapeModelFrm_Load(object sender, EventArgs e)
        {
            RefreshHWind(hw);
            InitModelCfgToUI();
            hw.viewWindow.displayROI(ref createRoiList);
        }


        public void Init(string ShapeModelSavePath)
        {
            this.ShapeModelSavePath = ShapeModelSavePath;
            if (File.Exists(ModelCfgSavePath))
            {
                shapeModelCfg = (ShapeModelCfg)XmlTool.ReadXML(ModelCfgSavePath, typeof(ShapeModelCfg));
            }
            else { XmlTool.WriteXML(shapeModelCfg, ModelCfgSavePath); }

            Type type = shapeModelCfg.GetType();
            for (int i = 0; i < CvInputArr.Length; i++)
            {
                PropertyInfo pinfo = type.GetProperty(CvInputArr[i].ToString());
                Type pInfoType = pinfo.PropertyType;
                if (pInfoType.Name == "Int32")
                {
                    HOperatorSet.SetDictTuple(InputDict, CvInputArr[i].ToString(), Convert.ToInt32(pinfo.GetValue(shapeModelCfg)));
                }
                else if (pInfoType.Name == "Double")
                {
                    HOperatorSet.SetDictTuple(InputDict, CvInputArr[i].ToString(), Convert.ToDouble(pinfo.GetValue(shapeModelCfg)));
                }
                
            }
            HOperatorSet.SetDictTuple(InputDict, ShapeModelCfgEnum.EnableNcc.ToString(), new HTuple(shapeModelCfg.EnableNcc));
            hw?.viewWindow.ClearWindow();
            hw?.viewWindow.notDisplayRoi();
            searchRoiList.Clear();
            OrigImage.Dispose();
            createRoiList.Clear();
            findModelRoiCfgList.Clear();
            ModelId = null;
            NccModelId = null;
            ModelOutputTuple = null;
            LoadModel(out createRoiList, out searchRoiList,out findModelRoiCfgList, out ModelId,out NccModelId, out OrigImage, out ModelOutputTuple);
            //判断是否为彩色图像，如是将边缘最锐利的图像作为模板图像
            HOperatorSet.CountChannels(OrigImage, out HTuple channels);
            if (channels.TupleLength() > 0 && channels.I == 3)
            {
                HDevelopExport.Instance.Color2OCAG(OrigImage, out HObject ag, out HObject oc);
                ModelImage.Dispose();
                ModelImage = ag;
            }
            else
            {
                ModelImage.Dispose();
                ModelImage = OrigImage;
            }

            AffineModelContour.Dispose();
            GenModelContour();

            TestImage.Dispose();
            HOperatorSet.GenEmptyObj(out TestImage);

        }

        private void RoiMove(int val)//ROI移动事件
        {
            switch (val)
            {
                case ROIController.EVENT_UPDATE_ROI:
                    int activeIdx = hw.viewWindow._roiController.getActiveROIIdx();
                    string type = hw.viewWindow._roiController.ROIList[activeIdx].GetType().Name;
                    if (type == "ROIRectangle1" && listBox_FindModelROI.Items.Count > activeIdx)
                    {
                        listBox_FindModelROI.SelectedIndex = activeIdx;
                    }
                    break;
                default:
                    break;
            }
        }

        public void GenModelContour()
        {
            //生成模板轮廓
            if (ModelId != null || ModelOutputTuple != null)
            {
                HOperatorSet.GetShapeModelContours(out HObject shapeModelContour, ModelId, 1);
                HOperatorSet.VectorAngleToRigid(0, 0, 0, ModelOutputTuple.TupleSelect(0),
                    ModelOutputTuple.TupleSelect(1), ModelOutputTuple.TupleSelect(2), out HTuple homMat);
                AffineModelContour.Dispose();
                HOperatorSet.AffineTransContourXld(shapeModelContour, out AffineModelContour, homMat);
                shapeModelContour.Dispose();
            }
        }

        /// <summary>
        /// 加载模板参数
        /// </summary>
        /// <param name="ModelName"></param>
        /// <param name="createRoiList"></param>
        /// <param name="searchRoiList"></param>
        /// <param name="ModelId"></param>
        /// <param name="Image"></param>
        /// <param name="ModelOutputDict"></param>
        public void LoadModel(out List<ROI> createRoiList, out List<ROI> searchRoiList,out List<FindModelROICfg> findModelRoiCfgList,
            out HTuple ModelId,out HTuple NccModelId, out HObject Image, out HTuple ModelOutputDict)
        {
            createRoiList = new List<ROI>();
            searchRoiList = new List<ROI>();
            findModelRoiCfgList = new List<FindModelROICfg>();
            ModelId = null;
            NccModelId = null;
            HOperatorSet.GenEmptyObj(out Image);
            ModelOutputDict = null;
            HWindow_Final hw = new HWindow_Final();
            if (File.Exists(ModelSearchROISavePath))
            {
                hw.viewWindow.loadROI(ModelSearchROISavePath, out searchRoiList);
            }
            if (File.Exists(ModelIdSavePath))
            {
                HOperatorSet.ReadShapeModel(ModelIdSavePath, out ModelId);
            }
            if (File.Exists(NccModelIdSavePath))
            {
                HOperatorSet.ReadNccModel(NccModelIdSavePath, out NccModelId);
            }
            if (File.Exists(ModelImageSavePath))
            {
                Image.Dispose();
                HOperatorSet.ReadImage(out Image, ModelImageSavePath);
            }
            if (File.Exists(ModelCreateROISavePath))
            {
                hw.viewWindow.loadROI(ModelCreateROISavePath, out createRoiList);
            }
            if (File.Exists(ModelOutputDictSavePath))
            {
                HOperatorSet.ReadTuple(ModelOutputDictSavePath, out ModelOutputDict);
            }
            if (File.Exists(ModelSearchROICfgSavePath))
            {
                findModelRoiCfgList =(List<FindModelROICfg>)XmlTool.ReadXML(ModelSearchROICfgSavePath, typeof(List<FindModelROICfg>));
            }
        }




        private void InitModelCfgToUI()//将参数显示在UI界面中
        {
            //Type type = shapeModelCfg.GetType();//通过反射将参数对应参数枚举，对应赋值
            //for (int i = 0; i < CvInputArr.Length; i++)
            //{
            //    PropertyInfo pinfo = type.GetProperty(CvInputArr[i].ToString());
            //    Type b = pinfo.PropertyType;
            //    dynamic c = b.Assembly.CreateInstance(b.FullName);
            //    c = pinfo.GetValue(shapeModelCfg);
            //    NdArr[i].Value = (decimal)c;
            //}

            propertyGrid1.SelectedObject = shapeModelCfg;
            //显示边缘查找参数
            listBox_FindModelROI.Items.Clear();
            for (int i = 0; i < findModelRoiCfgList.Count; i++)
            {
                listBox_FindModelROI.Items.Add(findModelRoiCfgList[i].RoiId);
            }
            if (listBox_FindModelROI.Items.Count > 0)
            {
                listBox_FindModelROI.SelectedIndex = listBox_FindModelROI.Items.Count - 1;
            }
            checkBox_EnableNcc.Checked = shapeModelCfg.EnableNcc;
        }

        #region 创建模板
        private void BnROI_Click(object sender, EventArgs e)
        {
            ToolStripButton tsbn = (ToolStripButton)sender;
            if (ModelImage == null || ModelImage.IsInitialized() == false)
            {
                return;
            }
            hw.HobjectToHimage(ModelImage);
            HOperatorSet.GetImageSize(ModelImage, out HTuple ImgW,out HTuple ImgH);
            createRoiList.Clear();
            switch (tsbn.Name)//将ROI显示在图像中心位置
            {
                case "ROI_Circle":
                    hw.viewWindow.genCircle(ImgH / 2, ImgW / 2, ImgW / 10, ref createRoiList);
                    break;
                case "ROI_Rect1":
                    hw.viewWindow.genRect1(ImgH / 2 - (ImgH / 10), ImgW / 2 - (ImgW / 10), ImgH / 2 + (ImgH / 10), ImgW / 2 + (ImgW / 10), ref createRoiList);
                    break;
                case "ROI_Rect2":
                    hw.viewWindow.genRect2(ImgH / 2, ImgW / 2, 0 , ImgH / 10, ImgW / 10, ref createRoiList);
                    break;
                default:
                    break;
            }
        }

        private void BnCreateModel_Click(object sender, EventArgs e)
        {
            if (ModelImage == null || ModelImage.IsInitialized() == false)
            {
                return;
            }
            HOperatorSet.GetImageSize(ModelImage, out HTuple w, out HTuple h);
            if (w.TupleLength() == 0)
            {
                return;
            }
            hw.HobjectToHimage(ModelImage);
            hw.viewWindow.displayROI(ref createRoiList);
            if (createRoiList.Count == 0)
            {
                MessageBox.Show("未设置模板ROI区域，创建失败！");
                return;
            }

            ShapeModelCfg2Dict();

            HTuple data = createRoiList[0].getModelData();
            string roiType = createRoiList[0].GetType().ToString();
            HObject ROI_Region;
            switch (roiType)
            {
                case "ViewWindow.Model.ROICircle":
                    HOperatorSet.GenCircle(out ROI_Region, data[0], data[1], data[2]);
                    break;
                case "ViewWindow.Model.ROIRectangle1":
                    HOperatorSet.GenRectangle1(out ROI_Region, data[0], data[1], data[2], data[3]);
                    break;
                case "ViewWindow.Model.ROIRectangle2":
                    HOperatorSet.GenRectangle2(out ROI_Region, data[0], data[1], -data[2].D, data[3], data[4]);
                    break;
                default:
                    MessageBox.Show("未设置模板ROI区域，创建失败！");
                    return;
            }
            HTuple exception;
            if (sender != null)
            {
                HDevelopExport.Instance.CreateShapeModel(ModelImage, ROI_Region, out HObject ModelContour, InputDict, out ModelId,out NccModelId, out exception);
            }
            AffineModelContour.Dispose();
            HDevelopExport.Instance.FindShapeModel(ModelImage, ROI_Region, out AffineModelContour, InputDict, ModelId,NccModelId, out ModelOutputTuple, out exception);
            hw.viewWindow.displayHobject(AffineModelContour, "red");
            MessageBox.Show("创建成功！");
        }

        private void toolStripButton_ShowCreateModelRoi_Click(object sender, EventArgs e)
        {
            if (ModelImage == null || ModelImage.IsInitialized() == false)
            {
                return;
            }
            hw.HobjectToHimage(ModelImage);
            hw.viewWindow.displayROI(ref createRoiList);
            hw.viewWindow.displayHobject(AffineModelContour, "red");

        }
        #endregion


        #region 查找模板
        private void BnAddSearchROI_Click(object sender, EventArgs e)
        {
            if (ModelImage == null || ModelImage.IsInitialized() == false)
            {
                return;
            }
            HOperatorSet.GetImageSize(ModelImage, out HTuple w, out HTuple h);
            if ( w.TupleLength() == 0)
            {
                return;
            }
            HOperatorSet.GetImageSize(ModelImage, out HTuple ImgW, out HTuple ImgH);
            Random r = new Random();
            hw.viewWindow.genRect1(ImgH / 2 - (ImgH / 10) + r.Next(-10, 10), ImgW / 2 - (ImgW / 10) + r.Next(-10, 10),
                ImgH / 2 + (ImgH / 10) + r.Next(-10, 10), ImgW / 2 + (ImgW / 10) + r.Next(-10, 10), ref searchRoiList);
        
            //获取当前ROI ID最大值,新生成ROI+1
            string[] roiIdArr = findModelRoiCfgList.Select(x => x.RoiId).ToArray();
            HTuple roiHtuple = new HTuple();
            for (int i = 0; i < roiIdArr.Length; i++)
            {
                roiHtuple = roiHtuple.TupleConcat(int.Parse(roiIdArr[i].Split('_')[1]));
            }
            int newRoiId;
            if (roiHtuple.TupleLength() == 0)
            {
                newRoiId = 1;
            }
            else { newRoiId = roiHtuple.TupleMax().I + 1; }

            //对应信息记录在ListBox中
            listBox_FindModelROI.Items.Add($"{FIND_MODEL_ID_PR}{newRoiId}");

            //保存对应参数
            findModelRoiCfgList.Add(new FindModelROICfg()
            {
                RoiId = $"{FIND_MODEL_ID_PR}{newRoiId}"
            });

            //ListBox显示最后,触发listBox_FindEdge_SelectedIndexChanged事件
            if (listBox_FindModelROI.Items.Count > 0)
            {
                listBox_FindModelROI.SelectedIndex = listBox_FindModelROI.Items.Count - 1;
            }
        }

        /// <summary>
        /// 删除选中ROI
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShapeModelFrm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Delete)
            {
                int ActiveId = roiController.getActiveROIIdx();
                if (ActiveId != -1)
                {
                    roiController.removeActive();
                    searchRoiList.RemoveAt(ActiveId);
                }
            }
        }


        private void BnFindModel_Click(object sender, EventArgs e)
        {
            ShapeModelCfg2Dict();

            if (ModelImage == null || ModelImage.IsInitialized() == false || ModelId == null || NccModelId == null) { return; }

            HOperatorSet.GetImageSize(TestImage,out HTuple w ,out HTuple h);
            if (w.TupleLength() == 0)
            {
                TestImage.Dispose();
                HOperatorSet.CopyImage(ModelImage, out TestImage);
            }
            hw.HobjectToHimage(TestImage);
            HDevelopExport.Instance.MatchAllModel(TestImage, ModelId, NccModelId, InputDict, searchRoiList,findModelRoiCfgList, out HTuple OutputDict, false,hw);

        }

        #endregion


        private void BnSaveModel_Click(object sender, EventArgs e)//保存模板
        {
            //保存NumericUpDown值
            Type type = shapeModelCfg.GetType();
            ShapeModelCfg2Dict();

            //检查模板参数是否齐全
            if (createRoiList.Count == 0) { MessageBox.Show("创建模板ROI 为空，模板保存失败！"); return; }
            if (ModelId == null) { MessageBox.Show("模板句柄不存在，模板保存失败！");return; }
            if (NccModelId == null) { MessageBox.Show("Ncc模板句柄不存在，模板保存失败！"); return; }
            if (OrigImage == null || OrigImage.IsInitialized() == false) { MessageBox.Show("模板图像为空或未初始化，模板保存失败！");return; }
            if (ModelOutputTuple == null) { MessageBox.Show("创建模板 匹配参数为空，模板保存失败！"); return; }

            

            XmlTool.WriteXML(shapeModelCfg, ModelCfgSavePath);
            hw.viewWindow.saveROI(searchRoiList, ModelSearchROISavePath);
            XmlTool.WriteXML(findModelRoiCfgList, ModelSearchROICfgSavePath);

            hw.viewWindow.saveROI(createRoiList, ModelCreateROISavePath);
            HOperatorSet.WriteShapeModel(ModelId, ModelIdSavePath);
            HOperatorSet.WriteNccModel(NccModelId, NccModelIdSavePath);
            HOperatorSet.WriteImage(OrigImage, "tiff", 0, ModelImageSavePath);

            

            HOperatorSet.WriteTuple(ModelOutputTuple, ModelOutputDictSavePath);
            MessageBox.Show("保存成功！");
        }




        public bool VerifyModelIsExist(string ModelName)
        {
            if (!Directory.Exists(ShapeModelSavePath))
            {
                Directory.CreateDirectory(ShapeModelSavePath);
            }
            foreach (var item in GetAllModelNames())
            {
                if (item == ModelName)
                {
                    return true;
                }
            }
            return false;
        }

        public List<string> GetAllModelNames()
        {
            DirectoryInfo di = new DirectoryInfo(ShapeModelSavePath);
            return di.GetDirectories().Select(d => d.Name).ToList();
        }




        #region 掩膜  （功能待完善）
        private HObject brush_region = new HObject();//笔刷
        private HObject final_region = new HObject();//需要获得的区域

        private void HWindowControl_HMouseUp(object sender, HMouseEventArgs e)
        {
            if (!isModelMaskAble)
            {
                return;
            }
            HObject brush_region_affine = new HObject();
            HTuple hv_Button = null;
            HTuple hv_Row = null, hv_Column = null;
            HTuple areaBrush, rowBrush, columnBrush, homMat2D;

            HOperatorSet.AreaCenter(brush_region, out areaBrush, out rowBrush, out columnBrush);

            Application.DoEvents();

            hv_Row = -1;
            hv_Column = -1;
            hv_Button = 1;
            while (hv_Button.TupleLength() != 0 && hv_Button == 1)
            {
                //获取鼠标坐标
                try
                {
                    HOperatorSet.GetMposition(hv_WindowHandle, out hv_Row, out hv_Column, out hv_Button);

                    HObject regPoint, intersectReg;
                    HOperatorSet.GenRegionPoints(out regPoint, hv_Row, hv_Column);
                    HOperatorSet.Intersection(final_region, regPoint, out intersectReg);
                    HTuple intersectRow, intersectCol;
                    HOperatorSet.GetRegionPoints(intersectReg, out intersectRow, out intersectCol);
                    if (intersectRow.TupleLength() > 0)
                    {
                        continue;
                    }
                }
                catch (HalconException)
                {
                    //hv_Button = 0;
                }


                HOperatorSet.SetSystem("flush_graphic", "false");
                if (final_region.IsInitialized())
                {
                    hw.viewWindow.displayHobject(final_region, "blue", true);
                }


                //check if mouse cursor is over window
                if (hv_Row >= 0 && hv_Column >= 0)
                {
                    //放射变换
                    HOperatorSet.VectorAngleToRigid(rowBrush, columnBrush, 0, hv_Row, hv_Column, 0, out homMat2D);
                    brush_region_affine.Dispose();
                    HOperatorSet.AffineTransRegion(brush_region, out brush_region_affine, homMat2D, "nearest_neighbor");
                    hw.viewWindow.displayHobject(brush_region_affine, "blue", true);

                    HOperatorSet.SetSystem("flush_graphic", "true");
                    HDevelopExport.Instance.set_display_font(hv_WindowHandle, 20, "mono", new HTuple("true"), new HTuple("false"));
                    HDevelopExport.Instance.disp_message(hv_WindowHandle, "按下鼠标左键涂画", "window", 20, 20, "red", "false");


                    if (final_region.IsInitialized())
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.Union2(final_region, brush_region_affine, out ExpTmpOutVar_0);
                        final_region.Dispose();
                        final_region = ExpTmpOutVar_0;
                    }
                    else
                    {
                        final_region = new HObject(brush_region_affine);
                    }
                }
                else
                {
                    HDevelopExport.Instance.set_display_font(hv_WindowHandle, 20, "mono", new HTuple("true"), new HTuple("false"));
                    HDevelopExport.Instance.disp_message(hv_WindowHandle, "请将鼠标移动到窗口内部", "window", 20, 20, "red", "false");
                }
            }

        }

        bool isModelMaskAble = false;
        private void toolStripButton_ModelMask_Click(object sender, EventArgs e)
        {
            if (toolStripButton_ModelMask.BackColor == SystemColors.ControlDark)
            {
                isModelMaskAble = true;
                toolStripButton_ModelMask.BackColor = Color.Lime;

                HTuple hv_Row1 = 0, hv_Column1 = 0,
                    hv_Row2 = 15, hv_Column2 = 15;

                HObject ho_temp_brush = new HObject();
                try
                {
                    //画图模式 开
                    hw.DrawModel = true;
                    hw.Focus();
                    //锁住功能区
                    //显示提示
                    HDevelopExport.Instance.set_display_font(hv_WindowHandle, 20, "mono", new HTuple("true"), new HTuple("false"));

                    //显示为黄色
                    HOperatorSet.SetColor(hv_WindowHandle, "yellow");

                    //HOperatorSet.DrawRectangle1(hv_WindowHandle, out hv_Row1, out hv_Column1, out hv_Row2,
                    //    out hv_Column2);
                    ho_temp_brush.Dispose();
                    HOperatorSet.GenRectangle1(out ho_temp_brush, hv_Row1, hv_Column1, hv_Row2,
                        hv_Column2);
                    //

                    brush_region.Dispose();
                    brush_region = ho_temp_brush;


                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    hw.DispObj(ho_temp_brush, "yellow");
                    hw.DrawModel = false;
                }

            }
            else
            {
                isModelMaskAble = false;
                toolStripButton_ModelMask.BackColor = SystemColors.ControlDark;
                hw.DrawModel = false;
            }
        }
        #endregion

        private void toolStripButton_InputImage_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string path = ofd.FileName;
                    

                    OrigImage.Dispose();
                    HOperatorSet.ReadImage(out OrigImage, path);
                    //判断是否为彩色图像，如是将边缘最锐利的图像作为模板图像
                    HOperatorSet.CountChannels(OrigImage, out HTuple channels);
                    if (channels.TupleLength() > 0 && channels.I == 3)
                    {
                        HDevelopExport.Instance.Color2OCAG(OrigImage, out HObject ag, out HObject oc);
                        ModelImage.Dispose();
                        ModelImage = ag;
                    }
                    else
                    {
                        ModelImage.Dispose();
                        ModelImage = OrigImage;
                    }
                    hw.HobjectToHimage(ModelImage);
                }
            }
        }

        private void toolStripButton_ClearModel_Click(object sender, EventArgs e)
        {
            hw?.viewWindow.ClearWindow();
            hw?.viewWindow.notDisplayRoi();
            searchRoiList.Clear();
            OrigImage.Dispose();
            ModelImage.Dispose();
            createRoiList.Clear();
            findModelRoiCfgList.Clear();
            ModelId = null;
            NccModelId = null;
            ModelOutputTuple = null;
            listBox_FindModelROI.Items.Clear();
            AffineModelContour.Dispose();
            TestImage.Dispose();
            if (MessageBox.Show("是否同步清除检测参数！", "提示",MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                clearModelEvent?.Invoke();
            }
        }
        private bool RefreshHWind(HWindow_Final hw)
        {
            if (ModelImage != null && ModelImage.IsInitialized())
            {
                HOperatorSet.GetImageSize(ModelImage, out HTuple w, out HTuple h);
                if (w.TupleLength() == 0)
                {
                    return false;
                }
                hw.HobjectToHimage(ModelImage);
                HOperatorSet.SetTposition(hw.hWindowControl.HalconWindow, 0, 0);
                if (AffineModelContour != null)
                {
                    hw.viewWindow.displayHobject(AffineModelContour, "red");
                }
                else { return false; }
            }
            else { return false; }
            return true;
        }

        private bool RefreshTestHWind(HWindow_Final hw)
        {
            if (ModelImage != null && ModelImage.IsInitialized())
            {
                HOperatorSet.GetImageSize(ModelImage, out HTuple w, out HTuple h);
                if (w.TupleLength() == 0)
                {
                    return false;
                }
                if (TestImage == null || TestImage.IsInitialized() == false)
                {
                    HOperatorSet.CopyImage(ModelImage, out TestImage);
                }
                else
                {
                    HOperatorSet.GetImageSize(TestImage, out w, out h);
                    if (w.TupleLength() == 0)
                    {
                        TestImage.Dispose();
                        HOperatorSet.CopyImage(ModelImage, out TestImage);
                    }
                }
                hw.HobjectToHimage(TestImage);
                HOperatorSet.SetTposition(hw.hWindowControl.HalconWindow, 0, 0);
                if (AffineModelContour != null && AffineModelContour.IsInitialized())
                {
                    hw.viewWindow.displayHobject(AffineModelContour, "red");
                }
                else { return false; }
            }
            else { return false; }
            return true;
        }

        private void ShapeModelCfg2Dict()//通过反射将参数对应参数枚举，对应赋值
        {
            Type type = shapeModelCfg.GetType();
            for (int i = 0; i < CvInputArr.Length; i++)
            {
                PropertyInfo pinfo = type.GetProperty(CvInputArr[i].ToString());
                Type pInfoType = pinfo.PropertyType;
                if (pInfoType.Name == "Int32")
                {
                    HOperatorSet.SetDictTuple(InputDict, CvInputArr[i].ToString(), Convert.ToInt32(pinfo.GetValue(shapeModelCfg)));
                }
                else if (pInfoType.Name == "Double")
                {
                    HOperatorSet.SetDictTuple(InputDict, CvInputArr[i].ToString(), Convert.ToDouble(pinfo.GetValue(shapeModelCfg)));
                }
                else if (pInfoType.Name == "Bool")
                {
                    HOperatorSet.SetDictTuple(InputDict, ShapeModelCfgEnum.EnableNcc.ToString(), Convert.ToDouble(pinfo.GetValue(shapeModelCfg)));
                }
            }
        }

        private void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            string changeValue = e.ChangedItem.Value.ToString();//2
            string changeName = e.ChangedItem.PropertyDescriptor.Name;//NumMatches
            

            Type type = shapeModelCfg.GetType();
            for (int i = 0; i < CvInputArr.Length; i++)
            {
                if (CvInputArr[i].ToString() == changeName)
                {
                    PropertyInfo pinfo = type.GetProperty(CvInputArr[i].ToString());
                    Type pInfoType = pinfo.PropertyType;
                    if (pInfoType.Name == "Int32")
                    {
                        HOperatorSet.SetDictTuple(InputDict, CvInputArr[i].ToString(), Convert.ToInt32(changeValue.ToString()));
                    }
                    else if (pInfoType.Name == "Double")
                    {
                        HOperatorSet.SetDictTuple(InputDict, CvInputArr[i].ToString(), Convert.ToDouble(changeValue.ToString()));
                    }

                }
            }
        }
        private void checkBox_EnableNcc_CheckedChanged(object sender, EventArgs e)
        {
            shapeModelCfg.EnableNcc = checkBox_EnableNcc.Checked;
            HOperatorSet.SetDictTuple(InputDict, ShapeModelCfgEnum.EnableNcc.ToString(), new HTuple(shapeModelCfg.EnableNcc));
        }
        private void listBox_FindModelROI_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selectIdx = listBox_FindModelROI.SelectedIndex;
            if (selectIdx == -1) { return; }

            RefreshTestHWind(hw);

            ShowRoi(hw, searchRoiList, true,selectIdx);
        }

        private void 删除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int selectIdx = listBox_FindModelROI.SelectedIndex;
            if (selectIdx == -1) { return; }

            searchRoiList.RemoveAt(selectIdx);
            findModelRoiCfgList.RemoveAt(selectIdx);
            listBox_FindModelROI.Items.RemoveAt(selectIdx);
            if (listBox_FindModelROI.Items.Count > 0)
            {
                listBox_FindModelROI.SelectedIndex = listBox_FindModelROI.Items.Count - 1;
            }
            else { RefreshTestHWind(hw); }
        }

        /// <summary>
        /// 显示ROI
        /// </summary>
        /// <param name="roiList"></param>
        /// <param name="isShowAll">是否全部显示</param>
        /// <param name="activeIdx">如不是全部显示，输入显示下表</param>
        private void ShowRoi(HWindow_Final hw, List<ROI> roiList, bool isShowAll, int activeIdx = 0)
        {
            if (isShowAll)
            {
                hw.viewWindow.notDisplayRoi();
                hw.viewWindow._roiController.viewController.ShowAllRoiModel = -1;
                hw.viewWindow.displayROI(ref roiList);
                if (roiList.Count > activeIdx)
                {
                    hw.viewWindow.selectROI(activeIdx);
                }
            }
            else
            {
                if (hw.viewWindow._roiController.ROIList.Count != roiList.Count)
                {
                    hw.viewWindow._roiController.viewController.ShowAllRoiModel = -1;
                    hw.viewWindow.displayROI(ref roiList);
                }
                hw.viewWindow._roiController.viewController.ShowAllRoiModel = activeIdx;
                hw.viewWindow._roiController.viewController.repaint(activeIdx);
            }
        }

        private void BnInputTestImage_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string path = ofd.FileName;
                    TestImage.Dispose();
                    HOperatorSet.ReadImage(out HObject obj, path);
                    //判断是否为彩色图像，如是将边缘最锐利的图像作为模板图像
                    HOperatorSet.CountChannels(obj, out HTuple channels);
                    if (channels.TupleLength() > 0 && channels.I == 3)
                    {
                        HDevelopExport.Instance.Color2OCAG(obj, out HObject ag, out HObject oc);
                        HOperatorSet.CopyImage(oc, out TestImage);
                    }
                    else
                    {
                        HOperatorSet.CopyImage(obj, out TestImage);
                    }


                    RefreshTestHWind(hw);
                    ShowRoi(hw, searchRoiList, true, listBox_FindModelROI.SelectedIndex);
                }
            }
        }

        private void BnTestModelImage_Click(object sender, EventArgs e)
        {
            if (ModelImage != null && ModelImage.IsInitialized())
            {
                HOperatorSet.GetImageSize(ModelImage, out HTuple w, out HTuple h);
                if (w.TupleLength() == 0)
                {
                    MessageBox.Show("模板图像未空！");
                    return;
                }
                TestImage?.Dispose();
                HOperatorSet.CopyImage(ModelImage,out TestImage);
                hw.HobjectToHimage(TestImage);
                HDevelopExport.Instance.MatchAllModel(TestImage, ModelId,NccModelId, InputDict, searchRoiList, findModelRoiCfgList, out HTuple OutputDict,false, hw);
            }
            else { MessageBox.Show("模板图像未空！"); }
        }

        
    }
}
