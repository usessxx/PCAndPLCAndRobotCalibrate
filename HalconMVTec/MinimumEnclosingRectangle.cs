using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChoiceTech.Halcon.Control;
using HalconDotNet;
using System.Windows.Forms;

namespace HalconMVTec
{
    public class MinimumEnclosingRectangle
    {
        #region MERCal:获取最小外接矩形(MinimumEnclosingRectangle)及其相关数据
        /// <summary>
        /// MERCal:获取最小外接矩形(MinimumEnclosingRectangle)及其相关数据
        /// 中心点Row坐标
        /// 中心点Column坐标
        /// 角度(宽度方向平行于水平轴为0度,逆时钟为正方向)
        /// 宽度
        /// 高度
        /// 第1个顶点的Row坐标(右上角为第一个顶点,按逆时钟排序)
        /// 第2个顶点的Row坐标
        /// 第3个顶点的Row坐标
        /// 第4个顶点的Row坐标
        /// 第1个顶点的Column坐标
        /// 第2个顶点的Column坐标
        /// 第3个顶点的Column坐标
        /// 第4个顶点的Column坐标
        /// </summary>
        /// <param name="ho_ImageInput">HObject,输入图片</param>
        /// <param name="_MERData">float[],最小外接矩形数据</param>
        /// <returns>int:0,1,-1(OK,找到多个矩形,找矩形失败)</returns>
        public int MERCal(HWindow_Final hw, HObject ho_ImageInput, float ThresholdMinGray, float ThresholdMaxGray, float OpeningWAndH,
                           float ShapeMinArea, float ShapeMaxArea, float ShapeMinRectangularity, float ShapeMaxRectangularity,
                           float DilationRectangleWidth, float DilationRectangleHeight,
                           float EdgesSubPixCannyAlpha, float EdgesSubPixCannyLow, float EdgesSubPixCannyHigh,
                           float UnionCollinearContoursXldMaxDistAbs, float UnionCollinearContoursXldMaxDistRel, float UnionCollinearContoursXldMaxShift, float UnionCollinearContoursXldMaxAngle,
                           float XldContlengthMin, float XldContlengthMax,
                           float XLDMinArea, float XLDMaxArea, float XLDMinRectangularity, float XLDMaxRectangularity,
                           float UnionAdjacentContoursXldMaxDistAbs, float UnionAdjacentContoursXldMaxDistRel,
                           ref float[] _MERData, bool AngleRotate, bool retract = true)
        {
            _MERData = new float[13];
            // Local iconic variables
            HObject ho_Image, ho_Region, ho_RegionFillUp;
            HObject ho_ConnectedRegions, ho_SelectedRegions, ho_RegionUnion;
            HObject ho_ImageReduced, ho_RegionBorder, ho_RegionDilation;
            HObject ho_ImageReduced1, ho_Edges, ho_Edges2, ho_RectangleEdges, ho_RectangleEdges2, ho_RectangleEdges3;
            HObject ho_Rectangle;
            // Local control variables
            HTuple hv_Width = new HTuple(), hv_Height = new HTuple();
            HTuple hv_WindowHandle = new HTuple(), hv_Row = new HTuple();
            HTuple hv_Column = new HTuple(), hv_Phi = new HTuple();
            HTuple hv_Length1 = new HTuple(), hv_Length2 = new HTuple();
            HTuple hv_PointOrder = new HTuple(), hv_String1 = new HTuple();
            HTuple hv_String2 = new HTuple(), hv_Deg = new HTuple();
            HTuple hv_DegString = new HTuple(), hv_Row1 = new HTuple();
            HTuple hv_Col1 = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Image);
            HOperatorSet.GenEmptyObj(out ho_Region);
            HOperatorSet.GenEmptyObj(out ho_RegionFillUp);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions);
            HOperatorSet.GenEmptyObj(out ho_RegionUnion);
            HOperatorSet.GenEmptyObj(out ho_ImageReduced);
            HOperatorSet.GenEmptyObj(out ho_RegionBorder);
            HOperatorSet.GenEmptyObj(out ho_RegionDilation);
            HOperatorSet.GenEmptyObj(out ho_ImageReduced1);
            HOperatorSet.GenEmptyObj(out ho_Edges);
            HOperatorSet.GenEmptyObj(out ho_Edges2);
            HOperatorSet.GenEmptyObj(out ho_RectangleEdges);
            HOperatorSet.GenEmptyObj(out ho_RectangleEdges2);
            HOperatorSet.GenEmptyObj(out ho_RectangleEdges3);
            HOperatorSet.GenEmptyObj(out ho_Rectangle);

            try
            {
                ho_Image.Dispose();
                HOperatorSet.CopyImage(ho_ImageInput, out ho_Image);
                ho_Region.Dispose();
                ho_ConnectedRegions = ThresholdTest(hw, ho_Image, ThresholdMinGray, ThresholdMaxGray, OpeningWAndH, retract);
                if (ho_ConnectedRegions == null || ho_ConnectedRegions.CountObj() == 0)
                    return -10;
                ho_SelectedRegions.Dispose();
                ho_RegionBorder = SelectShapeTest(hw, ho_Image, ho_ConnectedRegions, ShapeMinArea, ShapeMaxArea, ShapeMinRectangularity, ShapeMaxRectangularity);
                if (ho_RegionBorder == null || ho_RegionBorder.CountObj() == 0)
                    return -11;
                ho_RegionDilation.Dispose();
                ho_ImageReduced1 = DilationRectangle1Test(hw, ho_Image, ho_RegionBorder, DilationRectangleWidth, DilationRectangleHeight);
                if (ho_ImageReduced1 == null || ho_ImageReduced1.CountObj() == 0)
                    return -12;
                ho_Edges.Dispose();
                ho_Edges = EdgesSubPixTest(hw, ho_Image, ho_ImageReduced1, EdgesSubPixCannyAlpha, EdgesSubPixCannyLow, EdgesSubPixCannyHigh);
                if (ho_Edges == null || ho_Edges.CountObj() == 0)
                    return -13;
                ho_Edges2.Dispose();
                ho_Edges2 = UnionCollinearContoursXld(hw, ho_Image, ho_Edges, UnionCollinearContoursXldMaxDistAbs, UnionCollinearContoursXldMaxDistRel, UnionCollinearContoursXldMaxShift, UnionCollinearContoursXldMaxAngle);
                if (ho_Edges2 == null || ho_Edges2.CountObj() == 0)
                    return -14;
                ho_RectangleEdges.Dispose();
                ho_RectangleEdges = SelectShapeXld_ContlengthTest(hw, ho_Image, ho_Edges2, XldContlengthMin, XldContlengthMax);
                if (ho_RectangleEdges == null || ho_RectangleEdges.CountObj() == 0)
                    return -15;
                ho_RectangleEdges2.Dispose();
                ho_RectangleEdges2 = SelectShapeXld_AreaAndRectangularityTest(hw, ho_Image, ho_RectangleEdges, XLDMinArea, XLDMaxArea, XLDMinRectangularity, XLDMaxRectangularity);
                if (ho_RectangleEdges2 == null || ho_RectangleEdges2.CountObj() == 0)
                    return -16;
                ho_RectangleEdges3.Dispose();
                ho_RectangleEdges3 = UnionAdjacentContoursXldTest(hw, ho_Image, ho_RectangleEdges2, UnionAdjacentContoursXldMaxDistAbs, UnionAdjacentContoursXldMaxDistRel);
                if (ho_RectangleEdges3 == null || ho_RectangleEdges3.CountObj() == 0)
                    return -17;

                hv_Row.Dispose();
                hv_Column.Dispose();
                hv_Phi.Dispose();
                hv_Length1.Dispose();
                hv_Length2.Dispose();
                hv_PointOrder.Dispose();
                HOperatorSet.FitRectangle2ContourXld(ho_RectangleEdges3, "tukey", -1, 0, 0, 3, 2, out hv_Row, out hv_Column, out hv_Phi, out hv_Length1, out hv_Length2, out hv_PointOrder);
                if (hv_Row == null || hv_Row.Length == 0)
                    return -18;
                ho_Rectangle.Dispose();
                HOperatorSet.GenRectangle2ContourXld(out ho_Rectangle, hv_Row, hv_Column, hv_Phi, hv_Length1, hv_Length2);
                if (ho_Rectangle == null || ho_Rectangle.CountObj() == 0)
                    return -19;
                HOperatorSet.TupleDeg(hv_Phi, out hv_Deg);

                //在这里需要增加对得到的矩形的数量检查
                if (ho_Rectangle.CountObj() != 1)
                {
                    ho_Image.Dispose();
                    ho_Region.Dispose();
                    ho_RegionFillUp.Dispose();
                    ho_ConnectedRegions.Dispose();
                    ho_SelectedRegions.Dispose();
                    ho_RegionUnion.Dispose();
                    ho_ImageReduced.Dispose();
                    ho_RegionBorder.Dispose();
                    ho_RegionDilation.Dispose();
                    ho_ImageReduced1.Dispose();
                    ho_Edges.Dispose();
                    ho_Edges2.Dispose();
                    ho_RectangleEdges.Dispose();
                    ho_RectangleEdges2.Dispose();
                    ho_RectangleEdges3.Dispose();
                    ho_Rectangle.Dispose();

                    hv_Width.Dispose();
                    hv_Height.Dispose();
                    hv_WindowHandle.Dispose();
                    hv_Row.Dispose();
                    hv_Column.Dispose();
                    hv_Phi.Dispose();
                    hv_Length1.Dispose();
                    hv_Length2.Dispose();
                    hv_PointOrder.Dispose();
                    hv_String1.Dispose();
                    hv_String2.Dispose();
                    hv_Deg.Dispose();
                    hv_DegString.Dispose();
                    hv_Row1.Dispose();
                    hv_Col1.Dispose();
                    return 1;//找到多个矩形
                }

                hv_Row1.Dispose();
                hv_Col1.Dispose();
                HOperatorSet.GetContourXld(ho_Rectangle, out hv_Row1, out hv_Col1);



                _MERData[0] = hv_Row[0].F;//中心点Row坐标
                _MERData[1] = hv_Column[0].F;//中心点Column坐标
                _MERData[2] = hv_Deg[0].F;//角度(宽度方向平行于水平轴为0度,逆时钟为正方向)
                _MERData[3] = hv_Length1[0].F * 2;//宽度
                _MERData[4] = hv_Length2[0].F * 2;//高度
                _MERData[5] = hv_Row1[0].F;//第1个顶点的Row坐标(右上角为第一个顶点,按逆时钟排序)
                _MERData[6] = hv_Row1[1].F;//第2个顶点的Row坐标
                _MERData[7] = hv_Row1[2].F;//第3个顶点的Row坐标
                _MERData[8] = hv_Row1[3].F;//第4个顶点的Row坐标
                _MERData[9] = hv_Col1[0].F;//第1个顶点的Column坐标
                _MERData[10] = hv_Col1[1].F;//第2个顶点的Column坐标
                _MERData[11] = hv_Col1[2].F;//第3个顶点的Column坐标
                _MERData[12] = hv_Col1[3].F;//第4个顶点的Column坐标

                if (AngleRotate)
                {
                    if (hv_Deg.D > 0 && hv_Deg.D <= 90)
                    {
                        _MERData[2] = (float)(hv_Deg.D - 180);
                    }
                }

                GetRectRT(ho_Rectangle, out HTuple RTRow, out HTuple RTCol);
                _MERData[5] = RTRow[0].F;
                _MERData[9] = RTCol[0].F;

                ho_Image.Dispose();
                ho_Region.Dispose();
                ho_RegionFillUp.Dispose();
                ho_ConnectedRegions.Dispose();
                ho_SelectedRegions.Dispose();
                ho_RegionUnion.Dispose();
                ho_ImageReduced.Dispose();
                ho_RegionBorder.Dispose();
                ho_RegionDilation.Dispose();
                ho_ImageReduced1.Dispose();
                ho_Edges.Dispose();
                ho_Edges2.Dispose();
                ho_RectangleEdges.Dispose();
                ho_RectangleEdges2.Dispose();
                ho_RectangleEdges3.Dispose();
                ho_Rectangle.Dispose();

                hv_Width.Dispose();
                hv_Height.Dispose();
                hv_WindowHandle.Dispose();
                hv_Row.Dispose();
                hv_Column.Dispose();
                hv_Phi.Dispose();
                hv_Length1.Dispose();
                hv_Length2.Dispose();
                hv_PointOrder.Dispose();
                hv_String1.Dispose();
                hv_String2.Dispose();
                hv_Deg.Dispose();
                hv_DegString.Dispose();
                hv_Row1.Dispose();
                hv_Col1.Dispose();
                return 0;
            }
            catch (Exception ex)
            {
                try//因为对象是NULL时,不能Dispose,所以增加此try,catch
                {
                    ho_Image.Dispose();
                    ho_Region.Dispose();
                    ho_RegionFillUp.Dispose();
                    ho_ConnectedRegions.Dispose();
                    ho_SelectedRegions.Dispose();
                    ho_RegionUnion.Dispose();
                    ho_ImageReduced.Dispose();
                    ho_RegionBorder.Dispose();
                    ho_RegionDilation.Dispose();
                    ho_ImageReduced1.Dispose();
                    ho_Edges.Dispose();
                    ho_Edges2.Dispose();
                    ho_RectangleEdges.Dispose();
                    ho_RectangleEdges2.Dispose();
                    ho_RectangleEdges3.Dispose();
                    ho_Rectangle.Dispose();

                    hv_Width.Dispose();
                    hv_Height.Dispose();
                    hv_WindowHandle.Dispose();
                    hv_Row.Dispose();
                    hv_Column.Dispose();
                    hv_Phi.Dispose();
                    hv_Length1.Dispose();
                    hv_Length2.Dispose();
                    hv_PointOrder.Dispose();
                    hv_String1.Dispose();
                    hv_String2.Dispose();
                    hv_Deg.Dispose();
                    hv_DegString.Dispose();
                    hv_Row1.Dispose();
                    hv_Col1.Dispose();
                }
                catch (Exception ex2)
                {

                }
                return -1;
            }
        }
        #endregion
        public void GetRectRT(HObject ho_Rectangle, out HTuple hv_RTRow, out HTuple hv_RTCol)
        {



            // Local iconic variables 

            // Local control variables 

            HTuple hv_Row = new HTuple(), hv_Col = new HTuple();
            HTuple hv_Area = new HTuple(), hv_Row1 = new HTuple();
            HTuple hv_Column = new HTuple(), hv_PointOrder = new HTuple();
            HTuple hv_Index = new HTuple(), hv_Angle = new HTuple();
            HTuple hv_jd = new HTuple();
            // Initialize local and output iconic variables 
            hv_RTRow = new HTuple();
            hv_RTCol = new HTuple();
            hv_Row.Dispose(); hv_Col.Dispose();
            HOperatorSet.GetContourXld(ho_Rectangle, out hv_Row, out hv_Col);
            hv_Area.Dispose(); hv_Row1.Dispose(); hv_Column.Dispose(); hv_PointOrder.Dispose();
            HOperatorSet.AreaCenterXld(ho_Rectangle, out hv_Area, out hv_Row1, out hv_Column,
                out hv_PointOrder);
            for (hv_Index = 0; (int)hv_Index <= (int)((new HTuple(hv_Row.TupleLength())) - 1); hv_Index = (int)hv_Index + 1)
            {
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_Angle.Dispose();
                    HOperatorSet.AngleLx(hv_Row1, hv_Column, hv_Row.TupleSelect(hv_Index), hv_Col.TupleSelect(
                        hv_Index), out hv_Angle);
                }
                hv_jd.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_jd = hv_Angle.TupleDeg()
                        ;
                }
                if ((int)((new HTuple(hv_jd.TupleGreater(0))).TupleAnd(new HTuple(hv_jd.TupleLessEqual(
                    90)))) != 0)
                {
                    hv_RTRow.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_RTRow = hv_Row.TupleSelect(
                            hv_Index);
                    }
                    hv_RTCol.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_RTCol = hv_Col.TupleSelect(
                            hv_Index);
                    }
                }
            }

            hv_Row.Dispose();
            hv_Col.Dispose();
            hv_Area.Dispose();
            hv_Row1.Dispose();
            hv_Column.Dispose();
            hv_PointOrder.Dispose();
            hv_Index.Dispose();
            hv_Angle.Dispose();
            hv_jd.Dispose();

            return;
        }

        #region ThresholdTest
        public HObject ThresholdTest(HWindow_Final hw, HObject ho_Image, float ThresholdMinGray, float ThresholdMaxGray, float OpeningWAndH, bool retract = true)
        {
            HObject ho_Region = new HObject();
            HObject ho_RegionFillUp = new HObject();
            HObject ho_ConnectedRegions = new HObject();
            try
            {
                if (retract)
                {
                    HOperatorSet.GetImageSize(ho_Image, out HTuple w, out HTuple h);
                    HOperatorSet.GenRectangle1(out HObject rect, 200, 200, h.I - 200, w.I - 200);
                    HOperatorSet.ReduceDomain(ho_Image, rect, out ho_Image);
                }

                ho_Region.Dispose();
                //参数需要调试
                HOperatorSet.Threshold(ho_Image, out ho_Region, ThresholdMinGray, ThresholdMaxGray);
                ho_RegionFillUp.Dispose();
                HOperatorSet.FillUp(ho_Region, out ho_RegionFillUp);
                ho_ConnectedRegions.Dispose();
                HOperatorSet.Connection(ho_RegionFillUp, out ho_ConnectedRegions);

                //ADD 202210111924
                HOperatorSet.OpeningRectangle1(ho_ConnectedRegions, out ho_ConnectedRegions, OpeningWAndH, OpeningWAndH);
                HOperatorSet.Connection(ho_ConnectedRegions, out ho_ConnectedRegions);

                hw.ClearWindow();
                hw.HobjectToHimage(ho_Image);
                hw.viewWindow.displayHobject(ho_ConnectedRegions, "green", true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                ho_ConnectedRegions = null;
            }
            return ho_ConnectedRegions;
        }
        #endregion

        #region SelectShapeTest
        public HObject SelectShapeTest(HWindow_Final hw, HObject ho_Image, HObject ho_ConnectedRegions, float ShapeMinArea, float ShapeMaxArea, float ShapeMinRectangularity, float ShapeMaxRectangularity)
        {
            HObject ho_SelectedRegions = new HObject();
            HObject ho_RegionUnion = new HObject();
            HObject ho_ImageReduced = new HObject();
            HObject ho_RegionBorder = new HObject();
            try
            {
                ho_SelectedRegions.Dispose();
                //参数需要调试
                //HOperatorSet.SelectShape(ho_ConnectedRegions, out ho_SelectedRegions, (new HTuple("area")).TupleConcat("rectangularity"), "and", (new HTuple(ShapeMinArea)).TupleConcat(ShapeMinRectangularity), (new HTuple(ShapeMaxArea)).TupleConcat(ShapeMaxRectangularity));
                HOperatorSet.SelectShape(ho_ConnectedRegions, out ho_SelectedRegions, ((new HTuple("area")).TupleConcat("rectangularity")).TupleConcat("circularity"), "and", ((new HTuple(ShapeMinArea)).TupleConcat(ShapeMinRectangularity)).TupleConcat(0.5), ((new HTuple(ShapeMaxArea)).TupleConcat(ShapeMaxRectangularity)).TupleConcat(1));
                //HOperatorSet.SelectShape(ho_ConnectedRegions, out ho_SelectedRegions, (((((new HTuple("area")).TupleConcat("rectangularity")).TupleConcat("circularity")).TupleConcat("width")).TupleConcat("height")).TupleConcat("outer_radius"), "and", (((((new HTuple(ShapeMinArea)).TupleConcat(ShapeMinRectangularity)).TupleConcat(0.5)).TupleConcat(2400)).TupleConcat(2000)).TupleConcat(1500), (((((new HTuple(ShapeMaxArea)).TupleConcat(ShapeMaxRectangularity)).TupleConcat(1)).TupleConcat(2700)).TupleConcat(2300)).TupleConcat(1800));

                //宽长比筛选
                HTuple hv_Row = new HTuple(), hv_Column = new HTuple();
                HTuple hv_Phi = new HTuple(), hv_Length1 = new HTuple();
                HTuple hv_Length2 = new HTuple(), hv_ratio = new HTuple();
                HTuple hv_MinRatio = new HTuple(), hv_MaxRatio = new HTuple();
                HTuple hv_Greater = new HTuple(), hv_Selected = new HTuple();
                HTuple hv_Less = new HTuple(), hv_Selected1 = new HTuple();
                HTuple hv_Indices = new HTuple();
                HObject ho_ObjectSelected = new HObject();
                hv_Row.Dispose(); hv_Column.Dispose(); hv_Phi.Dispose(); hv_Length1.Dispose(); hv_Length2.Dispose();
                HOperatorSet.SmallestRectangle2(ho_SelectedRegions, out hv_Row, out hv_Column,
                    out hv_Phi, out hv_Length1, out hv_Length2);
                hv_ratio.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_ratio = hv_Length2 / hv_Length1;
                }
                hv_MinRatio.Dispose();
                hv_MinRatio = 0.75;
                hv_MaxRatio.Dispose();
                //hv_MaxRatio = 0.9;
                hv_MaxRatio = 0.99;
                hv_Greater.Dispose();
                HOperatorSet.TupleGreaterElem(hv_ratio, hv_MinRatio, out hv_Greater);
                hv_Selected.Dispose();
                HOperatorSet.TupleSelectMask(hv_ratio, hv_Greater, out hv_Selected);
                hv_Less.Dispose();
                HOperatorSet.TupleLessElem(hv_Selected, hv_MaxRatio, out hv_Less);
                hv_Selected1.Dispose();
                HOperatorSet.TupleSelectMask(hv_Selected, hv_Less, out hv_Selected1);
                hv_Indices.Dispose();
                HOperatorSet.TupleFind(hv_ratio, hv_Selected1, out hv_Indices);
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    ho_ObjectSelected.Dispose();
                    HOperatorSet.SelectObj(ho_SelectedRegions, out ho_ObjectSelected, hv_Indices + 1);
                }
                hv_Row.Dispose();
                hv_Column.Dispose();
                hv_Phi.Dispose();
                hv_Length1.Dispose();
                hv_Length2.Dispose();
                hv_ratio.Dispose();
                hv_MinRatio.Dispose();
                hv_MaxRatio.Dispose();
                hv_Greater.Dispose();
                hv_Selected.Dispose();
                hv_Less.Dispose();
                hv_Selected1.Dispose();
                hv_Indices.Dispose();

                ho_RegionUnion.Dispose();
                HOperatorSet.Union1(ho_ObjectSelected, out ho_RegionUnion);
                ho_ObjectSelected.Dispose();
                ho_ImageReduced.Dispose();
                HOperatorSet.ReduceDomain(ho_Image, ho_RegionUnion, out ho_ImageReduced);
                ho_RegionBorder.Dispose();
                HOperatorSet.Boundary(ho_ImageReduced, out ho_RegionBorder, "inner");

                hw.ClearWindow();
                //hw.HobjectToHimage(ho_Image);
                hw.viewWindow.displayHobject(ho_RegionBorder, "green", true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                ho_RegionBorder = null;
            }
            return ho_RegionBorder;
        }
        #endregion

        #region DilationRectangle1Test
        public HObject DilationRectangle1Test(HWindow_Final hw, HObject ho_Image, HObject ho_RegionBorder, float DilationRectangleWidth, float DilationRectangleHeight)
        {
            HObject ho_RegionDilation = new HObject();
            HObject ho_ImageReduced1 = new HObject();
            try
            {
                ho_RegionDilation.Dispose();
                //参数需要调试
                HOperatorSet.DilationRectangle1(ho_RegionBorder, out ho_RegionDilation, DilationRectangleWidth, DilationRectangleHeight);
                ho_ImageReduced1.Dispose();
                HOperatorSet.ReduceDomain(ho_Image, ho_RegionDilation, out ho_ImageReduced1);

                hw.ClearWindow();
                //hw.HobjectToHimage(ho_Image);
                hw.HobjectToHimage(ho_ImageReduced1);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                ho_ImageReduced1 = null;
            }
            return ho_ImageReduced1;
        }
        #endregion

        #region EdgesSubPixTest
        public HObject EdgesSubPixTest(HWindow_Final hw, HObject ho_Image, HObject ho_ImageReduced1, float EdgesSubPixCannyAlpha, float EdgesSubPixCannyLow, float EdgesSubPixCannyHigh)
        {
            HObject ho_Edges = new HObject();

            try
            {
                ho_Edges.Dispose();
                //参数需要调试
                HOperatorSet.EdgesSubPix(ho_ImageReduced1, out ho_Edges, "canny", EdgesSubPixCannyAlpha, EdgesSubPixCannyLow, EdgesSubPixCannyHigh);

                hw.ClearWindow();
                hw.HobjectToHimage(ho_Image);
                hw.viewWindow.displayHobject(ho_Edges, "green", true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                ho_Edges = null;
            }
            return ho_Edges;
        }
        #endregion

        #region UnionCollinearContoursXldTest
        public HObject UnionCollinearContoursXld(HWindow_Final hw, HObject ho_Image, HObject ho_Edges, float UnionCollinearContoursXldMaxDistAbs, float UnionCollinearContoursXldMaxDistRel, float UnionCollinearContoursXldMaxShift, float UnionCollinearContoursXldMaxAngle)
        {
            HObject ho_Edges2 = new HObject();
            try
            {
                ho_Edges2.Dispose();
                //参数需要调试
                HOperatorSet.UnionCollinearContoursXld(ho_Edges, out ho_Edges2, UnionCollinearContoursXldMaxDistAbs, UnionCollinearContoursXldMaxDistRel, UnionCollinearContoursXldMaxShift, UnionCollinearContoursXldMaxAngle, "attr_keep");

                hw.ClearWindow();
                hw.HobjectToHimage(ho_Image);
                hw.viewWindow.displayHobject(ho_Edges2, "red", true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                ho_Edges = null;
            }
            return ho_Edges2;
        }
        #endregion

        #region SelectShapeXld_ContlengthTest
        public HObject SelectShapeXld_ContlengthTest(HWindow_Final hw, HObject ho_Image, HObject ho_Edges2, float XldContlengthMin, float XldContlengthMax)
        {
            HObject ho_RectangleEdges = new HObject();
            try
            {
                ho_RectangleEdges.Dispose();
                //参数需要调试
                HOperatorSet.SelectShapeXld(ho_Edges2, out ho_RectangleEdges, "contlength", "and", XldContlengthMin, XldContlengthMax);

                hw.ClearWindow();
                //hw.HobjectToHimage(ho_Image);
                hw.viewWindow.displayHobject(ho_RectangleEdges, "green", true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                ho_RectangleEdges = null;
            }
            return ho_RectangleEdges;
        }
        #endregion

        #region SelectShapeXld_AreaAndRectangularityTest
        public HObject SelectShapeXld_AreaAndRectangularityTest(HWindow_Final hw, HObject ho_Image, HObject ho_RectangleEdges, float XLDMinArea, float XLDMaxArea, float XLDMinRectangularity, float XLDMaxRectangularity)
        {
            HObject ho_RectangleEdges2 = new HObject();
            try
            {
                ho_RectangleEdges2.Dispose();
                //参数需要调试
                HOperatorSet.SelectShapeXld(ho_RectangleEdges, out ho_RectangleEdges2, (new HTuple("area")).TupleConcat("rectangularity"), "and", (new HTuple(XLDMinArea)).TupleConcat(XLDMinRectangularity), (new HTuple(XLDMaxArea)).TupleConcat(XLDMaxRectangularity));
                //HOperatorSet.SelectShapeXld(ho_RectangleEdges, out ho_RectangleEdges2, ((new HTuple("area")).TupleConcat("rectangularity")).TupleConcat("circularity"), "and", ((new HTuple(XLDMinArea)).TupleConcat(XLDMinRectangularity)).TupleConcat(0.1), ((new HTuple(XLDMaxArea)).TupleConcat(XLDMaxRectangularity)).TupleConcat(1));

                hw.ClearWindow();
                //hw.HobjectToHimage(ho_Image);
                hw.viewWindow.displayHobject(ho_RectangleEdges2, "yellow", true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                ho_RectangleEdges2 = null;
            }
            return ho_RectangleEdges2;
        }
        #endregion

        #region UnionAdjacentContoursXldTest
        public HObject UnionAdjacentContoursXldTest(HWindow_Final hw, HObject ho_Image, HObject ho_RectangleEdges2, float UnionAdjacentContoursXldMaxDistAbs, float UnionAdjacentContoursXldMaxDistRel)
        {
            HObject ho_RectangleEdges3 = new HObject();
            try
            {
                ho_RectangleEdges3.Dispose();
                //参数需要调试
                HOperatorSet.UnionAdjacentContoursXld(ho_RectangleEdges2, out ho_RectangleEdges3, UnionAdjacentContoursXldMaxDistAbs, UnionAdjacentContoursXldMaxDistRel, "attr_keep");

                hw.ClearWindow();
                //hw.HobjectToHimage(ho_Image);
                hw.viewWindow.displayHobject(ho_RectangleEdges3, "red", true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                ho_RectangleEdges3 = null;
            }
            return ho_RectangleEdges3;
        }
        #endregion

        #region FitRectangle2ContourXldTest
        public HObject FitRectangle2ContourXldTest(HWindow_Final hw, HObject ho_Image, HObject ho_RectangleEdges3)
        {
            HTuple hv_Row;
            HTuple hv_Column;
            HTuple hv_Phi;
            HTuple hv_Length1;
            HTuple hv_Length2;
            HTuple hv_PointOrde;
            HTuple hv_PointOrder;
            HObject ho_Rectangle = new HObject();
            try
            {
                HOperatorSet.FitRectangle2ContourXld(ho_RectangleEdges3, "tukey", -1, 0, 0, 3, 2, out hv_Row, out hv_Column, out hv_Phi, out hv_Length1, out hv_Length2, out hv_PointOrder);
                ho_Rectangle.Dispose();
                HOperatorSet.GenRectangle2ContourXld(out ho_Rectangle, hv_Row, hv_Column, hv_Phi, hv_Length1, hv_Length2);

                hw.ClearWindow();
                hw.HobjectToHimage(ho_Image);
                hw.viewWindow.displayHobject(ho_Rectangle, "green", true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                ho_Rectangle = null;
            }
            return ho_Rectangle;
        }
        #endregion

        #region 矩形数据显示在图片上
        /// <summary>
        /// rectangleDataDisplay:矩形数据显示在图片上
        /// 中心点Row坐标
        /// 中心点Column坐标
        /// 角度(宽度方向平行于水平轴为0度,逆时钟为正方向)
        /// 宽度
        /// 高度
        /// 第1个顶点的Row坐标(右上角为第一个顶点,按逆时钟排序)
        /// 第2个顶点的Row坐标
        /// 第3个顶点的Row坐标
        /// 第4个顶点的Row坐标
        /// 第1个顶点的Column坐标
        /// 第2个顶点的Column坐标
        /// 第3个顶点的Column坐标
        /// 第4个顶点的Column坐标
        /// </summary>
        /// <param name="hWindowControl">HWindowControl,图片显示控件</param>
        /// <param name="_MERData">float[],最小外接矩形数据</param>
        /// <param name="_9PCalibrateMatrixFilePath">string,9点标定文件地址</param>
        /// <param name="_ROBCoordinateDisplay">bool,是否显示机械坐标(true显示)</param>
        /// <returns>bool:成功则返回true</returns>
        public bool rectangleDataDisplay(HWindow_Final hw, HObject ho_image, float[] _MERData, string _9PCalibrateMatrixFilePath, bool _ROBCoordinateDisplay)
        {
            HTuple hv_String1 = new HTuple();
            HTuple hv_String2 = new HTuple();
            HTuple hv_Deg = new HTuple();
            HTuple hv_Phi = new HTuple();
            HTuple hv_DegString = new HTuple();
            HTuple hv_Row1 = new HTuple();
            HTuple hv_Col1 = new HTuple();
            HTuple hv_Width1 = new HTuple();
            HTuple hv_Height1 = new HTuple();
            HObject cross = new HObject();

            RobCamCalibration newRobCamCalibration;
            float[] _XTransPos;
            float[] _YTransPos;

            try
            {
                hw.ClearWindow();
                hw.HobjectToHimage(ho_image);
                set_display_font(hw.hWindowControl.HalconWindow, 14, "mono", "false", "false");
                hv_String1.Dispose();
                HOperatorSet.TupleString(_MERData[0], "0", out hv_String1);
                hv_String2.Dispose();
                HOperatorSet.TupleString(_MERData[1], "0", out hv_String2);
                hv_Deg.Dispose();
                hv_Deg = _MERData[2];
                hv_Phi.Dispose();
                HOperatorSet.TupleRad(hv_Deg, out hv_Phi);
                HObject ho_Rectangle = new HObject();

                //绘制并显示cross
                HOperatorSet.GenCrossContourXld(out cross, _MERData[0], _MERData[1], 500, hv_Phi);
                hw.viewWindow.displayHobject(cross, "green");
                //ADD 202210141447
                HObject ho_Arrow = new HObject();
                gen_arrow_contour_xld(out ho_Arrow, _MERData[0], _MERData[1], _MERData[0] + 300 * Math.Sin(((hv_Deg + 90) * -1 / 180f) * Math.PI), _MERData[1] + 300 * Math.Cos(((hv_Deg + 90) * -1 / 180f) * Math.PI), 50, 50);
                hw.viewWindow.displayHobject(ho_Arrow, "red");

                hv_DegString.Dispose();
                HOperatorSet.TupleString(hv_Deg, "0", out hv_DegString);

                HOperatorSet.GenRectangle2ContourXld(out ho_Rectangle, _MERData[0], _MERData[1], hv_Phi, _MERData[3] / 2, _MERData[4] / 2);

                hw.viewWindow.displayHobject(ho_Rectangle, "green");

                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    //disp_message_Halcon(hWindowControl.HalconWindow, ((((("[" + hv_String1) + new HTuple(",")) + hv_String2) + new HTuple(",")) + hv_DegString) + "]",
                    //     "image", _MERData[0], _MERData[1] + 100, "black", "true");
                    //HOperatorSet.DispText(hWindowControl.HalconWindow, ((((("[" + hv_String1) + new HTuple(",")) + hv_String2) + new HTuple(",")) + hv_DegString) + "]", "image", _MERData[0], _MERData[1] + 100, "green", "box", "false");
                    hw.viewWindow.dispMessage(((((("[" + hv_String1) + new HTuple(",")) + hv_String2) + new HTuple(",")) + hv_DegString) + "]", "green", _MERData[0], _MERData[1] + 100, "false");
                }
                hv_Row1.Dispose(); hv_Col1.Dispose();
                HOperatorSet.GetContourXld(ho_Rectangle, out hv_Row1, out hv_Col1);

                _XTransPos = new float[hv_Row1.TupleLength()];
                _YTransPos = new float[hv_Col1.TupleLength()];
                if (_ROBCoordinateDisplay)
                {
                    newRobCamCalibration = new RobCamCalibration();
                    newRobCamCalibration.coordTransform(newRobCamCalibration.readDateRobAndCamCalib_9P(_9PCalibrateMatrixFilePath), hv_Row1.ToFArr(), hv_Col1.ToFArr(), ref _XTransPos, ref _YTransPos);
                }

                //绘制并显示cross
                HOperatorSet.GenCrossContourXld(out cross, hv_Row1, hv_Col1, 500, hv_Phi);
                hw.viewWindow.displayHobject(cross, "green");

                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_String1.Dispose();
                    HOperatorSet.TupleString(hv_Row1.TupleSelect(0), "0", out hv_String1);
                    if (_ROBCoordinateDisplay)
                        hv_String1 = $"{hv_String1.S}[{_XTransPos[0].ToString("0.000")}]\r\n";
                }
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_String2.Dispose();
                    HOperatorSet.TupleString(hv_Col1.TupleSelect(0), "0", out hv_String2);
                    if (_ROBCoordinateDisplay)
                        hv_String2 = $"{hv_String2.S}[{_YTransPos[0].ToString("0.000")}]\r\n";
                }
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    //disp_message_Halcon(hWindowControl.HalconWindow, ((("[" + hv_String1) + new HTuple(",")) + hv_String2) + "]",
                    //    "image", hv_Row1.TupleSelect(0), hv_Col1.TupleSelect(0), "black", "true");
                    //HOperatorSet.DispText(hWindowControl.HalconWindow, ((("[" + hv_String1) + new HTuple(",")) + hv_String2) + "]", "image", hv_Row1.TupleSelect(0), hv_Col1.TupleSelect(0), "green", "box", "false");
                    hw.viewWindow.dispMessage(((("[" + hv_String1) + new HTuple(",")) + hv_String2) + "]", "green", hv_Row1.TupleSelect(0), hv_Col1.TupleSelect(0), "false");
                }
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_String1.Dispose();
                    HOperatorSet.TupleString(hv_Row1.TupleSelect(1), "0", out hv_String1);
                    if (_ROBCoordinateDisplay)
                        hv_String1 = $"{hv_String1.S}[{_XTransPos[1].ToString("0.000")}]\r\n";
                }
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_String2.Dispose();
                    HOperatorSet.TupleString(hv_Col1.TupleSelect(1), "0", out hv_String2);
                    if (_ROBCoordinateDisplay)
                        hv_String2 = $"{hv_String2.S}[{_YTransPos[1].ToString("0.000")}]\r\n";
                }
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    //disp_message_Halcon(hWindowControl.HalconWindow, ((("[" + hv_String1) + new HTuple(",")) + hv_String2) + "]",
                    //    "image", hv_Row1.TupleSelect(1), hv_Col1.TupleSelect(1), "black", "true");
                    //HOperatorSet.DispText(hWindowControl.HalconWindow, ((("[" + hv_String1) + new HTuple(",")) + hv_String2) + "]", "image", hv_Row1.TupleSelect(1), hv_Col1.TupleSelect(1), "green", "box", "false");
                    hw.viewWindow.dispMessage(((("[" + hv_String1) + new HTuple(",")) + hv_String2) + "]", "green", hv_Row1.TupleSelect(1), hv_Col1.TupleSelect(1), "false");
                }
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_String1.Dispose();
                    HOperatorSet.TupleString(hv_Row1.TupleSelect(2), "0", out hv_String1);
                    if (_ROBCoordinateDisplay)
                        hv_String1 = $"{hv_String1.S}[{_XTransPos[2].ToString("0.000")}]\r\n";
                }
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_String2.Dispose();
                    HOperatorSet.TupleString(hv_Col1.TupleSelect(2), "0", out hv_String2);
                    if (_ROBCoordinateDisplay)
                        hv_String2 = $"{hv_String2.S}[{_YTransPos[2].ToString("0.000")}]\r\n";
                }
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    //disp_message_Halcon(hWindowControl.HalconWindow, ((("[" + hv_String1) + new HTuple(",")) + hv_String2) + "]",
                    //    "image", hv_Row1.TupleSelect(2), hv_Col1.TupleSelect(2), "black", "true");
                    //HOperatorSet.DispText(hWindowControl.HalconWindow, ((("[" + hv_String1) + new HTuple(",")) + hv_String2) + "]", "image", hv_Row1.TupleSelect(2), hv_Col1.TupleSelect(2), "green", "box", "false");
                    hw.viewWindow.dispMessage(((("[" + hv_String1) + new HTuple(",")) + hv_String2) + "]", "green", hv_Row1.TupleSelect(2), hv_Col1.TupleSelect(2) - 200, "false");
                }
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_String1.Dispose();
                    HOperatorSet.TupleString(hv_Row1.TupleSelect(3), "0", out hv_String1);
                    if (_ROBCoordinateDisplay)
                        hv_String1 = $"{hv_String1.S}[{_XTransPos[3].ToString("0.000")}]\r\n";
                }
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_String2.Dispose();
                    HOperatorSet.TupleString(hv_Col1.TupleSelect(3), "0", out hv_String2);
                    if (_ROBCoordinateDisplay)
                        hv_String2 = $"{hv_String2.S}[{_YTransPos[3].ToString("0.000")}]\r\n";
                }
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    //disp_message_Halcon(hWindowControl.HalconWindow, ((("[" + hv_String1) + new HTuple(",")) + hv_String2) + "]",
                    //    "image", hv_Row1.TupleSelect(3), hv_Col1.TupleSelect(3), "black", "true");
                    //HOperatorSet.DispText(hWindowControl.HalconWindow, ((("[" + hv_String1) + new HTuple(",")) + hv_String2) + "]", "image", hv_Row1.TupleSelect(3), hv_Col1.TupleSelect(3), "green", "box", "false");
                    hw.viewWindow.dispMessage(((("[" + hv_String1) + new HTuple(",")) + hv_String2) + "]", "green", hv_Row1.TupleSelect(3), hv_Col1.TupleSelect(3) - 200, "false");
                }

                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_String1.Dispose();
                    HOperatorSet.TupleString(_MERData[3], "0", out hv_String1);
                }
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    //disp_message_Halcon(hWindowControl.HalconWindow, hv_String1, "image", (hv_Row1.TupleSelect(0)) + 100,
                    //    ((hv_Col1.TupleSelect(0)) + (hv_Col1.TupleSelect(1))) / 2, "black", "true");
                    //HOperatorSet.DispText(hWindowControl.HalconWindow, hv_String1, "image", (hv_Row1.TupleSelect(0)) + 100,((hv_Col1.TupleSelect(0)) + (hv_Col1.TupleSelect(1))) / 2, "green", "box", "false");
                    hw.viewWindow.dispMessage(hv_String1, "green", (hv_Row1.TupleSelect(0)) + 100, ((hv_Col1.TupleSelect(0)) + (hv_Col1.TupleSelect(1))) / 2, "false");
                }
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_String1.Dispose();
                    HOperatorSet.TupleString(_MERData[4], "0", out hv_String1);
                }
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    //disp_message_Halcon(hWindowControl.HalconWindow, hv_String1, "image", ((hv_Row1.TupleSelect(1)) + (hv_Row1.TupleSelect(
                    //    2))) / 2, (hv_Col1.TupleSelect(1)) + 100, "black", "true");
                    //HOperatorSet.DispText(hWindowControl.HalconWindow, hv_String1, "image", ((hv_Row1.TupleSelect(1)) + (hv_Row1.TupleSelect(2))) / 2, (hv_Col1.TupleSelect(1)) + 100, "green", "box", "false");
                    hw.viewWindow.dispMessage(hv_String1, "green", ((hv_Row1.TupleSelect(1)) + (hv_Row1.TupleSelect(2))) / 2, (hv_Col1.TupleSelect(1)) + 100, "false");
                }

                //using (HDevDisposeHelper dh = new HDevDisposeHelper())
                //{
                //    hv_Width1.Dispose();
                //    HOperatorSet.DistancePp(hv_Row1.TupleSelect(0), hv_Col1.TupleSelect(0), hv_Row1.TupleSelect(
                //        1), hv_Col1.TupleSelect(1), out hv_Width1);
                //}
                //using (HDevDisposeHelper dh = new HDevDisposeHelper())
                //{
                //    hv_Height1.Dispose();
                //    HOperatorSet.DistancePp(hv_Row1.TupleSelect(1), hv_Col1.TupleSelect(1), hv_Row1.TupleSelect(
                //        2), hv_Col1.TupleSelect(2), out hv_Height1);
                //}

                hv_String1.Dispose();
                hv_String2.Dispose();
                hv_Deg.Dispose();
                hv_Phi.Dispose();
                hv_DegString.Dispose();
                hv_Row1.Dispose();
                hv_Col1.Dispose();
                hv_Width1.Dispose();
                hv_Height1.Dispose();
                cross.Dispose();
            }
            catch (Exception ex)
            {
                hv_String1.Dispose();
                hv_String2.Dispose();
                hv_Deg.Dispose();
                hv_Phi.Dispose();
                hv_DegString.Dispose();
                hv_Row1.Dispose();
                hv_Col1.Dispose();
                hv_Width1.Dispose();
                hv_Height1.Dispose();
                cross.Dispose();
                return false;
            }
            return true;
        }

        #endregion

        #region set_display_font
        // Chapter: Graphics / Text
        // Short Description: Set font independent of OS 
        public void set_display_font(HTuple hv_WindowHandle, HTuple hv_Size, HTuple hv_Font,
            HTuple hv_Bold, HTuple hv_Slant)
        {



            // Local iconic variables 

            // Local control variables 

            HTuple hv_OS = new HTuple(), hv_Fonts = new HTuple();
            HTuple hv_Style = new HTuple(), hv_Exception = new HTuple();
            HTuple hv_AvailableFonts = new HTuple(), hv_Fdx = new HTuple();
            HTuple hv_Indices = new HTuple();
            HTuple hv_Font_COPY_INP_TMP = new HTuple(hv_Font);
            HTuple hv_Size_COPY_INP_TMP = new HTuple(hv_Size);

            // Initialize local and output iconic variables 
            try
            {
                //This procedure sets the text font of the current window with
                //the specified attributes.
                //
                //Input parameters:
                //WindowHandle: The graphics window for which the font will be set
                //Size: The font size. If Size=-1, the default of 16 is used.
                //Bold: If set to 'true', a bold font is used
                //Slant: If set to 'true', a slanted font is used
                //
                hv_OS.Dispose();
                HOperatorSet.GetSystem("operating_system", out hv_OS);
                if ((int)((new HTuple(hv_Size_COPY_INP_TMP.TupleEqual(new HTuple()))).TupleOr(
                    new HTuple(hv_Size_COPY_INP_TMP.TupleEqual(-1)))) != 0)
                {
                    hv_Size_COPY_INP_TMP.Dispose();
                    hv_Size_COPY_INP_TMP = 16;
                }
                if ((int)(new HTuple(((hv_OS.TupleSubstr(0, 2))).TupleEqual("Win"))) != 0)
                {
                    //Restore previous behaviour
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_Size = ((1.13677 * hv_Size_COPY_INP_TMP)).TupleInt()
                                ;
                            hv_Size_COPY_INP_TMP.Dispose();
                            hv_Size_COPY_INP_TMP = ExpTmpLocalVar_Size;
                        }
                    }
                }
                else
                {
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_Size = hv_Size_COPY_INP_TMP.TupleInt()
                                ;
                            hv_Size_COPY_INP_TMP.Dispose();
                            hv_Size_COPY_INP_TMP = ExpTmpLocalVar_Size;
                        }
                    }
                }
                if ((int)(new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("Courier"))) != 0)
                {
                    hv_Fonts.Dispose();
                    hv_Fonts = new HTuple();
                    hv_Fonts[0] = "Courier";
                    hv_Fonts[1] = "Courier 10 Pitch";
                    hv_Fonts[2] = "Courier New";
                    hv_Fonts[3] = "CourierNew";
                    hv_Fonts[4] = "Liberation Mono";
                }
                else if ((int)(new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("mono"))) != 0)
                {
                    hv_Fonts.Dispose();
                    hv_Fonts = new HTuple();
                    hv_Fonts[0] = "Consolas";
                    hv_Fonts[1] = "Menlo";
                    hv_Fonts[2] = "Courier";
                    hv_Fonts[3] = "Courier 10 Pitch";
                    hv_Fonts[4] = "FreeMono";
                    hv_Fonts[5] = "Liberation Mono";
                }
                else if ((int)(new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("sans"))) != 0)
                {
                    hv_Fonts.Dispose();
                    hv_Fonts = new HTuple();
                    hv_Fonts[0] = "Luxi Sans";
                    hv_Fonts[1] = "DejaVu Sans";
                    hv_Fonts[2] = "FreeSans";
                    hv_Fonts[3] = "Arial";
                    hv_Fonts[4] = "Liberation Sans";
                }
                else if ((int)(new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("serif"))) != 0)
                {
                    hv_Fonts.Dispose();
                    hv_Fonts = new HTuple();
                    hv_Fonts[0] = "Times New Roman";
                    hv_Fonts[1] = "Luxi Serif";
                    hv_Fonts[2] = "DejaVu Serif";
                    hv_Fonts[3] = "FreeSerif";
                    hv_Fonts[4] = "Utopia";
                    hv_Fonts[5] = "Liberation Serif";
                }
                else
                {
                    hv_Fonts.Dispose();
                    hv_Fonts = new HTuple(hv_Font_COPY_INP_TMP);
                }
                hv_Style.Dispose();
                hv_Style = "";
                if ((int)(new HTuple(hv_Bold.TupleEqual("true"))) != 0)
                {
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_Style = hv_Style + "Bold";
                            hv_Style.Dispose();
                            hv_Style = ExpTmpLocalVar_Style;
                        }
                    }
                }
                else if ((int)(new HTuple(hv_Bold.TupleNotEqual("false"))) != 0)
                {
                    hv_Exception.Dispose();
                    hv_Exception = "Wrong value of control parameter Bold";
                    throw new HalconException(hv_Exception);
                }
                if ((int)(new HTuple(hv_Slant.TupleEqual("true"))) != 0)
                {
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_Style = hv_Style + "Italic";
                            hv_Style.Dispose();
                            hv_Style = ExpTmpLocalVar_Style;
                        }
                    }
                }
                else if ((int)(new HTuple(hv_Slant.TupleNotEqual("false"))) != 0)
                {
                    hv_Exception.Dispose();
                    hv_Exception = "Wrong value of control parameter Slant";
                    throw new HalconException(hv_Exception);
                }
                if ((int)(new HTuple(hv_Style.TupleEqual(""))) != 0)
                {
                    hv_Style.Dispose();
                    hv_Style = "Normal";
                }
                hv_AvailableFonts.Dispose();
                HOperatorSet.QueryFont(hv_WindowHandle, out hv_AvailableFonts);
                hv_Font_COPY_INP_TMP.Dispose();
                hv_Font_COPY_INP_TMP = "";
                for (hv_Fdx = 0; (int)hv_Fdx <= (int)((new HTuple(hv_Fonts.TupleLength())) - 1); hv_Fdx = (int)hv_Fdx + 1)
                {
                    hv_Indices.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Indices = hv_AvailableFonts.TupleFind(
                            hv_Fonts.TupleSelect(hv_Fdx));
                    }
                    if ((int)(new HTuple((new HTuple(hv_Indices.TupleLength())).TupleGreater(
                        0))) != 0)
                    {
                        if ((int)(new HTuple(((hv_Indices.TupleSelect(0))).TupleGreaterEqual(0))) != 0)
                        {
                            hv_Font_COPY_INP_TMP.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_Font_COPY_INP_TMP = hv_Fonts.TupleSelect(
                                    hv_Fdx);
                            }
                            break;
                        }
                    }
                }
                if ((int)(new HTuple(hv_Font_COPY_INP_TMP.TupleEqual(""))) != 0)
                {
                    throw new HalconException("Wrong value of control parameter Font");
                }
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    {
                        HTuple
                          ExpTmpLocalVar_Font = (((hv_Font_COPY_INP_TMP + "-") + hv_Style) + "-") + hv_Size_COPY_INP_TMP;
                        hv_Font_COPY_INP_TMP.Dispose();
                        hv_Font_COPY_INP_TMP = ExpTmpLocalVar_Font;
                    }
                }
                HOperatorSet.SetFont(hv_WindowHandle, hv_Font_COPY_INP_TMP);

                hv_Font_COPY_INP_TMP.Dispose();
                hv_Size_COPY_INP_TMP.Dispose();
                hv_OS.Dispose();
                hv_Fonts.Dispose();
                hv_Style.Dispose();
                hv_Exception.Dispose();
                hv_AvailableFonts.Dispose();
                hv_Fdx.Dispose();
                hv_Indices.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {

                hv_Font_COPY_INP_TMP.Dispose();
                hv_Size_COPY_INP_TMP.Dispose();
                hv_OS.Dispose();
                hv_Fonts.Dispose();
                hv_Style.Dispose();
                hv_Exception.Dispose();
                hv_AvailableFonts.Dispose();
                hv_Fdx.Dispose();
                hv_Indices.Dispose();

                throw HDevExpDefaultException;
            }

        }
        #endregion

        #region disp_message_Halcon:Halcon消息显示
        /// <summary>
        /// disp_message_Halcon:Halcon消息显示
        /// </summary>
        /// <param name="hv_WindowHandle">HTuple:要显示消息的图像窗口的句柄</param>
        /// <param name="hv_String">HTuple:要显示的消息</param>
        /// <param name="hv_CoordSystem">HTuple:要显示消息的坐标系,image,window</param>
        /// <param name="hv_Row">HTuple:</param>
        /// <param name="hv_Column">HTuple:</param>
        /// <param name="hv_Color">HTuple:</param>
        /// <param name="hv_Box">HTuple:</param>
        /// <returns></returns>
        //例如:
        //hv_Information[0] = "拟合圆失败!失败在第1步!";                  
        //hv_Information[1] = "";            
        //hv_Information[2] = "";          
        //disp_message_Halcon(WindowHandle_CircleInspection, hv_Information, "image", 0, 0, "red", "false");
        //disp_message_Halcon(posInspection.WH, "未能查找到边缘，请采集或读取新图片重试", "image", 0, 0, "red", "true");
        //'black', 'blue', 'yellow', 'red', 'green', 'cyan', 'magenta', 'forest green', 'lime green', 'coral', 'slate blue'
        public void disp_message_Halcon(HTuple hv_WindowHandle, HTuple hv_String,
            HTuple hv_CoordSystem, HTuple hv_Row, HTuple hv_Column, HTuple hv_Color, HTuple hv_Box)
        {
            // Local iconic variables 
            // Local control variables 
            HTuple hv_GenParamName = new HTuple(), hv_GenParamValue = new HTuple();
            HTuple hv_Color_COPY_INP_TMP = new HTuple(hv_Color);
            HTuple hv_Column_COPY_INP_TMP = new HTuple(hv_Column);
            HTuple hv_CoordSystem_COPY_INP_TMP = new HTuple(hv_CoordSystem);
            HTuple hv_Row_COPY_INP_TMP = new HTuple(hv_Row);
            // Initialize local and output iconic variables 
            try
            {
                if ((int)((new HTuple(hv_Row_COPY_INP_TMP.TupleEqual(new HTuple()))).TupleOr(
                    new HTuple(hv_Column_COPY_INP_TMP.TupleEqual(new HTuple())))) != 0)
                {
                    hv_Color_COPY_INP_TMP.Dispose();
                    hv_Column_COPY_INP_TMP.Dispose();
                    hv_CoordSystem_COPY_INP_TMP.Dispose();
                    hv_Row_COPY_INP_TMP.Dispose();
                    hv_GenParamName.Dispose();
                    hv_GenParamValue.Dispose();
                    return;
                }
                if ((int)(new HTuple(hv_Row_COPY_INP_TMP.TupleEqual(-1))) != 0)
                {
                    hv_Row_COPY_INP_TMP.Dispose();
                    hv_Row_COPY_INP_TMP = 12;
                }
                if ((int)(new HTuple(hv_Column_COPY_INP_TMP.TupleEqual(-1))) != 0)
                {
                    hv_Column_COPY_INP_TMP.Dispose();
                    hv_Column_COPY_INP_TMP = 12;
                }
                //Convert the parameter Box to generic parameters.
                hv_GenParamName.Dispose();
                hv_GenParamName = new HTuple();
                hv_GenParamValue.Dispose();
                hv_GenParamValue = new HTuple();
                if ((int)(new HTuple((new HTuple(hv_Box.TupleLength())).TupleGreater(0))) != 0)
                {
                    if ((int)(new HTuple(((hv_Box.TupleSelect(0))).TupleEqual("false"))) != 0)
                    {
                        //Display no box
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_GenParamName = hv_GenParamName.TupleConcat(
                                    "box");
                                hv_GenParamName.Dispose();
                                hv_GenParamName = ExpTmpLocalVar_GenParamName;
                            }
                        }
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_GenParamValue = hv_GenParamValue.TupleConcat(
                                    "false");
                                hv_GenParamValue.Dispose();
                                hv_GenParamValue = ExpTmpLocalVar_GenParamValue;
                            }
                        }
                    }
                    else if ((int)(new HTuple(((hv_Box.TupleSelect(0))).TupleNotEqual(
                        "true"))) != 0)
                    {
                        //Set a color other than the default.
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_GenParamName = hv_GenParamName.TupleConcat(
                                    "box_color");
                                hv_GenParamName.Dispose();
                                hv_GenParamName = ExpTmpLocalVar_GenParamName;
                            }
                        }
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_GenParamValue = hv_GenParamValue.TupleConcat(
                                    hv_Box.TupleSelect(0));
                                hv_GenParamValue.Dispose();
                                hv_GenParamValue = ExpTmpLocalVar_GenParamValue;
                            }
                        }
                    }
                }
                if ((int)(new HTuple((new HTuple(hv_Box.TupleLength())).TupleGreater(1))) != 0)
                {
                    if ((int)(new HTuple(((hv_Box.TupleSelect(1))).TupleEqual("false"))) != 0)
                    {
                        //Display no shadow.
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_GenParamName = hv_GenParamName.TupleConcat(
                                    "shadow");
                                hv_GenParamName.Dispose();
                                hv_GenParamName = ExpTmpLocalVar_GenParamName;
                            }
                        }
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_GenParamValue = hv_GenParamValue.TupleConcat(
                                    "false");
                                hv_GenParamValue.Dispose();
                                hv_GenParamValue = ExpTmpLocalVar_GenParamValue;
                            }
                        }
                    }
                    else if ((int)(new HTuple(((hv_Box.TupleSelect(1))).TupleNotEqual(
                        "true"))) != 0)
                    {
                        //Set a shadow color other than the default.
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_GenParamName = hv_GenParamName.TupleConcat(
                                    "shadow_color");
                                hv_GenParamName.Dispose();
                                hv_GenParamName = ExpTmpLocalVar_GenParamName;
                            }
                        }
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_GenParamValue = hv_GenParamValue.TupleConcat(
                                    hv_Box.TupleSelect(1));
                                hv_GenParamValue.Dispose();
                                hv_GenParamValue = ExpTmpLocalVar_GenParamValue;
                            }
                        }
                    }
                }
                //Restore default CoordSystem behavior.
                if ((int)(new HTuple(hv_CoordSystem_COPY_INP_TMP.TupleNotEqual("window"))) != 0)
                {
                    hv_CoordSystem_COPY_INP_TMP.Dispose();
                    hv_CoordSystem_COPY_INP_TMP = "image";
                }
                //
                if ((int)(new HTuple(hv_Color_COPY_INP_TMP.TupleEqual(""))) != 0)
                {
                    //disp_text does not accept an empty string for Color.
                    hv_Color_COPY_INP_TMP.Dispose();
                    hv_Color_COPY_INP_TMP = new HTuple();
                }
                //
                HOperatorSet.DispText(hv_WindowHandle, hv_String, hv_CoordSystem_COPY_INP_TMP,
                    hv_Row_COPY_INP_TMP, hv_Column_COPY_INP_TMP, hv_Color_COPY_INP_TMP, hv_GenParamName,
                    hv_GenParamValue);

                hv_Color_COPY_INP_TMP.Dispose();
                hv_Column_COPY_INP_TMP.Dispose();
                hv_CoordSystem_COPY_INP_TMP.Dispose();
                hv_Row_COPY_INP_TMP.Dispose();
                hv_GenParamName.Dispose();
                hv_GenParamValue.Dispose();
                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                hv_Color_COPY_INP_TMP.Dispose();
                hv_Column_COPY_INP_TMP.Dispose();
                hv_CoordSystem_COPY_INP_TMP.Dispose();
                hv_Row_COPY_INP_TMP.Dispose();
                hv_GenParamName.Dispose();
                hv_GenParamValue.Dispose();
                throw HDevExpDefaultException;
            }
        }
        #endregion

        #region imageRotation:图片旋转
        /// <summary>
        /// imageRotation:图片旋转
        /// </summary>
        /// <param name="ho_ImageInput">HObject,输入图片</param>
        /// <param name="_rotationDegree">float,旋转角度</param>
        /// <returns>HObject:旋转后的图片</returns>
        public HObject imageRotation(HObject ho_ImageInput, float _rotationDegree)
        {
            // Local iconic variables 
            HObject ho_Image1 = null, ho_ImageAffineTrans = null;
            HObject ho_Image11 = null, ho_Image2 = null, ho_Image3 = null;
            HObject ho_Image4 = null, ho_Image5 = null, ho_Image6 = null;
            HObject ho_ImageAffineTrans1 = null, ho_ImageAffineTrans2 = null;
            HObject ho_ImageAffineTrans3 = null, ho_MultiChannelImage = null;

            // Local control variables 
            HTuple hv_Width = new HTuple(), hv_Height = new HTuple();
            HTuple hv_minImageWidth = new HTuple(), hv_minImageHeight = new HTuple();
            HTuple hv_Angle = new HTuple(), hv_HomMat2D = new HTuple();
            HTuple hv_Channels = new HTuple();

            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Image1);
            HOperatorSet.GenEmptyObj(out ho_ImageAffineTrans);
            HOperatorSet.GenEmptyObj(out ho_Image11);
            HOperatorSet.GenEmptyObj(out ho_Image2);
            HOperatorSet.GenEmptyObj(out ho_Image3);
            HOperatorSet.GenEmptyObj(out ho_Image4);
            HOperatorSet.GenEmptyObj(out ho_Image5);
            HOperatorSet.GenEmptyObj(out ho_Image6);
            HOperatorSet.GenEmptyObj(out ho_ImageAffineTrans1);
            HOperatorSet.GenEmptyObj(out ho_ImageAffineTrans2);
            HOperatorSet.GenEmptyObj(out ho_ImageAffineTrans3);
            HOperatorSet.GenEmptyObj(out ho_MultiChannelImage);

            try
            {
                //获取图像宽和高
                hv_Width.Dispose(); hv_Height.Dispose();
                HOperatorSet.GetImageSize(ho_ImageInput, out hv_Width, out hv_Height);
                //旋转图像的最小外接正方形边长
                hv_minImageWidth.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_minImageWidth = (((hv_Width * hv_Width) + (hv_Height * hv_Height))).TupleSqrt();
                }
                hv_minImageHeight.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_minImageHeight = (((hv_Width * hv_Width) + (hv_Height * hv_Height))).TupleSqrt();
                }
                //设置旋转的角度
                hv_Angle.Dispose();
                hv_Angle = _rotationDegree;
                //建立旋转矩阵，这里将原图旋转15°
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_HomMat2D.Dispose();
                    HOperatorSet.VectorAngleToRigid(hv_Height / 2, hv_Width / 2, 0, hv_minImageHeight / 2, hv_minImageHeight / 2, hv_Angle.TupleRad(), out hv_HomMat2D);
                }
                //获取图像的通道数
                hv_Channels.Dispose();
                HOperatorSet.CountChannels(ho_ImageInput, out hv_Channels);
                //如果是单通道图像
                if ((int)(new HTuple(hv_Channels.TupleEqual(1))) != 0)
                {
                    //生成一个画布
                    ho_Image1.Dispose();
                    HOperatorSet.GenImageConst(out ho_Image1, "byte", hv_minImageWidth, hv_minImageHeight);
                    //将image放到画布上
                    HOperatorSet.OverpaintGray(ho_Image1, ho_ImageInput);
                    //图片旋转
                    ho_ImageAffineTrans.Dispose();
                    HOperatorSet.AffineTransImage(ho_Image1, out ho_ImageAffineTrans, hv_HomMat2D, "constant", "false");

                    ho_Image1.Dispose();
                    ho_Image11.Dispose();
                    ho_Image2.Dispose();
                    ho_Image3.Dispose();
                    ho_Image4.Dispose();
                    ho_Image5.Dispose();
                    ho_Image6.Dispose();
                    ho_ImageAffineTrans1.Dispose();
                    ho_ImageAffineTrans2.Dispose();
                    ho_ImageAffineTrans3.Dispose();
                    ho_MultiChannelImage.Dispose();
                    hv_Width.Dispose();
                    hv_Height.Dispose();
                    hv_minImageWidth.Dispose();
                    hv_minImageHeight.Dispose();
                    hv_Angle.Dispose();
                    hv_HomMat2D.Dispose();
                    hv_Channels.Dispose();
                    return ho_ImageAffineTrans;
                }
                //如果是三通道彩色图像
                else if ((int)(new HTuple(hv_Channels.TupleEqual(3))) != 0)
                {
                    //分解三通道图像
                    ho_Image11.Dispose(); ho_Image2.Dispose(); ho_Image3.Dispose();
                    HOperatorSet.Decompose3(ho_ImageInput, out ho_Image11, out ho_Image2, out ho_Image3
                        );
                    //生成三个画布
                    ho_Image4.Dispose();
                    HOperatorSet.GenImageConst(out ho_Image4, "byte", hv_minImageWidth, hv_minImageHeight);
                    ho_Image5.Dispose();
                    HOperatorSet.GenImageConst(out ho_Image5, "byte", hv_minImageWidth, hv_minImageHeight);
                    ho_Image6.Dispose();
                    HOperatorSet.GenImageConst(out ho_Image6, "byte", hv_minImageWidth, hv_minImageHeight);
                    //依次将三个单通道图像overpaint到画布上
                    HOperatorSet.OverpaintGray(ho_Image4, ho_Image11);
                    HOperatorSet.OverpaintGray(ho_Image5, ho_Image2);
                    HOperatorSet.OverpaintGray(ho_Image6, ho_Image3);
                    //仿射变换单通道图像
                    ho_ImageAffineTrans1.Dispose();
                    HOperatorSet.AffineTransImage(ho_Image4, out ho_ImageAffineTrans1, hv_HomMat2D, "constant", "false");
                    ho_ImageAffineTrans2.Dispose();
                    HOperatorSet.AffineTransImage(ho_Image5, out ho_ImageAffineTrans2, hv_HomMat2D, "constant", "false");
                    ho_ImageAffineTrans3.Dispose();
                    HOperatorSet.AffineTransImage(ho_Image6, out ho_ImageAffineTrans3, hv_HomMat2D, "constant", "false");
                    //三个单通道图像合成彩色图像
                    ho_MultiChannelImage.Dispose();
                    HOperatorSet.Compose3(ho_ImageAffineTrans1, ho_ImageAffineTrans2, ho_ImageAffineTrans3, out ho_MultiChannelImage);

                    ho_Image1.Dispose();
                    ho_ImageAffineTrans.Dispose();
                    ho_Image11.Dispose();
                    ho_Image2.Dispose();
                    ho_Image3.Dispose();
                    ho_Image4.Dispose();
                    ho_Image5.Dispose();
                    ho_Image6.Dispose();
                    ho_ImageAffineTrans1.Dispose();
                    ho_ImageAffineTrans2.Dispose();
                    ho_ImageAffineTrans3.Dispose();
                    hv_Width.Dispose();
                    hv_Height.Dispose();
                    hv_minImageWidth.Dispose();
                    hv_minImageHeight.Dispose();
                    hv_Angle.Dispose();
                    hv_HomMat2D.Dispose();
                    hv_Channels.Dispose();
                    return ho_MultiChannelImage;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());

                ho_Image1.Dispose();
                ho_ImageAffineTrans.Dispose();
                ho_Image11.Dispose();
                ho_Image2.Dispose();
                ho_Image3.Dispose();
                ho_Image4.Dispose();
                ho_Image5.Dispose();
                ho_Image6.Dispose();
                ho_ImageAffineTrans1.Dispose();
                ho_ImageAffineTrans2.Dispose();
                ho_ImageAffineTrans3.Dispose();
                ho_MultiChannelImage.Dispose();
                hv_Width.Dispose();
                hv_Height.Dispose();
                hv_minImageWidth.Dispose();
                hv_minImageHeight.Dispose();
                hv_Angle.Dispose();
                hv_HomMat2D.Dispose();
                hv_Channels.Dispose();
                return null;
            }

            ho_ImageInput.Dispose();
            ho_Image1.Dispose();
            ho_ImageAffineTrans.Dispose();
            ho_Image11.Dispose();
            ho_Image2.Dispose();
            ho_Image3.Dispose();
            ho_Image4.Dispose();
            ho_Image5.Dispose();
            ho_Image6.Dispose();
            ho_ImageAffineTrans1.Dispose();
            ho_ImageAffineTrans2.Dispose();
            ho_ImageAffineTrans3.Dispose();
            ho_MultiChannelImage.Dispose();
            hv_Width.Dispose();
            hv_Height.Dispose();
            hv_minImageWidth.Dispose();
            hv_minImageHeight.Dispose();
            hv_Angle.Dispose();
            hv_HomMat2D.Dispose();
            hv_Channels.Dispose();
            return null;
        }
        #endregion

        #region gen_arrow_contour_xld:绘制箭头
        public void gen_arrow_contour_xld(out HObject ho_Arrow, HTuple hv_Row1, HTuple hv_Column1, HTuple hv_Row2, HTuple hv_Column2, HTuple hv_HeadLength, HTuple hv_HeadWidth)
        {
            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];

            // Local iconic variables 

            HObject ho_TempArrow = null;

            // Local control variables 

            HTuple hv_Length = new HTuple(), hv_ZeroLengthIndices = new HTuple();
            HTuple hv_DR = new HTuple(), hv_DC = new HTuple(), hv_HalfHeadWidth = new HTuple();
            HTuple hv_RowP1 = new HTuple(), hv_ColP1 = new HTuple();
            HTuple hv_RowP2 = new HTuple(), hv_ColP2 = new HTuple();
            HTuple hv_Index = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Arrow);
            HOperatorSet.GenEmptyObj(out ho_TempArrow);
            //This procedure generates arrow shaped XLD contours,
            //pointing from (Row1, Column1) to (Row2, Column2).
            //If starting and end point are identical, a contour consisting
            //of a single point is returned.
            //
            //input parameteres:
            //Row1, Column1: Coordinates of the arrows' starting points
            //Row2, Column2: Coordinates of the arrows' end points
            //HeadLength, HeadWidth: Size of the arrow heads in pixels
            //
            //output parameter:
            //Arrow: The resulting XLD contour
            //
            //The input tuples Row1, Column1, Row2, and Column2 have to be of
            //the same length.
            //HeadLength and HeadWidth either have to be of the same length as
            //Row1, Column1, Row2, and Column2 or have to be a single element.
            //If one of the above restrictions is violated, an error will occur.
            //
            //
            //Init
            ho_Arrow.Dispose();
            HOperatorSet.GenEmptyObj(out ho_Arrow);
            //
            //Calculate the arrow length
            hv_Length.Dispose();
            HOperatorSet.DistancePp(hv_Row1, hv_Column1, hv_Row2, hv_Column2, out hv_Length);
            //
            //Mark arrows with identical start and end point
            //(set Length to -1 to avoid division-by-zero exception)
            hv_ZeroLengthIndices.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_ZeroLengthIndices = hv_Length.TupleFind(
                    0);
            }
            if ((int)(new HTuple(hv_ZeroLengthIndices.TupleNotEqual(-1))) != 0)
            {
                if (hv_Length == null)
                    hv_Length = new HTuple();
                hv_Length[hv_ZeroLengthIndices] = -1;
            }
            //
            //Calculate auxiliary variables.
            hv_DR.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_DR = (1.0 * (hv_Row2 - hv_Row1)) / hv_Length;
            }
            hv_DC.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_DC = (1.0 * (hv_Column2 - hv_Column1)) / hv_Length;
            }
            hv_HalfHeadWidth.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_HalfHeadWidth = hv_HeadWidth / 2.0;
            }
            //
            //Calculate end points of the arrow head.
            hv_RowP1.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_RowP1 = (hv_Row1 + ((hv_Length - hv_HeadLength) * hv_DR)) + (hv_HalfHeadWidth * hv_DC);
            }
            hv_ColP1.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_ColP1 = (hv_Column1 + ((hv_Length - hv_HeadLength) * hv_DC)) - (hv_HalfHeadWidth * hv_DR);
            }
            hv_RowP2.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_RowP2 = (hv_Row1 + ((hv_Length - hv_HeadLength) * hv_DR)) - (hv_HalfHeadWidth * hv_DC);
            }
            hv_ColP2.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_ColP2 = (hv_Column1 + ((hv_Length - hv_HeadLength) * hv_DC)) + (hv_HalfHeadWidth * hv_DR);
            }
            //
            //Finally create output XLD contour for each input point pair
            for (hv_Index = 0; (int)hv_Index <= (int)((new HTuple(hv_Length.TupleLength())) - 1); hv_Index = (int)hv_Index + 1)
            {
                if ((int)(new HTuple(((hv_Length.TupleSelect(hv_Index))).TupleEqual(-1))) != 0)
                {
                    //Create_ single points for arrows with identical start and end point
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        ho_TempArrow.Dispose();
                        HOperatorSet.GenContourPolygonXld(out ho_TempArrow, hv_Row1.TupleSelect(hv_Index),
                            hv_Column1.TupleSelect(hv_Index));
                    }
                }
                else
                {
                    //Create arrow contour
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        ho_TempArrow.Dispose();
                        HOperatorSet.GenContourPolygonXld(out ho_TempArrow, ((((((((((hv_Row1.TupleSelect(
                            hv_Index))).TupleConcat(hv_Row2.TupleSelect(hv_Index)))).TupleConcat(
                            hv_RowP1.TupleSelect(hv_Index)))).TupleConcat(hv_Row2.TupleSelect(hv_Index)))).TupleConcat(
                            hv_RowP2.TupleSelect(hv_Index)))).TupleConcat(hv_Row2.TupleSelect(hv_Index)),
                            ((((((((((hv_Column1.TupleSelect(hv_Index))).TupleConcat(hv_Column2.TupleSelect(
                            hv_Index)))).TupleConcat(hv_ColP1.TupleSelect(hv_Index)))).TupleConcat(
                            hv_Column2.TupleSelect(hv_Index)))).TupleConcat(hv_ColP2.TupleSelect(
                            hv_Index)))).TupleConcat(hv_Column2.TupleSelect(hv_Index)));
                    }
                }
                {
                    HObject ExpTmpOutVar_0;
                    HOperatorSet.ConcatObj(ho_Arrow, ho_TempArrow, out ExpTmpOutVar_0);
                    ho_Arrow.Dispose();
                    ho_Arrow = ExpTmpOutVar_0;
                }
            }
            ho_TempArrow.Dispose();

            hv_Length.Dispose();
            hv_ZeroLengthIndices.Dispose();
            hv_DR.Dispose();
            hv_DC.Dispose();
            hv_HalfHeadWidth.Dispose();
            hv_RowP1.Dispose();
            hv_ColP1.Dispose();
            hv_RowP2.Dispose();
            hv_ColP2.Dispose();
            hv_Index.Dispose();

            return;
        }
        #endregion
    }
}