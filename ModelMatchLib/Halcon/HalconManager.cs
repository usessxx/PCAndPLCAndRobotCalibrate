using ChoiceTech.Halcon.Control;
using HalconDotNet;
using MatchModel.Halcon;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ViewWindow.Model;

public partial class HDevelopExport
{
    public static HDevelopExport Instance { get; private set; }
    private HDevelopExport() { HOperatorSet.GenContourPolygonXld(out EmptyReg, new HTuple(0, 1), new HTuple(0, 1)); }

    static HDevelopExport()
    {
        Instance = new HDevelopExport();
    }


    HObject EmptyReg;


    /// <summary>
    /// 根据模板参数匹配模板
    /// </summary>
    /// <param name="Image"></param>
    /// <param name="ModelId"></param>
    /// <param name="InputDict"></param>
    /// <param name="searchRoiList"></param>
    /// <param name="hw"></param>
    public bool MatchAllModel(HObject Image, HTuple ModelId, HTuple NccModelId, HTuple InputDict, List<ROI> searchRoiList, List<FindModelROICfg> findModelROICfgList,
        out HTuple MatchResultDict,bool isRunning, HWindow_Final hw = null)
    {
        HTuple MatchRows = new HTuple(), MatchCols = new HTuple(), MatchAngles = new HTuple(), MatchScores = new HTuple();
        HOperatorSet.CreateDict(out MatchResultDict);
        if (searchRoiList.Count == 0)//如没设置ROI，则全图匹配
        {
            HOperatorSet.GetImageSize(Image, out HTuple ImgW, out HTuple ImgH);
            HOperatorSet.GenRectangle1(out HObject rect1, 0, 0, ImgW, ImgH);
            HDevelopExport.Instance.FindShapeModel(Image, rect1, out HObject AffineTransContour, InputDict, ModelId, NccModelId,
                out HTuple OutputTuple, out HTuple exception);

            if (hw != null)
            {
                hw.viewWindow.displayHobject(AffineTransContour, "red");
                if (OutputTuple.TupleLength() == 0 || OutputTuple.TupleSelect(3).TupleLength() == 0)
                {
                    hw.viewWindow.dispMessage($"匹配失败", "black", 100, 100);
                }
                else
                {
                    for (int i = 0; i < OutputTuple.TupleSelect(0).TupleLength(); i++)
                    {
                        //hw.viewWindow.dispMessage($"匹配得分：{OutputTuple.TupleSelect(3).D.ToString("0.00")}", "black",
                        //    OutputTuple.TupleSelect(0), OutputTuple.TupleSelect(1));
                        MatchRows = MatchRows.TupleConcat(OutputTuple.TupleSelect(0));
                        MatchCols = MatchCols.TupleConcat(OutputTuple.TupleSelect(1));
                        MatchAngles = MatchAngles.TupleConcat(OutputTuple.TupleSelect(2));
                        MatchScores = MatchScores.TupleConcat(OutputTuple.TupleSelect(3));
                    }
                }
            }
        }
        else
        {
            if (!isRunning)
            {
                hw?.viewWindow.displayROI(ref searchRoiList);
            }
            for (int i = 0; i < searchRoiList.Count; i++)
            {
                HTuple data = searchRoiList[i].getModelData();
                HOperatorSet.GenRectangle1(out HObject rect1, data[0], data[1], data[2], data[3]);
                if (isRunning)
                {
                    hw?.viewWindow.displayHobject(rect1, "blue");
                }
                HDevelopExport.Instance.FindShapeModel(Image, rect1, out HObject AffineTransContour, InputDict, ModelId, NccModelId,
                    out HTuple OutputTuple, out HTuple exception);
                if (exception.TupleLength() > 0)
                {
                    MessageBox.Show($"{exception.S}");
                }
                if (hw != null)
                {
                    hw.viewWindow.displayHobject(AffineTransContour, "red");
                    hw.viewWindow.dispMessage($"{findModelROICfgList[i].RoiId}", "black", data[0].D, data[1].D);
                    if (OutputTuple.TupleLength() == 0)
                    {
                        hw.viewWindow.dispMessage($"匹配失败", "black", (data[0] + data[2]) / 2, (data[1] + data[3]) / 2);
                        MatchRows = MatchRows.TupleConcat(0);
                        MatchCols = MatchCols.TupleConcat(0);
                        MatchAngles = MatchAngles.TupleConcat(0);
                        MatchScores = MatchScores.TupleConcat(0);
                        continue;
                    }
                    else
                    {
                        for (int j = 0; j < OutputTuple.TupleSelect(0).TupleLength(); j++)
                        {
                            //hw.viewWindow.dispMessage($"匹配得分：{OutputTuple.TupleSelect(3).TupleSelect(j).D.ToString("0.00")}",
                            //    "black", OutputTuple.TupleSelect(0).TupleSelect(j), OutputTuple.TupleSelect(1).TupleSelect(j));
                        }
                    }
                }
                MatchRows = MatchRows.TupleConcat(OutputTuple.TupleSelect(0));
                MatchCols = MatchCols.TupleConcat(OutputTuple.TupleSelect(1));
                MatchAngles = MatchAngles.TupleConcat(OutputTuple.TupleSelect(2));
                MatchScores = MatchScores.TupleConcat(OutputTuple.TupleSelect(3));
            }
        }

        HOperatorSet.SetDictTuple(MatchResultDict, ShapeModelOutputEnum.MatchRow.ToString(), MatchRows);
        HOperatorSet.SetDictTuple(MatchResultDict, ShapeModelOutputEnum.MatchCol.ToString(), MatchCols);
        HOperatorSet.SetDictTuple(MatchResultDict, ShapeModelOutputEnum.MatchAngle.ToString(), MatchAngles);
        HOperatorSet.SetDictTuple(MatchResultDict, ShapeModelOutputEnum.MatchScore.ToString(), MatchScores);
        return true;
    }

    public void GetCircleCenter(HObject OrigImage, HTuple ModelId, HTuple NccModelId, HTuple InputDict, List<ROI> searchRoiList, List<FindModelROICfg> findModelROICfgList, HTuple matchOrigTuple,
        List<ROI> FindEdgeRoiList, List<FindLineCfg> FindLineCfgList,out List<HTuple> CircleParamList,HWindow_Final hw = null)
    {
        CircleParamList = new List<HTuple>();
        //匹配所有测量位置
        if (!MatchAllModel(OrigImage, ModelId, NccModelId, InputDict, searchRoiList, findModelROICfgList, out HTuple MatchResultDict, true, hw))
        {
            return;
        }
        HOperatorSet.GetDictTuple(MatchResultDict, ShapeModelOutputEnum.MatchRow.ToString(), out HTuple MatchRows);
        HOperatorSet.GetDictTuple(MatchResultDict, ShapeModelOutputEnum.MatchCol.ToString(), out HTuple MatchCols);
        HOperatorSet.GetDictTuple(MatchResultDict, ShapeModelOutputEnum.MatchAngle.ToString(), out HTuple MatchAngles);


        for (int i = 0; i < MatchRows.TupleLength(); i++)//一个模板对应一个检测区域
        {
           
            //创建模板原点-->匹配位置 生成仿射变换矩阵
            HOperatorSet.VectorAngleToRigid(matchOrigTuple.TupleSelect(0), matchOrigTuple.TupleSelect(1), matchOrigTuple.TupleSelect(2),
                MatchRows.TupleSelect(i), MatchCols.TupleSelect(i), MatchAngles.TupleSelect(i), out HTuple homMat);
            //边缘查找
            HOperatorSet.GenEmptyObj(out HObject FitLines);
            for (int j = 0; j < FindEdgeRoiList.Count; j++)
            {
                //获取匹配 仿射变换之后的ROI
                HTuple OrigRect2Data = FindEdgeRoiList[j].getModelData();

                HOperatorSet.AffineTransPoint2d(homMat, OrigRect2Data[0], OrigRect2Data[1], out HTuple qx, out HTuple qy);

                //AffineRect2Data(out HObject ArrowAffine, OrigRect2Data, homMat, out HTuple rect2Data);

                HTuple rect2Data = new HTuple(qx, qy, OrigRect2Data[2]);


                FindLine(OrigImage, out HObject MeasureContour, out HObject FitLine, out HObject Cross, rect2Data, FindLineCfgList[j].Transition, FindLineCfgList[j].Select,
                        FindLineCfgList[j].MeasureThreshold,out HTuple circleParam, out HTuple ErrorLog);
                if (ErrorLog.TupleLength() > 0)
                {
                    MessageBox.Show($"圆提取：{findModelROICfgList[i].RoiId} - {FindLineCfgList[j].RoiId}{ErrorLog.ToString()}");
                    HOperatorSet.ConcatObj(FitLines, EmptyReg, out FitLines);
                    if (hw != null)//显示
                    {
                        HOperatorSet.GenRectangle2(out HObject TempRect2, rect2Data[0], rect2Data[1], -rect2Data[2].D, rect2Data[3], rect2Data[4]);
                        hw.viewWindow.displayHobject(TempRect2, "green");
                        //hw.viewWindow.displayHobject(ArrowAffine, "green");
                    }
                    continue;
                }
                HOperatorSet.ConcatObj(FitLines, FitLine, out FitLines);
                if (hw != null)//显示
                {
                    hw.viewWindow.displayHobject(FitLine, "light blue");
                    hw.viewWindow.displayHobject(Cross, "gold");
                    Cross.Dispose();
                }
                CircleParamList.Add(circleParam);
            }
        }


    }


    public void Color2OCAG(HObject ho_Image5ms2, out HObject ho_AG, out HObject ho_OC)
    {



        // Local iconic variables 

        HObject ho_R, ho_B, ho_subImage;
        // Initialize local and output iconic variables 
        HOperatorSet.GenEmptyObj(out ho_AG);
        HOperatorSet.GenEmptyObj(out ho_OC);
        HOperatorSet.GenEmptyObj(out ho_R);
        HOperatorSet.GenEmptyObj(out ho_B);
        HOperatorSet.GenEmptyObj(out ho_subImage);
        ho_R.Dispose(); ho_AG.Dispose(); ho_B.Dispose();
        HOperatorSet.Decompose3(ho_Image5ms2, out ho_R, out ho_AG, out ho_B);
        ho_subImage.Dispose();
        HOperatorSet.SubImage(ho_R, ho_AG, out ho_subImage, 3, 0);
        ho_OC.Dispose();
        HOperatorSet.EquHistoImage(ho_subImage, out ho_OC);
        ho_R.Dispose();
        ho_B.Dispose();
        ho_subImage.Dispose();


        return;
    }
}

