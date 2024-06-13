
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
    public partial class FindLineFrm : Form
    {
        private string ShapeModelSavePath;

        /// <summary>
        /// 边缘查找ROI保存地址
        /// </summary>
        public string FindEdgeROISavePath
        {
            get
            {
                return $"{ShapeModelSavePath}/FindEdgeROI.roi";
            }
        }
        /// <summary>
        /// 边缘查找参数保存地址
        /// </summary>
        public string FindEdgeCfgSavePath
        {
            get
            {
                return $"{ShapeModelSavePath}/FindEdgeCfg.xml";
            }
        }

       

        public List<ROI> FindEdgeRoiList = new List<ROI>();//边缘查找ROI

        public List<FindLineCfg> FindLineCfgList { set; get; } = new List<FindLineCfg>();//边缘查找参数

 

        private const string FIND_EDGE_ID_PR = "FindEdge_";//ROI名称前缀

    
        private HObject TestImage;//测试时的图像
        private HObject OrigImage;//模板图像，每一个测量块对应一个模板
        private HObject ModelAffineContour;//模板轮廓

        private HWindow_Final hw_FindEdge = new HWindow_Final();//边缘查找窗体


        public FindLineFrm(string ShapeModelSavePath)
        {
            InitializeComponent();
            this.ShapeModelSavePath = ShapeModelSavePath;

            splitContainer1.Panel1.Controls.Add(hw_FindEdge);
            hw_FindEdge.Dock = DockStyle.Fill;
            //hw_FindEdge.HobjectToHimage(Manager.EmptyImage);

            hw_FindEdge.viewWindow._roiController.NotifyRCObserver += new IconicDelegate(RoiMove_FindEdge);

            ShapeModelFrm.clearModelEvent += ShapeModelFrm_clearModelEvent;

            HOperatorSet.GenEmptyObj(out TestImage);

        }

        /// <summary>
        /// 清除模板时同步清除检测参数
        /// </summary>
        private void ShapeModelFrm_clearModelEvent()
        {
            FindLineCfgList.Clear();
            FindEdgeRoiList.Clear();
        }


       

        public void Init(string ShapeModelSavePath, HObject OrigImage, HObject ModelAffineContour)
        {
            try
            {
                this.ShapeModelSavePath = ShapeModelSavePath;

                //更新模板列表时加载模板参数
                this.OrigImage = OrigImage;

                TestImage.Dispose();
                HOperatorSet.CopyImage(OrigImage, out TestImage);

                this.ModelAffineContour = ModelAffineContour;


                FindLineCfgList.Clear();
                if (File.Exists(FindEdgeCfgSavePath))
                {
                    FindLineCfgList.AddRange((List<FindLineCfg>)XmlTool.ReadXML(FindEdgeCfgSavePath, typeof(List<FindLineCfg>)));
                }
                else
                {
                    XmlTool.WriteXML(FindLineCfgList, FindEdgeCfgSavePath);
                }

                FindEdgeRoiList.Clear();
                if (File.Exists(FindEdgeROISavePath))
                {
                    hw_FindEdge.viewWindow.loadROI(FindEdgeROISavePath, out FindEdgeRoiList);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


        }

        private void RectEdgeFrm_Load(object sender, EventArgs e)
        {
            if (OrigImage == null || ModelAffineContour == null || !ModelAffineContour.IsInitialized() || !OrigImage.IsInitialized())
            {
                MessageBox.Show("模板未设置完成，请先设置模板参数！");
                
            }
            else
            {
                HOperatorSet.GetImageSize(OrigImage, out HTuple w, out HTuple h);
                HOperatorSet.CountObj(ModelAffineContour, out HTuple length);
                if (w.TupleLength() == 0 || length.TupleLength() == 0)
                {
                    MessageBox.Show("模板未设置完成，请先设置模板参数！");
                }
                else
                {
                   
                    RefreshHWind(hw_FindEdge);
                    Load2UI();
                    checkBox_ShowAllFindEdgeRegion.Checked = false;
                }
            }

            
        }

    

        private void RoiMove_FindEdge(int val)
        {
            switch (val)
            {
                case ROIController.EVENT_UPDATE_ROI:
                    int activeIdx = hw_FindEdge.viewWindow._roiController.getActiveROIIdx();
                    if (listBox_FindEdge.Items.Count > activeIdx)
                    {
                        listBox_FindEdge.SelectedIndex = activeIdx;
                    }
                    break;
                default:
                    break;
            }
        }


        public void Load2UI()
        {
            //显示边缘查找参数
            listBox_FindEdge.Items.Clear();
            for (int i = 0; i < FindLineCfgList.Count; i++)
            {
                listBox_FindEdge.Items.Add(FindLineCfgList[i].RoiId);
            }
            if (listBox_FindEdge.Items.Count > 0)
            {
                listBox_FindEdge.SelectedIndex = listBox_FindEdge.Items.Count - 1;
            }
        }





        #region 边缘查找

       
        private void BnAddFindEdgeRegion_Click(object sender, EventArgs e)//添加ROI
        {
            if (TestImage == null || TestImage.IsInitialized() == false)
            {
                return;
            }
            HOperatorSet.GetImageSize(TestImage, out HTuple ImgW, out HTuple ImgH);
            Random r = new Random();
            //hw_FindEdge.viewWindow.genRect2(ImgH / 2 + r.Next(-10, 10), ImgW / 2 + r.Next(-10, 10), 0, ImgH / 20, ImgW / 20, ref FindEdgeRoiList);
            hw_FindEdge.viewWindow.genCircle(ImgH / 2 + r.Next(-10, 10), ImgW / 2 + r.Next(-10, 10), 200, ref FindEdgeRoiList);

            //获取当前ROI ID最大值,新生成ROI+1
            string[] roiIdArr = FindLineCfgList.Select(x => x.RoiId).ToArray();
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
            listBox_FindEdge.Items.Add($"{FIND_EDGE_ID_PR}{newRoiId}");

            //保存对应参数
            FindLineCfgList.Add(new FindLineCfg()
            {
                RoiId = $"{FIND_EDGE_ID_PR}{newRoiId}",
                Transition = XmlTool.GetEnumByDescription<CfgDescription>(Cmb_FindLineTransition.Text).ToString(),
                Select = XmlTool.GetEnumByDescription<CfgDescription>(Cmb_FindLineSelect.Text).ToString(),
                MeasureThreshold = (int)NdFindLineEdgeThreshold.Value,
            });

            //ListBox显示最后,触发listBox_FindEdge_SelectedIndexChanged事件
            if (listBox_FindEdge.Items.Count > 0)
            {
                listBox_FindEdge.SelectedIndex = listBox_FindEdge.Items.Count - 1;
            }
        }

        private void listBox_FindEdge_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selectIdx = listBox_FindEdge.SelectedIndex;
            if (selectIdx == -1) { return; }

            RefreshHWind(hw_FindEdge);
            hw_FindEdge.viewWindow.selectROI(selectIdx);
            Cmb_FindLineSelect.SelectedItem = XmlTool.GetEnumDescription<CfgDescription>(FindLineCfgList[selectIdx].Select).ToString();
            Cmb_FindLineTransition.SelectedItem = XmlTool.GetEnumDescription<CfgDescription>(FindLineCfgList[selectIdx].Transition).ToString();
            NdFindLineEdgeThreshold.Value = (int)FindLineCfgList[selectIdx].MeasureThreshold;
            TestFindEdge(selectIdx, 1);
        }

        private void BnTestFindEdge_Click(object sender, EventArgs e)//测试ROI
        {
            RefreshHWind(hw_FindEdge);
            if (checkBox_ShowAllFindEdgeRegion.Checked)
            {
                TestFindEdge(0, FindEdgeRoiList.Count);
            }
            else
            {
                int selectIdx = listBox_FindEdge.SelectedIndex;
                if (selectIdx == -1) { return; }
                TestFindEdge(selectIdx, 1);
            }

        }

        private void BnSaveFindEdge_Click(object sender, EventArgs e)//保存参数
        {
            //if (FindEdgeRoiList.Count == 0) { MessageBox.Show("边缘查找ROI 为空，保存失败！"); return; }

            XmlTool.WriteXML(FindLineCfgList, FindEdgeCfgSavePath);
            hw_FindEdge.viewWindow.saveROI(FindEdgeRoiList, FindEdgeROISavePath);
            MessageBox.Show("保存成功！");
        }

        private void Cmb_FindLineTransition_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selectIdx = listBox_FindEdge.SelectedIndex;
            if (selectIdx == -1) { return; }

            FindLineCfgList[selectIdx].Transition = XmlTool.GetEnumByDescription<CfgDescription>(Cmb_FindLineTransition.Text).ToString();

            RefreshHWind(hw_FindEdge);
            TestFindEdge(selectIdx, 1);
        }

        private void Cmb_FindLineSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selectIdx = listBox_FindEdge.SelectedIndex;
            if (selectIdx == -1) { return; }

            FindLineCfgList[selectIdx].Select = XmlTool.GetEnumByDescription<CfgDescription>(Cmb_FindLineSelect.Text).ToString();

            RefreshHWind(hw_FindEdge);
            TestFindEdge(selectIdx, 1);

        }

        private void NdFindLineEdgeThreshold_ValueChanged(object sender, EventArgs e)
        {
            int selectIdx = listBox_FindEdge.SelectedIndex;
            if (selectIdx == -1) { return; }

            FindLineCfgList[selectIdx].MeasureThreshold = (int)NdFindLineEdgeThreshold.Value;

            RefreshHWind(hw_FindEdge);
            TestFindEdge(selectIdx, 1);
        }

        private void TestFindEdge(int StartIdx, int Length)
        {
            if (FindEdgeRoiList.Count < StartIdx + Length)
            {
                return;
            }
            for (int i = StartIdx; i < StartIdx + Length; i++)
            {
                HTuple roiData = FindEdgeRoiList[i].getModelData();
                HDevelopExport.Instance.FindLine(TestImage, out HObject MeasureContour, out HObject FitLine, out HObject Cross,
                    FindEdgeRoiList[i].getModelData(), FindLineCfgList[i].Transition, FindLineCfgList[i].Select,
                    FindLineCfgList[i].MeasureThreshold, out HTuple circleParam, out HTuple ErrorLog);

                hw_FindEdge.viewWindow.displayHobject(Cross, "gold");
                hw_FindEdge.viewWindow.displayHobject(FitLine, "red");
            }
            ShowRoi(hw_FindEdge,FindEdgeRoiList, checkBox_ShowAllFindEdgeRegion.Checked, StartIdx);
        }

        private void 删除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int selectIdx = listBox_FindEdge.SelectedIndex;
            if (selectIdx == -1) { return; }

            string removeStr = listBox_FindEdge.SelectedItem.ToString();
            string[] removeStrArr = removeStr.Split('_');
            FindEdgeRoiList.RemoveAt(selectIdx);
            FindLineCfgList.RemoveAt(selectIdx);
            listBox_FindEdge.Items.RemoveAt(selectIdx);
            
        }

        private void 删除所有ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RefreshHWind(hw_FindEdge);
            FindEdgeRoiList.Clear();
            FindLineCfgList.Clear();
            listBox_FindEdge.Items.Clear();
        }

        private void checkBox_ShowAllFindEdgeRegion_CheckedChanged(object sender, EventArgs e)
        {
            RefreshHWind(hw_FindEdge);
            if (checkBox_ShowAllFindEdgeRegion.Checked)
            {
                ShowRoi(hw_FindEdge,FindEdgeRoiList, true);
            }
            else
            {
                int selectIdx = listBox_FindEdge.SelectedIndex;
                if (selectIdx == -1) { return; }
                ShowRoi(hw_FindEdge,FindEdgeRoiList, false, selectIdx);
            }
        }
        #endregion










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
                if (roiList.Count>activeIdx)
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

        private bool RefreshHWind(HWindow_Final hw)
        {
            HOperatorSet.GetImageSize(TestImage, out HTuple w, out HTuple h);
            if (TestImage != null && TestImage.IsInitialized() && w.TupleLength() > 0)
            {
                hw.ClearWindow();
                hw.HobjectToHimage(TestImage);
                HOperatorSet.SetTposition(hw.hWindowControl.HalconWindow, 0, 0);
                if (ModelAffineContour != null)
                {
                    hw.viewWindow.displayHobject(ModelAffineContour, "red");
                }
                else { return false; }
            }
            else { return false; }
            return true;
        }

        

        
    }
}
