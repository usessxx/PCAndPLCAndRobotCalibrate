using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HalconDotNet;
using System.Windows.Forms;

namespace HalconMVTec
{
    public class DeepLearningObjectDetectionRectangle2DataAugmentation
    {
        #region DLObjectDetectionRectangle2DataAugmentationAction
        public void DLObjectDetectionRectangle2DataAugmentationAction(HTuple hv_dict, HTuple hv_images_fotmat, HTuple hv_imags_fill, HTuple hv_WindowHandle, HTuple hv_data_enhanced, out HTuple hv_dict_copy)
        {
            // Local iconic variables 

            HObject ho_Image = null, ho_ImageMirror_row = null;
            HObject ho_ImageMirror_col = null, ho_ImageMirror_temp = null;
            HObject ho_ImageMirror_rc = null, ho_ImageEmphasize = null;
            HObject ho_ImageMirror_row_em = null, ho_ImageMirror_col_em = null;
            HObject ho_ImageMirror_rc_em = null, ho_ImageIlluminate = null;
            HObject ho_ImageIlluminate_row_ill = null, ho_ImageIlluminate_col_ill = null;
            HObject ho_ImageIlluminate_rc_ill = null;

            // Local control variables 

            HTuple hv_WindowHandle_now = new HTuple();
            HTuple hv_image_dir = new HTuple(), hv_new_images_dir = new HTuple();
            HTuple hv_FileExists = new HTuple(), hv_samples = new HTuple();
            HTuple hv_samples_copy = new HTuple(), hv_samples_Length = new HTuple();
            HTuple hv_MSecond2 = new HTuple(), hv_Second2 = new HTuple();
            HTuple hv_Minute2 = new HTuple(), hv_Hour2 = new HTuple();
            HTuple hv_Day2 = new HTuple(), hv_YDay2 = new HTuple();
            HTuple hv_Month2 = new HTuple(), hv_Year2 = new HTuple();
            HTuple hv_images_id = new HTuple(), hv_Index = new HTuple();
            HTuple hv_MSecond = new HTuple(), hv_Second = new HTuple();
            HTuple hv_Minute = new HTuple(), hv_Hour = new HTuple();
            HTuple hv_Day = new HTuple(), hv_YDay = new HTuple(), hv_Month = new HTuple();
            HTuple hv_Year = new HTuple(), hv_image_file_name = new HTuple();
            HTuple hv_split_name = new HTuple(), hv_split_name_Substrings = new HTuple();
            HTuple hv_sub_Length = new HTuple(), hv_new_sub_dir = new HTuple();
            HTuple hv_FileExists1 = new HTuple(), hv_bbox_label_id = new HTuple();
            HTuple hv_bbox_row = new HTuple(), hv_bbox_col = new HTuple();
            HTuple hv_bbox_length1 = new HTuple(), hv_bbox_length2 = new HTuple();
            HTuple hv_bbox_phi = new HTuple(), hv_Width = new HTuple();
            HTuple hv_Height = new HTuple(), hv_samples_sub_Original = new HTuple();
            HTuple hv_new_row = new HTuple(), hv_new_phi = new HTuple();
            HTuple hv_samples_sub_Row = new HTuple(), hv_new_col = new HTuple();
            HTuple hv_samples_sub_Col = new HTuple(), hv_samples_sub_RC = new HTuple();
            HTuple hv_samples_sub_Original_em = new HTuple(), hv_samples_sub_Row_em = new HTuple();
            HTuple hv_samples_sub_Col_em = new HTuple(), hv_samples_sub_RC_em = new HTuple();
            HTuple hv_samples_sub_Original_ill = new HTuple(), hv_samples_sub_Row_ill = new HTuple();
            HTuple hv_samples_sub_Col_ill = new HTuple(), hv_samples_sub_rc_ill = new HTuple();
            HTuple hv_MSecond1 = new HTuple(), hv_Second1 = new HTuple();
            HTuple hv_Minute1 = new HTuple(), hv_Hour1 = new HTuple();
            HTuple hv_Day1 = new HTuple(), hv_YDay1 = new HTuple();
            HTuple hv_Month1 = new HTuple(), hv_Year1 = new HTuple();
            HTuple hv_single_time = new HTuple(), hv_time_consuming = new HTuple();
            HTuple hv_WindowHandle_COPY_INP_TMP = new HTuple(hv_WindowHandle);

            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Image);
            HOperatorSet.GenEmptyObj(out ho_ImageMirror_row);
            HOperatorSet.GenEmptyObj(out ho_ImageMirror_col);
            HOperatorSet.GenEmptyObj(out ho_ImageMirror_temp);
            HOperatorSet.GenEmptyObj(out ho_ImageMirror_rc);
            HOperatorSet.GenEmptyObj(out ho_ImageEmphasize);
            HOperatorSet.GenEmptyObj(out ho_ImageMirror_row_em);
            HOperatorSet.GenEmptyObj(out ho_ImageMirror_col_em);
            HOperatorSet.GenEmptyObj(out ho_ImageMirror_rc_em);
            HOperatorSet.GenEmptyObj(out ho_ImageIlluminate);
            HOperatorSet.GenEmptyObj(out ho_ImageIlluminate_row_ill);
            HOperatorSet.GenEmptyObj(out ho_ImageIlluminate_col_ill);
            HOperatorSet.GenEmptyObj(out ho_ImageIlluminate_rc_ill);
            hv_dict_copy = new HTuple();
            try
            {
                //显示部分
                HOperatorSet.ClearWindow(hv_WindowHandle);
                set_display_font(hv_WindowHandle, 26, "mono", "true", "false");
                //disp_message(hv_WindowHandle, "管理员：DP", "window", 12, 12, "red", "false");
                disp_message(hv_WindowHandle, ("数据增强： -> " + hv_data_enhanced) + " 倍", "window", 50, 12, "red", "false");
                disp_message(hv_WindowHandle, "进度： 0 %", "window", 98, 12, "red", "false");
                disp_message(hv_WindowHandle, "", "window", 146, 12, "red", "false");
                disp_message(hv_WindowHandle, "已用时间：  ", "window", 192, 12, "red", "false");
                disp_message(hv_WindowHandle, "剩余时间：  ", "window", 240, 12, "red", "false");

                //复制一份数据，用于写操作(dict_copy)
                hv_dict_copy.Dispose();
                HOperatorSet.CopyDict(hv_dict, new HTuple(), new HTuple(), out hv_dict_copy);

                //得到数据集图片的路径（读） -> image_dir
                hv_image_dir.Dispose();
                HOperatorSet.GetDictTuple(hv_dict, "image_dir", out hv_image_dir);

                //*************************************************
                //新建图片目录（增强后的图片存放位置）
                hv_new_images_dir.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_new_images_dir = hv_image_dir + "new_images/";
                }
                hv_FileExists.Dispose();
                HOperatorSet.FileExists(hv_new_images_dir, out hv_FileExists);
                if ((int)(hv_FileExists.TupleNot()) != 0)
                {
                    HOperatorSet.MakeDir(hv_new_images_dir);
                }
                //*************************************************
                //获得'samples'
                hv_samples.Dispose();
                HOperatorSet.GetDictTuple(hv_dict, "samples", out hv_samples);
                //复制'samples' -> samples_copy(写操作）
                hv_samples_copy.Dispose();
                hv_samples_copy = new HTuple(hv_samples);

                //获得'samples'的长度 -> 计算for循环a
                hv_samples_Length.Dispose();
                HOperatorSet.TupleLength(hv_samples, out hv_samples_Length);


                hv_MSecond2.Dispose(); hv_Second2.Dispose(); hv_Minute2.Dispose(); hv_Hour2.Dispose(); hv_Day2.Dispose(); hv_YDay2.Dispose(); hv_Month2.Dispose(); hv_Year2.Dispose();
                HOperatorSet.GetSystemTime(out hv_MSecond2, out hv_Second2, out hv_Minute2,
                    out hv_Hour2, out hv_Day2, out hv_YDay2, out hv_Month2, out hv_Year2);
                //图片存放位置id
                hv_images_id.Dispose();
                hv_images_id = 0;

                //开始遍历每一张图片
                HTuple end_val46 = hv_samples_Length - 1;
                HTuple step_val46 = 1;
                for (hv_Index = 0; hv_Index.Continue(end_val46, step_val46); hv_Index = hv_Index.TupleAdd(step_val46))
                {
                    //******************获得公共部分********************************************
                    hv_MSecond.Dispose(); hv_Second.Dispose(); hv_Minute.Dispose(); hv_Hour.Dispose(); hv_Day.Dispose(); hv_YDay.Dispose(); hv_Month.Dispose(); hv_Year.Dispose();
                    HOperatorSet.GetSystemTime(out hv_MSecond, out hv_Second, out hv_Minute,
                        out hv_Hour, out hv_Day, out hv_YDay, out hv_Month, out hv_Year);
                    //解析每一个samples[Index] -> samples为数组
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_image_file_name.Dispose();
                        HOperatorSet.GetDictTuple(hv_samples.TupleSelect(hv_Index), "image_file_name",
                            out hv_image_file_name);
                    }
                    //1.bmp -> [1,bmp]
                    hv_split_name.Dispose();
                    HOperatorSet.TupleSplit(hv_image_file_name, ".", out hv_split_name);
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_split_name = hv_split_name.TupleSelect(
                                0);
                            hv_split_name.Dispose();
                            hv_split_name = ExpTmpLocalVar_split_name;
                        }
                    }
                    //判断有无子文件夹
                    hv_split_name_Substrings.Dispose();
                    HOperatorSet.TupleSplit(hv_split_name, "/", out hv_split_name_Substrings);
                    hv_sub_Length.Dispose();
                    HOperatorSet.TupleLength(hv_split_name_Substrings, out hv_sub_Length);
                    if ((int)(new HTuple(hv_sub_Length.TupleGreater(1))) != 0)
                    {
                        //ADD
                        hv_split_name.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_split_name = hv_split_name_Substrings.TupleSelect(
                                hv_sub_Length - 1);
                        }
                        hv_new_sub_dir.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_new_sub_dir = hv_new_images_dir + (hv_split_name_Substrings.TupleSelect(
                                0));
                        }
                        hv_FileExists1.Dispose();
                        HOperatorSet.FileExists(hv_new_sub_dir, out hv_FileExists1);
                        if ((int)(hv_FileExists1.TupleNot()) != 0)
                        {
                            HOperatorSet.MakeDir(hv_new_sub_dir);
                        }
                    }

                    //获得'bbox_label_id'
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_bbox_label_id.Dispose();
                        HOperatorSet.GetDictTuple(hv_samples.TupleSelect(hv_Index), "bbox_label_id",
                            out hv_bbox_label_id);
                    }

                    //与水平矩形不同处
                    //获得'bbox_row'
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_bbox_row.Dispose();
                        HOperatorSet.GetDictTuple(hv_samples.TupleSelect(hv_Index), "bbox_row", out hv_bbox_row);
                    }
                    //获得'bbox_col'
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_bbox_col.Dispose();
                        HOperatorSet.GetDictTuple(hv_samples.TupleSelect(hv_Index), "bbox_col", out hv_bbox_col);
                    }
                    //获得'bbox_length1'
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_bbox_length1.Dispose();
                        HOperatorSet.GetDictTuple(hv_samples.TupleSelect(hv_Index), "bbox_length1",
                            out hv_bbox_length1);
                    }
                    //获得'bbox_length2'
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_bbox_length2.Dispose();
                        HOperatorSet.GetDictTuple(hv_samples.TupleSelect(hv_Index), "bbox_length2",
                            out hv_bbox_length2);
                    }
                    //获得'bbox_phi'
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_bbox_phi.Dispose();
                        HOperatorSet.GetDictTuple(hv_samples.TupleSelect(hv_Index), "bbox_phi", out hv_bbox_phi);
                    }

                    //与水平矩形不同处
                    //获得标注框的宽和高
                    //lable_width := bbox_col2 - bbox_col1
                    //lable_height := bbox_row2 - bbox_row1

                    //原图-> 图片格式需要发生变化（jpeg,jpg,pn,bmp,tif,tiff)
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        ho_Image.Dispose();
                        HOperatorSet.ReadImage(out ho_Image, hv_image_dir + hv_image_file_name);
                    }
                    //获得图片的宽高
                    hv_Width.Dispose(); hv_Height.Dispose();
                    HOperatorSet.GetImageSize(ho_Image, out hv_Width, out hv_Height);
                    //*************原图*************************************************
                    if ((int)(new HTuple(hv_data_enhanced.TupleGreater(0))) != 0)
                    {
                        //发生变化的内容*
                        //保存原图-> 改变格式(1.png)
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            HOperatorSet.WriteImage(ho_Image, hv_images_fotmat, hv_imags_fill, ((hv_new_images_dir + hv_split_name) + ".") + hv_images_fotmat);
                        }

                        //把samples[Index]写入
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_samples_sub_Original.Dispose();
                            HOperatorSet.CopyDict(hv_samples.TupleSelect(hv_Index), new HTuple(), new HTuple(),
                                out hv_samples_sub_Original);
                        }
                        //更新images_id
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_images_id = hv_images_id + 1;
                                hv_images_id.Dispose();
                                hv_images_id = ExpTmpLocalVar_images_id;
                            }
                        }
                        HOperatorSet.SetDictTuple(hv_samples_sub_Original, "image_id", hv_images_id);
                        //更新'image_file_name'
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            HOperatorSet.SetDictTuple(hv_samples_sub_Original, "image_file_name", (hv_split_name + ".") + hv_images_fotmat);
                        }
                        //更新到samples_copy
                        if (hv_samples_copy == null)
                            hv_samples_copy = new HTuple();
                        hv_samples_copy[hv_images_id - 1] = hv_samples_sub_Original;
                    }
                    //****************行镜像**********************************************************
                    //行镜像图片
                    if ((int)(new HTuple(hv_data_enhanced.TupleGreater(1))) != 0)
                    {
                        ho_ImageMirror_row.Dispose();
                        HOperatorSet.MirrorImage(ho_Image, out ho_ImageMirror_row, "row");
                        //保存图片
                        //镜像后的图片 ->需要重新命名_row
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            HOperatorSet.WriteImage(ho_ImageMirror_row, hv_images_fotmat, hv_imags_fill,
                                ((hv_new_images_dir + hv_split_name) + "_row.") + hv_images_fotmat);
                        }

                        //与水平矩形不同处
                        //镜像坐标
                        hv_new_row.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_new_row = hv_Height - hv_bbox_row;
                        }
                        hv_new_phi.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_new_phi = 0.0 - hv_bbox_phi;
                        }
                        //更新改动的数据
                        //把samples[Index]写入
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_samples_sub_Row.Dispose();
                            HOperatorSet.CopyDict(hv_samples.TupleSelect(hv_Index), new HTuple(), new HTuple(),
                                out hv_samples_sub_Row);
                        }
                        //更新-> 'image_id'
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_images_id = hv_images_id + 1;
                                hv_images_id.Dispose();
                                hv_images_id = ExpTmpLocalVar_images_id;
                            }
                        }
                        HOperatorSet.SetDictTuple(hv_samples_sub_Row, "image_id", hv_images_id);
                        //更新'image_file_name'
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            HOperatorSet.SetDictTuple(hv_samples_sub_Row, "image_file_name", (hv_split_name + "_row.") + hv_images_fotmat);
                        }

                        //与水平矩形不同处
                        //更新'bbox_row'
                        HOperatorSet.SetDictTuple(hv_samples_sub_Row, "bbox_row", hv_new_row);
                        //更新'bbox_phi'
                        HOperatorSet.SetDictTuple(hv_samples_sub_Row, "bbox_phi", hv_new_phi);
                        //更新到samples_copy
                        if (hv_samples_copy == null)
                            hv_samples_copy = new HTuple();
                        hv_samples_copy[hv_images_id - 1] = hv_samples_sub_Row;
                    }
                    //***********列镜像***************************************************************
                    //列镜像
                    if ((int)(new HTuple(hv_data_enhanced.TupleGreater(2))) != 0)
                    {
                        ho_ImageMirror_col.Dispose();
                        HOperatorSet.MirrorImage(ho_Image, out ho_ImageMirror_col, "column");
                        //保存图片
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            HOperatorSet.WriteImage(ho_ImageMirror_col, hv_images_fotmat, hv_imags_fill,
                                ((hv_new_images_dir + hv_split_name) + "_col.") + hv_images_fotmat);
                        }

                        //与水平矩形不同处
                        //镜像坐标
                        hv_new_col.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_new_col = hv_Width - hv_bbox_col;
                        }
                        if ((int)(new HTuple(hv_bbox_phi.TupleGreaterEqual(0))) != 0)
                        {
                            hv_new_phi.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_new_phi = ((new HTuple(180)).TupleRad()
                                    ) - hv_bbox_phi;
                            }
                        }
                        if ((int)(new HTuple(hv_bbox_phi.TupleLess(0))) != 0)
                        {
                            hv_new_phi.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_new_phi = 0.0 - (((new HTuple(180)).TupleRad()
                                    ) + hv_bbox_phi);
                            }
                        }
                        //更新改动的数据
                        //把samples[Index]写入
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_samples_sub_Col.Dispose();
                            HOperatorSet.CopyDict(hv_samples.TupleSelect(hv_Index), new HTuple(), new HTuple(),
                                out hv_samples_sub_Col);
                        }
                        //更新-> 'image_id'
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_images_id = hv_images_id + 1;
                                hv_images_id.Dispose();
                                hv_images_id = ExpTmpLocalVar_images_id;
                            }
                        }
                        HOperatorSet.SetDictTuple(hv_samples_sub_Col, "image_id", hv_images_id);
                        //更新'image_file_name'
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            HOperatorSet.SetDictTuple(hv_samples_sub_Col, "image_file_name", (hv_split_name + "_col.") + hv_images_fotmat);
                        }

                        //与水平矩形不同处
                        //更新'bbox_col'
                        HOperatorSet.SetDictTuple(hv_samples_sub_Col, "bbox_col", hv_new_col);
                        //更新'bbox_phi'
                        HOperatorSet.SetDictTuple(hv_samples_sub_Col, "bbox_phi", hv_new_phi);
                        //更新到samples_copy
                        if (hv_samples_copy == null)
                            hv_samples_copy = new HTuple();
                        hv_samples_copy[hv_images_id - 1] = hv_samples_sub_Col;
                    }
                    //***********行列同时镜像***************************************************************
                    if ((int)(new HTuple(hv_data_enhanced.TupleGreater(3))) != 0)
                    {
                        ho_ImageMirror_temp.Dispose();
                        HOperatorSet.MirrorImage(ho_Image, out ho_ImageMirror_temp, "row");
                        ho_ImageMirror_rc.Dispose();
                        HOperatorSet.MirrorImage(ho_ImageMirror_temp, out ho_ImageMirror_rc, "column");
                        //保存图片
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            HOperatorSet.WriteImage(ho_ImageMirror_rc, hv_images_fotmat, hv_imags_fill,
                                ((hv_new_images_dir + hv_split_name) + "_rc.") + hv_images_fotmat);
                        }

                        //与水平矩形不同处
                        //镜像坐标
                        hv_new_row.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_new_row = hv_Height - hv_bbox_row;
                        }
                        hv_new_col.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_new_col = hv_Width - hv_bbox_col;
                        }
                        hv_new_phi.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_new_phi = 0.0 - hv_bbox_phi;
                        }
                        if ((int)(new HTuple(hv_new_phi.TupleGreaterEqual(0))) != 0)
                        {
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                {
                                    HTuple
                                      ExpTmpLocalVar_new_phi = ((new HTuple(180)).TupleRad()
                                        ) - hv_new_phi;
                                    hv_new_phi.Dispose();
                                    hv_new_phi = ExpTmpLocalVar_new_phi;
                                }
                            }
                        }
                        if ((int)(new HTuple(hv_new_phi.TupleLess(0))) != 0)
                        {
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                {
                                    HTuple
                                      ExpTmpLocalVar_new_phi = 0.0 - (((new HTuple(180)).TupleRad()
                                        ) + hv_new_phi);
                                    hv_new_phi.Dispose();
                                    hv_new_phi = ExpTmpLocalVar_new_phi;
                                }
                            }
                        }

                        //更新改动的数据
                        //把samples[Index]写入
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_samples_sub_RC.Dispose();
                            HOperatorSet.CopyDict(hv_samples.TupleSelect(hv_Index), new HTuple(), new HTuple(),
                                out hv_samples_sub_RC);
                        }
                        //更新-> 'image_id'
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_images_id = hv_images_id + 1;
                                hv_images_id.Dispose();
                                hv_images_id = ExpTmpLocalVar_images_id;
                            }
                        }
                        HOperatorSet.SetDictTuple(hv_samples_sub_RC, "image_id", hv_images_id);
                        //更新'image_file_name'
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            HOperatorSet.SetDictTuple(hv_samples_sub_RC, "image_file_name", (hv_split_name + "_rc.") + hv_images_fotmat);
                        }

                        //与水平矩形不同处
                        //更新'bbox_row'
                        HOperatorSet.SetDictTuple(hv_samples_sub_RC, "bbox_row", hv_new_row);
                        //更新'bbox_col'
                        HOperatorSet.SetDictTuple(hv_samples_sub_RC, "bbox_col", hv_new_col);
                        //更新'bbox_phi'
                        HOperatorSet.SetDictTuple(hv_samples_sub_RC, "bbox_phi", hv_new_phi);
                        //更新到samples_copy
                        if (hv_samples_copy == null)
                            hv_samples_copy = new HTuple();
                        hv_samples_copy[hv_images_id - 1] = hv_samples_sub_RC;
                    }
                    //**************************************************************************
                    if ((int)(new HTuple(hv_data_enhanced.TupleGreater(4))) != 0)
                    {
                        //对原图/行镜像/列镜像/行列同时镜像 -> 增加对比度（4+4）
                        //Image
                        ho_ImageEmphasize.Dispose();
                        HOperatorSet.Emphasize(ho_Image, out ho_ImageEmphasize, 15, 15, 3);
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            HOperatorSet.WriteImage(ho_ImageEmphasize, hv_images_fotmat, hv_imags_fill,
                                ((hv_new_images_dir + hv_split_name) + "_em.") + hv_images_fotmat);
                        }

                        //把samples[Index]写入
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_samples_sub_Original_em.Dispose();
                            HOperatorSet.CopyDict(hv_samples.TupleSelect(hv_Index), new HTuple(), new HTuple(),
                                out hv_samples_sub_Original_em);
                        }
                        //更新images_id
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_images_id = hv_images_id + 1;
                                hv_images_id.Dispose();
                                hv_images_id = ExpTmpLocalVar_images_id;
                            }
                        }
                        HOperatorSet.SetDictTuple(hv_samples_sub_Original_em, "image_id", hv_images_id);
                        //更新'image_file_name'
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            HOperatorSet.SetDictTuple(hv_samples_sub_Original_em, "image_file_name",
                                (hv_split_name + "_em.") + hv_images_fotmat);
                        }
                        //更新到samples_copy
                        if (hv_samples_copy == null)
                            hv_samples_copy = new HTuple();
                        hv_samples_copy[hv_images_id - 1] = hv_samples_sub_Original_em;
                    }

                    //********行镜像+对比度******************************************************************
                    if ((int)(new HTuple(hv_data_enhanced.TupleGreater(5))) != 0)
                    {
                        ho_ImageMirror_row_em.Dispose();
                        HOperatorSet.Emphasize(ho_ImageMirror_row, out ho_ImageMirror_row_em, 15,
                            15, 3);
                        //保存图片
                        //镜像后的图片 ->需要重新命名_row
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            HOperatorSet.WriteImage(ho_ImageMirror_row_em, hv_images_fotmat, hv_imags_fill,
                                ((hv_new_images_dir + hv_split_name) + "_row_em.") + hv_images_fotmat);
                        }
                        //更新改动的数据
                        //把samples[Index]写入

                        hv_samples_sub_Row_em.Dispose();
                        HOperatorSet.CopyDict(hv_samples_sub_Row, new HTuple(), new HTuple(), out hv_samples_sub_Row_em);
                        //更新-> 'image_id'
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_images_id = hv_images_id + 1;
                                hv_images_id.Dispose();
                                hv_images_id = ExpTmpLocalVar_images_id;
                            }
                        }
                        HOperatorSet.SetDictTuple(hv_samples_sub_Row_em, "image_id", hv_images_id);
                        //更新'image_file_name'
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            HOperatorSet.SetDictTuple(hv_samples_sub_Row_em, "image_file_name", (hv_split_name + "_row_em.") + hv_images_fotmat);
                        }
                        //更新到samples_copy
                        if (hv_samples_copy == null)
                            hv_samples_copy = new HTuple();
                        hv_samples_copy[hv_images_id - 1] = hv_samples_sub_Row_em;
                    }

                    //********列镜像+对比度******************************************************************
                    if ((int)(new HTuple(hv_data_enhanced.TupleGreater(6))) != 0)
                    {
                        ho_ImageMirror_col_em.Dispose();
                        HOperatorSet.Emphasize(ho_ImageMirror_col, out ho_ImageMirror_col_em, 15,
                            15, 3);
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            HOperatorSet.WriteImage(ho_ImageMirror_col, hv_images_fotmat, hv_imags_fill,
                                ((hv_new_images_dir + hv_split_name) + "_col_em.") + hv_images_fotmat);
                        }

                        hv_samples_sub_Col_em.Dispose();
                        HOperatorSet.CopyDict(hv_samples_sub_Col, new HTuple(), new HTuple(), out hv_samples_sub_Col_em);
                        //更新-> 'image_id'
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_images_id = hv_images_id + 1;
                                hv_images_id.Dispose();
                                hv_images_id = ExpTmpLocalVar_images_id;
                            }
                        }
                        HOperatorSet.SetDictTuple(hv_samples_sub_Col_em, "image_id", hv_images_id);
                        //更新'image_file_name'
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            HOperatorSet.SetDictTuple(hv_samples_sub_Col_em, "image_file_name", (hv_split_name + "_col_em.") + hv_images_fotmat);
                        }
                        //更新到samples_copy
                        if (hv_samples_copy == null)
                            hv_samples_copy = new HTuple();
                        hv_samples_copy[hv_images_id - 1] = hv_samples_sub_Col_em;
                    }
                    //********行列镜像+对比度******************************************************************
                    if ((int)(new HTuple(hv_data_enhanced.TupleGreater(7))) != 0)
                    {
                        ho_ImageMirror_rc_em.Dispose();
                        HOperatorSet.Emphasize(ho_ImageMirror_rc, out ho_ImageMirror_rc_em, 15,
                            15, 3);
                        //保存图片
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            HOperatorSet.WriteImage(ho_ImageMirror_rc_em, hv_images_fotmat, hv_imags_fill,
                                ((hv_new_images_dir + hv_split_name) + "_rc_em.") + hv_images_fotmat);
                        }

                        hv_samples_sub_RC_em.Dispose();
                        HOperatorSet.CopyDict(hv_samples_sub_RC, new HTuple(), new HTuple(), out hv_samples_sub_RC_em);

                        //更新-> 'image_id'
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_images_id = hv_images_id + 1;
                                hv_images_id.Dispose();
                                hv_images_id = ExpTmpLocalVar_images_id;
                            }
                        }
                        HOperatorSet.SetDictTuple(hv_samples_sub_RC_em, "image_id", hv_images_id);
                        //更新'image_file_name'
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            HOperatorSet.SetDictTuple(hv_samples_sub_RC_em, "image_file_name", (hv_split_name + "_rc_em.") + hv_images_fotmat);
                        }
                        //更新到samples_copy
                        if (hv_samples_copy == null)
                            hv_samples_copy = new HTuple();
                        hv_samples_copy[hv_images_id - 1] = hv_samples_sub_RC_em;
                    }
                    //**************弱化图像************************************************************
                    if ((int)(new HTuple(hv_data_enhanced.TupleGreater(8))) != 0)
                    {
                        //对原图/行镜像/列镜像/行列同时镜像 -> 弱化图像（4+4+4）
                        //Image
                        ho_ImageIlluminate.Dispose();
                        HOperatorSet.Illuminate(ho_Image, out ho_ImageIlluminate, 15, 15, 0.6);
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            HOperatorSet.WriteImage(ho_ImageIlluminate, hv_images_fotmat, hv_imags_fill,
                                ((hv_new_images_dir + hv_split_name) + "_ill.") + hv_images_fotmat);
                        }
                        //把samples[Index]写入
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_samples_sub_Original_ill.Dispose();
                            HOperatorSet.CopyDict(hv_samples.TupleSelect(hv_Index), new HTuple(), new HTuple(),
                                out hv_samples_sub_Original_ill);
                        }
                        //更新images_id
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_images_id = hv_images_id + 1;
                                hv_images_id.Dispose();
                                hv_images_id = ExpTmpLocalVar_images_id;
                            }
                        }
                        HOperatorSet.SetDictTuple(hv_samples_sub_Original_ill, "image_id", hv_images_id);
                        //更新'image_file_name'
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            HOperatorSet.SetDictTuple(hv_samples_sub_Original_ill, "image_file_name",
                                (hv_split_name + "_ill.") + hv_images_fotmat);
                        }
                        //更新到samples_copy
                        if (hv_samples_copy == null)
                            hv_samples_copy = new HTuple();
                        hv_samples_copy[hv_images_id - 1] = hv_samples_sub_Original_ill;
                    }
                    //**************行镜像——弱化图像************************************************************
                    if ((int)(new HTuple(hv_data_enhanced.TupleGreater(9))) != 0)
                    {
                        ho_ImageIlluminate_row_ill.Dispose();
                        HOperatorSet.Illuminate(ho_ImageMirror_row, out ho_ImageIlluminate_row_ill,
                            15, 15, 0.6);
                        //保存图片
                        //镜像后的图片 ->需要重新命名_row
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            HOperatorSet.WriteImage(ho_ImageIlluminate_row_ill, hv_images_fotmat, hv_imags_fill,
                                ((hv_new_images_dir + hv_split_name) + "_row_ill.") + hv_images_fotmat);
                        }
                        //更新改动的数据
                        //把samples[Index]写入
                        hv_samples_sub_Row_ill.Dispose();
                        HOperatorSet.CopyDict(hv_samples_sub_Row, new HTuple(), new HTuple(), out hv_samples_sub_Row_ill);
                        //更新-> 'image_id'
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_images_id = hv_images_id + 1;
                                hv_images_id.Dispose();
                                hv_images_id = ExpTmpLocalVar_images_id;
                            }
                        }
                        HOperatorSet.SetDictTuple(hv_samples_sub_Row_ill, "image_id", hv_images_id);
                        //更新'image_file_name'
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            HOperatorSet.SetDictTuple(hv_samples_sub_Row_ill, "image_file_name", (hv_split_name + "_row_ill.") + hv_images_fotmat);
                        }
                        //更新到samples_copy
                        if (hv_samples_copy == null)
                            hv_samples_copy = new HTuple();
                        hv_samples_copy[hv_images_id - 1] = hv_samples_sub_Row_ill;
                    }
                    //**************列镜像——弱化图像************************************************************
                    if ((int)(new HTuple(hv_data_enhanced.TupleGreater(10))) != 0)
                    {
                        ho_ImageIlluminate_col_ill.Dispose();
                        HOperatorSet.Illuminate(ho_ImageMirror_col, out ho_ImageIlluminate_col_ill,
                            15, 15, 0.6);
                        //保存图片
                        //镜像后的图片 ->需要重新命名_row
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            HOperatorSet.WriteImage(ho_ImageIlluminate_col_ill, hv_images_fotmat, hv_imags_fill,
                                ((hv_new_images_dir + hv_split_name) + "_col_ill.") + hv_images_fotmat);
                        }
                        //更新改动的数据
                        //把samples[Index]写入
                        hv_samples_sub_Col_ill.Dispose();
                        HOperatorSet.CopyDict(hv_samples_sub_Col, new HTuple(), new HTuple(), out hv_samples_sub_Col_ill);
                        //更新-> 'image_id'
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_images_id = hv_images_id + 1;
                                hv_images_id.Dispose();
                                hv_images_id = ExpTmpLocalVar_images_id;
                            }
                        }
                        HOperatorSet.SetDictTuple(hv_samples_sub_Col_ill, "image_id", hv_images_id);
                        //更新'image_file_name'
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            HOperatorSet.SetDictTuple(hv_samples_sub_Col_ill, "image_file_name", (hv_split_name + "_col_ill.") + hv_images_fotmat);
                        }
                        //更新到samples_copy
                        if (hv_samples_copy == null)
                            hv_samples_copy = new HTuple();
                        hv_samples_copy[hv_images_id - 1] = hv_samples_sub_Col_ill;
                    }

                    //**************行列镜像——弱化图像***********************************************************
                    if ((int)(new HTuple(hv_data_enhanced.TupleGreater(11))) != 0)
                    {
                        ho_ImageIlluminate_rc_ill.Dispose();
                        HOperatorSet.Illuminate(ho_ImageMirror_rc, out ho_ImageIlluminate_rc_ill,
                            15, 15, 0.6);
                        //保存图片
                        //镜像后的图片 ->需要重新命名_row
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            HOperatorSet.WriteImage(ho_ImageIlluminate_rc_ill, hv_images_fotmat, hv_imags_fill,
                                ((hv_new_images_dir + hv_split_name) + "_rc_ill.") + hv_images_fotmat);
                        }
                        //更新改动的数据
                        //把samples[Index]写入
                        hv_samples_sub_rc_ill.Dispose();
                        HOperatorSet.CopyDict(hv_samples_sub_RC, new HTuple(), new HTuple(), out hv_samples_sub_rc_ill);
                        //更新-> 'image_id'
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_images_id = hv_images_id + 1;
                                hv_images_id.Dispose();
                                hv_images_id = ExpTmpLocalVar_images_id;
                            }
                        }
                        HOperatorSet.SetDictTuple(hv_samples_sub_rc_ill, "image_id", hv_images_id);
                        //更新'image_file_name'
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            HOperatorSet.SetDictTuple(hv_samples_sub_rc_ill, "image_file_name", (hv_split_name + "_rc_ill.") + hv_images_fotmat);
                        }
                        //更新到samples_copy
                        if (hv_samples_copy == null)
                            hv_samples_copy = new HTuple();
                        hv_samples_copy[hv_images_id - 1] = hv_samples_sub_rc_ill;
                    }
                    //**************显示作用************************************************************
                    hv_MSecond1.Dispose(); hv_Second1.Dispose(); hv_Minute1.Dispose(); hv_Hour1.Dispose(); hv_Day1.Dispose(); hv_YDay1.Dispose(); hv_Month1.Dispose(); hv_Year1.Dispose();
                    HOperatorSet.GetSystemTime(out hv_MSecond1, out hv_Second1, out hv_Minute1,
                        out hv_Hour1, out hv_Day1, out hv_YDay1, out hv_Month1, out hv_Year1);
                    //单张耗时
                    hv_single_time.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_single_time = ((hv_Second1 - hv_Second) + ((hv_Minute1 - hv_Minute) * 60)) + ((hv_Hour1 - hv_Hour) * 3600);
                    }
                    //已经耗时
                    hv_time_consuming.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_time_consuming = ((hv_Second1 - hv_Second2) + ((hv_Minute1 - hv_Minute2) * 60)) + ((hv_Hour1 - hv_Hour2) * 3600);
                    }

                    //显示部分
                    HOperatorSet.ClearWindow(hv_WindowHandle);
                    //disp_message(hv_WindowHandle, "管理员：DP", "window", 12, 12, "red", "false");
                    disp_message(hv_WindowHandle, ("数据增强： -> " + hv_data_enhanced) + " 倍", "window", 50, 12, "red", "false");
                    disp_message(hv_WindowHandle, ("进度: " + (((hv_Index + 1.0) / hv_samples_Length) * 100)) + " %", "window", 98, 12, "red", "false");
                    disp_message(hv_WindowHandle, hv_image_file_name, "window", 146, 12, "red", "false");
                    disp_message(hv_WindowHandle, ("已用时间： " + hv_time_consuming) + " s", "window", 192, 12, "red", "false");
                    disp_message(hv_WindowHandle, ("剩余时间：  " + (hv_single_time * ((hv_samples_Length - hv_Index) - 1))) + " s", "window", 240, 12, "red", "false");

                    //**************************************************************************
                }

                //把samples_copy更新到dict_cop
                HOperatorSet.SetDictTuple(hv_dict_copy, "samples", hv_samples_copy);
                //更新images_dir -> new_images_dir := image_dir+'new_images/'
                HOperatorSet.SetDictTuple(hv_dict_copy, "image_dir", hv_new_images_dir);
                disp_message(hv_WindowHandle, "数据增强  ->  已完成", "window", 288, 12, "red", "false");             
                ho_Image.Dispose();
                ho_ImageMirror_row.Dispose();
                ho_ImageMirror_col.Dispose();
                ho_ImageMirror_temp.Dispose();
                ho_ImageMirror_rc.Dispose();
                ho_ImageEmphasize.Dispose();
                ho_ImageMirror_row_em.Dispose();
                ho_ImageMirror_col_em.Dispose();
                ho_ImageMirror_rc_em.Dispose();
                ho_ImageIlluminate.Dispose();
                ho_ImageIlluminate_row_ill.Dispose();
                ho_ImageIlluminate_col_ill.Dispose();
                ho_ImageIlluminate_rc_ill.Dispose();

                hv_WindowHandle_COPY_INP_TMP.Dispose();
                hv_WindowHandle_now.Dispose();
                hv_image_dir.Dispose();
                hv_new_images_dir.Dispose();
                hv_FileExists.Dispose();
                hv_samples.Dispose();
                hv_samples_copy.Dispose();
                hv_samples_Length.Dispose();
                hv_MSecond2.Dispose();
                hv_Second2.Dispose();
                hv_Minute2.Dispose();
                hv_Hour2.Dispose();
                hv_Day2.Dispose();
                hv_YDay2.Dispose();
                hv_Month2.Dispose();
                hv_Year2.Dispose();
                hv_images_id.Dispose();
                hv_Index.Dispose();
                hv_MSecond.Dispose();
                hv_Second.Dispose();
                hv_Minute.Dispose();
                hv_Hour.Dispose();
                hv_Day.Dispose();
                hv_YDay.Dispose();
                hv_Month.Dispose();
                hv_Year.Dispose();
                hv_image_file_name.Dispose();
                hv_split_name.Dispose();
                hv_split_name_Substrings.Dispose();
                hv_sub_Length.Dispose();
                hv_new_sub_dir.Dispose();
                hv_FileExists1.Dispose();
                hv_bbox_label_id.Dispose();
                hv_bbox_row.Dispose();
                hv_bbox_col.Dispose();
                hv_bbox_length1.Dispose();
                hv_bbox_length2.Dispose();
                hv_bbox_phi.Dispose();
                hv_Width.Dispose();
                hv_Height.Dispose();
                hv_samples_sub_Original.Dispose();
                hv_new_row.Dispose();
                hv_new_phi.Dispose();
                hv_samples_sub_Row.Dispose();
                hv_new_col.Dispose();
                hv_samples_sub_Col.Dispose();
                hv_samples_sub_RC.Dispose();
                hv_samples_sub_Original_em.Dispose();
                hv_samples_sub_Row_em.Dispose();
                hv_samples_sub_Col_em.Dispose();
                hv_samples_sub_RC_em.Dispose();
                hv_samples_sub_Original_ill.Dispose();
                hv_samples_sub_Row_ill.Dispose();
                hv_samples_sub_Col_ill.Dispose();
                hv_samples_sub_rc_ill.Dispose();
                hv_MSecond1.Dispose();
                hv_Second1.Dispose();
                hv_Minute1.Dispose();
                hv_Hour1.Dispose();
                hv_Day1.Dispose();
                hv_YDay1.Dispose();
                hv_Month1.Dispose();
                hv_Year1.Dispose();
                hv_single_time.Dispose();
                hv_time_consuming.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_Image.Dispose();
                ho_ImageMirror_row.Dispose();
                ho_ImageMirror_col.Dispose();
                ho_ImageMirror_temp.Dispose();
                ho_ImageMirror_rc.Dispose();
                ho_ImageEmphasize.Dispose();
                ho_ImageMirror_row_em.Dispose();
                ho_ImageMirror_col_em.Dispose();
                ho_ImageMirror_rc_em.Dispose();
                ho_ImageIlluminate.Dispose();
                ho_ImageIlluminate_row_ill.Dispose();
                ho_ImageIlluminate_col_ill.Dispose();
                ho_ImageIlluminate_rc_ill.Dispose();

                hv_WindowHandle_COPY_INP_TMP.Dispose();
                hv_WindowHandle_now.Dispose();
                hv_image_dir.Dispose();
                hv_new_images_dir.Dispose();
                hv_FileExists.Dispose();
                hv_samples.Dispose();
                hv_samples_copy.Dispose();
                hv_samples_Length.Dispose();
                hv_MSecond2.Dispose();
                hv_Second2.Dispose();
                hv_Minute2.Dispose();
                hv_Hour2.Dispose();
                hv_Day2.Dispose();
                hv_YDay2.Dispose();
                hv_Month2.Dispose();
                hv_Year2.Dispose();
                hv_images_id.Dispose();
                hv_Index.Dispose();
                hv_MSecond.Dispose();
                hv_Second.Dispose();
                hv_Minute.Dispose();
                hv_Hour.Dispose();
                hv_Day.Dispose();
                hv_YDay.Dispose();
                hv_Month.Dispose();
                hv_Year.Dispose();
                hv_image_file_name.Dispose();
                hv_split_name.Dispose();
                hv_split_name_Substrings.Dispose();
                hv_sub_Length.Dispose();
                hv_new_sub_dir.Dispose();
                hv_FileExists1.Dispose();
                hv_bbox_label_id.Dispose();
                hv_bbox_row.Dispose();
                hv_bbox_col.Dispose();
                hv_bbox_length1.Dispose();
                hv_bbox_length2.Dispose();
                hv_bbox_phi.Dispose();
                hv_Width.Dispose();
                hv_Height.Dispose();
                hv_samples_sub_Original.Dispose();
                hv_new_row.Dispose();
                hv_new_phi.Dispose();
                hv_samples_sub_Row.Dispose();
                hv_new_col.Dispose();
                hv_samples_sub_Col.Dispose();
                hv_samples_sub_RC.Dispose();
                hv_samples_sub_Original_em.Dispose();
                hv_samples_sub_Row_em.Dispose();
                hv_samples_sub_Col_em.Dispose();
                hv_samples_sub_RC_em.Dispose();
                hv_samples_sub_Original_ill.Dispose();
                hv_samples_sub_Row_ill.Dispose();
                hv_samples_sub_Col_ill.Dispose();
                hv_samples_sub_rc_ill.Dispose();
                hv_MSecond1.Dispose();
                hv_Second1.Dispose();
                hv_Minute1.Dispose();
                hv_Hour1.Dispose();
                hv_Day1.Dispose();
                hv_YDay1.Dispose();
                hv_Month1.Dispose();
                hv_Year1.Dispose();
                hv_single_time.Dispose();
                hv_time_consuming.Dispose();

                MessageBox.Show(HDevExpDefaultException.ToString());
                //throw HDevExpDefaultException;
            }        
        }
        #endregion

        #region disp_message
        // Procedures 
        // Chapter: Graphics / Text
        // Short Description: This procedure writes a text message. 
        public void disp_message(HTuple hv_WindowHandle, HTuple hv_String, HTuple hv_CoordSystem, HTuple hv_Row, HTuple hv_Column, HTuple hv_Color, HTuple hv_Box)   
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
                //This procedure displays text in a graphics window.
                //
                //Input parameters:
                //WindowHandle: The WindowHandle of the graphics window, where
                //   the message should be displayed.
                //String: A tuple of strings containing the text messages to be displayed.
                //CoordSystem: If set to 'window', the text position is given
                //   with respect to the window coordinate system.
                //   If set to 'image', image coordinates are used.
                //   (This may be useful in zoomed images.)
                //Row: The row coordinate of the desired text position.
                //   You can pass a single value or a tuple of values.
                //   See the explanation below.
                //   Default: 12.
                //Column: The column coordinate of the desired text position.
                //   You can pass a single value or a tuple of values.
                //   See the explanation below.
                //   Default: 12.
                //Color: defines the color of the text as string.
                //   If set to [] or '' the currently set color is used.
                //   If a tuple of strings is passed, the colors are used cyclically
                //   for every text position defined by Row and Column,
                //   or every new text line in case of |Row| == |Column| == 1.
                //Box: A tuple controlling a possible box surrounding the text.
                //   Its entries:
                //   - Box[0]: Controls the box and its color. Possible values:
                //     -- 'true' (Default): An orange box is displayed.
                //     -- 'false': No box is displayed.
                //     -- color string: A box is displayed in the given color, e.g., 'white', '#FF00CC'.
                //   - Box[1] (Optional): Controls the shadow of the box. Possible values:
                //     -- 'true' (Default): A shadow is displayed in
                //               darker orange if Box[0] is not a color and in 'white' otherwise.
                //     -- 'false': No shadow is displayed.
                //     -- color string: A shadow is displayed in the given color, e.g., 'white', '#FF00CC'.
                //
                //It is possible to display multiple text strings in a single call.
                //In this case, some restrictions apply on the
                //parameters String, Row, and Column:
                //They can only have either 1 entry or n entries.
                //Behavior in the different cases:
                //   - Multiple text positions are specified, i.e.,
                //       - |Row| == n, |Column| == n
                //       - |Row| == n, |Column| == 1
                //       - |Row| == 1, |Column| == n
                //     In this case we distinguish:
                //       - |String| == n: Each element of String is displayed
                //                        at the corresponding position.
                //       - |String| == 1: String is displayed n times
                //                        at the corresponding positions.
                //   - Exactly one text position is specified,
                //      i.e., |Row| == |Column| == 1:
                //      Each element of String is display in a new textline.
                //
                //
                //Convert the parameters for disp_text.
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
                //
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

        #region set_display_font
        // Chapter: Graphics / Text
        // Short Description: Set font independent of OS 
        public void set_display_font(HTuple hv_WindowHandle, HTuple hv_Size, HTuple hv_Font, HTuple hv_Bold, HTuple hv_Slant)          
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

    }
}

